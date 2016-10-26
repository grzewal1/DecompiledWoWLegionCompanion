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

	private void Awake()
	{
		this.m_titleText.set_font(GeneralHelpers.LoadFancyFont());
		this.m_titleText.set_text(StaticDB.GetString("EQUIPMENT", null));
		this.m_noEquipmentMessage.set_font(GeneralHelpers.LoadStandardFont());
		this.m_noEquipmentMessage.set_text(StaticDB.GetString("NO_EQUIPMENT2", "You do not have any Champion Equipment to equip."));
	}

	public void OnEnable()
	{
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
		Main expr_34 = Main.instance;
		expr_34.EquipmentInventoryChangedAction = (Action)Delegate.Combine(expr_34.EquipmentInventoryChangedAction, new Action(this.UpdateDisplayCB));
		MobilePlayerFollowerEquipmentRequest mobilePlayerFollowerEquipmentRequest = new MobilePlayerFollowerEquipmentRequest();
		mobilePlayerFollowerEquipmentRequest.GarrFollowerTypeID = 4;
		Login.instance.SendToMobileServer(mobilePlayerFollowerEquipmentRequest);
	}

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PopBackAction();
		Main expr_24 = Main.instance;
		expr_24.EquipmentInventoryChangedAction = (Action)Delegate.Remove(expr_24.EquipmentInventoryChangedAction, new Action(this.UpdateDisplayCB));
		this.m_garrAbilityID = 0;
		this.m_followerDetailView = null;
	}

	public void SetAbility(int garrAbilityID, FollowerDetailView followerDetailView)
	{
		GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(garrAbilityID);
		if (record == null)
		{
			Debug.LogWarning("Invalid garrAbilityID " + garrAbilityID);
			return;
		}
		this.m_garrAbilityID = garrAbilityID;
		this.m_followerDetailView = followerDetailView;
		this.m_abilityNameText.set_text(record.Name);
		this.m_abilityDescription.set_text(WowTextParser.parser.Parse(record.Description, 0));
		this.m_abilityDescription.set_supportRichText(WowTextParser.parser.IsRichText());
		this.m_abilityDisplay.SetAbility(garrAbilityID, true, true, null);
		this.UpdateEquipmentDisplay(garrAbilityID, followerDetailView);
	}

	private void UpdateEquipmentDisplay(int garrAbilityID, FollowerDetailView followerDetailView)
	{
		FollowerInventoryListItem[] componentsInChildren = this.m_equipmentListContent.GetComponentsInChildren<FollowerInventoryListItem>(true);
		FollowerInventoryListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			FollowerInventoryListItem followerInventoryListItem = array[i];
			Object.DestroyImmediate(followerInventoryListItem.get_gameObject());
		}
		bool active = true;
		IEnumerator enumerator = PersistentEquipmentData.equipmentDictionary.get_Values().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileFollowerEquipment mobileFollowerEquipment = (MobileFollowerEquipment)enumerator.get_Current();
				GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(mobileFollowerEquipment.GarrAbilityID);
				if (record != null)
				{
					if ((record.Flags & 64u) == 0u)
					{
						FollowerInventoryListItem followerInventoryListItem2 = Object.Instantiate<FollowerInventoryListItem>(this.m_equipmentListItemPrefab);
						followerInventoryListItem2.get_transform().SetParent(this.m_equipmentListContent.get_transform(), false);
						followerInventoryListItem2.SetEquipment(mobileFollowerEquipment, followerDetailView, garrAbilityID);
						active = false;
					}
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		this.m_noEquipmentMessage.get_gameObject().SetActive(active);
	}

	private void UpdateDisplayCB()
	{
		if (this.m_garrAbilityID > 0 && this.m_followerDetailView != null)
		{
			this.UpdateEquipmentDisplay(this.m_garrAbilityID, this.m_followerDetailView);
		}
	}
}
