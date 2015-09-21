using System;

namespace AVOSCloud.RealtimeMessage
{
	public interface IAVSessionListener
	{
		Error OnError
		{
			get;
			set;
		}

		Message OnMessage
		{
			get;
			set;
		}

		MessageFailure OnMessageFailure
		{
			get;
			set;
		}

		MessageSent OnMessageSent
		{
			get;
			set;
		}

		PeersUnwatched OnPeersUnwatched
		{
			get;
			set;
		}

		PeersWatched OnPeersWatched
		{
			get;
			set;
		}

		SessionClosed OnSessionClosed
		{
			get;
			set;
		}

		SessionOpen OnSessionOpen
		{
			get;
			set;
		}

		SessionPaused OnSessionPaused
		{
			get;
			set;
		}

		SessionResumed OnSessionResumed
		{
			get;
			set;
		}

		StatusOffline OnStatusOffline
		{
			get;
			set;
		}

		StatusOnline OnStatusOnline
		{
			get;
			set;
		}
	}
}