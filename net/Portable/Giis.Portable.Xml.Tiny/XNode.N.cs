using System;
using System.Collections.Generic;
using System.Xml;

namespace Giis.Portable.Xml.Tiny
{
    /// <summary>
	/// Simple wrapper to read xml documents with unified management of text and element nodes.
	/// Includes a few methods to manipulate the document
    /// </summary>
    public class XNode : XNodeAbstract
	{
		protected XmlNode node = null; // not private to allow extend this class

        public XNode(string xml)
		{
			XmlDocument doc = NewDocument(xml);
            this.node = doc.DocumentElement; //firstchild localizara otros elementos que pueda haber
		}
        // Not private to be more extensible
        public XNode(XmlNode nativeNode)
		{
			this.node = nativeNode;
		}
        public XmlNode GetNativeNode()
        {
            return this.node;
        }
        public XNode NewXNode(XmlNode nativeNode)
        {
            return nativeNode == null ? null : new XNode(nativeNode);
        }
        public override bool Equals(XNode xn)
        {
            if (this.node == null || xn == null || xn.GetNativeNode() == null)
                return false;
            return this.node.Equals(xn.GetNativeNode());
        }

        public override bool IsElement()
        {
            return this.node.NodeType == XmlNodeType.Element;
        }
        public override bool IsText()
        {
            return this.node.NodeType == XmlNodeType.Text;
        }
        public override bool IsWhitespace()
        {
            return node.NodeType == XmlNodeType.Whitespace;
        }
        private bool IsBlank(XmlNode n)
        {
            return n.NodeType == XmlNodeType.Whitespace;
        }
        public override XNode CreateElement(String elementName)
        {
            return NewXNode(this.node.OwnerDocument.CreateElement(elementName));
        }
        public override XNode CreateText(String textValue)
        {
            return NewXNode(this.node.OwnerDocument.CreateTextNode(textValue));
        }
        public override string Name()
        {
            return this.node.Name;
        }
        public override string Value()
        {
            return this.node.Value;
        }
        public override void SetValue(string value)
        {
            if (this.IsElement())
                return;
            this.node.Value=value;
        }
        public override string InnerText()
        {
            return this.node.InnerText;
        }
        public override void SetInnerText(String value)
        {
            this.node.InnerText=value;
        }
        public override string OuterXml()
        {
            return this.node.OuterXml;
        }
        public override string InnerXml()
        {
            return this.node.InnerXml;
        }
        public override void SetInnerXml(string value)
        {
            this.node.InnerXml = value;
        }
        public void RemoveChildren()
        {
            this.node.InnerXml = "";
        }
        public override string ToString()
		{
            return this.node.OuterXml;
		}

        public override string GetAttribute(string name)
        {
            if (!this.IsElement()) //los nodos texto o de otro tipo no tienen atributos
                return "";
            if (this.node.Attributes[name] == null)
                return "";
            return ((XmlElement)this.node).GetAttribute(name);
        }

        public override void SetAttribute(string name, string value)
        {
            if (!this.IsElement())
                return;
            ((XmlElement)this.node).SetAttribute(name, value);
        }

        public override IList<string> GetAttributeNames()
        {
            List<string> attrs = new List<string>();
            if (!this.IsElement())
                return attrs;
            XmlAttributeCollection na = this.node.Attributes;
            for (int i = 0; i < na.Count; i++)
                attrs.Add(na.Item(i).Name);
            attrs.Sort();
            return attrs;
        }
        public override string GetAttributesString()
        {
            if (!this.IsElement())
                return "";
            string attr = "";
            IList<string> na = this.GetAttributeNames();
            for (int i = 0; i < na.Count; i++)
            {
                string aName = na[i];
                string aValue = this.GetAttribute(aName);
                attr = attr + " " + aName + "=" + "\"" + EncodeAttribute(aValue) + "\"";
            }
            return attr;
        }

        public override XNode CloneNode()
        {
            return NewXNode(this.node.Clone());
        }
        public override XNode ImportNode(XNode source)
        {
            XmlNode xnn = source.GetNativeNode();
            XmlNode xni = this.node.OwnerDocument.ImportNode(xnn, true);
            return NewXNode(xni);
        }

        public override XNode CreateNode(string xml)
        {
            XNode n = this.CreateElement("newnode");
            n.SetInnerXml(xml);
            return n.FirstChild();
        }
        public override void Normalize()
        {
            this.node.Normalize();
        }

        public override XNode RemoveChild(XNode oldChild)
        {
            return NewXNode(this.node.RemoveChild(oldChild.node));
        }
        public override XNode AppendChild(XNode elementOrTextNode)
        {
            XmlNode newChild = this.node.AppendChild(elementOrTextNode.GetNativeNode());
            return new XNode(newChild);
        }
        public override XNode PrependChild(XNode node)
        {
            XNode first = this.FirstChild();
            if (first == null)
                return this.AppendChild(node);
            else
                return first.InsertBefore(node);
        }
        public override XNode ReplaceChild(XNode newChild, XNode oldChild)
        {
            return NewXNode(this.node.ReplaceChild(newChild.node, oldChild.node));
        }
        public override XNode InsertBefore(XNode newChild) {
            return NewXNode(this.node.ParentNode.InsertBefore(newChild.node, this.node));
        }
        public override XNode InsertAfter(XNode newChild)
        {
            return NewXNode(this.node.ParentNode.InsertAfter(newChild.node, this.node));
        }
        public override void RemoveAttribute(string name)
        {
            try
            {
                XmlAttribute attrNode = ((XmlElement)this.node).GetAttributeNode(name);
                if (attrNode != null)
                    ((XmlElement)this.node).Attributes.Remove(attrNode);
            }
            catch (Exception e)
            {
                throw new XmlException("XNode.setAttribute: Operacion no permitida si el nodo no es Element", e);
            }
        }

        public override bool IsRoot()
        {
            XNode parent = this.Parent();
            return (parent == null || parent.GetNativeNode().NodeType == XmlNodeType.Document);
        }
        public override XNode Root()
        {
            XmlNode n = this.node.OwnerDocument.FirstChild;
            return new XNode(n);
        }
        public override XNode Parent()
        {
            XmlNode parent=this.node.ParentNode;
            return parent == null || parent.NodeType == XmlNodeType.Document ? null : NewXNode(parent);
        }

        public override XNode FirstChild()
        {
            XmlNode n = this.node.FirstChild;
            n = this.SkipBlankSiblings(n, true);
            return NewXNode(n);
        }
        public override XNode NextSibling()
        { // si no hay sibling devuelve null
            XmlNode n = this.node.NextSibling;
            n = this.SkipBlankSiblings(n, true);
            return NewXNode(n);
        }
        public override XNode PreviousSibling()
        { // si no hay sibling devuelve null
            XmlNode n = this.node.PreviousSibling;
            n = this.SkipBlankSiblings(n, false);
            return NewXNode(n);
        }
        private XmlNode SkipBlankSiblings(XmlNode n, bool forward)
        {
            if (n == null) return null;
            while (n.NodeType == XmlNodeType.Whitespace)
            {
                n = forward ? n.NextSibling : n.PreviousSibling;
                if (n == null) return null;
            }
            return n;
        }


        // Additional methods for internal use

        /// <summary>
		/// Creates a new XmlDocument from the xml string
		/// </summary>
        private XmlDocument NewDocument(string xml)
		{
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);
            return doc;
		}

        //Returns child element or text nodes as indicated
        //If element, allows search by node name, oherwise, set elementName to null
        protected override IList<XNode> GetChildren(bool returnElements, bool returnTexts, String elementName, bool onlyFirst)
		{
			IList<XNode> children = new List<XNode>();
			XmlNode child = node.FirstChild;
			while (child != null)
			{
				if (returnElements && child.NodeType == XmlNodeType.Element && (elementName == null || child.Name == elementName)
                    || returnTexts && child.NodeType == XmlNodeType.Text && !IsBlank(child))
					{
						children.Add(new XNode(child));
						if (onlyFirst) //para devolver solo el primero que se encuentra
							return children;
					}
				child = child.NextSibling;
			}
			return children;
		}

        //not exactly as its java counterpart, only to compile tests
        public string ToXmlDocument()
        {
            return node.OuterXml;
        }
    }
}
