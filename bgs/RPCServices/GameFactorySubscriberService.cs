using bgs;
using System;

namespace bgs.RPCServices
{
	public class GameFactorySubscriberService : ServiceDescriptor
	{
		public const uint NOTIFY_GAME_FOUND_ID = 1;

		public GameFactorySubscriberService() : base("bnet.protocol.game_master.GameFactorySubscriber")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.game_master.GameFactorySubscriber.NotifyGameFound", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameFoundNotification>)) };
			}
		}
	}
}