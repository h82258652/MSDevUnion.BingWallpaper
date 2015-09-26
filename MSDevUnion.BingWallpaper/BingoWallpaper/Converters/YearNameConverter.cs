using BingoWallpaper.Models;
using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class YearNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var collection = value as WallpaperCollection;
            if (collection != null)
            {
                return collection.Year;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}