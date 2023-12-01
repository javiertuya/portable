/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Portable.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestParameters
	{
		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			Parameters.SetDefault();
		}

		[Test]
		public virtual void TestPlatformParameters()
		{
			if (Parameters.IsJava())
			{
				NUnit.Framework.Legacy.ClassicAssert.AreEqual("java", Parameters.GetPlatformName());
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(".", Parameters.GetProjectRoot());
				NUnit.Framework.Legacy.ClassicAssert.AreEqual("target", Parameters.GetReportSubdir());
			}
			if (Parameters.IsNetCore())
			{
				NUnit.Framework.Legacy.ClassicAssert.AreEqual("netcore", Parameters.GetPlatformName());
				NUnit.Framework.Legacy.ClassicAssert.AreEqual("../../../..", Parameters.GetProjectRoot().Replace("\\", "/"));
				NUnit.Framework.Legacy.ClassicAssert.AreEqual("reports", Parameters.GetReportSubdir());
			}
			Parameters.SetProjectRoot("custom-root");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("custom-root", Parameters.GetProjectRoot());
			Parameters.SetReportSubdir("custom-subdir");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("custom-subdir", Parameters.GetReportSubdir());
		}
	}
}
