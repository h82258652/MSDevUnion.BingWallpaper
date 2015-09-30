using BingoWallpaper.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SoftwareKobo.UniversalToolkit.Helpers;
using SoftwareKobo.UniversalToolkit.Services.LauncherServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BingoWallpaper.ViewModels
{
    public class DetailViewModel : ViewModelBase
    {
        private Wallpaper _wallpaper;

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

        private RelayCommand _openLockScreenSettingCommand;

        public RelayCommand OpenLockScreenSettingCommand
        {
            get
            {
                _openLockScreenSettingCommand = _openLockScreenSettingCommand ?? new RelayCommand(async () =>
                {
                    SystemSettingsService service = new SystemSettingsService();
                    await service.OpenLockScreenPageAsync();
                });
                return _openLockScreenSettingCommand;
            }
        }

        private RelayCommand _openWallpaperSettingCommand;

        public RelayCommand OpenWallpaperSettingCommand
        {
            get
            {
                _openWallpaperSettingCommand = _openWallpaperSettingCommand ?? new RelayCommand(async () =>
                {
                    SystemSettingsService service = new SystemSettingsService();
                    await service.OpenPersonalizationPageAsync();
                });
                return _openWallpaperSettingCommand;
            }
        }
    }
}
