using MSDevUnion.BingWallpaper.Models;
using System.Collections.ObjectModel;

namespace MSDevUnion.BingWallpaper.Datas
{
    public static class AppRunningDatas
    {
        public static ObservableCollection<BingWallpaperModel> CurrentViewingBingWallpapers
        {
            get;
            set;
        }
    }
}