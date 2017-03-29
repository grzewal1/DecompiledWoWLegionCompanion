using System;
using UnityEngine;
using UnityEngine.UI;

public class CollectLootListItem : MonoBehaviour
{
	public Text completedMissionsText;

	public CollectLootListItem()
	{
	}

	public void CompleteAllMissions()
	{
		Main.instance.CompleteAllMissions();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}