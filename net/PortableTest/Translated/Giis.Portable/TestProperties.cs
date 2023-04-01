/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Portable.Util;
using Java.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestProperties
	{
		[Test]
		public virtual void TestReadProperties()
		{
			string propFile = FileUtil.GetPath(Parameters.GetProjectRoot(), "..", "test-file.properties");
			Properties prop = new PropertiesFactory().GetPropertiesFromFilename(propFile);
			NUnit.Framework.Assert.AreEqual("psimple", prop.GetProperty("prop.simple"));
			NUnit.Framework.Assert.AreEqual("pspaces", prop.GetProperty("prop.spaces"));
			NUnit.Framework.Assert.AreEqual(string.Empty, prop.GetProperty("prop.empty"));
			// with defaults
			NUnit.Framework.Assert.AreEqual("psimple", prop.GetProperty("prop.simple", "default"));
			NUnit.Framework.Assert.AreEqual("default", prop.GetProperty("prop.doesnotexist", "default"));
			// does not exist, without defaults
			NUnit.Framework.Assert.IsNull(prop.GetProperty("prop.doesnotexist"));
		}

		[Test]
		public virtual void TestPropertiesFromFileNotFound()
		{
			Properties prop = new PropertiesFactory().GetPropertiesFromFilename("does-not-exist.properties");
			NUnit.Framework.Assert.IsNull(prop);
		}

		// java only
		[Test]
		public virtual void TestReadPropertiesFromClassPath()
		{
			if (!Parameters.IsJava())
			{
				return;
			}
			string propFile = "test-classpath.properties";
			Properties prop = new PropertiesFactory().GetPropertiesFromClassPath(propFile);
			NUnit.Framework.Assert.AreEqual("psimple", prop.GetProperty("prop.simple"));
		}

		[Test]
		public virtual void TestReadPropertiesFromClassPathNotFound()
		{
			if (!Parameters.IsJava())
			{
				return;
			}
			Properties prop = new PropertiesFactory().GetPropertiesFromClassPath("does-not-exist.properties");
			NUnit.Framework.Assert.IsNull(prop);
		}
	}
}
