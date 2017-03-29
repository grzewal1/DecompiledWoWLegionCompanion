using System;
using UnityEngine;

public class DarkRoomCamera : MonoBehaviour
{
	public GameObject m_drCanvasObject;

	private int wtf;

	public DarkRoomCamera()
	{
	}

	private void Awake()
	{
		this.wtf = 0;
	}

	private void OnPostRender()
	{
		DarkRoomCamera darkRoomCamera = this;
		int num = darkRoomCamera.wtf + 1;
		int num1 = num;
		darkRoomCamera.wtf = num;
		if (num1 <= 1)
		{
			return;
		}
	}
}