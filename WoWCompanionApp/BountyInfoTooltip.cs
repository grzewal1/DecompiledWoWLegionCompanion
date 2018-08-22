using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

		public RewardInfoPopup m_rewardInfo;

		private WrapperWorldQuestBounty m_bounty;

		public BountyInfoTooltip()
		{
		}

		public void SetBounty(WrapperWorldQuestBounty bounty)
		{
			Sprite sprite;
			this.m_bounty = bounty;
			Sprite sprite1 = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, bounty.IconFileDataID);
			if (sprite1 == null)
			{
				this.m_bountyIconInvalidFileDataID.gameObject.SetActive(true);
				this.m_bountyIconInvalidFileDataID.text = string.Concat(string.Empty, bounty.IconFileDataID);
			}
			else
			{
				this.m_bountyIconInvalidFileDataID.gameObject.SetActive(false);
				this.m_bountyIcon.sprite = sprite1;
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
					rectTransform.SetParent(null);
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
			bounty.Items.RemoveAll((WrapperWorldQuestReward item) => (item.RecordID == 157831 ? true : item.RecordID == 1500));
			if (bounty.Items.Count > 0 && StaticDB.itemDB.GetRecord(bounty.Items[0].RecordID) != null)
			{
				WrapperWorldQuestReward wrapperWorldQuestReward = bounty.Items[0];
				Sprite sprite2 = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, wrapperWorldQuestReward.FileDataID);
				this.m_rewardInfo.SetReward(MissionRewardDisplay.RewardType.item, wrapperWorldQuestReward.RecordID, wrapperWorldQuestReward.Quantity, sprite2, wrapperWorldQuestReward.ItemContext);
			}
			else if (bounty.Money > 1000000)
			{
				Sprite sprite3 = Resources.Load<Sprite>("MiscIcons/INV_Misc_Coin_01");
				this.m_rewardInfo.SetGold(bounty.Money / 10000, sprite3);
			}
			else if (bounty.Currencies.Count > 1)
			{
				int num = 0;
				foreach (WrapperWorldQuestReward currency in bounty.Currencies)
				{
					CurrencyTypesRec currencyTypesRec = StaticDB.currencyTypesDB.GetRecord(currency.RecordID);
					if (currency.RecordID == 1553 && currencyTypesRec != null)
					{
						if (CurrencyContainerDB.CheckAndGetValidCurrencyContainer(currency.RecordID, currency.Quantity) == null)
						{
							sprite = GeneralHelpers.LoadCurrencyIcon(currency.RecordID);
							int quantity = currency.Quantity / ((currencyTypesRec.Flags & 8) == 0 ? 1 : 100);
							if (quantity <= num)
							{
								continue;
							}
							num = quantity;
							this.m_rewardInfo.SetCurrency(currency.RecordID, num, sprite);
						}
						else
						{
							sprite = CurrencyContainerDB.LoadCurrencyContainerIcon(currency.RecordID, currency.Quantity);
							int quantity1 = currency.Quantity / ((currencyTypesRec.Flags & 8) == 0 ? 1 : 100);
							if (quantity1 > num)
							{
								num = quantity1;
								this.m_rewardInfo.SetCurrency(currency.RecordID, num, sprite);
							}
						}
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