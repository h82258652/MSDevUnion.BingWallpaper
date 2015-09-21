using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVOSCloud.Internal
{
	internal class AVPersistence
	{
		private const string CACHE_LIST_INDEX_SUFFIX = "_index";

		public AVPersistence()
		{
		}

		public static async void AppendCacheList(string key, IDictionary<string, object> value)
		{
			try
			{
				Guid guid = Guid.NewGuid();
				string str = string.Concat(key, "_index");
				IList<string> cacheListNameIndex =await AVPersistence.GetCacheListNameIndex(key);
				if (cacheListNameIndex == null)
				{
					cacheListNameIndex = new List<string>();
					cacheListNameIndex.Add(guid.ToString());
				}
				else
				{
					cacheListNameIndex.Add(guid.ToString());
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("keys", cacheListNameIndex);
				await AVClient.platformHooks.SetCache(str, dictionary);
				await AVClient.platformHooks.SetCache(string.Concat(key, guid), value);
			}
			catch
			{
				throw;
			}
		}

		public static async Task<IDictionary<string, object>>  GetCacheList(string key)
		{
            IDictionary<string, object> dictionary = null;
            IList<string> cacheListNameIndex =await AVPersistence.GetCacheListNameIndex(key);
            if (cacheListNameIndex != null)
            {
                dictionary = new Dictionary<string, object>();
                foreach (string key1 in cacheListNameIndex)
                {
                    IDictionary<string, object> cache =await AVClient.platformHooks.GetCache(key + key1);
                    dictionary.Add(key1, cache);
                }
            }
            return dictionary;
        }

		private static async Task<IList<string>> GetCacheListNameIndex(string key)
		{
			IList<string> list = null;
			string str = string.Concat(key, "_index");
			IDictionary<string, object> cache =await AVClient.platformHooks.GetCache(str);
			if (cache != null)
			{
				list = new List<string>();
				foreach (object item in cache["keys"] as IList<object>)
				{
					list.Add(item.ToString());
				}
			}
			return list;
		}

		public static long GetCurrentUnixTimestampFromDateTime()
		{
			long ticks = DateTime.Now.Ticks;
			DateTime dateTime = new DateTime(1970, 1, 1);
			return (ticks - dateTime.Ticks) / 10000;
		}

		public static async void RemoveCacheList(string key)
		{
			IList<string> cacheListNameIndex =await AVPersistence.GetCacheListNameIndex(key);
			AVClient.platformHooks.RemoveCache(string.Concat(key, "_index"));
			foreach (string str in cacheListNameIndex)
			{
				AVClient.platformHooks.RemoveCache(string.Concat(key, str));
			}
		}

		public static async void RemoveCacheListAt(string key, string name)
		{
			bool flag = AVClient.platformHooks.RemoveCache(string.Concat(key, name));
			string str = string.Concat(key, "_index");
			if (flag)
			{
				IList<string> cacheListNameIndex =await AVPersistence.GetCacheListNameIndex(key);
				cacheListNameIndex.Remove(name);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("keys", cacheListNameIndex);
				await AVClient.platformHooks.SetCache(str, dictionary);
			}
		}

		public static Task<bool> SaveCache(string key, IDictionary<string, object> value)
		{
			return AVClient.platformHooks.SetCache(key, value);
		}

		public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(unixTimeStamp * 10000L).ToLocalTime();
        }
    }
}