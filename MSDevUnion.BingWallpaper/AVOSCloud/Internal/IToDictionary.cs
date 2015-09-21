using System;
using System.Collections.Generic;

namespace AVOSCloud.Internal
{
	internal interface IToDictionary
	{
		IDictionary<string, object> ToDictionary();
	}
}