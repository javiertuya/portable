/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Portable.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestResolve
	{
		// More tests on resolution of the source file location
		[Test]
		public virtual void TestNoProjectLocResolved()
		{
			AssertPath("/a/b/c/x/y/Clazz.java", "/a/b/c", string.Empty, "x/y/Clazz.java");
			AssertPath("a/b/c/x/y/Clazz.java", "a/b/c", string.Empty, "x/y/Clazz.java");
			AssertPath("a/b/c/x/y/Clazz.java", "a/b/c/", string.Empty, "x/y/Clazz.java");
			AssertPath("a/b/c/x/y/Clazz.java", "a\\b\\c", string.Empty, "x\\y\\Clazz.java");
			AssertPath("c:/a/b/c/x/y/Clazz.java", "c:\\a\\b\\c", string.Empty, "x\\y\\Clazz.java");
			// current folder
			bool isJava = Parameters.IsJava();
			AssertPath(isJava ? Full("x/y/Clazz.java") : "./x/y/Clazz.java", ".", string.Empty, "x/y/Clazz.java");
			AssertPath(isJava ? Full("x/y/Clazz.java") : "./x/y/Clazz.java", "./", string.Empty, "x/y/Clazz.java");
		}

		[Test]
		public virtual void TestNoProjectLocUnresolved()
		{
			// if no source folder or file, returns empty (no resolved)
			AssertPath(string.Empty, string.Empty, string.Empty, "x/y/Clazz.java");
			AssertPath(string.Empty, null, string.Empty, "x/y/Clazz.java");
			AssertPath(string.Empty, string.Empty, null, "x/y/Clazz.java");
			AssertPath(string.Empty, "a/b/c", string.Empty, string.Empty);
			AssertPath(string.Empty, "a/b/c", string.Empty, null);
		}

		[Test]
		public virtual void TestWithProjectLocAbsoluteResolved()
		{
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "/x", "/x/y/Clazz.java");
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "/x/", "/x/y/Clazz.java");
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "\\x", "/x/y/Clazz.java");
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "\\x\\", "/x/y/Clazz.java");
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "\\x\\", "\\x\\y\\Clazz.java");
		}

		[Test]
		public virtual void TestWithProjectLocAbsoluteUnresolved()
		{
			// project folder is not included in full path
			AssertPath(string.Empty, "/a/b/c", "/w", "/x/y/Clazz.java");
		}

		[Test]
		public virtual void TestWithProjectLocRelativeResolved()
		{
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "x", FileUtil.GetFullPath("x/y/Clazz.java"));
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "x/", FileUtil.GetFullPath("x/y/Clazz.java"));
			AssertPath("/a/b/c/y/Clazz.java", "/a/b/c", "./x", FileUtil.GetFullPath("x/y/Clazz.java"));
			// current folder
			AssertPath("/a/b/c/x/y/Clazz.java", "/a/b/c", ".", FileUtil.GetFullPath("x/y/Clazz.java"));
			AssertPath("/a/b/c/x/y/Clazz.java", "/a/b/c", "./", FileUtil.GetFullPath("x/y/Clazz.java"));
		}

		[Test]
		public virtual void TestWithProjectLocRelativeUnresolved()
		{
			// project folder is not included in full path
			AssertPath(string.Empty, "/a/b/c", "w", FileUtil.GetFullPath("x/y/Clazz.java"));
		}

		public virtual string ResolveSourcePath(string sourceFolder, string projectFolder, string sourceFile)
		{
			System.Console.Out.WriteLine("Resolve: " + sourceFolder + " " + projectFolder + " " + sourceFile);
			sourceFolder = JavaCs.IsEmpty(sourceFolder) ? string.Empty : sourceFolder.Trim();
			projectFolder = JavaCs.IsEmpty(projectFolder) ? string.Empty : projectFolder.Trim();
			sourceFile = JavaCs.IsEmpty(sourceFile) ? string.Empty : sourceFile.Trim();
			// Source folder and source file are required, if not, the empty return means
			// that source can't be fount
			if (string.Empty.Equals(sourceFolder) || string.Empty.Equals(sourceFile))
			{
				return string.Empty;
			}
			// If projectFolder specified (for net generated coverage), it is expected a
			// full path source File
			// that is converted to relative to projectFolder
			if (!string.Empty.Equals(projectFolder))
			{
				// unifies separators (linux/windows) and simplifies double separators that
				// appear sometimes
				sourceFile = FileUtil.GetFullPath(sourceFile.Replace("\\", "/")).Replace("\\", "/").Replace("//", "/");
				string prefix = FileUtil.GetFullPath(projectFolder.Replace("\\", "/")).Replace("\\", "/").Replace("//", "/");
				if (!prefix.EndsWith("/"))
				{
					prefix = prefix + "/";
				}
				System.Console.Out.WriteLine(prefix);
				System.Console.Out.WriteLine(sourceFile);
				System.Console.Out.WriteLine(sourceFile.StartsWith(prefix));
				if (sourceFile.StartsWith(prefix))
				{
					sourceFile = JavaCs.Substring(sourceFile, prefix.Length, sourceFile.Length);
				}
				else
				{
					return string.Empty;
				}
			}
			return FileUtil.GetPath(sourceFolder, sourceFile);
		}

		private string Full(string filename)
		{
			return FileUtil.GetFullPath(filename).Replace("\\", "/");
		}

		private void AssertPath(string expected, string source, string project, string file)
		{
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(expected, ResolveSourcePath(source, project, file).Replace("\\", "/"));
		}
	}
}
