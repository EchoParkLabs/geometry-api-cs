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
	/// <summary>The MultiPoint is a collection of points.</summary>
	[System.Serializable]
	internal sealed class MultiPointImpl : com.epl.geometry.MultiVertexGeometryImpl
	{
		public MultiPointImpl()
			: base()
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			m_pointCount = 0;
		}

		public MultiPointImpl(com.epl.geometry.VertexDescription description)
			: base()
		{
			if (description == null)
			{
				throw new System.ArgumentException();
			}
			m_description = description;
			m_pointCount = 0;
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.MultiPoint(m_description);
		}

		/// <summary>Adds a Point to this MultiPoint.</summary>
		public void Add(com.epl.geometry.Point point)
		{
			Resize(m_pointCount + 1);
			SetPoint(m_pointCount - 1, point);
		}

		/// <summary>Adds a Point to this MultiPoint with given x, y coordinates.</summary>
		public void Add(double x, double y)
		{
			Resize(m_pointCount + 1);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.SetCoords(x, y);
			SetXY(m_pointCount - 1, pt);
		}

		/// <summary>Adds a Point to this MultiPoint with given x, y, z coordinates.</summary>
		public void Add(double x, double y, double z)
		{
			Resize(m_pointCount + 1);
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.SetCoords(x, y, z);
			SetXYZ(m_pointCount - 1, pt);
		}

		/// <summary>
		/// Appends points from another MultiVertexGeometryImpl at the end of this
		/// one.
		/// </summary>
		/// <param name="src">The source MultiVertexGeometryImpl</param>
		public void Add(com.epl.geometry.MultiVertexGeometryImpl src, int beginIndex, int endIndex)
		{
			int endIndexC = endIndex < 0 ? src.GetPointCount() : endIndex;
			if (beginIndex < 0 || beginIndex > src.GetPointCount() || endIndexC < beginIndex)
			{
				throw new System.ArgumentException();
			}
			if (beginIndex == endIndexC)
			{
				return;
			}
			MergeVertexDescription(src.GetDescription());
			int count = endIndexC - beginIndex;
			int oldPointCount = m_pointCount;
			Resize(m_pointCount + count);
			_verifyAllStreams();
			for (int iattrib = 0, nattrib = src.GetDescription().GetAttributeCount(); iattrib < nattrib; iattrib++)
			{
				int semantics = src.GetDescription()._getSemanticsImpl(iattrib);
				int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				com.epl.geometry.AttributeStreamBase stream = GetAttributeStreamRef(semantics);
				com.epl.geometry.AttributeStreamBase srcStream = src.GetAttributeStreamRef(semantics);
				stream.InsertRange(oldPointCount * ncomps, srcStream, beginIndex * ncomps, count * ncomps, true, 1, oldPointCount * ncomps);
			}
		}

		public void AddPoints(com.epl.geometry.Point2D[] points)
		{
			int count = points.Length;
			int oldPointCount = m_pointCount;
			Resize(m_pointCount + count);
			for (int i = 0; i < count; i++)
			{
				SetXY(oldPointCount + i, points[i]);
			}
		}

		public void InsertPoint(int beforePointIndex, com.epl.geometry.Point pt)
		{
			if (beforePointIndex > GetPointCount())
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			if (beforePointIndex < 0)
			{
				beforePointIndex = GetPointCount();
			}
			MergeVertexDescription(pt.GetDescription());
			int oldPointCount = m_pointCount;
			_resizeImpl(m_pointCount + 1);
			_verifyAllStreams();
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				com.epl.geometry.AttributeStreamBase stream = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(semantics, 1);
				if (pt.HasAttribute(semantics))
				{
					m_vertexAttributes[iattr].InsertAttributes(comp * beforePointIndex, pt, semantics, comp * oldPointCount);
				}
				else
				{
					// Need to make room for the attribute, so we copy a default
					// value in
					double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					m_vertexAttributes[iattr].InsertRange(comp * beforePointIndex, v, comp, comp * oldPointCount);
				}
			}
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		internal void RemovePoint(int pointIndex)
		{
			if (pointIndex < 0 || pointIndex >= GetPointCount())
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			_verifyAllStreams();
			// Remove the attribute value for the path
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				if (m_vertexAttributes[iattr] != null)
				{
					int semantics = m_description._getSemanticsImpl(iattr);
					int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					m_vertexAttributes[iattr].EraseRange(comp * pointIndex, comp, comp * m_pointCount);
				}
			}
			m_pointCount--;
			m_reservedPointCount--;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		/// <summary>Resizes the MultiPoint to have the given size.</summary>
		public void Resize(int pointCount)
		{
			_resizeImpl(pointCount);
		}

		internal override void _copyToImpl(com.epl.geometry.MultiVertexGeometryImpl mvg)
		{
		}

		public override void SetEmpty()
		{
			base._setEmptyImpl();
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			if (IsEmpty())
			{
				return;
			}
			_verifyAllStreams();
			com.epl.geometry.AttributeStreamOfDbl points = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			for (int ipoint = 0; ipoint < m_pointCount; ipoint++)
			{
				pt2.x = points.Read(ipoint * 2);
				pt2.y = points.Read(ipoint * 2 + 1);
				transform.Transform(pt2, pt2);
				points.Write(ipoint * 2, pt2.x);
				points.Write(ipoint * 2 + 1, pt2.y);
			}
			// REFACTOR: reset the exact envelope only and transform the loose
			// envelope
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			if (IsEmpty())
			{
				return;
			}
			_verifyAllStreams();
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			_verifyAllStreams();
			com.epl.geometry.AttributeStreamOfDbl points = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.AttributeStreamOfDbl zs = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[1];
			com.epl.geometry.Point3D pt3 = new com.epl.geometry.Point3D();
			for (int ipoint = 0; ipoint < m_pointCount; ipoint++)
			{
				pt3.x = points.Read(ipoint * 2);
				pt3.y = points.Read(ipoint * 2 + 1);
				pt3.z = zs.Read(ipoint);
				com.epl.geometry.Point3D res = transform.Transform(pt3);
				points.Write(ipoint * 2, res.x);
				points.Write(ipoint * 2 + 1, res.y);
				zs.Write(ipoint, res.z);
			}
			// REFACTOR: reset the exact envelope only and transform the loose
			// envelope
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		public override int GetDimension()
		{
			return 0;
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.MultiPoint;
		}

		public override double CalculateArea2D()
		{
			return 0;
		}

		public override double CalculateLength2D()
		{
			return 0;
		}

		protected internal override object _getImpl()
		{
			return this;
		}

		public override bool Equals(object other)
		{
			if (other == this)
			{
				return true;
			}
			if (!(other is com.epl.geometry.MultiPointImpl))
			{
				return false;
			}
			return base.Equals(other);
		}

		public void AddPoints(com.epl.geometry.Point[] points)
		{
			int count = points.Length;
			// int oldPointCount = m_pointCount;
			Resize(m_pointCount + count);
			for (int i = 0; i < count; i++)
			{
				SetPoint(i, points[i]);
			}
		}

		internal override int QueryCoordinates(com.epl.geometry.Point2D[] dst, int dstSize, int beginIndex, int endIndex)
		{
			int endIndexC = endIndex < 0 ? m_pointCount : endIndex;
			endIndexC = System.Math.Min(endIndexC, beginIndex + dstSize);
			if (beginIndex < 0 || beginIndex >= m_pointCount || endIndexC < beginIndex || dst.Length != dstSize)
			{
				throw new System.ArgumentException();
			}
			// GEOMTHROW(invalid_argument);
			com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			int pointCountToRead = endIndexC - beginIndex;
			double[] dstArray = new double[pointCountToRead * 2];
			xy.ReadRange(2 * beginIndex, pointCountToRead * 2, dstArray, 0, true);
			for (int i = 0; i < pointCountToRead; i++)
			{
				dst[i] = new com.epl.geometry.Point2D(dstArray[i * 2], dstArray[i * 2 + 1]);
			}
			return endIndexC;
		}

		protected internal override void _notifyModifiedAllImpl()
		{
		}

		// TODO Auto-generated method stub
		protected internal override void _verifyStreamsImpl()
		{
		}

		// TODO Auto-generated method stub
		public override bool _buildRasterizedGeometryAccelerator(double toleranceXY, com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			// TODO Auto-generated method stub
			return false;
		}

		public override bool _buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			// TODO Auto-generated method stub
			return false;
		}

		// @Override
		// void _notifyModifiedAllImpl() {
		// // TODO Auto-generated method stub
		//
		// }
		// @Override
		// protected void _verifyStreamsImpl() {
		// // TODO Auto-generated method stub
		//
		// }
		public override com.epl.geometry.Geometry GetBoundary()
		{
			return null;
		}
	}
}
