using System.Linq;
using System.Text.RegularExpressions;
using Orpheus.Models;

namespace Orpheus.Mpd.Commands
{
    class StatusCommand : IMpdCommand<MpdStatus>
    {
        public string Command { get; set; }

        public MpdStatus Response { get; set; }
        
        private readonly Regex _durationRegex = new Regex(@"^time: ([\d]+)\:([\d]+)", RegexOptions.IgnoreCase);
        private readonly Regex _elapsedRegex = new Regex(@"^elapsed: ([\d]+)\.([\d]+)", RegexOptions.IgnoreCase);
        private readonly Regex _stateRegex = new Regex(@"^state: ([\S\s]+)", RegexOptions.IgnoreCase);
        private readonly Regex _songidRegex = new Regex(@"^songid: ([\d]+)", RegexOptions.IgnoreCase);


        public StatusCommand(string command)
        {
            Command = command;
            Response = new MpdStatus();
        }

        public MpdStatus Parse(MpdResponse response)
        {
            Response = new MpdStatus();
            response.Lines.ToList().ForEach(line =>
                {
                    var matchDuration = _durationRegex.Match(line);
                    var matchState = _stateRegex.Match(line);
                    var matchElapsed = _elapsedRegex.Match(line);
                    var matchSongId = _songidRegex.Match(line);

                    if (matchDuration.Success)
                    {
                        Response.Duration = int.Parse(matchDuration.Groups[2].Value);
                    }

                    if (matchState.Success)
                    {
                        Response.State = matchState.Groups[1].Value;
                    }

                    if (matchElapsed.Success)
                    {
                        Response.Elapsed = int.Parse(matchElapsed.Groups[1].Value);
                    }

                    if (matchSongId.Success)
                    {
                        Response.SongId =matchSongId.Groups[1].Value;
                    }
                }
            );

            return Response;
        }
    }
}