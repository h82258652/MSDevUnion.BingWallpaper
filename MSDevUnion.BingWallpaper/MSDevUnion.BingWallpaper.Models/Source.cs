using MSDevUnion.BingWallpaper.Models.Converters;
using Newtonsoft.Json;

namespace MSDevUnion.BingWallpaper.Models
{
    [JsonArray(ItemConverterType = typeof(SourceConverter))]
    public class Source
    {
        public string Format
        {
            get;
            set;
        }

        public string Header
        {
            get;
            set;
        }

        public string Link
        {
            get;
            set;
        }
    }
}