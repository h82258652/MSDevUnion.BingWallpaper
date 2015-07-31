using Newtonsoft.Json;
using System.ComponentModel;

namespace MSDevUnion.BingWallpaper.Models
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

        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ACL
        {
            get;
            set;
        }

        [JsonProperty("vid")]
        public Video Video
        {
            get;
            set;
        }
    }
}