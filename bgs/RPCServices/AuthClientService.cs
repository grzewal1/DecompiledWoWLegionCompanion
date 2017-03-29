using bgs;
using System;

namespace bgs.RPCServices
{
	public class AuthClientService : ServiceDescriptor
	{
		public const uint MODULE_LOAD_METHOD_ID = 1;

		public const uint MODULE_MESSAGE_METHOD_ID = 2;

		public const uint ACCOUNT_SETTINGS_METHOD_ID = 3;

		public const uint SERVER_STATE_CHANGE_METHOD_ID = 4;

		public const uint LOGON_COMPLETE_METHOD_ID = 5;

		public const uint MEM_MODULE_LOAD_METHOD_ID = 6;

		public const uint LOGON_UPDATE_METHOD_ID = 10;

		public const uint VERSION_INFO_UPDATED_ID = 11;

		public const uint LOGON_QUEUE_UPDATE_ID = 12;

		public const uint LOGON_QUEUE_END_ID = 13;

		public const uint GAME_ACCOUNT_SELECTED = 14;

		public AuthClientService() : base("bnet.protocol.authentication.AuthenticationClient")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.ModuleLoad", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ModuleLoadRequest>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.ModuleMessage", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ModuleMessageRequest>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.AccountSettings", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<AccountSettingsNotification>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.ServerStateChange", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ServerStateChangeRequest>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.LogonComplete", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<LogonResult>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.MemModuleLoad", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<MemModuleLoadRequest>)), null, null, null, new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.LogonUpdate", 10, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<LogonUpdateRequest>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.VersionInfoUpdated", 11, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<VersionInfoNotification>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.LogonQueueUpdate", 12, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<LogonQueueUpdateRequest>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.LogonQueueEnd", 13, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationClient.GameAccountSelected", 14, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GameAccountSelectedRequest>)) };
			}
		}
	}
}