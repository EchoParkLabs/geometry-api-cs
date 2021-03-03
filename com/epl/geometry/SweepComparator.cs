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
/*
* To change this template, choose Tools | Templates
* and open the template in the editor.
*/


namespace com.epl.geometry
{
	internal class SweepComparator : com.epl.geometry.Treap.Comparator
	{
		internal sealed class SimpleEdge
		{
			internal int m_value;

			internal com.epl.geometry.Line m_line;

			internal com.epl.geometry.Segment m_segment;

			internal com.epl.geometry.Envelope1D m_env;

			internal double m_dxdy;

			internal bool m_b_horizontal;

			internal bool m_b_curve;

			internal SimpleEdge()
			{
				m_value = -1;
				m_line = new com.epl.geometry.Line();
				m_dxdy = 55555555;
				m_b_horizontal = false;
				m_b_curve = false;
				m_env = new com.epl.geometry.Envelope1D();
				m_env.SetCoordsNoNaN_(0, 0);
			}
		}

		private com.epl.geometry.EditShape m_shape;

		internal bool m_b_intersection_detected;

		internal com.epl.geometry.NonSimpleResult m_non_simple_result;

		internal com.epl.geometry.SweepComparator.SimpleEdge m_temp_simple_edge_1;

		internal com.epl.geometry.SweepComparator.SimpleEdge m_temp_simple_edge_2;

		internal int m_prev_1;

		internal int m_prev_2;

		internal int m_vertex_1;

		internal int m_vertex_2;

		internal int m_current_node;

		internal double m_prevx_1;

		internal double m_prevx_2;

		internal double m_prev_y;

		internal double m_prev_x;

		internal double m_sweep_y;

		internal double m_sweep_x;

		internal double m_tolerance;

		internal double m_tolerance_10;

		internal bool m_b_is_simple;

		internal System.Collections.Generic.List<com.epl.geometry.SweepComparator.SimpleEdge> m_simple_edges_cache;

		internal System.Collections.Generic.List<com.epl.geometry.SweepComparator.SimpleEdge> m_simple_edges_recycle;

		internal System.Collections.Generic.List<com.epl.geometry.SweepComparator.SimpleEdge> m_simple_edges_buffer;

		// Index 1 corresponds to the left segments, index 2 - right, e.g. m_line_1,
		// m_line_2
		// Returns a cached edge for the given value. May return NULL.
		internal virtual com.epl.geometry.SweepComparator.SimpleEdge TryGetCachedEdge_(int value)
		{
			com.epl.geometry.SweepComparator.SimpleEdge se = m_simple_edges_cache[(value & com.epl.geometry.NumberUtils.IntMax()) % m_simple_edges_cache.Count];
			if (se != null)
			{
				if (se.m_value == value)
				{
					return se;
				}
			}
			// int i = 0;
			// cache collision
			return null;
		}

		// Removes cached edge from the cache for the given value.
		internal virtual void TryDeleteCachedEdge_(int value)
		{
			int ind = (value & com.epl.geometry.NumberUtils.IntMax()) % m_simple_edges_cache.Count;
			com.epl.geometry.SweepComparator.SimpleEdge se = m_simple_edges_cache[ind];
			if (se != null && se.m_value == value)
			{
				// this value is cached
				m_simple_edges_recycle.Add(se);
				m_simple_edges_cache[ind] = null;
			}
		}

		// The value has not been cached
		// Creates a cached edge. May fail and return NULL.
		internal virtual com.epl.geometry.SweepComparator.SimpleEdge TryCreateCachedEdge_(int value)
		{
			int ind = (value & com.epl.geometry.NumberUtils.IntMax()) % m_simple_edges_cache.Count;
			com.epl.geometry.SweepComparator.SimpleEdge se = m_simple_edges_cache[ind];
			if (se == null)
			{
				if ((m_simple_edges_recycle.Count == 0))
				{
					// assert(m_simple_edges_buffer.size() <
					// m_simple_edges_buffer.capacity());//should never happen
					// assert(m_simple_edges_buffer.size() <
					// m_simple_edges_cache.size());//should never happen
					m_simple_edges_buffer.Add(new com.epl.geometry.SweepComparator.SimpleEdge());
					se = m_simple_edges_buffer[m_simple_edges_buffer.Count - 1];
				}
				else
				{
					se = m_simple_edges_recycle[m_simple_edges_recycle.Count - 1];
					m_simple_edges_recycle.RemoveAt(m_simple_edges_recycle.Count - 1);
				}
				se.m_value = value;
				m_simple_edges_cache[ind] = se;
				return se;
			}
			else
			{
				System.Diagnostics.Debug.Assert((se.m_value != value));
			}
			// do not call TryCreateCachedEdge
			// twice.
			return null;
		}

		internal virtual void InitSimpleEdge_(com.epl.geometry.SweepComparator.SimpleEdge se, int vertex)
		{
			se.m_segment = m_shape.GetSegment(vertex);
			se.m_b_curve = se.m_segment != null;
			if (!se.m_b_curve)
			{
				m_shape.QueryLineConnector(vertex, se.m_line);
				se.m_segment = se.m_line;
				se.m_env.SetCoordsNoNaN_(se.m_line.GetStartX(), se.m_line.GetEndX());
				se.m_env.vmax += m_tolerance;
				se.m_line.OrientBottomUp_();
				se.m_b_horizontal = se.m_line.GetEndY() == se.m_line.GetStartY();
				if (!se.m_b_horizontal)
				{
					se.m_dxdy = (se.m_line.GetEndX() - se.m_line.GetStartX()) / (se.m_line.GetEndY() - se.m_line.GetStartY());
				}
			}
		}

		// se.m_segment = se.m_segment_sptr.get();
		// Compares seg_1 and seg_2 x coordinates of intersection with the line
		// parallel to axis x, passing through the coordinate y.
		// If segments intersect not at the endpoint, the m_b_intersection_detected
		// is set.
		internal virtual int CompareTwoSegments_(com.epl.geometry.Segment seg_1, com.epl.geometry.Segment seg_2)
		{
			int res = seg_1._isIntersecting(seg_2, m_tolerance, true);
			if (res != 0)
			{
				if (res == 2)
				{
					return ErrorCoincident();
				}
				else
				{
					return ErrorCracking();
				}
			}
			com.epl.geometry.Point2D start_1 = seg_1.GetStartXY();
			com.epl.geometry.Point2D end1 = seg_1.GetEndXY();
			com.epl.geometry.Point2D start2 = seg_2.GetStartXY();
			com.epl.geometry.Point2D end2 = seg_2.GetEndXY();
			com.epl.geometry.Point2D ptSweep = new com.epl.geometry.Point2D();
			ptSweep.SetCoords(m_sweep_x, m_sweep_y);
			if (start_1.IsEqual(start2) && m_sweep_y == start_1.y)
			{
				System.Diagnostics.Debug.Assert((start_1.Compare(end1) < 0 && start2.Compare(end2) < 0));
				if (end1.Compare(end2) < 0)
				{
					ptSweep.SetCoords(end1);
				}
				else
				{
					ptSweep.SetCoords(end2);
				}
			}
			else
			{
				if (start_1.IsEqual(end2) && m_sweep_y == start_1.y)
				{
					System.Diagnostics.Debug.Assert((start_1.Compare(end1) < 0 && start2.Compare(end2) > 0));
					if (end1.Compare(start2) < 0)
					{
						ptSweep.SetCoords(end1);
					}
					else
					{
						ptSweep.SetCoords(start2);
					}
				}
				else
				{
					if (start2.IsEqual(end1) && m_sweep_y == start2.y)
					{
						System.Diagnostics.Debug.Assert((end1.Compare(start_1) < 0 && start2.Compare(end2) < 0));
						if (start_1.Compare(end2) < 0)
						{
							ptSweep.SetCoords(start_1);
						}
						else
						{
							ptSweep.SetCoords(end2);
						}
					}
					else
					{
						if (end1.IsEqual(end2) && m_sweep_y == end1.y)
						{
							System.Diagnostics.Debug.Assert((start_1.Compare(end1) > 0 && start2.Compare(end2) > 0));
							if (start_1.Compare(start2) < 0)
							{
								ptSweep.SetCoords(start_1);
							}
							else
							{
								ptSweep.SetCoords(start2);
							}
						}
					}
				}
			}
			double xleft = seg_1.IntersectionOfYMonotonicWithAxisX(ptSweep.y, ptSweep.x);
			double xright = seg_2.IntersectionOfYMonotonicWithAxisX(ptSweep.y, ptSweep.x);
			System.Diagnostics.Debug.Assert((xleft != xright));
			return xleft < xright ? -1 : 1;
		}

		internal virtual int CompareNonHorizontal_(com.epl.geometry.SweepComparator.SimpleEdge line_1, com.epl.geometry.SweepComparator.SimpleEdge line_2)
		{
			if (line_1.m_line.GetStartY() == line_2.m_line.GetStartY() && line_1.m_line.GetStartX() == line_2.m_line.GetStartX())
			{
				// connected
				// at
				// the
				// start
				// V
				// shape
				if (line_1.m_line.GetEndY() == line_2.m_line.GetEndY() && line_1.m_line.GetEndX() == line_2.m_line.GetEndX())
				{
					// connected
					// at
					// another
					// end
					// also
					if (m_b_is_simple)
					{
						return ErrorCoincident();
					}
					return 0;
				}
				return CompareNonHorizontalUpperEnd_(line_1, line_2);
			}
			if (line_1.m_line.GetEndY() == line_2.m_line.GetEndY() && line_1.m_line.GetEndX() == line_2.m_line.GetEndX())
			{
				// the case of upside-down V.
				return CompareNonHorizontalLowerEnd_(line_1, line_2);
			}
			int lower = CompareNonHorizontalLowerEnd_(line_1, line_2);
			int upper = CompareNonHorizontalUpperEnd_(line_1, line_2);
			if (lower < 0 && upper < 0)
			{
				return -1;
			}
			if (lower > 0 && upper > 0)
			{
				return 1;
			}
			return ErrorCracking();
		}

		internal virtual int CompareHorizontal1Case1_(com.epl.geometry.Line line_1, com.epl.geometry.Line line_2)
		{
			// line_2 goes up and line_1 is horizontal connected at the start going
			// to the right.
			if (line_1.GetEndX() > line_2.GetEndX())
			{
				// /
				// /
				// +------------------
				if (line_2.GetEndX() > line_2.GetStartX() && line_2.GetEndY() - line_2.GetStartY() < 2 * m_tolerance && line_1._isIntersectingPoint(line_2.GetEndXY(), m_tolerance, true))
				{
					return ErrorCracking();
				}
			}
			else
			{
				// /
				// /
				// /
				// +--
				System.Diagnostics.Debug.Assert((line_2.GetEndX() - line_2.GetStartX() != 0));
				// Note: line_2 cannot be vertical here
				// Avoid expensive is_intersecting_ by providing a simple estimate.
				double dydx = (line_2.GetEndY() - line_2.GetStartY()) / (line_2.GetEndX() - line_2.GetStartX());
				double d = dydx * (line_1.GetEndX() - line_1.GetStartX());
				if (d < m_tolerance_10 && line_2._isIntersectingPoint(line_1.GetEndXY(), m_tolerance, true))
				{
					return ErrorCracking();
				}
			}
			return 1;
		}

		internal virtual int CompareHorizontal1Case2_(com.epl.geometry.Line line_1, com.epl.geometry.Line line_2)
		{
			// -----------------+
			// /
			// /
			// /
			// line_2 goes up and below line_1. line_1 is horizontal connected at
			// the end to the line_2 end.
			if (line_1.GetStartX() < line_2.GetStartX())
			{
				if (line_2.GetEndX() > line_2.GetStartX() && line_2.GetEndY() - line_2.GetStartY() < 2 * m_tolerance && line_1._isIntersectingPoint(line_2.GetEndXY(), m_tolerance, true))
				{
					return ErrorCracking();
				}
			}
			else
			{
				// --+
				// /
				// /
				// /
				// Avoid expensive is_intersecting_ by providing a simple estimate.
				double dydx = (line_2.GetEndY() - line_2.GetStartY()) / (line_2.GetEndX() - line_2.GetStartX());
				double d = dydx * (line_1.GetStartX() - line_1.GetEndX());
				if (d < m_tolerance_10 && line_2._isIntersectingPoint(line_1.GetStartXY(), m_tolerance, true))
				{
					return ErrorCracking();
				}
			}
			return -1;
		}

		internal virtual int CompareHorizontal1Case3_(com.epl.geometry.Line line_1, com.epl.geometry.Line line_2)
		{
			com.epl.geometry.Point2D v0 = new com.epl.geometry.Point2D();
			v0.Sub(line_2.GetEndXY(), line_2.GetStartXY());
			v0.RightPerpendicular();
			v0.Normalize();
			com.epl.geometry.Point2D v_1 = new com.epl.geometry.Point2D();
			v_1.Sub(line_1.GetStartXY(), line_2.GetStartXY());
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			v_2.Sub(line_1.GetEndXY(), line_2.GetStartXY());
			double d_1 = v_1.DotProduct(v0);
			double d_2 = v_2.DotProduct(v0);
			double ad1 = System.Math.Abs(d_1);
			double ad2 = System.Math.Abs(d_2);
			if (ad1 < ad2)
			{
				if (ad1 < m_tolerance_10 && line_2._isIntersectingPoint(line_1.GetStartXY(), m_tolerance, true))
				{
					return ErrorCracking();
				}
			}
			else
			{
				if (ad2 < m_tolerance_10 && line_2._isIntersectingPoint(line_1.GetEndXY(), m_tolerance, true))
				{
					return ErrorCracking();
				}
			}
			if (d_1 < 0 && d_2 < 0)
			{
				return -1;
			}
			if (d_1 > 0 && d_2 > 0)
			{
				return 1;
			}
			return ErrorCracking();
		}

		internal virtual int CompareHorizontal1_(com.epl.geometry.Line line_1, com.epl.geometry.Line line_2)
		{
			// Two most important cases of connecting edges
			if (line_1.GetStartY() == line_2.GetStartY() && line_1.GetStartX() == line_2.GetStartX())
			{
				return CompareHorizontal1Case1_(line_1, line_2);
			}
			if (line_1.GetEndY() == line_2.GetEndY() && line_1.GetEndX() == line_2.GetEndX())
			{
				return CompareHorizontal1Case2_(line_1, line_2);
			}
			return CompareHorizontal1Case3_(line_1, line_2);
		}

		internal virtual int CompareHorizontal2_(com.epl.geometry.Line line_1, com.epl.geometry.Line line_2)
		{
			if (line_1.GetEndY() == line_2.GetEndY() && line_1.GetEndX() == line_2.GetEndX() && line_1.GetStartY() == line_2.GetStartY() && line_1.GetStartX() == line_2.GetStartX())
			{
				// both lines
				// coincide
				if (m_b_is_simple)
				{
					return ErrorCoincident();
				}
				return 0;
			}
			else
			{
				return ErrorCracking();
			}
		}

		internal virtual int CompareNonHorizontalLowerEnd_(com.epl.geometry.SweepComparator.SimpleEdge line_1, com.epl.geometry.SweepComparator.SimpleEdge line_2)
		{
			int sign = 1;
			if (line_1.m_line.GetStartY() < line_2.m_line.GetStartY())
			{
				sign = -1;
				com.epl.geometry.SweepComparator.SimpleEdge tmp = line_1;
				line_1 = line_2;
				line_2 = tmp;
			}
			com.epl.geometry.Line l1 = line_1.m_line;
			com.epl.geometry.Line l2 = line_2.m_line;
			// Now line_1 has Start point higher than line_2 startpoint.
			double x_1 = l1.GetStartX() - l2.GetStartX();
			double x2 = line_2.m_dxdy * (l1.GetStartY() - l2.GetStartY());
			double tol = m_tolerance_10;
			if (x_1 < x2 - tol)
			{
				return -sign;
			}
			else
			{
				if (x_1 > x2 + tol)
				{
					return sign;
				}
				else
				{
					// Possible problem
					if (l2._isIntersectingPoint(l1.GetStartXY(), m_tolerance, true))
					{
						return ErrorCracking();
					}
					return x_1 < x2 ? -sign : sign;
				}
			}
		}

		internal virtual int CompareNonHorizontalUpperEnd_(com.epl.geometry.SweepComparator.SimpleEdge line_1, com.epl.geometry.SweepComparator.SimpleEdge line_2)
		{
			int sign = 1;
			if (line_2.m_line.GetEndY() < line_1.m_line.GetEndY())
			{
				sign = -1;
				com.epl.geometry.SweepComparator.SimpleEdge tmp = line_1;
				line_1 = line_2;
				line_2 = tmp;
			}
			com.epl.geometry.Line l1 = line_1.m_line;
			com.epl.geometry.Line l2 = line_2.m_line;
			// Now line_1 has End point lower than line_2 endpoint.
			double x_1 = l1.GetEndX() - l2.GetStartX();
			double x2 = line_2.m_dxdy * (l1.GetEndY() - l2.GetStartY());
			double tol = m_tolerance_10;
			if (x_1 < x2 - tol)
			{
				return -sign;
			}
			else
			{
				if (x_1 > x2 + tol)
				{
					return sign;
				}
				else
				{
					// Possible problem
					if (l2._isIntersectingPoint(l1.GetEndXY(), m_tolerance, true))
					{
						return ErrorCracking();
					}
					return x_1 < x2 ? -sign : sign;
				}
			}
		}

		internal virtual int ErrorCoincident()
		{
			// two segments coincide.
			m_b_intersection_detected = true;
			System.Diagnostics.Debug.Assert((m_b_is_simple));
			com.epl.geometry.NonSimpleResult.Reason reason = com.epl.geometry.NonSimpleResult.Reason.CrossOver;
			m_non_simple_result = new com.epl.geometry.NonSimpleResult(reason, m_vertex_1, m_vertex_2);
			return -1;
		}

		internal virtual int ErrorCracking()
		{
			// cracking error
			m_b_intersection_detected = true;
			if (m_b_is_simple)
			{
				// only report the reason in IsSimple. Do not do
				// that for regular cracking.
				com.epl.geometry.NonSimpleResult.Reason reason = com.epl.geometry.NonSimpleResult.Reason.Cracking;
				m_non_simple_result = new com.epl.geometry.NonSimpleResult(reason, m_vertex_1, m_vertex_2);
			}
			else
			{
				// reset cached data after detected intersection
				m_prev_1 = -1;
				m_prev_2 = -1;
				m_vertex_1 = -1;
				m_vertex_2 = -1;
			}
			return -1;
		}

		internal virtual int CompareSegments_(int left, int right, com.epl.geometry.SweepComparator.SimpleEdge segLeft, com.epl.geometry.SweepComparator.SimpleEdge segRight)
		{
			if (m_b_intersection_detected)
			{
				return -1;
			}
			bool sameY = m_prev_y == m_sweep_y && m_prev_x == m_sweep_x;
			double xleft;
			if (sameY && left == m_prev_1)
			{
				xleft = m_prevx_1;
			}
			else
			{
				xleft = com.epl.geometry.NumberUtils.NaN();
				m_prev_1 = -1;
			}
			double xright;
			if (sameY && right == m_prev_2)
			{
				xright = m_prevx_2;
			}
			else
			{
				xright = com.epl.geometry.NumberUtils.NaN();
				m_prev_2 = -1;
			}
			// Quickly compare x projections.
			com.epl.geometry.Envelope1D envLeft = segLeft.m_segment.QueryInterval(com.epl.geometry.VertexDescription.Semantics.POSITION, 0);
			com.epl.geometry.Envelope1D envRight = segRight.m_segment.QueryInterval(com.epl.geometry.VertexDescription.Semantics.POSITION, 0);
			if (envLeft.vmax < envRight.vmin)
			{
				return -1;
			}
			if (envRight.vmax < envLeft.vmin)
			{
				return 1;
			}
			m_prev_y = m_sweep_y;
			m_prev_x = m_sweep_x;
			// Now do intersection with the sweep line (it is a line parallel to the
			// axis x.)
			if (com.epl.geometry.NumberUtils.IsNaN(xleft))
			{
				m_prev_1 = left;
				double x = segLeft.m_segment.IntersectionOfYMonotonicWithAxisX(m_sweep_y, m_sweep_x);
				xleft = x;
				m_prevx_1 = x;
			}
			if (com.epl.geometry.NumberUtils.IsNaN(xright))
			{
				m_prev_2 = right;
				double x = segRight.m_segment.IntersectionOfYMonotonicWithAxisX(m_sweep_y, m_sweep_x);
				xright = x;
				m_prevx_2 = x;
			}
			if (System.Math.Abs(xleft - xright) <= m_tolerance)
			{
				// special processing as we cannot decide in a simple way.
				return CompareTwoSegments_(segLeft.m_segment, segRight.m_segment);
			}
			else
			{
				return xleft < xright ? -1 : xleft > xright ? 1 : 0;
			}
		}

		internal SweepComparator(com.epl.geometry.EditShape shape, double tol, bool bIsSimple)
			: base(true)
		{
			m_shape = shape;
			m_sweep_y = com.epl.geometry.NumberUtils.TheNaN;
			m_sweep_x = 0;
			m_prev_x = 0;
			m_prev_y = com.epl.geometry.NumberUtils.TheNaN;
			m_tolerance = tol;
			m_tolerance_10 = 10 * tol;
			m_prevx_2 = com.epl.geometry.NumberUtils.TheNaN;
			m_prevx_1 = com.epl.geometry.NumberUtils.TheNaN;
			m_b_intersection_detected = false;
			m_prev_1 = -1;
			m_prev_2 = -1;
			m_vertex_1 = -1;
			m_vertex_2 = -1;
			m_current_node = -1;
			m_b_is_simple = bIsSimple;
			m_temp_simple_edge_1 = new com.epl.geometry.SweepComparator.SimpleEdge();
			m_temp_simple_edge_2 = new com.epl.geometry.SweepComparator.SimpleEdge();
			int s = System.Math.Min(shape.GetTotalPointCount() * 3 / 2, (int)(67));
			/* SIMPLEDGE_CACHESIZE */
			int cache_size = System.Math.Min((int)7, s);
			// m_simple_edges_buffer.reserve(cache_size);//must be reserved and
			// never grow beyond reserved size
			m_simple_edges_buffer = new System.Collections.Generic.List<com.epl.geometry.SweepComparator.SimpleEdge>();
			m_simple_edges_recycle = new System.Collections.Generic.List<com.epl.geometry.SweepComparator.SimpleEdge>();
			m_simple_edges_cache = new System.Collections.Generic.List<com.epl.geometry.SweepComparator.SimpleEdge>();
			for (int i = 0; i < cache_size; i++)
			{
				m_simple_edges_cache.Add(null);
			}
		}

		// Makes the comparator to forget about the last detected intersection.
		// Need to be called after the intersection has been resolved.
		internal virtual void ClearIntersectionDetectedFlag()
		{
			m_b_intersection_detected = false;
		}

		// Returns True if there has been intersection detected during compare call.
		// Once intersection is detected subsequent calls to compare method do
		// nothing until clear_intersection_detected_flag is called.
		internal virtual bool IntersectionDetected()
		{
			return m_b_intersection_detected;
		}

		// Returns the node at which the intersection has been detected
		internal virtual int GetLastComparedNode()
		{
			return m_current_node;
		}

		// When used in IsSimple (see corresponding parameter in ctor), returns the
		// reason of non-simplicity
		internal virtual com.epl.geometry.NonSimpleResult GetResult()
		{
			return m_non_simple_result;
		}

		// Sets new sweep line position.
		internal virtual void SetSweepY(double y, double x)
		{
			// _ASSERT(m_sweep_y != y || m_sweep_x != x);
			m_sweep_y = y;
			m_sweep_x = x;
			m_prev_1 = -1;
			m_prev_2 = -1;
			m_vertex_1 = -1;
			m_vertex_2 = -1;
		}

		// The compare method. Compares x values of the edge given by its origin
		// (elm) and the edge in the sweep structure and checks them for
		// intersection at the same time.
		internal override int Compare(com.epl.geometry.Treap treap, int left, int node)
		{
			// Compares two segments on a sweep line passing through m_sweep_y,
			// m_sweep_x.
			if (m_b_intersection_detected)
			{
				return -1;
			}
			int right = treap.GetElement(node);
			m_current_node = node;
			return CompareSegments(left, left, right, right);
		}

		internal virtual int CompareSegments(int leftElm, int left_vertex, int right_elm, int right_vertex)
		{
			com.epl.geometry.SweepComparator.SimpleEdge edgeLeft = TryGetCachedEdge_(leftElm);
			if (edgeLeft == null)
			{
				if (m_vertex_1 == left_vertex)
				{
					edgeLeft = m_temp_simple_edge_1;
				}
				else
				{
					m_vertex_1 = left_vertex;
					edgeLeft = TryCreateCachedEdge_(leftElm);
					if (edgeLeft == null)
					{
						edgeLeft = m_temp_simple_edge_1;
						m_temp_simple_edge_1.m_value = leftElm;
					}
					InitSimpleEdge_(edgeLeft, left_vertex);
				}
			}
			else
			{
				m_vertex_1 = left_vertex;
			}
			com.epl.geometry.SweepComparator.SimpleEdge edgeRight = TryGetCachedEdge_(right_elm);
			if (edgeRight == null)
			{
				if (m_vertex_2 == right_vertex)
				{
					edgeRight = m_temp_simple_edge_2;
				}
				else
				{
					m_vertex_2 = right_vertex;
					edgeRight = TryCreateCachedEdge_(right_elm);
					if (edgeRight == null)
					{
						edgeRight = m_temp_simple_edge_2;
						m_temp_simple_edge_2.m_value = right_elm;
					}
					InitSimpleEdge_(edgeRight, right_vertex);
				}
			}
			else
			{
				m_vertex_2 = right_vertex;
			}
			if (edgeLeft.m_b_curve || edgeRight.m_b_curve)
			{
				return CompareSegments_(left_vertex, right_vertex, edgeLeft, edgeRight);
			}
			// Usually we work with lines, so process them in the fastest way.
			// First check - assume segments are far apart. compare x intervals
			if (edgeLeft.m_env.vmax < edgeRight.m_env.vmin)
			{
				return -1;
			}
			if (edgeRight.m_env.vmax < edgeLeft.m_env.vmin)
			{
				return 1;
			}
			// compare case by case.
			int kind = edgeLeft.m_b_horizontal ? 1 : 0;
			kind |= edgeRight.m_b_horizontal ? 2 : 0;
			if (kind == 0)
			{
				// both segments are non-horizontal
				return CompareNonHorizontal_(edgeLeft, edgeRight);
			}
			else
			{
				if (kind == 1)
				{
					// line_1 horizontal, line_2 is not
					return CompareHorizontal1_(edgeLeft.m_line, edgeRight.m_line);
				}
				else
				{
					if (kind == 2)
					{
						// line_2 horizontal, line_1 is not
						return CompareHorizontal1_(edgeRight.m_line, edgeLeft.m_line) * -1;
					}
					else
					{
						// if (kind == 3) //both horizontal
						return CompareHorizontal2_(edgeLeft.m_line, edgeRight.m_line);
					}
				}
			}
		}

		internal override void OnDelete(int elm)
		{
			TryDeleteCachedEdge_(elm);
		}

		internal override void OnSet(int oldelm)
		{
			TryDeleteCachedEdge_(oldelm);
		}

		internal override void OnEndSearch(int elm)
		{
			TryDeleteCachedEdge_(elm);
		}

		internal override void OnAddUniqueElementFailed(int elm)
		{
			TryDeleteCachedEdge_(elm);
		}
	}
}
