using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class Main : MonoBehaviour
	{
		private int m_frameCount;

		public Canvas mainCanvas;

		public Canvas m_level2Canvas;

		public Canvas m_level3Canvas;

		private Animator canvasAnimator;

		public AllPanels allPanels;

		public AllPopups allPopups;

		private string m_unknownMsg;

		public UISound m_UISound;

		public bool m_enableNotifications;

		public static Main instance;

		public Action FollowerDataChangedAction;

		public Action<int> MissionSuccessChanceChangedAction;

		public Action GarrisonDataResetStartedAction;

		public Action GarrisonDataResetFinishedAction;

		public Action ShipmentTypesUpdatedAction;

		public Action<int> CreateShipmentResultAction;

		public Action<int, ulong> ShipmentAddedAction;

		public Action<SHIPMENT_RESULT, ulong> CompleteShipmentResultAction;

		public Action<int, int> MissionAddedAction;

		public Action<WrapperGarrisonFollower, WrapperGarrisonFollower> FollowerChangedXPAction;

		public Action<WrapperGarrisonFollower> TroopExpiredAction;

		public Action<int, int, string> CanResearchGarrisonTalentResultAction;

		public Action<int, int> ResearchGarrisonTalentResultAction;

		public Action BountyInfoUpdatedAction;

		public Action<int> UseEquipmentStartAction;

		public Action<WrapperGarrisonFollower, WrapperGarrisonFollower> UseEquipmentResultAction;

		public Action EquipmentInventoryChangedAction;

		public Action<int> UseArmamentStartAction;

		public Action<int, WrapperGarrisonFollower, WrapperGarrisonFollower> UseArmamentResultAction;

		public Action ArmamentInventoryChangedAction;

		public Action<OrderHallFilterOptionsButton> OrderHallfilterOptionsButtonSelectedAction;

		public Action<CompanionNavButton> CompanionNavButtonSelectionAction;

		public Action<int, WrapperShipmentItem> ShipmentItemPushedAction;

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

		private bool m_narrowScreen;

		public Main()
		{
		}

		public GameObject AddChildToLevel2Canvas(GameObject prefab)
		{
			GameObject gameObject = this.InstantiateBasedOnPrefab(prefab);
			this.AddChildToObject(this.m_level2Canvas.transform, gameObject.transform);
			return gameObject;
		}

		public GameObject AddChildToLevel3Canvas(GameObject prefab)
		{
			GameObject gameObject = this.InstantiateBasedOnPrefab(prefab);
			this.AddChildToObject(this.m_level3Canvas.transform, gameObject.transform);
			return gameObject;
		}

		private void AddChildToObject(Transform parent, Transform child)
		{
			child.SetParent(parent, false);
		}

		private void AdvanceMissionSetResultHandler(LegionCompanionWrapper.AdvanceMissionSetCheatResultEvent msg)
		{
			Debug.Log(string.Concat(new object[] { "Advance mission set ", msg.MissionSetID, " success: ", msg.Success }));
		}

		private void ArtifactInfoResultHandler(LegionCompanionWrapper.ArtifactInfoResultEvent eventArgs)
		{
			GarrisonStatus.ArtifactKnowledgeLevel = eventArgs.KnowledgeLevel;
			GarrisonStatus.ArtifactXpMultiplier = eventArgs.XpMultiplier;
		}

		private void Awake()
		{
			Main.instance = this;
			this.m_enableNotifications = true;
			this.GenerateUniqueIdentifier();
			this.canvasAnimator = this.mainCanvas.GetComponent<Animator>();
			this.SetNarrowScreen();
		}

		private void BountiesByWorldQuestUpdateHandler(LegionCompanionWrapper.BountiesByWorldQuestUpdateEvent eventArgs)
		{
			foreach (WrapperBountiesByWorldQuest quest in eventArgs.Quests)
			{
				PersistentBountyData.AddOrUpdateBountiesByWorldQuest(quest);
			}
			if (AdventureMapPanel.instance != null)
			{
				AdventureMapPanel.instance.UpdateWorldQuests();
			}
		}

		private void CanResearchGarrisonTalentResultHandler(LegionCompanionWrapper.CanResearchGarrisonTalentResultEvent eventArgs)
		{
			if (this.CanResearchGarrisonTalentResultAction != null)
			{
				this.CanResearchGarrisonTalentResultAction(eventArgs.GarrTalentID, eventArgs.Result, eventArgs.ConditionText);
			}
		}

		private void ChangeFollowerActiveResultHandler(LegionCompanionWrapper.ChangeFollowerActiveResultEvent eventArgs)
		{
			if (eventArgs.Result == (int)GARRISON_RESULT.SUCCESS)
			{
				PersistentFollowerData.AddOrUpdateFollower(eventArgs.Follower);
				if (GeneralHelpers.GetFollowerStatus(eventArgs.Follower) != FollowerStatus.inactive)
				{
					Debug.Log(string.Concat("Follower is now active. ", eventArgs.ActivationsRemaining, " activations remain for the day."));
				}
				else
				{
					Debug.Log(string.Concat("Follower is now inactive. ", eventArgs.ActivationsRemaining, " activations remain for the day."));
				}
				if (this.FollowerDataChangedAction != null)
				{
					this.FollowerDataChangedAction();
				}
				LegionCompanionWrapper.RequestFollowerActivationData((int)GarrisonStatus.GarrisonType);
			}
		}

		public void CheatFastForwardOneHour()
		{
			GarrisonStatus.CheatFastForwardOneHour();
		}

		public void ClaimMissionBonus(int garrMissionID)
		{
			LegionCompanionWrapper.ClaimMissionBonus(garrMissionID);
		}

		private void ClaimMissionBonusResultHandler(LegionCompanionWrapper.ClaimMissionBonusResultEvent msg)
		{
			PersistentMissionData.UpdateMission(msg.Result.Mission);
			AdventureMapMissionSite[] componentsInChildren = AdventureMapPanel.instance.m_mapViewContentsRT.GetComponentsInChildren<AdventureMapMissionSite>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				AdventureMapMissionSite adventureMapMissionSite = componentsInChildren[i];
				if (!adventureMapMissionSite.m_isStackablePreview)
				{
					if (adventureMapMissionSite.GetGarrMissionID() == msg.Result.GarrMissionID)
					{
						if (!adventureMapMissionSite.gameObject.activeSelf)
						{
							adventureMapMissionSite.gameObject.SetActive(true);
						}
						adventureMapMissionSite.HandleClaimMissionBonusResult(msg.Result.GarrMissionID, msg.Result.AwardOvermax, msg.Result.Result);
						break;
					}
				}
			}
		}

		public void ClearPendingNotifications()
		{
			LocalNotifications.ClearPending();
		}

		public void CompleteAllMissions()
		{
			Debug.Log("Main.CompleteAllMissions()");
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null && record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
				{
					if (value.MissionState != 1)
					{
						continue;
					}
					TimeSpan timeSpan = GarrisonStatus.CurrentTime() - value.StartTime;
					if ((value.MissionDuration - timeSpan).TotalSeconds > 0)
					{
						continue;
					}
					this.CompleteMission(value.MissionRecID);
				}
			}
		}

		public void CompleteMission(int garrMissionID)
		{
			LegionCompanionWrapper.GarrisonCompleteMission(garrMissionID);
		}

		private void CompleteMissionResultHandler(LegionCompanionWrapper.GarrisonCompleteMissionResultEvent eventArgs)
		{
			PersistentMissionData.UpdateMission(eventArgs.Result.Mission);
			AdventureMapMissionSite[] componentsInChildren = AdventureMapPanel.instance.m_mapViewContentsRT.GetComponentsInChildren<AdventureMapMissionSite>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				AdventureMapMissionSite adventureMapMissionSite = componentsInChildren[i];
				if (!adventureMapMissionSite.m_isStackablePreview)
				{
					if (adventureMapMissionSite.GetGarrMissionID() == eventArgs.Result.GarrMissionID)
					{
						if (!adventureMapMissionSite.gameObject.activeSelf)
						{
							adventureMapMissionSite.gameObject.SetActive(true);
						}
						adventureMapMissionSite.HandleCompleteMissionResult(eventArgs.Result.GarrMissionID, eventArgs.Result.BonusRollSucceeded);
						break;
					}
				}
			}
			LegionCompanionWrapper.RequestShipmentTypes((int)GarrisonStatus.GarrisonType);
			LegionCompanionWrapper.RequestShipments((int)GarrisonStatus.GarrisonType);
			LegionCompanionWrapper.RequestFollowerEquipment((int)GarrisonStatus.GarrisonFollowerType);
			LegionCompanionWrapper.RequestGarrisonData((int)GarrisonStatus.GarrisonType);
		}

		private void CompleteShipmentResultHandler(LegionCompanionWrapper.CompleteShipmentResultEvent eventArgs)
		{
			SHIPMENT_RESULT result = (SHIPMENT_RESULT)eventArgs.Result;
			if (this.CompleteShipmentResultAction != null)
			{
				this.CompleteShipmentResultAction(result, eventArgs.ShipmentID);
			}
		}

		private void CreateShipmentResultHandler(LegionCompanionWrapper.CreateShipmentResultEvent eventArgs)
		{
			GARRISON_RESULT result = (GARRISON_RESULT)eventArgs.Result;
			result == GARRISON_RESULT.SUCCESS;
			if (this.CreateShipmentResultAction != null)
			{
				this.CreateShipmentResultAction(eventArgs.Result);
			}
			if (result == GARRISON_RESULT.SUCCESS)
			{
				LegionCompanionWrapper.RequestShipmentTypes((int)GarrisonStatus.GarrisonType);
				LegionCompanionWrapper.RequestShipments((int)GarrisonStatus.GarrisonType);
				LegionCompanionWrapper.RequestGarrisonData((int)GarrisonStatus.GarrisonType);
			}
		}

		private static bool DoesMapIDSupportWorldQuests(int mapID)
		{
			return (mapID == 1220 || mapID == 1669 || mapID == 1642 ? true : mapID == 1643);
		}

		private void EmissaryFactionUpdateHandler(LegionCompanionWrapper.EmissaryFactionsUpdateEvent eventArgs)
		{
			this.allPopups.EmissaryFactionUpdate(eventArgs.Factions);
		}

		private void EvaluateMissionResultHandler(LegionCompanionWrapper.EvaluateMissionResultEvent eventArgs)
		{
			if (eventArgs.Result != 0)
			{
				GARRISON_RESULT result = (GARRISON_RESULT)eventArgs.Result;
			}
			else
			{
				MissionDataCache.AddOrUpdateMissionData(eventArgs.GarrMissionID, eventArgs.SuccessChance);
				if (this.MissionSuccessChanceChangedAction != null)
				{
					this.MissionSuccessChanceChangedAction(eventArgs.SuccessChance);
				}
			}
		}

		private void ExpediteMissionCheatResultHandler(LegionCompanionWrapper.ExpediteMissionCheatResultEvent eventArgs)
		{
			if (eventArgs.Result != 0)
			{
				Debug.Log(string.Concat(new object[] { "MobileClientExpediteMissionCheatResult: Mission ID ", eventArgs.MissionRecID, " failed with error ", eventArgs.Result }));
			}
			else
			{
				Debug.Log(string.Concat("Expedited completion of mission ", eventArgs.MissionRecID));
				LegionCompanionWrapper.RequestGarrisonData((int)GarrisonStatus.GarrisonType);
			}
		}

		private void FollowerActivationDataResultHandler(LegionCompanionWrapper.FollowerActivationDataResultEvent eventArgs)
		{
			GarrisonStatus.SetFollowerActivationInfo(eventArgs.ActivationsRemaining, eventArgs.GoldCost);
		}

		private void FollowerArmamentsExtendedResultHandler(LegionCompanionWrapper.FollowerArmamentsExtendedResultEvent eventArgs)
		{
			PersistentArmamentData.ClearData();
			for (int i = 0; i < eventArgs.Armaments.Count; i++)
			{
				PersistentArmamentData.AddOrUpdateArmament(eventArgs.Armaments[i]);
			}
			if (this.ArmamentInventoryChangedAction != null)
			{
				this.ArmamentInventoryChangedAction();
			}
		}

		private void FollowerChangedQualityHandler(LegionCompanionWrapper.FollowerChangedQualityEvent eventArgs)
		{
			PersistentFollowerData.AddOrUpdateFollower(eventArgs.Follower);
			if (this.UseEquipmentResultAction != null)
			{
				this.UseEquipmentResultAction(eventArgs.OldFollower, eventArgs.Follower);
			}
			LegionCompanionWrapper.RequestFollowerEquipment((int)GarrisonStatus.GarrisonFollowerType);
		}

		private void FollowerChangedXPHandler(LegionCompanionWrapper.FollowerChangedXpEvent eventArgs)
		{
			if (this.FollowerChangedXPAction != null)
			{
				this.FollowerChangedXPAction(eventArgs.OldFollower, eventArgs.Follower);
			}
		}

		private void FollowerEquipmentResultHandler(LegionCompanionWrapper.FollowerEquipmentResultEvent eventArgs)
		{
			PersistentEquipmentData.ClearData();
			for (int i = 0; i < eventArgs.Equipments.Count; i++)
			{
				PersistentEquipmentData.AddOrUpdateEquipment(eventArgs.Equipments[i]);
			}
			if (this.EquipmentInventoryChangedAction != null)
			{
				this.EquipmentInventoryChangedAction();
			}
		}

		private void GarrisonDataResultHandler(LegionCompanionWrapper.GarrisonDataRequestResultEvent eventArgs)
		{
			PersistentFollowerData.ClearData();
			PersistentMissionData.ClearData();
			PersistentTalentData.ClearData();
			if (this.GarrisonDataResetStartedAction != null)
			{
				this.GarrisonDataResetStartedAction();
			}
			GarrisonStatus.SetFaction(eventArgs.Data.PvpFaction);
			GarrisonStatus.SetGarrisonServerConnectTime(eventArgs.Data.ServerTime);
			GarrisonStatus.SetCurrencies(eventArgs.Data.GoldCurrency, eventArgs.Data.OrderhallResourcesCurrency, eventArgs.Data.WarResourcesCurrency);
			GarrisonStatus.SetCharacterName(eventArgs.Data.CharacterName);
			GarrisonStatus.SetCharacterLevel(eventArgs.Data.CharacterLevel);
			GarrisonStatus.SetCharacterClass(eventArgs.Data.CharacterClassID);
			for (int i = 0; i < eventArgs.Data.Followers.Count; i++)
			{
				WrapperGarrisonFollower item = eventArgs.Data.Followers[i];
				if (StaticDB.garrFollowerDB.GetRecord(item.GarrFollowerID) != null)
				{
					PersistentFollowerData.AddOrUpdateFollower(item);
					if ((item.Flags & 8) != 0 && item.Durability <= 0 && this.TroopExpiredAction != null)
					{
						this.TroopExpiredAction(item);
					}
				}
			}
			for (int j = 0; j < eventArgs.Data.Missions.Count; j++)
			{
				PersistentMissionData.AddMission(eventArgs.Data.Missions[j]);
			}
			for (int k = 0; k < eventArgs.Data.Talents.Count; k++)
			{
				PersistentTalentData.AddOrUpdateTalent(eventArgs.Data.Talents[k]);
			}
			if (!GarrisonStatus.Initialized)
			{
				Main.instance.GarrisonDataResetFinishedAction += new Action(this.HandleEnterWorld);
				GarrisonStatus.Initialized = true;
			}
			if (this.GarrisonDataResetFinishedAction != null)
			{
				this.GarrisonDataResetFinishedAction();
			}
			if (this.FollowerDataChangedAction != null)
			{
				this.FollowerDataChangedAction();
			}
			Singleton<Login>.Instance.MobileLoginDataRequestComplete();
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

		private void GetItemTooltipInfoResultHandler(LegionCompanionWrapper.ItemTooltipInfoResultEvent eventArgs)
		{
			ItemStatCache.instance.AddMobileItemStats(eventArgs.ItemID, eventArgs.ItemContext, eventArgs.Stats);
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
			this.PrecacheMissionChances();
			LocalNotifications.RegisterForNotifications();
			SceneManager.LoadSceneAsync(Scenes.MainSceneName);
		}

		public void HideDebugText()
		{
			this.m_debugButton.SetActive(false);
		}

		private GameObject InstantiateBasedOnPrefab(GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			RectTransform rectTransform1 = prefab.transform as RectTransform;
			rectTransform.anchorMin = rectTransform1.anchorMin;
			rectTransform.anchorMax = rectTransform1.anchorMax;
			Vector2 vector2 = Vector2.zero;
			rectTransform.offsetMax = vector2;
			rectTransform.offsetMin = vector2;
			return gameObject;
		}

		public bool IsNarrowScreen()
		{
			return this.m_narrowScreen;
		}

		private void LoadPreferences()
		{
			bool flag = this.m_UISound.IsSFXEnabled();
			bool.TryParse(SecurePlayerPrefs.GetString("EnableSFX", Main.uniqueIdentifier), out flag);
			this.m_UISound.EnableSFX(flag);
			bool.TryParse(SecurePlayerPrefs.GetString("EnableNotifications", Main.uniqueIdentifier), out this.m_enableNotifications);
		}

		public void Logout()
		{
			MissionDataCache.ClearData();
			AllPopups.instance.HideAllPopups();
			AllPanels.instance.ShowOrderHallMultiPanel(false);
			AllPanels.instance.ShowCompanionMultiPanel(false);
			Singleton<Login>.instance.ReconnectToMobileServerCharacterSelect();
		}

		private void MakeContributionResultHandler(LegionCompanionWrapper.MakeContributionResultEvent eventArgs)
		{
			Debug.Log(string.Concat(new object[] { "Make Contribution Result for ID ", eventArgs.ContributionID, " is ", eventArgs.Result }));
			LegionCompanionWrapper.RequestContributionInfo();
			if (this.ContributionInfoChangedAction != null)
			{
				this.ContributionInfoChangedAction();
			}
		}

		private void MissionAddedHandler(LegionCompanionWrapper.MissionAddedEvent eventArgs)
		{
			if (eventArgs.Result != 0 || eventArgs.Mission.MissionRecID == 0)
			{
				GARRISON_RESULT result = (GARRISON_RESULT)eventArgs.Result;
				object[] str = new object[] { "Error adding mission: ", result.ToString(), " Mission ID:", null };
				str[3] = eventArgs.Mission.MissionRecID;
				Debug.Log(string.Concat(str));
			}
			else
			{
				PersistentMissionData.AddMission(eventArgs.Mission);
			}
			if (this.MissionAddedAction != null)
			{
				this.MissionAddedAction(eventArgs.Mission.MissionRecID, eventArgs.Result);
			}
		}

		public void MobileLoggedIn()
		{
			PersistentArmamentData.ClearData();
			PersistentBountyData.ClearData();
			PersistentEquipmentData.ClearData();
			PersistentFollowerData.ClearData();
			PersistentFollowerData.ClearPreMissionFollowerData();
			PersistentMissionData.ClearData();
			PersistentShipmentData.ClearData();
			PersistentTalentData.ClearData();
			MissionDataCache.ClearData();
			WorldQuestData.ClearData();
			ItemStatCache.instance.ClearItemStats();
			GarrisonStatus.Initialized = false;
			Club.SetCommunityID(Singleton<CharacterData>.Instance.CommunityID);
			MobileClient.Initialize();
			this.MobileRequestData();
		}

		public void MobileRequestData()
		{
			LegionCompanionWrapper.RequestShipmentTypes((int)GarrisonStatus.GarrisonType);
			LegionCompanionWrapper.RequestShipments((int)GarrisonStatus.GarrisonType);
			LegionCompanionWrapper.RequestWorldQuestBounties(10);
			this.RequestWorldQuests();
			LegionCompanionWrapper.RequestFollowerEquipment((int)GarrisonStatus.GarrisonFollowerType);
			LegionCompanionWrapper.RequestFollowerArmamentsExtended((int)GarrisonStatus.GarrisonFollowerType);
			LegionCompanionWrapper.RequestFollowerActivationData((int)GarrisonStatus.GarrisonType);
			LegionCompanionWrapper.GetArtifactInfo();
			LegionCompanionWrapper.RequestContributionInfo();
			LegionCompanionWrapper.RequestAreaPoiInfo();
			LegionCompanionWrapper.RequestMaxFollowers((int)GarrisonStatus.GarrisonFollowerType);
			LegionCompanionWrapper.RequestGarrisonData((int)GarrisonStatus.GarrisonType);
			LegionCompanionWrapper.RequestAreaPoiInfo();
			if (this.GarrisonDataResetFinishedAction != null)
			{
				this.GarrisonDataResetFinishedAction();
			}
		}

		public void NudgeX(ref GameObject gameObj, float amount)
		{
			if (gameObj != null)
			{
				RectTransform component = gameObj.GetComponent<RectTransform>();
				if (component)
				{
					Vector2 vector2 = component.anchoredPosition;
					vector2.x += amount;
					component.anchoredPosition = vector2;
				}
			}
		}

		public void NudgeY(ref GameObject gameObj, float amount)
		{
			if (gameObj != null)
			{
				RectTransform component = gameObj.GetComponent<RectTransform>();
				if (component)
				{
					Vector2 vector2 = component.anchoredPosition;
					vector2.y += amount;
					component.anchoredPosition = vector2;
				}
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
		}

		private void OnApplicationQuit()
		{
			if (Network.IsConnected() && Network.IsLoggedIn())
			{
				Network.Logout();
			}
			Singleton<Login>.instance.BnQuit();
			MobileClient.Shutdown();
		}

		private void OnDestroy()
		{
			this.UnsubscribeFromEvents();
			if (SceneManager.GetActiveScene().name != Scenes.TitleSceneName)
			{
				CommunityData.Instance.Shutdown();
				MobileClient.Disconnect();
			}
		}

		public void OnQuitButton()
		{
			Application.Quit();
		}

		private void PlayerLevelUpHandler(LegionCompanionWrapper.PlayerLevelUpEvent eventArgs)
		{
			Debug.Log(string.Concat("Congrats, your character is now level ", eventArgs.NewLevel));
			AllPopups.instance.ShowLevelUpToast(eventArgs.NewLevel);
			if (this.PlayerLeveledUpAction != null)
			{
				this.PlayerLeveledUpAction(eventArgs.NewLevel);
			}
		}

		private void PrecacheMissionChances()
		{
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null && record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
				{
					if (value.MissionState != 1)
					{
						continue;
					}
					List<WrapperGarrisonFollower> wrapperGarrisonFollowers = new List<WrapperGarrisonFollower>();
					foreach (WrapperGarrisonFollower wrapperGarrisonFollower in PersistentFollowerData.followerDictionary.Values)
					{
						if (wrapperGarrisonFollower.CurrentMissionID != value.MissionRecID)
						{
							continue;
						}
						wrapperGarrisonFollowers.Add(wrapperGarrisonFollower);
					}
					LegionCompanionWrapper.EvaluateMission(value.MissionRecID, (
						from f in wrapperGarrisonFollowers
						select f.GarrFollowerID).ToList<int>());
				}
			}
		}

		private void QuestCompletedHandler(LegionCompanionWrapper.QuestCompletedEvent eventArgs)
		{
			foreach (WrapperQuestItem item in eventArgs.Items)
			{
				if (this.GotItemFromQuestCompletionAction == null)
				{
					continue;
				}
				this.GotItemFromQuestCompletionAction(item.ItemID, item.Quantity, eventArgs.QuestID);
			}
		}

		private void RequestAreaPoiInfoResultHandler(LegionCompanionWrapper.AreaPoiInfoResultEvent eventArgs)
		{
			LegionfallData.SetCurrentInvasionPOI(null);
			if (eventArgs.POIData != null && eventArgs.POIData.Count > 0)
			{
				LegionfallData.SetCurrentInvasionPOI(new WrapperAreaPoi?(eventArgs.POIData[0]));
				LegionfallData.SetCurrentInvasionExpirationTime(eventArgs.POIData[0].TimeRemaining);
			}
			if (this.InvasionPOIChangedAction != null)
			{
				this.InvasionPOIChangedAction();
			}
		}

		private void RequestContributionInfoResultHandler(LegionCompanionWrapper.ContributionInfoResultEvent eventArgs)
		{
			LegionfallData.ClearData();
			LegionfallData.SetLegionfallWarResources(eventArgs.LegionfallWarResources);
			LegionfallData.SetHasAccess(eventArgs.HasAccess);
			foreach (WrapperContribution contribution in eventArgs.Contributions)
			{
				LegionfallData.AddOrUpdateLegionfallBuilding(contribution);
			}
			if (this.ContributionInfoChangedAction != null)
			{
				this.ContributionInfoChangedAction();
			}
		}

		public void RequestEmissaryFactions(int factionID)
		{
			LegionCompanionWrapper.RequestEmissaryFactions(factionID);
		}

		private void RequestMaxFollowersResultHandler(LegionCompanionWrapper.RequestMaxFollowersResultEvent eventArgs)
		{
			GarrisonStatus.SetMaxActiveFollowers(eventArgs.MaxFollowers);
			if (this.MaxActiveFollowersChangedAction != null)
			{
				this.MaxActiveFollowersChangedAction();
			}
		}

		public void RequestWorldQuests()
		{
			LegionCompanionWrapper.RequestWorldQuests();
		}

		private void ResearchGarrisonTalentResultHandler(LegionCompanionWrapper.ResearchGarrisonTalentResultEvent eventArgs)
		{
			GARRISON_RESULT result = (GARRISON_RESULT)eventArgs.Result;
			if (result != GARRISON_RESULT.SUCCESS)
			{
				AllPopups.instance.ShowGenericPopup(StaticDB.GetString("TALENT_RESEARCH_FAILED", null), result.ToString());
			}
			if (this.ResearchGarrisonTalentResultAction != null)
			{
				this.ResearchGarrisonTalentResultAction(eventArgs.GarrTalentID, eventArgs.Result);
			}
		}

		public void ScheduleNotifications()
		{
			this.ClearPendingNotifications();
			if (!Main.instance.m_enableNotifications)
			{
				return;
			}
			List<NotificationData> notificationDatas = new List<NotificationData>();
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null && record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
				{
					if (value.MissionState == 1)
					{
						if ((record.Flags & 16) == 0)
						{
							TimeSpan timeSpan = GarrisonStatus.CurrentTime() - value.StartTime;
							TimeSpan missionDuration = value.MissionDuration - timeSpan;
							NotificationData notificationDatum = new NotificationData()
							{
								notificationText = record.Name,
								timeRemaining = missionDuration,
								notificationType = NotificationType.missionCompete
							};
							notificationDatas.Add(notificationDatum);
						}
					}
				}
			}
			foreach (WrapperCharacterShipment wrapperCharacterShipment in PersistentShipmentData.shipmentDictionary.Values)
			{
				CharShipmentRec charShipmentRec = StaticDB.charShipmentDB.GetRecord(wrapperCharacterShipment.ShipmentRecID);
				if (charShipmentRec != null)
				{
					string name = "Invalid";
					if (charShipmentRec.GarrFollowerID > 0)
					{
						GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord((int)charShipmentRec.GarrFollowerID);
						if (garrFollowerRec != null)
						{
							int num = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? garrFollowerRec.AllianceCreatureID : garrFollowerRec.HordeCreatureID);
							CreatureRec creatureRec = StaticDB.creatureDB.GetRecord(num);
							if (creatureRec != null)
							{
								name = creatureRec.Name;
							}
							else
							{
								Debug.LogError(string.Concat("Invalid Creature ID: ", num));
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
					TimeSpan timeSpan1 = GarrisonStatus.CurrentTime() - wrapperCharacterShipment.CreationTime;
					TimeSpan shipmentDuration = wrapperCharacterShipment.ShipmentDuration - timeSpan1;
					NotificationData notificationDatum1 = new NotificationData()
					{
						notificationText = name,
						timeRemaining = shipmentDuration,
						notificationType = NotificationType.workOrderReady
					};
					notificationDatas.Add(notificationDatum1);
				}
				else
				{
					Debug.LogError(string.Concat("Invalid Shipment ID: ", wrapperCharacterShipment.ShipmentRecID));
				}
			}
			foreach (WrapperGarrisonTalent wrapperGarrisonTalent in PersistentTalentData.talentDictionary.Values)
			{
				if ((wrapperGarrisonTalent.Flags & 1) == 0)
				{
					if (wrapperGarrisonTalent.StartTime > DateTime.UtcNow)
					{
						GarrTalentRec garrTalentRec = StaticDB.garrTalentDB.GetRecord(wrapperGarrisonTalent.GarrTalentID);
						if (garrTalentRec != null)
						{
							TimeSpan zero = TimeSpan.Zero;
							zero = ((wrapperGarrisonTalent.Flags & 2) != 0 ? TimeSpan.FromSeconds((double)garrTalentRec.RespecDurationSecs) - (GarrisonStatus.CurrentTime() - wrapperGarrisonTalent.StartTime) : TimeSpan.FromSeconds((double)garrTalentRec.ResearchDurationSecs) - (GarrisonStatus.CurrentTime() - wrapperGarrisonTalent.StartTime));
							NotificationData notificationDatum2 = new NotificationData()
							{
								notificationText = garrTalentRec.Name,
								timeRemaining = zero,
								notificationType = NotificationType.talentReady
							};
							notificationDatas.Add(notificationDatum2);
						}
					}
				}
			}
			int num1 = 0;
			foreach (NotificationData notificationDatum3 in 
				from n in notificationDatas
				orderby n.timeRemaining
				select n)
			{
				if (notificationDatum3.notificationType == NotificationType.missionCompete)
				{
					int num2 = num1 + 1;
					num1 = num2;
					LocalNotifications.ScheduleMissionCompleteNotification(notificationDatum3.notificationText, num2, Convert.ToInt64(notificationDatum3.timeRemaining.TotalSeconds));
				}
				if (notificationDatum3.notificationType == NotificationType.workOrderReady)
				{
					int num3 = num1 + 1;
					num1 = num3;
					LocalNotifications.ScheduleWorkOrderReadyNotification(notificationDatum3.notificationText, num3, Convert.ToInt64(notificationDatum3.timeRemaining.TotalSeconds));
				}
				if (notificationDatum3.notificationType == NotificationType.talentReady)
				{
					int num4 = num1 + 1;
					num1 = num4;
					LocalNotifications.ScheduleTalentResearchCompleteNotification(notificationDatum3.notificationText, num4, Convert.ToInt64(notificationDatum3.timeRemaining.TotalSeconds));
				}
				Debug.Log(string.Concat(new object[] { "Scheduling Notification for [", notificationDatum3.notificationType, "] ", notificationDatum3.notificationText, " (", num1, ") in ", notificationDatum3.timeRemaining.TotalSeconds, " seconds" }));
			}
		}

		public void SelectCompanionNavButton(CompanionNavButton navButton)
		{
			if (this.CompanionNavButtonSelectionAction != null)
			{
				this.CompanionNavButtonSelectionAction(navButton);
			}
		}

		public void SelectOrderHallFilterOptionsButton(OrderHallFilterOptionsButton filterOptionsButton)
		{
			if (this.OrderHallfilterOptionsButtonSelectedAction != null)
			{
				this.OrderHallfilterOptionsButtonSelectedAction(filterOptionsButton);
			}
		}

		public void SetDebugText(string newText)
		{
			this.m_debugText.text = newText;
			this.m_debugButton.SetActive(true);
		}

		private void SetMissionDurationCheatResultHandler(LegionCompanionWrapper.SetMissionDurationCheatResultEvent eventArgs)
		{
			AllPopups.instance.HideAllPopups();
		}

		private void SetNarrowScreen()
		{
			float single = 1f;
			float single1 = (float)Screen.height;
			float single2 = (float)Screen.width;
			if (single2 > 0f && single1 > 0f)
			{
				single = (single1 <= single2 ? single2 / single1 : single1 / single2);
			}
			if (single > 1.9f)
			{
				this.m_narrowScreen = true;
			}
		}

		private void ShipmentPushResultHandler(LegionCompanionWrapper.ShipmentPushResultEvent eventArgs)
		{
			foreach (WrapperShipmentItem item in eventArgs.Items)
			{
				if (this.ShipmentItemPushedAction == null)
				{
					continue;
				}
				this.ShipmentItemPushedAction(eventArgs.CharShipmentID, item);
			}
		}

		private void ShipmentsUpdateHandler(LegionCompanionWrapper.ShipmentsUpdateEvent eventArgs)
		{
			PersistentShipmentData.ClearData();
			foreach (WrapperCharacterShipment shipment in eventArgs.Shipments)
			{
				PersistentShipmentData.AddOrUpdateShipment(shipment);
				if (this.ShipmentAddedAction == null)
				{
					continue;
				}
				this.ShipmentAddedAction(shipment.ShipmentRecID, shipment.ShipmentID);
			}
		}

		private void ShipmentTypesHandler(LegionCompanionWrapper.ShipmentTypesResultEvent eventArgs)
		{
			PersistentShipmentData.SetAvailableShipmentTypes(eventArgs.Shipments);
			if (this.ShipmentTypesUpdatedAction != null)
			{
				this.ShipmentTypesUpdatedAction();
			}
		}

		private void Start()
		{
			Application.targetFrameRate = 30;
			GarrisonStatus.ArtifactKnowledgeLevel = 0;
			GarrisonStatus.ArtifactXpMultiplier = 1f;
			MobileClient.RegisterHandlers();
			this.SubscribeToEvents();
			this.LoadPreferences();
		}

		public void StartMission(int garrMissionID, IEnumerable<ulong> followerDBIDs)
		{
			LegionCompanionWrapper.GarrisonStartMission(garrMissionID, followerDBIDs.ToList<ulong>());
		}

		private void StartMissionResultHandler(LegionCompanionWrapper.GarrisonStartMissionResultEvent eventArgs)
		{
			if (eventArgs.Result.Result != 0)
			{
				GARRISON_RESULT result = (GARRISON_RESULT)eventArgs.Result.Result;
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

		private void SubscribeToEvents()
		{
			LegionCompanionWrapper.OnRequestWorldQuestsResult += new LegionCompanionWrapper.RequestWorldQuestsResultHandler(this.WorldQuestUpdateHandler);
			LegionCompanionWrapper.OnBountiesByWorldQuestUpdate += new LegionCompanionWrapper.BountiesByWorldQuestUpdateHandler(this.BountiesByWorldQuestUpdateHandler);
			LegionCompanionWrapper.OnRequestWorldQuestBountiesResult += new LegionCompanionWrapper.RequestWorldQuestBountiesResultHandler(this.WorldQuestBountiesResultHandler);
			LegionCompanionWrapper.OnRequestWorldQuestInactiveBountiesResult += new LegionCompanionWrapper.RequestWorldQuestInactiveBountiesResultHandler(this.WorldQuestInactiveBountiesResultHandler);
			LegionCompanionWrapper.OnGarrisonDataRequestResult += new LegionCompanionWrapper.GarrisonDataRequestResultHandler(this.GarrisonDataResultHandler);
			LegionCompanionWrapper.OnGarrisonStartMissionResult += new LegionCompanionWrapper.GarrisonStartMissionResultHandler(this.StartMissionResultHandler);
			LegionCompanionWrapper.OnGarrisonCompleteMissionResult += new LegionCompanionWrapper.GarrisonCompleteMissionResultHandler(this.CompleteMissionResultHandler);
			LegionCompanionWrapper.OnClaimMissionBonusResult += new LegionCompanionWrapper.ClaimMissionBonusResultHandler(this.ClaimMissionBonusResultHandler);
			LegionCompanionWrapper.OnMissionAdded += new LegionCompanionWrapper.MissionAddedHandler(this.MissionAddedHandler);
			LegionCompanionWrapper.OnFollowerChangedXp += new LegionCompanionWrapper.FollowerChangedXpHandler(this.FollowerChangedXPHandler);
			LegionCompanionWrapper.OnFollowerChangedQuality += new LegionCompanionWrapper.FollowerChangedQualityHandler(this.FollowerChangedQualityHandler);
			LegionCompanionWrapper.OnShipmentsUpdate += new LegionCompanionWrapper.ShipmentsUpdateHandler(this.ShipmentsUpdateHandler);
			LegionCompanionWrapper.OnUseFollowerArmamentResult += new LegionCompanionWrapper.UseFollowerArmamentResultHandler(this.UseFollowerArmamentResultHandler);
			LegionCompanionWrapper.OnChangeFollowerActiveResult += new LegionCompanionWrapper.ChangeFollowerActiveResultHandler(this.ChangeFollowerActiveResultHandler);
			LegionCompanionWrapper.OnRequestMaxFollowersResult += new LegionCompanionWrapper.RequestMaxFollowersResultHandler(this.RequestMaxFollowersResultHandler);
			LegionCompanionWrapper.OnFollowerArmamentsExtendedResult += new LegionCompanionWrapper.FollowerArmamentsExtendedResultHandler(this.FollowerArmamentsExtendedResultHandler);
			LegionCompanionWrapper.OnFollowerEquipmentResult += new LegionCompanionWrapper.FollowerEquipmentResultHandler(this.FollowerEquipmentResultHandler);
			LegionCompanionWrapper.OnEmissaryFactionsUpdate += new LegionCompanionWrapper.EmissaryFactionsUpdateHandler(this.EmissaryFactionUpdateHandler);
			LegionCompanionWrapper.OnCreateShipmentResult += new LegionCompanionWrapper.CreateShipmentResultHandler(this.CreateShipmentResultHandler);
			LegionCompanionWrapper.OnCompleteShipmentResult += new LegionCompanionWrapper.CompleteShipmentResultHandler(this.CompleteShipmentResultHandler);
			LegionCompanionWrapper.OnEvaluateMissionResult += new LegionCompanionWrapper.EvaluateMissionResultHandler(this.EvaluateMissionResultHandler);
			LegionCompanionWrapper.OnShipmentTypesResult += new LegionCompanionWrapper.ShipmentTypesResultHandler(this.ShipmentTypesHandler);
			LegionCompanionWrapper.OnCanResearchGarrisonTalentResult += new LegionCompanionWrapper.CanResearchGarrisonTalentResultHandler(this.CanResearchGarrisonTalentResultHandler);
			LegionCompanionWrapper.OnResearchGarrisonTalentResult += new LegionCompanionWrapper.ResearchGarrisonTalentResultHandler(this.ResearchGarrisonTalentResultHandler);
			LegionCompanionWrapper.OnFollowerActivationDataResult += new LegionCompanionWrapper.FollowerActivationDataResultHandler(this.FollowerActivationDataResultHandler);
			LegionCompanionWrapper.OnShipmentPushResult += new LegionCompanionWrapper.ShipmentPushResultHandler(this.ShipmentPushResultHandler);
			LegionCompanionWrapper.OnAreaPoiInfoResult += new LegionCompanionWrapper.AreaPoiInfoResultHandler(this.RequestAreaPoiInfoResultHandler);
			LegionCompanionWrapper.OnContributionInfoResult += new LegionCompanionWrapper.ContributionInfoResultHandler(this.RequestContributionInfoResultHandler);
			LegionCompanionWrapper.OnMakeContributionResult += new LegionCompanionWrapper.MakeContributionResultHandler(this.MakeContributionResultHandler);
			LegionCompanionWrapper.OnItemTooltipInfoResult += new LegionCompanionWrapper.ItemTooltipInfoResultHandler(this.GetItemTooltipInfoResultHandler);
			LegionCompanionWrapper.OnArtifactInfoResult += new LegionCompanionWrapper.ArtifactInfoResultHandler(this.ArtifactInfoResultHandler);
			LegionCompanionWrapper.OnPlayerLevelUp += new LegionCompanionWrapper.PlayerLevelUpHandler(this.PlayerLevelUpHandler);
			LegionCompanionWrapper.OnQuestCompleted += new LegionCompanionWrapper.QuestCompletedHandler(this.QuestCompletedHandler);
		}

		private void UnsubscribeFromEvents()
		{
			LegionCompanionWrapper.OnRequestWorldQuestsResult -= new LegionCompanionWrapper.RequestWorldQuestsResultHandler(this.WorldQuestUpdateHandler);
			LegionCompanionWrapper.OnBountiesByWorldQuestUpdate -= new LegionCompanionWrapper.BountiesByWorldQuestUpdateHandler(this.BountiesByWorldQuestUpdateHandler);
			LegionCompanionWrapper.OnRequestWorldQuestBountiesResult -= new LegionCompanionWrapper.RequestWorldQuestBountiesResultHandler(this.WorldQuestBountiesResultHandler);
			LegionCompanionWrapper.OnRequestWorldQuestInactiveBountiesResult -= new LegionCompanionWrapper.RequestWorldQuestInactiveBountiesResultHandler(this.WorldQuestInactiveBountiesResultHandler);
			LegionCompanionWrapper.OnGarrisonDataRequestResult -= new LegionCompanionWrapper.GarrisonDataRequestResultHandler(this.GarrisonDataResultHandler);
			LegionCompanionWrapper.OnGarrisonStartMissionResult -= new LegionCompanionWrapper.GarrisonStartMissionResultHandler(this.StartMissionResultHandler);
			LegionCompanionWrapper.OnGarrisonCompleteMissionResult -= new LegionCompanionWrapper.GarrisonCompleteMissionResultHandler(this.CompleteMissionResultHandler);
			LegionCompanionWrapper.OnClaimMissionBonusResult -= new LegionCompanionWrapper.ClaimMissionBonusResultHandler(this.ClaimMissionBonusResultHandler);
			LegionCompanionWrapper.OnMissionAdded -= new LegionCompanionWrapper.MissionAddedHandler(this.MissionAddedHandler);
			LegionCompanionWrapper.OnFollowerChangedXp -= new LegionCompanionWrapper.FollowerChangedXpHandler(this.FollowerChangedXPHandler);
			LegionCompanionWrapper.OnFollowerChangedQuality -= new LegionCompanionWrapper.FollowerChangedQualityHandler(this.FollowerChangedQualityHandler);
			LegionCompanionWrapper.OnShipmentsUpdate -= new LegionCompanionWrapper.ShipmentsUpdateHandler(this.ShipmentsUpdateHandler);
			LegionCompanionWrapper.OnUseFollowerArmamentResult -= new LegionCompanionWrapper.UseFollowerArmamentResultHandler(this.UseFollowerArmamentResultHandler);
			LegionCompanionWrapper.OnChangeFollowerActiveResult -= new LegionCompanionWrapper.ChangeFollowerActiveResultHandler(this.ChangeFollowerActiveResultHandler);
			LegionCompanionWrapper.OnRequestMaxFollowersResult -= new LegionCompanionWrapper.RequestMaxFollowersResultHandler(this.RequestMaxFollowersResultHandler);
			LegionCompanionWrapper.OnFollowerArmamentsExtendedResult -= new LegionCompanionWrapper.FollowerArmamentsExtendedResultHandler(this.FollowerArmamentsExtendedResultHandler);
			LegionCompanionWrapper.OnFollowerEquipmentResult -= new LegionCompanionWrapper.FollowerEquipmentResultHandler(this.FollowerEquipmentResultHandler);
			LegionCompanionWrapper.OnEmissaryFactionsUpdate -= new LegionCompanionWrapper.EmissaryFactionsUpdateHandler(this.EmissaryFactionUpdateHandler);
			LegionCompanionWrapper.OnCreateShipmentResult -= new LegionCompanionWrapper.CreateShipmentResultHandler(this.CreateShipmentResultHandler);
			LegionCompanionWrapper.OnCompleteShipmentResult -= new LegionCompanionWrapper.CompleteShipmentResultHandler(this.CompleteShipmentResultHandler);
			LegionCompanionWrapper.OnEvaluateMissionResult -= new LegionCompanionWrapper.EvaluateMissionResultHandler(this.EvaluateMissionResultHandler);
			LegionCompanionWrapper.OnShipmentTypesResult -= new LegionCompanionWrapper.ShipmentTypesResultHandler(this.ShipmentTypesHandler);
			LegionCompanionWrapper.OnCanResearchGarrisonTalentResult -= new LegionCompanionWrapper.CanResearchGarrisonTalentResultHandler(this.CanResearchGarrisonTalentResultHandler);
			LegionCompanionWrapper.OnResearchGarrisonTalentResult -= new LegionCompanionWrapper.ResearchGarrisonTalentResultHandler(this.ResearchGarrisonTalentResultHandler);
			LegionCompanionWrapper.OnFollowerActivationDataResult -= new LegionCompanionWrapper.FollowerActivationDataResultHandler(this.FollowerActivationDataResultHandler);
			LegionCompanionWrapper.OnShipmentPushResult -= new LegionCompanionWrapper.ShipmentPushResultHandler(this.ShipmentPushResultHandler);
			LegionCompanionWrapper.OnAreaPoiInfoResult -= new LegionCompanionWrapper.AreaPoiInfoResultHandler(this.RequestAreaPoiInfoResultHandler);
			LegionCompanionWrapper.OnContributionInfoResult -= new LegionCompanionWrapper.ContributionInfoResultHandler(this.RequestContributionInfoResultHandler);
			LegionCompanionWrapper.OnMakeContributionResult -= new LegionCompanionWrapper.MakeContributionResultHandler(this.MakeContributionResultHandler);
			LegionCompanionWrapper.OnItemTooltipInfoResult -= new LegionCompanionWrapper.ItemTooltipInfoResultHandler(this.GetItemTooltipInfoResultHandler);
			LegionCompanionWrapper.OnArtifactInfoResult -= new LegionCompanionWrapper.ArtifactInfoResultHandler(this.ArtifactInfoResultHandler);
			LegionCompanionWrapper.OnPlayerLevelUp -= new LegionCompanionWrapper.PlayerLevelUpHandler(this.PlayerLevelUpHandler);
			LegionCompanionWrapper.OnQuestCompleted -= new LegionCompanionWrapper.QuestCompletedHandler(this.QuestCompletedHandler);
		}

		private void Update()
		{
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
			this.m_frameCount++;
		}

		public void UseArmament(int garrFollowerID, int itemID)
		{
			if (this.UseArmamentStartAction != null)
			{
				this.UseArmamentStartAction(garrFollowerID);
			}
			GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(garrFollowerID);
			LegionCompanionWrapper.UseFollowerArmament((int)record.GarrFollowerTypeID, garrFollowerID, itemID);
		}

		public void UseEquipment(int garrFollowerID, int itemID, int replaceThisAbilityID)
		{
			if (this.UseEquipmentStartAction != null)
			{
				this.UseEquipmentStartAction(replaceThisAbilityID);
			}
			GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(garrFollowerID);
			LegionCompanionWrapper.UseFollowerEquipment((int)record.GarrFollowerTypeID, garrFollowerID, itemID, replaceThisAbilityID);
		}

		private void UseFollowerArmamentResultHandler(LegionCompanionWrapper.UseFollowerArmamentResultEvent eventArgs)
		{
			if (eventArgs.Result != (int)GARRISON_RESULT.SUCCESS)
			{
				AllPopups.instance.ShowGenericPopupFull(StaticDB.GetString("USE_ARMAMENT_FAILED", null));
			}
			else
			{
				PersistentFollowerData.AddOrUpdateFollower(eventArgs.Follower);
				LegionCompanionWrapper.RequestFollowerArmamentsExtended((int)GarrisonStatus.GarrisonFollowerType);
			}
			if (this.UseArmamentResultAction != null)
			{
				this.UseArmamentResultAction(eventArgs.Result, eventArgs.OldFollower, eventArgs.Follower);
			}
		}

		private void WorldQuestBountiesResultHandler(LegionCompanionWrapper.RequestWorldQuestBountiesResultEvent eventArgs)
		{
			PersistentBountyData.ClearData();
			PersistentBountyData.SetBountiesVisible(eventArgs.Visible);
			if (eventArgs.Visible)
			{
			}
			eventArgs.LockedQuestID == 0;
			for (int i = 0; i < eventArgs.Bounties.Count; i++)
			{
				PersistentBountyData.AddOrUpdateBounty(eventArgs.Bounties[i]);
			}
			if (this.BountyInfoUpdatedAction != null)
			{
				this.BountyInfoUpdatedAction();
			}
		}

		private void WorldQuestInactiveBountiesResultHandler(LegionCompanionWrapper.RequestWorldQuestInactiveBountiesResultEvent eventArgs)
		{
		}

		private void WorldQuestUpdateHandler(LegionCompanionWrapper.RequestWorldQuestsResultEvent eventArgs)
		{
			WorldQuestData.ClearData();
			foreach (WrapperWorldQuest quest in eventArgs.Quests)
			{
				if (!Main.DoesMapIDSupportWorldQuests(quest.StartLocationMapID))
				{
					continue;
				}
				WorldQuestData.AddWorldQuest(quest);
				for (int i = 0; i < quest.Items.Count; i++)
				{
					ItemStatCache itemStatCache = ItemStatCache.instance;
					int recordID = quest.Items[i].RecordID;
					WrapperWorldQuestReward item = quest.Items[i];
					itemStatCache.GetItemStats(recordID, item.ItemContext);
				}
			}
		}

		public interface SetCache
		{
			void SetCachePath(string cachePath);
		}
	}
}