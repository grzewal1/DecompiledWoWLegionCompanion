using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class BountyInfoTooltip : BaseDialog
	{
		public Image m_bountyIcon;

		public Text m_bountyIconInvalidFileDataID;

		public Text m_bountyName;

		public Text m_timeLeft;

		public Text m_bountyDescription;

		public GameObject m_bountyQuestCompleteIconPrefab;

		public GameObject m_bountyQuestAvailableIconPrefab;

		public Transform m_bountyQuestIconArea;

		public Image m_lootIcon;

		public Text m_lootIconInvalidFileDataID;

		public Text m_lootName;

		public Text m_lootDescription;

		public Text m_rewardsLabel;

		private WrapperWorldQuestBounty m_bounty;

		public BountyInfoTooltip()
		{
		}

		public void SetBounty(WrapperWorldQuestBounty bounty)
		{
			this.m_bounty = bounty;
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, bounty.IconFileDataID);
			if (sprite == null)
			{
				this.m_bountyIconInvalidFileDataID.gameObject.SetActive(true);
				this.m_bountyIconInvalidFileDataID.text = string.Concat(string.Empty, bounty.IconFileDataID);
			}
			else
			{
				this.m_bountyIconInvalidFileDataID.gameObject.SetActive(false);
				this.m_bountyIcon.sprite = sprite;
			}
			QuestV2Rec record = StaticDB.questDB.GetRecord(bounty.QuestID);
			if (record == null)
			{
				this.m_bountyName.text = string.Concat("Unknown Quest ID ", bounty.QuestID);
				this.m_bountyDescription.text = string.Concat("Unknown Quest ID ", bounty.QuestID);
			}
			else
			{
				this.m_bountyName.text = record.QuestTitle;
				this.m_bountyDescription.text = string.Concat(new object[] { string.Empty, bounty.NumCompleted, "/", bounty.NumNeeded, " ", record.LogDescription });
			}
			this.m_timeLeft.text = StaticDB.GetString("TIME_LEFT", "Time Left: PH");
			RectTransform[] componentsInChildren = this.m_bountyQuestIconArea.GetComponentsInChildren<RectTransform>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				RectTransform rectTransform = componentsInChildren[i];
				if (rectTransform != null && rectTransform.gameObject != this.m_bountyQuestIconArea.gameObject)
				{
					UnityEngine.Object.Destroy(rectTransform.gameObject);
				}
			}
			for (int j = 0; j < bounty.NumCompleted; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_bountyQuestCompleteIconPrefab);
				gameObject.transform.SetParent(this.m_bountyQuestIconArea.transform, false);
			}
			for (int k = 0; k < bounty.NumNeeded - bounty.NumCompleted; k++)
			{
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_bountyQuestAvailableIconPrefab);
				gameObject1.transform.SetParent(this.m_bountyQuestIconArea.transform, false);
			}
			this.UpdateTimeRemaining();
			if (bounty.Items.Count <= 0)
			{
				this.m_lootName.text = "ERROR: Loot Not Specified";
				this.m_lootDescription.text = "ERROR: Loot Not Specified";
			}
			else
			{
				ItemDB itemDB = StaticDB.itemDB;
				WrapperWorldQuestReward item = bounty.Items[0];
				ItemRec itemRec = itemDB.GetRecord(item.RecordID);
				if (itemRec == null)
				{
					Text mLootName = this.m_lootName;
					WrapperWorldQuestReward wrapperWorldQuestReward = bounty.Items[0];
					mLootName.text = string.Concat("Unknown item ", wrapperWorldQuestReward.RecordID);
					Text mLootDescription = this.m_lootDescription;
					WrapperWorldQuestReward item1 = bounty.Items[0];
					mLootDescription.text = string.Concat("Unknown item ", item1.RecordID);
				}
				else
				{
					this.m_lootName.text = itemRec.Display;
					this.m_lootDescription.text = GeneralHelpers.GetItemDescription(itemRec);
					Sprite sprite1 = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, itemRec.IconFileDataID);
					if (sprite1 != null)
					{
						this.m_lootIcon.sprite = sprite1;
					}
					else if (this.m_lootIconInvalidFileDataID != null)
					{
						this.m_lootIconInvalidFileDataID.gameObject.SetActive(true);
						this.m_lootIconInvalidFileDataID.text = string.Concat(string.Empty, itemRec.IconFileDataID);
					}
				}
			}
		}

		private void Update()
		{
			this.UpdateTimeRemaining();
		}

		private void UpdateTimeRemaining()
		{
			TimeSpan endTime = this.m_bounty.EndTime - GarrisonStatus.CurrentTime();
			endTime = (endTime.TotalSeconds <= 0 ? TimeSpan.Zero : endTime);
			this.m_timeLeft.text = string.Concat(StaticDB.GetString("TIME_LEFT", "Time Left: PH"), " ", endTime.GetDurationString(false));
		}
	}
}