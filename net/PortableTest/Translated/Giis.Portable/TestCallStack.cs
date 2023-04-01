/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Portable.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestCallStack
	{
		[Test]
		public virtual void TestStackOfThisMethod()
		{
			CallStack stack = new CallStack();
			NUnit.Framework.Assert.IsTrue(stack.Size() > 2);
			NUnit.Framework.Assert.AreEqual("giis.portable.util.callstack", stack.GetClassName(0).ToLower());
			NUnit.Framework.Assert.AreEqual("callstack.java", stack.GetFileName(0).Replace(".N.cs", ".java").ToLower());
			NUnit.Framework.Assert.IsTrue(stack.GetLineNumber(0) > 0);
			NUnit.Framework.Assert.AreEqual("giis.portable.testcallstack", stack.GetClassName(1).ToLower());
			NUnit.Framework.Assert.AreEqual("teststackofthismethod", stack.GetMethodName(1).ToLower());
			NUnit.Framework.Assert.AreEqual("testcallstack.java", stack.GetFileName(1).Replace(".cs", ".java").ToLower());
			NUnit.Framework.Assert.IsTrue(stack.GetLineNumber(1) > 0);
			NUnit.Framework.Assert.IsTrue(stack.GetString().ToLower().Contains("giis.portable.util.callstack"));
			NUnit.Framework.Assert.IsTrue(stack.GetString().ToLower().Contains("giis.portable.testcallstack"));
		}
	}
}
