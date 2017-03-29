using bgs;
using System;
using System.Collections.Generic;

namespace bgs.RPCServices
{
	public sealed class ServiceCollectionHelper
	{
		private Dictionary<uint, ServiceDescriptor> importedServices = new Dictionary<uint, ServiceDescriptor>();

		private Dictionary<uint, ServiceDescriptor> exportedServices = new Dictionary<uint, ServiceDescriptor>();

		public Dictionary<uint, ServiceDescriptor> ImportedServices
		{
			get
			{
				return this.importedServices;
			}
		}

		public ServiceCollectionHelper()
		{
		}

		public void AddExportedService(uint serviceId, ServiceDescriptor serviceDescriptor)
		{
			this.exportedServices.Add(serviceId, serviceDescriptor);
		}

		public void AddImportedService(uint serviceId, ServiceDescriptor serviceDescriptor)
		{
			this.importedServices.Add(serviceId, serviceDescriptor);
		}

		public ServiceDescriptor GetExportedServiceById(uint service_id)
		{
			ServiceDescriptor serviceDescriptor;
			this.exportedServices.TryGetValue(service_id, out serviceDescriptor);
			return serviceDescriptor;
		}

		public ServiceDescriptor GetImportedServiceById(uint service_id)
		{
			ServiceDescriptor serviceDescriptor;
			if (this.importedServices == null)
			{
				return null;
			}
			this.importedServices.TryGetValue(service_id, out serviceDescriptor);
			return serviceDescriptor;
		}

		public ServiceDescriptor GetImportedServiceByName(string name)
		{
			ServiceDescriptor value;
			Dictionary<uint, ServiceDescriptor>.Enumerator enumerator = this.importedServices.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, ServiceDescriptor> current = enumerator.Current;
					if (current.Value.Name != name)
					{
						continue;
					}
					value = current.Value;
					return value;
				}
				return null;
			}
			finally
			{
				((IDisposable)(object)enumerator).Dispose();
			}
			return value;
		}
	}
}