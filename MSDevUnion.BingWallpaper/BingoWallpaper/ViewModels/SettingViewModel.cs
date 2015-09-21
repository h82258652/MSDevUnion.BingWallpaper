using BingoWallpaper.Datas;
using BingoWallpaper.Services;
using BingoWallpaper.Utils;
using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace BingoWallpaper.ViewModels
{
    /// <summary>
    /// 设置页视图模型。
    /// </summary>
    public sealed class SettingViewModel : ViewModelBase
    {
        private bool _isWallpaperSizeReady;

        public SettingViewModel()
        {
        }

        public string Area
        {
            get
            {
                return AppSetting.Area;
            }
            set
            {
                AppSetting.Area = value;
                RaisePropertyChanged(() => Area);
            }
        }

        public IReadOnlyList<string> Areas
        {
            get
            {
                return ServiceArea.All;
            }
        }
        
        public string SaveLocation
        {
            get
            {
                return AppSetting.SaveLocation;
            }
            set
            {
                AppSetting.SaveLocation = value;
                RaisePropertyChanged(() => SaveLocation);
            }
        }

        public IReadOnlyList<string> SaveLocations
        {
            get
            {
                string[] list = new string[]
                {
                    LocalizedStrings.PictureLibrary,
                    LocalizedStrings.ChooseEveryTime,
                    LocalizedStrings.SavedPictures
                };
                return list;
            }
        }

        public Models.WallpaperSize WallpaperSize
        {
            get
            {
                return AppSetting.WallpaperSize;
            }
            set
            {
                AppSetting.WallpaperSize = value;
                RaisePropertyChanged(() => WallpaperSize);
            }
        }

        public IReadOnlyList<Models.WallpaperSize> WallpaperSizes
        {
            get
            {
                return Models.WallpaperSize.SupportWallpaperSizes;
            }
        }
    }
}