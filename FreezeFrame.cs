using System;
using UnityEngine;

public class FreezeFrame : MonoBehaviour
{
	public FreezeFrame()
	{
	}

	private void MakeSnapshot()
	{
		DarkRoom.MakeSnapshot(this);
	}

	private void Start()
	{
		this.MakeSnapshot();
	}
}