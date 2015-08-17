namespace MSDevUnion.BingWallpaper.Services
{
    public interface ILocalSetting
    {
        void Save(string key, string value);

        string Load(string key);
    }
}