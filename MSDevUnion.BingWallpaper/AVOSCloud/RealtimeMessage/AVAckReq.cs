using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVAckReq
	{
		public string MsgCount
		{
			get;
			set;
		}

		public string PeerId
		{
			get;
			set;
		}

		public AVAckReq()
		{
		}
	}
}