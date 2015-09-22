using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BingoWallpaper.Models
{
    [JsonObject]
    public class LeanCloudResultCollection<T> : LeanCloudResultBase, IEnumerable<T> where T : AVObject
    {
        [JsonProperty("results")]
        public T[] Results
        {
            get;
            set;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Results.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Results.GetEnumerator();
        }
    }
}