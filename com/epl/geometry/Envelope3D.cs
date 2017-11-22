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
	/// <summary>A class that represents axis parallel 3D rectangle.</summary>
	[System.Serializable]
	public sealed class Envelope3D
	{
		private const long serialVersionUID = 1L;

		public double xmin;

		public double ymin;

		public double zmin;

		public double xmax;

		public double ymax;

		public double zmax;

		public static com.epl.geometry.Envelope3D Construct(double _xmin, double _ymin, double _zmin, double _xmax, double _ymax, double _zmax)
		{
			com.epl.geometry.Envelope3D env = new com.epl.geometry.Envelope3D();
			env.xmin = _xmin;
			env.ymin = _ymin;
			env.zmin = _zmin;
			env.xmax = _xmax;
			env.ymax = _ymax;
			env.zmax = _zmax;
			return env;
		}

		public Envelope3D()
		{
		}

		public void SetInfinite()
		{
			xmin = com.epl.geometry.NumberUtils.NegativeInf();
			xmax = com.epl.geometry.NumberUtils.PositiveInf();
			ymin = com.epl.geometry.NumberUtils.NegativeInf();
			ymax = com.epl.geometry.NumberUtils.PositiveInf();
			zmin = com.epl.geometry.NumberUtils.NegativeInf();
			zmax = com.epl.geometry.NumberUtils.PositiveInf();
		}

		public void SetEmpty()
		{
			xmin = com.epl.geometry.NumberUtils.NaN();
			zmin = com.epl.geometry.NumberUtils.NaN();
		}

		public bool IsEmpty()
		{
			return com.epl.geometry.NumberUtils.IsNaN(xmin);
		}

		public void SetEmptyZ()
		{
			zmin = com.epl.geometry.NumberUtils.NaN();
		}

		public bool IsEmptyZ()
		{
			return com.epl.geometry.NumberUtils.IsNaN(zmin);
		}

		public bool HasEmptyDimension()
		{
			return IsEmpty() || IsEmptyZ();
		}

		public void SetCoords(double _xmin, double _ymin, double _zmin, double _xmax, double _ymax, double _zmax)
		{
			xmin = _xmin;
			ymin = _ymin;
			zmin = _zmin;
			xmax = _xmax;
			ymax = _ymax;
			zmax = _zmax;
		}

		public void SetCoords(double _x, double _y, double _z)
		{
			xmin = _x;
			ymin = _y;
			zmin = _z;
			xmax = _x;
			ymax = _y;
			zmax = _z;
		}

		public void SetCoords(com.epl.geometry.Point3D center, double width, double height, double depth)
		{
			xmin = center.x - width * 0.5;
			xmax = xmin + width;
			ymin = center.y - height * 0.5;
			ymax = ymin + height;
			zmin = center.z - depth * 0.5;
			zmax = zmin + depth;
		}

		public void Move(com.epl.geometry.Point3D vector)
		{
			xmin += vector.x;
			ymin += vector.y;
			zmin += vector.z;
			xmax += vector.x;
			ymax += vector.y;
			zmax += vector.z;
		}

		public void CopyTo(com.epl.geometry.Envelope2D env)
		{
			env.xmin = xmin;
			env.ymin = ymin;
			env.xmax = xmax;
			env.ymax = ymax;
		}

		public void MergeNE(double x, double y, double z)
		{
			if (xmin > x)
			{
				xmin = x;
			}
			else
			{
				if (xmax < x)
				{
					xmax = x;
				}
			}
			if (ymin > y)
			{
				ymin = y;
			}
			else
			{
				if (ymax < y)
				{
					ymax = y;
				}
			}
			if (zmin != com.epl.geometry.NumberUtils.NaN())
			{
				if (zmin > z)
				{
					zmin = z;
				}
				else
				{
					if (zmax < z)
					{
						zmax = z;
					}
				}
			}
			else
			{
				zmin = z;
				zmax = z;
			}
		}

		public void Merge(double x, double y, double z)
		{
			if (IsEmpty())
			{
				xmin = x;
				ymin = y;
				zmin = z;
				xmax = x;
				ymax = y;
				zmax = z;
			}
			else
			{
				MergeNE(x, y, z);
			}
		}

		public void Merge(com.epl.geometry.Point3D pt)
		{
			Merge(pt.x, pt.y, pt.z);
		}

		public void Merge(com.epl.geometry.Envelope3D other)
		{
			if (other.IsEmpty())
			{
				return;
			}
			Merge(other.xmin, other.ymin, other.zmin);
			Merge(other.xmax, other.ymax, other.zmax);
		}

		public void Merge(double x1, double y1, double z1, double x2, double y2, double z2)
		{
			Merge(x1, y1, z1);
			Merge(x2, y2, z2);
		}

		public void Construct(com.epl.geometry.Envelope1D xinterval, com.epl.geometry.Envelope1D yinterval, com.epl.geometry.Envelope1D zinterval)
		{
			if (xinterval.IsEmpty() || yinterval.IsEmpty())
			{
				SetEmpty();
				return;
			}
			xmin = xinterval.vmin;
			xmax = xinterval.vmax;
			ymin = yinterval.vmin;
			ymax = yinterval.vmax;
			zmin = zinterval.vmin;
			zmax = zinterval.vmax;
		}

		public void QueryCorners(com.epl.geometry.Point3D[] corners)
		{
			if ((corners == null) || (corners.Length < 8))
			{
				throw new System.ArgumentException();
			}
			corners[0] = new com.epl.geometry.Point3D(xmin, ymin, zmin);
			corners[1] = new com.epl.geometry.Point3D(xmin, ymax, zmin);
			corners[2] = new com.epl.geometry.Point3D(xmax, ymax, zmin);
			corners[3] = new com.epl.geometry.Point3D(xmax, ymin, zmin);
			corners[4] = new com.epl.geometry.Point3D(xmin, ymin, zmax);
			corners[5] = new com.epl.geometry.Point3D(xmin, ymax, zmax);
			corners[6] = new com.epl.geometry.Point3D(xmax, ymax, zmax);
			corners[7] = new com.epl.geometry.Point3D(xmax, ymin, zmax);
		}

		public void SetFromPoints(com.epl.geometry.Point3D[] points)
		{
			if (points == null || points.Length == 0)
			{
				SetEmpty();
				return;
			}
			com.epl.geometry.Point3D p = points[0];
			SetCoords(p.x, p.y, p.z);
			for (int i = 1; i < points.Length; i++)
			{
				com.epl.geometry.Point3D pt = points[i];
				MergeNE(pt.x, pt.y, pt.z);
			}
		}
	}
}
