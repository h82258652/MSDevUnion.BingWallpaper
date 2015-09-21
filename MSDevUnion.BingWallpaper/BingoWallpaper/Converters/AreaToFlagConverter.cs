using System;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class AreaToFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string area = (string)value;
            switch (area)
            {
                case "de-DE":
                    return "/Assets/flags/Germany.png";

                case "en-AU":
                    return "/Assets/flags/Australia.png";

                case "en-CA":
                    return "/Assets/flags/Canada.png";

                case "en-GB":
                    return "/Assets/flags/United_Kingdom.png";

                case "en-IN":
                    return "/Assets/flags/India.png";

                case "en-US":
                    return "/Assets/flags/United_States.png";

                case "fr-CA":
                    return "/Assets/flags/Canada.png";

                case "fr-FR":
                    return "/Assets/flags/France.png";

                case "ja-JP":
                    return "/Assets/flags/Japan.png";

                case "pt-BR":
                    return "/Assets/flags/Brazil.png";

                case "zh-CN":
                    return "/Assets/flags/China.png";

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}