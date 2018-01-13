using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Orpheus.Models;
using Orpheus.Mpd;
using System;

namespace Orpheus.Commands
{
    class PlayIdCommand : IMpdCommand<string>
    {
        public string Command { get; set; }

        public string Response { get; set; }
        
        public PlayIdCommand(string command, string[] args)
        {
            Command = $"{command}  {args[0]}";
            Response = string.Empty;
        }

        public string Parse(MpdResponse response)
        {
            return string.Empty;
        }
    }
}