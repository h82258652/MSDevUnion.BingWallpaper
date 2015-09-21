using AVOSCloud;
using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web;

namespace AVOSCloud.RealtimeMessage
{
    internal class RealtimeMessageHooks : IRealtimeMessage, IRealtimeMessagePlatformHook
    {
        internal readonly string RTC_Router_Server = "http://router.g0.push.avoscloud.com/v1/route?appId={0}&secure=1";
        private const int timeoutMs = 10000;
        internal string chatServerUrl;
        internal MessageWebSocket webSocketClient;
        private DataWriter messageWriter;
        private EventHandler<AVIMEventArgs> m_OnAskReqGet;
        private bool listend;
        private EventHandler<IDictionary<string, object>> m_onRecevied;

        public bool connected { get; set; }

        public SocketStatus status { get; set; }

        private EventHandler<AVIMEventArgs> m_OnMessage { get; set; }

        private EventHandler<AVIMEventArgs> m_OnMessageSent { get; set; }

        private EventHandler<AVIMEventArgs> m_OnPresenceChanged { get; set; }

        private EventHandler<AVIMEventArgs> m_OnSessionChanged { get; set; }

        private EventHandler<AVIMEventArgs> m_OnGroupAction { get; set; }

        private EventHandler<WebSoceketOpenEventArgs> m_OnWebSocketOpend { get; set; }

        private EventHandler<WebSoceketCloseEventArgs> m_OnWebSocketClosed { get; set; }

        private EventHandler<WebSocketConnectedFaildArgs> m_OnWebSocketConnectedFaild { get; set; }

        public event EventHandler<AVIMEventArgs> OnMessage
        {
            add { this.m_OnMessage += value; }
            remove { this.m_OnMessage -= value; }
        }

        public event EventHandler<AVIMEventArgs> OnMessageSent
        {
            add { this.m_OnMessageSent += value; }
            remove { this.m_OnMessageSent -= value; }
        }

        public event EventHandler<AVIMEventArgs> OnPresenceChanged
        {
            add { this.m_OnPresenceChanged += value; }
            remove { this.m_OnPresenceChanged -= value; }
        }

        public event EventHandler<AVIMEventArgs> OnSessionChanged
        {
            add { this.m_OnSessionChanged += value; }
            remove { this.m_OnSessionChanged -= value; }
        }

        public event EventHandler<AVIMEventArgs> OnGroupAction
        {
            add { this.m_OnGroupAction += value; }
            remove { this.m_OnGroupAction -= value; }
        }

        public event EventHandler<WebSoceketOpenEventArgs> OnWebSocketOpend
        {
            add { this.m_OnWebSocketOpend += value; }
            remove { this.m_OnWebSocketOpend -= value; }
        }

        public event EventHandler<WebSoceketCloseEventArgs> OnWebSocketClosed
        {
            add { this.m_OnWebSocketClosed += value; }
            remove { this.m_OnWebSocketClosed -= value; }
        }

        public event EventHandler<WebSocketConnectedFaildArgs> OnWebSocketConnectedFaild
        {
            add { this.m_OnWebSocketConnectedFaild += value; }
            remove { this.m_OnWebSocketConnectedFaild -= value; }
        }

        public event EventHandler<IDictionary<string, object>> OnReceived
        {
            add
            {
                this.m_onRecevied += value;
                if (this.listend)
                    return;
                this.BeginListen();
            }
            remove { this.m_onRecevied -= value; }
        }

        public RealtimeMessageHooks()
        {
            this.m_OnAskReqGet = (EventHandler<AVIMEventArgs>) ((sender, e) => { });
        }

        public void OpenSession(AVSession session, IList<string> watchPeerIds)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "session");
            cmdBody.Add("op", (object) "open");
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            if (watchPeerIds != null)
                cmdBody.Add("sessionPeerIds", (object) watchPeerIds);
            if (this.connected)
            {
                this.checkSignature(session, watchPeerIds, cmdBody);
            }
            else
            {
                this.OnWebSocketOpend +=
                    (EventHandler<WebSoceketOpenEventArgs>)
                        ((sender, e) => this.checkSignature(session, watchPeerIds, cmdBody));
                this.Initialize();
            }
        }

        public void CloseSession(AVSession session)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "session");
            cmdBody.Add("op", (object) "close");
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            this.SendCMD(cmdBody);
        }

        public void SendMessage(AVSession session, AVMessage message)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "direct");
            cmdBody.Add("msg", (object) message.Message);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("toPeerIds", (object) message.ToPeerIds);
            cmdBody.Add("transient", (message.IsTransient ? 1 : 0));
            cmdBody.Add("id", (object) message.localId);
            this.SendCMD(cmdBody);
        }

        public void SendMessage(AVSession session, AVGroup group, AVMessage message)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "direct");
            cmdBody.Add("msg", (object) message.Message);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            cmdBody.Add("roomId", (object) group.GroupId);
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("fromPeerId", (object) session.SelfId);
            cmdBody.Add("id", (object) message.localId);
            this.SendCMD(cmdBody);
        }

        public void WatchPeers(AVSession session, IList<string> peerIds)
        {
            string key = Guid.NewGuid().ToString();
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "session");
            cmdBody.Add("op", (object) "add");
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            cmdBody.Add("i", (object) key);
            cmdBody.Add("sessionPeerIds", (object) peerIds);
            this.SendCMD(cmdBody);
            AVSessionOp avSessionOp = new AVSessionOp()
            {
                id = key,
                op = "added"
            };
            avSessionOp.data = (IDictionary<string, object>) new Dictionary<string, object>();
            avSessionOp.data.Add("ToAddPeerIds", (object) peerIds);
            session.pendingSessionChanged.Add(key, avSessionOp);
        }

        public void UnWatchPeers(AVSession session, IList<string> peerIds)
        {
            string key = Guid.NewGuid().ToString();
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "session");
            cmdBody.Add("op", (object) "remove");
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            cmdBody.Add("i", (object) key);
            cmdBody.Add("sessionPeerIds", (object) peerIds);
            this.SendCMD(cmdBody);
            AVSessionOp avSessionOp = new AVSessionOp()
            {
                id = key,
                op = "removed"
            };
            avSessionOp.data = (IDictionary<string, object>) new Dictionary<string, object>();
            avSessionOp.data.Add("ToRemovePeerIds", (object) peerIds);
            session.pendingSessionChanged.Add(key, avSessionOp);
        }

        public void CreateGroup(AVSession session)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "room");
            cmdBody.Add("op", (object) "join");
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            this.checkGroupSinature(session, string.Empty, (IList<string>) null, "join", cmdBody);
        }

        public void JoinGroup(AVSession session, AVGroup group)
        {
            this.checkGroupSinature(session, group.GroupId, (IList<string>) null, "join",
                (IDictionary<string, object>) new Dictionary<string, object>()
                {
                    {
                        "cmd",
                        (object) "room"
                    },
                    {
                        "op",
                        (object) "join"
                    },
                    {
                        "peerId",
                        (object) session.SelfId
                    },
                    {
                        "roomId",
                        (object) group.GroupId
                    },
                    {
                        "appId",
                        (object) AVClient.ApplicationId
                    }
                });
        }

        public void AddPeersToGroup(AVSession session, AVGroup group, IList<string> peerIds)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "room");
            cmdBody.Add("op", (object) "invite");
            if (!string.IsNullOrEmpty(group.GroupId))
                cmdBody.Add("roomId", (object) group.GroupId);
            cmdBody.Add("roomPeerIds", (object) peerIds);
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            this.checkGroupSinature(session, group.GroupId == null ? string.Empty : group.GroupId, peerIds, "invite",
                cmdBody);
        }

        public void RemovePeersFromGroup(AVSession session, AVGroup group, IList<string> peerIds)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "room");
            cmdBody.Add("op", (object) "kick");
            if (!string.IsNullOrEmpty(group.GroupId))
                cmdBody.Add("roomId", (object) group.GroupId);
            cmdBody.Add("roomPeerIds", (object) peerIds);
            cmdBody.Add("peerId", (object) session.SelfId);
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            this.checkGroupSinature(session, group.GroupId == null ? string.Empty : group.GroupId, peerIds, "kick",
                cmdBody);
        }

        public void LeftFromGroup(AVSession session, AVGroup group)
        {
            this.checkGroupSinature(session, group.GroupId, (IList<string>) null, "leave",
                (IDictionary<string, object>) new Dictionary<string, object>()
                {
                    {
                        "cmd",
                        (object) "room"
                    },
                    {
                        "op",
                        (object) "leave"
                    },
                    {
                        "peerId",
                        (object) session.SelfId
                    },
                    {
                        "roomId",
                        (object) group.GroupId
                    },
                    {
                        "appId",
                        (object) AVClient.ApplicationId
                    }
                });
        }

        private async Task Send(string data)
        {
            messageWriter.WriteString(data);
            var dataWriterStoreOperation =await messageWriter.StoreAsync();

        }
        public Task<Tuple<string, IDictionary<string, object>>> EAP2TAP4WebSocket(IDictionary<string, object> cmd)
        {
            if (this.status != SocketStatus.SocketConnected)
                return
                    this.InitializeAsync()
                        .ContinueWith(
                            t => this.EAP2TAP4WebSocket(cmd)).Unwrap();
            TaskCompletionSource<Tuple<string, IDictionary<string, object>>> tcs =
                new TaskCompletionSource<Tuple<string, IDictionary<string, object>>>();
            new CancellationTokenSource(10000).Token.Register((Action) (() => tcs.TrySetCanceled()), false);
            string str = AVClient.SerializeJsonString(cmd);
                Send(str);
            TypedEventHandler<MessageWebSocket, MessageWebSocketMessageReceivedEventArgs> callBack =  null;
            callBack = (TypedEventHandler<MessageWebSocket, MessageWebSocketMessageReceivedEventArgs>) ((s, e) =>
            {
                IDictionary<string, object> dictionary = this.CaptureResponseByCMDId(cmd["i"].ToString(), e);
                if (dictionary == null)
                    return;
                tcs.TrySetResult(new Tuple<string, IDictionary<string, object>>(string.Empty, dictionary));
                this.webSocketClient.MessageReceived-=callBack;
            });
            this.webSocketClient.MessageReceived += callBack;
            return tcs.Task;
        }

        private void WebSocketClient_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            //OnOpen();
            try
            {
                var dataStream = args.GetDataStream();
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    //GetOnMessage(read);
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        private string GetOnMessage(MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                var dataStream = args.GetDataStream();
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    return read;
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
            }
            return String.Empty;
        }


        private void BeginListen()
        {
            this.listend = true;
            this.webSocketClient.MessageReceived += webSocketClient_MessageReceivedV2;
                //new EventHandler<MessageReceivedEventArgs>(this.webSocketClient_MessageReceivedV2));
        }

        private void webSocketClient_MessageReceivedV2(object sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            try
            {
                var dataStream = e.GetDataStream();
                using (DataReader reader = e.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    IDictionary<string, object> e1 = AVClient.DeserializeJsonString(read);
                    if (this.m_onRecevied == null)
                        return;
                    this.m_onRecevied((object)this, e1);
                }
               
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
        }

        private IDictionary<string, object> CaptureResponseByCMDId(string cmdId, MessageWebSocketMessageReceivedEventArgs e)
        {
            IDictionary<string, object> dictionary = null;
            IDictionary<string, object> data = AVClient.DeserializeJsonString(GetOnMessage(e));
            if (AVRMProtocolUtils.CaptureValueFromDictionary<long>(data, "i").ToString() == cmdId)
                dictionary = data;
            return dictionary;
        }

        internal void Initialize()
        {
            this.RquestGet(string.Format(this.RTC_Router_Server, (object) AVClient.ApplicationId), "GET",
                (IDictionary<string, object>) null, (AsyncCallback) (ar =>
                {
                    this.chatServerUrl = this.GetChatServer(ar);
                    if (!string.IsNullOrEmpty(this.chatServerUrl))
                    {
                        this.ConnectChatServer(this.chatServerUrl);
                    }
                    else
                    {
                        WebSocketConnectedFaildArgs e = new WebSocketConnectedFaildArgs();
                        e.Error = new AVIMError()
                        {
                            Code = AVException.ErrorCode.CurrentClientNetworkIsNotAvailable
                        };
                        if (this.m_OnWebSocketConnectedFaild == null)
                            return;
                        this.m_OnWebSocketConnectedFaild((object) this, e);
                    }
                }));
        }

        internal Task<bool> InitializeAsync()
        {
            string serverUrl = string.Format(this.RTC_Router_Server, (object) AVClient.ApplicationId);
            return
                InternalExtensions.OnSuccess(
                    AVClient.platformHooks.RequestAsync(new Uri(serverUrl), "GET", null, null, string.Empty,
                        CancellationToken.None), t =>
                        {
                            serverUrl = AVClient.DeserializeJsonString(t.Result.Item2)["server"].ToString();
                            webSocketClient = new MessageWebSocket();
                            webSocketClient.ConnectAsync(new Uri(serverUrl)).GetResults();
                            webSocketClient.Closed += WebSocketClient_Closed;
                            //this.webSocketClient = new WebSocket(serverUrl, "", null, null, "", "", (WebSocketVersion) - 1);
                            //this.webSocketClient.Open();
                            //this.webSocketClient.add_Closed(new EventHandler(this.webSocketClient_Closed));
                            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                            //EventHandler callback = (EventHandler)null;
                            //callback = (EventHandler)((s, e) =>
                            //{
                            //    tcs.TrySetResult(true);
                            //    this.status = SocketStatus.SocketConnected;
                            //    this.webSocketClient.remove_Opened(callback);
                            //});
                            //this.webSocketClient.add_Opened(callback);
                            tcs.TrySetResult(true);
                            return tcs.Task.Result;
                        });
        }

        private void WebSocketClient_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            if (this.m_OnWebSocketClosed != null)
                this.m_OnWebSocketClosed((object) this, new WebSoceketCloseEventArgs());
            this.connected = false;
            this.status = SocketStatus.SocketDisconnected;
        }

        public void RquestGet(string uri, string method, IDictionary<string, object> data, AsyncCallback callback)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
            httpWebRequest.Method = method;
            httpWebRequest.BeginGetResponse(callback, (object) httpWebRequest);
        }

        private string GetChatServer(IAsyncResult asynchronousResult)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) asynchronousResult.AsyncState;
            HttpWebResponse httpWebResponse;
            try
            {
                httpWebResponse = (HttpWebResponse) httpWebRequest.EndGetResponse(asynchronousResult);
            }
            catch (WebException ex)
            {
                httpWebResponse = (HttpWebResponse) ex.Response;
            }
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8, true);
            string jsonData = streamReader.ReadToEnd();
            //responseStream.Close();
            //streamReader.Close();
            //httpWebResponse.Close();
            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                return string.Empty;
            IDictionary<string, object> dictionary = AVClient.DeserializeJsonString(jsonData);
            if (!dictionary.Keys.Contains("server"))
                return "";
            else
                return dictionary["server"].ToString();
        }

        //private void webSocketClient_Error(object sender, ErrorEventArgs e)
        //{
        //}

        private void webSocketClient_Opened(object sender, EventArgs e)
        {
            this.m_OnWebSocketOpend((object) this, new WebSoceketOpenEventArgs());
        }

        private void webSocketClient_MessageReceived(object sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            string[] array = new string[6]
            {
                "session",
                "direct",
                "presence",
                "ackreq",
                "ack",
                "room"
            };
            EventHandler<AVIMEventArgs>[] eventHandlerArray = new EventHandler<AVIMEventArgs>[6]
            {
                this.m_OnSessionChanged,
                this.m_OnMessage,
                this.m_OnPresenceChanged,
                this.m_OnAskReqGet,
                this.m_OnMessageSent,
                this.m_OnGroupAction
            };
            RealtimeMessageHooks.CreateAVSessionEventArgs[] sessionEventArgsArray = new RealtimeMessageHooks.
                CreateAVSessionEventArgs[6]
            {
                new RealtimeMessageHooks.CreateAVSessionEventArgs(this.ProcessAVSession),
                new RealtimeMessageHooks.CreateAVSessionEventArgs(this.ProcessAVMessage),
                new RealtimeMessageHooks.CreateAVSessionEventArgs(this.ProcessAVPresence),
                new RealtimeMessageHooks.CreateAVSessionEventArgs(this.ProcessAckReq),
                new RealtimeMessageHooks.CreateAVSessionEventArgs(this.ProcessAck),
                new RealtimeMessageHooks.CreateAVSessionEventArgs(this.ProcessAVGroupOp)
            };
            IDictionary<string, object> data = AVClient.DeserializeJsonString(GetOnMessage(e));
            try
            {
                string str = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "cmd");
                int index = Array.IndexOf<string>(array, str);
                if (index <= -1)
                    return;
                EventHandler<AVIMEventArgs> eventHandler = eventHandlerArray[index];
                AVIMEventArgs e1 = sessionEventArgsArray[index](data);
                if (eventHandler == null)
                    return;
                eventHandler((object) this, e1);
            }
            catch
            {
            }
        }

        private AVIMEventArgs ProcessAVSession(IDictionary<string, object> data)
        {
            AVIMEventArgs avimEventArgs = new AVIMEventArgs();
            AVSessionOp avSessionOp = new AVSessionOp();
            string str = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "op");
            avSessionOp.id = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "i");
            avSessionOp.op = str;
            avSessionOp.data = data;
            avimEventArgs.SessionOp = avSessionOp;
            return avimEventArgs;
        }

        private AVIMEventArgs ProcessAckReq(IDictionary<string, object> data)
        {
            string str1 = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "peerId");
            string str2 = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "c");
            AVAckReq avAckReq = new AVAckReq()
            {
                MsgCount = str2,
                PeerId = str1
            };
            return new AVIMEventArgs()
            {
                AckReq = avAckReq
            };
        }

        private AVIMEventArgs ProcessAck(IDictionary<string, object> data)
        {
            string str1 = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "id");
            string str2 = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "peerId");
            AVAck avAck = new AVAck()
            {
                PeerId = str2,
                MsgId = str1
            };
            return new AVIMEventArgs()
            {
                Ack = avAck
            };
        }

        private SessionStatus ConvertSessionStatus(string statusText)
        {
            return (SessionStatus) Array.IndexOf<string>(new string[5]
            {
                "NAN",
                "opened",
                "added",
                "removed",
                "closed"
            }, statusText);
        }

        private AVIMEventArgs ProcessAVMessage(IDictionary<string, object> data)
        {
            AVMessage avMessage = new AVMessage();
            avMessage.FromPeerId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "fromPeerId");
            avMessage.IsTransient = AVRMProtocolUtils.CaptureValueFromDictionary<bool>(data, "transient");
            avMessage.Message = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "msg");
            avMessage.ToPeerIds = (IList<string>) new List<string>();
            string peerId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "peerId");
            avMessage.ToPeerIds.Add(peerId);
            avMessage.GroupId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "roomId");
            avMessage.Id = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "id");
            avMessage.Timestamp = AVRMProtocolUtils.CaptureValueFromDictionary<long>(data, "timestamp");
            this.SendAckCMD(peerId, avMessage.Id);
            return new AVIMEventArgs()
            {
                Message = avMessage
            };
        }

        private AVIMEventArgs ProcessAVPresence(IDictionary<string, object> data)
        {
            AVPresence avPresence = new AVPresence();
            avPresence.PeerId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "peerId");
            avPresence.Status = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "status");
            avPresence.SessionPeerIds = new List<string>();
            foreach (object obj in AVRMProtocolUtils.CaptureValueFromDictionary<List<object>>(data, "sessionPeerIds"))
                avPresence.SessionPeerIds.Add(obj.ToString());
            return new AVIMEventArgs()
            {
                Presence = avPresence
            };
        }

        private AVIMEventArgs ProcessAVGroupOp(IDictionary<string, object> data)
        {
            return new AVIMEventArgs()
            {
                GroupOp = new AVGroupOp()
                {
                    data = data,
                    groupId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "roomId"),
                    op = AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "op")
                }
            };
        }

        private void webSocketClient_Closed(object sender, EventArgs e)
        {
            if (this.m_OnWebSocketClosed != null)
                this.m_OnWebSocketClosed((object) this, new WebSoceketCloseEventArgs());
            this.connected = false;
            this.status = SocketStatus.SocketDisconnected;
        }

        //private void webSocketClient_DataReceived(object sender, DataReceivedEventArgs e)
        //{
        //}

        internal void SendCMD(IDictionary<string, object> cmdBody)
        {
            try
            {
                string str = AVClient.SerializeJsonString(cmdBody);
                this.Send(str);
            }
            catch
            {
            }
        }

        internal void SendAckCMD(string peerId, string id)
        {
            IDictionary<string, object> cmdBody = (IDictionary<string, object>) new Dictionary<string, object>();
            cmdBody.Add("cmd", (object) "ack");
            cmdBody.Add("appId", (object) AVClient.ApplicationId);
            cmdBody.Add("peerId", (object) peerId);
            cmdBody.Add("ids", (object) new List<string>()
            {
                id
            });
            this.SendCMD(cmdBody);
        }

        internal async void ConnectChatServer(string serverUrl)
        {
            //this.webSocketClient = new WebSocket(serverUrl, "", (List<KeyValuePair<string, string>>) null,
            //    (List<KeyValuePair<string, string>>) null, "", "", (WebSocketVersion) - 1);
            webSocketClient=new MessageWebSocket();
            await webSocketClient.ConnectAsync(new Uri(serverUrl));
            this.connected = true;
            //this.webSocketClient.add_Opened(new EventHandler(this.webSocketClient_Opened));
            //this.webSocketClient.add_DataReceived(
            //    new EventHandler<DataReceivedEventArgs>(this.webSocketClient_DataReceived));
            this.webSocketClient.Closed+=WebSocketClient_Closed;
            this.webSocketClient.MessageReceived+=this.webSocketClient_MessageReceived;
            //this.webSocketClient.add_Error(new EventHandler<ErrorEventArgs>(this.webSocketClient_Error));
            //this.webSocketClient.Open();
        }

        private void RTCUtils_OnWebSocketOpend(object sender, WebSoceketOpenEventArgs e)
        {
        }

        private void checkSignature(AVSession session, IList<string> watchPeerIds, IDictionary<string, object> cmdBody)
        {
            if (session.SignatureFactory != null)
            {
                session.SignatureFactory.CreateSignature(session.SelfId, watchPeerIds)
                    .ContinueWith((Action<Task<AVIMSignature>>) (x =>
                    {
                        AVIMSignature result = x.Result;
                        if (result == null)
                            return;
                        cmdBody.Add("s", (object) result.SignatureContent);
                        cmdBody.Add("t", (object) result.Timestamp);
                        cmdBody.Add("n", (object) result.Nonce);
                    })).ContinueWith((Action<Task>) (y => this.SendCMD(cmdBody)));
            }
            else
            {
                cmdBody.Add("s", (object) "");
                this.SendCMD(cmdBody);
            }
        }

        private void checkGroupSinature(AVSession session, string groupId, IList<string> targetPeerIds, string action,
            IDictionary<string, object> cmdBody)
        {
            if (session.SignatureFactory != null)
                session.SignatureFactory.CreateGroupSignature(groupId, session.SelfId, targetPeerIds, action)
                    .ContinueWith((Action<Task<AVIMSignature>>) (x =>
                    {
                        AVIMSignature result = x.Result;
                        if (result == null)
                            return;
                        cmdBody.Add("s", (object) result.SignatureContent);
                        cmdBody.Add("t", (object) result.Timestamp);
                        cmdBody.Add("n", (object) result.Nonce);
                    })).ContinueWith((Action<Task>) (y => this.SendCMD(cmdBody)));
            else
                this.SendCMD(cmdBody);
        }

        internal delegate AVIMEventArgs CreateAVSessionEventArgs(IDictionary<string, object> data);

        internal delegate AVSessionOp CreateAVSessionOp(IDictionary<string, object> data);
    }
}