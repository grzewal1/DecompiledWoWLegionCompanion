using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class AccountService : ServiceDescriptor
	{
		public const uint GET_GAME_ACCOUNT_ID = 12;

		public const uint GET_ACCOUNT_ID = 13;

		public const uint CREATE_GAME_ACCOUNT_ID = 14;

		public const uint IS_IGR_ADDRESS_ID = 15;

		public const uint CACHE_EXPIRE_ID = 20;

		public const uint CREDENTIAL_UPDATE_ID = 21;

		public const uint FLAG_UPDATE_ID = 22;

		public const uint GET_WALLET_LIST_ID = 23;

		public const uint GET_EBALANCE_ID = 24;

		public const uint SUBSCRIBE_ID = 25;

		public const uint UNSUBSCRIBE_ID = 26;

		public const uint GET_EBALANCE_RESTRICTIONS_ID = 27;

		public const uint GET_ACCOUNT_STATE_ID = 30;

		public const uint GET_GAME_ACCOUNT_STATE_ID = 31;

		public const uint GET_LICENSES_ID = 32;

		public const uint GET_GAME_TIME_REMAINING_INFO_ID = 33;

		public const uint GET_GAME_SESSION_INFO_ID = 34;

		public const uint GET_CAIS_INFO_ID = 35;

		public const uint FORWARD_CACHE_EXPIRE_ID = 36;

		public AccountService() : base("bnet.protocol.account.AccountService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[37];
				this.Methods[12] = new MethodDescriptor("bnet.protocol.account.AccountService.GetGameAccount", 12, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameAccountBlob>));
				this.Methods[13] = new MethodDescriptor("bnet.protocol.account.AccountService.GetAccount", 13, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetAccountResponse>));
				this.Methods[14] = new MethodDescriptor("bnet.protocol.account.AccountService.CreateGameAccount", 14, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameAccountHandle>));
				this.Methods[15] = new MethodDescriptor("bnet.protocol.account.AccountService.IsIgrAddress", 15, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>));
				this.Methods[20] = new MethodDescriptor("bnet.protocol.account.AccountService.CacheExpire", 20, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>));
				this.Methods[21] = new MethodDescriptor("bnet.protocol.account.AccountService.CredentialUpdate", 21, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>));
				this.Methods[22] = new MethodDescriptor("bnet.protocol.account.AccountService.FlagUpdate", 22, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<CredentialUpdateResponse>));
				this.Methods[23] = new MethodDescriptor("bnet.protocol.account.AccountService.GetWalletList", 23, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetWalletListResponse>));
				this.Methods[24] = new MethodDescriptor("bnet.protocol.account.AccountService.GetEBalance", 24, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetEBalanceResponse>));
				this.Methods[25] = new MethodDescriptor("bnet.protocol.account.AccountService.Subscribe", 25, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<SubscriptionUpdateResponse>));
				this.Methods[26] = new MethodDescriptor("bnet.protocol.account.AccountService.Unsubscribe", 26, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>));
				this.Methods[27] = new MethodDescriptor("bnet.protocol.account.AccountService.GetEBalanceRestrictions", 27, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetEBalanceRestrictionsResponse>));
				this.Methods[30] = new MethodDescriptor("bnet.protocol.account.AccountService.GetAccountState", 30, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetAccountStateResponse>));
				this.Methods[31] = new MethodDescriptor("bnet.protocol.account.AccountService.GetGameAccountState", 31, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetGameAccountStateResponse>));
				this.Methods[32] = new MethodDescriptor("bnet.protocol.account.AccountService.GetLicenses", 32, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetLicensesResponse>));
				this.Methods[33] = new MethodDescriptor("bnet.protocol.account.AccountService.GetGameTimeRemainingInfo", 33, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetGameTimeRemainingInfoResponse>));
				this.Methods[34] = new MethodDescriptor("bnet.protocol.account.AccountService.GetGameSessionInfo", 34, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetGameSessionInfoResponse>));
				this.Methods[35] = new MethodDescriptor("bnet.protocol.account.AccountService.GetCAISInfo", 35, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GetCAISInfoResponse>));
				this.Methods[36] = new MethodDescriptor("bnet.protocol.account.AccountService.ForwardCacheExpire", 36, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>));
			}
		}
	}
}