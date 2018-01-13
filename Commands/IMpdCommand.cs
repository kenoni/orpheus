using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orpheus.Mpd
{
    interface IMpdCommand <T>
    {
        string Command { get; set; }
        T Response { get; set; }

        T Parse(MpdResponse response);
    }
}
