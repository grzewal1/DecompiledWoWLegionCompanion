using System;
using System.Collections.Generic;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal interface IXmlNode
	{
		IList<IXmlNode> Attributes
		{
			get;
		}

		IList<IXmlNode> ChildNodes
		{
			get;
		}

		string LocalName
		{
			get;
		}

		string NamespaceURI
		{
			get;
		}

		XmlNodeType NodeType
		{
			get;
		}

		IXmlNode ParentNode
		{
			get;
		}

		string Value
		{
			get;
			set;
		}

		object WrappedNode
		{
			get;
		}

		IXmlNode AppendChild(IXmlNode newChild);
	}
}