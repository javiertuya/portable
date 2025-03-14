using Java.Util;
using NUnit.Framework;
using Giis.Portable.Util;
using Giis.Portable.Xml.Tiny;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Portable
{
    public class TestXmlTiny
    {
        //using spaces to check that is not whitespace aware
        private static readonly string XML2 = "<elem2>  \n" + "\t\t<nested>text1</nested>\n" + "\t\t<other>textother</other>\n" + "\t\t<nested>text2</nested>\n" + "  </elem2>";
        //note that XML1 contains XML2
        private static readonly string XML1 = "<noderoot>\r\n" + "  <elem1 />  \n" + "  " + XML2 + "\n" + "  <elem3>text3</elem3>  \n" + "</noderoot>\n ";
        [Test]
        public virtual void TestDocumentRoot()
        {
            XNode n = new XNode("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n \t \r<noderoot><elem1/></noderoot>   ");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("noderoot", n.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n.FirstChild().Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<noderoot><elem1 /></noderoot>", n.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<noderoot><elem1 /></noderoot>", n.ToString());

            //as xml document (native) gets the header, with little different element formatting
            if (Parameters.IsJava())
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><noderoot><elem1/></noderoot>", n.ToXmlDocument());
        }

        [Test]
        public virtual void TestFindChildren()
        {
            XNode n = new XNode("<noderoot><!-- comment --><elem1>text1</elem1>text2<elem3 /> \n </noderoot>");

            // by default, only element nodes
            IList<XNode> n1 = n.GetChildren();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, n1.Count);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(n1[0].IsElement());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n1[0].IsText());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n1[0].IsWhitespace());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n1[0].Name());
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(n1[1].IsElement());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem3", n1[1].Name());

            // but can get all too (only text and element, comments and blanks are ignored)
            n1 = n.GetChildrenWithText();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(3, n1.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n1[0].Name());
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(n1[1].IsText());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n1[1].IsElement());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n1[1].IsWhitespace());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text2", n1[1].InnerText());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem3", n1[2].Name());
        }

        [Test]
        public virtual void TestElementFindChildAndGetContent()
        {

            // also checks text and xml content
            XNode n = new XNode(XML1);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("noderoot", n.Name());

            // leaf element
            XNode n1 = n.GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n1.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem1 />", n1.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n1.InnerXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n1.InnerText());

            // with only text
            XNode n3 = n.GetChild("elem3");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem3", n3.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem3>text3</elem3>", n3.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text3", n3.InnerXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text3", n3.InnerText());

            // value() is null for elements
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(null, n3.Value());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text3", n3.FirstChild().Value());

            // setter no effect for elements
            n3.SetValue("xyz");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem3>text3</elem3>", n3.OuterXml());
            n3.FirstChild().SetValue("abc");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem3>abc</elem3>", n3.OuterXml());

            // child does not exist
            XNode nn = n.GetChild("doesnotexist");
            NUnit.Framework.Legacy.ClassicAssert.IsNull(nn);
        }

        [Test]
        public virtual void TestElementFindChildNestedAndGetContent()
        {
            XNode n = new XNode(XML1).GetChild("elem2");

            // with xml
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem2", n.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(XML2, n.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(XML2.Replace("</elem2>", "").Replace("<elem2>", ""), n.InnerXml());

            // innerText removes tags, but preserves everything, including whitespaces
            string n2text = XML2.Replace("</elem2>", "").Replace("<elem2>", "").Replace("</nested>", "").Replace("<nested>", "").Replace("</other>", "").Replace("<other>", "");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(n2text, n.InnerText());

            // the first element is selected if more than one match
            XNode xn = n.GetChild("nested");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<nested>text1</nested>", xn.OuterXml());
        }

        [Test]
        public virtual void TestElementFindChildrenByName()
        {
            XNode n = new XNode(XML1);
            IList<XNode> n1 = n.GetChildren("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, n1.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n1[0].Name());
            IList<XNode> n2 = n.GetChildren("elem2");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, n2.Count);
            IList<XNode> n21 = n.GetChild("elem2").GetChildren("nested");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, n21.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<nested>text1</nested>", n21[0].OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<nested>text2</nested>", n21[1].OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, n21[0].GetChildren("text1").Count); // text node, no children
            IList<XNode> nn = n.GetChildren("doesnotexist");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, nn.Count);
        }

        [Test]
        public virtual void TestNavigateSiblings()
        {
            string xml = "<root> \n <elem1/>text</root>";
            XNode n = new XNode(xml).FirstChild();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n.Name());
            NUnit.Framework.Legacy.ClassicAssert.IsNull(n.PreviousSibling());
            n = n.NextSibling();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text", n.InnerText());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem1", n.PreviousSibling().Name());
            n = n.NextSibling();
            NUnit.Framework.Legacy.ClassicAssert.IsNull(n);
        }

        [Test]
        public virtual void TestNavigateUpDown()
        {
            string xml = "<root><elem1><elem2 /></elem1></root>";
            XNode n = new XNode(xml);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(n.IsRoot());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n.FirstChild().IsRoot());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n.FirstChild().FirstChild().IsRoot());
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(n.FirstChild().FirstChild().Root().IsRoot());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(xml, n.FirstChild().FirstChild().Root().OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(xml, n.FirstChild().Parent().OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.IsNull(n.Parent());

            // navigating up/down returns the same node
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(n.Equals(n.FirstChild().FirstChild().Root()));
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n.Equals(n.FirstChild().FirstChild()));
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n.Equals(null));
        }

        [Test]
        public virtual void TestElementAttributesRead()
        {
            string xml = "<root><elem1 bbb=\"text1\" aaa=\"3\" /><elem2 /></root>";
            XNode n = new XNode(xml);
            XNode n1 = n.GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text1", n1.GetAttribute("bbb"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n1.GetAttribute("doesnotexist")); // default is empty
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("3", n1.GetAttribute("aaa"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(" aaa=\"3\" bbb=\"text1\"", n1.GetAttributesString()); // sorted
        }

        [Test]
        public virtual void TestElementAttributesUpdate()
        {
            XNode n = new XNode("<root><elem/></root>");
            XNode n2 = n.GetChild("elem");
            n2.SetAttribute("xxx", "new1");
            n2.SetAttribute("yyy", "new2");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("new1", n2.GetAttribute("xxx"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("new2", n2.GetAttribute("yyy"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem xxx=\"new1\" yyy=\"new2\" />", n2.OuterXml());
            n2.RemoveAttribute("yyy");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem xxx=\"new1\" />", n2.OuterXml());
            n2.RemoveAttribute("xxx");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem />", n2.OuterXml());
            n2.RemoveAttribute("doesnotexist");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem />", n2.OuterXml());
        }

        [Test]
        public virtual void TestElementAttributesAsInt()
        {
            string xml = "<root><elem1 aaa=\"text1\" bbb=\"3\"/><elem2/></root>";
            XNode n1 = new XNode(xml).GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(3, n1.GetIntAttribute("bbb"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, n1.GetIntAttribute("notexistsint")); // default is 0
            n1.IncrementIntAttribute("bbb", 2);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(5, n1.GetIntAttribute("bbb"));
            n1.IncrementIntAttribute("notexisted", 32); // default specified
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(32, n1.GetIntAttribute("notexisted"));
            NUnit.Framework.Assert.Throws(Is.InstanceOf(typeof(Exception)), () =>
            {
                n1.GetIntAttribute("aaa");
            });
        }

        [Test]
        public virtual void TestElementAttributesAsElement()
        {
            string xml = "<root><elem1><aaa>text1</aaa><bbb>text2</bbb><bbb>text3</bbb></elem1></root>";
            XNode n1 = new XNode(xml).GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text1", n1.GetElementAtribute("aaa"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text2", n1.GetElementAtribute("bbb")); // first if there are more than one element
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n1.GetElementAtribute("doesnotexist"));
        }

        [Test]
        public virtual void TestElementAttributeNames()
        {
            string xml = "<root><elem1 bbb=\"val2\" ccc=\"val3\" aaa=\"val1\" /><elem2/></root>";
            XNode n = new XNode(xml).GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[aaa, bbb, ccc]", JavaCs.DeepToString(JavaCs.ToArray(n.GetAttributeNames())));

            // without attributes
            n = new XNode("<root><elem1>text</elem1></root>").GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[]", JavaCs.DeepToString(JavaCs.ToArray(n.GetAttributeNames())));

            // is text
            n = n.FirstChild();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[]", JavaCs.DeepToString(JavaCs.ToArray(n.GetAttributeNames())));
        }

        [Test]
        public virtual void TestTextValuesAndChanges()
        {
            string xml = "<root><elem1/><elem2><child/></elem2><elem3>texto</elem3></root>";
            XNode n = new XNode(xml);
            XNode n1 = n.GetChild("elem1");

            // set text values from element
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n1.InnerText());
            n1.SetInnerText("val1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("val1", n1.InnerText());
            n1.SetInnerText("xxx");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", n1.InnerText());

            // en nodo que contiene xml, se elimina el contenido quedando solo texto
            XNode n2 = n.GetChild("elem2");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n2.InnerText());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem2><child /></elem2>", n2.OuterXml());
            n2.SetInnerText("replaced");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("replaced", n2.InnerText());

            // valores y cambios de valor en nodos texto
            XNode n3 = n.GetChild("elem3");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("texto", n3.InnerText());
            n3 = n3.FirstChild();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("texto", n3.InnerText());
            n3.SetInnerText("nuevo");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("nuevo", n3.InnerText());
        }

        [Test]
        public virtual void TestTextAttributes()
        {
            string xml = "<root><elem1>txt</elem1></root>";
            XNode n = new XNode(xml).GetChild("elem1").FirstChild();

            // set attribute does not have any effect, get returns empty
            n.SetAttribute("none", "empty");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", n.GetAttribute("none"));
        }

        [Test]
        public virtual void TestClone()
        {
            string xml = "<root><elem1>txt</elem1></root>";
            XNode n = new XNode(xml);
            XNode clon = n.CloneNode();

            // Same contents, but nodes are not the same
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(xml, clon.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(n.Equals(clon));
        }

        [Test]
        public virtual void TestEncodingFunctions()
        {
            string text = "a<b>c&d\"e";
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("a&lt;b&gt;c&amp;d\"e", XNode.EncodeText(text));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("a&lt;b&gt;c&amp;d&quot;e", XNode.EncodeAttribute(text));

            // encoding is reversible
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(text, XNode.DecodeText(XNode.EncodeText(text)));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(text, XNode.DecodeAttribute(XNode.EncodeAttribute(text)));

            // supports nulls
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", XNode.EncodeText(null));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", XNode.EncodeAttribute(null));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", XNode.DecodeText(null));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", XNode.DecodeAttribute(null));
        }

        [Test]
        public virtual void TestEncodingXNode()
        {
            string xmle = "<root><elem1 aaa=\"a &lt; &gt; &amp; &quot; b\">c &lt; &gt; &amp; &quot; d</elem1><elem2 /></root>";
            XNode n = new XNode(xmle);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><elem1 aaa=\"a &lt; &gt; &amp; &quot; b\">c &lt; &gt; &amp; \" d</elem1><elem2 /></root>", n.OuterXml());
            XNode n1 = n.GetChild("elem1");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("c < > & \" d", n1.InnerText());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("a < > & \" b", n1.GetAttribute("aaa"));
            n1.SetAttribute("aaa", "a \" & > < b");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("a \" & > < b", n1.GetAttribute("aaa"));

            // other node to check that quote in text is not encoded
            XNode n2 = n.GetChild("elem2");
            n2.SetInnerText("c < > & \" d");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("c < > & \" d", n2.InnerText());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<elem2>c &lt; &gt; &amp; \" d</elem2>", n2.OuterXml());
        }

        [Test]
        public virtual void TestChildAppend()
        {
            string xml = "<root><elem1/></root>";
            XNode n = new XNode(xml);

            // after all children
            XNode newNode = n.AppendChild("newelem");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("newelem", newNode.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><elem1 /><newelem /></root>", n.OuterXml());

            // after empty node
            newNode = newNode.AppendChild(newNode.CreateElement("afterempty"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("afterempty", newNode.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><elem1 /><newelem><afterempty /></newelem></root>", n.OuterXml());

            // after text
            n = new XNode("<root>text</root>");
            newNode = n.AppendChild("aftertext");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root>text<aftertext /></root>", n.OuterXml());

            // add text node
            newNode = n.AppendChild(n.CreateText("newtext"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("newtext", newNode.InnerText());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root>text<aftertext />newtext</root>", n.OuterXml());
        }

        [Test]
        public virtual void TestChildPrepend()
        {
            XNode n = new XNode("<root><elem1/></root>");
            n.PrependChild(n.CreateElement("new"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><new /><elem1 /></root>", n.OuterXml());
            n = new XNode("<root></root>");
            n.PrependChild(n.CreateElement("new"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><new /></root>", n.OuterXml());
        }

        [Test]
        public virtual void TestChildInsert()
        {
            string xml = "<root><elem1/>txt<elem2/></root>";
            XNode n = new XNode(xml);
            XNode n1 = n.FirstChild();
            XNode n2 = n.FirstChild().NextSibling().NextSibling();
            n1.InsertAfter(n.CreateElement("e1a"));
            n1.InsertBefore(n.CreateElement("e1b"));
            n2.InsertAfter("e2a");
            n2.InsertBefore("e2b");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><e1b /><elem1 /><e1a />txt<e2b /><elem2 /><e2a /></root>", n.OuterXml());
        }

        [Test]
        public virtual void TestChildReplace()
        {
            string xml = "<root><elem1/>txt<elem2/></root>";
            XNode n = new XNode(xml);
            XNode n2 = n.FirstChild().NextSibling().NextSibling();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("elem2", n2.Name());
            n.ReplaceChild(n.CreateNode("<new>newtext</new>"), n2);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("new", n.FirstChild().NextSibling().NextSibling().Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root><elem1 />txt<new>newtext</new></root>", n.OuterXml());
        }

        [Test]
        public virtual void TestNormalize()
        {
            string xml = "<root></root>";
            XNode n = new XNode(xml);
            n.AppendChild(n.CreateText("text1"));
            n.AppendChild(n.CreateText("text2"));

            //Consecutive text nodes are seen as one, but are different nodes
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root>text1text2</root>", n.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, n.ChildCount());

            //after normalize are only one node
            n.Normalize();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root>text1text2</root>", n.OuterXml());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, n.ChildCount());
        }

        [Test]
        public virtual void TestChildRemoveAll()
        {
            string xml = "<root><elem1/>txt</root>";
            XNode n = new XNode(xml);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, n.ChildCount());

            // remove all
            n.RemoveChildren();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("root", n.Name());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, n.ChildCount());

            //set to empty, same effect
            n = new XNode(xml);
            n.SetInnerXml("");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("root", n.Name());
        }

        [Test]
        public virtual void TestChildRemoveSingle()
        {

            //remove individual children (element and text)
            string xml = "<root><elem1/>txt</root>";
            XNode n = new XNode(xml);
            n.RemoveChild(n.FirstChild());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<root>txt</root>", n.OuterXml());
            n.RemoveChild(n.FirstChild());
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("root", n.Name());
        }

        [Test]
        public virtual void TestMixedDocuments()
        {
            XNode main = new XNode("<main><elem/></main>");

            //node created under a different document can't be added
            XNode external = new XNode("<ext><subelem/></ext>");
            NUnit.Framework.Assert.Throws(Is.InstanceOf(typeof(Exception)), () =>
            {
                new XNode("<main><elem/></main>").AppendChild(external);
            });

            //But works if the new node is imported
            main = new XNode("<main><elem/></main>");
            main.AppendChild(main.ImportNode(external));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<main><elem /><ext><subelem /></ext></main>", main.OuterXml());

            //or if the new node is created from the main node
            main = new XNode("<main><elem/></main>");
            XNode created = main.CreateNode("<cre><subelem/></cre>");
            main.AppendChild(created);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("<main><elem /><cre><subelem /></cre></main>", main.OuterXml());
        }
    }
}