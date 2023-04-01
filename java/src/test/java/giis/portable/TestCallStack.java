package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import org.junit.Test;

import giis.portable.util.CallStack;

public class TestCallStack {

	@Test
	public void testStackOfThisMethod() {
		CallStack stack = new CallStack();
		assertTrue(stack.size() > 2);
		assertEquals("giis.portable.util.callstack", stack.getClassName(0).toLowerCase());
		assertEquals("callstack.java", stack.getFileName(0).replace(".N.cs", ".java").toLowerCase());
		assertTrue(stack.getLineNumber(0) > 0);

		assertEquals("giis.portable.testcallstack", stack.getClassName(1).toLowerCase());
		assertEquals("teststackofthismethod", stack.getMethodName(1).toLowerCase());
		assertEquals("testcallstack.java", stack.getFileName(1).replace(".cs", ".java").toLowerCase());
		assertTrue(stack.getLineNumber(1) > 0);

		assertTrue(stack.getString().toLowerCase().contains("giis.portable.util.callstack"));
		assertTrue(stack.getString().toLowerCase().contains("giis.portable.testcallstack"));
	}

}
