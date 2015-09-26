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
                return wallpaper.GetCacheUrl(new WallpaperSize(1920, 1080));

                //if (wallpaper.IsLastInMonth)
                //{
                //    return wallpaper.GetUrl(new WallpaperSize(310, 150));
                //}
                //else
                //{
                //    return wallpaper.GetUrl(new WallpaperSize(150, 150));
                //}
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}