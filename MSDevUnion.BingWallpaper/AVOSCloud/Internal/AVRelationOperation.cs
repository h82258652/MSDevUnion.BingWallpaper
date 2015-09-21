using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
    internal class AVRelationOperation : IAVFieldOperation
    {
        private readonly IList<string> adds;
        private readonly IList<string> removes;
        private readonly string targetClassName;

        public string TargetClassName
        {
            get
            {
                return this.targetClassName;
            }
        }

        private AVRelationOperation(IEnumerable<string> adds, IEnumerable<string> removes, string targetClassName)
        {
            this.targetClassName = targetClassName;
            this.adds = (IList<string>)new ReadOnlyCollection<string>((IList<string>)Enumerable.ToList<string>(adds));
            this.removes = (IList<string>)new ReadOnlyCollection<string>((IList<string>)Enumerable.ToList<string>(removes));
        }

        public AVRelationOperation(IEnumerable<AVObject> adds, IEnumerable<AVObject> removes)
        {
            adds = adds ?? (IEnumerable<AVObject>)new AVObject[0];
            removes = removes ?? (IEnumerable<AVObject>)new AVObject[0];
            this.targetClassName = Enumerable.FirstOrDefault<string>(Enumerable.Select<AVObject, string>(Enumerable.Concat<AVObject>(adds, removes), (Func<AVObject, string>)(o => o.ClassName)));
            this.adds = (IList<string>)new ReadOnlyCollection<string>((IList<string>)Enumerable.ToList<string>(this.IdsFromObjects(adds)));
            this.removes = (IList<string>)new ReadOnlyCollection<string>((IList<string>)Enumerable.ToList<string>(this.IdsFromObjects(removes)));
        }

        public object Apply(object oldValue, AVObject obj, string key)
        {
            if (this.adds.Count == 0 && this.removes.Count == 0)
                return (object)null;
            if (oldValue == null)
                return (object)AVRelationBase.CreateRelation(obj, key, this.targetClassName);
            if (!(oldValue is AVRelationBase))
                throw new InvalidOperationException("Operation is invalid after previous operation.");
            AVRelationBase avRelationBase = (AVRelationBase)oldValue;
            string targetClassName = avRelationBase.TargetClassName;
            if (targetClassName != null && targetClassName != this.targetClassName)
            {
                throw new InvalidOperationException("Related object must be a " + targetClassName + ", but a " + this.targetClassName + " was passed in.");
            }
            else
            {
                avRelationBase.TargetClassName = this.targetClassName;
                return (object)avRelationBase;
            }
        }

        public object Encode()
        {
            List<object> list1 = Enumerable.ToList<object>(Enumerable.Select<string, object>((IEnumerable<string>)this.adds, (Func<string, object>)(id => AVClient.MaybeEncodeJSONObject((object)AVObject.CreateWithoutData(this.targetClassName, id), true))));
            List<object> list2 = Enumerable.ToList<object>(Enumerable.Select<string, object>((IEnumerable<string>)this.removes, (Func<string, object>)(id => AVClient.MaybeEncodeJSONObject((object)AVObject.CreateWithoutData(this.targetClassName, id), true))));
            Dictionary<string, object> dictionary1;
            if (list1.Count == 0)
                dictionary1 = (Dictionary<string, object>)null;
            else
                dictionary1 = new Dictionary<string, object>()
        {
          {
            "__op",
            (object) "AddRelation"
          },
          {
            "objects",
            (object) list1
          }
        };
            Dictionary<string, object> dictionary2 = dictionary1;
            Dictionary<string, object> dictionary3;
            if (list2.Count == 0)
                dictionary3 = (Dictionary<string, object>)null;
            else
                dictionary3 = new Dictionary<string, object>()
        {
          {
            "__op",
            (object) "RemoveRelation"
          },
          {
            "objects",
            (object) list2
          }
        };
            Dictionary<string, object> dictionary4 = dictionary3;
            if (dictionary2 == null || dictionary4 == null)
                return (object)dictionary2 ?? (object)dictionary4;
            return (object)new Dictionary<string, object>()
      {
        {
          "__op",
          (object) "Batch"
        },
        {
          "ops",
          (object) new Dictionary<string, object>[2]
          {
            dictionary2,
            dictionary4
          }
        }
      };
        }

        private IEnumerable<string> IdsFromObjects(IEnumerable<AVObject> objects)
        {
            foreach (AVObject avObject in objects)
            {
                if (avObject.ObjectId == null)
                    throw new ArgumentException("You can't add an unsaved AVObject to a relation.");
                if (!(avObject.ClassName == this.targetClassName))
                    throw new ArgumentException(string.Format("Tried to create a AVRelation with 2 different types: {0} and {1}", (object)this.targetClassName, (object)avObject.ClassName));
            }
            return Enumerable.Distinct<string>(Enumerable.Select<AVObject, string>(objects, (Func<AVObject, string>)(o => o.ObjectId)));
        }

        public IAVFieldOperation MergeWithPrevious(IAVFieldOperation previous)
        {
            if (previous == null)
                return (IAVFieldOperation)this;
            if (previous is AVDeleteOperation)
                throw new InvalidOperationException("You can't modify a relation after deleting it.");
            AVRelationOperation relationOperation = previous as AVRelationOperation;
            if (relationOperation == null)
                throw new InvalidOperationException("Operation is invalid after previous operation.");
            if (relationOperation.TargetClassName != this.TargetClassName)
                throw new InvalidOperationException(string.Format("Related object must be of class {0}, but {1} was passed in.", (object)relationOperation.TargetClassName, (object)this.TargetClassName));
            else
                return (IAVFieldOperation)new AVRelationOperation((IEnumerable<string>)Enumerable.ToList<string>(Enumerable.Union<string>((IEnumerable<string>)this.adds, Enumerable.Except<string>((IEnumerable<string>)relationOperation.adds, (IEnumerable<string>)this.removes))), (IEnumerable<string>)Enumerable.ToList<string>(Enumerable.Union<string>((IEnumerable<string>)this.removes, Enumerable.Except<string>((IEnumerable<string>)relationOperation.removes, (IEnumerable<string>)this.adds))), this.TargetClassName);
        }
    }
}