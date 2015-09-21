using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Utils;
using System;
using Windows.UI.Xaml.Data;

namespace MSDevUnion.BingWallpaper.Converters
{
    public class WallpaperSizeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            //WallpaperSize wallpaperSize = (WallpaperSize)value;
            //string name = wallpaperSize.GetName();
            //if (wallpaperSize.GetWidth() == ScreenResolution.Width
            //    && wallpaperSize.GetHeight() == ScreenResolution.Height)
            //{
            //    name = name + LocalizedStrings.Default;
            //}
            //return name;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}