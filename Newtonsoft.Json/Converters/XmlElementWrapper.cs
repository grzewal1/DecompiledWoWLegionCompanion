using System;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlElementWrapper : XmlNodeWrapper, IXmlElement, IXmlNode
	{
		private XmlElement _element;

		public XmlElementWrapper(XmlElement element) : base(element)
		{
			this._element = element;
		}

		public string GetPrefixOfNamespace(string namespaceURI)
		{
			return this._element.GetPrefixOfNamespace(namespaceURI);
		}

		public void SetAttributeNode(IXmlNode attribute)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)attribute;
			this._element.SetAttributeNode((XmlAttribute)xmlNodeWrapper.WrappedNode);
		}
	}
}