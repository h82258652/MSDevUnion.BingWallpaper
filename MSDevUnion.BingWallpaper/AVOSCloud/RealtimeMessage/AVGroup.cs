using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
    public class AVGroup
    {
        private string GROUP_TABLE_NAME = "AVOSRealtimeGroups";
        private IAVGroupListener _avGroupListener;

        internal AVSession Session { get; set; }

        internal IAVGroupListener AVGroupListener
        {
            get
            {
                return this._avGroupListener;
            }
            set
            {
                this._avGroupListener = value;
                this.SetListener(value);
            }
        }

        public string SelfId { get; private set; }

        public string GroupId { get; private set; }

        private EventHandler<EventArgs> m_OnJoined { get; set; }

        private EventHandler<EventArgs> m_OnLeft { get; set; }

        private EventHandler<AVGroupMembersJoinedEventArgs> m_OnMembersJoined { get; set; }

        private EventHandler<AVGroupMembersLeftEventArgs> m_OnMembersLeft { get; set; }

        private EventHandler<AVGroupMembersRemovedEventArgs> m_OnMembersRemoved { get; set; }

        private EventHandler<EventArgs> m_OnReject { get; set; }

        private EventHandler<AVGroupMessageSentEventArgs> m_OnGroupMessageSent { get; set; }

        private EventHandler<AVGroupMessageReceivedEventArgs> m_OnMessage { get; set; }

        public event EventHandler<EventArgs> OnJoined
        {
            add
            {
                this.m_OnJoined += value;
            }
            remove
            {
                this.m_OnJoined -= value;
            }
        }

        public event EventHandler<EventArgs> OnLeft
        {
            add
            {
                this.m_OnLeft += value;
            }
            remove
            {
                this.m_OnLeft -= value;
            }
        }

        public event EventHandler<AVGroupMembersJoinedEventArgs> OnMembersJoined
        {
            add
            {
                this.m_OnMembersJoined += value;
            }
            remove
            {
                this.m_OnMembersJoined -= value;
            }
        }

        public event EventHandler<AVGroupMembersLeftEventArgs> OnMembersLeft
        {
            add
            {
                this.m_OnMembersLeft += value;
            }
            remove
            {
                this.m_OnMembersLeft -= value;
            }
        }

        private event EventHandler<AVGroupMembersRemovedEventArgs> OnMembersRemoved
        {
            add
            {
                this.m_OnMembersRemoved += value;
            }
            remove
            {
                this.m_OnMembersRemoved -= value;
            }
        }

        public event EventHandler<EventArgs> OnReject
        {
            add
            {
                this.m_OnReject += value;
            }
            remove
            {
                this.m_OnReject -= value;
            }
        }

        public event EventHandler<AVGroupMessageSentEventArgs> OnGroupMessageSent
        {
            add
            {
                this.m_OnGroupMessageSent += value;
            }
            remove
            {
                this.m_OnGroupMessageSent -= value;
            }
        }

        public event EventHandler<AVGroupMessageReceivedEventArgs> OnMessage
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

        public AVGroup()
        {
        }

        internal AVGroup(string selfId, string groupId, AVSession session)
          : this(selfId, session)
        {
            this.GroupId = groupId;
        }

        internal AVGroup(string selfId, AVSession session)
          : this()
        {
            this.SelfId = selfId;
            this.Session = session;
            this.Session.OnGroupAction += new EventHandler<AVRtcGroupEventArgs>(this.Session_OnGroupAction);
        }

        internal AVGroup(string selfId, string groupId, AVSession session, EventHandler<EventArgs> onJoined, EventHandler<EventArgs> onLeft, EventHandler<AVGroupMembersJoinedEventArgs> onMembersJoined, EventHandler<AVGroupMembersLeftEventArgs> onMembersLeft)
          : this(selfId, groupId, session)
        {
            if (onJoined != null)
                this.OnJoined += onJoined;
            if (onLeft != null)
                this.OnLeft += onLeft;
            if (onMembersJoined != null)
                this.OnMembersJoined += onMembersJoined;
            if (onMembersLeft == null)
                return;
            this.OnMembersLeft += onMembersLeft;
        }

        public void Join()
        {
            this.Join((EventHandler<EventArgs>)null);
        }

        public void Join(EventHandler<EventArgs> onJoined)
        {
            this.Session.CreateGroup(this);
            if (onJoined == null)
                return;
            this.OnJoined += onJoined;
        }

        public void AddMembers(IList<string> peerIds, EventHandler<AVGroupMembersJoinedEventArgs> onMembersJoined)
        {
            if (onMembersJoined != null)
                this.OnMembersJoined += onMembersJoined;
            this.Session.AddPeersToGroup(this, peerIds);
        }

        public void RemoveMembers(IList<string> peerIds, EventHandler<AVGroupMembersRemovedEventArgs> onMembersRemoved)
        {
            if (onMembersRemoved != null)
                this.m_OnMembersRemoved += onMembersRemoved;
            this.Session.RemovedPeersFromGroup(this, peerIds);
        }

        public void SendMessage(AVMessage message, EventHandler<AVGroupMessageSentEventArgs> onGroupMessageSent, EventHandler<AVGroupMessageReceivedEventArgs> onGroupMessageRecevied)
        {
            if (onGroupMessageSent != null)
            {
                this.OnGroupMessageSent += onGroupMessageSent;
                this.Session.OnMessageSent += (this.Session_OnMessageSent);
            }
            if (onGroupMessageRecevied != null)
            {
                this.OnMessage += onGroupMessageRecevied;
                this.Session.OnMessage += (this.Session_OnMessage);
            }
            this.Session.SendMessage(this, message);
        }

        public void SendMessage(string msg, EventHandler<AVGroupMessageSentEventArgs> onGroupMessageSent, EventHandler<AVGroupMessageReceivedEventArgs> onGroupMessageRecevied)
        {
            AVMessage message = new AVMessage();
            message.FromPeerId = this.Session.SelfId;
            message.GroupId = this.GroupId;
            string str = Guid.NewGuid().ToString();
            message.localId = str;
            message.Message = msg;
            message.Timestamp = AVPersistence.GetCurrentUnixTimestampFromDateTime();
            this.SendMessage(message, onGroupMessageSent, onGroupMessageRecevied);
        }

        public void SendMessage(string msg)
        {
            this.SendMessage(msg, (EventHandler<AVGroupMessageSentEventArgs>)null, (EventHandler<AVGroupMessageReceivedEventArgs>)null);
        }

        public void Quit(EventHandler<EventArgs> onLeft)
        {
            if (onLeft != null)
                this.OnLeft += onLeft;
            this.Session.LeftFromGroup(this);
        }

        private void Session_OnGroupAction(object sender, AVRtcGroupEventArgs e)
        {
            AVGroupOp groupOp = e.GroupOp;
            int index = Array.IndexOf<string>(new string[7]
            {
        "joined",
        "left",
        "reject",
        "members-joined",
        "members-left",
        "invited",
        "kicked"
            }, groupOp.op);
            Action<AVGroupOp>[] actionArray = new Action<AVGroupOp>[7]
            {
        new Action<AVGroupOp>(this.GroupOp_Join),
        new Action<AVGroupOp>(this.GroupOp_Left),
        new Action<AVGroupOp>(this.GroupOp_Reject),
        new Action<AVGroupOp>(this.GroupOp_MembersJoined),
        new Action<AVGroupOp>(this.GroupOp_MembersLeft),
        new Action<AVGroupOp>(this.GroupOp_Invited),
        new Action<AVGroupOp>(this.GroupOp_Removed)
            };
            if (index <= -1)
                return;
            Action<AVGroupOp> action = actionArray[index];
            if (action == null)
                return;
            action(groupOp);
        }

        private void GroupOp_Join(AVGroupOp op)
        {
            this.GroupId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(op.data, "roomId");
            if (this.m_OnJoined == null)
                return;
            this.m_OnJoined((object)this, EventArgs.Empty);
        }

        private void GroupOp_MembersJoined(AVGroupOp op)
        {
            AVGroupMembersJoinedEventArgs e = new AVGroupMembersJoinedEventArgs();
            e.JoinedPeerIds = AVRMProtocolUtils.CaptureListFromDictionary<string>(op.data, "roomPeerIds");
            if (this.m_OnMembersJoined == null)
                return;
            this.m_OnMembersJoined((object)this, e);
        }

        private void GroupOp_Left(AVGroupOp op)
        {
            if (this.m_OnLeft == null)
                return;
            this.m_OnLeft((object)this, EventArgs.Empty);
        }

        private void GroupOp_MembersLeft(AVGroupOp op)
        {
            AVGroupMembersLeftEventArgs e = new AVGroupMembersLeftEventArgs();
            e.LeftPeerIds = AVRMProtocolUtils.CaptureListFromDictionary<string>(op.data, "roomPeerIds");
            if (this.m_OnMembersLeft == null)
                return;
            this.m_OnMembersLeft((object)this, e);
        }

        private void GroupOp_Reject(AVGroupOp op)
        {
            if (this.m_OnReject == null)
                return;
            this.m_OnReject((object)this, EventArgs.Empty);
        }

        private void GroupOp_Invited(AVGroupOp op)
        {
        }

        private void GroupOp_Removed(AVGroupOp op)
        {
            AVGroupMembersRemovedEventArgs e = new AVGroupMembersRemovedEventArgs();
            e.RemovedBy = AVRMProtocolUtils.CaptureValueFromDictionary<string>(op.data, "byPeerId");
            if (this.m_OnMembersRemoved == null)
                return;
            this.m_OnMembersRemoved((object)this, e);
        }

        private void Session_OnJoined(object sender, EventArgs e)
        {
            if (this.AVGroupListener == null || this.AVGroupListener.OnJoined == null)
                return;
            this.AVGroupListener.OnJoined(this, this.SelfId);
        }

        private void Session_OnMessageSent(object sender, AVMessageSentEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.GroupId) || !(e.Message.GroupId == this.GroupId) || this.m_OnGroupMessageSent == null)
                return;
            this.m_OnGroupMessageSent((object)this, new AVGroupMessageSentEventArgs()
            {
                Message = e.Message
            });
        }

        private void Session_OnMessage(object sender, AVMessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.GroupId) || !(e.Message.GroupId == this.GroupId) || this.m_OnMessage == null)
                return;
            this.m_OnMessage((object)this, new AVGroupMessageReceivedEventArgs()
            {
                Message = e.Message
            });
        }

        internal void SetListener(IAVGroupListener avGroupListener)
        {
            this.AVGroupListener = avGroupListener;
        }
    }
}