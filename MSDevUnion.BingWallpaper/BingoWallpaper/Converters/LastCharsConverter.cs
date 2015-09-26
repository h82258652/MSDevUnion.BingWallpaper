using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class LastCharsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            if (str != null)
            {
                if (str.Length > 0)
                {
                    return str.Substring(1);
                }
                return str;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}