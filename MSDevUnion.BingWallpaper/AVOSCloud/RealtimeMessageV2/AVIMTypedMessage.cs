using AVOSCloud;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
    public abstract class AVIMTypedMessage : AVIMMessage, IAVTypedMessage
    {
        internal IDictionary<string, object> typedMessageBody;

        public virtual AVIMMessageMediaType MediaType { get; set; }

        public IDictionary<string, object> Attributes { get; set; }

        public virtual IDictionary<string, object> TypedMessageBody
        {
            get
            {
                if (this.typedMessageBody == null)
                {
                    this.typedMessageBody = (IDictionary<string, object>)new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(this.Title))
                        AVRMProtocolUtils.Write(this.typedMessageBody, AVIMProtocol.LCTEXT, (object)this.Title);
                    if (this.Attributes != null)
                        AVRMProtocolUtils.Write(this.typedMessageBody, AVIMProtocol.LCATTRS, (object)this.Attributes);
                    AVRMProtocolUtils.Write(this.typedMessageBody, AVIMProtocol.LCTYPE, (object)this.MediaType);
                }
                return this.typedMessageBody;
            }
            set
            {
                this.typedMessageBody = value;
            }
        }

        public string Title { get; set; }

        public override string MessageBody
        {
            get
            {
                return AVClient.SerializeJsonString(this.TypedMessageBody);
            }
            set
            {
                base.MessageBody = value;
            }
        }

        public AVIMTypedMessage()
        {
        }

        public AVIMTypedMessage(AVIMMessage avMessage)
        {
        }

        internal void CopyFromBase(AVIMMessage avMessage)
        {
            this.Id = avMessage.Id;
            this.ConversationId = avMessage.ConversationId;
            this.cmdId = avMessage.cmdId;
            this.FromClientId = avMessage.FromClientId;
            this.MessageIOType = avMessage.MessageIOType;
            this.MessageStatus = avMessage.MessageStatus;
            this.Receipt = avMessage.Receipt;
            this.ServerTimestamp = avMessage.ServerTimestamp;
            this.Transient = avMessage.Transient;
            this.typedMessageBody = AVClient.DeserializeJsonString(avMessage.MessageBody);
            this.Attributes = this.typedMessageBody[AVIMProtocol.LCATTRS] as IDictionary<string, object>;
        }

        public virtual void Deserialize(IDictionary<string, object> typedMessageBodyFromServer)
        {
            this.MediaType = (AVIMMessageMediaType)AVRMProtocolUtils.CaptureInteger(typedMessageBodyFromServer, AVIMProtocol.LCTYPE);
        }
    }
}