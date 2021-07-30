using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ZoDream.Player.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var second = (double)value;
            var minute = Math.Floor(second / 60);
            if (minute > 60)
            {
                return toFixed(minute / 60) + ":" + toFixed(minute % 60) + ":" + toFixed(second % 60);
            }
            return toFixed(minute) + ":" + toFixed(second % 60); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string toFixed(double v)
        {
            return Math.Floor(v).ToString("00");
        }
    }
}
