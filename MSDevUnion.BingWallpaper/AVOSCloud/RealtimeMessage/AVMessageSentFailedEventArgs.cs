using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVMessageSentFailedEventArgs : EventArgs
	{
		public AVIMError Error
		{
			get;
			set;
		}

		public AVMessage Message
		{
			get;
			set;
		}

		public AVMessageSentFailedEventArgs()
		{
		}
	}
}