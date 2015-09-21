using AVOSCloud;
using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessageV2
{
	public class AVIMAudioMessage : AVIMFileMessageBase
	{
		public AVIMAudioMessage()
		{
			this.MediaType = AVIMMessageMediaType.Audio;
		}

		public AVIMAudioMessage(string name, Stream audioStream) : base(name, audioStream, AVIMMessageMediaType.Audio)
		{
		}

		public AVIMAudioMessage(string url) : base(url, AVIMMessageMediaType.Audio)
		{
		}

        internal override Task<IDictionary<string, object>> GetMetaDataFromQiniu()
        {
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, string>, IDictionary<string, object>>(AVClient.RequestAsync(new Uri((string)(object)this.SourceFile.Url + (object)"?avinfo"), "GET", (IList<KeyValuePair<string, string>>)null, (Stream)null, string.Empty, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, string>>, IDictionary<string, object>>)(t =>
            {
                if (t.Result.Item1 == HttpStatusCode.OK)
                {
                    IDictionary<string, object> dictionary1 = AVClient.DeserializeJsonString(t.Result.Item2);
                    if (dictionary1 != null)
                    {
                        if (this.metaData == null)
                            this.metaData = (IDictionary<string, object>)new Dictionary<string, object>();
                        IDictionary<string, object> dictionary2 = dictionary1["format"] as IDictionary<string, object>;
                        string str1 = dictionary2["format_name"].ToString();
                        string str2 = dictionary2["duration"].ToString();
                        string str3 = dictionary2["size"].ToString();
                        this.metaData.Add("format", (object)str1);
                        this.metaData.Add("duration", (object)Convert.ToDouble(str2));
                        this.metaData.Add("size", (object)Convert.ToDouble(str3));
                    }
                }
                return this.metaData;
            }));
        }
    }
}