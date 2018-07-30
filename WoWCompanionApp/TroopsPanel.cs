using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

namespace WoWCompanionApp
{
	public class TroopsPanel : MonoBehaviour
	{
		public GameObject m_troopsListItemPrefab;

		public GameObject m_troopsListContents;

		public float m_listItemInitialEntranceDelay;

		public float m_listItemEntranceDelay;

		public Text m_noRecruitsYetMessage;

		public RectTransform m_panelViewRT;

		public GameObject m_resourcesDisplay;

		private GamePanel m_gamePanel;

		public TroopsPanel()
		{
		}

		private void Awake()
		{
			this.m_gamePanel = base.GetComponentInParent<GamePanel>();
			this.m_noRecruitsYetMessage.font = FontLoader.LoadStandardFont();
			this.m_noRecruitsYetMessage.text = StaticDB.GetString("NO_RECRUITS_AVAILABLE_YET", "You have no recruits available yet.");
			this.InitList();
		}

		private void HandleEnteredWorld()
		{
			TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
		}

		private void HandleFollowerDataChanged()
		{
			this.InitList();
			TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				componentsInChildren[i].HandleFollowerDataChanged();
			}
		}

		public void HandleOrderHallNavButtonSelected(OrderHallNavButton navButton)
		{
			TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ClearAndHideLootArea();
			}
		}

		private void HandleRecruitResult(int result)
		{
			if (result == 0)
			{
				LegionCompanionWrapper.RequestShipments((int)GarrisonStatus.GarrisonType);
			}
		}

		private void HandleShipmentItemPushed(int charShipmentID, WrapperShipmentItem item)
		{
			TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				TroopsListItem troopsListItem = componentsInChildren[i];
				if (troopsListItem.GetCharShipmentTypeID() == charShipmentID)
				{
					troopsListItem.HandleShipmentItemPushed(item);
				}
			}
		}

		private void InitList()
		{
			List<WrapperShipmentType> availableShipmentTypes = PersistentShipmentData.GetAvailableShipmentTypes();
			if (availableShipmentTypes == null || availableShipmentTypes.Count == 0)
			{
				this.m_noRecruitsYetMessage.gameObject.SetActive(true);
			}
			else
			{
				this.m_noRecruitsYetMessage.gameObject.SetActive(false);
			}
			TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			TroopsListItem[] troopsListItemArray = componentsInChildren;
			for (int i = 0; i < (int)troopsListItemArray.Length; i++)
			{
				TroopsListItem troopsListItem = troopsListItemArray[i];
				bool flag = true;
				if (availableShipmentTypes != null)
				{
					foreach (WrapperShipmentType availableShipmentType in availableShipmentTypes)
					{
						if (troopsListItem.GetCharShipmentTypeID() != availableShipmentType.CharShipmentID)
						{
							continue;
						}
						flag = false;
						break;
					}
				}
				if (flag)
				{
					UnityEngine.Object.Destroy(troopsListItem.gameObject);
				}
			}
			if (availableShipmentTypes == null)
			{
				return;
			}
			componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			for (int j = 0; j < availableShipmentTypes.Count; j++)
			{
				bool flag1 = false;
				TroopsListItem[] troopsListItemArray1 = componentsInChildren;
				int num = 0;
				while (num < (int)troopsListItemArray1.Length)
				{
					if (troopsListItemArray1[num].GetCharShipmentTypeID() != availableShipmentTypes[j].CharShipmentID)
					{
						num++;
					}
					else
					{
						flag1 = true;
						break;
					}
				}
				if (!flag1)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_troopsListItemPrefab);
					gameObject.transform.SetParent(this.m_troopsListContents.transform, false);
					TroopsListItem component = gameObject.GetComponent<TroopsListItem>();
					component.SetCharShipment(new WrapperShipmentType?(availableShipmentTypes[j]), false, null);
					FancyEntrance mListItemInitialEntranceDelay = component.GetComponent<FancyEntrance>();
					mListItemInitialEntranceDelay.m_timeToDelayEntrance = this.m_listItemInitialEntranceDelay + this.m_listItemEntranceDelay * (float)j;
					mListItemInitialEntranceDelay.Activate();
				}
			}
			foreach (WrapperCharacterShipment value in PersistentShipmentData.shipmentDictionary.Values)
			{
				if (!PersistentShipmentData.ShipmentTypeForShipmentIsAvailable(value.ShipmentRecID))
				{
					bool flag2 = true;
					bool flag3 = false;
					if (value.ShipmentRecID < 372 || value.ShipmentRecID > 383)
					{
						flag2 = false;
					}
					if (value.ShipmentRecID == 178 || value.ShipmentRecID == 179 || value.ShipmentRecID == 180 || value.ShipmentRecID == 192 || value.ShipmentRecID == 194 || value.ShipmentRecID == 195)
					{
						flag3 = true;
					}
					if (flag2 || flag3)
					{
						CharShipmentRec record = StaticDB.charShipmentDB.GetRecord(value.ShipmentRecID);
						if (record != null)
						{
							GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_troopsListItemPrefab);
							gameObject1.transform.SetParent(this.m_troopsListContents.transform, false);
							gameObject1.GetComponent<TroopsListItem>().SetCharShipment(null, true, record);
						}
					}
				}
			}
		}

		private void OnDisable()
		{
			Main.instance.CreateShipmentResultAction -= new Action<int>(this.HandleRecruitResult);
			Main.instance.FollowerDataChangedAction -= new Action(this.HandleFollowerDataChanged);
			Main.instance.ShipmentTypesUpdatedAction -= new Action(this.InitList);
			Main.instance.ShipmentItemPushedAction -= new Action<int, WrapperShipmentItem>(this.HandleShipmentItemPushed);
			this.m_gamePanel.OrderHallNavButtonSelectedAction -= new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected);
		}

		public void OnEnable()
		{
			Main.instance.CreateShipmentResultAction += new Action<int>(this.HandleRecruitResult);
			Main.instance.FollowerDataChangedAction += new Action(this.HandleFollowerDataChanged);
			Main.instance.ShipmentTypesUpdatedAction += new Action(this.InitList);
			Main.instance.ShipmentItemPushedAction += new Action<int, WrapperShipmentItem>(this.HandleShipmentItemPushed);
			this.m_gamePanel.OrderHallNavButtonSelectedAction += new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected);
			this.InitList();
		}

		public void PurgeList()
		{
			TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}