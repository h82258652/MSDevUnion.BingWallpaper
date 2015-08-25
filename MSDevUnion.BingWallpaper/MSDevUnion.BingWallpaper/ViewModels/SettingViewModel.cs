using GalaSoft.MvvmLight;
using MSDevUnion.BingWallpaper.Datas;
using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private IServiceArea _serviceArea;

        public SettingViewModel(IServiceArea serviceArea)
        {
            this._serviceArea = serviceArea;
        }

        public string[] AllSaveLocation
        {
            get;
        } = new string[]
        {
            LocalizedStrings.PictureLibrary,
            LocalizedStrings.ChooseEveryTime,
            LocalizedStrings.SavedPictures
        };

        public string Area
        {
            get
            {
                return AppSettings.Area;
            }
            set
            {
                AppSettings.Area = value;
                RaisePropertyChanged();
            }
        }

        public IReadOnlyList<string> Areas
        {
            get
            {
                return this._serviceArea.AllSupportAreas;
            }
        }

        public string SaveLocation
        {
            get
            {
                return AppSettings.SaveLocation;
            }
            set
            {
                AppSettings.SaveLocation = value;
                RaisePropertyChanged();
            }
        }

        public IList<WallpaperSize> WallpaperSizes
        {
            get;
        } = Enum.GetValues(typeof(WallpaperSize)).Cast<WallpaperSize>().ToList();

        public WallpaperSize WallpaperSize
        {
            get
            {
                return AppSettings.WallpaperSize;
            }
            set
            {
                AppSettings.WallpaperSize = value;
                RaisePropertyChanged();
            }
        }
    }
}