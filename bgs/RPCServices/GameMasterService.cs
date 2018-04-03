using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class GameMasterService : ServiceDescriptor
	{
		public const uint JOIN_GAME_ID = 1;

		public const uint LIST_FACTORIES_ID = 2;

		public const uint FIND_GAME_ID = 3;

		public const uint CANCEL_GAME_ENTRY_ID = 4;

		public const uint GAME_ENDED_ID = 5;

		public const uint PLAYER_LEFT_ID = 6;

		public const uint REGISTER_SERVER_ID = 7;

		public const uint UNREGISTER_SERVER_ID = 8;

		public const uint REGISTER_UTILITIES_ID = 9;

		public const uint UNREGISTER_UTILITIES_ID = 10;

		public const uint SUBSCRIBE_ID = 11;

		public const uint UNSUBSCRIBE_ID = 12;

		public const uint CHANGE_GAME_ID = 13;

		public const uint GET_FACTORY_INFO_ID = 14;

		public const uint GET_GAME_STATS_ID = 15;

		public GameMasterService() : base("bnet.protocol.game_master.GameMaster")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.game_master.GameMaster.JoinGame", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<JoinGameResponse>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.ListFactories", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ListFactoriesResponse>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.FindGame", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<FindGameResponse>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.CancelGameEntry", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.GameEnded", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.PlayerLeft", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.RegisterServer", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.UnregisterServer", 8, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.RegisterUtilities", 9, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.UnregisterUtilities", 10, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.Subscribe", 11, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SubscribeResponse>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.Unsubscribe", 12, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.ChangeGame", 13, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.GetFactoryInfo", 14, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetFactoryInfoResponse>)), new MethodDescriptor("bnet.protocol.game_master.GameMaster.GetGameStats", 15, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetGameStatsResponse>)) };
			}
		}
	}
}