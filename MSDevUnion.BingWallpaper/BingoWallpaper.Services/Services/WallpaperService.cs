﻿using BingoWallpaper.Models;
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

        private const string LeanCloudAppId = @"2odv0fmdni1w22hceawylo48l76vxbltgpl1mnoq3hlxj55j";

        private const string LeanCloudAppKey = @"idsoc6l9k218zrge2qi06anel3qcoqgvhutbqm93e4l58d3i";

        private static Dictionary<string, JsonResultCache> DataCache
        {
            get
            {
                return _dataCache;
            }
        }

        private static Dictionary<string, JsonResultCache> _dataCache = new Dictionary<string, JsonResultCache>();

        public async Task<LeanCloudResultCollection<Archive>> GetArchivesAsync(int year, int month, string market)
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

            string json;

            if (DataCache.ContainsKey(requestUri) && DataCache[requestUri].IsUseable())
            {
                json = DataCache[requestUri].Json;
            }
            else
            {
                using (HttpClient client = CreateClient())
                {
                    var requestTime = DateTime.Now;

                    json = await client.GetStringAsync(new Uri(requestUri));
                    DataCache[requestUri] = new JsonResultCache()
                    {
                        Json = json,
                        RequestTime = requestTime
                    };
                }
            }

            return JsonConvert.DeserializeObject<LeanCloudResultCollection<Archive>>(json);
        }

        public async Task<Image> GetImageAsync(string objectId)
        {
            string requestUri = $"{URLBASE}/1.1/classes/Image/{objectId}";

            string json;

            if (DataCache.ContainsKey(requestUri) && DataCache[requestUri].IsUseable())
            {
                json = DataCache[requestUri].Json;
            }
            else
            {
                using (HttpClient client = CreateClient())
                {
                    var requestTime = DateTime.Now;

                    json = await client.GetStringAsync(new Uri(requestUri));
                    DataCache[requestUri] = new JsonResultCache()
                    {
                        Json = json,
                        RequestTime = requestTime
                    };
                }
            }

            return JsonConvert.DeserializeObject<Image>(json);
        }

        public async Task<Archive> GetNewestArchiveAsync(string market)
        {
            using (HttpClient client = CreateClient())
            {
                var where = new
                {
                    market = market
                };

                string requestUri = $"{URLBASE}/1.1/classes/Archive?where={WebUtility.UrlEncode(JsonConvert.SerializeObject(where))}&order=-date&limit=1";

                return (await client.GetJsonAsync<LeanCloudResultCollection<Archive>>(new Uri(requestUri))).FirstOrDefault();
            }
        }

        public async Task<Wallpaper> GetNewestWallpaperAsync(string market)
        {
            var archive = await GetNewestArchiveAsync(market);
            if (archive == null)
            {
                return null;
            }
            var image = await GetImageAsync(archive.Image.ObjectId);
            return new Wallpaper()
            {
                Archive = archive,
                Image = image
            };
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