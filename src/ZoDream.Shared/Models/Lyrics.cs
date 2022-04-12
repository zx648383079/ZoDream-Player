using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class Lyrics
    {
        /// <summary>
        /// 唱片集
        /// </summary>
        public string Album { get; set; } = string.Empty;
        /// <summary>
        /// 演唱者
        /// </summary>
        public string Artist { get; set; } = string.Empty;
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// 此LRC文件的创建者
        /// </summary>
        public string By { get; set; } = string.Empty;
        /// <summary>
        /// 创建此LRC文件的播放器或编辑器
        /// </summary>
        public string Re { get; set; } = string.Empty;
        /// <summary>
        /// 歌词标题
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 程序的版本
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// 偏移量毫秒数
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 歌曲总长毫秒数
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 歌词正文
        /// </summary>
        public List<LyricsItem> Items { get; set; } = new();

        /// <summary>
        /// 根据歌曲的实际时长修正一下
        /// </summary>
        /// <param name="duration"></param>
        public void ApplyDuration(int duration)
        {
            var lastEnd = duration - Offset;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                var item = Items[i];
                if (item.Duration == 0)
                {
                    item.Duration = lastEnd - item.Offset;
                }
                var lastWord = item.Duration;
                for (int j = item.WordItems.Count - 1; j >= 0; j--)
                {
                    var word = item.WordItems[j];
                    if (word.Duration == 0)
                    {
                        word.Duration = lastWord - word.Offset;
                    }
                    lastWord = word.Offset;
                }
                lastEnd = item.Offset;
            }
        }
    }
}
