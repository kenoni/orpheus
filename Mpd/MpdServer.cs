﻿/*This file is part of Orpheus.

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
using System.ComponentModel;
using System.Threading.Tasks;
using Orpheus.Models;
using Orpheus.Mpd.Commands;
using Orpheus.Properties;
namespace Orpheus.Mpd
{
    public class MpdServer
    {
        private readonly string _address;
        private readonly int _port;
        private static MpdSession _session;
        private  TaskQueue _queue;
        private static MpdServer _instance;

        private Action _connected;
        private Action<string> _displayMessage;

        private MpdServer(Action<string> displayStatus, Action connectedCallback)
        {
            var mpdAddress = Settings.Default.Mpd_Address.ToMpdAddress();

            _address = mpdAddress[0];
            _port = (mpdAddress.Length > 1) ? Convert.ToInt32(mpdAddress[1]) : 6600 ;

            _queue = new TaskQueue();

            _displayMessage = displayStatus;
            _connected = connectedCallback;

            CreateSession();
        }

        private async Task CreateSession()
        {
            _displayMessage?.Invoke("Connecting...");

            if (_session != null)
            {
                _session.Terminating = true;
            }

            _session = new MpdSession(_address, _port);
            _session.DisplayMessage += _displayMessage;
            _session.Connected += _connected;

            Task initSession = Task.Run(delegate { _session.Connect(); });
            
            await initSession;
        }

        public static void CreateInstance(Action<string> displayStatus, Action connectedCallback)
        {
            _instance = new MpdServer(displayStatus, connectedCallback);
        }

        public static MpdServer Instance => _instance;

        //private void _displayStatus(string message)
        //{
        //    _displayStatusAction?.Invoke(message);
        //}

        private async void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null) {
            if (_session?._tcpConnection != null)
            {
                _displayMessage?.Invoke(message);
                await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(task, callback); }));
            }
        }

        public void PlaylistInfo(Action<MpdPlaylist> callback)
        {
            RunCommand("Updating current playlist...", new PlaylistInfoCommand("playlistinfo"), callback);
        }

        public void FilelistInfo(Action<MpdFileSystem> callback)
        {
            RunCommand("Fetching file system ...", new ListallCommand("listall"), callback);
        }

        public void PlayId(string songId)
        {
            RunCommand("Playing id...", new OneArgCommand("playid", new[] { songId }));
        }

        public void DeleteId(string songId)
        {
            RunCommand("Deleting id...", new OneArgCommand("deleteid", new[] { songId }));
        }

        public void AddId(string uri)
        {
            RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri }));
        }

        public void AddId(string uri, string position)
        {
            RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri, position }));
        }

        public void Status(Action<MpdStatus> callback)
        {
            RunCommand("Fetching status...", new StatusCommand("status"), callback);
        }

        public void Update()
        {
            RunCommand("Updating music database...", new NoArgCommand("update"));
        }

        public void SeekCur(string time)
        {
            RunCommand("Seek current ...", new OneArgCommand("seekcur", new[] { time }));
        }

        public void Stats(Action<MpdStats> callback)
        {
            RunCommand("Fetching stats...", new StatsCommand("stats"), callback);
        }

        public void Outputs(Action<List<MpdOutput>> callback)
        {
            RunCommand("Fetching outputs...", new OutputCommand("outputs"), callback);
        }

        public void EnableOutput(string outputId)
        {
            RunCommand("Enabling output...", new OneArgCommand("enableoutput", new[] { outputId }), null);
        }
        public void DisableOutput(string outputId)
        {
            RunCommand("Disabling output...", new OneArgCommand("disableoutput", new[] { outputId }), null);
        }

        public void Next()
        {
            RunCommand("Next...", new NoArgCommand("next"));
        }

        public void Previous()
        {
            RunCommand("Previous...", new NoArgCommand("previous"));
        }

        public void Stop()
        {
            RunCommand("Stop...", new NoArgCommand("stop"));
        }

        //public DisableOutput
        public string ConnectionAsString
        {
            get =>  $"{_address}:{_port}"; 
        }
        
    }
}
