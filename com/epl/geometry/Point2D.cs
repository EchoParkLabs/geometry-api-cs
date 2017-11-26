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
using System;

namespace com.epl.geometry
{
	/// <summary>Basic 2D point class.</summary>
	/// <remarks>Basic 2D point class. Contains only two double fields.</remarks>
	[System.Serializable]
	public sealed class Point2D
	{
		private const long serialVersionUID = 1L;

		public double x;

		public double y;

		public Point2D()
		{
		}

		public Point2D(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public Point2D(com.epl.geometry.Point2D other)
		{
			SetCoords(other);
		}

		public static com.epl.geometry.Point2D Construct(double x, double y)
		{
			return new com.epl.geometry.Point2D(x, y);
		}

		public void SetCoords(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public void SetCoords(com.epl.geometry.Point2D other)
		{
			x = other.x;
			y = other.y;
		}

		public bool IsEqual(com.epl.geometry.Point2D other)
		{
			return x == other.x && y == other.y;
		}

		public bool IsEqual(double x_, double y_)
		{
			return x == x_ && y == y_;
		}

		public bool IsEqual(com.epl.geometry.Point2D other, double tol)
		{
			return (System.Math.Abs(x - other.x) <= tol) && (System.Math.Abs(y - other.y) <= tol);
		}

		public bool Equals(com.epl.geometry.Point2D other)
		{
			return x == other.x && y == other.y;
		}

		public override bool Equals(object other)
		{
			if (other == this)
			{
				return true;
			}
			if (!(other is com.epl.geometry.Point2D))
			{
				return false;
			}
			com.epl.geometry.Point2D v = (com.epl.geometry.Point2D)other;
			return x == v.x && y == v.y;
		}

		public void Sub(com.epl.geometry.Point2D other)
		{
			x -= other.x;
			y -= other.y;
		}

		public void Sub(com.epl.geometry.Point2D p1, com.epl.geometry.Point2D p2)
		{
			x = p1.x - p2.x;
			y = p1.y - p2.y;
		}

		public void Add(com.epl.geometry.Point2D other)
		{
			x += other.x;
			y += other.y;
		}

		public void Add(com.epl.geometry.Point2D p1, com.epl.geometry.Point2D p2)
		{
			x = p1.x + p2.x;
			y = p1.y + p2.y;
		}

		public void Negate()
		{
			x = -x;
			y = -y;
		}

		public void Negate(com.epl.geometry.Point2D other)
		{
			x = -other.x;
			y = -other.y;
		}

		public void Interpolate(com.epl.geometry.Point2D other, double alpha)
		{
			com.epl.geometry.MathUtils.Lerp(this, other, alpha, this);
		}

		public void Interpolate(com.epl.geometry.Point2D p1, com.epl.geometry.Point2D p2, double alpha)
		{
			com.epl.geometry.MathUtils.Lerp(p1, p2, alpha, this);
		}

		/// <summary>Calculates this = this * f + shift</summary>
		/// <param name="f"/>
		/// <param name="shift"/>
		public void ScaleAdd(double f, com.epl.geometry.Point2D shift)
		{
			x = x * f + shift.x;
			y = y * f + shift.y;
		}

		/// <summary>Calculates this = other * f + shift</summary>
		/// <param name="f"/>
		/// <param name="other"/>
		/// <param name="shift"/>
		public void ScaleAdd(double f, com.epl.geometry.Point2D other, com.epl.geometry.Point2D shift)
		{
			x = other.x * f + shift.x;
			y = other.y * f + shift.y;
		}

		public void Scale(double f, com.epl.geometry.Point2D other)
		{
			x = f * other.x;
			y = f * other.y;
		}

		public void Scale(double f)
		{
			x *= f;
			y *= f;
		}

		/// <summary>Compares two vertices lexicographically by y.</summary>
		public int Compare(com.epl.geometry.Point2D other)
		{
			return y < other.y ? -1 : (y > other.y ? 1 : (x < other.x ? -1 : (x > other.x ? 1 : 0)));
		}

		/// <summary>Compares two vertices lexicographically by x.</summary>
		internal int CompareX(com.epl.geometry.Point2D other)
		{
			return x < other.x ? -1 : (x > other.x ? 1 : (y < other.y ? -1 : (y > other.y ? 1 : 0)));
		}

		public void Normalize(com.epl.geometry.Point2D other)
		{
			double len = other.Length();
			if (len == 0)
			{
				x = 1.0;
				y = 0.0;
			}
			else
			{
				x = other.x / len;
				y = other.y / len;
			}
		}

		public void Normalize()
		{
			double len = Length();
			if (len == 0)
			{
				// (!len)
				x = 1.0;
				y = 0.0;
			}
			x /= len;
			y /= len;
		}

		public double Length()
		{
			return System.Math.Sqrt(x * x + y * y);
		}

		public double SqrLength()
		{
			return x * x + y * y;
		}

		public static double Distance(com.epl.geometry.Point2D pt1, com.epl.geometry.Point2D pt2)
		{
			return System.Math.Sqrt(SqrDistance(pt1, pt2));
		}

		public double DotProduct(com.epl.geometry.Point2D other)
		{
			return x * other.x + y * other.y;
		}

		internal double _dotProductAbs(com.epl.geometry.Point2D other)
		{
			return System.Math.Abs(x * other.x) + System.Math.Abs(y * other.y);
		}

		public double CrossProduct(com.epl.geometry.Point2D other)
		{
			return x * other.y - y * other.x;
		}

		public void RotateDirect(double Cos, double Sin)
		{
			// corresponds to the
			// Transformation2D.SetRotate(cos,
			// sin).Transform(pt)
			double xx = x * Cos - y * Sin;
			double yy = x * Sin + y * Cos;
			x = xx;
			y = yy;
		}

		public void RotateReverse(double Cos, double Sin)
		{
			double xx = x * Cos + y * Sin;
			double yy = -x * Sin + y * Cos;
			x = xx;
			y = yy;
		}

		/// <summary>90 degree rotation, anticlockwise.</summary>
		/// <remarks>
		/// 90 degree rotation, anticlockwise. Equivalent to RotateDirect(cos(pi/2),
		/// sin(pi/2)).
		/// </remarks>
		public void LeftPerpendicular()
		{
			double xx = x;
			x = -y;
			y = xx;
		}

		/// <summary>90 degree rotation, anticlockwise.</summary>
		/// <remarks>
		/// 90 degree rotation, anticlockwise. Equivalent to RotateDirect(cos(pi/2),
		/// sin(pi/2)).
		/// </remarks>
		public void LeftPerpendicular(com.epl.geometry.Point2D pt)
		{
			x = -pt.y;
			y = pt.x;
		}

		/// <summary>270 degree rotation, anticlockwise.</summary>
		/// <remarks>
		/// 270 degree rotation, anticlockwise. Equivalent to
		/// RotateDirect(-cos(pi/2), sin(-pi/2)).
		/// </remarks>
		public void RightPerpendicular()
		{
			double xx = x;
			x = y;
			y = -xx;
		}

		/// <summary>270 degree rotation, anticlockwise.</summary>
		/// <remarks>
		/// 270 degree rotation, anticlockwise. Equivalent to
		/// RotateDirect(-cos(pi/2), sin(-pi/2)).
		/// </remarks>
		public void RightPerpendicular(com.epl.geometry.Point2D pt)
		{
			x = pt.y;
			y = -pt.x;
		}

		internal void _setNan()
		{
			x = com.epl.geometry.NumberUtils.NaN();
			y = com.epl.geometry.NumberUtils.NaN();
		}

		internal bool _isNan()
		{
			return com.epl.geometry.NumberUtils.IsNaN(x) || com.epl.geometry.NumberUtils.IsNaN(y);
		}

		// calculates which quarter of xy plane the vector lies in. First quater is
		// between vectors (1,0) and (0, 1), second between (0, 1) and (-1, 0), etc.
		// Angle intervals corresponding to quarters: 1 : [0 : 90); 2 : [90 : 180);
		// 3 : [180 : 270); 4 : [270 : 360)
		internal int _getQuarter()
		{
			if (x > 0)
			{
				if (y >= 0)
				{
					return 1;
				}
				else
				{
					// x > 0 && y <= 0
					return 4;
				}
			}
			else
			{
				// y < 0 && x > 0. Should be x >= 0 && y < 0. The x ==
				// 0 case is processed later.
				if (y > 0)
				{
					return 2;
				}
				else
				{
					// x <= 0 && y > 0
					return x == 0 ? 4 : 3;
				}
			}
		}

		// 3: x < 0 && y <= 0. The case x == 0 &&
		// y <= 0 is attribute to the case 4.
		// The point x==0 and y==0 is a bug, but
		// will be assigned to 4.
		/// <summary>Calculates which quarter of XY plane the vector lies in.</summary>
		/// <remarks>
		/// Calculates which quarter of XY plane the vector lies in. First quarter is
		/// between vectors (1,0) and (0, 1), second between (0, 1) and (-1, 0), etc.
		/// The quarters are numbered counterclockwise.
		/// Angle intervals corresponding to quarters: 1 : [0 : 90); 2 : [90 : 180);
		/// 3 : [180 : 270); 4 : [270 : 360)
		/// </remarks>
		public int GetQuarter()
		{
			return _getQuarter();
		}

		// Assume vector v1 and v2 have same origin. The function compares the
		// vectors by angle from the x axis to the vector in the counter clockwise
		// direction.
		//   >    >
		//   \   /
		// V3 \ / V1
		//     \
		//      \
		//       >V2
		// _compareVectors(V1, V2) == -1.
		// _compareVectors(V1, V3) == -1
		// _compareVectors(V2, V3) == 1
		//
		internal static int _compareVectors(com.epl.geometry.Point2D v1, com.epl.geometry.Point2D v2)
		{
			int q1 = v1._getQuarter();
			int q2 = v2._getQuarter();
			if (q2 == q1)
			{
				double cross = v1.CrossProduct(v2);
				return cross < 0 ? 1 : (cross > 0 ? -1 : 0);
			}
			else
			{
				return q1 < q2 ? -1 : 1;
			}
		}

		/// <summary>Assume vector v1 and v2 have same origin.</summary>
		/// <remarks>
		/// Assume vector v1 and v2 have same origin. The function compares the
		/// vectors by angle in the counter clockwise direction from the axis X.
		/// For example, V1 makes 30 degree angle counterclockwise from horizontal x axis
		/// V2, makes 270, V3 makes 90, then
		/// compareVectors(V1, V2) == -1.
		/// compareVectors(V1, V3) == -1.
		/// compareVectors(V2, V3) == 1.
		/// </remarks>
		/// <returns>Returns 1 if v1 is less than v2, 0 if equal, and 1 if greater.</returns>
		public static int CompareVectors(com.epl.geometry.Point2D v1, com.epl.geometry.Point2D v2)
		{
			return _compareVectors(v1, v2);
		}

		internal class VectorComparator : System.Collections.Generic.IComparer<com.epl.geometry.Point2D>
		{
			public virtual int Compare(com.epl.geometry.Point2D v1, com.epl.geometry.Point2D v2)
			{
				return _compareVectors((com.epl.geometry.Point2D)v1, (com.epl.geometry.Point2D)v2);
			}
		}

		public static double SqrDistance(com.epl.geometry.Point2D pt1, com.epl.geometry.Point2D pt2)
		{
			double dx = pt1.x - pt2.x;
			double dy = pt1.y - pt2.y;
			return dx * dx + dy * dy;
		}

		public override string ToString()
		{
			return "(" + x + " , " + y + ")";
		}

		public void SetNaN()
		{
			x = com.epl.geometry.NumberUtils.NaN();
			y = com.epl.geometry.NumberUtils.NaN();
		}

		public bool IsNaN()
		{
			return com.epl.geometry.NumberUtils.IsNaN(x) || com.epl.geometry.NumberUtils.IsNaN(y);
		}

		// metric = 1: Manhattan metric
		// 2: Euclidian metric (default)
		// 0: used for L-infinite (max(fabs(x), fabs(y))
		// for predefined metrics, use the DistanceMetricEnum defined in WKSPoint.h
		internal double _norm(int metric)
		{
			if (metric < 0 || _isNan())
			{
				return com.epl.geometry.NumberUtils.NaN();
			}
			switch (metric)
			{
				case 0:
				{
					// L-infinite
					return System.Math.Abs(x) >= System.Math.Abs(y) ? System.Math.Abs(x) : System.Math.Abs(y);
				}

				case 1:
				{
					// L1 or Manhattan metric
					return System.Math.Abs(x) + System.Math.Abs(y);
				}

				case 2:
				{
					// L2 or Euclidean metric
					return System.Math.Sqrt(x * x + y * y);
				}

				default:
				{
					return System.Math.Pow(System.Math.Pow(x, (double)metric) + System.Math.Pow(y, (double)metric), 1.0 / (double)metric);
				}
			}
		}

		/// <summary>
		/// returns signed distance of point from infinite line represented by
		/// pt_1...pt_2.
		/// </summary>
		/// <remarks>
		/// returns signed distance of point from infinite line represented by
		/// pt_1...pt_2. The returned distance is positive if this point lies on the
		/// right-hand side of the line, negative otherwise. If the two input points
		/// are equal, the (positive) distance of this point to p_1 is returned.
		/// </remarks>
		internal double Offset(com.epl.geometry.Point2D pt1, com.epl.geometry.Point2D pt2)
		{
			/* const */
			/* const */
			double newDistance = Distance(pt1, pt2);
			com.epl.geometry.Point2D p = Construct(x, y);
			if (newDistance == 0.0)
			{
				return Distance(p, pt1);
			}
			// get vectors relative to pt_1
			com.epl.geometry.Point2D p2 = new com.epl.geometry.Point2D();
			p2.SetCoords(pt2);
			p2.Sub(pt1);
			p.Sub(pt1);
			double cross = p.CrossProduct(p2);
			return cross / newDistance;
		}

		/// <summary>Calculates the orientation of the triangle formed by p-&gt;q-&gt;r.</summary>
		/// <remarks>
		/// Calculates the orientation of the triangle formed by p-&gt;q-&gt;r. Returns 1
		/// for counter-clockwise, -1 for clockwise, and 0 for collinear. May use
		/// high precision arithmetics for some special degenerate cases.
		/// </remarks>
		public static int OrientationRobust(com.epl.geometry.Point2D p, com.epl.geometry.Point2D q, com.epl.geometry.Point2D r)
		{
			com.epl.geometry.ECoordinate det_ec = new com.epl.geometry.ECoordinate();
			det_ec.Set(q.x);
			det_ec.Sub(p.x);
			com.epl.geometry.ECoordinate rp_y_ec = new com.epl.geometry.ECoordinate();
			rp_y_ec.Set(r.y);
			rp_y_ec.Sub(p.y);
			com.epl.geometry.ECoordinate qp_y_ec = new com.epl.geometry.ECoordinate();
			qp_y_ec.Set(q.y);
			qp_y_ec.Sub(p.y);
			com.epl.geometry.ECoordinate rp_x_ec = new com.epl.geometry.ECoordinate();
			rp_x_ec.Set(r.x);
			rp_x_ec.Sub(p.x);
			det_ec.Mul(rp_y_ec);
			qp_y_ec.Mul(rp_x_ec);
			det_ec.Sub(qp_y_ec);
			if (!det_ec.IsFuzzyZero())
			{
				double det_ec_value = det_ec.Value();
				if (det_ec_value < 0.0)
				{
					return -1;
				}
				if (det_ec_value > 0.0)
				{
					return 1;
				}
				return 0;
			}
			// Need extended precision
			decimal det_mp = new decimal(q.x);
			decimal px_mp = new decimal(p.x);
			decimal py_mp = new decimal(p.y);
			det_mp = decimal.Subtract(det_mp, px_mp);
			decimal rp_y_mp = new decimal(r.y);
			rp_y_mp = decimal.Subtract(rp_y_mp, py_mp);
			decimal qp_y_mp = new decimal(q.y);
			qp_y_mp = decimal.Subtract(qp_y_mp, py_mp);
			decimal rp_x_mp = new decimal(r.x);
			rp_x_mp = decimal.Subtract(rp_x_mp, px_mp);
			det_mp = decimal.Multiply(det_mp, rp_y_mp);
			qp_y_mp = decimal.Multiply(qp_y_mp, rp_x_mp);
			det_mp = decimal.Subtract(det_mp, qp_y_mp);
			return System.Math.Sign(det_mp);
		}

		private static int InCircleRobustMP_(com.epl.geometry.Point2D p, com.epl.geometry.Point2D q, com.epl.geometry.Point2D r, com.epl.geometry.Point2D s)
		{
			decimal sx_mp = new decimal(s.x);
			decimal sy_mp = new decimal(s.y);
			decimal psx_mp = new decimal(p.x);
			decimal psy_mp = new decimal(p.y);
			psx_mp = decimal.Subtract(psx_mp, sx_mp);
			psy_mp = decimal.Subtract(psy_mp, sy_mp);
			decimal qsx_mp = new decimal(q.x);
			decimal qsy_mp = new decimal(q.y);
			qsx_mp = decimal.Subtract(qsx_mp, sx_mp);
			qsy_mp = decimal.Subtract(qsy_mp, sy_mp);
			decimal rsx_mp = new decimal(r.x);
			decimal rsy_mp = new decimal(r.y);
			rsx_mp = decimal.Subtract(rsx_mp, sx_mp);
			rsy_mp = decimal.Subtract(rsy_mp, sy_mp);
			decimal pq_det_mp =decimal.Subtract(decimal.Multiply(psx_mp, qsy_mp), decimal.Multiply(psy_mp, qsx_mp));
			decimal qr_det_mp =decimal.Subtract(decimal.Multiply(qsx_mp, rsy_mp), decimal.Multiply(qsy_mp, rsx_mp));
			decimal pr_det_mp =decimal.Subtract(decimal.Multiply(psx_mp, rsy_mp), decimal.Multiply(psy_mp, rsx_mp));
			decimal p_parab_mp =decimal.Add(decimal.Multiply(psx_mp, psx_mp), decimal.Multiply(psy_mp, psy_mp));
			decimal q_parab_mp =decimal.Add(decimal.Multiply(qsx_mp, qsx_mp), decimal.Multiply(qsy_mp, qsy_mp));
			decimal r_parab_mp =decimal.Add(decimal.Multiply(rsx_mp, rsx_mp), decimal.Multiply(rsy_mp, rsy_mp));
			decimal det_mp = decimal.Add(decimal.Subtract(decimal.Multiply(p_parab_mp, qr_det_mp), (decimal.Multiply(q_parab_mp, pr_det_mp))) , (decimal.Multiply(r_parab_mp, pq_det_mp)));
			return System.Math.Sign(det_mp);
		}

		/// <summary>Calculates if the point s is inside of the circumcircle inscribed by the clockwise oriented triangle p-q-r.</summary>
		/// <remarks>
		/// Calculates if the point s is inside of the circumcircle inscribed by the clockwise oriented triangle p-q-r.
		/// Returns 1 for outside, -1 for inside, and 0 for cocircular.
		/// Note that the convention used here differs from what is commonly found in literature, which can define the relation
		/// in terms of a counter-clockwise oriented circle, and this flips the sign (think of the signed volume of the tetrahedron).
		/// May use high precision arithmetics for some special cases.
		/// </remarks>
		internal static int InCircleRobust(com.epl.geometry.Point2D p, com.epl.geometry.Point2D q, com.epl.geometry.Point2D r, com.epl.geometry.Point2D s)
		{
			com.epl.geometry.ECoordinate psx_ec = new com.epl.geometry.ECoordinate();
			com.epl.geometry.ECoordinate psy_ec = new com.epl.geometry.ECoordinate();
			psx_ec.Set(p.x);
			psx_ec.Sub(s.x);
			psy_ec.Set(p.y);
			psy_ec.Sub(s.y);
			com.epl.geometry.ECoordinate qsx_ec = new com.epl.geometry.ECoordinate();
			com.epl.geometry.ECoordinate qsy_ec = new com.epl.geometry.ECoordinate();
			qsx_ec.Set(q.x);
			qsx_ec.Sub(s.x);
			qsy_ec.Set(q.y);
			qsy_ec.Sub(s.y);
			com.epl.geometry.ECoordinate rsx_ec = new com.epl.geometry.ECoordinate();
			com.epl.geometry.ECoordinate rsy_ec = new com.epl.geometry.ECoordinate();
			rsx_ec.Set(r.x);
			rsx_ec.Sub(s.x);
			rsy_ec.Set(r.y);
			rsy_ec.Sub(s.y);
			com.epl.geometry.ECoordinate psx_ec_qsy_ec = new com.epl.geometry.ECoordinate();
			psx_ec_qsy_ec.Set(psx_ec);
			psx_ec_qsy_ec.Mul(qsy_ec);
			com.epl.geometry.ECoordinate psy_ec_qsx_ec = new com.epl.geometry.ECoordinate();
			psy_ec_qsx_ec.Set(psy_ec);
			psy_ec_qsx_ec.Mul(qsx_ec);
			com.epl.geometry.ECoordinate qsx_ec_rsy_ec = new com.epl.geometry.ECoordinate();
			qsx_ec_rsy_ec.Set(qsx_ec);
			qsx_ec_rsy_ec.Mul(rsy_ec);
			com.epl.geometry.ECoordinate qsy_ec_rsx_ec = new com.epl.geometry.ECoordinate();
			qsy_ec_rsx_ec.Set(qsy_ec);
			qsy_ec_rsx_ec.Mul(rsx_ec);
			com.epl.geometry.ECoordinate psx_ec_rsy_ec = new com.epl.geometry.ECoordinate();
			psx_ec_rsy_ec.Set(psx_ec);
			psx_ec_rsy_ec.Mul(rsy_ec);
			com.epl.geometry.ECoordinate psy_ec_rsx_ec = new com.epl.geometry.ECoordinate();
			psy_ec_rsx_ec.Set(psy_ec);
			psy_ec_rsx_ec.Mul(rsx_ec);
			com.epl.geometry.ECoordinate pq_det_ec = new com.epl.geometry.ECoordinate();
			pq_det_ec.Set(psx_ec_qsy_ec);
			pq_det_ec.Sub(psy_ec_qsx_ec);
			com.epl.geometry.ECoordinate qr_det_ec = new com.epl.geometry.ECoordinate();
			qr_det_ec.Set(qsx_ec_rsy_ec);
			qr_det_ec.Sub(qsy_ec_rsx_ec);
			com.epl.geometry.ECoordinate pr_det_ec = new com.epl.geometry.ECoordinate();
			pr_det_ec.Set(psx_ec_rsy_ec);
			pr_det_ec.Sub(psy_ec_rsx_ec);
			com.epl.geometry.ECoordinate psx_ec_psx_ec = new com.epl.geometry.ECoordinate();
			psx_ec_psx_ec.Set(psx_ec);
			psx_ec_psx_ec.Mul(psx_ec);
			com.epl.geometry.ECoordinate psy_ec_psy_ec = new com.epl.geometry.ECoordinate();
			psy_ec_psy_ec.Set(psy_ec);
			psy_ec_psy_ec.Mul(psy_ec);
			com.epl.geometry.ECoordinate qsx_ec_qsx_ec = new com.epl.geometry.ECoordinate();
			qsx_ec_qsx_ec.Set(qsx_ec);
			qsx_ec_qsx_ec.Mul(qsx_ec);
			com.epl.geometry.ECoordinate qsy_ec_qsy_ec = new com.epl.geometry.ECoordinate();
			qsy_ec_qsy_ec.Set(qsy_ec);
			qsy_ec_qsy_ec.Mul(qsy_ec);
			com.epl.geometry.ECoordinate rsx_ec_rsx_ec = new com.epl.geometry.ECoordinate();
			rsx_ec_rsx_ec.Set(rsx_ec);
			rsx_ec_rsx_ec.Mul(rsx_ec);
			com.epl.geometry.ECoordinate rsy_ec_rsy_ec = new com.epl.geometry.ECoordinate();
			rsy_ec_rsy_ec.Set(rsy_ec);
			rsy_ec_rsy_ec.Mul(rsy_ec);
			com.epl.geometry.ECoordinate p_parab_ec = new com.epl.geometry.ECoordinate();
			p_parab_ec.Set(psx_ec_psx_ec);
			p_parab_ec.Add(psy_ec_psy_ec);
			com.epl.geometry.ECoordinate q_parab_ec = new com.epl.geometry.ECoordinate();
			q_parab_ec.Set(qsx_ec_qsx_ec);
			q_parab_ec.Add(qsy_ec_qsy_ec);
			com.epl.geometry.ECoordinate r_parab_ec = new com.epl.geometry.ECoordinate();
			r_parab_ec.Set(rsx_ec_rsx_ec);
			r_parab_ec.Add(rsy_ec_rsy_ec);
			p_parab_ec.Mul(qr_det_ec);
			q_parab_ec.Mul(pr_det_ec);
			r_parab_ec.Mul(pq_det_ec);
			com.epl.geometry.ECoordinate det_ec = new com.epl.geometry.ECoordinate();
			det_ec.Set(p_parab_ec);
			det_ec.Sub(q_parab_ec);
			det_ec.Add(r_parab_ec);
			if (!det_ec.IsFuzzyZero())
			{
				double det_ec_value = det_ec.Value();
				if (det_ec_value < 0.0)
				{
					return -1;
				}
				if (det_ec_value > 0.0)
				{
					return 1;
				}
				return 0;
			}
			return InCircleRobustMP_(p, q, r, s);
		}

		private static com.epl.geometry.Point2D CalculateCenterFromThreePointsHelperMP_(com.epl.geometry.Point2D from, com.epl.geometry.Point2D mid_point, com.epl.geometry.Point2D to)
		{
			System.Diagnostics.Debug.Assert((!mid_point.IsEqual(to) && !mid_point.IsEqual(from) && !from.IsEqual(to)));
			decimal mx = new decimal(mid_point.x);
			mx = decimal.Subtract(mx, new decimal(from.x));
			decimal my = new decimal(mid_point.y);
			my = decimal.Subtract(my, new decimal(from.y));
			decimal tx = new decimal(to.x);
			tx = decimal.Subtract(tx, new decimal(from.x));
			decimal ty = new decimal(to.y);
			ty = decimal.Subtract(ty, new decimal(from.y));
			decimal d = decimal.Multiply(mx, ty);
			decimal tmp = decimal.Multiply(my, tx);
			d = decimal.Subtract(d, tmp);
			if (System.Math.Sign(d) == 0)
			{
				return com.epl.geometry.Point2D.Construct(com.epl.geometry.NumberUtils.NaN(), com.epl.geometry.NumberUtils.NaN());
			}
			d = decimal.Multiply(d, new decimal(2.0));
			decimal mx2 = decimal.Multiply(mx, mx);
			decimal my2 = decimal.Multiply(my, my);
			decimal m_norm2 = decimal.Add(mx2, my2);
			decimal tx2 = decimal.Multiply(tx, tx);
			decimal ty2 = decimal.Multiply(ty, ty);
			decimal t_norm2 = decimal.Add(tx2, ty2);
			decimal xo = decimal.Multiply(my, t_norm2);
			tmp = decimal.Multiply(ty, m_norm2);
			xo = decimal.Subtract(xo, tmp);
			xo = decimal.Round(decimal.Divide(xo, d), MidpointRounding.ToEven);
			decimal yo = decimal.Multiply(mx, t_norm2);
			tmp = decimal.Multiply(tx, m_norm2);
			yo = decimal.Subtract(yo, tmp);
			yo = decimal.Round(decimal.Divide(yo, d), MidpointRounding.ToEven);
			com.epl.geometry.Point2D center = com.epl.geometry.Point2D.Construct(System.Convert.ToDouble(decimal.Subtract(new decimal(from.x), xo)), System.Convert.ToDouble(decimal.Add(new decimal(from.y), yo)));
			return center;
		}

		private static com.epl.geometry.Point2D CalculateCenterFromThreePointsHelper_(com.epl.geometry.Point2D from, com.epl.geometry.Point2D mid_point, com.epl.geometry.Point2D to)
		{
			System.Diagnostics.Debug.Assert((!mid_point.IsEqual(to) && !mid_point.IsEqual(from) && !from.IsEqual(to)));
			com.epl.geometry.ECoordinate mx = new com.epl.geometry.ECoordinate(mid_point.x);
			mx.Sub(from.x);
			com.epl.geometry.ECoordinate my = new com.epl.geometry.ECoordinate(mid_point.y);
			my.Sub(from.y);
			com.epl.geometry.ECoordinate tx = new com.epl.geometry.ECoordinate(to.x);
			tx.Sub(from.x);
			com.epl.geometry.ECoordinate ty = new com.epl.geometry.ECoordinate(to.y);
			ty.Sub(from.y);
			com.epl.geometry.ECoordinate d = new com.epl.geometry.ECoordinate(mx);
			d.Mul(ty);
			com.epl.geometry.ECoordinate tmp = new com.epl.geometry.ECoordinate(my);
			tmp.Mul(tx);
			d.Sub(tmp);
			if (d.Value() == 0.0)
			{
				return com.epl.geometry.Point2D.Construct(com.epl.geometry.NumberUtils.NaN(), com.epl.geometry.NumberUtils.NaN());
			}
			d.Mul(2.0);
			com.epl.geometry.ECoordinate mx2 = new com.epl.geometry.ECoordinate(mx);
			mx2.Mul(mx);
			com.epl.geometry.ECoordinate my2 = new com.epl.geometry.ECoordinate(my);
			my2.Mul(my);
			com.epl.geometry.ECoordinate m_norm2 = new com.epl.geometry.ECoordinate(mx2);
			m_norm2.Add(my2);
			com.epl.geometry.ECoordinate tx2 = new com.epl.geometry.ECoordinate(tx);
			tx2.Mul(tx);
			com.epl.geometry.ECoordinate ty2 = new com.epl.geometry.ECoordinate(ty);
			ty2.Mul(ty);
			com.epl.geometry.ECoordinate t_norm2 = new com.epl.geometry.ECoordinate(tx2);
			t_norm2.Add(ty2);
			com.epl.geometry.ECoordinate xo = new com.epl.geometry.ECoordinate(my);
			xo.Mul(t_norm2);
			tmp = new com.epl.geometry.ECoordinate(ty);
			tmp.Mul(m_norm2);
			xo.Sub(tmp);
			xo.Div(d);
			com.epl.geometry.ECoordinate yo = new com.epl.geometry.ECoordinate(mx);
			yo.Mul(t_norm2);
			tmp = new com.epl.geometry.ECoordinate(tx);
			tmp.Mul(m_norm2);
			yo.Sub(tmp);
			yo.Div(d);
			com.epl.geometry.Point2D center = com.epl.geometry.Point2D.Construct(from.x - xo.Value(), from.y + yo.Value());
			double r1 = com.epl.geometry.Point2D.Construct(from.x - center.x, from.y - center.y).Length();
			double r2 = com.epl.geometry.Point2D.Construct(mid_point.x - center.x, mid_point.y - center.y).Length();
			double r3 = com.epl.geometry.Point2D.Construct(to.x - center.x, to.y - center.y).Length();
			double @base = r1 + System.Math.Abs(from.x) + System.Math.Abs(mid_point.x) + System.Math.Abs(to.x) + System.Math.Abs(from.y) + System.Math.Abs(mid_point.y) + System.Math.Abs(to.y);
			double tol = 1e-15;
			if ((System.Math.Abs(r1 - r2) <= @base * tol && System.Math.Abs(r1 - r3) <= @base * tol))
			{
				return center;
			}
			//returns center value for MP_value type or when calculated radius value for from - center, mid - center, and to - center are very close.
			return com.epl.geometry.Point2D.Construct(com.epl.geometry.NumberUtils.NaN(), com.epl.geometry.NumberUtils.NaN());
		}

		internal static com.epl.geometry.Point2D CalculateCircleCenterFromThreePoints(com.epl.geometry.Point2D from, com.epl.geometry.Point2D mid_point, com.epl.geometry.Point2D to)
		{
			if (from.IsEqual(to) || from.IsEqual(mid_point) || to.IsEqual(mid_point))
			{
				return new com.epl.geometry.Point2D(com.epl.geometry.NumberUtils.NaN(), com.epl.geometry.NumberUtils.NaN());
			}
			com.epl.geometry.Point2D pt = CalculateCenterFromThreePointsHelper_(from, mid_point, to);
			//use error tracking calculations
			if (pt.IsNaN())
			{
				return CalculateCenterFromThreePointsHelperMP_(from, mid_point, to);
			}
			else
			{
				//use precise calculations
				return pt;
			}
		}

		public override int GetHashCode()
		{
			return com.epl.geometry.NumberUtils.Hash(com.epl.geometry.NumberUtils.Hash(x), y);
		}

		internal double GetAxis(int ordinate)
		{
			System.Diagnostics.Debug.Assert((ordinate == 0 || ordinate == 1));
			return (ordinate == 0 ? x : y);
		}
	}
}
