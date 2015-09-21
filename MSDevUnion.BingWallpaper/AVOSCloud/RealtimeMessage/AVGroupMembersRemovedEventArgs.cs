using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVGroupMembersRemovedEventArgs : EventArgs
	{
		public string RemovedBy
		{
			get;
			set;
		}

		public AVGroupMembersRemovedEventArgs()
		{
		}
	}
}