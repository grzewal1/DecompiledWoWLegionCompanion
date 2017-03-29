using System;
using UnityEngine;

public class modelAutoScaler : MonoBehaviour
{
	public Camera mainCamera;

	public bool isPortrait;

	public modelAutoScaler()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!this.isPortrait)
		{
			float single = 1.33333337f / this.mainCamera.aspect;
			base.gameObject.transform.localScale = new Vector3(single, single, single);
		}
		else
		{
			float single1 = 0.75f / this.mainCamera.aspect;
			base.gameObject.transform.localScale = new Vector3(single1, single1, single1);
		}
	}
}