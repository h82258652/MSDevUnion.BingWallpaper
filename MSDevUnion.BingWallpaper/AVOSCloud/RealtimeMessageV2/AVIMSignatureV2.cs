using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
	public class AVIMSignatureV2
	{
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

		public long Timestamp
		{
			get;
			set;
		}

		public AVIMSignatureV2()
		{
		}
	}
}