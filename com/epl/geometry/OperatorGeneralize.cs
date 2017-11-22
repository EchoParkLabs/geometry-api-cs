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
	/// <summary>Generalizes geometries using Douglas-Peucker algorithm.</summary>
	public abstract class OperatorGeneralize : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Generalize;
		}

		/// <summary>Performs the Generalize operation on a geometry set.</summary>
		/// <remarks>
		/// Performs the Generalize operation on a geometry set. Point and
		/// multipoint geometries are left unchanged. An envelope is converted to a
		/// polygon.
		/// </remarks>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor geoms, double maxDeviation, bool bRemoveDegenerateParts, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Generalize operation on a single geometry.</summary>
		/// <remarks>
		/// Performs the Generalize operation on a single geometry. Point and
		/// multipoint geometries are left unchanged. An envelope is converted to a
		/// polygon.
		/// </remarks>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom, double maxDeviation, bool bRemoveDegenerateParts, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorGeneralize Local()
		{
			return (com.epl.geometry.OperatorGeneralize)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Generalize);
		}
	}
}
