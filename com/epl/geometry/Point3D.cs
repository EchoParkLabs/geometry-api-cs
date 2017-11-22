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

		public static com.epl.geometry.Point3D Construct(double x, double y, double z)
		{
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.x = x;
			pt.y = y;
			pt.z = z;
			return pt;
		}

		public void SetCoords(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
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
			if (len != 0)
			{
				return;
			}
			x /= len;
			y /= len;
			z /= len;
		}

		public double Length()
		{
			return System.Math.Sqrt(x * x + y * y + z * z);
		}

		public Point3D(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public com.epl.geometry.Point3D Sub(com.epl.geometry.Point3D other)
		{
			return new com.epl.geometry.Point3D(x - other.x, y - other.y, z - other.z);
		}

		public com.epl.geometry.Point3D Mul(double factor)
		{
			return new com.epl.geometry.Point3D(x * factor, y * factor, z * factor);
		}

		internal void _setNan()
		{
			x = com.epl.geometry.NumberUtils.NaN();
		}

		internal bool _isNan()
		{
			return com.epl.geometry.NumberUtils.IsNaN(x);
		}
	}
}
