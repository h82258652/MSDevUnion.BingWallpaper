using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVGroupMessageSentEventArgs : EventArgs
	{
		public AVMessage Message
		{
			get;
			set;
		}

		public AVGroupMessageSentEventArgs()
		{
		}
	}
}