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
	/// <summary>
	/// A helper geometry structure that can store MultiPoint, Polyline, Polygon
	/// geometries in linked lists.
	/// </summary>
	/// <remarks>
	/// A helper geometry structure that can store MultiPoint, Polyline, Polygon
	/// geometries in linked lists. It allows constant time manipulation of geometry
	/// vertices.
	/// </remarks>
	internal sealed class EditShape
	{
		internal abstract class PathFlags_
		{
			public const int closedPath = 1;

			public const int exteriorPath = 2;

			public const int ringAreaValid = 4;
		}

		internal static class PathFlags_Constants
		{
		}

		private int m_geometryCount;

		private int m_path_count;

		private int m_point_count;

		private int m_first_geometry;

		private int m_last_geometry;

		private com.epl.geometry.StridedIndexTypeCollection m_vertex_index_list;

		private com.epl.geometry.MultiPoint m_vertices_mp;

		private com.epl.geometry.MultiPointImpl m_vertices;

		internal com.epl.geometry.AttributeStreamOfDbl m_xy_stream;

		internal com.epl.geometry.VertexDescription m_vertex_description;

		internal bool m_b_has_attributes;

		internal System.Collections.Generic.List<com.epl.geometry.Segment> m_segments;

		internal com.epl.geometry.AttributeStreamOfDbl m_weights;

		internal System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32> m_indices;

		internal com.epl.geometry.StridedIndexTypeCollection m_path_index_list;

		internal com.epl.geometry.AttributeStreamOfDbl m_path_areas;

		internal com.epl.geometry.AttributeStreamOfDbl m_path_lengths;

		internal System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32> m_pathindices;

		internal com.epl.geometry.StridedIndexTypeCollection m_geometry_index_list;

		internal System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32> m_geometry_indices;

		internal class EditShapeBucketSortHelper : com.epl.geometry.ClassicSort
		{
			internal com.epl.geometry.EditShape m_shape;

			internal EditShapeBucketSortHelper(com.epl.geometry.EditShape shape)
			{
				// ****************Vertex Data******************
				// vertex coordinates are stored here
				// Attribute_stream_of_index_type::SPtr m_indexRemap;
				// Internals of m_vertices_mp
				// The xy stream of the m_vertices.
				// a shortcut to the vertex
				// description.
				// a short cut to know if we have something in
				// addition to x and y.
				// may be NULL if all segments a Lines,
				// otherwise contains NULLs for Line
				// segments. Curves are not NULL.
				// may be NULL if no weights are provided.
				// NULL weights assumes weight value of 1.
				// user indices are here
				// ****************End Vertex Data**************
				// doubly connected list. Path
				// index into the Path Data
				// arrays, Prev path, next
				// path.
				// ******************Path Data******************
				// Block_array<Envelope::SPtr>::SPtr m_path_envelopes;
				// path user indices are
				// here
				// *****************End Path Data***************
				// geometry user
				// indices are here
				// *********** Helpers for Bucket sort**************
				m_shape = shape;
			}

			public override void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices)
			{
				m_shape.SortVerticesSimpleByYHelper_(indices, begin, end);
			}

			public override double GetValue(int index)
			{
				return m_shape.GetY(index);
			}
		}

		internal com.epl.geometry.BucketSort m_bucket_sort;

		internal com.epl.geometry.Point m_helper_point;

		// Envelope::SPtr m_envelope; //the BBOX for all attributes
		// a helper point for intermediate operations
		internal com.epl.geometry.Segment GetSegmentFromIndex_(int vindex)
		{
			return m_segments != null ? m_segments[vindex] : null;
		}

		internal void SetSegmentToIndex_(int vindex, com.epl.geometry.Segment seg)
		{
			if (m_segments == null)
			{
				if (seg == null)
				{
					return;
				}
				m_segments = new System.Collections.Generic.List<com.epl.geometry.Segment>();
				for (int i = 0, n = m_vertices.GetPointCount(); i < n; i++)
				{
					m_segments.Add(null);
				}
			}
			m_segments[vindex] = seg;
		}

		internal void SetPrevPath_(int path, int prev)
		{
			m_path_index_list.SetField(path, 1, prev);
		}

		internal void SetNextPath_(int path, int next)
		{
			m_path_index_list.SetField(path, 2, next);
		}

		internal void SetPathFlags_(int path, int flags)
		{
			m_path_index_list.SetField(path, 6, flags);
		}

		internal int GetPathFlags_(int path)
		{
			return m_path_index_list.GetField(path, 6);
		}

		internal void SetPathGeometry_(int path, int geom)
		{
			m_path_index_list.SetField(path, 7, geom);
		}

		internal int GetPathIndex_(int path)
		{
			return m_path_index_list.GetField(path, 0);
		}

		internal void SetNextGeometry_(int geom, int next)
		{
			m_geometry_index_list.SetField(geom, 1, next);
		}

		internal void SetPrevGeometry_(int geom, int prev)
		{
			m_geometry_index_list.SetField(geom, 0, prev);
		}

		internal int GetGeometryIndex_(int geom)
		{
			return m_geometry_index_list.GetField(geom, 7);
		}

		internal int GetFirstPath_(int geom)
		{
			return m_geometry_index_list.GetField(geom, 3);
		}

		internal void SetFirstPath_(int geom, int firstPath)
		{
			m_geometry_index_list.SetField(geom, 3, firstPath);
		}

		internal void SetLastPath_(int geom, int path)
		{
			m_geometry_index_list.SetField(geom, 4, path);
		}

		internal int NewGeometry_(int gt)
		{
			// Index_type index = m_first_free_geometry;
			if (m_geometry_index_list == null)
			{
				m_geometry_index_list = new com.epl.geometry.StridedIndexTypeCollection(8);
			}
			int index = m_geometry_index_list.NewElement();
			// m_geometry_index_list.set(index + 0, -1);//prev
			// m_geometry_index_list.set(index + 1, -1);//next
			m_geometry_index_list.SetField(index, 2, gt);
			// Geometry_type
			// m_geometry_index_list.set(index + 3, -1);//first path
			// m_geometry_index_list.set(index + 4, -1);//last path
			m_geometry_index_list.SetField(index, 5, 0);
			// point count
			m_geometry_index_list.SetField(index, 6, 0);
			// path count
			m_geometry_index_list.SetField(index, 7, m_geometry_index_list.ElementToIndex(index));
			// geometry index
			return index;
		}

		internal void FreeGeometry_(int geom)
		{
			m_geometry_index_list.DeleteElement(geom);
		}

		internal int NewPath_(int geom)
		{
			if (m_path_index_list == null)
			{
				m_path_index_list = new com.epl.geometry.StridedIndexTypeCollection(8);
				m_vertex_index_list = new com.epl.geometry.StridedIndexTypeCollection(5);
				m_path_areas = new com.epl.geometry.AttributeStreamOfDbl(0);
				m_path_lengths = new com.epl.geometry.AttributeStreamOfDbl(0);
			}
			int index = m_path_index_list.NewElement();
			int pindex = m_path_index_list.ElementToIndex(index);
			m_path_index_list.SetField(index, 0, pindex);
			// size
			// m_path_index_list.set(index + 1, -1);//prev
			// m_path_index_list.set(index + 2, -1);//next
			m_path_index_list.SetField(index, 3, 0);
			// size
			// m_path_index_list.set(index + 4, -1);//first vertex handle
			// m_path_index_list.set(index + 5, -1);//last vertex handle
			m_path_index_list.SetField(index, 6, 0);
			// path flags
			SetPathGeometry_(index, geom);
			if (pindex >= m_path_areas.Size())
			{
				int sz = pindex < 16 ? 16 : (pindex * 3) / 2;
				m_path_areas.Resize(sz);
				m_path_lengths.Resize(sz);
			}
			// if (m_path_envelopes)
			// m_path_envelopes.resize(sz);
			m_path_areas.Set(pindex, 0);
			m_path_lengths.Set(pindex, 0);
			// if (m_path_envelopes)
			// m_path_envelopes.set(pindex, nullptr);
			m_path_count++;
			return index;
		}

		internal void FreePath_(int path)
		{
			m_path_index_list.DeleteElement(path);
			m_path_count--;
		}

		internal void FreeVertex_(int vertex)
		{
			m_vertex_index_list.DeleteElement(vertex);
			m_point_count--;
		}

		internal int NewVertex_(int vindex)
		{
			System.Diagnostics.Debug.Assert((vindex >= 0 || vindex == -1));
			// vindex is not a handle
			if (m_path_index_list == null)
			{
				m_path_index_list = new com.epl.geometry.StridedIndexTypeCollection(8);
				m_vertex_index_list = new com.epl.geometry.StridedIndexTypeCollection(5);
				m_path_areas = new com.epl.geometry.AttributeStreamOfDbl(0);
				m_path_lengths = new com.epl.geometry.AttributeStreamOfDbl(0);
			}
			int index = m_vertex_index_list.NewElement();
			int vi = vindex >= 0 ? vindex : m_vertex_index_list.ElementToIndex(index);
			m_vertex_index_list.SetField(index, 0, vi);
			if (vindex < 0)
			{
				if (vi >= m_vertices.GetPointCount())
				{
					int sz = vi < 16 ? 16 : (vi * 3) / 2;
					// m_vertices.reserveRounded(sz);
					m_vertices.Resize(sz);
					if (m_segments != null)
					{
						for (int i = 0; i < sz; i++)
						{
							m_segments.Add(null);
						}
					}
					if (m_weights != null)
					{
						m_weights.Resize(sz);
					}
					m_xy_stream = (com.epl.geometry.AttributeStreamOfDbl)m_vertices.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				}
				m_vertices.SetXY(vi, -1e38, -1e38);
				if (m_segments != null)
				{
					m_segments[vi] = null;
				}
				if (m_weights != null)
				{
					m_weights.Write(vi, 1.0);
				}
			}
			// We do not set vertices or segments here, because we assume those
			// are set correctly already.
			// We only here to create linked list of indices on existing vertex
			// value.
			// m_segments->set(m_point_count, nullptr);
			m_vertex_index_list.SetField(index, 4, vi * 2);
			m_point_count++;
			return index;
		}

		internal void Free_vertex_(int vertex)
		{
			m_vertex_index_list.DeleteElement(vertex);
			m_point_count--;
		}

		internal int InsertVertex_(int path, int before, com.epl.geometry.Point point)
		{
			int prev = before != -1 ? GetPrevVertex(before) : GetLastVertex(path);
			int next = prev != -1 ? GetNextVertex(prev) : -1;
			int vertex = NewVertex_(point == null ? m_point_count : -1);
			int vindex = GetVertexIndex(vertex);
			if (point != null)
			{
				m_vertices.SetPointByVal(vindex, point);
			}
			SetPathToVertex_(vertex, path);
			SetNextVertex_(vertex, next);
			SetPrevVertex_(vertex, prev);
			if (next != -1)
			{
				SetPrevVertex_(next, vertex);
			}
			if (prev != -1)
			{
				SetNextVertex_(prev, vertex);
			}
			bool b_closed = IsClosedPath(path);
			int first = GetFirstVertex(path);
			if (before == -1)
			{
				SetLastVertex_(path, vertex);
			}
			if (before == first)
			{
				SetFirstVertex_(path, vertex);
			}
			if (b_closed && next == -1)
			{
				SetNextVertex_(vertex, vertex);
				SetPrevVertex_(vertex, vertex);
			}
			SetPathSize_(path, GetPathSize(path) + 1);
			int geometry = GetGeometryFromPath(path);
			SetGeometryVertexCount_(geometry, GetPointCount(geometry) + 1);
			return vertex;
		}

		internal com.epl.geometry.Point GetHelperPoint_()
		{
			if (m_helper_point == null)
			{
				m_helper_point = new com.epl.geometry.Point(m_vertices.GetDescription());
			}
			return m_helper_point;
		}

		internal void SetFillRule(int geom, int rule)
		{
			int t = m_geometry_index_list.GetField(geom, 2);
			t &= ~(unchecked((int)(0x8000000)));
			t |= rule == com.epl.geometry.Polygon.FillRule.enumFillRuleWinding ? unchecked((int)(0x8000000)) : 0;
			m_geometry_index_list.SetField(geom, 2, t);
		}

		//fill rule combined with geometry type
		internal int GetFillRule(int geom)
		{
			int t = m_geometry_index_list.GetField(geom, 2);
			return (t & unchecked((int)(0x8000000))) != 0 ? com.epl.geometry.Polygon.FillRule.enumFillRuleWinding : com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven;
		}

		internal int AddMultiPath_(com.epl.geometry.MultiPath multi_path)
		{
			int newgeom = CreateGeometry(multi_path.GetType(), multi_path.GetDescription());
			if (multi_path.GetType() == com.epl.geometry.Geometry.Type.Polygon)
			{
				SetFillRule(newgeom, ((com.epl.geometry.Polygon)multi_path).GetFillRule());
			}
			AppendMultiPath_(newgeom, multi_path);
			return newgeom;
		}

		internal int AddMultiPoint_(com.epl.geometry.MultiPoint multi_point)
		{
			int newgeometry = CreateGeometry(multi_point.GetType(), multi_point.GetDescription());
			AppendMultiPoint_(newgeometry, multi_point);
			return newgeometry;
		}

		internal void AppendMultiPath_(int dstGeom, com.epl.geometry.MultiPath multi_path)
		{
			com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)multi_path._getImpl();
			// m_vertices->reserve_rounded(m_vertices->get_point_count() +
			// mp_impl->get_point_count());//ensure reallocation happens by blocks
			// so that already allocated vertices do not get reallocated.
			m_vertices_mp.Add(multi_path, 0, mp_impl.GetPointCount());
			m_xy_stream = (com.epl.geometry.AttributeStreamOfDbl)m_vertices.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			bool b_some_segments = m_segments != null && mp_impl.GetSegmentFlagsStreamRef() != null;
			for (int ipath = 0, npath = mp_impl.GetPathCount(); ipath < npath; ipath++)
			{
				if (mp_impl.GetPathSize(ipath) < 2)
				{
					// CR249862 - Clipping geometry
					// which has empty part produces
					// a crash
					continue;
				}
				int path = InsertPath(dstGeom, -1);
				SetClosedPath(path, mp_impl.IsClosedPath(ipath));
				for (int ivertex = mp_impl.GetPathStart(ipath), iend = mp_impl.GetPathEnd(ipath); ivertex < iend; ivertex++)
				{
					int vertex = InsertVertex_(path, -1, null);
					if (b_some_segments)
					{
						int vindex = GetVertexIndex(vertex);
						if ((mp_impl.GetSegmentFlags(ivertex) & unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg)) != 0)
						{
							SetSegmentToIndex_(vindex, null);
						}
						else
						{
							com.epl.geometry.SegmentBuffer seg_buffer = new com.epl.geometry.SegmentBuffer();
							mp_impl.GetSegment(ivertex, seg_buffer, true);
							SetSegmentToIndex_(vindex, seg_buffer.Get());
						}
					}
				}
			}
		}

		// {//debug
		// #ifdef DEBUG
		// for (Index_type geometry = get_first_geometry(); geometry != -1;
		// geometry = get_next_geometry(geometry))
		// {
		// for (Index_type path = get_first_path(geometry); path != -1; path =
		// get_next_path(path))
		// {
		// Index_type first = get_first_vertex(path);
		// Index_type v = first;
		// for (get_next_vertex(v); v != first; v = get_next_vertex(v))
		// {
		// assert(get_next_vertex(get_prev_vertex(v)) == v);
		// }
		// }
		// }
		// #endif
		// }
		internal void AppendMultiPoint_(int dstGeom, com.epl.geometry.MultiPoint multi_point)
		{
			// m_vertices->reserve_rounded(m_vertices->get_point_count() +
			// multi_point.get_point_count());//ensure reallocation happens by
			// blocks so that already allocated vertices do not get reallocated.
			m_vertices_mp.Add(multi_point, 0, multi_point.GetPointCount());
			m_xy_stream = (com.epl.geometry.AttributeStreamOfDbl)m_vertices.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			int path = InsertPath(dstGeom, -1);
			for (int ivertex = 0, iend = multi_point.GetPointCount(); ivertex < iend; ivertex++)
			{
				InsertVertex_(path, -1, null);
			}
		}

		internal void SplitSegmentForward_(int origin_vertex, com.epl.geometry.SegmentIntersector intersector, int intersector_index)
		{
			int last_vertex = GetNextVertex(origin_vertex);
			if (last_vertex == -1)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			com.epl.geometry.Point point = GetHelperPoint_();
			int path = GetPathFromVertex(origin_vertex);
			int vertex = origin_vertex;
			for (int i = 0, n = intersector.GetResultSegmentCount(intersector_index); i < n; i++)
			{
				int vindex = GetVertexIndex(vertex);
				int next_vertex = GetNextVertex(vertex);
				com.epl.geometry.Segment seg = intersector.GetResultSegment(intersector_index, i);
				if (i == 0)
				{
					seg.QueryStart(point);
					// #ifdef DEBUG
					// Point2D pt = new Point2D();
					// getXY(vertex, pt);
					// assert(Point2D.distance(point.getXY(), pt) <=
					// intersector.get_tolerance_());
					// #endif
					SetPoint(vertex, point);
				}
				if (seg.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Line)
				{
					SetSegmentToIndex_(vindex, null);
				}
				else
				{
					SetSegmentToIndex_(vindex, (com.epl.geometry.Segment)com.epl.geometry.Geometry._clone(seg));
				}
				seg.QueryEnd(point);
				if (i < n - 1)
				{
					int inserted_vertex = InsertVertex_(path, next_vertex, point);
					vertex = inserted_vertex;
				}
				else
				{
					// #ifdef DEBUG
					// Point_2D pt;
					// get_xy(last_vertex, pt);
					// assert(Point_2D::distance(point->get_xy(), pt) <=
					// intersector.getTolerance_());
					// #endif
					SetPoint(last_vertex, point);
					System.Diagnostics.Debug.Assert((last_vertex == next_vertex));
				}
			}
		}

		internal void SplitSegmentBackward_(int origin_vertex, com.epl.geometry.SegmentIntersector intersector, int intersector_index)
		{
			int last_vertex = GetNextVertex(origin_vertex);
			if (last_vertex == -1)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			com.epl.geometry.Point point = GetHelperPoint_();
			int path = GetPathFromVertex(origin_vertex);
			int vertex = origin_vertex;
			for (int i = 0, n = intersector.GetResultSegmentCount(intersector_index); i < n; i++)
			{
				int vindex = GetVertexIndex(vertex);
				int next_vertex = GetNextVertex(vertex);
				com.epl.geometry.Segment seg = intersector.GetResultSegment(intersector_index, n - i - 1);
				if (i == 0)
				{
					seg.QueryEnd(point);
					// #ifdef DEBUG
					// Point2D pt = new Point2D();
					// getXY(vertex, pt);
					// assert(Point2D.distance(point.getXY(), pt) <=
					// intersector.getTolerance_());
					// #endif
					SetPoint(vertex, point);
				}
				if (seg.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Line)
				{
					SetSegmentToIndex_(vindex, null);
				}
				else
				{
					SetSegmentToIndex_(vindex, (com.epl.geometry.Segment)com.epl.geometry.Geometry._clone(seg));
				}
				seg.QueryStart(point);
				if (i < n - 1)
				{
					int inserted_vertex = InsertVertex_(path, next_vertex, point);
					vertex = inserted_vertex;
				}
				else
				{
					// #ifdef DEBUG
					// Point2D pt = new Point2D();
					// getXY(last_vertex, pt);
					// assert(Point2D.distance(point.getXY(), pt) <=
					// intersector.getTolerance_());
					// #endif
					SetPoint(last_vertex, point);
					System.Diagnostics.Debug.Assert((last_vertex == next_vertex));
				}
			}
		}

		internal EditShape()
		{
			m_path_count = 0;
			m_first_geometry = -1;
			m_last_geometry = -1;
			m_point_count = 0;
			m_geometryCount = 0;
			m_b_has_attributes = false;
			m_vertices = null;
			m_xy_stream = null;
			m_vertex_description = null;
		}

		// Total point count in all geometries
		internal int GetTotalPointCount()
		{
			return m_point_count;
		}

		// Returns envelope of all coordinates.
		internal com.epl.geometry.Envelope2D GetEnvelope2D()
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetEmpty();
			com.epl.geometry.EditShape.VertexIterator vert_iter = QueryVertexIterator();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			bool b_first = true;
			for (int ivertex = vert_iter.Next(); ivertex != -1; ivertex = vert_iter.Next())
			{
				GetXY(ivertex, pt);
				if (b_first)
				{
					env.Merge(pt.x, pt.y);
				}
				else
				{
					env.MergeNE(pt.x, pt.y);
				}
				b_first = false;
			}
			return env;
		}

		// Returns geometry count in the edit shape
		internal int GetGeometryCount()
		{
			return m_geometryCount;
		}

		// Adds a Geometry to the Edit_shape
		internal int AddGeometry(com.epl.geometry.Geometry geometry)
		{
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (com.epl.geometry.Geometry.IsMultiPath(gt.Value()))
			{
				return AddMultiPath_((com.epl.geometry.MultiPath)geometry);
			}
			if (gt == com.epl.geometry.Geometry.Type.MultiPoint)
			{
				return AddMultiPoint_((com.epl.geometry.MultiPoint)geometry);
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		// Append a Geometry to the given geometry of the Edit_shape
		internal void AppendGeometry(int dstGeometry, com.epl.geometry.Geometry srcGeometry)
		{
			com.epl.geometry.Geometry.Type gt = srcGeometry.GetType();
			if (com.epl.geometry.Geometry.IsMultiPath(gt.Value()))
			{
				AppendMultiPath_(dstGeometry, (com.epl.geometry.MultiPath)srcGeometry);
				return;
			}
			else
			{
				if (gt.Value() == com.epl.geometry.Geometry.GeometryType.MultiPoint)
				{
					AppendMultiPoint_(dstGeometry, (com.epl.geometry.MultiPoint)srcGeometry);
					return;
				}
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		// Adds a path
		internal int AddPathFromMultiPath(com.epl.geometry.MultiPath multi_path, int ipath, bool as_polygon)
		{
			int newgeom = CreateGeometry(as_polygon ? com.epl.geometry.Geometry.Type.Polygon : com.epl.geometry.Geometry.Type.Polyline, multi_path.GetDescription());
			com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)multi_path._getImpl();
			if (multi_path.GetPathSize(ipath) < 2)
			{
				return newgeom;
			}
			//return empty geometry
			// m_vertices->reserve_rounded(m_vertices->get_point_count() +
			// multi_path.get_path_size(ipath));//ensure reallocation happens by
			// blocks so that already allocated vertices do not get reallocated.
			m_vertices_mp.Add(multi_path, multi_path.GetPathStart(ipath), mp_impl.GetPathEnd(ipath));
			m_xy_stream = (com.epl.geometry.AttributeStreamOfDbl)m_vertices.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			int path = InsertPath(newgeom, -1);
			SetClosedPath(path, mp_impl.IsClosedPath(ipath) || as_polygon);
			bool b_some_segments = m_segments != null && mp_impl.GetSegmentFlagsStreamRef() != null;
			for (int ivertex = mp_impl.GetPathStart(ipath), iend = mp_impl.GetPathEnd(ipath); ivertex < iend; ivertex++)
			{
				int vertex = InsertVertex_(path, -1, null);
				if (b_some_segments)
				{
					int vindex = GetVertexIndex(vertex);
					if ((mp_impl.GetSegmentFlags(ivertex) & com.epl.geometry.SegmentFlags.enumLineSeg) != 0)
					{
						SetSegmentToIndex_(vindex, null);
					}
					else
					{
						com.epl.geometry.SegmentBuffer seg_buffer = new com.epl.geometry.SegmentBuffer();
						mp_impl.GetSegment(ivertex, seg_buffer, true);
						SetSegmentToIndex_(vindex, seg_buffer.Get());
					}
				}
			}
			return newgeom;
		}

		// Extracts a geometry from the Edit_shape. The method creates a new
		// Geometry instance and initializes it with the Edit_shape data for the
		// given geometry.
		internal com.epl.geometry.Geometry GetGeometry(int geometry)
		{
			int gt = GetGeometryType(geometry);
			com.epl.geometry.Geometry geom = com.epl.geometry.InternalUtils.CreateGeometry(gt, m_vertices_mp.GetDescription());
			int point_count = GetPointCount(geometry);
			if (point_count == 0)
			{
				return geom;
			}
			if (com.epl.geometry.Geometry.IsMultiPath(gt))
			{
				com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)geom._getImpl();
				int path_count = GetPathCount(geometry);
				com.epl.geometry.AttributeStreamOfInt32 parts = (com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(path_count + 1));
				com.epl.geometry.AttributeStreamOfInt8 pathFlags = (com.epl.geometry.AttributeStreamOfInt8)(com.epl.geometry.AttributeStreamBase.CreateByteStream(path_count + 1, unchecked((byte)0)));
				com.epl.geometry.VertexDescription description = geom.GetDescription();
				for (int iattrib = 0, nattrib = description.GetAttributeCount(); iattrib < nattrib; iattrib++)
				{
					int semantics = description.GetSemantics(iattrib);
					int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					com.epl.geometry.AttributeStreamBase dst_stream = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(semantics, point_count);
					com.epl.geometry.AttributeStreamBase src_stream = m_vertices.GetAttributeStreamRef(semantics);
					int dst_index = 0;
					int ipath = 0;
					int nvert = 0;
					for (int path = GetFirstPath(geometry); path != -1; path = GetNextPath(path))
					{
						byte flag_mask = 0;
						if (IsClosedPath(path))
						{
							flag_mask |= unchecked((byte)com.epl.geometry.PathFlags.enumClosed);
						}
						else
						{
							System.Diagnostics.Debug.Assert((gt != com.epl.geometry.Geometry.GeometryType.Polygon));
						}
						if (IsExterior(path))
						{
							flag_mask |= unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon);
						}
						if (flag_mask != 0)
						{
							pathFlags.SetBits(ipath, flag_mask);
						}
						int path_size = GetPathSize(path);
						parts.Write(ipath++, nvert);
						nvert += path_size;
						if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
						{
							com.epl.geometry.AttributeStreamOfDbl src_stream_dbl = (com.epl.geometry.AttributeStreamOfDbl)(src_stream);
							com.epl.geometry.AttributeStreamOfDbl dst_stream_dbl = (com.epl.geometry.AttributeStreamOfDbl)(dst_stream);
							com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
							for (int vertex = GetFirstVertex(path); dst_index < nvert; vertex = GetNextVertex(vertex), dst_index++)
							{
								int src_index = GetVertexIndex(vertex);
								src_stream_dbl.Read(src_index * 2, pt);
								dst_stream_dbl.Write(dst_index * 2, pt);
							}
						}
						else
						{
							for (int vertex = GetFirstVertex(path); dst_index < nvert; vertex = GetNextVertex(vertex), dst_index++)
							{
								int src_index = GetVertexIndex(vertex);
								for (int icomp = 0; icomp < ncomps; icomp++)
								{
									double d = src_stream.ReadAsDbl(src_index * ncomps + icomp);
									dst_stream.WriteAsDbl(dst_index * ncomps + icomp, d);
								}
							}
						}
					}
					System.Diagnostics.Debug.Assert((nvert == point_count));
					// Inconsistent content in the
					// Edit_shape. Please, fix.
					System.Diagnostics.Debug.Assert((ipath == path_count));
					mp_impl.SetAttributeStreamRef(semantics, dst_stream);
					parts.Write(path_count, point_count);
				}
				mp_impl.SetPathFlagsStreamRef(pathFlags);
				mp_impl.SetPathStreamRef(parts);
				mp_impl.NotifyModified(com.epl.geometry.DirtyFlags.dirtyAll);
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.GeometryType.MultiPoint)
				{
					com.epl.geometry.MultiPointImpl mp_impl = (com.epl.geometry.MultiPointImpl)geom._getImpl();
					com.epl.geometry.VertexDescription description = geom.GetDescription();
					// mp_impl.reserve(point_count);
					mp_impl.Resize(point_count);
					for (int iattrib = 0, nattrib = description.GetAttributeCount(); iattrib < nattrib; iattrib++)
					{
						int semantics = description.GetSemantics(iattrib);
						int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						com.epl.geometry.AttributeStreamBase dst_stream = mp_impl.GetAttributeStreamRef(semantics);
						// std::shared_ptr<Attribute_stream_base> dst_stream =
						// Attribute_stream_base::create_attribute_stream(semantics,
						// point_count);
						com.epl.geometry.AttributeStreamBase src_stream = m_vertices.GetAttributeStreamRef(semantics);
						int dst_index = 0;
						System.Diagnostics.Debug.Assert((GetPathCount(geometry) == 1));
						int path = GetFirstPath(geometry);
						int path_size = GetPathSize(path);
						for (int vertex = GetFirstVertex(path); dst_index < path_size; vertex = GetNextVertex(vertex), dst_index++)
						{
							int src_index = GetVertexIndex(vertex);
							for (int icomp = 0; icomp < ncomps; icomp++)
							{
								double d = src_stream.ReadAsDbl(src_index * ncomps + icomp);
								dst_stream.WriteAsDbl(dst_index * ncomps + icomp, d);
							}
						}
						mp_impl.SetAttributeStreamRef(semantics, dst_stream);
					}
					mp_impl.NotifyModified(com.epl.geometry.DirtyFlags.dirtyAll);
				}
				else
				{
					System.Diagnostics.Debug.Assert((false));
				}
			}
			return geom;
		}

		// create a new empty geometry of the given type
		internal int CreateGeometry(com.epl.geometry.Geometry.Type geometry_type)
		{
			return CreateGeometry(geometry_type, com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D());
		}

		// Deletes existing geometry from the edit shape and returns the next one.
		internal int RemoveGeometry(int geometry)
		{
			for (int path = GetFirstPath(geometry); path != -1; path = RemovePath(path))
			{
			}
			// removing paths in a loop
			int prev = GetPrevGeometry(geometry);
			int next = GetNextGeometry(geometry);
			if (prev != -1)
			{
				SetNextGeometry_(prev, next);
			}
			else
			{
				m_first_geometry = next;
			}
			if (next != -1)
			{
				SetPrevGeometry_(next, prev);
			}
			else
			{
				m_last_geometry = prev;
			}
			FreeGeometry_(geometry);
			return next;
		}

		// create a new empty geometry of the given type and attribute set.
		internal int CreateGeometry(com.epl.geometry.Geometry.Type geometry_type, com.epl.geometry.VertexDescription description)
		{
			int newgeom = NewGeometry_(geometry_type.Value());
			if (m_vertices == null)
			{
				m_vertices_mp = new com.epl.geometry.MultiPoint(description);
				m_vertices = (com.epl.geometry.MultiPointImpl)m_vertices_mp._getImpl();
			}
			else
			{
				m_vertices_mp.MergeVertexDescription(description);
			}
			m_vertex_description = m_vertices_mp.GetDescription();
			// this
			// description
			// will be a
			// merge of
			// existing
			// description
			// and the
			// description
			// of the
			// multi_path
			m_b_has_attributes = m_vertex_description.GetAttributeCount() > 1;
			if (m_first_geometry == -1)
			{
				m_first_geometry = newgeom;
				m_last_geometry = newgeom;
			}
			else
			{
				SetPrevGeometry_(newgeom, m_last_geometry);
				SetNextGeometry_(m_last_geometry, newgeom);
				m_last_geometry = newgeom;
			}
			return newgeom;
		}

		// Returns the first geometry in the Edit_shape.
		internal int GetFirstGeometry()
		{
			return m_first_geometry;
		}

		// Returns the next geometry in the Edit_shape. Returns -1 when there are no
		// more geometries.
		internal int GetNextGeometry(int geom)
		{
			return m_geometry_index_list.GetField(geom, 1);
		}

		// Returns the previous geometry in the Edit_shape. Returns -1 when there
		// are no more geometries.
		internal int GetPrevGeometry(int geom)
		{
			return m_geometry_index_list.GetField(geom, 0);
		}

		// Returns the type of the Geometry.
		internal int GetGeometryType(int geom)
		{
			return m_geometry_index_list.GetField(geom, 2) & unchecked((int)(0x7FFFFFFF));
		}

		// Sets value to the given user index on a geometry.
		internal void SetGeometryUserIndex(int geom, int index, int value)
		{
			com.epl.geometry.AttributeStreamOfInt32 stream = m_geometry_indices[index];
			int pindex = GetGeometryIndex_(geom);
			if (pindex >= stream.Size())
			{
				stream.Resize(System.Math.Max((int)(pindex * 1.25), (int)16), -1);
			}
			stream.Write(pindex, value);
		}

		// Returns the value of the given user index of a geometry
		internal int GetGeometryUserIndex(int geom, int index)
		{
			int pindex = GetGeometryIndex_(geom);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_geometry_indices[index];
			if (pindex < stream.Size())
			{
				return stream.Read(pindex);
			}
			else
			{
				return -1;
			}
		}

		// Creates new user index on a geometry. The geometry index allows to store
		// an integer user value on the geometry.
		// Until set_geometry_user_index is called for a given geometry, the index
		// stores -1 for that geometry.
		internal int CreateGeometryUserIndex()
		{
			if (m_geometry_indices == null)
			{
				m_geometry_indices = new System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32>(4);
			}
			// Try getting existing index. Use linear search. We do not expect many
			// indices to be created.
			for (int i = 0; i < m_geometry_indices.Count; i++)
			{
				if (m_geometry_indices[i] == null)
				{
					m_geometry_indices[i] = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0);
					return i;
				}
			}
			m_geometry_indices.Add((com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0));
			return m_geometry_indices.Count - 1;
		}

		// Removes the geometry user index.
		internal void RemoveGeometryUserIndex(int index)
		{
			m_geometry_indices[index] = null;
		}

		// Returns the first path of the geometry.
		internal int GetFirstPath(int geometry)
		{
			return m_geometry_index_list.GetField(geometry, 3);
		}

		// Returns the first path of the geometry.
		internal int GetLastPath(int geometry)
		{
			return m_geometry_index_list.GetField(geometry, 4);
		}

		// Point count in a geometry
		internal int GetPointCount(int geom)
		{
			return m_geometry_index_list.GetField(geom, 5);
		}

		// Path count in a geometry
		internal int GetPathCount(int geom)
		{
			return m_geometry_index_list.GetField(geom, 6);
		}

		// Filters degenerate segments in all multipath geometries
		// Returns 1 if a non-zero length segment has been removed. -1, if only zero
		// length segments have been removed.
		// 0 if no segments have been removed.
		// When b_remove_last_vertices and the result path is < 3 for polygon or < 2
		// for polyline, it'll be removed.
		internal int FilterClosePoints(double tolerance, bool b_remove_last_vertices, bool only_polygons)
		{
			int res = 0;
			for (int geometry = GetFirstGeometry(); geometry != -1; geometry = GetNextGeometry(geometry))
			{
				int gt = GetGeometryType(geometry);
				if (!com.epl.geometry.Geometry.IsMultiPath(gt))
				{
					continue;
				}
				if (only_polygons && gt != com.epl.geometry.Geometry.GeometryType.Polygon)
				{
					continue;
				}
				bool b_polygon = GetGeometryType(geometry) == com.epl.geometry.Geometry.GeometryType.Polygon;
				for (int path = GetFirstPath(geometry); path != -1; )
				{
					// We go from the start to the half of the path first, then we
					// go from the end to the half of the path.
					int vertex_counter = 0;
					for (int vertex = GetFirstVertex(path); vertex_counter < GetPathSize(path) / 2; )
					{
						int next = GetNextVertex(vertex);
						if (next == -1)
						{
							break;
						}
						int vindex = GetVertexIndex(vertex);
						com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex);
						double length = 0;
						if (seg != null)
						{
							length = seg.CalculateLength2D();
						}
						else
						{
							int vindex_next = GetVertexIndex(next);
							length = m_vertices._getShortestDistance(vindex, vindex_next);
						}
						if (length <= tolerance)
						{
							if (length == 0)
							{
								if (res == 0)
								{
									res = -1;
								}
							}
							else
							{
								res = 1;
							}
							if (next != GetLastVertex(path))
							{
								TransferAllDataToTheVertex(next, vertex);
								RemoveVertex(next, true);
							}
						}
						else
						{
							vertex = GetNextVertex(vertex);
						}
						vertex_counter++;
					}
					int first_vertex = GetFirstVertex(path);
					for (int vertex_1 = IsClosedPath(path) ? first_vertex : GetLastVertex(path); GetPathSize(path) > 0; )
					{
						int prev = GetPrevVertex(vertex_1);
						if (prev != -1)
						{
							int vindex_prev = GetVertexIndex(prev);
							com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex_prev);
							double length = 0;
							if (seg != null)
							{
								length = seg.CalculateLength2D();
							}
							else
							{
								int vindex = GetVertexIndex(vertex_1);
								length = m_vertices._getShortestDistance(vindex, vindex_prev);
							}
							if (length <= tolerance)
							{
								if (length == 0)
								{
									if (res == 0)
									{
										res = -1;
									}
								}
								else
								{
									res = 1;
								}
								TransferAllDataToTheVertex(prev, vertex_1);
								RemoveVertex(prev, false);
								if (first_vertex == prev)
								{
									first_vertex = GetFirstVertex(path);
								}
							}
							else
							{
								vertex_1 = GetPrevVertex(vertex_1);
								if (vertex_1 == first_vertex)
								{
									break;
								}
							}
						}
						else
						{
							RemoveVertex(vertex_1, true);
							// remove the last vertex in
							// the path
							if (res == 0)
							{
								res = -1;
							}
							break;
						}
					}
					int path_size = GetPathSize(path);
					if (b_remove_last_vertices && (b_polygon ? path_size < 3 : path_size < 2))
					{
						path = RemovePath(path);
						res = path_size > 0 ? 1 : (res == 0 ? -1 : res);
					}
					else
					{
						path = GetNextPath(path);
					}
				}
			}
			return res;
		}

		// Checks if there are degenerate segments in any of multipath geometries
		internal bool HasDegenerateSegments(double tolerance)
		{
			for (int geometry = GetFirstGeometry(); geometry != -1; geometry = GetNextGeometry(geometry))
			{
				if (!com.epl.geometry.Geometry.IsMultiPath(GetGeometryType(geometry)))
				{
					continue;
				}
				bool b_polygon = GetGeometryType(geometry) == com.epl.geometry.Geometry.GeometryType.Polygon;
				for (int path = GetFirstPath(geometry); path != -1; )
				{
					int path_size = GetPathSize(path);
					if (b_polygon ? path_size < 3 : path_size < 2)
					{
						return true;
					}
					int vertex = GetFirstVertex(path);
					for (int index = 0; index < path_size; index++)
					{
						int next = GetNextVertex(vertex);
						if (next == -1)
						{
							break;
						}
						int vindex = GetVertexIndex(vertex);
						com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex);
						double length = 0;
						if (seg != null)
						{
							length = seg.CalculateLength2D();
						}
						else
						{
							int vindex_next = GetVertexIndex(next);
							length = m_vertices._getShortestDistance(vindex, vindex_next);
						}
						if (length <= tolerance)
						{
							return true;
						}
						vertex = next;
					}
					path = GetNextPath(path);
				}
			}
			return false;
		}

		internal void TransferAllDataToTheVertex(int from_vertex, int to_vertex)
		{
			int vindexFrom = GetVertexIndex(from_vertex);
			int vindexTo = GetVertexIndex(to_vertex);
			if (m_weights != null)
			{
				double weight = m_weights.Read(vindexFrom);
				m_weights.Write(vindexTo, weight);
			}
			if (m_b_has_attributes)
			{
			}
			// TODO: implement copying of attributes with exception of x and y
			//
			// for (int i = 0, nattrib = 0; i < nattrib; i++)
			// {
			// m_vertices->get_attribute
			// }
			// Copy user index data
			if (m_indices != null)
			{
				for (int i = 0, n = (int)m_indices.Count; i < n; i++)
				{
					if (m_indices[i] != null)
					{
						int value = GetUserIndex(from_vertex, i);
						if (value != -1)
						{
							SetUserIndex(to_vertex, i, value);
						}
					}
				}
			}
		}

		// Splits segment originating from the origingVertex split_count times at
		// splitScalar points and inserts new vertices into the shape.
		// The split is not done, when the splitScalar[i] is 0 or 1, or is equal to
		// the splitScalar[i - 1].
		// Returns the number of splits actually happend (0 if no splits have
		// happend).
		internal int SplitSegment(int origin_vertex, double[] split_scalars, int split_count)
		{
			int actual_splits = 0;
			int next_vertex = GetNextVertex(origin_vertex);
			if (next_vertex == -1)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			int vindex = GetVertexIndex(origin_vertex);
			int vindex_next = GetVertexIndex(next_vertex);
			com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex);
			double seg_length = seg == null ? m_vertices._getShortestDistance(vindex, vindex_next) : seg.CalculateLength2D();
			double told = 0.0;
			for (int i = 0; i < split_count; i++)
			{
				double t = split_scalars[i];
				if (told < t && t < 1.0)
				{
					double f = t;
					if (seg != null)
					{
						f = seg_length > 0 ? seg._calculateSubLength(t) / seg_length : 0.0;
					}
					m_vertices._interpolateTwoVertices(vindex, vindex_next, f, GetHelperPoint_());
					// use this call mainly to
					// interpolate the attributes. XYs
					// are interpolated incorrectly for
					// curves and are recalculated when
					// segment is cut below.
					int inserted_vertex = InsertVertex_(GetPathFromVertex(origin_vertex), next_vertex, GetHelperPoint_());
					actual_splits++;
					if (seg != null)
					{
						com.epl.geometry.Segment subseg = seg.Cut(told, t);
						int prev_vertex = GetPrevVertex(inserted_vertex);
						int vindex_prev = GetVertexIndex(prev_vertex);
						SetSegmentToIndex_(vindex_prev, subseg);
						SetXY(inserted_vertex, subseg.GetEndXY());
						// fix XY
						// coordinates
						// to be
						// parameter
						// based
						// (interpolate_two_vertices_)
						if (i == split_count - 1 || split_scalars[i + 1] == 1.0)
						{
							// last
							// chance
							// to
							// set
							// last
							// split
							// segment
							// here:
							com.epl.geometry.Segment subseg_end = seg.Cut(t, 1.0);
							SetSegmentToIndex_(vindex_prev, subseg_end);
						}
					}
				}
			}
			return actual_splits;
		}

		// interpolates the attributes for the specified path between from_vertex
		// and to_vertex
		internal void InterpolateAttributesForClosedPath(int path, int from_vertex, int to_vertex)
		{
			System.Diagnostics.Debug.Assert((IsClosedPath(path)));
			if (!m_b_has_attributes)
			{
				return;
			}
			double sub_length = CalculateSubLength2D(path, from_vertex, to_vertex);
			if (sub_length == 0.0)
			{
				return;
			}
			int nattr = m_vertex_description.GetAttributeCount();
			for (int iattr = 1; iattr < nattr; iattr++)
			{
				int semantics = m_vertex_description.GetSemantics(iattr);
				int interpolation = com.epl.geometry.VertexDescription.GetInterpolation(semantics);
				if (interpolation == com.epl.geometry.VertexDescription.Interpolation.ANGULAR)
				{
					continue;
				}
				int components = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int ordinate = 0; ordinate < components; ordinate++)
				{
					InterpolateAttributesForClosedPath_(semantics, path, from_vertex, to_vertex, sub_length, ordinate);
				}
			}
			return;
		}

		// calculates the length for the specified path between from_vertex and
		// to_vertex
		internal double CalculateSubLength2D(int path, int from_vertex, int to_vertex)
		{
			int shape_from_index = GetVertexIndex(from_vertex);
			int shape_to_index = GetVertexIndex(to_vertex);
			if (shape_from_index < 0 || shape_to_index > GetTotalPointCount() - 1)
			{
				throw new System.ArgumentException("invalid call");
			}
			if (shape_from_index > shape_to_index)
			{
				if (!IsClosedPath(path))
				{
					throw new System.ArgumentException("cannot iterate across an open path");
				}
			}
			double sub_length = 0.0;
			for (int vertex = from_vertex; vertex != to_vertex; vertex = GetNextVertex(vertex))
			{
				int vertex_index = GetVertexIndex(vertex);
				com.epl.geometry.Segment segment = GetSegmentFromIndex_(vertex_index);
				if (segment != null)
				{
					sub_length += segment.CalculateLength2D();
				}
				else
				{
					int next_vertex_index = GetVertexIndex(GetNextVertex(vertex));
					sub_length += m_vertices._getShortestDistance(vertex_index, next_vertex_index);
				}
			}
			return sub_length;
		}

		// set_point modifies the vertex and associated segments.
		internal void SetPoint(int vertex, com.epl.geometry.Point new_coord)
		{
			int vindex = GetVertexIndex(vertex);
			m_vertices.SetPointByVal(vindex, new_coord);
			com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex);
			if (seg != null)
			{
				seg.SetStart(new_coord);
			}
			int prev = GetPrevVertex(vertex);
			if (prev != -1)
			{
				int vindex_p = GetVertexIndex(prev);
				com.epl.geometry.Segment seg_p = GetSegmentFromIndex_(vindex_p);
				if (seg_p != null)
				{
					seg.SetEnd(new_coord);
				}
			}
		}

		// Queries point for a given vertex.
		internal void QueryPoint(int vertex, com.epl.geometry.Point point)
		{
			int vindex = GetVertexIndex(vertex);
			m_vertices.GetPointByVal(vindex, point);
		}

		// assert(getXY(vertex) == point.getXY());
		// set_xy modifies the vertex and associated segments.
		internal void SetXY(int vertex, com.epl.geometry.Point2D new_coord)
		{
			SetXY(vertex, new_coord.x, new_coord.y);
		}

		// set_xy modifies the vertex and associated segments.
		internal void SetXY(int vertex, double new_x, double new_y)
		{
			int vindex = GetVertexIndex(vertex);
			m_vertices.SetXY(vindex, new_x, new_y);
			com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex);
			if (seg != null)
			{
				seg.SetStartXY(new_x, new_y);
			}
			int prev = GetPrevVertex(vertex);
			if (prev != -1)
			{
				int vindex_p = GetVertexIndex(prev);
				com.epl.geometry.Segment seg_p = GetSegmentFromIndex_(vindex_p);
				if (seg_p != null)
				{
					seg.SetEndXY(new_x, new_y);
				}
			}
		}

		internal com.epl.geometry.Point2D GetXY(int vertex)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			int vindex = GetVertexIndex(vertex);
			pt.SetCoords(m_vertices.GetXY(vindex));
			return pt;
		}

		// Returns the coordinates of the vertex.
		internal void GetXY(int vertex, com.epl.geometry.Point2D ptOut)
		{
			int vindex = GetVertexIndex(vertex);
			ptOut.SetCoords(m_vertices.GetXY(vindex));
		}

		internal void GetXYWithIndex(int index, com.epl.geometry.Point2D ptOut)
		{
			m_xy_stream.Read(2 * index, ptOut);
		}

		// Gets the attribute for the given semantics and ordinate.
		internal double GetAttributeAsDbl(int semantics, int vertex, int ordinate)
		{
			return m_vertices.GetAttributeAsDbl(semantics, GetVertexIndex(vertex), ordinate);
		}

		// Sets the attribute for the given semantics and ordinate.
		internal void SetAttribute(int semantics, int vertex, int ordinate, double value)
		{
			m_vertices.SetAttribute(semantics, GetVertexIndex(vertex), ordinate, value);
		}

		// Sets the attribute for the given semantics and ordinate.
		internal void SetAttribute(int semantics, int vertex, int ordinate, int value)
		{
			m_vertices.SetAttribute(semantics, GetVertexIndex(vertex), ordinate, value);
		}

		// Returns a reference to the vertex description
		internal com.epl.geometry.VertexDescription GetVertexDescription()
		{
			return m_vertex_description;
		}

		internal int GetMinPathVertexY(int path)
		{
			int first_vert = GetFirstVertex(path);
			int minv = first_vert;
			int vert = GetNextVertex(first_vert);
			while (vert != -1 && vert != first_vert)
			{
				if (CompareVerticesSimpleY_(vert, minv) < 0)
				{
					minv = vert;
				}
				vert = GetNextVertex(vert);
			}
			return minv;
		}

		// Returns an index value for the vertex inside of the underlying array of
		// vertices.
		// This index is for the use with the get_xy_with_index. get_xy is
		// equivalent to calling get_vertex_index and get_xy_with_index.
		internal int GetVertexIndex(int vertex)
		{
			return m_vertex_index_list.GetField(vertex, 0);
		}

		// Returns the y coordinate of the vertex.
		internal double GetY(int vertex)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			GetXY(vertex, pt);
			return pt.y;
		}

		// returns True if xy coordinates at vertices are equal.
		internal bool IsEqualXY(int vertex_1, int vertex_2)
		{
			int vindex1 = GetVertexIndex(vertex_1);
			int vindex2 = GetVertexIndex(vertex_2);
			return m_vertices.GetXY(vindex1).IsEqual(m_vertices.GetXY(vindex2));
		}

		// returns True if xy coordinates at vertices are equal.
		internal bool IsEqualXY(int vertex, com.epl.geometry.Point2D pt)
		{
			int vindex = GetVertexIndex(vertex);
			return m_vertices.GetXY(vindex).IsEqual(pt);
		}

		// Sets weight to the vertex. Weight is used by clustering and cracking.
		internal void SetWeight(int vertex, double weight)
		{
			if (weight < 1.0)
			{
				weight = 1.0;
			}
			if (m_weights == null)
			{
				if (weight == 1.0)
				{
					return;
				}
				m_weights = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateDoubleStream(m_vertices.GetPointCount(), 1.0));
			}
			int vindex = GetVertexIndex(vertex);
			if (vindex >= m_weights.Size())
			{
				m_weights.Resize(vindex + 1, 1.0);
			}
			m_weights.Write(vindex, weight);
		}

		internal double GetWeight(int vertex)
		{
			int vindex = GetVertexIndex(vertex);
			if (m_weights == null || vindex >= m_weights.Size())
			{
				return 1.0;
			}
			return m_weights.Read(vindex);
		}

		// Removes associated weights
		internal void RemoveWeights()
		{
			m_weights = null;
		}

		// Sets value to the given user index.
		internal void SetUserIndex(int vertex, int index, int value)
		{
			// CHECKVERTEXHANDLE(vertex);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_indices[index];
			// assert(get_prev_vertex(vertex) != -0x7eadbeaf);//using deleted vertex
			int vindex = GetVertexIndex(vertex);
			if (stream.Size() < m_vertices.GetPointCount())
			{
				stream.Resize(m_vertices.GetPointCount(), -1);
			}
			stream.Write(vindex, value);
		}

		internal int GetUserIndex(int vertex, int index)
		{
			// CHECKVERTEXHANDLE(vertex);
			int vindex = GetVertexIndex(vertex);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_indices[index];
			if (vindex < stream.Size())
			{
				int val = stream.Read(vindex);
				return val;
			}
			else
			{
				return -1;
			}
		}

		// Creates new user index. The index have random values. The index allows to
		// store an integer user value on the vertex.
		internal int CreateUserIndex()
		{
			if (m_indices == null)
			{
				m_indices = new System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32>(0);
			}
			// Try getting existing index. Use linear search. We do not expect many
			// indices to be created.
			for (int i = 0; i < m_indices.Count; i++)
			{
				if (m_indices[i] == null)
				{
					m_indices[i] = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0, -1);
					return i;
				}
			}
			m_indices.Add((com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0, -1));
			return m_indices.Count - 1;
		}

		// Removes the user index.
		internal void RemoveUserIndex(int index)
		{
			m_indices[index] = null;
		}

		// Returns segment, connecting currentVertex and next vertex. Returns NULL
		// if it is a Line.
		internal com.epl.geometry.Segment GetSegment(int vertex)
		{
			if (m_segments != null)
			{
				int vindex = GetVertexIndex(vertex);
				return m_segments[vindex];
			}
			return null;
		}

		// Returns a straight line that connects this and next vertices. No
		// attributes. Returns false if no next vertex exists (end of polyline
		// part).
		// Can be used together with get_segment.
		internal bool QueryLineConnector(int vertex, com.epl.geometry.Line line)
		{
			int next = GetNextVertex(vertex);
			if (next == -1)
			{
				return false;
			}
			if (!m_b_has_attributes)
			{
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				GetXY(vertex, pt);
				line.SetStartXY(pt);
				GetXY(next, pt);
				line.SetEndXY(pt);
			}
			else
			{
				com.epl.geometry.Point pt = new com.epl.geometry.Point();
				QueryPoint(vertex, pt);
				line.SetStart(pt);
				QueryPoint(next, pt);
				line.SetEnd(pt);
			}
			return true;
		}

		// Inserts an empty path before the given one. If before_path is -1, adds
		// path at the end.
		internal int InsertPath(int geometry, int before_path)
		{
			int prev = -1;
			if (before_path != -1)
			{
				if (geometry != GetGeometryFromPath(before_path))
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
				prev = GetPrevPath(before_path);
			}
			else
			{
				prev = GetLastPath(geometry);
			}
			int newpath = NewPath_(geometry);
			if (before_path != -1)
			{
				SetPrevPath_(before_path, newpath);
			}
			SetNextPath_(newpath, before_path);
			SetPrevPath_(newpath, prev);
			if (prev != -1)
			{
				SetNextPath_(prev, newpath);
			}
			else
			{
				SetFirstPath_(geometry, newpath);
			}
			if (before_path == -1)
			{
				SetLastPath_(geometry, newpath);
			}
			SetGeometryPathCount_(geometry, GetPathCount(geometry) + 1);
			return newpath;
		}

		internal int InsertClosedPath_(int geometry, int before_path, int first_vertex, int checked_vertex, bool[] contains_checked_vertex)
		{
			int path = InsertPath(geometry, -1);
			int path_size = 0;
			int vertex = first_vertex;
			bool contains = false;
			while (true)
			{
				if (vertex == checked_vertex)
				{
					contains = true;
				}
				SetPathToVertex_(vertex, path);
				path_size++;
				int next = GetNextVertex(vertex);
				System.Diagnostics.Debug.Assert((GetNextVertex(GetPrevVertex(vertex)) == vertex));
				if (next == first_vertex)
				{
					break;
				}
				vertex = next;
			}
			SetClosedPath(path, true);
			SetPathSize_(path, path_size);
			if (contains)
			{
				first_vertex = checked_vertex;
			}
			SetFirstVertex_(path, first_vertex);
			SetLastVertex_(path, GetPrevVertex(first_vertex));
			SetRingAreaValid_(path, false);
			if (contains_checked_vertex != null)
			{
				contains_checked_vertex[0] = contains;
			}
			return path;
		}

		// Removes a path, gets rid of all its vertices, and returns the next one
		internal int RemovePath(int path)
		{
			int prev = GetPrevPath(path);
			int next = GetNextPath(path);
			int geometry = GetGeometryFromPath(path);
			if (prev != -1)
			{
				SetNextPath_(prev, next);
			}
			else
			{
				SetFirstPath_(geometry, next);
			}
			if (next != -1)
			{
				SetPrevPath_(next, prev);
			}
			else
			{
				SetLastPath_(geometry, prev);
			}
			ClearPath(path);
			SetGeometryPathCount_(geometry, GetPathCount(geometry) - 1);
			FreePath_(path);
			return next;
		}

		// Clears all vertices from the path
		internal void ClearPath(int path)
		{
			int first_vertex = GetFirstVertex(path);
			if (first_vertex != -1)
			{
				// TODO: can ve do this in one shot?
				int vertex = first_vertex;
				for (int i = 0, n = GetPathSize(path); i < n; i++)
				{
					int v = vertex;
					vertex = GetNextVertex(vertex);
					FreeVertex_(v);
				}
				int geometry = GetGeometryFromPath(path);
				SetGeometryVertexCount_(geometry, GetPointCount(geometry) - GetPathSize(path));
			}
			SetPathSize_(path, 0);
		}

		// Returns the next path (-1 if there are no more paths in the geometry).
		internal int GetNextPath(int currentPath)
		{
			return m_path_index_list.GetField(currentPath, 2);
		}

		// Returns the previous path (-1 if there are no more paths in the
		// geometry).
		internal int GetPrevPath(int currentPath)
		{
			return m_path_index_list.GetField(currentPath, 1);
		}

		// Returns the number of vertices in the path.
		internal int GetPathSize(int path)
		{
			return m_path_index_list.GetField(path, 3);
		}

		// Returns True if the path is closed.
		internal bool IsClosedPath(int path)
		{
			return (GetPathFlags_(path) & com.epl.geometry.EditShape.PathFlags_.closedPath) != 0;
		}

		// Makes path closed. Closed paths are circular lists. get_next_vertex
		// always succeeds
		internal void SetClosedPath(int path, bool b_yes_no)
		{
			if (IsClosedPath(path) == b_yes_no)
			{
				return;
			}
			if (GetPathSize(path) > 0)
			{
				int first = GetFirstVertex(path);
				int last = GetLastVertex(path);
				if (b_yes_no)
				{
					// make a circular list
					SetNextVertex_(last, first);
					SetPrevVertex_(first, last);
					// set segment to NULL (just in case)
					int vindex = GetVertexIndex(last);
					SetSegmentToIndex_(vindex, null);
				}
				else
				{
					SetNextVertex_(last, -1);
					SetPrevVertex_(first, -1);
					int vindex = GetVertexIndex(last);
					SetSegmentToIndex_(vindex, null);
				}
			}
			int oldflags = GetPathFlags_(path);
			int flags = (oldflags | (int)com.epl.geometry.EditShape.PathFlags_.closedPath) - (int)com.epl.geometry.EditShape.PathFlags_.closedPath;
			// clear the bit;
			SetPathFlags_(path, flags | (b_yes_no ? (int)com.epl.geometry.EditShape.PathFlags_.closedPath : 0));
		}

		// Closes all paths of the geometry (has to be a polyline or polygon).
		internal void CloseAllPaths(int geometry)
		{
			if (GetGeometryType(geometry) == com.epl.geometry.Geometry.GeometryType.Polygon)
			{
				return;
			}
			if (!com.epl.geometry.Geometry.IsLinear(GetGeometryType(geometry)))
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			for (int path = GetFirstPath(geometry); path != -1; path = GetNextPath(path))
			{
				SetClosedPath(path, true);
			}
		}

		// Returns geometry from path
		internal int GetGeometryFromPath(int path)
		{
			return m_path_index_list.GetField(path, 7);
		}

		// Returns True if the path is exterior.
		internal bool IsExterior(int path)
		{
			return (GetPathFlags_(path) & com.epl.geometry.EditShape.PathFlags_.exteriorPath) != 0;
		}

		// Sets exterior flag
		internal void SetExterior(int path, bool b_yes_no)
		{
			int oldflags = GetPathFlags_(path);
			int flags = (oldflags | (int)com.epl.geometry.EditShape.PathFlags_.exteriorPath) - (int)com.epl.geometry.EditShape.PathFlags_.exteriorPath;
			// clear the bit;
			SetPathFlags_(path, flags | (b_yes_no ? (int)com.epl.geometry.EditShape.PathFlags_.exteriorPath : 0));
		}

		// Returns the ring area
		internal double GetRingArea(int path)
		{
			if (IsRingAreaValid_(path))
			{
				return m_path_areas.Get(GetPathIndex_(path));
			}
			com.epl.geometry.Line line = new com.epl.geometry.Line();
			int vertex = GetFirstVertex(path);
			if (vertex == -1)
			{
				return 0;
			}
			com.epl.geometry.Point2D pt0 = new com.epl.geometry.Point2D();
			GetXY(vertex, pt0);
			double area = 0;
			for (int i = 0, n = GetPathSize(path); i < n; i++, vertex = GetNextVertex(vertex))
			{
				com.epl.geometry.Segment seg = GetSegment(vertex);
				if (seg == null)
				{
					if (!QueryLineConnector(vertex, line))
					{
						continue;
					}
					seg = line;
				}
				double a = seg._calculateArea2DHelper(pt0.x, pt0.y);
				area += a;
			}
			SetRingAreaValid_(path, true);
			m_path_areas.Set(GetPathIndex_(path), area);
			return area;
		}

		// Sets value to the given user index on a path.
		internal void SetPathUserIndex(int path, int index, int value)
		{
			com.epl.geometry.AttributeStreamOfInt32 stream = m_pathindices[index];
			int pindex = GetPathIndex_(path);
			if (stream.Size() < m_path_areas.Size())
			{
				stream.Resize(m_path_areas.Size(), -1);
			}
			stream.Write(pindex, value);
		}

		// Returns the value of the given user index of a path
		internal int GetPathUserIndex(int path, int index)
		{
			int pindex = GetPathIndex_(path);
			com.epl.geometry.AttributeStreamOfInt32 stream = m_pathindices[index];
			if (pindex < stream.Size())
			{
				return stream.Read(pindex);
			}
			else
			{
				return -1;
			}
		}

		// Creates new user index on a path. The index have random values. The path
		// index allows to store an integer user value on the path.
		internal int CreatePathUserIndex()
		{
			if (m_pathindices == null)
			{
				m_pathindices = new System.Collections.Generic.List<com.epl.geometry.AttributeStreamOfInt32>(0);
			}
			// Try getting existing index. Use linear search. We do not expect many
			// indices to be created.
			for (int i = 0; i < m_pathindices.Count; i++)
			{
				if (m_pathindices[i] == null)
				{
					m_pathindices[i] = (com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(0));
					return i;
				}
			}
			m_pathindices.Add((com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(0)));
			return (int)(m_pathindices.Count - 1);
		}

		// Removes the path user index.
		internal void RemovePathUserIndex(int index)
		{
			m_pathindices[index] = null;
		}

		// Moves a path from any geometry before a given path in the dst_geom
		// geometry. The path_handle do not change after the operation.
		// before_path can be -1, then the path is moved to the end of the dst_geom.
		internal void MovePath(int geom, int before_path, int path_to_move)
		{
			if (path_to_move == -1)
			{
				throw new System.ArgumentException();
			}
			if (before_path == path_to_move)
			{
				return;
			}
			int next = GetNextPath(path_to_move);
			int prev = GetPrevPath(path_to_move);
			int geom_src = GetGeometryFromPath(path_to_move);
			if (prev == -1)
			{
				SetFirstPath_(geom_src, next);
			}
			else
			{
				SetNextPath_(prev, next);
			}
			if (next == -1)
			{
				SetLastPath_(geom_src, prev);
			}
			else
			{
				SetPrevPath_(next, prev);
			}
			SetGeometryVertexCount_(geom_src, GetPointCount(geom_src) - GetPathSize(path_to_move));
			SetGeometryPathCount_(geom_src, GetPathCount(geom_src) - 1);
			if (before_path == -1)
			{
				prev = GetLastPath(geom);
			}
			else
			{
				prev = GetPrevPath(before_path);
			}
			SetPrevPath_(path_to_move, prev);
			SetNextPath_(path_to_move, before_path);
			if (before_path == -1)
			{
				SetLastPath_(geom, path_to_move);
			}
			else
			{
				SetPrevPath_(before_path, path_to_move);
			}
			if (prev == -1)
			{
				SetFirstPath_(geom, path_to_move);
			}
			else
			{
				SetNextPath_(prev, path_to_move);
			}
			SetGeometryVertexCount_(geom, GetPointCount(geom) + GetPathSize(path_to_move));
			SetGeometryPathCount_(geom, GetPathCount(geom) + 1);
			SetPathGeometry_(path_to_move, geom);
		}

		// Adds a copy of a vertex to a path. Connects with a straight line.
		// Returns new vertex handle.
		internal int AddVertex(int path, int vertex)
		{
			m_vertices.GetPointByVal(GetVertexIndex(vertex), GetHelperPoint_());
			return InsertVertex_(path, -1, GetHelperPoint_());
		}

		// Removes vertex from path. Uses either left or right segments to
		// reconnect. Returns next vertex after erased one.
		internal int RemoveVertex(int vertex, bool b_left_segment)
		{
			int path = GetPathFromVertex(vertex);
			int prev = GetPrevVertex(vertex);
			int next = GetNextVertex(vertex);
			if (prev != -1)
			{
				SetNextVertex_(prev, next);
			}
			int path_size = GetPathSize(path);
			if (vertex == GetFirstVertex(path))
			{
				SetFirstVertex_(path, path_size > 1 ? next : -1);
			}
			if (next != -1)
			{
				SetPrevVertex_(next, prev);
			}
			if (vertex == GetLastVertex(path))
			{
				SetLastVertex_(path, path_size > 1 ? prev : -1);
			}
			if (prev != -1 && next != -1)
			{
				int vindex_prev = GetVertexIndex(prev);
				int vindex_next = GetVertexIndex(next);
				if (b_left_segment)
				{
					com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex_prev);
					if (seg != null)
					{
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						m_vertices.GetXY(vindex_next, pt);
						seg.SetEndXY(pt);
					}
				}
				else
				{
					int vindex_erased = GetVertexIndex(vertex);
					com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex_erased);
					SetSegmentToIndex_(vindex_prev, seg);
					if (seg != null)
					{
						com.epl.geometry.Point2D pt = m_vertices.GetXY(vindex_prev);
						seg.SetStartXY(pt);
					}
				}
			}
			SetPathSize_(path, path_size - 1);
			int geometry = GetGeometryFromPath(path);
			SetGeometryVertexCount_(geometry, GetPointCount(geometry) - 1);
			FreeVertex_(vertex);
			return next;
		}

		// Returns first vertex of the given path.
		internal int GetFirstVertex(int path)
		{
			return m_path_index_list.GetField(path, 4);
		}

		// Returns last vertex of the given path. For the closed paths
		// get_next_vertex for the last vertex returns the first vertex.
		internal int GetLastVertex(int path)
		{
			return m_path_index_list.GetField(path, 5);
		}

		// Returns next vertex. Closed paths are circular lists, so get_next_vertex
		// always returns vertex. Open paths return -1 for last vertex.
		internal int GetNextVertex(int currentVertex)
		{
			return m_vertex_index_list.GetField(currentVertex, 2);
		}

		// Returns previous vertex. Closed paths are circular lists, so
		// get_prev_vertex always returns vertex. Open paths return -1 for first
		// vertex.
		internal int GetPrevVertex(int currentVertex)
		{
			return m_vertex_index_list.GetField(currentVertex, 1);
		}

		internal int GetPrevVertex(int currentVertex, int dir)
		{
			return dir > 0 ? m_vertex_index_list.GetField(currentVertex, 1) : m_vertex_index_list.GetField(currentVertex, 2);
		}

		internal int GetNextVertex(int currentVertex, int dir)
		{
			return dir > 0 ? m_vertex_index_list.GetField(currentVertex, 2) : m_vertex_index_list.GetField(currentVertex, 1);
		}

		// Returns a path the vertex belongs to.
		internal int GetPathFromVertex(int vertex)
		{
			return m_vertex_index_list.GetField(vertex, 3);
		}

		// Adds a copy of the point to a path. Connects with a straight line.
		// Returns new vertex handle.
		internal int AddPoint(int path, com.epl.geometry.Point point)
		{
			return InsertVertex_(path, -1, point);
		}

		internal class VertexIterator
		{
			private com.epl.geometry.EditShape m_parent;

			private int m_geometry;

			private int m_path;

			private int m_vertex;

			private int m_first_vertex;

			private int m_index;

			internal bool m_b_first;

			internal bool m_b_skip_mulit_points;

			private VertexIterator(com.epl.geometry.EditShape parent, int geometry, int path, int vertex, int first_vertex, int index, bool b_skip_mulit_points)
			{
				// Vertex iterator allows to go through all vertices of the Edit_shape.
				m_parent = parent;
				m_geometry = geometry;
				m_path = path;
				m_vertex = vertex;
				m_index = index;
				m_b_skip_mulit_points = b_skip_mulit_points;
				m_first_vertex = first_vertex;
				m_b_first = true;
			}

			internal virtual int MoveToNext_()
			{
				if (m_b_first)
				{
					m_b_first = false;
					return m_vertex;
				}
				if (m_vertex != -1)
				{
					m_vertex = m_parent.GetNextVertex(m_vertex);
					m_index++;
					if (m_vertex != -1 && m_vertex != m_first_vertex)
					{
						return m_vertex;
					}
					return MoveToNextHelper_();
				}
				// separate into another function for
				// inlining
				return -1;
			}

			internal virtual int MoveToNextHelper_()
			{
				m_path = m_parent.GetNextPath(m_path);
				m_index = 0;
				while (m_geometry != -1)
				{
					for (; m_path != -1; m_path = m_parent.GetNextPath(m_path))
					{
						m_vertex = m_parent.GetFirstVertex(m_path);
						m_first_vertex = m_vertex;
						if (m_vertex != -1)
						{
							return m_vertex;
						}
					}
					m_geometry = m_parent.GetNextGeometry(m_geometry);
					if (m_geometry == -1)
					{
						break;
					}
					if (m_b_skip_mulit_points && !com.epl.geometry.Geometry.IsMultiPath(m_parent.GetGeometryType(m_geometry)))
					{
						continue;
					}
					m_path = m_parent.GetFirstPath(m_geometry);
				}
				return -1;
			}

			internal VertexIterator(com.epl.geometry.EditShape.VertexIterator source)
			{
				// moves to next vertex. Returns -1 when there are no more vertices.
				m_parent = source.m_parent;
				m_geometry = source.m_geometry;
				m_path = source.m_path;
				m_vertex = source.m_vertex;
				m_index = source.m_index;
				m_b_skip_mulit_points = source.m_b_skip_mulit_points;
				m_first_vertex = source.m_first_vertex;
				m_b_first = true;
			}

			public virtual int Next()
			{
				return MoveToNext_();
			}

			public virtual int CurrentGeometry()
			{
				System.Diagnostics.Debug.Assert((m_vertex != -1));
				return m_geometry;
			}

			public virtual int CurrentPath()
			{
				System.Diagnostics.Debug.Assert((m_vertex != -1));
				return m_path;
			}

			public static com.epl.geometry.EditShape.VertexIterator Create_(com.epl.geometry.EditShape parent, int geometry, int path, int vertex, int first_vertex, int index, bool b_skip_mulit_points)
			{
				return new com.epl.geometry.EditShape.VertexIterator(parent, geometry, path, vertex, first_vertex, index, b_skip_mulit_points);
			}
		}

		// Returns the vertex iterator that allows iteration through all vertices of
		// all paths of all geometries.
		internal com.epl.geometry.EditShape.VertexIterator QueryVertexIterator()
		{
			return QueryVertexIterator(false);
		}

		internal com.epl.geometry.EditShape.VertexIterator QueryVertexIterator(com.epl.geometry.EditShape.VertexIterator source)
		{
			return new com.epl.geometry.EditShape.VertexIterator(source);
		}

		// Returns the vertex iterator that allows iteration through all vertices of
		// all paths of all geometries.
		// If bSkipMultiPoints is true, then the iterator will skip the Multi_point
		// vertices
		internal com.epl.geometry.EditShape.VertexIterator QueryVertexIterator(bool b_skip_multi_points)
		{
			int geometry = -1;
			int path = -1;
			int vertex = -1;
			int first_vertex = -1;
			int index = 0;
			bool bFound = false;
			for (geometry = GetFirstGeometry(); geometry != -1; geometry = GetNextGeometry(geometry))
			{
				if (b_skip_multi_points && !com.epl.geometry.Geometry.IsMultiPath(GetGeometryType(geometry)))
				{
					continue;
				}
				for (path = GetFirstPath(geometry); path != -1; path = GetNextPath(path))
				{
					vertex = GetFirstVertex(path);
					first_vertex = vertex;
					index = 0;
					if (vertex == -1)
					{
						continue;
					}
					bFound = true;
					break;
				}
				if (bFound)
				{
					break;
				}
			}
			return com.epl.geometry.EditShape.VertexIterator.Create_(this, geometry, path, vertex, first_vertex, index, b_skip_multi_points);
		}

		// Applies affine transformation
		internal void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			m_vertices_mp.ApplyTransformation(transform);
			if (m_segments != null)
			{
				for (int i = 0, n = m_segments.Count; i < n; i++)
				{
					if (m_segments[i] != null)
					{
						m_segments[i].ApplyTransformation(transform);
					}
				}
			}
		}

		internal void InterpolateAttributesForClosedPath_(int semantics, int path, int from_vertex, int to_vertex, double sub_length, int ordinate)
		{
			if (from_vertex == to_vertex)
			{
				return;
			}
			double from_attribute = GetAttributeAsDbl(semantics, from_vertex, ordinate);
			double to_attribute = GetAttributeAsDbl(semantics, to_vertex, ordinate);
			double cumulative_length = 0.0;
			double prev_interpolated_attribute = from_attribute;
			for (int vertex = from_vertex; vertex != to_vertex; vertex = GetNextVertex(vertex))
			{
				SetAttribute(semantics, vertex, ordinate, prev_interpolated_attribute);
				int vertex_index = GetVertexIndex(vertex);
				com.epl.geometry.Segment segment = GetSegmentFromIndex_(vertex_index);
				double segment_length;
				if (segment != null)
				{
					segment_length = segment.CalculateLength2D();
				}
				else
				{
					int next_vertex_index = GetVertexIndex(GetNextVertex(vertex));
					segment_length = m_vertices._getShortestDistance(vertex_index, next_vertex_index);
				}
				cumulative_length += segment_length;
				double t = cumulative_length / sub_length;
				prev_interpolated_attribute = com.epl.geometry.MathUtils.Lerp(from_attribute, to_attribute, t);
			}
			return;
		}

		internal void SetGeometryType_(int geom, int gt)
		{
			m_geometry_index_list.SetField(geom, 2, gt);
		}

		internal void SplitSegment_(int origin_vertex, com.epl.geometry.SegmentIntersector intersector, int intersector_index, bool b_forward)
		{
			if (b_forward)
			{
				SplitSegmentForward_(origin_vertex, intersector, intersector_index);
			}
			else
			{
				SplitSegmentBackward_(origin_vertex, intersector, intersector_index);
			}
		}

		internal void SetPrevVertex_(int vertex, int prev)
		{
			m_vertex_index_list.SetField(vertex, 1, prev);
		}

		internal void SetNextVertex_(int vertex, int next)
		{
			m_vertex_index_list.SetField(vertex, 2, next);
		}

		internal void SetPathToVertex_(int vertex, int path)
		{
			m_vertex_index_list.SetField(vertex, 3, path);
		}

		internal void SetPathSize_(int path, int size)
		{
			m_path_index_list.SetField(path, 3, size);
		}

		internal void SetFirstVertex_(int path, int first_vertex)
		{
			m_path_index_list.SetField(path, 4, first_vertex);
		}

		internal void SetLastVertex_(int path, int last_vertex)
		{
			m_path_index_list.SetField(path, 5, last_vertex);
		}

		internal void SetGeometryPathCount_(int geom, int path_count)
		{
			m_geometry_index_list.SetField(geom, 6, path_count);
		}

		internal void SetGeometryVertexCount_(int geom, int vertex_count)
		{
			m_geometry_index_list.SetField(geom, 5, vertex_count);
		}

		internal bool RingParentageCheckInternal_(int vertex_1, int vertex_2)
		{
			if (vertex_1 == vertex_2)
			{
				return true;
			}
			int vprev_1 = vertex_1;
			int vprev_2 = vertex_2;
			for (int v_1 = GetNextVertex(vertex_1), v_2 = GetNextVertex(vertex_2); v_1 != vertex_1 && v_2 != vertex_2; v_1 = GetNextVertex(v_1), v_2 = GetNextVertex(v_2))
			{
				if (v_1 == vertex_2)
				{
					return true;
				}
				if (v_2 == vertex_1)
				{
					return true;
				}
				System.Diagnostics.Debug.Assert((GetPrevVertex(v_1) == vprev_1));
				// detect malformed list
				System.Diagnostics.Debug.Assert((GetPrevVertex(v_2) == vprev_2));
				// detect malformed list
				vprev_1 = v_1;
				vprev_2 = v_2;
			}
			return false;
		}

		internal void ReverseRingInternal_(int vertex)
		{
			int v = vertex;
			do
			{
				int prev = GetPrevVertex(v);
				int next = GetNextVertex(v);
				SetNextVertex_(v, prev);
				SetPrevVertex_(v, next);
				v = next;
			}
			while (v != vertex);
		}

		// Path's last becomes invalid. Do not attempt to fix it here, because
		// this is not the intent of the method
		// Note: only last is invalid. other things sould not change.
		internal void SetTotalPointCount_(int count)
		{
			m_point_count = count;
		}

		internal void RemovePathOnly_(int path)
		{
			int prev = GetPrevPath(path);
			int next = GetNextPath(path);
			int geometry = GetGeometryFromPath(path);
			if (prev != -1)
			{
				SetNextPath_(prev, next);
			}
			else
			{
				SetFirstPath_(geometry, next);
			}
			if (next != -1)
			{
				SetPrevPath_(next, prev);
			}
			else
			{
				SetLastPath_(geometry, prev);
			}
			SetFirstVertex_(path, -1);
			SetLastVertex_(path, -1);
			FreePath_(path);
		}

		// void DbgVerifyIntegrity(int vertex);
		// void dbg_verify_vertex_counts();
		internal int RemoveVertexInternal_(int vertex, bool b_left_segment)
		{
			int prev = GetPrevVertex(vertex);
			int next = GetNextVertex(vertex);
			if (prev != -1)
			{
				SetNextVertex_(prev, next);
			}
			if (next != -1)
			{
				SetPrevVertex_(next, prev);
			}
			if (prev != -1 && next != -1)
			{
				int vindex_prev = GetVertexIndex(prev);
				int vindex_next = GetVertexIndex(next);
				if (b_left_segment)
				{
					com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex_prev);
					if (seg != null)
					{
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						m_vertices.GetXY(vindex_next, pt);
						seg.SetEndXY(pt);
					}
				}
				else
				{
					int vindex_erased = GetVertexIndex(vertex);
					com.epl.geometry.Segment seg = GetSegmentFromIndex_(vindex_erased);
					SetSegmentToIndex_(vindex_prev, seg);
					if (seg != null)
					{
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						m_vertices.GetXY(vindex_prev, pt);
						seg.SetStartXY(pt);
					}
				}
			}
			FreeVertex_(vertex);
			return next;
		}

		internal bool IsRingAreaValid_(int path)
		{
			return (GetPathFlags_(path) & com.epl.geometry.EditShape.PathFlags_.ringAreaValid) != 0;
		}

		// Sets exterior flag
		internal void SetRingAreaValid_(int path, bool b_yes_no)
		{
			int oldflags = GetPathFlags_(path);
			int flags = (oldflags | (int)com.epl.geometry.EditShape.PathFlags_.ringAreaValid) - (int)com.epl.geometry.EditShape.PathFlags_.ringAreaValid;
			// clear the bit;
			SetPathFlags_(path, flags | (b_yes_no ? (int)com.epl.geometry.EditShape.PathFlags_.ringAreaValid : 0));
		}

		internal int CompareVerticesSimpleY_(int v_1, int v_2)
		{
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			GetXY(v_1, pt_1);
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			GetXY(v_2, pt_2);
			int res = pt_1.Compare(pt_2);
			return res;
		}

		internal int CompareVerticesSimpleX_(int v_1, int v_2)
		{
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			GetXY(v_1, pt_1);
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			GetXY(v_2, pt_2);
			int res = pt_1.Compare(pt_2);
			return res;
		}

		public class SimplificatorVertexComparerY : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.EditShape parent;

			internal SimplificatorVertexComparerY(com.epl.geometry.EditShape parent_)
			{
				parent = parent_;
			}

			public override int Compare(int i_1, int i_2)
			{
				return parent.CompareVerticesSimpleY_(i_1, i_2);
			}
		}

		public class SimplificatorVertexComparerX : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.EditShape parent;

			internal SimplificatorVertexComparerX(com.epl.geometry.EditShape parent_)
			{
				parent = parent_;
			}

			public override int Compare(int i_1, int i_2)
			{
				return parent.CompareVerticesSimpleX_(i_1, i_2);
			}
		}

		// void sort_vertices_simple_by_y_heap_merge(Dynamic_array<int>& points,
		// const Dynamic_array<int>* geoms);
		internal void SortVerticesSimpleByY_(com.epl.geometry.AttributeStreamOfInt32 points, int begin_, int end_)
		{
			if (m_bucket_sort == null)
			{
				m_bucket_sort = new com.epl.geometry.BucketSort();
			}
			m_bucket_sort.Sort(points, begin_, end_, new com.epl.geometry.EditShape.EditShapeBucketSortHelper(this));
		}

		internal void SortVerticesSimpleByYHelper_(com.epl.geometry.AttributeStreamOfInt32 points, int begin_, int end_)
		{
			points.Sort(begin_, end_, new com.epl.geometry.EditShape.SimplificatorVertexComparerY(this));
		}

		internal void SortVerticesSimpleByX_(com.epl.geometry.AttributeStreamOfInt32 points, int begin_, int end_)
		{
			points.Sort(begin_, end_, new com.epl.geometry.EditShape.SimplificatorVertexComparerX(this));
		}

		// Approximate size of the structure in memory.
		// The estimated size can be very slightly less than the actual size.
		// int estimate_memory_size() const;
		internal bool HasPointFeatures()
		{
			for (int geometry = GetFirstGeometry(); geometry != -1; geometry = GetNextGeometry(geometry))
			{
				if (!com.epl.geometry.Geometry.IsMultiPath(GetGeometryType(geometry)))
				{
					return true;
				}
			}
			return false;
		}

		internal void SwapGeometry(int geom1, int geom2)
		{
			int first_path1 = GetFirstPath(geom1);
			int first_path2 = GetFirstPath(geom2);
			int last_path1 = GetLastPath(geom1);
			int last_path2 = GetLastPath(geom2);
			for (int path = GetFirstPath(geom1); path != -1; path = GetNextPath(path))
			{
				SetPathGeometry_(path, geom2);
			}
			for (int path_1 = GetFirstPath(geom2); path_1 != -1; path_1 = GetNextPath(path_1))
			{
				SetPathGeometry_(path_1, geom1);
			}
			SetFirstPath_(geom1, first_path2);
			SetFirstPath_(geom2, first_path1);
			SetLastPath_(geom1, last_path2);
			SetLastPath_(geom2, last_path1);
			int vc1 = GetPointCount(geom1);
			int pc1 = GetPathCount(geom1);
			int vc2 = GetPointCount(geom2);
			int pc2 = GetPathCount(geom2);
			SetGeometryVertexCount_(geom1, vc2);
			SetGeometryVertexCount_(geom2, vc1);
			SetGeometryPathCount_(geom1, pc2);
			SetGeometryPathCount_(geom2, pc1);
			int gt1 = m_geometry_index_list.GetField(geom1, 2);
			int gt2 = m_geometry_index_list.GetField(geom2, 2);
			m_geometry_index_list.SetField(geom1, 2, gt2);
			m_geometry_index_list.SetField(geom2, 2, gt1);
		}
	}
}
