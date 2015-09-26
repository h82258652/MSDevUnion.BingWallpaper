using System;
using System.Linq;

namespace BingoWallpaper.Models
{
    public class Wallpaper
    {
        public Archive Archive
        {
            get;
            set;
        }

        public Image Image
        {
            get;
            set;
        }

        /// <summary>
        /// 是否当前月最后一副。
        /// </summary>
        public bool IsLastInMonth
        {
            get;
            set;
        }

        public string GetCacheUrl(WallpaperSize size)
        {
            if(WallpaperSize.SupportWallpaperSizes.Contains(size)== false)
            {
                throw new ArgumentException("7niu cache not support.",nameof(size));
            }

            return "http://7u2lw5.com5.z0.glb.clouddn.com" + Image.UrlBase + "_" + size.ToString() + ".jpg";
        }

        public string GetOriginalUrl(WallpaperSize size)
        {
            return "http://www.bing.com" + Image.UrlBase + "_" + size.ToString() + ".jpg";
        }

        //public string GetUrl(WallpaperSize size)
        //{
        //    return "http://7u2lw5.com5.z0.glb.clouddn.com" + Image.UrlBase + "_" + size.ToString() + ".jpg";
        //    //return "http://www.bing.com" + Image.UrlBase + "_" + size.ToString() + ".jpg";
        //}
    }
}