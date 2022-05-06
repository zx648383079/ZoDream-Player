using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Readers
{
    public class LrcWriter : ILyricsWriter
    {
        public async Task WriteAsync(string file, Lyrics data)
        {
            await WriteFileAsync(file, await WriteToStringAsync(data));
        }

        public Task<string> WriteToStringAsync(Lyrics data)
        {
            return Task.Factory.StartNew(() =>
            {
                return RenderToString(data);
            });
        }

        private string RenderToString(Lyrics data)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(data.Album))
            {
                sb.AppendLine($"[al:{data.Album}]");
            }
            if (!string.IsNullOrWhiteSpace(data.Artist))
            {
                sb.AppendLine($"[ar:{data.Artist}]");
            }
            if (!string.IsNullOrWhiteSpace(data.Author))
            {
                sb.AppendLine($"[au:{data.Author}]");
            }
            if (!string.IsNullOrWhiteSpace(data.By))
            {
                sb.AppendLine($"[by:{data.By}]");
            }
            if (!string.IsNullOrWhiteSpace(data.Re))
            {
                sb.AppendLine($"[re:{data.Re}]");
            }
            if (!string.IsNullOrWhiteSpace(data.Title))
            {
                sb.AppendLine($"[ti:{data.Title}]");
            }
            if (!string.IsNullOrWhiteSpace(data.Version))
            {
                sb.AppendLine($"[ve:{data.Version}]");
            }
            if (data.Offset > 0)
            {
                sb.AppendLine($"[offset:+{data.Offset}]");
            }
            else if (data.Offset < 0)
            {
                sb.AppendLine($"[offset:{data.Offset}]");
            }
            if (data.Duration > 0)
            {
                sb.AppendLine($"[total:{data.Duration}]");
            }
            for (int i = 0; i < data.Items.Count; i++)
            {
                sb.AppendLine(RenderLyricsLine(data.Items[i], (i + 1) >= data.Items.Count ? null : data.Items[i + 1]));
            }
            return sb.ToString();
        }

        private string RenderTagLine(string tag, object val)
        {
            return $"[{tag}:{val}]";
        }

        private string RenderLyricsLine(LyricsItem lyrics, LyricsItem? next = null)
        {
            if (lyrics == null)
            {
                return string.Empty;
            }
            if (lyrics.Offset == -1 && lyrics.Duration == 0)
            {
                return lyrics.Text;
            }
            var text = RenderWord(lyrics);
            switch (lyrics.Tag)
            {
                case LyricsTag.M:
                    text = $"M:{text}";
                    break;
                case LyricsTag.F:
                    text = $"F:{text}";
                    break;
                case LyricsTag.D:
                    text = $"D:{text}";
                    break;
                default:
                    break;
            }
            var time = RenderTime(lyrics, next);
            return $"[{time}]{text}";
        }

        protected virtual string RenderTime(LyricsItem lyrics, LyricsItem? next = null)
        {
            if (lyrics.Duration == 0 || (next != null && next.Offset == lyrics.Offset + lyrics.Duration))
            {
                return RenderTagTime(lyrics.Offset);
            }
            return $"{RenderTagTime(lyrics.Offset)},{RenderTagTime(lyrics.Duration)}";
        }

        protected virtual string RenderWord(LyricsItem lyrics)
        {
            if (lyrics.WordItems.Count < 1)
            {
                return lyrics.Text;
            }
            var sb = new StringBuilder();
            foreach (var item in lyrics.WordItems)
            {
                sb.Append(RenderWordTime(item));
                sb.Append(lyrics.Text.Substring(item.Index, item.Length));
            }
            return sb.ToString();
        }

        protected virtual string RenderTagTime(int val)
        {
            return Time.Format(val);
        }

        protected virtual string RenderWordTime(LyricsWordItem word)
        {
            return $"<{word.Offset},{word.Duration}>";
        }

        protected virtual async Task WriteFileAsync(string file, string content)
        {
            await LocationStorage.WriteAsync(file, content);
        }
    }
}
