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
    public class TestProperties
    {
        [Test]
        public virtual void TestReadProperties()
        {
            string propFile = FileUtil.GetPath(Parameters.GetProjectRoot(), "..", "test-file.properties");
            Properties prop = new PropertiesFactory().GetPropertiesFromFilename(propFile);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("psimple", prop.GetProperty("prop.simple"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("pspaces", prop.GetProperty("prop.spaces"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("", prop.GetProperty("prop.empty"));

            // with defaults
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("psimple", prop.GetProperty("prop.simple", "default"));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("default", prop.GetProperty("prop.doesnotexist", "default"));

            // does not exist, without defaults
            NUnit.Framework.Legacy.ClassicAssert.IsNull(prop.GetProperty("prop.doesnotexist"));
        }

        [Test]
        public virtual void TestPropertiesFromFileNotFound()
        {
            Properties prop = new PropertiesFactory().GetPropertiesFromFilename("does-not-exist.properties");
            NUnit.Framework.Legacy.ClassicAssert.IsNull(prop);
        }

        // java only
        [Test]
        public virtual void TestReadPropertiesFromClassPath()
        {
            if (!Parameters.IsJava())
                return;

            // If running from eclipse and fails, run from maven before and then refresh
            string propFile = "test-classpath.properties";
            Properties prop = new PropertiesFactory().GetPropertiesFromClassPath(propFile);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("psimple", prop.GetProperty("prop.simple"));
        }

        [Test]
        public virtual void TestReadPropertiesFromClassPathNotFound()
        {
            if (!Parameters.IsJava())
                return;
            Properties prop = new PropertiesFactory().GetPropertiesFromClassPath("does-not-exist.properties");
            NUnit.Framework.Legacy.ClassicAssert.IsNull(prop);
        }
    }
}