using Newtonsoft.Json;
using System.ComponentModel;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonObject]
    public class Archive : AVObject
    {
        [JsonProperty("date")]
        public string Date
        {
            get;
            set;
        }

        [JsonProperty("hs")]
        public Hotspot[] Hotspots
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

        [JsonProperty("cs")]
        public CoverStory CoverStory
        {
            get;
            set;
        }

        [JsonProperty("msg")]
        public Message[] Messages
        {
            get;
            set;
        }

        [JsonProperty("link")]
        public string Link
        {
            get;
            set;
        }

        [JsonProperty("info")]
        public string Info
        {
            get;
            set;
        }

        [JsonProperty("image")]
        public string ImageId
        {
            get;
            set;
        }

        [JsonProperty("market")]
        public string Market
        {
            get;
            set;
        }
    }
}