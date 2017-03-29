using bgs;
using System;

namespace bgs.RPCServices
{
	public class ChannelInvitationService : ServiceDescriptor
	{
		public const uint SUBSCRIBE_ID = 1;

		public const uint UNSUBSCRIBE_ID = 2;

		public const uint SEND_INVITATION_ID = 3;

		public const uint ACCEPT_INVITATION_ID = 4;

		public const uint DECLINE_INVITATION_ID = 5;

		public const uint REVOKE_INVITATION_ID = 6;

		public const uint SUGGEST_INVITATION_ID = 7;

		public const uint INCREMENT_CHANNEL_COUNT_ID = 8;

		public const uint DECREMENT_CHANNEL_COUNT_ID = 9;

		public const uint UPDATE_CHANNEL_COUNT_ID = 10;

		public const uint LIST_CHANNEL_COUNT_ID = 11;

		public ChannelInvitationService() : base("bnet.protocol.channel_invitation.ChannelInvitationService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.Subscribe", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SubscribeResponse>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.Unsubscribe", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.SendInvitation", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SendInvitationResponse>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.AcceptInvitation", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<AcceptInvitationResponse>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.DeclineInvitation", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.RevokeInvitation", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.SuggestInvitation", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.IncrementChannelCount", 8, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<IncrementChannelCountResponse>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.DecrementChannelCount", 9, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.UpdateChannelCount", 10, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel_invitation.ChannelInvitationService.ListChannelCount", 11, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ListChannelCountResponse>)) };
			}
		}
	}
}