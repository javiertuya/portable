using System;
using System.Reflection;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Allows determining the version number of java artifacts and net assemblies
    /// </summary>
    public class Versions
	{
		private string currentVersion;

        /// <summary>
        /// Platform compatible instantiation (not required parameters may be null):
        /// - On java: requires the group and artifact ids (version is obtained from
        ///   the META-INF folder created by maven archiver).
        /// - On net: requires a class in the assembly (version is obtained from
        ///   the InformationalVersion field).
		/// If no version can be found the getVersion method returns 0.0.0
        /// </summary>
        public Versions(Type target, string groupId, string artifactId)
		{
			currentVersion = GetVersion(target);
		}
		/// <summary>
		/// Net only instantiation
		/// </summary>
		public Versions(Type target)
		{
			currentVersion = GetVersion(target);
		}
        /// <summary>
        /// Gets a string with the version number
        /// </summary>
        public string GetVersion()
		{
			return this.currentVersion;
		}
		private string GetVersion(Type target)
		{
			if (target == null)
				return "0.0.0"; //fallback if no target specified
			string version = target.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
			if (string.IsNullOrEmpty(version))
				version = "0.0.0"; //fallback
			return version;
		}
	}
}
