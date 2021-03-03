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
	/// <summary>Basic 3D point class.</summary>
	[System.Serializable]
	public sealed class Point3D
	{
		private const long serialVersionUID = 1L;

		public double x;

		public double y;

		public double z;

		public Point3D()
		{
		}

		public Point3D(com.epl.geometry.Point3D other)
		{
			SetCoords(other);
		}

		public Point3D(double x, double y, double z)
		{
			SetCoords(x, y, z);
		}

		public static com.epl.geometry.Point3D Construct(double x, double y, double z)
		{
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.SetCoords(x, y, z);
			return pt;
		}

		public void SetCoords(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void SetCoords(com.epl.geometry.Point3D other)
		{
			SetCoords(other.x, other.y, other.z);
		}

		public void SetZero()
		{
			x = 0.0;
			y = 0.0;
			z = 0.0;
		}

		public void Normalize()
		{
			double len = Length();
			if (len == 0)
			{
				x = 1.0;
				y = 0.0;
				z = 0.0;
			}
			else
			{
				x /= len;
				y /= len;
				z /= len;
			}
		}

		public double DotProduct(com.epl.geometry.Point3D other)
		{
			return x * other.x + y * other.y + z * other.z;
		}

		public double SqrLength()
		{
			return x * x + y * y + z * z;
		}

		public double Length()
		{
			return System.Math.Sqrt(x * x + y * y + z * z);
		}

		public void Sub(com.epl.geometry.Point3D other)
		{
			x -= other.x;
			y -= other.y;
			z -= other.z;
		}

		public void Sub(com.epl.geometry.Point3D p1, com.epl.geometry.Point3D p2)
		{
			x = p1.x - p2.x;
			y = p1.y - p2.y;
			z = p1.z - p2.z;
		}

		public void Scale(double f, com.epl.geometry.Point3D other)
		{
			x = f * other.x;
			y = f * other.y;
			z = f * other.z;
		}

		public void Mul(double factor)
		{
			x *= factor;
			y *= factor;
			z *= factor;
		}

		internal void _setNan()
		{
			x = com.epl.geometry.NumberUtils.NaN();
			y = com.epl.geometry.NumberUtils.NaN();
			z = com.epl.geometry.NumberUtils.NaN();
		}

		internal bool _isNan()
		{
			return com.epl.geometry.NumberUtils.IsNaN(x) || com.epl.geometry.NumberUtils.IsNaN(y) || com.epl.geometry.NumberUtils.IsNaN(z);
		}
	}
}
