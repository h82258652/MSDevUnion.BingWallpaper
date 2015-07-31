using Newtonsoft.Json;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonObject]
    public class Video
    {
        [JsonProperty("sources")]
        public Source[] Sources
        {
            get;
            set;
        }

        [JsonProperty("loop")]
        public bool Loop
        {
            get;
            set;
        }

        [JsonProperty("image")]
        public string ImageUrl
        {
            get;
            set;
        }

        [JsonProperty("caption")]
        public string Caption
        {
            get;
            set;
        }

        [JsonProperty("captionlink")]
        public string CationLink
        {
            get;
            set;
        }

        [JsonProperty("dark")]
        public bool Dark
        {
            get;
            set;
        }
    }
}