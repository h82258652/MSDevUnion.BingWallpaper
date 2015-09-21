using BingoWallpaper.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
