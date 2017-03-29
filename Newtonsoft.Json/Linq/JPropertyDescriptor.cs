using Newtonsoft.Json.Utilities;
using System;
using System.ComponentModel;

namespace Newtonsoft.Json.Linq
{
	public class JPropertyDescriptor : PropertyDescriptor
	{
		private readonly Type _propertyType;

		public override Type ComponentType
		{
			get
			{
				return typeof(JObject);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		protected override int NameHashCode
		{
			get
			{
				return base.NameHashCode;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this._propertyType;
			}
		}

		public JPropertyDescriptor(string name, Type propertyType) : base(name, null)
		{
			ValidationUtils.ArgumentNotNull(name, "name");
			ValidationUtils.ArgumentNotNull(propertyType, "propertyType");
			this._propertyType = propertyType;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		private static JObject CastInstance(object instance)
		{
			return (JObject)instance;
		}

		public override object GetValue(object component)
		{
			return JPropertyDescriptor.CastInstance(component)[this.Name];
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			JToken jValue;
			if (!(value is JToken))
			{
				jValue = new JValue(value);
			}
			else
			{
				jValue = (JToken)value;
			}
			JPropertyDescriptor.CastInstance(component)[this.Name] = jValue;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}