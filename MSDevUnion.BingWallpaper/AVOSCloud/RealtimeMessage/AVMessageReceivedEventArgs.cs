using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVMessageReceivedEventArgs : EventArgs
	{
		public AVMessage Message
		{
			get;
			set;
		}

		public AVMessageReceivedEventArgs()
		{
		}
	}
}