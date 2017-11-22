

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestCut
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
		public static void testCut4326()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			testConsiderTouch1(sr);
			testConsiderTouch2(sr);
			testPolygon5(sr);
			testPolygon7(sr);
			testPolygon8(sr);
			testPolygon9(sr);
			testEngine(sr);
		}

		public static void testConsiderTouch1(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorCut opCut = (com.esri.core.geometry.OperatorCut)engine
				.getOperator(com.esri.core.geometry.Operator.Type.Cut);
			com.esri.core.geometry.Polyline polyline1 = makePolyline1();
			com.esri.core.geometry.Polyline cutter1 = makePolylineCutter1();
			com.esri.core.geometry.GeometryCursor cursor = opCut.execute(true, polyline1, cutter1
				, spatialReference, null);
			com.esri.core.geometry.Polyline cut;
			int pathCount;
			int segmentCount;
			double length;
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 4);
			NUnit.Framework.Assert.IsTrue(segmentCount == 4);
			NUnit.Framework.Assert.IsTrue(length == 6);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 6);
			NUnit.Framework.Assert.IsTrue(segmentCount == 8);
			NUnit.Framework.Assert.IsTrue(length == 12);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(length == 1);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(length == 1);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void testConsiderTouch2(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorCut opCut = (com.esri.core.geometry.OperatorCut)engine
				.getOperator(com.esri.core.geometry.Operator.Type.Cut);
			com.esri.core.geometry.Polyline polyline2 = makePolyline2();
			com.esri.core.geometry.Polyline cutter2 = makePolylineCutter2();
			com.esri.core.geometry.GeometryCursor cursor = opCut.execute(true, polyline2, cutter2
				, spatialReference, null);
			com.esri.core.geometry.Polyline cut;
			int pathCount;
			int segmentCount;
			double length;
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 4);
			NUnit.Framework.Assert.IsTrue(segmentCount == 4);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 5.74264068) <= 0.001);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 6);
			NUnit.Framework.Assert.IsTrue(segmentCount == 8);
			NUnit.Framework.Assert.IsTrue(length == 6.75);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 0.5) <= 0.001);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 0.25) <= 0.001);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 1) <= 0.001);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			pathCount = cut.getPathCount();
			segmentCount = cut.getSegmentCount();
			length = cut.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 1.41421356) <= 0.001);
			cut = (com.esri.core.geometry.Polyline)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void testPolygon5(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorCut opCut = (com.esri.core.geometry.OperatorCut)engine
				.getOperator(com.esri.core.geometry.Operator.Type.Cut);
			com.esri.core.geometry.Polygon polygon5 = makePolygon5();
			com.esri.core.geometry.Polyline cutter5 = makePolygonCutter5();
			com.esri.core.geometry.GeometryCursor cursor = opCut.execute(true, polygon5, cutter5
				, spatialReference, null);
			com.esri.core.geometry.Polygon cut;
			int pathCount;
			int pointCount;
			double area;
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			pathCount = cut.getPathCount();
			pointCount = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 4);
			NUnit.Framework.Assert.IsTrue(pointCount == 12);
			NUnit.Framework.Assert.IsTrue(area == 450);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			pathCount = cut.getPathCount();
			pointCount = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			NUnit.Framework.Assert.IsTrue(area == 450);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void testPolygon7(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorCut opCut = (com.esri.core.geometry.OperatorCut)engine
				.getOperator(com.esri.core.geometry.Operator.Type.Cut);
			com.esri.core.geometry.Polygon cut;
			int path_count;
			int point_count;
			double area;
			com.esri.core.geometry.Polygon polygon7 = makePolygon7();
			com.esri.core.geometry.Polyline cutter7 = makePolygonCutter7();
			com.esri.core.geometry.GeometryCursor cursor = opCut.execute(false, polygon7, cutter7
				, spatialReference, null);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			path_count = cut.getPathCount();
			point_count = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 1);
			NUnit.Framework.Assert.IsTrue(point_count == 4);
			NUnit.Framework.Assert.IsTrue(area == 100);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut.isEmpty());
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			path_count = cut.getPathCount();
			point_count = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 2);
			NUnit.Framework.Assert.IsTrue(point_count == 8);
			NUnit.Framework.Assert.IsTrue(area == 800);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void testPolygon8(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorCut opCut = (com.esri.core.geometry.OperatorCut)engine
				.getOperator(com.esri.core.geometry.Operator.Type.Cut);
			com.esri.core.geometry.Polygon polygon8 = makePolygon8();
			com.esri.core.geometry.Polyline cutter8 = makePolygonCutter8();
			com.esri.core.geometry.GeometryCursor cursor = opCut.execute(true, polygon8, cutter8
				, spatialReference, null);
			com.esri.core.geometry.Polygon cut;
			int pathCount;
			int pointCount;
			double area;
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut.isEmpty());
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			pathCount = cut.getPathCount();
			pointCount = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			NUnit.Framework.Assert.IsTrue(area == 100);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			pathCount = cut.getPathCount();
			pointCount = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 2);
			NUnit.Framework.Assert.IsTrue(pointCount == 8);
			NUnit.Framework.Assert.IsTrue(area == 800);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void testPolygon9(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorCut opCut = (com.esri.core.geometry.OperatorCut)engine
				.getOperator(com.esri.core.geometry.Operator.Type.Cut);
			com.esri.core.geometry.Polygon cut;
			int path_count;
			int point_count;
			double area;
			com.esri.core.geometry.Polygon polygon9 = makePolygon9();
			com.esri.core.geometry.Polyline cutter9 = makePolygonCutter9();
			com.esri.core.geometry.GeometryCursor cursor = opCut.execute(false, polygon9, cutter9
				, spatialReference, null);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			path_count = cut.getPathCount();
			point_count = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 3);
			NUnit.Framework.Assert.IsTrue(point_count == 12);
			NUnit.Framework.Assert.IsTrue(area == 150);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			path_count = cut.getPathCount();
			point_count = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 3);
			NUnit.Framework.Assert.IsTrue(point_count == 12);
			NUnit.Framework.Assert.IsTrue(area == 150);
			cut = (com.esri.core.geometry.Polygon)cursor.next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void testEngine(com.esri.core.geometry.SpatialReference spatialReference
			)
		{
			com.esri.core.geometry.Polygon polygon8 = makePolygon8();
			com.esri.core.geometry.Polyline cutter8 = makePolygonCutter8();
			com.esri.core.geometry.Geometry[] cuts = com.esri.core.geometry.GeometryEngine.cut
				(polygon8, cutter8, spatialReference);
			com.esri.core.geometry.Polygon cut;
			int pathCount;
			int pointCount;
			double area;
			cut = (com.esri.core.geometry.Polygon)cuts[0];
			pathCount = cut.getPathCount();
			pointCount = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			NUnit.Framework.Assert.IsTrue(area == 100);
			cut = (com.esri.core.geometry.Polygon)cuts[1];
			pathCount = cut.getPathCount();
			pointCount = cut.getPointCount();
			area = cut.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 2);
			NUnit.Framework.Assert.IsTrue(pointCount == 8);
			NUnit.Framework.Assert.IsTrue(area == 800);
		}

		public static com.esri.core.geometry.Polyline makePolyline1()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(2, 0);
			poly.lineTo(4, 0);
			poly.lineTo(6, 0);
			poly.lineTo(8, 0);
			poly.lineTo(10, 0);
			poly.lineTo(12, 0);
			poly.lineTo(14, 0);
			poly.lineTo(16, 0);
			poly.lineTo(18, 0);
			poly.lineTo(20, 0);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolylineCutter1()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(1, 0);
			poly.lineTo(4, 0);
			poly.startPath(6, -1);
			poly.lineTo(6, 1);
			poly.startPath(6, 0);
			poly.lineTo(8, 0);
			poly.startPath(9, -1);
			poly.lineTo(9, 1);
			poly.startPath(10, 0);
			poly.lineTo(12, 0);
			poly.startPath(12, 1);
			poly.lineTo(12, -1);
			poly.startPath(12, 0);
			poly.lineTo(15, 0);
			poly.startPath(15, 1);
			poly.lineTo(15, -1);
			poly.startPath(16, 0);
			poly.lineTo(16, -1);
			poly.lineTo(17, -1);
			poly.lineTo(17, 1);
			poly.lineTo(17, 0);
			poly.lineTo(18, 0);
			poly.startPath(18, 0);
			poly.lineTo(18, -1);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolyline2()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(-2, 0);
			poly.lineTo(-1, 0);
			poly.lineTo(0, 0);
			poly.lineTo(2, 0);
			poly.lineTo(4, 2);
			poly.lineTo(8, 2);
			poly.lineTo(10, 4);
			poly.lineTo(12, 4);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolylineCutter2()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(-1.5, 0);
			poly.lineTo(-.75, 0);
			poly.startPath(-.5, 0);
			poly.lineTo(1, 0);
			poly.lineTo(1, 2);
			poly.lineTo(3, -2);
			poly.lineTo(4, 2);
			poly.lineTo(5, -2);
			poly.lineTo(5, 4);
			poly.lineTo(8, 2);
			poly.lineTo(6, 0);
			poly.lineTo(6, 3);
			poly.startPath(9, 5);
			poly.lineTo(9, 2);
			poly.lineTo(10, 2);
			poly.lineTo(10, 5);
			poly.lineTo(10.5, 5);
			poly.lineTo(10.5, 3);
			poly.startPath(11, 4);
			poly.lineTo(11, 5);
			poly.startPath(12, 5);
			poly.lineTo(12, 4);
			return poly;
		}

		public static com.esri.core.geometry.Polygon makePolygon5()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 30);
			poly.lineTo(30, 30);
			poly.lineTo(30, 0);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolygonCutter5()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(15, 0);
			poly.lineTo(0, 15);
			poly.lineTo(15, 30);
			poly.lineTo(30, 15);
			poly.lineTo(15, 0);
			return poly;
		}

		public static com.esri.core.geometry.Polygon makePolygon7()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 30);
			poly.lineTo(30, 30);
			poly.lineTo(30, 0);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolygonCutter7()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 10);
			poly.lineTo(20, 10);
			poly.lineTo(20, 20);
			poly.lineTo(10, 20);
			poly.lineTo(10, 10);
			return poly;
		}

		public static com.esri.core.geometry.Polygon makePolygon8()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 30);
			poly.lineTo(30, 30);
			poly.lineTo(30, 0);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolygonCutter8()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 10);
			poly.lineTo(10, 20);
			poly.lineTo(20, 20);
			poly.lineTo(20, 10);
			poly.lineTo(10, 10);
			return poly;
		}

		public static com.esri.core.geometry.Polygon makePolygon9()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.startPath(0, 20);
			poly.lineTo(0, 30);
			poly.lineTo(10, 30);
			poly.lineTo(10, 20);
			poly.startPath(0, 40);
			poly.lineTo(0, 50);
			poly.lineTo(10, 50);
			poly.lineTo(10, 40);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolygonCutter9()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(5, -1);
			poly.lineTo(5, 51);
			return poly;
		}
	}
}
