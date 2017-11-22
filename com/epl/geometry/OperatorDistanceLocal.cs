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
	internal class OperatorDistanceLocal : com.epl.geometry.OperatorDistance
	{
		/// <summary>Performs the Distance operation on two geometries</summary>
		/// <returns>Returns a double.</returns>
		public override double Execute(com.epl.geometry.Geometry geom1, com.epl.geometry.Geometry geom2, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (null == geom1 || null == geom2)
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.Geometry geometryA = geom1;
			com.epl.geometry.Geometry geometryB = geom2;
			if (geometryA.IsEmpty() || geometryB.IsEmpty())
			{
				return com.epl.geometry.NumberUtils.TheNaN;
			}
			com.epl.geometry.Polygon polygonA;
			com.epl.geometry.Polygon polygonB;
			com.epl.geometry.MultiPoint multiPointA;
			com.epl.geometry.MultiPoint multiPointB;
			// if geometryA is an envelope use a polygon instead (if geom1 was
			// folded, then geometryA will already be a polygon)
			// if geometryA is a point use a multipoint instead
			com.epl.geometry.Geometry.Type gtA = geometryA.GetType();
			com.epl.geometry.Geometry.Type gtB = geometryB.GetType();
			if (gtA == com.epl.geometry.Geometry.Type.Point)
			{
				if (gtB == com.epl.geometry.Geometry.Type.Point)
				{
					return com.epl.geometry.Point2D.Distance(((com.epl.geometry.Point)geometryA).GetXY(), ((com.epl.geometry.Point)geometryB).GetXY());
				}
				else
				{
					if (gtB == com.epl.geometry.Geometry.Type.Envelope)
					{
						com.epl.geometry.Envelope2D envB = new com.epl.geometry.Envelope2D();
						geometryB.QueryEnvelope2D(envB);
						return envB.Distance(((com.epl.geometry.Point)geometryA).GetXY());
					}
				}
				multiPointA = new com.epl.geometry.MultiPoint();
				multiPointA.Add((com.epl.geometry.Point)geometryA);
				geometryA = multiPointA;
			}
			else
			{
				if (gtA == com.epl.geometry.Geometry.Type.Envelope)
				{
					if (gtB == com.epl.geometry.Geometry.Type.Envelope)
					{
						com.epl.geometry.Envelope2D envA = new com.epl.geometry.Envelope2D();
						geometryA.QueryEnvelope2D(envA);
						com.epl.geometry.Envelope2D envB = new com.epl.geometry.Envelope2D();
						geometryB.QueryEnvelope2D(envB);
						return envB.Distance(envA);
					}
					polygonA = new com.epl.geometry.Polygon();
					polygonA.AddEnvelope((com.epl.geometry.Envelope)geometryA, false);
					geometryA = polygonA;
				}
			}
			// if geom_2 is an envelope use a polygon instead
			// if geom_2 is a point use a multipoint instead
			if (gtB == com.epl.geometry.Geometry.Type.Point)
			{
				multiPointB = new com.epl.geometry.MultiPoint();
				multiPointB.Add((com.epl.geometry.Point)geometryB);
				geometryB = multiPointB;
			}
			else
			{
				if (gtB == com.epl.geometry.Geometry.Type.Envelope)
				{
					polygonB = new com.epl.geometry.Polygon();
					polygonB.AddEnvelope((com.epl.geometry.Envelope)geometryB, false);
					geometryB = polygonB;
				}
			}
			com.epl.geometry.OperatorDistanceLocal.DistanceCalculator distanceCalculator = new com.epl.geometry.OperatorDistanceLocal.DistanceCalculator(this, progressTracker);
			double distance = distanceCalculator.Calculate(geometryA, geometryB);
			return distance;
		}

		internal class DistanceCalculator
		{
			private com.epl.geometry.ProgressTracker m_progressTracker;

			private com.epl.geometry.Envelope2D m_env2DgeometryA;

			private com.epl.geometry.Envelope2D m_env2DgeometryB;

			// Implementation of distance algorithm.
			private void SwapEnvelopes_()
			{
				double temp;
				// swap xmin
				temp = this.m_env2DgeometryA.xmin;
				this.m_env2DgeometryA.xmin = this.m_env2DgeometryB.xmin;
				this.m_env2DgeometryB.xmin = temp;
				// swap xmax
				temp = this.m_env2DgeometryA.xmax;
				this.m_env2DgeometryA.xmax = this.m_env2DgeometryB.xmax;
				this.m_env2DgeometryB.xmax = temp;
				// swap ymin
				temp = this.m_env2DgeometryA.ymin;
				this.m_env2DgeometryA.ymin = this.m_env2DgeometryB.ymin;
				this.m_env2DgeometryB.ymin = temp;
				// swap ymax
				temp = this.m_env2DgeometryA.ymax;
				this.m_env2DgeometryA.ymax = this.m_env2DgeometryB.ymax;
				this.m_env2DgeometryB.ymax = temp;
			}

			private double ExecuteBruteForce_(com.epl.geometry.Geometry geometryA, com.epl.geometry.Geometry geometryB)
			{
				/* const */
				/* const */
				if ((this.m_progressTracker != null) && !(this.m_progressTracker.Progress(-1, -1)))
				{
					throw new System.Exception("user_canceled");
				}
				bool geometriesAreDisjoint = !this.m_env2DgeometryA.IsIntersecting(this.m_env2DgeometryB);
				if (com.epl.geometry.Geometry.IsMultiPath(geometryA.GetType().Value()) && com.epl.geometry.Geometry.IsMultiPath(geometryB.GetType().Value()))
				{
					// MultiPath
					// vs.
					// MultiPath
					// choose
					// the
					// multipath
					// with
					// more
					// points
					// to
					// be
					// geometryA,
					// this
					// way
					// more
					// of
					// geometryA
					// segments
					// can
					// be
					// disqualified
					// more
					// quickly
					// by
					// testing
					// segmentA
					// envelope
					// vs.
					// geometryB
					// envelope
					if (((com.epl.geometry.MultiPath)geometryA).GetPointCount() > ((com.epl.geometry.MultiPath)geometryB).GetPointCount())
					{
						return this.BruteForceMultiPathMultiPath_((com.epl.geometry.MultiPath)geometryA, (com.epl.geometry.MultiPath)geometryB, geometriesAreDisjoint);
					}
					this.SwapEnvelopes_();
					double answer = this.BruteForceMultiPathMultiPath_((com.epl.geometry.MultiPath)geometryB, (com.epl.geometry.MultiPath)geometryA, geometriesAreDisjoint);
					this.SwapEnvelopes_();
					return answer;
				}
				else
				{
					if (geometryA.GetType() == com.epl.geometry.Geometry.Type.MultiPoint && com.epl.geometry.Geometry.IsMultiPath(geometryB.GetType().Value()))
					{
						// MultiPoint
						// vs.
						// MultiPath
						this.SwapEnvelopes_();
						double answer = this.BruteForceMultiPathMultiPoint_((com.epl.geometry.MultiPath)geometryB, (com.epl.geometry.MultiPoint)geometryA, geometriesAreDisjoint);
						this.SwapEnvelopes_();
						return answer;
					}
					else
					{
						if (geometryB.GetType() == com.epl.geometry.Geometry.Type.MultiPoint && com.epl.geometry.Geometry.IsMultiPath(geometryA.GetType().Value()))
						{
							// MultiPath
							// vs.
							// MultiPoint
							return this.BruteForceMultiPathMultiPoint_((com.epl.geometry.MultiPath)geometryA, (com.epl.geometry.MultiPoint)geometryB, geometriesAreDisjoint);
						}
						else
						{
							if (geometryA.GetType() == com.epl.geometry.Geometry.Type.MultiPoint && geometryB.GetType() == com.epl.geometry.Geometry.Type.MultiPoint)
							{
								// MultiPoint
								// vs.
								// MultiPoint
								// choose
								// the
								// multipoint
								// with
								// more
								// vertices
								// to
								// be
								// the
								// "geometryA",
								// this
								// way
								// more
								// points
								// can
								// be
								// potentially
								// excluded
								// by
								// envelope
								// distance
								// tests.
								if (((com.epl.geometry.MultiPoint)geometryA).GetPointCount() > ((com.epl.geometry.MultiPoint)geometryB).GetPointCount())
								{
									return this.BruteForceMultiPointMultiPoint_((com.epl.geometry.MultiPoint)geometryA, (com.epl.geometry.MultiPoint)geometryB, geometriesAreDisjoint);
								}
								this.SwapEnvelopes_();
								double answer = this.BruteForceMultiPointMultiPoint_((com.epl.geometry.MultiPoint)geometryB, (com.epl.geometry.MultiPoint)geometryA, geometriesAreDisjoint);
								this.SwapEnvelopes_();
								return answer;
							}
						}
					}
				}
				return 0.0;
			}

			private double BruteForceMultiPathMultiPath_(com.epl.geometry.MultiPath geometryA, com.epl.geometry.MultiPath geometryB, bool geometriesAreDisjoint)
			{
				/* const */
				/* const */
				// It may be beneficial to have the geometry with less vertices
				// always be geometryA.
				com.epl.geometry.SegmentIterator segIterA = geometryA.QuerySegmentIterator();
				com.epl.geometry.SegmentIterator segIterB = geometryB.QuerySegmentIterator();
				com.epl.geometry.Envelope2D env2DSegmentA = new com.epl.geometry.Envelope2D();
				com.epl.geometry.Envelope2D env2DSegmentB = new com.epl.geometry.Envelope2D();
				double minSqrDistance = com.epl.geometry.NumberUtils.DoubleMax();
				if (!geometriesAreDisjoint)
				{
					// Geometries might be non-disjoint. Check if they intersect
					// using point-in-polygon tests
					if (this.WeakIntersectionTest_(geometryA, geometryB, segIterA, segIterB))
					{
						return 0.0;
					}
				}
				// if geometries are known disjoint, don't bother to do any tests
				// for polygon containment
				// nested while-loop insanity
				while (segIterA.NextPath())
				{
					while (segIterA.HasNextSegment())
					{
						/* const */
						com.epl.geometry.Segment segmentA = segIterA.NextSegment();
						segmentA.QueryEnvelope2D(env2DSegmentA);
						if (env2DSegmentA.SqrDistance(this.m_env2DgeometryB) > minSqrDistance)
						{
							continue;
						}
						while (segIterB.NextPath())
						{
							while (segIterB.HasNextSegment())
							{
								/* const */
								com.epl.geometry.Segment segmentB = segIterB.NextSegment();
								segmentB.QueryEnvelope2D(env2DSegmentB);
								if (env2DSegmentA.SqrDistance(env2DSegmentB) < minSqrDistance)
								{
									// get distance between segments
									double sqrDistance = segmentA.Distance(segmentB, geometriesAreDisjoint);
									sqrDistance *= sqrDistance;
									if (sqrDistance < minSqrDistance)
									{
										if (sqrDistance == 0.0)
										{
											return 0.0;
										}
										minSqrDistance = sqrDistance;
									}
								}
							}
						}
						segIterB.ResetToFirstPath();
					}
				}
				return System.Math.Sqrt(minSqrDistance);
			}

			private double BruteForceMultiPathMultiPoint_(com.epl.geometry.MultiPath geometryA, com.epl.geometry.MultiPoint geometryB, bool geometriesAreDisjoint)
			{
				/* const */
				/* const */
				com.epl.geometry.SegmentIterator segIterA = geometryA.QuerySegmentIterator();
				com.epl.geometry.Envelope2D env2DSegmentA = new com.epl.geometry.Envelope2D();
				double minSqrDistance = com.epl.geometry.NumberUtils.DoubleMax();
				com.epl.geometry.Point2D inputPoint = new com.epl.geometry.Point2D();
				double t = -1;
				double sqrDistance = minSqrDistance;
				/* const */
				com.epl.geometry.MultiPointImpl multiPointImplB = (com.epl.geometry.MultiPointImpl)geometryB._getImpl();
				int pointCountB = multiPointImplB.GetPointCount();
				bool bDoPiPTest = !geometriesAreDisjoint && (geometryA.GetType() == com.epl.geometry.Geometry.Type.Polygon);
				while (segIterA.NextPath())
				{
					while (segIterA.HasNextSegment())
					{
						/* const */
						com.epl.geometry.Segment segmentA = segIterA.NextSegment();
						segmentA.QueryEnvelope2D(env2DSegmentA);
						// if multipointB has only 1 vertex then it is faster to not
						// test for
						// env2DSegmentA.distance(env2DgeometryB)
						if (pointCountB > 1 && env2DSegmentA.SqrDistance(this.m_env2DgeometryB) > minSqrDistance)
						{
							continue;
						}
						for (int i = 0; i < pointCountB; i++)
						{
							multiPointImplB.GetXY(i, inputPoint);
							if (bDoPiPTest)
							{
								// Test for polygon containment. This takes the
								// place of a more general intersection test at the
								// beginning of the operator
								if (com.epl.geometry.PolygonUtils.IsPointInPolygon2D((com.epl.geometry.Polygon)geometryA, inputPoint, 0) != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
								{
									return 0.0;
								}
							}
							t = segmentA.GetClosestCoordinate(inputPoint, false);
							inputPoint.Sub(segmentA.GetCoord2D(t));
							sqrDistance = inputPoint.SqrLength();
							if (sqrDistance < minSqrDistance)
							{
								if (sqrDistance == 0.0)
								{
									return 0.0;
								}
								minSqrDistance = sqrDistance;
							}
						}
						// No need to do point-in-polygon anymore (if it is a
						// polygon vs polyline)
						bDoPiPTest = false;
					}
				}
				return System.Math.Sqrt(minSqrDistance);
			}

			private double BruteForceMultiPointMultiPoint_(com.epl.geometry.MultiPoint geometryA, com.epl.geometry.MultiPoint geometryB, bool geometriesAreDisjoint)
			{
				/* const */
				/* const */
				double minSqrDistance = com.epl.geometry.NumberUtils.DoubleMax();
				com.epl.geometry.Point2D pointA = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pointB = new com.epl.geometry.Point2D();
				double sqrDistance = minSqrDistance;
				/* const */
				com.epl.geometry.MultiPointImpl multiPointImplA = (com.epl.geometry.MultiPointImpl)geometryA._getImpl();
				/* const */
				/* const */
				com.epl.geometry.MultiPointImpl multiPointImplB = (com.epl.geometry.MultiPointImpl)geometryB._getImpl();
				/* const */
				int pointCountA = multiPointImplA.GetPointCount();
				int pointCountB = multiPointImplB.GetPointCount();
				for (int i = 0; i < pointCountA; i++)
				{
					multiPointImplA.GetXY(i, pointA);
					if (pointCountB > 1 && this.m_env2DgeometryB.SqrDistance(pointA) > minSqrDistance)
					{
						continue;
					}
					for (int j = 0; j < pointCountB; j++)
					{
						multiPointImplB.GetXY(j, pointB);
						sqrDistance = com.epl.geometry.Point2D.SqrDistance(pointA, pointB);
						if (sqrDistance < minSqrDistance)
						{
							if (sqrDistance == 0.0)
							{
								return 0.0;
							}
							minSqrDistance = sqrDistance;
						}
					}
				}
				return System.Math.Sqrt(minSqrDistance);
			}

			// resets Iterators if they are used.
			private bool WeakIntersectionTest_(com.epl.geometry.Geometry geometryA, com.epl.geometry.Geometry geometryB, com.epl.geometry.SegmentIterator segIterA, com.epl.geometry.SegmentIterator segIterB)
			{
				/* const */
				/* const */
				if (geometryA.GetType() == com.epl.geometry.Geometry.Type.Polygon)
				{
					// test PolygonA vs. first segment of each of geometryB's paths
					while (segIterB.NextPath())
					{
						if (segIterB.HasNextSegment())
						{
							/* const */
							com.epl.geometry.Segment segmentB = segIterB.NextSegment();
							if (com.epl.geometry.PolygonUtils.IsPointInPolygon2D((com.epl.geometry.Polygon)geometryA, segmentB.GetEndXY(), 0) != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
							{
								return true;
							}
						}
					}
					segIterB.ResetToFirstPath();
				}
				if (geometryB.GetType() == com.epl.geometry.Geometry.Type.Polygon)
				{
					// test PolygonB vs. first segment of each of geometryA's paths
					while (segIterA.NextPath())
					{
						if (segIterA.HasNextSegment())
						{
							/* const */
							com.epl.geometry.Segment segmentA = segIterA.NextSegment();
							if (com.epl.geometry.PolygonUtils.IsPointInPolygon2D((com.epl.geometry.Polygon)geometryB, segmentA.GetEndXY(), 0) != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
							{
								return true;
							}
						}
					}
					segIterA.ResetToFirstPath();
				}
				return false;
			}

			internal DistanceCalculator(OperatorDistanceLocal _enclosing, com.epl.geometry.ProgressTracker progressTracker)
			{
				this._enclosing = _enclosing;
				this.m_progressTracker = progressTracker;
				this.m_env2DgeometryA = new com.epl.geometry.Envelope2D();
				this.m_env2DgeometryA.SetEmpty();
				this.m_env2DgeometryB = new com.epl.geometry.Envelope2D();
				this.m_env2DgeometryB.SetEmpty();
			}

			internal virtual double Calculate(com.epl.geometry.Geometry geometryA, com.epl.geometry.Geometry geometryB)
			{
				/* const */
				/* const */
				if (geometryA.IsEmpty() || geometryB.IsEmpty())
				{
					return com.epl.geometry.NumberUtils.TheNaN;
				}
				geometryA.QueryEnvelope2D(this.m_env2DgeometryA);
				geometryB.QueryEnvelope2D(this.m_env2DgeometryB);
				return this.ExecuteBruteForce_(geometryA, geometryB);
			}

			private readonly OperatorDistanceLocal _enclosing;
		}
	}
}
