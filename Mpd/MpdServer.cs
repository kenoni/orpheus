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

        public async void PlaylistInfo(Action<MpdPlaylist> callback)
        {
            _displayStatus("Updating current playlist...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new PlaylistInfoCommand("playlistinfo"), callback); }));
        }

        public async void FilelistInfo(Action<MpdFileSystem> callback)
        {
            _displayStatus("Fetching file system ...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new ListallCommand("listall"), callback); }));
        }

        public async void PlayId(string songId)
        {
            _displayStatus("Playing id...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new PlayIdCommand("playid", new[] { songId }), null); }));
        }

        public async void DeleteId(string songId)
        {
            _displayStatus("Deleting id...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new DeleteIdCommand("deleteid", new[] { songId }), null); }));
        }

        public async void AddId(string uri)
        {
            _displayStatus("Adding id...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new AddIdCommand("addid", new[] { uri }), null); }));
        }

        public async void AddId(string uri, string position)
        {
            _displayStatus("Adding id...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new AddIdCommand("addid", new[] { uri, position }), null); }));
        }

        public async void Status(Action<MpdStatus> callback)
        {
            _displayStatus("Fetching status...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new StatusCommand("status"), callback); }));
        }

        public async void Update()
        {
            _displayStatus("Updating music database...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new UpdateCommand("update"), null); }));
        }

        public async void SeekCur(string time)
        {
            _displayStatus("Seek current ...");
            await _queue.Enqueue(() => Task.Run(delegate { _session.SendCommand(new SeekCurrCommand("seekcur", new[] { time }), null); }));
        }

        public string ConnectionAsString
        {
            get =>  $"{_address}:{_port}"; 
        }
        
    }
}
