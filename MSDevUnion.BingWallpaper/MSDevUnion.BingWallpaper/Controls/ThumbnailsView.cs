using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MSDevUnion.BingWallpaper.Controls
{
    public class ThumbnailsView : CalendarView
    {
        public ThumbnailsView() : base()
        {
            this.CalendarViewDayItemChanging += ThumbnailsView_CalendarViewDayItemChanging;
        }

        private void ThumbnailsView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            CalendarViewDayItem dayItem = args.Item;

            if (dayItem.DataContext == null)
            {
                dayItem.DataContext = dayItem;
            }

            int count = VisualTreeHelper.GetChildrenCount(dayItem);
            TextBlock numberTextBlock = VisualTreeHelper.GetChild(dayItem, count - 1) as TextBlock;
            if (numberTextBlock != null)
            {
                numberTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
}