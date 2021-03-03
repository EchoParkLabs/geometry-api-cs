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
	internal class OperatorDensifyByLengthLocal : com.epl.geometry.OperatorDensifyByLength
	{
		public override com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, double maxLength, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (maxLength <= 0)
			{
				// TODO fix geometry exception to match native implementation
				throw new System.ArgumentException();
			}
			// GEOMTHROW(invalid_argument);
			return new com.epl.geometry.OperatorDensifyByLengthCursor(inputGeometries, maxLength, progressTracker);
		}

		public override com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, double maxLength, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.SimpleGeometryCursor inputCursor = new com.epl.geometry.SimpleGeometryCursor(inputGeometry);
			com.epl.geometry.GeometryCursor outputCursor = Execute(inputCursor, maxLength, progressTracker);
			return outputCursor.Next();
		}
	}
}
