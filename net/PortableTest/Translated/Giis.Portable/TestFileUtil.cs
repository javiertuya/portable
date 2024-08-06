/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Giis.Portable.Util;
using NUnit.Framework;
using Sharpen;

namespace Giis.Portable
{
	public class TestFileUtil
	{
		private string GetNewTempDirectory()
		{
			string basePath = Parameters.GetReportSubdir();
			// target or reports (java, net)
			string path = FileUtil.GetPath(basePath, "test-files", JavaCs.GetUniqueId());
			FileUtil.CreateDirectory(path);
			return path;
		}

		[Test]
		public virtual void TestReadAndWriteScenario()
		{
			string path = GetNewTempDirectory();
			FileUtil.FileWrite(path + "/test.txt", "xxx");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(path + "/test.txt"));
			// overwrite existingfile
			FileUtil.FileWrite(path, "test.txt", "abc\ndef");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc\ndef", FileUtil.FileRead(path, "test.txt"));
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc\ndef", FileUtil.FileRead(path + "/test.txt"));
			IList<string> lines = FileUtil.FileReadLines(path, "test.txt");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, lines.Count);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc", lines[0]);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("def", lines[1]);
			IList<string> lines2 = FileUtil.FileReadLines(path + "/test.txt");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(2, lines2.Count);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc", lines[0]);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("def", lines[1]);
			// append and copy
			FileUtil.FileAppend(path, "test.txt", "\nxyz");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc\ndef\nxyz", FileUtil.FileRead(path, "test.txt"));
			FileUtil.FileAppend(path, "test2.txt", "\nxyz");
			// creates file if does not exist
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("\nxyz", FileUtil.FileRead(path, "test2.txt"));
			FileUtil.CopyFile(path + "/test.txt", path + "/test3.txt");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("abc\ndef\nxyz", FileUtil.FileRead(path, "test3.txt"));
		}

		[Test]
		public virtual void TestDirectoryContentScenario()
		{
			string path = GetNewTempDirectory();
			FileUtil.FileWrite(path + "/test.txt", "111");
			FileUtil.CreateDirectory(path + "/this-is-a-folder");
			FileUtil.FileWrite(path + "/test3.txt", "333");
			FileUtil.CreateDirectory(path + "/test3.folder");
			FileUtil.FileWrite(path + "/test2.txt", "222");
			// contents of the directory (list and delete), must be sorted by name
			IList<string> fileList = FileUtil.GetFileListInDirectory(path);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("[test.txt, test2.txt, test3.txt]", JavaCs.DeepToString(JavaCs.ToArray(fileList)));
			fileList = FileUtil.GetFilesMatchingWildcard(path, "*.*");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("[test.txt, test2.txt, test3.txt]", JavaCs.DeepToString(JavaCs.ToArray(fileList)));
			fileList = FileUtil.GetFilesMatchingWildcard(path, "test2.*");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("[test2.txt]", JavaCs.DeepToString(JavaCs.ToArray(fileList)));
			fileList = FileUtil.GetFilesMatchingWildcard(FileUtil.GetFullPath(path), "test3.*");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("[test3.txt]", JavaCs.DeepToString(JavaCs.ToArray(fileList)));
			FileUtil.DeleteFilesInDirectory(path);
			fileList = FileUtil.GetFileListInDirectory(path);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("[]", JavaCs.DeepToString(JavaCs.ToArray(fileList)));
		}

		[Test]
		public virtual void TestReadFileDoesNotExist()
		{
			NUnit.Framework.Legacy.ClassicAssert.IsNull(FileUtil.FileRead("target", "file-does-not-exist.tmp", false));
			try
			{
				FileUtil.FileRead("target", "file-does-not-exist.tmp", true);
				NUnit.Framework.Assert.Fail("Should raise exception");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestReadLinesDoesNotExist()
		{
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, FileUtil.FileReadLines("target", "file-does-not-exist.tmp", false).Count);
			try
			{
				FileUtil.FileReadLines("target", "file-does-not-exist.tmp", true);
				NUnit.Framework.Assert.Fail("Should raise exception");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestCopySourceDoesNotExist()
		{
			try
			{
				FileUtil.CopyFile("target/file-does-not-exist.tmp", "target/x.tmp");
				NUnit.Framework.Assert.Fail("Should raise exception");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestDirectoryDoesNotExist()
		{
			try
			{
				FileUtil.GetFileListInDirectory("directory-does-not-exist");
				// invalid path
				NUnit.Framework.Assert.Fail("Should raise exception");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestRelativePaths()
		{
			// file located in a path that must be always available when reading using
			// different paths that on net files are located under bin directory by default
			string testpaths = FileUtil.GetPath("target", "test-paths");
			FileUtil.CreateDirectory(testpaths);
			FileUtil.FileWrite(testpaths, "check.txt", "xxx");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(FileUtil.GetPath(testpaths), "check.txt"));
			// relative after base
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(FileUtil.GetPath(testpaths, "."), "check.txt"));
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(FileUtil.GetPath(testpaths, "..", "test-paths"), "check.txt"));
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(FileUtil.GetPath(testpaths, "test-paths", ".."), "check.txt"));
			// relative before base
			string parentFull = FileUtil.GetFullPath(".").Replace("\\", "/");
			// for windows compatibility
			string[] parents = JavaCs.SplitByChar(parentFull, '/');
			string parent = parents[parents.Length - 1];
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(FileUtil.GetPath(".", testpaths), "check.txt"));
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("xxx", FileUtil.FileRead(FileUtil.GetPath("..", parent, testpaths), "check.txt"));
		}
	}
}
