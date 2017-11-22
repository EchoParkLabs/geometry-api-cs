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
	/// <summary>Performs the Relation operation between two geometries using the DE-9IM matrix encoded as a string.</summary>
	public abstract class OperatorRelate : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Relate;
		}

		/// <summary>Performs the Relation operation between two geometries using the DE-9IM matrix encoded as a string.</summary>
		/// <param name="inputGeom1">The first geometry in the relation.</param>
		/// <param name="inputGeom2">The second geometry in the relation.</param>
		/// <param name="sr">The spatial reference of the geometries.</param>
		/// <param name="de_9im_string">The DE-9IM matrix relation encoded as a string.</param>
		/// <returns>Returns True if the relation holds, False otherwise.</returns>
		public abstract bool Execute(com.epl.geometry.Geometry inputGeom1, com.epl.geometry.Geometry inputGeom2, com.epl.geometry.SpatialReference sr, string de_9im_string, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorRelate Local()
		{
			return (com.epl.geometry.OperatorRelate)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate);
		}

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
