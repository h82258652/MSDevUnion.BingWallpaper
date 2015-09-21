using System;

namespace AVOSCloud.RealtimeMessageV2
{
	public delegate T ConvertTo<T>(AVIMMessage avIMMessage)
	where T : AVIMMessage;
}