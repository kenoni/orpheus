using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orpheus.DataContext;
using Orpheus.Models;

namespace Orpheus.AppData
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AppDataContent
    {
        private MainWindowDataContext _dataContext;

        public AppDataContent(MainWindowDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void UpdateContextStreams(IList<PlayerStream> streams)
        {
            if(streams != null)
            {
                _dataContext.PlayerStreams = streams.Select(x => new PlayerStream { Name = $"{x.Name} [{x.Url}]", Url = x.Url }).ToList();
            }
        }

        public IList<PlayerStream> StreamsFromContext => _dataContext.PlayerStreams;

        [JsonProperty]
        public IList<PlayerStream> Streams;
            
    }
}
