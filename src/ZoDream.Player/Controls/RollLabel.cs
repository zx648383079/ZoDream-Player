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
using System.Windows.Threading;

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
    ///     <MyNamespace:RollLabel/>
    ///
    /// </summary>
    public class RollLabel : Control
    {
        static RollLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollLabel), new FrameworkPropertyMetadata(typeof(RollLabel)));
        }

        public RollLabel()
        {
            Loaded += RollLabel_Loaded;
            Unloaded += RollLabel_Unloaded;
        }

        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(RollLabel), 
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    (d as RollLabel)?.UpdateText();
                }));




        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(RollLabel), new PropertyMetadata(true));


        private DispatcherTimer Timer = new();
        private FormattedText? Formatted;
        private double ContentWidth;
        private double RollProgress = 0;
        private readonly double Speed = 2;  // 滚动速度
        private readonly double StopTime = 50.0;//Speed * 100; // 停留时间根据滚动速度算出来的

        private void RollLabel_Unloaded(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
        }

        private void RollLabel_Loaded(object sender, RoutedEventArgs e)
        {
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!IsActive || IsMouseOver || Formatted == null)
            {
                return;
            }
            var visibleWidth = ActualWidth - Padding.Left - Padding.Right;
            if (ContentWidth < visibleWidth)
            {
                return;
            }
            RollProgress += Speed;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (ActualWidth <= 0 || ActualHeight <= 0 || Formatted == null)
            {
                return;
            }
            var visibleWidth = ActualWidth - Padding.Left - Padding.Right;
            var fontHideWidth = Math.Max(ContentWidth - visibleWidth, 0);
            if (RollProgress >= fontHideWidth + StopTime)
            {
                RollProgress = -StopTime;
            }
            var x = GetLeft() - Math.Min(Math.Max(RollProgress, 0), fontHideWidth);
            drawingContext.DrawText(Formatted, new Point(
                 x
                , Padding.Top));
        }

        public double GetLeft()
        {
            return HorizontalContentAlignment switch
            {
                HorizontalAlignment.Center => Math.Max((ActualWidth - ContentWidth) / 2, Padding.Top),
                HorizontalAlignment.Right => Math.Max(ActualWidth - ContentWidth - Padding.Right, Padding.Top),
                _ => Padding.Left,
            };
        }


        private void UpdateText()
        {
            ToolTip = Content;
            RollProgress = -StopTime;
            Formatted = new FormattedText(Content, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, 
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground, 1.25);
            ContentWidth = Formatted.WidthIncludingTrailingWhitespace;
            Height = Formatted.Height + Padding.Top + Padding.Right;
            InvalidateVisual();
        }
    }
}
