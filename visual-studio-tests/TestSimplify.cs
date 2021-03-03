/*
Copyright 2017-2021 David Raleigh

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

For additional information, contact:

email: davidraleigh@gmail.com
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestSimplify : NUnit.Framework.TestFixtureAttribute
	{
		internal com.epl.geometry.OperatorFactoryLocal factory = null;

		internal com.epl.geometry.OperatorSimplify simplifyOp = null;

		internal com.epl.geometry.OperatorSimplifyOGC simplifyOpOGC = null;

		internal com.epl.geometry.SpatialReference sr102100 = null;

		internal com.epl.geometry.SpatialReference sr4326 = null;

		internal com.epl.geometry.SpatialReference sr3857 = null;

		//import java.io.FileOutputStream;
		//import java.io.PrintStream;
		//import java.util.ArrayList;
		//import java.util.List;
		//import java.util.Random;
		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
			factory = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			simplifyOp = (com.epl.geometry.OperatorSimplify)factory.GetOperator(com.epl.geometry.Operator.Type.Simplify);
			simplifyOpOGC = (com.epl.geometry.OperatorSimplifyOGC)factory.GetOperator(com.epl.geometry.Operator.Type.SimplifyOGC);
			sr102100 = com.epl.geometry.SpatialReference.Create(102100);
			sr3857 = com.epl.geometry.SpatialReference.Create(3857);
			// PE_PCS_WGS_1984_WEB_MERCATOR_AUXSPHERE);
			sr4326 = com.epl.geometry.SpatialReference.Create(4326);
		}

		// enum_value2(SpatialReference,
		// Code, GCS_WGS_1984));
		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon2()
		{
			//MapGeometry mg = OperatorFactoryLocal.loadGeometryFromJSONFileDbg("c:/temp/simplify_polygon_gnomonic.txt");
			//Geometry res = OperatorSimplify.local().execute(mg.getGeometry(), mg.getSpatialReference(), true, null);
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 15);
			poly.LineTo(15, 15);
			poly.LineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.StartPath(5, 5);
			poly.LineTo(5, 6);
			poly.LineTo(6, 6);
			poly.LineTo(6, 5);
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
		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon5()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 0);
			poly.LineTo(0, 0);
			poly.LineTo(5, 5);
			poly.LineTo(10, 10);
			poly.LineTo(0, 10);
			poly.LineTo(5, 5);
			return poly;
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0()
		{
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			poly1.AddEnvelope(new com.epl.geometry.Envelope(10, 10, 40, 20), false);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		// assertTrue(poly1.equals(poly2));
		// done
		[NUnit.Framework.Test]
		public virtual void Test0Poly()
		{
			// simple
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			poly1.AddEnvelope(new com.epl.geometry.Envelope(10, 10, 40, 20), false);
			poly1.AddEnvelope(new com.epl.geometry.Envelope(50, 10, 100, 20), false);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		// assertTrue(poly1.equals(poly2));
		// done
		[NUnit.Framework.Test]
		public virtual void Test0Polygon_Spike1()
		{
			// non-simple (spike)
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			poly1.StartPath(10, 10);
			poly1.LineTo(10, 20);
			poly1.LineTo(40, 20);
			poly1.LineTo(40, 10);
			poly1.LineTo(60, 10);
			poly1.LineTo(70, 10);
			bool res = simplifyOp.IsSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(poly2.GetPointCount() == 4);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0Polygon_Spike2()
		{
			// non-simple (spikes)
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			// rectangle with a spike
			poly1.StartPath(10, 10);
			poly1.LineTo(10, 20);
			poly1.LineTo(40, 20);
			poly1.LineTo(40, 10);
			poly1.LineTo(60, 10);
			poly1.LineTo(70, 10);
			// degenerate
			poly1.StartPath(100, 100);
			poly1.LineTo(100, 120);
			poly1.LineTo(100, 130);
			bool res = simplifyOp.IsSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(poly2.GetPointCount() == 4);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0Polygon_Spike3()
		{
			// non-simple (spikes)
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			// degenerate
			poly1.StartPath(100, 100);
			poly1.LineTo(100, 120);
			poly1.LineTo(100, 130);
			bool res = simplifyOp.IsSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonSelfIntersect1()
		{
			// non-simple (self-intersection)
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			// touch uncracked
			poly1.StartPath(0, 0);
			poly1.LineTo(0, 100);
			poly1.LineTo(100, 100);
			poly1.LineTo(0, 50);
			poly1.LineTo(100, 0);
			bool res = simplifyOp.IsSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonSelfIntersect2()
		{
			// non-simple (self-intersection)
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			poly1.StartPath(0, 0);
			poly1.LineTo(0, 100);
			poly1.LineTo(100, 100);
			poly1.LineTo(-100, 0);
			// poly1.lineTo(100, 0);
			bool res = simplifyOp.IsSimpleAsFeature(poly1, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly1, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonSelfIntersect3()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 15);
			poly.LineTo(15, 15);
			poly.LineTo(15, 0);
			// This part intersects with the first part
			poly.StartPath(10, 10);
			poly.LineTo(10, 20);
			poly.LineTo(20, 20);
			poly.LineTo(20, 10);
			bool res = simplifyOp.IsSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonInteriorRing1()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 15);
			poly.LineTo(15, 15);
			poly.LineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.StartPath(5, 5);
			poly.LineTo(5, 6);
			poly.LineTo(6, 6);
			poly.LineTo(6, 5);
			bool res = simplifyOp.IsSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonInteriorRing2()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 15);
			poly.LineTo(15, 15);
			poly.LineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.StartPath(5, 5);
			poly.LineTo(5, 6);
			poly.LineTo(6, 6);
			poly.LineTo(6, 5);
			// This part intersects with the first part
			poly.StartPath(10, 10);
			poly.LineTo(10, 20);
			poly.LineTo(20, 20);
			poly.LineTo(20, 10);
			bool res = simplifyOp.IsSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonInteriorRingWithCommonBoundary1()
		{
			// Two rings have common boundary
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.StartPath(10, 0);
			poly.LineTo(10, 10);
			poly.LineTo(20, 10);
			poly.LineTo(20, 0);
			bool res = simplifyOp.IsSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void Test0PolygonInteriorRingWithCommonBoundary2()
		{
			// Two rings have common boundary
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.StartPath(10, 5);
			poly.LineTo(10, 6);
			poly.LineTo(20, 6);
			poly.LineTo(20, 5);
			bool res = simplifyOp.IsSimpleAsFeature(poly, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)simplifyOp.Execute(poly, null, false, null);
			res = simplifyOp.IsSimpleAsFeature(poly2, null, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolygon()
		{
			com.epl.geometry.Polygon nonSimplePolygon = MakeNonSimplePolygon();
			com.epl.geometry.Polygon simplePolygon = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon, sr3857, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			int partCount = simplePolygon.GetPathCount();
			// assertTrue(partCount == 2);
			double area = simplePolygon.CalculateRingArea2D(0);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 300) <= 0.0001);
			area = simplePolygon.CalculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - (-25.0)) <= 0.0001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolygon2()
		{
			com.epl.geometry.Polygon nonSimplePolygon2 = MakeNonSimplePolygon2();
			double area = nonSimplePolygon2.CalculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 1.0) <= 0.0001);
			com.epl.geometry.Polygon simplePolygon2 = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon2, sr3857, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon2, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			area = simplePolygon2.CalculateRingArea2D(0);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 225) <= 0.0001);
			area = simplePolygon2.CalculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - (-1.0)) <= 0.0001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolygon3()
		{
			com.epl.geometry.Polygon nonSimplePolygon3 = MakeNonSimplePolygon3();
			com.epl.geometry.Polygon simplePolygon3 = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon3, sr3857, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon3, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			double area = simplePolygon3.CalculateRingArea2D(0);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 875) <= 0.0001);
			area = simplePolygon3.CalculateRingArea2D(1);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - (-225)) <= 0.0001 || System.Math.Abs(area - (-50.0)) <= 0.0001);
			area = simplePolygon3.CalculateRingArea2D(2);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - (-225)) <= 0.0001 || System.Math.Abs(area - (-50.0)) <= 0.0001);
			area = simplePolygon3.CalculateRingArea2D(3);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 25) <= 0.0001);
			area = simplePolygon3.CalculateRingArea2D(4);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 25) <= 0.0001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolyline()
		{
			com.epl.geometry.Polyline nonSimplePolyline = MakeNonSimplePolyline();
			com.epl.geometry.Polyline simplePolyline = (com.epl.geometry.Polyline)simplifyOp.Execute(nonSimplePolyline, sr3857, false, null);
			int segmentCount = simplePolyline.GetSegmentCount();
			NUnit.Framework.Assert.IsTrue(segmentCount == 4);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolygon4()
		{
			com.epl.geometry.Polygon nonSimplePolygon4 = MakeNonSimplePolygon4();
			com.epl.geometry.Polygon simplePolygon4 = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon4, sr3857, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon4, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			NUnit.Framework.Assert.IsTrue(simplePolygon4.GetPointCount() == 5);
			com.epl.geometry.Point point = nonSimplePolygon4.GetPoint(0);
			NUnit.Framework.Assert.IsTrue(point.GetX() == 0.0 && point.GetY() == 0.0);
			point = nonSimplePolygon4.GetPoint(1);
			NUnit.Framework.Assert.IsTrue(point.GetX() == 0.0 && point.GetY() == 10.0);
			point = nonSimplePolygon4.GetPoint(2);
			NUnit.Framework.Assert.IsTrue(point.GetX() == 10.0 && point.GetY() == 10.0);
			point = nonSimplePolygon4.GetPoint(3);
			NUnit.Framework.Assert.IsTrue(point.GetX() == 10.0 && point.GetY() == 0.0);
			point = nonSimplePolygon4.GetPoint(4);
			NUnit.Framework.Assert.IsTrue(point.GetX() == 5.0 && point.GetY() == 0.0);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolygon5()
		{
			com.epl.geometry.Polygon nonSimplePolygon5 = MakeNonSimplePolygon5();
			com.epl.geometry.Polygon simplePolygon5 = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon5, sr3857, false, null);
			NUnit.Framework.Assert.IsTrue(simplePolygon5 != null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon5, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
			int pointCount = simplePolygon5.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 6);
			double area = simplePolygon5.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 50.0) <= 0.001);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestPolygon6()
		{
			com.epl.geometry.Polygon nonSimplePolygon6 = MakeNonSimplePolygon6();
			com.epl.geometry.Polygon simplePolygon6 = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon6, sr3857, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon6, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygon7()
		{
			com.epl.geometry.Polygon nonSimplePolygon7 = MakeNonSimplePolygon7();
			com.epl.geometry.Polygon simplePolygon7 = (com.epl.geometry.Polygon)simplifyOp.Execute(nonSimplePolygon7, sr3857, false, null);
			bool res = simplifyOp.IsSimpleAsFeature(simplePolygon7, sr3857, true, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 15);
			poly.LineTo(15, 15);
			poly.LineTo(15, 0);
			// This is an interior ring but it is clockwise
			poly.StartPath(5, 5);
			poly.LineTo(5, 6);
			poly.LineTo(6, 6);
			poly.LineTo(6, 5);
			// This part intersects with the first part
			poly.StartPath(10, 10);
			poly.LineTo(10, 20);
			poly.LineTo(20, 20);
			poly.LineTo(20, 10);
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
		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon3()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 25);
			poly.LineTo(35, 25);
			poly.LineTo(35, 0);
			poly.StartPath(5, 5);
			poly.LineTo(5, 15);
			poly.LineTo(10, 15);
			poly.LineTo(10, 5);
			poly.StartPath(40, 0);
			poly.LineTo(45, 0);
			poly.LineTo(45, 5);
			poly.LineTo(40, 5);
			poly.StartPath(20, 10);
			poly.LineTo(25, 10);
			poly.LineTo(25, 15);
			poly.LineTo(20, 15);
			poly.StartPath(15, 5);
			poly.LineTo(15, 20);
			poly.LineTo(30, 20);
			poly.LineTo(30, 5);
			return poly;
		}

		// done
		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon4()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.LineTo(5, 0);
			poly.LineTo(5, 5);
			poly.LineTo(5, 0);
			return poly;
		}

		// done
		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon6()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(35.34407570857744, 54.00551247713412);
			poly.LineTo(41.07663499357954, 20.0);
			poly.LineTo(40.66372033705177, 26.217432321849017);
			poly.StartPath(42.81936574509338, 20.0);
			poly.LineTo(43.58226670584747, 20.0);
			poly.LineTo(39.29611825817084, 22.64634933678729);
			poly.LineTo(44.369873312241346, 25.81893670527215);
			poly.LineTo(42.68845660737179, 20.0);
			poly.LineTo(38.569549792944244, 56.47456192829393);
			poly.LineTo(42.79274114188401, 45.45117792578003);
			poly.LineTo(41.09512147544657, 70.0);
			return poly;
		}

		public virtual com.epl.geometry.Polygon MakeNonSimplePolygon7()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(41.987895433319686, 53.75822619011542);
			poly.LineTo(41.98789542535497, 53.75822618803151);
			poly.LineTo(40.15120412113667, 68.12604154722113);
			poly.LineTo(37.72272697311022, 67.92767094118877);
			poly.LineTo(37.147347454283086, 49.497473094145505);
			poly.LineTo(38.636627026664385, 51.036687142232736);
			poly.StartPath(39.00920080789793, 62.063425518369016);
			poly.LineTo(38.604912643136885, 70.0);
			poly.LineTo(40.71826863485308, 43.60337143116787);
			poly.LineTo(35.34407570857744, 54.005512477134126);
			poly.LineTo(39.29611825817084, 22.64634933678729);
			return poly;
		}

		public virtual com.epl.geometry.Polyline MakeNonSimplePolyline()
		{
			// This polyline has a short segment
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(10, 0);
			poly.LineTo(10, 10);
			poly.LineTo(10, 5);
			poly.LineTo(-5, 5);
			return poly;
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleBasicsPoint()
		{
			bool result;
			// point is always simple
			com.epl.geometry.Point pt = new com.epl.geometry.Point();
			result = simplifyOp.IsSimpleAsFeature(pt, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			pt.SetXY(0, 0);
			result = simplifyOp.IsSimpleAsFeature(pt, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			pt.SetXY(100000, 10000);
			result = simplifyOp.IsSimpleAsFeature(pt, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleBasicsEnvelope()
		{
			// Envelope is simple, when it's width and height are not degenerate
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			bool result = simplifyOp.IsSimpleAsFeature(env, sr4326, false, null, null);
			// Empty is simple
			NUnit.Framework.Assert.IsTrue(result);
			env.SetCoords(0, 0, 10, 10);
			result = simplifyOp.IsSimpleAsFeature(env, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// sliver but still simple
			env.SetCoords(0, 0, 0 + sr4326.GetTolerance() * 2, 10);
			result = simplifyOp.IsSimpleAsFeature(env, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// sliver and not simple
			env.SetCoords(0, 0, 0 + sr4326.GetTolerance() * 0.5, 10);
			result = simplifyOp.IsSimpleAsFeature(env, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleBasicsLine()
		{
			com.epl.geometry.Line line = new com.epl.geometry.Line();
			bool result = simplifyOp.IsSimpleAsFeature(line, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			line.SetStart(new com.epl.geometry.Point(0, 0));
			// line.setEndXY(0, 0);
			result = simplifyOp.IsSimpleAsFeature(line, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			line.SetEnd(new com.epl.geometry.Point(1, 0));
			result = simplifyOp.IsSimpleAsFeature(line, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPoint1()
		{
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// empty is simple
			result = simplifyOp.IsSimpleAsFeature(simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPoint2FarApart()
		{
			// Two point test: far apart
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(20, 10);
			mp.Add(100, 100);
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.IsSimpleAsFeature(simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mp.GetPointCount() == 2);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPointCoincident()
		{
			// Two point test: coincident
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(100, 100);
			mp.Add(100, 100);
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.MultiPoint mpS;
			result = simplifyOp.IsSimpleAsFeature(mpS = (com.epl.geometry.MultiPoint)simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.GetPointCount() == 1);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestMultiPointSR4326_CR184439()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorSimplify simpOp = (com.epl.geometry.OperatorSimplify)engine.GetOperator(com.epl.geometry.Operator.Type.Simplify);
			com.epl.geometry.NonSimpleResult nonSimpResult = new com.epl.geometry.NonSimpleResult();
			nonSimpResult.m_reason = com.epl.geometry.NonSimpleResult.Reason.NotDetermined;
			com.epl.geometry.MultiPoint multiPoint = new com.epl.geometry.MultiPoint();
			multiPoint.Add(0, 0);
			multiPoint.Add(0, 1);
			multiPoint.Add(0, 0);
			bool multiPointIsSimple = simpOp.IsSimpleAsFeature(multiPoint, com.epl.geometry.SpatialReference.Create(4326), true, nonSimpResult, null);
			NUnit.Framework.Assert.IsFalse(multiPointIsSimple);
			NUnit.Framework.Assert.IsTrue(nonSimpResult.m_reason == com.epl.geometry.NonSimpleResult.Reason.Clustering);
			NUnit.Framework.Assert.IsTrue(nonSimpResult.m_vertexIndex1 == 0);
			NUnit.Framework.Assert.IsTrue(nonSimpResult.m_vertexIndex2 == 2);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPointCloserThanTolerance()
		{
			// Two point test: closer than tolerance
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint mpS;
			mp.Add(100, 100);
			mp.Add(100, 100 + sr4326.GetTolerance() * .5);
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.IsSimpleAsFeature(mpS = (com.epl.geometry.MultiPoint)simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.GetPointCount() == 2);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPointFarApart2()
		{
			// 5 point test: far apart
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(100, 100);
			mp.Add(100, 101);
			mp.Add(101, 101);
			mp.Add(11, 1);
			mp.Add(11, 14);
			com.epl.geometry.MultiPoint mpS;
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.IsSimpleAsFeature(mpS = (com.epl.geometry.MultiPoint)simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.GetPointCount() == 5);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPoint_coincident2()
		{
			// 5 point test: coincident
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(100, 100);
			mp.Add(100, 101);
			mp.Add(100, 100);
			mp.Add(11, 1);
			mp.Add(11, 14);
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.MultiPoint mpS;
			result = simplifyOp.IsSimpleAsFeature(mpS = (com.epl.geometry.MultiPoint)simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.GetPointCount() == 4);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(0).GetX(), 100, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(0).GetY(), 100, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(1).GetX(), 100, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(1).GetY(), 101, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(2).GetX(), 11, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(2).GetY(), 1, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(3).GetX(), 11, 1e-7);
			NUnit.Framework.Assert.AreEqual(mpS.GetPoint(3).GetY(), 14, 1e-7);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimpleMultiPointCloserThanTolerance2()
		{
			// 5 point test: closer than tolerance
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(100, 100);
			mp.Add(100, 101);
			mp.Add(100, 100 + sr4326.GetTolerance() / 2);
			mp.Add(11, 1);
			mp.Add(11, 14);
			com.epl.geometry.MultiPoint mpS;
			bool result = simplifyOp.IsSimpleAsFeature(mp, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			result = simplifyOp.IsSimpleAsFeature(mpS = (com.epl.geometry.MultiPoint)simplifyOp.Execute(mp, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(mpS.GetPointCount() == 5);
		}

		// done
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// empty is simple
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineFarApart()
		{
			// Two point test: far apart
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(20, 10);
			poly.LineTo(100, 100);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineCoincident()
		{
			// Two point test: coincident
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(100, 100);
			poly.LineTo(100, 100);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.Polyline polyS;
			result = simplifyOp.IsSimpleAsFeature(polyS = (com.epl.geometry.Polyline)simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineCloserThanTolerance()
		{
			// Two point test: closer than tolerance
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(100, 100);
			poly.LineTo(100, 100 + sr4326.GetTolerance() / 2);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.Polyline polyS;
			result = simplifyOp.IsSimpleAsFeature(polyS = (com.epl.geometry.Polyline)simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineFarApartSelfOverlap0()
		{
			// 3 point test: far apart, self overlapping
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(0, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineSelfIntersect()
		{
			// 4 point test: far apart, self intersecting
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(0, 100);
			poly.LineTo(100, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineDegenerateSegment()
		{
			// 4 point test: degenerate segment
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(100, 100 + sr4326.GetTolerance() / 2);
			poly.LineTo(100, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.Polyline polyS;
			result = simplifyOp.IsSimpleAsFeature(polyS = (com.epl.geometry.Polyline)simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			{
				com.epl.geometry.Polyline other = new com.epl.geometry.Polyline();
				other.StartPath(0, 0);
				other.LineTo(100, 100);
				other.LineTo(100, 0);
				other.Equals(poly);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineFarApartSelfOverlap()
		{
			// 3 point test: far apart, self overlapping
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(0, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineFarApartIntersect()
		{
			// 4 point 2 parts test: far apart, intersecting parts
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.StartPath(100, 0);
			poly.LineTo(0, 100);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineFarApartOverlap2()
		{
			// 4 point 2 parts test: far apart, overlapping parts. second part
			// starts where first one ends
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.StartPath(100, 100);
			poly.LineTo(0, 100);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// TO CONFIRM should be false
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineDegenerateVertical()
		{
			// 3 point test: degenerate vertical line
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(new com.epl.geometry.Point(100, 100));
			poly.LineTo(new com.epl.geometry.Point(100, 100));
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.Polyline polyS;
			result = simplifyOp.IsSimpleAsFeature(polyS = (com.epl.geometry.Polyline)simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(polyS.GetPointCount() == 2);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolylineEmptyPath()
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
		public virtual void TestIsSimplePolylineSinglePointInPath()
		{
			// Single point in path
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.RemovePoint(0, 1);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			com.epl.geometry.Polyline polyS;
			result = simplifyOp.IsSimpleAsFeature(polyS = (com.epl.geometry.Polyline)simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			NUnit.Framework.Assert.IsTrue(polyS.IsEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygon()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			// empty is simple
			result = simplifyOp.IsSimpleAsFeature(simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		// empty is simple
		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonEmptyPath()
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
		public virtual void TestIsSimplePolygonIncomplete1()
		{
			// Incomplete polygon 1
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			// poly.removePoint(0, 1);//TO CONFIRM no removePoint method in Java
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonIncomplete2()
		{
			// Incomplete polygon 2
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonDegenerateTriangle()
		{
			// Degenerate triangle (self overlap)
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(0, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonSelfIntersect()
		{
			// Self intersection - cracking is needed
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(0, 100);
			poly.LineTo(100, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonRectangleHole()
		{
			// Rectangle and rectangular hole that has one segment overlapping
			// with the with the exterior ring. Cracking is needed.
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(-200, -100, 200, 100), false);
			poly.AddEnvelope(new com.epl.geometry.Envelope(-100, -100, 100, 50), true);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonRectangleHole2()
		{
			// Rectangle and rectangular hole
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(-200, -100, 200, 100), false);
			poly.AddEnvelope(new com.epl.geometry.Envelope(-100, -50, 100, 50), true);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonSelfIntersectAtVertex()
		{
			// Self intersection at vertex
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(50, 50);
			poly.LineTo(100, 100);
			poly.LineTo(0, 100);
			poly.LineTo(50, 50);
			poly.LineTo(100, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			result = simplifyOp.IsSimpleAsFeature(simplifyOp.Execute(poly, sr4326, false, null), sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygon_2EdgesTouchAtVertex()
		{
			// No self-intersection, but more than two edges touch at the same
			// vertex. Simple for ArcGIS, not simple for OGC
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(50, 50);
			poly.LineTo(0, 100);
			poly.LineTo(100, 100);
			poly.LineTo(50, 50);
			poly.LineTo(100, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonTriangle()
		{
			// Triangle
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(100, 100);
			poly.LineTo(100, 0);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonRectangle()
		{
			// Rectangle
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(-200, -100, 100, 200), false);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonRectangleHoleWrongDirection()
		{
			// Rectangle and rectangular hole that has wrong direction
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(-200, -100, 200, 100), false);
			poly.AddEnvelope(new com.epl.geometry.Envelope(-100, -50, 100, 50), false);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygon_2RectanglesSideBySide()
		{
			// Two rectangles side by side, simple
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(-200, -100, 200, 100), false);
			poly.AddEnvelope(new com.epl.geometry.Envelope(220, -50, 300, 50), false);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSimplePolygonRectangleOneBelow()
		{
			// Two rectangles one below another, simple
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(50, 50, 100, 100), false);
			poly.AddEnvelope(new com.epl.geometry.Envelope(50, 200, 100, 250), false);
			bool result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(result);
			poly.ReverseAllPaths();
			result = simplifyOp.IsSimpleAsFeature(poly, sr4326, false, null, null);
			NUnit.Framework.Assert.IsTrue(!result);
		}

		[NUnit.Framework.Test]
		public virtual void TestisSimpleOGC()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(10, 0);
			bool result = simplifyOpOGC.IsSimpleOGC(poly, sr4326, true, null, null);
			NUnit.Framework.Assert.IsTrue(result);
//			poly = new com.epl.geometry.Polyline();
//			poly.StartPath(0, 0);
//			poly.LineTo(10, 10);
//			poly.LineTo(0, 10);
//			poly.LineTo(10, 0);
//			com.epl.geometry.NonSimpleResult nsr = new com.epl.geometry.NonSimpleResult();
//			result = simplifyOpOGC.IsSimpleOGC(poly, sr4326, true, nsr, null);
//			NUnit.Framework.Assert.IsTrue(!result);
//			NUnit.Framework.Assert.IsTrue(nsr.m_reason == com.epl.geometry.NonSimpleResult.Reason.Cracking);
//			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
//			mp.Add(0, 0);
//			mp.Add(10, 0);
//			result = simplifyOpOGC.IsSimpleOGC(mp, sr4326, true, null, null);
//			NUnit.Framework.Assert.IsTrue(result);
//			mp = new com.epl.geometry.MultiPoint();
//			mp.Add(10, 0);
//			mp.Add(10, 0);
//			nsr = new com.epl.geometry.NonSimpleResult();
//			result = simplifyOpOGC.IsSimpleOGC(mp, sr4326, true, nsr, null);
//			NUnit.Framework.Assert.IsTrue(!result);
//			NUnit.Framework.Assert.IsTrue(nsr.m_reason == com.epl.geometry.NonSimpleResult.Reason.Clustering);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolylineIsSimpleForOGC()
//		{
//			com.epl.geometry.OperatorImportFromJson importerJson = (com.epl.geometry.OperatorImportFromJson)factory.GetOperator(com.epl.geometry.Operator.Type.ImportFromJson);
//			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)factory.GetOperator(com.epl.geometry.Operator.Type.Simplify);
//			{
//				string s = "{\"paths\":[[[0, 10], [8, 5], [5, 2], [6, 0]]]}";
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 10], [6,  0], [7, 5], [0, 3]]]}";
//				// self
//				// intersection
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 10], [6,  0], [0, 3], [0, 10]]]}";
//				// closed
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 10], [5, 5], [6,  0], [0, 3], [5, 5], [0, 9], [0, 10]]]}";
//				// closed
//				// with
//				// self
//				// tangent
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 10], [5, 2]], [[5, 2], [6,  0]]]}";
//				// two
//				// paths
//				// connected
//				// at
//				// a
//				// point
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 0], [3, 3], [5, 0], [0, 0]], [[0, 10], [3, 3], [10, 10], [0, 10]]]}";
//				// two
//				// closed
//				// rings
//				// touch
//				// at
//				// one
//				// point
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 0], [10, 10]], [[0, 10], [10, 0]]]}";
//				// two
//				// lines
//				// intersect
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				string s = "{\"paths\":[[[0, 0], [5, 5], [0, 10]], [[10, 10], [5, 5], [10, 0]]]}";
//				// two
//				// paths
//				// share
//				// mid
//				// point.
//				com.epl.geometry.Geometry g = importerJson.Execute(com.epl.geometry.Geometry.Type.Unknown, com.epl.geometry.JsonParserReader.CreateFromString(s)).GetGeometry();
//				bool res = simplifyOpOGC.IsSimpleOGC(g, null, true, null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
		}

//		[NUnit.Framework.Test]
//		public virtual void TestFillRule()
//		{
//			//self intersecting star shape
//			com.epl.geometry.MapGeometry mg = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, "{\"rings\":[[[0,0], [5,10], [10, 0], [0, 7], [10, 7], [0, 0]]]}");
//			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)mg.GetGeometry();
//			NUnit.Framework.Assert.IsTrue(poly.GetFillRule() == com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
//			poly.SetFillRule(com.epl.geometry.Polygon.FillRule.enumFillRuleWinding);
//			NUnit.Framework.Assert.IsTrue(poly.GetFillRule() == com.epl.geometry.Polygon.FillRule.enumFillRuleWinding);
//			com.epl.geometry.Geometry simpleResult = com.epl.geometry.OperatorSimplify.Local().Execute(poly, null, true, null);
//			NUnit.Framework.Assert.IsTrue(((com.epl.geometry.Polygon)simpleResult).GetFillRule() == com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
//			//solid start without holes:
//			com.epl.geometry.MapGeometry mg1 = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, "{\"rings\":[[[0,0],[2.5925925925925926,5.185185185185185],[0,7],[3.5,7],[5,10],[6.5,7],[10,7],[7.407407407407407,5.185185185185185],[10,0],[5,3.5],[0,0]]]}"
//				);
//			bool equals = com.epl.geometry.OperatorEquals.Local().Execute(mg1.GetGeometry(), simpleResult, null, null);
//			NUnit.Framework.Assert.IsTrue(equals);
//		}
	}
}
