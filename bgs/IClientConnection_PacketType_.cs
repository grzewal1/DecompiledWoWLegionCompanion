using System;

namespace bgs
{
	public interface IClientConnection<PacketType>
	where PacketType : PacketFormat
	{
		bool AddConnectHandler(ConnectHandler handler);

		bool AddDisconnectHandler(DisconnectHandler handler);

		void AddListener(IClientConnectionListener<PacketType> listener, object state);

		void Connect(string host, int port);

		void Disconnect();

		void QueuePacket(PacketType packet);

		bool RemoveConnectHandler(ConnectHandler handler);

		void Update();
	}
}