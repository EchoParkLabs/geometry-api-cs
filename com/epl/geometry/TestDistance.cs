

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestDistance
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

		[NUnit.Framework.Test]
		public static void testDistanceBetweenVariousGeometries()
		{
			com.esri.core.geometry.Polygon polygon = makePolygon();
			com.esri.core.geometry.Polyline polyline = makePolyline();
			com.esri.core.geometry.MultiPoint multipoint = makeMultiPoint();
			com.esri.core.geometry.Point point = makePoint();
			// SpatialReference spatialRef =
			// SpatialReference.create(3857);//PCS_WGS_1984_WEB_MERCATOR_AUXSPHERE
			double distance;
			distance = com.esri.core.geometry.GeometryEngine.distance(polygon, polyline, null
				);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(distance - 5.0) < 0.00001);
			distance = com.esri.core.geometry.GeometryEngine.distance(polygon, multipoint, null
				);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(distance - 5.0) < 0.00001);
			distance = com.esri.core.geometry.GeometryEngine.distance(polygon, point, null);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(distance - 5.0) < 0.00001);
		}

		[NUnit.Framework.Test]
		public static void testDistanceBetweenTriangles()
		{
			double distance;
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			poly.startPath(0.0, 0.0);
			poly.lineTo(1.0, 2.0);
			poly.lineTo(0.0, 2.0);
			double xSeparation = 0.1;
			double ySeparation = 0.1;
			poly2.startPath(xSeparation + 1.0, 2.0 - ySeparation);
			poly2.lineTo(xSeparation + 2.0, 2.0 - ySeparation);
			poly2.lineTo(xSeparation + 2.0, 4.0 - ySeparation);
			distance = com.esri.core.geometry.GeometryEngine.distance(poly, poly2, null);
			NUnit.Framework.Assert.IsTrue(0.0 < distance && distance < xSeparation + ySeparation
				);
		}

		[NUnit.Framework.Test]
		public static void testDistanceBetweenPointAndEnvelope()
		{
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(23, 23, 
				23, 23);
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point(30, 30);
			double dist = com.esri.core.geometry.GeometryEngine.distance(env, pt, null);
			// expect just under 10.
			NUnit.Framework.Assert.IsTrue(System.Math.abs(dist - 9.8994949) < 0.0001);
		}

		[NUnit.Framework.Test]
		public static void testDistanceBetweenHugeGeometries()
		{
			/* const */
			int N = 1000;
			// Should be even
			/* const */
			double theoreticalDistance = 0.77;
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			double theta = 0.0;
			double thetaPlusPi = System.Math.PI;
			double dTheta = 2.0 * System.Math.PI / N;
			double distance;
			poly.startPath(System.Math.cos(theta), System.Math.sin(theta));
			// Add something so that poly2's bounding box is in poly's. Deleting
			// this should not affect answer.
			poly.lineTo(1.0, 1.5 + theoreticalDistance);
			poly.lineTo(3.5 + theoreticalDistance, 1.5 + theoreticalDistance);
			poly.lineTo(3.5 + theoreticalDistance, 2.0 + theoreticalDistance);
			poly.lineTo(0.95, 2.0 + theoreticalDistance);
			// ///////////////////////////////////////////////////////////
			poly2.startPath(2.0 + theoreticalDistance + System.Math.cos(thetaPlusPi), System.Math
				.sin(thetaPlusPi));
			for (double i = 1; i < N; i++)
			{
				theta += dTheta;
				thetaPlusPi += dTheta;
				poly.lineTo(System.Math.cos(theta), System.Math.sin(theta));
				poly2.lineTo(2.0 + theoreticalDistance + System.Math.cos(thetaPlusPi), System.Math
					.sin(thetaPlusPi));
			}
			distance = com.esri.core.geometry.GeometryEngine.distance(poly, poly2, null);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(distance - theoreticalDistance) < 1.0e-10
				);
		}

		private static com.esri.core.geometry.Polygon makePolygon()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.startPath(3, 3);
			poly.lineTo(7, 3);
			poly.lineTo(7, 7);
			poly.lineTo(3, 7);
			return poly;
		}

		private static com.esri.core.geometry.Polyline makePolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 15);
			poly.lineTo(15, 15);
			return poly;
		}

		private static com.esri.core.geometry.MultiPoint makeMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			mpoint.add(0, 30);
			mpoint.add(15, 15);
			mpoint.add(0, 15);
			return mpoint;
		}

		private static com.esri.core.geometry.Point makePoint()
		{
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point2D pt = new com.esri.core.geometry.Point2D();
			pt.setCoords(0, 15);
			point.setXY(pt);
			return point;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public static void testDistanceWithNullSpatialReference()
		{
			// There was a bug that distance op did not work with null Spatial
			// Reference.
			string str1 = "{\"paths\":[[[-117.138791850991,34.017492675023],[-117.138762336971,34.0174925550462]]]}";
			string str2 = "{\"paths\":[[[-117.138867827972,34.0174854109623],[-117.138850197027,34.0174929160126],[-117.138791850991,34.017492675023]]]}";
			org.codehaus.jackson.JsonFactory jsonFactory = new org.codehaus.jackson.JsonFactory
				();
			org.codehaus.jackson.JsonParser jsonParser1 = jsonFactory.createJsonParser(str1);
			org.codehaus.jackson.JsonParser jsonParser2 = jsonFactory.createJsonParser(str2);
			com.esri.core.geometry.MapGeometry geom1 = com.esri.core.geometry.GeometryEngine.
				jsonToGeometry(jsonParser1);
			com.esri.core.geometry.MapGeometry geom2 = com.esri.core.geometry.GeometryEngine.
				jsonToGeometry(jsonParser2);
			double distance = com.esri.core.geometry.GeometryEngine.distance(geom1.getGeometry
				(), geom2.getGeometry(), null);
			NUnit.Framework.Assert.IsTrue(distance == 0);
		}
	}
}
