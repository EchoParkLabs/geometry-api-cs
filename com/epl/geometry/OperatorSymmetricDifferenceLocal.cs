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
	internal class OperatorSymmetricDifferenceLocal : com.epl.geometry.OperatorSymmetricDifference
	{
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.GeometryCursor rightGeometry, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			return new com.epl.geometry.OperatorSymmetricDifferenceCursor(inputGeometries, rightGeometry, sr, progressTracker);
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry leftGeometry, com.epl.geometry.Geometry rightGeometry, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.SimpleGeometryCursor leftGeomCurs = new com.epl.geometry.SimpleGeometryCursor(leftGeometry);
			com.epl.geometry.SimpleGeometryCursor rightGeomCurs = new com.epl.geometry.SimpleGeometryCursor(rightGeometry);
			com.epl.geometry.GeometryCursor geometryCursor = Execute(leftGeomCurs, rightGeomCurs, sr, progressTracker);
			return geometryCursor.Next();
		}

		internal static com.epl.geometry.Geometry SymmetricDifference(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference spatial_reference, com.epl.geometry.ProgressTracker progress_tracker)
		{
			int dim_a = geometry_a.GetDimension();
			int dim_b = geometry_b.GetDimension();
			if (geometry_a.IsEmpty() && geometry_b.IsEmpty())
			{
				return dim_a > dim_b ? geometry_a : geometry_b;
			}
			if (geometry_a.IsEmpty())
			{
				return geometry_b;
			}
			if (geometry_b.IsEmpty())
			{
				return geometry_a;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_merged = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env_a);
			geometry_b.QueryEnvelope2D(env_b);
			env_merged.SetCoords(env_a);
			env_merged.Merge(env_b);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatial_reference, env_merged, false);
			int type_a = geometry_a.GetType().Value();
			int type_b = geometry_b.GetType().Value();
			if (type_a == com.epl.geometry.Geometry.GeometryType.Point && type_b == com.epl.geometry.Geometry.GeometryType.Point)
			{
				return PointSymDiffPoint_((com.epl.geometry.Point)(geometry_a), (com.epl.geometry.Point)(geometry_b), tolerance, progress_tracker);
			}
			if (type_a != type_b)
			{
				if (dim_a > 0 || dim_b > 0)
				{
					return dim_a > dim_b ? geometry_a : geometry_b;
				}
				// Multi_point/Point case
				if (type_a == com.epl.geometry.Geometry.GeometryType.MultiPoint)
				{
					return MultiPointSymDiffPoint_((com.epl.geometry.MultiPoint)(geometry_a), (com.epl.geometry.Point)(geometry_b), tolerance, progress_tracker);
				}
				return MultiPointSymDiffPoint_((com.epl.geometry.MultiPoint)(geometry_b), (com.epl.geometry.Point)(geometry_a), tolerance, progress_tracker);
			}
			return com.epl.geometry.TopologicalOperations.SymmetricDifference(geometry_a, geometry_b, spatial_reference, progress_tracker);
		}

		internal static com.epl.geometry.Geometry PointSymDiffPoint_(com.epl.geometry.Point point_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			double tolerance_cluster_sq = tolerance_cluster * tolerance_cluster;
			com.epl.geometry.Point2D pt_a = point_a.GetXY();
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			com.epl.geometry.MultiPoint multi_point = new com.epl.geometry.MultiPoint(point_a.GetDescription());
			if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) > tolerance_cluster_sq)
			{
				multi_point.Add(point_a);
				multi_point.Add(point_b);
			}
			return multi_point;
		}

		internal static com.epl.geometry.Geometry MultiPointSymDiffPoint_(com.epl.geometry.MultiPoint multi_point, com.epl.geometry.Point point, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)(multi_point._getImpl());
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			int point_count = multi_point.GetPointCount();
			com.epl.geometry.Point2D point2D = point.GetXY();
			com.epl.geometry.MultiPoint new_multipoint = (com.epl.geometry.MultiPoint)(multi_point.CreateInstance());
			double tolerance_cluster = tolerance * System.Math.Sqrt(2.0) * 1.00001;
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			multi_point.QueryEnvelope2D(env);
			env.Inflate(tolerance_cluster, tolerance_cluster);
			if (env.Contains(point2D))
			{
				double tolerance_cluster_sq = tolerance_cluster * tolerance_cluster;
				bool b_found_covered = false;
				bool[] covered = new bool[point_count];
				for (int i = 0; i < point_count; i++)
				{
					covered[i] = false;
				}
				for (int i_1 = 0; i_1 < point_count; i_1++)
				{
					double x = position.Read(2 * i_1);
					double y = position.Read(2 * i_1 + 1);
					double dx = x - point2D.x;
					double dy = y - point2D.y;
					if (dx * dx + dy * dy <= tolerance_cluster_sq)
					{
						b_found_covered = true;
						covered[i_1] = true;
					}
				}
				if (!b_found_covered)
				{
					new_multipoint.Add(multi_point, 0, point_count);
					new_multipoint.Add(point);
				}
				else
				{
					for (int i_2 = 0; i_2 < point_count; i_2++)
					{
						if (!covered[i_2])
						{
							new_multipoint.Add(multi_point, i_2, i_2 + 1);
						}
					}
				}
			}
			else
			{
				new_multipoint.Add(multi_point, 0, point_count);
				new_multipoint.Add(point);
			}
			return new_multipoint;
		}
	}
}
