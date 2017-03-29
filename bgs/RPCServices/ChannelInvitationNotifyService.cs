using bgs;
using System;

namespace bgs.RPCServices
{
	public class ChannelInvitationNotifyService : ServiceDescriptor
	{
		public const uint NOTIFY_RECEIVED_INVITATION_ADDED_ID = 1;

		public const uint NOTIFY_RECEIVED_INVITATION_REMOVED_ID = 2;

		public const uint NOTIFY_RECEIVED_SUGGESTION_ADDED_ID = 3;

		public const uint HAS_ROOM_FOR_INVITATION_ID = 4;

		public ChannelInvitationNotifyService() : base("bnet.protocol.channel_invitation.ChannelInvitationNotify")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationNotify.NotifyReceivedInvitationAdded", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<InvitationAddedNotification>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationNotify.NotifyReceivedInvitationRemoved", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<InvitationRemovedNotification>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationNotify.NotifyReceivedSuggestionAdded", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SuggestionAddedNotification>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationNotify.HasRoomForInvitation", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<HasRoomForInvitationRequest>)) };
			}
		}
	}
}