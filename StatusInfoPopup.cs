using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusInfoPopup : MonoBehaviour
{
	public GameObject m_popupView;

	public Text m_statusText;

	public StatusInfoPopup()
	{
	}

	private void Awake()
	{
		this.m_popupView.SetActive(false);
	}

	public void Hide()
	{
		this.m_popupView.SetActive(false);
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
	}

	public void SetStatusText(string statusText)
	{
		this.m_statusText.text = statusText;
	}

	public void Show()
	{
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		this.m_popupView.SetActive(true);
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
	}
}