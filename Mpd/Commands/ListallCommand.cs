using Orpheus.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Orpheus.Helpers;

namespace Orpheus.Mpd.Commands
{
    class ListallCommand : IMpdCommand<MpdFileSystem>
    {

        public string Command { get; set; }

        public MpdFileSystem Response { get; set; }

        private readonly Regex _fileRegex = new Regex(@"^file: (([\s\S]*)\/(.*))", RegexOptions.IgnoreCase);
        private readonly Regex _folderRegex = new Regex(@"^directory: ([\s\S]*)", RegexOptions.IgnoreCase);

        public ListallCommand(string command)
        {
            Command = command;
            Response = new MpdFileSystem { Items = new List<ITreeItem<MpdFile>>() };
        }

        public MpdFileSystem Parse(MpdResponse response)
        {
            foreach(var line in response.ResponseLines.ToList())
            {
                var matchFolder = _folderRegex.Match(line);
                if (matchFolder.Success)
                {
                    var item = new MpdFile
                    {
                        Type = MpdFileType.Folder,
                        Uri = matchFolder.Groups[1].Value,
                        Name = matchFolder.Groups[1].Value
                    };
                    Response.Items.Add(item);
                    continue;
                }

                var matchFile = _fileRegex.Match(line);
                if (matchFile.Success)
                {
                    var item = new MpdFile
                    {
                        Type = MpdFileType.File,
                        Uri = matchFile.Groups[1].Value,
                        Name = matchFile.Groups[3].Value
                    };

                    Response.Items.Add(item);
                }
            }
            var treeBuilder = new TreeBuilder<MpdFile>(Response.Items, '/');

            Response.Items = treeBuilder.BuildTree();

            return Response;
        }
    }
}
