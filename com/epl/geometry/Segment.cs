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
	/// <summary>A base class for segments.</summary>
	/// <remarks>A base class for segments. Presently only Line segments are supported.</remarks>
	[System.Serializable]
	public abstract class Segment : com.epl.geometry.Geometry
	{
		internal double m_xStart;

		internal double m_yStart;

		internal double m_xEnd;

		internal double m_yEnd;

		internal double[] m_attributes;

		// Header Definitions
		/// <summary>Returns XY coordinates of the start point.</summary>
		public virtual com.epl.geometry.Point2D GetStartXY()
		{
			return com.epl.geometry.Point2D.Construct(m_xStart, m_yStart);
		}

		public virtual void GetStartXY(com.epl.geometry.Point2D pt)
		{
			pt.x = m_xStart;
			pt.y = m_yStart;
		}

		/// <summary>Sets the XY coordinates of the start point.</summary>
		public virtual void SetStartXY(com.epl.geometry.Point2D pt)
		{
			_setXY(0, pt);
		}

		public virtual void SetStartXY(double x, double y)
		{
			_setXY(0, com.epl.geometry.Point2D.Construct(x, y));
		}

		/// <summary>Returns XYZ coordinates of the start point.</summary>
		/// <remarks>Returns XYZ coordinates of the start point. Z if 0 if Z is missing.</remarks>
		public virtual com.epl.geometry.Point3D GetStartXYZ()
		{
			return _getXYZ(0);
		}

		/// <summary>Sets the XYZ coordinates of the start point.</summary>
		public virtual void SetStartXYZ(com.epl.geometry.Point3D pt)
		{
			_setXYZ(0, pt);
		}

		public virtual void SetStartXYZ(double x, double y, double z)
		{
			_setXYZ(0, com.epl.geometry.Point3D.Construct(x, y, z));
		}

		/// <summary>Returns coordinates of the start point in a Point class.</summary>
		public virtual void QueryStart(com.epl.geometry.Point dstPoint)
		{
			_get(0, dstPoint);
		}

		/// <summary>Sets the coordinates of the start point in this segment.</summary>
		/// <param name="srcPoint">The new start point of this segment.</param>
		public virtual void SetStart(com.epl.geometry.Point srcPoint)
		{
			_set(0, srcPoint);
		}

		/// <summary>Returns value of the start vertex attribute's ordinate.</summary>
		/// <remarks>
		/// Returns value of the start vertex attribute's ordinate. Throws if the
		/// Point is empty.
		/// </remarks>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">
		/// The attribute's ordinate. For example, the y coordinate of the
		/// NORMAL has ordinate of 1.
		/// </param>
		/// <returns>Ordinate value as double.</returns>
		public virtual double GetStartAttributeAsDbl(int semantics, int ordinate)
		{
			return _getAttributeAsDbl(0, semantics, ordinate);
		}

		/// <summary>Returns the value of the start vertex attribute's ordinate.</summary>
		/// <remarks>
		/// Returns the value of the start vertex attribute's ordinate. The ordinate
		/// is always 0 because integer attributes always have one component.
		/// </remarks>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">
		/// The attribute's ordinate. For example, the y coordinate of the
		/// NORMAL has ordinate of 1.
		/// </param>
		/// <returns>Ordinate value truncated to 32 bit integer.</returns>
		public virtual int GetStartAttributeAsInt(int semantics, int ordinate)
		{
			return _getAttributeAsInt(0, semantics, ordinate);
		}

		/// <summary>Sets the value of the start vertex attribute.</summary>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="value">
		/// is the array to write values to. The attribute type and the
		/// number of elements must match the persistence type, as well as
		/// the number of components of the attribute.
		/// </param>
		public virtual void SetStartAttribute(int semantics, int ordinate, double value)
		{
			_setAttribute(0, semantics, ordinate, value);
		}

		public virtual void SetStartAttribute(int semantics, int ordinate, int value)
		{
			_setAttribute(0, semantics, ordinate, value);
		}

		/// <summary>Returns the X coordinate of starting point.</summary>
		/// <returns>The X coordinate of starting point.</returns>
		public virtual double GetStartX()
		{
			return m_xStart;
		}

		/// <summary>Returns the Y coordinate of starting point.</summary>
		/// <returns>The Y coordinate of starting point.</returns>
		public virtual double GetStartY()
		{
			return m_yStart;
		}

		/// <summary>Returns the X coordinate of ending point.</summary>
		/// <returns>The X coordinate of ending point.</returns>
		public virtual double GetEndX()
		{
			return m_xEnd;
		}

		/// <summary>Returns the Y coordinate of ending point.</summary>
		/// <returns>The Y coordinate of ending point.</returns>
		public virtual double GetEndY()
		{
			return m_yEnd;
		}

		/// <summary>Returns XY coordinates of the end point.</summary>
		/// <returns>The XY coordinates of the end point.</returns>
		public virtual com.epl.geometry.Point2D GetEndXY()
		{
			return com.epl.geometry.Point2D.Construct(m_xEnd, m_yEnd);
		}

		public virtual void GetEndXY(com.epl.geometry.Point2D pt)
		{
			pt.x = m_xEnd;
			pt.y = m_yEnd;
		}

		/// <summary>Sets the XY coordinates of the end point.</summary>
		/// <param name="pt">The end point of the segment.</param>
		public virtual void SetEndXY(com.epl.geometry.Point2D pt)
		{
			_setXY(1, pt);
		}

		public virtual void SetEndXY(double x, double y)
		{
			_setXY(1, com.epl.geometry.Point2D.Construct(x, y));
		}

		/// <summary>Returns XYZ coordinates of the end point.</summary>
		/// <remarks>Returns XYZ coordinates of the end point. Z if 0 if Z is missing.</remarks>
		/// <returns>The XYZ coordinates of the end point.</returns>
		public virtual com.epl.geometry.Point3D GetEndXYZ()
		{
			return _getXYZ(1);
		}

		/// <summary>Sets the XYZ coordinates of the end point.</summary>
		public virtual void SetEndXYZ(com.epl.geometry.Point3D pt)
		{
			_setXYZ(1, pt);
		}

		public virtual void SetEndXYZ(double x, double y, double z)
		{
			_setXYZ(1, com.epl.geometry.Point3D.Construct(x, y, z));
		}

		/// <summary>Returns coordinates of the end point in this segment.</summary>
		/// <param name="dstPoint">The end point of this segment.</param>
		public virtual void QueryEnd(com.epl.geometry.Point dstPoint)
		{
			_get(1, dstPoint);
		}

		/// <summary>Sets the coordinates of the end point in a Point class.</summary>
		/// <param name="srcPoint">The new end point of this segment.</param>
		public virtual void SetEnd(com.epl.geometry.Point srcPoint)
		{
			_set(1, srcPoint);
		}

		/// <summary>Returns value of the end vertex attribute's ordinate.</summary>
		/// <remarks>
		/// Returns value of the end vertex attribute's ordinate. Throws if the Point
		/// is empty.
		/// </remarks>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">
		/// The attribute's ordinate. For example, the y coordinate of the
		/// NORMAL has ordinate of 1.
		/// </param>
		/// <returns>Ordinate value as double.</returns>
		public virtual double GetEndAttributeAsDbl(int semantics, int ordinate)
		{
			return _getAttributeAsDbl(1, semantics, ordinate);
		}

		/// <summary>Returns the value of the end vertex attribute's ordinate.</summary>
		/// <remarks>
		/// Returns the value of the end vertex attribute's ordinate. The ordinate is
		/// always 0 because integer attributes always have one component.
		/// </remarks>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">
		/// The attribute's ordinate. For example, the y coordinate of the
		/// NORMAL has ordinate of 1.
		/// </param>
		/// <returns>The ordinate value truncated to 32 bit integer.</returns>
		public virtual int GetEndAttributeAsInt(int semantics, int ordinate)
		{
			return _getAttributeAsInt(1, semantics, ordinate);
		}

		/// <summary>Sets the value of end vertex attribute.</summary>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">The attribute's ordinate.</param>
		/// <param name="value">
		/// Is the array to write values to. The attribute type and the
		/// number of elements must match the persistence type, as well as
		/// the number of components of the attribute.
		/// </param>
		public virtual void SetEndAttribute(int semantics, int ordinate, double value)
		{
			_setAttribute(1, semantics, ordinate, value);
		}

		public virtual void SetEndAttribute(int semantics, int ordinate, int value)
		{
			_setAttribute(1, semantics, ordinate, value);
		}

		public sealed override int GetDimension()
		{
			return 1;
		}

		public sealed override bool IsEmpty()
		{
			return IsEmptyImpl();
		}

		public sealed override void SetEmpty()
		{
		}

		public override double CalculateArea2D()
		{
			return 0;
		}

		/// <summary>Calculates intersections of this segment with another segment.</summary>
		/// <remarks>
		/// Calculates intersections of this segment with another segment.
		/// <p>
		/// Note: This is not a topological operation. It needs to be paired with the
		/// Segment.Overlap call.
		/// </remarks>
		/// <param name="other">The segment to calculate intersection with.</param>
		/// <param name="intersectionPoints">The intersection points. Can be NULL.</param>
		/// <param name="paramThis">
		/// The value of the parameter in the intersection points for this
		/// Segment (between 0 and 1). Can be NULL.
		/// </param>
		/// <param name="paramOther">
		/// The value of the parameter in the intersection points for the
		/// other Segment (between 0 and 1). Can be NULL.
		/// </param>
		/// <param name="tolerance">
		/// The tolerance value for the intersection calculation. Can be
		/// 0.
		/// </param>
		/// <returns>
		/// The number of intersection points, 0 when no intersection points
		/// exist.
		/// </returns>
		internal virtual int Intersect(com.epl.geometry.Segment other, com.epl.geometry.Point2D[] intersectionPoints, double[] paramThis, double[] paramOther, double tolerance)
		{
			return _intersect(other, intersectionPoints, paramThis, paramOther, tolerance);
		}

		/// <summary>
		/// Returns TRUE if this segment intersects with the other segment with the
		/// given tolerance.
		/// </summary>
		public virtual bool IsIntersecting(com.epl.geometry.Segment other, double tolerance)
		{
			return _isIntersecting(other, tolerance, false) != 0;
		}

		/// <summary>
		/// Returns TRUE if the point and segment intersect (not disjoint) for the
		/// given tolerance.
		/// </summary>
		public virtual bool IsIntersecting(com.epl.geometry.Point2D pt, double tolerance)
		{
			return _isIntersectingPoint(pt, tolerance, false);
		}

		/// <summary>Non public abstract version of the function.</summary>
		public virtual bool IsEmptyImpl()
		{
			return false;
		}

		/// <summary>Creates a segment with start and end points (0,0).</summary>
		public Segment()
		{
			// Header Definitions
			// Cpp definitions
			m_xStart = 0;
			m_yStart = 0;
			m_xEnd = 0;
			m_yEnd = 0;
			m_attributes = null;
		}

		internal virtual void _resizeAttributes(int newSize)
		{
			_touch();
			if (m_attributes == null && newSize > 0)
			{
				m_attributes = new double[newSize * 2];
			}
			else
			{
				if (m_attributes != null && m_attributes.Length < newSize * 2)
				{
					double[] newbuffer = new double[newSize * 2];
					System.Array.Copy(m_attributes, 0, newbuffer, 0, m_attributes.Length);
					m_attributes = newbuffer;
				}
			}
		}

		internal static void _attributeCopy(double[] src, int srcStart, double[] dst, int dstStart, int count)
		{
			if (count > 0)
			{
				System.Array.Copy(src, srcStart, dst, dstStart, count);
			}
		}

		private com.epl.geometry.Point2D _getXY(int endPoint)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			if (endPoint != 0)
			{
				pt.SetCoords(m_xEnd, m_yEnd);
			}
			else
			{
				pt.SetCoords(m_xStart, m_yStart);
			}
			return pt;
		}

		private void _setXY(int endPoint, com.epl.geometry.Point2D pt)
		{
			if (endPoint != 0)
			{
				m_xEnd = pt.x;
				m_yEnd = pt.y;
			}
			else
			{
				m_xStart = pt.x;
				m_yStart = pt.y;
			}
		}

		private com.epl.geometry.Point3D _getXYZ(int endPoint)
		{
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			if (endPoint != 0)
			{
				pt.x = m_xEnd;
				pt.y = m_yEnd;
			}
			else
			{
				pt.x = m_xStart;
				pt.y = m_yStart;
			}
			if (m_description.HasZ())
			{
				pt.z = m_attributes[_getEndPointOffset(m_description, endPoint)];
			}
			else
			{
				pt.z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			return pt;
		}

		private void _setXYZ(int endPoint, com.epl.geometry.Point3D pt)
		{
			_touch();
			bool bHasZ = HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			if (!bHasZ && !com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, pt.z))
			{
				// add
				// Z
				// only
				// if
				// pt.z
				// is
				// not
				// a
				// default
				// value.
				AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				bHasZ = true;
			}
			if (endPoint != 0)
			{
				m_xEnd = pt.x;
				m_yEnd = pt.y;
			}
			else
			{
				m_xStart = pt.x;
				m_yStart = pt.y;
			}
			if (bHasZ)
			{
				m_attributes[_getEndPointOffset(m_description, endPoint)] = pt.z;
			}
		}

		protected internal override void _assignVertexDescriptionImpl(com.epl.geometry.VertexDescription newDescription)
		{
			if (m_attributes == null)
			{
				m_description = newDescription;
				return;
			}
			int[] mapping = com.epl.geometry.VertexDescriptionDesignerImpl.MapAttributes(newDescription, m_description);
			double[] newAttributes = new double[(newDescription.GetTotalComponentCount() - 2) * 2];
			int old_offset0 = _getEndPointOffset(m_description, 0);
			int old_offset1 = _getEndPointOffset(m_description, 1);
			int new_offset0 = _getEndPointOffset(newDescription, 0);
			int new_offset1 = _getEndPointOffset(newDescription, 1);
			int j = 0;
			for (int i = 1, n = newDescription.GetAttributeCount(); i < n; i++)
			{
				int semantics = newDescription.GetSemantics(i);
				int nords = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				if (mapping[i] == -1)
				{
					double d = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					for (int ord = 0; ord < nords; ord++)
					{
						newAttributes[new_offset0 + j] = d;
						newAttributes[new_offset1 + j] = d;
						j++;
					}
				}
				else
				{
					int m = mapping[i];
					int offset = m_description._getPointAttributeOffset(m) - 2;
					for (int ord = 0; ord < nords; ord++)
					{
						newAttributes[new_offset0 + j] = m_attributes[old_offset0 + offset];
						newAttributes[new_offset1 + j] = m_attributes[old_offset1 + offset];
						j++;
						offset++;
					}
				}
			}
			m_attributes = newAttributes;
			m_description = newDescription;
		}

		private void _get(int endPoint, com.epl.geometry.Point outPoint)
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("empty geometry");
			}
			// ._setToDefault();
			outPoint.AssignVertexDescription(m_description);
			if (outPoint.IsEmptyImpl())
			{
				outPoint._setToDefault();
			}
			for (int attributeIndex = 0; attributeIndex < m_description.GetAttributeCount(); attributeIndex++)
			{
				int semantics = m_description._getSemanticsImpl(attributeIndex);
				for (int icomp = 0, ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics); icomp < ncomp; icomp++)
				{
					double v = _getAttributeAsDbl(endPoint, semantics, icomp);
					outPoint.SetAttribute(semantics, icomp, v);
				}
			}
		}

		private void _set(int endPoint, com.epl.geometry.Point src)
		{
			_touch();
			com.epl.geometry.Point point = src;
			if (src.IsEmptyImpl())
			{
				// can not assign an empty point
				throw new com.epl.geometry.GeometryException("empty_Geometry");
			}
			com.epl.geometry.VertexDescription vdin = point.GetDescription();
			for (int attributeIndex = 0, nattrib = vdin.GetAttributeCount(); attributeIndex < nattrib; attributeIndex++)
			{
				int semantics = vdin._getSemanticsImpl(attributeIndex);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int icomp = 0; icomp < ncomp; icomp++)
				{
					double v = point.GetAttributeAsDbl(semantics, icomp);
					_setAttribute(endPoint, semantics, icomp, v);
				}
			}
		}

		internal virtual double _getAttributeAsDbl(int endPoint, int semantics, int ordinate)
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation was performed on an Empty Geometry.");
			}
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (endPoint != 0)
				{
					return (ordinate != 0) ? m_yEnd : m_xEnd;
				}
				else
				{
					return (ordinate != 0) ? m_yStart : m_xStart;
				}
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (attributeIndex >= 0)
			{
				if (m_attributes != null)
				{
					_resizeAttributes(m_description.GetTotalComponentCount() - 2);
				}
				return m_attributes[_getEndPointOffset(m_description, endPoint) + m_description._getPointAttributeOffset(attributeIndex) - 2 + ordinate];
			}
			else
			{
				return com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
			}
		}

		private int _getAttributeAsInt(int endPoint, int semantics, int ordinate)
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("Empty_Geometry.");
			}
			return (int)_getAttributeAsDbl(endPoint, semantics, ordinate);
		}

		internal virtual void _setAttribute(int endPoint, int semantics, int ordinate, double value)
		{
			_touch();
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (attributeIndex < 0)
			{
				AddAttribute(semantics);
				attributeIndex = m_description.GetAttributeIndex(semantics);
			}
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (endPoint != 0)
				{
					if (ordinate != 0)
					{
						m_yEnd = value;
					}
					else
					{
						m_xEnd = value;
					}
				}
				else
				{
					if (ordinate != 0)
					{
						m_yStart = value;
					}
					else
					{
						m_xStart = value;
					}
				}
				return;
			}
			if (m_attributes == null)
			{
				_resizeAttributes(m_description.GetTotalComponentCount() - 2);
			}
			m_attributes[_getEndPointOffset(m_description, endPoint) + m_description._getPointAttributeOffset(attributeIndex) - 2 + ordinate] = value;
		}

		internal virtual void _setAttribute(int endPoint, int semantics, int ordinate, int value)
		{
			_setAttribute(endPoint, semantics, ordinate, (double)value);
		}

		public override void CopyTo(com.epl.geometry.Geometry dst)
		{
			if (dst.GetType() != GetType())
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.Segment segDst = (com.epl.geometry.Segment)dst;
			segDst.m_description = m_description;
			segDst._resizeAttributes(m_description.GetTotalComponentCount() - 2);
			_attributeCopy(m_attributes, 0, segDst.m_attributes, 0, (m_description.GetTotalComponentCount() - 2) * 2);
			segDst.m_xStart = m_xStart;
			segDst.m_yStart = m_yStart;
			segDst.m_xEnd = m_xEnd;
			segDst.m_yEnd = m_yEnd;
			dst._touch();
			_copyToImpl(segDst);
		}

		public override com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate)
		{
			com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
			if (IsEmptyImpl())
			{
				env.SetEmpty();
				return env;
			}
			env.vmin = _getAttributeAsDbl(0, semantics, ordinate);
			env.vmax = env.vmin;
			env.MergeNE(_getAttributeAsDbl(1, semantics, ordinate));
			return env;
		}

		internal virtual void QueryCoord(double t, com.epl.geometry.Point point)
		{
			point.AssignVertexDescription(m_description);
			point.SetXY(GetCoord2D(t));
			for (int iattrib = 1, nattrib = m_description.GetAttributeCount(); iattrib < nattrib; iattrib++)
			{
				int semantics = m_description._getSemanticsImpl(iattrib);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomp; iord++)
				{
					double value = GetAttributeAsDbl(t, semantics, iord);
					point.SetAttribute(semantics, iord, value);
				}
			}
		}

		internal virtual bool _equalsImpl(com.epl.geometry.Segment other)
		{
			if (m_description != other.m_description)
			{
				return false;
			}
			if (m_xStart != other.m_xStart || m_xEnd != other.m_xEnd || m_yStart != other.m_yStart || m_yEnd != other.m_yEnd)
			{
				return false;
			}
			for (int i = 0; i < (m_description.GetTotalComponentCount() - 2) * 2; i++)
			{
				if (m_attributes[i] != other.m_attributes[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true, when this segment is a closed curve (start point is equal
		/// to end point exactly).
		/// </summary>
		/// <remarks>
		/// Returns true, when this segment is a closed curve (start point is equal
		/// to end point exactly).
		/// Note, this will return true for lines, that are degenerate to a point
		/// too.
		/// </remarks>
		internal virtual bool IsClosed()
		{
			return m_xStart == m_xEnd && m_yStart == m_yEnd;
		}

		internal virtual void Reverse()
		{
			_reverseImpl();
			double origxStart = m_xStart;
			double origxEnd = m_xEnd;
			m_xStart = origxEnd;
			m_xEnd = origxStart;
			double origyStart = m_yStart;
			double origyEnd = m_yEnd;
			m_yStart = origyEnd;
			m_yEnd = origyStart;
			for (int i = 1, n = m_description.GetAttributeCount(); i < n; i++)
			{
				int semantics = m_description.GetSemantics(i);
				// VertexDescription.Semantics
				// semantics =
				// m_description.getSemantics(i);
				for (int iord = 0, nord = com.epl.geometry.VertexDescription.GetComponentCount(semantics); iord < nord; iord++)
				{
					double v1 = _getAttributeAsDbl(0, semantics, iord);
					double v2 = _getAttributeAsDbl(1, semantics, iord);
					_setAttribute(0, semantics, iord, v2);
					_setAttribute(1, semantics, iord, v1);
				}
			}
		}

		internal virtual int _isIntersecting(com.epl.geometry.Segment other, double tolerance, bool bExcludeExactEndpoints)
		{
			int gtThis = GetType().Value();
			int gtOther = other.GetType().Value();
			switch (gtThis)
			{
				case com.epl.geometry.Geometry.GeometryType.Line:
				{
					if (gtOther == com.epl.geometry.Geometry.GeometryType.Line)
					{
						return com.epl.geometry.Line._isIntersectingLineLine((com.epl.geometry.Line)this, (com.epl.geometry.Line)other, tolerance, bExcludeExactEndpoints);
					}
					else
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					goto default;
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		internal virtual int _intersect(com.epl.geometry.Segment other, com.epl.geometry.Point2D[] intersectionPoints, double[] paramThis, double[] paramOther, double tolerance)
		{
			int gtThis = GetType().Value();
			int gtOther = other.GetType().Value();
			switch (gtThis)
			{
				case com.epl.geometry.Geometry.GeometryType.Line:
				{
					if (gtOther == com.epl.geometry.Geometry.GeometryType.Line)
					{
						return com.epl.geometry.Line._intersectLineLine((com.epl.geometry.Line)this, (com.epl.geometry.Line)other, intersectionPoints, paramThis, paramOther, tolerance);
					}
					else
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					goto default;
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		/// <summary>A helper function for area calculation.</summary>
		/// <remarks>
		/// A helper function for area calculation. Calculates the Integral(y(t)
		/// x'(t) * dt) for t = [0, 1]. The area of a ring is caluclated as a sum of
		/// the results of CalculateArea2DHelper.
		/// </remarks>
		internal abstract double _calculateArea2DHelper(double xorg, double yorg);

		internal static int _getEndPointOffset(com.epl.geometry.VertexDescription vd, int endPoint)
		{
			return endPoint * (vd.GetTotalComponentCount() - 2);
		}

		/// <summary>
		/// Returns the coordinate of the point on this segment for the given
		/// parameter value.
		/// </summary>
		public virtual com.epl.geometry.Point2D GetCoord2D(double t)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			GetCoord2D(t, pt);
			return pt;
		}

		/// <summary>
		/// Returns the coordinate of the point on this segment for the given
		/// parameter value (segments are parametric curves).
		/// </summary>
		/// <param name="t">
		/// the parameter coordinate along the segment from 0.0 to 1.0.
		/// Value of 0 returns the start point, 1 returns end point.
		/// </param>
		/// <param name="dst">the coordinate where result will be placed.</param>
		public abstract void GetCoord2D(double t, com.epl.geometry.Point2D dst);

		/// <summary>Finds a closest coordinate on this segment.</summary>
		/// <param name="inputPoint">The 2D point to find the closest coordinate on this segment.</param>
		/// <param name="bExtrapolate">
		/// TRUE if the segment is extrapolated at the end points along
		/// the end point tangents. Otherwise the result is limited to
		/// values between 0 and 1.
		/// </param>
		/// <returns>
		/// The parametric coordinate t on the segment (0 corresponds to the
		/// start point, 1 corresponds to the end point). Use getCoord2D to
		/// obtain the 2D coordinate on the segment from t. To find the
		/// distance, call (inputPoint.sub(seg.getCoord2D(t))).length();
		/// </returns>
		public abstract double GetClosestCoordinate(com.epl.geometry.Point2D inputPoint, bool bExtrapolate);

		/// <summary>
		/// Splits this segment into Y monotonic parts and places them into the input
		/// array.
		/// </summary>
		/// <param name="monotonicSegments">
		/// The in/out array of SegmentBuffer structures that will be
		/// filled with the monotonic parts. The monotonicSegments array
		/// must contain at least 3 elements.
		/// </param>
		/// <returns>
		/// The number of monotonic parts if the split had happened. Returns
		/// 0 if the segment is already monotonic.
		/// </returns>
		internal abstract int GetYMonotonicParts(com.epl.geometry.SegmentBuffer[] monotonicSegments);

		/// <summary>
		/// Calculates intersection points of this segment with an infinite line,
		/// parallel to one of the axes.
		/// </summary>
		/// <param name="bAxisX">
		/// TRUE if the function works with the line parallel to the axis
		/// X.
		/// </param>
		/// <param name="ordinate">The ordinate value of the line (x for axis Y, y for axis X).</param>
		/// <param name="resultOrdinates">
		/// The value of ordinate in the intersection points One ordinate
		/// is equal to the ordinate parameter. This parameter can be
		/// NULL.
		/// </param>
		/// <param name="parameters">
		/// The value of the parameter in the intersection points (between
		/// 0 and 1). This parameter can be NULL.
		/// </param>
		/// <returns>
		/// The number of intersection points, 0 when no intersection points
		/// exist, -1 when the segment coincides with the line (infinite
		/// number of intersection points).
		/// </returns>
		public abstract int IntersectionWithAxis2D(bool bAxisX, double ordinate, double[] resultOrdinates, double[] parameters);

		internal virtual void _reverseImpl()
		{
		}

		/// <summary>
		/// Returns True if the segment is degenerate to a point with relation to the
		/// given tolerance.
		/// </summary>
		/// <remarks>
		/// Returns True if the segment is degenerate to a point with relation to the
		/// given tolerance. For Lines this means the line length is not longer than
		/// the tolerance. For the curves, the distance between the segment endpoints
		/// should not be longer than the tolerance and the distance from the line,
		/// connecting the endpoints to the furtherst point on the segment is not
		/// larger than the tolerance.
		/// </remarks>
		internal abstract bool IsDegenerate(double tolerance);

		// Cpp definitions
		internal abstract bool IsCurve();

		internal abstract com.epl.geometry.Point2D _getTangent(double t);

		internal abstract bool _isDegenerate(double tolerance);

		internal virtual double _calculateSubLength(double t)
		{
			return TToLength(t);
		}

		internal virtual double _calculateSubLength(double t1, double t2)
		{
			return TToLength(t2) - TToLength(t1);
		}

		internal abstract void _copyToImpl(com.epl.geometry.Segment dst);

		/// <summary>Returns subsegment between parameters t1 and t2.</summary>
		/// <remarks>
		/// Returns subsegment between parameters t1 and t2. The attributes are
		/// interpolated along the length of the curve.
		/// </remarks>
		public abstract com.epl.geometry.Segment Cut(double t1, double t2);

		/// <summary>
		/// Calculates the subsegment between parameters t1 and t2, and stores the
		/// result in subSegmentBuffer.
		/// </summary>
		/// <remarks>
		/// Calculates the subsegment between parameters t1 and t2, and stores the
		/// result in subSegmentBuffer. The attributes are interpolated along the
		/// length of the curve.
		/// </remarks>
		internal abstract void Cut(double t1, double t2, com.epl.geometry.SegmentBuffer subSegmentBuffer);

		/// <summary>Returns the attribute on the segment for the given parameter value.</summary>
		/// <remarks>
		/// Returns the attribute on the segment for the given parameter value. The
		/// interpolation of attribute is given by the attribute interpolation type.
		/// </remarks>
		public abstract double GetAttributeAsDbl(double t, int semantics, int ordinate);

		internal abstract bool _isIntersectingPoint(com.epl.geometry.Point2D pt, double tolerance, bool bExcludeExactEndpoints);

		/// <summary>
		/// Calculates intersection point of this segment with an infinite line,
		/// parallel to axis X.
		/// </summary>
		/// <remarks>
		/// Calculates intersection point of this segment with an infinite line,
		/// parallel to axis X. This segment must be to be y-monotonic (or
		/// horizontal).
		/// </remarks>
		/// <param name="y">The y coordinate of the line.</param>
		/// <param name="xParallel">
		/// For segments, that are horizontal, and have y coordinate, this
		/// value is returned.
		/// </param>
		/// <returns>X coordinate of the intersection, or NaN, if no intersection.</returns>
		internal abstract double IntersectionOfYMonotonicWithAxisX(double y, double xParallel);

		/// <summary>Converts curves parameter t to the curve length.</summary>
		/// <remarks>Converts curves parameter t to the curve length. Can be expensive for curves.</remarks>
		internal abstract double TToLength(double t);

		internal abstract double LengthToT(double len);

		public virtual double Distance(com.epl.geometry.Segment otherSegment, bool bSegmentsKnownDisjoint)
		{
			/* const */
			// if the segments are not known to be disjoint, and
			// the segments are found to touch in any way, then return 0.0
			if (!bSegmentsKnownDisjoint && _isIntersecting(otherSegment, 0, false) != 0)
			{
				return 0.0;
			}
			double minDistance = com.epl.geometry.NumberUtils.DoubleMax();
			com.epl.geometry.Point2D input_point;
			double t;
			double distance;
			input_point = GetStartXY();
			t = otherSegment.GetClosestCoordinate(input_point, false);
			input_point.Sub(otherSegment.GetCoord2D(t));
			distance = input_point.Length();
			if (distance < minDistance)
			{
				minDistance = distance;
			}
			input_point = GetEndXY();
			t = otherSegment.GetClosestCoordinate(input_point, false);
			input_point.Sub(otherSegment.GetCoord2D(t));
			distance = input_point.Length();
			if (distance < minDistance)
			{
				minDistance = distance;
			}
			input_point = otherSegment.GetStartXY();
			t = GetClosestCoordinate(input_point, false);
			input_point.Sub(GetCoord2D(t));
			distance = input_point.Length();
			if (distance < minDistance)
			{
				minDistance = distance;
			}
			input_point = otherSegment.GetEndXY();
			t = GetClosestCoordinate(input_point, false);
			input_point.Sub(GetCoord2D(t));
			distance = input_point.Length();
			if (distance < minDistance)
			{
				minDistance = distance;
			}
			return minDistance;
		}

		public override com.epl.geometry.Geometry GetBoundary()
		{
			return com.epl.geometry.Boundary.Calculate(this, null);
		}
	}
}
