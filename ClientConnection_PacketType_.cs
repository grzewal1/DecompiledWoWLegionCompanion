using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using UnityEngine;
using WowJamMessages.MobileJSON;

public class ClientConnection<PacketType>
where PacketType : PacketFormat, new()
{
	private List<ClientConnection<PacketType>.ConnectHandler> m_connectHandlers;

	private List<ClientConnection<PacketType>.DisconnectHandler> m_disconnectHandlers;

	private static int RECEIVE_BUFFER_SIZE;

	private static int BACKING_BUFFER_SIZE;

	private bool m_stolenSocket;

	private ClientConnection<PacketType>.ConnectionState m_connectionState;

	private Socket m_socket;

	private byte[] m_receiveBuffer;

	private byte[] m_backingBuffer;

	private int m_backingBufferBytes;

	private Queue<PacketType> m_outQueue;

	private int m_outPacketsInFlight;

	private string m_hostAddress;

	private int m_hostPort;

	private PacketType m_currentPacket;

	private List<IConnectionListener<PacketType>> m_listeners;

	private List<object> m_listenerStates;

	private List<ClientConnection<PacketType>.ConnectionEvent> m_connectionEvents;

	private object m_mutex;

	public bool Active
	{
		get
		{
			if (this.m_socket == null)
			{
				return false;
			}
			if (this.m_socket.Connected)
			{
				return true;
			}
			return this.m_connectionState == ClientConnection<PacketType>.ConnectionState.Connecting;
		}
	}

	public string HostAddress
	{
		get
		{
			return string.Concat(this.m_hostAddress, ":", this.m_hostPort);
		}
	}

	public ClientConnection<PacketType>.ConnectionState State
	{
		get
		{
			return (ClientConnection<PacketType>.ConnectionState)this.m_connectionState;
		}
	}

	static ClientConnection()
	{
		ClientConnection<PacketType>.RECEIVE_BUFFER_SIZE = 65536;
		ClientConnection<PacketType>.BACKING_BUFFER_SIZE = 262144;
	}

	public ClientConnection()
	{
		this.m_connectionState = ClientConnection<PacketType>.ConnectionState.Disconnected;
		this.m_receiveBuffer = new byte[ClientConnection<PacketType>.RECEIVE_BUFFER_SIZE];
		this.m_backingBuffer = new byte[ClientConnection<PacketType>.BACKING_BUFFER_SIZE];
		this.m_stolenSocket = false;
	}

	public ClientConnection(Socket socket)
	{
		this.m_socket = socket;
		this.m_connectionState = ClientConnection<PacketType>.ConnectionState.Connected;
		this.m_receiveBuffer = new byte[ClientConnection<PacketType>.RECEIVE_BUFFER_SIZE];
		this.m_stolenSocket = true;
		this.m_hostAddress = "local";
		this.m_hostPort = 0;
	}

	private void AddConnectEvent(BattleNetErrors error, Exception exception = null)
	{
		ClientConnection<PacketType>.ConnectionEvent connectionEvent = new ClientConnection<PacketType>.ConnectionEvent()
		{
			Type = ClientConnection<PacketType>.ConnectionEventTypes.OnConnected,
			Error = error
		};
		lock (exception)
		{
			this.m_connectionEvents.Add(connectionEvent);
		}
	}

	public bool AddConnectHandler(ClientConnection<PacketType>.ConnectHandler handler)
	{
		if (this.m_connectHandlers.Contains(handler))
		{
			return false;
		}
		this.m_connectHandlers.Add(handler);
		return true;
	}

	private void AddDisconnectEvent(BattleNetErrors error)
	{
		ClientConnection<PacketType>.ConnectionEvent connectionEvent = new ClientConnection<PacketType>.ConnectionEvent()
		{
			Type = ClientConnection<PacketType>.ConnectionEventTypes.OnDisconnected
		};
		lock (error)
		{
			this.m_connectionEvents.Add(connectionEvent);
		}
	}

	public bool AddDisconnectHandler(ClientConnection<PacketType>.DisconnectHandler handler)
	{
		if (this.m_disconnectHandlers.Contains(handler))
		{
			return false;
		}
		this.m_disconnectHandlers.Add(handler);
		return true;
	}

	public void AddListener(IConnectionListener<PacketType> listener, object state)
	{
		this.m_listeners.Add(listener);
		this.m_listenerStates.Add(state);
	}

	private void BytesReceived(byte[] bytes, int nBytes, int offset)
	{
		while (nBytes > 0)
		{
			if (this.m_currentPacket == null)
			{
				this.m_currentPacket = Activator.CreateInstance<PacketType>();
			}
			int num = this.m_currentPacket.Decode(bytes, offset, nBytes);
			nBytes -= num;
			offset += num;
			if (!this.m_currentPacket.IsLoaded())
			{
				Array.Copy(bytes, offset, this.m_backingBuffer, 0, nBytes);
				this.m_backingBufferBytes = nBytes;
				return;
			}
			ClientConnection<PacketType>.ConnectionEvent connectionEvent = new ClientConnection<PacketType>.ConnectionEvent()
			{
				Type = ClientConnection<PacketType>.ConnectionEventTypes.OnPacketCompleted
			};
			lock (this.m_currentPacket)
			{
				this.m_connectionEvents.Add(connectionEvent);
			}
			this.m_currentPacket = (PacketType)null;
		}
		this.m_backingBufferBytes = 0;
	}

	private void BytesReceived(int nBytes)
	{
		if (this.m_backingBufferBytes <= 0)
		{
			this.BytesReceived(this.m_receiveBuffer, nBytes, 0);
		}
		else
		{
			if (nBytes + this.m_backingBufferBytes > ClientConnection<PacketType>.BACKING_BUFFER_SIZE)
			{
				Debug.LogError("backing buffer out of room");
				return;
			}
			int mBackingBufferBytes = this.m_backingBufferBytes + nBytes;
			Array.Copy(this.m_receiveBuffer, 0, this.m_backingBuffer, this.m_backingBufferBytes, nBytes);
			this.m_backingBufferBytes = 0;
			this.BytesReceived(this.m_backingBuffer, mBackingBufferBytes, 0);
		}
	}

	public void Connect(string host, int port)
	{
		this.m_hostAddress = host;
		this.m_hostPort = port;
		IPEndPoint pEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
		this.DisconnectSocket();
		this.m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		this.m_connectionState = ClientConnection<PacketType>.ConnectionState.Connecting;
		try
		{
			this.m_socket.BeginConnect(pEndPoint, new AsyncCallback(this.ConnectCallback), null);
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			Debug.LogWarning(string.Concat("Could not connect to ", this.HostAddress, " -- ", exception.Message));
			this.m_connectionState = ClientConnection<PacketType>.ConnectionState.ConnectionFailed;
			this.AddConnectEvent(BattleNetErrors.ERROR_RPC_PEER_UNKNOWN, null);
		}
	}

	private void ConnectCallback(IAsyncResult ar)
	{
		bool flag = false;
		Exception exception = null;
		try
		{
			this.m_socket.EndConnect(ar);
			this.m_socket.BeginReceive(this.m_receiveBuffer, 0, ClientConnection<PacketType>.RECEIVE_BUFFER_SIZE, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), null);
		}
		catch (Exception exception1)
		{
			exception = exception1;
			flag = true;
		}
		if (flag || !this.m_socket.Connected)
		{
			this.AddConnectEvent(BattleNetErrors.ERROR_RPC_PEER_UNAVAILABLE, exception);
		}
		else
		{
			this.AddConnectEvent(BattleNetErrors.ERROR_OK, null);
		}
	}

	public void Disconnect()
	{
		this.DisconnectSocket();
		this.m_connectionState = ClientConnection<PacketType>.ConnectionState.Disconnected;
	}

	private void DisconnectSocket()
	{
		if (this.m_socket == null)
		{
			return;
		}
		if (!this.m_socket.Connected)
		{
			return;
		}
		this.m_socket.Close();
	}

	~ClientConnection()
	{
		if (this.m_socket != null)
		{
			this.m_socket.Close();
		}
	}

	public bool HasEvents()
	{
		return this.m_connectionEvents.Count > 0;
	}

	public bool HasOutPacketsInFlight()
	{
		return this.m_outPacketsInFlight > 0;
	}

	public bool HasQueuedPackets()
	{
		return this.m_outQueue.Count > 0;
	}

	private void OnSendBytes(IAsyncResult ar)
	{
		try
		{
			this.m_socket.EndSend(ar);
		}
		catch (Exception exception)
		{
			this.AddDisconnectEvent(BattleNetErrors.ERROR_RPC_CONNECTION_TIMED_OUT);
		}
	}

	private void OnSendPacket(IAsyncResult ar)
	{
		this.OnSendBytes(ar);
		object mMutex = this.m_mutex;
		Monitor.Enter(mMutex);
		try
		{
			this.m_outPacketsInFlight--;
		}
		finally
		{
			Monitor.Exit(mMutex);
		}
	}

	private void PrintConnectionException(ClientConnection<PacketType>.ConnectionEvent connectionEvent)
	{
		Exception exception = connectionEvent.Exception;
		if (exception == null)
		{
			return;
		}
		Debug.LogError(string.Format("ClientConnection Exception - {0} - {1}:{2}\n{3}", new object[] { exception.Message, this.m_hostAddress, this.m_hostPort, exception.StackTrace }));
	}

	public void QueuePacket(PacketType packet)
	{
		this.m_outQueue.Enqueue(packet);
	}

	private void ReceiveCallback(IAsyncResult ar)
	{
		if (!this.m_socket.Connected)
		{
			this.AddDisconnectEvent(BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED);
		}
		else
		{
			try
			{
				int num = this.m_socket.EndReceive(ar);
				if (num <= 0)
				{
					this.AddDisconnectEvent(BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED);
				}
				else
				{
					this.BytesReceived(num);
					this.m_socket.BeginReceive(this.m_receiveBuffer, 0, ClientConnection<PacketType>.RECEIVE_BUFFER_SIZE, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), null);
				}
			}
			catch (Exception exception)
			{
				Debug.Log(exception.ToString());
				this.AddDisconnectEvent(BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED);
			}
		}
	}

	public bool RemoveConnectHandler(ClientConnection<PacketType>.ConnectHandler handler)
	{
		return this.m_connectHandlers.Remove(handler);
	}

	public bool RemoveDisconnectHandler(ClientConnection<PacketType>.DisconnectHandler handler)
	{
		return this.m_disconnectHandlers.Remove(handler);
	}

	public void RemoveListener(IConnectionListener<PacketType> listener)
	{
		while (this.m_listeners.Remove(listener))
		{
		}
	}

	public bool SendBytes(byte[] bytes, AsyncCallback callback, object userData)
	{
		if ((int)bytes.Length == 0)
		{
			return false;
		}
		if (!this.m_socket.Connected)
		{
			return false;
		}
		bool flag = false;
		try
		{
			this.m_socket.BeginSend(bytes, 0, (int)bytes.Length, SocketFlags.None, callback, userData);
			flag = true;
		}
		catch (Exception exception)
		{
		}
		return flag;
	}

	public void SendObject(MobileServerText obj)
	{
		string end;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			(new DataContractSerializer(typeof(MobileServerText))).WriteObject(memoryStream, obj);
			memoryStream.Position = (long)0;
			using (StreamReader streamReader = new StreamReader(memoryStream))
			{
				end = streamReader.ReadToEnd();
			}
		}
		this.SendString(end);
	}

	public bool SendPacket(PacketType packet)
	{
		// 
		// Current member / type: System.Boolean ClientConnection`1::SendPacket(PacketType)
		// File path: C:\jar_reverse\1.2.52\assets\bin\Data\Managed\Assembly-CSharp.dll
		// 
		// Product version: 2017.2.706.0
		// Exception in: System.Boolean SendPacket(PacketType)
		// 
		// La référence d'objet n'est pas définie à une instance d'un objet.
		//    à ..() dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 93
		//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 24
		//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
		//    à ..(DecompilationContext ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 19
		//    à ..(MethodBody ,  , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
		//    à ..(MethodBody , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
		//    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
		//    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
		//    à ..(ILanguage , MethodDefinition ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
		// 
		// mailto: JustDecompilePublicFeedback@telerik.com

	}

	public void SendQueuedPackets()
	{
		while (this.m_outQueue.Count > 0)
		{
			this.SendPacket(this.m_outQueue.Dequeue());
		}
	}

	public bool SendString(string str)
	{
		Debug.Log(string.Concat("SendString: ", str));
		byte[] bytes = (new ASCIIEncoding()).GetBytes(str);
		return this.SendBytes(bytes, new AsyncCallback(this.OnSendBytes), null);
	}

	public void StartReceiving()
	{
		if (!this.m_stolenSocket)
		{
			Debug.LogError("StartReceiving should only be called on sockets created with ClientConnection(Socket)");
			return;
		}
		try
		{
			this.m_socket.BeginReceive(this.m_receiveBuffer, 0, ClientConnection<PacketType>.RECEIVE_BUFFER_SIZE, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), null);
		}
		catch (Exception exception)
		{
			Debug.LogError(string.Concat("error receiving from local connection: ", exception.Message));
		}
	}

	public void Update()
	{
		object mMutex = this.m_mutex;
		Monitor.Enter(mMutex);
		try
		{
			foreach (ClientConnection<PacketType>.ConnectionEvent mConnectionEvent in this.m_connectionEvents)
			{
				this.PrintConnectionException(mConnectionEvent);
				switch (mConnectionEvent.Type)
				{
					case (ClientConnection<PacketType>.ConnectionEventTypes)ClientConnection<PacketType>.ConnectionEventTypes.OnConnected:
					{
						if (mConnectionEvent.Error == BattleNetErrors.ERROR_OK)
						{
							this.m_connectionState = ClientConnection<PacketType>.ConnectionState.Connected;
						}
						else
						{
							this.DisconnectSocket();
							this.m_connectionState = ClientConnection<PacketType>.ConnectionState.ConnectionFailed;
						}
						ClientConnection<PacketType>.ConnectHandler[] array = this.m_connectHandlers.ToArray();
						for (int i = 0; i < (int)array.Length; i++)
						{
							array[i](mConnectionEvent.Error);
						}
						continue;
					}
					case (ClientConnection<PacketType>.ConnectionEventTypes)ClientConnection<PacketType>.ConnectionEventTypes.OnDisconnected:
					{
						if (mConnectionEvent.Error != BattleNetErrors.ERROR_OK)
						{
							this.Disconnect();
						}
						ClientConnection<PacketType>.DisconnectHandler[] disconnectHandlerArray = this.m_disconnectHandlers.ToArray();
						for (int j = 0; j < (int)disconnectHandlerArray.Length; j++)
						{
							disconnectHandlerArray[j](mConnectionEvent.Error);
						}
						continue;
					}
					case (ClientConnection<PacketType>.ConnectionEventTypes)ClientConnection<PacketType>.ConnectionEventTypes.OnPacketCompleted:
					{
						for (int k = 0; k < this.m_listeners.Count; k++)
						{
							IConnectionListener<PacketType> item = this.m_listeners[k];
							object obj = this.m_listenerStates[k];
							item.PacketReceived(mConnectionEvent.Packet, obj);
						}
						continue;
					}
					default:
					{
						continue;
					}
				}
			}
			this.m_connectionEvents.Clear();
		}
		finally
		{
			Monitor.Exit(mMutex);
		}
		if (this.m_socket == null || this.m_connectionState != ClientConnection<PacketType>.ConnectionState.Connected)
		{
			return;
		}
		this.SendQueuedPackets();
	}

	public delegate void ConnectHandler(BattleNetErrors error);

	private class ConnectionEvent
	{
		public BattleNetErrors Error
		{
			get;
			set;
		}

		public Exception Exception
		{
			get;
			set;
		}

		public PacketType Packet
		{
			get;
			set;
		}

		public ClientConnection<PacketType>.ConnectionEventTypes Type
		{
			get;
			set;
		}

		public ConnectionEvent()
		{
		}
	}

	private enum ConnectionEventTypes
	{
		OnConnected,
		OnDisconnected,
		OnPacketCompleted
	}

	public enum ConnectionState
	{
		Disconnected,
		Connecting,
		ConnectionFailed,
		Connected
	}

	public delegate void DisconnectHandler(BattleNetErrors error);
}