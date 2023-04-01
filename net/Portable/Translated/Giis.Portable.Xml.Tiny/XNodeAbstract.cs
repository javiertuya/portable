/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Sharpen;

namespace Giis.Portable.Xml.Tiny
{
	/// <summary>Base class with implementation of methods that are common in java and net</summary>
	public abstract class XNodeAbstract
	{
		public abstract string GetAttribute(string name);

		public abstract void SetAttribute(string name, string value);

		public abstract IList<string> GetAttributeNames();

		public abstract XNode GetChild(string elementName);

		public abstract IList<XNode> GetChildren(string elementName);

		public abstract XNode GetFirstChild();

		public abstract string Name();

		public abstract string InnerText();

		public abstract void SetInnerText(string value);

		public abstract XNode AppendChild(string elementName);

		public abstract string OuterXml();

		public abstract string InnerXml();

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
