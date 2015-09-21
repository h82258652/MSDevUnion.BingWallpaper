using AVOSCloud;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
    internal class AVIMUtils
    {
        internal static void HandlerException(Tuple<string, IDictionary<string, object>> websocketResponse)
        {
            IDictionary<string, object> data = websocketResponse.Item2;
            if ("error".Equals(AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "cmd")))
                throw new AVException((AVException.ErrorCode)AVRMProtocolUtils.CaptureInteger(data, "code"), AVRMProtocolUtils.CaptureValueFromDictionary<string>(data, "reason"));
        }

        internal static AVIMMessage RestoreAVIMMessageFromServer(IDictionary<string, object> e)
        {
            AVIMMessage avimMessage = (AVIMMessage)new AVIMDefaultMessage();
            avimMessage.FromClientId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "fromPeerId");
            avimMessage.Transient = AVRMProtocolUtils.CaptureValueFromDictionary<bool>(e, "transient");
            avimMessage.ConversationId = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "cid");
            AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "peerId");
            avimMessage.MessageBody = e["msg"].ToString();
            avimMessage.Id = AVRMProtocolUtils.CaptureValueFromDictionary<string>(e, "id");
            avimMessage.ServerTimestamp = AVRMProtocolUtils.CaptureValueFromDictionary<long>(e, "timestamp");
            return avimMessage;
        }

        internal static AVIMOnMembersChangedEventArgs GenerateConversationMembersChangedArgs(IDictionary<string, object> e, List<Tuple<object, EventHandler<AVIMOnMembersChangedEventArgs>>> eventHandlers)
        {
            AVIMConversation conversation = new AVIMConversation
            {
                ConversationId = e.CaptureValueFromDictionary<string>("cid")
            };
            AVIMOnMembersChangedEventArgs aVIMOnMembersChangedEventArgs = null;
            GenerateAVIMOnMembersChangedEventArgs[] array = new GenerateAVIMOnMembersChangedEventArgs[]
            {
                (IDictionary<string, object> e1) => new AVIMOnMembersChangedEventArgs
                {
                    Conversation = conversation,
                    AffectedType = AVIMConversationEventType.Joined,
                    Oprator = e.CaptureValueFromDictionary<string>("initBy"),
                    AffectedMembers = null
                },
                (IDictionary<string, object> e2) => new AVIMOnMembersChangedEventArgs
                {
                    Conversation = conversation,
                    AffectedType = AVIMConversationEventType.Left,
                    Oprator = e.CaptureValueFromDictionary<string>("initBy"),
                    AffectedMembers = null
                },
                (IDictionary<string, object> e3) => new AVIMOnMembersChangedEventArgs
                {
                    Conversation = conversation,
                    AffectedType = AVIMConversationEventType.MembersJoined,
                    Oprator = e.CaptureValueFromDictionary<string>("initBy"),
                    AffectedMembers = e.CaptureListFromDictionary<string>("m")
                },
                (IDictionary<string, object> e4) => new AVIMOnMembersChangedEventArgs
                {
                    Conversation = conversation,
                    AffectedType = AVIMConversationEventType.MembersLeft,
                    Oprator = e.CaptureValueFromDictionary<string>("initBy"),
                    AffectedMembers = e.CaptureListFromDictionary<string>("m")
                },
                (IDictionary<string, object> e5) => new AVIMOnMembersChangedEventArgs
                {
                    Conversation = conversation,
                    AffectedType = AVIMConversationEventType.Invited,
                    Oprator = e.CaptureValueFromDictionary<string>("initBy")
                },
                (IDictionary<string, object> e6) => new AVIMOnMembersChangedEventArgs
                {
                    Conversation = conversation,
                    AffectedType = AVIMConversationEventType.Kicked,
                    Oprator = e.CaptureValueFromDictionary<string>("initBy")
                }
            };
            string[] array2 = new string[]
            {
                "joined",
                "left",
                "members-joined",
                "members-left",
                "added",
                "removed"
            };
            string text = e.CaptureValueFromDictionary<string>("op");
            int num = Array.IndexOf<string>(array2, text);
            if (num > 0)
            {
                GenerateAVIMOnMembersChangedEventArgs generateAVIMOnMembersChangedEventArgs = array[num];
                aVIMOnMembersChangedEventArgs = generateAVIMOnMembersChangedEventArgs(e);
                if (eventHandlers != null)
                {
                    Tuple<object, EventHandler<AVIMOnMembersChangedEventArgs>> tuple = eventHandlers[num];
                    if (tuple != null && tuple.Item2 != null)
                    {
                        tuple.Item2.Invoke(eventHandlers[num].Item1, aVIMOnMembersChangedEventArgs);
                    }
                }
            }
            return aVIMOnMembersChangedEventArgs;
        }
    }
}