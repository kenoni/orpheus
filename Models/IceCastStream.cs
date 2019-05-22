using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orpheus.Models
{
    public class IceCastStream : BaseStream
    {
        public string ServerName { get; set; }
        public string ListenUrl { get; set; }
        public string ServerType { get; set; }
        public string Bitrate { get; set; }
        public string Channels { get; set; }
        public string SampleRate { get; set; }
        public string Genre { get; set; }
        public string CurrentSong { get; set; }
    }
}
