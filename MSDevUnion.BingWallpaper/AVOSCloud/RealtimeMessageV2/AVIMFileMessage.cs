using AVOSCloud;
using AVOSCloud.Internal;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessageV2
{
    public class AVIMFileMessage : AVIMFileMessageBase
    {
        public AVIMFileMessage()
        {
            this.MediaType = AVIMMessageMediaType.File;
        }

        public AVIMFileMessage(AVFile avFile)
          : base(avFile)
        {
            this.MediaType = AVIMMessageMediaType.File;
        }

        internal override Task<IDictionary<string, object>> GetMetaDataFromQiniu()
        {
            return InternalExtensions.OnSuccess(AVClient.RequestAsync(new Uri((string)(object)this.SourceFile.Url + (object)"?imageInfo"), "GET", (IList<KeyValuePair<string, string>>)null, (Stream)null, string.Empty, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, string>>, IDictionary<string, object>>)(t =>
            {
                if (t.Result.Item1 == HttpStatusCode.OK)
                {
                    IDictionary<string, object> data = AVClient.DeserializeJsonString(t.Result.Item2);
                    if (data != null)
                    {
                        if (this.metaData == null)
                            this.metaData = (IDictionary<string, object>)new Dictionary<string, object>();
                        this.metaData.Add("format", (object)AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "format"));
                        this.metaData.Add("width", (object)AVRMProtocolUtils.CaptureValueFromDictionary<int>(data, "width"));
                        this.metaData.Add("height", (object)AVRMProtocolUtils.CaptureValueFromDictionary<int>(data, "height"));
                    }
                }
                return this.metaData;
            }));
        }
    }
}