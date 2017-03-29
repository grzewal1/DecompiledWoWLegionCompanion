using bgs;
using System;

namespace bgs.RPCServices
{
	public class AuthServerService : ServiceDescriptor
	{
		public const uint LOGON_METHOD_ID = 1;

		public const uint MODULE_NOTIFY_METHOD_ID = 2;

		public const uint MODULE_MESSAGE_METHOD_ID = 3;

		public const uint SELECT_GAME_ACCT_DEPRECATED_METHOD_ID = 4;

		public const uint GEN_TEMP_COOKIE_METHOD_ID = 5;

		public const uint SELECT_GAME_ACCT_METHOD_ID = 6;

		public const uint VERIFY_WEB_CREDENTIALS_METHOD_ID = 7;

		public AuthServerService() : base("bnet.protocol.authentication.AuthenticationServer")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.Logon", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.ModuleNotify", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.ModuleMessage", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.SelectGameAccount_DEPRECATED", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.GenerateTempCookie", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<GenerateSSOTokenResponse>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.SelectGameAccount", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.authentication.AuthenticationServer.VerifyWebCredentials", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)) };
			}
		}
	}
}