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
	[System.Serializable]
	internal sealed class MultiPathImpl : com.epl.geometry.MultiVertexGeometryImpl
	{
		protected internal bool m_bPolygon;

		protected internal com.epl.geometry.Point m_moveToPoint;

		protected internal double m_cachedLength2D;

		protected internal double m_cachedArea2D;

		protected internal com.epl.geometry.AttributeStreamOfDbl m_cachedRingAreas2D;

		protected internal bool m_bPathStarted;

		protected internal com.epl.geometry.AttributeStreamOfInt32 m_paths;

		protected internal com.epl.geometry.AttributeStreamOfInt8 m_pathFlags;

		protected internal com.epl.geometry.AttributeStreamOfInt8 m_segmentFlags;

		protected internal com.epl.geometry.AttributeStreamOfInt32 m_segmentParamIndex;

		protected internal com.epl.geometry.AttributeStreamOfDbl m_segmentParams;

		protected internal int m_curveParamwritePoint;

		private int m_currentPathIndex;

		private int m_fill_rule = com.epl.geometry.Polygon.FillRule.enumFillRuleOddEven;

		internal static int[] _segmentParamSizes = new int[] { 0, 0, 6, 0, 8, 0 };

		// Contains starting points of the parts. The size is getPartCount() + 1.
		// First element is 0, last element is equal to the getPointCount().
		// same size as m_parts. Holds flags for each part (whether the part is
		// closed, etc. See PathFlags)
		// The segment flags. Size is getPointCount(). This is not a vertex
		// attribute, because we may want to use indexed access later (via an index
		// buffer).
		// Can be NULL if the MultiPathImpl contains straight lines only.
		// An index into the m_segmentParams stream. Size is getPointCount(). Can be
		// NULL if the MultiPathImpl contains straight lines only.
		// None, Line,
		// Bezier, XXX, Arc,
		// XXX;
		public bool HasNonLinearSegments()
		{
			return m_curveParamwritePoint > 0;
		}

		public MultiPathImpl(bool bPolygon)
		{
			// / Cpp ///
			// Reviewed vs. Native Jan 11, 2011
			m_bPolygon = bPolygon;
			m_bPathStarted = false;
			m_curveParamwritePoint = 0;
			m_cachedLength2D = 0;
			m_cachedArea2D = 0;
			m_pointCount = 0;
			m_description = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			m_cachedRingAreas2D = null;
			m_currentPathIndex = 0;
		}

		public MultiPathImpl(bool bPolygon, com.epl.geometry.VertexDescription description)
		{
			// Reviewed vs. Native Jan 11, 2011
			if (description == null)
			{
				throw new System.ArgumentException();
			}
			m_bPolygon = bPolygon;
			m_bPathStarted = false;
			m_curveParamwritePoint = 0;
			m_cachedLength2D = 0;
			m_cachedArea2D = 0;
			m_pointCount = 0;
			m_description = description;
			m_cachedRingAreas2D = null;
			m_currentPathIndex = 0;
		}

		// Reviewed vs. Native Jan 11, 2011
		protected internal void _initPathStartPoint()
		{
			_touch();
			if (m_moveToPoint == null)
			{
				m_moveToPoint = new com.epl.geometry.Point(m_description);
			}
			else
			{
				m_moveToPoint.AssignVertexDescription(m_description);
			}
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>Starts a new Path at the Point.</summary>
		public void StartPath(double x, double y)
		{
			com.epl.geometry.Point2D endPoint = new com.epl.geometry.Point2D();
			endPoint.x = x;
			endPoint.y = y;
			StartPath(endPoint);
		}

		// Reviewed vs. Native Jan 11, 2011
		public void StartPath(com.epl.geometry.Point2D point)
		{
			_initPathStartPoint();
			m_moveToPoint.SetXY(point);
			m_bPathStarted = true;
		}

		// Reviewed vs. Native Jan 11, 2011
		public void StartPath(com.epl.geometry.Point3D point)
		{
			_initPathStartPoint();
			m_moveToPoint.SetXYZ(point);
			AssignVertexDescription(m_moveToPoint.GetDescription());
			m_bPathStarted = true;
		}

		// Reviewed vs. Native Jan 11, 2011
		public void StartPath(com.epl.geometry.Point point)
		{
			if (point.IsEmpty())
			{
				throw new System.ArgumentException();
			}
			// throw new
			// IllegalArgumentException();
			MergeVertexDescription(point.GetDescription());
			_initPathStartPoint();
			point.CopyTo(m_moveToPoint);
			// TODO check MultiPathImpl.cpp comment
			// "//the description will be merged later"
			// assignVertexDescription(m_moveToPoint.getDescription());
			m_bPathStarted = true;
		}

		// Reviewed vs. Native Jan 11, 2011
		protected internal void _beforeNewSegment(int resizeBy)
		{
			// Called for each new segment being added.
			if (m_bPathStarted)
			{
				_initPathStartPoint();
				// make sure the m_movetoPoint exists and has
				// right vertex description
				// The new path is started. Need to grow m_parts and m_pathFlags.
				if (m_paths == null)
				{
					m_paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(2);
					m_paths.Write(0, 0);
					m_pathFlags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(2, unchecked((byte)0));
				}
				else
				{
					// _ASSERT(m_parts.size() >= 2);
					m_paths.Resize(m_paths.Size() + 1, 0);
					m_pathFlags.Resize(m_pathFlags.Size() + 1, 0);
				}
				if (m_bPolygon)
				{
					// Mark the path as closed
					m_pathFlags.Write(m_pathFlags.Size() - 2, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
				}
				resizeBy++;
			}
			// +1 for the StartPath point.
			int oldcount = m_pointCount;
			m_paths.Write(m_paths.Size() - 1, m_pointCount + resizeBy);
			// The
			// NotifyModified
			// will
			// update
			// the
			// m_pointCount
			// with this
			// value.
			_resizeImpl(oldcount + resizeBy);
			m_pathFlags.Write(m_paths.Size() - 1, unchecked((byte)0));
			if (m_bPathStarted)
			{
				SetPointByVal(oldcount, m_moveToPoint);
				// setPoint(oldcount,
				// m_moveToPoint); //finally
				// set the start point to
				// the geometry
				m_bPathStarted = false;
			}
		}

		// Reviewed vs. Native Jan 11, 2011
		protected internal void _finishLineTo()
		{
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>adds a Line Segment from the last Point to the given endPoint.</summary>
		public void LineTo(double x, double y)
		{
			_beforeNewSegment(1);
			SetXY(m_pointCount - 1, x, y);
			_finishLineTo();
		}

		// Point2D endPoint = new Point2D();
		// endPoint.x = x; endPoint.y = y;
		// lineTo(endPoint);
		// Reviewed vs. Native Jan 11, 2011
		public void LineTo(com.epl.geometry.Point2D endPoint)
		{
			_beforeNewSegment(1);
			SetXY(m_pointCount - 1, endPoint);
			_finishLineTo();
		}

		// Reviewed vs. Native Jan 11, 2011
		public void LineTo(com.epl.geometry.Point3D endPoint)
		{
			_beforeNewSegment(1);
			SetXYZ(m_pointCount - 1, endPoint);
			_finishLineTo();
		}

		// Reviewed vs. Native Jan 11, 2011
		public void LineTo(com.epl.geometry.Point endPoint)
		{
			_beforeNewSegment(1);
			SetPointByVal(m_pointCount - 1, endPoint);
			_finishLineTo();
		}

		// Reviewed vs. Native Jan 11, 2011
		protected internal void _initSegmentData(int sz)
		{
			if (m_segmentParamIndex == null)
			{
				m_segmentFlags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(m_pointCount, unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg));
				m_segmentParamIndex = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(m_pointCount, -1);
			}
			int size = m_curveParamwritePoint + sz;
			if (m_segmentParams == null)
			{
				m_segmentParams = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithPersistence(com.epl.geometry.VertexDescription.Persistence.enumDouble, size);
			}
			else
			{
				m_segmentParams.Resize(size, 0);
			}
		}

		// Reviewed vs. Native Jan 11, 2011
		protected internal void _finishBezierTo()
		{
			// _ASSERT(m_segmentFlags != null);
			// _ASSERT(m_segmentParamIndex != null);
			m_segmentFlags.Write(m_pointCount - 2, unchecked((byte)com.epl.geometry.SegmentFlags.enumBezierSeg));
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>adds a Cubic Bezier Segment to the current Path.</summary>
		/// <remarks>
		/// adds a Cubic Bezier Segment to the current Path. The Bezier Segment
		/// connects the current last Point and the given endPoint.
		/// </remarks>
		public void BezierTo(com.epl.geometry.Point2D controlPoint1, com.epl.geometry.Point2D controlPoint2, com.epl.geometry.Point2D endPoint)
		{
			_beforeNewSegment(1);
			SetXY(m_pointCount - 1, endPoint);
			double z;
			_initSegmentData(6);
			m_pathFlags.SetBits(m_pathFlags.Size() - 1, unchecked((byte)com.epl.geometry.PathFlags.enumHasNonlinearSegments));
			m_segmentParamIndex.Write(m_pointCount - 2, m_curveParamwritePoint);
			m_curveParamwritePoint += 6;
			int curveIndex = m_curveParamwritePoint;
			m_segmentParams.Write(curveIndex, controlPoint1.x);
			m_segmentParams.Write(curveIndex + 1, controlPoint1.y);
			z = 0;
			// TODO: calculate me.
			m_segmentParams.Write(curveIndex + 2, z);
			m_segmentParams.Write(curveIndex + 3, controlPoint2.x);
			m_segmentParams.Write(curveIndex + 4, controlPoint2.y);
			z = 0;
			// TODO: calculate me.
			m_segmentParams.Write(curveIndex + 5, z);
			_finishBezierTo();
		}

		// Reviewed vs. Native Jan 11, 2011
		public void OpenPath(int pathIndex)
		{
			_touch();
			if (m_bPolygon)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// do not call this
			// method on a
			// polygon
			int pathCount = GetPathCount();
			if (pathIndex > GetPathCount())
			{
				throw new System.ArgumentException();
			}
			if (m_pathFlags == null)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			m_pathFlags.ClearBits(pathIndex, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
		}

		public void OpenPathAndDuplicateStartVertex(int pathIndex)
		{
			_touch();
			if (m_bPolygon)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// do not call this
			// method on a
			// polygon
			int pathCount = GetPathCount();
			if (pathIndex > pathCount)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			if (!IsClosedPath(pathIndex))
			{
				return;
			}
			// do not open if open
			if (m_pathFlags == null)
			{
				// if (!m_pathFlags)
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			int oldPointCount = m_pointCount;
			int pathIndexStart = GetPathStart(pathIndex);
			int pathIndexEnd = GetPathEnd(pathIndex);
			_resizeImpl(m_pointCount + 1);
			// resize does not write into m_paths
			// anymore!
			_verifyAllStreams();
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				if (m_vertexAttributes[iattr] != null)
				{
					// if
					// (m_vertexAttributes[iattr])
					int semantics = m_description._getSemanticsImpl(iattr);
					int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					m_vertexAttributes[iattr].InsertRange(comp * pathIndexEnd, m_vertexAttributes[iattr], comp * pathIndexStart, comp, true, 1, comp * oldPointCount);
				}
			}
			for (int ipath = pathCount; ipath > pathIndex; ipath--)
			{
				int iend = m_paths.Read(ipath);
				m_paths.Write(ipath, iend + 1);
			}
			m_pathFlags.ClearBits(pathIndex, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
		}

		// Reviewed vs. Native Jan 11, 2011
		// Major Changes on 16th of January
		public void OpenAllPathsAndDuplicateStartVertex()
		{
			_touch();
			if (m_bPolygon)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// do not call this
			// method on a
			// polygon
			if (m_pathFlags == null)
			{
				// if (!m_pathFlags)
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			_verifyAllStreams();
			int closedPathCount = 0;
			int pathCount = GetPathCount();
			for (int i = 0; i < pathCount; i++)
			{
				if (m_pathFlags.Read(i) == unchecked((byte)com.epl.geometry.PathFlags.enumClosed))
				{
					closedPathCount++;
				}
			}
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				if (m_vertexAttributes[iattr] != null)
				{
					int semantics = m_description._getSemanticsImpl(iattr);
					// int
					// semantics
					// =
					// m_description._getSemanticsImpl(iattr);
					int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					int newSize = comp * (m_pointCount + closedPathCount);
					m_vertexAttributes[iattr].Resize(newSize);
					int offset = closedPathCount;
					int ipath = pathCount;
					for (int i_1 = m_pointCount - 1; i_1 >= 0; i_1--)
					{
						if (i_1 + 1 == m_paths.Read(ipath))
						{
							ipath--;
							if (m_pathFlags.Read(ipath) == unchecked((byte)com.epl.geometry.PathFlags.enumClosed))
							{
								int istart = m_paths.Read(ipath);
								for (int c = 0; c < comp; c++)
								{
									double v = m_vertexAttributes[iattr].ReadAsDbl(comp * istart + c);
									m_vertexAttributes[iattr].WriteAsDbl(comp * (offset + i_1) + c, v);
								}
								if (--offset == 0)
								{
									break;
								}
							}
						}
						for (int c_1 = 0; c_1 < comp; c_1++)
						{
							double v = m_vertexAttributes[iattr].ReadAsDbl(comp * i_1 + c_1);
							m_vertexAttributes[iattr].WriteAsDbl(comp * (offset + i_1) + c_1, v);
						}
					}
				}
			}
			int offset_1 = closedPathCount;
			for (int ipath_1 = pathCount; ipath_1 > 0; ipath_1--)
			{
				int iend = m_paths.Read(ipath_1);
				m_paths.Write(ipath_1, iend + offset_1);
				if (m_pathFlags.Read(ipath_1 - 1) == unchecked((byte)com.epl.geometry.PathFlags.enumClosed))
				{
					m_pathFlags.ClearBits(ipath_1 - 1, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
					if (--offset_1 == 0)
					{
						break;
					}
				}
			}
			m_pointCount += closedPathCount;
		}

		internal void ClosePathWithLine(int path_index)
		{
			// touch_();
			ThrowIfEmpty();
			byte pf = m_pathFlags.Read(path_index);
			m_pathFlags.Write(path_index, unchecked((byte)(pf | com.epl.geometry.PathFlags.enumClosed)));
			if (m_segmentFlags != null)
			{
				int vindex = GetPathEnd(path_index) - 1;
				m_segmentFlags.Write(vindex, unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg));
				m_segmentParamIndex.Write(vindex, -1);
			}
		}

		internal void ClosePathWithLine()
		{
			ThrowIfEmpty();
			m_bPathStarted = false;
			ClosePathWithLine(GetPathCount() - 1);
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>
		/// Closes all open curves by adding an implicit line segment from the end
		/// point to the start point.
		/// </summary>
		public void CloseAllPaths()
		{
			_touch();
			if (m_bPolygon || IsEmptyImpl())
			{
				return;
			}
			m_bPathStarted = false;
			for (int ipath = 0, npart = m_paths.Size() - 1; ipath < npart; ipath++)
			{
				if (IsClosedPath(ipath))
				{
					continue;
				}
				byte pf = m_pathFlags.Read(ipath);
				m_pathFlags.Write(ipath, unchecked((byte)(pf | com.epl.geometry.PathFlags.enumClosed)));
			}
		}

		// if (m_segmentFlags)
		// {
		// m_segmentFlags.write(m_pointCount - 1,
		// (byte)SegmentFlags.LineSeg));
		// m_segmentParamIndex.write(m_pointCount - 1, -1);
		// }
		// Reviewed vs. Native Jan 11, 2011
		/// <summary>Returns the size of the segment data for the given segment type.</summary>
		/// <param name="flag">is one of the segment flags from the SegmentFlags enum.</param>
		/// <returns>the size of the segment params as the number of doubles.</returns>
		public static int GetSegmentDataSize(byte flag)
		{
			return _segmentParamSizes[flag];
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>Closes last path of the MultiPathImpl with the Bezier Segment.</summary>
		/// <remarks>
		/// Closes last path of the MultiPathImpl with the Bezier Segment.
		/// The start point of the Bezier is the last point of the path and the last
		/// point of the bezier is the first point of the path.
		/// </remarks>
		public void ClosePathWithBezier(com.epl.geometry.Point2D controlPoint1, com.epl.geometry.Point2D controlPoint2)
		{
			_touch();
			if (IsEmptyImpl())
			{
				throw new com.epl.geometry.GeometryException("Invalid call. This operation cannot be performed on an empty geometry.");
			}
			m_bPathStarted = false;
			int pathIndex = m_paths.Size() - 2;
			byte pf = m_pathFlags.Read(pathIndex);
			m_pathFlags.Write(pathIndex, unchecked((byte)(pf | com.epl.geometry.PathFlags.enumClosed | com.epl.geometry.PathFlags.enumHasNonlinearSegments)));
			_initSegmentData(6);
			byte oldType = m_segmentFlags.Read(unchecked((byte)((m_pointCount - 1) & com.epl.geometry.SegmentFlags.enumSegmentMask)));
			m_segmentFlags.Write(m_pointCount - 1, unchecked((byte)(com.epl.geometry.SegmentFlags.enumBezierSeg)));
			int curveIndex = m_curveParamwritePoint;
			if (GetSegmentDataSize(oldType) < GetSegmentDataSize(unchecked((byte)com.epl.geometry.SegmentFlags.enumBezierSeg)))
			{
				m_segmentParamIndex.Write(m_pointCount - 1, m_curveParamwritePoint);
				m_curveParamwritePoint += 6;
			}
			else
			{
				// there was a closing bezier curve or an arc here. We can reuse the
				// storage.
				curveIndex = m_segmentParamIndex.Read(m_pointCount - 1);
			}
			double z;
			m_segmentParams.Write(curveIndex, controlPoint1.x);
			m_segmentParams.Write(curveIndex + 1, controlPoint1.y);
			z = 0;
			// TODO: calculate me.
			m_segmentParams.Write(curveIndex + 2, z);
			m_segmentParams.Write(curveIndex + 3, controlPoint2.x);
			m_segmentParams.Write(curveIndex + 4, controlPoint2.y);
			z = 0;
			// TODO: calculate me.
			m_segmentParams.Write(curveIndex + 5, z);
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>Returns True if the given path is closed (represents a Ring).</summary>
		public bool IsClosedPath(int ipath)
		{
			// Should we make a function called _UpdateClosedPathFlags and call it
			// here?
			return (unchecked((byte)(m_pathFlags.Read(ipath) & com.epl.geometry.PathFlags.enumClosed))) != 0;
		}

		public bool IsClosedPathInXYPlane(int path_index)
		{
			if (IsClosedPath(path_index))
			{
				return true;
			}
			int istart = GetPathStart(path_index);
			int iend = GetPathEnd(path_index) - 1;
			if (istart > iend)
			{
				return false;
			}
			com.epl.geometry.Point2D ptS = GetXY(istart);
			com.epl.geometry.Point2D ptE = GetXY(iend);
			return ptS.IsEqual(ptE);
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>Returns True if the given path might have non-linear segments.</summary>
		public bool HasNonLinearSegments(int ipath)
		{
			// Should we make a function called _UpdateHasNonLinearSegmentsFlags and
			// call it here?
			return (m_pathFlags.Read(ipath) & com.epl.geometry.PathFlags.enumHasNonlinearSegments) != 0;
		}

		// Reviewed vs. Native Jan 11, 2011
		public void AddSegment(com.epl.geometry.Segment segment, bool bStartNewPath)
		{
			MergeVertexDescription(segment.GetDescription());
			if (segment.GetType() == com.epl.geometry.Geometry.Type.Line)
			{
				com.epl.geometry.Point point = new com.epl.geometry.Point();
				if (bStartNewPath || IsEmpty())
				{
					segment.QueryStart(point);
					StartPath(point);
				}
				segment.QueryEnd(point);
				LineTo(point);
			}
			else
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
		}

		// Reviewed vs. Native Jan 11, 2011
		/// <summary>adds a rectangular closed Path to the MultiPathImpl.</summary>
		/// <param name="envSrc">is the source rectangle.</param>
		/// <param name="bReverse">Creates reversed path.</param>
		public void AddEnvelope(com.epl.geometry.Envelope2D envSrc, bool bReverse)
		{
			bool bWasEmpty = m_pointCount == 0;
			StartPath(envSrc.xmin, envSrc.ymin);
			if (bReverse)
			{
				LineTo(envSrc.xmax, envSrc.ymin);
				LineTo(envSrc.xmax, envSrc.ymax);
				LineTo(envSrc.xmin, envSrc.ymax);
			}
			else
			{
				LineTo(envSrc.xmin, envSrc.ymax);
				LineTo(envSrc.xmax, envSrc.ymax);
				LineTo(envSrc.xmax, envSrc.ymin);
			}
			ClosePathWithLine();
			m_bPathStarted = false;
			if (bWasEmpty && !bReverse)
			{
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIsEnvelope, false);
			}
		}

		// now we no(sic?)
		// the polypath
		// is envelope
		// Reviewed vs. Native Jan 11, 2011
		/// <summary>adds a rectangular closed Path to the MultiPathImpl.</summary>
		/// <param name="envSrc">is the source rectangle.</param>
		/// <param name="bReverse">Creates reversed path.</param>
		public void AddEnvelope(com.epl.geometry.Envelope envSrc, bool bReverse)
		{
			if (envSrc.IsEmpty())
			{
				return;
			}
			bool bWasEmpty = m_pointCount == 0;
			com.epl.geometry.Point pt = new com.epl.geometry.Point(m_description);
			// getDescription());
			for (int i = 0, n = 4; i < n; i++)
			{
				int j = bReverse ? n - i - 1 : i;
				envSrc.QueryCornerByVal(j, pt);
				if (i == 0)
				{
					StartPath(pt);
				}
				else
				{
					LineTo(pt);
				}
			}
			ClosePathWithLine();
			m_bPathStarted = false;
			if (bWasEmpty && !bReverse)
			{
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIsEnvelope, false);
			}
		}

		// now we know the
		// polypath is
		// envelope
		// Reviewed vs. Native Jan 11, 2011
		public void Add(com.epl.geometry.MultiPathImpl src, bool bReversePaths)
		{
			for (int i = 0; i < src.GetPathCount(); i++)
			{
				AddPath(src, i, !bReversePaths);
			}
		}

		public void AddPath(com.epl.geometry.MultiPathImpl src, int srcPathIndex, bool bForward)
		{
			InsertPath(-1, src, srcPathIndex, bForward);
		}

		// Reviewed vs. Native Jan 11, 2011 Significant changes to last for loop
		public void AddPath(com.epl.geometry.Point2D[] _points, int count, bool bForward)
		{
			InsertPath(-1, _points, 0, count, bForward);
		}

		public void AddSegmentsFromPath(com.epl.geometry.MultiPathImpl src, int src_path_index, int src_segment_from, int src_segment_count, bool b_start_new_path)
		{
			if (!b_start_new_path && GetPathCount() == 0)
			{
				b_start_new_path = true;
			}
			if (src_path_index < 0)
			{
				src_path_index = src.GetPathCount() - 1;
			}
			if (src_path_index >= src.GetPathCount() || src_segment_from < 0 || src_segment_count < 0 || src_segment_count > src.GetSegmentCount(src_path_index))
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			if (src_segment_count == 0)
			{
				return;
			}
			bool bIncludesClosingSegment = src.IsClosedPath(src_path_index) && src_segment_from + src_segment_count == src.GetSegmentCount(src_path_index);
			if (bIncludesClosingSegment && src_segment_count == 1)
			{
				return;
			}
			// cannot add a closing segment alone.
			m_bPathStarted = false;
			MergeVertexDescription(src.GetDescription());
			int src_point_count = src_segment_count;
			int srcFromPoint = src.GetPathStart(src_path_index) + src_segment_from + 1;
			if (b_start_new_path)
			{
				// adding a new path.
				src_point_count++;
				// add start point.
				srcFromPoint--;
			}
			if (bIncludesClosingSegment)
			{
				src_point_count--;
			}
			int oldPointCount = m_pointCount;
			_resizeImpl(m_pointCount + src_point_count);
			_verifyAllStreams();
			if (b_start_new_path)
			{
				if (src_point_count == 0)
				{
					return;
				}
				// happens when adding a single closing segment to the
				// new path
				m_paths.Add(m_pointCount);
				byte flags = src.m_pathFlags.Read(src_path_index);
				flags &= unchecked((byte)~/*TODO check this `~` operator usage!!!!*/com.epl.geometry.PathFlags.enumCalcMask);
				// remove calculated flags
				if (m_bPolygon)
				{
					flags |= unchecked((byte)com.epl.geometry.PathFlags.enumClosed);
				}
				m_pathFlags.Write(m_pathFlags.Size() - 1, flags);
				m_pathFlags.Add(unchecked((byte)0));
			}
			else
			{
				m_paths.Write(m_pathFlags.Size() - 1, m_pointCount);
			}
			// Index_type absoluteIndex = pathStart + before_point_index;
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description.GetSemantics(iattr);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				int isrcAttr = src.m_description.GetAttributeIndex(semantics);
				if (isrcAttr < 0 || src.m_vertexAttributes[isrcAttr] == null)
				{
					// The
					// source
					// does
					// not
					// have
					// the
					// attribute.
					// insert
					// default
					// value
					double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					m_vertexAttributes[iattr].InsertRange(comp * oldPointCount, v, src_point_count * comp, comp * oldPointCount);
					continue;
				}
				// add vertices to the given stream
				bool b_forward = true;
				m_vertexAttributes[iattr].InsertRange(comp * oldPointCount, src.m_vertexAttributes[isrcAttr], comp * srcFromPoint, src_point_count * comp, b_forward, comp, comp * oldPointCount);
			}
			if (HasNonLinearSegments())
			{
				// TODO: implement me. For example as a while loop over all curves.
				// Replace, calling ReplaceSegment
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// m_segment_flags->write_range((get_path_start(path_index) +
			// before_point_index + src_point_count), (oldPointCount -
			// get_path_start(path_index) - before_point_index),
			// m_segment_flags, (get_path_start(path_index) +
			// before_point_index), true, 1);
			// m_segment_param_index->write_range((get_path_start(path_index) +
			// before_point_index + src_point_count), (oldPointCount -
			// get_path_start(path_index) - before_point_index),
			// m_segment_param_index, (get_path_start(path_index) +
			// before_point_index), true, 1);
			// for (Index_type i = get_path_start(path_index) +
			// before_point_index, n = get_path_start(path_index) +
			// before_point_index + src_point_count; i < n; i++)
			// {
			// m_segment_flags->write(i, (int8_t)enum_value1(Segment_flags,
			// enum_line_seg));
			// m_segment_param_index->write(i, -1);
			// }
			if (src.HasNonLinearSegments(src_path_index))
			{
				// TODO: implement me. For example as a while loop over all curves.
				// Replace, calling ReplaceSegment
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		// Reviewed vs. Native Jan 11, 2011
		public void ReverseAllPaths()
		{
			for (int i = 0, n = GetPathCount(); i < n; i++)
			{
				ReversePath(i);
			}
		}

		// Reviewed vs. Native Jan 11, 2011
		public void ReversePath(int pathIndex)
		{
			_verifyAllStreams();
			int pathCount = GetPathCount();
			if (pathIndex >= pathCount)
			{
				throw new System.ArgumentException();
			}
			int reversedPathStart = GetPathStart(pathIndex);
			int reversedPathSize = GetPathSize(pathIndex);
			int offset = IsClosedPath(pathIndex) ? 1 : 0;
			// TODO: a bug for the non linear segments here.
			// There could be an issue here if someone explicity closes the path
			// with the same start/end point.
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				if (m_vertexAttributes[iattr] != null)
				{
					int semantics = m_description._getSemanticsImpl(iattr);
					int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					m_vertexAttributes[iattr].ReverseRange(comp * (reversedPathStart + offset), comp * (reversedPathSize - offset), comp);
				}
			}
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		// Reviewed vs. Native Jan 11, 2011
		// TODO: Nonlinearsegments
		public void RemovePath(int pathIndex)
		{
			_verifyAllStreams();
			int pathCount = GetPathCount();
			if (pathIndex < 0)
			{
				pathIndex = pathCount - 1;
			}
			if (pathIndex >= pathCount)
			{
				throw new System.ArgumentException();
			}
			bool bDirtyRingAreas2D = _hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D);
			int removedPathStart = GetPathStart(pathIndex);
			int removedPathSize = GetPathSize(pathIndex);
			// Remove the attribute values for the path
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				if (m_vertexAttributes[iattr] != null)
				{
					int semantics = m_description._getSemanticsImpl(iattr);
					int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					m_vertexAttributes[iattr].EraseRange(comp * removedPathStart, comp * removedPathSize, comp * m_pointCount);
				}
			}
			// Change the start of each path after the removed path
			for (int i = pathIndex + 1; i <= pathCount; i++)
			{
				int istart = m_paths.Read(i);
				m_paths.Write(i - 1, istart - removedPathSize);
			}
			if (m_pathFlags == null)
			{
				for (int i_1 = pathIndex + 1; i_1 <= pathCount; i_1++)
				{
					byte flags = m_pathFlags.Read(i_1);
					m_pathFlags.Write(i_1 - 1, flags);
				}
			}
			m_paths.Resize(pathCount);
			m_pathFlags.Resize(pathCount);
			m_pointCount -= removedPathSize;
			m_reservedPointCount -= removedPathSize;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		// TODO: Nonlinearsegments
		public void InsertPath(int pathIndex, com.epl.geometry.MultiPathImpl src, int srcPathIndex, bool bForward)
		{
			if (src == this)
			{
				throw new System.ArgumentException();
			}
			if (srcPathIndex >= src.GetPathCount())
			{
				throw new System.ArgumentException();
			}
			int oldPathCount = GetPathCount();
			if (pathIndex > oldPathCount)
			{
				throw new System.ArgumentException();
			}
			if (pathIndex < 0)
			{
				pathIndex = oldPathCount;
			}
			if (srcPathIndex < 0)
			{
				srcPathIndex = src.GetPathCount() - 1;
			}
			m_bPathStarted = false;
			MergeVertexDescription(src.m_description);
			// merge attributes from the
			// source
			src._verifyAllStreams();
			// the source need to be correct.
			int srcPathIndexStart = src.GetPathStart(srcPathIndex);
			int srcPathSize = src.GetPathSize(srcPathIndex);
			int oldPointCount = m_pointCount;
			int offset = src.IsClosedPath(srcPathIndex) && !bForward ? 1 : 0;
			_resizeImpl(m_pointCount + srcPathSize);
			_verifyAllStreams();
			int pathIndexStart = pathIndex < oldPathCount ? GetPathStart(pathIndex) : oldPointCount;
			// Copy all attribute values.
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int isrcAttr = src.m_description.GetAttributeIndex(semantics);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				if (isrcAttr >= 0 && src.m_vertexAttributes[isrcAttr] != null)
				{
					if (offset != 0)
					{
						m_vertexAttributes[iattr].InsertRange(pathIndexStart * comp, src.m_vertexAttributes[isrcAttr], comp * srcPathIndexStart, comp, true, comp, comp * oldPointCount);
					}
					m_vertexAttributes[iattr].InsertRange((pathIndexStart + offset) * comp, src.m_vertexAttributes[isrcAttr], comp * (srcPathIndexStart + offset), comp * (srcPathSize - offset), bForward, comp, comp * (oldPointCount + offset));
				}
				else
				{
					// Need to make room for the attributes, so we copy default
					// values in
					double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					m_vertexAttributes[iattr].InsertRange(pathIndexStart * comp, v, comp * srcPathSize, comp * oldPointCount);
				}
			}
			int newPointCount = oldPointCount + srcPathSize;
			m_paths.Add(newPointCount);
			for (int ipath = oldPathCount; ipath >= pathIndex + 1; ipath--)
			{
				int iend = m_paths.Read(ipath - 1);
				m_paths.Write(ipath, iend + srcPathSize);
			}
			// ========================== todo: NonLinearSegments =================
			if (src.HasNonLinearSegments(srcPathIndex))
			{
			}
			m_pathFlags.Add(unchecked((byte)0));
			// _ASSERT(m_pathFlags.size() == m_paths.size());
			for (int ipath_1 = oldPathCount - 1; ipath_1 >= pathIndex + 1; ipath_1--)
			{
				byte flags = m_pathFlags.Read(ipath_1);
				flags &= unchecked((byte)~/*TODO check this `~` operator usage!!!!*/com.epl.geometry.PathFlags.enumCalcMask);
				// remove calculated flags
				m_pathFlags.Write(ipath_1 + 1, flags);
			}
			com.epl.geometry.AttributeStreamOfInt8 srcPathFlags = src.GetPathFlagsStreamRef();
			byte flags_1 = srcPathFlags.Read(srcPathIndex);
			flags_1 &= unchecked((byte)~/*TODO check this `~` operator usage!!!!*/com.epl.geometry.PathFlags.enumCalcMask);
			// remove calculated flags
			if (m_bPolygon)
			{
				flags_1 |= unchecked((byte)com.epl.geometry.PathFlags.enumClosed);
			}
			m_pathFlags.Write(pathIndex, flags_1);
		}

		public void InsertPath(int pathIndex, com.epl.geometry.Point2D[] points, int pointsOffset, int count, bool bForward)
		{
			int oldPathCount = GetPathCount();
			if (pathIndex > oldPathCount)
			{
				throw new System.ArgumentException();
			}
			if (pathIndex < 0)
			{
				pathIndex = oldPathCount;
			}
			m_bPathStarted = false;
			int oldPointCount = m_pointCount;
			// Copy all attribute values.
			if (points != null)
			{
				_resizeImpl(m_pointCount + count);
				_verifyAllStreams();
				int pathStart = pathIndex < oldPathCount ? GetPathStart(pathIndex) : oldPointCount;
				for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
				{
					int semantics = m_description._getSemanticsImpl(iattr);
					if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
					{
						// copy range to make place for new vertices
						m_vertexAttributes[iattr].WriteRange(2 * (pathStart + count), 2 * (oldPointCount - pathIndex), m_vertexAttributes[iattr], 2 * pathStart, true, 2);
						com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase)GetAttributeStreamRef(semantics);
						int j = pathStart;
						for (int i = 0; i < count; i++, j++)
						{
							int index = (bForward ? pointsOffset + i : pointsOffset + count - i - 1);
							position.Write(2 * j, points[index].x);
							position.Write(2 * j + 1, points[index].y);
						}
					}
					else
					{
						// Need to make room for the attributes, so we copy default
						// values in
						int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
						m_vertexAttributes[iattr].InsertRange(pathStart * comp, v, comp * count, comp * oldPointCount);
					}
				}
			}
			else
			{
				_verifyAllStreams();
			}
			m_paths.Add(m_pointCount);
			for (int ipath = oldPathCount; ipath >= pathIndex + 1; ipath--)
			{
				int iend = m_paths.Read(ipath - 1);
				m_paths.Write(ipath, iend + count);
			}
			m_pathFlags.Add(unchecked((byte)0));
			// _ASSERT(m_pathFlags.size() == m_paths.size());
			for (int ipath_1 = oldPathCount - 1; ipath_1 >= pathIndex + 1; ipath_1--)
			{
				byte flags = m_pathFlags.Read(ipath_1);
				flags &= unchecked((byte)~/*TODO check this `~` operator usage!!!!*/com.epl.geometry.PathFlags.enumCalcMask);
				// remove calculated flags
				m_pathFlags.Write(ipath_1 + 1, flags);
			}
			if (m_bPolygon)
			{
				m_pathFlags.Write(pathIndex, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
			}
		}

		public void InsertPoints(int pathIndex, int beforePointIndex, com.epl.geometry.MultiPathImpl src, int srcPathIndex, int srcPointIndexFrom, int srcPointCount, bool bForward)
		{
			if (pathIndex < 0)
			{
				pathIndex = GetPathCount();
			}
			if (srcPathIndex < 0)
			{
				srcPathIndex = src.GetPathCount() - 1;
			}
			if (pathIndex > GetPathCount() || beforePointIndex >= 0 && beforePointIndex > GetPathSize(pathIndex) || srcPathIndex >= src.GetPathCount() || srcPointCount > src.GetPathSize(srcPathIndex))
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			if (srcPointCount == 0)
			{
				return;
			}
			MergeVertexDescription(src.m_description);
			if (pathIndex == GetPathCount())
			{
				// adding a new path.
				m_paths.Add(m_pointCount);
				byte flags = src.m_pathFlags.Read(srcPathIndex);
				flags &= unchecked((byte)~/*TODO check this `~` operator usage!!!!*/com.epl.geometry.PathFlags.enumCalcMask);
				// remove calculated flags
				if (!m_bPolygon)
				{
					m_pathFlags.Add(flags);
				}
				else
				{
					m_pathFlags.Add(unchecked((byte)(flags | com.epl.geometry.PathFlags.enumClosed)));
				}
			}
			if (beforePointIndex < 0)
			{
				beforePointIndex = GetPathSize(pathIndex);
			}
			int oldPointCount = m_pointCount;
			_resizeImpl(m_pointCount + srcPointCount);
			_verifyAllStreams();
			src._verifyAllStreams();
			int pathStart = GetPathStart(pathIndex);
			int absoluteIndex = pathStart + beforePointIndex;
			if (srcPointCount < 0)
			{
				srcPointCount = src.GetPathSize(srcPathIndex);
			}
			int srcPathStart = src.GetPathStart(srcPathIndex);
			int srcAbsoluteIndex = srcPathStart + srcPointCount;
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				int isrcAttr = src.m_description.GetAttributeIndex(semantics);
				if (isrcAttr < 0 || src.m_vertexAttributes[isrcAttr] == null)
				{
					// The
					// source
					// does
					// not
					// have
					// the
					// attribute.
					double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					m_vertexAttributes[iattr].InsertRange(comp * absoluteIndex, v, srcAbsoluteIndex * comp, comp * oldPointCount);
					continue;
				}
				// add vertices to the given stream
				m_vertexAttributes[iattr].InsertRange(comp * (pathStart + beforePointIndex), src.m_vertexAttributes[isrcAttr], comp * (srcPathStart + srcPointIndexFrom), srcPointCount * comp, bForward, comp, comp * oldPointCount);
			}
			if (HasNonLinearSegments())
			{
				// TODO: probably a bug here when a new
				// path is added.
				m_segmentFlags.WriteRange((GetPathStart(pathIndex) + beforePointIndex + srcPointCount), (oldPointCount - GetPathStart(pathIndex) - beforePointIndex), m_segmentFlags, (GetPathStart(pathIndex) + beforePointIndex), true, 1);
				m_segmentParamIndex.WriteRange((GetPathStart(pathIndex) + beforePointIndex + srcPointCount), (oldPointCount - GetPathStart(pathIndex) - beforePointIndex), m_segmentParamIndex, (GetPathStart(pathIndex) + beforePointIndex), true, 1);
				for (int i = GetPathStart(pathIndex) + beforePointIndex, n = GetPathStart(pathIndex) + beforePointIndex + srcPointCount; i < n; i++)
				{
					m_segmentFlags.Write(i, unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg));
					m_segmentParamIndex.Write(i, -1);
				}
			}
			if (src.HasNonLinearSegments(srcPathIndex))
			{
				// TODO: implement me. For example as a while loop over all curves.
				// Replace, calling ReplaceSegment
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			for (int ipath = pathIndex + 1, npaths = GetPathCount(); ipath <= npaths; ipath++)
			{
				int num = m_paths.Read(ipath);
				m_paths.Write(ipath, num + srcPointCount);
			}
		}

		public void InsertPoints(int pathIndex, int beforePointIndex, com.epl.geometry.Point2D[] src, int srcPointIndexFrom, int srcPointCount, bool bForward)
		{
			if (pathIndex < 0)
			{
				pathIndex = GetPathCount();
			}
			if (pathIndex > GetPathCount() || beforePointIndex > GetPathSize(pathIndex) || srcPointIndexFrom < 0 || srcPointCount > src.Length)
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			if (srcPointCount == 0)
			{
				return;
			}
			if (pathIndex == GetPathCount())
			{
				// adding a new path.
				m_paths.Add(m_pointCount);
				if (!m_bPolygon)
				{
					m_pathFlags.Add(unchecked((byte)0));
				}
				else
				{
					m_pathFlags.Add(unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
				}
			}
			if (beforePointIndex < 0)
			{
				beforePointIndex = GetPathSize(pathIndex);
			}
			_verifyAllStreams();
			int oldPointCount = m_pointCount;
			_resizeImpl(m_pointCount + srcPointCount);
			_verifyAllStreams();
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				// copy range to make place for new vertices
				m_vertexAttributes[iattr].WriteRange(comp * (GetPathStart(pathIndex) + beforePointIndex + srcPointCount), (oldPointCount - GetPathStart(pathIndex) - beforePointIndex) * comp, m_vertexAttributes[iattr], comp * (GetPathStart(pathIndex) + beforePointIndex), true, comp);
				if (iattr == 0)
				{
					// add vertices to the given stream
					((com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase)m_vertexAttributes[iattr]).WriteRange(comp * (GetPathStart(pathIndex) + beforePointIndex), srcPointCount, src, srcPointIndexFrom, bForward);
				}
				else
				{
					double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					m_vertexAttributes[iattr].SetRange(v, (GetPathStart(pathIndex) + beforePointIndex) * comp, srcPointCount * comp);
				}
			}
			if (HasNonLinearSegments())
			{
				m_segmentFlags.WriteRange((GetPathStart(pathIndex) + beforePointIndex + srcPointCount), (oldPointCount - GetPathStart(pathIndex) - beforePointIndex), m_segmentFlags, (GetPathStart(pathIndex) + beforePointIndex), true, 1);
				m_segmentParamIndex.WriteRange((GetPathStart(pathIndex) + beforePointIndex + srcPointCount), (oldPointCount - GetPathStart(pathIndex) - beforePointIndex), m_segmentParamIndex, (GetPathStart(pathIndex) + beforePointIndex), true, 1);
				m_segmentFlags.SetRange(unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg), GetPathStart(pathIndex) + beforePointIndex, srcPointCount);
				m_segmentParamIndex.SetRange(-1, GetPathStart(pathIndex) + beforePointIndex, srcPointCount);
			}
			for (int ipath = pathIndex + 1, npaths = GetPathCount(); ipath <= npaths; ipath++)
			{
				m_paths.Write(ipath, m_paths.Read(ipath) + srcPointCount);
			}
		}

		public void InsertPoint(int pathIndex, int beforePointIndex, com.epl.geometry.Point2D pt)
		{
			int pathCount = GetPathCount();
			if (pathIndex < 0)
			{
				pathIndex = GetPathCount();
			}
			if (pathIndex >= pathCount || beforePointIndex > GetPathSize(pathIndex))
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			if (pathIndex == GetPathCount())
			{
				// adding a new path.
				m_paths.Add(m_pointCount);
				if (!m_bPolygon)
				{
					m_pathFlags.Add(unchecked((byte)0));
				}
				else
				{
					m_pathFlags.Add(unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
				}
			}
			if (beforePointIndex < 0)
			{
				beforePointIndex = GetPathSize(pathIndex);
			}
			int oldPointCount = m_pointCount;
			_resizeImpl(m_pointCount + 1);
			_verifyAllStreams();
			int pathStart = GetPathStart(pathIndex);
			((com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase)m_vertexAttributes[0]).Insert(2 * (pathStart + beforePointIndex), pt, 2 * oldPointCount);
			for (int iattr = 1, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				// Need to make room for the attribute, so we copy a default value
				// in
				double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
				m_vertexAttributes[iattr].InsertRange(comp * (pathStart + beforePointIndex), v, comp, comp * oldPointCount);
			}
			for (int ipath = pathIndex + 1, npaths = pathCount; ipath <= npaths; ipath++)
			{
				m_paths.Write(ipath, m_paths.Read(ipath) + 1);
			}
		}

		public void InsertPoint(int pathIndex, int beforePointIndex, com.epl.geometry.Point pt)
		{
			int pathCount = GetPathCount();
			if (pathIndex < 0)
			{
				pathIndex = GetPathCount();
			}
			if (pathIndex >= pathCount || beforePointIndex > GetPathSize(pathIndex))
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			if (pathIndex == GetPathCount())
			{
				// adding a new path.
				m_paths.Add(m_pointCount);
				if (!m_bPolygon)
				{
					m_pathFlags.Add(unchecked((byte)0));
				}
				else
				{
					m_pathFlags.Add(unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
				}
			}
			if (beforePointIndex < 0)
			{
				beforePointIndex = GetPathSize(pathIndex);
			}
			MergeVertexDescription(pt.GetDescription());
			int oldPointCount = m_pointCount;
			_resizeImpl(m_pointCount + 1);
			_verifyAllStreams();
			int pathStart = GetPathStart(pathIndex);
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				int semantics = m_description._getSemanticsImpl(iattr);
				int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				if (pt.HasAttribute(semantics))
				{
					m_vertexAttributes[iattr].InsertAttributes(comp * (pathStart + beforePointIndex), pt, semantics, comp * oldPointCount);
				}
				else
				{
					// Need to make room for the attribute, so we copy a default
					// value in
					double v = com.epl.geometry.VertexDescription.GetDefaultValue(semantics);
					m_vertexAttributes[iattr].InsertRange(comp * (pathStart + beforePointIndex), v, comp, comp * oldPointCount);
				}
			}
			for (int ipath = pathIndex + 1, npaths = pathCount; ipath <= npaths; ipath++)
			{
				m_paths.Write(ipath, m_paths.Read(ipath) + 1);
			}
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		public void RemovePoint(int pathIndex, int pointIndex)
		{
			int pathCount = GetPathCount();
			if (pathIndex < 0)
			{
				pathIndex = pathCount - 1;
			}
			if (pathIndex >= pathCount || pointIndex >= GetPathSize(pathIndex))
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			_verifyAllStreams();
			int pathStart = GetPathStart(pathIndex);
			if (pointIndex < 0)
			{
				pointIndex = GetPathSize(pathIndex) - 1;
			}
			int absoluteIndex = pathStart + pointIndex;
			// Remove the attribute values for the path
			for (int iattr = 0, nattr = m_description.GetAttributeCount(); iattr < nattr; iattr++)
			{
				if (m_vertexAttributes[iattr] != null)
				{
					int semantics = m_description._getSemanticsImpl(iattr);
					int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					m_vertexAttributes[iattr].EraseRange(comp * absoluteIndex, comp, comp * m_pointCount);
				}
			}
			for (int ipath = pathCount; ipath >= pathIndex + 1; ipath--)
			{
				int iend = m_paths.Read(ipath);
				m_paths.Write(ipath, iend - 1);
			}
			m_pointCount--;
			m_reservedPointCount--;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		public double CalculatePathLength2D(int pathIndex)
		{
			/* const */
			com.epl.geometry.SegmentIteratorImpl segIter = QuerySegmentIteratorAtVertex(GetPathStart(pathIndex));
			com.epl.geometry.MathUtils.KahanSummator len = new com.epl.geometry.MathUtils.KahanSummator(0);
			while (segIter.HasNextSegment())
			{
				len.Add(segIter.NextSegment().CalculateLength2D());
			}
			return len.GetResult();
		}

		internal double CalculateSubLength2D(int from_path_index, int from_point_index, int to_path_index, int to_point_index)
		{
			int absolute_from_index = GetPathStart(from_path_index) + from_point_index;
			int absolute_to_index = GetPathStart(to_path_index) + to_point_index;
			if (absolute_to_index < absolute_from_index || absolute_from_index < 0 || absolute_to_index > GetPointCount() - 1)
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.SegmentIteratorImpl seg_iter = QuerySegmentIterator();
			double sub_length = 0.0;
			seg_iter.ResetToVertex(absolute_from_index);
			do
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					if (seg_iter.GetStartPointIndex() == absolute_to_index)
					{
						break;
					}
					double segment_length = segment.CalculateLength2D();
					sub_length += segment_length;
				}
				if (seg_iter.GetStartPointIndex() == absolute_to_index)
				{
					break;
				}
			}
			while (seg_iter.NextPath());
			return sub_length;
		}

		internal double CalculateSubLength2D(int path_index, int from_point_index, int to_point_index)
		{
			int absolute_from_index = GetPathStart(path_index) + from_point_index;
			int absolute_to_index = GetPathStart(path_index) + to_point_index;
			if (absolute_from_index < 0 || absolute_to_index > GetPointCount() - 1)
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.SegmentIteratorImpl seg_iter = QuerySegmentIterator();
			if (absolute_from_index > absolute_to_index)
			{
				if (!IsClosedPath(path_index))
				{
					throw new System.ArgumentException("cannot iterate across an open path");
				}
				seg_iter.SetCirculator(true);
			}
			double prev_length = 0.0;
			double sub_length = 0.0;
			seg_iter.ResetToVertex(absolute_from_index);
			do
			{
				System.Diagnostics.Debug.Assert((seg_iter.HasNextSegment()));
				sub_length += prev_length;
				com.epl.geometry.Segment segment = seg_iter.NextSegment();
				prev_length = segment.CalculateLength2D();
			}
			while (seg_iter.GetStartPointIndex() != absolute_to_index);
			return sub_length;
		}

		public override com.epl.geometry.Geometry GetBoundary()
		{
			return com.epl.geometry.Boundary.Calculate(this, null);
		}

		// TODO: Add code fore interpolation type (none and angular)
		internal void InterpolateAttributes(int from_path_index, int from_point_index, int to_path_index, int to_point_index)
		{
			for (int ipath = from_path_index; ipath < to_path_index - 1; ipath++)
			{
				if (IsClosedPath(ipath))
				{
					throw new System.ArgumentException("cannot interpolate across closed paths");
				}
			}
			int nattr = m_description.GetAttributeCount();
			if (nattr == 1)
			{
				return;
			}
			// only has position
			double sub_length = CalculateSubLength2D(from_path_index, from_point_index, to_path_index, to_point_index);
			if (sub_length == 0.0)
			{
				return;
			}
			for (int iattr = 1; iattr < nattr; iattr++)
			{
				int semantics = m_description.GetSemantics(iattr);
				int interpolation = com.epl.geometry.VertexDescription.GetInterpolation(semantics);
				if (interpolation == com.epl.geometry.VertexDescription.Interpolation.ANGULAR)
				{
					continue;
				}
				int components = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int ordinate = 0; ordinate < components; ordinate++)
				{
					InterpolateAttributes_(semantics, from_path_index, from_point_index, to_path_index, to_point_index, sub_length, ordinate);
				}
			}
		}

		// TODO: Add code for interpolation type (none and angular)
		internal void InterpolateAttributesForSemantics(int semantics, int from_path_index, int from_point_index, int to_path_index, int to_point_index)
		{
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				return;
			}
			if (!HasAttribute(semantics))
			{
				throw new System.ArgumentException("does not have the given attribute");
			}
			int interpolation = com.epl.geometry.VertexDescription.GetInterpolation(semantics);
			if (interpolation == com.epl.geometry.VertexDescription.Interpolation.ANGULAR)
			{
				throw new System.ArgumentException("not implemented for the given semantics");
			}
			for (int ipath = from_path_index; ipath < to_path_index - 1; ipath++)
			{
				if (IsClosedPath(ipath))
				{
					throw new System.ArgumentException("cannot interpolate across closed paths");
				}
			}
			double sub_length = CalculateSubLength2D(from_path_index, from_point_index, to_path_index, to_point_index);
			if (sub_length == 0.0)
			{
				return;
			}
			int components = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			for (int ordinate = 0; ordinate < components; ordinate++)
			{
				InterpolateAttributes_(semantics, from_path_index, from_point_index, to_path_index, to_point_index, sub_length, ordinate);
			}
		}

		internal void InterpolateAttributes(int path_index, int from_point_index, int to_point_index)
		{
			int nattr = m_description.GetAttributeCount();
			if (nattr == 1)
			{
				return;
			}
			// only has position
			double sub_length = CalculateSubLength2D(path_index, from_point_index, to_point_index);
			if (sub_length == 0.0)
			{
				return;
			}
			for (int iattr = 1; iattr < nattr; iattr++)
			{
				int semantics = m_description.GetSemantics(iattr);
				int interpolation = com.epl.geometry.VertexDescription.GetInterpolation(semantics);
				if (interpolation == com.epl.geometry.VertexDescription.Interpolation.ANGULAR)
				{
					continue;
				}
				int components = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int ordinate = 0; ordinate < components; ordinate++)
				{
					InterpolateAttributes_(semantics, path_index, from_point_index, to_point_index, sub_length, ordinate);
				}
			}
		}

		internal void InterpolateAttributesForSemantics(int semantics, int path_index, int from_point_index, int to_point_index)
		{
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				return;
			}
			if (!HasAttribute(semantics))
			{
				throw new System.ArgumentException("does not have the given attribute");
			}
			int interpolation = com.epl.geometry.VertexDescription.GetInterpolation(semantics);
			if (interpolation == com.epl.geometry.VertexDescription.Interpolation.ANGULAR)
			{
				throw new System.ArgumentException("not implemented for the given semantics");
			}
			double sub_length = CalculateSubLength2D(path_index, from_point_index, to_point_index);
			if (sub_length == 0.0)
			{
				return;
			}
			int components = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			for (int ordinate = 0; ordinate < components; ordinate++)
			{
				InterpolateAttributes_(semantics, path_index, from_point_index, to_point_index, sub_length, ordinate);
			}
		}

		// TODO: Add code fore interpolation type (none and angular)
		internal void InterpolateAttributes_(int semantics, int from_path_index, int from_point_index, int to_path_index, int to_point_index, double sub_length, int ordinate)
		{
			com.epl.geometry.SegmentIteratorImpl seg_iter = QuerySegmentIterator();
			int absolute_from_index = GetPathStart(from_path_index) + from_point_index;
			int absolute_to_index = GetPathStart(to_path_index) + to_point_index;
			double from_attribute = GetAttributeAsDbl(semantics, absolute_from_index, ordinate);
			double to_attribute = GetAttributeAsDbl(semantics, absolute_to_index, ordinate);
			double interpolated_attribute = from_attribute;
			double cumulative_length = 0.0;
			seg_iter.ResetToVertex(absolute_from_index);
			do
			{
				if (seg_iter.HasNextSegment())
				{
					seg_iter.NextSegment();
					if (seg_iter.GetStartPointIndex() == absolute_to_index)
					{
						return;
					}
					SetAttribute(semantics, seg_iter.GetStartPointIndex(), ordinate, interpolated_attribute);
					seg_iter.PreviousSegment();
					do
					{
						com.epl.geometry.Segment segment = seg_iter.NextSegment();
						if (seg_iter.GetEndPointIndex() == absolute_to_index)
						{
							return;
						}
						double segment_length = segment.CalculateLength2D();
						cumulative_length += segment_length;
						double t = cumulative_length / sub_length;
						interpolated_attribute = com.epl.geometry.MathUtils.Lerp(from_attribute, to_attribute, t);
						if (!seg_iter.IsClosingSegment())
						{
							SetAttribute(semantics, seg_iter.GetEndPointIndex(), ordinate, interpolated_attribute);
						}
					}
					while (seg_iter.HasNextSegment());
				}
			}
			while (seg_iter.NextPath());
		}

		internal void InterpolateAttributes_(int semantics, int path_index, int from_point_index, int to_point_index, double sub_length, int ordinate)
		{
			System.Diagnostics.Debug.Assert((m_bPolygon));
			com.epl.geometry.SegmentIteratorImpl seg_iter = QuerySegmentIterator();
			int absolute_from_index = GetPathStart(path_index) + from_point_index;
			int absolute_to_index = GetPathStart(path_index) + to_point_index;
			if (absolute_to_index == absolute_from_index)
			{
				return;
			}
			double from_attribute = GetAttributeAsDbl(semantics, absolute_from_index, ordinate);
			double to_attribute = GetAttributeAsDbl(semantics, absolute_to_index, ordinate);
			double cumulative_length = 0.0;
			seg_iter.ResetToVertex(absolute_from_index);
			seg_iter.SetCirculator(true);
			double prev_interpolated_attribute = from_attribute;
			do
			{
				com.epl.geometry.Segment segment = seg_iter.NextSegment();
				SetAttribute(semantics, seg_iter.GetStartPointIndex(), ordinate, prev_interpolated_attribute);
				double segment_length = segment.CalculateLength2D();
				cumulative_length += segment_length;
				double t = cumulative_length / sub_length;
				prev_interpolated_attribute = com.epl.geometry.MathUtils.Lerp(from_attribute, to_attribute, t);
			}
			while (seg_iter.GetEndPointIndex() != absolute_to_index);
		}

		public override void SetEmpty()
		{
			m_curveParamwritePoint = 0;
			m_bPathStarted = false;
			m_paths = null;
			m_pathFlags = null;
			m_segmentParamIndex = null;
			m_segmentFlags = null;
			m_segmentParams = null;
			_setEmptyImpl();
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			ApplyTransformation(transform, -1);
		}

		public void ApplyTransformation(com.epl.geometry.Transformation2D transform, int pathIndex)
		{
			if (IsEmpty())
			{
				return;
			}
			if (transform.IsIdentity())
			{
				return;
			}
			_verifyAllStreams();
			com.epl.geometry.AttributeStreamOfDbl points = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.Point2D ptStart = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptControl = new com.epl.geometry.Point2D();
			bool bHasNonLinear;
			int fistIdx;
			int lastIdx;
			if (pathIndex < 0)
			{
				bHasNonLinear = HasNonLinearSegments();
				fistIdx = 0;
				lastIdx = m_pointCount;
			}
			else
			{
				bHasNonLinear = HasNonLinearSegments(pathIndex);
				fistIdx = GetPathStart(pathIndex);
				lastIdx = GetPathEnd(pathIndex);
			}
			for (int ipoint = fistIdx; ipoint < lastIdx; ipoint++)
			{
				ptStart.x = points.Read(ipoint * 2);
				ptStart.y = points.Read(ipoint * 2 + 1);
				if (bHasNonLinear)
				{
					int segIndex = m_segmentParamIndex.Read(ipoint);
					if (segIndex >= 0)
					{
						int segmentType = (int)m_segmentFlags.Read(ipoint);
						int type = segmentType & com.epl.geometry.SegmentFlags.enumSegmentMask;
						switch (type)
						{
							case com.epl.geometry.SegmentFlags.enumBezierSeg:
							{
								ptControl.x = m_segmentParams.Read(segIndex);
								ptControl.y = m_segmentParams.Read(segIndex + 1);
								transform.Transform(ptControl, ptControl);
								m_segmentParams.Write(segIndex, ptControl.x);
								m_segmentParams.Write(segIndex + 1, ptControl.y);
								ptControl.x = m_segmentParams.Read(segIndex + 3);
								ptControl.y = m_segmentParams.Read(segIndex + 4);
								transform.Transform(ptControl, ptControl);
								m_segmentParams.Write(segIndex + 3, ptControl.x);
								m_segmentParams.Write(segIndex + 4, ptControl.y);
								break;
							}

							case com.epl.geometry.SegmentFlags.enumArcSeg:
							{
								throw com.epl.geometry.GeometryException.GeometryInternalError();
							}
						}
					}
				}
				transform.Transform(ptStart, ptStart);
				points.Write(ipoint * 2, ptStart.x);
				points.Write(ipoint * 2 + 1, ptStart.y);
			}
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		// REFACTOR: reset the exact envelope only and transform the loose
		// envelope
		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			if (IsEmpty())
			{
				return;
			}
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			_verifyAllStreams();
			com.epl.geometry.AttributeStreamOfDbl points = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[0];
			com.epl.geometry.AttributeStreamOfDbl zs = (com.epl.geometry.AttributeStreamOfDbl)m_vertexAttributes[1];
			com.epl.geometry.Point3D ptStart = new com.epl.geometry.Point3D();
			com.epl.geometry.Point3D ptControl = new com.epl.geometry.Point3D();
			bool bHasNonLinear = HasNonLinearSegments();
			for (int ipoint = 0; ipoint < m_pointCount; ipoint++)
			{
				ptStart.x = points.Read(ipoint * 2);
				ptStart.y = points.Read(ipoint * 2 + 1);
				ptStart.z = zs.Read(ipoint);
				if (bHasNonLinear)
				{
					int segIndex = m_segmentParamIndex.Read(ipoint);
					if (segIndex >= 0)
					{
						int segmentType = (int)m_segmentFlags.Read(ipoint);
						int type = segmentType & (int)com.epl.geometry.SegmentFlags.enumSegmentMask;
						switch (type)
						{
							case com.epl.geometry.SegmentFlags.enumBezierSeg:
							{
								ptControl.x = m_segmentParams.Read(segIndex);
								ptControl.y = m_segmentParams.Read(segIndex + 1);
								ptControl.z = m_segmentParams.Read(segIndex + 2);
								ptControl = transform.Transform(ptControl);
								m_segmentParams.Write(segIndex, ptControl.x);
								m_segmentParams.Write(segIndex + 1, ptControl.y);
								m_segmentParams.Write(segIndex + 1, ptControl.z);
								ptControl.x = m_segmentParams.Read(segIndex + 3);
								ptControl.y = m_segmentParams.Read(segIndex + 4);
								ptControl.z = m_segmentParams.Read(segIndex + 5);
								ptControl = transform.Transform(ptControl);
								m_segmentParams.Write(segIndex + 3, ptControl.x);
								m_segmentParams.Write(segIndex + 4, ptControl.y);
								m_segmentParams.Write(segIndex + 5, ptControl.z);
								break;
							}

							case com.epl.geometry.SegmentFlags.enumArcSeg:
							{
								throw com.epl.geometry.GeometryException.GeometryInternalError();
							}
						}
					}
				}
				ptStart = transform.Transform(ptStart);
				points.Write(ipoint * 2, ptStart.x);
				points.Write(ipoint * 2 + 1, ptStart.y);
				zs.Write(ipoint, ptStart.z);
			}
			// REFACTOR: reset the exact envelope only and transform the loose
			// envelope
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyCoordinates);
		}

		protected internal override void _verifyStreamsImpl()
		{
			if (m_paths == null)
			{
				m_paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(1, 0);
				m_pathFlags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(1, unchecked((byte)0));
			}
			if (m_segmentFlags != null)
			{
				m_segmentFlags.Resize(m_reservedPointCount, unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg));
				m_segmentParamIndex.Resize(m_reservedPointCount, -1);
			}
		}

		internal override void _copyToImpl(com.epl.geometry.MultiVertexGeometryImpl dst)
		{
			com.epl.geometry.MultiPathImpl dstPoly = (com.epl.geometry.MultiPathImpl)dst;
			dstPoly.m_bPathStarted = false;
			dstPoly.m_curveParamwritePoint = m_curveParamwritePoint;
			dstPoly.m_fill_rule = m_fill_rule;
			if (m_paths != null)
			{
				dstPoly.m_paths = new com.epl.geometry.AttributeStreamOfInt32(m_paths);
			}
			else
			{
				dstPoly.m_paths = null;
			}
			if (m_pathFlags != null)
			{
				dstPoly.m_pathFlags = new com.epl.geometry.AttributeStreamOfInt8(m_pathFlags);
			}
			else
			{
				dstPoly.m_pathFlags = null;
			}
			if (m_segmentParamIndex != null)
			{
				dstPoly.m_segmentParamIndex = new com.epl.geometry.AttributeStreamOfInt32(m_segmentParamIndex);
			}
			else
			{
				dstPoly.m_segmentParamIndex = null;
			}
			if (m_segmentFlags != null)
			{
				dstPoly.m_segmentFlags = new com.epl.geometry.AttributeStreamOfInt8(m_segmentFlags);
			}
			else
			{
				dstPoly.m_segmentFlags = null;
			}
			if (m_segmentParams != null)
			{
				dstPoly.m_segmentParams = new com.epl.geometry.AttributeStreamOfDbl(m_segmentParams);
			}
			else
			{
				dstPoly.m_segmentParams = null;
			}
			dstPoly.m_cachedLength2D = m_cachedLength2D;
			dstPoly.m_cachedArea2D = m_cachedArea2D;
			if (!_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D))
			{
				dstPoly.m_cachedRingAreas2D = (com.epl.geometry.AttributeStreamOfDbl)m_cachedRingAreas2D;
			}
			else
			{
				dstPoly.m_cachedRingAreas2D = null;
			}
		}

		public override double CalculateLength2D()
		{
			if (!_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyLength2D))
			{
				return m_cachedLength2D;
			}
			com.epl.geometry.SegmentIteratorImpl segIter = QuerySegmentIterator();
			com.epl.geometry.MathUtils.KahanSummator len = new com.epl.geometry.MathUtils.KahanSummator(0);
			while (segIter.NextPath())
			{
				while (segIter.HasNextSegment())
				{
					len.Add(segIter.NextSegment().CalculateLength2D());
				}
			}
			m_cachedLength2D = len.GetResult();
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyLength2D, false);
			return len.GetResult();
		}

		public override bool Equals(object other)
		{
			if (other == this)
			{
				return true;
			}
			if (!(other is com.epl.geometry.MultiPathImpl))
			{
				return false;
			}
			if (!base.Equals(other))
			{
				return false;
			}
			com.epl.geometry.MultiPathImpl otherMultiPath = (com.epl.geometry.MultiPathImpl)other;
			int pathCount = GetPathCount();
			int pathCountOther = otherMultiPath.GetPathCount();
			if (pathCount != pathCountOther)
			{
				return false;
			}
			if (pathCount > 0 && m_paths != null && !m_paths.Equals(otherMultiPath.m_paths, 0, pathCount + 1))
			{
				return false;
			}
			if (m_fill_rule != otherMultiPath.m_fill_rule)
			{
				return false;
			}
			{
				// Note: OGC flags do not participate in the equals operation by
				// design.
				// Because for the polygon pathFlags will have all enum_closed set,
				// we do not need to compare this stream. Only for polyline.
				// Polyline does not have OGC flags set.
				if (!m_bPolygon)
				{
					if (m_pathFlags != null && !m_pathFlags.Equals(otherMultiPath.m_pathFlags, 0, pathCount))
					{
						return false;
					}
				}
			}
			return base.Equals(other);
		}

		/// <summary>
		/// Returns a SegmentIterator that set to a specific vertex of the
		/// MultiPathImpl.
		/// </summary>
		/// <remarks>
		/// Returns a SegmentIterator that set to a specific vertex of the
		/// MultiPathImpl. The call to NextSegment will return the segment that
		/// starts at the vertex. Call to PreviousSegment will return the segment
		/// that starts at the previous vertex.
		/// </remarks>
		public com.epl.geometry.SegmentIteratorImpl QuerySegmentIteratorAtVertex(int startVertexIndex)
		{
			if (startVertexIndex < 0 || startVertexIndex >= GetPointCount())
			{
				throw new System.IndexOutOfRangeException();
			}
			com.epl.geometry.SegmentIteratorImpl iter = new com.epl.geometry.SegmentIteratorImpl(this, startVertexIndex);
			return iter;
		}

		// void QuerySegmentIterator(int fromVertex, SegmentIterator iterator);
		public com.epl.geometry.SegmentIteratorImpl QuerySegmentIterator()
		{
			return new com.epl.geometry.SegmentIteratorImpl(this);
		}

		public override void _updateXYImpl(bool bExact)
		{
			base._updateXYImpl(bExact);
			bool bHasCurves = HasNonLinearSegments();
			if (bHasCurves)
			{
				com.epl.geometry.SegmentIteratorImpl segIter = QuerySegmentIterator();
				while (segIter.NextPath())
				{
					while (segIter.HasNextSegment())
					{
						com.epl.geometry.Segment curve = segIter.NextCurve();
						if (curve != null)
						{
							com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
							curve.QueryEnvelope2D(env2D);
							m_envelope.Merge(env2D);
						}
						else
						{
							break;
						}
					}
				}
			}
		}

		internal override void CalculateEnvelope2D(com.epl.geometry.Envelope2D env, bool bExact)
		{
			base.CalculateEnvelope2D(env, bExact);
			bool bHasCurves = HasNonLinearSegments();
			if (bHasCurves)
			{
				com.epl.geometry.SegmentIteratorImpl segIter = QuerySegmentIterator();
				while (segIter.NextPath())
				{
					while (segIter.HasNextSegment())
					{
						com.epl.geometry.Segment curve = segIter.NextCurve();
						if (curve != null)
						{
							com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
							curve.QueryEnvelope2D(env2D);
							env.Merge(env2D);
						}
						else
						{
							break;
						}
					}
				}
			}
		}

		protected internal override void _notifyModifiedAllImpl()
		{
			if (m_paths == null || m_paths.Size() == 0)
			{
				// if (m_paths == null ||
				// !m_paths.size())
				m_pointCount = 0;
			}
			else
			{
				m_pointCount = m_paths.Read(m_paths.Size() - 1);
			}
		}

		public override double CalculateArea2D()
		{
			if (!m_bPolygon)
			{
				return 0.0;
			}
			_updateRingAreas2D();
			return m_cachedArea2D;
		}

		/// <summary>Returns True if the ring is an exterior ring.</summary>
		/// <remarks>
		/// Returns True if the ring is an exterior ring. Valid only for simple
		/// polygons.
		/// </remarks>
		public bool IsExteriorRing(int ringIndex)
		{
			if (!m_bPolygon)
			{
				return false;
			}
			if (!_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags))
			{
				return (m_pathFlags.Read(ringIndex) & unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon)) != 0;
			}
			_updateRingAreas2D();
			return m_cachedRingAreas2D.Read(ringIndex) > 0;
		}

		// Should we make a function called _UpdateHasNonLinearSegmentsFlags and
		// call it here?
		public double CalculateRingArea2D(int pathIndex)
		{
			if (!m_bPolygon)
			{
				return 0.0;
			}
			_updateRingAreas2D();
			return m_cachedRingAreas2D.Read(pathIndex);
		}

		public void _updateRingAreas2D()
		{
			if (_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D))
			{
				int pathCount = GetPathCount();
				if (m_cachedRingAreas2D == null)
				{
					m_cachedRingAreas2D = new com.epl.geometry.AttributeStreamOfDbl(pathCount);
				}
				else
				{
					if (m_cachedRingAreas2D.Size() != pathCount)
					{
						m_cachedRingAreas2D.Resize(pathCount);
					}
				}
				com.epl.geometry.MathUtils.KahanSummator totalArea = new com.epl.geometry.MathUtils.KahanSummator(0);
				com.epl.geometry.MathUtils.KahanSummator pathArea = new com.epl.geometry.MathUtils.KahanSummator(0);
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				int ipath = 0;
				com.epl.geometry.SegmentIteratorImpl segIter = QuerySegmentIterator();
				while (segIter.NextPath())
				{
					pathArea.Reset();
					GetXY(GetPathStart(segIter.GetPathIndex()), pt);
					// get the area
					// calculation
					// origin to be
					// the origin of
					// the ring.
					while (segIter.HasNextSegment())
					{
						pathArea.Add(segIter.NextSegment()._calculateArea2DHelper(pt.x, pt.y));
					}
					totalArea.Add(pathArea.GetResult());
					int i = ipath++;
					m_cachedRingAreas2D.Write(i, pathArea.GetResult());
				}
				m_cachedArea2D = totalArea.GetResult();
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D, false);
			}
		}

		internal int GetOGCPolygonCount()
		{
			if (!m_bPolygon)
			{
				return 0;
			}
			_updateOGCFlags();
			int polygonCount = 0;
			int partCount = GetPathCount();
			for (int ipart = 0; ipart < partCount; ipart++)
			{
				if (((int)m_pathFlags.Read(ipart) & (int)com.epl.geometry.PathFlags.enumOGCStartPolygon) != 0)
				{
					polygonCount++;
				}
			}
			return polygonCount;
		}

		protected internal void _updateOGCFlags()
		{
			if (_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags))
			{
				_updateRingAreas2D();
				int pathCount = GetPathCount();
				if (pathCount > 0 && (m_pathFlags == null || m_pathFlags.Size() < pathCount))
				{
					m_pathFlags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(pathCount + 1);
				}
				int firstSign = 1;
				for (int ipath = 0; ipath < pathCount; ipath++)
				{
					double area = m_cachedRingAreas2D.Read(ipath);
					if (ipath == 0)
					{
						firstSign = area > 0 ? 1 : -1;
					}
					if (area * firstSign > 0.0)
					{
						m_pathFlags.SetBits(ipath, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
					}
					else
					{
						m_pathFlags.ClearBits(ipath, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
					}
				}
				_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags, false);
			}
		}

		public int GetPathIndexFromPointIndex(int pointIndex)
		{
			int positionHint = m_currentPathIndex;
			// in case of multithreading
			// thiswould simply produce an
			// invalid value
			int pathCount = GetPathCount();
			// Try using the hint position first to get the path index.
			if (positionHint >= 0 && positionHint < pathCount)
			{
				if (pointIndex < GetPathEnd(positionHint))
				{
					if (pointIndex >= GetPathStart(positionHint))
					{
						return positionHint;
					}
					positionHint--;
				}
				else
				{
					positionHint++;
				}
				if (positionHint >= 0 && positionHint < pathCount)
				{
					if (pointIndex >= GetPathStart(positionHint) && pointIndex < GetPathEnd(positionHint))
					{
						m_currentPathIndex = positionHint;
						return positionHint;
					}
				}
			}
			if (pathCount < 5)
			{
				// TODO: time the performance to choose when to use
				// linear search.
				for (int i = 0; i < pathCount; i++)
				{
					if (pointIndex < GetPathEnd(i))
					{
						m_currentPathIndex = i;
						return i;
					}
				}
				throw new com.epl.geometry.GeometryException("corrupted geometry");
			}
			// Do binary search:
			int minPathIndex = 0;
			int maxPathIndex = pathCount - 1;
			while (maxPathIndex > minPathIndex)
			{
				int mid = minPathIndex + ((maxPathIndex - minPathIndex) >> 1);
				int pathStart = GetPathStart(mid);
				if (pointIndex < pathStart)
				{
					maxPathIndex = mid - 1;
				}
				else
				{
					int pathEnd = GetPathEnd(mid);
					if (pointIndex >= pathEnd)
					{
						minPathIndex = mid + 1;
					}
					else
					{
						m_currentPathIndex = mid;
						return mid;
					}
				}
			}
			m_currentPathIndex = minPathIndex;
			return minPathIndex;
		}

		internal int GetHighestPointIndex(int path_index)
		{
			System.Diagnostics.Debug.Assert((path_index >= 0 && path_index < GetPathCount()));
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			com.epl.geometry.AttributeStreamOfInt32 paths = (com.epl.geometry.AttributeStreamOfInt32)(GetPathStreamRef());
			int path_end = GetPathEnd(path_index);
			int path_start = GetPathStart(path_index);
			int max_index = -1;
			com.epl.geometry.Point2D max_point = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			max_point.y = com.epl.geometry.NumberUtils.NegativeInf();
			max_point.x = com.epl.geometry.NumberUtils.NegativeInf();
			for (int i = path_start + 0; i < path_end; i++)
			{
				position.Read(2 * i, pt);
				if (max_point.Compare(pt) == -1)
				{
					max_index = i;
					max_point.SetCoords(pt);
				}
			}
			return max_index;
		}

		/// <summary>Returns total segment count in the MultiPathImpl.</summary>
		public int GetSegmentCount()
		{
			int segCount = GetPointCount();
			if (!m_bPolygon)
			{
				segCount -= GetPathCount();
				for (int i = 0, n = GetPathCount(); i < n; i++)
				{
					if (IsClosedPath(i))
					{
						segCount++;
					}
				}
			}
			return segCount;
		}

		public int GetSegmentCount(int path_index)
		{
			int segCount = GetPathSize(path_index);
			if (!IsClosedPath(path_index))
			{
				segCount--;
			}
			return segCount;
		}

		// HEADER defintions
		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.MultiPathImpl(m_bPolygon, GetDescription());
		}

		public override int GetDimension()
		{
			return m_bPolygon ? 2 : 1;
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return m_bPolygon ? com.epl.geometry.Geometry.Type.Polygon : com.epl.geometry.Geometry.Type.Polyline;
		}

		/// <summary>Returns True if the class is envelope.</summary>
		/// <remarks>
		/// Returns True if the class is envelope. THis is not an exact method. Only
		/// addEnvelope makes this true.
		/// </remarks>
		public bool IsEnvelope()
		{
			return !_hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyIsEnvelope);
		}

		/// <summary>
		/// Returns a reference to the AttributeStream of MultiPathImpl parts
		/// (Paths).
		/// </summary>
		/// <remarks>
		/// Returns a reference to the AttributeStream of MultiPathImpl parts
		/// (Paths).
		/// For the non empty MultiPathImpl, that stream contains start points of the
		/// MultiPathImpl curves. In addition, the last element is the total point
		/// count. The number of vertices in a given part is parts[i + 1] - parts[i].
		/// </remarks>
		public com.epl.geometry.AttributeStreamOfInt32 GetPathStreamRef()
		{
			ThrowIfEmpty();
			return m_paths;
		}

		/// <summary>sets a reference to an AttributeStream of MultiPathImpl paths (Paths).</summary>
		public void SetPathStreamRef(com.epl.geometry.AttributeStreamOfInt32 paths)
		{
			m_paths = paths;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
		}

		/// <summary>
		/// Returns a reference to the AttributeStream of Segment flags (SegmentFlags
		/// flags).
		/// </summary>
		/// <remarks>
		/// Returns a reference to the AttributeStream of Segment flags (SegmentFlags
		/// flags). Can be NULL when no non-linear segments are present.
		/// Segment flags indicate what kind of segment originates (starts) on the
		/// given point. The last vertices of open Path parts has enumNone flag.
		/// </remarks>
		public com.epl.geometry.AttributeStreamOfInt8 GetSegmentFlagsStreamRef()
		{
			ThrowIfEmpty();
			return m_segmentFlags;
		}

		/// <summary>
		/// Returns a reference to the AttributeStream of Path flags (PathFlags
		/// flags).
		/// </summary>
		/// <remarks>
		/// Returns a reference to the AttributeStream of Path flags (PathFlags
		/// flags).
		/// Each start point of a path has a flag set to indicate if the Path is open
		/// or closed.
		/// </remarks>
		public com.epl.geometry.AttributeStreamOfInt8 GetPathFlagsStreamRef()
		{
			ThrowIfEmpty();
			return m_pathFlags;
		}

		/// <summary>sets a reference to an AttributeStream of Path flags (PathFlags flags).</summary>
		public void SetPathFlagsStreamRef(com.epl.geometry.AttributeStreamOfInt8 pathFlags)
		{
			m_pathFlags = pathFlags;
			NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
		}

		public com.epl.geometry.AttributeStreamOfInt32 GetSegmentIndexStreamRef()
		{
			ThrowIfEmpty();
			return m_segmentParamIndex;
		}

		public com.epl.geometry.AttributeStreamOfDbl GetSegmentDataStreamRef()
		{
			ThrowIfEmpty();
			return m_segmentParams;
		}

		public int GetPathCount()
		{
			return (m_paths != null) ? m_paths.Size() - 1 : 0;
		}

		public int GetPathEnd(int partIndex)
		{
			return m_paths.Read(partIndex + 1);
		}

		public int GetPathSize(int partIndex)
		{
			return m_paths.Read(partIndex + 1) - m_paths.Read(partIndex);
		}

		public int GetPathStart(int partIndex)
		{
			return m_paths.Read(partIndex);
		}

		protected internal override object _getImpl()
		{
			return this;
		}

		public void SetDirtyOGCFlags(bool bYesNo)
		{
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags, bYesNo);
		}

		public bool HasDirtyOGCStartFlags()
		{
			return _hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags);
		}

		public void SetDirtyRingAreas2D(bool bYesNo)
		{
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D, bYesNo);
		}

		public bool HasDirtyRingAreas2D()
		{
			return _hasDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D);
		}

		public void SetRingAreasStreamRef(com.epl.geometry.AttributeStreamOfDbl ringAreas)
		{
			m_cachedRingAreas2D = ringAreas;
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyRingAreas2D, false);
		}

		// HEADER defintions
		// // TODO check this against current implementation in native
		// public void notifyModified(int flags)
		// {
		// if(flags == DirtyFlags.DirtyAll)
		// {
		// m_reservedPointCount = -1;
		// _notifyModifiedAllImpl();
		// }
		// m_flagsMask |= flags;
		// _clearAccelerators();
		//
		//
		// // ROHIT's implementation
		// // if (m_paths == null || 0 == m_paths.size())
		// // m_pointCount = 0;
		// // else
		// // m_pointCount = m_paths.read(m_paths.size() - 1);
		// //
		// // super.notifyModified(flags);
		// }
		public override bool _buildRasterizedGeometryAccelerator(double toleranceXY, com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			if (m_accelerators == null)
			{
				// (!m_accelerators)
				m_accelerators = new com.epl.geometry.GeometryAccelerators();
			}
			int rasterSize = com.epl.geometry.RasterizedGeometry2D.RasterSizeFromAccelerationDegree(accelDegree);
			com.epl.geometry.RasterizedGeometry2D rgeom = m_accelerators.GetRasterizedGeometry();
			if (rgeom != null)
			{
				if (rgeom.GetToleranceXY() < toleranceXY || rasterSize > rgeom.GetRasterSize())
				{
					m_accelerators._setRasterizedGeometry(null);
				}
				else
				{
					return true;
				}
			}
			rgeom = com.epl.geometry.RasterizedGeometry2D.Create(this, toleranceXY, rasterSize);
			m_accelerators._setRasterizedGeometry(rgeom);
			//rgeom.dbgSaveToBitmap("c:/temp/ddd.bmp");
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (!IsEmptyImpl())
			{
				int pathCount = GetPathCount();
				if (m_paths != null)
				{
					m_paths.CalculateHashImpl(hashCode, 0, pathCount + 1);
				}
				if (m_pathFlags != null)
				{
					m_pathFlags.CalculateHashImpl(hashCode, 0, pathCount);
				}
			}
			return hashCode;
		}

		public byte GetSegmentFlags(int ivertex)
		{
			if (m_segmentFlags != null)
			{
				return m_segmentFlags.Read(ivertex);
			}
			else
			{
				return unchecked((byte)com.epl.geometry.SegmentFlags.enumLineSeg);
			}
		}

		public void GetSegment(int startVertexIndex, com.epl.geometry.SegmentBuffer segBuffer, bool bStripAttributes)
		{
			int ipath = GetPathIndexFromPointIndex(startVertexIndex);
			if (startVertexIndex == GetPathEnd(ipath) - 1 && !IsClosedPath(ipath))
			{
				throw new com.epl.geometry.GeometryException("index out of bounds");
			}
			_verifyAllStreams();
			com.epl.geometry.AttributeStreamOfInt8 segFlagStream = GetSegmentFlagsStreamRef();
			int segFlag = com.epl.geometry.SegmentFlags.enumLineSeg;
			if (segFlagStream != null)
			{
				segFlag = segFlagStream.Read(startVertexIndex) & com.epl.geometry.SegmentFlags.enumSegmentMask;
			}
			switch (segFlag)
			{
				case com.epl.geometry.SegmentFlags.enumLineSeg:
				{
					segBuffer.CreateLine();
					break;
				}

				case com.epl.geometry.SegmentFlags.enumBezierSeg:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				case com.epl.geometry.SegmentFlags.enumArcSeg:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
			com.epl.geometry.Segment currentSegment = segBuffer.Get();
			if (!bStripAttributes)
			{
				currentSegment.AssignVertexDescription(m_description);
			}
			else
			{
				currentSegment.AssignVertexDescription(com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D());
			}
			int endVertexIndex;
			if (startVertexIndex == GetPathEnd(ipath) - 1 && IsClosedPath(ipath))
			{
				endVertexIndex = GetPathStart(ipath);
			}
			else
			{
				endVertexIndex = startVertexIndex + 1;
			}
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			GetXY(startVertexIndex, pt);
			currentSegment.SetStartXY(pt);
			GetXY(endVertexIndex, pt);
			currentSegment.SetEndXY(pt);
			if (!bStripAttributes)
			{
				for (int i = 1, nattr = m_description.GetAttributeCount(); i < nattr; i++)
				{
					int semantics = m_description._getSemanticsImpl(i);
					int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					for (int ord = 0; ord < ncomp; ord++)
					{
						double vs = GetAttributeAsDbl(semantics, startVertexIndex, ord);
						currentSegment.SetStartAttribute(semantics, ord, vs);
						double ve = GetAttributeAsDbl(semantics, endVertexIndex, ord);
						currentSegment.SetEndAttribute(semantics, ord, ve);
					}
				}
			}
		}

		internal void QueryPathEnvelope2D(int path_index, com.epl.geometry.Envelope2D envelope)
		{
			if (path_index >= GetPathCount())
			{
				throw new System.ArgumentException();
			}
			if (IsEmpty())
			{
				envelope.SetEmpty();
				return;
			}
			if (HasNonLinearSegments(path_index))
			{
				throw new com.epl.geometry.GeometryException("not implemented");
			}
			else
			{
				com.epl.geometry.AttributeStreamOfDbl stream = (com.epl.geometry.AttributeStreamOfDbl)GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetEmpty();
				for (int i = GetPathStart(path_index), iend = GetPathEnd(path_index); i < iend; i++)
				{
					stream.Read(2 * i, pt);
					env.Merge(pt);
				}
				envelope.SetCoords(env);
			}
		}

		public void QueryLoosePathEnvelope2D(int path_index, com.epl.geometry.Envelope2D envelope)
		{
			if (path_index >= GetPathCount())
			{
				throw new System.ArgumentException();
			}
			if (IsEmpty())
			{
				envelope.SetEmpty();
				return;
			}
			if (HasNonLinearSegments(path_index))
			{
				throw new com.epl.geometry.GeometryException("not implemented");
			}
			else
			{
				com.epl.geometry.AttributeStreamOfDbl stream = (com.epl.geometry.AttributeStreamOfDbl)GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetEmpty();
				for (int i = GetPathStart(path_index), iend = GetPathEnd(path_index); i < iend; i++)
				{
					stream.Read(2 * i, pt);
					env.Merge(pt);
				}
				envelope.SetCoords(env);
			}
		}

		public override bool _buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree d)
		{
			if (m_accelerators == null)
			{
				// (!m_accelerators)
				m_accelerators = new com.epl.geometry.GeometryAccelerators();
			}
			if (d == com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMild || GetPointCount() < 16)
			{
				return false;
			}
			com.epl.geometry.QuadTreeImpl quad_tree_impl = com.epl.geometry.InternalUtils.BuildQuadTree(this);
			m_accelerators._setQuadTree(quad_tree_impl);
			return true;
		}

		internal bool _buildQuadTreeForPathsAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree degree)
		{
			if (m_accelerators == null)
			{
				m_accelerators = new com.epl.geometry.GeometryAccelerators();
			}
			// TODO: when less than two envelopes - no need to this.
			if (m_accelerators.GetQuadTreeForPaths() != null)
			{
				return true;
			}
			m_accelerators._setQuadTreeForPaths(null);
			com.epl.geometry.QuadTreeImpl quad_tree_impl = com.epl.geometry.InternalUtils.BuildQuadTreeForPaths(this);
			m_accelerators._setQuadTreeForPaths(quad_tree_impl);
			return true;
		}

		internal void SetFillRule(int rule)
		{
			System.Diagnostics.Debug.Assert((m_bPolygon));
			m_fill_rule = rule;
		}

		internal int GetFillRule()
		{
			return m_fill_rule;
		}

		internal void ClearDirtyOGCFlags()
		{
			_setDirtyFlag(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyOGCFlags, false);
		}
	}
}
