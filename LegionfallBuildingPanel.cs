using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;
using WowStaticData;

public class LegionfallBuildingPanel : MonoBehaviour
{
	public SpellDisplay m_legionfallBuffSpellDisplayPrefab;

	public Text m_buildingName;

	public Image m_buildingStateImage;

	public Image m_buildingStateImageFrame_building;

	public Image m_buildingStateImageFrame_active;

	public Image m_buildingStateImageFrame_underAttack;

	public Image m_buildingStateImageFrame_destroyed;

	public Text m_buildingState;

	public Text m_buildingDescription;

	public Transform m_buffArea;

	public Image m_progressFillBar;

	public RectTransform m_progressFillGlow;

	public Image m_progressFillGlowGlow;

	public Text m_costLabel;

	public Text m_cost;

	public Image m_currencyIcon;

	public Text m_contributeButtonLabel;

	public Button m_contributeButton;

	public Image m_progressBarGlow;

	public float m_glowDuration;

	public GameObject m_contributeArea;

	public GameObject m_cantContributeArea;

	public Text m_cantContributeText;

	public Text m_healthText;

	public Image m_buildingTitleBanner;

	public Shader m_grayscaleShader;

	public GameObject m_gotLootArea;

	public Text m_gotLootLabel;

	public Text m_gotLootItemName;

	public MissionRewardDisplay m_rewardDisplay;

	public CanvasGroup m_contributeAreaCanvasGroup;

	public CanvasGroup m_lootAreaCanvasGroup;

	private int m_lootItemID;

	private int m_lootItemQuantity;

	private float m_lootDisplayTimeRemaining;

	private bool m_lootDisplayPending;

	private float m_delayBeforeShowingLoot;

	private int m_contributionID;

	private int m_questID;

	private static int s_lastContributionID;

	public LegionfallBuildingPanel()
	{
	}

	public void DoProgressBarGlow()
	{
		Main.instance.m_UISound.Play_ContributeSuccess();
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "GlowIn", "delay", 0f, "from", 0f, "to", 1f, "easeType", iTween.EaseType.easeInOutQuad, "time", this.m_glowDuration / 2f, "onupdate", "ProgressBarGlowUpdate", "oncomplete", "ProgressBarGlowInComplete" }));
	}

	private void FadeLootInCallback(float val)
	{
		this.m_lootAreaCanvasGroup.alpha = val;
		this.m_contributeAreaCanvasGroup.alpha = 1f - val;
	}

	private void FadeLootInCompleteCallback()
	{
		this.m_lootAreaCanvasGroup.alpha = 1f;
		this.m_contributeAreaCanvasGroup.alpha = 0f;
	}

	private void FadeLootOutCallback(float val)
	{
		this.m_lootAreaCanvasGroup.alpha = 1f - val;
		this.m_contributeAreaCanvasGroup.alpha = val;
	}

	private void FadeLootOutCompleteCallback()
	{
		this.m_lootAreaCanvasGroup.alpha = 0f;
		this.m_contributeAreaCanvasGroup.alpha = 1f;
		this.m_gotLootArea.SetActive(false);
		this.m_lootItemQuantity = 0;
	}

	private void HandleContributionInfoChanged()
	{
		this.InitPanel(this.m_contributionID, this.m_questID);
		if (LegionfallBuildingPanel.s_lastContributionID == this.m_contributionID)
		{
			this.DoProgressBarGlow();
		}
	}

	private void HandleGotItemFromQuestCompletion(int itemID, int itemQuantity, int questID)
	{
		if (this.m_questID != questID)
		{
			return;
		}
		this.m_lootItemID = itemID;
		this.m_lootItemQuantity += itemQuantity;
	}

	private void HandleMakeContributionRequestInitiated()
	{
		this.m_contributeButton.interactable = false;
		this.m_contributeButtonLabel.color = new Color(0.6f, 0.6f, 0.6f, 1f);
	}

	public void InitPanel(int contributionID, int questID)
	{
		if (this.m_grayscaleShader != null)
		{
			Material material = new Material(this.m_grayscaleShader);
			this.m_buildingTitleBanner.material = material;
		}
		this.m_questID = questID;
		this.m_contributionID = contributionID;
		this.m_buildingName.font = GeneralHelpers.LoadFancyFont();
		this.m_buildingState.font = GeneralHelpers.LoadStandardFont();
		this.m_buildingDescription.font = GeneralHelpers.LoadStandardFont();
		this.m_costLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_cost.font = GeneralHelpers.LoadStandardFont();
		this.m_contributeButtonLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_cantContributeText.font = GeneralHelpers.LoadStandardFont();
		this.m_cantContributeText.text = StaticDB.GetString("CANT_CONTRIBUTE", "You can only contribute when the building is under construction.");
		this.m_healthText.font = GeneralHelpers.LoadStandardFont();
		this.m_gotLootLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_gotLootLabel.text = StaticDB.GetString("YOU_RECEIVED_ITEM", "You received item: (PH)");
		this.m_gotLootItemName.font = GeneralHelpers.LoadStandardFont();
		this.m_gotLootArea.SetActive(false);
		if (!LegionfallData.legionfallDictionary.ContainsKey(contributionID))
		{
			return;
		}
		MobileContribution item = ((LegionfallData.MobileContributionData)LegionfallData.legionfallDictionary[contributionID]).contribution;
		int uitextureAtlasMemberIDUnderConstruction = 0;
		this.m_buildingStateImageFrame_building.gameObject.SetActive(false);
		this.m_buildingStateImageFrame_active.gameObject.SetActive(false);
		this.m_buildingStateImageFrame_underAttack.gameObject.SetActive(false);
		this.m_buildingStateImageFrame_destroyed.gameObject.SetActive(false);
		this.m_contributeArea.SetActive(false);
		this.m_cantContributeArea.SetActive(true);
		this.m_healthText.gameObject.SetActive(true);
		this.m_buildingTitleBanner.material.SetFloat("_GrayscaleAmount", (item.State != 4 ? 0f : 1f));
		this.m_buildingTitleBanner.color = (item.State != 4 ? Color.white : Color.gray);
		this.m_progressFillGlow.gameObject.SetActive(false);
		switch (item.State)
		{
			case 1:
			{
				this.m_progressFillGlow.gameObject.SetActive(true);
				uitextureAtlasMemberIDUnderConstruction = item.UitextureAtlasMemberIDUnderConstruction;
				this.m_buildingState.text = StaticDB.GetString("UNDER_CONSTRUCTION", "Under Construction (PH)");
				this.m_buildingState.color = new Color(1f, 0.8235f, 0f, 1f);
				this.m_buildingStateImageFrame_building.gameObject.SetActive(true);
				this.m_contributeArea.SetActive(true);
				this.m_cantContributeArea.SetActive(false);
				this.m_progressFillBar.sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_BarFilling_UnderConstruction");
				break;
			}
			case 2:
			{
				uitextureAtlasMemberIDUnderConstruction = item.UitextureAtlasMemberIDActive;
				this.m_buildingState.text = StaticDB.GetString("BUILDING_ACTIVE", "Building Active (PH)");
				this.m_buildingState.color = new Color(0f, 1f, 0f, 1f);
				this.m_buildingStateImageFrame_active.gameObject.SetActive(true);
				this.m_progressFillBar.sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_BarFilling_Active");
				break;
			}
			case 3:
			{
				uitextureAtlasMemberIDUnderConstruction = item.UitextureAtlasMemberIDUnderAttack;
				this.m_buildingState.text = StaticDB.GetString("UNDER_ATTACK", "Under Attack (PH)");
				this.m_buildingState.color = new Color(1f, 0f, 0f, 1f);
				this.m_buildingStateImageFrame_underAttack.gameObject.SetActive(true);
				this.m_progressFillBar.sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_BarFilling_UnderAttack");
				break;
			}
			case 4:
			{
				uitextureAtlasMemberIDUnderConstruction = item.UitextureAtlasMemberIDDestroyed;
				this.m_buildingState.text = StaticDB.GetString("DESTROYED", "Destroyed (PH)");
				this.m_buildingState.color = new Color(0.6f, 0.6f, 0.6f, 1f);
				this.m_buildingStateImageFrame_destroyed.gameObject.SetActive(true);
				this.m_progressFillBar.sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_BarFilling_UnderConstruction");
				break;
			}
			default:
			{
				return;
			}
		}
		Sprite sprite = null;
		switch (uitextureAtlasMemberIDUnderConstruction)
		{
			case 6318:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_CommandCenter_Active-v2");
				break;
			}
			case 6319:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_CommandCenter_Destroyed-v2");
				break;
			}
			case 6320:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_CommandCenter_UnderAttack-v2");
				break;
			}
			case 6321:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_CommandCenter_UnderConstruction-v2");
				break;
			}
			case 6322:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_MageTower_Active-v2");
				break;
			}
			case 6323:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_MageTower_Destroyed-v2");
				break;
			}
			case 6324:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_MageTower_UnderAttack-v2");
				break;
			}
			case 6325:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_MageTower_UnderConstruction-v2");
				break;
			}
			case 6326:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_NetherDisruptor_Active-v2");
				break;
			}
			case 6327:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_NetherDisruptor_Destroyed-v2");
				break;
			}
			case 6328:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_NetherDisruptor_UnderAttack-v2");
				break;
			}
			case 6329:
			{
				sprite = Resources.Load<Sprite>("LegionfallDialog/LegionfallCompanion_NetherDisruptor_UnderConstruction-v2");
				break;
			}
		}
		if (sprite != null)
		{
			this.m_buildingStateImage.sprite = sprite;
		}
		this.m_buildingName.text = item.Name;
		this.m_buildingDescription.text = GeneralHelpers.LimitZhLineLength(item.Description, 16);
		this.m_progressFillBar.fillAmount = item.UnitCompletion;
		this.m_progressFillGlow.anchorMin = new Vector2(this.m_progressFillBar.fillAmount, 0.5f);
		this.m_progressFillGlow.anchorMax = new Vector2(this.m_progressFillBar.fillAmount, 0.5f);
		this.m_progressFillGlow.anchoredPosition = Vector2.zero;
		if (item.State != 3)
		{
			Text mHealthText = this.m_healthText;
			int mProgressFillBar = (int)(this.m_progressFillBar.fillAmount * 100f);
			mHealthText.text = string.Concat(mProgressFillBar.ToString(), "%");
		}
		else
		{
			this.m_healthText.text = string.Concat(StaticDB.GetString("TIME_LEFT", "Time Left (PH):"), " 123 Hours");
		}
		this.m_costLabel.text = StaticDB.GetString("COST", null);
		if (LegionfallData.WarResources() < item.ContributionCurrencyCost)
		{
			Text mCost = this.m_cost;
			object[] str = new object[] { "<color=#ff0000ff>", null, null, null };
			int num = LegionfallData.WarResources();
			str[1] = num.ToString("N0");
			str[2] = "</color>/";
			str[3] = item.ContributionCurrencyCost;
			mCost.text = string.Concat(str);
			this.m_contributeButton.interactable = false;
			this.m_contributeButtonLabel.color = new Color(0.6f, 0.6f, 0.6f, 1f);
		}
		else
		{
			Text text = this.m_cost;
			int num1 = LegionfallData.WarResources();
			text.text = string.Concat(num1.ToString("N0"), "/", item.ContributionCurrencyCost);
			this.m_contributeButton.interactable = true;
			this.m_contributeButtonLabel.color = new Color(1f, 0.859f, 0f, 1f);
		}
		this.m_currencyIcon.sprite = GeneralHelpers.LoadCurrencyIcon(item.ContributionCurrencyType);
		this.m_contributeButtonLabel.text = StaticDB.GetString("CONTRIBUTE", "Contribute (PH)");
		SpellDisplay[] componentsInChildren = this.m_buffArea.GetComponentsInChildren<SpellDisplay>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		int[] spell = item.Spell;
		for (int j = 0; j < (int)spell.Length; j++)
		{
			int num2 = spell[j];
			SpellDisplay spellDisplay = UnityEngine.Object.Instantiate<SpellDisplay>(this.m_legionfallBuffSpellDisplayPrefab);
			spellDisplay.transform.SetParent(this.m_buffArea, false);
			spellDisplay.SetSpell(num2);
			spellDisplay.SetLocked((item.State == 2 ? false : item.State != 3));
		}
		iTween.Stop(base.gameObject);
		this.m_progressBarGlow.color = new Color(1f, 1f, 1f, 0f);
		this.m_progressFillGlowGlow.color = new Color(1f, 1f, 1f, 0f);
	}

	public void MakeContribution()
	{
		if (!LegionfallData.legionfallDictionary.ContainsKey(this.m_contributionID))
		{
			return;
		}
		LegionfallData.MobileContributionData item = (LegionfallData.MobileContributionData)LegionfallData.legionfallDictionary[this.m_contributionID];
		MobileContribution mobileContribution = item.contribution;
		if (mobileContribution.State == 1)
		{
			LegionfallBuildingPanel.s_lastContributionID = this.m_contributionID;
			Debug.Log(string.Concat("Starting to contribute to ID ", mobileContribution.ContributionID));
			if (Main.instance.MakeContributionRequestInitiatedAction != null)
			{
				Main.instance.MakeContributionRequestInitiatedAction();
			}
			MobilePlayerMakeContribution mobilePlayerMakeContribution = new MobilePlayerMakeContribution()
			{
				ContributionID = mobileContribution.ContributionID
			};
			Login.instance.SendToMobileServer(mobilePlayerMakeContribution);
			this.m_lootDisplayPending = true;
			this.m_delayBeforeShowingLoot = 2f;
			this.m_lootDisplayTimeRemaining = 3f;
			Main.instance.m_UISound.Play_ButtonRedClick();
		}
	}

	private void OnDisable()
	{
		Main.instance.MakeContributionRequestInitiatedAction -= new Action(this.HandleMakeContributionRequestInitiated);
		Main.instance.ContributionInfoChangedAction -= new Action(this.HandleContributionInfoChanged);
		Main.instance.GotItemFromQuestCompletionAction -= new Action<int, int, int>(this.HandleGotItemFromQuestCompletion);
	}

	private void OnEnable()
	{
		Main.instance.MakeContributionRequestInitiatedAction += new Action(this.HandleMakeContributionRequestInitiated);
		Main.instance.ContributionInfoChangedAction += new Action(this.HandleContributionInfoChanged);
		Main.instance.GotItemFromQuestCompletionAction += new Action<int, int, int>(this.HandleGotItemFromQuestCompletion);
	}

	private void ProgressBarGlowInComplete()
	{
		this.m_progressBarGlow.color = new Color(1f, 1f, 1f, 1f);
		this.m_progressFillGlowGlow.color = new Color(1f, 1f, 1f, 1f);
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "GlowOut", "delay", 0f, "from", 1f, "to", 0f, "easeType", iTween.EaseType.easeInOutQuad, "time", this.m_glowDuration / 2f, "onupdate", "ProgressBarGlowUpdate", "oncomplete", "ProgressBarGlowComplete" }));
	}

	private void ProgressBarGlowOutComplete()
	{
		this.m_progressBarGlow.color = new Color(1f, 1f, 1f, 0f);
		this.m_progressFillGlowGlow.color = new Color(1f, 1f, 1f, 0f);
	}

	private void ProgressBarGlowUpdate(float val)
	{
		this.m_progressBarGlow.color = new Color(1f, 1f, 1f, val);
		this.m_progressFillGlowGlow.color = new Color(1f, 1f, 1f, val);
	}

	private void Update()
	{
		if (this.m_lootDisplayPending && this.m_contributeButton.interactable)
		{
			this.m_delayBeforeShowingLoot -= Time.deltaTime;
			if (this.m_delayBeforeShowingLoot > 0f)
			{
				return;
			}
		}
		if (this.m_delayBeforeShowingLoot <= 0f && this.m_lootDisplayTimeRemaining > 0f && this.m_lootItemQuantity > 0)
		{
			if (!this.m_gotLootArea.activeSelf)
			{
				Main.instance.m_UISound.Play_GetItem();
				this.m_gotLootArea.SetActive(true);
				this.m_lootAreaCanvasGroup.alpha = 0f;
				iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "Fade Loot In", "from", 0f, "to", 1f, "easeType", "easeOutCubic", "time", 0.5f, "onupdate", "FadeLootInCallback", "oncomplete", "FadeLootInCompleteCallback" }));
			}
			ItemRec record = StaticDB.itemDB.GetRecord(this.m_lootItemID);
			if (record == null)
			{
				this.m_gotLootItemName.text = string.Concat("???", (this.m_lootItemQuantity != 1 ? string.Concat(" (", this.m_lootItemQuantity, "x)") : string.Empty));
			}
			else
			{
				this.m_gotLootItemName.text = string.Concat(record.Display, (this.m_lootItemQuantity != 1 ? string.Concat(" (", this.m_lootItemQuantity, "x)") : string.Empty));
			}
			this.m_rewardDisplay.InitReward(MissionRewardDisplay.RewardType.item, this.m_lootItemID, 1, 0, 0);
			this.m_lootDisplayTimeRemaining -= Time.deltaTime;
		}
		else if (this.m_lootDisplayTimeRemaining <= 0f && this.m_lootDisplayPending)
		{
			if (this.m_gotLootArea.activeSelf)
			{
				iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "Fade Loot Out", "from", 0f, "to", 1f, "easeType", "easeOutCubic", "time", 0.5f, "onupdate", "FadeLootOutCallback", "oncomplete", "FadeLootOutCompleteCallback" }));
			}
			this.m_lootDisplayPending = false;
		}
		if (LegionfallData.legionfallDictionary.ContainsKey(this.m_contributionID))
		{
			LegionfallData.MobileContributionData item = (LegionfallData.MobileContributionData)LegionfallData.legionfallDictionary[this.m_contributionID];
			MobileContribution mobileContribution = item.contribution;
			if (mobileContribution.State == 3)
			{
				if (item.underAttackExpireTime == (long)0)
				{
					item.underAttackExpireTime = GarrisonStatus.CurrentTime() + (long)item.contribution.CurrentValue;
				}
				long num = item.underAttackExpireTime - GarrisonStatus.CurrentTime();
				num = (num <= (long)0 ? (long)0 : num);
				Duration duration = new Duration((int)num, false);
				this.m_healthText.text = string.Concat(StaticDB.GetString("TIME_LEFT", null), " ", duration.DurationString);
				this.m_progressFillBar.fillAmount = mobileContribution.CurrentValue / mobileContribution.UpperValue;
			}
		}
	}
}