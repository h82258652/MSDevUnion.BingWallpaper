using BingoWallpaper.Datas;
using BingoWallpaper.Helpers;
using BingoWallpaper.Models;
using BingoWallpaper.Services;
using BingoWallpaper.Utils;
using SoftwareKobo.UniversalToolkit.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

namespace BingoWallpaper.Controls
{
    public sealed partial class ExtendedSplashScreen : ExtendedSplashScreenContent
    {
        private const string UPDATE_TILE_TASK_NAME = @"UpdateTileTask";

        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            this.Loaded += async (sender, e) =>
            {
                await this.HideStatusBar();
                await this.InitSuitableWallpaperSize();
                await this.RegisterBackgroundTask();
                await this.UpdatePrimaryTile();
                this.Finish();
            };
        }

        private async Task HideStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
        }

        /// <summary>
        /// 初始化最适合壁纸大小。
        /// </summary>
        /// <returns></returns>
        private async Task InitSuitableWallpaperSize()
        {
            await SuitableWallpaperSize.InitAsync();
            WallpaperSize wallpaperSize = AppSetting.WallpaperSize;
            if (wallpaperSize == null)
            {
                wallpaperSize = new WallpaperSize(SuitableWallpaperSize.Width, SuitableWallpaperSize.Height);
                if (WallpaperSize.SupportWallpaperSizes.Contains(wallpaperSize))
                {
                    AppSetting.WallpaperSize = wallpaperSize;
                }
                else
                {
                    AppSetting.WallpaperSize = new WallpaperSize(800, 600);
                }
            }
        }

        private async Task RegisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == UPDATE_TILE_TASK_NAME)
                {
                    // 已经注册后台任务。
                    return;
                }
            }

            BackgroundAccessStatus access = await BackgroundExecutionManager.RequestAccessAsync();

            if (access != BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
            {
                // 没有权限。
                return;
            }

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            // 仅在网络可用下执行后台任务。
            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

            builder.Name = UPDATE_TILE_TASK_NAME;
            builder.TaskEntryPoint = "BingoWallpaper.BackgroundTask.UpdateTileTask";
            // 每 90 分钟触发一次。
            builder.SetTrigger(new TimeTrigger(90, false));

            BackgroundTaskRegistration registration = builder.Register();
        }

        private async Task UpdatePrimaryTile()
        {
            try
            {
                WallpaperService service = new WallpaperService();
                Wallpaper wallpaper = await service.GetNewestWallpaperAsync(AppSetting.Area);
                if (wallpaper != null)
                {
                    TileHelper.UpdatePrimaryTile(wallpaper);
                }
            }
            catch
            {
            }
        }
    }
}