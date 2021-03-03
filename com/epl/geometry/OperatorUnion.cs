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
	/// <summary>Union of geometries.</summary>
	public abstract class OperatorUnion : com.epl.geometry.Operator, com.epl.geometry.CombineOperator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Union;
		}

		/// <summary>Performs the Topological Union operation on the geometry set.</summary>
		/// <param name="inputGeometries">is the set of Geometry instances to be unioned.</param>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Topological Union operation on two geometries.</summary>
		/// <param name="geom1">and geom2 are the geometry instances to be unioned.</param>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom1, com.epl.geometry.Geometry geom2, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorUnion Local()
		{
			return (com.epl.geometry.OperatorUnion)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Union);
		}
	}
}
