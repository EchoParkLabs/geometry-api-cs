

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestQuadTree
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
		public static void test1()
		{
			com.esri.core.geometry.Polyline polyline;
			polyline = makePolyline();
			com.esri.core.geometry.MultiPathImpl polylineImpl = (com.esri.core.geometry.MultiPathImpl
				)polyline._getImpl();
			com.esri.core.geometry.QuadTree quadtree = buildQuadTree_(polylineImpl);
			com.esri.core.geometry.Line queryline = new com.esri.core.geometry.Line(34, 9, 66
				, 46);
			com.esri.core.geometry.QuadTree.QuadTreeIterator qtIter = quadtree.getIterator();
			NUnit.Framework.Assert.IsTrue(qtIter.next() == -1);
			qtIter.resetIterator(queryline, 0.0);
			int element_handle = qtIter.next();
			while (element_handle > 0)
			{
				int index = quadtree.getElement(element_handle);
				NUnit.Framework.Assert.IsTrue(index == 6 || index == 8 || index == 14);
				element_handle = qtIter.next();
			}
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				(34, 9, 66, 46);
			com.esri.core.geometry.Polygon queryPolygon = new com.esri.core.geometry.Polygon(
				);
			queryPolygon.addEnvelope(envelope, true);
			qtIter.resetIterator(queryline, 0.0);
			element_handle = qtIter.next();
			while (element_handle > 0)
			{
				int index = quadtree.getElement(element_handle);
				NUnit.Framework.Assert.IsTrue(index == 6 || index == 8 || index == 14);
				element_handle = qtIter.next();
			}
		}

		[NUnit.Framework.Test]
		public static void test2()
		{
			com.esri.core.geometry.MultiPoint multipoint = new com.esri.core.geometry.MultiPoint
				();
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < 100; j++)
				{
					multipoint.add(i, j);
				}
			}
			com.esri.core.geometry.Envelope2D extent = new com.esri.core.geometry.Envelope2D(
				);
			multipoint.queryEnvelope2D(extent);
			com.esri.core.geometry.MultiPointImpl multipointImpl = (com.esri.core.geometry.MultiPointImpl
				)multipoint._getImpl();
			com.esri.core.geometry.QuadTree quadtree = buildQuadTree_(multipointImpl);
			com.esri.core.geometry.QuadTree.QuadTreeIterator qtIter = quadtree.getIterator();
			NUnit.Framework.Assert.IsTrue(qtIter.next() == -1);
			int count = 0;
			qtIter.resetIterator(extent, 0.0);
			while (qtIter.next() != -1)
			{
				count++;
			}
			NUnit.Framework.Assert.IsTrue(count == 10000);
		}

		public static com.esri.core.geometry.Polyline makePolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			// 0
			poly.startPath(0, 40);
			poly.lineTo(30, 0);
			// 1
			poly.startPath(20, 70);
			poly.lineTo(45, 100);
			// 2
			poly.startPath(50, 100);
			poly.lineTo(50, 60);
			// 3
			poly.startPath(35, 25);
			poly.lineTo(65, 45);
			// 4
			poly.startPath(60, 10);
			poly.lineTo(65, 35);
			// 5
			poly.startPath(60, 60);
			poly.lineTo(100, 60);
			// 6
			poly.startPath(80, 10);
			poly.lineTo(80, 99);
			// 7
			poly.startPath(60, 60);
			poly.lineTo(65, 35);
			return poly;
		}

		internal static com.esri.core.geometry.QuadTree buildQuadTree_(com.esri.core.geometry.MultiPathImpl
			 multipathImpl)
		{
			com.esri.core.geometry.Envelope2D extent = new com.esri.core.geometry.Envelope2D(
				);
			multipathImpl.queryEnvelope2D(extent);
			com.esri.core.geometry.QuadTree quadTree = new com.esri.core.geometry.QuadTree(extent
				, 8);
			int hint_index = -1;
			com.esri.core.geometry.Envelope2D boundingbox = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.SegmentIteratorImpl seg_iter = multipathImpl.querySegmentIterator
				();
			while (seg_iter.nextPath())
			{
				while (seg_iter.hasNextSegment())
				{
					com.esri.core.geometry.Segment segment = seg_iter.nextSegment();
					int index = seg_iter.getStartPointIndex();
					segment.queryEnvelope2D(boundingbox);
					hint_index = quadTree.insert(index, boundingbox, hint_index);
				}
			}
			return quadTree;
		}

		internal static com.esri.core.geometry.QuadTree buildQuadTree_(com.esri.core.geometry.MultiPointImpl
			 multipointImpl)
		{
			com.esri.core.geometry.Envelope2D extent = new com.esri.core.geometry.Envelope2D(
				);
			multipointImpl.queryEnvelope2D(extent);
			com.esri.core.geometry.QuadTree quadTree = new com.esri.core.geometry.QuadTree(extent
				, 8);
			com.esri.core.geometry.Envelope2D boundingbox = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.Point2D pt;
			for (int i = 0; i < multipointImpl.getPointCount(); i++)
			{
				pt = multipointImpl.getXY(i);
				boundingbox.setCoords(pt.x, pt.y, pt.x, pt.y);
				quadTree.insert(i, boundingbox, -1);
			}
			return quadTree;
		}
	}
}
