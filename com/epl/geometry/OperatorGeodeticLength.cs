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
	/// <summary>Geodetic length calculation.</summary>
	internal abstract class OperatorGeodeticLength : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.GeodeticLength;
		}

		/// <summary>Calculates the geodetic length of the input Geometry.</summary>
		/// <param name="geom">The input Geometry for the geodetic length calculation.</param>
		/// <param name="sr">The SpatialReference of the Geometry.</param>
		/// <param name="geodeticCurveType">
		/// Use the
		/// <see cref="GeodeticCurveType"/>
		/// interface to choose the
		/// interpretation of a line connecting two points.
		/// </param>
		/// <param name="progressTracker"/>
		/// <returns>Returns the geoetic length of the Geometry.</returns>
		public abstract double Execute(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference sr, int geodeticCurveType, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorGeodeticLength Local()
		{
			return (com.epl.geometry.OperatorGeodeticLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.GeodeticLength);
		}
	}
}
