using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
	public class AVIMTextMessage : AVIMTypedMessage
	{
		public string TextContent
		{
			get;
			set;
		}

		public override IDictionary<string, object> TypedMessageBody
		{
			get
			{
				this.typedMessageBody = base.TypedMessageBody;
				this.typedMessageBody.Write(AVIMProtocol.LCTYPE, -1);
				this.typedMessageBody.Write(AVIMProtocol.LCTEXT, this.TextContent);
				return this.typedMessageBody;
			}
			set
			{
				this.typedMessageBody = value;
			}
		}

		public AVIMTextMessage()
		{
			this.MediaType = AVIMMessageMediaType.Text;
		}

		public AVIMTextMessage(string textContent) : this()
		{
			this.TextContent = textContent;
		}

		public override void Deserialize(IDictionary<string, object> typedMessageBodyFromServer)
		{
			base.Deserialize(typedMessageBodyFromServer);
			this.TextContent = typedMessageBodyFromServer.CaptureValueFromDictionary<string>(AVIMProtocol.LCTEXT);
		}
	}
}