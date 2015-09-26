using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class FirstCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            if (str != null)
            {
                return str.ElementAtOrDefault(0);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}