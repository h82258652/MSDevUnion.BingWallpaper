using Windows.Storage;

namespace MSDevUnion.BingWallpaper.Services
{
    public class RoamingSetting : IRoamingSetting
    {
        public string Load(string key)
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {
                return (string)ApplicationData.Current.RoamingSettings.Values[key];
            }
            return string.Empty;
        }

        public void Save(string key, string value)
        {
            ApplicationData.Current.RoamingSettings.Values[key] = value;
        }
    }
}