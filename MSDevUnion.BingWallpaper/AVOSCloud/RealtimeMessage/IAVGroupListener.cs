using System;

namespace AVOSCloud.RealtimeMessage
{
	public interface IAVGroupListener
	{
		GroupMessageReceived OnGroupMessageReceived
		{
			get;
			set;
		}

		GroupMessageSent OnGroupMessageSent
		{
			get;
			set;
		}

		Joined OnJoined
		{
			get;
			set;
		}

		Left OnLeft
		{
			get;
			set;
		}

		MemberLeft OnMemberLeft
		{
			get;
			set;
		}

		MembersJoined OnMembersJoined
		{
			get;
			set;
		}

		Reject OnReject
		{
			get;
			set;
		}
	}
}