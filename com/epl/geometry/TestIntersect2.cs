

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestIntersect2
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
		public virtual void testIntersectBetweenPolylineAndPolygon()
		{
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-130, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-130, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 20));
			com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
			compPl.startPath(-116, 20);
			compPl.lineTo(-131, 10);
			compPl.lineTo(-121, 50);
			com.esri.core.geometry.Geometry intersectGeom = null;
			int noException = 1;
			// no exception
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
		}

		// Geometry[] geometries = new Geometry[1];
		// geometries[0] = basePl;
		// BorgGeometryUtils.getIntersectFromRestWS(geometries, compPl, 4326);
		[NUnit.Framework.Test]
		public virtual void testIntersectBetweenPolylines()
		{
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-130, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-120, 50));
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			try
			{
				com.esri.core.geometry.Geometry intersectGeom = com.esri.core.geometry.GeometryEngine
					.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPolyline1()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-116, 20);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.Point);
			com.esri.core.geometry.Point ip = (com.esri.core.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(-116, 0.1E7, ip.getX());
			NUnit.Framework.Assert.AreEqual(20, 0.1E7, ip.getY());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPolyline2()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-115, 20);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPolygon1()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-116, 20);
			com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.Point);
			com.esri.core.geometry.Point ip = (com.esri.core.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(-116, 0.1E7, ip.getX());
			NUnit.Framework.Assert.AreEqual(20, 0.1E7, ip.getY());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPolygon2()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-115, 20);
			com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPolygon3()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-121, 20);
			com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.Point);
			com.esri.core.geometry.Point ip = (com.esri.core.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(-121, 0.1E7, ip.getX());
			NUnit.Framework.Assert.AreEqual(20, 0.1E7, ip.getY());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPoint1()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-116, 20);
			com.esri.core.geometry.Point compPl = new com.esri.core.geometry.Point(-116, 20);
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.Point);
			com.esri.core.geometry.Point ip = (com.esri.core.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(-116, 0.1E7, ip.getX());
			NUnit.Framework.Assert.AreEqual(20, 0.1E7, ip.getY());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndPoint2()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-115, 20);
			com.esri.core.geometry.Point compPl = new com.esri.core.geometry.Point(-116, 20);
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndMultiPoint1()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-116, 20);
			com.esri.core.geometry.MultiPoint compPl = new com.esri.core.geometry.MultiPoint(
				);
			compPl.add(new com.esri.core.geometry.Point(-116, 20));
			compPl.add(new com.esri.core.geometry.Point(-118, 21));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.Point);
			com.esri.core.geometry.Point ip = (com.esri.core.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(-116, 0.1E7, ip.getX());
			NUnit.Framework.Assert.AreEqual(20, 0.1E7, ip.getY());
		}

		[NUnit.Framework.Test]
		public virtual void testPointAndMultiPoint2()
		{
			com.esri.core.geometry.Point basePl = new com.esri.core.geometry.Point(-115, 20);
			com.esri.core.geometry.MultiPoint compPl = new com.esri.core.geometry.MultiPoint(
				);
			compPl.add(new com.esri.core.geometry.Point(-116, 20));
			compPl.add(new com.esri.core.geometry.Point(-117, 21));
			compPl.add(new com.esri.core.geometry.Point(-118, 20));
			compPl.add(new com.esri.core.geometry.Point(-119, 21));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointAndMultiPoint1()
		{
			com.esri.core.geometry.MultiPoint basePl = new com.esri.core.geometry.MultiPoint(
				);
			basePl.add(new com.esri.core.geometry.Point(-116, 20));
			basePl.add(new com.esri.core.geometry.Point(-117, 20));
			com.esri.core.geometry.MultiPoint compPl = new com.esri.core.geometry.MultiPoint(
				);
			compPl.add(new com.esri.core.geometry.Point(-116, 20));
			compPl.add(new com.esri.core.geometry.Point(-118, 21));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.MultiPoint);
			com.esri.core.geometry.MultiPoint imp = (com.esri.core.geometry.MultiPoint)intersectGeom;
			NUnit.Framework.Assert.AreEqual(imp.getCoordinates2D().Length, 1);
			NUnit.Framework.Assert.AreEqual(-116, 0.0, imp.getCoordinates2D()[0].x);
			NUnit.Framework.Assert.AreEqual(20, 0.0, imp.getCoordinates2D()[0].y);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointAndMultiPoint2()
		{
			com.esri.core.geometry.MultiPoint basePl = new com.esri.core.geometry.MultiPoint(
				);
			basePl.add(new com.esri.core.geometry.Point(-116, 20));
			basePl.add(new com.esri.core.geometry.Point(-118, 21));
			com.esri.core.geometry.MultiPoint compPl = new com.esri.core.geometry.MultiPoint(
				);
			compPl.add(new com.esri.core.geometry.Point(-116, 20));
			compPl.add(new com.esri.core.geometry.Point(-118, 21));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.getType() == com.esri.core.geometry.Geometry.Type
				.MultiPoint);
			com.esri.core.geometry.MultiPoint ip = (com.esri.core.geometry.MultiPoint)intersectGeom;
			NUnit.Framework.Assert.AreEqual(-116, 0.1E7, ip.getPoint(0).getX());
			NUnit.Framework.Assert.AreEqual(20, 0.1E7, ip.getPoint(0).getY());
			NUnit.Framework.Assert.AreEqual(-118, 0.1E7, ip.getPoint(0).getX());
			NUnit.Framework.Assert.AreEqual(21, 0.1E7, ip.getPoint(0).getY());
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointAndMultiPoint3()
		{
			com.esri.core.geometry.MultiPoint basePl = new com.esri.core.geometry.MultiPoint(
				);
			basePl.add(new com.esri.core.geometry.Point(-116, 21));
			basePl.add(new com.esri.core.geometry.Point(-117, 20));
			com.esri.core.geometry.MultiPoint compPl = new com.esri.core.geometry.MultiPoint(
				);
			compPl.add(new com.esri.core.geometry.Point(-116, 20));
			compPl.add(new com.esri.core.geometry.Point(-117, 21));
			compPl.add(new com.esri.core.geometry.Point(-118, 20));
			compPl.add(new com.esri.core.geometry.Point(-119, 21));
			int noException = 1;
			// no exception
			com.esri.core.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.esri.core.geometry.GeometryEngine.intersect(basePl, compPl, com.esri.core.geometry.SpatialReference
					.create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.isEmpty());
		}
	}
}
