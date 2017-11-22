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
	public abstract class OperatorImportFromGeoJson : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ImportFromGeoJson;
		}

		/// <summary>Performs the ImportFromGeoJson operation.</summary>
		/// <param name="import_flags">
		/// Use the
		/// <see cref="GeoJsonImportFlags"/>
		/// interface.
		/// </param>
		/// <param name="type">
		/// Use the
		/// <see cref="Type"/>
		/// enum.
		/// </param>
		/// <param name="geoJsonString">The string holding the Geometry in geoJson format.</param>
		/// <returns>Returns the imported Geometry.</returns>
		/// <exception cref="org.json.JSONException"></exception>
		public abstract com.epl.geometry.MapGeometry Execute(int import_flags, com.epl.geometry.Geometry.Type type, string geoJsonString, com.epl.geometry.ProgressTracker progress_tracker);

		/// <summary>Performs the ImportFromGeoJson operation.</summary>
		/// <param name="import_flags">
		/// Use the
		/// <see cref="GeoJsonImportFlags"/>
		/// interface.
		/// </param>
		/// <param name="geoJsonString">The string holding the Geometry in geoJson format.</param>
		/// <returns>Returns the imported MapOGCStructure.</returns>
		/// <exception cref="org.json.JSONException"></exception>
		public abstract com.epl.geometry.MapOGCStructure ExecuteOGC(int import_flags, string geoJsonString, com.epl.geometry.ProgressTracker progress_tracker);

		public static com.epl.geometry.OperatorImportFromGeoJson Local()
		{
			return (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
		}
	}
}
