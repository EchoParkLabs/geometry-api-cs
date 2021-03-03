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
	/// <summary>An envelope is an axis-aligned rectangle.</summary>
	[System.Serializable]
	public class Envelope : com.epl.geometry.Geometry
	{
		internal com.epl.geometry.Envelope2D m_envelope = new com.epl.geometry.Envelope2D();

		internal double[] m_attributes;

		/// <summary>Creates an envelope by defining its center, width, and height.</summary>
		/// <param name="center">The center point of the envelope.</param>
		/// <param name="width">The width of the envelope.</param>
		/// <param name="height">The height of the envelope.</param>
		public Envelope(com.epl.geometry.Point center, double width, double height)
		{
			//We are using writeReplace instead.
			//private static final long serialVersionUID = 2L;
			// use doubles to store everything
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			m_envelope.SetEmpty();
			if (center.IsEmpty())
			{
				return;
			}
			_setFromPoint(center, width, height);
		}

		public Envelope(com.epl.geometry.Envelope2D env2D)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			m_envelope.SetCoords(env2D);
			m_envelope.Normalize();
		}

		public Envelope(com.epl.geometry.VertexDescription vd)
		{
			if (vd == null)
			{
				throw new System.ArgumentException();
			}
			m_description = vd;
			m_envelope.SetEmpty();
		}

		public Envelope(com.epl.geometry.VertexDescription vd, com.epl.geometry.Envelope2D env2D)
		{
			if (vd == null)
			{
				throw new System.ArgumentException();
			}
			m_description = vd;
			m_envelope.SetCoords(env2D);
			m_envelope.Normalize();
		}

		/// <summary>Constructs an empty envelope.</summary>
		public Envelope()
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			m_envelope.SetEmpty();
		}

		/// <summary>Constructs an envelope that covers the given point.</summary>
		/// <remarks>
		/// Constructs an envelope that covers the given point. The coordinates of
		/// the point are used to set the extent of the envelope.
		/// </remarks>
		/// <param name="point">The point that the envelope covers.</param>
		public Envelope(com.epl.geometry.Point point)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			m_envelope.SetEmpty();
			if (point.IsEmpty())
			{
				return;
			}
			_setFromPoint(point);
		}

		/// <summary>Constructs an envelope with the specified X and Y extents.</summary>
		/// <param name="xmin">The minimum x-coordinate of the envelope.</param>
		/// <param name="ymin">The minimum y-coordinate of the envelope.</param>
		/// <param name="xmax">The maximum x-coordinate of the envelope.</param>
		/// <param name="ymax">The maximum y-coordinate of the envelope.</param>
		public Envelope(double xmin, double ymin, double xmax, double ymax)
		{
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			SetCoords(xmin, ymin, xmax, ymax);
		}

		/// <summary>Sets the 2-dimensional extents of the envelope.</summary>
		/// <param name="xmin">The minimum x-coordinate of the envelope.</param>
		/// <param name="ymin">The minimum y-coordinate of the envelope.</param>
		/// <param name="xmax">The maximum x-coordinate of the envelope.</param>
		/// <param name="ymax">The maximum y-coordinate of the envelope.</param>
		public virtual void SetCoords(double xmin, double ymin, double xmax, double ymax)
		{
			_touch();
			m_envelope.SetCoords(xmin, ymin, xmax, ymax);
		}

		/// <summary>Sets the envelope from the array of points.</summary>
		/// <remarks>
		/// Sets the envelope from the array of points. The result envelope is a
		/// bounding box of all the points in the array. If the array has zero
		/// length, the envelope will be empty.
		/// </remarks>
		/// <param name="points">The point array.</param>
		internal virtual void SetCoords(com.epl.geometry.Point[] points)
		{
			_touch();
			SetEmpty();
			for (int i = 0, n = points.Length; i < n; i++)
			{
				Merge(points[i]);
			}
		}

		internal virtual void SetEnvelope2D(com.epl.geometry.Envelope2D e2d)
		{
			_touch();
			if (!e2d.IsValid())
			{
				throw new System.ArgumentException();
			}
			m_envelope.SetCoords(e2d);
		}

		/// <summary>Removes all points from this geometry.</summary>
		public override void SetEmpty()
		{
			_touch();
			m_envelope.SetEmpty();
		}

		/// <summary>Indicates whether this envelope contains any points.</summary>
		/// <returns>boolean Returns true if the envelope is empty.</returns>
		public override bool IsEmpty()
		{
			return m_envelope.IsEmpty();
		}

		/// <summary>The width of the envelope.</summary>
		/// <returns>The width of the envelope.</returns>
		public virtual double GetWidth()
		{
			return m_envelope.GetWidth();
		}

		/// <summary>The height of the envelope.</summary>
		/// <returns>The height of the envelope.</returns>
		public virtual double GetHeight()
		{
			return m_envelope.GetHeight();
		}

		/// <summary>The x-coordinate of the center of the envelope.</summary>
		/// <returns>The x-coordinate of the center of the envelope.</returns>
		public virtual double GetCenterX()
		{
			return m_envelope.GetCenterX();
		}

		/// <summary>The y-coordinate of center of the envelope.</summary>
		/// <returns>The y-coordinate of center of the envelope.</returns>
		public virtual double GetCenterY()
		{
			return m_envelope.GetCenterY();
		}

		/// <summary>The x and y-coordinates of the center of the envelope.</summary>
		/// <returns>A point whose x and y-coordinates are that of the center of the envelope.</returns>
		public virtual com.epl.geometry.Point2D GetCenterXY()
		{
			return m_envelope.GetCenter();
		}

		public virtual void GetCenter(com.epl.geometry.Point point_out)
		{
			point_out.AssignVertexDescription(m_description);
			if (IsEmpty())
			{
				point_out.SetEmpty();
				return;
			}
			int nattrib = m_description.GetAttributeCount();
			for (int i = 1; i < nattrib; i++)
			{
				int semantics = m_description.GetSemantics(i);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomp; iord++)
				{
					double v = 0.5 * (GetAttributeAsDblImpl_(0, semantics, iord) + GetAttributeAsDblImpl_(1, semantics, iord));
					point_out.SetAttribute(semantics, iord, v);
				}
			}
			point_out.SetXY(m_envelope.GetCenter());
		}

		public virtual void Merge(com.epl.geometry.Point2D pt)
		{
			_touch();
			m_envelope.Merge(pt);
		}

		/// <summary>Merges this envelope with the extent of the given envelope.</summary>
		/// <remarks>
		/// Merges this envelope with the extent of the given envelope. If this
		/// envelope is empty, the coordinates of the given envelope
		/// are assigned. If the given envelope is empty, this envelope is unchanged.
		/// </remarks>
		/// <param name="other">The envelope to merge.</param>
		public virtual void Merge(com.epl.geometry.Envelope other)
		{
			_touch();
			if (other.IsEmpty())
			{
				return;
			}
			com.epl.geometry.VertexDescription otherVD = other.m_description;
			if (otherVD != m_description)
			{
				MergeVertexDescription(otherVD);
			}
			m_envelope.Merge(other.m_envelope);
			for (int iattrib = 1, nattrib = otherVD.GetAttributeCount(); iattrib < nattrib; iattrib++)
			{
				int semantics = otherVD.GetSemantics(iattrib);
				int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomps; iord++)
				{
					com.epl.geometry.Envelope1D intervalOther = other.QueryInterval(semantics, iord);
					com.epl.geometry.Envelope1D interval = QueryInterval(semantics, iord);
					interval.Merge(intervalOther);
					SetInterval(semantics, iord, interval);
				}
			}
		}

		/// <summary>Merges this envelope with the point.</summary>
		/// <remarks>
		/// Merges this envelope with the point. The boundary of the envelope is
		/// increased to include the point. If the envelope is empty, the coordinates
		/// of the point to merge are assigned. If the point is empty, the original
		/// envelope is unchanged.
		/// </remarks>
		/// <param name="point">The point to be merged.</param>
		public virtual void Merge(com.epl.geometry.Point point)
		{
			_touch();
			if (point.IsEmptyImpl())
			{
				return;
			}
			com.epl.geometry.VertexDescription pointVD = point.m_description;
			if (m_description != pointVD)
			{
				MergeVertexDescription(pointVD);
			}
			if (IsEmpty())
			{
				_setFromPoint(point);
				return;
			}
			m_envelope.Merge(point.GetXY());
			for (int iattrib = 1, nattrib = pointVD.GetAttributeCount(); iattrib < nattrib; iattrib++)
			{
				int semantics = pointVD._getSemanticsImpl(iattrib);
				int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomps; iord++)
				{
					double v = point.GetAttributeAsDbl(semantics, iord);
					com.epl.geometry.Envelope1D interval = QueryInterval(semantics, iord);
					interval.Merge(v);
					SetInterval(semantics, iord, interval);
				}
			}
		}

		internal virtual void _setFromPoint(com.epl.geometry.Point centerPoint, double width, double height)
		{
			m_envelope.SetCoords(centerPoint.GetXY(), width, height);
			com.epl.geometry.VertexDescription pointVD = centerPoint.m_description;
			for (int iattrib = 1, nattrib = pointVD.GetAttributeCount(); iattrib < nattrib; iattrib++)
			{
				int semantics = pointVD._getSemanticsImpl(iattrib);
				int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomps; iord++)
				{
					double v = centerPoint.GetAttributeAsDbl(semantics, iord);
					SetInterval(semantics, iord, v, v);
				}
			}
		}

		internal virtual void _setFromPoint(com.epl.geometry.Point centerPoint)
		{
			m_envelope.SetCoords(centerPoint.m_attributes[0], centerPoint.m_attributes[1]);
			com.epl.geometry.VertexDescription pointVD = centerPoint.m_description;
			for (int iattrib = 1, nattrib = pointVD.GetAttributeCount(); iattrib < nattrib; iattrib++)
			{
				int semantics = pointVD._getSemanticsImpl(iattrib);
				int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomps; iord++)
				{
					double v = centerPoint.GetAttributeAsDbl(semantics, iord);
					SetInterval(semantics, iord, v, v);
				}
			}
		}

		public virtual void Merge(com.epl.geometry.Envelope2D other)
		{
			_touch();
			m_envelope.Merge(other);
		}

		public virtual void SetInterval(int semantics, int ordinate, double vmin, double vmax)
		{
			SetInterval(semantics, ordinate, new com.epl.geometry.Envelope1D(vmin, vmax));
		}

		/// <summary>Re-aspects this envelope to fit within the specified width and height.</summary>
		/// <param name="arWidth">The width within which to fit the envelope.</param>
		/// <param name="arHeight">The height within which to fit the envelope.</param>
		public virtual void Reaspect(double arWidth, double arHeight)
		{
			_touch();
			m_envelope.Reaspect(arWidth, arHeight);
		}

		/// <summary>Changes the dimensions of the envelope while preserving the center.</summary>
		/// <remarks>
		/// Changes the dimensions of the envelope while preserving the center. New width
		/// is Width + 2 * dx, new height is Height + 2 * dy. If the result envelope
		/// width or height becomes negative, the envelope is set to be empty.
		/// </remarks>
		/// <param name="dx">The inflation along the x-axis.</param>
		/// <param name="dy">The inflation along the y-axis.</param>
		public virtual void Inflate(double dx, double dy)
		{
			_touch();
			m_envelope.Inflate(dx, dy);
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			_touch();
			transform.Transform(m_envelope);
		}

		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			_touch();
			if (!m_envelope.IsEmpty())
			{
				com.epl.geometry.Envelope3D env = new com.epl.geometry.Envelope3D();
				QueryEnvelope3D(env);
				if (env.IsEmptyZ())
				{
					env.SetEmpty();
				}
				else
				{
					// Z components is empty, the
					// AffineTransformation3D makes the whole
					// envelope empty. Consider
					// throwing an assert instead.
					transform.Transform(env);
				}
			}
		}

		public override void CopyTo(com.epl.geometry.Geometry dst)
		{
			if (dst.GetType() != GetType())
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.Envelope envDst = (com.epl.geometry.Envelope)dst;
			dst._touch();
			envDst.m_description = m_description;
			envDst.m_envelope.SetCoords(m_envelope);
			envDst.m_attributes = null;
			if (m_attributes != null)
			{
				envDst._ensureAttributes();
				System.Array.Copy(m_attributes, 0, envDst.m_attributes, 0, (m_description.GetTotalComponentCount() - 2) * 2);
			}
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.Envelope(m_description);
		}

		public override double CalculateArea2D()
		{
			return m_envelope.GetArea();
		}

		public override double CalculateLength2D()
		{
			return m_envelope.GetLength();
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.Envelope;
		}

		public override int GetDimension()
		{
			return 2;
		}

		public override void QueryEnvelope(com.epl.geometry.Envelope env)
		{
			CopyTo(env);
		}

		public override void QueryEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			env.xmin = m_envelope.xmin;
			env.ymin = m_envelope.ymin;
			env.xmax = m_envelope.xmax;
			env.ymax = m_envelope.ymax;
		}

		internal override void QueryEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			env.xmin = m_envelope.xmin;
			env.ymin = m_envelope.ymin;
			env.xmax = m_envelope.xmax;
			env.ymax = m_envelope.ymax;
			env.SetCoords(m_envelope.xmin, m_envelope.ymin, _getAttributeAsDbl(0, com.epl.geometry.VertexDescription.Semantics.Z, 0), m_envelope.xmax, m_envelope.ymax, _getAttributeAsDbl(1, com.epl.geometry.VertexDescription.Semantics.Z, 0));
		}

		public override com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate)
		{
			com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
			env.SetCoords(_getAttributeAsDbl(0, semantics, ordinate), _getAttributeAsDbl(1, semantics, ordinate));
			return env;
		}

		public virtual void SetInterval(int semantics, int ordinate, com.epl.geometry.Envelope1D env)
		{
			_touch();
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (ordinate == 0)
				{
					m_envelope.xmin = env.vmin;
					m_envelope.xmax = env.vmax;
				}
				else
				{
					if (ordinate == 1)
					{
						m_envelope.ymin = env.vmin;
						m_envelope.ymax = env.vmax;
					}
					else
					{
						throw new System.IndexOutOfRangeException();
					}
				}
			}
			else
			{
				_setAttributeAsDbl(0, semantics, ordinate, env.vmin);
				_setAttributeAsDbl(1, semantics, ordinate, env.vmax);
			}
		}

		public virtual void QueryCoordinates(com.epl.geometry.Point2D[] dst)
		{
			if (dst == null || dst.Length < 4 || m_envelope.IsEmpty())
			{
				throw new System.ArgumentException();
			}
			m_envelope.QueryCorners(dst);
		}

		/// <summary>
		/// Sets the point's coordinates to the coordinates of the envelope at the
		/// given corner.
		/// </summary>
		/// <param name="index">
		/// The index of the envelope's corners from 0 to 3.
		/// <p>
		/// 0 = lower left corner
		/// <p>
		/// 1 = top-left corner
		/// <p>
		/// 2 = top right corner
		/// <p>
		/// 3 = bottom right corner
		/// </param>
		/// <param name="ptDst">
		/// The point whose coordinates are used to set the envelope's
		/// coordinate at a specified corner.
		/// </param>
		public virtual void QueryCornerByVal(int index, com.epl.geometry.Point ptDst)
		{
			ptDst.AssignVertexDescription(m_description);
			int nattrib = GetDescription().GetAttributeCount() - 1;
			switch (index)
			{
				case 0:
				{
					for (int i = 0; i < nattrib; i++)
					{
						int semantics = m_description.GetSemantics(i);
						int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int iord = 0; iord < ncomp; iord++)
						{
							ptDst.SetAttribute(semantics, iord, _getAttributeAsDbl(0, semantics, iord));
						}
					}
					ptDst.SetXY(m_envelope.xmin, m_envelope.ymin);
					return;
				}

				case 1:
				{
					for (int i = 0; i < nattrib; i++)
					{
						int semantics = m_description.GetSemantics(i);
						int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int iord = 0; iord < ncomp; iord++)
						{
							ptDst.SetAttribute(semantics, iord, _getAttributeAsDbl(1, semantics, iord));
						}
					}
					ptDst.SetXY(m_envelope.xmin, m_envelope.ymax);
					return;
				}

				case 2:
				{
					for (int i = 0; i < nattrib; i++)
					{
						int semantics = m_description.GetSemantics(i);
						int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int iord = 0; iord < ncomp; iord++)
						{
							ptDst.SetAttribute(semantics, iord, _getAttributeAsDbl(0, semantics, iord));
						}
					}
					ptDst.SetXY(m_envelope.xmax, m_envelope.ymax);
					return;
				}

				case 3:
				{
					for (int i = 0; i < nattrib; i++)
					{
						int semantics = m_description.GetSemantics(i);
						int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int iord = 0; iord < ncomp; iord++)
						{
							ptDst.SetAttribute(semantics, iord, _getAttributeAsDbl(1, semantics, iord));
						}
					}
					ptDst.SetXY(m_envelope.xmax, m_envelope.ymin);
					return;
				}

				default:
				{
					throw new System.IndexOutOfRangeException();
				}
			}
		}

		public virtual void QueryCorner(int index, com.epl.geometry.Point2D ptDst)
		{
			com.epl.geometry.Point2D p = m_envelope.QueryCorner(index);
			ptDst.SetCoords(p.x, p.y);
		}

		internal virtual int GetEndPointOffset(com.epl.geometry.VertexDescription descr, int end_point)
		{
			return end_point * (descr.GetTotalComponentCount() - 2);
		}

		internal virtual double GetAttributeAsDblImpl_(int end_point, int semantics, int ordinate)
		{
			if (m_envelope.IsEmpty())
			{
				throw new com.epl.geometry.GeometryException("empty geometry");
			}
			System.Diagnostics.Debug.Assert((end_point == 0 || end_point == 1));
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (end_point != 0)
				{
					return ordinate != 0 ? m_envelope.ymax : m_envelope.xmax;
				}
				else
				{
					return ordinate != 0 ? m_envelope.ymin : m_envelope.xmin;
				}
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.ArgumentException();
			}
			int attribute_index = m_description.GetAttributeIndex(semantics);
			_ensureAttributes();
			if (attribute_index >= 0)
			{
				return m_attributes[GetEndPointOffset(m_description, end_point) + m_description.GetPointAttributeOffset_(attribute_index) - 2 + ordinate];
			}
			return com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
		}

		internal virtual void SetAttributeAsDblImpl_(int end_point, int semantics, int ordinate, double value)
		{
			System.Diagnostics.Debug.Assert((end_point == 0 || end_point == 1));
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (end_point != 0)
				{
					if (ordinate != 0)
					{
						m_envelope.ymax = value;
					}
					else
					{
						m_envelope.xmax = value;
					}
				}
				else
				{
					if (ordinate != 0)
					{
						m_envelope.ymin = value;
					}
					else
					{
						m_envelope.xmin = value;
					}
				}
				return;
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.ArgumentException();
			}
			AddAttribute(semantics);
			_ensureAttributes();
			int attribute_index = m_description.GetAttributeIndex(semantics);
			m_attributes[GetEndPointOffset(m_description, end_point) + m_description.GetPointAttributeOffset_(attribute_index) - 2 + ordinate] = value;
		}

		internal virtual void _ensureAttributes()
		{
			_touch();
			if (m_attributes == null && m_description.GetTotalComponentCount() > 2)
			{
				m_attributes = new double[(m_description.GetTotalComponentCount() - 2) * 2];
				int offset0 = _getEndPointOffset(m_description, 0);
				int offset1 = _getEndPointOffset(m_description, 1);
				int j = 0;
				for (int i = 1, n = m_description.GetAttributeCount(); i < n; i++)
				{
					int semantics = m_description.GetSemantics(i);
					int nords = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					double d = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					for (int ord = 0; ord < nords; ord++)
					{
						m_attributes[offset0 + j] = d;
						m_attributes[offset1 + j] = d;
						j++;
					}
				}
			}
		}

		protected internal override void _assignVertexDescriptionImpl(com.epl.geometry.VertexDescription newDescription)
		{
			if (m_attributes == null)
			{
				m_description = newDescription;
				return;
			}
			if (newDescription.GetTotalComponentCount() > 2)
			{
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
			}
			else
			{
				m_attributes = null;
			}
			m_description = newDescription;
		}

		internal virtual double _getAttributeAsDbl(int endPoint, int semantics, int ordinate)
		{
			if (m_envelope.IsEmpty())
			{
				throw new com.epl.geometry.GeometryException("This operation was performed on an Empty Geometry.");
			}
			// _ASSERT(endPoint == 0 || endPoint == 1);
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (endPoint != 0)
				{
					return ordinate != 0 ? m_envelope.ymax : m_envelope.xmax;
				}
				else
				{
					return ordinate != 0 ? m_envelope.ymin : m_envelope.xmin;
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
				_ensureAttributes();
				return m_attributes[_getEndPointOffset(m_description, endPoint) + m_description._getPointAttributeOffset(attributeIndex) - 2 + ordinate];
			}
			else
			{
				return com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
			}
		}

		internal virtual void _setAttributeAsDbl(int endPoint, int semantics, int ordinate, double value)
		{
			_touch();
			// _ASSERT(endPoint == 0 || endPoint == 1);
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				if (endPoint != 0)
				{
					if (ordinate != 0)
					{
						m_envelope.ymax = value;
					}
					else
					{
						m_envelope.xmax = value;
					}
				}
				else
				{
					if (ordinate != 0)
					{
						m_envelope.ymin = value;
					}
					else
					{
						m_envelope.xmin = value;
					}
				}
				return;
			}
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			if (ordinate >= ncomps)
			{
				throw new System.IndexOutOfRangeException();
			}
			if (!HasAttribute(semantics))
			{
				if (com.epl.geometry.VertexDescription.IsDefaultValue(semantics, value))
				{
					return;
				}
				AddAttribute(semantics);
			}
			int attributeIndex = m_description.GetAttributeIndex(semantics);
			_ensureAttributes();
			m_attributes[_getEndPointOffset(m_description, endPoint) + m_description._getPointAttributeOffset(attributeIndex) - 2 + ordinate] = value;
		}

		internal virtual int _getAttributeAsInt(int endPoint, int semantics, int ordinate)
		{
			return (int)_getAttributeAsDbl(endPoint, semantics, ordinate);
		}

		internal static int _getEndPointOffset(com.epl.geometry.VertexDescription vd, int endPoint)
		{
			return endPoint * (vd.GetTotalComponentCount() - 2);
		}

		public virtual bool IsIntersecting(com.epl.geometry.Envelope2D other)
		{
			return m_envelope.IsIntersecting(other);
		}

		/// <summary>
		/// Changes this envelope to be the intersection of itself with the other
		/// envelope.
		/// </summary>
		/// <param name="other">The envelope to intersect.</param>
		/// <returns>Returns true if the result is not empty.</returns>
		public virtual bool Intersect(com.epl.geometry.Envelope other)
		{
			_touch();
			com.epl.geometry.Envelope2D e2d = new com.epl.geometry.Envelope2D();
			other.QueryEnvelope2D(e2d);
			return m_envelope.Intersect(e2d);
		}

		/// <summary>Returns true if the envelope and the other given envelope intersect.</summary>
		/// <param name="other">The envelope to with which to test intersection.</param>
		/// <returns>Returns true if the two envelopes intersect.</returns>
		public virtual bool IsIntersecting(com.epl.geometry.Envelope other)
		{
			// TODO: attributes.
			return m_envelope.IsIntersecting(other.m_envelope);
		}

		/// <summary>
		/// Sets the envelope's corners to be centered around the specified point,
		/// using its center, width, and height.
		/// </summary>
		/// <param name="c">The point around which to center the envelope.</param>
		/// <param name="w">The width to be set for the envelope.</param>
		/// <param name="h">The height to be set for this envelope.</param>
		public virtual void CenterAt(com.epl.geometry.Point c, double w, double h)
		{
			_touch();
			if (c.IsEmpty())
			{
				SetEmpty();
				return;
			}
			_setFromPoint(c, w, h);
		}

		/// <summary>Offsets the envelope by the specified distances along x and y-coordinates.</summary>
		/// <param name="dx">The X offset to be applied.</param>
		/// <param name="dy">The Y offset to be applied.</param>
		public virtual void Offset(double dx, double dy)
		{
			_touch();
			m_envelope.Offset(dx, dy);
		}

		/// <summary>
		/// Normalizes envelopes if the minimum dimension is larger than the
		/// maximum dimension.
		/// </summary>
		public virtual void Normalize()
		{
			// TODO: attributes
			_touch();
			m_envelope.Normalize();
		}

		/// <summary>Gets the center point of the envelope.</summary>
		/// <remarks>
		/// Gets the center point of the envelope. The center point occurs at: ((XMin
		/// + XMax) / 2, (YMin + YMax) / 2).
		/// </remarks>
		/// <returns>The center point of the envelope.</returns>
		public virtual com.epl.geometry.Point2D GetCenter2D()
		{
			return m_envelope.GetCenter();
		}

		/// <summary>Returns the center point of the envelope.</summary>
		/// <returns>The center point of the envelope.</returns>
		public virtual com.epl.geometry.Point GetCenter()
		{
			com.epl.geometry.Point pointOut = new com.epl.geometry.Point(m_description);
			if (IsEmpty())
			{
				return pointOut;
			}
			int nattrib = m_description.GetAttributeCount();
			for (int i = 1; i < nattrib; i++)
			{
				int semantics = m_description._getSemanticsImpl(i);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int iord = 0; iord < ncomp; iord++)
				{
					double v = 0.5 * (_getAttributeAsDbl(0, semantics, iord) + _getAttributeAsDbl(1, semantics, iord));
					pointOut.SetAttribute(semantics, iord, v);
				}
			}
			pointOut.SetXY(m_envelope.GetCenterX(), m_envelope.GetCenterY());
			return pointOut;
		}

		/// <summary>
		/// Centers the envelope around the specified point preserving the envelope's
		/// width and height.
		/// </summary>
		/// <param name="c">The new center point.</param>
		public virtual void CenterAt(com.epl.geometry.Point c)
		{
			_touch();
			if (c.IsEmpty())
			{
				SetEmpty();
				return;
			}
			m_envelope.CenterAt(c.GetX(), c.GetY());
		}

		/// <summary>Returns the envelope's lower left corner point.</summary>
		/// <returns>Returns the lower left corner point.</returns>
		public virtual com.epl.geometry.Point GetLowerLeft()
		{
			return new com.epl.geometry.Point(m_envelope.GetLowerLeft());
		}

		/// <summary>Returns the envelope's upper right corner point.</summary>
		/// <returns>Returns the upper right corner point.</returns>
		public virtual com.epl.geometry.Point GetUpperRight()
		{
			return new com.epl.geometry.Point(m_envelope.GetUpperRight());
		}

		/// <summary>Returns the envelope's lower right corner point.</summary>
		/// <returns>Returns the lower right corner point.</returns>
		public virtual com.epl.geometry.Point GetLowerRight()
		{
			return new com.epl.geometry.Point(m_envelope.GetLowerRight());
		}

		/// <summary>Returns the envelope's upper left corner point.</summary>
		/// <returns>Returns the upper left corner point.</returns>
		public virtual com.epl.geometry.Point GetUpperLeft()
		{
			return new com.epl.geometry.Point(m_envelope.GetUpperLeft());
		}

		/// <summary>Checks if this envelope contains (covers) the specified point.</summary>
		/// <param name="p">The Point to be tested for coverage.</param>
		/// <returns>TRUE if this envelope contains (covers) the specified point.</returns>
		public virtual bool Contains(com.epl.geometry.Point p)
		{
			if (p.IsEmpty())
			{
				return false;
			}
			return m_envelope.Contains(p.GetX(), p.GetY());
		}

		/// <summary>Checks if this envelope contains (covers) other envelope.</summary>
		/// <param name="env">The envelope to be tested for coverage.</param>
		/// <returns>TRUE if this envelope contains (covers) the specified envelope.</returns>
		public virtual bool Contains(com.epl.geometry.Envelope env)
		{
			return m_envelope.Contains(env.m_envelope);
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
			if (!(_other is com.epl.geometry.Envelope))
			{
				return false;
			}
			com.epl.geometry.Envelope other = (com.epl.geometry.Envelope)_other;
			if (m_description != other.m_description)
			{
				return false;
			}
			if (IsEmpty())
			{
				if (other.IsEmpty())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			if (!this.m_envelope.Equals(other.m_envelope))
			{
				return false;
			}
			for (int i = 0, n = (m_description.GetTotalComponentCount() - 2) * 2; i < n; i++)
			{
				if (m_attributes[i] != other.m_attributes[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Returns a hash code value for this envelope.</summary>
		/// <returns>A hash code value for this envelope.</returns>
		public override int GetHashCode()
		{
			int hashCode = m_description.GetHashCode();
			hashCode = com.epl.geometry.NumberUtils.Hash(hashCode, m_envelope.GetHashCode());
			if (!IsEmpty() && m_attributes != null)
			{
				for (int i = 0, n = (m_description.GetTotalComponentCount() - 2) * 2; i < n; i++)
				{
					hashCode = com.epl.geometry.NumberUtils.Hash(hashCode, m_attributes[i]);
				}
			}
			return hashCode;
		}

		/// <summary>Returns the X coordinate of the left corners of the envelope.</summary>
		/// <returns>The X coordinate of the left corners.</returns>
		public double GetXMin()
		{
			return m_envelope.xmin;
		}

		/// <summary>Returns the Y coordinate of the bottom corners of the envelope.</summary>
		/// <returns>The Y coordinate of the bottom corners.</returns>
		public double GetYMin()
		{
			return m_envelope.ymin;
		}

		/// <summary>Returns the X coordinate of the right corners of the envelope.</summary>
		/// <returns>The X coordinate of the right corners.</returns>
		public double GetXMax()
		{
			return m_envelope.xmax;
		}

		/// <summary>Returns the Y coordinate of the top corners of the envelope.</summary>
		/// <returns>The Y coordinate of the top corners.</returns>
		public double GetYMax()
		{
			return m_envelope.ymax;
		}

		/// <summary>Sets the left X coordinate.</summary>
		/// <param name="x">The X coordinate of the left corner</param>
		public virtual void SetXMin(double x)
		{
			_touch();
			m_envelope.xmin = x;
		}

		/// <summary>Sets the right X coordinate.</summary>
		/// <param name="x">The X coordinate of the right corner.</param>
		public virtual void SetXMax(double x)
		{
			_touch();
			m_envelope.xmax = x;
		}

		/// <summary>Sets the bottom Y coordinate.</summary>
		/// <param name="y">the Y coordinate of the bottom corner.</param>
		public virtual void SetYMin(double y)
		{
			_touch();
			m_envelope.ymin = y;
		}

		/// <summary>Sets the top Y coordinate.</summary>
		/// <param name="y">The Y coordinate of the top corner.</param>
		public virtual void SetYMax(double y)
		{
			_touch();
			m_envelope.ymax = y;
		}

		public override com.epl.geometry.Geometry GetBoundary()
		{
			return com.epl.geometry.Boundary.Calculate(this, null);
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
				com.epl.geometry.Envelope1D interval = QueryInterval(semantics, i);
				if (interval.IsEmpty())
				{
					interval.vmin = value;
					interval.vmax = value;
					SetInterval(semantics, i, interval);
				}
			}
		}

		/// <summary>The output of this method can be only used for debugging.</summary>
		/// <remarks>The output of this method can be only used for debugging. It is subject to change without notice.</remarks>
		public override string ToString()
		{
			if (IsEmpty())
			{
				return "Envelope: []";
			}
			string s = "Envelope: [" + m_envelope.xmin + ", " + m_envelope.ymin + ", " + m_envelope.xmax + ", " + m_envelope.ymax + "]";
			return s;
		}
	}
}
