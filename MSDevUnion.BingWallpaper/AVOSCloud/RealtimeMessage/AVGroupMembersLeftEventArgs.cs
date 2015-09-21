using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVGroupMembersLeftEventArgs : EventArgs
	{
		public IList<string> LeftPeerIds
		{
			get;
			set;
		}

		public AVGroupMembersLeftEventArgs()
		{
		}
	}
}