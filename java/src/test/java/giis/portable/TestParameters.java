package giis.portable;

import static org.junit.Assert.assertEquals;

import org.junit.After;
import org.junit.Test;

import giis.portable.util.Parameters;

public class TestParameters {

	@After
	public void tearDown() {
		Parameters.setDefault();
	}

	@Test
	public void testPlatformParameters() {
		if (Parameters.isJava()) {
			assertEquals("java", Parameters.getPlatformName());
			assertEquals(".", Parameters.getProjectRoot());
			assertEquals("target", Parameters.getReportSubdir());
		}
		if (Parameters.isNetCore()) {
			assertEquals("netcore", Parameters.getPlatformName());
			assertEquals("../../../..", Parameters.getProjectRoot().replace("\\", "/"));
			assertEquals("reports", Parameters.getReportSubdir());
		}
		Parameters.setProjectRoot("custom-root");
		assertEquals("custom-root", Parameters.getProjectRoot());
		Parameters.setReportSubdir("custom-subdir");
		assertEquals("custom-subdir", Parameters.getReportSubdir());
	}

}
