using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowJamMessages.MobileClientJSON;
using WowStatConstants;
using WowStaticData;

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

	public Image m_worldMapLowDetail_BrokenIsles;

	public Image m_worldMapLowDetail_Argus;

	public MapInfo m_mainMapInfo;

	public GameObject m_AdvMapMissionSitePrefab;

	public GameObject m_AdvMapWorldQuestPrefab;

	public GameObject m_bountySitePrefab;

	public MapInfo m_mapInfo_BrokenIsles;

	public MapInfo m_mapInfo_Argus;

	public GameObject m_missionAndWorldQuestArea_BrokenIsles;

	public GameObject m_missionAndWorldQuestArea_Argus;

	public ZoneMissionOverview[] m_allZoneMissionOverviews;

	public GameObject m_missionRewardResultsDisplayPrefab;

	public GameObject m_zoneLabel;

	public Text m_zoneLabelText;

	public AdventureMapPanel.eZone m_zoneID;

	public static AdventureMapPanel instance;

	public ZoneButton m_lastTappedZoneButton;

	public ZoneButton m_currentVisibleZone;

	public OrderHallNavButton m_adventureMapOrderHallNavButton;

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

	public RectTransform m_invasionNotification;

	public Text m_invasionTitle;

	public Text m_invasionTimeRemaining;

	private Duration m_invasionTimeRemainingDuration;

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
		this.m_invasionTimeRemainingDuration = new Duration(0, false);
		AdventureMapPanel.instance = this;
		this.m_zoneID = AdventureMapPanel.eZone.None;
		this.m_testMissionIconScale = 1f;
		this.m_mapFilters = new bool[15];
		for (int i = 0; i < (int)this.m_mapFilters.Length; i++)
		{
			this.m_mapFilters[i] = false;
		}
		this.EnableMapFilter(MapFilterType.All, true);
		AllPanels.instance.m_missionResultsPanel.gameObject.SetActive(true);
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
		if (this.m_adventureMapOrderHallNavButton.IsSelected())
		{
			this.CenterAndZoom(Vector2.zero, null, false);
		}
	}

	public void CenterMapInstantly()
	{
		this.m_mapViewContentsRT.anchoredPosition = Vector2.zero;
		this.m_mapViewRT.GetComponent<ScrollRect>().StopMovement();
	}

	private void CreateMissionSite(int garrMissionID)
	{
		GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
		if (record == null)
		{
			Debug.LogWarning(string.Concat("Mission Not Found: ID ", garrMissionID));
			return;
		}
		if (record.GarrFollowerTypeID != 4)
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
		if (((JamGarrisonMobileMission)PersistentMissionData.missionDictionary[garrMissionID]).MissionState == 0)
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
		float mWorldMapLowDetailBrokenIsles = this.m_worldMapLowDetail_BrokenIsles.sprite.textureRect.width;
		float mWorldMapLowDetailBrokenIsles1 = this.m_worldMapLowDetail_BrokenIsles.sprite.textureRect.height;
		Vector2 vector3 = new Vector3(mapposX / mWorldMapLowDetailBrokenIsles, mapposY / mWorldMapLowDetailBrokenIsles1);
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
						UnityEngine.Object.DestroyImmediate(gameObject);
					}
				}
			}
		}
		if (PersistentBountyData.bountyDictionary == null)
		{
			return;
		}
		IEnumerator enumerator = PersistentBountyData.bountyDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileWorldQuestBounty current = (MobileWorldQuestBounty)enumerator.Current;
				GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(this.m_bountySitePrefab);
				if (vector3 != null)
				{
					BountySite component1 = vector3.GetComponent<BountySite>();
					if (component1 != null)
					{
						component1.SetBounty(current);
						vector3.name = string.Concat("BountySite ", current.QuestID);
						RectTransform vector2 = vector3.GetComponent<RectTransform>();
						if (vector2 != null)
						{
							vector2.anchorMin = new Vector2(0.5f, 0.5f);
							vector2.anchorMax = new Vector2(0.5f, 0.5f);
							QuestV2Rec record = StaticDB.questDB.GetRecord(current.QuestID);
							int num = (record == null ? 0 : record.QuestSortID);
							bool flag = true;
							ZoneMissionOverview mAllZoneMissionOverviews = null;
							int num1 = 1220;
							int num2 = num;
							switch (num2)
							{
								case 7541:
								{
									mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[3];
									break;
								}
								case 7543:
								{
									mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[1];
									break;
								}
								default:
								{
									if (num2 != 7502)
									{
										if (num2 == 7503)
										{
											mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[2];
											break;
										}
										else if (num2 == 7334)
										{
											mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[0];
											break;
										}
										else if (num2 == 7558)
										{
											mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[5];
											break;
										}
										else if (num2 == 7637)
										{
											mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[4];
											break;
										}
										else if (num2 != 8147)
										{
											if (num2 == 8574)
											{
												mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[7];
												break;
											}
											else if (num2 == 8701)
											{
												mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[9];
												break;
											}
											else
											{
												Debug.LogError(string.Concat(new object[] { "INVALID QUESTSORTID ", num, " for quest ID:", current.QuestID }));
												flag = false;
												break;
											}
										}
									}
									mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[6];
									break;
								}
							}
							if (!flag)
							{
								vector3.transform.localPosition = new Vector3(0f, 0f, 0f);
								if (component1.m_errorImage != null && component1.m_errorImage.gameObject != null)
								{
									component1.m_errorImage.gameObject.SetActive(true);
								}
							}
							else
							{
								if (mAllZoneMissionOverviews != null)
								{
									if (mAllZoneMissionOverviews.zoneNameTag != null && mAllZoneMissionOverviews.zoneNameTag.Length > 0)
									{
										if (mAllZoneMissionOverviews.m_bountyButtonRoot != null)
										{
											vector3.transform.SetParent(mAllZoneMissionOverviews.m_bountyButtonRoot.transform, false);
										}
									}
									else if (mAllZoneMissionOverviews.m_anonymousBountyButtonRoot != null)
									{
										vector3.transform.SetParent(mAllZoneMissionOverviews.m_anonymousBountyButtonRoot.transform, false);
									}
								}
								vector3.transform.localPosition = Vector3.zero;
								if (component1.m_errorImage != null && component1.m_errorImage.gameObject != null)
								{
									component1.m_errorImage.gameObject.SetActive(false);
								}
							}
							StackableMapIcon stackableMapIcon = vector3.GetComponent<StackableMapIcon>();
							if (stackableMapIcon == null)
							{
								continue;
							}
							stackableMapIcon.RegisterWithManager(num1);
						}
					}
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

	private void HandleInvasionPOIChanged()
	{
		JamMobileAreaPOI currentInvasionPOI = LegionfallData.GetCurrentInvasionPOI();
		if (currentInvasionPOI != null)
		{
			this.m_invasionNotification.gameObject.SetActive(true);
			this.m_invasionTitle.text = currentInvasionPOI.Description;
			long currentInvasionExpirationTime = LegionfallData.GetCurrentInvasionExpirationTime() - GarrisonStatus.CurrentTime();
			currentInvasionExpirationTime = (currentInvasionExpirationTime <= (long)0 ? (long)0 : currentInvasionExpirationTime);
			this.m_invasionTimeRemainingDuration.FormatDurationString((int)currentInvasionExpirationTime, false);
			this.m_invasionTimeRemaining.text = this.m_invasionTimeRemainingDuration.DurationString;
		}
		else
		{
			this.m_invasionNotification.gameObject.SetActive(false);
		}
		if (!this.m_mapInfo_BrokenIsles.gameObject.activeSelf)
		{
			this.SetMapViewSize_Argus();
		}
		else
		{
			this.SetMapViewSize_BrokenIsles();
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
					UnityEngine.Object.DestroyImmediate(adventureMapMissionSite.gameObject);
				}
			}
		}
		AdventureMapMissionSite[] adventureMapMissionSiteArray = this.m_missionAndWorldQuestArea_Argus.transform.GetComponentsInChildren<AdventureMapMissionSite>(true);
		for (int j = 0; j < (int)adventureMapMissionSiteArray.Length; j++)
		{
			AdventureMapMissionSite adventureMapMissionSite1 = adventureMapMissionSiteArray[j];
			if (adventureMapMissionSite1 != null)
			{
				StackableMapIcon stackableMapIcon = adventureMapMissionSite1.GetComponent<StackableMapIcon>();
				GameObject gameObject1 = adventureMapMissionSite1.gameObject;
				if (stackableMapIcon != null)
				{
					stackableMapIcon.RemoveFromContainer();
				}
				if (gameObject1 != null)
				{
					UnityEngine.Object.DestroyImmediate(adventureMapMissionSite1.gameObject);
				}
			}
		}
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				this.CreateMissionSite(((JamGarrisonMobileMission)enumerator.Current).MissionRecID);
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
		this.MapFiltersChanged -= new Action(this.UpdateWorldQuests);
		Main.instance.InvasionPOIChangedAction -= new Action(this.HandleInvasionPOIChanged);
	}

	private void OnEnable()
	{
		this.MapFiltersChanged += new Action(this.UpdateWorldQuests);
		this.HandleInvasionPOIChanged();
		Main.instance.InvasionPOIChangedAction += new Action(this.HandleInvasionPOIChanged);
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

	public void SetFollowerToInspect(int garrFollowerID)
	{
		this.m_followerToInspect = garrFollowerID;
		if (this.FollowerToInspectChangedAction != null)
		{
			this.FollowerToInspectChangedAction(garrFollowerID);
		}
	}

	public void SetMapViewSize_Argus()
	{
		this.m_mapViewRT.sizeDelta = new Vector2(this.m_mapViewRT.sizeDelta.x, 720f + (!this.m_invasionNotification.gameObject.activeSelf ? 0f : -60f));
		this.m_mapViewRT.anchoredPosition = new Vector2(0f, 100f);
		this.m_pinchZoomContentManager.SetZoom(1f, false);
		this.CenterAndZoomOut();
	}

	public void SetMapViewSize_BrokenIsles()
	{
		this.m_mapViewRT.sizeDelta = new Vector2(this.m_mapViewRT.sizeDelta.x, 820f + (!this.m_invasionNotification.gameObject.activeSelf ? 0f : -60f));
		this.m_mapViewRT.anchoredPosition = new Vector2(0f, 0f);
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

	private void SetupWorldQuestIcon(MobileWorldQuest worldQuest, GameObject worldQuestObj, float mapOffsetX, float mapOffsetY, float mapScale)
	{
		float startLocationY = (float)worldQuest.StartLocationY * -mapScale;
		float startLocationX = (float)worldQuest.StartLocationX * mapScale;
		startLocationY += mapOffsetX;
		startLocationX += mapOffsetY;
		float single = (float)((worldQuest.StartLocationMapID != 1220 ? 2001 : 1201));
		float single1 = 1334f;
		Vector2 vector3 = new Vector3(startLocationY / single, startLocationX / single1);
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
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		this.InitMissionSites();
		this.UpdateWorldQuests();
		this.HandleBountyInfoUpdated();
		Main.instance.GarrisonDataResetFinishedAction += new Action(this.InitMissionSites);
		Main.instance.MissionAddedAction += new Action<int, int>(this.HandleMissionAdded);
		Main.instance.BountyInfoUpdatedAction += new Action(this.HandleBountyInfoUpdated);
		AllPopups.instance.EnableMissionDialog();
	}

	private void Update()
	{
		this.m_currentVisibleZone = null;
		if (this.m_currentMapMission > 0)
		{
			this.m_secondsMissionHasBeenSelected += Time.deltaTime;
		}
		if (this.m_invasionNotification.gameObject.activeSelf)
		{
			long currentInvasionExpirationTime = LegionfallData.GetCurrentInvasionExpirationTime() - GarrisonStatus.CurrentTime();
			currentInvasionExpirationTime = (currentInvasionExpirationTime <= (long)0 ? (long)0 : currentInvasionExpirationTime);
			if (currentInvasionExpirationTime <= (long)0)
			{
				this.m_invasionNotification.gameObject.SetActive(false);
				if (!this.m_mapInfo_BrokenIsles.gameObject.activeSelf)
				{
					this.SetMapViewSize_Argus();
				}
				else
				{
					this.SetMapViewSize_BrokenIsles();
				}
				Main.instance.RequestWorldQuests();
			}
			else
			{
				this.m_invasionTimeRemainingDuration.FormatDurationString((int)currentInvasionExpirationTime, false);
				this.m_invasionTimeRemaining.text = this.m_invasionTimeRemainingDuration.DurationString;
			}
		}
	}

	public void UpdateWorldQuests()
	{
		AdventureMapWorldQuest[] componentsInChildren = this.m_missionAndWorldQuestArea_BrokenIsles.GetComponentsInChildren<AdventureMapWorldQuest>(true);
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
				UnityEngine.Object.DestroyImmediate(adventureMapWorldQuest.gameObject);
			}
		}
		AdventureMapWorldQuest[] adventureMapWorldQuestArray = this.m_missionAndWorldQuestArea_Argus.GetComponentsInChildren<AdventureMapWorldQuest>(true);
		for (int j = 0; j < (int)adventureMapWorldQuestArray.Length; j++)
		{
			AdventureMapWorldQuest adventureMapWorldQuest1 = adventureMapWorldQuestArray[j];
			StackableMapIcon stackableMapIcon = adventureMapWorldQuest1.GetComponent<StackableMapIcon>();
			GameObject gameObject1 = adventureMapWorldQuest1.gameObject;
			if (stackableMapIcon != null)
			{
				stackableMapIcon.RemoveFromContainer();
			}
			if (gameObject1 != null)
			{
				UnityEngine.Object.DestroyImmediate(adventureMapWorldQuest1.gameObject);
			}
		}
		IEnumerator enumerator = WorldQuestData.worldQuestDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileWorldQuest current = (MobileWorldQuest)enumerator.Current;
				if (!this.IsFilterEnabled(MapFilterType.All))
				{
					bool flag = false;
					if (this.IsFilterEnabled(MapFilterType.ArtifactPower))
					{
						MobileWorldQuestReward[] item = current.Item;
						for (int k = 0; k < (int)item.Length; k++)
						{
							MobileWorldQuestReward mobileWorldQuestReward = item[k];
							StaticDB.itemEffectDB.EnumRecordsByParentID(mobileWorldQuestReward.RecordID, (ItemEffectRec itemEffectRec) => {
								StaticDB.spellEffectDB.EnumRecordsByParentID(itemEffectRec.SpellID, (SpellEffectRec spellEffectRec) => {
									if (spellEffectRec.Effect != 240)
									{
										return true;
									}
									flag = true;
									return false;
								});
								if (flag)
								{
									return false;
								}
								return true;
							});
						}
					}
					if (this.IsFilterEnabled(MapFilterType.OrderResources))
					{
						MobileWorldQuestReward[] currency = current.Currency;
						int num = 0;
						while (num < (int)currency.Length)
						{
							if (currency[num].RecordID != 1220)
							{
								num++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Gold) && current.Money > 0)
					{
						flag = true;
					}
					if (this.IsFilterEnabled(MapFilterType.Gear))
					{
						MobileWorldQuestReward[] mobileWorldQuestRewardArray = current.Item;
						int num1 = 0;
						while (num1 < (int)mobileWorldQuestRewardArray.Length)
						{
							MobileWorldQuestReward mobileWorldQuestReward1 = mobileWorldQuestRewardArray[num1];
							ItemRec record = StaticDB.itemDB.GetRecord(mobileWorldQuestReward1.RecordID);
							if (record == null || record.ClassID != 2 && record.ClassID != 3 && record.ClassID != 4 && record.ClassID != 6)
							{
								num1++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.ProfessionMats))
					{
						MobileWorldQuestReward[] item1 = current.Item;
						int num2 = 0;
						while (num2 < (int)item1.Length)
						{
							MobileWorldQuestReward mobileWorldQuestReward2 = item1[num2];
							ItemRec itemRec = StaticDB.itemDB.GetRecord(mobileWorldQuestReward2.RecordID);
							if (itemRec == null || itemRec.ClassID != 7)
							{
								num2++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.PetCharms))
					{
						MobileWorldQuestReward[] mobileWorldQuestRewardArray1 = current.Item;
						int num3 = 0;
						while (num3 < (int)mobileWorldQuestRewardArray1.Length)
						{
							if (mobileWorldQuestRewardArray1[num3].RecordID != 116415)
							{
								num3++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_HighmountainTribes) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest mobileBountiesByWorldQuest = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num4 = 0;
						while (num4 < (int)mobileBountiesByWorldQuest.BountyQuestID.Length)
						{
							if (mobileBountiesByWorldQuest.BountyQuestID[num4] != 42233)
							{
								num4++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_CourtOfFarondis) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest mobileBountiesByWorldQuest1 = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num5 = 0;
						while (num5 < (int)mobileBountiesByWorldQuest1.BountyQuestID.Length)
						{
							if (mobileBountiesByWorldQuest1.BountyQuestID[num5] != 42420)
							{
								num5++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_Dreamweavers) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest item2 = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num6 = 0;
						while (num6 < (int)item2.BountyQuestID.Length)
						{
							if (item2.BountyQuestID[num6] != 42170)
							{
								num6++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_Wardens) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest mobileBountiesByWorldQuest2 = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num7 = 0;
						while (num7 < (int)mobileBountiesByWorldQuest2.BountyQuestID.Length)
						{
							if (mobileBountiesByWorldQuest2.BountyQuestID[num7] != 42422)
							{
								num7++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_Nightfallen) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest item3 = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num8 = 0;
						while (num8 < (int)item3.BountyQuestID.Length)
						{
							if (item3.BountyQuestID[num8] != 42421)
							{
								num8++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_Valarjar) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest mobileBountiesByWorldQuest3 = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num9 = 0;
						while (num9 < (int)mobileBountiesByWorldQuest3.BountyQuestID.Length)
						{
							if (mobileBountiesByWorldQuest3.BountyQuestID[num9] != 42234)
							{
								num9++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Bounty_KirinTor) && PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(current.QuestID))
					{
						MobileBountiesByWorldQuest item4 = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary[current.QuestID];
						int num10 = 0;
						while (num10 < (int)item4.BountyQuestID.Length)
						{
							if (item4.BountyQuestID[num10] != 43179)
							{
								num10++;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (this.IsFilterEnabled(MapFilterType.Invasion))
					{
						QuestInfoRec questInfoRec = StaticDB.questInfoDB.GetRecord(current.QuestInfoID);
						if (questInfoRec == null)
						{
							break;
						}
						else if (questInfoRec.Type == 7)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(AdventureMapPanel.instance.m_AdvMapWorldQuestPrefab);
				if (current.StartLocationMapID != 1220)
				{
					vector3.transform.localScale = new Vector3(1.33f, 1.33f, 1.33f);
					vector3.transform.SetParent(this.m_missionAndWorldQuestArea_Argus.transform, false);
					int worldMapAreaID = current.WorldMapAreaID;
					if (worldMapAreaID == 1170)
					{
						this.SetupWorldQuestIcon(current, vector3, 4832.76f, -1232f, 0.39f);
					}
					else if (worldMapAreaID == 1171)
					{
						this.SetupWorldQuestIcon(current, vector3, 3981f, 1520f, 0.38f);
					}
					else if (worldMapAreaID == 1135)
					{
						this.SetupWorldQuestIcon(current, vector3, 2115.88f, -7.788513f, 0.3132809f);
					}
					else
					{
						Debug.LogError(string.Concat(new object[] { "UNHANDLED WORLD QUEST AREA ID ", current.QuestTitle, " ", current.WorldMapAreaID }));
					}
				}
				else
				{
					vector3.transform.SetParent(this.m_missionAndWorldQuestArea_BrokenIsles.transform, false);
					this.SetupWorldQuestIcon(current, vector3, 1036.88037f, 597.2115f, 0.10271506f);
				}
				vector3.GetComponent<AdventureMapWorldQuest>().SetQuestID(current.QuestID);
				StackableMapIcon component1 = vector3.GetComponent<StackableMapIcon>();
				if (component1 == null)
				{
					continue;
				}
				component1.RegisterWithManager(current.StartLocationMapID);
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
		None,
		NumZones
	}
}