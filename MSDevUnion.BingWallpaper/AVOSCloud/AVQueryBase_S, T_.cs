using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AVOSCloud
{
	public abstract class AVQueryBase<S, T> : AVQueryTuple<S, T>
	where T : IAVObjectBase
	{
		protected readonly string className;

		protected readonly Dictionary<string, object> @where;

		protected readonly ReadOnlyCollection<string> orderBy;

		protected readonly ReadOnlyCollection<string> includes;

		protected readonly int? skip;

		protected readonly int? limit;

		protected AVQueryBase()
		{
		}

		protected AVQueryBase(AVQueryBase<S, T> source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes)
		{
			int? nullable;
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.className = source.className;
			this.@where = source.@where;
			this.orderBy = source.orderBy;
			this.skip = source.skip;
			this.limit = source.limit;
			this.includes = source.includes;
			if (where != null)
			{
				this.@where = new Dictionary<string, object>(this.MergeWhereClauses(where));
			}
			if (replacementOrderBy != null)
			{
				this.orderBy = new ReadOnlyCollection<string>(Enumerable.ToList<string>(replacementOrderBy));
			}
			if (thenBy != null)
			{
				if (this.orderBy == null)
				{
					throw new ArgumentException("You must call OrderBy before calling ThenBy.");
				}
				List<string> list = new List<string>(this.orderBy);
				list.AddRange(thenBy);
				this.orderBy = new ReadOnlyCollection<string>(list);
			}
			if (skip.HasValue)
			{
				int? nullable1 = this.skip;
				int num = (nullable1.HasValue ? nullable1.GetValueOrDefault() : 0);
				int? nullable2 = skip;
				if (!nullable2.HasValue)
				{
					nullable = null;
				}
				else
				{
					nullable = new int?(num + nullable2.GetValueOrDefault());
				}
				this.skip = nullable;
			}
			if (limit.HasValue)
			{
				this.limit = limit;
			}
			if (includes != null)
			{
				HashSet<string> hashSet = this.MergeIncludes(includes);
				this.includes = new ReadOnlyCollection<string>(Enumerable.ToList<string>(hashSet));
			}
		}

		public AVQueryBase(string className)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className", "Must specify a AVObject class name when creating a AVQuery.");
			}
			this.className = className;
		}

        internal virtual IDictionary<string, object> BuildParameters(bool includeClassName)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (this.where != null)
                dictionary["where"] = AVClient.MaybeEncodeJSONObject((object)this.where, true);
            if (this.orderBy != null)
                dictionary["order"] = (object)string.Join(",", Enumerable.ToArray<string>((IEnumerable<string>)this.orderBy));
            if (this.skip.HasValue)
                dictionary["skip"] = (object)this.skip.Value;
            if (this.limit.HasValue)
                dictionary["limit"] = (object)this.limit.Value;
            if (this.includes != null)
                dictionary["include"] = (object)string.Join(",", Enumerable.ToArray<string>((IEnumerable<string>)this.includes));
            if (includeClassName)
                dictionary["className"] = (object)this.className;
            return (IDictionary<string, object>)dictionary;
        }

        public abstract Task<int> CountAsync();

		internal abstract S CreateInstance(S source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes);

		internal abstract S CreateInstance(AVQueryBase<S, T> source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes);

		internal IDictionary<string, object> EncodeRegex(Regex regex, string modifiers)
		{
            string regexOptions = this.GetRegexOptions(regex, modifiers);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["$regex"] = regex.ToString();
            if (!string.IsNullOrEmpty(regexOptions))
                dictionary["$options"] = regexOptions;
            return (IDictionary<string, object>)dictionary;
        }

		internal void EnsureNotInstallationQuery()
		{
			if (this.className.Equals("_Installation"))
			{
				throw new InvalidOperationException("Cannot directly query the Installation class.");
			}
		}

		public abstract Task<IEnumerable<T>> FindAsync();

		public abstract Task<T> GetAsync(string objectId);

		private string GetRegexOptions(Regex regex, string modifiers)
		{
			string str = modifiers ?? "";
			if (regex.Options.HasFlag((RegexOptions)1) && !modifiers.Contains("i"))
			{
				str = string.Concat(str, "i");
			}
			if (regex.Options.HasFlag((RegexOptions)2) && !modifiers.Contains("m"))
			{
				str = string.Concat(str, "m");
			}
			return str;
		}

		public S Limit(int count)
		{
			int? nullable = null;
			return this.CreateInstance(this, null, null, null, nullable, new int?(count), null);
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
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>((IDictionary<string, object>)this.where);
            foreach (KeyValuePair<string, object> keyValuePair1 in (IEnumerable<KeyValuePair<string, object>>)where)
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
                    foreach (KeyValuePair<string, object> keyValuePair2 in (IEnumerable<KeyValuePair<string, object>>)dictionary2)
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

		public S OrderBy(string key)
		{
			List<string> list = new List<string>();
			list.Add(key);
			List<string> list1 = list;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, null, list1, null, nullable, nullable1, null);
		}

		public S OrderByDescending(string key)
		{
			List<string> list = new List<string>();
			list.Add(string.Concat("-", key));
			List<string> list1 = list;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, null, list1, null, nullable, nullable1, null);
		}

		internal string RegexQuote(string input)
		{
			return string.Concat("\\Q", input.Replace("\\E", "\\E\\\\E\\Q"), "\\E");
		}

		public S Skip(int count)
		{
			int? nullable = null;
			return this.CreateInstance(this, null, null, null, new int?(count), nullable, null);
		}

		public S ThenBy(string key)
		{
			List<string> list = new List<string>();
			list.Add(key);
			List<string> list1 = list;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, null, null, list1, nullable, nullable1, null);
		}

		public S ThenByDescending(string key)
		{
			List<string> list = new List<string>();
			list.Add(string.Concat("-", key));
			List<string> list1 = list;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, null, null, list1, nullable, nullable1, null);
		}

		public S WhereContainedIn<TIn>(string key, IEnumerable<TIn> values)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$in", Enumerable.ToList<TIn>(values));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereContains(string key, string substring)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$regex", this.RegexQuote(substring));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereContainsAll<TIn>(string key, IEnumerable<TIn> values)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$all", Enumerable.ToList<TIn>(values));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereDoesNotExist(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$exists", false);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereDoesNotMatchQuery<TOther>(string key, AVQuery<TOther> query)
		where TOther : AVObject
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$notInQuery", query.BuildParameters(true));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereEndsWith(string key, string suffix)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$regex", string.Concat(this.RegexQuote(suffix), "$"));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public virtual S WhereEqualTo(string key, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(key, value);
			Dictionary<string, object> dictionary1 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary1, null, null, nullable, nullable1, null);
		}

		public S WhereExists(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$exists", true);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereGreaterThan(string key, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$gt", value);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereGreaterThanOrEqualTo(string key, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$gte", value);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereLessThan(string key, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$lt", value);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereLessThanOrEqualTo(string key, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$lte", value);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereMatches(string key, Regex regex, string modifiers)
		{
			if (!regex.Options.HasFlag((RegexOptions)256))
			{
				throw new ArgumentException("Only ECMAScript-compatible regexes are supported.  Please use the ECMAScript RegexOptions flag when creating your regex.");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(key, this.EncodeRegex(regex, modifiers));
			Dictionary<string, object> dictionary1 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary1, null, null, nullable, nullable1, null);
		}

		public S WhereMatches(string key, Regex regex)
		{
			return this.WhereMatches(key, regex, null);
		}

		public S WhereMatches(string key, string pattern, string modifiers)
		{
            return this.WhereMatches(key, new Regex(pattern, RegexOptions.ECMAScript), modifiers);
        }

        public S WhereMatches(string key, string pattern)
		{
			return this.WhereMatches(key, pattern, null);
		}

		public S WhereMatchesKeyInQuery<TOther>(string key, string keyInQuery, AVQuery<TOther> query)
		where TOther : AVObject
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("query", query.BuildParameters(true));
			dictionary.Add("key", keyInQuery);
			Dictionary<string, object> dictionary1 = dictionary;
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.Add("$select", dictionary1);
			dictionary2.Add(key, dictionary3);
			Dictionary<string, object> dictionary4 = dictionary2;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary4, null, null, nullable, nullable1, null);
		}

		public S WhereMatchesQuery<TOther>(string key, AVQuery<TOther> query)
		where TOther : AVObject
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$inQuery", query.BuildParameters(true));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereNear(string key, AVGeoPoint point)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$nearSphere", point);
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereNotContainedIn<TIn>(string key, IEnumerable<TIn> values)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$nin", Enumerable.ToList<TIn>(values));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

		public S WhereNotEqualTo(string key, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$ne", value);
			dictionary.Add(key, dictionary1);
			Dictionary<string, object> dictionary2 = dictionary;
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary2, null, null, nullable, nullable1, null);
		}

		public S WhereStartsWith(string key, string suffix)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			dictionary1.Add("$regex", string.Concat("^", this.RegexQuote(suffix)));
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}

	    public S WhereWithinDistance(string key, AVGeoPoint point, AVGeoDistance maxDistance)
	    {
	        S source = this.WhereNear(key, point);
	        Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
	        Dictionary<string, object> dictionary2 = new Dictionary<string, object>()
	        {
	            {
	                "$maxDistance",
	                (object) maxDistance.Radians
	            }
	        };
	        dictionary1.Add(key, (object) dictionary2);
	        int? skip = new int?();
	        int? limit = new int?();
	        return this.CreateInstance(source, (IDictionary<string, object>) dictionary1, (IEnumerable<string>) null,
	            (IEnumerable<string>) null, skip, limit, (IEnumerable<string>) null);
	    }

	    public S WhereWithinGeoBox(string key, AVGeoPoint southwest, AVGeoPoint northeast)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("$box", new AVGeoPoint[] { southwest, northeast });
			dictionary1.Add("$within", dictionary2);
			dictionary.Add(key, dictionary1);
			int? nullable = null;
			int? nullable1 = null;
			return this.CreateInstance(this, dictionary, null, null, nullable, nullable1, null);
		}
	}
}