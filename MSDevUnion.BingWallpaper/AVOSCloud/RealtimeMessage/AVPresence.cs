using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVPresence
	{
		public string PeerId
		{
			get;
			set;
		}

		public List<string> SessionPeerIds
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public AVPresence()
		{
		}
	}
}