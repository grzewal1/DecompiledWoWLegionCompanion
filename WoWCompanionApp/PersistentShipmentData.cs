using System;
using System.Collections.Generic;
using System.Linq;

namespace WoWCompanionApp
{
	public class PersistentShipmentData
	{
		private static PersistentShipmentData s_instance;

		private Dictionary<ulong, WrapperCharacterShipment> m_shipmentDictionary = new Dictionary<ulong, WrapperCharacterShipment>();

		private List<WrapperShipmentType> m_availableShipmentTypes;

		private static PersistentShipmentData instance
		{
			get
			{
				if (PersistentShipmentData.s_instance == null)
				{
					PersistentShipmentData.s_instance = new PersistentShipmentData();
				}
				return PersistentShipmentData.s_instance;
			}
		}

		public static IDictionary<ulong, WrapperCharacterShipment> shipmentDictionary
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

		public static void AddOrUpdateShipment(WrapperCharacterShipment shipment)
		{
			if (PersistentShipmentData.instance.m_shipmentDictionary.ContainsKey(shipment.ShipmentID))
			{
				PersistentShipmentData.instance.m_shipmentDictionary.Remove(shipment.ShipmentID);
			}
			PersistentShipmentData.instance.m_shipmentDictionary.Add(shipment.ShipmentID, shipment);
		}

		public static bool CanOrderShipmentType(int charShipmentID)
		{
			bool canOrder;
			List<WrapperShipmentType>.Enumerator enumerator = PersistentShipmentData.instance.m_availableShipmentTypes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WrapperShipmentType current = enumerator.Current;
					if (current.CharShipmentID != charShipmentID)
					{
						continue;
					}
					canOrder = current.CanOrder;
					return canOrder;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return canOrder;
		}

		public static bool CanPickupShipmentType(int charShipmentID)
		{
			bool canPickup;
			List<WrapperShipmentType>.Enumerator enumerator = PersistentShipmentData.instance.m_availableShipmentTypes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WrapperShipmentType current = enumerator.Current;
					if (current.CharShipmentID != charShipmentID)
					{
						continue;
					}
					canPickup = current.CanPickup;
					return canPickup;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return canPickup;
		}

		public static void ClearData()
		{
			PersistentShipmentData.instance.m_shipmentDictionary.Clear();
		}

		public static List<WrapperShipmentType> GetAvailableShipmentTypes()
		{
			return PersistentShipmentData.instance.m_availableShipmentTypes;
		}

		public static int GetNumReadyShipments()
		{
			int num = 0;
			foreach (WrapperCharacterShipment value in PersistentShipmentData.shipmentDictionary.Values)
			{
				if (PersistentShipmentData.ShipmentTypeForShipmentIsAvailable(value.ShipmentRecID))
				{
					TimeSpan timeSpan = GarrisonStatus.CurrentTime() - value.CreationTime;
					if ((value.ShipmentDuration - timeSpan).TotalSeconds > 0)
					{
						continue;
					}
					num++;
				}
			}
			return num;
		}

		public static void SetAvailableShipmentTypes(IEnumerable<WrapperShipmentType> availableShipmentTypes)
		{
			PersistentShipmentData.instance.m_availableShipmentTypes = availableShipmentTypes.ToList<WrapperShipmentType>();
		}

		public static bool ShipmentTypeForShipmentIsAvailable(int charShipmentID)
		{
			bool flag;
			List<WrapperShipmentType>.Enumerator enumerator = PersistentShipmentData.instance.m_availableShipmentTypes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CharShipmentID != charShipmentID)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}
	}
}