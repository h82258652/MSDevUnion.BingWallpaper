using System;
using Windows.UI.Xaml.Data;

namespace MSDevUnion.BingWallpaper.Converters
{
    public class AreaToFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string area = (string)value;
            switch (area)
            {
                case "de-DE":
                    return "/Assets/Germany.png";

                case "en-AU":
                    return "/Assets/Australia.png";

                case "en-CA":
                    return "/Assets/Canada.png";

                case "en-GB":
                    return "/Assets/United_Kingdom.png";

                case "en-IN":
                    return "/Assets/India.png";
                    
                case "en-US":
                    return "/Assets/United_States.png";

                case "fr-CA":
                    return "/Assets/Canada.png";

                case "fr-FR":
                    return "/Assets/France.png";

                case "ja-JP":
                    return "/Assets/Japan.png";

                case "pt-BR":
                    return "/Assets/Brazil.png";

                case "zh-CN":
                    return "/Assets/China.png";

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