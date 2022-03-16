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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Player.Models;

namespace ZoDream.Player.Controls
{
    /// <summary>
    /// LyricsLine.xaml 的交互逻辑
    /// </summary>
    public partial class LyricsLine : UserControl
    {
        public LyricsLine()
        {
            InitializeComponent();
        }

        public LyricsItem Source
        {
            get { return (LyricsItem)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(LyricsItem), typeof(LyricsLine), new PropertyMetadata(null));



        public Color Inline
        {
            get { return (Color)GetValue(InlineProperty); }
            set { SetValue(InlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Inline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlineProperty =
            DependencyProperty.Register("Inline", typeof(Color), typeof(LyricsLine), new PropertyMetadata(Colors.Black));



        public Color Outline
        {
            get { return (Color)GetValue(OutlineProperty); }
            set { SetValue(OutlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Outline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutlineProperty =
            DependencyProperty.Register("Outline", typeof(Color), typeof(LyricsLine), new PropertyMetadata(Colors.GreenYellow));



        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(LyricsLine), new PropertyMetadata(.0));

    }
}
