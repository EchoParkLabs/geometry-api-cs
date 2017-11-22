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
	/// <summary>Creates buffer polygons around geometries.</summary>
	public abstract class OperatorBuffer : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Buffer;
		}

		/// <summary>Creates a buffer around the input geometries</summary>
		/// <param name="inputGeometries">The geometries to buffer.</param>
		/// <param name="sr">The SpatialReference of the Geometries.</param>
		/// <param name="distances">The buffer distances for the Geometries. If the size of the distances array is less than the number of geometries in the inputGeometries, the last distance value is used for the rest of geometries.</param>
		/// <param name="bUnion">If True, the buffered geometries will be unioned, otherwise they wont be unioned.</param>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, double[] distances, bool bUnion, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Creates a buffer around the input geometry</summary>
		/// <param name="inputGeometry">The geometry to buffer.</param>
		/// <param name="sr">The SpatialReference of the Geometry.</param>
		/// <param name="distance">The buffer distance for the Geometry.</param>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.SpatialReference sr, double distance, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorBuffer Local()
		{
			return (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
		}
	}
}
