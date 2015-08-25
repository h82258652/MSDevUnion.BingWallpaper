using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Mvvm;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class Locator : ViewModelLocatorBase
    {
        public Locator()
        {
            // Register Services.
            Register<IScreenService, ScreenService>();
            Register<IServiceArea, ServiceArea>();

            // Register ViewModels.
            Register<MainViewModel>();
            Register<SettingViewModel>();
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
    }
}