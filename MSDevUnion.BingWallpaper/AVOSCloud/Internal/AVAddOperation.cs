using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AVOSCloud.Internal
{
	internal class AVAddOperation : IAVFieldOperation
	{
		private ReadOnlyCollection<object> objects;

		public IEnumerable<object> Objects
		{
			get
			{
				return this.objects;
			}
		}

		public AVAddOperation(IEnumerable<object> objects)
		{
			this.objects = new ReadOnlyCollection<object>(Enumerable.ToList<object>(objects));
		}

		public object Apply(object oldValue, AVObject obj, string key)
		{
			if (oldValue == null)
			{
				return Enumerable.ToList<object>(this.objects);
			}
			IList<object> list = (IList<object>)AVClient.ConvertTo<IList<object>>(oldValue);
			return Enumerable.ToList<object>(Enumerable.Concat<object>(list, this.objects));
		}

		public object Encode()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__op", "Add");
			dictionary.Add("objects", AVClient.MaybeEncodeJSONObject(this.objects, true));
			return dictionary;
		}

		public IAVFieldOperation MergeWithPrevious(IAVFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is AVDeleteOperation)
			{
				return new AVSetOperation(Enumerable.ToList<object>(this.objects));
			}
			if (previous is AVSetOperation)
			{
				IList<object> list = (IList<object>)AVClient.ConvertTo<IList<object>>(((AVSetOperation)previous).Value);
				return new AVSetOperation(Enumerable.ToList<object>(Enumerable.Concat<object>(list, this.objects)));
			}
			if (!(previous is AVAddOperation))
			{
				throw new InvalidOperationException("Operation is invalid after previous operation.");
			}
			return new AVAddOperation(Enumerable.Concat<object>(((AVAddOperation)previous).Objects, this.objects));
		}
	}
}