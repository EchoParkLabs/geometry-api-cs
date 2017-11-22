/*
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
Environmental Systems Research Institute, Inc.
Attn: Contracts Dept
380 New York Street
Redlands, California, USA 92373

email: contracts@esri.com
*/


namespace com.epl.geometry
{
	/// <summary>Export to GeoJson format.</summary>
	public abstract class OperatorExportToGeoJson : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ExportToGeoJson;
		}

		public abstract com.epl.geometry.JsonCursor Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor);

		public abstract string Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry);

		public abstract string Execute(int exportFlags, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry);

		public abstract string Execute(com.epl.geometry.Geometry geometry);

		public static com.epl.geometry.OperatorExportToGeoJson Local()
		{
			return (com.epl.geometry.OperatorExportToGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
		}
	}
}
