using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Orpheus.Converters
{
    public class SecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var seconds = double.Parse(value.ToString());
            var time = TimeSpan.FromSeconds(seconds);

            return  (seconds >= 3600) ? time.ToString(@"hh\:mm\:ss") : time.ToString(@"mm\:ss");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string time = value.ToString();
            return TimeSpan.Parse(time);
        }
    }
}
