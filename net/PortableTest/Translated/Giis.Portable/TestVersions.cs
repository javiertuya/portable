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
    public class TestVersions
    {
        [Test]
        public virtual void TestVersionOfThisArtifact()
        {

            // The portable approach by specifying a class and artifact name
            // Update when major or minor version changes
            // If running from eclipse and fails, run from maven before and then refresh
            string version = new Versions(new PortableException("").GetType(), "io.github.javiertuya", "portable-java").GetVersion();
            string[] items = JavaCs.SplitByDot(version);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("2", items[0]);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("3", items[1]);

            // java only, does not need specify any class
            if (Parameters.IsJava())
            {
                string jversion = new Versions(null, "io.github.javiertuya", "portable-java").GetVersion();
                NUnit.Framework.Legacy.ClassicAssert.AreEqual(version, jversion);
            }


            // net only, does not need specify artifact
            if (Parameters.IsNetCore())
            {
                string nversion = new Versions(new PortableException("").GetType(), null, null).GetVersion();
                NUnit.Framework.Legacy.ClassicAssert.AreEqual(version, nversion);
            }
        }

        [Test]
        public virtual void TestVersionUnknownArtifactGivesFallback()
        {
            string version = new Versions(null, "group-does-not-exist", "artifact-does-not-exist").GetVersion();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("0.0.0", version);
        }
    }
}