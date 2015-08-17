using GalaSoft.MvvmLight;
using MSDevUnion.BingWallpaper.Services;
using System.Collections.Generic;

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
            //LocalizedStrings.PictureLibrary,
            //LocalizedStrings.ChooseEveryTime,
            //LocalizedStrings.SavedPictures
        };

        public string Area
        {
            get;
            set;
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
                return null;
            }
        }
    }
}