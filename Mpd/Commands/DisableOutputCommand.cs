namespace Orpheus.Mpd.Commands
{
    class DisableOutputCommand : IMpdCommand<string>
    {
        public string Command { get; set; }

        public string Response { get; set; }
        
        public DisableOutputCommand(string command, string[] args)
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