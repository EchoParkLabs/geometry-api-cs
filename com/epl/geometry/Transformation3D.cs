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
	/// <summary>The 3D affine transformation class.</summary>
	/// <remarks>
	/// The 3D affine transformation class.
	/// We use matrices for transformations of the vectors as rows. That means the
	/// math expressions on the Geometry matrix operations should be writen like
	/// this: v' = v * M1 * M2 * M3 = ( (v * M1) * M2 ) * M3, where v is a vector, Mn
	/// are the matrices. This is equivalent to the following line of code:
	/// ResultVector = (M1.Mul(M2).Mul(M3)).Transform(Vector)
	/// </remarks>
	internal sealed class Transformation3D
	{
		public double xx;

		public double yx;

		public double zx;

		public double xd;

		public double xy;

		public double yy;

		public double zy;

		public double yd;

		public double xz;

		public double yz;

		public double zz;

		public double zd;

		public Transformation3D()
		{
		}

		/// <summary>Sets all elements to 0, thus producing and invalid transformation.</summary>
		public void SetZero()
		{
			xx = 0.0;
			yx = 0.0;
			zx = 0.0;
			xy = 0.0;
			yy = 0.0;
			zy = 0.0;
			xz = 0.0;
			yz = 0.0;
			zz = 0.0;
			xd = 0.0;
			yd = 0.0;
			zd = 0.0;
		}

		public void SetScale(double scaleX, double scaleY, double scaleZ)
		{
			xx = scaleX;
			yx = 0.0;
			zx = 0.0;
			xy = 0.0;
			yy = scaleY;
			zy = 0.0;
			xz = 0.0;
			yz = 0.0;
			zz = scaleZ;
			xd = 0.0;
			yd = 0.0;
			zd = 0.0;
		}

		public void SetTranslate(double deltax, double deltay, double deltaz)
		{
			xx = 1.0;
			yx = 0.0;
			zx = 0.0;
			xy = 0.0;
			yy = 1.0;
			zy = 0.0;
			xz = 0.0;
			yz = 0.0;
			zz = 1.0;
			xd = deltax;
			yd = deltay;
			zd = deltaz;
		}

		public void Translate(double deltax, double deltay, double deltaz)
		{
			xd += deltax;
			yd += deltay;
			zd += deltaz;
		}

		/// <summary>Transforms an envelope.</summary>
		/// <remarks>
		/// Transforms an envelope. The result is the bounding box of the transformed
		/// envelope.
		/// </remarks>
		public com.epl.geometry.Envelope3D Transform(com.epl.geometry.Envelope3D env)
		{
			if (env.IsEmpty())
			{
				return env;
			}
			com.epl.geometry.Point3D[] buf = new com.epl.geometry.Point3D[8];
			env.QueryCorners(buf);
			Transform(buf, 8, buf);
			env.SetFromPoints(buf);
			return env;
		}

		internal void Transform(com.epl.geometry.Point3D[] pointsIn, int count, com.epl.geometry.Point3D[] pointsOut)
		{
			for (int i = 0; i < count; i++)
			{
				com.epl.geometry.Point3D res = new com.epl.geometry.Point3D();
				com.epl.geometry.Point3D src = pointsIn[i];
				res.x = xx * src.x + xy * src.y + xz * src.z + xd;
				res.y = yx * src.x + yy * src.y + yz * src.z + yd;
				res.z = zx * src.x + zy * src.y + zz * src.z + zd;
				pointsOut[i] = res;
			}
		}

		public com.epl.geometry.Point3D Transform(com.epl.geometry.Point3D src)
		{
			com.epl.geometry.Point3D res = new com.epl.geometry.Point3D();
			res.x = xx * src.x + xy * src.y + xz * src.z + xd;
			res.y = yx * src.x + yy * src.y + yz * src.z + yd;
			res.z = zx * src.x + zy * src.y + zz * src.z + zd;
			return res;
		}

		public void Transform(com.epl.geometry.Point3D[] points, int start, int count)
		{
			int n = System.Math.Min(points.Length, start + count);
			for (int i = start; i < n; i++)
			{
				com.epl.geometry.Point3D res = new com.epl.geometry.Point3D();
				com.epl.geometry.Point3D src = points[i];
				res.x = xx * src.x + xy * src.y + xz * src.z + xd;
				res.y = yx * src.x + yy * src.y + yz * src.z + yd;
				res.z = zx * src.x + zy * src.y + zz * src.z + zd;
				points[i] = res;
			}
		}

		public void Mul(com.epl.geometry.Transformation3D right)
		{
			Multiply(this, right, this);
		}

		public void MulLeft(com.epl.geometry.Transformation3D left)
		{
			Multiply(left, this, this);
		}

		/// <summary>
		/// Performs multiplication of matrices a and b and places result into
		/// result.
		/// </summary>
		/// <remarks>
		/// Performs multiplication of matrices a and b and places result into
		/// result. The a, b, and result could point to same objects. <br />
		/// Equivalent to result = a * b.
		/// </remarks>
		public static void Multiply(com.epl.geometry.Transformation3D a, com.epl.geometry.Transformation3D b, com.epl.geometry.Transformation3D result)
		{
			// static
			double xx;
			double yx;
			double zx;
			double xy;
			double yy;
			double zy;
			double xz;
			double yz;
			double zz;
			double xd;
			double yd;
			double zd;
			xx = a.xx * b.xx + a.yx * b.xy + a.zx * b.xz;
			yx = a.xx * b.yx + a.yx * b.yy + a.zx * b.yz;
			zx = a.xx * b.zx + a.yx * b.zy + a.zx * b.zz;
			xy = a.xy * b.xx + a.yy * b.xy + a.zy * b.xz;
			yy = a.xy * b.yx + a.yy * b.yy + a.zy * b.yz;
			zy = a.xy * b.zx + a.yy * b.zy + a.zy * b.zz;
			xz = a.xz * b.xx + a.yz * b.xy + a.zz * b.xz;
			yz = a.xz * b.yx + a.yz * b.yy + a.zz * b.yz;
			zz = a.xz * b.zx + a.yz * b.zy + a.zz * b.zz;
			xd = a.xd * b.xx + a.yd * b.xy + a.zd * b.xz + b.xd;
			yd = a.xd * b.yx + a.yd * b.yy + a.zd * b.yz + b.yd;
			zd = a.xd * b.zx + a.yd * b.zy + a.zd * b.zz + b.zd;
			result.xx = xx;
			result.yx = yx;
			result.zx = zx;
			result.xy = xy;
			result.yy = yy;
			result.zy = zy;
			result.xz = xz;
			result.yz = yz;
			result.zz = zz;
			result.xd = xd;
			result.yd = yd;
			result.zd = zd;
		}

		/// <summary>Calculates the Inverse transformation.</summary>
		/// <param name="src">The input transformation.</param>
		/// <param name="result">
		/// The inverse of the input transformation. Throws the
		/// GeometryException("math singularity") exception if the Inverse
		/// can not be calculated.
		/// </param>
		public static void Inverse(com.epl.geometry.Transformation3D src, com.epl.geometry.Transformation3D result)
		{
			double det = src.xx * (src.yy * src.zz - src.zy * src.yz) - src.yx * (src.xy * src.zz - src.zy * src.xz) + src.zx * (src.xy * src.yz - src.yy * src.xz);
			if (det != 0)
			{
				double xx;
				double yx;
				double zx;
				double xy;
				double yy;
				double zy;
				double xz;
				double yz;
				double zz;
				double xd;
				double yd;
				double zd;
				double det_1 = 1.0 / det;
				xx = (src.yy * src.zz - src.zy * src.yz) * det_1;
				xy = -(src.xy * src.zz - src.zy * src.xz) * det_1;
				xz = (src.xy * src.yz - src.yy * src.xz) * det_1;
				yx = -(src.yx * src.zz - src.yz * src.zx) * det_1;
				yy = (src.xx * src.zz - src.zx * src.xz) * det_1;
				yz = -(src.xx * src.yz - src.yx * src.xz) * det_1;
				zx = (src.yx * src.zy - src.zx * src.yy) * det_1;
				zy = -(src.xx * src.zy - src.zx * src.xy) * det_1;
				zz = (src.xx * src.yy - src.yx * src.xy) * det_1;
				xd = -(src.xd * xx + src.yd * xy + src.zd * xz);
				yd = -(src.xd * yx + src.yd * yy + src.zd * yz);
				zd = -(src.xd * zx + src.yd * zy + src.zd * zz);
				result.xx = xx;
				result.yx = yx;
				result.zx = zx;
				result.xy = xy;
				result.yy = yy;
				result.zy = zy;
				result.xz = xz;
				result.yz = yz;
				result.zz = zz;
				result.xd = xd;
				result.yd = yd;
				result.zd = zd;
			}
			else
			{
				throw new com.epl.geometry.GeometryException("math singularity");
			}
		}

		public com.epl.geometry.Transformation3D Copy()
		{
			com.epl.geometry.Transformation3D result = new com.epl.geometry.Transformation3D();
			result.xx = xx;
			result.yx = yx;
			result.zx = zx;
			result.xy = xy;
			result.yy = yy;
			result.zy = zy;
			result.xz = xz;
			result.yz = yz;
			result.zz = zz;
			result.xd = xd;
			result.yd = yd;
			result.zd = zd;
			return result;
		}
	}
}
