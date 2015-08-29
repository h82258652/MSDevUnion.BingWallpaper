using Newtonsoft.Json;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonObject]
    public class Image : LeanCloudResultBase
    {
        [JsonProperty("urlbase")]
        public string UrlBase
        {
            get;
            set;
        }

        /// <summary>
        /// 使用拥有 1920x1200
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