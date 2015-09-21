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
    public class AVIMImageMessage : AVIMFileMessageBase
    {
        public AVIMImageMessage()
        {
            this.MediaType = AVIMMessageMediaType.Image;
        }

        public AVIMImageMessage(string name, Stream imageStrem)
            : base(name, imageStrem, AVIMMessageMediaType.Image)
        {
        }

        public AVIMImageMessage(string url)
            : base(url, AVIMMessageMediaType.Image)
        {
        }

        internal override Task<IDictionary<string, object>> GetMetaDataFromQiniu()
        {
            return
                InternalExtensions.OnSuccess<Tuple<HttpStatusCode, string>, IDictionary<string, object>>(
                    AVClient.RequestAsync(new Uri((string) (object) this.SourceFile.Url + (object) "?imageInfo"), "GET",
                        (IList<KeyValuePair<string, string>>) null, (Stream) null, string.Empty, CancellationToken.None),
                    (Func<Task<Tuple<HttpStatusCode, string>>, IDictionary<string, object>>) (t =>
                    {
                        if (t.Result.Item1 == HttpStatusCode.OK)
                        {
                            IDictionary<string, object> data = AVClient.DeserializeJsonString(t.Result.Item2);
                            if (data != null)
                            {
                                if (this.metaData == null)
                                    this.metaData = (IDictionary<string, object>) new Dictionary<string, object>();
                                this.metaData.Add("format",
                                    (object) AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "format"));
                                this.metaData.Add("width", (object) AVRMProtocolUtils.CaptureInteger(data, "width"));
                                this.metaData.Add("height", (object) AVRMProtocolUtils.CaptureInteger(data, "height"));
                            }
                        }
                        return this.metaData;
                    }));
        }

        internal override Task UploadFile()
        {
            return InternalExtensions.OnSuccess(base.UploadFile(), (Action<Task>) (t =>
            {
                this.metaData = (IDictionary<string, object>) new Dictionary<string, object>();
                if (this.IsExternalLink)
                    return;
                this.metaData.Add("size", this.SourceFile.GetMetaData()["size"]);
            }));
        }
    }
}