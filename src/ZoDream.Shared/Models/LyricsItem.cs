using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class LyricsItem
    {
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// 翻译
        /// </summary>
        public string Translation { get; set; } = string.Empty;
        /// <summary>
        /// 音译
        /// </summary>
        public string Transcription { get; set; } = string.Empty;

        /// <summary>
        /// 距离开始的毫秒数
        /// </summary>
        public int Offset { get; set; } = -1;
        /// <summary>
        /// 持续时间毫秒数
        /// </summary>
        public int Duration { get; set; }

        public LyricsTag Tag { get; set; } = LyricsTag.None;

        public List<LyricsWordItem> WordItems = new();

        /// <summary>
        /// 获取进度在歌词中的位置
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns>占比.1</returns>
        public double GetOffset(double currentTime)
        {
            if (!IsActive(currentTime))
            {
                return 0;
            }
            var progress = currentTime - Offset;
            if (WordItems.Count < 1)
            {
                return progress / Duration;
            }
            for (int i = WordItems.Count - 1; i >= 0; i--)
            {
                var item = WordItems[i];
                if (progress < item.Offset)
                {
                    continue;
                }
                if (progress >= item.Offset + item.Duration)
                {
                    return (item.Index + item.Length) / Text.Length;
                }
                return (item.Index + (progress - item.Offset) / item.Duration * item.Length) / Text.Length;
            }
            return 0;
        }

        public bool IsActive(double currentTime)
        {
            return currentTime >= Offset && currentTime <= Offset + Duration;
        }

        public LyricsItem()
        {

        }

        public LyricsItem(string text)
        {
            Text = text;
        }
    }

    public enum LyricsTag
    {
        None,
        M,
        F,
        D
    }
}
