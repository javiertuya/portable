package giis.portable;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;

import java.util.Properties;

import org.junit.Test;

import giis.portable.util.FileUtil;
import giis.portable.util.Parameters;
import giis.portable.util.PropertiesFactory;

public class TestProperties {

	@Test
	public void testReadProperties() {
		String propFile = FileUtil.getPath(Parameters.getProjectRoot(), "..", "test-file.properties");
		Properties prop = new PropertiesFactory().getPropertiesFromFilename(propFile);
		assertEquals("psimple", prop.getProperty("prop.simple"));
		assertEquals("pspaces", prop.getProperty("prop.spaces"));
		assertEquals("", prop.getProperty("prop.empty"));
		// with defaults
		assertEquals("psimple", prop.getProperty("prop.simple", "default"));
		assertEquals("default", prop.getProperty("prop.doesnotexist", "default"));
		// does not exist, without defaults
		assertNull(prop.getProperty("prop.doesnotexist"));
	}

	@Test
	public void testPropertiesFromFileNotFound() {
		Properties prop = new PropertiesFactory().getPropertiesFromFilename("does-not-exist.properties");
		assertNull(prop);
	}

	// java only

	@Test
	public void testReadPropertiesFromClassPath() {
		if (!Parameters.isJava())
			return;
		// If running from eclipse and fails, run from maven before and then refresh
		String propFile = "test-classpath.properties";
		Properties prop = new PropertiesFactory().getPropertiesFromClassPath(propFile);
		assertEquals("psimple", prop.getProperty("prop.simple"));
	}

	@Test
	public void testReadPropertiesFromClassPathNotFound() {
		if (!Parameters.isJava())
			return;
		Properties prop = new PropertiesFactory().getPropertiesFromClassPath("does-not-exist.properties");
		assertNull(prop);
	}

}
