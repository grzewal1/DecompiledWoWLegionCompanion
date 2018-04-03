using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class NotificationService : ServiceDescriptor
	{
		public const uint SEND_NOTIFICATION_ID = 1;

		public const uint REGISTER_CLIENT_ID = 2;

		public const uint UNREGISTER_CLIENT_ID = 3;

		public const uint FIND_CLIENT_ID = 4;

		public NotificationService() : base("bnet.protocol.notification.NotificationService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.notification.NotificationService.SendNotification", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.notification.NotificationService.RegisterClient", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.notification.NotificationService.UnregisterClient", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.notification.NotificationService.FindClient", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<FindClientResponse>)) };
			}
		}
	}
}