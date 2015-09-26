using BingoWallpaper.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class BackgroundImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            WallpaperCollection collection = value as WallpaperCollection;
            if (collection == null)
            {
                return value;
            }
            Wallpaper wallpaper = collection.FirstOrDefault();
            if (wallpaper == null)
            {
                return value;
            }

            return wallpaper.GetCacheUrl(new WallpaperSize(1920, 1080));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}