using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BingoWallpaper.ViewModels
{
    /// <summary>
    /// 设置页视图模型。
    /// </summary>
    public sealed class SettingViewModel : ViewModelBase
    {
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

        public SaveLocation SaveLocation
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

        public IReadOnlyList<SaveLocation> SaveLocations
        {
            get
            {
                return Enum.GetValues(typeof(SaveLocation)).Cast<SaveLocation>().ToList();
            }
        }

        public WallpaperSize WallpaperSize
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

        public IReadOnlyList<WallpaperSize> WallpaperSizes
        {
            get
            {
                return WallpaperSize.SupportWallpaperSizes;
            }
        }
    }
}