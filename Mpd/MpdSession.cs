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
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public class MpdSession
    {
        private NetworkStream _mpdStream = null;
        public TcpClient _tcpConnection;
        private int _tcpTimeout = 5000;
        public bool Terminating = false;
        private string _address;
        private int _port;
        public event Action<string> DisplayMessage;
        public Action Connected;
        public Action Authenticate;
        private object obj = new object();


        public MpdSession(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public void Connect()
        {
            while (_tcpConnection == null && !Terminating)
            {
                _tcpConnection = new TcpClient();
                _tcpConnection.SendTimeout = _tcpTimeout;
                _tcpConnection.ReceiveTimeout = _tcpTimeout;
                
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

                if(_tcpConnection != null)
                {
                    Authenticate?.Invoke();
                    Connected?.Invoke();
                    DataContext.MainContext.Instance.MainWindow.MpdConnected = true;
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

                _mpdStream?.Close();
                _mpdStream = null;
                _tcpConnection?.Close();
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
                    lock (obj)
                    {
                        _mpdStream.Write(buffer, 0, buffer.Length);

                        var rawResponse = ReadResponse();
                        var mpdResponse = command.Parse(rawResponse);

                        callback?.Invoke(mpdResponse);
                        DisplayMessage?.Invoke("Idle");
                    }
                    break;
                }
                catch (Exception ex)
                {
                    var reconnected = Reconnect();
                    if(!reconnected) DisplayMessage?.Invoke("Could not reconnect");
                }
            }

        }

        public bool Reconnect()
        {
            var reconnectAttempts = 0;
            while (true && reconnectAttempts < 10)
            {
                if(_tcpConnection?.Client != null)
                {
                    var connected = IsConnected(_tcpConnection.Client);
                    if (connected)
                    {
                        break;
                    }
                }

                reconnectAttempts++;
                Close();
                Connect();

                Thread.Sleep(1000);
            }

            return (reconnectAttempts < 10);
        }

        private bool IsConnected(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                client.Blocking = false;
                client.Send(tmp, 0, 0);

                DataContext.MainContext.Instance.MainWindow.MpdConnected = true;

                return true;
            }
            catch (SocketException e)
            {
                DataContext.MainContext.Instance.MainWindow.MpdConnected = (e.NativeErrorCode.Equals(10035));
                return (e.NativeErrorCode.Equals(10035));
            }
            finally
            {
                client.Blocking = blockingState;
            }
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
                } while (response.AddLine(line));
            }

            return response;
        }
    }
}
