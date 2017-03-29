using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class FollowerExperienceDisplay : MonoBehaviour
{
	[Header("Portrait")]
	public Image m_portraitBG;

	public Image m_followerPortrait;

	public Image m_qualityBorder;

	public Image m_qualityBorder_TitleQuality;

	public Image m_levelBorder;

	public Image m_levelBorder_TitleQuality;

	public Text m_followerNameText;

	public Text m_iLevelText;

	public Image m_classIcon;

	public Text m_classText;

	[Header("Troop Specific")]
	public GameObject m_troopHeartContainerEmpty;

	public GameObject m_troopHeartContainerFull;

	public GameObject m_troopHeartPrefab;

	public GameObject m_troopEmptyHeartPrefab;

	public GameObject m_expiredPortraitX;

	[Header("XP Bar")]
	public GameObject m_progressBarObj;

	public FancyNumberDisplay m_fancyNumberDisplay;

	public Image m_progressBarFillImage;

	public Text m_xpAmountText;

	public Text m_toNextLevelOrUpgradeText;

	private int m_followerID;

	private bool m_showedLevelUpEffect;

	private uint m_currentCap;

	private bool m_newCapIsQuality;

	private uint m_newCap;

	private bool m_newFollowerIsMaxLevelAndMaxQuality;

	public FollowerExperienceDisplay()
	{
	}

	public int GetFollowerID()
	{
		return this.m_followerID;
	}

	private void OnDisable()
	{
		this.m_fancyNumberDisplay.TimerUpdateAction -= new Action<int>(this.SetFillValue);
	}

	private void OnEnable()
	{
		this.m_fancyNumberDisplay.TimerUpdateAction += new Action<int>(this.SetFillValue);
	}

	private void SetFillValue(int newXPRemainingUntilNextLevel)
	{
		float single = 0f;
		if (newXPRemainingUntilNextLevel == 0 && this.m_currentCap != this.m_newCap)
		{
			if (!this.m_showedLevelUpEffect)
			{
				Main.instance.m_UISound.Play_ChampionLevelUp();
				UiAnimMgr.instance.PlayAnim("FlameGlowPulse", this.m_followerPortrait.transform, Vector3.zero, 2f, 0f);
				UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", this.m_followerPortrait.transform, Vector3.zero, 2f, 0f);
				this.m_showedLevelUpEffect = true;
			}
			JamGarrisonFollower item = PersistentFollowerData.followerDictionary[this.m_followerID];
			this.SetFollowerAppearance(item, this.m_newCapIsQuality, this.m_newFollowerIsMaxLevelAndMaxQuality, false, 0f);
			this.m_currentCap = this.m_newCap;
			this.m_fancyNumberDisplay.SetValue((int)this.m_newCap, true, 0f);
			this.m_fancyNumberDisplay.SetValue((int)(this.m_newCap - item.Xp), 0f);
		}
		this.m_xpAmountText.text = string.Concat(new object[] { string.Empty, (long)((ulong)this.m_currentCap - (long)newXPRemainingUntilNextLevel), "\\", this.m_currentCap });
		single = Mathf.Clamp01((float)((ulong)this.m_currentCap - (long)newXPRemainingUntilNextLevel) / (float)((float)this.m_currentCap));
		this.m_progressBarFillImage.fillAmount = single;
	}

	public void SetFollower(JamGarrisonFollower oldFollower, JamGarrisonFollower newFollower, float initialEffectDelay)
	{
		this.m_followerID = oldFollower.GarrFollowerID;
		if ((oldFollower.Flags & 8) != 0)
		{
			JamGarrisonFollower jamGarrisonFollower = newFollower ?? new JamGarrisonFollower()
			{
				GarrFollowerID = oldFollower.GarrFollowerID,
				Quality = oldFollower.Quality,
				Durability = 0
			};
			this.SetFollowerAppearance(jamGarrisonFollower, false, false, true, initialEffectDelay);
			return;
		}
		this.m_showedLevelUpEffect = false;
		bool flag = false;
		bool flag1 = false;
		GeneralHelpers.GetXpCapInfo(oldFollower.FollowerLevel, oldFollower.Quality, out this.m_currentCap, out flag1, out flag);
		this.SetFollowerAppearance(oldFollower, flag1, flag, false, initialEffectDelay);
		GeneralHelpers.GetXpCapInfo(newFollower.FollowerLevel, newFollower.Quality, out this.m_newCap, out this.m_newCapIsQuality, out this.m_newFollowerIsMaxLevelAndMaxQuality);
		this.m_fancyNumberDisplay.SetNumberLabel(StaticDB.GetString("XP2", null));
		this.m_fancyNumberDisplay.SetValue((int)(this.m_currentCap - oldFollower.Xp), true, 0f);
		if (oldFollower.FollowerLevel != newFollower.FollowerLevel || oldFollower.Quality != newFollower.Quality)
		{
			this.m_fancyNumberDisplay.SetValue(0, initialEffectDelay);
		}
		else
		{
			this.m_fancyNumberDisplay.SetValue((int)(this.m_currentCap - newFollower.Xp), initialEffectDelay);
		}
	}

	private void SetFollowerAppearance(JamGarrisonFollower follower, bool nextCapIsForQuality, bool isMaxLevelAndMaxQuality, bool isTroop, float initialEntranceDelay)
	{
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
		this.m_troopHeartContainerEmpty.SetActive(isTroop);
		this.m_troopHeartContainerFull.SetActive(isTroop);
		this.m_expiredPortraitX.SetActive(false);
		if (isTroop)
		{
			this.m_levelBorder.gameObject.SetActive(false);
			this.m_progressBarObj.SetActive(false);
			this.m_portraitBG.gameObject.SetActive(false);
			this.m_troopHeartContainerEmpty.SetActive(true);
			this.m_troopHeartContainerFull.SetActive(true);
			Transform[] componentsInChildren = this.m_troopHeartContainerEmpty.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Transform transforms = componentsInChildren[i];
				if (transforms != this.m_troopHeartContainerEmpty.transform)
				{
					UnityEngine.Object.DestroyImmediate(transforms.gameObject);
				}
			}
			Transform[] transformArrays = this.m_troopHeartContainerFull.GetComponentsInChildren<Transform>(true);
			for (int j = 0; j < (int)transformArrays.Length; j++)
			{
				Transform transforms1 = transformArrays[j];
				if (transforms1 != this.m_troopHeartContainerFull.transform)
				{
					UnityEngine.Object.DestroyImmediate(transforms1.gameObject);
				}
			}
			float single = 0.15f;
			JamGarrisonFollower item = PersistentFollowerData.preMissionFollowerDictionary[follower.GarrFollowerID];
			for (int k = 0; k < item.Durability; k++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_troopHeartPrefab);
				gameObject.transform.SetParent(this.m_troopHeartContainerFull.transform, false);
				if (k >= follower.Durability)
				{
					float single1 = initialEntranceDelay + (float)(item.Durability - (k - follower.Durability)) * single;
					float single2 = 2f;
					iTween.ValueTo(gameObject, iTween.Hash(new object[] { "name", "fade", "from", 0f, "to", 1f, "time", single2, "easetype", iTween.EaseType.easeOutCubic, "delay", single1, "onupdatetarget", gameObject, "onupdate", "SetHeartEffectProgress", "oncomplete", "FinishHeartEffect" }));
				}
			}
			for (int l = 0; l < record.Vitality; l++)
			{
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_troopEmptyHeartPrefab);
				gameObject1.transform.SetParent(this.m_troopHeartContainerEmpty.transform, false);
			}
			if (follower.Durability <= 0)
			{
				DelayedUIAnim delayedUIAnim = base.gameObject.AddComponent<DelayedUIAnim>();
				float single3 = initialEntranceDelay + (float)(item.Durability - follower.Durability) * single + 1f;
				delayedUIAnim.Init(single3, "RedFailX", "SFX/UI_Mission_Fail_Red_X", this.m_followerPortrait.transform, 1.5f, 0.3f);
				DelayedObjectEnable delayedObjectEnable = base.gameObject.AddComponent<DelayedObjectEnable>();
				delayedObjectEnable.Init(single3 + 0.25f, this.m_expiredPortraitX);
			}
		}
		Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceIconFileDataID : record.HordeIconFileDataID));
		if (sprite != null)
		{
			this.m_followerPortrait.sprite = sprite;
		}
		if (!isTroop)
		{
			if (follower.Quality != 6)
			{
				this.m_qualityBorder_TitleQuality.gameObject.SetActive(false);
				this.m_levelBorder_TitleQuality.gameObject.SetActive(false);
				this.m_qualityBorder.gameObject.SetActive(true);
				this.m_levelBorder.gameObject.SetActive(true);
			}
			else
			{
				this.m_qualityBorder_TitleQuality.gameObject.SetActive(true);
				this.m_levelBorder_TitleQuality.gameObject.SetActive(true);
				this.m_qualityBorder.gameObject.SetActive(false);
				this.m_levelBorder.gameObject.SetActive(false);
			}
			Color qualityColor = GeneralHelpers.GetQualityColor(follower.Quality);
			this.m_qualityBorder.color = qualityColor;
			this.m_levelBorder.color = qualityColor;
			this.m_followerNameText.color = qualityColor;
		}
		else
		{
			this.m_qualityBorder_TitleQuality.gameObject.SetActive(false);
			this.m_levelBorder_TitleQuality.gameObject.SetActive(false);
			this.m_qualityBorder.gameObject.SetActive(false);
			this.m_levelBorder.gameObject.SetActive(false);
			this.m_followerNameText.color = Color.white;
			this.m_iLevelText.gameObject.SetActive(false);
		}
		CreatureRec creatureRec = StaticDB.creatureDB.GetRecord((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceCreatureID : record.HordeCreatureID));
		if (follower.Quality == 6 && record.TitleName != null && record.TitleName.Length > 0)
		{
			this.m_followerNameText.text = record.TitleName;
		}
		else if (record != null)
		{
			this.m_followerNameText.text = creatureRec.Name;
		}
		if (follower.FollowerLevel >= 110)
		{
			Text mILevelText = this.m_iLevelText;
			string str = StaticDB.GetString("ILVL", null);
			int itemLevelArmor = (follower.ItemLevelArmor + follower.ItemLevelWeapon) / 2;
			mILevelText.text = string.Concat(str, " ", itemLevelArmor.ToString());
		}
		else
		{
			Text text = this.m_iLevelText;
			string str1 = StaticDB.GetString("LEVEL", null);
			int followerLevel = follower.FollowerLevel;
			text.text = GeneralHelpers.TextOrderString(str1, followerLevel.ToString());
		}
		GarrClassSpecRec garrClassSpecRec = StaticDB.garrClassSpecDB.GetRecord((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? (int)record.AllianceGarrClassSpecID : (int)record.HordeGarrClassSpecID));
		this.m_classText.text = garrClassSpecRec.ClassSpec;
		Sprite atlasSprite = TextureAtlas.instance.GetAtlasSprite((int)garrClassSpecRec.UiTextureAtlasMemberID);
		if (atlasSprite != null)
		{
			this.m_classIcon.sprite = atlasSprite;
		}
		if (!isTroop)
		{
			if (isMaxLevelAndMaxQuality)
			{
				this.m_progressBarObj.SetActive(false);
				this.m_toNextLevelOrUpgradeText.text = string.Empty;
			}
			else if (!nextCapIsForQuality)
			{
				this.m_progressBarObj.SetActive(true);
				this.m_toNextLevelOrUpgradeText.text = StaticDB.GetString("TO_NEXT_LEVEL", string.Empty);
			}
			else
			{
				this.m_progressBarObj.SetActive(true);
				this.m_toNextLevelOrUpgradeText.text = StaticDB.GetString("TO_NEXT_UPGRADE", string.Empty);
			}
		}
	}

	private void Start()
	{
		this.m_followerNameText.font = GeneralHelpers.LoadStandardFont();
		this.m_iLevelText.font = GeneralHelpers.LoadStandardFont();
		this.m_classText.font = GeneralHelpers.LoadStandardFont();
		this.m_xpAmountText.font = GeneralHelpers.LoadStandardFont();
		this.m_toNextLevelOrUpgradeText.font = GeneralHelpers.LoadStandardFont();
	}
}