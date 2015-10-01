using BingoWallpaper.Datas;
using BingoWallpaper.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BingoWallpaper.Models
{
    public class WallpaperCollection : ObservableCollection<Wallpaper>
    {
        private bool _isLoading;
        private int _month;
        private int _year;

        public WallpaperCollection(int year, int month)
        {
            this._year = year;
            this._month = month;
        }

        public string Cover
        {
            get
            {
                if (this.Count > 0)
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
                return this._isLoading;
            }
            private set
            {
                this._isLoading = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));
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
            if (this.IsLoading)
            {
                return;
            }

            this.IsLoading = true;

            this.ClearItems();

            try
            {
                WallpaperService service = new WallpaperService();
                var wallpapers = await service.GetWallpapersAsync(this._year, this._month, AppSetting.Area);// 加载指定年月，指定地区的壁纸信息。

                this.ClearItems();
                foreach (var wallpaper in wallpapers)
                {
                    this.Add(wallpaper);
                }
            }
            catch (Exception ex)
            {
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Cover)));

            this.IsLoading = false;
        }
    }
}