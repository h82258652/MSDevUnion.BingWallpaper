using AVOSCloud;
using System;

namespace AVOSCloud.Internal
{
	internal interface IAVFieldOperation
	{
		object Apply(object oldValue, AVObject obj, string key);

		object Encode();

		IAVFieldOperation MergeWithPrevious(IAVFieldOperation previous);
	}
}