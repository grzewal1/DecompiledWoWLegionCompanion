using System;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple=false)]
	public sealed class JsonObjectAttribute : JsonContainerAttribute
	{
		private Newtonsoft.Json.MemberSerialization _memberSerialization;

		public Newtonsoft.Json.MemberSerialization MemberSerialization
		{
			get
			{
				return this._memberSerialization;
			}
			set
			{
				this._memberSerialization = value;
			}
		}

		public JsonObjectAttribute()
		{
		}

		public JsonObjectAttribute(Newtonsoft.Json.MemberSerialization memberSerialization)
		{
			this.MemberSerialization = memberSerialization;
		}

		public JsonObjectAttribute(string id) : base(id)
		{
		}
	}
}