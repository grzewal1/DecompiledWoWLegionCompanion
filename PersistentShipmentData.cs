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
				PersistentShipmentData.s_instance = new PersistentShipmentData();
				PersistentShipmentData.s_instance.m_shipmentDictionary = new Hashtable();
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

	public static void AddOrUpdateShipment(JamCharacterShipment shipment)
	{
		if (PersistentShipmentData.instance.m_shipmentDictionary.ContainsKey(shipment.ShipmentID))
		{
			PersistentShipmentData.instance.m_shipmentDictionary.Remove(shipment.ShipmentID);
		}
		PersistentShipmentData.instance.m_shipmentDictionary.Add(shipment.ShipmentID, shipment);
	}

	public static void SetAvailableShipmentTypes(MobileClientShipmentType[] availableShipmentTypes)
	{
		PersistentShipmentData.instance.m_availableShipmentTypes = availableShipmentTypes;
	}

	public static MobileClientShipmentType[] GetAvailableShipmentTypes()
	{
		return PersistentShipmentData.instance.m_availableShipmentTypes;
	}

	public static bool ShipmentTypeForShipmentIsAvailable(int charShipmentID)
	{
		MobileClientShipmentType[] availableShipmentTypes = PersistentShipmentData.instance.m_availableShipmentTypes;
		for (int i = 0; i < availableShipmentTypes.Length; i++)
		{
			MobileClientShipmentType mobileClientShipmentType = availableShipmentTypes[i];
			if (mobileClientShipmentType.CharShipmentID == charShipmentID)
			{
				return true;
			}
		}
		return false;
	}

	public static void ClearData()
	{
		PersistentShipmentData.instance.m_shipmentDictionary.Clear();
	}

	public static int GetNumReadyShipments()
	{
		int num = 0;
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.get_Values().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamCharacterShipment jamCharacterShipment = (JamCharacterShipment)enumerator.get_Current();
				long num2 = GarrisonStatus.CurrentTime() - (long)jamCharacterShipment.CreationTime;
				long num3 = (long)jamCharacterShipment.ShipmentDuration - num2;
				if (num3 <= 0L)
				{
					num++;
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
		return num;
	}
}
