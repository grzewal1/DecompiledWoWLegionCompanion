using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlNodeWrapper : IXmlNode
	{
		private readonly XmlNode _node;

		public IList<IXmlNode> Attributes
		{
			get
			{
				if (this._node.Attributes == null)
				{
					return null;
				}
				return (
					from XmlAttribute a in this._node.Attributes
					select this.WrapNode(a)).ToList<IXmlNode>();
			}
		}

		public IList<IXmlNode> ChildNodes
		{
			get
			{
				return (
					from XmlNode n in this._node.ChildNodes
					select this.WrapNode(n)).ToList<IXmlNode>();
			}
		}

		public string LocalName
		{
			get
			{
				return this._node.LocalName;
			}
		}

		public string Name
		{
			get
			{
				return this._node.Name;
			}
		}

		public string NamespaceURI
		{
			get
			{
				return this._node.NamespaceURI;
			}
		}

		public XmlNodeType NodeType
		{
			get
			{
				return this._node.NodeType;
			}
		}

		public IXmlNode ParentNode
		{
			get
			{
				XmlNode parentNode;
				if (!(this._node is XmlAttribute))
				{
					parentNode = this._node.ParentNode;
				}
				else
				{
					parentNode = ((XmlAttribute)this._node).OwnerElement;
				}
				XmlNode xmlNodes = parentNode;
				if (xmlNodes == null)
				{
					return null;
				}
				return this.WrapNode(xmlNodes);
			}
		}

		public string Prefix
		{
			get
			{
				return this._node.Prefix;
			}
		}

		public string Value
		{
			get
			{
				return this._node.Value;
			}
			set
			{
				this._node.Value = value;
			}
		}

		public object WrappedNode
		{
			get
			{
				return this._node;
			}
		}

		public XmlNodeWrapper(XmlNode node)
		{
			this._node = node;
		}

		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			this._node.AppendChild(xmlNodeWrapper._node);
			return newChild;
		}

		private IXmlNode WrapNode(XmlNode node)
		{
			XmlNodeType nodeType = node.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				return new XmlElementWrapper((XmlElement)node);
			}
			if (nodeType != XmlNodeType.XmlDeclaration)
			{
				return new XmlNodeWrapper(node);
			}
			return new XmlDeclarationWrapper((XmlDeclaration)node);
		}
	}
}