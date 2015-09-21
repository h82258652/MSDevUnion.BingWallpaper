using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
    public abstract class AVIMMessage
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly IDictionary<string, object> serverData = (IDictionary<string, object>)new Dictionary<string, object>();

        public string ConversationId { get; set; }

        public string FromClientId { get; set; }

        public string Id { get; set; }

        public bool Receipt { get; set; }

        public bool Transient { get; set; }

        public virtual string MessageBody { get; set; }

        public AVIMMessageStatus MessageStatus { get; set; }

        public AVIMMessageIOType MessageIOType { get; set; }

        public long ServerTimestamp { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string cmdId { get; set; }
    }
}