using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class GeneralHelpers : MonoBehaviour
	{
		public static string s_defaultColor;

		public static string s_normalColor;

		public static string s_friendlyColor;

		static GeneralHelpers()
		{
			GeneralHelpers.s_defaultColor = "ffd200ff";
			GeneralHelpers.s_normalColor = "ffffffff";
			GeneralHelpers.s_friendlyColor = "00ff00ff";
		}

		public GeneralHelpers()
		{
		}

		public static long ApplyArtifactXPMultiplier(long inputAmount, double multiplier)
		{
			long num;
			if (multiplier <= 1)
			{
				return inputAmount;
			}
			double num1 = (double)inputAmount * multiplier;
			if (num1 < 50)
			{
				num = (long)1;
			}
			else if (num1 >= 1000)
			{
				num = (num1 >= 5000 ? (long)50 : (long)25);
			}
			else
			{
				num = (long)5;
			}
			return num * (long)Math.Round(num1 / (double)num);
		}

		public static int CurrentUnixTime()
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow.Subtract(new DateTime(1970, 1, 1));
			return (int)timeSpan.TotalSeconds;
		}

		public static int GetAdjustedMissionDuration(GarrMissionRec garrMissionRec, List<WrapperGarrisonFollower> followerList, GameObject enemyPortraits)
		{
			IEnumerable<GarrAbilityEffectRec> garrAbilityEffectRecs;
			float missionDuration = (float)garrMissionRec.MissionDuration;
			if (enemyPortraits != null)
			{
				garrAbilityEffectRecs = (
					from mechanic in (IEnumerable<MissionMechanic>)enemyPortraits.GetComponentsInChildren<MissionMechanic>(true)
					where (mechanic.IsCountered() ? false : mechanic.AbilityID() != 0)
					select mechanic).SelectMany<MissionMechanic, GarrAbilityEffectRec>((MissionMechanic mechanic) => StaticDB.garrAbilityEffectDB.GetRecordsByParentID(mechanic.AbilityID())).Where<GarrAbilityEffectRec>((GarrAbilityEffectRec garrAbilityEffectRec) => garrAbilityEffectRec.AbilityAction == 17);
				foreach (GarrAbilityEffectRec garrAbilityEffectRec1 in garrAbilityEffectRecs)
				{
					missionDuration *= garrAbilityEffectRec1.ActionValueFlat;
				}
			}
			garrAbilityEffectRecs = followerList.SelectMany<WrapperGarrisonFollower, GarrAbilityEffectRec>((WrapperGarrisonFollower follower) => follower.AbilityIDs.SelectMany<int, GarrAbilityEffectRec>((int abilityID) => StaticDB.garrAbilityEffectDB.GetRecordsByParentID(abilityID))).Where<GarrAbilityEffectRec>((GarrAbilityEffectRec garrAbilityEffectRec) => garrAbilityEffectRec.AbilityAction == 17);
			foreach (GarrAbilityEffectRec garrAbilityEffectRec2 in garrAbilityEffectRecs)
			{
				missionDuration *= garrAbilityEffectRec2.ActionValueFlat;
			}
			missionDuration *= GeneralHelpers.GetMissionDurationTalentMultiplier();
			return (int)missionDuration;
		}

		public static string GetBonusStatString(BonusStatIndex statIndex)
		{
			switch (statIndex)
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
					if (statIndex == BonusStatIndex.MASTERY_RATING)
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

		public static int[] GetBuffsForCurrentMission(int garrFollowerID, int garrMissionID, GameObject missionFollowerSlotGroup, int missionDuration)
		{
			HashSet<int> nums = new HashSet<int>();
			if (!PersistentFollowerData.followerDictionary.ContainsKey(garrFollowerID))
			{
				return nums.ToArray<int>();
			}
			WrapperGarrisonFollower item = PersistentFollowerData.followerDictionary[garrFollowerID];
			for (int i = 0; i < item.AbilityIDs.Count; i++)
			{
				GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(item.AbilityIDs[i]);
				if (record != null)
				{
					foreach (GarrAbilityEffectRec recordsByParentID in StaticDB.garrAbilityEffectDB.GetRecordsByParentID(record.ID))
					{
						bool flag = false;
						uint abilityAction = recordsByParentID.AbilityAction;
						switch (abilityAction)
						{
							case 0:
							{
								break;
							}
							case 1:
							{
								MissionFollowerSlot[] componentsInChildren = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
								int num = 0;
								MissionFollowerSlot[] missionFollowerSlotArray = componentsInChildren;
								for (int j = 0; j < (int)missionFollowerSlotArray.Length; j++)
								{
									if (missionFollowerSlotArray[j].GetCurrentGarrFollowerID() > 0)
									{
										num++;
									}
								}
								flag = num == 1;
								break;
							}
							case 5:
							{
								MissionFollowerSlot[] componentsInChildren1 = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
								bool flag1 = false;
								MissionFollowerSlot[] missionFollowerSlotArray1 = componentsInChildren1;
								for (int k = 0; k < (int)missionFollowerSlotArray1.Length; k++)
								{
									int currentGarrFollowerID = missionFollowerSlotArray1[k].GetCurrentGarrFollowerID();
									if (currentGarrFollowerID > 0 && currentGarrFollowerID != item.GarrFollowerID)
									{
										GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord(currentGarrFollowerID);
										if (garrFollowerRec != null)
										{
											if ((GarrisonStatus.Faction() != PVP_FACTION.ALLIANCE ? garrFollowerRec.HordeGarrFollRaceID : garrFollowerRec.AllianceGarrFollRaceID) == recordsByParentID.ActionRace)
											{
												flag1 = true;
												break;
											}
										}
									}
								}
								flag = flag1;
								break;
							}
							case 6:
							{
								flag = (float)missionDuration >= (float)((float)recordsByParentID.ActionHours) * 3600f;
								break;
							}
							case 7:
							{
								flag = (float)missionDuration <= (float)((float)recordsByParentID.ActionHours) * 3600f;
								break;
							}
							case 9:
							{
								GarrMissionRec garrMissionRec = StaticDB.garrMissionDB.GetRecord(garrMissionID);
								flag = (garrMissionRec == null ? false : (float)((float)garrMissionRec.TravelDuration) >= (float)((float)recordsByParentID.ActionHours) * 3600f);
								break;
							}
							case 10:
							{
								GarrMissionRec record1 = StaticDB.garrMissionDB.GetRecord(garrMissionID);
								flag = (record1 == null ? false : (float)((float)record1.TravelDuration) <= (float)((float)recordsByParentID.ActionHours) * 3600f);
								break;
							}
							case 12:
							{
								break;
							}
							default:
							{
								switch (abilityAction)
								{
									case 22:
									{
										MissionFollowerSlot[] componentsInChildren2 = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
										bool flag2 = false;
										MissionFollowerSlot[] missionFollowerSlotArray2 = componentsInChildren2;
										for (int l = 0; l < (int)missionFollowerSlotArray2.Length; l++)
										{
											int currentGarrFollowerID1 = missionFollowerSlotArray2[l].GetCurrentGarrFollowerID();
											if (currentGarrFollowerID1 > 0 && currentGarrFollowerID1 != item.GarrFollowerID)
											{
												GarrFollowerRec garrFollowerRec1 = StaticDB.garrFollowerDB.GetRecord(currentGarrFollowerID1);
												if (garrFollowerRec1 != null)
												{
													if ((GarrisonStatus.Faction() != PVP_FACTION.ALLIANCE ? garrFollowerRec1.HordeGarrClassSpecID : garrFollowerRec1.AllianceGarrClassSpecID) == recordsByParentID.ActionRecordID)
													{
														flag2 = true;
														break;
													}
												}
											}
										}
										flag = flag2;
										break;
									}
									case 23:
									{
										bool flag3 = false;
										if (PersistentMissionData.missionDictionary.ContainsKey(garrMissionID))
										{
											WrapperGarrisonMission wrapperGarrisonMission = PersistentMissionData.missionDictionary[garrMissionID];
											for (int m = 0; m < wrapperGarrisonMission.Encounters.Count; m++)
											{
												int num1 = 0;
												while (num1 < wrapperGarrisonMission.Encounters[m].MechanicIDs.Count)
												{
													GarrMechanicDB garrMechanicDB = StaticDB.garrMechanicDB;
													WrapperGarrisonEncounter wrapperGarrisonEncounter = wrapperGarrisonMission.Encounters[m];
													GarrMechanicRec garrMechanicRec = garrMechanicDB.GetRecord(wrapperGarrisonEncounter.MechanicIDs[num1]);
													if (garrMechanicRec == null || recordsByParentID.GarrMechanicTypeID != garrMechanicRec.GarrMechanicTypeID)
													{
														num1++;
													}
													else
													{
														flag3 = true;
														break;
													}
												}
											}
										}
										flag = flag3;
										break;
									}
									case 26:
									{
										MissionFollowerSlot[] componentsInChildren3 = missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
										bool flag4 = false;
										MissionFollowerSlot[] missionFollowerSlotArray3 = componentsInChildren3;
										int num2 = 0;
										while (num2 < (int)missionFollowerSlotArray3.Length)
										{
											int currentGarrFollowerID2 = missionFollowerSlotArray3[num2].GetCurrentGarrFollowerID();
											if (currentGarrFollowerID2 <= 0 || currentGarrFollowerID2 == item.GarrFollowerID || (ulong)recordsByParentID.ActionRecordID != (long)currentGarrFollowerID2)
											{
												num2++;
											}
											else
											{
												flag4 = true;
												break;
											}
										}
										flag = flag4;
										break;
									}
									default:
									{
										if (abilityAction == 37)
										{
										}
										break;
									}
								}
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
						nums.Add(record.ID);
					}
				}
				else
				{
					Debug.Log(string.Concat(new object[] { "Invalid Ability ID ", item.AbilityIDs[i], " from follower ", item.GarrFollowerID }));
				}
			}
			return nums.ToArray<int>();
		}

		public static string GetColorFromInt(int color)
		{
			int num = color >> 16 & 255;
			int num1 = color >> 8 & 255;
			int num2 = color & 255;
			string str = string.Format("{0:X2}{1:X2}{2:X2}", num, num1, num2);
			return str;
		}

		public static FollowerStatus GetFollowerStatus(WrapperGarrisonFollower follower)
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
			int num = (
				from effectRec in StaticDB.itemEffectDB.GetRecordsByParentID(itemRec.ID)
				select effectRec.SpellID).FirstOrDefault<int>((int id) => id != 0);
			if (num > 0)
			{
				SpellTooltipRec record = StaticDB.spellTooltipDB.GetRecord(num);
				if (record == null)
				{
					Debug.Log(string.Concat("GetItemDescription: spellID ", num, " not found in spellTooltipDB."));
				}
				else
				{
					str = empty;
					empty = string.Concat(new string[] { str, "<color=#", GeneralHelpers.s_friendlyColor, ">", WowTextParser.parser.Parse(record.Description, num), "</color>" });
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
			foreach (WrapperGarrisonTalent value in PersistentTalentData.talentDictionary.Values)
			{
				if ((value.Flags & 1) != 0)
				{
					GarrTalentRec record = StaticDB.garrTalentDB.GetRecord(value.GarrTalentID);
					if (record != null)
					{
						if (record.GarrAbilityID > 0)
						{
							foreach (GarrAbilityEffectRec garrAbilityEffectRec in 
								from rec in StaticDB.garrAbilityEffectDB.GetRecordsByParentID((int)record.GarrAbilityID)
								where rec.AbilityAction == 17
								select rec)
							{
								actionValueFlat *= garrAbilityEffectRec.ActionValueFlat;
							}
						}
					}
				}
			}
			return actionValueFlat;
		}

		private static int GetMobileAtlasMemberOverride(int iconFileDataID)
		{
			int num = 0;
			switch (iconFileDataID)
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
					if (iconFileDataID == 1380306)
					{
						num = 6147;
						break;
					}
					else if (iconFileDataID == 1390116)
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

		public static string GetMobileStatColorString(WrapperStatColor color)
		{
			switch (color)
			{
				case WrapperStatColor.MOBILE_STAT_COLOR_TRIVIAL:
				{
					return "808080ff";
				}
				case WrapperStatColor.MOBILE_STAT_COLOR_NORMAL:
				{
					return "ffffffff";
				}
				case WrapperStatColor.MOBILE_STAT_COLOR_FRIENDLY:
				{
					return "00ff00ff";
				}
				case WrapperStatColor.MOBILE_STAT_COLOR_HOSTILE:
				{
					return "ff0000ff";
				}
				case WrapperStatColor.MOBILE_STAT_COLOR_INACTIVE:
				{
					return "808080ff";
				}
				case WrapperStatColor.MOBILE_STAT_COLOR_ERROR:
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
			return PersistentFollowerData.followerDictionary.Count<KeyValuePair<int, WrapperGarrisonFollower>>((KeyValuePair<int, WrapperGarrisonFollower> pair) => (GeneralHelpers.IsFollowerInactive(pair.Value) ? false : !GeneralHelpers.IsFollowerTroop(pair.Value)));
		}

		public static int GetNumInactiveChampions()
		{
			return PersistentFollowerData.followerDictionary.Count<KeyValuePair<int, WrapperGarrisonFollower>>((KeyValuePair<int, WrapperGarrisonFollower> pair) => (!GeneralHelpers.IsFollowerInactive(pair.Value) ? false : !GeneralHelpers.IsFollowerTroop(pair.Value)));
		}

		public static int GetNumTroops()
		{
			return PersistentFollowerData.followerDictionary.Count<KeyValuePair<int, WrapperGarrisonFollower>>((KeyValuePair<int, WrapperGarrisonFollower> pair) => (GeneralHelpers.IsFollowerInactive(pair.Value) || !GeneralHelpers.IsFollowerTroop(pair.Value) ? false : pair.Value.Durability > 0));
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
			return Color.gray;
		}

		public static void GetXpCapInfo(int followerLevel, int followerQuality, out uint xpToNextLevelOrQuality, out bool isQuality, out bool isMaxLevelAndMaxQuality)
		{
			isMaxLevelAndMaxQuality = false;
			isQuality = false;
			GarrFollowerLevelXPRec garrFollowerLevelXPRec = StaticDB.garrFollowerLevelXPDB.GetRecordsByParentID(followerLevel).First<GarrFollowerLevelXPRec>((GarrFollowerLevelXPRec rec) => StaticDB.garrFollowerTypeDB.GetRecord((int)rec.GarrFollowerTypeID).GarrTypeID == 3);
			if (garrFollowerLevelXPRec.XpToNextLevel > 0)
			{
				xpToNextLevelOrQuality = garrFollowerLevelXPRec.XpToNextLevel;
				return;
			}
			isQuality = true;
			GarrFollowerQualityRec garrFollowerQualityRec = StaticDB.garrFollowerQualityDB.GetRecordsByParentID(followerQuality).First<GarrFollowerQualityRec>((GarrFollowerQualityRec rec) => rec.GarrFollowerTypeID == 4);
			xpToNextLevelOrQuality = garrFollowerQualityRec.XpToNextQuality;
			if (garrFollowerQualityRec.XpToNextQuality == 0)
			{
				isMaxLevelAndMaxQuality = true;
			}
		}

		public static FollowerCanCounterMechanic HasFollowerWhoCanCounter(int garrMechanicTypeID)
		{
			FollowerCanCounterMechanic followerCanCounterMechanic;
			bool flag = false;
			Dictionary<int, WrapperGarrisonFollower>.ValueCollection.Enumerator enumerator = PersistentFollowerData.followerDictionary.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WrapperGarrisonFollower current = enumerator.Current;
					for (int i = 0; i < current.AbilityIDs.Count; i++)
					{
						GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(current.AbilityIDs[i]);
						if (record == null)
						{
							Debug.Log(string.Concat(new object[] { "Invalid Ability ID ", current.AbilityIDs[i], " from follower ", current.GarrFollowerID }));
						}
						else if ((record.Flags & 1) == 0)
						{
							IEnumerable<GarrAbilityEffectRec> recordsByParentID = 
								from garrAbilityEffectRec in StaticDB.garrAbilityEffectDB.GetRecordsByParentID(record.ID)
								where (garrAbilityEffectRec.GarrMechanicTypeID == 0 || garrAbilityEffectRec.AbilityAction != 0 ? false : (long)garrMechanicTypeID == (ulong)garrAbilityEffectRec.GarrMechanicTypeID)
								select garrAbilityEffectRec;
							if (!recordsByParentID.Any<GarrAbilityEffectRec>((GarrAbilityEffectRec garrAbilityEffectRec) => ((current.Flags & 4) != 0 || (current.Flags & 2) != 0 || current.CurrentMissionID != 0 ? 1 : (int)(current.CurrentBuildingID != 0)) == 0))
							{
								flag = flag | recordsByParentID.Count<GarrAbilityEffectRec>() > 0;
							}
							else
							{
								followerCanCounterMechanic = FollowerCanCounterMechanic.yesAndAvailable;
								return followerCanCounterMechanic;
							}
						}
					}
				}
				if (flag)
				{
					return FollowerCanCounterMechanic.yesButBusy;
				}
				return FollowerCanCounterMechanic.no;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return followerCanCounterMechanic;
		}

		private static bool IsFollowerInactive(WrapperGarrisonFollower follower)
		{
			return (follower.Flags & 4) != 0;
		}

		private static bool IsFollowerTroop(WrapperGarrisonFollower follower)
		{
			return (follower.Flags & 8) != 0;
		}

		public static string LimitZhLineLength(string inText, int length)
		{
			if (Main.instance.GetLocale() != "zhCN" && Main.instance.GetLocale() != "zhTW")
			{
				return inText;
			}
			bool flag = false;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < inText.Length; i++)
			{
				string str = inText.Substring(i, 1);
				stringBuilder.Append(str);
				if (str == "<")
				{
					flag = true;
				}
				else if (str == ">")
				{
					flag = false;
				}
				else if (!flag)
				{
					num++;
					if (num > length)
					{
						stringBuilder.Append(" ");
						num = 0;
					}
				}
			}
			return stringBuilder.ToString();
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
			return FontLoader.LoadFancyFont();
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
				icons = AssetBundleManager.PortraitIcons;
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
			return FontLoader.LoadStandardFont();
		}

		public static bool MissionHasUncounteredDeadly(GameObject enemyPortraitsGroup)
		{
			if (enemyPortraitsGroup == null)
			{
				return false;
			}
			return (
				from mechanic in (IEnumerable<MissionMechanic>)enemyPortraitsGroup.GetComponentsInChildren<MissionMechanic>(true)
				where (mechanic.IsCountered() ? false : mechanic.AbilityID() != 0)
				select mechanic).SelectMany<MissionMechanic, GarrAbilityEffectRec>((MissionMechanic mechanic) => StaticDB.garrAbilityEffectDB.GetRecordsByParentID(mechanic.AbilityID())).Any<GarrAbilityEffectRec>((GarrAbilityEffectRec garrAbilityEffectRec) => garrAbilityEffectRec.AbilityAction == 27);
		}

		public static bool SpellGrantsArtifactXP(int spellID)
		{
			if (spellID <= 0)
			{
				return false;
			}
			return StaticDB.spellEffectDB.GetRecordsByParentID(spellID).Any<SpellEffectRec>((SpellEffectRec effectRec) => effectRec.Effect == 240);
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
}