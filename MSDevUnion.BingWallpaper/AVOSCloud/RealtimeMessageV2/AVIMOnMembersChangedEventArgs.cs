using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
	public class AVIMOnMembersChangedEventArgs : EventArgs
	{
		public IList<string> AffectedMembers
		{
			get;
			set;
		}

		public AVIMConversationEventType AffectedType
		{
			get;
			internal set;
		}

		public AVIMConversation Conversation
		{
			get;
			set;
		}

		public DateTime OpratedTime
		{
			get;
			set;
		}

		public string Oprator
		{
			get;
			set;
		}

		public AVIMOnMembersChangedEventArgs()
		{
		}
	}
}