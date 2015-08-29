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
            if (model!=null)
            {
                if (counter == 0 || counter == 1)
                {
                    counter++;
                    return "http://www.bing.com" + model.UrlBase + "_150x150.jpg";

                }
                else
                {
                    counter = 0;
                    return "http://www.bing.com" + model.UrlBase + "_310x150.jpg" ;
                }

            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}