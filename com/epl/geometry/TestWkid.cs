

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestWkid
	{
		[NUnit.Framework.Test]
		public virtual void test()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			NUnit.Framework.Assert.IsTrue(sr.getID() == 102100);
			NUnit.Framework.Assert.IsTrue(sr.getLatestID() == 3857);
			NUnit.Framework.Assert.IsTrue(sr.getOldID() == 102100);
			NUnit.Framework.Assert.IsTrue(sr.getTolerance() == 0.001);
			com.esri.core.geometry.SpatialReference sr84 = com.esri.core.geometry.SpatialReference
				.create(4326);
			double tol84 = sr84.getTolerance();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(tol84 - 1e-8) < 1e-8 * 1e-8);
		}

		[NUnit.Framework.Test]
		public virtual void test_80()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(3857);
			NUnit.Framework.Assert.IsTrue(sr.getID() == 3857);
			NUnit.Framework.Assert.IsTrue(sr.getLatestID() == 3857);
			NUnit.Framework.Assert.IsTrue(sr.getOldID() == 102100);
			NUnit.Framework.Assert.IsTrue(sr.getTolerance() == 0.001);
		}
	}
}
