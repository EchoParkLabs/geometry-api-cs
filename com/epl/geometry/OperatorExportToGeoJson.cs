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

		/// <summary>Performs the ExportToGeoJson operation</summary>
		/// <param name="spatialReference">The SpatialReference of the Geometry. Will be written as "crs":null if the spatialReference is null.</param>
		/// <param name="geometryCursor">The cursor of geometries to write as GeoJson.</param>
		/// <returns>Returns a JsonCursor.</returns>
		public abstract com.epl.geometry.JsonCursor Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor);

		/// <summary>Performs the ExportToGeoJson operation</summary>
		/// <param name="spatialReference">The SpatialReference of the Geometry. Will be written as "crs":null if the spatialReference is null.</param>
		/// <param name="geometry">The Geometry to write as GeoJson.</param>
		/// <returns>Returns a string in GeoJson format.</returns>
		public abstract string Execute(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry);

		/// <summary>Performs the ExportToGeoJson operation</summary>
		/// <param name="exportFlags">
		/// Use the
		/// <see cref="GeoJsonExportFlags"/>
		/// interface.
		/// </param>
		/// <param name="spatialReference">The SpatialReference of the Geometry. Will be written as "crs":null if the spatialReference is null.</param>
		/// <param name="geometry">The Geometry to write as GeoJson.</param>
		/// <returns>Returns a string in GeoJson format.</returns>
		public abstract string Execute(int exportFlags, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry);

		/// <summary>Performs the ExportToGeoJson operation.</summary>
		/// <remarks>Performs the ExportToGeoJson operation. Will not write out a spatial reference or crs tag. Assumes the geometry is in wgs84.</remarks>
		/// <param name="geometry">The Geometry to write as GeoJson.</param>
		/// <returns>Returns a string in GeoJson format.</returns>
		public abstract string Execute(com.epl.geometry.Geometry geometry);

		/// <summary>Performs the ExportToGeoJson operation on a spatial reference.</summary>
		/// <param name="export_flags">The flags used for the export.</param>
		/// <param name="spatial_reference">The spatial reference being exported. Cannot be null.</param>
		/// <returns>Returns the crs value object.</returns>
		public abstract string ExportSpatialReference(int export_flags, com.epl.geometry.SpatialReference spatial_reference);

		public static com.epl.geometry.OperatorExportToGeoJson Local()
		{
			return (com.epl.geometry.OperatorExportToGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
		}
	}
}
