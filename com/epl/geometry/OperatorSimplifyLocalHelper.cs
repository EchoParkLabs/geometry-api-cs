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
	internal class OperatorSimplifyLocalHelper
	{
		private sealed class Edge
		{
			internal Edge()
			{
				m_flags = 0;
			}

			internal com.epl.geometry.Segment m_segment;

			internal int m_vertexIndex;

			internal int m_pathIndex;

			internal int m_flags;

			// m_segment.createInstance();
			internal void SetReversed(bool bYesNo)
			{
				m_flags &= (~1);
				m_flags = m_flags | (bYesNo ? 1 : 0);
			}

			// The value returned by GetReversed is interpreted differently in
			// checkSelfIntersections_ and checkValidRingOrientation_
			internal bool GetReversed()
			{
				/* const */
				return (m_flags & 1) != 0;
			}

			internal int GetRightSide()
			{
				/* const */
				return GetReversed() ? 0 : 1;
			}
			// 0 means there should be an
			// emptiness on the right side of
			// the edge, 1 means there is
			// interior
		}

		private readonly com.epl.geometry.VertexDescription m_description;

		private com.epl.geometry.Geometry m_geometry;

		private com.epl.geometry.SpatialReferenceImpl m_sr;

		private int m_dbgCounter;

		private double m_toleranceIsSimple;

		private double m_toleranceSimplify;

		private int m_knownSimpleResult;

		private int m_attributeCount;

		private System.Collections.Generic.List<com.epl.geometry.OperatorSimplifyLocalHelper.Edge> m_edges;

		private com.epl.geometry.AttributeStreamOfInt32 m_FreeEdges;

		private System.Collections.Generic.List<com.epl.geometry.OperatorSimplifyLocalHelper.Edge> m_lineEdgesRecycle;

		private com.epl.geometry.AttributeStreamOfInt32 m_newEdges;

		private com.epl.geometry.SegmentIteratorImpl m_recycledSegIter;

		private com.epl.geometry.IndexMultiDCList m_crossOverHelperList;

		private com.epl.geometry.AttributeStreamOfInt32 m_paths_for_OGC_tests;

		private com.epl.geometry.ProgressTracker m_progressTracker;

		private com.epl.geometry.Treap m_AET;

		private com.epl.geometry.AttributeStreamOfInt32 m_xyToNode1;

		private com.epl.geometry.AttributeStreamOfInt32 m_xyToNode2;

		private com.epl.geometry.AttributeStreamOfInt32 m_pathOrientations;

		private com.epl.geometry.AttributeStreamOfInt32 m_pathParentage;

		private int m_unknownOrientationPathCount;

		private double m_yScanline;

		private com.epl.geometry.AttributeStreamOfDbl m_xy;

		private com.epl.geometry.AttributeStreamOfInt32 m_pairs;

		private com.epl.geometry.AttributeStreamOfInt32 m_pairIndices;

		private com.epl.geometry.EditShape m_editShape;

		private bool m_bOGCRestrictions;

		private bool m_bPlanarSimplify;

		// debugging counter(for breakpoints)
		// private double m_toleranceCluster; //cluster tolerance needs to be
		// sqrt(2) times larger than the tolerance of the other simplify processes.
		// for each vertex, contains -1,
		// or the edge node.
		// for each vertex, contains -1,
		// or the edge node.
		// 0 if undefined, -1 for
		// counterclockwise, 1
		// for clockwise.
		private int IsSimplePlanarImpl_()
		{
			m_bPlanarSimplify = true;
			if (com.epl.geometry.Geometry.IsMultiPath(m_geometry.GetType().Value()))
			{
				if (!CheckStructure_())
				{
					// check structure of geometry(no zero
					// length paths, etc)
					return 0;
				}
				if (!CheckDegenerateSegments_(false))
				{
					// check for degenerate
					// segments(only 2D,no zs or
					// other attributes)
					return 0;
				}
			}
			if (!CheckClustering_())
			{
				// check clustering(points are either
				// coincident,or further than tolerance)
				return 0;
			}
			if (!com.epl.geometry.Geometry.IsMultiPath(m_geometry.GetType().Value()))
			{
				return 2;
			}
			// multipoint is simple
			if (!CheckCracking_())
			{
				// check that there are no self intersections and
				// overlaps among segments.
				return 0;
			}
			if (m_geometry.GetType() == com.epl.geometry.Geometry.Type.Polyline)
			{
				if (!CheckSelfIntersectionsPolylinePlanar_())
				{
					return 0;
				}
				return 2;
			}
			// polyline is simple
			if (!CheckSelfIntersections_())
			{
				// check that there are no other self
				// intersections (for the cases of
				// several segments connect in a point)
				return 0;
			}
			// check that every hole is counterclockwise, and every exterior is
			// clockwise.
			// for the strong simple also check that exterior rings are followed by
			// the interior rings.
			return CheckValidRingOrientation_();
		}

		private bool TestToleranceDistance_(int xyindex1, int xyindex2)
		{
			double x1 = m_xy.Read(2 * xyindex1);
			double y1 = m_xy.Read(2 * xyindex1 + 1);
			double x2 = m_xy.Read(2 * xyindex2);
			double y2 = m_xy.Read(2 * xyindex2 + 1);
			bool b = !com.epl.geometry.Clusterer.IsClusterCandidate_(x1, y1, x2, y2, m_toleranceIsSimple * m_toleranceIsSimple);
			if (!b)
			{
				if (m_geometry.GetDimension() == 0)
				{
					return false;
				}
				return (x1 == x2 && y1 == y2);
			}
			// points either coincide or
			// further,than the tolerance
			return b;
		}

		private bool CheckStructure_()
		{
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			int minsize = multiPathImpl.m_bPolygon ? 3 : 2;
			for (int ipath = 0, npath = multiPathImpl.GetPathCount(); ipath < npath; ipath++)
			{
				if (multiPathImpl.GetPathSize(ipath) < minsize)
				{
					m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.Structure, ipath, 0);
					return false;
				}
			}
			return true;
		}

		private bool CheckDegenerateSegments_(bool bTestZs)
		{
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIter = multiPathImpl.QuerySegmentIterator();
			// Envelope2D env2D;
			bool bHasZ = multiPathImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			double ztolerance = !bHasZ ? 0 : com.epl.geometry.InternalUtils.CalculateZToleranceFromGeometry(m_sr, multiPathImpl, false);
			while (segIter.NextPath())
			{
				while (segIter.HasNextSegment())
				{
					/* const */
					com.epl.geometry.Segment seg = segIter.NextSegment();
					double length = seg.CalculateLength2D();
					if (length > m_toleranceIsSimple)
					{
						continue;
					}
					if (bTestZs && bHasZ)
					{
						double z0 = seg.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0);
						double z1 = seg.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0);
						if (System.Math.Abs(z1 - z0) > ztolerance)
						{
							continue;
						}
					}
					m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.DegenerateSegments, segIter.GetStartPointIndex(), -1);
					return false;
				}
			}
			return true;
		}

		private bool CheckClustering_()
		{
			com.epl.geometry.MultiVertexGeometryImpl multiVertexImpl = (com.epl.geometry.MultiVertexGeometryImpl)m_geometry._getImpl();
			com.epl.geometry.MultiPathImpl multiPathImpl = null;
			if (com.epl.geometry.Geometry.IsMultiPath(m_geometry.GetType().Value()))
			{
				multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			}
			bool get_paths = (m_bPlanarSimplify || m_bOGCRestrictions) && multiPathImpl != null;
			int pointCount = multiVertexImpl.GetPointCount();
			m_xy = (com.epl.geometry.AttributeStreamOfDbl)multiVertexImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			m_pairs = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_pairs.Reserve(pointCount * 2);
			m_pairIndices = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_pairIndices.Reserve(pointCount * 2);
			if (get_paths)
			{
				if (m_paths_for_OGC_tests == null)
				{
					m_paths_for_OGC_tests = new com.epl.geometry.AttributeStreamOfInt32(0);
				}
				m_paths_for_OGC_tests.Reserve(pointCount);
			}
			int ipath = 0;
			for (int i = 0; i < pointCount; i++)
			{
				m_pairs.Add(2 * i);
				// y - tol(BOTTOM)
				m_pairs.Add(2 * i + 1);
				// y + tol(TOP)
				m_pairIndices.Add(2 * i);
				m_pairIndices.Add(2 * i + 1);
				if (get_paths)
				{
					while (i >= multiPathImpl.GetPathEnd(ipath))
					{
						ipath++;
					}
					m_paths_for_OGC_tests.Add(ipath);
				}
			}
			com.epl.geometry.BucketSort sorter = new com.epl.geometry.BucketSort();
			sorter.Sort(m_pairIndices, 0, 2 * pointCount, new com.epl.geometry.OperatorSimplifyLocalHelper.IndexSorter(this, get_paths));
			m_AET.Clear();
			m_AET.SetComparator(new com.epl.geometry.OperatorSimplifyLocalHelper.ClusterTestComparator(this));
			m_AET.SetCapacity(pointCount);
			for (int index = 0, n = pointCount * 2; index < n; index++)
			{
				int pairIndex = m_pairIndices.Get(index);
				int pair = m_pairs.Get(pairIndex);
				int xyindex = pair >> 1;
				// k = 2n or 2n + 1 represent a vertical
				// segment for the same vertex.
				// Therefore, k / 2 represents a vertex
				// index
				// Points need to be either exactly equal or further than 2 *
				// tolerance apart.
				if ((pair & 1) == 0)
				{
					// bottom element
					int aetNode = m_AET.AddElement(xyindex, -1);
					// add it to the AET,end test it against its left and right
					// neighbours.
					int leftneighbour = m_AET.GetPrev(aetNode);
					if (leftneighbour != com.epl.geometry.Treap.NullNode() && !TestToleranceDistance_(m_AET.GetElement(leftneighbour), xyindex))
					{
						m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.Clustering, xyindex, m_AET.GetElement(leftneighbour));
						return false;
					}
					int rightneighbour = m_AET.GetNext(aetNode);
					if (rightneighbour != com.epl.geometry.Treap.NullNode() && !TestToleranceDistance_(m_AET.GetElement(rightneighbour), xyindex))
					{
						m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.Clustering, xyindex, m_AET.GetElement(rightneighbour));
						return false;
					}
				}
				else
				{
					// top
					// get left and right neighbours, and remove the element
					// from AET. Then test the neighbours with the
					// tolerance.
					int aetNode = m_AET.Search(xyindex, -1);
					int leftneighbour = m_AET.GetPrev(aetNode);
					int rightneighbour = m_AET.GetNext(aetNode);
					m_AET.DeleteNode(aetNode, -1);
					if (leftneighbour != com.epl.geometry.Treap.NullNode() && rightneighbour != com.epl.geometry.Treap.NullNode() && !TestToleranceDistance_(m_AET.GetElement(leftneighbour), m_AET.GetElement(rightneighbour)))
					{
						m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.Clustering, m_AET.GetElement(leftneighbour), m_AET.GetElement(rightneighbour));
						return false;
					}
				}
			}
			return true;
		}

		private bool CheckCracking_()
		{
			com.epl.geometry.MultiVertexGeometryImpl multiVertexImpl = (com.epl.geometry.MultiVertexGeometryImpl)m_geometry._getImpl();
			int pointCount = multiVertexImpl.GetPointCount();
			if (pointCount < 10)
			{
				// use brute force for smaller polygons
				return CheckCrackingBrute_();
			}
			else
			{
				return CheckCrackingPlanesweep_();
			}
		}

		private bool CheckCrackingPlanesweep_()
		{
			// cracker,that uses planesweep
			// algorithm.
			com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
			editShape.AddGeometry(m_geometry);
			com.epl.geometry.NonSimpleResult result = new com.epl.geometry.NonSimpleResult();
			bool bNonSimple = com.epl.geometry.Cracker.NeedsCracking(false, editShape, m_toleranceIsSimple, result, m_progressTracker);
			if (bNonSimple)
			{
				result.m_vertexIndex1 = editShape.GetVertexIndex(result.m_vertexIndex1);
				result.m_vertexIndex2 = editShape.GetVertexIndex(result.m_vertexIndex2);
				m_nonSimpleResult.Assign(result);
				return false;
			}
			else
			{
				return true;
			}
		}

		private bool CheckCrackingBrute_()
		{
			// cracker, that uses brute force (a
			// double loop) to find segment
			// intersections.
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			// Implementation without a QuadTreeImpl accelerator
			com.epl.geometry.SegmentIteratorImpl segIter1 = multiPathImpl.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIter2 = multiPathImpl.QuerySegmentIterator();
			// Envelope2D env2D;
			while (segIter1.NextPath())
			{
				while (segIter1.HasNextSegment())
				{
					/* const */
					com.epl.geometry.Segment seg1 = segIter1.NextSegment();
					if (!segIter1.IsLastSegmentInPath() || !segIter1.IsLastPath())
					{
						segIter2.ResetTo(segIter1);
						do
						{
							while (segIter2.HasNextSegment())
							{
								/* const */
								com.epl.geometry.Segment seg2 = segIter2.NextSegment();
								int res = seg1._isIntersecting(seg2, m_toleranceIsSimple, true);
								if (res != 0)
								{
									com.epl.geometry.NonSimpleResult.Reason reason = res == 2 ? com.epl.geometry.NonSimpleResult.Reason.CrossOver : com.epl.geometry.NonSimpleResult.Reason.Cracking;
									m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(reason, segIter1.GetStartPointIndex(), segIter2.GetStartPointIndex());
									return false;
								}
							}
						}
						while (segIter2.NextPath());
					}
				}
			}
			return true;
		}

		private bool CheckSelfIntersections_()
		{
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			m_edges.Clear();
			m_edges.Capacity = 20;
			// we reuse the edges while going through a
			// polygon.
			m_lineEdgesRecycle.Clear();
			m_lineEdgesRecycle.Capacity = 20;
			// we reuse the edges while going
			// through a polygon.
			m_recycledSegIter = multiPathImpl.QuerySegmentIterator();
			m_recycledSegIter.SetCirculator(true);
			com.epl.geometry.AttributeStreamOfInt32 bunch = new com.epl.geometry.AttributeStreamOfInt32(0);
			// stores
			// coincident
			// vertices
			bunch.Reserve(10);
			int pointCount = multiPathImpl.GetPointCount();
			double xprev = com.epl.geometry.NumberUtils.TheNaN;
			double yprev = 0;
			// We already have a sorted list of vertices from clustering check.
			for (int index = 0, n = pointCount * 2; index < n; index++)
			{
				int pairIndex = m_pairIndices.Get(index);
				int pair = m_pairs.Get(pairIndex);
				if ((pair & 1) != 0)
				{
					continue;
				}
				// m_pairs array is redundant. See checkClustering_.
				int xyindex = pair >> 1;
				double x = m_xy.Read(2 * xyindex);
				double y = m_xy.Read(2 * xyindex + 1);
				if (bunch.Size() != 0)
				{
					if (x != xprev || y != yprev)
					{
						if (!ProcessBunchForSelfIntersectionTest_(bunch))
						{
							return false;
						}
						if (bunch != null)
						{
							bunch.Clear(false);
						}
					}
				}
				bunch.Add(xyindex);
				xprev = x;
				yprev = y;
			}
			System.Diagnostics.Debug.Assert((bunch.Size() > 0));
			// cannot be empty
			if (!ProcessBunchForSelfIntersectionTest_(bunch))
			{
				return false;
			}
			return true;
		}

		internal sealed class Vertex_info
		{
			internal double x;

			internal double y;

			internal int ipath;

			internal int ivertex;

			internal bool boundary;
		}

		internal sealed class Vertex_info_pl
		{
			internal double x;

			internal double y;

			internal int ipath;

			internal int ivertex;

			internal bool boundary;

			internal bool end_point;
		}

		internal virtual bool CheckSelfIntersectionsPolylinePlanar_()
		{
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			bool[] closedPaths = new bool[multiPathImpl.GetPathCount()];
			for (int ipath = 0, npaths = multiPathImpl.GetPathCount(); ipath < npaths; ipath++)
			{
				closedPaths[ipath] = multiPathImpl.IsClosedPathInXYPlane(ipath);
			}
			com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pl vi_prev = new com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pl();
			bool is_closed_path;
			int path_start;
			int path_last;
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			{
				// scope
				int pairIndex = m_pairIndices.Get(0);
				int pair = m_pairs.Get(pairIndex);
				int xyindex = pair >> 1;
				m_xy.Read(2 * xyindex, pt);
				int ipath_1 = m_paths_for_OGC_tests.Get(xyindex);
				is_closed_path = closedPaths[ipath_1];
				path_start = multiPathImpl.GetPathStart(ipath_1);
				path_last = multiPathImpl.GetPathEnd(ipath_1) - 1;
				vi_prev.end_point = (xyindex == path_start) || (xyindex == path_last);
				if (m_bOGCRestrictions)
				{
					vi_prev.boundary = !is_closed_path && vi_prev.end_point;
				}
				else
				{
					// for regular planar simplify, only the end points are allowed
					// to coincide
					vi_prev.boundary = vi_prev.end_point;
				}
				vi_prev.ipath = ipath_1;
				vi_prev.x = pt.x;
				vi_prev.y = pt.y;
				vi_prev.ivertex = xyindex;
			}
			com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pl vi = new com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pl();
			for (int index = 1, n = m_pairIndices.Size(); index < n; index++)
			{
				int pairIndex = m_pairIndices.Get(index);
				int pair = m_pairs.Get(pairIndex);
				if ((pair & 1) != 0)
				{
					continue;
				}
				int xyindex = pair >> 1;
				m_xy.Read(2 * xyindex, pt);
				int ipath_1 = m_paths_for_OGC_tests.Get(xyindex);
				if (ipath_1 != vi_prev.ipath)
				{
					is_closed_path = closedPaths[ipath_1];
					path_start = multiPathImpl.GetPathStart(ipath_1);
					path_last = multiPathImpl.GetPathEnd(ipath_1) - 1;
				}
				bool boundary;
				bool end_point = (xyindex == path_start) || (xyindex == path_last);
				if (m_bOGCRestrictions)
				{
					boundary = !is_closed_path && vi_prev.end_point;
				}
				else
				{
					// for regular planar simplify, only the end points are allowed
					// to coincide
					boundary = vi_prev.end_point;
				}
				vi.x = pt.x;
				vi.y = pt.y;
				vi.ipath = ipath_1;
				vi.ivertex = xyindex;
				vi.boundary = boundary;
				vi.end_point = end_point;
				if (vi.x == vi_prev.x && vi.y == vi_prev.y)
				{
					if (m_bOGCRestrictions)
					{
						if (!vi.boundary || !vi_prev.boundary)
						{
							if ((vi.ipath != vi_prev.ipath) || (!vi.end_point && !vi_prev.end_point))
							{
								// check
								// that
								// this
								// is
								// not
								// the
								// endpoints
								// of
								// a
								// closed
								// path
								// one of coincident vertices is not on the boundary
								// this is either Non_simple_result::cross_over or
								// Non_simple_result::ogc_self_tangency.
								// too expensive to distinguish between the two.
								m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.OGCPolylineSelfTangency, vi.ivertex, vi_prev.ivertex);
								return false;
							}
						}
					}
					else
					{
						// common point not on the boundary
						if (!vi.end_point || !vi_prev.end_point)
						{
							// one of
							// coincident
							// vertices is
							// not an
							// endpoint
							m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.CrossOver, vi.ivertex, vi_prev.ivertex);
							return false;
						}
					}
				}
				// common point not on the boundary
				com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pl tmp = vi_prev;
				vi_prev = vi;
				vi = tmp;
			}
			return true;
		}

		internal sealed class Vertex_info_pg
		{
			internal double x;

			internal double y;

			internal int ipath;

			internal int ivertex;

			internal int ipolygon;

			internal Vertex_info_pg(double x_, double y_, int ipath_, int xyindex_, int polygon_)
			{
				x = x_;
				y = y_;
				ipath = ipath_;
				ivertex = xyindex_;
				ipolygon = polygon_;
			}

			internal bool Is_equal(com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg other)
			{
				return x == other.x && y == other.y && ipath == other.ipath && ivertex == other.ivertex && ipolygon == other.ipolygon;
			}
		}

		internal virtual bool Check_self_intersections_polygons_OGC_()
		{
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)(m_geometry._getImpl());
			// OGC MultiPolygon is simple when each Polygon is simple and Polygons a
			// allowed only touch at finite number of vertices.
			// OGC Polygon is simple if it consist of simple LinearRings.
			// LinearRings cannot cross.
			// Any two LinearRings of a OGC Polygon are allowed to touch at single
			// vertex only.
			// The OGC Polygon interior has to be a connected set.
			// At this point we assume that the ring order has to be correct (holes
			// follow corresponding exterior ring).
			// No Rings cross. Exterior rings can only touch at finite number of
			// vertices.
			// Fill a mapping of ring to
			int[] ring_to_polygon = new int[multiPathImpl.GetPathCount()];
			int exteriors = -1;
			bool has_holes = false;
			for (int ipath = 0, n = multiPathImpl.GetPathCount(); ipath < n; ipath++)
			{
				if (multiPathImpl.IsExteriorRing(ipath))
				{
					has_holes = false;
					exteriors++;
					if (ipath < n - 1)
					{
						if (!multiPathImpl.IsExteriorRing(ipath + 1))
						{
							has_holes = true;
						}
					}
				}
				// For OGC polygons with no holes, store -1.
				// For polygons with holes, store polygon index for each ring.
				ring_to_polygon[ipath] = has_holes ? exteriors : -1;
			}
			// Use already sorted m_pairIndices
			com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg vi_prev = null;
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			{
				// scope
				int pairIndex = m_pairIndices.Get(0);
				int pair = m_pairs.Get(pairIndex);
				int xyindex = pair >> 1;
				m_xy.Read(2 * xyindex, pt);
				int ipath_1 = m_paths_for_OGC_tests.Get(xyindex);
				vi_prev = new com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg(pt.x, pt.y, ipath_1, xyindex, ring_to_polygon[ipath_1]);
			}
			System.Collections.Generic.List<com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg> intersections = new System.Collections.Generic.List<com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg>(multiPathImpl.GetPathCount() * 2);
			for (int index = 1, n = m_pairIndices.Size(); index < n; index++)
			{
				int pairIndex = m_pairIndices.Get(index);
				int pair = m_pairs.Get(pairIndex);
				if ((pair & 1) != 0)
				{
					continue;
				}
				int xyindex = pair >> 1;
				m_xy.Read(2 * xyindex, pt);
				int ipath_1 = m_paths_for_OGC_tests.Get(xyindex);
				com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg vi = new com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg(pt.x, pt.y, ipath_1, xyindex, ring_to_polygon[ipath_1]);
				if (vi.x == vi_prev.x && vi.y == vi_prev.y)
				{
					if (vi.ipath == vi_prev.ipath)
					{
						// the ring has self tangency
						m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.OGCPolygonSelfTangency, vi.ivertex, vi_prev.ivertex);
						return false;
					}
					else
					{
						if (ring_to_polygon[vi.ipath] >= 0 && ring_to_polygon[vi.ipath] == ring_to_polygon[vi_prev.ipath])
						{
							// only
							// add
							// rings
							// from
							// polygons
							// with
							// holes.
							// Only
							// interested
							// in
							// touching
							// rings
							// that
							// belong
							// to
							// the
							// same
							// polygon
							if (intersections.Count == 0 || intersections[intersections.Count - 1] != vi_prev)
							{
								intersections.Add(vi_prev);
							}
							intersections.Add(vi);
						}
					}
				}
				vi_prev = vi;
			}
			if (intersections.Count == 0)
			{
				return true;
			}
			// Find disconnected interior cases (OGC spec: Interior of polygon has
			// to be a closed set)
			// Note: Now we'll reuse ring_to_polygon for different purpose - to
			// store mapping from the rings to the graph nodes.
			com.epl.geometry.IndexMultiDCList graph = new com.epl.geometry.IndexMultiDCList(true);
			java.util.Arrays.Fill(ring_to_polygon, -1);
			int vnode_index = -1;
			com.epl.geometry.Point2D prev = new com.epl.geometry.Point2D();
			prev.SetNaN();
			for (int i = 0, n = intersections.Count; i < n; i++)
			{
				com.epl.geometry.OperatorSimplifyLocalHelper.Vertex_info_pg cur = intersections[i];
				if (cur.x != prev.x || cur.y != prev.y)
				{
					vnode_index = graph.CreateList(0);
					prev.x = cur.x;
					prev.y = cur.y;
				}
				int rnode_index = ring_to_polygon[cur.ipath];
				if (rnode_index == -1)
				{
					rnode_index = graph.CreateList(2);
					ring_to_polygon[cur.ipath] = rnode_index;
				}
				graph.AddElement(rnode_index, vnode_index);
				// add to rnode
				// adjacency list the
				// current vnode
				graph.AddElement(vnode_index, rnode_index);
			}
			// add to vnode
			// adjacency list the
			// rnode
			com.epl.geometry.AttributeStreamOfInt32 depth_first_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			depth_first_stack.Reserve(10);
			for (int node = graph.GetFirstList(); node != -1; node = graph.GetNextList(node))
			{
				int ncolor = graph.GetListData(node);
				if ((ncolor & 1) != 0 || (ncolor & 2) == 0)
				{
					continue;
				}
				// already visited or this is a vnode (we do not want
				// to start from vnode).
				int bad_rnode = -1;
				depth_first_stack.Add(node);
				depth_first_stack.Add(-1);
				// parent
				while (depth_first_stack.Size() > 0)
				{
					int cur_node_parent = depth_first_stack.GetLast();
					depth_first_stack.RemoveLast();
					int cur_node = depth_first_stack.GetLast();
					depth_first_stack.RemoveLast();
					int color = graph.GetListData(cur_node);
					if ((color & 1) != 0)
					{
						// already visited this node. This means we found a loop.
						if ((color & 2) == 0)
						{
							// closing on vnode
							bad_rnode = cur_node_parent;
						}
						else
						{
							bad_rnode = cur_node;
						}
						// assert(bad_rnode != -1);
						break;
					}
					graph.SetListData(cur_node, color | 1);
					for (int adjacent_node = graph.GetFirst(cur_node); adjacent_node != -1; adjacent_node = graph.GetNext(adjacent_node))
					{
						int adjacent_node_data = graph.GetData(adjacent_node);
						if (adjacent_node_data == cur_node_parent)
						{
							continue;
						}
						// avoid going back to where we just came from
						depth_first_stack.Add(adjacent_node_data);
						depth_first_stack.Add(cur_node);
					}
				}
				// push cur_node as parent
				// of adjacent_node
				if (bad_rnode != -1)
				{
					int bad_ring_index = -1;
					for (int i_1 = 0, n = ring_to_polygon.Length; i_1 < n; i_1++)
					{
						if (ring_to_polygon[i_1] == bad_rnode)
						{
							bad_ring_index = i_1;
							break;
						}
					}
					// bad_ring_index is any ring in a problematic chain of touching
					// rings.
					// When chain of touching rings form a loop, the result is a
					// disconnected interior,
					// which is non-simple for OGC spec.
					m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.OGCDisconnectedInterior, bad_ring_index, -1);
					return false;
				}
			}
			return true;
		}

		private int CheckValidRingOrientation_()
		{
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			double totalArea = multiPathImpl.CalculateArea2D();
			if (totalArea <= 0)
			{
				m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.RingOrientation, multiPathImpl.GetPathCount() == 1 ? 1 : -1, -1);
				return 0;
			}
			if (multiPathImpl.GetPathCount() == 1)
			{
				// optimization for a single
				// polygon
				if (m_bOGCRestrictions)
				{
					if (!Check_self_intersections_polygons_OGC_())
					{
						return 0;
					}
				}
				return 2;
			}
			// 1.Go through all vertices in the sorted order.
			// 2.For each vertex,insert any non-horizontal segment that has the
			// vertex as low point(there can be max two segments)
			m_pathOrientations = new com.epl.geometry.AttributeStreamOfInt32(multiPathImpl.GetPathCount(), 0);
			m_pathParentage = new com.epl.geometry.AttributeStreamOfInt32(multiPathImpl.GetPathCount(), -1);
			int parent_ring = -1;
			double exteriorArea = 0;
			for (int ipath = 0, n = multiPathImpl.GetPathCount(); ipath < n; ipath++)
			{
				double area = multiPathImpl.CalculateRingArea2D(ipath);
				m_pathOrientations.Write(ipath, area < 0 ? 0 : 256);
				// 8th bit
				// is
				// existing
				// orientation
				if (area > 0)
				{
					parent_ring = ipath;
					exteriorArea = area;
				}
				else
				{
					if (area == 0)
					{
						m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.RingOrientation, ipath, -1);
						return 0;
					}
					else
					{
						// area < 0: this is a hole.
						// We write the parent exterior
						// ring for it (assumed to be first previous exterior ring)
						if (parent_ring < 0 || exteriorArea < System.Math.Abs(area))
						{
							// The first ring is a hole - this is a wrong ring ordering.
							// Or the hole's area is bigger than the previous exterior
							// area - this means ring order is broken,
							// because holes are always smaller. This is not the only
							// condition when ring order is broken though.
							m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.RingOrder, ipath, -1);
							if (m_bOGCRestrictions)
							{
								return 0;
							}
						}
						m_pathParentage.Write(ipath, parent_ring);
					}
				}
			}
			m_unknownOrientationPathCount = multiPathImpl.GetPathCount();
			m_newEdges = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_newEdges.Reserve(10);
			int pointCount = multiPathImpl.GetPointCount();
			m_yScanline = com.epl.geometry.NumberUtils.TheNaN;
			com.epl.geometry.AttributeStreamOfInt32 bunch = new com.epl.geometry.AttributeStreamOfInt32(0);
			// stores
			// coincident
			// vertices
			bunch.Reserve(10);
			// Each vertex has two edges attached.These two arrays map vertices to
			// edges as nodes in the m_AET
			m_xyToNode1 = new com.epl.geometry.AttributeStreamOfInt32(pointCount, com.epl.geometry.Treap.NullNode());
			m_xyToNode2 = new com.epl.geometry.AttributeStreamOfInt32(pointCount, com.epl.geometry.Treap.NullNode());
			if (m_FreeEdges != null)
			{
				m_FreeEdges.Clear(false);
			}
			else
			{
				m_FreeEdges = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			m_FreeEdges.Reserve(10);
			m_AET.Clear();
			m_AET.SetComparator(new com.epl.geometry.OperatorSimplifyLocalHelper.RingOrientationTestComparator(this));
			for (int index = 0, n = pointCount * 2; m_unknownOrientationPathCount > 0 && index < n; index++)
			{
				int pairIndex = m_pairIndices.Get(index);
				int pair = m_pairs.Get(pairIndex);
				if ((pair & 1) != 0)
				{
					continue;
				}
				// m_pairs array is redundant.See checkClustering_.
				int xyindex = pair >> 1;
				double y = m_xy.Read(2 * xyindex + 1);
				if (y != m_yScanline && bunch.Size() != 0)
				{
					if (!ProcessBunchForRingOrientationTest_(bunch))
					{
						// m_nonSimpleResult is set in the
						// processBunchForRingOrientationTest_
						return 0;
					}
					if (bunch != null)
					{
						bunch.Clear(false);
					}
				}
				bunch.Add(xyindex);
				// all vertices that have same y are added to the
				// bunch
				m_yScanline = y;
			}
			if (m_unknownOrientationPathCount > 0 && !ProcessBunchForRingOrientationTest_(bunch))
			{
				// m_nonSimpleResult is set in the
				// processBunchForRingOrientationTest_
				return 0;
			}
			if (m_bOGCRestrictions)
			{
				if (m_nonSimpleResult.m_reason != com.epl.geometry.NonSimpleResult.Reason.NotDetermined)
				{
					return 0;
				}
				// cannot proceed with OGC verification if the ring
				// order is broken (cannot decide polygons then).
				if (!Check_self_intersections_polygons_OGC_())
				{
					return 0;
				}
				return 2;
			}
			else
			{
				// everything is good
				if (m_nonSimpleResult.m_reason == com.epl.geometry.NonSimpleResult.Reason.NotDetermined)
				{
					return 2;
				}
				// everything is good
				// weak simple
				return 1;
			}
		}

		private bool ProcessBunchForSelfIntersectionTest_(com.epl.geometry.AttributeStreamOfInt32 bunch)
		{
			System.Diagnostics.Debug.Assert((bunch.Size() > 0));
			if (bunch.Size() == 1)
			{
				return true;
			}
			System.Diagnostics.Debug.Assert((m_edges.Count == 0));
			// Bunch contains vertices that have exactly same x and y.
			// We populate m_edges array with the edges that originate in the
			// vertices of the bunch.
			for (int i = 0, n = bunch.Size(); i < n; i++)
			{
				int xyindex = bunch.Get(i);
				m_recycledSegIter.ResetToVertex(xyindex);
				// the iterator is
				// circular.
				/* const */
				com.epl.geometry.Segment seg1 = m_recycledSegIter.PreviousSegment();
				m_edges.Add(CreateEdge_(seg1, xyindex, m_recycledSegIter.GetPathIndex(), true));
				m_recycledSegIter.NextSegment();
				// Need to skip one,because of the
				// previousSegment call
				// before (otherwise will get same segment again)
				/* const */
				com.epl.geometry.Segment seg2 = m_recycledSegIter.NextSegment();
				m_edges.Add(CreateEdge_(seg2, xyindex, m_recycledSegIter.GetPathIndex(), false));
			}
			System.Diagnostics.Debug.Assert(((m_edges.Count & 1) == 0));
			// even size
			// Analyze the bunch edges for self intersections(the edges touch at the
			// end points only at this stage of IsSimple)
			// 1.sort the edges by angle between edge and the unit vector along axis
			// x,using the cross product sign.Precondition:no overlaps occur at this
			// stage.
			m_edges.Sort(new com.epl.geometry.OperatorSimplifyLocalHelper.EdgeComparerForSelfIntersection(this));
			// 2.Analyze the bunch.There can be no edges between edges that share
			// same vertex coordinates.
			// We populate a doubly linked list with the edge indices and iterate
			// over this list getting rid of the neighbouring pairs of vertices.
			// The process is similar to peeling an onion.
			// If the list becomes empty,there are no crossovers,otherwise,the
			// geometry has cross-over.
			int list = m_crossOverHelperList.GetFirstList();
			if (list == -1)
			{
				list = m_crossOverHelperList.CreateList(0);
			}
			m_crossOverHelperList.ReserveNodes(m_edges.Count);
			for (int i_1 = 0, n = m_edges.Count; i_1 < n; i_1++)
			{
				m_crossOverHelperList.AddElement(list, i_1);
			}
			// Peel the onion
			bool bContinue = true;
			int i1 = -1;
			int i2 = -1;
			while (bContinue)
			{
				bContinue = false;
				int listnode = m_crossOverHelperList.GetFirst(list);
				if (listnode == -1)
				{
					break;
				}
				int nextnode = m_crossOverHelperList.GetNext(listnode);
				while (nextnode != -1)
				{
					int edgeindex1 = m_crossOverHelperList.GetData(listnode);
					int edgeindex2 = m_crossOverHelperList.GetData(nextnode);
					i1 = m_edges[edgeindex1].m_vertexIndex;
					i2 = m_edges[edgeindex2].m_vertexIndex;
					if (i1 == i2)
					{
						bContinue = true;
						m_crossOverHelperList.DeleteElement(list, listnode);
						listnode = m_crossOverHelperList.GetPrev(nextnode);
						nextnode = m_crossOverHelperList.DeleteElement(list, nextnode);
						if (nextnode == -1 || listnode == -1)
						{
							break;
						}
						else
						{
							continue;
						}
					}
					listnode = nextnode;
					nextnode = m_crossOverHelperList.GetNext(listnode);
				}
			}
			int listSize = m_crossOverHelperList.GetListSize(list);
			m_crossOverHelperList.Clear(list);
			if (listSize > 0)
			{
				// There is self-intersection here.
				m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.CrossOver, i1, i2);
				return false;
			}
			// Recycle the bunch to save on object creation
			for (int i_2 = 0, n = bunch.Size(); i_2 < n; i_2++)
			{
				RecycleEdge_(m_edges[i_2]);
			}
			m_edges.Clear();
			return true;
		}

		private bool ProcessBunchForRingOrientationTest_(com.epl.geometry.AttributeStreamOfInt32 bunch)
		{
			m_dbgCounter++;
			System.Diagnostics.Debug.Assert((bunch.Size() > 0));
			// remove nodes that go out of scope
			for (int i = 0, n = bunch.Size(); i < n; i++)
			{
				int xyindex = bunch.Get(i);
				int aetNode = m_xyToNode1.Read(xyindex);
				if (aetNode != com.epl.geometry.Treap.NullNode())
				{
					// We found that there is an edge
					// in AET, attached to the
					// xyindex vertex. This edge
					// goes out of scope. Delete it
					// from AET.
					int edgeIndex = m_AET.GetElement(aetNode);
					m_FreeEdges.Add(edgeIndex);
					m_AET.DeleteNode(aetNode, -1);
					RecycleEdge_(m_edges[edgeIndex]);
					m_edges[edgeIndex] = null;
					m_xyToNode1.Write(xyindex, com.epl.geometry.Treap.NullNode());
				}
				aetNode = m_xyToNode2.Read(xyindex);
				if (aetNode != com.epl.geometry.Treap.NullNode())
				{
					// We found that there is an edge
					// in AET, attached to the
					// xyindex vertex. This edge
					// goes out of scope. Delete it
					// from AET.
					int edgeIndex = m_AET.GetElement(aetNode);
					m_FreeEdges.Add(edgeIndex);
					m_AET.DeleteNode(aetNode, -1);
					RecycleEdge_(m_edges[edgeIndex]);
					m_edges[edgeIndex] = null;
					m_xyToNode2.Write(xyindex, com.epl.geometry.Treap.NullNode());
				}
			}
			// add new edges to AET
			for (int i_1 = 0, n = bunch.Size(); i_1 < n; i_1++)
			{
				int xyindex = bunch.Get(i_1);
				m_recycledSegIter.ResetToVertex(xyindex);
				// the iterator is
				// circular.
				com.epl.geometry.Segment seg1 = m_recycledSegIter.PreviousSegment();
				// this
				// segment
				// has
				// end
				// point
				// at
				// xyindex
				if (seg1.GetStartY() > seg1.GetEndY())
				{
					// do not allow horizontal
					// segments in here
					// get the top vertex index.We use it to determine what segments
					// to get rid of.
					int edgeTopIndex = m_recycledSegIter.GetStartPointIndex();
					com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge = CreateEdge_(seg1, xyindex, m_recycledSegIter.GetPathIndex(), true);
					int edgeIndex;
					if (m_FreeEdges.Size() > 0)
					{
						edgeIndex = m_FreeEdges.GetLast();
						m_FreeEdges.RemoveLast();
						m_edges[edgeIndex] = edge;
					}
					else
					{
						edgeIndex = m_edges.Count;
						m_edges.Add(edge);
					}
					int aetNode = m_AET.AddElement(edgeIndex, -1);
					// Remember AET nodes in the vertex to AET node maps.
					if (m_xyToNode1.Read(edgeTopIndex) == com.epl.geometry.Treap.NullNode())
					{
						m_xyToNode1.Write(edgeTopIndex, aetNode);
					}
					else
					{
						System.Diagnostics.Debug.Assert((m_xyToNode2.Read(edgeTopIndex) == com.epl.geometry.Treap.NullNode()));
						m_xyToNode2.Write(edgeTopIndex, aetNode);
					}
					// If this edge belongs to a path that has not have direction
					// figured out yet,
					// add it to m_newEdges for post processing
					if ((m_pathOrientations.Read(m_recycledSegIter.GetPathIndex()) & 3) == 0)
					{
						m_newEdges.Add(aetNode);
					}
				}
				m_recycledSegIter.NextSegment();
				// Need to skip one,because of the
				// previousSegment call
				// before(otherwise will get same
				// segment again)
				// seg1 is invalid now
				com.epl.geometry.Segment seg2 = m_recycledSegIter.NextSegment();
				// start has to be lower than end for this one
				if (seg2.GetStartY() < seg2.GetEndY())
				{
					// do not allow horizontal
					// segments in here
					// get the top vertex index.We use it to determine what segments
					// to get rid of.
					int edgeTopIndex = m_recycledSegIter.GetEndPointIndex();
					com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge = CreateEdge_(seg2, xyindex, m_recycledSegIter.GetPathIndex(), false);
					int edgeIndex;
					if (m_FreeEdges.Size() > 0)
					{
						edgeIndex = m_FreeEdges.GetLast();
						m_FreeEdges.RemoveLast();
						m_edges[edgeIndex] = edge;
					}
					else
					{
						edgeIndex = m_edges.Count;
						m_edges.Add(edge);
					}
					int aetNode = m_AET.AddElement(edgeIndex, -1);
					if (m_xyToNode1.Read(edgeTopIndex) == com.epl.geometry.Treap.NullNode())
					{
						m_xyToNode1.Write(edgeTopIndex, aetNode);
					}
					else
					{
						System.Diagnostics.Debug.Assert((m_xyToNode2.Read(edgeTopIndex) == com.epl.geometry.Treap.NullNode()));
						m_xyToNode2.Write(edgeTopIndex, aetNode);
					}
					// If this edge belongs to a path that has not have direction
					// figured out yet,
					// add it to m_newEdges for post processing
					if ((m_pathOrientations.Read(m_recycledSegIter.GetPathIndex()) & 3) == 0)
					{
						m_newEdges.Add(aetNode);
					}
				}
			}
			for (int i_2 = 0, n = m_newEdges.Size(); i_2 < n && m_unknownOrientationPathCount > 0; i_2++)
			{
				int aetNode = m_newEdges.Get(i_2);
				int edgeIndexInitial = m_AET.GetElement(aetNode);
				com.epl.geometry.OperatorSimplifyLocalHelper.Edge edgeInitial = m_edges[edgeIndexInitial];
				int pathIndexInitial = edgeInitial.m_pathIndex;
				int directionInitial = m_pathOrientations.Read(pathIndexInitial);
				if ((directionInitial & 3) == 0)
				{
					int prevExteriorPath = -1;
					int node = m_AET.GetPrev(aetNode);
					int prevNode = aetNode;
					int oddEven = 0;
					{
						// scope
						int edgeIndex = -1;
						com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge = null;
						int pathIndex = -1;
						int dir = 0;
						// find the leftmost edge for which the ring orientation is
						// known
						while (node != com.epl.geometry.Treap.NullNode())
						{
							edgeIndex = m_AET.GetElement(node);
							edge = m_edges[edgeIndex];
							pathIndex = edge.m_pathIndex;
							dir = m_pathOrientations.Read(pathIndex);
							if ((dir & 3) != 0)
							{
								break;
							}
							prevNode = node;
							node = m_AET.GetPrev(node);
						}
						if (node == com.epl.geometry.Treap.NullNode())
						{
							// if no edges have ring
							// orientation known, then
							// start
							// from the left most and it
							// has
							// to be exterior ring.
							oddEven = 1;
							node = prevNode;
						}
						else
						{
							if ((dir & 3) == 1)
							{
								prevExteriorPath = pathIndex;
							}
							else
							{
								prevExteriorPath = m_pathParentage.Read(pathIndex);
							}
							oddEven = (edge.GetRightSide() != 0) ? 0 : 1;
							node = m_AET.GetNext(node);
						}
					}
					do
					{
						int edgeIndex = m_AET.GetElement(node);
						com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge = m_edges[edgeIndex];
						int pathIndex = edge.m_pathIndex;
						int direction = m_pathOrientations.Read(pathIndex);
						if ((direction & 3) == 0)
						{
							if (oddEven != edge.GetRightSide())
							{
								m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.RingOrientation, pathIndex, -1);
								return false;
							}
							// wrong ring orientation
							int dir = (oddEven != 0 && !edge.GetReversed()) ? 1 : 2;
							direction = (direction & unchecked((int)(0xfc))) | dir;
							m_pathOrientations.Write(pathIndex, dir);
							if (dir == 2 && m_nonSimpleResult.m_reason == com.epl.geometry.NonSimpleResult.Reason.NotDetermined)
							{
								// check that this hole has a correct parent
								// exterior ring.
								int parent = m_pathParentage.Read(pathIndex);
								if (parent != prevExteriorPath)
								{
									m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.RingOrder, pathIndex, -1);
									if (m_bOGCRestrictions)
									{
										return false;
									}
								}
							}
							m_unknownOrientationPathCount--;
							if (m_unknownOrientationPathCount == 0)
							{
								// if(!m_unknownOrientationPathCount)
								return true;
							}
						}
						if ((direction & 3) == 1)
						{
							prevExteriorPath = pathIndex;
						}
						prevNode = node;
						node = m_AET.GetNext(node);
						oddEven = oddEven != 0 ? 0 : 1;
					}
					while (prevNode != aetNode);
				}
			}
			if (m_newEdges != null)
			{
				m_newEdges.Clear(false);
			}
			else
			{
				m_newEdges = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			return true;
		}

		private com.epl.geometry.OperatorSimplifyLocalHelper.Edge CreateEdge_(com.epl.geometry.Segment seg, int xyindex, int pathIndex, bool bReversed)
		{
			/* const */
			com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge;
			com.epl.geometry.Geometry.Type gt = seg.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Line)
			{
				edge = CreateEdgeLine_(seg);
			}
			else
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// implement
			// recycling for
			// curves
			edge.m_vertexIndex = xyindex;
			edge.m_pathIndex = pathIndex;
			edge.m_flags = 0;
			edge.SetReversed(bReversed);
			return edge;
		}

		private com.epl.geometry.OperatorSimplifyLocalHelper.Edge CreateEdgeLine_(com.epl.geometry.Segment seg)
		{
			/* const */
			com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge = null;
			if (m_lineEdgesRecycle.Count > 0)
			{
				int indexLast = m_lineEdgesRecycle.Count - 1;
				edge = m_lineEdgesRecycle[indexLast];
				m_lineEdgesRecycle.RemoveAt(indexLast);
				seg.CopyTo(edge.m_segment);
			}
			else
			{
				edge = new com.epl.geometry.OperatorSimplifyLocalHelper.Edge();
				edge.m_segment = (com.epl.geometry.Segment)com.epl.geometry.Segment._clone(seg);
			}
			return edge;
		}

		private void RecycleEdge_(com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge)
		{
			/* const */
			com.epl.geometry.Geometry.Type gt = edge.m_segment.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Line)
			{
				m_lineEdgesRecycle.Add(edge);
			}
		}

		private sealed class ClusterTestComparator : com.epl.geometry.Treap.Comparator
		{
			internal com.epl.geometry.OperatorSimplifyLocalHelper m_helper;

			internal ClusterTestComparator(com.epl.geometry.OperatorSimplifyLocalHelper helper)
			{
				m_helper = helper;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int xy1, int node)
			{
				/* const */
				int xy2 = treap.GetElement(node);
				double x1 = m_helper.m_xy.Read(2 * xy1);
				double x2 = m_helper.m_xy.Read(2 * xy2);
				double dx = x1 - x2;
				return dx < 0 ? -1 : (dx > 0 ? 1 : 0);
			}
		}

		private sealed class RingOrientationTestComparator : com.epl.geometry.Treap.Comparator
		{
			private com.epl.geometry.OperatorSimplifyLocalHelper m_helper;

			internal RingOrientationTestComparator(com.epl.geometry.OperatorSimplifyLocalHelper helper)
			{
				m_helper = helper;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int left, int node)
			{
				/* const */
				int right = treap.GetElement(node);
				com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge1 = m_helper.m_edges[left];
				com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge2 = m_helper.m_edges[right];
				bool bEdge1Reversed = edge1.GetReversed();
				bool bEdge2Reversed = edge2.GetReversed();
				double x1 = edge1.m_segment.IntersectionOfYMonotonicWithAxisX(m_helper.m_yScanline, 0);
				double x2 = edge2.m_segment.IntersectionOfYMonotonicWithAxisX(m_helper.m_yScanline, 0);
				if (x1 == x2)
				{
					// apparently these edges originate from same vertex and the
					// scanline is on the vertex.move scanline a little.
					double y1 = bEdge1Reversed ? edge1.m_segment.GetStartY() : edge1.m_segment.GetEndY();
					double y2 = bEdge2Reversed ? edge2.m_segment.GetStartY() : edge2.m_segment.GetEndY();
					double miny = System.Math.Min(y1, y2);
					double y = (miny - m_helper.m_yScanline) * 0.5 + m_helper.m_yScanline;
					if (y == m_helper.m_yScanline)
					{
						// assert(0); //ST: not a bug. just curious to see this
						// happens.
						y = miny;
					}
					// apparently, one of the segments is almost
					// horizontal line.
					x1 = edge1.m_segment.IntersectionOfYMonotonicWithAxisX(y, 0);
					x2 = edge2.m_segment.IntersectionOfYMonotonicWithAxisX(y, 0);
					System.Diagnostics.Debug.Assert((x1 != x2));
				}
				return x1 < x2 ? -1 : (x1 > x2 ? 1 : 0);
			}
		}

		internal virtual int MultiPointIsSimpleAsFeature_()
		{
			com.epl.geometry.MultiVertexGeometryImpl multiVertexImpl = (com.epl.geometry.MultiVertexGeometryImpl)m_geometry._getImpl();
			// sort lexicographically: by y,then by x, then by other attributes in
			// the order.
			// Go through the sorted list and make sure no points coincide exactly
			// (no tolerance is taken into account).
			int pointCount = multiVertexImpl.GetPointCount();
			com.epl.geometry.AttributeStreamOfInt32 indices = new com.epl.geometry.AttributeStreamOfInt32(0);
			for (int i = 0; i < pointCount; i++)
			{
				indices.Add(i);
			}
			indices.Sort(0, pointCount, new com.epl.geometry.OperatorSimplifyLocalHelper.MultiPointVertexComparer(this));
			for (int i_1 = 1; i_1 < pointCount; i_1++)
			{
				if (CompareVerticesMultiPoint_(indices.Get(i_1 - 1), indices.Get(i_1)) == 0)
				{
					m_nonSimpleResult = new com.epl.geometry.NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason.Clustering, indices.Get(i_1 - 1), indices.Get(i_1));
					return 0;
				}
			}
			// points are coincident-simplify.
			return 2;
		}

		internal virtual int PolylineIsSimpleAsFeature_()
		{
			if (!CheckStructure_())
			{
				return 0;
			}
			// Non planar IsSimple.
			// Go through all line segments and make sure no line segments are
			// degenerate.
			// Degenerate segment is the one which has its length shorter than
			// tolerance or Z projection shorter than z tolerance.
			return CheckDegenerateSegments_(true) ? 2 : 0;
		}

		internal virtual int PolygonIsSimpleAsFeature_()
		{
			return IsSimplePlanarImpl_();
		}

		internal virtual com.epl.geometry.MultiPoint MultiPointSimplifyAsFeature_()
		{
			com.epl.geometry.MultiVertexGeometryImpl multiVertexImpl = (com.epl.geometry.MultiVertexGeometryImpl)m_geometry._getImpl();
			// sort lexicographically:by y,then by x,then by other attributes in the
			// order.
			int pointCount = multiVertexImpl.GetPointCount();
			System.Diagnostics.Debug.Assert((pointCount > 0));
			com.epl.geometry.AttributeStreamOfInt32 indices = new com.epl.geometry.AttributeStreamOfInt32(0);
			for (int i = 0; i < pointCount; i++)
			{
				indices.Add(i);
			}
			indices.Sort(0, pointCount, new com.epl.geometry.OperatorSimplifyLocalHelper.MultiPointVertexComparer2(this));
			// Mark vertices that are unique
			bool[] indicesOut = new bool[pointCount];
			indicesOut[indices.Get(0)] = true;
			for (int i_1 = 1; i_1 < pointCount; i_1++)
			{
				int ind1 = indices.Get(i_1 - 1);
				int ind2 = indices.Get(i_1);
				if (CompareVerticesMultiPoint_(ind1, ind2) == 0)
				{
					indicesOut[ind2] = false;
					continue;
				}
				indicesOut[ind2] = true;
			}
			// get rid of non-unique vertices.
			// We preserve the order of MultiPoint vertices.Among duplicate
			// vertices,those that have
			// higher index are deleted.
			com.epl.geometry.MultiPoint dst = (com.epl.geometry.MultiPoint)m_geometry.CreateInstance();
			com.epl.geometry.MultiPoint src = (com.epl.geometry.MultiPoint)m_geometry;
			int istart = 0;
			int iend = 1;
			for (int i_2 = 0; i_2 < pointCount; i_2++)
			{
				if (indicesOut[i_2])
				{
					iend = i_2 + 1;
				}
				else
				{
					if (istart < iend)
					{
						dst.Add(src, istart, iend);
					}
					istart = i_2 + 1;
				}
			}
			if (istart < iend)
			{
				dst.Add(src, istart, iend);
			}
			((com.epl.geometry.MultiVertexGeometryImpl)dst._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, m_toleranceSimplify, false);
			return dst;
		}

		internal virtual com.epl.geometry.Polyline PolylineSimplifyAsFeature_()
		{
			// Non planar simplify.
			// Go through all line segments and make sure no line segments are
			// degenerate.
			// Degenerate segment is the one which has its length shorter than
			// tolerance or Z projection shorter than z tolerance.
			// The algorithm processes each path symmetrically from each end to
			// ensure the result of simplify does not depend on the direction of the
			// path.
			com.epl.geometry.MultiPathImpl multiPathImpl = (com.epl.geometry.MultiPathImpl)m_geometry._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIterFwd = multiPathImpl.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterBwd = multiPathImpl.QuerySegmentIterator();
			com.epl.geometry.Polyline dst = (com.epl.geometry.Polyline)m_geometry.CreateInstance();
			com.epl.geometry.Polyline src = (com.epl.geometry.Polyline)m_geometry;
			// Envelope2D env2D;
			bool bHasZ = multiPathImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			double ztolerance = !bHasZ ? 0.0 : com.epl.geometry.InternalUtils.CalculateZToleranceFromGeometry(m_sr, multiPathImpl, true);
			com.epl.geometry.AttributeStreamOfInt32 fwdStack = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 bwdStack = new com.epl.geometry.AttributeStreamOfInt32(0);
			fwdStack.Reserve(multiPathImpl.GetPointCount() / 2 + 1);
			bwdStack.Reserve(multiPathImpl.GetPointCount() / 2 + 1);
			while (segIterFwd.NextPath())
			{
				segIterBwd.NextPath();
				if (multiPathImpl.GetPathSize(segIterFwd.GetPathIndex()) < 2)
				{
					continue;
				}
				segIterBwd.ResetToLastSegment();
				double lengthFwd = 0;
				double lengthBwd = 0;
				bool bFirst = true;
				while (segIterFwd.HasNextSegment())
				{
					System.Diagnostics.Debug.Assert((segIterBwd.HasPreviousSegment()));
					/* const */
					com.epl.geometry.Segment segFwd = segIterFwd.NextSegment();
					/* const */
					com.epl.geometry.Segment segBwd = segIterBwd.PreviousSegment();
					int idx1 = segIterFwd.GetStartPointIndex();
					int idx2 = segIterBwd.GetStartPointIndex();
					if (idx1 > idx2)
					{
						break;
					}
					if (bFirst)
					{
						// add the very first and the very last point indices
						fwdStack.Add(segIterFwd.GetStartPointIndex());
						// first goes
						// to
						// fwdStack
						bwdStack.Add(segIterBwd.GetEndPointIndex());
						// last goes to
						// bwdStack
						bFirst = false;
					}
					{
						int index0 = fwdStack.GetLast();
						int index1 = segIterFwd.GetEndPointIndex();
						if (index1 - index0 > 1)
						{
							com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
							pt.Sub(multiPathImpl.GetXY(index0), multiPathImpl.GetXY(index1));
							lengthFwd = pt.Length();
						}
						else
						{
							lengthFwd = segFwd.CalculateLength2D();
						}
					}
					{
						int index0 = bwdStack.GetLast();
						int index1 = segIterBwd.GetStartPointIndex();
						if (index1 - index0 > 1)
						{
							com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
							pt.Sub(multiPathImpl.GetXY(index0), multiPathImpl.GetXY(index1));
							lengthBwd = pt.Length();
						}
						else
						{
							lengthBwd = segBwd.CalculateLength2D();
						}
					}
					if (lengthFwd > m_toleranceSimplify)
					{
						fwdStack.Add(segIterFwd.GetEndPointIndex());
						lengthFwd = 0;
					}
					else
					{
						if (bHasZ)
						{
							double z0 = multiPathImpl.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, fwdStack.GetLast(), 0);
							double z1 = segFwd.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0);
							if (System.Math.Abs(z1 - z0) > ztolerance)
							{
								fwdStack.Add(segIterFwd.GetEndPointIndex());
								lengthFwd = 0;
							}
						}
					}
					if (lengthBwd > m_toleranceSimplify)
					{
						bwdStack.Add(segIterBwd.GetStartPointIndex());
						lengthBwd = 0;
					}
					else
					{
						if (bHasZ)
						{
							double z0 = multiPathImpl.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, bwdStack.GetLast(), 0);
							double z1 = segBwd.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0);
							if (System.Math.Abs(z1 - z0) > ztolerance)
							{
								bwdStack.Add(segIterBwd.GetStartPointIndex());
								lengthBwd = 0;
							}
						}
					}
				}
				// assert(fwdStack.getLast() <= bwdStack.getLast());
				if (fwdStack.GetLast() < bwdStack.GetLast())
				{
					// There is degenerate segment in the middle. Remove.
					// If the path degenerate, this will make fwdStack.size() +
					// bwdStack.size() < 2.
					if (fwdStack.Size() > bwdStack.Size())
					{
						fwdStack.RemoveLast();
					}
					else
					{
						bwdStack.RemoveLast();
					}
				}
				else
				{
					if (fwdStack.GetLast() == bwdStack.GetLast())
					{
						bwdStack.RemoveLast();
					}
					else
					{
						System.Diagnostics.Debug.Assert((fwdStack.GetLast() - bwdStack.GetLast() == 1));
						bwdStack.RemoveLast();
						bwdStack.RemoveLast();
					}
				}
				if (bwdStack.Size() + fwdStack.Size() >= 2)
				{
					// Completely ignore the curves for now.
					com.epl.geometry.Point point = new com.epl.geometry.Point();
					for (int i = 0, n = fwdStack.Size(); i < n; i++)
					{
						src.GetPointByVal(fwdStack.Get(i), point);
						if (i == 0)
						{
							dst.StartPath(point);
						}
						else
						{
							dst.LineTo(point);
						}
					}
					// int prevIdx = fwdStack.getLast();
					for (int i_1 = bwdStack.Size() - 1; i_1 > 0; i_1--)
					{
						src.GetPointByVal(bwdStack.Get(i_1), point);
						dst.LineTo(point);
					}
					if (src.IsClosedPath(segIterFwd.GetPathIndex()))
					{
						dst.ClosePathWithLine();
					}
					else
					{
						if (bwdStack.Size() > 0)
						{
							src.GetPointByVal(bwdStack.Get(0), point);
							dst.LineTo(point);
						}
					}
				}
				// degenerate path won't be added
				if (fwdStack != null)
				{
					fwdStack.Clear(false);
				}
				if (bwdStack != null)
				{
					bwdStack.Clear(false);
				}
			}
			((com.epl.geometry.MultiVertexGeometryImpl)dst._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, m_toleranceSimplify, false);
			return dst;
		}

		internal virtual com.epl.geometry.Polygon PolygonSimplifyAsFeature_()
		{
			return (com.epl.geometry.Polygon)SimplifyPlanar_();
		}

		internal virtual com.epl.geometry.MultiVertexGeometry SimplifyPlanar_()
		{
			// do clustering/cracking loop
			// if (false)
			// {
			// ((MultiPathImpl)m_geometry._getImpl()).saveToTextFileDbg("c:/temp/_simplifyDbg0.txt");
			// }
			if (m_geometry.GetType() == com.epl.geometry.Geometry.Type.Polygon)
			{
				if (((com.epl.geometry.Polygon)m_geometry).GetFillRule() == com.epl.geometry.Polygon.FillRule.enumFillRuleWinding)
				{
					// when the fill rule is winding, we need to call a special
					// method.
					return com.epl.geometry.TopologicalOperations.PlanarSimplify((com.epl.geometry.MultiVertexGeometry)m_geometry, m_toleranceSimplify, true, false, m_progressTracker);
				}
			}
			m_editShape = new com.epl.geometry.EditShape();
			m_editShape.AddGeometry(m_geometry);
			if (m_editShape.GetTotalPointCount() != 0)
			{
				System.Diagnostics.Debug.Assert((m_knownSimpleResult != com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong));
				if (m_knownSimpleResult != com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak)
				{
					com.epl.geometry.CrackAndCluster.Execute(m_editShape, m_toleranceSimplify, m_progressTracker, true);
				}
				if (m_geometry.GetType().Equals(com.epl.geometry.Geometry.Type.Polygon))
				{
					com.epl.geometry.Simplificator.Execute(m_editShape, m_editShape.GetFirstGeometry(), m_knownSimpleResult, false, m_progressTracker);
				}
			}
			m_geometry = m_editShape.GetGeometry(m_editShape.GetFirstGeometry());
			// extract
			// the
			// result
			// of
			// simplify
			if (m_geometry.GetType().Equals(com.epl.geometry.Geometry.Type.Polygon))
			{
				((com.epl.geometry.MultiPathImpl)m_geometry._getImpl())._updateOGCFlags();
				((com.epl.geometry.Polygon)m_geometry).SetFillRule(com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
			}
			// We have simplified the geometry using the given tolerance. Now mark
			// the geometry as strong simple,
			// So that the next call will not have to repeat these steps.
			((com.epl.geometry.MultiVertexGeometryImpl)m_geometry._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, m_toleranceSimplify, false);
			return (com.epl.geometry.MultiVertexGeometry)(m_geometry);
		}

		internal com.epl.geometry.NonSimpleResult m_nonSimpleResult;

		internal OperatorSimplifyLocalHelper(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, int knownSimpleResult, com.epl.geometry.ProgressTracker progressTracker, bool bOGCRestrictions)
		{
			m_description = geometry.GetDescription();
			m_geometry = geometry;
			m_sr = (com.epl.geometry.SpatialReferenceImpl)spatialReference;
			m_dbgCounter = 0;
			m_toleranceIsSimple = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_sr, geometry, false);
			m_toleranceSimplify = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_sr, geometry, true);
			// m_toleranceCluster = m_toleranceSimplify * Math.sqrt(2.0) * 1.00001;
			m_knownSimpleResult = knownSimpleResult;
			m_attributeCount = m_description.GetAttributeCount();
			m_edges = new System.Collections.Generic.List<com.epl.geometry.OperatorSimplifyLocalHelper.Edge>();
			m_lineEdgesRecycle = new System.Collections.Generic.List<com.epl.geometry.OperatorSimplifyLocalHelper.Edge>();
			m_crossOverHelperList = new com.epl.geometry.IndexMultiDCList();
			m_AET = new com.epl.geometry.Treap();
			m_nonSimpleResult = new com.epl.geometry.NonSimpleResult();
			m_bOGCRestrictions = bOGCRestrictions;
			m_bPlanarSimplify = m_bOGCRestrictions;
		}

		// Returns 0 non-simple, 1 weak simple, 2 strong simple
		/// <summary>The code is executed in the 2D plane only.Attributes are ignored.</summary>
		/// <remarks>
		/// The code is executed in the 2D plane only.Attributes are ignored.
		/// MultiPoint-check for clustering. Polyline -check for clustering and
		/// cracking. Polygon -check for clustering,cracking,absence of
		/// self-intersections,and correct ring ordering.
		/// </remarks>
		protected internal static int IsSimplePlanar(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, bool bForce, com.epl.geometry.ProgressTracker progressTracker)
		{
			/* const */
			/* const */
			System.Diagnostics.Debug.Assert((false));
			// this code is not called yet.
			if (geometry.IsEmpty())
			{
				return 1;
			}
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				return 1;
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.Type.Envelope)
				{
					com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
					geometry.QueryEnvelope2D(env2D);
					bool bReturnValue = !env2D.IsDegenerate(com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false));
					return bReturnValue ? 1 : 0;
				}
				else
				{
					if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					else
					{
						// return seg.IsSimple(m_tolerance);
						if (!com.epl.geometry.Geometry.IsMultiVertex(gt.Value()))
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
				}
			}
			// What else?
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false);
			double geomTolerance = 0;
			int isSimple = ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl()).GetIsSimple(tolerance);
			int knownSimpleResult = bForce ? -1 : isSimple;
			// TODO: need to distinguish KnownSimple between SimpleAsFeature and
			// SimplePlanar. The SimplePlanar implies SimpleAsFeature.
			if (knownSimpleResult != -1)
			{
				return knownSimpleResult;
			}
			if (knownSimpleResult == com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak)
			{
				System.Diagnostics.Debug.Assert((tolerance <= geomTolerance));
				tolerance = geomTolerance;
			}
			// OVERRIDE the tolerance.
			com.epl.geometry.OperatorSimplifyLocalHelper helper = new com.epl.geometry.OperatorSimplifyLocalHelper(geometry, spatialReference, knownSimpleResult, progressTracker, false);
			knownSimpleResult = helper.IsSimplePlanarImpl_();
			((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl()).SetIsSimple(knownSimpleResult, tolerance, false);
			return knownSimpleResult;
		}

		/// <summary>
		/// Checks if Geometry is simple for storing in DB:
		/// MultiPoint:check that no points coincide.tolerance is ignored.
		/// </summary>
		/// <remarks>
		/// Checks if Geometry is simple for storing in DB:
		/// MultiPoint:check that no points coincide.tolerance is ignored.
		/// Polyline:ensure there no segments degenerate segments. Polygon:Same as
		/// IsSimplePlanar.
		/// </remarks>
		protected internal static int IsSimpleAsFeature(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, bool bForce, com.epl.geometry.NonSimpleResult result, com.epl.geometry.ProgressTracker progressTracker)
		{
			/* const */
			/* const */
			if (result != null)
			{
				result.m_reason = com.epl.geometry.NonSimpleResult.Reason.NotDetermined;
				result.m_vertexIndex1 = -1;
				result.m_vertexIndex2 = -1;
			}
			if (geometry.IsEmpty())
			{
				return 1;
			}
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				return 1;
			}
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false);
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				/* const */
				com.epl.geometry.Envelope env = (com.epl.geometry.Envelope)geometry;
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				env.QueryEnvelope2D(env2D);
				if (env2D.IsDegenerate(tolerance))
				{
					if (result != null)
					{
						result.m_reason = com.epl.geometry.NonSimpleResult.Reason.DegenerateSegments;
						result.m_vertexIndex1 = -1;
						result.m_vertexIndex2 = -1;
					}
					return 0;
				}
				return 1;
			}
			else
			{
				if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
				{
					/* const */
					com.epl.geometry.Segment seg = (com.epl.geometry.Segment)geometry;
					com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(seg.GetDescription());
					polyline.AddSegment(seg, true);
					return IsSimpleAsFeature(polyline, spatialReference, bForce, result, progressTracker);
				}
			}
			// double geomTolerance = 0;
			int isSimple = ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl()).GetIsSimple(tolerance);
			int knownSimpleResult = bForce ? -1 : isSimple;
			// TODO: need to distinguish KnownSimple between SimpleAsFeature and
			// SimplePlanar.
			// From the first sight it seems the SimplePlanar implies
			// SimpleAsFeature.
			if (knownSimpleResult != -1)
			{
				return knownSimpleResult;
			}
			com.epl.geometry.OperatorSimplifyLocalHelper helper = new com.epl.geometry.OperatorSimplifyLocalHelper(geometry, spatialReference, knownSimpleResult, progressTracker, false);
			if (gt == com.epl.geometry.Geometry.Type.MultiPoint)
			{
				knownSimpleResult = helper.MultiPointIsSimpleAsFeature_();
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.Type.Polyline)
				{
					knownSimpleResult = helper.PolylineIsSimpleAsFeature_();
				}
				else
				{
					if (gt == com.epl.geometry.Geometry.Type.Polygon)
					{
						knownSimpleResult = helper.PolygonIsSimpleAsFeature_();
					}
					else
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
				}
			}
			// what else?
			((com.epl.geometry.MultiVertexGeometryImpl)(geometry._getImpl())).SetIsSimple(knownSimpleResult, tolerance, false);
			if (result != null && knownSimpleResult == 0)
			{
				result.Assign(helper.m_nonSimpleResult);
			}
			return knownSimpleResult;
		}

		internal static int IsSimpleOGC(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, bool bForce, com.epl.geometry.NonSimpleResult result, com.epl.geometry.ProgressTracker progressTracker)
		{
			/* const */
			/* const */
			if (result != null)
			{
				result.m_reason = com.epl.geometry.NonSimpleResult.Reason.NotDetermined;
				result.m_vertexIndex1 = -1;
				result.m_vertexIndex2 = -1;
			}
			if (geometry.IsEmpty())
			{
				return 1;
			}
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				return 1;
			}
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false);
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				/* const */
				com.epl.geometry.Envelope env = (com.epl.geometry.Envelope)geometry;
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				env.QueryEnvelope2D(env2D);
				if (env2D.IsDegenerate(tolerance))
				{
					if (result != null)
					{
						result.m_reason = com.epl.geometry.NonSimpleResult.Reason.DegenerateSegments;
						result.m_vertexIndex1 = -1;
						result.m_vertexIndex2 = -1;
					}
					return 0;
				}
				return 1;
			}
			else
			{
				if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
				{
					/* const */
					com.epl.geometry.Segment seg = (com.epl.geometry.Segment)geometry;
					com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(seg.GetDescription());
					polyline.AddSegment(seg, true);
					return IsSimpleAsFeature(polyline, spatialReference, bForce, result, progressTracker);
				}
			}
			int knownSimpleResult = -1;
			com.epl.geometry.OperatorSimplifyLocalHelper helper = new com.epl.geometry.OperatorSimplifyLocalHelper(geometry, spatialReference, knownSimpleResult, progressTracker, true);
			if (gt == com.epl.geometry.Geometry.Type.MultiPoint || gt == com.epl.geometry.Geometry.Type.Polyline || gt == com.epl.geometry.Geometry.Type.Polygon)
			{
				knownSimpleResult = helper.IsSimplePlanarImpl_();
			}
			else
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// what else?
			if (result != null)
			{
				result.Assign(helper.m_nonSimpleResult);
			}
			return knownSimpleResult;
		}

		/// <summary>
		/// Simplifies geometries for storing in DB:
		/// MultiPoint:check that no points coincide.tolerance is ignored.
		/// </summary>
		/// <remarks>
		/// Simplifies geometries for storing in DB:
		/// MultiPoint:check that no points coincide.tolerance is ignored.
		/// Polyline:ensure there no segments degenerate segments. Polygon:cracks and
		/// clusters using cluster tolerance and resolves all self intersections,
		/// orients rings properly and arranges the rings in the OGC order.
		/// Returns simplified geometry.
		/// </remarks>
		protected internal static com.epl.geometry.Geometry SimplifyAsFeature(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, bool bForce, com.epl.geometry.ProgressTracker progressTracker)
		{
			/* const */
			/* const */
			if (geometry.IsEmpty())
			{
				return geometry;
			}
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				return geometry;
			}
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false);
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				com.epl.geometry.Envelope env = (com.epl.geometry.Envelope)geometry;
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				env.QueryEnvelope2D(env2D);
				if (env2D.IsDegenerate(tolerance))
				{
					return (com.epl.geometry.Geometry)(env.CreateInstance());
				}
				// return empty
				// geometry
				return geometry;
			}
			else
			{
				if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
				{
					com.epl.geometry.Segment seg = (com.epl.geometry.Segment)geometry;
					com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(seg.GetDescription());
					polyline.AddSegment(seg, true);
					return SimplifyAsFeature(polyline, spatialReference, bForce, progressTracker);
				}
			}
			double geomTolerance = 0;
			int isSimple = ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl()).GetIsSimple(tolerance);
			int knownSimpleResult = bForce ? com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Unknown : isSimple;
			// TODO: need to distinguish KnownSimple between SimpleAsFeature and
			// SimplePlanar.
			// From the first sight it seems the SimplePlanar implies
			// SimpleAsFeature.
			if (knownSimpleResult == com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong)
			{
				if (gt == com.epl.geometry.Geometry.Type.Polygon && ((com.epl.geometry.Polygon)geometry).GetFillRule() != com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven)
				{
					com.epl.geometry.Geometry res = geometry.Copy();
					((com.epl.geometry.Polygon)res).SetFillRule(com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven);
					//standardize on odd_even fill rule
					return res;
				}
				return geometry;
			}
			com.epl.geometry.OperatorSimplifyLocalHelper helper = new com.epl.geometry.OperatorSimplifyLocalHelper(geometry, spatialReference, knownSimpleResult, progressTracker, false);
			com.epl.geometry.Geometry result;
			if (gt == com.epl.geometry.Geometry.Type.MultiPoint)
			{
				result = (com.epl.geometry.Geometry)(helper.MultiPointSimplifyAsFeature_());
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.Type.Polyline)
				{
					result = (com.epl.geometry.Geometry)(helper.PolylineSimplifyAsFeature_());
				}
				else
				{
					if (gt == com.epl.geometry.Geometry.Type.Polygon)
					{
						result = (com.epl.geometry.Geometry)(helper.PolygonSimplifyAsFeature_());
					}
					else
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
				}
			}
			// what else?
			return result;
		}

		/// <summary>
		/// Simplifies geometries for storing in OGC format:
		/// MultiPoint:check that no points coincide.tolerance is ignored.
		/// </summary>
		/// <remarks>
		/// Simplifies geometries for storing in OGC format:
		/// MultiPoint:check that no points coincide.tolerance is ignored.
		/// Polyline:ensure there no segments degenerate segments. Polygon:cracks and
		/// clusters using cluster tolerance and resolves all self intersections,
		/// orients rings properly and arranges the rings in the OGC order.
		/// Returns simplified geometry.
		/// </remarks>
		internal static com.epl.geometry.Geometry SimplifyOGC(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, bool bForce, com.epl.geometry.ProgressTracker progressTracker)
		{
			/* const */
			/* const */
			if (geometry.IsEmpty())
			{
				return geometry;
			}
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				return geometry;
			}
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false);
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				com.epl.geometry.Envelope env = (com.epl.geometry.Envelope)geometry;
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				env.QueryEnvelope2D(env2D);
				if (env2D.IsDegenerate(tolerance))
				{
					return (com.epl.geometry.Geometry)(env.CreateInstance());
				}
				// return empty
				// geometry
				return geometry;
			}
			else
			{
				if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
				{
					com.epl.geometry.Segment seg = (com.epl.geometry.Segment)geometry;
					com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(seg.GetDescription());
					polyline.AddSegment(seg, true);
					return SimplifyOGC(polyline, spatialReference, bForce, progressTracker);
				}
			}
			if (!com.epl.geometry.Geometry.IsMultiVertex(gt.Value()))
			{
				throw new com.epl.geometry.GeometryException("OGC simplify is not implemented for this geometry type" + gt);
			}
			com.epl.geometry.MultiVertexGeometry result = com.epl.geometry.TopologicalOperations.SimplifyOGC((com.epl.geometry.MultiVertexGeometry)geometry, tolerance, false, progressTracker);
			return result;
		}

		private int CompareVertices_(int i1, int i2, bool get_paths)
		{
			if (i1 == i2)
			{
				return 0;
			}
			int pair1 = m_pairs.Get(i1);
			int pair2 = m_pairs.Get(i2);
			int xy1 = pair1 >> 1;
			int xy2 = pair2 >> 1;
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			m_xy.Read(2 * xy1, pt1);
			pt1.y += (((pair1 & 1) != 0) ? m_toleranceIsSimple : -m_toleranceIsSimple);
			m_xy.Read(2 * xy2, pt2);
			pt2.y += (((pair2 & 1) != 0) ? m_toleranceIsSimple : -m_toleranceIsSimple);
			int res = pt1.Compare(pt2);
			if (res == 0 && get_paths)
			{
				int di = m_paths_for_OGC_tests.Get(xy1) - m_paths_for_OGC_tests.Get(xy2);
				return di < 0 ? -1 : di > 0 ? 1 : 0;
			}
			return res;
		}

		private sealed class VertexComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.OperatorSimplifyLocalHelper parent;

			internal bool get_paths;

			internal VertexComparer(com.epl.geometry.OperatorSimplifyLocalHelper parent_, bool get_paths_)
			{
				parent = parent_;
				get_paths = get_paths_;
			}

			public override int Compare(int i1, int i2)
			{
				return parent.CompareVertices_(i1, i2, get_paths);
			}
		}

		private sealed class IndexSorter : com.epl.geometry.ClassicSort
		{
			internal com.epl.geometry.OperatorSimplifyLocalHelper parent;

			private bool get_paths;

			private com.epl.geometry.Point2D pt1_dummy = new com.epl.geometry.Point2D();

			internal IndexSorter(com.epl.geometry.OperatorSimplifyLocalHelper parent_, bool get_paths_)
			{
				parent = parent_;
				get_paths = get_paths_;
			}

			public override void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices)
			{
				indices.Sort(begin, end, new com.epl.geometry.OperatorSimplifyLocalHelper.VertexComparer(parent, get_paths));
			}

			public override double GetValue(int index)
			{
				/* const */
				int pair = parent.m_pairs.Get(index);
				int xy1 = pair >> 1;
				parent.m_xy.Read(2 * xy1, pt1_dummy);
				double y = pt1_dummy.y + (((pair & 1) != 0) ? parent.m_toleranceIsSimple : -parent.m_toleranceIsSimple);
				return y;
			}
		}

		private int CompareVerticesMultiPoint_(int i1, int i2)
		{
			if (i1 == i2)
			{
				return 0;
			}
			com.epl.geometry.MultiVertexGeometryImpl multiVertexImpl = (com.epl.geometry.MultiVertexGeometryImpl)m_geometry._getImpl();
			com.epl.geometry.Point2D pt1 = multiVertexImpl.GetXY(i1);
			com.epl.geometry.Point2D pt2 = multiVertexImpl.GetXY(i2);
			if (pt1.x < pt2.x)
			{
				return -1;
			}
			if (pt1.x > pt2.x)
			{
				return 1;
			}
			if (pt1.y < pt2.y)
			{
				return -1;
			}
			if (pt1.y > pt2.y)
			{
				return 1;
			}
			for (int attrib = 1; attrib < m_attributeCount; attrib++)
			{
				int semantics = m_description.GetSemantics(attrib);
				int nords = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int ord = 0; ord < nords; ord++)
				{
					double v1 = multiVertexImpl.GetAttributeAsDbl(semantics, i1, ord);
					double v2 = multiVertexImpl.GetAttributeAsDbl(semantics, i2, ord);
					if (v1 < v2)
					{
						return -1;
					}
					if (v1 > v2)
					{
						return 1;
					}
				}
			}
			return 0;
		}

		private int CompareVerticesMultiPoint2_(int i1, int i2)
		{
			int res = CompareVerticesMultiPoint_(i1, i2);
			if (res == 0)
			{
				return i1 < i2 ? -1 : 1;
			}
			else
			{
				return res;
			}
		}

		private sealed class EdgeComparerForSelfIntersection : System.Collections.Generic.IComparer<com.epl.geometry.OperatorSimplifyLocalHelper.Edge>
		{
			internal com.epl.geometry.OperatorSimplifyLocalHelper parent;

			internal EdgeComparerForSelfIntersection(com.epl.geometry.OperatorSimplifyLocalHelper parent_)
			{
				parent = parent_;
			}

			// Recall that the total ordering [<] induced by compare satisfies e1
			// [<] e2 if and only if compare(e1, e2) < 0.
			public int Compare(com.epl.geometry.OperatorSimplifyLocalHelper.Edge e1, com.epl.geometry.OperatorSimplifyLocalHelper.Edge e2)
			{
				return parent.EdgeAngleCompare_(e1, e2);
			}
		}

		private sealed class MultiPointVertexComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.OperatorSimplifyLocalHelper parent;

			internal MultiPointVertexComparer(com.epl.geometry.OperatorSimplifyLocalHelper parent_)
			{
				parent = parent_;
			}

			public override int Compare(int i1, int i2)
			{
				return parent.CompareVerticesMultiPoint_(i1, i2);
			}
		}

		private sealed class MultiPointVertexComparer2 : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.OperatorSimplifyLocalHelper parent;

			internal MultiPointVertexComparer2(com.epl.geometry.OperatorSimplifyLocalHelper parent_)
			{
				parent = parent_;
			}

			public override int Compare(int i1, int i2)
			{
				return parent.CompareVerticesMultiPoint2_(i1, i2);
			}
		}

		// compares angles between two edges
		private int EdgeAngleCompare_(com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge1, com.epl.geometry.OperatorSimplifyLocalHelper.Edge edge2)
		{
			/* const */
			/* const */
			if (edge1.Equals(edge2))
			{
				return 0;
			}
			com.epl.geometry.Point2D v1 = edge1.m_segment._getTangent(edge1.GetReversed() ? 1.0 : 0.0);
			if (edge1.GetReversed())
			{
				v1.Negate();
			}
			com.epl.geometry.Point2D v2 = edge2.m_segment._getTangent(edge2.GetReversed() ? 1.0 : 0.0);
			if (edge2.GetReversed())
			{
				v2.Negate();
			}
			int q1 = v1._getQuarter();
			int q2 = v2._getQuarter();
			if (q2 == q1)
			{
				double cross = v1.CrossProduct(v2);
				double crossError = 4 * com.epl.geometry.NumberUtils.DoubleEps() * (System.Math.Abs(v2.x * v1.y) + System.Math.Abs(v2.y * v1.x));
				if (System.Math.Abs(cross) <= crossError)
				{
					cross--;
					// To avoid warning of "this line has no effect" from
					// cross = cross.
					cross++;
				}
				System.Diagnostics.Debug.Assert((System.Math.Abs(cross) > crossError));
				return cross < 0 ? 1 : (cross > 0 ? -1 : 0);
			}
			else
			{
				return q1 < q2 ? -1 : 1;
			}
		}
	}
}
