

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestProximity2D
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
		public virtual void testProximity_2D_1()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorProximity2D proximityOp = (com.esri.core.geometry.OperatorProximity2D
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Proximity2D);
			com.esri.core.geometry.Point inputPoint = new com.esri.core.geometry.Point(3, 2);
			com.esri.core.geometry.Point point0 = new com.esri.core.geometry.Point(2.75, 2);
			// Point point1 = new Point(3, 2.5);
			// Point point2 = new Point(3.75, 2);
			// Point point3 = new Point(2.25, 2.5);
			// Point point4 = new Point(4, 2.25);
			// GetNearestVertices for Polygon (Native and DotNet)
			com.esri.core.geometry.Polygon polygon = MakePolygon();
			com.esri.core.geometry.Proximity2DResult[] resultArray = com.esri.core.geometry.GeometryEngine
				.getNearestVertices(polygon, inputPoint, 2.0, 8);
			NUnit.Framework.Assert.IsTrue(resultArray.Length == 8);
			double lastdistance;
			double distance;
			com.esri.core.geometry.Proximity2DResult result0 = resultArray[0];
			lastdistance = result0.getDistance();
			NUnit.Framework.Assert.IsTrue(lastdistance <= 2.0);
			com.esri.core.geometry.Proximity2DResult result1 = resultArray[1];
			distance = result1.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			lastdistance = distance;
			com.esri.core.geometry.Proximity2DResult result2 = resultArray[2];
			distance = result2.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			lastdistance = distance;
			com.esri.core.geometry.Proximity2DResult result3 = resultArray[3];
			distance = result3.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			lastdistance = distance;
			com.esri.core.geometry.Proximity2DResult result4 = resultArray[4];
			distance = result4.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			lastdistance = distance;
			com.esri.core.geometry.Proximity2DResult result5 = resultArray[5];
			distance = result5.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			lastdistance = distance;
			com.esri.core.geometry.Proximity2DResult result6 = resultArray[6];
			distance = result6.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			lastdistance = distance;
			com.esri.core.geometry.Proximity2DResult result7 = resultArray[7];
			distance = result7.getDistance();
			NUnit.Framework.Assert.IsTrue(distance <= 2.0 && distance >= lastdistance);
			// lastdistance = distance;
			// Point[] coordinates = polygon.get.getCoordinates2D();
			// int pointCount = polygon.getPointCount();
			//
			// int hits = 0;
			// for (int i = 0; i < pointCount; i++)
			// {
			// Point ipoint = coordinates[i];
			// distance = Point::Distance(ipoint, inputPoint);
			//
			// if (distance < lastdistance)
			// hits++;
			// }
			// assertTrue(hits < 8);
			// GetNearestVertices for Point
			com.esri.core.geometry.Point point = MakePoint();
			resultArray = com.esri.core.geometry.GeometryEngine.getNearestVertices(point, inputPoint
				, 1.0, 1);
			NUnit.Framework.Assert.IsTrue(resultArray.Length == 1);
			result0 = resultArray[0];
			com.esri.core.geometry.Point resultPoint0 = result0.getCoordinate();
			NUnit.Framework.Assert.IsTrue(resultPoint0.getX() == point.getX() && resultPoint0
				.getY() == point.getY());
			// GetNearestVertex for Polygon
			result0 = com.esri.core.geometry.GeometryEngine.getNearestVertex(polygon, inputPoint
				);
			resultPoint0 = result0.getCoordinate();
			NUnit.Framework.Assert.IsTrue(resultPoint0.getX() == point0.getX() && resultPoint0
				.getY() == point0.getY());
			// GetNearestVertex for Point
			result0 = com.esri.core.geometry.GeometryEngine.getNearestVertex(point, inputPoint
				);
			resultPoint0 = result0.getCoordinate();
			NUnit.Framework.Assert.IsTrue(resultPoint0.getX() == point.getX() && resultPoint0
				.getY() == point.getY());
			// GetNearestCoordinate for Polygon
			com.esri.core.geometry.Polygon polygon2 = MakePolygon2();
			result0 = com.esri.core.geometry.GeometryEngine.getNearestCoordinate(polygon2, inputPoint
				, true);
			resultPoint0 = result0.getCoordinate();
			NUnit.Framework.Assert.IsTrue(resultPoint0.getX() == inputPoint.getX() && resultPoint0
				.getY() == inputPoint.getY());
			// GetNearestCoordinate for Polyline
			com.esri.core.geometry.Polyline polyline = MakePolyline();
			result0 = com.esri.core.geometry.GeometryEngine.getNearestCoordinate(polyline, inputPoint
				, true);
			resultPoint0 = result0.getCoordinate();
			NUnit.Framework.Assert.IsTrue(resultPoint0.getX() == 0.0 && resultPoint0.getY() ==
				 2.0);
			com.esri.core.geometry.Polygon pp = new com.esri.core.geometry.Polygon();
			pp.startPath(0, 0);
			pp.lineTo(0, 10);
			pp.lineTo(10, 10);
			pp.lineTo(10, 0);
			inputPoint.setXY(15, -5);
			result0 = proximityOp.getNearestCoordinate(pp, inputPoint, true, true);
			bool is_right = result0.isRightSide();
			NUnit.Framework.Assert.IsTrue(!is_right);
		}

		internal virtual com.esri.core.geometry.Polygon MakePolygon()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(3, -2);
			poly.lineTo(2, -1);
			poly.lineTo(3, 0);
			poly.lineTo(4, 0);
			poly.startPath(1.75, 1);
			poly.lineTo(0.75, 2);
			poly.lineTo(1.75, 3);
			poly.lineTo(2.25, 2.5);
			poly.lineTo(2.75, 2);
			poly.startPath(3, 2.5);
			poly.lineTo(2.5, 3);
			poly.lineTo(2, 3.5);
			poly.lineTo(3, 4.5);
			poly.lineTo(4, 3.5);
			poly.startPath(4.75, 1);
			poly.lineTo(3.75, 2);
			poly.lineTo(4, 2.25);
			poly.lineTo(4.75, 3);
			poly.lineTo(5.75, 2);
			return poly;
		}

		internal virtual com.esri.core.geometry.Polygon MakePolygon2()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			return poly;
		}

		internal virtual com.esri.core.geometry.Polyline MakePolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			return poly;
		}

		internal virtual com.esri.core.geometry.Point MakePoint()
		{
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point(3, 2.5);
			return point;
		}

		[NUnit.Framework.Test]
		public virtual void testProximity2D_2()
		{
			com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point(3, 2);
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point(2, 4);
			com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope();
			envelope.setCoords(4, 3, 7, 6);
			com.esri.core.geometry.Polygon polygonToTest = new com.esri.core.geometry.Polygon
				();
			polygonToTest.addEnvelope(envelope, false);
			com.esri.core.geometry.Proximity2DResult prxResult1 = com.esri.core.geometry.GeometryEngine
				.getNearestVertex(envelope, point1);
			com.esri.core.geometry.Proximity2DResult prxResult2 = com.esri.core.geometry.GeometryEngine
				.getNearestVertex(polygonToTest, point1);
			com.esri.core.geometry.Proximity2DResult prxResult3 = com.esri.core.geometry.GeometryEngine
				.getNearestCoordinate(envelope, point2, false);
			com.esri.core.geometry.Proximity2DResult prxResult4 = com.esri.core.geometry.GeometryEngine
				.getNearestCoordinate(polygonToTest, point2, false);
			com.esri.core.geometry.Point result1 = prxResult1.getCoordinate();
			com.esri.core.geometry.Point result2 = prxResult2.getCoordinate();
			NUnit.Framework.Assert.IsTrue(result1.getX() == result2.getX());
			com.esri.core.geometry.Point result3 = prxResult3.getCoordinate();
			com.esri.core.geometry.Point result4 = prxResult4.getCoordinate();
			NUnit.Framework.Assert.IsTrue(result3.getX() == result4.getX());
		}

		[NUnit.Framework.Test]
		public static void testProximity2D_3()
		{
			com.esri.core.geometry.OperatorFactoryLocal factory = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorProximity2D proximity = (com.esri.core.geometry.OperatorProximity2D
				)factory.getOperator(com.esri.core.geometry.Operator.Type.Proximity2D);
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			polygon.startPath(new com.esri.core.geometry.Point(-120, 22));
			polygon.lineTo(new com.esri.core.geometry.Point(-120, 10));
			polygon.lineTo(new com.esri.core.geometry.Point(-110, 10));
			polygon.lineTo(new com.esri.core.geometry.Point(-110, 22));
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			point.setXY(-110, 20);
			com.esri.core.geometry.Proximity2DResult result = proximity.getNearestCoordinate(
				polygon, point, false);
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			point2.setXY(-120, 12);
			com.esri.core.geometry.Proximity2DResult[] results = proximity.getNearestVertices
				(polygon, point2, 10, 12);
		}

		[NUnit.Framework.Test]
		public static void testCR254240()
		{
			com.esri.core.geometry.OperatorProximity2D proximityOp = com.esri.core.geometry.OperatorProximity2D
				.local();
			com.esri.core.geometry.Point inputPoint = new com.esri.core.geometry.Point(-12, 12
				);
			com.esri.core.geometry.Polyline line = new com.esri.core.geometry.Polyline();
			line.startPath(-10, 0);
			line.lineTo(0, 0);
			com.esri.core.geometry.Proximity2DResult result = proximityOp.getNearestCoordinate
				(line, inputPoint, false, true);
			NUnit.Framework.Assert.IsTrue(result.isRightSide() == false);
		}
	}
}
