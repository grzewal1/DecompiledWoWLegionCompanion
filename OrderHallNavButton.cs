using System;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;

public class OrderHallNavButton : MonoBehaviour
{
	public Image m_normalImage;

	public Image m_selectedImage;

	public GameObject m_label;

	public LayoutElement m_holderLayoutElement;

	public OrderHallNavButton.NavButtonType m_navButtonType;

	public GameObject m_notificationBadgeRoot;

	public Text m_notificationBadgeText;

	private float m_selectedSize = 106f;

	private float m_normalSize = 80f;

	public float m_resizeDuration;

	public GameObject m_selectionGlowRoot;

	public Image m_greenSelectionGlow;

	private UiAnimMgr.UiAnimHandle m_glowSpinHandle;

	private UiAnimMgr.UiAnimHandle m_glowPulseHandle;

	private UiAnimMgr.UiAnimHandle m_notificationPulseHandle;

	private bool m_isSelected;

	public OrderHallNavButton()
	{
	}

	private void HandleOrderHallNavButtonSelected(OrderHallNavButton navButton)
	{
		if (navButton != this)
		{
			this.m_label.SetActive(false);
			this.m_normalImage.enabled = true;
			this.m_selectedImage.enabled = false;
			if (this.m_isSelected)
			{
				this.StopGlowEffect();
				this.ResizeForDeselect();
				this.m_isSelected = false;
			}
		}
		else if (!this.m_isSelected)
		{
			this.m_normalImage.enabled = false;
			this.m_selectedImage.enabled = true;
			this.StopGlowEffect();
			this.m_greenSelectionGlow.gameObject.SetActive(true);
			this.m_glowSpinHandle = UiAnimMgr.instance.PlayAnim("PrestigeSpin", this.m_selectionGlowRoot.transform, Vector3.zero, 1.66f, 0f);
			this.m_glowPulseHandle = UiAnimMgr.instance.PlayAnim("PrestigePulse", this.m_selectionGlowRoot.transform, Vector3.zero, 1.66f, 0f);
			UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 2f, 0f);
			this.m_label.SetActive(true);
			this.ResizeForSelect();
			this.m_isSelected = true;
		}
	}

	public bool IsSelected()
	{
		return this.m_isSelected;
	}

	private void OnDisable()
	{
		Main.instance.OrderHallNavButtonSelectedAction -= new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected);
	}

	private void OnEnable()
	{
		Main.instance.OrderHallNavButtonSelectedAction += new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected);
	}

	private void OnResizeDownComplete()
	{
		this.m_holderLayoutElement.minWidth = this.m_normalSize;
		this.m_holderLayoutElement.minHeight = this.m_normalSize;
	}

	private void OnResizeUpComplete()
	{
		this.m_holderLayoutElement.minWidth = this.m_selectedSize;
		this.m_holderLayoutElement.minHeight = this.m_selectedSize;
	}

	private void OnResizeUpdate(float newSize)
	{
		this.m_holderLayoutElement.minWidth = newSize;
		this.m_holderLayoutElement.minHeight = newSize;
	}

	private void ResizeForDeselect()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "ScaleUpForDeselect", "from", this.m_selectedSize, "to", this.m_normalSize, "time", this.m_resizeDuration, "onupdate", "OnResizeUpdate", "oncomplete", "OnResizeDownComplete" }));
	}

	private void ResizeForSelect()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "ScaleUpForSelect", "from", this.m_normalSize, "to", this.m_selectedSize, "time", this.m_resizeDuration, "onupdate", "OnResizeUpdate", "oncomplete", "OnResizeUpComplete" }));
	}

	public void SelectMe()
	{
		Main.instance.SelectOrderHallNavButton(this);
	}

	private void Start()
	{
		if (Main.instance.IsNarrowScreen())
		{
			this.m_selectedSize = 100f;
		}
	}

	private void StopGlowEffect()
	{
		if (this.m_glowSpinHandle != null)
		{
			UiAnimation anim = this.m_glowSpinHandle.GetAnim();
			if (anim != null)
			{
				anim.Stop(0.5f);
			}
			this.m_glowSpinHandle = null;
		}
		if (this.m_glowPulseHandle != null)
		{
			UiAnimation uiAnimation = this.m_glowPulseHandle.GetAnim();
			if (uiAnimation != null)
			{
				uiAnimation.Stop(0.5f);
			}
			this.m_glowPulseHandle = null;
		}
		this.m_greenSelectionGlow.gameObject.SetActive(false);
	}

	private void Update()
	{
		switch (this.m_navButtonType)
		{
			case OrderHallNavButton.NavButtonType.missions:
			{
				int numCompletedMissions = PersistentMissionData.GetNumCompletedMissions(true);
				if (numCompletedMissions == 0 && this.m_notificationBadgeRoot.activeSelf)
				{
					this.m_notificationBadgeRoot.SetActive(false);
				}
				else if (numCompletedMissions > 0)
				{
					if (!this.m_notificationBadgeRoot.activeSelf)
					{
						this.m_notificationBadgeRoot.SetActive(true);
					}
					if (this.m_notificationPulseHandle == null)
					{
						this.m_notificationPulseHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_notificationBadgeRoot.transform, Vector3.zero, 1f, 0f);
					}
					this.m_notificationBadgeText.text = string.Concat(string.Empty, numCompletedMissions);
				}
				break;
			}
			case OrderHallNavButton.NavButtonType.recruit:
			{
				int numReadyShipments = PersistentShipmentData.GetNumReadyShipments();
				if (ArtifactKnowledgeData.s_artifactKnowledgeInfo != null && ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel < ArtifactKnowledgeData.s_artifactKnowledgeInfo.MaxLevel)
				{
					numReadyShipments += ArtifactKnowledgeData.s_artifactKnowledgeInfo.ItemsInBags;
				}
				if (numReadyShipments == 0 && this.m_notificationBadgeRoot.activeSelf)
				{
					this.m_notificationBadgeRoot.SetActive(false);
				}
				else if (numReadyShipments > 0)
				{
					if (!this.m_notificationBadgeRoot.activeSelf)
					{
						this.m_notificationBadgeRoot.SetActive(true);
					}
					if (this.m_notificationPulseHandle == null)
					{
						this.m_notificationPulseHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_notificationBadgeRoot.transform, Vector3.zero, 1f, 0f);
					}
					this.m_notificationBadgeText.text = string.Concat(string.Empty, numReadyShipments);
				}
				break;
			}
			case OrderHallNavButton.NavButtonType.map:
			case OrderHallNavButton.NavButtonType.followers:
			{
				break;
			}
			case OrderHallNavButton.NavButtonType.talents:
			{
				bool playGreenCheckAnim = AllPanels.instance.m_talentTreePanel.TalentIsReadyToPlayGreenCheckAnim();
				if (!playGreenCheckAnim && this.m_notificationBadgeRoot.activeSelf)
				{
					this.m_notificationBadgeRoot.SetActive(false);
				}
				else if (playGreenCheckAnim && !this.m_notificationBadgeRoot.activeSelf)
				{
					this.m_notificationBadgeRoot.SetActive(true);
					this.m_notificationBadgeText.text = "1";
					if (this.m_notificationPulseHandle == null)
					{
						this.m_notificationPulseHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_notificationBadgeRoot.transform, Vector3.zero, 1f, 0f);
					}
				}
				break;
			}
			default:
			{
				goto case OrderHallNavButton.NavButtonType.followers;
			}
		}
	}

	public enum NavButtonType
	{
		missions,
		recruit,
		map,
		followers,
		talents
	}
}