using System;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
	public GameObject[] m_objectsToSpin;

	public float m_spinSpeed;

	public LoadingSpinner()
	{
	}

	private void Update()
	{
		GameObject[] mObjectsToSpin = this.m_objectsToSpin;
		for (int i = 0; i < (int)mObjectsToSpin.Length; i++)
		{
			GameObject gameObject = mObjectsToSpin[i];
			gameObject.transform.Rotate(0f, 0f, this.m_spinSpeed * Time.deltaTime);
		}
	}
}