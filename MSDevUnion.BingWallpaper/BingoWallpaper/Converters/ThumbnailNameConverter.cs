using BingoWallpaper.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class ThumbnailNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Wallpaper wallpaper = value as Wallpaper;
            string name = null;
            if (wallpaper != null)
            {
                var message = wallpaper.Archive?.Messages.FirstOrDefault();
                if (message != null)
                {
                    name = message.Text;
                }
                if (name == null)
                {
                    name = wallpaper.Archive.Info;
                }
                return name;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}