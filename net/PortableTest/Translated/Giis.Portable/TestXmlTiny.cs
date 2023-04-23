/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Giis.Portable.Util;
using Giis.Portable.Xml.Tiny;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestXmlTiny
	{
		private const string Xml2 = "<elem2>  \n" + "	\t<nested>text1</nested>\n" + "	\t<other>textother</other>\n" + "	\t<nested>text2</nested>\n" + "  </elem2>";

		private const string Xml1 = "<noderoot>\r\n" + "  <elem1 />  \n" + "  " + Xml2 + "\n" + "  <elem3>text3</elem3>  \n" + "</noderoot>\n ";

		//using spaces to check that is not whitespace aware
		//note that XML1 contains XML2
		[Test]
		public virtual void TestDocumentRoot()
		{
			XNode n = new XNode("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n   \n<noderoot><elem1/></noderoot>   ");
			NUnit.Framework.Assert.AreEqual("noderoot", n.Name());
			NUnit.Framework.Assert.AreEqual("elem1", n.GetFirstChild().Name());
			NUnit.Framework.Assert.AreEqual("<noderoot><elem1 /></noderoot>", n.OuterXml());
			NUnit.Framework.Assert.AreEqual("<noderoot><elem1 /></noderoot>", n.ToString());
			//as xml document (native) gets the header, with little different element formatting
			if (Parameters.IsJava())
			{
				NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><noderoot><elem1/></noderoot>", n.ToXmlDocument());
			}
		}

		[Test]
		public virtual void TestFindChildren()
		{
			XNode n = new XNode("<noderoot><!-- comment --><elem1>text1</elem1>text2<elem3 /></noderoot>");
			// by default, only element nodes
			IList<XNode> n1 = n.GetChildren();
			NUnit.Framework.Assert.AreEqual(2, n1.Count);
			NUnit.Framework.Assert.IsTrue(n1[0].IsElement());
			NUnit.Framework.Assert.AreEqual("elem1", n1[0].Name());
			NUnit.Framework.Assert.IsTrue(n1[1].IsElement());
			NUnit.Framework.Assert.AreEqual("elem3", n1[1].Name());
			// but can get all too (only text and element, comments are ignored)
			n1 = n.GetChildrenWithText();
			NUnit.Framework.Assert.AreEqual(3, n1.Count);
			NUnit.Framework.Assert.AreEqual("elem1", n1[0].Name());
			NUnit.Framework.Assert.IsTrue(n1[1].IsText());
			NUnit.Framework.Assert.AreEqual("text2", n1[1].InnerText());
			NUnit.Framework.Assert.AreEqual("elem3", n1[2].Name());
		}

		[Test]
		public virtual void TestElementFindChildAndGetContent()
		{
			// also checks text and xml content
			XNode n = new XNode(Xml1);
			NUnit.Framework.Assert.AreEqual("noderoot", n.Name());
			// leaf element
			XNode n1 = n.GetChild("elem1");
			NUnit.Framework.Assert.AreEqual("elem1", n1.Name());
			NUnit.Framework.Assert.AreEqual("<elem1 />", n1.OuterXml());
			NUnit.Framework.Assert.AreEqual(string.Empty, n1.InnerXml());
			NUnit.Framework.Assert.AreEqual(string.Empty, n1.InnerText());
			// with only text
			XNode n3 = n.GetChild("elem3");
			NUnit.Framework.Assert.AreEqual("elem3", n3.Name());
			NUnit.Framework.Assert.AreEqual("<elem3>text3</elem3>", n3.OuterXml());
			NUnit.Framework.Assert.AreEqual("text3", n3.InnerXml());
			NUnit.Framework.Assert.AreEqual("text3", n3.InnerText());
			// child does not exist
			XNode nn = n.GetChild("doesnotexist");
			NUnit.Framework.Assert.IsNull(nn);
		}

		[Test]
		public virtual void TestElementFindChildNestedAndGetContent()
		{
			XNode n = new XNode(Xml1).GetChild("elem2");
			// with xml
			NUnit.Framework.Assert.AreEqual("elem2", n.Name());
			NUnit.Framework.Assert.AreEqual(Xml2, n.OuterXml());
			NUnit.Framework.Assert.AreEqual(Xml2.Replace("</elem2>", string.Empty).Replace("<elem2>", string.Empty), n.InnerXml());
			// innerText removes tags, but preserves everything, including whitespaces
			string n2text = Xml2.Replace("</elem2>", string.Empty).Replace("<elem2>", string.Empty).Replace("</nested>", string.Empty).Replace("<nested>", string.Empty).Replace("</other>", string.Empty).Replace("<other>", string.Empty);
			NUnit.Framework.Assert.AreEqual(n2text, n.InnerText());
			// the first element is selected if more than one match
			XNode xn = n.GetChild("nested");
			NUnit.Framework.Assert.AreEqual("<nested>text1</nested>", xn.OuterXml());
		}

		[Test]
		public virtual void TestElementFindChildrenByName()
		{
			XNode n = new XNode(Xml1);
			IList<XNode> n1 = n.GetChildren("elem1");
			NUnit.Framework.Assert.AreEqual(1, n1.Count);
			NUnit.Framework.Assert.AreEqual("elem1", n1[0].Name());
			IList<XNode> n2 = n.GetChildren("elem2");
			NUnit.Framework.Assert.AreEqual(1, n2.Count);
			IList<XNode> n21 = n.GetChild("elem2").GetChildren("nested");
			NUnit.Framework.Assert.AreEqual(2, n21.Count);
			NUnit.Framework.Assert.AreEqual("<nested>text1</nested>", n21[0].OuterXml());
			NUnit.Framework.Assert.AreEqual("<nested>text2</nested>", n21[1].OuterXml());
			NUnit.Framework.Assert.AreEqual(0, n21[0].GetChildren("text1").Count);
			// text node, no children
			IList<XNode> nn = n.GetChildren("doesnotexist");
			NUnit.Framework.Assert.AreEqual(0, nn.Count);
		}

		[Test]
		public virtual void TestElementAttributes()
		{
			// TODO atributos sobre nodo texto
			string xml = "<root><elem1 aaa=\"text1\" bbb=\"3\" /><elem2 /></root>";
			XNode n = new XNode(xml);
			XNode n1 = n.GetChild("elem1");
			NUnit.Framework.Assert.AreEqual("text1", n1.GetAttribute("aaa"));
			NUnit.Framework.Assert.AreEqual(string.Empty, n1.GetAttribute("doesnotexist"));
			// default is empty
			NUnit.Framework.Assert.AreEqual("3", n1.GetAttribute("bbb"));
			NUnit.Framework.Assert.AreEqual(xml, n.OuterXml());
			XNode n2 = n.GetChild("elem2");
			n2.SetAttribute("xxx", "new");
			NUnit.Framework.Assert.AreEqual("new", n2.GetAttribute("xxx"));
			NUnit.Framework.Assert.AreEqual("<elem2 xxx=\"new\" />", n2.OuterXml());
		}

		[Test]
		public virtual void TestElementAttributesAsInt()
		{
			string xml = "<root><elem1 aaa=\"text1\" bbb=\"3\"/><elem2/></root>";
			XNode n1 = new XNode(xml).GetChild("elem1");
			NUnit.Framework.Assert.AreEqual(3, n1.GetIntAttribute("bbb"));
			NUnit.Framework.Assert.AreEqual(0, n1.GetIntAttribute("notexistsint"));
			// default is 0
			n1.IncrementIntAttribute("bbb", 2);
			NUnit.Framework.Assert.AreEqual(5, n1.GetIntAttribute("bbb"));
			n1.IncrementIntAttribute("notexisted", 32);
			// default specified
			NUnit.Framework.Assert.AreEqual(32, n1.GetIntAttribute("notexisted"));
			try
			{
				n1.GetIntAttribute("aaa");
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestElementAttributeNames()
		{
			string xml = "<root><elem1 bbb=\"val2\" ccc=\"val3\" aaa=\"val1\" /><elem2/></root>";
			XNode n = new XNode(xml).GetChild("elem1");
			NUnit.Framework.Assert.AreEqual("[aaa, bbb, ccc]", JavaCs.DeepToString(JavaCs.ToArray(n.GetAttributeNames())));
			// without attributes
			n = new XNode("<root><elem1>text</elem1></root>").GetChild("elem1");
			NUnit.Framework.Assert.AreEqual("[]", JavaCs.DeepToString(JavaCs.ToArray(n.GetAttributeNames())));
			// is text
			n = n.GetFirstChild();
			NUnit.Framework.Assert.AreEqual("[]", JavaCs.DeepToString(JavaCs.ToArray(n.GetAttributeNames())));
		}

		[Test]
		public virtual void TestTextValuesAndChanges()
		{
			string xml = "<root><elem1/><elem2><child/></elem2><elem3>texto</elem3></root>";
			XNode n = new XNode(xml);
			XNode n1 = n.GetChild("elem1");
			// set text values from element
			NUnit.Framework.Assert.AreEqual(string.Empty, n1.InnerText());
			n1.SetInnerText("val1");
			NUnit.Framework.Assert.AreEqual("val1", n1.InnerText());
			n1.SetInnerText("xxx");
			NUnit.Framework.Assert.AreEqual("xxx", n1.InnerText());
			// en nodo que contiene xml, se elimina el contenido quedando solo texto
			XNode n2 = n.GetChild("elem2");
			NUnit.Framework.Assert.AreEqual(string.Empty, n2.InnerText());
			NUnit.Framework.Assert.AreEqual("<elem2><child /></elem2>", n2.OuterXml());
			n2.SetInnerText("replaced");
			NUnit.Framework.Assert.AreEqual("replaced", n2.InnerText());
			// valores y cambios de valor en nodos texto
			XNode n3 = n.GetChild("elem3");
			NUnit.Framework.Assert.AreEqual("texto", n3.InnerText());
			n3 = n3.GetFirstChild();
			NUnit.Framework.Assert.AreEqual("texto", n3.InnerText());
			n3.SetInnerText("nuevo");
			NUnit.Framework.Assert.AreEqual("nuevo", n3.InnerText());
		}

		[Test]
		public virtual void TestTextAttributes()
		{
			string xml = "<root><elem1>txt</elem1></root>";
			XNode n = new XNode(xml).GetChild("elem1").GetFirstChild();
			// set attribute does not have any effect, get returns empty
			n.SetAttribute("none", "empty");
			NUnit.Framework.Assert.AreEqual(string.Empty, n.GetAttribute("none"));
		}

		[Test]
		public virtual void TestEncodingFunctions()
		{
			string text = "a<b>c&d\"e";
			NUnit.Framework.Assert.AreEqual("a&lt;b&gt;c&amp;d\"e", XNode.EncodeText(text));
			NUnit.Framework.Assert.AreEqual("a&lt;b&gt;c&amp;d&quot;e", XNode.EncodeAttribute(text));
			// encoding is reversible
			NUnit.Framework.Assert.AreEqual(text, XNode.DecodeText(XNode.EncodeText(text)));
			NUnit.Framework.Assert.AreEqual(text, XNode.DecodeAttribute(XNode.EncodeAttribute(text)));
			// supports nulls
			NUnit.Framework.Assert.AreEqual(string.Empty, XNode.EncodeText(null));
			NUnit.Framework.Assert.AreEqual(string.Empty, XNode.EncodeAttribute(null));
			NUnit.Framework.Assert.AreEqual(string.Empty, XNode.DecodeText(null));
			NUnit.Framework.Assert.AreEqual(string.Empty, XNode.DecodeAttribute(null));
		}

		[Test]
		public virtual void TestEncodingXNode()
		{
			string xmle = "<root><elem1 aaa=\"a &lt; &gt; &amp; &quot; b\">c &lt; &gt; &amp; &quot; d</elem1><elem2 /></root>";
			XNode n = new XNode(xmle);
			NUnit.Framework.Assert.AreEqual("<root><elem1 aaa=\"a &lt; &gt; &amp; &quot; b\">c &lt; &gt; &amp; \" d</elem1><elem2 /></root>", n.OuterXml());
			XNode n1 = n.GetChild("elem1");
			NUnit.Framework.Assert.AreEqual("c < > & \" d", n1.InnerText());
			NUnit.Framework.Assert.AreEqual("a < > & \" b", n1.GetAttribute("aaa"));
			n1.SetAttribute("aaa", "a \" & > < b");
			NUnit.Framework.Assert.AreEqual("a \" & > < b", n1.GetAttribute("aaa"));
			// other node to check that quote in text is not encoded
			XNode n2 = n.GetChild("elem2");
			n2.SetInnerText("c < > & \" d");
			NUnit.Framework.Assert.AreEqual("c < > & \" d", n2.InnerText());
			NUnit.Framework.Assert.AreEqual("<elem2>c &lt; &gt; &amp; \" d</elem2>", n2.OuterXml());
		}

		[Test]
		public virtual void TestChildAppend()
		{
			string xml = "<root><elem1/></root>";
			XNode n = new XNode(xml);
			// after all children
			XNode newNode = n.AppendChild("newelem");
			NUnit.Framework.Assert.AreEqual("newelem", newNode.Name());
			NUnit.Framework.Assert.AreEqual("<root><elem1 /><newelem /></root>", n.OuterXml());
			// after empty node
			newNode = newNode.AppendChild(newNode.CreateElement("afterempty"));
			NUnit.Framework.Assert.AreEqual("afterempty", newNode.Name());
			NUnit.Framework.Assert.AreEqual("<root><elem1 /><newelem><afterempty /></newelem></root>", n.OuterXml());
			// after text
			n = new XNode("<root>text</root>");
			newNode = n.AppendChild("aftertext");
			NUnit.Framework.Assert.AreEqual("<root>text<aftertext /></root>", n.OuterXml());
			// add text node
			newNode = n.AppendChild(n.CreateText("newtext"));
			NUnit.Framework.Assert.AreEqual("newtext", newNode.InnerText());
			NUnit.Framework.Assert.AreEqual("<root>text<aftertext />newtext</root>", n.OuterXml());
		}
	}
}
