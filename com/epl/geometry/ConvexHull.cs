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
	internal class ConvexHull
	{
		internal ConvexHull()
		{
			/*
			* Constructor for a Convex_hull object.Used for dynamic insertion of
			* geometries to create a convex hull.
			*/
			m_tree_hull = new com.epl.geometry.Treap();
			m_tree_hull.SetCapacity(20);
			m_shape = new com.epl.geometry.EditShape();
			m_geometry_handle = m_shape.CreateGeometry(com.epl.geometry.Geometry.Type.MultiPoint);
			m_path_handle = m_shape.InsertPath(m_geometry_handle, -1);
			m_call_back = new com.epl.geometry.ConvexHull.CallBackShape(this);
		}

		private ConvexHull(com.epl.geometry.MultiVertexGeometry mvg)
		{
			m_tree_hull = new com.epl.geometry.Treap();
			m_tree_hull.SetCapacity(20);
			m_mvg = mvg;
			m_call_back = new com.epl.geometry.ConvexHull.CallBackMvg(this);
		}

		private ConvexHull(com.epl.geometry.Point2D[] points)
		{
			m_tree_hull = new com.epl.geometry.Treap();
			m_tree_hull.SetCapacity(20);
			m_points = points;
			m_call_back = new com.epl.geometry.ConvexHull.CallBackPoints(this);
		}

		/// <summary>
		/// Adds a geometry to the current bounding geometry using an incremental
		/// algorithm for dynamic insertion.
		/// </summary>
		/// <remarks>
		/// Adds a geometry to the current bounding geometry using an incremental
		/// algorithm for dynamic insertion. \param geometry The geometry to add to
		/// the bounding geometry.
		/// </remarks>
		internal virtual void AddGeometry(com.epl.geometry.Geometry geometry)
		{
			int type = geometry.GetType().Value();
			if (com.epl.geometry.MultiVertexGeometry.IsMultiVertex(type))
			{
				AddMultiVertexGeometry_((com.epl.geometry.MultiVertexGeometry)geometry);
			}
			else
			{
				if (com.epl.geometry.MultiPath.IsSegment(type))
				{
					AddSegment_((com.epl.geometry.Segment)geometry);
				}
				else
				{
					if (type == com.epl.geometry.Geometry.GeometryType.Envelope)
					{
						AddEnvelope_((com.epl.geometry.Envelope)geometry);
					}
					else
					{
						if (type == com.epl.geometry.Geometry.GeometryType.Point)
						{
							AddPoint_((com.epl.geometry.Point)geometry);
						}
						else
						{
							throw new System.ArgumentException("invalid shape type");
						}
					}
				}
			}
		}

		/// <summary>Gets the current bounding geometry.</summary>
		/// <remarks>Gets the current bounding geometry. Returns a Geometry.</remarks>
		internal virtual com.epl.geometry.Geometry GetBoundingGeometry()
		{
			// Extracts the convex hull from the tree. Reading the tree in order
			// from first to last is the resulting convex hull.
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			int first = m_tree_hull.GetFirst(-1);
			com.epl.geometry.Polygon hull = new com.epl.geometry.Polygon(m_shape.GetVertexDescription());
			m_shape.QueryPoint(m_tree_hull.GetElement(first), point);
			hull.StartPath(point);
			for (int i = m_tree_hull.GetNext(first); i != -1; i = m_tree_hull.GetNext(i))
			{
				m_shape.QueryPoint(m_tree_hull.GetElement(i), point);
				hull.LineTo(point);
			}
			return hull;
		}

		/// <summary>Static method to construct the convex hull of a Multi_vertex_geometry.</summary>
		/// <remarks>
		/// Static method to construct the convex hull of a Multi_vertex_geometry.
		/// Returns a Polygon. \param mvg The geometry used to create the convex
		/// hull.
		/// </remarks>
		internal static com.epl.geometry.Polygon Construct(com.epl.geometry.MultiVertexGeometry mvg)
		{
			com.epl.geometry.ConvexHull convex_hull = new com.epl.geometry.ConvexHull(mvg);
			int N = mvg.GetPointCount();
			int t0 = 0;
			int tm = 1;
			com.epl.geometry.Point2D pt_0 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_m = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_p = new com.epl.geometry.Point2D();
			mvg.GetXY(t0, pt_0);
			while (true)
			{
				mvg.GetXY(tm, pt_m);
				if (!(pt_m.IsEqual(pt_0, com.epl.geometry.NumberUtils.DoubleEps()) && tm < N - 1))
				{
					break;
				}
				tm++;
			}
			// We don't want to close the gap between t0 and tm.
			convex_hull.m_tree_hull.AddElement(t0, -1);
			convex_hull.m_tree_hull.AddBiggestElement(tm, -1);
			for (int tp = tm + 1; tp < mvg.GetPointCount(); tp++)
			{
				// Dynamically
				// insert into
				// the current
				// convex hull
				mvg.GetXY(tp, pt_p);
				int p = convex_hull.TreeHull_(pt_p);
				if (p != -1)
				{
					convex_hull.m_tree_hull.SetElement(p, tp);
				}
			}
			// reset the place
			// holder to the
			// point index.
			// Extracts the convex hull from the tree. Reading the tree in order
			// from first to last is the resulting convex hull.
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			int first = convex_hull.m_tree_hull.GetFirst(-1);
			com.epl.geometry.Polygon hull = new com.epl.geometry.Polygon(mvg.GetDescription());
			mvg.GetPointByVal(convex_hull.m_tree_hull.GetElement(first), point);
			hull.StartPath(point);
			for (int i = convex_hull.m_tree_hull.GetNext(first); i != -1; i = convex_hull.m_tree_hull.GetNext(i))
			{
				mvg.GetPointByVal(convex_hull.m_tree_hull.GetElement(i), point);
				hull.LineTo(point);
			}
			return hull;
		}

		/// <summary>Static method to construct the convex hull from an array of points.</summary>
		/// <remarks>
		/// Static method to construct the convex hull from an array of points. The
		/// out_convex_hull array will be populated with the subset of index
		/// positions which contribute to the convex hull. Returns the number of
		/// points in the convex hull. \param points The points used to create the
		/// convex hull. \param count The number of points in the input Point2D
		/// array. \param out_convex_hull An index array allocated by the user at
		/// least as big as the size of the input points array.
		/// </remarks>
		internal static int Construct(com.epl.geometry.Point2D[] points, int count, int[] bounding_geometry)
		{
			com.epl.geometry.ConvexHull convex_hull = new com.epl.geometry.ConvexHull(points);
			int t0 = 0;
			int tm = 1;
			com.epl.geometry.Point2D pt_0 = points[t0];
			while (points[tm].IsEqual(pt_0, com.epl.geometry.NumberUtils.DoubleEps()) && tm < count - 1)
			{
				tm++;
			}
			// We don't want to close the gap between t0 and tm.
			convex_hull.m_tree_hull.AddElement(t0, -1);
			convex_hull.m_tree_hull.AddBiggestElement(tm, -1);
			for (int tp = tm + 1; tp < count; tp++)
			{
				// Dynamically insert into the
				// current convex hull.
				com.epl.geometry.Point2D pt_p = points[tp];
				int p = convex_hull.TreeHull_(pt_p);
				if (p != -1)
				{
					convex_hull.m_tree_hull.SetElement(p, tp);
				}
			}
			// reset the place
			// holder to the
			// point index.
			// Extracts the convex hull from the tree. Reading the tree in order
			// from first to last is the resulting convex hull.
			int out_count = 0;
			for (int i = convex_hull.m_tree_hull.GetFirst(-1); i != -1; i = convex_hull.m_tree_hull.GetNext(i))
			{
				bounding_geometry[out_count++] = convex_hull.m_tree_hull.GetElement(i);
			}
			return out_count;
		}

		/// <summary>Returns true if the given path of the input MultiPath is convex.</summary>
		/// <remarks>
		/// Returns true if the given path of the input MultiPath is convex. Returns
		/// false otherwise. \param multi_path The MultiPath to check if the path is
		/// convex. \param path_index The path of the MultiPath to check if its
		/// convex.
		/// </remarks>
		internal static bool IsPathConvex(com.epl.geometry.MultiPath multi_path, int path_index, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.MultiPathImpl mimpl = (com.epl.geometry.MultiPathImpl)multi_path._getImpl();
			int path_start = mimpl.GetPathStart(path_index);
			int path_end = mimpl.GetPathEnd(path_index);
			bool bxyclosed = !mimpl.IsClosedPath(path_index) && mimpl.IsClosedPathInXYPlane(path_index);
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(mimpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			int position_start = 2 * path_start;
			int position_end = 2 * path_end;
			if (bxyclosed)
			{
				position_end -= 2;
			}
			if (position_end - position_start < 6)
			{
				return true;
			}
			// This matches the logic for case 1 of the tree hull algorithm. The
			// idea is inductive. We assume we have a convex hull pt_0,...,pt_m, and
			// we see if
			// a new point (pt_pivot) is among the transitive tournament for pt_0,
			// knowing that pt_pivot comes after pt_m.
			// We check three conditions:
			// 1) pt_m->pt_pivot->pt_0 is clockwise (closure across the boundary is
			// convex)
			// 2) pt_1->pt_pivot->pt_0 is clockwise (the first step forward is
			// convex) (pt_1 is the next point after pt_0)
			// 3) pt_m->pt_pivot->pt_m_prev is clockwise (the first step backwards
			// is convex) (pt_m_prev is the previous point before pt_m)
			// If all three of the above conditions are clockwise, then pt_pivot is
			// among the transitive tournament for pt_0, and therefore the polygon
			// pt_0, ..., pt_m, pt_pivot is convex.
			com.epl.geometry.Point2D pt_0 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_m = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_pivot = new com.epl.geometry.Point2D();
			position.Read(position_start, pt_0);
			position.Read(position_start + 2, pt_m);
			position.Read(position_start + 4, pt_pivot);
			// Initial inductive step
			com.epl.geometry.ECoordinate det_ec = Determinant_(pt_m, pt_pivot, pt_0);
			if (det_ec.IsFuzzyZero() || !IsClockwise_(det_ec.Value()))
			{
				return false;
			}
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D(pt_m.x, pt_m.y);
			com.epl.geometry.Point2D pt_m_prev = new com.epl.geometry.Point2D();
			// Assume that pt_0,...,pt_m is convex. Check if the next point,
			// pt_pivot, maintains the convex invariant.
			for (int i = position_start + 6; i < position_end; i += 2)
			{
				pt_m_prev.SetCoords(pt_m);
				pt_m.SetCoords(pt_pivot);
				position.Read(i, pt_pivot);
				det_ec = Determinant_(pt_m, pt_pivot, pt_0);
				if (det_ec.IsFuzzyZero() || !IsClockwise_(det_ec.Value()))
				{
					return false;
				}
				det_ec = Determinant_(pt_1, pt_pivot, pt_0);
				if (det_ec.IsFuzzyZero() || !IsClockwise_(det_ec.Value()))
				{
					return false;
				}
				det_ec = Determinant_(pt_m, pt_pivot, pt_m_prev);
				if (det_ec.IsFuzzyZero() || !IsClockwise_(det_ec.Value()))
				{
					return false;
				}
			}
			return true;
		}

		// Dynamically inserts each geometry into the convex hull.
		private void AddMultiVertexGeometry_(com.epl.geometry.MultiVertexGeometry mvg)
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt_p = new com.epl.geometry.Point2D();
			for (int i = 0; i < mvg.GetPointCount(); i++)
			{
				mvg.GetXY(i, pt_p);
				int p = AddPoint_(pt_p);
				if (p != -1)
				{
					mvg.GetPointByVal(i, point);
					int tp = m_shape.AddPoint(m_path_handle, point);
					m_tree_hull.SetElement(p, tp);
				}
			}
		}

		// reset the place holder to tp
		private void AddEnvelope_(com.epl.geometry.Envelope envelope)
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt_p = new com.epl.geometry.Point2D();
			for (int i = 0; i < 4; i++)
			{
				envelope.QueryCorner(i, pt_p);
				int p = AddPoint_(pt_p);
				if (p != -1)
				{
					envelope.QueryCornerByVal(i, point);
					int tp = m_shape.AddPoint(m_path_handle, point);
					m_tree_hull.SetElement(p, tp);
				}
			}
		}

		// reset the place holder to tp
		private void AddSegment_(com.epl.geometry.Segment segment)
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt_start = segment.GetStartXY();
			int p_start = AddPoint_(pt_start);
			if (p_start != -1)
			{
				segment.QueryStart(point);
				int t_start = m_shape.AddPoint(m_path_handle, point);
				m_tree_hull.SetElement(p_start, t_start);
			}
			// reset the place holder
			// to tp
			com.epl.geometry.Point2D pt_end = segment.GetEndXY();
			int p_end = AddPoint_(pt_end);
			if (p_end != -1)
			{
				segment.QueryEnd(point);
				int t_end = m_shape.AddPoint(m_path_handle, point);
				m_tree_hull.SetElement(p_end, t_end);
			}
		}

		// reset the place holder to
		// tp
		private void AddPoint_(com.epl.geometry.Point point)
		{
			com.epl.geometry.Point2D pt_p = point.GetXY();
			int p = AddPoint_(pt_p);
			if (p != -1)
			{
				int tp = m_shape.AddPoint(m_path_handle, point);
				m_tree_hull.SetElement(p, tp);
			}
		}

		// reset the place holder to tp
		private int AddPoint_(com.epl.geometry.Point2D pt_p)
		{
			int p = -1;
			if (m_tree_hull.Size(-1) == 0)
			{
				p = m_tree_hull.AddElement(-4, -1);
				// set place holder to -4 to
				// indicate the first element
				// being added (t0).
				return p;
			}
			if (m_tree_hull.Size(-1) == 1)
			{
				int t0 = m_tree_hull.GetElement(m_tree_hull.GetFirst(-1));
				com.epl.geometry.Point2D pt_0 = m_shape.GetXY(t0);
				if (!pt_p.IsEqual(pt_0, com.epl.geometry.NumberUtils.DoubleEps()))
				{
					// We don't want
					// to close the
					// gap between
					// t0 and tm.
					p = m_tree_hull.AddBiggestElement(-5, -1);
				}
				// set place holder
				// to -5 to indicate
				// the second
				// element being
				// added (tm).
				return p;
			}
			p = TreeHull_(pt_p);
			return p;
		}

		// Algorithm taken from "Axioms and Hulls" by D.E. Knuth, Lecture Notes in
		// Computer Science 606, page 47.
		private int TreeHull_(com.epl.geometry.Point2D pt_pivot)
		{
			System.Diagnostics.Debug.Assert((m_tree_hull.Size(-1) >= 2));
			int p = -1;
			do
			{
				int first = m_tree_hull.GetFirst(-1);
				int last = m_tree_hull.GetLast(-1);
				int t0 = m_tree_hull.GetElement(first);
				int tm = m_tree_hull.GetElement(last);
				com.epl.geometry.Point2D pt_0 = new com.epl.geometry.Point2D();
				// should the memory be cached?
				com.epl.geometry.Point2D pt_m = new com.epl.geometry.Point2D();
				// should the memory be cached?
				m_call_back.GetXY(t0, pt_0);
				m_call_back.GetXY(tm, pt_m);
				System.Diagnostics.Debug.Assert((!pt_0.IsEqual(pt_m, com.epl.geometry.NumberUtils.DoubleEps())));
				// assert
				// that the
				// gap is
				// not
				// closed
				int orient_m_p_0 = com.epl.geometry.Point2D.OrientationRobust(pt_m, pt_pivot, pt_0);
				// determines
				// case
				// 1,
				// 2,
				// 3
				if (IsClockwise_(orient_m_p_0))
				{
					// Case 1: tp->t0->tm is clockwise
					p = m_tree_hull.AddBiggestElement(-1, -1);
					// set place holder
					// to -1 for case 1.
					int l = TreeHullWalkBackward_(pt_pivot, last, first);
					if (l != first)
					{
						TreeHullWalkForward_(pt_pivot, first, m_tree_hull.GetPrev(l));
					}
					continue;
				}
				if (IsCounterClockwise_(orient_m_p_0))
				{
					// Case 2: tp->tm->t0 is
					// clockwise
					int k = m_tree_hull.GetRoot(-1);
					int k_min = m_tree_hull.GetFirst(-1);
					int k_max = m_tree_hull.GetLast(-1);
					int k_prev;
					int tk;
					int tk_prev;
					com.epl.geometry.Point2D pt_k = new com.epl.geometry.Point2D();
					com.epl.geometry.Point2D pt_k_prev = new com.epl.geometry.Point2D();
					while (k_min != m_tree_hull.GetPrev(k_max))
					{
						// binary search to
						// find k such
						// that
						// t0->tp->tj
						// holds (i.e.
						// clockwise)
						// for j >= k.
						// Hence,
						// tj->tp->t0 is
						// clockwise (or
						// degenerate)
						// for j < k.
						tk = m_tree_hull.GetElement(k);
						m_call_back.GetXY(tk, pt_k);
						int orient_k_p_0 = com.epl.geometry.Point2D.OrientationRobust(pt_k, pt_pivot, pt_0);
						if (IsCounterClockwise_(orient_k_p_0))
						{
							k_max = k;
							k = m_tree_hull.GetLeft(k);
						}
						else
						{
							k_min = k;
							k = m_tree_hull.GetRight(k);
						}
					}
					k = k_max;
					k_prev = k_min;
					tk = m_tree_hull.GetElement(k);
					tk_prev = m_tree_hull.GetElement(k_prev);
					m_call_back.GetXY(tk, pt_k);
					m_call_back.GetXY(tk_prev, pt_k_prev);
					System.Diagnostics.Debug.Assert((IsCounterClockwise_(com.epl.geometry.Point2D.OrientationRobust(pt_k, pt_pivot, pt_0)) && !IsCounterClockwise_(com.epl.geometry.Point2D.OrientationRobust(pt_k_prev, pt_pivot, pt_0))));
					System.Diagnostics.Debug.Assert((k_prev != first || IsCounterClockwise_(com.epl.geometry.Point2D.OrientationRobust(pt_k, pt_pivot, pt_0))));
					if (k_prev != first)
					{
						int orient_k_prev_p_k = com.epl.geometry.Point2D.OrientationRobust(pt_k_prev, pt_pivot, pt_k);
						if (!IsClockwise_(orient_k_prev_p_k))
						{
							continue;
						}
					}
					// pt_pivot is inside the hull (or on the
					// boundary)
					p = m_tree_hull.AddElementAtPosition(k_prev, k, -2, true, false, -1);
					// set place holder to -2 for case 2.
					TreeHullWalkForward_(pt_pivot, k, last);
					TreeHullWalkBackward_(pt_pivot, k_prev, first);
					continue;
				}
				System.Diagnostics.Debug.Assert((IsDegenerate_(orient_m_p_0)));
				{
					// Case 3: degenerate
					if (m_line == null)
					{
						m_line = new com.epl.geometry.Line();
					}
					m_line.SetStartXY(pt_m);
					m_line.SetEndXY(pt_0);
					double scalar = m_line.GetClosestCoordinate(pt_pivot, true);
					// if
					// scalar
					// is
					// between
					// 0
					// and
					// 1,
					// then
					// we
					// don't
					// need
					// to
					// add
					// tp.
					if (scalar < 0.0)
					{
						int l = m_tree_hull.GetPrev(last);
						m_tree_hull.DeleteNode(last, -1);
						p = m_tree_hull.AddBiggestElement(-3, -1);
						// set place
						// holder to -3
						// for case 3.
						TreeHullWalkBackward_(pt_pivot, l, first);
					}
					else
					{
						if (scalar > 1.0)
						{
							int j = m_tree_hull.GetNext(first);
							m_tree_hull.DeleteNode(first, -1);
							p = m_tree_hull.AddElementAtPosition(-1, j, -3, true, false, -1);
							// set place holder to -3 for case 3.
							TreeHullWalkForward_(pt_pivot, j, last);
						}
					}
					continue;
				}
			}
			while (false);
			return p;
		}

		private int TreeHullWalkForward_(com.epl.geometry.Point2D pt_pivot, int start, int end)
		{
			if (start == end)
			{
				return end;
			}
			int j = start;
			int tj = m_tree_hull.GetElement(j);
			int j_next = m_tree_hull.GetNext(j);
			com.epl.geometry.Point2D pt_j = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_j_next = new com.epl.geometry.Point2D();
			m_call_back.GetXY(tj, pt_j);
			while (j != end && m_tree_hull.Size(-1) > 2)
			{
				// Stops when we find a
				// clockwise triple
				// containting the pivot
				// point, or when the
				// tree_hull size is 2.
				// Deletes non-clockwise
				// triples along the
				// way.
				int tj_next = m_tree_hull.GetElement(j_next);
				m_call_back.GetXY(tj_next, pt_j_next);
				int orient_j_next_p_j = com.epl.geometry.Point2D.OrientationRobust(pt_j_next, pt_pivot, pt_j);
				if (IsClockwise_(orient_j_next_p_j))
				{
					break;
				}
				int ccw = j;
				j = j_next;
				pt_j.SetCoords(pt_j_next);
				j_next = m_tree_hull.GetNext(j);
				m_call_back.DeleteNode(ccw);
			}
			return j;
		}

		private int TreeHullWalkBackward_(com.epl.geometry.Point2D pt_pivot, int start, int end)
		{
			if (start == end)
			{
				return end;
			}
			int l = start;
			int tl = m_tree_hull.GetElement(l);
			int l_prev = m_tree_hull.GetPrev(l);
			com.epl.geometry.Point2D pt_l = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_l_prev = new com.epl.geometry.Point2D();
			m_call_back.GetXY(tl, pt_l);
			while (l != end && m_tree_hull.Size(-1) > 2)
			{
				// Stops when we find a
				// clockwise triple
				// containting the pivot
				// point, or when the
				// tree_hull size is 2.
				// Deletes non-clockwise
				// triples along the
				// way.
				int tl_prev = m_tree_hull.GetElement(l_prev);
				m_call_back.GetXY(tl_prev, pt_l_prev);
				int orient_l_p_l_prev = com.epl.geometry.Point2D.OrientationRobust(pt_l, pt_pivot, pt_l_prev);
				if (IsClockwise_(orient_l_p_l_prev))
				{
					break;
				}
				int ccw = l;
				l = l_prev;
				pt_l.SetCoords(pt_l_prev);
				l_prev = m_tree_hull.GetPrev(l);
				m_call_back.DeleteNode(ccw);
			}
			return l;
		}

		// Orientation predicates
		private static com.epl.geometry.ECoordinate Determinant_(com.epl.geometry.Point2D p, com.epl.geometry.Point2D q, com.epl.geometry.Point2D r)
		{
			com.epl.geometry.ECoordinate det_ec = new com.epl.geometry.ECoordinate();
			det_ec.Set(q.x);
			det_ec.Sub(p.x);
			com.epl.geometry.ECoordinate rp_y_ec = new com.epl.geometry.ECoordinate();
			rp_y_ec.Set(r.y);
			rp_y_ec.Sub(p.y);
			com.epl.geometry.ECoordinate qp_y_ec = new com.epl.geometry.ECoordinate();
			qp_y_ec.Set(q.y);
			qp_y_ec.Sub(p.y);
			com.epl.geometry.ECoordinate rp_x_ec = new com.epl.geometry.ECoordinate();
			rp_x_ec.Set(r.x);
			rp_x_ec.Sub(p.x);
			det_ec.Mul(rp_y_ec);
			qp_y_ec.Mul(rp_x_ec);
			det_ec.Sub(qp_y_ec);
			return det_ec;
		}

		private static bool IsClockwise_(double det)
		{
			return det < 0.0;
		}

		private static bool IsCounterClockwise_(double det)
		{
			return det > 0.0;
		}

		private static bool IsDegenerate_(double det)
		{
			return det == 0.0;
		}

		private static bool IsClockwise_(int orientation)
		{
			return orientation < 0.0;
		}

		private static bool IsCounterClockwise_(int orientation)
		{
			return orientation > 0.0;
		}

		private static bool IsDegenerate_(int orientation)
		{
			return orientation == 0.0;
		}

		private abstract class CallBack
		{
			internal abstract void GetXY(int ti, com.epl.geometry.Point2D pt);

			internal abstract void DeleteNode(int i);
		}

		private sealed class CallBackShape : com.epl.geometry.ConvexHull.CallBack
		{
			private com.epl.geometry.ConvexHull m_convex_hull;

			internal CallBackShape(com.epl.geometry.ConvexHull convex_hull)
			{
				m_convex_hull = convex_hull;
			}

			internal override void GetXY(int ti, com.epl.geometry.Point2D pt)
			{
				m_convex_hull.m_shape.GetXY(ti, pt);
			}

			internal override void DeleteNode(int i)
			{
				int ti = m_convex_hull.m_tree_hull.GetElement(i);
				m_convex_hull.m_tree_hull.DeleteNode(i, -1);
				m_convex_hull.m_shape.RemoveVertex(ti, false);
			}
		}

		private sealed class CallBackMvg : com.epl.geometry.ConvexHull.CallBack
		{
			private com.epl.geometry.ConvexHull m_convex_hull;

			internal CallBackMvg(com.epl.geometry.ConvexHull convex_hull)
			{
				m_convex_hull = convex_hull;
			}

			internal override void GetXY(int ti, com.epl.geometry.Point2D pt)
			{
				m_convex_hull.m_mvg.GetXY(ti, pt);
			}

			internal override void DeleteNode(int i)
			{
				m_convex_hull.m_tree_hull.DeleteNode(i, -1);
			}
		}

		private sealed class CallBackPoints : com.epl.geometry.ConvexHull.CallBack
		{
			private com.epl.geometry.ConvexHull m_convex_hull;

			internal CallBackPoints(com.epl.geometry.ConvexHull convex_hull)
			{
				m_convex_hull = convex_hull;
			}

			internal override void GetXY(int ti, com.epl.geometry.Point2D pt)
			{
				pt.SetCoords(m_convex_hull.m_points[ti]);
			}

			internal override void DeleteNode(int i)
			{
				m_convex_hull.m_tree_hull.DeleteNode(i, -1);
			}
		}

		private com.epl.geometry.Treap m_tree_hull;

		private com.epl.geometry.EditShape m_shape;

		private com.epl.geometry.MultiVertexGeometry m_mvg;

		private com.epl.geometry.Point2D[] m_points;

		private int m_geometry_handle;

		private int m_path_handle;

		private com.epl.geometry.Line m_line;

		private com.epl.geometry.ConvexHull.CallBack m_call_back;
		// Members
	}
}
