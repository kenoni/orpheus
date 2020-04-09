using Orpheus.Models;
using Orpheus.Mpd.Commands;
using System;
using System.Collections.Generic;

namespace Orpheus.Mpd
{
    public class MpdServerWithCommands : MpdServerDecorator
    {
        public MpdServerWithCommands(MpdServerBase server) : base(server) { }

        public void PlaylistInfo(Action<MpdPlaylist> callback)
        {
            Server.RunCommand("Updating current playlist...", new PlaylistInfoCommand("playlistinfo"), callback);
        }

        public void FileListInfo(Action<MpdFileSystem> callback)
        {
            Server.RunCommand("Fetching file system ...", new ListallCommand("listall"), callback);
        }

        public void PlayId(string songId)
        {
            Server.RunCommand("Playing id...", new OneArgCommand("playid", new[] { songId }));
        }

        public void DeleteId(string songId)
        {
            Server.RunCommand("Deleting id...", new OneArgCommand("deleteid", new[] { songId }));
        }

        public  void AddId(string uri)
        {
            Server.RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri }));
        }

        public  void AddId(string uri, string position)
        {
            Server.RunCommand("Adding id...", new AddIdCommand("addid", new[] { uri, position }));
        }

        public void Status(Action<MpdStatus> callback)
        {
            Server.RunCommand("Fetching status...", new StatusCommand("status"), callback);
        }

        public  void Update()
        {
            Server.RunCommand("Updating music database...", new NoArgCommand("rescan"));
        }

        public  void SeekCur(string time)
        {
            Server.RunCommand("Seek current ...", new OneArgCommand("seekcur", new[] { time }));
        }

        public  void Stats(Action<MpdStats> callback)
        {
            Server.RunCommand("Fetching stats...", new StatsCommand("stats"), callback);
        }

        public  void Outputs(Action<List<MpdOutput>> callback)
        {
            Server.RunCommand("Fetching outputs...", new OutputCommand("outputs"), callback);
        }

        public  void EnableOutput(string outputId)
        {
            Server.RunCommand("Enabling output...", new OneArgCommand("enableoutput", new[] { outputId }), null);
        }
        public  void DisableOutput(string outputId)
        {
            Server.RunCommand("Disabling output...", new OneArgCommand("disableoutput", new[] { outputId }), null);
        }

        public  void Next()
        {
            Server.RunCommand("Next...", new NoArgCommand("next"));
        }

        public  void Previous()
        {
            Server.RunCommand("Previous...", new NoArgCommand("previous"));
        }

        public  void Stop()
        {
            Server.RunCommand("Stop...", new NoArgCommand("stop"));
        }

        public  void Random(string state)
        {
            Server.RunCommand("Toggling random...", new OneArgCommand("random", new[] { state }));
        }

        public  void Repeat(string state)
        {
            Server.RunCommand("Toggling repeat...", new OneArgCommand("repeat", new[] { state }));
        }
    }
}
