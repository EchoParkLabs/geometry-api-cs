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
	/// <summary>Densifies geometries preserving the shape of the segments in a given spatial reference by length and/or deviation.</summary>
	/// <remarks>
	/// Densifies geometries preserving the shape of the segments in a given spatial reference by length and/or deviation. The elliptic arc lengths of the resulting line segments are no longer than the
	/// given max length, and the line segments will be closer than the given max deviation to both the original segment curve and the joining elliptic arcs.
	/// </remarks>
	internal abstract class OperatorShapePreservingDensify : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ShapePreservingDensify;
		}

		/// <summary>Performs the Shape Preserving Densify operation on the geometry set.</summary>
		/// <remarks>
		/// Performs the Shape Preserving Densify operation on the geometry set. Attributes are interpolated along the scalar t-values of the input segments obtained from the length ratios along the
		/// densified segments.
		/// </remarks>
		/// <param name="geoms">The geometries to be densified.</param>
		/// <param name="sr">The spatial reference of the geometries.</param>
		/// <param name="maxLengthMeters">The maximum segment length allowed. Must be a positive value to be used. Pass zero or NaN to disable densification by length.</param>
		/// <param name="maxDeviationMeters">The maximum deviation. Must be a positive value to be used. Pass zero or NaN to disable densification by deviation.</param>
		/// <param name="reserved">Must be 0 or NaN. Reserved for future use. Throws and exception if not NaN or 0.</param>
		/// <returns>
		/// Returns the densified geometries (It does nothing to geometries with dim &lt; 1, but simply passes them along).
		/// The operation always starts from the lowest point on the segment, thus guaranteeing that topologically equal segments are always densified exactly the same.
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.SpatialReference sr, double maxLengthMeters, double maxDeviationMeters, double reserved, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Shape Preserving Densify operation on the geometry.</summary>
		/// <remarks>
		/// Performs the Shape Preserving Densify operation on the geometry. Attributes are interpolated along the scalar t-values of the input segments obtained from the length ratios along the densified
		/// segments.
		/// </remarks>
		/// <param name="geom">The geometry to be densified.</param>
		/// <param name="sr">The spatial reference of the geometry.</param>
		/// <param name="maxLengthMeters">The maximum segment length allowed. Must be a positive value to be used. Pass zero or NaN to disable densification by length.</param>
		/// <param name="maxDeviationMeters">The maximum deviation. Must be a positive value to be used. Pass zero or NaN to disable densification by deviation.</param>
		/// <param name="reserved">Must be 0 or NaN. Reserved for future use. Throws and exception if not NaN or 0.</param>
		/// <returns>
		/// Returns the densified geometries (It does nothing to geometries with dim &lt; 1, but simply passes them along).
		/// The operation always starts from the lowest point on the segment, thus guaranteeing that topologically equal segments are always densified exactly the same.
		/// </returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference sr, double maxLengthMeters, double maxDeviationMeters, double reserved, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorShapePreservingDensify Local()
		{
			return (com.epl.geometry.OperatorShapePreservingDensify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ShapePreservingDensify);
		}
	}
}
