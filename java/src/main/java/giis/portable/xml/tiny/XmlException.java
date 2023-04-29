package giis.portable.xml.tiny;

/**
 * Generic exception for Java/C# compatibility
 */
public class XmlException extends RuntimeException {
	private static final long serialVersionUID = 8827937327710355342L;

	public XmlException(Throwable e) {
		super(e);
	}
	public XmlException(String message) {
		super(message);
	}
	public XmlException(String message, Throwable cause) {
		super(message, cause);
	}
}
