using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class WorldQuestTooltip : MonoBehaviour
	{
		[Header("World Quest Icon Layers")]
		public Image m_dragonFrame;

		public Image m_background;

		public Image m_main;

		public Image m_expiringSoon;

		[Header("World Quest Info")]
		private Text m_worldQuestTimeText;

		public MissionRewardDisplay m_missionRewardDisplayPrefab;

		public GameObject m_worldQuestObjectiveRoot;

		public GameObject m_worldQuestObjectiveDisplayPrefab;

		public GameObject m_bountyLogoRoot;

		public BountySite m_bountyLogoPrefab;

		[Header("Misc")]
		public RewardInfoPopup[] m_rewardInfo;

		public Text m_rewardsLabel;

		private int m_questID;

		private const int WORLD_QUEST_TIME_LOW_MINUTES = 75;

		private DateTime m_endTime;

		private string m_timeLeftString;

		public WorldQuestTooltip()
		{
		}

		private void EnableAdditionalRewardDisplays(int highestActiveIndex)
		{
			this.m_rewardInfo[1].gameObject.SetActive(highestActiveIndex >= 1);
			this.m_rewardInfo[2].gameObject.SetActive(highestActiveIndex >= 2);
		}

		private void InitRewardInfoDisplay(WrapperWorldQuest worldQuest)
		{
			int num = 0;
			this.m_rewardInfo[0].gameObject.SetActive(true);
			this.m_rewardInfo[1].gameObject.SetActive(false);
			this.m_rewardInfo[2].gameObject.SetActive(false);
			if (worldQuest.Items != null && worldQuest.Items.Count<WrapperWorldQuestReward>() > 0)
			{
				foreach (WrapperWorldQuestReward item in worldQuest.Items)
				{
					Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, item.FileDataID);
					this.m_rewardInfo[num].SetReward(MissionRewardDisplay.RewardType.item, item.RecordID, item.Quantity, sprite, item.ItemContext);
					int num1 = num;
					num = num1 + 1;
					this.EnableAdditionalRewardDisplays(num1);
					if (num < 3)
					{
						continue;
					}
					return;
				}
			}
			else if (worldQuest.Currencies.Count<WrapperWorldQuestReward>() > 0)
			{
				foreach (WrapperWorldQuestReward currency in worldQuest.Currencies)
				{
					Sprite sprite1 = GeneralHelpers.LoadCurrencyIcon(currency.RecordID);
					CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(currency.RecordID);
					int quantity = currency.Quantity / ((record.Flags & 8) == 0 ? 1 : 100);
					this.m_rewardInfo[num].SetCurrency(currency.RecordID, quantity, sprite1);
					int num2 = num;
					num = num2 + 1;
					this.EnableAdditionalRewardDisplays(num2);
					if (num < 3)
					{
						continue;
					}
					return;
				}
			}
			else if (worldQuest.Money > 0)
			{
				Sprite sprite2 = Resources.Load<Sprite>("MiscIcons/INV_Misc_Coin_01");
				this.m_rewardInfo[num].SetGold(worldQuest.Money / 10000, sprite2);
				int num3 = num;
				num = num3 + 1;
				this.EnableAdditionalRewardDisplays(num3);
				if (num >= 3)
				{
					return;
				}
			}
			else if (worldQuest.Experience > 0)
			{
				Sprite localizedFollowerXpIcon = GeneralHelpers.GetLocalizedFollowerXpIcon();
				this.m_rewardInfo[num].SetFollowerXP(worldQuest.Experience, localizedFollowerXpIcon);
				int num4 = num;
				num = num4 + 1;
				this.EnableAdditionalRewardDisplays(num4);
				if (num >= 3)
				{
					return;
				}
			}
		}

		private void OnDisable()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PopBackAction();
		}

		public void OnEnable()
		{
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideAllPopups, null);
		}

		public void SetQuest(int questID)
		{
			this.m_expiringSoon.gameObject.SetActive(false);
			this.m_questID = questID;
			Transform[] componentsInChildren = this.m_worldQuestObjectiveRoot.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Transform transforms = componentsInChildren[i];
				if (transforms != null && transforms != this.m_worldQuestObjectiveRoot.transform)
				{
					UnityEngine.Object.Destroy(transforms.gameObject);
				}
			}
			WrapperWorldQuest item = WorldQuestData.WorldQuestDictionary[this.m_questID];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
			gameObject.transform.SetParent(this.m_worldQuestObjectiveRoot.transform, false);
			Text component = gameObject.GetComponent<Text>();
			component.text = item.QuestTitle;
			component.resizeTextMaxSize = 26;
			BountySite[] bountySiteArray = this.m_bountyLogoRoot.transform.GetComponentsInChildren<BountySite>(true);
			for (int j = 0; j < (int)bountySiteArray.Length; j++)
			{
				UnityEngine.Object.Destroy(bountySiteArray[j].gameObject);
			}
			if (PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(item.QuestID))
			{
				WrapperBountiesByWorldQuest wrapperBountiesByWorldQuest = PersistentBountyData.bountiesByWorldQuestDictionary[item.QuestID];
				for (int k = 0; k < wrapperBountiesByWorldQuest.BountyQuestIDs.Count; k++)
				{
					foreach (WrapperWorldQuestBounty value in PersistentBountyData.bountyDictionary.Values)
					{
						if (wrapperBountiesByWorldQuest.BountyQuestIDs[k] != value.QuestID)
						{
							continue;
						}
						QuestV2Rec record = StaticDB.questDB.GetRecord(value.QuestID);
						if (record == null)
						{
							continue;
						}
						GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
						gameObject1.transform.SetParent(this.m_worldQuestObjectiveRoot.transform, false);
						Text questTitle = gameObject1.GetComponent<Text>();
						questTitle.text = record.QuestTitle;
						questTitle.color = new Color(1f, 0.773f, 0f, 1f);
						BountySite bountySite = UnityEngine.Object.Instantiate<BountySite>(this.m_bountyLogoPrefab);
						bountySite.SetBounty(value);
						bountySite.transform.SetParent(this.m_bountyLogoRoot.transform, false);
					}
				}
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
			gameObject2.transform.SetParent(this.m_worldQuestObjectiveRoot.transform, false);
			this.m_worldQuestTimeText = gameObject2.GetComponent<Text>();
			this.m_worldQuestTimeText.text = item.QuestTitle;
			this.m_worldQuestTimeText.color = new Color(1f, 0.773f, 0f, 1f);
			foreach (WrapperWorldQuestObjective objective in item.Objectives)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
				gameObject3.transform.SetParent(this.m_worldQuestObjectiveRoot.transform, false);
				Text text = gameObject3.GetComponent<Text>();
				text.text = string.Concat("- ", objective.Text);
			}
			this.InitRewardInfoDisplay(item);
			this.m_endTime = item.EndTime;
			QuestInfoRec questInfoRec = StaticDB.questInfoDB.GetRecord(item.QuestInfoID);
			if (questInfoRec == null)
			{
				return;
			}
			bool modifiers = (questInfoRec.Modifiers & 2) != 0;
			this.m_dragonFrame.gameObject.SetActive(modifiers);
			if (questInfoRec.Type == 7)
			{
				this.m_background.sprite = Resources.Load<Sprite>("NewWorldQuest/Mobile-NormalQuest");
				this.m_main.sprite = Resources.Load<Sprite>("NewWorldQuest/Map-LegionInvasion-SargerasCrest");
				return;
			}
			this.m_background.sprite = Resources.Load<Sprite>("NewWorldQuest/Mobile-NormalQuest");
			if ((questInfoRec.Modifiers & 1) != 0 && questInfoRec.Type != 3)
			{
				this.m_background.sprite = Resources.Load<Sprite>("NewWorldQuest/Mobile-RareQuest");
			}
			if ((questInfoRec.Modifiers & 4) != 0 && questInfoRec.Type != 3)
			{
				this.m_background.sprite = Resources.Load<Sprite>("NewWorldQuest/Mobile-EpicQuest");
			}
			int uITextureAtlasMemberID = 0;
			string str = null;
			switch (questInfoRec.Type)
			{
				case 1:
				{
					int profession = questInfoRec.Profession;
					switch (profession)
					{
						case 182:
						{
							uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-herbalism");
							str = "Mobile-Herbalism";
							break;
						}
						case 185:
						{
							uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-cooking");
							str = "Mobile-Cooking";
							break;
						}
						case 186:
						{
							uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-mining");
							str = "Mobile-Mining";
							break;
						}
						default:
						{
							if (profession == 164)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-blacksmithing");
								str = "Mobile-Blacksmithing";
								break;
							}
							else if (profession == 165)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-leatherworking");
								str = "Mobile-Leatherworking";
								break;
							}
							else if (profession == 129)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-firstaid");
								str = "Mobile-FirstAid";
								break;
							}
							else if (profession == 171)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-alchemy");
								str = "Mobile-Alchemy";
								break;
							}
							else if (profession == 197)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-tailoring");
								str = "Mobile-Tailoring";
								break;
							}
							else if (profession == 202)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-engineering");
								str = "Mobile-Engineering";
								break;
							}
							else if (profession == 333)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-enchanting");
								str = "Mobile-Enchanting";
								break;
							}
							else if (profession == 356)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-fishing");
								str = "Mobile-Fishing";
								break;
							}
							else if (profession == 393)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-skinning");
								str = "Mobile-Skinning";
								break;
							}
							else if (profession == 755)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-jewelcrafting");
								str = "Mobile-Jewelcrafting";
								break;
							}
							else if (profession == 773)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-inscription");
								str = "Mobile-Inscription";
								break;
							}
							else if (profession == 794)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-archaeology");
								str = "Mobile-Archaeology";
								break;
							}
							else
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
								str = "Mobile-QuestExclamationIcon";
								break;
							}
						}
					}
					break;
				}
				case 2:
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
					str = "Mobile-QuestExclamationIcon";
					break;
				}
				case 3:
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-pvp-ffa");
					str = "Mobile-PVP";
					break;
				}
				case 4:
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-petbattle");
					str = "Mobile-Pets";
					break;
				}
				default:
				{
					goto case 2;
				}
			}
			if (str != null)
			{
				this.m_main.sprite = Resources.Load<Sprite>(string.Concat("NewWorldQuest/", str));
			}
			else if (uITextureAtlasMemberID > 0)
			{
				this.m_main.sprite = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID);
				this.m_main.SetNativeSize();
			}
			this.UpdateTimeRemaining();
		}

		private void Start()
		{
			this.m_rewardsLabel.font = FontLoader.LoadStandardFont();
			this.m_rewardsLabel.text = StaticDB.GetString("REWARDS", "Rewards");
			this.m_timeLeftString = StaticDB.GetString("TIME_LEFT", "Time Left: PH");
		}

		private void Update()
		{
			this.UpdateTimeRemaining();
		}

		private void UpdateTimeRemaining()
		{
			TimeSpan mEndTime = this.m_endTime - GarrisonStatus.CurrentTime();
			if (mEndTime.TotalSeconds < 0)
			{
				mEndTime = TimeSpan.Zero;
			}
			this.m_worldQuestTimeText.text = string.Concat(this.m_timeLeftString, " ", mEndTime.GetDurationString(false));
			this.m_expiringSoon.gameObject.SetActive(mEndTime.TotalSeconds < 4500);
		}
	}
}