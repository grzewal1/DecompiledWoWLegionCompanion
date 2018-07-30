using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace WoWCompanionApp
{
	public class DebugPrinter : Singleton<DebugPrinter>
	{
		private Queue<DebugPrinter.DebugPrintInfo> debugInfo = new Queue<DebugPrinter.DebugPrintInfo>();

		public DebugPrinter()
		{
		}

		public static void Log(string message, UnityEngine.Object context = null, DebugPrinter.LogLevel level = 0)
		{
			Queue<DebugPrinter.DebugPrintInfo> instance = Singleton<DebugPrinter>.Instance.debugInfo;
			DebugPrinter.DebugPrintInfo debugPrintInfo = new DebugPrinter.DebugPrintInfo()
			{
				Message = string.Concat(message, "\n", (new StackTrace(1, true)).ToString()),
				Level = level,
				Context = context
			};
			instance.Enqueue(debugPrintInfo);
		}

		private void Update()
		{
			foreach (DebugPrinter.DebugPrintInfo debugPrintInfo in this.debugInfo)
			{
				DebugPrinter.LogLevel level = debugPrintInfo.Level;
				if (level == DebugPrinter.LogLevel.Info)
				{
					UnityEngine.Debug.Log(debugPrintInfo.Message, debugPrintInfo.Context);
				}
				else if (level == DebugPrinter.LogLevel.Warning)
				{
					UnityEngine.Debug.LogWarning(debugPrintInfo.Message, debugPrintInfo.Context);
				}
				else if (level == DebugPrinter.LogLevel.Error)
				{
					UnityEngine.Debug.LogError(debugPrintInfo.Message, debugPrintInfo.Context);
				}
			}
			this.debugInfo.Clear();
		}

		private struct DebugPrintInfo
		{
			public string Message;

			public DebugPrinter.LogLevel Level;

			public UnityEngine.Object Context;
		}

		public enum LogLevel
		{
			Info,
			Warning,
			Error
		}
	}
}