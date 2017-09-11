using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WowJamMessages;
using WowJamMessages.MobileClientJSON;
using WowStatConstants;
using WowStaticData;

public class GeneralHelpers : MonoBehaviour
{
	public static string s_defaultColor;

	public static string s_normalColor;

	public static string s_friendlyColor;

	private static Font s_fancyFont;

	private static Font s_standardFont;

	static GeneralHelpers()
	{
		GeneralHelpers.s_defaultColor = "ffd200ff";
		GeneralHelpers.s_normalColor = "ffffffff";
		GeneralHelpers.s_friendlyColor = "00ff00ff";
	}

	public GeneralHelpers()
	{
	}

	public static int ApplyArtifactXPMultiplier(int inputAmount, float multiplier)
	{
		int num;
		if (multiplier <= 1f)
		{
			return inputAmount;
		}
		float single = (float)inputAmount * multiplier;
		if (single < 50f)
		{
			num = 1;
		}
		else if (single >= 1000f)
		{
			num = (single >= 5000f ? 50 : 25);
		}
		else
		{
			num = 5;
		}
		return num * (int)Math.Round((double)(single / (float)num));
	}

	public static int CurrentUnixTime()
	{
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan timeSpan = utcNow.Subtract(new DateTime(1970, 1, 1));
		return (int)timeSpan.TotalSeconds;
	}

	public static string GetBonusStatString(BonusStatIndex statIndex)
	{
		BonusStatIndex bonusStatIndex = statIndex;
		switch (bonusStatIndex)
		{
			case BonusStatIndex.MANA:
			{
				return StaticDB.GetString("ITEM_MOD_MANA_SHORT", null);
			}
			case BonusStatIndex.HEALTH:
			{
				return StaticDB.GetString("ITEM_MOD_HEALTH_SHORT", null);
			}
			case BonusStatIndex.AGILITY:
			{
				return StaticDB.GetString("ITEM_MOD_AGILITY_SHORT", null);
			}
			case BonusStatIndex.STRENGTH:
			{
				return StaticDB.GetString("ITEM_MOD_STRENGTH_SHORT", null);
			}
			case BonusStatIndex.INTELLECT:
			{
				return StaticDB.GetString("ITEM_MOD_INTELLECT_SHORT", null);
			}
			case BonusStatIndex.STAMINA:
			{
				return StaticDB.GetString("ITEM_MOD_STAMINA_SHORT", null);
			}
			case BonusStatIndex.DEFENSE_SKILL_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_DEFENSE_SKILL_RATING_SHORT", null);
			}
			case BonusStatIndex.DODGE_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_DODGE_RATING_SHORT", null);
			}
			case BonusStatIndex.PARRY_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_PARRY_RATING_SHORT", null);
			}
			case BonusStatIndex.BLOCK_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_BLOCK_RATING_SHORT", null);
			}
			case BonusStatIndex.HIT_MELEE_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_HIT_MELEE_RATING_SHORT", null);
			}
			case BonusStatIndex.HIT_RANGED_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_HIT_RANGED_RATING_SHORT", null);
			}
			case BonusStatIndex.HIT_SPELL_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_HIT_SPELL_RATING_SHORT", null);
			}
			case BonusStatIndex.CRIT_MELEE_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_CRIT_MELEE_RATING_SHORT", null);
			}
			case BonusStatIndex.CRIT_RANGED_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_CRIT_RANGED_RATING_SHORT", null);
			}
			case BonusStatIndex.CRIT_SPELL_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_CRIT_SPELL_RATING_SHORT", null);
			}
			case BonusStatIndex.HIT_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_HIT_RATING_SHORT", null);
			}
			case BonusStatIndex.CRIT_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_CRIT_RATING_SHORT", null);
			}
			case BonusStatIndex.RESILIENCE_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_RESILIENCE_RATING_SHORT", null);
			}
			case BonusStatIndex.HASTE_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_HASTE_RATING_SHORT", null);
			}
			case BonusStatIndex.EXPERTISE_RATING:
			{
				return StaticDB.GetString("ITEM_MOD_EXPERTISE_RATING_SHORT", null);
			}
			case BonusStatIndex.ATTACK_POWER:
			{
				return StaticDB.GetString("ITEM_MOD_ATTACK_POWER_SHORT", null);
			}
			case BonusStatIndex.VERSATILITY:
			{
				return StaticDB.GetString("ITEM_MOD_VERSATILITY", null);
			}
			default:
			{
				if (bonusStatIndex == BonusStatIndex.MASTERY_RATING)
				{
					break;
				}
				else
				{
					return statIndex.ToString();
				}
			}
		}
		return StaticDB.GetString("ITEM_MOD_MASTERY_RATING", null);
	}

	public static int[] GetBuffsForCurrentMission(int garrFollowerID, int garrMissionID, GameObject missionFollowerSlotGroup)
	{
		List<int> nums = new List<int>();
		if (!PersistentFollowerData.followerDictionary.ContainsKey(garrFollowerID))
		{
			return nums.ToArray();
		}
		JamGarrisonFollower jamGarrisonFollower = PersistentFollowerData.followerDictionary[garrFollowerID];
		for (int i1 = 0; i1 < (int)jamGarrisonFollower.AbilityID.Length; i1++)
		{
			GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(jamGarrisonFollower.AbilityID[i1]);
			if (garrAbilityRec != null)
			{
				StaticDB.garrAbilityEffectDB.EnumRecordsByParentID(garrAbilityRec.ID, (GarrAbilityEffectRec garrAbilityEffectRec) => {
					uint abilityAction = garrAbilityEffectRec.AbilityAction;
					switch (abilityAction)
					{
						case 0:
						{
							return true;
						}
						case 1:
						{
							MissionFollowerSlot[] componentsInChildren = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
							int num = 0;
							MissionFollowerSlot[] missionFollowerSlotArray = componentsInChildren;
							for (int i = 0; i < (int)missionFollowerSlotArray.Length; i++)
							{
								if (missionFollowerSlotArray[i].GetCurrentGarrFollowerID() > 0)
								{
									num++;
								}
							}
							if (num != 1)
							{
								return true;
							}
							break;
						}
						case 5:
						{
							MissionFollowerSlot[] componentsInChildren1 = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
							bool flag = false;
							MissionFollowerSlot[] missionFollowerSlotArray1 = componentsInChildren1;
							for (int j = 0; j < (int)missionFollowerSlotArray1.Length; j++)
							{
								int currentGarrFollowerID = missionFollowerSlotArray1[j].GetCurrentGarrFollowerID();
								if (currentGarrFollowerID > 0 && currentGarrFollowerID != jamGarrisonFollower.GarrFollowerID)
								{
									GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(currentGarrFollowerID);
									if (record != null)
									{
										if ((GarrisonStatus.Faction() != PVP_FACTION.ALLIANCE ? record.HordeGarrFollRaceID : record.AllianceGarrFollRaceID) == garrAbilityEffectRec.ActionRace)
										{
											flag = true;
											break;
										}
									}
								}
							}
							if (!flag)
							{
								return true;
							}
							break;
						}
						case 6:
						{
							GarrMissionRec garrMissionRec = StaticDB.garrMissionDB.GetRecord(garrMissionID);
							if (garrMissionRec != null && (float)garrMissionRec.MissionDuration < (float)((float)garrAbilityEffectRec.ActionHours) * 3600f)
							{
								return true;
							}
							break;
						}
						case 7:
						{
							GarrMissionRec record1 = StaticDB.garrMissionDB.GetRecord(garrMissionID);
							if (record1 != null && (float)record1.MissionDuration > (float)((float)garrAbilityEffectRec.ActionHours) * 3600f)
							{
								return true;
							}
							break;
						}
						case 9:
						{
							GarrMissionRec garrMissionRec1 = StaticDB.garrMissionDB.GetRecord(garrMissionID);
							if (garrMissionRec1 != null && (float)((float)garrMissionRec1.TravelDuration) < (float)((float)garrAbilityEffectRec.ActionHours) * 3600f)
							{
								return true;
							}
							break;
						}
						case 10:
						{
							GarrMissionRec record2 = StaticDB.garrMissionDB.GetRecord(garrMissionID);
							if (record2 != null && (float)((float)record2.TravelDuration) > (float)((float)garrAbilityEffectRec.ActionHours) * 3600f)
							{
								return true;
							}
							break;
						}
						case 12:
						{
							return true;
						}
						default:
						{
							if (abilityAction == 22)
							{
								MissionFollowerSlot[] componentsInChildren2 = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
								bool flag1 = false;
								MissionFollowerSlot[] missionFollowerSlotArray2 = componentsInChildren2;
								for (int k = 0; k < (int)missionFollowerSlotArray2.Length; k++)
								{
									int currentGarrFollowerID1 = missionFollowerSlotArray2[k].GetCurrentGarrFollowerID();
									if (currentGarrFollowerID1 > 0 && currentGarrFollowerID1 != jamGarrisonFollower.GarrFollowerID)
									{
										GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord(currentGarrFollowerID1);
										if (garrFollowerRec != null)
										{
											if ((GarrisonStatus.Faction() != PVP_FACTION.ALLIANCE ? garrFollowerRec.HordeGarrClassSpecID : garrFollowerRec.AllianceGarrClassSpecID) == garrAbilityEffectRec.ActionRecordID)
											{
												flag1 = true;
												break;
											}
										}
									}
								}
								if (!flag1)
								{
									return true;
								}
								break;
							}
							else if (abilityAction == 23)
							{
								bool flag2 = false;
								if (PersistentMissionData.missionDictionary.ContainsKey(garrMissionID))
								{
									JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[garrMissionID];
									for (int l = 0; l < (int)item.Encounter.Length; l++)
									{
										int num1 = 0;
										while (num1 < (int)item.Encounter[l].MechanicID.Length)
										{
											GarrMechanicRec garrMechanicRec = StaticDB.garrMechanicDB.GetRecord(item.Encounter[l].MechanicID[num1]);
											if (garrMechanicRec == null || garrAbilityEffectRec.GarrMechanicTypeID != garrMechanicRec.GarrMechanicTypeID)
											{
												num1++;
											}
											else
											{
												flag2 = true;
												break;
											}
										}
									}
								}
								if (!flag2)
								{
									return true;
								}
								break;
							}
							else
							{
								if (abilityAction == 37)
								{
									return true;
								}
								break;
							}
						}
					}
					if (!nums.Contains(garrAbilityRec.ID))
					{
						nums.Add(garrAbilityRec.ID);
					}
					return true;
				});
			}
			else
			{
				Debug.Log(string.Concat(new object[] { "Invalid Ability ID ", jamGarrisonFollower.AbilityID[i1], " from follower ", jamGarrisonFollower.GarrFollowerID }));
			}
		}
		return nums.ToArray();
	}

	public static string GetColorFromInt(int color)
	{
		int num = color >> 16 & 255;
		int num1 = color >> 8 & 255;
		int num2 = color & 255;
		string str = string.Format("{0:X2}{1:X2}{2:X2}", num, num1, num2);
		return str;
	}

	public static FollowerStatus GetFollowerStatus(JamGarrisonFollower follower)
	{
		if ((follower.Flags & 4) != 0)
		{
			return FollowerStatus.inactive;
		}
		if ((follower.Flags & 2) != 0)
		{
			return FollowerStatus.fatigued;
		}
		if (follower.CurrentBuildingID != 0)
		{
			return FollowerStatus.inBuilding;
		}
		if (follower.CurrentMissionID != 0)
		{
			return FollowerStatus.onMission;
		}
		return FollowerStatus.available;
	}

	public static string GetInventoryTypeString(INVENTORY_TYPE invType)
	{
		switch (invType)
		{
			case INVENTORY_TYPE.HEAD:
			{
				return StaticDB.GetString("INVTYPE_HEAD", null);
			}
			case INVENTORY_TYPE.NECK:
			{
				return StaticDB.GetString("INVTYPE_NECK", null);
			}
			case INVENTORY_TYPE.SHOULDER:
			{
				return StaticDB.GetString("INVTYPE_SHOULDER", null);
			}
			case INVENTORY_TYPE.BODY:
			{
				return StaticDB.GetString("INVTYPE_BODY", null);
			}
			case INVENTORY_TYPE.CHEST:
			{
				return StaticDB.GetString("INVTYPE_CHEST", null);
			}
			case INVENTORY_TYPE.WAIST:
			{
				return StaticDB.GetString("INVTYPE_WAIST", null);
			}
			case INVENTORY_TYPE.LEGS:
			{
				return StaticDB.GetString("INVTYPE_LEGS", null);
			}
			case INVENTORY_TYPE.FEET:
			{
				return StaticDB.GetString("INVTYPE_FEET", null);
			}
			case INVENTORY_TYPE.WRIST:
			{
				return StaticDB.GetString("INVTYPE_WRIST", null);
			}
			case INVENTORY_TYPE.HAND:
			{
				return StaticDB.GetString("INVTYPE_HAND", null);
			}
			case INVENTORY_TYPE.FINGER:
			{
				return StaticDB.GetString("INVTYPE_FINGER", null);
			}
			case INVENTORY_TYPE.TRINKET:
			{
				return StaticDB.GetString("INVTYPE_TRINKET", null);
			}
			case INVENTORY_TYPE.WEAPON:
			{
				return StaticDB.GetString("INVTYPE_WEAPON", null);
			}
			case INVENTORY_TYPE.SHIELD:
			{
				return StaticDB.GetString("INVTYPE_SHIELD", null);
			}
			case INVENTORY_TYPE.RANGED:
			{
				return StaticDB.GetString("INVTYPE_RANGED", null);
			}
			case INVENTORY_TYPE.CLOAK:
			{
				return StaticDB.GetString("INVTYPE_CLOAK", null);
			}
			case INVENTORY_TYPE.TWO_H_WEAPON:
			{
				return StaticDB.GetString("INVTYPE_2HWEAPON", null);
			}
			case INVENTORY_TYPE.BAG:
			{
				return StaticDB.GetString("INVTYPE_BAG", null);
			}
			case INVENTORY_TYPE.TABARD:
			{
				return StaticDB.GetString("INVTYPE_TABARD", null);
			}
			case INVENTORY_TYPE.ROBE:
			{
				return StaticDB.GetString("INVTYPE_ROBE", null);
			}
			case INVENTORY_TYPE.WEAPONMAINHAND:
			{
				return StaticDB.GetString("INVTYPE_WEAPONMAINHAND", null);
			}
			case INVENTORY_TYPE.WEAPONOFFHAND:
			{
				return StaticDB.GetString("INVTYPE_WEAPONOFFHAND", null);
			}
			case INVENTORY_TYPE.HOLDABLE:
			{
				return StaticDB.GetString("INVTYPE_HOLDABLE", null);
			}
			case INVENTORY_TYPE.AMMO:
			{
				return StaticDB.GetString("INVTYPE_AMMO", null);
			}
			case INVENTORY_TYPE.THROWN:
			{
				return StaticDB.GetString("INVTYPE_THROWN", null);
			}
			case INVENTORY_TYPE.RANGEDRIGHT:
			{
				return StaticDB.GetString("INVTYPE_RANGEDRIGHT", null);
			}
			case INVENTORY_TYPE.QUIVER:
			{
				return StaticDB.GetString("INVTYPE_QUIVER", null);
			}
			case INVENTORY_TYPE.RELIC:
			{
				return StaticDB.GetString("INVTYPE_RELIC", null);
			}
		}
		return string.Empty;
	}

	public static string GetItemDescription(ItemRec itemRec)
	{
		string str;
		string empty = string.Empty;
		int spellID = 0;
		StaticDB.itemEffectDB.EnumRecordsByParentID(itemRec.ID, (ItemEffectRec effectRec) => {
			if (effectRec.SpellID == 0)
			{
				return true;
			}
			spellID = effectRec.SpellID;
			return false;
		});
		if (spellID > 0)
		{
			SpellTooltipRec record = StaticDB.spellTooltipDB.GetRecord(spellID);
			if (record == null)
			{
				Debug.Log(string.Concat("GetItemDescription: spellID ", spellID, " not found in spellTooltipDB."));
			}
			else
			{
				str = empty;
				empty = string.Concat(new string[] { str, "<color=#", GeneralHelpers.s_friendlyColor, ">", WowTextParser.parser.Parse(record.Description, spellID), "</color>" });
			}
		}
		if (itemRec.Description != null && itemRec.Description != string.Empty)
		{
			if (empty != string.Empty)
			{
				empty = string.Concat(empty, "\n\n");
			}
			if (itemRec.ID != 141028 && itemRec.ID != 137565 && itemRec.ID != 137560 && itemRec.ID != 137561)
			{
				str = empty;
				empty = string.Concat(new string[] { str, "<color=#", GeneralHelpers.s_defaultColor, ">\"", WowTextParser.parser.Parse(itemRec.Description, 0), "\"</color>" });
			}
		}
		return empty;
	}

	public static string GetItemQualityColor(int qualityID)
	{
		switch (qualityID)
		{
			case 0:
			{
				return "9d9d9dff";
			}
			case 1:
			{
				return "ffffffff";
			}
			case 2:
			{
				return "1eff00ff";
			}
			case 3:
			{
				return "0070ddff";
			}
			case 4:
			{
				return "a335eeff";
			}
			case 5:
			{
				return "ff8000ff";
			}
			case 6:
			{
				return "e6cc80ff";
			}
			case 7:
			{
				return "00ccffff";
			}
			case 8:
			{
				return "00ccffff";
			}
			default:
			{
				return "ffffffff";
			}
		}
	}

	public static string GetItemQualityColorTag(int qualityID)
	{
		return string.Concat("<color=#", GeneralHelpers.GetItemQualityColor(qualityID), ">");
	}

	public static Sprite GetLocalizedFollowerXpIcon()
	{
		string locale = Main.instance.GetLocale();
		return Resources.Load<Sprite>(string.Concat("MiscIcons/LocalizedIcons/", locale, "/XPBonus_Icon"));
	}

	public static uint GetMaxFollowerItemLevel()
	{
		return StaticDB.garrFollowerTypeDB.GetRecord(4).MaxItemLevel;
	}

	public static float GetMissionDurationTalentMultiplier()
	{
		float actionValueFlat = 1f;
		IEnumerator enumerator = PersistentTalentData.talentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonTalent current = (JamGarrisonTalent)enumerator.Current;
				if ((current.Flags & 1) != 0)
				{
					GarrTalentRec record = StaticDB.garrTalentDB.GetRecord(current.GarrTalentID);
					if (record != null)
					{
						if (record.GarrAbilityID > 0)
						{
							StaticDB.garrAbilityEffectDB.EnumRecordsByParentID((int)record.GarrAbilityID, (GarrAbilityEffectRec garrAbilityEffectRec) => {
								if (garrAbilityEffectRec.AbilityAction == 17)
								{
									actionValueFlat *= garrAbilityEffectRec.ActionValueFlat;
								}
								return true;
							});
						}
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
		return actionValueFlat;
	}

	private static int GetMobileAtlasMemberOverride(int iconFileDataID)
	{
		int num = 0;
		int num1 = iconFileDataID;
		switch (num1)
		{
			case 1383681:
			{
				num = 6150;
				break;
			}
			case 1383682:
			{
				num = 6149;
				break;
			}
			case 1383683:
			{
				num = 6148;
				break;
			}
			default:
			{
				if (num1 == 1380306)
				{
					num = 6147;
					break;
				}
				else if (num1 == 1390116)
				{
					num = 0;
					break;
				}
				else
				{
					break;
				}
			}
		}
		return num;
	}

	public static string GetMobileStatColorString(MobileStatColor color)
	{
		switch (color)
		{
			case MobileStatColor.MOBILE_STAT_COLOR_TRIVIAL:
			{
				return "808080ff";
			}
			case MobileStatColor.MOBILE_STAT_COLOR_NORMAL:
			{
				return "ffffffff";
			}
			case MobileStatColor.MOBILE_STAT_COLOR_FRIENDLY:
			{
				return "00ff00ff";
			}
			case MobileStatColor.MOBILE_STAT_COLOR_HOSTILE:
			{
				return "ff0000ff";
			}
			case MobileStatColor.MOBILE_STAT_COLOR_INACTIVE:
			{
				return "808080ff";
			}
			case MobileStatColor.MOBILE_STAT_COLOR_ERROR:
			{
				return "ff2020ff";
			}
			default:
			{
				return "ffffffff";
			}
		}
	}

	public static int GetNumActiveChampions()
	{
		int num = 0;
		foreach (JamGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
		{
			bool flags = (value.Flags & 4) != 0;
			bool flag = (value.Flags & 8) != 0;
			if (flags || flag)
			{
				continue;
			}
			num++;
		}
		return num;
	}

	public static int GetNumInactiveChampions()
	{
		int num = 0;
		foreach (JamGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
		{
			bool flags = (value.Flags & 4) != 0;
			bool flag = (value.Flags & 8) != 0;
			if (!flags || flag)
			{
				continue;
			}
			num++;
		}
		return num;
	}

	public static int GetNumTroops()
	{
		int num = 0;
		foreach (JamGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
		{
			bool flags = (value.Flags & 4) != 0;
			bool flag = (value.Flags & 8) != 0;
			if (flags || !flag || value.Durability <= 0)
			{
				continue;
			}
			num++;
		}
		return num;
	}

	public static Color GetQualityColor(int quality)
	{
		Color color = Color.gray;
		Color color1 = Color.green;
		Color color2 = new Color(0.34f, 0.49f, 1f, 1f);
		Color color3 = new Color(0.58f, 0f, 1f, 1f);
		Color color4 = new Color(1f, 0.56f, 0f, 1f);
		switch (quality)
		{
			case 1:
			{
				return color;
			}
			case 2:
			{
				return color1;
			}
			case 3:
			{
				return color2;
			}
			case 4:
			{
				return color3;
			}
			case 5:
			{
				return color4;
			}
			case 6:
			{
				return color3;
			}
		}
		return Color.red;
	}

	public static void GetXpCapInfo(int followerLevel, int followerQuality, out uint xpToNextLevelOrQuality, out bool isQuality, out bool isMaxLevelAndMaxQuality)
	{
		isMaxLevelAndMaxQuality = false;
		isQuality = false;
		GarrFollowerLevelXPRec garrFollowerLevelXPRec = null;
		StaticDB.garrFollowerLevelXPDB.EnumRecordsByParentID(followerLevel, (GarrFollowerLevelXPRec rec) => {
			if (StaticDB.garrFollowerTypeDB.GetRecord((int)rec.GarrFollowerTypeID).GarrTypeID != 3)
			{
				return true;
			}
			garrFollowerLevelXPRec = rec;
			return false;
		});
		if (garrFollowerLevelXPRec.XpToNextLevel > 0)
		{
			xpToNextLevelOrQuality = garrFollowerLevelXPRec.XpToNextLevel;
			return;
		}
		isQuality = true;
		GarrFollowerQualityRec garrFollowerQualityRec = null;
		StaticDB.garrFollowerQualityDB.EnumRecordsByParentID(followerQuality, (GarrFollowerQualityRec rec) => {
			if (rec.GarrFollowerTypeID != 4)
			{
				return true;
			}
			garrFollowerQualityRec = rec;
			return false;
		});
		xpToNextLevelOrQuality = garrFollowerQualityRec.XpToNextQuality;
		if (garrFollowerQualityRec.XpToNextQuality == 0)
		{
			isMaxLevelAndMaxQuality = true;
		}
	}

	public static FollowerCanCounterMechanic HasFollowerWhoCanCounter(int garrMechanicTypeID)
	{
		bool flag = false;
		bool flag1 = false;
		foreach (JamGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
		{
			for (int i = 0; i < (int)value.AbilityID.Length; i++)
			{
				GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(value.AbilityID[i]);
				if (record == null)
				{
					Debug.Log(string.Concat(new object[] { "Invalid Ability ID ", value.AbilityID[i], " from follower ", value.GarrFollowerID }));
				}
				else if ((record.Flags & 1) == 0)
				{
					StaticDB.garrAbilityEffectDB.EnumRecordsByParentID(record.ID, (GarrAbilityEffectRec garrAbilityEffectRec) => {
						if (garrAbilityEffectRec.GarrMechanicTypeID == 0)
						{
							return FollowerCanCounterMechanic.no;
						}
						if (garrAbilityEffectRec.AbilityAction != 0)
						{
							return FollowerCanCounterMechanic.no;
						}
						if ((long)garrMechanicTypeID != (ulong)garrAbilityEffectRec.GarrMechanicTypeID)
						{
							return FollowerCanCounterMechanic.no;
						}
						if ((value.Flags & 4) == 0 && (value.Flags & 2) == 0 && value.CurrentMissionID == 0 && value.CurrentBuildingID == 0)
						{
							flag1 = true;
							return FollowerCanCounterMechanic.no;
						}
						flag = true;
						return FollowerCanCounterMechanic.no;
					});
				}
			}
		}
		if (flag1)
		{
			return FollowerCanCounterMechanic.yesAndAvailable;
		}
		if (flag)
		{
			return FollowerCanCounterMechanic.yesButBusy;
		}
		return FollowerCanCounterMechanic.no;
	}

	public static string LimitZhLineLength(string inText, int length)
	{
		if (!(Main.instance.GetLocale() == "zhCN") && !(Main.instance.GetLocale() == "zhTW"))
		{
			return inText;
		}
		int num = 0;
		string empty = string.Empty;
		while (inText.Substring(num).Length > length)
		{
			empty = string.Concat(empty, inText.Substring(num, length), " ");
			num += length;
		}
		empty = string.Concat(empty, inText.Substring(num));
		return empty;
	}

	public static Sprite LoadClassIcon(int classID)
	{
		Sprite sprite = null;
		switch (classID)
		{
			case 1:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Warrior");
				break;
			}
			case 2:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Paladin");
				break;
			}
			case 3:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Hunter");
				break;
			}
			case 4:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Rogue");
				break;
			}
			case 5:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Priest");
				break;
			}
			case 6:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Death Knight");
				break;
			}
			case 7:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Shaman");
				break;
			}
			case 8:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Mage");
				break;
			}
			case 9:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Warlock");
				break;
			}
			case 10:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Monk");
				break;
			}
			case 11:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Druid");
				break;
			}
			case 12:
			{
				sprite = Resources.Load<Sprite>("NewLoginPanel/Character-Selection-Demon Hunter");
				break;
			}
		}
		return sprite;
	}

	public static Sprite LoadCurrencyIcon(int currencyID)
	{
		Sprite sprite = AssetBundleManager.Icons.LoadAsset<Sprite>(string.Concat("Currency_", currencyID.ToString("00000000")));
		return sprite;
	}

	public static Font LoadFancyFont()
	{
		string locale = Main.instance.GetLocale();
		if (GeneralHelpers.s_fancyFont == null)
		{
			if (locale == "koKR")
			{
				GeneralHelpers.s_fancyFont = Resources.Load<Font>("Fonts/Official/K_Pagetext");
			}
			else if (locale == "zhCN")
			{
				GeneralHelpers.s_fancyFont = Resources.Load<Font>("Fonts/Official/ARKai_C");
			}
			else if (locale == "zhTW")
			{
				GeneralHelpers.s_fancyFont = Resources.Load<Font>("Fonts/Official/bLEI00D");
			}
			else if (locale != "ruRU")
			{
				GeneralHelpers.s_fancyFont = Resources.Load<Font>("Fonts/Official/MORPHEUS");
			}
			else
			{
				GeneralHelpers.s_fancyFont = Resources.Load<Font>("Fonts/Official/MORPHEUS_CYR");
			}
		}
		return GeneralHelpers.s_fancyFont;
	}

	public static Sprite LoadIconAsset(AssetBundleType assetBundleType, int iconFileDataID)
	{
		if (iconFileDataID == 894556)
		{
			string locale = Main.instance.GetLocale();
			return Resources.Load<Sprite>(string.Concat("MiscIcons/LocalizedIcons/", locale, "/XP_Icon"));
		}
		if (iconFileDataID == 1380306)
		{
			return Resources.Load<Sprite>("MissionMechanicEffects/MechanicIcon-Curse-Shadow");
		}
		if (iconFileDataID == 1383683)
		{
			return Resources.Load<Sprite>("MissionMechanicEffects/MechanicIcon-Disorienting-Shadow");
		}
		if (iconFileDataID == 1383682)
		{
			return Resources.Load<Sprite>("MissionMechanicEffects/MechanicIcon-Lethal-Shadow");
		}
		if (iconFileDataID == 1390116)
		{
			return Resources.Load<Sprite>("MissionMechanicEffects/MechanicIcon-Powerful-Shadow");
		}
		if (iconFileDataID == 1383681)
		{
			return Resources.Load<Sprite>("MissionMechanicEffects/MechanicIcon-Slowing-Shadow");
		}
		int mobileAtlasMemberOverride = GeneralHelpers.GetMobileAtlasMemberOverride(iconFileDataID);
		if (mobileAtlasMemberOverride > 0)
		{
			return TextureAtlas.instance.GetAtlasSprite(mobileAtlasMemberOverride);
		}
		AssetBundle icons = null;
		string empty = string.Empty;
		if (assetBundleType == AssetBundleType.Icons)
		{
			empty = "Assets/BundleAssets/Icons/";
			icons = AssetBundleManager.Icons;
		}
		if (assetBundleType == AssetBundleType.PortraitIcons)
		{
			empty = "Assets/BundleAssets/PortraitIcons/";
			icons = AssetBundleManager.portraitIcons;
		}
		string str = string.Concat(empty, iconFileDataID, ".tga");
		Sprite sprite = icons.LoadAsset<Sprite>(str);
		if (sprite == null)
		{
			str = string.Concat(empty, iconFileDataID, ".png");
			sprite = icons.LoadAsset<Sprite>(str);
		}
		return sprite;
	}

	public static Font LoadStandardFont()
	{
		string locale = Main.instance.GetLocale();
		if (GeneralHelpers.s_standardFont == null)
		{
			if (locale == "koKR")
			{
				GeneralHelpers.s_standardFont = Resources.Load<Font>("Fonts/Official/2002");
			}
			else if (locale == "zhCN")
			{
				GeneralHelpers.s_standardFont = Resources.Load<Font>("Fonts/Official/bHEI00M");
			}
			else if (locale == "zhTW")
			{
				GeneralHelpers.s_standardFont = Resources.Load<Font>("Fonts/Official/ARHei");
			}
			else if (locale != "ruRU")
			{
				GeneralHelpers.s_standardFont = Resources.Load<Font>("Fonts/Official/BLIZQUADRATA");
			}
			else
			{
				GeneralHelpers.s_standardFont = Resources.Load<Font>("Fonts/Official/FRIZQT___CYR");
			}
		}
		return GeneralHelpers.s_standardFont;
	}

	public static bool MissionHasUncounteredDeadly(GameObject enemyPortraitsGroup)
	{
		bool flag = false;
		if (enemyPortraitsGroup != null)
		{
			MissionMechanic[] componentsInChildren = enemyPortraitsGroup.GetComponentsInChildren<MissionMechanic>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				MissionMechanic missionMechanic = componentsInChildren[i];
				if (!missionMechanic.IsCountered())
				{
					if (missionMechanic.AbilityID() != 0)
					{
						StaticDB.garrAbilityEffectDB.EnumRecordsByParentID(missionMechanic.AbilityID(), (GarrAbilityEffectRec garrAbilityEffectRec) => {
							if (garrAbilityEffectRec.AbilityAction != 27)
							{
								return true;
							}
							flag = true;
							return false;
						});
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		return flag;
	}

	public static bool SpellGrantsArtifactXP(int spellID)
	{
		if (spellID <= 0)
		{
			return false;
		}
		bool flag = false;
		StaticDB.spellEffectDB.EnumRecordsByParentID(spellID, (SpellEffectRec effectRec) => {
			if (effectRec.Effect != 240)
			{
				return true;
			}
			flag = true;
			return false;
		});
		return flag;
	}

	public static string TextOrderString(string text1, string text2)
	{
		if (Main.instance.GetLocale() == "koKR")
		{
			return string.Format("{0} {1}", text2, text1);
		}
		return string.Format("{0} {1}", text1, text2);
	}
}