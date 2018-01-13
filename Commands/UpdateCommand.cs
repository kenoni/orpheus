using Orpheus.Mpd;

namespace Orpheus.Commands
{
    class UpdateCommand : IMpdCommand<string>
    {
        public string Command { get; set; }

        public string Response { get; set; }
        
        public UpdateCommand(string command)
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