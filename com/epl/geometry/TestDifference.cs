

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestDifference
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
		public static void testDifferenceAndSymmetricDifference()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorDifference differenceOp = (com.esri.core.geometry.OperatorDifference
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			com.esri.core.geometry.SpatialReference spatialRef = com.esri.core.geometry.SpatialReference
				.create(102113);
			com.esri.core.geometry.Polygon polygon1 = makePolygon1();
			com.esri.core.geometry.Polygon polygon2 = makePolygon2();
			com.esri.core.geometry.Polyline polyline1 = makePolyline1();
			com.esri.core.geometry.MultiPoint multipoint1 = makeMultiPoint1();
			com.esri.core.geometry.MultiPoint multipoint2 = makeMultiPoint2();
			com.esri.core.geometry.MultiPoint multipoint3 = makeMultiPoint3();
			com.esri.core.geometry.Point point1 = makePoint1();
			com.esri.core.geometry.Point point2 = makePoint2();
			com.esri.core.geometry.Envelope envelope1 = makeEnvelope1();
			com.esri.core.geometry.Envelope envelope2 = makeEnvelope2();
			com.esri.core.geometry.Envelope envelope3 = makeEnvelope3();
			com.esri.core.geometry.Polygon outputPolygon = (com.esri.core.geometry.Polygon)differenceOp
				.execute(polygon1, polygon2, spatialRef, null);
			double area = outputPolygon.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 75) <= 0.001);
			{
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point(-130, 10);
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point(-130, 10);
				com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(point_1
					.getX(), point_1.getY());
				com.esri.core.geometry.Geometry comparisonGeom = new com.esri.core.geometry.Point
					(point_2.getX(), point2.getY());
				com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
					.create(4326);
				com.esri.core.geometry.Geometry geom = differenceOp.execute(baseGeom, comparisonGeom
					, sr, null);
			}
			com.esri.core.geometry.OperatorSymmetricDifference symDifferenceOp = (com.esri.core.geometry.OperatorSymmetricDifference
				)engine.getOperator(com.esri.core.geometry.Operator.Type.SymmetricDifference);
			outputPolygon = (com.esri.core.geometry.Polygon)symDifferenceOp.execute(polygon1, 
				polygon2, spatialRef, null);
			area = outputPolygon.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 150) <= 0.001);
			com.esri.core.geometry.Polyline outputPolyline = (com.esri.core.geometry.Polyline
				)differenceOp.execute(polyline1, polygon1, spatialRef, null);
			double length = outputPolyline.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length * length - 50) < 0.001);
			com.esri.core.geometry.MultiPoint outputMultiPoint = (com.esri.core.geometry.MultiPoint
				)differenceOp.execute(multipoint1, polygon1, spatialRef, null);
			int pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 1);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(symDifferenceOp.execute(multipoint1
				, point1, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 2);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(symDifferenceOp.execute(multipoint1
				, point2, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(differenceOp.execute(multipoint1
				, point1, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 2);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(differenceOp.execute(multipoint1
				, point2, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 3);
			outputPolygon = (com.esri.core.geometry.Polygon)(differenceOp.execute(polygon1, envelope1
				, spatialRef, null));
			area = outputPolygon.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 75) <= 0.001);
			outputPolygon = (com.esri.core.geometry.Polygon)(differenceOp.execute(polygon2, envelope2
				, spatialRef, null));
			area = outputPolygon.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 75) <= 0.001);
			outputPolyline = (com.esri.core.geometry.Polyline)(differenceOp.execute(polyline1
				, envelope2, spatialRef, null));
			length = outputPolyline.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length * length - 50) <= 0.001);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(differenceOp.execute(multipoint1
				, envelope2, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 1);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(differenceOp.execute(multipoint2
				, envelope2, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 6);
			outputMultiPoint = (com.esri.core.geometry.MultiPoint)(differenceOp.execute(multipoint3
				, envelope2, spatialRef, null));
			pointCount = outputMultiPoint.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 0);
			com.esri.core.geometry.Point outputPoint = (com.esri.core.geometry.Point)(differenceOp
				.execute(point1, envelope2, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(!outputPoint.isEmpty());
			outputPoint = (com.esri.core.geometry.Point)(differenceOp.execute(point2, envelope2
				, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPoint.isEmpty());
			outputPolygon = (com.esri.core.geometry.Polygon)(differenceOp.execute(envelope3, 
				envelope2, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPolygon != null && outputPolygon.isEmpty());
			outputPolygon = (com.esri.core.geometry.Polygon)(symDifferenceOp.execute(envelope3
				, envelope3, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPolygon != null && outputPolygon.isEmpty());
			outputPoint = (com.esri.core.geometry.Point)(differenceOp.execute(point1, polygon1
				, spatialRef, null));
			NUnit.Framework.Assert.IsTrue(outputPoint != null);
		}

		[NUnit.Framework.Test]
		public static void testPointTypes()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorDifference difference = (com.esri.core.geometry.OperatorDifference
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			com.esri.core.geometry.OperatorSymmetricDifference sym_difference = (com.esri.core.geometry.OperatorSymmetricDifference
				)engine.getOperator(com.esri.core.geometry.Operator.Type.SymmetricDifference);
			{
				// point/point
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point();
				point_1.setXY(0, 0);
				point_2.setXY(0.000000009, 0.000000009);
				com.esri.core.geometry.Point differenced = (com.esri.core.geometry.Point)(difference
					.execute(point_1, point_2, com.esri.core.geometry.SpatialReference.create(4326), 
					null));
				NUnit.Framework.Assert.IsTrue(differenced.isEmpty());
				com.esri.core.geometry.MultiPoint sym_differenced = (com.esri.core.geometry.MultiPoint
					)(sym_difference.execute(point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(sym_differenced.isEmpty());
			}
			{
				// point/point
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point();
				point_1.setXY(0, 0);
				point_2.setXY(0.000000009, 0.0);
				com.esri.core.geometry.Point differenced = (com.esri.core.geometry.Point)(difference
					.execute(point_1, point_2, com.esri.core.geometry.SpatialReference.create(4326), 
					null));
				NUnit.Framework.Assert.IsTrue(differenced.isEmpty());
				com.esri.core.geometry.MultiPoint sym_differenced = (com.esri.core.geometry.MultiPoint
					)(sym_difference.execute(point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(sym_differenced.isEmpty());
			}
			{
				// point/point
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point();
				point_1.setXY(0, 0);
				point_2.setXY(0.00000002, 0.00000002);
				com.esri.core.geometry.Point differenced_1 = (com.esri.core.geometry.Point)(difference
					.execute(point_1, point_2, com.esri.core.geometry.SpatialReference.create(4326), 
					null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				com.esri.core.geometry.Point differenced_2 = (com.esri.core.geometry.Point)(difference
					.execute(point_2, point_1, com.esri.core.geometry.SpatialReference.create(4326), 
					null));
				NUnit.Framework.Assert.IsTrue(!differenced_2.isEmpty());
				com.esri.core.geometry.MultiPoint sym_differenced = (com.esri.core.geometry.MultiPoint
					)(sym_difference.execute(point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!sym_differenced.isEmpty());
				NUnit.Framework.Assert.IsTrue(sym_differenced.getXY(0).x == 0 && sym_differenced.
					getXY(0).y == 0);
				NUnit.Framework.Assert.IsTrue(sym_differenced.getXY(1).x == 0.00000002 && sym_differenced
					.getXY(1).y == 0.00000002);
			}
			{
				// multi_point/point
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point();
				multi_point_1.add(0, 0);
				multi_point_1.add(1, 1);
				point_2.setXY(0.000000009, 0.000000009);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1.getPointCount() == 1);
				NUnit.Framework.Assert.IsTrue(differenced_1.getXY(0).x == 1 && differenced_1.getXY
					(0).y == 1);
				com.esri.core.geometry.Point differenced_2 = (com.esri.core.geometry.Point)(difference
					.execute(point_2, multi_point_1, com.esri.core.geometry.SpatialReference.create(
					4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_2.isEmpty());
			}
			{
				// multi_point/point
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point();
				multi_point_1.add(0, 0);
				multi_point_1.add(1, 1);
				point_2.setXY(0.000000009, 0.0);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1.getXY(0).x == 1.0 && differenced_1.getXY
					(0).y == 1.0);
				com.esri.core.geometry.Point differenced_2 = (com.esri.core.geometry.Point)(difference
					.execute(point_2, multi_point_1, com.esri.core.geometry.SpatialReference.create(
					4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_2.isEmpty());
				com.esri.core.geometry.MultiPoint sym_differenced = (com.esri.core.geometry.MultiPoint
					)(sym_difference.execute(multi_point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!sym_differenced.isEmpty());
				NUnit.Framework.Assert.IsTrue(sym_differenced.getPointCount() == 1);
				NUnit.Framework.Assert.IsTrue(sym_differenced.getXY(0).x == 1 && sym_differenced.
					getXY(0).y == 1);
			}
			{
				// multi_point/point
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Point point_2 = new com.esri.core.geometry.Point();
				multi_point_1.add(0, 0);
				multi_point_1.add(0, 0);
				point_2.setXY(0.000000009, 0.0);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.isEmpty());
				com.esri.core.geometry.MultiPoint sym_differenced = (com.esri.core.geometry.MultiPoint
					)(sym_difference.execute(multi_point_1, point_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(sym_differenced.isEmpty());
			}
			{
				// multi_point/polygon
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Polygon polygon_2 = new com.esri.core.geometry.Polygon();
				multi_point_1.add(0, 0);
				multi_point_1.add(0, 0);
				multi_point_1.add(2, 2);
				polygon_2.startPath(-1, -1);
				polygon_2.lineTo(-1, 1);
				polygon_2.lineTo(1, 1);
				polygon_2.lineTo(1, -1);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, polygon_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1.getPointCount() == 1);
				NUnit.Framework.Assert.IsTrue(differenced_1.getXY(0).x == 2 && differenced_1.getXY
					(0).y == 2);
			}
			{
				// multi_point/polygon
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Polygon polygon_2 = new com.esri.core.geometry.Polygon();
				multi_point_1.add(0, 0);
				multi_point_1.add(0, 0);
				multi_point_1.add(1, 1);
				polygon_2.startPath(-1, -1);
				polygon_2.lineTo(-1, 1);
				polygon_2.lineTo(1, 1);
				polygon_2.lineTo(1, -1);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, polygon_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.isEmpty());
			}
			{
				// multi_point/envelope
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Envelope envelope_2 = new com.esri.core.geometry.Envelope(
					);
				multi_point_1.add(-2, 0);
				multi_point_1.add(0, 2);
				multi_point_1.add(2, 0);
				multi_point_1.add(0, -2);
				envelope_2.setCoords(-1, -1, 1, 1);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, envelope_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty() && differenced_1 == multi_point_1
					);
			}
			{
				// multi_point/polygon
				com.esri.core.geometry.MultiPoint multi_point_1 = new com.esri.core.geometry.MultiPoint
					();
				com.esri.core.geometry.Polygon polygon_2 = new com.esri.core.geometry.Polygon();
				multi_point_1.add(2, 2);
				multi_point_1.add(2, 2);
				multi_point_1.add(-2, -2);
				polygon_2.startPath(-1, -1);
				polygon_2.lineTo(-1, 1);
				polygon_2.lineTo(1, 1);
				polygon_2.lineTo(1, -1);
				com.esri.core.geometry.MultiPoint differenced_1 = (com.esri.core.geometry.MultiPoint
					)(difference.execute(multi_point_1, polygon_2, com.esri.core.geometry.SpatialReference
					.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty() && differenced_1 == multi_point_1
					);
			}
			{
				// point/polygon
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Polygon polygon_2 = new com.esri.core.geometry.Polygon();
				point_1.setXY(0, 0);
				polygon_2.startPath(-1, -1);
				polygon_2.lineTo(-1, 1);
				polygon_2.lineTo(1, 1);
				polygon_2.lineTo(1, -1);
				com.esri.core.geometry.Point differenced_1 = (com.esri.core.geometry.Point)(difference
					.execute(point_1, polygon_2, com.esri.core.geometry.SpatialReference.create(4326
					), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.isEmpty());
				polygon_2.setEmpty();
				polygon_2.startPath(1, 1);
				polygon_2.lineTo(1, 2);
				polygon_2.lineTo(2, 2);
				polygon_2.lineTo(2, 1);
				differenced_1 = (com.esri.core.geometry.Point)(difference.execute(point_1, polygon_2
					, com.esri.core.geometry.SpatialReference.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
			}
			{
				// point/polygon
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Polygon polygon_2 = new com.esri.core.geometry.Polygon();
				point_1.setXY(0, 0);
				polygon_2.startPath(1, 0);
				polygon_2.lineTo(0, 1);
				polygon_2.lineTo(1, 1);
				com.esri.core.geometry.Point differenced_1 = (com.esri.core.geometry.Point)(difference
					.execute(point_1, polygon_2, com.esri.core.geometry.SpatialReference.create(4326
					), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
				point_1.setEmpty();
				point_1.setXY(0.5, 0.5);
				polygon_2.setEmpty();
				polygon_2.startPath(1, 0);
				polygon_2.lineTo(0, 1);
				polygon_2.lineTo(1, 1);
				differenced_1 = (com.esri.core.geometry.Point)(difference.execute(point_1, polygon_2
					, com.esri.core.geometry.SpatialReference.create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.isEmpty());
			}
			{
				// point/envelope
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Envelope envelope_2 = new com.esri.core.geometry.Envelope(
					);
				point_1.setXY(0, 0);
				envelope_2.setCoords(-1, -1, 1, 1);
				com.esri.core.geometry.Point differenced_1 = (com.esri.core.geometry.Point)(difference
					.execute(point_1, envelope_2, com.esri.core.geometry.SpatialReference.create(4326
					), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.isEmpty());
				envelope_2.setEmpty();
				envelope_2.setCoords(1, 1, 2, 2);
				differenced_1 = (com.esri.core.geometry.Point)(difference.execute(point_1, envelope_2
					, com.esri.core.geometry.SpatialReference.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
			}
			{
				// point/polyline
				com.esri.core.geometry.Point point_1 = new com.esri.core.geometry.Point();
				com.esri.core.geometry.Polyline polyline_2 = new com.esri.core.geometry.Polyline(
					);
				point_1.setXY(0, 0);
				polyline_2.startPath(-1, 0);
				polyline_2.lineTo(1, 0);
				com.esri.core.geometry.Point differenced_1 = (com.esri.core.geometry.Point)(difference
					.execute(point_1, polyline_2, com.esri.core.geometry.SpatialReference.create(4326
					), null));
				NUnit.Framework.Assert.IsTrue(differenced_1.isEmpty());
				polyline_2.setEmpty();
				polyline_2.startPath(1, 0);
				polyline_2.lineTo(2, 0);
				differenced_1 = (com.esri.core.geometry.Point)(difference.execute(point_1, polyline_2
					, com.esri.core.geometry.SpatialReference.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
				polyline_2.setEmpty();
				polyline_2.startPath(-1, -1);
				polyline_2.lineTo(-1, 1);
				polyline_2.lineTo(1, 1);
				polyline_2.lineTo(1, -1);
				differenced_1 = (com.esri.core.geometry.Point)(difference.execute(point_1, polyline_2
					, com.esri.core.geometry.SpatialReference.create(4326), null));
				NUnit.Framework.Assert.IsTrue(!differenced_1.isEmpty());
				NUnit.Framework.Assert.IsTrue(differenced_1 == point_1);
			}
		}

		[NUnit.Framework.Test]
		public static void testDifferenceOnPolyline()
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
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(new com.esri.core.geometry.Point(-117, 20));
			basePl.lineTo(new com.esri.core.geometry.Point(-130, 10));
			basePl.lineTo(new com.esri.core.geometry.Point(-120, 50));
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(new com.esri.core.geometry.Point(-116, 20));
			compPl.lineTo(new com.esri.core.geometry.Point(-131, 10));
			compPl.lineTo(new com.esri.core.geometry.Point(-121, 50));
			com.esri.core.geometry.Geometry diffGeom = com.esri.core.geometry.GeometryEngine.
				difference(basePl, compPl, com.esri.core.geometry.SpatialReference.create(4326));
			NUnit.Framework.Assert.IsTrue(diffGeom is com.esri.core.geometry.Polyline);
			com.esri.core.geometry.Polyline diffPolyline = (com.esri.core.geometry.Polyline)diffGeom;
			int pointCountDiffPolyline = diffPolyline.getPointCount();
			// first line in comp_pl is 3y = 2x + 292
			NUnit.Framework.Assert.AreEqual(3 * 20, 2 * (-116) + 292);
			NUnit.Framework.Assert.AreEqual(3 * 10, 2 * (-131) + 292);
			// new points should also lie on this line
			NUnit.Framework.Assert.IsTrue(3.0 * diffPolyline.getCoordinates2D()[1].y - 2.0 * 
				diffPolyline.getCoordinates2D()[1].x - 292.0 == 0.0);
			NUnit.Framework.Assert.IsTrue(3.0 * diffPolyline.getCoordinates2D()[3].y - 2.0 * 
				diffPolyline.getCoordinates2D()[3].x - 292.0 == 0.0);
			for (int i = 0; i < 3; i++)
			{
				NUnit.Framework.Assert.IsTrue(basePl.getCoordinates2D()[i].x == diffPolyline.getCoordinates2D
					()[2 * i].x);
				NUnit.Framework.Assert.IsTrue(basePl.getCoordinates2D()[i].y == diffPolyline.getCoordinates2D
					()[2 * i].y);
			}
			NUnit.Framework.Assert.AreEqual(5, pointCountDiffPolyline);
		}

		public static com.esri.core.geometry.Polygon makePolygon1()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			return poly;
		}

		public static com.esri.core.geometry.Polygon makePolygon2()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(5, 5);
			poly.lineTo(5, 15);
			poly.lineTo(15, 15);
			poly.lineTo(15, 5);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolyline1()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(15, 15);
			return poly;
		}

		public static com.esri.core.geometry.MultiPoint makeMultiPoint1()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			com.esri.core.geometry.Point2D pt1 = new com.esri.core.geometry.Point2D();
			pt1.x = 1.0;
			pt1.y = 1.0;
			com.esri.core.geometry.Point2D pt2 = new com.esri.core.geometry.Point2D();
			pt2.x = 5.0;
			pt2.y = 5.0;
			com.esri.core.geometry.Point2D pt3 = new com.esri.core.geometry.Point2D();
			pt3.x = 15.0;
			pt3.y = 15.0;
			mpoint.add(pt1.x, pt1.y);
			mpoint.add(pt2.x, pt2.y);
			mpoint.add(pt3.x, pt3.y);
			return mpoint;
		}

		public static com.esri.core.geometry.MultiPoint makeMultiPoint2()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			com.esri.core.geometry.Point2D pt1 = new com.esri.core.geometry.Point2D();
			pt1.x = 1.0;
			pt1.y = 1.0;
			com.esri.core.geometry.Point2D pt2 = new com.esri.core.geometry.Point2D();
			pt2.x = 1.0;
			pt2.y = 1.0;
			com.esri.core.geometry.Point2D pt3 = new com.esri.core.geometry.Point2D();
			pt3.x = 15.0;
			pt3.y = 15.0;
			com.esri.core.geometry.Point2D pt4 = new com.esri.core.geometry.Point2D();
			pt4.x = 15.0;
			pt4.y = 15.0;
			com.esri.core.geometry.Point2D pt5 = new com.esri.core.geometry.Point2D();
			pt5.x = 1.0;
			pt5.y = 1.0;
			com.esri.core.geometry.Point2D pt6 = new com.esri.core.geometry.Point2D();
			pt6.x = 1.0;
			pt6.y = 1.0;
			com.esri.core.geometry.Point2D pt7 = new com.esri.core.geometry.Point2D();
			pt7.x = 15.0;
			pt7.y = 15.0;
			com.esri.core.geometry.Point2D pt8 = new com.esri.core.geometry.Point2D();
			pt8.x = 15.0;
			pt8.y = 15.0;
			com.esri.core.geometry.Point2D pt9 = new com.esri.core.geometry.Point2D();
			pt9.x = 15.0;
			pt9.y = 15.0;
			com.esri.core.geometry.Point2D pt10 = new com.esri.core.geometry.Point2D();
			pt10.x = 1.0;
			pt10.y = 1.0;
			com.esri.core.geometry.Point2D pt11 = new com.esri.core.geometry.Point2D();
			pt11.x = 15.0;
			pt11.y = 15.0;
			mpoint.add(pt1.x, pt1.y);
			mpoint.add(pt2.x, pt2.y);
			mpoint.add(pt3.x, pt3.y);
			mpoint.add(pt4.x, pt4.y);
			mpoint.add(pt5.x, pt5.y);
			mpoint.add(pt6.x, pt6.y);
			mpoint.add(pt7.x, pt7.y);
			mpoint.add(pt8.x, pt8.y);
			mpoint.add(pt9.x, pt9.y);
			mpoint.add(pt10.x, pt10.y);
			mpoint.add(pt11.x, pt11.y);
			return mpoint;
		}

		public static com.esri.core.geometry.MultiPoint makeMultiPoint3()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			com.esri.core.geometry.Point2D pt1 = new com.esri.core.geometry.Point2D();
			pt1.x = 1.0;
			pt1.y = 1.0;
			com.esri.core.geometry.Point2D pt2 = new com.esri.core.geometry.Point2D();
			pt2.x = 5.0;
			pt2.y = 5.0;
			mpoint.add(pt1.x, pt1.y);
			mpoint.add(pt2.x, pt2.y);
			return mpoint;
		}

		public static com.esri.core.geometry.Point makePoint1()
		{
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point2D pt = new com.esri.core.geometry.Point2D();
			pt.setCoords(15, 15);
			point.setXY(pt);
			return point;
		}

		public static com.esri.core.geometry.Point makePoint2()
		{
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point2D pt = new com.esri.core.geometry.Point2D();
			pt.setCoords(7, 7);
			point.setXY(pt);
			return point;
		}

		public static com.esri.core.geometry.Envelope makeEnvelope1()
		{
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
			env.setCoords(5, 5, 15, 15);
			com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope(env
				);
			return envelope;
		}

		public static com.esri.core.geometry.Envelope makeEnvelope2()
		{
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
			env.setCoords(0, 0, 10, 10);
			com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope(env
				);
			return envelope;
		}

		public static com.esri.core.geometry.Envelope makeEnvelope3()
		{
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
			env.setCoords(5, 5, 6, 6);
			com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope(env
				);
			return envelope;
		}
	}
}
