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
	internal class OperatorGeodesicBufferLocal : com.epl.geometry.OperatorGeodesicBuffer
	{
		//This is a stub
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, int curveType, double[] distancesMeters, double maxDeviationMeters, bool bReserved, bool bUnion, com.epl.geometry.ProgressTracker progressTracker
			)
		{
			throw new com.epl.geometry.GeometryException("not implemented");
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.SpatialReference sr, int curveType, double distanceMeters, double maxDeviationMeters, bool bReserved, com.epl.geometry.ProgressTracker progressTracker)
		{
			throw new com.epl.geometry.GeometryException("not implemented");
		}
	}
}
