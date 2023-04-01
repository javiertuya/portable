/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Giis.Portable.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestJavaCs
	{
		[Test]
		public virtual void TestEqualsIgnoreCase()
		{
			NUnit.Framework.Assert.IsTrue(JavaCs.EqualsIgnoreCase("aBc", "aBc"));
			NUnit.Framework.Assert.IsTrue(JavaCs.EqualsIgnoreCase("aBc", "AbC"));
			NUnit.Framework.Assert.IsFalse(JavaCs.EqualsIgnoreCase("aBc", "XbC"));
		}

		[Test]
		public virtual void TestSubstringOnlyBegin()
		{
			NUnit.Framework.Assert.AreEqual("ab", JavaCs.Substring("ab", 0));
			NUnit.Framework.Assert.AreEqual("b", JavaCs.Substring("ab", 1));
			NUnit.Framework.Assert.AreEqual(string.Empty, JavaCs.Substring("ab", 2));
			try
			{
				JavaCs.Substring("ab", 3);
				NUnit.Framework.Assert.Fail("Should raise exception");
			}
			catch (Exception)
			{
			}
		}

		// success
		[Test]
		public virtual void TestSubstringBeginAndEnd()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, JavaCs.Substring("abc", 1, 1));
			NUnit.Framework.Assert.AreEqual("b", JavaCs.Substring("abc", 1, 2));
			NUnit.Framework.Assert.AreEqual("bc", JavaCs.Substring("abc", 1, 3));
			try
			{
				JavaCs.Substring("abc", 1, 4);
				NUnit.Framework.Assert.Fail("Should raise exception");
			}
			catch (Exception)
			{
			}
		}

		// success
		[Test]
		public virtual void TestContainsIgnoreCase()
		{
			IList<string> target = new List<string>();
			target.Add("aBc");
			target.Add("xYz");
			NUnit.Framework.Assert.IsTrue(JavaCs.ContainsIgnoreCase(target, "AbC"));
			NUnit.Framework.Assert.IsTrue(JavaCs.ContainsIgnoreCase(target, "XyZ"));
			NUnit.Framework.Assert.IsFalse(JavaCs.ContainsIgnoreCase(target, "Xy"));
		}

		[Test]
		public virtual void TestToArray()
		{
			IList<string> single = new List<string>();
			single.Add("abc");
			single.Add("xyz");
			// uses deepToString to easier comparison on C#
			NUnit.Framework.Assert.AreEqual("[abc, xyz]", JavaCs.DeepToString(JavaCs.ToArray(single)));
		}

		[Test]
		public virtual void TestToList()
		{
			IList<string> actual = JavaCs.ToList(new string[] { "abc", "xyz" });
			NUnit.Framework.Assert.AreEqual(2, actual.Count);
			NUnit.Framework.Assert.AreEqual("abc", actual[0]);
			NUnit.Framework.Assert.AreEqual("xyz", actual[1]);
		}

		[Test]
		public virtual void TestIsEmpty()
		{
			NUnit.Framework.Assert.IsFalse(JavaCs.IsEmpty("x"));
			NUnit.Framework.Assert.IsTrue(JavaCs.IsEmpty(string.Empty));
			NUnit.Framework.Assert.IsTrue(JavaCs.IsEmpty(null));
			// whitespace/control chars also return empty (C# isEmptyOrWhiteSpace)
			NUnit.Framework.Assert.IsTrue(JavaCs.IsEmpty(" "));
			NUnit.Framework.Assert.IsTrue(JavaCs.IsEmpty("\n"));
			NUnit.Framework.Assert.IsTrue(JavaCs.IsEmpty("\n "));
		}

		[Test]
		public virtual void TestReplaceRegex()
		{
			NUnit.Framework.Assert.AreEqual("ab.###.cd.###.e", JavaCs.ReplaceRegex("ab.1.cd.2345.e", "\\.\\d+\\.", ".###."));
		}

		[Test]
		public virtual void TestSplit()
		{
			NUnit.Framework.Assert.AreEqual("[ab, cd, ef]", JavaCs.DeepToString(JavaCs.SplitByBar("ab|cd|ef")));
			NUnit.Framework.Assert.AreEqual("[ab, cd, ef]", JavaCs.DeepToString(JavaCs.SplitByDot("ab.cd.ef")));
			NUnit.Framework.Assert.AreEqual("[ab, cd, ef]", JavaCs.DeepToString(JavaCs.SplitByChar("ab,cd,ef", ',')));
			NUnit.Framework.Assert.AreEqual("[ab]", JavaCs.DeepToString(JavaCs.SplitByChar("ab", ',')));
			NUnit.Framework.Assert.AreEqual("[ab, , cd]", JavaCs.DeepToString(JavaCs.SplitByChar("ab,,cd", ',')));
			NUnit.Framework.Assert.AreEqual("[]", JavaCs.DeepToString(JavaCs.SplitByChar(string.Empty, ',')));
		}

		// note that empty string before first or after last separator has different
		// behaviour in java/C#
		[Test]
		public virtual void TestDates()
		{
			NUnit.Framework.Assert.AreEqual("2001-01-02T10:11:22.123", JavaCs.GetIsoDate(JavaCs.ParseIsoDate("2001-01-02T10:11:22.123")));
			string before = JavaCs.GetIsoDate(JavaCs.GetCurrentDate());
			JavaCs.Sleep(110);
			string after = JavaCs.GetIsoDate(JavaCs.GetCurrentDate());
			// dates are always different after some sleep (can't be flaky)
			NUnit.Framework.Assert.IsFalse(JavaCs.Substring(before, 0, 21).Equals(JavaCs.Substring(after, 0, 21)));
			// Milliseconds is long number
			NUnit.Framework.Assert.IsTrue(JavaCs.NumToString(JavaCs.CurrentTimeMillis()).Length > 12);
		}

		[Test]
		public virtual void TestEnvironment()
		{
			// some that exists both in linux and windows
			NUnit.Framework.Assert.IsTrue(JavaCs.GetEnvironmentVariable("PATH").Length > 0);
		}

		[Test]
		public virtual void TestIds()
		{
			NUnit.Framework.Assert.IsFalse(JavaCs.GetUniqueId().Equals(JavaCs.GetUniqueId()));
			NUnit.Framework.Assert.AreEqual(36, JavaCs.GetUniqueId().Length);
		}

		[Test]
		public virtual void TestHash()
		{
			string hash1 = JavaCs.GetHash("axymbzd");
			NUnit.Framework.Assert.AreEqual(64, hash1.Length);
			NUnit.Framework.Assert.AreEqual(JavaCs.GetHash("axymbzd"), hash1);
			NUnit.Framework.Assert.IsFalse(JavaCs.GetHash("abymbzd").Equals(hash1));
		}
	}
}
