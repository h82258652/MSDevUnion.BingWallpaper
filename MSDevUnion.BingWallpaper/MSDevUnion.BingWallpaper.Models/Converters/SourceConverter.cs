using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace MSDevUnion.BingWallpaper.Models.Converters
{
    public class SourceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Source);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Debugger.Break();

            Source source = new Source();
            source.Format = reader.ReadAsString();
            source.Header = reader.ReadAsString();
            source.Link = reader.ReadAsString();
            return source;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}