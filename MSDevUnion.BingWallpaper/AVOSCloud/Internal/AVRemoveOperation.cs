using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AVOSCloud.Internal
{
	internal class AVRemoveOperation : IAVFieldOperation
	{
		private ReadOnlyCollection<object> objects;

		public IEnumerable<object> Objects
		{
			get
			{
				return this.objects;
			}
		}

		public AVRemoveOperation(IEnumerable<object> objects)
		{
			this.objects = new ReadOnlyCollection<object>(Enumerable.ToList<object>(Enumerable.Distinct<object>(objects)));
		}

		public object Apply(object oldValue, AVObject obj, string key)
		{
			if (oldValue == null)
			{
				return new List<object>();
			}
			IList<object> list = (IList<object>)AVClient.ConvertTo<IList<object>>(oldValue);
			return Enumerable.ToList<object>(Enumerable.Except<object>(list, this.objects, AVFieldOperations.AVObjectComparer));
		}

		public object Encode()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__op", "Remove");
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
				return previous;
			}
			if (previous is AVSetOperation)
			{
				IList<object> list = (IList<object>)AVClient.ConvertTo<IList<object>>(((AVSetOperation)previous).Value);
				return new AVSetOperation(this.Apply(list, null, null));
			}
			if (!(previous is AVRemoveOperation))
			{
				throw new InvalidOperationException("Operation is invalid after previous operation.");
			}
			AVRemoveOperation aVRemoveOperation = (AVRemoveOperation)previous;
			return new AVRemoveOperation(Enumerable.Concat<object>(aVRemoveOperation.Objects, this.objects));
		}
	}
}