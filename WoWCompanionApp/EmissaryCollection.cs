using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class EmissaryCollection : MonoBehaviour
	{
		public GameObject m_collectionObject;

		public CanvasGroup m_canvasGroup;

		public EmissaryCollection()
		{
		}

		public void AddBountyObjectToCollection(GameObject obj)
		{
			if ((int)this.m_collectionObject.GetComponentsInChildren<BountySite>().Length < 3)
			{
				obj.transform.SetParent(this.m_collectionObject.transform, false);
				base.gameObject.SetActive(true);
			}
		}

		public void ClearCollection()
		{
			BountySite[] componentsInChildren = this.m_collectionObject.GetComponentsInChildren<BountySite>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i]);
			}
			this.m_collectionObject.DetachAllChildren();
			base.gameObject.SetActive(false);
		}
	}
}