

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestJSonToGeomFromWkiOrWkt_CR177613
	{
		internal org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory
			();

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

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testPolygonWithEmptyWKT_NoWKI()
		{
			string jsonStringPg = "{ \"rings\" :[  [ [-97.06138,32.837], [-97.06133,32.836], "
				 + "[-97.06124,32.834], [-97.06127,32.832], [-97.06138,32.837] ],  " + "[ [-97.06326,32.759], [-97.06298,32.755], [-97.06153,32.749], [-97.06326,32.759] ]], "
				 + "\"spatialReference\" : {\"wkt\" : \"\"}}";
			org.codehaus.jackson.JsonParser jsonParserPg = factory.createJsonParser(jsonStringPg
				);
			jsonParserPg.nextToken();
			com.esri.core.geometry.MapGeometry mapGeom = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParserPg);
			com.esri.core.geometry.Utils.showProjectedGeometryInfo(mapGeom);
			com.esri.core.geometry.SpatialReference sr = mapGeom.getSpatialReference();
			NUnit.Framework.Assert.IsTrue(sr == null);
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testOnlyWKI()
		{
			string jsonStringSR = "{\"wkid\" : 4326}";
			org.codehaus.jackson.JsonParser jsonParserSR = factory.createJsonParser(jsonStringSR
				);
			jsonParserSR.nextToken();
			com.esri.core.geometry.MapGeometry mapGeom = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParserSR);
			com.esri.core.geometry.Utils.showProjectedGeometryInfo(mapGeom);
			com.esri.core.geometry.SpatialReference sr = mapGeom.getSpatialReference();
			NUnit.Framework.Assert.IsTrue(sr == null);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void testMP2onCR175871()
		{
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(-50, 10);
			pg.lineTo(-50, 12);
			pg.lineTo(-45, 12);
			pg.lineTo(-45, 10);
			com.esri.core.geometry.Polygon pg1 = new com.esri.core.geometry.Polygon();
			pg1.startPath(-45, 10);
			pg1.lineTo(-40, 10);
			pg1.lineTo(-40, 8);
			pg.add(pg1, false);
			try
			{
				string jSonStr = com.esri.core.geometry.GeometryEngine.geometryToJson(4326, pg);
				org.codehaus.jackson.JsonFactory jf = new org.codehaus.jackson.JsonFactory();
				org.codehaus.jackson.JsonParser jp = jf.createJsonParser(jSonStr);
				jp.nextToken();
				com.esri.core.geometry.MapGeometry mg = com.esri.core.geometry.GeometryEngine.jsonToGeometry
					(jp);
				com.esri.core.geometry.Geometry gm = mg.getGeometry();
				NUnit.Framework.Assert.AreEqual(com.esri.core.geometry.Geometry.Type.Polygon, gm.
					getType());
				com.esri.core.geometry.Polygon pgNew = (com.esri.core.geometry.Polygon)gm;
				NUnit.Framework.Assert.AreEqual(pgNew.getPathCount(), pg.getPathCount());
				NUnit.Framework.Assert.AreEqual(pgNew.getPointCount(), pg.getPointCount());
				NUnit.Framework.Assert.AreEqual(pgNew.getSegmentCount(), pg.getSegmentCount());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(0).getX(), 0.000000001, pgNew.getPoint
					(0).getX());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(1).getX(), 0.000000001, pgNew.getPoint
					(1).getX());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(2).getX(), 0.000000001, pgNew.getPoint
					(2).getX());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(3).getX(), 0.000000001, pgNew.getPoint
					(3).getX());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(0).getY(), 0.000000001, pgNew.getPoint
					(0).getY());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(1).getY(), 0.000000001, pgNew.getPoint
					(1).getY());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(2).getY(), 0.000000001, pgNew.getPoint
					(2).getY());
				NUnit.Framework.Assert.AreEqual(pg.getPoint(3).getY(), 0.000000001, pgNew.getPoint
					(3).getY());
			}
			catch (System.Exception ex)
			{
				string err = ex.Message;
				System.Console.Out.Write(err);
				throw;
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		public static int fromJsonToWkid(org.codehaus.jackson.JsonParser parser)
		{
			int wkid = 0;
			if (parser.getCurrentToken() != org.codehaus.jackson.JsonToken.START_OBJECT)
			{
				return 0;
			}
			while (parser.nextToken() != org.codehaus.jackson.JsonToken.END_OBJECT)
			{
				string fieldName = parser.getCurrentName();
				if ("wkid".Equals(fieldName))
				{
					parser.nextToken();
					wkid = parser.getIntValue();
				}
			}
			return wkid;
		}
	}
}
