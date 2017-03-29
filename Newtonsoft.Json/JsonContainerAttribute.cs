using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false)]
	public abstract class JsonContainerAttribute : Attribute
	{
		internal bool? _isReference;

		public string Description
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public bool IsReference
		{
			get
			{
				bool? nullable = this._isReference;
				return (!nullable.HasValue ? false : nullable.Value);
			}
			set
			{
				this._isReference = new bool?(value);
			}
		}

		public string Title
		{
			get;
			set;
		}

		protected JsonContainerAttribute()
		{
		}

		protected JsonContainerAttribute(string id)
		{
			this.Id = id;
		}
	}
}