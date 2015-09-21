using AVOSCloud;
using System;
using System.Collections.Generic;

namespace AVOSCloud.Internal
{
	internal class AVDeleteOperation : IAVFieldOperation
	{
		internal readonly static object DeleteToken;

		private static AVDeleteOperation _Instance;

		public static AVDeleteOperation Instance
		{
			get
			{
				return _Instance;
			}
		}

		static AVDeleteOperation()
		{
			DeleteToken = new object();
			_Instance = new AVDeleteOperation();
		}

		private AVDeleteOperation()
		{
		}

		public object Apply(object oldValue, AVObject obj, string key)
		{
			return DeleteToken;
		}

		public object Encode()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__op", "Delete");
			return dictionary;
		}

		public IAVFieldOperation MergeWithPrevious(IAVFieldOperation previous)
		{
			return this;
		}
	}
}