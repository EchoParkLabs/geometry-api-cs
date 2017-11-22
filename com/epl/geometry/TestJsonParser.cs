

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestJsonParser
	{
		internal org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory
			();

		internal com.esri.core.geometry.SpatialReference spatialReferenceWebMerc1 = com.esri.core.geometry.SpatialReference
			.create(102100);

		internal com.esri.core.geometry.SpatialReference spatialReferenceWebMerc2 = com.esri.core.geometry.SpatialReference
			.create(spatialReferenceWebMerc1.getLatestID());

		internal com.esri.core.geometry.SpatialReference spatialReferenceWGS84 = com.esri.core.geometry.SpatialReference
			.create(4326);

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
		public virtual void test3DPoint()
		{
			string jsonString3DPt = "{\"x\" : -118.15, \"y\" : 33.80, \"z\" : 10.0, \"spatialReference\" : {\"wkid\" : 4326}}";
			org.codehaus.jackson.JsonParser jsonParser3DPt = factory.createJsonParser(jsonString3DPt
				);
			com.esri.core.geometry.MapGeometry point3DMP = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParser3DPt);
			NUnit.Framework.Assert.IsTrue(-118.15 == ((com.esri.core.geometry.Point)point3DMP
				.getGeometry()).getX());
			NUnit.Framework.Assert.IsTrue(33.80 == ((com.esri.core.geometry.Point)point3DMP.getGeometry
				()).getY());
			NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == point3DMP.getSpatialReference
				().getID());
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void test3DPoint1()
		{
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
				int srIdOri = spatialReferenceWebMerc1.getID();
				int srIdAfter = pointWebMerc1MP.getSpatialReference().getID();
				NUnit.Framework.Assert.IsTrue(srIdOri == srIdAfter || srIdAfter == 3857);
				pointWebMerc1Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
					.geometryToJson(null, point1));
				pointWebMerc1MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(pointWebMerc1Parser
					);
				NUnit.Framework.Assert.IsTrue(null == pointWebMerc1MP.getSpatialReference());
				string pointEmptyString = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWebMerc1
					, pointEmpty);
				pointWebMerc1Parser = factory.createJsonParser(pointEmptyString);
				pointWebMerc1MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(pointWebMerc1Parser
					);
				NUnit.Framework.Assert.IsTrue(pointWebMerc1MP.getGeometry().isEmpty());
				int srIdOri2 = spatialReferenceWebMerc1.getID();
				int srIdAfter2 = pointWebMerc1MP.getSpatialReference().getID();
				NUnit.Framework.Assert.IsTrue(srIdOri2 == srIdAfter2 || srIdAfter2 == 3857);
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void test3DPoint2()
		{
			{
				com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point(10.0, 20.0
					);
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
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void test3DPoint3()
		{
			{
				com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point(10.0, 20.0
					);
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
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testMultiPoint()
		{
			com.esri.core.geometry.MultiPoint multiPoint1 = new com.esri.core.geometry.MultiPoint
				();
			multiPoint1.add(-97.06138, 32.837);
			multiPoint1.add(-97.06133, 32.836);
			multiPoint1.add(-97.06124, 32.834);
			multiPoint1.add(-97.06127, 32.832);
			{
				org.codehaus.jackson.JsonParser mPointWgs84Parser = factory.createJsonParser(com.esri.core.geometry.GeometryEngine
					.geometryToJson(spatialReferenceWGS84, multiPoint1));
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
				com.esri.core.geometry.MultiPoint mPointEmpty = new com.esri.core.geometry.MultiPoint
					();
				string mPointEmptyString = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWGS84
					, mPointEmpty);
				mPointWgs84Parser = factory.createJsonParser(mPointEmptyString);
				mPointWgs84MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(mPointWgs84Parser
					);
				NUnit.Framework.Assert.IsTrue(mPointWgs84MP.getGeometry().isEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == mPointWgs84MP.getSpatialReference
					().getID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testPolyline()
		{
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
				com.esri.core.geometry.Polyline emptyPolyline = new com.esri.core.geometry.Polyline
					();
				string emptyString = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWGS84
					, emptyPolyline);
				mPolylineWGS84MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(factory.createJsonParser
					(emptyString));
				NUnit.Framework.Assert.IsTrue(mPolylineWGS84MP.getGeometry().isEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == mPolylineWGS84MP.getSpatialReference
					().getID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testPolygon()
		{
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
				com.esri.core.geometry.Polygon emptyPolygon = new com.esri.core.geometry.Polygon(
					);
				string emptyPolygonString = com.esri.core.geometry.GeometryEngine.geometryToJson(
					spatialReferenceWGS84, emptyPolygon);
				polygonPathsWgs84Parser = factory.createJsonParser(emptyPolygonString);
				mPolygonWGS84MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(polygonPathsWgs84Parser
					);
				NUnit.Framework.Assert.IsTrue(mPolygonWGS84MP.getGeometry().isEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == mPolygonWGS84MP.getSpatialReference
					().getID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testEnvelope()
		{
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
				com.esri.core.geometry.Envelope emptyEnvelope = new com.esri.core.geometry.Envelope
					();
				string emptyEnvString = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReferenceWGS84
					, emptyEnvelope);
				envelopeWGS84Parser = factory.createJsonParser(emptyEnvString);
				envelopeWGS84MP = com.esri.core.geometry.GeometryEngine.jsonToGeometry(envelopeWGS84Parser
					);
				NUnit.Framework.Assert.IsTrue(envelopeWGS84MP.getGeometry().isEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.getID() == envelopeWGS84MP.getSpatialReference
					().getID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testCR181369()
		{
			{
				// CR181369
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
				NUnit.Framework.Assert.IsTrue(mapGeom2.getSpatialReference().getText().Equals(mapGeom3
					.getSpatialReference().getText()));
				NUnit.Framework.Assert.IsTrue(mapGeom2.getSpatialReference().getID() == mapGeom3.
					getSpatialReference().getID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testSpatialRef()
		{
			// String jsonStringPt =
			// "{\"x\":-20037508.342787,\"y\":20037508.342787},\"spatialReference\":{\"wkid\":102100}}";
			string jsonStringPt = "{\"x\":10.0,\"y\":20.0,\"spatialReference\":{\"wkid\": 102100}}";
			// 102100
			string jsonStringPt2 = "{\"x\":10.0,\"y\":20.0,\"spatialReference\":{\"wkid\":4326}}";
			string jsonStringMpt = "{ \"points\" : [ [-97.06138,32.837], [-97.06133,32.836], [-97.06124,32.834], [-97.06127,32.832] ], \"spatialReference\" : {\"wkid\" : 4326}}";
			// 4326
			string jsonStringMpt3D = "{\"hasZs\" : true,\"points\" : [ [-97.06138,32.837,35.0], [-97.06133,32.836,35.1], [-97.06124,32.834,35.2], [-97.06127,32.832,35.3] ],\"spatialReference\" : {\"wkid\" : 4326}}";
			string jsonStringPl = "{\"paths\" : [  [ [-97.06138,32.837], [-97.06133,32.836], [-97.06124,32.834], [-97.06127,32.832] ],  [ [-97.06326,32.759], [-97.06298,32.755] ]],\"spatialReference\" : {\"wkid\" : 4326}}";
			string jsonStringPl3D = "{\"hasMs\" : true,\"paths\" : [[ [-97.06138,32.837,5], [-97.06133,32.836,6], [-97.06124,32.834,7], [-97.06127,32.832,8] ],[ [-97.06326,32.759], [-97.06298,32.755] ]],\"spatialReference\" : {\"wkid\" : 4326}}";
			string jsonStringPg = "{ \"rings\" :[  [ [-97.06138,32.837], [-97.06133,32.836], [-97.06124,32.834], [-97.06127,32.832], [-97.06138,32.837] ],  [ [-97.06326,32.759], [-97.06298,32.755], [-97.06153,32.749], [-97.06326,32.759] ]], \"spatialReference\" : {\"wkt\" : \"\"}}";
			string jsonStringPg3D = "{\"hasZs\" : true,\"hasMs\" : true,\"rings\" : [ [ [-97.06138, 32.837, 35.1, 4], [-97.06133, 32.836, 35.2, 4.1], [-97.06124, 32.834, 35.3, 4.2], [-97.06127, 32.832, 35.2, 44.3], [-97.06138, 32.837, 35.1, 4] ],[ [-97.06326, 32.759, 35.4], [-97.06298, 32.755, 35.5], [-97.06153, 32.749, 35.6], [-97.06326, 32.759, 35.4] ]],\"spatialReference\" : {\"wkid\" : 4326}}";
			string jsonStringPg2 = "{ \"spatialReference\" : {\"wkid\" : 4326}, \"rings\" : [[[-118.35,32.81],[-118.42,32.806],[-118.511,32.892],[-118.35,32.81]]]}";
			string jsonStringPg3 = "{ \"spatialReference\": {\"layerName\":\"GAS_POINTS\",\"name\":null,\"sdesrid\":102100,\"wkid\":102100,\"wkt\":null}}";
			string jsonString2SpatialReferences = "{ \"spatialReference\": {\"layerName\":\"GAS_POINTS\",\"name\":null,\"sdesrid\":102100,\"wkid\":102100,\"wkt\":\"GEOGCS[\\\"GCS_WGS_1984\\\",DATUM[\\\"D_WGS_1984\\\",SPHEROID[\\\"WGS_1984\\\",6378137,298.257223563]],PRIMEM[\\\"Greenwich\\\",0],UNIT[\\\"Degree\\\",0.017453292519943295]]\"}}";
			string jsonString2SpatialReferences2 = "{ \"spatialReference\": {\"layerName\":\"GAS_POINTS\",\"name\":null,\"sdesrid\":10,\"wkid\":10,\"wkt\":\"GEOGCS[\\\"GCS_WGS_1984\\\",DATUM[\\\"D_WGS_1984\\\",SPHEROID[\\\"WGS_1984\\\",6378137,298.257223563]],PRIMEM[\\\"Greenwich\\\",0],UNIT[\\\"Degree\\\",0.017453292519943295]]\"}}";
			string jsonStringSR = "{\"wkid\" : 4326}";
			string jsonStringEnv = "{\"xmin\" : -109.55, \"ymin\" : 25.76, \"xmax\" : -86.39, \"ymax\" : 49.94,\"spatialReference\" : {\"wkid\" : 4326}}";
			string jsonStringHongKon = "{\"xmin\" : -122.55, \"ymin\" : 37.65, \"xmax\" : -122.28, \"ymax\" : 37.84,\"spatialReference\" : {\"wkid\" : 4326}}";
			string jsonStringWKT = " {\"wkt\" : \"GEOGCS[\\\"GCS_WGS_1984\\\",DATUM[\\\"D_WGS_1984\\\",SPHEROID[\\\"WGS_1984\\\",6378137,298.257223563]],PRIMEM[\\\"Greenwich\\\",0],UNIT[\\\"Degree\\\",0.017453292519943295]]\"}";
			string jsonStringInvalidWKID = "{\"x\":10.0,\"y\":20.0},\"spatialReference\":{\"wkid\":35253523}}";
			string jsonStringOregon = "{\"xmin\":7531831.219849482,\"ymin\":585702.9799639136,\"xmax\":7750143.589982405,\"ymax\":733289.6299999952,\"spatialReference\":{\"wkid\":102726}}";
			org.codehaus.jackson.JsonParser jsonParserPt = factory.createJsonParser(jsonStringPt
				);
			org.codehaus.jackson.JsonParser jsonParserMpt = factory.createJsonParser(jsonStringMpt
				);
			org.codehaus.jackson.JsonParser jsonParserMpt3D = factory.createJsonParser(jsonStringMpt3D
				);
			org.codehaus.jackson.JsonParser jsonParserPl = factory.createJsonParser(jsonStringPl
				);
			org.codehaus.jackson.JsonParser jsonParserPl3D = factory.createJsonParser(jsonStringPl3D
				);
			org.codehaus.jackson.JsonParser jsonParserPg = factory.createJsonParser(jsonStringPg
				);
			org.codehaus.jackson.JsonParser jsonParserPg3D = factory.createJsonParser(jsonStringPg3D
				);
			org.codehaus.jackson.JsonParser jsonParserPg2 = factory.createJsonParser(jsonStringPg2
				);
			org.codehaus.jackson.JsonParser jsonParserSR = factory.createJsonParser(jsonStringSR
				);
			org.codehaus.jackson.JsonParser jsonParserEnv = factory.createJsonParser(jsonStringEnv
				);
			org.codehaus.jackson.JsonParser jsonParserPg3 = factory.createJsonParser(jsonStringPg3
				);
			org.codehaus.jackson.JsonParser jsonParserCrazy1 = factory.createJsonParser(jsonString2SpatialReferences
				);
			org.codehaus.jackson.JsonParser jsonParserCrazy2 = factory.createJsonParser(jsonString2SpatialReferences2
				);
			org.codehaus.jackson.JsonParser jsonParserInvalidWKID = factory.createJsonParser(
				jsonStringInvalidWKID);
			org.codehaus.jackson.JsonParser jsonParseHongKon = factory.createJsonParser(jsonStringHongKon
				);
			org.codehaus.jackson.JsonParser jsonParseOregon = factory.createJsonParser(jsonStringOregon
				);
			com.esri.core.geometry.MapGeometry mapGeom = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParserPt);
			// showProjectedGeometryInfo(mapGeom);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 102100);
			com.esri.core.geometry.MapGeometry mapGeomOregon = com.esri.core.geometry.GeometryEngine
				.jsonToGeometry(jsonParseOregon);
			NUnit.Framework.Assert.IsTrue(mapGeomOregon.getSpatialReference().getID() == 102726
				);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserMpt);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 4326);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserMpt3D);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 4326);
			{
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)mapGeom.getGeometry
					()).getPoint(0).getX() == -97.06138);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)mapGeom.getGeometry
					()).getPoint(0).getY() == 32.837);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)mapGeom.getGeometry
					()).getPoint(3).getX() == -97.06127);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)mapGeom.getGeometry
					()).getPoint(3).getY() == 32.832);
			}
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserPl);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 4326);
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserPl3D);
			{
				// [[ [-97.06138,32.837,5], [-97.06133,32.836,6],
				// [-97.06124,32.834,7], [-97.06127,32.832,8] ],
				// [ [-97.06326,32.759], [-97.06298,32.755] ]]";
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)mapGeom.getGeometry
					()).getPoint(0).getX() == -97.06138);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)mapGeom.getGeometry
					()).getPoint(0).getY() == 32.837);
				int lastIndex = ((com.esri.core.geometry.Polyline)mapGeom.getGeometry()).getPointCount
					() - 1;
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)mapGeom.getGeometry
					()).getPoint(lastIndex).getX() == -97.06298);
				// -97.06153, 32.749
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)mapGeom.getGeometry
					()).getPoint(lastIndex).getY() == 32.755);
				int lastIndexFirstLine = ((com.esri.core.geometry.Polyline)mapGeom.getGeometry())
					.getPathEnd(0) - 1;
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)mapGeom.getGeometry
					()).getPoint(lastIndexFirstLine).getX() == -97.06127);
				// -97.06153,
				// 32.749
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)mapGeom.getGeometry
					()).getPoint(lastIndexFirstLine).getY() == 32.832);
			}
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserPg);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference() == null);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserPg3D);
			{
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)mapGeom.getGeometry
					()).getPoint(0).getX() == -97.06138);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)mapGeom.getGeometry
					()).getPoint(0).getY() == 32.837);
				int lastIndex = ((com.esri.core.geometry.Polygon)mapGeom.getGeometry()).getPointCount
					() - 1;
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)mapGeom.getGeometry
					()).getPoint(lastIndex).getX() == -97.06153);
				// -97.06153, 32.749
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)mapGeom.getGeometry
					()).getPoint(lastIndex).getY() == 32.749);
			}
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserPg2);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 4326);
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserPg3);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 102100);
			// showProjectedGeometryInfo(mapGeom);
			// mapGeom = GeometryEngine.jsonToGeometry(jsonParserCrazy1);
			// Assert.assertTrue(mapGeom.getSpatialReference().getText().equals(""));
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserEnv);
			NUnit.Framework.Assert.IsTrue(mapGeom.getSpatialReference().getID() == 4326);
			// showProjectedGeometryInfo(mapGeom);
			try
			{
				com.esri.core.geometry.GeometryEngine.jsonToGeometry(jsonParserInvalidWKID);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false, "Should not throw for invalid wkid");
			}
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
			com.esri.core.geometry.SpatialReference spatialReference = com.esri.core.geometry.SpatialReference
				.create(4326);
			try
			{
				string jSonStr = com.esri.core.geometry.GeometryEngine.geometryToJson(spatialReference
					, pg);
				org.codehaus.jackson.JsonFactory jf = new org.codehaus.jackson.JsonFactory();
				org.codehaus.jackson.JsonParser jp = jf.createJsonParser(jSonStr);
				jp.nextToken();
				com.esri.core.geometry.MapGeometry mg = com.esri.core.geometry.GeometryEngine.jsonToGeometry
					(jp);
				com.esri.core.geometry.Geometry gm = mg.getGeometry();
				NUnit.Framework.Assert.AreEqual(com.esri.core.geometry.Geometry.Type.Polygon, gm.
					getType());
				NUnit.Framework.Assert.IsTrue(mg.getSpatialReference().getID() == 4326);
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
		[NUnit.Framework.Test]
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

		private static void showProjectedGeometryInfo(com.esri.core.geometry.MapGeometry 
			mapGeom)
		{
			System.Console.Out.WriteLine("\n");
			com.esri.core.geometry.MapGeometry geom = mapGeom;
			// while ((geom = geomCursor.next()) != null) {
			if (geom.getGeometry() is com.esri.core.geometry.Point)
			{
				com.esri.core.geometry.Point pnt = (com.esri.core.geometry.Point)geom.getGeometry
					();
				System.Console.Out.WriteLine("Point(" + pnt.getX() + " , " + pnt.getY() + ")");
				if (geom.getSpatialReference() == null)
				{
					System.Console.Out.WriteLine("No spatial reference");
				}
				else
				{
					System.Console.Out.WriteLine("wkid: " + geom.getSpatialReference().getID());
				}
			}
			else
			{
				if (geom.getGeometry() is com.esri.core.geometry.MultiPoint)
				{
					com.esri.core.geometry.MultiPoint mp = (com.esri.core.geometry.MultiPoint)geom.getGeometry
						();
					System.Console.Out.WriteLine("Multipoint has " + mp.getPointCount() + " points.");
					System.Console.Out.WriteLine("wkid: " + geom.getSpatialReference().getID());
				}
				else
				{
					if (geom.getGeometry() is com.esri.core.geometry.Polygon)
					{
						com.esri.core.geometry.Polygon mp = (com.esri.core.geometry.Polygon)geom.getGeometry
							();
						System.Console.Out.WriteLine("Polygon has " + mp.getPointCount() + " points and "
							 + mp.getPathCount() + " parts.");
						if (mp.getPathCount() > 1)
						{
							System.Console.Out.WriteLine("Part start of 2nd segment : " + mp.getPathStart(1));
							System.Console.Out.WriteLine("Part end of 2nd segment   : " + mp.getPathEnd(1));
							System.Console.Out.WriteLine("Part size of 2nd segment  : " + mp.getPathSize(1));
							int start = mp.getPathStart(1);
							int end = mp.getPathEnd(1);
							for (int i = start; i < end; i++)
							{
								com.esri.core.geometry.Point pp = mp.getPoint(i);
								System.Console.Out.WriteLine("Point(" + i + ") = (" + pp.getX() + ", " + pp.getY(
									) + ")");
							}
						}
						System.Console.Out.WriteLine("wkid: " + geom.getSpatialReference().getID());
					}
					else
					{
						if (geom.getGeometry() is com.esri.core.geometry.Polyline)
						{
							com.esri.core.geometry.Polyline mp = (com.esri.core.geometry.Polyline)geom.getGeometry
								();
							System.Console.Out.WriteLine("Polyline has " + mp.getPointCount() + " points and "
								 + mp.getPathCount() + " parts.");
							System.Console.Out.WriteLine("Part start of 2nd segment : " + mp.getPathStart(1));
							System.Console.Out.WriteLine("Part end of 2nd segment   : " + mp.getPathEnd(1));
							System.Console.Out.WriteLine("Part size of 2nd segment  : " + mp.getPathSize(1));
							int start = mp.getPathStart(1);
							int end = mp.getPathEnd(1);
							for (int i = start; i < end; i++)
							{
								com.esri.core.geometry.Point pp = mp.getPoint(i);
								System.Console.Out.WriteLine("Point(" + i + ") = (" + pp.getX() + ", " + pp.getY(
									) + ")");
							}
							System.Console.Out.WriteLine("wkid: " + geom.getSpatialReference().getID());
						}
					}
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void testGeometryToJSON()
		{
			com.esri.core.geometry.Polygon geom = new com.esri.core.geometry.Polygon();
			geom.startPath(new com.esri.core.geometry.Point(-113, 34));
			geom.lineTo(new com.esri.core.geometry.Point(-105, 34));
			geom.lineTo(new com.esri.core.geometry.Point(-108, 40));
			string outputPolygon1 = com.esri.core.geometry.GeometryEngine.geometryToJson(-1, 
				geom);
			// Test
			// WKID
			// == -1
			//System.out.println("Geom JSON STRING is" + outputPolygon1);
			string correctPolygon1 = "{\"rings\":[[[-113,34],[-105,34],[-108,40],[-113,34]]]}";
			NUnit.Framework.Assert.AreEqual(correctPolygon1, outputPolygon1);
			string outputPolygon2 = com.esri.core.geometry.GeometryEngine.geometryToJson(4326
				, geom);
			//System.out.println("Geom JSON STRING is" + outputPolygon2);
			string correctPolygon2 = "{\"rings\":[[[-113,34],[-105,34],[-108,40],[-113,34]]],\"spatialReference\":{\"wkid\":4326}}";
			NUnit.Framework.Assert.AreEqual(correctPolygon2, outputPolygon2);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void testGeometryToJSONOldID()
		{
			// CR
			com.esri.core.geometry.Polygon geom = new com.esri.core.geometry.Polygon();
			geom.startPath(new com.esri.core.geometry.Point(-113, 34));
			geom.lineTo(new com.esri.core.geometry.Point(-105, 34));
			geom.lineTo(new com.esri.core.geometry.Point(-108, 40));
			string outputPolygon = com.esri.core.geometry.GeometryEngine.geometryToJson(com.esri.core.geometry.SpatialReference
				.create(3857), geom);
			// Test WKID == -1
			string correctPolygon = "{\"rings\":[[[-113,34],[-105,34],[-108,40],[-113,34]]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}";
			NUnit.Framework.Assert.IsTrue(outputPolygon.Equals(correctPolygon));
			org.codehaus.jackson.JsonFactory jf = new org.codehaus.jackson.JsonFactory();
			org.codehaus.jackson.JsonParser jp = jf.createJsonParser(outputPolygon);
			jp.nextToken();
			com.esri.core.geometry.MapGeometry mg = com.esri.core.geometry.GeometryEngine.jsonToGeometry
				(jp);
			int srId = mg.getSpatialReference().getID();
			int srOldId = mg.getSpatialReference().getOldID();
			NUnit.Framework.Assert.IsTrue(mg.getSpatialReference().getID() == 3857);
			NUnit.Framework.Assert.IsTrue(mg.getSpatialReference().getLatestID() == 3857);
			NUnit.Framework.Assert.IsTrue(mg.getSpatialReference().getOldID() == 102100);
		}
	}
}
