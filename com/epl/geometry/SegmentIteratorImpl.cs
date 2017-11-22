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
	/// <summary>Provides functionality to iterate over MultiPath segments.</summary>
	internal sealed class SegmentIteratorImpl
	{
		protected internal com.epl.geometry.Line m_line;

		protected internal com.epl.geometry.Segment m_currentSegment;

		protected internal com.epl.geometry.Point2D m_dummyPoint;

		protected internal int m_currentPathIndex;

		protected internal int m_nextPathIndex;

		protected internal int m_prevPathIndex;

		protected internal int m_currentSegmentIndex;

		protected internal int m_nextSegmentIndex;

		protected internal int m_prevSegmentIndex;

		protected internal int m_segmentCount;

		protected internal int m_pathBegin;

		protected internal com.epl.geometry.MultiPathImpl m_parent;

		protected internal bool m_bCirculator;

		protected internal bool m_bNeedsUpdate;

		public SegmentIteratorImpl(com.epl.geometry.MultiPathImpl parent)
		{
			// Bezier m_bezier:
			// Arc m_arc;
			// parent of the iterator.
			// If true, the iterator circulates around
			// the current Path.
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = 0;
			m_nextPathIndex = 0;
			m_currentPathIndex = -1;
			m_parent = parent;
			m_segmentCount = _getSegmentCount(m_nextPathIndex);
			m_bCirculator = false;
			m_currentSegment = null;
			m_pathBegin = -1;
			m_dummyPoint = new com.epl.geometry.Point2D();
		}

		public SegmentIteratorImpl(com.epl.geometry.MultiPathImpl parent, int pointIndex)
		{
			if (pointIndex < 0 || pointIndex >= parent.GetPointCount())
			{
				throw new System.IndexOutOfRangeException();
			}
			m_currentSegmentIndex = -1;
			int path = parent.GetPathIndexFromPointIndex(pointIndex);
			m_nextSegmentIndex = pointIndex - parent.GetPathStart(path);
			m_nextPathIndex = path + 1;
			m_currentPathIndex = path;
			m_parent = parent;
			m_segmentCount = _getSegmentCount(m_currentPathIndex);
			m_bCirculator = false;
			m_currentSegment = null;
			m_pathBegin = m_parent.GetPathStart(m_currentPathIndex);
			m_dummyPoint = new com.epl.geometry.Point2D();
		}

		public SegmentIteratorImpl(com.epl.geometry.MultiPathImpl parent, int pathIndex, int segmentIndex)
		{
			if (pathIndex < 0 || pathIndex >= parent.GetPathCount() || segmentIndex < 0)
			{
				throw new System.IndexOutOfRangeException();
			}
			int d = parent.IsClosedPath(pathIndex) ? 0 : 1;
			if (segmentIndex >= parent.GetPathSize(pathIndex) - d)
			{
				throw new System.IndexOutOfRangeException();
			}
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = segmentIndex;
			m_currentPathIndex = pathIndex;
			m_nextPathIndex = m_nextSegmentIndex + 1;
			m_parent = parent;
			m_segmentCount = _getSegmentCount(m_nextPathIndex);
			m_bCirculator = false;
			m_currentSegment = null;
			m_pathBegin = m_parent.GetPathStart(m_currentPathIndex);
			m_dummyPoint = new com.epl.geometry.Point2D();
		}

		internal void ResetTo(com.epl.geometry.SegmentIteratorImpl src)
		{
			if (m_parent != src.m_parent)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			m_currentSegmentIndex = src.m_currentSegmentIndex;
			m_nextSegmentIndex = src.m_nextSegmentIndex;
			m_currentPathIndex = src.m_currentPathIndex;
			m_nextPathIndex = src.m_nextPathIndex;
			m_segmentCount = src.m_segmentCount;
			m_bCirculator = src.m_bCirculator;
			m_pathBegin = src.m_pathBegin;
			m_currentSegment = null;
		}

		/// <summary>Moves the iterator to the next curve segment and returns the segment.</summary>
		/// <remarks>
		/// Moves the iterator to the next curve segment and returns the segment.
		/// The Segment is returned by value and is owned by the iterator. Note: The
		/// method can return null if there are no curves in the part.
		/// </remarks>
		public com.epl.geometry.Segment NextCurve()
		{
			return null;
		}

		// TODO: Fix me. This method is supposed to go only through the curves
		// and skip the Line classes!!
		// It must be very efficient.
		/// <summary>Moves the iterator to next segment and returns the segment.</summary>
		/// <remarks>
		/// Moves the iterator to next segment and returns the segment.
		/// The Segment is returned by value and is owned by the iterator.
		/// </remarks>
		public com.epl.geometry.Segment NextSegment()
		{
			if (m_currentSegmentIndex != m_nextSegmentIndex)
			{
				_updateSegment();
			}
			if (m_bCirculator)
			{
				m_nextSegmentIndex = (m_nextSegmentIndex + 1) % m_segmentCount;
			}
			else
			{
				if (m_nextSegmentIndex == m_segmentCount)
				{
					throw new System.IndexOutOfRangeException();
				}
				m_nextSegmentIndex++;
			}
			return m_currentSegment;
		}

		/// <summary>Moves the iterator to previous segment and returns the segment.</summary>
		/// <remarks>
		/// Moves the iterator to previous segment and returns the segment.
		/// The Segment is returned by value and is owned by the iterator.
		/// </remarks>
		public com.epl.geometry.Segment PreviousSegment()
		{
			if (m_bCirculator)
			{
				m_nextSegmentIndex = (m_segmentCount + m_nextSegmentIndex - 1) % m_segmentCount;
			}
			else
			{
				if (m_nextSegmentIndex == 0)
				{
					throw new System.IndexOutOfRangeException();
				}
				m_nextSegmentIndex--;
			}
			if (m_nextSegmentIndex != m_currentSegmentIndex)
			{
				_updateSegment();
			}
			return m_currentSegment;
		}

		/// <summary>
		/// Resets the iterator so that the call to NextSegment will return the first
		/// segment of the current path.
		/// </summary>
		public void ResetToFirstSegment()
		{
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = 0;
		}

		/// <summary>
		/// Resets the iterator so that the call to PreviousSegment will return the
		/// last segment of the current path.
		/// </summary>
		public void ResetToLastSegment()
		{
			m_nextSegmentIndex = m_segmentCount;
			m_currentSegmentIndex = -1;
		}

		public void ResetToVertex(int vertexIndex)
		{
			ResetToVertex(vertexIndex, -1);
		}

		public void ResetToVertex(int vertexIndex, int _pathIndex)
		{
			if (m_currentPathIndex >= 0 && m_currentPathIndex < m_parent.GetPathCount())
			{
				// check if we
				// are in
				// the
				// current
				// path
				int start = _getPathBegin();
				if (vertexIndex >= start && vertexIndex < m_parent.GetPathEnd(m_currentPathIndex))
				{
					m_currentSegmentIndex = -1;
					m_nextSegmentIndex = vertexIndex - start;
					return;
				}
			}
			int path_index;
			if (_pathIndex >= 0 && _pathIndex < m_parent.GetPathCount() && vertexIndex >= m_parent.GetPathStart(_pathIndex) && vertexIndex < m_parent.GetPathEnd(_pathIndex))
			{
				path_index = _pathIndex;
			}
			else
			{
				path_index = m_parent.GetPathIndexFromPointIndex(vertexIndex);
			}
			m_nextPathIndex = path_index + 1;
			m_currentPathIndex = path_index;
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = vertexIndex - m_parent.GetPathStart(path_index);
			m_segmentCount = _getSegmentCount(path_index);
			m_pathBegin = m_parent.GetPathStart(m_currentPathIndex);
		}

		/// <summary>Moves the iterator to next path and returns true if successful.</summary>
		public bool NextPath()
		{
			// post-increment
			m_currentPathIndex = m_nextPathIndex;
			if (m_currentPathIndex >= m_parent.GetPathCount())
			{
				return false;
			}
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = 0;
			m_segmentCount = _getSegmentCount(m_currentPathIndex);
			m_pathBegin = m_parent.GetPathStart(m_currentPathIndex);
			m_nextPathIndex++;
			return true;
		}

		/// <summary>Moves the iterator to next path and returns true if successful.</summary>
		public bool PreviousPath()
		{
			// pre-decrement
			if (m_nextPathIndex == 0)
			{
				return false;
			}
			m_nextPathIndex--;
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = 0;
			m_segmentCount = _getSegmentCount(m_nextPathIndex);
			m_currentPathIndex = m_nextPathIndex;
			m_pathBegin = m_parent.GetPathStart(m_currentPathIndex);
			ResetToLastSegment();
			return true;
		}

		/// <summary>
		/// Resets the iterator such that the subsequent call to the NextPath will
		/// set the iterator to the first segment of the first path.
		/// </summary>
		public void ResetToFirstPath()
		{
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = -1;
			m_segmentCount = -1;
			m_nextPathIndex = 0;
			m_currentPathIndex = -1;
			m_pathBegin = -1;
		}

		/// <summary>
		/// Resets the iterator such that the subsequent call to the PreviousPath
		/// will set the iterator to the last segment of the last path.
		/// </summary>
		public void ResetToLastPath()
		{
			m_nextPathIndex = m_parent.GetPathCount();
			m_currentPathIndex = -1;
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = -1;
			m_segmentCount = -1;
			m_pathBegin = -1;
		}

		/// <summary>
		/// Resets the iterator such that the subsequent call to the NextPath will
		/// set the iterator to the first segment of the given path.
		/// </summary>
		/// <remarks>
		/// Resets the iterator such that the subsequent call to the NextPath will
		/// set the iterator to the first segment of the given path. The call to
		/// PreviousPath will reset the iterator to the last segment of path
		/// pathIndex - 1.
		/// </remarks>
		public void ResetToPath(int pathIndex)
		{
			if (pathIndex < 0)
			{
				throw new System.IndexOutOfRangeException();
			}
			m_nextPathIndex = pathIndex;
			m_currentPathIndex = -1;
			m_currentSegmentIndex = -1;
			m_nextSegmentIndex = -1;
			m_segmentCount = -1;
			m_pathBegin = -1;
		}

		public int _getSegmentCount(int pathIndex)
		{
			if (m_parent.IsEmptyImpl())
			{
				return 0;
			}
			int d = 1;
			if (m_parent.IsClosedPath(pathIndex))
			{
				d = 0;
			}
			return m_parent.GetPathSize(pathIndex) - d;
		}

		/// <summary>Returns True, if the segment is the closing segment of the closed path</summary>
		public bool IsClosingSegment()
		{
			return m_currentSegmentIndex == m_segmentCount - 1 && m_parent.IsClosedPath(m_currentPathIndex);
		}

		/// <summary>Switches the iterator navigation mode.</summary>
		/// <param name="bYesNo">
		/// If True, the iterator loops over the current path infinitely
		/// (unless the MultiPath is empty).
		/// </param>
		public void SetCirculator(bool bYesNo)
		{
			m_bCirculator = bYesNo;
		}

		/// <summary>Returns the index of the current path.</summary>
		public int GetPathIndex()
		{
			return m_currentPathIndex;
		}

		/// <summary>Returns the index of the start Point of the current Segment.</summary>
		public int GetStartPointIndex()
		{
			return _getPathBegin() + m_currentSegmentIndex;
		}

		public int _getPathBegin()
		{
			return m_parent.GetPathStart(m_currentPathIndex);
		}

		/// <summary>Returns the index of the end Point of the current Segment.</summary>
		public int GetEndPointIndex()
		{
			if (IsClosingSegment())
			{
				return m_parent.GetPathStart(m_currentPathIndex);
			}
			else
			{
				return GetStartPointIndex() + 1;
			}
		}

		/// <summary>Returns True if the segment is first one in the current Path.</summary>
		public bool IsFirstSegmentInPath()
		{
			return m_currentSegmentIndex == 0;
		}

		/// <summary>Returns True if the segment is last one in the current Path.</summary>
		public bool IsLastSegmentInPath()
		{
			return m_currentSegmentIndex == m_segmentCount - 1;
		}

		/// <summary>Returns True if the call to the NextSegment will succeed.</summary>
		public bool HasNextSegment()
		{
			return m_nextSegmentIndex < m_segmentCount;
		}

		/// <summary>Returns True if the call to the NextSegment will succeed.</summary>
		public bool HasPreviousSegment()
		{
			return m_nextSegmentIndex > 0;
		}

		public com.epl.geometry.SegmentIteratorImpl Copy()
		{
			com.epl.geometry.SegmentIteratorImpl clone = new com.epl.geometry.SegmentIteratorImpl(m_parent);
			clone.m_currentSegmentIndex = m_currentSegmentIndex;
			clone.m_nextSegmentIndex = m_nextSegmentIndex;
			clone.m_segmentCount = m_segmentCount;
			clone.m_currentPathIndex = m_currentPathIndex;
			clone.m_nextPathIndex = m_nextPathIndex;
			clone.m_parent = m_parent;
			clone.m_bCirculator = m_bCirculator;
			return clone;
		}

		public void _updateSegment()
		{
			if (m_nextSegmentIndex < 0 || m_nextSegmentIndex >= m_segmentCount)
			{
				throw new System.IndexOutOfRangeException();
			}
			m_currentSegmentIndex = m_nextSegmentIndex;
			int startVertexIndex = GetStartPointIndex();
			m_parent._verifyAllStreams();
			com.epl.geometry.AttributeStreamOfInt8 segFlagStream = m_parent.GetSegmentFlagsStreamRef();
			int segFlag = com.epl.geometry.SegmentFlags.enumLineSeg;
			if (segFlagStream != null)
			{
				segFlag = (segFlagStream.Read(startVertexIndex) & com.epl.geometry.SegmentFlags.enumSegmentMask);
			}
			com.epl.geometry.VertexDescription vertexDescr = m_parent.GetDescription();
			switch (segFlag)
			{
				case com.epl.geometry.SegmentFlags.enumLineSeg:
				{
					if (m_line == null)
					{
						m_line = new com.epl.geometry.Line();
					}
					m_currentSegment = (com.epl.geometry.Line)m_line;
					break;
				}

				case com.epl.geometry.SegmentFlags.enumBezierSeg:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				case com.epl.geometry.SegmentFlags.enumArcSeg:
				{
					// break;
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				default:
				{
					// break;
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
			m_currentSegment.AssignVertexDescription(vertexDescr);
			int endVertexIndex = GetEndPointIndex();
			m_parent.GetXY(startVertexIndex, m_dummyPoint);
			m_currentSegment.SetStartXY(m_dummyPoint);
			m_parent.GetXY(endVertexIndex, m_dummyPoint);
			m_currentSegment.SetEndXY(m_dummyPoint);
			for (int i = 1, nattr = vertexDescr.GetAttributeCount(); i < nattr; i++)
			{
				int semantics = vertexDescr.GetSemantics(i);
				int ncomp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
				for (int ord = 0; ord < ncomp; ord++)
				{
					double vs = m_parent.GetAttributeAsDbl(semantics, startVertexIndex, ord);
					m_currentSegment.SetStartAttribute(semantics, ord, vs);
					double ve = m_parent.GetAttributeAsDbl(semantics, endVertexIndex, ord);
					m_currentSegment.SetEndAttribute(semantics, ord, ve);
				}
			}
		}

		internal bool IsLastPath()
		{
			return m_currentPathIndex == m_parent.GetPathCount() - 1;
		}
	}
}
