using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited=false, AllowMultiple=false)]
	public sealed class DataContractAttribute : Attribute
	{
		private string name;

		private string ns;

		public bool IsReference
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
		}

		public DataContractAttribute()
		{
		}
	}
}