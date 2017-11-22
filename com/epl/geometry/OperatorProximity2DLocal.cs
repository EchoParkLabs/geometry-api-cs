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
	internal class OperatorProximity2DLocal : com.epl.geometry.OperatorProximity2D
	{
		internal class Side_helper
		{
			internal int m_i1;

			internal int m_i2;

			internal bool m_bRight1;

			internal bool m_bRight2;

			internal virtual void Reset()
			{
				this.m_i1 = -1;
				this.m_i2 = -1;
				this.m_bRight1 = false;
				this.m_bRight2 = false;
			}

			internal virtual int Find_non_degenerate(com.epl.geometry.SegmentIterator segIter, int vertexIndex, int pathIndex)
			{
				segIter.ResetToVertex(vertexIndex, pathIndex);
				while (segIter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = segIter.NextSegment();
					double length = segment.CalculateLength2D();
					if (length != 0.0)
					{
						return segIter.GetStartPointIndex();
					}
				}
				segIter.ResetToVertex(vertexIndex, pathIndex);
				while (segIter.HasPreviousSegment())
				{
					com.epl.geometry.Segment segment = segIter.PreviousSegment();
					double length = segment.CalculateLength2D();
					if (length != 0)
					{
						return segIter.GetStartPointIndex();
					}
				}
				return -1;
			}

			internal virtual int Find_prev_non_degenerate(com.epl.geometry.SegmentIterator segIter, int index)
			{
				segIter.ResetToVertex(index, -1);
				while (segIter.HasPreviousSegment())
				{
					com.epl.geometry.Segment segment = segIter.PreviousSegment();
					double length = segment.CalculateLength2D();
					if (length != 0)
					{
						return segIter.GetStartPointIndex();
					}
				}
				return -1;
			}

			internal virtual int Find_next_non_degenerate(com.epl.geometry.SegmentIterator segIter, int index)
			{
				segIter.ResetToVertex(index, -1);
				segIter.NextSegment();
				while (segIter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = segIter.NextSegment();
					double length = segment.CalculateLength2D();
					if (length != 0)
					{
						return segIter.GetStartPointIndex();
					}
				}
				return -1;
			}

			internal virtual void Find_analysis_pair_from_index(com.epl.geometry.Point2D inputPoint, com.epl.geometry.SegmentIterator segIter, int vertexIndex, int pathIndex)
			{
				this.m_i1 = this.Find_non_degenerate(segIter, vertexIndex, pathIndex);
				if (this.m_i1 != -1)
				{
					segIter.ResetToVertex(this.m_i1, -1);
					com.epl.geometry.Segment segment1 = segIter.NextSegment();
					double t1 = segment1.GetClosestCoordinate(inputPoint, false);
					com.epl.geometry.Point2D p1 = segment1.GetCoord2D(t1);
					double d1 = com.epl.geometry.Point2D.SqrDistance(p1, inputPoint);
					com.epl.geometry.Point2D pq = new com.epl.geometry.Point2D();
					pq.SetCoords(p1);
					pq.Sub(segment1.GetStartXY());
					com.epl.geometry.Point2D pr = new com.epl.geometry.Point2D();
					pr.SetCoords(inputPoint);
					pr.Sub(segment1.GetStartXY());
					this.m_bRight1 = (pq.CrossProduct(pr) < 0);
					this.m_i2 = this.Find_next_non_degenerate(segIter, this.m_i1);
					if (this.m_i2 != -1)
					{
						segIter.ResetToVertex(this.m_i2, -1);
						com.epl.geometry.Segment segment2 = segIter.NextSegment();
						double t2 = segment2.GetClosestCoordinate(inputPoint, false);
						com.epl.geometry.Point2D p2 = segment2.GetCoord2D(t2);
						double d2 = com.epl.geometry.Point2D.SqrDistance(p2, inputPoint);
						if (d2 > d1)
						{
							this.m_i2 = -1;
						}
						else
						{
							pq.SetCoords(p2);
							pq.Sub(segment2.GetStartXY());
							pr.SetCoords(inputPoint);
							pr.Sub(segment2.GetStartXY());
							this.m_bRight2 = (pq.CrossProduct(pr) < 0);
						}
					}
					if (this.m_i2 == -1)
					{
						this.m_i2 = this.Find_prev_non_degenerate(segIter, this.m_i1);
						if (this.m_i2 != -1)
						{
							segIter.ResetToVertex(this.m_i2, -1);
							com.epl.geometry.Segment segment2 = segIter.NextSegment();
							double t2 = segment2.GetClosestCoordinate(inputPoint, false);
							com.epl.geometry.Point2D p2 = segment2.GetCoord2D(t2);
							double d2 = com.epl.geometry.Point2D.SqrDistance(p2, inputPoint);
							if (d2 > d1)
							{
								this.m_i2 = -1;
							}
							else
							{
								pq.SetCoords(p2);
								pq.Sub(segment2.GetStartXY());
								pr.SetCoords(inputPoint);
								pr.Sub(segment2.GetStartXY());
								this.m_bRight2 = (pq.CrossProduct(pr) < 0);
								int itemp = this.m_i1;
								this.m_i1 = this.m_i2;
								this.m_i2 = itemp;
								bool btemp = this.m_bRight1;
								this.m_bRight1 = this.m_bRight2;
								this.m_bRight2 = btemp;
							}
						}
					}
				}
			}

			// Try to find two segements that are not degenerate
			internal virtual bool Calc_side(com.epl.geometry.Point2D inputPoint, bool bRight, com.epl.geometry.MultiPath multipath, int vertexIndex, int pathIndex)
			{
				com.epl.geometry.SegmentIterator segIter = multipath.QuerySegmentIterator();
				this.Find_analysis_pair_from_index(inputPoint, segIter, vertexIndex, pathIndex);
				if (this.m_i1 != -1 && this.m_i2 == -1)
				{
					// could not find a pair of segments
					return this.m_bRight1;
				}
				if (this.m_i1 != -1 && this.m_i2 != -1)
				{
					if (this.m_bRight1 == this.m_bRight2)
					{
						return this.m_bRight1;
					}
					else
					{
						// no conflicting result for the side
						// the conflicting result, that we are trying to resolve,
						// happens in the obtuse (outer) side of the turn only.
						segIter.ResetToVertex(this.m_i1, -1);
						com.epl.geometry.Segment segment1 = segIter.NextSegment();
						com.epl.geometry.Point2D tang1 = segment1._getTangent(1.0);
						segIter.ResetToVertex(this.m_i2, -1);
						com.epl.geometry.Segment segment2 = segIter.NextSegment();
						com.epl.geometry.Point2D tang2 = segment2._getTangent(0.0);
						double cross = tang1.CrossProduct(tang2);
						if (cross >= 0)
						{
							// the obtuse angle is on the right side
							return true;
						}
						else
						{
							// the obtuse angle is on the right side
							return false;
						}
					}
				}
				else
				{
					System.Diagnostics.Debug.Assert((this.m_i1 == -1 && this.m_i2 == -1));
					return bRight;
				}
			}

			internal Side_helper(OperatorProximity2DLocal _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly OperatorProximity2DLocal _enclosing;
			// could not resolve the side. So just return the
			// old value.
		}

		public override com.epl.geometry.Proximity2DResult GetNearestCoordinate(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint, bool bTestPolygonInterior)
		{
			return GetNearestCoordinate(geom, inputPoint, bTestPolygonInterior, false);
		}

		public override com.epl.geometry.Proximity2DResult GetNearestCoordinate(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint, bool bTestPolygonInterior, bool bCalculateLeftRightSide)
		{
			if (geom.IsEmpty())
			{
				return new com.epl.geometry.Proximity2DResult();
			}
			com.epl.geometry.Point2D inputPoint2D = inputPoint.GetXY();
			com.epl.geometry.Geometry proxmityTestGeom = geom;
			int gt = geom.GetType().Value();
			if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddEnvelope((com.epl.geometry.Envelope)geom, false);
				proxmityTestGeom = polygon;
				gt = com.epl.geometry.Geometry.GeometryType.Polygon;
			}
			switch (gt)
			{
				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					return PointGetNearestVertex((com.epl.geometry.Point)proxmityTestGeom, inputPoint2D);
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					return MultiVertexGetNearestVertex((com.epl.geometry.MultiVertexGeometry)proxmityTestGeom, inputPoint2D);
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					return MultiPathGetNearestCoordinate((com.epl.geometry.MultiPath)proxmityTestGeom, inputPoint2D, bTestPolygonInterior, bCalculateLeftRightSide);
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("not implemented");
				}
			}
		}

		public override com.epl.geometry.Proximity2DResult GetNearestVertex(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint)
		{
			if (geom.IsEmpty())
			{
				return new com.epl.geometry.Proximity2DResult();
			}
			com.epl.geometry.Point2D inputPoint2D = inputPoint.GetXY();
			com.epl.geometry.Geometry proxmityTestGeom = geom;
			int gt = geom.GetType().Value();
			if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddEnvelope((com.epl.geometry.Envelope)geom, false);
				proxmityTestGeom = polygon;
				gt = com.epl.geometry.Geometry.GeometryType.Polygon;
			}
			switch (gt)
			{
				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					return PointGetNearestVertex((com.epl.geometry.Point)proxmityTestGeom, inputPoint2D);
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				case com.epl.geometry.Geometry.GeometryType.Polyline:
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					return MultiVertexGetNearestVertex((com.epl.geometry.MultiVertexGeometry)proxmityTestGeom, inputPoint2D);
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("not implemented");
				}
			}
		}

		public override com.epl.geometry.Proximity2DResult[] GetNearestVertices(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint, double searchRadius, int maxVertexCountToReturn)
		{
			if (maxVertexCountToReturn < 0)
			{
				throw new System.ArgumentException();
			}
			if (geom.IsEmpty())
			{
				return new com.epl.geometry.Proximity2DResult[] {  };
			}
			com.epl.geometry.Point2D inputPoint2D = inputPoint.GetXY();
			com.epl.geometry.Geometry proxmityTestGeom = geom;
			int gt = geom.GetType().Value();
			if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddEnvelope((com.epl.geometry.Envelope)geom, false);
				proxmityTestGeom = polygon;
				gt = com.epl.geometry.Geometry.GeometryType.Polygon;
			}
			switch (gt)
			{
				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					return PointGetNearestVertices((com.epl.geometry.Point)proxmityTestGeom, inputPoint2D, searchRadius, maxVertexCountToReturn);
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				case com.epl.geometry.Geometry.GeometryType.Polyline:
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					return MultiVertexGetNearestVertices((com.epl.geometry.MultiVertexGeometry)proxmityTestGeom, inputPoint2D, searchRadius, maxVertexCountToReturn);
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("not implemented");
				}
			}
		}

		internal virtual com.epl.geometry.Proximity2DResult MultiPathGetNearestCoordinate(com.epl.geometry.MultiPath geom, com.epl.geometry.Point2D inputPoint, bool bTestPolygonInterior, bool bCalculateLeftRightSide)
		{
			if (geom.GetType() == com.epl.geometry.Geometry.Type.Polygon && bTestPolygonInterior)
			{
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				geom.QueryEnvelope2D(env);
				double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(null, env, false);
				com.epl.geometry.PolygonUtils.PiPResult pipResult;
				if (bCalculateLeftRightSide)
				{
					pipResult = com.epl.geometry.PolygonUtils.IsPointInPolygon2D((com.epl.geometry.Polygon)geom, inputPoint, 0.0);
				}
				else
				{
					pipResult = com.epl.geometry.PolygonUtils.IsPointInPolygon2D((com.epl.geometry.Polygon)geom, inputPoint, tolerance);
				}
				if (pipResult != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
				{
					com.epl.geometry.Proximity2DResult result = new com.epl.geometry.Proximity2DResult(inputPoint, 0, 0.0);
					if (bCalculateLeftRightSide)
					{
						result.SetRightSide(true);
					}
					return result;
				}
			}
			com.epl.geometry.SegmentIterator segIter = geom.QuerySegmentIterator();
			com.epl.geometry.Point2D closest = new com.epl.geometry.Point2D();
			int closestVertexIndex = -1;
			int closestPathIndex = -1;
			double closestDistanceSq = com.epl.geometry.NumberUtils.DoubleMax();
			bool bRight = false;
			int num_candidates = 0;
			while (segIter.NextPath())
			{
				while (segIter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = segIter.NextSegment();
					double t = segment.GetClosestCoordinate(inputPoint, false);
					com.epl.geometry.Point2D point = segment.GetCoord2D(t);
					double distanceSq = com.epl.geometry.Point2D.SqrDistance(point, inputPoint);
					if (distanceSq < closestDistanceSq)
					{
						num_candidates = 1;
						closest = point;
						closestVertexIndex = segIter.GetStartPointIndex();
						closestPathIndex = segIter.GetPathIndex();
						closestDistanceSq = distanceSq;
					}
					else
					{
						if (distanceSq == closestDistanceSq)
						{
							num_candidates++;
						}
					}
				}
			}
			com.epl.geometry.Proximity2DResult result_1 = new com.epl.geometry.Proximity2DResult(closest, closestVertexIndex, System.Math.Sqrt(closestDistanceSq));
			if (bCalculateLeftRightSide)
			{
				segIter.ResetToVertex(closestVertexIndex, closestPathIndex);
				com.epl.geometry.Segment segment = segIter.NextSegment();
				bRight = (com.epl.geometry.Point2D.OrientationRobust(inputPoint, segment.GetStartXY(), segment.GetEndXY()) < 0);
				if (num_candidates > 1)
				{
					com.epl.geometry.OperatorProximity2DLocal.Side_helper sideHelper = new com.epl.geometry.OperatorProximity2DLocal.Side_helper(this);
					sideHelper.Reset();
					bRight = sideHelper.Calc_side(inputPoint, bRight, geom, closestVertexIndex, closestPathIndex);
				}
				result_1.SetRightSide(bRight);
			}
			return result_1;
		}

		internal virtual com.epl.geometry.Proximity2DResult PointGetNearestVertex(com.epl.geometry.Point geom, com.epl.geometry.Point2D input_point)
		{
			com.epl.geometry.Point2D pt = geom.GetXY();
			double distance = com.epl.geometry.Point2D.Distance(pt, input_point);
			return new com.epl.geometry.Proximity2DResult(pt, 0, distance);
		}

		internal virtual com.epl.geometry.Proximity2DResult MultiVertexGetNearestVertex(com.epl.geometry.MultiVertexGeometry geom, com.epl.geometry.Point2D inputPoint)
		{
			com.epl.geometry.MultiVertexGeometryImpl mpImpl = (com.epl.geometry.MultiVertexGeometryImpl)geom._getImpl();
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef((com.epl.geometry.VertexDescription.Semantics.POSITION));
			int pointCount = geom.GetPointCount();
			int closestIndex = 0;
			double closestx = 0.0;
			double closesty = 0.0;
			double closestDistanceSq = com.epl.geometry.NumberUtils.DoubleMax();
			for (int i = 0; i < pointCount; i++)
			{
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				position.Read(2 * i, pt);
				double distanceSq = com.epl.geometry.Point2D.SqrDistance(pt, inputPoint);
				if (distanceSq < closestDistanceSq)
				{
					closestx = pt.x;
					closesty = pt.y;
					closestIndex = i;
					closestDistanceSq = distanceSq;
				}
			}
			com.epl.geometry.Proximity2DResult result = new com.epl.geometry.Proximity2DResult();
			result._setParams(closestx, closesty, closestIndex, System.Math.Sqrt(closestDistanceSq));
			return result;
		}

		internal virtual com.epl.geometry.Proximity2DResult[] PointGetNearestVertices(com.epl.geometry.Point geom, com.epl.geometry.Point2D inputPoint, double searchRadius, int maxVertexCountToReturn)
		{
			com.epl.geometry.Proximity2DResult[] resultArray;
			if (maxVertexCountToReturn == 0)
			{
				resultArray = new com.epl.geometry.Proximity2DResult[] {  };
				return resultArray;
			}
			double searchRadiusSq = searchRadius * searchRadius;
			com.epl.geometry.Point2D pt = geom.GetXY();
			double distanceSq = com.epl.geometry.Point2D.SqrDistance(pt, inputPoint);
			if (distanceSq <= searchRadiusSq)
			{
				resultArray = new com.epl.geometry.Proximity2DResult[1];
				com.epl.geometry.Proximity2DResult result = new com.epl.geometry.Proximity2DResult();
				result._setParams(pt.x, pt.y, 0, System.Math.Sqrt(distanceSq));
				resultArray[0] = result;
			}
			else
			{
				resultArray = new com.epl.geometry.Proximity2DResult[0];
			}
			return resultArray;
		}

		internal virtual com.epl.geometry.Proximity2DResult[] MultiVertexGetNearestVertices(com.epl.geometry.MultiVertexGeometry geom, com.epl.geometry.Point2D inputPoint, double searchRadius, int maxVertexCountToReturn)
		{
			com.epl.geometry.Proximity2DResult[] resultArray;
			if (maxVertexCountToReturn == 0)
			{
				resultArray = new com.epl.geometry.Proximity2DResult[0];
				return resultArray;
			}
			com.epl.geometry.MultiVertexGeometryImpl mpImpl = (com.epl.geometry.MultiVertexGeometryImpl)geom._getImpl();
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef((com.epl.geometry.VertexDescription.Semantics.POSITION));
			int pointCount = geom.GetPointCount();
			System.Collections.Generic.List<com.epl.geometry.Proximity2DResult> v = new System.Collections.Generic.List<com.epl.geometry.Proximity2DResult>(maxVertexCountToReturn);
			int count = 0;
			double searchRadiusSq = searchRadius * searchRadius;
			for (int i = 0; i < pointCount; i++)
			{
				double x = position.Read(2 * i);
				double y = position.Read(2 * i + 1);
				double xDiff = inputPoint.x - x;
				double yDiff = inputPoint.y - y;
				double distanceSq = xDiff * xDiff + yDiff * yDiff;
				if (distanceSq <= searchRadiusSq)
				{
					com.epl.geometry.Proximity2DResult result = new com.epl.geometry.Proximity2DResult();
					result._setParams(x, y, i, System.Math.Sqrt(distanceSq));
					count++;
					v.Add(result);
				}
			}
			int vsize = v.Count;
			v.Sort(new com.epl.geometry.Proximity2DResultComparator());
			if (maxVertexCountToReturn >= vsize)
			{
				return v.ToArray();
			}
			return v.GetRange(0, maxVertexCountToReturn - 0).ToArray();
		}
		/*
		* if (distanceSq <= searchRadiusSq) { if (count >= maxVertexCountToReturn +
		* 1) { count++; double frontDistance = v.get(0).getDistance(); if
		* (frontDistance * frontDistance <= distanceSq) continue; }
		*
		* Proximity2DResult result = new Proximity2DResult(); result._setParams(x,
		* y, i, Math.sqrt(distanceSq));
		*
		* count++;
		*
		* if (count <= maxVertexCountToReturn) { v.add(result); } // else // { //
		* if (count == maxVertexCountToReturn + 1) // MAKEHEAP(v,
		* Proximity2DResult, Proximity2DResult::_Compare); // // PUSHHEAP(v,
		* result, Proximity2DResult, Proximity2DResult::_Compare); // POPHEAP(v,
		* Proximity2DResult, Proximity2DResult::_Compare); // } } }
		*
		* int vsize = v.size(); Collections.sort(v, new
		* Proximity2DResultComparator());
		*
		* // SORTDYNAMICARRAY(v, Proximity2DResult, 0, vsize,
		* Proximity2DResult::_Compare); resultArray = new Proximity2DResult[vsize];
		* for (int i = 0; i < vsize; i++) { resultArray[i] =
		* (Proximity2DResult)v.get(i); }
		*
		* return resultArray; }
		*/
	}
}
