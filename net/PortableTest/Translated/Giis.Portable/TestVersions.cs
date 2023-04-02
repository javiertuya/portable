/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Portable.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestVersions
	{
		[Test]
		public virtual void TestVersionOfThisArtifact()
		{
			// The portable approach by specifying a class and artifact name
			// Update when major or minor version changes
			string version = new Versions(new PortableException(string.Empty).GetType(), "io.github.javiertuya", "portable-java").GetVersion();
			string[] items = JavaCs.SplitByDot(version);
			NUnit.Framework.Assert.AreEqual("2", items[0]);
			NUnit.Framework.Assert.AreEqual("1", items[1]);
			// java only, does not need specify any class
			if (Parameters.IsJava())
			{
				string jversion = new Versions(null, "io.github.javiertuya", "portable-java").GetVersion();
				NUnit.Framework.Assert.AreEqual(version, jversion);
			}
			// net only, does not need specify artifact
			if (Parameters.IsNetCore())
			{
				string nversion = new Versions(new PortableException(string.Empty).GetType(), null, null).GetVersion();
				NUnit.Framework.Assert.AreEqual(version, nversion);
			}
		}

		[Test]
		public virtual void TestVersionUnknownArtifactGivesFallback()
		{
			string version = new Versions(null, "group-does-not-exist", "artifact-does-not-exist").GetVersion();
			NUnit.Framework.Assert.AreEqual("0.0.0", version);
		}
	}
}
