using Newtonsoft.Json;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonObject]
    public class CoverStory
    {
        [JsonProperty("date")]
        public string Date
        {
            get;
            set;
        }

        [JsonProperty("title")]
        public string Title
        {
            get;
            set;
        }

        [JsonProperty("attribute")]
        public string Attribute
        {
            get;
            set;
        }

        [JsonProperty("para1")]
        public string Parameter1
        {
            get;
            set;
        }

        [JsonProperty("para2")]
        public string Parameter2
        {
            get;
            set;
        }

        [JsonProperty("provider")]
        public string Provider
        {
            get;
            set;
        }

        [JsonProperty("imageUrl")]
        public string ImageUrl
        {
            get;
            set;
        }

        [JsonProperty("primaryImageUrl")]
        public string PrimaryImageUrl
        {
            get;
            set;
        }
    }
}