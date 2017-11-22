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
	/// <summary>A base class for simple relation operators.</summary>
	public abstract class OperatorSimpleRelation : com.epl.geometry.Operator
	{
		/// <summary>Performs the given relation operation between two geometries.</summary>
		/// <returns>Returns True if the relation holds, False otherwise.</returns>
		public abstract bool Execute(com.epl.geometry.Geometry inputGeom1, com.epl.geometry.Geometry inputGeom2, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		public override bool CanAccelerateGeometry(com.epl.geometry.Geometry geometry)
		{
			return com.epl.geometry.RelationalOperations.Accelerate_helper.Can_accelerate_geometry(geometry);
		}

		public override bool AccelerateGeometry(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			return com.epl.geometry.RelationalOperations.Accelerate_helper.Accelerate_geometry(geometry, spatialReference, accelDegree);
		}
	}
}
