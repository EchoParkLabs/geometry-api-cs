

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestPolygon
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
		public virtual void testCreation()
		{
			// simple create
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			int number = poly.getStateFlag();
			NUnit.Framework.Assert.IsTrue(poly != null);
			// assertTrue(poly.getClass() == Polygon.class);
			// assertFalse(poly.getClass() == Envelope.class);
			NUnit.Framework.Assert.IsTrue(poly.getType() == com.esri.core.geometry.Geometry.Type
				.Polygon);
			NUnit.Framework.Assert.IsTrue(poly.isEmpty());
			NUnit.Framework.Assert.IsTrue(poly.getPointCount() == 0);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 0);
			number = poly.getStateFlag();
			poly = null;
			NUnit.Framework.Assert.IsFalse(poly != null);
			// play with default attributes
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
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
		public virtual void testCreation1()
		{
			// Simple area and length calcul test
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			int number = poly.getStateFlag();
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(1000, 2000
				, 1010, 2010);
			env.ToString();
			poly.addEnvelope(env, false);
			poly.ToString();
			number = poly.getStateFlag();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(poly.calculateArea2D() - 100) < 1e-12
				);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(poly.calculateLength2D() - 40) < 1e-12
				);
			poly.setEmpty();
			number = poly.getStateFlag();
			poly.addEnvelope(env, true);
			number = poly.getStateFlag();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(poly.calculateArea2D() + 100) < 1e-12
				);
			number = poly.getStateFlag();
		}

		[NUnit.Framework.Test]
		public virtual void testCreation2()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			int state1 = poly.getStateFlag();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.closePathWithLine();
			int state2 = poly.getStateFlag();
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
			poly.startPath(20, 13);
			poly.lineTo(150, 120);
			poly.lineTo(300, 414);
			poly.lineTo(610, 14);
			poly.lineTo(6210, 140);
			poly.closePathWithLine();
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
			poly.startPath(200, 1333);
			poly.lineTo(1150, 1120);
			poly.lineTo(300, 4114);
			poly.lineTo(6110, 114);
			poly.lineTo(61210, 1140);
			NUnit.Framework.Assert.IsTrue(poly.isClosedPath(2) == true);
			poly.closeAllPaths();
			NUnit.Framework.Assert.IsTrue(poly.isClosedPath(2) == true);
			{
				com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
				poly2.startPath(10, 10);
				poly2.lineTo(100, 10);
				poly2.lineTo(100, 100);
				poly2.lineTo(10, 100);
			}
			{
				com.esri.core.geometry.Polygon poly3 = new com.esri.core.geometry.Polygon();
				// create a star (non-simple)
				poly3.startPath(1, 0);
				poly3.lineTo(5, 10);
				poly3.lineTo(9, 0);
				poly3.lineTo(0, 6);
				poly3.lineTo(10, 6);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testCreateWithStreams()
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
		public virtual void testCloneStuff()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			poly.closePathWithLine();
			com.esri.core.geometry.Polygon clone = (com.esri.core.geometry.Polygon)poly.copy(
				);
			NUnit.Framework.Assert.IsTrue(clone.getPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(clone.getPathStart(2) == 8);
			NUnit.Framework.Assert.IsTrue(clone.isClosedPath(0));
			NUnit.Framework.Assert.IsTrue(clone.isClosedPath(1));
			NUnit.Framework.Assert.IsTrue(clone.isClosedPath(2));
			NUnit.Framework.Assert.IsTrue(clone.getXY(5).isEqual(new com.esri.core.geometry.Point2D
				(15, 20)));
		}

		[NUnit.Framework.Test]
		public virtual void testCloneStuffEnvelope()
		{
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(11, 12, 
				15, 24);
			com.esri.core.geometry.Envelope eCopy = (com.esri.core.geometry.Envelope)env.copy
				();
			NUnit.Framework.Assert.IsTrue(eCopy.Equals(env));
			NUnit.Framework.Assert.IsTrue(eCopy.getXMin() == 11);
			NUnit.Framework.Assert.IsTrue(eCopy.getYMin() == 12);
			NUnit.Framework.Assert.IsTrue(eCopy.getXMax() == 15);
			NUnit.Framework.Assert.IsTrue(eCopy.getYMax() == 24);
		}

		[NUnit.Framework.Test]
		public virtual void testCloneStuffPolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			poly.closePathWithLine();
			com.esri.core.geometry.Polyline clone = (com.esri.core.geometry.Polyline)poly.copy
				();
			NUnit.Framework.Assert.IsTrue(clone.getPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(clone.getPathStart(2) == 8);
			NUnit.Framework.Assert.IsTrue(!clone.isClosedPath(0));
			NUnit.Framework.Assert.IsTrue(!clone.isClosedPath(1));
			NUnit.Framework.Assert.IsTrue(clone.isClosedPath(2));
			NUnit.Framework.Assert.IsTrue(clone.getXY(5).isEqual(new com.esri.core.geometry.Point2D
				(15, 20)));
		}

		[NUnit.Framework.Test]
		public virtual void testAddpath()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.addPath(poly, 2, true);
			poly1.addPath(poly, 0, true);
			NUnit.Framework.Assert.IsTrue(poly1.getPathCount() == 2);
			NUnit.Framework.Assert.IsTrue(poly1.getPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(0));
			NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(1));
			com.esri.core.geometry.Point ptOut = poly1.getPoint(6);
			NUnit.Framework.Assert.IsTrue(ptOut.getX() == 30 && ptOut.getY() == 14);
		}

		[NUnit.Framework.Test]
		public virtual void testAddpath2()
		{
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			polygon.startPath(-179, 34);
			polygon.lineTo(-154, 34);
			polygon.lineTo(-179, 36);
			polygon.lineTo(-180, 90);
			polygon.lineTo(180, 90);
			polygon.lineTo(180, 36);
			polygon.lineTo(70, 46);
			polygon.lineTo(-76, 80);
			polygon.lineTo(12, 38);
			polygon.lineTo(-69, 51);
			polygon.lineTo(-95, 29);
			polygon.lineTo(-105, 7);
			polygon.lineTo(-112, -27);
			polygon.lineTo(-149, -11);
			polygon.lineTo(-149, -11);
			polygon.lineTo(-166, -4);
			polygon.lineTo(-179, 5);
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(180, 5);
			polyline.lineTo(140, 34);
			polyline.lineTo(180, 34);
			polygon.addPath(polyline, 0, true);
			com.esri.core.geometry.Point startpoint = polygon.getPoint(17);
			NUnit.Framework.Assert.IsTrue(startpoint.getX() == 180 && startpoint.getY() == 5);
		}

		[NUnit.Framework.Test]
		public virtual void testRemovepath()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
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
			poly.removePath(1);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 2);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.isClosedPath(0));
			NUnit.Framework.Assert.IsTrue(poly.isClosedPath(1));
			com.esri.core.geometry.Point ptOut = poly.getPoint(4);
			NUnit.Framework.Assert.IsTrue(ptOut.getX() == 10 && ptOut.getY() == 1);
			poly.removePath(0);
			poly.removePath(0);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 0);
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			poly2.startPath(0, 0);
			poly2.lineTo(0, 10);
			poly2.lineTo(10, 10);
			poly2.startPath(1, 1);
			poly2.lineTo(2, 2);
			poly2.removePath(0);
			// poly2->StartPath(0, 0);
			poly2.lineTo(0, 10);
			poly2.lineTo(10, 10);
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
			com.esri.core.geometry.Polygon polygon3 = new com.esri.core.geometry.Polygon();
			polygon3.startPath(0, 0);
			polygon3.lineTo(0, 10);
			polygon3.lineTo(10, 10);
			double area = polygon3.calculateArea2D();
			polygon3.removePath(0);
			polygon3.startPath(0, 0);
			polygon3.lineTo(0, 10);
			polygon3.lineTo(10, 10);
			area = polygon3.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(area > 0.0);
		}

		[NUnit.Framework.Test]
		public virtual void testReversepath()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
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
			poly.reversePath(1);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.isClosedPath(0));
			NUnit.Framework.Assert.IsTrue(poly.isClosedPath(1));
			com.esri.core.geometry.Point ptOut = poly.getPoint(4);
			NUnit.Framework.Assert.IsTrue(ptOut.getX() == 10 && ptOut.getY() == 1);
		}

		[NUnit.Framework.Test]
		public virtual void testReverseAllPaths()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
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
			double area = poly.calculateArea2D();
			poly.reverseAllPaths();
			double areaReversed = poly.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area + areaReversed) <= 0.001);
		}

		[NUnit.Framework.Test]
		public virtual void testOpenAllPaths()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.closePathWithLine();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.closePathWithLine();
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			poly.closePathWithLine();
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
		public virtual void testOpenPath()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.closePathWithLine();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(60, 144);
			poly.closePathWithLine();
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			poly.closePathWithLine();
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
		public virtual void testInsertPath()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(12, 2);
			poly.lineTo(16, 21);
			poly.lineTo(301, 15);
			poly.lineTo(61, 145);
			poly.startPath(13, 3);
			poly.lineTo(126, 22);
			poly.lineTo(31, 16);
			poly.lineTo(601, 146);
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
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			poly2.startPath(12, 2);
			poly2.lineTo(16, 21);
			poly2.lineTo(301, 15);
			poly2.lineTo(61, 145);
			poly.insertPath(0, poly2, 0, false);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(0) == 0);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(2) == 8);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(3) == 12);
			NUnit.Framework.Assert.IsTrue(poly.getPointCount() == 16);
			com.esri.core.geometry.Point2D pt0 = poly.getXY(0);
			NUnit.Framework.Assert.IsTrue(pt0.x == 12 && pt0.y == 2);
			com.esri.core.geometry.Point2D pt1 = poly.getXY(1);
			NUnit.Framework.Assert.IsTrue(pt1.x == 61 && pt1.y == 145);
			com.esri.core.geometry.Point2D pt2 = poly.getXY(2);
			NUnit.Framework.Assert.IsTrue(pt2.x == 301 && pt2.y == 15);
			com.esri.core.geometry.Point2D pt3 = poly.getXY(3);
			NUnit.Framework.Assert.IsTrue(pt3.x == 16 && pt3.y == 21);
			com.esri.core.geometry.Point pt2d = new com.esri.core.geometry.Point(-27, -27);
			poly.insertPoint(1, 0, pt2d);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(0) == 0);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(2) == 9);
			NUnit.Framework.Assert.IsTrue(poly.getPathStart(3) == 13);
			NUnit.Framework.Assert.IsTrue(poly.getPointCount() == 17);
		}

		[NUnit.Framework.Test]
		public virtual void testInsertPoints()
		{
			{
				// forward insertion
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 1);
				poly.lineTo(15, 20);
				poly.lineTo(30, 14);
				poly.lineTo(60, 144);
				poly.startPath(10, 1);
				poly.lineTo(15, 20);
				poly.lineTo(300, 14);
				poly.lineTo(314, 217);
				poly.lineTo(60, 144);
				poly.startPath(10, 1);
				poly.lineTo(125, 20);
				poly.lineTo(30, 14);
				poly.lineTo(600, 144);
				com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
				poly1.startPath(1, 17);
				poly1.lineTo(1, 207);
				poly1.lineTo(3, 147);
				poly1.lineTo(6, 1447);
				poly1.startPath(1000, 17);
				poly1.lineTo(1250, 207);
				poly1.lineTo(300, 147);
				poly1.lineTo(6000, 1447);
				poly1.insertPoints(1, 2, poly, 1, 1, 3, true);
				// forward
				NUnit.Framework.Assert.IsTrue(poly1.getPathCount() == 2);
				NUnit.Framework.Assert.IsTrue(poly1.getPathStart(1) == 4);
				NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(0));
				NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(1));
				NUnit.Framework.Assert.IsTrue(poly1.getPointCount() == 11);
				NUnit.Framework.Assert.IsTrue(poly1.getPathSize(1) == 7);
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
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 1);
				poly.lineTo(15, 20);
				poly.lineTo(30, 14);
				poly.lineTo(60, 144);
				poly.startPath(10, 1);
				poly.lineTo(15, 20);
				poly.lineTo(300, 14);
				poly.lineTo(314, 217);
				poly.lineTo(60, 144);
				poly.startPath(10, 1);
				poly.lineTo(125, 20);
				poly.lineTo(30, 14);
				poly.lineTo(600, 144);
				com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
				poly1.startPath(1, 17);
				poly1.lineTo(1, 207);
				poly1.lineTo(3, 147);
				poly1.lineTo(6, 1447);
				poly1.startPath(1000, 17);
				poly1.lineTo(1250, 207);
				poly1.lineTo(300, 147);
				poly1.lineTo(6000, 1447);
				poly1.insertPoints(1, 2, poly, 1, 1, 3, false);
				// reverse
				NUnit.Framework.Assert.IsTrue(poly1.getPathCount() == 2);
				NUnit.Framework.Assert.IsTrue(poly1.getPathStart(1) == 4);
				NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(0));
				NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(1));
				NUnit.Framework.Assert.IsTrue(poly1.getPointCount() == 11);
				NUnit.Framework.Assert.IsTrue(poly1.getPathSize(1) == 7);
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
		public virtual void testInsertPoint()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(314, 217);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point(-33, -34);
			poly.insertPoint(1, 1, pt);
			pt = poly.getPoint(4);
			NUnit.Framework.Assert.IsTrue(pt.getX() == 10 && pt.getY() == 1);
			pt = poly.getPoint(5);
			NUnit.Framework.Assert.IsTrue(pt.getX() == -33 && pt.getY() == -34);
			pt = poly.getPoint(6);
			NUnit.Framework.Assert.IsTrue(pt.getX() == 15 && pt.getY() == 20);
			NUnit.Framework.Assert.IsTrue(poly.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(poly.getPathSize(1) == 6);
			NUnit.Framework.Assert.IsTrue(poly.getPathSize(2) == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathCount() == 3);
		}

		[NUnit.Framework.Test]
		public virtual void testRemovePoint()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(300, 14);
			poly.lineTo(314, 217);
			poly.lineTo(60, 144);
			poly.startPath(10, 1);
			poly.lineTo(125, 20);
			poly.lineTo(30, 14);
			poly.lineTo(600, 144);
			poly.removePoint(1, 1);
			com.esri.core.geometry.Point pt;
			pt = poly.getPoint(4);
			NUnit.Framework.Assert.IsTrue(pt.getX() == 10 && pt.getY() == 1);
			pt = poly.getPoint(5);
			NUnit.Framework.Assert.IsTrue(pt.getX() == 300 && pt.getY() == 14);
			NUnit.Framework.Assert.IsTrue(poly.getPointCount() == 12);
			NUnit.Framework.Assert.IsTrue(poly.getPathSize(0) == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathSize(1) == 4);
			NUnit.Framework.Assert.IsTrue(poly.getPathSize(2) == 4);
		}

		[NUnit.Framework.Test]
		public static void testPolygonAreaAndLength()
		{
			com.esri.core.geometry.Polygon poly;
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
				poly = new com.esri.core.geometry.Polygon();
				double theta = 0.0;
				poly.startPath(r, 0.0);
				for (int k = 1; k <= n; k++)
				{
					theta -= 2 * System.Math.PI / n;
					poly.lineTo(r * System.Math.cos(theta), r * System.Math.sin(theta));
				}
				double sinPiOverN = System.Math.sin(System.Math.PI / n);
				double sinTwoPiOverN = System.Math.sin(2.0 * System.Math.PI / n);
				double analyticalLength = 2.0 * n * r * sinPiOverN;
				double analyticalArea = 0.5 * n * r * r * sinTwoPiOverN;
				double calculatedLength = poly.calculateLength2D();
				double calculatedArea = poly.calculateArea2D();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(analyticalLength - calculatedLength
					) < epsilon);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(analyticalArea - calculatedArea) < 
					epsilon);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testInsertPointsFromArray()
		{
			{
				// Test forward insertion of an array of Point2D
				// ArrayOf(Point2D) arr = new ArrayOf(Point2D)(5);
				// arr[0].SetCoords(10, 1);
				// arr[1].SetCoords(15, 20);
				// arr[2].SetCoords(300, 14);
				// arr[3].SetCoords(314, 217);
				// arr[4].SetCoords(60, 144);
				com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
				poly1.startPath(1, 17);
				poly1.lineTo(1, 207);
				poly1.lineTo(3, 147);
				poly1.lineTo(6, 1447);
				poly1.startPath(1000, 17);
				poly1.lineTo(1250, 207);
				poly1.lineTo(300, 147);
				poly1.lineTo(6000, 1447);
				NUnit.Framework.Assert.IsTrue(poly1.getPathCount() == 2);
				NUnit.Framework.Assert.IsTrue(poly1.getPathStart(1) == 4);
				NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(0));
				NUnit.Framework.Assert.IsTrue(poly1.isClosedPath(1));
			}
			{
			}
		}

		// Test reversed insertion of an array of Point2D
		[NUnit.Framework.Test]
		public virtual void testCR177477()
		{
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(-130, 40);
			pg.lineTo(-70, 40);
			pg.lineTo(-70, 10);
			pg.lineTo(-130, 10);
			com.esri.core.geometry.Polygon pg2 = new com.esri.core.geometry.Polygon();
			pg2.startPath(-60, 40);
			pg2.lineTo(-50, 40);
			pg2.lineTo(-50, 10);
			pg2.lineTo(-60, 10);
			pg.add(pg2, false);
		}

		[NUnit.Framework.Test]
		public virtual void testCR177477getPathEnd()
		{
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(-130, 40);
			pg.lineTo(-70, 40);
			pg.lineTo(-70, 10);
			pg.lineTo(-130, 10);
			pg.startPath(-60, 40);
			pg.lineTo(-50, 40);
			pg.lineTo(-50, 10);
			pg.lineTo(-60, 10);
			pg.startPath(-40, 40);
			pg.lineTo(-30, 40);
			pg.lineTo(-30, 10);
			pg.lineTo(-40, 10);
			int pathCount = pg.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 3);
			// int startIndex = pg.getPathStart(pathCount - 1);
			// int endIndex = pg.getPathEnd(pathCount - 1);
			com.esri.core.geometry.Line line = new com.esri.core.geometry.Line();
			line.ToString();
			line.setStart(new com.esri.core.geometry.Point(0, 0));
			line.setEnd(new com.esri.core.geometry.Point(1, 0));
			line.ToString();
			double geoLength = com.esri.core.geometry.GeometryEngine.geodesicDistanceOnWGS84(
				new com.esri.core.geometry.Point(0, 0), new com.esri.core.geometry.Point(1, 0));
			NUnit.Framework.Assert.IsTrue(System.Math.abs(geoLength - 111319) < 1);
		}

		[NUnit.Framework.Test]
		public virtual void testBug1()
		{
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(-130, 40);
			for (int i = 0; i < 1000; i++)
			{
				pg.lineTo(-70, 40);
			}
			for (int i_1 = 0; i_1 < 999; i_1++)
			{
				pg.removePoint(0, pg.getPointCount() - 1);
			}
			pg.lineTo(-70, 40);
		}

		[NUnit.Framework.Test]
		public virtual void testGeometryCopy()
		{
			bool noException = true;
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Point p1 = new com.esri.core.geometry.Point(-85.59285621496956
				, 38.26004727491098);
			com.esri.core.geometry.Point p2 = new com.esri.core.geometry.Point(-85.56417866635002
				, 38.28084064314639);
			com.esri.core.geometry.Point p3 = new com.esri.core.geometry.Point(-85.56845156650877
				, 38.24659881865461);
			com.esri.core.geometry.Point p4 = new com.esri.core.geometry.Point(-85.55341069949853
				, 38.26671513050464);
			polyline.startPath(p1);
			try
			{
				polyline.lineTo(p2);
				polyline.copy();
				polyline.lineTo(p3);
				polyline.copy();
				polyline.lineTo(p4);
			}
			catch (System.Exception e)
			{
				// exception thrown here!!!
				Sharpen.Runtime.printStackTrace(e);
				noException = false;
			}
			NUnit.Framework.Assert.IsTrue(noException);
		}

		// end of method
		[NUnit.Framework.Test]
		public virtual void testBoundary()
		{
			com.esri.core.geometry.Geometry g = com.esri.core.geometry.OperatorImportFromWkt.
				local().execute(0, com.esri.core.geometry.Geometry.Type.Unknown, "POLYGON((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				, null);
			com.esri.core.geometry.Geometry boundary = com.esri.core.geometry.OperatorBoundary
				.local().execute(g, null);
			com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)boundary;
			polyline.reverseAllPaths();
			string s = com.esri.core.geometry.OperatorExportToWkt.local().execute(0, boundary
				, null);
			NUnit.Framework.Assert.IsTrue(s.Equals("MULTILINESTRING ((-10 -10, 10 -10, 10 10, -10 10, -10 -10), (-5 -5, -5 5, 5 5, 5 -5, -5 -5))"
				));
		}

		[NUnit.Framework.Test]
		public virtual void testReplaceNaNs()
		{
			{
				com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
				com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
				pt.setXY(1, 2);
				pt.setZ(double.NaN);
				mp.add(pt);
				pt = new com.esri.core.geometry.Point();
				pt.setXY(11, 12);
				pt.setZ(3);
				mp.add(pt);
				mp.replaceNaNs(com.esri.core.geometry.VertexDescription.Semantics.Z, 5);
				NUnit.Framework.Assert.IsTrue(mp.getPoint(0).Equals(new com.esri.core.geometry.Point
					(1, 2, 5)));
				NUnit.Framework.Assert.IsTrue(mp.getPoint(1).Equals(new com.esri.core.geometry.Point
					(11, 12, 3)));
			}
			{
				com.esri.core.geometry.Polygon mp = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
				pt.setXY(1, 2);
				pt.setZ(double.NaN);
				mp.startPath(pt);
				pt = new com.esri.core.geometry.Point();
				pt.setXY(11, 12);
				pt.setZ(3);
				mp.lineTo(pt);
				mp.replaceNaNs(com.esri.core.geometry.VertexDescription.Semantics.Z, 5);
				NUnit.Framework.Assert.IsTrue(mp.getPoint(0).Equals(new com.esri.core.geometry.Point
					(1, 2, 5)));
				NUnit.Framework.Assert.IsTrue(mp.getPoint(1).Equals(new com.esri.core.geometry.Point
					(11, 12, 3)));
			}
			{
				com.esri.core.geometry.Polygon mp = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
				pt.setXY(1, 2);
				pt.setM(double.NaN);
				mp.startPath(pt);
				pt = new com.esri.core.geometry.Point();
				pt.setXY(11, 12);
				pt.setM(3);
				mp.lineTo(pt);
				mp.replaceNaNs(com.esri.core.geometry.VertexDescription.Semantics.M, 5);
				com.esri.core.geometry.Point p = new com.esri.core.geometry.Point(1, 2);
				p.setM(5);
				bool b = mp.getPoint(0).Equals(p);
				NUnit.Framework.Assert.IsTrue(b);
				p = new com.esri.core.geometry.Point(11, 12);
				p.setM(3);
				b = mp.getPoint(1).Equals(p);
				NUnit.Framework.Assert.IsTrue(b);
			}
		}
	}
}
