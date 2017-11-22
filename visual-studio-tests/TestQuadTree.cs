using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestQuadTree : NUnit.Framework.TestFixtureAttribute
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
		public static void Test1()
		{
			com.epl.geometry.Polyline polyline;
			polyline = MakePolyline();
			com.epl.geometry.MultiPathImpl polylineImpl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
			com.epl.geometry.QuadTree quadtree = BuildQuadTree_(polylineImpl);
			com.epl.geometry.Line queryline = new com.epl.geometry.Line(34, 9, 66, 46);
			com.epl.geometry.QuadTree.QuadTreeIterator qtIter = quadtree.GetIterator();
			NUnit.Framework.Assert.IsTrue(qtIter.Next() == -1);
			qtIter.ResetIterator(queryline, 0.0);
			int element_handle = qtIter.Next();
			while (element_handle > 0)
			{
				int index = quadtree.GetElement(element_handle);
				NUnit.Framework.Assert.IsTrue(index == 6 || index == 8 || index == 14);
				element_handle = qtIter.Next();
			}
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D(34, 9, 66, 46);
			com.epl.geometry.Polygon queryPolygon = new com.epl.geometry.Polygon();
			queryPolygon.AddEnvelope(envelope, true);
			qtIter.ResetIterator(queryline, 0.0);
			element_handle = qtIter.Next();
			while (element_handle > 0)
			{
				int index = quadtree.GetElement(element_handle);
				NUnit.Framework.Assert.IsTrue(index == 6 || index == 8 || index == 14);
				element_handle = qtIter.Next();
			}
		}

		[NUnit.Framework.Test]
		public static void Test2()
		{
			com.epl.geometry.MultiPoint multipoint = new com.epl.geometry.MultiPoint();
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < 100; j++)
				{
					multipoint.Add(i, j);
				}
			}
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipoint.QueryEnvelope2D(extent);
			com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
			com.epl.geometry.QuadTree quadtree = BuildQuadTree_(multipointImpl);
			com.epl.geometry.QuadTree.QuadTreeIterator qtIter = quadtree.GetIterator();
			NUnit.Framework.Assert.IsTrue(qtIter.Next() == -1);
			int count = 0;
			qtIter.ResetIterator(extent, 0.0);
			while (qtIter.Next() != -1)
			{
				count++;
			}
			NUnit.Framework.Assert.IsTrue(count == 10000);
		}

		public static com.epl.geometry.Polyline MakePolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			// 0
			poly.StartPath(0, 40);
			poly.LineTo(30, 0);
			// 1
			poly.StartPath(20, 70);
			poly.LineTo(45, 100);
			// 2
			poly.StartPath(50, 100);
			poly.LineTo(50, 60);
			// 3
			poly.StartPath(35, 25);
			poly.LineTo(65, 45);
			// 4
			poly.StartPath(60, 10);
			poly.LineTo(65, 35);
			// 5
			poly.StartPath(60, 60);
			poly.LineTo(100, 60);
			// 6
			poly.StartPath(80, 10);
			poly.LineTo(80, 99);
			// 7
			poly.StartPath(60, 60);
			poly.LineTo(65, 35);
			return poly;
		}

		internal static com.epl.geometry.QuadTree BuildQuadTree_(com.epl.geometry.MultiPathImpl multipathImpl)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipathImpl.QueryEnvelope2D(extent);
			com.epl.geometry.QuadTree quadTree = new com.epl.geometry.QuadTree(extent, 8);
			int hint_index = -1;
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			com.epl.geometry.SegmentIteratorImpl seg_iter = multipathImpl.QuerySegmentIterator();
			while (seg_iter.NextPath())
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					int index = seg_iter.GetStartPointIndex();
					segment.QueryEnvelope2D(boundingbox);
					hint_index = quadTree.Insert(index, boundingbox, hint_index);
				}
			}
			return quadTree;
		}

		internal static com.epl.geometry.QuadTree BuildQuadTree_(com.epl.geometry.MultiPointImpl multipointImpl)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipointImpl.QueryEnvelope2D(extent);
			com.epl.geometry.QuadTree quadTree = new com.epl.geometry.QuadTree(extent, 8);
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Point2D pt;
			for (int i = 0; i < multipointImpl.GetPointCount(); i++)
			{
				pt = multipointImpl.GetXY(i);
				boundingbox.SetCoords(pt.x, pt.y, pt.x, pt.y);
				quadTree.Insert(i, boundingbox, -1);
			}
			return quadTree;
		}
	}
}
