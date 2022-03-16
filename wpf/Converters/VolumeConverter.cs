using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ZoDream.Player.Converters
{
    public class VolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var volume = (double)value;
            if (volume < 0)
            {
                return "\uE74F";
            }
            if (volume < 1)
            {
                return "\uE992";
            }
            if (volume < 50)
            {
                return "\uE993";
            }
            if (volume < 90)
            {
                return "\uE994";
            }
            return "\uE15D";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
