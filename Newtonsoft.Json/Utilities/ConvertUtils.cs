using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	internal static class ConvertUtils
	{
		private readonly static ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>> CastConverters;

		static ConvertUtils()
		{
			ConvertUtils.CastConverters = new ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>>(new Func<ConvertUtils.TypeConvertKey, Func<object, object>>(ConvertUtils.CreateCastConverter));
		}

		public static bool CanConvertType(Type initialType, Type targetType, bool allowTypeNameToString)
		{
			ValidationUtils.ArgumentNotNull(initialType, "initialType");
			ValidationUtils.ArgumentNotNull(targetType, "targetType");
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			if (targetType == initialType)
			{
				return true;
			}
			if (typeof(IConvertible).IsAssignableFrom(initialType) && typeof(IConvertible).IsAssignableFrom(targetType))
			{
				return true;
			}
			if (initialType == typeof(DateTime) && targetType == typeof(DateTimeOffset))
			{
				return true;
			}
			if (initialType == typeof(Guid) && (targetType == typeof(Guid) || targetType == typeof(string)))
			{
				return true;
			}
			if (initialType == typeof(Type) && targetType == typeof(string))
			{
				return true;
			}
			TypeConverter converter = ConvertUtils.GetConverter(initialType);
			if (converter != null && !ConvertUtils.IsComponentConverter(converter) && converter.CanConvertTo(targetType) && (allowTypeNameToString || converter.GetType() != typeof(TypeConverter)))
			{
				return true;
			}
			TypeConverter typeConverter = ConvertUtils.GetConverter(targetType);
			if (typeConverter != null && !ConvertUtils.IsComponentConverter(typeConverter) && typeConverter.CanConvertFrom(initialType))
			{
				return true;
			}
			if (initialType == typeof(DBNull) && ReflectionUtils.IsNullable(targetType))
			{
				return true;
			}
			return false;
		}

		public static T Convert<T>(object initialValue)
		{
			return ConvertUtils.Convert<T>(initialValue, CultureInfo.CurrentCulture);
		}

		public static T Convert<T>(object initialValue, CultureInfo culture)
		{
			return (T)ConvertUtils.Convert(initialValue, culture, typeof(T));
		}

		public static object Convert(object initialValue, CultureInfo culture, Type targetType)
		{
			if (initialValue == null)
			{
				throw new ArgumentNullException("initialValue");
			}
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			Type type = initialValue.GetType();
			if (targetType == type)
			{
				return initialValue;
			}
			if (initialValue is string && typeof(Type).IsAssignableFrom(targetType))
			{
				return Type.GetType((string)initialValue, true);
			}
			if (targetType.IsInterface || targetType.IsGenericTypeDefinition || targetType.IsAbstract)
			{
				throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, new object[] { targetType }), "targetType");
			}
			if (initialValue is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
			{
				if (targetType.IsEnum)
				{
					if (initialValue is string)
					{
						return Enum.Parse(targetType, initialValue.ToString(), true);
					}
					if (ConvertUtils.IsInteger(initialValue))
					{
						return Enum.ToObject(targetType, initialValue);
					}
				}
				return Convert.ChangeType(initialValue, targetType, culture);
			}
			if (initialValue is DateTime && targetType == typeof(DateTimeOffset))
			{
				return new DateTimeOffset((DateTime)initialValue);
			}
			if (initialValue is string)
			{
				if (targetType == typeof(Guid))
				{
					return new Guid((string)initialValue);
				}
				if (targetType == typeof(Uri))
				{
					return new Uri((string)initialValue);
				}
				if (targetType == typeof(TimeSpan))
				{
					return TimeSpan.Parse((string)initialValue);
				}
			}
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && converter.CanConvertTo(targetType))
			{
				return converter.ConvertTo(null, culture, initialValue, targetType);
			}
			TypeConverter typeConverter = ConvertUtils.GetConverter(targetType);
			if (typeConverter != null && typeConverter.CanConvertFrom(type))
			{
				return typeConverter.ConvertFrom(null, culture, initialValue);
			}
			if (initialValue != DBNull.Value)
			{
				throw new Exception("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { type, targetType }));
			}
			if (!ReflectionUtils.IsNullable(targetType))
			{
				throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { type, targetType }));
			}
			return ConvertUtils.EnsureTypeAssignable(null, type, targetType);
		}

		public static T ConvertOrCast<T>(object initialValue)
		{
			return ConvertUtils.ConvertOrCast<T>(initialValue, CultureInfo.CurrentCulture);
		}

		public static T ConvertOrCast<T>(object initialValue, CultureInfo culture)
		{
			return (T)ConvertUtils.ConvertOrCast(initialValue, culture, typeof(T));
		}

		public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
		{
			object obj;
			if (targetType == typeof(object))
			{
				return initialValue;
			}
			if (initialValue == null && ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			if (ConvertUtils.TryConvert(initialValue, culture, targetType, out obj))
			{
				return obj;
			}
			return ConvertUtils.EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
		}

		private static Func<object, object> CreateCastConverter(ConvertUtils.TypeConvertKey t)
		{
			MethodInfo method = t.TargetType.GetMethod("op_Implicit", new Type[] { t.InitialType }) ?? t.TargetType.GetMethod("op_Explicit", new Type[] { t.InitialType });
			if (method == null)
			{
				return null;
			}
			MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => methodCall(null, new object[] { o });
		}

		private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
		{
			Type type;
			if (value == null)
			{
				type = null;
			}
			else
			{
				type = value.GetType();
			}
			Type type1 = type;
			if (value != null)
			{
				if (targetType.IsAssignableFrom(type1))
				{
					return value;
				}
				Func<object, object> func = ConvertUtils.CastConverters.Get(new ConvertUtils.TypeConvertKey(type1, targetType));
				if (func != null)
				{
					return func(value);
				}
			}
			else if (ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			throw new Exception("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { (initialType == null ? "{null}" : initialType.ToString()), targetType }));
		}

		internal static TypeConverter GetConverter(Type t)
		{
			return JsonTypeReflector.GetTypeConverter(t);
		}

		private static bool IsComponentConverter(TypeConverter converter)
		{
			return converter is ComponentConverter;
		}

		public static bool IsInteger(object value)
		{
			switch (Convert.GetTypeCode(value))
			{
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryConvert<T>(object initialValue, out T convertedValue)
		{
			return ConvertUtils.TryConvert<T>(initialValue, CultureInfo.CurrentCulture, out convertedValue);
		}

		public static bool TryConvert<T>(object initialValue, CultureInfo culture, out T convertedValue)
		{
			return MiscellaneousUtils.TryAction<T>(() => {
				object obj;
				ConvertUtils.TryConvert(initialValue, CultureInfo.CurrentCulture, typeof(T), out obj);
				return (T)obj;
			}, out convertedValue);
		}

		public static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object convertedValue)
		{
			return MiscellaneousUtils.TryAction<object>(() => ConvertUtils.Convert(initialValue, culture, targetType), out convertedValue);
		}

		public static bool TryConvertOrCast<T>(object initialValue, out T convertedValue)
		{
			return ConvertUtils.TryConvertOrCast<T>(initialValue, CultureInfo.CurrentCulture, out convertedValue);
		}

		public static bool TryConvertOrCast<T>(object initialValue, CultureInfo culture, out T convertedValue)
		{
			return MiscellaneousUtils.TryAction<T>(() => {
				object obj;
				ConvertUtils.TryConvertOrCast(initialValue, CultureInfo.CurrentCulture, typeof(T), out obj);
				return (T)obj;
			}, out convertedValue);
		}

		public static bool TryConvertOrCast(object initialValue, CultureInfo culture, Type targetType, out object convertedValue)
		{
			return MiscellaneousUtils.TryAction<object>(() => ConvertUtils.ConvertOrCast(initialValue, culture, targetType), out convertedValue);
		}

		internal struct TypeConvertKey : IEquatable<ConvertUtils.TypeConvertKey>
		{
			private readonly Type _initialType;

			private readonly Type _targetType;

			public Type InitialType
			{
				get
				{
					return this._initialType;
				}
			}

			public Type TargetType
			{
				get
				{
					return this._targetType;
				}
			}

			public TypeConvertKey(Type initialType, Type targetType)
			{
				this._initialType = initialType;
				this._targetType = targetType;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is ConvertUtils.TypeConvertKey))
				{
					return false;
				}
				return this.Equals((ConvertUtils.TypeConvertKey)obj);
			}

			public bool Equals(ConvertUtils.TypeConvertKey other)
			{
				return (this._initialType != other._initialType ? false : this._targetType == other._targetType);
			}

			public override int GetHashCode()
			{
				return this._initialType.GetHashCode() ^ this._targetType.GetHashCode();
			}
		}
	}
}