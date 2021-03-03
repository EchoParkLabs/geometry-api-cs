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
	/// <summary>
	/// Densifies MultiPath geometries by length so that no segments are longer than
	/// given threshold value.
	/// </summary>
	public abstract class OperatorDensifyByLength : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.DensifyByLength;
		}

		/// <summary>Performs the Densify operation on the geometry set.</summary>
		/// <param name="inputGeometries">The geometries to be densified.</param>
		/// <param name="maxLength">
		/// The maximum segment length allowed. Must be a positive value.
		/// Curves are densified to straight segments using the
		/// maxSegmentLength. Curves are split into shorter subcurves such
		/// that the length of subcurves is shorter than maxSegmentLength.
		/// After that the curves are replaced with straight segments.
		/// </param>
		/// <param name="progressTracker"/>
		/// <returns>
		/// Returns the densified geometries (It does nothing to geometries
		/// with dim &lt; 1, but simply passes them along).
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, double maxLength, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Densify operation on the geometry set.</summary>
		/// <param name="inputGeometry">The geometry to be densified.</param>
		/// <param name="maxLength">
		/// The maximum segment length allowed. Must be a positive value.
		/// Curves are densified to straight segments using the
		/// maxSegmentLength. Curves are split into shorter subcurves such
		/// that the length of subcurves is shorter than maxSegmentLength.
		/// After that the curves are replaced with straight segments.
		/// </param>
		/// <param name="progressTracker"/>
		/// <returns>
		/// Returns the densified geometry. (It does nothing to geometries
		/// with dim &lt; 1, but simply passes them along).
		/// </returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, double maxLength, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorDensifyByLength Local()
		{
			return (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
		}
	}
}
