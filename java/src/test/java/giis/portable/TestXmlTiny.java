package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.fail;

import java.util.List;

import org.junit.Test;

import giis.portable.util.JavaCs;
import giis.portable.xml.tiny.XNode;

public class TestXmlTiny {
	
	private static final String XML2 =
			"<elem2>  \n" + 
			"	\t<nested>text1</nested>\n" + 
			"	\t<other>textother</other>\n" + 
			"	\t<nested>text2</nested>\n" + 
			"  </elem2>";
	//note that XML1 contains XML2
	private static final String XML1 =
			"<noderoot>\r\n" + 
			"  <elem1 />  \n" + 
			"  " + XML2 + "\n" + 
			"  <elem3>text3</elem3>  \n" + 
			"</noderoot>\n ";

	@Test
	public void testDocumentRoot() {
		XNode n = new XNode("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n   \n<noderoot><elem1/></noderoot>   ");
		assertEquals("noderoot", n.name());
		assertEquals("elem1", n.getFirstChild().name());
		assertEquals("<noderoot><elem1 /></noderoot>", n.outerXml());
	}

	@Test
	public void testElementFindChildAndGetContent() {
		// also checks text and xml content
		XNode n = new XNode(XML1);
		assertEquals("noderoot", n.name());

		// leaf element
		XNode n1 = n.getChild("elem1");
		assertEquals("elem1", n1.name());
		assertEquals("<elem1 />", n1.outerXml());
		assertEquals("", n1.innerXml());
		assertEquals("", n1.innerText());

		// with only text
		XNode n3 = n.getChild("elem3");
		assertEquals("elem3", n3.name());
		assertEquals("<elem3>text3</elem3>", n3.outerXml());
		assertEquals("text3", n3.innerXml());
		assertEquals("text3", n3.innerText());

		// child does not exist
		XNode nn = n.getChild("doesnotexist");
		assertNull(nn);
	}
	@Test
	public void testElementFindChildNestedAndGetContent() {
		XNode n = new XNode(XML1).getChild("elem2");

		// with xml
		assertEquals("elem2", n.name());
		assertEquals(XML2, n.outerXml());
		assertEquals(XML2.replace("</elem2>", "").replace("<elem2>", ""), n.innerXml());
		// innerText removes tags, but preserves everything, including whitespaces
		String n2text = XML2.replace("</elem2>", "").replace("<elem2>", "").replace("</nested>", "")
				.replace("<nested>", "").replace("</other>", "").replace("<other>", "");
		assertEquals(n2text, n.innerText());

		// the first element is selected if more than one match
		XNode xn = n.getChild("nested");
		assertEquals("<nested>text1</nested>", xn.outerXml());
	}
	@Test
	public void testElementFindChildren() {
		XNode n = new XNode(XML1);
		List<XNode> n1 = n.getChildren("elem1");
		assertEquals(1, n1.size());
		assertEquals("elem1", n1.get(0).name());

		List<XNode> n2 = n.getChildren("elem2");
		assertEquals(1, n2.size());
		List<XNode> n21 = n.getChild("elem2").getChildren("nested");
		assertEquals(2, n21.size());
		assertEquals("<nested>text1</nested>", n21.get(0).outerXml());
		assertEquals("<nested>text2</nested>", n21.get(1).outerXml());
		assertEquals(0, n21.get(0).getChildren("text1").size()); // text node, no children

		List<XNode> nn = n.getChildren("doesnotexist");
		assertEquals(0, nn.size());
	}

	@Test
	public void testElementAttributes() { // TODO atributos sobre nodo texto
		String xml = "<root><elem1 aaa=\"text1\" bbb=\"3\" /><elem2 /></root>";
		XNode n = new XNode(xml);
		XNode n1 = n.getChild("elem1");
		assertEquals("text1", n1.getAttribute("aaa"));
		assertEquals("", n1.getAttribute("doesnotexist")); // default is empty
		assertEquals("3", n1.getAttribute("bbb"));
		assertEquals(xml, n.outerXml());

		XNode n2 = n.getChild("elem2");
		n2.setAttribute("xxx", "new");
		assertEquals("new", n2.getAttribute("xxx"));
		assertEquals("<elem2 xxx=\"new\" />", n2.outerXml());
	}
	@Test
	public void testElementAttributesAsInt() {
		String xml = "<root><elem1 aaa=\"text1\" bbb=\"3\"/><elem2/></root>";
		XNode n1 = new XNode(xml).getChild("elem1");
		assertEquals(3, n1.getIntAttribute("bbb"));

		assertEquals(0, n1.getIntAttribute("notexistsint")); // default is 0
		n1.incrementIntAttribute("bbb", 2);
		assertEquals(5, n1.getIntAttribute("bbb"));

		n1.incrementIntAttribute("notexisted", 32); // default specified
		assertEquals(32, n1.getIntAttribute("notexisted"));

		try {
			n1.getIntAttribute("aaa");
			fail("Should fail");
		} catch (RuntimeException e) {
		}
	}
	@Test
	public void testElementAttributeNames() {
		String xml = "<root><elem1 bbb=\"val2\" ccc=\"val3\" aaa=\"val1\" /><elem2/></root>";
		XNode n = new XNode(xml).getChild("elem1");
		assertEquals("[aaa, bbb, ccc]", JavaCs.deepToString(JavaCs.toArray(n.getAttributeNames())));

		// without attributes
		n = new XNode("<root><elem1>text</elem1></root>").getChild("elem1");
		assertEquals("[]", JavaCs.deepToString(JavaCs.toArray(n.getAttributeNames())));

		// is text
		n = n.getFirstChild();
		assertEquals("[]", JavaCs.deepToString(JavaCs.toArray(n.getAttributeNames())));
	}

	@Test
	public void testTextValuesAndChanges() {
		String xml = "<root><elem1/><elem2><child/></elem2><elem3>texto</elem3></root>";
		XNode n = new XNode(xml);
		XNode n1 = n.getChild("elem1");
		// set text values from element
		assertEquals("", n1.innerText());
		n1.setInnerText("val1");
		assertEquals("val1", n1.innerText());
		n1.setInnerText("xxx");
		assertEquals("xxx", n1.innerText());

		// en nodo que contiene xml, se elimina el contenido quedando solo texto
		XNode n2 = n.getChild("elem2");
		assertEquals("", n2.innerText());
		assertEquals("<elem2><child /></elem2>", n2.outerXml());
		n2.setInnerText("replaced");
		assertEquals("replaced", n2.innerText());

		// valores y cambios de valor en nodos texto
		XNode n3 = n.getChild("elem3");
		assertEquals("texto", n3.innerText());
		n3 = n3.getFirstChild();
		assertEquals("texto", n3.innerText());
		n3.setInnerText("nuevo");
		assertEquals("nuevo", n3.innerText());
	}
	@Test
	public void testTextAttributes() {
		String xml = "<root><elem1>txt</elem1></root>";
		XNode n = new XNode(xml).getChild("elem1").getFirstChild();
		// set attribute does not have any effect, get returns empty
		n.setAttribute("none", "empty");
		assertEquals("", n.getAttribute("none"));
	}

	@Test
	public void testEncodingFunctions() {
		String text = "a<b>c&d\"e";
		assertEquals("a&lt;b&gt;c&amp;d\"e", XNode.encodeText(text));
		assertEquals("a&lt;b&gt;c&amp;d&quot;e", XNode.encodeAttribute(text));
		// encoding is reversible
		assertEquals(text, XNode.decodeText(XNode.encodeText(text)));
		assertEquals(text, XNode.decodeAttribute(XNode.encodeAttribute(text)));
		// supports nulls
		assertEquals("", XNode.encodeText(null));
		assertEquals("", XNode.encodeAttribute(null));
		assertEquals("", XNode.decodeText(null));
		assertEquals("", XNode.decodeAttribute(null));
	}
	@Test
	public void testEncodingXNode() {
		String xmle = "<root><elem1 aaa=\"a &lt; &gt; &amp; &quot; b\">c &lt; &gt; &amp; &quot; d</elem1><elem2 /></root>";
		XNode n = new XNode(xmle);
		assertEquals("<root><elem1 aaa=\"a &lt; &gt; &amp; &quot; b\">c &lt; &gt; &amp; \" d</elem1><elem2 /></root>",
				n.outerXml());

		XNode n1 = n.getChild("elem1");
		assertEquals("c < > & \" d", n1.innerText());

		assertEquals("a < > & \" b", n1.getAttribute("aaa"));
		n1.setAttribute("aaa", "a \" & > < b");
		assertEquals("a \" & > < b", n1.getAttribute("aaa"));

		// other node to check that quote in text is not encoded
		XNode n2 = n.getChild("elem2");
		n2.setInnerText("c < > & \" d");
		assertEquals("c < > & \" d", n2.innerText());
		assertEquals("<elem2>c &lt; &gt; &amp; \" d</elem2>", n2.outerXml());
	}

	@Test
	public void testChildAppend() {
		String xml = "<root><elem1/></root>";
		XNode n = new XNode(xml);
		// after all children
		XNode newNode = n.appendChild("newelem");
		assertEquals("newelem", newNode.name());
		assertEquals("<root><elem1 /><newelem /></root>", n.outerXml());
		// after empty node
		newNode = newNode.appendChild("afterempty");
		assertEquals("<root><elem1 /><newelem><afterempty /></newelem></root>", n.outerXml());
		// after text
		n = new XNode("<root>text</root>");
		newNode = n.appendChild("aftertext");
		assertEquals("<root>text<aftertext /></root>", n.outerXml());
	}

}
