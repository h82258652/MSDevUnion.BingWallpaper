using BingoWallpaper.Datas;
using BingoWallpaper.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BingoWallpaper.Models
{
    public class WallpaperCollection : ObservableCollection<Wallpaper>
    {
        private bool _isLoading;

        private readonly int _month;

        private readonly int _year;

        public WallpaperCollection(int year, int month)
        {
            _year = year;
            _month = month;
        }

        public string Cover
        {
            get
            {
                if (Count > 0)
                {
                    return this[0].GetCacheUrl(new WallpaperSize(1920, 1080));
                }
                return null;
            }
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            private set
            {
                _isLoading = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public int Month
        {
            get
            {
                return _month;
            }
        }

        public int Year
        {
            get
            {
                return _year;
            }
        }

        /// <summary>
        /// 重新加载该年月的壁纸信息。
        /// </summary>
        public async Task ReLoad()
        {
            if (IsLoading)
            {
                return;
            }

            IsLoading = true;

            ClearItems();

            try
            {
                WallpaperService service = new WallpaperService();
                var wallpapers = await service.GetWallpapersAsync(_year, _month, AppSetting.Area);// 加载指定年月，指定地区的壁纸信息。

                ClearItems();
                foreach (var wallpaper in wallpapers)
                {
                    Add(wallpaper);
                }
            }
            catch
            {
                // ignored
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Cover)));

            IsLoading = false;
        }
    }
}