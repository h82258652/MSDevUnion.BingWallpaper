using AVOSCloud;
using AVOSCloud.Internal;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessageV2
{
    public class AVIMConversation : IAVObjectBase, INotifyPropertyChanged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly object mutex = new object();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal SynchronizedEventHandler<PropertyChangedEventArgs> propertyChanged = new SynchronizedEventHandler<PropertyChangedEventArgs>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string ListenerKeySuffix = "OnConversation";
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMOnMembersChangedEventArgs> m_OnMembersJoined;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMOnMembersChangedEventArgs> m_OnMembersLeft;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMOnMembersChangedEventArgs> m_OnJoined;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMOnMembersChangedEventArgs> m_OnLeft;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMOnMembersChangedEventArgs> m_OnKicked;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMOnMembersChangedEventArgs> m_OnInvited;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMMessage> m_OnMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMTypedMessage> m_OnTypedMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMFileMessage> m_OnFileMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMTextMessage> m_OnTextMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMImageMessage> m_OnImageMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMAudioMessage> m_OnAudioMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMVideoMessage> m_OnVideoMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMLocationMessage> m_OnLocationMessageReceived;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventHandler<AVIMMessage> m_OnMessageDeliverd;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IDictionary<string, object> fetchedAttributes;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IDictionary<string, object> pendingAttributes;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? _lastMessageAt;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal AVIMClient _currentClient;

        public string ConversationId { get; set; }

        public string Name { get; set; }

        public IList<string> MemberIds { get; set; }

        public IList<string> MuteMemberIds { get; set; }

        public string Creator { get; set; }

        public bool IsTransient { get; set; }

        public IDictionary<string, object> Attributes
        {
            get
            {
                return AVRMProtocolUtils.Merge(this.fetchedAttributes, this.pendingAttributes);
            }
        }

        public DateTime? LastMesaageAt
        {
            get
            {
                lock (this.mutex)
                  return this._lastMessageAt;
            }
            private set
            {
                lock (this.mutex)
                {
                    this._lastMessageAt = value;
                    this.OnPropertyChanged("LastMesaageAt");
                }
            }
        }

        public AVIMClient CurrentClient
        {
            get
            {
                return this._currentClient;
            }
            set
            {
                this._currentClient = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnMembersJoinedListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnMembersJoined" + this.ListenerKeySuffix;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnMembersLeftListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnMembersLeft" + this.ListenerKeySuffix;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnJoinedListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnJoined" + this.ListenerKeySuffix;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnLeftListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnLeft" + this.ListenerKeySuffix;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnKickedListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnKicked" + this.ListenerKeySuffix;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnInvitedListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnInvited" + this.ListenerKeySuffix;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string m_OnMessageRecievedListenerKey
        {
            get
            {
                return this.ConversationId + "_m_OnMessageRecieved" + this.ListenerKeySuffix;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.propertyChanged.Add((Delegate)value);
            }
            remove
            {
                this.propertyChanged.Remove((Delegate)value);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnMembersJoined
        {
            add
            {
                this.m_OnMembersJoined += value;
                this.CurrentClient.BindReceiver(this.m_OnMembersJoinedListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnConversation));
            }
            remove
            {
                this.m_OnMembersJoined -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnMembersJoinedListenerKey);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnMembersLeft
        {
            add
            {
                this.m_OnMembersLeft += value;
                this.CurrentClient.BindReceiver(this.m_OnMembersLeftListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnConversation));
            }
            remove
            {
                this.m_OnMembersLeft -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnMembersLeftListenerKey);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnJoined
        {
            add
            {
                this.m_OnJoined += value;
                this.CurrentClient.BindReceiver(this.m_OnJoinedListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnConversation));
            }
            remove
            {
                this.m_OnJoined -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnJoinedListenerKey);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnLeft
        {
            add
            {
                this.m_OnLeft += value;
                this.CurrentClient.BindReceiver(this.m_OnLeftListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnConversation));
            }
            remove
            {
                this.m_OnLeft -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnLeftListenerKey);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnKicked
        {
            add
            {
                this.m_OnKicked += value;
                this.CurrentClient.BindReceiver(this.m_OnKickedListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnConversation));
            }
            remove
            {
                this.m_OnKicked -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnKickedListenerKey);
            }
        }

        public event EventHandler<AVIMOnMembersChangedEventArgs> OnInvited
        {
            add
            {
                this.m_OnInvited += value;
                this.CurrentClient.BindReceiver(this.m_OnInvitedListenerKey, new CompareCMD(this.pairOnConversationMembersChanged), new EventHandler<IDictionary<string, object>>(this.invokeOnConversationMembersChangedOnConversation));
            }
            remove
            {
                this.m_OnInvited -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnInvitedListenerKey);
            }
        }

        public event EventHandler<AVIMMessage> OnMessageReceived
        {
            add
            {
                this.m_OnMessageReceived += value;
                this.CurrentClient.BindReceiver(this.m_OnMessageRecievedListenerKey, new CompareCMD(this.pairMessageReceivedOnConversation), new EventHandler<IDictionary<string, object>>(this.invokeOnAVIMMessageReceviedOnConversation));
            }
            remove
            {
                this.m_OnMessageReceived -= value;
                this.CurrentClient.UnbindReceiver(this.m_OnMessageRecievedListenerKey);
            }
        }

        public event EventHandler<AVIMTypedMessage> OnTypedMessageReceived
        {
            add
            {
                this.m_OnTypedMessageReceived += value;
            }
            remove
            {
                this.m_OnTypedMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMFileMessage> OnFileMessageReceived
        {
            add
            {
                this.m_OnFileMessageReceived += value;
            }
            remove
            {
                this.m_OnFileMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMTextMessage> OnTextMessageReceived
        {
            add
            {
                this.m_OnTextMessageReceived += value;
            }
            remove
            {
                this.m_OnTextMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMImageMessage> OnImageMessageReceived
        {
            add
            {
                this.m_OnImageMessageReceived += value;
            }
            remove
            {
                this.m_OnImageMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMAudioMessage> OnAudioMessageReceived
        {
            add
            {
                this.m_OnAudioMessageReceived += value;
            }
            remove
            {
                this.m_OnAudioMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMVideoMessage> OnVideoMessageReceived
        {
            add
            {
                this.m_OnVideoMessageReceived += value;
            }
            remove
            {
                this.m_OnVideoMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMLocationMessage> OnLocationMessageReceived
        {
            add
            {
                this.m_OnLocationMessageReceived += value;
            }
            remove
            {
                this.m_OnLocationMessageReceived -= value;
            }
        }

        public event EventHandler<AVIMMessage> OnMessageDeliverd
        {
            add
            {
                this.m_OnMessageDeliverd += value;
            }
            remove
            {
                this.m_OnMessageDeliverd -= value;
            }
        }

        internal AVIMConversation()
        {
        }

        public void SetAttribute(string key, object value)
        {
            if (this.pendingAttributes == null)
                this.pendingAttributes = (IDictionary<string, object>)new Dictionary<string, object>();
            AVRMProtocolUtils.Write(this.pendingAttributes, key, value);
        }

        public Task FetchAsync()
        {
            return this.CurrentClient.GetQuery().WhereEqualTo("objectId", (object)this.ConversationId).FindAsync().ContinueWith((Action<Task<IEnumerable<AVIMConversation>>>)(t =>
            {
                AVIMConversation avimConversation = Enumerable.FirstOrDefault<AVIMConversation>(t.Result);
                if (avimConversation == null)
                    return;
                this.MemberIds = avimConversation.MemberIds;
                this.MuteMemberIds = avimConversation.MuteMemberIds;
                this.Name = avimConversation.Name;
                this.fetchedAttributes = avimConversation.Attributes;
            }));
        }

        public Task<bool> AddMembersAsync(IList<string> clientIds)
        {
            Dictionary<string, object> cmdBody = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            cmdBody.Add("cmd", (object)"conv");
            cmdBody.Add("op", (object)"add");
            cmdBody.Add("peerId", (object)this.CurrentClient.ClientId);
            cmdBody.Add("appId", (object)AVClient.ApplicationId);
            cmdBody.Add("m", (object)clientIds);
            cmdBody.Add("i", (object)nextCmdId);
            cmdBody.Add("cid", (object)this.ConversationId);
            return TaskExtensions.Unwrap<bool>(this.CurrentClient.AttachConversationSignature((IDictionary<string, object>)cmdBody, this.ConversationId, clientIds, "invite").ContinueWith<Task<bool>>((Func<Task<IDictionary<string, object>>, Task<bool>>)(s => InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, bool>(this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)cmdBody), (Func<Task<Tuple<string, IDictionary<string, object>>>, bool>)(t =>
            {
                AVIMUtils.HandlerException(t.Result);
                IDictionary<string, object> dictionary = t.Result.Item2;
                return true;
            })))));
        }

        public Task<bool> AddMembersAsync(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
                completionSource.TrySetResult(false);
                return completionSource.Task;
            }
            else
            {
                IList<string> clientIds = (IList<string>)new List<string>();
                clientIds.Add(clientId);
                return this.AddMembersAsync(clientIds);
            }
        }

        public Task<bool> RemoveMembersAsync(IList<string> clientIds)
        {
            Dictionary<string, object> cmdBody = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            cmdBody.Add("cmd", (object)"conv");
            cmdBody.Add("op", (object)"remove");
            cmdBody.Add("peerId", (object)this.CurrentClient.ClientId);
            cmdBody.Add("appId", (object)AVClient.ApplicationId);
            cmdBody.Add("m", (object)clientIds);
            cmdBody.Add("i", (object)nextCmdId);
            cmdBody.Add("cid", (object)this.ConversationId);
            return TaskExtensions.Unwrap<bool>(this.CurrentClient.AttachConversationSignature((IDictionary<string, object>)cmdBody, this.ConversationId, clientIds, "kick").ContinueWith<Task<bool>>((Func<Task<IDictionary<string, object>>, Task<bool>>)(s => InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, bool>(this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)cmdBody), (Func<Task<Tuple<string, IDictionary<string, object>>>, bool>)(t =>
            {
                AVIMUtils.HandlerException(t.Result);
                IDictionary<string, object> dictionary = t.Result.Item2;
                return true;
            })))));
        }

        public Task<bool> RemoveMembersAsync(string clientId)
        {
            IList<string> clientIds = (IList<string>)new List<string>();
            clientIds.Add(clientId);
            return this.RemoveMembersAsync(clientIds);
        }

        public Task<bool> LeftAsync()
        {
            return this.RemoveMembersAsync(this.CurrentClient.ClientId);
        }

        public Task<bool> JoinAsync()
        {
            return this.AddMembersAsync(this.CurrentClient.ClientId);
        }

        public Task<Tuple<bool, AVIMTextMessage>> SendTextMessageAsync(string textContent, bool transient, bool receipt)
        {
            AVIMTextMessage avTextMessage = new AVIMTextMessage(textContent);
            avTextMessage.Receipt = receipt;
            avTextMessage.Transient = transient;
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMMessage>, Tuple<bool, AVIMTextMessage>>(this.SendMessageAsync((AVIMMessage)avTextMessage), (Func<Task<Tuple<bool, AVIMMessage>>, Tuple<bool, AVIMTextMessage>>)(x => new Tuple<bool, AVIMTextMessage>(x.Result.Item1, avTextMessage)));
        }

        public Task<Tuple<bool, AVIMTextMessage>> SendTextMessageAsync(string textContent)
        {
            return this.SendTextMessageAsync(textContent, false, false);
        }

        public Task<Tuple<bool, AVIMMessage>> SendMessageAsync(AVIMMessage avMessage)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int cmdId = AVIMCommon.NextCmdId;
            avMessage.MessageIOType = AVIMMessageIOType.AVIMMessageIOTypeOut;
            dictionary.Add("cmd", (object)"direct");
            dictionary.Add("cid", (object)this.ConversationId);
            dictionary.Add("r", (avMessage.Receipt ? 1 : 0));
            dictionary.Add("transient", (avMessage.Transient ? 1 : 0));
            dictionary.Add("msg", (object)avMessage.MessageBody);
            dictionary.Add("peerId", (object)this.CurrentClient.ClientId);
            dictionary.Add("i", (object)cmdId);
            dictionary.Add("appId", (object)AVClient.ApplicationId);
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, Tuple<bool, AVIMMessage>>(this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)dictionary), (Func<Task<Tuple<string, IDictionary<string, object>>>, Tuple<bool, AVIMMessage>>)(t =>
            {
                IDictionary<string, object> data = t.Result.Item2;
                AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "cmd");
                avMessage.cmdId = cmdId.ToString();
                avMessage.MessageStatus = AVIMMessageStatus.AVIMMessageStatusSent;
                avMessage.ConversationId = this.ConversationId;
                avMessage.FromClientId = this.CurrentClient.ClientId;
                avMessage.Id = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "uid");
                avMessage.ServerTimestamp = AVRMProtocolUtils.CaptureValueFromDictionary<long>(data, "t");
                Tuple<bool, AVIMMessage> tuple = new Tuple<bool, AVIMMessage>(true, avMessage);
                this.LastMesaageAt = new DateTime?(AVPersistence.UnixTimeStampToDateTime(avMessage.ServerTimestamp));
                return tuple;
            }));
        }

        public Task<Tuple<bool, AVIMTypedMessage>> SendTypedMessageAsync(AVIMTypedMessage avTypedMessage)
        {
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMMessage>, Tuple<bool, AVIMTypedMessage>>(this.SendMessageAsync((AVIMMessage)avTypedMessage), (Func<Task<Tuple<bool, AVIMMessage>>, Tuple<bool, AVIMTypedMessage>>)(x => new Tuple<bool, AVIMTypedMessage>(x.Result.Item1, avTypedMessage)));
        }

        public Task<Tuple<bool, AVIMLocationMessage>> SendLocationMessageAsync(AVIMLocationMessage avLocationMessage)
        {
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMTypedMessage>, Tuple<bool, AVIMLocationMessage>>(this.SendTypedMessageAsync((AVIMTypedMessage)avLocationMessage), (Func<Task<Tuple<bool, AVIMTypedMessage>>, Tuple<bool, AVIMLocationMessage>>)(x => new Tuple<bool, AVIMLocationMessage>(x.Result.Item1, avLocationMessage)));
        }

        public Task<Tuple<bool, AVIMFileMessageBase>> SendFileMessageAsync(AVIMFileMessageBase avFileMessage)
        {
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMMessage>, Tuple<bool, AVIMFileMessageBase>>(TaskExtensions.Unwrap<Tuple<bool, AVIMMessage>>(TaskExtensions.Unwrap<IDictionary<string, object>>(avFileMessage.UploadFile().ContinueWith<Task<IDictionary<string, object>>>((Func<Task, Task<IDictionary<string, object>>>)(t =>
            {
                if (avFileMessage.IsExternalLink)
                    return Task.FromResult<IDictionary<string, object>>((IDictionary<string, object>)null);
                else
                    return avFileMessage.GetMetaDataFromQiniu();
            }))).ContinueWith<Task<Tuple<bool, AVIMMessage>>>((Func<Task<IDictionary<string, object>>, Task<Tuple<bool, AVIMMessage>>>)(s => this.SendMessageAsync((AVIMMessage)avFileMessage)))), (Func<Task<Tuple<bool, AVIMMessage>>, Tuple<bool, AVIMFileMessageBase>>)(x => new Tuple<bool, AVIMFileMessageBase>(x.Result.Item1, avFileMessage)));
        }

        public Task<Tuple<bool, AVIMImageMessage>> SendImageMessageAsync(AVIMImageMessage avImageMessage)
        {
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMFileMessageBase>, Tuple<bool, AVIMImageMessage>>(this.SendFileMessageAsync((AVIMFileMessageBase)avImageMessage), (Func<Task<Tuple<bool, AVIMFileMessageBase>>, Tuple<bool, AVIMImageMessage>>)(x => new Tuple<bool, AVIMImageMessage>(x.Result.Item1, avImageMessage)));
        }

        public Task<Tuple<bool, AVIMVideoMessage>> SendVideoMessageAsync(AVIMVideoMessage avVideoMessage)
        {
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMFileMessageBase>, Tuple<bool, AVIMVideoMessage>>(this.SendFileMessageAsync((AVIMFileMessageBase)avVideoMessage), (Func<Task<Tuple<bool, AVIMFileMessageBase>>, Tuple<bool, AVIMVideoMessage>>)(x => new Tuple<bool, AVIMVideoMessage>(x.Result.Item1, avVideoMessage)));
        }

        public Task<Tuple<bool, AVIMAudioMessage>> SendAudioMessageAsync(AVIMAudioMessage avAudioMessage)
        {
            return InternalExtensions.OnSuccess<Tuple<bool, AVIMFileMessageBase>, Tuple<bool, AVIMAudioMessage>>(this.SendFileMessageAsync((AVIMFileMessageBase)avAudioMessage), (Func<Task<Tuple<bool, AVIMFileMessageBase>>, Tuple<bool, AVIMAudioMessage>>)(x => new Tuple<bool, AVIMAudioMessage>(x.Result.Item1, avAudioMessage)));
        }

        public Task SaveAsync()
        {
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            dictionary1.Add("cmd", (object)"conv");
            dictionary1.Add("op", (object)"update");
            dictionary1.Add("peerId", (object)this.CurrentClient.ClientId);
            dictionary1.Add("appId", (object)AVClient.ApplicationId);
            dictionary1.Add("i", (object)nextCmdId);
            dictionary1.Add("cid", (object)this.ConversationId);
            if (!string.IsNullOrEmpty(this.Name))
                dictionary2.Add("name", (object)this.Name);
            if (this.Attributes != null && this.Attributes.Count > 1)
                dictionary2.Add("attr", (object)this.Attributes);
            if (dictionary2.Count > 0)
                dictionary1.Add("attr", (object)dictionary2);
            return (Task)this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)dictionary1);
        }

        public Task<int> CountMembersAsync()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            dictionary.Add("cmd", (object)"conv");
            dictionary.Add("op", (object)"count");
            dictionary.Add("peerId", (object)this.CurrentClient.ClientId);
            dictionary.Add("appId", (object)AVClient.ApplicationId);
            dictionary.Add("i", (object)nextCmdId);
            dictionary.Add("cid", (object)this.ConversationId);
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, int>(this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)dictionary), (Func<Task<Tuple<string, IDictionary<string, object>>>, int>)(t => AVRMProtocolUtils.CaptureInteger(t.Result.Item2, "count")));
        }

        public Task MuteAsync()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            dictionary.Add("cmd", (object)"conv");
            dictionary.Add("op", (object)"mute");
            dictionary.Add("peerId", (object)this.CurrentClient.ClientId);
            dictionary.Add("appId", (object)AVClient.ApplicationId);
            dictionary.Add("i", (object)nextCmdId);
            dictionary.Add("cid", (object)this.ConversationId);
            return (Task)this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)dictionary);
        }

        public Task UnmuteAsync()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            dictionary.Add("cmd", (object)"conv");
            dictionary.Add("op", (object)"mute");
            dictionary.Add("peerId", (object)this.CurrentClient.ClientId);
            dictionary.Add("appId", (object)AVClient.ApplicationId);
            dictionary.Add("i", (object)nextCmdId);
            dictionary.Add("cid", (object)this.ConversationId);
            return (Task)this.CurrentClient.OpenThenSendAsync((IDictionary<string, object>)dictionary);
        }

        public Task<IEnumerable<AVIMMessage>> QueryHistory(DateTime latestFlag, int limit, string clientId)
        {
            Dictionary<string, object> paramsDic = new Dictionary<string, object>();
            paramsDic.Add("convid", (object)this.ConversationId);
            if (latestFlag >= DateTime.MinValue)
                paramsDic.Add("max_ts", (object)InternalExtensions.UnixTimeStamp(latestFlag));
            if (limit > 0)
                paramsDic.Add("limit", (object)limit);
            if (!string.IsNullOrEmpty(clientId))
                paramsDic.Add("peerid", (object)clientId);
            if (this.CurrentClient == null)
                this.CurrentClient = new AVIMClient(clientId);
            return TaskExtensions.Unwrap<IEnumerable<AVIMMessage>>(this.CurrentClient.AttachQueryHistorySignature((IDictionary<string, object>)paramsDic, this.ConversationId).ContinueWith<Task<IEnumerable<AVIMMessage>>>((Func<Task<IDictionary<string, object>>, Task<IEnumerable<AVIMMessage>>>)(a => InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, IEnumerable<AVIMMessage>>(AVClient.RequestAsync("GET", "/rtm/messages/logs?" + AVClient.BuildQueryString((IDictionary<string, object>)paramsDic), (string)null, (IDictionary<string, object>)null, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, IEnumerable<AVIMMessage>>)(t =>
            {
                IEnumerable<AVIMMessage> source = (IEnumerable<AVIMMessage>)null;
                if (t.Result.Item1 == HttpStatusCode.OK)
                    source = Enumerable.Select<object, AVIMMessage>((IEnumerable<object>)(t.Result.Item2["results"] as IList<object>), (Func<object, AVIMMessage>)(item => this.CreateAVIMMessageFromQueryResult(item as IDictionary<string, object>)));
                return (IEnumerable<AVIMMessage>)new ReadOnlyCollection<AVIMMessage>((IList<AVIMMessage>)Enumerable.ToList<AVIMMessage>(source));
            })))));
        }

        internal AVIMMessage CreateAVIMMessageFromQueryResult(IDictionary<string, object> msgLog)
        {
            AVIMDefaultMessage avimDefaultMessage = new AVIMDefaultMessage();
            avimDefaultMessage.ServerTimestamp = AVRMProtocolUtils.CaptureLong(msgLog, "timestamp");
            avimDefaultMessage.ConversationId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(msgLog, "conv-id");
            avimDefaultMessage.FromClientId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(msgLog, "from");
            avimDefaultMessage.MessageBody = AVRMProtocolUtils.CaptureValueFromDictionary<string>(msgLog, "data");
            return (AVIMMessage)avimDefaultMessage;
        }

        internal virtual void MergeMagicFields(IDictionary<string, object> data)
        {
            lock (this.mutex)
            {
                if (data.ContainsKey("objectId"))
                {
                    this.ConversationId = data["objectId"] as string;
                    this.OnPropertyChanged("IsDataAvailable");
                    data.Remove("objectId");
                }
                if (data.ContainsKey("name"))
                {
                    this.Name = data["name"] as string;
                    data.Remove("name");
                }
                if (data.ContainsKey("lm"))
                {
                    this.LastMesaageAt = AVClient.Decode(data["lm"]) as DateTime?;
                    data.Remove("lm");
                }
                if (data.ContainsKey("m"))
                {
                    this.MemberIds = AVRMProtocolUtils.CaptureListFromDictionary<string>(data, "m");
                    data.Remove("m");
                }
                if (data.ContainsKey("mu"))
                {
                    this.MuteMemberIds = AVRMProtocolUtils.CaptureListFromDictionary<string>(data, "mu");
                    data.Remove("mu");
                }
                if (data.ContainsKey("c"))
                {
                    this.Creator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "c");
                    data.Remove("c");
                }
                if (!data.ContainsKey("attr"))
                    return;
                this.fetchedAttributes = (IDictionary<string, object>)(data["attr"] as Dictionary<string, object>);
                data.Remove("attr");
            }
        }

        internal bool pairMessageReceivedOnConversation(IDictionary<string, object> e)
        {
            bool flag = false;
            try
            {
                if (AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "cmd") == "direct")
                {
                    if (AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "cid") == this.ConversationId)
                        flag = true;
                }
            }
            catch
            {
            }
            return flag;
        }

        internal void invokeOnAVIMMessageReceviedOnConversation(object sender, IDictionary<string, object> e)
        {
            AVIMMessage avimMessage = AVIMUtils.RestoreAVIMMessageFromServer(e);
            avimMessage.MessageIOType = AVIMMessageIOType.AVIMMessageIOTypeIn;
            this.CurrentClient.SendAck(avimMessage);
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
                    System.Enum.TryParse<AVIMMessageMediaType>(num.ToString(), out result);
                    switch (result)
                    {
                        case AVIMMessageMediaType.File:
                            AVIMFileMessage typedMessageType1 = this.CurrentClient.CreateDerivedTypedMessageType<AVIMFileMessage>(avimMessage);
                            if (this.m_OnFileMessageReceived == null)
                                break;
                            this.m_OnFileMessageReceived((object)this, typedMessageType1);
                            tuple = new Tuple<bool, string>(true, "m_OnFileMessageReceived");
                            break;
                        case AVIMMessageMediaType.Location:
                            AVIMLocationMessage typedMessageType2 = this.CurrentClient.CreateDerivedTypedMessageType<AVIMLocationMessage>(avimMessage);
                            if (this.m_OnLocationMessageReceived == null)
                                break;
                            this.m_OnLocationMessageReceived((object)this, typedMessageType2);
                            tuple = new Tuple<bool, string>(true, "m_OnLocationMessageReceived");
                            break;
                        case AVIMMessageMediaType.Video:
                            AVIMVideoMessage typedMessageType3 = this.CurrentClient.CreateDerivedTypedMessageType<AVIMVideoMessage>(avimMessage);
                            if (this.m_OnVideoMessageReceived == null)
                                break;
                            this.m_OnVideoMessageReceived((object)this, typedMessageType3);
                            tuple = new Tuple<bool, string>(true, "m_OnVideoMessageReceived");
                            break;
                        case AVIMMessageMediaType.Audio:
                            AVIMAudioMessage typedMessageType4 = this.CurrentClient.CreateDerivedTypedMessageType<AVIMAudioMessage>(avimMessage);
                            if (this.m_OnAudioMessageReceived == null)
                                break;
                            this.m_OnAudioMessageReceived((object)this, typedMessageType4);
                            tuple = new Tuple<bool, string>(true, "m_OnAudioMessageReceived");
                            break;
                        case AVIMMessageMediaType.Image:
                            AVIMImageMessage typedMessageType5 = this.CurrentClient.CreateDerivedTypedMessageType<AVIMImageMessage>(avimMessage);
                            if (this.m_OnImageMessageReceived == null)
                                break;
                            this.m_OnImageMessageReceived((object)this, typedMessageType5);
                            tuple = new Tuple<bool, string>(true, "m_OnImageMessageReceived");
                            break;
                        case AVIMMessageMediaType.Text:
                            AVIMTextMessage typedMessageType6 = this.CurrentClient.CreateDerivedTypedMessageType<AVIMTextMessage>(avimMessage);
                            if (this.m_OnTextMessageReceived == null)
                                break;
                            this.m_OnTextMessageReceived((object)this, typedMessageType6);
                            tuple = new Tuple<bool, string>(true, "m_OnTextMessageReceived");
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    if (this.m_OnTypedMessageReceived == null)
                        return;
                    AVIMDefaultTypedMessage typedMessageType = this.CurrentClient.CreateDerivedTypedMessageType<AVIMDefaultTypedMessage>(avimMessage);
                    typedMessageType.Deserialize(dictionary);
                    if (this.m_OnTypedMessageReceived == null)
                        return;
                    this.m_OnTypedMessageReceived((object)this, (AVIMTypedMessage)typedMessageType);
                    tuple = new Tuple<bool, string>(true, "m_OnTypedMessageReceived");
                }
            }
            catch
            {
                if (this.m_OnMessageReceived == null)
                    return;
                this.m_OnMessageReceived((object)this, avimMessage);
                tuple = new Tuple<bool, string>(true, "m_OnMessageReceived");
            }
            finally
            {
                if (!tuple.Item1 && this.m_OnMessageReceived != null)
                    this.m_OnMessageReceived((object)this, avimMessage);
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

        internal void invokeOnConversationMembersChangedOnConversation(object sender, IDictionary<string, object> e)
        {
            int index = Array.IndexOf<string>(new string[4]
            {
        "joined",
        "left",
        "members-joined",
        "members-left"
            }, AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "op"));
            if (index < 0)
                return;
            Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>[] tupleArray = new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>[6]
            {
        new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>(new AVIMOnMembersChangedEventArgs()
        {
          Conversation = this,
          AffectedType = AVIMConversationEventType.Joined,
          Oprator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy"),
          AffectedMembers = (IList<string>) null
        }, this.m_OnJoined),
        new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>(new AVIMOnMembersChangedEventArgs()
        {
          Conversation = this,
          AffectedType = AVIMConversationEventType.Left,
          Oprator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy"),
          AffectedMembers = (IList<string>) null
        }, this.m_OnLeft),
        new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>(new AVIMOnMembersChangedEventArgs()
        {
          Conversation = this,
          AffectedType = AVIMConversationEventType.MembersJoined,
          Oprator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy"),
          AffectedMembers = AVRMProtocolUtils.CaptureListFromDictionary<string>(e, "m")
        }, (EventHandler<AVIMOnMembersChangedEventArgs>) ((s_m_joined, e_m_joined) =>
        {
          if (e_m_joined.AffectedMembers != null)
          {
            foreach (string str in (IEnumerable<string>) e_m_joined.AffectedMembers)
            {
              if (!this.MemberIds.Contains(str))
                this.MemberIds.Add(str);
            }
          }
          this.m_OnMembersJoined(s_m_joined, e_m_joined);
        })),
        new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>(new AVIMOnMembersChangedEventArgs()
        {
          Conversation = this,
          AffectedType = AVIMConversationEventType.MembersLeft,
          Oprator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy"),
          AffectedMembers = AVRMProtocolUtils.CaptureListFromDictionary<string>(e, "m")
        }, (EventHandler<AVIMOnMembersChangedEventArgs>) ((s_m_left, e_m_left) =>
        {
          if (e_m_left.AffectedMembers != null)
          {
            foreach (string str in (IEnumerable<string>) e_m_left.AffectedMembers)
            {
              if (this.MemberIds.Contains(str))
                this.MemberIds.Remove(str);
            }
          }
          this.m_OnMembersLeft(s_m_left, e_m_left);
        })),
        new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>(new AVIMOnMembersChangedEventArgs()
        {
          Conversation = this,
          AffectedType = AVIMConversationEventType.Invited,
          Oprator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy")
        }, this.m_OnInvited),
        new Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>>(new AVIMOnMembersChangedEventArgs()
        {
          Conversation = this,
          AffectedType = AVIMConversationEventType.Kicked,
          Oprator = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy")
        }, this.m_OnKicked)
            };
            if (index <= -1)
                return;
            string str1 = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "initBy");
            if (index == 0)
            {
                if (str1 != this.CurrentClient.ClientId)
                    index = 2;
            }
            else if (index == 1 && str1 != this.CurrentClient.ClientId)
                index = 3;
            Tuple<AVIMOnMembersChangedEventArgs, EventHandler<AVIMOnMembersChangedEventArgs>> tuple = tupleArray[index];
            if (tuple.Item2 == null)
                return;
            tuple.Item2((object)this, tuple.Item1);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.propertyChanged.Invoke((object)this, new PropertyChangedEventArgs(propertyName));
        }
    }
}