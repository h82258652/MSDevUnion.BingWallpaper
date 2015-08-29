using MSDevUnion.BingWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Mvvm;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class DetailViewModel : ViewModelBase
    {
        public Hotspot[] Hotspots
        {
            get
            {
                return _bingWallpaper.Hotspots;
            }
        }

        public string UrlBase
        {
            get
            {
                return _bingWallpaper.UrlBase;
            }
        }

        public BingWallpaperModel BingWallpaper
        {
            get
            {
                return _bingWallpaper;
            }
            set
            {
                _bingWallpaper = value;
                RaisePropertyChanged();
            }
        }

        private BingWallpaperModel _bingWallpaper;

        public void SetBingWallpaper(BingWallpaperModel bingWallpaper)
        {
            this.BingWallpaper = bingWallpaper;
            RaiseAllPropertiesChanged();
        }
    }
}