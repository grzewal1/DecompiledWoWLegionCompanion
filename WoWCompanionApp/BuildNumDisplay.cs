using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	[RequireComponent(typeof(Text))]
	[RequireComponent(typeof(DebugObject))]
	public class BuildNumDisplay : MonoBehaviour
	{
		public BuildNumDisplay()
		{
		}

		private void Start()
		{
			base.GetComponent<Text>().text = string.Concat("Build num: ", MobileBuild.GetBuildNum());
		}
	}
}