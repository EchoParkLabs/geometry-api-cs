/*
Copyright 2017 Echo Park Labs

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

email: info@echoparklabs.io
*/


namespace com.epl.geometry
{
	internal sealed class PointInPolygonHelper
	{
		private com.epl.geometry.Point2D m_inputPoint;

		private int m_windnum;

		private com.epl.geometry.SegmentBuffer[] m_monotoneParts = null;

		private double[] m_xOrds = null;

		private double m_tolerance;

		private double m_toleranceSqr;

		private double m_miny;

		private double m_maxy;

		private bool m_bAlternate;

		private bool m_bTestBorder;

		private bool m_bBreak;

		private bool m_bPointInAnyOuterRingTest;

		private int Result()
		{
			return m_windnum != 0 ? 1 : 0;
		}

		private bool _testBorder(com.epl.geometry.Segment seg)
		{
			double t = seg.GetClosestCoordinate(m_inputPoint, false);
			com.epl.geometry.Point2D pt = seg.GetCoord2D(t);
			if (com.epl.geometry.Point2D.SqrDistance(pt, m_inputPoint) <= m_toleranceSqr)
			{
				return true;
			}
			return false;
		}

		private void DoOne(com.epl.geometry.Segment seg)
		{
			if (!m_bTestBorder)
			{
				// test if point is on the boundary
				if (m_bAlternate && m_inputPoint.IsEqual(seg.GetStartXY()) || m_inputPoint.IsEqual(seg.GetEndXY()))
				{
					// test if the
					// point
					// coincides
					// with a vertex
					m_bBreak = true;
					return;
				}
			}
			if (seg.GetStartY() == m_inputPoint.y && seg.GetStartY() == seg.GetEndY())
			{
				// skip horizontal
				// segments. test if the
				// point lies on a
				// horizontal segment
				if (m_bAlternate && !m_bTestBorder)
				{
					double minx = System.Math.Min(seg.GetStartX(), seg.GetEndX());
					double maxx = System.Math.Max(seg.GetStartX(), seg.GetEndX());
					if (m_inputPoint.x > minx && m_inputPoint.x < maxx)
					{
						m_bBreak = true;
					}
				}
				return;
			}
			// skip horizontal segments
			bool bToTheRight = false;
			double maxx_1 = System.Math.Max(seg.GetStartX(), seg.GetEndX());
			if (m_inputPoint.x > maxx_1)
			{
				bToTheRight = true;
			}
			else
			{
				if (m_inputPoint.x >= System.Math.Min(seg.GetStartX(), seg.GetEndX()))
				{
					int n = seg.IntersectionWithAxis2D(true, m_inputPoint.y, m_xOrds, null);
					bToTheRight = n > 0 && m_xOrds[0] <= m_inputPoint.x;
				}
			}
			if (bToTheRight)
			{
				// to prevent double counting, when the ray crosses a vertex, count
				// only the segments that are below the ray.
				if (m_inputPoint.y == seg.GetStartXY().y)
				{
					if (m_inputPoint.y < seg.GetEndXY().y)
					{
						return;
					}
				}
				else
				{
					if (m_inputPoint.y == seg.GetEndXY().y)
					{
						if (m_inputPoint.y < seg.GetStartXY().y)
						{
							return;
						}
					}
				}
				if (m_bAlternate)
				{
					m_windnum ^= 1;
				}
				else
				{
					m_windnum += (seg.GetStartXY().y > seg.GetEndXY().y) ? 1 : -1;
				}
			}
		}

		public PointInPolygonHelper(bool bFillRule_Alternate, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			// //_ASSERT(tolerance >= 0);
			m_inputPoint = inputPoint;
			m_miny = inputPoint.y - tolerance;
			m_maxy = inputPoint.y + tolerance;
			m_windnum = 0;
			m_bAlternate = bFillRule_Alternate;
			m_tolerance = tolerance;
			m_toleranceSqr = tolerance * tolerance;
			m_bTestBorder = tolerance != 0;
			//
			m_bBreak = false;
		}

		private bool ProcessSegment(com.epl.geometry.Segment segment)
		{
			com.epl.geometry.Envelope1D yrange = segment.QueryInterval((int)com.epl.geometry.VertexDescription.Semantics.POSITION, 1);
			if (yrange.vmin > m_maxy || yrange.vmax < m_miny)
			{
				return false;
			}
			if (m_bTestBorder && _testBorder(segment))
			{
				return true;
			}
			if (yrange.vmin > m_inputPoint.y || yrange.vmax < m_inputPoint.y)
			{
				return false;
			}
			if (m_monotoneParts == null)
			{
				m_monotoneParts = new com.epl.geometry.SegmentBuffer[5];
			}
			if (m_xOrds == null)
			{
				m_xOrds = new double[3];
			}
			int nparts = segment.GetYMonotonicParts(m_monotoneParts);
			if (nparts > 0)
			{
				// the segment is a curve and has been broken in
				// ymonotone parts
				for (int i = 0; i < nparts; i++)
				{
					com.epl.geometry.Segment part = m_monotoneParts[i].Get();
					DoOne(part);
					if (m_bBreak)
					{
						return true;
					}
				}
			}
			else
			{
				// the segment is a line or it is y monotone curve
				DoOne(segment);
				if (m_bBreak)
				{
					return true;
				}
			}
			return false;
		}

		private static int _isPointInPolygonInternal(com.epl.geometry.Polygon inputPolygon, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			bool bAltenate = inputPolygon.GetFillRule() == com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven;
			com.epl.geometry.PointInPolygonHelper helper = new com.epl.geometry.PointInPolygonHelper(bAltenate, inputPoint, tolerance);
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)inputPolygon._getImpl();
			com.epl.geometry.SegmentIteratorImpl iter = mpImpl.QuerySegmentIterator();
			while (iter.NextPath())
			{
				while (iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = iter.NextSegment();
					if (helper.ProcessSegment(segment))
					{
						return -1;
					}
				}
			}
			// point on boundary
			return helper.Result();
		}

		private static int _isPointInPolygonInternalWithQuadTree(com.epl.geometry.Polygon inputPolygon, com.epl.geometry.QuadTreeImpl quadTree, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			com.epl.geometry.Envelope2D envPoly = new com.epl.geometry.Envelope2D();
			inputPolygon.QueryLooseEnvelope(envPoly);
			envPoly.Inflate(tolerance, tolerance);
			bool bAltenate = inputPolygon.GetFillRule() == com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven;
			com.epl.geometry.PointInPolygonHelper helper = new com.epl.geometry.PointInPolygonHelper(bAltenate, inputPoint, tolerance);
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)inputPolygon._getImpl();
			com.epl.geometry.SegmentIteratorImpl iter = mpImpl.QuerySegmentIterator();
			com.epl.geometry.Envelope2D queryEnv = new com.epl.geometry.Envelope2D();
			queryEnv.SetCoords(envPoly);
			queryEnv.xmax = inputPoint.x + tolerance;
			// no need to query segments to
			// the right of the point.
			// Only segments to the left
			// matter.
			queryEnv.ymin = inputPoint.y - tolerance;
			queryEnv.ymax = inputPoint.y + tolerance;
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qiter = quadTree.GetIterator(queryEnv, tolerance);
			for (int qhandle = qiter.Next(); qhandle != -1; qhandle = qiter.Next())
			{
				iter.ResetToVertex(quadTree.GetElement(qhandle));
				if (iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = iter.NextSegment();
					if (helper.ProcessSegment(segment))
					{
						return -1;
					}
				}
			}
			// point on boundary
			return helper.Result();
		}

		public static int IsPointInPolygon(com.epl.geometry.Polygon inputPolygon, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			if (inputPolygon.IsEmpty())
			{
				return 0;
			}
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			inputPolygon.QueryLooseEnvelope(env);
			env.Inflate(tolerance, tolerance);
			if (!env.Contains(inputPoint))
			{
				return 0;
			}
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)inputPolygon._getImpl();
			com.epl.geometry.GeometryAccelerators accel = mpImpl._getAccelerators();
			if (accel != null)
			{
				// geometry has spatial indices built. Try using them.
				com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
				if (rgeom != null)
				{
					com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(inputPoint.x, inputPoint.y);
					if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
					{
						return 1;
					}
					else
					{
						if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
						{
							return 0;
						}
					}
				}
				com.epl.geometry.QuadTreeImpl qtree = accel.GetQuadTree();
				if (qtree != null)
				{
					return _isPointInPolygonInternalWithQuadTree(inputPolygon, qtree, inputPoint, tolerance);
				}
			}
			return _isPointInPolygonInternal(inputPolygon, inputPoint, tolerance);
		}

		internal static int IsPointInPolygon(com.epl.geometry.Polygon inputPolygon, double inputPointXVal, double inputPointYVal, double tolerance)
		{
			if (inputPolygon.IsEmpty())
			{
				return 0;
			}
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			inputPolygon.QueryLooseEnvelope(env);
			env.Inflate(tolerance, tolerance);
			if (!env.Contains(inputPointXVal, inputPointYVal))
			{
				return 0;
			}
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)inputPolygon._getImpl();
			com.epl.geometry.GeometryAccelerators accel = mpImpl._getAccelerators();
			if (accel != null)
			{
				com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
				if (rgeom != null)
				{
					com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(inputPointXVal, inputPointYVal);
					if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
					{
						return 1;
					}
					else
					{
						if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
						{
							return 0;
						}
					}
				}
			}
			return _isPointInPolygonInternal(inputPolygon, new com.epl.geometry.Point2D(inputPointXVal, inputPointYVal), tolerance);
		}

		public static int IsPointInRing(com.epl.geometry.MultiPathImpl inputPolygonImpl, int iRing, com.epl.geometry.Point2D inputPoint, double tolerance, com.epl.geometry.QuadTree quadTree)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			inputPolygonImpl.QueryLooseEnvelope2D(env);
			env.Inflate(tolerance, tolerance);
			if (!env.Contains(inputPoint))
			{
				return 0;
			}
			bool bAltenate = true;
			com.epl.geometry.PointInPolygonHelper helper = new com.epl.geometry.PointInPolygonHelper(bAltenate, inputPoint, tolerance);
			if (quadTree != null)
			{
				com.epl.geometry.Envelope2D queryEnv = new com.epl.geometry.Envelope2D();
				queryEnv.SetCoords(env);
				queryEnv.xmax = inputPoint.x + tolerance;
				// no need to query
				// segments to
				// the right of the
				// point.
				// Only segments to the
				// left
				// matter.
				queryEnv.ymin = inputPoint.y - tolerance;
				queryEnv.ymax = inputPoint.y + tolerance;
				com.epl.geometry.SegmentIteratorImpl iter = inputPolygonImpl.QuerySegmentIterator();
				com.epl.geometry.QuadTree.QuadTreeIterator qiter = quadTree.GetIterator(queryEnv, tolerance);
				for (int qhandle = qiter.Next(); qhandle != -1; qhandle = qiter.Next())
				{
					iter.ResetToVertex(quadTree.GetElement(qhandle), iRing);
					if (iter.HasNextSegment())
					{
						if (iter.GetPathIndex() != iRing)
						{
							continue;
						}
						com.epl.geometry.Segment segment = iter.NextSegment();
						if (helper.ProcessSegment(segment))
						{
							return -1;
						}
					}
				}
				// point on boundary
				return helper.Result();
			}
			else
			{
				com.epl.geometry.SegmentIteratorImpl iter = inputPolygonImpl.QuerySegmentIterator();
				iter.ResetToPath(iRing);
				if (iter.NextPath())
				{
					while (iter.HasNextSegment())
					{
						com.epl.geometry.Segment segment = iter.NextSegment();
						if (helper.ProcessSegment(segment))
						{
							return -1;
						}
					}
				}
				// point on boundary
				return helper.Result();
			}
		}

		public static int IsPointInPolygon(com.epl.geometry.Polygon inputPolygon, com.epl.geometry.Point inputPoint, double tolerance)
		{
			if (inputPoint.IsEmpty())
			{
				return 0;
			}
			return IsPointInPolygon(inputPolygon, inputPoint.GetXY(), tolerance);
		}

		public static int IsPointInAnyOuterRing(com.epl.geometry.Polygon inputPolygon, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			inputPolygon.QueryLooseEnvelope(env);
			env.Inflate(tolerance, tolerance);
			if (!env.Contains(inputPoint))
			{
				return 0;
			}
			// Note:
			// Wolfgang had noted that this could be optimized if the exterior rings
			// have positive area:
			// Only test the positive rings and bail out immediately when in a
			// positive ring.
			// The worst case complexity is still O(n), but on average for polygons
			// with holes, that would be faster.
			// However, that method would not work if polygon is reversed, while the
			// one here works fine same as PointInPolygon.
			bool bAltenate = false;
			// use winding in this test
			com.epl.geometry.PointInPolygonHelper helper = new com.epl.geometry.PointInPolygonHelper(bAltenate, inputPoint, tolerance);
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)inputPolygon._getImpl();
			com.epl.geometry.SegmentIteratorImpl iter = mpImpl.QuerySegmentIterator();
			while (iter.NextPath())
			{
				double ringArea = mpImpl.CalculateRingArea2D(iter.GetPathIndex());
				bool bIsHole = ringArea < 0;
				if (!bIsHole)
				{
					helper.m_windnum = 0;
					while (iter.HasNextSegment())
					{
						com.epl.geometry.Segment segment = iter.NextSegment();
						if (helper.ProcessSegment(segment))
						{
							return -1;
						}
					}
					// point on boundary
					if (helper.m_windnum != 0)
					{
						return 1;
					}
				}
			}
			return helper.Result();
		}

		// Tests if Ring1 is inside Ring2.
		// We assume here that the Polygon is Weak Simple. That is if one point of
		// Ring1 is found to be inside of Ring2, then
		// we assume that all of Ring1 is inside Ring2.
		internal static bool _isRingInRing2D(com.epl.geometry.MultiPath polygon, int iRing1, int iRing2, double tolerance, com.epl.geometry.QuadTree quadTree)
		{
			com.epl.geometry.MultiPathImpl polygonImpl = (com.epl.geometry.MultiPathImpl)polygon._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIter = polygonImpl.QuerySegmentIterator();
			segIter.ResetToPath(iRing1);
			if (!segIter.NextPath() || !segIter.HasNextSegment())
			{
				throw new com.epl.geometry.GeometryException("corrupted geometry");
			}
			int res = 2;
			while (res == 2 && segIter.HasNextSegment())
			{
				com.epl.geometry.Segment segment = segIter.NextSegment();
				com.epl.geometry.Point2D point = segment.GetCoord2D(0.5);
				res = com.epl.geometry.PointInPolygonHelper.IsPointInRing(polygonImpl, iRing2, point, tolerance, quadTree);
			}
			if (res == 2)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			if (res == 1)
			{
				return true;
			}
			return false;
		}

		internal static bool QuadTreeWillHelp(com.epl.geometry.Polygon polygon, int c_queries)
		{
			int n = polygon.GetPointCount();
			if (n < 16)
			{
				return false;
			}
			double c_build_quad_tree = 2.0;
			// what's a good constant?
			double c_query_quad_tree = 1.0;
			// what's a good constant?
			double c_point_in_polygon_brute_force = 1.0;
			// what's a good constant?
			double c_quad_tree = c_build_quad_tree * n + c_query_quad_tree * (System.Math.Log((double)n) / System.Math.Log(2.0)) * c_queries;
			double c_brute_force = c_point_in_polygon_brute_force * n * c_queries;
			return c_quad_tree < c_brute_force;
		}
	}
}
