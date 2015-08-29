using MSDevUnion.BingWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Extensions;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MSDevUnion.BingWallpaper.Services
{
    public class BingWallpaperService : IBingWallpaperService
    {
        private const string URLBASE = @"https://leancloud.cn";

        public async Task<LeanCloudResultCollection<Archive>> GetArchivesAsync(int year, int month, string market)
        {
            using (HttpClient client = CreateClient())
            {
                string requestUri = $"{URLBASE}/1.1/classes/Archive?where=";

                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("{\"market\":\"");
                queryBuilder.Append(market);
                queryBuilder.Append("\",\"date\":{\"$in\":[");
                int daysInMonth = DateTime.DaysInMonth(year, month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    queryBuilder.Append("\"");
                    queryBuilder.Append(year);
                    queryBuilder.Append(month.ToString("00"));
                    queryBuilder.Append(day.ToString("00"));
                    queryBuilder.Append("\"");
                    if (day != daysInMonth)
                    {
                        queryBuilder.Append(",");
                    }
                }
                queryBuilder.Append("]}}");
                string query = queryBuilder.ToString();

                query = WebUtility.UrlEncode(query);
                requestUri = requestUri + query + "&order=date";

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

        private HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-AVOSCloud-Application-Id", "2odv0fmdni1w22hceawylo48l76vxbltgpl1mnoq3hlxj55j");
            client.DefaultRequestHeaders.Add("X-AVOSCloud-Application-Key", "idsoc6l9k218zrge2qi06anel3qcoqgvhutbqm93e4l58d3i");
            return client;
        }
    }
}