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
	/// <summary>This class is a base for geometries with many vertices.</summary>
	/// <remarks>
	/// This class is a base for geometries with many vertices.
	/// The vertex attributes are stored in separate arrays of corresponding type.
	/// There are as many arrays as there are attributes in the vertex. It uses lazy
	/// allocation for the vertex attributes. This means, the actual AttributeStream
	/// is allocated only when the users asks for it, or sets a non-default value.
	/// </remarks>
	[System.Serializable]
	internal abstract class MultiVertexGeometryImpl : com.epl.geometry.MultiVertexGeometry
	{
		public abstract class GeometryXSimple
		{
			public const int Unknown = -1;

			public const int Not = 0;

			public const int Weak = 1;

			public const int Strong = 2;
			// HEADER DEFINED
			// not know if simple or not
			// not simple
			// weak simple (no self intersections, ring
			// orientation is correct, but ring order is not)
			// same as weak simple + OGC ring order.
		}

		public static class GeometryXSimpleConstants
		{
		}

		// TODO Remove?
		/// <summary>\internal CHildren implement this method to copy additional information</summary>
		internal abstract void _copyToImpl(com.epl.geometry.MultiVertexGeometryImpl mvg);

		protected internal abstract void _notifyModifiedAllImpl();

		/// <summary>
		/// \internal Called inside of the VerifyAllStreams to get a child class a
		/// chance to do additional verify.
		/// </summary>
		protected internal abstract void _verifyStreamsImpl();

		public abstract class DirtyFlags
		{
			public const int DirtyIsKnownSimple = 1;

			public const int IsWeakSimple = 2;

			public const int IsStrongSimple = 4;

			public const int DirtyOGCFlags = 8;

			public const int DirtyVerifiedStreams = 32;

			public const int DirtyExactIntervals = 64;

			public const int DirtyLooseIntervals = 128;

			public const int DirtyIntervals = DirtyExactIntervals | DirtyLooseIntervals;

			public const int DirtyIsEnvelope = 256;

			public const int DirtyLength2D = 512;

			public const int DirtyRingAreas2D = 1024;

			public const int DirtyCoordinates = DirtyIsKnownSimple | DirtyIntervals | DirtyIsEnvelope | DirtyLength2D | DirtyRingAreas2D | DirtyOGCFlags;

			public const int DirtyAllInternal = unchecked((int)(0xFFFF));

			public const int DirtyAll = unchecked((int)(0xFFFFFF));
			// !<0 when IsWeakSimple
			// flag is valid
			// !<when DirtyIsKnownSimple
			// is 0, this flag indicates
			// whether the geometry is
			// weak simple or not
			// !<OGCFlags are set by
			// Simplify or WKB/WKT
			// import.
			// < at least one
			// stream is
			// unverified
			// < exact envelope is
			// dirty
			// <
			// loose
			// and
			// dirty
			// envelopes
			// are
			// loose
			// < the geometry is not
			// known to be an
			// envelope
			// < the geometry length
			// needs update
			// update
			// <
			// m_cachedRingAreas2D
			// need update
			// there has been no
			// change to the
			// streams from
			// outside.
			// there has been a change
			// to one of attribute
			// streams from the
			// outside.
		}

		public static class DirtyFlagsConstants
		{
		}

		/// <summary>Returns the total vertex count in this Geometry.</summary>
		/// <returns>total vertex count in this Geometry.</returns>
		public override int GetPointCount()
		{
			return m_pointCount;
		}

		public override bool IsEmpty()
		{
			return IsEmptyImpl();
		}

		public virtual com.epl.geometry.VertexDescription GetDescriptionImpl()
		{
			return m_description;
		}

		internal virtual bool IsEmptyImpl()
		{
			return m_pointCount == 0;
		}

		protected internal virtual bool _hasDirtyFlag(int flag)
		{
			return (m_flagsMask & flag) != 0;
		}

		protected internal virtual void _setDirtyFlag(int flag, bool bYesNo)
		{
			if (bYesNo)
			{
				m_flagsMask |= flag;
			}
			else
			{
				m_flagsMask &= ~flag;
			}
		}

		protected internal virtual void _verifyAllStreams()
		{
			if (_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyVerifiedStreams))
			{
				_verifyAllStreamsImpl();
			}
		}

		protected internal virtual void ThrowIfEmpty()
		{
			if (IsEmptyImpl())
			{
				// TODO fix exceptions
				throw new com.epl.geometry.GeometryException("This operation was performed on an Empty Geometry.");
			}
		}

		private const long serialVersionUID = 1L;

		internal com.epl.geometry.AttributeStreamBase[] m_vertexAttributes;

		internal com.epl.geometry.GeometryAccelerators m_accelerators;

		internal com.epl.geometry.Envelope m_envelope;

		protected internal int m_pointCount;

		protected internal int m_reservedPointCount;

		protected internal int m_flagsMask;

		protected internal double m_simpleTolerance;

		public MultiVertexGeometryImpl()
		{
			// TODO implement accelerators
			// the BBOX for all attributes
			// the number of vertices reserved and
			// initialized to default value.
			// HEADER DEFINED
			// Cpp
			// Checked vs. Jan 11, 2011
			m_flagsMask = com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAllInternal;
			m_pointCount = 0;
			m_reservedPointCount = -1;
			m_accelerators = null;
		}

		public override void GetPointByVal(int index, com.epl.geometry.Point dst)
		{
			if (index < 0 || index >= m_pointCount)
			{
				// TODO
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			// _ASSERT(!IsEmpty());
			// _ASSERT(m_vertexAttributes != null);
			_verifyAllStreams();
			com.epl.geometry.Point outPoint = dst;
			outPoint.AssignVertexDescription(m_description);
			if (outPoint.IsEmpty())
			{
				outPoint._setToDefault();
			}
			for (int attributeIndex = 0; attributeIndex < m_description.GetAttributeCount(); attributeIndex++)
			{
				// fix semantics
				int semantics = m_description._getSemanticsImpl(attributeIndex);
				// VertexDescription.getComponentCount(semantics);
				for (int icomp = 0, ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics); icomp < ncomp; icomp++)
				{
					double v = m_vertexAttributes[attributeIndex].ReadAsDbl(ncomp * index + icomp);
					outPoint.SetAttribute(semantics, icomp, v);
				}
			}
		}

		public override void SetPointByVal(int index, com.epl.geometry.Point src)
		{
			if (index < 0 || index >= m_pointCount)
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			com.epl.geometry.Point point = src;
			if (src.IsEmpty())
			{
				// can not assign an empty point to a multipoint
				// vertex
				throw new System.ArgumentException();
			}
			_verifyAllStreams();
			// verify all allocated streams are of necessary
			// size.
			com.epl.geometry.VertexDescription vdin = point.GetDescription();
			for (int attributeIndex = 0; attributeIndex < vdin.GetAttributeCount(); attributeIndex++)
			{
				int semantics = vdin._getSemanticsImpl(attributeIndex);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int icomp = 0; icomp < ncomp; icomp++)
				{
					double v = point.GetAttributeAsDbl(semantics, icomp);
					SetAttribute(semantics, index, icomp, v);
				}
			}
		}

		// Checked vs. Jan 11, 2011
		public override com.epl.geometry.Point2D GetXY(int index)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			GetXY(index, pt);
			return pt;
		}

		public override void GetXY(int index, com.epl.geometry.Point2D pt)
		{
			if (index < 0 || index >= GetPointCount())
			{
				throw new System.IndexOutOfRangeException();
			}
			_verifyAllStreams();
			// AttributeStreamOfDbl v = (AttributeStreamOfDbl)
			// m_vertexAttributes[0];
			com.epl.geometry.AttributeStreamOfDbl v = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			v.Read(index * 2, pt);
		}

		// Checked vs. Jan 11, 2011
		public override void SetXY(int index, com.epl.geometry.Point2D pt)
		{
			if (index < 0 || index >= m_pointCount)
			{
				// TODO exception
				throw new System.IndexOutOfRangeException();
			}
			_verifyAllStreams();
			// AttributeStreamOfDbl v = (AttributeStreamOfDbl)
			// m_vertexAttributes[0];
			com.epl.geometry.AttributeStreamOfDbl v = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			v.Write(index * 2, pt);
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		// Checked vs. Jan 11, 2011
		public virtual void SetXY(int index, double x, double y)
		{
			if (index < 0 || index >= m_pointCount)
			{
				// TODO exc
				throw new System.IndexOutOfRangeException();
			}
			_verifyAllStreams();
			// AttributeStreamOfDbl v = (AttributeStreamOfDbl)
			// m_vertexAttributes[0];
			// TODO ask sergey about casts
			com.epl.geometry.AttributeStreamOfDbl v = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			v.Write(index * 2, x);
			v.Write(index * 2 + 1, y);
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		// Checked vs. Jan 11, 2011
		internal override com.epl.geometry.Point3D GetXYZ(int index)
		{
			if (index < 0 || index >= GetPointCount())
			{
				throw new System.IndexOutOfRangeException();
			}
			_verifyAllStreams();
			com.epl.geometry.AttributeStreamOfDbl v = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.Point3D pt = new com.epl.geometry.Point3D();
			pt.x = v.Read(index * 2);
			pt.y = v.Read(index * 2 + 1);
			// TODO check excluded if statement componenet
			if (HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z))
			{
				// && (m_vertexAttributes[1] != null))
				pt.z = m_vertexAttributes[1].ReadAsDbl(index);
			}
			else
			{
				pt.z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			return pt;
		}

		// Checked vs. Jan 11, 2011
		internal override void SetXYZ(int index, com.epl.geometry.Point3D pt)
		{
			if (index < 0 || index >= GetPointCount())
			{
				throw new System.IndexOutOfRangeException();
			}
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			_verifyAllStreams();
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
			com.epl.geometry.AttributeStreamOfDbl v = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			v.Write(index * 2, pt.x);
			v.Write(index * 2 + 1, pt.y);
			m_vertexAttributes[1].WriteAsDbl(index, pt.z);
		}

		// Checked vs. Jan 11, 2011
		internal override double GetAttributeAsDbl(int semantics, int offset, int ordinate)
		{
			if (offset < 0 || offset >= m_pointCount)
			{
				throw new System.IndexOutOfRangeException();
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			_verifyAllStreams();
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			// TODO check if statement
			if (attributeIndex >= 0)
			{
				// && m_vertexAttributes[attributeIndex] !=
				// null) {
				return m_vertexAttributes[attributeIndex].ReadAsDbl(offset * ncomps + ordinate);
			}
			return com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
		}

		// Checked vs. Jan 11, 2011
		internal override int GetAttributeAsInt(int semantics, int offset, int ordinate)
		{
			return (int)GetAttributeAsDbl(semantics, offset, ordinate);
		}

		// Checked vs. Jan 11, 2011
		internal override void SetAttribute(int semantics, int offset, int ordinate, double value)
		{
			if (offset < 0 || offset >= m_pointCount)
			{
				throw new System.IndexOutOfRangeException();
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			AddAttribute(semantics);
			_verifyAllStreams();
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
			m_vertexAttributes[attributeIndex].WriteAsDbl(offset * ncomps + ordinate, value);
		}

		// Checked vs. Jan 11, 2011
		internal override void SetAttribute(int semantics, int offset, int ordinate, int value)
		{
			SetAttribute(semantics, offset, ordinate, (double)value);
		}

		public virtual com.epl.geometry.AttributeStreamBase GetAttributeStreamRef(int semantics)
		{
			ThrowIfEmpty();
			AddAttribute(semantics);
			_verifyAllStreams();
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			return m_vertexAttributes[attributeIndex];
		}

		/// <summary>Sets a reference to the given AttributeStream of the Geometry.</summary>
		/// <remarks>
		/// Sets a reference to the given AttributeStream of the Geometry. Once the
		/// buffer has been obtained, the vertices of the Geometry can be manipulated
		/// directly. The AttributeStream parameters are not checked for the size. <br />
		/// If the attribute is missing, it will be added. <br />
		/// Note, that this method does not change the vertex count in the Geometry. <br />
		/// The stream can have more elements, than the Geometry point count, but
		/// only necessary part will be saved when exporting to a ESRI shape or other
		/// format. @param semantics Semantics of the attribute to assign the stream
		/// to. @param stream The input AttributeStream that will be assigned by
		/// reference. If one changes the stream later through the reference, one has
		/// to call NotifyStreamChanged. \exception Throws invalid_argument exception
		/// if the input stream type does not match that of the semantics
		/// persistence.
		/// </remarks>
		public virtual void SetAttributeStreamRef(int semantics, com.epl.geometry.AttributeStreamBase stream)
		{
			// int test1 = VertexDescription.getPersistence(semantics);
			// int test2 = stream.getPersistence();
			if ((stream != null) && com.epl.geometry.VertexDescription.GetPersistence(semantics) != stream.GetPersistence())
			{
				// input stream has wrong persistence
				throw new System.ArgumentException();
			}
			// Do not check for the stream size here to allow several streams to be
			// attached before the point count is changed.
			AddAttribute(semantics);
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (m_vertexAttributes == null)
			{
				m_vertexAttributes = new com.epl.geometry.AttributeStreamBase[m_description.GetAttributeCount()];
			}
			m_vertexAttributes[attributeIndex] = stream;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
		}

		protected internal override void _assignVertexDescriptionImpl(com.epl.geometry.VertexDescription newDescription)
		{
			com.epl.geometry.AttributeStreamBase[] newAttributes = null;
			if (m_vertexAttributes != null)
			{
				int[] mapping = com.epl.geometry.VertexDescriptionDesignerImpl.MapAttributes(newDescription, m_description);
				newAttributes = new com.epl.geometry.AttributeStreamBase[newDescription.GetAttributeCount()];
				for (int i = 0, n = newDescription.GetAttributeCount(); i < n; i++)
				{
					if (mapping[i] != -1)
					{
						int m = mapping[i];
						newAttributes[i] = m_vertexAttributes[m];
					}
				}
			}
			//if there are no streams we do not create them
			m_description = newDescription;
			m_vertexAttributes = newAttributes;
			// late assignment to try to stay
			m_reservedPointCount = -1;
			// we need to recreate the new attribute then
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
		}

		// Checked vs. Jan 11, 2011
		protected internal virtual void _updateEnvelope(com.epl.geometry.Envelope2D env)
		{
			_updateAllDirtyIntervals(true);
			m_envelope.QueryEnvelope2D(env);
		}

		// note: overload for polylines/polygons with curves
		// Checked vs. Jan 11, 2011
		protected internal virtual void _updateEnvelope(com.epl.geometry.Envelope3D env)
		{
			_updateAllDirtyIntervals(true);
			m_envelope.QueryEnvelope3D(env);
		}

		// note: overload for polylines/polygons with curves
		// Checked vs. Jan 11, 2011
		protected internal virtual void _updateLooseEnvelope(com.epl.geometry.Envelope2D env)
		{
			// TODO ROHIT has this set to true?
			_updateAllDirtyIntervals(false);
			m_envelope.QueryEnvelope2D(env);
		}

		// note: overload for polylines/polygons with curves
		// Checked vs. Jan 11, 2011
		/// <summary>\internal Calculates loose envelope.</summary>
		/// <remarks>
		/// \internal Calculates loose envelope. Returns True if the calculation
		/// renders exact envelope.
		/// </remarks>
		protected internal virtual void _updateLooseEnvelope(com.epl.geometry.Envelope3D env)
		{
			// TODO ROHIT has this set to true?
			_updateAllDirtyIntervals(false);
			m_envelope.QueryEnvelope3D(env);
		}

		// note: overload for polylines/polygons with curves
		// Checked vs. Jan 11, 2011
		public override void QueryEnvelope(com.epl.geometry.Envelope env)
		{
			_updateAllDirtyIntervals(true);
			m_envelope.CopyTo(env);
		}

		// TODO rename to remove 2D
		// Checked vs. Jan 11, 2011
		public override void QueryEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			_updateEnvelope(env);
		}

		// Checked vs. Jan 11, 2011
		// TODO rename to remove 3D
		internal override void QueryEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			_updateEnvelope(env);
		}

		// Checked vs. Jan 11, 2011
		// TODO rename to remove 2D
		public override void QueryLooseEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			_updateLooseEnvelope(env);
		}

		// Checked vs. Jan 11, 2011
		// TODO rename to remove 3D
		internal override void QueryLooseEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			_updateLooseEnvelope(env);
		}

		// Checked vs. Jan 11, 2011
		public override com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate)
		{
			com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
			if (IsEmptyImpl())
			{
				env.SetEmpty();
				return env;
			}
			_updateAllDirtyIntervals(true);
			return m_envelope.QueryInterval(semantics, ordinate);
		}

		// Checked vs. Jan 11, 2011
		// TODO Rename to getHashCode
		public override int GetHashCode()
		{
			int hashCode = m_description.GetHashCode();
			if (!IsEmptyImpl())
			{
				int pointCount = GetPointCount();
				for (int i = 0, n = m_description.GetAttributeCount(); i < n; i++)
				{
					int components = com.epl.geometry.VertexDescription.GetComponentCount(m_description._getSemanticsImpl(i));
					com.epl.geometry.AttributeStreamBase stream = m_vertexAttributes[i];
					hashCode = stream.CalculateHashImpl(hashCode, 0, pointCount * components);
				}
			}
			return hashCode;
		}

		// Checked vs. Jan 11, 2011
		public override bool Equals(object other)
		{
			// Java checks
			if (other == this)
			{
				return true;
			}
			if (!(other is com.epl.geometry.MultiVertexGeometryImpl))
			{
				return false;
			}
			com.epl.geometry.MultiVertexGeometryImpl otherMulti = (com.epl.geometry.MultiVertexGeometryImpl)other;
			if (!(m_description.Equals(otherMulti.m_description)))
			{
				return false;
			}
			if (IsEmptyImpl() != otherMulti.IsEmptyImpl())
			{
				return false;
			}
			if (IsEmptyImpl())
			{
				return true;
			}
			// both geometries are empty
			int pointCount = GetPointCount();
			int pointCountOther = otherMulti.GetPointCount();
			if (pointCount != pointCountOther)
			{
				return false;
			}
			for (int i = 0; i < m_description.GetAttributeCount(); i++)
			{
				int semantics = m_description.GetSemantics(i);
				com.epl.geometry.AttributeStreamBase stream = GetAttributeStreamRef(semantics);
				com.epl.geometry.AttributeStreamBase streamOther = otherMulti.GetAttributeStreamRef(semantics);
				int components = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				if (!stream.Equals(streamOther, 0, pointCount * components))
				{
					return false;
				}
			}
			return true;
		}

		// Checked vs. Jan 11, 2011
		/// <summary>Sets the envelope of the Geometry.</summary>
		/// <remarks>
		/// Sets the envelope of the Geometry. The Envelope description must match
		/// that of the Geometry.
		/// </remarks>
		public virtual void SetEnvelope(com.epl.geometry.Envelope env)
		{
			if (!m_description.Equals(env.GetDescription()))
			{
				throw new System.ArgumentException();
			}
			// m_envelope = (Envelope) env.clone();
			m_envelope = (com.epl.geometry.Envelope)env.CreateInstance();
			env.CopyTo(m_envelope);
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIntervals, false);
		}

		public override void CopyTo(com.epl.geometry.Geometry dstGeom)
		{
			com.epl.geometry.MultiVertexGeometryImpl dst = (com.epl.geometry.MultiVertexGeometryImpl)dstGeom;
			if (dst.GetType() != GetType())
			{
				throw new System.ArgumentException();
			}
			_copyToUnsafe(dst);
		}

		//Does not check geometry type. Used to copy Polygon to Polyline
		internal virtual void _copyToUnsafe(com.epl.geometry.MultiVertexGeometryImpl dst)
		{
			_verifyAllStreams();
			dst.m_description = m_description;
			dst.m_vertexAttributes = null;
			int nattrib = m_description.GetAttributeCount();
			com.epl.geometry.AttributeStreamBase[] cloneAttributes = null;
			if (m_vertexAttributes != null)
			{
				cloneAttributes = new com.epl.geometry.AttributeStreamBase[nattrib];
				for (int i = 0; i < nattrib; i++)
				{
					if (m_vertexAttributes[i] != null)
					{
						int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(m_description._getSemanticsImpl(i));
						cloneAttributes[i] = m_vertexAttributes[i].RestrictedClone(GetPointCount() * ncomps);
					}
				}
			}
			if (m_envelope != null)
			{
				dst.m_envelope = (com.epl.geometry.Envelope)m_envelope.CreateInstance();
				m_envelope.CopyTo(dst.m_envelope);
			}
			else
			{
				// dst.m_envelope = (Envelope) m_envelope.clone();
				dst.m_envelope = null;
			}
			dst.m_pointCount = m_pointCount;
			dst.m_flagsMask = m_flagsMask;
			dst.m_vertexAttributes = cloneAttributes;
			try
			{
				_copyToImpl(dst);
			}
			catch (System.Exception ex)
			{
				// copy child props
				dst.SetEmpty();
				throw new System.Exception("",ex);
			}
		}

		// Checked vs. Jan 11, 2011
		public virtual bool _attributeStreamIsAllocated(int semantics)
		{
			ThrowIfEmpty();
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			if (attributeIndex >= 0 && m_vertexAttributes[attributeIndex] != null)
			{
				return true;
			}
			return false;
		}

		// Checked vs. Jan 11, 2011
		internal virtual void _setEmptyImpl()
		{
			m_pointCount = 0;
			m_reservedPointCount = -1;
			m_vertexAttributes = null;
			// release it all streams.
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
		}

		// Checked vs. Jan 11, 2011
		/// <summary>
		/// Notifies the Geometry of changes made to the vertices so that it could
		/// reset cached structures.
		/// </summary>
		public virtual void NotifyModified(int flags)
		{
			if (flags == com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll)
			{
				m_reservedPointCount = -1;
				// forget the reserved point number
				_notifyModifiedAllImpl();
			}
			m_flagsMask |= flags;
			_clearAccelerators();
			_touch();
		}

		// Checked vs. Jan 11, 2011
		/// <param name="bExact">
		/// True, when the exact envelope need to be calculated and false
		/// for the loose one.
		/// </param>
		protected internal virtual void _updateAllDirtyIntervals(bool bExact)
		{
			_verifyAllStreams();
			if (_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIntervals))
			{
				if (null == m_envelope)
				{
					m_envelope = new com.epl.geometry.Envelope(m_description);
				}
				else
				{
					m_envelope.AssignVertexDescription(m_description);
				}
				if (IsEmpty())
				{
					m_envelope.SetEmpty();
					return;
				}
				_updateXYImpl(bExact);
				// efficient method for xy's
				// now go through other attribues.
				for (int attributeIndex = 1; attributeIndex < m_description.GetAttributeCount(); attributeIndex++)
				{
					int semantics = m_description._getSemanticsImpl(attributeIndex);
					int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					com.epl.geometry.AttributeStreamBase stream = m_vertexAttributes[attributeIndex];
					for (int iord = 0; iord < ncomps; iord++)
					{
						com.epl.geometry.Envelope1D interval = new com.epl.geometry.Envelope1D();
						interval.SetEmpty();
						for (int i = 0; i < m_pointCount; i++)
						{
							double value = stream.ReadAsDbl(i * ncomps + iord);
							// some
							// optimization
							// is
							// possible
							// if
							// non-virtual
							// method
							// is
							// used
							interval.Merge(value);
						}
						m_envelope.SetInterval(semantics, iord, interval);
					}
				}
				if (bExact)
				{
					_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIntervals, false);
				}
			}
		}

		// Checked vs. Jan 11, 2011
		/// <summary>\internal Updates x, y intervals.</summary>
		public virtual void _updateXYImpl(bool bExact)
		{
			m_envelope.SetEmpty();
			com.epl.geometry.AttributeStreamOfDbl stream = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int i = 0; i < m_pointCount; i++)
			{
				stream.Read(2 * i, pt);
				m_envelope.Merge(pt);
			}
		}

		internal virtual void CalculateEnvelope2D(com.epl.geometry.Envelope2D env, bool bExact)
		{
			env.SetEmpty();
			com.epl.geometry.AttributeStreamOfDbl stream = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int i = 0; i < m_pointCount; i++)
			{
				stream.Read(2 * i, pt);
				env.Merge(pt);
			}
		}

		// Checked vs. Jan 11, 2011 lots of changes
		/// <summary>\internal Verifies all streams (calls _VerifyStream for every attribute).</summary>
		protected internal virtual void _verifyAllStreamsImpl()
		{
			// This method checks that the streams are of correct size.
			// It resizes the streams to ensure they are not shorter than
			// m_PointCount
			// _ASSERT(_HasDirtyFlag(enum_value1(DirtyFlags,
			// DirtyVerifiedStreams)));
			if (m_reservedPointCount < m_pointCount)
			{
				// an optimization to skip this
				// expensive loop when
				// adding point by point
				if (m_vertexAttributes == null)
				{
					m_vertexAttributes = new com.epl.geometry.AttributeStreamBase[m_description.GetAttributeCount()];
				}
				m_reservedPointCount = com.epl.geometry.NumberUtils.IntMax();
				for (int attributeIndex = 0; attributeIndex < m_description.GetAttributeCount(); attributeIndex++)
				{
					int semantics = m_description._getSemanticsImpl(attributeIndex);
					if (m_vertexAttributes[attributeIndex] != null)
					{
						int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						int size = m_vertexAttributes[attributeIndex].VirtualSize() / ncomp;
						if (size < m_pointCount)
						{
							size = (m_reservedPointCount > m_pointCount + 5) ? (m_pointCount * 5 + 3) / 4 : m_pointCount;
							// reserve 25% more than user
							// asks
							m_vertexAttributes[attributeIndex].Resize(size * ncomp, com.epl.geometry.VertexDescription.GetDefaultValue(semantics));
						}
						if (size < m_reservedPointCount)
						{
							m_reservedPointCount = size;
						}
					}
					else
					{
						m_vertexAttributes[attributeIndex] = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(semantics, m_pointCount);
						m_reservedPointCount = m_pointCount;
					}
				}
			}
			_verifyStreamsImpl();
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyVerifiedStreams, false);
		}

		// Checked vs. Jan 11, 2011
		internal virtual void _resizeImpl(int pointCount)
		{
			if (pointCount < 0)
			{
				throw new System.ArgumentException();
			}
			if (pointCount == m_pointCount)
			{
				return;
			}
			m_pointCount = pointCount;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAllInternal);
		}

		// Checked vs. Jan 11, 2011
		internal virtual int QueryCoordinates(com.epl.geometry.Point2D[] dst, int dstSize, int beginIndex, int endIndex)
		{
			int endIndexC = endIndex < 0 ? m_pointCount : endIndex;
			endIndexC = System.Math.Min(endIndexC, beginIndex + dstSize);
			if (beginIndex < 0 || beginIndex >= m_pointCount || endIndexC < beginIndex)
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			int j = 0;
			double[] dstArray = new double[dst.Length * 2];
			xy.ReadRange(2 * beginIndex, (endIndexC - beginIndex) * 2, dstArray, j, true);
			for (int i = 0; i < dst.Length; i++)
			{
				dst[i] = new com.epl.geometry.Point2D(dstArray[i * 2], dstArray[i * 2 + 1]);
			}
			// for (int i = beginIndex; i < endIndexC; i++, j++)
			// {
			// xy.read(2 * i, dst[j]);
			// }
			return endIndexC;
		}

		// Checked vs. Jan 11, 2011
		internal virtual int QueryCoordinates(com.epl.geometry.Point3D[] dst, int dstSize, int beginIndex, int endIndex)
		{
			int endIndexC = endIndex < 0 ? m_pointCount : endIndex;
			endIndexC = System.Math.Min(endIndexC, beginIndex + dstSize);
			if (beginIndex < 0 || beginIndex >= m_pointCount || endIndexC < beginIndex)
			{
				// TODO replace geometry exc
				throw new System.ArgumentException();
			}
			com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.AttributeStreamOfDbl z = null;
			double v = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
			bool bHasZ = HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			if (bHasZ)
			{
				z = (com.epl.geometry.AttributeStreamOfDbl)GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			int j = 0;
			for (int i = beginIndex; i < endIndexC; i++, j++)
			{
				dst[j].x = xy.Read(2 * i);
				dst[j].y = xy.Read(2 * i + 1);
				dst[j].z = bHasZ ? z.Read(i) : v;
				dst[j] = GetXYZ(i);
			}
			return endIndexC;
		}

		// Checked vs. Jan 11, 2011
		// -1 : DirtySimple is true (whether or not the MultiPath is Simple is
		// unknown)
		// 0 : DirtySimple is false and the MultiPath is not Weak Simple
		// 1 : DirtySimple is false and the MultiPath is Weak Simple but not ring
		// ordering may be invalid
		// 2 : DirtySimple is false and the MultiPath is Strong Simple (Weak Simple
		// and valid ring ordering)
		public virtual int GetIsSimple(double tolerance)
		{
			if (!_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIsKnownSimple))
			{
				if (!_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsWeakSimple))
				{
					return 0;
				}
				if (m_simpleTolerance >= tolerance)
				{
					if (!_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags))
					{
						return 2;
					}
					return 1;
				}
				return -1;
			}
			return -1;
		}

		internal virtual void SetIsSimple(int isSimpleRes, double tolerance, bool ogc_known)
		{
			m_simpleTolerance = tolerance;
			if (isSimpleRes == com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Unknown)
			{
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIsKnownSimple, true);
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags, true);
				return;
			}
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIsKnownSimple, false);
			if (!ogc_known)
			{
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags, true);
			}
			if (isSimpleRes == com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Not)
			{
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsWeakSimple, false);
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsStrongSimple, false);
			}
			else
			{
				if (isSimpleRes == com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak)
				{
					_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsWeakSimple, true);
					_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsStrongSimple, false);
				}
				else
				{
					if (isSimpleRes == com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong)
					{
						_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsWeakSimple, true);
						_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.IsStrongSimple, true);
					}
					else
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
				}
			}
		}

		// what?
		internal virtual double _getSimpleTolerance()
		{
			return m_simpleTolerance;
		}

		public virtual com.epl.geometry.GeometryAccelerators _getAccelerators()
		{
			return m_accelerators;
		}

		internal virtual void _clearAccelerators()
		{
			if (m_accelerators != null)
			{
				m_accelerators = null;
			}
		}

		internal virtual void _interpolateTwoVertices(int vertex1, int vertex2, double f, com.epl.geometry.Point outPoint)
		{
			if (vertex1 < 0 || vertex1 >= m_pointCount)
			{
				throw new com.epl.geometry.GeometryException("index out of bounds.");
			}
			if (vertex2 < 0 || vertex2 >= m_pointCount)
			{
				throw new com.epl.geometry.GeometryException("index out of bounds.");
			}
			// _ASSERT(!IsEmpty());
			// _ASSERT(m_vertexAttributes != NULLPTR);
			_verifyAllStreams();
			outPoint.AssignVertexDescription(m_description);
			if (outPoint.IsEmpty())
			{
				outPoint._setToDefault();
			}
			for (int attributeIndex = 0; attributeIndex < m_description.GetAttributeCount(); attributeIndex++)
			{
				int semantics = m_description._getSemanticsImpl(attributeIndex);
				for (int icomp = 0, ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics); icomp < ncomp; icomp++)
				{
					double v1 = m_vertexAttributes[attributeIndex].ReadAsDbl(ncomp * vertex1 + icomp);
					double v2 = m_vertexAttributes[attributeIndex].ReadAsDbl(ncomp * vertex2 + icomp);
					outPoint.SetAttribute(semantics, icomp, com.epl.geometry.MathUtils.Lerp(v1, v2, f));
				}
			}
		}

		internal virtual double _getShortestDistance(int vertex1, int vertex2)
		{
			com.epl.geometry.Point2D pt = GetXY(vertex1);
			pt.Sub(GetXY(vertex2));
			return pt.Length();
		}

		// ////////////////// METHODS To REMOVE ///////////////////////
		public override com.epl.geometry.Point GetPoint(int index)
		{
			if (index < 0 || index >= m_pointCount)
			{
				throw new System.IndexOutOfRangeException();
			}
			_verifyAllStreams();
			com.epl.geometry.Point outPoint = new com.epl.geometry.Point();
			outPoint.AssignVertexDescription(m_description);
			if (outPoint.IsEmpty())
			{
				outPoint._setToDefault();
			}
			for (int attributeIndex = 0; attributeIndex < m_description.GetAttributeCount(); attributeIndex++)
			{
				int semantics = m_description.GetSemantics(attributeIndex);
				for (int icomp = 0, ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics); icomp < ncomp; icomp++)
				{
					double v = m_vertexAttributes[attributeIndex].ReadAsDbl(ncomp * index + icomp);
					outPoint.SetAttribute(semantics, icomp, v);
				}
			}
			return outPoint;
		}

		public override void SetPoint(int index, com.epl.geometry.Point src)
		{
			if (index < 0 || index >= m_pointCount)
			{
				throw new System.IndexOutOfRangeException();
			}
			com.epl.geometry.Point point = src;
			if (src.IsEmpty())
			{
				// can not assign an empty point to a multipoint
				// vertex
				throw new System.ArgumentException();
			}
			_verifyAllStreams();
			// verify all allocated streams are of necessary
			// size.
			com.epl.geometry.VertexDescription vdin = point.GetDescription();
			for (int attributeIndex = 0; attributeIndex < vdin.GetAttributeCount(); attributeIndex++)
			{
				int semantics = vdin.GetSemantics(attributeIndex);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int icomp = 0; icomp < ncomp; icomp++)
				{
					double v = point.GetAttributeAsDbl(semantics, icomp);
					SetAttribute(semantics, index, icomp, v);
				}
			}
		}

		public override void QueryCoordinates(com.epl.geometry.Point[] dst)
		{
			int sz = m_pointCount;
			if (dst.Length < sz)
			{
				throw new System.ArgumentException();
			}
			// TODO: refactor to a better AttributeAray call (ReadRange?)
			for (int i = 0; i < sz; i++)
			{
				dst[i] = GetPoint(i);
			}
		}

		public override void QueryCoordinates(com.epl.geometry.Point2D[] dst)
		{
			int sz = m_pointCount;
			if (dst.Length < sz)
			{
				throw new System.ArgumentException();
			}
			// TODO: refactor to a better AttributeAray call (ReadRange?)
			for (int i = 0; i < sz; i++)
			{
				dst[i] = GetXY(i);
			}
		}

		internal override void QueryCoordinates(com.epl.geometry.Point3D[] dst)
		{
			int sz = m_pointCount;
			if (dst.Length < sz)
			{
				throw new System.ArgumentException();
			}
			// TODO: refactor to a better AttributeAray call (ReadRange?)
			for (int i = 0; i < sz; i++)
			{
				dst[i] = GetXYZ(i);
			}
		}

		public override void ReplaceNaNs(int semantics, double value)
		{
			AddAttribute(semantics);
			if (IsEmpty())
			{
				return;
			}
			bool modified = false;
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			for (int i = 0; i < ncomps; i++)
			{
				com.epl.geometry.AttributeStreamBase streamBase = GetAttributeStreamRef(semantics);
				if (streamBase is com.epl.geometry.AttributeStreamOfDbl)
				{
					com.epl.geometry.AttributeStreamOfDbl dblStream = (com.epl.geometry.AttributeStreamOfDbl)streamBase;
					for (int ivert = 0, n = m_pointCount * ncomps; ivert < n; ivert++)
					{
						double v = dblStream.Read(ivert);
						if (double.IsNaN(v))
						{
							dblStream.Write(ivert, value);
							modified = true;
						}
					}
				}
				else
				{
					for (int ivert = 0, n = m_pointCount * ncomps; ivert < n; ivert++)
					{
						double v = streamBase.ReadAsDbl(ivert);
						if (double.IsNaN(v))
						{
							streamBase.WriteAsDbl(ivert, value);
							modified = true;
						}
					}
				}
			}
			if (modified)
			{
				NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
			}
		}

		public abstract bool _buildRasterizedGeometryAccelerator(double toleranceXY, com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree);

		public abstract bool _buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree d);

		public override string ToString()
		{
			return "MultiVertexGeometryImpl";
		}
	}
}
