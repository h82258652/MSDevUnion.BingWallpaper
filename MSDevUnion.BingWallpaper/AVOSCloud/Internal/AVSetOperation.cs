using AVOSCloud;
using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class AVSetOperation : IAVFieldOperation
	{
		public object Value
		{
			get;
			private set;
		}

		public AVSetOperation(object value)
		{
			this.Value = value;
		}

		public object Apply(object oldValue, AVObject obj, string key)
		{
			return this.Value;
		}

		public object Encode()
		{
			return AVClient.MaybeEncodeJSONObject(this.Value, true);
		}

		public IAVFieldOperation MergeWithPrevious(IAVFieldOperation previous)
		{
			return this;
		}
	}
}