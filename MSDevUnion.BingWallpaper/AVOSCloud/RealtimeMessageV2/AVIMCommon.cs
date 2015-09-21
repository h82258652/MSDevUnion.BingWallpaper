using System;

namespace AVOSCloud.RealtimeMessageV2
{
	internal static class AVIMCommon
	{
		internal readonly static object mutex;

		private static int lastCmdId;

		internal static int NextCmdId
		{
			get
			{
				int num;
				lock (AVIMCommon.mutex)
				{
					AVIMCommon.lastCmdId = AVIMCommon.lastCmdId + 1;
					if (AVIMCommon.lastCmdId > 65535)
					{
						AVIMCommon.lastCmdId = -65536;
					}
					num = AVIMCommon.lastCmdId;
				}
				return num;
			}
		}

		static AVIMCommon()
		{
			AVIMCommon.mutex = new object();
			AVIMCommon.lastCmdId = -65536;
		}
	}
}