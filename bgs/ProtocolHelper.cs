using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace bgs
{
	public static class ProtocolHelper
	{
		public static bnet.protocol.attribute.Attribute CreateAttribute(string name, long val)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetIntValue(val);
			attribute.SetName(name);
			attribute.SetValue(variant);
			return attribute;
		}

		public static bnet.protocol.attribute.Attribute CreateAttribute(string name, bool val)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetBoolValue(val);
			attribute.SetName(name);
			attribute.SetValue(variant);
			return attribute;
		}

		public static bnet.protocol.attribute.Attribute CreateAttribute(string name, string val)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetStringValue(val);
			attribute.SetName(name);
			attribute.SetValue(variant);
			return attribute;
		}

		public static bnet.protocol.attribute.Attribute CreateAttribute(string name, byte[] val)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetBlobValue(val);
			attribute.SetName(name);
			attribute.SetValue(variant);
			return attribute;
		}

		public static bnet.protocol.attribute.Attribute CreateAttribute(string name, ulong val)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetUintValue(val);
			attribute.SetName(name);
			attribute.SetValue(variant);
			return attribute;
		}

		public static EntityId CreateEntityId(ulong high, ulong low)
		{
			EntityId entityId = new EntityId();
			entityId.SetHigh(high);
			entityId.SetLow(low);
			return entityId;
		}

		public static bnet.protocol.attribute.Attribute FindAttribute(List<bnet.protocol.attribute.Attribute> list, string attributeName, Func<bnet.protocol.attribute.Attribute, bool> condition = null)
		{
			if (list == null)
			{
				return null;
			}
			if (condition == null)
			{
				return list.FirstOrDefault<bnet.protocol.attribute.Attribute>((bnet.protocol.attribute.Attribute a) => a.Name == attributeName);
			}
			return list.FirstOrDefault<bnet.protocol.attribute.Attribute>((bnet.protocol.attribute.Attribute a) => (a.Name != attributeName ? false : condition(a)));
		}

		public static ulong GetUintAttribute(List<bnet.protocol.attribute.Attribute> list, string attributeName, ulong defaultValue, Func<bnet.protocol.attribute.Attribute, bool> condition = null)
		{
			bnet.protocol.attribute.Attribute attribute;
			if (list == null)
			{
				return defaultValue;
			}
			attribute = (condition != null ? list.FirstOrDefault<bnet.protocol.attribute.Attribute>((bnet.protocol.attribute.Attribute a) => (!(a.Name == attributeName) || !a.Value.HasUintValue ? false : condition(a))) : list.FirstOrDefault<bnet.protocol.attribute.Attribute>((bnet.protocol.attribute.Attribute a) => (a.Name != attributeName ? false : a.Value.HasUintValue)));
			return (attribute != null ? attribute.Value.UintValue : defaultValue);
		}

		public static bool IsNone(this bnet.protocol.attribute.Variant val)
		{
			return (val.HasBoolValue || val.HasIntValue || val.HasFloatValue || val.HasStringValue || val.HasBlobValue || val.HasMessageValue || val.HasFourccValue || val.HasUintValue ? false : !val.HasEntityidValue);
		}
	}
}