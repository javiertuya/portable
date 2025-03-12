package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertThrows;
import static org.junit.Assert.assertTrue;

import java.util.ArrayList;
import java.util.List;

import org.junit.Test;

import giis.portable.util.JavaCs;

public class TestJavaCs {

	@Test
	public void testEqualsIgnoreCase() {
		assertTrue(JavaCs.equalsIgnoreCase("aBc", "aBc"));
		assertTrue(JavaCs.equalsIgnoreCase("aBc", "AbC"));
		assertFalse(JavaCs.equalsIgnoreCase("aBc", "XbC"));
	}

	@Test
	public void testSubstringOnlyBegin() {
		assertEquals("ab", JavaCs.substring("ab", 0));
		assertEquals("b", JavaCs.substring("ab", 1));
		assertEquals("", JavaCs.substring("ab", 2));
		assertThrows(RuntimeException.class, () -> {
			JavaCs.substring("ab", 3);
		});
	}

	@Test
	public void testSubstringBeginAndEnd() {
		assertEquals("", JavaCs.substring("abc", 1, 1));
		assertEquals("b", JavaCs.substring("abc", 1, 2));
		assertEquals("bc", JavaCs.substring("abc", 1, 3));
		assertThrows(RuntimeException.class, () -> {
			JavaCs.substring("abc", 1, 4);
		});
	}

	@Test
	public void testContainsIgnoreCase() {
		List<String> target = new ArrayList<String>(); // NOSONAR for java conversion
		target.add("aBc");
		target.add("xYz");
		assertTrue(JavaCs.containsIgnoreCase(target, "AbC"));
		assertTrue(JavaCs.containsIgnoreCase(target, "XyZ"));
		assertFalse(JavaCs.containsIgnoreCase(target, "Xy"));
	}

	@Test
	public void testToArray() {
		List<String> single = new ArrayList<String>(); // NOSONAR for java conversion
		single.add("abc");
		single.add("xyz");
		// uses deepToString to easier comparison on C#
		assertEquals("[abc, xyz]", JavaCs.deepToString(JavaCs.toArray(single)));
	}

	@Test
	public void testToList() {
		List<String> actual = JavaCs.toList(new String[] { "abc", "xyz" });
		assertEquals(2, actual.size());
		assertEquals("abc", actual.get(0));
		assertEquals("xyz", actual.get(1));
	}

	@Test
	public void testIsEmptyString() {
		assertFalse(JavaCs.isEmpty("x"));
		assertTrue(JavaCs.isEmpty(""));
		assertTrue(JavaCs.isEmpty((String)null));
		// whitespace/control chars also return empty (C# isEmptyOrWhiteSpace)
		assertTrue(JavaCs.isEmpty(" "));
		assertTrue(JavaCs.isEmpty("\n"));
		assertTrue(JavaCs.isEmpty("\n "));
	}

	@Test
	public void testIsEmptyList() {
		List<String> lst=new ArrayList<String>(); // NOSONAR for java conversion
		assertTrue(JavaCs.isEmpty(lst));
		assertTrue(JavaCs.isEmpty((List<String>)null));
		lst.add("txt");
		assertFalse(JavaCs.isEmpty(lst));
	}
	
	@Test
	public void testNumToString() {
		assertEquals("6", JavaCs.numToString(2+4));
		assertEquals(24, JavaCs.stringToInt("2"+"4"));
	}

	@Test
	public void testReplaceRegex() {
		assertEquals("ab.###.cd.###.e", JavaCs.replaceRegex("ab.1.cd.2345.e", "\\.\\d+\\.", ".###."));
	}

	@Test
	public void testSplit() {
		assertEquals("[ab, cd, ef]", JavaCs.deepToString(JavaCs.splitByBar("ab|cd|ef")));
		assertEquals("[ab, cd, ef]", JavaCs.deepToString(JavaCs.splitByDot("ab.cd.ef")));
		assertEquals("[ab, cd, ef]", JavaCs.deepToString(JavaCs.splitByChar("ab,cd,ef", ',')));
		assertEquals("[ab]", JavaCs.deepToString(JavaCs.splitByChar("ab", ',')));
		assertEquals("[ab, , cd]", JavaCs.deepToString(JavaCs.splitByChar("ab,,cd", ',')));
		assertEquals("[]", JavaCs.deepToString(JavaCs.splitByChar("", ',')));
		// note that empty string before first or after last separator has different
		// behaviour in java/C#
	}

	@Test
	public void testDates() {
		assertEquals("2001-01-02T10:11:22.123", JavaCs.getIsoDate(JavaCs.parseIsoDate("2001-01-02T10:11:22.123")));

		String before = JavaCs.getIsoDate(JavaCs.getCurrentDate());
		JavaCs.sleep(110);
		String after = JavaCs.getIsoDate(JavaCs.getCurrentDate());
		// dates are always different after some sleep (can't be flaky)
		assertFalse(JavaCs.substring(before, 0, 21).equals(JavaCs.substring(after, 0, 21)));

		// Milliseconds is long number
		assertTrue(JavaCs.numToString(JavaCs.currentTimeMillis()).length() > 12);
	}

	@Test
	public void testEnvironment() {
		// some that exists both in linux and windows
		assertTrue(JavaCs.getEnvironmentVariable("PATH").length() > 0);
	}

	@Test
	public void testIds() {
		assertFalse(JavaCs.getUniqueId().equals(JavaCs.getUniqueId()));
		assertEquals(36, JavaCs.getUniqueId().length());
	}

	@Test
	public void testHash() {
		String hash1 = JavaCs.getHash("axymbzd");
		assertEquals(64, hash1.length());
		assertEquals(JavaCs.getHash("axymbzd"), hash1);
		assertFalse(JavaCs.getHash("abymbzd").equals(hash1));
	}
}
