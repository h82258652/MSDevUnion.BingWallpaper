using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVAck
	{
		public string MsgId
		{
			get;
			set;
		}

		public List<string> MsgIds
		{
			get;
			set;
		}

		public string PeerId
		{
			get;
			set;
		}

		public AVAck()
		{
		}
	}
}