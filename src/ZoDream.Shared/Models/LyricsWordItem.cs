using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class LyricsWordItem
    {
        /// <summary>
        /// 字符的开始位置
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 多少个字符
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 跟本行开始的间隔毫秒数
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 持续时间毫秒数
        /// </summary>
        public int Duration { get; set; }
    }
}
