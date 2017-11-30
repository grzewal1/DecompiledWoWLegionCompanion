using System;
using UnityEngine;

public class ResetScaleOnEnable : MonoBehaviour
{
	public ResetScaleOnEnable()
	{
	}

	private void OnEnable()
	{
		base.transform.localScale = Vector3.one;
	}
}