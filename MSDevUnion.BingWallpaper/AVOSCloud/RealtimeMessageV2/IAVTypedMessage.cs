using System;
using System.Collections.Generic;

namespace AVOSCloud.RealtimeMessageV2
{
	public interface IAVTypedMessage
	{
		IDictionary<string, object> TypedMessageBody
		{
			get;
			set;
		}

		void Deserialize(IDictionary<string, object> typedMessageBodyFromServer);
	}
}