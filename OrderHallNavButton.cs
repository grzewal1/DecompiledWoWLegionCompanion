using System;
using UnityEngine;
using UnityEngine.UI;

public class OrderHallNavButton : MonoBehaviour
{
	public enum NavButtonType
	{
		missions = 0,
		recruit = 1,
		map = 2,
		followers = 3,
		talents = 4
	}

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

	private void OnEnable()
	{
		Main expr_05 = Main.instance;
		expr_05.OrderHallNavButtonSelectedAction = (Action<OrderHallNavButton>)Delegate.Combine(expr_05.OrderHallNavButtonSelectedAction, new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected));
	}

	private void OnDisable()
	{
		Main expr_05 = Main.instance;
		expr_05.OrderHallNavButtonSelectedAction = (Action<OrderHallNavButton>)Delegate.Remove(expr_05.OrderHallNavButtonSelectedAction, new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected));
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
			UiAnimation anim2 = this.m_glowPulseHandle.GetAnim();
			if (anim2 != null)
			{
				anim2.Stop(0.5f);
			}
			this.m_glowPulseHandle = null;
		}
		this.m_greenSelectionGlow.get_gameObject().SetActive(false);
	}

	private void OnResizeUpdate(float newSize)
	{
		this.m_holderLayoutElement.set_minWidth(newSize);
		this.m_holderLayoutElement.set_minHeight(newSize);
	}

	private void OnResizeUpComplete()
	{
		this.m_holderLayoutElement.set_minWidth(this.m_selectedSize);
		this.m_holderLayoutElement.set_minHeight(this.m_selectedSize);
	}

	private void OnResizeDownComplete()
	{
		this.m_holderLayoutElement.set_minWidth(this.m_normalSize);
		this.m_holderLayoutElement.set_minHeight(this.m_normalSize);
	}

	private void ResizeForSelect()
	{
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"ScaleUpForSelect",
			"from",
			this.m_normalSize,
			"to",
			this.m_selectedSize,
			"time",
			this.m_resizeDuration,
			"onupdate",
			"OnResizeUpdate",
			"oncomplete",
			"OnResizeUpComplete"
		}));
	}

	private void ResizeForDeselect()
	{
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"ScaleUpForDeselect",
			"from",
			this.m_selectedSize,
			"to",
			this.m_normalSize,
			"time",
			this.m_resizeDuration,
			"onupdate",
			"OnResizeUpdate",
			"oncomplete",
			"OnResizeDownComplete"
		}));
	}

	private void HandleOrderHallNavButtonSelected(OrderHallNavButton navButton)
	{
		if (navButton == this)
		{
			if (!this.m_isSelected)
			{
				this.m_normalImage.set_enabled(false);
				this.m_selectedImage.set_enabled(true);
				this.StopGlowEffect();
				this.m_greenSelectionGlow.get_gameObject().SetActive(true);
				this.m_glowSpinHandle = UiAnimMgr.instance.PlayAnim("PrestigeSpin", this.m_selectionGlowRoot.get_transform(), Vector3.get_zero(), 1.66f, 0f);
				this.m_glowPulseHandle = UiAnimMgr.instance.PlayAnim("PrestigePulse", this.m_selectionGlowRoot.get_transform(), Vector3.get_zero(), 1.66f, 0f);
				UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.get_transform(), Vector3.get_zero(), 2f, 0f);
				this.m_label.SetActive(true);
				this.ResizeForSelect();
				this.m_isSelected = true;
			}
		}
		else
		{
			this.m_label.SetActive(false);
			this.m_normalImage.set_enabled(true);
			this.m_selectedImage.set_enabled(false);
			if (this.m_isSelected)
			{
				this.StopGlowEffect();
				this.ResizeForDeselect();
				this.m_isSelected = false;
			}
		}
	}

	public void SelectMe()
	{
		Main.instance.SelectOrderHallNavButton(this);
	}

	public bool IsSelected()
	{
		return this.m_isSelected;
	}

	private void Update()
	{
		switch (this.m_navButtonType)
		{
		case OrderHallNavButton.NavButtonType.missions:
		{
			int numCompletedMissions = PersistentMissionData.GetNumCompletedMissions(true);
			if (numCompletedMissions == 0 && this.m_notificationBadgeRoot.get_activeSelf())
			{
				this.m_notificationBadgeRoot.SetActive(false);
			}
			else if (numCompletedMissions > 0)
			{
				if (!this.m_notificationBadgeRoot.get_activeSelf())
				{
					this.m_notificationBadgeRoot.SetActive(true);
				}
				if (this.m_notificationPulseHandle == null)
				{
					this.m_notificationPulseHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_notificationBadgeRoot.get_transform(), Vector3.get_zero(), 1f, 0f);
				}
				this.m_notificationBadgeText.set_text(string.Empty + numCompletedMissions);
			}
			break;
		}
		case OrderHallNavButton.NavButtonType.recruit:
		{
			int numReadyShipments = PersistentShipmentData.GetNumReadyShipments();
			if (numReadyShipments == 0 && this.m_notificationBadgeRoot.get_activeSelf())
			{
				this.m_notificationBadgeRoot.SetActive(false);
			}
			else if (numReadyShipments > 0)
			{
				if (!this.m_notificationBadgeRoot.get_activeSelf())
				{
					this.m_notificationBadgeRoot.SetActive(true);
				}
				if (this.m_notificationPulseHandle == null)
				{
					this.m_notificationPulseHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_notificationBadgeRoot.get_transform(), Vector3.get_zero(), 1f, 0f);
				}
				this.m_notificationBadgeText.set_text(string.Empty + numReadyShipments);
			}
			break;
		}
		case OrderHallNavButton.NavButtonType.talents:
		{
			bool flag = AllPanels.instance.m_talentTreePanel.TalentIsReadyToPlayGreenCheckAnim();
			if (!flag && this.m_notificationBadgeRoot.get_activeSelf())
			{
				this.m_notificationBadgeRoot.SetActive(false);
			}
			else if (flag && !this.m_notificationBadgeRoot.get_activeSelf())
			{
				this.m_notificationBadgeRoot.SetActive(true);
				this.m_notificationBadgeText.set_text("1");
				if (this.m_notificationPulseHandle == null)
				{
					this.m_notificationPulseHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_notificationBadgeRoot.get_transform(), Vector3.get_zero(), 1f, 0f);
				}
			}
			break;
		}
		}
	}
}
