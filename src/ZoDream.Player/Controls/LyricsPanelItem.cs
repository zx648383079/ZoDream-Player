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
using ZoDream.Shared.Models;

namespace ZoDream.Player.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Player.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Player.Controls;assembly=ZoDream.Player.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:LyricsPanelItem/>
    ///
    /// </summary>
    [TemplatePart(Name = TextTbName, Type =typeof(GradientLabel))]
    [TemplatePart(Name = TransTbName, Type =typeof(TextBlock))]
    [TemplatePart(Name = TranscriptionTbName, Type =typeof(TextBlock))]
    public class LyricsPanelItem : Control
    {
        public const string TextTbName = "PART_TextTb";
        public const string TransTbName = "PART_TransTb";
        public const string TranscriptionTbName = "PART_TranscriptionTb";
        static LyricsPanelItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LyricsPanelItem), new FrameworkPropertyMetadata(typeof(LyricsPanelItem)));
        }

        public LyricsItem Source
        {
            get { return (LyricsItem)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(LyricsItem), typeof(LyricsPanelItem), 
                new PropertyMetadata(null, (d, e) =>
                {
                    (d as LyricsPanelItem)?.UpdateSource();
                }));

        public Color FromColor
        {
            get { return (Color)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FromColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromColorProperty =
            DependencyProperty.Register("FromColor", typeof(Color), typeof(LyricsPanelItem), new PropertyMetadata(Colors.Black));



        public Color ToColor
        {
            get { return (Color)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToColorProperty =
            DependencyProperty.Register("ToColor", typeof(Color), typeof(LyricsPanelItem), new PropertyMetadata(Colors.Red));


        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(LyricsPanelItem), new PropertyMetadata(.0));

        private GradientLabel? TextTb;
        private TextBlock? TransTb;
        private TextBlock? TranscriptionTb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextTb = GetTemplateChild(TextTbName) as GradientLabel;
            TransTb = GetTemplateChild(TransTbName) as TextBlock;
            TranscriptionTb = GetTemplateChild(TranscriptionTbName) as TextBlock;
            UpdateSource();
        }

        private void UpdateSource()
        {
            if (TextTb != null)
            {
                TextTb.Text = Source == null ? string.Empty : Source.Text;
            }
            if (TransTb != null)
            {
                TransTb.Text = Source == null ? string.Empty : Source.Translation;
                TransTb.Visibility = string.IsNullOrWhiteSpace(TransTb.Text) ? Visibility.Collapsed : Visibility.Visible;
            }
            if (TranscriptionTb != null)
            {
                TranscriptionTb.Text = Source == null ? string.Empty : Source.Transcription;
                TranscriptionTb.Visibility = string.IsNullOrWhiteSpace(TranscriptionTb.Text) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

    }
}
