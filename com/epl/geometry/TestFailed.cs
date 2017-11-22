

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestFailed
	{
		/// <exception cref="System.Exception"/>
		protected override void setUp()
		{
			base.setUp();
		}

		/// <exception cref="System.Exception"/>
		protected override void tearDown()
		{
			base.tearDown();
		}

		[NUnit.Framework.Test]
		public virtual void testCenterXY()
		{
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(-130, 30
				, -70, 50);
			NUnit.Framework.Assert.AreEqual(env.getCenterX(), 0, -100);
			NUnit.Framework.Assert.AreEqual(env.getCenterY(), 0, 40);
		}

		[NUnit.Framework.Test]
		public virtual void testGeometryOperationSupport()
		{
			com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(-130, 
				10);
			com.esri.core.geometry.Geometry comparisonGeom = new com.esri.core.geometry.Point
				(-130, 10);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Geometry diffGeom = null;
			int noException = 1;
			// no exception
			try
			{
				diffGeom = com.esri.core.geometry.GeometryEngine.difference(baseGeom, comparisonGeom
					, sr);
			}
			catch (System.ArgumentException)
			{
				noException = 0;
			}
			catch (com.esri.core.geometry.GeometryException)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Test]
		public virtual void TestIntersection()
		{
			com.esri.core.geometry.OperatorIntersects op = (com.esri.core.geometry.OperatorIntersects
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Intersects);
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			// outer ring1
			polygon.startPath(0, 0);
			polygon.lineTo(10, 10);
			polygon.lineTo(20, 0);
			com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point(15, 10);
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point(2, 10);
			com.esri.core.geometry.Point point3 = new com.esri.core.geometry.Point(5, 5);
			bool res = op.execute(polygon, point1, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = op.execute(polygon, point2, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = op.execute(polygon, point3, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}
	}
}
