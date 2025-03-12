package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;

import java.util.HashMap;
import java.util.Map;
import java.util.TreeMap;

import org.junit.Test;

/**
 * To check after conversion that the C# Map implementations 
 * have the same behaviour than the native Java implementations
 */
public class TestJavaMaps {

	// Tests are done both for HashMap and TreeMap
	@Test
	public void testHashMapManipulation() {
		doMapManipulation(new HashMap<String, String>());
	}
	@Test
	public void testTreeMapManipulation() {
		doMapManipulation(new TreeMap<String, String>());
	}
	public void doMapManipulation(Map<String, String> map) {
		map.put("x", "one");
		map.put("a", "two");
		map.put("x", "oneone"); // repeated, should not fail in net

		assertEquals(2, map.size());
		assertEquals("oneone", map.get("x"));
		assertEquals("two", map.get("a"));
		assertNull(map.get("nothing")); // non existing, should not fail net

		assertTrue(map.containsKey("x"));
		assertFalse(map.containsKey("nothing"));
		assertFalse(map.isEmpty());

		map.remove("x");
		assertEquals(1, map.size());
		assertEquals("two", map.get("a"));

		map.clear();
		assertEquals(0, map.size());
		assertTrue(map.isEmpty());
	}

	@Test
	public void testHashMapPutAll() {
		doMapPutAll(new HashMap<String, String>(),
				new HashMap<String, String>());
	}
	@Test
	public void testTreeMapPutAll() {
		doMapPutAll(new TreeMap<String, String>(),
				new TreeMap<String, String>());
	}
	public void doMapPutAll(Map<String, String> map1,
			Map<String, String> map2) {
		map1.put("a", "one");
		map1.put("b", "two");
		map2.put("a", "oneone"); // this will change a value
		map2.put("c", "three");
		map1.putAll(map2);
		assertEquals(3, map1.size());
		assertEquals("oneone", map1.get("a"));
		assertEquals("two", map1.get("b"));
		assertEquals("three", map1.get("c"));
	}

	@Test
	public void testHashMapIteration() {
		doMapIteration(new HashMap<String, String>(), false);
	}
	@Test
	public void testTreeMapIteration() {
		doMapIteration(new TreeMap<String, String>(), true);
	}
	public void doMapIteration(Map<String, String> map, boolean sorted) {
		map.put("baaa", "one");
		map.put("aaaa", "two");
		StringBuilder sb = new StringBuilder();
		for (String key : map.keySet())
			sb.append(key);
		// with this data, HashMap and Dictionary retrieve the insertion order,
		// TreeMap and SortedDictionary shouldn't
		if (sorted) {
			assertEquals("aaaabaaa", sb.toString());
			assertEquals("{aaaa=two, baaa=one}", map.toString());
		} else {
			assertEquals("baaaaaaa", sb.toString());
			assertEquals("{baaa=one, aaaa=two}", map.toString());
		}
	}

}
