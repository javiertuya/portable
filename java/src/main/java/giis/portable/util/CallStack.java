package giis.portable.util;

/**
 * Determines the call stack trace from the position of the method where this
 * instance has been created
 */
public class CallStack {
	private StackTraceElement[] stack;

	public CallStack() {
		Throwable tr = new Throwable();
		stack = tr.getStackTrace();
	}

	public int size() {
		return stack.length;
	}
	public String getClassName(int position) {
		return stack[position].getClassName();
	}
	public String getMethodName(int position) {
		return position == 0 ? "" : stack[position].getMethodName();
	}
	public String getFileName(int position) {
		return stack[position].getFileName();
	}
	/**
	 * Gets the full file name of this class: 
	 * - on java its path is relative to the source location,
	 * - on .NET is an absolute path
	 */
	public String getFullFileName(int position) {
		String[] nameItems = JavaCs.splitByDot(stack[position].getClassName());
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < nameItems.length - 1; i++)
			sb.append(nameItems[i]).append("/");
		sb.append(stack[position].getFileName());
		return sb.toString();
	}
	public int getLineNumber(int position) {
		return stack[position].getLineNumber();
	}

	public String getString() {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < stack.length; i++)
			sb.append("\n        " + getClassName(i) + " " + getMethodName(i) + " " + getLineNumber(i) + " "
					+ getFileName(i));
		return sb.toString();
	}

}
