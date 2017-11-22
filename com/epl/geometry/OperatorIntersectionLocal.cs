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
	internal class OperatorIntersectionLocal : com.epl.geometry.OperatorIntersection
	{
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.GeometryCursor intersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			return new com.epl.geometry.OperatorIntersectionCursor(inputGeometries, intersector, sr, progressTracker, -1);
		}

		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor input_geometries, com.epl.geometry.GeometryCursor intersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker, int dimensionMask)
		{
			return new com.epl.geometry.OperatorIntersectionCursor(input_geometries, intersector, sr, progress_tracker, dimensionMask);
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.Geometry intersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.SimpleGeometryCursor inputGeomCurs = new com.epl.geometry.SimpleGeometryCursor(inputGeometry);
			com.epl.geometry.SimpleGeometryCursor intersectorCurs = new com.epl.geometry.SimpleGeometryCursor(intersector);
			com.epl.geometry.GeometryCursor geometryCursor = Execute(inputGeomCurs, intersectorCurs, sr, progressTracker);
			return geometryCursor.Next();
		}

		public override bool AccelerateGeometry(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			if (!CanAccelerateGeometry(geometry))
			{
				return false;
			}
			double tol = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, geometry, false);
			bool accelerated = ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl())._buildQuadTreeAccelerator(accelDegree);
			accelerated |= ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl())._buildRasterizedGeometryAccelerator(tol, accelDegree);
			return accelerated;
		}

		public override bool CanAccelerateGeometry(com.epl.geometry.Geometry geometry)
		{
			return com.epl.geometry.RasterizedGeometry2D.CanUseAccelerator(geometry);
		}
	}
}
