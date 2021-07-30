using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Player.Models
{
    public class LyricsItem
    {
        public string Text { get; set; }

        public double StartAt { get; set; }

        public double EndAt { get; set; }

        public double Duration
        {
            get
            {
                return EndAt - StartAt;
            }
        }

        public double GetOffset(double currentTime)
        {
            if (currentTime <= StartAt || currentTime >= EndAt)
            {
                return 0;
            }
            return (currentTime - StartAt) / Duration;
        }

        public bool IsActive(double currentTime)
        {
            return currentTime >= StartAt && currentTime <= EndAt;
        }
    }
}
