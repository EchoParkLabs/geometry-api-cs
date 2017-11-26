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
	/// <summary>An axis parallel 2-dimensional rectangle.</summary>
	[System.Serializable]
	public sealed class Envelope2D
	{
		private const long serialVersionUID = 1L;

		private const int XLESSXMIN = 1;

		private const int YLESSYMIN = 4;

		private const int XMASK = 3;

		private const int YMASK = 12;

		public double xmin;

		public double ymin;

		public double xmax;

		public double ymax;

		// private final int XGREATERXMAX = 2;
		// private final int YGREATERYMAX = 8;
		public static com.epl.geometry.Envelope2D Construct(double _xmin, double _ymin, double _xmax, double _ymax)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.xmin = _xmin;
			env.ymin = _ymin;
			env.xmax = _xmax;
			env.ymax = _ymax;
			return env;
		}

		public static com.epl.geometry.Envelope2D Construct(com.epl.geometry.Envelope2D other)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetCoords(other);
			return env;
		}

		public Envelope2D()
		{
			SetEmpty();
		}

		public Envelope2D(double _xmin, double _ymin, double _xmax, double _ymax)
		{
			xmin = _xmin;
			ymin = _ymin;
			xmax = _xmax;
			ymax = _ymax;
		}

		public Envelope2D(com.epl.geometry.Envelope2D other)
		{
			SetCoords(other);
		}

		public void SetCoords(double _x, double _y)
		{
			xmin = _x;
			ymin = _y;
			xmax = _x;
			ymax = _y;
		}

		public void SetCoords(double _xmin, double _ymin, double _xmax, double _ymax)
		{
			xmin = _xmin;
			ymin = _ymin;
			xmax = _xmax;
			ymax = _ymax;
			Normalize();
		}

		public void SetCoords(com.epl.geometry.Point2D center, double width, double height)
		{
			xmin = center.x - width * 0.5;
			xmax = xmin + width;
			ymin = center.y - height * 0.5;
			ymax = ymin + height;
			Normalize();
		}

		public void SetCoords(com.epl.geometry.Point2D pt)
		{
			xmin = pt.x;
			ymin = pt.y;
			xmax = pt.x;
			ymax = pt.y;
		}

		public void SetCoords(com.epl.geometry.Envelope2D envSrc)
		{
			SetCoords(envSrc.xmin, envSrc.ymin, envSrc.xmax, envSrc.ymax);
		}

		public com.epl.geometry.Envelope2D GetInflated(double dx, double dy)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetCoords(this.xmin, this.ymin, this.xmax, this.ymax);
			env.Inflate(dx, dy);
			return env;
		}

		/// <summary>Sets the envelope from the array of points.</summary>
		/// <remarks>
		/// Sets the envelope from the array of points. The envelope will be set to
		/// empty if the array is null.
		/// </remarks>
		public void SetFromPoints(com.epl.geometry.Point2D[] points)
		{
			if (points == null || points.Length == 0)
			{
				SetEmpty();
				return;
			}
			com.epl.geometry.Point2D pt = points[0];
			SetCoords(pt.x, pt.y);
			for (int i = 1; i < points.Length; i++)
			{
				com.epl.geometry.Point2D pt2d = points[i];
				MergeNE(pt2d.x, pt2d.y);
			}
		}

		public void SetEmpty()
		{
			xmin = com.epl.geometry.NumberUtils.TheNaN;
			ymin = com.epl.geometry.NumberUtils.TheNaN;
			xmax = com.epl.geometry.NumberUtils.TheNaN;
			ymax = com.epl.geometry.NumberUtils.TheNaN;
		}

		public void SetInfinite()
		{
			xmin = com.epl.geometry.NumberUtils.NegativeInf();
			xmax = com.epl.geometry.NumberUtils.PositiveInf();
			ymin = com.epl.geometry.NumberUtils.NegativeInf();
			ymax = com.epl.geometry.NumberUtils.PositiveInf();
		}

		public bool IsEmpty()
		{
			return com.epl.geometry.NumberUtils.IsNaN(xmin) || com.epl.geometry.NumberUtils.IsNaN(ymin) || com.epl.geometry.NumberUtils.IsNaN(xmax) || com.epl.geometry.NumberUtils.IsNaN(ymax);
		}

		public void SetCoords(com.epl.geometry.Envelope1D xinterval, com.epl.geometry.Envelope1D yinterval)
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
		}

		public void Merge(double x, double y)
		{
			if (IsEmpty())
			{
				xmin = x;
				ymin = y;
				xmax = x;
				ymax = y;
			}
			else
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
			}
		}

		/// <summary>
		/// Merges a point with this envelope without checking if the envelope is
		/// empty.
		/// </summary>
		/// <remarks>
		/// Merges a point with this envelope without checking if the envelope is
		/// empty. Use with care.
		/// </remarks>
		public void MergeNE(double x, double y)
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
		}

		public void Merge(com.epl.geometry.Point2D pt)
		{
			Merge(pt.x, pt.y);
		}

		public void Merge(com.epl.geometry.Point3D pt)
		{
			Merge(pt.x, pt.y);
		}

		public void Merge(com.epl.geometry.Envelope2D other)
		{
			if (other.IsEmpty())
			{
				return;
			}
			Merge(other.xmin, other.ymin);
			Merge(other.xmax, other.ymax);
		}

		public void Inflate(double dx, double dy)
		{
			if (IsEmpty())
			{
				return;
			}
			xmin -= dx;
			xmax += dx;
			ymin -= dy;
			ymax += dy;
			if (xmin > xmax || ymin > ymax)
			{
				SetEmpty();
			}
		}

		public void Scale(double f)
		{
			if (f < 0.0)
			{
				SetEmpty();
			}
			if (IsEmpty())
			{
				return;
			}
			xmin *= f;
			xmax *= f;
			ymin *= f;
			ymax *= f;
		}

		public void Zoom(double factorX, double factorY)
		{
			if (!IsEmpty())
			{
				SetCoords(GetCenter(), factorX * GetWidth(), factorY * GetHeight());
			}
		}

		/// <summary>Checks if this envelope intersects the other.</summary>
		/// <returns>True if this envelope intersects the other.</returns>
		public bool IsIntersecting(com.epl.geometry.Envelope2D other)
		{
			// No need to check if empty, this will work for empty envelopes too
			// (IEEE math)
			return ((xmin <= other.xmin) ? xmax >= other.xmin : other.xmax >= xmin) && ((ymin <= other.ymin) ? ymax >= other.ymin : other.ymax >= ymin);
		}

		// check that x projections overlap
		// check
		// that
		// y
		// projections
		// overlap
		/// <summary>Checks if this envelope intersects the other assuming neither one is empty.</summary>
		/// <returns>
		/// True if this envelope intersects the other. Assumes this and
		/// other envelopes are not empty.
		/// </returns>
		public bool IsIntersectingNE(com.epl.geometry.Envelope2D other)
		{
			return ((xmin <= other.xmin) ? xmax >= other.xmin : other.xmax >= xmin) && ((ymin <= other.ymin) ? ymax >= other.ymin : other.ymax >= ymin);
		}

		// check that x projections overlap
		// check
		// that
		// y
		// projections
		// overlap
		/// <summary>Checks if this envelope intersects the other.</summary>
		/// <returns>True if this envelope intersects the other.</returns>
		public bool IsIntersecting(double xmin_, double ymin_, double xmax_, double ymax_)
		{
			// No need to check if empty, this will work for empty geoms too (IEEE
			// math)
			return ((xmin <= xmin_) ? xmax >= xmin_ : xmax_ >= xmin) && ((ymin <= ymin_) ? ymax >= ymin_ : ymax_ >= ymin);
		}

		// check
		// that x
		// projections
		// overlap
		// check that
		// y
		// projections
		// overlap
		/// <summary>
		/// Intersects this envelope with the other and stores result in this
		/// envelope.
		/// </summary>
		/// <returns>
		/// True if this envelope intersects the other, otherwise sets this
		/// envelope to empty state and returns False.
		/// </returns>
		public bool Intersect(com.epl.geometry.Envelope2D other)
		{
			if (IsEmpty() || other.IsEmpty())
			{
				return false;
			}
			if (other.xmin > xmin)
			{
				xmin = other.xmin;
			}
			if (other.xmax < xmax)
			{
				xmax = other.xmax;
			}
			if (other.ymin > ymin)
			{
				ymin = other.ymin;
			}
			if (other.ymax < ymax)
			{
				ymax = other.ymax;
			}
			bool bIntersecting = xmin <= xmax && ymin <= ymax;
			if (!bIntersecting)
			{
				SetEmpty();
			}
			return bIntersecting;
		}

		/// <summary>Queries a corner of the envelope.</summary>
		/// <param name="index">
		/// Indicates a corner of the envelope.
		/// <p>
		/// 0 means lower left or (xmin, ymin)
		/// <p>
		/// 1 means upper left or (xmin, ymax)
		/// <p>
		/// 2 means upper right or (xmax, ymax)
		/// <p>
		/// 3 means lower right or (xmax, ymin)
		/// </param>
		/// <returns>Point at a corner of the envelope.</returns>
		public com.epl.geometry.Point2D QueryCorner(int index)
		{
			switch (index)
			{
				case 0:
				{
					return com.epl.geometry.Point2D.Construct(xmin, ymin);
				}

				case 1:
				{
					return com.epl.geometry.Point2D.Construct(xmin, ymax);
				}

				case 2:
				{
					return com.epl.geometry.Point2D.Construct(xmax, ymax);
				}

				case 3:
				{
					return com.epl.geometry.Point2D.Construct(xmax, ymin);
				}

				default:
				{
					throw new System.IndexOutOfRangeException();
				}
			}
		}

		/// <summary>Queries corners into a given array.</summary>
		/// <remarks>
		/// Queries corners into a given array. The array length must be at least
		/// 4. Starts from the lower left corner and goes clockwise.
		/// </remarks>
		public void QueryCorners(com.epl.geometry.Point2D[] corners)
		{
			if ((corners == null) || (corners.Length < 4))
			{
				throw new System.ArgumentException();
			}
			if (corners[0] != null)
			{
				corners[0].SetCoords(xmin, ymin);
			}
			else
			{
				corners[0] = new com.epl.geometry.Point2D(xmin, ymin);
			}
			if (corners[1] != null)
			{
				corners[1].SetCoords(xmin, ymax);
			}
			else
			{
				corners[1] = new com.epl.geometry.Point2D(xmin, ymax);
			}
			if (corners[2] != null)
			{
				corners[2].SetCoords(xmax, ymax);
			}
			else
			{
				corners[2] = new com.epl.geometry.Point2D(xmax, ymax);
			}
			if (corners[3] != null)
			{
				corners[3].SetCoords(xmax, ymin);
			}
			else
			{
				corners[3] = new com.epl.geometry.Point2D(xmax, ymin);
			}
		}

		/// <summary>Queries corners into a given array in reversed order.</summary>
		/// <remarks>
		/// Queries corners into a given array in reversed order. The array length
		/// must be at least 4. Starts from the lower left corner and goes
		/// counterclockwise.
		/// </remarks>
		public void QueryCornersReversed(com.epl.geometry.Point2D[] corners)
		{
			if (corners == null || ((corners != null) && (corners.Length < 4)))
			{
				throw new System.ArgumentException();
			}
			if (corners[0] != null)
			{
				corners[0].SetCoords(xmin, ymin);
			}
			else
			{
				corners[0] = new com.epl.geometry.Point2D(xmin, ymin);
			}
			if (corners[1] != null)
			{
				corners[1].SetCoords(xmax, ymin);
			}
			else
			{
				corners[1] = new com.epl.geometry.Point2D(xmax, ymin);
			}
			if (corners[2] != null)
			{
				corners[2].SetCoords(xmax, ymax);
			}
			else
			{
				corners[2] = new com.epl.geometry.Point2D(xmax, ymax);
			}
			if (corners[3] != null)
			{
				corners[3].SetCoords(xmin, ymax);
			}
			else
			{
				corners[3] = new com.epl.geometry.Point2D(xmin, ymax);
			}
		}

		public double GetArea()
		{
			if (IsEmpty())
			{
				return 0;
			}
			return GetWidth() * GetHeight();
		}

		public double GetLength()
		{
			if (IsEmpty())
			{
				return 0;
			}
			return 2.0 * (GetWidth() + GetHeight());
		}

		public void SetFromPoints(com.epl.geometry.Point2D[] points, int count)
		{
			if (count == 0)
			{
				SetEmpty();
				return;
			}
			xmin = points[0].x;
			ymin = points[0].y;
			xmax = xmin;
			ymax = ymin;
			for (int i = 1; i < count; i++)
			{
				com.epl.geometry.Point2D pt = points[i];
				if (pt.x < xmin)
				{
					xmin = pt.x;
				}
				else
				{
					if (pt.x > xmax)
					{
						xmax = pt.x;
					}
				}
				if (pt.y < ymin)
				{
					ymin = pt.y;
				}
				else
				{
					if (pt.y > ymax)
					{
						ymax = pt.y;
					}
				}
			}
		}

		public void Reaspect(double arWidth, double arHeight)
		{
			if (IsEmpty())
			{
				return;
			}
			double newAspectRatio = arWidth / arHeight;
			double widthHalf = GetWidth() * 0.5;
			double heightHalf = GetHeight() * 0.5;
			double newWidthHalf = heightHalf * newAspectRatio;
			if (widthHalf <= newWidthHalf)
			{
				// preserve height, increase width
				double xc = GetCenterX();
				xmin = xc - newWidthHalf;
				xmax = xc + newWidthHalf;
			}
			else
			{
				// preserve the width, increase height
				double newHeightHalf = widthHalf / newAspectRatio;
				double yc = GetCenterY();
				ymin = yc - newHeightHalf;
				ymax = yc + newHeightHalf;
			}
			Normalize();
		}

		public double GetCenterX()
		{
			double cx = (xmax + xmin) / 2d;
			return cx;
		}

		public double GetCenterY()
		{
			double cy = (ymax + ymin) / 2d;
			return cy;
		}

		public double GetWidth()
		{
			return xmax - xmin;
		}

		public double GetHeight()
		{
			return ymax - ymin;
		}

		/// <summary>Moves the Envelope by given distance.</summary>
		public void Move(double dx, double dy)
		{
			if (IsEmpty())
			{
				return;
			}
			xmin += dx;
			ymin += dy;
			xmax += dx;
			ymax += dy;
		}

		public void CenterAt(double x, double y)
		{
			Move(x - GetCenterX(), y - GetCenterY());
		}

		internal void CenterAt(com.epl.geometry.Point2D pt)
		{
			CenterAt(pt.x, pt.y);
		}

		public void Offset(double dx, double dy)
		{
			xmin += dx;
			// NaN remains NaN
			xmax += dx;
			ymin += dy;
			ymax += dy;
		}

		public void Normalize()
		{
			if (IsEmpty())
			{
				return;
			}
			double min = System.Math.Min(xmin, xmax);
			double max = System.Math.Max(xmin, xmax);
			xmin = min;
			xmax = max;
			min = System.Math.Min(ymin, ymax);
			max = System.Math.Max(ymin, ymax);
			ymin = min;
			ymax = max;
		}

		public void QueryLowerLeft(com.epl.geometry.Point2D pt)
		{
			pt.SetCoords(xmin, ymin);
		}

		public void QueryLowerRight(com.epl.geometry.Point2D pt)
		{
			pt.SetCoords(xmax, ymin);
		}

		public void QueryUpperLeft(com.epl.geometry.Point2D pt)
		{
			pt.SetCoords(xmin, ymax);
		}

		public void QueryUpperRight(com.epl.geometry.Point2D pt)
		{
			pt.SetCoords(xmax, ymax);
		}

		/// <summary>
		/// Returns True if this envelope is valid (empty, or has xmin less or equal
		/// to xmax, or ymin less or equal to ymax).
		/// </summary>
		public bool IsValid()
		{
			return IsEmpty() || (xmin <= xmax && ymin <= ymax);
		}

		/// <summary>Gets the center point of the envelope.</summary>
		/// <remarks>
		/// Gets the center point of the envelope. The Center Point occurs at: ((XMin
		/// + XMax) / 2, (YMin + YMax) / 2).
		/// </remarks>
		/// <returns>the center point</returns>
		public com.epl.geometry.Point2D GetCenter()
		{
			return new com.epl.geometry.Point2D((xmax + xmin) / 2d, (ymax + ymin) / 2d);
		}

		public void QueryCenter(com.epl.geometry.Point2D center)
		{
			center.x = (xmax + xmin) / 2d;
			center.y = (ymax + ymin) / 2d;
		}

		public void CenterAt(com.epl.geometry.Point c)
		{
			double cx = (xmax - xmin) / 2d;
			double cy = (ymax - ymin) / 2d;
			xmin = c.GetX() - cx;
			xmax = c.GetX() + cx;
			ymin = c.GetY() - cy;
			ymax = c.GetY() + cy;
		}

		public com.epl.geometry.Point2D GetLowerLeft()
		{
			return new com.epl.geometry.Point2D(xmin, ymin);
		}

		public com.epl.geometry.Point2D GetUpperLeft()
		{
			return new com.epl.geometry.Point2D(xmin, ymax);
		}

		public com.epl.geometry.Point2D GetLowerRight()
		{
			return new com.epl.geometry.Point2D(xmax, ymin);
		}

		public com.epl.geometry.Point2D GetUpperRight()
		{
			return new com.epl.geometry.Point2D(xmax, ymax);
		}

		public bool Contains(com.epl.geometry.Point p)
		{
			return Contains(p.GetX(), p.GetY());
		}

		public bool Contains(com.epl.geometry.Point2D p)
		{
			return Contains(p.x, p.y);
		}

		public bool Contains(double x, double y)
		{
			// Note: This will return False, if envelope is empty, thus no need to
			// call is_empty().
			return x >= xmin && x <= xmax && y >= ymin && y <= ymax;
		}

		/// <summary>
		/// Returns True if the envelope contains the other envelope (boundary
		/// inclusive).
		/// </summary>
		public bool Contains(com.epl.geometry.Envelope2D other)
		{
			// Note: Will return False, if
			// either envelope is empty.
			return other.xmin >= xmin && other.xmax <= xmax && other.ymin >= ymin && other.ymax <= ymax;
		}

		/// <summary>Returns True if the envelope contains the point (boundary exclusive).</summary>
		public bool ContainsExclusive(double x, double y)
		{
			// Note: This will return False, if envelope is empty, thus no need to
			// call is_empty().
			return x > xmin && x < xmax && y > ymin && y < ymax;
		}

		/// <summary>Returns True if the envelope contains the point (boundary exclusive).</summary>
		public bool ContainsExclusive(com.epl.geometry.Point2D pt)
		{
			return ContainsExclusive(pt.x, pt.y);
		}

		/// <summary>
		/// Returns True if the envelope contains the other envelope (boundary
		/// exclusive).
		/// </summary>
		internal bool ContainsExclusive(com.epl.geometry.Envelope2D other)
		{
			// Note: This will return False, if either envelope is empty, thus no
			// need to call is_empty().
			return other.xmin > xmin && other.xmax < xmax && other.ymin > ymin && other.ymax < ymax;
		}

		public override bool Equals(object _other)
		{
			if (_other == this)
			{
				return true;
			}
			if (!(_other is com.epl.geometry.Envelope2D))
			{
				return false;
			}
			com.epl.geometry.Envelope2D other = (com.epl.geometry.Envelope2D)_other;
			if (IsEmpty() && other.IsEmpty())
			{
				return true;
			}
			if (xmin != other.xmin || ymin != other.ymin || xmax != other.xmax || ymax != other.ymax)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			long bits = System.BitConverter.DoubleToInt64Bits(xmin);
			int hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
			int hash = com.epl.geometry.NumberUtils.Hash(hc);
			bits = System.BitConverter.DoubleToInt64Bits(xmax);
			hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
			hash = com.epl.geometry.NumberUtils.Hash(hash, hc);
			bits = System.BitConverter.DoubleToInt64Bits(ymin);
			hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
			hash = com.epl.geometry.NumberUtils.Hash(hash, hc);
			bits = System.BitConverter.DoubleToInt64Bits(ymax);
			hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
			hash = com.epl.geometry.NumberUtils.Hash(hash, hc);
			return hash;
		}

		internal com.epl.geometry.Point2D _snapToBoundary(com.epl.geometry.Point2D pt)
		{
			com.epl.geometry.Point2D p = new com.epl.geometry.Point2D();
			p.SetCoords(pt);
			if (p._isNan())
			{
				return p;
			}
			if (IsEmpty())
			{
				p._setNan();
				return p;
			}
			if (p.x < xmin)
			{
				p.x = xmin;
			}
			else
			{
				if (p.x > xmax)
				{
					p.x = xmax;
				}
			}
			if (p.y < ymin)
			{
				p.y = ymin;
			}
			else
			{
				if (p.y > ymax)
				{
					p.y = ymax;
				}
			}
			if (!p.Equals(pt))
			{
				return p;
			}
			// p is inside envelope
			com.epl.geometry.Point2D center = GetCenter();
			double deltax = p.x < center.x ? p.x - xmin : xmax - p.x;
			double deltay = p.y < center.y ? p.y - ymin : ymax - p.y;
			if (deltax < deltay)
			{
				p.x = p.x < center.x ? xmin : xmax;
			}
			else
			{
				p.y = p.y < center.y ? ymin : ymax;
			}
			return p;
		}

		// Calculates distance of point from lower left corner of envelope,
		// moving clockwise along the envelope boundary.
		// The input point is assumed to lie exactly on envelope boundary
		// If this is not the case then a projection to the nearest position on the
		// envelope boundary is performed.
		// (If the user knows that the input point does most likely not lie on the
		// boundary,
		// it is more efficient to perform ProjectToBoundary before using this
		// function).
		internal double _boundaryDistance(com.epl.geometry.Point2D pt)
		{
			if (IsEmpty())
			{
				return com.epl.geometry.NumberUtils.NaN();
			}
			if (pt.x == xmin)
			{
				return pt.y - ymin;
			}
			double height = ymax - ymin;
			double width = xmax - xmin;
			if (pt.y == ymax)
			{
				return height + pt.x - xmin;
			}
			if (pt.x == xmax)
			{
				return height + width + ymax - pt.y;
			}
			if (pt.y == ymin)
			{
				return height * 2.0 + width + xmax - pt.x;
			}
			return _boundaryDistance(_snapToBoundary(pt));
		}

		// returns 0,..3 depending on which side pt lies.
		internal int _envelopeSide(com.epl.geometry.Point2D pt)
		{
			if (IsEmpty())
			{
				return -1;
			}
			double boundaryDist = _boundaryDistance(pt);
			double height = ymax - ymin;
			double width = xmax - xmin;
			if (boundaryDist < height)
			{
				return 0;
			}
			if ((boundaryDist -= height) < width)
			{
				return 1;
			}
			return boundaryDist - width < height ? 2 : 3;
		}

		internal double _calculateToleranceFromEnvelope()
		{
			if (IsEmpty())
			{
				return com.epl.geometry.NumberUtils.DoubleEps() * 100.0;
			}
			// GEOMTERYX_EPSFACTOR
			// 100.0;
			double r = System.Math.Abs(xmin) + System.Math.Abs(xmax) + System.Math.Abs(ymin) + System.Math.Abs(ymax) + 1;
			return r * com.epl.geometry.NumberUtils.DoubleEps() * 100.0;
		}

		// GEOMTERYX_EPSFACTOR
		// 100.0;
		public int ClipLine(com.epl.geometry.Point2D p1, com.epl.geometry.Point2D p2)
		{
			// Modified Cohen-Sutherland Line-Clipping Algorithm
			// returns:
			// 0 - the segment is outside of the clipping window
			// 1 - p1 was modified
			// 2 - p2 was modified
			// 3 - p1 and p2 were modified
			// 4 - the segment is complitely inside of the clipping window
			int c1 = _clipCode(p1);
			int c2 = _clipCode(p2);
			if ((c1 & c2) != 0)
			{
				// (c1 & c2)
				return 0;
			}
			if ((c1 | c2) == 0)
			{
				// (!(c1 | c2))
				return 4;
			}
			int res = ((c1 != 0) ? 1 : 0) | ((c2 != 0) ? 2 : 0);
			do
			{
				// (c1 ? 1 :
				// 0) | (c2
				// ? 2 : 0);
				double dx = p2.x - p1.x;
				double dy = p2.y - p1.y;
				bool bDX = dx > dy;
				if (bDX)
				{
					if ((c1 & XMASK) != 0)
					{
						// (c1 & XMASK)
						if ((c1 & XLESSXMIN) != 0)
						{
							// (c1 & XLESSXMIN)
							p1.y += dy * (xmin - p1.x) / dx;
							p1.x = xmin;
						}
						else
						{
							p1.y += dy * (xmax - p1.x) / dx;
							p1.x = xmax;
						}
						c1 = _clipCode(p1);
					}
					else
					{
						if ((c2 & XMASK) != 0)
						{
							// (c2 & XMASK)
							if ((c2 & XLESSXMIN) != 0)
							{
								p2.y += dy * (xmin - p2.x) / dx;
								p2.x = xmin;
							}
							else
							{
								p2.y += dy * (xmax - p2.x) / dx;
								p2.x = xmax;
							}
							c2 = _clipCode(p2);
						}
						else
						{
							if (c1 != 0)
							{
								// (c1)
								if ((c1 & YLESSYMIN) != 0)
								{
									// (c1 & YLESSYMIN)
									p1.x += dx * (ymin - p1.y) / dy;
									p1.y = ymin;
								}
								else
								{
									p1.x += dx * (ymax - p1.y) / dy;
									p1.y = ymax;
								}
								c1 = _clipCode(p1);
							}
							else
							{
								if ((c2 & YLESSYMIN) != 0)
								{
									// (c2 & YLESSYMIN)
									p2.x += dx * (ymin - p2.y) / dy;
									p2.y = ymin;
								}
								else
								{
									p2.x += dx * (ymax - p2.y) / dy;
									p2.y = ymax;
								}
								c2 = _clipCode(p2);
							}
						}
					}
				}
				else
				{
					if ((c1 & YMASK) != 0)
					{
						// (c1 & YMASK)
						if ((c1 & YLESSYMIN) != 0)
						{
							// (c1 & YLESSYMIN)
							p1.x += dx * (ymin - p1.y) / dy;
							p1.y = ymin;
						}
						else
						{
							p1.x += dx * (ymax - p1.y) / dy;
							p1.y = ymax;
						}
						c1 = _clipCode(p1);
					}
					else
					{
						if ((c2 & YMASK) != 0)
						{
							// (c2 & YMASK)
							if ((c2 & YLESSYMIN) != 0)
							{
								// (c2 & YLESSYMIN)
								p2.x += dx * (ymin - p2.y) / dy;
								p2.y = ymin;
							}
							else
							{
								p2.x += dx * (ymax - p2.y) / dy;
								p2.y = ymax;
							}
							c2 = _clipCode(p2);
						}
						else
						{
							if (c1 != 0)
							{
								// (c1)
								if ((c1 & XLESSXMIN) != 0)
								{
									// (c1 & XLESSXMIN)
									p1.y += dy * (xmin - p1.x) / dx;
									p1.x = xmin;
								}
								else
								{
									p1.y += dy * (xmax - p1.x) / dx;
									p1.x = xmax;
								}
								c1 = _clipCode(p1);
							}
							else
							{
								if ((c2 & XLESSXMIN) != 0)
								{
									// (c2 & XLESSXMIN)
									p2.y += dy * (xmin - p2.x) / dx;
									p2.x = xmin;
								}
								else
								{
									p2.y += dy * (xmax - p2.x) / dx;
									p2.x = xmax;
								}
								c2 = _clipCode(p2);
							}
						}
					}
				}
				/*
				* if (c1) //original code. Faster, but less robust numerically.
				* ( //The Cohen-Sutherland Line-Clipping Algorithm) { if (c1 &
				* XLESSXMIN) { p1.y += dy * (xmin - p1.x) / dx; p1.x = xmin; }
				* else if (c1 & XGREATERXMAX) { p1.y += dy * (xmax - p1.x) /
				* dx; p1.x = xmax; } else if (c1 & YLESSYMIN) { p1.x += dx *
				* (ymin - p1.y) / dy; p1.y = ymin; } else if (c1 &
				* YGREATERYMAX) { p1.x += dx * (ymax - p1.y) / dy; p1.y = ymax;
				* }
				*
				* c1 = _clipCode(p1, ClipRect); } else { if (c2 & XLESSXMIN) {
				* p2.y += dy * (xmin - p2.x) / dx; p2.x = xmin; } else if (c2 &
				* XGREATERXMAX) { p2.y += dy * (xmax - p2.x) / dx; p2.x = xmax;
				* } else if (c2 & YLESSYMIN) { p2.x += dx * (ymin - p2.y) / dy;
				* p2.y = ymin; } else if (c2 & YGREATERYMAX) { p2.x += dx *
				* (ymax - p2.y) / dy; p2.y = ymax; }
				*
				* c2 = _clipCode(p2, ClipRect); }
				*/
				if ((c1 & c2) != 0)
				{
					// (c1 & c2)
					return 0;
				}
			}
			while ((c1 | c2) != 0);
			// (c1 | c2);
			return res;
		}

		internal int _clipCode(com.epl.geometry.Point2D p)
		{
			// returns a code from the Cohen-Sutherland (0000 is
			// boundary inclusive)
			int left = (p.x < xmin) ? 1 : 0;
			int right = (p.x > xmax) ? 1 : 0;
			int bottom = (p.y < ymin) ? 1 : 0;
			int top = (p.y > ymax) ? 1 : 0;
			return left | right << 1 | bottom << 2 | top << 3;
		}

		// Clips and optionally extends line within envelope; modifies point 'from',
		// 'to'.
		// Algorithm: Liang-Barsky parametric line-clipping (Foley, vanDam, Feiner,
		// Hughes, second edition, 117-124)
		// lineExtension: 0 no line eExtension, 1 extend line at from point, 2
		// extend line at endpoint, 3 extend line at both ends
		// boundaryDistances can be NULLPTR.
		// returns:
		// 0 - the segment is outside of the clipping window
		// 1 - p1 was modified
		// 2 - p2 was modified
		// 3 - p1 and p2 were modified
		// 4 - the segment is complitely inside of the clipping window
		internal int ClipLine(com.epl.geometry.Point2D p0, com.epl.geometry.Point2D p1, int lineExtension, double[] segParams, double[] boundaryDistances)
		{
			if (boundaryDistances != null)
			{
				boundaryDistances[0] = -1.0;
				boundaryDistances[1] = -1.0;
			}
			double[] tOld = new double[2];
			// LOCALREFCLASS1(ArrayOf(double), int,
			// tOld, 2);
			int modified = 0;
			com.epl.geometry.Point2D delta = new com.epl.geometry.Point2D(p1.x - p0.x, p1.y - p0.y);
			if (delta.x == 0.0 && delta.y == 0.0)
			{
				// input line degenerates to a
				// point
				segParams[0] = 0.0;
				segParams[1] = 0.0;
				return Contains(p0) ? 4 : 0;
			}
			segParams[0] = ((lineExtension & 1) != 0) ? com.epl.geometry.NumberUtils.NegativeInf() : 0.0;
			segParams[1] = ((lineExtension & 2) != 0) ? com.epl.geometry.NumberUtils.PositiveInf() : 1.0;
			tOld[0] = segParams[0];
			tOld[1] = segParams[1];
			if (ClipLineAuxiliary(delta.x, xmin - p0.x, segParams) && ClipLineAuxiliary(-delta.x, p0.x - xmax, segParams) && ClipLineAuxiliary(delta.y, ymin - p0.y, segParams) && ClipLineAuxiliary(-delta.y, p0.y - ymax, segParams))
			{
				if (segParams[1] < tOld[1])
				{
					p1.ScaleAdd(segParams[1], delta, p0);
					_snapToBoundary(p1);
					// needed for accuracy
					modified |= 2;
					if (boundaryDistances != null)
					{
						boundaryDistances[1] = _boundaryDistance(p1);
					}
				}
				if (segParams[0] > tOld[0])
				{
					p0.ScaleAdd(segParams[0], delta, p0);
					_snapToBoundary(p0);
					// needed for accuracy
					modified |= 1;
					if (boundaryDistances != null)
					{
						boundaryDistances[0] = _boundaryDistance(p0);
					}
				}
			}
			return modified;
		}

		internal bool ClipLineAuxiliary(double denominator, double numerator, double[] segParams)
		{
			double t = numerator / denominator;
			if (denominator > 0.0)
			{
				if (t > segParams[1])
				{
					return false;
				}
				if (t > segParams[0])
				{
					segParams[0] = t;
					return true;
				}
			}
			else
			{
				if (denominator < 0.0)
				{
					if (t < segParams[0])
					{
						return false;
					}
					if (t < segParams[1])
					{
						segParams[1] = t;
						return true;
					}
				}
				else
				{
					return numerator <= 0.0;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns True, envelope is degenerate (Width or Height are less than
		/// tolerance).
		/// </summary>
		/// <remarks>
		/// Returns True, envelope is degenerate (Width or Height are less than
		/// tolerance). Note: this returns False for Empty envelope.
		/// </remarks>
		public bool IsDegenerate(double tolerance)
		{
			return !IsEmpty() && (GetWidth() <= tolerance || GetHeight() <= tolerance);
		}

		internal com.epl.geometry.Point2D _snapClip(com.epl.geometry.Point2D pt)
		{
			// clips the point if it is outside, then snaps
			// it to the boundary.
			double x = com.epl.geometry.NumberUtils.Snap(pt.x, xmin, xmax);
			double y = com.epl.geometry.NumberUtils.Snap(pt.y, ymin, ymax);
			return new com.epl.geometry.Point2D(x, y);
		}

		public bool IsPointOnBoundary(com.epl.geometry.Point2D pt, double tolerance)
		{
			return System.Math.Abs(pt.x - xmin) <= tolerance || System.Math.Abs(pt.x - xmax) <= tolerance || System.Math.Abs(pt.y - ymin) <= tolerance || System.Math.Abs(pt.y - ymax) <= tolerance;
		}

		/// <summary>Calculates minimum distance from this envelope to the other.</summary>
		/// <remarks>
		/// Calculates minimum distance from this envelope to the other.
		/// Returns 0 for empty envelopes.
		/// </remarks>
		public double Distance(com.epl.geometry.Envelope2D other)
		{
			/* const */
			return System.Math.Sqrt(SqrDistance(other));
		}

		/// <summary>Calculates minimum distance from this envelope to the point.</summary>
		/// <remarks>
		/// Calculates minimum distance from this envelope to the point.
		/// Returns 0 for empty envelopes.
		/// </remarks>
		public double Distance(com.epl.geometry.Point2D pt2D)
		{
			return System.Math.Sqrt(SqrDistance(pt2D));
		}

		/// <summary>Calculates minimum squared distance from this envelope to the other.</summary>
		/// <remarks>
		/// Calculates minimum squared distance from this envelope to the other.
		/// Returns 0 for empty envelopes.
		/// </remarks>
		public double SqrDistance(com.epl.geometry.Envelope2D other)
		{
			double dx = 0;
			double dy = 0;
			double nn;
			nn = xmin - other.xmax;
			if (nn > dx)
			{
				dx = nn;
			}
			nn = ymin - other.ymax;
			if (nn > dy)
			{
				dy = nn;
			}
			nn = other.xmin - xmax;
			if (nn > dx)
			{
				dx = nn;
			}
			nn = other.ymin - ymax;
			if (nn > dy)
			{
				dy = nn;
			}
			return dx * dx + dy * dy;
		}

		/// <summary>Calculates minimum squared distance from this envelope to the other.</summary>
		/// <remarks>
		/// Calculates minimum squared distance from this envelope to the other.
		/// Returns 0 for empty envelopes.
		/// </remarks>
		public double SqrDistance(double xmin_, double ymin_, double xmax_, double ymax_)
		{
			double dx = 0;
			double dy = 0;
			double nn;
			nn = xmin - xmax_;
			if (nn > dx)
			{
				dx = nn;
			}
			nn = ymin - ymax_;
			if (nn > dy)
			{
				dy = nn;
			}
			nn = xmin_ - xmax;
			if (nn > dx)
			{
				dx = nn;
			}
			nn = ymin_ - ymax;
			if (nn > dy)
			{
				dy = nn;
			}
			return dx * dx + dy * dy;
		}

		/// <summary>Returns squared max distance between two bounding boxes.</summary>
		/// <remarks>Returns squared max distance between two bounding boxes. This is furthest distance between points on the two envelopes.</remarks>
		/// <param name="other">The bounding box to calculate the max distance two.</param>
		/// <returns>Squared distance value.</returns>
		public double SqrMaxDistance(com.epl.geometry.Envelope2D other)
		{
			if (IsEmpty() || other.IsEmpty())
			{
				return com.epl.geometry.NumberUtils.TheNaN;
			}
			double dist = 0;
			com.epl.geometry.Point2D[] points = new com.epl.geometry.Point2D[4];
			QueryCorners(points);
			com.epl.geometry.Point2D[] points_o = new com.epl.geometry.Point2D[4];
			other.QueryCorners(points_o);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					double d = com.epl.geometry.Point2D.SqrDistance(points[i], points_o[j]);
					if (d > dist)
					{
						dist = d;
					}
				}
			}
			return dist;
		}

		/// <summary>Calculates minimum squared distance from this envelope to the point.</summary>
		/// <remarks>
		/// Calculates minimum squared distance from this envelope to the point.
		/// Returns 0 for empty envelopes.
		/// </remarks>
		public double SqrDistance(com.epl.geometry.Point2D pt2D)
		{
			double dx = 0;
			double dy = 0;
			double nn;
			nn = xmin - pt2D.x;
			if (nn > dx)
			{
				dx = nn;
			}
			nn = ymin - pt2D.y;
			if (nn > dy)
			{
				dy = nn;
			}
			nn = pt2D.x - xmax;
			if (nn > dx)
			{
				dx = nn;
			}
			nn = pt2D.y - ymax;
			if (nn > dy)
			{
				dy = nn;
			}
			return dx * dx + dy * dy;
		}

		public void QueryIntervalX(com.epl.geometry.Envelope1D env1D)
		{
			if (IsEmpty())
			{
				env1D.SetEmpty();
			}
			else
			{
				env1D.SetCoords(xmin, xmax);
			}
		}

		public void QueryIntervalY(com.epl.geometry.Envelope1D env1D)
		{
			if (IsEmpty())
			{
				env1D.SetEmpty();
			}
			else
			{
				env1D.SetCoords(ymin, ymax);
			}
		}

//		/// <exception cref="System.IO.IOException"/>
//		private void WriteObject(java.io.ObjectOutputStream @out)
//		{
//			@out.DefaultWriteObject();
//		}
//
//		/// <exception cref="System.IO.IOException"/>
//		/// <exception cref="System.TypeLoadException"/>
//		private void ReadObject(java.io.ObjectInputStream @in)
//		{
//			@in.DefaultReadObject();
//		}

		/// <exception cref="java.io.ObjectStreamException"/>
		private void ReadObjectNoData()
		{
			SetEmpty();
		}
	}
}
