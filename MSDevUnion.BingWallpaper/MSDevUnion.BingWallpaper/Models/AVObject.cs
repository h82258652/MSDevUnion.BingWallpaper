using Newtonsoft.Json;
using System;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonObject]
    public abstract class AVObject : LeanCloudResultBase
    {
        [JsonProperty("objectId")]
        public string ObjectId
        {
            get;
            set;
        }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt
        {
            get;
            set;
        }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt
        {
            get;
            set;
        }
    }
}