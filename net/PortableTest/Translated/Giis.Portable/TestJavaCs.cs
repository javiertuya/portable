using Java.Util;
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
    public class TestJavaCs
    {
        [Test]
        public virtual void TestEqualsIgnoreCase()
        {
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.EqualsIgnoreCase("aBc", "aBc"));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.EqualsIgnoreCase("aBc", "AbC"));
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.EqualsIgnoreCase("aBc", "XbC"));
        }

        [Test]
        public virtual void TestSubstringOnlyBegin()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("ab", JavaCs.Substring("ab", 0));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("b", JavaCs.Substring("ab", 1));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", JavaCs.Substring("ab", 2));
            try
            {
                JavaCs.Substring("ab", 3);
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should raise exception");
            }
            catch (Exception e)
            {
            }
        }

        [Test]
        public virtual void TestSubstringBeginAndEnd()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", JavaCs.Substring("abc", 1, 1));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("b", JavaCs.Substring("abc", 1, 2));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("bc", JavaCs.Substring("abc", 1, 3));
            try
            {
                JavaCs.Substring("abc", 1, 4);
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should raise exception");
            }
            catch (Exception e)
            {
            }
        }

        [Test]
        public virtual void TestContainsIgnoreCase()
        {
            IList<string> target = new List<string>(); // NOSONAR for java conversion
            target.Add("aBc");
            target.Add("xYz");
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.ContainsIgnoreCase(target, "AbC"));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.ContainsIgnoreCase(target, "XyZ"));
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.ContainsIgnoreCase(target, "Xy"));
        }

        [Test]
        public virtual void TestToArray()
        {
            IList<string> single = new List<string>(); // NOSONAR for java conversion
            single.Add("abc");
            single.Add("xyz");

            // uses deepToString to easier comparison on C#
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[abc, xyz]", JavaCs.DeepToString(JavaCs.ToArray(single)));
        }

        [Test]
        public virtual void TestToList()
        {
            IList<string> actual = JavaCs.ToList(new string[] { "abc", "xyz" });
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, actual.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc", actual[0]);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("xyz", actual[1]);
        }

        [Test]
        public virtual void TestIsEmptyString()
        {
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.IsEmpty("x"));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty(""));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty((string)null));

            // whitespace/control chars also return empty (C# isEmptyOrWhiteSpace)
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty(" "));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty("\n"));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty("\n "));
        }

        [Test]
        public virtual void TestIsEmptyList()
        {
            IList<string> lst = new List<string>(); // NOSONAR for java conversion
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty(lst));
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.IsEmpty((IList<string>)null));
            lst.Add("txt");
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.IsEmpty(lst));
        }

        [Test]
        public virtual void TestNumToString()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("6", JavaCs.NumToString(2 + 4));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(24, JavaCs.StringToInt("2" + "4"));
        }

        [Test]
        public virtual void TestReplaceRegex()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("ab.###.cd.###.e", JavaCs.ReplaceRegex("ab.1.cd.2345.e", "\\.\\d+\\.", ".###."));
        }

        [Test]
        public virtual void TestSplit()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[ab, cd, ef]", JavaCs.DeepToString(JavaCs.SplitByBar("ab|cd|ef")));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[ab, cd, ef]", JavaCs.DeepToString(JavaCs.SplitByDot("ab.cd.ef")));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[ab, cd, ef]", JavaCs.DeepToString(JavaCs.SplitByChar("ab,cd,ef", ',')));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[ab]", JavaCs.DeepToString(JavaCs.SplitByChar("ab", ',')));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[ab, , cd]", JavaCs.DeepToString(JavaCs.SplitByChar("ab,,cd", ',')));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("[]", JavaCs.DeepToString(JavaCs.SplitByChar("", ','))); // note that empty string before first or after last separator has different
            // behaviour in java/C#
        }

        [Test]
        public virtual void TestDates()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("2001-01-02T10:11:22.123", JavaCs.GetIsoDate(JavaCs.ParseIsoDate("2001-01-02T10:11:22.123")));
            string before = JavaCs.GetIsoDate(JavaCs.GetCurrentDate());
            JavaCs.Sleep(110);
            string after = JavaCs.GetIsoDate(JavaCs.GetCurrentDate());

            // dates are always different after some sleep (can't be flaky)
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.Substring(before, 0, 21).Equals(JavaCs.Substring(after, 0, 21)));

            // Milliseconds is long number
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.NumToString(JavaCs.CurrentTimeMillis()).Length > 12);
        }

        [Test]
        public virtual void TestEnvironment()
        {

            // some that exists both in linux and windows
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(JavaCs.GetEnvironmentVariable("PATH").Length > 0);
        }

        [Test]
        public virtual void TestIds()
        {
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.GetUniqueId().Equals(JavaCs.GetUniqueId()));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(36, JavaCs.GetUniqueId().Length);
        }

        [Test]
        public virtual void TestHash()
        {
            string hash1 = JavaCs.GetHash("axymbzd");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(64, hash1.Length);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(JavaCs.GetHash("axymbzd"), hash1);
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(JavaCs.GetHash("abymbzd").Equals(hash1));
        }
    }
}