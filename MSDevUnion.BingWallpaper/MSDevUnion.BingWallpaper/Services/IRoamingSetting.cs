namespace MSDevUnion.BingWallpaper.Services
{
    public interface IRoamingSetting
    {
        void Save(string key, string value);

        string Load(string key);
    }
}