using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Mvvm;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class ViewModelLocator : ViewModelLocatorBase
    {
        public ViewModelLocator()
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