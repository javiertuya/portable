package giis.portable.xml.tiny;

import java.util.List;

/**
 * Base class with implementation of methods that are common in java and net
 */
public abstract class XNodeAbstract {
	public abstract String getAttribute(String name);
	public abstract void setAttribute(String name, String value);
	public abstract List<String> getAttributeNames();
	public abstract XNode getChild(String elementName);
	public abstract List<XNode> getChildren(String elementName);
	public abstract XNode getFirstChild();
	public abstract String name();
	public abstract String innerText();
	public abstract void setInnerText(String value);
	public abstract XNode appendChild(String elementName);
	public abstract String outerXml();
	public abstract String innerXml();

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
