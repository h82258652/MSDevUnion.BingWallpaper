using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVMessage
	{
		public string FromPeerId
		{
			get;
			set;
		}

		public string GroupId
		{
			get;
			set;
		}

		internal string Id
		{
			get;
			set;
		}

		public bool IsTransient
		{
			get;
			set;
		}

		internal string localId
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public long Timestamp
		{
			get;
			set;
		}

		public IList<string> ToPeerIds
		{
			get;
			set;
		}

		public AVMessage()
		{
		}
	}
}