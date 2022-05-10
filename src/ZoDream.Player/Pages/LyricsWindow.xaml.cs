using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZoDream.Player.Controls;
using ZoDream.Player.ViewModels;
using ZoDream.Shared.Controls;
using ZoDream.Shared.Models;

namespace ZoDream.Player.Pages
{
    /// <summary>
    /// LyricsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LyricsWindow : Window
    {
        public LyricsWindow()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel = App.ViewModel;
        private int LastIndex  = -1;
        private readonly List<LyricsItem> LyricsItems = new();
        private readonly int LyricsCount = 2;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.Option.DesktopLyricsPin)
            {
                return;
            }
            DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.Player.TimeUpdated += Player_TimeUpdated;
            var option = ViewModel.Option;
            Topmost = option.DesktopLyricsTop;
            Background = new SolidColorBrush(ColorHelper.FromRGBA(255, 255, 255, option.DesktopLyricsOpacity / 100));
            foreach (var item in LyricsPanel.Children)
            {
                if (item is GradientLabel label)
                {
                    label.FontFamily = Utils.Util.ToFont(option.LyricsFontFamily);
                    label.FromColor = ColorHelper.From(option.LyricsFromColor);
                    label.ToColor = ColorHelper.From(option.LyricsToColor);
                    label.FontSize = option.DesktopLyricsFontSize;
                }
            }
        }

        private void Player_TimeUpdated(object sender, double value)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var index = GetLyricsIndex(value);
                var isSet = SetLyrics(index);
                var i = -1;
                foreach (var item in LyricsPanel.Children)
                {
                    if (item is GradientLabel label)
                    {
                        ++i;
                        var lyrics = LyricsItems.Count > i ? LyricsItems[i] : null;
                        if (isSet)
                        {
                            label.Text = lyrics != null ? lyrics.Text : string.Empty;
                        }
                        label.Offset = lyrics != null ? lyrics.GetOffset(value) : 0;
                    }
                }
            });
        }

        private bool SetLyrics(int index)
        {
            if (index == LastIndex)
            {
                return false;
            }
            LyricsItems.Clear();
            if (index < 0)
            {
                LastIndex = index;
                return true;
            }
            var items = ViewModel.Lyrics!.Items;
            for (int i = index; i < Math.Min(items.Count, index + LyricsCount); i++)
            {
                LyricsItems.Add(items[i]);
            }
            LastIndex = index;
            return true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.Player.TimeUpdated -= Player_TimeUpdated;
        }

        private int GetLyricsIndex(double value)
        {
            var lyrics = ViewModel.Lyrics;
            if (lyrics == null)
            {
                return -1;
            }
            for (int i = 0; i < lyrics.Items.Count; i++)
            {
                if (lyrics.Items[i].IsActive(value))
                {
                    return i % 2 == 1? i - 1 : i;
                }
            }
            return -1;
        }
    }
}
