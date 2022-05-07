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
                case SpectrumType.SymmetryColumnar:
                    RenderSymmetryColumnar(drawingContext);
                    break;
                case SpectrumType.Ring:
                    RenderRing(drawingContext);
                    break;
                case SpectrumType.RingLine:
                    RenderRingLine(drawingContext);
                    break;
                case SpectrumType.SymmetryRing:
                    RenderSymmetryRing(drawingContext);
                    break;
                default:
                    break;
            }
        }

        private void RenderEach(DrawingContext drawingContext, 
            int dataBegin, int count, double rate, double maxHeight, 
            Action<DrawingContext, Pen, int, double, double, double> func)
        {
            var pen = new Pen(BorderBrush, 0);
            var outerWidth = ColumnWidth + Space;
            var dataIndex = dataBegin;
            var y = maxHeight;
            var preRectHeight = maxHeight / 300;
            for (int i = 0; i < count; i++)
            {
                func?.Invoke(drawingContext, pen, i, outerWidth * i, y,
                    dataIndex >= 0 && dataIndex < Items.Length ?
                    Items[dataIndex] * preRectHeight * rate : 0);
                dataIndex++;
            }
        }

        private void RenderColumnar(DrawingContext drawingContext)
        {
            var outerWidth = ColumnWidth + Space;
            RenderEach(drawingContext, 0,
                (int)Math.Floor(ActualWidth / outerWidth), 
                Rate * 3, ActualHeight, RenderColumnar);
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen, int index,
            double x, double y, double height)
        {
            RenderColumnar(drawingContext, pen, index, x, y, height, true);
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen, int index, 
            double x, double y, double height, bool hasHat)
        {
            RenderColumnar(drawingContext, pen, index, x, y, height, hasHat ? GetHat(index, height) : null);
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen, int index,
            double x, double y, double height, HatItem? hat)
        {
            var rectHeight = RectHeight > 0 ? RectHeight : height;
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
            else if (hat is null)
            {
                drawingContext.DrawRectangle(Foreground, pen
                    , new Rect(
                        x, y - 2, ColumnWidth, 2
                        ));
            }
            if (hat is null)
            {
                return;
            }
            var hatHeight = RectHeight > 0 ? RectHeight : Space;
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
            RenderRing(drawingContext, .6, 360.0, 1, (d, p, i, x, y, h, a, cx, cy) =>
            {
                var tranform = new RotateTransform(a, cx, cy);
                drawingContext.PushTransform(tranform);
                RenderColumnar(d, p, i, x, y, h);
                drawingContext.Pop();
            });
        }

        private void RenderRing(DrawingContext drawingContext, double radiusRate, 
            double maxAngle, double perimeterRate, 
            Action<DrawingContext, Pen, int, double, double, double, double, double, double> func)
        {
            var outerWidth = ColumnWidth + Space;
            var centerX = ActualWidth / 2;
            var centerY = ActualHeight / 2;
            var radius = Math.Min(centerX, centerY) * radiusRate;
            var columnCount = (int)Math.Min(Math.Max(Items.Length, 10), Math.Floor(Math.PI * radius * 2 * perimeterRate / outerWidth));
            var preAngle = maxAngle / columnCount;
            if (Items.Length < 1)
            {
                return;
            }
            drawingContext.DrawEllipse(new SolidColorBrush(Colors.Transparent),
                new Pen(Foreground, 1), new Point(centerX, centerY), radius, radius);

            var y = centerY - radius;
            var x = centerX - ColumnWidth / 2;
            RenderEach(drawingContext, 0, columnCount, Rate, y, (d, p, i, _, _, h) =>
            {
                func?.Invoke(d, p, i, x, y, h, i * preAngle, centerX, centerY);
            });
        }


        private void RenderSymmetryColumnar(DrawingContext drawingContext)
        {
            var outerWidth = ColumnWidth + Space;
            RenderEach(drawingContext, 0,
                (int)Math.Floor(ActualWidth / outerWidth),
                Rate, ActualHeight / 2, RenderSymmetryColumnar);
        }

        private void RenderSymmetryColumnar(DrawingContext drawingContext, Pen pen, int index,
            double x, double y, double height)
        {
            if (height < 2)
            {
                height = 1;
            }
            drawingContext.DrawRectangle(Foreground, pen
                , new Rect(
                    x, y - height, ColumnWidth, 2 * height
                    ));
        }

        private void RenderRingLine(DrawingContext drawingContext)
        {
            var outerWidth = ColumnWidth + Space;
            var centerX = ActualWidth / 2;
            var centerY = ActualHeight / 2;
            var radius = Math.Min(centerX, centerY) * .6;
            var columnCount = (int)Math.Min(Math.Max(Items.Length, 10), 
                Math.Floor(Math.PI * radius / outerWidth));
            var preAngle = 180 / columnCount;
            if (Items.Length < 1)
            {
                return;
            }
            var pen = new Pen(Foreground, 1);
            //drawingContext.DrawEllipse(new SolidColorBrush(Colors.Transparent), pen
            //    , new Point(centerX, centerY), radius, radius);
            RenderEach(drawingContext, 0, columnCount, Rate, centerY - radius, (d, _, i, _, _, h) =>
            {
                var len = radius + h;
                var angle = i * preAngle;
                var x = centerX + Math.Sin(angle) * len;
                var y = centerY - Math.Cos(angle) * len;

                len = radius - h;
                var x2 = centerX + Math.Sin(angle) * len;
                var y2 = centerY - Math.Cos(angle) * len;

                d.DrawLine(pen, new Point(x, y), new Point(x2, y2));
                d.DrawLine(pen, new Point(centerX * 2 - x, y), new Point(centerX * 2 - x2, y2));

            });
        }

        private void RenderSymmetryRing(DrawingContext drawingContext)
        {
            RenderRing(drawingContext, .6, 180.0, .5, (d, p, i, x, y, h, a, cx, cy) =>
            {
                var tranform = new RotateTransform(a, cx, cy);
                drawingContext.PushTransform(tranform);
                RenderColumnar(d, p, i, x, y, h);
                drawingContext.Pop();
                tranform = new RotateTransform(-a, cx, cy);
                drawingContext.PushTransform(tranform);
                RenderColumnar(d, p, i, x, y, h, HatItems[i]);
                drawingContext.Pop();
            });
        }

    }
}
