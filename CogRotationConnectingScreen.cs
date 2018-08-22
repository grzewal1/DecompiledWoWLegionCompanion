using System;
using UnityEngine;

public class CogRotationConnectingScreen : MonoBehaviour
{
	public float speed;

	public CogRotationConnectingScreen()
	{
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.back, this.speed * Time.deltaTime);
	}
}