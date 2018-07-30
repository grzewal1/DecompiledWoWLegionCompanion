using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoWCompanionApp
{
	public class Logger : MonoBehaviour
	{
		public LogPanel m_panelScript;

		public int m_maxLogLines;

		private WoWCompanionApp.Logger.PanelLogHandler m_panelLogHandler;

		private List<string> m_logLines;

		public List<string> LogLines
		{
			get
			{
				return this.m_logLines;
			}
			set
			{
			}
		}

		public Logger()
		{
		}

		public void AddLogLine(string newLine)
		{
			this.m_logLines.Add(newLine);
			if (this.m_logLines.Count > this.m_maxLogLines)
			{
				this.m_logLines.RemoveAt(0);
			}
			string empty = string.Empty;
			int count = this.m_logLines.Count - 1;
			if (count >= 0)
			{
				int num = count - 19;
				if (num < 0)
				{
					num = 0;
				}
				for (int i = count; i >= num; i--)
				{
					string str = empty;
					empty = string.Concat(new object[] { str, 1 + i - num, ": ", this.m_logLines[i], "\n" });
				}
				Main.instance.SetDebugText(empty);
			}
		}

		public void ClearLog()
		{
			this.m_logLines.Clear();
		}

		private void Start()
		{
			this.m_panelLogHandler = new WoWCompanionApp.Logger.PanelLogHandler(this, this.m_panelScript);
			this.m_panelLogHandler == null;
			this.m_logLines = new List<string>();
		}

		public class PanelLogHandler : ILogHandler
		{
			private ILogHandler m_DefaultLogHandler;

			private WoWCompanionApp.Logger m_loggerScript;

			private LogPanel m_panelScript;

			public PanelLogHandler(WoWCompanionApp.Logger loggerScript, LogPanel panelScript)
			{
				this.m_loggerScript = loggerScript;
				this.m_panelScript = panelScript;
			}

			public void LogException(Exception exception, UnityEngine.Object context)
			{
				this.m_DefaultLogHandler.LogException(exception, context);
			}

			public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
			{
				this.m_loggerScript.AddLogLine(string.Format(format, args));
				this.m_DefaultLogHandler.LogFormat(logType, context, format, args);
				if (this.m_panelScript != null && this.m_panelScript.gameObject.activeSelf)
				{
					this.m_panelScript.UpdateLogDisplay();
				}
			}
		}
	}
}