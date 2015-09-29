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
                    bool isNarrow = string.Equals(parameter, "Narrow");
                    switch (collection.Month)
                    {
                        case 1:
                            return isNarrow ? "Jan." : "January";

                        case 2:
                            return isNarrow ? "Feb." : "February";

                        case 3:
                            return isNarrow ? "Mar." : "March";

                        case 4:
                            return isNarrow ? "Apr." : "April";

                        case 5:
                            return isNarrow ? "May." : "May";

                        case 6:
                            return isNarrow ? "Jun." : "June";

                        case 7:
                            return isNarrow ? "Jul." : "July";

                        case 8:
                            return isNarrow ? "Aug." : "August";

                        case 9:
                            return isNarrow ? "Sep." : "September";

                        case 10:
                            return isNarrow ? "Oct." : "October";

                        case 11:
                            return isNarrow ? "Nov." : "November";

                        case 12:
                            return isNarrow ? "Dec." : "December";
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