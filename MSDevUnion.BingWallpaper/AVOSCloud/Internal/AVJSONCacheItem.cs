using AVOSCloud;
using System;

namespace AVOSCloud.Internal
{
	internal class AVJSONCacheItem
	{
		private readonly string comparisonString;

		public AVJSONCacheItem(object obj)
		{
			try
			{
				this.comparisonString = Json.Encode(AVClient.MaybeEncodeJSONObject(obj, true));
			}
			catch
			{
				this.comparisonString = "";
			}
		}

		public override bool Equals(object obj)
		{
			AVJSONCacheItem aVJSONCacheItem = (AVJSONCacheItem)obj;
			return this.comparisonString.Equals(aVJSONCacheItem.comparisonString);
		}

		public override int GetHashCode()
		{
			return this.comparisonString.GetHashCode();
		}
	}
}