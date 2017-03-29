using System;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingPanel : MonoBehaviour
{
	public Text m_statusText;

	public Text m_cancelText;

	public GameObject m_cancelButton;

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
		if (StaticDB.StringsAvailable())
		{
			this.m_cancelText.text = StaticDB.GetString("CANCEL", null);
		}
	}

	private void Start()
	{
		this.m_cancelText.font = GeneralHelpers.LoadStandardFont();
	}

	private void Update()
	{
		if (StaticDB.StringsAvailable())
		{
			if (!this.m_cancelButton.activeSelf)
			{
				this.m_cancelButton.SetActive(true);
				this.m_cancelText.text = StaticDB.GetString("CANCEL", null);
			}
		}
		else if (this.m_cancelButton.activeSelf)
		{
			this.m_cancelButton.SetActive(false);
		}
		if (this.m_isFadingIn && this.m_fadeInTimeElapsed < this.m_fadeInDuration)
		{
			ConnectingPanel mFadeInTimeElapsed = this;
			mFadeInTimeElapsed.m_fadeInTimeElapsed = mFadeInTimeElapsed.m_fadeInTimeElapsed + Time.deltaTime;
			float single = Mathf.Clamp(this.m_fadeInTimeElapsed / this.m_fadeInDuration, 0f, 1f);
			CanvasGroup[] mFadeCanvasGroups = this.m_fadeCanvasGroups;
			for (int i = 0; i < (int)mFadeCanvasGroups.Length; i++)
			{
				mFadeCanvasGroups[i].alpha = single;
			}
		}
		if (this.m_isFadingOut && this.m_fadeOutTimeElapsed < this.m_fadeOutDuration)
		{
			ConnectingPanel mFadeOutTimeElapsed = this;
			mFadeOutTimeElapsed.m_fadeOutTimeElapsed = mFadeOutTimeElapsed.m_fadeOutTimeElapsed + Time.deltaTime;
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