/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Sharpen;

namespace Giis.Portable.Xml.Tiny
{
	/// <summary>Base class with implementation of methods that are common in java and net</summary>
	public abstract class XNodeAbstract
	{
		// Node primitives
		public abstract bool Equals(XNode xn);

		public abstract bool IsElement();

		public abstract bool IsText();

		public abstract bool IsWhitespace();

		public abstract XNode CreateElement(string elementName);

		public abstract XNode CreateText(string textValue);

		public abstract string Name();

		public abstract string Value();

		public abstract void SetValue(string value);

		public abstract string InnerText();

		public abstract void SetInnerText(string value);

		public abstract string OuterXml();

		public abstract string InnerXml();

		public abstract void SetInnerXml(string value);

		// Attribute primitives
		public abstract string GetAttribute(string name);

		public abstract void SetAttribute(string name, string value);

		public abstract IList<string> GetAttributeNames();

		public abstract string GetAttributesString();

		public virtual int GetIntAttribute(string name)
		{
			string current = this.GetAttribute(name);
			if (string.Empty.Equals(current))
			{
				// does not exists, default
				return 0;
			}
			else
			{
				return int.Parse(current);
			}
		}

		public virtual void IncrementIntAttribute(string name, int value)
		{
			string current = this.GetAttribute(name);
			if (string.Empty.Equals(current))
			{
				// does not exist, sets this value
				this.SetAttribute(name, value.ToString());
			}
			else
			{
				// exist, increments
				this.SetAttribute(name, (value + System.Convert.ToInt32(this.GetAttribute(name))).ToString());
			}
		}

		/// <summary>Gets the text inside the element with the indicated name, if it does not exist returns empty.</summary>
		/// <remarks>
		/// Gets the text inside the element with the indicated name, if it does not exist returns empty.
		/// It allows to represent logical attributes as elements as an alternative to native attributes
		/// </remarks>
		public virtual string GetElementAtribute(string name)
		{
			XNode elem = this.GetChild(name);
			return elem == null ? string.Empty : elem.InnerText();
		}

		// Node manipulation primitives
		public abstract XNode CloneNode();

		public abstract XNode ImportNode(XNode source);

		public abstract XNode CreateNode(string xml);

		public abstract void Normalize();

		public abstract XNode RemoveChild(XNode oldChild);

		public abstract XNode AppendChild(XNode elementOrTextNode);

		public virtual XNode AppendChild(string elementName)
		{
			return AppendChild(CreateElement(elementName));
		}

		public abstract XNode PrependChild(XNode node);

		public abstract XNode ReplaceChild(XNode newChild, XNode oldChild);

		public abstract XNode InsertBefore(XNode newChild);

		public abstract XNode InsertAfter(XNode newChild);

		public virtual XNode InsertBefore(string elementName)
		{
			return this.InsertBefore(this.CreateElement(elementName));
		}

		public virtual XNode InsertAfter(string elementName)
		{
			return this.InsertAfter(this.CreateElement(elementName));
		}

		public abstract void RemoveAttribute(string name);

		// Node navigation
		public abstract bool IsRoot();

		public abstract XNode Parent();

		public abstract XNode Root();

		public abstract XNode FirstChild();

		public abstract XNode NextSibling();

		public abstract XNode PreviousSibling();

		[System.ObsoleteAttribute(@"use firstChild()")]
		public virtual XNode GetFirstChild()
		{
			return FirstChild();
		}

		protected internal abstract IList<XNode> GetChildren(bool returnElements, bool returnTexts, string elementName, bool onlyFirst);

		/// <summary>Gets the first element child with the specified name, null if it does not exist</summary>
		public virtual XNode GetChild(string elementName)
		{
			IList<XNode> children = GetChildren(true, false, elementName, true);
			if (children.Count > 0)
			{
				// NOSONAR compatibility java 1.4 and C#
				return children[0];
			}
			return null;
		}

		/// <summary>Gets a list of all element children with the specified name</summary>
		public virtual IList<XNode> GetChildren(string elementName)
		{
			return GetChildren(true, false, elementName, false);
		}

		/// <summary>Gets a list of element children</summary>
		public virtual IList<XNode> GetChildren()
		{
			return GetChildren(true, false, null, false);
		}

		/// <summary>Gets a list of all children, including both text and elements</summary>
		public virtual IList<XNode> GetChildrenWithText()
		{
			return GetChildren(true, true, null, false);
		}

		/// <summary>Gets the number of children, including both text and elements</summary>
		public virtual int ChildCount()
		{
			int count = 0;
			XNode child = this.FirstChild();
			while (child != null)
			{
				count++;
				child = child.NextSibling();
			}
			return count;
		}

		// Character encoding
		public static string EncodeText(string xml)
		{
			if (xml == null || string.Empty.Equals(xml))
			{
				return string.Empty;
			}
			return xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
		}

		public static string DecodeText(string xml)
		{
			if (xml == null || string.Empty.Equals(xml))
			{
				return string.Empty;
			}
			return xml.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
		}

		public static string EncodeAttribute(string xml)
		{
			return EncodeText(xml).Replace("\"", "&quot;");
		}

		public static string DecodeAttribute(string xml)
		{
			if (xml == null)
			{
				return string.Empty;
			}
			xml = xml.Replace("&quot;", "\"");
			return DecodeText(xml);
		}
	}
}
