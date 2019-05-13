using System.Linq;
using System.Text.RegularExpressions;

namespace Orpheus.Mpd.Commands
{
    class AddIdCommand : IMpdCommand<string>
    {
        private readonly Regex _idRegex = new Regex(@"^id: ([\d]+)", RegexOptions.IgnoreCase);
        public string Command { get; set; }

        public string Response { get; set; }

        public AddIdCommand(string command, string[] args)
        {
            if (args.Length == 1)
            {
                Command = $"{command}  " + Quote(args[0]);
                Response = string.Empty;
            }
            if (args.Length == 2)
            {
                Command = $"{command} " + Quote(args[0]) + " " + args[1];
                Response = string.Empty;
            }
        }

        private static string Quote(string s)
        {
            string intermediate = s.Replace("\\", "\\\\");
            string result = intermediate.Replace("\"", "\\\"");
            return "\"" + result + "\"";
        }

        public string Parse(MpdResponse response)
        {
            response.Lines.ToList().ForEach(line =>
            {
                var matchFileId = _idRegex.Match(line);

                if (matchFileId.Success)
                {
                    Response = matchFileId.Groups[1].Value;
                }
            }
           );

            return Response;
        }
    }
}