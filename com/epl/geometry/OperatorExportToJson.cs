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
	/// <summary>Export to JSON format.</summary>
	public abstract class OperatorExportToJson : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ExportToJson;
		}

		/// <summary>Performs the ExportToJson operation</summary>
		/// <returns>Returns a JsonCursor.</returns>
		public abstract com.epl.geometry.JsonCursor Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor);

		/// <summary>Performs the ExportToJson operation</summary>
		/// <returns>Returns a String.</returns>
		public abstract string Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry);

		/// <summary>Performs the ExportToJson operation</summary>
		/// <returns>Returns a String.</returns>
		public abstract string Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry, System.Collections.Generic.IDictionary<string, object> exportProperties);

		public static com.epl.geometry.OperatorExportToJson Local()
		{
			return (com.epl.geometry.OperatorExportToJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToJson);
		}
	}
}
