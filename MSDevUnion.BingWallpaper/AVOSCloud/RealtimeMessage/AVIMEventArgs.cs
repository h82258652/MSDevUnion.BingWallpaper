using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVIMEventArgs : EventArgs
	{
		public AVAck Ack
		{
			get;
			set;
		}

		public AVAckReq AckReq
		{
			get;
			set;
		}

		public AVGroupOp GroupOp
		{
			get;
			set;
		}

		public AVMessage Message
		{
			get;
			set;
		}

		public AVPresence Presence
		{
			get;
			set;
		}

		public AVSessionOp SessionOp
		{
			get;
			set;
		}

		public AVIMEventArgs()
		{
		}
	}
}