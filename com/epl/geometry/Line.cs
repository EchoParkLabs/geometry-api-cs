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
	/// <summary>A straight line between a pair of points.</summary>
	[System.Serializable]
	public sealed class Line : com.epl.geometry.Segment
	{
		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.Line;
		}

		public override double CalculateLength2D()
		{
			double dx = m_xStart - m_xEnd;
			double dy = m_yStart - m_yEnd;
			return System.Math.Sqrt(dx * dx + dy * dy);
		}

		internal override bool IsDegenerate(double tolerance)
		{
			double dx = m_xStart - m_xEnd;
			double dy = m_yStart - m_yEnd;
			return System.Math.Sqrt(dx * dx + dy * dy) <= tolerance;
		}

		/// <summary>Indicates if the line segment is a curve.</summary>
		internal override bool IsCurve()
		{
			return false;
		}

		internal override com.epl.geometry.Point2D _getTangent(double t)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.Sub(GetEndXY(), GetStartXY());
			return pt;
		}

		internal override bool _isDegenerate(double tolerance)
		{
			return CalculateLength2D() <= tolerance;
		}

		/// <summary>Creates a line segment.</summary>
		public Line()
		{
			// HEADER DEF
			// Cpp
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
		}

		internal Line(com.epl.geometry.VertexDescription vd)
		{
			m_description = vd;
		}

		public Line(double x1, double y1, double x2, double y2)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			SetStartXY(x1, y1);
			SetEndXY(x2, y2);
		}

		public override void QueryEnvelope(com.epl.geometry.Envelope env)
		{
			env.SetEmpty();
			env.AssignVertexDescription(m_description);
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			QueryEnvelope2D(env2D);
			env.SetEnvelope2D(env2D);
			for (int i = 1, n = m_description.GetAttributeCount(); i < n; i++)
			{
				int semantics = m_description.GetSemantics(i);
				for (int iord = 0, nord = com.epl.geometry.VertexDescription.GetComponentCount(semantics); i < nord; i++)
				{
					com.epl.geometry.Envelope1D interval = QueryInterval(semantics, iord);
					env.SetInterval(semantics, iord, interval);
				}
			}
		}

		public override void QueryEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			env.SetCoords(m_xStart, m_yStart, m_xEnd, m_yEnd);
			env.Normalize();
		}

		internal override void QueryEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			env.SetEmpty();
			env.Merge(m_xStart, m_yStart, _getAttributeAsDbl(0, com.epl.geometry.VertexDescription.Semantics.Z, 0));
			env.Merge(m_xEnd, m_yEnd, _getAttributeAsDbl(1, com.epl.geometry.VertexDescription.Semantics.Z, 0));
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			_touch();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.x = m_xStart;
			pt.y = m_yStart;
			transform.Transform(pt, pt);
			m_xStart = pt.x;
			m_yStart = pt.y;
			pt.x = m_xEnd;
			pt.y = m_yEnd;
			transform.Transform(pt, pt);
			m_xEnd = pt.x;
			m_yEnd = pt.y;
		}

		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			_touch();
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.x = m_xStart;
			pt.y = m_yStart;
			pt.z = _getAttributeAsDbl(0, com.epl.geometry.VertexDescription.Semantics.Z, 0);
			pt = transform.Transform(pt);
			m_xStart = pt.x;
			m_yStart = pt.y;
			_setAttribute(0, com.epl.geometry.VertexDescription.Semantics.Z, 0, pt.z);
			pt.x = m_xEnd;
			pt.y = m_yEnd;
			pt.z = _getAttributeAsDbl(1, com.epl.geometry.VertexDescription.Semantics.Z, 0);
			pt = transform.Transform(pt);
			m_xEnd = pt.x;
			m_yEnd = pt.y;
			_setAttribute(1, com.epl.geometry.VertexDescription.Semantics.Z, 0, pt.z);
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.Line(m_description);
		}

		internal override double _calculateArea2DHelper(double xorg, double yorg)
		{
			return ((m_xEnd - xorg) - (m_xStart - xorg)) * ((m_yEnd - yorg) + (m_yStart - yorg)) * 0.5;
		}

		internal override double TToLength(double t)
		{
			return t * CalculateLength2D();
		}

		internal override double LengthToT(double len)
		{
			return len / CalculateLength2D();
		}

		internal double GetCoordX_(double t)
		{
			// Must match query_coord_2D and vice verse
			// Also match get_attribute_as_dbl
			return com.epl.geometry.MathUtils.Lerp(m_xStart, m_xEnd, t);
		}

		internal double GetCoordY_(double t)
		{
			// Must match query_coord_2D and vice verse
			// Also match get_attribute_as_dbl
			return com.epl.geometry.MathUtils.Lerp(m_yStart, m_yEnd, t);
		}

		public override void GetCoord2D(double t, com.epl.geometry.Point2D pt)
		{
			// We want:
			// 1. When t == 0, get exactly Start
			// 2. When t == 1, get exactly End
			// 3. When m_x_end == m_x_start, we want m_x_start exactly
			// 4. When m_y_end == m_y_start, we want m_y_start exactly
			com.epl.geometry.MathUtils.Lerp(m_xStart, m_yStart, m_xEnd, m_yEnd, t, pt);
		}

		public override com.epl.geometry.Segment Cut(double t1, double t2)
		{
			com.epl.geometry.SegmentBuffer segmentBuffer = new com.epl.geometry.SegmentBuffer();
			Cut(t1, t2, segmentBuffer);
			return segmentBuffer.Get();
		}

		internal override void Cut(double t1, double t2, com.epl.geometry.SegmentBuffer subSegmentBuffer)
		{
			if (subSegmentBuffer == null)
			{
				throw new System.ArgumentException();
			}
			subSegmentBuffer.CreateLine();
			// Make sure buffer contains Line class.
			com.epl.geometry.Segment subSegment = subSegmentBuffer.Get();
			subSegment.AssignVertexDescription(m_description);
			com.epl.geometry.Point2D point = new com.epl.geometry.Point2D();
			GetCoord2D(t1, point);
			subSegment.SetStartXY(point.x, point.y);
			GetCoord2D(t2, point);
			subSegment.SetEndXY(point.x, point.y);
			for (int iattr = 1, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int ordinate = 0; ordinate < ncomps; ordinate++)
				{
					double value1 = GetAttributeAsDbl(t1, semantics, ordinate);
					subSegment.SetStartAttribute(semantics, ordinate, value1);
					double value2 = GetAttributeAsDbl(t2, semantics, ordinate);
					subSegment.SetEndAttribute(semantics, ordinate, value2);
				}
			}
		}

		public override double GetAttributeAsDbl(double t, int semantics, int ordinate)
		{
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				return ordinate == 0 ? GetCoord2D(t).x : GetCoord2D(t).y;
			}
			int interpolation = com.epl.geometry.VertexDescription.GetInterpolation(semantics);
			switch (interpolation)
			{
				case com.epl.geometry.VertexDescription.Interpolation.NONE:
				{
					if (t < 0.5)
					{
						return GetStartAttributeAsDbl(semantics, ordinate);
					}
					else
					{
						return GetEndAttributeAsDbl(semantics, ordinate);
					}
					goto case com.epl.geometry.VertexDescription.Interpolation.LINEAR;
				}

				case com.epl.geometry.VertexDescription.Interpolation.LINEAR:
				{
					double s = GetStartAttributeAsDbl(semantics, ordinate);
					double e = GetEndAttributeAsDbl(semantics, ordinate);
					return com.epl.geometry.MathUtils.Lerp(s, e, t);
				}

				case com.epl.geometry.VertexDescription.Interpolation.ANGULAR:
				{
					throw new com.epl.geometry.GeometryException("not implemented");
				}
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		public override double GetClosestCoordinate(com.epl.geometry.Point2D inputPt, bool bExtrapolate)
		{
			double vx = m_xEnd - m_xStart;
			double vy = m_yEnd - m_yStart;
			double v2 = vx * vx + vy * vy;
			if (v2 == 0)
			{
				return 0.5;
			}
			double rx = inputPt.x - m_xStart;
			double ry = inputPt.y - m_yStart;
			double t = (rx * vx + ry * vy) / v2;
			if (!bExtrapolate)
			{
				if (t < 0.0)
				{
					t = 0.0;
				}
				else
				{
					if (t > 1.0)
					{
						t = 1.0;
					}
				}
			}
			return t;
		}

		public override int IntersectionWithAxis2D(bool b_axis_x, double ordinate, double[] result_ordinates, double[] parameters)
		{
			if (b_axis_x)
			{
				double a = (m_yEnd - m_yStart);
				if (a == 0)
				{
					return (ordinate == m_yEnd) ? -1 : 0;
				}
				double t = (ordinate - m_yStart) / a;
				if (t < 0.0 || t > 1.0)
				{
					return 0;
				}
				if (result_ordinates != null)
				{
					(result_ordinates)[0] = GetCoordX_(t);
				}
				if (parameters != null)
				{
					(parameters)[0] = t;
				}
				return 1;
			}
			else
			{
				double a = (m_xEnd - m_xStart);
				if (a == 0)
				{
					return (ordinate == m_xEnd) ? -1 : 0;
				}
				double t = (ordinate - m_xStart) / a;
				if (t < 0.0 || t > 1.0)
				{
					return 0;
				}
				if (result_ordinates != null)
				{
					(result_ordinates)[0] = GetCoordY_(t);
				}
				if (parameters != null)
				{
					(parameters)[0] = t;
				}
				return 1;
			}
		}

		// line segment can have 0 or 1 intersection interval with clipEnv2D.
		// The function return 0 or 2 segParams (e.g. 0.0, 0.4; or 0.1, 0.9; or 0.6,
		// 1.0; or 0.0, 1.0)
		// segParams will be sorted in ascending order; the order of the
		// envelopeDistances will correspond (i.e. the envelopeDistances may not be
		// in ascending order);
		// an envelopeDistance can be -1.0 if the corresponding endpoint is properly
		// inside clipEnv2D.
		internal int IntersectionWithEnvelope2D(com.epl.geometry.Envelope2D clipEnv2D, bool includeEnvBoundary, double[] segParams, double[] envelopeDistances)
		{
			com.epl.geometry.Point2D p1 = GetStartXY();
			com.epl.geometry.Point2D p2 = GetEndXY();
			// includeEnvBoundary xxx ???
			int modified = clipEnv2D.ClipLine(p1, p2, 0, segParams, envelopeDistances);
			return modified != 0 ? 2 : 0;
		}

		internal override double IntersectionOfYMonotonicWithAxisX(double y, double x_parallel)
		{
			double a = (m_yEnd - m_yStart);
			if (a == 0)
			{
				return (y == m_yEnd) ? x_parallel : com.epl.geometry.NumberUtils.NaN();
			}
			double t = (y - m_yStart) / a;
			System.Diagnostics.Debug.Assert((t >= 0 && t <= 1.0));
			// double t_1 = 1.0 - t;
			// assert(t + t_1 == 1.0);
			double resx = GetCoordX_(t);
			if (t == 1.0)
			{
				resx = m_xEnd;
			}
			System.Diagnostics.Debug.Assert(((resx >= m_xStart && resx <= m_xEnd) || (resx <= m_xStart && resx >= m_xEnd)));
			return resx;
		}

		internal override bool _isIntersectingPoint(com.epl.geometry.Point2D pt, double tolerance, bool bExcludeExactEndpoints)
		{
			return _intersection(pt, tolerance, bExcludeExactEndpoints) >= 0;
		}

		// must
		// use
		// same
		// method
		// that
		// the
		// intersection
		// routine
		// uses.
		/// <summary>
		/// Returns True if point and the segment intersect (not disjoint) for the
		/// given tolerance.
		/// </summary>
		public override bool IsIntersecting(com.epl.geometry.Point2D pt, double tolerance)
		{
			return _isIntersectingPoint(pt, tolerance, false);
		}

		internal void OrientBottomUp_()
		{
			if (m_yEnd < m_yStart || (m_yEnd == m_yStart && m_xEnd < m_xStart))
			{
				double x = m_xStart;
				m_xStart = m_xEnd;
				m_xEnd = x;
				double y = m_yStart;
				m_yStart = m_yEnd;
				m_yEnd = y;
				for (int i = 0, n = m_description.GetTotalComponentCount() - 2; i < n; i++)
				{
					double a = m_attributes[i];
					m_attributes[i] = m_attributes[i + n];
					m_attributes[i + n] = a;
				}
			}
		}

		// return -1 for the left side from the infinite line passing through thais
		// Line, 1 for the right side of the line, 0 if on the line (in the bounds
		// of the roundoff error)
		internal int _side(com.epl.geometry.Point2D pt)
		{
			return _side(pt.x, pt.y);
		}

		// return -1 for the left side from the infinite line passing through thais
		// Line, 1 for the right side of the line, 0 if on the line (in the bounds
		// of the roundoff error)
		internal int _side(double ptX, double ptY)
		{
			com.epl.geometry.Point2D v1 = new com.epl.geometry.Point2D(ptX, ptY);
			v1.Sub(GetStartXY());
			com.epl.geometry.Point2D v2 = new com.epl.geometry.Point2D();
			v2.Sub(GetEndXY(), GetStartXY());
			double cross = v2.CrossProduct(v1);
			double crossError = 4 * com.epl.geometry.NumberUtils.DoubleEps() * (System.Math.Abs(v2.x * v1.y) + System.Math.Abs(v2.y * v1.x));
			return cross > crossError ? -1 : cross < -crossError ? 1 : 0;
		}

		internal double _intersection(com.epl.geometry.Point2D pt, double tolerance, bool bExcludeExactEndPoints)
		{
			com.epl.geometry.Point2D v = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D start = new com.epl.geometry.Point2D();
			// Test start point distance to pt.
			start.SetCoords(m_xStart, m_yStart);
			v.Sub(pt, start);
			double vlength = v.Length();
			double vLengthError = vlength * 3 * com.epl.geometry.NumberUtils.DoubleEps();
			if (vlength <= System.Math.Max(tolerance, vLengthError))
			{
				System.Diagnostics.Debug.Assert((vlength != 0 || pt.IsEqual(start)));
				// probably never asserts
				if (bExcludeExactEndPoints && vlength == 0)
				{
					return com.epl.geometry.NumberUtils.TheNaN;
				}
				else
				{
					return 0;
				}
			}
			com.epl.geometry.Point2D end2D = GetEndXY();
			// Test end point distance to pt.
			v.Sub(pt, end2D);
			vlength = v.Length();
			vLengthError = vlength * 3 * com.epl.geometry.NumberUtils.DoubleEps();
			if (vlength <= System.Math.Max(tolerance, vLengthError))
			{
				System.Diagnostics.Debug.Assert((vlength != 0 || pt.IsEqual(end2D)));
				// probably never asserts
				if (bExcludeExactEndPoints && vlength == 0)
				{
					return com.epl.geometry.NumberUtils.TheNaN;
				}
				else
				{
					return 1.0;
				}
			}
			// Find a distance from the line to pt.
			v.SetCoords(m_xEnd - m_xStart, m_yEnd - m_yStart);
			double len = v.Length();
			if (len > 0)
			{
				double invertedLength = 1.0 / len;
				v.Scale(invertedLength);
				com.epl.geometry.Point2D relativePoint = new com.epl.geometry.Point2D();
				relativePoint.Sub(pt, start);
				double projection = relativePoint.DotProduct(v);
				double projectionError = 8 * relativePoint._dotProductAbs(v) * com.epl.geometry.NumberUtils.DoubleEps();
				// See Error Estimation Rules In
				// Borg.docx
				v.LeftPerpendicular();
				// get left normal to v
				double distance = relativePoint.DotProduct(v);
				double distanceError = 8 * relativePoint._dotProductAbs(v) * com.epl.geometry.NumberUtils.DoubleEps();
				// See Error Estimation Rules In
				// Borg.docx
				double perror = System.Math.Max(tolerance, projectionError);
				if (projection < -perror || projection > len + perror)
				{
					return com.epl.geometry.NumberUtils.TheNaN;
				}
				double merror = System.Math.Max(tolerance, distanceError);
				if (System.Math.Abs(distance) <= merror)
				{
					double t = projection * invertedLength;
					t = com.epl.geometry.NumberUtils.Snap(t, 0.0, 1.0);
					com.epl.geometry.Point2D ptOnLine = new com.epl.geometry.Point2D();
					GetCoord2D(t, ptOnLine);
					if (com.epl.geometry.Point2D.Distance(ptOnLine, pt) <= tolerance)
					{
						if (t < 0.5)
						{
							if (com.epl.geometry.Point2D.Distance(ptOnLine, start) <= tolerance)
							{
								// the
								// projected
								// point
								// is
								// close
								// to
								// the
								// start
								// point.
								// Need
								// to
								// return
								// 0.
								return 0;
							}
						}
						else
						{
							if (com.epl.geometry.Point2D.Distance(ptOnLine, end2D) <= tolerance)
							{
								// the
								// projected
								// point
								// is
								// close
								// to
								// the
								// end
								// point.
								// Need
								// to
								// return
								// 1.0.
								return 1.0;
							}
						}
						return t;
					}
				}
			}
			return com.epl.geometry.NumberUtils.TheNaN;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (((Geometry)other).GetType() != GetType())
			{
				return false;
			}
			return _equalsImpl((com.epl.geometry.Segment)other);
		}

		internal bool Equals(com.epl.geometry.Line other)
		{
			if (other == this)
			{
				return true;
			}
			if (!(other is com.epl.geometry.Line))
			{
				return false;
			}
			return _equalsImpl((com.epl.geometry.Segment)other);
		}

		internal bool _projectionIntersectHelper(com.epl.geometry.Line other, com.epl.geometry.Point2D v, bool bStart)
		{
			// v is the vector in the direction of this line == end - start.
			double orgX = bStart ? m_xStart : m_xEnd;
			double orgY = bStart ? m_yStart : m_yEnd;
			com.epl.geometry.Point2D m = new com.epl.geometry.Point2D();
			m.x = other.GetEndX() - orgX;
			m.y = other.GetEndY() - orgY;
			double dot = v.DotProduct(m);
			double dotError = 3 * com.epl.geometry.NumberUtils.DoubleEps() * v._dotProductAbs(m);
			if (dot > dotError)
			{
				m.x = other.GetStartX() - orgX;
				m.y = other.GetStartY() - orgY;
				double dot2 = v.DotProduct(m);
				double dotError2 = 3 * com.epl.geometry.NumberUtils.DoubleEps() * v._dotProductAbs(m);
				return dot2 <= dotError2;
			}
			return true;
		}

		internal bool _projectionIntersect(com.epl.geometry.Line other)
		{
			// This function returns true, if the "other"'s projection on "this"
			com.epl.geometry.Point2D v = new com.epl.geometry.Point2D();
			v.x = m_xEnd - m_xStart;
			v.y = m_yEnd - m_yStart;
			if (!_projectionIntersectHelper(other, v, false))
			{
				return false;
			}
			// Both other.Start and other.End projections on
			// "this" lie to the right of the this.End
			v.Negate();
			if (!_projectionIntersectHelper(other, v, true))
			{
				return false;
			}
			// Both other.Start and other.End projections on
			// "this" lie to the left of the this.End
			return true;
		}

		// Tests if two lines intersect using projection of one line to another.
		internal static bool _isIntersectingHelper(com.epl.geometry.Line line1, com.epl.geometry.Line line2)
		{
			int s11 = line1._side(line2.m_xStart, line2.m_yStart);
			int s12 = line1._side(line2.m_xEnd, line2.m_yEnd);
			if (s11 < 0 && s12 < 0 || s11 > 0 && s12 > 0)
			{
				return false;
			}
			// no intersection. The line2 lies to one side of an
			// infinite line passing through line1
			int s21 = line2._side(line1.m_xStart, line1.m_yStart);
			int s22 = line2._side(line1.m_xEnd, line1.m_yEnd);
			if (s21 < 0 && s22 < 0 || s21 > 0 && s22 > 0)
			{
				return false;
			}
			// no intersection.The line1 lies to one side of an
			// infinite line passing through line2
			double len1 = line1.CalculateLength2D();
			double len2 = line2.CalculateLength2D();
			if (len1 > len2)
			{
				return line1._projectionIntersect(line2);
			}
			else
			{
				return line2._projectionIntersect(line1);
			}
		}

		internal static com.epl.geometry.Point2D _intersectHelper1(com.epl.geometry.Line line1, com.epl.geometry.Line line2, double tolerance)
		{
			com.epl.geometry.Point2D result = new com.epl.geometry.Point2D(com.epl.geometry.NumberUtils.NaN(), com.epl.geometry.NumberUtils.NaN());
			double k1x = line1.m_xEnd - line1.m_xStart;
			double k1y = line1.m_yEnd - line1.m_yStart;
			double k2x = line2.m_xEnd - line2.m_xStart;
			double k2y = line2.m_yEnd - line2.m_yStart;
			double det = k2x * k1y - k1x * k2y;
			if (det == 0)
			{
				return result;
			}
			// estimate roundoff error for det:
			double errdet = 4 * com.epl.geometry.NumberUtils.DoubleEps() * (System.Math.Abs(k2x * k1y) + System.Math.Abs(k1x * k2y));
			double bx = line2.m_xStart - line1.m_xStart;
			double by = line2.m_yStart - line1.m_yStart;
			double a0 = (k2x * by - bx * k2y);
			double a0error = 4 * com.epl.geometry.NumberUtils.DoubleEps() * (System.Math.Abs(k2x * by) + System.Math.Abs(bx * k2y));
			double t0 = a0 / det;
			double absdet = System.Math.Abs(det);
			double t0error = (a0error * absdet + errdet * System.Math.Abs(a0)) / (det * det) + com.epl.geometry.NumberUtils.DoubleEps() * System.Math.Abs(t0);
			if (t0 < -t0error || t0 > 1.0 + t0error)
			{
				return result;
			}
			double a1 = (k1x * by - bx * k1y);
			double a1error = 4 * com.epl.geometry.NumberUtils.DoubleEps() * (System.Math.Abs(k1x * by) + System.Math.Abs(bx * k1y));
			double t1 = a1 / det;
			double t1error = (a1error * absdet + errdet * System.Math.Abs(a1)) / (det * det) + com.epl.geometry.NumberUtils.DoubleEps() * System.Math.Abs(t1);
			if (t1 < -t1error || t1 > 1.0 + t1error)
			{
				return result;
			}
			double t0r = com.epl.geometry.NumberUtils.Snap(t0, 0.0, 1.0);
			double t1r = com.epl.geometry.NumberUtils.Snap(t1, 0.0, 1.0);
			com.epl.geometry.Point2D pt0 = line1.GetCoord2D(t0r);
			com.epl.geometry.Point2D pt1 = line2.GetCoord2D(t1r);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.Sub(pt0, pt1);
			if (pt.Length() > tolerance)
			{
				// Roundoff errors cause imprecise result. Try recalculate.
				// 1. Use averaged point and recalculate the t values
				// Point2D pt;
				pt.Add(pt0, pt1);
				pt.Scale(0.5);
				t0r = line1.GetClosestCoordinate(pt, false);
				t1r = line2.GetClosestCoordinate(pt, false);
				com.epl.geometry.Point2D pt01 = line1.GetCoord2D(t0r);
				com.epl.geometry.Point2D pt11 = line2.GetCoord2D(t1r);
				pt01.Sub(pt11);
				if (pt01.Length() > tolerance)
				{
					// Seems to be no intersection here actually. Return NaNs
					return result;
				}
			}
			result.SetCoords(t0r, t1r);
			return result;
		}

		internal static int _isIntersectingLineLine(com.epl.geometry.Line line1, com.epl.geometry.Line line2, double tolerance, bool bExcludeExactEndpoints)
		{
			// _ASSERT(line1 != line2);
			// Check for the endpoints.
			// The bExcludeExactEndpoints is True, means we care only about overlaps
			// and real intersections, but do not care if the endpoints are exactly
			// equal.
			// bExcludeExactEndpoints is used in Cracking check test, because during
			// cracking test all points are either coincident or further than the
			// tolerance.
			int counter = 0;
			if (line1.m_xStart == line2.m_xStart && line1.m_yStart == line2.m_yStart || line1.m_xStart == line2.m_xEnd && line1.m_yStart == line2.m_yEnd)
			{
				counter++;
				if (!bExcludeExactEndpoints)
				{
					return 1;
				}
			}
			if (line1.m_xEnd == line2.m_xStart && line1.m_yEnd == line2.m_yStart || line1.m_xEnd == line2.m_xEnd && line1.m_yEnd == line2.m_yEnd)
			{
				counter++;
				if (counter == 2)
				{
					return 2;
				}
				// counter == 2 means both endpoints coincide (Lines
				// overlap).
				if (!bExcludeExactEndpoints)
				{
					return 1;
				}
			}
			if (line2._isIntersectingPoint(line1.GetStartXY(), tolerance, true))
			{
				return 1;
			}
			// return true;
			if (line2._isIntersectingPoint(line1.GetEndXY(), tolerance, true))
			{
				return 1;
			}
			// return true;
			if (line1._isIntersectingPoint(line2.GetStartXY(), tolerance, true))
			{
				return 1;
			}
			// return true;
			if (line1._isIntersectingPoint(line2.GetEndXY(), tolerance, true))
			{
				return 1;
			}
			// return true;
			if (bExcludeExactEndpoints && (counter != 0))
			{
				return 0;
			}
			// return false;
			return _isIntersectingHelper(line1, line2) == false ? 0 : 1;
		}

		internal int _intersectLineLineExact(com.epl.geometry.Line line1, com.epl.geometry.Line line2, com.epl.geometry.Point2D[] intersectionPoints, double[] param1, double[] param2)
		{
			int counter = 0;
			if (line1.m_xStart == line2.m_xStart && line1.m_yStart == line2.m_yStart)
			{
				if (param1 != null)
				{
					// if (param1)
					param1[counter] = 0.0;
				}
				if (param2 != null)
				{
					// if (param2)
					param2[counter] = 0.0;
				}
				if (intersectionPoints != null)
				{
					// if (intersectionPoints)
					intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line1.m_xStart, line1.m_yStart);
				}
				counter++;
			}
			if (line1.m_xStart == line2.m_xEnd && line1.m_yStart == line2.m_yEnd)
			{
				if (param1 != null)
				{
					// if (param1)
					param1[counter] = 0.0;
				}
				if (param2 != null)
				{
					// if (param2)
					param2[counter] = 1.0;
				}
				if (intersectionPoints != null)
				{
					// if (intersectionPoints)
					intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line1.m_xStart, line1.m_yStart);
				}
				counter++;
			}
			if (line1.m_xEnd == line2.m_xStart && line1.m_yEnd == line2.m_yStart)
			{
				if (counter == 2)
				{
					// both segments a degenerate
					if (param1 != null)
					{
						// if (param1)
						param1[0] = 0.0;
						param1[1] = 1.0;
					}
					if (param2 != null)
					{
						// if (param2)
						param2[0] = 1.0;
					}
					if (intersectionPoints != null)
					{
						// if (intersectionPoints)
						intersectionPoints[0] = com.epl.geometry.Point2D.Construct(line1.m_xEnd, line1.m_yEnd);
						intersectionPoints[1] = com.epl.geometry.Point2D.Construct(line1.m_xEnd, line1.m_yEnd);
					}
					return counter;
				}
				if (param1 != null)
				{
					// if (param1)
					param1[counter] = 1.0;
				}
				if (param2 != null)
				{
					// if (param2)
					param2[counter] = 0.0;
				}
				if (intersectionPoints != null)
				{
					// if (intersectionPoints)
					intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line1.m_xEnd, line1.m_yEnd);
				}
				counter++;
			}
			if (line1.m_xEnd == line2.m_xEnd && line1.m_yEnd == line2.m_yEnd)
			{
				if (counter == 2)
				{
					// both segments are degenerate
					if (param1 != null)
					{
						// if (param1)
						param1[0] = 0.0;
						param1[1] = 1.0;
					}
					if (param2 != null)
					{
						// if (param2)
						param2[0] = 1.0;
					}
					if (intersectionPoints != null)
					{
						// if (intersectionPoints)
						intersectionPoints[0] = com.epl.geometry.Point2D.Construct(line1.m_xEnd, line1.m_yEnd);
						intersectionPoints[1] = com.epl.geometry.Point2D.Construct(line1.m_xEnd, line1.m_yEnd);
					}
					return counter;
				}
				if (param1 != null)
				{
					// if (param1)
					param1[counter] = 1.0;
				}
				if (param2 != null)
				{
					// if (param2)
					param2[counter] = 1.0;
				}
				if (intersectionPoints != null)
				{
					// if (intersectionPoints)
					intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line1.m_xEnd, line1.m_yEnd);
				}
				counter++;
			}
			return counter;
		}

		internal static int _intersectLineLine(com.epl.geometry.Line line1, com.epl.geometry.Line line2, com.epl.geometry.Point2D[] intersectionPoints, double[] param1, double[] param2, double tolerance)
		{
			// _ASSERT(!param1 && !param2 || param1);
			int counter = 0;
			// Test the end points for exact coincidence.
			double t11 = line1._intersection(line2.GetStartXY(), tolerance, false);
			double t12 = line1._intersection(line2.GetEndXY(), tolerance, false);
			double t21 = line2._intersection(line1.GetStartXY(), tolerance, false);
			double t22 = line2._intersection(line1.GetEndXY(), tolerance, false);
			if (!com.epl.geometry.NumberUtils.IsNaN(t11))
			{
				if (param1 != null)
				{
					// if (param1)
					param1[counter] = t11;
				}
				if (param2 != null)
				{
					// if (param2)
					param2[counter] = 0;
				}
				if (intersectionPoints != null)
				{
					// if (intersectionPoints)
					intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line2.m_xStart, line2.m_yStart);
				}
				counter++;
			}
			if (!com.epl.geometry.NumberUtils.IsNaN(t12))
			{
				if (param1 != null)
				{
					// if (param1)
					param1[counter] = t12;
				}
				if (param2 != null)
				{
					// if (param2)
					param2[counter] = 1.0;
				}
				if (intersectionPoints != null)
				{
					// if (intersectionPoints)
					intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line2.m_xEnd, line2.m_yEnd);
				}
				counter++;
			}
			if (counter != 2 && !com.epl.geometry.NumberUtils.IsNaN(t21))
			{
				if (!(t11 == 0 && t21 == 0) && !(t12 == 0 && t21 == 1.0))
				{
					// the "if"
					// makes
					// sure
					// this
					// has
					// not
					// been
					// already
					// calculated
					if (param1 != null)
					{
						// if (param1)
						param1[counter] = 0;
					}
					if (param2 != null)
					{
						// if (param2)
						param2[counter] = t21;
					}
					if (intersectionPoints != null)
					{
						// if (intersectionPoints)
						intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line1.m_xStart, line1.m_yStart);
					}
					counter++;
				}
			}
			if (counter != 2 && !com.epl.geometry.NumberUtils.IsNaN(t22))
			{
				if (!(t11 == 1.0 && t22 == 0) && !(t12 == 1.0 && t22 == 1.0))
				{
					// the
					// "if"
					// makes
					// sure
					// this
					// has
					// not
					// been
					// already
					// calculated
					if (param1 != null)
					{
						// if (param1)
						param1[counter] = 1.0;
					}
					if (param2 != null)
					{
						// if (param2)
						param2[counter] = t22;
					}
					if (intersectionPoints != null)
					{
						// if (intersectionPoints)
						intersectionPoints[counter] = com.epl.geometry.Point2D.Construct(line2.m_xEnd, line2.m_yEnd);
					}
					counter++;
				}
			}
			if (counter > 0)
			{
				if (counter == 2 && param1 != null && param1[0] > param1[1])
				{
					// make
					// sure
					// the
					// intersection
					// events
					// are
					// sorted
					// along
					// the
					// line1
					// can't
					// swap
					// doulbes
					// in
					// java
					// NumberUtils::Swap(param1[0],
					// param1[1]);
					double zeroParam1 = param1[0];
					param1[0] = param1[1];
					param1[1] = zeroParam1;
					if (param2 != null)
					{
						// if (param2)
						double zeroParam2 = param2[0];
						param2[0] = param2[1];
						param2[1] = zeroParam2;
					}
					// NumberUtils::Swap(ARRAYELEMENT(param2,
					// 0), ARRAYELEMENT(param2, 1));
					if (intersectionPoints != null)
					{
						// if (intersectionPoints)
						com.epl.geometry.Point2D tmp = new com.epl.geometry.Point2D(intersectionPoints[0].x, intersectionPoints[0].y);
						intersectionPoints[0] = intersectionPoints[1];
						intersectionPoints[1] = tmp;
					}
				}
				return counter;
			}
			com.epl.geometry.Point2D @params = _intersectHelper1(line1, line2, tolerance);
			if (com.epl.geometry.NumberUtils.IsNaN(@params.x))
			{
				return 0;
			}
			if (intersectionPoints != null)
			{
				// if (intersectionPoints)
				intersectionPoints[0] = line1.GetCoord2D(@params.x);
			}
			if (param1 != null)
			{
				// if (param1)
				param1[0] = @params.x;
			}
			if (param2 != null)
			{
				// if (param2)
				param2[0] = @params.y;
			}
			return 1;
		}

		public override void ReplaceNaNs(int semantics, double value)
		{
			AddAttribute(semantics);
			if (IsEmpty())
			{
				return;
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			for (int i = 0; i < ncomps; i++)
			{
				double v = _getAttributeAsDbl(0, semantics, i);
				if (double.IsNaN(v))
				{
					_setAttribute(0, semantics, 0, value);
				}
				v = _getAttributeAsDbl(1, semantics, i);
				if (double.IsNaN(v))
				{
					_setAttribute(1, semantics, 0, value);
				}
			}
		}

		internal override int GetYMonotonicParts(com.epl.geometry.SegmentBuffer[] monotonicSegments)
		{
			return 0;
		}

		internal override void _copyToImpl(com.epl.geometry.Segment dst)
		{
		}

		// TODO Auto-generated method stub
		/// <summary>The output of this method can be only used for debugging.</summary>
		/// <remarks>The output of this method can be only used for debugging. It is subject to change without notice.</remarks>
		public override string ToString()
		{
			string s = "Line: [" + m_xStart + ", " + m_yStart + ", " + m_xEnd + ", " + m_yEnd + "]";
			return s;
		}
	}
}
