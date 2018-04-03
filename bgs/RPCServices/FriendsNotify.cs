using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class FriendsNotify : ServiceDescriptor
	{
		public const uint NOTIFY_FRIEND_ADDED = 1;

		public const uint NOTIFY_FRIEND_REMOVED = 2;

		public const uint NOTIFY_RECEIVED_INVITATION_ADDED = 3;

		public const uint NOTIFY_RECEIVED_INVITATION_REMOVED = 4;

		public const uint NOTIFY_SENT_INVITATION_ADDED = 5;

		public const uint NOTIFY_SENT_INVITATION_REMOVED = 6;

		public const uint NOTIFY_UPDATE_FRIEND_STATE = 7;

		public FriendsNotify() : base("bnet.protocol.friends.FriendsNotify")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifyFriendAdded", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<FriendNotification>)), new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifyFriendRemoved", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<FriendNotification>)), new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifyReceivedInvitationAdded", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<InvitationNotification>)), new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifyReceivedInvitationRemoved", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<InvitationNotification>)), new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifySentInvitationAdded", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<InvitationNotification>)), new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifySentInvitationRemoved", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<InvitationNotification>)), new MethodDescriptor("bnet.protocol.friends.FriendsNotify.NotifyUpdateFriendState", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<UpdateFriendStateNotification>)) };
			}
		}
	}
}