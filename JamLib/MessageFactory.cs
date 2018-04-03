using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JamLib
{
	public static class MessageFactory
	{
		private static Dictionary<string, Type> s_messageDictionary;

		private static JsonSerializerSettings s_jsonSerializerSettings;

		public static JsonSerializerSettings SerializerSettings
		{
			get
			{
				return MessageFactory.s_jsonSerializerSettings;
			}
		}

		static MessageFactory()
		{
			IEnumerable<Type> types = 
				from t in Assembly.GetExecutingAssembly().GetTypes()
				where (t.Namespace == null || !t.Namespace.StartsWith("WowJamMessages") ? false : t.IsClass)
				select t;
			MessageFactory.s_messageDictionary = types.ToDictionary<Type, string>((Type t) => t.Name);
			MessageFactory.s_jsonSerializerSettings = new JsonSerializerSettings();
			List<JsonConverter> jsonConverters = new List<JsonConverter>()
			{
				new StringEnumConverter(),
				new JamEmbeddedMessageConverter(),
				new ByteArrayConverter()
			};
			MessageFactory.s_jsonSerializerSettings.Converters = jsonConverters;
		}

		public static object Deserialize(string message)
		{
			object obj;
			int num = message.IndexOf(':');
			if (num <= 0)
			{
				return null;
			}
			string str = message.Substring(0, num);
			string str1 = message.Substring(num + 1);
			if (str.Length <= 0 || str1.Length <= 0)
			{
				return null;
			}
			Type messageType = MessageFactory.GetMessageType(str);
			if (messageType == null)
			{
				return null;
			}
			try
			{
				obj = JsonConvert.DeserializeObject(str1, messageType, MessageFactory.s_jsonSerializerSettings);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				return null;
			}
			return obj;
		}

		public static MessageDispatch GetDispatcher(Type handlerType)
		{
			IEnumerable<Type> types = MessageFactory.s_messageDictionary.Values.Where<Type>((Type t) => {
				MethodInfo method = handlerType.GetMethod(string.Concat(t.Name, "Handler"));
				return (method == null || (int)method.GetParameters().Length != 1 ? false : method.GetParameters()[0].ParameterType == t);
			});
			Dictionary<Type, MethodInfo> dictionary = types.ToDictionary<Type, Type, MethodInfo>((Type t) => t, (Type t) => handlerType.GetMethod(string.Concat(t.Name, "Handler")));
			return (object handler, object message) => {
				MethodInfo methodInfo;
				if (!dictionary.TryGetValue(message.GetType(), out methodInfo))
				{
					return false;
				}
				methodInfo.Invoke(handler, new object[] { message });
				return true;
			};
		}

		public static Type GetMessageType(string nameSpace, string nameType)
		{
			return Type.GetType(string.Concat(nameSpace, ".", nameType));
		}

		public static Type GetMessageType(string nameType)
		{
			Type type;
			MessageFactory.s_messageDictionary.TryGetValue(nameType, out type);
			return type;
		}

		public static string Serialize(object message)
		{
			Type type = message.GetType();
			return string.Concat(type.Name, ":", JsonConvert.SerializeObject(message, Formatting.None, MessageFactory.s_jsonSerializerSettings));
		}
	}
}