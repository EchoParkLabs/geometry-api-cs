

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestTouch
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
		public virtual void testTouchOnPointAndPolyline()
		{
			com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(-130, 
				10);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			pl.startPath(new com.esri.core.geometry.Point(-130, 10));
			pl.lineTo(-131, 15);
			pl.lineTo(-140, 20);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(baseGeom, pl, sr);
				isTouched2 = com.esri.core.geometry.GeometryEngine.touches(pl, baseGeom, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
				isTouched2 = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched && isTouched2, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchOnPointAndPolygon()
		{
			com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(-130, 
				10);
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(new com.esri.core.geometry.Point(-130, 10));
			pg.lineTo(-131, 15);
			pg.lineTo(-140, 20);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(baseGeom, pg, sr);
				isTouched2 = com.esri.core.geometry.GeometryEngine.touches(pg, baseGeom, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
				isTouched2 = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched && isTouched2, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchOnPolygons()
		{
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(new com.esri.core.geometry.Point(-130, 10));
			pg.lineTo(-131, 15);
			pg.lineTo(-140, 20);
			com.esri.core.geometry.Polygon pg2 = new com.esri.core.geometry.Polygon();
			pg2.startPath(new com.esri.core.geometry.Point(-130, 10));
			pg2.lineTo(-131, 15);
			pg2.lineTo(-120, 20);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(pg, pg2, sr);
				isTouched2 = com.esri.core.geometry.GeometryEngine.touches(pg2, pg, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
				isTouched2 = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched && isTouched2, true);
		}

		// boolean isTouchedFromRest = GeometryUtils.isRelationTrue(pg2, pg, sr,
		// GeometryUtils.SpatialRelationType.esriGeometryRelationTouch, "");
		// assertTrue(isTouchedFromRest==isTouched);
		[NUnit.Framework.Test]
		public virtual void testTouchesOnPolylines()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 20));
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(new com.esri.core.geometry.Point(-104, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-108, 25));
			compPl.lineTo(new com.esri.core.geometry.Point(-100, 20));
			// compPl.lineTo(new Point(-100, 30));
			// compPl.lineTo(new Point(-117, 30));
			// compPl.lineTo(new Point(-117, 20));
			bool isTouched;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(basePl, compPl, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesOnPolylineAndPolygon()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polygon basePl = new com.esri.core.geometry.Polygon();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 10));
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(new com.esri.core.geometry.Point(-117, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-108, 25));
			compPl.lineTo(new com.esri.core.geometry.Point(-100, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-100, 30));
			bool isTouched;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(basePl, compPl, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchOnEnvelopes()
		{
			// case1, not touched
			// Envelope env = new Envelope(new Point(-117,20), 12, 12);
			// Envelope env2 = new Envelope(-100,20,-80,30);
			// case2 touched
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(new com.esri.core.geometry.Point
				(-117, 20), 12, 12);
			com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope(-117, 
				26, -80, 30);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(env, env2, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesOnPolylineAndEnvelope()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 20));
			// Envelope env = new Envelope(new Point(-117,20), 12, 12);//not touched
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(-100, 20
				, -80, 30);
			// touched
			bool isTouched;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(basePl, env, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesOnPolygonAndEnvelope()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polygon basePl = new com.esri.core.geometry.Polygon();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-100, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-117, 10));
			// Envelope env = new Envelope(new Point(-117,20), 12, 12);//not touched
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(-100, 20
				, -80, 30);
			// touched
			bool isTouched;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(basePl, env, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesOnPointAndEnvelope()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(-130, 10);
			// Envelope env = new Envelope(p, 12, 12);//not touched
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(-130, 10
				, -110, 20);
			// touched
			bool isTouched;
			try
			{
				isTouched = com.esri.core.geometry.GeometryEngine.touches(p, env, sr);
			}
			catch (System.ArgumentException)
			{
				isTouched = false;
			}
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testRelationTouch()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(2, 2);
			basePl.lineTo(2, 10);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(2, 4);
			compPl.lineTo(9, 4);
			compPl.lineTo(9, 9);
			compPl.lineTo(2, 9);
			compPl.lineTo(2, 4);
			bool isTouched = false;
			// GeometryEngine.relation(basePl, compPl, sr,
			// "G1 TOUCH G2");
			NUnit.Framework.Assert.AreEqual(isTouched, false);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesBetweenPointAndLine()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(2, 4);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(2, 4);
			compPl.lineTo(9, 4);
			compPl.lineTo(9, 9);
			compPl.lineTo(2, 9);
			compPl.lineTo(2, 4);
			bool isTouched = com.esri.core.geometry.GeometryEngine.touches(p, compPl, sr);
			NUnit.Framework.Assert.IsTrue(!isTouched);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesBetweenPolylines()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			pl.startPath(2, 4);
			pl.lineTo(9, 9);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(2, 4);
			compPl.lineTo(9, 4);
			compPl.lineTo(9, 9);
			compPl.lineTo(2, 9);
			compPl.lineTo(2, 4);
			bool isTouched = com.esri.core.geometry.GeometryEngine.touches(pl, compPl, sr);
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesBetweenPolylineAndPolygon()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			pl.startPath(2, 4);
			pl.lineTo(1, 10);
			pl.lineTo(6, 12);
			com.esri.core.geometry.Polygon compPg = new com.esri.core.geometry.Polygon();
			compPg.startPath(2, 4);
			compPg.lineTo(2, 9);
			compPg.lineTo(9, 9);
			compPg.lineTo(9, 4);
			compPg.startPath(2, 9);
			compPg.lineTo(6, 12);
			compPg.lineTo(9, 10);
			bool isTouched = com.esri.core.geometry.GeometryEngine.touches(pl, compPg, sr);
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchesBetweenMultipartPolylines()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			pl.startPath(2, 4);
			pl.lineTo(1, 10);
			pl.lineTo(6, 12);
			pl.startPath(6, 12);
			pl.lineTo(12, 12);
			pl.lineTo(9, 9);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(2, 4);
			compPl.lineTo(2, 9);
			compPl.lineTo(9, 9);
			compPl.lineTo(9, 4);
			compPl.startPath(2, 9);
			compPl.lineTo(6, 12);
			compPl.lineTo(9, 10);
			bool isTouched = com.esri.core.geometry.GeometryEngine.touches(pl, compPl, sr);
			NUnit.Framework.Assert.IsTrue(!isTouched);
		}

		// boolean isTouchedFromRest = GeometryUtils.isRelationTrue(compPl, pl,
		// sr,
		// GeometryUtils.SpatialRelationType.esriGeometryRelationTouch, "");
		// assertTrue(isTouchedFromRest == isTouched);
		[NUnit.Framework.Test]
		public virtual void testTouchesBetweenMultipartPolygons2()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polygon pl = new com.esri.core.geometry.Polygon();
			pl.startPath(2, 4);
			pl.lineTo(1, 9);
			pl.lineTo(2, 6);
			pl.startPath(2, 9);
			pl.lineTo(6, 14);
			pl.lineTo(6, 12);
			com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
			compPl.startPath(2, 4);
			compPl.lineTo(2, 9);
			compPl.lineTo(9, 9);
			compPl.lineTo(9, 4);
			compPl.startPath(2, 9);
			compPl.lineTo(6, 12);
			compPl.lineTo(9, 10);
			bool isTouched = com.esri.core.geometry.GeometryEngine.touches(pl, compPl, sr);
			NUnit.Framework.Assert.AreEqual(isTouched, true);
		}

		[NUnit.Framework.Test]
		public virtual void testTouchPointLineCR183227()
		{
			// Tests CR 183227
			com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(-130, 
				10);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			// pl.startPath(new Point(-130, 10));
			pl.startPath(-130, 10);
			pl.lineTo(-131, 15);
			pl.lineTo(-140, 20);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = com.esri.core.geometry.GeometryEngine.touches(baseGeom, pl, sr);
			isTouched2 = com.esri.core.geometry.GeometryEngine.touches(pl, baseGeom, sr);
			NUnit.Framework.Assert.IsTrue(isTouched && isTouched2);
			{
				com.esri.core.geometry.Geometry baseGeom2 = (com.esri.core.geometry.Geometry)new 
					com.esri.core.geometry.Point(-131, 15);
				bool bIsTouched;
				bool bIsTouched2;
				bIsTouched = com.esri.core.geometry.GeometryEngine.touches(baseGeom2, pl, sr);
				bIsTouched2 = com.esri.core.geometry.GeometryEngine.touches(pl, baseGeom2, sr);
				NUnit.Framework.Assert.IsTrue(!bIsTouched && !bIsTouched2);
			}
		}
	}
}
