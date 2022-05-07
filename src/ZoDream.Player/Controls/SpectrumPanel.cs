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
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Player"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Player;assembly=ZoDream.Player"
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
    ///     <MyNamespace:SpectrumPanel/>
    ///
    /// </summary>
    public class SpectrumPanel : Control
    {
        static SpectrumPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumPanel), new FrameworkPropertyMetadata(typeof(SpectrumPanel)));
        }

        public int MaxLength { get; set; } = 30;

        public float[] Items
        {
            get { return (float[])GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(float[]), typeof(SpectrumPanel), 
                new PropertyMetadata(Array.Empty<float>(), (d, e) =>
            {
                (d as SpectrumPanel)?.InvalidateVisual();
            }));



        public double RectHeight
        {
            get { return (double)GetValue(RectHeightProperty); }
            set { SetValue(RectHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectHeightProperty =
            DependencyProperty.Register("RectHeight", typeof(double), typeof(SpectrumPanel), new PropertyMetadata(.0));



        public SpectrumType Kind
        {
            get { return (SpectrumType)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Kind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(SpectrumType), typeof(SpectrumPanel), new PropertyMetadata(SpectrumType.Columnar));

        private List<HatItem> HatItems = new();
        private readonly double Space = 1.0;
        private readonly double ColumnWidth = 4.0;
        private readonly double HatSpeed = 2.0;
        private readonly double Rate = 400;

        public int MaxCount => (int)Math.Floor(ActualWidth / (ColumnWidth + Space));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (ActualWidth < ColumnWidth || ActualHeight < ColumnWidth)
            {
                return;
            }
            switch (Kind)
            {
                case SpectrumType.Columnar:
                    RenderColumnar(drawingContext);
                    break;
                case SpectrumType.Ring:
                    RenderRing(drawingContext);
                    break;
                default:
                    break;
            }
        }

        private void RenderColumnar(DrawingContext drawingContext)
        {
            var pen = new Pen(BorderBrush, 0);
            var outerWidth = ColumnWidth + Space;
            var columnCount = Math.Floor(ActualWidth / outerWidth);
            var dataIndex = 0;//Convert.ToInt32((Items.Length - columnCount) / 2);
            var maxHeight = ActualHeight;
            var y = maxHeight;
            var preRectHeight = maxHeight / 300;
            for (int i = 0; i < columnCount; i++)
            {
                RenderColumnar(drawingContext, pen, i, outerWidth * i, y, 
                    dataIndex >= 0 && dataIndex < Items.Length ? 
                    Items[dataIndex] * preRectHeight * Rate: 0);
                dataIndex++;
            }
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen, int index, 
            double x, double y, double height)
        {
            var rectHeight = RectHeight > 0 ? RectHeight: height;
            if (height > 0)
            {
                var bottom = .0;
                while (bottom < height)
                {
                    var h = Math.Min(rectHeight, height - bottom);
                    drawingContext.DrawRectangle(Foreground, pen
                    , new Rect(
                        x, y - bottom - h, ColumnWidth, h
                        ));
                    bottom += rectHeight + Space;
                }
            }
            var hatHeight = RectHeight > 0 ? RectHeight : Space;
            var hat = GetHat(index, height);
            var hatY = y - hat.Current - hatHeight;
            if (hat.Current > 0)
            {
                hatY -= Space;
            } 
            drawingContext.DrawRectangle(Foreground,
                pen, new Rect(
                    x, hatY, ColumnWidth, hatHeight
                    ));
        }

        private HatItem GetHat(int index, double height)
        {
            HatItem item;
            if (HatItems.Count <= index)
            {
                item = new HatItem(height, height);
                HatItems.Add(item);
                return item;
            }
            item = HatItems[index];
            if (item.Current < height || item.Current == item.Target)
            {
                item.Speed = HatSpeed;
            } else
            {
                item.Speed += HatSpeed;
            }
            if (item.Target < height || item.Current == item.Target)
            {
                item.Target = height;
            }
            if (item.Target > item.Current)
            {
                item.Current = Math.Min(item.Current + item.Speed, item.Target);
            } else if (item.Target < item.Current)
            {
                item.Current = Math.Max(item.Current - item.Speed, item.Target);
            }
            if (item.Current < height)
            {
                item.Current = height;
            }
            return item;
        }


        private void RenderRing(DrawingContext drawingContext)
        {
            var pen = new Pen(BorderBrush, 0);
            var outerWidth = ColumnWidth + Space;
            var dataIndex = 0;//Convert.ToInt32((Items.Length - columnCount) / 2);
            var centerX = ActualWidth / 2;
            var centerY = ActualHeight / 2;
            var radius = Math.Min(centerX, centerY) / 1.5;
            var columnCount = (int)Math.Min(Math.Max(Items.Length, 10), Math.Floor(Math.PI * radius * 2 / outerWidth));
            var preAngle = 360.0 / columnCount;
            drawingContext.DrawEllipse(new SolidColorBrush(Colors.Transparent),
                new Pen(Foreground, 1), new Point(centerX, centerY), radius, radius);
            if (Items.Length < 1)
            {
                return;
            }
            var y = centerY - radius;
            var preRectHeight = y / 300;
            for (int i = 0; i < columnCount; i++)
            {
                var tranform = new RotateTransform(i * preAngle, centerX, centerY);
                drawingContext.PushTransform(tranform);
                RenderColumnar(drawingContext, pen, i, 
                    centerX - ColumnWidth / 2
                    , y,
                    dataIndex >= 0 && dataIndex < Items.Length ?
                    Items[dataIndex] * preRectHeight * Rate : 0);
                drawingContext.Pop();
                dataIndex++;
            }
        }

        private void RenderRing(DrawingContext drawingContext, Pen pen, int index, double x, double y, double height, double angle)
        {

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
