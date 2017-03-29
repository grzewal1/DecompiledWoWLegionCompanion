using bgs.RPCServices;
using bgs.types;
using bnet.protocol;
using bnet.protocol.attribute;
using bnet.protocol.channel;
using bnet.protocol.game_master;
using bnet.protocol.game_utilities;
using bnet.protocol.notification;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace bgs
{
	public class GamesAPI : BattleNetAPI
	{
		private const int NO_AI_DECK = 0;

		private const bool RANK_NA = true;

		private const bool RANKED = false;

		private const bool UNRANKED = true;

		private Queue<GamesAPI.UtilResponse> m_utilPackets = new Queue<GamesAPI.UtilResponse>();

		private Queue<GamesAPI.GetAllValuesForAttributeResult> m_getAllValuesForAttributeResults = new Queue<GamesAPI.GetAllValuesForAttributeResult>();

		private Queue<QueueEvent> m_queueEvents = new Queue<QueueEvent>();

		private ServiceDescriptor m_gameUtilitiesService = new GameUtilitiesService();

		private ServiceDescriptor m_gameMasterService = new bgs.RPCServices.GameMasterService();

		private ServiceDescriptor m_gameMasterSubscriberService = new bgs.RPCServices.GameMasterSubscriberService();

		private ServiceDescriptor m_gameFactorySubscriberService = new GameFactorySubscriberService();

		private static bool warnComplete;

		private ulong s_gameRequest;

		public ulong CurrentGameRequest
		{
			get
			{
				return this.s_gameRequest;
			}
			set
			{
				this.s_gameRequest = value;
			}
		}

		public ServiceDescriptor GameFactorySubscribeService
		{
			get
			{
				return this.m_gameFactorySubscriberService;
			}
		}

		public ServiceDescriptor GameMasterService
		{
			get
			{
				return this.m_gameMasterService;
			}
		}

		public ServiceDescriptor GameMasterSubscriberService
		{
			get
			{
				return this.m_gameMasterSubscriberService;
			}
		}

		public ServiceDescriptor GameUtilityService
		{
			get
			{
				return this.m_gameUtilitiesService;
			}
		}

		public bool IsFindGamePending
		{
			get;
			private set;
		}

		static GamesAPI()
		{
		}

		public GamesAPI(BattleNetCSharp battlenet) : base(battlenet, "Games")
		{
		}

		private void AddQueueEvent(QueueEvent.Type queueType, int minSeconds = 0, int maxSeconds = 0, int bnetError = 0, GameServerInfo gsInfo = null)
		{
			QueueEvent queueEvent = new QueueEvent(queueType, minSeconds, maxSeconds, bnetError, gsInfo);
			Queue<QueueEvent> mQueueEvents = this.m_queueEvents;
			Monitor.Enter(mQueueEvents);
			try
			{
				this.m_queueEvents.Enqueue(queueEvent);
			}
			finally
			{
				Monitor.Exit(mQueueEvents);
			}
		}

		public void CancelFindGame()
		{
			this.CancelFindGame(this.s_gameRequest);
			this.s_gameRequest = (ulong)0;
		}

		private void CancelFindGame(ulong gameRequestId)
		{
			CancelGameEntryRequest cancelGameEntryRequest = new CancelGameEntryRequest()
			{
				RequestId = gameRequestId
			};
			Player player = new Player();
			Identity identity = new Identity();
			identity.SetGameAccountId(this.m_battleNet.GameAccountId);
			player.SetIdentity(identity);
			cancelGameEntryRequest.AddPlayer(player);
			GamesAPI.CancelGameContext cancelGameContext = new GamesAPI.CancelGameContext(gameRequestId);
			this.m_rpcConnection.QueueRequest(this.m_gameMasterService.Id, 4, cancelGameEntryRequest, new RPCContextDelegate(cancelGameContext.CancelGameCallback), 0);
		}

		private void ClientRequestCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				if (context.SystemId != 1)
				{
					this.m_battleNet.EnqueueErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnClientRequest, status, 0);
				}
				return;
			}
			ClientResponse clientResponse = ClientResponse.ParseFrom(context.Payload);
			base.ApiLog.LogDebug("Enqueuing response");
			this.m_utilPackets.Enqueue(new GamesAPI.UtilResponse(clientResponse, context.Context));
		}

		private ClientRequest CreateClientRequest(int type, int sys, byte[] bs, ulong route)
		{
			ClientRequest clientRequest = new ClientRequest();
			clientRequest.AddAttribute(ProtocolHelper.CreateAttribute("p", bs));
			if (BattleNet.IsVersionInt())
			{
				int num = 0;
				if (!int.TryParse(BattleNet.GetVersion(), out num))
				{
					LogAdapter.Log(LogLevel.Error, string.Concat("Could not convert BattleNetVersion to int: ", BattleNet.GetVersion()));
				}
				clientRequest.AddAttribute(ProtocolHelper.CreateAttribute("v", (long)(10 * num + sys)));
			}
			else
			{
				clientRequest.AddAttribute(ProtocolHelper.CreateAttribute("v", string.Concat(BattleNet.GetVersion(), (sys != 0 ? "b" : "c"))));
			}
			if (route != 0)
			{
				clientRequest.AddAttribute(ProtocolHelper.CreateAttribute("r", route));
			}
			return clientRequest;
		}

		public void CreateFriendlyChallengeGame(long myDeck, long hisDeck, bnet.protocol.EntityId hisGameAccount, int scenario)
		{
			FindGameRequest findGameRequest = new FindGameRequest();
			Player player = new Player();
			Identity identity = new Identity();
			identity.SetGameAccountId(this.m_battleNet.GameAccountId);
			GameProperties gameProperty = new GameProperties();
			AttributeFilter attributeFilter = new AttributeFilter();
			attributeFilter.SetOp(AttributeFilter.Types.Operation.MATCH_ALL);
			if (BattleNet.IsVersionInt())
			{
				int num = 0;
				if (!int.TryParse(BattleNet.GetVersion(), out num))
				{
					LogAdapter.Log(LogLevel.Error, string.Concat("Could not convert BattleNetVersion to int: ", BattleNet.GetVersion()));
				}
				attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("version", (long)num));
			}
			else
			{
				attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("version", BattleNet.GetVersion()));
			}
			attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("GameType", (long)1));
			attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("ScenarioId", (long)scenario));
			gameProperty.SetFilter(attributeFilter);
			gameProperty.AddCreationAttributes(ProtocolHelper.CreateAttribute("type", (long)1));
			gameProperty.AddCreationAttributes(ProtocolHelper.CreateAttribute("scenario", (long)scenario));
			player.SetIdentity(identity);
			player.AddAttribute(ProtocolHelper.CreateAttribute("type", (long)1));
			player.AddAttribute(ProtocolHelper.CreateAttribute("scenario", (long)scenario));
			player.AddAttribute(ProtocolHelper.CreateAttribute("deck", (long)((int)myDeck)));
			findGameRequest.AddPlayer(player);
			identity = new Identity();
			player = new Player();
			identity.SetGameAccountId(hisGameAccount);
			player.SetIdentity(identity);
			player.AddAttribute(ProtocolHelper.CreateAttribute("type", (long)1));
			player.AddAttribute(ProtocolHelper.CreateAttribute("scenario", (long)scenario));
			player.AddAttribute(ProtocolHelper.CreateAttribute("deck", (long)((int)hisDeck)));
			findGameRequest.AddPlayer(player);
			findGameRequest.SetProperties(gameProperty);
			findGameRequest.SetAdvancedNotification(true);
			FindGameRequest findGameRequest1 = findGameRequest;
			this.PrintFindGameRequest(findGameRequest1);
			this.IsFindGamePending = true;
			this.m_rpcConnection.QueueRequest(this.m_gameMasterService.Id, 3, findGameRequest1, new RPCContextDelegate(this.FindGameCallback), 0);
		}

		public void FindGame(byte[] requestGuid, int gameType, int scenario, long deckId, long aiDeckId, bool setScenarioIdAttr)
		{
			if (this.s_gameRequest != 0)
			{
				LogAdapter.Log(LogLevel.Warning, "WARNING: FindGame called with an active game");
				this.CancelFindGame(this.s_gameRequest);
				this.s_gameRequest = (ulong)0;
			}
			Player player = new Player();
			Identity identity = new Identity();
			identity.SetGameAccountId(this.m_battleNet.GameAccountId);
			player.SetIdentity(identity);
			player.AddAttribute(ProtocolHelper.CreateAttribute("type", (long)gameType));
			player.AddAttribute(ProtocolHelper.CreateAttribute("scenario", (long)scenario));
			player.AddAttribute(ProtocolHelper.CreateAttribute("deck", (long)((int)deckId)));
			player.AddAttribute(ProtocolHelper.CreateAttribute("aideck", (long)((int)aiDeckId)));
			player.AddAttribute(ProtocolHelper.CreateAttribute("request_guid", requestGuid));
			GameProperties gameProperty = new GameProperties();
			AttributeFilter attributeFilter = new AttributeFilter();
			attributeFilter.SetOp(AttributeFilter.Types.Operation.MATCH_ALL);
			if (BattleNet.IsVersionInt())
			{
				int num = 0;
				if (!int.TryParse(BattleNet.GetVersion(), out num))
				{
					LogAdapter.Log(LogLevel.Error, string.Concat("Could not convert BattleNetVersion to int: ", BattleNet.GetVersion()));
				}
				attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("version", (long)num));
			}
			else
			{
				attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("version", BattleNet.GetVersion()));
			}
			attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("GameType", (long)gameType));
			if (setScenarioIdAttr)
			{
				attributeFilter.AddAttribute(ProtocolHelper.CreateAttribute("ScenarioId", (long)scenario));
			}
			gameProperty.SetFilter(attributeFilter);
			gameProperty.AddCreationAttributes(ProtocolHelper.CreateAttribute("type", (long)gameType));
			gameProperty.AddCreationAttributes(ProtocolHelper.CreateAttribute("scenario", (long)scenario));
			FindGameRequest findGameRequest = new FindGameRequest();
			findGameRequest.AddPlayer(player);
			findGameRequest.SetProperties(gameProperty);
			findGameRequest.SetAdvancedNotification(true);
			FindGameRequest findGameRequest1 = findGameRequest;
			this.PrintFindGameRequest(findGameRequest1);
			this.IsFindGamePending = true;
			this.m_rpcConnection.QueueRequest(this.m_gameMasterService.Id, 3, findGameRequest1, new RPCContextDelegate(this.FindGameCallback), 0);
		}

		private void FindGameCallback(RPCContext context)
		{
			this.IsFindGamePending = false;
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			base.ApiLog.LogDebug(string.Concat("Find Game Callback, status=", status));
			if (status != BattleNetErrors.ERROR_OK)
			{
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnFindGame, status, 0);
				return;
			}
			FindGameResponse findGameResponse = FindGameResponse.ParseFrom(context.Payload);
			if (findGameResponse.HasRequestId)
			{
				this.s_gameRequest = findGameResponse.RequestId;
			}
		}

		public void GameEntryHandler(Notification notification)
		{
			base.ApiLog.LogDebug("GAME_ENTRY");
			string host = null;
			int port = 0;
			string stringValue = null;
			int intValue = 0;
			int num = 0;
			string str = null;
			bool boolValue = false;
			string stringValue1 = null;
			foreach (bnet.protocol.attribute.Attribute attributeList in notification.AttributeList)
			{
				if (attributeList.Name.Equals("connection_info") && attributeList.Value.HasMessageValue)
				{
					ConnectInfo connectInfo = ConnectInfo.ParseFrom(attributeList.Value.MessageValue);
					host = connectInfo.Host;
					port = connectInfo.Port;
					if (connectInfo.HasToken)
					{
						str = Encoding.UTF8.GetString(connectInfo.Token);
					}
					foreach (bnet.protocol.attribute.Attribute attribute in connectInfo.AttributeList)
					{
						if (attribute.Name.Equals("version") && attribute.Value.HasStringValue)
						{
							stringValue = attribute.Value.StringValue;
						}
						else if (attribute.Name.Equals("game") && attribute.Value.HasIntValue)
						{
							intValue = (int)attribute.Value.IntValue;
						}
						else if (attribute.Name.Equals("id") && attribute.Value.HasIntValue)
						{
							num = (int)attribute.Value.IntValue;
						}
						else if (!attribute.Name.Equals("resumable") || !attribute.Value.HasBoolValue)
						{
							if (!attribute.Name.Equals("spectator_password") || !attribute.Value.HasStringValue)
							{
								continue;
							}
							stringValue1 = attribute.Value.StringValue;
						}
						else
						{
							boolValue = attribute.Value.BoolValue;
						}
					}
				}
				else if (!attributeList.Name.Equals("game_handle") || !attributeList.Value.HasMessageValue)
				{
					if (!attributeList.Name.Equals("sender_id") || !attributeList.Value.HasMessageValue)
					{
						continue;
					}
					base.ApiLog.LogDebug("sender_id");
				}
				else
				{
					GameHandle gameHandle = GameHandle.ParseFrom(attributeList.Value.MessageValue);
					this.m_battleNet.Channel.JoinChannel(gameHandle.GameId, ChannelAPI.ChannelType.GAME_CHANNEL);
				}
			}
			GameServerInfo gameServerInfo = new GameServerInfo()
			{
				Address = host,
				Port = port,
				AuroraPassword = str,
				Version = stringValue,
				GameHandle = intValue,
				ClientHandle = (long)num,
				Resumable = boolValue,
				SpectatorPassword = stringValue1
			};
			this.AddQueueEvent(QueueEvent.Type.QUEUE_GAME_STARTED, 0, 0, 0, gameServerInfo);
		}

		public void GameLeft(ChannelAPI.ChannelReferenceObject channelRefObject, RemoveNotification notification)
		{
			base.ApiLog.LogDebug(string.Concat(new object[] { "GameLeft ChannelID: ", channelRefObject.m_channelData.m_channelId, " notification: ", notification }));
			if (this.s_gameRequest != 0)
			{
				this.s_gameRequest = (ulong)0;
			}
		}

		public void GetAllValuesForAttribute(string attributeKey, int context)
		{
			GetAllValuesForAttributeRequest getAllValuesForAttributeRequest = new GetAllValuesForAttributeRequest()
			{
				AttributeKey = attributeKey
			};
			if (this.m_rpcConnection == null)
			{
				base.ApiLog.LogError(string.Concat("GetAllValuesForAttribute could not send, connection not valid : ", getAllValuesForAttributeRequest.ToString()));
				return;
			}
			RPCContext rPCContext = this.m_rpcConnection.QueueRequest(this.m_gameUtilitiesService.Id, 10, getAllValuesForAttributeRequest, new RPCContextDelegate(this.GetAllValuesForAttributeCallback), 0);
			rPCContext.Context = context;
		}

		private void GetAllValuesForAttributeCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnGetAllValuesForAttribute, status, 0);
				return;
			}
			GetAllValuesForAttributeResponse getAllValuesForAttributeResponse = ProtobufUtil.ParseFrom<GetAllValuesForAttributeResponse>(context.Payload, 0, -1);
			this.m_getAllValuesForAttributeResults.Enqueue(new GamesAPI.GetAllValuesForAttributeResult(getAllValuesForAttributeResponse, context.Context));
		}

		public static ClientResponse GetClientResponseFromContext(RPCContext context)
		{
			return ClientResponse.ParseFrom(context.Payload);
		}

		public QueueEvent GetQueueEvent()
		{
			QueueEvent queueEvent = null;
			Queue<QueueEvent> mQueueEvents = this.m_queueEvents;
			Monitor.Enter(mQueueEvents);
			try
			{
				if (this.m_queueEvents.Count > 0)
				{
					queueEvent = this.m_queueEvents.Dequeue();
				}
			}
			finally
			{
				Monitor.Exit(mQueueEvents);
			}
			return queueEvent;
		}

		private void HandleGameUtilityServerRequest(RPCContext context)
		{
			base.ApiLog.LogDebug("RPC Called: GameUtilityServer");
		}

		private void HandleNotifyGameFoundRequest(RPCContext context)
		{
			base.ApiLog.LogDebug("RPC Called: NotifyGameFound");
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_gameUtilitiesService.Id, 6, new RPCContextDelegate(this.HandleGameUtilityServerRequest));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_gameFactorySubscriberService.Id, 1, new RPCContextDelegate(this.HandleNotifyGameFoundRequest));
		}

		public void MatchMakerEndHandler(Notification notification)
		{
			BattleNetErrors uintAttribute = (BattleNetErrors)((uint)ProtocolHelper.GetUintAttribute(notification.AttributeList, "error", (ulong)0, null));
			ulong num = ProtocolHelper.GetUintAttribute(notification.AttributeList, "game_request_id", (ulong)0, null);
			base.ApiLog.LogDebug("MM_END requestId={0} code={1}", new object[] { num, (uint)uintAttribute });
			QueueEvent.Type type = QueueEvent.Type.QUEUE_LEAVE;
			if (uintAttribute == BattleNetErrors.ERROR_OK)
			{
				type = QueueEvent.Type.QUEUE_LEAVE;
			}
			else if (uintAttribute != BattleNetErrors.ERROR_GAME_MASTER_GAME_ENTRY_CANCELLED)
			{
				type = (uintAttribute != BattleNetErrors.ERROR_GAME_MASTER_GAME_ENTRY_ABORTED_CLIENT_DROPPED ? QueueEvent.Type.QUEUE_AMM_ERROR : QueueEvent.Type.ABORT_CLIENT_DROPPED);
			}
			else
			{
				type = QueueEvent.Type.QUEUE_CANCEL;
			}
			base.ApiLog.LogDebug("MatchMakerEndHandler event={0} code={1}", new object[] { type, (uint)uintAttribute });
			this.AddQueueEvent(type, 0, 0, (int)uintAttribute, null);
		}

		public void MatchMakerStartHandler(Notification notification)
		{
			base.ApiLog.LogDebug("MM_START");
			this.AddQueueEvent(QueueEvent.Type.QUEUE_ENTER, 0, 0, 0, null);
		}

		public GamesAPI.GetAllValuesForAttributeResult NextGetAllValuesForAttributeResult()
		{
			if (this.m_getAllValuesForAttributeResults.Count <= 0)
			{
				return null;
			}
			return this.m_getAllValuesForAttributeResults.Dequeue();
		}

		public GamesAPI.UtilResponse NextUtilPacket()
		{
			if (this.m_utilPackets.Count <= 0)
			{
				return null;
			}
			return this.m_utilPackets.Dequeue();
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this.s_gameRequest = (ulong)0;
			this.m_queueEvents.Clear();
			this.m_utilPackets.Clear();
			this.m_getAllValuesForAttributeResults.Clear();
		}

		private void PrintFindGameRequest(FindGameRequest request)
		{
			string str;
			string str1 = "FindGameRequest: { ";
			int playerCount = request.PlayerCount;
			for (int i = 0; i < playerCount; i++)
			{
				Player item = request.Player[i];
				str1 = string.Concat(str1, this.PrintPlayer(item));
			}
			if (request.HasFactoryId)
			{
				str = str1;
				str1 = string.Concat(new object[] { str, "Factory Id: ", request.FactoryId, " " });
			}
			if (request.HasProperties)
			{
				str1 = string.Concat(str1, this.PrintGameProperties(request.Properties));
			}
			if (request.HasObjectId)
			{
				str = str1;
				str1 = string.Concat(new object[] { str, "Obj Id: ", request.ObjectId, " " });
			}
			if (request.HasRequestId)
			{
				str = str1;
				str1 = string.Concat(new object[] { str, "Request Id: ", request.RequestId, " " });
			}
			str1 = string.Concat(str1, "}");
			base.ApiLog.LogDebug(str1);
		}

		private string PrintGameMasterAttributeFilter(AttributeFilter filter)
		{
			string str;
			string str1 = "Attribute Filter: [";
			switch (filter.Op)
			{
				case AttributeFilter.Types.Operation.MATCH_NONE:
				{
					str = "MATCH_NONE";
					break;
				}
				case AttributeFilter.Types.Operation.MATCH_ANY:
				{
					str = "MATCH_ANY";
					break;
				}
				case AttributeFilter.Types.Operation.MATCH_ALL:
				{
					str = "MATCH_ALL";
					break;
				}
				case AttributeFilter.Types.Operation.MATCH_ALL_MOST_SPECIFIC:
				{
					str = "MATCH_ALL_MOST_SPECIFIC";
					break;
				}
				default:
				{
					str = "UNKNOWN";
					break;
				}
			}
			str1 = string.Concat(str1, "Operation: ", str, " ");
			str1 = string.Concat(str1, "Attributes: [");
			int attributeCount = filter.AttributeCount;
			for (int i = 0; i < attributeCount; i++)
			{
				bnet.protocol.attribute.Attribute item = filter.Attribute[i];
				string str2 = str1;
				str1 = string.Concat(new object[] { str2, "Name: ", item.Name, " Value: ", item.Value, " " });
			}
			str1 = string.Concat(str1, "] ");
			return str1;
		}

		private string PrintGameMasterIdentity(Identity identity)
		{
			string str;
			string empty = string.Empty;
			empty = string.Concat(empty, "Identity: [");
			if (identity.HasAccountId)
			{
				str = empty;
				empty = string.Concat(new object[] { str, "Acct Id: ", identity.AccountId.High, ":", identity.AccountId.Low, " " });
			}
			if (identity.HasGameAccountId)
			{
				str = empty;
				empty = string.Concat(new object[] { str, "Game Acct Id: ", identity.GameAccountId.High, ":", identity.GameAccountId.Low, " " });
			}
			empty = string.Concat(empty, "] ");
			return empty;
		}

		private string PrintGameProperties(GameProperties properties)
		{
			string str;
			string empty = string.Empty;
			empty = "Game Properties: [";
			int creationAttributesCount = properties.CreationAttributesCount;
			empty = string.Concat(empty, "Creation Attributes: ");
			for (int i = 0; i < creationAttributesCount; i++)
			{
				bnet.protocol.attribute.Attribute item = properties.CreationAttributes[i];
				str = empty;
				empty = string.Concat(new object[] { str, "[Name: ", item.Name, " Value: ", item.Value, "] " });
			}
			if (properties.HasFilter)
			{
				this.PrintGameMasterAttributeFilter(properties.Filter);
			}
			if (properties.HasCreate)
			{
				str = empty;
				empty = string.Concat(new object[] { str, "Create New Game?: ", properties.Create, " " });
			}
			if (properties.HasOpen)
			{
				str = empty;
				empty = string.Concat(new object[] { str, "Game Is Open?: ", properties.Open, " " });
			}
			if (properties.HasProgramId)
			{
				empty = string.Concat(empty, "Program Id(4CC): ", properties.ProgramId);
			}
			return empty;
		}

		private string PrintPlayer(Player player)
		{
			string empty = string.Empty;
			empty = string.Concat(empty, "Player: [");
			if (player.HasIdentity)
			{
				this.PrintGameMasterIdentity(player.Identity);
			}
			int attributeCount = player.AttributeCount;
			empty = string.Concat(empty, "Attributes: ");
			for (int i = 0; i < attributeCount; i++)
			{
				bnet.protocol.attribute.Attribute item = player.Attribute[i];
				string str = empty;
				empty = string.Concat(new object[] { str, "[Name: ", item.Name, " Value: ", item.Value, "] " });
			}
			empty = string.Concat(empty, "] ");
			return empty;
		}

		public void QueueEntryHandler(Notification notification)
		{
			base.ApiLog.LogDebug(string.Concat("QueueEntryHandler: ", notification));
			this.QueueUpdate(QueueEvent.Type.QUEUE_DELAY, notification);
		}

		public void QueueExitHandler(Notification notification)
		{
			BattleNetErrors uintAttribute = (BattleNetErrors)((uint)ProtocolHelper.GetUintAttribute(notification.AttributeList, "error", (ulong)0, null));
			ulong num = ProtocolHelper.GetUintAttribute(notification.AttributeList, "game_request_id", (ulong)0, null);
			base.ApiLog.LogDebug("QueueExitHandler: requestId={0} code={1}", new object[] { num, (uint)uintAttribute });
			if (uintAttribute != BattleNetErrors.ERROR_OK)
			{
				QueueEvent.Type type = QueueEvent.Type.QUEUE_DELAY_ERROR;
				base.ApiLog.LogDebug("QueueExitHandler event={0} code={1}", new object[] { type, (uint)uintAttribute });
				this.AddQueueEvent(type, 0, 0, (int)uintAttribute, null);
			}
		}

		private void QueueUpdate(QueueEvent.Type queueType, Notification notification)
		{
			int uintValue = 0;
			int num = 0;
			foreach (bnet.protocol.attribute.Attribute attributeList in notification.AttributeList)
			{
				if (!(attributeList.Name == "min_wait") || !attributeList.Value.HasUintValue)
				{
					if (!(attributeList.Name == "max_wait") || !attributeList.Value.HasUintValue)
					{
						continue;
					}
					num = (int)attributeList.Value.UintValue;
				}
				else
				{
					uintValue = (int)attributeList.Value.UintValue;
				}
			}
			this.AddQueueEvent(queueType, uintValue, num, 0, null);
		}

		public void QueueUpdateHandler(Notification notification)
		{
			base.ApiLog.LogDebug(string.Concat("QueueUpdateHandler: ", notification));
			this.QueueUpdate(QueueEvent.Type.QUEUE_UPDATE, notification);
		}

		public void SendClientRequest(ClientRequest request, RPCContextDelegate callback = null)
		{
			this.m_rpcConnection.QueueRequest(this.m_gameUtilitiesService.Id, 1, request, (callback == null ? new RPCContextDelegate(this.ClientRequestCallback) : callback), 0);
		}

		public void SendUtilPacket(int packetId, int systemId, byte[] bytes, int size, int subID, int context, ulong route)
		{
			ClientRequest clientRequest = this.CreateClientRequest(packetId, systemId, bytes, route);
			if (this.m_rpcConnection == null)
			{
				base.ApiLog.LogError(string.Concat("SendUtilPacket could not send, connection not valid : ", clientRequest.ToString()));
				return;
			}
			if (!GamesAPI.warnComplete)
			{
				base.ApiLog.LogWarning("SendUtilPacket: need to map context to RPCContext");
				GamesAPI.warnComplete = true;
			}
			RPCContext rPCContext = this.m_rpcConnection.QueueRequest(this.m_gameUtilitiesService.Id, 1, clientRequest, new RPCContextDelegate(this.UtilClientRequestCallback), 0);
			rPCContext.SystemId = systemId;
			rPCContext.Context = context;
		}

		private void UtilClientRequestCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnClientRequest, status, 0);
				return;
			}
			ClientResponse clientResponse = ClientResponse.ParseFrom(context.Payload);
			if (clientResponse.AttributeCount < 2)
			{
				base.ApiLog.LogError("Malformed Attribute in Util Packet: missing values");
			}
			else
			{
				bnet.protocol.attribute.Attribute item = clientResponse.AttributeList[0];
				bnet.protocol.attribute.Attribute attribute = clientResponse.AttributeList[1];
				if (!item.Value.HasIntValue || !attribute.Value.HasBlobValue)
				{
					base.ApiLog.LogError("Malformed Attribute in Util Packet: incorrect values");
				}
				this.m_utilPackets.Enqueue(new GamesAPI.UtilResponse(clientResponse, context.Context));
			}
		}

		private class CancelGameContext
		{
			private ulong m_gameRequestId;

			public CancelGameContext(ulong gameRequestId)
			{
				this.m_gameRequestId = gameRequestId;
			}

			public void CancelGameCallback(RPCContext context)
			{
				BattleNetCSharp battleNetCSharp = BattleNet.Get() as BattleNetCSharp;
				if (battleNetCSharp == null || battleNetCSharp.Games == null)
				{
					return;
				}
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				battleNetCSharp.Games.ApiLog.LogDebug(string.Concat("CancelGameCallback, status=", status));
				if (status != BattleNetErrors.ERROR_OK && status != BattleNetErrors.ERROR_GAME_MASTER_INVALID_GAME)
				{
					battleNetCSharp.EnqueueErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnCancelGame, status, 0);
				}
				else if (battleNetCSharp.Games.IsFindGamePending || battleNetCSharp.Games.CurrentGameRequest != 0 && battleNetCSharp.Games.CurrentGameRequest != this.m_gameRequestId)
				{
					battleNetCSharp.Games.ApiLog.LogDebug("CancelGameCallback received for id={0} but is not the current gameRequest={1}, ignoring it.", new object[] { this.m_gameRequestId, battleNetCSharp.Games.CurrentGameRequest });
				}
				else
				{
					battleNetCSharp.Games.CurrentGameRequest = (ulong)0;
					battleNetCSharp.Games.AddQueueEvent(QueueEvent.Type.QUEUE_CANCEL, 0, 0, 0, null);
				}
			}
		}

		public class GetAllValuesForAttributeResult
		{
			public GetAllValuesForAttributeResponse m_response;

			public int m_context;

			public GetAllValuesForAttributeResult(GetAllValuesForAttributeResponse response, int context)
			{
				this.m_response = response;
				this.m_context = context;
			}
		}

		public class UtilResponse
		{
			public ClientResponse m_response;

			public int m_context;

			public UtilResponse(ClientResponse response, int context)
			{
				this.m_response = response;
				this.m_context = context;
			}
		}
	}
}