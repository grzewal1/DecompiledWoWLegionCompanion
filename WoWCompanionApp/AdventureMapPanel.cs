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

		public Image m_worldMapLowDetail_BrokenIsles;

		public Image m_worldMapLowDetail_Argus;

		public MapInfo m_mainMapInfo;

		public GameObject m_AdvMapMissionSitePrefab;

		public GameObject m_AdvMapWorldQuestPrefab;

		public GameObject m_bountySitePrefab;

		public MapInfo m_mapInfo_BrokenIsles;

		public MapInfo m_mapInfo_Argus;

		public MapInfo m_mapInfo_KulTiras;

		public MapInfo m_mapInfo_Zandalar;

		public GameObject m_missionAndWorldQuestArea_BrokenIsles;

		public GameObject m_missionAndWorldQuestArea_Argus;

		public GameObject m_missionAndWorldQuestArea_KulTiras;

		public GameObject m_missionAndWorldQuestArea_Zandalar;

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
			this.m_mapFilters = new bool[14];
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

		private static int GetImageWByMapID(int startLocationMapId)
		{
			if (startLocationMapId == 1642)
			{
				return 1201;
			}
			if (startLocationMapId == 1643)
			{
				return 1201;
			}
			if (startLocationMapId == 1220)
			{
				return 1201;
			}
			if (startLocationMapId == 1669)
			{
				return 2001;
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
			if (PersistentBountyData.bountyDictionary == null)
			{
				return;
			}
			foreach (WrapperWorldQuestBounty value in PersistentBountyData.bountyDictionary.Values)
			{
				GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(this.m_bountySitePrefab);
				if (vector3 != null)
				{
					BountySite component1 = vector3.GetComponent<BountySite>();
					if (component1 != null)
					{
						component1.SetBounty(value);
						vector3.name = string.Concat("BountySite ", value.QuestID);
						RectTransform vector2 = vector3.GetComponent<RectTransform>();
						if (vector2 != null)
						{
							vector2.anchorMin = new Vector2(0.5f, 0.5f);
							vector2.anchorMax = new Vector2(0.5f, 0.5f);
							QuestV2Rec record = StaticDB.questDB.GetRecord(value.QuestID);
							int num = (record == null ? 0 : record.QuestSortID);
							bool flag = true;
							ZoneMissionOverview mAllZoneMissionOverviews = null;
							int num1 = 1220;
							switch (num)
							{
								case 8499:
								{
									break;
								}
								case 8500:
								{
									break;
								}
								case 8501:
								{
									break;
								}
								default:
								{
									if (num != 7502)
									{
										if (num == 7503)
										{
											mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[2];
											break;
										}
										else
										{
											switch (num)
											{
												case 7541:
												{
													mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[3];
													goto Label0;
												}
												case 7543:
												{
													mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[1];
													goto Label0;
												}
												default:
												{
													if (num == 7334)
													{
														mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[0];
														goto Label0;
													}
													else if (num == 7558)
													{
														mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[5];
														goto Label0;
													}
													else if (num == 7637)
													{
														mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[4];
														goto Label0;
													}
													else
													{
														if (num == 8147)
														{
															break;
														}
														if (num == 8567)
														{
															goto Label0;
														}
														else if (num == 8574)
														{
															mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[7];
															goto Label0;
														}
														else if (num == 8701)
														{
															mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[9];
															goto Label0;
														}
														else if (num == 8721)
														{
															goto Label0;
														}
														else if (num == 9042)
														{
															goto Label0;
														}
														else
														{
															Debug.LogError(string.Concat(new object[] { "INVALID QUESTSORTID ", num, " for quest ID:", value.QuestID }));
															flag = false;
															goto Label0;
														}
													}
												}
											}
										}
									}
									mAllZoneMissionOverviews = this.m_allZoneMissionOverviews[6];
									break;
								}
							}
						Label0:
							if (!flag)
							{
								vector3.transform.localPosition = new Vector3(0f, 0f, 0f);
								if (component1.m_errorImage != null)
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
								if (component1.m_errorImage != null)
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

		private void HandleInvasionPOIChanged()
		{
			if (LegionfallData.HasCurrentInvasionPOI())
			{
				WrapperAreaPoi currentInvasionPOI = LegionfallData.GetCurrentInvasionPOI();
				this.m_invasionNotification.gameObject.SetActive(true);
				this.m_invasionTitle.text = currentInvasionPOI.Description;
				TimeSpan currentInvasionExpirationTime = LegionfallData.GetCurrentInvasionExpirationTime() - GarrisonStatus.CurrentTime();
				currentInvasionExpirationTime = (currentInvasionExpirationTime.TotalSeconds <= 0 ? TimeSpan.Zero : currentInvasionExpirationTime);
				this.m_invasionTimeRemaining.text = currentInvasionExpirationTime.GetDurationString(false);
			}
			else
			{
				this.m_invasionNotification.gameObject.SetActive(false);
			}
			this.SetActiveMapViewSize();
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
						UnityEngine.Object.Destroy(adventureMapMissionSite1.gameObject);
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
			Main.instance.InvasionPOIChangedAction -= new Action(this.HandleInvasionPOIChanged);
		}

		private void OnEnable()
		{
			AdventureMapPanel.instance = this;
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

		private void SetActiveMapViewSize()
		{
			if (this.m_mapInfo_BrokenIsles.gameObject.activeSelf)
			{
				this.SetMapViewSize_BrokenIsles();
			}
			else if (this.m_mapInfo_Argus.gameObject.activeSelf)
			{
				this.SetMapViewSize_Argus();
			}
			else if (this.m_mapInfo_KulTiras.gameObject.activeSelf)
			{
				this.SetMapViewSize_KulTiras();
			}
			else if (this.m_mapInfo_Zandalar.gameObject.activeSelf)
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

		private void SetMapViewSize_Argus()
		{
			this.m_mapViewRT.sizeDelta = new Vector2(this.m_mapViewRT.sizeDelta.x, 720f + (!this.m_invasionNotification.gameObject.activeSelf ? 0f : -60f));
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 100f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		private void SetMapViewSize_BrokenIsles()
		{
			this.m_mapViewRT.sizeDelta = new Vector2(this.m_mapViewRT.sizeDelta.x, 820f + (!this.m_invasionNotification.gameObject.activeSelf ? 0f : -60f));
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 0f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		private void SetMapViewSize_KulTiras()
		{
			this.m_mapViewRT.sizeDelta = new Vector2(this.m_mapViewRT.sizeDelta.x, 820f + (!this.m_invasionNotification.gameObject.activeSelf ? 0f : -60f));
			this.m_mapViewRT.anchoredPosition = new Vector2(25f, 75f);
			this.m_pinchZoomContentManager.SetZoom(1f, false);
			this.CenterAndZoomOut();
		}

		private void SetMapViewSize_Zandalar()
		{
			this.m_mapViewRT.sizeDelta = new Vector2(this.m_mapViewRT.sizeDelta.x, 720f + (!this.m_invasionNotification.gameObject.activeSelf ? 0f : -60f));
			this.m_mapViewRT.anchoredPosition = new Vector2(0f, 100f);
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

		private void Update()
		{
			this.m_currentVisibleZone = null;
			if (this.m_currentMapMission > 0)
			{
				this.m_secondsMissionHasBeenSelected += Time.deltaTime;
			}
			if (this.m_invasionNotification.gameObject.activeSelf)
			{
				TimeSpan currentInvasionExpirationTime = LegionfallData.GetCurrentInvasionExpirationTime() - GarrisonStatus.CurrentTime();
				currentInvasionExpirationTime = (currentInvasionExpirationTime.TotalSeconds <= 0 ? TimeSpan.Zero : currentInvasionExpirationTime);
				if (currentInvasionExpirationTime.TotalSeconds <= 0)
				{
					this.m_invasionNotification.gameObject.SetActive(false);
					this.SetActiveMapViewSize();
					Main.instance.RequestWorldQuests();
				}
				else
				{
					this.m_invasionTimeRemaining.text = currentInvasionExpirationTime.GetDurationString(false);
				}
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
			AdventureMapPanel.ClearWorldQuestArea(this.m_missionAndWorldQuestArea_BrokenIsles);
			AdventureMapPanel.ClearWorldQuestArea(this.m_missionAndWorldQuestArea_Argus);
			AdventureMapPanel.ClearWorldQuestArea(this.m_missionAndWorldQuestArea_KulTiras);
			AdventureMapPanel.ClearWorldQuestArea(this.m_missionAndWorldQuestArea_Zandalar);
			foreach (WrapperWorldQuest value in WorldQuestData.WorldQuestDictionary.Values)
			{
				if (!this.IsFilterEnabled(MapFilterType.All))
				{
					bool flag7 = false;
					if (!flag7 && this.IsFilterEnabled(MapFilterType.OrderResources))
					{
						flag7 = flag7 | value.Currencies.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => reward.RecordID == 1220);
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Gold) && value.Money > 0)
					{
						flag7 = true;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Gear))
					{
						flag7 = flag7 | value.Items.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => {
							ItemRec record = StaticDB.itemDB.GetRecord(reward.RecordID);
							return (record == null ? false : (record.ClassID == 2 || record.ClassID == 3 || record.ClassID == 4 ? true : record.ClassID == 6));
						});
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.ProfessionMats))
					{
						flag7 = flag7 | value.Items.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => {
							ItemRec record = StaticDB.itemDB.GetRecord(reward.RecordID);
							return (record == null ? false : record.ClassID == 7);
						});
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.PetCharms))
					{
						flag7 = flag7 | value.Items.Any<WrapperWorldQuestReward>((WrapperWorldQuestReward reward) => reward.RecordID == 116415);
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_HighmountainTribes))
					{
						bool flag8 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag6 = false;
						}
						else
						{
							WrapperBountiesByWorldQuest item = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag6 = item.BountyQuestIDs.Any<int>((int questID) => questID == 42233);
						}
						flag7 = flag8 | flag6;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_CourtOfFarondis))
					{
						bool flag9 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag5 = false;
						}
						else
						{
							WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag5 = wrapperBountiesByWorldQuest.BountyQuestIDs.Any<int>((int questID) => questID == 42420);
						}
						flag7 = flag9 | flag5;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_Dreamweavers))
					{
						bool flag10 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag4 = false;
						}
						else
						{
							WrapperBountiesByWorldQuest item1 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag4 = item1.BountyQuestIDs.Any<int>((int questID) => questID == 42170);
						}
						flag7 = flag10 | flag4;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_Wardens))
					{
						bool flag11 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag3 = false;
						}
						else
						{
							WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest1 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag3 = wrapperBountiesByWorldQuest1.BountyQuestIDs.Any<int>((int questID) => questID == 42422);
						}
						flag7 = flag11 | flag3;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_Nightfallen))
					{
						bool flag12 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag2 = false;
						}
						else
						{
							WrapperBountiesByWorldQuest item2 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag2 = item2.BountyQuestIDs.Any<int>((int questID) => questID == 42421);
						}
						flag7 = flag12 | flag2;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_Valarjar))
					{
						bool flag13 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag1 = false;
						}
						else
						{
							WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest2 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag1 = wrapperBountiesByWorldQuest2.BountyQuestIDs.Any<int>((int questID) => questID == 42234);
						}
						flag7 = flag13 | flag1;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Bounty_KirinTor))
					{
						bool flag14 = flag7;
						if (!PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(value.QuestID))
						{
							flag = false;
						}
						else
						{
							WrapperBountiesByWorldQuest item3 = PersistentBountyData.bountiesByWorldQuestDictionary[value.QuestID];
							flag = item3.BountyQuestIDs.Any<int>((int questID) => questID == 43179);
						}
						flag7 = flag14 | flag;
					}
					if (!flag7 && this.IsFilterEnabled(MapFilterType.Invasion))
					{
						QuestInfoRec questInfoRec = StaticDB.questInfoDB.GetRecord(value.QuestInfoID);
						if (questInfoRec == null)
						{
							break;
						}
						else if (questInfoRec.Type == 7)
						{
							flag7 = true;
						}
					}
					if (!flag7)
					{
						continue;
					}
				}
				GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(AdventureMapPanel.instance.m_AdvMapWorldQuestPrefab);
				if (value.StartLocationMapID == 1220)
				{
					vector3.transform.SetParent(this.m_missionAndWorldQuestArea_BrokenIsles.transform, false);
					this.SetupWorldQuestIcon(value, vector3, 1036.88025f, 597.2115f, 0.10271506f);
				}
				else if (value.StartLocationMapID == 1669)
				{
					vector3.transform.localScale = new Vector3(1.33f, 1.33f, 1.33f);
					vector3.transform.SetParent(this.m_missionAndWorldQuestArea_Argus.transform, false);
					int worldMapAreaID = value.WorldMapAreaID;
					switch (worldMapAreaID)
					{
						case 882:
						{
							this.SetupWorldQuestIcon(value, vector3, 4832.76f, -1232f, 0.39f);
							break;
						}
						case 885:
						{
							this.SetupWorldQuestIcon(value, vector3, 3981f, 1520f, 0.38f);
							break;
						}
						default:
						{
							if (worldMapAreaID == 1170)
							{
								goto case 882;
							}
							if (worldMapAreaID == 1171)
							{
								goto case 885;
							}
							if (worldMapAreaID == 830 || worldMapAreaID == 1135)
							{
								this.SetupWorldQuestIcon(value, vector3, 2115.88f, -7.788513f, 0.3132809f);
								break;
							}
							else
							{
								Debug.LogError(string.Concat(new object[] { "UNHANDLED WORLD QUEST AREA ID ", value.QuestTitle, " ", value.WorldMapAreaID }));
								break;
							}
						}
					}
				}
				else if (value.StartLocationMapID == 1642)
				{
					vector3.transform.SetParent(this.m_missionAndWorldQuestArea_Zandalar.transform, false);
					this.SetupWorldQuestIcon(value, vector3, 1036.88025f, 597.2115f, 0.10271506f);
				}
				else if (value.StartLocationMapID != 1643)
				{
					Debug.LogError(string.Concat(new object[] { "UNHANDLED WORLD QUEST AREA ID ", value.QuestTitle, " ", value.WorldMapAreaID }));
				}
				else
				{
					vector3.transform.SetParent(this.m_missionAndWorldQuestArea_KulTiras.transform, false);
					this.SetupWorldQuestIcon(value, vector3, 1036.88025f, 597.2115f, 0.10271506f);
				}
				vector3.GetComponent<AdventureMapWorldQuest>().SetQuestID(value.QuestID);
				StackableMapIcon component = vector3.GetComponent<StackableMapIcon>();
				if (component == null)
				{
					continue;
				}
				component.RegisterWithManager(value.StartLocationMapID);
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
			None,
			NumZones
		}
	}
}