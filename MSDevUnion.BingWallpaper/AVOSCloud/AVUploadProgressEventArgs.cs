using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
	public class AVUploadProgressEventArgs : EventArgs
	{
		public double Progress
		{
			get;
			internal set;
		}

		internal AVUploadProgressEventArgs()
		{
		}
	}
}