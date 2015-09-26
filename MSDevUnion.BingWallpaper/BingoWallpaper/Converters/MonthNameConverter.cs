using BingoWallpaper.Models;
using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace BingoWallpaper.Converters
{
    public class MonthNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var collection = value as WallpaperCollection;
            if (collection != null)
            {
                if (CultureInfo.CurrentCulture.Name == "zh-CN")
                {
                    return collection.Month + "月";
                }
                else
                {
                    switch (collection.Month)
                    {
                        case 1:
                            return "January";

                        case 2:
                            return "February";

                        case 3:
                            return "March";

                        case 4:
                            return "April";

                        case 5:
                            return "May";

                        case 6:
                            return "June";

                        case 7:
                            return "July";

                        case 8:
                            return "August";

                        case 9:
                            return "September";

                        case 10:
                            return "October";

                        case 11:
                            return "November";

                        case 12:
                            return "December";
                    }
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