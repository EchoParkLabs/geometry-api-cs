using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestGeomToJSonExportSRFromWkiOrWkt_CR181369 : NUnit.Framework.TestFixtureAttribute
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

		internal org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory();

		internal com.epl.geometry.SpatialReference spatialReferenceWebMerc1 = com.epl.geometry.SpatialReference.Create(102100);

		internal com.epl.geometry.SpatialReference spatialReferenceWebMerc2 = com.epl.geometry.SpatialReference.Create(spatialReferenceWebMerc1.GetLatestID());

		internal com.epl.geometry.SpatialReference spatialReferenceWGS84 = com.epl.geometry.SpatialReference.Create(4326);

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestLocalExport()
		{
			string s = com.epl.geometry.OperatorExportToJson.Local().Execute(null, new com.epl.geometry.Point(1000000.2, 2000000.3));
			//assertTrue(s.contains("."));
			//assertFalse(s.contains(","));
			com.epl.geometry.Polyline line = new com.epl.geometry.Polyline();
			line.StartPath(1.1, 2.2);
			line.LineTo(2.3, 4.5);
			string s1 = com.epl.geometry.OperatorExportToJson.Local().Execute(null, line);
			NUnit.Framework.Assert.IsTrue(s.Contains("."));
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool TestPoint()
		{
			bool bAnswer = true;
			com.epl.geometry.Point point1 = new com.epl.geometry.Point(10.0, 20.0);
			com.epl.geometry.Point pointEmpty = new com.epl.geometry.Point();
			{
				org.codehaus.jackson.JsonParser pointWebMerc1Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, point1));
				com.epl.geometry.MapGeometry pointWebMerc1MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc1Parser);
				NUnit.Framework.Assert.IsTrue(point1.GetX() == ((com.epl.geometry.Point)pointWebMerc1MP.GetGeometry()).GetX());
				NUnit.Framework.Assert.IsTrue(point1.GetY() == ((com.epl.geometry.Point)pointWebMerc1MP.GetGeometry()).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWebMerc1.GetID() == pointWebMerc1MP.GetSpatialReference().GetID() || pointWebMerc1MP.GetSpatialReference().GetID() == 3857);
				if (!CheckResultSpatialRef(pointWebMerc1MP, 102100, 3857))
				{
					bAnswer = false;
				}
				pointWebMerc1Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(null, point1));
				pointWebMerc1MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc1Parser);
				NUnit.Framework.Assert.IsTrue(null == pointWebMerc1MP.GetSpatialReference());
				if (pointWebMerc1MP.GetSpatialReference() != null)
				{
					if (!CheckResultSpatialRef(pointWebMerc1MP, 102100, 3857))
					{
						bAnswer = false;
					}
				}
				string pointEmptyString = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, pointEmpty);
				pointWebMerc1Parser = factory.CreateJsonParser(pointEmptyString);
			}
			org.codehaus.jackson.JsonParser pointWebMerc2Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc2, point1));
			com.epl.geometry.MapGeometry pointWebMerc2MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWebMerc2Parser);
			NUnit.Framework.Assert.IsTrue(point1.GetX() == ((com.epl.geometry.Point)pointWebMerc2MP.GetGeometry()).GetX());
			NUnit.Framework.Assert.IsTrue(point1.GetY() == ((com.epl.geometry.Point)pointWebMerc2MP.GetGeometry()).GetY());
			NUnit.Framework.Assert.IsTrue(spatialReferenceWebMerc2.GetLatestID() == pointWebMerc2MP.GetSpatialReference().GetLatestID());
			if (!CheckResultSpatialRef(pointWebMerc2MP, spatialReferenceWebMerc2.GetLatestID(), 0))
			{
				bAnswer = false;
			}
			{
				org.codehaus.jackson.JsonParser pointWgs84Parser = factory.CreateJsonParser(com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, point1));
				com.epl.geometry.MapGeometry pointWgs84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(pointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(point1.GetX() == ((com.epl.geometry.Point)pointWgs84MP.GetGeometry()).GetX());
				NUnit.Framework.Assert.IsTrue(point1.GetY() == ((com.epl.geometry.Point)pointWgs84MP.GetGeometry()).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == pointWgs84MP.GetSpatialReference().GetID());
				if (!CheckResultSpatialRef(pointWgs84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.epl.geometry.Point p = new com.epl.geometry.Point();
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"x\":null,\"y\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"x\":null,\"y\":null,\"z\":null,\"m\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
			}
			{
				com.epl.geometry.Point p = new com.epl.geometry.Point(10.0, 20.0, 30.0);
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"x\":10,\"y\":20,\"z\":30,\"m\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
			}
			{
				// import
				string s = "{\"x\":0.0,\"y\":1.0,\"z\":5.0,\"m\":11.0,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}";
				org.codehaus.jackson.JsonParser parser = factory.CreateJsonParser(s);
				com.epl.geometry.MapGeometry map_pt = com.epl.geometry.GeometryEngine.JsonToGeometry(parser);
				com.epl.geometry.Point pt = (com.epl.geometry.Point)map_pt.GetGeometry();
				NUnit.Framework.Assert.IsTrue(pt.GetX() == 0.0);
				NUnit.Framework.Assert.IsTrue(pt.GetY() == 1.0);
				NUnit.Framework.Assert.IsTrue(pt.GetZ() == 5.0);
				NUnit.Framework.Assert.IsTrue(pt.GetM() == 11.0);
			}
			{
				string s = "{\"x\" : 5.0, \"y\" : null, \"spatialReference\" : {\"wkid\" : 4326}} ";
				org.codehaus.jackson.JsonParser parser = factory.CreateJsonParser(s);
				com.epl.geometry.MapGeometry map_pt = com.epl.geometry.GeometryEngine.JsonToGeometry(parser);
				com.epl.geometry.Point pt = (com.epl.geometry.Point)map_pt.GetGeometry();
				NUnit.Framework.Assert.IsTrue(pt.IsEmpty());
				com.epl.geometry.SpatialReference spatial_reference = map_pt.GetSpatialReference();
				NUnit.Framework.Assert.IsTrue(spatial_reference.GetID() == 4326);
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool TestMultiPoint()
		{
			bool bAnswer = true;
			com.epl.geometry.MultiPoint multiPoint1 = new com.epl.geometry.MultiPoint();
			multiPoint1.Add(-97.06138, 32.837);
			multiPoint1.Add(-97.06133, 32.836);
			multiPoint1.Add(-97.06124, 32.834);
			multiPoint1.Add(-97.06127, 32.832);
			{
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWGS84, multiPoint1);
				org.codehaus.jackson.JsonParser mPointWgs84Parser = factory.CreateJsonParser(s);
				com.epl.geometry.MapGeometry mPointWgs84MP = com.epl.geometry.GeometryEngine.JsonToGeometry(mPointWgs84Parser);
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPointCount() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPointCount());
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(0).GetX() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(0).GetX());
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(0).GetY() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(0).GetY());
				int lastIndex = multiPoint1.GetPointCount() - 1;
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(lastIndex).GetX() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(lastIndex).GetX());
				NUnit.Framework.Assert.IsTrue(multiPoint1.GetPoint(lastIndex).GetY() == ((com.epl.geometry.MultiPoint)mPointWgs84MP.GetGeometry()).GetPoint(lastIndex).GetY());
				NUnit.Framework.Assert.IsTrue(spatialReferenceWGS84.GetID() == mPointWgs84MP.GetSpatialReference().GetID());
				if (!CheckResultSpatialRef(mPointWgs84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.epl.geometry.MultiPoint p = new com.epl.geometry.MultiPoint();
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"points\":[],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
				p.Add(10.0, 20.0, 30.0);
				p.Add(20.0, 40.0, 60.0);
				s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"points\":[[10,20,30,null],[20,40,60,null]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
			}
			{
				string points = "{\"hasM\" : false, \"hasZ\" : true, \"uncle remus\" : null, \"points\" : [ [0,0,1], [0.0,10.0,1], [10.0,10.0,1], [10.0,0.0,1, 6666] ],\"spatialReference\" : {\"wkid\" : 4326}}";
				com.epl.geometry.MapGeometry mp = com.epl.geometry.GeometryEngine.JsonToGeometry(factory.CreateJsonParser(points));
				com.epl.geometry.MultiPoint multipoint = (com.epl.geometry.MultiPoint)mp.GetGeometry();
				NUnit.Framework.Assert.IsTrue(multipoint.GetPointCount() == 4);
				com.epl.geometry.Point2D point2d;
				point2d = multipoint.GetXY(0);
				NUnit.Framework.Assert.IsTrue(point2d.x == 0.0 && point2d.y == 0.0);
				point2d = multipoint.GetXY(1);
				NUnit.Framework.Assert.IsTrue(point2d.x == 0.0 && point2d.y == 10.0);
				point2d = multipoint.GetXY(2);
				NUnit.Framework.Assert.IsTrue(point2d.x == 10.0 && point2d.y == 10.0);
				point2d = multipoint.GetXY(3);
				NUnit.Framework.Assert.IsTrue(point2d.x == 10.0 && point2d.y == 0.0);
				NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
				NUnit.Framework.Assert.IsTrue(!multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
				double z = multipoint.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0);
				NUnit.Framework.Assert.IsTrue(z == 1);
				com.epl.geometry.SpatialReference spatial_reference = mp.GetSpatialReference();
				NUnit.Framework.Assert.IsTrue(spatial_reference.GetID() == 4326);
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool TestPolyline()
		{
			bool bAnswer = true;
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
				if (!CheckResultSpatialRef(mPolylineWGS84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.epl.geometry.Polyline p = new com.epl.geometry.Polyline();
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"paths\":[],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
				p.StartPath(0, 0);
				p.LineTo(0, 1);
				p.StartPath(2, 2);
				p.LineTo(3, 3);
				p.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 3);
				p.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 5);
				s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"paths\":[[[0,0,3,null],[0,1,0,5]],[[2,2,0,null],[3,3,0,null]]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
			}
			{
				string paths = "{\"hasZ\" : true, \"paths\" : [ [ [0.0, 0.0,3], [0, 10.0,3], [10.0, 10.0,3, 6666], [10.0, 0.0,3, 6666] ], [ [1.0, 1,3], [1.0, 9.0,3], [9.0, 9.0,3], [1.0, 9.0,3] ] ], \"spatialReference\" : {\"wkid\" : 4326}, \"hasM\" : false}";
				com.epl.geometry.MapGeometry mapGeometry = com.epl.geometry.GeometryEngine.JsonToGeometry(factory.CreateJsonParser(paths));
				com.epl.geometry.Polyline p = (com.epl.geometry.Polyline)mapGeometry.GetGeometry();
				NUnit.Framework.Assert.IsTrue(p.GetPathCount() == 2);
				int count = p.GetPathCount();
				NUnit.Framework.Assert.IsTrue(p.GetPointCount() == 8);
				NUnit.Framework.Assert.IsTrue(p.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
				NUnit.Framework.Assert.IsTrue(!p.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
				double z = p.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0);
				NUnit.Framework.Assert.IsTrue(z == 3);
				double length = p.CalculateLength2D();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 54.0) <= 0.001);
				com.epl.geometry.SpatialReference spatial_reference = mapGeometry.GetSpatialReference();
				NUnit.Framework.Assert.IsTrue(spatial_reference.GetID() == 4326);
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool TestPolygon()
		{
			bool bAnswer = true;
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
				if (!CheckResultSpatialRef(mPolygonWGS84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				p.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"rings\":[],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
				p.StartPath(0, 0);
				p.LineTo(0, 1);
				p.LineTo(4, 4);
				p.StartPath(2, 2);
				p.LineTo(3, 3);
				p.LineTo(7, 8);
				p.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 3);
				p.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 7);
				p.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 5);
				p.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 5, 0, 5);
				s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, p);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"hasZ\":true,\"hasM\":true,\"rings\":[[[0,0,3,null],[0,1,0,7],[4,4,0,5],[0,0,3,null]],[[2,2,0,null],[3,3,0,null],[7,8,0,5],[2,2,0,null]]],\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
			}
			{
				// Test Import Polygon from Polygon
				string rings = "{\"hasZ\": true, \"rings\" : [ [ [0,0, 5], [0.0, 10.0, 5], [10.0,10.0, 5, 66666], [10.0,0.0, 5] ], [ [12, 12] ],  [ [13 , 17], [13 , 17] ], [ [1.0, 1.0, 5, 66666], [9.0,1.0, 5], [9.0,9.0, 5], [1.0,9.0, 5], [1.0, 1.0, 5] ] ] }";
				com.epl.geometry.MapGeometry mapGeometry = com.epl.geometry.GeometryEngine.JsonToGeometry(factory.CreateJsonParser(rings));
				com.epl.geometry.Polygon p = (com.epl.geometry.Polygon)mapGeometry.GetGeometry();
				double area = p.CalculateArea2D();
				double length = p.CalculateLength2D();
				NUnit.Framework.Assert.IsTrue(p.GetPathCount() == 4);
				int count = p.GetPointCount();
				NUnit.Framework.Assert.IsTrue(count == 15);
				NUnit.Framework.Assert.IsTrue(p.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
				NUnit.Framework.Assert.IsTrue(!p.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool TestEnvelope()
		{
			bool bAnswer = true;
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
				if (!CheckResultSpatialRef(envelopeWGS84MP, 4326, 0))
				{
					bAnswer = false;
				}
			}
			{
				// export
				com.epl.geometry.Envelope e = new com.epl.geometry.Envelope();
				e.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				e.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				string s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, e);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"xmin\":null,\"ymin\":null,\"xmax\":null,\"ymax\":null,\"zmin\":null,\"zmax\":null,\"mmin\":null,\"mmax\":null,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
				e.SetCoords(0, 1, 2, 3);
				com.epl.geometry.Envelope1D z = new com.epl.geometry.Envelope1D();
				com.epl.geometry.Envelope1D m = new com.epl.geometry.Envelope1D();
				z.SetCoords(5, 7);
				m.SetCoords(11, 13);
				e.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, z);
				e.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, m);
				s = com.epl.geometry.GeometryEngine.GeometryToJson(spatialReferenceWebMerc1, e);
				NUnit.Framework.Assert.IsTrue(s.Equals("{\"xmin\":0,\"ymin\":1,\"xmax\":2,\"ymax\":3,\"zmin\":5,\"zmax\":7,\"mmin\":11,\"mmax\":13,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}"));
			}
			{
				// import
				string s = "{\"xmin\":0.0,\"ymin\":1.0,\"xmax\":2.0,\"ymax\":3.0,\"zmin\":5.0,\"zmax\":7.0,\"mmin\":11.0,\"mmax\":13.0,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}";
				org.codehaus.jackson.JsonParser parser = factory.CreateJsonParser(s);
				com.epl.geometry.MapGeometry map_env = com.epl.geometry.GeometryEngine.JsonToGeometry(parser);
				com.epl.geometry.Envelope env = (com.epl.geometry.Envelope)map_env.GetGeometry();
				com.epl.geometry.Envelope1D z = env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
				com.epl.geometry.Envelope1D m = env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
				NUnit.Framework.Assert.IsTrue(z.vmin == 5.0);
				NUnit.Framework.Assert.IsTrue(z.vmax == 7.0);
				NUnit.Framework.Assert.IsTrue(m.vmin == 11.0);
				NUnit.Framework.Assert.IsTrue(m.vmax == 13.0);
			}
			{
				string s = "{ \"zmin\" : 33, \"xmin\" : -109.55, \"zmax\" : 53, \"ymin\" : 25.76, \"xmax\" : -86.39, \"ymax\" : 49.94, \"mmax\" : 13}";
				org.codehaus.jackson.JsonParser parser = factory.CreateJsonParser(s);
				com.epl.geometry.MapGeometry map_env = com.epl.geometry.GeometryEngine.JsonToGeometry(parser);
				com.epl.geometry.Envelope env = (com.epl.geometry.Envelope)map_env.GetGeometry();
				com.epl.geometry.Envelope2D e = new com.epl.geometry.Envelope2D();
				env.QueryEnvelope2D(e);
				NUnit.Framework.Assert.IsTrue(e.xmin == -109.55 && e.ymin == 25.76 && e.xmax == -86.39 && e.ymax == 49.94);
				com.epl.geometry.Envelope1D e1D;
				NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
				e1D = env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
				NUnit.Framework.Assert.IsTrue(e1D.vmin == 33 && e1D.vmax == 53);
				NUnit.Framework.Assert.IsTrue(!env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			}
			return bAnswer;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		internal virtual bool TestCR181369()
		{
			// CR181369
			bool bAnswer = true;
			string jsonStringPointAndWKT = "{\"x\":10.0,\"y\":20.0,\"spatialReference\":{\"wkt\" : \"PROJCS[\\\"NAD83_UTM_zone_15N\\\",GEOGCS[\\\"GCS_North_American_1983\\\",DATUM[\\\"D_North_American_1983\\\",SPHEROID[\\\"GRS_1980\\\",6378137.0,298.257222101]],PRIMEM[\\\"Greenwich\\\",0.0],UNIT[\\\"Degree\\\",0.0174532925199433]],PROJECTION[\\\"Transverse_Mercator\\\"],PARAMETER[\\\"false_easting\\\",500000.0],PARAMETER[\\\"false_northing\\\",0.0],PARAMETER[\\\"central_meridian\\\",-93.0],PARAMETER[\\\"scale_factor\\\",0.9996],PARAMETER[\\\"latitude_of_origin\\\",0.0],UNIT[\\\"Meter\\\",1.0]]\"} }";
			org.codehaus.jackson.JsonParser jsonParserPointAndWKT = factory.CreateJsonParser(jsonStringPointAndWKT);
			com.epl.geometry.MapGeometry mapGeom2 = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPointAndWKT);
			string jsonStringPointAndWKT2 = com.epl.geometry.GeometryEngine.GeometryToJson(mapGeom2.GetSpatialReference(), mapGeom2.GetGeometry());
			org.codehaus.jackson.JsonParser jsonParserPointAndWKT2 = factory.CreateJsonParser(jsonStringPointAndWKT2);
			com.epl.geometry.MapGeometry mapGeom3 = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParserPointAndWKT2);
			NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Point)mapGeom2.GetGeometry()).GetX() == ((com.epl.geometry.Point)mapGeom3.GetGeometry()).GetX());
			NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Point)mapGeom2.GetGeometry()).GetY() == ((com.epl.geometry.Point)mapGeom3.GetGeometry()).GetY());
			string s1 = mapGeom2.GetSpatialReference().GetText();
			string s2 = mapGeom3.GetSpatialReference().GetText();
			NUnit.Framework.Assert.IsTrue(s1.Equals(s2));
			int id2 = mapGeom2.GetSpatialReference().GetID();
			int id3 = mapGeom3.GetSpatialReference().GetID();
			NUnit.Framework.Assert.IsTrue(id2 == id3);
			if (!CheckResultSpatialRef(mapGeom3, mapGeom2.GetSpatialReference().GetID(), 0))
			{
				bAnswer = false;
			}
			return bAnswer;
		}

		internal virtual bool CheckResultSpatialRef(com.epl.geometry.MapGeometry mapGeometry, int expectWki1, int expectWki2)
		{
			com.epl.geometry.SpatialReference sr = mapGeometry.GetSpatialReference();
			string Wkt = sr.GetText();
			int wki1 = sr.GetLatestID();
			if (!(wki1 == expectWki1 || wki1 == expectWki2))
			{
				return false;
			}
			if (!(Wkt != null && Wkt.Length > 0))
			{
				return false;
			}
			com.epl.geometry.SpatialReference sr2 = com.epl.geometry.SpatialReference.Create(Wkt);
			int wki2 = sr2.GetID();
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
