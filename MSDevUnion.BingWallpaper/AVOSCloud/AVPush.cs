using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
    public class AVPush
    {
        private DateTime? expiration = new DateTime?();
        private TimeSpan? expirationInterval = new TimeSpan?();
        private object mutex;
        private AVQuery<AVInstallation> query;
        private IEnumerable<string> channels;
        private IDictionary<string, object> data;
        private string alert;

        public string Alert
        {
            get
            {
                lock (this.mutex)
                  return this.alert;
            }
            set
            {
                lock (this.mutex)
                {
                    if (this.Data != null && value != null)
                        throw new InvalidOperationException("A push may not have both an Alert and Data");
                    this.alert = value;
                }
            }
        }

        public IEnumerable<string> Channels
        {
            get
            {
                lock (this.mutex)
                  return this.channels;
            }
            set
            {
                lock (this.mutex)
                {
                    if (value != null && this.Query != null && this.Query.GetConstraint("channels") != null)
                        throw new InvalidOperationException("A push may not have both Channels and a Query with a channels constraint");
                    this.channels = value;
                }
            }
        }

        public IDictionary<string, object> Data
        {
            get
            {
                lock (this.mutex)
                  return this.data;
            }
            set
            {
                lock (this.mutex)
                {
                    if (this.Alert != null && value != null)
                        throw new InvalidOperationException("A push may not have both an Alert and Data");
                    this.data = value;
                }
            }
        }

        public DateTime? Expiration
        {
            get
            {
                lock (this.mutex)
                  return this.expiration;
            }
            set
            {
                lock (this.mutex)
                {
                    if (this.expirationInterval.HasValue)
                        throw new InvalidOperationException("Cannot set Expiration after setting ExpirationInterval");
                    this.expiration = value;
                }
            }
        }

        public TimeSpan? ExpirationInterval
        {
            get
            {
                lock (this.mutex)
                  return this.expirationInterval;
            }
            set
            {
                lock (this.mutex)
                {
                    if (this.expiration.HasValue)
                        throw new InvalidOperationException("Cannot set ExpirationInterval after setting Expiration");
                    this.expirationInterval = value;
                }
            }
        }

        public AVQuery<AVInstallation> Query
        {
            get
            {
                lock (this.mutex)
                  return this.query;
            }
            set
            {
                lock (this.mutex)
                {
                    if (this.Channels != null && value != null && value.GetConstraint("channels") != null)
                        throw new InvalidOperationException("A push may not have both Channels and a Query with a channels constraint");
                    this.query = value;
                }
            }
        }

        public AVPush()
        {
            this.mutex = new object();
            this.query = AVInstallation.Query;
        }

        internal IDictionary<string, object> Encode()
        {
            lock (this.mutex)
            {
                if (this.Alert == null && this.Data == null)
                    throw new InvalidOperationException("A push must have either an Alert or Data");
                if (this.Channels == null && this.Query == null)
                    throw new InvalidOperationException("A push must have either Channels or a Query");
                object local_1 = (object)this.Data;
                if (local_1 == null)
                    local_1 = (object)new Dictionary<string, object>()
          {
            {
              "alert",
              (object) this.Alert
            }
          };
                IDictionary<string, object> local_3 = (IDictionary<string, object>)local_1;
                AVQuery<AVInstallation> local_4 = this.Query ?? AVInstallation.Query;
                if (this.channels != null)
                    local_4 = local_4.WhereContainedIn<string>("channels", this.Channels);
                Dictionary<string, object> local_6 = new Dictionary<string, object>()
        {
          {
            "data",
            (object) local_3
          },
          {
            "where",
            InternalExtensions.GetOrDefault<string, object>(local_4.BuildParameters(false), "where", (object) new Dictionary<string, object>())
          }
        };
                if (this.expiration.HasValue)
                {
                    DateTime local_7 = this.expiration.Value;
                    local_6["expiration_time"] = (object)local_7.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
                else if (this.expirationInterval.HasValue)
                    local_6["expiration_interval"] = (object)this.expirationInterval.Value.TotalSeconds;
                return (IDictionary<string, object>)local_6;
            }
        }

        public static Task SendAlertAsync(string alert)
        {
            return new AVPush()
            {
                Alert = alert
            }.SendAsync();
        }

        public static Task SendAlertAsync(string alert, string channel)
        {
            return new AVPush()
            {
                Channels = ((IEnumerable<string>)new List<string>()
        {
          channel
        }),
                Alert = alert
            }.SendAsync();
        }

        public static Task SendAlertAsync(string alert, IEnumerable<string> channels)
        {
            return new AVPush()
            {
                Channels = channels,
                Alert = alert
            }.SendAsync();
        }

        public static Task SendAlertAsync(string alert, AVQuery<AVInstallation> query)
        {
            return new AVPush()
            {
                Query = query,
                Alert = alert
            }.SendAsync();
        }

        public Task SendAsync()
        {
            return this.SendAsync(CancellationToken.None);
        }

        public Task SendAsync(CancellationToken cancellationToken)
        {
            return (Task)AVClient.RequestAsync("POST", "/push", AVUser.CurrentUser != null ? AVUser.CurrentSessionToken : (string)null, this.Encode(), cancellationToken);
        }

        public static Task SendDataAsync(IDictionary<string, object> data)
        {
            return new AVPush()
            {
                Data = data
            }.SendAsync();
        }

        public static Task SendDataAsync(IDictionary<string, object> data, string channel)
        {
            return new AVPush()
            {
                Channels = ((IEnumerable<string>)new List<string>()
        {
          channel
        }),
                Data = data
            }.SendAsync();
        }

        public static Task SendDataAsync(IDictionary<string, object> data, IEnumerable<string> channels)
        {
            return new AVPush()
            {
                Channels = channels,
                Data = data
            }.SendAsync();
        }

        public static Task SendDataAsync(IDictionary<string, object> data, AVQuery<AVInstallation> query)
        {
            return new AVPush()
            {
                Query = query,
                Data = data
            }.SendAsync();
        }

        public static Task SubscribeAsync(string channel)
        {
            return AVPush.SubscribeAsync((IEnumerable<string>)new List<string>()
      {
        channel
      }, CancellationToken.None);
        }

        public static Task SubscribeAsync(string channel, CancellationToken cancellationToken)
        {
            return AVPush.SubscribeAsync((IEnumerable<string>)new List<string>()
      {
        channel
      }, cancellationToken);
        }

        public static Task SubscribeAsync(IEnumerable<string> channels)
        {
            return AVPush.SubscribeAsync(channels, CancellationToken.None);
        }

        public static Task SubscribeAsync(IEnumerable<string> channels, CancellationToken cancellationToken)
        {
            AVInstallation currentInstallation = AVInstallation.CurrentInstallation;
            currentInstallation.AddRangeUniqueToList<string>("channels", channels);
            return ((AVObject)currentInstallation).SaveAsync(cancellationToken);
        }

        public static Task UnsubscribeAsync(string channel)
        {
            return AVPush.UnsubscribeAsync((IEnumerable<string>)new List<string>()
      {
        channel
      }, CancellationToken.None);
        }

        public static Task UnsubscribeAsync(string channel, CancellationToken cancellationToken)
        {
            return AVPush.UnsubscribeAsync((IEnumerable<string>)new List<string>()
      {
        channel
      }, cancellationToken);
        }

        public static Task UnsubscribeAsync(IEnumerable<string> channels)
        {
            return AVPush.UnsubscribeAsync(channels, CancellationToken.None);
        }

        public static Task UnsubscribeAsync(IEnumerable<string> channels, CancellationToken cancellationToken)
        {
            AVInstallation currentInstallation = AVInstallation.CurrentInstallation;
            currentInstallation.RemoveAllFromList<string>("channels", channels);
            return ((AVObject)currentInstallation).SaveAsync(cancellationToken);
        }
    }
}