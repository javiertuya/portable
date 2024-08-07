package giis.portable;

import static org.junit.Assert.assertEquals;

import org.junit.Test;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;

public class TestResolve {

	// More tests on resolution of the source file location
	@Test
	public void testSourcePathResolution() {
		assertPath("/a/b/c/x/y/Clazz.java", "/a/b/c", "", "x/y/Clazz.java");
		assertPath("a/b/c/x/y/Clazz.java", "a/b/c", "", "x/y/Clazz.java");
		assertPath("a/b/c/x/y/Clazz.java", "a/b/c/", "", "x/y/Clazz.java");
		assertPath("a/b/c/x/y/Clazz.java", "a\\b\\c", "", "x\\y\\Clazz.java");
		assertPath("c:/a/b/c/x/y/Clazz.java", "c:\\a\\b\\c", "", "x\\y\\Clazz.java");
		
		// if no source folder or file, returns empty (no resolved)
		assertPath("", "", "", "x/y/Clazz.java");
		assertPath("", null, "", "x/y/Clazz.java");
		assertPath("", "", null, "x/y/Clazz.java");
		assertPath("", "a/b/c", "", "");
		assertPath("", "a/b/c", "", null);

		// current folder
		boolean isJava = giis.portable.util.Parameters.isJava();
		assertPath(isJava ? full("x/y/Clazz.java") : "./x/y/Clazz.java", ".", "", "x/y/Clazz.java");
		assertPath(isJava ? full("x/y/Clazz.java") : "./x/y/Clazz.java", "./", "", "x/y/Clazz.java");
	}
	@Test
	public void testSourcePathResolutionAbsoluteProjectLocation() {
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "/x", "/x/y/Clazz.java");
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "/x/", "/x/y/Clazz.java");
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "\\x", "/x/y/Clazz.java");
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "\\x\\", "/x/y/Clazz.java");
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "\\x\\", "\\x\\y\\Clazz.java");
	}
	@Test
	public void testSourcePathResolutionNotResolved() {
		// project folder not included in full path, not found
		assertPath("", "/a/b/c", "/w", "/x/y/Clazz.java");
	}
	@Test
	public void testSourcePathResolutionRelativeProjectLocation() {
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "x", FileUtil.getFullPath("x/y/Clazz.java"));
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "x/", FileUtil.getFullPath("x/y/Clazz.java"));
		assertPath("/a/b/c/y/Clazz.java", "/a/b/c", "./x", FileUtil.getFullPath("x/y/Clazz.java"));
	}
	
	public String resolveSourcePath(String sourceFolder, String projectFolder, String sourceFile) {
		System.out.println("Resolve: "+sourceFolder+" "+projectFolder+" "+sourceFile);
		sourceFolder = JavaCs.isEmpty(sourceFolder) ? "" : sourceFolder.trim();
		projectFolder = JavaCs.isEmpty(projectFolder) ? "" : projectFolder.trim();
		sourceFile = JavaCs.isEmpty(sourceFile) ? "" : sourceFile.trim();
		
		// Source folder and source file are required, if not, the empty return means that source can't be fount
		if ("".equals(sourceFolder) || "".equals(sourceFile))
			return ""; 
		
		// If projectFolder specified (for net generated coverage), it is expected a full path source File
		// that is converted to relative to projectFolder
		if (!"".equals(projectFolder)) {
			sourceFile = FileUtil.getFullPath(sourceFile).replace("\\", "/");
			String prefix = FileUtil.getFullPath(projectFolder).replace("\\", "/");
			if (!prefix.endsWith("/"))
				prefix = prefix + "/";
			System.out.println(prefix);
			System.out.println(sourceFile);
			System.out.println(sourceFile.startsWith(prefix));
			if (sourceFile.startsWith(prefix))
				sourceFile = JavaCs.substring(sourceFile, prefix.length(), sourceFile.length());
			else
				return "";
		}
		return FileUtil.getPath(sourceFolder, sourceFile);
	}

	private String full(String filename) {
		return FileUtil.getFullPath(filename).replace("\\", "/");
	}
	private void assertPath(String expected, String source, String project, String file) {
		assertEquals(expected, resolveSourcePath(source, project, file).replace("\\", "/"));
	}
}
