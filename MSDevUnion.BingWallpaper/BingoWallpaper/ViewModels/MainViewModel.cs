using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
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
        public IList<WallpaperCollection> AllWallpapers
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
                _refreshCommand = _refreshCommand ?? new RelayCommand(async () =>
                {
                    var index = AllWallpapers.IndexOf(ViewingWallpaper);
                    var previous = AllWallpapers.ElementAtOrDefault(index - 1);
                    var next = AllWallpapers.ElementAtOrDefault(index + 1);

                    await ViewingWallpaper.ReLoad();
                    if (previous != null)
                    {
                        await previous.ReLoad();
                    }
                    if (next != null)
                    {
                        await next.ReLoad();
                    }
                });
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