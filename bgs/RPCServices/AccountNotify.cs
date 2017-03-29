using bgs;
using System;

namespace bgs.RPCServices
{
	public class AccountNotify : ServiceDescriptor
	{
		public const uint NOTIFY_ACCOUNT_STATE_UPDATED_ID = 1;

		public const uint NOTIFY_GAME_ACCOUNT_STATE_UPDATED_ID = 2;

		public const uint NOTIFY_GAME_ACCOUNTS_UPDATED_ID = 3;

		public const uint NOTIFY_GAME_SESSION_UPDATED_ID = 4;

		public AccountNotify() : base("bnet.protocol.account.AccountNotify")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.account.AccountNotify.NotifyAccountStateUpdated", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<AccountStateNotification>)), new MethodDescriptor("bnet.protocol.account.AccountNotify.NotifyGameAccountStateUpdated", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameAccountStateNotification>)), new MethodDescriptor("bnet.protocol.account.AccountNotify.NotifyGameAccountsUpdated", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameAccountNotification>)), new MethodDescriptor("bnet.protocol.account.AccountNotify.NotifyGameSessionUpdated", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameAccountSessionNotification>)) };
			}
		}
	}
}