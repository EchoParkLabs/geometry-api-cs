/*
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
Environmental Systems Research Institute, Inc.
Attn: Contracts Dept
380 New York Street
Redlands, California, USA 92373

email: contracts@esri.com
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestGeomToGeoJson : NUnit.Framework.TestFixtureAttribute
	{
		internal com.epl.geometry.OperatorFactoryLocal factory = com.epl.geometry.OperatorFactoryLocal.GetInstance();

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
			com.epl.geometry.Point p = new com.epl.geometry.Point(10.0, 20.0);
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[10.0,20.0]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyPoint()
		{
			com.epl.geometry.Point p = new com.epl.geometry.Point();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":null}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointGeometryEngine()
		{
			com.epl.geometry.Point p = new com.epl.geometry.Point(10.0, 20.0);
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[10.0,20.0]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestOGCPoint()
		{
			com.epl.geometry.Point p = new com.epl.geometry.Point(10.0, 20.0);
			com.epl.geometry.ogc.OGCGeometry ogcPoint = new com.epl.geometry.ogc.OGCPoint(p, null);
			string result = ogcPoint.AsGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[10.0,20.0]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPoint()
		{
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(10.0, 20.0);
			mp.Add(20.0, 30.0);
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(mp);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[10.0,20.0],[20.0,30.0]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyMultiPoint()
		{
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(mp);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":null}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointGeometryEngine()
		{
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(10.0, 20.0);
			mp.Add(20.0, 30.0);
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(mp);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[10.0,20.0],[20.0,30.0]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestOGCMultiPoint()
		{
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(10.0, 20.0);
			mp.Add(20.0, 30.0);
			com.epl.geometry.ogc.OGCMultiPoint ogcMultiPoint = new com.epl.geometry.ogc.OGCMultiPoint(mp, null);
			string result = ogcMultiPoint.AsGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[10.0,20.0],[20.0,30.0]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolyline()
		{
			com.epl.geometry.Polyline p = new com.epl.geometry.Polyline();
			p.StartPath(100.0, 0.0);
			p.LineTo(101.0, 0.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(100.0, 1.0);
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyPolyline()
		{
			com.epl.geometry.Polyline p = new com.epl.geometry.Polyline();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":null}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineGeometryEngine()
		{
			com.epl.geometry.Polyline p = new com.epl.geometry.Polyline();
			p.StartPath(100.0, 0.0);
			p.LineTo(101.0, 0.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(100.0, 1.0);
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestOGCLineString()
		{
			com.epl.geometry.Polyline p = new com.epl.geometry.Polyline();
			p.StartPath(100.0, 0.0);
			p.LineTo(101.0, 0.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(100.0, 1.0);
			com.epl.geometry.ogc.OGCLineString ogcLineString = new com.epl.geometry.ogc.OGCLineString(p, 0, null);
			string result = ogcLineString.AsGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygon()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			p.StartPath(100.0, 0.0);
			p.LineTo(101.0, 0.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(100.0, 1.0);
			p.ClosePathWithLine();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonWithHole()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			//exterior ring - has to be clockwise for Esri
			p.StartPath(100.0, 0.0);
			p.LineTo(100.0, 1.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(101.0, 0.0);
			p.ClosePathWithLine();
			//hole - counterclockwise for Esri
			p.StartPath(100.2, 0.2);
			p.LineTo(100.8, 0.2);
			p.LineTo(100.8, 0.8);
			p.LineTo(100.2, 0.8);
			p.ClosePathWithLine();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[100.0,1.0],[101.0,1.0],[101.0,0.0],[100.0,0.0]],[[100.2,0.2],[100.8,0.2],[100.8,0.8],[100.2,0.8],[100.2,0.2]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonWithHoleReversed()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			//exterior ring - has to be clockwise for Esri
			p.StartPath(100.0, 0.0);
			p.LineTo(100.0, 1.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(101.0, 0.0);
			p.ClosePathWithLine();
			//hole - counterclockwise for Esri
			p.StartPath(100.2, 0.2);
			p.LineTo(100.8, 0.2);
			p.LineTo(100.8, 0.8);
			p.LineTo(100.2, 0.8);
			p.ClosePathWithLine();
			p.ReverseAllPaths();
			//make it reversed. Exterior ring - ccw, hole - cw.
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]],[[100.2,0.2],[100.2,0.8],[100.8,0.8],[100.8,0.2],[100.2,0.2]]]}", result);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestMultiPolygon()
		{
			org.codehaus.jackson.JsonFactory jsonFactory = new org.codehaus.jackson.JsonFactory();
			string geoJsonPolygon = "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[-100.0,-100.0],[-100.0,100.0],[100.0,100.0],[100.0,-100.0],[-100.0,-100.0]],[[-90.0,-90.0],[90.0,90.0],[-90.0,90.0],[90.0,-90.0],[-90.0,-90.0]]],[[[-10.0,-10.0],[-10.0,10.0],[10.0,10.0],[10.0,-10.0],[-10.0,-10.0]]]]}";
			string esriJsonPolygon = "{\"rings\": [[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[-90, -90], [90, 90], [-90, 90], [90, -90], [-90, -90]], [[-10, -10], [-10, 10], [10, 10], [10, -10], [-10, -10]]]}";
			org.codehaus.jackson.JsonParser parser = jsonFactory.CreateJsonParser(esriJsonPolygon);
			com.epl.geometry.MapGeometry parsedPoly = com.epl.geometry.GeometryEngine.JsonToGeometry(parser);
			//MapGeometry parsedPoly = GeometryEngine.geometryFromGeoJson(jsonPolygon, 0, Geometry.Type.Polygon);
			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)parsedPoly.GetGeometry();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			//String result = exporter.execute(parsedPoly.getGeometry());
			string result = exporter.Execute(poly);
			NUnit.Framework.Assert.AreEqual(geoJsonPolygon, result);
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public virtual void TestEmptyPolygon()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":null}", result);
			com.epl.geometry.MapGeometry imported = com.epl.geometry.OperatorImportFromGeoJson.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, result, null);
			NUnit.Framework.Assert.IsTrue(imported.GetGeometry().IsEmpty());
			NUnit.Framework.Assert.IsTrue(imported.GetGeometry().GetType() == com.epl.geometry.Geometry.Type.Polygon);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonGeometryEngine()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			p.StartPath(100.0, 0.0);
			p.LineTo(101.0, 0.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(100.0, 1.0);
			p.ClosePathWithLine();
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestOGCPolygon()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			p.StartPath(100.0, 0.0);
			p.LineTo(101.0, 0.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(100.0, 1.0);
			p.ClosePathWithLine();
			com.epl.geometry.ogc.OGCPolygon ogcPolygon = new com.epl.geometry.ogc.OGCPolygon(p, null);
			string result = ogcPolygon.AsGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonWithHoleGeometryEngine()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			p.StartPath(100.0, 0.0);
			//clockwise exterior
			p.LineTo(100.0, 1.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(101.0, 0.0);
			p.ClosePathWithLine();
			p.StartPath(100.2, 0.2);
			//counterclockwise hole
			p.LineTo(100.8, 0.2);
			p.LineTo(100.8, 0.8);
			p.LineTo(100.2, 0.8);
			p.ClosePathWithLine();
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[100.0,1.0],[101.0,1.0],[101.0,0.0],[100.0,0.0]],[[100.2,0.2],[100.8,0.2],[100.8,0.8],[100.2,0.8],[100.2,0.2]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineWithTwoPaths()
		{
			com.epl.geometry.Polyline p = new com.epl.geometry.Polyline();
			p.StartPath(100.0, 0.0);
			p.LineTo(100.0, 1.0);
			p.StartPath(100.2, 0.2);
			p.LineTo(100.8, 0.2);
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiLineString\",\"coordinates\":[[[100.0,0.0],[100.0,1.0]],[[100.2,0.2],[100.8,0.2]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestOGCPolygonWithHole()
		{
			com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
			p.StartPath(100.0, 0.0);
			p.LineTo(100.0, 1.0);
			p.LineTo(101.0, 1.0);
			p.LineTo(101.0, 0.0);
			p.ClosePathWithLine();
			p.StartPath(100.2, 0.2);
			p.LineTo(100.8, 0.2);
			p.LineTo(100.8, 0.8);
			p.LineTo(100.2, 0.8);
			p.ClosePathWithLine();
			com.epl.geometry.ogc.OGCPolygon ogcPolygon = new com.epl.geometry.ogc.OGCPolygon(p, null);
			string result = ogcPolygon.AsGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[100.0,1.0],[101.0,1.0],[101.0,0.0],[100.0,0.0]],[[100.2,0.2],[100.8,0.2],[100.8,0.8],[100.2,0.8],[100.2,0.2]]]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestEnvelope()
		{
			com.epl.geometry.Envelope e = new com.epl.geometry.Envelope();
			e.SetCoords(-180.0, -90.0, 180.0, 90.0);
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(e);
			NUnit.Framework.Assert.AreEqual("{\"bbox\":[-180.0,-90.0,180.0,90.0]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyEnvelope()
		{
			com.epl.geometry.Envelope e = new com.epl.geometry.Envelope();
			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.Execute(e);
			NUnit.Framework.Assert.AreEqual("{\"bbox\":null}", result);
		}

		[NUnit.Framework.Test]
		public virtual void TestEnvelopeGeometryEngine()
		{
			com.epl.geometry.Envelope e = new com.epl.geometry.Envelope();
			e.SetCoords(-180.0, -90.0, 180.0, 90.0);
			string result = com.epl.geometry.GeometryEngine.GeometryToGeoJson(e);
			NUnit.Framework.Assert.AreEqual("{\"bbox\":[-180.0,-90.0,180.0,90.0]}", result);
		}
	}
}
