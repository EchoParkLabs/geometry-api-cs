using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestJsonParser : NUnit.Framework.TestFixtureAttribute
	{
		internal org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory();

		internal com.epl.geometry.SpatialReference spatialReferenceWebMerc1 = com.epl.geometry.SpatialReference.Create(102100);

		internal com.epl.geometry.SpatialReference spatialReferenceWebMerc2 = com.epl.geometry.SpatialReference.Create(spatialReferenceWebMerc1.GetLatestID());

		internal com.epl.geometry.SpatialReference spatialReferenceWGS84 = com.epl.geometry.SpatialReference.Create(4326);

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
		public virtual void Test3DPoint()
		{
			string jsonString3DPt = "{\"x\" : -118.15, \"y\" : 33.80, \"z\" : 10.0, \"spatialReference\" : {\"wkid\" : 4326}}";
			org.codehaus.jackson.JsonParser jsonParser3DPt = factory.CreateJsonParser(jsonString3DPt);
			com.epl.geometry.MapGeometry point3DMP = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParser3DPt);
			NUnit.Framework.Assert.IsTrue(-118.15 == ((com.epl.geometry.Point)point3DMP.GetGeometry()).GetX());
			NUnit.Framework.Assert.IsTrue(33.80 == ((com.epl.geometry.Point)point3DMP.GetGeometry()).GetY());
			NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == point3DMP.GetSpatialReference().GetID());
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Test3DPoint1()
		{
			com.epl.geometry.Point point1 = new com.epl.geometry.Point(10.0, 20.0);
			com.epl.geometry.Point pointEmpty = new com.epl.geometry.Point();
			{
				org.codehaus.jackson.JsonParser pointWebMerc1Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, point1));
				com.epl.geometry.MapGeometry pointWebMerc1MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc1Parser);
				NUnit.Framework.Assert.IsTrue(point1.GetX() == ((com.epl.geometry.Point)pointWebMerc1MP.GetGeometry()).GetX());
				NUnit.Framework.Assert.IsTrue(point1.GetY() == ((com.epl.geometry.Point)pointWebMerc1MP.GetGeometry()).GetY());
				int srIdOri = spatialReferenceWebMerc1.GetID();
				int srIdAfter = pointWebMerc1MP.GetSpatialReference().GetID();
				NUnit.Framework.Assert.IsTrue(srIdOri == srIdAfter || srIdAfter == 3857);
				pointWebMerc1Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(null, point1));
				pointWebMerc1MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc1Parser);
				NUnit.Framework.Assert.IsTrue(null == pointWebMerc1MP.GetSpatialReference());
				string pointEmptyString = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, pointEmpty);
				pointWebMerc1Parser = factory.CreateJsonParser(pointEmptyString);
				pointWebMerc1MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc1Parser);
				NUnit.Framework.Assert.IsTrue(pointWebMerc1MP.GetGeometry().IsEmpty());
				int srIdOri2 = spatialReferenceWebMerc1.GetID();
				int srIdAfter2 = pointWebMerc1MP.GetSpatialReference().GetID();
				NUnit.Framework.Assert.IsTrue(srIdOri2 == srIdAfter2 || srIdAfter2 == 3857);
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Test3DPoint2()
		{
			{
				com.epl.geometry.Point point1 = new com.epl.geometry.Point(10.0, 20.0);
				org.codehaus.jackson.JsonParser pointWebMerc2Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc2, point1));
				com.epl.geometry.MapGeometry pointWebMerc2MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc2Parser);
				NUnit.Framework.Assert.IsTrue(point1.GetX() == ((com.epl.geometry.Point)pointWebMerc2MP.GetGeometry()).GetX());
				NUnit.Framework.Assert.IsTrue(point1.GetY() == ((com.epl.geometry.Point)pointWebMerc2MP.GetGeometry()).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWebMerc2.GetLatestID() == pointWebMerc2MP.GetSpatialReference().GetLatestID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Test3DPoint3()
		{
			{
				com.epl.geometry.Point point1 = new com.epl.geometry.Point(10.0, 20.0);
				org.codehaus.jackson.JsonParser pointWgs84Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, point1));
				com.epl.geometry.MapGeometry pointWgs84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(point1.GetX() == ((com.epl.geometry.Point)pointWgs84MP.GetGeometry()).GetX());
				NUnit.Framework.Assert.IsTrue(point1.GetY() == ((com.epl.geometry.Point)pointWgs84MP.GetGeometry()).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == pointWgs84MP.GetSpatialReference().GetID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestMultiPoint()
		{
			com.epl.geometry.MultiPoint multiPoint1 = new com.epl.geometry.MultiPoint();
			multiPoint1.Add(-97.06138, 32.837);
			multiPoint1.Add(-97.06133, 32.836);
			multiPoint1.Add(-97.06124, 32.834);
			multiPoint1.Add(-97.06127, 32.832);
			{
				org.codehaus.jackson.JsonParser mPointWgs84Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, multiPoint1));
				com.epl.geometry.MapGeometry mPointWgs84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(mPointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPointCount() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPointCount());
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(0).GetX() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(0).GetX());
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(0).GetY() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(0).GetY());
				int lastIndex = multiPoint1.GetPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(lastIndex).GetX() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(lastIndex).GetX());
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(lastIndex).GetY() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(lastIndex).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPointWgs84MP.GetSpatialReference().GetID());
				com.epl.geometry.MultiPoint mPointEmpty = new com.epl.geometry.MultiPoint();
				string mPointEmptyString = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, mPointEmpty);
				mPointWgs84Parser = factory.CreateJsonParser(mPointEmptyString);
				mPointWgs84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(mPointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(mPointWgs84MP.GetGeometry().IsEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPointWgs84MP.GetSpatialReference().GetID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestPolyline()
		{
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.StartPath(-97.06138, 32.837);
			polyline.LineTo(-97.06133, 32.836);
			polyline.LineTo(-97.06124, 32.834);
			polyline.LineTo(-97.06127, 32.832);
			polyline.StartPath(-97.06326, 32.759);
			polyline.LineTo(-97.06298, 32.755);
			{
				org.codehaus.jackson.JsonParser polylinePathsWgs84Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, polyline));
				com.epl.geometry.MapGeometry mPolylineWGS84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(polylinePathsWgs84Parser);
				NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetPointCount());
				NUnit.Framework.Assert.IsTrue(polyline.GetPoint(0).GetX() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetPoint(0).GetX());
				NUnit.Framework.Assert.IsTrue(polyline.GetPoint(0).GetY() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetPoint(0).GetY());
				NUnit.Framework.Assert.IsTrue(polyline.GetPathCount() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetPathCount());
				NUnit.Framework.Assert.IsTrue(polyline.GetSegmentCount() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetSegmentCount());
				NUnit.Framework.Assert.IsTrue(polyline.GetSegmentCount(0) == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetSegmentCount(0));
				NUnit.Framework.Assert.IsTrue(polyline.GetSegmentCount(1) == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetSegmentCount(1));
				int lastIndex = polyline.GetPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(polyline.GetPoint(lastIndex).GetX() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetPoint(lastIndex).GetX());
				NUnit.Framework.Assert.IsTrue(polyline.GetPoint(lastIndex).GetY() == ((com.epl.geometry.Polyline)mPolylineWGS84MP.GetGeometry()).GetPoint(lastIndex).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPolylineWGS84MP.GetSpatialReference().GetID());
				com.epl.geometry.Polyline emptyPolyline = new com.epl.geometry.Polyline();
				string emptyString = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, emptyPolyline);
				mPolylineWGS84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(factory.CreateJsonParser(emptyString));
				NUnit.Framework.Assert.IsTrue(mPolylineWGS84MP.GetGeometry().IsEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPolylineWGS84MP.GetSpatialReference().GetID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestPolygon()
		{
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			polygon.StartPath(-97.06138, 32.837);
			polygon.LineTo(-97.06133, 32.836);
			polygon.LineTo(-97.06124, 32.834);
			polygon.LineTo(-97.06127, 32.832);
			polygon.StartPath(-97.06326, 32.759);
			polygon.LineTo(-97.06298, 32.755);
			{
				org.codehaus.jackson.JsonParser polygonPathsWgs84Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, polygon));
				com.epl.geometry.MapGeometry mPolygonWGS84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(polygonPathsWgs84Parser);
				NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() + 1 == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetPointCount());
				NUnit.Framework.Assert.IsTrue(polygon.GetPoint(0).GetX() == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetPoint(0).GetX());
				NUnit.Framework.Assert.IsTrue(polygon.GetPoint(0).GetY() == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetPoint(0).GetY());
				NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetPathCount());
				NUnit.Framework.Assert.IsTrue(polygon.GetSegmentCount() + 1 == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetSegmentCount());
				NUnit.Framework.Assert.IsTrue(polygon.GetSegmentCount(0) == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetSegmentCount(0));
				NUnit.Framework.Assert.IsTrue(polygon.GetSegmentCount(1) + 1 == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetSegmentCount(1));
				int lastIndex = polygon.GetPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(polygon.GetPoint(lastIndex).GetX() == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetPoint(lastIndex).GetX());
				NUnit.Framework.Assert.IsTrue(polygon.GetPoint(lastIndex).GetY() == ((com.epl.geometry.Polygon)mPolygonWGS84MP.GetGeometry()).GetPoint(lastIndex).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPolygonWGS84MP.GetSpatialReference().GetID());
				com.epl.geometry.Polygon emptyPolygon = new com.epl.geometry.Polygon();
				string emptyPolygonString = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, emptyPolygon);
				polygonPathsWgs84Parser = factory.CreateJsonParser(emptyPolygonString);
				mPolygonWGS84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(polygonPathsWgs84Parser);
				NUnit.Framework.Assert.IsTrue(mPolygonWGS84MP.GetGeometry().IsEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPolygonWGS84MP.GetSpatialReference().GetID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestEnvelope()
		{
			com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope();
			envelope.SetCoords(-109.55, 25.76, -86.39, 49.94);
			{
				org.codehaus.jackson.JsonParser envelopeWGS84Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, envelope));
				com.epl.geometry.MapGeometry envelopeWGS84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(envelopeWGS84Parser);
				NUnit.Framework.Assert.IsTrue(envelope.IsEmpty() == envelopeWGS84MP.GetGeometry().IsEmpty());
				NUnit.Framework.Assert.IsTrue(envelope.GetXMax() == ((com.epl.geometry.Envelope)envelopeWGS84MP.GetGeometry()).GetXMax());
				NUnit.Framework.Assert.IsTrue(envelope.GetYMax() == ((com.epl.geometry.Envelope)envelopeWGS84MP.GetGeometry()).GetYMax());
				NUnit.Framework.Assert.IsTrue(envelope.GetXMin() == ((com.epl.geometry.Envelope)envelopeWGS84MP.GetGeometry()).GetXMin());
				NUnit.Framework.Assert.IsTrue(envelope.GetYMin() == ((com.epl.geometry.Envelope)envelopeWGS84MP.GetGeometry()).GetYMin());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == envelopeWGS84MP.GetSpatialReference().GetID());
				com.epl.geometry.Envelope emptyEnvelope = new com.epl.geometry.Envelope();
				string emptyEnvString = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, emptyEnvelope);
				envelopeWGS84Parser = factory.CreateJsonParser(emptyEnvString);
				envelopeWGS84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(envelopeWGS84Parser);
				NUnit.Framework.Assert.IsTrue(envelopeWGS84MP.GetGeometry().IsEmpty());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == envelopeWGS84MP.GetSpatialReference().GetID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestCR181369()
		{
			{
				// CR181369
				string jsonStringPointAndWKT = "{\"x\":10.0,\"y\":20.0,\"spatialReference\":{\"wkt\" : \"PROJCS[\\\"NAD83_UTM_zone_15N\\\",GEOGCS[\\\"GCS_North_American_1983\\\",DATUM[\\\"D_North_American_1983\\\",SPHEROID[\\\"GRS_1980\\\",6378137.0,298.257222101]],PRIMEM[\\\"Greenwich\\\",0.0],UNIT[\\\"Degree\\\",0.0174532925199433]],PROJECTION[\\\"Transverse_Mercator\\\"],PARAMETER[\\\"false_easting\\\",500000.0],PARAMETER[\\\"false_northing\\\",0.0],PARAMETER[\\\"central_meridian\\\",-93.0],PARAMETER[\\\"scale_factor\\\",0.9996],PARAMETER[\\\"latitude_of_origin\\\",0.0],UNIT[\\\"Meter\\\",1.0]]\"} }";
				org.codehaus.jackson.JsonParser jsonParserPointAndWKT = factory.CreateJsonParser(jsonStringPointAndWKT);
				com.epl.geometry.MapGeometry mapGeom2 = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPointAndWKT);
				string jsonStringPointAndWKT2 = com.epl.geometry.GeometryEngine.GeometryToJson(mapGeom2.GetSpatialReference(), mapGeom2.GetGeometry());
				org.codehaus.jackson.JsonParser jsonParserPointAndWKT2 = factory.CreateJsonParser(jsonStringPointAndWKT2);
				com.epl.geometry.MapGeometry mapGeom3 = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPointAndWKT2);
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Point)mapGeom2.GetGeometry()).GetX() == ((com.epl.geometry.Point)mapGeom3.GetGeometry()).GetX());
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Point)mapGeom2.GetGeometry()).GetY() == ((com.epl.geometry.Point)mapGeom3.GetGeometry()).GetY());
				NUnit.Framework.Assert.IsTrue(mapGeom2.GetSpatialReference().GetText().Equals(mapGeom3.GetSpatialReference().GetText()));
				NUnit.Framework.Assert.IsTrue(mapGeom2.GetSpatialReference().GetID() == mapGeom3.GetSpatialReference().GetID());
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestSpatialRef()
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
			org.codehaus.jackson.JsonParser jsonParserPt = factory.CreateJsonParser(jsonStringPt);
			org.codehaus.jackson.JsonParser jsonParserMpt = factory.CreateJsonParser(jsonStringMpt);
			org.codehaus.jackson.JsonParser jsonParserMpt3D = factory.CreateJsonParser(jsonStringMpt3D);
			org.codehaus.jackson.JsonParser jsonParserPl = factory.CreateJsonParser(jsonStringPl);
			org.codehaus.jackson.JsonParser jsonParserPl3D = factory.CreateJsonParser(jsonStringPl3D);
			org.codehaus.jackson.JsonParser jsonParserPg = factory.CreateJsonParser(jsonStringPg);
			org.codehaus.jackson.JsonParser jsonParserPg3D = factory.CreateJsonParser(jsonStringPg3D);
			org.codehaus.jackson.JsonParser jsonParserPg2 = factory.CreateJsonParser(jsonStringPg2);
			org.codehaus.jackson.JsonParser jsonParserSR = factory.CreateJsonParser(jsonStringSR);
			org.codehaus.jackson.JsonParser jsonParserEnv = factory.CreateJsonParser(jsonStringEnv);
			org.codehaus.jackson.JsonParser jsonParserPg3 = factory.CreateJsonParser(jsonStringPg3);
			org.codehaus.jackson.JsonParser jsonParserCrazy1 = factory.CreateJsonParser(jsonString2SpatialReferences);
			org.codehaus.jackson.JsonParser jsonParserCrazy2 = factory.CreateJsonParser(jsonString2SpatialReferences2);
			org.codehaus.jackson.JsonParser jsonParserInvalidWKID = factory.CreateJsonParser(jsonStringInvalidWKID);
			org.codehaus.jackson.JsonParser jsonParseHongKon = factory.CreateJsonParser(jsonStringHongKon);
			org.codehaus.jackson.JsonParser jsonParseOregon = factory.CreateJsonParser(jsonStringOregon);
			com.epl.geometry.MapGeometry mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPt);
			// showProjectedGeometryInfo(mapGeom);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 102100);
			com.epl.geometry.MapGeometry mapGeomOregon = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParseOregon);
			NUnit.Framework.Assert.IsTrue(mapGeomOregon.GetSpatialReference().GetID() == 102726);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserMpt);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 4326);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserMpt3D);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 4326);
			{
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.MultiPoint)mapGeom.GetGeometry()).GetPoint(0).GetX() == -97.06138);
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.MultiPoint)mapGeom.GetGeometry()).GetPoint(0).GetY() == 32.837);
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.MultiPoint)mapGeom.GetGeometry()).GetPoint(3).GetX() == -97.06127);
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.MultiPoint)mapGeom.GetGeometry()).GetPoint(3).GetY() == 32.832);
			}
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPl);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 4326);
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPl3D);
			{
				// [[ [-97.06138,32.837,5], [-97.06133,32.836,6],
				// [-97.06124,32.834,7], [-97.06127,32.832,8] ],
				// [ [-97.06326,32.759], [-97.06298,32.755] ]]";
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPoint(0).GetX() == -97.06138);
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPoint(0).GetY() == 32.837);
				int lastIndex = ((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPoint(lastIndex).GetX() == -97.06298);
				// -97.06153, 32.749
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPoint(lastIndex).GetY() == 32.755);
				int lastIndexFirstLine = ((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPathEnd(0) - 1;
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPoint(lastIndexFirstLine).GetX() == -97.06127);
				// -97.06153,
				// 32.749
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polyline)mapGeom.GetGeometry()).GetPoint(lastIndexFirstLine).GetY() == 32.832);
			}
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPg);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference() == null);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPg3D);
			{
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polygon)mapGeom.GetGeometry()).GetPoint(0).GetX() == -97.06138);
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polygon)mapGeom.GetGeometry()).GetPoint(0).GetY() == 32.837);
				int lastIndex = ((com.epl.geometry.Polygon)mapGeom.GetGeometry()).GetPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polygon)mapGeom.GetGeometry()).GetPoint(lastIndex).GetX() == -97.06153);
				// -97.06153, 32.749
				NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polygon)mapGeom.GetGeometry()).GetPoint(lastIndex).GetY() == 32.749);
			}
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPg2);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 4326);
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPg3);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 102100);
			// showProjectedGeometryInfo(mapGeom);
			// mapGeom = GeometryEngine.jsonToGeometry(jsonParserCrazy1);
			// Assert.assertTrue(mapGeom.getSpatialReference().getText().equals(""));
			// showProjectedGeometryInfo(mapGeom);
			mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserEnv);
			NUnit.Framework.Assert.IsTrue(mapGeom.GetSpatialReference().GetID() == 4326);
			// showProjectedGeometryInfo(mapGeom);
			try
			{
				com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserInvalidWKID);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue("Should not throw for invalid wkid", false);
			}
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
			com.epl.geometry.SpatialReference spatialReference = com.epl.geometry.SpatialReference.Create(4326);
			try
			{
				string jSonStr = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReference, pg);
				org.codehaus.jackson.JsonFactory jf = new org.codehaus.jackson.JsonFactory();
				org.codehaus.jackson.JsonParser jp = jf.CreateJsonParser(jSonStr);
				jp.NextToken();
				com.epl.geometry.MapGeometry mg = com.epl.geometry.GeometryEngine.JsonToGeometry(jp);
				com.epl.geometry.Geometry gm = mg.GetGeometry();
				NUnit.Framework.Assert.AreEqual(com.epl.geometry.Geometry.Type.Polygon, gm.GetType());
				NUnit.Framework.Assert.IsTrue(mg.GetSpatialReference().GetID() == 4326);
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
		[NUnit.Framework.Test]
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

		private static void ShowProjectedGeometryInfo(com.epl.geometry.MapGeometry mapGeom)
		{
			System.Console.Out.WriteLine("\n");
			com.epl.geometry.MapGeometry geom = mapGeom;
			// while ((geom = geomCursor.next()) != null) {
			if (geom.GetGeometry() is com.epl.geometry.Point)
			{
				com.epl.geometry.Point pnt = (com.epl.geometry.Point)geom.GetGeometry();
				System.Console.Out.WriteLine("Point(" + pnt.GetX() + " , " + pnt.GetY() + ")");
				if (geom.GetSpatialReference() == null)
				{
					System.Console.Out.WriteLine("No spatial reference");
				}
				else
				{
					System.Console.Out.WriteLine("wkid: " + geom.GetSpatialReference().GetID());
				}
			}
			else
			{
				if (geom.GetGeometry() is com.epl.geometry.MultiPoint)
				{
					com.epl.geometry.MultiPoint mp = (com.epl.geometry.MultiPoint)geom.GetGeometry();
					System.Console.Out.WriteLine("Multipoint has " + mp.GetPointCount() + " points.");
					System.Console.Out.WriteLine("wkid: " + geom.GetSpatialReference().GetID());
				}
				else
				{
					if (geom.GetGeometry() is com.epl.geometry.Polygon)
					{
						com.epl.geometry.Polygon mp = (com.epl.geometry.Polygon)geom.GetGeometry();
						System.Console.Out.WriteLine("Polygon has " + mp.GetPointCount() + " points and " + mp.GetPathCount() + " parts.");
						if (mp.GetPathCount() > 1)
						{
							System.Console.Out.WriteLine("Part start of 2nd segment : " + mp.GetPathStart(1));
							System.Console.Out.WriteLine("Part end of 2nd segment   : " + mp.GetPathEnd(1));
							System.Console.Out.WriteLine("Part size of 2nd segment  : " + mp.GetPathSize(1));
							int start = mp.GetPathStart(1);
							int end = mp.GetPathEnd(1);
							for (int i = start; i < end; i++)
							{
								com.epl.geometry.Point pp = mp.GetPoint(i);
								System.Console.Out.WriteLine("Point(" + i + ") = (" + pp.GetX() + ", " + pp.GetY() + ")");
							}
						}
						System.Console.Out.WriteLine("wkid: " + geom.GetSpatialReference().GetID());
					}
					else
					{
						if (geom.GetGeometry() is com.epl.geometry.Polyline)
						{
							com.epl.geometry.Polyline mp = (com.epl.geometry.Polyline)geom.GetGeometry();
							System.Console.Out.WriteLine("Polyline has " + mp.GetPointCount() + " points and " + mp.GetPathCount() + " parts.");
							System.Console.Out.WriteLine("Part start of 2nd segment : " + mp.GetPathStart(1));
							System.Console.Out.WriteLine("Part end of 2nd segment   : " + mp.GetPathEnd(1));
							System.Console.Out.WriteLine("Part size of 2nd segment  : " + mp.GetPathSize(1));
							int start = mp.GetPathStart(1);
							int end = mp.GetPathEnd(1);
							for (int i = start; i < end; i++)
							{
								com.epl.geometry.Point pp = mp.GetPoint(i);
								System.Console.Out.WriteLine("Point(" + i + ") = (" + pp.GetX() + ", " + pp.GetY() + ")");
							}
							System.Console.Out.WriteLine("wkid: " + geom.GetSpatialReference().GetID());
						}
					}
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestGeometryToJSON()
		{
			com.epl.geometry.Polygon geom = new com.epl.geometry.Polygon();
			geom.StartPath(new com.epl.geometry.Point(-113, 34));
			geom.LineTo(new com.epl.geometry.Point(-105, 34));
			geom.LineTo(new com.epl.geometry.Point(-108, 40));
			string outputPolygon1 = com.epl.geometry.GeometryEngine.GeometryToJson(-1, geom);
			// Test
			// WKID
			// == -1
			//System.out.println("Geom JSON STRING is" + outputPolygon1);
			string correctPolygon1 = "{\"rings\":[[[-113,34],[-105,34],[-108,40],[-113,34]]]}";
			NUnit.Framework.Assert.AreEqual(correctPolygon1, outputPolygon1);
			string outputPolygon2 = com.epl.geometry.GeometryEngine.GeometryToJson(4326, geom);
			//System.out.println("Geom JSON STRING is" + outputPolygon2);
			string correctPolygon2 = "{\"rings\":[[[-113,34],[-105,34],[-108,40],[-113,34]]],\"spatialReference\":{\"wkid\":4326}}";
			NUnit.Framework.Assert.AreEqual(correctPolygon2, outputPolygon2);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGeometryToJSONOldID()
		{
			// CR
			com.epl.geometry.Polygon geom = new com.epl.geometry.Polygon();
			geom.StartPath(new com.epl.geometry.Point(-113, 34));
			geom.LineTo(new com.epl.geometry.Point(-105, 34));
			geom.LineTo(new com.epl.geometry.Point(-108, 40));
			string outputPolygon = com.epl.geometry.GeometryEngine.GeometryToJson(com.epl.geometry.SpatialReference.Create(3857), geom);
			// Test WKID == -1
			string correctPolygon = "{\"rings\":[[[-113,34],[-105,34],[-108,40],[-113,34]]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}";
			NUnit.Framework.Assert.IsTrue(outputPolygon.Equals(correctPolygon));
			org.codehaus.jackson.JsonFactory jf = new org.codehaus.jackson.JsonFactory();
			org.codehaus.jackson.JsonParser jp = jf.CreateJsonParser(outputPolygon);
			jp.NextToken();
			com.epl.geometry.MapGeometry mg = com.epl.geometry.GeometryEngine.JsonToGeometry(jp);
			int srId = mg.GetSpatialReference().GetID();
			int srOldId = mg.GetSpatialReference().GetOldID();
			NUnit.Framework.Assert.IsTrue(mg.GetSpatialReference().GetID() == 3857);
			NUnit.Framework.Assert.IsTrue(mg.GetSpatialReference().GetLatestID() == 3857);
			NUnit.Framework.Assert.IsTrue(mg.GetSpatialReference().GetOldID() == 102100);
		}
	}
}
