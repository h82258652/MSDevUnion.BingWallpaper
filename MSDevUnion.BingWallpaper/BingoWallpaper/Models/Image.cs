using Newtonsoft.Json;

namespace BingoWallpaper.Models
{
    [JsonObject]
    public class Image : AVObject
    {
        [JsonProperty("urlbase")]
        public string UrlBase
        {
            get;
            set;
        }

        /// <summary>
        /// 拥有 1920x1200 分辨率。
        /// </summary>
        [JsonProperty("wp")]
        public bool ExistWUXGA
        {
            get;
            set;
        }

        [JsonProperty("copyright")]
        public string Copyright
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }
    }
}