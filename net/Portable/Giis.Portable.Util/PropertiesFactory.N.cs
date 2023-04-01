using System;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Creation of Properties objects with exception management (on .net uses
    /// a custom implementation), returns null if not found
    /// </summary>
    public class PropertiesFactory
	{
		public virtual Java.Util.Properties GetPropertiesFromClassPath(string fileName)
		{
            throw new PortableException("PropertiesFactory.GetPropertiesFromClassPath not supported in .NET");
		}

        public virtual Java.Util.Properties GetPropertiesFromSystemProperty(string fileName, string defaultFileName)
        {
            throw new PortableException("PropertiesFactory.GetPropertiesFromSystemProperty not supported in .NET");
        }

        public virtual Java.Util.Properties GetPropertiesFromFilename(string fileName)
		{
            try
            {
                Java.Util.Properties prop = new Java.Util.Properties();
                prop.Load(fileName);
                return prop;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
