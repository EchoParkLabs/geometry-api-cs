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
	/// <summary>Symmetric difference (XOR) operation between geometries.</summary>
	public abstract class OperatorSymmetricDifference : com.epl.geometry.Operator, com.epl.geometry.CombineOperator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Difference;
		}

		/// <summary>Performs the Symmetric Difference operation on the geometry set.</summary>
		/// <param name="inputGeometries">is the set of Geometry instances to be XOR'd by rightGeometry.</param>
		/// <param name="rightGeometry">is the Geometry being XOR'd with the inputGeometies.</param>
		/// <returns>
		/// Returns the result of the symmetric difference.
		/// The operator XOR's every geometry in inputGeometries with rightGeometry.
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.GeometryCursor rightGeometry, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Symmetric Difference operation on the two geometries.</summary>
		/// <param name="leftGeometry">is one of the Geometry instances in the XOR operation.</param>
		/// <param name="rightGeometry">is one of the Geometry instances in the XOR operation.</param>
		/// <returns>Returns the result of the symmetric difference.</returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry leftGeometry, com.epl.geometry.Geometry rightGeometry, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorSymmetricDifference Local()
		{
			return (com.epl.geometry.OperatorSymmetricDifference)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.SymmetricDifference);
		}
	}
}
