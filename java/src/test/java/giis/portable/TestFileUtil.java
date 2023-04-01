package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.fail;

import java.util.List;

import org.junit.Test;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;
import giis.portable.util.Parameters;

public class TestFileUtil {

	private String getNewTempDirectory() {
		String basePath = Parameters.getReportSubdir(); // target or reports (java, net)
		String path = FileUtil.getPath(basePath, "test-files", JavaCs.getUniqueId());
		FileUtil.createDirectory(path);
		return path;
	}

	@Test
	public void testReadAndWriteScenario() {
		String path = getNewTempDirectory();

		FileUtil.fileWrite(path + "/test.txt", "xxx");
		assertEquals("xxx", FileUtil.fileRead(path + "/test.txt"));

		// overwrite existingfile
		FileUtil.fileWrite(path, "test.txt", "abc\ndef");
		assertEquals("abc\ndef", FileUtil.fileRead(path, "test.txt"));
		assertEquals("abc\ndef", FileUtil.fileRead(path + "/test.txt"));

		List<String> lines = FileUtil.fileReadLines(path, "test.txt");
		assertEquals(2, lines.size());
		assertEquals("abc", lines.get(0));
		assertEquals("def", lines.get(1));

		// append and copy
		FileUtil.fileAppend(path, "test.txt", "\nxyz");
		assertEquals("abc\ndef\nxyz", FileUtil.fileRead(path, "test.txt"));

		FileUtil.fileAppend(path, "test2.txt", "\nxyz"); // creates file if does not exist
		assertEquals("\nxyz", FileUtil.fileRead(path, "test2.txt"));

		FileUtil.copyFile(path + "/test.txt", path + "/test3.txt");
		assertEquals("abc\ndef\nxyz", FileUtil.fileRead(path, "test3.txt"));
	}

	@Test
	public void testDirectoryContentScenario() {
		String path = getNewTempDirectory();

		FileUtil.fileWrite(path + "/test.txt", "111");
		FileUtil.fileWrite(path + "/test3.txt", "333");
		FileUtil.fileWrite(path + "/test2.txt", "222");

		// contents of the directory (list and delete), must be sorted by name
		List<String> fileList = FileUtil.getFileListInDirectory(path);
		assertEquals("[test.txt, test2.txt, test3.txt]", JavaCs.deepToString(JavaCs.toArray(fileList)));

		fileList = FileUtil.getFilesMatchingWildcard(path, "*.*");
		assertEquals("[test.txt, test2.txt, test3.txt]", JavaCs.deepToString(JavaCs.toArray(fileList)));
		fileList = FileUtil.getFilesMatchingWildcard(path, "test2.*");
		assertEquals("[test2.txt]", JavaCs.deepToString(JavaCs.toArray(fileList)));

		fileList = FileUtil.getFilesMatchingWildcard(FileUtil.getFullPath(path), "test3.*");
		assertEquals("[test3.txt]", JavaCs.deepToString(JavaCs.toArray(fileList)));

		FileUtil.deleteFilesInDirectory(path);
		fileList = FileUtil.getFileListInDirectory(path);
		assertEquals("[]", JavaCs.deepToString(JavaCs.toArray(fileList)));
	}

	@Test
	public void testReadFileDoesNotExist() {
		assertNull(FileUtil.fileRead("target", "file-does-not-exist.tmp", false));
		try {
			FileUtil.fileRead("target", "file-does-not-exist.tmp", true);
			fail("Should raise exception");
		} catch (RuntimeException e) {
		}
	}
	@Test
	public void testReadLinesDoesNotExist() {
		try {
			FileUtil.fileReadLines("target", "file-does-not-exist.tmp");
			fail("Should raise exception");
		} catch (RuntimeException e) {
		}
	}
	@Test
	public void testCopySourceDoesNotExist() {
		try {
			FileUtil.copyFile("target/file-does-not-exist.tmp", "target/x.tmp");
			fail("Should raise exception");
		} catch (RuntimeException e) {
		}
	}
	@Test
	public void testDirectoryDoesNotExist() {
		try {
			FileUtil.getFileListInDirectory("directory-does-not-exist"); // invalid path
			fail("Should raise exception");
		} catch (RuntimeException e) {
		}
	}

	@Test
	public void testRelativePaths() {
		// file located in a path that must be always available when reading using
		// different paths that on net files are located under bin directory by default
		String testpaths = FileUtil.getPath("target", "test-paths");
		FileUtil.createDirectory(testpaths);
		FileUtil.fileWrite(testpaths, "check.txt", "xxx");
		assertEquals("xxx", FileUtil.fileRead(FileUtil.getPath(testpaths), "check.txt"));

		// relative after base
		assertEquals("xxx", FileUtil.fileRead(FileUtil.getPath(testpaths, "."), "check.txt"));
		assertEquals("xxx", FileUtil.fileRead(FileUtil.getPath(testpaths, "..", "test-paths"), "check.txt"));
		assertEquals("xxx", FileUtil.fileRead(FileUtil.getPath(testpaths, "test-paths", ".."), "check.txt"));

		// relative before base
		String parentFull = FileUtil.getFullPath(".").replace("\\", "/"); // for windows compatibility
		String[] parents = JavaCs.splitByChar(parentFull, '/');
		String parent = parents[parents.length - 1];
		assertEquals("xxx", FileUtil.fileRead(FileUtil.getPath(".", testpaths), "check.txt"));
		assertEquals("xxx", FileUtil.fileRead(FileUtil.getPath("..", parent, testpaths), "check.txt"));
	}

}
