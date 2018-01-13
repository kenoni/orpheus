using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orpheus
{
    public static class Extensions
    {
       public static int ToInt(this string value)
        {
            Int32.TryParse(value, out int intValue);
            return intValue;
        }
    }
}
