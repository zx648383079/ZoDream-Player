using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZoDream.Shared.Controls;

namespace ZoDream.Player.Utils
{
    public static class Util
    {
        public static Brush ToBrush(string color)
        {
            return new SolidColorBrush(ColorHelper.From(color));
        }

        public static FontFamily ToFont(string font)
        {
            if (string.IsNullOrWhiteSpace(font))
            {
                return new FontFamily("Microsoft YaHei");
            }
            return new FontFamily(font);
        }

        public static Visibility ToVisible(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
