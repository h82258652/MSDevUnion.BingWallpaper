using MSDevUnion.BingWallpaper.Datas;
using MSDevUnion.BingWallpaper.Models;
using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace MSDevUnion.BingWallpaper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DelegateCommand _previousYearCommand;
        private DelegateCommand _previousMonthCommand;
        private DelegateCommand _nextMonthCommand;
        private DelegateCommand _nextYearCommand;

        public DelegateCommand PreviousYearCommand
        {
            get
            {
                if (_previousYearCommand == null)
                {
                    _previousYearCommand = new DelegateCommand(() =>
                    {
                        this.ViewingDate = this.ViewingDate.AddYears(-1);
                    });
                }
                return _previousYearCommand;
            }
        }

        public DelegateCommand PreviousMonthCommand
        {
            get
            {
                if (_previousMonthCommand == null)
                {
                    _previousMonthCommand = new DelegateCommand(() =>
                    {
                        this.ViewingDate = this.ViewingDate.AddMonths(-1);
                    });
                }
                return _previousMonthCommand;
            }
        }

        public DelegateCommand NextMonthCommand
        {
            get
            {
                if (_nextMonthCommand == null)
                {
                    _nextMonthCommand = new DelegateCommand(() =>
                    {
                        this.ViewingDate = this.ViewingDate.AddMonths(1);
                    });
                }
                return _nextMonthCommand;
            }
        }

        public DelegateCommand NextYearCommand
        {
            get
            {
                if (_nextYearCommand == null)
                {
                    _nextYearCommand = new DelegateCommand(() =>
                    {
                        this.ViewingDate = this.ViewingDate.AddYears(1);
                    });
                }
                return _nextYearCommand;
            }
        }

        public ObservableCollection<BingWallpaperModel> ViewingBingWallpapers
        {
            get
            {
                return AppRunningDatas.CurrentViewingBingWallpapers;
            }
            set
            {
                AppRunningDatas.CurrentViewingBingWallpapers = value;
                RaisePropertyChanged();
            }
        }

        public DateTimeOffset ViewingDate
        {
            get
            {
                return AppSettings.LastViewDate;
            }
            set
            {
                AppSettings.LastViewDate = value;
                RaisePropertyChanged();
            }
        }

        public async void LoadBingWallpapers()
        {
            string market = AppSettings.Area;
            int year = ViewingDate.Year;
            int month = ViewingDate.Month;
           this.ViewingBingWallpapers = await PackagedBingWallpaperService.GetAsync(market, year, month);
        }
    }
}