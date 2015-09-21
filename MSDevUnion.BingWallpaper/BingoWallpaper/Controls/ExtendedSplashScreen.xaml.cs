using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.Services;
using BingoWallpaper.Utils;
using SoftwareKobo.UniversalToolkit.Controls;
using System.Linq;
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BingoWallpaper.Controls
{
    public sealed partial class ExtendedSplashScreen : ExtendedSplashScreenContent
    {
        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            this.Loaded += async (sender, e) =>
            {
                this.InitLeanCloudService();
                await this.InitSuitableWallpaperSize();
                Finish();
            };
        }

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

        private void InitLeanCloudService()
        {
            WallpaperService.LeanCloudAppId = "2odv0fmdni1w22hceawylo48l76vxbltgpl1mnoq3hlxj55j";
            WallpaperService.LeanCloudAppKey = "idsoc6l9k218zrge2qi06anel3qcoqgvhutbqm93e4l58d3i";
        }

        private void RegisterBackgroundTask()
        {
        }
    }
}