using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanel : MonoBehaviour
{
	public Text m_logText;

	public Logger m_logger;

	public LogPanel()
	{
	}

	public void ClearLog()
	{
		this.m_logger.ClearLog();
		this.UpdateLogDisplay();
	}

	public void ClosePanel()
	{
		base.gameObject.SetActive(false);
	}

	public void ShowPanel()
	{
		base.gameObject.SetActive(true);
		this.UpdateLogDisplay();
	}

	public void UpdateLogDisplay()
	{
		int num = 1;
		this.m_logText.text = string.Empty;
		foreach (string logLine in this.m_logger.LogLines)
		{
			if (num > 1)
			{
				Text mLogText = this.m_logText;
				mLogText.text = string.Concat(mLogText.text, "\n\n");
			}
			Text text = this.m_logText;
			string str = text.text;
			text.text = string.Concat(new object[] { str, num, ") ", logLine });
			num++;
		}
	}
}