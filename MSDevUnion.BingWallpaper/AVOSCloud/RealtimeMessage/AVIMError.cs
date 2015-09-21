using AVOSCloud;
using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVIMError
	{
		public AVException.ErrorCode Code
		{
			get;
			set;
		}

		public AVIMError()
		{
		}
	}
}