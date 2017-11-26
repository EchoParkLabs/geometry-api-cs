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
	/// <summary>
	/// A Point is a zero-dimensional object that represents a specific (X,Y)
	/// location in a two-dimensional XY-Plane.
	/// </summary>
	/// <remarks>
	/// A Point is a zero-dimensional object that represents a specific (X,Y)
	/// location in a two-dimensional XY-Plane. In case of Geographic Coordinate
	/// Systems, the X coordinate is the longitude and the Y is the latitude.
	/// </remarks>
	[System.Serializable]
	public class Point : com.epl.geometry.Geometry
	{
		internal double[] m_attributes;

		/// <summary>Creates an empty 2D point.</summary>
		public Point()
		{
			//We are using writeReplace instead.
			//private static final long serialVersionUID = 2L;
			// use doubles to store everything (long are bitcast)
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
		}

		public Point(com.epl.geometry.VertexDescription vd)
		{
			if (vd == null)
			{
				throw new System.ArgumentException();
			}
			m_description = vd;
		}

		/// <summary>Creates a 2D Point with specified X and Y coordinates.</summary>
		/// <remarks>
		/// Creates a 2D Point with specified X and Y coordinates. In case of
		/// Geographic Coordinate Systems, the X coordinate is the longitude and the
		/// Y is the latitude.
		/// </remarks>
		/// <param name="x">The X coordinate of the new 2D point.</param>
		/// <param name="y">The Y coordinate of the new 2D point.</param>
		public Point(double x, double y)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			SetXY(x, y);
		}

		public Point(com.epl.geometry.Point2D pt)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			SetXY(pt);
		}

		/// <summary>Creates a 3D point with specified X, Y and Z coordinates.</summary>
		/// <remarks>
		/// Creates a 3D point with specified X, Y and Z coordinates. In case of
		/// Geographic Coordinate Systems, the X coordinate is the longitude and the
		/// Y is the latitude.
		/// </remarks>
		/// <param name="x">The X coordinate of the new 3D point.</param>
		/// <param name="y">The Y coordinate of the new 3D point.</param>
		/// <param name="z">The Z coordinate of the new 3D point.</param>
		public Point(double x, double y, double z)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.SetCoords(x, y, z);
			SetXYZ(pt);
		}

		/// <summary>Returns XY coordinates of this point.</summary>
		public com.epl.geometry.Point2D GetXY()
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation should not be performed on an empty geometry.");
			}
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.SetCoords(m_attributes[0], m_attributes[1]);
			return pt;
		}

		/// <summary>Returns XY coordinates of this point.</summary>
		public void GetXY(com.epl.geometry.Point2D pt)
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation should not be performed on an empty geometry.");
			}
			pt.SetCoords(m_attributes[0], m_attributes[1]);
		}

		/// <summary>Sets the XY coordinates of this point.</summary>
		/// <remarks>
		/// Sets the XY coordinates of this point. param pt The point to create the X
		/// and Y coordinate from.
		/// </remarks>
		public void SetXY(com.epl.geometry.Point2D pt)
		{
			_touch();
			SetXY(pt.x, pt.y);
		}

		/// <summary>Returns XYZ coordinates of the point.</summary>
		/// <remarks>Returns XYZ coordinates of the point. Z will be set to 0 if Z is missing.</remarks>
		public virtual com.epl.geometry.Point3D GetXYZ()
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation should not be performed on an empty geometry.");
			}
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.x = m_attributes[0];
			pt.y = m_attributes[1];
			if (m_description.HasZ())
			{
				pt.z = m_attributes[2];
			}
			else
			{
				pt.z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			return pt;
		}

		/// <summary>Sets the XYZ coordinates of this point.</summary>
		/// <param name="pt">The point to create the XYZ coordinate from.</param>
		public virtual void SetXYZ(com.epl.geometry.Point3D pt)
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
			if (m_attributes == null)
			{
				_setToDefault();
			}
			m_attributes[0] = pt.x;
			m_attributes[1] = pt.y;
			if (bHasZ)
			{
				m_attributes[2] = pt.z;
			}
		}

		/// <summary>Returns the X coordinate of the point.</summary>
		public double GetX()
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation should not be performed on an empty geometry.");
			}
			return m_attributes[0];
		}

		/// <summary>Sets the X coordinate of the point.</summary>
		/// <param name="x">The X coordinate to be set for this point.</param>
		public virtual void SetX(double x)
		{
			SetAttribute(com.epl.geometry.VertexDescription.Semantics.POSITION, 0, x);
		}

		/// <summary>Returns the Y coordinate of this point.</summary>
		public double GetY()
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation should not be performed on an empty geometry.");
			}
			return m_attributes[1];
		}

		/// <summary>Sets the Y coordinate of this point.</summary>
		/// <param name="y">The Y coordinate to be set for this point.</param>
		public virtual void SetY(double y)
		{
			SetAttribute(com.epl.geometry.VertexDescription.Semantics.POSITION, 1, y);
		}

		/// <summary>Returns the Z coordinate of this point.</summary>
		public virtual double GetZ()
		{
			return GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0);
		}

		/// <summary>Sets the Z coordinate of this point.</summary>
		/// <param name="z">The Z coordinate to be set for this point.</param>
		public virtual void SetZ(double z)
		{
			SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, z);
		}

		/// <summary>Returns the attribute M of this point.</summary>
		public virtual double GetM()
		{
			return GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0);
		}

		/// <summary>Sets the M coordinate of this point.</summary>
		/// <param name="m">The M coordinate to be set for this point.</param>
		public virtual void SetM(double m)
		{
			SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, m);
		}

		/// <summary>Returns the ID of this point.</summary>
		public virtual int GetID()
		{
			return GetAttributeAsInt(com.epl.geometry.VertexDescription.Semantics.ID, 0);
		}

		/// <summary>Sets the ID of this point.</summary>
		/// <param name="id">The ID of this point.</param>
		public virtual void SetID(int id)
		{
			SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, id);
		}

		/// <summary>Returns value of the given vertex attribute's ordinate.</summary>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">
		/// The attribute's ordinate. For example, the Y coordinate of the
		/// NORMAL has ordinate of 1.
		/// </param>
		/// <returns>The ordinate as double value.</returns>
		public virtual double GetAttributeAsDbl(int semantics, int ordinate)
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation was performed on an Empty Geometry.");
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (attributeIndex >= 0)
			{
				return m_attributes[m_description._getPointAttributeOffset(attributeIndex) + ordinate];
			}
			else
			{
				return com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
			}
		}

		/// <summary>Returns value of the given vertex attribute's ordinate.</summary>
		/// <remarks>
		/// Returns value of the given vertex attribute's ordinate. The ordinate is
		/// always 0 because integer attributes always have one component.
		/// </remarks>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">
		/// The attribute's ordinate. For example, the y coordinate of the
		/// NORMAL has ordinate of 1.
		/// </param>
		/// <returns>The ordinate value truncated to a 32 bit integer value.</returns>
		public virtual int GetAttributeAsInt(int semantics, int ordinate)
		{
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("This operation was performed on an Empty Geometry.");
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (attributeIndex >= 0)
			{
				return (int)m_attributes[m_description._getPointAttributeOffset(attributeIndex) + ordinate];
			}
			else
			{
				return (int)com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
			}
		}

		/// <summary>Sets the value of the attribute.</summary>
		/// <param name="semantics">The attribute semantics.</param>
		/// <param name="ordinate">The ordinate of the attribute.</param>
		/// <param name="value">
		/// Is the array to write values to. The attribute type and the
		/// number of elements must match the persistence type, as well as
		/// the number of components of the attribute.
		/// </param>
		public virtual void SetAttribute(int semantics, int ordinate, double value)
		{
			_touch();
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ncomps < ordinate)
			{
				throw new System.IndexOutOfRangeException();
			}
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (attributeIndex < 0)
			{
				AddAttribute(semantics);
				attributeIndex = m_description.GetAttributeIndex(semantics);
			}
			if (m_attributes == null)
			{
				_setToDefault();
			}
			m_attributes[m_description._getPointAttributeOffset(attributeIndex) + ordinate] = value;
		}

		public virtual void SetAttribute(int semantics, int ordinate, int value)
		{
			SetAttribute(semantics, ordinate, (double)value);
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.Point;
		}

		public override int GetDimension()
		{
			return 0;
		}

		public override void SetEmpty()
		{
			_touch();
			if (m_attributes != null)
			{
				m_attributes[0] = com.epl.geometry.NumberUtils.NaN();
				m_attributes[1] = com.epl.geometry.NumberUtils.NaN();
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
			double[] newAttributes = new double[newDescription.GetTotalComponentCount()];
			int j = 0;
			for (int i = 0, n = newDescription.GetAttributeCount(); i < n; i++)
			{
				int semantics = newDescription.GetSemantics(i);
				int nords = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				if (mapping[i] == -1)
				{
					double d = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					for (int ord = 0; ord < nords; ord++)
					{
						newAttributes[j] = d;
						j++;
					}
				}
				else
				{
					int m = mapping[i];
					int offset = m_description._getPointAttributeOffset(m);
					for (int ord = 0; ord < nords; ord++)
					{
						newAttributes[j] = m_attributes[offset];
						j++;
						offset++;
					}
				}
			}
			m_attributes = newAttributes;
			m_description = newDescription;
		}

		/// <summary>Sets the Point to a default, non-empty state.</summary>
		internal virtual void _setToDefault()
		{
			ResizeAttributes(m_description.GetTotalComponentCount());
			com.epl.geometry.Point.AttributeCopy(m_description._getDefaultPointAttributes(), m_attributes, m_description.GetTotalComponentCount());
			m_attributes[0] = com.epl.geometry.NumberUtils.NaN();
			m_attributes[1] = com.epl.geometry.NumberUtils.NaN();
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			if (IsEmptyImpl())
			{
				return;
			}
			com.epl.geometry.Point2D pt = GetXY();
			transform.Transform(pt, pt);
			SetXY(pt);
		}

		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			if (IsEmptyImpl())
			{
				return;
			}
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			com.epl.geometry.Point3D pt = GetXYZ();
			SetXYZ(transform.Transform(pt));
		}

		public override void CopyTo(com.epl.geometry.Geometry dst)
		{
			if (dst.GetType() != com.epl.geometry.Geometry.Type.Point)
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.Point pointDst = (com.epl.geometry.Point)dst;
			dst._touch();
			if (m_attributes == null)
			{
				pointDst.SetEmpty();
				pointDst.m_attributes = null;
				pointDst.AssignVertexDescription(m_description);
			}
			else
			{
				pointDst.AssignVertexDescription(m_description);
				pointDst.ResizeAttributes(m_description.GetTotalComponentCount());
				AttributeCopy(m_attributes, pointDst.m_attributes, m_description.GetTotalComponentCount());
			}
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point(m_description);
			return point;
		}

		public override bool IsEmpty()
		{
			return IsEmptyImpl();
		}

		internal bool IsEmptyImpl()
		{
			return ((m_attributes == null) || com.epl.geometry.NumberUtils.IsNaN(m_attributes[0]) || com.epl.geometry.NumberUtils.IsNaN(m_attributes[1]));
		}

		public override void QueryEnvelope(com.epl.geometry.Envelope env)
		{
			env.SetEmpty();
			if (m_description != env.m_description)
			{
				env.AssignVertexDescription(m_description);
			}
			env.Merge(this);
		}

		public override void QueryEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			if (IsEmptyImpl())
			{
				env.SetEmpty();
				return;
			}
			env.xmin = m_attributes[0];
			env.ymin = m_attributes[1];
			env.xmax = m_attributes[0];
			env.ymax = m_attributes[1];
		}

		internal override void QueryEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			if (IsEmptyImpl())
			{
				env.SetEmpty();
				return;
			}
			com.epl.geometry.Point3D pt = GetXYZ();
			env.xmin = pt.x;
			env.ymin = pt.y;
			env.zmin = pt.z;
			env.xmax = pt.x;
			env.ymax = pt.y;
			env.zmax = pt.z;
		}

		public override com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate)
		{
			com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
			if (IsEmptyImpl())
			{
				env.SetEmpty();
				return env;
			}
			double s = GetAttributeAsDbl(semantics, ordinate);
			env.vmin = s;
			env.vmax = s;
			return env;
		}

		private void ResizeAttributes(int newSize)
		{
			if (m_attributes == null)
			{
				m_attributes = new double[newSize];
			}
			else
			{
				if (m_attributes.Length < newSize)
				{
					double[] newbuffer = new double[newSize];
					System.Array.Copy(m_attributes, 0, newbuffer, 0, m_attributes.Length);
					m_attributes = newbuffer;
				}
			}
		}

		internal static void AttributeCopy(double[] src, double[] dst, int count)
		{
			if (count > 0)
			{
				System.Array.Copy(src, 0, dst, 0, count);
			}
		}

		/// <summary>Set the X and Y coordinate of the point.</summary>
		/// <param name="x">X coordinate of the point.</param>
		/// <param name="y">Y coordinate of the point.</param>
		public virtual void SetXY(double x, double y)
		{
			_touch();
			if (m_attributes == null)
			{
				_setToDefault();
			}
			m_attributes[0] = x;
			m_attributes[1] = y;
		}

		/// <summary>
		/// Returns TRUE when this geometry has exactly same type, properties, and
		/// coordinates as the other geometry.
		/// </summary>
		public override bool Equals(object _other)
		{
			if (_other == this)
			{
				return true;
			}
			if (!(_other is com.epl.geometry.Point))
			{
				return false;
			}
			com.epl.geometry.Point otherPt = (com.epl.geometry.Point)_other;
			if (m_description != otherPt.m_description)
			{
				return false;
			}
			if (IsEmptyImpl())
			{
				if (otherPt.IsEmptyImpl())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			for (int i = 0, n = m_description.GetTotalComponentCount(); i < n; i++)
			{
				if (m_attributes[i] != otherPt.m_attributes[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Returns the hash code for the point.</summary>
		public override int GetHashCode()
		{
			int hashCode = m_description.GetHashCode();
			if (!IsEmptyImpl())
			{
				for (int i = 0, n = m_description.GetTotalComponentCount(); i < n; i++)
				{
					long bits = System.BitConverter.DoubleToInt64Bits(m_attributes[i]);
					int hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
					hashCode = com.epl.geometry.NumberUtils.Hash(hashCode, hc);
				}
			}
			return hashCode;
		}

		public override com.epl.geometry.Geometry GetBoundary()
		{
			return null;
		}

		public override void ReplaceNaNs(int semantics, double value)
		{
			AddAttribute(semantics);
			if (IsEmpty())
			{
				return;
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			for (int i = 0; i < ncomps; i++)
			{
				double v = GetAttributeAsDbl(semantics, i);
				if (double.IsNaN(v))
				{
					SetAttribute(semantics, i, value);
				}
			}
		}
	}
}
