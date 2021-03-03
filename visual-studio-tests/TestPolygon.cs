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
	public class TestPolygon : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestCreation()
		{
			// simple create
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			int number = poly.GetStateFlag();
			NUnit.Framework.Assert.IsTrue(poly != null);
			// assertTrue(poly.getClass() == Polygon.class);
			// assertFalse(poly.getClass() == Envelope.class);
			NUnit.Framework.Assert.IsTrue(poly.GetType() == com.epl.geometry.Geometry.Type.Polygon);
			NUnit.Framework.Assert.IsTrue(poly.IsEmpty());
			NUnit.Framework.Assert.IsTrue(poly.GetPointCount() == 0);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 0);
			number = poly.GetStateFlag();
			poly = null;
			NUnit.Framework.Assert.IsFalse(poly != null);
			// play with default attributes
			com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
		}

		// SimpleTest(poly2);
		// creation1();
		// creation2();
		// addpath();
		// addpath2();
		// removepath();
		// reversepath();
		// reverseallpaths();
		// openallpaths();
		// openpath();
		// insertpath();
		// insertpoints();
		// insertpoint();
		// removepoint();
		// insertpointsfromaray();
		// createWithStreams();
		// testBug1();
		[NUnit.Framework.Test]
		public virtual void TestCreation1()
		{
			// Simple area and length calcul test
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			int number = poly.GetStateFlag();
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(1000, 2000, 1010, 2010);
			env.ToString();
			poly.AddEnvelope(env, false);
			poly.ToString();
			number = poly.GetStateFlag();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(poly.CalculateArea2D() - 100) < 1e-12);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(poly.CalculateLength2D() - 40) < 1e-12);
			poly.SetEmpty();
			number = poly.GetStateFlag();
			poly.AddEnvelope(env, true);
			number = poly.GetStateFlag();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(poly.CalculateArea2D() + 100) < 1e-12);
			number = poly.GetStateFlag();
		}

		[NUnit.Framework.Test]
		public virtual void TestCreation2()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			int state1 = poly.GetStateFlag();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.ClosePathWithLine();
			int state2 = poly.GetStateFlag();
			NUnit.Framework.Assert.IsTrue(state2 == state1 + 1);
			// MultiPathImpl::Pointer mpImpl =
			// (MultiPathImpl::Pointer)poly->_GetImpl();
			//
			// assertTrue(mpImpl.getPointCount() == 4);
			// assertTrue(mpImpl.getPathCount() == 1);
			// AttributeStreamBase xy =
			// mpImpl.getAttributeStreamRef(enum_value2(VertexDescription,
			// Semantics, POSITION));
			// double x, y;
			// x = xy.readAsDbl(2 * 2);
			// y = xy.readAsDbl(2 * 2 + 1);
			// assertTrue(x == 30); assertTrue(y == 14);
			//
			// AttributeStreamOfIndexType parts = mpImpl.getPathStreamRef();
			// assertTrue(parts.size() == 2);
			// assertTrue(parts.read(0) == 0);
			// assertTrue(parts.read(1) == 4);
			// assertTrue(mpImpl.isClosedPath(0));
			// assertTrue(mpImpl.getSegmentFlagsStreamRef() == NULLPTR);
			// assertTrue(mpImpl.getSegmentIndexStreamRef() == NULLPTR);
			// assertTrue(mpImpl.getSegmentDataStreamRef() == NULLPTR);
			poly.StartPath(20, 13);
			poly.LineTo(150, 120);
			poly.LineTo(300, 414);
			poly.LineTo(610, 14);
			poly.LineTo(6210, 140);
			poly.ClosePathWithLine();
			// assertTrue(mpImpl.getPointCount() == 9);
			// assertTrue(mpImpl.getPathCount() == 2);
			// assertTrue(mpImpl.isClosedPath(1));
			// xy = mpImpl.getAttributeStreamRef(enum_value2(VertexDescription,
			// Semantics, POSITION));
			// x = xy.readAsDbl(2 * 3);
			// y = xy.readAsDbl(2 * 3 + 1);
			// assertTrue(x == 60); assertTrue(y == 144);
			//
			// x = xy.readAsDbl(2 * 6);
			// y = xy.readAsDbl(2 * 6 + 1);
			// assertTrue(x == 300); assertTrue(y == 414);
			// parts = mpImpl.getPathStreamRef();
			// assertTrue(parts.size() == 3);
			// assertTrue(parts.read(0) == 0);
			// assertTrue(parts.read(1) == 4);
			// assertTrue(parts.read(2) == 9);
			// assertTrue(mpImpl.getSegmentIndexStreamRef() == NULLPTR);
			// assertTrue(mpImpl.getSegmentFlagsStreamRef() == NULLPTR);
			// assertTrue(mpImpl.getSegmentDataStreamRef() == NULLPTR);
			poly.StartPath(200, 1333);
			poly.LineTo(1150, 1120);
			poly.LineTo(300, 4114);
			poly.LineTo(6110, 114);
			poly.LineTo(61210, 1140);
			NUnit.Framework.Assert.IsTrue(poly.IsClosedPath(2) == true);
			poly.CloseAllPaths();
			NUnit.Framework.Assert.IsTrue(poly.IsClosedPath(2) == true);
			{
				com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
				poly2.StartPath(10, 10);
				poly2.LineTo(100, 10);
				poly2.LineTo(100, 100);
				poly2.LineTo(10, 100);
			}
			{
				com.epl.geometry.Polygon poly3 = new com.epl.geometry.Polygon();
				// create a star (non-simple)
				poly3.StartPath(1, 0);
				poly3.LineTo(5, 10);
				poly3.LineTo(9, 0);
				poly3.LineTo(0, 6);
				poly3.LineTo(10, 6);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestCreateWithStreams()
		{
		}

		// Polygon poly = new Polygon();
		// poly.addAttribute((int)Semantics.M);
		// try
		// {
		// OutputDebugString(L"Test an assert\n");
		// GeometryException::m_assertOnException = false;
		// ((MultiPathImpl::Pointer)poly->_GetImpl()).getPathStreamRef();
		// }
		// catch(GeometryException except)
		// {
		// assertTrue(except->empty_geometry);
		// GeometryException::m_assertOnException = true;
		// }
		// try
		// {
		// OutputDebugString(L"Test an assert\n");
		// GeometryException::m_assertOnException = false;
		// ((MultiPathImpl::Pointer)poly->_GetImpl()).getAttributeStreamRef(enum_value2(VertexDescription,
		// Semantics, POSITION));
		// }
		// catch(GeometryException except)
		// {
		// assertTrue(except->empty_geometry);
		// GeometryException::m_assertOnException = true;
		// }
		//
		// MultiPathImpl::Pointer mpImpl =
		// (MultiPathImpl::Pointer)poly->_GetImpl();
		//
		// AttributeStreamOfIndexType parts =
		// (AttributeStreamOfIndexType)AttributeStreamBase::CreateIndexStream(3);
		// mpImpl.setPathStreamRef(parts);
		//
		// parts.write(0, 0); //first element is always 0
		// parts.write(1, 4); //second element is the index of the first vertex
		// of the second part
		// parts.write(2, 8); //the third element is the total point count.
		//
		// AttributeStreamOfInt8 flags =
		// (AttributeStreamOfInt8)AttributeStreamBase::CreateByteStream(3);
		// mpImpl.setPathFlagsStreamRef(flags);
		// flags.write(0, enum_value1(PathFlags, enumClosed));
		// flags.write(1, enum_value1(PathFlags, enumClosed));
		// flags.write(2, 0);
		//
		// AttributeStreamOfDbl xy =
		// (AttributeStreamOfDbl)AttributeStreamBase::CreateDoubleStream(16);
		// //16 doubles means 8 points
		// mpImpl.setAttributeStreamRef(enum_value2(VertexDescription,
		// Semantics, POSITION), xy);
		//
		// Envelope2D env;
		// env.SetCoords(-1000, -2000, 1000, 2000);
		// Point2D buf[4];
		// env.QueryCorners(buf);
		// xy.writeRange(0, 8, (double*)buf, 0, true);
		//
		// env.SetCoords(-100, -200, 100, 200);
		// env.QueryCornersReversed(buf); //make a hole by quering reversed
		// order
		// xy.writeRange(8, 8, (double*)buf, 0, true);
		//
		// mpImpl.notifyModified(MultiVertexGeometryImpl::DirtyAll); //notify
		// the path that the vertices had changed.
		//
		// assertTrue(poly.getPointCount() == 8);
		// assertTrue(poly.getPathCount() == 2);
		// assertTrue(poly.getPathSize(1) == 4);
		// assertTrue(poly.isClosedPath(0));
		// assertTrue(poly.isClosedPath(1));
		[NUnit.Framework.Test]
		public virtual void TestCloneStuff()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			poly.ClosePathWithLine();
			com.epl.geometry.Polygon clone = (com.epl.geometry.Polygon)poly.Copy();
			NUnit.Framework.Assert.IsTrue(clone.GetPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(clone.GetPathStart(2) == 8);
			NUnit.Framework.Assert.IsTrue(clone.IsClosedPath(0));
			NUnit.Framework.Assert.IsTrue(clone.IsClosedPath(1));
			NUnit.Framework.Assert.IsTrue(clone.IsClosedPath(2));
			NUnit.Framework.Assert.IsTrue(clone.GetXY(5).IsEqual(new com.epl.geometry.Point2D(15, 20)));
		}

		[NUnit.Framework.Test]
		public virtual void TestCloneStuffEnvelope()
		{
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(11, 12, 15, 24);
			com.epl.geometry.Envelope eCopy = (com.epl.geometry.Envelope)env.Copy();
			NUnit.Framework.Assert.IsTrue(eCopy.Equals(env));
			NUnit.Framework.Assert.IsTrue(eCopy.GetXMin() == 11);
			NUnit.Framework.Assert.IsTrue(eCopy.GetYMin() == 12);
			NUnit.Framework.Assert.IsTrue(eCopy.GetXMax() == 15);
			NUnit.Framework.Assert.IsTrue(eCopy.GetYMax() == 24);
		}

		[NUnit.Framework.Test]
		public virtual void TestCloneStuffPolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			poly.ClosePathWithLine();
			com.epl.geometry.Polyline clone = (com.epl.geometry.Polyline)poly.Copy();
			NUnit.Framework.Assert.IsTrue(clone.GetPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(clone.GetPathStart(2) == 8);
			NUnit.Framework.Assert.IsTrue(!clone.IsClosedPath(0));
			NUnit.Framework.Assert.IsTrue(!clone.IsClosedPath(1));
			NUnit.Framework.Assert.IsTrue(clone.IsClosedPath(2));
			NUnit.Framework.Assert.IsTrue(clone.GetXY(5).IsEqual(new com.epl.geometry.Point2D(15, 20)));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddpath()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			poly1.AddPath(poly, 2, true);
			poly1.AddPath(poly, 0, true);
			NUnit.Framework.Assert.IsTrue(poly1.GetPathCount() == 2);
			NUnit.Framework.Assert.IsTrue(poly1.GetPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(0));
			NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(1));
			com.epl.geometry.Point ptOut = poly1.GetPoint(6);
			NUnit.Framework.Assert.IsTrue(ptOut.GetX() == 30 && ptOut.GetY() == 14);
		}

		[NUnit.Framework.Test]
		public virtual void TestAddpath2()
		{
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			polygon.StartPath(-179, 34);
			polygon.LineTo(-154, 34);
			polygon.LineTo(-179, 36);
			polygon.LineTo(-180, 90);
			polygon.LineTo(180, 90);
			polygon.LineTo(180, 36);
			polygon.LineTo(70, 46);
			polygon.LineTo(-76, 80);
			polygon.LineTo(12, 38);
			polygon.LineTo(-69, 51);
			polygon.LineTo(-95, 29);
			polygon.LineTo(-105, 7);
			polygon.LineTo(-112, -27);
			polygon.LineTo(-149, -11);
			polygon.LineTo(-149, -11);
			polygon.LineTo(-166, -4);
			polygon.LineTo(-179, 5);
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.StartPath(180, 5);
			polyline.LineTo(140, 34);
			polyline.LineTo(180, 34);
			polygon.AddPath(polyline, 0, true);
			com.epl.geometry.Point startpoint = polygon.GetPoint(17);
			NUnit.Framework.Assert.IsTrue(startpoint.GetX() == 180 && startpoint.GetY() == 5);
		}

		[NUnit.Framework.Test]
		public virtual void TestRemovepath()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 0,
			// 0, 2);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 1,
			// 0, 3);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 2,
			// 0, 5);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 3,
			// 0, 7);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 4,
			// 0, 11);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 5,
			// 0, 13);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 6,
			// 0, 17);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 7,
			// 0, 19);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 8,
			// 0, 23);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 9,
			// 0, 29);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 10,
			// 0, 31);
			// poly->SetAttribute(enum_value2(VertexDescription, Semantics, Z), 11,
			// 0, 37);
			poly.RemovePath(1);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 2);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.IsClosedPath(0));
			NUnit.Framework.Assert.IsTrue(poly.IsClosedPath(1));
			com.epl.geometry.Point ptOut = poly.GetPoint(4);
			NUnit.Framework.Assert.IsTrue(ptOut.GetX() == 10 && ptOut.GetY() == 1);
			poly.RemovePath(0);
			poly.RemovePath(0);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 0);
			com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
			poly2.StartPath(0, 0);
			poly2.LineTo(0, 10);
			poly2.LineTo(10, 10);
			poly2.StartPath(1, 1);
			poly2.LineTo(2, 2);
			poly2.RemovePath(0);
			// poly2->StartPath(0, 0);
			poly2.LineTo(0, 10);
			poly2.LineTo(10, 10);
			// Polygon polygon2 = new Polygon();
			// polygon2.addPath(poly, -1, true);
			// polygon2.addPath(poly, -1, true);
			// polygon2.addPath(poly, -1, true);
			// assertTrue(polygon2.getPathCount() == 3);
			// polygon2.removePath(0);
			// polygon2.removePath(0);
			// polygon2.removePath(0);
			// assertTrue(polygon2.getPathCount() == 0);
			// polygon2.addPath(poly, -1, true);
			// Point point1 = new Point();
			// Point point2 = new Point();
			// point1.setX(0);
			// point1.setY(0);
			// point2.setX(0);
			// point2.setY(0);
			// polygon2.addPath(poly2, 0, true);
			// polygon2.removePath(0);
			// polygon2.insertPoint(0, 0, point1);
			// polygon2.insertPoint(0, 0, point2);
			// assertTrue(polygon2.getPathCount() == 1);
			// assertTrue(polygon2.getPointCount() == 2);
			com.epl.geometry.Polygon polygon3 = new com.epl.geometry.Polygon();
			polygon3.StartPath(0, 0);
			polygon3.LineTo(0, 10);
			polygon3.LineTo(10, 10);
			double area = polygon3.CalculateArea2D();
			polygon3.RemovePath(0);
			polygon3.StartPath(0, 0);
			polygon3.LineTo(0, 10);
			polygon3.LineTo(10, 10);
			area = polygon3.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(area > 0.0);
		}

		[NUnit.Framework.Test]
		public virtual void TestReversepath()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 0, 0,
			// 2);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 1, 0,
			// 3);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 2, 0,
			// 5);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 3, 0,
			// 7);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 4, 0,
			// 11);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 5, 0,
			// 13);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 6, 0,
			// 17);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 7, 0,
			// 19);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 8, 0,
			// 23);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 9, 0,
			// 29);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 10,
			// 0, 31);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 11,
			// 0, 37);
			poly.ReversePath(1);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.IsClosedPath(0));
			NUnit.Framework.Assert.IsTrue(poly.IsClosedPath(1));
			com.epl.geometry.Point ptOut = poly.GetPoint(4);
			NUnit.Framework.Assert.IsTrue(ptOut.GetX() == 10 && ptOut.GetY() == 1);
		}

		[NUnit.Framework.Test]
		public virtual void TestReverseAllPaths()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 0, 0,
			// 2);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 1, 0,
			// 3);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 2, 0,
			// 5);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 3, 0,
			// 7);
			//
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 4, 0,
			// 11);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 5, 0,
			// 13);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 6, 0,
			// 17);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 7, 0,
			// 19);
			//
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 8, 0,
			// 23);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 9, 0,
			// 29);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 10,
			// 0, 31);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 11,
			// 0, 37);
			double area = poly.CalculateArea2D();
			poly.ReverseAllPaths();
			double areaReversed = poly.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area + areaReversed) <= 0.001);
		}

		[NUnit.Framework.Test]
		public virtual void TestOpenAllPaths()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.ClosePathWithLine();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.ClosePathWithLine();
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			poly.ClosePathWithLine();
		}

		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 0, 0,
		// 2);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 1, 0,
		// 3);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 2, 0,
		// 5);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 3, 0,
		// 7);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 4, 0,
		// 11);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 5, 0,
		// 13);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 6, 0,
		// 17);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 7, 0,
		// 19);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 8, 0,
		// 23);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 9, 0,
		// 29);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 10,
		// 0, 31);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 11,
		// 0, 37);
		// MultiPathImpl::Pointer mpImpl =
		// (MultiPathImpl::Pointer)poly->_GetImpl();
		// poly.openAllPathsAndDuplicateStartVertex();
		// assertTrue(poly.getPathCount() == 3);
		// assertTrue(poly.getPathStart(0) == 0);
		// assertTrue(poly.getPathStart(1) == 5);
		// assertTrue(poly.getPathStart(2) == 10);
		// assertTrue(poly.getPointCount() == 15);
		// Point ptstart = poly.getPoint(0);
		// Point ptend = poly.getPoint(4);
		// assertTrue(ptstart.getX() == ptend.getX() && ptstart.getY() ==
		// ptend.getY());
		// ptstart = poly.getPoint(5);
		// ptend = poly.getPoint(9);
		// assertTrue(ptstart.getX() == ptend.getX() && ptstart.getY() ==
		// ptend.getY());
		// ptstart = poly.getPoint(10);
		// ptend = poly.getPoint(14);
		// assertTrue(ptstart.getX() == ptend.getX() && ptstart.getY() ==
		// ptend.getY());
		[NUnit.Framework.Test]
		public virtual void TestOpenPath()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.ClosePathWithLine();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(60, 144);
			poly.ClosePathWithLine();
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			poly.ClosePathWithLine();
		}

		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 0, 0,
		// 2);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 1, 0,
		// 3);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 2, 0,
		// 5);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 3, 0,
		// 7);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 4, 0,
		// 11);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 5, 0,
		// 13);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 6, 0,
		// 17);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 7, 0,
		// 19);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 8, 0,
		// 23);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 9, 0,
		// 29);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 10,
		// 0, 31);
		// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 11,
		// 0, 37);
		//
		// MultiPathImpl::Pointer mpImpl =
		// (MultiPathImpl::Pointer)poly->_GetImpl();
		// poly.openPathAndDuplicateStartVertex(1);
		// assertTrue(poly.getPathCount() == 3);
		// assertTrue(poly.getPathStart(0) == 0);
		// assertTrue(poly.getPathStart(1) == 4);
		// assertTrue(poly.getPathStart(2) == 9);
		// assertTrue(poly.getPointCount() == 13);
		// Point ptstart = poly.getPoint(4);
		// Point ptend = poly.getPoint(8);
		// assertTrue(ptstart.getX() == ptend.getX() && ptstart.getY() ==
		// ptend.getY());
		// ptstart = poly.getPoint(9);
		// assertTrue(ptstart.getX() == 10 && ptstart.getY() == 1);
		[NUnit.Framework.Test]
		public virtual void TestInsertPath()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(12, 2);
			poly.LineTo(16, 21);
			poly.LineTo(301, 15);
			poly.LineTo(61, 145);
			poly.StartPath(13, 3);
			poly.LineTo(126, 22);
			poly.LineTo(31, 16);
			poly.LineTo(601, 146);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 0, 0,
			// 2);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 1, 0,
			// 3);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 2, 0,
			// 5);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 3, 0,
			// 7);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 4, 0,
			// 11);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 5, 0,
			// 13);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 6, 0,
			// 17);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 7, 0,
			// 19);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 8, 0,
			// 23);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 9, 0,
			// 29);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 10,
			// 0, 31);
			// poly.setAttribute(enum_value2(VertexDescription, Semantics, Z), 11,
			// 0, 37);
			com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
			poly2.StartPath(12, 2);
			poly2.LineTo(16, 21);
			poly2.LineTo(301, 15);
			poly2.LineTo(61, 145);
			poly.InsertPath(0, poly2, 0, false);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(0) == 0);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(2) == 8);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(3) == 12);
			NUnit.Framework.Assert.IsTrue(poly.GetPointCount() == 16);
			com.epl.geometry.Point2D pt0 = poly.GetXY(0);
			NUnit.Framework.Assert.IsTrue(pt0.x == 12 && pt0.y == 2);
			com.epl.geometry.Point2D pt1 = poly.GetXY(1);
			NUnit.Framework.Assert.IsTrue(pt1.x == 61 && pt1.y == 145);
			com.epl.geometry.Point2D pt2 = poly.GetXY(2);
			NUnit.Framework.Assert.IsTrue(pt2.x == 301 && pt2.y == 15);
			com.epl.geometry.Point2D pt3 = poly.GetXY(3);
			NUnit.Framework.Assert.IsTrue(pt3.x == 16 && pt3.y == 21);
			com.epl.geometry.Point pt2d = new com.epl.geometry.Point(-27, -27);
			poly.InsertPoint(1, 0, pt2d);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(0) == 0);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(2) == 9);
			NUnit.Framework.Assert.IsTrue(poly.GetPathStart(3) == 13);
			NUnit.Framework.Assert.IsTrue(poly.GetPointCount() == 17);
		}

		[NUnit.Framework.Test]
		public virtual void TestInsertPoints()
		{
			{
				// forward insertion
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 1);
				poly.LineTo(15, 20);
				poly.LineTo(30, 14);
				poly.LineTo(60, 144);
				poly.StartPath(10, 1);
				poly.LineTo(15, 20);
				poly.LineTo(300, 14);
				poly.LineTo(314, 217);
				poly.LineTo(60, 144);
				poly.StartPath(10, 1);
				poly.LineTo(125, 20);
				poly.LineTo(30, 14);
				poly.LineTo(600, 144);
				com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
				poly1.StartPath(1, 17);
				poly1.LineTo(1, 207);
				poly1.LineTo(3, 147);
				poly1.LineTo(6, 1447);
				poly1.StartPath(1000, 17);
				poly1.LineTo(1250, 207);
				poly1.LineTo(300, 147);
				poly1.LineTo(6000, 1447);
				poly1.InsertPoints(1, 2, poly, 1, 1, 3, true);
				// forward
				NUnit.Framework.Assert.IsTrue(poly1.GetPathCount() == 2);
				NUnit.Framework.Assert.IsTrue(poly1.GetPathStart(1) == 4);
				NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(0));
				NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(1));
				NUnit.Framework.Assert.IsTrue(poly1.GetPointCount() == 11);
				NUnit.Framework.Assert.IsTrue(poly1.GetPathSize(1) == 7);
			}
			{
				// Point2D ptOut;
				// ptOut = poly1.getXY(5);
				// assertTrue(ptOut.x == 1250 && ptOut.y == 207);
				// ptOut = poly1.getXY(6);
				// assertTrue(ptOut.x == 15 && ptOut.y == 20);
				// ptOut = poly1.getXY(7);
				// assertTrue(ptOut.x == 300 && ptOut.y == 14);
				// ptOut = poly1.getXY(8);
				// assertTrue(ptOut.x == 314 && ptOut.y == 217);
				// ptOut = poly1.getXY(9);
				// assertTrue(ptOut.x == 300 && ptOut.y == 147);
				// ptOut = poly1.getXY(10);
				// assertTrue(ptOut.x == 6000 && ptOut.y == 1447);
				// reverse insertion
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 1);
				poly.LineTo(15, 20);
				poly.LineTo(30, 14);
				poly.LineTo(60, 144);
				poly.StartPath(10, 1);
				poly.LineTo(15, 20);
				poly.LineTo(300, 14);
				poly.LineTo(314, 217);
				poly.LineTo(60, 144);
				poly.StartPath(10, 1);
				poly.LineTo(125, 20);
				poly.LineTo(30, 14);
				poly.LineTo(600, 144);
				com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
				poly1.StartPath(1, 17);
				poly1.LineTo(1, 207);
				poly1.LineTo(3, 147);
				poly1.LineTo(6, 1447);
				poly1.StartPath(1000, 17);
				poly1.LineTo(1250, 207);
				poly1.LineTo(300, 147);
				poly1.LineTo(6000, 1447);
				poly1.InsertPoints(1, 2, poly, 1, 1, 3, false);
				// reverse
				NUnit.Framework.Assert.IsTrue(poly1.GetPathCount() == 2);
				NUnit.Framework.Assert.IsTrue(poly1.GetPathStart(1) == 4);
				NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(0));
				NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(1));
				NUnit.Framework.Assert.IsTrue(poly1.GetPointCount() == 11);
				NUnit.Framework.Assert.IsTrue(poly1.GetPathSize(1) == 7);
			}
		}

		// Point2D ptOut;
		// ptOut = poly1.getXY(5);
		// assertTrue(ptOut.x == 1250 && ptOut.y == 207);
		// ptOut = poly1.getXY(6);
		// assertTrue(ptOut.x == 314 && ptOut.y == 217);
		// ptOut = poly1.getXY(7);
		// assertTrue(ptOut.x == 300 && ptOut.y == 14);
		// ptOut = poly1.getXY(8);
		// assertTrue(ptOut.x == 15 && ptOut.y == 20);
		// ptOut = poly1.getXY(9);
		// assertTrue(ptOut.x == 300 && ptOut.y == 147);
		// ptOut = poly1.getXY(10);
		// assertTrue(ptOut.x == 6000 && ptOut.y == 1447);
		[NUnit.Framework.Test]
		public virtual void TestInsertPoint()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(314, 217);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			com.epl.geometry.Point pt = new com.epl.geometry.Point(-33, -34);
			poly.InsertPoint(1, 1, pt);
			pt = poly.GetPoint(4);
			NUnit.Framework.Assert.IsTrue(pt.GetX() == 10 && pt.GetY() == 1);
			pt = poly.GetPoint(5);
			NUnit.Framework.Assert.IsTrue(pt.GetX() == -33 && pt.GetY() == -34);
			pt = poly.GetPoint(6);
			NUnit.Framework.Assert.IsTrue(pt.GetX() == 15 && pt.GetY() == 20);
			NUnit.Framework.Assert.IsTrue(poly.GetPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(poly.GetPathSize(1) == 6);
			NUnit.Framework.Assert.IsTrue(poly.GetPathSize(2) == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathCount() == 3);
		}

		[NUnit.Framework.Test]
		public virtual void TestRemovePoint()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(300, 14);
			poly.LineTo(314, 217);
			poly.LineTo(60, 144);
			poly.StartPath(10, 1);
			poly.LineTo(125, 20);
			poly.LineTo(30, 14);
			poly.LineTo(600, 144);
			poly.RemovePoint(1, 1);
			com.epl.geometry.Point pt;
			pt = poly.GetPoint(4);
			NUnit.Framework.Assert.IsTrue(pt.GetX() == 10 && pt.GetY() == 1);
			pt = poly.GetPoint(5);
			NUnit.Framework.Assert.IsTrue(pt.GetX() == 300 && pt.GetY() == 14);
			NUnit.Framework.Assert.IsTrue(poly.GetPointCount() == 12);
			NUnit.Framework.Assert.IsTrue(poly.GetPathSize(0) == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathSize(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetPathSize(2) == 4);
		}

		[NUnit.Framework.Test]
		public static void TestPolygonAreaAndLength()
		{
			com.epl.geometry.Polygon poly;
			/* const */
			double r = 1.0;
			/* const */
			double epsilon = 1.0e-14;
			/* const */
			int nMax = 40;
			// If r == 1.0 and nMax == 40 and epsilon == 1.0e-14, it will pass.
			// But if r == 1.0 and nMax == 40 and epsilon == 1.0e-15, it will fail.
			for (int n = 3; n < nMax; n++)
			{
				// regular polygon with n vertices and length from center to vertex
				// = r
				poly = new com.epl.geometry.Polygon();
				double theta = 0.0;
				poly.StartPath(r, 0.0);
				for (int k = 1; k <= n; k++)
				{
					theta -= 2 * System.Math.PI / n;
					poly.LineTo(r * System.Math.Cos(theta), r * System.Math.Sin(theta));
				}
				double sinPiOverN = System.Math.Sin(System.Math.PI / n);
				double sinTwoPiOverN = System.Math.Sin(2.0 * System.Math.PI / n);
				double analyticalLength = 2.0 * n * r * sinPiOverN;
				double analyticalArea = 0.5 * n * r * r * sinTwoPiOverN;
				double calculatedLength = poly.CalculateLength2D();
				double calculatedArea = poly.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(analyticalLength - calculatedLength) < epsilon);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(analyticalArea - calculatedArea) < epsilon);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInsertPointsFromArray()
		{
			{
				// Test forward insertion of an array of Point2D
				// ArrayOf(Point2D) arr = new ArrayOf(Point2D)(5);
				// arr[0].SetCoords(10, 1);
				// arr[1].SetCoords(15, 20);
				// arr[2].SetCoords(300, 14);
				// arr[3].SetCoords(314, 217);
				// arr[4].SetCoords(60, 144);
				com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
				poly1.StartPath(1, 17);
				poly1.LineTo(1, 207);
				poly1.LineTo(3, 147);
				poly1.LineTo(6, 1447);
				poly1.StartPath(1000, 17);
				poly1.LineTo(1250, 207);
				poly1.LineTo(300, 147);
				poly1.LineTo(6000, 1447);
				NUnit.Framework.Assert.IsTrue(poly1.GetPathCount() == 2);
				NUnit.Framework.Assert.IsTrue(poly1.GetPathStart(1) == 4);
				NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(0));
				NUnit.Framework.Assert.IsTrue(poly1.IsClosedPath(1));
			}
			{
			}
		}

		// Test reversed insertion of an array of Point2D
		[NUnit.Framework.Test]
		public virtual void TestCR177477()
		{
			com.epl.geometry.Polygon pg = new com.epl.geometry.Polygon();
			pg.StartPath(-130, 40);
			pg.LineTo(-70, 40);
			pg.LineTo(-70, 10);
			pg.LineTo(-130, 10);
			com.epl.geometry.Polygon pg2 = new com.epl.geometry.Polygon();
			pg2.StartPath(-60, 40);
			pg2.LineTo(-50, 40);
			pg2.LineTo(-50, 10);
			pg2.LineTo(-60, 10);
			pg.Add(pg2, false);
		}

		[NUnit.Framework.Test]
		public virtual void TestCR177477getPathEnd()
		{
			com.epl.geometry.Polygon pg = new com.epl.geometry.Polygon();
			pg.StartPath(-130, 40);
			pg.LineTo(-70, 40);
			pg.LineTo(-70, 10);
			pg.LineTo(-130, 10);
			pg.StartPath(-60, 40);
			pg.LineTo(-50, 40);
			pg.LineTo(-50, 10);
			pg.LineTo(-60, 10);
			pg.StartPath(-40, 40);
			pg.LineTo(-30, 40);
			pg.LineTo(-30, 10);
			pg.LineTo(-40, 10);
			int pathCount = pg.GetPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 3);
			// int startIndex = pg.getPathStart(pathCount - 1);
			// int endIndex = pg.getPathEnd(pathCount - 1);
			com.epl.geometry.Line line = new com.epl.geometry.Line();
			line.ToString();
			line.SetStart(new com.epl.geometry.Point(0, 0));
			line.SetEnd(new com.epl.geometry.Point(1, 0));
			line.ToString();
			double geoLength = com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(new com.epl.geometry.Point(0, 0), new com.epl.geometry.Point(1, 0));
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(geoLength - 111319) < 1);
		}

		[NUnit.Framework.Test]
		public virtual void TestBug1()
		{
			com.epl.geometry.Polygon pg = new com.epl.geometry.Polygon();
			pg.StartPath(-130, 40);
			for (int i = 0; i < 1000; i++)
			{
				pg.LineTo(-70, 40);
			}
			for (int i_1 = 0; i_1 < 999; i_1++)
			{
				pg.RemovePoint(0, pg.GetPointCount() - 1);
			}
			pg.LineTo(-70, 40);
		}

		[NUnit.Framework.Test]
		public virtual void TestGeometryCopy()
		{
			bool noException = true;
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			com.epl.geometry.Point p1 = new com.epl.geometry.Point(-85.59285621496956, 38.26004727491098);
			com.epl.geometry.Point p2 = new com.epl.geometry.Point(-85.56417866635002, 38.28084064314639);
			com.epl.geometry.Point p3 = new com.epl.geometry.Point(-85.56845156650877, 38.24659881865461);
			com.epl.geometry.Point p4 = new com.epl.geometry.Point(-85.55341069949853, 38.26671513050464);
			polyline.StartPath(p1);
			try
			{
				polyline.LineTo(p2);
				polyline.Copy();
				polyline.LineTo(p3);
				polyline.Copy();
				polyline.LineTo(p4);
			}
			catch (System.Exception e)
			{
				// exception thrown here!!!
//				e.PrintStackTrace();
				noException = false;
			}
			NUnit.Framework.Assert.IsTrue(noException);
		}

		// end of method
		[NUnit.Framework.Test]
		public virtual void TestBoundary()
		{
			com.epl.geometry.Geometry g = com.epl.geometry.OperatorImportFromWkt.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, "POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))", null);
			com.epl.geometry.Geometry boundary = com.epl.geometry.OperatorBoundary.Local().Execute(g, null);
			com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)boundary;
			polyline.ReverseAllPaths();
			string s = com.epl.geometry.OperatorExportToWkt.Local().Execute(0, boundary, null);
			NUnit.Framework.Assert.IsTrue(s.Equals("MULTILINESTRING ((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"));
		}

		[NUnit.Framework.Test]
		public virtual void TestReplaceNaNs()
		{
			{
				com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
				com.epl.geometry.Point pt = new com.epl.geometry.Point();
				pt.SetXY(1, 2);
				pt.SetZ(double.NaN);
				mp.Add(pt);
				pt = new com.epl.geometry.Point();
				pt.SetXY(11, 12);
				pt.SetZ(3);
				mp.Add(pt);
				mp.ReplaceNaNs(com.epl.geometry.VertexDescription.Semantics.Z, 5);
				NUnit.Framework.Assert.IsTrue(mp.GetPoint(0).Equals(new com.epl.geometry.Point(1, 2, 5)));
				NUnit.Framework.Assert.IsTrue(mp.GetPoint(1).Equals(new com.epl.geometry.Point(11, 12, 3)));
			}
			{
				com.epl.geometry.Polygon mp = new com.epl.geometry.Polygon();
				com.epl.geometry.Point pt = new com.epl.geometry.Point();
				pt.SetXY(1, 2);
				pt.SetZ(double.NaN);
				mp.StartPath(pt);
				pt = new com.epl.geometry.Point();
				pt.SetXY(11, 12);
				pt.SetZ(3);
				mp.LineTo(pt);
				mp.ReplaceNaNs(com.epl.geometry.VertexDescription.Semantics.Z, 5);
				NUnit.Framework.Assert.IsTrue(mp.GetPoint(0).Equals(new com.epl.geometry.Point(1, 2, 5)));
				NUnit.Framework.Assert.IsTrue(mp.GetPoint(1).Equals(new com.epl.geometry.Point(11, 12, 3)));
			}
			{
				com.epl.geometry.Polygon mp = new com.epl.geometry.Polygon();
				com.epl.geometry.Point pt = new com.epl.geometry.Point();
				pt.SetXY(1, 2);
				pt.SetM(double.NaN);
				mp.StartPath(pt);
				pt = new com.epl.geometry.Point();
				pt.SetXY(11, 12);
				pt.SetM(3);
				mp.LineTo(pt);
				mp.ReplaceNaNs(com.epl.geometry.VertexDescription.Semantics.M, 5);
				com.epl.geometry.Point p = new com.epl.geometry.Point(1, 2);
				p.SetM(5);
				bool b = mp.GetPoint(0).Equals(p);
				NUnit.Framework.Assert.IsTrue(b);
				p = new com.epl.geometry.Point(11, 12);
				p.SetM(3);
				b = mp.GetPoint(1).Equals(p);
				NUnit.Framework.Assert.IsTrue(b);
			}
		}

//		[NUnit.Framework.Test]
//		public virtual void TestPolygon2PolygonFails()
//		{
//			com.epl.geometry.OperatorFactoryLocal factory = com.epl.geometry.OperatorFactoryLocal.GetInstance();
//			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
//			string result = exporter.Execute(Birmingham());
//			com.epl.geometry.OperatorImportFromGeoJson importer = (com.epl.geometry.OperatorImportFromGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.MapGeometry mapGeometry = importer.Execute(com.epl.geometry.GeoJsonImportFlags.geoJsonImportDefaults, com.epl.geometry.Geometry.Type.Polygon, result, null);
//			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)mapGeometry.GetGeometry();
//			NUnit.Framework.Assert.AreEqual(Birmingham(), polygon);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygon2PolygonFails2()
//		{
//			string birminghamGeojson = com.epl.geometry.GeometryEngine.GeometryToGeoJson(Birmingham());
//			com.epl.geometry.MapGeometry returnedGeometry = com.epl.geometry.GeometryEngine.GeoJsonToGeometry(birminghamGeojson, com.epl.geometry.GeoJsonImportFlags.geoJsonImportDefaults, com.epl.geometry.Geometry.Type.Polygon);
//			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)returnedGeometry.GetGeometry();
//			NUnit.Framework.Assert.AreEqual(polygon, Birmingham());
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygon2PolygonWorks()
//		{
//			string birminghamGeojson = com.epl.geometry.GeometryEngine.GeometryToGeoJson(Birmingham());
//			com.epl.geometry.MapGeometry returnedGeometry = com.epl.geometry.GeometryEngine.GeoJsonToGeometry(birminghamGeojson, com.epl.geometry.GeoJsonImportFlags.geoJsonImportDefaults, com.epl.geometry.Geometry.Type.Polygon);
//			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)returnedGeometry.GetGeometry();
//			NUnit.Framework.Assert.AreEqual(polygon.ToString(), Birmingham().ToString());
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygon2Polygon2Works()
//		{
//			string birminghamJson = com.epl.geometry.GeometryEngine.GeometryToJson(4326, Birmingham());
//			com.epl.geometry.MapGeometry returnedGeometry = com.epl.geometry.GeometryEngine.JsonToGeometry(birminghamJson);
//			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)returnedGeometry.GetGeometry();
//			NUnit.Framework.Assert.AreEqual(polygon, Birmingham());
//			string s = polygon.ToString();
//		}

		[NUnit.Framework.Test]
		public virtual void TestSegmentIteratorCrash()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			// clockwise => outer ring
			poly.StartPath(0, 0);
			poly.LineTo(-0.5, 0.5);
			poly.LineTo(0.5, 1);
			poly.LineTo(1, 0.5);
			poly.LineTo(0.5, 0);
			// hole
			poly.StartPath(0.5, 0.2);
			poly.LineTo(0.6, 0.5);
			poly.LineTo(0.2, 0.9);
			poly.LineTo(-0.2, 0.5);
			poly.LineTo(0.1, 0.2);
			poly.LineTo(0.2, 0.3);
			// island
			poly.StartPath(0.1, 0.7);
			poly.LineTo(0.3, 0.7);
			poly.LineTo(0.3, 0.4);
			poly.LineTo(0.1, 0.4);
			NUnit.Framework.Assert.AreEqual(poly.GetSegmentCount(), 15);
			NUnit.Framework.Assert.AreEqual(poly.GetPathCount(), 3);
			com.epl.geometry.SegmentIterator segmentIterator = poly.QuerySegmentIterator();
			int paths = 0;
			int segments = 0;
			while (segmentIterator.NextPath())
			{
				paths++;
				com.epl.geometry.Segment segment;
				while (segmentIterator.HasNextSegment())
				{
					segment = segmentIterator.NextSegment();
					segments++;
				}
			}
			NUnit.Framework.Assert.AreEqual(paths, 3);
			NUnit.Framework.Assert.AreEqual(segments, 15);
		}

		private static com.epl.geometry.Polygon Birmingham()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.AddEnvelope(new com.epl.geometry.Envelope(-1.954245, 52.513531, -1.837357, 52.450123), false);
			poly.AddEnvelope(new com.epl.geometry.Envelope(0, 0, 1, 1), false);
			return poly;
		}
	}
}
