using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Player.Models;

namespace ZoDream.Player.Helpers
{
    public class LyricsReader
    {
        /// <summary>
        /// 根据内容格式化歌词
        /// </summary>
        /// <param name="content"></param>
        /// <param name="duration">音乐的总时长</param>
        /// <returns></returns>
        public static IList<LyricsItem> Render(string content, double duration = 0)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            var items = new List<LyricsItem>();
            var lines = content.Split(new char[] { '\r', '\n' });
            foreach (var line in lines)
            {
                var item = From(line);
                items.Add(item);
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].EndAt > 0)
                {
                    continue;
                }
                items[i].EndAt = i < items.Count - 1 ? items[i + 1].StartAt : duration;
            }
            return items;
        }

        /// <summary>
        /// 根据文件格式化歌词
        /// </summary>
        /// <param name="file"></param>
        /// <param name="duration">音乐的总时长</param>
        /// <returns></returns>
        public static IList<LyricsItem> RenderFile(string file, double duration = 0)
        {
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }
            return Render(Local.Read(file));
        }

        public static Task<IList<LyricsItem>> RenderFileAsync(string file, double duration = 0)
        {
            return Task.Factory.StartNew(() =>
            {
                return RenderFile(file, duration);
            });
        }

        /// <summary>
        /// 把歌词组成一行字符串
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns></returns>
        public static string To(LyricsItem lyrics)
        {
            if (lyrics == null)
            {
                return string.Empty;
            }
            if (lyrics.StartAt == lyrics.EndAt && lyrics.EndAt == 0)
            {
                return lyrics.Text;
            }
            if (lyrics.EndAt == 0)
            {
                return $"[{DoubleToTime(lyrics.StartAt)}]{lyrics.Text}";
            }
            return $"[{DoubleToTime(lyrics.StartAt)}-{DoubleToTime(lyrics.EndAt)}]{lyrics.Text}";
        }


        /// <summary>
        /// 把一行转成歌词
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static LyricsItem From(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new LyricsItem() { Text = line};
            }
            var time = line.IndexOf('[');
            var text = line.IndexOf(']', time);
            if (time < 0 || text < 0)
            {
                return new LyricsItem() { Text = line};
            }
            time++;
            var end = .0;
            var start = TimeToQuantum(line.Substring(time, text - time), ref end);
            return new LyricsItem() { StartAt = start, EndAt = end, Text = line.Substring(text + 1).Trim() };
        }

        /// <summary>
        /// 取时间段
        /// </summary>
        /// <param name="line"></param>
        /// <param name="endAt"></param>
        /// <returns></returns>
        private static double TimeToQuantum(string line, ref double endAt)
        {
            var i = line.IndexOf("-");
            var len = 1;
            if (i > 0)
            {
                var e = line.IndexOf('>', i);
                if (e > 0)
                {
                    len = e - i;
                }
            }
            if (i < 0)
            {
                return TimeToDouble(line);
            }
            endAt = TimeToDouble(line.Substring(i + len));
            return TimeToDouble(line.Substring(0, i));
        }

        /// <summary>
        /// 时间转秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static double TimeToDouble(string time)
        {
            var args = time.Split(':');
            var count = .0;
            for (int i = 0; i < args.Length; i++)
            {
                count += Convert.ToDouble(args[i]) * Math.Pow(60, args.Length - i - 1);
            }
            return count;
        }

        private static string DoubleToTime(double second)
        {
            var minute = Math.Floor(second / 60);
            if (minute > 60)
            {
                return toFixed(minute / 60) + ":" + toFixed(minute % 60) + ":" + TwoPad(second % 60);
            }
            return toFixed(minute) + ":" + TwoPad(second % 60);
        }

        private static string toFixed(double v)
        {
            return Math.Floor(v).ToString("00");
        }

        private static string TwoPad(double v)
        {
            var s = string.Format("{0:F}", v);
            return v >= 10 ? s : $"0{s}";
        }
    }
}
