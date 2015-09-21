using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class AreaToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string area = (string)value;
            string name = null;
            switch (area)
            {
                case "de-DE":
                    name = LocalizedStrings.Germany;
                    break;

                case "en-AU":
                    name = LocalizedStrings.Australia;
                    break;

                case "en-CA":
                    name = LocalizedStrings.Canada_English;
                    break;

                case "en-GB":
                    name = LocalizedStrings.United_Kingdom;
                    break;

                case "en-IN":

                    name = LocalizedStrings.India;
                    break;

                case "en-US":
                    name = LocalizedStrings.United_States;
                    break;

                case "fr-CA":
                    name = LocalizedStrings.Canada_French;
                    break;

                case "fr-FR":
                    name = LocalizedStrings.France;
                    break;

                case "ja-JP":
                    name = LocalizedStrings.Japan;
                    break;

                case "pt-BR":
                    name = LocalizedStrings.Brazil;
                    break;

                case "zh-CN":
                    name = LocalizedStrings.China;
                    break;
            }
            return $"{name}({area})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}