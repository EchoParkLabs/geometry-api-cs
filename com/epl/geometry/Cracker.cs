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


namespace com.epl.geometry
{
	/// <summary>Implementation for the segment cracking.</summary>
	/// <remarks>
	/// Implementation for the segment cracking.
	/// Finds and splits all intersecting segments. Used by the TopoGraph and
	/// Simplify.
	/// </remarks>
	internal sealed class Cracker
	{
		private com.epl.geometry.EditShape m_shape;

		private com.epl.geometry.ProgressTracker m_progress_tracker;

		private com.epl.geometry.NonSimpleResult m_non_simple_result;

		private double m_tolerance;

		private com.epl.geometry.Treap m_sweep_structure;

		private com.epl.geometry.SweepComparator m_sweep_comparator;

		private bool m_bAllowCoincident;

		private com.epl.geometry.Segment GetSegment_(int vertex, com.epl.geometry.Line lineHelper)
		{
			com.epl.geometry.Segment seg = m_shape.GetSegment(vertex);
			if (seg == null)
			{
				if (!m_shape.QueryLineConnector(vertex, lineHelper))
				{
					return null;
				}
				seg = (com.epl.geometry.Segment)lineHelper;
			}
			return seg;
		}

		private bool CrackBruteForce_()
		{
			com.epl.geometry.EditShape.VertexIterator iter_1 = m_shape.QueryVertexIterator(false);
			bool b_cracked = false;
			com.epl.geometry.Line line_1 = new com.epl.geometry.Line();
			com.epl.geometry.Line line_2 = new com.epl.geometry.Line();
			com.epl.geometry.Envelope2D seg_1_env = new com.epl.geometry.Envelope2D();
			seg_1_env.SetEmpty();
			com.epl.geometry.Envelope2D seg_2_env = new com.epl.geometry.Envelope2D();
			seg_2_env.SetEmpty();
			bool assume_intersecting = false;
			com.epl.geometry.Point helper_point = new com.epl.geometry.Point();
			com.epl.geometry.SegmentIntersector segment_intersector = new com.epl.geometry.SegmentIntersector();
			for (int vertex_1 = iter_1.Next(); vertex_1 != -1; vertex_1 = iter_1.Next())
			{
				com.epl.geometry.ProgressTracker.CheckAndThrow(m_progress_tracker);
				int GT_1 = m_shape.GetGeometryType(iter_1.CurrentGeometry());
				com.epl.geometry.Segment seg_1 = null;
				bool seg_1_zero = false;
				if (!com.epl.geometry.Geometry.IsPoint(GT_1))
				{
					seg_1 = GetSegment_(vertex_1, line_1);
					if (seg_1 == null)
					{
						continue;
					}
					seg_1.QueryEnvelope2D(seg_1_env);
					seg_1_env.Inflate(m_tolerance, m_tolerance);
					if (seg_1.IsDegenerate(m_tolerance))
					{
						// do not crack with
						// degenerate segments
						if (seg_1.IsDegenerate(0))
						{
							seg_1_zero = true;
							seg_1 = null;
						}
						else
						{
							continue;
						}
					}
				}
				com.epl.geometry.EditShape.VertexIterator iter_2 = m_shape.QueryVertexIterator(iter_1);
				int vertex_2 = iter_2.Next();
				if (vertex_2 != -1)
				{
					vertex_2 = iter_2.Next();
				}
				for (; vertex_2 != -1; vertex_2 = iter_2.Next())
				{
					int GT_2 = m_shape.GetGeometryType(iter_2.CurrentGeometry());
					com.epl.geometry.Segment seg_2 = null;
					bool seg_2_zero = false;
					if (!com.epl.geometry.Geometry.IsPoint(GT_2))
					{
						seg_2 = GetSegment_(vertex_2, line_2);
						if (seg_2 == null)
						{
							continue;
						}
						seg_2.QueryEnvelope2D(seg_2_env);
						if (seg_2.IsDegenerate(m_tolerance))
						{
							// do not crack with
							// degenerate segments
							if (seg_2.IsDegenerate(0))
							{
								seg_2_zero = true;
								seg_2 = null;
							}
							else
							{
								continue;
							}
						}
					}
					int split_count_1 = 0;
					int split_count_2 = 0;
					if (seg_1 != null && seg_2 != null)
					{
						if (seg_1_env.IsIntersectingNE(seg_2_env))
						{
							segment_intersector.PushSegment(seg_1);
							segment_intersector.PushSegment(seg_2);
							segment_intersector.Intersect(m_tolerance, assume_intersecting);
							split_count_1 = segment_intersector.GetResultSegmentCount(0);
							split_count_2 = segment_intersector.GetResultSegmentCount(1);
							if (split_count_1 + split_count_2 > 0)
							{
								m_shape.SplitSegment_(vertex_1, segment_intersector, 0, true);
								m_shape.SplitSegment_(vertex_2, segment_intersector, 1, true);
							}
							segment_intersector.Clear();
						}
					}
					else
					{
						if (seg_1 != null)
						{
							com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
							m_shape.GetXY(vertex_2, pt);
							if (seg_1_env.Contains(pt))
							{
								segment_intersector.PushSegment(seg_1);
								m_shape.QueryPoint(vertex_2, helper_point);
								segment_intersector.Intersect(m_tolerance, helper_point, 0, 1.0, assume_intersecting);
								split_count_1 = segment_intersector.GetResultSegmentCount(0);
								if (split_count_1 > 0)
								{
									m_shape.SplitSegment_(vertex_1, segment_intersector, 0, true);
									if (seg_2_zero)
									{
										//seg_2 was zero length. Need to change all coincident points
										//segment at vertex_2 is dzero length, change all attached zero length segments
										int v_to = -1;
										for (int v = m_shape.GetNextVertex(vertex_2); v != -1 && v != vertex_2; v = m_shape.GetNextVertex(v))
										{
											seg_2 = GetSegment_(v, line_2);
											v_to = v;
											if (seg_2 == null || !seg_2.IsDegenerate(0))
											{
												break;
											}
										}
										//change from vertex_2 to v_to (inclusive).
										for (int v_1 = vertex_2; v_1 != -1; v_1 = m_shape.GetNextVertex(v_1))
										{
											m_shape.SetPoint(v_1, segment_intersector.GetResultPoint());
											if (v_1 == v_to)
											{
												break;
											}
										}
									}
									else
									{
										m_shape.SetPoint(vertex_2, segment_intersector.GetResultPoint());
									}
								}
								segment_intersector.Clear();
							}
						}
						else
						{
							if (seg_2 != null)
							{
								com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
								m_shape.GetXY(vertex_1, pt);
								seg_2_env.Inflate(m_tolerance, m_tolerance);
								if (seg_2_env.Contains(pt))
								{
									segment_intersector.PushSegment(seg_2);
									m_shape.QueryPoint(vertex_1, helper_point);
									segment_intersector.Intersect(m_tolerance, helper_point, 0, 1.0, assume_intersecting);
									split_count_2 = segment_intersector.GetResultSegmentCount(0);
									if (split_count_2 > 0)
									{
										m_shape.SplitSegment_(vertex_2, segment_intersector, 0, true);
										if (seg_1_zero)
										{
											//seg_1 was zero length. Need to change all coincident points
											//segment at vertex_2 is dzero length, change all attached zero length segments
											int v_to = -1;
											for (int v = m_shape.GetNextVertex(vertex_1); v != -1 && v != vertex_1; v = m_shape.GetNextVertex(v))
											{
												seg_2 = GetSegment_(v, line_2);
												//using here seg_2 for seg_1
												v_to = v;
												if (seg_2 == null || !seg_2.IsDegenerate(0))
												{
													break;
												}
											}
											//change from vertex_2 to v_to (inclusive).
											for (int v_1 = vertex_1; v_1 != -1; v_1 = m_shape.GetNextVertex(v_1))
											{
												m_shape.SetPoint(v_1, segment_intersector.GetResultPoint());
												if (v_1 == v_to)
												{
													break;
												}
											}
										}
										else
										{
											m_shape.SetPoint(vertex_1, segment_intersector.GetResultPoint());
										}
									}
									segment_intersector.Clear();
								}
							}
							else
							{
								continue;
							}
						}
					}
					// points on points
					if (split_count_1 + split_count_2 != 0)
					{
						if (split_count_1 != 0)
						{
							seg_1 = m_shape.GetSegment(vertex_1);
							// reload segment
							// after split
							if (seg_1 == null)
							{
								if (!m_shape.QueryLineConnector(vertex_1, line_1))
								{
									continue;
								}
								seg_1 = line_1;
								line_1.QueryEnvelope2D(seg_1_env);
							}
							else
							{
								seg_1.QueryEnvelope2D(seg_1_env);
							}
							if (seg_1.IsDegenerate(m_tolerance))
							{
								// do not crack with
								// degenerate
								// segments
								break;
							}
						}
						b_cracked = true;
					}
				}
			}
			return b_cracked;
		}

		internal bool CrackerPlaneSweep_()
		{
			bool b_cracked = PlaneSweep_();
			return b_cracked;
		}

		internal bool PlaneSweep_()
		{
			com.epl.geometry.PlaneSweepCrackerHelper plane_sweep = new com.epl.geometry.PlaneSweepCrackerHelper();
			bool b_cracked = plane_sweep.Sweep(m_shape, m_tolerance);
			return b_cracked;
		}

		internal bool NeedsCrackingImpl_()
		{
			bool b_needs_cracking = false;
			if (m_sweep_structure == null)
			{
				m_sweep_structure = new com.epl.geometry.Treap();
			}
			com.epl.geometry.AttributeStreamOfInt32 event_q = new com.epl.geometry.AttributeStreamOfInt32(0);
			event_q.Reserve(m_shape.GetTotalPointCount() + 1);
			com.epl.geometry.EditShape.VertexIterator iter = m_shape.QueryVertexIterator();
			for (int vert = iter.Next(); vert != -1; vert = iter.Next())
			{
				event_q.Add(vert);
			}
			System.Diagnostics.Debug.Assert((m_shape.GetTotalPointCount() == event_q.Size()));
			m_shape.SortVerticesSimpleByY_(event_q, 0, event_q.Size());
			event_q.Add(-1);
			// for termination;
			// create user indices to store edges that end at vertices.
			int edge_index_1 = m_shape.CreateUserIndex();
			int edge_index_2 = m_shape.CreateUserIndex();
			m_sweep_comparator = new com.epl.geometry.SweepComparator(m_shape, m_tolerance, !m_bAllowCoincident);
			m_sweep_structure.SetComparator(m_sweep_comparator);
			com.epl.geometry.AttributeStreamOfInt32 swept_edges_to_delete = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 edges_to_insert = new com.epl.geometry.AttributeStreamOfInt32(0);
			// Go throught the sorted vertices
			int event_q_index = 0;
			com.epl.geometry.Point2D cluster_pt = new com.epl.geometry.Point2D();
			// sweep-line algorithm:
			for (int vertex = event_q.Get(event_q_index++); vertex != -1; )
			{
				m_shape.GetXY(vertex, cluster_pt);
				do
				{
					int next_vertex = m_shape.GetNextVertex(vertex);
					int prev_vertex = m_shape.GetPrevVertex(vertex);
					if (next_vertex != -1 && m_shape.CompareVerticesSimpleY_(vertex, next_vertex) < 0)
					{
						edges_to_insert.Add(vertex);
						edges_to_insert.Add(next_vertex);
					}
					if (prev_vertex != -1 && m_shape.CompareVerticesSimpleY_(vertex, prev_vertex) < 0)
					{
						edges_to_insert.Add(prev_vertex);
						edges_to_insert.Add(prev_vertex);
					}
					// Continue accumulating current cluster
					int attached_edge_1 = m_shape.GetUserIndex(vertex, edge_index_1);
					if (attached_edge_1 != -1)
					{
						swept_edges_to_delete.Add(attached_edge_1);
						m_shape.SetUserIndex(vertex, edge_index_1, -1);
					}
					int attached_edge_2 = m_shape.GetUserIndex(vertex, edge_index_2);
					if (attached_edge_2 != -1)
					{
						swept_edges_to_delete.Add(attached_edge_2);
						m_shape.SetUserIndex(vertex, edge_index_2, -1);
					}
					vertex = event_q.Get(event_q_index++);
				}
				while (vertex != -1 && m_shape.IsEqualXY(vertex, cluster_pt));
				bool b_continuing_segment_chain_optimization = swept_edges_to_delete.Size() == 1 && edges_to_insert.Size() == 2;
				int new_left = -1;
				int new_right = -1;
				// Process the cluster
				for (int i = 0, n = swept_edges_to_delete.Size(); i < n; i++)
				{
					// Find left and right neighbour of the edges that terminate at
					// the cluster (there will be atmost only one left and one
					// right).
					int edge = swept_edges_to_delete.Get(i);
					int left = m_sweep_structure.GetPrev(edge);
					if (left != -1 && !swept_edges_to_delete.HasElement(left))
					{
						// Note:
						// for
						// some
						// heavy
						// cases,
						// it
						// could
						// be
						// better
						// to
						// use
						// binary
						// search.
						System.Diagnostics.Debug.Assert((new_left == -1));
						new_left = left;
					}
					int right = m_sweep_structure.GetNext(edge);
					if (right != -1 && !swept_edges_to_delete.HasElement(right))
					{
						System.Diagnostics.Debug.Assert((new_right == -1));
						new_right = right;
					}
					//#ifdef NDEBUG				
					if (new_left != -1 && new_right != -1)
					{
						break;
					}
				}
				//#endif
				System.Diagnostics.Debug.Assert((new_left == -1 || new_left != new_right));
				m_sweep_comparator.SetSweepY(cluster_pt.y, cluster_pt.x);
				// Delete the edges that terminate at the cluster.
				for (int i_1 = 0, n = swept_edges_to_delete.Size(); i_1 < n; i_1++)
				{
					int edge = swept_edges_to_delete.Get(i_1);
					m_sweep_structure.DeleteNode(edge, -1);
				}
				swept_edges_to_delete.Clear(false);
				if (!b_continuing_segment_chain_optimization && new_left != -1 && new_right != -1)
				{
					if (CheckForIntersections_(new_left, new_right))
					{
						b_needs_cracking = true;
						m_non_simple_result = m_sweep_comparator.GetResult();
						break;
					}
				}
				for (int i_2 = 0, n = edges_to_insert.Size(); i_2 < n; i_2 += 2)
				{
					int v = edges_to_insert.Get(i_2);
					int otherv = edges_to_insert.Get(i_2 + 1);
					int new_edge_1 = -1;
					if (b_continuing_segment_chain_optimization)
					{
						new_edge_1 = m_sweep_structure.AddElementAtPosition(new_left, new_right, v, true, true, -1);
						b_continuing_segment_chain_optimization = false;
					}
					else
					{
						new_edge_1 = m_sweep_structure.AddElement(v, -1);
					}
					// the
					// sweep
					// structure
					// consist
					// of
					// the
					// origin
					// vertices
					// for
					// edges.
					// One
					// can
					// always
					// get
					// the
					// other
					// endpoint
					// as
					// the
					// next
					// vertex.
					if (m_sweep_comparator.IntersectionDetected())
					{
						m_non_simple_result = m_sweep_comparator.GetResult();
						b_needs_cracking = true;
						break;
					}
					int e_1 = m_shape.GetUserIndex(otherv, edge_index_1);
					if (e_1 == -1)
					{
						m_shape.SetUserIndex(otherv, edge_index_1, new_edge_1);
					}
					else
					{
						System.Diagnostics.Debug.Assert((m_shape.GetUserIndex(otherv, edge_index_2) == -1));
						m_shape.SetUserIndex(otherv, edge_index_2, new_edge_1);
					}
				}
				if (b_needs_cracking)
				{
					break;
				}
				// Start accumulating new cluster
				edges_to_insert.ResizePreserveCapacity(0);
			}
			m_shape.RemoveUserIndex(edge_index_1);
			m_shape.RemoveUserIndex(edge_index_2);
			return b_needs_cracking;
		}

		internal bool CheckForIntersections_(int sweep_edge_1, int sweep_edge_2)
		{
			System.Diagnostics.Debug.Assert((sweep_edge_1 != sweep_edge_2));
			int left = m_sweep_structure.GetElement(sweep_edge_1);
			System.Diagnostics.Debug.Assert((left != m_sweep_structure.GetElement(sweep_edge_2)));
			m_sweep_comparator.Compare(m_sweep_structure, left, sweep_edge_2);
			// compare
			// detects
			// intersections
			bool b_intersects = m_sweep_comparator.IntersectionDetected();
			m_sweep_comparator.ClearIntersectionDetectedFlag();
			return b_intersects;
		}

		internal Cracker(com.epl.geometry.ProgressTracker progress_tracker)
		{
			// void dbg_print_sweep_edge_(int edge);
			// void dbg_print_sweep_structure_();
			// void dbg_check_sweep_structure_();
			m_progress_tracker = progress_tracker;
			m_bAllowCoincident = true;
		}

		internal static bool CanBeCracked(com.epl.geometry.EditShape shape)
		{
			for (int geometry = shape.GetFirstGeometry(); geometry != -1; geometry = shape.GetNextGeometry(geometry))
			{
				if (!com.epl.geometry.Geometry.IsMultiPath(shape.GetGeometryType(geometry)))
				{
					continue;
				}
				return true;
			}
			return false;
		}

		internal static bool Execute(com.epl.geometry.EditShape shape, com.epl.geometry.Envelope2D extent, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (!CanBeCracked(shape))
			{
				// make sure it contains some segments,
				// otherwise no need to crack.
				return false;
			}
			com.epl.geometry.Cracker cracker = new com.epl.geometry.Cracker(progress_tracker);
			cracker.m_shape = shape;
			cracker.m_tolerance = tolerance;
			// Use brute force for smaller shapes, and a planesweep for bigger
			// shapes.
			bool b_cracked = false;
			if (shape.GetTotalPointCount() < 15)
			{
				// what is a good number?
				b_cracked = cracker.CrackBruteForce_();
			}
			else
			{
				bool b_cracked_1 = cracker.CrackerPlaneSweep_();
				return b_cracked_1;
			}
			return b_cracked;
		}

		internal static bool Execute(com.epl.geometry.EditShape shape, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return com.epl.geometry.Cracker.Execute(shape, shape.GetEnvelope2D(), tolerance, progress_tracker);
		}

		// Used for IsSimple.
		internal static bool NeedsCracking(bool allowCoincident, com.epl.geometry.EditShape shape, double tolerance, com.epl.geometry.NonSimpleResult result, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (!CanBeCracked(shape))
			{
				return false;
			}
			com.epl.geometry.Cracker cracker = new com.epl.geometry.Cracker(progress_tracker);
			cracker.m_shape = shape;
			cracker.m_tolerance = tolerance;
			cracker.m_bAllowCoincident = allowCoincident;
			if (cracker.NeedsCrackingImpl_())
			{
				if (result != null)
				{
					result.Assign(cracker.m_non_simple_result);
				}
				return true;
			}
			// Now swap the coordinates to catch horizontal cases.
			com.epl.geometry.Transformation2D transform = new com.epl.geometry.Transformation2D();
			transform.SetSwapCoordinates();
			shape.ApplyTransformation(transform);
			cracker = new com.epl.geometry.Cracker(progress_tracker);
			cracker.m_shape = shape;
			cracker.m_tolerance = tolerance;
			cracker.m_bAllowCoincident = allowCoincident;
			bool b_res = cracker.NeedsCrackingImpl_();
			transform.SetSwapCoordinates();
			shape.ApplyTransformation(transform);
			// restore shape
			if (b_res)
			{
				if (result != null)
				{
					result.Assign(cracker.m_non_simple_result);
				}
				return true;
			}
			return false;
		}
	}
}
