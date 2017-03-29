using System;
using UnityEngine;

public class ScaleToFill : MonoBehaviour
{
	private float nativeHeight;

	public ScaleToFill()
	{
	}

	private void Start()
	{
		this.nativeHeight = base.GetComponent<RectTransform>().rect.height;
	}

	private void Update()
	{
		RectTransform component = base.transform.parent.gameObject.GetComponent<RectTransform>();
		float single = component.rect.height / this.nativeHeight;
		base.transform.localScale = new Vector3(single, single, single);
	}
}