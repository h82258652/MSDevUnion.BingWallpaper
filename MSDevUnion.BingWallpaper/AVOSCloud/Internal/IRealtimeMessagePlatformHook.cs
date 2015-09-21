using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVOSCloud.Internal
{
	internal interface IRealtimeMessagePlatformHook
	{
		SocketStatus status
		{
			get;
			set;
		}

		Task<Tuple<string, IDictionary<string, object>>> EAP2TAP4WebSocket(IDictionary<string, object> cmd);

		event EventHandler<IDictionary<string, object>> OnReceived;
	}
}