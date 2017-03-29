using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;
using WowStaticData;

public class TroopsPanel : MonoBehaviour
{
	public GameObject m_troopsListItemPrefab;

	public GameObject m_troopsListContents;

	public float m_listItemInitialEntranceDelay;

	public float m_listItemEntranceDelay;

	public Text m_noRecruitsYetMessage;

	public RectTransform m_panelViewRT;

	public TroopsPanel()
	{
	}

	private void Awake()
	{
		this.m_noRecruitsYetMessage.font = GeneralHelpers.LoadStandardFont();
		this.m_noRecruitsYetMessage.text = StaticDB.GetString("NO_RECRUITS_AVAILABLE_YET", "You have no recruits available yet.");
		this.InitList();
	}

	private void HandleEnteredWorld()
	{
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
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
			TroopsListItem troopsListItem = componentsInChildren[i];
			troopsListItem.ClearAndHideLootArea();
			troopsListItem.AddInventoryItems();
		}
	}

	private void HandleRecruitResult(int result)
	{
		if (result == 0)
		{
			MobilePlayerRequestShipments mobilePlayerRequestShipment = new MobilePlayerRequestShipments();
			Login.instance.SendToMobileServer(mobilePlayerRequestShipment);
		}
	}

	private void HandleShipmentItemPushed(int charShipmentID, MobileClientShipmentItem item)
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
		MobileClientShipmentType[] availableShipmentTypes = PersistentShipmentData.GetAvailableShipmentTypes();
		if (availableShipmentTypes == null || (int)availableShipmentTypes.Length == 0)
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
				MobileClientShipmentType[] mobileClientShipmentTypeArray = availableShipmentTypes;
				int num = 0;
				while (num < (int)mobileClientShipmentTypeArray.Length)
				{
					MobileClientShipmentType mobileClientShipmentType = mobileClientShipmentTypeArray[num];
					if (troopsListItem.GetCharShipmentTypeID() != mobileClientShipmentType.CharShipmentID)
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				UnityEngine.Object.DestroyImmediate(troopsListItem.gameObject);
			}
		}
		if (availableShipmentTypes == null)
		{
			return;
		}
		componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		for (int j = 0; j < (int)availableShipmentTypes.Length; j++)
		{
			bool flag1 = false;
			TroopsListItem[] troopsListItemArray1 = componentsInChildren;
			int num1 = 0;
			while (num1 < (int)troopsListItemArray1.Length)
			{
				if (troopsListItemArray1[num1].GetCharShipmentTypeID() != availableShipmentTypes[j].CharShipmentID)
				{
					num1++;
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
				component.SetCharShipment(availableShipmentTypes[j], false, null);
				FancyEntrance mListItemInitialEntranceDelay = component.GetComponent<FancyEntrance>();
				mListItemInitialEntranceDelay.m_timeToDelayEntrance = this.m_listItemInitialEntranceDelay + this.m_listItemEntranceDelay * (float)j;
				mListItemInitialEntranceDelay.Activate();
			}
		}
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamCharacterShipment current = (JamCharacterShipment)enumerator.Current;
				if (!PersistentShipmentData.ShipmentTypeForShipmentIsAvailable(current.ShipmentRecID))
				{
					bool flag2 = true;
					bool flag3 = false;
					if (current.ShipmentRecID < 372 || current.ShipmentRecID > 383)
					{
						flag2 = false;
					}
					if (current.ShipmentRecID == 178 || current.ShipmentRecID == 179 || current.ShipmentRecID == 180 || current.ShipmentRecID == 192 || current.ShipmentRecID == 194 || current.ShipmentRecID == 195)
					{
						flag3 = true;
					}
					if (flag2 || flag3)
					{
						CharShipmentRec record = StaticDB.charShipmentDB.GetRecord(current.ShipmentRecID);
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
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
	}

	private void OnDisable()
	{
		Main.instance.CreateShipmentResultAction -= new Action<int>(this.HandleRecruitResult);
		Main.instance.FollowerDataChangedAction -= new Action(this.HandleFollowerDataChanged);
		Main.instance.ShipmentTypesUpdatedAction -= new Action(this.InitList);
		Main.instance.ShipmentItemPushedAction -= new Action<int, MobileClientShipmentItem>(this.HandleShipmentItemPushed);
		Main.instance.OrderHallNavButtonSelectedAction -= new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected);
	}

	public void OnEnable()
	{
		Main.instance.CreateShipmentResultAction += new Action<int>(this.HandleRecruitResult);
		Main.instance.FollowerDataChangedAction += new Action(this.HandleFollowerDataChanged);
		Main.instance.ShipmentTypesUpdatedAction += new Action(this.InitList);
		Main.instance.ShipmentItemPushedAction += new Action<int, MobileClientShipmentItem>(this.HandleShipmentItemPushed);
		Main.instance.OrderHallNavButtonSelectedAction += new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected);
		this.InitList();
	}

	public void PurgeList()
	{
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
	}

	private void Update()
	{
	}
}