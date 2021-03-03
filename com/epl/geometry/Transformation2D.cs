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
	/// <summary>The affine transformation class for 2D.</summary>
	/// <remarks>
	/// The affine transformation class for 2D.
	/// Vector is a row:
	/// <code>
	/// <br />           |m11 m12 0|
	/// <br />| x y 1| * |m21 m22 0| = |m11 * x + m21 * y + m31   m12 * x + m22 * y + m32   1|
	/// <br />           |m31 m32 1|
	/// <br />Then elements of the Transformation2D are as follows:
	/// <br />           |xx  yx  0|
	/// <br />| x y 1| * |xy  yy  0| = |xx * x + xy * y + xd   yx * x + yy * y + yd    1|
	/// <br />           |xd  yd  1|
	/// <br />
	/// </code> Matrices are used for transformations of the vectors as rows (case
	/// 2). That means the math expressions on the Geometry matrix operations should
	/// be writen like this: <br />
	/// v' = v * M1 * M2 * M3 = ( (v * M1) * M2 ) * M3, where v is a vector, Mn are
	/// the matrices. <br />
	/// This is equivalent to the following line of code: <br />
	/// ResultVector = (M1.mul(M2).mul(M3)).transform(Vector)
	/// </remarks>
	public sealed class Transformation2D
	{
		/// <summary>Matrix coefficient XX of the transformation.</summary>
		public double xx;

		/// <summary>Matrix coefficient XY of the transformation.</summary>
		public double xy;

		/// <summary>X translation component of the transformation.</summary>
		public double xd;

		/// <summary>Matrix coefficient YX of the transformation.</summary>
		public double yx;

		/// <summary>Matrix coefficient YY of the transformation.</summary>
		public double yy;

		/// <summary>Y translation component of the transformation.</summary>
		public double yd;

		/// <summary>Creates a 2D affine transformation with identity transformation.</summary>
		public Transformation2D()
		{
			SetIdentity();
		}

		/// <summary>Creates a 2D affine transformation with a specified scale.</summary>
		/// <param name="scale">The scale to use for the transformation.</param>
		public Transformation2D(double scale)
		{
			SetScale(scale);
		}

		/// <summary>Initializes a zero transformation.</summary>
		/// <remarks>Initializes a zero transformation. Transforms any coordinate to (0, 0).</remarks>
		public void SetZero()
		{
			xx = 0;
			yy = 0;
			xy = 0;
			yx = 0;
			xd = 0;
			yd = 0;
		}

		internal void Transform(com.epl.geometry.Point2D psrc, com.epl.geometry.Point2D pdst)
		{
			double x = xx * psrc.x + xy * psrc.y + xd;
			double y = yx * psrc.x + yy * psrc.y + yd;
			pdst.x = x;
			pdst.y = y;
		}

		/// <summary>
		/// Returns True when all members of this transformation are equal to the
		/// corresponding members of the other.
		/// </summary>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (!(other is com.epl.geometry.Transformation2D))
			{
				return false;
			}
			com.epl.geometry.Transformation2D that = (com.epl.geometry.Transformation2D)other;
			return (xx == that.xx && xy == that.xy && xd == that.xd && yx == that.yx && yy == that.yy && yd == that.yd);
		}

		/// <summary>Returns the hash code for the 2D transformation.</summary>
		public override int GetHashCode()
		{
			int hash = com.epl.geometry.NumberUtils.Hash(xx);
			hash = com.epl.geometry.NumberUtils.Hash(hash, xy);
			hash = com.epl.geometry.NumberUtils.Hash(hash, xd);
			hash = com.epl.geometry.NumberUtils.Hash(hash, yx);
			hash = com.epl.geometry.NumberUtils.Hash(hash, yy);
			hash = com.epl.geometry.NumberUtils.Hash(hash, yd);
			return hash;
		}

		internal void Transform(com.epl.geometry.Point2D[] points, int start, int count)
		{
			int n = System.Math.Min(points.Length, start + count);
			for (int i = count; i < n; i++)
			{
				Transform(points[i], points[i]);
			}
		}

		/// <summary>Transforms an array of points.</summary>
		/// <param name="pointsIn">The points to be transformed.</param>
		/// <param name="count">The number of points to transform.</param>
		/// <param name="pointsOut">
		/// The transformed points are returned using this array. It
		/// should have the same or greater size as the input array.
		/// </param>
		public void Transform(com.epl.geometry.Point[] pointsIn, int count, com.epl.geometry.Point[] pointsOut)
		{
			com.epl.geometry.Point2D res = new com.epl.geometry.Point2D();
			for (int i = 0; i < count; i++)
			{
				com.epl.geometry.Point2D p = pointsIn[i].GetXY();
				res.x = xx * p.x + xy * p.y + xd;
				res.y = yx * p.x + yy * p.y + yd;
				pointsOut[i] = new com.epl.geometry.Point(res.x, res.y);
			}
		}

		/// <summary>
		/// Transforms an array of points stored in an array of doubles as
		/// interleaved XY coordinates.
		/// </summary>
		/// <param name="pointsXYInterleaved">
		/// The array of points with interleaved X, Y values to be
		/// transformed.
		/// </param>
		/// <param name="start">
		/// The start point index to transform from (the actual element
		/// index is 2 * start).
		/// </param>
		/// <param name="count">
		/// The number of points to transform (the actual element count is
		/// 2 * count).
		/// </param>
		public void Transform(double[] pointsXYInterleaved, int start, int count)
		{
			int n = System.Math.Min(pointsXYInterleaved.Length, (start + count) * 2) / 2;
			for (int i = count; i < n; i++)
			{
				double px = pointsXYInterleaved[2 * i];
				double py = pointsXYInterleaved[2 * i + 1];
				pointsXYInterleaved[2 * i] = xx * px + xy * py + xd;
				pointsXYInterleaved[2 * i + 1] = yx * px + yy * py + yd;
			}
		}

		/// <summary>Multiplies this matrix on the right with the "right" matrix.</summary>
		/// <remarks>
		/// Multiplies this matrix on the right with the "right" matrix. Stores the
		/// result into this matrix and returns a reference to it. <br />
		/// Equivalent to this *= right.
		/// </remarks>
		/// <param name="right">The matrix to be multiplied with.</param>
		public void Multiply(com.epl.geometry.Transformation2D right)
		{
			Multiply(this, right, this);
		}

		/// <summary>Multiplies this matrix on the left with the "left" matrix.</summary>
		/// <remarks>
		/// Multiplies this matrix on the left with the "left" matrix. Stores the
		/// result into this matrix and returns a reference to it. <br />
		/// Equivalent to this = left * this.
		/// </remarks>
		/// <param name="left">The matrix to be multiplied with.</param>
		public void MulLeft(com.epl.geometry.Transformation2D left)
		{
			Multiply(left, this, this);
		}

		/// <summary>
		/// Performs multiplication of matrices a and b and places the result into
		/// this matrix.
		/// </summary>
		/// <remarks>
		/// Performs multiplication of matrices a and b and places the result into
		/// this matrix. The a, b, and result could point to same objects. <br />
		/// Equivalent to result = a * b.
		/// </remarks>
		/// <param name="a">The 2D transformation to be multiplied.</param>
		/// <param name="b">The 2D transformation to be multiplied.</param>
		/// <param name="result">The 2D transformation created by multiplication of matrices.</param>
		public static void Multiply(com.epl.geometry.Transformation2D a, com.epl.geometry.Transformation2D b, com.epl.geometry.Transformation2D result)
		{
			double xx;
			double xy;
			double xd;
			double yx;
			double yy;
			double yd;
			xx = a.xx * b.xx + a.yx * b.xy;
			xy = a.xy * b.xx + a.yy * b.xy;
			xd = a.xd * b.xx + a.yd * b.xy + b.xd;
			yx = a.xx * b.yx + a.yx * b.yy;
			yy = a.xy * b.yx + a.yy * b.yy;
			yd = a.xd * b.yx + a.yd * b.yy + b.yd;
			result.xx = xx;
			result.xy = xy;
			result.xd = xd;
			result.yx = yx;
			result.yy = yy;
			result.yd = yd;
		}

		/// <summary>Returns a copy of the Transformation2D object.</summary>
		/// <returns>A copy of this object.</returns>
		public com.epl.geometry.Transformation2D Copy()
		{
			com.epl.geometry.Transformation2D result = new com.epl.geometry.Transformation2D();
			result.xx = xx;
			result.xy = xy;
			result.xd = xd;
			result.yx = yx;
			result.yy = yy;
			result.yd = yd;
			return result;
		}

		/// <summary>
		/// Writes the matrix coefficients in the order XX, XY, XD, YX, YY, YD into
		/// the given array.
		/// </summary>
		/// <param name="coefs">
		/// The array into which the coefficients are returned. Should be
		/// of size 6 elements.
		/// </param>
		public void GetCoefficients(double[] coefs)
		{
			if (coefs.Length < 6)
			{
				throw new com.epl.geometry.GeometryException("Buffer is too small. coefs needs 6 members");
			}
			coefs[0] = xx;
			coefs[1] = xy;
			coefs[2] = xd;
			coefs[3] = yx;
			coefs[4] = yy;
			coefs[5] = yd;
		}

		/// <summary>Transforms envelope</summary>
		/// <param name="env">The envelope that is to be transformed</param>
		internal void Transform(com.epl.geometry.Envelope2D env)
		{
			if (env.IsEmpty())
			{
				return;
			}
			com.epl.geometry.Point2D[] buf = new com.epl.geometry.Point2D[4];
			env.QueryCorners(buf);
			Transform(buf, buf);
			env.SetFromPoints(buf, 4);
		}

		internal void Transform(com.epl.geometry.Point2D[] pointsIn, com.epl.geometry.Point2D[] pointsOut)
		{
			for (int i = 0; i < pointsIn.Length; i++)
			{
				com.epl.geometry.Point2D res = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D p = pointsIn[i];
				res.x = xx * p.x + xy * p.y + xd;
				res.y = yx * p.x + yy * p.y + yd;
				pointsOut[i] = res;
			}
		}

		/// <summary>Initialize transformation from two rectangles.</summary>
		internal void InitializeFromRect(com.epl.geometry.Envelope2D src, com.epl.geometry.Envelope2D dest)
		{
			if (src.IsEmpty() || dest.IsEmpty() || 0 == src.GetWidth() || 0 == src.GetHeight())
			{
				SetZero();
			}
			else
			{
				xy = yx = 0;
				xx = dest.GetWidth() / src.GetWidth();
				yy = dest.GetHeight() / src.GetHeight();
				xd = dest.xmin - src.xmin * xx;
				yd = dest.ymin - src.ymin * yy;
			}
		}

		/// <summary>
		/// Initializes an orhtonormal transformation from the Src and Dest
		/// rectangles.
		/// </summary>
		/// <remarks>
		/// Initializes an orhtonormal transformation from the Src and Dest
		/// rectangles.
		/// The result transformation proportionally fits the Src into the Dest. The
		/// center of the Src will be in the center of the Dest.
		/// </remarks>
		internal void InitializeFromRectIsotropic(com.epl.geometry.Envelope2D src, com.epl.geometry.Envelope2D dest)
		{
			if (src.IsEmpty() || dest.IsEmpty() || 0 == src.GetWidth() || 0 == src.GetHeight())
			{
				SetZero();
			}
			else
			{
				yx = 0;
				xy = 0;
				xx = dest.GetWidth() / src.GetWidth();
				yy = dest.GetHeight() / src.GetHeight();
				if (xx > yy)
				{
					xx = yy;
				}
				else
				{
					yy = xx;
				}
				com.epl.geometry.Point2D destCenter = dest.GetCenter();
				com.epl.geometry.Point2D srcCenter = src.GetCenter();
				xd = destCenter.x - srcCenter.x * xx;
				yd = destCenter.y - srcCenter.y * yy;
			}
		}

		/// <summary>
		/// Initializes transformation from Position, Tangent vector and offset
		/// value.
		/// </summary>
		/// <remarks>
		/// Initializes transformation from Position, Tangent vector and offset
		/// value. Tangent vector must have unity length
		/// </remarks>
		internal void InitializeFromCurveParameters(com.epl.geometry.Point2D Position, com.epl.geometry.Point2D Tangent, double Offset)
		{
		}

		// TODO
		/// <summary>Transforms size.</summary>
		/// <remarks>
		/// Transforms size.
		/// Creates an AABB with width of SizeSrc.x and height of SizeSrc.y.
		/// Transforms that AABB and gets a quadrangle in new coordinate system. The
		/// result x contains the length of the quadrangle edge, which were parallel
		/// to X in the original system, and y contains the length of the edge, that
		/// were parallel to the Y axis in the original system.
		/// </remarks>
		internal com.epl.geometry.Point2D TransformSize(com.epl.geometry.Point2D SizeSrc)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.x = System.Math.Sqrt(xx * xx + yx * yx) * SizeSrc.x;
			pt.y = System.Math.Sqrt(xy * xy + yy * yy) * SizeSrc.y;
			return pt;
		}

		/// <summary>Transforms a tolerance value.</summary>
		/// <param name="tolerance">The tolerance value.</param>
		public double Transform(double tolerance)
		{
			// the function should be implemented as follows: find encompassing
			// circle for the transformed circle of radius = Tolerance.
			// this is approximation.
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			/*
			* pt[0].Set(0, 0); pt[1].Set(1, 0); pt[2].Set(0, 1); Transform(pt);
			* pt[1] -= pt[0]; pt[2] -= pt[0];
			*/
			pt1.SetCoords(xx, yx);
			pt2.SetCoords(xy, yy);
			pt1.Sub(pt1);
			double d1 = pt1.SqrLength() * 0.5;
			pt1.SetCoords(xx, yx);
			pt2.SetCoords(xy, yy);
			pt1.Add(pt2);
			double d2 = pt1.SqrLength() * 0.5;
			return tolerance * ((d1 > d2) ? System.Math.Sqrt(d1) : System.Math.Sqrt(d2));
		}

		// Performs linear part of the transformation only. Same as if xd, yd would
		// be zeroed.
		internal void TransformWithoutShift(com.epl.geometry.Point2D[] pointsIn, int from, int count, com.epl.geometry.Point2D[] pointsOut)
		{
			for (int i = from, n = from + count; i < n; i++)
			{
				com.epl.geometry.Point2D p = pointsIn[i];
				double new_x = xx * p.x + xy * p.y;
				double new_y = yx * p.x + yy * p.y;
				pointsOut[i].SetCoords(new_x, new_y);
			}
		}

		internal com.epl.geometry.Point2D TransformWithoutShift(com.epl.geometry.Point2D srcPoint)
		{
			double new_x = xx * srcPoint.x + xy * srcPoint.y;
			double new_y = yx * srcPoint.x + yy * srcPoint.y;
			return com.epl.geometry.Point2D.Construct(new_x, new_y);
		}

		/// <summary>Sets this matrix to be the identity matrix.</summary>
		public void SetIdentity()
		{
			xx = 1.0;
			xy = 0;
			xd = 0;
			yx = 0;
			yy = 1.0;
			yd = 0;
		}

		/// <summary>Returns TRUE if this matrix is the identity matrix.</summary>
		public bool IsIdentity()
		{
			return xx == 1.0 && yy == 1.0 && (0 == xy && 0 == xd && 0 == yx && 0 == yd);
		}

		/// <summary>
		/// Returns TRUE if this matrix is an identity matrix within the given
		/// tolerance.
		/// </summary>
		/// <param name="tol">The tolerance value.</param>
		public bool IsIdentity(double tol)
		{
			com.epl.geometry.Point2D pt = com.epl.geometry.Point2D.Construct(0.0, 1.0);
			Transform(pt, pt);
			pt.Sub(com.epl.geometry.Point2D.Construct(0.0, 1.0));
			if (pt.SqrLength() > tol * tol)
			{
				return false;
			}
			pt.SetCoords(0, 0);
			Transform(pt, pt);
			if (pt.SqrLength() > tol * tol)
			{
				return false;
			}
			pt.SetCoords(1.0, 0.0);
			Transform(pt, pt);
			pt.Sub(com.epl.geometry.Point2D.Construct(1.0, 0.0));
			return pt.SqrLength() <= tol * tol;
		}

		/// <summary>Returns TRUE for reflective transformations.</summary>
		/// <remarks>
		/// Returns TRUE for reflective transformations. It inverts the sign of
		/// vector cross product.
		/// </remarks>
		public bool IsReflective()
		{
			return xx * yy - yx * xy < 0;
		}

		/// <summary>Returns TRUE if this transformation is a uniform transformation.</summary>
		/// <remarks>
		/// Returns TRUE if this transformation is a uniform transformation.
		/// The uniform transformation is a transformation, which transforms a square
		/// to a square.
		/// </remarks>
		public bool IsUniform(double eps)
		{
			double v1 = xx * xx + yx * yx;
			double v2 = xy * xy + yy * yy;
			double e = (v1 + v2) * eps;
			return System.Math.Abs(v1 - v2) <= e && System.Math.Abs(xx * xy + yx * yy) <= e;
		}

		/// <summary>Returns TRUE if this transformation is a shift transformation.</summary>
		/// <remarks>
		/// Returns TRUE if this transformation is a shift transformation. The shift
		/// transformation performs shift only.
		/// </remarks>
		public bool IsShift()
		{
			return xx == 1.0 && yy == 1.0 && 0 == xy && 0 == yx;
		}

		/// <summary>
		/// Returns TRUE if this transformation is a shift transformation within the
		/// given tolerance.
		/// </summary>
		/// <param name="tol">The tolerance value.</param>
		public bool IsShift(double tol)
		{
			com.epl.geometry.Point2D pt = TransformWithoutShift(com.epl.geometry.Point2D.Construct(0.0, 1.0));
			pt.y -= 1.0;
			if (pt.SqrLength() > tol * tol)
			{
				return false;
			}
			pt = TransformWithoutShift(com.epl.geometry.Point2D.Construct(1.0, 0.0));
			pt.x -= 1.0;
			return pt.SqrLength() <= tol * tol;
		}

		/// <summary>
		/// Returns TRUE if this is an orthonormal transformation with the given
		/// tolerance.
		/// </summary>
		/// <remarks>
		/// Returns TRUE if this is an orthonormal transformation with the given
		/// tolerance. The orthonormal: Rotation or rotoinversion and shift
		/// (preserves lengths of vectors and angles between vectors).
		/// </remarks>
		/// <param name="tol">The tolerance value.</param>
		public bool IsOrthonormal(double tol)
		{
			com.epl.geometry.Transformation2D r = new com.epl.geometry.Transformation2D();
			r.xx = xx * xx + xy * xy;
			r.xy = xx * yx + xy * yy;
			r.yx = yx * xx + yy * xy;
			r.yy = yx * yx + yy * yy;
			r.xd = 0;
			r.yd = 0;
			return r.IsIdentity(tol);
		}

		/// <summary>
		/// Returns TRUE if this matrix is degenerated (does not have an inverse)
		/// within the given tolerance.
		/// </summary>
		/// <param name="tol">The tolerance value.</param>
		public bool IsDegenerate(double tol)
		{
			return System.Math.Abs(xx * yy - yx * xy) <= 2 * tol * (System.Math.Abs(xx * yy) + System.Math.Abs(yx * xy));
		}

		/// <summary>
		/// Returns TRUE, if this transformation does not have rotation and shear
		/// within the given tolerance.
		/// </summary>
		/// <param name="tol">The tolerance value.</param>
		public bool IsScaleAndShift(double tol)
		{
			return xy * xy + yx * yx < (xx * xx + yy * yy) * tol;
		}

		/// <summary>Set this transformation to be a shift.</summary>
		/// <param name="x">The X coordinate to shift to.</param>
		/// <param name="y">The Y coordinate to shift to.</param>
		public void SetShift(double x, double y)
		{
			xx = 1;
			xy = 0;
			xd = x;
			yx = 0;
			yy = 1;
			yd = y;
		}

		/// <summary>Set this transformation to be a scale.</summary>
		/// <param name="x">The X coordinate to scale to.</param>
		/// <param name="y">The Y coordinate to scale to.</param>
		public void SetScale(double x, double y)
		{
			xx = x;
			xy = 0;
			xd = 0;
			yx = 0;
			yy = y;
			yd = 0;
		}

		/// <summary>Set transformation to be a uniform scale.</summary>
		/// <param name="_scale">The scale of the transformation.</param>
		public void SetScale(double _scale)
		{
			SetScale(_scale, _scale);
		}

		/// <summary>Sets the transformation to be a flip around the X axis.</summary>
		/// <remarks>
		/// Sets the transformation to be a flip around the X axis. Flips the X
		/// coordinates so that the x0 becomes x1 and vice verse.
		/// </remarks>
		/// <param name="x0">The X coordinate to flip.</param>
		/// <param name="x1">The X coordinate to flip to.</param>
		public void SetFlipX(double x0, double x1)
		{
			xx = -1;
			xy = 0;
			xd = x0 + x1;
			yx = 0;
			yy = 1;
			yd = 0;
		}

		/// <summary>Sets the transformation to be a flip around the Y axis.</summary>
		/// <remarks>
		/// Sets the transformation to be a flip around the Y axis. Flips the Y
		/// coordinates so that the y0 becomes y1 and vice verse.
		/// </remarks>
		/// <param name="y0">The Y coordinate to flip.</param>
		/// <param name="y1">The Y coordinate to flip to.</param>
		public void SetFlipY(double y0, double y1)
		{
			xx = 1;
			xy = 0;
			xd = 0;
			yx = 0;
			yy = -1;
			yd = y0 + y1;
		}

		/// <summary>Set transformation to a shear.</summary>
		/// <param name="proportionX">The proportion of shearing in x direction.</param>
		/// <param name="proportionY">The proportion of shearing in y direction.</param>
		public void SetShear(double proportionX, double proportionY)
		{
			xx = 1;
			xy = proportionX;
			xd = 0;
			yx = proportionY;
			yy = 1;
			yd = 0;
		}

		/// <summary>Sets this transformation to be a rotation around point (0, 0).</summary>
		/// <remarks>
		/// Sets this transformation to be a rotation around point (0, 0).
		/// When the axis Y is directed up and X is directed to the right, the
		/// positive angle corresponds to the anti-clockwise rotation. When the axis
		/// Y is directed down and X is directed to the right, the positive angle
		/// corresponds to the clockwise rotation.
		/// </remarks>
		/// <param name="angle_in_Radians">The rotation angle in radian.</param>
		public void SetRotate(double angle_in_Radians)
		{
			SetRotate(System.Math.Cos(angle_in_Radians), System.Math.Sin(angle_in_Radians));
		}

		/// <summary>Produces a transformation that swaps x and y coordinate values.</summary>
		/// <remarks>
		/// Produces a transformation that swaps x and y coordinate values. xx = 0.0;
		/// xy = 1.0; xd = 0; yx = 1.0; yy = 0.0; yd = 0;
		/// </remarks>
		internal com.epl.geometry.Transformation2D SetSwapCoordinates()
		{
			xx = 0.0;
			xy = 1.0;
			xd = 0;
			yx = 1.0;
			yy = 0.0;
			yd = 0;
			return this;
		}

		/// <summary>Sets this transformation to be a rotation around point rotationCenter.</summary>
		/// <remarks>
		/// Sets this transformation to be a rotation around point rotationCenter.
		/// When the axis Y is directed up and X is directed to the right, the
		/// positive angle corresponds to the anti-clockwise rotation. When the axis
		/// Y is directed down and X is directed to the right, the positive angle
		/// corresponds to the clockwise rotation.
		/// </remarks>
		/// <param name="angle_in_Radians">The rotation angle in radian.</param>
		/// <param name="rotationCenter">The center point of the rotation.</param>
		internal void SetRotate(double angle_in_Radians, com.epl.geometry.Point2D rotationCenter)
		{
			SetRotate(System.Math.Cos(angle_in_Radians), System.Math.Sin(angle_in_Radians), rotationCenter);
		}

		/// <summary>Sets rotation for this transformation.</summary>
		/// <remarks>
		/// Sets rotation for this transformation.
		/// When the axis Y is directed up and X is directed to the right, the
		/// positive angle corresponds to the anti-clockwise rotation. When the axis
		/// Y is directed down and X is directed to the right, the positive angle
		/// corresponds to the clockwise rotation.
		/// </remarks>
		/// <param name="cosA">The rotation angle.</param>
		/// <param name="sinA">The rotation angle.</param>
		public void SetRotate(double cosA, double sinA)
		{
			xx = cosA;
			xy = -sinA;
			xd = 0;
			yx = sinA;
			yy = cosA;
			yd = 0;
		}

		/// <summary>Sets this transformation to be a rotation around point rotationCenter.</summary>
		/// <remarks>
		/// Sets this transformation to be a rotation around point rotationCenter.
		/// When the axis Y is directed up and X is directed to the right, the
		/// positive angle corresponds to the anti-clockwise rotation. When the axis
		/// Y is directed down and X is directed to the right, the positive angle
		/// corresponds to the clockwise rotation.
		/// </remarks>
		/// <param name="cosA">The cos of the rotation angle.</param>
		/// <param name="sinA">The sin of the rotation angle.</param>
		/// <param name="rotationCenter">The center point of the rotation.</param>
		internal void SetRotate(double cosA, double sinA, com.epl.geometry.Point2D rotationCenter)
		{
			SetShift(-rotationCenter.x, -rotationCenter.y);
			com.epl.geometry.Transformation2D temp = new com.epl.geometry.Transformation2D();
			temp.SetRotate(cosA, sinA);
			Multiply(temp);
			Shift(rotationCenter.x, rotationCenter.y);
		}

		/// <summary>Shifts the transformation.</summary>
		/// <param name="x">The shift factor in X direction.</param>
		/// <param name="y">The shift factor in Y direction.</param>
		public void Shift(double x, double y)
		{
			xd += x;
			yd += y;
		}

		/// <summary>Scales the transformation.</summary>
		/// <param name="x">The scale factor in X direction.</param>
		/// <param name="y">The scale factor in Y direction.</param>
		public void Scale(double x, double y)
		{
			xx *= x;
			xy *= x;
			xd *= x;
			yx *= y;
			yy *= y;
			yd *= y;
		}

		/// <summary>Flips the transformation around the X axis.</summary>
		/// <param name="x0">The X coordinate to flip.</param>
		/// <param name="x1">The X coordinate to flip to.</param>
		public void FlipX(double x0, double x1)
		{
			xx = -xx;
			xy = -xy;
			xd = x0 + x1 - xd;
		}

		/// <summary>Flips the transformation around the Y axis.</summary>
		/// <param name="y0">The Y coordinate to flip.</param>
		/// <param name="y1">The Y coordinate to flip to.</param>
		public void FlipY(double y0, double y1)
		{
			yx = -yx;
			yy = -yy;
			yd = y0 + y1 - yd;
		}

		/// <summary>Shears the transformation.</summary>
		/// <param name="proportionX">The proportion of shearing in x direction.</param>
		/// <param name="proportionY">The proportion of shearing in y direction.</param>
		public void Shear(double proportionX, double proportionY)
		{
			com.epl.geometry.Transformation2D temp = new com.epl.geometry.Transformation2D();
			temp.SetShear(proportionX, proportionY);
			Multiply(temp);
		}

		/// <summary>Rotates the transformation.</summary>
		/// <param name="angle_in_Radians">The rotation angle in radian.</param>
		public void Rotate(double angle_in_Radians)
		{
			com.epl.geometry.Transformation2D temp = new com.epl.geometry.Transformation2D();
			temp.SetRotate(angle_in_Radians);
			Multiply(temp);
		}

		/// <summary>Rotates the transformation.</summary>
		/// <param name="cos">The cos angle of the rotation.</param>
		/// <param name="sin">The sin angle of the rotation.</param>
		public void Rotate(double cos, double sin)
		{
			com.epl.geometry.Transformation2D temp = new com.epl.geometry.Transformation2D();
			temp.SetRotate(cos, sin);
			Multiply(temp);
		}

		/// <summary>Rotates the transformation aroung a center point.</summary>
		/// <param name="cos">The cos angle of the rotation.</param>
		/// <param name="sin">sin angle of the rotation.</param>
		/// <param name="rotationCenter">The center point of the rotation.</param>
		public void Rotate(double cos, double sin, com.epl.geometry.Point2D rotationCenter)
		{
			com.epl.geometry.Transformation2D temp = new com.epl.geometry.Transformation2D();
			temp.SetRotate(cos, sin, rotationCenter);
			Multiply(temp);
		}

		/// <summary>
		/// Produces inverse matrix for this matrix and puts result into the inverse
		/// parameter.
		/// </summary>
		/// <param name="inverse">The result inverse matrix.</param>
		public void Inverse(com.epl.geometry.Transformation2D inverse)
		{
			double det = xx * yy - xy * yx;
			if (det == 0)
			{
				inverse.SetZero();
				return;
			}
			det = 1 / det;
			inverse.xd = (xy * yd - xd * yy) * det;
			inverse.yd = (xd * yx - xx * yd) * det;
			inverse.xx = yy * det;
			inverse.xy = -xy * det;
			inverse.yx = -yx * det;
			inverse.yy = xx * det;
		}

		/// <summary>Inverses the matrix.</summary>
		public void Inverse()
		{
			Inverse(this);
		}

		/// <summary>Extracts scaling part of the transformation.</summary>
		/// <remarks>
		/// Extracts scaling part of the transformation. this == scale
		/// rotateNshearNshift.
		/// </remarks>
		/// <param name="scale">The destination matrix where the scale part is copied.</param>
		/// <param name="rotateNshearNshift">
		/// The destination matrix where the part excluding rotation is
		/// copied.
		/// </param>
		public void ExtractScaleTransform(com.epl.geometry.Transformation2D scale, com.epl.geometry.Transformation2D rotateNshearNshift)
		{
			scale.SetScale(System.Math.Sqrt(xx * xx + xy * xy), System.Math.Sqrt(yx * yx + yy * yy));
			rotateNshearNshift.SetScale(1.0 / scale.xx, 1.0 / scale.yy);
			rotateNshearNshift.Multiply(this);
		}
	}
}
