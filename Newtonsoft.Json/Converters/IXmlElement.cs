using System;

namespace Newtonsoft.Json.Converters
{
	internal interface IXmlElement : IXmlNode
	{
		string GetPrefixOfNamespace(string namespaceURI);

		void SetAttributeNode(IXmlNode attribute);
	}
}