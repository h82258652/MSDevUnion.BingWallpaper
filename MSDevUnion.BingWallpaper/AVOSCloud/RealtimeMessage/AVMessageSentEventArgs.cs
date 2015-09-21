using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVMessageSentEventArgs : EventArgs
	{
		public AVMessage Message
		{
			get;
			set;
		}

		public AVMessageSentEventArgs()
		{
		}
	}
}