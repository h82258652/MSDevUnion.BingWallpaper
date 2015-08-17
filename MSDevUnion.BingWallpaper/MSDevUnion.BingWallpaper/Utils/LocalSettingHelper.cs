using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MSDevUnion.BingWallpaper.Utils
{
    public static class LocalSettingHelper
    {
        public static void Save(string key,string value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static string Load(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return (string)ApplicationData.Current.LocalSettings.Values[key];
            }
            return string.Empty;
        }
    }
}
