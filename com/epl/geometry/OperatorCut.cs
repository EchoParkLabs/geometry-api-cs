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
	/// <summary>Splits the target polyline or polygon where it is crossed by the cutter polyline.</summary>
	public abstract class OperatorCut : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Cut;
		}

		/// <summary>Performs the Cut operation on a geometry.</summary>
		/// <param name="bConsiderTouch">
		/// Indicates whether we consider a touch event a cut.
		/// This only applies to polylines, but it's recommended to set this variable to True.
		/// </param>
		/// <param name="cuttee">The input geometry to be cut.</param>
		/// <param name="cutter">
		/// The polyline that will be used to divide the cuttee into
		/// pieces where it crosses the cutter.
		/// </param>
		/// <returns>
		/// Returns a GeometryCursor of cut geometries.
		/// All left cuts will be grouped together in the first geometry. Right cuts and
		/// coincident cuts are grouped in the second geometry, and each undefined cut along
		/// with any uncut parts are output as separate geometries. If there were no cuts
		/// the cursor will return no geometry. If the left or right cut does not
		/// exist, the returned geometry will be empty for this type of cut. An
		/// undefined cut will only be produced if a left cut or right cut was
		/// produced and there was a part left over after cutting or a cut is
		/// bounded to the left and right of the cutter.
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(bool bConsiderTouch, com.epl.geometry.Geometry cuttee, com.epl.geometry.Polyline cutter, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorCut Local()
		{
			return (com.epl.geometry.OperatorCut)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Cut);
		}
	}
}
