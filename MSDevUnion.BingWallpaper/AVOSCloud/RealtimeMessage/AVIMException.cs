using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVIMException : Exception
	{
		public AVIMError Error
		{
			get;
			private set;
		}

		internal AVIMException()
		{
		}

		internal AVIMException(AVIMError error)
		{
			this.Error = error;
		}
	}
}