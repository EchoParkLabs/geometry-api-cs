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
	internal abstract class OperatorGeodesicBuffer : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.GeodesicBuffer;
		}

		/// <summary>Creates a geodesic buffer around the input geometries</summary>
		/// <param name="input_geometries">The geometries to buffer.</param>
		/// <param name="sr">The Spatial_reference of the Geometries.</param>
		/// <param name="curveType">
		/// The geodetic curve type of the segments. If the curve_type is Geodetic_curve::shape_preserving, then the segments are densified in the projection where they are defined before
		/// buffering.
		/// </param>
		/// <param name="distancesMeters">
		/// The buffer distances in meters for the Geometries. If the size of the distances array is less than the number of geometries in the input_geometries, the last distance value
		/// is used for the rest of geometries.
		/// </param>
		/// <param name="maxDeviationMeters">
		/// The deviation offset to use for convergence. The geodesic arcs of the resulting buffer will be closer than the max deviation of the true buffer. Pass in NaN to use the
		/// default deviation.
		/// </param>
		/// <param name="bReserved">Must be false. Reserved for future development. Will throw an exception if not false.</param>
		/// <param name="bUnion">If True, the buffered geometries will be unioned, otherwise they wont be unioned.</param>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, int curveType, double[] distancesMeters, double maxDeviationMeters, bool bReserved, bool bUnion, com.epl.geometry.ProgressTracker progressTracker
			);

		/// <summary>Creates a geodesic buffer around the input geometry</summary>
		/// <param name="input_geometry">The geometry to buffer.</param>
		/// <param name="sr">The Spatial_reference of the Geometry.</param>
		/// <param name="curveType">
		/// The geodetic curve type of the segments. If the curve_type is Geodetic_curve::shape_preserving, then the segments are densified in the projection where they are defined before
		/// buffering.
		/// </param>
		/// <param name="distanceMeters">The buffer distance in meters for the Geometry.</param>
		/// <param name="maxDeviationMeters">
		/// The deviation offset to use for convergence. The geodesic arcs of the resulting buffer will be closer than the max deviation of the true buffer. Pass in NaN to use the
		/// default deviation.
		/// </param>
		/// <param name="bReserved">Must be false. Reserved for future development. Will throw an exception if not false.</param>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.SpatialReference sr, int curveType, double distanceMeters, double maxDeviationMeters, bool bReserved, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorGeodesicBuffer Local()
		{
			return (com.epl.geometry.OperatorGeodesicBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.GeodesicBuffer);
		}
	}
}
