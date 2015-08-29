using MSDevUnion.BingWallpaper.Datas;
using System;
using Windows.UI.Xaml.Data;

namespace MSDevUnion.BingWallpaper.Converters
{
    public class WallpaperUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string urlbase = (string)value;
            string size = AppSettings.WallpaperSize.ToString();
            return $"http://www.bing.com{urlbase}{size}.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}