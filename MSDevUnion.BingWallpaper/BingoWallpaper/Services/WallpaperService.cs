using BingoWallpaper.Models;
using Newtonsoft.Json;
using SoftwareKobo.UniversalToolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace BingoWallpaper.Services
{
    public class WallpaperService
    {
        private const string URLBASE = @"https://leancloud.cn";

        public static string LeanCloudAppId
        {
            get;
            set;
        }

        public static string LeanCloudAppKey
        {
            get;
            set;
        }

        public async Task<LeanCloudResultCollection<Archive>> GetArchivesAsync(int year, int month, string market)
        {
            if (market == null)
            {
                throw new ArgumentNullException(nameof(market));
            }

            using (HttpClient client = CreateClient())
            {
                var where = new
                {
                    market = market,
                    date = new Dictionary<string, string>
                    {
                        {
                            "$regex",
                            @"\Q" + new DateTime(year,month,1).ToString("yyyyMM") + @"\E"
                        }
                    }
                };

                string requestUri = $"{URLBASE}/1.1/classes/Archive?where={WebUtility.UrlEncode(JsonConvert.SerializeObject(where))}&order=-date";

                return await client.GetJsonAsync<LeanCloudResultCollection<Archive>>(new Uri(requestUri));
            }
        }

        public async Task<Image> GetImageAsync(string objectId)
        {
            using (HttpClient client = CreateClient())
            {
                string requestUri = $"{URLBASE}/1.1/classes/Image/{objectId}";

                return await client.GetJsonAsync<Image>(new Uri(requestUri));
            }
        }

        public async Task<IEnumerable<Wallpaper>> GetWallpapersAsync(int year, int month, string market)
        {
            var collection = await GetArchivesAsync(year, month, market);
            List<Wallpaper> wallpapers = new List<Wallpaper>();
            foreach (var archive in collection)
            {
                var image = await GetImageAsync(archive.Image.ObjectId);
                wallpapers.Add(new Wallpaper()
                {
                    Archive = archive,
                    Image = image
                });
            }
            var first = wallpapers.FirstOrDefault();
            if (first != null)
            {
                first.IsLastInMonth = true;
            }
            return wallpapers;
        }

        private HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-AVOSCloud-Application-Id", LeanCloudAppId);
            client.DefaultRequestHeaders.Add("X-AVOSCloud-Application-Key", LeanCloudAppKey);
            return client;
        }
    }
}