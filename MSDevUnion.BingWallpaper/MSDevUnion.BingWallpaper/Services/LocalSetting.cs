using Windows.Storage;

namespace MSDevUnion.BingWallpaper.Services
{
    public class LocalSetting : ILocalSetting
    {
        public string Load(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return (string)ApplicationData.Current.LocalSettings.Values[key];
            }
            return string.Empty;
        }

        public void Save(string key, string value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}