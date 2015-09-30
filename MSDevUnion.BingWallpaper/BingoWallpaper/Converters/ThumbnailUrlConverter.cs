using BingoWallpaper.Models;
using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class ThumbnailUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Wallpaper wallpaper = value as Wallpaper;
            if (wallpaper != null)
            {
                return wallpaper.GetCacheUrl(new WallpaperSize(800, 480));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}