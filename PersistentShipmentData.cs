using System;
using System.Collections;
using WowJamMessages;
using WowJamMessages.MobileClientJSON;

public class PersistentShipmentData
{
	private static PersistentShipmentData s_instance;

	private Hashtable m_shipmentDictionary;

	private MobileClientShipmentType[] m_availableShipmentTypes;

	private static PersistentShipmentData instance
	{
		get
		{
			if (PersistentShipmentData.s_instance == null)
			{
				PersistentShipmentData.s_instance = new PersistentShipmentData()
				{
					m_shipmentDictionary = new Hashtable()
				};
			}
			return PersistentShipmentData.s_instance;
		}
	}

	public static Hashtable shipmentDictionary
	{
		get
		{
			return PersistentShipmentData.instance.m_shipmentDictionary;
		}
	}

	static PersistentShipmentData()
	{
	}

	public PersistentShipmentData()
	{
	}

	public static void AddOrUpdateShipment(JamCharacterShipment shipment)
	{
		if (PersistentShipmentData.instance.m_shipmentDictionary.ContainsKey(shipment.ShipmentID))
		{
			PersistentShipmentData.instance.m_shipmentDictionary.Remove(shipment.ShipmentID);
		}
		PersistentShipmentData.instance.m_shipmentDictionary.Add(shipment.ShipmentID, shipment);
	}

	public static bool CanOrderShipmentType(int charShipmentID)
	{
		MobileClientShipmentType[] mAvailableShipmentTypes = PersistentShipmentData.instance.m_availableShipmentTypes;
		for (int i = 0; i < (int)mAvailableShipmentTypes.Length; i++)
		{
			MobileClientShipmentType mobileClientShipmentType = mAvailableShipmentTypes[i];
			if (mobileClientShipmentType.CharShipmentID == charShipmentID)
			{
				return mobileClientShipmentType.CanOrder;
			}
		}
		return false;
	}

	public static bool CanPickupShipmentType(int charShipmentID)
	{
		MobileClientShipmentType[] mAvailableShipmentTypes = PersistentShipmentData.instance.m_availableShipmentTypes;
		for (int i = 0; i < (int)mAvailableShipmentTypes.Length; i++)
		{
			MobileClientShipmentType mobileClientShipmentType = mAvailableShipmentTypes[i];
			if (mobileClientShipmentType.CharShipmentID == charShipmentID)
			{
				return mobileClientShipmentType.CanPickup;
			}
		}
		return false;
	}

	public static void ClearData()
	{
		PersistentShipmentData.instance.m_shipmentDictionary.Clear();
	}

	public static MobileClientShipmentType[] GetAvailableShipmentTypes()
	{
		return PersistentShipmentData.instance.m_availableShipmentTypes;
	}

	public static int GetNumReadyShipments()
	{
		int num = 0;
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamCharacterShipment current = (JamCharacterShipment)enumerator.Current;
				if (PersistentShipmentData.ShipmentTypeForShipmentIsAvailable(current.ShipmentRecID))
				{
					long num1 = GarrisonStatus.CurrentTime() - (long)current.CreationTime;
					if ((long)current.ShipmentDuration - num1 > (long)0)
					{
						continue;
					}
					num++;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			IDisposable disposable1 = disposable;
			if (disposable != null)
			{
				disposable1.Dispose();
			}
		}
		return num;
	}

	public static void SetAvailableShipmentTypes(MobileClientShipmentType[] availableShipmentTypes)
	{
		PersistentShipmentData.instance.m_availableShipmentTypes = availableShipmentTypes;
	}

	public static bool ShipmentTypeForShipmentIsAvailable(int charShipmentID)
	{
		MobileClientShipmentType[] mAvailableShipmentTypes = PersistentShipmentData.instance.m_availableShipmentTypes;
		for (int i = 0; i < (int)mAvailableShipmentTypes.Length; i++)
		{
			if (mAvailableShipmentTypes[i].CharShipmentID == charShipmentID)
			{
				return true;
			}
		}
		return false;
	}
}