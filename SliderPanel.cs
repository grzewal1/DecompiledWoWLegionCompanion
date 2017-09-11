using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderPanel : MonoBehaviour
{
	public float m_previewSeconds;

	public float m_maximizeSeconds;

	public float m_hideSeconds;

	public bool m_isHorizontal;

	private Vector2 m_startingPos;

	private Vector2 m_startingPointerPos;

	public int m_startingVerticalOffset;

	public int m_missionPanelSliderPreviewHeight;

	public int m_missionPanelSliderFullHeight;

	public bool m_stretchAbovePreviewHight;

	public int m_missionPanelSliderPreviewWidth;

	public int m_missionPanelSliderFullWidth;

	public bool m_stretchAbovePreviewWidth;

	public bool m_hidePreviewWhenMaximized;

	public CanvasGroup m_masterCanvasGroup;

	public CanvasGroup m_previewCanvasGroup;

	public CanvasGroup m_maximizedCanvasGroup;

	public Action SliderPanelMaximizedAction;

	public Action SliderPanelBeginMinimizeAction;

	public Action SliderPanelFinishMinimizeAction;

	public Action SliderPanelBeginDragAction;

	public Action SliderPanelBeginShrinkToPreviewPositionAction;

	private bool m_busyMoving;

	private bool m_movementIsPending;

	private bool m_pendingMovementIsToShowPreview;

	private bool m_pendingMovementIsToMaximize;

	private bool m_pendingMovementIsToHide;

	private bool m_isShowing;

	public SliderPanel()
	{
	}

	private void DisableSliderPanel()
	{
		if (this.m_masterCanvasGroup != null)
		{
			this.m_masterCanvasGroup.alpha = 0f;
		}
		this.m_busyMoving = false;
		this.m_isShowing = false;
		if (this.SliderPanelFinishMinimizeAction != null)
		{
			this.SliderPanelFinishMinimizeAction();
		}
	}

	public void HideSliderPanel()
	{
		if (this.m_busyMoving)
		{
			this.m_movementIsPending = true;
			this.m_pendingMovementIsToShowPreview = false;
			this.m_pendingMovementIsToMaximize = false;
			this.m_pendingMovementIsToHide = true;
			return;
		}
		this.m_busyMoving = true;
		if (this.SliderPanelBeginMinimizeAction != null)
		{
			this.SliderPanelBeginMinimizeAction();
		}
		Vector2 vector2 = new Vector2(0f, (float)this.m_startingVerticalOffset);
		GameObject gameObject = base.gameObject;
		object[] component = new object[] { "name", "Slide Mission Details Out (Bottom)", "from", base.GetComponent<RectTransform>().anchoredPosition, "to", vector2, "easeType", "easeOutCubic", "time", null, null, null, null, null };
		component[9] = (this.m_hideSeconds <= 0f ? 0.5f : this.m_hideSeconds);
		component[10] = "onupdate";
		component[11] = "PanelSliderBottomTweenCallback";
		component[12] = "oncomplete";
		component[13] = "DisableSliderPanel";
		iTween.ValueTo(gameObject, iTween.Hash(component));
	}

	public bool IsBusyMoving()
	{
		return this.m_busyMoving;
	}

	public bool IsShowing()
	{
		return this.m_isShowing;
	}

	public void MaximizeSliderPanel()
	{
		if (this.m_busyMoving)
		{
			this.m_movementIsPending = true;
			this.m_pendingMovementIsToShowPreview = false;
			this.m_pendingMovementIsToMaximize = true;
			this.m_pendingMovementIsToHide = false;
			return;
		}
		this.m_busyMoving = true;
		if (this.m_masterCanvasGroup != null)
		{
			this.m_masterCanvasGroup.alpha = 1f;
		}
		Vector2 vector2 = new Vector2((float)this.m_missionPanelSliderFullWidth, (float)(this.m_missionPanelSliderFullHeight + this.m_startingVerticalOffset));
		Vector2 component = base.GetComponent<RectTransform>().anchoredPosition;
		component.y += (float)this.m_startingVerticalOffset;
		GameObject gameObject = base.gameObject;
		object[] objArray = new object[] { "name", "Slide Mission Details In (Bottom)", "from", component, "to", vector2, "easeType", "easeOutCubic", "time", null, null, null, null, null };
		objArray[9] = (this.m_maximizeSeconds <= 0f ? 0.5f : this.m_maximizeSeconds);
		objArray[10] = "onupdate";
		objArray[11] = "PanelSliderBottomTweenCallback";
		objArray[12] = "oncomplete";
		objArray[13] = "OnDoneSlidingInMaximize";
		iTween.ValueTo(gameObject, iTween.Hash(objArray));
	}

	public void MissionPanelSlider_HandleAutopositioning_Bottom()
	{
		float single = 5f;
		RectTransform component = base.GetComponent<RectTransform>();
		if (!this.m_isHorizontal)
		{
			if (this.m_startingPos.y < (float)this.m_missionPanelSliderPreviewHeight + single)
			{
				if (component.anchoredPosition.y < (float)this.m_missionPanelSliderPreviewHeight - single)
				{
					this.HideSliderPanel();
					return;
				}
				this.MaximizeSliderPanel();
				return;
			}
		}
		else if (this.m_startingPos.x < (float)this.m_missionPanelSliderPreviewWidth + single)
		{
			if (component.anchoredPosition.x < (float)this.m_missionPanelSliderPreviewWidth - single)
			{
				this.HideSliderPanel();
				return;
			}
			this.MaximizeSliderPanel();
			return;
		}
		if (this.SliderPanelBeginShrinkToPreviewPositionAction != null)
		{
			this.SliderPanelBeginShrinkToPreviewPositionAction();
		}
		this.ShowSliderPanel();
	}

	public void OnBeginDrag(BaseEventData eventData)
	{
		if (this.SliderPanelBeginDragAction != null)
		{
			this.SliderPanelBeginDragAction();
		}
		this.m_startingPos = base.transform.localPosition;
		Vector2 localPointInMapViewRT = AdventureMapPanel.instance.ScreenPointToLocalPointInMapViewRT(((PointerEventData)eventData).position);
		this.m_startingPointerPos = localPointInMapViewRT;
		this.m_busyMoving = false;
		this.m_movementIsPending = false;
		iTween.Stop(base.gameObject);
	}

	private void OnDoneSlidingInMaximize()
	{
		this.m_busyMoving = false;
		this.m_isShowing = true;
		if (this.m_hidePreviewWhenMaximized && !this.m_isHorizontal)
		{
			if (this.m_previewCanvasGroup != null)
			{
				this.m_previewCanvasGroup.alpha = 0f;
				this.m_previewCanvasGroup.blocksRaycasts = false;
			}
			if (this.m_maximizedCanvasGroup != null)
			{
				this.m_maximizedCanvasGroup.alpha = 1f;
			}
		}
		if (this.SliderPanelMaximizedAction != null)
		{
			this.SliderPanelMaximizedAction();
		}
	}

	private void OnDoneSlidingInPreview()
	{
		this.m_busyMoving = false;
		this.m_isShowing = true;
		if (this.m_hidePreviewWhenMaximized && !this.m_isHorizontal && this.m_previewCanvasGroup != null)
		{
			this.m_previewCanvasGroup.blocksRaycasts = true;
		}
	}

	public void OnDrag(BaseEventData eventData)
	{
		if (this.m_isHorizontal)
		{
			Vector2 localPointInMapViewRT = AdventureMapPanel.instance.ScreenPointToLocalPointInMapViewRT(((PointerEventData)eventData).position);
			float mStartingPointerPos = localPointInMapViewRT.x - this.m_startingPointerPos.x + this.m_startingPos.x;
			float mMissionPanelSliderFullWidth = (float)this.m_missionPanelSliderFullWidth / 2f;
			mStartingPointerPos = Mathf.Min(mStartingPointerPos, mMissionPanelSliderFullWidth);
			Transform vector3 = base.transform;
			float single = base.transform.localPosition.y;
			Vector3 vector31 = base.transform.localPosition;
			vector3.localPosition = new Vector3(mStartingPointerPos, single, vector31.z);
			RectTransform component = base.GetComponent<RectTransform>();
			if (this.m_stretchAbovePreviewWidth)
			{
				if (mStartingPointerPos <= (float)this.m_missionPanelSliderPreviewWidth - mMissionPanelSliderFullWidth)
				{
					Vector2 mMissionPanelSliderPreviewWidth = component.sizeDelta;
					mMissionPanelSliderPreviewWidth.x = (float)this.m_missionPanelSliderPreviewWidth;
					component.sizeDelta = mMissionPanelSliderPreviewWidth;
				}
				else
				{
					Vector2 vector2 = component.sizeDelta;
					vector2.x = mMissionPanelSliderFullWidth + mStartingPointerPos;
					component.sizeDelta = vector2;
				}
			}
		}
		else
		{
			Vector2 localPointInMapViewRT1 = AdventureMapPanel.instance.ScreenPointToLocalPointInMapViewRT(((PointerEventData)eventData).position);
			float mStartingPointerPos1 = localPointInMapViewRT1.y - this.m_startingPointerPos.y + this.m_startingPos.y;
			float mMissionPanelSliderFullHeight = (float)this.m_missionPanelSliderFullHeight / 2f;
			mStartingPointerPos1 = Mathf.Min(mStartingPointerPos1, mMissionPanelSliderFullHeight);
			Transform transforms = base.transform;
			float single1 = base.transform.localPosition.x;
			Vector3 vector32 = base.transform.localPosition;
			transforms.localPosition = new Vector3(single1, mStartingPointerPos1, vector32.z);
			RectTransform rectTransform = base.GetComponent<RectTransform>();
			if (this.m_stretchAbovePreviewHight)
			{
				if (mStartingPointerPos1 <= (float)this.m_missionPanelSliderPreviewHeight - mMissionPanelSliderFullHeight)
				{
					Vector2 mMissionPanelSliderPreviewHeight = rectTransform.sizeDelta;
					mMissionPanelSliderPreviewHeight.y = (float)this.m_missionPanelSliderPreviewHeight;
					rectTransform.sizeDelta = mMissionPanelSliderPreviewHeight;
				}
				else
				{
					Vector2 vector21 = rectTransform.sizeDelta;
					vector21.y = mMissionPanelSliderFullHeight + mStartingPointerPos1;
					rectTransform.sizeDelta = vector21;
				}
			}
		}
	}

	private void OnEnable()
	{
		this.m_busyMoving = false;
		this.m_movementIsPending = false;
		this.m_isShowing = false;
	}

	private void PanelSliderBottomTweenCallback(Vector2 val)
	{
		RectTransform component = base.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2(val.x, val.y);
		if (!this.m_isHorizontal)
		{
			if (this.m_stretchAbovePreviewHight)
			{
				if (val.y <= (float)this.m_missionPanelSliderPreviewHeight)
				{
					Vector2 mMissionPanelSliderPreviewHeight = component.sizeDelta;
					mMissionPanelSliderPreviewHeight.y = (float)this.m_missionPanelSliderPreviewHeight;
					component.sizeDelta = mMissionPanelSliderPreviewHeight;
				}
				else
				{
					Vector2 vector2 = component.sizeDelta;
					vector2.y = val.y;
					component.sizeDelta = vector2;
				}
			}
		}
		else if (this.m_stretchAbovePreviewWidth)
		{
			if (val.x <= (float)this.m_missionPanelSliderPreviewWidth)
			{
				Vector2 mMissionPanelSliderPreviewWidth = component.sizeDelta;
				mMissionPanelSliderPreviewWidth.x = (float)this.m_missionPanelSliderPreviewWidth;
				component.sizeDelta = mMissionPanelSliderPreviewWidth;
			}
			else
			{
				Vector2 vector21 = component.sizeDelta;
				vector21.x = val.x;
				component.sizeDelta = vector21;
			}
		}
		this.m_startingPos = val;
		if (this.m_hidePreviewWhenMaximized && !this.m_isHorizontal)
		{
			float single = (val.y - (float)this.m_missionPanelSliderPreviewHeight) / (float)(this.m_missionPanelSliderFullHeight - this.m_missionPanelSliderPreviewHeight);
			if (this.m_previewCanvasGroup != null)
			{
				this.m_previewCanvasGroup.alpha = 1f - single;
			}
			if (this.m_maximizedCanvasGroup != null)
			{
				this.m_maximizedCanvasGroup.alpha = single;
			}
		}
	}

	public void ShowSliderPanel()
	{
		if (this.m_busyMoving)
		{
			this.m_movementIsPending = true;
			this.m_pendingMovementIsToShowPreview = true;
			this.m_pendingMovementIsToMaximize = false;
			this.m_pendingMovementIsToHide = false;
			return;
		}
		this.m_busyMoving = true;
		if (this.m_masterCanvasGroup != null)
		{
			this.m_masterCanvasGroup.alpha = 1f;
		}
		Vector2 vector2 = new Vector2((float)this.m_missionPanelSliderPreviewWidth, (float)this.m_missionPanelSliderPreviewHeight);
		GameObject gameObject = base.gameObject;
		object[] component = new object[] { "name", "Slide Mission Details In (Bottom)", "from", base.GetComponent<RectTransform>().anchoredPosition, "to", vector2, "easeType", "easeOutCubic", "time", null, null, null, null, null };
		component[9] = (this.m_previewSeconds <= 0f ? 0.5f : this.m_previewSeconds);
		component[10] = "onupdate";
		component[11] = "PanelSliderBottomTweenCallback";
		component[12] = "oncomplete";
		component[13] = "OnDoneSlidingInPreview";
		iTween.ValueTo(gameObject, iTween.Hash(component));
	}

	private void Update()
	{
		if (this.m_movementIsPending && !this.m_busyMoving)
		{
			this.m_movementIsPending = false;
			if (this.m_pendingMovementIsToShowPreview)
			{
				this.ShowSliderPanel();
			}
			else if (this.m_pendingMovementIsToMaximize)
			{
				this.MaximizeSliderPanel();
			}
			else if (this.m_pendingMovementIsToHide)
			{
				this.HideSliderPanel();
			}
		}
	}
}