using bgs;
using System;

namespace bgs.RPCServices
{
	public class FriendsService : ServiceDescriptor
	{
		public const uint SUBSCRIBE_TO_FRIENDS = 1;

		public const uint SEND_INVITATION = 2;

		public const uint ACCEPT_INVITATION = 3;

		public const uint REVOKE_INVITATION = 4;

		public const uint DECLINE_INVITATION = 5;

		public const uint IGNORE_INVITATION = 6;

		public const uint ASSIGN_ROLE = 7;

		public const uint REMOVE_FRIEND = 8;

		public const uint VIEW_FRIENDS = 9;

		public const uint UPDATE_FRIEND_STATE = 10;

		public const uint UNSUBSCRIBE_TO_FRIENDS = 11;

		public const uint REVOKE_ALL_INVITATIONS = 12;

		public FriendsService() : base("bnet.protocol.friends.FriendsService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.friends.FriendsService.SubscribeToFriends", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SubscribeToFriendsResponse>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.SendInvitation", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.AcceptInvitation", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.RevokeInvitation", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.DeclineInvitation", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.IgnoreInvitation", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.AssignRole", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.RemoveFriend", 8, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GenericFriendResponse>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.ViewFriends", 9, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ViewFriendsResponse>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.UpdateFriendState", 10, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.UnsubscribeToFriends", 11, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.friends.FriendsService.RevokeAllInvitations", 12, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)) };
			}
		}
	}
}