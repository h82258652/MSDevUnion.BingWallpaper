using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class AVAnalyticEvent : IToDictionary
	{
		public IDictionary<string, object> attributes
		{
			get;
			set;
		}

		public long du
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public string sessionId
		{
			get;
			set;
		}

		public bool stop
		{
			get;
			set;
		}

		public string tag
		{
			get;
			set;
		}

		public long ts
		{
			get;
			set;
		}

		internal AVAnalyticEvent()
		{
		}

		public IDictionary<string, object> ToDictionary()
		{
            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["du"] = this.du;
            dictionary["name"] = this.name;
            dictionary["sessionId"] = this.sessionId;
            dictionary["tag"] = this.tag;
            dictionary["ts"] = this.ts;
            return dictionary;
        }
	}
}