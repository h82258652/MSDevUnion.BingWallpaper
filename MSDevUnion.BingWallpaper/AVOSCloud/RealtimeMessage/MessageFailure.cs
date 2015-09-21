using System;

namespace AVOSCloud.RealtimeMessage
{
	public delegate void MessageFailure(AVSession session, AVMessage message, AVIMError error);
}