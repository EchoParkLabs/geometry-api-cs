using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestDifference : NUnit.Framework.TestFixtureAttribute
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
		public static void TestDifferenceAndSymmetricDifference()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorDifference differenceOp = (com.epl.geometry.OperatorDifference)engine.GetOperator(com.epl.geometry.Operator.Type.Difference);
			com.epl.geometry.SpatialReference spatialRef = com.epl.geometry.SpatialReference.Create(102113);
			com.epl.geometry.Polygon polygon1 = MakePolygon1();
			com.epl.geometry.Polygon polygon2 = MakePolygon2();
			com.epl.geometry.Polyline polyline1 = MakePolyline1();
			com.epl.geometry.MultiPoint multipoint1 = MakeMultiPoint1();
			com.epl.geometry.MultiPoint multipoint2 = MakeMultiPoint2();
			com.epl.geometry.MultiPoint multipoint3 = MakeMultiPoint3();
			com.epl.geometry.Point point1 = MakePoint1();
			com.epl.geometry.Point point2 = MakePoint2();
			com.epl.geometry.Envelope envelope1 = MakeEnvelope1();
			com.epl.geometry.Envelope envelope2 = MakeEnvelope2();
			com.epl.geometry.Envelope envelope3 = MakeEnvelope3();
			com.epl.geometry.Polygon outputPolygon = (com.epl.geometry.Polygon)differenceOp.Execute(polygon1, polygon2, spatialRef, null);
			double area = outputPolygon.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 75) <= 0.001);
			{
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point(-130, 10);
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point(-130, 10);
				com.epl.geometry.Geometry baseGeom = new com.epl.geometry.Point(point_1.GetX(), point_1.GetY());
				com.epl.geometry.Geometry comparisonGeom = new com.epl.geometry.Point(point_2.GetX(), point2.GetY());
				com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
				com.epl.geometry.Geometry geom = differenceOp.Execute(baseGeom, comparisonGeom, sr, null);
			}
			com.epl.geometry.OperatorSymmetricDifference symDifferenceOp = (com.epl.geometry.OperatorSymmetricDifference)engine.GetOperator(com.epl.geometry.Operator.Type.SymmetricDifference);
			outputPolygon = (com.epl.geometry.Polygon)symDifferenceOp.Execute(polygon1, polygon2, spatialRef, null);
			area = outputPolygon.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 150) <= 0.001);
			com.epl.geometry.Polyline outputPolyline = (com.epl.geometry.Polyline)differenceOp.Execute(polyline1, polygon1, spatialRef, null);
			double length = outputPolyline.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length * length - 50) < 0.001);
			com.epl.geometry.MultiPoint outputMultiPoint = (com.epl.geometry.MultiPoint)differenceOp.Execute(multipoint1, polygon1, spatialRef, null);
			int pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 1);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(symDifferenceOp.Execute(multipoint1, point1, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 2);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(symDifferenceOp.Execute(multipoint1, point2, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(differenceOp.Execute(multipoint1, point1, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 2);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(differenceOp.Execute(multipoint1, point2, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 3);
			outputPolygon = (com.epl.geometry.Polygon)(differenceOp.Execute(polygon1, envelope1, spatialRef, null));
			area = outputPolygon.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 75) <= 0.001);
			outputPolygon = (com.epl.geometry.Polygon)(differenceOp.Execute(polygon2, envelope2, spatialRef, null));
			area = outputPolygon.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 75) <= 0.001);
			outputPolyline = (com.epl.geometry.Polyline)(differenceOp.Execute(polyline1, envelope2, spatialRef, null));
			length = outputPolyline.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length * length - 50) <= 0.001);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(differenceOp.Execute(multipoint1, envelope2, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 1);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(differenceOp.Execute(multipoint2, envelope2, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 6);
			outputMultiPoint = (com.epl.geometry.MultiPoint)(differenceOp.Execute(multipoint3, envelope2, spatialRef, null));
			pointCount = outputMultiPoint.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 0);
			com.epl.geometry.Point outputPoint = (com.epl.geometry.Point)(differenceOp.Execute(point1, envelope2, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(!outputPoint.IsEmpty());
			outputPoint = (com.epl.geometry.Point)(differenceOp.Execute(point2, envelope2, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPoint.IsEmpty());
			outputPolygon = (com.epl.geometry.Polygon)(differenceOp.Execute(envelope3, envelope2, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPolygon != null && outputPolygon.IsEmpty());
			outputPolygon = (com.epl.geometry.Polygon)(symDifferenceOp.Execute(envelope3, envelope3, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPolygon != null && outputPolygon.IsEmpty());
			outputPoint = (com.epl.geometry.Point)(differenceOp.Execute(point1, polygon1, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPoint != null);
		}

		[NUnit.Framework.Test]
		public static void TestPointTypes()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorDifference difference = (com.epl.geometry.OperatorDifference)engine.GetOperator(com.epl.geometry.Operator.Type.Difference);
			com.epl.geometry.OperatorSymmetricDifference sym_difference = (com.epl.geometry.OperatorSymmetricDifference)engine.GetOperator(com.epl.geometry.Operator.Type.SymmetricDifference);
			{
				// point/point
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point();
				point_1.SetXY(0, 0);
				point_2.SetXY(0.000000009, 0.000000009);
				com.epl.geometry.Point differenced = (com.epl.geometry.Point)(difference.Execute(point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
				com.epl.geometry.MultiPoint sym_differenced = (com.epl.geometry.MultiPoint)(sym_difference.Execute(point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(sym_differenced.IsEmpty());
			}
			{
				// point/point
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point();
				point_1.SetXY(0, 0);
				point_2.SetXY(0.000000009, 0.0);
				com.epl.geometry.Point differenced = (com.epl.geometry.Point)(difference.Execute(point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
				com.epl.geometry.MultiPoint sym_differenced = (com.epl.geometry.MultiPoint)(sym_difference.Execute(point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(sym_differenced.IsEmpty());
			}
			{
				// point/point
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point();
				point_1.SetXY(0, 0);
				point_2.SetXY(0.00000002, 0.00000002);
				com.epl.geometry.Point differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				com.epl.geometry.Point differenced_2 = (com.epl.geometry.Point)(difference.Execute(point_2, point_1, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_2.IsEmpty());
				com.epl.geometry.MultiPoint sym_differenced = (com.epl.geometry.MultiPoint)(sym_difference.Execute(point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!sym_differenced.IsEmpty());
				NUnit.Framework.Assert.IsTrue(sym_differenced.GetXY(0).x == 0 && sym_differenced.GetXY(0).y == 0);
				NUnit.Framework.Assert.IsTrue(sym_differenced.GetXY(1).x == 0.00000002 && sym_differenced.GetXY(1).y == 0.00000002);
			}
			{
				// multi_point/point
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point();
				multi_point_1.Add(0, 0);
				multi_point_1.Add(1, 1);
				point_2.SetXY(0.000000009, 0.000000009);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1.GetPointCount() == 1);
				NUnit.Framework.Assert.IsTrue(differenced_1.GetXY(0).x == 1 && differenced_1.GetXY(0).y == 1);
				com.epl.geometry.Point differenced_2 = (com.epl.geometry.Point)(difference.Execute(point_2, multi_point_1, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_2.IsEmpty());
			}
			{
				// multi_point/point
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point();
				multi_point_1.Add(0, 0);
				multi_point_1.Add(1, 1);
				point_2.SetXY(0.000000009, 0.0);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1.GetXY(0).x == 1.0 && differenced_1.GetXY(0).y == 1.0);
				com.epl.geometry.Point differenced_2 = (com.epl.geometry.Point)(difference.Execute(point_2, multi_point_1, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_2.IsEmpty());
				com.epl.geometry.MultiPoint sym_differenced = (com.epl.geometry.MultiPoint)(sym_difference.Execute(multi_point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!sym_differenced.IsEmpty());
				NUnit.Framework.Assert.IsTrue(sym_differenced.GetPointCount() == 1);
				NUnit.Framework.Assert.IsTrue(sym_differenced.GetXY(0).x == 1 && sym_differenced.GetXY(0).y == 1);
			}
			{
				// multi_point/point
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Point point_2 = new com.epl.geometry.Point();
				multi_point_1.Add(0, 0);
				multi_point_1.Add(0, 0);
				point_2.SetXY(0.000000009, 0.0);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.IsEmpty());
				com.epl.geometry.MultiPoint sym_differenced = (com.epl.geometry.MultiPoint)(sym_difference.Execute(multi_point_1, point_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(sym_differenced.IsEmpty());
			}
			{
				// multi_point/polygon
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Polygon polygon_2 = new com.epl.geometry.Polygon();
				multi_point_1.Add(0, 0);
				multi_point_1.Add(0, 0);
				multi_point_1.Add(2, 2);
				polygon_2.StartPath(-1, -1);
				polygon_2.LineTo(-1, 1);
				polygon_2.LineTo(1, 1);
				polygon_2.LineTo(1, -1);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1.GetPointCount() == 1);
				NUnit.Framework.Assert.IsTrue(differenced_1.GetXY(0).x == 2 && differenced_1.GetXY(0).y == 2);
			}
			{
				// multi_point/polygon
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Polygon polygon_2 = new com.epl.geometry.Polygon();
				multi_point_1.Add(0, 0);
				multi_point_1.Add(0, 0);
				multi_point_1.Add(1, 1);
				polygon_2.StartPath(-1, -1);
				polygon_2.LineTo(-1, 1);
				polygon_2.LineTo(1, 1);
				polygon_2.LineTo(1, -1);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.IsEmpty());
			}
			{
				// multi_point/envelope
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Envelope envelope_2 = new com.epl.geometry.Envelope();
				multi_point_1.Add(-2, 0);
				multi_point_1.Add(0, 2);
				multi_point_1.Add(2, 0);
				multi_point_1.Add(0, -2);
				envelope_2.SetCoords(-1, -1, 1, 1);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, envelope_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty() && differenced_1 == multi_point_1);
			}
			{
				// multi_point/polygon
				com.epl.geometry.MultiPoint multi_point_1 = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Polygon polygon_2 = new com.epl.geometry.Polygon();
				multi_point_1.Add(2, 2);
				multi_point_1.Add(2, 2);
				multi_point_1.Add(-2, -2);
				polygon_2.StartPath(-1, -1);
				polygon_2.LineTo(-1, 1);
				polygon_2.LineTo(1, 1);
				polygon_2.LineTo(1, -1);
				com.epl.geometry.MultiPoint differenced_1 = (com.epl.geometry.MultiPoint)(difference.Execute(multi_point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty() && differenced_1 == multi_point_1);
			}
			{
				// point/polygon
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Polygon polygon_2 = new com.epl.geometry.Polygon();
				point_1.SetXY(0, 0);
				polygon_2.StartPath(-1, -1);
				polygon_2.LineTo(-1, 1);
				polygon_2.LineTo(1, 1);
				polygon_2.LineTo(1, -1);
				com.epl.geometry.Point differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.IsEmpty());
				polygon_2.SetEmpty();
				polygon_2.StartPath(1, 1);
				polygon_2.LineTo(1, 2);
				polygon_2.LineTo(2, 2);
				polygon_2.LineTo(2, 1);
				differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
			}
			{
				// point/polygon
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Polygon polygon_2 = new com.epl.geometry.Polygon();
				point_1.SetXY(0, 0);
				polygon_2.StartPath(1, 0);
				polygon_2.LineTo(0, 1);
				polygon_2.LineTo(1, 1);
				com.epl.geometry.Point differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
				point_1.SetEmpty();
				point_1.SetXY(0.5, 0.5);
				polygon_2.SetEmpty();
				polygon_2.StartPath(1, 0);
				polygon_2.LineTo(0, 1);
				polygon_2.LineTo(1, 1);
				differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polygon_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.IsEmpty());
			}
			{
				// point/envelope
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Envelope envelope_2 = new com.epl.geometry.Envelope();
				point_1.SetXY(0, 0);
				envelope_2.SetCoords(-1, -1, 1, 1);
				com.epl.geometry.Point differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, envelope_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.IsEmpty());
				envelope_2.SetEmpty();
				envelope_2.SetCoords(1, 1, 2, 2);
				differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, envelope_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
			}
			{
				// point/polyline
				com.epl.geometry.Point point_1 = new com.epl.geometry.Point();
				com.epl.geometry.Polyline polyline_2 = new com.epl.geometry.Polyline();
				point_1.SetXY(0, 0);
				polyline_2.StartPath(-1, 0);
				polyline_2.LineTo(1, 0);
				com.epl.geometry.Point differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polyline_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.IsEmpty());
				polyline_2.SetEmpty();
				polyline_2.StartPath(1, 0);
				polyline_2.LineTo(2, 0);
				differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polyline_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
				polyline_2.SetEmpty();
				polyline_2.StartPath(-1, -1);
				polyline_2.LineTo(-1, 1);
				polyline_2.LineTo(1, 1);
				polyline_2.LineTo(1, -1);
				differenced_1 = (com.epl.geometry.Point)(difference.Execute(point_1, polyline_2, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.IsEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
			}
		}

		[NUnit.Framework.Test]
		public static void TestDifferenceOnPolyline()
		{
			// # * * #
			// # * @
			// # @ *
			// # *
			//
			// ///////////////////////////////
			//
			// The polyline drawn in *s represents basePl
			// The polyline drawn in #s represents compPl
			// The @ represents their intersection points, so that
			// the difference polyline will be basePl with two new vertices @ added.
			com.epl.geometry.Polyline basePl = new com.epl.geometry.Polyline();
			basePl.StartPath(new com.epl.geometry.Point(-117, 20));
			basePl.LineTo(new com.epl.geometry.Point(-130, 10));
			basePl.LineTo(new com.epl.geometry.Point(-120, 50));
			com.epl.geometry.Polyline compPl = new com.epl.geometry.Polyline();
			compPl.StartPath(new com.epl.geometry.Point(-116, 20));
			compPl.LineTo(new com.epl.geometry.Point(-131, 10));
			compPl.LineTo(new com.epl.geometry.Point(-121, 50));
			com.epl.geometry.Geometry diffGeom = com.epl.geometry.GeometryEngine.Difference(basePl, compPl, com.epl.geometry.SpatialReference.Create(4326));
			NUnit.Framework.Assert.IsTrue(diffGeom is com.epl.geometry.Polyline);
			com.epl.geometry.Polyline diffPolyline = (com.epl.geometry.Polyline)diffGeom;
			int pointCountDiffPolyline = diffPolyline.GetPointCount();
			// first line in comp_pl is 3y = 2x + 292
			NUnit.Framework.Assert.AreEqual(3 * 20, 2 * (-116) + 292);
			NUnit.Framework.Assert.AreEqual(3 * 10, 2 * (-131) + 292);
			// new points should also lie on this line
			NUnit.Framework.Assert.IsTrue(3.0 * diffPolyline.GetCoordinates2D()[1].y - 2.0 * diffPolyline.GetCoordinates2D()[1].x - 292.0 == 0.0);
			NUnit.Framework.Assert.IsTrue(3.0 * diffPolyline.GetCoordinates2D()[3].y - 2.0 * diffPolyline.GetCoordinates2D()[3].x - 292.0 == 0.0);
			for (int i = 0; i < 3; i++)
			{
				NUnit.Framework.Assert.IsTrue(basePl.GetCoordinates2D()[i].x == diffPolyline.GetCoordinates2D()[2 * i].x);
				NUnit.Framework.Assert.IsTrue(basePl.GetCoordinates2D()[i].y == diffPolyline.GetCoordinates2D()[2 * i].y);
			}
			NUnit.Framework.Assert.AreEqual(5, pointCountDiffPolyline);
		}

		public static com.epl.geometry.Polygon MakePolygon1()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			return poly;
		}

		public static com.epl.geometry.Polygon MakePolygon2()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(5, 5);
			poly.LineTo(5, 15);
			poly.LineTo(15, 15);
			poly.LineTo(15, 5);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolyline1()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(15, 15);
			return poly;
		}

		public static com.epl.geometry.MultiPoint MakeMultiPoint1()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			pt1.x = 1.0;
			pt1.y = 1.0;
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			pt2.x = 5.0;
			pt2.y = 5.0;
			com.epl.geometry.Point2D pt3 = new com.epl.geometry.Point2D();
			pt3.x = 15.0;
			pt3.y = 15.0;
			mpoint.Add(pt1.x, pt1.y);
			mpoint.Add(pt2.x, pt2.y);
			mpoint.Add(pt3.x, pt3.y);
			return mpoint;
		}

		public static com.epl.geometry.MultiPoint MakeMultiPoint2()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			pt1.x = 1.0;
			pt1.y = 1.0;
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			pt2.x = 1.0;
			pt2.y = 1.0;
			com.epl.geometry.Point2D pt3 = new com.epl.geometry.Point2D();
			pt3.x = 15.0;
			pt3.y = 15.0;
			com.epl.geometry.Point2D pt4 = new com.epl.geometry.Point2D();
			pt4.x = 15.0;
			pt4.y = 15.0;
			com.epl.geometry.Point2D pt5 = new com.epl.geometry.Point2D();
			pt5.x = 1.0;
			pt5.y = 1.0;
			com.epl.geometry.Point2D pt6 = new com.epl.geometry.Point2D();
			pt6.x = 1.0;
			pt6.y = 1.0;
			com.epl.geometry.Point2D pt7 = new com.epl.geometry.Point2D();
			pt7.x = 15.0;
			pt7.y = 15.0;
			com.epl.geometry.Point2D pt8 = new com.epl.geometry.Point2D();
			pt8.x = 15.0;
			pt8.y = 15.0;
			com.epl.geometry.Point2D pt9 = new com.epl.geometry.Point2D();
			pt9.x = 15.0;
			pt9.y = 15.0;
			com.epl.geometry.Point2D pt10 = new com.epl.geometry.Point2D();
			pt10.x = 1.0;
			pt10.y = 1.0;
			com.epl.geometry.Point2D pt11 = new com.epl.geometry.Point2D();
			pt11.x = 15.0;
			pt11.y = 15.0;
			mpoint.Add(pt1.x, pt1.y);
			mpoint.Add(pt2.x, pt2.y);
			mpoint.Add(pt3.x, pt3.y);
			mpoint.Add(pt4.x, pt4.y);
			mpoint.Add(pt5.x, pt5.y);
			mpoint.Add(pt6.x, pt6.y);
			mpoint.Add(pt7.x, pt7.y);
			mpoint.Add(pt8.x, pt8.y);
			mpoint.Add(pt9.x, pt9.y);
			mpoint.Add(pt10.x, pt10.y);
			mpoint.Add(pt11.x, pt11.y);
			return mpoint;
		}

		public static com.epl.geometry.MultiPoint MakeMultiPoint3()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			pt1.x = 1.0;
			pt1.y = 1.0;
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			pt2.x = 5.0;
			pt2.y = 5.0;
			mpoint.Add(pt1.x, pt1.y);
			mpoint.Add(pt2.x, pt2.y);
			return mpoint;
		}

		public static com.epl.geometry.Point MakePoint1()
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.SetCoords(15, 15);
			point.SetXY(pt);
			return point;
		}

		public static com.epl.geometry.Point MakePoint2()
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.SetCoords(7, 7);
			point.SetXY(pt);
			return point;
		}

		public static com.epl.geometry.Envelope MakeEnvelope1()
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetCoords(5, 5, 15, 15);
			com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope(env);
			return envelope;
		}

		public static com.epl.geometry.Envelope MakeEnvelope2()
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetCoords(0, 0, 10, 10);
			com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope(env);
			return envelope;
		}

		public static com.epl.geometry.Envelope MakeEnvelope3()
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetCoords(5, 5, 6, 6);
			com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope(env);
			return envelope;
		}
	}
}
