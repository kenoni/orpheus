using Orpheus.Mpd;
using System;

namespace Orpheus.Commands
{
    class SeekCurrCommand : IMpdCommand<string>
    {
        public string Command { get; set; }

        public string Response { get; set; }
        
        public SeekCurrCommand(string command, string[] args)
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