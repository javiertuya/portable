package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import org.junit.Test;

import giis.portable.util.CallStack;
import giis.portable.util.Parameters;

public class TestCallStack {

	@Test
	public void testStackOfThisMethod() {
		CallStack stack = new CallStack();
		assertTrue(stack.size() > 2);
		assertTrue(stack.getLineNumber(0) > 0);
		assertTrue(stack.getLineNumber(1) > 0);
		assertTrue(stack.getString().toLowerCase().contains("giis.portable.util.callstack"));
		assertTrue(stack.getString().toLowerCase().contains("giis.portable.testcallstack"));
		
		if (Parameters.isJava()) {
			assertEquals("giis.portable.util.CallStack", stack.getClassName(0));
			assertEquals("CallStack.java", stack.getFileName(0));
			assertEquals("giis/portable/util/CallStack.java", stack.getFullFileName(0));
			assertEquals("", stack.getMethodName(0));
			
			assertEquals("giis.portable.TestCallStack", stack.getClassName(1));
			assertEquals("TestCallStack.java", stack.getFileName(1).replace(".cs", ".java"));
			assertEquals("giis/portable/TestCallStack.java", stack.getFullFileName(1));
			assertEquals("testStackOfThisMethod", stack.getMethodName(1));
		} else { // net
			assertEquals("Giis.Portable.Util.CallStack", stack.getClassName(0));
			assertEquals("CallStack.java", stack.getFileName(0).replace(".N.cs", ".java"));
			assertTrue(stack.getFullFileName(0).endsWith("/Portable/Giis.Portable.Util/CallStack.N.cs"));
			assertEquals("", stack.getMethodName(0));
			
			assertEquals("Giis.Portable.TestCallStack", stack.getClassName(1));
			assertEquals("TestCallStack.cs", stack.getFileName(1));
			assertTrue(stack.getFullFileName(1).endsWith("/PortableTest/Translated/Giis.Portable/TestCallStack.cs"));
			assertEquals("TestStackOfThisMethod", stack.getMethodName(1));
		}
	}

	@Test
	public void testStackOfInnerClass() {
		CallStack stack = new InnerClass().getCallStack();
		assertTrue(stack.size() > 2);

		if (Parameters.isJava()) {
			assertEquals("giis.portable.util.CallStack", stack.getClassName(0));
			assertEquals("CallStack.java", stack.getFileName(0));
			assertEquals("giis/portable/util/CallStack.java", stack.getFullFileName(0));
			assertEquals("", stack.getMethodName(0));
		} else { // net
			assertEquals("Giis.Portable.Util.CallStack", stack.getClassName(0));
			assertEquals("CallStack.java", stack.getFileName(0).replace(".N.cs", ".java"));
			assertTrue(stack.getFullFileName(0).endsWith("/Portable/Giis.Portable.Util/CallStack.N.cs"));
			assertEquals("", stack.getMethodName(0));
		}
	}
	
	public class InnerClass {
		public CallStack getCallStack() {
			return new CallStack();
		}
	}

}
