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
	internal sealed class TopologicalOperations
	{
		internal com.epl.geometry.TopoGraph m_topo_graph = null;

		internal com.epl.geometry.Point2D m_dummy_pt_1 = new com.epl.geometry.Point2D();

		internal com.epl.geometry.Point2D m_dummy_pt_2 = new com.epl.geometry.Point2D();

		internal int m_from_edge_for_polylines;

		internal bool[] m_mask_lookup = null;

		internal bool m_bOGCOutput = false;

		internal bool IsGoodParentage(int parentage)
		{
			return parentage < m_mask_lookup.Length ? m_mask_lookup[parentage] : false;
		}

		internal void Cut(int sideIndex, int cuttee, int cutter, com.epl.geometry.AttributeStreamOfInt32 cutHandles)
		{
			int gtCuttee = m_topo_graph.GetShape().GetGeometryType(cuttee);
			int gtCutter = m_topo_graph.GetShape().GetGeometryType(cutter);
			int dimCuttee = com.epl.geometry.Geometry.GetDimensionFromType(gtCuttee);
			int dimCutter = com.epl.geometry.Geometry.GetDimensionFromType(gtCutter);
			if (dimCuttee == 2 && dimCutter == 1)
			{
				CutPolygonPolyline_(sideIndex, cuttee, cutter, cutHandles);
				return;
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal sealed class CompareCuts : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			private com.epl.geometry.EditShape m_editShape;

			public CompareCuts(com.epl.geometry.EditShape editShape)
			{
				m_editShape = editShape;
			}

			public override int Compare(int c1, int c2)
			{
				int path1 = m_editShape.GetFirstPath(c1);
				double area1 = m_editShape.GetRingArea(path1);
				int path2 = m_editShape.GetFirstPath(c2);
				double area2 = m_editShape.GetRingArea(path2);
				if (area1 < area2)
				{
					return -1;
				}
				if (area1 == area2)
				{
					return 0;
				}
				return 1;
			}
		}

		public TopologicalOperations()
		{
			m_from_edge_for_polylines = -1;
		}

		internal void SetEditShape(com.epl.geometry.EditShape shape, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (m_topo_graph == null)
			{
				m_topo_graph = new com.epl.geometry.TopoGraph();
			}
			m_topo_graph.SetEditShape(shape, progressTracker);
		}

		internal void SetEditShapeCrackAndCluster(com.epl.geometry.EditShape shape, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.CrackAndCluster.Execute(shape, tolerance, progressTracker, true);
			for (int geometry = shape.GetFirstGeometry(); geometry != -1; geometry = shape.GetNextGeometry(geometry))
			{
				if (shape.GetGeometryType(geometry) == com.epl.geometry.Geometry.Type.Polygon.Value())
				{
					com.epl.geometry.Simplificator.Execute(shape, geometry, -1, m_bOGCOutput, progressTracker);
				}
			}
			SetEditShape(shape, progressTracker);
		}

		private void CollectPolygonPathsPreservingFrom_(int geometryFrom, int newGeometry, int visitedEdges, int visitedClusters, int geometry_dominant)
		{
			// This function tries to create polygon paths using the paths that were
			// in the input shape.
			// This way we preserve original shape as much as possible.
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			if (shape.GetGeometryType(geometryFrom) != com.epl.geometry.Geometry.Type.Polygon.Value())
			{
				return;
			}
			for (int path = shape.GetFirstPath(geometryFrom); path != -1; path = shape.GetNextPath(path))
			{
				int first_vertex = shape.GetFirstVertex(path);
				int firstCluster = m_topo_graph.GetClusterFromVertex(first_vertex);
				System.Diagnostics.Debug.Assert((firstCluster != -1));
				int secondVertex = shape.GetNextVertex(first_vertex);
				int secondCluster = m_topo_graph.GetClusterFromVertex(secondVertex);
				System.Diagnostics.Debug.Assert((secondCluster != -1));
				int firstHalfEdge = m_topo_graph.GetHalfEdgeFromVertex(first_vertex);
				if (firstHalfEdge == -1)
				{
					continue;
				}
				// Usually there will be a half-edge that starts at
				// first_vertex and goes to secondVertex, but it
				// could happen that this half edge has been
				// removed.
				System.Diagnostics.Debug.Assert((m_topo_graph.GetHalfEdgeTo(firstHalfEdge) == secondCluster && m_topo_graph.GetHalfEdgeOrigin(firstHalfEdge) == firstCluster));
				int visited = m_topo_graph.GetHalfEdgeUserIndex(firstHalfEdge, visitedEdges);
				if (visited == 1 || visited == 2)
				{
					continue;
				}
				int parentage = m_topo_graph.GetHalfEdgeFaceParentage(firstHalfEdge);
				if (!IsGoodParentage(parentage))
				{
					m_topo_graph.SetHalfEdgeUserIndex(firstHalfEdge, visitedEdges, 2);
					continue;
				}
				m_topo_graph.SetHalfEdgeUserIndex(firstHalfEdge, visitedEdges, 1);
				int newPath = shape.InsertPath(newGeometry, -1);
				// add new path at
				// the end
				int half_edge = firstHalfEdge;
				int vertex = first_vertex;
				int cluster = m_topo_graph.GetClusterFromVertex(vertex);
				int dir = 1;
				do
				{
					//Walk the chain of half edges, preferably selecting vertices that belong to the
					//polygon path we have started from.
					int vertex_dominant = GetVertexByID_(vertex, geometry_dominant);
					shape.AddVertex(newPath, vertex_dominant);
					if (visitedClusters != -1)
					{
						m_topo_graph.SetClusterUserIndex(cluster, visitedClusters, 1);
					}
					m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 1);
					half_edge = m_topo_graph.GetHalfEdgeNext(half_edge);
					int v;
					int cv;
					do
					{
						// move in a loop through coincident vertices (probably
						// vertical segments).
						v = dir == 1 ? shape.GetNextVertex(vertex) : shape.GetPrevVertex(vertex);
						// if we came to the polyline
						// tail, the next may return
						// -1.
						cv = v != -1 ? m_topo_graph.GetClusterFromVertex(v) : -1;
					}
					while (cv == cluster);
					int originCluster = m_topo_graph.GetHalfEdgeOrigin(half_edge);
					if (originCluster != cv)
					{
						do
						{
							// try going opposite way
							// move in a loop through coincident vertices (probably
							// vertical segments).
							v = dir == 1 ? shape.GetPrevVertex(vertex) : shape.GetNextVertex(vertex);
							// if we came to the
							// polyline tail, the
							// next may return -1.
							cv = v != -1 ? m_topo_graph.GetClusterFromVertex(v) : -1;
						}
						while (cv == cluster);
						if (originCluster != cv)
						{
							// pick any vertex.
							cv = originCluster;
							int iterator = m_topo_graph.GetClusterVertexIterator(cv);
							v = m_topo_graph.GetVertexFromVertexIterator(iterator);
						}
						else
						{
							dir = -dir;
						}
					}
					// remember direction we were going for
					// performance
					cluster = cv;
					vertex = v;
				}
				while (half_edge != firstHalfEdge);
				shape.SetClosedPath(newPath, true);
			}
		}

		// processes Topo_graph and removes edges that border faces with good
		// parentage
		// If bAllowBrokenFaces is True the function will break face structure for
		// dissolved faces. Only face parentage will be uasable.
		internal void DissolveCommonEdges_()
		{
			int visitedEdges = m_topo_graph.CreateUserIndexForHalfEdges();
			com.epl.geometry.AttributeStreamOfInt32 edgesToDelete = new com.epl.geometry.AttributeStreamOfInt32(0);
			// Now extract paths that
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int firstHalfEdge = m_topo_graph.GetClusterHalfEdge(cluster);
				int half_edge = firstHalfEdge;
				if (firstHalfEdge == -1)
				{
					continue;
				}
				do
				{
					int visited = m_topo_graph.GetHalfEdgeUserIndex(half_edge, visitedEdges);
					if (visited != 1)
					{
						int halfEdgeTwin = m_topo_graph.GetHalfEdgeTwin(half_edge);
						m_topo_graph.SetHalfEdgeUserIndex(halfEdgeTwin, visitedEdges, 1);
						m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 1);
						int parentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
						if (IsGoodParentage(parentage))
						{
							int twinParentage = m_topo_graph.GetHalfEdgeFaceParentage(halfEdgeTwin);
							if (IsGoodParentage(twinParentage))
							{
								// This half_edge pair is a border between two faces
								// that share the parentage or it is a dangling edge
								edgesToDelete.Add(half_edge);
							}
						}
					}
					// remember for
					// subsequent delete
					half_edge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(half_edge));
				}
				while (half_edge != firstHalfEdge);
			}
			m_topo_graph.DeleteUserIndexForHalfEdges(visitedEdges);
			m_topo_graph.DeleteEdgesBreakFaces_(edgesToDelete);
		}

		internal int GetVertexByID_(int vertex, int geometry_id)
		{
			if (geometry_id == -1)
			{
				return vertex;
			}
			return GetVertexByIDImpl_(vertex, geometry_id);
		}

		internal int GetVertexByIDImpl_(int vertex, int geometry_id)
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int v;
			int geometry;
			int vertex_iterator = m_topo_graph.GetClusterVertexIterator(m_topo_graph.GetClusterFromVertex(vertex));
			do
			{
				v = m_topo_graph.GetVertexFromVertexIterator(vertex_iterator);
				geometry = shape.GetGeometryFromPath(shape.GetPathFromVertex(v));
				if (geometry == geometry_id)
				{
					return v;
				}
				vertex_iterator = m_topo_graph.IncrementVertexIterator(vertex_iterator);
			}
			while (vertex_iterator != -1);
			return vertex;
		}

		private int TopoOperationPolygonPolygon_(int geometry_a, int geometry_b, int geometry_dominant)
		{
			DissolveCommonEdges_();
			// faces are partially broken after this call.
			// See help to this call.
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int newGeometry = shape.CreateGeometry(com.epl.geometry.Geometry.Type.Polygon);
			int visitedEdges = m_topo_graph.CreateUserIndexForHalfEdges();
			TopoOperationPolygonPolygonHelper_(geometry_a, geometry_b, newGeometry, geometry_dominant, visitedEdges, -1);
			m_topo_graph.DeleteUserIndexForHalfEdges(visitedEdges);
			com.epl.geometry.Simplificator.Execute(shape, newGeometry, com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, m_bOGCOutput, null);
			return newGeometry;
		}

		private void TopoOperationPolygonPolygonHelper_(int geometry_a, int geometry_b, int newGeometryPolygon, int geometry_dominant, int visitedEdges, int visitedClusters)
		{
			CollectPolygonPathsPreservingFrom_(geometry_a, newGeometryPolygon, visitedEdges, visitedClusters, geometry_dominant);
			if (geometry_b != -1)
			{
				CollectPolygonPathsPreservingFrom_(geometry_b, newGeometryPolygon, visitedEdges, visitedClusters, geometry_dominant);
			}
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			// Now extract polygon paths that has not been extracted on the previous
			// step.
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int firstHalfEdge = m_topo_graph.GetClusterHalfEdge(cluster);
				if (firstHalfEdge == -1)
				{
					continue;
				}
				int half_edge = firstHalfEdge;
				do
				{
					int visited = m_topo_graph.GetHalfEdgeUserIndex(half_edge, visitedEdges);
					if (visited != 1 && visited != 2)
					{
						int parentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
						if (IsGoodParentage(parentage))
						{
							// Extract face.
							int newPath = shape.InsertPath(newGeometryPolygon, -1);
							// add
							// new
							// path
							// at
							// the
							// end
							int faceHalfEdge = half_edge;
							do
							{
								int viter = m_topo_graph.GetHalfEdgeVertexIterator(faceHalfEdge);
								int v;
								if (viter != -1)
								{
									v = m_topo_graph.GetVertexFromVertexIterator(viter);
								}
								else
								{
									int viter1 = m_topo_graph.GetHalfEdgeVertexIterator(m_topo_graph.GetHalfEdgeTwin(faceHalfEdge));
									System.Diagnostics.Debug.Assert((viter1 != -1));
									v = m_topo_graph.GetVertexFromVertexIterator(viter1);
									v = m_topo_graph.GetShape().GetNextVertex(v);
								}
								System.Diagnostics.Debug.Assert((v != -1));
								int vertex_dominant = GetVertexByID_(v, geometry_dominant);
								shape.AddVertex(newPath, vertex_dominant);
								System.Diagnostics.Debug.Assert((IsGoodParentage(m_topo_graph.GetHalfEdgeFaceParentage(faceHalfEdge))));
								m_topo_graph.SetHalfEdgeUserIndex(faceHalfEdge, visitedEdges, 1);
								//
								if (visitedClusters != -1)
								{
									int c = m_topo_graph.GetClusterFromVertex(vertex_dominant);
									m_topo_graph.SetClusterUserIndex(c, visitedClusters, 1);
								}
								faceHalfEdge = m_topo_graph.GetHalfEdgeNext(faceHalfEdge);
							}
							while (faceHalfEdge != half_edge);
							shape.SetClosedPath(newPath, true);
						}
						else
						{
							// cannot extract a face
							m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 2);
						}
					}
					half_edge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(half_edge));
				}
				while (half_edge != firstHalfEdge);
			}
		}

		internal int[] TopoOperationPolygonPolygonEx_(int geometry_a, int geometry_b, int geometry_dominant)
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int newGeometryPolygon = shape.CreateGeometry(com.epl.geometry.Geometry.Type.Polygon);
			int newGeometryPolyline = shape.CreateGeometry(com.epl.geometry.Geometry.Type.Polyline);
			int newGeometryMultipoint = shape.CreateGeometry(com.epl.geometry.Geometry.Type.MultiPoint);
			DissolveCommonEdges_();
			// faces are partially broken after this call.
			// See help to this call.
			int multipointPath = -1;
			int visitedEdges = m_topo_graph.CreateUserIndexForHalfEdges();
			int visitedClusters = m_topo_graph.CreateUserIndexForClusters();
			TopoOperationPolygonPolygonHelper_(geometry_a, geometry_b, newGeometryPolygon, geometry_dominant, visitedEdges, visitedClusters);
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int firstHalfEdge = m_topo_graph.GetClusterHalfEdge(cluster);
				if (firstHalfEdge == -1)
				{
					continue;
				}
				int half_edge = firstHalfEdge;
				do
				{
					int visited1 = m_topo_graph.GetHalfEdgeUserIndex(half_edge, visitedEdges);
					int visited2 = m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgeTwin(half_edge), visitedEdges);
					int visited = visited1 | visited2;
					if (visited == 2)
					{
						int parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
						if (IsGoodParentage(parentage))
						{
							// Extract face.
							int newPath = shape.InsertPath(newGeometryPolyline, -1);
							// add
							// new
							// path
							// at
							// the
							// end
							int polyHalfEdge = half_edge;
							int vert = SelectVertex_(cluster, shape);
							System.Diagnostics.Debug.Assert((vert != -1));
							int vertex_dominant = GetVertexByID_(vert, geometry_dominant);
							shape.AddVertex(newPath, vertex_dominant);
							m_topo_graph.SetClusterUserIndex(cluster, visitedClusters, 1);
							do
							{
								int clusterTo = m_topo_graph.GetHalfEdgeTo(polyHalfEdge);
								int vert1 = SelectVertex_(clusterTo, shape);
								System.Diagnostics.Debug.Assert((vert1 != -1));
								int vertex_dominant1 = GetVertexByID_(vert1, geometry_dominant);
								shape.AddVertex(newPath, vertex_dominant1);
								m_topo_graph.SetHalfEdgeUserIndex(polyHalfEdge, visitedEdges, 1);
								//
								m_topo_graph.SetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgeTwin(polyHalfEdge), visitedEdges, 1);
								//
								m_topo_graph.SetClusterUserIndex(clusterTo, visitedClusters, 1);
								polyHalfEdge = m_topo_graph.GetHalfEdgeNext(polyHalfEdge);
								visited1 = m_topo_graph.GetHalfEdgeUserIndex(polyHalfEdge, visitedEdges);
								visited2 = m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgeTwin(polyHalfEdge), visitedEdges);
								visited = visited1 | visited2;
								if (visited != 2)
								{
									break;
								}
								parentage = m_topo_graph.GetHalfEdgeParentage(polyHalfEdge);
								if (!IsGoodParentage(parentage))
								{
									m_topo_graph.SetHalfEdgeUserIndex(polyHalfEdge, visitedEdges, 1);
									m_topo_graph.SetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgeTwin(polyHalfEdge), visitedEdges, 1);
									break;
								}
							}
							while (polyHalfEdge != half_edge);
						}
						else
						{
							m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 1);
							m_topo_graph.SetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgeTwin(half_edge), visitedEdges, 1);
						}
					}
					half_edge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(half_edge));
				}
				while (half_edge != firstHalfEdge);
			}
			for (int cluster_1 = m_topo_graph.GetFirstCluster(); cluster_1 != -1; cluster_1 = m_topo_graph.GetNextCluster(cluster_1))
			{
				int visited = m_topo_graph.GetClusterUserIndex(cluster_1, visitedClusters);
				if (visited == 1)
				{
					continue;
				}
				int parentage = m_topo_graph.GetClusterParentage(cluster_1);
				if (IsGoodParentage(parentage))
				{
					if (multipointPath == -1)
					{
						multipointPath = shape.InsertPath(newGeometryMultipoint, -1);
					}
					int viter = m_topo_graph.GetClusterVertexIterator(cluster_1);
					int v;
					if (viter != -1)
					{
						v = m_topo_graph.GetVertexFromVertexIterator(viter);
						int vertex_dominant = GetVertexByID_(v, geometry_dominant);
						shape.AddVertex(multipointPath, vertex_dominant);
					}
				}
			}
			m_topo_graph.DeleteUserIndexForClusters(visitedClusters);
			m_topo_graph.DeleteUserIndexForHalfEdges(visitedEdges);
			com.epl.geometry.Simplificator.Execute(shape, newGeometryPolygon, com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, m_bOGCOutput, null);
			int[] result = new int[3];
			// always returns size 3 result.
			result[0] = newGeometryMultipoint;
			result[1] = newGeometryPolyline;
			result[2] = newGeometryPolygon;
			return result;
		}

		internal int SelectVertex_(int cluster, com.epl.geometry.EditShape shape)
		{
			int vert = -1;
			for (int iterator = m_topo_graph.GetClusterVertexIterator(cluster); iterator != -1; iterator = m_topo_graph.IncrementVertexIterator(iterator))
			{
				int vertex = m_topo_graph.GetVertexFromVertexIterator(iterator);
				if (vert == -1)
				{
					vert = vertex;
				}
				int geometry = shape.GetGeometryFromPath(shape.GetPathFromVertex(vertex));
				int geomID = m_topo_graph.GetGeometryID(geometry);
				if (IsGoodParentage(geomID))
				{
					vert = vertex;
					break;
				}
			}
			return vert;
		}

		private double PrevailingDirection_(com.epl.geometry.EditShape shape, int half_edge)
		{
			int cluster = m_topo_graph.GetHalfEdgeOrigin(half_edge);
			int clusterTo = m_topo_graph.GetHalfEdgeTo(half_edge);
			int signTotal = 0;
			int signCorrect = 0;
			for (int iterator = m_topo_graph.GetClusterVertexIterator(cluster); iterator != -1; iterator = m_topo_graph.IncrementVertexIterator(iterator))
			{
				int vertex = m_topo_graph.GetVertexFromVertexIterator(iterator);
				int path = shape.GetPathFromVertex(vertex);
				int geometry = shape.GetGeometryFromPath(path);
				int geomID = m_topo_graph.GetGeometryID(geometry);
				int nextVert = shape.GetNextVertex(vertex);
				int prevVert = shape.GetPrevVertex(vertex);
				int firstVert = shape.GetFirstVertex(path);
				if (firstVert == vertex)
				{
					// remember the first half edge of the
					// path. We use it to produce correct
					// startpath for closed polyline loops
					m_from_edge_for_polylines = half_edge;
				}
				if (nextVert != -1 && m_topo_graph.GetClusterFromVertex(nextVert) == clusterTo)
				{
					signTotal++;
					if (IsGoodParentage(geomID))
					{
						if (firstVert == nextVert)
						{
							// remember the first vertex of
							// the path. We use it to
							// produce correct startpath for
							// closed polyline loops
							m_from_edge_for_polylines = m_topo_graph.GetHalfEdgeNext(half_edge);
						}
						// update the sign
						signCorrect++;
					}
				}
				else
				{
					if (prevVert != -1 && m_topo_graph.GetClusterFromVertex(prevVert) == clusterTo)
					{
						signTotal--;
						if (IsGoodParentage(geomID))
						{
							if (firstVert == prevVert)
							{
								// remember the first vertex of
								// the path. We use it to
								// produce correct startpath for
								// closed polyline loops
								m_from_edge_for_polylines = m_topo_graph.GetHalfEdgeNext(half_edge);
							}
							// update the sign
							signCorrect--;
						}
					}
				}
			}
			m_topo_graph.GetXY(cluster, m_dummy_pt_1);
			m_topo_graph.GetXY(clusterTo, m_dummy_pt_2);
			double len = com.epl.geometry.Point2D.Distance(m_dummy_pt_1, m_dummy_pt_2);
			return (signCorrect != 0 ? signCorrect : signTotal) * len;
		}

		internal int GetCombinedHalfEdgeParentage_(int e)
		{
			return m_topo_graph.GetHalfEdgeParentage(e) | m_topo_graph.GetHalfEdgeFaceParentage(e) | m_topo_graph.GetHalfEdgeFaceParentage(m_topo_graph.GetHalfEdgeTwin(e));
		}

		internal int TryMoveThroughCrossroadBackwards_(int half_edge)
		{
			int e = m_topo_graph.GetHalfEdgeTwin(m_topo_graph.GetHalfEdgePrev(half_edge));
			int goodEdge = -1;
			while (e != half_edge)
			{
				int parentage = GetCombinedHalfEdgeParentage_(e);
				if (IsGoodParentage(parentage))
				{
					if (goodEdge != -1)
					{
						return -1;
					}
					goodEdge = e;
				}
				e = m_topo_graph.GetHalfEdgeTwin(m_topo_graph.GetHalfEdgePrev(e));
			}
			return goodEdge != -1 ? m_topo_graph.GetHalfEdgeTwin(goodEdge) : -1;
		}

		internal int TryMoveThroughCrossroadForward_(int half_edge)
		{
			int e = m_topo_graph.GetHalfEdgeTwin(m_topo_graph.GetHalfEdgeNext(half_edge));
			int goodEdge = -1;
			while (e != half_edge)
			{
				int parentage = GetCombinedHalfEdgeParentage_(e);
				if (IsGoodParentage(parentage))
				{
					if (goodEdge != -1)
					{
						return -1;
					}
					// more than one way to move through the
					// intersection
					goodEdge = e;
				}
				e = m_topo_graph.GetHalfEdgeTwin(m_topo_graph.GetHalfEdgeNext(e));
			}
			return goodEdge != -1 ? m_topo_graph.GetHalfEdgeTwin(goodEdge) : -1;
		}

		private void RestorePolylineParts_(int first_edge, int newGeometry, int visitedEdges, int visitedClusters, int geometry_dominant)
		{
			System.Diagnostics.Debug.Assert((IsGoodParentage(GetCombinedHalfEdgeParentage_(first_edge))));
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int half_edge = first_edge;
			int halfEdgeTwin = m_topo_graph.GetHalfEdgeTwin(half_edge);
			m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 1);
			m_topo_graph.SetHalfEdgeUserIndex(halfEdgeTwin, visitedEdges, 1);
			double prevailingLength = PrevailingDirection_(shape, half_edge);
			// prevailing
			// direction
			// is
			// used
			// to
			// figure
			// out
			// the
			// polyline
			// direction.
			// Prevailing length is the sum of the length of vectors that constitute
			// the polyline.
			// Vector length is positive, if the halfedge direction coincides with
			// the direction of the original geometry
			// and negative otherwise.
			m_from_edge_for_polylines = -1;
			int fromEdge = half_edge;
			int toEdge = -1;
			bool b_found_impassable_crossroad = false;
			int edgeCount = 1;
			while (true)
			{
				int halfEdgePrev = m_topo_graph.GetHalfEdgePrev(half_edge);
				if (halfEdgePrev == halfEdgeTwin)
				{
					break;
				}
				// the end of a polyline
				int halfEdgeTwinNext = m_topo_graph.GetHalfEdgeNext(halfEdgeTwin);
				if (m_topo_graph.GetHalfEdgeTwin(halfEdgePrev) != halfEdgeTwinNext)
				{
					// Crossroads is here. We can move through the crossroad only if
					// there is only a single way to pass through.
					//When doing planar_simplify we'll never go through the crossroad.
					half_edge = TryMoveThroughCrossroadBackwards_(half_edge);
					if (half_edge == -1)
					{
						break;
					}
					else
					{
						b_found_impassable_crossroad = true;
						halfEdgeTwin = m_topo_graph.GetHalfEdgeTwin(half_edge);
					}
				}
				else
				{
					half_edge = halfEdgePrev;
					halfEdgeTwin = halfEdgeTwinNext;
				}
				if (half_edge == first_edge)
				{
					// we are in a loop. No need to search
					// for the toEdge. Just remember the
					// toEdge and skip the next while
					// loop.
					toEdge = first_edge;
					break;
				}
				int parentage = GetCombinedHalfEdgeParentage_(half_edge);
				if (!IsGoodParentage(parentage))
				{
					break;
				}
				m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 1);
				m_topo_graph.SetHalfEdgeUserIndex(halfEdgeTwin, visitedEdges, 1);
				fromEdge = half_edge;
				prevailingLength += PrevailingDirection_(shape, half_edge);
				edgeCount++;
			}
			if (toEdge == -1)
			{
				half_edge = first_edge;
				halfEdgeTwin = m_topo_graph.GetHalfEdgeTwin(half_edge);
				toEdge = half_edge;
				while (true)
				{
					int halfEdgeNext = m_topo_graph.GetHalfEdgeNext(half_edge);
					if (halfEdgeNext == halfEdgeTwin)
					{
						break;
					}
					int halfEdgeTwinPrev = m_topo_graph.GetHalfEdgePrev(halfEdgeTwin);
					if (m_topo_graph.GetHalfEdgeTwin(halfEdgeNext) != halfEdgeTwinPrev)
					{
						// Crossroads is here. We can move through the crossroad
						// only if there is only a single way to pass through.
						half_edge = TryMoveThroughCrossroadForward_(half_edge);
						if (half_edge == -1)
						{
							b_found_impassable_crossroad = true;
							break;
						}
						else
						{
							halfEdgeTwin = m_topo_graph.GetHalfEdgeTwin(half_edge);
						}
					}
					else
					{
						half_edge = halfEdgeNext;
						halfEdgeTwin = halfEdgeTwinPrev;
					}
					int parentage = GetCombinedHalfEdgeParentage_(half_edge);
					if (!IsGoodParentage(parentage))
					{
						break;
					}
					m_topo_graph.SetHalfEdgeUserIndex(half_edge, visitedEdges, 1);
					m_topo_graph.SetHalfEdgeUserIndex(halfEdgeTwin, visitedEdges, 1);
					toEdge = half_edge;
					prevailingLength += PrevailingDirection_(shape, half_edge);
					edgeCount++;
				}
			}
			else
			{
				// toEdge has been found in the first while loop. This happens when
				// we go around a face.
				// Closed loops need special processing as we do not know where the
				// polyline started or ended.
				if (m_from_edge_for_polylines != -1)
				{
					fromEdge = m_from_edge_for_polylines;
					toEdge = m_topo_graph.GetHalfEdgePrev(m_from_edge_for_polylines);
					// try
					// simply
					// getting
					// prev
					int fromEdgeTwin = m_topo_graph.GetHalfEdgeTwin(fromEdge);
					int fromEdgeTwinNext = m_topo_graph.GetHalfEdgeNext(fromEdgeTwin);
					if (m_topo_graph.GetHalfEdgeTwin(toEdge) != fromEdgeTwinNext)
					{
						// Crossroads is here. Pass through the crossroad.
						toEdge = TryMoveThroughCrossroadBackwards_(fromEdge);
						if (toEdge == -1)
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
					// what?
					System.Diagnostics.Debug.Assert((IsGoodParentage(GetCombinedHalfEdgeParentage_(m_from_edge_for_polylines))));
					System.Diagnostics.Debug.Assert((IsGoodParentage(GetCombinedHalfEdgeParentage_(toEdge))));
				}
			}
			bool dir = prevailingLength >= 0;
			if (!dir)
			{
				int e = toEdge;
				toEdge = fromEdge;
				fromEdge = e;
				toEdge = m_topo_graph.GetHalfEdgeTwin(toEdge);
				// switch to twin so
				// that we can use
				// next instead of
				// Prev
				System.Diagnostics.Debug.Assert((IsGoodParentage(GetCombinedHalfEdgeParentage_(toEdge))));
				fromEdge = m_topo_graph.GetHalfEdgeTwin(fromEdge);
				System.Diagnostics.Debug.Assert((IsGoodParentage(GetCombinedHalfEdgeParentage_(fromEdge))));
			}
			int newPath = shape.InsertPath(newGeometry, -1);
			// add new path at the
			// end
			half_edge = fromEdge;
			int cluster = m_topo_graph.GetHalfEdgeOrigin(fromEdge);
			int clusterLast = m_topo_graph.GetHalfEdgeTo(toEdge);
			bool b_closed = clusterLast == cluster;
			// The linestrings can touch at boundary points only, while closed path
			// has no boundary, therefore no other path can touch it.
			// Therefore, if a closed path touches another path, we need to split
			// the closed path in two to make the result OGC simple.
			bool b_closed_linestring_touches_other_linestring = b_closed && b_found_impassable_crossroad;
			int vert = SelectVertex_(cluster, shape);
			System.Diagnostics.Debug.Assert((vert != -1));
			int vertex_dominant = GetVertexByID_(vert, geometry_dominant);
			shape.AddVertex(newPath, vertex_dominant);
			if (visitedClusters != -1)
			{
				m_topo_graph.SetClusterUserIndex(cluster, visitedClusters, 1);
			}
			int counter = 0;
			int splitAt = b_closed_linestring_touches_other_linestring ? (edgeCount + 1) / 2 : -1;
			while (true)
			{
				int clusterTo = m_topo_graph.GetHalfEdgeTo(half_edge);
				int vert_1 = SelectVertex_(clusterTo, shape);
				vertex_dominant = GetVertexByID_(vert_1, geometry_dominant);
				shape.AddVertex(newPath, vertex_dominant);
				counter++;
				if (visitedClusters != -1)
				{
					m_topo_graph.SetClusterUserIndex(clusterTo, visitedClusters, 1);
				}
				if (b_closed_linestring_touches_other_linestring && counter == splitAt)
				{
					newPath = shape.InsertPath(newGeometry, -1);
					// add new path at
					// the end
					shape.AddVertex(newPath, vertex_dominant);
				}
				System.Diagnostics.Debug.Assert((IsGoodParentage(GetCombinedHalfEdgeParentage_(half_edge))));
				if (half_edge == toEdge)
				{
					break;
				}
				int halfEdgeNext = m_topo_graph.GetHalfEdgeNext(half_edge);
				if (m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)) != m_topo_graph.GetHalfEdgeTwin(halfEdgeNext))
				{
					// crossroads.
					half_edge = TryMoveThroughCrossroadForward_(half_edge);
					if (half_edge == -1)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
				}
				else
				{
					// a bug. This
					// shoulf
					// never
					// happen
					half_edge = halfEdgeNext;
				}
			}
		}

		private int TopoOperationPolylinePolylineOrPolygon_(int geometry_dominant)
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int newGeometry = shape.CreateGeometry(com.epl.geometry.Geometry.Type.Polyline);
			int visitedEdges = m_topo_graph.CreateUserIndexForHalfEdges();
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int firstClusterHalfEdge = m_topo_graph.GetClusterHalfEdge(cluster);
				int clusterHalfEdge = firstClusterHalfEdge;
				do
				{
					int visited = m_topo_graph.GetHalfEdgeUserIndex(clusterHalfEdge, visitedEdges);
					if (visited != 1)
					{
						int parentage = GetCombinedHalfEdgeParentage_(clusterHalfEdge);
						if (IsGoodParentage(parentage))
						{
							RestorePolylineParts_(clusterHalfEdge, newGeometry, visitedEdges, -1, geometry_dominant);
						}
					}
					//
					clusterHalfEdge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(clusterHalfEdge));
				}
				while (clusterHalfEdge != firstClusterHalfEdge);
			}
			m_topo_graph.DeleteUserIndexForHalfEdges(visitedEdges);
			return newGeometry;
		}

		internal int[] TopoOperationPolylinePolylineOrPolygonEx_(int geometry_dominant)
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int newPolyline = shape.CreateGeometry(com.epl.geometry.Geometry.Type.Polyline);
			int newMultipoint = shape.CreateGeometry(com.epl.geometry.Geometry.Type.MultiPoint);
			int visitedEdges = m_topo_graph.CreateUserIndexForHalfEdges();
			int visitedClusters = m_topo_graph.CreateUserIndexForClusters();
			int multipointPath = -1;
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int firstClusterHalfEdge = m_topo_graph.GetClusterHalfEdge(cluster);
				int clusterHalfEdge = firstClusterHalfEdge;
				do
				{
					int visited = m_topo_graph.GetHalfEdgeUserIndex(clusterHalfEdge, visitedEdges);
					if (visited != 1)
					{
						int parentage = GetCombinedHalfEdgeParentage_(clusterHalfEdge);
						if (IsGoodParentage(parentage))
						{
							RestorePolylineParts_(clusterHalfEdge, newPolyline, visitedEdges, visitedClusters, geometry_dominant);
						}
					}
					//
					clusterHalfEdge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(clusterHalfEdge));
				}
				while (clusterHalfEdge != firstClusterHalfEdge);
			}
			for (int cluster_1 = m_topo_graph.GetFirstCluster(); cluster_1 != -1; cluster_1 = m_topo_graph.GetNextCluster(cluster_1))
			{
				int visited = m_topo_graph.GetClusterUserIndex(cluster_1, visitedClusters);
				if (visited != 1)
				{
					int parentage = m_topo_graph.GetClusterParentage(cluster_1);
					if (IsGoodParentage(parentage))
					{
						if (multipointPath == -1)
						{
							multipointPath = shape.InsertPath(newMultipoint, -1);
						}
						int viter = m_topo_graph.GetClusterVertexIterator(cluster_1);
						int v;
						if (viter != -1)
						{
							v = m_topo_graph.GetVertexFromVertexIterator(viter);
							int vertex_dominant = GetVertexByID_(v, geometry_dominant);
							shape.AddVertex(multipointPath, vertex_dominant);
						}
					}
				}
			}
			//
			m_topo_graph.DeleteUserIndexForHalfEdges(visitedEdges);
			m_topo_graph.DeleteUserIndexForClusters(visitedClusters);
			int[] result = new int[2];
			result[0] = newMultipoint;
			result[1] = newPolyline;
			return result;
		}

		private int TopoOperationMultiPoint_()
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int newGeometry = shape.CreateGeometry(com.epl.geometry.Geometry.Type.MultiPoint);
			int newPath = shape.InsertPath(newGeometry, -1);
			// add new path at the
			// end
			// Now extract paths that
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int parentage = m_topo_graph.GetClusterParentage(cluster);
				if (IsGoodParentage(parentage))
				{
					int vert = -1;
					for (int iterator = m_topo_graph.GetClusterVertexIterator(cluster); iterator != -1; iterator = m_topo_graph.IncrementVertexIterator(iterator))
					{
						int vertex = m_topo_graph.GetVertexFromVertexIterator(iterator);
						if (vert == -1)
						{
							vert = vertex;
						}
						int geometry = shape.GetGeometryFromPath(shape.GetPathFromVertex(vertex));
						int geomID = m_topo_graph.GetGeometryID(geometry);
						if (IsGoodParentage(geomID))
						{
							vert = vertex;
							break;
						}
					}
					System.Diagnostics.Debug.Assert((vert != -1));
					shape.AddVertex(newPath, vert);
				}
			}
			return newGeometry;
		}

		internal void InitMaskLookupArray_(int len)
		{
			m_mask_lookup = new bool[len];
			for (int i = 0; i < len; i++)
			{
				m_mask_lookup[i] = false;
			}
		}

		internal static com.epl.geometry.MultiPoint ProcessMultiPointIntersectOrDiff_(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Geometry intersector, double tolerance, bool bClipIn)
		{
			com.epl.geometry.MultiPoint multi_point_out = ((com.epl.geometry.MultiPoint)multi_point.CreateInstance());
			com.epl.geometry.Point2D[] input_points = new com.epl.geometry.Point2D[1000];
			com.epl.geometry.PolygonUtils.PiPResult[] test_results = new com.epl.geometry.PolygonUtils.PiPResult[1000];
			int npoints = multi_point.GetPointCount();
			bool bFirstOut = true;
			bool bArea = (intersector.GetDimension() == 2);
			if (intersector.GetDimension() != 1 && intersector.GetDimension() != 2)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			for (int ipoints = 0; ipoints < npoints; )
			{
				int num = multi_point.QueryCoordinates(input_points, 1000, ipoints, -1) - ipoints;
				if (bArea)
				{
					com.epl.geometry.PolygonUtils.TestPointsInArea2D(intersector, input_points, (int)num, tolerance, test_results);
				}
				else
				{
					com.epl.geometry.PolygonUtils.TestPointsOnLine2D(intersector, input_points, (int)num, tolerance, test_results);
				}
				int i0 = 0;
				for (int i = 0; i < num; i++)
				{
					bool bTest = test_results[i] == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
					if (!bClipIn)
					{
						bTest = !bTest;
					}
					if (bTest)
					{
						if (bFirstOut)
						{
							bFirstOut = false;
							multi_point_out.Add(multi_point, 0, ipoints);
						}
						if (i0 != i)
						{
							multi_point_out.Add(multi_point, ipoints + i0, ipoints + i);
						}
						i0 = i + 1;
					}
				}
				if (!bFirstOut && i0 != num)
				{
					multi_point_out.Add(multi_point, ipoints + i0, ipoints + num);
				}
				ipoints += num;
			}
			if (bFirstOut)
			{
				return multi_point;
			}
			return multi_point_out;
		}

		internal static com.epl.geometry.MultiPoint Intersection(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Geometry multi_path, double tolerance)
		{
			return ProcessMultiPointIntersectOrDiff_(multi_point, multi_path, tolerance, true);
		}

		internal static com.epl.geometry.MultiPoint Difference(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Geometry multi_path, double tolerance)
		{
			return ProcessMultiPointIntersectOrDiff_(multi_point, multi_path, tolerance, false);
		}

		internal static com.epl.geometry.Point ProcessPointIntersectOrDiff_(com.epl.geometry.Point point, com.epl.geometry.Geometry intersector, double tolerance, bool bClipIn)
		{
			if (point.IsEmpty())
			{
				return ((com.epl.geometry.Point)point.CreateInstance());
			}
			if (intersector.IsEmpty())
			{
				return bClipIn ? ((com.epl.geometry.Point)point.CreateInstance()) : null;
			}
			com.epl.geometry.Point2D[] input_points = new com.epl.geometry.Point2D[1];
			com.epl.geometry.PolygonUtils.PiPResult[] test_results = new com.epl.geometry.PolygonUtils.PiPResult[1];
			bool bArea = intersector.GetDimension() == 2;
			if (intersector.GetDimension() != 1 && intersector.GetDimension() != 2)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			input_points[0] = point.GetXY();
			if (bArea)
			{
				com.epl.geometry.PolygonUtils.TestPointsInArea2D(intersector, input_points, 1, tolerance, test_results);
			}
			else
			{
				com.epl.geometry.PolygonUtils.TestPointsOnLine2D(intersector, input_points, 1, tolerance, test_results);
			}
			bool bTest = test_results[0] == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
			if (!bClipIn)
			{
				bTest = !bTest;
			}
			if (!bTest)
			{
				return point;
			}
			else
			{
				return ((com.epl.geometry.Point)point.CreateInstance());
			}
		}

		internal static com.epl.geometry.Point Intersection(com.epl.geometry.Point point, com.epl.geometry.Geometry geom, double tolerance)
		{
			return ProcessPointIntersectOrDiff_(point, geom, tolerance, true);
		}

		internal static com.epl.geometry.Point Difference(com.epl.geometry.Point point, com.epl.geometry.Geometry geom, double tolerance)
		{
			return ProcessPointIntersectOrDiff_(point, geom, tolerance, false);
		}

		internal static com.epl.geometry.Point Intersection(com.epl.geometry.Point point, com.epl.geometry.Point point2, double tolerance)
		{
			if (point.IsEmpty() || point2.IsEmpty())
			{
				return (com.epl.geometry.Point)point.CreateInstance();
			}
			if (com.epl.geometry.CrackAndCluster.Non_empty_points_need_to_cluster(tolerance, point, point2))
			{
				return com.epl.geometry.CrackAndCluster.Cluster_non_empty_points(point, point2, 1, 1, 1, 1);
			}
			return (com.epl.geometry.Point)point.CreateInstance();
		}

		internal static com.epl.geometry.Point Difference(com.epl.geometry.Point point, com.epl.geometry.Point point2, double tolerance)
		{
			if (point.IsEmpty())
			{
				return (com.epl.geometry.Point)point.CreateInstance();
			}
			if (point2.IsEmpty())
			{
				return point;
			}
			if (com.epl.geometry.CrackAndCluster.Non_empty_points_need_to_cluster(tolerance, point, point2))
			{
				return (com.epl.geometry.Point)point.CreateInstance();
			}
			return point;
		}

		internal com.epl.geometry.MultiVertexGeometry PlanarSimplifyImpl_(com.epl.geometry.MultiVertexGeometry input_geom, double tolerance, bool b_use_winding_rule_for_polygons, bool dirty_result, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (input_geom.IsEmpty())
			{
				return input_geom;
			}
			com.epl.geometry.EditShape shape = new com.epl.geometry.EditShape();
			int geom = shape.AddGeometry(input_geom);
			return PlanarSimplify(shape, geom, tolerance, b_use_winding_rule_for_polygons, dirty_result, progress_tracker);
		}

		internal com.epl.geometry.MultiVertexGeometry PlanarSimplify(com.epl.geometry.EditShape shape, int geom, double tolerance, bool b_use_winding_rule_for_polygons, bool dirty_result, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// This method will produce a polygon from a polyline when
			// b_use_winding_rule_for_polygons is true. This is used by buffer.
			m_topo_graph = new com.epl.geometry.TopoGraph();
			try
			{
				if (dirty_result && shape.GetGeometryType(geom) != com.epl.geometry.Geometry.Type.MultiPoint.Value())
				{
					com.epl.geometry.PlaneSweepCrackerHelper plane_sweeper = new com.epl.geometry.PlaneSweepCrackerHelper();
					plane_sweeper.SweepVertical(shape, tolerance);
					if (plane_sweeper.HadCompications())
					{
						// shame. The one pass
						// planesweep had some
						// complications. Need to do
						// full crack and cluster.
						com.epl.geometry.CrackAndCluster.Execute(shape, tolerance, progress_tracker, true);
						dirty_result = false;
					}
					else
					{
						m_topo_graph.Check_dirty_planesweep(tolerance);
					}
				}
				else
				{
					com.epl.geometry.CrackAndCluster.Execute(shape, tolerance, progress_tracker, true);
					dirty_result = false;
				}
				if (!b_use_winding_rule_for_polygons || shape.GetGeometryType(geom) == com.epl.geometry.Geometry.Type.MultiPoint.Value())
				{
					m_topo_graph.SetAndSimplifyEditShapeAlternate(shape, geom, progress_tracker);
				}
				else
				{
					m_topo_graph.SetAndSimplifyEditShapeWinding(shape, geom, progress_tracker);
				}
				if (m_topo_graph.Dirty_check_failed())
				{
					// we ran the sweep_vertical() before and it produced some
					// issues that where detected by topo graph only.
					System.Diagnostics.Debug.Assert((dirty_result));
					m_topo_graph.RemoveShape();
					m_topo_graph = null;
					// that's at most two level recursion
					return PlanarSimplify(shape, geom, tolerance, b_use_winding_rule_for_polygons, false, progress_tracker);
				}
				//can proceed
				m_topo_graph.Check_dirty_planesweep(com.epl.geometry.NumberUtils.TheNaN);
				int ID_a = m_topo_graph.GetGeometryID(geom);
				InitMaskLookupArray_((ID_a) + 1);
				m_mask_lookup[ID_a] = true;
				// Works only when there is a single
				// geometry in the edit shape.
				// To make it work when many geometries are present, this need to be
				// modified.
				if (shape.GetGeometryType(geom) == com.epl.geometry.Geometry.Type.Polygon.Value() || (b_use_winding_rule_for_polygons && shape.GetGeometryType(geom) != com.epl.geometry.Geometry.Type.MultiPoint.Value()))
				{
					// geom can be a polygon or a polyline.
					// It can be a polyline only when the winding rule is true.
					shape.SetFillRule(geom, com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
					int resGeom = TopoOperationPolygonPolygon_(geom, -1, -1);
					com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)shape.GetGeometry(resGeom);
					polygon.SetFillRule(com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
					//standardize the fill rule.
					if (!dirty_result)
					{
						((com.epl.geometry.MultiVertexGeometryImpl)polygon._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
						((com.epl.geometry.MultiPathImpl)polygon._getImpl())._updateOGCFlags();
					}
					else
					{
						((com.epl.geometry.MultiVertexGeometryImpl)polygon._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, 0.0, false);
					}
					// dirty result means
					// simple but with 0
					// tolerance.
					return polygon;
				}
				else
				{
					if (shape.GetGeometryType(geom) == com.epl.geometry.Geometry.Type.Polyline.Value())
					{
						int resGeom = TopoOperationPolylinePolylineOrPolygon_(-1);
						com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)shape.GetGeometry(resGeom);
						if (!dirty_result)
						{
							((com.epl.geometry.MultiVertexGeometryImpl)polyline._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
						}
						return polyline;
					}
					else
					{
						if (shape.GetGeometryType(geom) == com.epl.geometry.Geometry.Type.MultiPoint.Value())
						{
							int resGeom = TopoOperationMultiPoint_();
							com.epl.geometry.MultiPoint mp = (com.epl.geometry.MultiPoint)shape.GetGeometry(resGeom);
							if (!dirty_result)
							{
								((com.epl.geometry.MultiVertexGeometryImpl)mp._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
							}
							return mp;
						}
						else
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
				}
			}
			finally
			{
				m_topo_graph.RemoveShape();
			}
		}

		// static
		internal static com.epl.geometry.MultiVertexGeometry PlanarSimplify(com.epl.geometry.MultiVertexGeometry input_geom, double tolerance, bool use_winding_rule_for_polygons, bool dirty_result, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			return topoOps.PlanarSimplifyImpl_(input_geom, tolerance, use_winding_rule_for_polygons, dirty_result, progress_tracker);
		}

		internal bool PlanarSimplifyNoCrackingAndCluster(bool OGCoutput, com.epl.geometry.EditShape shape, int geom, com.epl.geometry.ProgressTracker progress_tracker)
		{
			m_bOGCOutput = OGCoutput;
			m_topo_graph = new com.epl.geometry.TopoGraph();
			int rule = shape.GetFillRule(geom);
			int gt = shape.GetGeometryType(geom);
			if (rule != com.epl.geometry.Polygon.FillRule.enumFillRuleWinding || gt == com.epl.geometry.Geometry.GeometryType.MultiPoint)
			{
				m_topo_graph.SetAndSimplifyEditShapeAlternate(shape, geom, progress_tracker);
			}
			else
			{
				m_topo_graph.SetAndSimplifyEditShapeWinding(shape, geom, progress_tracker);
			}
			if (m_topo_graph.Dirty_check_failed())
			{
				return false;
			}
			m_topo_graph.Check_dirty_planesweep(com.epl.geometry.NumberUtils.TheNaN);
			int ID_a = m_topo_graph.GetGeometryID(geom);
			InitMaskLookupArray_((ID_a) + 1);
			m_mask_lookup[ID_a] = true;
			//Works only when there is a single geometry in the edit shape.
			//To make it work when many geometries are present, this need to be modified.
			if (shape.GetGeometryType(geom) == com.epl.geometry.Geometry.GeometryType.Polygon || (rule == com.epl.geometry.Polygon.FillRule.enumFillRuleWinding && shape.GetGeometryType(geom) != com.epl.geometry.Geometry.GeometryType.MultiPoint))
			{
				//geom can be a polygon or a polyline.
				//It can be a polyline only when the winding rule is true.
				shape.SetFillRule(geom, com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
				int resGeom = TopoOperationPolygonPolygon_(geom, -1, -1);
				shape.SwapGeometry(resGeom, geom);
				shape.RemoveGeometry(resGeom);
			}
			else
			{
				if (shape.GetGeometryType(geom) == com.epl.geometry.Geometry.GeometryType.Polyline)
				{
					int resGeom = TopoOperationPolylinePolylineOrPolygon_(-1);
					shape.SwapGeometry(resGeom, geom);
					shape.RemoveGeometry(resGeom);
				}
				else
				{
					if (shape.GetGeometryType(geom) == com.epl.geometry.Geometry.GeometryType.MultiPoint)
					{
						int resGeom = TopoOperationMultiPoint_();
						shape.SwapGeometry(resGeom, geom);
						shape.RemoveGeometry(resGeom);
					}
					else
					{
						throw new com.epl.geometry.GeometryException("internal error");
					}
				}
			}
			return true;
		}

		internal static com.epl.geometry.MultiVertexGeometry SimplifyOGC(com.epl.geometry.MultiVertexGeometry input_geom, double tolerance, bool dirty_result, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			topoOps.m_bOGCOutput = true;
			return topoOps.PlanarSimplifyImpl_(input_geom, tolerance, false, dirty_result, progress_tracker);
		}

		public int Difference(int geometry_a, int geometry_b)
		{
			int gtA = m_topo_graph.GetShape().GetGeometryType(geometry_a);
			int gtB = m_topo_graph.GetShape().GetGeometryType(geometry_b);
			int dim_a = com.epl.geometry.Geometry.GetDimensionFromType(gtA);
			int dim_b = com.epl.geometry.Geometry.GetDimensionFromType(gtB);
			if (dim_a > dim_b)
			{
				return geometry_a;
			}
			int ID_a = m_topo_graph.GetGeometryID(geometry_a);
			int ID_b = m_topo_graph.GetGeometryID(geometry_b);
			InitMaskLookupArray_((ID_a | ID_b) + 1);
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_a)] = true;
			if (dim_a == 2 && dim_b == 2)
			{
				return TopoOperationPolygonPolygon_(geometry_a, geometry_b, -1);
			}
			if (dim_a == 1 && dim_b == 2)
			{
				return TopoOperationPolylinePolylineOrPolygon_(-1);
			}
			if (dim_a == 1 && dim_b == 1)
			{
				return TopoOperationPolylinePolylineOrPolygon_(-1);
			}
			if (dim_a == 0)
			{
				return TopoOperationMultiPoint_();
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal int Dissolve(int geometry_a, int geometry_b)
		{
			int gtA = m_topo_graph.GetShape().GetGeometryType(geometry_a);
			int gtB = m_topo_graph.GetShape().GetGeometryType(geometry_b);
			int dim_a = com.epl.geometry.Geometry.GetDimensionFromType(gtA);
			int dim_b = com.epl.geometry.Geometry.GetDimensionFromType(gtB);
			if (dim_a > dim_b)
			{
				return geometry_a;
			}
			if (dim_a < dim_b)
			{
				return geometry_b;
			}
			int ID_a = m_topo_graph.GetGeometryID(geometry_a);
			int ID_b = m_topo_graph.GetGeometryID(geometry_b);
			InitMaskLookupArray_(((ID_a | ID_b) + 1));
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_a)] = true;
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_b)] = true;
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_a) | m_topo_graph.GetGeometryID(geometry_b)] = true;
			if (dim_a == 2 && dim_b == 2)
			{
				return TopoOperationPolygonPolygon_(geometry_a, geometry_b, -1);
			}
			if (dim_a == 1 && dim_b == 1)
			{
				return TopoOperationPolylinePolylineOrPolygon_(-1);
			}
			if (dim_a == 0 && dim_b == 0)
			{
				return TopoOperationMultiPoint_();
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		public int Intersection(int geometry_a, int geometry_b)
		{
			int gtA = m_topo_graph.GetShape().GetGeometryType(geometry_a);
			int gtB = m_topo_graph.GetShape().GetGeometryType(geometry_b);
			int dim_a = com.epl.geometry.Geometry.GetDimensionFromType(gtA);
			int dim_b = com.epl.geometry.Geometry.GetDimensionFromType(gtB);
			int ID_a = m_topo_graph.GetGeometryID(geometry_a);
			int ID_b = m_topo_graph.GetGeometryID(geometry_b);
			InitMaskLookupArray_(((ID_a | ID_b) + 1));
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_a) | m_topo_graph.GetGeometryID(geometry_b)] = true;
			int geometry_dominant = -1;
			bool b_vertex_dominance = (m_topo_graph.GetShape().GetVertexDescription().GetAttributeCount() > 1);
			if (b_vertex_dominance)
			{
				geometry_dominant = geometry_a;
			}
			if (dim_a == 2 && dim_b == 2)
			{
				// intersect two polygons
				return TopoOperationPolygonPolygon_(geometry_a, geometry_b, geometry_dominant);
			}
			if ((dim_a == 1 && dim_b > 0) || (dim_b == 1 && dim_a > 0))
			{
				// intersect
				// polyline
				// with
				// polyline
				// or
				// polygon
				return TopoOperationPolylinePolylineOrPolygon_(geometry_dominant);
			}
			if (dim_a == 0 || dim_b == 0)
			{
				// intersect a multipoint with something
				// else
				return TopoOperationMultiPoint_();
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal int[] IntersectionEx(int geometry_a, int geometry_b)
		{
			int gtA = m_topo_graph.GetShape().GetGeometryType(geometry_a);
			int gtB = m_topo_graph.GetShape().GetGeometryType(geometry_b);
			int dim_a = com.epl.geometry.Geometry.GetDimensionFromType(gtA);
			int dim_b = com.epl.geometry.Geometry.GetDimensionFromType(gtB);
			int ID_a = m_topo_graph.GetGeometryID(geometry_a);
			int ID_b = m_topo_graph.GetGeometryID(geometry_b);
			InitMaskLookupArray_(((ID_a | ID_b) + 1));
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_a) | m_topo_graph.GetGeometryID(geometry_b)] = true;
			int geometry_dominant = -1;
			bool b_vertex_dominance = (m_topo_graph.GetShape().GetVertexDescription().GetAttributeCount() > 1);
			if (b_vertex_dominance)
			{
				geometry_dominant = geometry_a;
			}
			if (dim_a == 2 && dim_b == 2)
			{
				// intersect two polygons
				return TopoOperationPolygonPolygonEx_(geometry_a, geometry_b, geometry_dominant);
			}
			if ((dim_a == 1 && dim_b > 0) || (dim_b == 1 && dim_a > 0))
			{
				// intersect
				// polyline
				// with
				// polyline
				// or
				// polygon
				return TopoOperationPolylinePolylineOrPolygonEx_(geometry_dominant);
			}
			if (dim_a == 0 || dim_b == 0)
			{
				// intersect a multipoint with something
				// else
				int[] res = new int[1];
				res[0] = TopoOperationMultiPoint_();
				return res;
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		public int SymmetricDifference(int geometry_a, int geometry_b)
		{
			int gtA = m_topo_graph.GetShape().GetGeometryType(geometry_a);
			int gtB = m_topo_graph.GetShape().GetGeometryType(geometry_b);
			int dim_a = com.epl.geometry.Geometry.GetDimensionFromType(gtA);
			int dim_b = com.epl.geometry.Geometry.GetDimensionFromType(gtB);
			int ID_a = m_topo_graph.GetGeometryID(geometry_a);
			int ID_b = m_topo_graph.GetGeometryID(geometry_b);
			InitMaskLookupArray_((ID_a | ID_b) + 1);
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_a)] = true;
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_b)] = true;
			if (dim_a == 2 && dim_b == 2)
			{
				return TopoOperationPolygonPolygon_(geometry_a, geometry_b, -1);
			}
			if (dim_a == 1 && dim_b == 1)
			{
				return TopoOperationPolylinePolylineOrPolygon_(-1);
			}
			if (dim_a == 0 && dim_b == 0)
			{
				return TopoOperationMultiPoint_();
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal int ExtractShape(int geometry_in)
		{
			int gtA = m_topo_graph.GetShape().GetGeometryType(geometry_in);
			int dim_a = com.epl.geometry.Geometry.GetDimensionFromType(gtA);
			int ID_a = m_topo_graph.GetGeometryID(geometry_in);
			InitMaskLookupArray_((ID_a) + 1);
			m_mask_lookup[m_topo_graph.GetGeometryID(geometry_in)] = true;
			// Works
			// only
			// when
			// there
			// is a
			// single
			// geometry
			// in
			// the
			// edit
			// shape.
			// To make it work when many geometries are present, this need to be
			// modified.
			if (dim_a == 2)
			{
				return TopoOperationPolygonPolygon_(geometry_in, -1, -1);
			}
			if (dim_a == 1)
			{
				return TopoOperationPolylinePolylineOrPolygon_(-1);
			}
			if (dim_a == 0)
			{
				return TopoOperationMultiPoint_();
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal static com.epl.geometry.Geometry NormalizeInputGeometry_(com.epl.geometry.Geometry geom)
		{
			com.epl.geometry.Geometry.Type gt = geom.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon(geom.GetDescription());
				if (!geom.IsEmpty())
				{
					poly.AddEnvelope((com.epl.geometry.Envelope)geom, false);
				}
				return poly;
			}
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				com.epl.geometry.MultiPoint poly = new com.epl.geometry.MultiPoint(geom.GetDescription());
				if (!geom.IsEmpty())
				{
					poly.Add((com.epl.geometry.Point)geom);
				}
				return poly;
			}
			if (gt == com.epl.geometry.Geometry.Type.Line)
			{
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline(geom.GetDescription());
				if (!geom.IsEmpty())
				{
					poly.AddSegment((com.epl.geometry.Segment)geom, true);
				}
				return poly;
			}
			return geom;
		}

		internal static com.epl.geometry.Geometry NormalizeResult_(com.epl.geometry.Geometry geomRes, com.epl.geometry.Geometry geom_a, com.epl.geometry.Geometry dummy, char op)
		{
			// assert(strchr("-&^|",op) != NULL);
			com.epl.geometry.Geometry.Type gtRes = geomRes.GetType();
			if (gtRes == com.epl.geometry.Geometry.Type.Envelope)
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon(geomRes.GetDescription());
				if (!geomRes.IsEmpty())
				{
					poly.AddEnvelope((com.epl.geometry.Envelope)geomRes, false);
				}
				return poly;
			}
			if (gtRes == com.epl.geometry.Geometry.Type.Point && (op == '|' || op == '^'))
			{
				com.epl.geometry.MultiPoint poly = new com.epl.geometry.MultiPoint(geomRes.GetDescription());
				if (!geomRes.IsEmpty())
				{
					poly.Add((com.epl.geometry.Point)geomRes);
				}
				return poly;
			}
			if (gtRes == com.epl.geometry.Geometry.Type.Line)
			{
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline(geomRes.GetDescription());
				if (!geomRes.IsEmpty())
				{
					poly.AddSegment((com.epl.geometry.Segment)geomRes, true);
				}
				return poly;
			}
			if (gtRes == com.epl.geometry.Geometry.Type.Point && op == '-')
			{
				if (geom_a.GetType() == com.epl.geometry.Geometry.Type.Point)
				{
					com.epl.geometry.Point pt = new com.epl.geometry.Point(geomRes.GetDescription());
					if (!geomRes.IsEmpty())
					{
						System.Diagnostics.Debug.Assert((((com.epl.geometry.MultiPoint)geomRes).GetPointCount() == 1));
						((com.epl.geometry.MultiPoint)geomRes).GetPointByVal(0, pt);
					}
					return pt;
				}
			}
			if (gtRes == com.epl.geometry.Geometry.Type.MultiPoint && op == '&')
			{
				if (geom_a.GetType() == com.epl.geometry.Geometry.Type.Point)
				{
					com.epl.geometry.Point pt = new com.epl.geometry.Point(geomRes.GetDescription());
					if (!geomRes.IsEmpty())
					{
						System.Diagnostics.Debug.Assert((((com.epl.geometry.MultiPoint)geomRes).GetPointCount() == 1));
						((com.epl.geometry.MultiPoint)geomRes).GetPointByVal(0, pt);
					}
					return pt;
				}
			}
			return geomRes;
		}

		// static
		public static com.epl.geometry.Geometry Difference(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geometry_a.IsEmpty() || geometry_b.IsEmpty() || geometry_a.GetDimension() > geometry_b.GetDimension())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_a), geometry_a, geometry_b, '-');
			}
			com.epl.geometry.Envelope2D env2D_1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env2D_1);
			com.epl.geometry.Envelope2D env2D_2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2D_2);
			if (!env2D_1.IsIntersecting(env2D_2))
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_a), geometry_a, geometry_b, '-');
			}
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env2D_1);
			envMerged.Merge(env2D_2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, true);
			// conservative to have same effect as simplify
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_a));
			int geom_b = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_b));
			topoOps.SetEditShapeCrackAndCluster(edit_shape, tolerance, progress_tracker);
			int result = topoOps.Difference(geom_a, geom_b);
			com.epl.geometry.Geometry resGeom = edit_shape.GetGeometry(result);
			com.epl.geometry.Geometry res_geom = NormalizeResult_(resGeom, geometry_a, geometry_b, '-');
			if (com.epl.geometry.Geometry.IsMultiPath(res_geom.GetType().Value()))
			{
				((com.epl.geometry.MultiVertexGeometryImpl)res_geom._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
				if (res_geom.GetType() == com.epl.geometry.Geometry.Type.Polygon)
				{
					((com.epl.geometry.MultiPathImpl)res_geom._getImpl())._updateOGCFlags();
				}
			}
			return res_geom;
		}

		public static com.epl.geometry.Geometry Dissolve(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geometry_a.GetDimension() > geometry_b.GetDimension())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_a), geometry_a, geometry_b, '|');
			}
			if (geometry_a.GetDimension() < geometry_b.GetDimension())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_b), geometry_a, geometry_b, '|');
			}
			if (geometry_a.IsEmpty())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_b), geometry_a, geometry_b, '|');
			}
			if (geometry_b.IsEmpty())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_a), geometry_a, geometry_b, '|');
			}
			com.epl.geometry.Envelope2D env2D_1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env2D_1);
			com.epl.geometry.Envelope2D env2D_2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2D_2);
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env2D_1);
			envMerged.Merge(env2D_2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, true);
			// conservative to have same effect as simplify
			if (!env2D_1.IsIntersecting(env2D_2.GetInflated(tolerance, tolerance)))
			{
				// TODO: add optimization here to merge two geometries if the
				// envelopes do not overlap.
				com.epl.geometry.Geometry geom1 = NormalizeInputGeometry_(geometry_a);
				System.Diagnostics.Debug.Assert((com.epl.geometry.Geometry.IsMultiVertex(geom1.GetType().Value())));
				com.epl.geometry.Geometry geom2 = NormalizeInputGeometry_(geometry_b);
				System.Diagnostics.Debug.Assert((com.epl.geometry.Geometry.IsMultiVertex(geom2.GetType().Value())));
				System.Diagnostics.Debug.Assert((geom1.GetType() == geom2.GetType()));
				switch (geom1.GetType().Value())
				{
					case com.epl.geometry.Geometry.GeometryType.MultiPoint:
					{
						com.epl.geometry.Geometry res = com.epl.geometry.Geometry._clone(geom1);
						((com.epl.geometry.MultiPoint)res).Add((com.epl.geometry.MultiPoint)geom2, 0, -1);
						return res;
					}

					case com.epl.geometry.Geometry.GeometryType.Polyline:
					{
						// break;
						com.epl.geometry.Geometry res = com.epl.geometry.Geometry._clone(geom1);
						((com.epl.geometry.Polyline)res).Add((com.epl.geometry.MultiPath)geom2, false);
						return res;
					}

					case com.epl.geometry.Geometry.GeometryType.Polygon:
					{
						// break;
						com.epl.geometry.Geometry res = com.epl.geometry.Geometry._clone(geom1);
						((com.epl.geometry.Polygon)res).Add((com.epl.geometry.MultiPath)geom2, false);
						return res;
					}

					default:
					{
						// break;
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
				}
			}
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_a));
			int geom_b = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_b));
			topoOps.SetEditShapeCrackAndCluster(edit_shape, tolerance, progress_tracker);
			int result = topoOps.Dissolve(geom_a, geom_b);
			com.epl.geometry.Geometry res_geom = NormalizeResult_(edit_shape.GetGeometry(result), geometry_a, geometry_b, '|');
			if (com.epl.geometry.Geometry.IsMultiPath(res_geom.GetType().Value()))
			{
				((com.epl.geometry.MultiVertexGeometryImpl)res_geom._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
				if (res_geom.GetType() == com.epl.geometry.Geometry.Type.Polygon)
				{
					((com.epl.geometry.MultiPathImpl)res_geom._getImpl())._updateOGCFlags();
				}
			}
			return res_geom;
		}

		internal static com.epl.geometry.Geometry DissolveDirty(System.Collections.Generic.List<com.epl.geometry.Geometry> geometries, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geometries.Count < 2)
			{
				throw new System.ArgumentException("not enough geometries to dissolve");
			}
			int dim = 0;
			for (int i = 0, n = geometries.Count; i < n; i++)
			{
				dim = System.Math.Max(geometries[i].GetDimension(), dim);
			}
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetEmpty();
			com.epl.geometry.EditShape shape = new com.epl.geometry.EditShape();
			int geom = -1;
			int count = 0;
			int any_index = -1;
			for (int i_1 = 0, n = geometries.Count; i_1 < n; i_1++)
			{
				if (geometries[i_1].GetDimension() == dim)
				{
					if (!geometries[i_1].IsEmpty())
					{
						any_index = i_1;
						if (geom == -1)
						{
							geom = shape.AddGeometry(NormalizeInputGeometry_(geometries[i_1]));
						}
						else
						{
							shape.AppendGeometry(geom, NormalizeInputGeometry_(geometries[i_1]));
						}
						com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
						geometries[i_1].QueryLooseEnvelope2D(env);
						envMerged.Merge(env);
						count++;
					}
					else
					{
						if (any_index == -1)
						{
							any_index = i_1;
						}
					}
				}
			}
			if (count < 2)
			{
				return NormalizeInputGeometry_(geometries[any_index]);
			}
			bool winding = dim == 2;
			com.epl.geometry.SpatialReference psr = dim == 0 ? sr : null;
			// if points, then use
			// correct tolerance.
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(psr, envMerged, true);
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			return topoOps.PlanarSimplify(shape, geom, tolerance, winding, true, progress_tracker);
		}

		// static
		public static com.epl.geometry.Geometry Intersection(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env2D_1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env2D_1);
			com.epl.geometry.Envelope2D env2D_2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2D_2);
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env2D_1);
			envMerged.Merge(env2D_2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, true);
			// conservative to have same effect as simplify
			com.epl.geometry.Envelope2D e = new com.epl.geometry.Envelope2D();
			e.SetCoords(env2D_2);
			double tol_cluster = com.epl.geometry.InternalUtils.Adjust_tolerance_for_TE_clustering(tolerance);
			e.Inflate(tol_cluster, tol_cluster);
			if (!env2D_1.IsIntersecting(e))
			{
				// also includes the empty geometry
				// cases
				if (geometry_a.GetDimension() <= geometry_b.GetDimension())
				{
					return NormalizeResult_(NormalizeInputGeometry_(geometry_a.CreateInstance()), geometry_a, geometry_b, '&');
				}
				if (geometry_a.GetDimension() > geometry_b.GetDimension())
				{
					return NormalizeResult_(NormalizeInputGeometry_(geometry_b.CreateInstance()), geometry_a, geometry_b, '&');
				}
			}
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_a));
			int geom_b = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_b));
			topoOps.SetEditShapeCrackAndCluster(edit_shape, tolerance, progress_tracker);
			int result = topoOps.Intersection(geom_a, geom_b);
			com.epl.geometry.Geometry res_geom = NormalizeResult_(edit_shape.GetGeometry(result), geometry_a, geometry_b, '&');
			if (com.epl.geometry.Geometry.IsMultiPath(res_geom.GetType().Value()))
			{
				((com.epl.geometry.MultiVertexGeometryImpl)res_geom._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
				if (res_geom.GetType() == com.epl.geometry.Geometry.Type.Polygon)
				{
					((com.epl.geometry.MultiPathImpl)res_geom._getImpl())._updateOGCFlags();
				}
			}
			return res_geom;
		}

		internal static com.epl.geometry.Geometry[] IntersectionEx(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Geometry[] res_vec = new com.epl.geometry.Geometry[3];
			com.epl.geometry.Envelope2D env2D_1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env2D_1);
			com.epl.geometry.Envelope2D env2D_2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2D_2);
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env2D_1);
			envMerged.Merge(env2D_2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, true);
			// conservative to have same effect as simplify
			com.epl.geometry.Envelope2D e = new com.epl.geometry.Envelope2D();
			e.SetCoords(env2D_2);
			double tol_cluster = com.epl.geometry.InternalUtils.Adjust_tolerance_for_TE_clustering(tolerance);
			e.Inflate(tol_cluster, tol_cluster);
			if (!env2D_1.IsIntersecting(e))
			{
				// also includes the empty geometry
				// cases
				if (geometry_a.GetDimension() <= geometry_b.GetDimension())
				{
					com.epl.geometry.Geometry geom = NormalizeResult_(NormalizeInputGeometry_(geometry_a.CreateInstance()), geometry_a, geometry_b, '&');
					res_vec[geom.GetDimension()] = geom;
					return res_vec;
				}
				if (geometry_a.GetDimension() > geometry_b.GetDimension())
				{
					com.epl.geometry.Geometry geom = NormalizeResult_(NormalizeInputGeometry_(geometry_b.CreateInstance()), geometry_a, geometry_b, '&');
					res_vec[geom.GetDimension()] = geom;
					return res_vec;
				}
			}
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_a));
			int geom_b = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_b));
			topoOps.SetEditShapeCrackAndCluster(edit_shape, tolerance, progress_tracker);
			int[] result_geom_handles = topoOps.IntersectionEx(geom_a, geom_b);
			for (int i = 0; i < result_geom_handles.Length; i++)
			{
				com.epl.geometry.Geometry res_geom = NormalizeResult_(edit_shape.GetGeometry(result_geom_handles[i]), geometry_a, geometry_b, '&');
				if (com.epl.geometry.Geometry.IsMultiPath(res_geom.GetType().Value()))
				{
					((com.epl.geometry.MultiVertexGeometryImpl)res_geom._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
					if (res_geom.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon)
					{
						((com.epl.geometry.MultiPathImpl)res_geom._getImpl())._updateOGCFlags();
					}
				}
				res_vec[res_geom.GetDimension()] = res_geom;
			}
			return res_vec;
		}

		// static
		public static com.epl.geometry.Geometry SymmetricDifference(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geometry_a.GetDimension() > geometry_b.GetDimension())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_a), geometry_a, geometry_b, '^');
			}
			if (geometry_a.GetDimension() < geometry_b.GetDimension())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_b), geometry_a, geometry_b, '^');
			}
			if (geometry_a.IsEmpty())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_b), geometry_a, geometry_b, '^');
			}
			if (geometry_b.IsEmpty())
			{
				return NormalizeResult_(NormalizeInputGeometry_(geometry_a), geometry_a, geometry_b, '^');
			}
			com.epl.geometry.Envelope2D env2D_1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env2D_1);
			com.epl.geometry.Envelope2D env2D_2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2D_2);
			// TODO: add optimization here to merge two geometries if the envelopes
			// do not overlap.
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env2D_1);
			envMerged.Merge(env2D_2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, true);
			// conservative to have same effect as simplify
			com.epl.geometry.TopologicalOperations topoOps = new com.epl.geometry.TopologicalOperations();
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_a));
			int geom_b = edit_shape.AddGeometry(NormalizeInputGeometry_(geometry_b));
			topoOps.SetEditShapeCrackAndCluster(edit_shape, tolerance, progress_tracker);
			int result = topoOps.SymmetricDifference(geom_a, geom_b);
			com.epl.geometry.Geometry res_geom = NormalizeResult_(edit_shape.GetGeometry(result), geometry_a, geometry_b, '^');
			if (com.epl.geometry.Geometry.IsMultiPath(res_geom.GetType().Value()))
			{
				((com.epl.geometry.MultiVertexGeometryImpl)res_geom._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, tolerance, false);
				if (res_geom.GetType() == com.epl.geometry.Geometry.Type.Polygon)
				{
					((com.epl.geometry.MultiPathImpl)res_geom._getImpl())._updateOGCFlags();
				}
			}
			return res_geom;
		}

		internal static com.epl.geometry.Geometry _denormalizeGeometry(com.epl.geometry.Geometry geom, com.epl.geometry.Geometry geomA, com.epl.geometry.Geometry geomB)
		{
			com.epl.geometry.Geometry.Type gtA = geomA.GetType();
			com.epl.geometry.Geometry.Type gtB = geomB.GetType();
			com.epl.geometry.Geometry.Type gt = geom.GetType();
			if (gt == com.epl.geometry.Geometry.Type.MultiPoint)
			{
				if (gtA == com.epl.geometry.Geometry.Type.Point || gtB == com.epl.geometry.Geometry.Type.Point)
				{
					com.epl.geometry.MultiPoint mp = (com.epl.geometry.MultiPoint)geom;
					if (mp.GetPointCount() <= 1)
					{
						com.epl.geometry.Point pt = new com.epl.geometry.Point(geom.GetDescription());
						if (!mp.IsEmpty())
						{
							mp.GetPointByVal(0, pt);
						}
						return (com.epl.geometry.Geometry)pt;
					}
				}
			}
			return geom;
		}

		private void FlushVertices_(int geometry, com.epl.geometry.AttributeStreamOfInt32 vertices)
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int path = shape.InsertPath(geometry, -1);
			int size = vertices.Size();
			// _ASSERT(size != 0);
			for (int i = 0; i < size; i++)
			{
				int vertex = vertices.Get(i);
				shape.AddVertex(path, vertex);
			}
			shape.SetClosedPath(path, true);
		}

		// need to close polygon rings
		private void SetHalfEdgeOrientations_(int orientationIndex, int cutter)
		{
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			for (int igeometry = shape.GetFirstGeometry(); igeometry != -1; igeometry = shape.GetNextGeometry(igeometry))
			{
				if (igeometry != cutter)
				{
					continue;
				}
				for (int ipath = shape.GetFirstPath(igeometry); ipath != -1; ipath = shape.GetNextPath(ipath))
				{
					int ivertex = shape.GetFirstVertex(ipath);
					if (ivertex == -1)
					{
						continue;
					}
					int ivertexNext = shape.GetNextVertex(ivertex);
					System.Diagnostics.Debug.Assert((ivertexNext != -1));
					while (ivertexNext != -1)
					{
						int clusterFrom = m_topo_graph.GetClusterFromVertex(ivertex);
						int clusterTo = m_topo_graph.GetClusterFromVertex(ivertexNext);
						int half_edge = m_topo_graph.GetHalfEdgeConnector(clusterFrom, clusterTo);
						if (half_edge != -1)
						{
							int halfEdgeTwin = m_topo_graph.GetHalfEdgeTwin(half_edge);
							m_topo_graph.SetHalfEdgeUserIndex(half_edge, orientationIndex, 1);
							m_topo_graph.SetHalfEdgeUserIndex(halfEdgeTwin, orientationIndex, 2);
						}
						ivertex = ivertexNext;
						ivertexNext = shape.GetNextVertex(ivertex);
					}
				}
			}
		}

		private void ProcessPolygonCuts_(int orientationIndex, int sideIndex, int cuttee, int cutter)
		{
			int idCuttee = m_topo_graph.GetGeometryID(cuttee);
			int idCutter = m_topo_graph.GetGeometryID(cutter);
			com.epl.geometry.AttributeStreamOfInt32 vertices = new com.epl.geometry.AttributeStreamOfInt32(0);
			vertices.Reserve(256);
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int visitedIndex = m_topo_graph.CreateUserIndexForHalfEdges();
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int firstHalfEdge = m_topo_graph.GetClusterHalfEdge(cluster);
				if (firstHalfEdge == -1)
				{
					continue;
				}
				int half_edge = firstHalfEdge;
				do
				{
					int visited = m_topo_graph.GetHalfEdgeUserIndex(half_edge, visitedIndex);
					if (visited != 1)
					{
						int faceHalfEdge = half_edge;
						int toHalfEdge = half_edge;
						bool bFoundCutter = false;
						int side = 0;
						do
						{
							m_topo_graph.SetHalfEdgeUserIndex(faceHalfEdge, visitedIndex, 1);
							if (!bFoundCutter)
							{
								int edgeParentage = m_topo_graph.GetHalfEdgeParentage(faceHalfEdge);
								if ((edgeParentage & idCutter) != 0)
								{
									int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(faceHalfEdge);
									if ((faceParentage & idCuttee) != 0)
									{
										toHalfEdge = faceHalfEdge;
										// reset the loop
										bFoundCutter = true;
									}
								}
							}
							if (bFoundCutter)
							{
								int clusterOrigin = m_topo_graph.GetHalfEdgeOrigin(faceHalfEdge);
								int iterator = m_topo_graph.GetClusterVertexIterator(clusterOrigin);
								System.Diagnostics.Debug.Assert((iterator != -1));
								int vertex = m_topo_graph.GetVertexFromVertexIterator(iterator);
								vertices.Add(vertex);
								// get side
								if (orientationIndex != -1)
								{
									int edgeParentage = m_topo_graph.GetHalfEdgeParentage(faceHalfEdge);
									if ((edgeParentage & idCutter) != 0)
									{
										int orientation = m_topo_graph.GetHalfEdgeUserIndex(faceHalfEdge, orientationIndex);
										System.Diagnostics.Debug.Assert((orientation == 1 || orientation == 2));
										side |= orientation;
									}
								}
							}
							int next = m_topo_graph.GetHalfEdgeNext(faceHalfEdge);
							faceHalfEdge = next;
						}
						while (faceHalfEdge != toHalfEdge);
						if (bFoundCutter && m_topo_graph.GetChainArea(m_topo_graph.GetHalfEdgeChain(toHalfEdge)) > 0.0)
						{
							// if
							// we
							// found
							// a
							// cutter
							// face
							// and
							// its
							// area
							// is
							// positive,
							// then
							// add
							// the
							// cutter
							// face
							// as
							// new
							// polygon.
							int geometry = shape.CreateGeometry(com.epl.geometry.Geometry.Type.Polygon);
							FlushVertices_(geometry, vertices);
							// adds the cutter
							// face vertices to
							// the new polygon
							if (sideIndex != -1)
							{
								shape.SetGeometryUserIndex(geometry, sideIndex, side);
							}
						}
						// what is that?
						vertices.Clear(false);
					}
					half_edge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(half_edge));
				}
				while (half_edge != firstHalfEdge);
			}
			m_topo_graph.DeleteUserIndexForHalfEdges(visitedIndex);
		}

		private void CutPolygonPolyline_(int sideIndex, int cuttee, int cutter, com.epl.geometry.AttributeStreamOfInt32 cutHandles)
		{
			m_topo_graph.RemoveSpikes_();
			int orientationIndex = -1;
			if (sideIndex != -1)
			{
				orientationIndex = m_topo_graph.CreateUserIndexForHalfEdges();
				SetHalfEdgeOrientations_(orientationIndex, cutter);
			}
			ProcessPolygonCuts_(orientationIndex, sideIndex, cuttee, cutter);
			com.epl.geometry.EditShape shape = m_topo_graph.GetShape();
			int cutCount = 0;
			for (int geometry_handle = shape.GetFirstGeometry(); geometry_handle != -1; geometry_handle = shape.GetNextGeometry(geometry_handle))
			{
				if (geometry_handle != cuttee && geometry_handle != cutter)
				{
					cutHandles.Add(geometry_handle);
					cutCount++;
				}
			}
			// sort
			com.epl.geometry.TopologicalOperations.CompareCuts compareCuts = new com.epl.geometry.TopologicalOperations.CompareCuts(shape);
			cutHandles.Sort(0, cutCount, compareCuts);
		}

		//call this if EditShape instance has to survive the TopologicalOperations life.
		internal void RemoveShape()
		{
			if (m_topo_graph != null)
			{
				m_topo_graph.RemoveShape();
				m_topo_graph = null;
			}
		}
	}
}
