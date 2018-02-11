using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Orpheus.Models;
namespace Orpheus.Mpd.Commands
{

    class OutputCommand : IMpdCommand<List<MpdOutput>>
    {

        public string Command { get; set; }

        public List<MpdOutput> Response { get; set; }

        private readonly Regex _outputIdRegex = new Regex(@"^outputid: ([\d]+)", RegexOptions.IgnoreCase);
        private readonly Regex _outputNameRegex = new Regex(@"^outputname: ([\s\S]+)", RegexOptions.IgnoreCase);
        private readonly Regex _outputEnabledRegex = new Regex(@"^outputenabled: ([\d]+)", RegexOptions.IgnoreCase);

        
        public OutputCommand(string command)
        {
            Command = command;
            Response = new List<MpdOutput>();
        }

        public List<MpdOutput> Parse(MpdResponse response)
        {
            MpdOutput item = null;
            foreach (var line in response.ResponseLines.ToList())
            {
                var matchOutputId = _outputIdRegex.Match(line);
                if (matchOutputId.Success)
                {
                    item = new MpdOutput { Id = matchOutputId.Groups[1].Value };
                    continue;
                }

                var matchOutputName = _outputNameRegex.Match(line);
                if (matchOutputName.Success)
                {
                    item.Name = matchOutputName.Groups[1].Value;
                    continue;
                }

                var matchOutputEnabled = _outputEnabledRegex.Match(line);
                if (matchOutputEnabled.Success)
                {
                    item.IsSelected = (matchOutputEnabled.Groups[1].Value == "1");
                    Response.Add(item);
                }
            }

            return Response;
        }
    }

}
