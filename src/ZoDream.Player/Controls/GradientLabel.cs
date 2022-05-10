using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    ///     <MyNamespace:GradientLabel/>
    ///
    /// </summary>
    public class GradientLabel : Control
    {
        static GradientLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GradientLabel), new FrameworkPropertyMetadata(typeof(GradientLabel)));
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GradientLabel), 
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    (d as GradientLabel)?.UpdateText();
                }));



        public Color FromColor
        {
            get { return (Color)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FromColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromColorProperty =
            DependencyProperty.Register("FromColor", typeof(Color), typeof(GradientLabel), new PropertyMetadata(Colors.Black));



        public Color ToColor
        {
            get { return (Color)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToColorProperty =
            DependencyProperty.Register("ToColor", typeof(Color), typeof(GradientLabel), new PropertyMetadata(Colors.Red));


        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(GradientLabel), new PropertyMetadata(.0, (d, e) =>
            {
                (d as GradientLabel)?.UpdateOffset((double)e.OldValue, (double)e.NewValue);
            }));

        private void UpdateOffset(double oldVal, double newVal)
        {
            //UpdateText();
            //InvalidateVisual();
            if (string.IsNullOrWhiteSpace(Text))
            {
                UpdateText();
                InvalidateVisual();
                return;
            }
            if (oldVal != newVal)
            {
                if (oldVal <= 0 || newVal <= 0)
                {
                    UpdateText();
                }
                InvalidateVisual();
                return;
            }
        }

        private void UpdateText()
        {
            var text = Text;
            if (ActualWidth <= 0 || string.IsNullOrEmpty(text))
            {
                Height = FontSize + Padding.Top + Padding.Bottom;
                return;
            }
            var count = Math.Floor(ActualWidth / FontSize);
            Height = Padding.Top + Padding.Bottom + 
                Math.Ceiling(text.Length / count) * (FontSize + Padding.Top);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var text = Text;
            if (ActualWidth <= 0 || string.IsNullOrEmpty(text))
            {
                return;
            }
            // drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Green), 1), new Point(1, 1), new Point(100, 1));
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var y = Padding.Top;
            var format = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, new SolidColorBrush(FromColor), 1.25);
            var offset = text.Length * Offset;
            if (ActualWidth > format.WidthIncludingTrailingWhitespace)
            {
                if (offset <= 0)
                {
                    RenderText(drawingContext, format, GetLeft(format.WidthIncludingTrailingWhitespace), y);
                } else
                {
                    RenderText(drawingContext, font, text, y, offset);
                }
                
                Height = y + format.Height + Padding.Bottom;
                return;
            }
            var count = (int)Math.Floor(ActualWidth / FontSize);
            var i = 0;
            while (i < text.Length)
            {
                var len = text.Length - i;
                if (len > count)
                {
                    len = count;
                }
                var height = RenderText(drawingContext, font, text.Substring(i, len), y, offset);
                i += len;
                y += height + Padding.Top;
                offset -= len;
            }
            Height = y;
        }

        private double RenderText(DrawingContext drawingContext, Typeface font, string text, double y, double offset)
        {
            if (offset <= 0)
            {
                return RenderText(drawingContext, font, new SolidColorBrush(FromColor), text, y);
            }
            if (offset >= text.Length)
            {
                return RenderText(drawingContext, font, new SolidColorBrush(ToColor), text, y);
            }
            var fromBrush = new SolidColorBrush(FromColor);
            var format = new FormattedText(text,
                    CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, fromBrush, 1.25);
            var height = format.Height;
            var width = format.Width;
            var x = GetLeft(width);
            var start = (int)Math.Floor(offset);
            var startX = .0;
            if (start > 0)
            {
                var line = text.Substring(0, start);
                format = new FormattedText(line, 
                    CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, new SolidColorBrush(ToColor), 1.25);
                RenderText(drawingContext, format, x, y);
                startX = format.WidthIncludingTrailingWhitespace;
            }
            if (start < offset)
            {
                var code = text.Substring(start, 1);
                
                if (code != " ")
                {
                    var off = offset - start;
                    var items = new GradientStopCollection
                    {
                        new GradientStop(ToColor, 0),
                        new GradientStop(ToColor, off),
                        new GradientStop(FromColor, off),
                        new GradientStop(FromColor, 1)
                    };
                    format = new FormattedText(code,
                        CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                font, FontSize, new LinearGradientBrush(items, 0), 1.25);
                    RenderText(drawingContext, format, x + startX, y);
                    startX += format.WidthIncludingTrailingWhitespace;
                } else
                {
                    format = new FormattedText(code,
                        CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                font, FontSize, fromBrush, 1.25);
                    startX += format.WidthIncludingTrailingWhitespace;
                }
                start++;
            }
            if (text.Length - start > 0)
            {
                var end = text.Substring(start);
                if (end[0] == ' ')
                {
                    if (end.Length < 2)
                    {
                        return height;
                    }
                    // startX += spaceWidth;
                }
                format = new FormattedText(end,
                    CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, fromBrush, 1.25);
                RenderText(drawingContext, format, x + startX, y);
            }
            return height;
        }


        private double RenderText(DrawingContext drawingContext, Typeface font, 
            Brush brush,
            string text, double y)
        {
            var format = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, brush, 1.25);
            RenderText(drawingContext, format, GetLeft(format.Width), y);
            return format.Height;
        }

        private void RenderText(DrawingContext drawingContext,
            FormattedText format, double x, double y)
        {
            drawingContext.DrawText(format, new Point(
                x, y));
        }

        private double GetLeft(double fontWidth)
        {
            switch (HorizontalContentAlignment)
            {
                case HorizontalAlignment.Center:
                    return (ActualWidth - fontWidth) / 2;
                case HorizontalAlignment.Right:
                    return ActualWidth - fontWidth - Padding.Right;
                default:
                    return Padding.Left;
            }
        }
    }
}
