using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class ChannelService : ServiceDescriptor
	{
		public const uint ADD_MEMBER_ID = 1;

		public const uint REMOVE_MEMBER_ID = 2;

		public const uint SEND_MESSAGE_ID = 3;

		public const uint UPDATE_CHANNEL_STATE_ID = 4;

		public const uint UPDATE_MEMBER_STATE_ID = 5;

		public const uint DISSOLVE_ID = 6;

		public const uint SETROLES_ID = 7;

		public const uint UNSUBSCRIBE_MEMBER_ID = 8;

		public ChannelService() : base("bnet.protocol.channel.Channel")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.channel.Channel.AddMember", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.RemoveMember", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.SendMessage", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.UpdateChannelState", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.UpdateMemberState", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.Dissolve", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.AddMember", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.channel.Channel.UnsubscribeMember", 8, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)) };
			}
		}
	}
}