using System;
using UnityEngine;

public class ButtonSwipe : MonoBehaviour
{
	public float m_initialX;

	public float m_initialTouchX;

	public float m_currentTouchX;

	public ButtonSwipe()
	{
	}

	public void OnBeginDrag()
	{
		this.m_initialX = base.GetComponent<RectTransform>().localPosition.x;
		if (Input.touchCount <= 0)
		{
			this.m_initialTouchX = Input.mousePosition.x;
		}
		else
		{
			this.m_initialTouchX = Input.GetTouch(0).position.x;
		}
	}

	public void OnDrag()
	{
		if (Input.touchCount <= 0)
		{
			this.m_currentTouchX = Input.mousePosition.x;
		}
		else
		{
			this.m_currentTouchX = Input.GetTouch(0).position.x;
		}
		float mCurrentTouchX = this.m_currentTouchX - this.m_initialTouchX;
		RectTransform component = base.GetComponent<RectTransform>();
		Vector3 mInitialX = component.localPosition;
		mInitialX.x = this.m_initialX + mCurrentTouchX;
		component.localPosition = mInitialX;
	}

	public void OnEndDrag()
	{
	}
}