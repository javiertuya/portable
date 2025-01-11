using NUnit.Framework;
using Giis.Portable.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Portable
{
    public class TestCallStack
    {
        [Test]
        public virtual void TestStackOfThisMethod()
        {
            CallStack stack = new CallStack();
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.Count > 2);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetLineNumber(0) > 0);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetLineNumber(1) > 0);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetString().ToLower().Contains("giis.portable.util.callstack"));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetString().ToLower().Contains("giis.portable.testcallstack"));
            if (Parameters.IsJava())
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis.portable.util.CallStack", stack.GetClassName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("CallStack.java", stack.GetFileName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis/portable/util/CallStack.java", stack.GetFullFileName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("", stack.GetMethodName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis.portable.TestCallStack", stack.GetClassName(1));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("TestCallStack.java", stack.GetFileName(1).Replace(".cs", ".java"));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis/portable/TestCallStack.java", stack.GetFullFileName(1));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("testStackOfThisMethod", stack.GetMethodName(1));
            } // net
            else
            {

                // net
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("Giis.Portable.Util.CallStack", stack.GetClassName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("CallStack.java", stack.GetFileName(0).Replace(".N.cs", ".java"));
                NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetFullFileName(0).EndsWith("/Portable/Giis.Portable.Util/CallStack.N.cs"));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("", stack.GetMethodName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("Giis.Portable.TestCallStack", stack.GetClassName(1));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("TestCallStack.cs", stack.GetFileName(1));
                NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetFullFileName(1).EndsWith("/PortableTest/Translated/Giis.Portable/TestCallStack.cs"));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("TestStackOfThisMethod", stack.GetMethodName(1));
            }
        }

        [Test]
        public virtual void TestStackOfInnerClass()
        {
            CallStack stack = new InnerClass().GetCallStack();
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.Count > 2);
            if (Parameters.IsJava())
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis.portable.util.CallStack", stack.GetClassName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("CallStack.java", stack.GetFileName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("giis/portable/util/CallStack.java", stack.GetFullFileName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("", stack.GetMethodName(0));
            } // net
            else
            {

                // net
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("Giis.Portable.Util.CallStack", stack.GetClassName(0));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("CallStack.java", stack.GetFileName(0).Replace(".N.cs", ".java"));
                NUnit.Framework.Legacy.ClassicAssert.IsTrue(stack.GetFullFileName(0).EndsWith("/Portable/Giis.Portable.Util/CallStack.N.cs"));
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("", stack.GetMethodName(0));
            }
        }

        public class InnerClass
        {
            public virtual CallStack GetCallStack()
            {
                return new CallStack();
            }
        }
    }
}