using BingoWallpaper.Datas;
using BingoWallpaper.Helpers;
using BingoWallpaper.Models;
using BingoWallpaper.Services;
using Windows.ApplicationModel.Background;

namespace BingoWallpaper.BackgroundTask
{
    public sealed class UpdateTileTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            try
            {
                WallpaperService service = new WallpaperService();
                Wallpaper wallpaper = await service.GetNewestWallpaperAsync(AppSetting.Area);
                if (wallpaper != null)
                {
                    TileHelper.UpdatePrimaryTile(wallpaper);
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}