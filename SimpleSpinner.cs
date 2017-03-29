using System;
using UnityEngine;

public class SimpleSpinner : MonoBehaviour
{
	public float m_spinSpeed;

	public SimpleSpinner()
	{
	}

	private void Update()
	{
		base.transform.Rotate(0f, 0f, this.m_spinSpeed * Time.deltaTime);
	}
}