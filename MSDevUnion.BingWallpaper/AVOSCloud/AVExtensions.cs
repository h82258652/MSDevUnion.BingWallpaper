using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
	public static class AVExtensions
	{
		public static Task<IEnumerable<T>> FetchAllAsync<T>(this IEnumerable<T> objects)
		where T : AVObject
		{
			return AVObject.FetchAllAsync<T>(objects);
		}

		public static Task<IEnumerable<T>> FetchAllAsync<T>(this IEnumerable<T> objects, CancellationToken cancellationToken)
		where T : AVObject
		{
			return AVObject.FetchAllAsync<T>(objects, cancellationToken);
		}

		public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(this IEnumerable<T> objects)
		where T : AVObject
		{
			return AVObject.FetchAllIfNeededAsync<T>(objects);
		}

		public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(this IEnumerable<T> objects, CancellationToken cancellationToken)
		where T : AVObject
		{
			return AVObject.FetchAllIfNeededAsync<T>(objects, cancellationToken);
		}

		public static Task<T> FetchAsync<T>(this T obj)
		where T : AVObject
        {
            return obj.FetchAsyncInternal(CancellationToken.None).OnSuccess((Task<AVObject> t) => (T)((object)t.Result));
        }

        public static Task<T> FetchAsync<T>(this T obj, CancellationToken cancellationToken)
		where T : AVObject
        {
            return obj.FetchAsyncInternal(cancellationToken).OnSuccess((Task<AVObject> t) => (T)((object)t.Result));
        }


        public static Task<T> FetchIfNeededAsync<T>(this T obj)
		where T : AVObject
        {
            return obj.FetchIfNeededAsyncInternal(CancellationToken.None).OnSuccess((Task<AVObject> t) => (T)((object)t.Result));
        }


        public static Task<T> FetchIfNeededAsync<T>(this T obj, CancellationToken cancellationToken)
		where T : AVObject
        {
            return obj.FetchIfNeededAsyncInternal(cancellationToken).OnSuccess((Task<AVObject> t) => (T)((object)t.Result));
        }


        public static string InsertAttrPrefix(this string key)
		{
			return key.Insert(0, "attr.");
		}

		public static AVQuery<T> Or<T>(this AVQuery<T> source, params AVQuery<T>[] queries)
		where T : AVObject
		{
			AVQuery<T>[] aVQueryArray = new AVQuery<T>[] { source };
			return AVQuery<T>.Or(Enumerable.Concat<AVQuery<T>>(queries, aVQueryArray));
		}

		public static Task SaveAllAsync<T>(this IEnumerable<T> objects)
		where T : AVObject
		{
			return AVObject.SaveAllAsync<T>(objects);
		}

		public static Task SaveAllAsync<T>(this IEnumerable<T> objects, CancellationToken cancellationToken)
		where T : AVObject
		{
			return AVObject.SaveAllAsync<T>(objects, cancellationToken);
		}
	}
}