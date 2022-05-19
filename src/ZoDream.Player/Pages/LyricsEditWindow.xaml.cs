using System;
using System.Collections.Generic;
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
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Player.Pages
{
    /// <summary>
    /// LyricsEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LyricsEditWindow : Window
    {
        public LyricsEditWindow()
        {
            InitializeComponent();
        }

        public event RoutedPropertyChangedEventHandler<Lyrics>? OnPreview;

        private void LyricsTb_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void LyricsTb_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = ((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop));
                _ = LoadLyrics(files.FirstOrDefault());
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                RestoreDirectory = true,
                Filter = "Lyrics|*.lrc;*.qrc;*.krc|All|*.*",
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            _ = SaveLyrics(picker.FileName, LyricsTb.Text);
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                RestoreDirectory = true,
                Filter = "Lyrics|*.lrc;*.qrc;*.krc|All|*.*",
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            _ = LoadLyrics(picker.FileName);
        }

        private async Task SaveLyrics(string? fileName, string content)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(content))
            {
                return;
            }
            var lyrics = await new LrcReader().ReadFromStringAsync(content);
            if (lyrics == null)
            {
                return;
            }
            await LyricsProvider.WriteAsync(fileName, lyrics);
        }

        public async Task LoadLyrics(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            await LoadLyrics(await LyricsProvider.ReadAsync(fileName));
        }

        public async Task LoadLyrics(Lyrics? lyrics)
        {
            if (lyrics == null)
            {
                return;
            }
            var content = await new LrcWriter().WriteToStringAsync(lyrics);
            App.Current.Dispatcher.Invoke(() =>
            {
                LyricsTb.Text = content;
            });
        }

        private async void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            var content = LyricsTb.Text;
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }
            var lyrics = await new LrcReader().ReadFromStringAsync(content);
            if (lyrics == null)
            {
                return;
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                OnPreview?.Invoke(this, new RoutedPropertyChangedEventArgs<Lyrics>(null, lyrics));
            });
        }

        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsPaused)
            {
                return;
            }
            var time = App.ViewModel.Player.Progress / 1000;
            EditLine((o,d,l) =>
            {
                return $"[{Time.Format(o, true, 2)},{Time.Format(time - o, true, 2)}]{l}";
            });
        }

        private void BeginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsPaused)
            {
                return;
            }
            var time = App.ViewModel.Player.Progress / 1000;
            EditLine((o, d, l) =>
            {
                if (d == 0)
                {
                    return $"[{Time.Format(time, true, 2)}]{l}";
                }
                var end = o + d;
                return $"[{Time.Format(time, true, 2)},{Time.Format(end - time, true, 2)}]{l}";
            });
        }

        private void EditLine(Func<double, double, string, string> func)
        {
            var text = LyricsTb.Text;
            LyricsTb.Focus();
            var i = LyricsTb.SelectionStart;
            var lineIndex = LyricsTb.GetLineIndexFromCharacterIndex(i);
            var line = LyricsTb.GetLineText(lineIndex);
            var lineStart = LyricsTb.GetCharacterIndexFromLineIndex(lineIndex);
            var lineEnd = lineStart + line.Length;
            line = SplitLyrics(line, out var offset, out var duration);
            LyricsTb.Text = text.Substring(0, lineStart) + func(offset, duration, line) + text.Substring(lineEnd);
            LyricsTb.Select(lineStart, 1);
        }

        private string SplitLyrics(string line, out double offset, out double duration)
        {
            var i = line.IndexOf('[');
            var j = i < 0 ? -1 : line.IndexOf(']', i);
            if (i < 0 || j < 0)
            {
                offset = 0;
                duration = 0;
                return line;
            }
            var args = line.Substring(i + 1, j - i - 1).Split(new char[] { ','}, 2);
            offset = Time.Render(args[0]);
            duration = args.Length > 1 ? Time.Render(args[1]) : 0;
            return line.Substring(j + 1);
        }
    }
}
