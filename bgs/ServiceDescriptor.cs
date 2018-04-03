using bnet.protocol;
using System;

namespace bgs
{
	public class ServiceDescriptor
	{
		private string name;

		private uint id;

		private uint hash;

		protected MethodDescriptor[] Methods;

		private const uint INVALID_SERVICE_ID = 255;

		public uint Hash
		{
			get
			{
				return this.hash;
			}
		}

		public uint Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ServiceDescriptor(string n)
		{
			this.name = n;
			this.id = 255;
			this.hash = Compute32.Hash(this.name);
			Console.WriteLine(string.Concat(new object[] { "service: ", n, ", hash: ", this.hash }));
		}

		public int GetMethodCount()
		{
			if (this.Methods == null)
			{
				return 0;
			}
			return (int)this.Methods.Length;
		}

		public MethodDescriptor GetMethodDescriptor(uint method_id)
		{
			unsafe
			{
				if (this.Methods == null)
				{
					return null;
				}
				if ((ulong)method_id >= (long)((int)this.Methods.Length))
				{
					return null;
				}
				return this.Methods[method_id];
			}
		}

		public MethodDescriptor GetMethodDescriptorByName(string name)
		{
			if (this.Methods == null)
			{
				return null;
			}
			MethodDescriptor[] methods = this.Methods;
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodDescriptor methodDescriptor = methods[i];
				if (methodDescriptor != null && methodDescriptor.Name == name)
				{
					return methodDescriptor;
				}
			}
			return null;
		}

		public string GetMethodName(uint method_id)
		{
			unsafe
			{
				if (this.Methods == null || method_id <= 0 || (ulong)method_id > (long)((int)this.Methods.Length))
				{
					return string.Empty;
				}
				return this.Methods[method_id].Name;
			}
		}

		public MethodDescriptor.ParseMethod GetParser(uint method_id)
		{
			unsafe
			{
				if (this.Methods == null)
				{
					BattleNet.Log.LogWarning("ServiceDescriptor unable to get parser, no methods have been set.");
					return null;
				}
				if (method_id <= 0)
				{
					BattleNet.Log.LogWarning("ServiceDescriptor unable to get parser, invalid index={0}/{1}", new object[] { method_id, (int)this.Methods.Length });
					return null;
				}
				if ((ulong)method_id >= (long)((int)this.Methods.Length))
				{
					BattleNet.Log.LogWarning("ServiceDescriptor unable to get parser, invalid index={0}/{1}", new object[] { method_id, (int)this.Methods.Length });
					return null;
				}
				if (this.Methods[method_id].Parser != null)
				{
					return this.Methods[method_id].Parser;
				}
				BattleNet.Log.LogWarning("ServiceDescriptor unable to get parser, invalid index={0}/{1}", new object[] { method_id, (int)this.Methods.Length });
				return null;
			}
		}

		public bool HasMethodListener(uint method_id)
		{
			unsafe
			{
				if (this.Methods == null || method_id <= 0 || (ulong)method_id > (long)((int)this.Methods.Length))
				{
					return false;
				}
				return this.Methods[method_id].HasListener();
			}
		}

		public void NotifyMethodListener(RPCContext context)
		{
			unsafe
			{
				if (this.Methods != null && context.Header.MethodId > 0 && (ulong)context.Header.MethodId <= (long)((int)this.Methods.Length))
				{
					this.Methods[context.Header.MethodId].NotifyListener(context);
				}
			}
		}

		public void RegisterMethodListener(uint method_id, RPCContextDelegate callback)
		{
			unsafe
			{
				if (this.Methods != null && method_id > 0 && (ulong)method_id <= (long)((int)this.Methods.Length))
				{
					this.Methods[method_id].RegisterListener(callback);
				}
			}
		}
	}
}