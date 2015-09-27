using System;

namespace BingoWallpaper.Services
{
    public struct JsonResultCache
    {
        public string Json
        {
            get;
            set;
        }

        public DateTime RequestTime
        {
            get;
            set;
        }

        public bool IsUseable()
        {
            return RequestTime.AddHours(1) >= DateTime.Now;
        }
    }
}