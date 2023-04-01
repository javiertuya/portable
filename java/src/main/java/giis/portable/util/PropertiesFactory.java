package giis.portable.util;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URL;
import java.nio.charset.Charset;

/**
 * Creation of Properties objects with exception management (on .net there will
 * be a custom implementation), returns null if not found
 */
public class PropertiesFactory {

	public java.util.Properties getPropertiesFromClassPath(String fileName) {
		try {
			InputStream stream = this.getClass().getClassLoader().getResourceAsStream(fileName);
			if (stream != null) {
				java.util.Properties prop = new java.util.Properties();
				prop.load(stream);
				return prop;
			}
		} catch (IOException e) {
			return null;
		}
		return null;
	}

	public java.util.Properties getPropertiesFromFilename(String fileName) {
		try {
			java.util.Properties prop = new java.util.Properties();
			prop.load(new FileInputStream(fileName)); // NOSONAR no try-with-resources for java 1.6 compat
			return prop;
		} catch (IOException e) {
			return null;
		}
	}

	public java.util.Properties getPropertiesFromSystemProperty(String fileName, String defaultFileName) {
		try {
			String propsFileName = System.getProperty(fileName, defaultFileName);
			java.util.Properties prop = new java.util.Properties();
			File fp;
			URL url = null;
			fp = new File(propsFileName);
			if (fp.exists()) {
				url = fp.toURI().toURL();
				String optionsFileCharsetProperty = fileName.concat(".charset");
				String charsetName = System.getProperty(optionsFileCharsetProperty, Charset.defaultCharset().name());
				InputStream in = url.openStream(); // NOSONAR for java 1.6 compat
				prop.load(new InputStreamReader(in, charsetName)); // NOSONAR for java 1.6 compat
				return prop;
			}
			return null;
		} catch (IOException e) {
			return null;
		}
	}

}
