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
    internal class FlexibleDictionaryWrapper<TOut, TIn> : IDictionary<string, TOut>, ICollection<KeyValuePair<string, TOut>>, IEnumerable<KeyValuePair<string, TOut>>, IEnumerable
    {
        private readonly IDictionary<string, TIn> toWrap;

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

        public TOut this[string key]
        {
            get
            {
                return (TOut)AVClient.ConvertTo<TOut>((object)this.toWrap[key]);
            }
            set
            {
                this.toWrap[key] = (TIn)AVClient.ConvertTo<TIn>((object)value);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.toWrap.Keys;
            }
        }

        public ICollection<TOut> Values
        {
            get
            {
                return (ICollection<TOut>)Enumerable.ToList<TOut>(Enumerable.Select<TIn, TOut>((IEnumerable<TIn>)this.toWrap.Values, (Func<TIn, TOut>)(item => (TOut)AVClient.ConvertTo<TOut>((object)item))));
            }
        }

        public FlexibleDictionaryWrapper(IDictionary<string, TIn> toWrap)
        {
            this.toWrap = toWrap;
        }

        public void Add(string key, TOut value)
        {
            this.toWrap.Add(key, (TIn)AVClient.ConvertTo<TIn>((object)value));
        }

        public void Add(KeyValuePair<string, TOut> item)
        {
            this.toWrap.Add(new KeyValuePair<string, TIn>(item.Key, (TIn)AVClient.ConvertTo<TIn>((object)item.Value)));
        }

        public void Clear()
        {
            this.toWrap.Clear();
        }

        public bool Contains(KeyValuePair<string, TOut> item)
        {
            return this.toWrap.Contains(new KeyValuePair<string, TIn>(item.Key, (TIn)AVClient.ConvertTo<TIn>((object)item.Value)));
        }

        public bool ContainsKey(string key)
        {
            return this.toWrap.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, TOut>[] array, int arrayIndex)
        {
            Enumerable.ToList<KeyValuePair<string, TOut>>(Enumerable.Select<KeyValuePair<string, TIn>, KeyValuePair<string, TOut>>((IEnumerable<KeyValuePair<string, TIn>>)this.toWrap, (Func<KeyValuePair<string, TIn>, KeyValuePair<string, TOut>>)(pair => new KeyValuePair<string, TOut>(pair.Key, (TOut)AVClient.ConvertTo<TOut>((object)pair.Value))))).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, TOut>> GetEnumerator()
        {
            foreach (KeyValuePair<string, TIn> keyValuePair in (IEnumerable<KeyValuePair<string, TIn>>)this.toWrap)
            {
                string key = keyValuePair.Key;
                KeyValuePair<string, TIn> keyValuePair1 = keyValuePair;
                yield return new KeyValuePair<string, TOut>(key, (TOut)AVClient.ConvertTo<TOut>((object)keyValuePair1.Value));
            }
        }

        public bool Remove(string key)
        {
            return this.toWrap.Remove(key);
        }

        public bool Remove(KeyValuePair<string, TOut> item)
        {
            return this.toWrap.Remove(new KeyValuePair<string, TIn>(item.Key, (TIn)AVClient.ConvertTo<TIn>((object)item.Value)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        public bool TryGetValue(string key, out TOut value)
        {
            TIn @in;
            bool flag = this.toWrap.TryGetValue(key, out @in);
            value = (TOut)AVClient.ConvertTo<TOut>((object)@in);
            return flag;
        }
    }
}