using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class AdventureMapPanel : MonoBehaviour
	{
		public bool m_testEnableDetailedZoneMaps;

		public bool m_testEnableAutoZoomInOut;

		public bool m_testEnableTapToZoomOut;

		public float m_testMissionIconScale;

		public Action<float> TestIconSizeChanged;

		public Camera m_mainCamera;

		public PinchZoomContentManager m_pinchZoomContentManager;

		public RectTransform m_mapViewRT;

		public RectTransform m_mapAndRewardParentViewRT;

		public RectTransform m_mapViewContentsRT;

		public MapInfo m_mainMapInfo;

		public GameObject m_AdvMapMissionSitePrefab;

		public GameObject m_AdvMapWorldQuestPrefab;

		public GameObject m_bountySitePrefab;

		public MapInfo m_mapInfo_KulTiras;

		public MapInfo m_mapInfo_Zandalar;

		public GameObject m_missionAndWorldQuestArea_KulTiras;

		public GameObject m_missionAndWorldQuestArea_Zandalar;

		public EmissaryCollection m_emissaryCollection;

		public ZoneMissionOverview[] m_allZoneMissionOverviews;

		public GameObject m_missionRewardResultsDisplayPrefab;

		public GameObject m_zoneLabel;

		public Text m_zoneLabelText;

		public AdventureMapPanel.eZone m_zoneID;

		public static AdventureMapPanel instance;

		public ZoneButton m_lastTappedZoneButton;

		public ZoneButton m_currentVisibleZone;

		public OrderHallNavButton m_adventureMapOrderHallNavButton;

		public Image m_selectedMapImage;

		public Image m_notSelectedMapImage;

		public Sprite m_kultirasNavButtonImage;

		public Sprite m_zandalarNavButtonImage;

		private int m_currentMapMission;

		public Action<int> MissionSelectedFromMapAction;

		public Action<int> MissionMapSelectionChangedAction;

		private int m_currentListMission;

		public Action<int> MissionSelectedFromListAction;

		private int m_currentWorldQuest;

		public Action<int> WorldQuestChangedAction;

		public Action OnZoomOutMap;

		public Action<int> OnAddMissionLootToRewardPanel;

		public Action<bool> OnShowMissionRewardPanel;

		public Action OnInitMissionSites;

		public Action MapFiltersChanged;

		private int m_followerToInspect;

		public Action<int> FollowerToInspectChangedAction;

		public Action DeselectAllFollowerListItemsAction;

		public Action<bool> OnShowFollowerDetails;

		public Action<int, bool> OnMissionFollowerSlotChanged;

		public Action<int, int, bool> ShowMissionResultAction;

		private StackableMapIconContainer m_iconContainer;

		public Action<StackableMapIconContainer> SelectedIconContainerChanged;

		public float m_secondsMissionHasBeenSelected;

		public CanvasGroup m_topLevelMapCanvasGroup;

		public PlayerInfoDisplay m_playerInfoDisplay;

		private bool[] m_mapFilters;

		public MissionResultsPanel m_missionResultsPanel;

		public NextCompletedMissionButton m_nextCompletedMissionButton;

		public AdventureMapPanel()
		{
		}

		public void AddMissionLootToRewardPanel(int garrMissionID)
		{
			if (this.OnAddMissionLootToRewardPanel != null)
			{
				this.OnAddMissionLootToRewardPanel(garrMissionID);
			}
		}

		private void Awake()
		{
			AdventureMapPanel.instance = this;
			this.m_zoneID = AdventureMapPanel.eZone.None;
			this.m_testMissionIconScale = 1f;
			this.m_mapFilters = new bool[18];
			for (int i = 0; i < (int)this.m_mapFilters.Length; i++)
			{
				this.m_mapFilters[i] = false;
			}
			this.EnableMapFilter(MapFilterType.All, true);
			if (this.m_missionResultsPanel)
			{
				this.m_missionResultsPanel.gameObject.SetActive(true);
			}
		}

		public void CenterAndZoom(Vector2 tapPos, ZoneButton zoneButton, bool zoomIn)
		{
			Vector2 vector2 = new Vector2();
			Vector2 vector21 = new Vector2();
			iTween.Stop(this.m_mapViewContentsRT.gameObject);
			iTween.Stop(base.gameObject);
			this.m_lastTappedZoneButton = zoneButton;
			Vector3[] vector3Array = new Vector3[4];
			this.m_mapViewRT.GetWorldCorners(vector3Array);
			float single = vector3Array[2].x - vector3Array[0].x;
			float single1 = vector3Array[2].y - vector3Array[0].y;
			vector2.x = vector3Array[0].x + single * 0.5f;
			vector2.y = vector3Array[0].y + single1 * 0.5f;
			Vector3[] vector3Array1 = new Vector3[4];
			this.m_mapViewContentsRT.GetWorldCorners(vector3Array1);
			float single2 = vector3Array1[2].x - vector3Array1[0].x;
			float single3 = vector3Array1[2].y - vector3Array1[0].y;
			vector21.x = vector3Array1[0].x + single2 * 0.5f;
			vector21.y = vector3Array1[0].y + single3 * 0.5f;
			MapInfo componentInChildren = base.GetComponentInChildren<MapInfo>();
			if (componentInChildren == null)
			{
				return;
			}
			if (!zoomIn)
			{
				if (this.OnZoomOutMap != null)
				{
					Main.instance.m_UISound.Play_MapZoomOut();
					this.OnZoomOutMap();
				}
				iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "Zoom View Out", "from", this.m_pinchZoomContentManager.m_zoomFactor, "to", componentInChildren.m_minZoomFactor, "easeType", "easeOutCubic", "time", 0.8f, "onupdate", "ZoomOutTweenCallback" }));
				iTween.MoveTo(this.m_mapViewContentsRT.gameObject, iTween.Hash(new object[] { "name", "Pan View To Point (out)", "x", vector2.x, "y", vector2.y, "easeType", "easeOutQuad", "time", 0.8f }));
			}
			else
			{
				if (this.m_pinchZoomContentManager.m_zoomFactor < 1.001f)
				{
					Main.instance.m_UISound.Play_MapZoomIn();
				}
				Vector2 mMaxZoomFactor = tapPos - vector21;
				mMaxZoomFactor = mMaxZoomFactor * (componentInChildren.m_maxZoomFactor / this.m_pinchZoomContentManager.m_zoomFactor);
				Vector2 vector22 = vector21 + mMaxZoomFactor;
				iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "Zoom View In", "from", this.m_pinchZoomContentManager.m_zoomFactor, "to", componentInChildren.m_maxZoomFactor, "easeType", "easeOutCubic", "time", 0.8f, "onupdate", "ZoomInTweenCallback" }));
				iTween.MoveBy(this.m_mapViewContentsRT.gameObject, iTween.Hash(new object[] { "name", "Pan View To Point (in)", "x", vector2.x - vector22.x, "y", vector2.y - vector22.y, "easeType", "easeOutQuad", "time", 0.8f }));
			}
		}

		public void CenterAndZoomIn()
		{
			if (Input.touchCount != 1)
			{
				return;
			}
			Touch touch = Input.GetTouch(0);
			this.CenterAndZoom(touch.position, null, true);
		}

		public void CenterAndZoomOut()
		{
			if (this.m_adventureMapOrderHallNavButton && this.m_adventureMapOrderHallNavButton.IsSelected())
			{
				this.CenterAndZoom(Vector2.zero, null, false);
			}
		}

		public void CenterMapInstantly()
		{
			this.m_mapViewContentsRT.anchoredPosition = Vector2.zero;
			this.m_mapViewRT.GetComponent<ScrollRect>().StopMovement();
		}

		private static void ClearWorldQuestArea(GameObject questArea)
		{
			AdventureMapWorldQuest[] componentsInChildren = questArea.GetComponentsInChildren<AdventureMapWorldQuest>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				AdventureMapWorldQuest adventureMapWorldQuest = componentsInChildren[i];
				StackableMapIcon component = adventureMapWorldQuest.GetComponent<StackableMapIcon>();
				GameObject gameObject = adventureMapWorldQuest.gameObject;
				if (component != null)
				{
					component.RemoveFromContainer();
				}
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(adventureMapWorldQuest.gameObject);
				}
			}
		}

		private void CreateMissionSite(int garrMissionID)
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
			if (record == null)
			{
				Debug.LogWarning(string.Concat("Mission Not Found: ID ", garrMissionID));
				return;
			}
			if (record.GarrFollowerTypeID != (uint)GarrisonStatus.GarrisonFollowerType)
			{
				return;
			}
			if ((record.Flags & 16) != 0)
			{
				return;
			}
			if (!PersistentMissionData.missionDictionary.ContainsKey(garrMissionID))
			{
				return;
			}
			if (PersistentMissionData.missionDictionary[garrMissionID].MissionState == 0)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(AdventureMapPanel.instance.m_AdvMapMissionSitePrefab);
			gameObject.transform.SetParent(this.m_mapViewContentsRT, false);
			float single = 1.84887111f;
			float mapposX = (float)record.Mappos_x * single;
			float mapposY = (float)record.Mappos_y * -single;
			float single1 = -272.5694f;
			float single2 = 1318.388f;
			mapposX += single1;
			mapposY += single2;
			Vector2 vector3 = new Vector3(mapposX / 1f, mapposY / 1f);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = vector3;
			component.anchorMax = vector3;
			component.anchoredPosition = Vector2.zero;
			gameObject.GetComponent<AdventureMapMissionSite>().SetMission(record.ID);
			StackableMapIcon stackableMapIcon = gameObject.GetComponent<StackableMapIcon>();
			if (stackableMapIcon != null)
			{
				stackableMapIcon.RegisterWithManager(record.AreaID);
			}
		}

		public void DeselectAllFollowerListItems()
		{
			if (this.DeselectAllFollowerListItemsAction != null)
			{
				this.DeselectAllFollowerListItemsAction();
			}
		}

		public void EnableMapFilter(MapFilterType mapFilterType, bool enable)
		{
			for (int i = 0; i < (int)this.m_mapFilters.Length; i++)
			{
				this.m_mapFilters[i] = false;
			}
			this.m_mapFilters[(int)mapFilterType] = true;
			AllPopups.instance.m_optionsDialog.SyncWithOptions();
			if (this.MapFiltersChanged != null)
			{
				this.MapFiltersChanged();
			}
		}

		public int GetCurrentListMission()
		{
			return this.m_currentListMission;
		}

		public int GetCurrentMapMission()
		{
			return this.m_currentMapMission;
		}

		public int GetCurrentWorldQuest()
		{
			return this.m_currentWorldQuest;
		}

		public int GetFollowerToInspect()
		{
			return this.m_followerToInspect;
		}

		private static int GetImageWByMapID(int startLocationMapId)
		{
			if (startLocationMapId == 1642)
			{
				return 1985;
			}
			if (startLocationMapId == 1643)
			{
				return 1985;
			}
			return 0;
		}

		public StackableMapIconContainer GetSelectedIconContainer()
		{
			return this.m_iconContainer;
		}

		public void HandleBountyInfoUpdated()
		{
			if (this.m_mapViewContentsRT != null)
			{
				BountySite[] componentsInChildren = this.m_mapViewContentsRT.GetComponentsInChildren<BountySite>(true);
				if (componentsInChildren != null)
				{
					BountySite[] bountySiteArray = componentsInChildren;
					for (int i = 0; i < (int)bountySiteArray.Length; i++)
					{
						BountySite bountySite = bountySiteArray[i];
						StackableMapIcon component = bountySite.GetComponent<StackableMapIcon>();
						GameObject gameObject = bountySite.gameObject;
						if (component != null)
						{
							component.RemoveFromContainer();
						}
						if (gameObject != null)
						{
							UnityEngine.Object.Destroy(gameObject);
						}
					}
				}
			}
			if (PersistentBountyData.bountyDictionary == null || this.m_emissaryCollection == null)
			{
				return;
			}
			this.m_emissaryCollection.ClearCollection();
			foreach (WrapperWorldQuestBounty value in PersistentBountyData.bountyDictionary.Values)
			{
				QuestV2Rec record = StaticDB.questDB.GetRecord(value.QuestID);
				int num = (record == null ? 0 : record.QuestSortID);
				if (record == null)
				{
					Debug.LogWarning(string.Concat("HandleBountyInfoUpdated Warning: Failed to get Bounty quest with ID ", value.QuestID.ToString()));
				}
				else if (num != 7502 && num != 7503)
				{
					switch (num)
					{
						case 7541:
						case 7543:
						{
							break;
						}
						default:
						{
							if (num == 7334 || num == 7558 || num == 7637 || num == 8147 || num == 8574 || num == 8701)
							{
								break;
							}
							GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_bountySitePrefab);
							if (gameObject1 != null)
							{
								BountySite component1 = gameObject1.GetComponent<BountySite>();
								if (component1 != null)
								{
									component1.SetBounty(value);
									gameObject1.name = string.Concat("BountySite ", value.QuestID);
									RectTransform vector2 = gameObject1.GetComponent<RectTransform>();
									if (vector2 != null)
									{
										vector2.anchorMin = new Vector2(0.5f, 0.5f);
										vector2.anchorMax = new Vector2(0.5f, 0.5f);
										this.m_emissaryCollection.AddBountyObjectToCollection(gameObject1);
										continue;
									}
									else
									{
										continue;
									}
								}
								else
								{
									continue;
								}
							}
							else
							{
								continue;
							}
						}
					}
				}
			}
		}

		private void HandleMissionAdded(int garrMissionID, int result)
		{
		}

		public void HideRecentCharacterPanel()
		{
			this.m_playerInfoDisplay.HideRecentCharacterPanel();
		}

		private void InitMissionSites()
		{
			if (this.OnInitMissionSites != null)
			{
				this.OnInitMissionSites();
			}
			AdventureMapMissionSite[] componentsInChildren = this.m_mapViewContentsRT.GetComponentsInChildren<AdventureMapMissionSite>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				AdventureMapMissionSite adventureMapMissionSite = componentsInChildren[i];
				if (adventureMapMissionSite != null)
				{
					StackableMapIcon component = adventureMapMissionSite.GetComponent<StackableMapIcon>();
					GameObject gameObject = adventureMapMissionSite.gameObject;
					if (component != null)
					{
						component.RemoveFromContainer();
					}
					if (gameObject != null)
					{
						UnityEngine.Object.Destroy(adventureMapMissionSite.gameObject);
					}
				}
			}
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				this.CreateMissionSite(value.MissionRecID);
			}
		}

		public bool IsFilterEnabled(MapFilterType mapFilterType)
		{
			return this.m_mapFilters[(int)mapFilterType];
		}

		public void MissionFollowerSlotChanged(int garrFollowerID, bool inParty)
		{
			if (this.OnMissionFollowerSlotChanged != null)
			{
				this.OnMissionFollowerSlotChanged(garrFollowerID, inParty);
			}
		}

		private void OnDisable()
		{
			AdventureMapPanel.instance = this;
			this.MapFiltersChanged -= new Action(this.UpdateWorldQuests);
		}

		private void OnEnable()
		{
			AdventureMapPanel.instance = this;
			this.MapFiltersChanged += new Action(this.UpdateWorldQuests);
		}

		public Vector2 ScreenPointToLocalPointInMapViewRT(Vector2 screenPoint)
		{
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_mapViewRT, screenPoint, this.m_mainCamera, out vector2);
			return vector2;
		}

		public void SelectMissionFromList(int garrMissionID)
		{
			this.m_currentListMission = garrMissionID;
			if (this.MissionSelectedFromListAction != null)
			{
				this.MissionSelectedFromListAction(garrMissionID);
			}
		}

		public void SelectMissionFromMap(int garrMissionID)
		{
			if (this.m_currentMapMission != garrMissionID)
			{
				this.m_secondsMissionHasBeenSelected = 0f;
				this.m_currentMapMission = garrMissionID;
				if (this.MissionMapSelectionChangedAction != null)
				{
					this.MissionMapSelectionChangedAction(this.m_currentMapMission);
				}
			}
			if (this.MissionSelectedFromMapAction != null)
			{
				this.MissionSelectedFromMapAction(this.m_currentMapMission);
			}
			if (garrMissionID > 0)
			{
				this.SelectWorldQuest(0);
			}
		}

		public void SelectWorldQuest(int worldQuestID)
		{
			this.m_currentWorldQuest = worldQuestID;
			if (this.WorldQuestChangedAction != null)
			{
				this.WorldQuestChangedAction(this.m_currentWorldQuest);
			}
			if (worldQuestID > 0)
			{
				this.SelectMissionFromMap(0);
			}
		}

		private void SetActiveMapViewSize()
		{
			if (this.m_zoneID == AdventureMapPanel.eZone.Kultiras)
			{
				this.SetMapViewSize_KulTiras();
			}
			else if (this.m_zoneID == AdventureMapPanel.eZone.Zandalar)
			{
				this.SetMapViewSize_Zandalar();
			}
		}

		public void SetFollowerToInspect(int garrFollowerID)
		{
			this.m_followerToInspect = garrFollowerID;
			if (this.FollowerToInspectChangedAction != null)
			{
				this.FollowerToInspectChangedAction(garrFollowerID);
			}
		}

		private void SetMapByActiveZoneID()
		{
			if (this.m_zoneID == AdventureMapPanel.eZone.Zandalar)
			{
				this.m_mapInfo_Zandalar.gameObject.SetActive(true);
				this.m_mapInfo_KulTiras.gameObject.SetActive(false);
				this.m_selectedMapImage.sprite = this.m_zandalarNavButtonImage;
				this.m_notSelectedMapImage.sprite = this.m_zandalarNavButtonImage;
			}
			if (this.m_zoneID == AdventureMapPanel.eZone.Kultiras)
			{
				this.m_mapInfo_Zandalar.gameObject.SetActive(false);
				this.m_mapInfo_KulTiras.gameObject.SetActive(true);
				this.m_selectedMapImage.sprite = this.m_kultirasNavButtonImage;
				this.m_notSelectedMapImage.sprite = this.m_kultirasNavButtonImage;
			}
			string str = (this.m_zoneID != AdventureMapPanel.eZone.Zandalar ? "KUL_TIRAS_CAPS" : "ZANDALAR_CAPS");
			string str1 = str;
			this.m_zoneLabelText.text = StaticDB.GetString(str, string.Concat("[PH] ", str1));
			this.SetActiveMapViewSize();
		}

		private void SetMapViewSize_Argus()
		{
			RectTransform mMapViewRT = this.m_mapViewRT;
			Vector2 vector2 = this.m_mapViewRT.sizeDelta;
			mMapViewRT.sizeDelta = new Vector2(vector2.x, 720f);
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 100f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		private void SetMapViewSize_BrokenIsles()
		{
			RectTransform mMapViewRT = this.m_mapViewRT;
			Vector2 vector2 = this.m_mapViewRT.sizeDelta;
			mMapViewRT.sizeDelta = new Vector2(vector2.x, 720f);
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 0f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		private void SetMapViewSize_KulTiras()
		{
			RectTransform mMapViewRT = this.m_mapViewRT;
			Vector2 vector2 = this.m_mapViewRT.sizeDelta;
			mMapViewRT.sizeDelta = new Vector2(vector2.x, 660f);
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 110f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		private void SetMapViewSize_Zandalar()
		{
			RectTransform mMapViewRT = this.m_mapViewRT;
			Vector2 vector2 = this.m_mapViewRT.sizeDelta;
			mMapViewRT.sizeDelta = new Vector2(vector2.x, 660f);
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 110f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		public void SetMissionIconScale(float val)
		{
			this.m_testMissionIconScale = val;
			if (this.TestIconSizeChanged != null)
			{
				this.TestIconSizeChanged(this.m_testMissionIconScale);
			}
		}

		public void SetSelectedIconContainer(StackableMapIconContainer container)
		{
			this.m_iconContainer = container;
			if (this.SelectedIconContainerChanged != null)
			{
				this.SelectedIconContainerChanged(container);
			}
		}

		public void SetStartingMapByFaction()
		{
			if (GarrisonStatus.Faction() == PVP_FACTION.HORDE)
			{
				this.m_zoneID = AdventureMapPanel.eZone.Zandalar;
			}
			else if (GarrisonStatus.Faction() == PVP_FACTION.ALLIANCE)
			{
				this.m_zoneID = AdventureMapPanel.eZone.Kultiras;
			}
			this.SetMapByActiveZoneID();
		}

		private void SetupWorldQuestIcon(WrapperWorldQuest worldQuest, GameObject worldQuestObj, float mapOffsetX, float mapOffsetY, float mapScale)
		{
			float startLocationY = (float)worldQuest.StartLocationY * -mapScale;
			float startLocationX = (float)worldQuest.StartLocationX * mapScale;
			startLocationY += mapOffsetX;
			startLocationX += mapOffsetY;
			Vector2 vector3 = new Vector3(startLocationY / (float)AdventureMapPanel.GetImageWByMapID(worldQuest.StartLocationMapID), startLocationX / 1334f);
			RectTransform component = worldQuestObj.GetComponent<RectTransform>();
			component.anchorMin = vector3;
			component.anchorMax = vector3;
			component.anchoredPosition = Vector2.zero;
		}

		public void ShowFollowerDetails(bool show)
		{
			if (this.OnShowFollowerDetails != null)
			{
				this.OnShowFollowerDetails(show);
			}
		}

		public void ShowRewardPanel(bool show)
		{
			if (this.OnShowMissionRewardPanel != null)
			{
				this.OnShowMissionRewardPanel(show);
			}
		}

		public void ShowWorldMap(bool show)
		{
			this.m_mainMapInfo.gameObject.SetActive(show);
		}

		private void Start()
		{
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			StackableMapIconContainer[] componentsInChildren = this.m_mapViewContentsRT.GetComponentsInChildren<StackableMapIconContainer>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			this.InitMissionSites();
			this.UpdateWorldQuests();
			this.HandleBountyInfoUpdated();
			Main.instance.GarrisonDataResetFinishedAction += new Action(this.InitMissionSites);
			Main.instance.MissionAddedAction += new Action<int, int>(this.HandleMissionAdded);
			Main.instance.BountyInfoUpdatedAction += new Action(this.HandleBountyInfoUpdated);
		}

		public void SwapMaps()
		{
			this.m_zoneID = (this.m_zoneID != AdventureMapPanel.eZone.Zandalar ? AdventureMapPanel.eZone.Zandalar : AdventureMapPanel.eZone.Kultiras);
			this.SetMapByActiveZoneID();
		}

		private void Update()
		{
			this.m_currentVisibleZone = null;
			if (this.m_currentMapMission > 0)
			{
				this.m_secondsMissionHasBeenSelected += Time.deltaTime;
			}
			this.UpdateCompletedMissionsDisplay();
		}

		private void UpdateCompletedMissionsDisplay()
		{
			if (PersistentMissionData.GetNumCompletedMissions(false) > 0)
			{
				this.m_nextCompletedMissionButton.gameObject.SetActive(true);
			}
		}

		public void UpdateWorldQuests()
		{
			bool flag;
			bool flag1;
			bool flag2;
			bool flag3;
			bool flag4;
			bool flag5;
			bool flag6;
			bool flag7;
			bool flag8;
			bool flag9;
			AdventureMapPanel.ClearWorldQuestArea(this.m_missionAndWorldQuestArea_KulTiras);
			AdventureMapPanel.ClearWorldQuestArea(this.m_missionAndWorldQuestArea_Zandalar);
			foreach (WrapperWorldQuest value in WorldQuestData.WorldQuestDictionary.Values)
			{
				if (value.StartLocationMapID != 1220 && value.StartLocationMapID != 1669)
				{
					if (!this.IsFilterEnabled(MapFilterType.All))
					{
						bool questInfoID = false;
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Azerite))
						{
							questInfoID = questInfoID | value.Currencies.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => reward.RecordID == 1553);
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.OrderResources))
						{
							questInfoID = questInfoID | value.Currencies.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => reward.RecordID == 1560);
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Gold) && value.Money > 0)
						{
							questInfoID = true;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Gear))
						{
							questInfoID = questInfoID | value.Items.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => {
								ItemRec record = StaticDB.itemDB.GetRecord(reward.RecordID);
								return (record == null ? false : (record.ClassID == 2 || record.ClassID == 3 || record.ClassID == 4 ? true : record.ClassID == 6));
							});
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.ProfessionMats))
						{
							questInfoID = questInfoID | value.Items.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => {
								ItemRec record = StaticDB.itemDB.GetRecord(reward.RecordID);
								return (record == null ? false : record.ClassID == 7);
							});
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.PetBattles))
						{
							questInfoID = questInfoID | value.QuestInfoID == 115;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Reputation))
						{
							questInfoID = questInfoID | value.Currencies.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => {
								CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(reward.RecordID);
								return (record == null ? false : record.FactionID != 0);
							});
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_ChampionsOfAzeroth))
						{
							bool flag10 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag9 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest item = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag9 = item.BountyQuestIDs.Any<int>((int questID) => questID == 50562);
							}
							questInfoID = flag10 | flag9;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_ZandalariEmpire))
						{
							bool flag11 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag8 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag8 = wrapperBountiesByWorldQuest.BountyQuestIDs.Any<int>((int questID) => questID == 50598);
							}
							questInfoID = flag11 | flag8;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_ProudmooreAdmiralty))
						{
							bool flag12 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag7 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest item1 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag7 = item1.BountyQuestIDs.Any<int>((int questID) => questID == 50599);
							}
							questInfoID = flag12 | flag7;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_OrderOfEmbers))
						{
							bool flag13 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag6 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest1 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag6 = wrapperBountiesByWorldQuest1.BountyQuestIDs.Any<int>((int questID) => questID == 50600);
							}
							questInfoID = flag13 | flag6;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_StormsWake))
						{
							bool flag14 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag5 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest item2 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag5 = item2.BountyQuestIDs.Any<int>((int questID) => questID == 50601);
							}
							questInfoID = flag14 | flag5;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_TalanjisExpedition))
						{
							bool flag15 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag4 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest2 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag4 = wrapperBountiesByWorldQuest2.BountyQuestIDs.Any<int>((int questID) => questID == 50602);
							}
							questInfoID = flag15 | flag4;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_Voldunai))
						{
							bool flag16 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag3 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest item3 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag3 = item3.BountyQuestIDs.Any<int>((int questID) => questID == 50603);
							}
							questInfoID = flag16 | flag3;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_TortollanSeekers))
						{
							bool flag17 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag2 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest3 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag2 = wrapperBountiesByWorldQuest3.BountyQuestIDs.Any<int>((int questID) => questID == 50604);
							}
							questInfoID = flag17 | flag2;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_AllianceWarEffort))
						{
							bool flag18 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag1 = false;
							}
							else
							{
								WrapperBountiesByWorldQuest item4 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag1 = item4.BountyQuestIDs.Any<int>((int questID) => questID == 50605);
							}
							questInfoID = flag18 | flag1;
						}
						if (!questInfoID && this.IsFilterEnabled(MapFilterType.Bounty_HordeWarEffort))
						{
							bool flag19 = questInfoID;
							if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
							{
								flag = false;
							}
							else
							{
								WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest4 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
								flag = wrapperBountiesByWorldQuest4.BountyQuestIDs.Any<int>((int questID) => questID == 50606);
							}
							questInfoID = flag19 | flag;
						}
						if (!questInfoID)
						{
							continue;
						}
					}
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(AdventureMapPanel.instance.m_AdvMapWorldQuestPrefab);
					if (value.StartLocationMapID == 1642)
					{
						gameObject.transform.SetParent(this.m_missionAndWorldQuestArea_Zandalar.transform, false);
						float single = 0.152715057f;
						float single1 = 1250.88025f;
						float single2 = 697.2115f;
						if (value.WorldMapAreaID == 863)
						{
							single -= 0.02f;
						}
						else if (value.WorldMapAreaID == 864)
						{
							single1 += 60f;
							single2 -= 20f;
						}
						this.SetupWorldQuestIcon(value, gameObject, single1, single2, single);
					}
					else if (value.StartLocationMapID == 1643)
					{
						gameObject.transform.SetParent(this.m_missionAndWorldQuestArea_KulTiras.transform, false);
						this.SetupWorldQuestIcon(value, gameObject, 1150.88025f, 497.2115f, 0.152715057f);
					}
					gameObject.GetComponent<AdventureMapWorldQuest>().SetQuestID(value.QuestID);
					StackableMapIcon component = gameObject.GetComponent<StackableMapIcon>();
					if (component == null)
					{
						continue;
					}
					component.RegisterWithManager(value.StartLocationMapID);
				}
			}
			this.m_pinchZoomContentManager.ForceZoomFactorChanged();
		}

		private void ZoomInTweenCallback(float newZoomFactor)
		{
			this.m_pinchZoomContentManager.SetZoom(newZoomFactor, false);
		}

		private void ZoomOutTweenCallback(float newZoomFactor)
		{
			this.m_pinchZoomContentManager.SetZoom(newZoomFactor, true);
		}

		public enum eZone
		{
			Azsuna,
			BrokenShore,
			HighMountain,
			Stormheim,
			Suramar,
			ValShara,
			Argus,
			MacAree,
			StygianWake,
			Drustvar,
			TiragardeSound,
			StormsongValley,
			Zuldazar,
			VolDun,
			Nazmir,
			Zandalar,
			Kultiras,
			None,
			NumZones
		}
	}
}