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
	internal sealed class PlaneSweepCrackerHelper
	{
		internal PlaneSweepCrackerHelper()
		{
			m_edges = new com.epl.geometry.StridedIndexTypeCollection(8);
			m_clusters = new com.epl.geometry.StridedIndexTypeCollection(5);
			m_cluster_vertices = new com.epl.geometry.IndexMultiList();
			m_edge_vertices = new com.epl.geometry.IndexMultiList();
			m_complications = false;
			m_sweep_point = new com.epl.geometry.Point2D();
			m_sweep_point.SetCoords(0, 0);
			m_tolerance = 0;
			m_vertex_cluster_index = -1;
			m_b_cracked = false;
			m_shape = null;
			m_event_q = new com.epl.geometry.Treap();
			m_sweep_structure = new com.epl.geometry.Treap();
			m_edges_to_insert_in_sweep_structure = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_segment_intersector = new com.epl.geometry.SegmentIntersector();
			m_temp_edge_buffer = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_modified_clusters = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_helper_point = new com.epl.geometry.Point();
		}

		// For use in Cluster/Cracker loop
		internal bool Sweep(com.epl.geometry.EditShape shape, double tolerance)
		{
			com.epl.geometry.Transformation2D transform = new com.epl.geometry.Transformation2D();
			transform.SetSwapCoordinates();
			shape.ApplyTransformation(transform);
			// swap coordinates for the sweep
			// along x
			SetEditShape_(shape);
			m_b_cracked = false;
			m_tolerance = tolerance;
			m_tolerance_sqr = tolerance * tolerance;
			bool b_cracked = SweepImpl_();
			shape.ApplyTransformation(transform);
			if (!b_cracked)
			{
				FillEventQueuePass2();
				b_cracked |= SweepImpl_();
			}
			if (m_vertex_cluster_index != -1)
			{
				m_shape.RemoveUserIndex(m_vertex_cluster_index);
				m_vertex_cluster_index = -1;
			}
			m_shape = null;
			return m_b_cracked;
		}

		// Does one pass sweep vertically
		internal bool SweepVertical(com.epl.geometry.EditShape shape, double tolerance)
		{
			SetEditShape_(shape);
			m_b_cracked = false;
			m_tolerance = tolerance;
			m_tolerance_sqr = tolerance * tolerance;
			m_complications = false;
			bool bresult = SweepImpl_();
			if (!m_complications)
			{
				int filtered = shape.FilterClosePoints(tolerance, true, false);
				m_complications = filtered == 1;
				bresult |= filtered == 1;
			}
			if (m_vertex_cluster_index != -1)
			{
				m_shape.RemoveUserIndex(m_vertex_cluster_index);
				m_vertex_cluster_index = -1;
			}
			m_shape = null;
			return bresult;
		}

		internal bool HadCompications()
		{
			return m_complications;
		}

		private com.epl.geometry.EditShape m_shape;

		private com.epl.geometry.StridedIndexTypeCollection m_edges;

		private com.epl.geometry.StridedIndexTypeCollection m_clusters;

		private com.epl.geometry.IndexMultiList m_cluster_vertices;

		private com.epl.geometry.IndexMultiList m_edge_vertices;

		private com.epl.geometry.Point m_helper_point;

		private com.epl.geometry.Treap m_event_q;

		private com.epl.geometry.Treap m_sweep_structure;

		internal bool m_complications;

		internal sealed class SimplifySweepComparator : com.epl.geometry.SweepComparator
		{
			internal com.epl.geometry.PlaneSweepCrackerHelper m_parent;

			internal SimplifySweepComparator(com.epl.geometry.PlaneSweepCrackerHelper parent)
				: base(parent.m_shape, parent.m_tolerance, false)
			{
				m_parent = parent;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int elm, int node)
			{
				// Compares two segments on a sweep line passing through m_sweep_y,
				// m_sweep_x.
				if (m_b_intersection_detected)
				{
					return -1;
				}
				int vertex_list_left = m_parent.GetEdgeOriginVertices(elm);
				int left = m_parent.m_edge_vertices.GetFirstElement(vertex_list_left);
				int right_elm = treap.GetElement(node);
				System.Diagnostics.Debug.Assert((m_parent.GetEdgeSweepNode(right_elm) == node));
				int vertex_list_right = m_parent.GetEdgeOriginVertices(right_elm);
				int right = m_parent.m_edge_vertices.GetFirstElement(vertex_list_right);
				m_current_node = node;
				return CompareSegments(elm, left, right_elm, right);
			}
		}

		internal sealed class SimplifySweepMonikerComparator : com.epl.geometry.SweepMonkierComparator
		{
			internal com.epl.geometry.PlaneSweepCrackerHelper m_parent;

			internal SimplifySweepMonikerComparator(com.epl.geometry.PlaneSweepCrackerHelper parent)
				: base(parent.m_shape, parent.m_tolerance)
			{
				m_parent = parent;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int node)
			{
				// Compares two segments on a sweep line passing through m_sweep_y,
				// m_sweep_x.
				if (m_b_intersection_detected)
				{
					return -1;
				}
				int elm = treap.GetElement(node);
				int vertexList = m_parent.GetEdgeOriginVertices(elm);
				int vertex = m_parent.m_edge_vertices.GetFirstElement(vertexList);
				m_current_node = node;
				return CompareVertex_(treap, node, vertex);
			}
		}

		internal com.epl.geometry.PlaneSweepCrackerHelper.SimplifySweepComparator m_sweep_comparator;

		internal com.epl.geometry.AttributeStreamOfInt32 m_temp_edge_buffer;

		internal com.epl.geometry.AttributeStreamOfInt32 m_modified_clusters;

		internal com.epl.geometry.AttributeStreamOfInt32 m_edges_to_insert_in_sweep_structure;

		internal int m_prev_neighbour;

		internal int m_next_neighbour;

		internal bool m_b_continuing_segment_chain_optimization;

		internal com.epl.geometry.SegmentIntersector m_segment_intersector;

		internal com.epl.geometry.Line m_line_1;

		internal com.epl.geometry.Line m_line_2;

		internal com.epl.geometry.Point2D m_sweep_point;

		internal double m_tolerance;

		internal double m_tolerance_sqr;

		internal int m_sweep_point_cluster;

		internal int m_vertex_cluster_index;

		internal bool m_b_cracked;

		internal bool m_b_sweep_point_cluster_was_modified;

		// set to true, when the
		// cluster has two edges
		// attached, one is
		// below and another
		// above the sweep line
		// set to true if the
		// coordinates of the
		// cluster, where the sweep
		// line was, has been
		// changed.
		internal int GetEdgeCluster(int edge, int end)
		{
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			return m_edges.GetField(edge, 0 + end);
		}

		internal void SetEdgeCluster_(int edge, int end, int cluster)
		{
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			m_edges.SetField(edge, 0 + end, cluster);
		}

		// Edge may have several origin vertices, when there are two or more equal
		// segements in that edge
		// We have to store edge origin separately from the cluster vertices,
		// because cluster can have several different edges started on it.
		internal int GetEdgeOriginVertices(int edge)
		{
			return m_edges.GetField(edge, 2);
		}

		internal void SetEdgeOriginVertices_(int edge, int vertices)
		{
			m_edges.SetField(edge, 2, vertices);
		}

		internal int GetNextEdgeEx(int edge, int end)
		{
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			return m_edges.GetField(edge, 3 + end);
		}

		internal void SetNextEdgeEx_(int edge, int end, int next_edge)
		{
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			m_edges.SetField(edge, 3 + end, next_edge);
		}

		// int get_prev_edge_ex(int edge, int end)
		// {
		// assert(end == 0 || end == 1);
		// return m_edges.get_field(edge, 5 + end);
		// }
		// void set_prev_edge_ex_(int edge, int end, int prevEdge)
		// {
		// assert(end == 0 || end == 1);
		// m_edges.set_field(edge, 5 + end, prevEdge);
		// }
		internal int GetEdgeSweepNode(int edge)
		{
			return m_edges.GetField(edge, 7);
		}

		internal void SetEdgeSweepNode_(int edge, int sweepNode)
		{
			m_edges.SetField(edge, 7, sweepNode);
		}

		internal int GetNextEdge(int edge, int cluster)
		{
			int end = GetEdgeEnd(edge, cluster);
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			return m_edges.GetField(edge, 3 + end);
		}

		internal void SetNextEdge_(int edge, int cluster, int next_edge)
		{
			int end = GetEdgeEnd(edge, cluster);
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			m_edges.SetField(edge, 3 + end, next_edge);
		}

		internal int GetPrevEdge(int edge, int cluster)
		{
			int end = GetEdgeEnd(edge, cluster);
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			return m_edges.GetField(edge, 5 + end);
		}

		internal void SetPrevEdge_(int edge, int cluster, int prevEdge)
		{
			int end = GetEdgeEnd(edge, cluster);
			System.Diagnostics.Debug.Assert((end == 0 || end == 1));
			m_edges.SetField(edge, 5 + end, prevEdge);
		}

		internal int GetClusterVertices(int cluster)
		{
			return m_clusters.GetField(cluster, 0);
		}

		internal void SetClusterVertices_(int cluster, int vertices)
		{
			m_clusters.SetField(cluster, 0, vertices);
		}

		internal int GetClusterVertexIndex(int cluster)
		{
			return m_clusters.GetField(cluster, 4);
		}

		internal void SetClusterVertexIndex_(int cluster, int vindex)
		{
			m_clusters.SetField(cluster, 4, vindex);
		}

		internal int GetClusterSweepEdgeList(int cluster)
		{
			return m_clusters.GetField(cluster, 2);
		}

		internal void SetClusterSweepEdgeList_(int cluster, int sweep_edges)
		{
			m_clusters.SetField(cluster, 2, sweep_edges);
		}

		internal int GetClusterFirstEdge(int cluster)
		{
			return m_clusters.GetField(cluster, 1);
		}

		internal void SetClusterFirstEdge_(int cluster, int first_edge)
		{
			m_clusters.SetField(cluster, 1, first_edge);
		}

		internal int GetClusterEventQNode(int cluster)
		{
			return m_clusters.GetField(cluster, 3);
		}

		internal void SetClusterEventQNode_(int cluster, int node)
		{
			m_clusters.SetField(cluster, 3, node);
		}

		internal int NewCluster_(int vertex)
		{
			int cluster = m_clusters.NewElement();
			int vertexList = m_cluster_vertices.CreateList();
			SetClusterVertices_(cluster, vertexList);
			if (vertex != -1)
			{
				m_cluster_vertices.AddElement(vertexList, vertex);
				System.Diagnostics.Debug.Assert((m_shape.GetUserIndex(vertex, m_vertex_cluster_index) == -1));
				m_shape.SetUserIndex(vertex, m_vertex_cluster_index, cluster);
				SetClusterVertexIndex_(cluster, m_shape.GetVertexIndex(vertex));
			}
			else
			{
				SetClusterVertexIndex_(cluster, -1);
			}
			return cluster;
		}

		internal void DeleteCluster_(int cluster)
		{
			m_clusters.DeleteElement(cluster);
		}

		internal void AddVertexToCluster_(int cluster, int vertex)
		{
			System.Diagnostics.Debug.Assert((m_shape.GetUserIndex(vertex, m_vertex_cluster_index) == -1));
			int vertexList = GetClusterVertices(cluster);
			m_cluster_vertices.AddElement(vertexList, vertex);
			m_shape.SetUserIndex(vertex, m_vertex_cluster_index, cluster);
		}

		// Creates a new unattached edge with the given origin.
		internal int NewEdge_(int origin_vertex)
		{
			int edge = m_edges.NewElement();
			int edgeVertices = m_edge_vertices.CreateList();
			SetEdgeOriginVertices_(edge, edgeVertices);
			if (origin_vertex != -1)
			{
				m_edge_vertices.AddElement(edgeVertices, origin_vertex);
			}
			return edge;
		}

		internal void AddVertexToEdge_(int edge, int vertex)
		{
			int vertexList = GetEdgeOriginVertices(edge);
			m_edge_vertices.AddElement(vertexList, vertex);
		}

		internal void DeleteEdge_(int edge)
		{
			m_edges.DeleteElement(edge);
			int ind = m_edges_to_insert_in_sweep_structure.FindElement(edge);
			if (ind >= 0)
			{
				m_edges_to_insert_in_sweep_structure.PopElement(ind);
			}
		}

		internal void AddEdgeToCluster(int edge, int cluster)
		{
			if (GetEdgeCluster(edge, 0) == -1)
			{
				System.Diagnostics.Debug.Assert((GetEdgeCluster(edge, 1) != cluster));
				SetEdgeCluster_(edge, 0, cluster);
			}
			else
			{
				if (GetEdgeCluster(edge, 1) == -1)
				{
					System.Diagnostics.Debug.Assert((GetEdgeCluster(edge, 0) != cluster));
					SetEdgeCluster_(edge, 1, cluster);
				}
				else
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
			AddEdgeToClusterImpl_(edge, cluster);
		}

		// simply adds the edge to the list
		// of cluster edges.
		internal void AddEdgeToClusterImpl_(int edge, int cluster)
		{
			int first_edge = GetClusterFirstEdge(cluster);
			if (first_edge != -1)
			{
				int next = GetNextEdge(first_edge, cluster);
				SetPrevEdge_(next, cluster, edge);
				SetNextEdge_(edge, cluster, next);
				SetNextEdge_(first_edge, cluster, edge);
				SetPrevEdge_(edge, cluster, first_edge);
			}
			else
			{
				SetPrevEdge_(edge, cluster, edge);
				// point to itself
				SetNextEdge_(edge, cluster, edge);
				SetClusterFirstEdge_(cluster, edge);
			}
		}

		internal int GetEdgeEnd(int edge, int cluster)
		{
			if (GetEdgeCluster(edge, 0) == cluster)
			{
				System.Diagnostics.Debug.Assert((GetEdgeCluster(edge, 1) != cluster));
				return 0;
			}
			else
			{
				System.Diagnostics.Debug.Assert((GetEdgeCluster(edge, 1) == cluster));
				return 1;
			}
		}

		// Merges two coincident clusters into one. The cluster2 becomes invalid.
		internal void MergeClusters_(int cluster_1, int cluster2)
		{
			// dbg_check_cluster_(cluster_1);
			// dbg_check_cluster_(cluster2);
			int eventQnode = GetClusterEventQNode(cluster2);
			if (eventQnode != -1)
			{
				m_event_q.DeleteNode(eventQnode, -1);
				SetClusterEventQNode_(cluster2, -1);
			}
			int firstEdge1 = GetClusterFirstEdge(cluster_1);
			int firstEdge2 = GetClusterFirstEdge(cluster2);
			if (firstEdge2 != -1)
			{
				// scope
				int edge2 = firstEdge2;
				int lastEdge = firstEdge2;
				bool bForceContinue = false;
				do
				{
					// Delete edges that connect cluster_1 and cluster2.
					// dbg_check_edge_(edge2);
					bForceContinue = false;
					// assert(!StridedIndexTypeCollection.isValidElement(getEdgeSweepNode(edge2)));
					int end = GetEdgeEnd(edge2, cluster2);
					int nextEdge2 = GetNextEdgeEx(edge2, end);
					if (GetEdgeCluster(edge2, (end + 1) & 1) == cluster_1)
					{
						// Snapping
						// clusters
						// that
						// are
						// connected
						// with
						// an
						// edge
						// Delete
						// the
						// edge.
						DisconnectEdge_(edge2);
						int edgeOrigins2 = GetEdgeOriginVertices(edge2);
						m_edge_vertices.DeleteList(edgeOrigins2);
						DeleteEdge_(edge2);
						if (edge2 == nextEdge2)
						{
							// deleted last edge connecting to
							// the cluster2 (all connections
							// are degenerate)
							firstEdge2 = -1;
							break;
						}
						if (firstEdge2 == edge2)
						{
							firstEdge2 = GetClusterFirstEdge(cluster2);
							lastEdge = nextEdge2;
							bForceContinue = true;
						}
					}
					else
					{
						System.Diagnostics.Debug.Assert((edge2 != GetClusterFirstEdge(cluster_1)));
					}
					edge2 = nextEdge2;
				}
				while (edge2 != lastEdge || bForceContinue);
				if (firstEdge2 != -1)
				{
					do
					{
						// set the cluster to the edge ends
						int end = GetEdgeEnd(edge2, cluster2);
						int nextEdge2 = GetNextEdgeEx(edge2, end);
						System.Diagnostics.Debug.Assert((edge2 != GetClusterFirstEdge(cluster_1)));
						SetEdgeCluster_(edge2, end, cluster_1);
						edge2 = nextEdge2;
					}
					while (edge2 != lastEdge);
					firstEdge1 = GetClusterFirstEdge(cluster_1);
					if (firstEdge1 != -1)
					{
						int next1 = GetNextEdge(firstEdge1, cluster_1);
						int next2 = GetNextEdge(firstEdge2, cluster_1);
						if (next1 == firstEdge1)
						{
							SetClusterFirstEdge_(cluster_1, firstEdge2);
							AddEdgeToClusterImpl_(firstEdge1, cluster_1);
							SetClusterFirstEdge_(cluster_1, firstEdge1);
						}
						else
						{
							if (next2 == firstEdge2)
							{
								AddEdgeToClusterImpl_(firstEdge2, cluster_1);
							}
						}
						SetNextEdge_(firstEdge2, cluster_1, next1);
						SetPrevEdge_(next1, cluster_1, firstEdge2);
						SetNextEdge_(firstEdge1, cluster_1, next2);
						SetPrevEdge_(next2, cluster_1, firstEdge1);
					}
					else
					{
						SetClusterFirstEdge_(cluster_1, firstEdge2);
					}
				}
			}
			int vertices1 = GetClusterVertices(cluster_1);
			int vertices2 = GetClusterVertices(cluster2);
			// Update cluster info on vertices.
			for (int vh = m_cluster_vertices.GetFirst(vertices2); vh != -1; vh = m_cluster_vertices.GetNext(vh))
			{
				int v = m_cluster_vertices.GetElement(vh);
				m_shape.SetUserIndex(v, m_vertex_cluster_index, cluster_1);
			}
			m_cluster_vertices.ConcatenateLists(vertices1, vertices2);
			DeleteCluster_(cluster2);
		}

		// dbg_check_cluster_(cluster_1);
		// Merges two coincident edges into one. The edge2 becomes invalid.
		internal void MergeEdges_(int edge1, int edge2)
		{
			// dbg_check_edge_(edge1);
			int cluster_1 = GetEdgeCluster(edge1, 0);
			int cluster2 = GetEdgeCluster(edge1, 1);
			int cluster21 = GetEdgeCluster(edge2, 0);
			int cluster22 = GetEdgeCluster(edge2, 1);
			int originVertices1 = GetEdgeOriginVertices(edge1);
			int originVertices2 = GetEdgeOriginVertices(edge2);
			m_edge_vertices.ConcatenateLists(originVertices1, originVertices2);
			if (edge2 == GetClusterFirstEdge(cluster_1))
			{
				SetClusterFirstEdge_(cluster_1, edge1);
			}
			if (edge2 == GetClusterFirstEdge(cluster2))
			{
				SetClusterFirstEdge_(cluster2, edge1);
			}
			DisconnectEdge_(edge2);
			// disconnects the edge2 from the clusters.
			DeleteEdge_(edge2);
			if (!((cluster_1 == cluster21 && cluster2 == cluster22) || (cluster2 == cluster21 && cluster_1 == cluster22)))
			{
				// Merged edges have different clusters (clusters have not yet been
				// merged)
				// merge clusters before merging the edges
				GetClusterXY(cluster_1, pt_1);
				GetClusterXY(cluster21, pt_2);
				if (pt_1.IsEqual(pt_2))
				{
					if (cluster_1 != cluster21)
					{
						MergeClusters_(cluster_1, cluster21);
						System.Diagnostics.Debug.Assert((!m_modified_clusters.HasElement(cluster21)));
					}
					if (cluster2 != cluster22)
					{
						MergeClusters_(cluster2, cluster22);
						System.Diagnostics.Debug.Assert((!m_modified_clusters.HasElement(cluster22)));
					}
				}
				else
				{
					if (cluster2 != cluster21)
					{
						MergeClusters_(cluster2, cluster21);
						System.Diagnostics.Debug.Assert((!m_modified_clusters.HasElement(cluster21)));
					}
					if (cluster_1 != cluster22)
					{
						MergeClusters_(cluster_1, cluster22);
						System.Diagnostics.Debug.Assert((!m_modified_clusters.HasElement(cluster22)));
					}
				}
			}
		}

		// Merged edges have equal clusters.
		// dbg_check_edge_(edge1);
		// Disconnects the edge from its clusters.
		internal void DisconnectEdge_(int edge)
		{
			int cluster_1 = GetEdgeCluster(edge, 0);
			int cluster2 = GetEdgeCluster(edge, 1);
			DisconnectEdgeFromCluster_(edge, cluster_1);
			DisconnectEdgeFromCluster_(edge, cluster2);
		}

		// Disconnects the edge from a cluster it is connected to.
		internal void DisconnectEdgeFromCluster_(int edge, int cluster)
		{
			int next = GetNextEdge(edge, cluster);
			System.Diagnostics.Debug.Assert((GetPrevEdge(next, cluster) == edge));
			int prev = GetPrevEdge(edge, cluster);
			System.Diagnostics.Debug.Assert((GetNextEdge(prev, cluster) == edge));
			int first_edge = GetClusterFirstEdge(cluster);
			if (next != edge)
			{
				SetNextEdge_(prev, cluster, next);
				SetPrevEdge_(next, cluster, prev);
				if (first_edge == edge)
				{
					SetClusterFirstEdge_(cluster, next);
				}
			}
			else
			{
				SetClusterFirstEdge_(cluster, -1);
			}
		}

		internal void ApplyIntersectorToEditShape_(int edgeOrigins, com.epl.geometry.SegmentIntersector intersector, int intersector_index)
		{
			// Split Edit_shape segments and produce new vertices. Modify
			// coordinates as necessary. No vertices are deleted.
			int vertexHandle = m_edge_vertices.GetFirst(edgeOrigins);
			int first_vertex = m_edge_vertices.GetElement(vertexHandle);
			int cluster_1 = GetClusterFromVertex(first_vertex);
			int cluster2 = GetClusterFromVertex(m_shape.GetNextVertex(first_vertex));
			bool bComplexCase = cluster_1 == cluster2;
			System.Diagnostics.Debug.Assert((!bComplexCase));
			// if it ever asserts there will be a bug. Should
			// be a case of a curve that forms a loop.
			m_shape.SplitSegment_(first_vertex, intersector, intersector_index, true);
			for (vertexHandle = m_edge_vertices.GetNext(vertexHandle); vertexHandle != -1; vertexHandle = m_edge_vertices.GetNext(vertexHandle))
			{
				int vertex = m_edge_vertices.GetElement(vertexHandle);
				bool b_forward = GetClusterFromVertex(vertex) == cluster_1;
				System.Diagnostics.Debug.Assert(((b_forward && GetClusterFromVertex(m_shape.GetNextVertex(vertex)) == cluster2) || (GetClusterFromVertex(vertex) == cluster2 && GetClusterFromVertex(m_shape.GetNextVertex(vertex)) == cluster_1)));
				m_shape.SplitSegment_(vertex, intersector, intersector_index, b_forward);
			}
			// Now apply the updated coordinates to all vertices in the cluster_1
			// and cluster2.
			com.epl.geometry.Point2D pt_0;
			com.epl.geometry.Point2D pt_1;
			pt_0 = intersector.GetResultSegment(intersector_index, 0).GetStartXY();
			pt_1 = intersector.GetResultSegment(intersector_index, intersector.GetResultSegmentCount(intersector_index) - 1).GetEndXY();
			UpdateClusterXY(cluster_1, pt_0);
			UpdateClusterXY(cluster2, pt_1);
		}

		internal void CreateEdgesAndClustersFromSplitEdge_(int edge1, com.epl.geometry.SegmentIntersector intersector, int intersector_index)
		{
			// dbg_check_new_edges_array_();
			// The method uses m_temp_edge_buffer for temporary storage and clears
			// it at the end.
			int edgeOrigins1 = GetEdgeOriginVertices(edge1);
			// create new edges and clusters
			// Note that edge1 is disconnected from its clusters already (the
			// cluster's edge list does not contain it).
			int cluster_1 = GetEdgeCluster(edge1, 0);
			int cluster2 = GetEdgeCluster(edge1, 1);
			int prevEdge = NewEdge_(-1);
			m_edges_to_insert_in_sweep_structure.Add(prevEdge);
			int c_3 = com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex3();
			SetEdgeSweepNode_(prevEdge, c_3);
			// mark that its in
			// m_edges_to_insert_in_sweep_structure
			m_temp_edge_buffer.Add(prevEdge);
			AddEdgeToCluster(prevEdge, cluster_1);
			for (int i = 1, n = intersector.GetResultSegmentCount(intersector_index); i < n; i++)
			{
				// each
				// iteration
				// adds
				// new
				// Cluster
				// and
				// Edge.
				int newCluster = NewCluster_(-1);
				m_modified_clusters.Add(newCluster);
				m_temp_edge_buffer.Add(newCluster);
				AddEdgeToCluster(prevEdge, newCluster);
				int newEdge = NewEdge_(-1);
				m_edges_to_insert_in_sweep_structure.Add(newEdge);
				SetEdgeSweepNode_(newEdge, c_3);
				// mark that its in
				// m_edges_to_insert_in_sweep_structure
				m_temp_edge_buffer.Add(newEdge);
				AddEdgeToCluster(newEdge, newCluster);
				prevEdge = newEdge;
			}
			AddEdgeToCluster(prevEdge, cluster2);
			// set the Edit_shape vertices to the new clusters and edges.
			for (int vertexHandle = m_edge_vertices.GetFirst(edgeOrigins1); vertexHandle != -1; vertexHandle = m_edge_vertices.GetNext(vertexHandle))
			{
				int vertex = m_edge_vertices.GetElement(vertexHandle);
				int cluster = GetClusterFromVertex(vertex);
				if (cluster == cluster_1)
				{
					// connecting from cluster_1 to cluster2
					int i_1 = 0;
					do
					{
						if (i_1 > 0)
						{
							int c = m_temp_edge_buffer.Get(i_1 - 1);
							AddVertexToCluster_(c, vertex);
							if (GetClusterVertexIndex(c) == -1)
							{
								SetClusterVertexIndex_(c, m_shape.GetVertexIndex(vertex));
							}
						}
						int edge = m_temp_edge_buffer.Get(i_1);
						i_1 += 2;
						AddVertexToEdge_(edge, vertex);
						vertex = m_shape.GetNextVertex(vertex);
					}
					while (i_1 < m_temp_edge_buffer.Size());
					System.Diagnostics.Debug.Assert((GetClusterFromVertex(vertex) == cluster2));
				}
				else
				{
					// connecting from cluster2 to cluster_1
					System.Diagnostics.Debug.Assert((cluster == cluster2));
					int i_1 = m_temp_edge_buffer.Size() - 1;
					do
					{
						if (i_1 < m_temp_edge_buffer.Size() - 2)
						{
							int c = m_temp_edge_buffer.Get(i_1 + 1);
							AddVertexToCluster_(c, vertex);
							if (GetClusterVertexIndex(c) < 0)
							{
								SetClusterVertexIndex_(c, m_shape.GetVertexIndex(vertex));
							}
						}
						System.Diagnostics.Debug.Assert((i_1 % 2 == 0));
						int edge = m_temp_edge_buffer.Get(i_1);
						i_1 -= 2;
						AddVertexToEdge_(edge, vertex);
						vertex = m_shape.GetNextVertex(vertex);
					}
					while (i_1 >= 0);
					System.Diagnostics.Debug.Assert((GetClusterFromVertex(vertex) == cluster_1));
				}
			}
			// #ifdef _DEBUG_TOPO
			// for (int i = 0, j = 0, n =
			// intersector->get_result_segment_count(intersector_index); i < n; i++,
			// j+=2)
			// {
			// int edge = m_temp_edge_buffer.get(j);
			// dbg_check_edge_(edge);
			// }
			// #endif
			m_temp_edge_buffer.Clear(false);
		}

		// dbg_check_new_edges_array_();
		internal int GetVertexFromClusterIndex(int cluster)
		{
			int vertexList = GetClusterVertices(cluster);
			int vertex = m_cluster_vertices.GetFirstElement(vertexList);
			return vertex;
		}

		internal int GetClusterFromVertex(int vertex)
		{
			return m_shape.GetUserIndex(vertex, m_vertex_cluster_index);
		}

		internal sealed class QComparator : com.epl.geometry.Treap.Comparator
		{
			internal com.epl.geometry.EditShape m_shape;

			internal com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();

			internal com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();

			internal QComparator(com.epl.geometry.EditShape shape)
			{
				m_shape = shape;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int vertex, int node)
			{
				m_shape.GetXY(vertex, pt_1);
				int v_2 = treap.GetElement(node);
				m_shape.GetXY(v_2, pt_2);
				return pt_1.Compare(pt_2);
			}
		}

		internal sealed class QMonikerComparator : com.epl.geometry.Treap.MonikerComparator
		{
			internal com.epl.geometry.EditShape m_shape;

			internal com.epl.geometry.Point2D m_point = new com.epl.geometry.Point2D();

			internal com.epl.geometry.Point2D m_pt = new com.epl.geometry.Point2D();

			internal QMonikerComparator(com.epl.geometry.EditShape shape)
			{
				m_shape = shape;
			}

			internal void SetPoint(com.epl.geometry.Point2D pt)
			{
				m_point.SetCoords(pt);
			}

			internal override int Compare(com.epl.geometry.Treap treap, int node)
			{
				int v = treap.GetElement(node);
				m_shape.GetXY(v, m_pt);
				return m_point.Compare(m_pt);
			}
		}

		internal void ProcessSplitHelper1_(int index, int edge, com.epl.geometry.SegmentIntersector intersector)
		{
			int clusterStart = GetEdgeCluster(edge, 0);
			com.epl.geometry.Point2D ptClusterStart = new com.epl.geometry.Point2D();
			GetClusterXY(clusterStart, ptClusterStart);
			com.epl.geometry.Point2D ptClusterEnd = new com.epl.geometry.Point2D();
			int clusterEnd = GetEdgeCluster(edge, 1);
			GetClusterXY(clusterEnd, ptClusterEnd);
			// Collect all edges that are affected by the split and that are in the
			// sweep structure.
			int count = intersector.GetResultSegmentCount(index);
			com.epl.geometry.Segment seg = intersector.GetResultSegment(index, 0);
			com.epl.geometry.Point2D newStart = new com.epl.geometry.Point2D();
			seg.GetStartXY(newStart);
			if (!ptClusterStart.IsEqual(newStart))
			{
				if (!m_complications)
				{
					int res1 = ptClusterStart.Compare(m_sweep_point);
					int res2 = newStart.Compare(m_sweep_point);
					if (res1 * res2 < 0)
					{
						m_complications = true;
					}
				}
				// point is not yet have been processed
				// but moved before the sweep point,
				// this will require
				// repeating the cracking step and the sweep_vertical cannot
				// help here
				// This cluster's position needs to be changed
				GetAffectedEdges(clusterStart, m_temp_edge_buffer);
				m_modified_clusters.Add(clusterStart);
			}
			if (!m_complications && count > 1)
			{
				int dir = ptClusterStart.Compare(ptClusterEnd);
				com.epl.geometry.Point2D midPoint = seg.GetEndXY();
				if (ptClusterStart.Compare(midPoint) != dir || midPoint.Compare(ptClusterEnd) != dir)
				{
					// split segment
					// midpoint is
					// above the
					// sweep line.
					// Therefore the
					// part of the
					// segment
					m_complications = true;
				}
				else
				{
					if (midPoint.Compare(m_sweep_point) < 0)
					{
						// midpoint moved below sweepline.
						m_complications = true;
					}
				}
			}
			seg = intersector.GetResultSegment(index, count - 1);
			com.epl.geometry.Point2D newEnd = seg.GetEndXY();
			if (!ptClusterEnd.IsEqual(newEnd))
			{
				if (!m_complications)
				{
					int res1 = ptClusterEnd.Compare(m_sweep_point);
					int res2 = newEnd.Compare(m_sweep_point);
					if (res1 * res2 < 0)
					{
						m_complications = true;
					}
				}
				// point is not yet have been processed
				// but moved before the sweep point.
				// This cluster's position needs to be changed
				GetAffectedEdges(clusterEnd, m_temp_edge_buffer);
				m_modified_clusters.Add(clusterEnd);
			}
			m_temp_edge_buffer.Add(edge);
			// Delete all nodes from the sweep structure that are affected by the
			// change.
			for (int i = 0, n = m_temp_edge_buffer.Size(); i < n; i++)
			{
				int e = m_temp_edge_buffer.Get(i);
				int sweepNode = GetEdgeSweepNode(e);
				if (com.epl.geometry.StridedIndexTypeCollection.IsValidElement(sweepNode))
				{
					m_sweep_structure.DeleteNode(sweepNode, -1);
					SetEdgeSweepNode_(e, -1);
				}
				int c_3 = com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex3();
				if (e != edge && GetEdgeSweepNode(e) != c_3)
				{
					// c_3 means the edge is
					// already in the
					// m_edges_to_insert_in_sweep_structure
					m_edges_to_insert_in_sweep_structure.Add(e);
					SetEdgeSweepNode_(e, c_3);
				}
			}
			m_temp_edge_buffer.Clear(false);
		}

		internal bool CheckAndFixIntersection_(int leftSweepNode, int rightSweepNode)
		{
			int leftEdge = m_sweep_structure.GetElement(leftSweepNode);
			m_sweep_comparator.Compare(m_sweep_structure, leftEdge, rightSweepNode);
			if (m_sweep_comparator.IntersectionDetected())
			{
				m_sweep_comparator.ClearIntersectionDetectedFlag();
				FixIntersection_(leftSweepNode, rightSweepNode);
				return true;
			}
			return false;
		}

		internal void FixIntersection_(int left, int right)
		{
			m_b_cracked = true;
			int edge1 = m_sweep_structure.GetElement(left);
			int edge2 = m_sweep_structure.GetElement(right);
			System.Diagnostics.Debug.Assert((edge1 != edge2));
			com.epl.geometry.Segment seg_1;
			com.epl.geometry.Segment seg_2;
			int vertexList1 = GetEdgeOriginVertices(edge1);
			int origin1 = m_edge_vertices.GetFirstElement(vertexList1);
			int vertexList2 = GetEdgeOriginVertices(edge2);
			int origin2 = m_edge_vertices.GetFirstElement(vertexList2);
			seg_1 = m_shape.GetSegment(origin1);
			if (seg_1 == null)
			{
				if (m_line_1 == null)
				{
					m_line_1 = new com.epl.geometry.Line();
				}
				m_shape.QueryLineConnector(origin1, m_line_1);
				seg_1 = m_line_1;
			}
			seg_2 = m_shape.GetSegment(origin2);
			if (seg_2 == null)
			{
				if (m_line_2 == null)
				{
					m_line_2 = new com.epl.geometry.Line();
				}
				m_shape.QueryLineConnector(origin2, m_line_2);
				seg_2 = m_line_2;
			}
			// #ifdef _DEBUG_CRACKING_REPORT
			// {
			// Point_2D pt11, pt12, pt21, pt22;
			// pt11 = seg_1->get_start_xy();
			// pt12 = seg_1->get_end_xy();
			// pt21 = seg_2->get_start_xy();
			// pt22 = seg_2->get_end_xy();
			// DEBUGPRINTF(L"Intersecting %d (%0.4f, %0.4f - %0.4f, %0.4f) and %d (%0.4f, %0.4f - %0.4f, %0.4f)\n",
			// edge1, pt11.x, pt11.y, pt12.x, pt12.y, edge2, pt21.x, pt21.y, pt22.x,
			// pt22.y);
			// }
			// #endif
			m_segment_intersector.PushSegment(seg_1);
			m_segment_intersector.PushSegment(seg_2);
			if (m_segment_intersector.Intersect(m_tolerance, true))
			{
				m_complications = true;
			}
			SplitEdge_(edge1, edge2, -1, m_segment_intersector);
			m_segment_intersector.Clear();
		}

		internal void FixIntersectionPointSegment_(int cluster, int node)
		{
			m_b_cracked = true;
			int edge1 = m_sweep_structure.GetElement(node);
			com.epl.geometry.Segment seg_1;
			int vertexList1 = GetEdgeOriginVertices(edge1);
			int origin1 = m_edge_vertices.GetFirstElement(vertexList1);
			seg_1 = m_shape.GetSegment(origin1);
			if (seg_1 == null)
			{
				if (m_line_1 == null)
				{
					m_line_1 = new com.epl.geometry.Line();
				}
				m_shape.QueryLineConnector(origin1, m_line_1);
				seg_1 = m_line_1;
			}
			int clusterVertex = GetClusterFirstVertex(cluster);
			m_segment_intersector.PushSegment(seg_1);
			m_shape.QueryPoint(clusterVertex, m_helper_point);
			m_segment_intersector.Intersect(m_tolerance, m_helper_point, 0, 1.0, true);
			SplitEdge_(edge1, -1, cluster, m_segment_intersector);
			m_segment_intersector.Clear();
		}

		internal void InsertNewEdges_()
		{
			if (m_edges_to_insert_in_sweep_structure.Size() == 0)
			{
				return;
			}
			while (m_edges_to_insert_in_sweep_structure.Size() != 0)
			{
				if (m_edges_to_insert_in_sweep_structure.Size() > System.Math.Max((int)100, m_shape.GetTotalPointCount()))
				{
					System.Diagnostics.Debug.Assert((false));
					m_edges_to_insert_in_sweep_structure.Clear(false);
					m_complications = true;
					break;
				}
				// something strange going on here. bail out, forget about
				// these edges and continue with sweep line. We'll
				// iterate on the data one more time.
				int edge = m_edges_to_insert_in_sweep_structure.GetLast();
				m_edges_to_insert_in_sweep_structure.RemoveLast();
				System.Diagnostics.Debug.Assert((GetEdgeSweepNode(edge) == com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex3()));
				SetEdgeSweepNode_(edge, -1);
				int terminatingCluster = IsEdgeOnSweepLine_(edge);
				if (terminatingCluster != -1)
				{
					InsertNewEdgeToSweepStructure_(edge, terminatingCluster);
				}
				m_b_continuing_segment_chain_optimization = false;
			}
		}

		internal bool InsertNewEdgeToSweepStructure_(int edge, int terminatingCluster)
		{
			System.Diagnostics.Debug.Assert((GetEdgeSweepNode(edge) == -1));
			int newEdgeNode;
			if (m_b_continuing_segment_chain_optimization)
			{
				newEdgeNode = m_sweep_structure.AddElementAtPosition(m_prev_neighbour, m_next_neighbour, edge, true, true, -1);
				m_b_continuing_segment_chain_optimization = false;
			}
			else
			{
				newEdgeNode = m_sweep_structure.AddUniqueElement(edge, -1);
			}
			if (newEdgeNode == -1)
			{
				// a coinciding edge.
				int existingNode = m_sweep_structure.GetDuplicateElement(-1);
				int existingEdge = m_sweep_structure.GetElement(existingNode);
				MergeEdges_(existingEdge, edge);
				return false;
			}
			// Remember the sweep structure node in the edge.
			SetEdgeSweepNode_(edge, newEdgeNode);
			if (m_sweep_comparator.IntersectionDetected())
			{
				// The edge has been inserted into the sweep structure and an
				// intersection has beebn found. The edge will be split and removed.
				m_sweep_comparator.ClearIntersectionDetectedFlag();
				int intersectionNode = m_sweep_comparator.GetLastComparedNode();
				FixIntersection_(intersectionNode, newEdgeNode);
				return true;
			}
			// The edge has been inserted into the sweep structure without
			// problems (it does not intersect its neighbours)
			return false;
		}

		internal com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();

		internal com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();

		internal int IsEdgeOnSweepLine_(int edge)
		{
			int cluster_1 = GetEdgeCluster(edge, 0);
			int cluster2 = GetEdgeCluster(edge, 1);
			GetClusterXY(cluster_1, pt_1);
			GetClusterXY(cluster2, pt_2);
			if (com.epl.geometry.Point2D.SqrDistance(pt_1, pt_2) <= m_tolerance_sqr)
			{
				// avoid
				// degenerate
				// segments
				m_complications = true;
				return -1;
			}
			int cmp1 = pt_1.Compare(m_sweep_point);
			int cmp2 = pt_2.Compare(m_sweep_point);
			if (cmp1 <= 0 && cmp2 > 0)
			{
				return cluster2;
			}
			if (cmp2 <= 0 && cmp1 > 0)
			{
				return cluster_1;
			}
			return -1;
		}

		// void set_edit_shape(Edit_shape* shape);
		// Fills the event queue and merges coincident clusters.
		internal void FillEventQueue()
		{
			com.epl.geometry.AttributeStreamOfInt32 event_q = new com.epl.geometry.AttributeStreamOfInt32(0);
			event_q.Reserve(m_shape.GetTotalPointCount());
			// temporary structure to
			// sort and find
			// clusters
			com.epl.geometry.EditShape.VertexIterator iter = m_shape.QueryVertexIterator();
			for (int vert = iter.Next(); vert != -1; vert = iter.Next())
			{
				if (m_shape.GetUserIndex(vert, m_vertex_cluster_index) != -1)
				{
					event_q.Add(vert);
				}
			}
			// Now we can merge coincident clusters and form the envent structure.
			// sort vertices lexicographically.
			m_shape.SortVerticesSimpleByY_(event_q, 0, event_q.Size());
			// The m_event_q is the event structure for the planesweep algorithm.
			// We could use any data structure that allows log(n) insertion and
			// deletion in the sorted order and
			// allow to iterate through in the sorted order.
			m_event_q.Clear();
			// Populate the event structure
			m_event_q.SetCapacity(event_q.Size());
			{
				// The comparator is used to sort vertices by the m_event_q
				m_event_q.SetComparator(new com.epl.geometry.PlaneSweepCrackerHelper.QComparator(m_shape));
			}
			// create the vertex clusters and fill the event structure m_event_q.
			// Because most vertices are expected to be non clustered, we create
			// clusters only for actual clusters to save some memory.
			com.epl.geometry.Point2D cluster_pt = new com.epl.geometry.Point2D();
			cluster_pt.SetNaN();
			int cluster = -1;
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int index = 0, nvertex = event_q.Size(); index < nvertex; index++)
			{
				int vertex = event_q.Get(index);
				m_shape.GetXY(vertex, pt);
				if (pt.IsEqual(cluster_pt))
				{
					int vertexCluster = m_shape.GetUserIndex(vertex, m_vertex_cluster_index);
					MergeClusters_(cluster, vertexCluster);
					continue;
				}
				cluster = GetClusterFromVertex(vertex);
				// add a vertex to the event queue
				m_shape.GetXY(vertex, cluster_pt);
				int eventQnode = m_event_q.AddBiggestElement(vertex, -1);
				// this
				// method
				// does
				// not
				// call
				// comparator's
				// compare,
				// assuming
				// sorted
				// order.
				SetClusterEventQNode_(cluster, eventQnode);
			}
		}

		internal void FillEventQueuePass2()
		{
			com.epl.geometry.AttributeStreamOfInt32 event_q = new com.epl.geometry.AttributeStreamOfInt32(0);
			event_q.Reserve(m_shape.GetTotalPointCount());
			// temporary structure to
			// sort and find
			// clusters
			for (int node = m_event_q.GetFirst(-1); node != -1; node = m_event_q.GetNext(node))
			{
				int v = m_event_q.GetElement(node);
				event_q.Add(v);
			}
			System.Diagnostics.Debug.Assert((event_q.Size() == m_event_q.Size(-1)));
			m_event_q.Clear();
			// sort vertices lexicographically.
			m_shape.SortVerticesSimpleByY_(event_q, 0, event_q.Size());
			for (int index = 0, nvertex = event_q.Size(); index < nvertex; index++)
			{
				int vertex = event_q.Get(index);
				int cluster = GetClusterFromVertex(vertex);
				int eventQnode = m_event_q.AddBiggestElement(vertex, -1);
				// this
				// method
				// does
				// not
				// call
				// comparator's
				// compare,
				// assuming
				// sorted
				// order.
				SetClusterEventQNode_(cluster, eventQnode);
			}
		}

		// Returns edges already in the sweep structure that are affected by the
		// change of cluster coordinate.
		internal void GetAffectedEdges(int cluster, com.epl.geometry.AttributeStreamOfInt32 edges)
		{
			int first_edge = GetClusterFirstEdge(cluster);
			if (first_edge == -1)
			{
				return;
			}
			int edge = first_edge;
			do
			{
				int sweepNode = GetEdgeSweepNode(edge);
				if (com.epl.geometry.StridedIndexTypeCollection.IsValidElement(sweepNode))
				{
					edges.Add(edge);
				}
				edge = GetNextEdge(edge, cluster);
			}
			while (edge != first_edge);
		}

		// Updates all vertices of the cluster to new coordinate
		internal void UpdateClusterXY(int cluster, com.epl.geometry.Point2D pt)
		{
			int vertexList = GetClusterVertices(cluster);
			for (int vh = m_cluster_vertices.GetFirst(vertexList); vh != -1; vh = m_cluster_vertices.GetNext(vh))
			{
				int vertex = m_cluster_vertices.GetElement(vh);
				m_shape.SetXY(vertex, pt);
			}
		}

		// Modifies the given edges given the intersector class and the result
		// index.
		// The function updates the the event structure and puts new edges into the
		// m_edges_to_insert_in_sweep_structure.
		internal void SplitEdge_(int edge1, int edge2, int intersectionCluster, com.epl.geometry.SegmentIntersector intersector)
		{
			DisconnectEdge_(edge1);
			// disconnects the edge from the clusters. The
			// edge still remembers the clusters.
			if (edge2 != -1)
			{
				DisconnectEdge_(edge2);
			}
			// disconnects the edge from the clusters.
			// The edge still remembers the clusters.
			// Collect all edges that are affected when the clusters change position
			// due to snapping
			// The edges are collected in m_edges_to_insert_in_sweep_structure.
			// Collect the modified clusters in m_modified_clusters.
			ProcessSplitHelper1_(0, edge1, intersector);
			if (edge2 != -1)
			{
				ProcessSplitHelper1_(1, edge2, intersector);
			}
			if (intersectionCluster != -1)
			{
				intersector.GetResultPoint().GetXY(pt_1);
				GetClusterXY(intersectionCluster, pt_2);
				if (!pt_2.IsEqual(pt_1))
				{
					m_modified_clusters.Add(intersectionCluster);
				}
			}
			// remove modified clusters from the event queue. We'll reincert them
			// later
			for (int i = 0, n = m_modified_clusters.Size(); i < n; i++)
			{
				int cluster = m_modified_clusters.Get(i);
				int eventQnode = GetClusterEventQNode(cluster);
				if (eventQnode != -1)
				{
					m_event_q.DeleteNode(eventQnode, -1);
					SetClusterEventQNode_(cluster, -1);
				}
			}
			int edgeOrigins1 = GetEdgeOriginVertices(edge1);
			int edgeOrigins2 = (edge2 != -1) ? GetEdgeOriginVertices(edge2) : -1;
			// Adjust the vertex coordinates and split the segments in the the edit
			// shape.
			ApplyIntersectorToEditShape_(edgeOrigins1, intersector, 0);
			if (edge2 != -1)
			{
				ApplyIntersectorToEditShape_(edgeOrigins2, intersector, 1);
			}
			// Produce clusters, and new edges. The new edges are added to
			// m_edges_to_insert_in_sweep_structure.
			CreateEdgesAndClustersFromSplitEdge_(edge1, intersector, 0);
			if (edge2 != -1)
			{
				CreateEdgesAndClustersFromSplitEdge_(edge2, intersector, 1);
			}
			m_edge_vertices.DeleteList(edgeOrigins1);
			DeleteEdge_(edge1);
			if (edge2 != -1)
			{
				m_edge_vertices.DeleteList(edgeOrigins2);
				DeleteEdge_(edge2);
			}
			// insert clusters into the event queue and the edges into the sweep
			// structure.
			for (int i_1 = 0, n = m_modified_clusters.Size(); i_1 < n; i_1++)
			{
				int cluster = m_modified_clusters.Get(i_1);
				if (cluster == m_sweep_point_cluster)
				{
					m_b_sweep_point_cluster_was_modified = true;
				}
				int eventQnode = GetClusterEventQNode(cluster);
				if (eventQnode == -1)
				{
					int vertex = GetClusterFirstVertex(cluster);
					System.Diagnostics.Debug.Assert((GetClusterFromVertex(vertex) == cluster));
					eventQnode = m_event_q.AddUniqueElement(vertex, -1);
					// O(logN)
					// operation
					if (eventQnode == -1)
					{
						// the cluster is coinciding with another
						// one. merge.
						int existingNode = m_event_q.GetDuplicateElement(-1);
						int v = m_event_q.GetElement(existingNode);
						System.Diagnostics.Debug.Assert((m_shape.IsEqualXY(vertex, v)));
						int existingCluster = GetClusterFromVertex(v);
						MergeClusters_(existingCluster, cluster);
					}
					else
					{
						SetClusterEventQNode_(cluster, eventQnode);
					}
				}
			}
			// if already inserted (probably impossible) case
			m_modified_clusters.Clear(false);
		}

		// Returns a cluster's xy.
		internal void GetClusterXY(int cluster, com.epl.geometry.Point2D ptOut)
		{
			int vindex = GetClusterVertexIndex(cluster);
			m_shape.GetXYWithIndex(vindex, ptOut);
		}

		internal int GetClusterFirstVertex(int cluster)
		{
			int vertexList = GetClusterVertices(cluster);
			int vertex = m_cluster_vertices.GetFirstElement(vertexList);
			return vertex;
		}

		internal bool SweepImpl_()
		{
			m_b_sweep_point_cluster_was_modified = false;
			m_sweep_point_cluster = -1;
			if (m_sweep_comparator == null)
			{
				m_sweep_structure.DisableBalancing();
				m_sweep_comparator = new com.epl.geometry.PlaneSweepCrackerHelper.SimplifySweepComparator(this);
				m_sweep_structure.SetComparator(m_sweep_comparator);
			}
			com.epl.geometry.AttributeStreamOfInt32 edgesToDelete = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.PlaneSweepCrackerHelper.SimplifySweepMonikerComparator sweepMoniker = null;
			com.epl.geometry.PlaneSweepCrackerHelper.QMonikerComparator moniker = null;
			int iterationCounter = 0;
			m_prev_neighbour = -1;
			m_next_neighbour = -1;
			m_b_continuing_segment_chain_optimization = false;
			int c_2 = com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2();
			int c_3 = com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex3();
			System.Diagnostics.Debug.Assert((c_2 != c_3));
			for (int eventQnode = m_event_q.GetFirst(-1); eventQnode != -1; )
			{
				iterationCounter++;
				m_b_continuing_segment_chain_optimization = false;
				int vertex = m_event_q.GetElement(eventQnode);
				m_sweep_point_cluster = GetClusterFromVertex(vertex);
				m_shape.GetXY(vertex, m_sweep_point);
				m_sweep_comparator.SetSweepY(m_sweep_point.y, m_sweep_point.x);
				// move
				// the
				// sweep
				// line
				bool bDisconnectedCluster = false;
				{
					// scope
					int first_edge = GetClusterFirstEdge(m_sweep_point_cluster);
					bDisconnectedCluster = first_edge == -1;
					if (!bDisconnectedCluster)
					{
						int edge = first_edge;
						do
						{
							int sweepNode = GetEdgeSweepNode(edge);
							if (sweepNode == -1)
							{
								m_edges_to_insert_in_sweep_structure.Add(edge);
								SetEdgeSweepNode_(edge, c_3);
							}
							else
							{
								// mark that its in
								// m_edges_to_insert_in_sweep_structure
								if (sweepNode != c_3)
								{
									System.Diagnostics.Debug.Assert((com.epl.geometry.StridedIndexTypeCollection.IsValidElement(sweepNode)));
									edgesToDelete.Add(sweepNode);
								}
							}
							edge = GetNextEdge(edge, m_sweep_point_cluster);
						}
						while (edge != first_edge);
					}
				}
				// st_counter_insertions_peaks += edgesToDelete.size() == 0 &&
				// m_edges_to_insert_in_sweep_structure.size() > 0;
				// First step is to delete the edges that terminate in the
				// cluster.
				// During that step we also determine the left and right neighbors
				// of the deleted bunch and then check if those left and right
				// intersect or not.
				if (edgesToDelete.Size() > 0)
				{
					m_b_continuing_segment_chain_optimization = (edgesToDelete.Size() == 1 && m_edges_to_insert_in_sweep_structure.Size() == 1);
					// Mark nodes that need to be deleted by setting c_2 to the
					// edge's sweep node member.
					for (int i = 0, n = edgesToDelete.Size(); i < n; i++)
					{
						int edge = m_sweep_structure.GetElement(edgesToDelete.Get(i));
						SetEdgeSweepNode_(edge, c_2);
					}
					int left = c_2;
					int right = c_2;
					// Determine left and right nodes for the bunch of nodes we are
					// deleting.
					for (int i_1 = 0, n = edgesToDelete.Size(); i_1 < n; i_1++)
					{
						int sweepNode = edgesToDelete.Get(i_1);
						if (left == c_2)
						{
							int localleft = m_sweep_structure.GetPrev(sweepNode);
							if (localleft != -1)
							{
								int edge = m_sweep_structure.GetElement(localleft);
								int node = GetEdgeSweepNode(edge);
								if (node != c_2)
								{
									left = localleft;
								}
							}
							else
							{
								left = -1;
							}
						}
						if (right == c_2)
						{
							int localright = m_sweep_structure.GetNext(sweepNode);
							if (localright != -1)
							{
								int edge = m_sweep_structure.GetElement(localright);
								int node = GetEdgeSweepNode(edge);
								if (node != c_2)
								{
									right = localright;
								}
							}
							else
							{
								right = -1;
							}
						}
						if (left != c_2 && right != c_2)
						{
							break;
						}
					}
					System.Diagnostics.Debug.Assert((left != c_2 && right != c_2));
					// Now delete the bunch.
					for (int i_2 = 0, n = edgesToDelete.Size(); i_2 < n; i_2++)
					{
						int sweepNode = edgesToDelete.Get(i_2);
						int edge = m_sweep_structure.GetElement(sweepNode);
						m_sweep_structure.DeleteNode(sweepNode, -1);
						SetEdgeSweepNode_(edge, -1);
					}
					edgesToDelete.Clear(false);
					m_prev_neighbour = left != -1 ? left : -1;
					m_next_neighbour = right != -1 ? right : -1;
					// Now check if the left and right we found intersect or not.
					if (left != -1 && right != -1)
					{
						if (!m_b_continuing_segment_chain_optimization)
						{
							bool bIntersected = CheckAndFixIntersection_(left, right);
						}
					}
					else
					{
						if ((left == -1) && (right == -1))
						{
							m_b_continuing_segment_chain_optimization = false;
						}
					}
				}
				else
				{
					// edgesToDelete.size() == 0 - nothing to delete here. This is a
					// cluster which has all edges directed up or a disconnected
					// cluster.
					if (bDisconnectedCluster)
					{
						// check standalone cluster (point or
						// multipoint) if it cracks an edge.
						if (sweepMoniker == null)
						{
							sweepMoniker = new com.epl.geometry.PlaneSweepCrackerHelper.SimplifySweepMonikerComparator(this);
						}
						sweepMoniker.SetPoint(m_sweep_point);
						m_sweep_structure.SearchUpperBound(sweepMoniker, -1);
						if (sweepMoniker.IntersectionDetected())
						{
							sweepMoniker.ClearIntersectionDetectedFlag();
							FixIntersectionPointSegment_(m_sweep_point_cluster, sweepMoniker.GetCurrentNode());
						}
					}
				}
				// Now insert edges that start at the cluster and go up
				InsertNewEdges_();
				if (m_b_sweep_point_cluster_was_modified)
				{
					m_b_sweep_point_cluster_was_modified = false;
					if (moniker == null)
					{
						moniker = new com.epl.geometry.PlaneSweepCrackerHelper.QMonikerComparator(m_shape);
					}
					moniker.SetPoint(m_sweep_point);
					eventQnode = m_event_q.SearchUpperBound(moniker, -1);
				}
				else
				{
					eventQnode = m_event_q.GetNext(eventQnode);
				}
			}
			return m_b_cracked;
		}

		internal void SetEditShape_(com.epl.geometry.EditShape shape)
		{
			// Populate the cluster and edge structures.
			m_shape = shape;
			m_vertex_cluster_index = m_shape.CreateUserIndex();
			m_edges.SetCapacity(shape.GetTotalPointCount() + 32);
			m_clusters.SetCapacity(shape.GetTotalPointCount());
			m_cluster_vertices.ReserveLists(shape.GetTotalPointCount());
			m_cluster_vertices.ReserveNodes(shape.GetTotalPointCount());
			m_edge_vertices.ReserveLists(shape.GetTotalPointCount() + 32);
			m_edge_vertices.ReserveNodes(shape.GetTotalPointCount() + 32);
			for (int geometry = m_shape.GetFirstGeometry(); geometry != -1; geometry = m_shape.GetNextGeometry(geometry))
			{
				bool bMultiPath = com.epl.geometry.Geometry.IsMultiPath(m_shape.GetGeometryType(geometry));
				if (!bMultiPath)
				{
					// for multipoints do not add edges.
					System.Diagnostics.Debug.Assert((m_shape.GetGeometryType(geometry) == com.epl.geometry.Geometry.GeometryType.MultiPoint));
					for (int path = m_shape.GetFirstPath(geometry); path != -1; path = m_shape.GetNextPath(path))
					{
						int vertex = m_shape.GetFirstVertex(path);
						for (int i = 0, n = m_shape.GetPathSize(path); i < n; i++)
						{
							// int cluster
							NewCluster_(vertex);
							vertex = m_shape.GetNextVertex(vertex);
						}
					}
					continue;
				}
				for (int path_1 = m_shape.GetFirstPath(geometry); path_1 != -1; path_1 = m_shape.GetNextPath(path_1))
				{
					int path_size = m_shape.GetPathSize(path_1);
					System.Diagnostics.Debug.Assert((path_size > 1));
					int first_vertex = m_shape.GetFirstVertex(path_1);
					// first------------------
					int firstCluster = NewCluster_(first_vertex);
					int first_edge = NewEdge_(first_vertex);
					AddEdgeToCluster(first_edge, firstCluster);
					int prevEdge = first_edge;
					int vertex = m_shape.GetNextVertex(first_vertex);
					for (int index = 0, n = path_size - 2; index < n; index++)
					{
						int nextvertex = m_shape.GetNextVertex(vertex);
						// ------------x------------
						int cluster = NewCluster_(vertex);
						AddEdgeToCluster(prevEdge, cluster);
						int newEdge = NewEdge_(vertex);
						AddEdgeToCluster(newEdge, cluster);
						prevEdge = newEdge;
						vertex = nextvertex;
					}
					// ------------------lastx
					if (m_shape.IsClosedPath(path_1))
					{
						int cluster = NewCluster_(vertex);
						AddEdgeToCluster(prevEdge, cluster);
						// close the path
						// lastx------------------firstx
						int newEdge = NewEdge_(vertex);
						AddEdgeToCluster(newEdge, cluster);
						AddEdgeToCluster(newEdge, firstCluster);
					}
					else
					{
						// ------------------lastx
						int cluster = NewCluster_(vertex);
						AddEdgeToCluster(prevEdge, cluster);
					}
				}
			}
			FillEventQueue();
		}
		// int perPoint = estimate_memory_size() /
		// m_shape.get_total_point_count();
		// perPoint = 0;
	}
}
