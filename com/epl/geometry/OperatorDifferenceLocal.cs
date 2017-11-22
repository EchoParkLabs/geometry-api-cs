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
	internal class OperatorDifferenceLocal : com.epl.geometry.OperatorDifference
	{
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.GeometryCursor subtractor, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			return new com.epl.geometry.OperatorDifferenceCursor(inputGeometries, subtractor, sr, progressTracker);
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.Geometry subtractor, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.SimpleGeometryCursor inputGeomCurs = new com.epl.geometry.SimpleGeometryCursor(inputGeometry);
			com.epl.geometry.SimpleGeometryCursor subractorCurs = new com.epl.geometry.SimpleGeometryCursor(subtractor);
			com.epl.geometry.GeometryCursor geometryCursor = Execute(inputGeomCurs, subractorCurs, sr, progressTracker);
			return geometryCursor.Next();
		}

		internal static com.epl.geometry.Geometry Difference(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference spatial_reference, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geometry_a.IsEmpty() || geometry_b.IsEmpty())
			{
				return geometry_a;
			}
			int dimension_a = geometry_a.GetDimension();
			int dimension_b = geometry_b.GetDimension();
			if (dimension_a > dimension_b)
			{
				return geometry_a;
			}
			int type_a = geometry_a.GetType().Value();
			int type_b = geometry_b.GetType().Value();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_merged = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env_a);
			geometry_b.QueryEnvelope2D(env_b);
			env_merged.SetCoords(env_a);
			env_merged.Merge(env_b);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatial_reference, env_merged, false);
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			com.epl.geometry.Envelope2D env_a_inflated = new com.epl.geometry.Envelope2D();
			env_a_inflated.SetCoords(env_a);
			env_a_inflated.Inflate(tolerance_cluster, tolerance_cluster);
			// inflate
			// by
			// cluster
			// tolerance
			if (!env_a_inflated.IsIntersecting(env_b))
			{
				return geometry_a;
			}
			if (dimension_a == 1 && dimension_b == 2)
			{
				return PolylineMinusArea_(geometry_a, geometry_b, type_b, spatial_reference, progress_tracker);
			}
			if (type_a == com.epl.geometry.Geometry.GeometryType.Point)
			{
				com.epl.geometry.Geometry geometry_b_;
				if (com.epl.geometry.MultiPath.IsSegment(type_b))
				{
					geometry_b_ = new com.epl.geometry.Polyline(geometry_b.GetDescription());
					((com.epl.geometry.Polyline)(geometry_b_)).AddSegment((com.epl.geometry.Segment)(geometry_b), true);
				}
				else
				{
					geometry_b_ = geometry_b;
				}
				switch (type_b)
				{
					case com.epl.geometry.Geometry.GeometryType.Polygon:
					{
						return PointMinusPolygon_((com.epl.geometry.Point)(geometry_a), (com.epl.geometry.Polygon)(geometry_b_), tolerance, progress_tracker);
					}

					case com.epl.geometry.Geometry.GeometryType.Polyline:
					{
						return PointMinusPolyline_((com.epl.geometry.Point)(geometry_a), (com.epl.geometry.Polyline)(geometry_b_), tolerance, progress_tracker);
					}

					case com.epl.geometry.Geometry.GeometryType.MultiPoint:
					{
						return PointMinusMultiPoint_((com.epl.geometry.Point)(geometry_a), (com.epl.geometry.MultiPoint)(geometry_b_), tolerance, progress_tracker);
					}

					case com.epl.geometry.Geometry.GeometryType.Envelope:
					{
						return PointMinusEnvelope_((com.epl.geometry.Point)(geometry_a), (com.epl.geometry.Envelope)(geometry_b_), tolerance, progress_tracker);
					}

					case com.epl.geometry.Geometry.GeometryType.Point:
					{
						return PointMinusPoint_((com.epl.geometry.Point)(geometry_a), (com.epl.geometry.Point)(geometry_b_), tolerance, progress_tracker);
					}

					default:
					{
						throw new System.ArgumentException();
					}
				}
			}
			else
			{
				if (type_a == com.epl.geometry.Geometry.GeometryType.MultiPoint)
				{
					switch (type_b)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							return MultiPointMinusPolygon_((com.epl.geometry.MultiPoint)(geometry_a), (com.epl.geometry.Polygon)(geometry_b), tolerance, progress_tracker);
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							return MultiPointMinusEnvelope_((com.epl.geometry.MultiPoint)(geometry_a), (com.epl.geometry.Envelope)(geometry_b), tolerance, progress_tracker);
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							return MultiPointMinusPoint_((com.epl.geometry.MultiPoint)(geometry_a), (com.epl.geometry.Point)(geometry_b), tolerance, progress_tracker);
						}

						default:
						{
							break;
						}
					}
				}
			}
			return com.epl.geometry.TopologicalOperations.Difference(geometry_a, geometry_b, spatial_reference, progress_tracker);
		}

		// these are special implementations, all others delegate to the topo-graph.
		internal static com.epl.geometry.Geometry PointMinusPolygon_(com.epl.geometry.Point point, com.epl.geometry.Polygon polygon, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, point, tolerance);
			if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return point;
			}
			return point.CreateInstance();
		}

		internal static com.epl.geometry.Geometry PointMinusPolyline_(com.epl.geometry.Point point, com.epl.geometry.Polyline polyline, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Point2D pt = point.GetXY();
			com.epl.geometry.SegmentIterator seg_iter = polyline.QuerySegmentIterator();
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			double tolerance_cluster_sq = tolerance_cluster * tolerance_cluster;
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			while (seg_iter.NextPath())
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					segment.QueryEnvelope2D(env);
					env.Inflate(tolerance_cluster, tolerance_cluster);
					if (!env.Contains(pt))
					{
						continue;
					}
					if (segment.IsIntersecting(pt, tolerance))
					{
						return point.CreateInstance();
					}
					// check segment end points to the cluster tolerance
					com.epl.geometry.Point2D end_point = segment.GetStartXY();
					if (com.epl.geometry.Point2D.SqrDistance(pt, end_point) <= tolerance_cluster_sq)
					{
						return point.CreateInstance();
					}
					end_point = segment.GetEndXY();
					if (com.epl.geometry.Point2D.SqrDistance(pt, end_point) <= tolerance_cluster_sq)
					{
						return point.CreateInstance();
					}
				}
			}
			return point;
		}

		internal static com.epl.geometry.Geometry PointMinusMultiPoint_(com.epl.geometry.Point point, com.epl.geometry.MultiPoint multi_point, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)(multi_point._getImpl());
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			int point_count = multi_point.GetPointCount();
			com.epl.geometry.Point2D point2D = point.GetXY();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			double tolerance_cluster_sq = tolerance_cluster * tolerance_cluster;
			for (int i = 0; i < point_count; i++)
			{
				position.Read(2 * i, pt);
				double sqr_dist = com.epl.geometry.Point2D.SqrDistance(pt, point2D);
				if (sqr_dist <= tolerance_cluster_sq)
				{
					return point.CreateInstance();
				}
			}
			// return an empty point.
			return point;
		}

		// return the input point
		internal static com.epl.geometry.Geometry PointMinusEnvelope_(com.epl.geometry.Point point, com.epl.geometry.Envelope envelope, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			envelope.QueryEnvelope2D(env);
			env.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D pt = point.GetXY();
			if (!env.Contains(pt))
			{
				return point;
			}
			return point.CreateInstance();
		}

		internal static com.epl.geometry.Geometry PointMinusPoint_(com.epl.geometry.Point point_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			double tolerance_cluster_sq = tolerance_cluster * tolerance_cluster;
			com.epl.geometry.Point2D pt_a = point_a.GetXY();
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) <= tolerance_cluster_sq)
			{
				return point_a.CreateInstance();
			}
			// return empty point
			return point_a;
		}

		internal static com.epl.geometry.Geometry MultiPointMinusPolygon_(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Polygon polygon, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			polygon.QueryEnvelope2D(env);
			env.Inflate(tolerance, tolerance);
			int point_count = multi_point.GetPointCount();
			bool b_found_covered = false;
			bool[] covered = new bool[point_count];
			for (int i = 0; i < point_count; i++)
			{
				covered[i] = false;
			}
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int i_1 = 0; i_1 < point_count; i_1++)
			{
				multi_point.GetXY(i_1, pt);
				if (!env.Contains(pt))
				{
					continue;
				}
				com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, pt, tolerance);
				if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
				{
					continue;
				}
				b_found_covered = true;
				covered[i_1] = true;
			}
			if (!b_found_covered)
			{
				return multi_point;
			}
			com.epl.geometry.MultiPoint new_multipoint = (com.epl.geometry.MultiPoint)multi_point.CreateInstance();
			for (int i_2 = 0; i_2 < point_count; i_2++)
			{
				if (!covered[i_2])
				{
					new_multipoint.Add(multi_point, i_2, i_2 + 1);
				}
			}
			return new_multipoint;
		}

		internal static com.epl.geometry.Geometry MultiPointMinusEnvelope_(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Envelope envelope, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			envelope.QueryEnvelope2D(env);
			env.Inflate(tolerance, tolerance);
			int point_count = multi_point.GetPointCount();
			bool b_found_covered = false;
			bool[] covered = new bool[point_count];
			for (int i = 0; i < point_count; i++)
			{
				covered[i] = false;
			}
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int i_1 = 0; i_1 < point_count; i_1++)
			{
				multi_point.GetXY(i_1, pt);
				if (!env.Contains(pt))
				{
					continue;
				}
				b_found_covered = true;
				covered[i_1] = true;
			}
			if (!b_found_covered)
			{
				return multi_point;
			}
			com.epl.geometry.MultiPoint new_multipoint = (com.epl.geometry.MultiPoint)multi_point.CreateInstance();
			for (int i_2 = 0; i_2 < point_count; i_2++)
			{
				if (!covered[i_2])
				{
					new_multipoint.Add(multi_point, i_2, i_2 + 1);
				}
			}
			return new_multipoint;
		}

		internal static com.epl.geometry.Geometry MultiPointMinusPoint_(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Point point, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)(multi_point._getImpl());
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			int point_count = multi_point.GetPointCount();
			com.epl.geometry.Point2D point2D = point.GetXY();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			bool b_found_covered = false;
			bool[] covered = new bool[point_count];
			for (int i = 0; i < point_count; i++)
			{
				covered[i] = false;
			}
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			double tolerance_cluster_sq = tolerance_cluster * tolerance_cluster;
			for (int i_1 = 0; i_1 < point_count; i_1++)
			{
				position.Read(2 * i_1, pt);
				double sqr_dist = com.epl.geometry.Point2D.SqrDistance(pt, point2D);
				if (sqr_dist <= tolerance_cluster_sq)
				{
					b_found_covered = true;
					covered[i_1] = true;
				}
			}
			if (!b_found_covered)
			{
				return multi_point;
			}
			com.epl.geometry.MultiPoint new_multipoint = (com.epl.geometry.MultiPoint)(multi_point.CreateInstance());
			for (int i_2 = 0; i_2 < point_count; i_2++)
			{
				if (!covered[i_2])
				{
					new_multipoint.Add(multi_point, i_2, i_2 + 1);
				}
			}
			return new_multipoint;
		}

		internal static com.epl.geometry.Geometry PolylineMinusArea_(com.epl.geometry.Geometry geometry, com.epl.geometry.Geometry area, int area_type, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// construct the complement of the Polygon (or Envelope)
			com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope();
			geometry.QueryEnvelope(envelope);
			com.epl.geometry.Envelope2D env_2D = new com.epl.geometry.Envelope2D();
			area.QueryEnvelope2D(env_2D);
			envelope.Merge(env_2D);
			double dw = 0.1 * envelope.GetWidth();
			double dh = 0.1 * envelope.GetHeight();
			envelope.Inflate(dw, dh);
			com.epl.geometry.Polygon complement = new com.epl.geometry.Polygon();
			complement.AddEnvelope(envelope, false);
			com.epl.geometry.MultiPathImpl complementImpl = (com.epl.geometry.MultiPathImpl)(complement._getImpl());
			if (area_type == com.epl.geometry.Geometry.GeometryType.Polygon)
			{
				com.epl.geometry.MultiPathImpl polygonImpl = (com.epl.geometry.MultiPathImpl)(area._getImpl());
				complementImpl.Add(polygonImpl, true);
			}
			else
			{
				complementImpl.AddEnvelope((com.epl.geometry.Envelope)(area), true);
			}
			com.epl.geometry.OperatorFactoryLocal projEnv = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorIntersection operatorIntersection = (com.epl.geometry.OperatorIntersection)projEnv.GetOperator(com.epl.geometry.Operator.Type.Intersection);
			com.epl.geometry.Geometry difference = operatorIntersection.Execute(geometry, complement, sr, progress_tracker);
			return difference;
		}
	}
}
