/*This file is part of Orpheus.

   Orpheus is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Orpheus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Orpheus.  If not, see<http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orpheus.Models;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    class MpdSession
    {
        private TaskQueue _queue;
        private NetworkStream _mpdStream = null;
        public TcpClient _tcpConnection;
        private int _tcpTimeout = 5000;
        public bool Terminating = false;
        private string _address;
        private int _port;
        public Action<string> DisplayStatus { get; set; }

        public MpdSession(string address, int port)
        {
            _address = address;
            _port = port;
            _queue = new TaskQueue();

            Connect();

        }

        private void Connect()
        {
            while (_tcpConnection == null)
            {
                _tcpConnection = new TcpClient();
                _tcpConnection.SendTimeout = _tcpTimeout;
                _tcpConnection.ReceiveTimeout = _tcpTimeout;

                //m_Parent.OnConnectionStateChanged(ServerSession.SessionState.Connecting);
                //m_Parent.OnActivityChanged("");
                IAsyncResult connectResult = _tcpConnection.BeginConnect(_address, _port, null, null);

                while (!connectResult.IsCompleted && !Terminating)
                {
                    Thread.Sleep(100);
                }

                if (Terminating)
                {
                    _tcpConnection = null;
                }
                else
                {
                    try
                    {
                        _tcpConnection.EndConnect(connectResult);
                        _mpdStream = _tcpConnection.GetStream();
                    }
                    catch
                    {
                        _tcpConnection = null;
                    }
                }
            }
        }

        public void Close()
        {
            if (_tcpConnection != null)
            {
                if (_tcpConnection.Connected)
                {
                   // SendCommand("close").Wait();
                }

                _mpdStream.Close();
                _mpdStream = null;
                _tcpConnection.Close();
                _tcpConnection = null;
            }
        }

        public  void SendCommand<T>(IMpdCommand<T> command, Action<T> callback)
        {
            while (true)
            {
                try
                {
                    var buffer = Encoding.UTF8.GetBytes($"{command.Command}\n");
                     _mpdStream.Write(buffer, 0, buffer.Length);

                    var rawResponse = ReadResponse();
                    var mpdResponse = command.Parse(rawResponse);

                    callback?.Invoke(mpdResponse);
                    DisplayStatus?.Invoke("Idle");

                    break;
                }
                catch (Exception ex)
                {
                    Reconnect();
                }

            }
        }

        public bool Reconnect()
        {
            while (true)
            {
                var connected = _tcpConnection.Client.IsConnected();
                if (connected)
                {
                    break;
                }

                Close();
                Connect();

                Thread.Sleep(1000);
            }

            return true;
        }


        private MpdResponse ReadResponse()
        {
            var response = new MpdResponse();
            using (var reader = new StreamReader(_mpdStream, Encoding.UTF8, true, 512, true))
            {
                string line;
                do
                {
                    line =  reader.ReadLine();
                } while (!response.AddLine(line));
            }

            return response;
        }
    }
}
