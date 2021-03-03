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
	internal sealed class InternalUtils
	{
		// p0 and p1 have to be on left/right boundary of fullRange2D (since this
		// fuction can be called recursively, p0 or p1 can also be fullRange2D
		// corners)
		internal static int AddPointsToArray(com.epl.geometry.Point2D p0In, com.epl.geometry.Point2D p1In, com.epl.geometry.Point2D[] pointsArray, int idx, com.epl.geometry.Envelope2D fullRange2D, bool clockwise, double densifyDist)
		{
			// PointerOfArrayOf(Point2D)
			// pointsArray, int idx,
			// Envelope2D fullRange2D,
			// boolean clockwise, double
			// densifyDist)
			com.epl.geometry.Point2D p0 = new com.epl.geometry.Point2D();
			p0.SetCoords(p0In);
			com.epl.geometry.Point2D p1 = new com.epl.geometry.Point2D();
			p1.SetCoords(p1In);
			fullRange2D._snapToBoundary(p0);
			fullRange2D._snapToBoundary(p1);
			// //_ASSERT((p0.x == fullRange2D.xmin || p0.x == fullRange2D.xmax) &&
			// (p1.x == fullRange2D.xmin || p1.x == fullRange2D.xmax));
			double boundDist0 = fullRange2D._boundaryDistance(p0);
			double boundDist1 = fullRange2D._boundaryDistance(p1);
			if (boundDist1 == 0.0)
			{
				boundDist1 = fullRange2D.GetLength();
			}
			if ((p0.x == p1.x || p0.y == p1.y && (p0.y == fullRange2D.ymin || p0.y == fullRange2D.ymax)) && (boundDist1 > boundDist0) == clockwise)
			{
				com.epl.geometry.Point2D delta = new com.epl.geometry.Point2D();
				delta.SetCoords(p1.x - p0.x, p1.y - p0.y);
				if (densifyDist != 0)
				{
					// if (densifyDist)
					long cPoints = (long)(delta._norm(0) / densifyDist);
					if (cPoints > 0)
					{
						// if (cPoints)
						delta.Scale(1.0 / (cPoints + 1));
						for (long i = 0; i < cPoints; i++)
						{
							p0.Add(delta);
							pointsArray[idx++].SetCoords(p0.x, p0.y);
						}
					}
				}
			}
			else
			{
				// ARRAYELEMENT(pointsArray,
				// idx++).setCoords(p0.x,
				// p0.y);
				int side0 = fullRange2D._envelopeSide(p0);
				int side1 = fullRange2D._envelopeSide(p1);
				// create up to four corner points; the order depends on boolean
				// clockwise
				com.epl.geometry.Point2D corner;
				int deltaSide = clockwise ? 1 : 3;
				do
				{
					// 3 is equivalent to -1
					side0 = (side0 + deltaSide) & 3;
					corner = fullRange2D.QueryCorner(side0);
					if (densifyDist != 0)
					{
						// if (densifyDist)
						idx = AddPointsToArray(p0, corner, pointsArray, idx, fullRange2D, clockwise, densifyDist);
					}
					pointsArray[idx++].SetCoords(corner.x, corner.y);
					// ARRAYELEMENT(pointsArray,
					// idx++).setCoords(corner.x,
					// corner.y);
					p0 = corner;
				}
				while ((side0 & 3) != side1);
				if (densifyDist != 0)
				{
					// if (densifyDist)
					idx = AddPointsToArray(p0, p1, pointsArray, idx, fullRange2D, clockwise, densifyDist);
				}
			}
			return idx;
		}

		internal void ShiftPath(com.epl.geometry.MultiPath inputGeom, int iPath, double shift)
		{
			com.epl.geometry.MultiVertexGeometryImpl vertexGeometryImpl = (com.epl.geometry.MultiVertexGeometryImpl)inputGeom._getImpl();
			com.epl.geometry.AttributeStreamOfDbl xyStream = (com.epl.geometry.AttributeStreamOfDbl)vertexGeometryImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			int i1 = inputGeom.GetPathStart(iPath);
			int i2 = inputGeom.GetPathEnd(iPath);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			while (i1 < i2)
			{
				xyStream.Read(i1, pt);
				pt.x += shift;
				xyStream.Write(i1, pt);
				i1++;
			}
		}

		internal static double CalculateToleranceFromGeometry(com.epl.geometry.SpatialReference sr, com.epl.geometry.Envelope2D env2D, bool bConservative)
		{
			double gtolerance = env2D._calculateToleranceFromEnvelope();
			double stolerance = sr != null ? sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION) : 0;
			if (bConservative)
			{
				gtolerance *= 4;
				stolerance *= 1.1;
			}
			return System.Math.Max(stolerance, gtolerance);
		}

		internal static double Adjust_tolerance_for_TE_clustering(double tol)
		{
			return 2.0 * System.Math.Sqrt(2.0) * tol;
		}

		internal static double Adjust_tolerance_for_TE_cracking(double tol)
		{
			return System.Math.Sqrt(2.0) * tol;
		}

		internal static double CalculateToleranceFromGeometry(com.epl.geometry.SpatialReference sr, com.epl.geometry.Geometry geometry, bool bConservative)
		{
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			geometry.QueryEnvelope2D(env2D);
			return CalculateToleranceFromGeometry(sr, env2D, bConservative);
		}

		internal static double CalculateZToleranceFromGeometry(com.epl.geometry.SpatialReference sr, com.epl.geometry.Geometry geometry, bool bConservative)
		{
			com.epl.geometry.Envelope1D env1D = geometry.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			double gtolerance = env1D._calculateToleranceFromEnvelope();
			double stolerance = sr != null ? sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.Z) : 0;
			if (bConservative)
			{
				gtolerance *= 4;
				stolerance *= 1.1;
			}
			return System.Math.Max(stolerance, gtolerance);
		}

		internal double CalculateZToleranceFromGeometry(com.epl.geometry.SpatialReference sr, com.epl.geometry.Geometry geometry)
		{
			com.epl.geometry.Envelope1D env1D = geometry.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			double tolerance = env1D._calculateToleranceFromEnvelope();
			return System.Math.Max(sr != null ? sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.Z) : 0, tolerance);
		}

		public static com.epl.geometry.Envelope2D GetMergedExtent(com.epl.geometry.Geometry geom1, com.epl.geometry.Envelope2D env2)
		{
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
			geom1.QueryLooseEnvelope2D(env1);
			env1.Merge(env2);
			return env1;
		}

		public static com.epl.geometry.Envelope2D GetMergedExtent(com.epl.geometry.Geometry geom1, com.epl.geometry.Geometry geom2)
		{
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
			geom1.QueryLooseEnvelope2D(env1);
			com.epl.geometry.Envelope2D env2 = new com.epl.geometry.Envelope2D();
			geom2.QueryLooseEnvelope2D(env2);
			env1.Merge(env2);
			return env1;
		}

		public static com.epl.geometry.Geometry CreateGeometry(int gt, com.epl.geometry.VertexDescription vdIn)
		{
			com.epl.geometry.VertexDescription vd = vdIn;
			if (vd == null)
			{
				vd = com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D();
			}
			switch (gt)
			{
				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					return new com.epl.geometry.Point(vd);
				}

				case com.epl.geometry.Geometry.GeometryType.Line:
				{
					return new com.epl.geometry.Line(vd);
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					// case enum_value2(Geometry, GeometryType, enumBezier):
					// break;
					// case enum_value2(Geometry, GeometryType, enumEllipticArc):
					// break;
					return new com.epl.geometry.Envelope(vd);
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					return new com.epl.geometry.MultiPoint(vd);
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					return new com.epl.geometry.Polyline(vd);
				}

				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					return new com.epl.geometry.Polygon(vd);
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("invalid argument.");
				}
			}
		}

		internal static bool IsClockwiseRing(com.epl.geometry.MultiPathImpl polygon, int iring)
		{
			int high_point_index = polygon.GetHighestPointIndex(iring);
			int path_start = polygon.GetPathStart(iring);
			int path_end = polygon.GetPathEnd(iring);
			com.epl.geometry.Point2D q = polygon.GetXY(high_point_index);
			com.epl.geometry.Point2D p;
			com.epl.geometry.Point2D r;
			if (high_point_index == path_start)
			{
				p = polygon.GetXY(path_end - 1);
				r = polygon.GetXY(path_start + 1);
			}
			else
			{
				if (high_point_index == path_end - 1)
				{
					p = polygon.GetXY(high_point_index - 1);
					r = polygon.GetXY(path_start);
				}
				else
				{
					p = polygon.GetXY(high_point_index - 1);
					r = polygon.GetXY(high_point_index + 1);
				}
			}
			int orientation = com.epl.geometry.Point2D.OrientationRobust(p, q, r);
			if (orientation == 0)
			{
				return polygon.CalculateRingArea2D(iring) > 0.0;
			}
			return orientation == -1;
		}

		internal static com.epl.geometry.QuadTreeImpl BuildQuadTree(com.epl.geometry.MultiPathImpl multipathImpl)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipathImpl.QueryLooseEnvelope2D(extent);
			com.epl.geometry.QuadTreeImpl quad_tree_impl = new com.epl.geometry.QuadTreeImpl(extent, 8);
			int hint_index = -1;
			com.epl.geometry.SegmentIteratorImpl seg_iter = multipathImpl.QuerySegmentIterator();
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			bool resized_extent = false;
			while (seg_iter.NextPath())
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					int index = seg_iter.GetStartPointIndex();
					segment.QueryEnvelope2D(boundingbox);
					hint_index = quad_tree_impl.Insert(index, boundingbox, hint_index);
					if (hint_index == -1)
					{
						if (resized_extent)
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
						// resize extent
						multipathImpl.CalculateEnvelope2D(extent, false);
						resized_extent = true;
						quad_tree_impl.Reset(extent, 8);
						seg_iter.ResetToFirstPath();
						break;
					}
				}
			}
			return quad_tree_impl;
		}

		internal static com.epl.geometry.QuadTreeImpl BuildQuadTree(com.epl.geometry.MultiPathImpl multipathImpl, com.epl.geometry.Envelope2D extentOfInterest)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipathImpl.QueryLooseEnvelope2D(extent);
			com.epl.geometry.QuadTreeImpl quad_tree_impl = new com.epl.geometry.QuadTreeImpl(extent, 8);
			int hint_index = -1;
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			com.epl.geometry.SegmentIteratorImpl seg_iter = multipathImpl.QuerySegmentIterator();
			bool resized_extent = false;
			while (seg_iter.NextPath())
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					int index = seg_iter.GetStartPointIndex();
					segment.QueryEnvelope2D(boundingbox);
					if (boundingbox.IsIntersecting(extentOfInterest))
					{
						hint_index = quad_tree_impl.Insert(index, boundingbox, hint_index);
						if (hint_index == -1)
						{
							if (resized_extent)
							{
								throw com.epl.geometry.GeometryException.GeometryInternalError();
							}
							// resize extent
							multipathImpl.CalculateEnvelope2D(extent, false);
							resized_extent = true;
							quad_tree_impl.Reset(extent, 8);
							seg_iter.ResetToFirstPath();
							break;
						}
					}
				}
			}
			return quad_tree_impl;
		}

		internal static com.epl.geometry.QuadTreeImpl BuildQuadTreeForPaths(com.epl.geometry.MultiPathImpl multipathImpl)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipathImpl.QueryLooseEnvelope2D(extent);
			if (extent.IsEmpty())
			{
				return null;
			}
			com.epl.geometry.QuadTreeImpl quad_tree_impl = new com.epl.geometry.QuadTreeImpl(extent, 8);
			int hint_index = -1;
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			bool resized_extent = false;
			do
			{
				for (int ipath = 0, npaths = multipathImpl.GetPathCount(); ipath < npaths; ipath++)
				{
					multipathImpl.QueryPathEnvelope2D(ipath, boundingbox);
					hint_index = quad_tree_impl.Insert(ipath, boundingbox, hint_index);
					if (hint_index == -1)
					{
						if (resized_extent)
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
						//This is usually happens because esri shape buffer contains geometry extent which is slightly different from the true extent.
						//Recalculate extent
						multipathImpl.CalculateEnvelope2D(extent, false);
						resized_extent = true;
						quad_tree_impl.Reset(extent, 8);
						break;
					}
					else
					{
						//break the for loop
						resized_extent = false;
					}
				}
			}
			while (resized_extent);
			return quad_tree_impl;
		}

		internal static com.epl.geometry.QuadTreeImpl BuildQuadTree(com.epl.geometry.MultiPointImpl multipointImpl)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipointImpl.QueryLooseEnvelope2D(extent);
			com.epl.geometry.QuadTreeImpl quad_tree_impl = new com.epl.geometry.QuadTreeImpl(extent, 8);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			bool resized_extent = false;
			for (int i = 0; i < multipointImpl.GetPointCount(); i++)
			{
				multipointImpl.GetXY(i, pt);
				boundingbox.SetCoords(pt);
				int element_handle = quad_tree_impl.Insert(i, boundingbox);
				if (element_handle == -1)
				{
					if (resized_extent)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					// resize extent
					multipointImpl.CalculateEnvelope2D(extent, false);
					resized_extent = true;
					quad_tree_impl.Reset(extent, 8);
					i = -1;
					// resets the for-loop
					continue;
				}
			}
			return quad_tree_impl;
		}

		internal static com.epl.geometry.QuadTreeImpl BuildQuadTree(com.epl.geometry.MultiPointImpl multipointImpl, com.epl.geometry.Envelope2D extentOfInterest)
		{
			com.epl.geometry.QuadTreeImpl quad_tree_impl = new com.epl.geometry.QuadTreeImpl(extentOfInterest, 8);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			bool resized_extent = false;
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			for (int i = 0; i < multipointImpl.GetPointCount(); i++)
			{
				multipointImpl.GetXY(i, pt);
				if (!extentOfInterest.Contains(pt))
				{
					continue;
				}
				boundingbox.SetCoords(pt);
				int element_handle = quad_tree_impl.Insert(i, boundingbox);
				if (element_handle == -1)
				{
					if (resized_extent)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					// resize extent
					resized_extent = true;
					com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
					multipointImpl.CalculateEnvelope2D(extent, false);
					quad_tree_impl.Reset(extent, 8);
					i = -1;
					// resets the for-loop
					continue;
				}
			}
			return quad_tree_impl;
		}

		internal static com.epl.geometry.Envelope2DIntersectorImpl GetEnvelope2DIntersector(com.epl.geometry.MultiPathImpl multipathImplA, com.epl.geometry.MultiPathImpl multipathImplB, double tolerance)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipathImplA.QueryLooseEnvelope2D(env_a);
			multipathImplB.QueryLooseEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.SegmentIteratorImpl segIterA = multipathImplA.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = multipathImplB.QuerySegmentIterator();
			com.epl.geometry.Envelope2DIntersectorImpl intersector = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector.SetTolerance(tolerance);
			bool b_found_red = false;
			intersector.StartRedConstruction();
			while (segIterA.NextPath())
			{
				while (segIterA.HasNextSegment())
				{
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					segmentA.QueryEnvelope2D(env_a);
					if (!env_a.IsIntersecting(envInter))
					{
						continue;
					}
					b_found_red = true;
					com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
					env.SetCoords(env_a);
					intersector.AddRedEnvelope(segIterA.GetStartPointIndex(), env);
				}
			}
			intersector.EndRedConstruction();
			if (!b_found_red)
			{
				return null;
			}
			bool b_found_blue = false;
			intersector.StartBlueConstruction();
			while (segIterB.NextPath())
			{
				while (segIterB.HasNextSegment())
				{
					com.epl.geometry.Segment segmentB = segIterB.NextSegment();
					segmentB.QueryEnvelope2D(env_b);
					if (!env_b.IsIntersecting(envInter))
					{
						continue;
					}
					b_found_blue = true;
					com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
					env.SetCoords(env_b);
					intersector.AddBlueEnvelope(segIterB.GetStartPointIndex(), env);
				}
			}
			intersector.EndBlueConstruction();
			if (!b_found_blue)
			{
				return null;
			}
			return intersector;
		}

		internal static com.epl.geometry.Envelope2DIntersectorImpl GetEnvelope2DIntersectorForParts(com.epl.geometry.MultiPathImpl multipathImplA, com.epl.geometry.MultiPathImpl multipathImplB, double tolerance, bool bExteriorOnlyA, bool bExteriorOnlyB)
		{
			int type_a = multipathImplA.GetType().Value();
			int type_b = multipathImplB.GetType().Value();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipathImplA.QueryLooseEnvelope2D(env_a);
			multipathImplB.QueryLooseEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.Envelope2DIntersectorImpl intersector = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector.SetTolerance(tolerance);
			bool b_found_red = false;
			intersector.StartRedConstruction();
			for (int ipath_a = 0, npaths = multipathImplA.GetPathCount(); ipath_a < npaths; ipath_a++)
			{
				if (bExteriorOnlyA && type_a == com.epl.geometry.Geometry.GeometryType.Polygon && !multipathImplA.IsExteriorRing(ipath_a))
				{
					continue;
				}
				multipathImplA.QueryPathEnvelope2D(ipath_a, env_a);
				if (!env_a.IsIntersecting(envInter))
				{
					continue;
				}
				b_found_red = true;
				intersector.AddRedEnvelope(ipath_a, env_a);
			}
			intersector.EndRedConstruction();
			if (!b_found_red)
			{
				return null;
			}
			bool b_found_blue = false;
			intersector.StartBlueConstruction();
			for (int ipath_b = 0, npaths = multipathImplB.GetPathCount(); ipath_b < npaths; ipath_b++)
			{
				if (bExteriorOnlyB && type_b == com.epl.geometry.Geometry.GeometryType.Polygon && !multipathImplB.IsExteriorRing(ipath_b))
				{
					continue;
				}
				multipathImplB.QueryPathEnvelope2D(ipath_b, env_b);
				if (!env_b.IsIntersecting(envInter))
				{
					continue;
				}
				b_found_blue = true;
				intersector.AddBlueEnvelope(ipath_b, env_b);
			}
			intersector.EndBlueConstruction();
			if (!b_found_blue)
			{
				return null;
			}
			return intersector;
		}

		internal static bool IsWeakSimple(com.epl.geometry.MultiVertexGeometry geom, double tol)
		{
			return ((com.epl.geometry.MultiVertexGeometryImpl)geom._getImpl()).GetIsSimple(tol) > 0;
		}

//		internal static com.epl.geometry.QuadTree BuildQuadTreeForOnePath(com.epl.geometry.MultiPathImpl multipathImpl, int path)
//		{
//			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
//			multipathImpl.QueryLoosePathEnvelope2D(path, extent);
//			com.epl.geometry.QuadTree quad_tree = new com.epl.geometry.QuadTree(extent, 8);
//			int hint_index = -1;
//			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
//			com.epl.geometry.SegmentIteratorImpl seg_iter = multipathImpl.QuerySegmentIterator();
//			seg_iter.ResetToPath(path);
//			if (seg_iter.NextPath())
//			{
//				while (seg_iter.HasNextSegment())
//				{
//					com.epl.geometry.Segment segment = seg_iter.NextSegment();
//					int index = seg_iter.GetStartPointIndex();
//					segment.QueryLooseEnvelope2D(boundingbox);
//					hint_index = quad_tree.Insert(index, boundingbox, hint_index);
//					if (hint_index == -1)
//					{
//						throw new com.epl.geometry.GeometryException("internal error");
//					}
//				}
//			}
//			return quad_tree;
//		}
	}
}
