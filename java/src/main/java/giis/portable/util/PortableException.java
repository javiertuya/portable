package giis.portable.util;

/**
 * Generic exception for Java/C# compatibility
 */
public class PortableException extends RuntimeException {
	private static final long serialVersionUID = 8827937327710355342L;

	public PortableException(Throwable e) {
		super(e);
	}
	public PortableException(String message) {
		super(message);
	}
	public PortableException(String message, Throwable cause) {
		super(message, cause);
	}
}
