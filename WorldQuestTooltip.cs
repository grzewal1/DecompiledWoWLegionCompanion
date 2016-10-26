using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;
using WowStatConstants;
using WowStaticData;

public class WorldQuestTooltip : MonoBehaviour
{
	private const int WORLD_QUEST_TIME_LOW_MINUTES = 75;

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
	public RewardInfoPopup m_rewardInfo;

	public Text m_rewardsLabel;

	private int m_questID;

	private long m_endTime;

	private string m_timeLeftString;

	private void Start()
	{
		this.m_rewardsLabel.set_font(GeneralHelpers.LoadStandardFont());
		this.m_rewardsLabel.set_text(StaticDB.GetString("REWARDS", "Rewards"));
		this.m_timeLeftString = StaticDB.GetString("TIME_LEFT", "Time Left: PH");
	}

	public void OnEnable()
	{
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
	}

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PopBackAction();
	}

	private void InitRewardInfoDisplay(MobileWorldQuest worldQuest)
	{
		if (worldQuest.Item != null && Enumerable.Count<MobileWorldQuestReward>(worldQuest.Item) > 0)
		{
			MobileWorldQuestReward[] item = worldQuest.Item;
			int num = 0;
			if (num < item.Length)
			{
				MobileWorldQuestReward mobileWorldQuestReward = item[num];
				Sprite rewardSprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, mobileWorldQuestReward.FileDataID);
				this.m_rewardInfo.SetReward(MissionRewardDisplay.RewardType.item, mobileWorldQuestReward.RecordID, mobileWorldQuestReward.Quantity, rewardSprite, mobileWorldQuestReward.ItemContext);
			}
		}
		else if (Enumerable.Count<MobileWorldQuestReward>(worldQuest.Currency) > 0)
		{
			MobileWorldQuestReward[] currency = worldQuest.Currency;
			int num2 = 0;
			if (num2 < currency.Length)
			{
				MobileWorldQuestReward mobileWorldQuestReward2 = currency[num2];
				Sprite iconSprite = GeneralHelpers.LoadCurrencyIcon(mobileWorldQuestReward2.RecordID);
				CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(mobileWorldQuestReward2.RecordID);
				int quantity = mobileWorldQuestReward2.Quantity / (((record.Flags & 8u) == 0u) ? 1 : 100);
				this.m_rewardInfo.SetCurrency(mobileWorldQuestReward2.RecordID, quantity, iconSprite);
			}
		}
		else if (worldQuest.Money > 0)
		{
			Sprite iconSprite2 = Resources.Load<Sprite>("MiscIcons/INV_Misc_Coin_01");
			this.m_rewardInfo.SetGold(worldQuest.Money / 10000, iconSprite2);
		}
		else if (worldQuest.Experience > 0)
		{
			Sprite localizedFollowerXpIcon = GeneralHelpers.GetLocalizedFollowerXpIcon();
			this.m_rewardInfo.SetFollowerXP(worldQuest.Experience, localizedFollowerXpIcon);
		}
	}

	public void SetQuest(int questID)
	{
		this.m_expiringSoon.get_gameObject().SetActive(false);
		this.m_questID = questID;
		Transform[] componentsInChildren = this.m_worldQuestObjectiveRoot.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform != null && transform != this.m_worldQuestObjectiveRoot.get_transform())
			{
				Object.DestroyImmediate(transform.get_gameObject());
			}
		}
		MobileWorldQuest mobileWorldQuest = (MobileWorldQuest)WorldQuestData.worldQuestDictionary.get_Item(this.m_questID);
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
		gameObject.get_transform().SetParent(this.m_worldQuestObjectiveRoot.get_transform(), false);
		Text component = gameObject.GetComponent<Text>();
		component.set_text(mobileWorldQuest.QuestTitle);
		component.set_resizeTextMaxSize(26);
		BountySite[] componentsInChildren2 = this.m_bountyLogoRoot.get_transform().GetComponentsInChildren<BountySite>(true);
		BountySite[] array2 = componentsInChildren2;
		for (int j = 0; j < array2.Length; j++)
		{
			BountySite bountySite = array2[j];
			Object.DestroyImmediate(bountySite.get_gameObject());
		}
		if (PersistentBountyData.bountiesByWorldQuestDictionary.ContainsKey(mobileWorldQuest.QuestID))
		{
			MobileBountiesByWorldQuest mobileBountiesByWorldQuest = (MobileBountiesByWorldQuest)PersistentBountyData.bountiesByWorldQuestDictionary.get_Item(mobileWorldQuest.QuestID);
			for (int k = 0; k < mobileBountiesByWorldQuest.BountyQuestID.Length; k++)
			{
				IEnumerator enumerator = PersistentBountyData.bountyDictionary.get_Values().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MobileWorldQuestBounty mobileWorldQuestBounty = (MobileWorldQuestBounty)enumerator.get_Current();
						if (mobileBountiesByWorldQuest.BountyQuestID[k] == mobileWorldQuestBounty.QuestID)
						{
							QuestV2Rec record = StaticDB.questDB.GetRecord(mobileWorldQuestBounty.QuestID);
							if (record != null)
							{
								GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
								gameObject2.get_transform().SetParent(this.m_worldQuestObjectiveRoot.get_transform(), false);
								this.m_worldQuestTimeText = gameObject2.GetComponent<Text>();
								this.m_worldQuestTimeText.set_text(record.QuestTitle);
								this.m_worldQuestTimeText.set_horizontalOverflow(1);
								this.m_worldQuestTimeText.set_color(new Color(1f, 0.773f, 0f, 1f));
								BountySite bountySite2 = Object.Instantiate<BountySite>(this.m_bountyLogoPrefab);
								bountySite2.SetBounty(mobileWorldQuestBounty);
								bountySite2.get_transform().SetParent(this.m_bountyLogoRoot.get_transform(), false);
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
			}
		}
		GameObject gameObject3 = Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
		gameObject3.get_transform().SetParent(this.m_worldQuestObjectiveRoot.get_transform(), false);
		this.m_worldQuestTimeText = gameObject3.GetComponent<Text>();
		this.m_worldQuestTimeText.set_text(mobileWorldQuest.QuestTitle);
		this.m_worldQuestTimeText.set_horizontalOverflow(1);
		this.m_worldQuestTimeText.set_color(new Color(1f, 0.773f, 0f, 1f));
		using (IEnumerator<MobileWorldQuestObjective> enumerator2 = Enumerable.AsEnumerable<MobileWorldQuestObjective>(mobileWorldQuest.Objective).GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				MobileWorldQuestObjective current = enumerator2.get_Current();
				GameObject gameObject4 = Object.Instantiate<GameObject>(this.m_worldQuestObjectiveDisplayPrefab);
				gameObject4.get_transform().SetParent(this.m_worldQuestObjectiveRoot.get_transform(), false);
				Text component2 = gameObject4.GetComponent<Text>();
				component2.set_text("-" + current.Text);
			}
		}
		this.InitRewardInfoDisplay(mobileWorldQuest);
		this.m_endTime = (long)(mobileWorldQuest.EndTime - 900);
		QuestInfoRec record2 = StaticDB.questInfoDB.GetRecord(mobileWorldQuest.QuestInfoID);
		if (record2 == null)
		{
			return;
		}
		bool active = (record2.Modifiers & 2) != 0;
		this.m_dragonFrame.get_gameObject().SetActive(active);
		this.m_background.set_sprite(Resources.Load<Sprite>("NewWorldQuest/Mobile-NormalQuest"));
		bool flag = (record2.Modifiers & 1) != 0;
		if (flag && record2.Type != 3)
		{
			this.m_background.set_sprite(Resources.Load<Sprite>("NewWorldQuest/Mobile-RareQuest"));
		}
		bool flag2 = (record2.Modifiers & 4) != 0;
		if (flag2 && record2.Type != 3)
		{
			this.m_background.set_sprite(Resources.Load<Sprite>("NewWorldQuest/Mobile-EpicQuest"));
		}
		int uITextureAtlasMemberID;
		string text;
		switch (record2.Type)
		{
		case 1:
		{
			int profession = record2.Profession;
			switch (profession)
			{
			case 182:
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-herbalism");
				text = "Mobile-Herbalism";
				goto IL_6CB;
			case 183:
			case 184:
				IL_4B6:
				if (profession == 164)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-blacksmithing");
					text = "Mobile-Blacksmithing";
					goto IL_6CB;
				}
				if (profession == 165)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-leatherworking");
					text = "Mobile-Leatherworking";
					goto IL_6CB;
				}
				if (profession == 129)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-firstaid");
					text = "Mobile-FirstAid";
					goto IL_6CB;
				}
				if (profession == 171)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-alchemy");
					text = "Mobile-Alchemy";
					goto IL_6CB;
				}
				if (profession == 197)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-tailoring");
					text = "Mobile-Tailoring";
					goto IL_6CB;
				}
				if (profession == 202)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-engineering");
					text = "Mobile-Engineering";
					goto IL_6CB;
				}
				if (profession == 333)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-enchanting");
					text = "Mobile-Enchanting";
					goto IL_6CB;
				}
				if (profession == 356)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-fishing");
					text = "Mobile-Fishing";
					goto IL_6CB;
				}
				if (profession == 393)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-skinning");
					text = "Mobile-Skinning";
					goto IL_6CB;
				}
				if (profession == 755)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-jewelcrafting");
					text = "Mobile-Jewelcrafting";
					goto IL_6CB;
				}
				if (profession == 773)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-inscription");
					text = "Mobile-Inscription";
					goto IL_6CB;
				}
				if (profession != 794)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
					text = "Mobile-QuestExclamationIcon";
					goto IL_6CB;
				}
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-archaeology");
				text = "Mobile-Archaeology";
				goto IL_6CB;
			case 185:
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-cooking");
				text = "Mobile-Cooking";
				goto IL_6CB;
			case 186:
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-mining");
				text = "Mobile-Mining";
				goto IL_6CB;
			}
			goto IL_4B6;
			IL_6CB:
			goto IL_718;
		}
		case 3:
			uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-pvp-ffa");
			text = "Mobile-PVP";
			goto IL_718;
		case 4:
			uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-petbattle");
			text = "Mobile-Pets";
			goto IL_718;
		}
		uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
		text = "Mobile-QuestExclamationIcon";
		IL_718:
		if (text != null)
		{
			this.m_main.set_sprite(Resources.Load<Sprite>("NewWorldQuest/" + text));
		}
		else if (uITextureAtlasMemberID > 0)
		{
			this.m_main.set_sprite(TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID));
			this.m_main.SetNativeSize();
		}
		this.UpdateTimeRemaining();
	}

	private void UpdateTimeRemaining()
	{
		int num = (int)(this.m_endTime - GarrisonStatus.CurrentTime());
		if (num < 0)
		{
			num = 0;
		}
		Duration duration = new Duration(num, false);
		this.m_worldQuestTimeText.set_text(this.m_timeLeftString + " " + duration.DurationString);
		bool active = num < 4500;
		this.m_expiringSoon.get_gameObject().SetActive(active);
	}

	private void Update()
	{
		this.UpdateTimeRemaining();
	}
}
