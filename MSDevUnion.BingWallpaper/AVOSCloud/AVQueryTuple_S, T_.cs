using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVOSCloud
{
	internal interface AVQueryTuple<S, T>
	where T : IAVObjectBase
	{
		Task<int> CountAsync();

		Task<IEnumerable<T>> FindAsync();

		Task<T> GetAsync(string objectId);
	}
}