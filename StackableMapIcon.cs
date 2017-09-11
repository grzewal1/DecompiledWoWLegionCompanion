using System;
using UnityEngine;

public class StackableMapIcon : MonoBehaviour
{
	public RectTransform m_iconBoundsRT;

	private StackableMapIconContainer m_iconContainer;

	public StackableMapIcon()
	{
	}

	public StackableMapIconContainer GetContainer()
	{
		return this.m_iconContainer;
	}

	public Rect GetWorldRect()
	{
		Vector3[] vector3Array = new Vector3[4];
		this.m_iconBoundsRT.GetWorldCorners(vector3Array);
		float single = vector3Array[2].x - vector3Array[0].x;
		float single1 = vector3Array[2].y - vector3Array[0].y;
		float mZoomFactor = AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor;
		single *= mZoomFactor;
		single1 *= mZoomFactor;
		Rect rect = new Rect(vector3Array[0].x, vector3Array[0].y, single, single1);
		return rect;
	}

	public void RegisterWithManager()
	{
		StackableMapIconManager.RegisterStackableMapIcon(this);
	}

	public void RemoveFromContainer()
	{
		if (this.m_iconContainer != null)
		{
			this.m_iconContainer.RemoveStackableMapIcon(this);
		}
	}

	public void SetContainer(StackableMapIconContainer iconContainer)
	{
		this.m_iconContainer = iconContainer;
	}
}