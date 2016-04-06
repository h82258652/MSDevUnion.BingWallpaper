using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class WallpaperUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Wallpaper wallpaper = value as Wallpaper;
            if (wallpaper != null)
            {
                string url = wallpaper.GetCacheUrl(AppSetting.WallpaperSize);
                return url;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}