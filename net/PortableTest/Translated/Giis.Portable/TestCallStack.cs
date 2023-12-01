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
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.Size() > 2);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis.portable.util.callstack", stack.GetClassName(0).ToLower());
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("callstack.java", stack.GetFileName(0).Replace(".N.cs", ".java").ToLower());
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetLineNumber(0) > 0);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis.portable.testcallstack", stack.GetClassName(1).ToLower());
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("teststackofthismethod", stack.GetMethodName(1).ToLower());
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("testcallstack.java", stack.GetFileName(1).Replace(".cs", ".java").ToLower());
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetLineNumber(1) > 0);
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetString().ToLower().Contains("giis.portable.util.callstack"));
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetString().ToLower().Contains("giis.portable.testcallstack"));
		}
	}
}
