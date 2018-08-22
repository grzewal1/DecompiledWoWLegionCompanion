using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class TroopSlot : MonoBehaviour
	{
		public Image m_troopPortraitImage;

		public Image m_troopBuildEmptyRing;

		public Image m_troopOwnedCheckmark;

		public Text m_timeRemainingText;

		public Shader m_grayscaleShader;

		public Transform m_greenCheckEffectRoot;

		public GameObject m_collectingSpinner;

		private int m_ownedGarrFollowerID;

		private bool m_training;

		private bool m_collected;

		private bool m_pendingCreate;

		private DateTime m_shipmentCreationTime;

		private TimeSpan m_shipmentDuration;

		private ulong m_shipmentDBID;

		private UiAnimMgr.UiAnimHandle m_glowLoopHandle;

		public TroopSlot()
		{
		}

		private void Awake()
		{
			this.m_collected = false;
			this.m_pendingCreate = false;
			this.m_collectingSpinner.SetActive(false);
		}

		public ulong GetDBID()
		{
			return this.m_shipmentDBID;
		}

		public int GetOwnedFollowerID()
		{
			return this.m_ownedGarrFollowerID;
		}

		private void HandleCollectTroopResult(SHIPMENT_RESULT result, ulong shipmentDBID)
		{
			if (result == SHIPMENT_RESULT.SUCCESS && shipmentDBID == this.m_shipmentDBID)
			{
				if (this.m_glowLoopHandle != null)
				{
					UiAnimation anim = this.m_glowLoopHandle.GetAnim();
					if (anim != null)
					{
						anim.Stop(0.5f);
					}
					this.m_glowLoopHandle = null;
				}
				UiAnimMgr.instance.PlayAnim("GreenCheckRound", this.m_greenCheckEffectRoot, Vector3.zero, 1.8f, 0f);
				Main.instance.m_UISound.Play_GreenCheck();
				this.m_training = false;
				this.m_troopOwnedCheckmark.gameObject.SetActive(true);
				this.m_troopPortraitImage.gameObject.SetActive(true);
				this.m_timeRemainingText.gameObject.SetActive(false);
				this.m_troopPortraitImage.material = null;
				PersistentShipmentData.shipmentDictionary.Remove(shipmentDBID);
				LegionCompanionWrapper.RequestShipmentTypes((int)GarrisonStatus.GarrisonType);
				LegionCompanionWrapper.RequestShipments((int)GarrisonStatus.GarrisonType);
				LegionCompanionWrapper.RequestGarrisonData((int)GarrisonStatus.GarrisonType);
			}
		}

		public bool IsCollected()
		{
			return this.m_collected;
		}

		public bool IsEmpty()
		{
			return (this.m_training || this.m_collected || this.m_pendingCreate ? false : this.m_ownedGarrFollowerID == 0);
		}

		public bool IsOwned()
		{
			return this.m_ownedGarrFollowerID != 0;
		}

		public bool IsPendingCreate()
		{
			return this.m_pendingCreate;
		}

		public bool IsTraining()
		{
			return this.m_training;
		}

		public void OnCollectTroop()
		{
			TimeSpan timeSpan = GarrisonStatus.CurrentTime() - this.m_shipmentCreationTime;
			if ((this.m_shipmentDuration - timeSpan).TotalSeconds > 0)
			{
				return;
			}
			if (this.m_shipmentDBID != (long)0 && !this.m_collected)
			{
				this.m_collected = true;
				this.m_collectingSpinner.SetActive(true);
				UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 2f, 0f);
				Main.instance.m_UISound.Play_CollectTroop();
				LegionCompanionWrapper.CompleteShipment(this.m_shipmentDBID);
			}
		}

		private void OnDisable()
		{
			if (Main.instance != null)
			{
				Main.instance.CompleteShipmentResultAction -= new Action<SHIPMENT_RESULT, ulong>(this.HandleCollectTroopResult);
			}
		}

		private void OnEnable()
		{
			if (Main.instance != null)
			{
				Main.instance.CompleteShipmentResultAction += new Action<SHIPMENT_RESULT, ulong>(this.HandleCollectTroopResult);
			}
		}

		public void SetCharShipment(int charShipmentID, ulong shipmentDBID, int ownedGarrFollowerID, bool training, int iconFileDataID = 0)
		{
			CharShipmentRec record = StaticDB.charShipmentDB.GetRecord(charShipmentID);
			if (record == null)
			{
				Debug.LogError(string.Concat("Invalid Shipment ID: ", charShipmentID));
				return;
			}
			if (this.m_glowLoopHandle != null)
			{
				UiAnimation anim = this.m_glowLoopHandle.GetAnim();
				if (anim != null)
				{
					anim.Stop(0.5f);
				}
				this.m_glowLoopHandle = null;
			}
			this.m_collected = false;
			this.m_pendingCreate = false;
			this.m_collectingSpinner.SetActive(false);
			this.m_ownedGarrFollowerID = ownedGarrFollowerID;
			this.m_training = training;
			this.m_shipmentDBID = shipmentDBID;
			if (training)
			{
				if (PersistentShipmentData.shipmentDictionary.ContainsKey(shipmentDBID))
				{
					WrapperCharacterShipment item = PersistentShipmentData.shipmentDictionary[shipmentDBID];
					this.m_shipmentCreationTime = item.CreationTime;
					this.m_shipmentDuration = item.ShipmentDuration;
				}
				else
				{
					training = false;
					Debug.LogWarning(string.Concat("Shipment not found in Persistent: ", charShipmentID));
				}
			}
			if (record.GarrFollowerID > 0)
			{
				this.SetCharShipmentTroop(record, iconFileDataID);
			}
			else if (record.DummyItemID > 0)
			{
				this.SetCharShipmentItem(record);
			}
			if (ownedGarrFollowerID != 0)
			{
				this.m_troopBuildEmptyRing.gameObject.SetActive(false);
				this.m_troopOwnedCheckmark.gameObject.SetActive(true);
				this.m_troopPortraitImage.gameObject.SetActive(true);
				this.m_timeRemainingText.gameObject.SetActive(false);
				return;
			}
			if (!training)
			{
				this.m_troopBuildEmptyRing.gameObject.SetActive(false);
				this.m_troopOwnedCheckmark.gameObject.SetActive(false);
				this.m_troopPortraitImage.gameObject.SetActive(false);
				this.m_timeRemainingText.gameObject.SetActive(false);
			}
			else
			{
				this.m_troopBuildEmptyRing.gameObject.SetActive(true);
				this.m_troopOwnedCheckmark.gameObject.SetActive(false);
				this.m_troopPortraitImage.gameObject.SetActive(true);
				this.m_timeRemainingText.gameObject.SetActive(true);
				this.m_timeRemainingText.text = string.Empty;
				if (this.m_grayscaleShader != null)
				{
					Material material = new Material(this.m_grayscaleShader);
					this.m_troopPortraitImage.material = material;
				}
			}
		}

		private void SetCharShipmentItem(CharShipmentRec charShipmentRec)
		{
			ItemRec record = StaticDB.itemDB.GetRecord(charShipmentRec.DummyItemID);
			if (record == null)
			{
				Debug.LogError(string.Concat("Invalid Item ID: ", charShipmentRec.DummyItemID));
				return;
			}
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
			if (sprite != null)
			{
				this.m_troopPortraitImage.sprite = sprite;
			}
		}

		public void SetCharShipmentTroop(CharShipmentRec charShipmentRec, int iconFileDataID = 0)
		{
			GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord((int)charShipmentRec.GarrFollowerID);
			if (record == null)
			{
				Debug.LogError(string.Concat("Invalid Follower ID: ", charShipmentRec.GarrFollowerID));
				return;
			}
			if (iconFileDataID <= 0)
			{
				iconFileDataID = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceIconFileDataID : record.HordeIconFileDataID);
			}
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, iconFileDataID);
			if (sprite != null)
			{
				this.m_troopPortraitImage.sprite = sprite;
			}
		}

		public void SetPendingCreate()
		{
			this.m_pendingCreate = true;
			this.m_collectingSpinner.SetActive(true);
		}

		private void Update()
		{
			if (this.m_training)
			{
				TimeSpan timeSpan = GarrisonStatus.CurrentTime() - this.m_shipmentCreationTime;
				Mathf.Clamp((float)timeSpan.TotalSeconds / (float)this.m_shipmentDuration.TotalSeconds, 0f, 1f);
				TimeSpan mShipmentDuration = this.m_shipmentDuration - timeSpan;
				if (mShipmentDuration.TotalSeconds < 0)
				{
					mShipmentDuration = TimeSpan.Zero;
				}
				if (mShipmentDuration.TotalSeconds > 0)
				{
					this.m_timeRemainingText.text = mShipmentDuration.GetDurationString(true);
				}
				else if (this.m_glowLoopHandle == null)
				{
					this.m_glowLoopHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", base.transform, Vector3.zero, 2f, 0f);
					this.m_timeRemainingText.text = StaticDB.GetString("COLLECT", null);
					Main.instance.m_UISound.Play_TroopsReadyToast();
				}
			}
		}
	}
}