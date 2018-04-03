using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class ChannelSubscriberService : ServiceDescriptor
	{
		public const uint NOTIFY_ADD_ID = 1;

		public const uint NOTIFY_JOIN_ID = 2;

		public const uint NOTIFY_REMOVE_ID = 3;

		public const uint NOTIFY_LEAVE_ID = 4;

		public const uint NOTIFY_SEND_MESSAGE_ID = 5;

		public const uint NOTIFY_UPDATE_CHANNEL_STATE_ID = 6;

		public const uint NOTIFY_UPDATE_MEMBER_STATE_ID = 7;

		public ChannelSubscriberService() : base("bnet.protocol.channel.ChannelSubscriber")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifyAdd", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<AddNotification>)), new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifyJoin", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<JoinNotification>)), new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifyRemove", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<RemoveNotification>)), new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifyLeave", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<LeaveNotification>)), new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifySendMessage", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SendMessageNotification>)), new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifyUpdateChannelState", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<UpdateChannelStateNotification>)), new MethodDescriptor("bnet.protocol.channel.ChannelSubscriber.NotifyUpdateMemberState", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<UpdateMemberStateNotification>)) };
			}
		}
	}
}