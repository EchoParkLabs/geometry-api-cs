using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestJSonToGeomFromWkiOrWkt_CR177613 : NUnit.Framework.TestFixtureAttribute
	{
		internal org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory();

		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
		}

		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestPolygonWithEmptyWKT_NoWKI()
		{
			string jsonStringPg = "{ \"rings\" :[  [ [-97.06138,32.837], [-97.06133,32.836], " + "[-97.06124,32.834], [-97.06127,32.832], [-97.06138,32.837] ],  " + "[ [-97.06326,32.759], [-97.06298,32.755], [-97.06153,32.749], [-97.06326,32.759] ]], " + "\"spatialReference\" : {\"wkt\" : \"\"}}";
			org.codehaus.jackson.JsonParser jsonParserPg = factory.CreateJsonParser(jsonStringPg);
			jsonParserPg.NextToken();
			com.epl.geometry.MapGeometry mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPg);
			com.epl.geometry.Utils.ShowProjectedGeometryInfo(mapGeom);
			com.epl.geometry.SpatialReference sr = mapGeom.GetSpatialReference();
			NUnit.Framework.Assert.IsTrue(sr == null);
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestOnlyWKI()
		{
			string jsonStringSR = "{\"wkid\" : 4326}";
			org.codehaus.jackson.JsonParser jsonParserSR = factory.CreateJsonParser(jsonStringSR);
			jsonParserSR.NextToken();
			com.epl.geometry.MapGeometry mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserSR);
			com.epl.geometry.Utils.ShowProjectedGeometryInfo(mapGeom);
			com.epl.geometry.SpatialReference sr = mapGeom.GetSpatialReference();
			NUnit.Framework.Assert.IsTrue(sr == null);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestMP2onCR175871()
		{
			com.epl.geometry.Polygon pg = new com.epl.geometry.Polygon();
			pg.StartPath(-50, 10);
			pg.LineTo(-50, 12);
			pg.LineTo(-45, 12);
			pg.LineTo(-45, 10);
			com.epl.geometry.Polygon pg1 = new com.epl.geometry.Polygon();
			pg1.StartPath(-45, 10);
			pg1.LineTo(-40, 10);
			pg1.LineTo(-40, 8);
			pg.Add(pg1, false);
			try
			{
				string jSonStr = com.epl.geometry.GeometryEngine.GeometryToJson(4326, pg);
				org.codehaus.jackson.JsonFactory jf = new org.codehaus.jackson.JsonFactory();
				org.codehaus.jackson.JsonParser jp = jf.CreateJsonParser(jSonStr);
				jp.NextToken();
				com.epl.geometry.MapGeometry mg = com.epl.geometry.GeometryEngine.JsonToGeometry(jp);
				com.epl.geometry.Geometry gm = mg.GetGeometry();
				NUnit.Framework.Assert.AreEqual(com.epl.geometry.Geometry.Type.Polygon, gm.GetType());
				com.epl.geometry.Polygon pgNew = (com.epl.geometry.Polygon)gm;
				NUnit.Framework.Assert.AreEqual(pgNew.GetPathCount(), pg.GetPathCount());
				NUnit.Framework.Assert.AreEqual(pgNew.GetPointCount(), pg.GetPointCount());
				NUnit.Framework.Assert.AreEqual(pgNew.GetSegmentCount(), pg.GetSegmentCount());
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(0).GetX(), pg.GetPoint(0).GetX(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(1).GetX(), pg.GetPoint(1).GetX(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(2).GetX(), pg.GetPoint(2).GetX(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(3).GetX(), pg.GetPoint(3).GetX(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(0).GetY(), pg.GetPoint(0).GetY(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(1).GetY(), pg.GetPoint(1).GetY(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(2).GetY(), pg.GetPoint(2).GetY(), 0.000000001);
				NUnit.Framework.Assert.AreEqual(pgNew.GetPoint(3).GetY(), pg.GetPoint(3).GetY(), 0.000000001);
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
		public static int FromJsonToWkid(org.codehaus.jackson.JsonParser parser)
		{
			int wkid = 0;
			if (parser.GetCurrentToken() != org.codehaus.jackson.JsonToken.START_OBJECT)
			{
				return 0;
			}
			while (parser.NextToken() != org.codehaus.jackson.JsonToken.END_OBJECT)
			{
				string fieldName = parser.GetCurrentName();
				if ("wkid".Equals(fieldName))
				{
					parser.NextToken();
					wkid = parser.GetIntValue();
				}
			}
			return wkid;
		}
	}
}
