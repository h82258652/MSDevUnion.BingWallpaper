using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
	public class AVQuery<T> : AVQueryBase<AVQuery<T>, T>
	where T : AVObject
	{
		private string relativeUri;

		private string JsonString
		{
			get
			{
				return AVClient.SerializeJsonString(this.BuildParameters(true));
			}
		}

		internal string RelativeUri
		{
			get
			{
				string empty = string.Empty;
				empty = (!string.IsNullOrEmpty(this.relativeUri) ? string.Concat(this.relativeUri, "?{0}") : string.Concat("/classes/", Uri.EscapeDataString(this.className), "?{0}"));
				return empty;
			}
			set
			{
				this.relativeUri = value;
			}
		}

		private AVQuery(AVQuery<T> source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes) : base(source, where, replacementOrderBy, thenBy, skip, limit, includes)
		{
		}

		public AVQuery() : this(AVObject.GetClassName(typeof(T)))
		{
		}

		public AVQuery(string className) : base(className)
		{
		}

		public override Task<int> CountAsync()
		{
			return this.CountAsync(CancellationToken.None);
		}

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            this.EnsureNotInstallationQuery();
            IDictionary<string, object> parameters = this.BuildParameters(false);
            parameters["limit"] = 0;
            parameters["count"] = 1;
            return AVClient.RequestAsync("GET", string.Format(this.RelativeUri, (object)AVClient.BuildQueryString(parameters)), AVUser.CurrentSessionToken, null, cancellationToken).OnSuccess(t => Convert.ToInt32(t.Result.Item2["count"]));
        }

        internal T CreateAVObjectFromQueryResult(IDictionary<string, object> data)
		{
			T t = (T)AVObject.CreateWithoutData(this.className, null);
			t.MergeAfterFetch(data);
			return t;
		}

		internal override AVQuery<T> CreateInstance(AVQuery<T> source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes)
		{
			return new AVQuery<T>(this, where, replacementOrderBy, thenBy, skip, limit, includes);
		}

		internal override AVQuery<T> CreateInstance(AVQueryBase<AVQuery<T>, T> source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes)
		{
			return new AVQuery<T>(this, where, replacementOrderBy, thenBy, skip, limit, includes);
		}

		public static Task<IEnumerable<T>> DoCloudQuery(string cql)
		{
			return AVQuery<T>.DoCloudQuery(cql, CancellationToken.None);
		}

        public static Task<IEnumerable<T>> DoCloudQuery(string cql, CancellationToken cancellationToken)
        {
            return AVClient.RequestAsync("GET", $"/cloudQuery?cql={(object) Uri.EscapeDataString(cql)}", AVUser.CurrentSessionToken, (IDictionary<string, object>)null, cancellationToken).OnSuccess(t => new AVQuery<T>(t.Result.Item2["className"].ToString()).PrepareObjectsFromResults(t.Result.Item2["results"] as IList<object>));
        }

        public static Task<IEnumerable<T>> DoCloudQuery(string cql, IList<object> pvalues)
		{
			return AVQuery<T>.DoCloudQuery(cql, pvalues, CancellationToken.None);
		}

        public static Task<IEnumerable<T>> DoCloudQuery(string cql, IList<object> pvalues, CancellationToken cancellationToken)
        {
            string format = "/cloudQuery?cql={0}&pvalues={1}";
            string stringToEscape = Json.Encode(pvalues);
            return AVClient.RequestAsync("GET", string.Format(format, (object)Uri.EscapeDataString(cql), (object)Uri.EscapeDataString(stringToEscape)), AVUser.CurrentSessionToken, null, cancellationToken).OnSuccess(t => new AVQuery<T>(t.Result.Item2["className"].ToString()).PrepareObjectsFromResults(t.Result.Item2["results"] as IList<object>));
        }

        public override Task<IEnumerable<T>> FindAsync()
		{
			return this.FindAsync(CancellationToken.None);
		}

        public Task<IEnumerable<T>> FindAsync(CancellationToken cancellationToken)
        {
            this.EnsureNotInstallationQuery();
            return AVClient.RequestAsync("GET", string.Format(this.RelativeUri, (object)AVClient.BuildQueryString(this.BuildParameters(false))), AVUser.CurrentSessionToken, null, cancellationToken).OnSuccess(t => this.PrepareObjectsFromResults(t.Result.Item2["results"] as IList<object>));
        }

        public Task<T> FirstAsync()
		{
			return this.FirstAsync(CancellationToken.None);
		}

		public Task<T> FirstAsync(CancellationToken cancellationToken)
        {
            return this.FirstOrDefaultAsync(cancellationToken).OnSuccess<T, T>(t =>
            {
                if ((object)t.Result == null)
                    throw new AVException(AVException.ErrorCode.ObjectNotFound, "No results matched the query.", (Exception)null);
                else
                    return t.Result;
            });
        }

        public Task<T> FirstOrDefaultAsync()
		{
			return this.FirstOrDefaultAsync(CancellationToken.None);
		}

		public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            this.EnsureNotInstallationQuery();
            IDictionary<string, object> parameters = this.BuildParameters(false);
            parameters["limit"] = (object)1;
            return AVClient.RequestAsync("GET", string.Format(this.RelativeUri, (object)AVClient.BuildQueryString(parameters)), AVUser.CurrentSessionToken, null, cancellationToken).OnSuccess(t => this.PrepareObjectsFromResults(t.Result.Item2["results"] as IList<object>).FirstOrDefault<T>());
        }

        public override Task<T> GetAsync(string objectId)
		{
			return this.GetAsync(objectId, CancellationToken.None);
		}

		public Task<T> GetAsync(string objectId, CancellationToken cancellationToken)
		{
            AVQuery<T> source = new AVQuery<T>(this.className).WhereEqualTo("objectId", (object)objectId);
            //IEnumerable<string> includes = this.includes;
            int? skip = new int?();
            return new AVQuery<T>(source, null, null, null, skip, new int?(1), includes).FindAsync(cancellationToken).OnSuccess(t =>
            {
                T obj = t.Result.FirstOrDefault<T>();
                if ((object)obj == null)
                    throw new AVException(AVException.ErrorCode.ObjectNotFound, "Object with the given objectId found.", null);
                else
                    return obj;
            });
        }

		internal object GetConstraint(string key)
		{
			if (this.@where == null)
			{
				return null;
			}
			return this.@where.GetOrDefault<string, object>(key, null);
		}

		public AVQuery<T> Include(string key)
		{
			int? nullable = null;
			int? nullable1 = null;
			List<string> list = new List<string>();
			list.Add(key);
			return new AVQuery<T>(this, null, null, null, nullable, nullable1, list);
		}

		private HashSet<string> MergeIncludes(IEnumerable<string> includes)
		{
			if (this.includes == null)
			{
				return new HashSet<string>(includes);
			}
			HashSet<string> hashSet = new HashSet<string>(this.includes);
			foreach (string include in includes)
			{
				hashSet.Add(include);
			}
			return hashSet;
		}

		private IDictionary<string, object> MergeWhereClauses(IDictionary<string, object> where)
		{
            if (this.where == null)
                return where;
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>(this.@where);
            foreach (KeyValuePair<string, object> keyValuePair1 in @where)
            {
                IDictionary<string, object> dictionary2 = keyValuePair1.Value as IDictionary<string, object>;
                if (!dictionary1.ContainsKey(keyValuePair1.Key))
                {
                    dictionary1[keyValuePair1.Key] = keyValuePair1.Value;
                }
                else
                {
                    IDictionary<string, object> dictionary3 = dictionary1[keyValuePair1.Key] as IDictionary<string, object>;
                    if (dictionary3 == null || dictionary2 == null)
                        throw new ArgumentException("More than one where clause for the given key provided.");
                    Dictionary<string, object> dictionary4 = new Dictionary<string, object>(dictionary3);
                    foreach (KeyValuePair<string, object> keyValuePair2 in dictionary2)
                    {
                        if (dictionary4.ContainsKey(keyValuePair2.Key))
                            throw new ArgumentException("More than one condition for the given key provided.");
                        dictionary4[keyValuePair2.Key] = keyValuePair2.Value;
                    }
                    dictionary1[keyValuePair1.Key] = (object)dictionary4;
                }
            }
            return (IDictionary<string, object>)dictionary1;
        }

	    public static AVQuery<T> Or(IEnumerable<AVQuery<T>> queries)
	    {
	        string className = (string) null;
	        List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
	        foreach (AVQuery<T> avQuery in queries)
	        {
	            if (className != null && avQuery.className != className)
	                throw new ArgumentException("All of the queries in an or query must be on the same class.");
	            className = avQuery.className;
	            IDictionary<string, object> dictionary = avQuery.BuildParameters(false);
	            if (dictionary.Count != 0)
	            {
	                object obj;
	                if (!dictionary.TryGetValue("where", out obj) || dictionary.Count > 1)
	                    throw new ArgumentException("None of the queries in an or query can have non-filtering clauses");
	                list.Add(obj as IDictionary<string, object>);
	            }
	        }
	        return new AVQuery<T>(new AVQuery<T>(className), (IDictionary<string, object>) new Dictionary<string, object>()
	        {
	            {
	                "$or",
	                (object) list
	            }
	        }, (IEnumerable<string>) null, (IEnumerable<string>) null, new int?(), new int?(), (IEnumerable<string>) null);
	    }

	    internal IEnumerable<T> PrepareObjectsFromResults(IList<object> results)
	    {
	        var s =
	            results.Select(item => this.CreateAVObjectFromQueryResult(item as IDictionary<string, object>)).ToList<T>();
            return new ReadOnlyCollection<T>(s);
        }


        internal AVQuery<T> WhereRelatedTo(AVObject parent, string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("object", parent);
			dictionary1.Add("key", key);
			dictionary.Add("$relatedTo", dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return new AVQuery<T>(this, dictionary, null, null, nullable, nullable1, null);
		}
	}
}