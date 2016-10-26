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

	private void Awake()
	{
		this.m_noRecruitsYetMessage.set_font(GeneralHelpers.LoadStandardFont());
		this.m_noRecruitsYetMessage.set_text(StaticDB.GetString("NO_RECRUITS_AVAILABLE_YET", "You have no recruits available yet."));
		this.InitList();
	}

	public void OnEnable()
	{
		Main expr_05 = Main.instance;
		expr_05.CreateShipmentResultAction = (Action<int>)Delegate.Combine(expr_05.CreateShipmentResultAction, new Action<int>(this.HandleRecruitResult));
		Main expr_2B = Main.instance;
		expr_2B.FollowerDataChangedAction = (Action)Delegate.Combine(expr_2B.FollowerDataChangedAction, new Action(this.HandleFollowerDataChanged));
		Main expr_51 = Main.instance;
		expr_51.ShipmentTypesUpdatedAction = (Action)Delegate.Combine(expr_51.ShipmentTypesUpdatedAction, new Action(this.InitList));
		Main expr_77 = Main.instance;
		expr_77.ShipmentItemPushedAction = (Action<int, MobileClientShipmentItem>)Delegate.Combine(expr_77.ShipmentItemPushedAction, new Action<int, MobileClientShipmentItem>(this.HandleShipmentItemPushed));
		Main expr_9D = Main.instance;
		expr_9D.OrderHallNavButtonSelectedAction = (Action<OrderHallNavButton>)Delegate.Combine(expr_9D.OrderHallNavButtonSelectedAction, new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected));
		this.InitList();
	}

	private void OnDisable()
	{
		Main expr_05 = Main.instance;
		expr_05.CreateShipmentResultAction = (Action<int>)Delegate.Remove(expr_05.CreateShipmentResultAction, new Action<int>(this.HandleRecruitResult));
		Main expr_2B = Main.instance;
		expr_2B.FollowerDataChangedAction = (Action)Delegate.Remove(expr_2B.FollowerDataChangedAction, new Action(this.HandleFollowerDataChanged));
		Main expr_51 = Main.instance;
		expr_51.ShipmentTypesUpdatedAction = (Action)Delegate.Remove(expr_51.ShipmentTypesUpdatedAction, new Action(this.InitList));
		Main expr_77 = Main.instance;
		expr_77.ShipmentItemPushedAction = (Action<int, MobileClientShipmentItem>)Delegate.Remove(expr_77.ShipmentItemPushedAction, new Action<int, MobileClientShipmentItem>(this.HandleShipmentItemPushed));
		Main expr_9D = Main.instance;
		expr_9D.OrderHallNavButtonSelectedAction = (Action<OrderHallNavButton>)Delegate.Remove(expr_9D.OrderHallNavButtonSelectedAction, new Action<OrderHallNavButton>(this.HandleOrderHallNavButtonSelected));
	}

	public void HandleOrderHallNavButtonSelected(OrderHallNavButton navButton)
	{
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		TroopsListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TroopsListItem troopsListItem = array[i];
			troopsListItem.ClearAndHideLootArea();
		}
	}

	private void Update()
	{
	}

	private void HandleFollowerDataChanged()
	{
		this.InitList();
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		TroopsListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TroopsListItem troopsListItem = array[i];
			troopsListItem.HandleFollowerDataChanged();
		}
	}

	private void HandleEnteredWorld()
	{
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		TroopsListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TroopsListItem troopsListItem = array[i];
			Object.DestroyImmediate(troopsListItem.get_gameObject());
		}
	}

	private void HandleShipmentItemPushed(int charShipmentID, MobileClientShipmentItem item)
	{
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		TroopsListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TroopsListItem troopsListItem = array[i];
			if (troopsListItem.GetCharShipmentTypeID() == charShipmentID)
			{
				troopsListItem.HandleShipmentItemPushed(item);
			}
		}
	}

	private void InitList()
	{
		MobileClientShipmentType[] availableShipmentTypes = PersistentShipmentData.GetAvailableShipmentTypes();
		if (availableShipmentTypes == null || availableShipmentTypes.Length == 0)
		{
			this.m_noRecruitsYetMessage.get_gameObject().SetActive(true);
		}
		else
		{
			this.m_noRecruitsYetMessage.get_gameObject().SetActive(false);
		}
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		TroopsListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TroopsListItem troopsListItem = array[i];
			bool flag = true;
			if (availableShipmentTypes != null)
			{
				MobileClientShipmentType[] array2 = availableShipmentTypes;
				for (int j = 0; j < array2.Length; j++)
				{
					MobileClientShipmentType mobileClientShipmentType = array2[j];
					if (troopsListItem.GetCharShipmentTypeID() == mobileClientShipmentType.CharShipmentID)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Object.DestroyImmediate(troopsListItem.get_gameObject());
			}
		}
		if (availableShipmentTypes == null)
		{
			return;
		}
		componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		for (int k = 0; k < availableShipmentTypes.Length; k++)
		{
			bool flag2 = false;
			TroopsListItem[] array3 = componentsInChildren;
			for (int l = 0; l < array3.Length; l++)
			{
				TroopsListItem troopsListItem2 = array3[l];
				if (troopsListItem2.GetCharShipmentTypeID() == availableShipmentTypes[k].CharShipmentID)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.m_troopsListItemPrefab);
				gameObject.get_transform().SetParent(this.m_troopsListContents.get_transform(), false);
				TroopsListItem component = gameObject.GetComponent<TroopsListItem>();
				component.SetCharShipment(availableShipmentTypes[k], false, null);
				FancyEntrance component2 = component.GetComponent<FancyEntrance>();
				component2.m_timeToDelayEntrance = this.m_listItemInitialEntranceDelay + this.m_listItemEntranceDelay * (float)k;
				component2.Activate();
			}
		}
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.get_Values().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamCharacterShipment jamCharacterShipment = (JamCharacterShipment)enumerator.get_Current();
				if (!PersistentShipmentData.ShipmentTypeForShipmentIsAvailable(jamCharacterShipment.ShipmentRecID))
				{
					CharShipmentRec record = StaticDB.charShipmentDB.GetRecord(jamCharacterShipment.ShipmentRecID);
					if (record != null)
					{
						GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_troopsListItemPrefab);
						gameObject2.get_transform().SetParent(this.m_troopsListContents.get_transform(), false);
						TroopsListItem component3 = gameObject2.GetComponent<TroopsListItem>();
						component3.SetCharShipment(null, true, record);
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

	private void HandleRecruitResult(int result)
	{
		if (result == 0)
		{
			MobilePlayerRequestShipments obj = new MobilePlayerRequestShipments();
			Login.instance.SendToMobileServer(obj);
		}
	}

	public void PurgeList()
	{
		TroopsListItem[] componentsInChildren = this.m_troopsListContents.GetComponentsInChildren<TroopsListItem>(true);
		TroopsListItem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TroopsListItem troopsListItem = array[i];
			Object.DestroyImmediate(troopsListItem.get_gameObject());
		}
	}
}
