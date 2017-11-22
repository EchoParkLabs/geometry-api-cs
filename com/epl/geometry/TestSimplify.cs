

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestSimplify
	{
		internal com.esri.core.geometry.OperatorFactoryLocal factory = null;

		internal com.esri.core.geometry.OperatorSimplify simplifyOp = null;

		internal com.esri.core.geometry.OperatorSimplifyOGC simplifyOpOGC = null;

		internal com.esri.core.geometry.SpatialReference sr102100 = null;

		internal com.esri.core.geometry.SpatialReference sr4326 = null;

		internal com.esri.core.geometry.SpatialReference sr3857 = null;

		//import java.io.FileOutputStream;
		//import java.io.PrintStream;
		//import java.util.ArrayList;
		//import java.util.List;
		//import java.util.Random;
		/// <exception cref="System.Exception"/>
		protected override void setUp()
		{
			base.setUp();
			factory = com.esri.core.geometry.OperatorFactoryLocal.getInstance();
			simplifyOp = (com.esri.core.geometry.OperatorSimplify)factory.getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			simplifyOpOGC = (com.esri.core.geometry.OperatorSimplifyOGC)factory.getOperator(com.esri.core.geometry.Operator.Type
				.SimplifyOGC);
			sr102100 = com.esri.core.geometry.SpatialReference.create(102100);
			sr3857 = com.esri.core.geometry.SpatialReference.create(3857);
			// PE_PCS_WGS_1984_WEB_MERCATOR_AUXSPHERE);
			sr4326 = com.esri.core.geometry.SpatialReference.create(4326);
		}

		// enum_value2(SpatialReference,
		// Code, GCS_WGS_1984));
		/// <exception cref="System.Exception"/>
		protected override void tearDown()
		{
			base.tearDown();
		}

		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon2()
		{
			//MapGeometry mg = OperatorFactoryLocal.loadGeometryFromJSONFileDbg("c:/temp/simplify_polygon_gnomonic.txt");
			//Geometry res = OperatorSimplify.local().execute(mg.getGeometry(), mg.getSpatialReference(), true, null);
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 15);
			poly.lineTo(15, 15);
			poly.lineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.startPath(5, 5);
			poly.lineTo(5, 6);
			poly.lineTo(6, 6);
			poly.lineTo(6, 5);
			return poly;
		}

		// done
		/*
		* ------------>---------------->--------------- | | | (1) | | | | --->---
		* ------->------- | | | | | (5) | | | | | | --<-- | | | | (2) | | | | | | |
		* | | | | (4) | | | | | | | -->-- | | --<-- | ---<--- | | | | | |
		* -------<------- | | (3) | -------------<---------------<---------------
		* -->--
		*/
		// Bowtie case with vertices at intersection
		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon5()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 0);
			poly.lineTo(0, 0);
			poly.lineTo(5, 5);
			poly.lineTo(10, 10);
			poly.lineTo(0, 10);
			poly.lineTo(5, 5);
			return poly;
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0()
		{
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.addEnvelope(new com.esri.core.geometry.Envelope(10, 10, 40, 20), false);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			bool res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		// assertTrue(poly1.equals(poly2));
		// done
		[NUnit.Framework.Test]
		public virtual void test0Poly()
		{
			// simple
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.addEnvelope(new com.esri.core.geometry.Envelope(10, 10, 40, 20), false);
			poly1.addEnvelope(new com.esri.core.geometry.Envelope(50, 10, 100, 20), false);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			bool res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		// assertTrue(poly1.equals(poly2));
		// done
		[NUnit.Framework.Test]
		public virtual void test0Polygon_Spike1()
		{
			// non-simple (spike)
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.startPath(10, 10);
			poly1.lineTo(10, 20);
			poly1.lineTo(40, 20);
			poly1.lineTo(40, 10);
			poly1.lineTo(60, 10);
			poly1.lineTo(70, 10);
			bool res = simplifyOp.isSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(poly2.getPointCount() == 4);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0Polygon_Spike2()
		{
			// non-simple (spikes)
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			// rectangle with a spike
			poly1.startPath(10, 10);
			poly1.lineTo(10, 20);
			poly1.lineTo(40, 20);
			poly1.lineTo(40, 10);
			poly1.lineTo(60, 10);
			poly1.lineTo(70, 10);
			// degenerate
			poly1.startPath(100, 100);
			poly1.lineTo(100, 120);
			poly1.lineTo(100, 130);
			bool res = simplifyOp.isSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(poly2.getPointCount() == 4);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0Polygon_Spike3()
		{
			// non-simple (spikes)
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			// degenerate
			poly1.startPath(100, 100);
			poly1.lineTo(100, 120);
			poly1.lineTo(100, 130);
			bool res = simplifyOp.isSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonSelfIntersect1()
		{
			// non-simple (self-intersection)
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			// touch uncracked
			poly1.startPath(0, 0);
			poly1.lineTo(0, 100);
			poly1.lineTo(100, 100);
			poly1.lineTo(0, 50);
			poly1.lineTo(100, 0);
			bool res = simplifyOp.isSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonSelfIntersect2()
		{
			// non-simple (self-intersection)
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.startPath(0, 0);
			poly1.lineTo(0, 100);
			poly1.lineTo(100, 100);
			poly1.lineTo(-100, 0);
			// poly1.lineTo(100, 0);
			bool res = simplifyOp.isSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly1, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonSelfIntersect3()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 15);
			poly.lineTo(15, 15);
			poly.lineTo(15, 0);
			// This part intersects with the first part
			poly.startPath(10, 10);
			poly.lineTo(10, 20);
			poly.lineTo(20, 20);
			poly.lineTo(20, 10);
			bool res = simplifyOp.isSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonInteriorRing1()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 15);
			poly.lineTo(15, 15);
			poly.lineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.startPath(5, 5);
			poly.lineTo(5, 6);
			poly.lineTo(6, 6);
			poly.lineTo(6, 5);
			bool res = simplifyOp.isSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonInteriorRing2()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 15);
			poly.lineTo(15, 15);
			poly.lineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.startPath(5, 5);
			poly.lineTo(5, 6);
			poly.lineTo(6, 6);
			poly.lineTo(6, 5);
			// This part intersects with the first part
			poly.startPath(10, 10);
			poly.lineTo(10, 20);
			poly.lineTo(20, 20);
			poly.lineTo(20, 10);
			bool res = simplifyOp.isSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonInteriorRingWithCommonBoundary1()
		{
			// Two rings have common boundary
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.startPath(10, 0);
			poly.lineTo(10, 10);
			poly.lineTo(20, 10);
			poly.lineTo(20, 0);
			bool res = simplifyOp.isSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void test0PolygonInteriorRingWithCommonBoundary2()
		{
			// Two rings have common boundary
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.startPath(10, 5);
			poly.lineTo(10, 6);
			poly.lineTo(20, 6);
			poly.lineTo(20, 5);
			bool res = simplifyOp.isSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(poly, null, false, null);
			res = simplifyOp.isSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolygon()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon = makeNonSimplePolygon();
			com.esri.core.geometry.Polygon simplePolygon = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon, sr3857, false, null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			int partCount = simplePolygon.getPathCount();
			// assertTrue(partCount == 2);
			double area = simplePolygon.calculateRingArea2D(0);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 300) <= 0.0001);
			area = simplePolygon.calculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - (-25.0)) <= 0.0001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolygon2()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon2 = makeNonSimplePolygon2();
			double area = nonSimplePolygon2.calculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 1.0) <= 0.0001);
			com.esri.core.geometry.Polygon simplePolygon2 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon2, sr3857, false, null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon2, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			area = simplePolygon2.calculateRingArea2D(0);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 225) <= 0.0001);
			area = simplePolygon2.calculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - (-1.0)) <= 0.0001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolygon3()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon3 = makeNonSimplePolygon3();
			com.esri.core.geometry.Polygon simplePolygon3 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon3, sr3857, false, null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon3, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			double area = simplePolygon3.calculateRingArea2D(0);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 875) <= 0.0001);
			area = simplePolygon3.calculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - (-225)) <= 0.0001 || System.Math
				.abs(area - (-50.0)) <= 0.0001);
			area = simplePolygon3.calculateRingArea2D(2);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - (-225)) <= 0.0001 || System.Math
				.abs(area - (-50.0)) <= 0.0001);
			area = simplePolygon3.calculateRingArea2D(3);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 25) <= 0.0001);
			area = simplePolygon3.calculateRingArea2D(4);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 25) <= 0.0001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolyline()
		{
			com.esri.core.geometry.Polyline nonSimplePolyline = makeNonSimplePolyline();
			com.esri.core.geometry.Polyline simplePolyline = (com.esri.core.geometry.Polyline
				)simplifyOp.execute(nonSimplePolyline, sr3857, false, null);
			int segmentCount = simplePolyline.getSegmentCount();
			NUnit.Framework.Assert.IsTrue(segmentCount == 4);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolygon4()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon4 = makeNonSimplePolygon4();
			com.esri.core.geometry.Polygon simplePolygon4 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon4, sr3857, false, null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon4, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(simplePolygon4.getPointCount() == 5);
			com.esri.core.geometry.Point point = nonSimplePolygon4.getPoint(0);
			NUnit.Framework.Assert.IsTrue(point.getX() == 0.0 && point.getY() == 0.0);
			point = nonSimplePolygon4.getPoint(1);
			NUnit.Framework.Assert.IsTrue(point.getX() == 0.0 && point.getY() == 10.0);
			point = nonSimplePolygon4.getPoint(2);
			NUnit.Framework.Assert.IsTrue(point.getX() == 10.0 && point.getY() == 10.0);
			point = nonSimplePolygon4.getPoint(3);
			NUnit.Framework.Assert.IsTrue(point.getX() == 10.0 && point.getY() == 0.0);
			point = nonSimplePolygon4.getPoint(4);
			NUnit.Framework.Assert.IsTrue(point.getX() == 5.0 && point.getY() == 0.0);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolygon5()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon5 = makeNonSimplePolygon5();
			com.esri.core.geometry.Polygon simplePolygon5 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon5, sr3857, false, null);
			NUnit.Framework.Assert.IsTrue(simplePolygon5 != null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon5, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			int pointCount = simplePolygon5.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 6);
			double area = simplePolygon5.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 50.0) <= 0.001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testPolygon6()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon6 = makeNonSimplePolygon6();
			com.esri.core.geometry.Polygon simplePolygon6 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon6, sr3857, false, null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon6, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygon7()
		{
			com.esri.core.geometry.Polygon nonSimplePolygon7 = makeNonSimplePolygon7();
			com.esri.core.geometry.Polygon simplePolygon7 = (com.esri.core.geometry.Polygon)simplifyOp
				.execute(nonSimplePolygon7, sr3857, false, null);
			bool res = simplifyOp.isSimpleAsFeature(simplePolygon7, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 15);
			poly.lineTo(15, 15);
			poly.lineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.startPath(5, 5);
			poly.lineTo(5, 6);
			poly.lineTo(6, 6);
			poly.lineTo(6, 5);
			// This part intersects with the first part
			poly.startPath(10, 10);
			poly.lineTo(10, 20);
			poly.lineTo(20, 20);
			poly.lineTo(20, 10);
			return poly;
		}

		// done
		/*
		* ------------>---------------->--------------- | | | (1) | | | | --->---
		* ------->------- | | | | | (5) | | | | | | --<-- | | | | (2) | | | | | | |
		* | | | | (4) | | | | | | | -->-- | | --<-- | ---<--- | | | | | |
		* -------<------- | | (3) | -------------<---------------<---------------
		* -->--
		*/
		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon3()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 25);
			poly.lineTo(35, 25);
			poly.lineTo(35, 0);
			poly.startPath(5, 5);
			poly.lineTo(5, 15);
			poly.lineTo(10, 15);
			poly.lineTo(10, 5);
			poly.startPath(40, 0);
			poly.lineTo(45, 0);
			poly.lineTo(45, 5);
			poly.lineTo(40, 5);
			poly.startPath(20, 10);
			poly.lineTo(25, 10);
			poly.lineTo(25, 15);
			poly.lineTo(20, 15);
			poly.startPath(15, 5);
			poly.lineTo(15, 20);
			poly.lineTo(30, 20);
			poly.lineTo(30, 5);
			return poly;
		}

		// done
		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon4()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.lineTo(5, 0);
			poly.lineTo(5, 5);
			poly.lineTo(5, 0);
			return poly;
		}

		// done
		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon6()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(35.34407570857744, 54.00551247713412);
			poly.lineTo(41.07663499357954, 20.0);
			poly.lineTo(40.66372033705177, 26.217432321849017);
			poly.startPath(42.81936574509338, 20.0);
			poly.lineTo(43.58226670584747, 20.0);
			poly.lineTo(39.29611825817084, 22.64634933678729);
			poly.lineTo(44.369873312241346, 25.81893670527215);
			poly.lineTo(42.68845660737179, 20.0);
			poly.lineTo(38.569549792944244, 56.47456192829393);
			poly.lineTo(42.79274114188401, 45.45117792578003);
			poly.lineTo(41.09512147544657, 70.0);
			return poly;
		}

		public virtual com.esri.core.geometry.Polygon makeNonSimplePolygon7()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(41.987895433319686, 53.75822619011542);
			poly.lineTo(41.98789542535497, 53.75822618803151);
			poly.lineTo(40.15120412113667, 68.12604154722113);
			poly.lineTo(37.72272697311022, 67.92767094118877);
			poly.lineTo(37.147347454283086, 49.497473094145505);
			poly.lineTo(38.636627026664385, 51.036687142232736);
			poly.startPath(39.00920080789793, 62.063425518369016);
			poly.lineTo(38.604912643136885, 70.0);
			poly.lineTo(40.71826863485308, 43.60337143116787);
			poly.lineTo(35.34407570857744, 54.005512477134126);
			poly.lineTo(39.29611825817084, 22.64634933678729);
			return poly;
		}

		public virtual com.esri.core.geometry.Polyline makeNonSimplePolyline()
		{
			// This polyline has a short segment
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(10, 0);
			poly.lineTo(10, 10);
			poly.lineTo(10, 5);
			poly.lineTo(-5, 5);
			return poly;
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleBasicsPoint()
		{
			bool result;
			// point is always simple
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
			result = simplifyOp.isSimpleAsFeature(pt, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			pt.setXY(0, 0);
			result = simplifyOp.isSimpleAsFeature(pt, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			pt.setXY(100000, 10000);
			result = simplifyOp.isSimpleAsFeature(pt, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleBasicsEnvelope()
		{
			// Envelope is simple, when it's width and height are not degenerate
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			bool result = simplifyOp.isSimpleAsFeature(env, sr4326, false, null, null);
			// Empty is simple
			NUnit.Framework.Assert.IsTrue(result);
			env.setCoords(0, 0, 10, 10);
			result = simplifyOp.isSimpleAsFeature(env, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// sliver but still simple
			env.setCoords(0, 0, 0 + sr4326.getTolerance() * 2, 10);
			result = simplifyOp.isSimpleAsFeature(env, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// sliver and not simple
			env.setCoords(0, 0, 0 + sr4326.getTolerance() * 0.5, 10);
			result = simplifyOp.isSimpleAsFeature(env, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleBasicsLine()
		{
			com.esri.core.geometry.Line line = new com.esri.core.geometry.Line();
			bool result = simplifyOp.isSimpleAsFeature(line, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			line.setStart(new com.esri.core.geometry.Point(0, 0));
			// line.setEndXY(0, 0);
			result = simplifyOp.isSimpleAsFeature(line, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			line.setEnd(new com.esri.core.geometry.Point(1, 0));
			result = simplifyOp.isSimpleAsFeature(line, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPoint1()
		{
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// empty is simple
			result = simplifyOp.isSimpleAsFeature(simplifyOp.execute(mp, sr4326, false, null)
				, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPoint2FarApart()
		{
			// Two point test: far apart
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(20, 10);
			mp.add(100, 100);
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.isSimpleAsFeature(simplifyOp.execute(mp, sr4326, false, null)
				, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mp.getPointCount() == 2);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPointCoincident()
		{
			// Two point test: coincident
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(100, 100);
			mp.add(100, 100);
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.MultiPoint mpS;
			result = simplifyOp.isSimpleAsFeature(mpS = (com.esri.core.geometry.MultiPoint)simplifyOp
				.execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.getPointCount() == 1);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testMultiPointSR4326_CR184439()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorSimplify simpOp = (com.esri.core.geometry.OperatorSimplify
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Simplify);
			com.esri.core.geometry.NonSimpleResult nonSimpResult = new com.esri.core.geometry.NonSimpleResult
				();
			nonSimpResult.m_reason = com.esri.core.geometry.NonSimpleResult.Reason.NotDetermined;
			com.esri.core.geometry.MultiPoint multiPoint = new com.esri.core.geometry.MultiPoint
				();
			multiPoint.add(0, 0);
			multiPoint.add(0, 1);
			multiPoint.add(0, 0);
			bool multiPointIsSimple = simpOp.isSimpleAsFeature(multiPoint, com.esri.core.geometry.SpatialReference
				.create(4326), true, nonSimpResult, null);
			NUnit.Framework.Assert.IsFalse(multiPointIsSimple);
			NUnit.Framework.Assert.IsTrue(nonSimpResult.m_reason == com.esri.core.geometry.NonSimpleResult.Reason
				.Clustering);
			NUnit.Framework.Assert.IsTrue(nonSimpResult.m_vertexIndex1 == 0);
			NUnit.Framework.Assert.IsTrue(nonSimpResult.m_vertexIndex2 == 2);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPointCloserThanTolerance()
		{
			// Two point test: closer than tolerance
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			com.esri.core.geometry.MultiPoint mpS;
			mp.add(100, 100);
			mp.add(100, 100 + sr4326.getTolerance() * .5);
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.isSimpleAsFeature(mpS = (com.esri.core.geometry.MultiPoint)simplifyOp
				.execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.getPointCount() == 2);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPointFarApart2()
		{
			// 5 point test: far apart
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(100, 100);
			mp.add(100, 101);
			mp.add(101, 101);
			mp.add(11, 1);
			mp.add(11, 14);
			com.esri.core.geometry.MultiPoint mpS;
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.isSimpleAsFeature(mpS = (com.esri.core.geometry.MultiPoint)simplifyOp
				.execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.getPointCount() == 5);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPoint_coincident2()
		{
			// 5 point test: coincident
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(100, 100);
			mp.add(100, 101);
			mp.add(100, 100);
			mp.add(11, 1);
			mp.add(11, 14);
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.MultiPoint mpS;
			result = simplifyOp.isSimpleAsFeature(mpS = (com.esri.core.geometry.MultiPoint)simplifyOp
				.execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.getPointCount() == 4);
			NUnit.Framework.Assert.AreEqual(100, 1e-7, mpS.getPoint(0).getX());
			NUnit.Framework.Assert.AreEqual(100, 1e-7, mpS.getPoint(0).getY());
			NUnit.Framework.Assert.AreEqual(100, 1e-7, mpS.getPoint(1).getX());
			NUnit.Framework.Assert.AreEqual(101, 1e-7, mpS.getPoint(1).getY());
			NUnit.Framework.Assert.AreEqual(11, 1e-7, mpS.getPoint(2).getX());
			NUnit.Framework.Assert.AreEqual(1, 1e-7, mpS.getPoint(2).getY());
			NUnit.Framework.Assert.AreEqual(11, 1e-7, mpS.getPoint(3).getX());
			NUnit.Framework.Assert.AreEqual(14, 1e-7, mpS.getPoint(3).getY());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimpleMultiPointCloserThanTolerance2()
		{
			// 5 point test: closer than tolerance
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(100, 100);
			mp.add(100, 101);
			mp.add(100, 100 + sr4326.getTolerance() / 2);
			mp.add(11, 1);
			mp.add(11, 14);
			com.esri.core.geometry.MultiPoint mpS;
			bool result = simplifyOp.isSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.isSimpleAsFeature(mpS = (com.esri.core.geometry.MultiPoint)simplifyOp
				.execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.getPointCount() == 5);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// empty is simple
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineFarApart()
		{
			// Two point test: far apart
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(20, 10);
			poly.lineTo(100, 100);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineCoincident()
		{
			// Two point test: coincident
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(100, 100);
			poly.lineTo(100, 100);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.Polyline polyS;
			result = simplifyOp.isSimpleAsFeature(polyS = (com.esri.core.geometry.Polyline)simplifyOp
				.execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineCloserThanTolerance()
		{
			// Two point test: closer than tolerance
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(100, 100);
			poly.lineTo(100, 100 + sr4326.getTolerance() / 2);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.Polyline polyS;
			result = simplifyOp.isSimpleAsFeature(polyS = (com.esri.core.geometry.Polyline)simplifyOp
				.execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineFarApartSelfOverlap0()
		{
			// 3 point test: far apart, self overlapping
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(0, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineSelfIntersect()
		{
			// 4 point test: far apart, self intersecting
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(0, 100);
			poly.lineTo(100, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineDegenerateSegment()
		{
			// 4 point test: degenerate segment
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(100, 100 + sr4326.getTolerance() / 2);
			poly.lineTo(100, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.Polyline polyS;
			result = simplifyOp.isSimpleAsFeature(polyS = (com.esri.core.geometry.Polyline)simplifyOp
				.execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			{
				com.esri.core.geometry.Polyline other = new com.esri.core.geometry.Polyline();
				other.startPath(0, 0);
				other.lineTo(100, 100);
				other.lineTo(100, 0);
				other.Equals(poly);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineFarApartSelfOverlap()
		{
			// 3 point test: far apart, self overlapping
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(0, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineFarApartIntersect()
		{
			// 4 point 2 parts test: far apart, intersecting parts
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.startPath(100, 0);
			poly.lineTo(0, 100);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineFarApartOverlap2()
		{
			// 4 point 2 parts test: far apart, overlapping parts. second part
			// starts where first one ends
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.startPath(100, 100);
			poly.lineTo(0, 100);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineDegenerateVertical()
		{
			// 3 point test: degenerate vertical line
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(new com.esri.core.geometry.Point(100, 100));
			poly.lineTo(new com.esri.core.geometry.Point(100, 100));
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.Polyline polyS;
			result = simplifyOp.isSimpleAsFeature(polyS = (com.esri.core.geometry.Polyline)simplifyOp
				.execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(polyS.getPointCount() == 2);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineEmptyPath()
		{
		}

		// TODO: any way to test this?
		// Empty path
		// Polyline poly = new Polyline();
		// assertTrue(poly.isEmpty());
		// poly.addPath(new Polyline(), 0, true);
		// assertTrue(poly.isEmpty());
		// boolean result = simplifyOp.isSimpleAsFeature(poly, sr4326, false,
		// null, null);
		// assertTrue(result);
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolylineSinglePointInPath()
		{
			// Single point in path
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.removePoint(0, 1);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.esri.core.geometry.Polyline polyS;
			result = simplifyOp.isSimpleAsFeature(polyS = (com.esri.core.geometry.Polyline)simplifyOp
				.execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(polyS.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygon()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// empty is simple
			result = simplifyOp.isSimpleAsFeature(simplifyOp.execute(poly, sr4326, false, null
				), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// empty is simple
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonEmptyPath()
		{
		}

		// TODO:
		// Empty path
		// Polygon poly = new Polygon();
		// poly.addPath(new Polyline(), 0, true);
		// assertTrue(poly.getPathCount() == 1);
		// boolean result = simplifyOp.isSimpleAsFeature(poly, sr4326, false,
		// null,
		// null);
		// assertTrue(result);
		// result = simplifyOp.isSimpleAsFeature(simplifyOp.execute(poly,
		// sr4326, false, null), sr4326, false, null, null);
		// assertTrue(result);// empty is simple
		// assertTrue(poly.getPathCount() == 1);
		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonIncomplete1()
		{
			// Incomplete polygon 1
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			// poly.removePoint(0, 1);//TO CONFIRM no removePoint method in Java
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonIncomplete2()
		{
			// Incomplete polygon 2
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonDegenerateTriangle()
		{
			// Degenerate triangle (self overlap)
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(0, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonSelfIntersect()
		{
			// Self intersection - cracking is needed
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(0, 100);
			poly.lineTo(100, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonRectangleHole()
		{
			// Rectangle and rectangular hole that has one segment overlapping
			// with the with the exterior ring. Cracking is needed.
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-200, -100, 200, 100), false
				);
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-100, -100, 100, 50), true);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonRectangleHole2()
		{
			// Rectangle and rectangular hole
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-200, -100, 200, 100), false
				);
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-100, -50, 100, 50), true);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonSelfIntersectAtVertex()
		{
			// Self intersection at vertex
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(50, 50);
			poly.lineTo(100, 100);
			poly.lineTo(0, 100);
			poly.lineTo(50, 50);
			poly.lineTo(100, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			result = simplifyOp.isSimpleAsFeature(simplifyOp.execute(poly, sr4326, false, null
				), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygon_2EdgesTouchAtVertex()
		{
			// No self-intersection, but more than two edges touch at the same
			// vertex. Simple for ArcGIS, not simple for OGC
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(50, 50);
			poly.lineTo(0, 100);
			poly.lineTo(100, 100);
			poly.lineTo(50, 50);
			poly.lineTo(100, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonTriangle()
		{
			// Triangle
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(100, 100);
			poly.lineTo(100, 0);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonRectangle()
		{
			// Rectangle
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-200, -100, 100, 200), false
				);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonRectangleHoleWrongDirection()
		{
			// Rectangle and rectangular hole that has wrong direction
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-200, -100, 200, 100), false
				);
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-100, -50, 100, 50), false);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygon_2RectanglesSideBySide()
		{
			// Two rectangles side by side, simple
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.addEnvelope(new com.esri.core.geometry.Envelope(-200, -100, 200, 100), false
				);
			poly.addEnvelope(new com.esri.core.geometry.Envelope(220, -50, 300, 50), false);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testIsSimplePolygonRectangleOneBelow()
		{
			// Two rectangles one below another, simple
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.addEnvelope(new com.esri.core.geometry.Envelope(50, 50, 100, 100), false);
			poly.addEnvelope(new com.esri.core.geometry.Envelope(50, 200, 100, 250), false);
			bool result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.reverseAllPaths();
			result = simplifyOp.isSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void testisSimpleOGC()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(10, 0);
			bool result = simplifyOpOGC.isSimpleOGC(poly, sr4326, true, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(10, 10);
			poly.lineTo(0, 10);
			poly.lineTo(10, 0);
			com.esri.core.geometry.NonSimpleResult nsr = new com.esri.core.geometry.NonSimpleResult
				();
			result = simplifyOpOGC.isSimpleOGC(poly, sr4326, true, nsr, null);
			NUnit.Framework.Assert.IsTrue(!result);
			NUnit.Framework.Assert.IsTrue(nsr.m_reason == com.esri.core.geometry.NonSimpleResult.Reason
				.Cracking);
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(0, 0);
			mp.add(10, 0);
			result = simplifyOpOGC.isSimpleOGC(mp, sr4326, true, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			mp = new com.esri.core.geometry.MultiPoint();
			mp.add(10, 0);
			mp.add(10, 0);
			nsr = new com.esri.core.geometry.NonSimpleResult();
			result = simplifyOpOGC.isSimpleOGC(mp, sr4326, true, nsr, null);
			NUnit.Framework.Assert.IsTrue(!result);
			NUnit.Framework.Assert.IsTrue(nsr.m_reason == com.esri.core.geometry.NonSimpleResult.Reason
				.Clustering);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testPolylineIsSimpleForOGC()
		{
			com.esri.core.geometry.OperatorImportFromJson importerJson = (com.esri.core.geometry.OperatorImportFromJson
				)factory.getOperator(com.esri.core.geometry.Operator.Type.ImportFromJson);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)factory.getOperator(com.esri.core.geometry.Operator.Type.Simplify);
			org.codehaus.jackson.JsonFactory f = new org.codehaus.jackson.JsonFactory();
			{
				string s = "{\"paths\":[[[0, 10], [8, 5], [5, 2], [6, 0]]]}";
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				string s = "{\"paths\":[[[0, 10], [6,  0], [7, 5], [0, 3]]]}";
				// self
				// intersection
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				string s = "{\"paths\":[[[0, 10], [6,  0], [0, 3], [0, 10]]]}";
				// closed
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				string s = "{\"paths\":[[[0, 10], [5, 5], [6,  0], [0, 3], [5, 5], [0, 9], [0, 10]]]}";
				// closed
				// with
				// self
				// tangent
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				string s = "{\"paths\":[[[0, 10], [5, 2]], [[5, 2], [6,  0]]]}";
				// two
				// paths
				// connected
				// at
				// a
				// point
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				string s = "{\"paths\":[[[0, 0], [3, 3], [5, 0], [0, 0]], [[0, 10], [3, 3], [10, 10], [0, 10]]]}";
				// two
				// closed
				// rings
				// touch
				// at
				// one
				// point
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				string s = "{\"paths\":[[[0, 0], [10, 10]], [[0, 10], [10, 0]]]}";
				// two
				// lines
				// intersect
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				string s = "{\"paths\":[[[0, 0], [5, 5], [0, 10]], [[10, 10], [5, 5], [10, 0]]]}";
				// two
				// paths
				// share
				// mid
				// point.
				com.esri.core.geometry.Geometry g = importerJson.execute(com.esri.core.geometry.Geometry.Type
					.Unknown, f.createJsonParser(s)).getGeometry();
				bool res = simplifyOpOGC.isSimpleOGC(g, null, true, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void testFillRule()
		{
			//self intersecting star shape
			com.esri.core.geometry.MapGeometry mg = com.esri.core.geometry.OperatorImportFromJson
				.local().execute(com.esri.core.geometry.Geometry.Type.Unknown, "{\"rings\":[[[0,0], [5,10], [10, 0], [0, 7], [10, 7], [0, 0]]]}"
				);
			com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)mg.getGeometry
				();
			NUnit.Framework.Assert.IsTrue(poly.getFillRule() == com.esri.core.geometry.Polygon.FillRule
				.enumFillRuleOddEven);
			poly.setFillRule(com.esri.core.geometry.Polygon.FillRule.enumFillRuleWinding);
			NUnit.Framework.Assert.IsTrue(poly.getFillRule() == com.esri.core.geometry.Polygon.FillRule
				.enumFillRuleWinding);
			com.esri.core.geometry.Geometry simpleResult = com.esri.core.geometry.OperatorSimplify
				.local().execute(poly, null, true, null);
			NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)simpleResult).getFillRule
				() == com.esri.core.geometry.Polygon.FillRule.enumFillRuleOddEven);
			//solid start without holes:
			com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.OperatorImportFromJson
				.local().execute(com.esri.core.geometry.Geometry.Type.Unknown, "{\"rings\":[[[0,0],[2.5925925925925926,5.185185185185185],[0,7],[3.5,7],[5,10],[6.5,7],[10,7],[7.407407407407407,5.185185185185185],[10,0],[5,3.5],[0,0]]]}"
				);
			bool equals = com.esri.core.geometry.OperatorEquals.local().execute(mg1.getGeometry
				(), simpleResult, null, null);
			NUnit.Framework.Assert.IsTrue(equals);
		}
	}
}
