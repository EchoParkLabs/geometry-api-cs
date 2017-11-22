using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestDistance : NUnit.Framework.TestFixtureAttribute
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

		[NUnit.Framework.Test]
		public static void TestDistanceBetweenVariousGeometries()
		{
			com.epl.geometry.Polygon polygon = MakePolygon();
			com.epl.geometry.Polyline polyline = MakePolyline();
			com.epl.geometry.MultiPoint multipoint = MakeMultiPoint();
			com.epl.geometry.Point point = MakePoint();
			// SpatialReference spatialRef =
			// SpatialReference.create(3857);//PCS_WGS_1984_WEB_MERCATOR_AUXSPHERE
			double distance;
			distance = com.epl.geometry.GeometryEngine.Distance(polygon, polyline, null);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(distance - 5.0) < 0.00001);
			distance = com.epl.geometry.GeometryEngine.Distance(polygon, multipoint, null);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(distance - 5.0) < 0.00001);
			distance = com.epl.geometry.GeometryEngine.Distance(polygon, point, null);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(distance - 5.0) < 0.00001);
		}

		[NUnit.Framework.Test]
		public static void TestDistanceBetweenTriangles()
		{
			double distance;
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
			poly.StartPath(0.0, 0.0);
			poly.LineTo(1.0, 2.0);
			poly.LineTo(0.0, 2.0);
			double xSeparation = 0.1;
			double ySeparation = 0.1;
			poly2.StartPath(xSeparation + 1.0, 2.0 - ySeparation);
			poly2.LineTo(xSeparation + 2.0, 2.0 - ySeparation);
			poly2.LineTo(xSeparation + 2.0, 4.0 - ySeparation);
			distance = com.epl.geometry.GeometryEngine.Distance(poly, poly2, null);
			NUnit.Framework.Assert.IsTrue(0.0 < distance && distance < xSeparation + ySeparation);
		}

		[NUnit.Framework.Test]
		public static void TestDistanceBetweenPointAndEnvelope()
		{
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(23, 23, 23, 23);
			com.epl.geometry.Point pt = new com.epl.geometry.Point(30, 30);
			double dist = com.epl.geometry.GeometryEngine.Distance(env, pt, null);
			// expect just under 10.
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(dist - 9.8994949) < 0.0001);
		}

		[NUnit.Framework.Test]
		public static void TestDistanceBetweenHugeGeometries()
		{
			/* const */
			int N = 1000;
			// Should be even
			/* const */
			double theoreticalDistance = 0.77;
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
			double theta = 0.0;
			double thetaPlusPi = System.Math.PI;
			double dTheta = 2.0 * System.Math.PI / N;
			double distance;
			poly.StartPath(System.Math.Cos(theta), System.Math.Sin(theta));
			// Add something so that poly2's bounding box is in poly's. Deleting
			// this should not affect answer.
			poly.LineTo(1.0, 1.5 + theoreticalDistance);
			poly.LineTo(3.5 + theoreticalDistance, 1.5 + theoreticalDistance);
			poly.LineTo(3.5 + theoreticalDistance, 2.0 + theoreticalDistance);
			poly.LineTo(0.95, 2.0 + theoreticalDistance);
			// ///////////////////////////////////////////////////////////
			poly2.StartPath(2.0 + theoreticalDistance + System.Math.Cos(thetaPlusPi), System.Math.Sin(thetaPlusPi));
			for (double i = 1; i < N; i++)
			{
				theta += dTheta;
				thetaPlusPi += dTheta;
				poly.LineTo(System.Math.Cos(theta), System.Math.Sin(theta));
				poly2.LineTo(2.0 + theoreticalDistance + System.Math.Cos(thetaPlusPi), System.Math.Sin(thetaPlusPi));
			}
			distance = com.epl.geometry.GeometryEngine.Distance(poly, poly2, null);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(distance - theoreticalDistance) < 1.0e-10);
		}

		private static com.epl.geometry.Polygon MakePolygon()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.StartPath(3, 3);
			poly.LineTo(7, 3);
			poly.LineTo(7, 7);
			poly.LineTo(3, 7);
			return poly;
		}

		private static com.epl.geometry.Polyline MakePolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 15);
			poly.LineTo(15, 15);
			return poly;
		}

		private static com.epl.geometry.MultiPoint MakeMultiPoint()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			mpoint.Add(0, 30);
			mpoint.Add(15, 15);
			mpoint.Add(0, 15);
			return mpoint;
		}

		private static com.epl.geometry.Point MakePoint()
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.SetCoords(0, 15);
			point.SetXY(pt);
			return point;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
//		[NUnit.Framework.Test]
//		public static void TestDistanceWithNullSpatialReference()
//		{
//			// There was a bug that distance op did not work with null Spatial
//			// Reference.
//			string str1 = "{\"paths\":[[[-117.138791850991,34.017492675023],[-117.138762336971,34.0174925550462]]]}";
//			string str2 = "{\"paths\":[[[-117.138867827972,34.0174854109623],[-117.138850197027,34.0174929160126],[-117.138791850991,34.017492675023]]]}";
//			org.codehaus.jackson.JsonFactory jsonFactory = new org.codehaus.jackson.JsonFactory();
//			org.codehaus.jackson.JsonParser jsonParser1 = jsonFactory.CreateJsonParser(str1);
//			org.codehaus.jackson.JsonParser jsonParser2 = jsonFactory.CreateJsonParser(str2);
//			com.epl.geometry.MapGeometry geom1 = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParser1);
//			com.epl.geometry.MapGeometry geom2 = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonParser2);
//			double distance = com.epl.geometry.GeometryEngine.Distance(geom1.GetGeometry(), geom2.GetGeometry(), null);
//			NUnit.Framework.Assert.IsTrue(distance == 0);
//		}
	}
}
