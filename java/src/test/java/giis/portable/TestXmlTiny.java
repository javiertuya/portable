package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.fail;

import java.util.List;

import org.junit.Test;

import giis.portable.util.JavaCs;
import giis.portable.util.Parameters;
import giis.portable.xml.tiny.XNode;

public class TestXmlTiny {
	
	//using spaces to check that is not whitespace aware
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
		XNode n = new XNode("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n \t \r<noderoot><elem1/></noderoot>   ");
		assertEquals("noderoot", n.name());
		assertEquals("elem1", n.firstChild().name());
		assertEquals("<noderoot><elem1 /></noderoot>", n.outerXml());
		assertEquals("<noderoot><elem1 /></noderoot>", n.toString());
		//as xml document (native) gets the header, with little different element formatting
		if (Parameters.isJava())
			assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?><noderoot><elem1/></noderoot>", n.toXmlDocument());
	}

	@Test
	public void testFindChildren() {
		XNode n = new XNode("<noderoot><!-- comment --><elem1>text1</elem1>text2<elem3 /> \n </noderoot>");
		// by default, only element nodes
		List<XNode> n1 = n.getChildren();
		assertEquals(2, n1.size());
		assertTrue(n1.get(0).isElement());
		assertFalse(n1.get(0).isText());
		assertFalse(n1.get(0).isWhitespace());
		assertEquals("elem1", n1.get(0).name());
		assertTrue(n1.get(1).isElement());
		assertEquals("elem3", n1.get(1).name());

		// but can get all too (only text and element, comments and blanks are ignored)
		n1 = n.getChildrenWithText();
		assertEquals(3, n1.size());
		assertEquals("elem1", n1.get(0).name());
		assertTrue(n1.get(1).isText());
		assertFalse(n1.get(1).isElement());
		assertFalse(n1.get(1).isWhitespace());
		assertEquals("text2", n1.get(1).innerText());
		assertEquals("elem3", n1.get(2).name());
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

		// value() is null for elements
		assertEquals(null, n3.value());
		assertEquals("text3", n3.firstChild().value());
		// setter no effect for elements
		n3.setValue("xyz");
		assertEquals("<elem3>text3</elem3>", n3.outerXml());
		n3.firstChild().setValue("abc");
		assertEquals("<elem3>abc</elem3>", n3.outerXml());
		
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
	public void testElementFindChildrenByName() {
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
	public void testNavigateSiblings() {
		String xml = "<root> \n <elem1/>text</root>";
		XNode n = new XNode(xml).firstChild();
		assertEquals("elem1", n.name());
		assertNull(n.previousSibling());
		n = n.nextSibling();
		assertEquals("text", n.innerText());
		assertEquals("elem1", n.previousSibling().name());
		n = n.nextSibling();
		assertNull(n);
	}
	@Test
	public void testNavigateUpDown() {
		String xml = "<root><elem1><elem2 /></elem1></root>";
		XNode n = new XNode(xml);
		assertTrue(n.isRoot());
		assertFalse(n.firstChild().isRoot());
		assertFalse(n.firstChild().firstChild().isRoot());
		assertTrue(n.firstChild().firstChild().root().isRoot());
		assertEquals(xml, n.firstChild().firstChild().root().outerXml());
		assertEquals(xml, n.firstChild().parent().outerXml());
		assertNull(n.parent());

		// navigating up/down returns the same node
		assertTrue(n.equals(n.firstChild().firstChild().root()));
		assertFalse(n.equals(n.firstChild().firstChild()));
		assertFalse(n.equals(null));
	}

	@Test
	public void testElementAttributesRead() {
		String xml = "<root><elem1 bbb=\"text1\" aaa=\"3\" /><elem2 /></root>";
		XNode n = new XNode(xml);
		XNode n1 = n.getChild("elem1");
		assertEquals("text1", n1.getAttribute("bbb"));
		assertEquals("", n1.getAttribute("doesnotexist")); // default is empty
		assertEquals("3", n1.getAttribute("aaa"));
		assertEquals(" aaa=\"3\" bbb=\"text1\"", n1.getAttributesString()); // sorted
	}
	@Test
	public void testElementAttributesUpdate() {
		XNode n = new XNode("<root><elem/></root>");
		XNode n2 = n.getChild("elem");
		n2.setAttribute("xxx", "new1");
		n2.setAttribute("yyy", "new2");
		assertEquals("new1", n2.getAttribute("xxx"));
		assertEquals("new2", n2.getAttribute("yyy"));
		assertEquals("<elem xxx=\"new1\" yyy=\"new2\" />", n2.outerXml());

		n2.removeAttribute("yyy");
		assertEquals("<elem xxx=\"new1\" />", n2.outerXml());
		n2.removeAttribute("xxx");
		assertEquals("<elem />", n2.outerXml());
		n2.removeAttribute("doesnotexist");
		assertEquals("<elem />", n2.outerXml());
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
	public void testElementAttributesAsElement() {
		String xml = "<root><elem1><aaa>text1</aaa><bbb>text2</bbb><bbb>text3</bbb></elem1></root>";
		XNode n1 = new XNode(xml).getChild("elem1");
		assertEquals("text1", n1.getElementAtribute("aaa"));
		assertEquals("text2", n1.getElementAtribute("bbb")); // first if there are more than one element
		assertEquals("", n1.getElementAtribute("doesnotexist"));
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
		n = n.firstChild();
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
		n3 = n3.firstChild();
		assertEquals("texto", n3.innerText());
		n3.setInnerText("nuevo");
		assertEquals("nuevo", n3.innerText());
	}
	@Test
	public void testTextAttributes() {
		String xml = "<root><elem1>txt</elem1></root>";
		XNode n = new XNode(xml).getChild("elem1").firstChild();
		// set attribute does not have any effect, get returns empty
		n.setAttribute("none", "empty");
		assertEquals("", n.getAttribute("none"));
	}

	@Test
	public void testClone() {
		String xml = "<root><elem1>txt</elem1></root>";
		XNode n = new XNode(xml);
		XNode clon = n.cloneNode();
		// Same contents, but nodes are not the same
		assertEquals(xml, clon.outerXml());
		assertFalse(n.equals(clon));
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
		newNode = newNode.appendChild(newNode.createElement("afterempty"));
		assertEquals("afterempty", newNode.name());
		assertEquals("<root><elem1 /><newelem><afterempty /></newelem></root>", n.outerXml());
		// after text
		n = new XNode("<root>text</root>");
		newNode = n.appendChild("aftertext");
		assertEquals("<root>text<aftertext /></root>", n.outerXml());
		// add text node
		newNode = n.appendChild(n.createText("newtext"));
		assertEquals("newtext", newNode.innerText());
		assertEquals("<root>text<aftertext />newtext</root>", n.outerXml());
	}

	@Test
	public void testChildPrepend() {
		XNode n = new XNode("<root><elem1/></root>");
		n.prependChild(n.createElement("new"));
		assertEquals("<root><new /><elem1 /></root>", n.outerXml());
		n = new XNode("<root></root>");
		n.prependChild(n.createElement("new"));
		assertEquals("<root><new /></root>", n.outerXml());
	}
	@Test
	public void testChildInsert() {
		String xml = "<root><elem1/>txt<elem2/></root>";
		XNode n = new XNode(xml);
		XNode n1 = n.firstChild();
		XNode n2 = n.firstChild().nextSibling().nextSibling();
		n1.insertAfter(n.createElement("e1a"));
		n1.insertBefore(n.createElement("e1b"));
		n2.insertAfter("e2a");
		n2.insertBefore("e2b");
		assertEquals("<root><e1b /><elem1 /><e1a />txt<e2b /><elem2 /><e2a /></root>", n.outerXml());
	}
	
	@Test
	public void testChildReplace() {
		String xml = "<root><elem1/>txt<elem2/></root>";
		XNode n = new XNode(xml);
		XNode n2 = n.firstChild().nextSibling().nextSibling();
		assertEquals("elem2", n2.name());
		n.replaceChild(n.createNode("<new>newtext</new>"), n2);
		assertEquals("new", n.firstChild().nextSibling().nextSibling().name());
		assertEquals("<root><elem1 />txt<new>newtext</new></root>", n.outerXml());
	}
	@Test
	public void testNormalize() {
		String xml = "<root></root>";
		XNode n = new XNode(xml);
		n.appendChild(n.createText("text1"));
		n.appendChild(n.createText("text2"));
		//Consecutive text nodes are seen as one, but are different nodes
		assertEquals("<root>text1text2</root>", n.outerXml());
		assertEquals(2, n.childCount());
		//after normalize are only one node
		n.normalize();
		assertEquals("<root>text1text2</root>", n.outerXml());
		assertEquals(1, n.childCount());
	}
	
	@Test
	public void testChildRemoveAll() {
		String xml = "<root><elem1/>txt</root>";
		XNode n = new XNode(xml);
		assertEquals(2, n.childCount());
		// remove all
		n.removeChildren();
		assertEquals("root", n.name());
		assertEquals(0, n.childCount());
		
		//set to empty, same effect
		n = new XNode(xml);
		n.setInnerXml("");
		assertEquals("root", n.name());
	}
	
	@Test
	public void testChildRemoveSingle() {
		//remove individual children (element and text)
		String xml = "<root><elem1/>txt</root>";
		XNode n = new XNode(xml);
		n.removeChild(n.firstChild());
		assertEquals("<root>txt</root>", n.outerXml());
		n.removeChild(n.firstChild());
		assertEquals("root", n.name());
	}
	
	@Test
	public void testMixedDocuments() {
		XNode main=new XNode("<main><elem/></main>");
		
		//node created under a different document can't be added
		XNode external=new XNode("<ext><subelem/></ext>");
		try {
			main.appendChild(external);
			fail("Should fail");
		} catch (RuntimeException e) {
		}
		//But works if the new node is imported
		main=new XNode("<main><elem/></main>");
		main.appendChild(main.importNode(external));
		assertEquals("<main><elem /><ext><subelem /></ext></main>", main.outerXml());
		
		//or if the new node is created from the main node
		main=new XNode("<main><elem/></main>");
		XNode created=main.createNode("<cre><subelem/></cre>");
		main.appendChild(created);
		assertEquals("<main><elem /><cre><subelem /></cre></main>", main.outerXml());
	}

}
