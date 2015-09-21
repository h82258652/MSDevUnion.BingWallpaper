using System;
using System.Collections.Generic;

namespace AVOSCloud.RealtimeMessage
{
	internal delegate void StatusChanged(AVSession session, List<string> peerIds);
}