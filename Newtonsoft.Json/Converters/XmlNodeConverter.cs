using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	public class XmlNodeConverter : JsonConverter
	{
		private const string TextName = "#text";

		private const string CommentName = "#comment";

		private const string CDataName = "#cdata-section";

		private const string WhitespaceName = "#whitespace";

		private const string SignificantWhitespaceName = "#significant-whitespace";

		private const string DeclarationName = "?xml";

		private const string JsonNamespaceUri = "http://james.newtonking.com/projects/json";

		public string DeserializeRootElementName
		{
			get;
			set;
		}

		public bool OmitRootObject
		{
			get;
			set;
		}

		public bool WriteArrayAttribute
		{
			get;
			set;
		}

		public XmlNodeConverter()
		{
		}

		private void AddJsonArrayAttribute(IXmlElement element, IXmlDocument document)
		{
			element.SetAttributeNode(document.CreateAttribute("json:Array", "http://james.newtonking.com/projects/json", "true"));
		}

		public override bool CanConvert(Type valueType)
		{
			return false;
		}

		private IXmlElement CreateElement(string elementName, IXmlDocument document, string elementPrefix, XmlNamespaceManager manager)
		{
			return (string.IsNullOrEmpty(elementPrefix) ? document.CreateElement(elementName) : document.CreateElement(elementName, manager.LookupNamespace(elementPrefix)));
		}

		private void CreateInstruction(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName)
		{
			int num;
			if (propertyName != "?xml")
			{
				IXmlNode xmlNode = document.CreateProcessingInstruction(propertyName.Substring(1), reader.Value.ToString());
				currentNode.AppendChild(xmlNode);
			}
			else
			{
				string str = null;
				string str1 = null;
				string str2 = null;
				while (true)
				{
					if (!reader.Read() || reader.TokenType == JsonToken.EndObject)
					{
						IXmlNode xmlNode1 = document.CreateXmlDeclaration(str, str1, str2);
						currentNode.AppendChild(xmlNode1);
						return;
					}
					else
					{
						string str3 = reader.Value.ToString();
						if (str3 == null)
						{
							break;
						}
						if (XmlNodeConverter.<>f__switch$map1 == null)
						{
							Dictionary<string, int> strs = new Dictionary<string, int>(3)
							{
								{ "@version", 0 },
								{ "@encoding", 1 },
								{ "@standalone", 2 }
							};
							XmlNodeConverter.<>f__switch$map1 = strs;
						}
						if (!XmlNodeConverter.<>f__switch$map1.TryGetValue(str3, out num))
						{
							break;
						}
						switch (num)
						{
							case 0:
							{
								reader.Read();
								str = reader.Value.ToString();
								continue;
							}
							case 1:
							{
								reader.Read();
								str1 = reader.Value.ToString();
								continue;
							}
							case 2:
							{
								reader.Read();
								str2 = reader.Value.ToString();
								continue;
							}
						}
					}
				}
				throw new JsonSerializationException(string.Concat("Unexpected property name encountered while deserializing XmlDeclaration: ", reader.Value));
			}
		}

		private void DeserializeNode(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, IXmlNode currentNode)
		{
			do
			{
				JsonToken tokenType = reader.TokenType;
				switch (tokenType)
				{
					case JsonToken.StartConstructor:
					{
						string str = reader.Value.ToString();
						while (reader.Read() && reader.TokenType != JsonToken.EndConstructor)
						{
							this.DeserializeValue(reader, document, manager, str, currentNode);
						}
						continue;
					}
					case JsonToken.PropertyName:
					{
						if (currentNode.NodeType == XmlNodeType.Document && document.DocumentElement != null)
						{
							throw new JsonSerializationException("JSON root object has multiple properties. The root object must have a single property in order to create a valid XML document. Consider specifing a DeserializeRootElementName.");
						}
						string str1 = reader.Value.ToString();
						reader.Read();
						if (reader.TokenType != JsonToken.StartArray)
						{
							this.DeserializeValue(reader, document, manager, str1, currentNode);
						}
						else
						{
							int num = 0;
							while (reader.Read() && reader.TokenType != JsonToken.EndArray)
							{
								this.DeserializeValue(reader, document, manager, str1, currentNode);
								num++;
							}
							if (num == 1 && this.WriteArrayAttribute)
							{
								IXmlElement xmlElement = currentNode.ChildNodes.CastValid<IXmlElement>().Single<IXmlElement>((IXmlElement n) => n.LocalName == str1);
								this.AddJsonArrayAttribute(xmlElement, document);
							}
						}
						continue;
					}
					case JsonToken.Comment:
					{
						currentNode.AppendChild(document.CreateComment((string)reader.Value));
						continue;
					}
					default:
					{
						if (tokenType == JsonToken.EndObject || tokenType == JsonToken.EndArray)
						{
							break;
						}
						else
						{
							throw new JsonSerializationException(string.Concat("Unexpected JsonToken when deserializing node: ", reader.TokenType));
						}
					}
				}
				return;
			}
			while (reader.TokenType == JsonToken.PropertyName || reader.Read());
		}

		private void DeserializeValue(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, string propertyName, IXmlNode currentNode)
		{
			int num;
			string str = propertyName;
			if (str != null)
			{
				if (XmlNodeConverter.<>f__switch$map0 == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(4)
					{
						{ "#text", 0 },
						{ "#cdata-section", 1 },
						{ "#whitespace", 2 },
						{ "#significant-whitespace", 3 }
					};
					XmlNodeConverter.<>f__switch$map0 = strs;
				}
				if (XmlNodeConverter.<>f__switch$map0.TryGetValue(str, out num))
				{
					switch (num)
					{
						case 0:
						{
							currentNode.AppendChild(document.CreateTextNode(reader.Value.ToString()));
							return;
						}
						case 1:
						{
							currentNode.AppendChild(document.CreateCDataSection(reader.Value.ToString()));
							return;
						}
						case 2:
						{
							currentNode.AppendChild(document.CreateWhitespace(reader.Value.ToString()));
							return;
						}
						case 3:
						{
							currentNode.AppendChild(document.CreateSignificantWhitespace(reader.Value.ToString()));
							return;
						}
					}
				}
			}
			if (string.IsNullOrEmpty(propertyName) || propertyName[0] != '?')
			{
				if (reader.TokenType == JsonToken.StartArray)
				{
					this.ReadArrayElements(reader, document, propertyName, currentNode, manager);
					return;
				}
				this.ReadElement(reader, document, currentNode, propertyName, manager);
			}
			else
			{
				this.CreateInstruction(reader, document, currentNode, propertyName);
			}
		}

		private string GetPropertyName(IXmlNode node, XmlNamespaceManager manager)
		{
			switch (node.NodeType)
			{
				case XmlNodeType.Element:
				{
					return this.ResolveFullName(node, manager);
				}
				case XmlNodeType.Attribute:
				{
					if (node.NamespaceURI == "http://james.newtonking.com/projects/json")
					{
						return string.Concat("$", node.LocalName);
					}
					return string.Concat("@", this.ResolveFullName(node, manager));
				}
				case XmlNodeType.Text:
				{
					return "#text";
				}
				case XmlNodeType.CDATA:
				{
					return "#cdata-section";
				}
				case XmlNodeType.EntityReference:
				case XmlNodeType.Entity:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentType:
				case XmlNodeType.DocumentFragment:
				case XmlNodeType.Notation:
				case XmlNodeType.EndElement:
				case XmlNodeType.EndEntity:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when getting node name: ", node.NodeType));
				}
				case XmlNodeType.ProcessingInstruction:
				{
					return string.Concat("?", this.ResolveFullName(node, manager));
				}
				case XmlNodeType.Comment:
				{
					return "#comment";
				}
				case XmlNodeType.Whitespace:
				{
					return "#whitespace";
				}
				case XmlNodeType.SignificantWhitespace:
				{
					return "#significant-whitespace";
				}
				case XmlNodeType.XmlDeclaration:
				{
					return "?xml";
				}
				default:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when getting node name: ", node.NodeType));
				}
			}
		}

		private bool IsArray(IXmlNode node)
		{
			IXmlNode xmlNode;
			if (node.Attributes == null)
			{
				xmlNode = null;
			}
			else
			{
				xmlNode = node.Attributes.SingleOrDefault<IXmlNode>((IXmlNode a) => (a.LocalName != "Array" ? false : a.NamespaceURI == "http://james.newtonking.com/projects/json"));
			}
			IXmlNode xmlNode1 = xmlNode;
			return (xmlNode1 == null ? false : XmlConvert.ToBoolean(xmlNode1.Value));
		}

		private bool IsNamespaceAttribute(string attributeName, out string prefix)
		{
			if (attributeName.StartsWith("xmlns", StringComparison.Ordinal))
			{
				if (attributeName.Length == 5)
				{
					prefix = string.Empty;
					return true;
				}
				if (attributeName[5] == ':')
				{
					prefix = attributeName.Substring(6, attributeName.Length - 6);
					return true;
				}
			}
			prefix = null;
			return false;
		}

		private void PushParentNamespaces(IXmlNode node, XmlNamespaceManager manager)
		{
			List<IXmlNode> xmlNodes = null;
			IXmlNode xmlNode = node;
			while (true)
			{
				IXmlNode parentNode = xmlNode.ParentNode;
				xmlNode = parentNode;
				if (parentNode == null)
				{
					break;
				}
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (xmlNodes == null)
					{
						xmlNodes = new List<IXmlNode>();
					}
					xmlNodes.Add(xmlNode);
				}
			}
			if (xmlNodes != null)
			{
				xmlNodes.Reverse();
				foreach (IXmlNode xmlNode1 in xmlNodes)
				{
					manager.PushScope();
					IEnumerator<IXmlNode> enumerator = xmlNode1.Attributes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							IXmlNode current = enumerator.Current;
							if (!(current.NamespaceURI == "http://www.w3.org/2000/xmlns/") || !(current.LocalName != "xmlns"))
							{
								continue;
							}
							manager.AddNamespace(current.LocalName, current.Value);
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
			}
		}

		private void ReadArrayElements(JsonReader reader, IXmlDocument document, string propertyName, IXmlNode currentNode, XmlNamespaceManager manager)
		{
			string prefix = MiscellaneousUtils.GetPrefix(propertyName);
			IXmlElement xmlElement = this.CreateElement(propertyName, document, prefix, manager);
			currentNode.AppendChild(xmlElement);
			int num = 0;
			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				this.DeserializeValue(reader, document, manager, propertyName, xmlElement);
				num++;
			}
			if (this.WriteArrayAttribute)
			{
				this.AddJsonArrayAttribute(xmlElement, document);
			}
			if (num == 1 && this.WriteArrayAttribute)
			{
				IXmlElement xmlElement1 = xmlElement.ChildNodes.CastValid<IXmlElement>().Single<IXmlElement>((IXmlElement n) => n.LocalName == propertyName);
				this.AddJsonArrayAttribute(xmlElement1, document);
			}
		}

		private Dictionary<string, string> ReadAttributeElements(JsonReader reader, XmlNamespaceManager manager)
		{
			string str;
			string str1;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			bool flag = false;
			bool flag1 = false;
			if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null && reader.TokenType != JsonToken.Boolean && reader.TokenType != JsonToken.Integer && reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Date && reader.TokenType != JsonToken.StartConstructor)
			{
				while (!flag && !flag1 && reader.Read())
				{
					JsonToken tokenType = reader.TokenType;
					if (tokenType == JsonToken.PropertyName)
					{
						string str2 = reader.Value.ToString();
						if (string.IsNullOrEmpty(str2))
						{
							flag = true;
						}
						else
						{
							char chr = str2[0];
							if (chr == '$')
							{
								str2 = str2.Substring(1);
								reader.Read();
								str = reader.Value.ToString();
								string str3 = manager.LookupPrefix("http://james.newtonking.com/projects/json");
								if (str3 == null)
								{
									int? nullable = null;
									while (manager.LookupNamespace(string.Concat("json", nullable)) != null)
									{
										nullable = new int?(nullable.GetValueOrDefault() + 1);
									}
									str3 = string.Concat("json", nullable);
									strs.Add(string.Concat("xmlns:", str3), "http://james.newtonking.com/projects/json");
									manager.AddNamespace(str3, "http://james.newtonking.com/projects/json");
								}
								strs.Add(string.Concat(str3, ":", str2), str);
							}
							else if (chr == '@')
							{
								str2 = str2.Substring(1);
								reader.Read();
								str = reader.Value.ToString();
								strs.Add(str2, str);
								if (this.IsNamespaceAttribute(str2, out str1))
								{
									manager.AddNamespace(str1, str);
								}
							}
							else
							{
								flag = true;
							}
						}
					}
					else
					{
						if (tokenType != JsonToken.EndObject)
						{
							throw new JsonSerializationException(string.Concat("Unexpected JsonToken: ", reader.TokenType));
						}
						flag1 = true;
					}
				}
			}
			return strs;
		}

		private void ReadElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName, XmlNamespaceManager manager)
		{
			IXmlNode xmlNode;
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new JsonSerializationException("XmlNodeConverter cannot convert JSON with an empty property name to XML.");
			}
			Dictionary<string, string> strs = this.ReadAttributeElements(reader, manager);
			string prefix = MiscellaneousUtils.GetPrefix(propertyName);
			IXmlElement xmlElement = this.CreateElement(propertyName, document, prefix, manager);
			currentNode.AppendChild(xmlElement);
			foreach (KeyValuePair<string, string> keyValuePair in strs)
			{
				string str = MiscellaneousUtils.GetPrefix(keyValuePair.Key);
				if (string.IsNullOrEmpty(str))
				{
					xmlNode = document.CreateAttribute(keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					IXmlNode xmlNode1 = document.CreateAttribute(keyValuePair.Key, manager.LookupNamespace(str), keyValuePair.Value);
					xmlNode = xmlNode1;
				}
				xmlElement.SetAttributeNode(xmlNode);
			}
			if (reader.TokenType == JsonToken.String)
			{
				xmlElement.AppendChild(document.CreateTextNode(reader.Value.ToString()));
			}
			else if (reader.TokenType == JsonToken.Integer)
			{
				xmlElement.AppendChild(document.CreateTextNode(XmlConvert.ToString((long)reader.Value)));
			}
			else if (reader.TokenType == JsonToken.Float)
			{
				xmlElement.AppendChild(document.CreateTextNode(XmlConvert.ToString((double)reader.Value)));
			}
			else if (reader.TokenType == JsonToken.Boolean)
			{
				xmlElement.AppendChild(document.CreateTextNode(XmlConvert.ToString((bool)reader.Value)));
			}
			else if (reader.TokenType == JsonToken.Date)
			{
				DateTime value = (DateTime)reader.Value;
				xmlElement.AppendChild(document.CreateTextNode(XmlConvert.ToString(value, Newtonsoft.Json.Utilities.DateTimeUtils.ToSerializationMode(value.Kind))));
			}
			else if (reader.TokenType != JsonToken.Null)
			{
				if (reader.TokenType != JsonToken.EndObject)
				{
					manager.PushScope();
					this.DeserializeNode(reader, document, manager, xmlElement);
					manager.PopScope();
				}
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(new NameTable());
			IXmlDocument xmlDocument = null;
			IXmlNode xmlNode = null;
			if (xmlDocument == null || xmlNode == null)
			{
				throw new JsonSerializationException(string.Concat("Unexpected type when converting XML: ", objectType));
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new JsonSerializationException("XmlNodeConverter can only convert JSON that begins with an object.");
			}
			if (string.IsNullOrEmpty(this.DeserializeRootElementName))
			{
				reader.Read();
				this.DeserializeNode(reader, xmlDocument, xmlNamespaceManagers, xmlNode);
			}
			else
			{
				this.ReadElement(reader, xmlDocument, xmlNode, this.DeserializeRootElementName, xmlNamespaceManagers);
			}
			return xmlDocument.WrappedNode;
		}

		private string ResolveFullName(IXmlNode node, XmlNamespaceManager manager)
		{
			string str;
			if (node.NamespaceURI == null || node.LocalName == "xmlns" && node.NamespaceURI == "http://www.w3.org/2000/xmlns/")
			{
				str = null;
			}
			else
			{
				str = manager.LookupPrefix(node.NamespaceURI);
			}
			string str1 = str;
			if (string.IsNullOrEmpty(str1))
			{
				return node.LocalName;
			}
			return string.Concat(str1, ":", node.LocalName);
		}

		private void SerializeGroupedNodes(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
		{
			List<IXmlNode> xmlNodes;
			bool flag;
			Dictionary<string, List<IXmlNode>> strs = new Dictionary<string, List<IXmlNode>>();
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				IXmlNode item = node.ChildNodes[i];
				string propertyName = this.GetPropertyName(item, manager);
				if (!strs.TryGetValue(propertyName, out xmlNodes))
				{
					xmlNodes = new List<IXmlNode>();
					strs.Add(propertyName, xmlNodes);
				}
				xmlNodes.Add(item);
			}
			foreach (KeyValuePair<string, List<IXmlNode>> str in strs)
			{
				List<IXmlNode> value = str.Value;
				flag = (value.Count != 1 ? true : this.IsArray(value[0]));
				if (flag)
				{
					string key = str.Key;
					if (writePropertyName)
					{
						writer.WritePropertyName(key);
					}
					writer.WriteStartArray();
					for (int j = 0; j < value.Count; j++)
					{
						this.SerializeNode(writer, value[j], manager, false);
					}
					writer.WriteEndArray();
				}
				else
				{
					this.SerializeNode(writer, value[0], manager, writePropertyName);
				}
			}
		}

		private void SerializeNode(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
		{
			switch (node.NodeType)
			{
				case XmlNodeType.Element:
				{
					if (!this.IsArray(node) || !node.ChildNodes.All<IXmlNode>((IXmlNode n) => n.LocalName == node.LocalName) || node.ChildNodes.Count <= 0)
					{
						IEnumerator<IXmlNode> enumerator = node.Attributes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								IXmlNode current = enumerator.Current;
								if (current.NamespaceURI != "http://www.w3.org/2000/xmlns/")
								{
									continue;
								}
								manager.AddNamespace((current.LocalName == "xmlns" ? string.Empty : current.LocalName), current.Value);
							}
						}
						finally
						{
							if (enumerator == null)
							{
							}
							enumerator.Dispose();
						}
						if (writePropertyName)
						{
							writer.WritePropertyName(this.GetPropertyName(node, manager));
						}
						if (this.ValueAttributes(node.Attributes).Count<IXmlNode>() == 0 && node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType == XmlNodeType.Text)
						{
							writer.WriteValue(node.ChildNodes[0].Value);
						}
						else if (node.ChildNodes.Count != 0 || !CollectionUtils.IsNullOrEmpty<IXmlNode>(node.Attributes))
						{
							writer.WriteStartObject();
							for (int i = 0; i < node.Attributes.Count; i++)
							{
								this.SerializeNode(writer, node.Attributes[i], manager, true);
							}
							this.SerializeGroupedNodes(writer, node, manager, true);
							writer.WriteEndObject();
						}
						else
						{
							writer.WriteNull();
						}
					}
					else
					{
						this.SerializeGroupedNodes(writer, node, manager, false);
					}
					break;
				}
				case XmlNodeType.Attribute:
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
				{
					if (node.NamespaceURI == "http://www.w3.org/2000/xmlns/" && node.Value == "http://james.newtonking.com/projects/json")
					{
						return;
					}
					if (node.NamespaceURI == "http://james.newtonking.com/projects/json" && node.LocalName == "Array")
					{
						return;
					}
					if (writePropertyName)
					{
						writer.WritePropertyName(this.GetPropertyName(node, manager));
					}
					writer.WriteValue(node.Value);
					break;
				}
				case XmlNodeType.EntityReference:
				case XmlNodeType.Entity:
				case XmlNodeType.DocumentType:
				case XmlNodeType.Notation:
				case XmlNodeType.EndElement:
				case XmlNodeType.EndEntity:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when serializing nodes: ", node.NodeType));
				}
				case XmlNodeType.Comment:
				{
					if (writePropertyName)
					{
						writer.WriteComment(node.Value);
					}
					break;
				}
				case XmlNodeType.Document:
				case XmlNodeType.DocumentFragment:
				{
					this.SerializeGroupedNodes(writer, node, manager, writePropertyName);
					break;
				}
				case XmlNodeType.XmlDeclaration:
				{
					IXmlDeclaration xmlDeclaration = (IXmlDeclaration)node;
					writer.WritePropertyName(this.GetPropertyName(node, manager));
					writer.WriteStartObject();
					if (!string.IsNullOrEmpty(xmlDeclaration.Version))
					{
						writer.WritePropertyName("@version");
						writer.WriteValue(xmlDeclaration.Version);
					}
					if (!string.IsNullOrEmpty(xmlDeclaration.Encoding))
					{
						writer.WritePropertyName("@encoding");
						writer.WriteValue(xmlDeclaration.Encoding);
					}
					if (!string.IsNullOrEmpty(xmlDeclaration.Standalone))
					{
						writer.WritePropertyName("@standalone");
						writer.WriteValue(xmlDeclaration.Standalone);
					}
					writer.WriteEndObject();
					break;
				}
				default:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when serializing nodes: ", node.NodeType));
				}
			}
		}

		private IEnumerable<IXmlNode> ValueAttributes(IEnumerable<IXmlNode> c)
		{
			return 
				from a in c
				where a.NamespaceURI != "http://james.newtonking.com/projects/json"
				select a;
		}

		private IXmlNode WrapXml(object value)
		{
			throw new ArgumentException("Value must be an XML object.", "value");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			IXmlNode xmlNode = this.WrapXml(value);
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(new NameTable());
			this.PushParentNamespaces(xmlNode, xmlNamespaceManagers);
			if (!this.OmitRootObject)
			{
				writer.WriteStartObject();
			}
			this.SerializeNode(writer, xmlNode, xmlNamespaceManagers, !this.OmitRootObject);
			if (!this.OmitRootObject)
			{
				writer.WriteEndObject();
			}
		}
	}
}