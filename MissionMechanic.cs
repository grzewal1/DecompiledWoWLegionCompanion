using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class MissionMechanic : MonoBehaviour
{
	public int m_missionMechanicTypeID;

	public Image m_missionMechanicIcon;

	public Image m_missionMechanicIconBorder;

	public Image m_counteredIcon;

	public Image m_canCounterButBusyIcon;

	public Shader m_grayscaleShader;

	private bool m_isCountered;

	private int m_garrAbilityID;

	private int m_counterWithThisAbilityID;

	public MissionMechanic()
	{
	}

	public int AbilityID()
	{
		return this.m_garrAbilityID;
	}

	public static int GetAbilityToCounterMechanicType(int garrMechanicTypeID)
	{
		int garrAbilityID = 0;
		StaticDB.garrFollowerDB.EnumRecords((GarrFollowerRec garrFollowerRec) => {
			if (garrFollowerRec.GarrFollowerTypeID != 4)
			{
				return true;
			}
			if (garrFollowerRec.ChrClassID != GarrisonStatus.CharacterClassID() && garrFollowerRec.ChrClassID != 0)
			{
				return true;
			}
			StaticDB.garrFollowerXAbilityDB.EnumRecordsByParentID(garrFollowerRec.ID, (GarrFollowerXAbilityRec xAbilityRec) => {
				StaticDB.garrAbilityEffectDB.EnumRecords((GarrAbilityEffectRec garrAbilityEffectRec) => {
					if (garrAbilityEffectRec.GarrMechanicTypeID == 0)
					{
						return true;
					}
					if (garrAbilityEffectRec.AbilityAction != 0)
					{
						return true;
					}
					if ((ulong)garrAbilityEffectRec.GarrAbilityID != (long)xAbilityRec.GarrAbilityID || (ulong)garrAbilityEffectRec.GarrMechanicTypeID != (long)garrMechanicTypeID)
					{
						return true;
					}
					garrAbilityID = xAbilityRec.GarrAbilityID;
					return false;
				});
				if (garrAbilityID != 0)
				{
					return false;
				}
				return true;
			});
			if (garrAbilityID != 0)
			{
				return false;
			}
			return true;
		});
		return garrAbilityID;
	}

	public static List<int> GetUsefulBuffAbilitiesForFollower(int garrFollowerID)
	{
		List<int> nums = new List<int>();
		int[] abilityID = PersistentFollowerData.followerDictionary[garrFollowerID].AbilityID;
		for (int i = 0; i < (int)abilityID.Length; i++)
		{
			int num = abilityID[i];
			GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(num);
			if (record != null)
			{
				if ((record.Flags & 1024) != 0)
				{
					StaticDB.garrAbilityEffectDB.EnumRecordsByParentID(num, (GarrAbilityEffectRec garrAbilityEffectRec) => {
						if (garrAbilityEffectRec.AbilityAction == 0)
						{
							return true;
						}
						nums.Add(num);
						return true;
					});
				}
			}
		}
		return nums;
	}

	public bool IsCountered()
	{
		return this.m_isCountered;
	}

	public void SetCountered(bool isCountered, bool counteringFollowerIsBusy = false, bool playCounteredEffect = true)
	{
		if (isCountered && this.m_isCountered)
		{
			return;
		}
		if (counteringFollowerIsBusy)
		{
			if (!this.m_counteredIcon.gameObject.activeSelf)
			{
				this.m_canCounterButBusyIcon.gameObject.SetActive(true);
			}
			return;
		}
		this.m_isCountered = isCountered;
		this.m_counteredIcon.gameObject.SetActive(isCountered);
		if (this.m_isCountered && this.m_canCounterButBusyIcon != null)
		{
			this.m_canCounterButBusyIcon.gameObject.SetActive(false);
		}
		this.m_missionMechanicIcon.material.SetFloat("_GrayscaleAmount", (!this.m_isCountered ? 1f : 0f));
		if (this.m_isCountered && playCounteredEffect)
		{
			UiAnimMgr.instance.PlayAnim("GreenCheck", base.transform, Vector3.zero, 1.8f, 0f);
			Main.instance.m_UISound.Play_GreenCheck();
		}
	}

	public void SetMechanicType(int missionMechanicTypeID, int mechanicAbilityID, bool hideBorder = false)
	{
		this.m_garrAbilityID = mechanicAbilityID;
		GarrMechanicTypeRec record = StaticDB.garrMechanicTypeDB.GetRecord(missionMechanicTypeID);
		if (record == null)
		{
			this.m_missionMechanicIcon.gameObject.SetActive(false);
		}
		else
		{
			this.m_missionMechanicIcon.gameObject.SetActive(true);
			this.m_missionMechanicTypeID = record.ID;
			this.m_counterWithThisAbilityID = MissionMechanic.GetAbilityToCounterMechanicType(missionMechanicTypeID);
			if (this.m_counterWithThisAbilityID != 0)
			{
				GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(this.m_counterWithThisAbilityID);
				if (garrAbilityRec != null)
				{
					Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, garrAbilityRec.IconFileDataID);
					if (sprite != null)
					{
						this.m_missionMechanicIcon.sprite = sprite;
						if (this.m_grayscaleShader != null)
						{
							Material material = new Material(this.m_grayscaleShader);
							this.m_missionMechanicIcon.material = material;
						}
					}
				}
			}
		}
		this.SetCountered(false, false, true);
		this.m_missionMechanicIconBorder.gameObject.SetActive(!hideBorder);
	}

	public void SetMechanicTypeWithMechanicID(int missionMechanicID, bool hideBorder = false)
	{
		GarrMechanicRec record = StaticDB.garrMechanicDB.GetRecord(missionMechanicID);
		if (record == null)
		{
			Debug.LogWarning(string.Concat("Invalid MissionMechanicID ", missionMechanicID));
			return;
		}
		this.SetMechanicType((int)record.GarrMechanicTypeID, record.GarrAbilityID, hideBorder);
	}

	public void ShowTooltip()
	{
		Main.instance.allPopups.ShowAbilityInfoPopup(this.m_counterWithThisAbilityID);
	}
}