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
using System.Threading.Tasks;
using Orpheus.Commands;
using Orpheus.Models;
using Orpheus.Properties;
using static Orpheus.Mpd.MpdHelper;
namespace Orpheus.Mpd
{
    public class MpdServer
    {
        private readonly string _address;
        private readonly int _port;
        private MpdSession _session;
        private static TaskQueue _queue;
        public Action<string> DisplayStatus { get; set; }

        public MpdServer()
        {
            var mpdAddress = Settings.Default.Mpd_Address.ToMpdAddress();

            _address = mpdAddress[0];
            _port = (mpdAddress.Length > 1) ? Convert.ToInt32(mpdAddress[1]) : 6600 ;

            _session = new MpdSession(_address, _port);
            _session.DisplayStatus += _displayStatus;
            _queue = new TaskQueue();
        }

        private void _displayStatus(string message)
        {
            DisplayStatus?.Invoke(message);
        }

        private  void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback) {
            _displayStatus(message);
             _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand<T>(task, callback); }));
        }

        public void PlaylistInfo(Action<MpdPlaylist> callback)
        {
            RunCommand("Updating current playlist...", new PlaylistInfoCommand("playlistinfo"), callback);
        }

        public async void FilelistInfo(Action<MpdFileSystem> callback)
        {
            RunCommand("Fetching file system ...", new ListallCommand("listall"), callback);
        }

        public async void PlayId(string songId)
        {
            RunCommand("Playing id...", new PlayIdCommand("playid", new[] { songId }), null);
        }

        public async void DeleteId(string songId)
        {
            RunCommand("Deleting id...", new DeleteIdCommand("deleteid", new[] { songId }), null);
        }

        public async void AddId(string uri)
        {
            RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri }), null);
        }

        public async void AddId(string uri, string position)
        {
            RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri, position }), null);
        }

        public async void Status(Action<MpdStatus> callback)
        {
            RunCommand("Fetching status...", new StatusCommand("status"), callback);
        }

        public async void Update()
        {
            RunCommand("Updating music database...", new UpdateCommand("update"), null);
        }

        public async void SeekCur(string time)
        {
            RunCommand("Seek current ...", new SeekCurrCommand("seekcur", new[] { time }), null);
        }

        public string ConnectionAsString
        {
            get =>  $"{_address}:{_port}"; 
        }
        
    }
}
