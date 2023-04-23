package giis.portable.xml.tiny;

import java.util.List;

/**
 * Base class with implementation of methods that are common in java and net
 */
public abstract class XNodeAbstract {
	
	// Node primitives
	
	public abstract boolean isElement();
	public abstract boolean isText();
	public abstract XNode createElement(String elementName);
	public abstract XNode createText(String textValue);
	public abstract String name();
	public abstract String innerText();
	public abstract void setInnerText(String value);
	public abstract String outerXml();
	public abstract String innerXml();
	
	// Attribute primitives
	
	public abstract String getAttribute(String name);
	public abstract void setAttribute(String name, String value);
	public abstract List<String> getAttributeNames();
	
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
	
	// Node manipulation
	
	public abstract XNode getFirstChild();
	public abstract XNode appendChild(String elementName);
	
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
