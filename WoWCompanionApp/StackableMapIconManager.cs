using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoWCompanionApp
{
	public class StackableMapIconManager : MonoBehaviour
	{
		public GameObject m_stackableMapContainerPrefab;

		private List<StackableMapIconContainer> m_containers;

		private static StackableMapIconManager s_instance;

		public StackableMapIconManager()
		{
		}

		private void Awake()
		{
			StackableMapIconManager.s_instance = this;
			this.m_containers = new List<StackableMapIconContainer>();
		}

		public static void RegisterStackableMapIcon(StackableMapIcon icon)
		{
			if (StackableMapIconManager.s_instance == null)
			{
				Debug.LogError("ERROR: RegisterStackableMapIcon with null s_instance");
				return;
			}
			if (icon == null)
			{
				Debug.LogError("ERROR: RegisterStackableMapIcon with null icon");
				return;
			}
			bool flag = false;
			foreach (StackableMapIconContainer mContainer in StackableMapIconManager.s_instance.m_containers)
			{
				if (mContainer != null)
				{
					if (mContainer.gameObject.activeSelf)
					{
						if (mContainer.m_startLocationMapID == icon.m_startLocationMapID)
						{
							Rect worldRect = mContainer.GetWorldRect();
							if (!icon.GetWorldRect().Overlaps(worldRect))
							{
								continue;
							}
							mContainer.AddStackableMapIcon(icon);
							icon.SetContainer(mContainer);
							StackableMapIconManager.s_instance.m_containers.Add(mContainer);
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StackableMapIconManager.s_instance.m_stackableMapContainerPrefab);
				gameObject.transform.SetParent(icon.transform.parent, false);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				RectTransform rectTransform = icon.GetComponent<RectTransform>();
				component.anchorMin = rectTransform.anchorMin;
				component.anchorMax = rectTransform.anchorMax;
				component.sizeDelta = icon.m_iconBoundsRT.sizeDelta;
				component.anchoredPosition = Vector2.zero;
				StackableMapIconContainer mStartLocationMapID = gameObject.GetComponent<StackableMapIconContainer>();
				if (mStartLocationMapID == null)
				{
					Debug.LogError("ERROR: containerObj has no StackableMapIconContainer!!");
				}
				else
				{
					mStartLocationMapID.m_startLocationMapID = icon.m_startLocationMapID;
					mStartLocationMapID.AddStackableMapIcon(icon);
					icon.SetContainer(mStartLocationMapID);
					StackableMapIconManager.s_instance.m_containers.Add(mStartLocationMapID);
				}
			}
		}

		public static void RemoveStackableMapIconContainer(StackableMapIconContainer container)
		{
			if (StackableMapIconManager.s_instance != null && StackableMapIconManager.s_instance.m_containers.Contains(container))
			{
				StackableMapIconManager.s_instance.m_containers.Remove(container);
			}
		}
	}
}