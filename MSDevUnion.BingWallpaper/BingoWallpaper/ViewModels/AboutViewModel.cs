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
                if (this._reviewCommand == null)
                {
                    this._reviewCommand = new DelegateCommand(async () =>
                    {
                        StoreService service = new StoreService();
                        await service.OpenCurrentAppReviewPageAsync();
                    });
                }
                return this._reviewCommand;
            }
        }
    }
}