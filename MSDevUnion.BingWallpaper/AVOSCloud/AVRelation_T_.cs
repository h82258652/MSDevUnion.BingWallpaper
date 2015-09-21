using System;

namespace AVOSCloud
{
	public sealed class AVRelation<T> : AVRelationBase
	where T : AVObject
	{
		public AVQuery<T> Query
		{
			get
			{
				return base.GetQuery<T>();
			}
		}

		internal AVRelation(AVObject parent, string key) : base(parent, key)
		{
		}

		internal AVRelation(AVObject parent, string key, string targetClassName) : base(parent, key, targetClassName)
		{
		}

		public void Add(T obj)
		{
			base.Add(obj);
		}

		public void Remove(T obj)
		{
			base.Remove(obj);
		}
	}
}