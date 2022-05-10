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
    ///     <MyNamespace:LyricsPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = ScrollBarName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = LyricsBoxName, Type = typeof(StackPanel))]
    public class LyricsPanel : Control
    {
        public const string ScrollBarName = "PART_ScrollBar";
        public const string LyricsBoxName = "PART_LyricsBox";

        static LyricsPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LyricsPanel), new FrameworkPropertyMetadata(typeof(LyricsPanel)));
        }

        public IList<LyricsItem> Items
        {
            get { return (IList<LyricsItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList<LyricsItem>), typeof(LyricsPanel), new PropertyMetadata(null, (d, e) =>
            {
                (d as LyricsPanel)?.RefreshView();
            }));




        public double CurrentTime
        {
            get { return (double)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(double), typeof(LyricsPanel), new PropertyMetadata(.0, (d, e) =>
            {
                (d as LyricsPanel)?.TimeUpdate();
            }));



        public double ActiveFontSize
        {
            get { return (double)GetValue(ActiveFontSizeProperty); }
            set { SetValue(ActiveFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveFontSizeProperty =
            DependencyProperty.Register("ActiveFontSize", typeof(double), typeof(LyricsPanel), new PropertyMetadata(20.0));


        public Color FromColor
        {
            get { return (Color)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FromColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromColorProperty =
            DependencyProperty.Register("FromColor", typeof(Color), typeof(LyricsPanel), new PropertyMetadata(Colors.Black));



        public Color ToColor
        {
            get { return (Color)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToColorProperty =
            DependencyProperty.Register("ToColor", typeof(Color), typeof(LyricsPanel), new PropertyMetadata(Colors.Red));



        private ScrollViewer? ScrollBar;
        private StackPanel? MainBox;

        public event EventHandler<LyricsItem>? ItemChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ScrollBar = GetTemplateChild(ScrollBarName) as ScrollViewer;
            MainBox = GetTemplateChild(LyricsBoxName) as StackPanel;
        }


        private void RefreshView()
        {
            if (MainBox == null || ScrollBar == null)
            {
                return;
            }
            MainBox.Children.Clear();
            ScrollBar.ScrollToHome();
            if (Items == null)
            {
                return;
            }
            foreach (var item in Items)
            {
                var line = new LyricsPanelItem
                {
                    Source = item,
                    FromColor = FromColor,
                    ToColor = ToColor,
                };
                MainBox.Children.Add(line);
                line.MouseDoubleClick += Line_MouseDoubleClick;
            }
        }

        private void Line_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemChanged?.Invoke(this, (sender as LyricsPanelItem).Source);
        }

        private void TimeUpdate()
        {
            if (MainBox == null || ScrollBar == null)
            {
                return;
            }
            var isScroll = false;
            MainBox.Margin = new Thickness(0, ActualHeight / 2, 0, ActualHeight / 2);
            foreach (LyricsPanelItem item in MainBox.Children)
            {
                if (item is null)
                {
                    continue;
                }
                var isActive = item.Source.IsActive(CurrentTime);
                item.FromColor = FromColor;
                item.ToColor = ToColor;
                item.FontSize = isActive ? ActiveFontSize : FontSize;
                // item.HorizontalAlignment = HorizontalAlignment.Center;
                if (isActive && !isScroll)
                {
                    isScroll = true;
                    var currentScrollPosition = ScrollBar.VerticalOffset;
                    var point = new Point(0, currentScrollPosition);
                    var targetPosition = item.TransformToVisual(ScrollBar).Transform(point);
                    ScrollBar.ScrollToVerticalOffset(targetPosition.Y - (ActualHeight - item.ActualHeight) / 2);
                }
                item.Offset = item.Source.GetOffset(CurrentTime);
            }
        }
    }
}
