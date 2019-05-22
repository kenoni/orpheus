using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orpheus.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerStream: BaseStream,INotifyPropertyChanged
    {
             
    }
}
