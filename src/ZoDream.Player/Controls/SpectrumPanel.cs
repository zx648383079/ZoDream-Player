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

        private readonly List<HatItem> HatItems = new();
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
                case SpectrumType.InverseColumnar:
                    RenderInverseColumnar(drawingContext);
                    break;
                case SpectrumType.Ring:
                    RenderRing(drawingContext);
                    break;
                case SpectrumType.RingLine:
                    RenderRingLine(drawingContext, true);
                    break;
                case SpectrumType.SymmetryRing:
                    RenderSymmetryRing(drawingContext);
                    break;
                case SpectrumType.Polyline:
                    RenderPolyline(drawingContext);
                    break;
                case SpectrumType.PolylineRing:
                    RenderPolylineRing(drawingContext);
                    break;
                case SpectrumType.InversePolyline:
                    RenderInversePolyline(drawingContext);
                    break;
                case SpectrumType.InversePolylineRing:
                    RenderInversePolylineRing(drawingContext);
                    break;
                default:
                    break;
            }
        }

        private void RenderEach(DrawingContext drawingContext, 
            int dataBegin, int count, double rate, double maxHeight,
            RenderSpectumFunc func)
        {
            var pen = new Pen(BorderBrush, 0);
            var outerWidth = ColumnWidth + Space;
            var dataIndex = dataBegin;
            var y = maxHeight;
            var preRectHeight = maxHeight / 300;
            for (int i = 0; i < count; i++)
            {
                func?.Invoke(drawingContext, pen, i, outerWidth * i, y,
                    ColumnWidth,
                    dataIndex >= 0 && dataIndex < Items.Length ?
                    Math.Min(Items[dataIndex] * preRectHeight * rate, maxHeight) : 0);
                dataIndex++;
            }
        }

        private void RenderColumnar(DrawingContext drawingContext)
        {
            RenderEach(drawingContext, RenderColumnarHat);
        }

        private void RenderPolyline(DrawingContext drawingContext)
        {
            var lastPoint = new Point(0, ActualHeight);
            var pen = new Pen(Foreground, 1);
            RenderEach(drawingContext, (d, _, x, y, w, h, _) =>
            {
                var point = new Point(x + w / 2, y - h);
                d.DrawLine(pen, lastPoint, point);
                lastPoint = point;
            });
            if (lastPoint.X == 0 || pen == null)
            {
                return;
            }
            drawingContext.DrawLine(pen, lastPoint, 
                new Point(lastPoint.X + ColumnWidth / 2, ActualHeight));
        }

        private void RenderEach(DrawingContext drawingContext,
            RenderSpectumHatFunc func)
        {
            RenderEach(drawingContext, false, func);
        }

        private void RenderEach(DrawingContext drawingContext,
            bool isSymmetry,
            RenderSpectumHatFunc func)
        {
            var outerWidth = ColumnWidth + Space;
            var maxWidth = isSymmetry ? ActualWidth / 2 : ActualWidth;
            var leftX = isSymmetry ? maxWidth - Space / 2 : 0;
            var rightX = isSymmetry ? maxWidth + Space / 2 : 0;
            RenderEach(drawingContext, 0,
                (int)Math.Floor(maxWidth / outerWidth),
                Rate * 2, ActualHeight, (d,p,i,x,y,w,h) =>
                {
                    var hat = GetHat(i, h);
                    func?.Invoke(d, p, x + rightX, y, w, h, hat);
                    if (isSymmetry)
                    {
                        func?.Invoke(d, p, leftX - x, y, w, h, hat);
                    }
                });
        }

        private void RenderInversePolyline(DrawingContext drawingContext)
        {
            var outerWidth = ColumnWidth + Space;
            var centerX = ActualWidth / 2;
            var leftX = centerX - Space / 2;
            var rightX = centerX + Space / 2;
            var lastX = -Space/2;
            var lastY = ActualHeight;
            var pen = new Pen(Foreground, 1);
            var j = -1;
            RenderEach(drawingContext, 0,
                (int)Math.Floor(centerX / outerWidth),
                Rate, ActualHeight, (d, _, i, x, y, w, h) =>
                {
                    j++;
                    var top = y - h;
                    if (j < 1)
                    {
                        lastY = Math.Min(top + 3, lastY);
                    }
                    d.DrawLine(pen, 
                        new Point(lastX + rightX, lastY), 
                        new Point(x + rightX, top));
                    d.DrawLine(pen,
                        new Point(leftX - lastX, lastY),
                        new Point(leftX - x, top));
                    lastX = x;
                    lastY = top;
                });
        }

        private void RenderInverseColumnar(DrawingContext drawingContext)
        {
            RenderEach(drawingContext, true, RenderColumnarHat);
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen, int index,
            double x, double y, double height)
        {
            RenderColumnar(drawingContext, pen, index, x, y, height, true);
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen, int index, 
            double x, double y, double height, bool hasHat)
        {
            RenderColumnar(drawingContext, pen, x, y, height, hasHat ? GetHat(index, height) : null);
        }

        private void RenderColumnarHat(DrawingContext drawingContext, Pen pen,
            double x, double y, double width, double height, HatItem? hat)
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
                        x, y - bottom - h, width, h
                        ));
                    bottom += rectHeight + Space;
                }
            }
            else if (hat is null)
            {
                drawingContext.DrawRectangle(Foreground, pen
                    , new Rect(
                        x, y - 2, width, 2
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
                    x, hatY, width, hatHeight
                    ));
        }

        private void RenderColumnar(DrawingContext drawingContext, Pen pen,
            double x, double y, double height, HatItem? hat)
        {
            RenderColumnarHat(drawingContext, pen, x, y, ColumnWidth, height, hat);
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
            RenderColumnarRingEach(drawingContext, false, RenderColumnarHat);
        }

        private void RenderRing(DrawingContext drawingContext, double radiusRate,
            double maxAngle, double perimeterRate,
            RenderSpectumRingFunc func)
        {
            RenderRing(drawingContext, radiusRate, maxAngle, perimeterRate, true, func);
        }

        private void RenderRing(DrawingContext drawingContext, double radiusRate, 
            double maxAngle, double perimeterRate, bool hasCircle, 
            RenderSpectumRingFunc func)
        {
            var outerWidth = ColumnWidth + Space;
            var centerX = ActualWidth / 2;
            var centerY = ActualHeight / 2;
            var radius = Math.Min(centerX, centerY) * radiusRate;
            var columnCount = (int)Math.Min(Math.Max(Items.Length, 10), 
                Math.Floor(Math.PI * radius * 2 * perimeterRate / outerWidth));
            var preAngle = maxAngle / columnCount;
            if (Items.Length < 1)
            {
                return;
            }
            if (hasCircle)
            {
                drawingContext.DrawEllipse(new SolidColorBrush(Colors.Transparent),
                                new Pen(Foreground, 1), new Point(centerX, centerY), radius, radius);
            }
            var y = centerY - radius;
            var x = centerX - ColumnWidth / 2;
            RenderEach(drawingContext, 0, columnCount, Rate, y, (d, p, i, _, _, _, h) =>
            {
                func?.Invoke(d, p, i, x, y, ColumnWidth, h, i * preAngle, centerX, centerY, radius);
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
            double x, double y, double width, double height)
        {
            if (height < 2)
            {
                height = 1;
            }
            drawingContext.DrawRectangle(Foreground, pen
                , new Rect(
                    x, y - height, width, 2 * height
                    ));
        }

        private void RenderColumnarRingEach(DrawingContext drawingContext, bool isSymmetry,
            RenderSpectumHatFunc func)
        {
            RenderRing(drawingContext, .6, isSymmetry ? 180.0 : 360.0, isSymmetry ? .5 : 1, 
                (d, p, i, x, y, w, h, a, cx, cy, _) =>
            {
                var hat = GetHat(i, h);
                var tranform = new RotateTransform(a, cx, cy);
                drawingContext.PushTransform(tranform);
                func?.Invoke(d, p, x, y, w, h, hat);
                drawingContext.Pop();
                if (isSymmetry)
                {
                    tranform = new RotateTransform(-a, cx, cy);
                    drawingContext.PushTransform(tranform);
                    func?.Invoke(d, p, x, y, w, h, hat);
                    drawingContext.Pop();
                }
            });
        }

        private void RenderRingEach(DrawingContext drawingContext, bool isSymmetry,
            RenderSpectumRingPointFunc func)
        {
            RenderRing(drawingContext, .6, isSymmetry ? 180.0 : 360.0, 
                isSymmetry ? .5 : 1, false, (d, p, i, _, _, w, h, a, cx, cy, radius) =>
            {
                var len = radius + h;
                var angle = ToDeg(a);
                var x = cx + Math.Sin(angle) * len;
                var y = cy - Math.Cos(angle) * len;
                func?.Invoke(d, p, x, y);

                if (isSymmetry)
                {
                    func?.Invoke(d, p, cx * 2 - x , y);
                }
            });
        }

        private void RenderRingLine(DrawingContext drawingContext, bool isSymmetry)
        {
            var pen = new Pen(Foreground, 1);
            RenderRing(drawingContext, .6, isSymmetry ? 180.0 : 360.0,
                isSymmetry ? .5 : 1, false, (d, _, i, _, _, w, h, a, cx, cy, radius) =>
                {
                    var len = radius + h;
                    var angle = ToDeg(a);
                    var x = cx + Math.Sin(angle) * len;
                    var y = cy - Math.Cos(angle) * len;
                    len = radius - h;
                    var x2 = cx + Math.Sin(angle) * len;
                    var y2 = cy - Math.Cos(angle) * len;
                    if (x2 == x && y2 == y)
                    {
                        return;
                    }
                    d.DrawLine(pen, new Point(x, y), new Point(x2, y2));
                    if (isSymmetry)
                    {
                        d.DrawLine(pen, new Point(cx * 2 - x, y), new Point(cx * 2 - x2, y2));
                    }
                });
        }

        private void RenderSymmetryRing(DrawingContext drawingContext)
        {
            RenderColumnarRingEach(drawingContext, true, RenderColumnarHat);
        }

        private void RenderPolylineRing(DrawingContext drawingContext)
        {
            var i = -1;
            var first = new Point();
            var last = new Point();
            var pen = new Pen(Foreground, 1);
            RenderRingEach(drawingContext, false, (d, _, x, y) =>
            {
                i++;
                var point = new Point(x, y);
                if (i < 1)
                {
                    last = first = point;
                    return;
                }
                d.DrawLine(pen, last, point);
                last = point;
            });
            if (pen == null)
            {
                return;
            }
            drawingContext.DrawLine(pen, last, first);
        }

        private void RenderInversePolylineRing(DrawingContext drawingContext)
        {
            var i = -1;
            var last1 = new Point();
            var last2 = new Point();
            var pen = new Pen(Foreground, 1);
            RenderRingEach(drawingContext, true, (d, _, x, y) =>
            {
                i++;
                var point = new Point(x, y);
                if (i == 0)
                {
                    last1 = point;
                    return;
                }
                if (i == 1)
                {
                    last2 = point;
                    d.DrawLine(pen, last1, point);
                    return;
                }
                if (i % 2 == 0)
                {
                    d.DrawLine(pen, last1, point);
                    last1 = point;
                    return;
                }
                d.DrawLine(pen, last2, point);
                last2 = point;
            });
            drawingContext.DrawLine(pen, last1, last2);
        }

        private static double ToDeg(double a)
        {
            return a * Math.PI / 180;
        }
    }

    internal delegate void RenderSpectumHatFunc(DrawingContext context, Pen pen, double x, double y, double width, double height, HatItem? hat);
    internal delegate void RenderSpectumFunc(DrawingContext context, Pen pen, int dataIndex, double x, double y, double width, double height);
    internal delegate void RenderSpectumRingPointFunc(DrawingContext context, Pen pen, double x, double y);
    internal delegate void RenderSpectumRingFunc(DrawingContext context, Pen pen, int dataIndex, double x, double y, double width, double height, 
        double angle, double centerX, double centerY, double radius);
}
