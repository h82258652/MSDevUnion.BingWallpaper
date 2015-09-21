using MSDevUnion.BingWallpaper.Models;
using System;
using Windows.UI.Xaml.Data;

namespace MSDevUnion.BingWallpaper.Converters
{
    public class ThumbnailUrlConverter : IValueConverter
    {
        private static int counter = 0;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BingWallpaperModel model = value as BingWallpaperModel;
            if (model != null)
            {
                string size = model.IsFirst ? "_310x150.jpg" : "_150x150.jpg";
                return "http://www.bing.com" + model.UrlBase + size;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}