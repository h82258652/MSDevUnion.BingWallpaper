using BingoWallpaper.Models;
using BingoWallpaper.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace BingoWallpaper.Datas
{
    public static class AppRunningData
    {
        public static ObservableCollection<Wallpaper> Wallpapers
        {
            get;
            set;
        }

        public static async Task ReLoadWallpapers()
        {
            WallpaperService service = new WallpaperService();
            Wallpapers = new ObservableCollection<Wallpaper>(await service.GetWallpapersAsync(AppSetting.ViewMonth.Year, AppSetting.ViewMonth.Month, AppSetting.Area));
        }
    }
}