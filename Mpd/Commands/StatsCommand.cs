using System.Linq;
using System.Text.RegularExpressions;
using Orpheus.Models;

namespace Orpheus.Mpd.Commands
{
    class StatsCommand : IMpdCommand<MpdStats>
    {
        public string Command { get; set; }

        public MpdStats Response { get; set; }
        
        private readonly Regex _updateTimenRegex = new Regex(@"^db_update: ([\d]+)", RegexOptions.IgnoreCase);

        public StatsCommand(string command)
        {
            Command = command;
            Response = new MpdStats();
        }

        public MpdStats Parse(MpdResponse response)
        {
            Response = new MpdStats();
            response.ResponseLines.ToList().ForEach(line =>
                {
                    var matchDuration = _updateTimenRegex.Match(line);

                    if (matchDuration.Success)
                    {
                        Response.UpdateTime = decimal.Parse(matchDuration.Groups[1].Value);
                    }
                }
            );

            return Response;
        }
    }
}