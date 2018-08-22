using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace WoWCompanionApp
{
	public class CharacterWebView : MonoBehaviour
	{
		private string Url = "https://worldofwarcraft.com/en-us/404";

		private WebViewObject webViewObject;

		private CharacterViewPanel characterViewPanel;

		public CharacterWebView()
		{
		}

		public void SetWebViewVisible(bool visible)
		{
			this.webViewObject.SetVisibility(visible);
		}

		[DebuggerHidden]
		private IEnumerator Start()
		{
			CharacterWebView.<Start>c__Iterator0 variable = null;
			return variable;
		}
	}
}