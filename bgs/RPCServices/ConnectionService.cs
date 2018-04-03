using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class ConnectionService : ServiceDescriptor
	{
		public const uint CONNECT_METHOD_ID = 1;

		public const uint BIND_METHOD_ID = 2;

		public const uint ECHO_METHOD_ID = 3;

		public const uint FORCE_DISCONNECT_METHOD_ID = 4;

		public const uint KEEP_ALIVE_METHOD_ID = 5;

		public const uint ENCRYPT_METHOD_ID = 6;

		public const uint REQUEST_DISCONNECT_METHOD_ID = 7;

		public ConnectionService() : base("bnet.protocol.connection.ConnectionService")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.connection.ConnectionService.Connect", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ConnectResponse>)), new MethodDescriptor("bnet.protocol.connection.ConnectionService.Bind", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<BindResponse>)), new MethodDescriptor("bnet.protocol.connection.ConnectionService.Echo", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<EchoResponse>)), new MethodDescriptor("bnet.protocol.connection.ConnectionService.ForceDisconnect", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.connection.ConnectionService.KeepAlive", 5, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)), new MethodDescriptor("bnet.protocol.connection.ConnectionService.Encrypt", 6, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NoData>)), new MethodDescriptor("bnet.protocol.connection.ConnectionService.RequestDisconnect", 7, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<NORESPONSE>)) };
			}
		}
	}
}