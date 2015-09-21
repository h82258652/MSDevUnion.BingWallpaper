using System;
using System.Collections.Generic;

namespace AVOSCloud.Internal
{
	internal static class AVFieldOperations
	{
		private static AVObjectIdComparer comparer;

		public static IEqualityComparer<object> AVObjectComparer
		{
			get
			{
				if (comparer == null)
				{
					comparer = new AVObjectIdComparer();
				}
				return comparer;
			}
		}

		public static IAVFieldOperation Decode(IDictionary<string, object> json)
		{
			throw new NotImplementedException();
		}
	}
}