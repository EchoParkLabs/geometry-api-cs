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
			x = x * (1.0 - alpha) + other.x * alpha;
			y = y * (1.0 - alpha) + other.y * alpha;
		}

		public void Interpolate(com.epl.geometry.Point2D p1, com.epl.geometry.Point2D p2, double alpha)
		{
			x = p1.x * (1.0 - alpha) + p2.x * alpha;
			y = p1.y * (1.0 - alpha) + p2.y * alpha;
		}

		public void ScaleAdd(double f, com.epl.geometry.Point2D shift)
		{
			x = x * f + shift.x;
			y = y * f + shift.y;
		}

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

		/// <summary>Compares two vertices lexicographicaly.</summary>
		public int Compare(com.epl.geometry.Point2D other)
		{
			return y < other.y ? -1 : (y > other.y ? 1 : (x < other.x ? -1 : (x > other.x ? 1 : 0)));
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
			return com.epl.geometry.NumberUtils.IsNaN(x);
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
			rp_y_mp = rp_y_mp - py_mp;
			decimal qp_y_mp = new decimal(q.y);
			qp_y_mp = qp_y_mp - py_mp;
			decimal rp_x_mp = new decimal(r.x);
			rp_x_mp = rp_x_mp - px_mp;
			det_mp = det_mp * rp_y_mp;
			qp_y_mp = qp_y_mp * rp_x_mp;
			det_mp = det_mp - qp_y_mp;
			return System.Math.Sign(det_mp);
		}

		public override int GetHashCode()
		{
			return com.epl.geometry.NumberUtils.Hash(com.epl.geometry.NumberUtils.Hash(x), y);
		}
	}
}
