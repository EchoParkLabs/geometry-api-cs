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
	internal class OperatorUnionLocal : com.epl.geometry.OperatorUnion
	{
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			return new com.epl.geometry.OperatorUnionCursor(inputGeometries, sr, progressTracker);
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom1, com.epl.geometry.Geometry geom2, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.Geometry[] geomArray = new com.epl.geometry.Geometry[] { geom1, geom2 };
			com.epl.geometry.SimpleGeometryCursor inputGeometries = new com.epl.geometry.SimpleGeometryCursor(geomArray);
			com.epl.geometry.GeometryCursor outputCursor = Execute(inputGeometries, sr, progressTracker);
			return outputCursor.Next();
		}
	}
}
