using AVOSCloud;
using AVOSCloud.Internal;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AVOSCloud.RealtimeMessageV2
{
    public class AVIMClient
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, Tuple<CompareCMD, EventHandler<IDictionary<string, object>>>> listeners;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Timer timer;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EventHandler<EventArgs> m_onReconnected;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EventHandler<AVIMOnMessageReceivedEventArgs> m_OnMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EventHandler<AVIMOnMembersChangedEventArgs> m_OnConversationMembersChanged;

        public string ClientId { get; set; }

        public ClientStatus Status { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IRealtimeMessagePlatformHook imHook { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal Dictionary<string, Tuple<CompareCMD, EventHandler<IDictionary<string, object>>>> Listeners
        {
            get
            {
                if (this.listeners == null)
                    this.listeners = new Dictionary<string, Tuple<CompareCMD, EventHandler<IDictionary<string, object>>>>();
                return this.listeners;
            }
            set
            {
                this.listeners = value;
            }
        }

        public ISignatureFactoryV2 SignatureFactory { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string onMessageRecievedOnClientListenerKey
        {
            get
            {
                return this.ClientId + "_OnMessageRecievedOnClient";
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string onConversationMembersChangedOnClientListenerKey
        {
            get
            {
                return this.ClientId + "_OnConversationMembersChangedOnClient";
            }
        }

        public event EventHandler<EventArgs> OnReconnected
        {
            add
            {
                this.m_onReconnected += value;
            }
            remove
            {
                this.m_onReconnected -= value;
            }
        }

        public event EventHandler<AVIMOnMessageReceivedEventArgs> OnMessageReceieved
        {
            add
            {
                this.m_OnMessageReceived += value;
                this.BindReceiver(this.onMessageRecievedOnClientListenerKey, new CompareCMD(this.pairMessageReceivedOnClient), new EventHandler<IDictionary<string, object>>(this.invokeOnAVIMMessageReceviedOnClient));
            }
            remove
            {
                this.m_OnMessageReceived -= value;
                this.UnbindReceiver(this.onMessageRecievedOnClientListenerKey);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnConversationMembersChanged
        {
            add
            {
                this.m_OnConversationMembersChanged += value;
                this.BindReceiver(this.onConversationMembersChangedOnClientListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnClient));
            }
            remove
            {
                this.m_OnConversationMembersChanged -= value;
                this.UnbindReceiver(this.onConversationMembersChangedOnClientListenerKey);
            }
        }

        internal AVIMClient()
        {
        }

        public AVIMClient(string clientId)
          : this()
        {
            this.ClientId = clientId;
            this.imHook = Activator.CreateInstance(Type.GetType("AVOSCloud.RealtimeMessage.RealtimeMessageHooks")) as IRealtimeMessagePlatformHook;
        }

        private void SetHeartbeat()
        {
            this.timer = new Timer((s => this.OpenThenSendAsync(new Dictionary<string, object>())), null, 300000, 300000);
            this.timer.Change(300000, 300000);
        }

        private void StopHeartbeat()
        {
            this.timer.Dispose();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.OpenThenSendAsync((IDictionary<string, object>)new Dictionary<string, object>());
        }

        public AVIMConversationQuery GetQuery()
        {
            return new AVIMConversationQuery(this);
        }

        public Task<bool> ConnectAsync()
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>)new Dictionary<string, object>();
            cmdBody.Add("i", (object)AVIMCommon.NextCmdId);
            cmdBody.Add("ua", (object)("wp/" + (object)AVClient.Version));
            cmdBody.Add("cmd", (object)"session");
            cmdBody.Add("op", (object)"open");
            cmdBody.Add("peerId", (object)this.ClientId);
            cmdBody.Add("appId", (object)AVClient.ApplicationId);
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, bool>(TaskExtensions.Unwrap<Tuple<string, IDictionary<string, object>>>(this.AttachConnectSignature(cmdBody).ContinueWith<Task<Tuple<string, IDictionary<string, object>>>>((Func<Task<IDictionary<string, object>>, Task<Tuple<string, IDictionary<string, object>>>>)(s => this.imHook.EAP2TAP4WebSocket(s.Result)))), (Func<Task<Tuple<string, IDictionary<string, object>>>, bool>)(t =>
            {
                AVIMUtils.HandlerException(t.Result);
                this.SetHeartbeat();
                this.Status = ClientStatus.Online;
                this.imHook.OnReceived += new EventHandler<IDictionary<string, object>>(this.imHook_OnReceived);
                //PhoneApplicationService.get_Current().add_Deactivated(new EventHandler<DeactivatedEventArgs>(this.Current_Deactivated));
                //PhoneApplicationService.get_Current().add_Activated(new EventHandler<ActivatedEventArgs>(this.Current_Activated));
                Application.Current.Resuming += Current_Resuming;
                Application.Current.Suspending += Current_Suspending;
                return true;
            }));
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
           
        }

        private void Current_Resuming(object sender, object e)
        {
            this.StopHeartbeat();
        }

        public Task<bool> ReconnectAsync()
        {
            IDictionary<string, object> cmd = (IDictionary<string, object>)new Dictionary<string, object>();
            cmd.Add("i", (object)AVIMCommon.NextCmdId);
            cmd.Add("ua", (object)("wp/" + (object)AVClient.Version));
            cmd.Add("cmd", (object)"session");
            cmd.Add("op", (object)"open");
            cmd.Add("peerId", (object)this.ClientId);
            cmd.Add("appId", (object)AVClient.ApplicationId);
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, bool>(this.imHook.EAP2TAP4WebSocket(cmd), (Func<Task<Tuple<string, IDictionary<string, object>>>, bool>)(t =>
            {
                AVIMUtils.HandlerException(t.Result);
                this.SetHeartbeat();
                this.Status = ClientStatus.Online;
                return true;
            }));
        }

      

        public Task<bool> DisconnectAsync()
        {
            IDictionary<string, object> cmd = (IDictionary<string, object>)new Dictionary<string, object>();
            cmd.Add("i", (object)AVIMCommon.NextCmdId);
            cmd.Add("cmd", (object)"session");
            cmd.Add("op", (object)"close");
            cmd.Add("peerId", (object)this.ClientId);
            cmd.Add("appId", (object)AVClient.ApplicationId);
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, bool>(this.OpenThenSendAsync(cmd), (Func<Task<Tuple<string, IDictionary<string, object>>>, bool>)(t =>
            {
                this.Status = ClientStatus.Offline;
                Application.Current.Resuming -= Current_Resuming;
                Application.Current.Suspending -= Current_Suspending;
                return true;
            }));
        }

        public AVIMConversation GetConversationById(string conversationId)
        {
            return new AVIMConversation()
            {
                ConversationId = conversationId,
                CurrentClient = this
            };
        }

        public Task<AVIMConversation> CreateConversationAsync(string clientId)
        {
            return this.CreateConversationAsync(clientId, (string)null, (IDictionary<string, object>)null);
        }

        public Task<AVIMConversation> CreateConversationAsync(string clientId, IDictionary<string, object> attr)
        {
            IList<string> clientIds = (IList<string>)new List<string>();
            clientIds.Add(clientId);
            return this.CreateConversationAsync(clientIds, attr);
        }

        public Task<AVIMConversation> CreateConversationAsync(string clientId, string name, IDictionary<string, object> attr)
        {
            IList<string> clientIds = (IList<string>)new List<string>();
            clientIds.Add(clientId);
            return this.CreateConversationAsync(clientIds, name, attr, false);
        }

        public Task<AVIMConversation> CreateConversationAsync(string clientId, string name)
        {
            return this.CreateConversationAsync(clientId, name, (IDictionary<string, object>)null);
        }

        public Task<AVIMConversation> CreateConversationAsync(IList<string> clientIds)
        {
            return this.CreateConversationAsync(clientIds, (IDictionary<string, object>)null);
        }

        public Task<AVIMConversation> CreateConversationAsync(IList<string> clientIds, IDictionary<string, object> attr)
        {
            return this.CreateConversationAsync(clientIds, string.Empty, attr, false);
        }

        public Task<AVIMConversation> CreateConversationAsync(IList<string> clientIds, string name, IDictionary<string, object> attr, bool transient)
        {
            Dictionary<string, object> cmdBody = new Dictionary<string, object>();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            cmdBody.Add("cmd", (object)"conv");
            cmdBody.Add("op", (object)"start");
            cmdBody.Add("peerId", (object)this.ClientId);
            if (clientIds == null && !transient)
            {
                throw new AVIMException(new AVIMError()
                {
                    Code = AVException.ErrorCode.ClientIdsCanNotBeNull
                });
            }
            else
            {
                cmdBody.Add("m", (object)clientIds);
                cmdBody.Add("i", (object)nextCmdId);
                cmdBody.Add("appId", (object)AVClient.ApplicationId);
                cmdBody.Add("transient", (transient ? 1 : 0));
                if (!string.IsNullOrEmpty(name))
                    dictionary.Add("name", (object)name);
                if (attr != null && attr.Count > 1)
                    dictionary.Add("attr", (object)attr);
                if (dictionary.Count > 0)
                    cmdBody.Add("attr", (object)dictionary);
                Task<IDictionary<string, object>> task;
                if (this.SignatureFactory != null)
                {
                    task = this.AttachSignature((IDictionary<string, object>)cmdBody, this.SignatureFactory.CreateStartConversationSignature(this.ClientId, clientIds));
                }
                else
                {
                    TaskCompletionSource<IDictionary<string, object>> completionSource = new TaskCompletionSource<IDictionary<string, object>>();
                    completionSource.SetResult((IDictionary<string, object>)cmdBody);
                    task = completionSource.Task;
                }
                return InternalExtensions.OnSuccess<IDictionary<string, object>, AVIMConversation>(task, (Func<Task<IDictionary<string, object>>, AVIMConversation>)(s => InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, AVIMConversation>(this.OpenThenSendAsync((IDictionary<string, object>)cmdBody), (Func<Task<Tuple<string, IDictionary<string, object>>>, AVIMConversation>)(t =>
                {
                    AVIMUtils.HandlerException(t.Result);
                    IDictionary<string, object> data = t.Result.Item2;
                    AVIMConversation avimConversation = new AVIMConversation();
                    avimConversation.ConversationId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "cid");
                    avimConversation.CurrentClient = this;
                    List<string> list = new List<string>();
                    if (clientIds == null)
                        clientIds = (IList<string>)new List<string>();
                    clientIds.Insert(0, this.ClientId);
                    list.AddRange((IEnumerable<string>)clientIds);
                    avimConversation.MemberIds = (IList<string>)list;
                    avimConversation.IsTransient = transient;
                    avimConversation.Name = name;
                    return avimConversation;
                })).Result));
            }
        }

        public Task<AVIMConversation> CreateChatRoomAsync(string name)
        {
            return this.CreateConversationAsync((IList<string>)null, name, (IDictionary<string, object>)null, true);
        }

        internal Task<Tuple<string, IDictionary<string, object>>> OpenThenSendAsync(IDictionary<string, object> cmd)
        {
            return this.Status == ClientStatus.Online ? this.imHook.EAP2TAP4WebSocket(cmd) : TaskExtensions.Unwrap<Tuple<string, IDictionary<string, object>>>(this.ReconnectAsync().ContinueWith<Task<Tuple<string, IDictionary<string, object>>>>((Func<Task<bool>, Task<Tuple<string, IDictionary<string, object>>>>)(t => this.imHook.EAP2TAP4WebSocket(cmd))));
        }

        internal Task<IDictionary<string, object>> AttachConnectSignature(IDictionary<string, object> cmdBody)
        {
            TaskCompletionSource<IDictionary<string, object>> completionSource = new TaskCompletionSource<IDictionary<string, object>>();
            if (this.SignatureFactory != null)
                return this.AttachSignature(cmdBody, this.SignatureFactory.CreateConnectSignature(this.ClientId));
            completionSource.SetResult(cmdBody);
            return completionSource.Task;
        }

        internal Task<IDictionary<string, object>> AttachSignature(IDictionary<string, object> cmdBody, Task<AVIMSignatureV2> SignatureTask)
        {
            TaskCompletionSource<IDictionary<string, object>> completionSource = new TaskCompletionSource<IDictionary<string, object>>();
            if (SignatureTask != null)
                return InternalExtensions.OnSuccess<AVIMSignatureV2, IDictionary<string, object>>(SignatureTask, (Func<Task<AVIMSignatureV2>, IDictionary<string, object>>)(t =>
                {
                    if (t.Result != null)
                    {
                        AVIMSignatureV2 result = t.Result;
                        cmdBody.Add("t", (object)result.Timestamp);
                        cmdBody.Add("n", (object)result.Nonce);
                        cmdBody.Add("s", (object)result.SignatureContent);
                    }
                    return cmdBody;
                }));
            completionSource.SetResult(cmdBody);
            return completionSource.Task;
        }

        internal Task<IDictionary<string, object>> AttachConversationSignature(IDictionary<string, object> cmdBody, string conversationId, IList<string> memberIds, string action)
        {
            TaskCompletionSource<IDictionary<string, object>> completionSource = new TaskCompletionSource<IDictionary<string, object>>();
            if (this.SignatureFactory != null)
                return this.AttachSignature(cmdBody, this.SignatureFactory.CreateConversationSignature(conversationId, this.ClientId, memberIds, action));
            completionSource.SetResult(cmdBody);
            return completionSource.Task;
        }

        internal Task<IDictionary<string, object>> AttachQueryHistorySignature(IDictionary<string, object> cmdBody, string conversationId)
        {
            TaskCompletionSource<IDictionary<string, object>> completionSource = new TaskCompletionSource<IDictionary<string, object>>();
            if (this.SignatureFactory != null)
                return this.AttachSignature(cmdBody, this.SignatureFactory.CreateQueryHistorySignature(this.ClientId, conversationId));
            completionSource.SetResult(cmdBody);
            return completionSource.Task;
        }

        internal void BindReceiver(string key, CompareCMD comparer, EventHandler<IDictionary<string, object>> handler)
        {
            if (Enumerable.Contains<string>((IEnumerable<string>)this.Listeners.Keys, key))
                this.Listeners[key] = new Tuple<CompareCMD, EventHandler<IDictionary<string, object>>>(comparer, handler);
            else
                this.Listeners.Add(key, new Tuple<CompareCMD, EventHandler<IDictionary<string, object>>>(comparer, handler));
        }

        internal void UnbindReceiver(string key)
        {
            if (!Enumerable.Contains<string>((IEnumerable<string>)this.Listeners.Keys, key))
                return;
            this.Listeners.Remove(key);
        }

        private void imHook_OnReceived(object sender, IDictionary<string, object> e)
        {
            if (this.listeners == null)
                return;
            foreach (KeyValuePair<string, Tuple<CompareCMD, EventHandler<IDictionary<string, object>>>> keyValuePair in this.listeners)
            {
                if (keyValuePair.Value.Item1(e))
                    keyValuePair.Value.Item2((object)this, e);
            }
        }

        internal bool pairMessageReceivedOnClient(IDictionary<string, object> e)
        {
            bool flag = false;
            try
            {
                if (AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "cmd") == "direct")
                    flag = true;
            }
            catch
            {
            }
            return flag;
        }

        internal void invokeOnAVIMMessageReceviedOnClient(object sender, IDictionary<string, object> e)
        {
            AVIMConversation avimConversation = new AVIMConversation()
            {
                ConversationId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "cid")
            };
            AVIMMessage avimMessage = AVIMUtils.RestoreAVIMMessageFromServer(e);
            avimMessage.MessageIOType = AVIMMessageIOType.AVIMMessageIOTypeIn;
            this.SendAck(avimMessage);
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "NONE");
            try
            {
                IDictionary<string, object> dictionary = AVClient.DeserializeJsonString(avimMessage.MessageBody);
                if (dictionary == null || !dictionary.ContainsKey(AVIMProtocol.LCTYPE))
                    return;
                int num = AVRMProtocolUtils.CaptureInteger(dictionary, AVIMProtocol.LCTYPE);
                AVIMMessageMediaType result = AVIMMessageMediaType.None;
                try
                {
                    Enum.TryParse<AVIMMessageMediaType>(num.ToString(), out result);
                    switch (result)
                    {
                        case AVIMMessageMediaType.File:
                            avimMessage = (AVIMMessage)this.CreateDerivedTypedMessageType<AVIMFileMessage>(avimMessage);
                            break;
                        case AVIMMessageMediaType.Location:
                            avimMessage = (AVIMMessage)this.CreateDerivedTypedMessageType<AVIMLocationMessage>(avimMessage);
                            break;
                        case AVIMMessageMediaType.Video:
                            avimMessage = (AVIMMessage)this.CreateDerivedTypedMessageType<AVIMVideoMessage>(avimMessage);
                            break;
                        case AVIMMessageMediaType.Audio:
                            avimMessage = (AVIMMessage)this.CreateDerivedTypedMessageType<AVIMAudioMessage>(avimMessage);
                            break;
                        case AVIMMessageMediaType.Image:
                            avimMessage = (AVIMMessage)this.CreateDerivedTypedMessageType<AVIMImageMessage>(avimMessage);
                            break;
                        case AVIMMessageMediaType.Text:
                            avimMessage = (AVIMMessage)this.CreateDerivedTypedMessageType<AVIMTextMessage>(avimMessage);
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    AVIMDefaultTypedMessage typedMessageType = this.CreateDerivedTypedMessageType<AVIMDefaultTypedMessage>(avimMessage);
                    typedMessageType.Deserialize(dictionary);
                    avimMessage = (AVIMMessage)typedMessageType;
                }
            }
            catch
            {
            }
            finally
            {
                this.m_OnMessageReceived((object)this, new AVIMOnMessageReceivedEventArgs()
                {
                    Conversation = avimConversation,
                    Message = avimMessage
                });
            }
        }

        internal void SendAck(AVIMMessage message)
        {
            if (!message.Receipt)
                return;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            dictionary.Add("cmd", (object)"ack");
            dictionary.Add("cid", (object)message.ConversationId);
            dictionary.Add("mid", (object)message.Id);
            dictionary.Add("peerId", (object)this.ClientId);
            dictionary.Add("i", (object)nextCmdId);
            dictionary.Add("appId", (object)AVClient.ApplicationId);
            this.OpenThenSendAsync((IDictionary<string, object>)dictionary);
        }

        internal T CreateDerivedTypedMessageType<T>(AVIMMessage avMessage) where T : AVIMTypedMessage, new()
        {
            T instance = Activator.CreateInstance<T>();
            IDictionary<string, object> typedMessageBodyFromServer = AVClient.DeserializeJsonString(avMessage.MessageBody);
            instance.CopyFromBase(avMessage);
            instance.Deserialize(typedMessageBodyFromServer);
            return instance;
        }

        internal AVIMMessage RestoreAVIMMessageFromServer<T>(IDictionary<string, object> serverData) where T : AVIMMessage
        {
            AVIMMessage avimMessage = AVIMUtils.RestoreAVIMMessageFromServer(serverData);
            avimMessage.MessageIOType = AVIMMessageIOType.AVIMMessageIOTypeIn;
            IDictionary<string, object> dictionary = AVClient.DeserializeJsonString(avimMessage.MessageBody);
            try
            {
                Type type = typeof(T);
                MethodInfo method1 = type.GetMethod("CopyFromBase",null);
                MethodInfo method2 = type.GetMethod("Deserialize",null);
                T instance = Activator.CreateInstance<T>();
                object[] parameters1 = new object[1]
                {
          (object) avimMessage
                };
                method1.Invoke((object)instance, parameters1);
                object[] parameters2 = new object[1]
                {
          (object) dictionary
                };
                method2.Invoke((object)instance, parameters2);
                return (AVIMMessage)instance;
            }
            catch
            {
                return avimMessage;
            }
        }

        internal bool pairOnConversationMembersChanged(IDictionary<string, object> e)
        {
            bool flag = false;
            try
            {
                if (AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "cmd") == "conv")
                    flag = true;
            }
            catch
            {
            }
            return flag;
        }

        internal void invokeOnConversationMembersChangedOnClient(object sender, IDictionary<string, object> e)
        {
            List<Tuple<object, EventHandler<AVIMOnMembersChangedEventArgs>>> eventHandlers = new List<Tuple<object, EventHandler<AVIMOnMembersChangedEventArgs>>>()
      {
        new Tuple<object, EventHandler<AVIMOnMembersChangedEventArgs>>((object) this, this.m_OnConversationMembersChanged)
      };
            AVIMUtils.GenerateConversationMembersChangedArgs(e, eventHandlers);
        }
    }
}