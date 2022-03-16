using System.Windows;
using ZoDream.Player.Core;
using ZoDream.Player.Helpers;
using ZoDream.Player.Models;

namespace ZoDream.Player.Views
{
    /// <summary>
    /// LyricsEditView.xaml 的交互逻辑
    /// </summary>
    public partial class LyricsEditView : Window
    {
        public BassPlayer Player { private get; set; }

        public LyricsEditView()
        {
            InitializeComponent();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var file = Local.ChooseFile("歌词文件|*.lrc|所有文件|*.*");
            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            ContentTb.Text = Local.Read(file);
        }

        private void ContentTb_Drop(object sender, DragEventArgs e)
        {
            var fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            var ext = System.IO.Path.GetExtension(fileName);
            if (ext != ".lrc")
            {
                return;
            }
            ContentTb.Text = Local.Read(fileName);
        }

        private void ContentTb_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void BeginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Player == null)
            {
                return;
            }
            InsertTime(Player.Position);
        }

        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Player == null)
            {
                return;
            }
            InsertTime(Player.Position, true);
        }

        private void InsertTime(double time, bool isEnd = false)
        {
            var position = ContentTb.SelectionStart;
            ContentTb.Text = InsertTime(ContentTb.Text, position, time, isEnd);
            ContentTb.Focus();
            ContentTb.Select(position, 0);
        }

        private string InsertTime(string text, int position, double time, bool isEnd = false)
        {
            var line = GetLineRange(text, position, out int begin, out int end);
            var lyrics = LyricsReader.From(line);
            if (lyrics == null)
            {
                lyrics = new LyricsItem();
            }
            if (isEnd)
            {
                lyrics.EndAt = time;
            }
            else
            {
                lyrics.StartAt = time;
            }
            return text.Substring(0, begin) + LyricsReader.To(lyrics) + text.Substring(end);
        }

        private string GetLineRange(string text, int position, out int begin, out int end)
        {
            var lineTag = '\r';
            begin = position > 0 ? text.LastIndexOf(lineTag, position) + 1 : 0;
            end = position < text.Length ? text.IndexOf(lineTag, position) : text.Length;
            if (end < 0)
            {
                end = text.Length;
            }
            return text.Substring(begin, end - begin);
        }
    }
}
