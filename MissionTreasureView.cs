using System;
using UnityEngine;
using UnityEngine.UI;

public class MissionTreasureView : MonoBehaviour
{
	public Text m_chanceText;

	public MissionTreasureView()
	{
	}

	private void Start()
	{
		this.m_chanceText.text = StaticDB.GetString("CHANCE", null);
	}

	private void Update()
	{
	}
}