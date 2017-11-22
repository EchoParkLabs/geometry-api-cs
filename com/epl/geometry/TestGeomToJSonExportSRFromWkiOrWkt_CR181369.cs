

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestGeomToJSonExportSRFromWkiOrWkt_CR181369
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

		internal org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory
			();

		internal com.esri.core.geometry.SpatialReference spatialReferenceWebMerc1 = com.esri.core.geometry.SpatialReference
			.create(102100);

		internal com.esri.core.geometry.SpatialReference spatialReferenceWebMerc2 = com.esri.core.geometry.SpatialReference
			.create(spatialReferenceWebMerc1.getLatestID());

		internal com.esri.core.geometry.SpatialReference spatialReferenceWGS84 = com.esri.core.geometry.SpatialReference
			.create(4326);

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testLocalExport()
		{
			string s = com.esri.core.geometry.OperatorExportToJson.local().execute(null, new 
				com.esri.core.geometry.Point(1000000.2, 2000000.3));
			//assertTrue(s.contains("."));
			//assertFalse(s.contains(","));
			com.esri.core.geometry.Polyline line = new com.esri.core.geometry.Polyline();
			line.startPath(1.1, 2.2);
			line.lineTo(2.3, 4.5);
			string s1 = com.esri.core.geometry.OperatorExportToJson.local().execute(null, line
				);
			NUnit.Framework.Assert.IsTrue(s.contains("."));
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool testPoint()
		{
			bool bAnswer = true;
			com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point(10.0, 20.0
				);
			com.esri.core.geometry.Point pointEmpty = new com.esri.core.geometry.Point();
			{
				org.codehaus.jackson.JsonParser pointWebMerc1Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
					.geometryToJson(spatialReferenceWebMerc1, point1));
				com.esri.core.geometry.MapGeometry pointWebMerc1MP = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(pointWebMerc1Parser);
				NUnit.Framework.Assert.IsTrue(point1.getX() == ((com.esri.core.geometry.Point)pointWebMerc1MP
					.getGeometry()).getX());
				NUnit.Framework.Assert.IsTrue(point1.getY() == ((com.esri.core.geometry.Point)pointWebMerc1MP
					.getGeometry()).getY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWebMerc1.getID() == pointWebMerc1MP
					.getSpatialReference().getID() || pointWebMerc1MP.getSpatialReference().getID() 
					== 3857);
				if (!checkResultSpatialRef(pointWebMerc1MP, 102100, 3857))
				{
					bAnswer = false;
				}
				pointWebMerc1Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
					.geometryToJson(null, point1));
				pointWebMerc1MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(pointWebMerc1Parser
					);
				NUnit.Framework.Assert.IsTrue(null == pointWebMerc1MP.getSpatialReference());
				if (pointWebMerc1MP.getSpatialReference() != null)
				{
					if (!checkResultSpatialRef(pointWebMerc1MP, 102100, 3857))
					{
						bAnswer = false;
					}
				}
				string pointEmptyString = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, pointEmpty);
				pointWebMerc1Parser = factory.createJsonParser(pointEmptyString);
			}
			org.codehaus.jackson.JsonParser pointWebMerc2Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
				.geometryToJson(spatialReferenceWebMerc2, point1));
			com.esri.core.geometry.MapGeometry pointWebMerc2MP = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(pointWebMerc2Parser);
			NUnit.Framework.Assert.IsTrue(point1.getX() == ((com.esri.core.geometry.Point)pointWebMerc2MP
				.getGeometry()).getX());
			NUnit.Framework.Assert.IsTrue(point1.getY() == ((com.esri.core.geometry.Point)pointWebMerc2MP
				.getGeometry()).getY());
			NUnit.Framework.Assert.IsTrue(spatialReferenceWebMerc2.getLatestID() == pointWebMerc2MP
				.getSpatialReference().getLatestID());
			if (!checkResultSpatialRef(pointWebMerc2MP, spatialReferenceWebMerc2.getLatestID(
				), 0))
			{
				bAnswer = false;
			}
			{
				org.codehaus.jackson.JsonParser pointWgs84Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
					.geometryToJson(spatialReferenceWGS84, point1));
				com.esri.core.geometry.MapGeometry pointWgs84MP = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(pointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(point1.getX() == ((com.esri.core.geometry.Point)pointWgs84MP
					.getGeometry()).getX());
				NUnit.Framework.Assert.IsTrue(point1.getY() == ((com.esri.core.geometry.Point)pointWgs84MP
					.getGeometry()).getY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == pointWgs84MP.getSpatialReference
					().getID());
				if (!checkResultSpatialRef(pointWgs84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.esri.core.geometry.Point p = new com.esri.core.geometry.Point();
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"x\":null,\"y\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"x\":null,\"y\":null,\"z\":null,\"m\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
			}
			{
				com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(10.0, 20.0, 30.0
					);
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"x\":10,\"y\":20,\"z\":30,\"m\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
			}
			{
				// import
				string s = "{\"x\":0.0,\"y\":1.0,\"z\":5.0,\"m\":11.0,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}";
				org.codehaus.jackson.JsonParser parser = factory.createJsonParser(s);
				com.esri.core.geometry.MapGeometry map_pt = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(parser);
				com.esri.core.geometry.Point pt = (com.esri.core.geometry.Point)map_pt.getGeometry
					();
				NUnit.Framework.Assert.IsTrue(pt.getX() == 0.0);
				NUnit.Framework.Assert.IsTrue(pt.getY() == 1.0);
				NUnit.Framework.Assert.IsTrue(pt.getZ() == 5.0);
				NUnit.Framework.Assert.IsTrue(pt.getM() == 11.0);
			}
			{
				string s = "{\"x\" : 5.0, \"y\" : null, \"spatialReference\" : {\"wkid\" : 4326}} ";
				org.codehaus.jackson.JsonParser parser = factory.createJsonParser(s);
				com.esri.core.geometry.MapGeometry map_pt = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(parser);
				com.esri.core.geometry.Point pt = (com.esri.core.geometry.Point)map_pt.getGeometry
					();
				NUnit.Framework.Assert.IsTrue(pt.isEmpty());
				com.esri.core.geometry.SpatialReference spatial_reference = map_pt.getSpatialReference
					();
				NUnit.Framework.Assert.IsTrue(spatial_reference.getID() == 4326);
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool testMultiPoint()
		{
			bool bAnswer = true;
			com.esri.core.geometry.MultiPoint multiPoint1 = new com.esri.core.geometry.MultiPoint
				();
			multiPoint1.add(-97.06138, 32.837);
			multiPoint1.add(-97.06133, 32.836);
			multiPoint1.add(-97.06124, 32.834);
			multiPoint1.add(-97.06127, 32.832);
			{
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWGS84
					, multiPoint1);
				org.codehaus.jackson.JsonParser mPointWgs84Parser = factory.createJsonParser(s);
				com.esri.core.geometry.MapGeometry mPointWgs84MP = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(mPointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(multiPoint1.getPointCount() == ((com.esri.core.geometry.MultiPoint
					)mPointWgs84MP.getGeometry()).getPointCount());
				NUnit.Framework.Assert.IsTrue(multiPoint1.getPoint(0).getX() == ((com.esri.core.geometry.MultiPoint
					)mPointWgs84MP.getGeometry()).getPoint(0).getX());
				NUnit.Framework.Assert.IsTrue(multiPoint1.getPoint(0).getY() == ((com.esri.core.geometry.MultiPoint
					)mPointWgs84MP.getGeometry()).getPoint(0).getY());
				int lastIndex = multiPoint1.getPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(multiPoint1.getPoint(lastIndex).getX() == ((com.esri.core.geometry.MultiPoint
					)mPointWgs84MP.getGeometry()).getPoint(lastIndex).getX());
				NUnit.Framework.Assert.IsTrue(multiPoint1.getPoint(lastIndex).getY() == ((com.esri.core.geometry.MultiPoint
					)mPointWgs84MP.getGeometry()).getPoint(lastIndex).getY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == mPointWgs84MP.getSpatialReference
					().getID());
				if (!checkResultSpatialRef(mPointWgs84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.esri.core.geometry.MultiPoint p = new com.esri.core.geometry.MultiPoint();
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"points\":[],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
				p.add(10.0, 20.0, 30.0);
				p.add(20.0, 40.0, 60.0);
				s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"points\":[[10,20,30,null],[20,40,60,null]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
			}
			{
				string points = "{\"hasM\" : false, \"hasZ\" : true, \"uncle remus\" : null, \"points\" : [ [0,0,1], [0.0,10.0,1], [10.0,10.0,1], [10.0,0.0,1, 6666] ],\"spatialReference\" : {\"wkid\" : 4326}}";
				com.esri.core.geometry.MapGeometry mp = com.esri.core.geometry.GeometryEngine.jsonToGeometry
					(factory.createJsonParser(points));
				com.esri.core.geometry.MultiPoint multipoint = (com.esri.core.geometry.MultiPoint
					)mp.getGeometry();
				NUnit.Framework.Assert.IsTrue(multipoint.getPointCount() == 4);
				com.esri.core.geometry.Point2D point2d;
				point2d = multipoint.getXY(0);
				NUnit.Framework.Assert.IsTrue(point2d.x == 0.0 && point2d.y == 0.0);
				point2d = multipoint.getXY(1);
				NUnit.Framework.Assert.IsTrue(point2d.x == 0.0 && point2d.y == 10.0);
				point2d = multipoint.getXY(2);
				NUnit.Framework.Assert.IsTrue(point2d.x == 10.0 && point2d.y == 10.0);
				point2d = multipoint.getXY(3);
				NUnit.Framework.Assert.IsTrue(point2d.x == 10.0 && point2d.y == 0.0);
				NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z));
				NUnit.Framework.Assert.IsTrue(!multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.M));
				double z = multipoint.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 0, 0);
				NUnit.Framework.Assert.IsTrue(z == 1);
				com.esri.core.geometry.SpatialReference spatial_reference = mp.getSpatialReference
					();
				NUnit.Framework.Assert.IsTrue(spatial_reference.getID() == 4326);
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool testPolyline()
		{
			bool bAnswer = true;
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(-97.06138, 32.837);
			polyline.lineTo(-97.06133, 32.836);
			polyline.lineTo(-97.06124, 32.834);
			polyline.lineTo(-97.06127, 32.832);
			polyline.startPath(-97.06326, 32.759);
			polyline.lineTo(-97.06298, 32.755);
			{
				org.codehaus.jackson.JsonParser polylinePathsWgs84Parser = factory.createJsonParser
					(com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWGS84, polyline
					));
				com.esri.core.geometry.MapGeometry mPolylineWGS84MP = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(polylinePathsWgs84Parser);
				NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getPointCount());
				NUnit.Framework.Assert.IsTrue(polyline.getPoint(0).getX() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getPoint(0).getX());
				NUnit.Framework.Assert.IsTrue(polyline.getPoint(0).getY() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getPoint(0).getY());
				NUnit.Framework.Assert.IsTrue(polyline.getPathCount() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getPathCount());
				NUnit.Framework.Assert.IsTrue(polyline.getSegmentCount() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getSegmentCount());
				NUnit.Framework.Assert.IsTrue(polyline.getSegmentCount(0) == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getSegmentCount(0));
				NUnit.Framework.Assert.IsTrue(polyline.getSegmentCount(1) == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getSegmentCount(1));
				int lastIndex = polyline.getPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(polyline.getPoint(lastIndex).getX() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getPoint(lastIndex).getX());
				NUnit.Framework.Assert.IsTrue(polyline.getPoint(lastIndex).getY() == ((com.esri.core.geometry.Polyline
					)mPolylineWGS84MP.getGeometry()).getPoint(lastIndex).getY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == mPolylineWGS84MP.getSpatialReference
					().getID());
				if (!checkResultSpatialRef(mPolylineWGS84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.esri.core.geometry.Polyline p = new com.esri.core.geometry.Polyline();
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"paths\":[],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
				p.startPath(0, 0);
				p.lineTo(0, 1);
				p.startPath(2, 2);
				p.lineTo(3, 3);
				p.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 3);
				p.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 5);
				s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"paths\":[[[0,0,3,null],[0,1,0,5]],[[2,2,0,null],[3,3,0,null]]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
			}
			{
				string paths = "{\"hasZ\" : true, \"paths\" : [ [ [0.0, 0.0,3], [0, 10.0,3], [10.0, 10.0,3, 6666], [10.0, 0.0,3, 6666] ], [ [1.0, 1,3], [1.0, 9.0,3], [9.0, 9.0,3], [1.0, 9.0,3] ] ], \"spatialReference\" : {\"wkid\" : 4326}, \"hasM\" : false}";
				com.esri.core.geometry.MapGeometry mapGeometry = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(factory.createJsonParser(paths));
				com.esri.core.geometry.Polyline p = (com.esri.core.geometry.Polyline)mapGeometry.
					getGeometry();
				NUnit.Framework.Assert.IsTrue(p.getPathCount() == 2);
				int count = p.getPathCount();
				NUnit.Framework.Assert.IsTrue(p.getPointCount() == 8);
				NUnit.Framework.Assert.IsTrue(p.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z));
				NUnit.Framework.Assert.IsTrue(!p.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.M));
				double z = p.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 0, 0);
				NUnit.Framework.Assert.IsTrue(z == 3);
				double length = p.calculateLength2D();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 54.0) <= 0.001);
				com.esri.core.geometry.SpatialReference spatial_reference = mapGeometry.getSpatialReference
					();
				NUnit.Framework.Assert.IsTrue(spatial_reference.getID() == 4326);
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool testPolygon()
		{
			bool bAnswer = true;
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			polygon.startPath(-97.06138, 32.837);
			polygon.lineTo(-97.06133, 32.836);
			polygon.lineTo(-97.06124, 32.834);
			polygon.lineTo(-97.06127, 32.832);
			polygon.startPath(-97.06326, 32.759);
			polygon.lineTo(-97.06298, 32.755);
			{
				org.codehaus.jackson.JsonParser polygonPathsWgs84Parser = factory.createJsonParser
					(com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWGS84, polygon
					));
				com.esri.core.geometry.MapGeometry mPolygonWGS84MP = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(polygonPathsWgs84Parser);
				NUnit.Framework.Assert.IsTrue(polygon.getPointCount() + 1 == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getPointCount());
				NUnit.Framework.Assert.IsTrue(polygon.getPoint(0).getX() == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getPoint(0).getX());
				NUnit.Framework.Assert.IsTrue(polygon.getPoint(0).getY() == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getPoint(0).getY());
				NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getPathCount());
				NUnit.Framework.Assert.IsTrue(polygon.getSegmentCount() + 1 == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getSegmentCount());
				NUnit.Framework.Assert.IsTrue(polygon.getSegmentCount(0) == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getSegmentCount(0));
				NUnit.Framework.Assert.IsTrue(polygon.getSegmentCount(1) + 1 == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getSegmentCount(1));
				int lastIndex = polygon.getPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(polygon.getPoint(lastIndex).getX() == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getPoint(lastIndex).getX());
				NUnit.Framework.Assert.IsTrue(polygon.getPoint(lastIndex).getY() == ((com.esri.core.geometry.Polygon
					)mPolygonWGS84MP.getGeometry()).getPoint(lastIndex).getY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == mPolygonWGS84MP.getSpatialReference
					().getID());
				if (!checkResultSpatialRef(mPolygonWGS84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.esri.core.geometry.Polygon p = new com.esri.core.geometry.Polygon();
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				p.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"rings\":[],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
				p.startPath(0, 0);
				p.lineTo(0, 1);
				p.lineTo(4, 4);
				p.startPath(2, 2);
				p.lineTo(3, 3);
				p.lineTo(7, 8);
				p.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 3);
				p.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 7);
				p.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 5);
				p.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 5, 0, 5);
				s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"rings\":[[[0,0,3,null],[0,1,0,7],[4,4,0,5],[0,0,3,null]],[[2,2,0,null],[3,3,0,null],[7,8,0,5],[2,2,0,null]]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
			}
			{
				// Test Import Polygon from Polygon
				string rings = "{\"hasZ\": true, \"rings\" : [ [ [0,0, 5], [0.0, 10.0, 5], [10.0,10.0, 5, 66666], [10.0,0.0, 5] ], [ [12, 12] ],  [ [13 , 17], [13 , 17] ], [ [1.0, 1.0, 5, 66666], [9.0,1.0, 5], [9.0,9.0, 5], [1.0,9.0, 5], [1.0, 1.0, 5] ] ] }";
				com.esri.core.geometry.MapGeometry mapGeometry = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(factory.createJsonParser(rings));
				com.esri.core.geometry.Polygon p = (com.esri.core.geometry.Polygon)mapGeometry.getGeometry
					();
				double area = p.calculateArea2D();
				double length = p.calculateLength2D();
				NUnit.Framework.Assert.IsTrue(p.getPathCount() == 4);
				int count = p.getPointCount();
				NUnit.Framework.Assert.IsTrue(count == 15);
				NUnit.Framework.Assert.IsTrue(p.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z));
				NUnit.Framework.Assert.IsTrue(!p.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.M));
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool testEnvelope()
		{
			bool bAnswer = true;
			com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope();
			envelope.setCoords(-109.55, 25.76, -86.39, 49.94);
			{
				org.codehaus.jackson.JsonParser envelopeWGS84Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
					.geometryToJson(spatialReferenceWGS84, envelope));
				com.esri.core.geometry.MapGeometry envelopeWGS84MP = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(envelopeWGS84Parser);
				NUnit.Framework.Assert.IsTrue(envelope.isEmpty() == envelopeWGS84MP.getGeometry()
					.isEmpty());
				NUnit.Framework.Assert.IsTrue(envelope.getXMax() == ((com.esri.core.geometry.Envelope
					)envelopeWGS84MP.getGeometry()).getXMax());
				NUnit.Framework.Assert.IsTrue(envelope.getYMax() == ((com.esri.core.geometry.Envelope
					)envelopeWGS84MP.getGeometry()).getYMax());
				NUnit.Framework.Assert.IsTrue(envelope.getXMin() == ((com.esri.core.geometry.Envelope
					)envelopeWGS84MP.getGeometry()).getXMin());
				NUnit.Framework.Assert.IsTrue(envelope.getYMin() == ((com.esri.core.geometry.Envelope
					)envelopeWGS84MP.getGeometry()).getYMin());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == envelopeWGS84MP.getSpatialReference
					().getID());
				if (!checkResultSpatialRef(envelopeWGS84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				// export
				com.esri.core.geometry.Envelope e = new com.esri.core.geometry.Envelope();
				e.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				e.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				string s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, e);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"xmin\":null,\"ymin\":null,\"xmax\":null,\"ymax\":null,\"zmin\":null,\"zmax\":null,\"mmin\":null,\"mmax\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
				e.setCoords(0, 1, 2, 3);
				com.esri.core.geometry.Envelope1D z = new com.esri.core.geometry.Envelope1D();
				com.esri.core.geometry.Envelope1D m = new com.esri.core.geometry.Envelope1D();
				z.setCoords(5, 7);
				m.setCoords(11, 13);
				e.setInterval(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, z);
				e.setInterval(com.esri.core.geometry.VertexDescription.Semantics.M, 0, m);
				s = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, e);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"xmin\":0,\"ymin\":1,\"xmax\":2,\"ymax\":3,\"zmin\":5,\"zmax\":7,\"mmin\":11,\"mmax\":13,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"
					));
			}
			{
				// import
				string s = "{\"xmin\":0.0,\"ymin\":1.0,\"xmax\":2.0,\"ymax\":3.0,\"zmin\":5.0,\"zmax\":7.0,\"mmin\":11.0,\"mmax\":13.0,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}";
				org.codehaus.jackson.JsonParser parser = factory.createJsonParser(s);
				com.esri.core.geometry.MapGeometry map_env = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(parser);
				com.esri.core.geometry.Envelope env = (com.esri.core.geometry.Envelope)map_env.getGeometry
					();
				com.esri.core.geometry.Envelope1D z = env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 0);
				com.esri.core.geometry.Envelope1D m = env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
					.M, 0);
				NUnit.Framework.Assert.IsTrue(z.vmin == 5.0);
				NUnit.Framework.Assert.IsTrue(z.vmax == 7.0);
				NUnit.Framework.Assert.IsTrue(m.vmin == 11.0);
				NUnit.Framework.Assert.IsTrue(m.vmax == 13.0);
			}
			{
				string s = "{ \"zmin\" : 33, \"xmin\" : -109.55, \"zmax\" : 53, \"ymin\" : 25.76, \"xmax\" : -86.39, \"ymax\" : 49.94, \"mmax\" : 13}";
				org.codehaus.jackson.JsonParser parser = factory.createJsonParser(s);
				com.esri.core.geometry.MapGeometry map_env = com.esri.core.geometry.GeometryEngine
					.jsonToGeometry(parser);
				com.esri.core.geometry.Envelope env = (com.esri.core.geometry.Envelope)map_env.getGeometry
					();
				com.esri.core.geometry.Envelope2D e = new com.esri.core.geometry.Envelope2D();
				env.queryEnvelope2D(e);
				NUnit.Framework.Assert.IsTrue(e.xmin == -109.55 && e.ymin == 25.76 && e.xmax == -
					86.39 && e.ymax == 49.94);
				com.esri.core.geometry.Envelope1D e1D;
				NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z));
				e1D = env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics.Z, 0);
				NUnit.Framework.Assert.IsTrue(e1D.vmin == 33 && e1D.vmax == 53);
				NUnit.Framework.Assert.IsTrue(!env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.M));
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool testCR181369()
		{
			// CR181369
			bool bAnswer = true;
			string jsonStringPointAndWKT = "{\"x\":10.0,\"y\":20.0,\"spatialReference\":{\"wkt\" : \"PROJCS[\\\"NAD83_UTM_zone_15N\\\",GEOGCS[\\\"GCS_North_American_1983\\\",DATUM[\\\"D_North_American_1983\\\",SPHEROID[\\\"GRS_1980\\\",6378137.0,298.257222101]],PRIMEM[\\\"Greenwich\\\",0.0],UNIT[\\\"Degree\\\",0.0174532925199433]],PROJECTION[\\\"Transverse_Mercator\\\"],PARAMETER[\\\"false_easting\\\",500000.0],PARAMETER[\\\"false_northing\\\",0.0],PARAMETER[\\\"central_meridian\\\",-93.0],PARAMETER[\\\"scale_factor\\\",0.9996],PARAMETER[\\\"latitude_of_origin\\\",0.0],UNIT[\\\"Meter\\\",1.0]]\"} }";
			org.codehaus.jackson.JsonParser jsonParserPointAndWKT = factory.createJsonParser(
				jsonStringPointAndWKT);
			com.esri.core.geometry.MapGeometry mapGeom2 = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParserPointAndWKT);
			string jsonStringPointAndWKT2 = com.esri.core.geometry.GeometryEngine.geometryToJson
				(mapGeom2.getSpatialReference(), mapGeom2.getGeometry());
			org.codehaus.jackson.JsonParser jsonParserPointAndWKT2 = factory.createJsonParser
				(jsonStringPointAndWKT2);
			com.esri.core.geometry.MapGeometry mapGeom3 = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParserPointAndWKT2);
			NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Point)mapGeom2.getGeometry
				()).getX() == ((com.esri.core.geometry.Point)mapGeom3.getGeometry()).getX());
			NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Point)mapGeom2.getGeometry
				()).getY() == ((com.esri.core.geometry.Point)mapGeom3.getGeometry()).getY());
			string s1 = mapGeom2.getSpatialReference().getText();
			string s2 = mapGeom3.getSpatialReference().getText();
			NUnit.Framework.Assert.IsTrue(s1.Equals(s2));
			int id2 = mapGeom2.getSpatialReference().getID();
			int id3 = mapGeom3.getSpatialReference().getID();
			NUnit.Framework.Assert.IsTrue(id2 == id3);
			if (!checkResultSpatialRef(mapGeom3, mapGeom2.getSpatialReference().getID(), 0))
			{
				bAnswer = false;
			}
			return bAnswer;
		}

		internal virtual bool checkResultSpatialRef(com.esri.core.geometry.MapGeometry mapGeometry
			, int expectWki1, int expectWki2)
		{
			com.esri.core.geometry.SpatialReference sr = mapGeometry.getSpatialReference();
			string Wkt = sr.getText();
			int wki1 = sr.getLatestID();
			if (!(wki1 == expectWki1 || wki1 == expectWki2))
			{
				return false;
			}
			if (!(Wkt != null && Wkt.Length > 0))
			{
				return false;
			}
			com.esri.core.geometry.SpatialReference sr2 = com.esri.core.geometry.SpatialReference
				.create(Wkt);
			int wki2 = sr2.getID();
			if (expectWki2 > 0)
			{
				if (!(wki2 == expectWki1 || wki2 == expectWki2))
				{
					return false;
				}
			}
			else
			{
				if (!(wki2 == expectWki1))
				{
					return false;
				}
			}
			return true;
		}
	}
}
