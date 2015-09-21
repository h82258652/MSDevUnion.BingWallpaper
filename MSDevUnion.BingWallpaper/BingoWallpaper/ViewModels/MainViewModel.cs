using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BingoWallpaper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _backgroundImage;

        private RelayCommand _nextMonthCommand;

        private RelayCommand _previousMonthCommand;

        public MainViewModel()
        {
            this.LoadWallpaper();
            //  BackgroundImage = "http://www.bing.com/az/hprichbg/rb/BratwurstPolka_ZH-CN13791851201_1920x1080.jpg";
        }

        public string BackgroundImage
        {
            get
            {
                string url = null;
                if (this.Wallpapers != null)
                {
                    var first = this.Wallpapers.FirstOrDefault();
                    if (first != null)
                    {
                        url = "http://www.bing.com" + first.Image.UrlBase + "_1920x1080.jpg";
                    }
                }
                return url;
            }
        }

        public RelayCommand NextMonthCommand
        {
            get
            {
                this._nextMonthCommand = this._nextMonthCommand ?? new RelayCommand(() =>
                {
                    this.ViewMonth = this.ViewMonth.AddMonths(1);
                }, () => true);
                return this._nextMonthCommand;
            }
        }

        public RelayCommand PreviousMonthCommand
        {
            get
            {
                this._previousMonthCommand = this._previousMonthCommand ?? new RelayCommand(() =>
                {
                    this.ViewMonth = this.ViewMonth.AddMonths(-1);
                }, () => true);
                return this._previousMonthCommand;
            }
        }

        public DateTimeOffset ViewMonth
        {
            get
            {
                return AppSetting.ViewMonth;
            }
            set
            {
                AppSetting.ViewMonth = value;
                RaisePropertyChanged(() => ViewMonth);
                RaisePropertyChanged(() => PreviousMonthCommand);
                RaisePropertyChanged(() => NextMonthCommand);
            }
        }

        public ObservableCollection<Wallpaper> Wallpapers
        {
            get
            {
                return AppRunningData.Wallpapers;
            }
            set
            {
                AppRunningData.Wallpapers = value;
                RaisePropertyChanged(() => Wallpapers);
                RaisePropertyChanged(() => BackgroundImage);
            }
        }

        private async void LoadWallpaper()
        {
            try
            {
                await AppRunningData.ReLoadWallpapers();
                this.Wallpapers = AppRunningData.Wallpapers;
            }
            catch
            {
            }
        }
    }
}