package giis.portable.xml.tiny;

import java.util.List;

/**
 * Base class with implementation of methods that are common in java and net
 */
public abstract class XNodeAbstract {
	
	// Node primitives
	
	public abstract boolean equals(XNode xn);
	public abstract boolean isElement();
	public abstract boolean isText();
	public abstract boolean isWhitespace();
	
	public abstract XNode createElement(String elementName);
	public abstract XNode createText(String textValue);
	public abstract String name();
	public abstract String value();
	public abstract void setValue(String value);
	public abstract String innerText();
	public abstract void setInnerText(String value);
	public abstract String outerXml();
	public abstract String innerXml();
	public abstract void setInnerXml(String value);
	
	// Attribute primitives
	
	public abstract String getAttribute(String name);
	public abstract void setAttribute(String name, String value);
	public abstract List<String> getAttributeNames();
	public abstract String getAttributesString();
	
	public int getIntAttribute(String name) {
		String current = this.getAttribute(name);
		if ("".equals(current)) // does not exists, default
			return 0;
		else
			return Integer.valueOf(current);
	}

	public void incrementIntAttribute(String name, int value) {
		String current = this.getAttribute(name);
		if ("".equals(current)) // does not exist, sets this value
			this.setAttribute(name, String.valueOf(value));
		else // exist, increments
			this.setAttribute(name, String.valueOf(value + Integer.parseInt(this.getAttribute(name))));
	}
	
	/**
	 * Gets the text inside the element with the indicated name, if it does not exist returns empty.
	 * It allows to represent logical attributes as elements as an alternative to native attributes
	 */
	public String getElementAtribute(String name) {
		XNode elem = this.getChild(name);
		return elem == null ? "" : elem.innerText();
	}
	
	// Node manipulation primitives
	
	public abstract XNode cloneNode();
	public abstract XNode importNode(XNode source);
	public abstract XNode createNode(String xml);
	public abstract void normalize();
	
	public abstract XNode removeChild(XNode oldChild);
	public abstract XNode appendChild(XNode elementOrTextNode);
	public XNode appendChild(String elementName) {
		return appendChild(createElement(elementName));
	}
	public abstract XNode prependChild(XNode node);
	public abstract XNode replaceChild(XNode newChild, XNode oldChild);
	public abstract XNode insertBefore(XNode newChild);
	public abstract XNode insertAfter(XNode newChild);
	public XNode insertBefore(String elementName) {
		return this.insertBefore(this.createElement(elementName));
	}
	public XNode insertAfter(String elementName) {
		return this.insertAfter(this.createElement(elementName));
	}
	
	public abstract void removeAttribute(String name);
	
	// Node navigation
	
	public abstract boolean isRoot();
	public abstract XNode parent();
	public abstract XNode root();
	public abstract XNode firstChild();
	public abstract XNode nextSibling();
	public abstract XNode previousSibling();
	/**
	 * @deprecated use firstChild()
	 */
	@Deprecated
	public XNode getFirstChild() {
		return firstChild();
	}
	
	protected abstract List<XNode> getChildren(boolean returnElements, boolean returnTexts, String elementName, boolean onlyFirst);
	
	/**
	 * Gets the first element child with the specified name, null if it does not exist
	 */
	public XNode getChild(String elementName) {
		List<XNode> children = getChildren(true, false, elementName, true);
		if (children.size() > 0) // NOSONAR compatibility java 1.4 and C#
			return children.get(0);
		return null;
	}
	/**
	 * Gets a list of all element children with the specified name
	 */
	public List<XNode> getChildren(String elementName) {
		return getChildren(true, false, elementName, false);
	}
	/**
	 * Gets a list of element children
	 */
	public List<XNode> getChildren() {
		return getChildren(true, false, null, false);
	}
	/**
	 * Gets a list of all children, including both text and elements
	 */
	public List<XNode> getChildrenWithText() {
		return getChildren(true, true, null, false);
	}
	/**
	 * Gets the number of children, including both text and elements
	 */
	public int childCount() {
		int count = 0;
		XNode child = this.firstChild();
		while (child != null) {
			count++;
			child = child.nextSibling();
		}
		return count;
	}

	// Character encoding

	public static String encodeText(String xml) {
		if (xml == null || "".equals(xml))
			return "";
		return xml.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;");
	}
	public static String decodeText(String xml) {
		if (xml == null || "".equals(xml))
			return "";
		return xml.replace("&lt;", "<").replace("&gt;", ">").replace("&amp;", "&");
	}
	public static String encodeAttribute(String xml) {
		return encodeText(xml).replace("\"", "&quot;");
	}
	public static String decodeAttribute(String xml) {
		if (xml == null)
			return "";
		xml = xml.replace("&quot;", "\"");
		return decodeText(xml);
	}

}
