using bgs;
using System;

namespace bgs.RPCServices
{
	public class NotificationListenerService : ServiceDescriptor
	{
		public const uint ON_NOTIFICATION_REC_ID = 1;

		public NotificationListenerService() : base("bnet.protocol.notification.NotificationListener")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.notification.NotificationListener.OnNotificationReceived", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<Notification>)) };
			}
		}
	}
}