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
	internal class OperatorExportToJsonLocal : com.epl.geometry.OperatorExportToJson
	{
		public override com.epl.geometry.JsonCursor Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor)
		{
			return new com.epl.geometry.OperatorExportToJsonCursor(spatialReference, geometryCursor);
		}

		public override string Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry)
		{
			com.epl.geometry.SimpleGeometryCursor gc = new com.epl.geometry.SimpleGeometryCursor(geometry);
			com.epl.geometry.JsonCursor cursor = new com.epl.geometry.OperatorExportToJsonCursor(spatialReference, gc);
			return cursor.Next();
		}

		public override string Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			return com.epl.geometry.OperatorExportToJsonCursor.ExportToString(geometry, spatialReference, exportProperties);
		}
	}
}
