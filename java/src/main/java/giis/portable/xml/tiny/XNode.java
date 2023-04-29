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
	private XNode newXNode(Node nativeNode) {
		return nativeNode==null ? null : new XNode(nativeNode);
	}

	@Override
	public boolean equals(XNode xn) { //NOSONAR
		/*mismo codigo que en .NET*/
		if (this.node==null || xn==null || xn.getNativeNode()==null) 
			return false;
		return this.node.equals(xn.getNativeNode());
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
	public boolean isWhitespace() {
		return isBlank(this.node);
	}
	private boolean isBlank(Node n) {
		if (n == null)
			return false;
		if (n.getNodeType() == Node.TEXT_NODE) { // nodo texto, si algun caracter es no blanco finaliza con false
			String nodeText = n.getNodeValue();
			for (int i = 0; i < nodeText.length(); i++)
				if (nodeText.charAt(i) != ' ' && nodeText.charAt(i) != '\n' && nodeText.charAt(i) != '\r'
						&& nodeText.charAt(i) != '\t')
					return false;
			return true; // si llega aqui el contenido es solamente blancos (o es una cadena vacia)
		} else // nodo no texto, falso
			return false;
	}
	
	@Override
	public XNode createElement(String elementName) {
		return newXNode(this.node.getOwnerDocument().createElement(elementName));
	}

	@Override
	public XNode createText(String textValue) {
		return newXNode(this.node.getOwnerDocument().createTextNode(textValue));
	}

	@Override
	public String name() {
		return this.node.getNodeName();
	}
	@Override
	public String value() {
		return this.node.getNodeValue();
	}
	@Override
	public void setValue(String value) {
		this.node.setNodeValue(value);
	}

	@Override
	public String innerText() { // no uso getTextContent pues no es compatible con java 1.4
		if (this.node.getNodeType() == Node.TEXT_NODE)
			return this.node.getNodeValue();
		// elemento, obtiene el texto de todos los nodos recursivamente
		StringBuilder sb = new StringBuilder();
		NodeList nl = this.node.getChildNodes();
		for (int i = 0; i < nl.getLength(); i++)
			sb.append(newXNode(nl.item(i)).innerText());
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
		//for (int i = 0; i < nl.getLength(); i++)
		for (int i=nl.getLength()-1; i>=0; i--)
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
	public void setInnerXml(String value) { 
		//excepcion si es un nodo de tipo texto
		if (this.node.getNodeType()==Node.TEXT_NODE)
			throw new XmlException("XNode.setInnerXml: Only allowed for Element nodes");
		//primero borra el contenido del nodo actual
		//lo hace desde el final hasta el principio por si acaso
		removeChildren();
		//si no hay que introducir ningun valor finaliza aqui
		if (value.equals("")) 
			return;
		//crea un nuevo documento para extraer de el todos los nodos
		Document d=newDocument("<document>"+value+"</document>");
		Node n=d.getFirstChild();
		NodeList nlist=n.getChildNodes();
		// ahora copia cada hijo de estos al destino
		for (int i=0; i<nlist.getLength(); i++) {
			//da igual que sean nodos en blanco, paso todos para evitar problemas ya
			//que adoptNode funciona eliminando primero el original, y esto podria invalidar la iteracion?
			Node adopted=this.node.getOwnerDocument().importNode(nlist.item(i),true);
			this.node.appendChild(adopted);
		}
	}
	public void removeChildren() {
		while (this.node.getFirstChild()!=null)
			this.node.removeChild(this.node.getFirstChild());
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
	public String getAttributesString() {
		return getAttributesString(this.node);
	}

	@Override
	public XNode cloneNode() {
		return newXNode(this.node.cloneNode(true));
	}
	/**
	 * Returns the node after being imported into the Document of current node.
	 * Avoids exceptions like:
	 * WRONG_DOCUMENT_ERR: A node is used in a different document than the one that created it.
	 */
	@Override
	public XNode importNode(XNode source) {
		Node xnn=source.getNativeNode();
		Node xni=this.node.getOwnerDocument().importNode(xnn, true);
		return newXNode(xni);
	}

	/**
	 * Creates a new Node from a xml string: the difference with constructor is that
	 * the new node is created in the context of the Document to which the current
	 * node belongs (this avoids importing nodes)
	 */
	@Override
	public XNode createNode(String xml) {
		XNode n = this.createElement("newnode");
		n.setInnerXml(xml);
		return n.firstChild();
	}
	
	@Override
	public void normalize() {
		this.node.normalize();
	}
	
	@Override
	public XNode removeChild(XNode oldChild) {
		return newXNode(this.node.removeChild(oldChild.getNativeNode()));
	}
	@Override
	public XNode appendChild(XNode elementOrTextNode) {
		Node newChild = this.node.appendChild(elementOrTextNode.getNativeNode());
		return newXNode(newChild);
	}
	@Override
	public XNode prependChild(XNode node) {
		XNode first = this.firstChild();
		if (first == null)
			return this.appendChild(node);
		else
			return first.insertBefore(node);
	}
	@Override
	public XNode replaceChild(XNode newChild, XNode oldChild) {
		return newXNode(this.node.replaceChild(newChild.node, oldChild.node));
	}
	
	@Override
	public XNode insertBefore(XNode newChild) {
		return newXNode(this.node.getParentNode().insertBefore(newChild.getNativeNode(), this.node));
	}
	@Override
	public XNode insertAfter(XNode newChild) {
		//puesto que insertAfter no existe en dom (aunque si insertBefore), este metodo implementa una alternativa
		if (this.nextSibling() == null) // si esta al final usa el metodo de anyadir nodos
			return this.parent().appendChild(newChild);
		else // inserta antes del siguiente
			return this.nextSibling().insertBefore(newChild);
	}

	@Override
	public void removeAttribute(String name) {
		try {
			((Element) this.node).removeAttribute(name);
		} catch (ClassCastException e) {
			throw new XmlException("XNode.removeAttribute: Operacion no permitida si el nodo no es Element", e);
		}
	}

	@Override
	public boolean isRoot() {
		XNode parent = this.parent();
		return (parent == null || parent.getNativeNode().getNodeType() == Node.DOCUMENT_NODE);
	}
	@Override
	public XNode root() {
		Node n = this.node.getOwnerDocument().getFirstChild();
		return new XNode(n);
	}
	@Override
	public XNode parent() {
		Node parent=this.node.getParentNode();
		return parent == null || parent.getNodeType() == Node.DOCUMENT_NODE ? null : newXNode(parent);
	}
	@Override
	public XNode firstChild() { 
		Node n=this.node.getFirstChild();
		n=this.skipBlankSiblings(n,true);
		return newXNode(n);
	}
	@Override
	public XNode nextSibling() {
		Node n=this.node.getNextSibling();
		n=this.skipBlankSiblings(n,true);
		return newXNode(n); 
	}
	@Override
	public XNode previousSibling() {
		Node n=this.node.getPreviousSibling();
		n=this.skipBlankSiblings(n,false);
		return newXNode(n); 
	}
	private Node skipBlankSiblings(Node n, boolean forward) {
		while (isBlank(n))
			n = forward ? n.getNextSibling() : n.getPreviousSibling(); 
		return n;
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
			throw new XmlException(e);
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
					|| returnTexts && child.getNodeType() == Node.TEXT_NODE && !isBlank(child)) {
				children.add(newXNode(child));
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
			throw new XmlException(tce.getException() == null ? tce : tce.getException());
		}
	}

}
