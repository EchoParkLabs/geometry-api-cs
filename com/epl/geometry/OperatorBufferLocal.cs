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
	internal class OperatorBufferLocal : com.epl.geometry.OperatorBuffer
	{
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, double[] distances, bool bUnion, com.epl.geometry.ProgressTracker progressTracker)
		{
			return Execute(inputGeometries, sr, distances, com.epl.geometry.NumberUtils.NaN(), 96, bUnion, progressTracker);
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.SpatialReference sr, double distance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.SimpleGeometryCursor inputCursor = new com.epl.geometry.SimpleGeometryCursor(inputGeometry);
			double[] distances = new double[1];
			distances[0] = distance;
			com.epl.geometry.GeometryCursor outputCursor = Execute(inputCursor, sr, distances, false, progressTracker);
			return outputCursor.Next();
		}

		internal override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, double[] distances, double max_deviation, int max_vertices_in_full_circle, bool b_union, com.epl.geometry.ProgressTracker progressTracker
			)
		{
			if (b_union)
			{
				com.epl.geometry.OperatorBufferCursor cursor = new com.epl.geometry.OperatorBufferCursor(inputGeometries, sr, distances, max_deviation, max_vertices_in_full_circle, false, progressTracker);
				return com.epl.geometry.OperatorUnion.Local().Execute(cursor, sr, progressTracker);
			}
			else
			{
				// (int)Operator_union::Options::enum_disable_edge_dissolver
				return new com.epl.geometry.OperatorBufferCursor(inputGeometries, sr, distances, max_deviation, max_vertices_in_full_circle, false, progressTracker);
			}
		}
	}
}
