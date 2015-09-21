using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessage
{
	public interface ISignatureFactory
	{
		Task<AVIMSignature> CreateGroupSignature(string groupId, string peerId, IList<string> targetPeerIds, string action);

		Task<AVIMSignature> CreateSignature(string peerId, IList<string> watchIds);
	}
}