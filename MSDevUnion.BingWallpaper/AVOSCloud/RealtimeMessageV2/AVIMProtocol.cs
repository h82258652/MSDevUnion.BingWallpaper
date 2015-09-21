using System;

namespace AVOSCloud.RealtimeMessageV2
{
	internal class AVIMProtocol
	{
		internal readonly string CMD = "cmd";

		internal readonly string CONV = "conv";

		internal readonly string DIRECT = "direct";

		internal readonly string QUERY = "query";

		internal readonly string OP = "op";

		internal readonly string PEERID = "peerId";

		internal readonly string APPID = "appId";

		internal readonly string M = "m";

		internal readonly string I = "i";

		internal readonly string CID = "cid";

		internal readonly static string LCTYPE;

		internal readonly static string LCFILE;

		internal readonly static string LCTEXT;

		internal readonly static string LCATTRS;

		internal readonly static string LCLOC;

		static AVIMProtocol()
		{
			AVIMProtocol.LCTYPE = "_lctype";
			AVIMProtocol.LCFILE = "_lcfile";
			AVIMProtocol.LCTEXT = "_lctext";
			AVIMProtocol.LCATTRS = "_lcattrs";
			AVIMProtocol.LCLOC = "_lcloc";
		}

		public AVIMProtocol()
		{
		}
	}
}