using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVOSCloud.RealtimeMessageV2
{
	public interface ISignatureFactoryV2
	{
		Task<AVIMSignatureV2> CreateConnectSignature(string clientId);

		Task<AVIMSignatureV2> CreateConversationSignature(string conversationId, string clientId, IList<string> targetIds, string action);

		Task<AVIMSignatureV2> CreateQueryHistorySignature(string clientId, string conversationId);

		Task<AVIMSignatureV2> CreateStartConversationSignature(string clientId, IList<string> targetIds);
	}
}