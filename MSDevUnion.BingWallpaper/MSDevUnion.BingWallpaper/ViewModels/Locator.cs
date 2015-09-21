using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Mvvm;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class Locator : ViewModelLocatorBase
    {
        public Locator()
        {
            // Register Services.
            Register<IBingWallpaperService, BingWallpaperService>();
            Register<IServiceArea, ServiceArea>();

            // Register ViewModels.
            Register<MainViewModel>();
            Register<SettingViewModel>();
            Register<DetailViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return GetInstance<MainViewModel>();
            }
        }

        public SettingViewModel Setting
        {
            get
            {
                return GetInstance<SettingViewModel>();
            }
        }

        public DetailViewModel Detail
        {
            get
            {
                return GetInstance<DetailViewModel>();
            }
        }
    }
}