using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Players;

namespace ZoDream.Shared.Models
{
    public class AppOption
    {
        public bool InfoVisible { get; set; }

        public bool TitleRoll { get; set; } = true;

        public LoopMode Mode { get; set; } = LoopMode.None;

        public int Volume { get; set; } = 100;

        public bool SpectrumVisible { get; set; } = true;

        public SpectrumType SpectrumType { get; set; } = SpectrumType.Columnar;

        public string SpectrumColor { get; set; } = "#333";

        public bool LyricsVisible { get; set; } = true;

        public bool DesktopLyricsVisible { get; set; }

        public bool DesktopLyricsTop { get; set; }

        public bool DesktopLyricsPin { get; set; }

        public int DesktopLyricsOpacity { get; set; } = 100;

        public double DesktopLyricsFontSize { get; set; } = 30;

        public string LyricsFromColor { get; set; } = "#000";

        public string LyricsToColor { get; set; } = "#ff0000";

        public double LyricsFontSize { get; set; } = 16;

        public string LyricsFontFamily { get; set; } = string.Empty;

        public double LyricsActiveFontSize { get; set; } = 20;

        public List<string> PluginItems { get; set; } = new();

    }
}
