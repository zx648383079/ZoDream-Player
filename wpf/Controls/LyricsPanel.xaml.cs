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
    /// LyricsPanel.xaml 的交互逻辑
    /// </summary>
    public partial class LyricsPanel : UserControl
    {
        public LyricsPanel()
        {
            InitializeComponent();
        }

        public IList<LyricsItem> Items
        {
            get { return (IList<LyricsItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList<LyricsItem>), typeof(LyricsPanel), new PropertyMetadata(null, OnItemsChanged));




        public double CurrentTime
        {
            get { return (double)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(double), typeof(LyricsPanel), new PropertyMetadata((double)0, OnTimeUpdate));

        private static void OnTimeUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LyricsPanel).TimeUpdate();
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LyricsPanel).RefreshView();
        }

        private void RefreshView()
        {
            MainBox.Children.Clear();
            ScrollBar.ScrollToHome();
            if (Items == null)
            {
                return;
            }
            foreach (var item in Items)
            {
                var line = new LyricsLine();
                line.Source = item;
                MainBox.Children.Add(line);
            }
        }

        private void TimeUpdate()
        {
            var isScroll = false;
            MainBox.Margin = new Thickness(0, ActualHeight/2, 0, ActualHeight / 2);
            foreach (LyricsLine item in MainBox.Children)
            {
                if (item is not LyricsLine)
                {
                    continue;
                }
                var isActive = item.Source.IsActive(CurrentTime);
                item.FontSize = isActive ? 20 : 16;
                item.HorizontalAlignment = HorizontalAlignment.Center;
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
