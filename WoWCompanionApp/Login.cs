using bgs;
using bgs.types;
using Blizzard;
using bnet.protocol;
using bnet.protocol.account;
using bnet.protocol.attribute;
using bnet.protocol.game_utilities;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using JamLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WowJamMessages;
using WowJamMessages.JSONRealmList;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class Login : Singleton<Login>
	{
		private string m_webToken;

		private Login.eLoginState m_loginState = Login.eLoginState.WAIT_FOR_ASSET_BUNDLES;

		private WebAuth m_webAuth;

		private string m_webAuthUrl;

		private string m_mobileServerAddress;

		private int m_mobileServerPort;

		private string m_bnetAccount;

		private ulong m_virtualRealmAddress;

		private string m_wowAccount;

		private byte[] m_realmListTicket;

		private byte[] m_realmJoinTicket;

		private string[] m_subRegions;

		private string m_subRegion;

		private List<Login.LoginGameAccount> m_loginGameAccounts;

		private float m_characterListStartTime;

		private const float m_characterListRefreshWaitTime = 30f;

		private const float m_characterListDisplayTimeout = 10f;

		private Login.RealmJoinInfo m_realmJoinInfo;

		private float m_mobileLoginTime;

		private float m_mobileLoginTimeout = 25f;

		public DotNetUrlDownloader m_urlDownloader;

		private int m_expectedServerProtocolVersion = 4;

		private int m_currentServerProtocolVersion = -1;

		private bool m_useCachedLogin;

		private bool m_useCachedRealm;

		private bool m_useCachedCharacter;

		private bool m_useCachedWoWAccount = true;

		public const int m_numRecentChars = 3;

		public bnet.protocol.EntityId m_gameAccount;

		private JamJSONCharacterEntry m_selectedCharacterEntry;

		private int m_pauseTimestamp;

		private const int m_unpauseReconnectTime = 30;

		private List<RecentCharacter> m_recentCharacters;

		private const int m_recentCharacterVersion = 2;

		private byte[] m_joinSecret;

		private byte[] m_clientSecret;

		private DisconnectReason m_recentDisconnectReason;

		public bool m_clearCharacterListOnReply;

		private const float m_bnLoginTimeout = 20f;

		private float m_bnLoginStartTime;

		private int m_battlenetFailures;

		private bool m_initialUnpause = true;

		private bool m_loggingIn;

		private WoWCompanionApp.LoginUI m_loginUI;

		public static string m_portal;

		public static string[] m_devPortals;

		public WoWCompanionApp.LoginUI LoginUI
		{
			get
			{
				if (this.m_loginUI == null)
				{
					this.m_loginUI = UnityEngine.Object.FindObjectOfType<WoWCompanionApp.LoginUI>();
				}
				return this.m_loginUI;
			}
		}

		public bool ReturnToRecentCharacter
		{
			get;
			private set;
		}

		static Login()
		{
			Login.m_portal = "wow-dev";
			Login.m_devPortals = new string[] { "wow-dev", "st1", "st-us", "st2", "st-eu", "st3", "st-kr", "st5", "st-cn", "st21", "st22", "st23", "st25" };
		}

		public Login()
		{
		}

		public void AddGameAccountButton(bnet.protocol.EntityId gameAccount, string name, bool isBanned, bool isSuspended)
		{
			if (this.m_loginState != Login.eLoginState.BN_ACCOUNT_NAME_WAIT)
			{
				this.LoginLog(string.Concat("AddGameAccountButton(): Ignored because in state ", this.m_loginState));
				return;
			}
			RealmListView realmListView = this.LoginUI.RealmListView;
			if (this.m_loginGameAccounts.Count == 0)
			{
				realmListView.ClearBnRealmList();
			}
			Login.LoginGameAccount loginGameAccount = new Login.LoginGameAccount()
			{
				gameAccount = gameAccount,
				name = name,
				isBanned = isBanned,
				isSuspended = isSuspended
			};
			this.m_loginGameAccounts.Add(loginGameAccount);
			List<bnet.protocol.EntityId> gameAccountList = BattleNet.GetGameAccountList();
			if (gameAccountList.Count <= 1)
			{
				if (isBanned)
				{
					this.BnLoginFailed(StaticDB.GetString("SUSPENDED", null), StaticDB.GetString("SUSPENDED_LONG", null));
					return;
				}
				if (isSuspended)
				{
					this.BnLoginFailed(StaticDB.GetString("SUSPENDED", null), StaticDB.GetString("SUSPENDED_LONG", null));
					return;
				}
				this.m_gameAccount = gameAccountList[0];
				this.SetLoginState(Login.eLoginState.BN_TICKET_WAIT);
				this.SendRealmListTicketRequest();
			}
			else
			{
				string str = SecurePlayerPrefs.GetString("GameAccountHigh", Main.uniqueIdentifier);
				string str1 = SecurePlayerPrefs.GetString("GameAccountLow", Main.uniqueIdentifier);
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				if (str != null && str != string.Empty)
				{
					num = Convert.ToUInt64(str);
				}
				if (str1 != null && str1 != string.Empty)
				{
					num1 = Convert.ToUInt64(str1);
				}
				if (!this.m_useCachedWoWAccount)
				{
					if (!this.LoginUI.IsShowingRealmListPanel())
					{
						this.LoginLog("Called ShowingRealmListPanel() because m_useCachedWoWAccount is false.");
						this.LoginUI.ShowRealmListPanel();
					}
				}
				else if (num == gameAccount.High && num1 == gameAccount.Low)
				{
					if (this.m_useCachedLogin && !isBanned && !isSuspended)
					{
						if (this.m_gameAccount == null)
						{
							this.m_gameAccount = new bnet.protocol.EntityId();
						}
						this.m_gameAccount.High = num;
						this.m_gameAccount.Low = num1;
						this.SetLoginState(Login.eLoginState.BN_TICKET_WAIT);
						this.SendRealmListTicketRequest();
					}
				}
				else if (this.m_loginGameAccounts.Count >= gameAccountList.Count && !this.LoginUI.IsShowingRealmListPanel())
				{
					this.LoginLog(string.Concat(new object[] { "Called ShowingRealmListPanel() because all accounts received. ", this.m_loginGameAccounts.Count, " > ", gameAccountList.Count }));
					this.LoginUI.ShowRealmListPanel();
				}
			}
			realmListView.AddGameAccountButton(gameAccount, name, isBanned, isSuspended);
		}

		public void BackToAccountSelect()
		{
			List<bnet.protocol.EntityId> gameAccountList = BattleNet.GetGameAccountList();
			if (gameAccountList == null)
			{
				this.MobileDisconnect(DisconnectReason.Generic);
				return;
			}
			if (gameAccountList.Count <= 1 || this.m_loginState != Login.eLoginState.BN_CHARACTER_LIST_WAIT)
			{
				BattleNet.Get().RequestCloseAurora();
				BattleNet.ProcessAurora();
				this.BnErrorsUpdate();
				this.LoginUI.ShowTitlePanel();
				this.SetLoginState(Login.eLoginState.IDLE);
				this.m_useCachedWoWAccount = true;
			}
			else
			{
				this.m_useCachedWoWAccount = false;
				this.BnLoginStart(true, false, false, false);
			}
		}

		public void BackToTitle()
		{
			BattleNet.Get().RequestCloseAurora();
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			this.SetLoginState(Login.eLoginState.IDLE);
			this.LoginUI.ShowTitlePanel();
		}

		private void BnAccountNameWait()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			this.LoginUI.RealmListView.SetGameAccountTitle();
		}

		private void BnCharacterListWait()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			if (this.LoginUI.IsShowingCharacterListPanel())
			{
				if (Time.timeSinceLevelLoad > this.m_characterListStartTime + 30f)
				{
					this.m_characterListStartTime = Time.timeSinceLevelLoad;
					this.m_clearCharacterListOnReply = true;
					string[] mSubRegions = this.m_subRegions;
					for (int i = 0; i < (int)mSubRegions.Length; i++)
					{
						this.SendCharacterListRequest(mSubRegions[i]);
					}
					this.LoginLog("Refreshing character list.");
				}
			}
			else if (!Singleton<Login>.instance.UseCachedCharacter() || Time.timeSinceLevelLoad > this.m_characterListStartTime + 10f)
			{
				this.LoginLog("Displayed CharacterList panel after timeout.");
				this.LoginUI.ShowCharacterListPanel();
			}
		}

		private void BnErrorsUpdate()
		{
			int errorsCount = BattleNet.GetErrorsCount();
			if (errorsCount > 0)
			{
				BnetErrorInfo[] bnetErrorInfoArray = new BnetErrorInfo[errorsCount];
				BattleNet.GetErrors(bnetErrorInfoArray);
				BattleNet.ClearErrors();
				BnetErrorInfo[] bnetErrorInfoArray1 = bnetErrorInfoArray;
				for (int i = 0; i < (int)bnetErrorInfoArray1.Length; i++)
				{
					BnetErrorInfo bnetErrorInfo = bnetErrorInfoArray1[i];
					this.LoginLog(string.Concat("Battle.net error: Name: ", bnetErrorInfo.GetName()));
					this.LoginLog(string.Concat("Battle.net error: Feature: ", bnetErrorInfo.GetFeature()));
					this.LoginLog(string.Concat("Battle.net error: FeatureEvent: ", bnetErrorInfo.GetFeatureEvent()));
					this.LoginLog(string.Concat("Battle.net error: Context: ", bnetErrorInfo.GetContext()));
					if (bnetErrorInfo.GetError() != BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED)
					{
						this.BnLoginFailed(null, null);
						return;
					}
					if (this.m_loginState == Login.eLoginState.BN_CHARACTER_LIST_WAIT)
					{
						this.BnLoginStart(true, true, false, false);
					}
				}
			}
			if (Log.BattleNet.GetLogEvents().Count > 0)
			{
				Log.BattleNet.ClearLogEvents();
			}
		}

		private void BnLoginFailed(string popupTitle = null, string popupDescription = null)
		{
			this.LoginUI.HideAllPopups();
			BattleNet.Get().RequestCloseAurora();
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			this.LoginUI.ShowTitlePanel();
			if (popupTitle != null && popupDescription != null)
			{
				this.LoginUI.ShowGenericPopup(popupTitle, popupDescription);
			}
			else if (popupTitle != null)
			{
				this.LoginUI.ShowGenericPopupFull(popupTitle);
			}
			else if (this.m_battlenetFailures != 0)
			{
				this.LoginUI.ShowGenericPopup(StaticDB.GetString("NETWORK_ERROR", null), StaticDB.GetString("CANT_CONNECT", null));
			}
			else
			{
				GenericPopup.DisabledAction += new Action(this.ReconnectPopupDisabledAction);
				if (Main.instance.GetLocale() != "enUS")
				{
					this.LoginUI.ShowGenericPopup(StaticDB.GetString("NETWORK_ERROR", null), StaticDB.GetString("CANT_CONNECT", null));
				}
				else
				{
					this.LoginUI.ShowGenericPopup("Battle.net Error", "Unable to contact Battle.net, tap anywhere to retry.");
				}
			}
			this.m_battlenetFailures++;
			this.SetLoginState(Login.eLoginState.IDLE);
			this.LoginLog(string.Concat("=================== BN Login Failed. ", this.m_battlenetFailures, " ==================="));
		}

		private void BnLoginProvideToken()
		{
			BattleNet.ProcessAurora();
			BattleNet.ProvideWebAuthToken(this.m_webToken);
			this.LoginLog(string.Concat("BnLoginProvideToken(): Provided web auth token ", this.m_webToken));
			this.SetLoginState(Login.eLoginState.BN_LOGGING_IN);
		}

		private void BnLoginStart(bool cachedLogin, bool cachedRealm, bool cachedCharacter, bool returnToRecentCharacter = false)
		{
			BattleNet.RequestCloseAurora();
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			this.m_useCachedLogin = cachedLogin;
			this.m_useCachedRealm = cachedRealm;
			this.m_useCachedCharacter = cachedCharacter;
			this.ReturnToRecentCharacter = returnToRecentCharacter;
			this.m_bnLoginStartTime = Time.timeSinceLevelLoad;
			if (this.LoginUI != null)
			{
				this.LoginUI.ShowConnectingPanel();
			}
			string bnServerString = this.GetBnServerString();
			int num = 1119;
			ClientInterface myClientInterface = new MyClientInterface();
			SslParameters sslParameter = new SslParameters();
			this.LoginLog(string.Concat(new string[] { "BnLoginStart(", this.m_useCachedLogin.ToString(), ",", this.m_useCachedRealm.ToString(), ",", this.m_useCachedCharacter.ToString(), "): server = ", bnServerString }));
			if (!BattleNet.Init(true, string.Empty, bnServerString, num, sslParameter, myClientInterface))
			{
				this.LoginLog("BattleNet.Init() failed.");
				this.BnLoginFailed(null, null);
			}
			else
			{
				BattleNet.ProcessAurora();
				this.SetLoginState(Login.eLoginState.BN_LOGIN_WAIT_FOR_LOGON);
			}
		}

		private void BnLoginSucceeded()
		{
			this.m_battlenetFailures = 0;
			SecurePlayerPrefs.SetString("WebToken", this.m_webToken, Main.uniqueIdentifier);
			PlayerPrefs.Save();
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			if (this.m_loginGameAccounts == null)
			{
				this.m_loginGameAccounts = new List<Login.LoginGameAccount>();
			}
			this.m_loginGameAccounts.Clear();
			this.RequestGameAccountNames();
		}

		private void BnLoginUpdate()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			if (BattleNet.CheckWebAuth(out this.m_webAuthUrl))
			{
				this.LoginLog("CheckWebAuth was true in BnLoginUpdate, starting WebAuth.");
				this.SetLoginState(Login.eLoginState.WEB_AUTH_START);
				return;
			}
			switch (BattleNet.BattleNetStatus())
			{
				case 0:
				{
					if (this.m_loginState != Login.eLoginState.BN_LOGIN_UNKNOWN)
					{
						this.SetLoginState(Login.eLoginState.BN_LOGIN_UNKNOWN);
					}
					break;
				}
				case 1:
				{
					if (this.m_loginState != Login.eLoginState.BN_LOGGING_IN)
					{
						this.SetLoginState(Login.eLoginState.BN_LOGGING_IN);
					}
					break;
				}
				case 2:
				{
					this.LoginLog("BnLoginUpdate(): timed out.");
					this.BnLoginFailed(null, null);
					break;
				}
				case 3:
				{
					this.LoginLog("BnLoginUpdate(): login failed.");
					this.BnLoginFailed(null, null);
					break;
				}
				case 4:
				{
					this.LoginLog("-------------------BnLoginUpdate(): Success! Logged in.--------------------------");
					this.BnLoginSucceeded();
					break;
				}
			}
		}

		private void BnLoginWaitForLogon()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			if (BattleNet.CheckWebAuth(out this.m_webAuthUrl))
			{
				this.LoginLog(string.Concat("Received WebAuth challenge URL: ", this.m_webAuthUrl));
				this.m_webToken = SecurePlayerPrefs.GetString("WebToken", Main.uniqueIdentifier);
				if (!this.m_useCachedLogin || this.m_webToken == null || !(this.m_webToken != string.Empty))
				{
					this.SetLoginState(Login.eLoginState.WEB_AUTH_START);
				}
				else
				{
					this.SetLoginState(Login.eLoginState.BN_LOGIN_PROVIDE_TOKEN);
				}
			}
			if (Time.timeSinceLevelLoad > this.m_bnLoginStartTime + 20f)
			{
				this.LoginLog("BnLoginWaitForLogon(): timed out.");
				this.BnLoginFailed(null, null);
			}
		}

		public void BnQuit()
		{
			BattleNet.AppQuit();
		}

		private void BnRealmJoinWait()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
		}

		private void BnSubRegionListWait()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			GamesAPI.GetAllValuesForAttributeResult getAllValuesForAttributeResult = BattleNet.NextGetAllValuesForAttributeResult();
			if (getAllValuesForAttributeResult != null)
			{
				this.LoginUI.ClearCharacterList();
				this.m_selectedCharacterEntry = null;
				int num = 0;
				this.m_subRegions = new string[getAllValuesForAttributeResult.m_response.AttributeValue.Count];
				foreach (bnet.protocol.attribute.Variant attributeValue in getAllValuesForAttributeResult.m_response.AttributeValue)
				{
					int num1 = num;
					num = num1 + 1;
					this.m_subRegions[num1] = attributeValue.StringValue;
				}
				string[] mSubRegions = this.m_subRegions;
				for (int i = 0; i < (int)mSubRegions.Length; i++)
				{
					string str = mSubRegions[i];
					this.LoginLog(string.Concat("Received sub region ", str));
					this.SendCharacterListRequest(str);
				}
				this.SetLoginState(Login.eLoginState.BN_CHARACTER_LIST_WAIT);
				this.LoginUI.ShowConnectingPanel();
				this.LoginUI.RealmListView.ClearBnRealmList();
				this.m_characterListStartTime = Time.timeSinceLevelLoad;
			}
		}

		private void BnTicketWait()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			bool flag = false;
			GamesAPI.UtilResponse utilResponse = BattleNet.NextUtilPacket();
			if (utilResponse != null)
			{
				this.LoginLog(string.Concat("BnTicketWait(): Received ClientResponse: <", utilResponse.GetType().ToString(), ">"));
				foreach (bnet.protocol.attribute.Attribute attributeList in utilResponse.m_response.AttributeList)
				{
					this.LoginLog(string.Concat("Attrib: <", attributeList.Name, ">"));
					if (attributeList.Name.StartsWith("Param_MobileErrorVersion"))
					{
						flag = false;
						this.BnLoginFailed(StaticDB.GetString("UPDATE_REQUIRED", null), StaticDB.GetString("UPDATE_REQUIRED_DESCRIPTION", null));
						break;
					}
					else if (attributeList.Name.StartsWith("Param_MobileError"))
					{
						flag = false;
						this.BnLoginFailed(StaticDB.GetString("LOGIN_UNAVAILABLE", null), null);
						break;
					}
					else if (!attributeList.Name.StartsWith("Param_CustomErrorMessage"))
					{
						if (!attributeList.Name.StartsWith("Param_RealmListTicket"))
						{
							continue;
						}
						this.m_realmListTicket = attributeList.Value.BlobValue;
						flag = true;
					}
					else if (!attributeList.Value.HasStringValue)
					{
						flag = false;
						this.BnLoginFailed(StaticDB.GetString("LOGIN_UNAVAILABLE", null), null);
						break;
					}
					else
					{
						flag = false;
						this.BnLoginFailed(attributeList.Value.StringValue, null);
						break;
					}
				}
			}
			if (flag)
			{
				this.SetLoginState(Login.eLoginState.BN_SUBREGION_LIST_WAIT);
				this.SendSubRegionListRequest();
			}
		}

		public void CancelLogin()
		{
			switch (this.m_loginState)
			{
				case Login.eLoginState.WEB_AUTH_START:
				case Login.eLoginState.WEB_AUTH_LOADING:
				case Login.eLoginState.WEB_AUTH_IN_PROGRESS:
				case Login.eLoginState.WEB_AUTH_FAILED:
				case Login.eLoginState.BN_LOGIN_START:
				case Login.eLoginState.BN_LOGIN_WAIT_FOR_LOGON:
				case Login.eLoginState.BN_LOGIN_PROVIDE_TOKEN:
				case Login.eLoginState.BN_LOGGING_IN:
				case Login.eLoginState.BN_TICKET_WAIT:
				case Login.eLoginState.BN_SUBREGION_LIST_WAIT:
				case Login.eLoginState.BN_CHARACTER_LIST_WAIT:
				case Login.eLoginState.BN_REALM_JOIN_WAIT:
				case Login.eLoginState.BN_LOGIN_UNKNOWN:
				{
					BattleNet.Get().RequestCloseAurora();
					BattleNet.ProcessAurora();
					this.BnErrorsUpdate();
					this.SetLoginState(Login.eLoginState.IDLE);
					this.LoginUI.ShowTitlePanel();
					return;
				}
				case Login.eLoginState.BN_ACCOUNT_NAME_WAIT:
				{
					return;
				}
				case Login.eLoginState.MOBILE_CONNECT:
				case Login.eLoginState.MOBILE_CONNECTING:
				case Login.eLoginState.MOBILE_CONNECT_FAILED:
				case Login.eLoginState.MOBILE_DISCONNECTING:
				case Login.eLoginState.MOBILE_DISCONNECTED:
				case Login.eLoginState.MOBILE_DISCONNECTED_BY_SERVER:
				case Login.eLoginState.MOBILE_DISCONNECTED_IDLE:
				case Login.eLoginState.MOBILE_LOGGING_IN:
				case Login.eLoginState.MOBILE_LOGGED_IN:
				case Login.eLoginState.MOBILE_LOGGED_IN_IDLE:
				case Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE:
				{
					Singleton<Login>.instance.ReconnectToMobileServerCharacterSelect();
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public void CancelRegionIndex()
		{
			this.LoginUI.CancelRegionIndex();
			this.LoginUI.HideAllPopups();
		}

		public void CancelWebAuth()
		{
			this.LoginUI.HideWebAuthPanel();
			if (this.m_webAuth != null)
			{
				this.m_webAuth.Close();
				this.m_webAuth = null;
			}
		}

		private void CheckRecentCharacters(JamJSONCharacterEntry[] characters, string subRegion)
		{
			List<RecentCharacter> recentCharacters = new List<RecentCharacter>();
			foreach (RecentCharacter mRecentCharacter in this.m_recentCharacters)
			{
				if (mRecentCharacter.SubRegion != subRegion)
				{
					continue;
				}
				JamJSONCharacterEntry jamJSONCharacterEntry = null;
				JamJSONCharacterEntry[] jamJSONCharacterEntryArray = characters;
				int num = 0;
				while (num < (int)jamJSONCharacterEntryArray.Length)
				{
					JamJSONCharacterEntry jamJSONCharacterEntry1 = jamJSONCharacterEntryArray[num];
					if (jamJSONCharacterEntry1.PlayerGuid != mRecentCharacter.Entry.PlayerGuid)
					{
						num++;
					}
					else
					{
						jamJSONCharacterEntry = jamJSONCharacterEntry1;
						break;
					}
				}
				if (jamJSONCharacterEntry == null)
				{
					this.LoginLog(string.Concat("Removing recent character ", mRecentCharacter.Entry.Name, ", not found in character list."));
					recentCharacters.Add(mRecentCharacter);
				}
				else if (jamJSONCharacterEntry.Name == mRecentCharacter.Entry.Name)
				{
					if (jamJSONCharacterEntry.VirtualRealmAddress == mRecentCharacter.Entry.VirtualRealmAddress)
					{
						continue;
					}
					this.LoginLog(string.Concat(new object[] { "Removing recent character ", mRecentCharacter.Entry.Name, ", realm address changed from ", mRecentCharacter.Entry.VirtualRealmAddress, " to ", jamJSONCharacterEntry.VirtualRealmAddress }));
					recentCharacters.Add(mRecentCharacter);
				}
				else
				{
					this.LoginLog(string.Concat("Removing recent character ", mRecentCharacter.Entry.Name, ", name doesn't match ", jamJSONCharacterEntry.Name));
					recentCharacters.Add(mRecentCharacter);
				}
			}
			foreach (RecentCharacter recentCharacter in recentCharacters)
			{
				this.m_recentCharacters.Remove(recentCharacter);
			}
			this.SaveRecentCharacters();
		}

		public void ClearAllCachedTokens()
		{
			SecurePlayerPrefs.DeleteKey("WebToken");
			SecurePlayerPrefs.DeleteKey("CharacterID");
			SecurePlayerPrefs.DeleteKey("CharacterName");
			SecurePlayerPrefs.DeleteKey("GameAccountHigh");
			SecurePlayerPrefs.DeleteKey("GameAccountLow");
			this.LoginLog("Clearing cached BN token, game account, and character");
		}

		private void ClearCachedTokens()
		{
			SecurePlayerPrefs.DeleteKey("WebToken");
			SecurePlayerPrefs.DeleteKey("CharacterID");
			this.LoginLog("Clearing cached BN token, realm, and character");
		}

		public void ClearRealmAndCharacterTokens()
		{
			SecurePlayerPrefs.DeleteKey("CharacterID");
			this.LoginLog("Clearing cached realm and character");
		}

		public void ConnectToMobileServer(string mobileServerAddress, int mobileServerPort, string bnetAccount, ulong virtualRealmAddress, string wowAccount, byte[] realmJoinTicket, bool showConnectingPanel = true)
		{
			this.m_mobileServerAddress = mobileServerAddress;
			this.m_mobileServerPort = mobileServerPort;
			this.m_bnetAccount = bnetAccount;
			this.m_virtualRealmAddress = virtualRealmAddress;
			this.m_wowAccount = wowAccount;
			this.m_realmJoinTicket = realmJoinTicket;
			if (showConnectingPanel)
			{
				this.LoginUI.ShowConnectingPanel();
			}
			this.SetLoginState(Login.eLoginState.MOBILE_CONNECT);
			BattleNet.Get().RequestCloseAurora();
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
		}

		public static string DecompressJsonAttribBlob(byte[] data)
		{
			string str;
			MemoryStream memoryStream = new MemoryStream(data);
			byte[] numArray = new byte[4];
			memoryStream.Read(numArray, 0, 4);
			int num = BitConverter.ToInt32(numArray, 0);
			InflaterInputStream inflaterInputStream = new InflaterInputStream(memoryStream, new Inflater(false));
			byte[] numArray1 = new byte[num];
			int num1 = 0;
			int length = (int)numArray1.Length;
			try
			{
				while (true)
				{
					int num2 = inflaterInputStream.Read(numArray1, num1, length);
					if (num2 <= 0)
					{
						break;
					}
					num1 += num2;
					length -= num2;
				}
				if (num1 != num)
				{
					return null;
				}
				string str1 = Encoding.UTF8.GetString(numArray1);
				string str2 = str1.Substring(str1.IndexOf(':') + 1);
				str2 = str2.Substring(0, str2.Length - 1);
				return str2;
			}
			catch (Exception exception)
			{
				str = null;
			}
			return str;
		}

		public void DidReceiveDeeplinkURLDelegateHandler(string url)
		{
			this.LoginLog(string.Concat("DidReceiveDeeplinkURLDelegateHandler: url ", url));
		}

		public void DidReceiveRegistrationTokenHandler(string deviceToken)
		{
			this.LoginLog(string.Concat("DidReceiveRegistrationTokenHandler: device token ", deviceToken));
		}

		public string GetBnPortal()
		{
			string str = SecurePlayerPrefs.GetString("Portal", Main.uniqueIdentifier);
			if (str != null && str != string.Empty)
			{
				Login.m_portal = str;
			}
			this.LoginLog(string.Concat("Portal is ", Login.m_portal));
			return Login.m_portal;
		}

		public string GetBnServerString()
		{
			string str;
			string str1 = SecurePlayerPrefs.GetString("Portal", Main.uniqueIdentifier);
			if (str1 != null && str1 != string.Empty)
			{
				Login.m_portal = str1;
			}
			str = (!this.IsDevPortal(Login.m_portal) ? string.Concat(Login.m_portal, ".actual.battle.net") : string.Concat(Login.m_portal, ".bgs.battle.net"));
			this.LoginLog(string.Concat("BnServerString is ", str));
			return str;
		}

		public Login.eLoginState GetLoginState()
		{
			return this.m_loginState;
		}

		public bool HaveCachedWebToken()
		{
			string str = SecurePlayerPrefs.GetString("WebToken", Main.uniqueIdentifier);
			return (str == null ? false : str != string.Empty);
		}

		public void InitiateMobileDisconnect()
		{
			this.LoginLog("InitiateMobileDisconnect called");
			this.MobileDisconnect(DisconnectReason.Generic);
		}

		private void InitRecentCharacters()
		{
			this.m_recentCharacters = new List<RecentCharacter>();
		}

		public void InvalidateRecentTokens()
		{
			for (int i = 0; i < this.m_recentCharacters.Count && i < 3; i++)
			{
				StringBuilder stringBuilder = new StringBuilder(this.m_recentCharacters[i].WebToken);
				if (stringBuilder.Length > 10)
				{
					stringBuilder[6] = '0';
					stringBuilder[7] = '0';
					stringBuilder[8] = '0';
					stringBuilder[9] = '0';
					this.m_recentCharacters[i].WebToken = stringBuilder.ToString();
				}
			}
			this.SaveRecentCharacters();
			StringBuilder stringBuilder1 = new StringBuilder(SecurePlayerPrefs.GetString("WebToken", Main.uniqueIdentifier));
			if (stringBuilder1.Length > 10)
			{
				stringBuilder1[6] = '0';
				stringBuilder1[7] = '0';
				stringBuilder1[8] = '0';
				stringBuilder1[9] = '0';
				SecurePlayerPrefs.SetString("WebToken", stringBuilder1.ToString(), Main.uniqueIdentifier);
			}
		}

		public bool IsDevPortal(string portal)
		{
			string[] mDevPortals = Login.m_devPortals;
			for (int i = 0; i < (int)mDevPortals.Length; i++)
			{
				if (portal == mDevPortals[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool IsDevRegionList()
		{
			return false;
		}

		private bool IsUpdateAvailable()
		{
			return Singleton<AssetBundleManager>.instance.LatestVersion > new Version(Application.version);
		}

		public bool IsWebAuthState()
		{
			switch (this.m_loginState)
			{
				case Login.eLoginState.WEB_AUTH_START:
				case Login.eLoginState.WEB_AUTH_LOADING:
				case Login.eLoginState.WEB_AUTH_IN_PROGRESS:
				case Login.eLoginState.WEB_AUTH_FAILED:
				{
					return true;
				}
			}
			return false;
		}

		private void LoadRecentCharacters()
		{
			for (int i = 0; i < 3; i++)
			{
				string str = SecurePlayerPrefs.GetString(string.Concat("RecentCharacter", i), Main.uniqueIdentifier);
				if (str != null && str != string.Empty)
				{
					object obj = MessageFactory.Deserialize(str);
					if (obj is RecentCharacter)
					{
						RecentCharacter recentCharacter = (RecentCharacter)obj;
						if (recentCharacter.Version != 2)
						{
							PlayerPrefs.DeleteKey(string.Concat("RecentCharacter", i));
						}
						else
						{
							this.m_recentCharacters.Add(recentCharacter);
						}
					}
				}
			}
		}

		private void LoginFailedConsumptionTimeNotAllowedHandler(Network.LoginFailedConsumptionTimeNotAllowedEvent eventArgs)
		{
			this.LoginLog("LoginFailedConsumptionTimeNotAllowedHandler called");
			this.MobileDisconnect(DisconnectReason.ConsumptionTimeNotAllowed);
		}

		private void LoginFailedDuplicateCharacterHandler(Network.LoginFailedDuplicateCharacterEvent eventArgs)
		{
			this.LoginLog("LoginFailedDuplicateCharacterHandler called");
			this.MobileDisconnect(DisconnectReason.CharacterInWorld);
		}

		private void LoginFailedTrialNotAllowedHandler(Network.LoginFailedTrialNotAllowedEvent eventArgs)
		{
			this.LoginLog("LoginFailedTrialNotAllowedHandler called");
			this.MobileDisconnect(DisconnectReason.TrialNotAllowed);
		}

		private void LoginFailedUnknownReasonHandler(Network.LoginFailedUnknownReasonEvent eventArgs)
		{
			this.LoginLog(string.Concat("LoginFailedUnknownReasonHandler called, reason: ", eventArgs.Reason));
			this.MobileDisconnect(DisconnectReason.Generic);
		}

		private void LoginFailedVeteranAccountHandler(Network.LoginFailedVeteranAccountEvent eventArgs)
		{
			this.LoginLog("LoginFailedVeteranAccountHandler called");
			this.MobileDisconnect(DisconnectReason.ConsumptionTimeNotAllowed);
		}

		public void LoginLog(string message)
		{
		}

		private void MobileConnect()
		{
			this.LoginLog(string.Concat("Connecting to: ", this.m_mobileServerAddress));
			List<byte> list = this.m_realmJoinTicket.ToList<byte>();
			List<byte> nums = this.m_joinSecret.ToList<byte>();
			List<byte> list1 = this.m_clientSecret.ToList<byte>();
			List<byte> nums1 = BattleNet.GetSessionKey().ToList<byte>();
			ulong myAccountId = BattleNet.GetMyAccountId().hi;
			bgs.types.EntityId entityId = BattleNet.GetMyAccountId();
			Network.SetLoginData(list, nums, list1, nums1, myAccountId, entityId.lo, BattleNet.GetMyBattleTag());
			Network.SetSelectedRealm(this.m_virtualRealmAddress);
			Network.Connect(this.m_mobileServerAddress, this.m_mobileServerPort);
			this.m_mobileLoginTime = Time.timeSinceLevelLoad;
			this.SetLoginState(Login.eLoginState.MOBILE_CONNECTING);
		}

		public void MobileConnectDestroy()
		{
			if (Network.IsConnected())
			{
				this.LoginLog("MobileConnectDestroy(): Disconnecting.");
				Network.Disconnect();
			}
		}

		private void MobileConnected()
		{
			this.LoginLog("============= Mobile Connected ==================");
			this.SetLoginState(Login.eLoginState.MOBILE_LOGGING_IN);
			this.m_loggingIn = false;
		}

		private void MobileConnectFailed()
		{
			this.LoginUI.ShowTitlePanel();
			this.SetLoginState(Login.eLoginState.IDLE);
		}

		private void MobileConnecting()
		{
			if (Time.timeSinceLevelLoad <= this.m_mobileLoginTime + this.m_mobileLoginTimeout)
			{
				if (Network.IsConnected())
				{
					this.MobileConnected();
				}
				return;
			}
			this.LoginLog("MobileConnecting(): timeout exceeded while connecting");
			this.SetLoginState(Login.eLoginState.MOBILE_CONNECT_FAILED);
		}

		private void MobileDisconnect(DisconnectReason reason)
		{
			this.m_recentDisconnectReason = reason;
			this.LoginLog(string.Concat(new object[] { "Disconnecting for reason: ", reason, ". Network connected: ", Network.IsConnected(), ", authorized: ", Network.IsAuthorized(), ", logged in: ", Network.IsLoggedIn() }));
			if (!Network.IsConnected())
			{
				this.SetLoginState(Login.eLoginState.MOBILE_DISCONNECTED);
			}
			else
			{
				this.SetLoginState(Login.eLoginState.MOBILE_DISCONNECTING);
				Network.Logout();
				Network.Disconnect();
			}
		}

		private void MobileDisconnected()
		{
			this.LoginLog("Disconnected");
			this.LoginUI.HideAllPopups();
			this.LoginUI.ShowTitlePanel();
			this.SetLoginState(Login.eLoginState.MOBILE_DISCONNECTED_IDLE);
			string str = null;
			string str1 = null;
			switch (this.m_recentDisconnectReason)
			{
				case DisconnectReason.ConnectionLost:
				case DisconnectReason.PingTimeout:
				{
					str1 = StaticDB.GetString("DISCONNECTED_BY_SERVER", null);
					break;
				}
				case DisconnectReason.TimeoutContactingServer:
				case DisconnectReason.Generic:
				{
					str1 = StaticDB.GetString("CANT_CONNECT", null);
					break;
				}
				case DisconnectReason.AppVersionOld:
				{
					str = StaticDB.GetString("UPDATE_REQUIRED", null);
					str1 = StaticDB.GetString("UPDATE_REQUIRED_DESCRIPTION", null);
					break;
				}
				case DisconnectReason.AppVersionNew:
				{
					str = StaticDB.GetString("INCOMPATIBLE_REALM", null);
					str1 = StaticDB.GetString("INCOMPATIBLE_REALM_DESCRIPTION", null);
					break;
				}
				case DisconnectReason.CharacterInWorld:
				{
					str1 = StaticDB.GetString("ALREADY_LOGGED_IN", null);
					break;
				}
				case DisconnectReason.CantEnterWorld:
				{
					str1 = StaticDB.GetString("CHARACTER_UNAVAILABLE", null);
					break;
				}
				case DisconnectReason.LoginDisabled:
				{
					str1 = StaticDB.GetString("LOGIN_UNAVAILABLE", null);
					break;
				}
				case DisconnectReason.TrialNotAllowed:
				{
					str1 = StaticDB.GetString("TRIAL_LOGIN_UNAVAILABLE", null);
					break;
				}
				case DisconnectReason.ConsumptionTimeNotAllowed:
				{
					str1 = StaticDB.GetString("SUBSCRIPTION_REQUIRED", null);
					break;
				}
			}
			if (str != null && str1 != null)
			{
				this.LoginUI.ShowGenericPopup(str, str1);
				if (SceneManager.GetActiveScene().name != Scenes.TitleSceneName)
				{
					GenericPopup.DisabledAction += new Action(this.ReturnToTitleScene);
				}
			}
			else if (str1 != null)
			{
				this.LoginUI.ShowGenericPopupFull(str1);
				if (SceneManager.GetActiveScene().name != Scenes.TitleSceneName)
				{
					GenericPopup.DisabledAction += new Action(this.ReturnToTitleScene);
				}
			}
			this.m_recentDisconnectReason = DisconnectReason.None;
		}

		private void MobileDisconnectedByServer()
		{
			this.LoginLog("DisconnectedByServer");
			this.LoginUI.HideAllPopups();
			this.LoginUI.ShowTitlePanel();
			this.SetLoginState(Login.eLoginState.MOBILE_DISCONNECTED_IDLE);
			if (this.m_currentServerProtocolVersion < 0)
			{
				this.LoginUI.ShowGenericPopupFull(StaticDB.GetString("DISCONNECTED_BY_SERVER", null));
			}
			else if (this.m_currentServerProtocolVersion > this.m_expectedServerProtocolVersion)
			{
				this.LoginUI.ShowGenericPopup(StaticDB.GetString("UPDATE_REQUIRED", null), StaticDB.GetString("UPDATE_REQUIRED_DESCRIPTION", null));
			}
			else if (this.m_currentServerProtocolVersion >= this.m_expectedServerProtocolVersion)
			{
				this.LoginUI.ShowGenericPopupFull(StaticDB.GetString("DISCONNECTED_BY_SERVER", null));
			}
			else
			{
				this.LoginUI.ShowGenericPopup(StaticDB.GetString("INCOMPATIBLE_REALM", null), StaticDB.GetString("INCOMPATIBLE_REALM_DESCRIPTION", null));
			}
		}

		private void MobileDisconnecting()
		{
			if (!Network.IsConnected())
			{
				this.SetLoginState(Login.eLoginState.MOBILE_DISCONNECTED);
			}
		}

		private void MobileLoggedIn()
		{
			Main.instance.MobileLoggedIn();
			this.SetLoginState(Login.eLoginState.MOBILE_LOGGED_IN_IDLE);
			this.LoginUI.HideAllPopups();
			this.m_loggingIn = false;
			this.m_mobileLoginTime = Time.timeSinceLevelLoad;
		}

		private void MobileLoggedInIdle()
		{
			if (Time.timeSinceLevelLoad > this.m_mobileLoginTime + this.m_mobileLoginTimeout)
			{
				this.LoginLog("Initial data request attempt timed out.");
				this.MobileDisconnect(DisconnectReason.TimeoutContactingServer);
			}
		}

		private void MobileLoggingIn()
		{
			if (!Network.IsConnected())
			{
				this.LoginLog("Mobile network disconnected.");
				this.MobileDisconnect(DisconnectReason.ConnectionLost);
				return;
			}
			if (Time.timeSinceLevelLoad > this.m_mobileLoginTime + this.m_mobileLoginTimeout)
			{
				this.LoginLog("Mobile login attempt timed out.");
				this.MobileDisconnect(DisconnectReason.TimeoutContactingServer);
				return;
			}
			if (Network.IsAuthorized())
			{
				if (!this.m_loggingIn)
				{
					this.LoginLog("Attempting to log in");
					Network.Login(Singleton<CharacterData>.Instance.PlayerGuid);
					this.m_loggingIn = true;
				}
				else if (Network.IsLoggedIn())
				{
					this.SetLoginState(Login.eLoginState.MOBILE_LOGGED_IN);
				}
			}
		}

		public void MobileLoginDataRequestComplete()
		{
			this.SetLoginState(Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE);
		}

		private void OnApplicationPause(bool paused)
		{
			this.LoginLog(string.Concat("OnApplicationPause: ", paused.ToString()));
			if (Main.instance != null)
			{
				if (!paused)
				{
					Main.instance.ClearPendingNotifications();
				}
				else
				{
					Main.instance.ScheduleNotifications();
				}
			}
			if (!paused)
			{
				if (this.m_loginState == Login.eLoginState.UPDATE_REQUIRED_START || this.m_loginState == Login.eLoginState.UPDATE_REQUIRED_IDLE)
				{
					this.LoginLog("OnApplicationPause: Update required, exiting early.");
					return;
				}
				int num = GeneralHelpers.CurrentUnixTime() - this.m_pauseTimestamp;
				bool flag = num > 30;
				if (flag)
				{
					if (this.LoginUI != null)
					{
						this.LoginUI.HideAllPopups();
					}
					if (AdventureMapPanel.instance != null)
					{
						AdventureMapPanel.instance.SelectMissionFromList(0);
					}
					switch (this.m_loginState)
					{
						case Login.eLoginState.IDLE:
						case Login.eLoginState.WAIT_FOR_ASSET_BUNDLES:
						case Login.eLoginState.WEB_AUTH_START:
						case Login.eLoginState.WEB_AUTH_LOADING:
						case Login.eLoginState.WEB_AUTH_IN_PROGRESS:
						{
							break;
						}
						default:
						{
							this.LoginUI.ShowConnectingPanel();
							break;
						}
					}
				}
				BattleNet.ApplicationWasUnpaused();
				if (!this.m_initialUnpause)
				{
					if (flag)
					{
						this.LoginLog("reconnect update test");
						Singleton<AssetBundleManager>.instance.UpdateVersion();
						if (!this.IsUpdateAvailable())
						{
							this.LoginLog("updateFound = false");
						}
						else
						{
							this.SetLoginState(Login.eLoginState.UPDATE_REQUIRED_START);
							this.CancelWebAuth();
							this.LoginLog("updateFound = true");
						}
					}
				}
				Login.eLoginState mLoginState = this.m_loginState;
				switch (mLoginState)
				{
					case Login.eLoginState.MOBILE_LOGGING_IN:
					{
						this.LoginLog("OnApplicationPause: Reconnecting: mobile login states");
						this.m_battlenetFailures = 0;
						this.ReturnToTitleScene();
						break;
					}
					case Login.eLoginState.MOBILE_LOGGED_IN:
					case Login.eLoginState.MOBILE_LOGGED_IN_IDLE:
					case Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE:
					{
						if (flag)
						{
							this.LoginLog("OnApplicationPause: Reconnecting: mobile idle state");
							this.m_battlenetFailures = 0;
							this.ReturnToRecentCharacter = true;
							Network.Logout();
							Network.Disconnect();
							this.ReturnToTitleScene();
						}
						else if (Network.IsConnected())
						{
							this.LoginLog(string.Concat("OnApplicationPause: Still connected. Not reconnecting after short pause time of ", num));
						}
						else
						{
							this.LoginLog(string.Concat("OnApplicationPause: Reconnecting, not connected after short pause time of ", num));
							this.m_battlenetFailures = 0;
							this.ReturnToTitleScene();
						}
						break;
					}
					case Login.eLoginState.UPDATE_REQUIRED_START:
					case Login.eLoginState.UPDATE_REQUIRED_IDLE:
					{
						break;
					}
					default:
					{
						switch (mLoginState)
						{
							case Login.eLoginState.WAIT_FOR_ASSET_BUNDLES:
							{
								this.LoginLog("OnApplicationPause: Wait for asset bundles");
								break;
							}
							case Login.eLoginState.WEB_AUTH_START:
							case Login.eLoginState.WEB_AUTH_LOADING:
							case Login.eLoginState.WEB_AUTH_IN_PROGRESS:
							{
								this.LoginUI.HideConnectingPanel();
								this.LoginLog("OnApplicationPause: Hiding all panels");
								break;
							}
							default:
							{
								if (mLoginState != Login.eLoginState.BN_CHARACTER_LIST_WAIT)
								{
									if (this.m_loginState != Login.eLoginState.IDLE)
									{
										this.LoginLog("OnApplicationPause: Back to Title");
										this.ReturnToTitleScene();
									}
								}
								else if (!flag)
								{
									this.LoginLog(string.Concat("OnApplicationPause: Not reconnecting after short pause time of ", num));
								}
								else
								{
									this.m_battlenetFailures = 0;
									this.LoginLog("OnApplicationPause: Reconnecting: character list wait state");
									this.ReturnToTitleScene();
								}
								break;
							}
						}
						break;
					}
				}
				this.m_initialUnpause = false;
			}
			else
			{
				BattleNet.ApplicationWasPaused();
				this.m_pauseTimestamp = GeneralHelpers.CurrentUnixTime();
			}
		}

		public void OnClickBackToAccountSelect()
		{
			this.BackToAccountSelect();
		}

		public void OnClickCharacterSelectCancel()
		{
			this.LoginUI.ShowLogoutConfirmationPopup(false);
		}

		public void OnClickConnectingCancel()
		{
			if (!this.ReturnToRecentCharacter)
			{
				this.CancelLogin();
			}
			else
			{
				this.StartCachedLogin(true, true);
			}
		}

		public void OnClickTitleConnect()
		{
			if (!this.HaveCachedWebToken())
			{
				this.StartNewLogin();
			}
			else
			{
				this.LoginUI.ShowLogoutConfirmationPopup(true);
			}
		}

		public void OnClickTitleResume()
		{
			this.StartCachedLogin(true, false);
		}

		private void OnDestroy()
		{
			if (!base.IsCloneGettingRemoved)
			{
				Network.Shutdown();
				this.UnsubscribeFromEvents();
			}
		}

		public void OnLogoutCancel()
		{
			this.LoginUI.HideAllPopups();
		}

		public void OnLogoutConfirmed(bool goToWebAuth)
		{
			this.LoginUI.HideAllPopups();
			if (this.GetLoginState() == Login.eLoginState.MOBILE_LOGGED_IN_IDLE || this.GetLoginState() == Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE)
			{
				this.InitiateMobileDisconnect();
			}
			if (!goToWebAuth)
			{
				this.BackToAccountSelect();
			}
			else
			{
				this.StartNewLogin();
			}
		}

		private void OnReturnToTitleScene(Scene scene, LoadSceneMode loadSceneMode)
		{
			this.OnLogoutConfirmed(false);
			SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnReturnToTitleScene);
		}

		private void ReconnectPopupDisabledAction()
		{
			GenericPopup.DisabledAction -= new Action(this.ReconnectPopupDisabledAction);
			if (Network.IsConnected())
			{
				Network.Logout();
				Network.Disconnect();
			}
			this.BnLoginStart(true, true, true, false);
		}

		public void ReconnectToMobileServerCachedCharacter()
		{
			if (Network.IsConnected())
			{
				Network.Logout();
				Network.Disconnect();
			}
			this.BnLoginStart(true, true, true, false);
		}

		public void ReconnectToMobileServerCharacterSelect()
		{
			if (Network.IsConnected())
			{
				Network.Logout();
				Network.Disconnect();
			}
			this.BnLoginStart(true, true, false, false);
		}

		private void RegisterPushManager(string token, string locale, string bnetAccountID)
		{
			string str = locale.Substring(2);
			BLPushManagerBuilder didReceiveRegistrationTokenDelegate = ScriptableObject.CreateInstance<BLPushManagerBuilder>();
			if (Login.m_portal.ToLower() != "wow-dev")
			{
				didReceiveRegistrationTokenDelegate.isDebug = false;
				didReceiveRegistrationTokenDelegate.applicationName = "wowcompanion";
			}
			else
			{
				didReceiveRegistrationTokenDelegate.isDebug = true;
				didReceiveRegistrationTokenDelegate.applicationName = "test.wowcompanion";
			}
			didReceiveRegistrationTokenDelegate.shouldRegisterwithBPNS = true;
			didReceiveRegistrationTokenDelegate.region = str;
			didReceiveRegistrationTokenDelegate.locale = locale;
			didReceiveRegistrationTokenDelegate.authToken = token;
			didReceiveRegistrationTokenDelegate.authRegion = str;
			didReceiveRegistrationTokenDelegate.appAccountID = bnetAccountID;
			didReceiveRegistrationTokenDelegate.senderId = "952133414280";
			didReceiveRegistrationTokenDelegate.didReceiveRegistrationTokenDelegate = new DidReceiveRegistrationTokenDelegate(this.DidReceiveRegistrationTokenHandler);
			didReceiveRegistrationTokenDelegate.didReceiveDeeplinkURLDelegate = new DidReceiveDeeplinkURLDelegate(this.DidReceiveDeeplinkURLDelegateHandler);
			BLPushManager.instance.InitWithBuilder(didReceiveRegistrationTokenDelegate);
			BLPushManager.instance.RegisterForPushNotifications();
			this.LoginLog(string.Concat("Registered for push using game account ", bnetAccountID, ", region ", str));
		}

		private void RemoveWebTokenFromCaches()
		{
			RecentCharacter recentCharacter = null;
			foreach (RecentCharacter mRecentCharacter in this.m_recentCharacters)
			{
				if (mRecentCharacter.WebToken != this.m_webToken)
				{
					continue;
				}
				recentCharacter = mRecentCharacter;
			}
			if (recentCharacter != null)
			{
				this.m_recentCharacters.Remove(recentCharacter);
				this.SaveRecentCharacters();
			}
		}

		private void RequestGameAccountNames()
		{
			this.SetLoginState(Login.eLoginState.BN_ACCOUNT_NAME_WAIT);
			foreach (bnet.protocol.EntityId gameAccountList in BattleNet.GetGameAccountList())
			{
				Login.GameAccountStateCallback gameAccountStateCallback = new Login.GameAccountStateCallback()
				{
					EntityID = gameAccountList
				};
				GetGameAccountStateRequest getGameAccountStateRequest = new GetGameAccountStateRequest()
				{
					GameAccountId = gameAccountList,
					Options = new GameAccountFieldOptions()
				};
				getGameAccountStateRequest.Options.SetFieldGameLevelInfo(true);
				getGameAccountStateRequest.Options.SetFieldGameStatus(true);
				BattleNet.GetGameAccountState(getGameAccountStateRequest, new RPCContextDelegate(gameAccountStateCallback.Callback));
			}
		}

		public void ReturnToTitleScene()
		{
			GenericPopup.DisabledAction -= new Action(this.ReturnToTitleScene);
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(Singleton<Login>.instance.OnReturnToTitleScene);
			SceneManager.LoadSceneAsync(Scenes.TitleSceneName);
		}

		private void SaveRecentCharacters()
		{
			int i;
			for (i = 0; i < this.m_recentCharacters.Count && i < 3; i++)
			{
				string str = MessageFactory.Serialize(this.m_recentCharacters[i]);
				SecurePlayerPrefs.SetString(string.Concat("RecentCharacter", i), str, Main.uniqueIdentifier);
			}
			while (i < 3)
			{
				SecurePlayerPrefs.DeleteKey(string.Concat("RecentCharacter", i));
				i++;
			}
			PlayerPrefs.Save();
		}

		public void SelectCharacterNew(JamJSONCharacterEntry characterEntry, string subRegion)
		{
			if (this.GetLoginState() == Login.eLoginState.MOBILE_LOGGED_IN_IDLE)
			{
				this.LoginUI.CloseCharacterListDialog();
			}
			this.m_selectedCharacterEntry = characterEntry;
			SecurePlayerPrefs.SetString("CharacterID", characterEntry.PlayerGuid, Main.uniqueIdentifier);
			SecurePlayerPrefs.SetString("CharacterName", characterEntry.Name, Main.uniqueIdentifier);
			PlayerPrefs.Save();
			Login.eLoginState loginState = Singleton<Login>.instance.GetLoginState();
			if (loginState == Login.eLoginState.MOBILE_LOGGED_IN_IDLE || loginState == Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE)
			{
				this.ReconnectToMobileServerCachedCharacter();
			}
			else
			{
				this.SendRealmJoinRequest("dummyRealmName", (ulong)characterEntry.VirtualRealmAddress, subRegion);
				Singleton<Login>.instance.SetLoginState(Login.eLoginState.BN_REALM_JOIN_WAIT);
				this.LoginUI.ShowConnectingPanel();
			}
			Singleton<CharacterData>.Instance.CopyCharacterEntry(this.m_selectedCharacterEntry);
		}

		public void SelectGameAccount(bnet.protocol.EntityId gameAccount)
		{
			this.m_useCachedWoWAccount = true;
			Login.LoginGameAccount loginGameAccount = null;
			foreach (Login.LoginGameAccount mLoginGameAccount in this.m_loginGameAccounts)
			{
				if (mLoginGameAccount.gameAccount != gameAccount)
				{
					continue;
				}
				loginGameAccount = mLoginGameAccount;
			}
			if (loginGameAccount == null)
			{
				this.LoginLog(string.Concat("SelectGameAccount: Could not find account ", gameAccount.ToString()));
				this.BnLoginFailed(null, null);
				return;
			}
			if (loginGameAccount.isBanned)
			{
				if (BattleNet.GetGameAccountList().Count <= 1)
				{
					this.BnLoginFailed(StaticDB.GetString("BANNED", null), StaticDB.GetString("BANNED_LONG", null));
				}
				else
				{
					this.LoginUI.ShowGenericPopup(StaticDB.GetString("BANNED", null), StaticDB.GetString("BANNED_LONG", null));
				}
				return;
			}
			if (loginGameAccount.isSuspended)
			{
				if (BattleNet.GetGameAccountList().Count <= 1)
				{
					this.BnLoginFailed(StaticDB.GetString("SUSPENDED", null), StaticDB.GetString("SUSPENDED_LONG", null));
				}
				else
				{
					this.LoginUI.ShowGenericPopup(StaticDB.GetString("SUSPENDED", null), StaticDB.GetString("SUSPENDED_LONG", null));
				}
				return;
			}
			ulong high = gameAccount.High;
			SecurePlayerPrefs.SetString("GameAccountHigh", high.ToString(), Main.uniqueIdentifier);
			ulong low = gameAccount.Low;
			SecurePlayerPrefs.SetString("GameAccountLow", low.ToString(), Main.uniqueIdentifier);
			PlayerPrefs.Save();
			this.m_gameAccount = gameAccount;
			this.SetLoginState(Login.eLoginState.BN_TICKET_WAIT);
			this.SendRealmListTicketRequest();
			RealmListView realmListView = this.LoginUI.RealmListView;
			realmListView.ClearBnRealmList();
			realmListView.SetRealmListTitle();
		}

		public void SelectRecentCharacter(RecentCharacter recentChar, string subRegion)
		{
			SecurePlayerPrefs.SetString("WebToken", recentChar.WebToken, Main.uniqueIdentifier);
			ulong high = recentChar.GameAccount.High;
			SecurePlayerPrefs.SetString("GameAccountHigh", high.ToString(), Main.uniqueIdentifier);
			ulong low = recentChar.GameAccount.Low;
			SecurePlayerPrefs.SetString("GameAccountLow", low.ToString(), Main.uniqueIdentifier);
			SecurePlayerPrefs.SetString("CharacterID", recentChar.Entry.PlayerGuid, Main.uniqueIdentifier);
			SecurePlayerPrefs.SetString("CharacterName", recentChar.Entry.Name, Main.uniqueIdentifier);
			PlayerPrefs.Save();
			if (Network.IsConnected())
			{
				Network.Logout();
				Network.Disconnect();
			}
			this.BnLoginStart(true, true, true, false);
		}

		private void SendCharacterListRequest(string subRegion)
		{
			this.LoginLog(string.Concat("Sending realm list ClientRequest for subregion ", subRegion));
			ClientRequest clientRequest = new ClientRequest();
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			attribute.SetName(MobileBuild.GetCharacterListRequestName());
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetStringValue(subRegion);
			attribute.SetValue(variant);
			clientRequest.AddAttribute(attribute);
			bnet.protocol.attribute.Attribute attribute1 = new bnet.protocol.attribute.Attribute();
			attribute1.SetName("Param_RealmListTicket");
			bnet.protocol.attribute.Variant variant1 = new bnet.protocol.attribute.Variant();
			variant1.SetBlobValue(this.m_realmListTicket);
			attribute1.SetValue(variant1);
			clientRequest.AddAttribute(attribute1);
			Login.CharacterListCallback characterListCallback = new Login.CharacterListCallback()
			{
				SubRegion = subRegion
			};
			BattleNet.Get().SendClientRequest(clientRequest, new RPCContextDelegate(characterListCallback.Callback));
			this.LoginLog(string.Concat("---------------------------Sent Character List request for subregion ", subRegion, " -------------------------------"));
		}

		public void SendRealmJoinRequest(string realmName, ulong realmAddress, string subRegion)
		{
			if (this.m_realmJoinInfo != null && !string.IsNullOrEmpty(this.m_realmJoinInfo.IpAddress))
			{
				return;
			}
			this.m_subRegion = subRegion;
			ClientRequest clientRequest = new ClientRequest();
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			attribute.SetName(MobileBuild.GetRealmJoinRequestName());
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetStringValue(subRegion);
			attribute.SetValue(variant);
			clientRequest.AddAttribute(attribute);
			bnet.protocol.attribute.Attribute attribute1 = new bnet.protocol.attribute.Attribute();
			attribute1.SetName("Param_RealmAddress");
			bnet.protocol.attribute.Variant variant1 = new bnet.protocol.attribute.Variant();
			variant1.SetUintValue(realmAddress);
			attribute1.SetValue(variant1);
			clientRequest.AddAttribute(attribute1);
			bnet.protocol.attribute.Attribute attribute2 = new bnet.protocol.attribute.Attribute();
			attribute2.SetName("Param_RealmListTicket");
			bnet.protocol.attribute.Variant variant2 = new bnet.protocol.attribute.Variant();
			variant2.SetBlobValue(this.m_realmListTicket);
			attribute2.SetValue(variant2);
			clientRequest.AddAttribute(attribute2);
			bnet.protocol.attribute.Attribute attribute3 = new bnet.protocol.attribute.Attribute();
			attribute3.SetName("Param_BnetSessionKey");
			bnet.protocol.attribute.Variant variant3 = new bnet.protocol.attribute.Variant();
			variant3.SetBlobValue(BattleNet.GetSessionKey());
			attribute3.SetValue(variant3);
			clientRequest.AddAttribute(attribute3);
			this.m_realmJoinInfo = new Login.RealmJoinInfo()
			{
				RealmAddress = realmAddress
			};
			BattleNet.Get().SendClientRequest(clientRequest, new RPCContextDelegate(this.m_realmJoinInfo.ClientResponseCallback));
			PlayerPrefs.Save();
			this.LoginLog("----------------------------Sent realm join info request------------------------------");
		}

		private void SendRealmListTicketRequest()
		{
			this.LoginLog("Sending realm list ticket ClientRequest...");
			ClientRequest clientRequest = new ClientRequest();
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			attribute.SetName(MobileBuild.GetRealmListTicketRequestName());
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetIntValue((long)0);
			attribute.SetValue(variant);
			clientRequest.AddAttribute(attribute);
			bnet.protocol.attribute.Attribute attribute1 = new bnet.protocol.attribute.Attribute();
			attribute1.SetName("Param_Identity");
			JSONRealmListTicketIdentity jSONRealmListTicketIdentity = new JSONRealmListTicketIdentity();
			byte high = (byte)((this.m_gameAccount.High & 1095216660480L) >> 32);
			jSONRealmListTicketIdentity.GameAccountRegion = high;
			jSONRealmListTicketIdentity.GameAccountID = this.m_gameAccount.Low;
			this.LoginLog(string.Concat("Region is ", high));
			this.LoginLog(string.Concat("GameAccountRegion: ", jSONRealmListTicketIdentity.GameAccountRegion));
			this.LoginLog(string.Concat("GameAccountID: ", jSONRealmListTicketIdentity.GameAccountID));
			bgs.types.EntityId myAccountId = BattleNet.GetMyAccountId();
			this.LoginLog(string.Format("BNetAccount-0-{0:X12}", myAccountId.lo));
			this.LoginLog(string.Format("WowAccount-0-{0:X12}", this.m_gameAccount.Low));
			string str = string.Concat(jSONRealmListTicketIdentity.ToString().Substring(jSONRealmListTicketIdentity.ToString().LastIndexOf(".") + 1), ":");
			str = string.Concat(str, JsonConvert.SerializeObject(jSONRealmListTicketIdentity, Formatting.None));
			str = string.Concat(str, '\0');
			bnet.protocol.attribute.Variant variant1 = new bnet.protocol.attribute.Variant();
			variant1.SetBlobValue(Encoding.UTF8.GetBytes(str));
			this.LoginLog(string.Concat("Identity: <", Encoding.UTF8.GetString(variant1.BlobValue), ">"));
			attribute1.SetValue(variant1);
			clientRequest.AddAttribute(attribute1);
			bnet.protocol.attribute.Attribute attribute2 = new bnet.protocol.attribute.Attribute();
			attribute2.SetName("Param_ClientInfo");
			FourCC fourCC = new FourCC();
			fourCC.SetValue((uint)MobileBuild.GetClientId());
			FourCC fourCC1 = new FourCC();
			fourCC1.SetString(BattleNet.Client().GetPlatformName());
			FourCC fourCC2 = new FourCC();
			fourCC2.SetString(Main.instance.GetLocale());
			JSONRealmListTicketClientInformation jSONRealmListTicketClientInformation = new JSONRealmListTicketClientInformation()
			{
				Info = new JamJSONRealmListTicketClientInformation()
				{
					Type = fourCC.GetValue(),
					Platform = fourCC1.GetValue(),
					BuildVariant = MobileBuild.GetBuildVariant(),
					TextLocale = fourCC2.GetValue(),
					AudioLocale = fourCC2.GetValue(),
					Version = new JamJSONGameVersion()
					{
						VersionMajor = (uint)MobileBuild.GetVersionMajor(),
						VersionMinor = (uint)MobileBuild.GetVersionMinor(),
						VersionRevision = (uint)MobileBuild.GetVersionRevision(),
						VersionBuild = (uint)MobileBuild.GetBuildNum()
					},
					VersionDataBuild = (uint)MobileBuild.GetDataBuildNum(),
					CurrentTime = (int)BattleNet.Get().CurrentUTCTime(),
					TimeZone = "Etc/UTC"
				}
			};
			this.m_clientSecret = WowAuthCrypto.GenerateSecret();
			jSONRealmListTicketClientInformation.Info.Secret = this.m_clientSecret;
			jSONRealmListTicketClientInformation.Info.PlatformType = MobileBuild.GetPlatformType();
			jSONRealmListTicketClientInformation.Info.ClientArch = MobileBuild.GetClientArchitecture();
			jSONRealmListTicketClientInformation.Info.SystemArch = MobileBuild.GetSystemArchitecture();
			jSONRealmListTicketClientInformation.Info.SystemVersion = MobileBuild.GetOSVersion();
			this.LoginLog(string.Concat("clientInfoJSON.Info.Type is ", jSONRealmListTicketClientInformation.Info.Type));
			this.LoginLog(string.Concat("clientInfoJSON.Info.Textlocale is ", jSONRealmListTicketClientInformation.Info.TextLocale));
			this.LoginLog(string.Concat("clientInfoJSON.Info.Version.VersionBuild is ", jSONRealmListTicketClientInformation.Info.Version.VersionBuild));
			string str1 = string.Concat(jSONRealmListTicketClientInformation.ToString().Substring(jSONRealmListTicketClientInformation.ToString().LastIndexOf(".") + 1), ":");
			str1 = string.Concat(str1, JsonConvert.SerializeObject(jSONRealmListTicketClientInformation, Formatting.None, MessageFactory.SerializerSettings));
			str1 = string.Concat(str1, '\0');
			bnet.protocol.attribute.Variant variant2 = new bnet.protocol.attribute.Variant();
			variant2.SetBlobValue(Encoding.UTF8.GetBytes(str1));
			this.LoginLog(string.Concat("info: <", Encoding.UTF8.GetString(variant2.BlobValue), ">"));
			attribute2.SetValue(variant2);
			clientRequest.AddAttribute(attribute2);
			BattleNet.Get().SendClientRequest(clientRequest, null);
			this.LoginLog("-------------------------Sent realm list TICKET request--------------------------");
		}

		private void SendSubRegionListRequest()
		{
			BattleNet.GetAllValuesForAttribute(MobileBuild.GetCharacterListRequestName(), 1);
		}

		private void SetInitialPortal()
		{
			string str;
			int num;
			string str1 = SecurePlayerPrefs.GetString("Portal", Main.uniqueIdentifier);
			if (str1 != null && str1 != string.Empty)
			{
				Login.m_portal = str1;
			}
			else if (!this.IsDevRegionList())
			{
				string locale = Main.instance.GetLocale();
				if (locale != null)
				{
					if (Login.<>f__switch$map7 == null)
					{
						Dictionary<string, int> strs = new Dictionary<string, int>(7)
						{
							{ "frFR", 0 },
							{ "deDE", 0 },
							{ "ruRU", 0 },
							{ "itIT", 0 },
							{ "koKR", 1 },
							{ "zhTW", 1 },
							{ "zhCN", 2 }
						};
						Login.<>f__switch$map7 = strs;
					}
					if (Login.<>f__switch$map7.TryGetValue(locale, out num))
					{
						switch (num)
						{
							case 0:
							{
								str = "eu";
								goto Label0;
							}
							case 1:
							{
								str = "kr";
								goto Label0;
							}
							case 2:
							{
								str = "cn";
								goto Label0;
							}
						}
					}
				}
				str = "us";
			Label0:
				this.LoginLog(string.Concat("Setting initial portal to ", str));
				this.SetPortal(str);
			}
		}

		public void SetJoinSecret(byte[] secret)
		{
			this.m_joinSecret = secret;
		}

		private void SetLoginState(Login.eLoginState newState)
		{
			if ((this.m_loginState == Login.eLoginState.UPDATE_REQUIRED_START || this.m_loginState == Login.eLoginState.UPDATE_REQUIRED_IDLE) && newState != Login.eLoginState.UPDATE_REQUIRED_START && newState != Login.eLoginState.UPDATE_REQUIRED_IDLE && (Singleton<AssetBundleManager>.instance.ForceUpgrade || newState != Login.eLoginState.IDLE))
			{
				this.LoginLog(string.Concat("SetLoginState(): Update required, igoring state change to ", newState.ToString(), "."));
				return;
			}
			this.LoginLog(string.Concat(new string[] { "SetLoginState(): from ", this.m_loginState.ToString(), " to ", newState.ToString(), "." }));
			this.m_loginState = newState;
		}

		public void SetPortal(string newValue)
		{
			Login.m_portal = newValue.ToLower();
			SecurePlayerPrefs.SetString("Portal", Login.m_portal, Main.uniqueIdentifier);
			PlayerPrefs.Save();
			this.LoginLog(string.Concat("Setting portal to ", Login.m_portal));
		}

		public void SetRegionIndex()
		{
			this.LoginUI.SetRegionIndex();
			this.LoginUI.HideAllPopups();
		}

		public void ShowCreateNewLoginPanel()
		{
			this.LoginUI.ShowCreateNewLoginPanel();
		}

		public void ShowRealmListPanel()
		{
			this.LoginUI.ShowRealmListPanel();
		}

		public void ShowWebAuthView()
		{
			if (this.m_webAuth != null && this.m_loginState == Login.eLoginState.WEB_AUTH_IN_PROGRESS)
			{
				this.LoginLog("============================= Showing WebAuth View ========================");
				this.m_webAuth.Show();
				this.LoginUI.ShowWebAuthPanel();
			}
		}

		private void Start()
		{
			this.LoginLog("-----START-----");
			this.m_urlDownloader = new DotNetUrlDownloader();
			this.SetInitialPortal();
			if (this.LoginUI == null)
			{
				return;
			}
			this.LoginUI.ShowConnectingPanel();
			this.SetLoginState(Login.eLoginState.WAIT_FOR_ASSET_BUNDLES);
			this.InitRecentCharacters();
			this.LoadRecentCharacters();
			this.ReturnToRecentCharacter = false;
			if (this.IsDevRegionList())
			{
				this.m_mobileLoginTimeout = 120f;
			}
			this.SubscribeToEvents();
			Network.Initialize();
		}

		public void StartCachedLogin(bool useCachedRealm, bool useCachedCharacter)
		{
			this.BnLoginStart(true, useCachedRealm, useCachedCharacter, false);
		}

		public void StartNewLogin()
		{
			this.BnLoginStart(false, false, false, false);
		}

		private void SubscribeToEvents()
		{
			Network.OnLoginFailedDuplicateCharacter += new Network.LoginFailedDuplicateCharacterHandler(this.LoginFailedDuplicateCharacterHandler);
			Network.OnLoginFailedUnknownReason += new Network.LoginFailedUnknownReasonHandler(this.LoginFailedUnknownReasonHandler);
			Network.OnLoginFailedVeteranAccount += new Network.LoginFailedVeteranAccountHandler(this.LoginFailedVeteranAccountHandler);
			Network.OnLoginFailedConsumptionTimeNotAllowed += new Network.LoginFailedConsumptionTimeNotAllowedHandler(this.LoginFailedConsumptionTimeNotAllowedHandler);
			Network.OnLoginFailedTrialNotAllowed += new Network.LoginFailedTrialNotAllowedHandler(this.LoginFailedTrialNotAllowedHandler);
		}

		private void UnsubscribeFromEvents()
		{
			Network.OnLoginFailedDuplicateCharacter -= new Network.LoginFailedDuplicateCharacterHandler(this.LoginFailedDuplicateCharacterHandler);
			Network.OnLoginFailedUnknownReason -= new Network.LoginFailedUnknownReasonHandler(this.LoginFailedUnknownReasonHandler);
			Network.OnLoginFailedVeteranAccount -= new Network.LoginFailedVeteranAccountHandler(this.LoginFailedVeteranAccountHandler);
			Network.OnLoginFailedConsumptionTimeNotAllowed -= new Network.LoginFailedConsumptionTimeNotAllowedHandler(this.LoginFailedConsumptionTimeNotAllowedHandler);
			Network.OnLoginFailedTrialNotAllowed -= new Network.LoginFailedTrialNotAllowedHandler(this.LoginFailedTrialNotAllowedHandler);
		}

		private void Update()
		{
			this.UpdateLoginState();
			this.BnErrorsUpdate();
			if (this.m_urlDownloader != null)
			{
				this.m_urlDownloader.Process();
			}
			Network.Update();
		}

		private void UpdateAppPopupDisabledAction()
		{
			GenericPopup.DisabledAction -= new Action(this.UpdateAppPopupDisabledAction);
			string str = null;
			str = (Singleton<Login>.instance.GetBnPortal() != "cn" ? Singleton<AssetBundleManager>.instance.AppStoreUrl : Singleton<AssetBundleManager>.instance.AppStoreUrl_CN);
			if (str != null)
			{
				Application.OpenURL(str);
			}
		}

		private void UpdateLoginState()
		{
			switch (this.m_loginState)
			{
				case Login.eLoginState.IDLE:
				{
					if (this.ReturnToRecentCharacter)
					{
						this.SetLoginState(Login.eLoginState.BN_LOGIN_START);
						this.ReturnToRecentCharacter = false;
					}
					break;
				}
				case Login.eLoginState.WAIT_FOR_ASSET_BUNDLES:
				{
					this.WaitForAssetBundles();
					break;
				}
				case Login.eLoginState.WEB_AUTH_START:
				{
					this.WebAuthStart();
					break;
				}
				case Login.eLoginState.WEB_AUTH_LOADING:
				{
					this.WebAuthUpdate();
					break;
				}
				case Login.eLoginState.WEB_AUTH_IN_PROGRESS:
				{
					this.WebAuthUpdate();
					break;
				}
				case Login.eLoginState.WEB_AUTH_FAILED:
				case Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE:
				{
					break;
				}
				case Login.eLoginState.BN_LOGIN_START:
				{
					this.BnLoginStart(true, true, true, false);
					break;
				}
				case Login.eLoginState.BN_LOGIN_WAIT_FOR_LOGON:
				{
					this.BnLoginWaitForLogon();
					break;
				}
				case Login.eLoginState.BN_LOGIN_PROVIDE_TOKEN:
				{
					this.BnLoginProvideToken();
					break;
				}
				case Login.eLoginState.BN_LOGGING_IN:
				case Login.eLoginState.BN_LOGIN_UNKNOWN:
				{
					this.BnLoginUpdate();
					break;
				}
				case Login.eLoginState.BN_ACCOUNT_NAME_WAIT:
				{
					this.BnAccountNameWait();
					break;
				}
				case Login.eLoginState.BN_TICKET_WAIT:
				{
					this.BnTicketWait();
					break;
				}
				case Login.eLoginState.BN_SUBREGION_LIST_WAIT:
				{
					this.BnSubRegionListWait();
					break;
				}
				case Login.eLoginState.BN_CHARACTER_LIST_WAIT:
				{
					this.BnCharacterListWait();
					break;
				}
				case Login.eLoginState.BN_REALM_JOIN_WAIT:
				{
					this.BnRealmJoinWait();
					break;
				}
				case Login.eLoginState.MOBILE_CONNECT:
				{
					this.MobileConnect();
					break;
				}
				case Login.eLoginState.MOBILE_CONNECTING:
				{
					this.MobileConnecting();
					break;
				}
				case Login.eLoginState.MOBILE_CONNECT_FAILED:
				{
					this.MobileConnectFailed();
					break;
				}
				case Login.eLoginState.MOBILE_DISCONNECTING:
				{
					this.MobileDisconnecting();
					break;
				}
				case Login.eLoginState.MOBILE_DISCONNECTED:
				{
					this.MobileDisconnected();
					break;
				}
				case Login.eLoginState.MOBILE_DISCONNECTED_BY_SERVER:
				{
					this.MobileDisconnectedByServer();
					break;
				}
				case Login.eLoginState.MOBILE_DISCONNECTED_IDLE:
				{
					break;
				}
				case Login.eLoginState.MOBILE_LOGGING_IN:
				{
					this.MobileLoggingIn();
					break;
				}
				case Login.eLoginState.MOBILE_LOGGED_IN:
				{
					this.MobileLoggedIn();
					break;
				}
				case Login.eLoginState.MOBILE_LOGGED_IN_IDLE:
				{
					this.MobileLoggedInIdle();
					break;
				}
				case Login.eLoginState.UPDATE_REQUIRED_START:
				{
					this.UpdateRequiredStart();
					break;
				}
				case Login.eLoginState.UPDATE_REQUIRED_IDLE:
				{
					this.UpdateRequiredIdle();
					break;
				}
				default:
				{
					goto case Login.eLoginState.MOBILE_LOGGED_IN_DATA_COMPLETE;
				}
			}
		}

		public void UpdateRecentCharacters()
		{
			RecentCharacter recentCharacter = new RecentCharacter()
			{
				Entry = this.m_selectedCharacterEntry,
				GameAccount = this.m_gameAccount,
				UnixTime = GeneralHelpers.CurrentUnixTime(),
				WebToken = this.m_webToken,
				SubRegion = this.m_subRegion,
				Version = 2
			};
			RecentCharacter recentCharacter1 = null;
			foreach (RecentCharacter mRecentCharacter in this.m_recentCharacters)
			{
				if (mRecentCharacter.Entry.PlayerGuid != recentCharacter.Entry.PlayerGuid)
				{
					continue;
				}
				recentCharacter1 = mRecentCharacter;
			}
			if (recentCharacter1 == null)
			{
				RecentCharacter recentCharacter2 = null;
				int unixTime = GeneralHelpers.CurrentUnixTime();
				foreach (RecentCharacter mRecentCharacter1 in this.m_recentCharacters)
				{
					if (mRecentCharacter1.UnixTime >= unixTime)
					{
						continue;
					}
					unixTime = mRecentCharacter1.UnixTime;
					recentCharacter2 = mRecentCharacter1;
				}
				if (recentCharacter2 != null && this.m_recentCharacters.Count >= 3)
				{
					this.m_recentCharacters.Remove(recentCharacter2);
				}
				this.m_recentCharacters.Add(recentCharacter);
			}
			else
			{
				this.m_recentCharacters.Remove(recentCharacter1);
				this.m_recentCharacters.Add(recentCharacter);
			}
			this.SaveRecentCharacters();
			int num = 0;
			for (int i = 0; i < this.m_recentCharacters.Count && i < 3; i++)
			{
				if (this.m_recentCharacters[i].Entry.PlayerGuid != this.m_selectedCharacterEntry.PlayerGuid)
				{
					int num1 = num;
					num = num1 + 1;
					this.LoginUI.SetRecentCharacter(num1, this.m_recentCharacters[i]);
				}
			}
			while (num < 3)
			{
				this.LoginUI.SetRecentCharacter(num, null);
				num++;
			}
		}

		private void UpdateRequiredIdle()
		{
			if (Singleton<AssetBundleManager>.instance.ForceUpgrade && !this.LoginUI.IsGenericPopupShowing())
			{
				this.LoginLog("UpdateRequiredIdle() opening popup");
				this.LoginUI.ShowTitlePanel();
				GenericPopup.DisabledAction += new Action(this.UpdateAppPopupDisabledAction);
				this.LoginUI.ShowGenericPopup(StaticDB.GetString("UPDATE_REQUIRED", null), StaticDB.GetString("UPDATE_REQUIRED_DESCRIPTION", null));
			}
		}

		private void UpdateRequiredStart()
		{
			this.LoginLog("UpdateRequiredStart()");
			this.LoginUI.ShowTitlePanel();
			GenericPopup.DisabledAction += new Action(this.UpdateAppPopupDisabledAction);
			this.LoginUI.ShowGenericPopup(StaticDB.GetString("UPDATE_REQUIRED", null), StaticDB.GetString("UPDATE_REQUIRED_DESCRIPTION", null));
			if (!Singleton<AssetBundleManager>.instance.ForceUpgrade)
			{
				this.LoginLog("UpdateRequiredStart() do not force upgrade");
				this.SetLoginState(Login.eLoginState.IDLE);
			}
			else
			{
				this.LoginLog("UpdateRequiredStart() force upgrade");
				this.SetLoginState(Login.eLoginState.UPDATE_REQUIRED_IDLE);
			}
		}

		public bool UseCachedCharacter()
		{
			return this.m_useCachedCharacter;
		}

		private void WaitForAssetBundles()
		{
			if (!Singleton<AssetBundleManager>.instance.IsInitialized())
			{
				return;
			}
			this.GetBnServerString();
			this.LoginLog(string.Concat("Latest version is ", Singleton<AssetBundleManager>.instance.LatestVersion));
			this.LoginLog(string.Concat("Force upgrade is ", Singleton<AssetBundleManager>.instance.ForceUpgrade));
			string str = SecurePlayerPrefs.GetString("WebToken", Main.uniqueIdentifier);
			if (this.IsUpdateAvailable())
			{
				this.SetLoginState(Login.eLoginState.UPDATE_REQUIRED_START);
			}
			else if (str != null && str != string.Empty)
			{
				if (this.LoginUI != null)
				{
					this.LoginUI.ShowConnectingPanel();
					this.SetLoginState(Login.eLoginState.BN_LOGIN_START);
				}
			}
			else if (this.LoginUI != null)
			{
				this.LoginUI.ShowTitlePanel();
				this.SetLoginState(Login.eLoginState.IDLE);
			}
		}

		private void WebAuthStart()
		{
			this.LoginUI.HidePanelsForWebAuth();
			if (this.m_webAuth == null)
			{
				GameObject gameObject = GameObject.Find("MainCanvas");
				if (gameObject == null)
				{
					throw new Exception("Canvas game obj was null in WebAuthStart");
				}
				Canvas component = gameObject.GetComponent<Canvas>();
				if (component == null)
				{
					throw new Exception("webCanvas was null in WebAuthStart");
				}
				if (this.m_webAuthUrl == null)
				{
					throw new Exception("m_webAuthUrl was null in WebAuthStart");
				}
				float single = 0f;
				float single1 = 0f;
				float single2 = component.pixelRect.width;
				float single3 = component.pixelRect.height;
				this.m_webAuth = new WebAuth(this.m_webAuthUrl, single, single1, single2, single3, "MainScriptObj");
				this.LoginLog(string.Concat("Created WebAuth, url: ", this.m_webAuthUrl));
			}
			if (this.m_webAuth == null)
			{
				throw new Exception("m_webAuth was null in WebAuthStart");
			}
			this.SetLoginState(Login.eLoginState.WEB_AUTH_IN_PROGRESS);
			string str = Main.instance.GetLocale().Substring(2);
			this.LoginLog(string.Concat("Setting web auth country code to ", str));
			this.m_webAuth.SetCountryCodeCookie(str, ".blizzard.net");
			WebAuth.ClearLoginData();
			this.m_webAuth.Load();
			this.LoginUI.ShowConnectingPanel();
			this.LoginLog("------------------------------ Loading WebAuth ----------------------------");
		}

		private void WebAuthUpdate()
		{
			BattleNet.ProcessAurora();
			this.BnErrorsUpdate();
			if (this.m_webAuth == null)
			{
				this.SetLoginState(Login.eLoginState.WEB_AUTH_FAILED);
				this.LoginLog("m_webAuth was null in WebAuthUpdate");
				return;
			}
			WebAuth.Status status = this.m_webAuth.GetStatus();
			if (status == WebAuth.Status.Success)
			{
				this.m_webToken = this.m_webAuth.GetLoginCode();
				this.LoginLog(string.Concat("Received web auth token: ", this.m_webToken));
				this.SetLoginState(Login.eLoginState.BN_LOGIN_PROVIDE_TOKEN);
				this.m_webAuth.Close();
				this.m_webAuth = null;
				this.LoginUI.ShowConnectingPanel();
			}
			else if (status == WebAuth.Status.Error)
			{
				this.LoginLog("Web auth status: Error.");
				this.SetLoginState(Login.eLoginState.WEB_AUTH_FAILED);
			}
		}

		private class CharacterListCallback
		{
			private string m_subRegion;

			private JSONRealmListUpdates m_updates;

			private JSONRealmCharacterList m_characters;

			public string SubRegion
			{
				get
				{
					return this.m_subRegion;
				}
				set
				{
					this.m_subRegion = value;
				}
			}

			public CharacterListCallback()
			{
			}

			public void Callback(RPCContext context)
			{
				Singleton<Login>.instance.LoginLog(string.Concat("--------- CharacterListCallback received for subregion ", this.SubRegion, " ----------"));
				Singleton<Login>.instance.m_realmJoinInfo = null;
				ClientResponse clientResponseFromContext = GamesAPI.GetClientResponseFromContext(context);
				if (clientResponseFromContext == null)
				{
					throw new Exception(string.Concat("ClientResponse was null in CharacterListCallback for sub region: ", this.SubRegion));
				}
				if (Singleton<Login>.instance.m_clearCharacterListOnReply)
				{
					Singleton<Login>.instance.m_clearCharacterListOnReply = false;
					Singleton<Login>.instance.LoginUI.ClearCharacterList();
				}
				foreach (bnet.protocol.attribute.Attribute attributeList in clientResponseFromContext.AttributeList)
				{
					if (attributeList.Name != "Param_RealmList")
					{
						if (attributeList.Name != "Param_CharacterList")
						{
							continue;
						}
						string str = Login.DecompressJsonAttribBlob(attributeList.Value.BlobValue);
						this.m_characters = JsonConvert.DeserializeObject<JSONRealmCharacterList>(str);
					}
					else
					{
						string str1 = Login.DecompressJsonAttribBlob(attributeList.Value.BlobValue);
						this.m_updates = JsonConvert.DeserializeObject<JSONRealmListUpdates>(str1);
					}
				}
				string str2 = SecurePlayerPrefs.GetString("CharacterID", Main.uniqueIdentifier);
				if (this.m_characters != null && this.m_updates != null)
				{
					JamJSONCharacterEntry[] characterList = this.m_characters.CharacterList;
					for (int i = 0; i < (int)characterList.Length; i++)
					{
						JamJSONCharacterEntry jamJSONCharacterEntry = characterList[i];
						string name = "unknown";
						bool populationState = false;
						bool flag = false;
						JamJSONRealmListUpdatePart[] updates = this.m_updates.Updates;
						int num = 0;
						while (num < (int)updates.Length)
						{
							JamJSONRealmListUpdatePart jamJSONRealmListUpdatePart = updates[num];
							if (jamJSONRealmListUpdatePart.Update.WowRealmAddress != jamJSONCharacterEntry.VirtualRealmAddress)
							{
								num++;
							}
							else
							{
								name = jamJSONRealmListUpdatePart.Update.Name;
								populationState = jamJSONRealmListUpdatePart.Update.PopulationState != 0;
								Singleton<Login>.instance.LoginUI.AddCharacterButton(jamJSONCharacterEntry, this.SubRegion, name, populationState);
								flag = true;
								break;
							}
						}
						flag;
						if (Singleton<Login>.instance.UseCachedCharacter() && str2 != null && jamJSONCharacterEntry.PlayerGuid == str2 && Singleton<Login>.Instance.GetLoginState() != Login.eLoginState.BN_REALM_JOIN_WAIT)
						{
							Singleton<Login>.instance.m_selectedCharacterEntry = jamJSONCharacterEntry;
							Singleton<Login>.instance.SendRealmJoinRequest("dummyRealmName", (ulong)jamJSONCharacterEntry.VirtualRealmAddress, this.SubRegion);
							Singleton<Login>.instance.SetLoginState(Login.eLoginState.BN_REALM_JOIN_WAIT);
							Singleton<CharacterData>.Instance.CopyCharacterEntry(jamJSONCharacterEntry);
						}
					}
					Singleton<Login>.instance.CheckRecentCharacters(this.m_characters.CharacterList, this.SubRegion);
				}
				Singleton<Login>.instance.LoginUI.SortCharacterList();
			}
		}

		public enum eLoginState
		{
			IDLE,
			WAIT_FOR_ASSET_BUNDLES,
			WEB_AUTH_START,
			WEB_AUTH_LOADING,
			WEB_AUTH_IN_PROGRESS,
			WEB_AUTH_FAILED,
			BN_LOGIN_START,
			BN_LOGIN_WAIT_FOR_LOGON,
			BN_LOGIN_PROVIDE_TOKEN,
			BN_LOGGING_IN,
			BN_ACCOUNT_NAME_WAIT,
			BN_TICKET_WAIT,
			BN_SUBREGION_LIST_WAIT,
			BN_CHARACTER_LIST_WAIT,
			BN_REALM_JOIN_WAIT,
			BN_LOGIN_UNKNOWN,
			MOBILE_CONNECT,
			MOBILE_CONNECTING,
			MOBILE_CONNECT_FAILED,
			MOBILE_DISCONNECTING,
			MOBILE_DISCONNECTED,
			MOBILE_DISCONNECTED_BY_SERVER,
			MOBILE_DISCONNECTED_IDLE,
			MOBILE_LOGGING_IN,
			MOBILE_LOGGED_IN,
			MOBILE_LOGGED_IN_IDLE,
			MOBILE_LOGGED_IN_DATA_COMPLETE,
			UPDATE_REQUIRED_START,
			UPDATE_REQUIRED_IDLE
		}

		private class GameAccountStateCallback
		{
			private bnet.protocol.EntityId m_entityID;

			public bnet.protocol.EntityId EntityID
			{
				get
				{
					return this.m_entityID;
				}
				set
				{
					this.m_entityID = value;
				}
			}

			public GameAccountStateCallback()
			{
			}

			public void Callback(RPCContext context)
			{
				if (Singleton<Login>.instance == null || context == null || context.Payload == null || this.EntityID == null)
				{
					return;
				}
				Singleton<Login>.instance.LoginLog("GameAccountStateCallback called");
				GetGameAccountStateResponse getGameAccountStateResponse = GetGameAccountStateResponse.ParseFrom(context.Payload);
				if (getGameAccountStateResponse == null || getGameAccountStateResponse.State == null || getGameAccountStateResponse.State.GameLevelInfo == null || getGameAccountStateResponse.State.GameStatus == null || getGameAccountStateResponse.State.GameLevelInfo.Name == null)
				{
					return;
				}
				Singleton<Login>.instance.LoginLog(string.Concat(new object[] { "GameAccountStateCallback: Received name ", getGameAccountStateResponse.State.GameLevelInfo.Name, " for game account ", this.EntityID.Low }));
				if (getGameAccountStateResponse.State.GameStatus.IsBanned)
				{
					Singleton<Login>.instance.LoginLog(string.Concat(new object[] { "GameAccountStateCallback: Account ", getGameAccountStateResponse.State.GameLevelInfo.Name, ", (", this.EntityID.Low, ") is Banned!" }));
				}
				if (getGameAccountStateResponse.State.GameStatus.IsSuspended)
				{
					Singleton<Login>.instance.LoginLog(string.Concat(new object[] { "GameAccountStateCallback: Account ", getGameAccountStateResponse.State.GameLevelInfo.Name, ", (", this.EntityID.Low, ") is Suspended!" }));
				}
				Singleton<Login>.instance.AddGameAccountButton(this.EntityID, getGameAccountStateResponse.State.GameLevelInfo.Name, getGameAccountStateResponse.State.GameStatus.IsBanned, getGameAccountStateResponse.State.GameStatus.IsSuspended);
			}
		}

		private class LoginGameAccount
		{
			public bnet.protocol.EntityId gameAccount;

			public string name;

			public bool isBanned;

			public bool isSuspended;

			public LoginGameAccount()
			{
			}
		}

		private class RealmJoinInfo
		{
			private ulong m_realmAddress;

			private string m_ipAddress;

			private byte[] m_joinTicket;

			private byte[] m_joinSecret;

			public string IpAddress
			{
				get
				{
					return this.m_ipAddress;
				}
				set
				{
					this.m_ipAddress = value;
				}
			}

			public byte[] JoinSecret
			{
				get
				{
					return this.m_joinSecret;
				}
				set
				{
					this.m_joinSecret = value;
				}
			}

			public byte[] JoinTicket
			{
				get
				{
					return this.m_joinTicket;
				}
				set
				{
					this.m_joinTicket = value;
				}
			}

			public int Port
			{
				get;
				set;
			}

			public ulong RealmAddress
			{
				get
				{
					return this.m_realmAddress;
				}
				set
				{
					this.m_realmAddress = value;
				}
			}

			public RealmJoinInfo()
			{
			}

			public void ClientResponseCallback(RPCContext context)
			{
				ClientResponse clientResponseFromContext = GamesAPI.GetClientResponseFromContext(context);
				if (clientResponseFromContext == null)
				{
					throw new Exception("ClientResponse was null in RealmJoinCallback");
				}
				foreach (bnet.protocol.attribute.Attribute attributeList in clientResponseFromContext.AttributeList)
				{
					if (attributeList.Name == "Param_RealmJoinTicket")
					{
						this.JoinTicket = attributeList.Value.BlobValue;
					}
					else if (attributeList.Name != "Param_ServerAddresses")
					{
						if (attributeList.Name != "Param_JoinSecret")
						{
							continue;
						}
						this.JoinSecret = attributeList.Value.BlobValue;
						Singleton<Login>.instance.SetJoinSecret(attributeList.Value.BlobValue);
					}
					else
					{
						string str = Login.DecompressJsonAttribBlob(attributeList.Value.BlobValue);
						IEnumerable<JamJSONRealmListServerIPAddress> jamJSONRealmListServerIPAddresses = (
							from family in (IEnumerable<JamJSONRealmListServerIPFamily>)JsonConvert.DeserializeObject<JSONRealmListServerIPAddresses>(str).Families
							where (int)family.Family == (int)((sbyte)MobileBuild.GetSockAddrFamilyIpV4Enum())
							select family).SelectMany<JamJSONRealmListServerIPFamily, JamJSONRealmListServerIPAddress>((JamJSONRealmListServerIPFamily family) => family.Addresses);
						JamJSONRealmListServerIPAddress jamJSONRealmListServerIPAddress = jamJSONRealmListServerIPAddresses.ElementAt<JamJSONRealmListServerIPAddress>(UnityEngine.Random.Range(0, jamJSONRealmListServerIPAddresses.Count<JamJSONRealmListServerIPAddress>()));
						this.IpAddress = jamJSONRealmListServerIPAddress.Ip;
						this.Port = jamJSONRealmListServerIPAddress.Port;
						Singleton<Login>.instance.LoginLog(string.Concat(new object[] { "RealmJoinInfo: Found ip ", this.IpAddress, ", port ", this.Port }));
					}
				}
				this.RealmJoinConnectToMobileServer();
			}

			public void RealmJoinConnectToMobileServer()
			{
				if (this.IpAddress == null || this.IpAddress == string.Empty)
				{
					Singleton<Login>.instance.LoginLog("Couldn't connect to mobile server, ip address was blank.");
					Singleton<Login>.instance.SetLoginState(Login.eLoginState.MOBILE_CONNECT_FAILED);
					Singleton<Login>.instance.LoginUI.ShowGenericPopup(StaticDB.GetString("NETWORK_ERROR", null), StaticDB.GetString("CANT_CONNECT", null));
					return;
				}
				int port = this.Port;
				if (port == 0)
				{
					port = 6012;
				}
				bgs.types.EntityId myAccountId = BattleNet.GetMyAccountId();
				string str = string.Format("BNetAccount-0-{0:X12}", myAccountId.lo);
				string str1 = string.Format("WowAccount-0-{0:X12}", Singleton<Login>.instance.m_gameAccount.Low);
				Singleton<Login>.instance.ConnectToMobileServer(this.IpAddress, port, str, this.RealmAddress, str1, this.JoinTicket, true);
			}
		}
	}
}