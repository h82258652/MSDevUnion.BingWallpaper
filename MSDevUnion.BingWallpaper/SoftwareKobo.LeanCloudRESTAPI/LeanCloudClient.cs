using MSDevUnion.BingWallpaper.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SoftwareKobo.LeanCloudRESTAPI
{
    public class LeanCloudClient : IDisposable
    {
        private HttpClient _client;

        private const string URLBASE = @"https://leancloud.cn";

        public LeanCloudClient()
        {
            this._client = new HttpClient();
            this._client.DefaultRequestHeaders.Add("X-AVOSCloud-Application-Id", LeanCloudSettings.AppID);
            this._client.DefaultRequestHeaders.Add("X-AVOSCloud-Application-Key", LeanCloudSettings.AppKey);
        }

        public void Dispose()
        {
        }

        public async Task<T> GetAsync<T>(string objectId) where T : AVObject
        {
            string className = typeof(T).Name;
            string requestUri = $"{URLBASE}/1.1/classes/{className}/{objectId}";
            string json = await this._client.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<LeanCloudResultCollection<T>> GetAsync<T>() where T : AVObject
        {
            string className = typeof(T).Name;
            string requestUri = $"{URLBASE}/1.1/classes/{className}";

            string json = await this._client.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<LeanCloudResultCollection<T>>(json);
        }
    }
}