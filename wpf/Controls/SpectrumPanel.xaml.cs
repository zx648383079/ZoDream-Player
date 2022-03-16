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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoDream.Player.Controls
{
    /// <summary>
    /// SpectrumPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SpectrumPanel : UserControl
    {
        public SpectrumPanel()
        {
            InitializeComponent();
        }

        public int MaxLength { get; set; } = 30;

        public byte[] Items
        {
            get { return (byte[])GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(byte[]), typeof(SpectrumPanel), new PropertyMetadata(null, OnItemsChanged));

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SpectrumPanel).RefreshView();
        }

        private void RefreshView()
        {
            var items = RenderColumnar(MainBox.ActualWidth, MainBox.ActualHeight, MaxLength, Items);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Rectangle rectangle;
                if (i >= MainBox.Children.Count)
                {
                    rectangle = new Rectangle();
                    MainBox.Children.Add(rectangle);
                    rectangle.Fill = new SolidColorBrush(Colors.Yellow);
                } else
                {
                    rectangle = MainBox.Children[i] as Rectangle;
                }
                Canvas.SetLeft(rectangle, item.Left);
                Canvas.SetTop(rectangle, item.Top);
                rectangle.Width = item.Width;
                rectangle.Height = item.Height;
            }
        }

        /// <summary>
        /// 画柱状
        /// </summary>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="maxLength"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IList<Rect> RenderColumnar(double maxWidth, double maxHeight, int maxLength, byte[] data)
        {
            var items = new List<Rect>();
            var w = maxWidth / maxLength - 2;
            for (int i = 0; i < maxLength; i++)
            {
                var x = (w + 1) * i;
                var b = data == null || i >= data.Length ? 0 : Math.Max(0, (int)data[i]);
                var h = maxHeight * b / 256;
                var y = maxHeight - h;
                items.Add(new Rect(x, y, w, h));
            }
            return items;
        }

        /// <summary>
        /// 画圆环
        /// </summary>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="maxLength"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IList<Rect> RenderRing(double maxWidth, double maxHeight, int maxLength, byte[] data)
        {
            var items = new List<Rect>();
            var centerX = maxWidth / 2;
            var centerY = maxHeight / 2;
            var radius = Math.Min(centerX, centerY);
            var angle = 360 / maxLength;
            for (int i = 0; i < maxLength; i++)
            {
                var b = data == null || i >= data.Length ? 0 : Math.Max(0, (int)data[i]);
                var h = radius * b / 256;
                var a = angle * i - 90;
                var x = radius * Math.Cos(a);
                var y = radius * Math.Sin(a);
                items.Add(new Rect(x, y, angle - 2, h));
            }
            return items;
        }
    }
}
