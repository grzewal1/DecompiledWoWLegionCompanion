using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace bgs
{
	public class SslClientConnection : IClientConnection<BattleNetPacket>
	{
		private const float BLOCKING_SEND_TIME_OUT = 1f;

		private static int RECEIVE_BUFFER_SIZE;

		private static int BACKING_BUFFER_SIZE;

		private SslClientConnection.ConnectionState m_connectionState;

		private SslSocket m_sslSocket;

		private byte[] m_receiveBuffer;

		private byte[] m_backingBuffer;

		private int m_backingBufferBytes;

		private Queue<BattleNetPacket> m_outQueue = new Queue<BattleNetPacket>();

		private string m_hostAddress;

		private int m_hostPort;

		private BattleNetPacket m_currentPacket;

		private SslCertBundleSettings m_bundleSettings;

		private List<IClientConnectionListener<BattleNetPacket>> m_listeners = new List<IClientConnectionListener<BattleNetPacket>>();

		private List<object> m_listenerStates = new List<object>();

		private List<SslClientConnection.ConnectionEvent> m_connectionEvents = new List<SslClientConnection.ConnectionEvent>();

		private List<ConnectHandler> m_connectHandlers = new List<ConnectHandler>();

		private List<DisconnectHandler> m_disconnectHandlers = new List<DisconnectHandler>();

		public bool Active
		{
			get
			{
				return this.m_sslSocket.Connected;
			}
		}

		public bool BlockOnSend
		{
			get;
			set;
		}

		public bool OnlyOneSend
		{
			get;
			set;
		}

		static SslClientConnection()
		{
			SslClientConnection.RECEIVE_BUFFER_SIZE = 262144;
			SslClientConnection.BACKING_BUFFER_SIZE = 262144;
		}

		public SslClientConnection(SslCertBundleSettings bundleSettings)
		{
			this.m_connectionState = SslClientConnection.ConnectionState.Disconnected;
			this.m_receiveBuffer = new byte[SslClientConnection.RECEIVE_BUFFER_SIZE];
			this.m_backingBuffer = new byte[SslClientConnection.BACKING_BUFFER_SIZE];
			this.m_bundleSettings = bundleSettings;
		}

		public bool AddConnectHandler(ConnectHandler handler)
		{
			if (this.m_connectHandlers.Contains(handler))
			{
				return false;
			}
			this.m_connectHandlers.Add(handler);
			return true;
		}

		public bool AddDisconnectHandler(DisconnectHandler handler)
		{
			if (this.m_disconnectHandlers.Contains(handler))
			{
				return false;
			}
			this.m_disconnectHandlers.Add(handler);
			return true;
		}

		public void AddListener(IClientConnectionListener<BattleNetPacket> listener, object state)
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
					this.m_currentPacket = new BattleNetPacket();
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
				SslClientConnection.ConnectionEvent connectionEvent = new SslClientConnection.ConnectionEvent()
				{
					Type = SslClientConnection.ConnectionEventTypes.OnPacketCompleted
				};
				lock (this.m_currentPacket)
				{
					this.m_connectionEvents.Add(connectionEvent);
				}
				this.m_currentPacket = null;
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
				int mBackingBufferBytes = this.m_backingBufferBytes + nBytes;
				if (mBackingBufferBytes > (int)this.m_backingBuffer.Length)
				{
					int bACKINGBUFFERSIZE = (mBackingBufferBytes + SslClientConnection.BACKING_BUFFER_SIZE - 1) / SslClientConnection.BACKING_BUFFER_SIZE;
					byte[] numArray = new byte[bACKINGBUFFERSIZE * SslClientConnection.BACKING_BUFFER_SIZE];
					Array.Copy(this.m_backingBuffer, 0, numArray, 0, (int)this.m_backingBuffer.Length);
					this.m_backingBuffer = numArray;
				}
				Array.Copy(this.m_receiveBuffer, 0, this.m_backingBuffer, this.m_backingBufferBytes, nBytes);
				this.m_backingBufferBytes = 0;
				this.BytesReceived(this.m_backingBuffer, mBackingBufferBytes, 0);
			}
		}

		public void Connect(string host, int port)
		{
			this.m_hostAddress = host;
			this.m_hostPort = port;
			this.Disconnect();
			this.m_sslSocket = new SslSocket();
			this.m_connectionState = SslClientConnection.ConnectionState.Connecting;
			try
			{
				this.m_sslSocket.BeginConnect(host, port, this.m_bundleSettings, new SslSocket.BeginConnectDelegate(this.ConnectCallback));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Concat(this.m_hostAddress, ":", this.m_hostPort);
				LogAdapter.Log(LogLevel.Warning, string.Concat("Could not connect to ", str, " -- ", exception.Message));
				this.m_connectionState = SslClientConnection.ConnectionState.ConnectionFailed;
				this.TriggerOnConnectHandler(BattleNetErrors.ERROR_RPC_PEER_UNKNOWN);
			}
			this.m_bundleSettings = null;
		}

		private void ConnectCallback(bool connectFailed, bool isEncrypted, bool isSigned)
		{
			if (!connectFailed)
			{
				try
				{
					this.m_sslSocket.BeginReceive(this.m_receiveBuffer, SslClientConnection.RECEIVE_BUFFER_SIZE, new SslSocket.BeginReceiveDelegate(this.ReceiveCallback));
				}
				catch (Exception exception)
				{
					connectFailed = true;
				}
			}
			if (connectFailed || !this.m_sslSocket.Connected)
			{
				this.TriggerOnConnectHandler(BattleNetErrors.ERROR_RPC_PEER_UNAVAILABLE);
			}
			else
			{
				this.TriggerOnConnectHandler(BattleNetErrors.ERROR_OK);
			}
		}

		public void Disconnect()
		{
			if (this.m_sslSocket != null)
			{
				this.m_sslSocket.Close();
				this.m_sslSocket = null;
			}
			this.m_connectionState = SslClientConnection.ConnectionState.Disconnected;
		}

		~SslClientConnection()
		{
			if (this.m_sslSocket != null)
			{
				this.Disconnect();
			}
		}

		public void QueuePacket(BattleNetPacket packet)
		{
			this.m_outQueue.Enqueue(packet);
		}

		private void ReceiveCallback(int bytesReceived)
		{
			if (bytesReceived == 0 || !this.m_sslSocket.Connected)
			{
				this.TriggerOnDisconnectHandler(BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED);
				return;
			}
			if (this.m_sslSocket != null && this.m_sslSocket.Connected)
			{
				try
				{
					if (bytesReceived > 0)
					{
						this.BytesReceived(bytesReceived);
						this.m_sslSocket.BeginReceive(this.m_receiveBuffer, SslClientConnection.RECEIVE_BUFFER_SIZE, new SslSocket.BeginReceiveDelegate(this.ReceiveCallback));
					}
				}
				catch (Exception exception)
				{
					this.TriggerOnDisconnectHandler(BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED);
				}
			}
		}

		public bool RemoveConnectHandler(ConnectHandler handler)
		{
			return this.m_connectHandlers.Remove(handler);
		}

		public bool RemoveDisconnectHandler(DisconnectHandler handler)
		{
			return this.m_disconnectHandlers.Remove(handler);
		}

		public void RemoveListener(IClientConnectionListener<BattleNetPacket> listener)
		{
			while (this.m_listeners.Remove(listener))
			{
			}
		}

		private void SendBytes(byte[] bytes)
		{
			// 
			// Current member / type: System.Void bgs.SslClientConnection::SendBytes(System.Byte[])
			// File path: C:\jar_reverse\1.2.52\assets\bin\Data\Managed\Assembly-CSharp.dll
			// 
			// Product version: 2017.2.706.0
			// Exception in: System.Void SendBytes(System.Byte[])
			// 
			// La référence d'objet n'est pas définie à une instance d'un objet.
			//    à ..() dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 93
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 24
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			//    à ..(DecompilationContext ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 19
			//    à ..(MethodBody ,  , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			//    à ..(MethodBody , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			//    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			//    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 72
			//    à ...( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:ligne 317
			//    à ..(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 125
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 274
			//    à ..Visit[,]( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 284
			//    à ..Visit( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 320
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 497
			//    à ..(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 87
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 274
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 381
			//    à ..(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 59
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 274
			//    à ...Match( , Int32 ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:ligne 112
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:ligne 28
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			//    à ..(IfStatement ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 405
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
			//    à ..Visit(IEnumerable ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 380
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 385
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:ligne 33
			//    à ..(DecompilationContext ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:ligne 21
			//    à ..(MethodBody ,  , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			//    à ..(MethodBody , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			//    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			//    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			//    à ..(ILanguage , MethodDefinition ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void SendPacket(BattleNetPacket packet)
		{
			this.SendBytes(packet.Encode());
		}

		private void TriggerOnConnectHandler(BattleNetErrors error)
		{
			SslClientConnection.ConnectionEvent connectionEvent = new SslClientConnection.ConnectionEvent()
			{
				Type = SslClientConnection.ConnectionEventTypes.OnConnected
			};
			lock (error)
			{
				this.m_connectionEvents.Add(connectionEvent);
			}
		}

		private void TriggerOnDisconnectHandler(BattleNetErrors error)
		{
			SslClientConnection.ConnectionEvent connectionEvent = new SslClientConnection.ConnectionEvent()
			{
				Type = SslClientConnection.ConnectionEventTypes.OnDisconnected
			};
			lock (error)
			{
				this.m_connectionEvents.Add(connectionEvent);
			}
		}

		public void Update()
		{
			SslSocket.Process();
			List<SslClientConnection.ConnectionEvent> mConnectionEvents = this.m_connectionEvents;
			Monitor.Enter(mConnectionEvents);
			try
			{
				foreach (SslClientConnection.ConnectionEvent mConnectionEvent in this.m_connectionEvents)
				{
					switch (mConnectionEvent.Type)
					{
						case SslClientConnection.ConnectionEventTypes.OnConnected:
						{
							if (mConnectionEvent.Error == BattleNetErrors.ERROR_OK)
							{
								this.m_connectionState = SslClientConnection.ConnectionState.Connected;
							}
							else
							{
								this.Disconnect();
								this.m_connectionState = SslClientConnection.ConnectionState.ConnectionFailed;
							}
							ConnectHandler[] array = this.m_connectHandlers.ToArray();
							for (int i = 0; i < (int)array.Length; i++)
							{
								array[i](mConnectionEvent.Error);
							}
							continue;
						}
						case SslClientConnection.ConnectionEventTypes.OnDisconnected:
						{
							if (mConnectionEvent.Error != BattleNetErrors.ERROR_OK)
							{
								this.Disconnect();
							}
							DisconnectHandler[] disconnectHandlerArray = this.m_disconnectHandlers.ToArray();
							for (int j = 0; j < (int)disconnectHandlerArray.Length; j++)
							{
								disconnectHandlerArray[j](mConnectionEvent.Error);
							}
							continue;
						}
						case SslClientConnection.ConnectionEventTypes.OnPacketCompleted:
						{
							for (int k = 0; k < this.m_listeners.Count; k++)
							{
								IClientConnectionListener<BattleNetPacket> item = this.m_listeners[k];
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
				Monitor.Exit(mConnectionEvents);
			}
			if (this.m_sslSocket == null || this.m_connectionState != SslClientConnection.ConnectionState.Connected)
			{
				return;
			}
			while (this.m_outQueue.Count > 0)
			{
				if (this.OnlyOneSend && !this.m_sslSocket.m_canSend)
				{
					return;
				}
				this.SendPacket(this.m_outQueue.Dequeue());
			}
		}

		private class ConnectionEvent
		{
			public BattleNetErrors Error
			{
				get;
				set;
			}

			public BattleNetPacket Packet
			{
				get;
				set;
			}

			public SslClientConnection.ConnectionEventTypes Type
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

		private enum ConnectionState
		{
			Disconnected,
			Connecting,
			ConnectionFailed,
			Connected
		}
	}
}