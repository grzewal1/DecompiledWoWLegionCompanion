using System;
using UnityEngine;
using UnityEngine.UI;

public class OrderHallMultiPanel : MonoBehaviour
{
	public Text m_titleText;

	public Text m_troopButtonText;

	public Text m_followerButtonText;

	public Text m_allyButtonText;

	public Text m_talentButtonText;

	public Text m_worldMapButtonText;

	public OrderHallNavButton m_defaultNavButton;

	public OrderHallNavButton m_talentNavButton;

	public AutoCenterScrollRect m_autoCenterScrollRect;

	public float m_navButtonInitialEntranceDelay;

	public float m_navButtonEntranceDelay;

	public Canvas m_missionListPanelCanvas;

	public CanvasGroup m_missionListPanelCanvasGroup;

	public MiniMissionListPanel m_miniMissionListPanel;

	public RaycastTargetHack m_missionListPanelRTH;

	public Canvas m_troopsPanelCanvas;

	public CanvasGroup m_troopsPanelCanvasGroup;

	public TroopsPanel m_troopsPanel;

	public RaycastTargetHack m_troopsPanelRTH;

	public Canvas m_adventureMapPanelCanvas;

	public CanvasGroup m_adventureMapPanelCanvasGroup;

	public CanvasGroup m_playerInfoDisplayCanvasGroup;

	public AdventureMapPanel m_adventureMapPanel;

	public RaycastTargetHack m_adventureMapPanelRTH;

	public Canvas m_followersPanelCanvas;

	public CanvasGroup m_followersPanelCanvasGroup;

	public OrderHallFollowersPanel m_followersPanel;

	public RaycastTargetHack m_followersPanelRTH;

	public Canvas m_talentTreePanelCanvas;

	public CanvasGroup m_talentTreePanelCanvasGroup;

	public TalentTreePanel m_talentTreePanel;

	public RaycastTargetHack m_talentTreePanelRTH;

	public GameObject m_navBarArea;

	private bool m_actuallyDisablePanels;

	private void Start()
	{
		if (this.m_titleText != null)
		{
			this.m_titleText.set_font(GeneralHelpers.LoadStandardFont());
			this.m_titleText.set_text(StaticDB.GetString("CLASS_ORDER_HALL", null));
		}
		this.m_troopButtonText.set_font(GeneralHelpers.LoadStandardFont());
		this.m_troopButtonText.set_text(StaticDB.GetString("RECRUIT", null));
		this.m_followerButtonText.set_font(GeneralHelpers.LoadStandardFont());
		this.m_followerButtonText.set_text(StaticDB.GetString("FOLLOWERS", null));
		this.m_allyButtonText.set_font(GeneralHelpers.LoadStandardFont());
		this.m_allyButtonText.set_text(StaticDB.GetString("MISSIONS", null));
		this.m_talentButtonText.set_font(GeneralHelpers.LoadStandardFont());
		this.m_talentButtonText.set_text(StaticDB.GetString("RESEARCH", null));
		this.m_worldMapButtonText.set_font(GeneralHelpers.LoadStandardFont());
		this.m_worldMapButtonText.set_text(StaticDB.GetString("WORLD_MAP", null));
		Text[] componentsInChildren = base.GetComponentsInChildren<Text>(true);
		Text[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Text text = array[i];
			if (text.get_text() == "Abilities")
			{
				text.set_text(StaticDB.GetString("ABILITIES", null));
			}
			else if (text.get_text() == "Counters:")
			{
				text.set_text(StaticDB.GetString("COUNTERS", null) + ":");
			}
		}
		this.m_defaultNavButton.SelectMe();
	}

	private void HideMainPanels()
	{
		if (this.m_actuallyDisablePanels)
		{
			this.m_missionListPanelCanvasGroup.set_alpha(1f);
			this.m_missionListPanelCanvasGroup.set_interactable(true);
			this.m_missionListPanelCanvasGroup.set_blocksRaycasts(true);
			this.m_troopsPanelCanvasGroup.set_alpha(1f);
			this.m_troopsPanelCanvasGroup.set_interactable(true);
			this.m_troopsPanelCanvasGroup.set_blocksRaycasts(true);
			this.m_adventureMapPanelCanvasGroup.set_alpha(1f);
			this.m_adventureMapPanelCanvasGroup.set_interactable(true);
			this.m_adventureMapPanelCanvasGroup.set_blocksRaycasts(true);
			this.m_playerInfoDisplayCanvasGroup.set_alpha(1f);
			this.m_playerInfoDisplayCanvasGroup.set_interactable(true);
			this.m_playerInfoDisplayCanvasGroup.set_blocksRaycasts(true);
			this.m_followersPanelCanvasGroup.set_alpha(1f);
			this.m_followersPanelCanvasGroup.set_interactable(true);
			this.m_followersPanelCanvasGroup.set_blocksRaycasts(true);
			this.m_talentTreePanelCanvasGroup.set_alpha(1f);
			this.m_talentTreePanelCanvasGroup.set_interactable(true);
			this.m_talentTreePanelCanvasGroup.set_blocksRaycasts(true);
			this.m_missionListPanelCanvasGroup.get_gameObject().SetActive(false);
			this.m_troopsPanelCanvasGroup.get_gameObject().SetActive(false);
			this.m_adventureMapPanelCanvasGroup.get_gameObject().SetActive(false);
			this.m_followersPanelCanvasGroup.get_gameObject().SetActive(false);
			this.m_talentTreePanelCanvasGroup.get_gameObject().SetActive(false);
		}
		else
		{
			this.m_missionListPanelRTH.DisableTargetList();
			this.m_troopsPanelRTH.DisableTargetList();
			this.m_adventureMapPanelRTH.DisableTargetList();
			this.m_followersPanelRTH.DisableTargetList();
			this.m_talentTreePanelRTH.DisableTargetList();
			this.m_missionListPanelCanvasGroup.set_alpha(0f);
			this.m_missionListPanelCanvasGroup.set_interactable(false);
			this.m_missionListPanelCanvasGroup.set_blocksRaycasts(false);
			this.m_troopsPanelCanvasGroup.set_alpha(0f);
			this.m_troopsPanelCanvasGroup.set_interactable(false);
			this.m_troopsPanelCanvasGroup.set_blocksRaycasts(false);
			this.m_adventureMapPanelCanvasGroup.set_alpha(0f);
			this.m_adventureMapPanelCanvasGroup.set_interactable(false);
			this.m_adventureMapPanelCanvasGroup.set_blocksRaycasts(false);
			this.m_playerInfoDisplayCanvasGroup.set_alpha(0f);
			this.m_playerInfoDisplayCanvasGroup.set_interactable(false);
			this.m_playerInfoDisplayCanvasGroup.set_blocksRaycasts(false);
			this.m_followersPanelCanvasGroup.set_alpha(0f);
			this.m_followersPanelCanvasGroup.set_interactable(false);
			this.m_followersPanelCanvasGroup.set_blocksRaycasts(false);
			this.m_talentTreePanelCanvasGroup.set_alpha(0f);
			this.m_talentTreePanelCanvasGroup.set_interactable(false);
			this.m_talentTreePanelCanvasGroup.set_blocksRaycasts(false);
		}
	}

	public void ShowMissionListPanel()
	{
		this.HideMainPanels();
		if (this.m_actuallyDisablePanels)
		{
			this.m_missionListPanelCanvasGroup.get_gameObject().SetActive(true);
		}
		else
		{
			this.m_missionListPanelRTH.EnableTargetList();
			this.m_missionListPanelCanvasGroup.set_alpha(1f);
			this.m_missionListPanelCanvasGroup.set_interactable(true);
			this.m_missionListPanelCanvasGroup.set_blocksRaycasts(true);
		}
	}

	public void ShowTroopsPanel()
	{
		this.HideMainPanels();
		if (this.m_actuallyDisablePanels)
		{
			this.m_troopsPanelCanvasGroup.get_gameObject().SetActive(true);
		}
		else
		{
			this.m_troopsPanelRTH.EnableTargetList();
			this.m_troopsPanelCanvasGroup.set_alpha(1f);
			this.m_troopsPanelCanvasGroup.set_interactable(true);
			this.m_troopsPanelCanvasGroup.set_blocksRaycasts(true);
		}
	}

	public void ShowAdventureMapPanel()
	{
		this.HideMainPanels();
		if (this.m_actuallyDisablePanels)
		{
			this.m_adventureMapPanelCanvasGroup.get_gameObject().SetActive(true);
		}
		else
		{
			this.m_adventureMapPanelRTH.EnableTargetList();
			this.m_adventureMapPanelCanvasGroup.set_alpha(1f);
			this.m_adventureMapPanelCanvasGroup.set_interactable(true);
			this.m_adventureMapPanelCanvasGroup.set_blocksRaycasts(true);
			this.m_playerInfoDisplayCanvasGroup.set_alpha(1f);
			this.m_playerInfoDisplayCanvasGroup.set_interactable(true);
			this.m_playerInfoDisplayCanvasGroup.set_blocksRaycasts(true);
		}
	}

	public void ShowFollowersPanel()
	{
		this.HideMainPanels();
		if (this.m_actuallyDisablePanels)
		{
			this.m_followersPanelCanvasGroup.get_gameObject().SetActive(true);
		}
		else
		{
			this.m_followersPanelRTH.EnableTargetList();
			this.m_followersPanelCanvasGroup.set_alpha(1f);
			this.m_followersPanelCanvasGroup.set_interactable(true);
			this.m_followersPanelCanvasGroup.set_blocksRaycasts(true);
		}
	}

	public void ShowTalentTreePanel()
	{
		this.HideMainPanels();
		if (this.m_actuallyDisablePanels)
		{
			this.m_talentTreePanelCanvasGroup.get_gameObject().SetActive(true);
		}
		else
		{
			this.m_talentTreePanelRTH.EnableTargetList();
			this.m_talentTreePanelCanvasGroup.set_alpha(1f);
			this.m_talentTreePanelCanvasGroup.set_interactable(true);
			this.m_talentTreePanelCanvasGroup.set_blocksRaycasts(true);
		}
	}

	private void OnEnable()
	{
		this.ShowAdventureMapPanel();
		this.m_navBarArea.SetActive(true);
	}
}
