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


namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestGeomToGeoJson
	{
		internal com.esri.core.geometry.OperatorFactoryLocal factory = com.esri.core.geometry.OperatorFactoryLocal
			.getInstance();

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
			com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(10.0, 20.0);
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[10.0,20.0]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testEmptyPoint()
		{
			com.esri.core.geometry.Point p = new com.esri.core.geometry.Point();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":null}", result
				);
		}

		[NUnit.Framework.Test]
		public virtual void testPointGeometryEngine()
		{
			com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(10.0, 20.0);
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[10.0,20.0]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testOGCPoint()
		{
			com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(10.0, 20.0);
			com.esri.core.geometry.ogc.OGCGeometry ogcPoint = new com.esri.core.geometry.ogc.OGCPoint
				(p, null);
			string result = ogcPoint.asGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[10.0,20.0]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(10.0, 20.0);
			mp.add(20.0, 30.0);
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(mp);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[10.0,20.0],[20.0,30.0]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testEmptyMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(mp);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":null}", 
				result);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointGeometryEngine()
		{
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(10.0, 20.0);
			mp.add(20.0, 30.0);
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(mp);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[10.0,20.0],[20.0,30.0]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testOGCMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(10.0, 20.0);
			mp.add(20.0, 30.0);
			com.esri.core.geometry.ogc.OGCMultiPoint ogcMultiPoint = new com.esri.core.geometry.ogc.OGCMultiPoint
				(mp, null);
			string result = ogcMultiPoint.asGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[10.0,20.0],[20.0,30.0]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolyline()
		{
			com.esri.core.geometry.Polyline p = new com.esri.core.geometry.Polyline();
			p.startPath(100.0, 0.0);
			p.lineTo(101.0, 0.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(100.0, 1.0);
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testEmptyPolyline()
		{
			com.esri.core.geometry.Polyline p = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":null}", 
				result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineGeometryEngine()
		{
			com.esri.core.geometry.Polyline p = new com.esri.core.geometry.Polyline();
			p.startPath(100.0, 0.0);
			p.lineTo(101.0, 0.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(100.0, 1.0);
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testOGCLineString()
		{
			com.esri.core.geometry.Polyline p = new com.esri.core.geometry.Polyline();
			p.startPath(100.0, 0.0);
			p.lineTo(101.0, 0.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(100.0, 1.0);
			com.esri.core.geometry.ogc.OGCLineString ogcLineString = new com.esri.core.geometry.ogc.OGCLineString
				(p, 0, null);
			string result = ogcLineString.asGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygon()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			p.startPath(100.0, 0.0);
			p.lineTo(101.0, 0.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(100.0, 1.0);
			p.closePathWithLine();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonWithHole()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			//exterior ring - has to be clockwise for Esri
			p.startPath(100.0, 0.0);
			p.lineTo(100.0, 1.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(101.0, 0.0);
			p.closePathWithLine();
			//hole - counterclockwise for Esri
			p.startPath(100.2, 0.2);
			p.lineTo(100.8, 0.2);
			p.lineTo(100.8, 0.8);
			p.lineTo(100.2, 0.8);
			p.closePathWithLine();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[100.0,1.0],[101.0,1.0],[101.0,0.0],[100.0,0.0]],[[100.2,0.2],[100.8,0.2],[100.8,0.8],[100.2,0.8],[100.2,0.2]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonWithHoleReversed()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			//exterior ring - has to be clockwise for Esri
			p.startPath(100.0, 0.0);
			p.lineTo(100.0, 1.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(101.0, 0.0);
			p.closePathWithLine();
			//hole - counterclockwise for Esri
			p.startPath(100.2, 0.2);
			p.lineTo(100.8, 0.2);
			p.lineTo(100.8, 0.8);
			p.lineTo(100.2, 0.8);
			p.closePathWithLine();
			p.reverseAllPaths();
			//make it reversed. Exterior ring - ccw, hole - cw.
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]],[[100.2,0.2],[100.2,0.8],[100.8,0.8],[100.8,0.2],[100.2,0.2]]]}"
				, result);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testMultiPolygon()
		{
			org.codehaus.jackson.JsonFactory jsonFactory = new org.codehaus.jackson.JsonFactory
				();
			string geoJsonPolygon = "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[-100.0,-100.0],[-100.0,100.0],[100.0,100.0],[100.0,-100.0],[-100.0,-100.0]],[[-90.0,-90.0],[90.0,90.0],[-90.0,90.0],[90.0,-90.0],[-90.0,-90.0]]],[[[-10.0,-10.0],[-10.0,10.0],[10.0,10.0],[10.0,-10.0],[-10.0,-10.0]]]]}";
			string esriJsonPolygon = "{\"rings\": [[[-100, -100], [-100, 100], [100, 100], [100, -100], [-100, -100]], [[-90, -90], [90, 90], [-90, 90], [90, -90], [-90, -90]], [[-10, -10], [-10, 10], [10, 10], [10, -10], [-10, -10]]]}";
			org.codehaus.jackson.JsonParser parser = jsonFactory.createJsonParser(esriJsonPolygon
				);
			com.esri.core.geometry.MapGeometry parsedPoly = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(parser);
			//MapGeometry parsedPoly = GeometryEngine.geometryFromGeoJson(jsonPolygon, 0, Geometry.Type.Polygon);
			com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)parsedPoly.
				getGeometry();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			//String result = exporter.execute(parsedPoly.getGeometry());
			string result = exporter.execute(poly);
			NUnit.Framework.Assert.AreEqual(geoJsonPolygon, result);
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public virtual void testEmptyPolygon()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":null}", result
				);
			com.esri.core.geometry.MapGeometry imported = com.esri.core.geometry.OperatorImportFromGeoJson
				.local().execute(0, com.esri.core.geometry.Geometry.Type.Unknown, result, null);
			NUnit.Framework.Assert.IsTrue(imported.getGeometry().isEmpty());
			NUnit.Framework.Assert.IsTrue(imported.getGeometry().getType() == com.esri.core.geometry.Geometry.Type
				.Polygon);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonGeometryEngine()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			p.startPath(100.0, 0.0);
			p.lineTo(101.0, 0.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(100.0, 1.0);
			p.closePathWithLine();
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testOGCPolygon()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			p.startPath(100.0, 0.0);
			p.lineTo(101.0, 0.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(100.0, 1.0);
			p.closePathWithLine();
			com.esri.core.geometry.ogc.OGCPolygon ogcPolygon = new com.esri.core.geometry.ogc.OGCPolygon
				(p, null);
			string result = ogcPolygon.asGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[101.0,0.0],[101.0,1.0],[100.0,1.0],[100.0,0.0]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonWithHoleGeometryEngine()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			p.startPath(100.0, 0.0);
			//clockwise exterior
			p.lineTo(100.0, 1.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(101.0, 0.0);
			p.closePathWithLine();
			p.startPath(100.2, 0.2);
			//counterclockwise hole
			p.lineTo(100.8, 0.2);
			p.lineTo(100.8, 0.8);
			p.lineTo(100.2, 0.8);
			p.closePathWithLine();
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[100.0,1.0],[101.0,1.0],[101.0,0.0],[100.0,0.0]],[[100.2,0.2],[100.8,0.2],[100.8,0.8],[100.2,0.8],[100.2,0.2]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineWithTwoPaths()
		{
			com.esri.core.geometry.Polyline p = new com.esri.core.geometry.Polyline();
			p.startPath(100.0, 0.0);
			p.lineTo(100.0, 1.0);
			p.startPath(100.2, 0.2);
			p.lineTo(100.8, 0.2);
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(p);
			NUnit.Framework.Assert.AreEqual("{\"type\":\"MultiLineString\",\"coordinates\":[[[100.0,0.0],[100.0,1.0]],[[100.2,0.2],[100.8,0.2]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testOGCPolygonWithHole()
		{
			com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
			p.startPath(100.0, 0.0);
			p.lineTo(100.0, 1.0);
			p.lineTo(101.0, 1.0);
			p.lineTo(101.0, 0.0);
			p.closePathWithLine();
			p.startPath(100.2, 0.2);
			p.lineTo(100.8, 0.2);
			p.lineTo(100.8, 0.8);
			p.lineTo(100.2, 0.8);
			p.closePathWithLine();
			com.esri.core.geometry.ogc.OGCPolygon ogcPolygon = new com.esri.core.geometry.ogc.OGCPolygon
				(p, null);
			string result = ogcPolygon.asGeoJson();
			NUnit.Framework.Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[100.0,0.0],[100.0,1.0],[101.0,1.0],[101.0,0.0],[100.0,0.0]],[[100.2,0.2],[100.8,0.2],[100.8,0.8],[100.2,0.8],[100.2,0.2]]]}"
				, result);
		}

		[NUnit.Framework.Test]
		public virtual void testEnvelope()
		{
			com.esri.core.geometry.Envelope e = new com.esri.core.geometry.Envelope();
			e.setCoords(-180.0, -90.0, 180.0, 90.0);
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(e);
			NUnit.Framework.Assert.AreEqual("{\"bbox\":[-180.0,-90.0,180.0,90.0]}", result);
		}

		[NUnit.Framework.Test]
		public virtual void testEmptyEnvelope()
		{
			com.esri.core.geometry.Envelope e = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.OperatorExportToGeoJson exporter = (com.esri.core.geometry.OperatorExportToGeoJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ExportToGeoJson);
			string result = exporter.execute(e);
			NUnit.Framework.Assert.AreEqual("{\"bbox\":null}", result);
		}

		[NUnit.Framework.Test]
		public virtual void testEnvelopeGeometryEngine()
		{
			com.esri.core.geometry.Envelope e = new com.esri.core.geometry.Envelope();
			e.setCoords(-180.0, -90.0, 180.0, 90.0);
			string result = com.esri.core.geometry.GeometryEngine.geometryToGeoJson(e);
			NUnit.Framework.Assert.AreEqual("{\"bbox\":[-180.0,-90.0,180.0,90.0]}", result);
		}
	}
}
