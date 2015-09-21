using AVOSCloud;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessageV2
{
    public abstract class AVIMFileMessageBase : AVIMTypedMessage
    {
        protected IDictionary<string, object> metaData;

        public string Url { get; set; }

        public IProgress<AVUploadProgressEventArgs> FileUploadProgress { get; set; }

        internal AVFile SourceFile { get; set; }

        public virtual IDictionary<string, object> FileMetaData { get; set; }

        public override IDictionary<string, object> TypedMessageBody
        {
            get
            {
                this.typedMessageBody = base.TypedMessageBody;
                if (this.FileMetaData == null)
                {
                    this.FileMetaData = (IDictionary<string, object>)new Dictionary<string, object>();
                    if (this.IsExternalLink)
                    {
                        this.FileMetaData.Add("url", (object)this.Url);
                    }
                    else
                    {
                        this.FileMetaData.Add("url", (object)this.SourceFile.Url.OriginalString);
                        this.FileMetaData.Add("objId", (object)this.SourceFile.ObjectId);
                        this.FileMetaData.Add("metaData", (object)this.metaData);
                    }
                }
                AVRMProtocolUtils.Write(this.typedMessageBody, AVIMProtocol.LCFILE, (object)this.FileMetaData);
                return this.typedMessageBody;
            }
            set
            {
                base.TypedMessageBody = value;
            }
        }

        internal bool Synced
        {
            get
            {
                if (this.SourceFile != null)
                    return this.SourceFile.ObjectId != null;
                else
                    return false;
            }
        }

        internal bool IsExternalLink { get; private set; }

        public AVIMFileMessageBase()
        {
        }

        public AVIMFileMessageBase(AVIMMessageMediaType mediaType)
        {
            this.MediaType = mediaType;
        }

        public AVIMFileMessageBase(string name, Stream fileStream, AVIMMessageMediaType mediaType)
        {
            this.SourceFile = new AVFile(name, fileStream);
            this.MediaType = mediaType;
        }

        public AVIMFileMessageBase(AVFile avFile)
        {
            this.SourceFile = avFile;
        }

        public AVIMFileMessageBase(string url, AVIMMessageMediaType mediaType)
          : this(mediaType)
        {
            this.Url = url;
            this.IsExternalLink = true;
        }

        internal virtual Task UploadFile()
        {
            return this.UploadFile(this.FileUploadProgress);
        }

        internal virtual Task UploadFile(IProgress<AVUploadProgressEventArgs> progress)
        {
            if (this.Synced || this.IsExternalLink)
                return (Task)Task.FromResult<bool>(true);
            else
                return this.SourceFile.SaveAsync(progress);
        }

        internal abstract Task<IDictionary<string, object>> GetMetaDataFromQiniu();

        public override void Deserialize(IDictionary<string, object> typedMessageBodyFromServer)
        {
            base.Deserialize(typedMessageBodyFromServer);
            this.FileMetaData = this.typedMessageBody[AVIMProtocol.LCFILE] as IDictionary<string, object>;
            this.Url = AVRMProtocolUtils.CaptureValueFromDictionary<string>(this.FileMetaData, "url");
        }
    }
}