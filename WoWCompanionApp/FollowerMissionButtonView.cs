using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class FollowerMissionButtonView : MonoBehaviour
	{
		public Text followerMissionToggleButtonText;

		public FollowerMissionButtonView()
		{
		}

		public void ShowMissionList()
		{
			Main.instance.allPanels.ShowAdventureMap();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}