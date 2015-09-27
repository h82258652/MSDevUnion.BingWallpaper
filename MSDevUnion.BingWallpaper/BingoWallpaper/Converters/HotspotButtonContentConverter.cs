using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class HotspotButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isOpen = (bool)value;
            if (isOpen)
            {
                return "/Assets/lightbulb-online.png";
            }
            else
            {
                return "/Assets/lightbulb-outline.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}