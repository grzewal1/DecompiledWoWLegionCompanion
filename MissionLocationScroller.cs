using System;
using UnityEngine;

public class MissionLocationScroller : MonoBehaviour
{
	public float scrollSpeed;

	public float imageWidth;

	private RectTransform m_myRT;

	public MissionLocationScroller()
	{
	}

	private void Awake()
	{
		this.m_myRT = base.GetComponent<RectTransform>();
	}

	private void Update()
	{
		Vector2 mMyRT = this.m_myRT.anchoredPosition;
		mMyRT.x = mMyRT.x + this.scrollSpeed * Time.deltaTime;
		this.m_myRT.anchoredPosition = mMyRT;
		float single = this.m_myRT.anchoredPosition.x;
		Vector3 vector3 = this.m_myRT.localScale;
		if (single <= -this.imageWidth * 0.5f * vector3.x)
		{
			mMyRT = this.m_myRT.anchoredPosition;
			mMyRT.x = 0f;
			this.m_myRT.anchoredPosition = mMyRT;
		}
	}
}