using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class CombatAllyDialog : MonoBehaviour
	{
		public FollowerInventoryListItem m_combatAllyChampionListItemPrefab;

		public GameObject m_combatAllyListContent;

		public Text m_combatAllyCost;

		public Image m_combatAllyCostResourceIcon;

		public Text m_titleText;

		public CombatAllyDialog()
		{
		}

		private void CreateCombatAllyItems(int combatAllyMissionID, int combatAllyMissionCost)
		{
			foreach (WrapperGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
			{
				FollowerStatus followerStatus = GeneralHelpers.GetFollowerStatus(value);
				if (value.ZoneSupportSpellID <= 0 || followerStatus != FollowerStatus.available && followerStatus != FollowerStatus.onMission)
				{
					continue;
				}
				FollowerInventoryListItem followerInventoryListItem = UnityEngine.Object.Instantiate<FollowerInventoryListItem>(this.m_combatAllyChampionListItemPrefab);
				followerInventoryListItem.transform.SetParent(this.m_combatAllyListContent.transform, false);
				followerInventoryListItem.SetCombatAllyChampion(value, combatAllyMissionID, combatAllyMissionCost);
			}
		}

		public void Init()
		{
			FollowerInventoryListItem[] componentsInChildren = this.m_combatAllyListContent.GetComponentsInChildren<FollowerInventoryListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			int missionCost = 0;
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null)
				{
					if ((record.Flags & 16) == 0)
					{
						continue;
					}
					this.CreateCombatAllyItems(value.MissionRecID, (int)record.MissionCost);
					missionCost = (int)record.MissionCost;
					break;
				}
			}
			if (missionCost > GarrisonStatus.WarResources())
			{
				this.m_combatAllyCost.text = string.Concat(new object[] { StaticDB.GetString("COST2", "Cost:"), " <color=#ff0000ff>", missionCost, "</color>" });
			}
			else
			{
				this.m_combatAllyCost.text = string.Concat(new object[] { StaticDB.GetString("COST2", "Cost:"), " <color=#ffffffff>", missionCost, "</color>" });
			}
			Sprite sprite = GeneralHelpers.LoadCurrencyIcon(1560);
			if (sprite != null)
			{
				this.m_combatAllyCostResourceIcon.sprite = sprite;
			}
		}

		private void OnDisable()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PopBackAction();
		}

		public void OnEnable()
		{
			Main.instance.m_UISound.Play_ShowGenericTooltip();
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideAllPopups, null);
		}

		public void Start()
		{
			this.m_combatAllyCost.font = GeneralHelpers.LoadStandardFont();
			this.m_titleText.text = StaticDB.GetString("COMBAT_ALLY", null);
		}
	}
}