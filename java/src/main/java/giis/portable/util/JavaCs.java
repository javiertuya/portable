package giis.portable.util;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import org.apache.commons.codec.digest.DigestUtils;

/**
 * Basic utilities Java/C# compatible.
 * Use this methods when java code is transformed with Sharpen
 */
public class JavaCs {
	private JavaCs() {
		throw new IllegalAccessError("Utility class");
	}

	public static boolean equalsIgnoreCase(String thisString, String anotherString) {
		return thisString.equalsIgnoreCase(anotherString);
	}
	public static String substring(String fromString, int beginIndex) {
		return fromString.substring(beginIndex);
	}
	public static String substring(String fromString, int beginIndex, int endIndex) {
		return fromString.substring(beginIndex, endIndex);
	}
	public static boolean containsIgnoreCase(List<String> values, String target) {
		for (int j = 0; j < values.size(); j++)
			if (values.get(j).equalsIgnoreCase(target))
				return true;
		return false;
	}
	public static char charAt(String str, int index) {
		return str.charAt(index);
	}

	public static String numToString(long value) {
		return Long.toString(value);
	}
	public static int stringToInt(String value) {
		return Integer.valueOf(value);
	}

	public static String[] toArray(List<String> lst) {
		return lst.toArray(new String[lst.size()]);
	}
	public static List<String> toList(String[] array) {
		return Arrays.asList(array);
	}
	public static String deepToString(String[] strArray) {
		return Arrays.deepToString(strArray);
	}

	/**
	 * @deprecated Use the Java native Map implementations or the custom .NET Map implementations in java.util
	 */
	@Deprecated
	public static void putAll(Map<String, Object> targetMap, Map<String, Object> mapToAdd) {
		targetMap.putAll(mapToAdd);
	}

	public static boolean isEmpty(String str) {
		return str == null || "".equals(str.trim());
	}
	public static boolean isEmpty(List<String> lst) {
		return lst == null || lst.isEmpty();
	}

	/**
	 * Replacement using a regular expression (java replaceAll), needed because in
	 * C# replace does not uses regular expressions
	 */
	public static String replaceRegex(String str, String regex, String replacement) {
		return str.replaceAll(regex, replacement);
	}
	/**
	 * Split by a single char (needed to former netcore 2.0 compatibility), note
	 * that the char must not be special character in regular expressions.
	 */
	public static String[] splitByChar(String str, char c) {
		return str.split(String.valueOf(c));
	}
	public static String[] splitByDot(String str) {
		return str.split("\\.");
	}
	public static String[] splitByBar(String str) {
		return str.split("\\|");
	}

	public static Date getCurrentDate() {
		return new Date();
	}
	public static String getTime(Date date) {
		return new SimpleDateFormat("HH:mm:ss").format(date);
	}
	public static String getIsoDate(Date date) {
		return new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS").format(date);
	}
	public static Date parseIsoDate(String dateString) {
		try {
			return new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS").parse(dateString);
		} catch (ParseException e) {
			throw new RuntimeException(e.getMessage()); // NOSONAR to do not generate additional dependencies
		}
	}
	public static long currentTimeMillis() {
		return System.currentTimeMillis();
	}

	public static void sleep(int millis) {
		try {
			Thread.sleep((long) millis);
		} catch (Exception e1) { // NOSONAR
			throw new PortableException("Exception in Thread.Sleep", e1);
		}
	}

	public static String getEnvironmentVariable(String name) {
		return System.getenv(name); // NOSONAR
	}
	public static String getUniqueId() {
		return UUID.randomUUID().toString();
	}
	public static String getHash(String str) {
		return DigestUtils.sha256Hex(str);
	}
	public static String getHashMd5(String str) { // for java 1.4 versions
		return DigestUtils.md5Hex(str);
	}
}
