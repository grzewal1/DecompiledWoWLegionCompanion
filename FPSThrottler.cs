using System;
using UnityEngine;

public class FPSThrottler : MonoBehaviour
{
	public int m_normalFPS;

	public int m_throttledFPS;

	public int m_sleepFPS;

	public int m_hybernateFPS;

	public float m_throttleDelay;

	public float m_sleepDelay;

	public float m_hybernateDelay;

	private float m_timeSinceLastTouch;

	public FPSThrottler()
	{
	}

	private void Awake()
	{
		this.m_timeSinceLastTouch = this.m_throttleDelay;
	}

	private void Update()
	{
		if (AllPanels.instance != null && (!AllPanels.instance.m_orderHallMultiPanel.gameObject.activeSelf || AllPanels.instance.m_missionResultsPanel.m_popupView.activeSelf))
		{
			this.m_timeSinceLastTouch = 0f;
			Application.targetFrameRate = this.m_normalFPS;
			return;
		}
		if (Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			this.m_timeSinceLastTouch = 0f;
			Application.targetFrameRate = this.m_normalFPS;
			return;
		}
		FPSThrottler mTimeSinceLastTouch = this;
		mTimeSinceLastTouch.m_timeSinceLastTouch = mTimeSinceLastTouch.m_timeSinceLastTouch + Time.deltaTime;
		if (this.m_timeSinceLastTouch > this.m_hybernateDelay)
		{
			Application.targetFrameRate = this.m_hybernateFPS;
			return;
		}
		if (this.m_timeSinceLastTouch > this.m_sleepDelay)
		{
			Application.targetFrameRate = this.m_sleepFPS;
			return;
		}
		if (this.m_timeSinceLastTouch > this.m_throttleDelay)
		{
			Application.targetFrameRate = this.m_throttledFPS;
		}
	}
}