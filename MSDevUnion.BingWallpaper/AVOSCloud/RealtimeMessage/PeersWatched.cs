using System;
using System.Collections.Generic;

namespace AVOSCloud.RealtimeMessage
{
	public delegate void PeersWatched(AVSession session, IList<string> peerIds);
}