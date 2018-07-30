using bgs.RPCServices;
using bnet.protocol;
using bnet.protocol.connection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace bgs
{
	public class RPCConnection : IClientConnectionListener<BattleNetPacket>
	{
		private const int RESPONSE_SERVICE_ID = 254;

		private BattleNetLogSource m_logSource = new BattleNetLogSource("Network");

		private BattleNetLogSource m_cmLogSource = new BattleNetLogSource("ConnectionMetering");

		private IClientConnection<BattleNetPacket> Connection;

		public ServiceCollectionHelper serviceHelper = new ServiceCollectionHelper();

		private Queue<BattleNetPacket> outBoundPackets = new Queue<BattleNetPacket>();

		private Queue<BattleNetPacket> incomingPackets = new Queue<BattleNetPacket>();

		private List<BattleNetPacket> m_pendingOutboundPackets = new List<BattleNetPacket>();

		private object tokenLock = new object();

		private static uint nextToken;

		private Dictionary<uint, RPCContext> waitingForResponse = new Dictionary<uint, RPCContext>();

		private RPCConnection.OnConnectHandler m_onConnectHandler;

		private RPCConnection.OnDisconectHandler m_onDisconnectHandler;

		private Stopwatch m_stopWatch;

		private RPCConnectionMetering m_connMetering = new RPCConnectionMetering();

		public long MillisecondsSinceLastPacketSent
		{
			get
			{
				return this.m_stopWatch.ElapsedMilliseconds;
			}
		}

		static RPCConnection()
		{
		}

		public RPCConnection()
		{
		}

		public void BeginAuth()
		{
			this.m_connMetering.ResetStartupPeriod();
		}

		public void Connect(string host, int port, SslParameters sslParams)
		{
			this.m_stopWatch = new Stopwatch();
			if (!sslParams.useSsl)
			{
				this.Connection = new ClientConnection<BattleNetPacket>();
			}
			else
			{
				SslClientConnection sslClientConnection = new SslClientConnection(sslParams.bundleSettings)
				{
					OnlyOneSend = true
				};
				this.Connection = sslClientConnection;
			}
			this.Connection.AddListener(this, null);
			this.Connection.AddConnectHandler(new ConnectHandler(this.OnConnectCallback));
			this.Connection.AddDisconnectHandler(new DisconnectHandler(this.OnDisconnectCallback));
			this.Connection.Connect(host, port);
		}

		protected Header CreateHeader(uint serviceId, uint methodId, uint objectId, uint token, uint size)
		{
			Header header = new Header();
			header.SetServiceId(serviceId);
			header.SetMethodId(methodId);
			if (objectId != 0)
			{
				header.SetObjectId((ulong)objectId);
			}
			header.SetToken(token);
			header.SetSize(size);
			return header;
		}

		public void Disconnect()
		{
			if (this.Connection is SslClientConnection)
			{
				((SslClientConnection)this.Connection).BlockOnSend = true;
			}
			this.Update();
			this.Connection.Disconnect();
		}

		private void DownloadCompletedCallback(byte[] data, object userContext)
		{
			if (data == null)
			{
				this.m_cmLogSource.LogWarning("Downloading of the connection metering data failed!");
				return;
			}
			this.m_cmLogSource.LogDebug("Connection metering file downloaded. Length={0}", new object[] { (int)data.Length });
			this.m_connMetering.SetConnectionMeteringData(data, this.serviceHelper);
		}

		private ServiceDescriptor GetExportedServiceDescriptor(uint serviceId)
		{
			return this.serviceHelper.GetExportedServiceById(serviceId);
		}

		public uint GetExportedServiceNameHash(uint serviceId)
		{
			ServiceDescriptor exportedServiceDescriptor = this.GetExportedServiceDescriptor(serviceId);
			if (exportedServiceDescriptor == null)
			{
				return (uint)-1;
			}
			return exportedServiceDescriptor.Hash;
		}

		private ServiceDescriptor GetImportedServiceDescriptor(uint serviceId)
		{
			return this.serviceHelper.GetImportedServiceById(serviceId);
		}

		public uint GetImportedServiceNameHash(uint serviceId)
		{
			ServiceDescriptor importedServiceDescriptor = this.GetImportedServiceDescriptor(serviceId);
			if (importedServiceDescriptor == null)
			{
				return (uint)-1;
			}
			return importedServiceDescriptor.Hash;
		}

		private string GetMethodName(Header header)
		{
			return this.GetMethodName(header, true);
		}

		private string GetMethodName(Header header, bool outgoing)
		{
			if (header.ServiceId == 254)
			{
				return "Response";
			}
			ServiceDescriptor serviceDescriptor = null;
			serviceDescriptor = (!outgoing ? this.serviceHelper.GetExportedServiceById(header.ServiceId) : this.serviceHelper.GetImportedServiceById(header.ServiceId));
			return (serviceDescriptor != null ? serviceDescriptor.GetMethodName(header.MethodId) : "No Descriptor");
		}

		private string GetServiceName(Header header, bool outgoing)
		{
			if (header.ServiceId == 254)
			{
				return "Response";
			}
			ServiceDescriptor serviceDescriptor = null;
			serviceDescriptor = (!outgoing ? this.serviceHelper.GetExportedServiceById(header.ServiceId) : this.serviceHelper.GetImportedServiceById(header.ServiceId));
			return (serviceDescriptor != null ? serviceDescriptor.Name : "No Descriptor");
		}

		private void LogOutgoingPacket(BattleNetPacket packet, bool wasMetered)
		{
			string methodName;
			if (this.m_logSource == null)
			{
				LogAdapter.Log(LogLevel.Warning, "tried to log with null log source, skipping");
				return;
			}
			bool flag = false;
			IProtoBuf body = (IProtoBuf)packet.GetBody();
			Header header = packet.GetHeader();
			uint serviceId = header.ServiceId;
			uint methodId = header.MethodId;
			string str = (!wasMetered ? "QueueRequest" : "QueueRequest (METERED)");
			if (string.IsNullOrEmpty(body.ToString()))
			{
				ServiceDescriptor importedServiceById = this.serviceHelper.GetImportedServiceById(serviceId);
				if (importedServiceById != null)
				{
					methodName = importedServiceById.GetMethodName(methodId);
				}
				else
				{
					methodName = null;
				}
				string str1 = methodName;
				if (!(str1 != "bnet.protocol.connection.ConnectionService.KeepAlive") || str1 == null)
				{
					flag = true;
				}
				else
				{
					this.m_logSource.LogDebug("{0}: type = {1}, header = {2}", new object[] { str, str1, header.ToString() });
				}
			}
			else
			{
				ServiceDescriptor serviceDescriptor = this.serviceHelper.GetImportedServiceById(serviceId);
				string str2 = (serviceDescriptor != null ? serviceDescriptor.GetMethodName(methodId) : "null");
				if (!str2.Contains("KeepAlive"))
				{
					this.m_logSource.LogDebug("{0}: type = {1}, header = {2}, request = {3}", new object[] { str, str2, header.ToString(), body.ToString() });
				}
			}
			if (!flag)
			{
				this.m_logSource.LogDebugStackTrace("LogOutgoingPacket: ", 32, 1);
			}
		}

		private void OnConnectCallback(BattleNetErrors error)
		{
			if (this.m_onConnectHandler != null)
			{
				this.m_onConnectHandler(error);
			}
		}

		private void OnDisconnectCallback(BattleNetErrors error)
		{
			if (this.m_onDisconnectHandler != null)
			{
				this.m_onDisconnectHandler(error);
			}
		}

		public string PacketHeaderToString(Header header, bool outgoing)
		{
			string empty = string.Empty;
			string str = empty;
			empty = string.Concat(new object[] { str, "Service:(", header.ServiceId, ")", this.GetServiceName(header, outgoing) });
			empty = string.Concat(empty, " ");
			empty = string.Concat(empty, "Method:(", (!header.HasMethodId ? "?)" : string.Concat(header.MethodId, ")", this.GetMethodName(header, outgoing))));
			empty = string.Concat(empty, " ");
			empty = string.Concat(empty, "Token:", header.Token);
			empty = string.Concat(empty, " ");
			empty = string.Concat(empty, "Status:", (BattleNetErrors)header.Status);
			if (header.ErrorCount > 0)
			{
				empty = string.Concat(empty, " Error:[");
				foreach (ErrorInfo errorList in header.ErrorList)
				{
					str = empty;
					empty = string.Concat(new object[] { str, " ErrorInfo{ ", errorList.ObjectAddress.Host.Label, "/", errorList.ObjectAddress.Host.Epoch, "}" });
				}
				empty = string.Concat(empty, "]");
			}
			return empty;
		}

		public void PacketReceived(BattleNetPacket p, object state)
		{
			object obj = this.incomingPackets;
			Monitor.Enter(obj);
			try
			{
				this.incomingPackets.Enqueue(p);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public string PacketToString(BattleNetPacket packet, bool outgoing)
		{
			return this.PacketHeaderToString(packet.GetHeader(), outgoing);
		}

		private void PrintHeader(Header h)
		{
			string str = string.Format("Packet received: Header = [ ServiceId: {0}, MethodId: {1} Token: {2} Size: {3} Status: {4}", new object[] { h.ServiceId, h.MethodId, h.Token, h.Size, (BattleNetErrors)h.Status });
			if (h.ErrorCount > 0)
			{
				str = string.Concat(str, " Error:[");
				foreach (ErrorInfo errorList in h.ErrorList)
				{
					string str1 = str;
					str = string.Concat(new object[] { str1, " ErrorInfo{ ", errorList.ObjectAddress.Host.Label, "/", errorList.ObjectAddress.Host.Epoch, "}" });
				}
				str = string.Concat(str, "]");
			}
			str = string.Concat(str, "]");
			this.m_logSource.LogDebug(str);
		}

		private void ProcessPendingOutboundPackets()
		{
			if (this.m_pendingOutboundPackets.Count > 0)
			{
				List<BattleNetPacket> battleNetPackets = new List<BattleNetPacket>();
				foreach (BattleNetPacket mPendingOutboundPacket in this.m_pendingOutboundPackets)
				{
					Header header = mPendingOutboundPacket.GetHeader();
					uint serviceId = header.ServiceId;
					uint methodId = header.MethodId;
					if (!this.m_connMetering.AllowRPCCall(serviceId, methodId))
					{
						battleNetPackets.Add(mPendingOutboundPacket);
					}
					else
					{
						this.QueuePacket(mPendingOutboundPacket);
					}
				}
				this.m_pendingOutboundPackets = battleNetPackets;
			}
		}

		protected void QueuePacket(BattleNetPacket packet)
		{
			this.LogOutgoingPacket(packet, false);
			object obj = this.outBoundPackets;
			Monitor.Enter(obj);
			try
			{
				this.outBoundPackets.Enqueue(packet);
				this.m_stopWatch.Reset();
				this.m_stopWatch.Start();
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public RPCContext QueueRequest(uint serviceId, uint methodId, IProtoBuf message, RPCContextDelegate callback = null, uint objectId = 0)
		{
			// 
			// Current member / type: bgs.RPCContext bgs.RPCConnection::QueueRequest(System.UInt32,System.UInt32,IProtoBuf,bgs.RPCContextDelegate,System.UInt32)
			// File path: C:\Users\Selenium\Downloads\WoW_2.0.4 - Copie\assets\bin\Data\Managed\Assembly-CSharp.dll
			// 
			// Product version: 2018.2.605.0
			// Exception in: bgs.RPCContext QueueRequest(System.UInt32,System.UInt32,IProtoBuf,bgs.RPCContextDelegate,System.UInt32)
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

		public void QueueResponse(RPCContext context, IProtoBuf message)
		{
			if (message == null || context.Header == null)
			{
				this.m_logSource.LogError("QueueResponse: invalid response");
				return;
			}
			if (this.serviceHelper.GetImportedServiceById(context.Header.ServiceId) == null)
			{
				this.m_logSource.LogError(string.Concat("QueueResponse: error, unrecognized service id: ", context.Header.ServiceId));
				return;
			}
			this.m_logSource.LogDebug(string.Concat(new object[] { "QueueResponse: type=", this.serviceHelper.GetImportedServiceById(context.Header.ServiceId).GetMethodName(context.Header.MethodId), " data=", message }));
			Header header = context.Header;
			header.SetServiceId(254);
			header.SetMethodId(0);
			header.SetSize(message.GetSerializedSize());
			context.Header = header;
			this.QueuePacket(new BattleNetPacket(context.Header, message));
		}

		public void RegisterServiceMethodListener(uint serviceId, uint methodId, RPCContextDelegate callback)
		{
			ServiceDescriptor exportedServiceDescriptor = this.GetExportedServiceDescriptor(serviceId);
			if (exportedServiceDescriptor != null)
			{
				exportedServiceDescriptor.RegisterMethodListener(methodId, callback);
			}
		}

		public void SetConnectionMeteringContentHandles(ConnectionMeteringContentHandles handles, LocalStorageAPI localStorage)
		{
			if (handles == null || !handles.IsInitialized || handles.ContentHandleCount == 0)
			{
				this.m_cmLogSource.LogWarning("Invalid connection metering content handle received.");
				return;
			}
			if (handles.ContentHandleCount != 1)
			{
				this.m_cmLogSource.LogWarning("More than 1 connection metering content handle specified!");
			}
			bnet.protocol.ContentHandle item = handles.ContentHandle[0];
			if (item == null || !item.IsInitialized)
			{
				this.m_cmLogSource.LogWarning("The content handle received is not valid!");
				return;
			}
			this.m_cmLogSource.LogDebug("Received request to enable connection metering.");
			bgs.ContentHandle contentHandle = bgs.ContentHandle.FromProtocol(item);
			this.m_cmLogSource.LogDebug("Requesting file from local storage. ContentHandle={0}", new object[] { contentHandle });
			localStorage.GetFile(contentHandle, new LocalStorageAPI.DownloadCompletedCallback(this.DownloadCompletedCallback), null);
		}

		public void SetOnConnectHandler(RPCConnection.OnConnectHandler handler)
		{
			this.m_onConnectHandler = handler;
		}

		public void SetOnDisconnectHandler(RPCConnection.OnDisconectHandler handler)
		{
			this.m_onDisconnectHandler = handler;
		}

		public void Update()
		{
			Queue<BattleNetPacket> battleNetPackets;
			Queue<BattleNetPacket> battleNetPackets1;
			RPCContext rPCContext;
			this.ProcessPendingOutboundPackets();
			if (this.outBoundPackets.Count > 0)
			{
				object obj = this.outBoundPackets;
				Monitor.Enter(obj);
				try
				{
					battleNetPackets = new Queue<BattleNetPacket>(this.outBoundPackets.ToArray());
					this.outBoundPackets.Clear();
				}
				finally
				{
					Monitor.Exit(obj);
				}
				while (battleNetPackets.Count > 0)
				{
					BattleNetPacket battleNetPacket = battleNetPackets.Dequeue();
					if (this.Connection == null)
					{
						this.m_logSource.LogError("##Client Connection object does not exists!##");
					}
					else
					{
						this.Connection.QueuePacket(battleNetPacket);
					}
				}
			}
			if (this.Connection != null)
			{
				this.Connection.Update();
			}
			if (this.incomingPackets.Count > 0)
			{
				object obj1 = this.incomingPackets;
				Monitor.Enter(obj1);
				try
				{
					battleNetPackets1 = new Queue<BattleNetPacket>(this.incomingPackets.ToArray());
					this.incomingPackets.Clear();
				}
				finally
				{
					Monitor.Exit(obj1);
				}
				while (battleNetPackets1.Count > 0)
				{
					BattleNetPacket battleNetPacket1 = battleNetPackets1.Dequeue();
					Header header = battleNetPacket1.GetHeader();
					this.PrintHeader(header);
					byte[] body = (byte[])battleNetPacket1.GetBody();
					if (header.ServiceId != 254)
					{
						ServiceDescriptor exportedServiceDescriptor = this.GetExportedServiceDescriptor(header.ServiceId);
						if (exportedServiceDescriptor == null)
						{
							this.m_logSource.LogError(string.Concat(new object[] { "[!]Server Requested an Unsupported (Service id:", header.ServiceId, " Method id:", header.MethodId, ")" }));
						}
						else
						{
							if (this.serviceHelper.GetExportedServiceById(header.ServiceId).GetParser(header.MethodId) == null)
							{
								this.m_logSource.LogDebug(string.Concat("Incoming Packet: NULL TYPE service=", this.serviceHelper.GetExportedServiceById(header.ServiceId).Name, ", method=", this.serviceHelper.GetExportedServiceById(header.ServiceId).GetMethodName(header.MethodId)));
							}
							if (!exportedServiceDescriptor.HasMethodListener(header.MethodId))
							{
								string str = (exportedServiceDescriptor == null || string.IsNullOrEmpty(exportedServiceDescriptor.Name) ? "<null>" : exportedServiceDescriptor.Name);
								this.m_logSource.LogError(string.Concat(new object[] { "[!]Unhandled Server Request Received (Service Name: ", str, " Service id:", header.ServiceId, " Method id:", header.MethodId, ")" }));
							}
							else
							{
								RPCContext rPCContext1 = new RPCContext()
								{
									Header = header,
									Payload = body,
									ResponseReceived = true
								};
								exportedServiceDescriptor.NotifyMethodListener(rPCContext1);
							}
						}
					}
					else if (this.waitingForResponse.TryGetValue(header.Token, out rPCContext))
					{
						ServiceDescriptor importedServiceById = this.serviceHelper.GetImportedServiceById(rPCContext.Header.ServiceId);
						MethodDescriptor.ParseMethod parser = null;
						if (importedServiceById != null)
						{
							parser = importedServiceById.GetParser(rPCContext.Header.MethodId);
						}
						if (parser == null)
						{
							if (importedServiceById == null)
							{
								this.m_logSource.LogWarning("Incoming Response: Unable to identify service id={0}", new object[] { rPCContext.Header.ServiceId });
							}
							else
							{
								this.m_logSource.LogWarning("Incoming Response: Unable to find method for serviceName={0} method id={1}", new object[] { importedServiceById.Name, rPCContext.Header.MethodId });
								int methodCount = importedServiceById.GetMethodCount();
								this.m_logSource.LogDebug("  Found {0} methods", new object[] { methodCount });
								for (int i = 0; i < methodCount; i++)
								{
									MethodDescriptor methodDescriptor = importedServiceById.GetMethodDescriptor((uint)i);
									if (methodDescriptor != null || i == 0)
									{
										this.m_logSource.LogDebug("  Found method id={0} name={1}", new object[] { i, methodDescriptor.Name });
									}
									else
									{
										this.m_logSource.LogDebug("  Found method id={0} name={1}", new object[] { i, "<null>" });
									}
								}
							}
						}
						rPCContext.Header = header;
						rPCContext.Payload = body;
						rPCContext.ResponseReceived = true;
						if (rPCContext.Callback != null)
						{
							rPCContext.Callback(rPCContext);
						}
						this.waitingForResponse.Remove(header.Token);
					}
				}
			}
		}

		public delegate void OnConnectHandler(BattleNetErrors error);

		public delegate void OnDisconectHandler(BattleNetErrors error);
	}
}