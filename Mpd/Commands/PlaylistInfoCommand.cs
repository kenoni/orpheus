using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Orpheus.Models;

namespace Orpheus.Mpd.Commands
{
    class PlaylistInfoCommand : IMpdCommand<MpdPlaylist>
    {
        public string Command { get; set; }

        public MpdPlaylist Response { get; set; }
        
        private readonly Regex _fileRegex = new Regex(@"^file: ([\s\S]*\/(.*))", RegexOptions.IgnoreCase);
        private readonly Regex _idRegex = new Regex(@"^id: ([\d]+)", RegexOptions.IgnoreCase);

        public PlaylistInfoCommand(string command)
        {
            Command = command;
            Response = new MpdPlaylist {Items = new List<MpdPlaylistEntry>()};
        }

        public MpdPlaylist Parse(MpdResponse response)
        {
            MpdPlaylistEntry item = null;
            response.Lines.ToList().ForEach(line =>
                {
                    var matchFile = _fileRegex.Match(line);
                    var matchFileId = _idRegex.Match(line);

                    if (matchFile.Success)
                    {
                        item = new MpdPlaylistEntry { Name = matchFile.Groups[2].Value
                                                    , Path = matchFile.Groups[1].Value
                                                    , Type = MpdPlaylistEntryType.File
                                                    , IsCurrentlyPlaying = false }; 
                    }

                    if (matchFileId.Success && item != null)
                    {
                        item.Id = matchFileId.Groups[1].Value;
                        item.Name = item.Name;
                        Response.Items.Add(item);
                    }
                }
            );

            return Response;
        }
    }
}
