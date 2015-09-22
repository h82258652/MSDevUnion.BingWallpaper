using BingoWallpaper.Models;
using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class SaveLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SaveLocation saveLocation = (SaveLocation)value;
            switch (saveLocation)
            {
                case SaveLocation.PictureLibrary:
                    return LocalizedStrings.PictureLibrary;

                case SaveLocation.ChooseEveryTime:
                    return LocalizedStrings.ChooseEveryTime;

                case SaveLocation.SavedPictures:
                    return LocalizedStrings.SavedPictures;

                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}