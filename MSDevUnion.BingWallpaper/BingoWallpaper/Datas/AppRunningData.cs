using BingoWallpaper.Models;
using System;
using System.Collections.Generic;

namespace BingoWallpaper.Datas
{
    public static class AppRunningData
    {
        /// <summary>
        /// 提供的壁纸信息最小日期。
        /// </summary>
        private static readonly DateTimeOffset MIN_VIEW_MONTH = new DateTimeOffset(new DateTime(2015, 1, 1));

        private static IList<WallpaperCollection> _allWallpapers;

        /// <summary>
        /// 所有壁纸信息。
        /// </summary>
        public static IList<WallpaperCollection> AllWallpapers
        {
            get
            {
                return _allWallpapers;
            }
        }

        static AppRunningData()
        {
            _allWallpapers = new List<WallpaperCollection>();
            DateTime date = MIN_VIEW_MONTH.DateTime;
            while (date <= DateTime.Now)
            {
                _allWallpapers.Add(new WallpaperCollection(date.Year, date.Month));
                date = date.AddMonths(1);
            }
        }

        /// <summary>
        /// 重新加载所有壁纸信息。
        /// </summary>
        public static async void ReloadAll()
        {
            foreach (var collection in AllWallpapers)
            {
                await collection.ReLoad();
            }
        }
    }
}