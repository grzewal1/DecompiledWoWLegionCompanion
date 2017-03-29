using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	internal static class EnumUtils
	{
		public static IList<T> GetFlagsValues<T>(T value)
		where T : struct
		{
			Type type = typeof(T);
			if (!type.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new Exception("Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
			}
			Type underlyingType = Enum.GetUnderlyingType(value.GetType());
			ulong num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			EnumValues<ulong> namesAndValues = EnumUtils.GetNamesAndValues<T>();
			IList<T> ts = new List<T>();
			IEnumerator<EnumValue<ulong>> enumerator = namesAndValues.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					EnumValue<ulong> current = enumerator.Current;
					if ((num & current.Value) != current.Value || current.Value == 0)
					{
						continue;
					}
					ts.Add((T)Convert.ChangeType(current.Value, underlyingType, CultureInfo.CurrentCulture));
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			if (ts.Count == 0 && namesAndValues.SingleOrDefault<EnumValue<ulong>>((EnumValue<ulong> v) => v.Value == (long)0) != null)
			{
				ts.Add(default(T));
			}
			return ts;
		}

		public static TEnumType GetMaximumValue<TEnumType>(Type enumType)
		where TEnumType : IConvertible, IComparable<TEnumType>
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			Type underlyingType = Enum.GetUnderlyingType(enumType);
			if (!typeof(TEnumType).IsAssignableFrom(underlyingType))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "TEnumType is not assignable from the enum's underlying type of {0}.", new object[] { underlyingType.Name }));
			}
			ulong num = (ulong)0;
			IList<object> values = EnumUtils.GetValues(enumType);
			if (!enumType.IsDefined(typeof(FlagsAttribute), false))
			{
				IEnumerator<object> enumerator = values.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ulong num1 = ((TEnumType)enumerator.Current).ToUInt64(CultureInfo.InvariantCulture);
						if (num.CompareTo(num1) != -1)
						{
							continue;
						}
						num = num1;
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					enumerator.Dispose();
				}
			}
			else
			{
				IEnumerator<object> enumerator1 = values.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						TEnumType current = (TEnumType)enumerator1.Current;
						num = num | current.ToUInt64(CultureInfo.InvariantCulture);
					}
				}
				finally
				{
					if (enumerator1 == null)
					{
					}
					enumerator1.Dispose();
				}
			}
			return (TEnumType)Convert.ChangeType(num, typeof(TEnumType), CultureInfo.InvariantCulture);
		}

		public static IList<string> GetNames<T>()
		{
			return EnumUtils.GetNames(typeof(T));
		}

		public static IList<string> GetNames(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException(string.Concat("Type '", enumType.Name, "' is not an enum."));
			}
			List<string> strs = new List<string>();
			IEnumerable<FieldInfo> fields = 
				from field in (IEnumerable<FieldInfo>)enumType.GetFields()
				where field.IsLiteral
				select field;
			IEnumerator<FieldInfo> enumerator = fields.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					strs.Add(enumerator.Current.Name);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return strs;
		}

		public static EnumValues<ulong> GetNamesAndValues<T>()
		where T : struct
		{
			return EnumUtils.GetNamesAndValues<ulong>(typeof(T));
		}

		public static EnumValues<TUnderlyingType> GetNamesAndValues<TEnum, TUnderlyingType>()
		where TEnum : struct
		where TUnderlyingType : struct
		{
			return EnumUtils.GetNamesAndValues<TUnderlyingType>(typeof(TEnum));
		}

		public static EnumValues<TUnderlyingType> GetNamesAndValues<TUnderlyingType>(Type enumType)
		where TUnderlyingType : struct
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			ValidationUtils.ArgumentTypeIsEnum(enumType, "enumType");
			IList<object> values = EnumUtils.GetValues(enumType);
			IList<string> names = EnumUtils.GetNames(enumType);
			EnumValues<TUnderlyingType> enumValue = new EnumValues<TUnderlyingType>();
			for (int i = 0; i < values.Count; i++)
			{
				try
				{
					enumValue.Add(new EnumValue<TUnderlyingType>(names[i], (TUnderlyingType)Convert.ChangeType(values[i], typeof(TUnderlyingType), CultureInfo.CurrentCulture)));
				}
				catch (OverflowException overflowException1)
				{
					OverflowException overflowException = overflowException1;
					throw new Exception(string.Format(CultureInfo.InvariantCulture, "Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}", new object[] { Enum.GetUnderlyingType(enumType), typeof(TUnderlyingType), Convert.ToUInt64(values[i], CultureInfo.InvariantCulture) }), overflowException);
				}
			}
			return enumValue;
		}

		public static IList<T> GetValues<T>()
		{
			return EnumUtils.GetValues(typeof(T)).Cast<T>().ToList<T>();
		}

		public static IList<object> GetValues(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException(string.Concat("Type '", enumType.Name, "' is not an enum."));
			}
			List<object> objs = new List<object>();
			IEnumerable<FieldInfo> fields = 
				from field in (IEnumerable<FieldInfo>)enumType.GetFields()
				where field.IsLiteral
				select field;
			IEnumerator<FieldInfo> enumerator = fields.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					objs.Add(enumerator.Current.GetValue(enumType));
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return objs;
		}

		public static T Parse<T>(string enumMemberName)
		where T : struct
		{
			return EnumUtils.Parse<T>(enumMemberName, false);
		}

		public static T Parse<T>(string enumMemberName, bool ignoreCase)
		where T : struct
		{
			ValidationUtils.ArgumentTypeIsEnum(typeof(T), "T");
			return (T)Enum.Parse(typeof(T), enumMemberName, ignoreCase);
		}

		public static bool TryParse<T>(string enumMemberName, bool ignoreCase, out T value)
		where T : struct
		{
			ValidationUtils.ArgumentTypeIsEnum(typeof(T), "T");
			return MiscellaneousUtils.TryAction<T>(() => EnumUtils.Parse<T>(enumMemberName, ignoreCase), out value);
		}
	}
}