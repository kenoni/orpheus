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
using System.Threading.Tasks;
using Orpheus.Models;
using Orpheus.Mpd.Commands;
using Orpheus.Properties;
namespace Orpheus.Mpd
{
    public class MpdServer : MpdServerBase
    {
        private readonly string _address;
        private readonly int _port;
        private  static  MpdSession _session;
        private Action<string> _displayMessage;
        private Action _connected;
        private Action _authenticate;

        private MpdServer(Action<string> displayStatus, Action connectedCallback)
        {
            var mpdAddress = Settings.Default.Mpd_Address.ToMpdAddress();

            _address = mpdAddress[0];
            _port = (mpdAddress.Length > 1) ? Convert.ToInt32(mpdAddress[1]) : 6600;

            _displayMessage = displayStatus;
            _connected = connectedCallback;
            if (mpdAddress.Length > 2) _authenticate = delegate { RunCommand("Authenticating...", new OneArgCommand("password", new[] { mpdAddress[2] })); };

            CreateSession();
        }

        private async void CreateSession()
        {
            _displayMessage?.Invoke("Connecting...");

            if (_session != null)
            {
                _session.Terminating = true;
            }

            _session = new MpdSession(_address, _port);
            _session.DisplayMessage += _displayMessage;
            _session.Connected += _connected;
            _session.Authenticate += _authenticate;

            Task initSession = Task.Run(() => { _session.Connect(); });

            await initSession;
        }

        public static void CreateInstance(Action<string> displayStatus, Action connectedCallback)
        {
            Instance = new CommandDecorator(new MpdServer(displayStatus, connectedCallback));
        }

        public static MpdServerBase Instance { get; private set; }

        //public DisableOutput
        public override string ConnectionAsString
        {
            get => $"{_address}:{_port}";
        }

        public override void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null)
        {
            if (_session?._tcpConnection != null && _session?._tcpConnection?.Connected != false)
            {
                _displayMessage?.Invoke(message);
                Task.Factory.StartNew(() => _session.SendCommand(task, callback));
            }
        }

    }
}
