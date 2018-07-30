using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class ConnectingPanel : MonoBehaviour
	{
		public float m_fadeInDuration;

		public float m_fadeOutDuration;

		public CanvasGroup[] m_fadeCanvasGroups;

		private bool m_isFadingIn;

		private bool m_isFadingOut;

		private float m_fadeInTimeElapsed;

		private float m_fadeOutTimeElapsed;

		public ConnectingPanel()
		{
		}

		public void Hide()
		{
			this.m_isFadingIn = false;
			this.m_isFadingOut = true;
			this.m_fadeOutTimeElapsed = 0f;
		}

		private void OnEnable()
		{
			this.m_isFadingIn = true;
			this.m_isFadingOut = false;
			this.m_fadeInTimeElapsed = 0f;
			CanvasGroup[] mFadeCanvasGroups = this.m_fadeCanvasGroups;
			for (int i = 0; i < (int)mFadeCanvasGroups.Length; i++)
			{
				mFadeCanvasGroups[i].alpha = 0f;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (this.m_isFadingIn && this.m_fadeInTimeElapsed < this.m_fadeInDuration)
			{
				this.m_fadeInTimeElapsed += Time.deltaTime;
				float single = Mathf.Clamp(this.m_fadeInTimeElapsed / this.m_fadeInDuration, 0f, 1f);
				CanvasGroup[] mFadeCanvasGroups = this.m_fadeCanvasGroups;
				for (int i = 0; i < (int)mFadeCanvasGroups.Length; i++)
				{
					mFadeCanvasGroups[i].alpha = single;
				}
			}
			if (this.m_isFadingOut && this.m_fadeOutTimeElapsed < this.m_fadeOutDuration)
			{
				this.m_fadeOutTimeElapsed += Time.deltaTime;
				float single1 = 1f - Mathf.Clamp(this.m_fadeOutTimeElapsed / this.m_fadeOutDuration, 0f, 1f);
				CanvasGroup[] canvasGroupArray = this.m_fadeCanvasGroups;
				for (int j = 0; j < (int)canvasGroupArray.Length; j++)
				{
					canvasGroupArray[j].alpha = single1;
				}
				if (this.m_fadeOutTimeElapsed > this.m_fadeOutDuration)
				{
					this.m_isFadingOut = false;
					base.gameObject.SetActive(false);
				}
			}
		}
	}
}