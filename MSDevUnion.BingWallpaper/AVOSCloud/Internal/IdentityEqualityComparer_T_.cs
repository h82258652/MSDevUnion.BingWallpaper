using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class IdentityEqualityComparer<T> : IEqualityComparer<T>
	{
		public IdentityEqualityComparer()
		{
		}

		public bool Equals(T x, T y)
		{
			return object.ReferenceEquals(x, y);
		}

		public int GetHashCode(T obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
}