using SoftwareKobo.UniversalToolkit.Mvvm;
using SoftwareKobo.UniversalToolkit.Services.LauncherServices;

namespace BingoWallpaper.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private DelegateCommand _reviewCommand;

        public string DisplayName
        {
            get
            {
                return LocalizedStrings.DisplayName;
            }
        }

        public DelegateCommand ReviewCommand
        {
            get
            {
                _reviewCommand = _reviewCommand ?? new DelegateCommand(async () =>
                {
                    StoreService service = new StoreService();
                    await service.OpenCurrentAppReviewPageAsync();
                });
                return _reviewCommand;
            }
        }
    }
}