using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Readers
{
    public class LrcReader : ILyricsReader
    {
        public async Task<Lyrics?> ReadAsync(string file)
        {
            var content = await ReadFileAsync(file);
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            return ReadFromString(content);
        }

        public Task<Lyrics?> ReadFromStringAsync(string content)
        {
            return Task.Factory.StartNew(() =>
            {
                return ReadFromString(content);
            });
        }

        private Lyrics? ReadFromString(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            var res = new Lyrics();
            var lines = content.Split(new char[] { '\r', '\n' });
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var item = SplitLine(line);
                if (IsTagLine(item.Item1))
                {
                    FormatTagLine(ref res, item.Item1);
                    continue;
                }
                res.Items.Add(FormatLyriceLine(item.Item1, item.Item2));
            }
            return res;
        }

        private Tuple<string, string> SplitLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new Tuple<string, string>(string.Empty, line);
            }
            var time = line.IndexOf('[');
            var text = line.IndexOf(']', time);
            if (time < 0 || text < 0)
            {
                return new Tuple<string, string>(string.Empty, line);
            }
            time++;
            return new Tuple<string, string>(line.Substring(time, text - time).Trim(), line.Substring(text + 1).Trim());
        }

        private bool IsTagLine(string tag)
        {
            return Regex.IsMatch(tag, @"^[a-zA-Z]+:");
        }

        private void FormatTagLine(ref Lyrics lyrics, string line)
        {
            var args = line.Split(new char[] { ':' }, 2);
            var val = args[1].Trim();
            if (string.IsNullOrEmpty(val))
            {
                return;
            }
            switch (args[0].Trim().ToLower())
            {
                case "al":
                    lyrics.Album = val;
                    break;
                case "ar":
                    lyrics.Artist = val;
                    break;
                case "au":
                    lyrics.Author = val;
                    break;
                case "by":
                    lyrics.By = val;
                    break;
                case "re":
                    lyrics.Re = val;
                    break;
                case "ti":
                    lyrics.Title = val;
                    break;
                case "offset":
                    lyrics.Offset = Convert.ToInt32(val);
                    break;
                case "total":
                    lyrics.Duration = Convert.ToInt32(val);
                    break;
                case "ve":
                case "ver":
                    lyrics.Version = val;
                    break;
            }
        }

        private LyricsItem FormatLyriceLine(string tag, string text)
        {
            var tags = FormatLyriceTag(text);
            if (string.IsNullOrWhiteSpace(tag))
            {
                return new LyricsItem(tags.Item1) { Tag = tags.Item2 };
            }
            var res = new LyricsItem() { Tag = tags.Item2 };
            FormatTime(ref res, tag);
            FormatWord(ref res, tags.Item1);
            return res;
        }

        private void FormatTime(ref LyricsItem lyrics, string tag)
        {
            var args = tag.Split(',');
            lyrics.Offset = FormatTagTime(args[0].Trim());
            if (args.Length > 1)
            {
                lyrics.Duration = FormatTagTime(args[1].Trim());
            }
        }

        private void FormatWord(ref LyricsItem lyrics, string text)
        {
            if (!text.Contains("<"))
            {
                lyrics.Text = text;
                return;
            }
            var sb = new StringBuilder();
            var blocks = text.Split(new char[] { '<', '>' });
            if (!string.IsNullOrWhiteSpace(blocks[0]))
            {
                blocks[0] = blocks[0].Trim();
                sb.Append(blocks[0]);
                lyrics.WordItems.Add(new LyricsWordItem() { Length = blocks[0].Length });
            }
            for (int i = 1; i < blocks.Length; i += 2)
            {
                var time = blocks[i].Split(',');
                var val = blocks[i + 1].Trim();
                lyrics.WordItems.Add(new LyricsWordItem()
                {
                    Index = sb.Length,
                    Length = val.Length,
                    Offset = Convert.ToInt32(time[0]),
                    Duration = time.Length > 1 ? Convert.ToInt32(time[1]) : 0
                });
                sb.Append(val);
            }
            // 最后需要补一个持续时间
            lyrics.Text = sb.ToString();
        }



        private Tuple<string, LyricsTag> FormatLyriceTag(string line)
        {
            var i = line.IndexOf(':');
            if (i <= 0)
            {
                return new Tuple<string, LyricsTag>(line, LyricsTag.None);
            }
            var tag = LyricsTag.None;
            switch (line.Substring(0, i).Trim())
            {
                case "D":
                case "d":
                    tag = LyricsTag.D;
                    break;
                case "F":
                case "f":
                    tag = LyricsTag.F;
                    break;
                case "M":
                case "m":
                    tag = LyricsTag.M;
                    break;
            }
            if (tag == LyricsTag.None)
            {
                return new Tuple<string, LyricsTag>(line, LyricsTag.None);
            }
            return new Tuple<string, LyricsTag>(line.Substring(i+1).Trim(), tag);
        }

        /// <summary>
        /// 转化单个标签的时间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual int FormatTagTime(string val)
        {
            return Convert.ToInt32(Time.Render(val, false));
        }
        /// <summary>
        /// 转化单个词的时间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual int FormatWordTime(string val)
        {
            return Convert.ToInt32(Time.Render(val, false));
        }
        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual async Task<string> ReadFileAsync(string file)
        {
            return await File.ReadAsync(file);
        }
    }
}
