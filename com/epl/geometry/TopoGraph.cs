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
	internal sealed class TopoGraph
	{
		internal abstract class EnumInputMode
		{
			public const int enumInputModeBuildGraph = 0;

			public const int enumInputModeSimplifyAlternate = 4 + 0;

			public const int enumInputModeSimplifyWinding = 4 + 1;

			public const int enumInputModeIsSimplePolygon = 4 + 3;
		}

		internal static class EnumInputModeConstants
		{
		}

		internal com.epl.geometry.EditShape m_shape;

		internal com.epl.geometry.StridedIndexTypeCollection m_clusterData;

		internal com.epl.geometry.StridedIndexTypeCollection m_clusterVertices;

		internal int m_firstCluster;

		internal int m_lastCluster;

		internal com.epl.geometry.StridedIndexTypeCollection m_halfEdgeData;

		internal com.epl.geometry.StridedIndexTypeCollection m_chainData;

		internal com.epl.geometry.AttributeStreamOfDbl m_chainAreas;

		internal com.epl.geometry.AttributeStreamOfDbl m_chainPerimeters;

		internal readonly int c_edgeParentageMask;

		internal readonly int c_edgeBitMask;

		internal int m_universeChain;

		internal System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32> m_edgeIndices;

		internal System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32> m_clusterIndices;

		internal System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32> m_chainIndices;

		internal int m_geometryIDIndex;

		internal int m_clusterIndex;

		internal int m_halfEdgeIndex;

		internal int m_tmpHalfEdgeParentageIndex;

		internal int m_tmpHalfEdgeWindingNumberIndex;

		internal int m_tmpHalfEdgeOddEvenNumberIndex = -1;

		internal int m_universe_geomID = -1;

		internal bool m_buildChains = true;

		private bool m_dirty_check_failed = false;

		private double m_check_dirty_planesweep_tolerance = double.NaN;

		// cluster data: index, parentage, halfEdge, globalPrev, globalNext
		// edge data: index, origin, faceParentage, edgeParentage, twin, prev, next
		// chain data index, half_edge, parentage, parentChain, firstIsland,
		// nextInParent, prev, next
		// index of geometryIDs in the m_shape
		// vertex index of cluster handles in the m_shape
		// vertex index of half-edges in the m_shape
		internal void Check_dirty_planesweep(double tolerance)
		{
			m_check_dirty_planesweep_tolerance = tolerance;
		}

		internal bool Dirty_check_failed()
		{
			return m_dirty_check_failed;
		}

		internal com.epl.geometry.NonSimpleResult m_non_simple_result = new com.epl.geometry.NonSimpleResult();

		internal int m_pointCount;

		internal sealed class PlaneSweepComparator : com.epl.geometry.Treap.Comparator
		{
			internal com.epl.geometry.TopoGraph m_helper;

			internal com.epl.geometry.SegmentBuffer m_buffer_left;

			internal com.epl.geometry.SegmentBuffer m_buffer_right;

			internal com.epl.geometry.Envelope1D interval_left;

			internal com.epl.geometry.Envelope1D interval_right;

			internal double m_y_scanline;

			internal PlaneSweepComparator(com.epl.geometry.TopoGraph helper)
			{
				// point count processed in this Topo_graph. Used to
				// reserve data.
				m_helper = helper;
				m_y_scanline = com.epl.geometry.NumberUtils.TheNaN;
				m_buffer_left = new com.epl.geometry.SegmentBuffer();
				m_buffer_right = new com.epl.geometry.SegmentBuffer();
				interval_left = new com.epl.geometry.Envelope1D();
				interval_right = new com.epl.geometry.Envelope1D();
			}

			internal override int Compare(com.epl.geometry.Treap treap, int left, int node)
			{
				int right = treap.GetElement(node);
				// can be sped up a little, because left or right stay the same
				// while an edge is inserted into the tree.
				m_helper.QuerySegmentXY(left, m_buffer_left);
				m_helper.QuerySegmentXY(right, m_buffer_right);
				com.epl.geometry.Segment segLeft = m_buffer_left.Get();
				com.epl.geometry.Segment segRight = m_buffer_right.Get();
				// Prerequisite: The segments have the start point lexicographically
				// above the end point.
				System.Diagnostics.Debug.Assert((segLeft.GetStartXY().Compare(segLeft.GetEndXY()) < 0));
				System.Diagnostics.Debug.Assert((segRight.GetStartXY().Compare(segRight.GetEndXY()) < 0));
				// Simple test for faraway segments
				interval_left.SetCoords(segLeft.GetStartX(), segLeft.GetEndX());
				interval_right.SetCoords(segRight.GetStartX(), segRight.GetEndX());
				if (interval_left.vmax < interval_right.vmin)
				{
					return -1;
				}
				if (interval_left.vmin > interval_right.vmax)
				{
					return 1;
				}
				bool bLeftHorz = segLeft.GetStartY() == segLeft.GetEndY();
				bool bRightHorz = segRight.GetStartY() == segRight.GetEndY();
				if (bLeftHorz || bRightHorz)
				{
					if (bLeftHorz && bRightHorz)
					{
						System.Diagnostics.Debug.Assert((interval_left.Equals(interval_right)));
						return 0;
					}
					// left segment is horizontal. The right one is not.
					// Prerequisite of this algorithm is that this can only happen
					// when:
					// left
					// |right -------------------- end == end
					// | |
					// | left |
					// -------------------- right |
					// start == start
					// or:
					// right segment is horizontal. The left one is not.
					// Prerequisite of this algorithm is that his can only happen
					// when:
					// right
					// |left -------------------- end == end
					// | |
					// | right |
					// -------------------- left |
					// start == start
					if (segLeft.GetStartY() == segRight.GetStartY() && segLeft.GetStartX() == segRight.GetStartX())
					{
						return bLeftHorz ? 1 : -1;
					}
					else
					{
						if (segLeft.GetEndY() == segRight.GetEndY() && segLeft.GetEndX() == segRight.GetEndX())
						{
							return bLeftHorz ? -1 : 1;
						}
					}
				}
				// Now do actual intersections
				double xLeft = segLeft.IntersectionOfYMonotonicWithAxisX(m_y_scanline, interval_left.vmin);
				double xRight = segRight.IntersectionOfYMonotonicWithAxisX(m_y_scanline, interval_right.vmin);
				if (xLeft == xRight)
				{
					// apparently these edges originate from same vertex and the
					// scanline is on the vertex. move scanline a little.
					double yLeft = segLeft.GetEndY();
					double yRight = segRight.GetEndY();
					double miny = System.Math.Min(yLeft, yRight);
					double y = (miny + m_y_scanline) * 0.5;
					if (y == m_y_scanline)
					{
						// assert(0);//ST: not a bug. just curious to see this
						// happens.
						y = miny;
					}
					// apparently, one of the segments is almost
					// horizontal line.
					xLeft = segLeft.IntersectionOfYMonotonicWithAxisX(y, interval_left.vmin);
					xRight = segRight.IntersectionOfYMonotonicWithAxisX(y, interval_right.vmin);
				}
				return xLeft < xRight ? -1 : (xLeft > xRight ? 1 : 0);
			}

			internal void SetY(double y)
			{
				m_y_scanline = y;
			}
			// void operator=(const Plane_sweep_comparator&); // do not allow
			// operator =
		}

		internal sealed class TopoGraphAngleComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.TopoGraph m_parent;

			internal TopoGraphAngleComparer(com.epl.geometry.TopoGraph parent_)
			{
				m_parent = parent_;
			}

			public override int Compare(int v1, int v2)
			{
				return m_parent.CompareEdgeAngles_(v1, v2);
			}
		}

		internal sealed class ClusterSweepMonikerComparator : com.epl.geometry.Treap.MonikerComparator
		{
			internal com.epl.geometry.TopoGraph m_parent;

			internal com.epl.geometry.SegmentBuffer m_segment_buffer;

			internal com.epl.geometry.Point2D m_point;

			internal com.epl.geometry.Envelope1D m_interval;

			internal ClusterSweepMonikerComparator(com.epl.geometry.TopoGraph parent)
			{
				m_parent = parent;
				m_segment_buffer = new com.epl.geometry.SegmentBuffer();
				m_point = new com.epl.geometry.Point2D();
				m_interval = new com.epl.geometry.Envelope1D();
			}

			internal void SetPointXY(com.epl.geometry.Point2D pt)
			{
				m_point.SetCoords(pt);
			}

			internal override int Compare(com.epl.geometry.Treap treap, int node)
			{
				int half_edge = treap.GetElement(node);
				// can be sped up a little, because left or right stay the same
				// while an edge is inserted into the tree.
				m_parent.QuerySegmentXY(half_edge, m_segment_buffer);
				com.epl.geometry.Segment seg = m_segment_buffer.Get();
				// Simple test for faraway segments
				m_interval.SetCoords(seg.GetStartX(), seg.GetEndX());
				if (m_point.x < m_interval.vmin)
				{
					return -1;
				}
				if (m_point.x > m_interval.vmax)
				{
					return 1;
				}
				// Now do actual intersections
				double x = seg.IntersectionOfYMonotonicWithAxisX(m_point.y, m_point.x);
				System.Diagnostics.Debug.Assert((x != m_point.x));
				return m_point.x < x ? -1 : (m_point.x > x ? 1 : 0);
			}
		}

		internal int NewCluster_()
		{
			if (m_clusterData == null)
			{
				m_clusterData = new com.epl.geometry.StridedIndexTypeCollection(8);
			}
			int cluster = m_clusterData.NewElement();
			// m_clusterData->add(-1);//first vertex
			m_clusterData.SetField(cluster, 1, 0);
			// parentage
			// m_clusterData->add(-1);//first half edge
			// m_clusterData->add(-1);//prev cluster
			// m_clusterData->add(-1);//next cluster
			return cluster;
		}

		internal int NewHalfEdgePair_()
		{
			if (m_halfEdgeData == null)
			{
				m_halfEdgeData = new com.epl.geometry.StridedIndexTypeCollection(8);
			}
			int halfEdge = m_halfEdgeData.NewElement();
			// m_halfEdgeData.add(-1);//origin cluster
			m_halfEdgeData.SetField(halfEdge, 2, 0);
			// chain parentage
			m_halfEdgeData.SetField(halfEdge, 3, 0);
			// edge parentage
			// m_halfEdgeData.add(-1);//twin
			// m_halfEdgeData.add(-1);//prev
			// m_halfEdgeData.add(-1);//next
			int twinHalfEdge = m_halfEdgeData.NewElement();
			// m_halfEdgeData.add(-1);//origin cluster
			m_halfEdgeData.SetField(twinHalfEdge, 2, 0);
			// chain parentage
			m_halfEdgeData.SetField(twinHalfEdge, 3, 0);
			// edge parentage
			// m_halfEdgeData.add(-1);//twin
			// m_halfEdgeData.add(-1);//prev
			// m_halfEdgeData.add(-1);//next
			SetHalfEdgeTwin_(halfEdge, twinHalfEdge);
			SetHalfEdgeTwin_(twinHalfEdge, halfEdge);
			return halfEdge;
		}

		internal int NewChain_()
		{
			if (m_chainData == null)
			{
				m_chainData = new com.epl.geometry.StridedIndexTypeCollection(8);
			}
			int chain = m_chainData.NewElement();
			// m_chainData->write(chain, + 1, -1);//half_edge
			m_chainData.SetField(chain, 2, 0);
			// parentage (geometric)
			// m_chainData->write(m_chainReserved + 3, -1);//parent chain
			// m_chainData->write(m_chainReserved + 4, -1);//firstIsland
			// m_chainData->write(m_chainReserved + 5, -1);//nextInParent
			// m_chainData->write(m_chainReserved + 6, -1);//prev
			// m_chainData->write(m_chainReserved + 7, -1);//next
			// m_chainReserved += 8;
			return chain;
		}

		internal int DeleteChain_(int chain)
		{
			// Note: this method cannot be after _PlaneSweep
			System.Diagnostics.Debug.Assert((m_universeChain != chain));
			int n = GetChainNext(chain);
			m_chainData.DeleteElement(chain);
			// Note: no need to update the first chain, because one should never try
			// deleting the first (the universe) chain.
			return n;
		}

		internal int GetClusterIndex_(int cluster)
		{
			return m_clusterData.ElementToIndex(cluster);
		}

		internal void SetClusterVertexIterator_(int cluster, int verticeList)
		{
			m_clusterData.SetField(cluster, 7, verticeList);
		}

		internal void SetClusterHalfEdge_(int cluster, int half_edge)
		{
			m_clusterData.SetField(cluster, 2, half_edge);
		}

		internal void SetClusterParentage_(int cluster, int parentage)
		{
			m_clusterData.SetField(cluster, 1, parentage);
		}

		internal void SetPrevCluster_(int cluster, int nextCluster)
		{
			m_clusterData.SetField(cluster, 3, nextCluster);
		}

		internal void SetNextCluster_(int cluster, int nextCluster)
		{
			m_clusterData.SetField(cluster, 4, nextCluster);
		}

		internal void SetClusterVertexIndex_(int cluster, int index)
		{
			m_clusterData.SetField(cluster, 5, index);
		}

		internal int GetClusterVertexIndex_(int cluster)
		{
			return m_clusterData.GetField(cluster, 5);
		}

		internal void SetClusterChain_(int cluster, int chain)
		{
			m_clusterData.SetField(cluster, 6, chain);
		}

		internal void AddClusterToExteriorChain_(int chain, int cluster)
		{
			System.Diagnostics.Debug.Assert((GetClusterChain(cluster) == -1));
			SetClusterChain_(cluster, chain);
		}

		// There is no link from the chain to the cluster. Only vice versa.
		// Consider for change?
		internal int GetHalfEdgeIndex_(int he)
		{
			return m_halfEdgeData.ElementToIndex(he);
		}

		internal void SetHalfEdgeOrigin_(int half_edge, int cluster)
		{
			m_halfEdgeData.SetField(half_edge, 1, cluster);
		}

		internal void SetHalfEdgeTwin_(int half_edge, int twinHalfEdge)
		{
			m_halfEdgeData.SetField(half_edge, 4, twinHalfEdge);
		}

		internal void SetHalfEdgePrev_(int half_edge, int prevHalfEdge)
		{
			m_halfEdgeData.SetField(half_edge, 5, prevHalfEdge);
		}

		internal void SetHalfEdgeNext_(int half_edge, int nextHalfEdge)
		{
			m_halfEdgeData.SetField(half_edge, 6, nextHalfEdge);
		}

		// void set_half_edge_chain_parentage_(int half_edge, int
		// chainParentageMask) { m_halfEdgeData.setField(half_edge + 2,
		// chainParentageMask); }
		internal void SetHalfEdgeChain_(int half_edge, int chain)
		{
			m_halfEdgeData.SetField(half_edge, 2, chain);
		}

		internal void SetHalfEdgeParentage_(int half_edge, int parentageMask)
		{
			m_halfEdgeData.SetField(half_edge, 3, parentageMask);
		}

		internal int GetHalfEdgeParentageMask_(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 3);
		}

		internal void SetHalfEdgeVertexIterator_(int half_edge, int vertexIterator)
		{
			m_halfEdgeData.SetField(half_edge, 7, vertexIterator);
		}

		internal void UpdateVertexToHalfEdgeConnectionHelper_(int half_edge, bool bClear)
		{
			int viter = GetHalfEdgeVertexIterator(half_edge);
			if (viter != -1)
			{
				int he = bClear ? -1 : half_edge;
				for (int viter_ = GetHalfEdgeVertexIterator(half_edge); viter_ != -1; viter_ = IncrementVertexIterator(viter_))
				{
					int vertex = GetVertexFromVertexIterator(viter_);
					m_shape.SetUserIndex(vertex, m_halfEdgeIndex, he);
				}
			}
		}

		internal void UpdateVertexToHalfEdgeConnection_(int half_edge, bool bClear)
		{
			if (half_edge == -1)
			{
				return;
			}
			UpdateVertexToHalfEdgeConnectionHelper_(half_edge, bClear);
			UpdateVertexToHalfEdgeConnectionHelper_(GetHalfEdgeTwin(half_edge), bClear);
		}

		internal int GetChainIndex_(int chain)
		{
			return m_chainData.ElementToIndex(chain);
		}

		internal void SetChainHalfEdge_(int chain, int half_edge)
		{
			m_chainData.SetField(chain, 1, half_edge);
		}

		internal void SetChainParentage_(int chain, int parentage)
		{
			m_chainData.SetField(chain, 2, parentage);
		}

		internal void SetChainParent_(int chain, int parentChain)
		{
			System.Diagnostics.Debug.Assert((m_chainData.GetField(chain, 3) != parentChain));
			m_chainData.SetField(chain, 3, parentChain);
			int firstIsland = GetChainFirstIsland(parentChain);
			SetChainNextInParent_(chain, firstIsland);
			SetChainFirstIsland_(parentChain, chain);
		}

		internal void SetChainFirstIsland_(int chain, int islandChain)
		{
			m_chainData.SetField(chain, 4, islandChain);
		}

		internal void SetChainNextInParent_(int chain, int nextInParent)
		{
			m_chainData.SetField(chain, 5, nextInParent);
		}

		internal void SetChainPrev_(int chain, int prev)
		{
			m_chainData.SetField(chain, 6, prev);
		}

		internal void SetChainNext_(int chain, int next)
		{
			m_chainData.SetField(chain, 7, next);
		}

		internal void SetChainArea_(int chain, double area)
		{
			int chainIndex = GetChainIndex_(chain);
			m_chainAreas.Write(chainIndex, area);
		}

		internal void SetChainPerimeter_(int chain, double perimeter)
		{
			int chainIndex = GetChainIndex_(chain);
			m_chainPerimeters.Write(chainIndex, perimeter);
		}

		internal void UpdateChainAreaAndPerimeter_(int chain)
		{
			double area = 0;
			double perimeter = 0;
			int firstHalfEdge = GetChainHalfEdge(chain);
			com.epl.geometry.Point2D origin = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D from = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D to = new com.epl.geometry.Point2D();
			GetHalfEdgeFromXY(firstHalfEdge, origin);
			from.SetCoords(origin);
			int half_edge = firstHalfEdge;
			do
			{
				GetHalfEdgeToXY(half_edge, to);
				perimeter += com.epl.geometry.Point2D.Distance(from, to);
				int twinChain = GetHalfEdgeChain(GetHalfEdgeTwin(half_edge));
				if (twinChain != chain)
				{
					// only count edges are not dangling segments
					// of polylines
					area += ((to.x - origin.x) - (from.x - origin.x)) * ((to.y - origin.y) + (from.y - origin.y)) * 0.5;
				}
				from.SetCoords(to);
				half_edge = GetHalfEdgeNext(half_edge);
			}
			while (half_edge != firstHalfEdge);
			int ind = GetChainIndex_(chain);
			m_chainAreas.Write(ind, area);
			m_chainPerimeters.Write(ind, perimeter);
		}

		internal int GetChainTopMostEdge_(int chain)
		{
			int firstHalfEdge = GetChainHalfEdge(chain);
			com.epl.geometry.Point2D top = new com.epl.geometry.Point2D();
			GetHalfEdgeFromXY(firstHalfEdge, top);
			int topEdge = firstHalfEdge;
			com.epl.geometry.Point2D v = new com.epl.geometry.Point2D();
			int half_edge = firstHalfEdge;
			do
			{
				GetHalfEdgeFromXY(half_edge, v);
				if (v.Compare(top) > 0)
				{
					top.SetCoords(v);
					topEdge = half_edge;
				}
				half_edge = GetHalfEdgeNext(half_edge);
			}
			while (half_edge != firstHalfEdge);
			return topEdge;
		}

		internal void PlaneSweepParentage_(int inputMode, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.TopoGraph.PlaneSweepComparator comparator = new com.epl.geometry.TopoGraph.PlaneSweepComparator(this);
			com.epl.geometry.Treap aet = new com.epl.geometry.Treap();
			aet.SetCapacity(m_pointCount / 2);
			aet.SetComparator(comparator);
			com.epl.geometry.AttributeStreamOfInt32 new_edges = new com.epl.geometry.AttributeStreamOfInt32(0);
			int treeNodeIndex = CreateUserIndexForHalfEdges();
			com.epl.geometry.TopoGraph.ClusterSweepMonikerComparator clusterMoniker = null;
			int counter = 0;
			// Clusters are sorted by the y, x coordinate in ascending order.
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			// Each cluster is an event of the sweep-line algorithm.
			for (int cluster = GetFirstCluster(); cluster != -1; cluster = GetNextCluster(cluster))
			{
				counter++;
				if ((counter & unchecked((int)(0xFF))) == 0)
				{
					if ((progress_tracker != null) && !(progress_tracker.Progress(-1, -1)))
					{
						throw new com.epl.geometry.UserCancelException();
					}
				}
				int firstHalfEdge = GetClusterHalfEdge(cluster);
				if (firstHalfEdge != -1)
				{
					new_edges.ResizePreserveCapacity(0);
					if (!TryOptimizedInsertion_(aet, treeNodeIndex, new_edges, cluster, firstHalfEdge))
					{
						// optimized insertion is for a
						// simple chain, in that case we
						// simply replace an old edge
						// with a new one in AET - O(1)
						// This is more complex than a simple chain of edges
						GetXY(cluster, pt);
						comparator.SetY(pt.y);
						int clusterHalfEdge = firstHalfEdge;
						do
						{
							// Delete all edges that end at the cluster.
							// edges that end at the cluster have been assigned an
							// AET node in the treeNodeIndex.
							int attachedTreeNode = GetHalfEdgeUserIndex(clusterHalfEdge, treeNodeIndex);
							if (attachedTreeNode != -1)
							{
								System.Diagnostics.Debug.Assert((attachedTreeNode != com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2()));
								aet.DeleteNode(attachedTreeNode, -1);
								SetHalfEdgeUserIndex(clusterHalfEdge, treeNodeIndex, com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2());
							}
							// set it to -2
							clusterHalfEdge = GetHalfEdgeNext(GetHalfEdgeTwin(clusterHalfEdge));
							System.Diagnostics.Debug.Assert((GetHalfEdgeOrigin(clusterHalfEdge) == cluster));
						}
						while (firstHalfEdge != clusterHalfEdge);
						// insert edges that start at the cluster.
						// We need to insert only the edges that have the from point
						// below the to point.
						// This is ensured by the logic of the algorithm.
						clusterHalfEdge = firstHalfEdge;
						do
						{
							int attachedTreeNode = GetHalfEdgeUserIndex(clusterHalfEdge, treeNodeIndex);
							if (attachedTreeNode == -1)
							{
								int newTreeNode = aet.AddElement(clusterHalfEdge, -1);
								new_edges.Add(newTreeNode);
							}
							clusterHalfEdge = GetHalfEdgeNext(GetHalfEdgeTwin(clusterHalfEdge));
							System.Diagnostics.Debug.Assert((GetHalfEdgeOrigin(clusterHalfEdge) == cluster));
						}
						while (firstHalfEdge != clusterHalfEdge);
					}
					// Analyze new edges.
					// We go in the opposite order, because of the way how the half
					// edges are sorted on a cluster.
					// We want to go from the left to the right.
					for (int i = new_edges.Size() - 1; i >= 0; i--)
					{
						int newTreeNode = new_edges.Get(i);
						int clusterHalfEdge = aet.GetElement(newTreeNode);
						int twinEdge = GetHalfEdgeTwin(clusterHalfEdge);
						System.Diagnostics.Debug.Assert((GetHalfEdgeUserIndex(twinEdge, treeNodeIndex) == -1));
						SetHalfEdgeUserIndex(twinEdge, treeNodeIndex, newTreeNode);
						PlaneSweepParentagePropagateParentage_(aet, newTreeNode, inputMode);
					}
				}
				else
				{
					if (GetClusterChain(cluster) == -1)
					{
						// get the left half edge of a face. The point belongs to the
						// face.
						if (clusterMoniker == null)
						{
							clusterMoniker = new com.epl.geometry.TopoGraph.ClusterSweepMonikerComparator(this);
						}
						GetXY(cluster, pt);
						clusterMoniker.SetPointXY(pt);
						int leftNode = aet.SearchLowerBound(clusterMoniker, -1);
						int chain = m_universeChain;
						if (leftNode != -1)
						{
							int edge = aet.GetElement(leftNode);
							int leftChain = GetHalfEdgeChain(edge);
							if (leftChain == GetHalfEdgeChain(GetHalfEdgeTwin(edge)))
							{
								edge = GetLeftSkipPolylines_(aet, leftNode);
							}
							if (edge != -1)
							{
								chain = GetHalfEdgeChain(edge);
							}
						}
						AddClusterToExteriorChain_(chain, cluster);
					}
				}
			}
			DeleteUserIndexForHalfEdges(treeNodeIndex);
		}

		internal void PlaneSweepParentagePropagateParentage_(com.epl.geometry.Treap aet, int treeNode, int inputMode)
		{
			int edge = aet.GetElement(treeNode);
			int edgeChain = GetHalfEdgeChain(edge);
			int edgeChainParent = GetChainParent(edgeChain);
			if (edgeChainParent != -1)
			{
				return;
			}
			// this edge has been processed already.
			// get contributing left edge.
			int leftEdge = GetLeftSkipPolylines_(aet, treeNode);
			int twinEdge = GetHalfEdgeTwin(edge);
			int twinHalfEdgeChain = GetHalfEdgeChain(twinEdge);
			double chainArea = GetChainArea(edgeChain);
			double twinChainArea = GetChainArea(twinHalfEdgeChain);
			int parentChain = GetChainParent(edgeChain);
			int twinParentChain = GetChainParent(twinHalfEdgeChain);
			if (leftEdge == -1 && parentChain == -1)
			{
				// This edge/twin pair does not have a neighbour edge to the left.
				// twin parent is not yet been assigned.
				if (twinHalfEdgeChain == edgeChain)
				{
					// set parentage of a polyline
					// edge (any edge for which
					// the edge ant its twin
					// belong to the same chain)
					SetChainParent_(twinHalfEdgeChain, GetFirstChain());
					twinParentChain = GetFirstChain();
					parentChain = twinParentChain;
				}
				else
				{
					// We have two touching chains that do not have parent chain
					// set.
					// The edge is directed up, the twin edge is directed down.
					// There is no edge to the left. THat means there is no other
					// than the universe surrounding this edge.
					// The edge must belong to a clockwise chain, and the twin edge
					// must belong to a ccw chain that encloses this edge. This
					// follows from the way how we connect edges around clusters.
					System.Diagnostics.Debug.Assert((twinChainArea < 0 && chainArea > 0));
					if (twinParentChain == -1)
					{
						SetChainParent_(twinHalfEdgeChain, m_universeChain);
						twinParentChain = m_universeChain;
					}
					else
					{
						System.Diagnostics.Debug.Assert((GetFirstChain() == twinParentChain));
					}
					SetChainParent_(edgeChain, twinHalfEdgeChain);
					parentChain = twinHalfEdgeChain;
				}
			}
			if (leftEdge != -1)
			{
				int leftEdgeChain = GetHalfEdgeChain(leftEdge);
				// the twin edge has not been processed yet
				if (twinParentChain == -1)
				{
					double leftArea = GetChainArea(leftEdgeChain);
					if (leftArea <= 0)
					{
						// if left Edge's chain area is negative,
						// then it is a chain that ends at the left
						// edge, so we need to get the parent of the
						// left chain and it will be the parent of
						// this one.
						int leftChainParent = GetChainParent(leftEdgeChain);
						System.Diagnostics.Debug.Assert((leftChainParent != -1));
						SetChainParent_(twinHalfEdgeChain, leftChainParent);
						twinParentChain = leftChainParent;
					}
					else
					{
						// (leftArea > 0)
						// left edge is an edge of positive chain. It surrounds the
						// twin chain.
						SetChainParent_(twinHalfEdgeChain, leftEdgeChain);
						twinParentChain = leftEdgeChain;
					}
					if (twinHalfEdgeChain == edgeChain)
					{
						// if this is a polyline
						// chain
						parentChain = twinParentChain;
					}
				}
			}
			if (parentChain == -1)
			{
				TrySetChainParentFromTwin_(edgeChain, twinHalfEdgeChain);
				parentChain = GetChainParent(edgeChain);
			}
			System.Diagnostics.Debug.Assert((parentChain != -1));
			if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeBuildGraph)
			{
				Propagate_parentage_build_graph_(aet, treeNode, edge, leftEdge, edgeChain, edgeChainParent, twinHalfEdgeChain);
			}
			else
			{
				if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyWinding)
				{
					Propagate_parentage_winding_(aet, treeNode, edge, leftEdge, twinEdge, edgeChain, edgeChainParent, twinHalfEdgeChain);
				}
				else
				{
					if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyAlternate)
					{
						Propagate_parentage_alternate_(aet, treeNode, edge, leftEdge, twinEdge, edgeChain, edgeChainParent, twinHalfEdgeChain);
					}
				}
			}
		}

		internal void Propagate_parentage_build_graph_(com.epl.geometry.Treap aet, int treeNode, int edge, int leftEdge, int edgeChain, int edgeChainParent, int twinHalfEdgeChain)
		{
			// Now do specific sweep calculations
			int chainParentage = GetChainParentage(edgeChain);
			if (leftEdge != -1)
			{
				// borrow the parentage from the left edge also
				int leftEdgeChain = GetHalfEdgeChain(leftEdge);
				// We take parentage from the left edge (that edge has been
				// already processed), and move its face parentage accross this
				// edge/twin pair.
				// While the parentage is moved, accross, any bits of the
				// parentage that is present in the twin are removed, because
				// the twin is the right edge of the current face.
				// The remaining bits are added to the face parentage of this
				// edge, indicating that the face this edge borders, belongs to
				// all the parents that are still active to the left.
				int twinChainParentage = GetChainParentage(twinHalfEdgeChain);
				int leftChainParentage = GetChainParentage(leftEdgeChain);
				int edgeParentage = GetHalfEdgeParentage(edge);
				int spikeParentage = chainParentage & twinChainParentage & leftChainParentage;
				// parentage that needs to stay
				leftChainParentage = leftChainParentage ^ (leftChainParentage & edgeParentage);
				leftChainParentage |= spikeParentage;
				if (leftChainParentage != 0)
				{
					// propagate left parentage to the current edge and its
					// twin.
					SetChainParentage_(twinHalfEdgeChain, twinChainParentage | leftChainParentage);
					SetChainParentage_(edgeChain, leftChainParentage | chainParentage);
					chainParentage |= leftChainParentage;
				}
			}
			// dbg_print_edge_(edge);
			for (int rightNode = aet.GetNext(treeNode); rightNode != -1; rightNode = aet.GetNext(rightNode))
			{
				int rightEdge = aet.GetElement(rightNode);
				int rightTwin = GetHalfEdgeTwin(rightEdge);
				int rightTwinChain = GetHalfEdgeChain(rightTwin);
				int rightTwinChainParentage = GetChainParentage(rightTwinChain);
				int rightEdgeParentage = GetHalfEdgeParentage(rightEdge);
				int rightEdgeChain = GetHalfEdgeChain(rightEdge);
				int rightChainParentage = GetChainParentage(rightEdgeChain);
				int spikeParentage = rightTwinChainParentage & rightChainParentage & chainParentage;
				// parentage
				// that needs to
				// stay
				chainParentage = chainParentage ^ (chainParentage & rightEdgeParentage);
				// only
				// parentage
				// that is
				// abscent in
				// the twin is
				// propagated to
				// the right
				chainParentage |= spikeParentage;
				if (chainParentage == 0)
				{
					break;
				}
				SetChainParentage_(rightTwinChain, rightTwinChainParentage | chainParentage);
				SetChainParentage_(rightEdgeChain, rightChainParentage | chainParentage);
			}
		}

		internal void Propagate_parentage_winding_(com.epl.geometry.Treap aet, int treeNode, int edge, int leftEdge, int twinEdge, int edgeChain, int edgeChainParent, int twinHalfEdgeChain)
		{
			if (edgeChain == twinHalfEdgeChain)
			{
				return;
			}
			// starting from the left most edge, calculate winding.
			int edgeWinding = GetHalfEdgeUserIndex(edge, m_tmpHalfEdgeWindingNumberIndex);
			edgeWinding += GetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeWindingNumberIndex);
			int winding = 0;
			com.epl.geometry.AttributeStreamOfInt32 chainStack = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 windingStack = new com.epl.geometry.AttributeStreamOfInt32(0);
			windingStack.Add(0);
			for (int leftNode = aet.GetFirst(-1); leftNode != treeNode; leftNode = aet.GetNext(leftNode))
			{
				int leftEdge1 = aet.GetElement(leftNode);
				int leftTwin = GetHalfEdgeTwin(leftEdge1);
				int l_chain = GetHalfEdgeChain(leftEdge1);
				int lt_chain = GetHalfEdgeChain(leftTwin);
				if (l_chain != lt_chain)
				{
					int leftWinding = GetHalfEdgeUserIndex(leftEdge1, m_tmpHalfEdgeWindingNumberIndex);
					leftWinding += GetHalfEdgeUserIndex(leftTwin, m_tmpHalfEdgeWindingNumberIndex);
					winding += leftWinding;
					bool popped = false;
					if (chainStack.Size() != 0 && chainStack.GetLast() == lt_chain)
					{
						windingStack.RemoveLast();
						chainStack.RemoveLast();
						popped = true;
					}
					if (GetChainParent(lt_chain) == -1)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					if (!popped || GetChainParent(lt_chain) != l_chain)
					{
						windingStack.Add(winding);
						chainStack.Add(l_chain);
					}
				}
			}
			winding += edgeWinding;
			if (chainStack.Size() != 0 && chainStack.GetLast() == twinHalfEdgeChain)
			{
				windingStack.RemoveLast();
				chainStack.RemoveLast();
			}
			if (winding != 0)
			{
				if (windingStack.GetLast() == 0)
				{
					int geometry = m_shape.GetFirstGeometry();
					int geometryID = GetGeometryID(geometry);
					SetChainParentage_(edgeChain, geometryID);
				}
			}
			else
			{
				if (windingStack.GetLast() != 0)
				{
					int geometry = m_shape.GetFirstGeometry();
					int geometryID = GetGeometryID(geometry);
					SetChainParentage_(edgeChain, geometryID);
				}
			}
		}

		internal void Propagate_parentage_alternate_(com.epl.geometry.Treap aet, int treeNode, int edge, int leftEdge, int twinEdge, int edgeChain, int edgeChainParent, int twinHalfEdgeChain)
		{
			// Now do specific sweep calculations
			// This one is done when we are doing a topological operation.
			int geometry = m_shape.GetFirstGeometry();
			int geometryID = GetGeometryID(geometry);
			if (leftEdge == -1)
			{
				// no left edge neighbour means the twin chain is surrounded by the
				// universe
				System.Diagnostics.Debug.Assert((GetChainParent(twinHalfEdgeChain) == m_universeChain));
				System.Diagnostics.Debug.Assert((GetChainParentage(twinHalfEdgeChain) == 0 || GetChainParentage(twinHalfEdgeChain) == m_universe_geomID));
				System.Diagnostics.Debug.Assert((GetChainParentage(edgeChain) == 0));
				SetChainParentage_(twinHalfEdgeChain, m_universe_geomID);
				int parity = GetHalfEdgeUserIndex(edge, m_tmpHalfEdgeOddEvenNumberIndex);
				if ((parity & 1) != 0)
				{
					SetChainParentage_(edgeChain, geometryID);
				}
				else
				{
					// set the parenentage
					// from the parity
					SetChainParentage_(edgeChain, m_universe_geomID);
				}
			}
			else
			{
				// this chain
				// does not
				// belong to
				// geometry
				int twin_parentage = GetChainParentage(twinHalfEdgeChain);
				if (twin_parentage == 0)
				{
					int leftEdgeChain = GetHalfEdgeChain(leftEdge);
					int left_parentage = GetChainParentage(leftEdgeChain);
					SetChainParentage_(twinHalfEdgeChain, left_parentage);
					int parity = GetHalfEdgeUserIndex(edge, m_tmpHalfEdgeOddEvenNumberIndex);
					if ((parity & 1) != 0)
					{
						SetChainParentage_(edgeChain, (left_parentage == geometryID) ? m_universe_geomID : geometryID);
					}
					else
					{
						SetChainParentage_(edgeChain, left_parentage);
					}
				}
				else
				{
					int parity = GetHalfEdgeUserIndex(edge, m_tmpHalfEdgeOddEvenNumberIndex);
					if ((parity & 1) != 0)
					{
						SetChainParentage_(edgeChain, (twin_parentage == geometryID) ? m_universe_geomID : geometryID);
					}
					else
					{
						SetChainParentage_(edgeChain, twin_parentage);
					}
				}
			}
		}

		internal bool TryOptimizedInsertion_(com.epl.geometry.Treap aet, int treeNodeIndex, com.epl.geometry.AttributeStreamOfInt32 new_edges, int cluster, int firstHalfEdge)
		{
			int clusterHalfEdge = firstHalfEdge;
			int attachedTreeNode = -1;
			int newEdge = -1;
			// Delete all edges that end at the cluster.
			int count = 0;
			do
			{
				if (count == 2)
				{
					return false;
				}
				int n = GetHalfEdgeUserIndex(clusterHalfEdge, treeNodeIndex);
				if (n != -1)
				{
					if (attachedTreeNode != -1)
					{
						return false;
					}
					// two edges end at the cluster
					attachedTreeNode = n;
				}
				else
				{
					if (newEdge != -1)
					{
						return false;
					}
					// two edges start from the cluster
					newEdge = clusterHalfEdge;
				}
				System.Diagnostics.Debug.Assert((GetHalfEdgeOrigin(clusterHalfEdge) == cluster));
				count++;
				clusterHalfEdge = GetHalfEdgeNext(GetHalfEdgeTwin(clusterHalfEdge));
			}
			while (firstHalfEdge != clusterHalfEdge);
			if (newEdge == -1 || attachedTreeNode == -1)
			{
				return false;
			}
			SetHalfEdgeUserIndex(aet.GetElement(attachedTreeNode), treeNodeIndex, com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2());
			aet.SetElement(attachedTreeNode, newEdge);
			new_edges.Add(attachedTreeNode);
			return true;
		}

		internal bool TrySetChainParentFromTwin_(int chainToSet, int twinChain)
		{
			System.Diagnostics.Debug.Assert((GetChainParent(chainToSet) == -1));
			double area = GetChainArea(chainToSet);
			if (area == 0)
			{
				return false;
			}
			double twinArea = GetChainArea(twinChain);
			System.Diagnostics.Debug.Assert((twinArea != 0));
			if (area > 0 && twinArea < 0)
			{
				SetChainParent_(chainToSet, twinChain);
				return true;
			}
			if (area < 0 && twinArea > 0)
			{
				SetChainParent_(chainToSet, twinChain);
				return true;
			}
			else
			{
				int twinParent = GetChainParent(twinChain);
				if (twinParent != -1)
				{
					SetChainParent_(chainToSet, twinParent);
					return true;
				}
			}
			return false;
		}

		internal void CreateHalfEdges_(int inputMode, com.epl.geometry.AttributeStreamOfInt32 sorted_vertices)
		{
			// After this loop all halfedges will be created.
			// This loop also sets the known parentage on the edges.
			// The half edges are connected with each other in a random order
			m_halfEdgeIndex = m_shape.CreateUserIndex();
			for (int i = 0, nvert = sorted_vertices.Size(); i < nvert; i++)
			{
				int vertex = sorted_vertices.Get(i);
				int cluster = m_shape.GetUserIndex(vertex, m_clusterIndex);
				int path = m_shape.GetPathFromVertex(vertex);
				int geometry = m_shape.GetGeometryFromPath(path);
				int gt = m_shape.GetGeometryType(geometry);
				if (com.epl.geometry.Geometry.IsMultiPath(gt))
				{
					int next = m_shape.GetNextVertex(vertex);
					if (next == -1)
					{
						continue;
					}
					int clusterTo = m_shape.GetUserIndex(next, m_clusterIndex);
					System.Diagnostics.Debug.Assert((clusterTo != -1));
					if (cluster == clusterTo)
					{
						if (m_shape.GetSegment(vertex) != null)
						{
							System.Diagnostics.Debug.Assert((m_shape.GetSegment(vertex).CalculateLength2D() == 0));
						}
						else
						{
							System.Diagnostics.Debug.Assert((m_shape.GetXY(vertex).IsEqual(m_shape.GetXY(next))));
						}
						continue;
					}
					int half_edge = NewHalfEdgePair_();
					int twinEdge = GetHalfEdgeTwin(half_edge);
					// add vertex to the half edge.
					int vertIndex = m_clusterVertices.NewElement();
					m_clusterVertices.SetField(vertIndex, 0, vertex);
					m_clusterVertices.SetField(vertIndex, 1, -1);
					SetHalfEdgeVertexIterator_(half_edge, vertIndex);
					SetHalfEdgeOrigin_(half_edge, cluster);
					int firstHalfEdge = GetClusterHalfEdge(cluster);
					if (firstHalfEdge == -1)
					{
						SetClusterHalfEdge_(cluster, half_edge);
						SetHalfEdgePrev_(half_edge, twinEdge);
						SetHalfEdgeNext_(twinEdge, half_edge);
					}
					else
					{
						// It does not matter what order we insert the new edges in.
						// We fix the order later.
						int firstPrev = GetHalfEdgePrev(firstHalfEdge);
						System.Diagnostics.Debug.Assert((GetHalfEdgeNext(firstPrev) == firstHalfEdge));
						SetHalfEdgePrev_(firstHalfEdge, twinEdge);
						SetHalfEdgeNext_(twinEdge, firstHalfEdge);
						System.Diagnostics.Debug.Assert((GetHalfEdgePrev(firstHalfEdge) == twinEdge));
						System.Diagnostics.Debug.Assert((GetHalfEdgeNext(twinEdge) == firstHalfEdge));
						SetHalfEdgeNext_(firstPrev, half_edge);
						SetHalfEdgePrev_(half_edge, firstPrev);
						System.Diagnostics.Debug.Assert((GetHalfEdgePrev(half_edge) == firstPrev));
						System.Diagnostics.Debug.Assert((GetHalfEdgeNext(firstPrev) == half_edge));
					}
					SetHalfEdgeOrigin_(twinEdge, clusterTo);
					int firstTo = GetClusterHalfEdge(clusterTo);
					if (firstTo == -1)
					{
						SetClusterHalfEdge_(clusterTo, twinEdge);
						SetHalfEdgeNext_(half_edge, twinEdge);
						SetHalfEdgePrev_(twinEdge, half_edge);
					}
					else
					{
						int firstToPrev = GetHalfEdgePrev(firstTo);
						System.Diagnostics.Debug.Assert((GetHalfEdgeNext(firstToPrev) == firstTo));
						SetHalfEdgePrev_(firstTo, half_edge);
						SetHalfEdgeNext_(half_edge, firstTo);
						System.Diagnostics.Debug.Assert((GetHalfEdgePrev(firstTo) == half_edge));
						System.Diagnostics.Debug.Assert((GetHalfEdgeNext(half_edge) == firstTo));
						SetHalfEdgeNext_(firstToPrev, twinEdge);
						SetHalfEdgePrev_(twinEdge, firstToPrev);
						System.Diagnostics.Debug.Assert((GetHalfEdgePrev(twinEdge) == firstToPrev));
						System.Diagnostics.Debug.Assert((GetHalfEdgeNext(firstToPrev) == twinEdge));
					}
					int geometryID = GetGeometryID(geometry);
					// No chains yet exists, so we use a temporary user index to
					// store chain parentage.
					// The input polygons has been already simplified so their edges
					// directed such that the hole is to the left from the edge
					// (each edge is directed from the "from" to "to" point).
					if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeBuildGraph)
					{
						SetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeParentageIndex, 0);
						// Hole is always to the left. left side here is
						// the twin.
						SetHalfEdgeUserIndex(half_edge, m_tmpHalfEdgeParentageIndex, gt == com.epl.geometry.Geometry.GeometryType.Polygon ? geometryID : 0);
					}
					else
					{
						if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyWinding)
						{
							com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
							m_shape.GetXY(vertex, pt_1);
							com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
							m_shape.GetXY(next, pt_2);
							int windingNumber = 0;
							int windingNumberTwin = 0;
							if (pt_1.Compare(pt_2) < 0)
							{
								// The edge is directed bottom-up. That means it has the
								// winding number of +1.
								// The half-edge direction coincides with the edge
								// direction. THe twin is directed top-down.
								// The half edge will have the winding number of 1 and
								// its twin the winding number of 0.
								// When crossing the half-edge/twin pair from left to
								// right, the winding number is changed by +1
								windingNumber = 1;
							}
							else
							{
								// The edge is directed top-down. That means it has the
								// winding number of -1.
								// The half-edge direction coincides with the edge
								// direction. The twin is directed bottom-up.
								// The half edge will have the winding number of 0 and
								// its twin the winding number of -1.
								// When crossing the half-edge/twin pair from left to
								// right, the winding number is changed by -1.
								windingNumberTwin = -1;
							}
							// When we get a half-edge/twin pair, we can determine the
							// winding number of the underlying edge
							// by summing up the half-edge and twin's
							// winding numbers.
							SetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeParentageIndex, 0);
							SetHalfEdgeUserIndex(half_edge, m_tmpHalfEdgeParentageIndex, 0);
							// We split the winding number between the half edge and its
							// twin.
							// This allows us to determine which half edge goes in the
							// direction of the edge, and also it allows to calculate
							// the
							// winging number by summing up the winding number of half
							// edge and its twin.
							SetHalfEdgeUserIndex(half_edge, m_tmpHalfEdgeWindingNumberIndex, windingNumber);
							SetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeWindingNumberIndex, windingNumberTwin);
						}
						else
						{
							if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeIsSimplePolygon)
							{
								SetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeParentageIndex, m_universe_geomID);
								SetHalfEdgeUserIndex(half_edge, m_tmpHalfEdgeParentageIndex, gt == com.epl.geometry.Geometry.GeometryType.Polygon ? geometryID : 0);
							}
							else
							{
								if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyAlternate)
								{
									SetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeParentageIndex, 0);
									SetHalfEdgeUserIndex(half_edge, m_tmpHalfEdgeParentageIndex, 0);
									SetHalfEdgeUserIndex(half_edge, m_tmpHalfEdgeOddEvenNumberIndex, 1);
									SetHalfEdgeUserIndex(twinEdge, m_tmpHalfEdgeOddEvenNumberIndex, 1);
								}
							}
						}
					}
					int edgeBit = gt == com.epl.geometry.Geometry.GeometryType.Polygon ? c_edgeBitMask : 0;
					SetHalfEdgeParentage_(half_edge, geometryID | edgeBit);
					SetHalfEdgeParentage_(twinEdge, geometryID | edgeBit);
				}
			}
		}

		internal void MergeVertexListsOfEdges_(int eDst, int eSrc)
		{
			System.Diagnostics.Debug.Assert((GetHalfEdgeTo(eDst) == GetHalfEdgeTo(eSrc)));
			System.Diagnostics.Debug.Assert((GetHalfEdgeOrigin(eDst) == GetHalfEdgeOrigin(eSrc)));
			{
				int vertFirst2 = GetHalfEdgeVertexIterator(eSrc);
				if (vertFirst2 != -1)
				{
					int vertFirst1 = GetHalfEdgeVertexIterator(eDst);
					m_clusterVertices.SetField(vertFirst2, 1, vertFirst1);
					SetHalfEdgeVertexIterator_(eDst, vertFirst2);
					SetHalfEdgeVertexIterator_(eSrc, -1);
				}
			}
			int eDstTwin = GetHalfEdgeTwin(eDst);
			int eSrcTwin = GetHalfEdgeTwin(eSrc);
			{
				int vertFirst2 = GetHalfEdgeVertexIterator(eSrcTwin);
				if (vertFirst2 != -1)
				{
					int vertFirst1 = GetHalfEdgeVertexIterator(eDstTwin);
					m_clusterVertices.SetField(vertFirst2, 1, vertFirst1);
					SetHalfEdgeVertexIterator_(eDstTwin, vertFirst2);
					SetHalfEdgeVertexIterator_(eSrcTwin, -1);
				}
			}
		}

		internal void SortHalfEdgesByAngle_(int inputMode)
		{
			com.epl.geometry.AttributeStreamOfInt32 angleSorter = new com.epl.geometry.AttributeStreamOfInt32(0);
			angleSorter.Reserve(10);
			com.epl.geometry.TopoGraph.TopoGraphAngleComparer tgac = new com.epl.geometry.TopoGraph.TopoGraphAngleComparer(this);
			// Now go through the clusters, sort edges in each cluster by angle, and
			// reconnect the halfedges of sorted edges in the sorted order.
			// Also share the parentage information between coinciding edges and
			// remove duplicates.
			for (int cluster = GetFirstCluster(); cluster != -1; cluster = GetNextCluster(cluster))
			{
				angleSorter.Clear(false);
				int first = GetClusterHalfEdge(cluster);
				if (first != -1)
				{
					// 1. sort edges originating at the cluster by angle (counter -
					// clockwise).
					int edge = first;
					do
					{
						angleSorter.Add(edge);
						// edges have the cluster in their
						// origin and are directed away from
						// it. The twin edges are directed
						// towards the cluster.
						edge = GetHalfEdgeNext(GetHalfEdgeTwin(edge));
					}
					while (edge != first);
					if (angleSorter.Size() > 1)
					{
						bool changed_order = true;
						if (angleSorter.Size() > 2)
						{
							angleSorter.Sort(0, angleSorter.Size(), tgac);
							// std::sort(angleSorter.get_ptr(),
							// angleSorter.get_ptr()
							// +
							// angleSorter.size(),
							// TopoGraphAngleComparer(this));
							angleSorter.Add(angleSorter.Get(0));
						}
						else
						{
							//no need to sort most two edge cases. we only need to make sure that edges going up are sorted
							if (CompareEdgeAnglesForPair_(angleSorter.Get(0), angleSorter.Get(1)) > 0)
							{
								int tmp = angleSorter.Get(0);
								angleSorter.Set(0, angleSorter.Get(1));
								angleSorter.Set(1, tmp);
							}
							else
							{
								changed_order = false;
							}
						}
						// 2. get rid of duplicate edges by merging them (duplicate
						// edges appear at this step because we converted all
						// segments into the edges, including overlapping).
						int e0 = angleSorter.Get(0);
						int ePrev = e0;
						int ePrevTo = GetHalfEdgeTo(ePrev);
						int ePrevTwin = GetHalfEdgeTwin(ePrev);
						int prevMerged = -1;
						for (int i = 1, n = angleSorter.Size(); i < n; i++)
						{
							int e = angleSorter.Get(i);
							int eTwin = GetHalfEdgeTwin(e);
							int eTo = GetHalfEdgeOrigin(eTwin);
							System.Diagnostics.Debug.Assert((GetHalfEdgeOrigin(e) == GetHalfEdgeOrigin(ePrev)));
							// e
							// origin
							// and
							// ePrev
							// origin
							// are
							// equal
							// by
							// definition
							// (e
							// and
							// ePrev
							// emanate
							// from
							// the
							// same
							// cluster)
							if (eTo == ePrevTo && e != ePrev)
							{
								// e's To cluster and
								// ePrev's To
								// cluster are
								// equal, means the
								// edges coincide
								// and need to be
								// merged.
								// remove duplicate edge. Before removing, propagate
								// the parentage to the remaning edge
								if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeBuildGraph)
								{
									int newEdgeParentage = GetHalfEdgeParentageMask_(ePrev) | GetHalfEdgeParentageMask_(e);
									SetHalfEdgeParentage_(ePrev, newEdgeParentage);
									SetHalfEdgeParentage_(ePrevTwin, newEdgeParentage);
									System.Diagnostics.Debug.Assert((GetHalfEdgeParentageMask_(ePrev) == GetHalfEdgeParentageMask_(ePrevTwin)));
									SetHalfEdgeUserIndex(ePrev, m_tmpHalfEdgeParentageIndex, GetHalfEdgeUserIndex(ePrev, m_tmpHalfEdgeParentageIndex) | GetHalfEdgeUserIndex(e, m_tmpHalfEdgeParentageIndex));
									SetHalfEdgeUserIndex(ePrevTwin, m_tmpHalfEdgeParentageIndex, GetHalfEdgeUserIndex(ePrevTwin, m_tmpHalfEdgeParentageIndex) | GetHalfEdgeUserIndex(eTwin, m_tmpHalfEdgeParentageIndex));
								}
								else
								{
									if (m_tmpHalfEdgeWindingNumberIndex != -1)
									{
										// when doing simplify the
										// m_tmpHalfEdgeWindingNumberIndex contains the
										// winding number.
										// When edges are merged their winding numbers
										// are added.
										int newHalfEdgeWinding = GetHalfEdgeUserIndex(ePrev, m_tmpHalfEdgeWindingNumberIndex) + GetHalfEdgeUserIndex(e, m_tmpHalfEdgeWindingNumberIndex);
										int newTwinEdgeWinding = GetHalfEdgeUserIndex(ePrevTwin, m_tmpHalfEdgeWindingNumberIndex) + GetHalfEdgeUserIndex(eTwin, m_tmpHalfEdgeWindingNumberIndex);
										SetHalfEdgeUserIndex(ePrev, m_tmpHalfEdgeWindingNumberIndex, newHalfEdgeWinding);
										SetHalfEdgeUserIndex(ePrevTwin, m_tmpHalfEdgeWindingNumberIndex, newTwinEdgeWinding);
									}
									else
									{
										// The winding number of an edge is a sum of the
										// winding numbers of the half edge and its
										// twin.
										// To determine which half edge direction
										// coincides with the edge direction, determine
										// which half edge has larger abs value of
										// winding number. If half edge and twin winding
										// numbers cancel each other, the edge winding
										// number is zero, meaning there are
										// even number of edges coinciding there and
										// half of them has opposite direction to
										// another half.
										if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeIsSimplePolygon)
										{
											m_non_simple_result = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.CrossOver, cluster, -1);
											return;
										}
										else
										{
											if (m_tmpHalfEdgeOddEvenNumberIndex != -1)
											{
												int newHalfEdgeWinding = GetHalfEdgeUserIndex(ePrev, m_tmpHalfEdgeOddEvenNumberIndex) + GetHalfEdgeUserIndex(e, m_tmpHalfEdgeOddEvenNumberIndex);
												int newTwinEdgeWinding = GetHalfEdgeUserIndex(ePrevTwin, m_tmpHalfEdgeOddEvenNumberIndex) + GetHalfEdgeUserIndex(eTwin, m_tmpHalfEdgeOddEvenNumberIndex);
												SetHalfEdgeUserIndex(ePrev, m_tmpHalfEdgeOddEvenNumberIndex, newHalfEdgeWinding);
												SetHalfEdgeUserIndex(ePrevTwin, m_tmpHalfEdgeOddEvenNumberIndex, newTwinEdgeWinding);
											}
										}
									}
								}
								MergeVertexListsOfEdges_(ePrev, e);
								DeleteEdgeImpl_(e);
								System.Diagnostics.Debug.Assert((n < 3 || e0 == angleSorter.GetLast()));
								prevMerged = ePrev;
								angleSorter.Set(i, -1);
								if (e == e0)
								{
									angleSorter.Set(0, -1);
									e0 = -1;
								}
								continue;
							}
							//edges do not coincide
							UpdateVertexToHalfEdgeConnection_(prevMerged, false);
							prevMerged = -1;
							ePrev = e;
							ePrevTo = eTo;
							ePrevTwin = eTwin;
						}
						UpdateVertexToHalfEdgeConnection_(prevMerged, false);
						prevMerged = -1;
						if (!changed_order)
						{
							//small optimization to avoid reconnecting if nothing changed
							e0 = -1;
							for (int i_1 = 0, n = angleSorter.Size(); i_1 < n; i_1++)
							{
								int e = angleSorter.Get(i_1);
								if (e == -1)
								{
									continue;
								}
								e0 = e;
								break;
							}
							if (first != e0)
							{
								SetClusterHalfEdge_(cluster, e0);
							}
							continue;
						}
						//next cluster 
						// 3. Reconnect edges in the sorted order. The edges are
						// sorted counter clockwise.
						// We connect them such that every right turn is made in the
						// clockwise order.
						// This guarantees that the smallest faces are clockwise.
						e0 = -1;
						for (int i_2 = 0, n = angleSorter.Size(); i_2 < n; i_2++)
						{
							int e = angleSorter.Get(i_2);
							if (e == -1)
							{
								continue;
							}
							if (e0 == -1)
							{
								e0 = e;
								ePrev = e0;
								ePrevTo = GetHalfEdgeTo(ePrev);
								ePrevTwin = GetHalfEdgeTwin(ePrev);
								continue;
							}
							if (e == ePrev)
							{
								// This condition can only happen if all edges in
								// the bunch coincide.
								System.Diagnostics.Debug.Assert((i_2 == n - 1));
								continue;
							}
							int eTwin = GetHalfEdgeTwin(e);
							int eTo = GetHalfEdgeOrigin(eTwin);
							System.Diagnostics.Debug.Assert((GetHalfEdgeOrigin(e) == GetHalfEdgeOrigin(ePrev)));
							System.Diagnostics.Debug.Assert((eTo != ePrevTo));
							SetHalfEdgeNext_(ePrevTwin, e);
							SetHalfEdgePrev_(e, ePrevTwin);
							ePrev = e;
							ePrevTo = eTo;
							ePrevTwin = eTwin;
							if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeIsSimplePolygon)
							{
								int par1 = GetHalfEdgeUserIndex(e, m_tmpHalfEdgeParentageIndex) | GetHalfEdgeUserIndex(GetHalfEdgePrev(e), m_tmpHalfEdgeParentageIndex);
								if (par1 == (m_universe_geomID | 1))
								{
									//violation of face parentage
									m_non_simple_result = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.CrossOver, cluster, -1);
									return;
								}
							}
						}
						SetClusterHalfEdge_(cluster, e0);
					}
				}
			}
		}

		// smallest angle goes
		// first.
		internal void BuildChains_(int inputMode)
		{
			// Creates chains and puts them in the list of chains.
			// Does not set the chain parentage
			// Does not connect chains
			int firstChain = -1;
			int visitedHalfEdgeIndex = CreateUserIndexForHalfEdges();
			// Visit all the clusters
			for (int cluster = GetFirstCluster(); cluster != -1; cluster = GetNextCluster(cluster))
			{
				// For each cluster visit all half edges on the cluster
				int first = GetClusterHalfEdge(cluster);
				if (first != -1)
				{
					int edge = first;
					do
					{
						if (GetHalfEdgeUserIndex(edge, visitedHalfEdgeIndex) != 1)
						{
							// check
							// if
							// we
							// have
							// visited
							// this
							// halfedge
							// already
							// if we have not visited this halfedge yet, then we have
							// not created a chain for it yet.
							int chain = NewChain_();
							// new chain's parentage is set
							// to 0.
							SetChainHalfEdge_(chain, edge);
							// Note, the half-edge's
							// Origin is the lowest
							// point of the chain.
							SetChainNext_(chain, firstChain);
							// add the new chain to
							// the list of
							// chains.
							if (firstChain != -1)
							{
								SetChainPrev_(firstChain, chain);
							}
							firstChain = chain;
							// go thorough all halfedges until return back to the
							// same one. Thus forming a chain.
							int parentage = 0;
							int e = edge;
							do
							{
								// accumulate chain parentage from all the chain
								// edges m_tmpHalfEdgeParentageIndex.
								parentage |= GetHalfEdgeUserIndex(e, m_tmpHalfEdgeParentageIndex);
								System.Diagnostics.Debug.Assert((GetHalfEdgeUserIndex(e, visitedHalfEdgeIndex) != 1));
								SetHalfEdgeChain_(e, chain);
								SetHalfEdgeUserIndex(e, visitedHalfEdgeIndex, 1);
								// mark
								// the
								// edge
								// visited.
								e = GetHalfEdgeNext(e);
							}
							while (e != edge);
							System.Diagnostics.Debug.Assert((inputMode != com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeIsSimplePolygon || parentage != (1 | m_universe_geomID)));
							SetChainParentage_(chain, parentage);
						}
						edge = GetHalfEdgeNext(GetHalfEdgeTwin(edge));
					}
					while (edge != first);
				}
			}
			// next
			// halfedge
			// on the
			// cluster
			// add the Universe chain. We want it to be the one that getFirstChain
			// returns.
			int chain_1 = NewChain_();
			SetChainHalfEdge_(chain_1, -1);
			SetChainNext_(chain_1, firstChain);
			if (firstChain != -1)
			{
				SetChainPrev_(firstChain, chain_1);
			}
			m_universeChain = chain_1;
			m_chainAreas = new com.epl.geometry.AttributeStreamOfDbl(m_chainData.Size(), com.epl.geometry.NumberUtils.TheNaN);
			m_chainPerimeters = new com.epl.geometry.AttributeStreamOfDbl(m_chainData.Size(), com.epl.geometry.NumberUtils.TheNaN);
			SetChainArea_(m_universeChain, com.epl.geometry.NumberUtils.PositiveInf());
			// the
			// Universe
			// is
			// infinite
			SetChainPerimeter_(m_universeChain, com.epl.geometry.NumberUtils.PositiveInf());
			// the
			// Universe
			// is
			// infinite
			DeleteUserIndexForHalfEdges(visitedHalfEdgeIndex);
		}

		internal void Simplify_(int inputMode)
		{
			if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyAlternate)
			{
				SimplifyAlternate_();
			}
			else
			{
				if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyWinding)
				{
					SimplifyWinding_();
				}
			}
		}

		internal void SimplifyAlternate_()
		{
		}

		//there is nothing to do
		internal void SimplifyWinding_()
		{
		}

		//there is nothing to do
		private int GetFirstUnvisitedHalfEdgeOnCluster_(int cluster, int hintEdge, int vistiedEdgesIndex)
		{
			// finds first half edge which is unvisited (index is not set to 1.
			// when hintEdge != -1, it is used to start going around the edges.
			int edge = hintEdge != -1 ? hintEdge : GetClusterHalfEdge(cluster);
			if (edge == -1)
			{
				return -1;
			}
			int f = edge;
			while (true)
			{
				int v = GetHalfEdgeUserIndex(edge, vistiedEdgesIndex);
				if (v != 1)
				{
					return edge;
				}
				int next = GetHalfEdgeNext(GetHalfEdgeTwin(edge));
				if (next == f)
				{
					return -1;
				}
				edge = next;
			}
		}

		internal bool RemoveSpikes_()
		{
			bool removed = false;
			int visitedIndex = CreateUserIndexForHalfEdges();
			for (int cluster = GetFirstCluster(); cluster != -1; cluster = GetNextCluster(cluster))
			{
				int nextClusterEdge = -1;
				//a hint
				while (true)
				{
					int firstHalfEdge = GetFirstUnvisitedHalfEdgeOnCluster_(cluster, nextClusterEdge, visitedIndex);
					if (firstHalfEdge == -1)
					{
						break;
					}
					nextClusterEdge = GetHalfEdgeNext(GetHalfEdgeTwin(firstHalfEdge));
					int faceHalfEdge = firstHalfEdge;
					while (true)
					{
						int faceHalfEdgeNext = GetHalfEdgeNext(faceHalfEdge);
						int faceHalfEdgePrev = GetHalfEdgePrev(faceHalfEdge);
						int faceHalfEdgeTwin = GetHalfEdgeTwin(faceHalfEdge);
						if (faceHalfEdgePrev == faceHalfEdgeTwin)
						{
							DeleteEdgeInternal_(faceHalfEdge);
							//deletes the edge and its twin
							removed = true;
							if (nextClusterEdge == faceHalfEdge || nextClusterEdge == faceHalfEdgeTwin)
							{
								nextClusterEdge = -1;
							}
							//deleted the hint edge
							if (faceHalfEdge == firstHalfEdge || faceHalfEdgePrev == firstHalfEdge)
							{
								firstHalfEdge = faceHalfEdgeNext;
								if (faceHalfEdge == firstHalfEdge || faceHalfEdgePrev == firstHalfEdge)
								{
									//deleted all edges in a face
									break;
								}
								faceHalfEdge = faceHalfEdgeNext;
								continue;
							}
						}
						else
						{
							SetHalfEdgeUserIndex(faceHalfEdge, visitedIndex, 1);
						}
						faceHalfEdge = faceHalfEdgeNext;
						if (faceHalfEdge == firstHalfEdge)
						{
							break;
						}
					}
				}
			}
			return removed;
		}

		internal void SetEditShapeImpl_(com.epl.geometry.EditShape shape, int inputMode, com.epl.geometry.AttributeStreamOfInt32 editShapeGeometries, com.epl.geometry.ProgressTracker progress_tracker, bool bBuildChains)
		{
			System.Diagnostics.Debug.Assert((!m_dirty_check_failed));
			System.Diagnostics.Debug.Assert((editShapeGeometries == null || editShapeGeometries.Size() > 0));
			RemoveShape();
			m_buildChains = bBuildChains;
			System.Diagnostics.Debug.Assert((m_shape == null));
			m_shape = shape;
			m_geometryIDIndex = m_shape.CreateGeometryUserIndex();
			// sort vertices lexicographically
			// Firstly copy all vertices to an array.
			com.epl.geometry.AttributeStreamOfInt32 verticesSorter = new com.epl.geometry.AttributeStreamOfInt32(0);
			verticesSorter.Reserve(editShapeGeometries != null ? m_shape.GetPointCount(editShapeGeometries.Get(0)) : m_shape.GetTotalPointCount());
			int path_count = 0;
			int geomID = 1;
			{
				// scope
				int geometry = editShapeGeometries != null ? editShapeGeometries.Get(0) : m_shape.GetFirstGeometry();
				int ind = 1;
				while (geometry != -1)
				{
					m_shape.SetGeometryUserIndex(geometry, m_geometryIDIndex, geomID);
					geomID = geomID << 1;
					for (int path = m_shape.GetFirstPath(geometry); path != -1; path = m_shape.GetNextPath(path))
					{
						int vertex = m_shape.GetFirstVertex(path);
						for (int index = 0, n = m_shape.GetPathSize(path); index < n; index++)
						{
							verticesSorter.Add(vertex);
							vertex = m_shape.GetNextVertex(vertex);
						}
					}
					if (!com.epl.geometry.Geometry.IsPoint(m_shape.GetGeometryType(geometry)))
					{
						path_count += m_shape.GetPathCount(geometry);
					}
					if (editShapeGeometries != null)
					{
						geometry = ind < editShapeGeometries.Size() ? editShapeGeometries.Get(ind) : -1;
						ind++;
					}
					else
					{
						geometry = m_shape.GetNextGeometry(geometry);
					}
				}
			}
			m_universe_geomID = geomID;
			m_pointCount = verticesSorter.Size();
			// sort
			m_shape.SortVerticesSimpleByY_(verticesSorter, 0, m_pointCount);
			if (m_clusterVertices == null)
			{
				m_clusterVertices = new com.epl.geometry.StridedIndexTypeCollection(2);
				m_clusterData = new com.epl.geometry.StridedIndexTypeCollection(8);
				m_halfEdgeData = new com.epl.geometry.StridedIndexTypeCollection(8);
				m_chainData = new com.epl.geometry.StridedIndexTypeCollection(8);
			}
			m_clusterVertices.SetCapacity(m_pointCount);
			com.epl.geometry.ProgressTracker.CheckAndThrow(progress_tracker);
			m_clusterData.SetCapacity(m_pointCount + 10);
			// 10 for some self
			// intersections
			m_halfEdgeData.SetCapacity(2 * m_pointCount + 32);
			m_chainData.SetCapacity(System.Math.Max((int)32, path_count));
			// create all clusters
			System.Diagnostics.Debug.Assert((m_clusterIndex == -1));
			// cleanup was incorrect
			m_clusterIndex = m_shape.CreateUserIndex();
			com.epl.geometry.Point2D ptFirst = new com.epl.geometry.Point2D();
			int ifirst = 0;
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			ptFirst.SetNaN();
			for (int i = 0; i <= m_pointCount; i++)
			{
				if (i < m_pointCount)
				{
					int vertex = verticesSorter.Get(i);
					m_shape.GetXY(vertex, pt);
				}
				else
				{
					pt.SetNaN();
				}
				// makes it to go into the following "if" statement.
				if (!ptFirst.IsEqual(pt))
				{
					if (ifirst < i)
					{
						int cluster = NewCluster_();
						int vertFirst = -1;
						int vert = -1;
						for (int ind = ifirst; ind < i; ind++)
						{
							vert = verticesSorter.Get(ind);
							m_shape.SetUserIndex(vert, m_clusterIndex, cluster);
							// add vertex to the cluster's vertex list
							int vertIndex = m_clusterVertices.NewElement();
							m_clusterVertices.SetField(vertIndex, 0, vert);
							m_clusterVertices.SetField(vertIndex, 1, vertFirst);
							vertFirst = vertIndex;
							int path = m_shape.GetPathFromVertex(vert);
							int geometry = m_shape.GetGeometryFromPath(path);
							int geometryID = GetGeometryID(geometry);
							SetClusterParentage_(cluster, GetClusterParentage(cluster) | geometryID);
						}
						SetClusterVertexIterator_(cluster, vertFirst);
						SetClusterVertexIndex_(cluster, m_shape.GetVertexIndex(vert));
						if (m_lastCluster != -1)
						{
							SetNextCluster_(m_lastCluster, cluster);
						}
						SetPrevCluster_(cluster, m_lastCluster);
						m_lastCluster = cluster;
						if (m_firstCluster == -1)
						{
							m_firstCluster = cluster;
						}
					}
					ifirst = i;
					ptFirst.SetCoords(pt);
				}
			}
			com.epl.geometry.ProgressTracker.CheckAndThrow(progress_tracker);
			m_tmpHalfEdgeParentageIndex = CreateUserIndexForHalfEdges();
			if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyWinding)
			{
				m_tmpHalfEdgeWindingNumberIndex = CreateUserIndexForHalfEdges();
			}
			if (inputMode == com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyAlternate)
			{
				m_tmpHalfEdgeOddEvenNumberIndex = CreateUserIndexForHalfEdges();
			}
			CreateHalfEdges_(inputMode, verticesSorter);
			// For each geometry produce
			// clusters and half edges
			if (m_non_simple_result.m_reason != com.epl.geometry.NonSimpleResult.Reason.NotDetermined)
			{
				return;
			}
			SortHalfEdgesByAngle_(inputMode);
			if (m_non_simple_result.m_reason != com.epl.geometry.NonSimpleResult.Reason.NotDetermined)
			{
				return;
			}
			if (!com.epl.geometry.NumberUtils.IsNaN(m_check_dirty_planesweep_tolerance))
			{
				if (!Check_structure_after_dirty_sweep_())
				{
					// checks the edges.
					m_dirty_check_failed = true;
					// set m_dirty_check_failed when an
					// issue is found. We'll rerun the
					// planesweep using robust crack and
					// cluster approach.
					return;
				}
			}
			BuildChains_(inputMode);
			if (m_non_simple_result.m_reason != com.epl.geometry.NonSimpleResult.Reason.NotDetermined)
			{
				return;
			}
			DeleteUserIndexForHalfEdges(m_tmpHalfEdgeParentageIndex);
			m_tmpHalfEdgeParentageIndex = -1;
			if (m_buildChains)
			{
				PlaneSweepParentage_(inputMode, progress_tracker);
			}
			Simplify_(inputMode);
		}

		internal void DeleteEdgeImpl_(int half_edge)
		{
			int halfEdgeNext = GetHalfEdgeNext(half_edge);
			int halfEdgePrev = GetHalfEdgePrev(half_edge);
			int halfEdgeTwin = GetHalfEdgeTwin(half_edge);
			int halfEdgeTwinNext = GetHalfEdgeNext(halfEdgeTwin);
			int halfEdgeTwinPrev = GetHalfEdgePrev(halfEdgeTwin);
			if (halfEdgeNext != halfEdgeTwin)
			{
				SetHalfEdgeNext_(halfEdgeTwinPrev, halfEdgeNext);
				SetHalfEdgePrev_(halfEdgeNext, halfEdgeTwinPrev);
			}
			if (halfEdgePrev != halfEdgeTwin)
			{
				SetHalfEdgeNext_(halfEdgePrev, halfEdgeTwinNext);
				SetHalfEdgePrev_(halfEdgeTwinNext, halfEdgePrev);
			}
			int cluster_1 = GetHalfEdgeOrigin(half_edge);
			int clusterFirstEdge1 = GetClusterHalfEdge(cluster_1);
			if (clusterFirstEdge1 == half_edge)
			{
				if (halfEdgeTwinNext != half_edge)
				{
					SetClusterHalfEdge_(cluster_1, halfEdgeTwinNext);
				}
				else
				{
					SetClusterHalfEdge_(cluster_1, -1);
				}
			}
			// cluster has no more edges
			int cluster2 = GetHalfEdgeOrigin(halfEdgeTwin);
			int clusterFirstEdge2 = GetClusterHalfEdge(cluster2);
			if (clusterFirstEdge2 == halfEdgeTwin)
			{
				if (halfEdgeNext != halfEdgeTwin)
				{
					SetClusterHalfEdge_(cluster2, halfEdgeNext);
				}
				else
				{
					SetClusterHalfEdge_(cluster2, -1);
				}
			}
			// cluster has no more edges
			m_halfEdgeData.DeleteElement(half_edge);
			m_halfEdgeData.DeleteElement(halfEdgeTwin);
		}

		internal int GetLeftSkipPolylines_(com.epl.geometry.Treap aet, int treeNode)
		{
			int leftNode = treeNode;
			for (; ; )
			{
				leftNode = aet.GetPrev(leftNode);
				if (leftNode != -1)
				{
					int e = aet.GetElement(leftNode);
					int leftChain = GetHalfEdgeChain(e);
					if (leftChain != GetHalfEdgeChain(GetHalfEdgeTwin(e)))
					{
						return e;
					}
				}
				else
				{
					// the left edge is a piece of polyline - does not
					// contribute to the face parentage
					return -1;
				}
			}
		}

		internal TopoGraph()
		{
			c_edgeParentageMask = ((int)-1) ^ ((int)1 << (com.epl.geometry.NumberUtils.SizeOf((int)0) * 8 - 1));
			c_edgeBitMask = (int)1 << (com.epl.geometry.NumberUtils.SizeOf((int)0) * 8 - 1);
			m_firstCluster = -1;
			m_lastCluster = -1;
			m_geometryIDIndex = -1;
			m_clusterIndex = -1;
			m_halfEdgeIndex = -1;
			m_universeChain = -1;
			m_tmpHalfEdgeParentageIndex = -1;
			m_tmpHalfEdgeWindingNumberIndex = -1;
			m_pointCount = 0;
		}

		internal com.epl.geometry.EditShape GetShape()
		{
			return m_shape;
		}

		// Sets an edit shape. The geometry has to be cracked and clustered before
		// calling this!
		internal void SetEditShape(com.epl.geometry.EditShape shape, com.epl.geometry.ProgressTracker progress_tracker)
		{
			SetEditShapeImpl_(shape, com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeBuildGraph, null, progress_tracker, true);
		}

		internal void SetEditShape(com.epl.geometry.EditShape shape, com.epl.geometry.ProgressTracker progress_tracker, bool bBuildChains)
		{
			SetEditShapeImpl_(shape, com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeBuildGraph, null, progress_tracker, bBuildChains);
		}

		internal void SetAndSimplifyEditShapeAlternate(com.epl.geometry.EditShape shape, int geometry, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.AttributeStreamOfInt32 geoms = new com.epl.geometry.AttributeStreamOfInt32(0);
			geoms.Add(geometry);
			SetEditShapeImpl_(shape, com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyAlternate, geoms, progressTracker, shape.GetGeometryType(geometry) == com.epl.geometry.Geometry.Type.Polygon.Value());
		}

		internal void SetAndSimplifyEditShapeWinding(com.epl.geometry.EditShape shape, int geometry, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.AttributeStreamOfInt32 geoms = new com.epl.geometry.AttributeStreamOfInt32(0);
			geoms.Add(geometry);
			SetEditShapeImpl_(shape, com.epl.geometry.TopoGraph.EnumInputMode.enumInputModeSimplifyWinding, geoms, progressTracker, true);
		}

		// Removes shape from the topograph and removes any user index created on
		// the edit shape.
		internal void RemoveShape()
		{
			if (m_shape == null)
			{
				return;
			}
			if (m_geometryIDIndex != -1)
			{
				m_shape.RemoveGeometryUserIndex(m_geometryIDIndex);
				m_geometryIDIndex = -1;
			}
			if (m_clusterIndex != -1)
			{
				m_shape.RemoveUserIndex(m_clusterIndex);
				m_clusterIndex = -1;
			}
			if (m_halfEdgeIndex != -1)
			{
				m_shape.RemoveUserIndex(m_halfEdgeIndex);
				m_halfEdgeIndex = -1;
			}
			if (m_tmpHalfEdgeParentageIndex != -1)
			{
				DeleteUserIndexForHalfEdges(m_tmpHalfEdgeParentageIndex);
				m_tmpHalfEdgeParentageIndex = -1;
			}
			if (m_tmpHalfEdgeWindingNumberIndex != -1)
			{
				DeleteUserIndexForHalfEdges(m_tmpHalfEdgeWindingNumberIndex);
				m_tmpHalfEdgeWindingNumberIndex = -1;
			}
			if (m_tmpHalfEdgeOddEvenNumberIndex != -1)
			{
				DeleteUserIndexForHalfEdges(m_tmpHalfEdgeOddEvenNumberIndex);
				m_tmpHalfEdgeOddEvenNumberIndex = -1;
			}
			m_shape = null;
			m_clusterData.DeleteAll(true);
			m_clusterVertices.DeleteAll(true);
			m_firstCluster = -1;
			m_lastCluster = -1;
			if (m_halfEdgeData != null)
			{
				m_halfEdgeData.DeleteAll(true);
			}
			if (m_edgeIndices != null)
			{
				m_edgeIndices.Clear();
			}
			if (m_clusterIndices != null)
			{
				m_clusterIndices.Clear();
			}
			if (m_chainIndices != null)
			{
				m_chainIndices.Clear();
			}
			if (m_chainData != null)
			{
				m_chainData.DeleteAll(true);
			}
			m_universeChain = -1;
			m_chainAreas = null;
		}

		// Returns a half-edge emanating the cluster. All other half-edges can be
		// visited with:
		// incident_half_edge = getHalfEdgeTwin(half_edge);//get twin of the
		// half_edge, it has the vertex as the end point.
		// emanating_half_edge = getHalfEdgeTwin(incident_half_edge); //get next
		// emanating half-edge
		internal int GetClusterHalfEdge(int cluster)
		{
			return m_clusterData.GetField(cluster, 2);
		}

		// Returns the coordinates of the cluster
		internal void GetXY(int cluster, com.epl.geometry.Point2D pt)
		{
			int vindex = GetClusterVertexIndex_(cluster);
			m_shape.GetXYWithIndex(vindex, pt);
		}

		// Returns parentage mask of the cluster
		internal int GetClusterParentage(int cluster)
		{
			return m_clusterData.GetField(cluster, 1);
		}

		// Returns first cluster in the Topo_graph (has lowest y, x coordinates).
		internal int GetFirstCluster()
		{
			return m_firstCluster;
		}

		// Returns previous cluster in the Topo_graph (in the sorted order of y,x
		// coordinates).
		internal int GetPrevCluster(int cluster)
		{
			return m_clusterData.GetField(cluster, 3);
		}

		// Returns next cluster in the Topo_graph (in the sorted order of y,x
		// coordinates).
		internal int GetNextCluster(int cluster)
		{
			return m_clusterData.GetField(cluster, 4);
		}

		// Returns an exterior chain of a face this cluster belongs to (belongs only
		// to interior). set only for the clusters that are standalone clusters (do
		// not have half-edges with them).
		internal int GetClusterChain(int cluster)
		{
			return m_clusterData.GetField(cluster, 6);
		}

		// Returns iterator for cluster vertices
		internal int GetClusterVertexIterator(int cluster)
		{
			return m_clusterData.GetField(cluster, 7);
		}

		// Increments iterator. Returns -1 if no more vertices in the cluster
		internal int IncrementVertexIterator(int vertexIterator)
		{
			return m_clusterVertices.GetField(vertexIterator, 1);
		}

		// Dereference the iterator
		internal int GetVertexFromVertexIterator(int vertexIterator)
		{
			return m_clusterVertices.GetField(vertexIterator, 0);
		}

		// Returns a user index value for the cluster.
		internal int GetClusterUserIndex(int cluster, int index)
		{
			int i = GetClusterIndex_(cluster);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_clusterIndices[index];
			if (stream.Size() <= i)
			{
				return -1;
			}
			return stream.Read(i);
		}

		// Sets a user index value for the cluster.
		internal void SetClusterUserIndex(int cluster, int index, int value)
		{
			int i = GetClusterIndex_(cluster);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_clusterIndices[index];
			if (stream.Size() <= i)
			{
				stream.Resize(m_clusterData.Size(), -1);
			}
			stream.Write(i, value);
		}

		// Creates a new user index for the cluster. The index values are set to -1.
		internal int CreateUserIndexForClusters()
		{
			if (m_clusterIndices == null)
			{
				m_clusterIndices = new System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32>(3);
			}
			com.epl.geometry.AttributeStreamOfInt32 new_stream = new com.epl.geometry.AttributeStreamOfInt32(m_clusterData.Capacity(), -1);
			for (int i = 0, n = m_clusterIndices.Count; i < n; i++)
			{
				if (m_clusterIndices[i] == null)
				{
					m_clusterIndices[i] = new_stream;
					return i;
				}
			}
			m_clusterIndices.Add(new_stream);
			return m_clusterIndices.Count - 1;
		}

		// Deletes user index
		internal void DeleteUserIndexForClusters(int userIndex)
		{
			System.Diagnostics.Debug.Assert((m_clusterIndices[userIndex] != null));
			m_clusterIndices[userIndex] = null;
		}

		// Returns origin of this half edge. To get the other end:
		// incident_half_edge = getHalfEdgeTwin(half_edge);
		// edge_end_point = getHalfEdgeOrigin(incident_half_edge);
		internal int GetHalfEdgeOrigin(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 1);
		}

		// Returns the to point of the half edge
		internal int GetHalfEdgeTo(int half_edge)
		{
			return GetHalfEdgeOrigin(GetHalfEdgeTwin(half_edge));
		}

		// Twin of this halfedge, it has opposite direction and same endpoints
		internal int GetHalfEdgeTwin(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 4);
		}

		// Returns previous halfedge. It ends, where this halfedge starts.
		internal int GetHalfEdgePrev(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 5);
		}

		// Returns next halfedge. It starts, where this halfedge ends.
		internal int GetHalfEdgeNext(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 6);
		}

		// Returns half edge chain. Chain is on the right from the halfedge
		internal int GetHalfEdgeChain(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 2);
		}

		// Returns half edge chain parentage. The call is implemented as as
		// getChainParentage(getHalfEdgeChain());
		internal int GetHalfEdgeFaceParentage(int half_edge)
		{
			return GetChainParentage(m_halfEdgeData.GetField(half_edge, 2));
		}

		// Returns iterator for cluster vertices
		internal int GetHalfEdgeVertexIterator(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 7);
		}

		// Returns the coordinates of the origin of the half_edge
		internal void GetHalfEdgeFromXY(int half_edge, com.epl.geometry.Point2D pt)
		{
			GetXY(GetHalfEdgeOrigin(half_edge), pt);
		}

		// Returns the coordinates of the end of the half_edge
		internal void GetHalfEdgeToXY(int half_edge, com.epl.geometry.Point2D pt)
		{
			GetXY(GetHalfEdgeTo(half_edge), pt);
		}

		// Returns parentage mask of this halfedge. Parentage mask of halfedge and
		// its twin are the same
		internal int GetHalfEdgeParentage(int half_edge)
		{
			return m_halfEdgeData.GetField(half_edge, 3) & c_edgeParentageMask;
		}

		// Returns a user index value for the half edge
		internal int GetHalfEdgeUserIndex(int half_edge, int index)
		{
			int i = GetHalfEdgeIndex_(half_edge);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_edgeIndices[index];
			if (stream.Size() <= i)
			{
				return -1;
			}
			return stream.Read(i);
		}

		// Sets a user index value for a half edge
		internal void SetHalfEdgeUserIndex(int half_edge, int index, int value)
		{
			int i = GetHalfEdgeIndex_(half_edge);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_edgeIndices[index];
			if (stream.Size() <= i)
			{
				stream.Resize(m_halfEdgeData.Size(), -1);
			}
			stream.Write(i, value);
		}

		// create a new user index for half edges. The index values are set to -1.
		internal int CreateUserIndexForHalfEdges()
		{
			if (m_edgeIndices == null)
			{
				m_edgeIndices = new System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32>(3);
			}
			com.epl.geometry.AttributeStreamOfInt32 new_stream = new com.epl.geometry.AttributeStreamOfInt32(m_halfEdgeData.Capacity(), -1);
			for (int i = 0, n = m_edgeIndices.Count; i < n; i++)
			{
				if (m_edgeIndices[i] == null)
				{
					m_edgeIndices[i] = new_stream;
					return i;
				}
			}
			m_edgeIndices.Add(new_stream);
			return m_edgeIndices.Count - 1;
		}

		// Deletes the given user index for half edges
		internal void DeleteUserIndexForHalfEdges(int userIndex)
		{
			System.Diagnostics.Debug.Assert((m_edgeIndices[userIndex] != null));
			m_edgeIndices[userIndex] = null;
		}

		// Deletes the half_edge and it's twin. It works presently when removing a
		// spike only.
		// Returns next valid half-edge, or -1 if no more half edges.
		// Use with care.
		internal int DeleteEdgeInternal_(int half_edge)
		{
			int chain = GetHalfEdgeChain(half_edge);
			int halfEdgeTwin = GetHalfEdgeTwin(half_edge);
			int chainTwin = GetHalfEdgeChain(halfEdgeTwin);
			// This function only works for spikes. These two asserts check for that
			System.Diagnostics.Debug.Assert((chainTwin == chain));
			System.Diagnostics.Debug.Assert((half_edge == GetHalfEdgeNext(halfEdgeTwin) || halfEdgeTwin == GetHalfEdgeNext(half_edge)));
			int n = GetHalfEdgeNext(half_edge);
			if (n == halfEdgeTwin)
			{
				n = GetHalfEdgeNext(n);
				if (n == half_edge)
				{
					n = -1;
				}
			}
			if (GetChainHalfEdge(chain) == half_edge)
			{
				SetChainHalfEdge_(chain, n);
			}
			int chainIndex = GetChainIndex_(chain);
			double v = m_chainAreas.Read(chainIndex);
			if (!com.epl.geometry.NumberUtils.IsNaN(v))
			{
				SetChainArea_(chain, com.epl.geometry.NumberUtils.TheNaN);
				SetChainPerimeter_(chain, com.epl.geometry.NumberUtils.TheNaN);
			}
			UpdateVertexToHalfEdgeConnection_(half_edge, true);
			DeleteEdgeImpl_(half_edge);
			// does not change chain information
			return n;
		}

		// Deletes the halfEdges and their twin. The chains are broken after this
		// call.
		// For every chain the halfedges belong to, it will set the first edge to
		// -1.
		// However, the halfedge will still reference the chain so one can get the
		// parentage information still.
		internal void DeleteEdgesBreakFaces_(com.epl.geometry.AttributeStreamOfInt32 edgesToDelete)
		{
			for (int i = 0, n = edgesToDelete.Size(); i < n; i++)
			{
				int half_edge = edgesToDelete.Get(i);
				int chain = GetHalfEdgeChain(half_edge);
				int halfEdgeTwin = GetHalfEdgeTwin(half_edge);
				int chainTwin = GetHalfEdgeChain(halfEdgeTwin);
				SetChainHalfEdge_(chain, -1);
				SetChainHalfEdge_(chainTwin, -1);
				UpdateVertexToHalfEdgeConnection_(half_edge, true);
				DeleteEdgeImpl_(half_edge);
			}
		}

		internal bool DoesHalfEdgeBelongToAPolygonInterior(int half_edge, int polygonId)
		{
			// Half edge belongs to polygon interior if both it and its twin belong
			// to boundary of faces that have the polygon parentage (the poygon both
			// to the left and to the right of the edge).
			int p_1 = GetHalfEdgeFaceParentage(half_edge);
			int p_2 = GetHalfEdgeFaceParentage(GetHalfEdgeTwin(half_edge));
			return (p_1 & polygonId) != 0 && (p_2 & polygonId) != 0;
		}

		internal bool DoesHalfEdgeBelongToAPolygonExterior(int half_edge, int polygonId)
		{
			// Half edge belongs to polygon interior if both it and its twin belong
			// to boundary of faces that have the polygon parentage (the poygon both
			// to the left and to the right of the edge).
			int p_1 = GetHalfEdgeFaceParentage(half_edge);
			int p_2 = GetHalfEdgeFaceParentage(GetHalfEdgeTwin(half_edge));
			return (p_1 & polygonId) == 0 && (p_2 & polygonId) == 0;
		}

		internal bool DoesHalfEdgeBelongToAPolygonBoundary(int half_edge, int polygonId)
		{
			// Half edge overlaps polygon boundary
			int p_1 = GetHalfEdgeParentage(half_edge);
			return (p_1 & polygonId) != 0;
		}

		internal bool DoesHalfEdgeBelongToAPolylineInterior(int half_edge, int polylineId)
		{
			// Half-edge belongs to a polyline interioir if it has the polyline
			// parentage (1D intersection (aka overlap)).
			int p_1 = GetHalfEdgeParentage(half_edge);
			if ((p_1 & polylineId) != 0)
			{
				return true;
			}
			return false;
		}

		internal bool DoesHalfEdgeBelongToAPolylineExterior(int half_edge, int polylineId)
		{
			// Half-edge belongs to a polyline Exterioir if it does not have the
			// polyline parentage and both its clusters also do not have polyline's
			// parentage (to exclude touch at point).
			int p_1 = GetHalfEdgeParentage(half_edge);
			if ((p_1 & polylineId) == 0)
			{
				int c = GetHalfEdgeOrigin(half_edge);
				int pc = GetClusterParentage(c);
				if ((pc & polylineId) == 0)
				{
					c = GetHalfEdgeTo(half_edge);
					pc = GetClusterParentage(c);
					if ((pc & polylineId) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool DoesClusterBelongToAPolygonInterior(int cluster, int polygonId)
		{
			// cluster belongs to a polygon interior when
			// 1) It is a standalone cluster that has face parentage of this polygon
			// GetClusterFaceParentage()
			// 2) or It is a cluster with half edges attached and
			// a) It is not on the polygon boundrary (get_cluster_parentage)
			// b) Any half edge associated with it has face parentage of the polygon
			// (get_half_edge_face_parentage(getClusterHalfEdge()))
			int chain = GetClusterChain(cluster);
			if (chain != -1)
			{
				if ((GetChainParentage(chain) & polygonId) != 0)
				{
					return true;
				}
			}
			else
			{
				int p_1 = GetClusterParentage(cluster);
				if ((p_1 & polygonId) == 0)
				{
					// not on the polygon boundary
					int half_edge = GetClusterHalfEdge(cluster);
					System.Diagnostics.Debug.Assert((half_edge != -1));
					int p_2 = GetHalfEdgeFaceParentage(half_edge);
					if ((p_2 & polygonId) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool DoesClusterBelongToAPolygonExterior(int cluster, int polygonId)
		{
			int p_1 = GetClusterParentage(cluster);
			if ((p_1 & polygonId) == 0)
			{
				return DoesClusterBelongToAPolygonInterior(cluster, polygonId);
			}
			return false;
		}

		internal bool DoesClusterBelongToAPolygonBoundary(int cluster, int polygonId)
		{
			int p_1 = GetClusterParentage(cluster);
			if ((p_1 & polygonId) != 0)
			{
				return true;
			}
			return false;
		}

		// bool DoesClusterBelongToAPolylineInterioir(int cluster, int polylineId);
		// bool does_cluster_belong_to_a_polyline_exterior(int cluster, int
		// polylineId);
		// bool does_cluster_belong_to_a_polyline_boundary(int cluster, int
		// polylineId);
		// Returns the first chain, which is always the Universe chain.
		internal int GetFirstChain()
		{
			return m_universeChain;
		}

		// Returns the chain half edge.
		internal int GetChainHalfEdge(int chain)
		{
			return m_chainData.GetField(chain, 1);
		}

		// Returns the chain's face parentage. That is the parentage of a face this
		// chain borders with.
		internal int GetChainParentage(int chain)
		{
			return m_chainData.GetField(chain, 2);
		}

		// Returns the parent of the chain (the chain, this chain is inside of).
		internal int GetChainParent(int chain)
		{
			return m_chainData.GetField(chain, 3);
		}

		// Returns the first island chain in that chain. Island chains are always
		// counterclockwise.
		// Each island chain will have its complement chain, which is a chain of a
		// twin of any halfedge of that chain.
		internal int GetChainFirstIsland(int chain)
		{
			return m_chainData.GetField(chain, 4);
		}

		// Returns the first island chain in that chain. Island chains are always
		// counterclockwise.
		internal int GetChainNextInParent(int chain)
		{
			return m_chainData.GetField(chain, 5);
		}

		// Returns the next chain in arbitrary order.
		internal int GetChainNext(int chain)
		{
			return m_chainData.GetField(chain, 7);
		}

		// Returns the area of the chain. The area does not include any islands.
		// +Inf is returned for the universe chain.
		internal double GetChainArea(int chain)
		{
			int chainIndex = GetChainIndex_(chain);
			double v = m_chainAreas.Read(chainIndex);
			if (com.epl.geometry.NumberUtils.IsNaN(v))
			{
				UpdateChainAreaAndPerimeter_(chain);
				v = m_chainAreas.Read(chainIndex);
			}
			return v;
		}

		// Returns the perimeter of the chain (> 0). +Inf is returned for the
		// universe chain.
		internal double GetChainPerimeter(int chain)
		{
			int chainIndex = GetChainIndex_(chain);
			double v = m_chainPerimeters.Read(chainIndex);
			if (com.epl.geometry.NumberUtils.IsNaN(v))
			{
				UpdateChainAreaAndPerimeter_(chain);
				v = m_chainPerimeters.Read(chainIndex);
			}
			return v;
		}

		// Returns a user index value for the chain.
		internal int GetChainUserIndex(int chain, int index)
		{
			int i = GetChainIndex_(chain);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_chainIndices[index];
			if (stream.Size() <= i)
			{
				return -1;
			}
			return stream.Read(i);
		}

		// Sets a user index value for the chain.
		internal void SetChainUserIndex(int chain, int index, int value)
		{
			int i = GetChainIndex_(chain);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_chainIndices[index];
			if (stream.Size() <= i)
			{
				stream.Resize(m_chainData.Size(), -1);
			}
			stream.Write(i, value);
		}

		// Creates a new user index for the chains. The index values are set to -1.
		internal int CreateUserIndexForChains()
		{
			if (m_chainIndices == null)
			{
				m_chainIndices = new System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32>(3);
			}
			com.epl.geometry.AttributeStreamOfInt32 new_stream = new com.epl.geometry.AttributeStreamOfInt32(m_chainData.Capacity(), -1);
			for (int i = 0, n = m_chainIndices.Count; i < n; i++)
			{
				if (m_chainIndices[i] == null)
				{
					m_chainIndices[i] = new_stream;
					return i;
				}
			}
			m_chainIndices.Add(new_stream);
			return m_chainIndices.Count - 1;
		}

		// Deletes user index
		internal void DeleteUserIndexForChains(int userIndex)
		{
			System.Diagnostics.Debug.Assert((m_chainIndices[userIndex] != null));
			m_chainIndices[userIndex] = null;
		}

		// Returns geometry ID mask from the geometry handle.
		// Topo_graph creates a user index for geometries in the shape, which exists
		// until the topo graph is destroyed.
		internal int GetGeometryID(int geometry)
		{
			return m_shape.GetGeometryUserIndex(geometry, m_geometryIDIndex);
		}

		// Returns cluster from vertex handle.
		// Topo_graph creates a user index for vertices in the shape to hold cluster
		// handles. The index exists until the topo graph is destroyed.
		internal int GetClusterFromVertex(int vertex)
		{
			return m_shape.GetUserIndex(vertex, m_clusterIndex);
		}

		internal int GetHalfEdgeFromVertex(int vertex)
		{
			return m_shape.GetUserIndex(vertex, m_halfEdgeIndex);
		}

		// Finds an edge connecting the two clusters. Returns -1 if not found.
		// Could be a slow operation when valency of each cluster is high.
		internal int GetHalfEdgeConnector(int clusterFrom, int clusterTo)
		{
			int first_edge = GetClusterHalfEdge(clusterFrom);
			if (first_edge == -1)
			{
				return -1;
			}
			int edge = first_edge;
			int firstEdgeTo = -1;
			int eTo = -1;
			do
			{
				// Doing two loops in parallel - one on the half-edges attached to the
				// clusterFrom, another - attached to clusterTo.
				if (GetHalfEdgeTo(edge) == clusterTo)
				{
					return edge;
				}
				if (firstEdgeTo == -1)
				{
					firstEdgeTo = GetClusterHalfEdge(clusterTo);
					if (firstEdgeTo == -1)
					{
						return -1;
					}
					eTo = firstEdgeTo;
				}
				if (GetHalfEdgeTo(eTo) == clusterFrom)
				{
					edge = GetHalfEdgeTwin(eTo);
					System.Diagnostics.Debug.Assert((GetHalfEdgeTo(edge) == clusterTo && GetHalfEdgeOrigin(edge) == clusterFrom));
					return edge;
				}
				edge = GetHalfEdgeNext(GetHalfEdgeTwin(edge));
				eTo = GetHalfEdgeNext(GetHalfEdgeTwin(eTo));
			}
			while (edge != first_edge && eTo != firstEdgeTo);
			return -1;
		}

		// Queries segment for the edge (only xy coordinates, no attributes)
		internal void QuerySegmentXY(int half_edge, com.epl.geometry.SegmentBuffer outBuffer)
		{
			outBuffer.CreateLine();
			com.epl.geometry.Segment seg = outBuffer.Get();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			GetHalfEdgeFromXY(half_edge, pt);
			seg.SetStartXY(pt);
			GetHalfEdgeToXY(half_edge, pt);
			seg.SetEndXY(pt);
		}

		internal int CompareEdgeAngles_(int edge1, int edge2)
		{
			if (edge1 == edge2)
			{
				return 0;
			}
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			GetHalfEdgeToXY(edge1, pt_1);
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			GetHalfEdgeToXY(edge2, pt_2);
			if (pt_1.IsEqual(pt_2))
			{
				return 0;
			}
			// overlap case
			com.epl.geometry.Point2D pt10 = new com.epl.geometry.Point2D();
			GetHalfEdgeFromXY(edge1, pt10);
			com.epl.geometry.Point2D v_1 = new com.epl.geometry.Point2D();
			v_1.Sub(pt_1, pt10);
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			v_2.Sub(pt_2, pt10);
			int result = com.epl.geometry.Point2D._compareVectors(v_1, v_2);
			return result;
		}

		internal int CompareEdgeAnglesForPair_(int edge1, int edge2)
		{
			if (edge1 == edge2)
			{
				return 0;
			}
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			GetHalfEdgeToXY(edge1, pt_1);
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			GetHalfEdgeToXY(edge2, pt_2);
			if (pt_1.IsEqual(pt_2))
			{
				return 0;
			}
			// overlap case
			com.epl.geometry.Point2D pt10 = new com.epl.geometry.Point2D();
			GetHalfEdgeFromXY(edge1, pt10);
			com.epl.geometry.Point2D v_1 = new com.epl.geometry.Point2D();
			v_1.Sub(pt_1, pt10);
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			v_2.Sub(pt_2, pt10);
			if (v_2.y >= 0 && v_1.y > 0)
			{
				int result = com.epl.geometry.Point2D._compareVectors(v_1, v_2);
				return result;
			}
			else
			{
				return 0;
			}
		}

		internal bool Check_structure_after_dirty_sweep_()
		{
			// for each cluster go through the cluster half edges and check that
			// min(edge1_length, edge2_length) * angle_between is less than
			// m_check_dirty_planesweep_tolerance.
			// Doing this helps us weed out cases missed by the dirty plane sweep.
			// We do not need absolute accuracy here.
			System.Diagnostics.Debug.Assert((!m_dirty_check_failed));
			System.Diagnostics.Debug.Assert((!com.epl.geometry.NumberUtils.IsNaN(m_check_dirty_planesweep_tolerance)));
			double sqr_tol = com.epl.geometry.MathUtils.Sqr(m_check_dirty_planesweep_tolerance);
			com.epl.geometry.Point2D pt10 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			for (int cluster = GetFirstCluster(); cluster != -1; cluster = GetNextCluster(cluster))
			{
				int first = GetClusterHalfEdge(cluster);
				if (first != -1)
				{
					int edge = first;
					GetHalfEdgeFromXY(edge, pt10);
					GetHalfEdgeToXY(edge, pt_2);
					v_2.Sub(pt_2, pt10);
					double sqr_len2 = v_2.SqrLength();
					do
					{
						int prev = edge;
						edge = GetHalfEdgeNext(GetHalfEdgeTwin(edge));
						if (edge != prev)
						{
							GetHalfEdgeToXY(edge, pt_1);
							System.Diagnostics.Debug.Assert((!pt_1.IsEqual(pt_2)));
							v_1.Sub(pt_1, pt10);
							double sqr_len1 = v_1.SqrLength();
							double cross = v_1.CrossProduct(v_2);
							// cross_prod =
							// len1 * len2 *
							// sinA => sinA
							// = cross_prod
							// / (len1 *
							// len2);
							double sqr_sinA = (cross * cross) / (sqr_len1 * sqr_len2);
							// sqr_sinA is
							// approximately A^2
							// especially for
							// smaller angles
							double sqr_dist = System.Math.Min(sqr_len1, sqr_len2) * sqr_sinA;
							if (sqr_dist <= sqr_tol)
							{
								// these edges incident on the cluster form a narrow
								// wedge and thei require cracking event that was
								// missed.
								return false;
							}
							v_2.SetCoords(v_1);
							sqr_len2 = sqr_len1;
							pt_2.SetCoords(pt_1);
						}
					}
					while (edge != first);
				}
			}
			return true;
		}
	}
}
