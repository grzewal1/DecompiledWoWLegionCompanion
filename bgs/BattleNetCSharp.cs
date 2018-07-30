using bgs.RPCServices;
using bgs.Shared.Util;
using bgs.types;
using bnet.protocol;
using bnet.protocol.account;
using bnet.protocol.attribute;
using bnet.protocol.authentication;
using bnet.protocol.connection;
using bnet.protocol.game_utilities;
using bnet.protocol.notification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class BattleNetCSharp : IBattleNet
	{
		private BattleNetLogSource m_logSource = new BattleNetLogSource("Main");

		private FriendsAPI m_friendAPI;

		private PresenceAPI m_presenceAPI;

		private ChannelAPI m_channelAPI;

		private GamesAPI m_gamesAPI;

		private PartyAPI m_partyAPI;

		private ChallengeAPI m_challengeAPI;

		private WhisperAPI m_whisperAPI;

		private NotificationAPI m_notificationAPI;

		private BroadcastAPI m_broadcastAPI;

		private AccountAPI m_accountAPI;

		private AuthenticationAPI m_authenticationAPI;

		private LocalStorageAPI m_localStorageAPI;

		private ResourcesAPI m_resourcesAPI;

		private ProfanityAPI m_profanityAPI;

		private Dictionary<string, BattleNetCSharp.NotificationHandler> m_notificationHandlers;

		private Dictionary<BattleNetCSharp.ConnectionState, BattleNetCSharp.AuroraStateHandler> m_stateHandlers;

		private List<ServiceDescriptor> m_importedServices;

		private List<ServiceDescriptor> m_exportedServices;

		private List<BattleNet.BnetEvent> m_bnetEvents;

		private readonly long m_keepAliveIntervalMilliseconds = (long)20000;

		private readonly DateTime m_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private long m_serverTimeUTCAtConnectMicroseconds;

		private long m_serverTimeDeltaUTCSeconds;

		private List<BattleNetAPI> m_apiList;

		private uint m_connectedRegion = -1;

		private readonly string programCode = (new FourCC("WoW")).GetString();

		private readonly string launchOptionBasePath = "Software\\Blizzard Entertainment\\Battle.net\\Launch Options\\";

		protected ClientInterface m_clientInterface;

		private Stopwatch m_stopwatch = new Stopwatch();

		private int m_shutdownInMinutes;

		private string m_auroraEnvironment;

		private int m_auroraPort;

		private string m_userEmailAddress;

		private bool m_initialized;

		private RPCConnection m_rpcConnection;

		public BattleNetCSharp.ConnectionState m_connectionState;

		private List<BnetErrorInfo> m_errorEvents = new List<BnetErrorInfo>();

		private const int DEFAULT_PORT = 1119;

		public const string s_programName = "WoW";

		private ServiceDescriptor m_connectionService = new ConnectionService();

		private ServiceDescriptor m_notificationService = new bgs.RPCServices.NotificationService();

		private ServiceDescriptor m_notificationListenerService = new NotificationListenerService();

		public AccountAPI Account
		{
			get
			{
				return this.m_accountAPI;
			}
		}

		public bnet.protocol.EntityId AccountId
		{
			get
			{
				return this.m_authenticationAPI.AccountId;
			}
		}

		public BroadcastAPI Broadcast
		{
			get
			{
				return this.m_broadcastAPI;
			}
		}

		public ChallengeAPI Challenge
		{
			get
			{
				return this.m_challengeAPI;
			}
		}

		public ChannelAPI Channel
		{
			get
			{
				return this.m_channelAPI;
			}
		}

		public long CurrentUTCServerTimeSeconds
		{
			get
			{
				return this.GetCurrentTimeSecondsSinceUnixEpoch() + this.m_serverTimeDeltaUTCSeconds;
			}
		}

		public FriendsAPI Friends
		{
			get
			{
				return this.m_friendAPI;
			}
		}

		public bnet.protocol.EntityId GameAccountId
		{
			get
			{
				return this.m_authenticationAPI.GameAccountId;
			}
		}

		public GamesAPI Games
		{
			get
			{
				return this.m_gamesAPI;
			}
		}

		protected string launchOptionPath
		{
			get
			{
				return string.Concat(this.launchOptionBasePath, this.programCode);
			}
		}

		public LocalStorageAPI LocalStorage
		{
			get
			{
				return this.m_localStorageAPI;
			}
		}

		public NotificationAPI Notification
		{
			get
			{
				return this.m_notificationAPI;
			}
		}

		public ServiceDescriptor NotificationService
		{
			get
			{
				return this.m_notificationService;
			}
		}

		public PartyAPI Party
		{
			get
			{
				return this.m_partyAPI;
			}
		}

		public PresenceAPI Presence
		{
			get
			{
				return this.m_presenceAPI;
			}
		}

		public ResourcesAPI Resources
		{
			get
			{
				return this.m_resourcesAPI;
			}
		}

		public long ServerTimeUTCAtConnectMicroseconds
		{
			get
			{
				return this.m_serverTimeUTCAtConnectMicroseconds;
			}
		}

		public WhisperAPI Whisper
		{
			get
			{
				return this.m_whisperAPI;
			}
		}

		public BattleNetCSharp()
		{
			this.m_notificationHandlers = new Dictionary<string, BattleNetCSharp.NotificationHandler>();
			this.m_stateHandlers = new Dictionary<BattleNetCSharp.ConnectionState, BattleNetCSharp.AuroraStateHandler>();
			this.m_importedServices = new List<ServiceDescriptor>();
			this.m_exportedServices = new List<ServiceDescriptor>();
			this.m_apiList = new List<BattleNetAPI>();
			this.m_bnetEvents = new List<BattleNet.BnetEvent>();
			this.m_friendAPI = new FriendsAPI(this);
			this.m_presenceAPI = new PresenceAPI(this);
			this.m_channelAPI = new ChannelAPI(this);
			this.m_gamesAPI = new GamesAPI(this);
			this.m_partyAPI = new PartyAPI(this);
			this.m_challengeAPI = new ChallengeAPI(this);
			this.m_whisperAPI = new WhisperAPI(this);
			this.m_notificationAPI = new NotificationAPI(this);
			this.m_broadcastAPI = new BroadcastAPI(this);
			this.m_accountAPI = new AccountAPI(this);
			this.m_authenticationAPI = new AuthenticationAPI(this);
			this.m_localStorageAPI = new LocalStorageAPI(this);
			this.m_resourcesAPI = new ResourcesAPI(this);
			this.m_profanityAPI = new ProfanityAPI(this);
			this.m_notificationHandlers.Add("GQ_ENTRY", new BattleNetCSharp.NotificationHandler(this.m_gamesAPI.QueueEntryHandler));
			this.m_notificationHandlers.Add("GQ_UPDATE", new BattleNetCSharp.NotificationHandler(this.m_gamesAPI.QueueUpdateHandler));
			this.m_notificationHandlers.Add("GQ_EXIT", new BattleNetCSharp.NotificationHandler(this.m_gamesAPI.QueueExitHandler));
			this.m_notificationHandlers.Add("MM_START", new BattleNetCSharp.NotificationHandler(this.m_gamesAPI.MatchMakerStartHandler));
			this.m_notificationHandlers.Add("MM_END", new BattleNetCSharp.NotificationHandler(this.m_gamesAPI.MatchMakerEndHandler));
			this.m_notificationHandlers.Add("G_RESULT", new BattleNetCSharp.NotificationHandler(this.m_gamesAPI.GameEntryHandler));
			this.m_notificationHandlers.Add("WHISPER", new BattleNetCSharp.NotificationHandler(this.m_whisperAPI.OnWhisper));
			this.m_notificationHandlers.Add("BROADCAST", new BattleNetCSharp.NotificationHandler(this.m_broadcastAPI.OnBroadcast));
			this.m_notificationHandlers.Add("WTCG.UtilNotificationMessage", new BattleNetCSharp.NotificationHandler((bnet.protocol.notification.Notification n) => this.m_notificationAPI.OnNotification("WTCG.UtilNotificationMessage", n)));
			this.m_broadcastAPI.RegisterListener(new BroadcastAPI.BroadcastCallback(this.OnBroadcastReceived));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.Connect, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_Connect));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.InitRPC, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_InitRPC));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.WaitForInitRPC, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_WaitForInitRPC));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.Logon, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_Logon));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.WaitForLogon, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_WaitForLogon));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.WaitForGameAccountSelect, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_WaitForGameAccountSelect));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.WaitForAPIToInitialize, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_WaitForAPIToInitialize));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.Ready, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_Ready));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.Disconnected, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_Disconnected));
			this.m_stateHandlers.Add(BattleNetCSharp.ConnectionState.Error, new BattleNetCSharp.AuroraStateHandler(this.AuroraStateHandler_Error));
			this.m_apiList.Add(this.m_gamesAPI);
			this.m_apiList.Add(this.m_challengeAPI);
			this.m_apiList.Add(this.m_notificationAPI);
			this.m_apiList.Add(this.m_broadcastAPI);
			this.m_apiList.Add(this.m_accountAPI);
			this.m_apiList.Add(this.m_authenticationAPI);
			this.m_apiList.Add(this.m_localStorageAPI);
			this.m_apiList.Add(this.m_resourcesAPI);
		}

		public void AcceptFriendlyChallenge(ref bgs.types.EntityId partyId)
		{
			this.m_partyAPI.AcceptFriendlyChallenge(partyId.ToProtocol());
		}

		public void AcceptPartyInvite(ulong invitationId)
		{
			this.m_partyAPI.AcceptPartyInvite(invitationId);
		}

		public void AnswerChallenge(ulong challengeID, string answer)
		{
			this.m_challengeAPI.AnswerChallenge(challengeID, answer);
		}

		public void ApplicationWasPaused()
		{
			this.m_logSource.LogWarning("Application was paused.");
			if (this.m_rpcConnection != null)
			{
				this.m_rpcConnection.Update();
			}
		}

		public void ApplicationWasUnpaused()
		{
			this.m_logSource.LogWarning("Application was unpaused.");
		}

		public void AppQuit()
		{
			this.RequestCloseAurora();
		}

		public void AssignPartyRole(bgs.types.EntityId partyId, bgs.types.EntityId memberId, uint roleId)
		{
			this.m_partyAPI.AssignPartyRole(partyId.ToProtocol(), memberId.ToProtocol(), roleId);
		}

		public void AuroraStateHandler_Connect()
		{
		}

		public void AuroraStateHandler_Disconnected()
		{
			this.m_logSource.LogError("Aurora State Event: Disonnected");
		}

		public void AuroraStateHandler_Error()
		{
			this.m_logSource.LogError("Aurora State Event: Error");
		}

		public void AuroraStateHandler_InitRPC()
		{
			this.m_importedServices.Clear();
			this.m_exportedServices.Clear();
			ConnectRequest connectRequest = new ConnectRequest();
			this.m_importedServices.Add(this.m_authenticationAPI.AuthServerService);
			this.m_importedServices.Add(this.m_gamesAPI.GameUtilityService);
			this.m_importedServices.Add(this.m_gamesAPI.GameMasterService);
			this.m_importedServices.Add(this.m_notificationService);
			this.m_importedServices.Add(this.m_presenceAPI.PresenceService);
			this.m_importedServices.Add(this.m_channelAPI.ChannelService);
			this.m_importedServices.Add(this.m_channelAPI.ChannelOwnerService);
			this.m_importedServices.Add(this.m_channelAPI.ChannelInvitationService);
			this.m_importedServices.Add(this.m_friendAPI.FriendsService);
			this.m_importedServices.Add(this.m_challengeAPI.ChallengeService);
			this.m_importedServices.Add(this.m_accountAPI.AccountService);
			this.m_importedServices.Add(this.m_resourcesAPI.ResourcesService);
			this.m_exportedServices.Add(this.m_authenticationAPI.AuthClientService);
			this.m_exportedServices.Add(this.m_gamesAPI.GameMasterSubscriberService);
			this.m_exportedServices.Add(this.m_gamesAPI.GameFactorySubscribeService);
			this.m_exportedServices.Add(this.m_notificationListenerService);
			this.m_exportedServices.Add(this.m_channelAPI.ChannelSubscriberService);
			this.m_exportedServices.Add(this.m_channelAPI.ChannelInvitationNotifyService);
			this.m_exportedServices.Add(this.m_friendAPI.FriendsNotifyService);
			this.m_exportedServices.Add(this.m_challengeAPI.ChallengeNotifyService);
			this.m_exportedServices.Add(this.m_accountAPI.AccountNotifyService);
			connectRequest.SetBindRequest(this.CreateBindRequest(this.m_importedServices, this.m_exportedServices));
			this.m_rpcConnection.QueueRequest(this.m_connectionService.Id, 1, connectRequest, new RPCContextDelegate(this.OnConnectCallback), 0);
			this.SwitchToState(BattleNetCSharp.ConnectionState.WaitForInitRPC);
		}

		public void AuroraStateHandler_Logon()
		{
			this.m_logSource.LogDebug("Sending Logon request");
			LogonRequest logonRequest = this.CreateLogonRequest();
			this.m_rpcConnection.QueueRequest(this.m_authenticationAPI.AuthServerService.Id, 1, logonRequest, null, 0);
			this.SwitchToState(BattleNetCSharp.ConnectionState.WaitForLogon);
		}

		public void AuroraStateHandler_Ready()
		{
		}

		public void AuroraStateHandler_Unhandled()
		{
			this.m_logSource.LogError("Unhandled Aurora State");
		}

		public void AuroraStateHandler_WaitForAPIToInitialize()
		{
			this.SwitchToState(BattleNetCSharp.ConnectionState.Ready);
		}

		public void AuroraStateHandler_WaitForGameAccountSelect()
		{
		}

		public void AuroraStateHandler_WaitForInitRPC()
		{
		}

		public void AuroraStateHandler_WaitForLogon()
		{
			if (this.m_authenticationAPI.AuthenticationFailure())
			{
				this.EnqueueErrorInfo(BnetFeature.Bnet, BnetFeatureEvent.Bnet_OnDisconnected, BattleNetErrors.ERROR_NO_AUTH, 0);
			}
		}

		public int BattleNetStatus()
		{
			switch (this.m_connectionState)
			{
				case BattleNetCSharp.ConnectionState.Disconnected:
				{
					return 0;
				}
				case BattleNetCSharp.ConnectionState.Connect:
				case BattleNetCSharp.ConnectionState.InitRPC:
				case BattleNetCSharp.ConnectionState.WaitForInitRPC:
				case BattleNetCSharp.ConnectionState.Logon:
				case BattleNetCSharp.ConnectionState.WaitForLogon:
				case BattleNetCSharp.ConnectionState.WaitForGameAccountSelect:
				case BattleNetCSharp.ConnectionState.WaitForAPIToInitialize:
				{
					return 1;
				}
				case BattleNetCSharp.ConnectionState.Ready:
				{
					return 4;
				}
				case BattleNetCSharp.ConnectionState.Error:
				{
					return 3;
				}
			}
			this.m_logSource.LogError("Unknown Battle.Net Status");
			return 0;
		}

		public void CancelChallenge(ulong challengeID)
		{
			this.m_challengeAPI.CancelChallenge(challengeID);
		}

		public void CancelFindGame()
		{
			this.m_gamesAPI.CancelFindGame();
		}

		public bool CheckWebAuth(out string url)
		{
			url = null;
			if (this.m_challengeAPI != null && this.InState(BattleNetCSharp.ConnectionState.WaitForLogon))
			{
				ExternalChallenge nextExternalChallenge = this.m_challengeAPI.GetNextExternalChallenge();
				if (nextExternalChallenge != null)
				{
					url = nextExternalChallenge.URL;
					this.m_logSource.LogDebug("Delivering a challenge url={0}", new object[] { url });
					return true;
				}
			}
			return false;
		}

		public void ClearBnetEvents()
		{
			this.m_bnetEvents.Clear();
		}

		public void ClearChallenges()
		{
			this.m_challengeAPI.ClearChallenges();
		}

		public void ClearErrors()
		{
			this.m_errorEvents.Clear();
		}

		public void ClearFriendsUpdates()
		{
			this.m_friendAPI.ClearFriendsUpdates();
		}

		public void ClearNotifications()
		{
			this.m_notificationAPI.ClearNotifications();
		}

		public void ClearPartyAttribute(bgs.types.EntityId partyId, string attributeKey)
		{
			this.m_partyAPI.ClearPartyAttribute(partyId.ToProtocol(), attributeKey);
		}

		public void ClearPartyListenerEvents()
		{
			this.m_partyAPI.ClearPartyListenerEvents();
		}

		public void ClearPartyUpdates()
		{
			this.m_partyAPI.ClearPartyUpdates();
		}

		public void ClearPresence()
		{
			this.m_presenceAPI.ClearPresence();
		}

		public void ClearWhispers()
		{
			this.m_whisperAPI.ClearWhispers();
		}

		public ClientInterface Client()
		{
			return this.m_clientInterface;
		}

		public void CloseAurora()
		{
			BattleNet.Log.LogError("CloseAurora() is deprecated in BattleNetCSharp. Use RequestCloseAurora() instead.");
		}

		public void ConnectAurora(string address, int port, SslParameters sslParams)
		{
			this.m_logSource.LogInfo("Sending connection request to {0}:{1}", new object[] { address, port });
			this.m_logSource.LogDebug("Aurora version is {0}", new object[] { this.GetVersion() });
			this.m_rpcConnection = new RPCConnection();
			this.m_connectionService.Id = 0;
			this.m_rpcConnection.serviceHelper.AddImportedService(this.m_connectionService.Id, this.m_connectionService);
			this.m_rpcConnection.serviceHelper.AddExportedService(this.m_connectionService.Id, this.m_connectionService);
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_connectionService.Id, 4, new RPCContextDelegate(this.HandleForceDisconnectRequest));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_connectionService.Id, 3, new RPCContextDelegate(this.HandleEchoRequest));
			this.m_rpcConnection.SetOnConnectHandler(new RPCConnection.OnConnectHandler(this.OnConnectHandlerCallback));
			this.m_rpcConnection.SetOnDisconnectHandler(new RPCConnection.OnDisconectHandler(this.OnDisconectHandlerCallback));
			this.m_rpcConnection.Connect(address, port, sslParams);
			this.SwitchToState(BattleNetCSharp.ConnectionState.InitRPC);
		}

		private BindRequest CreateBindRequest(List<ServiceDescriptor> imports, List<ServiceDescriptor> exports)
		{
			BindRequest bindRequest = new BindRequest();
			foreach (ServiceDescriptor import in imports)
			{
				bindRequest.AddImportedServiceHash(import.Hash);
			}
			uint num = 0;
			foreach (ServiceDescriptor export in exports)
			{
				UInt32 num1 = num + 1;
				num = num1;
				export.Id = num1;
				BoundService boundService = new BoundService();
				boundService.SetId(export.Id);
				boundService.SetHash(export.Hash);
				bindRequest.AddExportedService(boundService);
				this.m_rpcConnection.serviceHelper.AddExportedService(export.Id, export);
				this.m_logSource.LogDebug("Exporting service id={0} name={1}", new object[] { export.Id, export.Name });
			}
			return bindRequest;
		}

		private LogonRequest CreateLogonRequest()
		{
			LogonRequest logonRequest = new LogonRequest();
			logonRequest.SetProgram("WoW");
			logonRequest.SetLocale(this.Client().GetLocaleName());
			logonRequest.SetPlatform(this.Client().GetPlatformName());
			logonRequest.SetVersion(this.Client().GetAuroraVersionName());
			logonRequest.SetApplicationVersion(1);
			logonRequest.SetPublicComputer(false);
			logonRequest.SetAllowLogonQueueNotifications(true);
			string userAgent = this.Client().GetUserAgent();
			if (!string.IsNullOrEmpty(userAgent))
			{
				logonRequest.SetUserAgent(userAgent);
			}
			bool flag = false;
			logonRequest.SetWebClientVerification(true);
			flag = true;
			this.m_logSource.LogDebug("CreateLogonRequest SSL={0}", new object[] { flag });
			if (!string.IsNullOrEmpty(this.m_userEmailAddress))
			{
				this.m_logSource.LogDebug("Email = {0}", new object[] { this.m_userEmailAddress });
			}
			return logonRequest;
		}

		public void CreateParty(string szPartyType, int privacyLevel, byte[] creatorBlob)
		{
			this.m_partyAPI.CreateParty(szPartyType, privacyLevel, creatorBlob);
		}

		public long CurrentUTCTime()
		{
			return this.CurrentUTCServerTimeSeconds;
		}

		public void DeclineFriendlyChallenge(ref bgs.types.EntityId partyId)
		{
			this.m_partyAPI.DeclineFriendlyChallenge(partyId.ToProtocol(), "deny");
		}

		public void DeclinePartyInvite(ulong invitationId)
		{
			this.m_partyAPI.DeclinePartyInvite(invitationId);
		}

		public void DissolveParty(bgs.types.EntityId partyId)
		{
			this.m_partyAPI.DissolveParty(partyId.ToProtocol());
		}

		public void EnqueueErrorInfo(BnetFeature feature, BnetFeatureEvent featureEvent, BattleNetErrors error, int context = 0)
		{
			Log.BattleNet.Print(LogLevel.Warning, string.Format("Enqueuing BattleNetError {0} {1} code={2} context={3}", new object[] { feature.ToString(), featureEvent.ToString(), new Error(error), context }));
			this.m_errorEvents.Add(new BnetErrorInfo(feature, featureEvent, error));
		}

		public string FilterProfanity(string unfiltered)
		{
			return this.m_profanityAPI.FilterProfanity(unfiltered);
		}

		public void FindGame(byte[] requestGuid, int gameType, int missionId, long deckId, long aiDeckId, bool setScenarioIdAttr)
		{
			this.m_gamesAPI.FindGame(requestGuid, gameType, missionId, deckId, aiDeckId, setScenarioIdAttr);
		}

		public string GetAccountCountry()
		{
			return this.Account.GetAccountCountry();
		}

		public bnet.protocol.EntityId GetAccountEntity()
		{
			return this.m_authenticationAPI.AccountId;
		}

		public constants.BnetRegion GetAccountRegion()
		{
			if (this.Account.GetPreferredRegion() == -1)
			{
				return constants.BnetRegion.REGION_UNINITIALIZED;
			}
			return (constants.BnetRegion)this.Account.GetPreferredRegion();
		}

		public void GetAllPartyAttributes(bgs.types.EntityId partyId, out string[] allKeys)
		{
			this.m_partyAPI.GetAllPartyAttributes(partyId.ToProtocol(), out allKeys);
		}

		public void GetAllValuesForAttribute(string attributeKey, int context)
		{
			this.m_gamesAPI.GetAllValuesForAttribute(attributeKey, context);
		}

		public void GetBnetEvents([Out] BattleNet.BnetEvent[] bnetEvents)
		{
			this.m_bnetEvents.CopyTo(bnetEvents);
		}

		public int GetBnetEventsSize()
		{
			return this.m_bnetEvents.Count;
		}

		public void GetChallenges([Out] ChallengeInfo[] challenges)
		{
			this.m_challengeAPI.GetChallenges(challenges);
		}

		public int GetCountPartyMembers(bgs.types.EntityId partyId)
		{
			return this.m_partyAPI.GetCountPartyMembers(partyId.ToProtocol());
		}

		public constants.BnetRegion GetCurrentRegion()
		{
			if (this.m_connectedRegion == -1)
			{
				return constants.BnetRegion.REGION_UNINITIALIZED;
			}
			return (constants.BnetRegion)this.m_connectedRegion;
		}

		public long GetCurrentTimeSecondsSinceUnixEpoch()
		{
			return (long)(DateTime.UtcNow - this.m_unixEpoch).TotalSeconds;
		}

		public string GetEnvironment()
		{
			return this.m_auroraEnvironment;
		}

		public void GetErrors([Out] BnetErrorInfo[] errors)
		{
			this.m_errorEvents.Sort(new BattleNetCSharp.BnetErrorComparer());
			this.m_errorEvents.CopyTo(errors);
		}

		public int GetErrorsCount()
		{
			return this.m_errorEvents.Count;
		}

		public void GetFriendsInfo(ref FriendsInfo info)
		{
			this.m_friendAPI.GetFriendsInfo(ref info);
		}

		public void GetFriendsUpdates([Out] FriendsUpdate[] updates)
		{
			this.m_friendAPI.GetFriendsUpdates(updates);
		}

		public List<bnet.protocol.EntityId> GetGameAccountList()
		{
			return this.m_authenticationAPI.GetGameAccountList();
		}

		public void GetGameAccountState(GetGameAccountStateRequest request, RPCContextDelegate callback)
		{
			this.m_accountAPI.GetGameAccountState(request, callback);
		}

		public string GetLaunchOption(string key, bool encrypted)
		{
			string str;
			if (Registry.RetrieveString(this.launchOptionPath, key, out str, encrypted) == BattleNetErrors.ERROR_OK)
			{
				return str;
			}
			return string.Empty;
		}

		public BattleNetLogSource GetLogSource()
		{
			return this.m_logSource;
		}

		public int GetMaxPartyMembers(bgs.types.EntityId partyId)
		{
			return this.m_partyAPI.GetMaxPartyMembers(partyId.ToProtocol());
		}

		public bgs.types.EntityId GetMyAccountId()
		{
			bgs.types.EntityId entityId = new bgs.types.EntityId()
			{
				hi = this.AccountId.High,
				lo = this.AccountId.Low
			};
			return entityId;
		}

		public string GetMyBattleTag()
		{
			return this.m_authenticationAPI.BattleTag;
		}

		public bgs.types.EntityId GetMyGameAccountId()
		{
			bgs.types.EntityId entityId = new bgs.types.EntityId()
			{
				hi = this.GameAccountId.High,
				lo = this.GameAccountId.Low
			};
			return entityId;
		}

		public int GetNotificationCount()
		{
			return this.m_notificationAPI.GetNotificationCount();
		}

		public void GetNotifications([Out] BnetNotification[] notifications)
		{
			this.m_notificationAPI.GetNotifications(notifications);
		}

		public void GetPartyAttributeBlob(bgs.types.EntityId partyId, string attributeKey, out byte[] value)
		{
			this.m_partyAPI.GetPartyAttributeBlob(partyId.ToProtocol(), attributeKey, out value);
		}

		public bool GetPartyAttributeLong(bgs.types.EntityId partyId, string attributeKey, out long value)
		{
			return this.m_partyAPI.GetPartyAttributeLong(partyId.ToProtocol(), attributeKey, out value);
		}

		public void GetPartyAttributeString(bgs.types.EntityId partyId, string attributeKey, out string value)
		{
			this.m_partyAPI.GetPartyAttributeString(partyId.ToProtocol(), attributeKey, out value);
		}

		public void GetPartyInviteRequests(bgs.types.EntityId partyId, out InviteRequest[] requests)
		{
			this.m_partyAPI.GetPartyInviteRequests(partyId.ToProtocol(), out requests);
		}

		public void GetPartyListenerEvents(out PartyListenerEvent[] updates)
		{
			this.m_partyAPI.GetPartyListenerEvents(out updates);
		}

		public void GetPartyMembers(bgs.types.EntityId partyId, out bgs.types.PartyMember[] members)
		{
			this.m_partyAPI.GetPartyMembers(partyId.ToProtocol(), out members);
		}

		public int GetPartyPrivacy(bgs.types.EntityId partyId)
		{
			return this.m_partyAPI.GetPartyPrivacy(partyId.ToProtocol());
		}

		public void GetPartySentInvites(bgs.types.EntityId partyId, out PartyInvite[] invites)
		{
			this.m_partyAPI.GetPartySentInvites(partyId.ToProtocol(), out invites);
		}

		public void GetPartyUpdates([Out] PartyEvent[] updates)
		{
			this.m_partyAPI.GetPartyUpdates(updates);
		}

		public void GetPartyUpdatesInfo(ref bgs.types.PartyInfo info)
		{
			info.size = this.m_partyAPI.GetPartyUpdateCount();
		}

		public void GetPlayRestrictions(ref Lockouts restrictions, bool reload)
		{
			this.Account.GetPlayRestrictions(ref restrictions, reload);
		}

		public int GetPort()
		{
			return this.m_auroraPort;
		}

		public void GetPresence([Out] PresenceUpdate[] updates)
		{
			this.m_presenceAPI.GetPresence(updates);
		}

		public QueueEvent GetQueueEvent()
		{
			return this.m_gamesAPI.GetQueueEvent();
		}

		public void GetQueueInfo(ref QueueInfo queueInfo)
		{
			this.m_authenticationAPI.GetQueueInfo(ref queueInfo);
		}

		public double GetRealTimeSinceStartup()
		{
			return this.m_stopwatch.Elapsed.TotalSeconds;
		}

		public void GetReceivedPartyInvites(out PartyInvite[] invites)
		{
			this.m_partyAPI.GetReceivedPartyInvites(out invites);
		}

		public byte[] GetSessionKey()
		{
			return this.m_authenticationAPI.SessionKey;
		}

		public int GetShutdownMinutes()
		{
			return this.m_shutdownInMinutes;
		}

		public string GetUserEmailAddress()
		{
			return this.m_userEmailAddress;
		}

		public string GetVersion()
		{
			return this.m_clientInterface.GetVersion();
		}

		public void GetWhisperInfo(ref WhisperInfo info)
		{
			this.m_whisperAPI.GetWhisperInfo(ref info);
		}

		public void GetWhispers([Out] BnetWhisper[] whispers)
		{
			this.m_whisperAPI.GetWhispers(whispers);
		}

		private void HandleEchoRequest(RPCContext context)
		{
			if (this.m_rpcConnection == null)
			{
				LogAdapter.Log(LogLevel.Error, "HandleEchoRequest with null RPC Connection");
				return;
			}
			EchoRequest echoRequest = EchoRequest.ParseFrom(context.Payload);
			EchoResponse echoResponse = new EchoResponse();
			if (echoRequest.HasTime)
			{
				echoResponse.SetTime(echoRequest.Time);
			}
			if (echoRequest.HasPayload)
			{
				echoResponse.SetPayload(echoRequest.Payload);
			}
			this.m_rpcConnection.QueueResponse(context, echoResponse);
			Console.WriteLine(string.Empty);
			Console.WriteLine("[*]send echo response");
		}

		private void HandleForceDisconnectRequest(RPCContext context)
		{
			DisconnectNotification disconnectNotification = DisconnectNotification.ParseFrom(context.Payload);
			this.m_logSource.LogDebug(string.Concat("RPC Called: ForceDisconnect : ", disconnectNotification.ErrorCode));
			this.EnqueueErrorInfo(BnetFeature.Bnet, BnetFeatureEvent.Bnet_OnDisconnected, (BattleNetErrors)disconnectNotification.ErrorCode, 0);
		}

		private void HandleNotificationReceived(RPCContext context)
		{
			BattleNetCSharp.NotificationHandler notificationHandler;
			bnet.protocol.notification.Notification notification = bnet.protocol.notification.Notification.ParseFrom(context.Payload);
			this.m_logSource.LogDebug(string.Concat("Notification: ", notification));
			if (!this.m_notificationHandlers.TryGetValue(notification.Type, out notificationHandler))
			{
				this.m_logSource.LogWarning(string.Concat("unhandled battle net notification: ", notification.Type));
			}
			else
			{
				notificationHandler(notification);
			}
		}

		public void IgnoreInviteRequest(bgs.types.EntityId partyId, bgs.types.EntityId requestedTargetId)
		{
			this.m_partyAPI.IgnoreInviteRequest(partyId.ToProtocol(), requestedTargetId.ToProtocol());
		}

		public bool Init(bool internalMode, string userEmailAddress, string targetServer, int port, SslParameters sslParams, ClientInterface ci)
		{
			string str;
			if (this.m_initialized)
			{
				return true;
			}
			if (ci == null)
			{
				return false;
			}
			this.m_stopwatch.Reset();
			this.m_stopwatch.Start();
			this.m_clientInterface = ci;
			this.m_auroraEnvironment = targetServer;
			this.m_auroraPort = (port > 0 ? port : 1119);
			this.m_userEmailAddress = userEmailAddress;
			bool hostAddress = false;
			try
			{
				hostAddress = UriUtils.GetHostAddress(targetServer, out str);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.m_logSource.LogError(string.Concat("Exception within GetHostAddress: ", exception.Message));
			}
			if (!hostAddress)
			{
				LogAdapter.Log(LogLevel.Fatal, "GLOBAL_ERROR_NETWORK_NO_CONNECTION");
			}
			else
			{
				this.m_initialized = true;
				this.ConnectAurora(targetServer, this.m_auroraPort, sslParams);
			}
			return this.m_initialized;
		}

		private void InitRPCListeners()
		{
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_notificationListenerService.Id, 1, new RPCContextDelegate(this.HandleNotificationReceived));
			foreach (BattleNetAPI mApiList in this.m_apiList)
			{
				mApiList.InitRPCListeners(this.m_rpcConnection);
			}
		}

		private bool InState(BattleNetCSharp.ConnectionState state)
		{
			return this.m_connectionState == state;
		}

		public bool IsInitialized()
		{
			return this.m_initialized;
		}

		public void IssueSelectGameAccountRequest()
		{
			this.SwitchToState(BattleNetCSharp.ConnectionState.WaitForAPIToInitialize);
			foreach (BattleNetAPI mApiList in this.m_apiList)
			{
				mApiList.Initialize();
				mApiList.OnGameAccountSelected();
			}
		}

		public bool IsVersionInt()
		{
			return this.m_clientInterface.IsVersionInt();
		}

		public void JoinParty(bgs.types.EntityId partyId, string szPartyType)
		{
			this.m_partyAPI.JoinParty(partyId.ToProtocol(), szPartyType);
		}

		public void KickPartyMember(bgs.types.EntityId partyId, bgs.types.EntityId memberId)
		{
			this.m_partyAPI.KickPartyMember(partyId.ToProtocol(), memberId.ToProtocol());
		}

		public void LeaveParty(bgs.types.EntityId partyId)
		{
			this.m_partyAPI.LeaveParty(partyId.ToProtocol());
		}

		public void ManageFriendInvite(int action, ulong inviteId)
		{
			this.m_friendAPI.ManageFriendInvite(action, inviteId);
		}

		public GamesAPI.GetAllValuesForAttributeResult NextGetAllValuesForAttributeResult()
		{
			return this.m_gamesAPI.NextGetAllValuesForAttributeResult();
		}

		public GamesAPI.UtilResponse NextUtilPacket()
		{
			return this.m_gamesAPI.NextUtilPacket();
		}

		public int NumChallenges()
		{
			return this.m_challengeAPI.NumChallenges();
		}

		public void OnBroadcastReceived(IList<bnet.protocol.attribute.Attribute> AttributeList)
		{
			foreach (bnet.protocol.attribute.Attribute attributeList in AttributeList)
			{
				if (attributeList.Name != "shutdown")
				{
					continue;
				}
				this.m_shutdownInMinutes = (int)attributeList.Value.IntValue;
			}
		}

		private void OnConnectCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			this.m_logSource.LogDebug(string.Concat("RPC Connected, error : ", status.ToString()));
			if (status != BattleNetErrors.ERROR_OK)
			{
				this.SwitchToState(BattleNetCSharp.ConnectionState.Error);
				this.EnqueueErrorInfo(BnetFeature.Bnet, BnetFeatureEvent.Bnet_OnConnected, status, 0);
				return;
			}
			ConnectResponse connectResponse = ConnectResponse.ParseFrom(context.Payload);
			if (connectResponse.HasServerTime)
			{
				this.m_serverTimeUTCAtConnectMicroseconds = (long)connectResponse.ServerTime;
				this.m_serverTimeDeltaUTCSeconds = this.m_serverTimeUTCAtConnectMicroseconds / (long)1000000 - this.GetCurrentTimeSecondsSinceUnixEpoch();
			}
			if (!connectResponse.HasBindResult || !connectResponse.HasBindResponse || connectResponse.BindResult != 0)
			{
				this.m_logSource.LogWarning("BindRequest failed with error={0}", new object[] { connectResponse.BindResult });
				this.SwitchToState(BattleNetCSharp.ConnectionState.Error);
				return;
			}
			int num = 0;
			foreach (uint importedServiceIdList in connectResponse.BindResponse.ImportedServiceIdList)
			{
				int num1 = num;
				num = num1 + 1;
				ServiceDescriptor item = this.m_importedServices[num1];
				item.Id = importedServiceIdList;
				this.m_rpcConnection.serviceHelper.AddImportedService(importedServiceIdList, item);
				this.m_logSource.LogDebug("Importing service id={0} name={1}", new object[] { item.Id, item.Name });
			}
			if (!connectResponse.HasContentHandleArray)
			{
				this.m_logSource.LogDebug("Connection response had not connection metering request");
			}
			else if (this.m_clientInterface.GetDisableConnectionMetering())
			{
				this.m_logSource.LogWarning("Connection metering disabled by configuration.");
			}
			else
			{
				this.m_rpcConnection.SetConnectionMeteringContentHandles(connectResponse.ContentHandleArray, this.m_localStorageAPI);
			}
			this.m_logSource.LogDebug("FRONT ServerId={0:x2}", new object[] { connectResponse.ServerId.Label });
			this.InitRPCListeners();
			this.PrintBindServiceResponse(connectResponse.BindResponse);
			this.SwitchToState(BattleNetCSharp.ConnectionState.Logon);
		}

		public void OnConnectHandlerCallback(BattleNetErrors error)
		{
			if (error != BattleNetErrors.ERROR_OK)
			{
				this.SwitchToState(BattleNetCSharp.ConnectionState.Error);
				this.EnqueueErrorInfo(BnetFeature.Bnet, BnetFeatureEvent.Bnet_OnConnected, error, 0);
			}
			foreach (BattleNetAPI mApiList in this.m_apiList)
			{
				mApiList.OnConnected(error);
			}
		}

		private void OnDisconectHandlerCallback(BattleNetErrors error)
		{
			BattleNet.Log.LogInfo("Disconnected: code={0} {1}", new object[] { (int)error, error.ToString() });
			if (error != BattleNetErrors.ERROR_OK)
			{
				this.EnqueueErrorInfo(BnetFeature.Bnet, BnetFeatureEvent.Bnet_OnDisconnected, error, 0);
			}
			this.m_bnetEvents.Add(BattleNet.BnetEvent.Disconnected);
			foreach (BattleNetAPI mApiList in this.m_apiList)
			{
				mApiList.OnDisconnected();
			}
		}

		private void OnSelectGameAccountCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				this.SwitchToState(BattleNetCSharp.ConnectionState.Error);
				this.EnqueueErrorInfo(BnetFeature.Auth, BnetFeatureEvent.Auth_GameAccountSelected, status, 0);
				this.m_logSource.LogError("Failed to select a game account status = {0}", new object[] { status.ToString() });
			}
			else
			{
				this.SwitchToState(BattleNetCSharp.ConnectionState.WaitForAPIToInitialize);
				foreach (BattleNetAPI mApiList in this.m_apiList)
				{
					mApiList.Initialize();
					mApiList.OnGameAccountSelected();
				}
			}
		}

		public int PresenceSize()
		{
			return this.m_presenceAPI.PresenceSize();
		}

		private void PrintBindServiceResponse(BindResponse response)
		{
			string str = "BindResponse: { ";
			int importedServiceIdCount = response.ImportedServiceIdCount;
			str = string.Concat(str, "Num Imported Services: ", importedServiceIdCount);
			str = string.Concat(str, " [");
			for (int i = 0; i < importedServiceIdCount; i++)
			{
				str = string.Concat(str, " Id:", response.ImportedServiceId[i]);
			}
			str = string.Concat(str, " ]");
			str = string.Concat(str, " }");
			this.m_logSource.LogDebug(str);
		}

		public void ProcessAurora()
		{
			BattleNetCSharp.AuroraStateHandler auroraStateHandler;
			if (this.InState(BattleNetCSharp.ConnectionState.Disconnected) || this.InState(BattleNetCSharp.ConnectionState.Error))
			{
				return;
			}
			if (this.m_rpcConnection != null)
			{
				this.m_rpcConnection.Update();
				if (this.m_rpcConnection.MillisecondsSinceLastPacketSent > this.m_keepAliveIntervalMilliseconds)
				{
					this.m_rpcConnection.QueueRequest(this.m_connectionService.Id, 5, new NoData(), null, 0);
				}
			}
			if (!this.m_stateHandlers.TryGetValue(this.m_connectionState, out auroraStateHandler))
			{
				this.m_logSource.LogError("Missing state handler");
			}
			else
			{
				auroraStateHandler();
			}
			for (int i = 0; i < this.m_apiList.Count; i++)
			{
				this.m_apiList[i].Process();
			}
		}

		public void ProvideWebAuthToken(string token)
		{
			this.m_logSource.LogDebug("ProvideWebAuthToken token={0}", new object[] { token });
			if (this.m_authenticationAPI != null && this.InState(BattleNetCSharp.ConnectionState.WaitForLogon))
			{
				this.m_authenticationAPI.VerifyWebCredentials(token);
			}
		}

		public void QueryAurora()
		{
		}

		public void RemoveFriend(BnetAccountId account)
		{
			this.m_friendAPI.RemoveFriend(account);
		}

		public void RequestCloseAurora()
		{
			if (this.m_rpcConnection != null)
			{
				this.m_rpcConnection.Disconnect();
			}
			this.SwitchToState(BattleNetCSharp.ConnectionState.Disconnected);
			this.m_initialized = false;
		}

		public void RequestPartyInvite(bgs.types.EntityId partyId, bgs.types.EntityId whomToAskForApproval, bgs.types.EntityId whomToInvite, string szPartyType)
		{
			this.m_partyAPI.RequestPartyInvite(partyId.ToProtocol(), whomToAskForApproval.ToProtocol(), whomToInvite.ToProtocol(), szPartyType);
		}

		public void RequestPresenceFields(bool isGameAccountEntityId, [In] bgs.types.EntityId entityId, [In] PresenceFieldKey[] fieldList)
		{
			this.m_presenceAPI.RequestPresenceFields(isGameAccountEntityId, entityId, fieldList);
		}

		public void RescindFriendlyChallenge(ref bgs.types.EntityId partyId)
		{
			this.m_partyAPI.DeclineFriendlyChallenge(partyId.ToProtocol(), "kill");
		}

		public void RevokePartyInvite(bgs.types.EntityId partyId, ulong invitationId)
		{
			this.m_partyAPI.RevokePartyInvite(partyId.ToProtocol(), invitationId);
		}

		public void SendClientRequest(ClientRequest request, RPCContextDelegate callback = null)
		{
			this.m_gamesAPI.SendClientRequest(request, callback);
		}

		public void SendFriendInvite(string sender, string target, bool byEmail)
		{
			this.m_friendAPI.SendFriendInvite(sender, target, byEmail);
		}

		public void SendFriendlyChallengeInvite(ref bgs.types.EntityId gameAccount, int scenarioId)
		{
			this.m_partyAPI.SendFriendlyChallengeInvite(gameAccount.ToProtocol(), scenarioId);
		}

		public void SendPartyChatMessage(bgs.types.EntityId partyId, string message)
		{
			this.m_partyAPI.SendPartyChatMessage(partyId.ToProtocol(), message);
		}

		public void SendPartyInvite(bgs.types.EntityId partyId, bgs.types.EntityId inviteeId, bool isReservation)
		{
			this.m_partyAPI.SendPartyInvite(partyId.ToProtocol(), inviteeId.ToProtocol(), isReservation);
		}

		public void SendUtilPacket(int packetId, int systemId, byte[] bytes, int size, int subID, int context, ulong route)
		{
			this.m_gamesAPI.SendUtilPacket(packetId, systemId, bytes, size, subID, context, route);
		}

		public void SendWhisper(BnetGameAccountId gameAccount, string message)
		{
			this.m_whisperAPI.SendWhisper(gameAccount, message);
		}

		public void SetConnectedRegion(uint region)
		{
			this.m_connectedRegion = region;
		}

		public void SetMyFriendlyChallengeDeck(ref bgs.types.EntityId partyId, long deckID)
		{
			this.m_partyAPI.SetPartyDeck(partyId.ToProtocol(), deckID);
		}

		public void SetPartyAttributeBlob(bgs.types.EntityId partyId, string attributeKey, [In] byte[] value)
		{
			this.m_partyAPI.SetPartyAttributeBlob(partyId.ToProtocol(), attributeKey, value);
		}

		public void SetPartyAttributeLong(bgs.types.EntityId partyId, string attributeKey, [In] long value)
		{
			this.m_partyAPI.SetPartyAttributeLong(partyId.ToProtocol(), attributeKey, value);
		}

		public void SetPartyAttributeString(bgs.types.EntityId partyId, string attributeKey, [In] string value)
		{
			this.m_partyAPI.SetPartyAttributeString(partyId.ToProtocol(), attributeKey, value);
		}

		public void SetPartyPrivacy(bgs.types.EntityId partyId, int privacyLevel)
		{
			this.m_partyAPI.SetPartyPrivacy(partyId.ToProtocol(), privacyLevel);
		}

		public void SetPresenceBlob(uint field, byte[] val)
		{
			this.m_presenceAPI.SetPresenceBlob(field, val);
		}

		public void SetPresenceBool(uint field, bool val)
		{
			this.m_presenceAPI.SetPresenceBool(field, val);
		}

		public void SetPresenceInt(uint field, long val)
		{
			this.m_presenceAPI.SetPresenceInt(field, val);
		}

		public void SetPresenceString(uint field, string val)
		{
			this.m_presenceAPI.SetPresenceString(field, val);
		}

		public void SetRichPresence([In] RichPresenceUpdate[] updates)
		{
			this.m_presenceAPI.PublishRichPresence(updates);
		}

		private bool SwitchToState(BattleNetCSharp.ConnectionState state)
		{
			if (state == this.m_connectionState)
			{
				return false;
			}
			bool flag = true;
			if (state != BattleNetCSharp.ConnectionState.Disconnected || this.m_connectionState != BattleNetCSharp.ConnectionState.Ready)
			{
				flag = state > this.m_connectionState;
			}
			if (!flag)
			{
				this.m_logSource.LogWarning("Unexpected state changes {0} -> {1}", new object[] { this.m_connectionState.ToString(), state.ToString() });
				this.m_logSource.LogDebugStackTrace("SwitchToState", 5, 0);
			}
			else
			{
				this.m_logSource.LogDebug("Expected state change {0} -> {1}", new object[] { this.m_connectionState.ToString(), state.ToString() });
			}
			this.m_connectionState = state;
			return true;
		}

		public delegate void AuroraStateHandler();

		private class BnetErrorComparer : IComparer<BnetErrorInfo>
		{
			public BnetErrorComparer()
			{
			}

			public int Compare(BnetErrorInfo x, BnetErrorInfo y)
			{
				if (x == null || y == null)
				{
					return 0;
				}
				if (x.GetError() == BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED && y.GetError() != BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED)
				{
					return 1;
				}
				if (x.GetError() != BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED && y.GetError() == BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED)
				{
					return -1;
				}
				return 0;
			}
		}

		public enum ConnectionState
		{
			Disconnected,
			Connect,
			InitRPC,
			WaitForInitRPC,
			Logon,
			WaitForLogon,
			WaitForGameAccountSelect,
			WaitForAPIToInitialize,
			Ready,
			Error
		}

		private delegate void NotificationHandler(bnet.protocol.notification.Notification notification);

		private delegate void OnConnectHandler(BattleNetErrors error);

		private delegate void OnDisconectHandler(BattleNetErrors error);
	}
}