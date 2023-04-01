package giis.portable.util;

import java.io.InputStream;
import java.util.Properties;

/**
 * Allows determining the version number of java artifacts and net assemblies
 */
public class Versions {
	private final String currentVersion;

	/**
	 * Platform compatible instantiation (not required parameters may be null):
	 * - On java: requires the group and artifact ids (version is obtained from
	 *   the META-INF folder created by maven archiver).
	 * - On net: requires a class in the assembly (version is obtained from
	 *   the InformationalVersion field).
	 * If no version can be found the getVersion method returns 0.0.0
	 */
	public Versions(@SuppressWarnings("rawtypes") Class target, String groupId, String artifactId) { // NOSONAR
		this(groupId, artifactId);
	}

	/**
	 * Java only instantiation
	 */
	public Versions(String groupId, String artifactId) {
		this.currentVersion = getVersion(getClass(),
				"/META-INF/maven/" + groupId + "/" + artifactId + "/pom.properties");
	}

	/**
	 * Gets a string with the version number
	 */
	public String getVersion() {
		return this.currentVersion;
	}

	private String getVersion(@SuppressWarnings("rawtypes") Class target, String pomResourcePath) {
		String version = null;
		// First looks at the META-INF path, if fails, looks at the manifest
		// https://stackoverflow.com/questions/2712970/get-maven-artifact-version-at-runtime
		try {
			Properties p = new Properties();
			InputStream is = target.getResourceAsStream(pomResourcePath);
			if (is != null) {
				p.load(is);
				version = p.getProperty("version", "");
			}
		} catch (Exception e) {
			// ignore
		}

		// fallback to using Java API
		if (version == null) {
			Package aPackage = getClass().getPackage();
			if (aPackage != null) {
				version = aPackage.getImplementationVersion();
				if (version == null) {
					version = aPackage.getSpecificationVersion();
				}
			}
		}

		if (version == null) {
			version = "0.0.0"; // fallback
		}

		return version;
	}
	
}
