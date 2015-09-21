using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud.Internal
{
    internal class TaskQueue
    {
        private readonly object mutex = new object();
        private Task tail;

        public object Mutex
        {
            get
            {
                return this.mutex;
            }
        }

        public T Enqueue<T>(Func<Task, T> taskStart, CancellationToken cancellationToken) where T : Task
        {
            T obj;
            lock (this.mutex)
            {
                Task local_2 = (Task)((object)this.tail ?? (object)Task.FromResult<bool>(true));
                obj = taskStart(this.GetTaskToAwait(cancellationToken));
                this.tail = Task.WhenAll(local_2, (Task)obj);
            }
            return obj;
        }

        private Task GetTaskToAwait(CancellationToken cancellationToken)
        {
            lock (this.mutex)
              return ((Task)((object)this.tail ?? (object)Task.FromResult<bool>(true))).ContinueWith((Action<Task>)(task => { }), cancellationToken);
        }
    }
}