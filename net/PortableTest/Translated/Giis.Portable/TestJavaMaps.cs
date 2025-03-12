using Java.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Portable
{
    /// <summary>
    /// To check after conversion that the C# Map implementations
    /// have the same behaviour than the native Java implementations
    /// </summary>
    public class TestJavaMaps
    {
        // Tests are done both for HashMap and TreeMap
        [Test]
        public virtual void TestHashMapManipulation()
        {
            DoMapManipulation(new HashMap<string, string>());
        }

        [Test]
        public virtual void TestTreeMapManipulation()
        {
            DoMapManipulation(new TreeMap<string, string>());
        }

        public virtual void DoMapManipulation(Map<string, string> map)
        {
            map.Put("x", "one");
            map.Put("a", "two");
            map.Put("x", "oneone"); // repeated, should not fail in net
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, map.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("oneone", map["x"]);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("two", map["a"]);
            NUnit.Framework.Legacy.ClassicAssert.IsNull(map["nothing"]); // non existing, should not fail net
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(map.ContainsKey("x"));
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(map.ContainsKey("nothing"));
            NUnit.Framework.Legacy.ClassicAssert.IsFalse(map.IsEmpty());
            map.Remove("x");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, map.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("two", map["a"]);
            map.Clear();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, map.Count);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(map.IsEmpty());
        }

        [Test]
        public virtual void TestHashMapPutAll()
        {
            DoMapPutAll(new HashMap<string, string>(), new HashMap<string, string>());
        }

        [Test]
        public virtual void TestTreeMapPutAll()
        {
            DoMapPutAll(new TreeMap<string, string>(), new TreeMap<string, string>());
        }

        public virtual void DoMapPutAll(Map<string, string> map1, Map<string, string> map2)
        {
            map1.Put("a", "one");
            map1.Put("b", "two");
            map2.Put("a", "oneone"); // this will change a value
            map2.Put("c", "three");
            map1.PutAll(map2);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(3, map1.Count);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("oneone", map1["a"]);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("two", map1["b"]);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("three", map1["c"]);
        }

        [Test]
        public virtual void TestHashMapIteration()
        {
            DoMapIteration(new HashMap<string, string>(), false);
        }

        [Test]
        public virtual void TestTreeMapIteration()
        {
            DoMapIteration(new TreeMap<string, string>(), true);
        }

        public virtual void DoMapIteration(Map<string, string> map, bool sorted)
        {
            map.Put("baaa", "one");
            map.Put("aaaa", "two");
            StringBuilder sb = new StringBuilder();
            foreach (string key in map.KeySet())
                sb.Append(key);

            // with this data, HashMap and Dictionary retrieve the insertion order,
            // TreeMap and SortedDictionary shouldn't
            if (sorted)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("aaaabaaa", sb.ToString());
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("{aaaa=two, baaa=one}", map.ToString());
            }
            else
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("baaaaaaa", sb.ToString());
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("{baaa=one, aaaa=two}", map.ToString());
            }
        }
    }
}