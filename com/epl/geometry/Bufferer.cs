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
using System.Linq;

namespace com.epl.geometry
{
	internal class Bufferer
	{
		/// <summary>Result is always a polygon.</summary>
		/// <remarks>
		/// Result is always a polygon. For non positive distance and non-areas
		/// returns an empty polygon. For points returns circles.
		/// </remarks>
		internal static com.epl.geometry.Geometry Buffer(com.epl.geometry.Geometry geometry, double distance, com.epl.geometry.SpatialReference sr, double densify_dist, int max_vertex_in_complete_circle, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geometry == null)
			{
				throw new System.ArgumentException();
			}
			if (densify_dist < 0)
			{
				throw new System.ArgumentException();
			}
			if (geometry.IsEmpty())
			{
				return new com.epl.geometry.Polygon(geometry.GetDescription());
			}
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			geometry.QueryLooseEnvelope2D(env2D);
			if (distance > 0)
			{
				env2D.Inflate(distance, distance);
			}
			com.epl.geometry.Bufferer bufferer = new com.epl.geometry.Bufferer(progress_tracker);
			bufferer.m_spatialReference = sr;
			bufferer.m_geometry = geometry;
			bufferer.m_tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, env2D, true);
			// conservative to have same effect as simplify
			bufferer.m_small_tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(null, env2D, true);
			// conservative
			// to have
			// same
			// effect as
			// simplify
			bufferer.m_distance = distance;
			bufferer.m_original_geom_type = geometry.GetType().Value();
			if (max_vertex_in_complete_circle <= 0)
			{
				max_vertex_in_complete_circle = 96;
			}
			// 96 is the value used by SG.
			// This is the number of
			// vertices in the full circle.
			bufferer.m_abs_distance = System.Math.Abs(bufferer.m_distance);
			bufferer.m_abs_distance_reversed = bufferer.m_abs_distance != 0 ? 1.0 / bufferer.m_abs_distance : 0;
			if (com.epl.geometry.NumberUtils.IsNaN(densify_dist) || densify_dist == 0)
			{
				densify_dist = bufferer.m_abs_distance * 1e-5;
			}
			else
			{
				if (densify_dist > bufferer.m_abs_distance * 0.5)
				{
					densify_dist = bufferer.m_abs_distance * 0.5;
				}
			}
			// do not allow too
			// large densify
			// distance (the
			// value will be
			// adjusted
			// anyway later)
			if (max_vertex_in_complete_circle < 12)
			{
				max_vertex_in_complete_circle = 12;
			}
			double max_dd = System.Math.Abs(distance) * (1 - System.Math.Cos(System.Math.PI / max_vertex_in_complete_circle));
			if (max_dd > densify_dist)
			{
				densify_dist = max_dd;
			}
			else
			{
				// the densify distance has to agree with the
				// max_vertex_in_complete_circle
				double vertex_count = System.Math.PI / System.Math.Acos(1.0 - densify_dist / System.Math.Abs(distance));
				if (vertex_count < (double)max_vertex_in_complete_circle - 1.0)
				{
					max_vertex_in_complete_circle = (int)vertex_count;
					if (max_vertex_in_complete_circle < 12)
					{
						max_vertex_in_complete_circle = 12;
						densify_dist = System.Math.Abs(distance) * (1 - System.Math.Cos(System.Math.PI / max_vertex_in_complete_circle));
					}
				}
			}
			bufferer.m_densify_dist = densify_dist;
			bufferer.m_max_vertex_in_complete_circle = max_vertex_in_complete_circle;
			// when filtering close points we do not want the filter to distort
			// generated buffer too much.
			bufferer.m_filter_tolerance = System.Math.Min(bufferer.m_small_tolerance, densify_dist * 0.25);
			return bufferer.Buffer_();
		}

		private com.epl.geometry.Geometry m_geometry;

		internal sealed class BufferCommand
		{
			internal abstract class Flags
			{
				public const int enum_line = 1;

				public const int enum_arc = 2;

				public const int enum_dummy = 4;

				public const int enum_concave_dip = 8;

				public const int enum_connection = enum_arc | enum_line;
			}

			internal static class FlagsConstants
			{
			}

			internal com.epl.geometry.Point2D m_from;

			internal com.epl.geometry.Point2D m_to;

			internal com.epl.geometry.Point2D m_center;

			internal int m_next;

			internal int m_prev;

			internal int m_type;

			internal BufferCommand(com.epl.geometry.Point2D from, com.epl.geometry.Point2D to, com.epl.geometry.Point2D center, int type, int next, int prev)
			{
				m_from = new com.epl.geometry.Point2D();
				m_to = new com.epl.geometry.Point2D();
				m_center = new com.epl.geometry.Point2D();
				m_from.SetCoords(from);
				m_to.SetCoords(to);
				m_center.SetCoords(center);
				m_type = type;
				m_next = next;
				m_prev = prev;
			}

			internal BufferCommand(com.epl.geometry.Point2D from, com.epl.geometry.Point2D to, int next, int prev, string dummy)
			{
				m_from = new com.epl.geometry.Point2D();
				m_to = new com.epl.geometry.Point2D();
				m_center = new com.epl.geometry.Point2D();
				m_from.SetCoords(from);
				m_to.SetCoords(to);
				m_center.SetNaN();
				m_type = 4;
				m_next = next;
				m_prev = prev;
			}
		}

		private System.Collections.Generic.List<com.epl.geometry.Bufferer.BufferCommand> m_buffer_commands;

		private int m_original_geom_type;

		private com.epl.geometry.ProgressTracker m_progress_tracker;

		private int m_max_vertex_in_complete_circle;

		private com.epl.geometry.SpatialReference m_spatialReference;

		private double m_tolerance;

		private double m_small_tolerance;

		private double m_filter_tolerance;

		private double m_densify_dist;

		private double m_distance;

		private double m_abs_distance;

		private double m_abs_distance_reversed;

		private double m_dA;

		private bool m_b_output_loops;

		private bool m_bfilter;

		private System.Collections.Generic.List<com.epl.geometry.Point2D> m_circle_template;

		private System.Collections.Generic.List<com.epl.geometry.Point2D> m_left_stack;

		private System.Collections.Generic.List<com.epl.geometry.Point2D> m_middle_stack;

		private com.epl.geometry.Line m_helper_line_1;

		private com.epl.geometry.Line m_helper_line_2;

		private com.epl.geometry.Point2D[] m_helper_array;

		private int m_progress_counter;

		private void GenerateCircleTemplate_()
		{
			if (m_circle_template == null)
			{
				m_circle_template = new System.Collections.Generic.List<com.epl.geometry.Point2D>(0);
			}
			else
			{
				if (!(m_circle_template.Count == 0))
				{
					return;
				}
			}
			int N = CalcN_(4);
			System.Diagnostics.Debug.Assert((N >= 4));
			int real_size = (N + 3) / 4;
			double dA = (System.Math.PI * 0.5) / real_size;
			m_dA = dA;
			for (int i = 0; i < real_size * 4; i++)
			{
				m_circle_template.Add(null);
			}
			double dcos = System.Math.Cos(dA);
			double dsin = System.Math.Sin(dA);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D(0.0, 1.0);
			for (int i_1 = 0; i_1 < real_size; i_1++)
			{
				m_circle_template[i_1 + real_size * 0] = new com.epl.geometry.Point2D(pt.y, -pt.x);
				m_circle_template[i_1 + real_size * 1] = new com.epl.geometry.Point2D(-pt.x, -pt.y);
				m_circle_template[i_1 + real_size * 2] = new com.epl.geometry.Point2D(-pt.y, pt.x);
				m_circle_template[i_1 + real_size * 3] = pt;
				pt = new com.epl.geometry.Point2D(pt.x, pt.y);
				pt.RotateReverse(dcos, dsin);
			}
		}

		private sealed class GeometryCursorForMultiPoint : com.epl.geometry.GeometryCursor
		{
			private int m_index;

			private com.epl.geometry.Geometry m_buffered_polygon;

			private com.epl.geometry.MultiPoint m_mp;

			private com.epl.geometry.SpatialReference m_spatialReference;

			private double m_distance;

			private double m_densify_dist;

			private double m_x;

			private double m_y;

			private int m_max_vertex_in_complete_circle;

			private com.epl.geometry.ProgressTracker m_progress_tracker;

			internal GeometryCursorForMultiPoint(com.epl.geometry.MultiPoint mp, double distance, com.epl.geometry.SpatialReference sr, double densify_dist, int max_vertex_in_complete_circle, com.epl.geometry.ProgressTracker progress_tracker)
			{
				// the template is filled with the index 0 corresponding to the point
				// (0, 0), following clockwise direction (0, -1), (-1, 0), (1, 0)
				m_index = 0;
				m_mp = mp;
				m_x = 0;
				m_y = 0;
				m_distance = distance;
				m_spatialReference = sr;
				m_densify_dist = densify_dist;
				m_max_vertex_in_complete_circle = max_vertex_in_complete_circle;
				m_progress_tracker = progress_tracker;
			}

			public override com.epl.geometry.Geometry Next()
			{
				com.epl.geometry.Point point = new com.epl.geometry.Point();
				while (true)
				{
					if (m_index == m_mp.GetPointCount())
					{
						return null;
					}
					m_mp.GetPointByVal(m_index, point);
					m_index++;
					if (point.IsEmpty())
					{
						continue;
					}
					break;
				}
				bool b_first = false;
				if (m_buffered_polygon == null)
				{
					m_x = point.GetX();
					m_y = point.GetY();
					m_buffered_polygon = com.epl.geometry.Bufferer.Buffer(point, m_distance, m_spatialReference, m_densify_dist, m_max_vertex_in_complete_circle, m_progress_tracker);
					b_first = true;
				}
				com.epl.geometry.Geometry res;
				if (m_index < m_mp.GetPointCount())
				{
					res = new com.epl.geometry.Polygon();
					m_buffered_polygon.CopyTo(res);
				}
				else
				{
					res = m_buffered_polygon;
				}
				// do not clone the last geometry.
				if (!b_first)
				{
					// don't apply transformation unnecessary
					com.epl.geometry.Transformation2D transform = new com.epl.geometry.Transformation2D();
					double dx = point.GetX() - m_x;
					double dy = point.GetY() - m_y;
					transform.SetShift(dx, dy);
					res.ApplyTransformation(transform);
				}
				return res;
			}

			public override int GetGeometryID()
			{
				return 0;
			}
		}

		private sealed class GeometryCursorForPolyline : com.epl.geometry.GeometryCursor
		{
			private com.epl.geometry.Bufferer m_bufferer;

			private int m_index;

			private bool m_bfilter;

			internal GeometryCursorForPolyline(com.epl.geometry.Bufferer bufferer, bool bfilter)
			{
				m_bufferer = bufferer;
				m_index = 0;
				m_bfilter = bfilter;
			}

			public override com.epl.geometry.Geometry Next()
			{
				com.epl.geometry.MultiPathImpl mp = (com.epl.geometry.MultiPathImpl)(m_bufferer.m_geometry._getImpl());
				if (m_index < mp.GetPathCount())
				{
					int ind = m_index;
					m_index++;
					if (!mp.IsClosedPathInXYPlane(ind))
					{
						com.epl.geometry.Point2D prev_end = mp.GetXY(mp.GetPathEnd(ind) - 1);
						while (m_index < mp.GetPathCount())
						{
							com.epl.geometry.Point2D start = mp.GetXY(mp.GetPathStart(m_index));
							if (mp.IsClosedPathInXYPlane(m_index))
							{
								break;
							}
							if (start != prev_end)
							{
								break;
							}
							prev_end = mp.GetXY(mp.GetPathEnd(m_index) - 1);
							m_index++;
						}
					}
					if (m_index - ind == 1)
					{
						return m_bufferer.BufferPolylinePath_((com.epl.geometry.Polyline)(m_bufferer.m_geometry), ind, m_bfilter);
					}
					else
					{
						com.epl.geometry.Polyline tmp_polyline = new com.epl.geometry.Polyline(m_bufferer.m_geometry.GetDescription());
						tmp_polyline.AddPath((com.epl.geometry.Polyline)(m_bufferer.m_geometry), ind, true);
						for (int i = ind + 1; i < m_index; i++)
						{
							((com.epl.geometry.MultiPathImpl)tmp_polyline._getImpl()).AddSegmentsFromPath((com.epl.geometry.MultiPathImpl)m_bufferer.m_geometry._getImpl(), i, 0, mp.GetSegmentCount(i), false);
						}
						// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/buffer_ppp.txt",
						// tmp_polyline, nullptr);
						com.epl.geometry.Polygon res = m_bufferer.BufferPolylinePath_(tmp_polyline, 0, m_bfilter);
						// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/buffer_ppp_res.txt",
						// *res, nullptr);
						return res;
					}
				}
				return null;
			}

			public override int GetGeometryID()
			{
				return 0;
			}
		}

		private sealed class GeometryCursorForPolygon : com.epl.geometry.GeometryCursor
		{
			private com.epl.geometry.Bufferer m_bufferer;

			private int m_index;

			internal GeometryCursorForPolygon(com.epl.geometry.Bufferer bufferer)
			{
				m_bufferer = bufferer;
				m_index = 0;
			}

			public override com.epl.geometry.Geometry Next()
			{
				com.epl.geometry.Polygon input_polygon = (com.epl.geometry.Polygon)(m_bufferer.m_geometry);
				if (m_index < input_polygon.GetPathCount())
				{
					int ind = m_index;
					double area = input_polygon.CalculateRingArea2D(m_index);
					System.Diagnostics.Debug.Assert((area > 0));
					m_index++;
					while (m_index < input_polygon.GetPathCount())
					{
						double hole_area = input_polygon.CalculateRingArea2D(m_index);
						if (hole_area > 0)
						{
							break;
						}
						// not a hole
						m_index++;
					}
					if (ind == 0 && m_index == input_polygon.GetPathCount())
					{
						return m_bufferer.BufferPolygonImpl_(input_polygon, 0, input_polygon.GetPathCount());
					}
					else
					{
						return m_bufferer.BufferPolygonImpl_(input_polygon, ind, m_index);
					}
				}
				return null;
			}

			public override int GetGeometryID()
			{
				return 0;
			}
		}

		private Bufferer(com.epl.geometry.ProgressTracker progress_tracker)
		{
			m_buffer_commands = new System.Collections.Generic.List<com.epl.geometry.Bufferer.BufferCommand>(0);
			m_progress_tracker = progress_tracker;
			m_tolerance = 0;
			m_small_tolerance = 0;
			m_filter_tolerance = 0;
			m_distance = 0;
			m_original_geom_type = com.epl.geometry.Geometry.GeometryType.Unknown;
			m_abs_distance_reversed = 0;
			m_abs_distance = 0;
			m_densify_dist = -1;
			m_dA = -1;
			m_b_output_loops = true;
			m_bfilter = true;
		}

		private com.epl.geometry.Geometry Buffer_()
		{
			int gt = m_geometry.GetType().Value();
			if (com.epl.geometry.Geometry.IsSegment(gt))
			{
				// convert segment to a polyline and repeat
				// the call
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(m_geometry.GetDescription());
				polyline.AddSegment((com.epl.geometry.Segment)(m_geometry), true);
				m_geometry = polyline;
				return Buffer_();
			}
			if (m_distance <= m_tolerance)
			{
				if (com.epl.geometry.Geometry.IsArea(gt))
				{
					if (m_distance <= 0)
					{
						// if the geometry is area type, then the negative distance
						// may produce a degenerate shape. Check for this and return
						// empty geometry.
						com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
						m_geometry.QueryEnvelope2D(env);
						if (env.GetWidth() <= -m_distance * 2 || env.GetHeight() <= m_distance * 2)
						{
							return new com.epl.geometry.Polygon(m_geometry.GetDescription());
						}
					}
				}
				else
				{
					return new com.epl.geometry.Polygon(m_geometry.GetDescription());
				}
			}
			switch (m_geometry.GetType().Value())
			{
				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					// return an
					// empty polygon
					// for distance
					// <=
					// m_tolerance
					// and any input
					// other than
					// polygon.
					// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/buffer_input.txt",
					// *m_geometry, nullptr);
					// Complex cases:
					return BufferPoint_();
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					return BufferMultiPoint_();
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					return BufferPolyline_();
				}

				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					return BufferPolygon_();
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					return BufferEnvelope_();
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		private com.epl.geometry.Geometry BufferPolyline_()
		{
			if (IsDegenerateGeometry_(m_geometry))
			{
				com.epl.geometry.Point point = new com.epl.geometry.Point();
				((com.epl.geometry.MultiVertexGeometry)m_geometry).GetPointByVal(0, point);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				m_geometry.QueryEnvelope2D(env2D);
				point.SetXY(env2D.GetCenter());
				return BufferPoint_(point);
			}
			System.Diagnostics.Debug.Assert((m_distance > 0));
			m_geometry = PreparePolyline_((com.epl.geometry.Polyline)(m_geometry));
			com.epl.geometry.Bufferer.GeometryCursorForPolyline cursor = new com.epl.geometry.Bufferer.GeometryCursorForPolyline(this, m_bfilter);
			com.epl.geometry.GeometryCursor union_cursor = ((com.epl.geometry.OperatorUnion)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Union)).Execute(cursor, m_spatialReference, m_progress_tracker);
			com.epl.geometry.Geometry result = union_cursor.Next();
			return result;
		}

		private com.epl.geometry.Geometry BufferPolygon_()
		{
			if (m_distance == 0)
			{
				return m_geometry;
			}
			// return input to the output.
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			GenerateCircleTemplate_();
			m_geometry = simplify.Execute(m_geometry, null, false, m_progress_tracker);
			if (m_distance < 0)
			{
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(m_geometry);
				com.epl.geometry.Polygon buffered_result = BufferPolygonImpl_(poly, 0, poly.GetPathCount());
				return simplify.Execute(buffered_result, m_spatialReference, false, m_progress_tracker);
			}
			else
			{
				if (IsDegenerateGeometry_(m_geometry))
				{
					com.epl.geometry.Point point = new com.epl.geometry.Point();
					((com.epl.geometry.MultiVertexGeometry)m_geometry).GetPointByVal(0, point);
					com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
					m_geometry.QueryEnvelope2D(env2D);
					point.SetXY(env2D.GetCenter());
					return BufferPoint_(point);
				}
				// For the positive distance we need to process polygon in the parts
				// such that each exterior ring with holes is processed separatelly.
				com.epl.geometry.Bufferer.GeometryCursorForPolygon cursor = new com.epl.geometry.Bufferer.GeometryCursorForPolygon(this);
				com.epl.geometry.GeometryCursor union_cursor = ((com.epl.geometry.OperatorUnion)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Union)).Execute(cursor, m_spatialReference, m_progress_tracker);
				com.epl.geometry.Geometry result = union_cursor.Next();
				return result;
			}
		}

		private com.epl.geometry.Polygon BufferPolygonImpl_(com.epl.geometry.Polygon input_geom, int ipath_begin, int ipath_end)
		{
			com.epl.geometry.MultiPath input_mp = (com.epl.geometry.MultiPath)(input_geom);
			com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)(input_mp._getImpl());
			com.epl.geometry.Polygon intermediate_polygon = new com.epl.geometry.Polygon(input_geom.GetDescription());
			for (int ipath = ipath_begin; ipath < ipath_end; ipath++)
			{
				if (mp_impl.GetPathSize(ipath) < 1)
				{
					continue;
				}
				double path_area = mp_impl.CalculateRingArea2D(ipath);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				mp_impl.QueryPathEnvelope2D(ipath, env2D);
				if (m_distance > 0)
				{
					if (path_area > 0)
					{
						if (IsDegeneratePath_(mp_impl, ipath))
						{
							// if a path is
							// degenerate
							// (almost a point),
							// then we can draw
							// a circle instead
							// of it as a buffer
							// and nobody would
							// notice :)
							com.epl.geometry.Point point = new com.epl.geometry.Point();
							mp_impl.GetPointByVal(mp_impl.GetPathStart(ipath), point);
							point.SetXY(env2D.GetCenter());
							AddCircle_((com.epl.geometry.MultiPathImpl)intermediate_polygon._getImpl(), point);
						}
						else
						{
							com.epl.geometry.Polyline result_polyline = new com.epl.geometry.Polyline(input_geom.GetDescription());
							com.epl.geometry.MultiPathImpl result_mp = (com.epl.geometry.MultiPathImpl)result_polyline._getImpl();
							// We often see convex hulls, buffering those is an
							// extremely simple task.
							bool bConvex = com.epl.geometry.ConvexHull.IsPathConvex((com.epl.geometry.Polygon)(m_geometry), ipath, m_progress_tracker);
							if (bConvex || BufferClosedPath_(m_geometry, ipath, result_mp, true, 1) == 2)
							{
								com.epl.geometry.Polygon buffered_path = BufferConvexPath_(input_mp, ipath);
								intermediate_polygon.Add(buffered_path, false);
							}
							else
							{
								com.epl.geometry.Polygon buffered_path = BufferCleanup_(result_polyline, false);
								intermediate_polygon.Add(buffered_path, false);
							}
						}
					}
					else
					{
						if (env2D.GetWidth() + m_tolerance <= 2 * m_abs_distance || env2D.GetHeight() + m_tolerance <= 2 * m_abs_distance)
						{
							// skip
							// parts
							// that
							// will
							// dissapear
							continue;
						}
						com.epl.geometry.Polyline result_polyline = new com.epl.geometry.Polyline(input_geom.GetDescription());
						com.epl.geometry.MultiPathImpl result_mp = (com.epl.geometry.MultiPathImpl)result_polyline._getImpl();
						BufferClosedPath_(m_geometry, ipath, result_mp, true, 1);
						if (!result_polyline.IsEmpty())
						{
							com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
							env.SetCoords(env2D);
							env.Inflate(m_abs_distance, m_abs_distance);
							result_mp.AddEnvelope(env, false);
							com.epl.geometry.Polygon buffered_path = BufferCleanup_(result_polyline, false);
							// intermediate_polygon.reserve(intermediate_polygon.getPointCount()
							// + buffered_path.getPointCount() - 4);
							for (int i = 1, n = buffered_path.GetPathCount(); i < n; i++)
							{
								intermediate_polygon.AddPath(buffered_path, i, true);
							}
						}
					}
				}
				else
				{
					if (path_area > 0)
					{
						if (env2D.GetWidth() + m_tolerance <= 2 * m_abs_distance || env2D.GetHeight() + m_tolerance <= 2 * m_abs_distance)
						{
							// skip
							// parts
							// that
							// will
							// dissapear
							continue;
						}
						com.epl.geometry.Polyline result_polyline = new com.epl.geometry.Polyline(input_geom.GetDescription());
						com.epl.geometry.MultiPathImpl result_mp = (com.epl.geometry.MultiPathImpl)result_polyline._getImpl();
						BufferClosedPath_(m_geometry, ipath, result_mp, true, -1);
						// this
						// will
						// provide
						// a
						// shape
						// buffered
						// inwards.
						// It
						// has
						// counterclockwise
						// orientation
						if (!result_polyline.IsEmpty())
						{
							com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
							result_mp.QueryLooseEnvelope2D(env);
							env.Inflate(m_abs_distance, m_abs_distance);
							result_mp.AddEnvelope(env, false);
							// add an envelope
							// exterior shell
							com.epl.geometry.Polygon buffered_path = BufferCleanup_(result_polyline, false);
							// simplify with winding rule
							// extract all parts but the first one (which is the
							// envelope we added previously)
							for (int i = 1, npaths = buffered_path.GetPathCount(); i < npaths; i++)
							{
								// the extracted parts have inverted orientation.
								intermediate_polygon.AddPath(buffered_path, i, true);
							}
						}
					}
					else
					{
						// the path has been erased
						// When buffering a hole with negative distance, buffer it
						// as if it is an exterior ring buffered with positive
						// distance
						com.epl.geometry.Polyline result_polyline = new com.epl.geometry.Polyline(input_geom.GetDescription());
						com.epl.geometry.MultiPathImpl result_mp = (com.epl.geometry.MultiPathImpl)result_polyline._getImpl();
						BufferClosedPath_(m_geometry, ipath, result_mp, true, -1);
						// this
						// will
						// provide
						// a
						// shape
						// buffered
						// inwards.
						com.epl.geometry.Polygon buffered_path = BufferCleanup_(result_polyline, false);
						for (int i = 0, npaths = buffered_path.GetPathCount(); i < npaths; i++)
						{
							intermediate_polygon.AddPath(buffered_path, i, true);
						}
					}
				}
			}
			// adds
			// buffered
			// hole
			// reversed
			// as
			// if
			// it
			// is
			// exteror
			// ring
			// intermediate_polygon has inverted orientation.
			if (m_distance > 0)
			{
				if (intermediate_polygon.GetPathCount() > 1)
				{
					com.epl.geometry.Polygon cleaned_polygon = BufferCleanup_(intermediate_polygon, false);
					return cleaned_polygon;
				}
				else
				{
					return SetWeakSimple_(intermediate_polygon);
				}
			}
			else
			{
				com.epl.geometry.Envelope2D polyenv = new com.epl.geometry.Envelope2D();
				intermediate_polygon.QueryLooseEnvelope2D(polyenv);
				if (!intermediate_polygon.IsEmpty())
				{
					// negative buffer distance. We got buffered holes and exterior
					// rings. They all have wrong orientation.
					// we need to apply winding simplify again to ensure all holes
					// are unioned.
					// For that create a big envelope and add all rings of the
					// intermediate_polygon to it.
					polyenv.Inflate(m_abs_distance, m_abs_distance);
					intermediate_polygon.AddEnvelope(polyenv, false);
					com.epl.geometry.Polygon cleaned_polygon = BufferCleanup_(intermediate_polygon, false);
					// intermediate_polygon.reset();//free memory
					com.epl.geometry.Polygon result_polygon = new com.epl.geometry.Polygon(cleaned_polygon.GetDescription());
					for (int i = 1, n = cleaned_polygon.GetPathCount(); i < n; i++)
					{
						result_polygon.AddPath(cleaned_polygon, i, false);
					}
					return SetWeakSimple_(result_polygon);
				}
				else
				{
					return SetWeakSimple_(intermediate_polygon);
				}
			}
		}

		private com.epl.geometry.Geometry BufferPoint_()
		{
			return BufferPoint_((com.epl.geometry.Point)(m_geometry));
		}

		private com.epl.geometry.Geometry BufferPoint_(com.epl.geometry.Point point)
		{
			System.Diagnostics.Debug.Assert((m_distance > 0));
			com.epl.geometry.Polygon resultPolygon = new com.epl.geometry.Polygon(m_geometry.GetDescription());
			AddCircle_((com.epl.geometry.MultiPathImpl)resultPolygon._getImpl(), point);
			return SetStrongSimple_(resultPolygon);
		}

		private com.epl.geometry.Geometry BufferMultiPoint_()
		{
			System.Diagnostics.Debug.Assert((m_distance > 0));
			com.epl.geometry.Bufferer.GeometryCursorForMultiPoint mpCursor = new com.epl.geometry.Bufferer.GeometryCursorForMultiPoint((com.epl.geometry.MultiPoint)(m_geometry), m_distance, m_spatialReference, m_densify_dist, m_max_vertex_in_complete_circle, m_progress_tracker);
			com.epl.geometry.GeometryCursor c = ((com.epl.geometry.OperatorUnion)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Union)).Execute(mpCursor, m_spatialReference, m_progress_tracker);
			return c.Next();
		}

		private com.epl.geometry.Geometry BufferEnvelope_()
		{
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon(m_geometry.GetDescription());
			if (m_distance <= 0)
			{
				if (m_distance == 0)
				{
					polygon.AddEnvelope((com.epl.geometry.Envelope)(m_geometry), false);
				}
				else
				{
					com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
					m_geometry.QueryEnvelope(env);
					env.Inflate(m_distance, m_distance);
					polygon.AddEnvelope(env, false);
				}
				return polygon;
			}
			// nothing is easier than negative buffer on the
			// envelope.
			polygon.AddEnvelope((com.epl.geometry.Envelope)(m_geometry), false);
			m_geometry = polygon;
			return BufferConvexPath_(polygon, 0);
		}

		private com.epl.geometry.Polygon BufferConvexPath_(com.epl.geometry.MultiPath src, int ipath)
		{
			GenerateCircleTemplate_();
			com.epl.geometry.Polygon resultPolygon = new com.epl.geometry.Polygon(src.GetDescription());
			com.epl.geometry.MultiPathImpl result_mp = (com.epl.geometry.MultiPathImpl)resultPolygon._getImpl();
			// resultPolygon.reserve((m_circle_template.size() / 10 + 4) *
			// src.getPathSize(ipath));
			com.epl.geometry.Point2D pt_1_tmp = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_2_tmp = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_3_tmp = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_3 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			com.epl.geometry.MultiPathImpl src_mp = (com.epl.geometry.MultiPathImpl)src._getImpl();
			int path_size = src.GetPathSize(ipath);
			int path_start = src.GetPathStart(ipath);
			for (int i = 0, n = src.GetPathSize(ipath); i < n; i++)
			{
				src_mp.GetXY(path_start + i, pt_1);
				src_mp.GetXY(path_start + (i + 1) % path_size, pt_2);
				src_mp.GetXY(path_start + (i + 2) % path_size, pt_3);
				v_1.Sub(pt_2, pt_1);
				if (v_1.Length() == 0)
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
				v_1.LeftPerpendicular();
				v_1.Normalize();
				v_1.Scale(m_abs_distance);
				pt_1_tmp.Add(v_1, pt_1);
				pt_2_tmp.Add(v_1, pt_2);
				if (i == 0)
				{
					result_mp.StartPath(pt_1_tmp);
				}
				else
				{
					result_mp.LineTo(pt_1_tmp);
				}
				result_mp.LineTo(pt_2_tmp);
				v_2.Sub(pt_3, pt_2);
				if (v_2.Length() == 0)
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
				v_2.LeftPerpendicular();
				v_2.Normalize();
				v_2.Scale(m_abs_distance);
				pt_3_tmp.Add(v_2, pt_2);
				AddJoin_(result_mp, pt_2, pt_2_tmp, pt_3_tmp, false, false);
			}
			return SetWeakSimple_(resultPolygon);
		}

		private com.epl.geometry.Polygon BufferPolylinePath_(com.epl.geometry.Polyline polyline, int ipath, bool bfilter)
		{
			System.Diagnostics.Debug.Assert((m_distance != 0));
			GenerateCircleTemplate_();
			com.epl.geometry.MultiPath input_multi_path = polyline;
			com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)(input_multi_path._getImpl());
			if (mp_impl.GetPathSize(ipath) < 1)
			{
				return null;
			}
			if (IsDegeneratePath_(mp_impl, ipath) && m_distance > 0)
			{
				// if a path
				// is
				// degenerate
				// (almost a
				// point),
				// then we
				// can draw
				// a circle
				// instead
				// of it as
				// a buffer
				// and
				// nobody
				// would
				// notice :)
				com.epl.geometry.Point point = new com.epl.geometry.Point();
				mp_impl.GetPointByVal(mp_impl.GetPathStart(ipath), point);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				mp_impl.QueryPathEnvelope2D(ipath, env2D);
				point.SetXY(env2D.GetCenter());
				return (com.epl.geometry.Polygon)(BufferPoint_(point));
			}
			com.epl.geometry.Polyline result_polyline = new com.epl.geometry.Polyline(polyline.GetDescription());
			// result_polyline.reserve((m_circle_template.size() / 10 + 4) *
			// mp_impl.getPathSize(ipath));
			com.epl.geometry.MultiPathImpl result_mp = (com.epl.geometry.MultiPathImpl)result_polyline._getImpl();
			bool b_closed = mp_impl.IsClosedPathInXYPlane(ipath);
			if (b_closed)
			{
				BufferClosedPath_(input_multi_path, ipath, result_mp, bfilter, 1);
				BufferClosedPath_(input_multi_path, ipath, result_mp, bfilter, -1);
			}
			else
			{
				com.epl.geometry.Polyline tmpPoly = new com.epl.geometry.Polyline(input_multi_path.GetDescription());
				tmpPoly.AddPath(input_multi_path, ipath, false);
				((com.epl.geometry.MultiPathImpl)tmpPoly._getImpl()).AddSegmentsFromPath((com.epl.geometry.MultiPathImpl)input_multi_path._getImpl(), ipath, 0, input_multi_path.GetSegmentCount(ipath), false);
				BufferClosedPath_(tmpPoly, 0, result_mp, bfilter, 1);
			}
			// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/buffer_prepare.txt",
			// *result_polyline, nullptr);
			return BufferCleanup_(result_polyline, false);
		}

		private void Progress_()
		{
			m_progress_counter++;
			if (m_progress_counter % 1024 == 0)
			{
				if ((m_progress_tracker != null) && !(m_progress_tracker.Progress(-1, -1)))
				{
					throw new System.Exception("user_canceled");
				}
			}
		}

		private com.epl.geometry.Polygon BufferCleanup_(com.epl.geometry.MultiPath multi_path, bool simplify_result)
		{
			double tol = simplify_result ? m_tolerance : m_small_tolerance;
			com.epl.geometry.Polygon resultPolygon = (com.epl.geometry.Polygon)(com.epl.geometry.TopologicalOperations.PlanarSimplify(multi_path, tol, true, !simplify_result, m_progress_tracker));
			System.Diagnostics.Debug.Assert((com.epl.geometry.InternalUtils.IsWeakSimple(resultPolygon, 0.0)));
			return resultPolygon;
		}

		private int CalcN_(int minN)
		{
			if (m_densify_dist == 0)
			{
				return m_max_vertex_in_complete_circle;
			}
			double r = m_densify_dist * System.Math.Abs(m_abs_distance_reversed);
			double cos_a = 1 - r;
			double N;
			if (cos_a < -1)
			{
				N = minN;
			}
			else
			{
				N = 2.0 * System.Math.PI / System.Math.Acos(cos_a) + 0.5;
			}
			if (N < minN)
			{
				N = minN;
			}
			else
			{
				if (N > m_max_vertex_in_complete_circle)
				{
					N = m_max_vertex_in_complete_circle;
				}
			}
			return (int)N;
		}

		private void AddJoin_(com.epl.geometry.MultiPathImpl dst, com.epl.geometry.Point2D center, com.epl.geometry.Point2D fromPt, com.epl.geometry.Point2D toPt, bool bStartPath, bool bFinishAtToPt)
		{
			GenerateCircleTemplate_();
			com.epl.geometry.Point2D v_1 = new com.epl.geometry.Point2D();
			v_1.Sub(fromPt, center);
			v_1.Scale(m_abs_distance_reversed);
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			v_2.Sub(toPt, center);
			v_2.Scale(m_abs_distance_reversed);
			double angle_from = System.Math.Atan2(v_1.y, v_1.x);
			double dindex_from = angle_from / m_dA;
			if (dindex_from < 0)
			{
				dindex_from = (double)m_circle_template.Count + dindex_from;
			}
			dindex_from = (double)m_circle_template.Count - dindex_from;
			double angle_to = System.Math.Atan2(v_2.y, v_2.x);
			double dindex_to = angle_to / m_dA;
			if (dindex_to < 0)
			{
				dindex_to = (double)m_circle_template.Count + dindex_to;
			}
			dindex_to = (double)m_circle_template.Count - dindex_to;
			if (dindex_to < dindex_from)
			{
				dindex_to += (double)m_circle_template.Count;
			}
			System.Diagnostics.Debug.Assert((dindex_to >= dindex_from));
			int index_to = (int)dindex_to;
			int index_from = (int)System.Math.Ceiling(dindex_from);
			if (bStartPath)
			{
				dst.StartPath(fromPt);
				bStartPath = false;
			}
			com.epl.geometry.Point2D p = new com.epl.geometry.Point2D();
			p.SetCoords(m_circle_template[index_from % m_circle_template.Count]);
			p.ScaleAdd(m_abs_distance, center);
			double ddd = m_tolerance * 10;
			p.Sub(fromPt);
			if (p.Length() < ddd)
			{
				// if too close to the fromPt, then use the next
				// point
				index_from += 1;
			}
			p.SetCoords(m_circle_template[index_to % m_circle_template.Count]);
			p.ScaleAdd(m_abs_distance, center);
			p.Sub(toPt);
			if (p.Length() < ddd)
			{
				// if too close to the toPt, then use the prev
				// point
				index_to -= 1;
			}
			int count = index_to - index_from;
			count++;
			for (int i = 0, j = index_from % m_circle_template.Count; i < count; i++, j = (j + 1) % m_circle_template.Count)
			{
				p.SetCoords(m_circle_template[j]);
				p.ScaleAdd(m_abs_distance, center);
				dst.LineTo(p);
				Progress_();
			}
			if (bFinishAtToPt)
			{
				dst.LineTo(toPt);
			}
		}

		private int BufferClosedPath_(com.epl.geometry.Geometry input_geom, int ipath, com.epl.geometry.MultiPathImpl result_mp, bool bfilter, int dir)
		{
			// Use temporary polyline for the path buffering.
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom = edit_shape.AddPathFromMultiPath((com.epl.geometry.MultiPath)input_geom, ipath, true);
			edit_shape.FilterClosePoints(m_filter_tolerance, false, false);
			if (edit_shape.GetPointCount(geom) < 2)
			{
				// Got degenerate output.
				// Wither bail out or
				// produce a circle.
				if (dir < 0)
				{
					return 1;
				}
				// negative direction produces nothing.
				com.epl.geometry.MultiPath mpIn = (com.epl.geometry.MultiPath)input_geom;
				// Add a circle
				com.epl.geometry.Point pt = new com.epl.geometry.Point();
				mpIn.GetPointByVal(mpIn.GetPathStart(ipath), pt);
				AddCircle_(result_mp, pt);
				return 1;
			}
			System.Diagnostics.Debug.Assert((edit_shape.GetFirstPath(geom) != -1));
			System.Diagnostics.Debug.Assert((edit_shape.GetFirstVertex(edit_shape.GetFirstPath(geom)) != -1));
			com.epl.geometry.Point2D origin = edit_shape.GetXY(edit_shape.GetFirstVertex(edit_shape.GetFirstPath(geom)));
			com.epl.geometry.Transformation2D tr = new com.epl.geometry.Transformation2D();
			tr.SetShift(-origin.x, -origin.y);
			// move the path to origin for better accuracy in calculations.
			edit_shape.ApplyTransformation(tr);
			if (bfilter)
			{
				// try removing the noise that does not contribute to the buffer.
				int res_filter = FilterPath_(edit_shape, geom, dir, true);
				System.Diagnostics.Debug.Assert((res_filter == 1));
				// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/buffer_filter.txt",
				// *edit_shape.get_geometry(geom), nullptr);
				if (edit_shape.GetPointCount(geom) < 2)
				{
					// got degenerate output.
					// Wither bail out or
					// produce a circle.
					if (dir < 0)
					{
						return 1;
					}
					// negative direction produces nothing.
					com.epl.geometry.MultiPath mpIn = (com.epl.geometry.MultiPath)input_geom;
					// Add a circle
					com.epl.geometry.Point pt = new com.epl.geometry.Point();
					mpIn.GetPointByVal(mpIn.GetPathStart(ipath), pt);
					AddCircle_(result_mp, pt);
					return 1;
				}
			}
			m_buffer_commands.Clear();
			int path = edit_shape.GetFirstPath(geom);
			int ivert = edit_shape.GetFirstVertex(path);
			int iprev = dir == 1 ? edit_shape.GetPrevVertex(ivert) : edit_shape.GetNextVertex(ivert);
			int inext = dir == 1 ? edit_shape.GetNextVertex(ivert) : edit_shape.GetPrevVertex(ivert);
			bool b_first = true;
			com.epl.geometry.Point2D pt_current = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_after = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_before = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_left_prev = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_after = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_before = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_left = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D v_left_prev = new com.epl.geometry.Point2D();
			double abs_d = m_abs_distance;
			int ncount = edit_shape.GetPathSize(path);
			// write out buffer commands as a set of arcs and line segments.
			// if we'd convert this directly to a polygon and draw using winding
			// fill rule, we'd get the buffered result.
			for (int index = 0; index < ncount; index++)
			{
				edit_shape.GetXY(inext, pt_after);
				if (b_first)
				{
					edit_shape.GetXY(ivert, pt_current);
					edit_shape.GetXY(iprev, pt_before);
					v_before.Sub(pt_current, pt_before);
					v_before.Normalize();
					v_left_prev.LeftPerpendicular(v_before);
					v_left_prev.Scale(abs_d);
					pt_left_prev.Add(v_left_prev, pt_current);
				}
				v_after.Sub(pt_after, pt_current);
				v_after.Normalize();
				v_left.LeftPerpendicular(v_after);
				v_left.Scale(abs_d);
				pt_1.Add(pt_current, v_left);
				double cross = v_before.CrossProduct(v_after);
				double dot = v_before.DotProduct(v_after);
				bool bDoJoin = cross < 0 || (dot < 0 && cross == 0);
				if (bDoJoin)
				{
					m_buffer_commands.Add(new com.epl.geometry.Bufferer.BufferCommand(pt_left_prev, pt_1, pt_current, com.epl.geometry.Bufferer.BufferCommand.Flags.enum_arc, m_buffer_commands.Count + 1, m_buffer_commands.Count - 1));
				}
				else
				{
					if (!pt_left_prev.IsEqual(pt_1))
					{
						m_buffer_commands.Add(new com.epl.geometry.Bufferer.BufferCommand(pt_left_prev, pt_current, m_buffer_commands.Count + 1, m_buffer_commands.Count - 1, "dummy"));
						m_buffer_commands.Add(new com.epl.geometry.Bufferer.BufferCommand(pt_current, pt_1, m_buffer_commands.Count + 1, m_buffer_commands.Count - 1, "dummy"));
					}
				}
				pt1.Add(pt_after, v_left);
				m_buffer_commands.Add(new com.epl.geometry.Bufferer.BufferCommand(pt_1, pt1, pt_current, com.epl.geometry.Bufferer.BufferCommand.Flags.enum_line, m_buffer_commands.Count + 1, m_buffer_commands.Count - 1));
				pt_left_prev.SetCoords(pt1);
				v_left_prev.SetCoords(v_left);
				pt_before.SetCoords(pt_current);
				pt_current.SetCoords(pt_after);
				v_before.SetCoords(v_after);
				iprev = ivert;
				ivert = inext;
				b_first = false;
				inext = dir == 1 ? edit_shape.GetNextVertex(ivert) : edit_shape.GetPrevVertex(ivert);
			}
			m_buffer_commands[m_buffer_commands.Count - 1].m_next = 0;
			m_buffer_commands[0].m_prev = m_buffer_commands.Count - 1;
			ProcessBufferCommands_(result_mp);
			tr.SetShift(origin.x, origin.y);
			// move the path to improve precision.
			result_mp.ApplyTransformation(tr, result_mp.GetPathCount() - 1);
			return 1;
		}

		private void ProcessBufferCommands_(com.epl.geometry.MultiPathImpl result_mp)
		{
			int ifirst_seg = CleanupBufferCommands_();
			bool first = true;
			int iseg_next = ifirst_seg + 1;
			for (int iseg = ifirst_seg; iseg_next != ifirst_seg; iseg = iseg_next)
			{
				com.epl.geometry.Bufferer.BufferCommand command = m_buffer_commands[iseg];
				iseg_next = command.m_next != -1 ? command.m_next : (iseg + 1) % m_buffer_commands.Count;
				if (command.m_type == 0)
				{
					continue;
				}
				// deleted segment
				if (first)
				{
					result_mp.StartPath(command.m_from);
					first = false;
				}
				if (command.m_type == com.epl.geometry.Bufferer.BufferCommand.Flags.enum_arc)
				{
					// arc
					AddJoin_(result_mp, command.m_center, command.m_from, command.m_to, false, true);
				}
				else
				{
					result_mp.LineTo(command.m_to);
				}
				first = false;
			}
		}

		private int CleanupBufferCommands_()
		{
			// The purpose of this function is to remove as many self intersections
			// from the buffered shape as possible.
			// The buffer works without cleanup also, but slower.
			if (m_helper_array == null)
			{
				m_helper_array = new com.epl.geometry.Point2D[9];
			}
			int istart = 0;
			for (int iseg = 0, nseg = m_buffer_commands.Count; iseg < nseg; )
			{
				com.epl.geometry.Bufferer.BufferCommand command = m_buffer_commands[iseg];
				if ((command.m_type & com.epl.geometry.Bufferer.BufferCommand.Flags.enum_connection) != 0)
				{
					istart = iseg;
					break;
				}
				iseg = command.m_next;
			}
			int iseg_next = istart + 1;
			for (int iseg_1 = istart; iseg_next != istart; iseg_1 = iseg_next)
			{
				com.epl.geometry.Bufferer.BufferCommand command = m_buffer_commands[iseg_1];
				iseg_next = command.m_next;
				int count = 1;
				com.epl.geometry.Bufferer.BufferCommand command_next = null;
				while (iseg_next != iseg_1)
				{
					// find next segement
					command_next = m_buffer_commands[iseg_next];
					if ((command_next.m_type & com.epl.geometry.Bufferer.BufferCommand.Flags.enum_connection) != 0)
					{
						break;
					}
					iseg_next = command_next.m_next;
					count++;
				}
				if (count == 1)
				{
					// Next segment starts where this one ends. Skip this case as it
					// is simple.
					System.Diagnostics.Debug.Assert((command.m_to.IsEqual(command_next.m_from)));
					continue;
				}
				if ((command.m_type & command_next.m_type) == com.epl.geometry.Bufferer.BufferCommand.Flags.enum_line)
				{
					// simplest
					// cleanup
					// -
					// intersect
					// lines
					if (m_helper_line_1 == null)
					{
						m_helper_line_1 = new com.epl.geometry.Line();
						m_helper_line_2 = new com.epl.geometry.Line();
					}
					m_helper_line_1.SetStartXY(command.m_from);
					m_helper_line_1.SetEndXY(command.m_to);
					m_helper_line_2.SetStartXY(command_next.m_from);
					m_helper_line_2.SetEndXY(command_next.m_to);
					int count_ = m_helper_line_1.Intersect(m_helper_line_2, m_helper_array, null, null, m_small_tolerance);
					if (count_ == 1)
					{
						command.m_to.SetCoords(m_helper_array[0]);
						command_next.m_from.SetCoords(m_helper_array[0]);
						command.m_next = iseg_next;
						// skip until iseg_next
						command_next.m_prev = iseg_1;
					}
					else
					{
						if (count_ == 2)
						{
						}
					}
				}
			}
			// TODO: this case needs improvement
			return istart;
		}

		private bool IsGap_(com.epl.geometry.Point2D pt_before, com.epl.geometry.Point2D pt_current, com.epl.geometry.Point2D pt_after)
		{
			com.epl.geometry.Point2D v_gap = new com.epl.geometry.Point2D();
			v_gap.Sub(pt_after, pt_before);
			double gap_length = v_gap.Length();
			double sqr_delta = m_abs_distance * m_abs_distance - gap_length * gap_length * 0.25;
			if (sqr_delta > 0)
			{
				double delta = System.Math.Sqrt(sqr_delta);
				v_gap.Normalize();
				v_gap.RightPerpendicular();
				com.epl.geometry.Point2D p = new com.epl.geometry.Point2D();
				p.Sub(pt_current, pt_before);
				double d = p.DotProduct(v_gap);
				if (d + delta >= m_abs_distance)
				{
					return true;
				}
			}
			return false;
		}

		private int FilterPath_(com.epl.geometry.EditShape edit_shape, int geom, int dir, bool closed)
		{
			// **********************!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// return 1;
			bool bConvex = true;
			for (int pass = 0; pass < 1; pass++)
			{
				bool b_filtered = false;
				int ipath = edit_shape.GetFirstPath(geom);
				int isize = edit_shape.GetPathSize(ipath);
				if (isize == 0)
				{
					return 0;
				}
				int ncount = isize;
				if (isize < 3)
				{
					return 1;
				}
				if (closed && !edit_shape.IsClosedPath(ipath))
				{
					// the path is closed
					// only virtually
					ncount = isize - 1;
				}
				System.Diagnostics.Debug.Assert((dir == 1 || dir == -1));
				int ivert = edit_shape.GetFirstVertex(ipath);
				if (!closed)
				{
					edit_shape.GetNextVertex(ivert);
				}
				int iprev = dir > 0 ? edit_shape.GetPrevVertex(ivert) : edit_shape.GetNextVertex(ivert);
				int inext = dir > 0 ? edit_shape.GetNextVertex(ivert) : edit_shape.GetPrevVertex(ivert);
				int ibefore = iprev;
				bool reload = true;
				com.epl.geometry.Point2D pt_current = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_after = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_before = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_before_before = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_middle = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_gap_last = new com.epl.geometry.Point2D(0, 0);
				com.epl.geometry.Point2D v_after = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D v_before = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D v_gap = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D temp = new com.epl.geometry.Point2D();
				double abs_d = m_abs_distance;
				// When the path is open we cannot process the first and the last
				// vertices, so we process size - 2.
				// When the path is closed, we can process all vertices.
				int iter_count = closed ? ncount : isize - 2;
				int gap_counter = 0;
				for (int iter = 0; iter < iter_count; )
				{
					edit_shape.GetXY(inext, pt_after);
					if (reload)
					{
						edit_shape.GetXY(ivert, pt_current);
						edit_shape.GetXY(iprev, pt_before);
						ibefore = iprev;
					}
					v_before.Sub(pt_current, pt_before);
					v_before.Normalize();
					v_after.Sub(pt_after, pt_current);
					v_after.Normalize();
					if (ibefore == inext)
					{
						break;
					}
					double cross = v_before.CrossProduct(v_after);
					double dot = v_before.DotProduct(v_after);
					bool bDoJoin = cross < 0 || (dot < 0 && cross == 0);
					bool b_write = true;
					if (!bDoJoin)
					{
						if (IsGap_(pt_before, pt_current, pt_after))
						{
							pt_gap_last.SetCoords(pt_after);
							b_write = false;
							++gap_counter;
							b_filtered = true;
						}
						bConvex = false;
					}
					if (b_write)
					{
						if (gap_counter > 0)
						{
							for (; ; )
							{
								// re-test back
								int ibefore_before = dir > 0 ? edit_shape.GetPrevVertex(ibefore) : edit_shape.GetNextVertex(ibefore);
								if (ibefore_before == ivert)
								{
									break;
								}
								edit_shape.GetXY(ibefore_before, pt_before_before);
								if (IsGap_(pt_before_before, pt_before, pt_gap_last))
								{
									pt_before.SetCoords(pt_before_before);
									ibefore = ibefore_before;
									b_write = false;
									++gap_counter;
									continue;
								}
								else
								{
									if (ibefore_before != inext && IsGap_(pt_before_before, pt_before, pt_after) && IsGap_(pt_before_before, pt_current, pt_after))
									{
										// now the current
										// point is a part
										// of the gap also.
										// We retest it.
										pt_before.SetCoords(pt_before_before);
										ibefore = ibefore_before;
										b_write = false;
										++gap_counter;
									}
								}
								break;
							}
						}
						if (!b_write)
						{
							continue;
						}
						// retest forward
						if (gap_counter > 0)
						{
							// remove all but one gap vertices.
							int p = dir > 0 ? edit_shape.GetPrevVertex(iprev) : edit_shape.GetNextVertex(iprev);
							for (int i = 1; i < gap_counter; i++)
							{
								int pp = dir > 0 ? edit_shape.GetPrevVertex(p) : edit_shape.GetNextVertex(p);
								edit_shape.RemoveVertex(p, true);
								p = pp;
							}
							v_gap.Sub(pt_current, pt_before);
							double gap_length = v_gap.Length();
							double sqr_delta = abs_d * abs_d - gap_length * gap_length * 0.25;
							double delta = System.Math.Sqrt(sqr_delta);
							if (abs_d - delta > m_densify_dist * 0.5)
							{
								pt_middle.Add(pt_before, pt_current);
								pt_middle.Scale(0.5);
								v_gap.Normalize();
								v_gap.RightPerpendicular();
								temp.SetCoords(v_gap);
								temp.Scale(abs_d - delta);
								pt_middle.Add(temp);
								edit_shape.SetXY(iprev, pt_middle);
							}
							else
							{
								// the gap is too short to be considered. Can close
								// it with the straight segment;
								edit_shape.RemoveVertex(iprev, true);
							}
							gap_counter = 0;
						}
						pt_before.SetCoords(pt_current);
						ibefore = ivert;
					}
					pt_current.SetCoords(pt_after);
					iprev = ivert;
					ivert = inext;
					// reload = false;
					inext = dir > 0 ? edit_shape.GetNextVertex(ivert) : edit_shape.GetPrevVertex(ivert);
					iter++;
					reload = false;
				}
				if (gap_counter > 0)
				{
					int p = dir > 0 ? edit_shape.GetPrevVertex(iprev) : edit_shape.GetNextVertex(iprev);
					for (int i = 1; i < gap_counter; i++)
					{
						int pp = dir > 0 ? edit_shape.GetPrevVertex(p) : edit_shape.GetNextVertex(p);
						edit_shape.RemoveVertex(p, true);
						p = pp;
					}
					pt_middle.Add(pt_before, pt_current);
					pt_middle.Scale(0.5);
					v_gap.Sub(pt_current, pt_before);
					double gap_length = v_gap.Length();
					double sqr_delta = abs_d * abs_d - gap_length * gap_length * 0.25;
					System.Diagnostics.Debug.Assert((sqr_delta > 0));
					double delta = System.Math.Sqrt(sqr_delta);
					v_gap.Normalize();
					v_gap.RightPerpendicular();
					temp.SetCoords(v_gap);
					temp.Scale(abs_d - delta);
					pt_middle.Add(temp);
					edit_shape.SetXY(iprev, pt_middle);
				}
				edit_shape.FilterClosePoints(m_filter_tolerance, false, false);
				if (!b_filtered)
				{
					break;
				}
			}
			return 1;
		}

		private bool IsDegeneratePath_(com.epl.geometry.MultiPathImpl mp_impl, int ipath)
		{
			if (mp_impl.GetPathSize(ipath) == 1)
			{
				return true;
			}
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			mp_impl.QueryPathEnvelope2D(ipath, env);
			if (System.Math.Max(env.GetWidth(), env.GetHeight()) < m_densify_dist * 0.5)
			{
				return true;
			}
			return false;
		}

		private bool IsDegenerateGeometry_(com.epl.geometry.Geometry geom)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			geom.QueryEnvelope2D(env);
			if (System.Math.Max(env.GetWidth(), env.GetHeight()) < m_densify_dist * 0.5)
			{
				return true;
			}
			return false;
		}

		private com.epl.geometry.Polyline PreparePolyline_(com.epl.geometry.Polyline input_geom)
		{
			// Generalize it firstly using 25% of the densification deviation as a
			// criterion.
			com.epl.geometry.Polyline generalized_polyline = (com.epl.geometry.Polyline)((com.epl.geometry.OperatorGeneralize)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Generalize)).Execute(input_geom, m_densify_dist * 0.25, false, 
				m_progress_tracker);
			int path_point_count = 0;
			for (int i = 0, npath = generalized_polyline.GetPathCount(); i < npath; i++)
			{
				path_point_count = System.Math.Max(generalized_polyline.GetPathSize(i), path_point_count);
			}
			if (path_point_count < 32)
			{
				m_bfilter = false;
				return generalized_polyline;
			}
			else
			{
				m_bfilter = true;
				// If we apply a filter to the polyline, then we have to resolve all
				// self intersections.
				com.epl.geometry.Polyline simple_polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TopologicalOperations.PlanarSimplify(generalized_polyline, m_small_tolerance, false, true, m_progress_tracker));
				// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/buffer_simplify.txt",
				// *simple_polyline, nullptr);
				return simple_polyline;
			}
		}

		private void AddCircle_(com.epl.geometry.MultiPathImpl result_mp, com.epl.geometry.Point point)
		{
			// Uses same calculations for each of the quadrants, generating a
			// symmetric distribution of points.
			com.epl.geometry.Point2D center = point.GetXY();
			if (m_circle_template != null && !(m_circle_template.Count == 0))
			{
				// use
				// template
				// if
				// available.
				com.epl.geometry.Point2D p = new com.epl.geometry.Point2D();
				p.SetCoords(m_circle_template[0]);
				p.ScaleAdd(m_abs_distance, center);
				result_mp.StartPath(p);
				for (int i = 1, n = (int)m_circle_template.Count; i < n; i++)
				{
					p.SetCoords(m_circle_template[i]);
					p.ScaleAdd(m_abs_distance, center);
					result_mp.LineTo(p);
				}
				return;
			}
			// avoid unnecessary memory allocation for the circle template. Just do
			// the point here.
			int N = CalcN_(4);
			int real_size = (N + 3) / 4;
			double dA = (System.Math.PI * 0.5) / real_size;
			// result_mp.reserve(real_size * 4);
			double dcos = System.Math.Cos(dA);
			double dsin = System.Math.Sin(dA);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int quadrant = 3; quadrant >= 0; quadrant--)
			{
				pt.SetCoords(0.0, m_abs_distance);
				switch (quadrant)
				{
					case 0:
					{
						// upper left quadrant
						for (int i = 0; i < real_size; i++)
						{
							result_mp.LineTo(pt.x + center.x, pt.y + center.y);
							pt.RotateReverse(dcos, dsin);
						}
						break;
					}

					case 1:
					{
						// upper left quadrant
						for (int i = 0; i < real_size; i++)
						{
							// m_circle_template.set(i
							// + real_size * 1,
							// Point_2D::construct(-pt.y,
							// pt.x));
							result_mp.LineTo(-pt.y + center.x, pt.x + center.y);
							pt.RotateReverse(dcos, dsin);
						}
						break;
					}

					case 2:
					{
						// lower left quadrant
						// m_circle_template.set(i + real_size * 2,
						// Point_2D::construct(-pt.x, -pt.y));
						for (int i = 0; i < real_size; i++)
						{
							result_mp.LineTo(-pt.x + center.x, -pt.y + center.y);
							pt.RotateReverse(dcos, dsin);
						}
						break;
					}

					default:
					{
						// case 3:
						// lower right quadrant
						// m_circle_template.set(i + real_size * 3,
						// Point_2D::construct(pt.y, -pt.x));
						result_mp.StartPath(pt.y + center.x, -pt.x + center.y);
						// we
						// start
						// at
						// the
						// quadrant
						// 3.
						// The
						// first
						// point
						// is
						// (0,
						// -m_distance)
						// +
						// center
						for (int i = 1; i < real_size; i++)
						{
							pt.RotateReverse(dcos, dsin);
							result_mp.LineTo(pt.y + center.x, -pt.x + center.y);
						}
						break;
					}
				}
				Progress_();
			}
		}

		private static com.epl.geometry.Polygon SetWeakSimple_(com.epl.geometry.Polygon poly)
		{
			((com.epl.geometry.MultiPathImpl)poly._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, 0.0, false);
			return poly;
		}

		private com.epl.geometry.Polygon SetStrongSimple_(com.epl.geometry.Polygon poly)
		{
			((com.epl.geometry.MultiPathImpl)poly._getImpl()).SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong, m_tolerance, false);
			((com.epl.geometry.MultiPathImpl)poly._getImpl())._updateOGCFlags();
			return poly;
		}
	}
}
