package giis.portable;

import static org.junit.Assert.assertEquals;

import org.junit.Test;

import giis.portable.util.JavaCs;
import giis.portable.util.Parameters;
import giis.portable.util.PortableException;
import giis.portable.util.Versions;

public class TestVersions {

	@Test
	public void testVersionOfThisArtifact() {
		// The portable approach by specifying a class and artifact name
		// Update when major or minor version changes
		String version = new Versions(new PortableException("").getClass(), "io.github.javiertuya", "portable-java").getVersion();
		String[] items = JavaCs.splitByDot(version);
		assertEquals("2", items[0]);
		assertEquals("1", items[1]);

		// java only, does not need specify any class
		if (Parameters.isJava()) {
			String jversion = new Versions(null, "io.github.javiertuya", "portable-java").getVersion();
			assertEquals(version, jversion);
		}

		// net only, does not need specify artifact
		if (Parameters.isNetCore()) {
			String nversion = new Versions(new PortableException("").getClass(), null, null).getVersion();
			assertEquals(version, nversion);
		}
	}

	@Test
	public void testVersionUnknownArtifactGivesFallback() {
		String version = new Versions(null, "group-does-not-exist", "artifact-does-not-exist").getVersion();
		assertEquals("0.0.0", version);
	}
}
