using Orpheus.Models;
using Orpheus.Mpd.Commands;
using System;
using System.Collections.Generic;

namespace Orpheus.Mpd
{
    class CommandDecorator : MpdServerDecorator
    {
        public CommandDecorator(MpdServerBase server) : base(server) { }

        public override void PlaylistInfo(Action<MpdPlaylist> callback)
        {
            _server.RunCommand("Updating current playlist...", new PlaylistInfoCommand("playlistinfo"), callback);
        }

        public override void FilelistInfo(Action<MpdFileSystem> callback)
        {
            _server.RunCommand("Fetching file system ...", new ListallCommand("listall"), callback);
        }

        public override void PlayId(string songId)
        {
            _server.RunCommand("Playing id...", new OneArgCommand("playid", new[] { songId }));
        }

        public override void DeleteId(string songId)
        {
            _server.RunCommand("Deleting id...", new OneArgCommand("deleteid", new[] { songId }));
        }

        public override void AddId(string uri)
        {
            _server.RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri }));
        }

        public override void AddId(string uri, string position)
        {
            _server.RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri, position }));
        }

        public override void Status(Action<MpdStatus> callback)
        {
            _server.RunCommand("Fetching status...", new StatusCommand("status"), callback);
        }

        public override void Update()
        {
            _server.RunCommand("Updating music database...", new NoArgCommand("rescan"));
        }

        public override void SeekCur(string time)
        {
            _server.RunCommand("Seek current ...", new OneArgCommand("seekcur", new[] { time }));
        }

        public override void Stats(Action<MpdStats> callback)
        {
            _server.RunCommand("Fetching stats...", new StatsCommand("stats"), callback);
        }

        public override void Outputs(Action<List<MpdOutput>> callback)
        {
            _server.RunCommand("Fetching outputs...", new OutputCommand("outputs"), callback);
        }

        public override void EnableOutput(string outputId)
        {
            _server.RunCommand("Enabling output...", new OneArgCommand("enableoutput", new[] { outputId }), null);
        }
        public override void DisableOutput(string outputId)
        {
            _server.RunCommand("Disabling output...", new OneArgCommand("disableoutput", new[] { outputId }), null);
        }

        public override void Next()
        {
            _server.RunCommand("Next...", new NoArgCommand("next"));
        }

        public override void Previous()
        {
            _server.RunCommand("Previous...", new NoArgCommand("previous"));
        }

        public override void Stop()
        {
            _server.RunCommand("Stop...", new NoArgCommand("stop"));
        }

        public override void Random(string state)
        {
            _server.RunCommand("Toggling random...", new OneArgCommand("random", new[] { state }), null);
        }

        public override void Repeat(string state)
        {
            _server.RunCommand("Toggling repeat...", new OneArgCommand("repeat", new[] { state }), null);
        }
    }
}
