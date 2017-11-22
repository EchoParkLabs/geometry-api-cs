using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestOGC : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestPoint()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("POINT(1 2)");
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("Point"));
			com.epl.geometry.ogc.OGCPoint p = (com.epl.geometry.ogc.OGCPoint)g;
			NUnit.Framework.Assert.IsTrue(p.X() == 1);
			NUnit.Framework.Assert.IsTrue(p.Y() == 2);
			NUnit.Framework.Assert.IsTrue(g.Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(1 2)")));
			NUnit.Framework.Assert.IsTrue(!g.Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(1 3)")));
			com.epl.geometry.ogc.OGCGeometry buf = g.Buffer(10);
			NUnit.Framework.Assert.IsTrue(buf.GeometryType().Equals("Polygon"));
			com.epl.geometry.ogc.OGCPolygon poly = (com.epl.geometry.ogc.OGCPolygon)buf.Envelope();
			double a = poly.Area();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(a - 400) < 1e-1);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygon()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))");
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("Polygon"));
			com.epl.geometry.ogc.OGCPolygon p = (com.epl.geometry.ogc.OGCPolygon)g;
			NUnit.Framework.Assert.IsTrue(p.NumInteriorRing() == 1);
			com.epl.geometry.ogc.OGCLineString ls = p.ExteriorRing();
			// assertTrue(ls.pointN(1).equals(OGCGeometry.fromText("POINT(10 -10)")));
			bool b = ls.Equals(com.epl.geometry.ogc.OGCGeometry.FromText("LINESTRING(-10 -10, 10 -10, 10 10, -10 10, -10 -10)"));
			NUnit.Framework.Assert.IsTrue(b);
			com.epl.geometry.ogc.OGCLineString lsi = p.InteriorRingN(0);
			b = lsi.Equals(com.epl.geometry.ogc.OGCGeometry.FromText("LINESTRING(-5 -5, -5 5, 5 5, 5 -5, -5 -5)"));
			NUnit.Framework.Assert.IsTrue(b);
			NUnit.Framework.Assert.IsTrue(!lsi.Equals(ls));
			com.epl.geometry.ogc.OGCMultiCurve boundary = ((com.epl.geometry.ogc.OGCMultiCurve)p.Boundary());
			string s = boundary.AsText();
			NUnit.Framework.Assert.IsTrue(s.Equals("MULTILINESTRING ((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"));
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public virtual void TestGeometryCollection()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("GEOMETRYCOLLECTION(POLYGON EMPTY, POINT(1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)");
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("GeometryCollection"));
			com.epl.geometry.ogc.OGCConcreteGeometryCollection gc = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)g;
			NUnit.Framework.Assert.IsTrue(gc.NumGeometries() == 5);
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(0).GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(1).GeometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(2).GeometryType().Equals("LineString"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(3).GeometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(4).GeometryType().Equals("MultiLineString"));
			g = com.epl.geometry.ogc.OGCGeometry.FromText("GEOMETRYCOLLECTION(POLYGON EMPTY, POINT(1 1), GEOMETRYCOLLECTION EMPTY, LINESTRING EMPTY, GEOMETRYCOLLECTION(POLYGON EMPTY, POINT(1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY, MULTIPOINT EMPTY), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				);
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("GeometryCollection"));
			gc = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)g;
			NUnit.Framework.Assert.IsTrue(gc.NumGeometries() == 7);
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(0).GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(1).GeometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(2).GeometryType().Equals("GeometryCollection"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(3).GeometryType().Equals("LineString"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(4).GeometryType().Equals("GeometryCollection"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(5).GeometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(6).GeometryType().Equals("MultiLineString"));
			com.epl.geometry.ogc.OGCConcreteGeometryCollection gc2 = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)gc.GeometryN(4);
			NUnit.Framework.Assert.IsTrue(gc2.NumGeometries() == 6);
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(0).GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(1).GeometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(2).GeometryType().Equals("LineString"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(3).GeometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(4).GeometryType().Equals("MultiLineString"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(5).GeometryType().Equals("MultiPoint"));
			System.IO.MemoryStream wkbBuffer = g.AsBinary();
			g = com.epl.geometry.ogc.OGCGeometry.FromBinary(wkbBuffer);
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("GeometryCollection"));
			gc = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)g;
			NUnit.Framework.Assert.IsTrue(gc.NumGeometries() == 7);
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(0).GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(1).GeometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(2).GeometryType().Equals("GeometryCollection"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(3).GeometryType().Equals("LineString"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(4).GeometryType().Equals("GeometryCollection"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(5).GeometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(gc.GeometryN(6).GeometryType().Equals("MultiLineString"));
			gc2 = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)gc.GeometryN(4);
			NUnit.Framework.Assert.IsTrue(gc2.NumGeometries() == 6);
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(0).GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(1).GeometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(2).GeometryType().Equals("LineString"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(3).GeometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(4).GeometryType().Equals("MultiLineString"));
			NUnit.Framework.Assert.IsTrue(gc2.GeometryN(5).GeometryType().Equals("MultiPoint"));
			string wktString = g.AsText();
			NUnit.Framework.Assert.IsTrue(wktString.Equals("GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), GEOMETRYCOLLECTION EMPTY, LINESTRING EMPTY, GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY, MULTIPOINT EMPTY), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				));
			g = com.epl.geometry.ogc.OGCGeometry.FromGeoJson("{\"type\" : \"GeometryCollection\", \"geometries\" : [{\"type\" : \"Polygon\", \"coordinates\" : []}, {\"type\" : \"Point\", \"coordinates\" : [1, 1]}, {\"type\" : \"GeometryCollection\", \"geometries\" : []}, {\"type\" : \"LineString\", \"coordinates\" : []}, {\"type\" : \"GeometryCollection\", \"geometries\" : [{\"type\": \"Polygon\", \"coordinates\" : []}, {\"type\" : \"Point\", \"coordinates\" : [1,1]}, {\"type\" : \"LineString\", \"coordinates\" : []}, {\"type\" : \"MultiPolygon\", \"coordinates\" : []}, {\"type\" : \"MultiLineString\", \"coordinates\" : []}, {\"type\" : \"MultiPoint\", \"coordinates\" : []}]}, {\"type\" : \"MultiPolygon\", \"coordinates\" : []}, {\"type\" : \"MultiLineString\", \"coordinates\" : []} ] }"
				);
			wktString = g.AsText();
			NUnit.Framework.Assert.IsTrue(wktString.Equals("GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), GEOMETRYCOLLECTION EMPTY, LINESTRING EMPTY, GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY, MULTIPOINT EMPTY), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestFirstPointOfPolygon()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))");
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("Polygon"));
			com.epl.geometry.ogc.OGCPolygon p = (com.epl.geometry.ogc.OGCPolygon)g;
			NUnit.Framework.Assert.IsTrue(p.NumInteriorRing() == 1);
			com.epl.geometry.ogc.OGCLineString ls = p.ExteriorRing();
			com.epl.geometry.ogc.OGCPoint p1 = ls.PointN(1);
			NUnit.Framework.Assert.IsTrue(ls.PointN(1).Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(10 -10)")));
			com.epl.geometry.ogc.OGCPoint p2 = ls.PointN(3);
			NUnit.Framework.Assert.IsTrue(ls.PointN(3).Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-10 10)")));
			com.epl.geometry.ogc.OGCPoint p0 = ls.PointN(0);
			NUnit.Framework.Assert.IsTrue(ls.PointN(0).Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-10 -10)")));
			string ms = g.ConvertToMulti().AsText();
			NUnit.Framework.Assert.IsTrue(ms.Equals("MULTIPOLYGON (((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5)))"));
		}

		[NUnit.Framework.Test]
		public virtual void TestFirstPointOfLineString()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("LINESTRING(-10 -10, 10 -10, 10 10, -10 10, -10 -10)");
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("LineString"));
			com.epl.geometry.ogc.OGCLineString p = (com.epl.geometry.ogc.OGCLineString)g;
			NUnit.Framework.Assert.IsTrue(p.NumPoints() == 5);
			NUnit.Framework.Assert.IsTrue(p.IsClosed());
			NUnit.Framework.Assert.IsTrue(p.PointN(1).Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(10 -10)")));
			string ms = g.ConvertToMulti().AsText();
			NUnit.Framework.Assert.IsTrue(ms.Equals("MULTILINESTRING ((-10 -10, 10 -10, 10 10, -10 10, -10 -10))"));
		}

		[NUnit.Framework.Test]
		public virtual void TestPointInPolygon()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))");
			NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(!g.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(g.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(!g.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-20 1)")));
			NUnit.Framework.Assert.IsTrue(g.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(!g.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(g.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-20 1)")));
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPolygon()
		{
			{
				com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("MULTIPOLYGON(((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5)))");
				NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("MultiPolygon"));
				// the type is
				// reduced
				NUnit.Framework.Assert.IsTrue(!g.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(0 0)")));
				NUnit.Framework.Assert.IsTrue(g.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(!g.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-20 1)")));
				NUnit.Framework.Assert.IsTrue(g.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(0 0)")));
				NUnit.Framework.Assert.IsTrue(!g.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(g.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-20 1)")));
				NUnit.Framework.Assert.IsTrue(g.ConvertToMulti() == g);
			}
			{
				com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("MULTIPOLYGON(((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5)), ((90 90, 110 90, 110 110, 90 110, 90 90), (95 95, 95 105, 105 105, 105 95, 95 95)))");
				NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("MultiPolygon"));
				// the type is
				com.epl.geometry.ogc.OGCMultiPolygon mp = (com.epl.geometry.ogc.OGCMultiPolygon)g;
				NUnit.Framework.Assert.IsTrue(mp.NumGeometries() == 2);
				com.epl.geometry.ogc.OGCGeometry p1 = mp.GeometryN(0);
				NUnit.Framework.Assert.IsTrue(p1.GeometryType().Equals("Polygon"));
				// the type is
				NUnit.Framework.Assert.IsTrue(p1.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(!p1.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(109 109)")));
				com.epl.geometry.ogc.OGCGeometry p2 = mp.GeometryN(1);
				NUnit.Framework.Assert.IsTrue(p2.GeometryType().Equals("Polygon"));
				// the type is
				NUnit.Framework.Assert.IsTrue(!p2.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(p2.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(109 109)")));
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPolygonUnion()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))");
			com.epl.geometry.ogc.OGCGeometry g2 = com.epl.geometry.ogc.OGCGeometry.FromText("POLYGON((90 90, 110 90, 110 110, 90 110, 90 90))");
			com.epl.geometry.ogc.OGCGeometry u = g.Union(g2);
			NUnit.Framework.Assert.IsTrue(u.GeometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(!u.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(u.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(!u.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-20 1)")));
			NUnit.Framework.Assert.IsTrue(u.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(!u.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(u.Disjoint(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(-20 1)")));
			NUnit.Framework.Assert.IsTrue(u.Contains(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(100 100)")));
		}

		[NUnit.Framework.Test]
		public virtual void TestIntersection()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("LINESTRING(0 0, 10 10)");
			com.epl.geometry.ogc.OGCGeometry g2 = com.epl.geometry.ogc.OGCGeometry.FromText("LINESTRING(10 0, 0 10)");
			com.epl.geometry.ogc.OGCGeometry u = g.Intersection(g2);
			NUnit.Framework.Assert.IsTrue(u.Dimension() == 0);
			string s = u.AsText();
			NUnit.Framework.Assert.IsTrue(u.Equals(com.epl.geometry.ogc.OGCGeometry.FromText("POINT(5 5)")));
		}

		[NUnit.Framework.Test]
		public virtual void TestPointSymDif()
		{
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromText("POINT(1 2)");
			com.epl.geometry.ogc.OGCGeometry g2 = com.epl.geometry.ogc.OGCGeometry.FromText("POINT(3 4)");
			com.epl.geometry.ogc.OGCGeometry gg = g1.SymDifference(g2);
			NUnit.Framework.Assert.IsTrue(gg.Equals(com.epl.geometry.ogc.OGCGeometry.FromText("MULTIPOINT(1 2, 3 4)")));
			com.epl.geometry.ogc.OGCGeometry g3 = com.epl.geometry.ogc.OGCGeometry.FromText("POINT(1 2)");
			com.epl.geometry.ogc.OGCGeometry gg1 = g1.SymDifference(g3);
			NUnit.Framework.Assert.IsTrue(gg1 == null || gg1.IsEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void TestNullSr()
		{
			string wkt = "point (0 0)";
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
			g.SetSpatialReference(null);
			NUnit.Framework.Assert.IsTrue(g.SRID() < 1);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsectPoint()
		{
			string wkt = "point (0 0)";
			string wk2 = "point (0 0)";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromText(wk2);
			g0.SetSpatialReference(null);
			g1.SetSpatialReference(null);
			try
			{
				com.epl.geometry.ogc.OGCGeometry rslt = g0.Intersection(g1);
				// ArrayIndexOutOfBoundsException
				NUnit.Framework.Assert.IsTrue(rslt != null);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestIsectDisjoint()
		{
			string wk3 = "linestring (0 0, 1 1)";
			string wk4 = "linestring (2 2, 4 4)";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromText(wk3);
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromText(wk4);
			g0.SetSpatialReference(null);
			g1.SetSpatialReference(null);
			try
			{
				com.epl.geometry.ogc.OGCGeometry rslt = g0.Intersection(g1);
				// null
				NUnit.Framework.Assert.IsTrue(rslt != null);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void Test_polygon_is_simple_for_OGC()
		{
			try
			{
				{
					string s = "{\"rings\":[[[0, 0], [0, 10], [10, 10], [10, 0], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// exterior ring is self-tangent
					string s = "{\"rings\":[[[0, 0], [0, 10], [5, 5], [10, 10], [10, 0], [5, 5], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// ring orientation (hole is cw)
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.IsSimpleRelaxed());
				}
				{
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [10, 0], [5, 5], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// ring order
					string s = "{\"rings\":[[[0, 0], [10, 0], [5, 5], [0, 0]], [[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// hole is self tangent
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [10, 10], [5, 5], [0, 10], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// two holes touch
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [10, 0], [5, 5], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// two holes touch, bad orientation
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.IsSimpleRelaxed());
				}
				{
					// hole touches exterior in two spots
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 100], [-10, 0], [0, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// hole touches exterior in one spot
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 90], [-10, 0], [0, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// exterior has inversion (planar simple)
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [10, 0], [0, 90], [-10, 0], [0, -100], [-100, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					// two holes touch in one spot, and they also touch exterior in
					// two spots, producing disconnected interior
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, -50], [0, 0], [-10, -50], [0, -100]], [[0, 0], [10, 50], [0, 100], [-10, 50], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void Test_polygon_simplify_for_OGC()
		{
			try
			{
				{
					//degenerate
					string s = "{\"rings\":[[[0, 0], [0, 10], [10, 10], [10, 0], [20, 0], [10, 0], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					string res_str = og.AsText();
					NUnit.Framework.Assert.IsTrue(og.IsSimple());
				}
				{
					string s = "{\"rings\":[[[0, 0], [0, 10], [10, 10], [10, 0], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					string res_str = og.AsText();
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 0);
					NUnit.Framework.Assert.IsTrue(og.IsSimple());
				}
				{
					// exterior ring is self-tangent
					string s = "{\"rings\":[[[0, 0], [0, 10], [5, 5], [10, 10], [10, 0], [5, 5], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					res = og.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCGeometryCollection)og).NumGeometries() == 2);
				}
				{
					// ring orientation (hole is cw)
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					res = og.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 1);
				}
				{
					// ring order
					string s = "{\"rings\":[[[0, 0], [10, 0], [5, 5], [0, 0]], [[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					res = og.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
				}
				{
					// hole is self tangent
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [10, 10], [5, 5], [0, 10], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					string res_str = og.AsText();
					res = og.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 2);
				}
				{
					// two holes touch
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [10, 0], [5, 5], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 2);
				}
				{
					// two holes touch, bad orientation
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 2);
				}
				{
					// hole touches exterior in two spots
					//OperatorSimplifyOGC produces a multipolygon with two polygons without holes.				
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 100], [-10, 0], [0, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCMultiPolygon)og).NumGeometries() == 2);
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)((com.epl.geometry.ogc.OGCMultiPolygon)og).GeometryN(0)).NumInteriorRing() == 0);
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)((com.epl.geometry.ogc.OGCMultiPolygon)og).GeometryN(1)).NumInteriorRing() == 0);
				}
				{
					// hole touches exterior in one spot
					//OperatorSimplifyOGC produces a polygons with a hole.				
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 90], [-10, 0], [0, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 1);
				}
				{
					// exterior has inversion (non simple for OGC)
					//OperatorSimplifyOGC produces a polygons with a hole.				
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [10, 0], [0, 90], [-10, 0], [0, -100], [-100, -100]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)og).NumInteriorRing() == 1);
				}
				{
					// two holes touch in one spot, and they also touch exterior in
					// two spots, producing disconnected interior
					//OperatorSimplifyOGC produces two polygons with no holes.
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, -50], [0, 0], [-10, -50], [0, -100]], [[0, 0], [10, 50], [0, 100], [-10, 50], [0, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
					com.epl.geometry.Geometry resg = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(g.GetEsriGeometry(), null, true, null);
					com.epl.geometry.ogc.OGCGeometry og = com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.GeometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCMultiPolygon)og).NumGeometries() == 2);
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)((com.epl.geometry.ogc.OGCMultiPolygon)og).GeometryN(0)).NumInteriorRing() == 0);
					NUnit.Framework.Assert.IsTrue(((com.epl.geometry.ogc.OGCPolygon)((com.epl.geometry.ogc.OGCMultiPolygon)og).GeometryN(1)).NumInteriorRing() == 0);
				}
				{
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson("{\"rings\":[[[-3,4],[6,4],[6,-3],[-3,-3],[-3,4]],[[0,2],[2,2],[0,0],[4,0],[4,2],[2,0],[2,2],[4,2],[3,3],[2,2],[1,3],[0,2]]], \"spatialReference\":{\"wkid\":4326}}");
					NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("Polygon"));
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.IsSimpleRelaxed());
					com.epl.geometry.ogc.OGCGeometry simpleG = g.MakeSimple();
					NUnit.Framework.Assert.IsTrue(simpleG.GeometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(simpleG.IsSimple());
					com.epl.geometry.ogc.OGCMultiPolygon mp = (com.epl.geometry.ogc.OGCMultiPolygon)simpleG;
					NUnit.Framework.Assert.IsTrue(mp.NumGeometries() == 2);
					com.epl.geometry.ogc.OGCPolygon g1 = (com.epl.geometry.ogc.OGCPolygon)mp.GeometryN(0);
					com.epl.geometry.ogc.OGCPolygon g2 = (com.epl.geometry.ogc.OGCPolygon)mp.GeometryN(1);
					NUnit.Framework.Assert.IsTrue((g1.NumInteriorRing() == 0 && g1.NumInteriorRing() == 2) || (g1.NumInteriorRing() == 2 && g2.NumInteriorRing() == 0));
					com.epl.geometry.ogc.OGCGeometry oldOutput = com.epl.geometry.ogc.OGCGeometry.FromJson("{\"rings\":[[[-3,-3],[-3,4],[6,4],[6,-3],[-3,-3]],[[0,0],[2,0],[4,0],[4,2],[3,3],[2,2],[1,3],[0,2],[2,2],[0,0]],[[2,0],[2,2],[4,2],[2,0]]],\"spatialReference\":{\"wkid\":4326}}");
					NUnit.Framework.Assert.IsTrue(oldOutput.IsSimpleRelaxed());
					NUnit.Framework.Assert.IsFalse(oldOutput.IsSimple());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void Test_polyline_is_simple_for_OGC()
		{
			try
			{
				{
					string s = "{\"paths\":[[[0, 10], [8, 5], [5, 2], [6, 0]]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [6,  0], [7, 5], [0, 3]]]}";
					// self
					// intersection
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [6,  0], [0, 3], [0, 10]]]}";
					// closed
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [5, 5], [6,  0], [0, 3], [5, 5], [0, 9], [0, 10]]]}";
					// closed
					// with
					// self
					// tangent
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [5, 2]], [[5, 2], [6,  0]]]}";
					// two
					// paths
					// connected
					// at
					// a
					// point
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 0], [3, 3], [5, 0], [0, 0]], [[0, 10], [3, 3], [10, 10], [0, 10]]]}";
					// two
					// closed
					// rings
					// touch
					// at
					// one
					// point
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[3, 3], [0, 0], [5, 0], [3, 3]], [[3, 3], [0, 10], [10, 10], [3, 3]]]}";
					// two closed rings touch at one point. The touch happens at the
					// endpoints of the paths.
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 0], [10, 10]], [[0, 10], [10, 0]]]}";
					// two
					// lines
					// intersect
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 0], [5, 5], [0, 10]], [[10, 10], [5, 5], [10, 0]]]}";
					// two
					// paths
					// share
					// mid
					// point.
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void Test_multipoint_is_simple_for_OGC()
		{
			try
			{
				com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
				{
					string s = "{\"points\":[[0, 0], [5, 5], [0, 10]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
				{
					string s = "{\"points\":[[0, 0], [5, 5], [0, 0], [0, 10]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.IsSimpleRelaxed());
				}
				{
					string s = "{\"points\":[[0, 0], [5, 5], [1e-10, -1e-10], [0, 10]]}";
					com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(s);
					g.SetSpatialReference(sr);
					bool res = g.IsSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.IsSimpleRelaxed());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestGeometryCollectionBuffer()
		{
			com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromText("GEOMETRYCOLLECTION(POINT(1 1), POINT(1 1), POINT(1 2), LINESTRING (0 0, 1 1, 1 0, 0 1), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)");
			com.epl.geometry.ogc.OGCGeometry simpleG = g.Buffer(0);
			string t = simpleG.GeometryType();
			string rt = simpleG.AsText();
			NUnit.Framework.Assert.IsTrue(simpleG.GeometryType().Equals("GeometryCollection"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsectTria1()
		{
			string wkt = "polygon((1 0, 3 0, 1 2, 1 0))";
			string wk2 = "polygon((0 1, 2 1, 0 3, 0 1))";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromText(wk2);
			g0.SetSpatialReference(com.epl.geometry.SpatialReference.Create(4326));
			g1.SetSpatialReference(com.epl.geometry.SpatialReference.Create(4326));
			com.epl.geometry.ogc.OGCGeometry rslt = g0.Intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(rslt.esriSR.GetID() == 4326);
			string s = rslt.AsText();
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestIsectTriaJson1()
		{
			string json1 = "{\"rings\":[[[1, 0], [3, 0], [1, 2], [1, 0]]], \"spatialReference\":{\"wkid\":4326}}";
			string json2 = "{\"rings\":[[[0, 1], [2, 1], [0, 3], [0, 1]]], \"spatialReference\":{\"wkid\":4326}}";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromJson(json1);
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromJson(json2);
			com.epl.geometry.ogc.OGCGeometry rslt = g0.Intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.GeometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(rslt.esriSR.GetID() == 4326);
			string s = com.epl.geometry.GeometryEngine.GeometryToJson(rslt.GetEsriSpatialReference().GetID(), rslt.GetEsriGeometry());
		}

		[NUnit.Framework.Test]
		public virtual void TestIsectTria2()
		{
			string wkt = "polygon((1 0, 3 0, 1 2, 1 0))";
			string wk2 = "polygon((0 3, 2 1, 3 1, 0 3))";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromText(wk2);
			g0.SetSpatialReference(null);
			g1.SetSpatialReference(null);
			com.epl.geometry.ogc.OGCGeometry rslt = g0.Intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.Dimension() == 1);
			NUnit.Framework.Assert.IsTrue(rslt.GeometryType().Equals("LineString"));
			string s = rslt.AsText();
		}

		[NUnit.Framework.Test]
		public virtual void TestIsectTria3()
		{
			string wkt = "polygon((1 0, 3 0, 1 2, 1 0))";
			string wk2 = "polygon((2 2, 2 1, 3 1, 2 2))";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
			com.epl.geometry.ogc.OGCGeometry g1 = com.epl.geometry.ogc.OGCGeometry.FromText(wk2);
			g0.SetSpatialReference(com.epl.geometry.SpatialReference.Create(4326));
			g1.SetSpatialReference(com.epl.geometry.SpatialReference.Create(4326));
			com.epl.geometry.ogc.OGCGeometry rslt = g0.Intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.Dimension() == 0);
			NUnit.Framework.Assert.IsTrue(rslt.GeometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(rslt.esriSR.GetID() == 4326);
			string s = rslt.AsText();
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointSinglePoint()
		{
			string wkt = "multipoint((1 0))";
			com.epl.geometry.ogc.OGCGeometry g0 = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
			NUnit.Framework.Assert.IsTrue(g0.Dimension() == 0);
			string gt = g0.GeometryType();
			NUnit.Framework.Assert.IsTrue(gt.Equals("MultiPoint"));
			com.epl.geometry.ogc.OGCMultiPoint mp = (com.epl.geometry.ogc.OGCMultiPoint)g0;
			NUnit.Framework.Assert.IsTrue(mp.NumGeometries() == 1);
			com.epl.geometry.ogc.OGCGeometry p = mp.GeometryN(0);
			string s = p.AsText();
			NUnit.Framework.Assert.IsTrue(s.Equals("POINT (1 0)"));
			string ms = p.ConvertToMulti().AsText();
			NUnit.Framework.Assert.IsTrue(ms.Equals("MULTIPOINT ((1 0))"));
		}

		[NUnit.Framework.Test]
		public virtual void TestWktMultiPolygon()
		{
			string restJson = "{\"rings\": [[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[-90, -90], [90, 90], [-90, 90], [90, -90], [-90, -90]],	[[-10, -10], [-10, 10], [10, 10], [10, -10], [-10, -10]]]}";
			com.epl.geometry.MapGeometry g = null;
			try
			{
				g = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, restJson);
			}
			catch (org.codehaus.jackson.JsonParseException e)
			{
				// TODO Auto-generated catch block
				e.PrintStackTrace();
			}
			catch (System.IO.IOException e)
			{
				// TODO Auto-generated catch block
				e.PrintStackTrace();
			}
			string wkt = com.epl.geometry.OperatorExportToWkt.Local().Execute(0, g.GetGeometry(), null);
			NUnit.Framework.Assert.IsTrue(wkt.Equals("MULTIPOLYGON (((-100 -100, 100 -100, 100 100, -100 100, -100 -100), (-90 -90, 90 -90, -90 90, 90 90, -90 -90)), ((-10 -10, 10 -10, 10 10, -10 10, -10 -10)))"));
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPolygonArea()
		{
			//MultiPolygon Area #36 
			string wkt = "MULTIPOLYGON (((1001200 2432900, 1001420 2432691, 1001250 2432388, 1001498 2432325, 1001100 2432100, 1001500 2431900, 1002044 2431764, 1002059 2432120, 1002182 2432003, 1002400 2432300, 1002650 2432150, 1002610 2432323, 1002772 2432434, 1002410 2432821, 1002700 2433000, 1001824 2432866, 1001600 2433150, 1001200 2432900)), ((1000393 2433983, 1000914 2434018, 1000933 2433817, 1000568 2433834, 1000580 2433584, 1000700 2433750, 1000800 2433650, 1000700 2433450, 1000600 2433550, 1000200 2433350, 1000100 2433900, 1000393 2433983)), ((1001200 2432900, 1000878 2432891, 1000900 2433300, 1001659 2433509, 1001600 2433150, 1001200 2432900)), ((1002450 2431650, 1002300 2431650, 1002300 2431900, 1002500 2432100, 1002600 2431800, 1002450 2431800, 1002450 2431650)), ((999750 2433550, 999850 2433600, 999900 2433350, 999780 2433433, 999750 2433550)), ((1002950 2432050, 1003005 2431932, 1002850 2432250, 1002928 2432210, 1002950 2432050)), ((1002600 2431750, 1002642 2431882, 1002750 2431900, 1002750 2431750, 1002600 2431750)), ((1002950 2431750, 1003050 2431650, 1002968 2431609, 1002950 2431750)))";
			{
				com.epl.geometry.ogc.OGCGeometry ogcg = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
				NUnit.Framework.Assert.IsTrue(ogcg.GeometryType().Equals("MultiPolygon"));
				com.epl.geometry.ogc.OGCMultiPolygon mp = (com.epl.geometry.ogc.OGCMultiPolygon)ogcg;
				double a = mp.Area();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(mp.Area() - 2037634.5) < a * 1e-14);
			}
			{
				com.epl.geometry.ogc.OGCGeometry ogcg = com.epl.geometry.ogc.OGCGeometry.FromText(wkt);
				NUnit.Framework.Assert.IsTrue(ogcg.GeometryType().Equals("MultiPolygon"));
				com.epl.geometry.Geometry g = ogcg.GetEsriGeometry();
				double a = g.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(a - 2037634.5) < a * 1e-14);
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestPolylineSimplifyIssueGithub52()
		{
			string json = "{\"paths\":[[[2,0],[4,3],[5,1],[3.25,1.875],[1,3]]],\"spatialReference\":{\"wkid\":4326}}";
			{
				com.epl.geometry.ogc.OGCGeometry g = com.epl.geometry.ogc.OGCGeometry.FromJson(json);
				NUnit.Framework.Assert.IsTrue(g.GeometryType().Equals("LineString"));
				com.epl.geometry.ogc.OGCGeometry simpleG = g.MakeSimple();
				//make ogc simple
				NUnit.Framework.Assert.IsTrue(simpleG.GeometryType().Equals("MultiLineString"));
				NUnit.Framework.Assert.IsTrue(simpleG.IsSimpleRelaxed());
				//geodatabase simple
				NUnit.Framework.Assert.IsTrue(simpleG.IsSimple());
				//ogc simple
				com.epl.geometry.ogc.OGCMultiLineString mls = (com.epl.geometry.ogc.OGCMultiLineString)simpleG;
				NUnit.Framework.Assert.IsTrue(mls.NumGeometries() == 4);
				com.epl.geometry.ogc.OGCGeometry baseGeom = com.epl.geometry.ogc.OGCGeometry.FromJson("{\"paths\":[[[2,0],[3.25,1.875]],[[3.25,1.875],[4,3],[5,1]],[[5,1],[3.25,1.875]],[[3.25,1.875],[1,3]]],\"spatialReference\":{\"wkid\":4326}}");
				NUnit.Framework.Assert.IsTrue(simpleG.Equals(baseGeom));
			}
		}
	}
}
