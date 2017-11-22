using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestCut : NUnit.Framework.TestFixtureAttribute
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
		public static void TestCut4326()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			TestConsiderTouch1(sr);
			TestConsiderTouch2(sr);
			TestPolygon5(sr);
			TestPolygon7(sr);
			TestPolygon8(sr);
			TestPolygon9(sr);
			TestEngine(sr);
		}

		public static void TestConsiderTouch1(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorCut opCut = (com.epl.geometry.OperatorCut)engine.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.Polyline polyline1 = MakePolyline1();
			com.epl.geometry.Polyline cutter1 = MakePolylineCutter1();
			com.epl.geometry.GeometryCursor cursor = opCut.Execute(true, polyline1, cutter1, spatialReference, null);
			com.epl.geometry.Polyline cut;
			int pathCount;
			int segmentCount;
			double length;
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 4);
			NUnit.Framework.Assert.IsTrue(segmentCount == 4);
			NUnit.Framework.Assert.IsTrue(length == 6);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 6);
			NUnit.Framework.Assert.IsTrue(segmentCount == 8);
			NUnit.Framework.Assert.IsTrue(length == 12);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(length == 1);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(length == 1);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void TestConsiderTouch2(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorCut opCut = (com.epl.geometry.OperatorCut)engine.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.Polyline polyline2 = MakePolyline2();
			com.epl.geometry.Polyline cutter2 = MakePolylineCutter2();
			com.epl.geometry.GeometryCursor cursor = opCut.Execute(true, polyline2, cutter2, spatialReference, null);
			com.epl.geometry.Polyline cut;
			int pathCount;
			int segmentCount;
			double length;
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 4);
			NUnit.Framework.Assert.IsTrue(segmentCount == 4);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 5.74264068) <= 0.001);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 6);
			NUnit.Framework.Assert.IsTrue(segmentCount == 8);
			NUnit.Framework.Assert.IsTrue(length == 6.75);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 0.5) <= 0.001);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 0.25) <= 0.001);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 1) <= 0.001);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			pathCount = cut.GetPathCount();
			segmentCount = cut.GetSegmentCount();
			length = cut.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(segmentCount == 1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 1.41421356) <= 0.001);
			cut = (com.epl.geometry.Polyline)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void TestPolygon5(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorCut opCut = (com.epl.geometry.OperatorCut)engine.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.Polygon polygon5 = MakePolygon5();
			com.epl.geometry.Polyline cutter5 = MakePolygonCutter5();
			com.epl.geometry.GeometryCursor cursor = opCut.Execute(true, polygon5, cutter5, spatialReference, null);
			com.epl.geometry.Polygon cut;
			int pathCount;
			int pointCount;
			double area;
			cut = (com.epl.geometry.Polygon)cursor.Next();
			pathCount = cut.GetPathCount();
			pointCount = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 4);
			NUnit.Framework.Assert.IsTrue(pointCount == 12);
			NUnit.Framework.Assert.IsTrue(area == 450);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			pathCount = cut.GetPathCount();
			pointCount = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			NUnit.Framework.Assert.IsTrue(area == 450);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void TestPolygon7(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorCut opCut = (com.epl.geometry.OperatorCut)engine.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.Polygon cut;
			int path_count;
			int point_count;
			double area;
			com.epl.geometry.Polygon polygon7 = MakePolygon7();
			com.epl.geometry.Polyline cutter7 = MakePolygonCutter7();
			com.epl.geometry.GeometryCursor cursor = opCut.Execute(false, polygon7, cutter7, spatialReference, null);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			path_count = cut.GetPathCount();
			point_count = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 1);
			NUnit.Framework.Assert.IsTrue(point_count == 4);
			NUnit.Framework.Assert.IsTrue(area == 100);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut.IsEmpty());
			cut = (com.epl.geometry.Polygon)cursor.Next();
			path_count = cut.GetPathCount();
			point_count = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 2);
			NUnit.Framework.Assert.IsTrue(point_count == 8);
			NUnit.Framework.Assert.IsTrue(area == 800);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void TestPolygon8(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorCut opCut = (com.epl.geometry.OperatorCut)engine.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.Polygon polygon8 = MakePolygon8();
			com.epl.geometry.Polyline cutter8 = MakePolygonCutter8();
			com.epl.geometry.GeometryCursor cursor = opCut.Execute(true, polygon8, cutter8, spatialReference, null);
			com.epl.geometry.Polygon cut;
			int pathCount;
			int pointCount;
			double area;
			cut = (com.epl.geometry.Polygon)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut.IsEmpty());
			cut = (com.epl.geometry.Polygon)cursor.Next();
			pathCount = cut.GetPathCount();
			pointCount = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			NUnit.Framework.Assert.IsTrue(area == 100);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			pathCount = cut.GetPathCount();
			pointCount = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 2);
			NUnit.Framework.Assert.IsTrue(pointCount == 8);
			NUnit.Framework.Assert.IsTrue(area == 800);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void TestPolygon9(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorCut opCut = (com.epl.geometry.OperatorCut)engine.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.Polygon cut;
			int path_count;
			int point_count;
			double area;
			com.epl.geometry.Polygon polygon9 = MakePolygon9();
			com.epl.geometry.Polyline cutter9 = MakePolygonCutter9();
			com.epl.geometry.GeometryCursor cursor = opCut.Execute(false, polygon9, cutter9, spatialReference, null);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			path_count = cut.GetPathCount();
			point_count = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 3);
			NUnit.Framework.Assert.IsTrue(point_count == 12);
			NUnit.Framework.Assert.IsTrue(area == 150);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			path_count = cut.GetPathCount();
			point_count = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(path_count == 3);
			NUnit.Framework.Assert.IsTrue(point_count == 12);
			NUnit.Framework.Assert.IsTrue(area == 150);
			cut = (com.epl.geometry.Polygon)cursor.Next();
			NUnit.Framework.Assert.IsTrue(cut == null);
		}

		public static void TestEngine(com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.Polygon polygon8 = MakePolygon8();
			com.epl.geometry.Polyline cutter8 = MakePolygonCutter8();
			com.epl.geometry.Geometry[] cuts = com.epl.geometry.GeometryEngine.Cut(polygon8, cutter8, spatialReference);
			com.epl.geometry.Polygon cut;
			int pathCount;
			int pointCount;
			double area;
			cut = (com.epl.geometry.Polygon)cuts[0];
			pathCount = cut.GetPathCount();
			pointCount = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			NUnit.Framework.Assert.IsTrue(area == 100);
			cut = (com.epl.geometry.Polygon)cuts[1];
			pathCount = cut.GetPathCount();
			pointCount = cut.GetPointCount();
			area = cut.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(pathCount == 2);
			NUnit.Framework.Assert.IsTrue(pointCount == 8);
			NUnit.Framework.Assert.IsTrue(area == 800);
		}

		public static com.epl.geometry.Polyline MakePolyline1()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(2, 0);
			poly.LineTo(4, 0);
			poly.LineTo(6, 0);
			poly.LineTo(8, 0);
			poly.LineTo(10, 0);
			poly.LineTo(12, 0);
			poly.LineTo(14, 0);
			poly.LineTo(16, 0);
			poly.LineTo(18, 0);
			poly.LineTo(20, 0);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolylineCutter1()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(1, 0);
			poly.LineTo(4, 0);
			poly.StartPath(6, -1);
			poly.LineTo(6, 1);
			poly.StartPath(6, 0);
			poly.LineTo(8, 0);
			poly.StartPath(9, -1);
			poly.LineTo(9, 1);
			poly.StartPath(10, 0);
			poly.LineTo(12, 0);
			poly.StartPath(12, 1);
			poly.LineTo(12, -1);
			poly.StartPath(12, 0);
			poly.LineTo(15, 0);
			poly.StartPath(15, 1);
			poly.LineTo(15, -1);
			poly.StartPath(16, 0);
			poly.LineTo(16, -1);
			poly.LineTo(17, -1);
			poly.LineTo(17, 1);
			poly.LineTo(17, 0);
			poly.LineTo(18, 0);
			poly.StartPath(18, 0);
			poly.LineTo(18, -1);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolyline2()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(-2, 0);
			poly.LineTo(-1, 0);
			poly.LineTo(0, 0);
			poly.LineTo(2, 0);
			poly.LineTo(4, 2);
			poly.LineTo(8, 2);
			poly.LineTo(10, 4);
			poly.LineTo(12, 4);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolylineCutter2()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(-1.5, 0);
			poly.LineTo(-.75, 0);
			poly.StartPath(-.5, 0);
			poly.LineTo(1, 0);
			poly.LineTo(1, 2);
			poly.LineTo(3, -2);
			poly.LineTo(4, 2);
			poly.LineTo(5, -2);
			poly.LineTo(5, 4);
			poly.LineTo(8, 2);
			poly.LineTo(6, 0);
			poly.LineTo(6, 3);
			poly.StartPath(9, 5);
			poly.LineTo(9, 2);
			poly.LineTo(10, 2);
			poly.LineTo(10, 5);
			poly.LineTo(10.5, 5);
			poly.LineTo(10.5, 3);
			poly.StartPath(11, 4);
			poly.LineTo(11, 5);
			poly.StartPath(12, 5);
			poly.LineTo(12, 4);
			return poly;
		}

		public static com.epl.geometry.Polygon MakePolygon5()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 30);
			poly.LineTo(30, 30);
			poly.LineTo(30, 0);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolygonCutter5()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(15, 0);
			poly.LineTo(0, 15);
			poly.LineTo(15, 30);
			poly.LineTo(30, 15);
			poly.LineTo(15, 0);
			return poly;
		}

		public static com.epl.geometry.Polygon MakePolygon7()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 30);
			poly.LineTo(30, 30);
			poly.LineTo(30, 0);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolygonCutter7()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 10);
			poly.LineTo(20, 10);
			poly.LineTo(20, 20);
			poly.LineTo(10, 20);
			poly.LineTo(10, 10);
			return poly;
		}

		public static com.epl.geometry.Polygon MakePolygon8()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 30);
			poly.LineTo(30, 30);
			poly.LineTo(30, 0);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolygonCutter8()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 10);
			poly.LineTo(10, 20);
			poly.LineTo(20, 20);
			poly.LineTo(20, 10);
			poly.LineTo(10, 10);
			return poly;
		}

		public static com.epl.geometry.Polygon MakePolygon9()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.StartPath(0, 20);
			poly.LineTo(0, 30);
			poly.LineTo(10, 30);
			poly.LineTo(10, 20);
			poly.StartPath(0, 40);
			poly.LineTo(0, 50);
			poly.LineTo(10, 50);
			poly.LineTo(10, 40);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolygonCutter9()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(5, -1);
			poly.LineTo(5, 51);
			return poly;
		}
	}
}
