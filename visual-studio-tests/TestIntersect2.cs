using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestIntersect2 : NUnit.Framework.TestFixtureAttribute
	{
		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
		}

		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		[NUnit.Framework.Test]
		public virtual void TestIntersectBetweenPolylineAndPolygon()
		{
			com.epl.geometry.Polyline basePl = new com.epl.geometry.Polyline();
			basePl.StartPath(new com.epl.geometry.Point(-117, 20));
			basePl.LineTo(new com.epl.geometry.Point(-117, 10));
			basePl.LineTo(new com.epl.geometry.Point(-130, 10));
			basePl.LineTo(new com.epl.geometry.Point(-130, 20));
			basePl.LineTo(new com.epl.geometry.Point(-117, 20));
			com.epl.geometry.Polygon compPl = new com.epl.geometry.Polygon();
			compPl.StartPath(-116, 20);
			compPl.LineTo(-131, 10);
			compPl.LineTo(-121, 50);
			com.epl.geometry.Geometry intersectGeom = null;
			int noException = 1;
			// no exception
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
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
		public virtual void TestIntersectBetweenPolylines()
		{
			com.epl.geometry.Polyline basePl = new com.epl.geometry.Polyline();
			basePl.StartPath(new com.epl.geometry.Point(-117, 20));
			basePl.LineTo(new com.epl.geometry.Point(-130, 10));
			basePl.LineTo(new com.epl.geometry.Point(-120, 50));
			com.epl.geometry.Polyline compPl = new com.epl.geometry.Polyline();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			try
			{
				com.epl.geometry.Geometry intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPolyline1()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-116, 20);
			com.epl.geometry.Polyline compPl = new com.epl.geometry.Polyline();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.Point);
			com.epl.geometry.Point ip = (com.epl.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(ip.GetX(), -116, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetY(), 20, 0.1E7);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPolyline2()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-115, 20);
			com.epl.geometry.Polyline compPl = new com.epl.geometry.Polyline();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.IsEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPolygon1()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-116, 20);
			com.epl.geometry.Polygon compPl = new com.epl.geometry.Polygon();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.Point);
			com.epl.geometry.Point ip = (com.epl.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(ip.GetX(), -116, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetY(), 20, 0.1E7);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPolygon2()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-115, 20);
			com.epl.geometry.Polygon compPl = new com.epl.geometry.Polygon();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.IsEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPolygon3()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-121, 20);
			com.epl.geometry.Polygon compPl = new com.epl.geometry.Polygon();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.Point);
			com.epl.geometry.Point ip = (com.epl.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(ip.GetX(), -121, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetY(), 20, 0.1E7);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPoint1()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-116, 20);
			com.epl.geometry.Point compPl = new com.epl.geometry.Point(-116, 20);
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.Point);
			com.epl.geometry.Point ip = (com.epl.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(ip.GetX(), -116, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetY(), 20, 0.1E7);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndPoint2()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-115, 20);
			com.epl.geometry.Point compPl = new com.epl.geometry.Point(-116, 20);
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.IsEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndMultiPoint1()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-116, 20);
			com.epl.geometry.MultiPoint compPl = new com.epl.geometry.MultiPoint();
			compPl.Add(new com.epl.geometry.Point(-116, 20));
			compPl.Add(new com.epl.geometry.Point(-118, 21));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.Point);
			com.epl.geometry.Point ip = (com.epl.geometry.Point)intersectGeom;
			NUnit.Framework.Assert.AreEqual(ip.GetX(), -116, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetY(), 20, 0.1E7);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointAndMultiPoint2()
		{
			com.epl.geometry.Point basePl = new com.epl.geometry.Point(-115, 20);
			com.epl.geometry.MultiPoint compPl = new com.epl.geometry.MultiPoint();
			compPl.Add(new com.epl.geometry.Point(-116, 20));
			compPl.Add(new com.epl.geometry.Point(-117, 21));
			compPl.Add(new com.epl.geometry.Point(-118, 20));
			compPl.Add(new com.epl.geometry.Point(-119, 21));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.IsEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointAndMultiPoint1()
		{
			com.epl.geometry.MultiPoint basePl = new com.epl.geometry.MultiPoint();
			basePl.Add(new com.epl.geometry.Point(-116, 20));
			basePl.Add(new com.epl.geometry.Point(-117, 20));
			com.epl.geometry.MultiPoint compPl = new com.epl.geometry.MultiPoint();
			compPl.Add(new com.epl.geometry.Point(-116, 20));
			compPl.Add(new com.epl.geometry.Point(-118, 21));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.MultiPoint);
			com.epl.geometry.MultiPoint imp = (com.epl.geometry.MultiPoint)intersectGeom;
			NUnit.Framework.Assert.AreEqual(imp.GetCoordinates2D().Length, 1);
			NUnit.Framework.Assert.AreEqual(imp.GetCoordinates2D()[0].x, -116, 0.0);
			NUnit.Framework.Assert.AreEqual(imp.GetCoordinates2D()[0].y, 20, 0.0);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointAndMultiPoint2()
		{
			com.epl.geometry.MultiPoint basePl = new com.epl.geometry.MultiPoint();
			basePl.Add(new com.epl.geometry.Point(-116, 20));
			basePl.Add(new com.epl.geometry.Point(-118, 21));
			com.epl.geometry.MultiPoint compPl = new com.epl.geometry.MultiPoint();
			compPl.Add(new com.epl.geometry.Point(-116, 20));
			compPl.Add(new com.epl.geometry.Point(-118, 21));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsNotNull(intersectGeom);
			NUnit.Framework.Assert.IsTrue(intersectGeom.GetType() == com.epl.geometry.Geometry.Type.MultiPoint);
			com.epl.geometry.MultiPoint ip = (com.epl.geometry.MultiPoint)intersectGeom;
			NUnit.Framework.Assert.AreEqual(ip.GetPoint(0).GetX(), -116, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetPoint(0).GetY(), 20, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetPoint(0).GetX(), -118, 0.1E7);
			NUnit.Framework.Assert.AreEqual(ip.GetPoint(0).GetY(), 21, 0.1E7);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointAndMultiPoint3()
		{
			com.epl.geometry.MultiPoint basePl = new com.epl.geometry.MultiPoint();
			basePl.Add(new com.epl.geometry.Point(-116, 21));
			basePl.Add(new com.epl.geometry.Point(-117, 20));
			com.epl.geometry.MultiPoint compPl = new com.epl.geometry.MultiPoint();
			compPl.Add(new com.epl.geometry.Point(-116, 20));
			compPl.Add(new com.epl.geometry.Point(-117, 21));
			compPl.Add(new com.epl.geometry.Point(-118, 20));
			compPl.Add(new com.epl.geometry.Point(-119, 21));
			int noException = 1;
			// no exception
			com.epl.geometry.Geometry intersectGeom = null;
			try
			{
				intersectGeom = com.epl.geometry.GeometryEngine.Intersect(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			}
			catch (System.Exception)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
			NUnit.Framework.Assert.IsTrue(intersectGeom.IsEmpty());
		}
	}
}
