using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class WebSocketConnectedFaildArgs : EventArgs
	{
		public AVIMError Error
		{
			get;
			set;
		}

		public WebSocketConnectedFaildArgs()
		{
		}
	}
}