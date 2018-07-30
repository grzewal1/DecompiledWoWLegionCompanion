using System;
using UnityEngine;

public class EventsPanel : MonoBehaviour
{
	public GameObject m_scrollContent;

	public GameObject m_eventObjectPrefab;

	public GameObject m_dateObjectPrefab;

	public EventsPanel()
	{
	}

	private GameObject AddObjectToContent(GameObject prefab)
	{
		GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(prefab);
		vector3.transform.SetParent(this.m_scrollContent.transform);
		vector3.transform.SetAsLastSibling();
		vector3.transform.localScale = new Vector3(1f, 1f, 1f);
		return vector3;
	}

	private void ClearContent()
	{
		for (int i = this.m_scrollContent.transform.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(this.m_scrollContent.transform.GetChild(i).gameObject);
		}
		this.m_scrollContent.transform.DetachChildren();
	}

	private void OnEnable()
	{
	}

	private void RefreshEvents()
	{
		this.ClearContent();
	}

	private void Start()
	{
		base.gameObject.transform.localScale = Vector3.one;
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		Vector2 vector2 = Vector2.zero;
		base.gameObject.GetComponent<RectTransform>().offsetMax = vector2;
		component.offsetMin = vector2;
	}
}