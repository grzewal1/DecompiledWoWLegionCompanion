using System;
using UnityEngine;
using UnityEngine.UI;

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