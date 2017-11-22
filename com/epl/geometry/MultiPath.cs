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
	/// <summary>The MulitPath class is a base class for polygons and polylines.</summary>
	[System.Serializable]
	public abstract class MultiPath : com.epl.geometry.MultiVertexGeometry
	{
		internal com.epl.geometry.MultiPathImpl m_impl;

		public override com.epl.geometry.VertexDescription GetDescription()
		{
			return m_impl.GetDescription();
		}

		internal override void AssignVertexDescription(com.epl.geometry.VertexDescription src)
		{
			m_impl.AssignVertexDescription(src);
		}

		internal override void MergeVertexDescription(com.epl.geometry.VertexDescription src)
		{
			m_impl.MergeVertexDescription(src);
		}

		public override void AddAttribute(int semantics)
		{
			m_impl.AddAttribute(semantics);
		}

		public override void DropAttribute(int semantics)
		{
			m_impl.DropAttribute(semantics);
		}

		public override void DropAllAttributes()
		{
			m_impl.DropAllAttributes();
		}

		public override int GetPointCount()
		{
			return m_impl.GetPointCount();
		}

		public override com.epl.geometry.Point GetPoint(int index)
		{
			return m_impl.GetPoint(index);
		}

		public override void SetPoint(int index, com.epl.geometry.Point point)
		{
			m_impl.SetPoint(index, point);
		}

		public override bool IsEmpty()
		{
			return m_impl.IsEmptyImpl();
		}

		public override double CalculateArea2D()
		{
			return m_impl.CalculateArea2D();
		}

		public override double CalculateLength2D()
		{
			return m_impl.CalculateLength2D();
		}

		public virtual double CalculatePathLength2D(int pathIndex)
		{
			return m_impl.CalculatePathLength2D(pathIndex);
		}

		internal override double GetAttributeAsDbl(int semantics, int index, int ordinate)
		{
			return m_impl.GetAttributeAsDbl(semantics, index, ordinate);
		}

		internal override int GetAttributeAsInt(int semantics, int index, int ordinate)
		{
			return m_impl.GetAttributeAsInt(semantics, index, ordinate);
		}

		internal override void SetAttribute(int semantics, int index, int ordinate, double value)
		{
			m_impl.SetAttribute(semantics, index, ordinate, value);
		}

		internal override void SetAttribute(int semantics, int index, int ordinate, int value)
		{
			m_impl.SetAttribute(semantics, index, ordinate, value);
		}

		public override com.epl.geometry.Point2D GetXY(int index)
		{
			return m_impl.GetXY(index);
		}

		public override void GetXY(int index, com.epl.geometry.Point2D pt)
		{
			m_impl.GetXY(index, pt);
		}

		public override void SetXY(int index, com.epl.geometry.Point2D pt)
		{
			m_impl.SetXY(index, pt);
		}

		internal override com.epl.geometry.Point3D GetXYZ(int index)
		{
			return m_impl.GetXYZ(index);
		}

		internal override void SetXYZ(int index, com.epl.geometry.Point3D pt)
		{
			m_impl.SetXYZ(index, pt);
		}

		public override void QueryEnvelope(com.epl.geometry.Envelope env)
		{
			m_impl.QueryEnvelope(env);
		}

		public override void QueryEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			m_impl.QueryEnvelope2D(env);
		}

		public virtual void QueryPathEnvelope2D(int pathIndex, com.epl.geometry.Envelope2D env)
		{
			m_impl.QueryPathEnvelope2D(pathIndex, env);
		}

		internal override void QueryEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			m_impl.QueryEnvelope3D(env);
		}

		public virtual void QueryLooseEnvelope(com.epl.geometry.Envelope2D env)
		{
			m_impl.QueryLooseEnvelope2D(env);
		}

		internal virtual void QueryLooseEnvelope(com.epl.geometry.Envelope3D env)
		{
			m_impl.QueryLooseEnvelope3D(env);
		}

		public override com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate)
		{
			return m_impl.QueryInterval(semantics, ordinate);
		}

		public override void CopyTo(com.epl.geometry.Geometry dst)
		{
			if (GetType() != dst.GetType())
			{
				throw new System.ArgumentException();
			}
			m_impl.CopyTo((com.epl.geometry.Geometry)dst._getImpl());
		}

		public override com.epl.geometry.Geometry GetBoundary()
		{
			return m_impl.GetBoundary();
		}

		public override void QueryCoordinates(com.epl.geometry.Point2D[] dst)
		{
			m_impl.QueryCoordinates(dst);
		}

		public virtual void QueryCoordinates(com.epl.geometry.Point2D[] dst, int dstSize, int beginIndex, int endIndex)
		{
			m_impl.QueryCoordinates(dst, dstSize, beginIndex, endIndex);
		}

		internal override void QueryCoordinates(com.epl.geometry.Point3D[] dst)
		{
			m_impl.QueryCoordinates(dst);
		}

		public override void QueryCoordinates(com.epl.geometry.Point[] dst)
		{
			m_impl.QueryCoordinates(dst);
		}

		/// <summary>Returns TRUE if the multipath contains non-linear segments.</summary>
		internal virtual bool HasNonLinearSegments()
		{
			return m_impl.HasNonLinearSegments();
		}

		/// <summary>Returns total segment count in the MultiPath.</summary>
		public virtual int GetSegmentCount()
		{
			return m_impl.GetSegmentCount();
		}

		/// <summary>Returns the segment count in the given multipath path.</summary>
		/// <param name="pathIndex">The path to determine the segment.</param>
		/// <returns>The segment of the multipath.</returns>
		public virtual int GetSegmentCount(int pathIndex)
		{
			int segCount = GetPathSize(pathIndex);
			if (!IsClosedPath(pathIndex))
			{
				segCount--;
			}
			return segCount;
		}

		/// <summary>Appends all paths from another multipath.</summary>
		/// <param name="src">The multipath to append to this multipath.</param>
		/// <param name="bReversePaths">
		/// TRUE if the multipath is added should be added with its paths
		/// reversed.
		/// </param>
		public virtual void Add(com.epl.geometry.MultiPath src, bool bReversePaths)
		{
			m_impl.Add((com.epl.geometry.MultiPathImpl)src._getImpl(), bReversePaths);
		}

		/// <summary>Copies a path from another multipath.</summary>
		/// <param name="src">The multipath to copy from.</param>
		/// <param name="srcPathIndex">The index of the path in the the source MultiPath.</param>
		/// <param name="bForward">When FALSE, the points are inserted in reverse order.</param>
		public virtual void AddPath(com.epl.geometry.MultiPath src, int srcPathIndex, bool bForward)
		{
			m_impl.AddPath((com.epl.geometry.MultiPathImpl)src._getImpl(), srcPathIndex, bForward);
		}

		/// <summary>Adds a new path to this multipath.</summary>
		/// <param name="points">The array of points to add to this multipath.</param>
		/// <param name="count">The number of points added to the mulitpath.</param>
		/// <param name="bForward">When FALSE, the points are inserted in reverse order.</param>
		internal virtual void AddPath(com.epl.geometry.Point2D[] points, int count, bool bForward)
		{
			m_impl.AddPath(points, count, bForward);
		}

		/// <summary>Adds a new segment to this multipath.</summary>
		/// <param name="segment">The segment to be added to this mulitpath.</param>
		/// <param name="bStartNewPath">TRUE if a new path will be added.</param>
		public virtual void AddSegment(com.epl.geometry.Segment segment, bool bStartNewPath)
		{
			m_impl.AddSegment(segment, bStartNewPath);
		}

		/// <summary>Reverses the order of the vertices in each path.</summary>
		public virtual void ReverseAllPaths()
		{
			m_impl.ReverseAllPaths();
		}

		/// <summary>Reverses the order of vertices in the path.</summary>
		/// <param name="pathIndex">The start index of the path to reverse the order.</param>
		public virtual void ReversePath(int pathIndex)
		{
			m_impl.ReversePath(pathIndex);
		}

		/// <summary>Removes the path at the given index.</summary>
		/// <param name="pathIndex">The start index to remove the path.</param>
		public virtual void RemovePath(int pathIndex)
		{
			m_impl.RemovePath(pathIndex);
		}

		/// <summary>Inserts a path from another multipath.</summary>
		/// <param name="pathIndex">The start index of the multipath to insert.</param>
		/// <param name="src">
		/// The multipath to insert into this multipath. Can be the same
		/// as the multipath being modified.
		/// </param>
		/// <param name="srcPathIndex">The start index to insert the path into the multipath.</param>
		/// <param name="bForward">When FALSE, the points are inserted in reverse order.</param>
		public virtual void InsertPath(int pathIndex, com.epl.geometry.MultiPath src, int srcPathIndex, bool bForward)
		{
			m_impl.InsertPath(pathIndex, (com.epl.geometry.MultiPathImpl)src._getImpl(), srcPathIndex, bForward);
		}

		/// <summary>Inserts a path from an array of 2D Points.</summary>
		/// <param name="pathIndex">The path index of the multipath to place the new path.</param>
		/// <param name="points">The array of points defining the new path.</param>
		/// <param name="pointsOffset">The offset into the array to start reading.</param>
		/// <param name="count">The number of points to insert into the new path.</param>
		/// <param name="bForward">When FALSE, the points are inserted in reverse order.</param>
		internal virtual void InsertPath(int pathIndex, com.epl.geometry.Point2D[] points, int pointsOffset, int count, bool bForward)
		{
			m_impl.InsertPath(pathIndex, points, pointsOffset, count, bForward);
		}

		/// <summary>Inserts vertices from the given multipath into this multipath.</summary>
		/// <remarks>
		/// Inserts vertices from the given multipath into this multipath. All added
		/// vertices are connected by linear segments with each other and with the
		/// existing vertices.
		/// </remarks>
		/// <param name="pathIndex">
		/// The path index in this multipath to insert points to. Must
		/// correspond to an existing path.
		/// </param>
		/// <param name="beforePointIndex">
		/// The point index before all other vertices to insert in the
		/// given path of this multipath. This value must be between 0 and
		/// GetPathSize(pathIndex), or -1 to insert points at the end of
		/// the given path.
		/// </param>
		/// <param name="src">The source multipath.</param>
		/// <param name="srcPathIndex">The source path index to copy points from.</param>
		/// <param name="srcPointIndexFrom">The start point in the source path to start copying from.</param>
		/// <param name="srcPointCount">The count of points to add.</param>
		/// <param name="bForward">When FALSE, the points are inserted in reverse order.</param>
		public virtual void InsertPoints(int pathIndex, int beforePointIndex, com.epl.geometry.MultiPath src, int srcPathIndex, int srcPointIndexFrom, int srcPointCount, bool bForward)
		{
			m_impl.InsertPoints(pathIndex, beforePointIndex, (com.epl.geometry.MultiPathImpl)src._getImpl(), srcPathIndex, srcPointIndexFrom, srcPointCount, bForward);
		}

		/// <summary>Inserts a part of a path from the given array.</summary>
		/// <param name="pathIndex">
		/// The path index in this class to insert points to. Must
		/// correspond to an existing path.
		/// </param>
		/// <param name="beforePointIndex">
		/// The point index in the given path of this MultiPath before
		/// which the vertices need to be inserted. This value must be
		/// between 0 and GetPathSize(pathIndex), or -1 to insert points
		/// at the end of the given path.
		/// </param>
		/// <param name="src">The source array</param>
		/// <param name="srcPointIndexFrom">The start point in the source array to start copying from.</param>
		/// <param name="srcPointCount">The count of points to add.</param>
		/// <param name="bForward">When FALSE, the points are inserted in reverse order.</param>
		internal virtual void InsertPoints(int pathIndex, int beforePointIndex, com.epl.geometry.Point2D[] src, int srcPointIndexFrom, int srcPointCount, bool bForward)
		{
			m_impl.InsertPoints(pathIndex, beforePointIndex, src, srcPointIndexFrom, srcPointCount, bForward);
		}

		/// <summary>Inserts a point.</summary>
		/// <param name="pathIndex">
		/// The path index in this class to insert the point to. Must
		/// correspond to an existing path.
		/// </param>
		/// <param name="beforePointIndex">
		/// The point index in the given path of this multipath. This
		/// value must be between 0 and GetPathSize(pathIndex), or -1 to
		/// insert the point at the end of the given path.
		/// </param>
		/// <param name="pt">The point to be inserted.</param>
		internal virtual void InsertPoint(int pathIndex, int beforePointIndex, com.epl.geometry.Point2D pt)
		{
			m_impl.InsertPoint(pathIndex, beforePointIndex, pt);
		}

		/// <summary>Inserts a point.</summary>
		/// <param name="pathIndex">
		/// The path index in this class to insert the point to. Must
		/// correspond to an existing path.
		/// </param>
		/// <param name="beforePointIndex">
		/// The point index in the given path of this multipath. This
		/// value must be between 0 and GetPathSize(pathIndex), or -1 to
		/// insert the point at the end of the given path.
		/// </param>
		/// <param name="pt">The point to be inserted.</param>
		public virtual void InsertPoint(int pathIndex, int beforePointIndex, com.epl.geometry.Point pt)
		{
			m_impl.InsertPoint(pathIndex, beforePointIndex, pt);
		}

		/// <summary>Removes a point at a given index.</summary>
		/// <param name="pathIndex">The path from whom to remove the point.</param>
		/// <param name="pointIndex">The index of the point to be removed.</param>
		public virtual void RemovePoint(int pathIndex, int pointIndex)
		{
			m_impl.RemovePoint(pathIndex, pointIndex);
		}

		/// <summary>Returns the number of paths in this multipath.</summary>
		/// <returns>The number of paths in this multipath.</returns>
		public virtual int GetPathCount()
		{
			return m_impl.GetPathCount();
		}

		/// <summary>Returns the number of vertices in a path.</summary>
		/// <param name="pathIndex">The index of the path to return the number of vertices from.</param>
		/// <returns>The number of vertices in a path.</returns>
		public virtual int GetPathSize(int pathIndex)
		{
			return m_impl.GetPathSize(pathIndex);
		}

		/// <summary>Returns the start index of the path.</summary>
		/// <param name="pathIndex">The index of the path to return the start index from.</param>
		/// <returns>The start index of the path.</returns>
		public virtual int GetPathStart(int pathIndex)
		{
			return m_impl.GetPathStart(pathIndex);
		}

		/// <summary>Returns the index immediately following the last index of the path.</summary>
		/// <param name="pathIndex">The index of the path to return the end index from.</param>
		/// <returns>Integer index after last index of path</returns>
		public virtual int GetPathEnd(int pathIndex)
		{
			return m_impl.GetPathEnd(pathIndex);
		}

		/// <summary>Returns the path index from the point index.</summary>
		/// <remarks>Returns the path index from the point index. This is O(log N) operation.</remarks>
		/// <param name="pointIndex">The index of the point.</param>
		/// <returns>The index of the path.</returns>
		public virtual int GetPathIndexFromPointIndex(int pointIndex)
		{
			return m_impl.GetPathIndexFromPointIndex(pointIndex);
		}

		/// <summary>Starts a new path at given coordinates.</summary>
		/// <param name="x">The X coordinate of the start point.</param>
		/// <param name="y">The Y coordinate of the start point.</param>
		public virtual void StartPath(double x, double y)
		{
			m_impl.StartPath(x, y);
		}

		internal virtual void StartPath(com.epl.geometry.Point2D point)
		{
			m_impl.StartPath(point);
		}

		internal virtual void StartPath(com.epl.geometry.Point3D point)
		{
			m_impl.StartPath(point);
		}

		/// <summary>Starts a new path at a point.</summary>
		/// <param name="point">The point to start the path from.</param>
		public virtual void StartPath(com.epl.geometry.Point point)
		{
			m_impl.StartPath(point);
		}

		/// <summary>Adds a line segment from the last point to the given end coordinates.</summary>
		/// <param name="x">The X coordinate to the end point.</param>
		/// <param name="y">The Y coordinate to the end point.</param>
		public virtual void LineTo(double x, double y)
		{
			m_impl.LineTo(x, y);
		}

		internal virtual void LineTo(com.epl.geometry.Point2D endPoint)
		{
			m_impl.LineTo(endPoint);
		}

		internal virtual void LineTo(com.epl.geometry.Point3D endPoint)
		{
			m_impl.LineTo(endPoint);
		}

		/// <summary>Adds a Line Segment to the given end point.</summary>
		/// <param name="endPoint">
		/// The end point to which the newly added line segment should
		/// point.
		/// </param>
		public virtual void LineTo(com.epl.geometry.Point endPoint)
		{
			m_impl.LineTo(endPoint);
		}

		/// <summary>Adds a Cubic Bezier Segment to the current Path.</summary>
		/// <remarks>
		/// Adds a Cubic Bezier Segment to the current Path. The Bezier Segment
		/// connects the current last Point and the given endPoint.
		/// </remarks>
		internal virtual void BezierTo(com.epl.geometry.Point2D controlPoint1, com.epl.geometry.Point2D controlPoint2, com.epl.geometry.Point2D endPoint)
		{
			m_impl.BezierTo(controlPoint1, controlPoint2, endPoint);
		}

		/// <summary>Closes the last path of this multipath with a line segment.</summary>
		/// <remarks>
		/// Closes the last path of this multipath with a line segment. The closing
		/// segment is a segment that connects the last and the first points of the
		/// path. This is a virtual segment. The first point is not duplicated to
		/// close the path.
		/// Call this method only for polylines. For polygons this method is
		/// implicitly called for the Polygon class.
		/// </remarks>
		public virtual void ClosePathWithLine()
		{
			m_impl.ClosePathWithLine();
		}

		/// <summary>Closes last path of the MultiPath with the Bezier Segment.</summary>
		/// <remarks>
		/// Closes last path of the MultiPath with the Bezier Segment.
		/// The start point of the Bezier is the last point of the path and the last
		/// point of the bezier is the first point of the path.
		/// </remarks>
		internal virtual void ClosePathWithBezier(com.epl.geometry.Point2D controlPoint1, com.epl.geometry.Point2D controlPoint2)
		{
			m_impl.ClosePathWithBezier(controlPoint1, controlPoint2);
		}

		/// <summary>Closes last path of the MultiPath with the Arc Segment.</summary>
		internal virtual void ClosePathWithArc()
		{
			throw new System.Exception("not implemented");
		}

		/// <summary>
		/// Closes all open paths by adding an implicit line segment from the end
		/// point to the start point.
		/// </summary>
		/// <remarks>
		/// Closes all open paths by adding an implicit line segment from the end
		/// point to the start point. Call this method only for polylines.For
		/// polygons this method is implicitly called for the Polygon class.
		/// </remarks>
		public virtual void CloseAllPaths()
		{
			m_impl.CloseAllPaths();
		}

		/// <summary>Indicates if the given path is closed (represents a ring).</summary>
		/// <remarks>
		/// Indicates if the given path is closed (represents a ring). A closed path
		/// has a virtual segment that connects the last and the first points of the
		/// path. The first point is not duplicated to close the path. Polygons
		/// always have all paths closed.
		/// </remarks>
		/// <param name="pathIndex">The index of the path to check to be closed.</param>
		/// <returns>TRUE if the given path is closed (represents a Ring).</returns>
		public virtual bool IsClosedPath(int pathIndex)
		{
			return m_impl.IsClosedPath(pathIndex);
		}

		public virtual bool IsClosedPathInXYPlane(int pathIndex)
		{
			return m_impl.IsClosedPathInXYPlane(pathIndex);
		}

		/// <summary>Returns TRUE if the given path might have non-linear segments.</summary>
		internal virtual bool HasNonLinearSegments(int pathIndex)
		{
			return m_impl.HasNonLinearSegments(pathIndex);
		}

		/// <summary>Adds a rectangular closed Path to the MultiPathImpl.</summary>
		/// <param name="envSrc">is the source rectangle.</param>
		/// <param name="bReverse">Creates reversed path.</param>
		public virtual void AddEnvelope(com.epl.geometry.Envelope2D envSrc, bool bReverse)
		{
			m_impl.AddEnvelope(envSrc, bReverse);
		}

		/// <summary>Adds a rectangular closed path to this multipath.</summary>
		/// <param name="envSrc">Is the envelope to add to this mulitpath.</param>
		/// <param name="bReverse">Adds the path reversed (counter-clockwise).</param>
		public virtual void AddEnvelope(com.epl.geometry.Envelope envSrc, bool bReverse)
		{
			m_impl.AddEnvelope(envSrc, bReverse);
		}

		/// <summary>
		/// Returns a SegmentIterator that is set right before the beginning of the
		/// multipath.
		/// </summary>
		/// <remarks>
		/// Returns a SegmentIterator that is set right before the beginning of the
		/// multipath. Calling nextPath() will set the iterator to the first path of
		/// this multipath.
		/// </remarks>
		/// <returns>The SegmentIterator for this mulitpath.</returns>
		public virtual com.epl.geometry.SegmentIterator QuerySegmentIterator()
		{
			return new com.epl.geometry.SegmentIterator(m_impl.QuerySegmentIterator());
		}

		/// <summary>
		/// Returns a SegmentIterator that is set to a specific vertex of the
		/// MultiPath.
		/// </summary>
		/// <remarks>
		/// Returns a SegmentIterator that is set to a specific vertex of the
		/// MultiPath. The call to nextSegment() will return the segment that starts
		/// at the vertex. Calling PreviousSegment () will return the segment that
		/// starts at the previous vertex.
		/// </remarks>
		/// <param name="startVertexIndex">The start index of the SegementIterator.</param>
		/// <returns>The SegmentIterator for this mulitpath at the specified vertex.</returns>
		public virtual com.epl.geometry.SegmentIterator QuerySegmentIteratorAtVertex(int startVertexIndex)
		{
			return new com.epl.geometry.SegmentIterator(m_impl.QuerySegmentIteratorAtVertex(startVertexIndex));
		}

		public override void SetEmpty()
		{
			m_impl.SetEmpty();
		}

		public override void ApplyTransformation(com.epl.geometry.Transformation2D transform)
		{
			m_impl.ApplyTransformation(transform);
		}

		internal override void ApplyTransformation(com.epl.geometry.Transformation3D transform)
		{
			m_impl.ApplyTransformation(transform);
		}

		protected internal override object _getImpl()
		{
			return m_impl;
		}

		/// <summary>Returns the hash code for the multipath.</summary>
		public override int GetHashCode()
		{
			return m_impl.GetHashCode();
		}

		internal override void GetPointByVal(int index, com.epl.geometry.Point outPoint)
		{
			m_impl.GetPointByVal(index, outPoint);
		}

		internal override void SetPointByVal(int index, com.epl.geometry.Point point)
		{
			m_impl.SetPointByVal(index, point);
		}

		public override int GetStateFlag()
		{
			return m_impl.GetStateFlag();
		}

		public override void ReplaceNaNs(int semantics, double value)
		{
			m_impl.ReplaceNaNs(semantics, value);
		}
	}
}
