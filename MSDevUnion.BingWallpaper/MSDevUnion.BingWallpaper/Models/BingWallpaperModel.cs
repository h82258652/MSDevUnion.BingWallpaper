namespace MSDevUnion.BingWallpaper.Models
{
    public class BingWallpaperModel
    {
        public string UrlBase
        {
            get;
            set;
        }

        public Hotspot[] Hotspots
        {
            get;
            set;
        }

        public string Info
        {
            get;
            set;
        }

        public bool ExistWUXGA
        {
            get;
            set;
        }

        public bool IsFirst
        {
            get;
            set;
        }
    }
}