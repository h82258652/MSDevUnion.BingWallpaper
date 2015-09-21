using AVOSCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
    internal class FlexibleListWrapper<TOut, TIn> : IList<TOut>, ICollection<TOut>, IEnumerable<TOut>, IEnumerable
    {
        private IList<TIn> toWrap;

        public int Count
        {
            get
            {
                return this.toWrap.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.toWrap.IsReadOnly;
            }
        }

        public TOut this[int index]
        {
            get
            {
                return (TOut)AVClient.ConvertTo<TOut>((object)this.toWrap[index]);
            }
            set
            {
                this.toWrap[index] = (TIn)AVClient.ConvertTo<TIn>((object)value);
            }
        }

        public FlexibleListWrapper(IList<TIn> toWrap)
        {
            this.toWrap = toWrap;
        }

        public void Add(TOut item)
        {
            this.toWrap.Add((TIn)AVClient.ConvertTo<TIn>((object)item));
        }

        public void Clear()
        {
            this.toWrap.Clear();
        }

        public bool Contains(TOut item)
        {
            return this.toWrap.Contains((TIn)AVClient.ConvertTo<TIn>((object)item));
        }

        public void CopyTo(TOut[] array, int arrayIndex)
        {
            Enumerable.ToList<TOut>(Enumerable.Select<TIn, TOut>((IEnumerable<TIn>)this.toWrap, (Func<TIn, TOut>)(item => (TOut)AVClient.ConvertTo<TOut>((object)item)))).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            foreach (TIn @in in (IEnumerable<TIn>)this.toWrap)
            {
                object obj = (object)@in;
                yield return (TOut)AVClient.ConvertTo<TOut>(obj);
            }
        }

        public int IndexOf(TOut item)
        {
            return this.toWrap.IndexOf((TIn)AVClient.ConvertTo<TIn>((object)item));
        }

        public void Insert(int index, TOut item)
        {
            this.toWrap.Insert(index, (TIn)AVClient.ConvertTo<TIn>((object)item));
        }

        public bool Remove(TOut item)
        {
            return this.toWrap.Remove((TIn)AVClient.ConvertTo<TIn>((object)item));
        }

        public void RemoveAt(int index)
        {
            this.toWrap.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}