using AVOSCloud.Internal;
//using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace AVOSCloud
{
    [AVClassName("_Installation")]
    public class AVInstallation : AVObject
    {
        private static bool currentInstallationMatchesDisk = false;
        private static AVInstallation currentInstallation = (AVInstallation) null;
        private static readonly object currentInstallationMutex = new object();

        private static readonly HashSet<string> protectedFields = new HashSet<string>()
        {
            "deviceType",
            "subscriptionUri",
            "installationId",
            "valid",
            "timeZone",
            "avoscloudVersion",
            "appName",
            "appIdentifier",
            "appVersion"
        };

        private static readonly Dictionary<string, string> tzNameMap = new Dictionary<string, string>()
        {
            {
                "Dateline Standard Time",
                "Etc/GMT+12"
            },
            {
                "UTC-11",
                "Etc/GMT+11"
            },
            {
                "Hawaiian Standard Time",
                "Pacific/Honolulu"
            },
            {
                "Alaskan Standard Time",
                "America/Anchorage"
            },
            {
                "Pacific Standard Time (Mexico)",
                "America/Santa_Isabel"
            },
            {
                "Pacific Standard Time",
                "America/Los_Angeles"
            },
            {
                "US Mountain Standard Time",
                "America/Phoenix"
            },
            {
                "Mountain Standard Time (Mexico)",
                "America/Chihuahua"
            },
            {
                "Mountain Standard Time",
                "America/Denver"
            },
            {
                "Central America Standard Time",
                "America/Guatemala"
            },
            {
                "Central Standard Time",
                "America/Chicago"
            },
            {
                "Central Standard Time (Mexico)",
                "America/Mexico_City"
            },
            {
                "Canada Central Standard Time",
                "America/Regina"
            },
            {
                "SA Pacific Standard Time",
                "America/Bogota"
            },
            {
                "Eastern Standard Time",
                "America/New_York"
            },
            {
                "US Eastern Standard Time",
                "America/Indianapolis"
            },
            {
                "Venezuela Standard Time",
                "America/Caracas"
            },
            {
                "Paraguay Standard Time",
                "America/Asuncion"
            },
            {
                "Atlantic Standard Time",
                "America/Halifax"
            },
            {
                "Central Brazilian Standard Time",
                "America/Cuiaba"
            },
            {
                "SA Western Standard Time",
                "America/La_Paz"
            },
            {
                "Pacific SA Standard Time",
                "America/Santiago"
            },
            {
                "Newfoundland Standard Time",
                "America/St_Johns"
            },
            {
                "E. South America Standard Time",
                "America/Sao_Paulo"
            },
            {
                "Argentina Standard Time",
                "America/Buenos_Aires"
            },
            {
                "SA Eastern Standard Time",
                "America/Cayenne"
            },
            {
                "Greenland Standard Time",
                "America/Godthab"
            },
            {
                "Montevideo Standard Time",
                "America/Montevideo"
            },
            {
                "Bahia Standard Time",
                "America/Bahia"
            },
            {
                "UTC-02",
                "Etc/GMT+2"
            },
            {
                "Azores Standard Time",
                "Atlantic/Azores"
            },
            {
                "Cape Verde Standard Time",
                "Atlantic/Cape_Verde"
            },
            {
                "Morocco Standard Time",
                "Africa/Casablanca"
            },
            {
                "UTC",
                "Etc/GMT"
            },
            {
                "GMT Standard Time",
                "Europe/London"
            },
            {
                "Greenwich Standard Time",
                "Atlantic/Reykjavik"
            },
            {
                "W. Europe Standard Time",
                "Europe/Berlin"
            },
            {
                "Central Europe Standard Time",
                "Europe/Budapest"
            },
            {
                "Romance Standard Time",
                "Europe/Paris"
            },
            {
                "Central European Standard Time",
                "Europe/Warsaw"
            },
            {
                "W. Central Africa Standard Time",
                "Africa/Lagos"
            },
            {
                "Namibia Standard Time",
                "Africa/Windhoek"
            },
            {
                "GTB Standard Time",
                "Europe/Bucharest"
            },
            {
                "Middle East Standard Time",
                "Asia/Beirut"
            },
            {
                "Egypt Standard Time",
                "Africa/Cairo"
            },
            {
                "Syria Standard Time",
                "Asia/Damascus"
            },
            {
                "E. Europe Standard Time",
                "Asia/Nicosia"
            },
            {
                "South Africa Standard Time",
                "Africa/Johannesburg"
            },
            {
                "FLE Standard Time",
                "Europe/Kiev"
            },
            {
                "Turkey Standard Time",
                "Europe/Istanbul"
            },
            {
                "Israel Standard Time",
                "Asia/Jerusalem"
            },
            {
                "Jordan Standard Time",
                "Asia/Amman"
            },
            {
                "Arabic Standard Time",
                "Asia/Baghdad"
            },
            {
                "Kaliningrad Standard Time",
                "Europe/Kaliningrad"
            },
            {
                "Arab Standard Time",
                "Asia/Riyadh"
            },
            {
                "E. Africa Standard Time",
                "Africa/Nairobi"
            },
            {
                "Iran Standard Time",
                "Asia/Tehran"
            },
            {
                "Arabian Standard Time",
                "Asia/Dubai"
            },
            {
                "Azerbaijan Standard Time",
                "Asia/Baku"
            },
            {
                "Russian Standard Time",
                "Europe/Moscow"
            },
            {
                "Mauritius Standard Time",
                "Indian/Mauritius"
            },
            {
                "Georgian Standard Time",
                "Asia/Tbilisi"
            },
            {
                "Caucasus Standard Time",
                "Asia/Yerevan"
            },
            {
                "Afghanistan Standard Time",
                "Asia/Kabul"
            },
            {
                "Pakistan Standard Time",
                "Asia/Karachi"
            },
            {
                "West Asia Standard Time",
                "Asia/Tashkent"
            },
            {
                "India Standard Time",
                "Asia/Calcutta"
            },
            {
                "Sri Lanka Standard Time",
                "Asia/Colombo"
            },
            {
                "Nepal Standard Time",
                "Asia/Katmandu"
            },
            {
                "Central Asia Standard Time",
                "Asia/Almaty"
            },
            {
                "Bangladesh Standard Time",
                "Asia/Dhaka"
            },
            {
                "Ekaterinburg Standard Time",
                "Asia/Yekaterinburg"
            },
            {
                "Myanmar Standard Time",
                "Asia/Rangoon"
            },
            {
                "SE Asia Standard Time",
                "Asia/Bangkok"
            },
            {
                "N. Central Asia Standard Time",
                "Asia/Novosibirsk"
            },
            {
                "China Standard Time",
                "Asia/Shanghai"
            },
            {
                "North Asia Standard Time",
                "Asia/Krasnoyarsk"
            },
            {
                "Singapore Standard Time",
                "Asia/Singapore"
            },
            {
                "W. Australia Standard Time",
                "Australia/Perth"
            },
            {
                "Taipei Standard Time",
                "Asia/Taipei"
            },
            {
                "Ulaanbaatar Standard Time",
                "Asia/Ulaanbaatar"
            },
            {
                "North Asia East Standard Time",
                "Asia/Irkutsk"
            },
            {
                "Tokyo Standard Time",
                "Asia/Tokyo"
            },
            {
                "Korea Standard Time",
                "Asia/Seoul"
            },
            {
                "Cen. Australia Standard Time",
                "Australia/Adelaide"
            },
            {
                "AUS Central Standard Time",
                "Australia/Darwin"
            },
            {
                "E. Australia Standard Time",
                "Australia/Brisbane"
            },
            {
                "AUS Eastern Standard Time",
                "Australia/Sydney"
            },
            {
                "West Pacific Standard Time",
                "Pacific/Port_Moresby"
            },
            {
                "Tasmania Standard Time",
                "Australia/Hobart"
            },
            {
                "Yakutsk Standard Time",
                "Asia/Yakutsk"
            },
            {
                "Central Pacific Standard Time",
                "Pacific/Guadalcanal"
            },
            {
                "Vladivostok Standard Time",
                "Asia/Vladivostok"
            },
            {
                "New Zealand Standard Time",
                "Pacific/Auckland"
            },
            {
                "UTC+12",
                "Etc/GMT-12"
            },
            {
                "Fiji Standard Time",
                "Pacific/Fiji"
            },
            {
                "Magadan Standard Time",
                "Asia/Magadan"
            },
            {
                "Tonga Standard Time",
                "Pacific/Tongatapu"
            },
            {
                "Samoa Standard Time",
                "Pacific/Apia"
            }
        };

        //private static Lazy<Task<HttpNotificationChannel>> getToastChannelTask =
        //    new Lazy<Task<HttpNotificationChannel>>(
        //        (Func<Task<HttpNotificationChannel>>)
        //            (() => Task.Run<HttpNotificationChannel>((Func<HttpNotificationChannel>) (() =>
        //            {
        //                try
        //                {
        //                    HttpNotificationChannel notificationChannel =
        //                        HttpNotificationChannel.Find(AVInstallation.toastChannelTag);
        //                    if (notificationChannel == null)
        //                    {
        //                        notificationChannel = new HttpNotificationChannel(AVInstallation.toastChannelTag);
        //                        notificationChannel.Open();
        //                    }
        //                    if (!notificationChannel.get_IsShellToastBound())
        //                        notificationChannel.BindToShellToast();
        //                    return notificationChannel;
        //                }
        //                catch (UnauthorizedAccessException ex)
        //                {
        //                    return (HttpNotificationChannel) null;
        //                }
        //            }))));

        //private static Lazy<Task<string>> getToastUriTask = new Lazy<Task<string>>((Func<Task<string>>) (async () =>
        //{
        //    HttpNotificationChannel channel = await AVInstallation.getToastChannelTask.Value;
        //    string rtn = string.Empty;
        //    string str;
        //    if (channel == null || channel.get_ChannelUri() == (Uri) null)
        //    {
        //        rtn = (string) null;
        //    }
        //    else
        //    {
        //        TaskCompletionSource<string> source = new TaskCompletionSource<string>();
        //        EventHandler<NotificationChannelUriEventArgs> handler =
        //            (EventHandler<NotificationChannelUriEventArgs>) null;
        //        handler = (EventHandler<NotificationChannelUriEventArgs>) ((sender, args) =>
        //        {
        //            source.SetResult(args.get_ChannelUri().AbsoluteUri);
        //            channel.remove_ChannelUriUpdated(handler);
        //        });
        //        channel.add_ChannelUriUpdated(handler);
        //        try
        //        {
        //            rtn = channel.get_ChannelUri().AbsoluteUri;
        //            str = rtn;
        //            goto label_8;
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //        }
        //        rtn = await source.Task;
        //    }
        //    str = rtn;
        //    label_8:
        //    return str;
        //}));

        private static readonly string toastChannelTag = "_Toast";
        private string appName;
        private string productId;
        private string version;

        public static AVInstallation CurrentInstallation
        {
            get
            {
                lock (AVInstallation.currentInstallationMutex)
                {
                    if (AVInstallation.currentInstallation != null || AVInstallation.currentInstallationMatchesDisk)
                        return AVInstallation.currentInstallation;
                    object local_0;
                    AVClient.ApplicationSettings.TryGetValue("CurrentInstallation", out local_0);
                    string local_1 = local_0 as string;
                    if (local_1 != null)
                    {
                        IDictionary<string, object> local_2 = AVClient.DeserializeJsonString(local_1);
                        AVInstallation local_3 = AVObject.CreateWithoutData<AVInstallation>((string) null);
                        local_3.MergeAfterFetch(local_2);
                        AVInstallation.currentInstallationMatchesDisk = true;
                        AVInstallation.currentInstallation = local_3;
                    }
                    else
                        AVInstallation.currentInstallation = AVObject.Create<AVInstallation>();
                    return AVInstallation.currentInstallation;
                }
            }
        }

        public static AVQuery<AVInstallation> Query
        {
            get { return new AVQuery<AVInstallation>(); }
        }

        public Guid InstallationId
        {
            get
            {
                Guid? installationId = AVClient.InstallationId;
                if (!installationId.HasValue)
                    throw new InvalidOperationException(
                        "Cannot call AVInstallation.InstallationId before AVClient.Initialize() has been called");
                else
                    return installationId.Value;
            }
        }

        [AVFieldName("timeZone")]
        public string TimeZone
        {
            get
            {
                
                //CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                string standardName = TimeZoneInfo.Local.StandardName;
                //Thread.CurrentThread.CurrentCulture = currentCulture;
                if (AVInstallation.tzNameMap.ContainsKey(standardName))
                    return AVInstallation.tzNameMap[standardName];
                else
                    return (string) null;
            }
        }

        [AVFieldName("avoscloudVersion")]
        public Version AVVersion
        {
            get { return AVClient.Version; }
        }

        [AVFieldName("channels")]
        public IList<string> Channels
        {
            get { return this.GetProperty<IList<string>>("Channels"); }
            set { this.SetProperty<IList<string>>(value, "Channels"); }
        }

        public override object this[string key]
        {
            set
            {
                if (AVInstallation.protectedFields.Contains(key))
                    throw new InvalidOperationException("Cannot change the automatic field " + key);
                base[key] = value;
            }
        }

        //internal static Task<HttpNotificationChannel> GetToastChannelTask
        //{
        //    get { return AVInstallation.getToastChannelTask.Value; }
        //}

        [AVFieldName("deviceType")]
        public string DeviceType
        {
            get { return "wp"; }
        }

        [AVFieldName("appName")]
        public string AppName
        {
            get
            {
                if (this.appName == null)
                    this.appName = Package.Current.Id.FullName;//AVInstallation.GetAppAttribute("Title");
                return this.appName;
            }
        }

        [AVFieldName("appIdentifier")]
        public string AppIdentifier
        {
            get
            {
                if (this.productId == null)
                    this.productId = Package.Current.Id.ProductId;//AVInstallation.GetAppAttribute("ProductID");
                return this.productId;
            }
        }

        [AVFieldName("appVersion")]
        public string AppVersion
        {
            get
            {
                if (this.version == null)
                    this.version = Application.Current.GetType().GetTypeInfo().Assembly.GetName().Version.ToString(4);//Package.Current.Id.Version.ToString(4);//AVInstallation.GetAppAttribute("Version");
                return this.version;
            }
        }

        static AVInstallation()
        {
            //Task<HttpNotificationChannel> toastChannelTask = AVInstallation.GetToastChannelTask;
        }

        internal static void SaveCurrentInstallation()
        {
            lock (AVInstallation.currentInstallationMutex)
            {
                if (AVInstallation.currentInstallation == null)
                {
                    AVClient.ApplicationSettings.Remove("CurrentInstallation");
                }
                else
                {
                    IDictionary<string, object> local_0 =
                        AVInstallation.currentInstallation.ServerDataToJSONObjectForSerialization();
                    local_0["objectId"] = (object) AVInstallation.currentInstallation.ObjectId;
                    local_0["createdAt"] =
                        (object) AVInstallation.currentInstallation.CreatedAt.Value.ToString(AVClient.dateFormatString);
                    local_0["updatedAt"] =
                        (object) AVInstallation.currentInstallation.UpdatedAt.Value.ToString(AVClient.dateFormatString);
                    AVClient.ApplicationSettings["CurrentInstallation"] = (object) AVClient.SerializeJsonString(local_0);
                    AVInstallation.currentInstallationMatchesDisk = true;
                }
            }
        }

        internal static void ClearInMemoryInstallation()
        {
            lock (AVInstallation.currentInstallationMutex)
            {
                AVInstallation.currentInstallation = (AVInstallation) null;
                AVInstallation.currentInstallationMatchesDisk = false;
            }
        }

        private void SetIfDifferent<T>(string key, T value)
        {
            T result;
            bool flag = this.TryGetValue<T>(key, out result);
            if ((object) value == null)
            {
                if (!flag)
                    return;
                this.Remove(key);
            }
            else
            {
                if (flag && value.Equals((object) result))
                    return;
                base[key] = (object) value;
            }
        }

        private void SetAutomaticValues()
        {
            this.SetIfDifferent<string>("deviceType", this.DeviceType);
            this.SetIfDifferent<string>("installationId", this.InstallationId.ToString());
            this.SetIfDifferent<string>("timeZone", this.TimeZone);
            this.SetIfDifferent<string>("avoscloudVersion", ((object) this.AVVersion).ToString());
            this.SetIfDifferent<string>("appName", this.AppName);
            this.SetIfDifferent<string>("appVersion", this.AppVersion);
        }

        internal override async Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
        {
            this.SetAutomaticValues();
            //await
            //    InternalExtensions.PartialAsync((object) this,
            //        (InternalExtensions.PartialAccessor<Task>) ((ref Task t) => this.UpdatePushUri(ref t)));
            await base.SaveAsync(toAwait, cancellationToken);
            AVInstallation.SaveCurrentInstallation();
        }

        //private void UpdatePushUri(ref Task partialTask)
        //{
        //    partialTask = AVInstallation.getToastUriTask.Value.ContinueWith((Action<Task<string>>) (task =>
        //    {
        //        AVInstallation avInstallation = this;
        //        string key = "subscriptionUri";
        //        Dictionary<string, string> dictionary;
        //        if (task.Result != null)
        //            dictionary = new Dictionary<string, string>()
        //            {
        //                {
        //                    AVInstallation.toastChannelTag,
        //                    task.Result
        //                }
        //            };
        //        else
        //            dictionary = (Dictionary<string, string>) null;
        //        avInstallation.SetIfDifferent<string>(key, dictionary[AVInstallation.toastChannelTag]);
        //    }));
        //}

        //internal static string GetAppAttribute(string attributeName)
        //{

        //    string inputUri = "WMAppManifest.xml";
        //    string name = "App";
        //    XmlReader xmlReader = XmlReader.Create(inputUri, new XmlReaderSettings()
        //    {
        //        //XmlResolver = (XmlResolver) new XmlXapResolver()
        //    });
        //    try
        //    {
        //        xmlReader.ReadToDescendant(name);
        //        if (!xmlReader.IsStartElement())
        //            throw new FormatException(inputUri + " is missing " + name);
        //        else
        //            return xmlReader.GetAttribute(attributeName);
        //    }
        //    finally
        //    {
        //        if (xmlReader != null)
        //            xmlReader.Dispose();
        //    }
        //}
    }
}
