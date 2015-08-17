using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MSDevUnion.BingWallpaper.Services;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Register Services.
            SimpleIoc.Default.Register<IScreenService, ScreenService>();
            SimpleIoc.Default.Register<IServiceArea, ServiceArea>();

            // Register ViewModels.
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public DetailViewModel Detail
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DetailViewModel>();
            }
        }

        public IServiceArea ServiceArea
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IServiceArea>();
            }
        }
    }
}