using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
	public class AVIMOnMessageReceivedEventArgs : EventArgs
	{
		public AVIMConversation Conversation
		{
			get;
			set;
		}

		public AVIMMessage Message
		{
			get;
			set;
		}

		public AVIMOnMessageReceivedEventArgs()
		{
		}
	}
}