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
	internal class RingOrientationFixer
	{
		internal com.epl.geometry.EditShape m_shape;

		internal com.epl.geometry.Treap m_AET;

		internal double m_y_scanline;

		internal int m_geometry;

		internal int m_unknown_ring_orientation_count;

		internal com.epl.geometry.IndexMultiDCList m_sorted_vertices;

		internal com.epl.geometry.AttributeStreamOfInt32 m_unknown_nodes;

		internal int m_node_1_user_index;

		internal int m_node_2_user_index;

		internal int m_path_orientation_index;

		internal int m_path_parentage_index;

		internal bool m_fixSelfTangency;

		internal sealed class Edges
		{
			internal com.epl.geometry.EditShape m_shape;

			internal com.epl.geometry.AttributeStreamOfInt32 m_end_1_nodes;

			internal com.epl.geometry.AttributeStreamOfInt32 m_end_2_nodes;

			internal com.epl.geometry.AttributeStreamOfInt8 m_directions;

			internal com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();

			internal com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();

			internal int m_first_free;

			internal bool GetDirection_(int index)
			{
				return m_shape.GetNextVertex(GetEnd1(index)) == GetEnd2(index);
			}

			internal int GetEnd_(int index)
			{
				int v_1 = GetEnd1(index);
				int v_2 = GetEnd2(index);
				if (m_shape.GetNextVertex(v_1) == v_2)
				{
					return v_2;
				}
				else
				{
					return v_1;
				}
			}

			internal Edges(com.epl.geometry.EditShape shape)
			{
				m_shape = shape;
				m_first_free = -1;
			}

			internal com.epl.geometry.Segment GetSegment(int index)
			{
				return m_shape.GetSegment(GetStart(index));
			}

			// True if the start vertex is the lower point of the edge.
			internal bool IsBottomUp(int index)
			{
				int v_1 = GetEnd1(index);
				int v_2 = GetEnd2(index);
				if (m_shape.GetPrevVertex(v_1) == v_2)
				{
					int temp = v_1;
					v_1 = v_2;
					v_2 = temp;
				}
				m_shape.GetXY(v_1, pt_1);
				m_shape.GetXY(v_2, pt_2);
				return pt_1.y < pt_2.y;
			}

			internal int GetStart(int index)
			{
				int v_1 = GetEnd1(index);
				int v_2 = GetEnd2(index);
				return (m_shape.GetNextVertex(v_1) == v_2) ? v_1 : v_2;
			}

			internal int GetEnd1(int index)
			{
				return m_end_1_nodes.Get(index);
			}

			internal int GetEnd2(int index)
			{
				return m_end_2_nodes.Get(index);
			}

			internal void FreeEdge(int edge)
			{
				m_end_1_nodes.Set(edge, m_first_free);
				m_first_free = edge;
			}

			internal int NewEdge(int vertex)
			{
				if (m_first_free != -1)
				{
					int index = m_first_free;
					m_first_free = m_end_1_nodes.Get(index);
					m_end_1_nodes.Set(index, vertex);
					m_end_2_nodes.Set(index, m_shape.GetNextVertex(vertex));
					return index;
				}
				else
				{
					if (m_end_1_nodes == null)
					{
						m_end_1_nodes = new com.epl.geometry.AttributeStreamOfInt32(0);
						m_end_2_nodes = new com.epl.geometry.AttributeStreamOfInt32(0);
					}
				}
				int index_1 = m_end_1_nodes.Size();
				m_end_1_nodes.Add(vertex);
				m_end_2_nodes.Add(m_shape.GetNextVertex(vertex));
				return index_1;
			}

			internal com.epl.geometry.EditShape GetShape()
			{
				return m_shape;
			}

			internal int GetPath(int index)
			{
				return m_shape.GetPathFromVertex(GetEnd1(index));
			}
		}

		internal com.epl.geometry.RingOrientationFixer.Edges m_edges;

		internal class RingOrientationTestComparator : com.epl.geometry.Treap.Comparator
		{
			internal com.epl.geometry.RingOrientationFixer m_helper;

			internal com.epl.geometry.Line m_line_1;

			internal com.epl.geometry.Line m_line_2;

			internal int m_left_elm;

			internal double m_leftx;

			internal com.epl.geometry.Segment m_seg_1;

			internal RingOrientationTestComparator(RingOrientationFixer _enclosing, com.epl.geometry.RingOrientationFixer helper)
			{
				this._enclosing = _enclosing;
				this.m_helper = helper;
				this.m_line_1 = new com.epl.geometry.Line();
				this.m_line_2 = new com.epl.geometry.Line();
				this.m_leftx = 0;
				this.m_seg_1 = null;
				this.m_left_elm = -1;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int left, int node)
			{
				int right = treap.GetElement(node);
				com.epl.geometry.RingOrientationFixer.Edges edges = this.m_helper.m_edges;
				double x_1;
				if (this.m_left_elm == left)
				{
					x_1 = this.m_leftx;
				}
				else
				{
					this.m_seg_1 = edges.GetSegment(left);
					if (this.m_seg_1 == null)
					{
						com.epl.geometry.EditShape shape = edges.GetShape();
						shape.QueryLineConnector(edges.GetStart(left), this.m_line_1);
						this.m_seg_1 = this.m_line_1;
						x_1 = this.m_line_1.IntersectionOfYMonotonicWithAxisX(this.m_helper.m_y_scanline, 0);
					}
					else
					{
						x_1 = this.m_seg_1.IntersectionOfYMonotonicWithAxisX(this.m_helper.m_y_scanline, 0);
					}
					this.m_leftx = x_1;
					this.m_left_elm = left;
				}
				com.epl.geometry.Segment seg_2 = edges.GetSegment(right);
				double x2;
				if (seg_2 == null)
				{
					com.epl.geometry.EditShape shape = edges.GetShape();
					shape.QueryLineConnector(edges.GetStart(right), this.m_line_2);
					seg_2 = this.m_line_2;
					x2 = this.m_line_2.IntersectionOfYMonotonicWithAxisX(this.m_helper.m_y_scanline, 0);
				}
				else
				{
					x2 = seg_2.IntersectionOfYMonotonicWithAxisX(this.m_helper.m_y_scanline, 0);
				}
				if (x_1 == x2)
				{
					bool bStartLower1 = edges.IsBottomUp(left);
					bool bStartLower2 = edges.IsBottomUp(right);
					// apparently these edges originate from same vertex and the
					// scanline is on the vertex. move scanline a little.
					double y1 = !bStartLower1 ? this.m_seg_1.GetStartY() : this.m_seg_1.GetEndY();
					double y2 = !bStartLower2 ? seg_2.GetStartY() : seg_2.GetEndY();
					double miny = System.Math.Min(y1, y2);
					double y = (miny + this.m_helper.m_y_scanline) * 0.5;
					if (y == this.m_helper.m_y_scanline)
					{
						// assert(0);//ST: not a bug. just curious to see this
						// happens.
						y = miny;
					}
					// apparently, one of the segments is almost
					// horizontal line.
					x_1 = this.m_seg_1.IntersectionOfYMonotonicWithAxisX(y, 0);
					x2 = seg_2.IntersectionOfYMonotonicWithAxisX(y, 0);
					System.Diagnostics.Debug.Assert((x_1 != x2));
				}
				return x_1 < x2 ? -1 : (x_1 > x2 ? 1 : 0);
			}

			internal virtual void Reset()
			{
				this.m_left_elm = -1;
			}

			private readonly RingOrientationFixer _enclosing;
		}

		internal com.epl.geometry.RingOrientationFixer.RingOrientationTestComparator m_sweep_comparator;

		internal RingOrientationFixer()
		{
			m_AET = new com.epl.geometry.Treap();
			m_AET.DisableBalancing();
			m_sweep_comparator = new com.epl.geometry.RingOrientationFixer.RingOrientationTestComparator(this, this);
			m_AET.SetComparator(m_sweep_comparator);
		}

		internal virtual bool FixRingOrientation_()
		{
			bool bFound = false;
			if (m_fixSelfTangency)
			{
				bFound = FixRingSelfTangency_();
			}
			if (m_shape.GetPathCount(m_geometry) == 1)
			{
				int path = m_shape.GetFirstPath(m_geometry);
				double area = m_shape.GetRingArea(path);
				m_shape.SetExterior(path, true);
				if (area < 0)
				{
					int first = m_shape.GetFirstVertex(path);
					m_shape.ReverseRingInternal_(first);
					m_shape.SetLastVertex_(path, m_shape.GetPrevVertex(first));
					// fix
					// last
					// after
					// the
					// reverse
					return true;
				}
				return false;
			}
			m_path_orientation_index = m_shape.CreatePathUserIndex();
			// used to
			// store
			// discovered
			// orientation
			// (3 -
			// extrior,
			// 2 -
			// interior)
			m_path_parentage_index = m_shape.CreatePathUserIndex();
			// used to
			// resolve OGC
			// order
			for (int path_1 = m_shape.GetFirstPath(m_geometry); path_1 != -1; path_1 = m_shape.GetNextPath(path_1))
			{
				m_shape.SetPathUserIndex(path_1, m_path_orientation_index, 0);
				m_shape.SetPathUserIndex(path_1, m_path_parentage_index, -1);
			}
			com.epl.geometry.AttributeStreamOfInt32 bunch = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_y_scanline = com.epl.geometry.NumberUtils.TheNaN;
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			m_unknown_ring_orientation_count = m_shape.GetPathCount(m_geometry);
			m_node_1_user_index = m_shape.CreateUserIndex();
			m_node_2_user_index = m_shape.CreateUserIndex();
			for (int ivertex = m_sorted_vertices.GetFirst(m_sorted_vertices.GetFirstList()); ivertex != -1; ivertex = m_sorted_vertices.GetNext(ivertex))
			{
				int vertex = m_sorted_vertices.GetData(ivertex);
				m_shape.GetXY(vertex, pt);
				if (pt.y != m_y_scanline && bunch.Size() != 0)
				{
					bFound |= ProcessBunchForRingOrientationTest_(bunch);
					m_sweep_comparator.Reset();
					bunch.Clear(false);
				}
				bunch.Add(vertex);
				// all vertices that have same y are added to the
				// bunch
				m_y_scanline = pt.y;
				if (m_unknown_ring_orientation_count == 0)
				{
					break;
				}
			}
			if (m_unknown_ring_orientation_count > 0)
			{
				bFound |= ProcessBunchForRingOrientationTest_(bunch);
				bunch.Clear(false);
			}
			m_shape.RemoveUserIndex(m_node_1_user_index);
			m_shape.RemoveUserIndex(m_node_2_user_index);
			// dbg_verify_ring_orientation_();//debug
			for (int path_2 = m_shape.GetFirstPath(m_geometry); path_2 != -1; )
			{
				if (m_shape.GetPathUserIndex(path_2, m_path_orientation_index) == 3)
				{
					// exterior
					m_shape.SetExterior(path_2, true);
					int afterPath = path_2;
					for (int nextHole = m_shape.GetPathUserIndex(path_2, m_path_parentage_index); nextHole != -1; )
					{
						int p = m_shape.GetPathUserIndex(nextHole, m_path_parentage_index);
						m_shape.MovePath(m_geometry, m_shape.GetNextPath(afterPath), nextHole);
						afterPath = nextHole;
						nextHole = p;
					}
					path_2 = m_shape.GetNextPath(afterPath);
				}
				else
				{
					m_shape.SetExterior(path_2, false);
					path_2 = m_shape.GetNextPath(path_2);
				}
			}
			m_shape.RemovePathUserIndex(m_path_orientation_index);
			m_shape.RemovePathUserIndex(m_path_parentage_index);
			return bFound;
		}

		internal virtual bool ProcessBunchForRingOrientationTest_(com.epl.geometry.AttributeStreamOfInt32 bunch)
		{
			return ProcessBunchForRingOrientationTestOddEven_(bunch);
		}

		internal virtual bool ProcessBunchForRingOrientationTestOddEven_(com.epl.geometry.AttributeStreamOfInt32 bunch)
		{
			bool bModified = false;
			if (m_edges == null)
			{
				m_edges = new com.epl.geometry.RingOrientationFixer.Edges(m_shape);
			}
			if (m_unknown_nodes == null)
			{
				m_unknown_nodes = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_unknown_nodes.Reserve(16);
			}
			else
			{
				m_unknown_nodes.Clear(false);
			}
			ProcessBunchForRingOrientationRemoveEdges_(bunch);
			// add edges that come into scope
			for (int i = 0, n = bunch.Size(); i < n; i++)
			{
				int vertex = bunch.Get(i);
				if (vertex == -1)
				{
					continue;
				}
				InsertEdge_(vertex, -1);
			}
			for (int i_1 = 0; i_1 < m_unknown_nodes.Size() && m_unknown_ring_orientation_count > 0; i_1++)
			{
				int aetNode = m_unknown_nodes.Get(i_1);
				int edge = m_AET.GetElement(aetNode);
				int path = m_edges.GetPath(edge);
				int orientation = m_shape.GetPathUserIndex(path, m_path_orientation_index);
				int prevPath = -1;
				if (orientation == 0)
				{
					int node = m_AET.GetPrev(aetNode);
					int prevNode = aetNode;
					bool odd_even = false;
					// find the leftmost edge for which the ring orientation is
					// known
					while (node != com.epl.geometry.Treap.NullNode())
					{
						int edge1 = m_AET.GetElement(node);
						int path1 = m_edges.GetPath(edge1);
						int orientation1 = m_shape.GetPathUserIndex(path1, m_path_orientation_index);
						if (orientation1 != 0)
						{
							prevPath = path1;
							break;
						}
						prevNode = node;
						node = m_AET.GetPrev(node);
					}
					if (node == com.epl.geometry.Treap.NullNode())
					{
						// if no edges have ring
						// orientation known, then start
						// from the left most and it has
						// to be exterior ring.
						odd_even = true;
						node = prevNode;
					}
					else
					{
						int edge1 = m_AET.GetElement(node);
						odd_even = m_edges.IsBottomUp(edge1);
						node = m_AET.GetNext(node);
						odd_even = !odd_even;
					}
					do
					{
						int edge1 = m_AET.GetElement(node);
						int path1 = m_edges.GetPath(edge1);
						int orientation1 = m_shape.GetPathUserIndex(path1, m_path_orientation_index);
						if (orientation1 == 0)
						{
							if (odd_even != m_edges.IsBottomUp(edge1))
							{
								int first = m_shape.GetFirstVertex(path1);
								m_shape.ReverseRingInternal_(first);
								m_shape.SetLastVertex_(path1, m_shape.GetPrevVertex(first));
								bModified = true;
							}
							m_shape.SetPathUserIndex(path1, m_path_orientation_index, odd_even ? 3 : 2);
							if (!odd_even)
							{
								// link the holes into the linked list
								// to mantain the OGC order.
								int lastHole = m_shape.GetPathUserIndex(prevPath, m_path_parentage_index);
								m_shape.SetPathUserIndex(prevPath, m_path_parentage_index, path1);
								m_shape.SetPathUserIndex(path1, m_path_parentage_index, lastHole);
							}
							m_unknown_ring_orientation_count--;
							if (m_unknown_ring_orientation_count == 0)
							{
								return bModified;
							}
						}
						prevPath = path1;
						prevNode = node;
						node = m_AET.GetNext(node);
						odd_even = !odd_even;
					}
					while (prevNode != aetNode);
				}
			}
			return bModified;
		}

		internal virtual void ProcessBunchForRingOrientationRemoveEdges_(com.epl.geometry.AttributeStreamOfInt32 bunch)
		{
			// remove all nodes that go out of scope
			for (int i = 0, n = bunch.Size(); i < n; i++)
			{
				int vertex = bunch.Get(i);
				int node1 = m_shape.GetUserIndex(vertex, m_node_1_user_index);
				int node2 = m_shape.GetUserIndex(vertex, m_node_2_user_index);
				if (node1 != -1)
				{
					int edge = m_AET.GetElement(node1);
					m_edges.FreeEdge(edge);
					m_shape.SetUserIndex(vertex, m_node_1_user_index, -1);
				}
				if (node2 != -1)
				{
					int edge = m_AET.GetElement(node2);
					m_edges.FreeEdge(edge);
					m_shape.SetUserIndex(vertex, m_node_2_user_index, -1);
				}
				int reused_node = -1;
				if (node1 != -1 && node2 != -1)
				{
					// terminating vertex
					m_AET.DeleteNode(node1, -1);
					m_AET.DeleteNode(node2, -1);
					bunch.Set(i, -1);
				}
				else
				{
					reused_node = node1 != -1 ? node1 : node2;
				}
				if (reused_node != -1)
				{
					// this vertex is a part of vertical chain.
					// Sorted order in AET did not change, so
					// reuse the AET node.
					if (!InsertEdge_(vertex, reused_node))
					{
						m_AET.DeleteNode(reused_node, -1);
					}
					// horizontal edge was not
					// inserted
					bunch.Set(i, -1);
				}
			}
		}

		internal virtual bool InsertEdge_(int vertex, int reused_node)
		{
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			m_shape.GetXY(vertex, pt_1);
			int next = m_shape.GetNextVertex(vertex);
			m_shape.GetXY(next, pt_2);
			bool b_res = false;
			if (pt_1.y < pt_2.y)
			{
				b_res = true;
				int edge = m_edges.NewEdge(vertex);
				int aetNode;
				if (reused_node == -1)
				{
					aetNode = m_AET.AddElement(edge, -1);
				}
				else
				{
					aetNode = reused_node;
					m_AET.SetElement(aetNode, edge);
				}
				int node = m_shape.GetUserIndex(next, m_node_1_user_index);
				if (node == -1)
				{
					m_shape.SetUserIndex(next, m_node_1_user_index, aetNode);
				}
				else
				{
					m_shape.SetUserIndex(next, m_node_2_user_index, aetNode);
				}
				int path = m_shape.GetPathFromVertex(vertex);
				if (m_shape.GetPathUserIndex(path, m_path_orientation_index) == 0)
				{
					m_unknown_nodes.Add(aetNode);
				}
			}
			int prev = m_shape.GetPrevVertex(vertex);
			m_shape.GetXY(prev, pt_2);
			if (pt_1.y < pt_2.y)
			{
				b_res = true;
				int edge = m_edges.NewEdge(prev);
				int aetNode;
				if (reused_node == -1)
				{
					aetNode = m_AET.AddElement(edge, -1);
				}
				else
				{
					aetNode = reused_node;
					m_AET.SetElement(aetNode, edge);
				}
				int node = m_shape.GetUserIndex(prev, m_node_1_user_index);
				if (node == -1)
				{
					m_shape.SetUserIndex(prev, m_node_1_user_index, aetNode);
				}
				else
				{
					m_shape.SetUserIndex(prev, m_node_2_user_index, aetNode);
				}
				int path = m_shape.GetPathFromVertex(vertex);
				if (m_shape.GetPathUserIndex(path, m_path_orientation_index) == 0)
				{
					m_unknown_nodes.Add(aetNode);
				}
			}
			return b_res;
		}

		internal static bool Execute(com.epl.geometry.EditShape shape, int geometry, com.epl.geometry.IndexMultiDCList sorted_vertices, bool fixSelfTangency)
		{
			com.epl.geometry.RingOrientationFixer fixer = new com.epl.geometry.RingOrientationFixer();
			fixer.m_shape = shape;
			fixer.m_geometry = geometry;
			fixer.m_sorted_vertices = sorted_vertices;
			fixer.m_fixSelfTangency = fixSelfTangency;
			return fixer.FixRingOrientation_();
		}

		internal virtual bool FixRingSelfTangency_()
		{
			com.epl.geometry.AttributeStreamOfInt32 self_tangent_paths = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 self_tangency_clusters = new com.epl.geometry.AttributeStreamOfInt32(0);
			int tangent_path_first_vertex_index = -1;
			int tangent_vertex_cluster_index = -1;
			com.epl.geometry.Point2D pt_prev = new com.epl.geometry.Point2D();
			pt_prev.SetNaN();
			int prev_vertex = -1;
			int old_path = -1;
			int current_cluster = -1;
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int ivertex = m_sorted_vertices.GetFirst(m_sorted_vertices.GetFirstList()); ivertex != -1; ivertex = m_sorted_vertices.GetNext(ivertex))
			{
				int vertex = m_sorted_vertices.GetData(ivertex);
				m_shape.GetXY(vertex, pt);
				int path = m_shape.GetPathFromVertex(vertex);
				if (pt_prev.IsEqual(pt) && old_path == path)
				{
					if (tangent_vertex_cluster_index == -1)
					{
						tangent_path_first_vertex_index = m_shape.CreatePathUserIndex();
						tangent_vertex_cluster_index = m_shape.CreateUserIndex();
					}
					if (current_cluster == -1)
					{
						current_cluster = self_tangency_clusters.Size();
						m_shape.SetUserIndex(prev_vertex, tangent_vertex_cluster_index, current_cluster);
						self_tangency_clusters.Add(1);
						int p = m_shape.GetPathUserIndex(path, tangent_path_first_vertex_index);
						if (p == -1)
						{
							m_shape.SetPathUserIndex(path, tangent_path_first_vertex_index, prev_vertex);
							self_tangent_paths.Add(path);
						}
					}
					m_shape.SetUserIndex(vertex, tangent_vertex_cluster_index, current_cluster);
					self_tangency_clusters.SetLast(self_tangency_clusters.GetLast() + 1);
				}
				else
				{
					current_cluster = -1;
					pt_prev.SetCoords(pt);
				}
				prev_vertex = vertex;
				old_path = path;
			}
			if (self_tangent_paths.Size() == 0)
			{
				return false;
			}
			// Now self_tangent_paths contains list of clusters of tangency for each
			// path.
			// The clusters contains list of clusters and for each cluster it
			// contains a list of vertices.
			com.epl.geometry.AttributeStreamOfInt32 vertex_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 cluster_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			for (int ipath = 0, npath = self_tangent_paths.Size(); ipath < npath; ipath++)
			{
				int path = self_tangent_paths.Get(ipath);
				int first_vertex = m_shape.GetPathUserIndex(path, tangent_path_first_vertex_index);
				int cluster = m_shape.GetUserIndex(first_vertex, tangent_vertex_cluster_index);
				vertex_stack.Clear(false);
				cluster_stack.Clear(false);
				vertex_stack.Add(first_vertex);
				cluster_stack.Add(cluster);
				for (int vertex = m_shape.GetNextVertex(first_vertex); vertex != first_vertex; vertex = m_shape.GetNextVertex(vertex))
				{
					int vertex_to = vertex;
					int cluster_to = m_shape.GetUserIndex(vertex_to, tangent_vertex_cluster_index);
					if (cluster_to != -1)
					{
						if (cluster_stack.Size() == 0)
						{
							cluster_stack.Add(cluster_to);
							vertex_stack.Add(vertex_to);
							continue;
						}
						if (cluster_stack.GetLast() == cluster_to)
						{
							int vertex_from = vertex_stack.GetLast();
							// peel the loop from path
							int from_next = m_shape.GetNextVertex(vertex_from);
							int from_prev = m_shape.GetPrevVertex(vertex_from);
							int to_next = m_shape.GetNextVertex(vertex_to);
							int to_prev = m_shape.GetPrevVertex(vertex_to);
							m_shape.SetNextVertex_(vertex_from, to_next);
							m_shape.SetPrevVertex_(to_next, vertex_from);
							m_shape.SetNextVertex_(vertex_to, from_next);
							m_shape.SetPrevVertex_(from_next, vertex_to);
							// vertex_from is left in the path we are processing,
							// while the vertex_to is in the loop being teared off.
							bool[] first_vertex_correction_requied = new bool[] { false };
							int new_path = m_shape.InsertClosedPath_(m_geometry, -1, from_next, m_shape.GetFirstVertex(path), first_vertex_correction_requied);
							m_shape.SetUserIndex(vertex, tangent_vertex_cluster_index, -1);
							// Fix the path after peeling if the peeled loop had the
							// first path vertex in it
							if (first_vertex_correction_requied[0])
							{
								m_shape.SetFirstVertex_(path, to_next);
							}
							int path_size = m_shape.GetPathSize(path);
							int new_path_size = m_shape.GetPathSize(new_path);
							path_size -= new_path_size;
							System.Diagnostics.Debug.Assert((path_size >= 3));
							m_shape.SetPathSize_(path, path_size);
							self_tangency_clusters.Set(cluster_to, self_tangency_clusters.Get(cluster_to) - 1);
							if (self_tangency_clusters.Get(cluster_to) == 1)
							{
								self_tangency_clusters.Set(cluster_to, 0);
								cluster_stack.RemoveLast();
								vertex_stack.RemoveLast();
							}
							// this cluster has more than two vertices in it.
							first_vertex = vertex_from;
							// reset the counter to
							// ensure we find all loops.
							vertex = vertex_from;
						}
						else
						{
							vertex_stack.Add(vertex);
							cluster_stack.Add(cluster_to);
						}
					}
				}
			}
			m_shape.RemovePathUserIndex(tangent_path_first_vertex_index);
			m_shape.RemoveUserIndex(tangent_vertex_cluster_index);
			return true;
		}
	}
}
