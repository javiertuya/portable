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
				NUnit.Framework.Assert.AreEqual("java", Parameters.GetPlatformName());
				NUnit.Framework.Assert.AreEqual(".", Parameters.GetProjectRoot());
				NUnit.Framework.Assert.AreEqual("target", Parameters.GetReportSubdir());
			}
			if (Parameters.IsNetCore())
			{
				NUnit.Framework.Assert.AreEqual("netcore", Parameters.GetPlatformName());
				NUnit.Framework.Assert.AreEqual("../../../..", Parameters.GetProjectRoot().Replace("\\", "/"));
				NUnit.Framework.Assert.AreEqual("reports", Parameters.GetReportSubdir());
			}
			Parameters.SetProjectRoot("custom-root");
			NUnit.Framework.Assert.AreEqual("custom-root", Parameters.GetProjectRoot());
			Parameters.SetReportSubdir("custom-subdir");
			NUnit.Framework.Assert.AreEqual("custom-subdir", Parameters.GetReportSubdir());
		}
	}
}
