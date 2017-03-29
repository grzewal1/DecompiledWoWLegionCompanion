using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;
using WowStatConstants;
using WowStaticData;

public class EquipmentDialog : MonoBehaviour
{
	public AbilityDisplay m_abilityDisplay;

	public Text m_titleText;

	public Text m_abilityNameText;

	public Text m_abilityDescription;

	public Text m_noEquipmentMessage;

	public FollowerInventoryListItem m_equipmentListItemPrefab;

	public GameObject m_equipmentListContent;

	private int m_garrAbilityID;

	private FollowerDetailView m_followerDetailView;

	public EquipmentDialog()
	{
	}

	private void Awake()
	{
		this.m_titleText.font = GeneralHelpers.LoadFancyFont();
		this.m_titleText.text = StaticDB.GetString("EQUIPMENT", null);
		this.m_noEquipmentMessage.font = GeneralHelpers.LoadStandardFont();
		this.m_noEquipmentMessage.text = StaticDB.GetString("NO_EQUIPMENT2", "You do not have any Champion Equipment to equip.");
	}

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PopBackAction();
		Main.instance.EquipmentInventoryChangedAction -= new Action(this.UpdateDisplayCB);
		this.m_garrAbilityID = 0;
		this.m_followerDetailView = null;
	}

	public void OnEnable()
	{
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
		Main.instance.EquipmentInventoryChangedAction += new Action(this.UpdateDisplayCB);
		MobilePlayerFollowerEquipmentRequest mobilePlayerFollowerEquipmentRequest = new MobilePlayerFollowerEquipmentRequest()
		{
			GarrFollowerTypeID = 4
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerEquipmentRequest);
	}

	public void SetAbility(int garrAbilityID, FollowerDetailView followerDetailView)
	{
		GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(garrAbilityID);
		if (record == null)
		{
			Debug.LogWarning(string.Concat("Invalid garrAbilityID ", garrAbilityID));
			return;
		}
		this.m_garrAbilityID = garrAbilityID;
		this.m_followerDetailView = followerDetailView;
		this.m_abilityNameText.text = record.Name;
		this.m_abilityDescription.text = WowTextParser.parser.Parse(record.Description, 0);
		this.m_abilityDescription.supportRichText = WowTextParser.parser.IsRichText();
		this.m_abilityDisplay.SetAbility(garrAbilityID, true, true, null);
		this.UpdateEquipmentDisplay(garrAbilityID, followerDetailView);
	}

	private void UpdateDisplayCB()
	{
		if (this.m_garrAbilityID > 0 && this.m_followerDetailView != null)
		{
			this.UpdateEquipmentDisplay(this.m_garrAbilityID, this.m_followerDetailView);
		}
	}

	private void UpdateEquipmentDisplay(int garrAbilityID, FollowerDetailView followerDetailView)
	{
		FollowerInventoryListItem[] componentsInChildren = this.m_equipmentListContent.GetComponentsInChildren<FollowerInventoryListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		bool flag = true;
		IEnumerator enumerator = PersistentEquipmentData.equipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileFollowerEquipment current = (MobileFollowerEquipment)enumerator.Current;
				GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(current.GarrAbilityID);
				if (record != null)
				{
					if ((record.Flags & 64) == 0)
					{
						FollowerInventoryListItem followerInventoryListItem = UnityEngine.Object.Instantiate<FollowerInventoryListItem>(this.m_equipmentListItemPrefab);
						followerInventoryListItem.transform.SetParent(this.m_equipmentListContent.transform, false);
						followerInventoryListItem.SetEquipment(current, followerDetailView, garrAbilityID);
						flag = false;
					}
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
		this.m_noEquipmentMessage.gameObject.SetActive(flag);
	}
}