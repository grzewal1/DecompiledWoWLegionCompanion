using bnet.protocol.attribute;
using bnet.protocol.notification;
using System;
using System.Collections.Generic;

namespace bgs
{
	public class NotificationAPI : BattleNetAPI
	{
		private List<BnetNotification> m_notifications = new List<BnetNotification>();

		public NotificationAPI(BattleNetCSharp battlenet) : base(battlenet, "Notification")
		{
		}

		public void ClearNotifications()
		{
			this.m_notifications.Clear();
		}

		public int GetNotificationCount()
		{
			return this.m_notifications.Count;
		}

		public void GetNotifications([Out] BnetNotification[] Notifications)
		{
			this.m_notifications.CopyTo(Notifications, 0);
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		public void OnNotification(string notificationType, Notification notification)
		{
			if (notification.AttributeCount <= 0)
			{
				return;
			}
			BnetNotification bnetNotification = new BnetNotification(notificationType);
			SortedDictionary<string, int> strs = new SortedDictionary<string, int>();
			int length = 0;
			bnetNotification.MessageType = 0;
			bnetNotification.MessageSize = 0;
			for (int i = 0; i < notification.AttributeCount; i++)
			{
				bnet.protocol.attribute.Attribute item = notification.Attribute[i];
				if (item.Name == "message_type")
				{
					bnetNotification.MessageType = (int)item.Value.IntValue;
				}
				else if (item.Name == "message_size")
				{
					bnetNotification.MessageSize = (int)item.Value.IntValue;
				}
				else if (item.Name.StartsWith("fragment_"))
				{
					length = length + (int)item.Value.BlobValue.Length;
					strs.Add(item.Name, i);
				}
			}
			if (bnetNotification.MessageType == 0)
			{
				BattleNet.Log.LogError(string.Format("Missing notification type {0} of size {1}", bnetNotification.MessageType, bnetNotification.MessageSize));
				return;
			}
			if (0 < length)
			{
				bnetNotification.BlobMessage = new byte[length];
				SortedDictionary<string, int>.Enumerator enumerator = strs.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					byte[] blobValue = notification.Attribute[enumerator.Current.Value].Value.BlobValue;
					Array.Copy(blobValue, 0, bnetNotification.BlobMessage, num, (int)blobValue.Length);
					num = num + (int)blobValue.Length;
				}
			}
			if (bnetNotification.MessageSize == length)
			{
				this.m_notifications.Add(bnetNotification);
				return;
			}
			BattleNet.Log.LogError(string.Format("Message size mismatch for notification type {0} - {1} != {2}", bnetNotification.MessageType, bnetNotification.MessageSize, length));
		}
	}
}