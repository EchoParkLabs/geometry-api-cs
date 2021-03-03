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
	/// <summary>A Multipoint is a collection of points.</summary>
	/// <remarks>
	/// A Multipoint is a collection of points. A multipoint is a one-dimensional
	/// geometry object. Multipoints can be used to store a collection of point-based
	/// information where the order and individual identity of each point is not an
	/// essential characteristic of the point set.
	/// </remarks>
	[System.Serializable]
	public class MultiPoint : com.epl.geometry.MultiVertexGeometry
	{
		private const long serialVersionUID = 2L;

		private com.epl.geometry.MultiPointImpl m_impl;

		/// <summary>Creates a new empty multipoint.</summary>
		public MultiPoint()
		{
			m_impl = new com.epl.geometry.MultiPointImpl();
		}

		public MultiPoint(com.epl.geometry.VertexDescription description)
		{
			m_impl = new com.epl.geometry.MultiPointImpl(description);
		}

		internal override double GetAttributeAsDbl(int semantics, int index, int ordinate)
		{
			return m_impl.GetAttributeAsDbl(semantics, index, ordinate);
		}

		internal override int GetAttributeAsInt(int semantics, int index, int ordinate)
		{
			return m_impl.GetAttributeAsInt(semantics, index, ordinate);
		}

		public override com.epl.geometry.Point GetPoint(int index)
		{
			return m_impl.GetPoint(index);
		}

		public override int GetPointCount()
		{
			return m_impl.GetPointCount();
		}

		public override com.epl.geometry.Point2D GetXY(int index)
		{
			return m_impl.GetXY(index);
		}

		public override void GetXY(int index, com.epl.geometry.Point2D pt)
		{
			m_impl.GetXY(index, pt);
		}

		internal override com.epl.geometry.Point3D GetXYZ(int index)
		{
			return m_impl.GetXYZ(index);
		}

		public override void QueryCoordinates(com.epl.geometry.Point2D[] dst)
		{
			m_impl.QueryCoordinates(dst);
		}

		public override void QueryCoordinates(com.epl.geometry.Point[] dst)
		{
			m_impl.QueryCoordinates(dst);
		}

		protected internal override object _getImpl()
		{
			return m_impl;
		}

		/// <summary>Adds a point multipoint.</summary>
		/// <param name="point">The Point to be added to this multipoint.</param>
		public virtual void Add(com.epl.geometry.Point point)
		{
			m_impl.Add(point);
		}

		/// <summary>Adds a point with the specified X, Y coordinates to this multipoint.</summary>
		/// <param name="x">The new Point's X coordinate.</param>
		/// <param name="y">The new Point's Y coordinate.</param>
		public virtual void Add(double x, double y)
		{
			m_impl.Add(x, y);
		}

		/// <summary>Adds a point with the specified X, Y coordinates to this multipoint.</summary>
		/// <param name="pt">the point to add</param>
		public virtual void Add(com.epl.geometry.Point2D pt)
		{
			m_impl.Add(pt.x, pt.y);
		}

		/// <summary>Adds a 3DPoint with the specified X, Y, Z coordinates to this multipoint.</summary>
		/// <param name="x">The new Point's X coordinate.</param>
		/// <param name="y">The new Point's Y coordinate.</param>
		/// <param name="z">The new Point's Z coordinate.</param>
		internal virtual void Add(double x, double y, double z)
		{
			m_impl.Add(x, y, z);
		}

		/// <summary>Appends points from another multipoint at the end of this multipoint.</summary>
		/// <param name="src">The mulitpoint to append to this multipoint.</param>
		/// <param name="srcFrom">
		/// The start index in the source multipoint from which to start
		/// appending points.
		/// </param>
		/// <param name="srcTo">
		/// The end index in the source multipoint right after the last
		/// point to be appended. Use -1 to indicate the rest of the
		/// source multipoint.
		/// </param>
		public virtual void Add(com.epl.geometry.MultiVertexGeometry src, int srcFrom, int srcTo)
		{
			m_impl.Add((com.epl.geometry.MultiVertexGeometryImpl)src._getImpl(), srcFrom, srcTo);
		}

		internal virtual void AddPoints(com.epl.geometry.Point2D[] points)
		{
			m_impl.AddPoints(points);
		}

		internal virtual void AddPoints(com.epl.geometry.Point[] points)
		{
			m_impl.AddPoints(points);
		}

		/// <summary>Inserts a point to this multipoint.</summary>
		/// <param name="beforePointIndex">The index right before the new point to insert.</param>
		/// <param name="pt">The point to insert.</param>
		public virtual void InsertPoint(int beforePointIndex, com.epl.geometry.Point pt)
		{
			m_impl.InsertPoint(beforePointIndex, pt);
		}

		// inserts a point. The point is connected with Lines
		/// <summary>Removes a point from this multipoint.</summary>
		/// <param name="pointIndex">The index of the point to be removed.</param>
		public virtual void RemovePoint(int pointIndex)
		{
			m_impl.RemovePoint(pointIndex);
		}

		/// <summary>Resizes the multipoint to have the given size.</summary>
		/// <param name="pointCount">- The number of points in this multipoint.</param>
		public virtual void Resize(int pointCount)
		{
			m_impl.Resize(pointCount);
		}

		internal override void QueryCoordinates(com.epl.geometry.Point3D[] dst)
		{
			m_impl.QueryCoordinates(dst);
		}

		internal override void SetAttribute(int semantics, int index, int ordinate, double value)
		{
			m_impl.SetAttribute(semantics, index, ordinate, value);
		}

		internal override void SetAttribute(int semantics, int index, int ordinate, int value)
		{
			m_impl.SetAttribute(semantics, index, ordinate, value);
		}

		public override void SetPoint(int index, com.epl.geometry.Point pointSrc)
		{
			m_impl.SetPoint(index, pointSrc);
		}

		public override void SetXY(int index, com.epl.geometry.Point2D pt)
		{
			m_impl.SetXY(index, pt);
		}

		internal override void SetXYZ(int index, com.epl.geometry.Point3D pt)
		{
			m_impl.SetXYZ(index, pt);
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			m_impl.ApplyTransformation(transform);
		}

		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			m_impl.ApplyTransformation(transform);
		}

		public override void CopyTo(com.epl.geometry.Geometry dst)
		{
			m_impl.CopyTo((com.epl.geometry.Geometry)dst._getImpl());
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.MultiPoint(GetDescription());
		}

		public override int GetDimension()
		{
			return 0;
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.MultiPoint;
		}

		public override com.epl.geometry.VertexDescription GetDescription()
		{
			return m_impl.GetDescription();
		}

		public override void AddAttribute(int semantics)
		{
			m_impl.AddAttribute(semantics);
		}

		public override void AssignVertexDescription(com.epl.geometry.VertexDescription src)
		{
			m_impl.AssignVertexDescription(src);
		}

		public override void DropAllAttributes()
		{
			m_impl.DropAllAttributes();
		}

		public override void DropAttribute(int semantics)
		{
			m_impl.DropAttribute(semantics);
		}

		public override void MergeVertexDescription(com.epl.geometry.VertexDescription src)
		{
			m_impl.MergeVertexDescription(src);
		}

		public override bool IsEmpty()
		{
			return m_impl.IsEmpty();
		}

		public override void QueryEnvelope(com.epl.geometry.Envelope env)
		{
			m_impl.QueryEnvelope(env);
		}

		public override void QueryEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			m_impl.QueryEnvelope2D(env);
		}

		internal override void QueryEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			m_impl.QueryEnvelope3D(env);
		}

		public override com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate)
		{
			return m_impl.QueryInterval(semantics, ordinate);
		}

		public override void SetEmpty()
		{
			m_impl.SetEmpty();
		}

		/// <summary>
		/// Returns TRUE when this geometry has exactly same type, properties, and
		/// coordinates as the other geometry.
		/// </summary>
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
			return m_impl.Equals(((com.epl.geometry.MultiPoint)other)._getImpl());
		}

		/// <summary>Returns a hash code value for this multipoint.</summary>
		public override int GetHashCode()
		{
			return m_impl.GetHashCode();
		}

		internal virtual int QueryCoordinates(com.epl.geometry.Point2D[] dst, int dstSize, int beginIndex, int endIndex)
		{
			return m_impl.QueryCoordinates(dst, dstSize, beginIndex, endIndex);
		}

		public override void GetPointByVal(int index, com.epl.geometry.Point outPoint)
		{
			m_impl.GetPointByVal(index, outPoint);
		}

		public override void SetPointByVal(int index, com.epl.geometry.Point pointSrc)
		{
			m_impl.SetPointByVal(index, pointSrc);
		}

		public override int GetStateFlag()
		{
			return m_impl.GetStateFlag();
		}

		public override com.epl.geometry.Geometry GetBoundary()
		{
			return m_impl.GetBoundary();
		}

		public override void ReplaceNaNs(int semantics, double value)
		{
			m_impl.ReplaceNaNs(semantics, value);
		}
	}
}
