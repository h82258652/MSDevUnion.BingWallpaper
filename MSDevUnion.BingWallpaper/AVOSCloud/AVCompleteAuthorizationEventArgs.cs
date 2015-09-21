using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
	public class AVCompleteAuthorizationEventArgs : EventArgs
	{
		public IDictionary<string, object> AuthData
		{
			get;
			set;
		}

		public AVCompleteAuthorizationEventArgs()
		{
		}
	}
}