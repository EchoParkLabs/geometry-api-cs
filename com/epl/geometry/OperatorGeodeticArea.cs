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
	/// <summary>A base class for the ExportToESRIShape Operator.</summary>
	internal abstract class OperatorGeodeticArea : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.GeodeticArea;
		}

		/// <summary>Calculates the geodetic area of each geometry in the geometry cursor.</summary>
		/// <param name="geoms">
		/// The geometry cursor to be iterated over to perform the
		/// Geodetic Area calculation.
		/// </param>
		/// <param name="sr">The SpatialReference of the geometries.</param>
		/// <param name="geodeticCurveType">
		/// Use the
		/// <see cref="GeodeticCurveType"/>
		/// interface to choose the
		/// interpretation of a line connecting two points.
		/// </param>
		/// <param name="progressTracker"/>
		/// <returns>Returns an array of the geodetic areas of the geometries.</returns>
		public abstract double[] Execute(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.SpatialReference sr, int geodeticCurveType, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Calculates the geodetic area of the input Geometry.</summary>
		/// <param name="geom">The input Geometry for the geodetic area calculation.</param>
		/// <param name="sr">The SpatialReference of the Geometry.</param>
		/// <param name="geodeticCurveType">
		/// Use the
		/// <see cref="GeodeticCurveType"/>
		/// interface to choose the
		/// interpretation of a line connecting two points.
		/// </param>
		/// <param name="progressTracker"/>
		/// <returns>Returns the geodetic area of the Geometry.</returns>
		public abstract double Execute(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference sr, int geodeticCurveType, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorGeodeticArea Local()
		{
			return (com.epl.geometry.OperatorGeodeticArea)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.GeodeticArea);
		}
	}
}
