using AVOSCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AVOSCloud.Internal
{
    internal static class InternalExtensions
    {
        internal static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue defaultValue)
        {
            TValue obj;
            if (self.TryGetValue(key, out obj))
                return obj;
            else
                return defaultValue;
        }

        internal static T GetPartial<T>(this AVObject self, InternalExtensions.PartialAccessor<T> action)
        {
            return InternalExtensions.GetPartial<T>(action);
        }

        internal static T GetPartial<T>(InternalExtensions.PartialAccessor<T> action)
        {
            T obj = default(T);
            action(ref obj);
            return obj;
        }

        internal static Task<TResult> OnSuccess<TIn, TResult>(this Task<TIn> task, Func<Task<TIn>, TResult> continuation)
        {
            return InternalExtensions.OnSuccess<TResult>((Task)task, (Func<Task, TResult>)(t => continuation((Task<TIn>)t)));
        }

        internal static Task OnSuccess<TIn>(this Task<TIn> task, Action<Task<TIn>> continuation)
        {
            return (Task)InternalExtensions.OnSuccess<TIn, object>(task, (Func<Task<TIn>, object>)(t =>
            {
                continuation(t);
                return (object)null;
            }));
        }

        internal static Task<TResult> OnSuccess<TResult>(this Task task, Func<Task, TResult> continuation)
        {
            return TaskExtensions.Unwrap<TResult>(task.ContinueWith<Task<TResult>>((Func<Task, Task<TResult>>)(t =>
            {
                if (!t.IsFaulted)
                {
                    if (!t.IsCanceled)
                        return Task.FromResult<TResult>(continuation(t));
                    TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
                    completionSource.SetCanceled();
                    return completionSource.Task;
                }
                else
                {
                    AggregateException aggregateException = t.Exception.Flatten();
                    if (aggregateException.InnerExceptions.Count != 1)
                        ExceptionDispatchInfo.Capture((Exception)aggregateException).Throw();
                    else
                        ExceptionDispatchInfo.Capture(aggregateException.InnerExceptions[0]).Throw();
                    return Task.FromResult<TResult>(default(TResult));
                }
            })));
        }

        internal static Task OnSuccess(this Task task, Action<Task> continuation)
        {
            return (Task)InternalExtensions.OnSuccess<object>(task, (Func<Task, object>)(t =>
            {
                continuation(t);
                return (object)null;
            }));
        }

        internal static Task<T> PartialAsync<T>(this object self, InternalExtensions.PartialAccessor<Task<T>> partial)
        {
            return InternalExtensions.PartialAsync<T>(partial);
        }

        internal static Task<T> PartialAsync<T>(InternalExtensions.PartialAccessor<Task<T>> partial)
        {
            Task<T> task = (Task<T>)null;
            partial(ref task);
            return InternalExtensions.Safe<T>(task);
        }

        internal static Task PartialAsync(this object self, InternalExtensions.PartialAccessor<Task> partial)
        {
            return InternalExtensions.PartialAsync(partial);
        }

        internal static Task PartialAsync(InternalExtensions.PartialAccessor<Task> partial)
        {
            Task task = (Task)null;
            partial(ref task);
            return InternalExtensions.Safe(task);
        }

        internal static Task<T> Safe<T>(this Task<T> task)
        {
            return task ?? Task.FromResult<T>(default(T));
        }

        internal static Task Safe(this Task task)
        {
            return (Task)((object)task ?? (object)Task.FromResult<object>((object)null));
        }

        internal static Task WhileAsync(Func<Task<bool>> predicate, Func<Task> body)
        {
            return ((Func<Task>)(() => TaskExtensions.Unwrap(InternalExtensions.OnSuccess<bool, Task>(predicate(), (Func<Task<bool>, Task>)(t =>
            {
                if (!t.Result)
                    return (Task)Task.FromResult<int>(0);
                else
                    return TaskExtensions.Unwrap(InternalExtensions.OnSuccess<Task>(body(), (Func<Task, Task>)(_ => (Task)predicate())));
            })))))();
        }

        internal static IList<object> ToListDictionary<T>(this IList<T> list) where T : IToDictionary
        {
            IList<object> list1 = (IList<object>)new List<object>();
            foreach (T obj in (IEnumerable<T>)list)
                list1.Add((object)obj.ToDictionary());
            return list1;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] objArray = new T[length];
            Array.Copy((Array)data, index, (Array)objArray, 0, length);
            return objArray;
        }

        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> Source, Func<TIn, Task<TResult>> executor)
        {
            List<TResult> rtn = new List<TResult>();
            IEnumerator<TIn> enumerator = Source.GetEnumerator();
            if (!enumerator.MoveNext())
                return InternalExtensions.EndForNull<TResult>();
            else
                return InternalExtensions.Chaining<TIn, TResult>(enumerator, executor, rtn);
        }

        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(IEnumerator<TIn> enumerator, Func<TIn, Task<TResult>> executor, List<TResult> rtn)
        {
            if ((object)enumerator.Current == null)
                return InternalExtensions.EndForNull<TResult>();
            TIn current = enumerator.Current;
            if (!enumerator.MoveNext())
                return TaskExtensions.Unwrap<IEnumerable<TResult>>(executor(current).ContinueWith<Task<IEnumerable<TResult>>>((Func<Task<TResult>, Task<IEnumerable<TResult>>>)(t =>
                {
                    rtn.Add(t.Result);
                    TaskCompletionSource<IEnumerable<TResult>> completionSource = new TaskCompletionSource<IEnumerable<TResult>>();
                    completionSource.SetResult((IEnumerable<TResult>)rtn);
                    return completionSource.Task;
                })));
            else
                return TaskExtensions.Unwrap<IEnumerable<TResult>>(executor(current).ContinueWith<Task<IEnumerable<TResult>>>((Func<Task<TResult>, Task<IEnumerable<TResult>>>)(t =>
                {
                    rtn.Add(t.Result);
                    return InternalExtensions.Chaining<TIn, TResult>(enumerator, executor, rtn);
                })));
        }

        public static Task<IEnumerable<TResult>> EndForNull<TResult>()
        {
            TaskCompletionSource<IEnumerable<TResult>> completionSource = new TaskCompletionSource<IEnumerable<TResult>>();
            completionSource.SetResult((IEnumerable<TResult>)null);
            return completionSource.Task;
        }

        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> Source, Func<TIn, Task<TResult>> executor, Func<TResult, TIn, TIn> beforeNext)
        {
            return InternalExtensions.Chaining<TIn, TResult>(Source, executor, beforeNext, default(TResult));
        }

        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> Source, Func<TIn, Task<TResult>> executor, Func<TResult, TIn, TIn> beforeNext, TResult firstSeed)
        {
            return InternalExtensions.Chaining<TIn, TResult>(Source, executor, beforeNext, (Action<TResult>)null, firstSeed);
        }

        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> Source, Func<TIn, Task<TResult>> executor, Func<TResult, TIn, TIn> beforeNext, Action<TResult> afterLast, TResult firstSeed)
        {
            List<TResult> rtn = new List<TResult>();
            IEnumerator<TIn> enumerator = Source.GetEnumerator();
            if (!enumerator.MoveNext())
                return InternalExtensions.EndForNull<TResult>();
            else
                return InternalExtensions.Chaining<TIn, TResult>(enumerator, executor, rtn, beforeNext, afterLast, firstSeed);
        }

        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(IEnumerator<TIn> enumerator, Func<TIn, Task<TResult>> executor, List<TResult> rtn, Func<TResult, TIn, TIn> beforeNext, Action<TResult> afterLast, TResult lastResult)
        {
            if ((object)enumerator.Current == null)
            {
                TaskCompletionSource<IEnumerable<TResult>> completionSource = new TaskCompletionSource<IEnumerable<TResult>>();
                completionSource.SetResult((IEnumerable<TResult>)null);
                return completionSource.Task;
            }
            else
            {
                TIn @in = enumerator.Current;
                if (beforeNext != null)
                    @in = beforeNext(lastResult, @in);
                if (!enumerator.MoveNext())
                    return TaskExtensions.Unwrap<IEnumerable<TResult>>(executor(@in).ContinueWith<Task<IEnumerable<TResult>>>((Func<Task<TResult>, Task<IEnumerable<TResult>>>)(t =>
                    {
                        if (afterLast != null)
                            afterLast(t.Result);
                        TaskCompletionSource<IEnumerable<TResult>> completionSource = new TaskCompletionSource<IEnumerable<TResult>>();
                        completionSource.SetResult((IEnumerable<TResult>)rtn);
                        return completionSource.Task;
                    })));
                else
                    return TaskExtensions.Unwrap<IEnumerable<TResult>>(executor(@in).ContinueWith<Task<IEnumerable<TResult>>>((Func<Task<TResult>, Task<IEnumerable<TResult>>>)(t =>
                    {
                        rtn.Add(t.Result);
                        if (afterLast != null)
                            afterLast(t.Result);
                        return InternalExtensions.Chaining<TIn, TResult>(enumerator, executor, rtn, beforeNext, afterLast, t.Result);
                    })));
            }
        }

        public static LinkedListNode<T> Append<T>(this LinkedList<T> ll, T value)
        {
            if (ll.Count == 0)
                return ll.AddFirst(value);
            else
                return ll.AddAfter(ll.Last, value);
        }

        public static IList<T[]> Cut<T>(this T[] source, int size)
        {
            IList<T[]> list = (IList<T[]>)new List<T[]>();
            int num = (source.Length + size - 1) / size;
            int length1 = source.Length;
            for (int index1 = 0; index1 < num; ++index1)
            {
                int length2 = size;
                if (index1 == num - 1)
                    length2 = length1 - index1 * size;
                T[] objArray = new T[length2];
                for (int index2 = 0; index2 < length2; ++index2)
                    objArray[index2] = source[index1 * size + index2];
                list.Add(objArray);
            }
            return list;
        }

        public static long UnixTimeStamp(this DateTime date)
        {
            return (date.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000L;
        }

        internal delegate void PartialAccessor<T>(ref T arg);
    }
}