using System;
using System.Collections.Generic;

namespace AVOSCloud.RealtimeMessage
{
	public delegate void PeersUnwatched(AVSession session, IList<string> peerIds);
}