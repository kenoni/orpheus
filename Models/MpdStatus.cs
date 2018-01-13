using System.Collections.Generic;

namespace Orpheus.Models
{
    public class MpdStatus
    {
        public string State { get; set; }
        public int Duration { get; set; }
        public int Elapsed { get; set; }
        public string SongId { get; set; }
    }
}