using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Utils
{
    public static class Time
    {
        public static string Format(double val, bool isSecond = true)
        {
            if (!isSecond)
            {
                val /= 1000;
            }
            var items = new List<string>
            {
                TwoPad(val % 60)
            };
            while (true)
            {
                val = Math.Floor(val / 60);
                if (val <= 0)
                {
                    break;
                }
                items.Add(ToFixed(val % 60));
            }
            items.Reverse();
            return string.Join(":", items);
        }

        public static string MinuteFormat(double val, bool isSecond = true)
        {
            if (!isSecond)
            {
                val /= 1000;
            }
            return $"{TwoPad(val / 60)}:{TwoPad(val % 60)}";
        }

        private static string ToFixed(double v)
        {
            return Math.Floor(v).ToString("00");
        }

        private static string TwoPad(double v)
        {
            var s = string.Format("{0:F}", v);
            return v >= 10 ? s : $"0{s}";
        }

        public static double Render(string val, bool isSecond = true)
        {
            var args = val.Split(':');
            var count = .0;
            for (int i = 0; i < args.Length; i++)
            {
                count += Convert.ToDouble(args[i]) * Math.Pow(60, args.Length - i - 1);
            }
            if (!isSecond && args.Length > 1)
            {
                count *= 1000;
            }
            return count;
        }
    }
}
