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
	/// <summary>Calculates distance between geometries.</summary>
	public abstract class OperatorDistance : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Distance;
		}

		/// <summary>Calculates distance between two geometries.</summary>
		public abstract double Execute(com.epl.geometry.Geometry geom1, com.epl.geometry.Geometry geom2, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorDistance Local()
		{
			return (com.epl.geometry.OperatorDistance)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Distance);
		}
	}
}
