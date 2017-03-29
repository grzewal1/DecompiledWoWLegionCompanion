using System;
using UnityEngine;

public class FollowerModelSpinningBase : MonoBehaviour
{
	public float inititalYRotation;

	private float initialTouchX;

	public FollowerModelSpinningBase()
	{
	}

	public void OnBeginDrag()
	{
		this.inititalYRotation = base.transform.localEulerAngles.y;
		if (Input.touchCount <= 0)
		{
			this.initialTouchX = Input.mousePosition.x;
		}
		else
		{
			this.initialTouchX = Input.GetTouch(0).position.x;
		}
	}

	public void OnDrag()
	{
		float single = 0f;
		single = (Input.touchCount <= 0 ? Input.mousePosition.x : Input.GetTouch(0).position.x);
		float single1 = (this.initialTouchX - single) / (float)Screen.width;
		single1 = single1 * 2f;
		base.transform.localRotation = Quaternion.identity;
		base.transform.Rotate(0f, this.inititalYRotation + single1 * 360f, 0f, Space.Self);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}