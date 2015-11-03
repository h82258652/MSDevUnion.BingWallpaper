using BingoWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Mvvm;
using SoftwareKobo.UniversalToolkit.Services.LauncherServices;

namespace BingoWallpaper.ViewModels
{
    public class DetailViewModel : ViewModelBase
    {
        private DelegateCommand _openLockScreenSettingCommand;

        private DelegateCommand _openWallpaperSettingCommand;

        private Wallpaper _wallpaper;

        public DelegateCommand OpenLockScreenSettingCommand
        {
            get
            {
                _openLockScreenSettingCommand = _openLockScreenSettingCommand ?? new DelegateCommand(async () =>
                {
                    SystemSettingsService service = new SystemSettingsService();
                    await service.OpenLockScreenPageAsync();
                });
                return _openLockScreenSettingCommand;
            }
        }

        public DelegateCommand OpenWallpaperSettingCommand
        {
            get
            {
                _openWallpaperSettingCommand = _openWallpaperSettingCommand ?? new DelegateCommand(async () =>
                {
                    SystemSettingsService service = new SystemSettingsService();
                    await service.OpenPersonalizationPageAsync();
                });
                return _openWallpaperSettingCommand;
            }
        }

        public Wallpaper Wallpaper
        {
            get
            {
                return this._wallpaper;
            }
            set
            {
                this._wallpaper = value;
                this.RaisePropertyChanged(() => this.Wallpaper);
            }
        }
    }
}