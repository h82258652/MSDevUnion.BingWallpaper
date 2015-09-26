using BingoWallpaper.Controls;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class IsUseThumbnailPanelInstanceConverter : IValueConverter
    {
        public event ItemClickEventHandler ItemClick;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                bool b = (bool)value;
                if (b)
                {
                    var panel = new ThumbnailPanel();
                    panel.ItemClick += (sender,e) => 
                    {
                        if (this.ItemClick!=null)
                        {
                            this.ItemClick(sender, e);
                        }
                    };
                    return panel;
                }
                else
                {
                    return null;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}