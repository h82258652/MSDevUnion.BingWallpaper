using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SoftwareKobo.UniversalToolkit.Services.LauncherServices;

namespace BingoWallpaper.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private RelayCommand _reviewCommand;

        public string DisplayName
        {
            get
            {
                return LocalizedStrings.DisplayName;
            }
        }

        public RelayCommand ReviewCommand
        {
            get
            {
                _reviewCommand = _reviewCommand ?? new RelayCommand(async () =>
                {
                    StoreService service = new StoreService();
                    await service.OpenCurrentAppReviewPageAsync();
                });
                return _reviewCommand;
            }
        }
    }
}