using System;
using System.ComponentModel;

namespace JamLib
{
	public class NonStringExpandableObjectConverter : ExpandableObjectConverter
	{
		public NonStringExpandableObjectConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return false;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return false;
			}
			return base.CanConvertTo(context, destinationType);
		}
	}
}