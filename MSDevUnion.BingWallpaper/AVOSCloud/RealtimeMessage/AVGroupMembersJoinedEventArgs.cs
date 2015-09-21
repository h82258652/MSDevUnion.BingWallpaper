using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVGroupMembersJoinedEventArgs : EventArgs
	{
		public IList<string> JoinedPeerIds
		{
			get;
			set;
		}

		public AVGroupMembersJoinedEventArgs()
		{
		}
	}
}