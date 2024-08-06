package giis.portable.util;

import java.io.File;
import java.io.FileFilter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.apache.commons.io.FilenameUtils; //no usa java.nio.file.Paths por compatibilidad con java 1.6
import org.apache.commons.io.FileUtils;
import org.apache.commons.io.filefilter.WildcardFileFilter;

/**
 * Basic file management Java/C# compatible
 */
public class FileUtil {
	private static final String UTF_8 = "UTF-8";

	private FileUtil() {
		throw new IllegalAccessError("Utility class");
	}

	public static String fileRead(String fileName, boolean throwIfNotExists) {
		try {
			File f = new File(fileName);
			if (f.exists())
				return FileUtils.readFileToString(f, UTF_8);
			if (throwIfNotExists)
				throw new PortableException("File does not exist " + fileName);
			else
				return null;
		} catch (IOException e) {
			throw new PortableException(e);
		}
	}
	public static String fileRead(String path, String name, boolean throwIfNotExists) {
		return fileRead(getPath(path, name), throwIfNotExists);
	}

	public static String fileRead(String path, String name) {
		return fileRead(getPath(path, name), true);
	}
	public static String fileRead(String fileName) {
		return fileRead(fileName, true);
	}
	
	public static List<String> fileReadLines(String fileName) {
		return fileReadLines(fileName, true);
	}
	public static List<String> fileReadLines(String path, String name) {
		return fileReadLines(getPath(path, name), true);
	}
	public static List<String> fileReadLines(String fileName, boolean throwIfNotExists) {
		try {
			return FileUtils.readLines(new File(fileName), UTF_8);
		} catch (IOException e) {
			if (!throwIfNotExists)
				return new ArrayList<>();
			throw new PortableException("Error reading file " + fileName, e);
		}
	}
	public static List<String> fileReadLines(String path, String name, boolean throwIfNotExists) {
		return fileReadLines(getPath(path, name), throwIfNotExists);
	}

	public static void fileWrite(String path, String name, String content) {
		fileWrite(getPath(path, name), content);
	}
	public static void fileWrite(String fileName, String contents) {
		try {
			FileUtils.writeStringToFile(new File(fileName), contents, UTF_8);
		} catch (IOException e) {
			throw new PortableException("Error writing file " + fileName, e);
		}
	}

	public static void fileAppend(String path, String name, String content) {
		fileAppend(getPath(path, name), content);
	}
	public static void fileAppend(String fileName, String line) {
		try {
			FileUtils.writeStringToFile(new File(fileName), line, UTF_8, true);
		} catch (IOException e) {
			throw new PortableException("Error appending to file " + fileName, e);
		}
	}

	public static void copyFile(String source, String dest) {
		copyFile(new File(source), new File(dest));
	}

	public static void copyFile(File source, File dest) {
		try {
			FileUtils.copyFile(source, dest);
		} catch (IOException e) {
			throw new PortableException("Error copying files", e);
		}
	}

	/**
	 * Returns files at the specified folder that match a wildcard
	 */
	public static List<String> getFilesMatchingWildcard(String folder, String fileNameWildcard) {
		File[] files = listMatchingWildcard(folder, fileNameWildcard);
		List<String> names = new ArrayList<>();
		for (File file : files)
			if (!file.isDirectory())
				names.add(file.getName());
		Collections.sort(names); // to make it repetible (linux returns different order than windows)
		return names;
	}
	private static File[] listMatchingWildcard(String folder, String fileNameWildcard) {
		File dir = new File(folder);
		FileFilter fileFilter = WildcardFileFilter.builder().setWildcards(fileNameWildcard).get();
		return dir.listFiles(fileFilter);
	}
	public static List<String> getFileListInDirectory(String path) {
		List<String> lst = new ArrayList<>();
		try {
			File dir = new File(path);
			for (File file : dir.listFiles()) {
				if (!file.isDirectory())
					lst.add(file.getName());
			}
		} catch (RuntimeException e) {
			throw new PortableException("Can't browse directory at path " + path);
		}
		Collections.sort(lst); // to make it repetible (linux returns different order than windows)
		return lst;
	}

	public static void deleteFilesInDirectory(String path) {
		// No recursivo, solo ficheros
		File dir = new File(path);
		if (dir.exists())
			for (File file : dir.listFiles())
				if (!file.isDirectory()) {
					boolean success = file.delete(); // NOSONAR
					if (!success)
						throw new PortableException("Can't delete file " + file.getName());
				}
	}

	public static String getPath(String first, String... more) {
		String result = first;
		// El primer componente no puede empezar de forma relativa como .. (concat
		// devuelve null), por lo que busca el full path primero
		if (result.startsWith("."))
			result = getFullPath(result);
		for (int i = 0; i < more.length; i++)
			result = FilenameUtils.concat(result, more[i]);
		return result;
	}

	// Variant with dynamic array for compatibility with jdk4 retrotranslator
	public static String getPath(String first, String more1) {
		return getPath(first, new String[] { more1 }); // NOSONAR for compatibilidad
	}

	public static String getPath(String first, String more1, String more2) {
		return getPath(first, new String[] { more1, more2 }); // NOSONAR for compatibility
	}
	public static String getFullPath(String path) {
		try {
			return new File(path).getCanonicalPath();
		} catch (IOException e) {
			throw new PortableException("Error getting full path of " + path, e);
		}
	}

	public static void createDirectory(String path) {
		try {
			FileUtils.forceMkdir(new File(path));
		} catch (IOException e) {
			throw new PortableException(e);
		}
	}

}
