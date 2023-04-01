package giis.portable.xml.tiny;

import java.io.IOException;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import javax.xml.XMLConstants;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

/**
 * Simple wrapper to read xml documents with unified management of text and element nodes.
 * Includes a few methods to manipulate the document
 */
public class XNode extends XNodeAbstract {
	private Node node = null;

	public XNode(String xml) {
		Document doc = newDocument(xml);
		this.node = doc.getFirstChild();
	}

	private XNode(Node nativeNode) {
		this.node = nativeNode;
	}

	@Override
	public String getAttribute(String name) {
		if (!isElement(this.node)) // text nodes do not have attributes
			return "";
		return ((Element) this.node).getAttribute(name);
	}
	@Override
	public void setAttribute(String name, String value) {
		if (!isElement(this.node))
			return;
		((Element) this.node).setAttribute(name, value);
	}
	@Override
	public List<String> getAttributeNames() {
		List<String> attrs = new ArrayList<>();
		if (!isElement(this.node))
			return attrs;
		NamedNodeMap xattrs = this.node.getAttributes();
		for (int i = 0; i < xattrs.getLength(); i++)
			attrs.add(xattrs.item(i).getNodeName());
		Collections.sort(attrs);
		return attrs;
	}

	/**
	 * Gets the first child with the specified name, null if it does not exist
	 */
	@Override
	public XNode getChild(String elementName) {
		List<XNode> children = getChildren(elementName, false);
		if (children.size() > 0) // NOSONAR compatibility java 1.4 and C#
			return children.get(0);
		return null;
	}
	/**
	 * Gets a list of all children with the specified name
	 */
	@Override
	public List<XNode> getChildren(String elementName) {
		return getChildren(elementName, true);
	}
	
	@Override
	public XNode getFirstChild() {
		return new XNode(this.node.getFirstChild());
	}

	@Override
	public String name() {
		return this.node.getNodeName();
	}
	@Override
	public String innerText() { // no uso getTextContent pues no es compatible con java 1.4
		if (this.node.getNodeType() == Node.TEXT_NODE)
			return this.node.getNodeValue();
		// elemento, obtiene el texto de todos los nodos recursivamente
		StringBuilder sb = new StringBuilder();
		NodeList nl = this.node.getChildNodes();
		for (int i = 0; i < nl.getLength(); i++)
			sb.append(new XNode(nl.item(i)).innerText());
		return sb.toString();
	}
	@Override
	public void setInnerText(String value) {
		if (this.node.getNodeType() == Node.TEXT_NODE) {
			this.node.setNodeValue(value);
			return;
		}
		// elemento, borra el contenido y anyade el nodo texto con el valor
		NodeList nl = this.node.getChildNodes();
		for (int i = 0; i < nl.getLength(); i++)
			this.node.removeChild(nl.item(i));
		Node newChild = this.node.getOwnerDocument().createTextNode(value);
		this.node.appendChild(newChild);
	}

	@Override
	public XNode appendChild(String elementName) {
		Node newChild = this.node.getOwnerDocument().createElement(elementName);
		this.node.appendChild(newChild);
		return new XNode(newChild);
	}

	@Override
	public String outerXml() {
		return toXmlString(this.node, true);
	}
	@Override
	public String innerXml() {
		return toXmlString(this.node, false);
	}
	@Override
	public String toString() {
		return outerXml();
	}

	// Additional methods for internal use

	/**
	 * Creates a new Document from the xml string
	 */
	public static Document newDocument(String xml) {
		try {
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			factory.setAttribute(XMLConstants.ACCESS_EXTERNAL_DTD, "");
			factory.setAttribute(XMLConstants.ACCESS_EXTERNAL_SCHEMA, "");
			DocumentBuilder builder;
			builder = factory.newDocumentBuilder();
			return builder.parse(new InputSource(new StringReader(xml)));
		} catch (ParserConfigurationException e1) {
			throw new RuntimeException(e1); // NOSONAR
		} catch (SAXException e2) {
			throw new RuntimeException(e2); // NOSONAR
		} catch (IOException e3) {
			throw new RuntimeException(e3); // NOSONAR
		}
	}

	private List<XNode> getChildren(String elementName, boolean returnAll) {
		List<XNode> children = new ArrayList<>();
		Node child = node.getFirstChild();
		while (child != null) {
			if (isElement(child) && child.getNodeName().equals(elementName)) {
				children.add(new XNode(child));
				if (!returnAll) // returns the first one that matches
					return children;
			}
			child = child.getNextSibling();
		}
		return children;
	}

	private boolean isElement(Node n) {
		return n.getNodeType() == Node.ELEMENT_NODE;
	}

	private String toXmlString(Node n, boolean viewRoot) {
		String nodeName = n.getNodeName();
		switch (n.getNodeType()) {
		case Node.TEXT_NODE:
			if (viewRoot)
				return encodeText(n.getNodeValue());
			else
				return "";
		case Node.ELEMENT_NODE:
			String attr = getAttributesString(n);
			if (n.hasChildNodes()) {
				return getChildrenRecursive(n, nodeName, attr, viewRoot);
			} else {
				if (viewRoot)
					return "<" + nodeName + attr + " />";
				else
					return "";
			}
		default:
			return ""; // ignora otros tipos de nodo
		}
	}

	private String getChildrenRecursive(Node n, String nodeName, String attr, boolean viewRoot) {
		StringBuilder sb = new StringBuilder();
		// open tag
		if (viewRoot)
			sb.append("<" + nodeName + attr + ">");
		// recursive lookup
		Node child = n.getFirstChild();
		while (child != null) {
			sb.append(toXmlString(child, true));
			child = child.getNextSibling();
		}
		// close tag
		if (viewRoot)
			sb.append("</" + nodeName + ">");
		return sb.toString();
	}

	private String getAttributesString(Node n) {
		StringBuilder attr = new StringBuilder();
		if (isElement(n) && n.hasAttributes()) {
			for (int i = 0; i < n.getAttributes().getLength(); i++) {
				String aName = n.getAttributes().item(i).getNodeName();
				String aValue = n.getAttributes().item(i).getNodeValue();
				attr.append(" " + aName + "=" + "\"" + encodeAttribute(aValue) + "\"");
			}
		}
		return attr.toString();
	}

}
