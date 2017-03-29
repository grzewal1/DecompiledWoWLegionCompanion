using System;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlDocumentWrapper : XmlNodeWrapper, IXmlDocument, IXmlNode
	{
		private XmlDocument _document;

		public IXmlElement DocumentElement
		{
			get
			{
				if (this._document.DocumentElement == null)
				{
					return null;
				}
				return new XmlElementWrapper(this._document.DocumentElement);
			}
		}

		public XmlDocumentWrapper(XmlDocument document) : base(document)
		{
			this._document = document;
		}

		public IXmlNode CreateAttribute(string name, string value)
		{
			XmlNodeWrapper xmlNodeWrapper = new XmlNodeWrapper(this._document.CreateAttribute(name))
			{
				Value = value
			};
			return xmlNodeWrapper;
		}

		public IXmlNode CreateAttribute(string qualifiedName, string namespaceURI, string value)
		{
			XmlNodeWrapper xmlNodeWrapper = new XmlNodeWrapper(this._document.CreateAttribute(qualifiedName, namespaceURI))
			{
				Value = value
			};
			return xmlNodeWrapper;
		}

		public IXmlNode CreateCDataSection(string data)
		{
			return new XmlNodeWrapper(this._document.CreateCDataSection(data));
		}

		public IXmlNode CreateComment(string data)
		{
			return new XmlNodeWrapper(this._document.CreateComment(data));
		}

		public IXmlElement CreateElement(string elementName)
		{
			return new XmlElementWrapper(this._document.CreateElement(elementName));
		}

		public IXmlElement CreateElement(string qualifiedName, string namespaceURI)
		{
			return new XmlElementWrapper(this._document.CreateElement(qualifiedName, namespaceURI));
		}

		public IXmlNode CreateProcessingInstruction(string target, string data)
		{
			return new XmlNodeWrapper(this._document.CreateProcessingInstruction(target, data));
		}

		public IXmlNode CreateSignificantWhitespace(string text)
		{
			return new XmlNodeWrapper(this._document.CreateSignificantWhitespace(text));
		}

		public IXmlNode CreateTextNode(string text)
		{
			return new XmlNodeWrapper(this._document.CreateTextNode(text));
		}

		public IXmlNode CreateWhitespace(string text)
		{
			return new XmlNodeWrapper(this._document.CreateWhitespace(text));
		}

		public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XmlNodeWrapper(this._document.CreateXmlDeclaration(version, encoding, standalone));
		}
	}
}