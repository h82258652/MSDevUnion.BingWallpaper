using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class AVAddUniqueOperation : IAVFieldOperation
	{
		private ReadOnlyCollection<object> objects;

		public IEnumerable<object> Objects
		{
			get
			{
				return this.objects;
			}
		}

		public AVAddUniqueOperation(IEnumerable<object> objects)
		{
			this.objects = new ReadOnlyCollection<object>(Enumerable.ToList<object>(Enumerable.Distinct<object>(objects)));
		}

		public object Apply(object oldValue, AVObject obj, string key)
		{
            if (oldValue == null)
                return this.objects.ToList();
            List<object> list = ((IEnumerable<object>)AVClient.ConvertTo<IList<object>>(oldValue)).ToList<object>();
            IEqualityComparer<object> avObjectComparer = AVFieldOperations.AVObjectComparer;
            foreach (object obj1 in this.objects)
            {
                object @object = obj1;
                if (!(@object is AVObject))
                {
                    if (!list.Contains<object>(@object, avObjectComparer))
                        list.Add(@object);
                }
                else
                {
                    object obj2 = list.FirstOrDefault(listObj => avObjectComparer.Equals(@object, listObj));
                    if (obj2 != null)
                        list[list.IndexOf(obj2)] = @object;
                    else
                        list.Add(@object);
                }
            }
            return (object)list;
        }

		public object Encode()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__op", "AddUnique");
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
				return new AVSetOperation(this.Apply(list, null, null));
			}
			if (!(previous is AVAddUniqueOperation))
			{
				throw new InvalidOperationException("Operation is invalid after previous operation.");
			}
			IEnumerable<object> objects = ((AVAddUniqueOperation)previous).Objects;
			return new AVAddUniqueOperation((IList<object>)this.Apply(objects, null, null));
		}
	}
}