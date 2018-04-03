using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class ChallengeService : ServiceDescriptor
	{
		public const uint CHALLENGED_PICKED = 1;

		public const uint CHALLENGED_ANSWERED = 2;

		public const uint CHALLENGED_CANCELLED = 3;

		public ChallengeService() : base("bnet.protocol.challenge.ChallengeService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.challenge.ChallengeService.ChallengePicked", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ChallengePickedResponse>)), new MethodDescriptor("bnet.protocol.challenge.ChallengeService.ChallengeAnswered", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ChallengeAnsweredResponse>)), new MethodDescriptor("bnet.protocol.challenge.ChallengeService.ChallengeCancelled", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)) };
			}
		}
	}
}