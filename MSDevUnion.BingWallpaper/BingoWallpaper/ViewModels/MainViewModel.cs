using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;

namespace BingoWallpaper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _refreshCommand;

        private WallpaperCollection _viewingWallpaper;

        public MainViewModel()
        {
        }

        /// <summary>
        /// 所有壁纸信息。
        /// </summary>
        public ObservableCollection<WallpaperCollection> AllWallpapers
        {
            get
            {
                return AppRunningData.AllWallpapers;
            }
        }

        /// <summary>
        /// 刷新所有壁纸信息命令。
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new RelayCommand(ReloadAll);
                return _refreshCommand;
            }
        }

        /// <summary>
        /// 正在查看的壁纸信息。
        /// </summary>
        public WallpaperCollection ViewingWallpaper
        {
            get
            {
                if (_viewingWallpaper == null)
                {
                    _viewingWallpaper = AllWallpapers.LastOrDefault();
                }
                return _viewingWallpaper;
            }
            set
            {
                _viewingWallpaper = value;
                RaisePropertyChanged(() => ViewingWallpaper);
            }
        }

        /// <summary>
        /// 重新加载所有壁纸信息。
        /// </summary>
        public void ReloadAll()
        {
            AppRunningData.ReloadAll();
            RaisePropertyChanged(() => AllWallpapers);
        }
    }
}