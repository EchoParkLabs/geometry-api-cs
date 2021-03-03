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
	/// <summary>Difference of geometries.</summary>
	public abstract class OperatorDifference : com.epl.geometry.Operator, com.epl.geometry.CombineOperator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Difference;
		}

		/// <summary>Performs the Topological Difference operation on the geometry set.</summary>
		/// <param name="inputGeometries">
		/// is the set of Geometry instances to be subtracted by the
		/// subtractor
		/// </param>
		/// <param name="subtractor">is the Geometry being subtracted.</param>
		/// <returns>
		/// Returns the result of the subtraction.
		/// The operator subtracts subtractor from every geometry in
		/// inputGeometries.
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.GeometryCursor subtractor, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Topological Difference operation on the two geometries.</summary>
		/// <param name="inputGeometry">
		/// is the Geometry instance on the left hand side of the
		/// subtraction.
		/// </param>
		/// <param name="subtractor">is the Geometry on the right hand side being subtracted.</param>
		/// <returns>Returns the result of subtraction.</returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.Geometry subtractor, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorDifference Local()
		{
			return (com.epl.geometry.OperatorDifference)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Difference);
		}
	}
}
