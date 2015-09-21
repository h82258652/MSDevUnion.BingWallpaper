using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
    public class AVAnalytics
    {
        private static readonly int AVANALYTIC_BATCH;

        private static readonly int AVANALYTIC_INTERVAL;

        private static readonly string AVCACHE_ANALYTICDATA_KEY;

        private static IList<AVAnalyticEvent> eventTics;

        private static IList<AVAnalyticActivity> activityTics;

        private static DateTime oppendTime;

        private static IDictionary<string, object> launch;

        private static string sessionId;

        private static string appId;

        private static int transStrategy;

        private static bool toggleTrack;

        private static string transStrategyUrl;

        static AVAnalytics()
        {
            AVAnalytics.AVANALYTIC_BATCH = 30;
            AVAnalytics.AVANALYTIC_INTERVAL = 20;
            AVAnalytics.AVCACHE_ANALYTICDATA_KEY = "AVAnalyticData_";
            AVAnalytics.transStrategyUrl = "/statistics/apps/{0}/sendPolicy";
            AVAnalytics.Initialize();
        }

        internal static void Initialize()
        {
            AVAnalytics.SetSeesion();
            AVAnalytics.appId = AVClient.ApplicationId;
            Task.Factory.StartNew<Task>(new Func<Task>(AVAnalytics.InitializeAVAnalytic));
        }

        internal static void SetSeesion()
        {
            AVAnalytics.sessionId = Guid.NewGuid().ToString();
            AVAnalytics.activityTics = new List<AVAnalyticActivity>();
            AVAnalytics.eventTics = new List<AVAnalyticEvent>();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("date", AVAnalytics.UnixTimestampFromDateTime(DateTime.Now));
            dictionary.Add("sessionId", AVAnalytics.sessionId);
            AVAnalytics.launch = dictionary;
            AVAnalytics.oppendTime = DateTime.Now;
        }

        internal static void ResetSession()
        {
            AVAnalytics.sessionId = Guid.NewGuid().ToString();
            AVAnalytics.activityTics = new List<AVAnalyticActivity>();
            AVAnalytics.eventTics = new List<AVAnalyticEvent>();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("date", AVAnalytics.UnixTimestampFromDateTime(DateTime.Now));
            dictionary.Add("sessionId", AVAnalytics.sessionId);
            AVAnalytics.launch = dictionary;
            AVAnalytics.oppendTime = DateTime.Now;
            if (AVAnalytics.transStrategy == 7)
            {
                AVAnalytics.SendCacheToServer();
            }
        }

        internal static void CloseSession()
        {
            Guid empty = Guid.Empty;
            AVAnalytics.sessionId = empty.ToString();
            AVAnalytics.activityTics = new List<AVAnalyticActivity>();
            AVAnalytics.eventTics = new List<AVAnalyticEvent>();
        }

        internal static async void SendCacheToServer()
        {
            IDictionary<string, object> cacheList =await AVPersistence.GetCacheList(AVAnalytics.AVCACHE_ANALYTICDATA_KEY);
            if (cacheList != null)
            {
                foreach (var c in cacheList)
                {
                    Func<object, Task> sad = SendAnalyticDataAsync;
                    Task.Factory.StartNew(sad, c.Value).ContinueWith(delegate {
                        AVPersistence.RemoveCacheListAt(AVCACHE_ANALYTICDATA_KEY, c.Key);
                    });
                }
            }
        }

        internal static Task InitializeAVAnalytic()
        {
            string currentSessionToken = AVUser.CurrentSessionToken;
            CancellationToken cancellationToken = CancellationToken.None;
            return AVClient.RequestAsync("GET", string.Format(AVAnalytics.transStrategyUrl, AVAnalytics.appId), currentSessionToken, null, cancellationToken).ContinueWith(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                Tuple<HttpStatusCode, IDictionary<string, object>> result = t.Result;
                if (result.Item1 == HttpStatusCode.OK)
                {
                    AVAnalytics.toggleTrack = bool.Parse(result.Item2[("enable")].ToString());
                    AVAnalytics.transStrategy = int.Parse(result.Item2["policy"].ToString());
                }
            }).ContinueWith(delegate (Task s)
            {
                if (AVAnalytics.toggleTrack)
                {
                    if (AVAnalytics.transStrategy == 6)
                    {
                        new Timer(delegate {
                            Task.Factory.StartNew(new Func<object, Task>(SendAnalyticDataAsync), null, cancellationToken);
                        }, null, -1, AVAnalytics.AVANALYTIC_INTERVAL * 1000);
                        return;
                    }
                    if (transStrategy == 7)
                    {
                        SendCacheToServer();
                    }
                }
            }, cancellationToken);
        }

        public static Task SendAnalyticDataAsync(object state)
        {
            IDictionary<string, object> data = null;
            if (state != null)
            {
                data = (state as IDictionary<string, object>);
            }
            else
            {
                data = AVAnalytics.GetCurrentAnalyticData();
                AVClient.SerializeJsonString(data);
            }
            string currentSessionToken = AVUser.CurrentSessionToken;
            CancellationToken cancellationToken = default(CancellationToken);
            return AVClient.RequestAsync("POST", "/stats/collect", currentSessionToken, data, cancellationToken).ContinueWith(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                Tuple<HttpStatusCode, IDictionary<string, object>> result = t.Result;
                if (result.Item1 == HttpStatusCode.OK)
                {
                    AVAnalytics.activityTics = new List<AVAnalyticActivity>();
                    AVAnalytics.eventTics = new List<AVAnalyticEvent>();
                    return;
                }
                AVPersistence.AppendCacheList(AVAnalytics.AVCACHE_ANALYTICDATA_KEY, data);
            });
        }

        private static IDictionary<string, object> GetCurrentAnalyticData()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            dictionary2.Add("sessionId", AVAnalytics.sessionId);
            IDictionary<string, object> deviceHook = AVClient.DeviceHook;
            dictionary2.Add("activities", AVAnalytics.activityTics.ToListDictionary<AVAnalyticActivity>());
            dictionary2.Add("duration", (DateTime.Now - AVAnalytics.oppendTime).TotalMilliseconds);
            Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
            dictionary3.Add("event", AVAnalytics.eventTics.ToListDictionary<AVAnalyticEvent>());
            dictionary3.Add("launch", AVAnalytics.launch);
            dictionary3.Add("terminate", dictionary2);
            Dictionary<string, object> dictionary4 = dictionary3;
            dictionary.Add("device", deviceHook);
            dictionary.Add("events", dictionary4);
            return dictionary;
        }

        internal static void SaveCurrentSession()
        {
            if (AVAnalytics.toggleTrack && AVAnalytics.transStrategy == 7)
            {
                IDictionary<string, object> currentAnalyticData = AVAnalytics.GetCurrentAnalyticData();
                AVPersistence.AppendCacheList(AVAnalytics.AVCACHE_ANALYTICDATA_KEY, currentAnalyticData);
            }
        }

        private void RealTimeCheck()
        {
            if (AVAnalytics.transStrategy == 1 && AVAnalytics.eventTics.Count + AVAnalytics.activityTics.Count >= AVAnalytics.AVANALYTIC_BATCH)
            {
                Task.Factory.StartNew<Task>(AVAnalytics.SendAnalyticDataAsync, null);
            }
        }

        public static void TrackAppOpened()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("date", AVAnalytics.UnixTimestampFromDateTime(DateTime.Now));
            dictionary.Add("sessionId", AVAnalytics.sessionId);
            AVAnalytics.launch = dictionary;
            AVAnalytics.TrackEvent("!AV!AppOpen");
        }

        public static void TrackEvent(string name)
        {
            AVAnalytics.TrackEvent(name, null);
        }

        public static void StartEvent(string name)
        {
            AVAnalytics.StartEvent(name, null);
        }

        public static void StartEvent(string name, IDictionary<string, object> dimensions)
        {
            if (Enumerable.Any<AVAnalyticEvent>(AVAnalytics.eventTics, (AVAnalyticEvent item) => item.name.Equals(name)))
            {
                AVAnalyticEvent aVAnalyticEvent = Enumerable.First<AVAnalyticEvent>(AVAnalytics.eventTics, (AVAnalyticEvent item) => item.name.Equals(name));
                if (!aVAnalyticEvent.stop)
                {
                    aVAnalyticEvent.ts = AVAnalytics.UnixTimestampFromDateTime(DateTime.Now);
                    return;
                }
            }
            AVAnalyticEvent aVAnalyticEvent2 = new AVAnalyticEvent
            {
                du = 0L,
                name = name,
                sessionId = AVAnalytics.sessionId,
                tag = name,
                ts = AVAnalytics.UnixTimestampFromDateTime(DateTime.Now),
                attributes = dimensions
            };
            AVAnalytics.eventTics.Add(aVAnalyticEvent2);
        }

        public static void StopEvent(string name)
        {
            AVAnalytics.StopEvent(name, null);
        }

        public static void StopEvent(string name, IDictionary<string, object> dimensions)
        {
            if (Enumerable.Any<AVAnalyticEvent>(AVAnalytics.eventTics, (AVAnalyticEvent item) => item.name.Equals(name)))
            {
                AVAnalyticEvent aVAnalyticEvent = Enumerable.First<AVAnalyticEvent>(AVAnalytics.eventTics, (AVAnalyticEvent item) => item.name.Equals(name));
                aVAnalyticEvent.stop = true;
                aVAnalyticEvent.du = AVAnalytics.UnixTimestampFromDateTime(DateTime.Now) - aVAnalyticEvent.ts;
            }
        }

        public static void TrackEvent(string name, IDictionary<string, object> dimensions)
        {
            if (name == null || name.Trim().Length == 0)
            {
                throw new ArgumentException("A name for the custom event must be provided.");
            }
            AVAnalyticEvent aVAnalyticEvent = new AVAnalyticEvent
            {
                du = 0L,
                name = name,
                sessionId = AVAnalytics.sessionId,
                tag = name,
                ts = AVAnalytics.UnixTimestampFromDateTime(DateTime.Now),
                attributes = dimensions
            };
            AVAnalytics.eventTics.Add(aVAnalyticEvent);
        }

        private static long UnixTimestampFromDateTime(DateTime date)
        {
            long num = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            return num / 10000L;
        }

        public static void OnSceneStart(string pageName)
        {
            if (pageName == null || pageName.Trim().Length == 0)
            {
                throw new ArgumentException("A name for the custom event must be provided.");
            }
            AVAnalyticActivity aVAnalyticActivity = new AVAnalyticActivity
            {
                du = 0L,
                name = pageName,
                ts = AVAnalytics.UnixTimestampFromDateTime(DateTime.Now)
            };
            AVAnalytics.activityTics.Add(aVAnalyticActivity);
        }

        public static void OnSceneEnd(string pageName)
        {
            if (pageName == null || pageName.Trim().Length == 0)
            {
                throw new ArgumentException("A name for the custom event must be provided.");
            }
            if (Enumerable.Any<AVAnalyticActivity>(AVAnalytics.activityTics, (AVAnalyticActivity item) => item.name.Equals(pageName)))
            {
                AVAnalyticActivity aVAnalyticActivity = Enumerable.First<AVAnalyticActivity>(AVAnalytics.activityTics, (AVAnalyticActivity item) => item.name.Equals(pageName));
                aVAnalyticActivity.du = AVAnalytics.UnixTimestampFromDateTime(DateTime.Now) - aVAnalyticActivity.ts;
            }
        }

        public static void OnAppSetBackgroud()
        {
            AVAnalytics.SaveCurrentSession();
            AVAnalytics.CloseSession();
        }

        public static void OnExit()
        {
            AVAnalytics.SaveCurrentSession();
            AVAnalytics.CloseSession();
        }
    }
}