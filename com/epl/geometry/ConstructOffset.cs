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
	internal class ConstructOffset
	{
		internal com.epl.geometry.ProgressTracker m_progressTracker;

		internal com.epl.geometry.Geometry m_inputGeometry;

		internal double m_distance;

		internal double m_tolerance;

		internal com.epl.geometry.OperatorOffset.JoinType m_joins;

		internal double m_miterLimit;

		internal class GraphicPoint
		{
			internal double x;

			internal double y;

			internal int m_next;

			internal int m_prev;

			internal double m;

			internal int type;

			internal GraphicPoint(double x_, double y_)
			{
				// Note: m_distance<0 offsets to the left, m_distance>0 offsets to the right
				// multipath offset
				x = x_;
				y = y_;
				type = 0;
				m = 0;
			}

			internal GraphicPoint(com.epl.geometry.Point2D r)
			{
				x = r.x;
				y = r.y;
				type = 0;
				m = 0;
			}

			internal GraphicPoint(com.epl.geometry.ConstructOffset.GraphicPoint pt)
			{
				x = pt.x;
				y = pt.y;
				type = pt.type;
				m = pt.m;
			}

			internal GraphicPoint(com.epl.geometry.ConstructOffset.GraphicPoint srcPt, double d, double angle)
			{
				x = srcPt.x + d * System.Math.Cos(angle);
				y = srcPt.y + d * System.Math.Sin(angle);
				type = srcPt.type;
				m = srcPt.m;
			}

			internal GraphicPoint(com.epl.geometry.ConstructOffset.GraphicPoint pt1, com.epl.geometry.ConstructOffset.GraphicPoint pt2)
			{
				x = (pt1.x + pt2.x) * 0.5;
				y = (pt1.y + pt2.y) * 0.5;
				type = pt1.type;
				m = pt1.m;
			}

			internal GraphicPoint(com.epl.geometry.ConstructOffset.GraphicPoint pt1, com.epl.geometry.ConstructOffset.GraphicPoint pt2, double ratio)
			{
				x = pt1.x + (pt2.x - pt1.x) * ratio;
				y = pt1.y + (pt2.y - pt1.y) * ratio;
				type = pt1.type;
				m = pt1.m;
			}
		}

		internal class GraphicRect
		{
			internal double x1;

			internal double x2;

			internal double y1;

			internal double y2;
		}

		internal class IntersectionInfo
		{
			internal com.epl.geometry.ConstructOffset.GraphicPoint pt;

			internal double rFirst;

			internal double rSecond;

			internal bool atExistingPt;
		}

		internal System.Collections.Generic.List<com.epl.geometry.ConstructOffset.GraphicPoint> m_srcPts;

		internal int m_srcPtCount;

		internal System.Collections.Generic.List<com.epl.geometry.ConstructOffset.GraphicPoint> m_offsetPts;

		internal int m_offsetPtCount;

		internal com.epl.geometry.MultiPath m_resultPath;

		internal int m_resultPoints;

		internal double m_a1;

		internal double m_a2;

		internal bool m_bBadSegs;

		internal ConstructOffset(com.epl.geometry.ProgressTracker progressTracker)
		{
			m_progressTracker = progressTracker;
		}

		// static
		internal static com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, double distance, com.epl.geometry.OperatorOffset.JoinType joins, double miterLimit, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (inputGeometry == null)
			{
				throw new System.ArgumentException();
			}
			if (inputGeometry.GetDimension() < 1)
			{
				// can offset Polygons and
				// Polylines only
				throw new System.ArgumentException();
			}
			if (distance == 0 || inputGeometry.IsEmpty())
			{
				return inputGeometry;
			}
			com.epl.geometry.ConstructOffset offset = new com.epl.geometry.ConstructOffset(progressTracker);
			offset.m_inputGeometry = inputGeometry;
			offset.m_distance = distance;
			offset.m_tolerance = tolerance;
			offset.m_joins = joins;
			offset.m_miterLimit = miterLimit;
			return offset._ConstructOffset();
		}

		internal virtual com.epl.geometry.Geometry _OffsetLine()
		{
			com.epl.geometry.Line line = (com.epl.geometry.Line)m_inputGeometry;
			com.epl.geometry.Point2D start = line.GetStartXY();
			com.epl.geometry.Point2D end = line.GetEndXY();
			com.epl.geometry.Point2D v = new com.epl.geometry.Point2D();
			v.Sub(end, start);
			v.Normalize();
			v.LeftPerpendicular();
			v.Scale(m_distance);
			start.Add(v);
			end.Add(v);
			com.epl.geometry.Line resLine = (com.epl.geometry.Line)line.CreateInstance();
			line.SetStartXY(start);
			line.SetEndXY(end);
			return resLine;
		}

		internal virtual com.epl.geometry.Geometry _OffsetEnvelope()
		{
			com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)m_inputGeometry;
			if ((m_distance > 0) && (m_joins != com.epl.geometry.OperatorOffset.JoinType.Miter))
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.AddEnvelope(envelope, false);
				m_inputGeometry = poly;
				return _ConstructOffset();
			}
			com.epl.geometry.Envelope resEnv = new com.epl.geometry.Envelope(envelope.m_envelope);
			resEnv.Inflate(m_distance, m_distance);
			return resEnv;
		}

		private readonly double pi = System.Math.PI;

		private readonly double two_pi = System.Math.PI * 2;

		private readonly double half_pi = System.Math.PI / 2;

		private readonly double sqrt2 = 1.4142135623730950488016887242097;

		private readonly double oneDegree = 0.01745329251994329576923690768489;

		private readonly int BAD_SEG = unchecked((int)(0x0100));

		private readonly int IS_END = unchecked((int)(0x0200));

		private readonly int CLOSING_SEG = unchecked((int)(0x0400));

		// GEOMETRYX_PI;
		// GEOMETRYX_2PI;
		// GEOMETRYX_HalfPI;
		internal virtual void AddPoint(com.epl.geometry.ConstructOffset.GraphicPoint pt)
		{
			m_offsetPts.Add(pt);
			m_offsetPtCount++;
		}

		internal virtual double Scal(com.epl.geometry.ConstructOffset.GraphicPoint pt1, com.epl.geometry.ConstructOffset.GraphicPoint pt2, com.epl.geometry.ConstructOffset.GraphicPoint pt3, com.epl.geometry.ConstructOffset.GraphicPoint pt4)
		{
			return (pt2.x - pt1.x) * (pt4.x - pt3.x) + (pt2.y - pt1.y) * (pt4.y - pt3.y);
		}

		// offPt is the point to add.
		// this point corresponds to the offset version of the end of seg1.
		// it could generate a segment going in the opposite direction of the
		// original segment
		// this situation is handled here by adding an additional "bad" segment
		internal virtual void AddPoint(com.epl.geometry.ConstructOffset.GraphicPoint offPt, int i_src)
		{
			if (m_offsetPtCount == 0)
			{
				// TODO: can we have this outside of this
				// method?
				AddPoint(offPt);
				return;
			}
			int n_src = m_srcPtCount;
			com.epl.geometry.ConstructOffset.GraphicPoint pt1;
			com.epl.geometry.ConstructOffset.GraphicPoint pt;
			pt1 = m_srcPts[i_src == 0 ? n_src - 1 : i_src - 1];
			pt = m_srcPts[i_src];
			// calculate scalar product to determine if the offset segment goes in
			// the same/opposite direction compared to the original one
			double s = Scal(pt1, pt, m_offsetPts[m_offsetPtCount - 1], offPt);
			if (s > 0)
			{
				// original segment and offset segment go in the same direction. Just
				// add the point
				AddPoint(offPt);
				return;
			}
			if (s < 0)
			{
				// we will add a loop. We need to make sure the points we introduce
				// don't generate a "reversed" segment
				// let's project the first point of the reversed segment
				// (m_offsetPts + m_offsetPtCount - 1) to check
				// if it falls on the good side of the original segment (scalar
				// product sign again)
				if (Scal(pt1, pt, pt, m_offsetPts[m_offsetPtCount - 1]) > 0)
				{
					com.epl.geometry.ConstructOffset.GraphicPoint p;
					// change value of m_offsetPts + m_offsetPtCount - 1
					int k;
					if (i_src == 0)
					{
						k = n_src - 2;
					}
					else
					{
						if (i_src == 1)
						{
							k = n_src - 1;
						}
						else
						{
							k = i_src - 2;
						}
					}
					com.epl.geometry.ConstructOffset.GraphicPoint pt0 = m_srcPts[k];
					double a = System.Math.Atan2(pt1.y - pt0.y, pt1.x - pt0.x);
					p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt1, m_distance, a - half_pi);
					m_offsetPts[m_offsetPtCount - 1] = p;
					if (m_joins == com.epl.geometry.OperatorOffset.JoinType.Bevel || m_joins == com.epl.geometry.OperatorOffset.JoinType.Miter)
					{
						// this block is added as well as the commented BAD_SEG in
						// the next block
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(p, pt1);
						AddPoint(p);
						// "bad" segment
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt1, m_distance, m_a1 + half_pi);
						com.epl.geometry.ConstructOffset.GraphicPoint p_ = new com.epl.geometry.ConstructOffset.GraphicPoint(p, pt1);
						p_.type |= BAD_SEG;
						AddPoint(p_);
						AddPoint(p);
					}
					else
					{
						// the working stuff for round and square
						// "bad" segment
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt1, m_distance, m_a1 + half_pi);
						p.type |= BAD_SEG;
						AddPoint(p);
					}
					// add offPt
					AddPoint(offPt, i_src);
				}
				else
				{
					com.epl.geometry.ConstructOffset.GraphicPoint p;
					// we don't add offPt but the loop containing the "bad" segment
					p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, m_a1 + half_pi);
					AddPoint(p);
					if (m_joins == com.epl.geometry.OperatorOffset.JoinType.Bevel || m_joins == com.epl.geometry.OperatorOffset.JoinType.Miter)
					{
						// this block is added as well as the commented BAD_SEG in
						// the next block
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(p, pt);
						AddPoint(p);
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, m_a2 - half_pi);
						com.epl.geometry.ConstructOffset.GraphicPoint p_ = new com.epl.geometry.ConstructOffset.GraphicPoint(p, pt);
						p_.type |= BAD_SEG;
						AddPoint(p_);
						AddPoint(p);
					}
					else
					{
						// the working stuff for round and square
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, m_a2 - half_pi);
						p.type |= BAD_SEG;
						AddPoint(p);
					}
				}
			}
		}

		internal virtual bool BuildOffset()
		{
			// make sure we have at least three points and no identical points
			int i;
			double a1;
			double a2;
			com.epl.geometry.ConstructOffset.GraphicPoint pt;
			com.epl.geometry.ConstructOffset.GraphicPoint pt1;
			com.epl.geometry.ConstructOffset.GraphicPoint pt2;
			com.epl.geometry.ConstructOffset.GraphicPoint p;
			// number of points to deal with
			int n = m_srcPtCount;
			m_offsetPtCount = 0;
			double flattenTolerance = m_tolerance * 0.5;
			double a1_0 = 0;
			double a2_0 = 0;
			for (i = 0; i < n; i++)
			{
				pt = m_srcPts[i];
				// point before
				if (i == 0)
				{
					pt1 = m_srcPts[n - 1];
				}
				else
				{
					pt1 = m_srcPts[i - 1];
				}
				// point after
				if (i == n - 1)
				{
					pt2 = m_srcPts[0];
				}
				else
				{
					pt2 = m_srcPts[i + 1];
				}
				// angles of enclosing segments
				double dx1 = pt1.x - pt.x;
				double dy1 = pt1.y - pt.y;
				double dx2 = pt2.x - pt.x;
				double dy2 = pt2.y - pt.y;
				a1 = System.Math.Atan2(dy1, dx1);
				a2 = System.Math.Atan2(dy2, dx2);
				m_a1 = a1;
				m_a2 = a2;
				if (i == 0)
				{
					a1_0 = a1;
					a2_0 = a2;
				}
				// double dot_product = dx1 * dx2 + dy1 * dy2;
				double cross_product = dx1 * dy2 - dx2 * dy1;
				// boolean bInnerAngle = (cross_product == 0) ? (m_distance > 0) :
				// (cross_product * m_distance >= 0.0);
				// check for inner angles (always managed the same, whatever the
				// type of join)
				double saved_a2 = a2;
				if (a2 < a1)
				{
					a2 += two_pi;
				}
				// this guaranties that (a1 + a2) / 2 is on the
				// right side of the curve
				if (cross_product * m_distance > 0.0)
				{
					// inner angle
					// inner angle
					if (m_joins == com.epl.geometry.OperatorOffset.JoinType.Bevel || m_joins == com.epl.geometry.OperatorOffset.JoinType.Miter)
					{
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, a1 + half_pi);
						AddPoint(p);
						// this block is added as well as the commented BAD_SEG in
						// the next block
						double ratio = 0.001;
						// TODO: the higher the ratio, the
						// better the result (shorter
						// segments)
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, p, ratio);
						AddPoint(p);
						// this is the "bad" segment
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, a2 - half_pi);
						com.epl.geometry.ConstructOffset.GraphicPoint p_ = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, p, ratio);
						p_.type |= BAD_SEG;
						AddPoint(p_);
						AddPoint(p);
					}
					else
					{
						// this method works for square and round, but not bevel
						double r = (a2 - a1) * 0.5;
						double d = m_distance / System.Math.Abs(System.Math.Sin(r));
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, (a1 + a2) * 0.5);
						AddPoint(p, i);
					}
					// will deal with reversed segments
					continue;
				}
				// outer angles
				// check if we have an end point first
				if ((pt.type & IS_END) != 0)
				{
					// TODO: deal with other options. assume rounded and
					// perpendicular for now
					// we need to use the outer regular polygon of the round join
					// TODO: explain this in a doc
					// calculate the number of points based on a flatten tolerance
					double r = 1.0 - flattenTolerance / System.Math.Abs(m_distance);
					long na = 1;
					double da = (m_distance < 0) ? -pi : pi;
					// da is negative when
					// m_offset is
					// negative (???)
					if (r > -1.0 && r < 1.0)
					{
						double a = System.Math.Acos(r) * 2;
						// angle where "arrow?" is less
						// than flattenTolerance
						// do not consider an angle smaller than a degree
						if (a < oneDegree)
						{
							a = oneDegree;
						}
						na = (long)(pi / a + 1.5);
						if (na > 1)
						{
							da /= na;
						}
					}
					// add first point
					double a_1 = a1 + half_pi;
					p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, a_1);
					if (i == 0)
					{
						p.type |= CLOSING_SEG;
					}
					// TODO: should we simplify this by
					// considering the last point
					// instead of the first one??
					AddPoint(p, i);
					// will deal with reversed segments
					double d = m_distance / System.Math.Cos(da / 2);
					a_1 += da / 2;
					p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, a_1);
					p.type |= CLOSING_SEG;
					AddPoint(p);
					while (--na > 0)
					{
						a_1 += da;
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, a_1);
						p.type |= CLOSING_SEG;
						AddPoint(p);
					}
					// last point (optional except for the first point)
					p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, a2 - half_pi);
					// this one
					// is
					// optional
					// except
					// for the
					// first
					// point
					p.type |= CLOSING_SEG;
					AddPoint(p);
					continue;
				}
				else
				{
					if (m_joins == com.epl.geometry.OperatorOffset.JoinType.Bevel)
					{
						// bevel
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, a1 + half_pi);
						AddPoint(p, i);
						// will deal with reversed segments
						p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, m_distance, a2 - half_pi);
						AddPoint(p);
						continue;
					}
					else
					{
						if (m_joins == com.epl.geometry.OperatorOffset.JoinType.Round)
						{
							// we need to use the outer regular polygon of the round join
							// TODO: explain this in a doc
							// calculate the number of points based on a flatten tolerance
							double r = 1.0 - flattenTolerance / System.Math.Abs(m_distance);
							long na = 1;
							double da = (a2 - half_pi) - (a1 + half_pi);
							// da is negative
							// when
							// m_distance is
							// negative
							if (r > -1.0 && r < 1.0)
							{
								double a = System.Math.Acos(r) * 2.0;
								// angle where "arrow?" is
								// less than
								// flattenTolerance
								// do not consider an angle smaller than a degree
								if (a < oneDegree)
								{
									a = oneDegree;
								}
								na = (long)(System.Math.Abs(da) / a + 1.5);
								if (na > 1)
								{
									da /= na;
								}
							}
							double d = m_distance / System.Math.Cos(da * 0.5);
							double a_1 = a1 + half_pi + da * 0.5;
							p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, a_1);
							AddPoint(p, i);
							// will deal with reversed segments
							while (--na > 0)
							{
								a_1 += da;
								p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, a_1);
								AddPoint(p);
							}
							continue;
						}
						else
						{
							if (m_joins == com.epl.geometry.OperatorOffset.JoinType.Miter)
							{
								dx1 = pt1.x - pt.x;
								dy1 = pt1.y - pt.y;
								dx2 = pt2.x - pt.x;
								dy2 = pt2.y - pt.y;
								double d1 = System.Math.Sqrt(dx1 * dx1 + dy1 * dy1);
								double d2 = System.Math.Sqrt(dx2 * dx2 + dy2 * dy2);
								double cosa = (dx1 * dx2 + dy1 * dy2) / d1 / d2;
								if (cosa > 1.0 - 1.0e-8)
								{
									// there's a spike in the polygon boundary; this could
									// happen when filtering out short segments in Init()
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, sqrt2 * m_distance, a2 - pi * 0.25);
									AddPoint(p, i);
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, sqrt2 * m_distance, a2 + pi * 0.25);
									AddPoint(p);
									continue;
								}
								// original miter code
								// if (m_miterLimit * m_miterLimit * (1 - cosa) < 2)
								// {
								// // bevel join
								// p = new GraphicPoint(pt, m_distance, a1 + half_pi);
								// AddPoint(p, src_poly, srcPtCount, i); // will deal with
								// reversed segments
								// p = new GraphicPoint(pt, m_distance, a2 - half_pi);
								// AddPoint(p);
								// continue;
								// }
								double distanceFromCorner = System.Math.Abs(m_distance / System.Math.Sin(System.Math.Acos(cosa) * 0.5));
								double bevelDistance = System.Math.Abs(m_miterLimit * m_distance);
								if (distanceFromCorner > bevelDistance)
								{
									double r = (a2 - a1) * 0.5;
									double d = m_distance / System.Math.Abs(System.Math.Sin(r));
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, (a1 + a2) * 0.5);
									// construct bevel points, see comment in
									// c:\ArcGIS\System\Geometry\Geometry\ConstructCurveImpl.cpp,
									// ESRI::OffsetCurve::EstimateBevelPoints
									com.epl.geometry.Point2D corner = new com.epl.geometry.Point2D(p.x, p.y);
									com.epl.geometry.Point2D through = new com.epl.geometry.Point2D(pt.x, pt.y);
									com.epl.geometry.Point2D delta = new com.epl.geometry.Point2D();
									delta.Sub(corner, through);
									// Point2D midPoint = through + delta * (bevelDistance /
									// delta.Length());
									com.epl.geometry.Point2D midPoint = new com.epl.geometry.Point2D();
									midPoint.ScaleAdd(bevelDistance / delta.Length(), delta, through);
									double sideLength = System.Math.Sqrt(distanceFromCorner * distanceFromCorner - m_distance * m_distance);
									double halfWidth = (distanceFromCorner - bevelDistance) * System.Math.Abs(m_distance) / sideLength;
									// delta = delta.RotateDirect(0.0, m_distance > 0.0 ? -1.0 :
									// 1.0) * (halfWidth/delta.Length());
									if (m_distance > 0.0)
									{
										delta.LeftPerpendicular();
									}
									else
									{
										delta.RightPerpendicular();
									}
									delta.Scale(halfWidth / delta.Length());
									com.epl.geometry.Point2D from = new com.epl.geometry.Point2D();
									from.Add(midPoint, delta);
									com.epl.geometry.Point2D to = new com.epl.geometry.Point2D();
									to.Sub(midPoint, delta);
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(from);
									// _ASSERT(::_finite(p.x));
									// _ASSERT(::_finite(p.y));
									AddPoint(p, i);
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(to);
									// _ASSERT(::_finite(p.x));
									// _ASSERT(::_finite(p.y));
									AddPoint(p);
									continue;
								}
								// miter join
								double r_1 = (a2 - a1) * 0.5;
								double d_1 = m_distance / System.Math.Abs(System.Math.Sin(r_1));
								// r should not
								// be null
								// (trapped by
								// the bevel
								// case)
								p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d_1, (a1 + a2) * 0.5);
								AddPoint(p, i);
								// will deal with reversed segments
								continue;
							}
							else
							{
								// the new "square" join
								a2 = saved_a2;
								// identify if angle is less than pi/2
								// in this case, we introduce a segment that is perpendicular to
								// the bissector of the angle
								// TODO: see figure X for details
								bool bAddSegment;
								if (m_distance > 0.0)
								{
									if (a2 > a1)
									{
										// > and not >=
										a2 -= two_pi;
									}
									bAddSegment = (a1 - a2 < half_pi);
								}
								else
								{
									if (a2 < a1)
									{
										// < and not <=
										a2 += two_pi;
									}
									bAddSegment = (a2 - a1 < half_pi);
								}
								if (bAddSegment)
								{
									// make it continuous when angle is pi/2 (but not tangent to
									// the round join)
									double d = m_distance * sqrt2;
									double a;
									if (d < 0.0)
									{
										a = a1 + pi * 0.25;
									}
									else
									{
										a = a1 + 3.0 * pi * 0.25;
									}
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, a);
									AddPoint(p, i);
									if (d < 0)
									{
										a = a2 - pi * 0.25;
									}
									else
									{
										a = a2 - 3.0 * pi * 0.25;
									}
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, a);
									AddPoint(p);
								}
								else
								{
									// standard case: we just add the intersection point of
									// offset segments
									double r = (a2 - a1) * 0.5;
									double d = m_distance / System.Math.Abs(System.Math.Sin(r));
									if (a2 < a1)
									{
										a2 += two_pi;
									}
									// this guaranties that (a1 + a2) / 2 is
									// on the right side with a positive
									// offset
									p = new com.epl.geometry.ConstructOffset.GraphicPoint(pt, d, (a1 + a2) / 2);
									AddPoint(p, i);
								}
							}
						}
					}
				}
			}
			// closing point
			m_a1 = a1_0;
			m_a2 = a2_0;
			AddPoint(m_offsetPts[0], 0);
			// make sure the first point matches the last (in case a problem of
			// reversed segment happens there)
			pt = new com.epl.geometry.ConstructOffset.GraphicPoint(m_offsetPts[m_offsetPtCount - 1]);
			m_offsetPts[0] = pt;
			// remove loops
			return RemoveBadSegsFast();
		}

		internal virtual void AddPart(int iStart, int cPts)
		{
			if (cPts < 2)
			{
				return;
			}
			for (int i = 0; i < cPts; i++)
			{
				com.epl.geometry.ConstructOffset.GraphicPoint pt = m_offsetPts[iStart + i];
				if (i != 0)
				{
					m_resultPath.LineTo(new com.epl.geometry.Point2D(pt.x, pt.y));
				}
				else
				{
					m_resultPath.StartPath(new com.epl.geometry.Point2D(pt.x, pt.y));
				}
			}
		}

		internal virtual void _OffsetPath(com.epl.geometry.MultiPath multiPath, int pathIndex, com.epl.geometry.MultiPath resultingPath)
		{
			int startVertex = multiPath.GetPathStart(pathIndex);
			int endVertex = multiPath.GetPathEnd(pathIndex);
			m_offsetPts = new System.Collections.Generic.List<com.epl.geometry.ConstructOffset.GraphicPoint>();
			// test if part is closed
			m_resultPath = resultingPath;
			m_resultPoints = 0;
			if (multiPath.IsClosedPath(pathIndex))
			{
				// check if last point is a duplicate of first
				com.epl.geometry.Point2D ptStart = multiPath.GetXY(startVertex);
				while (multiPath.GetXY(endVertex - 1).IsEqual(ptStart))
				{
					endVertex--;
				}
				// we need at least three points for a polygon
				if (endVertex - startVertex >= 2)
				{
					m_srcPtCount = endVertex - startVertex;
					m_srcPts = new System.Collections.Generic.List<com.epl.geometry.ConstructOffset.GraphicPoint>(m_srcPtCount);
					// TODO: may throw std::bad:alloc()
					for (int i = startVertex; i < endVertex; i++)
					{
						m_srcPts.Add(new com.epl.geometry.ConstructOffset.GraphicPoint(multiPath.GetXY(i)));
					}
					if (BuildOffset())
					{
						AddPart(0, m_offsetPtCount - 1);
					}
				}
			}
			else
			{
				// do not repeat closing
				// point
				// remove duplicate points at extremities
				com.epl.geometry.Point2D ptStart = multiPath.GetXY(startVertex);
				while ((startVertex < endVertex) && multiPath.GetXY(startVertex + 1).IsEqual(ptStart))
				{
					startVertex++;
				}
				com.epl.geometry.Point2D ptEnd = multiPath.GetXY(endVertex - 1);
				while ((startVertex < endVertex) && multiPath.GetXY(endVertex - 2).IsEqual(ptEnd))
				{
					endVertex--;
				}
				// we need at least two points for a polyline
				if (endVertex - startVertex >= 2)
				{
					// close the line and mark the opposite segments as non valid
					m_srcPtCount = (endVertex - startVertex) * 2 - 2;
					m_srcPts = new System.Collections.Generic.List<com.epl.geometry.ConstructOffset.GraphicPoint>(m_srcPtCount);
					// TODO: may throw std::bad:alloc()
					com.epl.geometry.ConstructOffset.GraphicPoint pt = new com.epl.geometry.ConstructOffset.GraphicPoint(multiPath.GetXY(startVertex));
					pt.type |= IS_END + CLOSING_SEG;
					m_srcPts.Add(pt);
					for (int i = startVertex + 1; i < endVertex - 1; i++)
					{
						pt = new com.epl.geometry.ConstructOffset.GraphicPoint(multiPath.GetXY(i));
						m_srcPts.Add(pt);
					}
					pt = new com.epl.geometry.ConstructOffset.GraphicPoint(multiPath.GetXY(endVertex - 1));
					pt.type |= IS_END;
					m_srcPts.Add(pt);
					for (int i_1 = endVertex - 2; i_1 >= startVertex + 1; i_1--)
					{
						pt = new com.epl.geometry.ConstructOffset.GraphicPoint(multiPath.GetXY(i_1));
						pt.type |= CLOSING_SEG;
						m_srcPts.Add(pt);
					}
					if (BuildOffset())
					{
						if (m_offsetPts.Count >= 2)
						{
							// extract the part that doesn't have the CLOSING_SEG
							// attribute
							int iStart = -1;
							int iEnd = -1;
							bool prevClosed = (m_offsetPts[m_offsetPtCount - 1].type & CLOSING_SEG) != 0;
							if (!prevClosed)
							{
								iStart = 0;
							}
							for (int i_2 = 1; i_2 < m_offsetPtCount; i_2++)
							{
								bool closed = (m_offsetPts[i_2].type & CLOSING_SEG) != 0;
								if (!closed)
								{
									if (prevClosed)
									{
										// if ((m_offsetPts[i - 1].type & MOVE_TO)
										// == 0)
										// m_offsetPts[i - 1].type += MOVE_TO -
										// LINE_TO;
										iStart = i_2 - 1;
									}
								}
								else
								{
									if (!prevClosed)
									{
										iEnd = i_2 - 1;
										// for (long i = iStart; i <= iEnd; i++)
										// m_offsetPts[i].type &= OUR_FLAGS_MASK;
										if (iEnd - iStart + 1 > 1)
										{
											AddPart(iStart, iEnd - iStart + 1);
										}
									}
								}
								prevClosed = closed;
							}
							if (!prevClosed)
							{
								iEnd = m_offsetPtCount - 1;
								// for (long i = iStart; i <= iEnd; i++)
								// m_offsetPts[i].type &= OUR_FLAGS_MASK;
								if (iEnd - iStart + 1 > 1)
								{
									AddPart(iStart, iEnd - iStart + 1);
								}
							}
						}
						else
						{
							int iStart = 0;
							int iEnd = m_offsetPtCount - 1;
							if (iStart >= 0 && iEnd - iStart >= 1)
							{
								// for (long i = iStart; i <= iEnd; i++)
								// m_offsetPts[i].type &= OUR_FLAGS_MASK;
								AddPart(iStart, iEnd - iStart + 1);
							}
						}
					}
				}
			}
			// clear source
			m_srcPts = null;
			m_srcPtCount = 0;
			// free offset buffer
			m_offsetPts = null;
			m_offsetPtCount = 0;
		}

		internal virtual bool RemoveBadSegsFast()
		{
			bool bWrong = false;
			// initialize circular doubly-linked list
			// skip last point which is dup of first point
			for (int i = 0; i < m_offsetPtCount; i++)
			{
				com.epl.geometry.ConstructOffset.GraphicPoint pt = m_offsetPts[i];
				pt.m_next = i + 1;
				pt.m_prev = i - 1;
				m_offsetPts[i] = pt;
			}
			// need to update the first and last elements
			com.epl.geometry.ConstructOffset.GraphicPoint pt_1;
			pt_1 = m_offsetPts[0];
			pt_1.m_prev = m_offsetPtCount - 2;
			m_offsetPts[0] = pt_1;
			pt_1 = m_offsetPts[m_offsetPtCount - 2];
			pt_1.m_next = 0;
			m_offsetPts[m_offsetPtCount - 2] = pt_1;
			int w = 0;
			for (int i_1 = 0; i_1 < m_offsetPtCount; i_1++)
			{
				if ((m_offsetPts[w].type & BAD_SEG) != 0)
				{
					int wNext = DeleteClosedSeg(w);
					if (wNext != -1)
					{
						w = wNext;
					}
					else
					{
						bWrong = true;
						break;
					}
				}
				else
				{
					w = m_offsetPts[w].m_next;
				}
			}
			if (bWrong)
			{
				return false;
			}
			// w is the index of a known good (i.e. surviving ) point in the offset
			// array
			CompressOffsetArray(w);
			return true;
		}

		internal virtual int DeleteClosedSeg(int seg)
		{
			int n = m_offsetPtCount - 1;
			// number of segments
			// check combinations of segments
			int ip0 = seg;
			int ip;
			int im;
			for (int i = 1; i <= n - 2; i++)
			{
				ip0 = m_offsetPts[ip0].m_next;
				ip = ip0;
				im = seg;
				for (int j = 1; j <= i; j++)
				{
					im = m_offsetPts[im].m_prev;
					if ((m_offsetPts[im].type & BAD_SEG) == 0 && (m_offsetPts[ip].type & BAD_SEG) == 0)
					{
						int rSegNext = HandleClosedIntersection(im, ip);
						if (rSegNext != -1)
						{
							return rSegNext;
						}
					}
					ip = m_offsetPts[ip].m_prev;
				}
			}
			return -1;
		}

		// line segments defined by (im-1, im) and (ip-1, ip)
		internal virtual int HandleClosedIntersection(int im, int ip)
		{
			com.epl.geometry.ConstructOffset.GraphicPoint pt1;
			com.epl.geometry.ConstructOffset.GraphicPoint pt2;
			com.epl.geometry.ConstructOffset.GraphicPoint pt3;
			com.epl.geometry.ConstructOffset.GraphicPoint pt4;
			pt1 = m_offsetPts[m_offsetPts[im].m_prev];
			pt2 = m_offsetPts[im];
			pt3 = m_offsetPts[m_offsetPts[ip].m_prev];
			pt4 = m_offsetPts[ip];
			if (!SectGraphicRect(pt1, pt2, pt3, pt4))
			{
				return -1;
			}
			// intersection
			com.epl.geometry.ConstructOffset.IntersectionInfo ii = new com.epl.geometry.ConstructOffset.IntersectionInfo();
			if (FindIntersection(pt1, pt2, pt3, pt4, ii) && !ii.atExistingPt)
			{
				if (System.Math.Sign((pt2.x - pt1.x) * (pt4.y - pt3.y) - (pt2.y - pt1.y) * (pt4.x - pt3.x)) != System.Math.Sign(m_distance))
				{
					int prev0 = m_offsetPts[im].m_prev;
					ii.pt.type = pt2.type;
					ii.pt.m_next = ip;
					ii.pt.m_prev = prev0;
					m_offsetPts[im] = ii.pt;
					ii.pt = m_offsetPts[ip];
					ii.pt.m_prev = im;
					m_offsetPts[ip] = ii.pt;
					return ip;
				}
			}
			return -1;
		}

		internal virtual bool SectGraphicRect(com.epl.geometry.ConstructOffset.GraphicPoint pt1, com.epl.geometry.ConstructOffset.GraphicPoint pt2, com.epl.geometry.ConstructOffset.GraphicPoint pt3, com.epl.geometry.ConstructOffset.GraphicPoint pt4)
		{
			return (System.Math.Max(pt1.x, pt2.x) >= System.Math.Min(pt3.x, pt4.x) && System.Math.Max(pt3.x, pt4.x) >= System.Math.Min(pt1.x, pt2.x) && System.Math.Max(pt1.y, pt2.y) >= System.Math.Min(pt3.y, pt4.y) && System.Math.Max(pt3.y, pt4.y) >= System.Math.Min(pt1.y, pt2.y));
		}

		internal virtual bool FindIntersection(com.epl.geometry.ConstructOffset.GraphicPoint bp1, com.epl.geometry.ConstructOffset.GraphicPoint bp2, com.epl.geometry.ConstructOffset.GraphicPoint bp3, com.epl.geometry.ConstructOffset.GraphicPoint bp4, com.epl.geometry.ConstructOffset.IntersectionInfo
			 intersectionInfo)
		{
			intersectionInfo.atExistingPt = false;
			// Note: test if rectangles intersect already done by caller
			// intersection
			double i;
			double j;
			double r;
			double r1;
			i = (bp2.y - bp1.y) * (bp4.x - bp3.x) - (bp2.x - bp1.x) * (bp4.y - bp3.y);
			j = (bp3.y - bp1.y) * (bp2.x - bp1.x) - (bp3.x - bp1.x) * (bp2.y - bp1.y);
			if (i == 0.0)
			{
				r = 2.0;
			}
			else
			{
				r = j / i;
			}
			if ((r >= 0.0) && (r <= 1.0))
			{
				r1 = r;
				i = (bp4.y - bp3.y) * (bp2.x - bp1.x) - (bp4.x - bp3.x) * (bp2.y - bp1.y);
				j = (bp1.y - bp3.y) * (bp4.x - bp3.x) - (bp1.x - bp3.x) * (bp4.y - bp3.y);
				if (i == 0.0)
				{
					r = 2.0;
				}
				else
				{
					r = j / i;
				}
				if ((r >= 0.0) && (r <= 1.0))
				{
					intersectionInfo.pt = new com.epl.geometry.ConstructOffset.GraphicPoint(bp1.x + r * (bp2.x - bp1.x), bp1.y + r * (bp2.y - bp1.y));
					intersectionInfo.pt.m = bp3.m + r1 * (bp4.m - bp3.m);
					if (((r1 == 0.0) || (r1 == 1.0)) && ((r == 0.0) || (r == 1.0)))
					{
						intersectionInfo.atExistingPt = true;
					}
					intersectionInfo.rFirst = r;
					intersectionInfo.rSecond = r1;
					if (((r1 == 0.0) || (r1 == 1.0)) && ((r > 0.0) && (r < 1.0)) || ((r == 0.0) || (r == 1.0)) && ((r1 > 0.0) && (r1 < 1.0)))
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		// i0 is the index of a known good point in the offset points array; that
		// is, its the index of a point that isn't part of a deleted loop
		internal virtual void CompressOffsetArray(int i0)
		{
			int i_ = i0;
			while (m_offsetPts[i_].m_prev < i_)
			{
				i_ = m_offsetPts[i_].m_prev;
			}
			int j = 0;
			int i = i_;
			do
			{
				com.epl.geometry.ConstructOffset.GraphicPoint pt = m_offsetPts[i];
				m_offsetPts[j] = pt;
				i = pt.m_next;
				j++;
			}
			while (i != i_);
			m_offsetPts[j] = m_offsetPts[0];
			// duplicate closing point
			m_offsetPtCount = j + 1;
		}

		internal virtual void _OffsetMultiPath(com.epl.geometry.MultiPath resultingPath)
		{
			// we process all path independently, then merge the results
			com.epl.geometry.MultiPath multiPath = (com.epl.geometry.MultiPath)m_inputGeometry;
			com.epl.geometry.SegmentIterator segmentIterator = multiPath.QuerySegmentIterator();
			if (segmentIterator == null)
			{
				return;
			}
			// TODO: strategy on error?
			segmentIterator.ResetToFirstPath();
			int pathIndex = -1;
			while (segmentIterator.NextPath())
			{
				pathIndex++;
				_OffsetPath(multiPath, pathIndex, resultingPath);
			}
		}

		internal virtual com.epl.geometry.Geometry _ConstructOffset()
		{
			int gt = m_inputGeometry.GetType().Value();
			if (gt == com.epl.geometry.Geometry.GeometryType.Line)
			{
				return _OffsetLine();
			}
			if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				return _OffsetEnvelope();
			}
			if (com.epl.geometry.Geometry.IsSegment(gt))
			{
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
				poly.AddSegment((com.epl.geometry.Segment)m_inputGeometry, true);
				m_inputGeometry = poly;
				return _ConstructOffset();
			}
			if (gt == com.epl.geometry.Geometry.GeometryType.Polyline)
			{
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				_OffsetMultiPath(polyline);
				return polyline;
			}
			if (gt == com.epl.geometry.Geometry.GeometryType.Polygon)
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				_OffsetMultiPath(polygon);
				return polygon;
			}
			// throw new GeometryException("not implemented");
			return null;
		}
	}
}
