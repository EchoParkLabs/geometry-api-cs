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
	/// <summary>Densifies the line segments by length, making them run along specified geodetic curves.</summary>
	/// <remarks>
	/// Densifies the line segments by length, making them run along specified geodetic curves.
	/// Use this operator to construct geodetic curves.
	/// </remarks>
	internal abstract class OperatorGeodeticDensifyByLength : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.GeodeticDensifyByLength;
		}

		/// <summary>Densifies input geometries.</summary>
		/// <remarks>Densifies input geometries. Attributes are interpolated along the scalar t-values of the input segments obtained from the length ratios along the densified segments.</remarks>
		/// <param name="geoms">The geometries to be densified.</param>
		/// <param name="maxSegmentLengthMeters">The maximum segment length (in meters) allowed. Must be a positive value.</param>
		/// <param name="sr">The SpatialReference of the Geometry.</param>
		/// <param name="curveType">The interpretation of a line connecting two points.</param>
		/// <returns>
		/// Returns the densified geometries (It does nothing to geometries with dim less than 1, but simply passes them along).
		/// Note the behavior is not determined for any geodetic curve segments that connect two poles, or for loxodrome segments that connect to any pole.
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor geoms, double maxSegmentLengthMeters, com.epl.geometry.SpatialReference sr, int curveType, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Same as above, but works with a single geometry.</summary>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom, double maxSegmentLengthMeters, com.epl.geometry.SpatialReference sr, int curveType, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorGeodeticDensifyByLength Local()
		{
			return (com.epl.geometry.OperatorGeodeticDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.GeodeticDensifyByLength);
		}
	}
}
