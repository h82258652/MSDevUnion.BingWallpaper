using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
	public class AVDownloadProgressEventArgs : EventArgs
	{
		public double Progress
		{
			get;
			internal set;
		}

		internal AVDownloadProgressEventArgs()
		{
		}
	}
}