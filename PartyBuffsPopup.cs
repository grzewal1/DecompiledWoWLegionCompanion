using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

public class PartyBuffsPopup : MonoBehaviour
{
	public Text m_partyBuffsLabel;

	public PartyBuffDisplay m_partyBuffDisplayPrefab;

	public GameObject m_partyBuffRoot;

	public PartyBuffsPopup()
	{
	}

	private void Awake()
	{
		this.m_partyBuffsLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_partyBuffsLabel.text = StaticDB.GetString("PARTY_BUFFS", "Party Buffs [PH]");
	}

	public void Init(int[] buffIDs)
	{
		PartyBuffDisplay[] componentsInChildren = this.m_partyBuffRoot.GetComponentsInChildren<PartyBuffDisplay>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
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

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_canvasBlurManager.RemoveBlurRef_Level2Canvas();
		Main.instance.m_backButtonManager.PopBackAction();
	}

	public void OnEnable()
	{
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_canvasBlurManager.AddBlurRef_Level2Canvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
	}
}