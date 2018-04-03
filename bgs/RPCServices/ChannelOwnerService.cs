using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class ChannelOwnerService : ServiceDescriptor
	{
		public const uint GET_CHANNELID_ID = 1;

		public const uint CREATE_CHANNEL_ID = 2;

		public const uint JOIN_CHANNEL_ID = 3;

		public const uint FIND_CHANNEL_ID = 4;

		public const uint GET_CHANNEL_INFO_ID = 5;

		public const uint SUBSCRIBE_CHANNEL_ID = 6;

		public ChannelOwnerService() : base("bnet.protocol.channel.ChannelOwner")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.channel.ChannelOwner.GetChannelId", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetChannelIdResponse>)), new MethodDescriptor("bnet.protocol.channel.ChannelOwner.CreateChannel", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<CreateChannelResponse>)), new MethodDescriptor("bnet.protocol.channel.ChannelOwner.JoinChannel", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<JoinChannelResponse>)), new MethodDescriptor("bnet.protocol.channel.ChannelOwner.FindChannel", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<FindChannelResponse>)), new MethodDescriptor("bnet.protocol.channel.ChannelOwner.GetChannelInfo", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetChannelInfoResponse>)), new MethodDescriptor("bnet.protocol.channel.ChannelOwner.SubscribeChannel", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SubscribeChannelResponse>)) };
			}
		}
	}
}