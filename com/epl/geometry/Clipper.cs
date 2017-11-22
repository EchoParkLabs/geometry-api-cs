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
	internal class Clipper
	{
		internal com.epl.geometry.Envelope2D m_extent;

		internal com.epl.geometry.EditShape m_shape;

		internal int m_geometry;

		internal int m_vertices_on_extent_index;

		internal com.epl.geometry.AttributeStreamOfInt32 m_vertices_on_extent;

		internal virtual int CheckSegmentIntersection_(com.epl.geometry.Envelope2D seg_env, int side, double clip_value)
		{
			switch (side)
			{
				case 0:
				{
					if (seg_env.xmin < clip_value && seg_env.xmax <= clip_value)
					{
						return 0;
					}
					else
					{
						// outside (or on the border)
						if (seg_env.xmin >= clip_value)
						{
							return 1;
						}
						else
						{
							// inside
							return -1;
						}
					}
					goto case 1;
				}

				case 1:
				{
					// intersects
					if (seg_env.ymin < clip_value && seg_env.ymax <= clip_value)
					{
						return 0;
					}
					else
					{
						if (seg_env.ymin >= clip_value)
						{
							return 1;
						}
						else
						{
							return -1;
						}
					}
					goto case 2;
				}

				case 2:
				{
					if (seg_env.xmin >= clip_value && seg_env.xmax > clip_value)
					{
						return 0;
					}
					else
					{
						if (seg_env.xmax <= clip_value)
						{
							return 1;
						}
						else
						{
							return -1;
						}
					}
					goto case 3;
				}

				case 3:
				{
					if (seg_env.ymin >= clip_value && seg_env.ymax > clip_value)
					{
						return 0;
					}
					else
					{
						if (seg_env.ymax <= clip_value)
						{
							return 1;
						}
						else
						{
							return -1;
						}
					}
					break;
				}
			}
			System.Diagnostics.Debug.Assert((false));
			// cannot be here
			return 0;
		}

		internal virtual com.epl.geometry.MultiPath ClipMultiPath2_(com.epl.geometry.MultiPath multi_path_in, double tolerance, double densify_dist)
		{
			bool b_is_polygon = multi_path_in.GetType() == com.epl.geometry.Geometry.Type.Polygon;
			if (b_is_polygon)
			{
				return ClipPolygon2_((com.epl.geometry.Polygon)multi_path_in, tolerance, densify_dist);
			}
			else
			{
				return ClipPolyline_((com.epl.geometry.Polyline)multi_path_in, tolerance);
			}
		}

		internal virtual com.epl.geometry.MultiPath ClipPolygon2_(com.epl.geometry.Polygon polygon_in, double tolerance, double densify_dist)
		{
			// If extent is degenerate, return 0.
			if (m_extent.GetWidth() == 0 || m_extent.GetHeight() == 0)
			{
				return (com.epl.geometry.MultiPath)polygon_in.CreateInstance();
			}
			com.epl.geometry.Envelope2D orig_env2D = new com.epl.geometry.Envelope2D();
			polygon_in.QueryLooseEnvelope(orig_env2D);
			// m_shape = GCNEW Edit_shape();
			m_geometry = m_shape.AddGeometry(polygon_in);
			// Forward decl for java port
			com.epl.geometry.Envelope2D seg_env = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D sub_seg_env = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			double[] result_ordinates = new double[9];
			double[] parameters = new double[9];
			com.epl.geometry.SegmentBuffer sub_segment_buffer = new com.epl.geometry.SegmentBuffer();
			com.epl.geometry.Line line = new com.epl.geometry.Line();
			com.epl.geometry.AttributeStreamOfInt32 delete_candidates = new com.epl.geometry.AttributeStreamOfInt32(0);
			delete_candidates.Reserve(System.Math.Min(100, polygon_in.GetPointCount()));
			// clip the polygon successively by each plane
			bool b_all_outside = false;
			for (int iclip_plane = 0; !b_all_outside && iclip_plane < 4; iclip_plane++)
			{
				bool b_intersects_plane = false;
				bool b_axis_x = (iclip_plane & 1) != 0;
				double clip_value = 0;
				switch (iclip_plane)
				{
					case 0:
					{
						clip_value = m_extent.xmin;
						b_intersects_plane = orig_env2D.xmin <= clip_value && orig_env2D.xmax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value < orig_env2D.xmin));
						break;
					}

					case 1:
					{
						clip_value = m_extent.ymin;
						b_intersects_plane = orig_env2D.ymin <= clip_value && orig_env2D.ymax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value < orig_env2D.ymin));
						break;
					}

					case 2:
					{
						clip_value = m_extent.xmax;
						b_intersects_plane = orig_env2D.xmin <= clip_value && orig_env2D.xmax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value > orig_env2D.xmax));
						break;
					}

					case 3:
					{
						clip_value = m_extent.ymax;
						b_intersects_plane = orig_env2D.ymin <= clip_value && orig_env2D.ymax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value > orig_env2D.ymax));
						break;
					}
				}
				if (!b_intersects_plane)
				{
					continue;
				}
				// Optimize for common case when only few sides of the
				// clipper envelope intersect the geometry.
				b_all_outside = true;
				for (int path = m_shape.GetFirstPath(m_geometry); path != -1; )
				{
					int inside = -1;
					int firstinside = -1;
					int first = m_shape.GetFirstVertex(path);
					int vertex = first;
					do
					{
						com.epl.geometry.Segment segment = m_shape.GetSegment(vertex);
						if (segment == null)
						{
							segment = line;
							m_shape.GetXY(vertex, pt_1);
							segment.SetStartXY(pt_1);
							m_shape.GetXY(m_shape.GetNextVertex(vertex), pt_2);
							segment.SetEndXY(pt_2);
						}
						segment.QueryEnvelope2D(seg_env);
						int seg_plane_intersection_status = CheckSegmentIntersection_(seg_env, iclip_plane, clip_value);
						int split_count = 0;
						int next_vertex = -1;
						if (seg_plane_intersection_status == -1)
						{
							// intersects plane
							int count = segment.IntersectionWithAxis2D(b_axis_x, clip_value, result_ordinates, parameters);
							if (count > 0)
							{
								split_count = m_shape.SplitSegment(vertex, parameters, count);
							}
							else
							{
								System.Diagnostics.Debug.Assert((count == 0));
								// might be -1 when the segment
								// is almost parallel to the
								// clip lane. Just to see this
								// happens.
								split_count = 0;
							}
							// add +1 to ensure we check the original segment if no
							// split produced due to degeneracy.
							// Also +1 is necessary to check the last segment of the
							// split
							split_count += 1;
							// split_count will never be 0 after
							// this if-block.
							int split_vert = vertex;
							int next_split_vert = m_shape.GetNextVertex(split_vert);
							for (int i = 0; i < split_count; i++)
							{
								m_shape.GetXY(split_vert, pt_1);
								m_shape.GetXY(next_split_vert, pt_2);
								com.epl.geometry.Segment sub_seg = m_shape.GetSegment(split_vert);
								if (sub_seg == null)
								{
									sub_seg = line;
									sub_seg.SetStartXY(pt_1);
									sub_seg.SetEndXY(pt_2);
								}
								sub_seg.QueryEnvelope2D(sub_seg_env);
								int sub_segment_plane_intersection_status = CheckSegmentIntersection_(sub_seg_env, iclip_plane, clip_value);
								if (sub_segment_plane_intersection_status == -1)
								{
									// subsegment is intertsecting the plane. We
									// need to snap one of the endpoints to ensure
									// no intersection.
									// TODO: ensure this works for curves. For
									// curves we have to adjust the curve shape.
									if (!b_axis_x)
									{
										System.Diagnostics.Debug.Assert(((pt_1.x < clip_value && pt_2.x > clip_value) || (pt_1.x > clip_value && pt_2.x < clip_value)));
										double d_1 = System.Math.Abs(pt_1.x - clip_value);
										double d_2 = System.Math.Abs(pt_2.x - clip_value);
										if (d_1 < d_2)
										{
											pt_1.x = clip_value;
											m_shape.SetXY(split_vert, pt_1);
										}
										else
										{
											pt_2.x = clip_value;
											m_shape.SetXY(next_split_vert, pt_2);
										}
									}
									else
									{
										System.Diagnostics.Debug.Assert(((pt_1.y < clip_value && pt_2.y > clip_value) || (pt_1.y > clip_value && pt_2.y < clip_value)));
										double d_1 = System.Math.Abs(pt_1.y - clip_value);
										double d_2 = System.Math.Abs(pt_2.y - clip_value);
										if (d_1 < d_2)
										{
											pt_1.y = clip_value;
											m_shape.SetXY(split_vert, pt_1);
										}
										else
										{
											pt_2.y = clip_value;
											m_shape.SetXY(next_split_vert, pt_2);
										}
									}
									// after the endpoint has been adjusted, recheck
									// the segment.
									sub_seg = m_shape.GetSegment(split_vert);
									if (sub_seg == null)
									{
										sub_seg = line;
										sub_seg.SetStartXY(pt_1);
										sub_seg.SetEndXY(pt_2);
									}
									sub_seg.QueryEnvelope2D(sub_seg_env);
									sub_segment_plane_intersection_status = CheckSegmentIntersection_(sub_seg_env, iclip_plane, clip_value);
								}
								System.Diagnostics.Debug.Assert((sub_segment_plane_intersection_status != -1));
								int old_inside = inside;
								inside = sub_segment_plane_intersection_status;
								if (firstinside == -1)
								{
									firstinside = inside;
								}
								// add connections along the clipping plane line
								if (old_inside == 0 && inside == 1)
								{
								}
								else
								{
									// going from outside to inside. Do nothing
									if (old_inside == 1 && inside == 0)
									{
									}
									else
									{
										// going from inside to outside
										if (old_inside == 0 && inside == 0)
										{
											// staying outside
											// remember the start point of the outside
											// segment to be deleted.
											delete_candidates.Add(split_vert);
										}
									}
								}
								// is a
								// candidate
								// to be
								// deleted
								if (inside == 1)
								{
									b_all_outside = false;
								}
								split_vert = next_split_vert;
								next_vertex = split_vert;
								next_split_vert = m_shape.GetNextVertex(next_split_vert);
							}
						}
						if (split_count == 0)
						{
							System.Diagnostics.Debug.Assert((seg_plane_intersection_status != -1));
							// cannot
							// happen.
							int old_inside = inside;
							inside = seg_plane_intersection_status;
							if (firstinside == -1)
							{
								firstinside = inside;
							}
							if (old_inside == 0 && inside == 1)
							{
							}
							else
							{
								// going from outside to inside.
								if (old_inside == 1 && inside == 0)
								{
								}
								else
								{
									// going from inside to outside
									if (old_inside == 0 && inside == 0)
									{
										// remember the start point of the outside segment
										// to be deleted.
										delete_candidates.Add(vertex);
									}
								}
							}
							// is a candidate to
							// be deleted
							if (inside == 1)
							{
								b_all_outside = false;
							}
							next_vertex = m_shape.GetNextVertex(vertex);
						}
						vertex = next_vertex;
					}
					while (vertex != first);
					if (firstinside == 0 && inside == 0)
					{
						// first vertex need to be
						// deleted.
						delete_candidates.Add(first);
					}
					// is a candidate to be
					// deleted
					for (int i_1 = 0, n = delete_candidates.Size(); i_1 < n; i_1++)
					{
						int delete_vert = delete_candidates.Get(i_1);
						m_shape.RemoveVertex(delete_vert, false);
					}
					delete_candidates.Clear(false);
					if (m_shape.GetPathSize(path) < 3)
					{
						path = m_shape.RemovePath(path);
					}
					else
					{
						path = m_shape.GetNextPath(path);
					}
				}
			}
			if (b_all_outside)
			{
				return (com.epl.geometry.MultiPath)polygon_in.CreateInstance();
			}
			// After the clipping, we could have produced unwanted segment overlaps
			// along the clipping envelope boundary.
			// Detect and resolve that case if possible.
			ResolveBoundaryOverlaps_();
			if (densify_dist > 0)
			{
				DensifyAlongClipExtent_(densify_dist);
			}
			return (com.epl.geometry.MultiPath)m_shape.GetGeometry(m_geometry);
		}

		internal virtual com.epl.geometry.MultiPath ClipPolyline_(com.epl.geometry.Polyline polyline_in, double tolerance)
		{
			// Forward decl for java port
			com.epl.geometry.Envelope2D seg_env = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D sub_seg_env = new com.epl.geometry.Envelope2D();
			double[] result_ordinates = new double[9];
			double[] parameters = new double[9];
			com.epl.geometry.SegmentBuffer sub_segment_buffer = new com.epl.geometry.SegmentBuffer();
			com.epl.geometry.MultiPath result_poly = polyline_in;
			com.epl.geometry.Envelope2D orig_env2D = new com.epl.geometry.Envelope2D();
			polyline_in.QueryLooseEnvelope(orig_env2D);
			for (int iclip_plane = 0; iclip_plane < 4; iclip_plane++)
			{
				bool b_intersects_plane = false;
				bool b_axis_x = (iclip_plane & 1) != 0;
				double clip_value = 0;
				switch (iclip_plane)
				{
					case 0:
					{
						clip_value = m_extent.xmin;
						b_intersects_plane = orig_env2D.xmin <= clip_value && orig_env2D.xmax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value < orig_env2D.xmin));
						break;
					}

					case 1:
					{
						clip_value = m_extent.ymin;
						b_intersects_plane = orig_env2D.ymin <= clip_value && orig_env2D.ymax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value < orig_env2D.ymin));
						break;
					}

					case 2:
					{
						clip_value = m_extent.xmax;
						b_intersects_plane = orig_env2D.xmin <= clip_value && orig_env2D.xmax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value > orig_env2D.xmax));
						break;
					}

					case 3:
					{
						clip_value = m_extent.ymax;
						b_intersects_plane = orig_env2D.ymin <= clip_value && orig_env2D.ymax >= clip_value;
						System.Diagnostics.Debug.Assert((b_intersects_plane || clip_value > orig_env2D.ymax));
						break;
					}
				}
				if (!b_intersects_plane)
				{
					continue;
				}
				// Optimize for common case when only few sides of the
				// clipper envelope intersect the geometry.
				com.epl.geometry.MultiPath src_poly = result_poly;
				result_poly = (com.epl.geometry.MultiPath)polyline_in.CreateInstance();
				com.epl.geometry.MultiPathImpl mp_impl_src = (com.epl.geometry.MultiPathImpl)src_poly._getImpl();
				com.epl.geometry.SegmentIteratorImpl seg_iter = mp_impl_src.QuerySegmentIterator();
				seg_iter.ResetToFirstPath();
				com.epl.geometry.Point2D pt_prev;
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				while (seg_iter.NextPath())
				{
					int inside = -1;
					bool b_start_new_path = true;
					while (seg_iter.HasNextSegment())
					{
						com.epl.geometry.Segment segment = seg_iter.NextSegment();
						segment.QueryEnvelope2D(seg_env);
						int seg_plane_intersection_status = CheckSegmentIntersection_(seg_env, iclip_plane, clip_value);
						if (seg_plane_intersection_status == -1)
						{
							// intersects plane
							int count = segment.IntersectionWithAxis2D(b_axis_x, clip_value, result_ordinates, parameters);
							if (count > 0)
							{
								double t0 = 0.0;
								pt_prev = segment.GetStartXY();
								for (int i = 0; i <= count; i++)
								{
									double t = i < count ? parameters[i] : 1.0;
									if (t0 == t)
									{
										continue;
									}
									segment.Cut(t0, t, sub_segment_buffer);
									com.epl.geometry.Segment sub_seg = sub_segment_buffer.Get();
									sub_seg.SetStartXY(pt_prev);
									if (i < count)
									{
										// snap to plane
										if (b_axis_x)
										{
											pt.x = result_ordinates[i];
											pt.y = clip_value;
										}
										else
										{
											pt.x = clip_value;
											pt.y = result_ordinates[i];
										}
										sub_seg.SetEndXY(pt);
									}
									sub_seg.QueryEnvelope2D(sub_seg_env);
									int sub_segment_plane_intersection_status = CheckSegmentIntersection_(sub_seg_env, iclip_plane, clip_value);
									if (sub_segment_plane_intersection_status == -1)
									{
										// subsegment is intertsecting the plane. We
										// need to snap one of the endpoints to
										// ensure no intersection.
										// TODO: ensure this works for curves. For
										// curves we have to adjust the curve shape.
										com.epl.geometry.Point2D pt_1 = sub_seg.GetStartXY();
										com.epl.geometry.Point2D pt_2 = sub_seg.GetEndXY();
										if (!b_axis_x)
										{
											System.Diagnostics.Debug.Assert(((pt_1.x < clip_value && pt_2.x > clip_value) || (pt_1.x > clip_value && pt_2.x < clip_value)));
											double d_1 = System.Math.Abs(pt_1.x - clip_value);
											double d_2 = System.Math.Abs(pt_2.x - clip_value);
											if (d_1 < d_2)
											{
												pt_1.x = clip_value;
												sub_seg.SetStartXY(pt_1);
											}
											else
											{
												pt_2.x = clip_value;
												sub_seg.SetEndXY(pt_2);
											}
										}
										else
										{
											System.Diagnostics.Debug.Assert(((pt_1.y < clip_value && pt_2.y > clip_value) || (pt_1.y > clip_value && pt_2.y < clip_value)));
											double d_1 = System.Math.Abs(pt_1.y - clip_value);
											double d_2 = System.Math.Abs(pt_2.y - clip_value);
											if (d_1 < d_2)
											{
												pt_1.y = clip_value;
												sub_seg.SetStartXY(pt_1);
											}
											else
											{
												pt_2.y = clip_value;
												sub_seg.SetEndXY(pt_2);
											}
										}
										// after the endpoint has been adjusted,
										// recheck the segment.
										sub_seg.QueryEnvelope2D(sub_seg_env);
										sub_segment_plane_intersection_status = CheckSegmentIntersection_(sub_seg_env, iclip_plane, clip_value);
									}
									System.Diagnostics.Debug.Assert((sub_segment_plane_intersection_status != -1));
									pt_prev = sub_seg.GetEndXY();
									t0 = t;
									inside = sub_segment_plane_intersection_status;
									if (inside == 1)
									{
										result_poly.AddSegment(sub_seg, b_start_new_path);
										b_start_new_path = false;
									}
									else
									{
										b_start_new_path = true;
									}
								}
							}
						}
						else
						{
							inside = seg_plane_intersection_status;
							if (inside == 1)
							{
								result_poly.AddSegment(segment, b_start_new_path);
								b_start_new_path = false;
							}
							else
							{
								b_start_new_path = true;
							}
						}
					}
				}
			}
			return result_poly;
		}

		internal virtual void ResolveBoundaryOverlaps_()
		{
			m_vertices_on_extent_index = -1;
			SplitSegments_(false, m_extent.xmin);
			SplitSegments_(false, m_extent.xmax);
			SplitSegments_(true, m_extent.ymin);
			SplitSegments_(true, m_extent.ymax);
			m_vertices_on_extent.Resize(0);
			m_vertices_on_extent.Reserve(100);
			m_vertices_on_extent_index = m_shape.CreateUserIndex();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; path = m_shape.GetNextPath(path))
			{
				int vertex = m_shape.GetFirstVertex(path);
				for (int ivert = 0, nvert = m_shape.GetPathSize(path); ivert < nvert; ivert++, vertex = m_shape.GetNextVertex(vertex))
				{
					m_shape.GetXY(vertex, pt);
					if (m_extent.xmin == pt.x || m_extent.xmax == pt.x || m_extent.ymin == pt.y || m_extent.ymax == pt.y)
					{
						m_shape.SetUserIndex(vertex, m_vertices_on_extent_index, m_vertices_on_extent.Size());
						m_vertices_on_extent.Add(vertex);
					}
				}
			}
			// dbg_check_path_first_();
			ResolveOverlaps_(false, m_extent.xmin);
			// dbg_check_path_first_();
			ResolveOverlaps_(false, m_extent.xmax);
			// dbg_check_path_first_();
			ResolveOverlaps_(true, m_extent.ymin);
			// dbg_check_path_first_();
			ResolveOverlaps_(true, m_extent.ymax);
			FixPaths_();
		}

		internal virtual void DensifyAlongClipExtent_(double densify_dist)
		{
			System.Diagnostics.Debug.Assert((densify_dist > 0));
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			double[] split_scalars = new double[2048];
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; path = m_shape.GetNextPath(path))
			{
				int first_vertex = m_shape.GetFirstVertex(path);
				int vertex = first_vertex;
				do
				{
					int next_vertex = m_shape.GetNextVertex(vertex);
					m_shape.GetXY(vertex, pt_1);
					int b_densify_x = -1;
					if (pt_1.x == m_extent.xmin)
					{
						m_shape.GetXY(next_vertex, pt_2);
						if (pt_2.x == m_extent.xmin)
						{
							b_densify_x = 1;
						}
					}
					else
					{
						if (pt_1.x == m_extent.xmax)
						{
							m_shape.GetXY(next_vertex, pt_2);
							if (pt_2.x == m_extent.xmax)
							{
								b_densify_x = 1;
							}
						}
					}
					if (pt_1.y == m_extent.ymin)
					{
						m_shape.GetXY(next_vertex, pt_2);
						if (pt_2.y == m_extent.ymin)
						{
							b_densify_x = 0;
						}
					}
					else
					{
						if (pt_1.y == m_extent.ymax)
						{
							m_shape.GetXY(next_vertex, pt_2);
							if (pt_2.y == m_extent.ymax)
							{
								b_densify_x = 0;
							}
						}
					}
					if (b_densify_x == -1)
					{
						vertex = next_vertex;
						continue;
					}
					double len = com.epl.geometry.Point2D.Distance(pt_1, pt_2);
					int num = (int)System.Math.Min(System.Math.Ceiling(len / densify_dist), 2048.0);
					if (num <= 1)
					{
						vertex = next_vertex;
						continue;
					}
					for (int i = 1; i < num; i++)
					{
						split_scalars[i - 1] = (1.0 * i) / num;
					}
					int actual_splits = m_shape.SplitSegment(vertex, split_scalars, num - 1);
					System.Diagnostics.Debug.Assert((actual_splits == num - 1));
					vertex = next_vertex;
				}
				while (vertex != first_vertex);
			}
		}

		internal virtual void SplitSegments_(bool b_axis_x, double clip_value)
		{
			// After the clipping, we could have produced unwanted segment overlaps
			// along the clipping envelope boundary.
			// Detect and resolve that case if possible.
			int usage_index = m_shape.CreateUserIndex();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			com.epl.geometry.AttributeStreamOfInt32 sorted_vertices = new com.epl.geometry.AttributeStreamOfInt32(0);
			sorted_vertices.Reserve(100);
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; path = m_shape.GetNextPath(path))
			{
				int vertex = m_shape.GetFirstVertex(path);
				for (int ivert = 0, nvert = m_shape.GetPathSize(path); ivert < nvert; ivert++)
				{
					int next_vertex = m_shape.GetNextVertex(vertex);
					m_shape.GetXY(vertex, pt);
					if (b_axis_x ? pt.y == clip_value : pt.x == clip_value)
					{
						m_shape.GetXY(next_vertex, pt);
						if (b_axis_x ? pt.y == clip_value : pt.x == clip_value)
						{
							if (m_shape.GetUserIndex(vertex, usage_index) != 1)
							{
								sorted_vertices.Add(vertex);
								m_shape.SetUserIndex(vertex, usage_index, 1);
							}
							if (m_shape.GetUserIndex(next_vertex, usage_index) != 1)
							{
								sorted_vertices.Add(next_vertex);
								m_shape.SetUserIndex(next_vertex, usage_index, 1);
							}
						}
					}
					vertex = next_vertex;
				}
			}
			m_shape.RemoveUserIndex(usage_index);
			if (sorted_vertices.Size() < 3)
			{
				return;
			}
			sorted_vertices.Sort(0, sorted_vertices.Size(), new com.epl.geometry.Clipper.ClipperVertexComparer(this));
			com.epl.geometry.Point2D pt_tmp = new com.epl.geometry.Point2D();
			// forward declare for java port
			// optimization
			com.epl.geometry.Point2D pt_0 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			pt_0.SetNaN();
			int index_0 = -1;
			com.epl.geometry.AttributeStreamOfInt32 active_intervals = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 new_active_intervals = new com.epl.geometry.AttributeStreamOfInt32(0);
			int node1 = m_shape.CreateUserIndex();
			int node2 = m_shape.CreateUserIndex();
			for (int index = 0, n = sorted_vertices.Size(); index < n; index++)
			{
				int vert = sorted_vertices.Get(index);
				m_shape.GetXY(vert, pt);
				if (!pt.IsEqual(pt_0))
				{
					if (index_0 == -1)
					{
						index_0 = index;
						pt_0.SetCoords(pt);
						continue;
					}
					// add new intervals, that started at pt_0
					for (int i = index_0; i < index; i++)
					{
						int v = sorted_vertices.Get(i);
						int nextv = m_shape.GetNextVertex(v);
						int prevv = m_shape.GetPrevVertex(v);
						bool bAdded = false;
						if (CompareVertices_(v, nextv) < 0)
						{
							m_shape.GetXY(nextv, pt_tmp);
							if (b_axis_x ? pt_tmp.y == clip_value : pt_tmp.x == clip_value)
							{
								active_intervals.Add(v);
								bAdded = true;
								m_shape.SetUserIndex(v, node2, 1);
							}
						}
						if (CompareVertices_(v, prevv) < 0)
						{
							m_shape.GetXY(prevv, pt_tmp);
							if (b_axis_x ? pt_tmp.y == clip_value : pt_tmp.x == clip_value)
							{
								if (!bAdded)
								{
									active_intervals.Add(v);
								}
								m_shape.SetUserIndex(v, node1, 1);
							}
						}
					}
					// Split all active intervals at new point
					for (int ia = 0, na = active_intervals.Size(); ia < na; ia++)
					{
						int v = active_intervals.Get(ia);
						int n_1 = m_shape.GetUserIndex(v, node1);
						int n_2 = m_shape.GetUserIndex(v, node2);
						if (n_1 == 1)
						{
							int prevv = m_shape.GetPrevVertex(v);
							m_shape.GetXY(prevv, pt_1);
							double[] t = new double[1];
							t[0] = 0;
							if (!pt_1.IsEqual(pt))
							{
								// Split the active segment
								double active_segment_length = com.epl.geometry.Point2D.Distance(pt_0, pt_1);
								t[0] = com.epl.geometry.Point2D.Distance(pt_1, pt) / active_segment_length;
								System.Diagnostics.Debug.Assert((t[0] >= 0 && t[0] <= 1.0));
								if (t[0] == 0)
								{
									t[0] = com.epl.geometry.NumberUtils.DoubleEps();
								}
								else
								{
									// some
									// roundoff
									// issue.
									// split
									// anyway.
									if (t[0] == 1.0)
									{
										t[0] = 1.0 - com.epl.geometry.NumberUtils.DoubleEps();
										// some
										// roundoff
										// issue.
										// split
										// anyway.
										System.Diagnostics.Debug.Assert((t[0] != 1.0));
									}
								}
								int split_count = m_shape.SplitSegment(prevv, t, 1);
								System.Diagnostics.Debug.Assert((split_count > 0));
								int v_1 = m_shape.GetPrevVertex(v);
								m_shape.SetXY(v_1, pt);
								new_active_intervals.Add(v_1);
								m_shape.SetUserIndex(v_1, node1, 1);
								m_shape.SetUserIndex(v_1, node2, -1);
							}
						}
						// The active segment ends at the current point.
						// We skip it, and it goes away.
						if (n_2 == 1)
						{
							int nextv = m_shape.GetNextVertex(v);
							m_shape.GetXY(nextv, pt_1);
							double[] t = new double[1];
							t[0] = 0;
							if (!pt_1.IsEqual(pt))
							{
								double active_segment_length = com.epl.geometry.Point2D.Distance(pt_0, pt_1);
								t[0] = com.epl.geometry.Point2D.Distance(pt_0, pt) / active_segment_length;
								System.Diagnostics.Debug.Assert((t[0] >= 0 && t[0] <= 1.0));
								if (t[0] == 0)
								{
									t[0] = com.epl.geometry.NumberUtils.DoubleEps();
								}
								else
								{
									// some
									// roundoff
									// issue.
									// split
									// anyway.
									if (t[0] == 1.0)
									{
										t[0] = 1.0 - com.epl.geometry.NumberUtils.DoubleEps();
										// some
										// roundoff
										// issue.
										// split
										// anyway.
										System.Diagnostics.Debug.Assert((t[0] != 1.0));
									}
								}
								int split_count = m_shape.SplitSegment(v, t, 1);
								System.Diagnostics.Debug.Assert((split_count > 0));
								int v_1 = m_shape.GetNextVertex(v);
								m_shape.SetXY(v_1, pt);
								new_active_intervals.Add(v_1);
								m_shape.SetUserIndex(v_1, node1, -1);
								m_shape.SetUserIndex(v_1, node2, 1);
							}
						}
					}
					com.epl.geometry.AttributeStreamOfInt32 tmp = active_intervals;
					active_intervals = new_active_intervals;
					new_active_intervals = tmp;
					new_active_intervals.Clear(false);
					index_0 = index;
					pt_0.SetCoords(pt);
				}
			}
			m_shape.RemoveUserIndex(node1);
			m_shape.RemoveUserIndex(node2);
		}

		internal virtual void ResolveOverlaps_(bool b_axis_x, double clip_value)
		{
			// Along the envelope boundary there could be overlapped segments.
			// Example, exterior ring with a hole is cut with a line, that
			// passes through the center of the hole.
			// Detect pairs of opposite overlapping segments and get rid of them
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			com.epl.geometry.AttributeStreamOfInt32 sorted_vertices = new com.epl.geometry.AttributeStreamOfInt32(0);
			sorted_vertices.Reserve(100);
			int sorted_index = m_shape.CreateUserIndex();
			// DEBUGPRINTF(L"ee\n");
			for (int ivert = 0, nvert = m_vertices_on_extent.Size(); ivert < nvert; ivert++)
			{
				int vertex = m_vertices_on_extent.Get(ivert);
				if (vertex == -1)
				{
					continue;
				}
				int next_vertex = m_shape.GetNextVertex(vertex);
				m_shape.GetXY(vertex, pt);
				// DEBUGPRINTF(L"%f\t%f\n", pt.x, pt.y);
				if (b_axis_x ? pt.y == clip_value : pt.x == clip_value)
				{
					m_shape.GetXY(next_vertex, pt);
					if (b_axis_x ? pt.y == clip_value : pt.x == clip_value)
					{
						System.Diagnostics.Debug.Assert((m_shape.GetUserIndex(next_vertex, m_vertices_on_extent_index) != -1));
						if (m_shape.GetUserIndex(vertex, sorted_index) != -2)
						{
							sorted_vertices.Add(vertex);
							// remember the vertex. The
							// attached segment belongs
							// to the given clip plane.
							m_shape.SetUserIndex(vertex, sorted_index, -2);
						}
						if (m_shape.GetUserIndex(next_vertex, sorted_index) != -2)
						{
							sorted_vertices.Add(next_vertex);
							m_shape.SetUserIndex(next_vertex, sorted_index, -2);
						}
					}
				}
			}
			if (sorted_vertices.Size() == 0)
			{
				m_shape.RemoveUserIndex(sorted_index);
				return;
			}
			sorted_vertices.Sort(0, sorted_vertices.Size(), new com.epl.geometry.Clipper.ClipperVertexComparer(this));
			// std::sort(sorted_vertices.get_ptr(), sorted_vertices.get_ptr() +
			// sorted_vertices.size(), Clipper_vertex_comparer(this));
			// DEBUGPRINTF(L"**\n");
			for (int index = 0, n = sorted_vertices.Size(); index < n; index++)
			{
				int vert = sorted_vertices.Get(index);
				m_shape.SetUserIndex(vert, sorted_index, index);
			}
			// Point_2D pt;
			// m_shape.get_xy(vert, pt);
			// DEBUGPRINTF(L"%f\t%f\t%d\n", pt.x, pt.y, vert);
			com.epl.geometry.Point2D pt_tmp = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_0 = new com.epl.geometry.Point2D();
			pt_0.SetNaN();
			int index_0 = -1;
			for (int index_1 = 0, n = sorted_vertices.Size(); index_1 < n; index_1++)
			{
				int vert = sorted_vertices.Get(index_1);
				if (vert == -1)
				{
					continue;
				}
				m_shape.GetXY(vert, pt);
				if (!pt.IsEqual(pt_0))
				{
					if (index_0 != -1)
					{
						while (true)
						{
							bool b_overlap_resolved = false;
							int index_to = index_1 - index_0 > 1 ? index_1 - 1 : index_1;
							for (int i = index_0; i < index_to; i++)
							{
								int v = sorted_vertices.Get(i);
								if (v == -1)
								{
									continue;
								}
								int nextv = -1;
								int nv = m_shape.GetNextVertex(v);
								if (CompareVertices_(v, nv) < 0)
								{
									m_shape.GetXY(nv, pt_tmp);
									if (b_axis_x ? pt_tmp.y == clip_value : pt_tmp.x == clip_value)
									{
										nextv = nv;
									}
								}
								int prevv = -1;
								int pv = m_shape.GetPrevVertex(v);
								if (CompareVertices_(v, pv) < 0)
								{
									m_shape.GetXY(pv, pt_tmp);
									if (b_axis_x ? pt_tmp.y == clip_value : pt_tmp.x == clip_value)
									{
										prevv = pv;
									}
								}
								if (nextv != -1 && prevv != -1)
								{
									// we have a cusp here. remove the vertex.
									BeforeRemoveVertex_(v, sorted_vertices, sorted_index);
									m_shape.RemoveVertex(v, false);
									BeforeRemoveVertex_(nextv, sorted_vertices, sorted_index);
									m_shape.RemoveVertex(nextv, false);
									b_overlap_resolved = true;
									continue;
								}
								if (nextv == -1 && prevv == -1)
								{
									continue;
								}
								for (int j = i + 1; j < index_1; j++)
								{
									int v_1 = sorted_vertices.Get(j);
									if (v_1 == -1)
									{
										continue;
									}
									int nv1 = m_shape.GetNextVertex(v_1);
									int nextv1 = -1;
									if (CompareVertices_(v_1, nv1) < 0)
									{
										m_shape.GetXY(nv1, pt_tmp);
										if (b_axis_x ? pt_tmp.y == clip_value : pt_tmp.x == clip_value)
										{
											nextv1 = nv1;
										}
									}
									int pv1 = m_shape.GetPrevVertex(v_1);
									int prevv_1 = -1;
									if (CompareVertices_(v_1, pv1) < 0)
									{
										m_shape.GetXY(pv1, pt_tmp);
										if (b_axis_x ? pt_tmp.y == clip_value : pt_tmp.x == clip_value)
										{
											prevv_1 = pv1;
										}
									}
									if (nextv1 != -1 && prevv_1 != -1)
									{
										// we have a cusp here. remove the vertex.
										BeforeRemoveVertex_(v_1, sorted_vertices, sorted_index);
										m_shape.RemoveVertex(v_1, false);
										BeforeRemoveVertex_(nextv1, sorted_vertices, sorted_index);
										m_shape.RemoveVertex(nextv1, false);
										b_overlap_resolved = true;
										break;
									}
									if (nextv != -1 && prevv_1 != -1)
									{
										RemoveOverlap_(sorted_vertices, v, nextv, v_1, prevv_1, sorted_index);
										b_overlap_resolved = true;
										break;
									}
									else
									{
										if (prevv != -1 && nextv1 != -1)
										{
											RemoveOverlap_(sorted_vertices, v_1, nextv1, v, prevv, sorted_index);
											b_overlap_resolved = true;
											break;
										}
									}
								}
								if (b_overlap_resolved)
								{
									break;
								}
							}
							if (!b_overlap_resolved)
							{
								break;
							}
						}
					}
					index_0 = index_1;
					pt_0.SetCoords(pt);
				}
			}
			m_shape.RemoveUserIndex(sorted_index);
		}

		internal virtual void BeforeRemoveVertex_(int v_1, com.epl.geometry.AttributeStreamOfInt32 sorted_vertices, int sorted_index)
		{
			int ind = m_shape.GetUserIndex(v_1, sorted_index);
			sorted_vertices.Set(ind, -1);
			ind = m_shape.GetUserIndex(v_1, m_vertices_on_extent_index);
			m_vertices_on_extent.Set(ind, -1);
			int path = m_shape.GetPathFromVertex(v_1);
			if (path != -1)
			{
				int first = m_shape.GetFirstVertex(path);
				if (first == v_1)
				{
					m_shape.SetFirstVertex_(path, -1);
					m_shape.SetLastVertex_(path, -1);
				}
			}
		}

		internal virtual void RemoveOverlap_(com.epl.geometry.AttributeStreamOfInt32 sorted_vertices, int v, int nextv, int v_1, int prevv_1, int sorted_index)
		{
			System.Diagnostics.Debug.Assert((m_shape.IsEqualXY(v, v_1)));
			System.Diagnostics.Debug.Assert((m_shape.IsEqualXY(nextv, prevv_1)));
			System.Diagnostics.Debug.Assert((m_shape.GetNextVertex(v) == nextv));
			System.Diagnostics.Debug.Assert((m_shape.GetNextVertex(prevv_1) == v_1));
			m_shape.SetNextVertex_(v, v_1);
			m_shape.SetPrevVertex_(v_1, v);
			m_shape.SetPrevVertex_(nextv, prevv_1);
			m_shape.SetNextVertex_(prevv_1, nextv);
			BeforeRemoveVertex_(v_1, sorted_vertices, sorted_index);
			m_shape.RemoveVertexInternal_(v_1, false);
			BeforeRemoveVertex_(prevv_1, sorted_vertices, sorted_index);
			m_shape.RemoveVertexInternal_(prevv_1, true);
		}

		internal virtual void FixPaths_()
		{
			for (int ivert = 0, nvert = m_vertices_on_extent.Size(); ivert < nvert; ivert++)
			{
				int vertex = m_vertices_on_extent.Get(ivert);
				if (vertex != -1)
				{
					m_shape.SetPathToVertex_(vertex, -1);
				}
			}
			int path_count = 0;
			int geometry_size = 0;
			for (int path = m_shape.GetFirstPath(m_geometry); path != -1; )
			{
				int first = m_shape.GetFirstVertex(path);
				if (first == -1 || path != m_shape.GetPathFromVertex(first))
				{
					// The
					// path's
					// first
					// vertex
					// has
					// been
					// deleted.
					// Or
					// the
					// path
					// first
					// vertex
					// is
					// now
					// part
					// of
					// another
					// path.
					// We
					// have
					// to
					// delete
					// such
					// path
					// object.
					int p = path;
					path = m_shape.GetNextPath(path);
					m_shape.SetFirstVertex_(p, -1);
					m_shape.RemovePathOnly_(p);
					continue;
				}
				System.Diagnostics.Debug.Assert((path == m_shape.GetPathFromVertex(first)));
				int vertex = first;
				int path_size = 0;
				do
				{
					m_shape.SetPathToVertex_(vertex, path);
					path_size++;
					vertex = m_shape.GetNextVertex(vertex);
				}
				while (vertex != first);
				if (path_size <= 2)
				{
					int ind = m_shape.GetUserIndex(first, m_vertices_on_extent_index);
					m_vertices_on_extent.Set(ind, -1);
					int nv = m_shape.RemoveVertex(first, false);
					if (path_size == 2)
					{
						ind = m_shape.GetUserIndex(nv, m_vertices_on_extent_index);
						m_vertices_on_extent.Set(ind, -1);
						m_shape.RemoveVertex(nv, false);
					}
					int p = path;
					path = m_shape.GetNextPath(path);
					m_shape.SetFirstVertex_(p, -1);
					m_shape.RemovePathOnly_(p);
					continue;
				}
				m_shape.SetRingAreaValid_(path, false);
				m_shape.SetLastVertex_(path, m_shape.GetPrevVertex(first));
				m_shape.SetPathSize_(path, path_size);
				geometry_size += path_size;
				path_count++;
				path = m_shape.GetNextPath(path);
			}
			for (int ivert_1 = 0, nvert = m_vertices_on_extent.Size(); ivert_1 < nvert; ivert_1++)
			{
				int vertex = m_vertices_on_extent.Get(ivert_1);
				if (vertex == -1)
				{
					continue;
				}
				int path_1 = m_shape.GetPathFromVertex(vertex);
				if (path_1 != -1)
				{
					continue;
				}
				path_1 = m_shape.InsertPath(m_geometry, -1);
				int path_size = 0;
				int first = vertex;
				do
				{
					m_shape.SetPathToVertex_(vertex, path_1);
					path_size++;
					vertex = m_shape.GetNextVertex(vertex);
				}
				while (vertex != first);
				if (path_size <= 2)
				{
					int ind = m_shape.GetUserIndex(first, m_vertices_on_extent_index);
					m_vertices_on_extent.Set(ind, -1);
					int nv = m_shape.RemoveVertex(first, false);
					if (path_size == 2)
					{
						ind = m_shape.GetUserIndex(nv, m_vertices_on_extent_index);
						if (ind >= 0)
						{
							m_vertices_on_extent.Set(ind, -1);
						}
						// this vertex is not on the extent.
						m_shape.RemoveVertex(nv, false);
					}
					int p = path_1;
					path_1 = m_shape.GetNextPath(path_1);
					m_shape.SetFirstVertex_(p, -1);
					m_shape.RemovePathOnly_(p);
					continue;
				}
				m_shape.SetClosedPath(path_1, true);
				m_shape.SetPathSize_(path_1, path_size);
				m_shape.SetFirstVertex_(path_1, first);
				m_shape.SetLastVertex_(path_1, m_shape.GetPrevVertex(first));
				m_shape.SetRingAreaValid_(path_1, false);
				geometry_size += path_size;
				path_count++;
			}
			m_shape.SetGeometryPathCount_(m_geometry, path_count);
			m_shape.SetGeometryVertexCount_(m_geometry, geometry_size);
			int total_point_count = 0;
			for (int geometry = m_shape.GetFirstGeometry(); geometry != -1; geometry = m_shape.GetNextGeometry(geometry))
			{
				total_point_count += m_shape.GetPointCount(geometry);
			}
			m_shape.SetTotalPointCount_(total_point_count);
		}

		internal static com.epl.geometry.Geometry ClipMultiPath_(com.epl.geometry.MultiPath multipath, com.epl.geometry.Envelope2D extent, double tolerance, double densify_dist)
		{
			com.epl.geometry.Clipper clipper = new com.epl.geometry.Clipper(extent);
			return clipper.ClipMultiPath2_(multipath, tolerance, densify_dist);
		}

		internal Clipper(com.epl.geometry.Envelope2D extent)
		{
			m_extent = extent;
			m_shape = new com.epl.geometry.EditShape();
			m_vertices_on_extent = new com.epl.geometry.AttributeStreamOfInt32(0);
		}

		// static std::shared_ptr<Polygon> create_polygon_from_polyline(const
		// std::shared_ptr<Multi_path>& polyline, const Envelope_2D& env_2D, bool
		// add_envelope, double tolerance, double densify_dist, int
		// corner_is_inside);
		internal static com.epl.geometry.Geometry Clip(com.epl.geometry.Geometry geometry, com.epl.geometry.Envelope2D extent, double tolerance, double densify_dist)
		{
			if (geometry.IsEmpty())
			{
				return geometry;
			}
			if (extent.IsEmpty())
			{
				return geometry.CreateInstance();
			}
			// return an empty geometry
			int geomtype = geometry.GetType().Value();
			// Test firstly the simplest geometry types point and envelope.
			// After that we'll check the envelope intersection for the optimization
			if (geomtype == com.epl.geometry.Geometry.Type.Point.Value())
			{
				com.epl.geometry.Point2D pt = ((com.epl.geometry.Point)geometry).GetXY();
				if (extent.Contains(pt))
				{
					return geometry;
				}
				else
				{
					return geometry.CreateInstance();
				}
			}
			else
			{
				// return an empty geometry
				if (geomtype == com.epl.geometry.Geometry.Type.Envelope.Value())
				{
					com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
					geometry.QueryEnvelope2D(env);
					if (env.Intersect(extent))
					{
						com.epl.geometry.Envelope result_env = new com.epl.geometry.Envelope();
						geometry.CopyTo(result_env);
						result_env.SetEnvelope2D(env);
						return result_env;
					}
					else
					{
						return geometry.CreateInstance();
					}
				}
			}
			// return an empty geometry
			// Test the geometry envelope
			com.epl.geometry.Envelope2D env_2D = new com.epl.geometry.Envelope2D();
			geometry.QueryLooseEnvelope2D(env_2D);
			if (extent.Contains(env_2D))
			{
				return geometry;
			}
			// completely inside of bounds
			if (!extent.IsIntersecting(env_2D))
			{
				return geometry.CreateInstance();
			}
			// outside of bounds. return empty
			// geometry.
			com.epl.geometry.MultiVertexGeometryImpl impl = (com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl();
			com.epl.geometry.GeometryAccelerators accel = impl._getAccelerators();
			if (accel != null)
			{
				com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
				if (rgeom != null)
				{
					com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryEnvelopeInGeometry(extent);
					if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
					{
						if (geomtype != com.epl.geometry.Geometry.Type.Polygon.Value())
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
						com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon(geometry.GetDescription());
						poly.AddEnvelope(extent, false);
						return poly;
					}
					else
					{
						if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
						{
							return geometry.CreateInstance();
						}
					}
				}
			}
			switch (geomtype)
			{
				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					// outside of bounds.
					// return empty
					// geometry.
					com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)geometry;
					com.epl.geometry.MultiPoint multi_point_out = null;
					int npoints = multi_point.GetPointCount();
					com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)((com.epl.geometry.MultiPointImpl)multi_point._getImpl()).GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
					// create the new geometry only if there are points that has been
					// clipped out.
					// If all vertices are inside of the envelope, it returns the input
					// multipoint.
					int ipoints0 = 0;
					for (int ipoints = 0; ipoints < npoints; ipoints++)
					{
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						xy.Read(2 * ipoints, pt);
						if (!extent.Contains(pt))
						{
							// vertex is outside of the envelope
							if (ipoints0 == 0)
							{
								multi_point_out = (com.epl.geometry.MultiPoint)multi_point.CreateInstance();
							}
							if (ipoints0 < ipoints)
							{
								multi_point_out.Add(multi_point, ipoints0, ipoints);
							}
							ipoints0 = ipoints + 1;
						}
					}
					// ipoints0 contains index of vertex
					// right after the last clipped out
					// vertex.
					// add the rest of the batch to the result multipoint (only if
					// something has been already clipped out)
					if (ipoints0 > 0)
					{
						multi_point_out.Add(multi_point, ipoints0, npoints);
					}
					if (ipoints0 == 0)
					{
						return multi_point;
					}
					else
					{
						// everything is inside, so return the input
						// geometry
						return multi_point_out;
					}
					goto case com.epl.geometry.Geometry.GeometryType.Polygon;
				}

				case com.epl.geometry.Geometry.GeometryType.Polygon:
				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					// clipping has happend, return the
					// clipped geometry
					return ClipMultiPath_((com.epl.geometry.MultiPath)geometry, extent, tolerance, densify_dist);
				}

				default:
				{
					System.Diagnostics.Debug.Assert((false));
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		internal virtual int CompareVertices_(int v_1, int v_2)
		{
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			m_shape.GetXY(v_1, pt_1);
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			m_shape.GetXY(v_2, pt_2);
			int res = pt_1.Compare(pt_2);
			return res;
		}

		internal sealed class ClipperVertexComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.Clipper m_clipper;

			internal ClipperVertexComparer(com.epl.geometry.Clipper clipper)
			{
				m_clipper = clipper;
			}

			public override int Compare(int v1, int v2)
			{
				return m_clipper.CompareVertices_(v1, v2);
			}
		}
	}
}
