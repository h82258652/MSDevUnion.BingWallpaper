using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
	public class AVIMSignature
	{
		public string GroupAction
		{
			get;
			set;
		}

		public string GroupId
		{
			get;
			set;
		}

		public string Nonce
		{
			get;
			set;
		}

		public string SignatureContent
		{
			get;
			set;
		}

		public List<string> SignedPeerIds
		{
			get;
			set;
		}

		public long Timestamp
		{
			get;
			set;
		}

		public AVIMSignature()
		{
		}
	}
}