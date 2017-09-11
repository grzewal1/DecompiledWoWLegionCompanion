using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackableMapIconContainer : MonoBehaviour
{
	public GameObject m_countainerPreviewIconsGroup;

	public Text m_iconCount;

	public GameObject m_iconArea;

	public Canvas m_iconAreaCanvas;

	public CanvasGroup m_iconAreaCanvasGroup;

	public Image m_iconAreaBG;

	public List<StackableMapIcon> m_icons;

	public StackableMapIconContainer()
	{
	}

	public void AddStackableMapIcon(StackableMapIcon icon)
	{
		if (this.m_icons.Contains(icon))
		{
			return;
		}
		this.m_icons.Add(icon);
		icon.transform.SetParent(this.m_iconArea.transform, false);
		icon.transform.transform.localPosition = Vector3.zero;
		this.UpdateAppearance();
	}

	private void Awake()
	{
		this.m_icons = new List<StackableMapIcon>();
	}

	public int GetIconCount()
	{
		return this.m_icons.Count;
	}

	public Rect GetWorldRect()
	{
		Vector3[] vector3Array = new Vector3[4];
		base.GetComponent<RectTransform>().GetWorldCorners(vector3Array);
		float single = vector3Array[2].x - vector3Array[0].x;
		float single1 = vector3Array[2].y - vector3Array[0].y;
		float mZoomFactor = AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor;
		single *= mZoomFactor;
		single1 *= mZoomFactor;
		Rect rect = new Rect(vector3Array[0].x, vector3Array[0].y, single, single1);
		return rect;
	}

	private void HandleSelectedIconContainerChanged(StackableMapIconContainer container)
	{
		if (container != this)
		{
			this.ShowIconArea(false);
		}
		else if (this.GetIconCount() > 1)
		{
			this.ShowIconArea(true);
		}
	}

	private void OnDisable()
	{
		AdventureMapPanel.instance.SelectedIconContainerChanged -= new Action<StackableMapIconContainer>(this.HandleSelectedIconContainerChanged);
	}

	private void OnEnable()
	{
		this.ShowIconArea(false);
		AdventureMapPanel.instance.SelectedIconContainerChanged += new Action<StackableMapIconContainer>(this.HandleSelectedIconContainerChanged);
	}

	public void PlayTapSound()
	{
		Main.instance.m_UISound.Play_SelectWorldQuest();
	}

	public void RemoveStackableMapIcon(StackableMapIcon icon)
	{
		if (this.m_icons.Contains(icon))
		{
			if (this.GetIconCount() == 1)
			{
				StackableMapIconManager.RemoveStackableMapIconContainer(this);
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			this.m_icons.Remove(icon);
		}
		this.UpdateAppearance();
	}

	public void ShowIconArea(bool show)
	{
		if (!show && this.GetIconCount() <= 1)
		{
			return;
		}
		this.m_iconAreaCanvasGroup.gameObject.SetActive(show);
		if (!show || this.GetIconCount() <= 1)
		{
			this.m_iconAreaCanvas.sortingOrder = 1;
		}
		else
		{
			this.m_iconAreaCanvas.sortingOrder = 2;
		}
	}

	public void ShowIconAreaBG(bool show)
	{
		this.m_iconAreaBG.gameObject.SetActive(show);
	}

	public void ToggleIconList()
	{
		this.PlayTapSound();
		this.ShowIconArea(!this.m_iconArea.activeSelf);
		if (!this.m_iconArea.activeSelf)
		{
			AdventureMapPanel.instance.SetSelectedIconContainer(null);
		}
		else
		{
			AdventureMapPanel.instance.SetSelectedIconContainer(this);
			UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 3f, 0f);
		}
	}

	private void UpdateAppearance()
	{
		if (this.m_icons == null || this.GetIconCount() == 0)
		{
			StackableMapIconManager.RemoveStackableMapIconContainer(this);
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		this.ShowIconArea(false);
		if (this.GetIconCount() != 1)
		{
			this.m_countainerPreviewIconsGroup.SetActive(true);
			this.ShowIconAreaBG(true);
		}
		else
		{
			this.m_countainerPreviewIconsGroup.SetActive(false);
			this.ShowIconArea(true);
			this.ShowIconAreaBG(false);
		}
		this.m_iconCount.text = string.Concat(string.Empty, this.GetIconCount());
		base.gameObject.name = string.Concat("IconContainer (", (this.GetIconCount() <= 0 ? "Single" : string.Concat(string.Empty, this.GetIconCount())), ")");
		base.gameObject.SetActive(true);
	}
}