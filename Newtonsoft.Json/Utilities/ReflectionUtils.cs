using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace Newtonsoft.Json.Utilities
{
	internal static class ReflectionUtils
	{
		public static bool AssignableToTypeName(this Type type, string fullTypeName, out Type match)
		{
			for (Type i = type; i != null; i = i.BaseType)
			{
				if (string.Equals(i.FullName, fullTypeName, StringComparison.Ordinal))
				{
					match = i;
					return true;
				}
			}
			Type[] interfaces = type.GetInterfaces();
			for (int j = 0; j < (int)interfaces.Length; j++)
			{
				if (string.Equals(interfaces[j].Name, fullTypeName, StringComparison.Ordinal))
				{
					match = type;
					return true;
				}
			}
			match = null;
			return false;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName)
		{
			Type type1;
			return type.AssignableToTypeName(fullTypeName, out type1);
		}

		public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)member;
				if (nonPublic)
				{
					return true;
				}
				if (fieldInfo.IsPublic)
				{
					return true;
				}
				return false;
			}
			if (memberType != MemberTypes.Property)
			{
				return false;
			}
			PropertyInfo propertyInfo = (PropertyInfo)member;
			if (!propertyInfo.CanRead)
			{
				return false;
			}
			if (nonPublic)
			{
				return true;
			}
			return propertyInfo.GetGetMethod(nonPublic) != null;
		}

		public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)member;
				if (fieldInfo.IsInitOnly && !canSetReadOnly)
				{
					return false;
				}
				if (nonPublic)
				{
					return true;
				}
				if (fieldInfo.IsPublic)
				{
					return true;
				}
				return false;
			}
			if (memberType != MemberTypes.Property)
			{
				return false;
			}
			PropertyInfo propertyInfo = (PropertyInfo)member;
			if (!propertyInfo.CanWrite)
			{
				return false;
			}
			if (nonPublic)
			{
				return true;
			}
			return propertyInfo.GetSetMethod(nonPublic) != null;
		}

		public static object CreateGeneric(Type genericTypeDefinition, Type innerType, params object[] args)
		{
			return ReflectionUtils.CreateGeneric(genericTypeDefinition, (IList<Type>)(new Type[] { innerType }), args);
		}

		public static object CreateGeneric(Type genericTypeDefinition, IList<Type> innerTypes, params object[] args)
		{
			return ReflectionUtils.CreateGeneric(genericTypeDefinition, innerTypes, (Type t, IList<object> a) => ReflectionUtils.CreateInstance(t, a.ToArray<object>()), args);
		}

		public static object CreateGeneric(Type genericTypeDefinition, IList<Type> innerTypes, Func<Type, IList<object>, object> instanceCreator, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(genericTypeDefinition, "genericTypeDefinition");
			ValidationUtils.ArgumentNotNullOrEmpty<Type>(innerTypes, "innerTypes");
			ValidationUtils.ArgumentNotNull(instanceCreator, "createInstance");
			Type type = ReflectionUtils.MakeGenericType(genericTypeDefinition, innerTypes.ToArray<Type>());
			return instanceCreator(type, args);
		}

		public static object CreateInstance(Type type, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			return Activator.CreateInstance(type, args);
		}

		public static object CreateUnitializedValue(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Type {0} is a generic type definition and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }), "type");
			}
			if (type.IsClass || type.IsInterface || type == typeof(void))
			{
				return null;
			}
			if (!type.IsValueType)
			{
				throw new ArgumentException("Type {0} cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }), "type");
			}
			return Activator.CreateInstance(type);
		}

		public static Type EnsureNotNullableType(Type t)
		{
			return (!ReflectionUtils.IsNullableType(t) ? t : Nullable.GetUnderlyingType(t));
		}

		private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName)
		{
			int num = 0;
			for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
			{
				char chr = fullyQualifiedTypeName[i];
				switch (chr)
				{
					case '[':
					{
						num++;
						break;
					}
					case ']':
					{
						num--;
						break;
					}
					default:
					{
						if (chr == ',')
						{
							if (num == 0)
							{
								return new int?(i);
							}
							break;
						}
						else
						{
							break;
						}
					}
				}
			}
			return null;
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider)
		where T : Attribute
		{
			return ReflectionUtils.GetAttribute<T>(attributeProvider, true);
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider, bool inherit)
		where T : Attribute
		{
			return CollectionUtils.GetSingleItem<T>(ReflectionUtils.GetAttributes<T>(attributeProvider, inherit), true);
		}

		public static T[] GetAttributes<T>(ICustomAttributeProvider attributeProvider, bool inherit)
		where T : Attribute
		{
			ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");
			if (attributeProvider is Type)
			{
				return (T[])((Type)attributeProvider).GetCustomAttributes(typeof(T), inherit);
			}
			if (attributeProvider is Assembly)
			{
				return (T[])Attribute.GetCustomAttributes((Assembly)attributeProvider, typeof(T), inherit);
			}
			if (attributeProvider is MemberInfo)
			{
				return (T[])Attribute.GetCustomAttributes((MemberInfo)attributeProvider, typeof(T), inherit);
			}
			if (attributeProvider is Module)
			{
				return (T[])Attribute.GetCustomAttributes((Module)attributeProvider, typeof(T), inherit);
			}
			if (!(attributeProvider is ParameterInfo))
			{
				return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
			}
			return (T[])Attribute.GetCustomAttributes((ParameterInfo)attributeProvider, typeof(T), inherit);
		}

		private static void GetChildPrivateFields(IList<MemberInfo> initialFields, Type targetType, BindingFlags bindingAttr)
		{
			if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				BindingFlags bindingFlag = bindingAttr.RemoveFlag(BindingFlags.Public);
				while (true)
				{
					Type baseType = targetType.BaseType;
					targetType = baseType;
					if (baseType == null)
					{
						break;
					}
					IEnumerable<MemberInfo> memberInfos = (
						from f in (IEnumerable<FieldInfo>)targetType.GetFields(bindingFlag)
						where f.IsPrivate
						select f).Cast<MemberInfo>();
					initialFields.AddRange<MemberInfo>(memberInfos);
				}
			}
		}

		private static void GetChildPrivateProperties(IList<PropertyInfo> initialProperties, Type targetType, BindingFlags bindingAttr)
		{
			if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				BindingFlags bindingFlag = bindingAttr.RemoveFlag(BindingFlags.Public);
			Label1:
				Type baseType = targetType.BaseType;
				targetType = baseType;
				if (baseType == null)
				{
					return;
				}
				PropertyInfo[] properties = targetType.GetProperties(bindingFlag);
				for (int i = 0; i < (int)properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					int num = initialProperties.IndexOf<PropertyInfo>((PropertyInfo p) => p.Name == propertyInfo.Name);
					if (num != -1)
					{
						initialProperties[num] = propertyInfo;
					}
					else
					{
						initialProperties.Add(propertyInfo);
					}
				}
				goto Label1;
			}
		}

		public static Type GetCollectionItemType(Type type)
		{
			Type type1;
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsArray)
			{
				return type.GetElementType();
			}
			if (!ReflectionUtils.ImplementsGenericDefinition(type, typeof(IEnumerable<>), out type1))
			{
				if (!typeof(IEnumerable).IsAssignableFrom(type))
				{
					throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
				}
				return null;
			}
			if (type1.IsGenericTypeDefinition)
			{
				throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
			}
			return type1.GetGenericArguments()[0];
		}

		public static ConstructorInfo GetDefaultConstructor(Type t)
		{
			return ReflectionUtils.GetDefaultConstructor(t, false);
		}

		public static ConstructorInfo GetDefaultConstructor(Type t, bool nonPublic)
		{
			BindingFlags bindingFlag = BindingFlags.Public;
			if (nonPublic)
			{
				bindingFlag = bindingFlag | BindingFlags.NonPublic;
			}
			return t.GetConstructor(bindingFlag | BindingFlags.Instance, null, new Type[0], null);
		}

		public static Type GetDictionaryKeyType(Type dictionaryType)
		{
			Type type;
			Type type1;
			ReflectionUtils.GetDictionaryKeyValueTypes(dictionaryType, out type, out type1);
			return type;
		}

		public static void GetDictionaryKeyValueTypes(Type dictionaryType, out Type keyType, out Type valueType)
		{
			Type type;
			ValidationUtils.ArgumentNotNull(dictionaryType, "type");
			if (!ReflectionUtils.ImplementsGenericDefinition(dictionaryType, typeof(IDictionary<,>), out type))
			{
				if (!typeof(IDictionary).IsAssignableFrom(dictionaryType))
				{
					throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, new object[] { dictionaryType }));
				}
				keyType = null;
				valueType = null;
				return;
			}
			if (type.IsGenericTypeDefinition)
			{
				throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, new object[] { dictionaryType }));
			}
			Type[] genericArguments = type.GetGenericArguments();
			keyType = genericArguments[0];
			valueType = genericArguments[1];
		}

		public static Type GetDictionaryValueType(Type dictionaryType)
		{
			Type type;
			Type type1;
			ReflectionUtils.GetDictionaryKeyValueTypes(dictionaryType, out type, out type1);
			return type1;
		}

		public static IEnumerable<FieldInfo> GetFields(Type targetType, BindingFlags bindingAttr)
		{
			ValidationUtils.ArgumentNotNull(targetType, "targetType");
			List<MemberInfo> memberInfos = new List<MemberInfo>(targetType.GetFields(bindingAttr));
			ReflectionUtils.GetChildPrivateFields(memberInfos, targetType, bindingAttr);
			return memberInfos.Cast<FieldInfo>();
		}

		public static List<MemberInfo> GetFieldsAndProperties<T>(BindingFlags bindingAttr)
		{
			return ReflectionUtils.GetFieldsAndProperties(typeof(T), bindingAttr);
		}

		public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
		{
			List<MemberInfo> memberInfos = new List<MemberInfo>();
			memberInfos.AddRange(ReflectionUtils.GetFields(type, bindingAttr));
			memberInfos.AddRange(ReflectionUtils.GetProperties(type, bindingAttr));
			List<MemberInfo> memberInfos1 = new List<MemberInfo>(memberInfos.Count);
			var name = 
				from m in memberInfos
				group m by m.Name into g
				select new { Count = g.Count<MemberInfo>(), Members = g.Cast<MemberInfo>() };
			var enumerator = name.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					if (current.Count != 1)
					{
						IEnumerable<MemberInfo> members = 
							from m in current.Members
							where (!ReflectionUtils.IsOverridenGenericMember(m, bindingAttr) ? true : m.Name == "Item")
							select m;
						memberInfos1.AddRange(members);
					}
					else
					{
						memberInfos1.Add(current.Members.First<MemberInfo>());
					}
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return memberInfos1;
		}

		public static MemberInfo GetMemberInfoFromType(Type targetType, MemberInfo memberInfo)
		{
			BindingFlags bindingFlag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			if (memberInfo.MemberType != MemberTypes.Property)
			{
				return targetType.GetMember(memberInfo.Name, memberInfo.MemberType, bindingFlag).SingleOrDefault<MemberInfo>();
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			Type[] array = (
				from p in (IEnumerable<ParameterInfo>)propertyInfo.GetIndexParameters()
				select p.ParameterType).ToArray<Type>();
			return targetType.GetProperty(propertyInfo.Name, bindingFlag, null, propertyInfo.PropertyType, array, null);
		}

		public static Type GetMemberUnderlyingType(MemberInfo member)
		{
			ValidationUtils.ArgumentNotNull(member, "member");
			MemberTypes memberType = member.MemberType;
			switch (memberType)
			{
				case MemberTypes.Event:
				{
					return ((EventInfo)member).EventHandlerType;
				}
				case MemberTypes.Field:
				{
					return ((FieldInfo)member).FieldType;
				}
				default:
				{
					if (memberType == MemberTypes.Property)
					{
						break;
					}
					else
					{
						throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or EventInfo", "member");
					}
				}
			}
			return ((PropertyInfo)member).PropertyType;
		}

		public static object GetMemberValue(MemberInfo member, object target)
		{
			object value;
			ValidationUtils.ArgumentNotNull(member, "member");
			ValidationUtils.ArgumentNotNull(target, "target");
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).GetValue(target);
			}
			if (memberType != MemberTypes.Property)
			{
				throw new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, new object[] { CultureInfo.InvariantCulture, member.Name }), "member");
			}
			try
			{
				value = ((PropertyInfo)member).GetValue(target, null);
			}
			catch (TargetParameterCountException targetParameterCountException1)
			{
				TargetParameterCountException targetParameterCountException = targetParameterCountException1;
				throw new ArgumentException("MemberInfo '{0}' has index parameters".FormatWith(CultureInfo.InvariantCulture, new object[] { member.Name }), targetParameterCountException);
			}
			return value;
		}

		public static string GetNameAndAssessmblyName(Type t)
		{
			ValidationUtils.ArgumentNotNull(t, "t");
			return string.Concat(t.FullName, ", ", t.Assembly.GetName().Name);
		}

		public static Type GetObjectType(object v)
		{
			Type type;
			if (v == null)
			{
				type = null;
			}
			else
			{
				type = v.GetType();
			}
			return type;
		}

		public static IEnumerable<PropertyInfo> GetProperties(Type targetType, BindingFlags bindingAttr)
		{
			ValidationUtils.ArgumentNotNull(targetType, "targetType");
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>(targetType.GetProperties(bindingAttr));
			ReflectionUtils.GetChildPrivateProperties(propertyInfos, targetType, bindingAttr);
			for (int i = 0; i < propertyInfos.Count; i++)
			{
				PropertyInfo item = propertyInfos[i];
				if (item.DeclaringType != targetType)
				{
					PropertyInfo memberInfoFromType = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(item.DeclaringType, item);
					propertyInfos[i] = memberInfoFromType;
				}
			}
			return propertyInfos;
		}

		public static string GetTypeName(Type t, FormatterAssemblyStyle assemblyFormat)
		{
			return ReflectionUtils.GetTypeName(t, assemblyFormat, null);
		}

		public static string GetTypeName(Type t, FormatterAssemblyStyle assemblyFormat, SerializationBinder binder)
		{
			string assemblyQualifiedName = t.AssemblyQualifiedName;
			FormatterAssemblyStyle formatterAssemblyStyle = assemblyFormat;
			if (formatterAssemblyStyle == FormatterAssemblyStyle.Simple)
			{
				return ReflectionUtils.RemoveAssemblyDetails(assemblyQualifiedName);
			}
			if (formatterAssemblyStyle != FormatterAssemblyStyle.Full)
			{
				throw new ArgumentOutOfRangeException();
			}
			return t.AssemblyQualifiedName;
		}

		public static bool HasDefaultConstructor(Type t)
		{
			return ReflectionUtils.HasDefaultConstructor(t, false);
		}

		public static bool HasDefaultConstructor(Type t, bool nonPublic)
		{
			ValidationUtils.ArgumentNotNull(t, "t");
			if (t.IsValueType)
			{
				return true;
			}
			return ReflectionUtils.GetDefaultConstructor(t, nonPublic) != null;
		}

		public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition)
		{
			Type type1;
			return ReflectionUtils.ImplementsGenericDefinition(type, genericInterfaceDefinition, out type1);
		}

		public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, "genericInterfaceDefinition");
			if (!genericInterfaceDefinition.IsInterface || !genericInterfaceDefinition.IsGenericTypeDefinition)
			{
				throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatWith(CultureInfo.InvariantCulture, new object[] { genericInterfaceDefinition }));
			}
			if (type.IsInterface && type.IsGenericType && genericInterfaceDefinition == type.GetGenericTypeDefinition())
			{
				implementingType = type;
				return true;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				Type type1 = interfaces[i];
				if (type1.IsGenericType && genericInterfaceDefinition == type1.GetGenericTypeDefinition())
				{
					implementingType = type1;
					return true;
				}
			}
			implementingType = null;
			return false;
		}

		public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition)
		{
			Type type1;
			return ReflectionUtils.InheritsGenericDefinition(type, genericClassDefinition, out type1);
		}

		public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition, out Type implementingType)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			ValidationUtils.ArgumentNotNull(genericClassDefinition, "genericClassDefinition");
			if (!genericClassDefinition.IsClass || !genericClassDefinition.IsGenericTypeDefinition)
			{
				throw new ArgumentNullException("'{0}' is not a generic class definition.".FormatWith(CultureInfo.InvariantCulture, new object[] { genericClassDefinition }));
			}
			return ReflectionUtils.InheritsGenericDefinitionInternal(type, genericClassDefinition, out implementingType);
		}

		private static bool InheritsGenericDefinitionInternal(Type currentType, Type genericClassDefinition, out Type implementingType)
		{
			if (currentType.IsGenericType && genericClassDefinition == currentType.GetGenericTypeDefinition())
			{
				implementingType = currentType;
				return true;
			}
			if (currentType.BaseType == null)
			{
				implementingType = null;
				return false;
			}
			return ReflectionUtils.InheritsGenericDefinitionInternal(currentType.BaseType, genericClassDefinition, out implementingType);
		}

		public static bool IsCompatibleValue(object value, Type type)
		{
			if (value == null)
			{
				return ReflectionUtils.IsNullable(type);
			}
			if (type.IsAssignableFrom(value.GetType()))
			{
				return true;
			}
			return false;
		}

		public static bool IsIndexedProperty(MemberInfo member)
		{
			ValidationUtils.ArgumentNotNull(member, "member");
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo == null)
			{
				return false;
			}
			return ReflectionUtils.IsIndexedProperty(propertyInfo);
		}

		public static bool IsIndexedProperty(PropertyInfo property)
		{
			ValidationUtils.ArgumentNotNull(property, "property");
			return (int)property.GetIndexParameters().Length > 0;
		}

		public static bool IsInstantiatableType(Type t)
		{
			ValidationUtils.ArgumentNotNull(t, "t");
			if (t.IsAbstract || t.IsInterface || t.IsArray || t.IsGenericTypeDefinition || t == typeof(void))
			{
				return false;
			}
			if (!ReflectionUtils.HasDefaultConstructor(t))
			{
				return false;
			}
			return true;
		}

		public static bool IsNullable(Type t)
		{
			ValidationUtils.ArgumentNotNull(t, "t");
			if (!t.IsValueType)
			{
				return true;
			}
			return ReflectionUtils.IsNullableType(t);
		}

		public static bool IsNullableType(Type t)
		{
			ValidationUtils.ArgumentNotNull(t, "t");
			return (!t.IsGenericType ? false : t.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		private static bool IsOverridenGenericMember(MemberInfo memberInfo, BindingFlags bindingAttr)
		{
			if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
			{
				throw new ArgumentException("Member must be a field or property.");
			}
			Type declaringType = memberInfo.DeclaringType;
			if (!declaringType.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = declaringType.GetGenericTypeDefinition();
			if (genericTypeDefinition == null)
			{
				return false;
			}
			MemberInfo[] member = genericTypeDefinition.GetMember(memberInfo.Name, bindingAttr);
			if ((int)member.Length == 0)
			{
				return false;
			}
			if (!ReflectionUtils.GetMemberUnderlyingType(member[0]).IsGenericParameter)
			{
				return false;
			}
			return true;
		}

		public static bool IsPropertyIndexed(PropertyInfo property)
		{
			ValidationUtils.ArgumentNotNull(property, "property");
			return !CollectionUtils.IsNullOrEmpty<ParameterInfo>(property.GetIndexParameters());
		}

		public static bool IsUnitializedValue(object value)
		{
			if (value == null)
			{
				return true;
			}
			return value.Equals(ReflectionUtils.CreateUnitializedValue(value.GetType()));
		}

		public static bool IsVirtual(this PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			if (getMethod != null && getMethod.IsVirtual)
			{
				return true;
			}
			getMethod = propertyInfo.GetSetMethod();
			if (getMethod != null && getMethod.IsVirtual)
			{
				return true;
			}
			return false;
		}

		public static bool ItemsUnitializedValue<T>(IList<T> list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			Type collectionItemType = ReflectionUtils.GetCollectionItemType(list.GetType());
			if (!collectionItemType.IsValueType)
			{
				if (!collectionItemType.IsClass)
				{
					throw new Exception("Type {0} is neither a ValueType or a Class.".FormatWith(CultureInfo.InvariantCulture, new object[] { collectionItemType }));
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						return false;
					}
				}
			}
			else
			{
				object obj = ReflectionUtils.CreateUnitializedValue(collectionItemType);
				for (int j = 0; j < list.Count; j++)
				{
					if (!list[j].Equals(obj))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static Type MakeGenericType(Type genericTypeDefinition, params Type[] innerTypes)
		{
			ValidationUtils.ArgumentNotNull(genericTypeDefinition, "genericTypeDefinition");
			ValidationUtils.ArgumentNotNullOrEmpty<Type>(innerTypes, "innerTypes");
			ValidationUtils.ArgumentConditionTrue(genericTypeDefinition.IsGenericTypeDefinition, "genericTypeDefinition", "Type {0} is not a generic type definition.".FormatWith(CultureInfo.InvariantCulture, new object[] { genericTypeDefinition }));
			return genericTypeDefinition.MakeGenericType(innerTypes);
		}

		private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool flag1 = false;
			for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
			{
				char chr = fullyQualifiedTypeName[i];
				char chr1 = chr;
				switch (chr1)
				{
					case '[':
					{
						flag = false;
						flag1 = false;
						stringBuilder.Append(chr);
						break;
					}
					case ']':
					{
						flag = false;
						flag1 = false;
						stringBuilder.Append(chr);
						break;
					}
					default:
					{
						if (chr1 == ',')
						{
							if (flag)
							{
								flag1 = true;
							}
							else
							{
								flag = true;
								stringBuilder.Append(chr);
							}
							break;
						}
						else
						{
							if (!flag1)
							{
								stringBuilder.Append(chr);
							}
							break;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static BindingFlags RemoveFlag(this BindingFlags bindingAttr, BindingFlags flag)
		{
			return ((bindingAttr & flag) != flag ? bindingAttr : bindingAttr ^ flag);
		}

		public static void SetMemberValue(MemberInfo member, object target, object value)
		{
			ValidationUtils.ArgumentNotNull(member, "member");
			ValidationUtils.ArgumentNotNull(target, "target");
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				((FieldInfo)member).SetValue(target, value);
			}
			else
			{
				if (memberType != MemberTypes.Property)
				{
					throw new ArgumentException("MemberInfo '{0}' must be of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, new object[] { member.Name }), "member");
				}
				((PropertyInfo)member).SetValue(target, value, null);
			}
		}

		public static void SplitFullyQualifiedTypeName(string fullyQualifiedTypeName, out string typeName, out string assemblyName)
		{
			int? assemblyDelimiterIndex = ReflectionUtils.GetAssemblyDelimiterIndex(fullyQualifiedTypeName);
			if (!assemblyDelimiterIndex.HasValue)
			{
				typeName = fullyQualifiedTypeName;
				assemblyName = null;
			}
			else
			{
				typeName = fullyQualifiedTypeName.Substring(0, assemblyDelimiterIndex.Value).Trim();
				assemblyName = fullyQualifiedTypeName.Substring(assemblyDelimiterIndex.Value + 1, fullyQualifiedTypeName.Length - assemblyDelimiterIndex.Value - 1).Trim();
			}
		}
	}
}