using AVOSCloud.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AVOSCloud
{
    public class AVObject : IEnumerable<KeyValuePair<string, object>>, INotifyPropertyChanged, IAVObjectBase
    {
        private static readonly string AutoClassName;

        private static readonly IDictionary<Tuple<Type, string>, string> propertyFieldNames;

        private static readonly IDictionary<string, Tuple<Func<AVObject>, Type>> objectFactories;

        private static readonly IDictionary<Type, IDictionary<string, string>> propertyMappings;

        private static readonly ReaderWriterLockSlim propertyMappingsLock;

        internal readonly object mutex = new object();

        internal readonly IDictionary<string, object> serverData = new Dictionary<string, object>();

        private readonly LinkedList<IDictionary<string, IAVFieldOperation>> operationSetQueue = new LinkedList<IDictionary<string, IAVFieldOperation>>();

        private readonly IDictionary<string, object> estimatedData = new Dictionary<string, object>();

        private readonly IDictionary<string, bool> dataAvailability = new Dictionary<string, bool>();

        private readonly IDictionary<object, AVJSONCacheItem> hashedObjects = new Dictionary<object, AVJSONCacheItem>();

        private static readonly ThreadLocal<bool> isCreatingPointer;

        private bool hasBeenFetched;

        private bool dirty;

        internal TaskQueue taskQueue = new TaskQueue();

        private bool isNew;

        private DateTime? updatedAt;

        private DateTime? createdAt;

        private string objectId;

        private string className;

        private SynchronizedEventHandler<PropertyChangedEventArgs> propertyChanged = new SynchronizedEventHandler<PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.propertyChanged.Add(value);
            }
            remove
            {
                this.propertyChanged.Remove(value);
            }
        }

        [AVFieldName("ACL")]
        public AVACL ACL
        {
            get
            {
                return this.GetProperty<AVACL>(null, "ACL");
            }
            set
            {
                this.SetProperty<AVACL>(value, "ACL");
            }
        }

        private bool CanBeSerialized
        {
            get
            {
                bool result;
                lock (this.mutex)
                {
                    result = AVObject.CanBeSerializedAsValue(this.estimatedData);
                }
                return result;
            }
        }

        public string ClassName
        {
            get
            {
                string result;
                lock (this.mutex)
                {
                    result = this.className;
                }
                return result;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.className = value;
                    this.OnPropertyChanged("ClassName");
                }
            }
        }

        [AVFieldName("createdAt")]
        public DateTime? CreatedAt
        {
            get
            {
                DateTime? result;
                lock (this.mutex)
                {
                    result = this.createdAt;
                }
                return result;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.createdAt = value;
                    this.OnPropertyChanged("CreatedAt");
                }
            }
        }

        internal IDictionary<string, IAVFieldOperation> CurrentOperations
        {
            get
            {
                return this.operationSetQueue.Last.Value;
            }
        }

        private bool HasDirtyChildren
        {
            get
            {
                bool result;
                lock (this.mutex)
                {
                    result = (Enumerable.FirstOrDefault<AVObject>(this.FindUnsavedChildren()) != null);
                }
                return result;
            }
        }

        public bool IsDataAvailable
        {
            get
            {
                bool result;
                lock (this.mutex)
                {
                    result = this.hasBeenFetched;
                }
                return result;
            }
        }

        public bool IsDirty
        {
            get
            {
                bool result;
                lock (this.mutex)
                {
                    result = this.CheckIsDirty(true);
                }
                return result;
            }
            internal set
            {
                lock (this.mutex)
                {
                    this.dirty = value;
                    this.OnPropertyChanged("IsDirty");
                }
            }
        }

        public bool IsNew
        {
            get
            {
                bool result;
                lock (this.mutex)
                {
                    result = this.isNew;
                }
                return result;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.isNew = value;
                    this.OnPropertyChanged("IsNew");
                }
            }
        }

        public virtual object this[string key]
        {
            get
            {
                object result;
                lock (this.mutex)
                {
                    this.CheckGetAccess(key);
                    if (estimatedData.ContainsKey(key))
                    {
                        object obj2 = this.estimatedData[key];
                        AVRelationBase aVRelationBase = obj2 as AVRelationBase;
                        if (aVRelationBase != null)
                        {
                            aVRelationBase.EnsureParentAndKey(this, key);
                        }
                        result = obj2;
                    }
                    else
                    {
                        return null;
                    }
                   
                }
                return result;
            }
            set
            {
                lock (this.mutex)
                {
                    this.OnSettingValue(ref key, ref value);
                    if (!AVClient.IsValidType(value))
                    {
                        throw new ArgumentException("Invalid type for value: " + value.GetType().ToString());
                    }
                    this.PerformOperation(key, new AVSetOperation(value));
                    this.CheckpointMutableContainer(value);
                }
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                ICollection<string> keys;
                lock (this.mutex)
                {
                    keys = this.estimatedData.Keys;
                }
                return keys;
            }
        }

        [AVFieldName("objectId")]
        public string ObjectId
        {
            get
            {
                string result;
                lock (this.mutex)
                {
                    result = this.objectId;
                }
                return result;
            }
            set
            {
                lock (this.mutex)
                {
                    this.IsDirty = true;
                    this.SetObjectIdInternal(value);
                }
            }
        }

        internal IDictionary<string, string> PropertyMappings
        {
            get
            {
                AVObject.propertyMappingsLock.EnterReadLock();
                System.Collections.Generic.IDictionary<string, string> result;
                try
                {
                    System.Collections.Generic.IDictionary<string, string> dictionary;
                    if (AVObject.propertyMappings.TryGetValue(base.GetType(), out dictionary))
                    {
                        result = dictionary;
                        return result;
                    }
                }
                finally
                {
                    AVObject.propertyMappingsLock.ExitReadLock();
                }
                AVObject.propertyMappingsLock.EnterUpgradeableReadLock();
                try
                {
                    System.Collections.Generic.IDictionary<string, string> dictionary;
                    if (!AVObject.propertyMappings.TryGetValue(base.GetType(), out dictionary))
                    {
                        dictionary = new System.Collections.Generic.Dictionary<string, string>();
                        PropertyInfo[] properties = (PropertyInfo[]) base.GetType().GetProperties();
                        foreach (PropertyInfo t in properties)
                        {
                            AVFieldNameAttribute customAttribute = t.GetCustomAttribute<AVFieldNameAttribute>(true); 
                            if (customAttribute != null)
                            {
                                dictionary[customAttribute.FieldName] = t.Name;
                            } 
                        }
                        AVObject.propertyMappingsLock.EnterWriteLock();
                        try
                        {
                            AVObject.propertyMappings[base.GetType()] = dictionary;
                        }
                        finally
                        {
                            AVObject.propertyMappingsLock.ExitWriteLock();
                        }
                    }
                    result = dictionary;
                }
                finally
                {
                    AVObject.propertyMappingsLock.ExitUpgradeableReadLock();
                }
                return result;
            }
        }

        [AVFieldName("updatedAt")]
        public DateTime? UpdatedAt
        {
            get
            {
                DateTime? result;
                lock (this.mutex)
                {
                    result = this.updatedAt;
                }
                return result;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.updatedAt = value;
                    this.OnPropertyChanged("UpdatedAt");
                }
            }
        }

        static AVObject()
        {
            AVObject.AutoClassName = "_Automatic";
            AVObject.propertyFieldNames = new Dictionary<Tuple<Type, string>, string>();
            AVObject.objectFactories = new Dictionary<string, Tuple<Func<AVObject>, Type>>();
            AVObject.propertyMappings = new Dictionary<Type, IDictionary<string, string>>();
            AVObject.propertyMappingsLock = new ReaderWriterLockSlim();
            AVObject.isCreatingPointer = new ThreadLocal<bool>(() => false);
        }

        protected AVObject() : this(AVObject.AutoClassName)
        {
        }

        public AVObject(string className)
        {
            bool value = AVObject.isCreatingPointer.Value;
            AVObject.isCreatingPointer.Value=false;
            if (className == null)
            {
                throw new ArgumentException("You must specify a AV class name when creating a new AVObject.");
            }
            if (AVObject.AutoClassName.Equals(className))
            {
                className = AVObject.GetClassName(base.GetType());
            }
            if (base.GetType().Equals(typeof(AVObject)) && AVObject.objectFactories.ContainsKey(className))
            {
                throw new ArgumentException("You must create this type of AVObject using AVObject.Create() or the proper subclass.");
            }
            this.ClassName = className;
            this.operationSetQueue.AddLast(new Dictionary<string, IAVFieldOperation>());
            if (value)
            {
                this.IsDirty = false;
                this.hasBeenFetched = false;
                return;
            }
            this.hasBeenFetched = true;
            this.IsDirty = true;
            this.SetDefaultValues();
        }

        public void Add(string key, object value)
        {
            lock (this.mutex)
            {
                if (this.ContainsKey(key))
                {
                    throw new ArgumentException("Key already exists", key);
                }
                this[key] = value;
            }
        }

        public void AddRangeToList<T>(string key, IEnumerable<T> values)
        {
            this.PerformOperation(key, new AVAddOperation(Enumerable.Cast<object>(values)));
        }

        public void AddRangeUniqueToList<T>(string key, IEnumerable<T> values)
        {
            this.PerformOperation(key, new AVAddUniqueOperation(Enumerable.Cast<object>(values)));
        }

        internal void AddToHashedObjects(object obj)
        {
            lock (this.mutex)
            {
                this.hashedObjects[obj]= new AVJSONCacheItem(obj);
            }
        }

        public void AddToList(string key, object value)
        {
            this.AddRangeToList<object>(key, new object[]
            {
                value
            });
        }

        public void AddUniqueToList(string key, object value)
        {
            this.AddRangeUniqueToList<object>(key, new object[]
            {
                value
            });
        }

        private void ApplyOperations(IDictionary<string, IAVFieldOperation> operations, IDictionary<string, object> map)
        {
            lock (this.mutex)
            {
                foreach (var o in operations)
                {
                    object oldValue;
                    map.TryGetValue(o.Key, out oldValue);
                    object obj2 = o.Value.Apply(oldValue, this, o.Key);
                    if (obj2 == AVDeleteOperation.DeleteToken)
                    {
                        map.Remove(o.Key);
                    }
                    else
                    {
                        map[o.Key] = obj2;
                    }
                }
            }
        }

        private static bool CanBeSerializedAsValue(object value)
        {
            return AVClient.DeepTraversal(value, false, true).OfType<AVObject>().All(o => o.ObjectId != null);
        }

        private void CheckForChangesToMutableContainer(string key, object obj)
        {
            lock (this.mutex)
            {
                if (AVClient.IsContainerObject(obj))
                {
                    AVJSONCacheItem aVJSONCacheItem;
                    this.hashedObjects.TryGetValue(obj, out aVJSONCacheItem);
                    if (aVJSONCacheItem == null)
                    {
                        throw new ArgumentException("AVObjects contains container item that isn't cached.");
                    }
                    if (!aVJSONCacheItem.Equals(new AVJSONCacheItem(obj)))
                    {
                        this.PerformOperation(key, new AVSetOperation(obj));
                    }
                }
                else if (obj != null)
                {
                    this.hashedObjects.Remove(obj);
                }
            }
        }

        internal void CheckForChangesToMutableContainers()
        {
            lock (this.mutex)
            {
                foreach (var e in estimatedData)
                {
                    CheckForChangesToMutableContainer(e.Key, e.Value);
                }
                List<object> list = this.hashedObjects.Keys.Except(this.estimatedData.Values).ToList();
                object[] array = list.ToArray<object>();
                object[] array2 = array;
                foreach (object obj2 in array2)
                {
                    this.hashedObjects.Remove(obj2);
                }
            }
        }

        private void CheckGetAccess(string key)
        {
            lock (this.mutex)
            {
                if (!this.CheckIsDataAvailable(key))
                {
                    throw new InvalidOperationException("AVObject has no data for this key.  Call FetchIfNeededAsync() to get the data.");
                }
            }
        }

        private bool CheckIsDataAvailable(string key)
        {
            bool result;
            lock (this.mutex)
            {
                bool flag2 = this.IsDataAvailable || (this.dataAvailability.ContainsKey(key) && this.dataAvailability[key]);
                result = flag2;
            }
            return result;
        }

        private bool CheckIsDirty(bool considerChildren)
        {
            this.CheckForChangesToMutableContainers();
            return this.dirty || this.CurrentOperations.Count > 0 || (considerChildren && this.HasDirtyChildren);
        }

        private void CheckpointMutableContainer(object obj)
        {
            lock (this.mutex)
            {
                if (AVClient.IsContainerObject(obj))
                {
                    this.hashedObjects[obj] = new AVJSONCacheItem(obj);
                }
            }
        }

        private static void CollectDirtyChildren(object node, IList<AVObject> dirtyChildren, ICollection<AVObject> seen, ICollection<AVObject> seenNew)
        {
            foreach (var vo in AVClient.DeepTraversal(node, false, false).OfType<AVObject>())
            {
                ICollection<AVObject> collection;
                if (vo.ObjectId != null)
                {
                    collection = new HashSet<AVObject>(new IdentityEqualityComparer<AVObject>());
                }
                else
                {
                    if (seenNew.Contains(vo))
                    {
                        throw new InvalidOperationException("Found a circular dependency while saving");
                    }
                    collection = new HashSet<AVObject>(seenNew, new IdentityEqualityComparer<AVObject>());
                    collection.Add(vo);
                }
                if (seen.Contains(vo))
                {
                    break;
                }
                seen.Add(vo);
                AVObject.CollectDirtyChildren(vo.estimatedData, dirtyChildren, seen, collection);
                if (vo.CheckIsDirty(false))
                {
                    dirtyChildren.Add(vo);
                }
            }
        }

        private static void CollectDirtyChildren(object node, IList<AVObject> dirtyChildren)
        {
            HashSet<AVObject> seen = new HashSet<AVObject>(new IdentityEqualityComparer<AVObject>());
            HashSet<AVObject> seenNew = new HashSet<AVObject>(new IdentityEqualityComparer<AVObject>());
            AVObject.CollectDirtyChildren(node, dirtyChildren, seen, seenNew);
        }

        public bool ContainsKey(string key)
        {
            bool result;
            lock (this.mutex)
            {
                result = this.estimatedData.ContainsKey(key);
            }
            return result;
        }

        public static AVObject Create(string className)
        {
            return AVObject.GetFactory(className).Invoke();
        }

        public static T Create<T>() where T : AVObject
        {
            return (T)((object)AVObject.Create(AVObject.GetClassName(typeof(T))));
        }

        public static AVObject CreateWithoutData(string className, string objectId)
        {
            AVObject.isCreatingPointer.Value=(true);
            AVObject result;
            try
            {
                AVObject aVObject = AVObject.GetFactory(className).Invoke();
                aVObject.ObjectId = objectId;
                aVObject.IsDirty = false;
                if (aVObject.IsDirty)
                {
                    throw new InvalidOperationException("A AVObject subclass default constructor must not make changes to the object that cause it to be dirty.");
                }
                result = aVObject;
            }
            finally
            {
                AVObject.isCreatingPointer.Value=(false);
            }
            return result;
        }

        public static T CreateWithoutData<T>(string objectId) where T : AVObject
        {
            return (T)((object)AVObject.CreateWithoutData(AVObject.GetClassName(typeof(T)), objectId));
        }

        private static Task DeepSaveAsync(object obj, string sessionToken, CancellationToken cancellationToken)
        {
            List<AVObject> aVObjects7 = new List<AVObject>();
            AVObject.CollectDirtyChildren(obj, aVObjects7);
            HashSet<AVObject> aVObjects8 = new HashSet<AVObject>(aVObjects7, new IdentityEqualityComparer<AVObject>());
            return Task.WhenAll((
                from f in AVClient.DeepTraversal(obj, true, false).OfType<AVFile>()
                select f into f
                select f.SaveAsync(cancellationToken)).ToList<Task>()).OnSuccess<Task>((Task _) => {
                    Func<AVObject, AVObject> func3 = null;
                    Func<AVObject, AVObject> func4 = null;
                    IEnumerable<AVObject> aVObjects6 = new List<AVObject>(aVObjects8);
                    return InternalExtensions.WhileAsync(() => Task.FromResult<bool>(aVObjects6.Any<AVObject>()), () => {
                        Func<AVObject, IDictionary<string, IAVFieldOperation>> func2 = null;
                        Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, object> result = null;
                        //AVObject.<> c__DisplayClass2f cSu0024u003cu003e8_locals302 = this;
                        IEnumerable<AVObject> aVObjects2 = aVObjects6;
                        if (func3 == null)
                        {
                            func3 = (AVObject item) => item;
                        }
                        List<AVObject> aVObjects3 = aVObjects2.Select<AVObject, AVObject>(func3).ToList<AVObject>();
                        IEnumerable<AVObject> aVObjects4 = aVObjects6;
                        if (func4 == null)
                        {
                            func4 = (AVObject item) => item;
                        }
                        List<AVObject> aVObjects5 = aVObjects4.Select<AVObject, AVObject>(func4).ToList<AVObject>();
                        if (aVObjects3.Count<AVObject>() > 20)
                        {
                            aVObjects5.InsertRange(0, aVObjects3.GetRange(20, aVObjects3.Count<AVObject>() - 20));
                            aVObjects3.RemoveRange(20, aVObjects3.Count - 20);
                        }
                        aVObjects6 = aVObjects5;
                        if (aVObjects3.Count<AVObject>() == 0)
                        {
                            throw new InvalidOperationException("Unable to save a AVObject with a relation to a cycle.");
                        }
                        return AVObject.EnqueueForAll<object>(aVObjects3, (Task toAwait) => {
                            Func<AVObject, IDictionary<string, IAVFieldOperation>, Dictionary<string, object>> func1 = null;
                            List<AVObject> aVObjects1 = aVObjects3;
                            if (func2 == null)
                            {
                                func2 = (AVObject item) => item.StartSave();
                            }
                            List<IDictionary<string, IAVFieldOperation>> list = aVObjects1.Select(func2).ToList();
                            Task<Tuple<HttpStatusCode, IDictionary<string, object>>> task = toAwait.OnSuccess((Task __) => {
                                if (func1 == null)
                                {
                                    func1 = (item, ops) => new Dictionary<string, object>()
                                {
                                    { "method", (item.ObjectId == null ? "POST" : "PUT") },
                                    { "path", (item.ObjectId == null ? string.Format(string.Concat(AVClient.APIVersionString, "/classes/{0}"), new object[] { Uri.EscapeDataString(item.ClassName) }) : string.Format("/classes/{0}/{1}", new object[] { Uri.EscapeDataString(item.ClassName), Uri.EscapeDataString(item.ObjectId) })) },
                                    { "body", item.ToJSONObjectForSaving(ops) }
                                };
                                }
                                Func<AVObject, IDictionary<string, IAVFieldOperation>, Dictionary<string, object>> cSu0024u003cu003e9_CachedAnonymousMethodDelegate3e = func1;
                                Dictionary<string, object> strs = new Dictionary<string, object>()
                            {
                                { "requests", aVObjects3.Zip(list, cSu0024u003cu003e9_CachedAnonymousMethodDelegate3e).Cast<object>().ToList<object>() }
                            };
                                return AVClient.RequestAsync("POST", "/batch", sessionToken, strs, cancellationToken);
                            }).Unwrap().ContinueWith<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>>(requestTask => {
                                if (requestTask.IsCanceled || requestTask.IsFaulted)
                                {
                                    List<AVObject> aVObjects = aVObjects3;
                                    List<IDictionary<string, IAVFieldOperation>> dictionaries = list;
                                    foreach (var variable in aVObjects.Zip(dictionaries, (AVObject item, IDictionary<string, IAVFieldOperation> ops) => new { item = item, ops = ops }))
                                    {
                                        variable.item.MergeAfterFailedSave(variable.ops);
                                    }
                                    return requestTask;
                                }
                                List<object> objs = (List<object>)requestTask.Result.Item2["results"];
                                var collection =
                                    from i in Enumerable.Range(0, aVObjects3.Count)
                                    select new { Obj = aVObjects3[i], Result = (Dictionary<string, object>)objs.ElementAtOrDefault<object>(i), Operation = list[i] };
                                AVException aVException = null;
                                foreach (var variable1 in collection)
                                {
                                    if (variable1.Result == null || !variable1.Result.ContainsKey("success"))
                                    {
                                        variable1.Obj.MergeAfterFailedSave(variable1.Operation);
                                        if (aVException != null)
                                        {
                                            continue;
                                        }
                                        Dictionary<string, object> strs = (Dictionary<string, object>)variable1.Result["error"];
                                        int num = (strs.ContainsKey("code") ? (int)((long)strs["code"]) : -1);
                                        string str = strs["error"] as string;
                                        aVException = new AVException((AVException.ErrorCode)num, str, null);
                                    }
                                    else
                                    {
                                        variable1.Obj.MergeAfterSave((Dictionary<string, object>)variable1.Result["success"]);
                                    }
                                }
                                if (aVException != null)
                                {
                                    throw aVException;
                                }
                                return requestTask;
                            }).Unwrap();
                            if (result == null)
                            {
                                result = t => t.Result;
                            }
                            return task.OnSuccess(result);
                        }, cancellationToken);
                    });
                }).Unwrap();
        }

        internal Task DeleteAsync(Task toAwait, CancellationToken cancellationToken)
        {
            Task result;
            lock (this.mutex)
            {
                if (this.ObjectId != null)
                {
                    string currentSessionToken = AVUser.CurrentSessionToken;
                    return TaskExtensions.Unwrap<Tuple<HttpStatusCode, IDictionary<string, object>>>(toAwait.OnSuccess(delegate (Task _)
                    {
                        Task<Tuple<HttpStatusCode, IDictionary<string, object>>> result2;
                        lock (this.mutex)
                        {
                            result2 = AVClient.RequestAsync("DELETE", string.Format("/classes/{0}/{1}", new object[]
                            {
                                this.ClassName,
                                this.ObjectId
                            }), currentSessionToken, null, cancellationToken);
                        }
                        return result2;
                    })).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> _) => true);
                }
                result = Task.FromResult<int>(0);
            }
            return result;
        }

        public Task DeleteAsync()
        {
            return this.DeleteAsync(CancellationToken.None);
        }

        public Task DeleteAsync(CancellationToken cancellationToken)
        {
            return this.taskQueue.Enqueue<Task>((Task toAwait) => this.DeleteAsync(toAwait, cancellationToken), cancellationToken);
        }

        private static Task<T> EnqueueForAll<T>(IEnumerable<AVObject> objects, Func<Task, Task<T>> taskStart, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            LockSet lockSet = new LockSet(Enumerable.Select<AVObject, object>(objects, (AVObject o) => o.taskQueue.Mutex));
            lockSet.Enter();
            Task<T> task3;
            try
            {
                Task<T> task2 = taskStart.Invoke(taskCompletionSource.Task);
                List<Task> tasks = new List<Task>();
                using (IEnumerator<AVObject> enumerator = objects.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        AVObject current = enumerator.Current;
                        current.taskQueue.Enqueue<Task<T>>(delegate (Task task)
                        {
                            tasks.Add(task);
                            return task2;
                        }, cancellationToken);
                    }
                }
                Task.WhenAll(tasks.ToArray()).ContinueWith(delegate (Task task)
                {
                    taskCompletionSource.SetResult(null);
                });
                task3 = task2;
            }
            finally
            {
                lockSet.Exit();
            }
            return task3;
        }

        public static Task<IEnumerable<T>> FetchAllAsync<T>(IEnumerable<T> objects) where T : AVObject
        {
            return AVObject.FetchAllAsync<T>(objects, CancellationToken.None);
        }

        public static Task<IEnumerable<T>> FetchAllAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : AVObject
        {
            return AVObject.EnqueueForAll<IEnumerable<T>>(Enumerable.Cast<AVObject>(objects), (Task toAwait) => AVObject.FetchAllInternalAsync<T>(objects, true, toAwait, cancellationToken), cancellationToken);
        }

        public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(IEnumerable<T> objects) where T : AVObject
        {
            return AVObject.FetchAllIfNeededAsync<T>(objects, CancellationToken.None);
        }

        public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : AVObject
        {
            return AVObject.EnqueueForAll<IEnumerable<T>>(Enumerable.Cast<AVObject>(objects), (Task toAwait) => AVObject.FetchAllInternalAsync<T>(objects, false, toAwait, cancellationToken), cancellationToken);
        }

        private static Task<IEnumerable<T>> FetchAllInternalAsync<T>(IEnumerable<T> objects, bool force, Task toAwait, CancellationToken cancellationToken) where T : AVObject
        {
            return null;
        }

        internal virtual Task<AVObject> FetchAsyncInternal(Task toAwait, CancellationToken cancellationToken)
        {
            string currentSessionToken = AVUser.CurrentSessionToken;
            return TaskExtensions.Unwrap<Tuple<HttpStatusCode, IDictionary<string, object>>>(toAwait.OnSuccess(delegate (Task _)
            {
                if (this.ObjectId == null)
                {
                    throw new InvalidOperationException("Cannot outresh an object that hasn't been saved to the server.");
                }
                return AVClient.RequestAsync("GET", string.Format("/classes/{0}/{1}", new object[]
                {
                    Uri.EscapeDataString(this.ClassName),
                    Uri.EscapeDataString(this.ObjectId)
                }), currentSessionToken, null, cancellationToken);
            })).OnSuccess(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                this.MergeAfterFetch(t.Result.Item2);
                return this;
            });
        }

        internal Task<AVObject> FetchAsyncInternal(CancellationToken cancellationToken)
        {
            return this.taskQueue.Enqueue<Task<AVObject>>((Task toAwait) => this.FetchAsyncInternal(toAwait, cancellationToken), cancellationToken);
        }

        internal Task<AVObject> FetchIfNeededAsyncInternal(Task toAwait, CancellationToken cancellationToken)
        {
            if (this.IsDataAvailable)
            {
                return Task.FromResult<AVObject>(this);
            }
            return this.FetchAsyncInternal(toAwait, cancellationToken);
        }

        internal Task<AVObject> FetchIfNeededAsyncInternal(CancellationToken cancellationToken)
        {
            return this.taskQueue.Enqueue<Task<AVObject>>((Task toAwait) => this.FetchIfNeededAsyncInternal(toAwait, cancellationToken), cancellationToken);
        }

        private IEnumerable<AVObject> FindUnsavedChildren()
        {
            return AVClient.DeepTraversal(this.estimatedData, false, false).OfType<AVObject>().Where<AVObject>(o => o.IsDirty);
        }

        public T Get<T>(string key)
        {
            if (this[key] == null)
                return default(T);
            return (T) AVClient.ConvertTo<T>(this[key]);
        }

        /// <summary>
        /// use json.net
        /// </summary>
        /// <typeparam name="T">ClassType</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetConverter<T>(string key)
        {
            var o = ((AVObject)this[key]).ToString();
            return JsonConvert.DeserializeObject<T>(o);
        }
        internal static string GetClassName(Type t)
        {
            AVClassNameAttribute customAttribute = t.GetTypeInfo().GetCustomAttribute<AVClassNameAttribute>();
            if (customAttribute == null)
            {
                throw new ArgumentException("No AVClassName attribute specified on the given subclass.");
            }
            return customAttribute.ClassName;
        }

        private static Func<AVObject> GetFactory(string className)
        {
            Tuple<Func<AVObject>, Type> tuple;
            if (AVObject.objectFactories.TryGetValue(className, out tuple))
            {
                return tuple.Item1;
            }
            return () => new AVObject(className);
        }

        private static string GetFieldForPropertyName(Type type, string propertyName)
        {
            Tuple<Type, string> tuple = new Tuple<Type, string>(type, propertyName);
            string result;
            if (AVObject.propertyFieldNames.TryGetValue(tuple, out result))
            {
                return result;
            }
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentException(propertyName + " property does not exist on type " + type);
            }
            AVFieldNameAttribute customAttribute = CustomAttributeExtensions.GetCustomAttribute<AVFieldNameAttribute>(property);
            if (customAttribute == null)
            {
                throw new ArgumentException(propertyName + " does not have a AVFieldName attribute specified.");
            }
            IDictionary<Tuple<Type, string>, string> dictionary = AVObject.propertyFieldNames;
            string fieldName = customAttribute.FieldName;
            result = fieldName;
            dictionary[tuple]= fieldName;
            return result;
        }

        public static AVQuery<AVObject> GetQuery(string className)
        {
            if (AVObject.GetType(className) != typeof(AVObject))
            {
                throw new ArgumentException("Use the class-specific query properties for class " + className, "className");
            }
            return new AVQuery<AVObject>(className);
        }

        public AVRelation<T> GetRelation<T>(string key) where T : AVObject
        {
            AVRelation<T> aVRelation = null;
            this.TryGetValue(key, out aVRelation);
            return aVRelation ?? new AVRelation<T>(this, key);
        }

        internal static Type GetType(string className)
        {
            Tuple<Func<AVObject>, Type> tuple;
            if (!AVObject.objectFactories.TryGetValue(className, out tuple))
            {
                return typeof(AVObject);
            }
            return tuple.Item2;
        }

        public bool HasSameId(AVObject other)
        {
            bool result;
            lock (this.mutex)
            {
                result = (other != null && object.Equals(this.ClassName, other.ClassName) && object.Equals(this.ObjectId, other.ObjectId));
            }
            return result;
        }

        public void Increment(string key)
        {
            this.Increment(key, 1L);
        }

        public void Increment(string key, long amount)
        {
            this.PerformOperation(key, new AVIncrementOperation(amount));
        }

        public void Increment(string key, double amount)
        {
            this.PerformOperation(key, new AVIncrementOperation(amount));
        }

        public bool IsKeyDirty(string key)
        {
            bool result;
            lock (this.mutex)
            {
                result = this.CurrentOperations.ContainsKey(key);
            }
            return result;
        }

        internal void MergeAfterFailedSave(IDictionary<string, IAVFieldOperation> operationsBeforeSave)
        {
            lock (this.mutex)
            {
                LinkedListNode<IDictionary<string, IAVFieldOperation>> linkedListNode = this.operationSetQueue.Find(operationsBeforeSave);
                IDictionary<string, IAVFieldOperation> value = linkedListNode.Next.Value;
                bool flag2 = value.Count > 0;
                this.operationSetQueue.Remove(linkedListNode);
                using (IEnumerator<KeyValuePair<string, IAVFieldOperation>> enumerator = operationsBeforeSave.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, IAVFieldOperation> current = enumerator.Current;
                        IAVFieldOperation value2 = current.Value;
                        IAVFieldOperation iAVFieldOperation = null;
                        value.TryGetValue(current.Key, out iAVFieldOperation);
                        iAVFieldOperation = ((iAVFieldOperation == null) ? value2 : iAVFieldOperation.MergeWithPrevious(value2));
                        value[current.Key] = iAVFieldOperation;
                    }
                }
                if (!flag2 && value == this.CurrentOperations && operationsBeforeSave.Count > 0)
                {
                    this.OnPropertyChanged("IsDirty");
                }
            }
        }

        internal virtual void MergeAfterFetch(IDictionary<string, object> result)
        {
            lock (this.mutex)
            {
                this.MergeFromServer(result);
                this.RebuildEstimatedData();
            }
        }

        internal virtual void MergeAfterSave(IDictionary<string, object> result)
        {
            lock (this.mutex)
            {
                IDictionary<string, IAVFieldOperation> value = this.operationSetQueue.First.Value;
                this.operationSetQueue.RemoveFirst();
                this.ApplyOperations(value, this.serverData);
                this.MergeFromServer(result);
                this.RebuildEstimatedData();
            }
        }

        internal void MergeFromServer(IDictionary<string, object> data)
        {
            lock (this.mutex)
            {
                this.IsDirty = false;
                this.MergeMagicFields(data);
                using (IEnumerator<KeyValuePair<string, object>> enumerator = data.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, object> current = enumerator.Current;
                        if (!(current.Key == "__type") && !(current.Key == "className"))
                        {
                            object obj2 = current.Value;
                            object obj3 = AVClient.Decode(obj2);
                            if (AVClient.IsContainerObject(obj2))
                            {
                                IDictionary<string, object> dictionary = obj3 as IDictionary<string, object>;
                                if (dictionary != null)
                                {
                                    object obj4;
                                    dictionary.TryGetValue("__type", out obj4);
                                    if ((string)obj4 == "Relation")
                                    {
                                        string targetClassName = dictionary["className"] as string;
                                        obj2 = AVRelationBase.CreateRelation(this, current.Key, targetClassName);
                                    }
                                }
                                this.AddToHashedObjects(obj3);
                            }
                            this.serverData[current.Key]= obj3;
                        }
                    }
                }
                if (!this.UpdatedAt.HasValue && this.CreatedAt.HasValue)
                {
                    this.UpdatedAt = this.CreatedAt;
                }
                this.IsDirty = false;
                this.RebuildEstimatedData();
            }
        }

        internal virtual void MergeMagicFields(IDictionary<string, object> data)
        {
            lock (this.mutex)
            {
                if (data.ContainsKey("objectId"))
                {
                    this.SetObjectIdInternal(data["objectId"] as string);
                    this.hasBeenFetched = true;
                    this.OnPropertyChanged("IsDataAvailable");
                    data.Remove("objectId");
                }
                if (data.ContainsKey("createdAt"))
                {
                    this.CreatedAt = new DateTime?(AVClient.ParseDate(data["createdAt"] as string));
                    data.Remove("createdAt");
                }
                if (data.ContainsKey("updatedAt"))
                {
                    this.UpdatedAt = new DateTime?(AVClient.ParseDate(data["updatedAt"] as string));
                    data.Remove("updatedAt");
                }
                if (data.ContainsKey("ACL"))
                {
                    AVACL aVACL = new AVACL(data["ACL"] as IDictionary<string, object>);
                    this.serverData["ACL"]= aVACL;
                    this.AddToHashedObjects(aVACL);
                    data.Remove("ACL");
                }
            }
        }

        protected void OnFieldsChanged(IEnumerable<string> fieldNames)
        {
            IDictionary<string, string> dictionary = this.PropertyMappings;
            object obj = fieldNames;
            if (obj == null)
            {
                obj = dictionary.Keys;
            }
            fieldNames = (IEnumerable<string>)obj;
            foreach (var f in fieldNames)
            {
                string propertyName;
                if (dictionary.TryGetValue(f, out propertyName))
                {
                    this.OnPropertyChanged(propertyName);
                }
            }
            this.OnPropertyChanged("Item[]");
        }

        internal virtual void OnSettingValue(ref string key, ref object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
        }


        internal void PerformOperation(string key, IAVFieldOperation operation)
        {
            lock (this.mutex)
            {
                object oldValue;
                this.estimatedData.TryGetValue(key, out oldValue);
                object obj2 = operation.Apply(oldValue, this, key);
                if (obj2 == AVDeleteOperation.DeleteToken)
                {
                    this.estimatedData.Remove(key);
                }
                else
                {
                    this.estimatedData[key]= obj2;
                }
                bool flag2 = this.CurrentOperations.Count > 0;
                IAVFieldOperation previous;
                this.CurrentOperations.TryGetValue(key, out previous);
                IAVFieldOperation iAVFieldOperation = operation.MergeWithPrevious(previous);
                this.CurrentOperations[key]=iAVFieldOperation;
                if (!flag2)
                {
                    this.OnPropertyChanged("IsDirty");
                }
                this.CheckpointMutableContainer(obj2);
                this.dataAvailability[key]= true;
                this.OnFieldsChanged(new string[]
                {
                    key
                });
            }
        }

        internal void RebuildEstimatedData()
        {
            lock (this.mutex)
            {
                this.estimatedData.Clear();
                foreach (var sd in serverData)
                {
                    estimatedData.Add(sd);
                }
                foreach (var os in operationSetQueue)
                {
                    ApplyOperations(os,estimatedData);
                }
                this.hashedObjects.Clear();
                foreach (var ed in estimatedData)
                {
                    CheckpointMutableContainer(ed.Value);
                }
                this.OnFieldsChanged(null);
            }
        }

        public static void RegisterSubclass<T>() where T : AVObject, new()
        {
            string text = AVObject.GetClassName(typeof(T));
            if (text == null)
            {
                throw new ArgumentException("No AVClassName attribute defined for " + typeof(T));
            }
            Tuple<Func<AVObject>, Type> tuple;
            if (AVObject.objectFactories.TryGetValue(text, out tuple))
            {
                if (typeof(T).GetTypeInfo().IsAssignableFrom(tuple.Item2.GetTypeInfo()))
                {
                    return;
                }
                if (text.Equals(AVObject.GetClassName(typeof(AVUser))))
                {
                    AVUser.ClearInMemoryUser();
                }
                else if (text.Equals("_Installation"))
                {
                    AVClient.ClearInMemoryInstallation();
                }
            }
            AVObject.objectFactories[text]= new Tuple<Func<AVObject>, Type>(Activator.CreateInstance<T>, typeof(T));
        }

        public virtual void Remove(string key)
        {
            lock (this.mutex)
            {
                this.PerformOperation(key, AVDeleteOperation.Instance);
            }
        }

        public void RemoveAllFromList<T>(string key, IEnumerable<T> values)
        {
            this.PerformOperation(key, new AVRemoveOperation(Enumerable.Cast<object>(values)));
        }

        public void Revert()
        {
            lock (this.mutex)
            {
                if (this.CurrentOperations.Count > 0)
                {
                    this.CurrentOperations.Clear();
                    this.RebuildEstimatedData();
                    this.OnPropertyChanged("IsDirty");
                }
            }
        }

        public static Task SaveAllAsync<T>(IEnumerable<T> objects) where T : AVObject
        {
            return AVObject.SaveAllAsync<T>(objects, CancellationToken.None);
        }

        public static Task SaveAllAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : AVObject
        {
            return AVObject.DeepSaveAsync(Enumerable.ToList<T>(objects), AVUser.CurrentSessionToken, cancellationToken);
        }

        internal virtual Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
        {
            IDictionary<string, object> jSONObjectForSaving = null;
            Tuple<HttpStatusCode, IDictionary<string, object>> tuple1 = null;
            IDictionary<string, IAVFieldOperation> strs = null;
            if (!this.IsDirty)
            {
                return Task.FromResult<int>(0);
            }
            Task task;
            lock (this.mutex)
            {
                string currentSessionToken = AVUser.CurrentSessionToken;
                strs = this.StartSave();
                task = AVObject.DeepSaveAsync(this.estimatedData, currentSessionToken, cancellationToken);
            }
            return TaskExtensions.Unwrap<Tuple<HttpStatusCode, IDictionary<string, object>>>(TaskExtensions.Unwrap<Tuple<HttpStatusCode, IDictionary<string, object>>>(TaskExtensions.Unwrap(task.OnSuccess((Task t) => toAwait)).OnSuccess(delegate (Task t)
            {
                Task<Tuple<HttpStatusCode, IDictionary<string, object>>> result;
                lock (this.mutex)
                {
                    string currentSessionToken2 = AVUser.CurrentSessionToken;
                    jSONObjectForSaving = this.ToJSONObjectForSaving(strs);
                    result = ((this.ObjectId == null) ? AVClient.RequestAsync("POST", string.Format("/classes/{0}", new object[]
                    {
                        Uri.EscapeDataString(this.ClassName)
                    }), currentSessionToken2, jSONObjectForSaving, cancellationToken) : ((jSONObjectForSaving.Count != 0) ? AVClient.RequestAsync("PUT", string.Format("/classes/{0}/{1}", new object[]
                    {
                        Uri.EscapeDataString(this.ClassName),
                        Uri.EscapeDataString(this.ObjectId)
                    }), currentSessionToken2, jSONObjectForSaving, cancellationToken) : Task.FromResult<Tuple<HttpStatusCode, IDictionary<string, object>>>(new Tuple<HttpStatusCode, IDictionary<string, object>>(HttpStatusCode.OK, new Dictionary<string, object>()))));
                }
                return result;
            })).OnSuccess(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                Tuple<HttpStatusCode, IDictionary<string, object>> result = t.Result;
                Tuple<HttpStatusCode, IDictionary<string, object>> result2 = result;
                tuple1 = result;
                return result2;
            }).ContinueWith(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                lock (this.mutex)
                {
                    if (tuple1 == null)
                    {
                        this.MergeAfterFailedSave(strs);
                    }
                    else
                    {
                        this.IsNew = (tuple1.Item1 == HttpStatusCode.Created);
                        this.MergeAfterSave(tuple1.Item2);
                    }
                }
                return t;
            }));
        }

        public Task SaveAsync()
        {
            return this.SaveAsync(CancellationToken.None);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return this.taskQueue.Enqueue<Task>((Task toAwait) => this.SaveAsync(toAwait, cancellationToken), cancellationToken);
        }

        internal IDictionary<string, object> ServerDataToJSONObjectForSerialization()
        {
            IDictionary<string, object> result;
            lock (this.mutex)
            {
                result = (AVClient.MaybeEncodeJSONObject(this.serverData, true) as IDictionary<string, object>);
            }
            return result;
        }

        internal virtual void SetDefaultValues()
        {
        }

        private void SetObjectIdInternal(string objectId)
        {
            lock (this.mutex)
            {
                this.objectId = objectId;
                this.OnPropertyChanged("ObjectId");
            }
        }

        protected void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            this[AVObject.GetFieldForPropertyName(base.GetType(), propertyName)] = value;
        }

        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            return this.GetProperty<T>(default(T), propertyName);
        }

        protected T GetProperty<T>(T defaultValue, [CallerMemberName] string propertyName = null)
        {
            T result;
            if (this.TryGetValue<T>(AVObject.GetFieldForPropertyName(base.GetType(), propertyName), out result))
            {
                return result;
            }
            return defaultValue;
        }

        protected AVRelation<T> GetRelationProperty<T>([CallerMemberName] string propertyName = null) where T : AVObject
        {
            return this.GetRelation<T>(AVObject.GetFieldForPropertyName(base.GetType(), propertyName));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal IDictionary<string, IAVFieldOperation> StartSave()
        {
            IDictionary<string, IAVFieldOperation> result;
            lock (this.mutex)
            {
                IDictionary<string, IAVFieldOperation> currentOperations = this.CurrentOperations;
                this.operationSetQueue.AddLast(new Dictionary<string, IAVFieldOperation>());
                this.OnPropertyChanged("IsDirty");
                result = currentOperations;
            }
            return result;
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this.estimatedData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
        }

        internal IDictionary<string, object> ToJSONObjectForSaving(IDictionary<string, IAVFieldOperation> operations)
        {
            IDictionary<string, object> result;
            lock (this.mutex)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                using (IEnumerator<KeyValuePair<string, IAVFieldOperation>> enumerator = operations.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, IAVFieldOperation> current = enumerator.Current;
                        IAVFieldOperation value = current.Value;
                        dictionary[current.Key]=AVClient.MaybeEncodeJSONObject(value, true);
                    }
                }
                result = dictionary;
            }
            return result;
        }

        internal IDictionary<string, object> ToJSONObjectForSaving(IDictionary<string, object> data)
        {
            IDictionary<string, object> dictionary;
            lock (this.mutex)
            {
                dictionary = new Dictionary<string, object>();
                using (IEnumerator<string> enumerator = data.Keys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        object value = data[current];
                        dictionary.Add(current, AVClient.MaybeEncodeJSONObject(value, true));
                    }
                }
            }
            return dictionary;
        }

        public bool TryGetValue<T>(string key, out T result)
        {
            bool result2;
            lock (this.mutex)
            {
                if (this.ContainsKey(key))
                {
                    object obj2 = AVClient.ConvertTo<T>(this[key]);
                    if (obj2 is T || (obj2 == null && (!Type.GetTypeFromHandle(typeof(T).TypeHandle).GetTypeInfo().IsValueType || typeof(T).IsNullable())))
                    {
                        result = (T)((object)obj2);
                        result2 = true;
                        return result2;
                    }
                }
                result = default(T);
                result2 = false;
            }
            return result2;
        }

        internal static void UnregisterSubclass(string className)
        {
            AVObject.objectFactories.Remove(className);
        }
    }
}
