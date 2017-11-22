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
	internal class OperatorRelateLocal : com.epl.geometry.OperatorRelate
	{
		public override bool Execute(com.epl.geometry.Geometry inputGeom1, com.epl.geometry.Geometry inputGeom2, com.epl.geometry.SpatialReference sr, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return com.epl.geometry.RelationalOperationsMatrix.Relate(inputGeom1, inputGeom2, sr, scl, progress_tracker);
		}
	}
}
