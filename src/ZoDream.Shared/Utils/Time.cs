using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Utils
{
    public static class Time
    {
        static readonly int[] TIME_SPLIT = new int[] { 60, 60, 60, 24, 30 };

        public static string Format(double val, bool isSecond = true, int minLength = 0)
        {
            if (!isSecond)
            {
                val /= 1000;
            }
            var items = new List<string>
            {
                TwoPad(val % TIME_SPLIT[0])
            };
            var i = 0;
            while (true)
            {
                val = Math.Floor(val / TIME_SPLIT[i++]);
                if (val <= 0)
                {
                    break;
                }
                items.Add(ToFixed(val % TIME_SPLIT[i]));
            }
            for (i = items.Count; i < minLength; i++)
            {
                items.Add(ToFixed(0));
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
            return $"{ToFixed(val / 60)}:{ToFixed(val % 60)}";
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
            var baseTime = 1;
            var j = 0;
            for (int i = args.Length - 1; i >= 0; i--,j++)
            {
                if (j > 0)
                {
                    baseTime *= TIME_SPLIT[j - 1];
                }
                count += Convert.ToDouble(args[i]) * baseTime;
            }
            if (!isSecond && args.Length > 1)
            {
                count *= 1000;
            }
            return count;
        }
    }
}
