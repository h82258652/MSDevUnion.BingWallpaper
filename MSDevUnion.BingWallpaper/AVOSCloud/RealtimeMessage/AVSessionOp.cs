using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVSessionOp
	{
		internal IDictionary<string, object> data
		{
			get;
			set;
		}

		internal string id
		{
			get;
			set;
		}

		internal string op
		{
			get;
			set;
		}

		public AVSessionOp()
		{
		}
	}
}