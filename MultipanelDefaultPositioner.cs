using System;
using UnityEngine;

public class MultipanelDefaultPositioner : MonoBehaviour
{
	public MultipanelDefaultPositioner()
	{
	}

	private void Start()
	{
		Vector3 vector3 = base.gameObject.transform.localPosition;
		vector3.x = -1690f;
		base.gameObject.transform.localPosition = vector3;
	}
}