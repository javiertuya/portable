package giis.portable.util;

/**
 * Platform specific configuration for Java/C# compatibility
 */
public class Parameters { // NOSONAR
	private static final String PLATFORM_NAME = "java";
	public static final String DEFAULT_PROJECT_ROOT = ".";
	public static final String DEFAULT_REPORT_SUBDIR = "target";
	// Path relative to the project root as seen at runtime
	// On java is current folder, on .net is several levels up from current folder
	private static String projectRoot;
	// Path relative to the project root where reports are stored
	private static String reportSubdir;

	static {
		setDefault();
	}

	public static void setDefault() {
		projectRoot = DEFAULT_PROJECT_ROOT;
		reportSubdir = DEFAULT_REPORT_SUBDIR;
	}

	public static String getPlatformName() {
		return PLATFORM_NAME;
	}
	public static boolean isJava() {
		return true;
	}
	public static boolean isNetCore() {
		return false;
	}

	public static String getProjectRoot() {
		return projectRoot;
	}
	public static void setProjectRoot(String root) {
		projectRoot = root;
	}
	public static String getReportSubdir() {
		return reportSubdir;
	}
	public static void setReportSubdir(String subdir) {
		reportSubdir = subdir;
	}

}
