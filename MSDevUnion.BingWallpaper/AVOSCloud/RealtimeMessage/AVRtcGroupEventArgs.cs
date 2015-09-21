using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVRtcGroupEventArgs : EventArgs
	{
		public AVGroupOp GroupOp
		{
			get;
			set;
		}

		public AVRtcGroupEventArgs()
		{
		}
	}
}