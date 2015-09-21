using AVOSCloud;
using AVOSCloud.Internal;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessageV2
{
    public class AVIMConversationQuery : AVQueryBase<AVIMConversationQuery, AVIMConversation>
    {
        internal AVIMClient CurrentClient { get; set; }

        internal AVIMConversationQuery(AVIMClient _currentClient)
        {
            this.CurrentClient = _currentClient;
        }

        private AVIMConversationQuery(AVIMConversationQuery source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes)
          : base((AVQueryBase<AVIMConversationQuery, AVIMConversation>)source, where, replacementOrderBy, thenBy, skip, limit, includes)
        {
        }

        internal override AVIMConversationQuery CreateInstance(AVIMConversationQuery source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes)
        {
            return new AVIMConversationQuery(this, where, replacementOrderBy, thenBy, skip, limit, includes)
            {
                CurrentClient = this.CurrentClient
            };
        }

        internal override AVIMConversationQuery CreateInstance(AVQueryBase<AVIMConversationQuery, AVIMConversation> source, IDictionary<string, object> where, IEnumerable<string> replacementOrderBy, IEnumerable<string> thenBy, int? skip, int? limit, IEnumerable<string> includes)
        {
            return new AVIMConversationQuery(this, where, replacementOrderBy, thenBy, skip, limit, includes)
            {
                CurrentClient = this.CurrentClient
            };
        }

        public override Task<IEnumerable<AVIMConversation>> FindAsync()
        {
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, IEnumerable<AVIMConversation>>(this.CurrentClient.OpenThenSendAsync(this.BuildCMDBody()), (Func<Task<Tuple<string, IDictionary<string, object>>>, IEnumerable<AVIMConversation>>)(t =>
            {
                IDictionary<string, object> dictionary = t.Result.Item2;
                IList<AVIMConversation> list1 = (IList<AVIMConversation>)new List<AVIMConversation>();
                IList<object> list2 = dictionary["results"] as IList<object>;
                if (list2 != null)
                {
                    foreach (object obj in (IEnumerable<object>)list2)
                    {
                        IDictionary<string, object> data = obj as IDictionary<string, object>;
                        if (data != null)
                        {
                            AVIMConversation avimConversation = new AVIMConversation()
                            {
                                CurrentClient = this.CurrentClient
                            };
                            avimConversation.MergeMagicFields(data);
                            list1.Add(avimConversation);
                        }
                    }
                }
                return (IEnumerable<AVIMConversation>)list1;
            }));
        }

        internal IDictionary<string, object> BuildCMDBody()
        {
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
            int nextCmdId = AVIMCommon.NextCmdId;
            dictionary1.Add("cmd", (object)"conv");
            dictionary1.Add("op", (object)"query");
            dictionary1.Add("peerId", (object)this.CurrentClient.ClientId);
            dictionary1.Add("i", (object)nextCmdId);
            dictionary1.Add("appId", (object)AVClient.ApplicationId);
            IDictionary<string, object> dictionary2 = this.BuildParameters(false);
            if (dictionary2 != null)
            {
                if (dictionary2.Keys.Contains("where"))
                    dictionary1.Add("where", dictionary2["where"]);
                if (dictionary2.Keys.Contains("skip"))
                    dictionary1.Add("skip", dictionary2["skip"]);
                if (dictionary2.Keys.Contains("limit"))
                    dictionary1.Add("limit", dictionary2["limit"]);
                if (dictionary2.Keys.Contains("sort"))
                    dictionary1.Add("sort", dictionary2["sort"]);
            }
            return (IDictionary<string, object>)dictionary1;
        }

        public override Task<int> CountAsync()
        {
            IDictionary<string, object> cmd = this.BuildCMDBody();
            cmd["limit"] = (object)0;
            cmd["count"] = (object)1;
            return InternalExtensions.OnSuccess<Tuple<string, IDictionary<string, object>>, int>(this.CurrentClient.OpenThenSendAsync(cmd), (Func<Task<Tuple<string, IDictionary<string, object>>>, int>)(t =>
            {
                IDictionary<string, object> data = t.Result.Item2;
                List<AVIMConversation> list = new List<AVIMConversation>();
                return AVRMProtocolUtils.CaptureInteger(data, "count");
            }));
        }

        public override Task<AVIMConversation> GetAsync(string objectId)
        {
            return InternalExtensions.OnSuccess<IEnumerable<AVIMConversation>, AVIMConversation>(this.CurrentClient.GetQuery().WhereEqualTo("objectId", (object)objectId).FindAsync(), (Func<Task<IEnumerable<AVIMConversation>>, AVIMConversation>)(t =>
            {
                AVIMConversation avimConversation = (AVIMConversation)null;
                if (Enumerable.Count<AVIMConversation>(t.Result) > 0)
                    avimConversation = Enumerable.FirstOrDefault<AVIMConversation>(t.Result);
                return avimConversation;
            }));
        }
    }
}