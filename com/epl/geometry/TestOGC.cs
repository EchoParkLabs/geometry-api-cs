

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestOGC
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
		public virtual void testPoint()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(1 2)");
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("Point"));
			com.esri.core.geometry.ogc.OGCPoint p = (com.esri.core.geometry.ogc.OGCPoint)g;
			NUnit.Framework.Assert.IsTrue(p.X() == 1);
			NUnit.Framework.Assert.IsTrue(p.Y() == 2);
			NUnit.Framework.Assert.IsTrue(g.equals(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(1 2)")));
			NUnit.Framework.Assert.IsTrue(!g.equals(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(1 3)")));
			com.esri.core.geometry.ogc.OGCGeometry buf = g.buffer(10);
			NUnit.Framework.Assert.IsTrue(buf.geometryType().Equals("Polygon"));
			com.esri.core.geometry.ogc.OGCPolygon poly = (com.esri.core.geometry.ogc.OGCPolygon
				)buf.envelope();
			double a = poly.area();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(a - 400) < 1e-1);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygon()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				);
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("Polygon"));
			com.esri.core.geometry.ogc.OGCPolygon p = (com.esri.core.geometry.ogc.OGCPolygon)
				g;
			NUnit.Framework.Assert.IsTrue(p.numInteriorRing() == 1);
			com.esri.core.geometry.ogc.OGCLineString ls = p.exteriorRing();
			// assertTrue(ls.pointN(1).equals(OGCGeometry.fromText("POINT(10 -10)")));
			bool b = ls.equals(com.esri.core.geometry.ogc.OGCGeometry.fromText("LINESTRING(-10 -10, 10 -10, 10 10, -10 10, -10 -10)"
				));
			NUnit.Framework.Assert.IsTrue(b);
			com.esri.core.geometry.ogc.OGCLineString lsi = p.interiorRingN(0);
			b = lsi.equals(com.esri.core.geometry.ogc.OGCGeometry.fromText("LINESTRING(-5 -5, -5 5, 5 5, 5 -5, -5 -5)"
				));
			NUnit.Framework.Assert.IsTrue(b);
			NUnit.Framework.Assert.IsTrue(!lsi.equals(ls));
			com.esri.core.geometry.ogc.OGCMultiCurve boundary = ((com.esri.core.geometry.ogc.OGCMultiCurve
				)p.boundary());
			string s = boundary.asText();
			NUnit.Framework.Assert.IsTrue(s.Equals("MULTILINESTRING ((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				));
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public virtual void testGeometryCollection()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("GEOMETRYCOLLECTION(POLYGON EMPTY, POINT(1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				);
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("GeometryCollection"));
			com.esri.core.geometry.ogc.OGCConcreteGeometryCollection gc = (com.esri.core.geometry.ogc.OGCConcreteGeometryCollection
				)g;
			NUnit.Framework.Assert.IsTrue(gc.numGeometries() == 5);
			NUnit.Framework.Assert.IsTrue(gc.geometryN(0).geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(1).geometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(2).geometryType().Equals("LineString")
				);
			NUnit.Framework.Assert.IsTrue(gc.geometryN(3).geometryType().Equals("MultiPolygon"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(4).geometryType().Equals("MultiLineString"
				));
			g = com.esri.core.geometry.ogc.OGCGeometry.fromText("GEOMETRYCOLLECTION(POLYGON EMPTY, POINT(1 1), GEOMETRYCOLLECTION EMPTY, LINESTRING EMPTY, GEOMETRYCOLLECTION(POLYGON EMPTY, POINT(1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY, MULTIPOINT EMPTY), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				);
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("GeometryCollection"));
			gc = (com.esri.core.geometry.ogc.OGCConcreteGeometryCollection)g;
			NUnit.Framework.Assert.IsTrue(gc.numGeometries() == 7);
			NUnit.Framework.Assert.IsTrue(gc.geometryN(0).geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(1).geometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(2).geometryType().Equals("GeometryCollection"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(3).geometryType().Equals("LineString")
				);
			NUnit.Framework.Assert.IsTrue(gc.geometryN(4).geometryType().Equals("GeometryCollection"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(5).geometryType().Equals("MultiPolygon"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(6).geometryType().Equals("MultiLineString"
				));
			com.esri.core.geometry.ogc.OGCConcreteGeometryCollection gc2 = (com.esri.core.geometry.ogc.OGCConcreteGeometryCollection
				)gc.geometryN(4);
			NUnit.Framework.Assert.IsTrue(gc2.numGeometries() == 6);
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(0).geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(1).geometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(2).geometryType().Equals("LineString"
				));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(3).geometryType().Equals("MultiPolygon"
				));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(4).geometryType().Equals("MultiLineString"
				));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(5).geometryType().Equals("MultiPoint"
				));
			java.nio.ByteBuffer wkbBuffer = g.asBinary();
			g = com.esri.core.geometry.ogc.OGCGeometry.fromBinary(wkbBuffer);
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("GeometryCollection"));
			gc = (com.esri.core.geometry.ogc.OGCConcreteGeometryCollection)g;
			NUnit.Framework.Assert.IsTrue(gc.numGeometries() == 7);
			NUnit.Framework.Assert.IsTrue(gc.geometryN(0).geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(1).geometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(2).geometryType().Equals("GeometryCollection"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(3).geometryType().Equals("LineString")
				);
			NUnit.Framework.Assert.IsTrue(gc.geometryN(4).geometryType().Equals("GeometryCollection"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(5).geometryType().Equals("MultiPolygon"
				));
			NUnit.Framework.Assert.IsTrue(gc.geometryN(6).geometryType().Equals("MultiLineString"
				));
			gc2 = (com.esri.core.geometry.ogc.OGCConcreteGeometryCollection)gc.geometryN(4);
			NUnit.Framework.Assert.IsTrue(gc2.numGeometries() == 6);
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(0).geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(1).geometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(2).geometryType().Equals("LineString"
				));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(3).geometryType().Equals("MultiPolygon"
				));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(4).geometryType().Equals("MultiLineString"
				));
			NUnit.Framework.Assert.IsTrue(gc2.geometryN(5).geometryType().Equals("MultiPoint"
				));
			string wktString = g.asText();
			NUnit.Framework.Assert.IsTrue(wktString.Equals("GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), GEOMETRYCOLLECTION EMPTY, LINESTRING EMPTY, GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY, MULTIPOINT EMPTY), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				));
			g = com.esri.core.geometry.ogc.OGCGeometry.fromGeoJson("{\"type\" : \"GeometryCollection\", \"geometries\" : [{\"type\" : \"Polygon\", \"coordinates\" : []}, {\"type\" : \"Point\", \"coordinates\" : [1, 1]}, {\"type\" : \"GeometryCollection\", \"geometries\" : []}, {\"type\" : \"LineString\", \"coordinates\" : []}, {\"type\" : \"GeometryCollection\", \"geometries\" : [{\"type\": \"Polygon\", \"coordinates\" : []}, {\"type\" : \"Point\", \"coordinates\" : [1,1]}, {\"type\" : \"LineString\", \"coordinates\" : []}, {\"type\" : \"MultiPolygon\", \"coordinates\" : []}, {\"type\" : \"MultiLineString\", \"coordinates\" : []}, {\"type\" : \"MultiPoint\", \"coordinates\" : []}]}, {\"type\" : \"MultiPolygon\", \"coordinates\" : []}, {\"type\" : \"MultiLineString\", \"coordinates\" : []} ] }"
				);
			wktString = g.asText();
			NUnit.Framework.Assert.IsTrue(wktString.Equals("GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), GEOMETRYCOLLECTION EMPTY, LINESTRING EMPTY, GEOMETRYCOLLECTION (POLYGON EMPTY, POINT (1 1), LINESTRING EMPTY, MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY, MULTIPOINT EMPTY), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				));
		}

		[NUnit.Framework.Test]
		public virtual void testFirstPointOfPolygon()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				);
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("Polygon"));
			com.esri.core.geometry.ogc.OGCPolygon p = (com.esri.core.geometry.ogc.OGCPolygon)
				g;
			NUnit.Framework.Assert.IsTrue(p.numInteriorRing() == 1);
			com.esri.core.geometry.ogc.OGCLineString ls = p.exteriorRing();
			com.esri.core.geometry.ogc.OGCPoint p1 = ls.pointN(1);
			NUnit.Framework.Assert.IsTrue(ls.pointN(1).equals(com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(10 -10)")));
			com.esri.core.geometry.ogc.OGCPoint p2 = ls.pointN(3);
			NUnit.Framework.Assert.IsTrue(ls.pointN(3).equals(com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(-10 10)")));
			com.esri.core.geometry.ogc.OGCPoint p0 = ls.pointN(0);
			NUnit.Framework.Assert.IsTrue(ls.pointN(0).equals(com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(-10 -10)")));
			string ms = g.convertToMulti().asText();
			NUnit.Framework.Assert.IsTrue(ms.Equals("MULTIPOLYGON (((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5)))"
				));
		}

		[NUnit.Framework.Test]
		public virtual void testFirstPointOfLineString()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("LINESTRING(-10 -10, 10 -10, 10 10, -10 10, -10 -10)");
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("LineString"));
			com.esri.core.geometry.ogc.OGCLineString p = (com.esri.core.geometry.ogc.OGCLineString
				)g;
			NUnit.Framework.Assert.IsTrue(p.numPoints() == 5);
			NUnit.Framework.Assert.IsTrue(p.isClosed());
			NUnit.Framework.Assert.IsTrue(p.pointN(1).equals(com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(10 -10)")));
			string ms = g.convertToMulti().asText();
			NUnit.Framework.Assert.IsTrue(ms.Equals("MULTILINESTRING ((-10 -10, 10 -10, 10 10, -10 10, -10 -10))"
				));
		}

		[NUnit.Framework.Test]
		public virtual void testPointInPolygon()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				);
			NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(!g.contains(com.esri.core.geometry.ogc.OGCGeometry.
				fromText("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(g.contains(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(!g.contains(com.esri.core.geometry.ogc.OGCGeometry.
				fromText("POINT(-20 1)")));
			NUnit.Framework.Assert.IsTrue(g.disjoint(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(!g.disjoint(com.esri.core.geometry.ogc.OGCGeometry.
				fromText("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(g.disjoint(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(-20 1)")));
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPolygon()
		{
			{
				com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
					.fromText("MULTIPOLYGON(((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5)))"
					);
				NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("MultiPolygon"));
				// the type is
				// reduced
				NUnit.Framework.Assert.IsTrue(!g.contains(com.esri.core.geometry.ogc.OGCGeometry.
					fromText("POINT(0 0)")));
				NUnit.Framework.Assert.IsTrue(g.contains(com.esri.core.geometry.ogc.OGCGeometry.fromText
					("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(!g.contains(com.esri.core.geometry.ogc.OGCGeometry.
					fromText("POINT(-20 1)")));
				NUnit.Framework.Assert.IsTrue(g.disjoint(com.esri.core.geometry.ogc.OGCGeometry.fromText
					("POINT(0 0)")));
				NUnit.Framework.Assert.IsTrue(!g.disjoint(com.esri.core.geometry.ogc.OGCGeometry.
					fromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(g.disjoint(com.esri.core.geometry.ogc.OGCGeometry.fromText
					("POINT(-20 1)")));
				NUnit.Framework.Assert.IsTrue(g.convertToMulti() == g);
			}
			{
				com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
					.fromText("MULTIPOLYGON(((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5)), ((90 90, 110 90, 110 110, 90 110, 90 90), (95 95, 95 105, 105 105, 105 95, 95 95)))"
					);
				NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("MultiPolygon"));
				// the type is
				com.esri.core.geometry.ogc.OGCMultiPolygon mp = (com.esri.core.geometry.ogc.OGCMultiPolygon
					)g;
				NUnit.Framework.Assert.IsTrue(mp.numGeometries() == 2);
				com.esri.core.geometry.ogc.OGCGeometry p1 = mp.geometryN(0);
				NUnit.Framework.Assert.IsTrue(p1.geometryType().Equals("Polygon"));
				// the type is
				NUnit.Framework.Assert.IsTrue(p1.contains(com.esri.core.geometry.ogc.OGCGeometry.
					fromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(!p1.contains(com.esri.core.geometry.ogc.OGCGeometry
					.fromText("POINT(109 109)")));
				com.esri.core.geometry.ogc.OGCGeometry p2 = mp.geometryN(1);
				NUnit.Framework.Assert.IsTrue(p2.geometryType().Equals("Polygon"));
				// the type is
				NUnit.Framework.Assert.IsTrue(!p2.contains(com.esri.core.geometry.ogc.OGCGeometry
					.fromText("POINT(9 9)")));
				NUnit.Framework.Assert.IsTrue(p2.contains(com.esri.core.geometry.ogc.OGCGeometry.
					fromText("POINT(109 109)")));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPolygonUnion()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				);
			com.esri.core.geometry.ogc.OGCGeometry g2 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POLYGON((90 90, 110 90, 110 110, 90 110, 90 90))");
			com.esri.core.geometry.ogc.OGCGeometry u = g.union(g2);
			NUnit.Framework.Assert.IsTrue(u.geometryType().Equals("MultiPolygon"));
			NUnit.Framework.Assert.IsTrue(!u.contains(com.esri.core.geometry.ogc.OGCGeometry.
				fromText("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(u.contains(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(!u.contains(com.esri.core.geometry.ogc.OGCGeometry.
				fromText("POINT(-20 1)")));
			NUnit.Framework.Assert.IsTrue(u.disjoint(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(0 0)")));
			NUnit.Framework.Assert.IsTrue(!u.disjoint(com.esri.core.geometry.ogc.OGCGeometry.
				fromText("POINT(9 9)")));
			NUnit.Framework.Assert.IsTrue(u.disjoint(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(-20 1)")));
			NUnit.Framework.Assert.IsTrue(u.contains(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(100 100)")));
		}

		[NUnit.Framework.Test]
		public virtual void testIntersection()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("LINESTRING(0 0, 10 10)");
			com.esri.core.geometry.ogc.OGCGeometry g2 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("LINESTRING(10 0, 0 10)");
			com.esri.core.geometry.ogc.OGCGeometry u = g.intersection(g2);
			NUnit.Framework.Assert.IsTrue(u.dimension() == 0);
			string s = u.asText();
			NUnit.Framework.Assert.IsTrue(u.equals(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("POINT(5 5)")));
		}

		[NUnit.Framework.Test]
		public virtual void testPointSymDif()
		{
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(1 2)");
			com.esri.core.geometry.ogc.OGCGeometry g2 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(3 4)");
			com.esri.core.geometry.ogc.OGCGeometry gg = g1.symDifference(g2);
			NUnit.Framework.Assert.IsTrue(gg.equals(com.esri.core.geometry.ogc.OGCGeometry.fromText
				("MULTIPOINT(1 2, 3 4)")));
			com.esri.core.geometry.ogc.OGCGeometry g3 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("POINT(1 2)");
			com.esri.core.geometry.ogc.OGCGeometry gg1 = g1.symDifference(g3);
			NUnit.Framework.Assert.IsTrue(gg1 == null || gg1.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testNullSr()
		{
			string wkt = "point (0 0)";
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wkt);
			g.setSpatialReference(null);
			NUnit.Framework.Assert.IsTrue(g.SRID() < 1);
		}

		[NUnit.Framework.Test]
		public virtual void testIsectPoint()
		{
			string wkt = "point (0 0)";
			string wk2 = "point (0 0)";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wkt);
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wk2);
			g0.setSpatialReference(null);
			g1.setSpatialReference(null);
			try
			{
				com.esri.core.geometry.ogc.OGCGeometry rslt = g0.intersection(g1);
				// ArrayIndexOutOfBoundsException
				NUnit.Framework.Assert.IsTrue(rslt != null);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testIsectDisjoint()
		{
			string wk3 = "linestring (0 0, 1 1)";
			string wk4 = "linestring (2 2, 4 4)";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wk3);
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wk4);
			g0.setSpatialReference(null);
			g1.setSpatialReference(null);
			try
			{
				com.esri.core.geometry.ogc.OGCGeometry rslt = g0.intersection(g1);
				// null
				NUnit.Framework.Assert.IsTrue(rslt != null);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void test_polygon_is_simple_for_OGC()
		{
			try
			{
				{
					string s = "{\"rings\":[[[0, 0], [0, 10], [10, 10], [10, 0], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// exterior ring is self-tangent
					string s = "{\"rings\":[[[0, 0], [0, 10], [5, 5], [10, 10], [10, 0], [5, 5], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// ring orientation (hole is cw)
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.isSimpleRelaxed());
				}
				{
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [10, 0], [5, 5], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// ring order
					string s = "{\"rings\":[[[0, 0], [10, 0], [5, 5], [0, 0]], [[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// hole is self tangent
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [10, 10], [5, 5], [0, 10], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// two holes touch
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [10, 0], [5, 5], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// two holes touch, bad orientation
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.isSimpleRelaxed());
				}
				{
					// hole touches exterior in two spots
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 100], [-10, 0], [0, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// hole touches exterior in one spot
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 90], [-10, 0], [0, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// exterior has inversion (planar simple)
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [10, 0], [0, 90], [-10, 0], [0, -100], [-100, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					// two holes touch in one spot, and they also touch exterior in
					// two spots, producing disconnected interior
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, -50], [0, 0], [-10, -50], [0, -100]], [[0, 0], [10, 50], [0, 100], [-10, 50], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void test_polygon_simplify_for_OGC()
		{
			try
			{
				{
					//degenerate
					string s = "{\"rings\":[[[0, 0], [0, 10], [10, 10], [10, 0], [20, 0], [10, 0], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					string res_str = og.asText();
					NUnit.Framework.Assert.IsTrue(og.isSimple());
				}
				{
					string s = "{\"rings\":[[[0, 0], [0, 10], [10, 10], [10, 0], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					string res_str = og.asText();
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 0);
					NUnit.Framework.Assert.IsTrue(og.isSimple());
				}
				{
					// exterior ring is self-tangent
					string s = "{\"rings\":[[[0, 0], [0, 10], [5, 5], [10, 10], [10, 0], [5, 5], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					res = og.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCGeometryCollection)
						og).numGeometries() == 2);
				}
				{
					// ring orientation (hole is cw)
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					res = og.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 1);
				}
				{
					// ring order
					string s = "{\"rings\":[[[0, 0], [10, 0], [5, 5], [0, 0]], [[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					res = og.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
				}
				{
					// hole is self tangent
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [10, 10], [5, 5], [0, 10], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					string res_str = og.asText();
					res = og.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 2);
				}
				{
					// two holes touch
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [10, 0], [5, 5], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 2);
				}
				{
					// two holes touch, bad orientation
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[0, 0], [5, 5], [10, 0], [0, 0]], [[10, 10], [0, 10], [5, 5], [10, 10]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 2);
				}
				{
					// hole touches exterior in two spots
					//OperatorSimplifyOGC produces a multipolygon with two polygons without holes.				
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 100], [-10, 0], [0, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCMultiPolygon)og).numGeometries
						() == 2);
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)((com.esri.core.geometry.ogc.OGCMultiPolygon
						)og).geometryN(0)).numInteriorRing() == 0);
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)((com.esri.core.geometry.ogc.OGCMultiPolygon
						)og).geometryN(1)).numInteriorRing() == 0);
				}
				{
					// hole touches exterior in one spot
					//OperatorSimplifyOGC produces a polygons with a hole.				
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, 0], [0, 90], [-10, 0], [0, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 1);
				}
				{
					// exterior has inversion (non simple for OGC)
					//OperatorSimplifyOGC produces a polygons with a hole.				
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [10, 0], [0, 90], [-10, 0], [0, -100], [-100, -100]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("Polygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)og).numInteriorRing
						() == 1);
				}
				{
					// two holes touch in one spot, and they also touch exterior in
					// two spots, producing disconnected interior
					//OperatorSimplifyOGC produces two polygons with no holes.
					string s = "{\"rings\":[[[-100, -100], [-100, 100], [0, 100], [100, 100], [100, -100], [0, -100], [-100, -100]], [[0, -100], [10, -50], [0, 0], [-10, -50], [0, -100]], [[0, 0], [10, 50], [0, 100], [-10, 50], [0, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
					com.esri.core.geometry.Geometry resg = com.esri.core.geometry.OperatorSimplifyOGC
						.local().execute(g.getEsriGeometry(), null, true, null);
					com.esri.core.geometry.ogc.OGCGeometry og = com.esri.core.geometry.ogc.OGCGeometry
						.createFromEsriGeometry(resg, null);
					NUnit.Framework.Assert.IsTrue(og.geometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCMultiPolygon)og).numGeometries
						() == 2);
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)((com.esri.core.geometry.ogc.OGCMultiPolygon
						)og).geometryN(0)).numInteriorRing() == 0);
					NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.ogc.OGCPolygon)((com.esri.core.geometry.ogc.OGCMultiPolygon
						)og).geometryN(1)).numInteriorRing() == 0);
				}
				{
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson("{\"rings\":[[[-3,4],[6,4],[6,-3],[-3,-3],[-3,4]],[[0,2],[2,2],[0,0],[4,0],[4,2],[2,0],[2,2],[4,2],[3,3],[2,2],[1,3],[0,2]]], \"spatialReference\":{\"wkid\":4326}}"
						);
					NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("Polygon"));
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.isSimpleRelaxed());
					com.esri.core.geometry.ogc.OGCGeometry simpleG = g.makeSimple();
					NUnit.Framework.Assert.IsTrue(simpleG.geometryType().Equals("MultiPolygon"));
					NUnit.Framework.Assert.IsTrue(simpleG.isSimple());
					com.esri.core.geometry.ogc.OGCMultiPolygon mp = (com.esri.core.geometry.ogc.OGCMultiPolygon
						)simpleG;
					NUnit.Framework.Assert.IsTrue(mp.numGeometries() == 2);
					com.esri.core.geometry.ogc.OGCPolygon g1 = (com.esri.core.geometry.ogc.OGCPolygon
						)mp.geometryN(0);
					com.esri.core.geometry.ogc.OGCPolygon g2 = (com.esri.core.geometry.ogc.OGCPolygon
						)mp.geometryN(1);
					NUnit.Framework.Assert.IsTrue((g1.numInteriorRing() == 0 && g1.numInteriorRing() 
						== 2) || (g1.numInteriorRing() == 2 && g2.numInteriorRing() == 0));
					com.esri.core.geometry.ogc.OGCGeometry oldOutput = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson("{\"rings\":[[[-3,-3],[-3,4],[6,4],[6,-3],[-3,-3]],[[0,0],[2,0],[4,0],[4,2],[3,3],[2,2],[1,3],[0,2],[2,2],[0,0]],[[2,0],[2,2],[4,2],[2,0]]],\"spatialReference\":{\"wkid\":4326}}"
						);
					NUnit.Framework.Assert.IsTrue(oldOutput.isSimpleRelaxed());
					NUnit.Framework.Assert.IsFalse(oldOutput.isSimple());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void test_polyline_is_simple_for_OGC()
		{
			try
			{
				{
					string s = "{\"paths\":[[[0, 10], [8, 5], [5, 2], [6, 0]]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [6,  0], [7, 5], [0, 3]]]}";
					// self
					// intersection
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [6,  0], [0, 3], [0, 10]]]}";
					// closed
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [5, 5], [6,  0], [0, 3], [5, 5], [0, 9], [0, 10]]]}";
					// closed
					// with
					// self
					// tangent
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 10], [5, 2]], [[5, 2], [6,  0]]]}";
					// two
					// paths
					// connected
					// at
					// a
					// point
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
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
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[3, 3], [0, 0], [5, 0], [3, 3]], [[3, 3], [0, 10], [10, 10], [3, 3]]]}";
					// two closed rings touch at one point. The touch happens at the
					// endpoints of the paths.
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 0], [10, 10]], [[0, 10], [10, 0]]]}";
					// two
					// lines
					// intersect
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"paths\":[[[0, 0], [5, 5], [0, 10]], [[10, 10], [5, 5], [10, 0]]]}";
					// two
					// paths
					// share
					// mid
					// point.
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void test_multipoint_is_simple_for_OGC()
		{
			try
			{
				com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
					.create(4326);
				{
					string s = "{\"points\":[[0, 0], [5, 5], [0, 10]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
				{
					string s = "{\"points\":[[0, 0], [5, 5], [0, 0], [0, 10]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(!g.isSimpleRelaxed());
				}
				{
					string s = "{\"points\":[[0, 0], [5, 5], [1e-10, -1e-10], [0, 10]]}";
					com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
						.fromJson(s);
					g.setSpatialReference(sr);
					bool res = g.isSimple();
					NUnit.Framework.Assert.IsTrue(!res);
					NUnit.Framework.Assert.IsTrue(g.isSimpleRelaxed());
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testGeometryCollectionBuffer()
		{
			com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
				.fromText("GEOMETRYCOLLECTION(POINT(1 1), POINT(1 1), POINT(1 2), LINESTRING (0 0, 1 1, 1 0, 0 1), MULTIPOLYGON EMPTY, MULTILINESTRING EMPTY)"
				);
			com.esri.core.geometry.ogc.OGCGeometry simpleG = g.buffer(0);
			string t = simpleG.geometryType();
			string rt = simpleG.asText();
			NUnit.Framework.Assert.IsTrue(simpleG.geometryType().Equals("GeometryCollection")
				);
		}

		[NUnit.Framework.Test]
		public virtual void testIsectTria1()
		{
			string wkt = "polygon((1 0, 3 0, 1 2, 1 0))";
			string wk2 = "polygon((0 1, 2 1, 0 3, 0 1))";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wkt);
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wk2);
			g0.setSpatialReference(com.esri.core.geometry.SpatialReference.create(4326));
			g1.setSpatialReference(com.esri.core.geometry.SpatialReference.create(4326));
			com.esri.core.geometry.ogc.OGCGeometry rslt = g0.intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(rslt.esriSR.getID() == 4326);
			string s = rslt.asText();
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testIsectTriaJson1()
		{
			string json1 = "{\"rings\":[[[1, 0], [3, 0], [1, 2], [1, 0]]], \"spatialReference\":{\"wkid\":4326}}";
			string json2 = "{\"rings\":[[[0, 1], [2, 1], [0, 3], [0, 1]]], \"spatialReference\":{\"wkid\":4326}}";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromJson(json1);
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromJson(json2);
			com.esri.core.geometry.ogc.OGCGeometry rslt = g0.intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.geometryType().Equals("Polygon"));
			NUnit.Framework.Assert.IsTrue(rslt.esriSR.getID() == 4326);
			string s = com.esri.core.geometry.GeometryEngine.geometryToJson(rslt.getEsriSpatialReference
				().getID(), rslt.getEsriGeometry());
		}

		[NUnit.Framework.Test]
		public virtual void testIsectTria2()
		{
			string wkt = "polygon((1 0, 3 0, 1 2, 1 0))";
			string wk2 = "polygon((0 3, 2 1, 3 1, 0 3))";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wkt);
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wk2);
			g0.setSpatialReference(null);
			g1.setSpatialReference(null);
			com.esri.core.geometry.ogc.OGCGeometry rslt = g0.intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.dimension() == 1);
			NUnit.Framework.Assert.IsTrue(rslt.geometryType().Equals("LineString"));
			string s = rslt.asText();
		}

		[NUnit.Framework.Test]
		public virtual void testIsectTria3()
		{
			string wkt = "polygon((1 0, 3 0, 1 2, 1 0))";
			string wk2 = "polygon((2 2, 2 1, 3 1, 2 2))";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wkt);
			com.esri.core.geometry.ogc.OGCGeometry g1 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wk2);
			g0.setSpatialReference(com.esri.core.geometry.SpatialReference.create(4326));
			g1.setSpatialReference(com.esri.core.geometry.SpatialReference.create(4326));
			com.esri.core.geometry.ogc.OGCGeometry rslt = g0.intersection(g1);
			NUnit.Framework.Assert.IsTrue(rslt != null);
			NUnit.Framework.Assert.IsTrue(rslt.dimension() == 0);
			NUnit.Framework.Assert.IsTrue(rslt.geometryType().Equals("Point"));
			NUnit.Framework.Assert.IsTrue(rslt.esriSR.getID() == 4326);
			string s = rslt.asText();
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointSinglePoint()
		{
			string wkt = "multipoint((1 0))";
			com.esri.core.geometry.ogc.OGCGeometry g0 = com.esri.core.geometry.ogc.OGCGeometry
				.fromText(wkt);
			NUnit.Framework.Assert.IsTrue(g0.dimension() == 0);
			string gt = g0.geometryType();
			NUnit.Framework.Assert.IsTrue(gt.Equals("MultiPoint"));
			com.esri.core.geometry.ogc.OGCMultiPoint mp = (com.esri.core.geometry.ogc.OGCMultiPoint
				)g0;
			NUnit.Framework.Assert.IsTrue(mp.numGeometries() == 1);
			com.esri.core.geometry.ogc.OGCGeometry p = mp.geometryN(0);
			string s = p.asText();
			NUnit.Framework.Assert.IsTrue(s.Equals("POINT (1 0)"));
			string ms = p.convertToMulti().asText();
			NUnit.Framework.Assert.IsTrue(ms.Equals("MULTIPOINT ((1 0))"));
		}

		[NUnit.Framework.Test]
		public virtual void testWktMultiPolygon()
		{
			string restJson = "{\"rings\": [[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[-90, -90], [90, 90], [-90, 90], [90, -90], [-90, -90]],	[[-10, -10], [-10, 10], [10, 10], [10, -10], [-10, -10]]]}";
			com.esri.core.geometry.MapGeometry g = null;
			try
			{
				g = com.esri.core.geometry.OperatorImportFromJson.local().execute(com.esri.core.geometry.Geometry.Type
					.Unknown, restJson);
			}
			catch (org.codehaus.jackson.JsonParseException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.printStackTrace(e);
			}
			catch (System.IO.IOException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.printStackTrace(e);
			}
			string wkt = com.esri.core.geometry.OperatorExportToWkt.local().execute(0, g.getGeometry
				(), null);
			NUnit.Framework.Assert.IsTrue(wkt.Equals("MULTIPOLYGON (((-100 -100, 100 -100, 100 100, -100 100, -100 -100), (-90 -90, 90 -90, -90 90, 90 90, -90 -90)), ((-10 -10, 10 -10, 10 10, -10 10, -10 -10)))"
				));
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPolygonArea()
		{
			//MultiPolygon Area #36 
			string wkt = "MULTIPOLYGON (((1001200 2432900, 1001420 2432691, 1001250 2432388, 1001498 2432325, 1001100 2432100, 1001500 2431900, 1002044 2431764, 1002059 2432120, 1002182 2432003, 1002400 2432300, 1002650 2432150, 1002610 2432323, 1002772 2432434, 1002410 2432821, 1002700 2433000, 1001824 2432866, 1001600 2433150, 1001200 2432900)), ((1000393 2433983, 1000914 2434018, 1000933 2433817, 1000568 2433834, 1000580 2433584, 1000700 2433750, 1000800 2433650, 1000700 2433450, 1000600 2433550, 1000200 2433350, 1000100 2433900, 1000393 2433983)), ((1001200 2432900, 1000878 2432891, 1000900 2433300, 1001659 2433509, 1001600 2433150, 1001200 2432900)), ((1002450 2431650, 1002300 2431650, 1002300 2431900, 1002500 2432100, 1002600 2431800, 1002450 2431800, 1002450 2431650)), ((999750 2433550, 999850 2433600, 999900 2433350, 999780 2433433, 999750 2433550)), ((1002950 2432050, 1003005 2431932, 1002850 2432250, 1002928 2432210, 1002950 2432050)), ((1002600 2431750, 1002642 2431882, 1002750 2431900, 1002750 2431750, 1002600 2431750)), ((1002950 2431750, 1003050 2431650, 1002968 2431609, 1002950 2431750)))";
			{
				com.esri.core.geometry.ogc.OGCGeometry ogcg = com.esri.core.geometry.ogc.OGCGeometry
					.fromText(wkt);
				NUnit.Framework.Assert.IsTrue(ogcg.geometryType().Equals("MultiPolygon"));
				com.esri.core.geometry.ogc.OGCMultiPolygon mp = (com.esri.core.geometry.ogc.OGCMultiPolygon
					)ogcg;
				double a = mp.area();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(mp.area() - 2037634.5) < a * 1e-14);
			}
			{
				com.esri.core.geometry.ogc.OGCGeometry ogcg = com.esri.core.geometry.ogc.OGCGeometry
					.fromText(wkt);
				NUnit.Framework.Assert.IsTrue(ogcg.geometryType().Equals("MultiPolygon"));
				com.esri.core.geometry.Geometry g = ogcg.getEsriGeometry();
				double a = g.calculateArea2D();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(a - 2037634.5) < a * 1e-14);
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testPolylineSimplifyIssueGithub52()
		{
			string json = "{\"paths\":[[[2,0],[4,3],[5,1],[3.25,1.875],[1,3]]],\"spatialReference\":{\"wkid\":4326}}";
			{
				com.esri.core.geometry.ogc.OGCGeometry g = com.esri.core.geometry.ogc.OGCGeometry
					.fromJson(json);
				NUnit.Framework.Assert.IsTrue(g.geometryType().Equals("LineString"));
				com.esri.core.geometry.ogc.OGCGeometry simpleG = g.makeSimple();
				//make ogc simple
				NUnit.Framework.Assert.IsTrue(simpleG.geometryType().Equals("MultiLineString"));
				NUnit.Framework.Assert.IsTrue(simpleG.isSimpleRelaxed());
				//geodatabase simple
				NUnit.Framework.Assert.IsTrue(simpleG.isSimple());
				//ogc simple
				com.esri.core.geometry.ogc.OGCMultiLineString mls = (com.esri.core.geometry.ogc.OGCMultiLineString
					)simpleG;
				NUnit.Framework.Assert.IsTrue(mls.numGeometries() == 4);
				com.esri.core.geometry.ogc.OGCGeometry baseGeom = com.esri.core.geometry.ogc.OGCGeometry
					.fromJson("{\"paths\":[[[2,0],[3.25,1.875]],[[3.25,1.875],[4,3],[5,1]],[[5,1],[3.25,1.875]],[[3.25,1.875],[1,3]]],\"spatialReference\":{\"wkid\":4326}}"
					);
				NUnit.Framework.Assert.IsTrue(simpleG.equals(baseGeom));
			}
		}
	}
}
