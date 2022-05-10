using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ZoDream.Shared.Models;
using ZoDream.Shared.Players;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Player.ViewModels
{
    public class SettingViewModel : BindableBase
    {
        public SettingViewModel(): this(new AppOption())
        {
        }

        public SettingViewModel(AppOption option)
        {
            Source = option;
            InfoVisible = Source.InfoVisible;
            TitleRoll = Source.TitleRoll;
            Mode = Source.Mode;
            Volume = Source.Volume;
            SpectrumVisible = Source.SpectrumVisible;
            SpectrumColor = Source.SpectrumColor;
            SpectrumType = Source.SpectrumType;
            LyricsVisible = Source.LyricsVisible;
            LyricsToColor = Source.LyricsToColor;
            LyricsActiveFontSize = Source.LyricsActiveFontSize;
            LyricsFontFamily = Source.LyricsFontFamily;
            LyricsFontSize = Source.LyricsFontSize;
            LyricsFromColor = Source.LyricsFromColor;
            DesktopLyricsOpacity = Source.DesktopLyricsOpacity;
            DesktopLyricsPin = Source.DesktopLyricsPin;
            DesktopLyricsTop = Source.DesktopLyricsTop;
            DesktopLyricsVisible = Source.DesktopLyricsVisible;
            DesktopLyricsFontSize = Source.DesktopLyricsFontSize;
        }

        private readonly AppOption Source;

        private Array modeItems = Enum.GetValues(typeof(LoopMode));

        public Array ModeItems
        {
            get => modeItems;
            set => Set(ref modeItems, value);
        }

        private Array spectrumItems = Enum.GetValues(typeof(SpectrumType));

        public Array SpectrumItems
        {
            get => spectrumItems;
            set => Set(ref spectrumItems, value);
        }



        private string[] fontItems = Fonts.SystemFontFamilies.Select(i => i.Source).ToArray();

        public string[] FontItems
        {
            get => fontItems;
            set => Set(ref fontItems, value);
        }

        private bool infoVisible;

        public bool InfoVisible
        {
            get => infoVisible;
            set => Set(ref infoVisible, value);
        }

        private bool titleRoll;

        public bool TitleRoll
        {
            get => titleRoll;
            set => Set(ref titleRoll, value);
        }


        private LoopMode mode = LoopMode.None;

        public LoopMode Mode
        {
            get => mode;
            set => Set(ref mode, value);
        }

        private int volume;

        public int Volume
        {
            get => volume;
            set => Set(ref volume, value);
        }

        private bool spectrumVisible;

        public bool SpectrumVisible
        {
            get => spectrumVisible;
            set => Set(ref spectrumVisible, value);
        }

        private SpectrumType spectrumType;

        public SpectrumType SpectrumType
        {
            get => spectrumType;
            set => Set(ref spectrumType, value);
        }

        private string spectrumColor = string.Empty;

        public string SpectrumColor
        {
            get => spectrumColor;
            set => Set(ref spectrumColor, value);
        }

        private bool lyricsVisible;

        public bool LyricsVisible
        {
            get => lyricsVisible;
            set => Set(ref lyricsVisible, value);
        }

        private bool desktopLyricsVisible;

        public bool DesktopLyricsVisible
        {
            get => desktopLyricsVisible;
            set => Set(ref desktopLyricsVisible, value);
        }

        private double desktopLyricsFontSize;

        public double DesktopLyricsFontSize
        {
            get => desktopLyricsFontSize;
            set => Set(ref desktopLyricsFontSize, value);
        }


        private bool desktopLyricsTop;

        public bool DesktopLyricsTop
        {
            get => desktopLyricsTop;
            set => Set(ref desktopLyricsTop, value);
        }

        private bool desktopLyricsPin;

        public bool DesktopLyricsPin
        {
            get => desktopLyricsPin;
            set => Set(ref desktopLyricsPin, value);
        }

        private int desktopLyricsOpacity;

        public int DesktopLyricsOpacity
        {
            get => desktopLyricsOpacity;
            set => Set(ref desktopLyricsOpacity, value);
        }

        private string lyricsFromColor = string.Empty;

        public string LyricsFromColor
        {
            get => lyricsFromColor;
            set => Set(ref lyricsFromColor, value);
        }

        private string lyricsToColor = string.Empty;

        public string LyricsToColor
        {
            get => lyricsToColor;
            set => Set(ref lyricsToColor, value);
        }

        private double lyricsFontSize;

        public double LyricsFontSize
        {
            get => lyricsFontSize;
            set => Set(ref lyricsFontSize, value);
        }

        private double lyricsActiveFontSize;

        public double LyricsActiveFontSize
        {
            get => lyricsActiveFontSize;
            set => Set(ref lyricsActiveFontSize, value);
        }

        private string lyricsFontFamily = string.Empty;

        public string LyricsFontFamily
        {
            get => lyricsFontFamily;
            set => Set(ref lyricsFontFamily, value);
        }



        public AppOption ToOption()
        {
            Source.InfoVisible = InfoVisible;
            Source.TitleRoll = TitleRoll;
            Source.Mode = Mode;
            Source.Volume = Volume;
            Source.SpectrumVisible = SpectrumVisible;
            Source.SpectrumColor = SpectrumColor;
            Source.SpectrumType = SpectrumType;
            Source.LyricsVisible = LyricsVisible;
            Source.LyricsToColor = LyricsToColor;
            Source.LyricsActiveFontSize = LyricsActiveFontSize;
            Source.LyricsFontFamily =LyricsFontFamily;
            Source.LyricsFontSize = LyricsFontSize;
            Source.LyricsFromColor = LyricsFromColor;
            Source.DesktopLyricsOpacity = DesktopLyricsOpacity;
            Source.DesktopLyricsPin = DesktopLyricsPin;
            Source.DesktopLyricsTop = DesktopLyricsTop;
            Source.DesktopLyricsVisible = DesktopLyricsVisible;
            Source.DesktopLyricsFontSize = DesktopLyricsFontSize;
            return Source;
        }
    }
}
