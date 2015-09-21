using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AVOSCloud.Internal
{
    internal class LockSet
    {
        private static readonly ConditionalWeakTable<object, IComparable> stableIds = new ConditionalWeakTable<object, IComparable>();
        private static long nextStableId = 0L;
        private readonly IEnumerable<object> mutexes;

        public LockSet(IEnumerable<object> mutexes)
        {
            this.mutexes = (IEnumerable<object>)Enumerable.ToList<object>((IEnumerable<object>)Enumerable.OrderBy<object, IComparable>(mutexes, (Func<object, IComparable>)(mutex => LockSet.GetStableId(mutex))));
        }

        public void Enter()
        {
            foreach (object obj in this.mutexes)
                Monitor.Enter(obj);
        }

        public void Exit()
        {
            foreach (object obj in this.mutexes)
                Monitor.Exit(obj);
        }

        private static IComparable GetStableId(object mutex)
        {
            lock (LockSet.stableIds)
              return LockSet.stableIds.GetValue(mutex, (ConditionalWeakTable<object, IComparable>.CreateValueCallback)(k =>
              {
                  long num = LockSet.nextStableId;
                  LockSet.nextStableId = num + 1L;
                  return (IComparable)num;
              }));
        }
    }
}