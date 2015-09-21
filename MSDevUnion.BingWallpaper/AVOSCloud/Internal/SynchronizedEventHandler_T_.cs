using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud.Internal
{
    internal class SynchronizedEventHandler<T>
    {
        private LinkedList<Tuple<Delegate, TaskFactory>> delegates = new LinkedList<Tuple<Delegate, TaskFactory>>();

        public void Add(Delegate del)
        {
            lock (this.delegates)
            {
                TaskFactory local_0 = SynchronizationContext.Current == null ? Task.Factory : new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.FromCurrentSynchronizationContext());
                foreach (Delegate item_0 in del.GetInvocationList())
                    this.delegates.AddLast(new Tuple<Delegate, TaskFactory>(item_0, local_0));
            }
        }

        public Task Invoke(object sender, T args)
        {
            Task<int>[] taskArray = new Task<int>[1]
            {
        Task.FromResult<int>(0)
            };
            IEnumerable<Tuple<Delegate, TaskFactory>> source;
            lock (this.delegates)
              source = (IEnumerable<Tuple<Delegate, TaskFactory>>)Enumerable.ToList<Tuple<Delegate, TaskFactory>>((IEnumerable<Tuple<Delegate, TaskFactory>>)this.delegates);
            return Task.WhenAll((IEnumerable<Task>)Enumerable.ToList<Task>((IEnumerable<Task>)Enumerable.Select<Tuple<Delegate, TaskFactory>, Task<object>>(source, (Func<Tuple<Delegate, TaskFactory>, Task<object>>)(p => p.Item2.ContinueWhenAll<object>((Task[])taskArray, (Func<Task[], object>)(_ => p.Item1.DynamicInvoke(sender, (object)args)))))));
        }

        public void Remove(Delegate del)
        {
            lock (this.delegates)
            {
                if (this.delegates.Count == 0)
                    return;
                foreach (Delegate item_0 in del.GetInvocationList())
                {
                    for (LinkedListNode<Tuple<Delegate, TaskFactory>> local_3 = this.delegates.First; local_3 != null; local_3 = local_3.Next)
                    {
                        if (!(local_3.Value.Item1 != item_0))
                        {
                            this.delegates.Remove(local_3);
                            break;
                        }
                    }
                }
            }
        }
    }
}