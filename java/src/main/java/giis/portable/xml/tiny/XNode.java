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
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import giis.portable.util.PortableException;

/**
 * Simple wrapper to read xml documents with unified management of text and element nodes.
 * Includes a few methods to manipulate the document
 */
public class XNode extends XNodeAbstract {
	protected Node node = null; // not private to allow extend this class

	public XNode(String xml) {
		Document doc = newDocument(xml);
		this.node = doc.getFirstChild();
	}
	// Not private to be more extensible
	public XNode(Node nativeNode) {
		this.node = nativeNode;
	}
	public Node getNativeNode() {
		return this.node;
	}

	@Override
	public boolean isElement() {
		return this.node.getNodeType() == Node.ELEMENT_NODE;
	}
	@Override
	public boolean isText() {
		return this.node.getNodeType() == Node.TEXT_NODE;
	}
	@Override
	public XNode createElement(String elementName) {
		return new XNode(this.node.getOwnerDocument().createElement(elementName));
	}
	@Override
	public XNode createText(String textValue) {
		return new XNode(this.node.getOwnerDocument().createTextNode(textValue));
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

	@Override
	public String getAttribute(String name) {
		if (!this.isElement()) // text nodes do not have attributes
			return "";
		return ((Element) this.node).getAttribute(name);
	}
	@Override
	public void setAttribute(String name, String value) {
		if (!this.isElement())
			return;
		((Element) this.node).setAttribute(name, value);
	}
	@Override
	public List<String> getAttributeNames() {
		List<String> attrs = new ArrayList<>();
		if (!this.isElement())
			return attrs;
		NamedNodeMap xattrs = this.node.getAttributes();
		for (int i = 0; i < xattrs.getLength(); i++)
			attrs.add(xattrs.item(i).getNodeName());
		Collections.sort(attrs);
		return attrs;
	}

	@Override
	public XNode getFirstChild() {
		return new XNode(this.node.getFirstChild());
	}
	@Override
	public XNode appendChild(String elementName) {
		return appendChild(this.createElement(elementName));
	}
	public XNode appendChild(XNode elementOrTextNode) {
		Node newChild = this.node.appendChild(elementOrTextNode.getNativeNode());
		return new XNode(newChild);
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
		} catch (ParserConfigurationException | SAXException | IOException e) {
			throw new PortableException(e);
		}
	}

	//Returns child element or text nodes as indicated
	//If element, allows search by node name, oherwise, set elementName to null
	@Override
	protected List<XNode> getChildren(boolean returnElements, boolean returnTexts, String elementName, boolean onlyFirst) {
		List<XNode> children = new ArrayList<>();
		Node child = node.getFirstChild();
		while (child != null) {
			if (returnElements && child.getNodeType() == Node.ELEMENT_NODE && (elementName==null || child.getNodeName().equals(elementName))
					|| returnTexts && child.getNodeType() == Node.TEXT_NODE) {
				children.add(new XNode(child));
				if (onlyFirst) // returns the first one that matches
					return children;
			}
			child = child.getNextSibling();
		}
		return children;
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
		if (n.getNodeType() == Node.ELEMENT_NODE && n.hasAttributes()) {
			for (int i = 0; i < n.getAttributes().getLength(); i++) {
				String aName = n.getAttributes().item(i).getNodeName();
				String aValue = n.getAttributes().item(i).getNodeValue();
				attr.append(" " + aName + "=" + "\"" + encodeAttribute(aValue) + "\"");
			}
		}
		return attr.toString();
	}

	/**
	 * Returns the string that represents a node as a document (including the
	 * document header) using a transformer for output; use this method when
	 * transforming large nodes to string.
	 * http://stackoverflow.com/questions/3300839/get-a-nodes-inner-xml-as-string-in-java-dom
	 */
	public String toXmlDocument() {
		try {
			TransformerFactory tFactory = TransformerFactory.newInstance(); // NOSONAR
			Transformer transformer = tFactory.newTransformer();
			DOMSource source = new DOMSource(this.node);
			java.io.StringWriter sw = new java.io.StringWriter();
			StreamResult result = new StreamResult(sw);
			transformer.transform(source, result);
			return sw.toString();
		} catch (TransformerException tce) {
			// Error generated by the parser, use the contained exception, if any
			throw new PortableException(tce.getException() == null ? tce : tce.getException());
		}
	}

}
