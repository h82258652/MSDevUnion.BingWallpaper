using AVOSCloud;
using AVOSCloud.Internal;
using System;
using System.Collections.Generic;

namespace AVOSCloud.RealtimeMessage
{
    public class AVSession
    {
        internal readonly object mutex = new object();
        internal IRealtimeMessage rtc;
        private IAVSessionListener _avSessionListener;

        public string SelfId { get; set; }

        public IAVSessionListener AVSessionListener
        {
            get
            {
                return this._avSessionListener;
            }
            set
            {
                this._avSessionListener = value;
                this.SetListener(value);
            }
        }

        private IDictionary<string, AVMessage> pendingMeaasge { get; set; }

        internal IDictionary<string, AVSessionOp> pendingSessionChanged { get; set; }

        internal IDictionary<string, IDictionary<string, object>> pendingcmd { get; set; }

        public ISignatureFactory SignatureFactory { get; set; }

        public SessionStatus Status { get; set; }

        public List<string> AllPeerIds { get; set; }

        public List<string> OnlinePeerIds { get; set; }

        private EventHandler<EventArgs> m_OnSessionOpen { get; set; }

        private EventHandler<EventArgs> m_OnSessionPaused { get; set; }

        private EventHandler<EventArgs> m_OnSessionResumed { get; set; }

        private EventHandler<EventArgs> m_OnSessionClosed { get; set; }

        private EventHandler<AVRtcGroupEventArgs> m_OnGroupAction { get; set; }

        private EventHandler<AVMessageReceivedEventArgs> m_OnMessage { get; set; }

        private EventHandler<AVMessageSentEventArgs> m_OnMessageSent { get; set; }

        private EventHandler<AVMessageSentFailedEventArgs> m_OnMessageFailed { get; set; }

        public event EventHandler<EventArgs> OnSessionOpen
        {
            add
            {
                this.m_OnSessionOpen += value;
            }
            remove
            {
                this.m_OnSessionOpen -= value;
            }
        }

        public event EventHandler<EventArgs> OnSessionPaused
        {
            add
            {
                this.m_OnSessionPaused += value;
            }
            remove
            {
                this.m_OnSessionPaused -= value;
            }
        }

        public event EventHandler<EventArgs> OnSessionResumed
        {
            add
            {
                this.m_OnSessionResumed += value;
            }
            remove
            {
                this.m_OnSessionResumed -= value;
            }
        }

        public event EventHandler<EventArgs> OnSessionClosed
        {
            add
            {
                this.m_OnSessionClosed += value;
            }
            remove
            {
                this.m_OnSessionClosed -= value;
            }
        }

        internal event EventHandler<AVRtcGroupEventArgs> OnGroupAction
        {
            add
            {
                this.m_OnGroupAction += value;
            }
            remove
            {
                this.m_OnGroupAction -= value;
            }
        }

        public event EventHandler<AVMessageReceivedEventArgs> OnMessage
        {
            add
            {
                this.m_OnMessage += value;
            }
            remove
            {
                this.m_OnMessage -= value;
            }
        }

        public event EventHandler<AVMessageSentEventArgs> OnMessageSent
        {
            add
            {
                this.m_OnMessageSent += value;
            }
            remove
            {
                this.m_OnMessageSent -= value;
            }
        }

        public event EventHandler<AVMessageSentFailedEventArgs> OnMessageFailed
        {
            add
            {
                this.m_OnMessageFailed += value;
            }
            remove
            {
                this.m_OnMessageFailed -= value;
            }
        }

        internal AVSession()
        {
        }

        public AVSession(string selfId)
          : this()
        {
            this.SelfId = selfId;
            this.pendingMeaasge = (IDictionary<string, AVMessage>)new Dictionary<string, AVMessage>();
            this.pendingSessionChanged = (IDictionary<string, AVSessionOp>)new Dictionary<string, AVSessionOp>();
            this.pendingcmd = (IDictionary<string, IDictionary<string, object>>)new Dictionary<string, IDictionary<string, object>>();
            this.Initialize();
        }

        internal AVSession(string selfId, Action action)
          : this(selfId)
        {
            this.Open();
            this.OnSessionOpen += (EventHandler<EventArgs>)((s, e) => action());
        }

        public AVSession(string selfId, IAVSessionListener avSessionListener)
          : this(selfId)
        {
            this.SetListener(avSessionListener);
        }

        internal void Initialize()
        {
            this.rtc = Activator.CreateInstance(Type.GetType("AVOSCloud.RealtimeMessage.RealtimeMessageHooks")) as IRealtimeMessage;
            this.rtc.OnSessionChanged += new EventHandler<AVIMEventArgs>(this.rtc_OnSessionChanged);
            this.rtc.OnMessage += new EventHandler<AVIMEventArgs>(this.rtc_OnMessage);
            this.rtc.OnMessageSent += new EventHandler<AVIMEventArgs>(this.rtc_OnMessageSent);
            this.rtc.OnWebSocketClosed += new EventHandler<WebSoceketCloseEventArgs>(this.rtc_OnWebSocketClosed);
            this.rtc.OnPresenceChanged += new EventHandler<AVIMEventArgs>(this.rtc_OnPresenceChanged);
            this.rtc.OnWebSocketConnectedFaild += new EventHandler<WebSocketConnectedFaildArgs>(this.rtc_OnWebSocketConnectedFaild);
            this.rtc.OnGroupAction += new EventHandler<AVIMEventArgs>(this.rtc_OnGroupAction);
        }

        public void Open(string selfId, IList<string> watchPeerIds)
        {
            if (string.IsNullOrEmpty(this.SelfId))
                this.SelfId = selfId;
            this.Open(watchPeerIds);
        }

        public void Open()
        {
            this.Open(string.Empty);
        }

        public void Open(string watchPeerId)
        {
            IList<string> watchPeerIds = (IList<string>)new List<string>();
            if (!string.IsNullOrEmpty(watchPeerId))
                watchPeerIds.Add(watchPeerId);
            else
                watchPeerIds = (IList<string>)null;
            this.Open(watchPeerIds);
        }

        public void Open(IList<string> watchPeerIds)
        {
            this.rtc.OpenSession(this, watchPeerIds);
        }

        public void Close()
        {
            this.rtc.CloseSession(this);
        }

        public void SendMessage(string msg, IList<string> toPeers, bool transient, EventHandler<AVMessageReceivedEventArgs> onMessage)
        {
            if (onMessage != null)
                this.m_OnMessage += onMessage;
            if (this.Status == SessionStatus.Opend)
            {
                AVMessage message = new AVMessage();
                string key = Guid.NewGuid().ToString();
                message.localId = key;
                message.Message = msg;
                message.Timestamp = AVPersistence.GetCurrentUnixTimestampFromDateTime();
                message.FromPeerId = this.SelfId;
                message.ToPeerIds = toPeers;
                lock (this.mutex)
                  this.pendingMeaasge.Add(key, message);
                this.rtc.SendMessage(this, message);
            }
            else
            {
                if (this._avSessionListener.OnMessageFailure == null)
                    return;
                this._avSessionListener.OnMessageFailure(this, new AVMessage()
                {
                    Message = msg,
                    FromPeerId = this.SelfId,
                    ToPeerIds = toPeers
                }, new AVIMError()
                {
                    Code = AVException.ErrorCode.CanNotSendMessageOnSessionNotOpen
                });
            }
        }

        public void SendMessage(string msg, string toPeer, bool transient)
        {
            this.SendMessage(msg, toPeer, transient, (EventHandler<AVMessageReceivedEventArgs>)null);
        }

        public void SendMessage(string msg, string toPeer, bool transient, EventHandler<AVMessageReceivedEventArgs> onMessage)
        {
            this.SendMessage(msg, (IList<string>)new List<string>()
      {
        toPeer
      }, transient, onMessage);
        }

        public void SetListener(IAVSessionListener avSessionListener)
        {
            this._avSessionListener = avSessionListener;
        }

        private void rtc_OnGroupAction(object sender, AVIMEventArgs e)
        {
            this.m_OnGroupAction((object)this, new AVRtcGroupEventArgs()
            {
                GroupOp = e.GroupOp
            });
        }

        private void rtc_OnPresenceChanged(object sender, AVIMEventArgs e)
        {
            string[] array = new string[2]
            {
        "on",
        "off"
            };
            StatusChanged[] statusChangedArray = new StatusChanged[2]
            {
        new StatusChanged(this.OnPeerOnline),
        new StatusChanged(this.OnPeerOffline)
            };
            int index = Array.IndexOf<string>(array, e.Presence.Status);
            if (index <= -1)
                return;
            statusChangedArray[index](this, e.Presence.SessionPeerIds);
        }

        private void OnPeerOnline(AVSession session, List<string> peerIds)
        {
            lock (this.mutex)
            {
                foreach (string item_0 in peerIds)
                {
                    if (!this.OnlinePeerIds.Contains(item_0))
                        this.OnlinePeerIds.Add(item_0);
                }
            }
            if (this.AVSessionListener.OnStatusOnline == null)
                return;
            this.AVSessionListener.OnStatusOnline(session, (IList<string>)peerIds);
        }

        private void OnPeerOffline(AVSession session, List<string> peerIds)
        {
            lock (this.mutex)
            {
                foreach (string item_0 in peerIds)
                {
                    if (this.OnlinePeerIds.Contains(item_0))
                        this.OnlinePeerIds.Remove(item_0);
                }
            }
            if (this.AVSessionListener.OnStatusOffline == null)
                return;
            this.AVSessionListener.OnStatusOffline(session, (IList<string>)peerIds);
        }

        public void Reconnect()
        {
            if (this.Status == SessionStatus.Opend)
                return;
            this.Initialize();
        }

        public void WatchPeer(string peerId)
        {
            this.WatchPeers((IList<string>)new List<string>()
      {
        peerId
      });
        }

        public void WatchPeers(IList<string> peerIds)
        {
            this.rtc.WatchPeers(this, peerIds);
        }

        public void UnWatchPeer(string peerId)
        {
            this.UnWatchPeers((IList<string>)new List<string>()
      {
        peerId
      });
        }

        public void UnWatchPeers(IList<string> peerIds)
        {
            this.rtc.UnWatchPeers(this, peerIds);
        }

        public AVGroup GetGroup()
        {
            return new AVGroup(this.SelfId, this);
        }

        public AVGroup GetGroup(string groupId)
        {
            return new AVGroup(this.SelfId, groupId, this);
        }

        internal void CreateGroup(AVGroup localGroup)
        {
            this.DoActionAfterSessionOpen((Action)(() => this.rtc.CreateGroup(this)));
        }

        internal void JoinGroup(AVGroup group)
        {
            this.DoActionAfterSessionOpen((Action)(() => this.rtc.CreateGroup(this)));
        }

        internal void AddPeersToGroup(AVGroup group, IList<string> peerIds)
        {
            this.DoActionAfterSessionOpen((Action)(() => this.rtc.AddPeersToGroup(this, group, peerIds)));
        }

        internal void RemovedPeersFromGroup(AVGroup group, IList<string> peerIds)
        {
            this.DoActionAfterSessionOpen((Action)(() => this.rtc.RemovePeersFromGroup(this, group, peerIds)));
        }

        public void SendMessage(AVGroup group, AVMessage message)
        {
            this.DoActionAfterSessionOpen((Action)(() =>
            {
                lock (this.mutex)
                  this.pendingMeaasge.Add(message.localId, message);
                this.rtc.SendMessage(this, group, message);
            }));
        }

        public void LeftFromGroup(AVGroup group)
        {
            this.DoActionAfterSessionOpen((Action)(() => this.rtc.LeftFromGroup(this, group)));
        }

        internal void DoActionAfterSessionOpen(Action action)
        {
            if (this.Status == SessionStatus.None || this.Status == SessionStatus.Closed)
            {
                this.Open();
                this.OnSessionOpen += (EventHandler<EventArgs>)((s, e) => action());
            }
            else
                action();
        }

        internal void DoActionAfterJoinedGroup(AVGroup group, Action action)
        {
            this.DoActionAfterSessionOpen((Action)(() => group.Join((EventHandler<EventArgs>)((s, e) => action()))));
        }

        private void rtc_OnWebSocketClosed(object sender, WebSoceketCloseEventArgs e)
        {
            this.Status = SessionStatus.Paused;
            if (this.m_OnSessionClosed != null)
                this.m_OnSessionClosed((object)this, (EventArgs)e);
            if (this.AVSessionListener == null || this.AVSessionListener.OnSessionClosed == null)
                return;
            this.AVSessionListener.OnSessionClosed(this);
        }

        private void rtc_OnMessageSent(object sender, AVIMEventArgs e)
        {
            lock (this.mutex)
            {
                AVMessage local_1 = this.pendingMeaasge[e.Ack.MsgId];
                if (this.m_OnMessageSent != null)
                    this.m_OnMessageSent((object)this, new AVMessageSentEventArgs()
                    {
                        Message = local_1
                    });
                if (this.AVSessionListener == null || this.AVSessionListener.OnMessageSent == null)
                    return;
                this.AVSessionListener.OnMessageSent(this, local_1);
            }
        }

        private void rtc_OnWebSocketConnectedFaild(object sender, WebSocketConnectedFaildArgs e)
        {
            if (this.AVSessionListener == null || this.AVSessionListener.OnError == null)
                return;
            this.AVSessionListener.OnError(this, e.Error);
        }

        private void rtc_OnSessionChanged(object sender, AVIMEventArgs e)
        {
            AVSessionOp sessionOp = e.SessionOp;
            Action<AVSessionOp> action = new Action<AVSessionOp>[4]
            {
        new Action<AVSessionOp>(this.SessionOp_Open),
        new Action<AVSessionOp>(this.SessionOp_Added),
        new Action<AVSessionOp>(this.SessionOp_Removed),
        new Action<AVSessionOp>(this.SessionOp_Closed)
            }[Array.IndexOf<string>(new string[4]
            {
        "opened",
        "added",
        "removed",
        "closed"
            }, sessionOp.op)];
            if (action == null)
                return;
            action(sessionOp);
        }

        private void SessionOp_Open(AVSessionOp op)
        {
            if (this.Status == SessionStatus.Paused)
            {
                this.Status = SessionStatus.Opend;
                if (this.m_OnSessionResumed != null)
                    this.m_OnSessionResumed((object)this, EventArgs.Empty);
            }
            this.Status = SessionStatus.Opend;
            if (this.m_OnSessionOpen != null)
                this.m_OnSessionOpen((object)this, EventArgs.Empty);
            this.OnlinePeerIds = op.data["onlineSessionPeerIds"] as List<string>;
            this.AllPeerIds = this.OnlinePeerIds;
            if (this.OnlinePeerIds == null)
            {
                this.OnlinePeerIds = new List<string>();
                this.AllPeerIds = new List<string>();
            }
            if (this.AVSessionListener == null || this.AVSessionListener.OnSessionOpen == null)
                return;
            this.AVSessionListener.OnSessionOpen(this);
        }

        private void SessionOp_Added(AVSessionOp op)
        {
            lock (this.mutex)
            {
                if (this.AVSessionListener == null)
                    return;
                AVSessionOp local_0 = this.pendingSessionChanged[op.id];
                if (local_0 == null)
                    return;
                IList<string> local_1 = local_0.data["ToAddPeerIds"] as IList<string>;
                if (local_1 != null)
                {
                    foreach (string item_0 in (IEnumerable<string>)local_1)
                    {
                        if (!this.AllPeerIds.Contains(item_0))
                            this.AllPeerIds.Add(item_0);
                    }
                }
                if (this.AVSessionListener.OnPeersWatched == null)
                    return;
                this.AVSessionListener.OnPeersWatched(this, local_1);
            }
        }

        private void SessionOp_Removed(AVSessionOp op)
        {
            lock (this.mutex)
            {
                if (this.AVSessionListener == null)
                    return;
                AVSessionOp local_0 = this.pendingSessionChanged[op.id];
                if (local_0 == null)
                    return;
                IList<string> local_1 = local_0.data["ToRemovePeerIds"] as IList<string>;
                if (local_1 != null)
                {
                    foreach (string item_0 in (IEnumerable<string>)local_1)
                    {
                        if (this.AllPeerIds.Contains(item_0))
                            this.AllPeerIds.Remove(item_0);
                    }
                }
                if (this.AVSessionListener.OnPeersUnwatched == null)
                    return;
                this.AVSessionListener.OnPeersUnwatched(this, local_1);
            }
        }

        private void SessionOp_Closed(AVSessionOp op)
        {
            this.Status = SessionStatus.Closed;
            if (this.AVSessionListener == null)
                return;
            this.AVSessionListener.OnSessionClosed(this);
        }

        private void rtc_OnMessage(object sender, AVIMEventArgs e)
        {
            if (this.m_OnMessage != null)
                this.m_OnMessage((object)this, new AVMessageReceivedEventArgs()
                {
                    Message = e.Message
                });
            if (this.AVSessionListener == null)
                return;
            this.AVSessionListener.OnMessage(this, e.Message);
        }
    }
}
