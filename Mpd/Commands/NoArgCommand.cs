using Orpheus.Mpd;

namespace Orpheus.Mpd.Commands
{
    class NoArgCommand : IMpdCommand<string>
    {
        public string Command { get; set; }

        public string Response { get; set; }
        
        public NoArgCommand(string command)
        {
            Command = $"{command}";
            Response = string.Empty;
        }

        public string Parse(MpdResponse response)
        {
            return string.Empty;
        }
    }
}