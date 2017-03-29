using bgs;
using System;

namespace bgs.RPCServices
{
	public class PresenceService : ServiceDescriptor
	{
		public const uint SUBSCRIBE_ID = 1;

		public const uint UNSUBSCRIBE_ID = 2;

		public const uint UPDATE_ID = 3;

		public const uint QUERY_ID = 4;

		public PresenceService() : base("bnet.protocol.presence.PresenceService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.presence.PresenceService.Subscribe", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.presence.PresenceService.Unsubscribe", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.presence.PresenceService.Update", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.presence.PresenceService.Query", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)) };
			}
		}
	}
}