using System;
using UnityEngine;

public class SlidingPanel : MonoBehaviour
{
	private bool m_closing;

	private bool m_sliding;

	public float m_slideDuration;

	public SlidingPanel.SlideDirection m_slideDirection;

	public Action m_closedAction;

	public Action m_openedAction;

	public SlidingPanel()
	{
	}

	private void FinishedMoving()
	{
		if (this.m_closing && this.m_closedAction != null)
		{
			this.m_closedAction();
		}
		else if (this.m_openedAction != null)
		{
			this.m_openedAction();
		}
		this.m_sliding = false;
		this.m_closing = false;
	}

	private Vector2 GetOffsetFromSlideDirection(Rect rect)
	{
		float single;
		float single1;
		if (this.IsHorizontalSlide())
		{
			single = (this.m_slideDirection != SlidingPanel.SlideDirection.FromLeft ? rect.xMin - rect.xMax : rect.xMax - rect.xMin);
		}
		else
		{
			single = 0f;
		}
		float single2 = single;
		if (this.IsVerticalSlide())
		{
			single1 = (this.m_slideDirection != SlidingPanel.SlideDirection.FromTop ? rect.yMin - rect.yMax : rect.yMax - rect.yMin);
		}
		else
		{
			single1 = 0f;
		}
		return new Vector2(single2, single1);
	}

	private bool IsHorizontalSlide()
	{
		return (this.m_slideDirection == SlidingPanel.SlideDirection.FromLeft ? true : this.m_slideDirection == SlidingPanel.SlideDirection.FromRight);
	}

	public bool IsSliding()
	{
		return this.m_sliding;
	}

	private bool IsVerticalSlide()
	{
		return (this.m_slideDirection == SlidingPanel.SlideDirection.FromTop ? true : this.m_slideDirection == SlidingPanel.SlideDirection.FromBottom);
	}

	private void SetAnchoredPosition(Vector2 position)
	{
		base.GetComponent<RectTransform>().anchoredPosition = position;
	}

	public void SlideIn()
	{
		this.StartSlide(true);
	}

	public void SlideOut()
	{
		this.StartSlide(false);
	}

	private void StartSlide(bool slidingIn)
	{
		if (!this.m_sliding)
		{
			this.m_sliding = true;
			RectTransform component = base.GetComponent<RectTransform>();
			Vector2 offsetFromSlideDirection = this.GetOffsetFromSlideDirection(component.rect);
			if (!slidingIn)
			{
				offsetFromSlideDirection *= -1f;
				this.m_closing = true;
			}
			iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "from", component.anchoredPosition, "to", component.anchoredPosition + offsetFromSlideDirection, "time", this.m_slideDuration, "onupdate", "SetAnchoredPosition", "oncomplete", "FinishedMoving", "oncompletetarget", base.gameObject }));
		}
	}

	public enum SlideDirection
	{
		FromTop,
		FromBottom,
		FromLeft,
		FromRight
	}
}