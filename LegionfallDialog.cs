using System;
using UnityEngine;
using UnityEngine.UI;

public class LegionfallDialog : MonoBehaviour
{
	public LegionfallBuildingPanel m_legionfallBuildingPanelPrefab;

	public Transform m_content;

	public LegionfallDialog()
	{
	}

	private void OnEnable()
	{
		LegionfallBuildingPanel[] componentsInChildren = this.m_content.gameObject.GetComponentsInChildren<LegionfallBuildingPanel>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		LegionfallBuildingPanel legionfallBuildingPanel = UnityEngine.Object.Instantiate<LegionfallBuildingPanel>(this.m_legionfallBuildingPanelPrefab);
		legionfallBuildingPanel.InitPanel(1, 46277);
		legionfallBuildingPanel.transform.SetParent(this.m_content, false);
		LegionfallBuildingPanel legionfallBuildingPanel1 = UnityEngine.Object.Instantiate<LegionfallBuildingPanel>(this.m_legionfallBuildingPanelPrefab);
		legionfallBuildingPanel1.InitPanel(3, 46735);
		legionfallBuildingPanel1.transform.SetParent(this.m_content, false);
		LegionfallBuildingPanel legionfallBuildingPanel2 = UnityEngine.Object.Instantiate<LegionfallBuildingPanel>(this.m_legionfallBuildingPanelPrefab);
		legionfallBuildingPanel2.InitPanel(4, 46736);
		legionfallBuildingPanel2.transform.SetParent(this.m_content, false);
	}

	private void Start()
	{
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		LayoutElement layoutElement = this.m_legionfallBuildingPanelPrefab.GetComponent<LayoutElement>();
		HorizontalLayoutGroup horizontalLayoutGroup = this.m_content.GetComponent<HorizontalLayoutGroup>();
		RectOffset rectOffset = horizontalLayoutGroup.padding;
		Rect rect = component.rect;
		rectOffset.left = (int)((rect.width - layoutElement.minWidth) / 2f);
		horizontalLayoutGroup.padding.right = horizontalLayoutGroup.padding.left;
		horizontalLayoutGroup.spacing = (float)(horizontalLayoutGroup.padding.left - 39);
	}
}