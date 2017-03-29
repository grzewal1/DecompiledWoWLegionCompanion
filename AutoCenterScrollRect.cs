using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoCenterScrollRect : MonoBehaviour
{
	public RectTransform m_scrollViewportRT;

	public RectTransform m_scrollRectContentsRT;

	public float m_autoCenterDuration;

	public iTween.EaseType m_easeType;

	public float m_minSwipeDistance;

	public float m_maxSwipeDistance;

	public HorizontalLayoutGroup m_horizontalLayoutGroup;

	private float m_targetX;

	private float m_touchStartX;

	private float m_touchEndX;

	private AutoCenterItem m_centeredItem;

	public AutoCenterScrollRect()
	{
	}

	private void Awake()
	{
		this.m_horizontalLayoutGroup = this.m_scrollRectContentsRT.GetComponentInChildren<HorizontalLayoutGroup>();
	}

	public void CenterOnItem(int itemIndex)
	{
		AutoCenterItem[] componentsInChildren = this.m_scrollRectContentsRT.GetComponentsInChildren<AutoCenterItem>(true);
		RectTransform component = componentsInChildren[itemIndex].GetComponent<RectTransform>();
		this.m_targetX = -component.anchoredPosition.x;
		if (componentsInChildren[itemIndex].IsCentered())
		{
			return;
		}
		AutoCenterItem[] autoCenterItemArray = componentsInChildren;
		for (int i = 0; i < (int)autoCenterItemArray.Length; i++)
		{
			autoCenterItemArray[i].SetCentered(false);
		}
		this.m_centeredItem = componentsInChildren[itemIndex];
		iTween.Stop(this.m_scrollRectContentsRT.gameObject);
		iTween.ValueTo(this.m_scrollRectContentsRT.gameObject, iTween.Hash(new object[] { "name", "autocenter", "from", this.m_scrollRectContentsRT.anchoredPosition.x, "to", this.m_targetX, "time", this.m_autoCenterDuration, "easetype", this.m_easeType, "onupdatetarget", base.gameObject, "onupdate", "OnAutoCenterUpdate", "oncompletetarget", base.gameObject, "oncomplete", "OnAutoCenterComplete" }));
	}

	private void OnAutoCenterComplete()
	{
		Vector3 mScrollRectContentsRT = this.m_scrollRectContentsRT.anchoredPosition;
		mScrollRectContentsRT.x = this.m_targetX;
		this.m_scrollRectContentsRT.anchoredPosition = mScrollRectContentsRT;
		this.m_centeredItem.SetCentered(true);
	}

	private void OnAutoCenterUpdate(float newX)
	{
		Vector3 mScrollRectContentsRT = this.m_scrollRectContentsRT.anchoredPosition;
		mScrollRectContentsRT.x = newX;
		this.m_scrollRectContentsRT.anchoredPosition = mScrollRectContentsRT;
	}

	public void OnTouchEnd(BaseEventData eventData)
	{
		this.OnTouchEnd(eventData, -1);
	}

	public void OnTouchEnd(BaseEventData eventData, int itemIndex)
	{
		Vector2 vector2;
		bool flag;
		if (eventData != null)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_scrollViewportRT, ((PointerEventData)eventData).position, Camera.main, out vector2);
			this.m_touchEndX = vector2.x;
		}
		bool mTouchEndX = this.m_touchEndX - this.m_touchStartX >= this.m_minSwipeDistance;
		bool mTouchStartX = this.m_touchStartX - this.m_touchEndX >= this.m_minSwipeDistance;
		bool flag1 = false;
		if (!mTouchEndX || this.m_touchEndX - this.m_touchStartX <= this.m_maxSwipeDistance)
		{
			flag = (!mTouchStartX ? false : this.m_touchStartX - this.m_touchEndX > this.m_maxSwipeDistance);
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		AutoCenterItem[] componentsInChildren = this.m_scrollRectContentsRT.GetComponentsInChildren<AutoCenterItem>(true);
		int num = 0;
		if (itemIndex < 0)
		{
			float single = -1f;
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				RectTransform component = componentsInChildren[i].GetComponent<RectTransform>();
				float mScrollRectContentsRT = -this.m_scrollRectContentsRT.anchoredPosition.x;
				Vector2 vector21 = component.anchoredPosition;
				float single1 = Mathf.Abs(mScrollRectContentsRT - vector21.x);
				if (single < 0f || single1 < single)
				{
					single = single1;
					num = i;
				}
			}
			if (num == 0 && this.m_scrollRectContentsRT.anchoredPosition.x > 0f)
			{
				flag1 = true;
			}
			if (num == (int)componentsInChildren.Length - 1)
			{
				RectTransform rectTransform = componentsInChildren[num].GetComponent<RectTransform>();
				if (-this.m_scrollRectContentsRT.anchoredPosition.x > rectTransform.anchoredPosition.x)
				{
					flag1 = true;
				}
			}
		}
		else
		{
			num = itemIndex;
		}
		if (flag1)
		{
			return;
		}
		if (eventData != null && !flag2)
		{
			if (mTouchStartX && num < (int)componentsInChildren.Length - 1)
			{
				num++;
			}
			if (mTouchEndX && num > 0)
			{
				num--;
			}
		}
		Vector2 component1 = componentsInChildren[num].GetComponent<RectTransform>().anchoredPosition;
		this.m_targetX = -component1.x + (float)this.m_horizontalLayoutGroup.padding.left;
		AutoCenterItem[] autoCenterItemArray = componentsInChildren;
		for (int j = 0; j < (int)autoCenterItemArray.Length; j++)
		{
			autoCenterItemArray[j].SetCentered(false);
		}
		this.m_centeredItem = componentsInChildren[num];
		iTween.Stop(this.m_scrollRectContentsRT.gameObject);
		iTween.ValueTo(this.m_scrollRectContentsRT.gameObject, iTween.Hash(new object[] { "name", "autocenter", "from", this.m_scrollRectContentsRT.anchoredPosition.x, "to", this.m_targetX, "time", this.m_autoCenterDuration, "easetype", this.m_easeType, "onupdatetarget", base.gameObject, "onupdate", "OnAutoCenterUpdate", "oncompletetarget", base.gameObject, "oncomplete", "OnAutoCenterComplete" }));
	}

	public void OnTouchStart(BaseEventData eventData)
	{
		Vector2 vector2;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_scrollViewportRT, ((PointerEventData)eventData).position, Camera.main, out vector2);
		this.m_touchStartX = vector2.x;
		iTween.Stop(this.m_scrollRectContentsRT.gameObject);
	}
}