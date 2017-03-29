using System;
using UnityEngine;
using UnityEngine.UI;

public class ExceptionPanel : MonoBehaviour
{
	public Text m_exceptionText;

	public ExceptionPanel()
	{
	}

	public void OnDismiss()
	{
		base.gameObject.SetActive(false);
	}

	public void SetExceptionText(string text)
	{
		this.m_exceptionText.text = text;
	}
}