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
	internal class Simplificator
	{
		private com.epl.geometry.EditShape m_shape;

		private int m_geometry;

		private com.epl.geometry.IndexMultiDCList m_sortedVertices;

		private com.epl.geometry.AttributeStreamOfInt32 m_bunchEdgeEndPoints;

		private com.epl.geometry.AttributeStreamOfInt32 m_bunchEdgeCenterPoints;

		private com.epl.geometry.AttributeStreamOfInt32 m_bunchEdgeIndices;

		private int m_dbgCounter;

		private int m_sortedVerticesListIndex;

		private int m_userIndexSortedIndexToVertex;

		private int m_userIndexSortedAngleIndexToVertex;

		private int m_nextVertexToProcess;

		private int m_firstCoincidentVertex;

		private int m_knownSimpleResult;

		private bool m_bWinding;

		private bool m_fixSelfTangency;

		private com.epl.geometry.ProgressTracker m_progressTracker;

		// private AttributeStreamOfInt32 m_orphanVertices;
		private void _beforeRemoveVertex(int vertex, bool bChangePathFirst)
		{
			int vertexlistIndex = m_shape.GetUserIndex(vertex, m_userIndexSortedIndexToVertex);
			if (m_nextVertexToProcess == vertexlistIndex)
			{
				m_nextVertexToProcess = m_sortedVertices.GetNext(m_nextVertexToProcess);
			}
			if (m_firstCoincidentVertex == vertexlistIndex)
			{
				m_firstCoincidentVertex = m_sortedVertices.GetNext(m_firstCoincidentVertex);
			}
			m_sortedVertices.DeleteElement(m_sortedVerticesListIndex, vertexlistIndex);
			_removeAngleSortInfo(vertex);
			if (bChangePathFirst)
			{
				int path = m_shape.GetPathFromVertex(vertex);
				if (path != -1)
				{
					int first = m_shape.GetFirstVertex(path);
					if (first == vertex)
					{
						int next = m_shape.GetNextVertex(vertex);
						if (next != vertex)
						{
							int p = m_shape.GetPathFromVertex(next);
							if (p == path)
							{
								m_shape.SetFirstVertex_(path, next);
								return;
							}
							else
							{
								int prev = m_shape.GetPrevVertex(vertex);
								if (prev != vertex)
								{
									p = m_shape.GetPathFromVertex(prev);
									if (p == path)
									{
										m_shape.SetFirstVertex_(path, prev);
										return;
									}
								}
							}
						}
						m_shape.SetFirstVertex_(path, -1);
						m_shape.SetLastVertex_(path, -1);
					}
				}
			}
		}

		internal class SimplificatorAngleComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.Simplificator m_parent;

			public SimplificatorAngleComparer(com.epl.geometry.Simplificator parent)
			{
				m_parent = parent;
			}

			public override int Compare(int v1, int v2)
			{
				return m_parent._compareAngles(v1, v2);
			}
		}

		private bool _processBunch()
		{
			bool bModified = false;
			int iter = 0;
			com.epl.geometry.Point2D ptCenter = new com.epl.geometry.Point2D();
			while (true)
			{
				m_dbgCounter++;
				// only for debugging
				iter++;
				// _ASSERT(iter < 10);
				if (m_bunchEdgeEndPoints == null)
				{
					m_bunchEdgeEndPoints = new com.epl.geometry.AttributeStreamOfInt32(0);
					m_bunchEdgeCenterPoints = new com.epl.geometry.AttributeStreamOfInt32(0);
					m_bunchEdgeIndices = new com.epl.geometry.AttributeStreamOfInt32(0);
				}
				else
				{
					m_bunchEdgeEndPoints.Clear(false);
					m_bunchEdgeCenterPoints.Clear(false);
					m_bunchEdgeIndices.Clear(false);
				}
				int currentVertex = m_firstCoincidentVertex;
				int index = 0;
				bool bFirst = true;
				while (currentVertex != m_nextVertexToProcess)
				{
					int v = m_sortedVertices.GetData(currentVertex);
					{
						// debug
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						m_shape.GetXY(v, pt);
						double y = pt.x;
					}
					if (bFirst)
					{
						m_shape.GetXY(v, ptCenter);
						bFirst = false;
					}
					int vertP = m_shape.GetPrevVertex(v);
					int vertN = m_shape.GetNextVertex(v);
					// _ASSERT(vertP != vertN || m_shape.getPrevVertex(vertN) == v
					// && m_shape.getNextVertex(vertP) == v);
					int id = m_shape.GetUserIndex(vertP, m_userIndexSortedAngleIndexToVertex);
					if (id != unchecked((int)(0xdeadbeef)))
					{
						// avoid adding a point twice
						// _ASSERT(id == -1);
						m_bunchEdgeEndPoints.Add(vertP);
						m_shape.SetUserIndex(vertP, m_userIndexSortedAngleIndexToVertex, unchecked((int)(0xdeadbeef)));
						// mark
						// that
						// it
						// has
						// been
						// already
						// added
						m_bunchEdgeCenterPoints.Add(v);
						m_bunchEdgeIndices.Add(index++);
					}
					int id2 = m_shape.GetUserIndex(vertN, m_userIndexSortedAngleIndexToVertex);
					if (id2 != unchecked((int)(0xdeadbeef)))
					{
						// avoid adding a point twice
						// _ASSERT(id2 == -1);
						m_bunchEdgeEndPoints.Add(vertN);
						m_shape.SetUserIndex(vertN, m_userIndexSortedAngleIndexToVertex, unchecked((int)(0xdeadbeef)));
						// mark
						// that
						// it
						// has
						// been
						// already
						// added
						m_bunchEdgeCenterPoints.Add(v);
						m_bunchEdgeIndices.Add(index++);
					}
					currentVertex = m_sortedVertices.GetNext(currentVertex);
				}
				if (m_bunchEdgeEndPoints.Size() < 2)
				{
					break;
				}
				// Sort the bunch edpoints by angle (angle between the axis x and
				// the edge, connecting the endpoint with the bunch center)
				m_bunchEdgeIndices.Sort(0, m_bunchEdgeIndices.Size(), new com.epl.geometry.Simplificator.SimplificatorAngleComparer(this));
				// SORTDYNAMICARRAYEX(m_bunchEdgeIndices, int, 0,
				// m_bunchEdgeIndices.size(), SimplificatorAngleComparer, this);
				for (int i = 0, n = m_bunchEdgeIndices.Size(); i < n; i++)
				{
					int indexL = m_bunchEdgeIndices.Get(i);
					int vertex = m_bunchEdgeEndPoints.Get(indexL);
					m_shape.SetUserIndex(vertex, m_userIndexSortedAngleIndexToVertex, i);
					{
						// rember the
						// sort by angle
						// order
						// debug
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						m_shape.GetXY(vertex, pt);
						double y = pt.x;
					}
				}
				bool bCrossOverResolved = _processCrossOvers(ptCenter);
				// see of
				// there
				// are
				// crossing
				// over
				// edges.
				for (int i_1 = 0, n = m_bunchEdgeIndices.Size(); i_1 < n; i_1++)
				{
					int indexL = m_bunchEdgeIndices.Get(i_1);
					if (indexL == -1)
					{
						continue;
					}
					int vertex = m_bunchEdgeEndPoints.Get(indexL);
					m_shape.SetUserIndex(vertex, m_userIndexSortedAngleIndexToVertex, -1);
				}
				// remove
				// mapping
				if (bCrossOverResolved)
				{
					bModified = true;
					continue;
				}
				break;
			}
			return bModified;
		}

		private bool _processCrossOvers(com.epl.geometry.Point2D ptCenter)
		{
			bool bFound = false;
			// Resolve all overlaps
			bool bContinue = true;
			while (bContinue)
			{
				// The nearest pairts in the middle of the list
				bContinue = false;
				int index1 = 0;
				if (m_bunchEdgeIndices.Get(index1) == -1)
				{
					index1 = _getNextEdgeIndex(index1);
				}
				int index2 = _getNextEdgeIndex(index1);
				for (int i = 0, n = m_bunchEdgeIndices.Size(); i < n && index1 != -1 && index2 != -1 && index1 != index2; i++)
				{
					int edgeindex1 = m_bunchEdgeIndices.Get(index1);
					int edgeindex2 = m_bunchEdgeIndices.Get(index2);
					int vertexB1 = m_bunchEdgeEndPoints.Get(edgeindex1);
					int vertexB2 = m_bunchEdgeEndPoints.Get(edgeindex2);
					// _ASSERT(vertexB2 != vertexB1);
					int vertexA1 = m_shape.GetNextVertex(vertexB1);
					if (!m_shape.IsEqualXY(vertexA1, ptCenter))
					{
						vertexA1 = m_shape.GetPrevVertex(vertexB1);
					}
					int vertexA2 = m_shape.GetNextVertex(vertexB2);
					if (!m_shape.IsEqualXY(vertexA2, ptCenter))
					{
						vertexA2 = m_shape.GetPrevVertex(vertexB2);
					}
					// _ASSERT(m_shape.isEqualXY(vertexA1, vertexA2));
					// _ASSERT(m_shape.isEqualXY(vertexA1, ptCenter));
					bool bDirection1 = _getDirection(vertexA1, vertexB1);
					bool bDirection2 = _getDirection(vertexA2, vertexB2);
					int vertexC1 = bDirection1 ? m_shape.GetPrevVertex(vertexA1) : m_shape.GetNextVertex(vertexA1);
					int vertexC2 = bDirection2 ? m_shape.GetPrevVertex(vertexA2) : m_shape.GetNextVertex(vertexA2);
					bool bOverlap = false;
					if (_removeSpike(vertexA1))
					{
						bOverlap = true;
					}
					else
					{
						if (_removeSpike(vertexA2))
						{
							bOverlap = true;
						}
						else
						{
							if (_removeSpike(vertexB1))
							{
								bOverlap = true;
							}
							else
							{
								if (_removeSpike(vertexB2))
								{
									bOverlap = true;
								}
								else
								{
									if (_removeSpike(vertexC1))
									{
										bOverlap = true;
									}
									else
									{
										if (_removeSpike(vertexC2))
										{
											bOverlap = true;
										}
									}
								}
							}
						}
					}
					if (!bOverlap && m_shape.IsEqualXY(vertexB1, vertexB2))
					{
						bOverlap = true;
						_resolveOverlap(bDirection1, bDirection2, vertexA1, vertexB1, vertexA2, vertexB2);
					}
					if (!bOverlap && m_shape.IsEqualXY(vertexC1, vertexC2))
					{
						bOverlap = true;
						_resolveOverlap(!bDirection1, !bDirection2, vertexA1, vertexC1, vertexA2, vertexC2);
					}
					if (bOverlap)
					{
						bFound = true;
					}
					bContinue |= bOverlap;
					index1 = _getNextEdgeIndex(index1);
					index2 = _getNextEdgeIndex(index1);
				}
			}
			if (!bFound)
			{
				// resolve all cross overs
				int index1 = 0;
				if (m_bunchEdgeIndices.Get(index1) == -1)
				{
					index1 = _getNextEdgeIndex(index1);
				}
				int index2 = _getNextEdgeIndex(index1);
				for (int i = 0, n = m_bunchEdgeIndices.Size(); i < n && index1 != -1 && index2 != -1 && index1 != index2; i++)
				{
					int edgeindex1 = m_bunchEdgeIndices.Get(index1);
					int edgeindex2 = m_bunchEdgeIndices.Get(index2);
					int vertexB1 = m_bunchEdgeEndPoints.Get(edgeindex1);
					int vertexB2 = m_bunchEdgeEndPoints.Get(edgeindex2);
					int vertexA1 = m_shape.GetNextVertex(vertexB1);
					if (!m_shape.IsEqualXY(vertexA1, ptCenter))
					{
						vertexA1 = m_shape.GetPrevVertex(vertexB1);
					}
					int vertexA2 = m_shape.GetNextVertex(vertexB2);
					if (!m_shape.IsEqualXY(vertexA2, ptCenter))
					{
						vertexA2 = m_shape.GetPrevVertex(vertexB2);
					}
					// _ASSERT(m_shape.isEqualXY(vertexA1, vertexA2));
					// _ASSERT(m_shape.isEqualXY(vertexA1, ptCenter));
					bool bDirection1 = _getDirection(vertexA1, vertexB1);
					bool bDirection2 = _getDirection(vertexA2, vertexB2);
					int vertexC1 = bDirection1 ? m_shape.GetPrevVertex(vertexA1) : m_shape.GetNextVertex(vertexA1);
					int vertexC2 = bDirection2 ? m_shape.GetPrevVertex(vertexA2) : m_shape.GetNextVertex(vertexA2);
					if (_detectAndResolveCrossOver(bDirection1, bDirection2, vertexB1, vertexA1, vertexC1, vertexB2, vertexA2, vertexC2))
					{
						bFound = true;
					}
					index1 = _getNextEdgeIndex(index1);
					index2 = _getNextEdgeIndex(index1);
				}
			}
			return bFound;
		}

		internal class SimplificatorVertexComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.Simplificator m_parent;

			internal SimplificatorVertexComparer(com.epl.geometry.Simplificator parent)
			{
				m_parent = parent;
			}

			public override int Compare(int v1, int v2)
			{
				return m_parent._compareVerticesSimple(v1, v2);
			}
		}

		private bool _simplify()
		{
			if (m_shape.GetGeometryType(m_geometry) == com.epl.geometry.Geometry.Type.Polygon.Value() && m_shape.GetFillRule(m_geometry) == com.epl.geometry.Polygon.FillRule.enumFillRuleWinding)
			{
				com.epl.geometry.TopologicalOperations ops = new com.epl.geometry.TopologicalOperations();
				ops.PlanarSimplifyNoCrackingAndCluster(m_fixSelfTangency, m_shape, m_geometry, m_progressTracker);
				System.Diagnostics.Debug.Assert((m_shape.GetFillRule(m_geometry) == com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven));
			}
			bool bChanged = false;
			bool bNeedWindingRepeat = true;
			bool bWinding = false;
			m_userIndexSortedIndexToVertex = -1;
			m_userIndexSortedAngleIndexToVertex = -1;
			int pointCount = m_shape.GetPointCount(m_geometry);
			// Sort vertices lexicographically
			// Firstly copy allvertices to an array.
			com.epl.geometry.AttributeStreamOfInt32 verticesSorter = new com.epl.geometry.AttributeStreamOfInt32(0);
			verticesSorter.Reserve(pointCount);
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; path = m_shape.GetNextPath(path))
			{
				int vertex = m_shape.GetFirstVertex(path);
				for (int index = 0, n = m_shape.GetPathSize(path); index < n; index++)
				{
					verticesSorter.Add(vertex);
					vertex = m_shape.GetNextVertex(vertex);
				}
			}
			// Sort
			verticesSorter.Sort(0, pointCount, new com.epl.geometry.Simplificator.SimplificatorVertexComparer(this));
			// SORTDYNAMICARRAYEX(verticesSorter, int, 0, pointCount,
			// SimplificatorVertexComparer, this);
			// Copy sorted vertices to the m_sortedVertices list. Make a mapping
			// from the edit shape vertices to the sorted vertices.
			m_userIndexSortedIndexToVertex = m_shape.CreateUserIndex();
			// this index
			// is used
			// to map
			// from edit
			// shape
			// vertex to
			// the
			// m_sortedVertices
			// list
			m_sortedVertices = new com.epl.geometry.IndexMultiDCList();
			m_sortedVerticesListIndex = m_sortedVertices.CreateList(0);
			for (int i = 0; i < pointCount; i++)
			{
				int vertex = verticesSorter.Get(i);
				{
					// debug
					com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
					m_shape.GetXY(vertex, pt);
					// for debugging
					double y = pt.x;
				}
				int vertexlistIndex = m_sortedVertices.AddElement(m_sortedVerticesListIndex, vertex);
				m_shape.SetUserIndex(vertex, m_userIndexSortedIndexToVertex, vertexlistIndex);
			}
			// remember the sorted list element on the
			// vertex.
			// When we remove a vertex, we also remove associated sorted list
			// element.
			m_userIndexSortedAngleIndexToVertex = m_shape.CreateUserIndex();
			// create
			// additional
			// list
			// to
			// store
			// angular
			// sort
			// mapping.
			m_nextVertexToProcess = -1;
			if (_cleanupSpikes())
			{
				// cleanup any spikes on the polygon.
				bChanged = true;
			}
			// External iteration loop for the simplificator.
			// ST. I am not sure if it actually needs this loop. TODO: figure this
			// out.
			while (bNeedWindingRepeat)
			{
				bNeedWindingRepeat = false;
				int max_iter = m_shape.GetPointCount(m_geometry) + 10 > 30 ? 1000 : (m_shape.GetPointCount(m_geometry) + 10) * (m_shape.GetPointCount(m_geometry) + 10);
				// Simplify polygon
				int iRepeatNum = 0;
				bool bNeedRepeat = false;
				do
				{
					// Internal iteration loop for the simplificator.
					// ST. I am not sure if it actually needs this loop. TODO: figure
					// this out.
					// while (bNeedRepeat);
					bNeedRepeat = false;
					bool bVertexRecheck = false;
					m_firstCoincidentVertex = -1;
					int coincidentCount = 0;
					com.epl.geometry.Point2D ptFirst = new com.epl.geometry.Point2D();
					com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
					// Main loop of the simplificator. Go through the vertices and
					// for those that have same coordinates,
					for (int vlistindex = m_sortedVertices.GetFirst(m_sortedVerticesListIndex); vlistindex != com.epl.geometry.IndexMultiDCList.NullNode(); )
					{
						int vertex = m_sortedVertices.GetData(vlistindex);
						{
							// debug
							// Point2D pt = new Point2D();
							m_shape.GetXY(vertex, pt);
							double d = pt.x;
						}
						if (m_firstCoincidentVertex != -1)
						{
							// Point2D pt = new Point2D();
							m_shape.GetXY(vertex, pt);
							if (ptFirst.IsEqual(pt))
							{
								coincidentCount++;
							}
							else
							{
								ptFirst.SetCoords(pt);
								m_nextVertexToProcess = vlistindex;
								// we remeber the
								// next index in
								// the member
								// variable to
								// allow it to
								// be updated if
								// a vertex is
								// removed
								// inside of the
								// _ProcessBunch.
								if (coincidentCount > 0)
								{
									bool result = _processBunch();
									// process a
									// bunch of
									// coinciding
									// vertices
									if (result)
									{
										// something has changed.
										// Note that ProcessBunch may
										// change m_nextVertexToProcess
										// and m_firstCoincidentVertex.
										bNeedRepeat = true;
										if (m_nextVertexToProcess != com.epl.geometry.IndexMultiDCList.NullNode())
										{
											int v = m_sortedVertices.GetData(m_nextVertexToProcess);
											m_shape.GetXY(v, ptFirst);
										}
									}
								}
								vlistindex = m_nextVertexToProcess;
								m_firstCoincidentVertex = vlistindex;
								coincidentCount = 0;
							}
						}
						else
						{
							m_firstCoincidentVertex = vlistindex;
							m_shape.GetXY(m_sortedVertices.GetData(vlistindex), ptFirst);
							coincidentCount = 0;
						}
						if (vlistindex != -1)
						{
							//vlistindex can be set to -1 after ProcessBunch call above
							vlistindex = m_sortedVertices.GetNext(vlistindex);
						}
					}
					m_nextVertexToProcess = -1;
					if (coincidentCount > 0)
					{
						bool result = _processBunch();
						if (result)
						{
							bNeedRepeat = true;
						}
					}
					if (iRepeatNum++ > 10)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					if (bNeedRepeat)
					{
						_fixOrphanVertices();
					}
					// fix broken structure of the shape
					if (_cleanupSpikes())
					{
						bNeedRepeat = true;
					}
					bNeedWindingRepeat |= bNeedRepeat && bWinding;
					bChanged |= bNeedRepeat;
				}
				while (bNeedRepeat);
			}
			// while (bNeedWindingRepeat)
			// Now process rings. Fix ring orientation and determine rings that need
			// to be deleted.
			m_shape.RemoveUserIndex(m_userIndexSortedIndexToVertex);
			m_shape.RemoveUserIndex(m_userIndexSortedAngleIndexToVertex);
			bChanged |= com.epl.geometry.RingOrientationFixer.Execute(m_shape, m_geometry, m_sortedVertices, m_fixSelfTangency);
			return bChanged;
		}

		private bool _getDirection(int vert1, int vert2)
		{
			if (m_shape.GetNextVertex(vert2) == vert1)
			{
				// _ASSERT(m_shape.getPrevVertex(vert1) == vert2);
				return false;
			}
			else
			{
				// _ASSERT(m_shape.getPrevVertex(vert2) == vert1);
				// _ASSERT(m_shape.getNextVertex(vert1) == vert2);
				return true;
			}
		}

		private bool _detectAndResolveCrossOver(bool bDirection1, bool bDirection2, int vertexB1, int vertexA1, int vertexC1, int vertexB2, int vertexA2, int vertexC2)
		{
			// _ASSERT(!m_shape.isEqualXY(vertexB1, vertexB2));
			// _ASSERT(!m_shape.isEqualXY(vertexC1, vertexC2));
			if (vertexA1 == vertexA2)
			{
				_removeAngleSortInfo(vertexB1);
				_removeAngleSortInfo(vertexB2);
				return false;
			}
			// _ASSERT(!m_shape.isEqualXY(vertexB1, vertexC2));
			// _ASSERT(!m_shape.isEqualXY(vertexB1, vertexC1));
			// _ASSERT(!m_shape.isEqualXY(vertexB2, vertexC2));
			// _ASSERT(!m_shape.isEqualXY(vertexB2, vertexC1));
			// _ASSERT(!m_shape.isEqualXY(vertexA1, vertexB1));
			// _ASSERT(!m_shape.isEqualXY(vertexA1, vertexC1));
			// _ASSERT(!m_shape.isEqualXY(vertexA2, vertexB2));
			// _ASSERT(!m_shape.isEqualXY(vertexA2, vertexC2));
			// _ASSERT(m_shape.isEqualXY(vertexA1, vertexA2));
			// get indices of the vertices for the angle sort.
			int iB1 = m_shape.GetUserIndex(vertexB1, m_userIndexSortedAngleIndexToVertex);
			int iC1 = m_shape.GetUserIndex(vertexC1, m_userIndexSortedAngleIndexToVertex);
			int iB2 = m_shape.GetUserIndex(vertexB2, m_userIndexSortedAngleIndexToVertex);
			int iC2 = m_shape.GetUserIndex(vertexC2, m_userIndexSortedAngleIndexToVertex);
			// _ASSERT(iB1 >= 0);
			// _ASSERT(iC1 >= 0);
			// _ASSERT(iB2 >= 0);
			// _ASSERT(iC2 >= 0);
			// Sort the indices to restore the angle-sort order
			int[] ar = new int[8];
			int[] br = new int[4];
			ar[0] = 0;
			br[0] = iB1;
			ar[1] = 0;
			br[1] = iC1;
			ar[2] = 1;
			br[2] = iB2;
			ar[3] = 1;
			br[3] = iC2;
			for (int j = 1; j < 4; j++)
			{
				// insertion sort
				int key = br[j];
				int data = ar[j];
				int i = j - 1;
				while (i >= 0 && br[i] > key)
				{
					br[i + 1] = br[i];
					ar[i + 1] = ar[i];
					i--;
				}
				br[i + 1] = key;
				ar[i + 1] = data;
			}
			int detector = 0;
			if (ar[0] != 0)
			{
				detector |= 1;
			}
			if (ar[1] != 0)
			{
				detector |= 2;
			}
			if (ar[2] != 0)
			{
				detector |= 4;
			}
			if (ar[3] != 0)
			{
				detector |= 8;
			}
			if (detector != 5 && detector != 10)
			{
				// not an overlap
				return false;
			}
			if (bDirection1 == bDirection2)
			{
				if (bDirection1)
				{
					m_shape.SetNextVertex_(vertexC2, vertexA1);
					// B1< >B2
					m_shape.SetPrevVertex_(vertexA1, vertexC2);
					// \ /
					m_shape.SetNextVertex_(vertexC1, vertexA2);
					// A1A2
					m_shape.SetPrevVertex_(vertexA2, vertexC1);
				}
				else
				{
					// / \ //
					// C2> <C1
					m_shape.SetPrevVertex_(vertexC2, vertexA1);
					// B1> <B2
					m_shape.SetNextVertex_(vertexA1, vertexC2);
					// \ /
					m_shape.SetPrevVertex_(vertexC1, vertexA2);
					// A1A2
					m_shape.SetNextVertex_(vertexA2, vertexC1);
				}
			}
			else
			{
				// / \ //
				// C2< >C1
				if (bDirection1)
				{
					m_shape.SetPrevVertex_(vertexA1, vertexB2);
					// B1< <B2
					m_shape.SetNextVertex_(vertexB2, vertexA1);
					// \ /
					m_shape.SetPrevVertex_(vertexA2, vertexC1);
					// A1A2
					m_shape.SetNextVertex_(vertexC1, vertexA2);
				}
				else
				{
					// / \ //
					// C2< <C1
					m_shape.SetNextVertex_(vertexA1, vertexB2);
					// B1> >B2
					m_shape.SetPrevVertex_(vertexB2, vertexA1);
					// \ /
					m_shape.SetNextVertex_(vertexA2, vertexC1);
					// A1A2
					m_shape.SetPrevVertex_(vertexC1, vertexA2);
				}
			}
			// / \ //
			// C2> >C1
			return true;
		}

		private void _resolveOverlap(bool bDirection1, bool bDirection2, int vertexA1, int vertexB1, int vertexA2, int vertexB2)
		{
			if (m_bWinding)
			{
				_resolveOverlapWinding(bDirection1, bDirection2, vertexA1, vertexB1, vertexA2, vertexB2);
			}
			else
			{
				_resolveOverlapOddEven(bDirection1, bDirection2, vertexA1, vertexB1, vertexA2, vertexB2);
			}
		}

		private void _resolveOverlapWinding(bool bDirection1, bool bDirection2, int vertexA1, int vertexB1, int vertexA2, int vertexB2)
		{
			throw new com.epl.geometry.GeometryException("not implemented.");
		}

		private void _resolveOverlapOddEven(bool bDirection1, bool bDirection2, int vertexA1, int vertexB1, int vertexA2, int vertexB2)
		{
			if (bDirection1 != bDirection2)
			{
				if (bDirection1)
				{
					// _ASSERT(m_shape.getNextVertex(vertexA1) == vertexB1);
					// _ASSERT(m_shape.getNextVertex(vertexB2) == vertexA2);
					m_shape.SetNextVertex_(vertexA1, vertexA2);
					// B1< B2
					m_shape.SetPrevVertex_(vertexA2, vertexA1);
					// | |
					m_shape.SetNextVertex_(vertexB2, vertexB1);
					// | |
					m_shape.SetPrevVertex_(vertexB1, vertexB2);
					// A1 >A2
					_transferVertexData(vertexA2, vertexA1);
					_beforeRemoveVertex(vertexA2, true);
					m_shape.RemoveVertexInternal_(vertexA2, true);
					_removeAngleSortInfo(vertexA1);
					_transferVertexData(vertexB2, vertexB1);
					_beforeRemoveVertex(vertexB2, true);
					m_shape.RemoveVertexInternal_(vertexB2, false);
					_removeAngleSortInfo(vertexB1);
				}
				else
				{
					m_shape.SetNextVertex_(vertexA2, vertexA1);
					// B1 B2<
					m_shape.SetPrevVertex_(vertexA1, vertexA2);
					// | |
					m_shape.SetNextVertex_(vertexB1, vertexB2);
					// | |
					m_shape.SetPrevVertex_(vertexB2, vertexB1);
					// A1< A2
					_transferVertexData(vertexA2, vertexA1);
					_beforeRemoveVertex(vertexA2, true);
					m_shape.RemoveVertexInternal_(vertexA2, false);
					_removeAngleSortInfo(vertexA1);
					_transferVertexData(vertexB2, vertexB1);
					_beforeRemoveVertex(vertexB2, true);
					m_shape.RemoveVertexInternal_(vertexB2, true);
					_removeAngleSortInfo(vertexB1);
				}
			}
			else
			{
				// bDirection1 == bDirection2
				if (!bDirection1)
				{
				}
				{
					// _ASSERT(m_shape.getNextVertex(vertexB1) == vertexA1);
					// _ASSERT(m_shape.getNextVertex(vertexB2) == vertexA2);
					// _ASSERT(m_shape.getNextVertex(vertexA1) == vertexB1);
					// _ASSERT(m_shape.getNextVertex(vertexA2) == vertexB2);
					// if (m_shape._RingParentageCheckInternal(vertexA1, vertexA2))
					int a1 = bDirection1 ? vertexA1 : vertexB1;
					int a2 = bDirection2 ? vertexA2 : vertexB2;
					int b1 = bDirection1 ? vertexB1 : vertexA1;
					int b2 = bDirection2 ? vertexB2 : vertexA2;
					// m_shape.dbgVerifyIntegrity(a1);//debug
					// m_shape.dbgVerifyIntegrity(a2);//debug
					bool bVisitedA1 = false;
					m_shape.SetNextVertex_(a1, a2);
					m_shape.SetNextVertex_(a2, a1);
					m_shape.SetPrevVertex_(b1, b2);
					m_shape.SetPrevVertex_(b2, b1);
					int v = b2;
					while (v != a2)
					{
						int prev = m_shape.GetPrevVertex(v);
						int next = m_shape.GetNextVertex(v);
						m_shape.SetPrevVertex_(v, next);
						m_shape.SetNextVertex_(v, prev);
						bVisitedA1 |= v == a1;
						v = next;
					}
					if (!bVisitedA1)
					{
						// a case of two rings being merged
						int prev = m_shape.GetPrevVertex(a2);
						int next = m_shape.GetNextVertex(a2);
						m_shape.SetPrevVertex_(a2, next);
						m_shape.SetNextVertex_(a2, prev);
					}
					// merge happend on the same ring.
					// m_shape.dbgVerifyIntegrity(b1);//debug
					// m_shape.dbgVerifyIntegrity(a1);//debug
					_transferVertexData(a2, a1);
					_beforeRemoveVertex(a2, true);
					m_shape.RemoveVertexInternal_(a2, false);
					_removeAngleSortInfo(a1);
					_transferVertexData(b2, b1);
					_beforeRemoveVertex(b2, true);
					m_shape.RemoveVertexInternal_(b2, false);
					_removeAngleSortInfo(b1);
				}
			}
		}

		// m_shape.dbgVerifyIntegrity(b1);//debug
		// m_shape.dbgVerifyIntegrity(a1);//debug
		private bool _cleanupSpikes()
		{
			bool bModified = false;
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; )
			{
				int vertex = m_shape.GetFirstVertex(path);
				for (int vindex = 0, n = m_shape.GetPathSize(path); vindex < n && n > 1; )
				{
					int prev = m_shape.GetPrevVertex(vertex);
					int next = m_shape.GetNextVertex(vertex);
					if (m_shape.IsEqualXY(prev, next))
					{
						bModified = true;
						_beforeRemoveVertex(vertex, false);
						m_shape.RemoveVertex(vertex, true);
						// not internal, because
						// path is valid at this
						// point
						_beforeRemoveVertex(next, false);
						m_shape.RemoveVertex(next, true);
						vertex = prev;
						vindex = 0;
						n = m_shape.GetPathSize(path);
					}
					else
					{
						vertex = next;
						vindex++;
					}
				}
				if (m_shape.GetPathSize(path) < 2)
				{
					int vertexL = m_shape.GetFirstVertex(path);
					for (int vindex_1 = 0, n = m_shape.GetPathSize(path); vindex_1 < n; vindex_1++)
					{
						_beforeRemoveVertex(vertexL, false);
						vertexL = m_shape.GetNextVertex(vertexL);
					}
					path = m_shape.RemovePath(path);
					bModified = true;
				}
				else
				{
					path = m_shape.GetNextPath(path);
				}
			}
			return bModified;
		}

		private bool _removeSpike(int vertexIn)
		{
			// m_shape.dbgVerifyIntegrity(vertex);//debug
			int vertex = vertexIn;
			// _ASSERT(m_shape.isEqualXY(m_shape.getNextVertex(vertex),
			// m_shape.getPrevVertex(vertex)));
			bool bFound = false;
			while (true)
			{
				int next = m_shape.GetNextVertex(vertex);
				int prev = m_shape.GetPrevVertex(vertex);
				if (next == vertex)
				{
					// last vertex in a ring
					_beforeRemoveVertex(vertex, true);
					m_shape.RemoveVertexInternal_(vertex, false);
					return true;
				}
				if (!m_shape.IsEqualXY(next, prev))
				{
					break;
				}
				bFound = true;
				_removeAngleSortInfo(prev);
				_removeAngleSortInfo(next);
				_beforeRemoveVertex(vertex, true);
				m_shape.RemoveVertexInternal_(vertex, false);
				// m_shape.dbgVerifyIntegrity(prev);//debug
				_transferVertexData(next, prev);
				_beforeRemoveVertex(next, true);
				m_shape.RemoveVertexInternal_(next, true);
				if (next == prev)
				{
					break;
				}
				// deleted the last vertex
				// m_shape.dbgVerifyIntegrity(prev);//debug
				vertex = prev;
			}
			return bFound;
		}

		private void _fixOrphanVertices()
		{
			int pathCount = 0;
			// clean any path info
			for (int node = m_sortedVertices.GetFirst(m_sortedVertices.GetFirstList()); node != -1; node = m_sortedVertices.GetNext(node))
			{
				int vertex = m_sortedVertices.GetData(node);
				m_shape.SetPathToVertex_(vertex, -1);
			}
			int geometrySize = 0;
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; )
			{
				int first = m_shape.GetFirstVertex(path);
				if (first == -1 || m_shape.GetPathFromVertex(first) != -1)
				{
					int p = path;
					path = m_shape.GetNextPath(path);
					m_shape.RemovePathOnly_(p);
					continue;
				}
				m_shape.SetPathToVertex_(first, path);
				int pathSize = 1;
				for (int vertex = m_shape.GetNextVertex(first); vertex != first; vertex = m_shape.GetNextVertex(vertex))
				{
					m_shape.SetPathToVertex_(vertex, path);
					pathSize++;
				}
				m_shape.SetRingAreaValid_(path, false);
				m_shape.SetPathSize_(path, pathSize);
				m_shape.SetLastVertex_(path, m_shape.GetPrevVertex(first));
				geometrySize += pathSize;
				pathCount++;
				path = m_shape.GetNextPath(path);
			}
			// Some vertices do not belong to any path. We have to create new path
			// objects for those.
			// Produce new paths for the orphan vertices.
			for (int node_1 = m_sortedVertices.GetFirst(m_sortedVertices.GetFirstList()); node_1 != -1; node_1 = m_sortedVertices.GetNext(node_1))
			{
				int vertex = m_sortedVertices.GetData(node_1);
				if (m_shape.GetPathFromVertex(vertex) != -1)
				{
					continue;
				}
				int path_1 = m_shape.InsertClosedPath_(m_geometry, -1, vertex, vertex, null);
				geometrySize += m_shape.GetPathSize(path_1);
				pathCount++;
			}
			m_shape.SetGeometryPathCount_(m_geometry, pathCount);
			m_shape.SetGeometryVertexCount_(m_geometry, geometrySize);
			int totalPointCount = 0;
			for (int geometry = m_shape.GetFirstGeometry(); geometry != -1; geometry = m_shape.GetNextGeometry(geometry))
			{
				totalPointCount += m_shape.GetPointCount(geometry);
			}
			m_shape.SetTotalPointCount_(totalPointCount);
		}

		private int _getNextEdgeIndex(int indexIn)
		{
			int index = indexIn;
			for (int i = 0, n = m_bunchEdgeIndices.Size() - 1; i < n; i++)
			{
				index = (index + 1) % m_bunchEdgeIndices.Size();
				if (m_bunchEdgeIndices.Get(index) != -1)
				{
					return index;
				}
			}
			return -1;
		}

		private void _transferVertexData(int vertexFrom, int vertexTo)
		{
			int v1 = m_shape.GetUserIndex(vertexTo, m_userIndexSortedIndexToVertex);
			int v2 = m_shape.GetUserIndex(vertexTo, m_userIndexSortedAngleIndexToVertex);
			m_shape.TransferAllDataToTheVertex(vertexFrom, vertexTo);
			m_shape.SetUserIndex(vertexTo, m_userIndexSortedIndexToVertex, v1);
			m_shape.SetUserIndex(vertexTo, m_userIndexSortedAngleIndexToVertex, v2);
		}

		private void _removeAngleSortInfo(int vertex)
		{
			int angleIndex = m_shape.GetUserIndex(vertex, m_userIndexSortedAngleIndexToVertex);
			if (angleIndex != -1)
			{
				m_bunchEdgeIndices.Set(angleIndex, -1);
				m_shape.SetUserIndex(vertex, m_userIndexSortedAngleIndexToVertex, -1);
			}
		}

		protected internal Simplificator()
		{
			m_dbgCounter = 0;
		}

		public static bool Execute(com.epl.geometry.EditShape shape, int geometry, int knownSimpleResult, bool fixSelfTangency, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.Simplificator simplificator = new com.epl.geometry.Simplificator();
			simplificator.m_shape = shape;
			// simplificator.m_bWinding = bWinding;
			simplificator.m_geometry = geometry;
			simplificator.m_knownSimpleResult = knownSimpleResult;
			simplificator.m_fixSelfTangency = fixSelfTangency;
			simplificator.m_progressTracker = progressTracker;
			return simplificator._simplify();
		}

		internal virtual int _compareVerticesSimple(int v1, int v2)
		{
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			m_shape.GetXY(v1, pt1);
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			m_shape.GetXY(v2, pt2);
			int res = pt1.Compare(pt2);
			if (res == 0)
			{
				// sort equal vertices by the path ID
				int i1 = m_shape.GetPathFromVertex(v1);
				int i2 = m_shape.GetPathFromVertex(v2);
				res = i1 < i2 ? -1 : (i1 == i2 ? 0 : 1);
			}
			return res;
		}

		internal virtual int _compareAngles(int index1, int index2)
		{
			int vert1 = m_bunchEdgeEndPoints.Get(index1);
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			m_shape.GetXY(vert1, pt1);
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			int vert2 = m_bunchEdgeEndPoints.Get(index2);
			m_shape.GetXY(vert2, pt2);
			if (pt1.IsEqual(pt2))
			{
				return 0;
			}
			// overlap case
			int vert10 = m_bunchEdgeCenterPoints.Get(index1);
			com.epl.geometry.Point2D pt10 = new com.epl.geometry.Point2D();
			m_shape.GetXY(vert10, pt10);
			int vert20 = m_bunchEdgeCenterPoints.Get(index2);
			com.epl.geometry.Point2D pt20 = new com.epl.geometry.Point2D();
			m_shape.GetXY(vert20, pt20);
			// _ASSERT(pt10.isEqual(pt20));
			com.epl.geometry.Point2D v1 = new com.epl.geometry.Point2D();
			v1.Sub(pt1, pt10);
			com.epl.geometry.Point2D v2 = new com.epl.geometry.Point2D();
			v2.Sub(pt2, pt20);
			int result = com.epl.geometry.Point2D._compareVectors(v1, v2);
			return result;
		}
	}
}
