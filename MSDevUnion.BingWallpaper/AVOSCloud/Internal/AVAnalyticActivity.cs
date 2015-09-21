using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class AVAnalyticActivity : IToDictionary
	{
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

		public long ts
		{
			get;
			set;
		}

		internal AVAnalyticActivity()
		{
		}

		public IDictionary<string, object> ToDictionary()
		{
            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["du"] = this.du;
            dictionary["name"] = this.name;
            dictionary["ts"] = this.ts;
            return dictionary;
        }
	}
}