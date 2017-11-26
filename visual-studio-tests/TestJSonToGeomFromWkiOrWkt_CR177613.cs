/*
Copyright 2017 Echo Park Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

For additional information, contact:

email: info@echoparklabs.io
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestJSonToGeomFromWkiOrWkt_CR177613 : NUnit.Framework.TestFixtureAttribute
	{
		internal com.fasterxml.jackson.core.JsonFactory factory = new com.fasterxml.jackson.core.JsonFactory();

		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
		}

		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		/// <exception cref="com.fasterxml.jackson.core.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestPolygonWithEmptyWKT_NoWKI()
		{
			string jsonStringPg = "{ \"rings\" :[  [ [-97.06138,32.837], [-97.06133,32.836], " + "[-97.06124,32.834], [-97.06127,32.832], [-97.06138,32.837] ],  " + "[ [-97.06326,32.759], [-97.06298,32.755], [-97.06153,32.749], [-97.06326,32.759] ]], " + "\"spatialReference\" : {\"wkt\" : \"\"}}";
			com.fasterxml.jackson.core.JsonParser jsonParserPg = factory.CreateParser(jsonStringPg);
			jsonParserPg.NextToken();
			com.epl.geometry.MapGeometry mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPg);
			com.epl.geometry.Utils.ShowProjectedGeometryInfo(mapGeom);
			com.epl.geometry.SpatialReference sr = mapGeom.GetSpatialReference();
			NUnit.Framework.Assert.IsTrue(sr == null);
		}

		/// <exception cref="com.fasterxml.jackson.core.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestOnlyWKI()
		{
			string jsonStringSR = "{\"wkid\" : 4326}";
			com.fasterxml.jackson.core.JsonParser jsonParserSR = factory.CreateJsonParser(jsonStringSR);
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
				com.fasterxml.jackson.core.JsonFactory jf = new com.fasterxml.jackson.core.JsonFactory();
				com.fasterxml.jackson.core.JsonParser jp = jf.CreateJsonParser(jSonStr);
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

		/// <exception cref="com.fasterxml.jackson.core.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		public static int FromJsonToWkid(com.fasterxml.jackson.core.JsonParser parser)
		{
			int wkid = 0;
			if (parser.GetCurrentToken() != com.fasterxml.jackson.core.JsonToken.START_OBJECT)
			{
				return 0;
			}
			while (parser.NextToken() != com.fasterxml.jackson.core.JsonToken.END_OBJECT)
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
