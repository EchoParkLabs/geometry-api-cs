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
	internal class OperatorIntersectionCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeoms;

		internal com.epl.geometry.GeometryCursor m_smallCursor;

		internal com.epl.geometry.ProgressTracker m_progress_tracker;

		internal com.epl.geometry.SpatialReference m_spatial_reference;

		internal com.epl.geometry.Geometry m_geomIntersector;

		internal com.epl.geometry.Geometry m_geomIntersectorEmptyGeom;

		internal int m_geomIntersectorType;

		internal int m_currentGeomType;

		internal int m_index;

		internal int m_dimensionMask;

		internal bool m_bEmpty;

		internal OperatorIntersectionCursor(com.epl.geometry.GeometryCursor inputGeoms, com.epl.geometry.GeometryCursor geomIntersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker, int dimensionMask)
		{
			// holds empty geometry of intersector
			// type.
			m_bEmpty = geomIntersector == null;
			m_index = -1;
			m_inputGeoms = inputGeoms;
			m_spatial_reference = sr;
			m_geomIntersector = geomIntersector.Next();
			m_geomIntersectorType = m_geomIntersector.GetType().Value();
			m_currentGeomType = com.epl.geometry.Geometry.Type.Unknown.Value();
			m_progress_tracker = progress_tracker;
			m_dimensionMask = dimensionMask;
			if (m_dimensionMask != -1 && (m_dimensionMask <= 0 || m_dimensionMask > 7))
			{
				throw new System.ArgumentException("bad dimension mask");
			}
		}

		// dimension
		// mask
		// can
		// be
		// -1,
		// for
		// the
		// default
		// behavior,
		// or a
		// value
		// between
		// 1 and
		// 7.
		public override com.epl.geometry.Geometry Next()
		{
			if (m_bEmpty)
			{
				return null;
			}
			com.epl.geometry.Geometry geom;
			if (m_smallCursor != null)
			{
				// when dimension mask is used, we produce a
				geom = m_smallCursor.Next();
				if (geom != null)
				{
					return geom;
				}
				else
				{
					m_smallCursor = null;
				}
			}
			// done with the small cursor
			while ((geom = m_inputGeoms.Next()) != null)
			{
				m_index = m_inputGeoms.GetGeometryID();
				if (m_dimensionMask == -1)
				{
					com.epl.geometry.Geometry resGeom = Intersect(geom);
					System.Diagnostics.Debug.Assert((resGeom != null));
					return resGeom;
				}
				else
				{
					m_smallCursor = IntersectEx(geom);
					com.epl.geometry.Geometry resGeom = m_smallCursor.Next();
					System.Diagnostics.Debug.Assert((resGeom != null));
					return resGeom;
				}
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		internal virtual com.epl.geometry.Geometry Intersect(com.epl.geometry.Geometry input_geom)
		{
			com.epl.geometry.Geometry dst_geom = TryNativeImplementation_(input_geom);
			if (dst_geom != null)
			{
				return dst_geom;
			}
			com.epl.geometry.Envelope2D commonExtent = com.epl.geometry.InternalUtils.GetMergedExtent(m_geomIntersector, input_geom);
			// return Topological_operations::intersection(input_geom,
			// m_geomIntersector, m_spatial_reference, m_progress_tracker);
			// Preprocess geometries to be clipped to the extent of intersection to
			// get rid of extra segments.
			double t = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_spatial_reference, commonExtent, true);
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			m_geomIntersector.QueryEnvelope2D(env);
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
			input_geom.QueryEnvelope2D(env1);
			env.Inflate(2.0 * t, 2.0 * t);
			env.Intersect(env1);
			System.Diagnostics.Debug.Assert((!env.IsEmpty()));
			env.Inflate(100 * t, 100 * t);
			double tol = 0;
			com.epl.geometry.Geometry clippedIntersector = com.epl.geometry.Clipper.Clip(m_geomIntersector, env, tol, 0.0);
			com.epl.geometry.Geometry clippedInputGeom = com.epl.geometry.Clipper.Clip(input_geom, env, tol, 0.0);
			// perform the clip
			return com.epl.geometry.TopologicalOperations.Intersection(clippedInputGeom, clippedIntersector, m_spatial_reference, m_progress_tracker);
		}

		// Parses the input vector to ensure the out result contains only geometries
		// as indicated with the dimensionMask
		internal virtual com.epl.geometry.GeometryCursor PrepareVector_(com.epl.geometry.VertexDescription descr, int dimensionMask, com.epl.geometry.Geometry[] res_vec)
		{
			int inext = 0;
			if ((dimensionMask & 1) != 0)
			{
				if (res_vec[0] == null)
				{
					res_vec[0] = new com.epl.geometry.MultiPoint(descr);
				}
				inext++;
			}
			else
			{
				for (int i = 0; i < res_vec.Length - 1; i++)
				{
					res_vec[i] = res_vec[i + 1];
				}
			}
			if ((dimensionMask & 2) != 0)
			{
				if (res_vec[inext] == null)
				{
					res_vec[inext] = new com.epl.geometry.Polyline(descr);
				}
				inext++;
			}
			else
			{
				for (int i = inext; i < res_vec.Length - 1; i++)
				{
					res_vec[i] = res_vec[i + 1];
				}
			}
			if ((dimensionMask & 4) != 0)
			{
				if (res_vec[inext] == null)
				{
					res_vec[inext] = new com.epl.geometry.Polygon(descr);
				}
				inext++;
			}
			else
			{
				for (int i = inext; i < res_vec.Length - 1; i++)
				{
					res_vec[i] = res_vec[i + 1];
				}
			}
			if (inext != 3)
			{
				com.epl.geometry.Geometry[] r = new com.epl.geometry.Geometry[inext];
				for (int i = 0; i < inext; i++)
				{
					r[i] = res_vec[i];
				}
				return new com.epl.geometry.SimpleGeometryCursor(r);
			}
			else
			{
				return new com.epl.geometry.SimpleGeometryCursor(res_vec);
			}
		}

		internal virtual com.epl.geometry.GeometryCursor IntersectEx(com.epl.geometry.Geometry input_geom)
		{
			System.Diagnostics.Debug.Assert((m_dimensionMask != -1));
			com.epl.geometry.Geometry dst_geom = TryNativeImplementation_(input_geom);
			if (dst_geom != null)
			{
				com.epl.geometry.Geometry[] res_vec = new com.epl.geometry.Geometry[3];
				res_vec[dst_geom.GetDimension()] = dst_geom;
				return PrepareVector_(input_geom.GetDescription(), m_dimensionMask, res_vec);
			}
			com.epl.geometry.Envelope2D commonExtent = com.epl.geometry.InternalUtils.GetMergedExtent(m_geomIntersector, input_geom);
			double t = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_spatial_reference, commonExtent, true);
			// Preprocess geometries to be clipped to the extent of intersection to
			// get rid of extra segments.
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			m_geomIntersector.QueryEnvelope2D(env);
			env.Inflate(2 * t, 2 * t);
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
			input_geom.QueryEnvelope2D(env1);
			env.Intersect(env1);
			System.Diagnostics.Debug.Assert((!env.IsEmpty()));
			env.Inflate(100 * t, 100 * t);
			double tol = 0;
			com.epl.geometry.Geometry clippedIntersector = com.epl.geometry.Clipper.Clip(m_geomIntersector, env, tol, 0.0);
			com.epl.geometry.Geometry clippedInputGeom = com.epl.geometry.Clipper.Clip(input_geom, env, tol, 0.0);
			// perform the clip
			com.epl.geometry.Geometry[] res_vec_1;
			res_vec_1 = com.epl.geometry.TopologicalOperations.IntersectionEx(clippedInputGeom, clippedIntersector, m_spatial_reference, m_progress_tracker);
			return PrepareVector_(input_geom.GetDescription(), m_dimensionMask, res_vec_1);
		}

		internal virtual com.epl.geometry.Geometry TryNativeImplementation_(com.epl.geometry.Geometry input_geom)
		{
			// A note on attributes:
			// 1. The geometry with lower dimension wins in regard to the
			// attributes.
			// 2. If the dimensions are the same, the input_geometry attributes win.
			// 3. The exception to the 2. is when the input is an Envelope, and the
			// intersector is a polygon, then the intersector wins.
			// A note on the tolerance:
			// This operator performs a simple intersection operation. Should it use
			// the tolerance?
			// Example: Point is intersected by the envelope.
			// If it is slightly outside of the envelope, should we still return it
			// if it is closer than the tolerance?
			// Should we do crack and cluster and snap the point coordinates to the
			// envelope boundary?
			//
			// Consider floating point arithmetics approach. When you compare
			// doubles, you should use an epsilon (equals means ::fabs(a - b) <
			// eps), however when you add/subtract, etc them, you do not use
			// epsilon.
			// Shouldn't we do same here? Relational operators use tolerance, but
			// the action operators don't.
			com.epl.geometry.Envelope2D mergedExtent = com.epl.geometry.InternalUtils.GetMergedExtent(input_geom, m_geomIntersector);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_spatial_reference, mergedExtent, false);
			int gtInput = input_geom.GetType().Value();
			bool bInputEmpty = input_geom.IsEmpty();
			bool bGeomIntersectorEmpty = m_geomIntersector.IsEmpty();
			bool bResultIsEmpty = bInputEmpty || bGeomIntersectorEmpty;
			if (!bResultIsEmpty)
			{
				// test envelopes
				com.epl.geometry.Envelope2D env2D1 = new com.epl.geometry.Envelope2D();
				input_geom.QueryEnvelope2D(env2D1);
				com.epl.geometry.Envelope2D env2D2 = new com.epl.geometry.Envelope2D();
				m_geomIntersector.QueryEnvelope2D(env2D2);
				env2D2.Inflate(2.0 * tolerance, 2.0 * tolerance);
				bResultIsEmpty = !env2D1.IsIntersecting(env2D2);
			}
			if (!bResultIsEmpty)
			{
				// try accelerated test
				int res = com.epl.geometry.OperatorInternalRelationUtils.QuickTest2D_Accelerated_DisjointOrContains(m_geomIntersector, input_geom, tolerance);
				if (res == com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint)
				{
					// disjoint
					bResultIsEmpty = true;
				}
				else
				{
					if ((res & com.epl.geometry.OperatorInternalRelationUtils.Relation.Within) != 0)
					{
						// intersector
						// is
						// within
						// the
						// input_geom
						// TODO:
						// assign
						// input_geom
						// attributes
						// first
						return m_geomIntersector;
					}
					else
					{
						if ((res & com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains) != 0)
						{
							// intersector
							// contains
							// input_geom
							return input_geom;
						}
					}
				}
			}
			if (bResultIsEmpty)
			{
				// When one geometry or both are empty, we need to
				// return an empty geometry.
				// Here we do that end also ensure the type is
				// correct.
				// That is the lower dimension need to be
				// returned. Also, for Point vs Multi_point, an
				// empty Point need to be returned.
				int dim1 = com.epl.geometry.Geometry.GetDimensionFromType(gtInput);
				int dim2 = com.epl.geometry.Geometry.GetDimensionFromType(m_geomIntersectorType);
				if (dim1 < dim2)
				{
					return ReturnEmpty_(input_geom, bInputEmpty);
				}
				else
				{
					if (dim1 > dim2)
					{
						return ReturnEmptyIntersector_();
					}
					else
					{
						if (dim1 == 0)
						{
							if (gtInput == com.epl.geometry.Geometry.GeometryType.MultiPoint && m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.Point)
							{
								// point
								// vs
								// Multi_point
								// need
								// special
								// treatment
								// to
								// ensure
								// Point
								// is
								// returned
								// always.
								return ReturnEmptyIntersector_();
							}
							else
							{
								// Both input and intersector have same gtype, or input is
								// Point.
								return ReturnEmpty_(input_geom, bInputEmpty);
							}
						}
						else
						{
							return ReturnEmpty_(input_geom, bInputEmpty);
						}
					}
				}
			}
			// Note: No empty geometries after this point!
			// Warning: Do not try clip for polylines and polygons.
			// Try clip of Envelope with Envelope.
			if ((m_dimensionMask == -1 || m_dimensionMask == (1 << 2)) && gtInput == com.epl.geometry.Geometry.GeometryType.Envelope && m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)input_geom;
				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)m_geomIntersector;
				com.epl.geometry.Envelope2D env2D_1 = new com.epl.geometry.Envelope2D();
				env1.QueryEnvelope2D(env2D_1);
				com.epl.geometry.Envelope2D env2D_2 = new com.epl.geometry.Envelope2D();
				env2.QueryEnvelope2D(env2D_2);
				env2D_1.Intersect(env2D_2);
				com.epl.geometry.Envelope result_env = new com.epl.geometry.Envelope();
				env1.CopyTo(result_env);
				result_env.SetEnvelope2D(env2D_1);
				return result_env;
			}
			// Use clip for Point and Multi_point with Envelope
			if ((gtInput == com.epl.geometry.Geometry.GeometryType.Envelope && com.epl.geometry.Geometry.GetDimensionFromType(m_geomIntersectorType) == 0) || (m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.Envelope && com.epl.geometry.Geometry.GetDimensionFromType(gtInput
				) == 0))
			{
				com.epl.geometry.Envelope env = gtInput == com.epl.geometry.Geometry.GeometryType.Envelope ? (com.epl.geometry.Envelope)input_geom : (com.epl.geometry.Envelope)m_geomIntersector;
				com.epl.geometry.Geometry other = gtInput == com.epl.geometry.Geometry.GeometryType.Envelope ? m_geomIntersector : input_geom;
				com.epl.geometry.Envelope2D env_2D = new com.epl.geometry.Envelope2D();
				env.QueryEnvelope2D(env_2D);
				return com.epl.geometry.Clipper.Clip(other, env_2D, tolerance, 0);
			}
			if ((com.epl.geometry.Geometry.GetDimensionFromType(gtInput) == 0 && com.epl.geometry.Geometry.GetDimensionFromType(m_geomIntersectorType) > 0) || (com.epl.geometry.Geometry.GetDimensionFromType(gtInput) > 0 && com.epl.geometry.Geometry.GetDimensionFromType(m_geomIntersectorType
				) == 0))
			{
				// multipoint
				// intersection
				double tolerance1 = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_spatial_reference, input_geom, false);
				if (gtInput == com.epl.geometry.Geometry.GeometryType.MultiPoint)
				{
					return com.epl.geometry.TopologicalOperations.Intersection((com.epl.geometry.MultiPoint)input_geom, m_geomIntersector, tolerance1);
				}
				if (gtInput == com.epl.geometry.Geometry.GeometryType.Point)
				{
					return com.epl.geometry.TopologicalOperations.Intersection((com.epl.geometry.Point)input_geom, m_geomIntersector, tolerance1);
				}
				if (m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.MultiPoint)
				{
					return com.epl.geometry.TopologicalOperations.Intersection((com.epl.geometry.MultiPoint)m_geomIntersector, input_geom, tolerance1);
				}
				if (m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.Point)
				{
					return com.epl.geometry.TopologicalOperations.Intersection((com.epl.geometry.Point)m_geomIntersector, input_geom, tolerance1);
				}
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			// Try Polyline vs Polygon
			if ((m_dimensionMask == -1 || m_dimensionMask == (1 << 1)) && (gtInput == com.epl.geometry.Geometry.GeometryType.Polyline) && (m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.Polygon))
			{
				return TryFastIntersectPolylinePolygon_((com.epl.geometry.Polyline)(input_geom), (com.epl.geometry.Polygon)(m_geomIntersector));
			}
			// Try Polygon vs Polyline
			if ((m_dimensionMask == -1 || m_dimensionMask == (1 << 1)) && (gtInput == com.epl.geometry.Geometry.GeometryType.Polygon) && (m_geomIntersectorType == com.epl.geometry.Geometry.GeometryType.Polyline))
			{
				return TryFastIntersectPolylinePolygon_((com.epl.geometry.Polyline)(m_geomIntersector), (com.epl.geometry.Polygon)(input_geom));
			}
			return null;
		}

		internal virtual com.epl.geometry.Geometry TryFastIntersectPolylinePolygon_(com.epl.geometry.Polyline polyline, com.epl.geometry.Polygon polygon)
		{
			com.epl.geometry.MultiPathImpl polylineImpl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
			com.epl.geometry.MultiPathImpl polygonImpl = (com.epl.geometry.MultiPathImpl)polygon._getImpl();
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_spatial_reference, polygon, false);
			com.epl.geometry.Envelope2D clipEnvelope = new com.epl.geometry.Envelope2D();
			{
				polygonImpl.QueryEnvelope2D(clipEnvelope);
				com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
				polylineImpl.QueryEnvelope2D(env1);
				env1.Inflate(2.0 * tolerance, 2.0 * tolerance);
				clipEnvelope.Intersect(env1);
				System.Diagnostics.Debug.Assert((!clipEnvelope.IsEmpty()));
			}
			clipEnvelope.Inflate(10 * tolerance, 10 * tolerance);
			if (true)
			{
				double tol = 0;
				com.epl.geometry.Geometry clippedPolyline = com.epl.geometry.Clipper.Clip(polyline, clipEnvelope, tol, 0.0);
				polyline = (com.epl.geometry.Polyline)clippedPolyline;
				polylineImpl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
			}
			com.epl.geometry.AttributeStreamOfInt32 clipResult = new com.epl.geometry.AttributeStreamOfInt32(0);
			int unresolvedSegments = -1;
			com.epl.geometry.GeometryAccelerators accel = polygonImpl._getAccelerators();
			if (accel != null)
			{
				com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
				if (rgeom != null)
				{
					unresolvedSegments = 0;
					clipResult.Reserve(polylineImpl.GetPointCount() + polylineImpl.GetPathCount());
					com.epl.geometry.Envelope2D seg_env = new com.epl.geometry.Envelope2D();
					com.epl.geometry.SegmentIteratorImpl iter = polylineImpl.QuerySegmentIterator();
					while (iter.NextPath())
					{
						while (iter.HasNextSegment())
						{
							com.epl.geometry.Segment seg = iter.NextSegment();
							seg.QueryEnvelope2D(seg_env);
							com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryEnvelopeInGeometry(seg_env);
							if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
							{
								clipResult.Add(1);
							}
							else
							{
								if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
								{
									clipResult.Add(0);
								}
								else
								{
									clipResult.Add(-1);
									unresolvedSegments++;
								}
							}
						}
					}
				}
			}
			if (polygon.GetPointCount() > 5)
			{
				double tol = 0;
				com.epl.geometry.Geometry clippedPolygon = com.epl.geometry.Clipper.Clip(polygon, clipEnvelope, tol, 0.0);
				polygon = (com.epl.geometry.Polygon)clippedPolygon;
				polygonImpl = (com.epl.geometry.MultiPathImpl)polygon._getImpl();
				accel = polygonImpl._getAccelerators();
			}
			//update accelerators
			if (unresolvedSegments < 0)
			{
				unresolvedSegments = polylineImpl.GetSegmentCount();
			}
			// Some heuristics to decide if it makes sense to go with fast intersect
			// vs going with the regular planesweep.
			double totalPoints = (double)(polylineImpl.GetPointCount() + polygonImpl.GetPointCount());
			double thisAlgorithmComplexity = ((double)unresolvedSegments * polygonImpl.GetPointCount());
			// assume the worst case.
			double planesweepComplexity = System.Math.Log(totalPoints) * totalPoints;
			double empiricConstantFactorPlaneSweep = 4;
			if (thisAlgorithmComplexity > planesweepComplexity * empiricConstantFactorPlaneSweep)
			{
				// Based on the number of input points, we deduced that the
				// plansweep performance should be better than the brute force
				// performance.
				return null;
			}
			// resort to planesweep if quadtree does not help
			com.epl.geometry.QuadTreeImpl polygonQuadTree = null;
			com.epl.geometry.SegmentIteratorImpl polygonIter = polygonImpl.QuerySegmentIterator();
			// Some logic to decide if it makes sense to build a quadtree on the
			// polygon segments
			if (accel != null && accel.GetQuadTree() != null)
			{
				polygonQuadTree = accel.GetQuadTree();
			}
			if (polygonQuadTree == null && polygonImpl.GetPointCount() > 20)
			{
				polygonQuadTree = com.epl.geometry.InternalUtils.BuildQuadTree(polygonImpl);
			}
			com.epl.geometry.Polyline result_polyline = (com.epl.geometry.Polyline)polyline.CreateInstance();
			com.epl.geometry.MultiPathImpl resultPolylineImpl = (com.epl.geometry.MultiPathImpl)result_polyline._getImpl();
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qIter = null;
			com.epl.geometry.SegmentIteratorImpl polylineIter = polylineImpl.QuerySegmentIterator();
			double[] @params = new double[9];
			com.epl.geometry.AttributeStreamOfDbl intersections = new com.epl.geometry.AttributeStreamOfDbl(0);
			com.epl.geometry.SegmentBuffer segmentBuffer = new com.epl.geometry.SegmentBuffer();
			int start_index = -1;
			int inCount = 0;
			int segIndex = 0;
			bool bOptimized = clipResult.Size() > 0;
			// The algorithm is like that:
			// Loop through all the segments of the polyline.
			// For each polyline segment, intersect it with each of the polygon
			// segments.
			// If no intersections found then,
			// If the polyline segment is completely inside, it is added to the
			// result polyline.
			// If it is outside, it is thrown out.
			// If it intersects, then cut the polyline segment to pieces and test
			// each part of the intersected result.
			// The cut pieces will either have one point inside, or one point
			// outside, or the middle point inside/outside.
			//
			int polylinePathIndex = -1;
			while (polylineIter.NextPath())
			{
				polylinePathIndex = polylineIter.GetPathIndex();
				int stateNewPath = 0;
				int stateAddSegment = 1;
				int stateManySegments = 2;
				int stateManySegmentsContinuePath = 2;
				int stateManySegmentsNewPath = 3;
				int state = stateNewPath;
				start_index = -1;
				inCount = 0;
				while (polylineIter.HasNextSegment())
				{
					int clipStatus = bOptimized ? (int)clipResult.Get(segIndex) : -1;
					segIndex++;
					com.epl.geometry.Segment polylineSeg = polylineIter.NextSegment();
					if (clipStatus < 0)
					{
						System.Diagnostics.Debug.Assert((clipStatus == -1));
						// Analyse polyline segment for intersection with the
						// polygon.
						if (polygonQuadTree != null)
						{
							if (qIter == null)
							{
								qIter = polygonQuadTree.GetIterator(polylineSeg, tolerance);
							}
							else
							{
								qIter.ResetIterator(polylineSeg, tolerance);
							}
							int path_index = -1;
							for (int ind = qIter.Next(); ind != -1; ind = qIter.Next())
							{
								polygonIter.ResetToVertex(polygonQuadTree.GetElement(ind));
								// path_index
								path_index = polygonIter.GetPathIndex();
								com.epl.geometry.Segment polygonSeg = polygonIter.NextSegment();
								// intersect polylineSeg and polygonSeg.
								int count = polylineSeg.Intersect(polygonSeg, null, @params, null, tolerance);
								for (int i = 0; i < count; i++)
								{
									intersections.Add(@params[i]);
								}
							}
						}
						else
						{
							// no quadtree built
							polygonIter.ResetToFirstPath();
							while (polygonIter.NextPath())
							{
								while (polygonIter.HasNextSegment())
								{
									com.epl.geometry.Segment polygonSeg = polygonIter.NextSegment();
									// intersect polylineSeg and polygonSeg.
									int count = polylineSeg.Intersect(polygonSeg, null, @params, null, tolerance);
									for (int i = 0; i < count; i++)
									{
										intersections.Add(@params[i]);
									}
								}
							}
						}
						if (intersections.Size() > 0)
						{
							// intersections detected.
							intersections.Sort(0, intersections.Size());
							// std::sort(intersections.begin(),
							// intersections.end());
							double t0 = 0;
							intersections.Add(1.0);
							int status = -1;
							for (int i = 0, n = intersections.Size(); i < n; i++)
							{
								double t = intersections.Get(i);
								if (t == t0)
								{
									continue;
								}
								bool bWholeSegment = false;
								com.epl.geometry.Segment resSeg;
								if (t0 != 0 || t != 1.0)
								{
									polylineSeg.Cut(t0, t, segmentBuffer);
									resSeg = segmentBuffer.Get();
								}
								else
								{
									resSeg = polylineSeg;
									bWholeSegment = true;
								}
								if (state >= stateManySegments)
								{
									resultPolylineImpl.AddSegmentsFromPath(polylineImpl, polylinePathIndex, start_index, inCount, state == stateManySegmentsNewPath);
									if (AnalyseClipSegment_(polygon, resSeg.GetStartXY(), tolerance) != 1)
									{
										if (AnalyseClipSegment_(polygon, resSeg, tolerance) != 1)
										{
											return null;
										}
									}
									//someting went wrong we'll falback to slower but robust planesweep code.
									resultPolylineImpl.AddSegment(resSeg, false);
									state = stateAddSegment;
									inCount = 0;
								}
								else
								{
									status = AnalyseClipSegment_(polygon, resSeg, tolerance);
									switch (status)
									{
										case 1:
										{
											if (!bWholeSegment)
											{
												resultPolylineImpl.AddSegment(resSeg, state == stateNewPath);
												state = stateAddSegment;
											}
											else
											{
												if (state < stateManySegments)
												{
													start_index = polylineIter.GetStartPointIndex() - polylineImpl.GetPathStart(polylinePathIndex);
													inCount = 1;
													if (state == stateNewPath)
													{
														state = stateManySegmentsNewPath;
													}
													else
													{
														System.Diagnostics.Debug.Assert((state == stateAddSegment));
														state = stateManySegmentsContinuePath;
													}
												}
												else
												{
													inCount++;
												}
											}
											break;
										}

										case 0:
										{
											state = stateNewPath;
											start_index = -1;
											inCount = 0;
											break;
										}

										default:
										{
											return null;
										}
									}
								}
								// may happen if a segment
								// coincides with the border.
								t0 = t;
							}
						}
						else
						{
							clipStatus = AnalyseClipSegment_(polygon, polylineSeg.GetStartXY(), tolerance);
							// simple
							// case
							// no
							// intersection.
							// Both
							// points
							// must
							// be
							// inside.
							if (clipStatus < 0)
							{
								System.Diagnostics.Debug.Assert((clipStatus >= 0));
								return null;
							}
							// something goes wrong, resort to
							// planesweep
							System.Diagnostics.Debug.Assert((AnalyseClipSegment_(polygon, polylineSeg.GetEndXY(), tolerance) == clipStatus));
							if (clipStatus == 1)
							{
								// the whole segment inside
								if (state < stateManySegments)
								{
									System.Diagnostics.Debug.Assert((inCount == 0));
									start_index = polylineIter.GetStartPointIndex() - polylineImpl.GetPathStart(polylinePathIndex);
									if (state == stateNewPath)
									{
										state = stateManySegmentsNewPath;
									}
									else
									{
										System.Diagnostics.Debug.Assert((state == stateAddSegment));
										state = stateManySegmentsContinuePath;
									}
								}
								inCount++;
							}
							else
							{
								System.Diagnostics.Debug.Assert((state < stateManySegments));
								start_index = -1;
								inCount = 0;
							}
						}
						intersections.Clear(false);
					}
					else
					{
						// clip status is determined by other means
						if (clipStatus == 0)
						{
							// outside
							System.Diagnostics.Debug.Assert((AnalyseClipSegment_(polygon, polylineSeg, tolerance) == 0));
							System.Diagnostics.Debug.Assert((start_index < 0));
							System.Diagnostics.Debug.Assert((inCount == 0));
							continue;
						}
						if (clipStatus == 1)
						{
							System.Diagnostics.Debug.Assert((AnalyseClipSegment_(polygon, polylineSeg, tolerance) == 1));
							if (state == stateNewPath)
							{
								state = stateManySegmentsNewPath;
								start_index = polylineIter.GetStartPointIndex() - polylineImpl.GetPathStart(polylinePathIndex);
							}
							else
							{
								if (state == stateAddSegment)
								{
									state = stateManySegmentsContinuePath;
									start_index = polylineIter.GetStartPointIndex() - polylineImpl.GetPathStart(polylinePathIndex);
								}
								else
								{
									System.Diagnostics.Debug.Assert((state >= stateManySegments));
								}
							}
							inCount++;
							continue;
						}
					}
				}
				if (state >= stateManySegments)
				{
					resultPolylineImpl.AddSegmentsFromPath(polylineImpl, polylinePathIndex, start_index, inCount, state == stateManySegmentsNewPath);
					start_index = -1;
				}
			}
			return result_polyline;
		}

		internal virtual int AnalyseClipSegment_(com.epl.geometry.Polygon polygon, com.epl.geometry.Point2D pt, double tol)
		{
			int v = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, pt, tol);
			return v;
		}

		internal virtual int AnalyseClipSegment_(com.epl.geometry.Polygon polygon, com.epl.geometry.Segment seg, double tol)
		{
			com.epl.geometry.Point2D pt_1 = seg.GetStartXY();
			com.epl.geometry.Point2D pt_2 = seg.GetEndXY();
			int v_1 = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, pt_1, tol);
			int v_2 = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, pt_2, tol);
			if ((v_1 == 1 && v_2 == 0) || (v_1 == 0 && v_2 == 1))
			{
				// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/badPointInPolygon.json",
				// polygon, m_spatial_reference);
				System.Diagnostics.Debug.Assert((false));
				// if happens
				return -1;
			}
			// something went wrong. One point is inside, the other is
			// outside. Should not happen. We'll resort to
			// planesweep.
			if (v_1 == 0 || v_2 == 0)
			{
				return 0;
			}
			if (v_1 == 1 || v_2 == 1)
			{
				return 1;
			}
			com.epl.geometry.Point2D midPt = new com.epl.geometry.Point2D();
			midPt.Add(pt_1, pt_2);
			midPt.Scale(0.5);
			// calculate midpoint
			int v = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, midPt, tol);
			if (v == 0)
			{
				return 0;
			}
			if (v == 1)
			{
				return 1;
			}
			return -1;
		}

		internal virtual com.epl.geometry.Geometry NormalizeIntersectionOutput(com.epl.geometry.Geometry geom, int GT_1, int GT_2)
		{
			if (GT_1 == com.epl.geometry.Geometry.GeometryType.Point || GT_2 == com.epl.geometry.Geometry.GeometryType.Point)
			{
				System.Diagnostics.Debug.Assert((geom.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Point));
			}
			if (GT_1 == com.epl.geometry.Geometry.GeometryType.MultiPoint)
			{
				if (geom.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Point)
				{
					com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint(geom.GetDescription());
					if (!geom.IsEmpty())
					{
						mp.Add((com.epl.geometry.Point)geom);
					}
					return mp;
				}
			}
			return geom;
		}

		internal static com.epl.geometry.Geometry ReturnEmpty_(com.epl.geometry.Geometry geom, bool bEmpty)
		{
			return bEmpty ? geom : geom.CreateInstance();
		}

		internal virtual com.epl.geometry.Geometry ReturnEmptyIntersector_()
		{
			if (m_geomIntersectorEmptyGeom == null)
			{
				m_geomIntersectorEmptyGeom = m_geomIntersector.CreateInstance();
			}
			return m_geomIntersectorEmptyGeom;
		}
		// virtual boolean IsRecycling() OVERRIDE { return false; }
	}
}
