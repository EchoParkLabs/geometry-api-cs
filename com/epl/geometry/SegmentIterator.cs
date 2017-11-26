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
	/// <summary>This class provides functionality to iterate over MultiPath segments.</summary>
	/// <remarks>
	/// This class provides functionality to iterate over MultiPath segments.
	/// Example:
	/// <pre><code>
	/// SegmentIterator iterator = polygon.querySegmentIterator();
	/// while (iterator.nextPath()) {
	/// while (iterator.hasNextSegment()) {
	/// Segment segment = iterator.nextSegment();
	/// }
	/// }
	/// </code></pre>
	/// </remarks>
	public class SegmentIterator
	{
		private com.epl.geometry.SegmentIteratorImpl m_impl;

		internal SegmentIterator(object obj)
		{
			m_impl = (com.epl.geometry.SegmentIteratorImpl)obj;
		}

		/// <summary>Moves the iterator to the next path.</summary>
		/// <remarks>Moves the iterator to the next path. Returns the TRUE if successful.</remarks>
		/// <returns>TRUE if the next path exists.</returns>
		public virtual bool NextPath()
		{
			return m_impl.NextPath();
		}

		/// <summary>Moves the iterator to the previous path.</summary>
		/// <remarks>Moves the iterator to the previous path. Returns the TRUE if successful.</remarks>
		/// <returns>TRUE if the previous path exists.</returns>
		public virtual bool PreviousPath()
		{
			return m_impl.PreviousPath();
		}

		/// <summary>
		/// Resets the iterator such that a subsequent call to NextPath will set the
		/// iterator to the first path.
		/// </summary>
		public virtual void ResetToFirstPath()
		{
			m_impl.ResetToFirstPath();
		}

		/// <summary>
		/// Resets the iterator such that a subsequent call to PreviousPath will set
		/// the iterator to the last path.
		/// </summary>
		public virtual void ResetToLastPath()
		{
			m_impl.ResetToLastPath();
		}

		/// <summary>
		/// Resets the iterator such that a subsequent call to NextPath will set the
		/// iterator to the given path index.
		/// </summary>
		/// <remarks>
		/// Resets the iterator such that a subsequent call to NextPath will set the
		/// iterator to the given path index. A call to PreviousPath will set the
		/// iterator to the path at pathIndex - 1.
		/// </remarks>
		public virtual void ResetToPath(int pathIndex)
		{
			m_impl.ResetToPath(pathIndex);
		}

		/// <summary>
		/// Indicates whether the iterator points to the first segment in the current
		/// path.
		/// </summary>
		/// <returns>
		/// TRUE if the iterator points to the first segment in the current
		/// path.
		/// </returns>
		public virtual bool IsFirstSegmentInPath()
		{
			return m_impl.IsFirstSegmentInPath();
		}

		/// <summary>
		/// Indicates whether the iterator points to the last segment in the current
		/// path.
		/// </summary>
		/// <returns>
		/// TRUE if the iterator points to the last segment in the current
		/// path.
		/// </returns>
		public virtual bool IsLastSegmentInPath()
		{
			return m_impl.IsLastSegmentInPath();
		}

		/// <summary>
		/// Resets the iterator so that the call to NextSegment will return the first
		/// segment of the current path.
		/// </summary>
		public virtual void ResetToFirstSegment()
		{
			m_impl.ResetToFirstSegment();
		}

		/// <summary>
		/// Resets the iterator so that the call to PreviousSegment will return the
		/// last segment of the current path.
		/// </summary>
		public virtual void ResetToLastSegment()
		{
			m_impl.ResetToLastSegment();
		}

		/// <summary>Resets the iterator to a specific vertex.</summary>
		/// <remarks>
		/// Resets the iterator to a specific vertex.
		/// The call to next_segment will return the segment that starts at the vertex.
		/// Call to previous_segment will return the segment that starts at the previous vertex.
		/// </remarks>
		/// <param name="vertexIndex">The vertex index to reset the iterator to.</param>
		/// <param name="pathIndex">The path index to reset the iterator to. Used as a hint. If the path_index is wrong or -1, then the path_index is automatically calculated.</param>
		public virtual void ResetToVertex(int vertexIndex, int pathIndex)
		{
			m_impl.ResetToVertex(vertexIndex, pathIndex);
		}

		/// <summary>Indicates whether a next segment exists for the path.</summary>
		/// <returns>TRUE is the next segment exists.</returns>
		public virtual bool HasNextSegment()
		{
			return m_impl.HasNextSegment();
		}

		/// <summary>Indicates whether a previous segment exists in the path.</summary>
		/// <returns>TRUE if the previous segment exists.</returns>
		public virtual bool HasPreviousSegment()
		{
			return m_impl.HasPreviousSegment();
		}

		/// <summary>Moves the iterator to the next segment and returns the segment.</summary>
		/// <remarks>
		/// Moves the iterator to the next segment and returns the segment.
		/// The Segment is returned by value and is owned by the iterator.
		/// </remarks>
		public virtual com.epl.geometry.Segment NextSegment()
		{
			return m_impl.NextSegment();
		}

		/// <summary>Moves the iterator to previous segment and returns the segment.</summary>
		/// <remarks>
		/// Moves the iterator to previous segment and returns the segment.
		/// The Segment is returned by value and is owned by the iterator.
		/// </remarks>
		public virtual com.epl.geometry.Segment PreviousSegment()
		{
			return m_impl.PreviousSegment();
		}

		/// <summary>Returns the index of the current path.</summary>
		public virtual int GetPathIndex()
		{
			return m_impl.GetPathIndex();
		}

		/// <summary>Returns the index of the start point of this segment.</summary>
		public virtual int GetStartPointIndex()
		{
			return m_impl.GetStartPointIndex();
		}

		/// <summary>Returns the index of the end point of the current segment.</summary>
		public virtual int GetEndPointIndex()
		{
			return m_impl.GetEndPointIndex();
		}

		/// <summary>Returns TRUE, if the segment is the closing segment of the closed path</summary>
		public virtual bool IsClosingSegment()
		{
			return m_impl.IsClosingSegment();
		}

		/// <summary>Switches the iterator to navigation mode.</summary>
		/// <param name="bYesNo">
		/// If TRUE, the iterator loops over the current path infinitely
		/// (unless the multipath is empty).
		/// </param>
		public virtual void SetCirculator(bool bYesNo)
		{
			m_impl.SetCirculator(bYesNo);
		}

		/// <summary>Copies this SegmentIterator.</summary>
		/// <returns>SegmentIterator.</returns>
		public virtual object Copy()
		{
			return new com.epl.geometry.SegmentIterator(m_impl.Copy());
		}

		protected internal virtual object _getImpl()
		{
			return (com.epl.geometry.SegmentIteratorImpl)m_impl;
		}
	}
}
