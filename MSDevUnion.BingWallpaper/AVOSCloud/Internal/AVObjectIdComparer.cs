using AVOSCloud;
using System;
using System.Collections.Generic;

namespace AVOSCloud.Internal
{
	internal class AVObjectIdComparer : IEqualityComparer<object>
	{
		public AVObjectIdComparer()
		{
		}

		public int GetHashCode(object p)
		{
			AVObject aVObjects = p as AVObject;
			if (aVObjects == null)
			{
				return p.GetHashCode();
			}
			return aVObjects.ObjectId.GetHashCode();
		}

		bool System.Collections.Generic.IEqualityComparer<System.Object>.Equals(object p1, object p2)
		{
			AVObject aVObjects = p1 as AVObject;
			AVObject aVObjects1 = p2 as AVObject;
			if (aVObjects == null || aVObjects1 == null)
			{
				return object.Equals(p1, p2);
			}
			return object.Equals(aVObjects.ObjectId, aVObjects1.ObjectId);
		}
	}
}