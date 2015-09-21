using System;
using System.Collections.Generic;

namespace AVOSCloud.RealtimeMessage
{
	internal interface IRealtimeMessage
	{
		void AddPeersToGroup(AVSession session, AVGroup group, IList<string> peerIds);

		void CloseSession(AVSession session);

		void CreateGroup(AVSession session);

		void JoinGroup(AVSession session, AVGroup group);

		void LeftFromGroup(AVSession session, AVGroup group);

		void OpenSession(AVSession session, IList<string> peerIds);

		void RemovePeersFromGroup(AVSession session, AVGroup group, IList<string> peerIds);

		void SendMessage(AVSession session, AVMessage message);

		void SendMessage(AVSession session, AVGroup group, AVMessage message);

		void UnWatchPeers(AVSession session, IList<string> peerIds);

		void WatchPeers(AVSession session, IList<string> peerIds);

		event EventHandler<AVIMEventArgs> OnGroupAction;

		event EventHandler<AVIMEventArgs> OnMessage;

		event EventHandler<AVIMEventArgs> OnMessageSent;

		event EventHandler<AVIMEventArgs> OnPresenceChanged;

		event EventHandler<AVIMEventArgs> OnSessionChanged;

		event EventHandler<WebSoceketCloseEventArgs> OnWebSocketClosed;

		event EventHandler<WebSocketConnectedFaildArgs> OnWebSocketConnectedFaild;

		event EventHandler<WebSoceketOpenEventArgs> OnWebSocketOpend;
	}
}