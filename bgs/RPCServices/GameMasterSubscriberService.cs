using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class GameMasterSubscriberService : ServiceDescriptor
	{
		public const uint NOTIFY_FACTORY_UPDATE_ID = 1;

		public GameMasterSubscriberService() : base("bnet.protocol.game_master.GameMasterSubscriber")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.game_master.GameMasterSubscriber.NotifyFactoryUpdate", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<FactoryUpdateNotification>)) };
			}
		}
	}
}