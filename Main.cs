using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;
using WowStatConstants;
using WowStaticData;

public class Main : MonoBehaviour
{
	private int m_frameCount;

	private Queue<Main.MobileMessage> m_messageQueue = new Queue<Main.MobileMessage>();

	public Canvas mainCanvas;

	private Animator canvasAnimator;

	public AllPanels allPanels;

	public AllPopups allPopups;

	private string m_unknownMsg;

	public UISound m_UISound;

	public bool m_enableNotifications;

	public static Main instance;

	private GuildChatSlider m_chatPopup;

	public Action FollowerDataChangedAction;

	public Action<int> MissionSuccessChanceChangedAction;

	public Action GarrisonDataResetStartedAction;

	public Action GarrisonDataResetFinishedAction;

	public Action ShipmentTypesUpdatedAction;

	public Action<int> CreateShipmentResultAction;

	public Action<int, ulong> ShipmentAddedAction;

	public Action<SHIPMENT_RESULT, ulong> CompleteShipmentResultAction;

	public Action<int, int> MissionAddedAction;

	public Action<JamGarrisonFollower, JamGarrisonFollower> FollowerChangedXPAction;

	public Action<JamGarrisonFollower> TroopExpiredAction;

	public Action<int, int, string> CanResearchGarrisonTalentResultAction;

	public Action<int, int> ResearchGarrisonTalentResultAction;

	public Action BountyInfoUpdatedAction;

	public Action<int> UseEquipmentStartAction;

	public Action<JamGarrisonFollower, JamGarrisonFollower> UseEquipmentResultAction;

	public Action EquipmentInventoryChangedAction;

	public Action<int> UseArmamentStartAction;

	public Action<int, JamGarrisonFollower, JamGarrisonFollower> UseArmamentResultAction;

	public Action ArmamentInventoryChangedAction;

	public Action<OrderHallNavButton> OrderHallNavButtonSelectedAction;

	public Action<OrderHallFilterOptionsButton> OrderHallfilterOptionsButtonSelectedAction;

	public Action<int, MobileClientShipmentItem> ShipmentItemPushedAction;

	public Action<int> PlayerLeveledUpAction;

	public Action MakeContributionRequestInitiatedAction;

	public Action ContributionInfoChangedAction;

	public Action InvasionPOIChangedAction;

	public Action ArtifactKnowledgeInfoChangedAction;

	public Action MaxActiveFollowersChangedAction;

	public Action<int, int, int> GotItemFromQuestCompletionAction;

	public GameObject m_debugButton;

	public Text m_debugText;

	private string m_locale;

	public GameClient m_gameClientScript;

	public static string uniqueIdentifier;

	public CanvasBlurManager m_canvasBlurManager;

	public BackButtonManager m_backButtonManager;

	public Main()
	{
	}

	public void AddMissionCheat(int garrMissionID)
	{
		MobilePlayerAddMissionCheat mobilePlayerAddMissionCheat = new MobilePlayerAddMissionCheat()
		{
			GarrMissionID = garrMissionID
		};
		Login.instance.SendToMobileServer(mobilePlayerAddMissionCheat);
	}

	public void AdvanceMissionSetCheat()
	{
		MobilePlayerGarrisonAdvanceMissionSet mobilePlayerGarrisonAdvanceMissionSet = new MobilePlayerGarrisonAdvanceMissionSet()
		{
			GarrTypeID = 3,
			MissionSetID = 0
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonAdvanceMissionSet);
	}

	public void AllTalentsCheat()
	{
		MobilePlayerGarrisonCompleteAllTalentsCheat mobilePlayerGarrisonCompleteAllTalentsCheat = new MobilePlayerGarrisonCompleteAllTalentsCheat();
		Login.instance.SendToMobileServer(mobilePlayerGarrisonCompleteAllTalentsCheat);
		MobilePlayerGarrisonDataRequest mobilePlayerGarrisonDataRequest = new MobilePlayerGarrisonDataRequest()
		{
			GarrTypeID = 3
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonDataRequest);
	}

	public void AllTheMissions()
	{
		StaticDB.garrMissionDB.EnumRecords((GarrMissionRec missionRec) => {
			if (missionRec.GarrTypeID == 3)
			{
				this.AddMissionCheat(missionRec.ID);
			}
			return true;
		});
	}

	private void Awake()
	{
		Main.instance = this;
		this.m_enableNotifications = true;
		this.GenerateUniqueIdentifier();
		this.canvasAnimator = this.mainCanvas.GetComponent<Animator>();
		this.allPanels.ShowConnectingPanel();
		AllPanels.instance.ShowConnectingPanelCancelButton(false);
		AllPanels.instance.SetConnectingPanelStatus(string.Empty);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void CheatFastForwardOneHour()
	{
		GarrisonStatus.CheatFastForwardOneHour();
	}

	public void ClaimMissionBonus(int garrMissionID)
	{
		Debug.Log(string.Concat(new object[] { "Main.ClaimMissionBonus() ", garrMissionID, " State is: ", ((JamGarrisonMobileMission)PersistentMissionData.missionDictionary[garrMissionID]).MissionState }));
		MobilePlayerClaimMissionBonus mobilePlayerClaimMissionBonu = new MobilePlayerClaimMissionBonus()
		{
			GarrMissionID = garrMissionID
		};
		Login.instance.SendToMobileServer(mobilePlayerClaimMissionBonu);
	}

	private void ClearPendingNotifications()
	{
		LocalNotifications.ClearPending();
	}

	public void CompleteAllMissions()
	{
		Debug.Log("Main.CompleteAllMissions()");
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonMobileMission current = (JamGarrisonMobileMission)enumerator.Current;
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(current.MissionRecID);
				if (record != null && record.GarrFollowerTypeID == 4)
				{
					if (current.MissionState != 1)
					{
						continue;
					}
					long num = GarrisonStatus.CurrentTime() - current.StartTime;
					if (current.MissionDuration - num > (long)0)
					{
						continue;
					}
					this.CompleteMission(current.MissionRecID);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
	}

	public void CompleteMission(int garrMissionID)
	{
		Debug.Log(string.Concat("Main.CompleteMission() ", garrMissionID));
		MobilePlayerGarrisonCompleteMission mobilePlayerGarrisonCompleteMission = new MobilePlayerGarrisonCompleteMission()
		{
			GarrMissionID = garrMissionID
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonCompleteMission);
	}

	public void ExpediteMissionCheat(int garrMissionID)
	{
		MobilePlayerExpediteMissionCheat mobilePlayerExpediteMissionCheat = new MobilePlayerExpediteMissionCheat()
		{
			GarrMissionID = garrMissionID
		};
		Login.instance.SendToMobileServer(mobilePlayerExpediteMissionCheat);
	}

	public void FastMissionsCheat()
	{
		MobilePlayerSetMissionDurationCheat mobilePlayerSetMissionDurationCheat = new MobilePlayerSetMissionDurationCheat()
		{
			Seconds = 10
		};
		Login.instance.SendToMobileServer(mobilePlayerSetMissionDurationCheat);
	}

	public void FastShipmentsCheat()
	{
		MobilePlayerSetShipmentDurationCheat mobilePlayerSetShipmentDurationCheat = new MobilePlayerSetShipmentDurationCheat()
		{
			Seconds = 10
		};
		Login.instance.SendToMobileServer(mobilePlayerSetShipmentDurationCheat);
	}

	private void GenerateUniqueIdentifier()
	{
		bool flag = false;
		string empty = string.Empty;
		AndroidJavaObject @static = (new AndroidJavaClass("com.unity3d.player.UnityPlayer")).GetStatic<AndroidJavaObject>("currentActivity");
		string str = (new AndroidJavaClass("android.content.Context")).GetStatic<string>("TELEPHONY_SERVICE");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getSystemService", new object[] { str });
		bool flag1 = false;
		try
		{
			empty = androidJavaObject.Call<string>("getDeviceId", new object[0]);
		}
		catch (Exception exception)
		{
			Debug.Log(exception.ToString());
			flag1 = true;
		}
		if (empty == null)
		{
			empty = string.Empty;
		}
		if (flag1 && !flag || !flag1 && empty == string.Empty && flag)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.provider.Settings$Secure");
			string static1 = androidJavaClass.GetStatic<string>("ANDROID_ID");
			AndroidJavaObject androidJavaObject1 = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
			empty = androidJavaClass.CallStatic<string>("getString", new object[] { androidJavaObject1, static1 }) ?? string.Empty;
		}
		if (empty == string.Empty)
		{
			string str1 = "00000000000000000000000000000000";
			try
			{
				StreamReader streamReader = new StreamReader("/sys/class/net/wlan0/address");
				str1 = streamReader.ReadLine();
				streamReader.Close();
			}
			catch (Exception exception1)
			{
				Debug.Log(exception1.ToString());
			}
			empty = str1.Replace(":", string.Empty);
		}
		Main.uniqueIdentifier = Main.getMd5Hash(empty);
	}

	public string GetLocale()
	{
		if (this.m_locale == null || this.m_locale == string.Empty)
		{
			this.m_locale = MobileDeviceLocale.GetBestGuessForLocale();
		}
		return this.m_locale;
	}

	private static string getMd5Hash(string input)
	{
		if (input == string.Empty)
		{
			return string.Empty;
		}
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] numArray = mD5CryptoServiceProvider.ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < (int)numArray.Length; i++)
		{
			stringBuilder.Append(numArray[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private void HandleEnterWorld()
	{
		Main.instance.GarrisonDataResetFinishedAction -= new Action(this.HandleEnterWorld);
		AllPanels.instance.m_troopsPanel.PurgeList();
		AllPanels.instance.ShowAdventureMap();
		AllPanels.instance.m_orderHallMultiPanel.m_troopsPanel.HandleOrderHallNavButtonSelected(null);
		AllPanels.instance.m_orderHallMultiPanel.m_adventureMapPanel.CenterAndZoom(Vector2.zero, null, false);
		AllPanels.instance.m_orderHallMultiPanel.SelectDefaultNavButton();
		this.PrecacheMissionChances();
		MobilePlayerRequestAreaPoiInfo mobilePlayerRequestAreaPoiInfo = new MobilePlayerRequestAreaPoiInfo();
		Login.instance.SendToMobileServer(mobilePlayerRequestAreaPoiInfo);
	}

	public void HideDebugText()
	{
		this.m_debugButton.SetActive(false);
	}

	public void Logout()
	{
		MissionDataCache.ClearData();
		AllPopups.instance.HideAllPopups();
		AllPanels.instance.ShowOrderHallMultiPanel(false);
		Login.instance.ReconnectToMobileServerCharacterSelect();
	}

	private void MobileClientAdvanceMissionSetResultHandler(MobileClientAdvanceMissionSetResult msg)
	{
		Debug.Log(string.Concat(new object[] { "Advance mission set ", msg.MissionSetID, " success: ", msg.Success }));
	}

	private void MobileClientArtifactInfoResultHandler(MobileClientArtifactInfoResult msg)
	{
		GarrisonStatus.ArtifactKnowledgeLevel = msg.KnowledgeLevel;
		GarrisonStatus.ArtifactXpMultiplier = msg.XpMultiplier;
	}

	private void MobileClientArtifactKnowledgeInfoResultHandler(MobileClientArtifactKnowledgeInfoResult msg)
	{
		ArtifactKnowledgeData.ClearData();
		ArtifactKnowledgeData.SetArtifactKnowledgeInfo(msg);
		if (this.ArtifactKnowledgeInfoChangedAction != null)
		{
			this.ArtifactKnowledgeInfoChangedAction();
		}
	}

	private void MobileClientAuthChallengeHandler(MobileClientAuthChallenge msg)
	{
		Login.instance.MobileAuthChallengeReceived(msg.ServerChallenge);
	}

	private void MobileClientBountiesByWorldQuestUpdateHandler(MobileClientBountiesByWorldQuestUpdate msg)
	{
		MobileBountiesByWorldQuest[] quest = msg.Quest;
		for (int i = 0; i < (int)quest.Length; i++)
		{
			PersistentBountyData.AddOrUpdateBountiesByWorldQuest(quest[i]);
		}
		if (AdventureMapPanel.instance != null)
		{
			AdventureMapPanel.instance.UpdateWorldQuests();
		}
	}

	private void MobileClientCanResearchGarrisonTalentResultHandler(MobileClientCanResearchGarrisonTalentResult msg)
	{
		if (this.CanResearchGarrisonTalentResultAction != null)
		{
			this.CanResearchGarrisonTalentResultAction(msg.GarrTalentID, msg.Result, msg.ConditionText);
		}
	}

	private void MobileClientChangeFollowerActiveResultHandler(MobileClientChangeFollowerActiveResult msg)
	{
		GARRISON_RESULT result = (GARRISON_RESULT)msg.Result;
		if (result != GARRISON_RESULT.SUCCESS)
		{
			Debug.Log(string.Concat("Follower activation/deactivation failed for reason ", result.ToString()));
		}
		else
		{
			PersistentFollowerData.AddOrUpdateFollower(msg.Follower);
			if (GeneralHelpers.GetFollowerStatus(msg.Follower) != FollowerStatus.inactive)
			{
				Debug.Log(string.Concat("Follower is now active. ", msg.ActivationsRemaining, " activations remain for the day."));
			}
			else
			{
				Debug.Log(string.Concat("Follower is now inactive. ", msg.ActivationsRemaining, " activations remain for the day."));
			}
			if (this.FollowerDataChangedAction != null)
			{
				this.FollowerDataChangedAction();
			}
			MobilePlayerFollowerActivationDataRequest mobilePlayerFollowerActivationDataRequest = new MobilePlayerFollowerActivationDataRequest()
			{
				GarrTypeID = 3
			};
			Login.instance.SendToMobileServer(mobilePlayerFollowerActivationDataRequest);
		}
	}

	private void MobileClientChatHandler(MobileClientChat msg, int count)
	{
		if (msg.SlashCmd == 4)
		{
			this.m_chatPopup.OnReceiveText(msg.SenderName, msg.ChatText);
		}
		Debug.Log(string.Concat(new object[] { "Count: ", count, ", Chat type ", (GuildChatSlider.SLASH_CMD)msg.SlashCmd, " from ", msg.SenderName, ": ", msg.ChatText }));
	}

	private void MobileClientClaimMissionBonusResultHandler(MobileClientClaimMissionBonusResult msg)
	{
		PersistentMissionData.UpdateMission(msg.Mission);
		AdventureMapMissionSite[] componentsInChildren = AdventureMapPanel.instance.m_missionAndWordQuestArea.GetComponentsInChildren<AdventureMapMissionSite>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			AdventureMapMissionSite adventureMapMissionSite = componentsInChildren[i];
			if (!adventureMapMissionSite.m_isStackablePreview)
			{
				if (adventureMapMissionSite.GetGarrMissionID() == msg.GarrMissionID)
				{
					if (!adventureMapMissionSite.gameObject.activeSelf)
					{
						adventureMapMissionSite.gameObject.SetActive(true);
					}
					adventureMapMissionSite.HandleClaimMissionBonusResult(msg.GarrMissionID, msg.AwardOvermax, msg.Result);
					break;
				}
			}
		}
	}

	private void MobileClientCompleteMissionResultHandler(MobileClientCompleteMissionResult msg)
	{
		Debug.Log(string.Concat(new object[] { "CompleteMissionResult: ID=", msg.GarrMissionID, ", result=", msg.Result, " success chance was ", msg.MissionSuccessChance }));
		PersistentMissionData.UpdateMission(msg.Mission);
		AdventureMapMissionSite[] componentsInChildren = AdventureMapPanel.instance.m_missionAndWordQuestArea.GetComponentsInChildren<AdventureMapMissionSite>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			AdventureMapMissionSite adventureMapMissionSite = componentsInChildren[i];
			if (!adventureMapMissionSite.m_isStackablePreview)
			{
				if (adventureMapMissionSite.GetGarrMissionID() == msg.GarrMissionID)
				{
					if (!adventureMapMissionSite.gameObject.activeSelf)
					{
						adventureMapMissionSite.gameObject.SetActive(true);
					}
					adventureMapMissionSite.HandleCompleteMissionResult(msg.GarrMissionID, msg.BonusRollSucceeded);
					break;
				}
			}
		}
		MobilePlayerRequestShipmentTypes mobilePlayerRequestShipmentType = new MobilePlayerRequestShipmentTypes();
		Login.instance.SendToMobileServer(mobilePlayerRequestShipmentType);
		MobilePlayerRequestShipments mobilePlayerRequestShipment = new MobilePlayerRequestShipments();
		Login.instance.SendToMobileServer(mobilePlayerRequestShipment);
		MobilePlayerFollowerEquipmentRequest mobilePlayerFollowerEquipmentRequest = new MobilePlayerFollowerEquipmentRequest()
		{
			GarrFollowerTypeID = 4
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerEquipmentRequest);
		MobilePlayerGarrisonDataRequest mobilePlayerGarrisonDataRequest = new MobilePlayerGarrisonDataRequest()
		{
			GarrTypeID = 3
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonDataRequest);
	}

	private void MobileClientCompleteShipmentResultHandler(MobileClientCompleteShipmentResult msg)
	{
		SHIPMENT_RESULT result = (SHIPMENT_RESULT)msg.Result;
		if (this.CompleteShipmentResultAction != null)
		{
			this.CompleteShipmentResultAction(result, msg.ShipmentID);
		}
	}

	private void MobileClientConnectResultHandler(MobileClientConnectResult msg)
	{
		Login.instance.MobileConnectResult(msg);
	}

	private void MobileClientCreateShipmentResultHandler(MobileClientCreateShipmentResult msg)
	{
		GARRISON_RESULT result = (GARRISON_RESULT)msg.Result;
		if (result != GARRISON_RESULT.SUCCESS)
		{
			AllPopups.instance.ShowGenericPopup(StaticDB.GetString("SHIPMENT_CREATION_FAILED", null), result.ToString());
		}
		if (this.CreateShipmentResultAction != null)
		{
			this.CreateShipmentResultAction(msg.Result);
		}
		if (result == GARRISON_RESULT.SUCCESS)
		{
			MobilePlayerRequestShipmentTypes mobilePlayerRequestShipmentType = new MobilePlayerRequestShipmentTypes();
			Login.instance.SendToMobileServer(mobilePlayerRequestShipmentType);
			MobilePlayerRequestShipments mobilePlayerRequestShipment = new MobilePlayerRequestShipments();
			Login.instance.SendToMobileServer(mobilePlayerRequestShipment);
			MobilePlayerGarrisonDataRequest mobilePlayerGarrisonDataRequest = new MobilePlayerGarrisonDataRequest()
			{
				GarrTypeID = 3
			};
			Login.instance.SendToMobileServer(mobilePlayerGarrisonDataRequest);
		}
	}

	private void MobileClientEmissaryFactionUpdateHandler(MobileClientEmissaryFactionUpdate msg)
	{
		this.allPopups.EmissaryFactionUpdate(msg);
	}

	private void MobileClientEvaluateMissionResultHandler(MobileClientEvaluateMissionResult msg)
	{
		if (msg.Result != 0)
		{
			GARRISON_RESULT result = (GARRISON_RESULT)msg.Result;
			Debug.Log(string.Concat("MobileClientEvaluateMissionResult failed with error ", result.ToString()));
		}
		else
		{
			MissionDataCache.AddOrUpdateMissionData(msg.GarrMissionID, msg.SuccessChance);
			if (this.MissionSuccessChanceChangedAction != null)
			{
				this.MissionSuccessChanceChangedAction(msg.SuccessChance);
			}
		}
	}

	private void MobileClientExpediteMissionCheatResultHandler(MobileClientExpediteMissionCheatResult msg)
	{
		if (msg.Result != 0)
		{
			Debug.Log(string.Concat(new object[] { "MobileClientExpediteMissionCheatResult: Mission ID ", msg.MissionRecID, " failed with error ", msg.Result }));
		}
		else
		{
			Debug.Log(string.Concat("Expedited completion of mission ", msg.MissionRecID));
			MobilePlayerGarrisonDataRequest mobilePlayerGarrisonDataRequest = new MobilePlayerGarrisonDataRequest()
			{
				GarrTypeID = 3
			};
			Login.instance.SendToMobileServer(mobilePlayerGarrisonDataRequest);
		}
	}

	private void MobileClientFollowerActivationDataResultHandler(MobileClientFollowerActivationDataResult msg)
	{
		GarrisonStatus.SetFollowerActivationInfo(msg.ActivationsRemaining, msg.GoldCost);
	}

	private void MobileClientFollowerArmamentsExtendedResultHandler(MobileClientFollowerArmamentsExtendedResult msg)
	{
		unsafe
		{
			PersistentArmamentData.ClearData();
			for (uint i = 0; (ulong)i < (long)((int)msg.Armament.Length); i++)
			{
				PersistentArmamentData.AddOrUpdateArmament(msg.Armament[i]);
			}
			if (this.ArmamentInventoryChangedAction != null)
			{
				this.ArmamentInventoryChangedAction();
			}
		}
	}

	private void MobileClientFollowerArmamentsResultHandler(MobileClientFollowerArmamentsResult msg)
	{
	}

	private void MobileClientFollowerChangedQualityHandler(MobileClientFollowerChangedQuality msg)
	{
		PersistentFollowerData.AddOrUpdateFollower(msg.Follower);
		if (this.UseEquipmentResultAction != null)
		{
			this.UseEquipmentResultAction(msg.OldFollower, msg.Follower);
		}
		MobilePlayerFollowerEquipmentRequest mobilePlayerFollowerEquipmentRequest = new MobilePlayerFollowerEquipmentRequest()
		{
			GarrFollowerTypeID = 4
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerEquipmentRequest);
	}

	private void MobileClientFollowerChangedXPHandler(MobileClientFollowerChangedXP msg)
	{
		Debug.Log(string.Concat(new object[] { "MobileClientFollowerChangedXPHandler: follower ", msg.Follower.GarrFollowerID, " xp changed by ", msg.XpChange }));
		if (this.FollowerChangedXPAction != null)
		{
			this.FollowerChangedXPAction(msg.OldFollower, msg.Follower);
		}
	}

	private void MobileClientFollowerEquipmentResultHandler(MobileClientFollowerEquipmentResult msg)
	{
		unsafe
		{
			PersistentEquipmentData.ClearData();
			for (uint i = 0; (ulong)i < (long)((int)msg.Equipment.Length); i++)
			{
				PersistentEquipmentData.AddOrUpdateEquipment(msg.Equipment[i]);
			}
			if (this.EquipmentInventoryChangedAction != null)
			{
				this.EquipmentInventoryChangedAction();
			}
		}
	}

	private void MobileClientGarrisonDataRequestResultHandler(MobileClientGarrisonDataRequestResult msg)
	{
		unsafe
		{
			PersistentFollowerData.ClearData();
			PersistentMissionData.ClearData();
			PersistentTalentData.ClearData();
			if (this.GarrisonDataResetStartedAction != null)
			{
				this.GarrisonDataResetStartedAction();
			}
			GarrisonStatus.SetFaction(msg.PvpFaction);
			GarrisonStatus.SetGarrisonServerConnectTime(msg.ServerTime);
			GarrisonStatus.SetCurrencies(msg.GoldCurrency, msg.OilCurrency, msg.OrderhallResourcesCurrency);
			GarrisonStatus.SetCharacterName(msg.CharacterName);
			GarrisonStatus.SetCharacterLevel(msg.CharacterLevel);
			GarrisonStatus.SetCharacterClass(msg.CharacterClassID);
			for (uint i = 0; (ulong)i < (long)msg.Follower.GetLength(0); i++)
			{
				JamGarrisonFollower follower = msg.Follower[i];
				if (StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID) != null)
				{
					PersistentFollowerData.AddOrUpdateFollower(follower);
					if ((follower.Flags & 8) != 0 && follower.Durability <= 0)
					{
						Debug.Log(string.Concat("Follower ", follower.GarrFollowerID, " has expired."));
						if (this.TroopExpiredAction != null)
						{
							this.TroopExpiredAction(follower);
						}
					}
				}
			}
			for (uint j = 0; (ulong)j < (long)msg.Mission.GetLength(0); j++)
			{
				PersistentMissionData.AddMission(msg.Mission[j]);
			}
			for (int k = 0; k < msg.Talent.GetLength(0); k++)
			{
				PersistentTalentData.AddOrUpdateTalent(msg.Talent[k]);
			}
			if (this.GarrisonDataResetFinishedAction != null)
			{
				this.GarrisonDataResetFinishedAction();
			}
			if (this.FollowerDataChangedAction != null)
			{
				this.FollowerDataChangedAction();
			}
		}
	}

	private void MobileClientGetItemTooltipInfoResultHandler(MobileClientGetItemTooltipInfoResult msg)
	{
		ItemStatCache.instance.AddMobileItemStats(msg.ItemID, msg.ItemContext, msg.Stats);
	}

	private void MobileClientGuildMemberLoggedInHandler(MobileClientGuildMemberLoggedIn msg)
	{
		Debug.Log(string.Concat(new string[] { "Guild member ", msg.Member.Name, " (", msg.Member.Guid, ") logged in." }));
		GuildData.AddGuildMember(msg.Member);
	}

	private void MobileClientGuildMemberLoggedOutHandler(MobileClientGuildMemberLoggedOut msg)
	{
		Debug.Log(string.Concat(new string[] { "Guild member ", msg.Member.Name, " (", msg.Member.Guid, ") logged out." }));
		GuildData.RemoveGuildMember(msg.Member.Guid);
	}

	private void MobileClientGuildMembersOnlineHandler(MobileClientGuildMembersOnline msg)
	{
		IEnumerator<MobileGuildMember> enumerator = msg.Members.AsEnumerable<MobileGuildMember>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileGuildMember current = enumerator.Current;
				Debug.Log(string.Concat(new string[] { "Guild member ", current.Name, " (", current.Guid, ") is online." }));
				GuildData.AddGuildMember(current);
			}
		}
		finally
		{
			if (enumerator == null)
			{
			}
			enumerator.Dispose();
		}
	}

	private void MobileClientLoginResultHandler(MobileClientLoginResult msg)
	{
		Login.instance.MobileLoginResult(msg.Success, msg.Version);
	}

	private void MobileClientMakeContributionResultHandler(MobileClientMakeContributionResult msg)
	{
		Debug.Log(string.Concat(new object[] { "Make Contribution Result for ID ", msg.ContributionID, " is ", msg.Result }));
		MobilePlayerRequestContributionInfo mobilePlayerRequestContributionInfo = new MobilePlayerRequestContributionInfo();
		Login.instance.SendToMobileServer(mobilePlayerRequestContributionInfo);
		if (this.ContributionInfoChangedAction != null)
		{
			this.ContributionInfoChangedAction();
		}
	}

	private void MobileClientMissionAddedHandler(MobileClientMissionAdded msg)
	{
		if (msg.Result != 0 || msg.Mission.MissionRecID == 0)
		{
			GARRISON_RESULT result = (GARRISON_RESULT)msg.Result;
			Debug.Log(string.Concat(new object[] { "Error adding mission: ", result.ToString(), " Mission ID:", msg.Mission.MissionRecID }));
		}
		else
		{
			PersistentMissionData.AddMission(msg.Mission);
		}
		if (this.MissionAddedAction != null)
		{
			this.MissionAddedAction(msg.Mission.MissionRecID, msg.Result);
		}
	}

	private void MobileClientPlayerLevelUpHandler(MobileClientPlayerLevelUp msg)
	{
		Debug.Log(string.Concat("Congrats, your character is now level ", msg.NewLevel));
		AllPopups.instance.ShowLevelUpToast(msg.NewLevel);
		if (this.PlayerLeveledUpAction != null)
		{
			this.PlayerLeveledUpAction(msg.NewLevel);
		}
	}

	private void MobileClientPongHandler(MobileClientPong msg)
	{
		Login.instance.PongReceived();
	}

	private void MobileClientQuestCompletedHandler(MobileClientQuestCompleted msg)
	{
		MobileQuestItem[] item = msg.Item;
		for (int i = 0; i < (int)item.Length; i++)
		{
			MobileQuestItem mobileQuestItem = item[i];
			if (this.GotItemFromQuestCompletionAction != null)
			{
				this.GotItemFromQuestCompletionAction(mobileQuestItem.ItemID, mobileQuestItem.Quantity, msg.QuestID);
			}
		}
	}

	private void MobileClientRequestAreaPoiInfoResultHandler(MobileClientRequestAreaPoiInfoResult msg)
	{
		LegionfallData.SetCurrentInvasionPOI(null);
		if (msg.PoiData != null && (int)msg.PoiData.Length > 0)
		{
			LegionfallData.SetCurrentInvasionPOI(msg.PoiData[0]);
			LegionfallData.SetCurrentInvasionExpirationTime(msg.PoiData[0].TimeRemaining);
		}
		if (this.InvasionPOIChangedAction != null)
		{
			this.InvasionPOIChangedAction();
		}
	}

	private void MobileClientRequestContributionInfoResultHandler(MobileClientRequestContributionInfoResult msg)
	{
		LegionfallData.ClearData();
		LegionfallData.SetLegionfallWarResources(msg.LegionfallWarResources);
		LegionfallData.SetHasAccess(msg.HasAccess);
		MobileContribution[] contribution = msg.Contribution;
		for (int i = 0; i < (int)contribution.Length; i++)
		{
			LegionfallData.AddOrUpdateLegionfallBuilding(contribution[i]);
		}
		if (this.ContributionInfoChangedAction != null)
		{
			this.ContributionInfoChangedAction();
		}
	}

	private void MobileClientRequestMaxFollowersResultHandler(MobileClientRequestMaxFollowersResult msg)
	{
		GarrisonStatus.SetMaxActiveFollowers(msg.MaxFollowers);
		if (this.MaxActiveFollowersChangedAction != null)
		{
			this.MaxActiveFollowersChangedAction();
		}
	}

	private void MobileClientResearchGarrisonTalentResultHandler(MobileClientResearchGarrisonTalentResult msg)
	{
		GARRISON_RESULT result = (GARRISON_RESULT)msg.Result;
		if (result != GARRISON_RESULT.SUCCESS)
		{
			AllPopups.instance.ShowGenericPopup(StaticDB.GetString("TALENT_RESEARCH_FAILED", null), result.ToString());
		}
		if (this.ResearchGarrisonTalentResultAction != null)
		{
			this.ResearchGarrisonTalentResultAction(msg.GarrTalentID, msg.Result);
		}
	}

	private void MobileClientSetMissionDurationCheatResultHandler(MobileClientSetMissionDurationCheatResult msg)
	{
		AllPopups.instance.HideAllPopups();
	}

	private void MobileClientSetShipmentDurationCheatResultHandler(MobileClientSetShipmentDurationCheatResult msg)
	{
		AllPopups.instance.HideAllPopups();
	}

	private void MobileClientShipmentPushResultHandler(MobileClientShipmentPushResult msg)
	{
		MobileClientShipmentItem[] items = msg.Items;
		for (int i = 0; i < (int)items.Length; i++)
		{
			MobileClientShipmentItem mobileClientShipmentItem = items[i];
			if (this.ShipmentItemPushedAction != null)
			{
				this.ShipmentItemPushedAction(msg.CharShipmentID, mobileClientShipmentItem);
			}
		}
	}

	private void MobileClientShipmentsUpdateHandler(MobileClientShipmentsUpdate msg)
	{
		PersistentShipmentData.ClearData();
		JamCharacterShipment[] shipment = msg.Shipment;
		for (int i = 0; i < (int)shipment.Length; i++)
		{
			JamCharacterShipment jamCharacterShipment = shipment[i];
			PersistentShipmentData.AddOrUpdateShipment(jamCharacterShipment);
			if (this.ShipmentAddedAction != null)
			{
				this.ShipmentAddedAction(jamCharacterShipment.ShipmentRecID, jamCharacterShipment.ShipmentID);
			}
		}
	}

	private void MobileClientShipmentTypesHandler(MobileClientShipmentTypes msg)
	{
		PersistentShipmentData.SetAvailableShipmentTypes(msg.Shipment);
		if (this.ShipmentTypesUpdatedAction != null)
		{
			this.ShipmentTypesUpdatedAction();
		}
	}

	private void MobileClientStartMissionResultHandler(MobileClientStartMissionResult msg)
	{
		Debug.Log(string.Concat(new object[] { "StartMissionResult: ID=", msg.GarrMissionID, ", result=", msg.Result }));
		if (msg.Result != 0)
		{
			GARRISON_RESULT result = (GARRISON_RESULT)msg.Result;
			string str = result.ToString();
			if (result == GARRISON_RESULT.FOLLOWER_SOFT_CAP_EXCEEDED)
			{
				str = StaticDB.GetString("TOO_MANY_ACTIVE_CHAMPIONS", null);
				AllPopups.instance.ShowGenericPopup(StaticDB.GetString("MISSION_START_FAILED", null), str);
			}
			else if (result != GARRISON_RESULT.MISSION_MISSING_REQUIRED_FOLLOWER)
			{
				AllPopups.instance.ShowGenericPopupFull(StaticDB.GetString("MISSION_START_FAILED", null));
				Debug.Log(string.Concat("Mission start result: ", str));
			}
			else
			{
				str = StaticDB.GetString("MISSING_REQUIRED_FOLLOWER", null);
				AllPopups.instance.ShowGenericPopup(StaticDB.GetString("MISSION_START_FAILED", null), str);
			}
		}
		this.MobileRequestData();
	}

	private void MobileClientUseFollowerArmamentResultHandler(MobileClientUseFollowerArmamentResult msg)
	{
		if (msg.Result != (int)GARRISON_RESULT.SUCCESS)
		{
			AllPopups.instance.ShowGenericPopupFull(StaticDB.GetString("USE_ARMAMENT_FAILED", null));
		}
		else
		{
			PersistentFollowerData.AddOrUpdateFollower(msg.Follower);
			MobilePlayerFollowerArmamentsExtendedRequest mobilePlayerFollowerArmamentsExtendedRequest = new MobilePlayerFollowerArmamentsExtendedRequest()
			{
				GarrFollowerTypeID = 4
			};
			Login.instance.SendToMobileServer(mobilePlayerFollowerArmamentsExtendedRequest);
		}
		if (this.UseArmamentResultAction != null)
		{
			this.UseArmamentResultAction(msg.Result, msg.OldFollower, msg.Follower);
		}
	}

	private void MobileClientWorldQuestBountiesResultHandler(MobileClientWorldQuestBountiesResult msg)
	{
		unsafe
		{
			PersistentBountyData.ClearData();
			PersistentBountyData.SetBountiesVisible(msg.Visible);
			if (msg.Visible)
			{
			}
			msg.LockedQuestID == 0;
			for (uint i = 0; (ulong)i < (long)((int)msg.Bounty.Length); i++)
			{
				PersistentBountyData.AddOrUpdateBounty(msg.Bounty[i]);
			}
			if (this.BountyInfoUpdatedAction != null)
			{
				this.BountyInfoUpdatedAction();
			}
		}
	}

	private void MobileClientWorldQuestInactiveBountiesResultHandler(MobileClientWorldQuestInactiveBountiesResult msg)
	{
	}

	private void MobileClientWorldQuestUpdateHandler(MobileClientWorldQuestUpdate msg)
	{
		WorldQuestData.ClearData();
		MobileWorldQuest[] quest = msg.Quest;
		for (int i = 0; i < (int)quest.Length; i++)
		{
			MobileWorldQuest mobileWorldQuest = quest[i];
			if (mobileWorldQuest.StartLocationMapID == 1220)
			{
				WorldQuestData.AddWorldQuest(mobileWorldQuest);
				for (int j = 0; j < mobileWorldQuest.Item.Count<MobileWorldQuestReward>(); j++)
				{
					ItemStatCache.instance.GetItemStats(mobileWorldQuest.Item[j].RecordID, mobileWorldQuest.Item[j].ItemContext);
				}
			}
		}
	}

	public void MobileLoggedIn()
	{
		Main.instance.GarrisonDataResetFinishedAction += new Action(this.HandleEnterWorld);
		PersistentArmamentData.ClearData();
		PersistentBountyData.ClearData();
		PersistentEquipmentData.ClearData();
		PersistentFollowerData.ClearData();
		PersistentFollowerData.ClearPreMissionFollowerData();
		PersistentMissionData.ClearData();
		PersistentShipmentData.ClearData();
		PersistentTalentData.ClearData();
		GuildData.ClearData();
		MissionDataCache.ClearData();
		WorldQuestData.ClearData();
		ItemStatCache.instance.ClearItemStats();
		this.MobileRequestData();
	}

	public void MobileRequestData()
	{
		MobilePlayerRequestShipmentTypes mobilePlayerRequestShipmentType = new MobilePlayerRequestShipmentTypes();
		Login.instance.SendToMobileServer(mobilePlayerRequestShipmentType);
		MobilePlayerRequestShipments mobilePlayerRequestShipment = new MobilePlayerRequestShipments();
		Login.instance.SendToMobileServer(mobilePlayerRequestShipment);
		MobilePlayerWorldQuestBountiesRequest mobilePlayerWorldQuestBountiesRequest = new MobilePlayerWorldQuestBountiesRequest();
		Login.instance.SendToMobileServer(mobilePlayerWorldQuestBountiesRequest);
		this.RequestWorldQuests();
		MobilePlayerFollowerEquipmentRequest mobilePlayerFollowerEquipmentRequest = new MobilePlayerFollowerEquipmentRequest()
		{
			GarrFollowerTypeID = 4
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerEquipmentRequest);
		MobilePlayerFollowerArmamentsExtendedRequest mobilePlayerFollowerArmamentsExtendedRequest = new MobilePlayerFollowerArmamentsExtendedRequest()
		{
			GarrFollowerTypeID = 4
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerArmamentsExtendedRequest);
		MobilePlayerFollowerActivationDataRequest mobilePlayerFollowerActivationDataRequest = new MobilePlayerFollowerActivationDataRequest()
		{
			GarrTypeID = 3
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerActivationDataRequest);
		MobilePlayerGetArtifactInfo mobilePlayerGetArtifactInfo = new MobilePlayerGetArtifactInfo();
		Login.instance.SendToMobileServer(mobilePlayerGetArtifactInfo);
		MobilePlayerRequestContributionInfo mobilePlayerRequestContributionInfo = new MobilePlayerRequestContributionInfo();
		Login.instance.SendToMobileServer(mobilePlayerRequestContributionInfo);
		MobilePlayerRequestAreaPoiInfo mobilePlayerRequestAreaPoiInfo = new MobilePlayerRequestAreaPoiInfo();
		Login.instance.SendToMobileServer(mobilePlayerRequestAreaPoiInfo);
		MobilePlayerRequestArtifactKnowledgeInfo mobilePlayerRequestArtifactKnowledgeInfo = new MobilePlayerRequestArtifactKnowledgeInfo();
		Login.instance.SendToMobileServer(mobilePlayerRequestArtifactKnowledgeInfo);
		MobilePlayerRequestMaxFollowers mobilePlayerRequestMaxFollower = new MobilePlayerRequestMaxFollowers();
		Login.instance.SendToMobileServer(mobilePlayerRequestMaxFollower);
		MobilePlayerGarrisonDataRequest mobilePlayerGarrisonDataRequest = new MobilePlayerGarrisonDataRequest()
		{
			GarrTypeID = 3
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonDataRequest);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			this.ClearPendingNotifications();
		}
		else
		{
			this.ScheduleNotifications();
		}
	}

	private void OnDestroy()
	{
		Login.instance.MobileConnectDestroy();
	}

	public void OnMessageReceivedCB(object msg, MobileNetwork.MobileNetworkEventArgs args)
	{
		Queue<Main.MobileMessage> mMessageQueue = this.m_messageQueue;
		Monitor.Enter(mMessageQueue);
		try
		{
			Main.MobileMessage mobileMessage = new Main.MobileMessage()
			{
				msg = msg,
				args = args
			};
			this.m_messageQueue.Enqueue(mobileMessage);
		}
		finally
		{
			Monitor.Exit(mMessageQueue);
		}
	}

	public void OnQuitButton()
	{
		Login.instance.BnQuit();
		Application.Quit();
	}

	public void OnUnknownMessageReceivedCB(object msg, EventArgs e)
	{
		this.m_unknownMsg = (string)msg;
		Debug.Log(string.Concat("Received unknown message ", this.m_unknownMsg.ToString()));
	}

	private void PrecacheMissionChances()
	{
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonMobileMission current = (JamGarrisonMobileMission)enumerator.Current;
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(current.MissionRecID);
				if (record != null && record.GarrFollowerTypeID == 4)
				{
					if (current.MissionState != 1)
					{
						continue;
					}
					List<JamGarrisonFollower> jamGarrisonFollowers = new List<JamGarrisonFollower>();
					Dictionary<int, JamGarrisonFollower>.ValueCollection.Enumerator enumerator1 = PersistentFollowerData.followerDictionary.Values.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							JamGarrisonFollower jamGarrisonFollower = enumerator1.Current;
							if (jamGarrisonFollower.CurrentMissionID != current.MissionRecID)
							{
								continue;
							}
							jamGarrisonFollowers.Add(jamGarrisonFollower);
						}
					}
					finally
					{
						((IDisposable)(object)enumerator1).Dispose();
					}
					MobilePlayerEvaluateMission mobilePlayerEvaluateMission = new MobilePlayerEvaluateMission()
					{
						GarrMissionID = current.MissionRecID,
						GarrFollowerID = new int[jamGarrisonFollowers.Count]
					};
					int num = 0;
					List<JamGarrisonFollower>.Enumerator enumerator2 = jamGarrisonFollowers.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							JamGarrisonFollower current1 = enumerator2.Current;
							int num1 = num;
							num = num1 + 1;
							mobilePlayerEvaluateMission.GarrFollowerID[num1] = current1.GarrFollowerID;
						}
					}
					finally
					{
						((IDisposable)(object)enumerator2).Dispose();
					}
					Login.instance.SendToMobileServer(mobilePlayerEvaluateMission);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
	}

	private void ProcessMessage(Main.MobileMessage mobileMsg)
	{
		object obj = mobileMsg.msg;
		if (obj is MobileClientLoginResult)
		{
			this.MobileClientLoginResultHandler((MobileClientLoginResult)obj);
		}
		else if (obj is MobileClientConnectResult)
		{
			this.MobileClientConnectResultHandler((MobileClientConnectResult)obj);
		}
		else if (obj is MobileClientGarrisonDataRequestResult)
		{
			this.MobileClientGarrisonDataRequestResultHandler((MobileClientGarrisonDataRequestResult)obj);
		}
		else if (obj is MobileClientStartMissionResult)
		{
			this.MobileClientStartMissionResultHandler((MobileClientStartMissionResult)obj);
		}
		else if (obj is MobileClientCompleteMissionResult)
		{
			this.MobileClientCompleteMissionResultHandler((MobileClientCompleteMissionResult)obj);
		}
		else if (obj is MobileClientClaimMissionBonusResult)
		{
			this.MobileClientClaimMissionBonusResultHandler((MobileClientClaimMissionBonusResult)obj);
		}
		else if (obj is MobileClientMissionAdded)
		{
			this.MobileClientMissionAddedHandler((MobileClientMissionAdded)obj);
		}
		else if (obj is MobileClientPong)
		{
			this.MobileClientPongHandler((MobileClientPong)obj);
		}
		else if (obj is MobileClientFollowerChangedXP)
		{
			this.MobileClientFollowerChangedXPHandler((MobileClientFollowerChangedXP)obj);
		}
		else if (obj is MobileClientExpediteMissionCheatResult)
		{
			this.MobileClientExpediteMissionCheatResultHandler((MobileClientExpediteMissionCheatResult)obj);
		}
		else if (obj is MobileClientAdvanceMissionSetResult)
		{
			this.MobileClientAdvanceMissionSetResultHandler((MobileClientAdvanceMissionSetResult)obj);
		}
		else if (obj is MobileClientChat)
		{
			this.MobileClientChatHandler((MobileClientChat)obj, mobileMsg.args.Count);
		}
		else if (obj is MobileClientGuildMembersOnline)
		{
			this.MobileClientGuildMembersOnlineHandler((MobileClientGuildMembersOnline)obj);
		}
		else if (obj is MobileClientGuildMemberLoggedIn)
		{
			this.MobileClientGuildMemberLoggedInHandler((MobileClientGuildMemberLoggedIn)obj);
		}
		else if (obj is MobileClientGuildMemberLoggedOut)
		{
			this.MobileClientGuildMemberLoggedOutHandler((MobileClientGuildMemberLoggedOut)obj);
		}
		else if (obj is MobileClientEmissaryFactionUpdate)
		{
			this.MobileClientEmissaryFactionUpdateHandler((MobileClientEmissaryFactionUpdate)obj);
		}
		else if (obj is MobileClientCreateShipmentResult)
		{
			this.MobileClientCreateShipmentResultHandler((MobileClientCreateShipmentResult)obj);
		}
		else if (obj is MobileClientShipmentsUpdate)
		{
			this.MobileClientShipmentsUpdateHandler((MobileClientShipmentsUpdate)obj);
		}
		else if (obj is MobileClientWorldQuestUpdate)
		{
			this.MobileClientWorldQuestUpdateHandler((MobileClientWorldQuestUpdate)obj);
		}
		else if (obj is MobileClientBountiesByWorldQuestUpdate)
		{
			this.MobileClientBountiesByWorldQuestUpdateHandler((MobileClientBountiesByWorldQuestUpdate)obj);
		}
		else if (obj is MobileClientEvaluateMissionResult)
		{
			this.MobileClientEvaluateMissionResultHandler((MobileClientEvaluateMissionResult)obj);
		}
		else if (obj is MobileClientShipmentTypes)
		{
			this.MobileClientShipmentTypesHandler((MobileClientShipmentTypes)obj);
		}
		else if (obj is MobileClientCompleteShipmentResult)
		{
			this.MobileClientCompleteShipmentResultHandler((MobileClientCompleteShipmentResult)obj);
		}
		else if (obj is MobileClientSetShipmentDurationCheatResult)
		{
			this.MobileClientSetShipmentDurationCheatResultHandler((MobileClientSetShipmentDurationCheatResult)obj);
		}
		else if (obj is MobileClientShipmentPushResult)
		{
			this.MobileClientShipmentPushResultHandler((MobileClientShipmentPushResult)obj);
		}
		else if (obj is MobileClientSetMissionDurationCheatResult)
		{
			this.MobileClientSetMissionDurationCheatResultHandler((MobileClientSetMissionDurationCheatResult)obj);
		}
		else if (obj is MobileClientCanResearchGarrisonTalentResult)
		{
			this.MobileClientCanResearchGarrisonTalentResultHandler((MobileClientCanResearchGarrisonTalentResult)obj);
		}
		else if (obj is MobileClientResearchGarrisonTalentResult)
		{
			this.MobileClientResearchGarrisonTalentResultHandler((MobileClientResearchGarrisonTalentResult)obj);
		}
		else if (obj is MobileClientFollowerEquipmentResult)
		{
			this.MobileClientFollowerEquipmentResultHandler((MobileClientFollowerEquipmentResult)obj);
		}
		else if (obj is MobileClientFollowerChangedQuality)
		{
			this.MobileClientFollowerChangedQualityHandler((MobileClientFollowerChangedQuality)obj);
		}
		else if (obj is MobileClientFollowerArmamentsResult)
		{
			this.MobileClientFollowerArmamentsResultHandler((MobileClientFollowerArmamentsResult)obj);
		}
		else if (obj is MobileClientFollowerArmamentsExtendedResult)
		{
			this.MobileClientFollowerArmamentsExtendedResultHandler((MobileClientFollowerArmamentsExtendedResult)obj);
		}
		else if (obj is MobileClientUseFollowerArmamentResult)
		{
			this.MobileClientUseFollowerArmamentResultHandler((MobileClientUseFollowerArmamentResult)obj);
		}
		else if (obj is MobileClientWorldQuestBountiesResult)
		{
			this.MobileClientWorldQuestBountiesResultHandler((MobileClientWorldQuestBountiesResult)obj);
		}
		else if (obj is MobileClientWorldQuestInactiveBountiesResult)
		{
			this.MobileClientWorldQuestInactiveBountiesResultHandler((MobileClientWorldQuestInactiveBountiesResult)obj);
		}
		else if (obj is MobileClientFollowerActivationDataResult)
		{
			this.MobileClientFollowerActivationDataResultHandler((MobileClientFollowerActivationDataResult)obj);
		}
		else if (obj is MobileClientChangeFollowerActiveResult)
		{
			this.MobileClientChangeFollowerActiveResultHandler((MobileClientChangeFollowerActiveResult)obj);
		}
		else if (obj is MobileClientGetItemTooltipInfoResult)
		{
			this.MobileClientGetItemTooltipInfoResultHandler((MobileClientGetItemTooltipInfoResult)obj);
		}
		else if (obj is MobileClientAuthChallenge)
		{
			this.MobileClientAuthChallengeHandler((MobileClientAuthChallenge)obj);
		}
		else if (obj is MobileClientArtifactInfoResult)
		{
			this.MobileClientArtifactInfoResultHandler((MobileClientArtifactInfoResult)obj);
		}
		else if (obj is MobileClientPlayerLevelUp)
		{
			this.MobileClientPlayerLevelUpHandler((MobileClientPlayerLevelUp)obj);
		}
		else if (obj is MobileClientRequestContributionInfoResult)
		{
			this.MobileClientRequestContributionInfoResultHandler((MobileClientRequestContributionInfoResult)obj);
		}
		else if (obj is MobileClientRequestAreaPoiInfoResult)
		{
			this.MobileClientRequestAreaPoiInfoResultHandler((MobileClientRequestAreaPoiInfoResult)obj);
		}
		else if (obj is MobileClientMakeContributionResult)
		{
			this.MobileClientMakeContributionResultHandler((MobileClientMakeContributionResult)obj);
		}
		else if (obj is MobileClientArtifactKnowledgeInfoResult)
		{
			this.MobileClientArtifactKnowledgeInfoResultHandler((MobileClientArtifactKnowledgeInfoResult)obj);
		}
		else if (obj is MobileClientRequestMaxFollowersResult)
		{
			this.MobileClientRequestMaxFollowersResultHandler((MobileClientRequestMaxFollowersResult)obj);
		}
		else if (!(obj is MobileClientQuestCompleted))
		{
			Debug.Log(string.Concat("Unknown message received: ", obj.ToString()));
		}
		else
		{
			this.MobileClientQuestCompletedHandler((MobileClientQuestCompleted)obj);
		}
	}

	private void ProcessMessages()
	{
		Main.MobileMessage mobileMessage;
		do
		{
			Queue<Main.MobileMessage> mMessageQueue = this.m_messageQueue;
			Monitor.Enter(mMessageQueue);
			try
			{
				if (this.m_messageQueue.Count <= 0)
				{
					mobileMessage = null;
				}
				else
				{
					mobileMessage = this.m_messageQueue.Dequeue();
				}
			}
			finally
			{
				Monitor.Exit(mMessageQueue);
			}
			if (mobileMessage == null)
			{
				continue;
			}
			this.ProcessMessage(mobileMessage);
		}
		while (mobileMessage != null);
	}

	public void RemoveTalentsCheat()
	{
		MobilePlayerGarrisonRemoveAllTalentsCheat mobilePlayerGarrisonRemoveAllTalentsCheat = new MobilePlayerGarrisonRemoveAllTalentsCheat();
		Login.instance.SendToMobileServer(mobilePlayerGarrisonRemoveAllTalentsCheat);
		MobilePlayerGarrisonDataRequest mobilePlayerGarrisonDataRequest = new MobilePlayerGarrisonDataRequest()
		{
			GarrTypeID = 3
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonDataRequest);
	}

	public void RequestEmissaryFactions()
	{
		MobilePlayerRequestEmissaryFactions mobilePlayerRequestEmissaryFaction = new MobilePlayerRequestEmissaryFactions();
		Login.instance.SendToMobileServer(mobilePlayerRequestEmissaryFaction);
	}

	public void RequestWorldQuests()
	{
		MobilePlayerRequestWorldQuests mobilePlayerRequestWorldQuest = new MobilePlayerRequestWorldQuests();
		Login.instance.SendToMobileServer(mobilePlayerRequestWorldQuest);
	}

	private void ScheduleNotifications()
	{
		this.ClearPendingNotifications();
		if (!Main.instance.m_enableNotifications)
		{
			return;
		}
		List<NotificationData> notificationDatas = new List<NotificationData>();
		foreach (JamGarrisonMobileMission list in PersistentMissionData.missionDictionary.Values.OfType<JamGarrisonMobileMission>().ToList<JamGarrisonMobileMission>())
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(list.MissionRecID);
			if (record != null && record.GarrFollowerTypeID == 4)
			{
				if (list.MissionState == 1)
				{
					if ((record.Flags & 16) == 0)
					{
						long num = GarrisonStatus.CurrentTime() - list.StartTime;
						long missionDuration = list.MissionDuration - num;
						NotificationData notificationDatum = new NotificationData()
						{
							notificationText = record.Name,
							secondsRemaining = missionDuration,
							notificationType = NotificationType.missionCompete
						};
						notificationDatas.Add(notificationDatum);
					}
				}
			}
		}
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamCharacterShipment current = (JamCharacterShipment)enumerator.Current;
				CharShipmentRec charShipmentRec = StaticDB.charShipmentDB.GetRecord(current.ShipmentRecID);
				if (charShipmentRec != null)
				{
					string name = "Invalid";
					if (charShipmentRec.GarrFollowerID > 0)
					{
						GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord((int)charShipmentRec.GarrFollowerID);
						if (garrFollowerRec != null)
						{
							int num1 = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? garrFollowerRec.AllianceCreatureID : garrFollowerRec.HordeCreatureID);
							CreatureRec creatureRec = StaticDB.creatureDB.GetRecord(num1);
							if (creatureRec != null)
							{
								name = creatureRec.Name;
							}
							else
							{
								Debug.LogError(string.Concat("Invalid Creature ID: ", num1));
								continue;
							}
						}
						else
						{
							Debug.LogError(string.Concat("Invalid Follower ID: ", charShipmentRec.GarrFollowerID));
							continue;
						}
					}
					if (charShipmentRec.DummyItemID > 0)
					{
						ItemRec itemRec = StaticDB.itemDB.GetRecord(charShipmentRec.DummyItemID);
						if (itemRec != null)
						{
							name = itemRec.Display;
						}
						else
						{
							Debug.LogError(string.Concat("Invalid Item ID: ", charShipmentRec.DummyItemID));
							continue;
						}
					}
					long num2 = GarrisonStatus.CurrentTime() - (long)current.CreationTime;
					long shipmentDuration = (long)current.ShipmentDuration - num2;
					NotificationData notificationDatum1 = new NotificationData()
					{
						notificationText = name,
						secondsRemaining = shipmentDuration,
						notificationType = NotificationType.workOrderReady
					};
					notificationDatas.Add(notificationDatum1);
				}
				else
				{
					Debug.LogError(string.Concat("Invalid Shipment ID: ", current.ShipmentRecID));
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
		IEnumerator enumerator1 = PersistentTalentData.talentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator1.MoveNext())
			{
				JamGarrisonTalent jamGarrisonTalent = (JamGarrisonTalent)enumerator1.Current;
				if ((jamGarrisonTalent.Flags & 1) == 0)
				{
					if (jamGarrisonTalent.StartTime > 0)
					{
						GarrTalentRec garrTalentRec = StaticDB.garrTalentDB.GetRecord(jamGarrisonTalent.GarrTalentID);
						if (garrTalentRec != null)
						{
							long num3 = (long)0;
							num3 = ((jamGarrisonTalent.Flags & 2) != 0 ? (long)garrTalentRec.RespecDurationSecs - (GarrisonStatus.CurrentTime() - (long)jamGarrisonTalent.StartTime) : (long)garrTalentRec.ResearchDurationSecs - (GarrisonStatus.CurrentTime() - (long)jamGarrisonTalent.StartTime));
							NotificationData notificationDatum2 = new NotificationData()
							{
								notificationText = garrTalentRec.Name,
								secondsRemaining = num3,
								notificationType = NotificationType.talentReady
							};
							notificationDatas.Add(notificationDatum2);
						}
					}
				}
			}
		}
		finally
		{
			IDisposable disposable1 = enumerator1 as IDisposable;
			if (disposable1 == null)
			{
			}
			disposable1.Dispose();
		}
		notificationDatas.Sort(new NotificationDataComparer());
		int num4 = 0;
		foreach (NotificationData notificationData in notificationDatas)
		{
			if (notificationData.notificationType == NotificationType.missionCompete)
			{
				int num5 = num4 + 1;
				num4 = num5;
				LocalNotifications.ScheduleMissionCompleteNotification(notificationData.notificationText, num5, notificationData.secondsRemaining);
			}
			if (notificationData.notificationType == NotificationType.workOrderReady)
			{
				int num6 = num4 + 1;
				num4 = num6;
				LocalNotifications.ScheduleWorkOrderReadyNotification(notificationData.notificationText, num6, notificationData.secondsRemaining);
			}
			if (notificationData.notificationType == NotificationType.talentReady)
			{
				int num7 = num4 + 1;
				num4 = num7;
				LocalNotifications.ScheduleTalentResearchCompleteNotification(notificationData.notificationText, num7, notificationData.secondsRemaining);
			}
			Debug.Log(string.Concat(new object[] { "Scheduling Notification for [", notificationData.notificationType, "] ", notificationData.notificationText, " (", num4, ") in ", notificationData.secondsRemaining, " seconds" }));
		}
	}

	public void SelectOrderHallFilterOptionsButton(OrderHallFilterOptionsButton filterOptionsButton)
	{
		if (this.OrderHallfilterOptionsButtonSelectedAction != null)
		{
			this.OrderHallfilterOptionsButtonSelectedAction(filterOptionsButton);
		}
	}

	public void SelectOrderHallNavButton(OrderHallNavButton navButton)
	{
		if (this.OrderHallNavButtonSelectedAction != null)
		{
			this.OrderHallNavButtonSelectedAction(navButton);
		}
	}

	public void SendGuildChat(string chatText)
	{
		MobilePlayerChat mobilePlayerChat = new MobilePlayerChat()
		{
			SlashCmd = 4,
			ChatText = chatText
		};
		Login.instance.SendToMobileServer(mobilePlayerChat);
	}

	public void SetChatScript(GuildChatSlider script)
	{
		this.m_chatPopup = script;
	}

	public void SetDebugText(string newText)
	{
		this.m_debugText.text = newText;
		this.m_debugButton.SetActive(true);
	}

	private void Start()
	{
		Application.targetFrameRate = 30;
		GarrisonStatus.ArtifactKnowledgeLevel = 0;
		GarrisonStatus.ArtifactXpMultiplier = 1f;
	}

	public void StartMission(int garrMissionID, ulong[] followerDBIDs)
	{
		Debug.Log(string.Concat("Main.StartMission() ", garrMissionID));
		MobilePlayerGarrisonStartMission mobilePlayerGarrisonStartMission = new MobilePlayerGarrisonStartMission()
		{
			GarrMissionID = garrMissionID,
			FollowerDBIDs = followerDBIDs
		};
		Login.instance.SendToMobileServer(mobilePlayerGarrisonStartMission);
	}

	private void Update()
	{
		this.ProcessMessages();
		this.UpdateDebugText();
		this.UpdateCanvasOrientation();
	}

	private void UpdateCanvasOrientation()
	{
		if (Screen.width <= Screen.height)
		{
			this.canvasAnimator.SetBool("isLandscape", false);
		}
		else
		{
			this.canvasAnimator.SetBool("isLandscape", true);
		}
	}

	private void UpdateDebugText()
	{
		Main mFrameCount = this;
		mFrameCount.m_frameCount = mFrameCount.m_frameCount + 1;
	}

	public void UseArmament(int garrFollowerID, int itemID)
	{
		if (this.UseArmamentStartAction != null)
		{
			this.UseArmamentStartAction(garrFollowerID);
		}
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(garrFollowerID);
		MobilePlayerUseFollowerArmament mobilePlayerUseFollowerArmament = new MobilePlayerUseFollowerArmament()
		{
			GarrFollowerID = garrFollowerID,
			GarrFollowerTypeID = (int)record.GarrFollowerTypeID,
			ItemID = itemID
		};
		Login.instance.SendToMobileServer(mobilePlayerUseFollowerArmament);
	}

	public void UseEquipment(int garrFollowerID, int itemID, int replaceThisAbilityID)
	{
		if (this.UseEquipmentStartAction != null)
		{
			this.UseEquipmentStartAction(replaceThisAbilityID);
		}
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(garrFollowerID);
		MobilePlayerUseFollowerEquipment mobilePlayerUseFollowerEquipment = new MobilePlayerUseFollowerEquipment()
		{
			GarrFollowerID = garrFollowerID,
			GarrFollowerTypeID = (int)record.GarrFollowerTypeID,
			ItemID = itemID,
			ReplaceAbilityID = replaceThisAbilityID
		};
		Login.instance.SendToMobileServer(mobilePlayerUseFollowerEquipment);
	}

	private class MobileMessage
	{
		public object msg;

		public MobileNetwork.MobileNetworkEventArgs args;

		public MobileMessage()
		{
		}
	}

	public interface SetCache
	{
		void SetCachePath(string cachePath);
	}
}