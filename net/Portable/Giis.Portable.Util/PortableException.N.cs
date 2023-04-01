using System;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Generic exception for Java/C# compatibility
    /// </summary>
    public class PortableException : Exception
	{
		public PortableException(Exception e) : base(e.Message) { }

		public PortableException(string message) : base(message) { }

		public PortableException(string message, Exception cause) : base(message, cause) { }
	}
}
