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
/*
* To change this template, choose Tools | Templates
* and open the template in the editor.
*/


namespace com.epl.geometry
{
	internal class SegmentIntersector
	{
		internal class IntersectionPart
		{
			public com.epl.geometry.Segment seg;

			public double weight_start;

			public double weight_end;

			public int rank_start;

			public int rank_end;

			public int rank_interior;

			internal IntersectionPart(com.epl.geometry.Segment _seg)
			{
				// Weight controls the snapping. When points of the same rank are
				// snapped together,
				// The new posistion is calculated as a weighted average.
				// The rank controls the snapping. The point with lower rank will be
				// snapped to the point with the higher rank.
				// the rank of the start point
				// the rank of the end point
				// the rank of the interior point
				seg = _seg;
				weight_start = 1.0;
				weight_end = 1.0;
				rank_start = 0;
				rank_end = 0;
				rank_interior = 0;
			}
		}

		private System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart> m_input_segments;

		private System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart> m_result_segments_1;

		private System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart> m_result_segments_2;

		private System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart> m_recycled_intersection_parts;

		private System.Collections.Generic.List<com.epl.geometry.SegmentBuffer> m_recycled_segments;

		private double[] m_param_1 = new double[15];

		private double[] m_param_2 = new double[15];

		private com.epl.geometry.Point m_point = new com.epl.geometry.Point();

		private int m_used_recycled_segments;

		// typedef std::shared_ptr<Segment_buffer> segment_buffer_sptr;
		// typedef std::shared_ptr<Intersection_part> intersection_part_sptr;
		// typedef Dynamic_array<intersection_part_sptr> intersection_parts;
		private void Recycle_(System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart> parts)
		{
			if (parts == null)
			{
				return;
			}
			for (int i = 0, n = (int)parts.Count; i < n; i++)
			{
				Recycle_(parts[i]);
			}
			parts.Clear();
		}

		private void Recycle_(com.epl.geometry.SegmentIntersector.IntersectionPart part)
		{
			part.seg = null;
			m_recycled_intersection_parts.Add(part);
		}

		private com.epl.geometry.SegmentIntersector.IntersectionPart NewIntersectionPart_(com.epl.geometry.Segment _seg)
		{
			if ((m_recycled_intersection_parts.Count == 0))
			{
				com.epl.geometry.SegmentIntersector.IntersectionPart part = new com.epl.geometry.SegmentIntersector.IntersectionPart(_seg);
				return part;
			}
			else
			{
				com.epl.geometry.SegmentIntersector.IntersectionPart part = m_recycled_intersection_parts[m_recycled_intersection_parts.Count - 1];
				part.seg = _seg;
				m_recycled_intersection_parts.RemoveAt(m_recycled_intersection_parts.Count - 1);
				return part;
			}
		}

		private com.epl.geometry.SegmentIntersector.IntersectionPart GetResultPart_(int input_segment_index, int segment_index)
		{
			if (input_segment_index == 0)
			{
				return m_result_segments_1[segment_index];
			}
			else
			{
				System.Diagnostics.Debug.Assert((input_segment_index == 1));
				return m_result_segments_2[segment_index];
			}
		}

		private com.epl.geometry.SegmentBuffer NewSegmentBuffer_()
		{
			if (m_used_recycled_segments >= m_recycled_segments.Count)
			{
				m_recycled_segments.Add(new com.epl.geometry.SegmentBuffer());
			}
			com.epl.geometry.SegmentBuffer p = m_recycled_segments[m_used_recycled_segments];
			m_used_recycled_segments++;
			return p;
		}

		private double m_tolerance;

		public SegmentIntersector()
		{
			m_used_recycled_segments = 0;
			m_tolerance = 0;
			m_input_segments = new System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart>();
			m_result_segments_1 = new System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart>();
			m_result_segments_2 = new System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart>();
			m_recycled_intersection_parts = new System.Collections.Generic.List<com.epl.geometry.SegmentIntersector.IntersectionPart>();
			m_recycled_segments = new System.Collections.Generic.List<com.epl.geometry.SegmentBuffer>();
		}

		// Clears the results and input segments
		public virtual void Clear()
		{
			Recycle_(m_input_segments);
			Recycle_(m_result_segments_1);
			Recycle_(m_result_segments_2);
			m_used_recycled_segments = 0;
		}

		// Adds a segment to intersect and returns an index of the segment.
		// Two segments has to be pushed for the intersect method to succeed.
		public virtual int PushSegment(com.epl.geometry.Segment seg)
		{
			System.Diagnostics.Debug.Assert((m_input_segments.Count < 2));
			m_input_segments.Add(NewIntersectionPart_(seg));
			// m_param_1.resize(15);
			// m_param_2.resize(15);
			return (int)m_input_segments.Count - 1;
		}

		public virtual void SetRankAndWeight(int input_segment_index, double start_weight, int start_rank, double end_weight, int end_rank, int interior_rank)
		{
			com.epl.geometry.SegmentIntersector.IntersectionPart part = m_input_segments[input_segment_index];
			part.rank_end = end_rank;
			part.weight_start = start_weight;
			part.weight_end = end_weight;
			part.rank_start = start_rank;
			part.rank_end = end_rank;
			part.rank_interior = interior_rank;
		}

		// Returns the number of segments the input segment has been split to.
		public virtual int GetResultSegmentCount(int input_segment_index)
		{
			if (input_segment_index == 0)
			{
				return (int)m_result_segments_1.Count;
			}
			else
			{
				System.Diagnostics.Debug.Assert((input_segment_index == 1));
				return (int)m_result_segments_2.Count;
			}
		}

		// Returns a part of the input segment that is the result of the
		// intersection with another segment.
		// input_segment_index is the index of the input segment.
		// segment_index is between 0 and
		// get_result_segment_count(input_segment_index) - 1
		public virtual com.epl.geometry.Segment GetResultSegment(int input_segment_index, int segment_index)
		{
			return GetResultPart_(input_segment_index, segment_index).seg;
		}

		// double get_result_segment_start_point_weight(int input_segment_index, int
		// segment_index);
		// int get_result_segment_start_point_rank(int input_segment_index, int
		// segment_index);
		// double get_result_segment_end_point_weight(int input_segment_index, int
		// segment_index);
		// int get_result_segment_end_point_rank(int input_segment_index, int
		// segment_index);
		// int get_result_segment_interior_rank(int input_segment_index, int
		// segment_index);
		public virtual com.epl.geometry.Point GetResultPoint()
		{
			return m_point;
		}

		// Performs the intersection
		public virtual bool Intersect(double tolerance, bool b_intersecting)
		{
			if (m_input_segments.Count != 2)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			m_tolerance = tolerance;
			double small_tolerance_sqr = com.epl.geometry.MathUtils.Sqr(tolerance * 0.01);
			bool bigmove = false;
			com.epl.geometry.SegmentIntersector.IntersectionPart part1 = m_input_segments[0];
			com.epl.geometry.SegmentIntersector.IntersectionPart part2 = m_input_segments[1];
			if (b_intersecting || (part1.seg._isIntersecting(part2.seg, tolerance, true) & 5) != 0)
			{
				if (part1.seg.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Line)
				{
					com.epl.geometry.Line line_1 = (com.epl.geometry.Line)part1.seg;
					if (part2.seg.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Line)
					{
						com.epl.geometry.Line line_2 = (com.epl.geometry.Line)part2.seg;
						int count = com.epl.geometry.Line._intersectLineLine(line_1, line_2, null, m_param_1, m_param_2, tolerance);
						if (count == 0)
						{
							System.Diagnostics.Debug.Assert((count > 0));
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
						com.epl.geometry.Point2D[] points = new com.epl.geometry.Point2D[9];
						for (int i = 0; i < count; i++)
						{
							// For each point of intersection, we calculate a
							// weighted point
							// based on the ranks and weights of the endpoints and
							// the interior.
							double t1 = m_param_1[i];
							double t2 = m_param_2[i];
							int rank1 = part1.rank_interior;
							double weight1 = 1.0;
							if (t1 == 0)
							{
								rank1 = part1.rank_start;
								weight1 = part1.weight_start;
							}
							else
							{
								if (t1 == 1.0)
								{
									rank1 = part1.rank_end;
									weight1 = part1.weight_end;
								}
							}
							int rank2 = part2.rank_interior;
							double weight2 = 1.0;
							if (t2 == 0)
							{
								rank2 = part2.rank_start;
								weight2 = part2.weight_start;
							}
							else
							{
								if (t2 == 1.0)
								{
									rank2 = part2.rank_end;
									weight2 = part2.weight_end;
								}
							}
							double ptWeight;
							com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
							if (rank1 == rank2)
							{
								// for equal ranks use weighted sum
								com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
								line_1.GetCoord2D(t1, pt_1);
								com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
								line_2.GetCoord2D(t2, pt_2);
								ptWeight = weight1 + weight2;
								double t = weight2 / ptWeight;
								com.epl.geometry.MathUtils.Lerp(pt_1, pt_2, t, pt);
								if (com.epl.geometry.Point2D.SqrDistance(pt, pt_1) + com.epl.geometry.Point2D.SqrDistance(pt, pt_2) > small_tolerance_sqr)
								{
									bigmove = true;
								}
							}
							else
							{
								// for non-equal ranks, the higher rank wins
								if (rank1 > rank2)
								{
									line_1.GetCoord2D(t1, pt);
									ptWeight = weight1;
									com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
									line_2.GetCoord2D(t2, pt_2);
									if (com.epl.geometry.Point2D.SqrDistance(pt, pt_2) > small_tolerance_sqr)
									{
										bigmove = true;
									}
								}
								else
								{
									line_2.GetCoord2D(t2, pt);
									ptWeight = weight2;
									com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
									line_1.GetCoord2D(t1, pt_1);
									if (com.epl.geometry.Point2D.SqrDistance(pt, pt_1) > small_tolerance_sqr)
									{
										bigmove = true;
									}
								}
							}
							points[i] = pt;
						}
						// Split the line_1, making sure the endpoints are adusted
						// to the weighted
						double t0 = 0;
						int i0 = -1;
						for (int i_1 = 0; i_1 <= count; i_1++)
						{
							double t = i_1 < count ? m_param_1[i_1] : 1.0;
							if (t != t0)
							{
								com.epl.geometry.SegmentBuffer seg_buffer = NewSegmentBuffer_();
								line_1.Cut(t0, t, seg_buffer);
								if (i0 != -1)
								{
									seg_buffer.Get().SetStartXY(points[i0]);
								}
								if (i_1 != count)
								{
									seg_buffer.Get().SetEndXY(points[i_1]);
								}
								t0 = t;
								m_result_segments_1.Add(NewIntersectionPart_(seg_buffer.Get()));
							}
							i0 = i_1;
						}
						int[] indices = new int[9];
						for (int i_2 = 0; i_2 < count; i_2++)
						{
							indices[i_2] = i_2;
						}
						if (count > 1)
						{
							if (m_param_2[0] > m_param_2[1])
							{
								double t = m_param_2[0];
								m_param_2[0] = m_param_2[1];
								m_param_2[1] = t;
								int i_3 = indices[0];
								indices[0] = indices[1];
								indices[1] = i_3;
							}
						}
						// Split the line_2
						t0 = 0;
						i0 = -1;
						for (int i_4 = 0; i_4 <= count; i_4++)
						{
							double t = i_4 < count ? m_param_2[i_4] : 1.0;
							if (t != t0)
							{
								com.epl.geometry.SegmentBuffer seg_buffer = NewSegmentBuffer_();
								line_2.Cut(t0, t, seg_buffer);
								if (i0 != -1)
								{
									int ind = indices[i0];
									seg_buffer.Get().SetStartXY(points[ind]);
								}
								if (i_4 != count)
								{
									int ind = indices[i_4];
									seg_buffer.Get().SetEndXY(points[ind]);
								}
								t0 = t;
								m_result_segments_2.Add(NewIntersectionPart_(seg_buffer.Get()));
							}
							i0 = i_4;
						}
						return bigmove;
					}
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			return false;
		}

		public virtual void Intersect(double tolerance, com.epl.geometry.Point pt_intersector_point, int point_rank, double point_weight, bool b_intersecting)
		{
			pt_intersector_point.CopyTo(m_point);
			if (m_input_segments.Count != 1)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			m_tolerance = tolerance;
			com.epl.geometry.SegmentIntersector.IntersectionPart part1 = m_input_segments[0];
			if (b_intersecting || part1.seg._isIntersectingPoint(pt_intersector_point.GetXY(), tolerance, true))
			{
				if (part1.seg.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Line)
				{
					com.epl.geometry.Line line_1 = (com.epl.geometry.Line)(part1.seg);
					double t1 = line_1.GetClosestCoordinate(pt_intersector_point.GetXY(), false);
					m_param_1[0] = t1;
					// For each point of intersection, we calculate a weighted point
					// based on the ranks and weights of the endpoints and the
					// interior.
					int rank1 = part1.rank_interior;
					double weight1 = 1.0;
					if (t1 == 0)
					{
						rank1 = part1.rank_start;
						weight1 = part1.weight_start;
					}
					else
					{
						if (t1 == 1.0)
						{
							rank1 = part1.rank_end;
							weight1 = part1.weight_end;
						}
					}
					int rank2 = point_rank;
					double weight2 = point_weight;
					double ptWeight;
					com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
					if (rank1 == rank2)
					{
						// for equal ranks use weighted sum
						com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
						line_1.GetCoord2D(t1, pt_1);
						com.epl.geometry.Point2D pt_2 = pt_intersector_point.GetXY();
						ptWeight = weight1 + weight2;
						double t = weight2 / ptWeight;
						com.epl.geometry.MathUtils.Lerp(pt_1, pt_2, t, pt);
					}
					else
					{
						// for non-equal ranks, the higher rank wins
						if (rank1 > rank2)
						{
							pt = new com.epl.geometry.Point2D();
							line_1.GetCoord2D(t1, pt);
							ptWeight = weight1;
						}
						else
						{
							pt = pt_intersector_point.GetXY();
							ptWeight = weight2;
						}
					}
					// Split the line_1, making sure the endpoints are adusted to
					// the weighted
					double t0 = 0;
					int i0 = -1;
					int count = 1;
					for (int i = 0; i <= count; i++)
					{
						double t = i < count ? m_param_1[i] : 1.0;
						if (t != t0)
						{
							com.epl.geometry.SegmentBuffer seg_buffer = NewSegmentBuffer_();
							line_1.Cut(t0, t, seg_buffer);
							if (i0 != -1)
							{
								seg_buffer.Get().SetStartXY(pt);
							}
							if (i != count)
							{
								seg_buffer.Get().SetEndXY(pt);
							}
							t0 = t;
							m_result_segments_1.Add(NewIntersectionPart_(seg_buffer.Get()));
						}
						i0 = i;
					}
					m_point.SetXY(pt);
					return;
				}
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
		}

		public virtual double Get_tolerance_()
		{
			return m_tolerance;
		}
	}
}
