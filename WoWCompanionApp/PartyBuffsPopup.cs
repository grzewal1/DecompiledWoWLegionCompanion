using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class PartyBuffsPopup : BaseDialog
	{
		public PartyBuffDisplay m_partyBuffDisplayPrefab;

		public GameObject m_partyBuffRoot;

		public PartyBuffsPopup()
		{
		}

		private void Awake()
		{
		}

		public void Init(int[] buffIDs)
		{
			PartyBuffDisplay[] componentsInChildren = this.m_partyBuffRoot.GetComponentsInChildren<PartyBuffDisplay>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			int[] numArray = buffIDs;
			for (int j = 0; j < (int)numArray.Length; j++)
			{
				int num = numArray[j];
				PartyBuffDisplay partyBuffDisplay = UnityEngine.Object.Instantiate<PartyBuffDisplay>(this.m_partyBuffDisplayPrefab);
				partyBuffDisplay.transform.SetParent(this.m_partyBuffRoot.transform, false);
				partyBuffDisplay.SetAbility(num);
				if ((int)buffIDs.Length > 7)
				{
					partyBuffDisplay.UseReducedHeight();
					if ((int)buffIDs.Length > 9)
					{
						VerticalLayoutGroup component = this.m_partyBuffRoot.GetComponent<VerticalLayoutGroup>();
						if (component != null)
						{
							component.spacing = 3f;
						}
					}
				}
			}
		}
	}
}