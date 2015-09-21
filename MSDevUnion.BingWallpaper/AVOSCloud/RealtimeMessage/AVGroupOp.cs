using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	internal class AVGroupOp
	{
		internal IDictionary<string, object> data
		{
			get;
			set;
		}

		internal string groupId
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

		public AVGroupOp()
		{
		}
	}
}