using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BingoWallpaper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _nextMonthCommand;

        private RelayCommand _previousMonthCommand;

        public MainViewModel()
        {
        }

        public string BackgroundImage
        {
            get
            {
                string url = null;
                if (this.Wallpapers != null)
                {
                    var first = this.Wallpapers.FirstOrDefault();
                    if (first!=null)
                    {
                        url = first.GetUrl(new WallpaperSize(1920, 1080));
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
                    this.LoadWallpaper();
                }, () => this.ViewMonth <= DateTimeOffset.Now.AddMinutes(-1));
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
                    this.LoadWallpaper();
                },
                () => this.ViewMonth >= AppSetting.MIN_VIEW_MONTH.AddMonths(1));
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
                PreviousMonthCommand.RaiseCanExecuteChanged();
                NextMonthCommand.RaiseCanExecuteChanged();
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

        private static Task _loadWallpapersTask;

        public async void LoadWallpaper()
        {
            try
            {
                if (_loadWallpapersTask != null)
                {
                    _loadWallpapersTask.AsAsyncAction().Cancel();
                }

                _loadWallpapersTask = AppRunningData.ReLoadWallpapers();
                await _loadWallpapersTask;
                this.Wallpapers = AppRunningData.Wallpapers;
            }
            catch
            {
            }
        }
    }
}