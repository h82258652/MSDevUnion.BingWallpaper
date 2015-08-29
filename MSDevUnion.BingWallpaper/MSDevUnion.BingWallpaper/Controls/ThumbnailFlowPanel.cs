using MSDevUnion.BingWallpaper.Models;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MSDevUnion.BingWallpaper.Controls
{
    public class ThumbnailFlowPanel : Panel
    {
        public int FlowCount
        {
            get
            {
                return (int)this.GetValue(FlowCountProperty);
            }
            set
            {
                this.SetValue(FlowCountProperty, value);
            }
        }

        public static readonly DependencyProperty FlowCountProperty = DependencyProperty.Register(nameof(FlowCount), typeof(int), typeof(ThumbnailFlowPanel), new PropertyMetadata(2));

        protected override Size MeasureOverride(Size availableSize)
        {
            // 前面每行所使用的宽度。
            List<double> currentRowsUsedWidth = new List<double>();
            // 总共使用高度。
            double currentUsedHeight = 0;

            // 计算每个纵向流的宽度。
            double flowWidth = availableSize.Width / this.FlowCount;

            foreach (UIElement element in Children)
            {
                // 子控件的高度和宽度。
                double elementWidth = flowWidth;
                double elementHeight = flowWidth;

                FrameworkElement child = element as FrameworkElement;
                if (child != null)
                {
                    BingWallpaperModel bingWallpaper = child.DataContext as BingWallpaperModel;
                    if (bingWallpaper != null && bingWallpaper.ExistWUXGA)
                    {
                        // 该子控件双倍宽度。
                        elementWidth = elementWidth * 2;
                    }
                }
                element.Measure(new Size(elementWidth, elementHeight));

                Size elementSize = element.DesiredSize;

                // 子控件是否在已使用行中安置好。
                bool hadSet = false;
                for (int i = currentRowsUsedWidth.Count - 1; i >= 0; i--)
                {
                    double rowUsedWidth = currentRowsUsedWidth[i];
                    if (elementSize.Width <= availableSize.Width - rowUsedWidth)
                    {
                        rowUsedWidth = rowUsedWidth - elementSize.Width;
                        currentRowsUsedWidth[i] = rowUsedWidth;
                        hadSet = true;
                        break;
                    }
                }
                if (hadSet == false)
                {
                    // 已有行空间不足，新开一行。
                    currentRowsUsedWidth.Add(elementSize.Width);
                    currentUsedHeight = currentUsedHeight + elementSize.Height;
                }
            }

            return new Size(availableSize.Width, currentUsedHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // 前面每行所使用的宽度。
            List<double> currentRowsUsedWidth = new List<double>();
            // 总共使用高度。
            double currentUsedHeight = 0;

            // 计算每个纵向流的宽度。
            double flowWidth = finalSize.Width / this.FlowCount;

            foreach (UIElement element in Children)
            {
                Size elementSize = element.DesiredSize;

                // 子控件是否在已使用行中安置好。
                bool hadSet = false;
                for (int i = currentRowsUsedWidth.Count - 1; i >= 0; i--)
                {
                    double rowUsedWidth = currentRowsUsedWidth[i];
                    if (elementSize.Width <= finalSize.Width - rowUsedWidth)
                    {
                        Point location = new Point(rowUsedWidth, currentUsedHeight);
                        element.Arrange(new Rect(location, elementSize));

                        rowUsedWidth = rowUsedWidth - elementSize.Width;
                        currentRowsUsedWidth[i] = rowUsedWidth;
                        hadSet = true;
                        break;
                    }
                }
                if (hadSet == false)
                {
                    // 已有行空间不足，新开一行。
                    element.Arrange(new Rect(new Point(0, currentUsedHeight), elementSize));

                    currentRowsUsedWidth.Add(elementSize.Width);
                    currentUsedHeight = currentUsedHeight + elementSize.Height;
                }
            }

            return finalSize;
        }
    }
}