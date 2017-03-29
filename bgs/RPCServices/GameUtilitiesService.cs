using bgs;
using System;

namespace bgs.RPCServices
{
	public class GameUtilitiesService : ServiceDescriptor
	{
		public const uint PROCESS_CLIENT_REQUEST_ID = 1;

		public const uint PRESENCE_CHANNEL_CREATED_ID = 2;

		public const uint GET_PLAYER_VARIABLES_ID = 3;

		public const uint GET_LOAD_ID = 5;

		public const uint PROCESS_SERVER_REQUEST_ID = 6;

		public const uint NOTIFY_GAME_ACCT_ONLINE_ID = 7;

		public const uint NOTIFY_GAME_ACCT_OFFLINE_ID = 8;

		public const uint GET_ALL_VALUES_FOR_ATTRIBUTE_ID = 10;

		public GameUtilitiesService() : base("bnet.protocol.game_utilities.GameUtilities")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.ProcessClientRequest", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ClientResponse>)), new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.PresenceChannelCreated", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.GetPlayerVariables", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetPlayerVariablesResponse>)), null, new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.GetLoad", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ServerState>)), new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.ProcessServerRequest", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ServerResponse>)), new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.NotifyGameAccountOnline", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.NotifyGameAccountOffline", 8, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), null, new MethodDescriptor("bnet.protocol.game_utilities.GameUtilities.GetAllValuesForAttribute", 10, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetAllValuesForAttributeResponse>)) };
			}
		}
	}
}