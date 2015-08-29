using Newtonsoft.Json;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonObject]
    public class LeanCloudPointer
    {
        [JsonProperty("__type")]
        public string Type
        {
            get;
            set;
        }

        [JsonProperty("className")]
        public string ClassName
        {
            get;
            set;
        }

        [JsonProperty("objectId")]
        public string ObjectId
        {
            get;
            set;
        }
    }
}