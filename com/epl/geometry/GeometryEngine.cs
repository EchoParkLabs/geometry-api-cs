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
	/// <summary>Provides services that operate on geometry instances.</summary>
	/// <remarks>
	/// Provides services that operate on geometry instances. The methods of GeometryEngine class call corresponding OperatorXXX classes.
	/// Consider using OperatorXXX classes directly as they often provide more functionality and better performance. For example, some Operators accept
	/// GeometryCursor class that could be implemented to wrap a feature cursor and make it feed geometries directly into an Operator.
	/// Also, some operators provide a way to accelerate an operation by using Operator.accelerateGeometry method.
	/// </remarks>
	public class GeometryEngine
	{
		private static com.epl.geometry.OperatorFactoryLocal factory = com.epl.geometry.OperatorFactoryLocal.GetInstance();

//		/// <summary>Imports the MapGeometry from its JSON representation.</summary>
//		/// <remarks>
//		/// Imports the MapGeometry from its JSON representation. M and Z values are
//		/// not imported from JSON representation.
//		/// See OperatorImportFromJson.
//		/// </remarks>
//		/// <param name="json">
//		/// The JSON representation of the geometry (with spatial
//		/// reference).
//		/// </param>
//		/// <returns>
//		/// The MapGeometry instance containing the imported geometry and its
//		/// spatial reference.
//		/// </returns>
//		public static com.epl.geometry.MapGeometry JsonToGeometry(com.fasterxml.jackson.core.JsonParser json)
//		{
//			com.epl.geometry.MapGeometry geom = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, new com.epl.geometry.JsonParserReader(json));
//			return geom;
//		}
//
//		/// <summary>Imports the MapGeometry from its JSON representation.</summary>
//		/// <remarks>
//		/// Imports the MapGeometry from its JSON representation. M and Z values are
//		/// not imported from JSON representation.
//		/// See OperatorImportFromJson.
//		/// </remarks>
//		/// <param name="json">
//		/// The JSON representation of the geometry (with spatial
//		/// reference).
//		/// </param>
//		/// <returns>
//		/// The MapGeometry instance containing the imported geometry and its
//		/// spatial reference.
//		/// </returns>
//		public static com.epl.geometry.MapGeometry JsonToGeometry(com.epl.geometry.JsonReader json)
//		{
//			com.epl.geometry.MapGeometry geom = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, json);
//			return geom;
//		}
//
//		/// <summary>Imports the MapGeometry from its JSON representation.</summary>
//		/// <remarks>
//		/// Imports the MapGeometry from its JSON representation. M and Z values are
//		/// not imported from JSON representation.
//		/// See OperatorImportFromJson.
//		/// </remarks>
//		/// <param name="json">
//		/// The JSON representation of the geometry (with spatial
//		/// reference).
//		/// </param>
//		/// <returns>
//		/// The MapGeometry instance containing the imported geometry and its
//		/// spatial reference.
//		/// </returns>
//		/// <exception cref="System.IO.IOException"></exception>
//		/// <exception cref="JsonParseException"></exception>
//		public static com.epl.geometry.MapGeometry JsonToGeometry(string json)
//		{
//			com.epl.geometry.MapGeometry geom = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, json);
//			return geom;
//		}
//
//		/// <summary>Exports the specified geometry instance to it's JSON representation.</summary>
//		/// <remarks>
//		/// Exports the specified geometry instance to it's JSON representation.
//		/// See OperatorExportToJson.
//		/// </remarks>
//		/// <seealso cref="GeometryToJson(SpatialReference, Geometry)"/>
//		/// <param name="wkid">
//		/// The spatial reference Well Known ID to be used for the JSON
//		/// representation.
//		/// </param>
//		/// <param name="geometry">The geometry to be exported to JSON.</param>
//		/// <returns>The JSON representation of the specified Geometry.</returns>
//		public static string GeometryToJson(int wkid, com.epl.geometry.Geometry geometry)
//		{
//			return com.epl.geometry.GeometryEngine.GeometryToJson(wkid > 0 ? com.epl.geometry.SpatialReference.Create(wkid) : null, geometry);
//		}
//
//		/// <summary>Exports the specified geometry instance to it's JSON representation.</summary>
//		/// <remarks>
//		/// Exports the specified geometry instance to it's JSON representation. M
//		/// and Z values are not imported from JSON representation.
//		/// See OperatorExportToJson.
//		/// </remarks>
//		/// <param name="spatialReference">The spatial reference of associated object.</param>
//		/// <param name="geometry">The geometry.</param>
//		/// <returns>The JSON representation of the specified geometry.</returns>
//		public static string GeometryToJson(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry)
//		{
//			com.epl.geometry.OperatorExportToJson exporter = (com.epl.geometry.OperatorExportToJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToJson);
//			return exporter.Execute(spatialReference, geometry);
//		}
//
//		public static string GeometryToGeoJson(com.epl.geometry.Geometry geometry)
//		{
//			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
//			return exporter.Execute(geometry);
//		}
//
//		/// <summary>Imports the MapGeometry from its JSON representation.</summary>
//		/// <remarks>
//		/// Imports the MapGeometry from its JSON representation. M and Z values are
//		/// not imported from JSON representation.
//		/// See OperatorImportFromJson.
//		/// </remarks>
//		/// <param name="json">
//		/// The JSON representation of the geometry (with spatial
//		/// reference).
//		/// </param>
//		/// <returns>
//		/// The MapGeometry instance containing the imported geometry and its
//		/// spatial reference.
//		/// </returns>
//		/// <exception cref="System.IO.IOException"></exception>
//		/// <exception cref="JsonParseException"></exception>
//		public static com.epl.geometry.MapGeometry GeoJsonToGeometry(string json, int importFlags, com.epl.geometry.Geometry.Type type)
//		{
//			com.epl.geometry.MapGeometry geom = com.epl.geometry.OperatorImportFromGeoJson.Local().Execute(importFlags, type, json, null);
//			return geom;
//		}
//
//		/// <summary>Exports the specified geometry instance to its GeoJSON representation.</summary>
//		/// <remarks>
//		/// Exports the specified geometry instance to its GeoJSON representation.
//		/// See OperatorExportToGeoJson.
//		/// </remarks>
//		/// <seealso cref="GeometryToGeoJson(SpatialReference, Geometry)"/>
//		/// <param name="wkid">
//		/// The spatial reference Well Known ID to be used for the GeoJSON
//		/// representation.
//		/// </param>
//		/// <param name="geometry">The geometry to be exported to GeoJSON.</param>
//		/// <returns>The GeoJSON representation of the specified geometry.</returns>
//		public static string GeometryToGeoJson(int wkid, com.epl.geometry.Geometry geometry)
//		{
//			return com.epl.geometry.GeometryEngine.GeometryToGeoJson(wkid > 0 ? com.epl.geometry.SpatialReference.Create(wkid) : null, geometry);
//		}
//
//		/// <summary>Exports the specified geometry instance to it's JSON representation.</summary>
//		/// <remarks>
//		/// Exports the specified geometry instance to it's JSON representation.
//		/// See OperatorImportFromGeoJson.
//		/// </remarks>
//		/// <param name="spatialReference">The spatial reference of associated object.</param>
//		/// <param name="geometry">The geometry.</param>
//		/// <returns>The GeoJSON representation of the specified geometry.</returns>
//		public static string GeometryToGeoJson(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry geometry)
//		{
//			com.epl.geometry.OperatorExportToGeoJson exporter = (com.epl.geometry.OperatorExportToGeoJson)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
//			return exporter.Execute(spatialReference, geometry);
//		}

		/// <summary>Imports geometry from the ESRI shape file format.</summary>
		/// <remarks>
		/// Imports geometry from the ESRI shape file format.
		/// See OperatorImportFromESRIShape.
		/// </remarks>
		/// <param name="esriShapeBuffer">The buffer containing geometry in the ESRI shape file format.</param>
		/// <param name="geometryType">
		/// The required type of the Geometry to be imported. Use
		/// Geometry.Type.Unknown if the geometry type needs to be
		/// determined from the buffer content.
		/// </param>
		/// <returns>The geometry or null if the buffer contains null shape.</returns>
		/// <exception cref="GeometryException">
		/// when the geometryType is not Geometry.Type.Unknown and the
		/// buffer contains geometry that cannot be converted to the
		/// given geometryType. or the buffer is corrupt. Another
		/// exception possible is IllegalArgumentsException.
		/// </exception>
		public static com.epl.geometry.Geometry GeometryFromEsriShape(byte[] esriShapeBuffer, com.epl.geometry.Geometry.Type geometryType)
		{
			com.epl.geometry.OperatorImportFromESRIShape op = (com.epl.geometry.OperatorImportFromESRIShape)factory.GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			return op.Execute(com.epl.geometry.ShapeImportFlags.ShapeImportNonTrusted, geometryType, new System.IO.MemoryStream(esriShapeBuffer));
		}

		/// <summary>Exports geometry to the ESRI shape file format.</summary>
		/// <remarks>
		/// Exports geometry to the ESRI shape file format.
		/// See OperatorExportToESRIShape.
		/// </remarks>
		/// <param name="geometry">The geometry to export. (null value is not allowed)</param>
		/// <returns>Array containing the exported ESRI shape file.</returns>
		public static byte[] GeometryToEsriShape(com.epl.geometry.Geometry geometry)
		{
			if (geometry == null)
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.OperatorExportToESRIShape op = (com.epl.geometry.OperatorExportToESRIShape)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToESRIShape);
			return ((byte[])op.Execute(0, geometry).ToArray());
		}

		/// <summary>Imports a geometry from a WKT string.</summary>
		/// <remarks>
		/// Imports a geometry from a WKT string.
		/// See OperatorImportFromWkt.
		/// </remarks>
		/// <param name="wkt">The string containing the geometry in WKT format.</param>
		/// <param name="importFlags">
		/// Use the
		/// <see cref="WktImportFlags"/>
		/// interface.
		/// </param>
		/// <param name="geometryType">The required type of the Geometry to be imported. Use Geometry.Type.Unknown if the geometry type needs to be determined from the WKT context.</param>
		/// <returns>The geometry.</returns>
		/// <exception cref="GeometryException">when the geometryType is not Geometry.Type.Unknown and the WKT contains a geometry that cannot be converted to the given geometryType.</exception>
		/// <exception cref="IllegalArgument">exception if an error is found while parsing the WKT string.</exception>
		public static com.epl.geometry.Geometry GeometryFromWkt(string wkt, int importFlags, com.epl.geometry.Geometry.Type geometryType)
		{
			com.epl.geometry.OperatorImportFromWkt op = (com.epl.geometry.OperatorImportFromWkt)factory.GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			return op.Execute(importFlags, geometryType, wkt, null);
		}

		/// <summary>Exports a geometry to a string in WKT format.</summary>
		/// <remarks>
		/// Exports a geometry to a string in WKT format.
		/// See OperatorExportToWkt.
		/// </remarks>
		/// <param name="geometry">The geometry to export. (null value is not allowed)</param>
		/// <param name="exportFlags">
		/// Use the
		/// <see cref="WktExportFlags"/>
		/// interface.
		/// </param>
		/// <returns>A String containing the exported geometry in WKT format.</returns>
		public static string GeometryToWkt(com.epl.geometry.Geometry geometry, int exportFlags)
		{
			com.epl.geometry.OperatorExportToWkt op = (com.epl.geometry.OperatorExportToWkt)factory.GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
			return op.Execute(exportFlags, geometry, null);
		}

		/// <summary>Constructs a new geometry by union an array of geometries.</summary>
		/// <remarks>
		/// Constructs a new geometry by union an array of geometries. All inputs
		/// must be of the same type of geometries and share one spatial reference.
		/// See OperatorUnion.
		/// </remarks>
		/// <param name="geometries">The geometries to union.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>The geometry object representing the resultant union.</returns>
		public static com.epl.geometry.Geometry Union(com.epl.geometry.Geometry[] geometries, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorUnion op = (com.epl.geometry.OperatorUnion)factory.GetOperator(com.epl.geometry.Operator.Type.Union);
			com.epl.geometry.SimpleGeometryCursor inputGeometries = new com.epl.geometry.SimpleGeometryCursor(geometries);
			com.epl.geometry.GeometryCursor result = op.Execute(inputGeometries, spatialReference, null);
			return result.Next();
		}

		/// <summary>Creates the difference of two geometries.</summary>
		/// <remarks>
		/// Creates the difference of two geometries. The dimension of geometry2 has
		/// to be equal to or greater than that of geometry1.
		/// See OperatorDifference.
		/// </remarks>
		/// <param name="geometry1">The geometry being subtracted.</param>
		/// <param name="substractor">The geometry object to subtract from.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>The geometry of the differences.</returns>
		public static com.epl.geometry.Geometry Difference(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry substractor, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorDifference op = (com.epl.geometry.OperatorDifference)factory.GetOperator(com.epl.geometry.Operator.Type.Difference);
			com.epl.geometry.Geometry result = op.Execute(geometry1, substractor, spatialReference, null);
			return result;
		}

		/// <summary>Creates the symmetric difference of two geometries.</summary>
		/// <remarks>
		/// Creates the symmetric difference of two geometries.
		/// See OperatorSymmetricDifference.
		/// </remarks>
		/// <param name="leftGeometry">is one of the Geometry instances in the XOR operation.</param>
		/// <param name="rightGeometry">is one of the Geometry instances in the XOR operation.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>Returns the result of the symmetric difference.</returns>
		public static com.epl.geometry.Geometry SymmetricDifference(com.epl.geometry.Geometry leftGeometry, com.epl.geometry.Geometry rightGeometry, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorSymmetricDifference op = (com.epl.geometry.OperatorSymmetricDifference)factory.GetOperator(com.epl.geometry.Operator.Type.SymmetricDifference);
			com.epl.geometry.Geometry result = op.Execute(leftGeometry, rightGeometry, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if two geometries are equal.</summary>
		/// <remarks>
		/// Indicates if two geometries are equal.
		/// See OperatorEquals.
		/// </remarks>
		/// <param name="geometry1">Geometry.</param>
		/// <param name="geometry2">Geometry.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>TRUE if both geometry objects are equal.</returns>
		public static bool Equals(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorEquals op = (com.epl.geometry.OperatorEquals)factory.GetOperator(com.epl.geometry.Operator.Type.Equals);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>See OperatorDisjoint.</summary>
		public static bool Disjoint(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorDisjoint op = (com.epl.geometry.OperatorDisjoint)factory.GetOperator(com.epl.geometry.Operator.Type.Disjoint);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>
		/// Constructs the set-theoretic intersection between an array of geometries
		/// and another geometry.
		/// </summary>
		/// <remarks>
		/// Constructs the set-theoretic intersection between an array of geometries
		/// and another geometry.
		/// See OperatorIntersection (also for dimension specific intersection).
		/// </remarks>
		/// <param name="inputGeometries">An array of geometry objects.</param>
		/// <param name="geometry">The geometry object.</param>
		/// <returns>Any array of geometry objects showing the intersection.</returns>
		internal static com.epl.geometry.Geometry[] Intersect(com.epl.geometry.Geometry[] inputGeometries, com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorIntersection op = (com.epl.geometry.OperatorIntersection)factory.GetOperator(com.epl.geometry.Operator.Type.Intersection);
			com.epl.geometry.SimpleGeometryCursor inputGeometriesCursor = new com.epl.geometry.SimpleGeometryCursor(inputGeometries);
			com.epl.geometry.SimpleGeometryCursor intersectorCursor = new com.epl.geometry.SimpleGeometryCursor(geometry);
			com.epl.geometry.GeometryCursor result = op.Execute(inputGeometriesCursor, intersectorCursor, spatialReference, null);
			System.Collections.Generic.List<com.epl.geometry.Geometry> resultGeoms = new System.Collections.Generic.List<com.epl.geometry.Geometry>();
			com.epl.geometry.Geometry g;
			while ((g = result.Next()) != null)
			{
				resultGeoms.Add(g);
			}
			com.epl.geometry.Geometry[] resultarr = resultGeoms.ToArray();
			return resultarr;
		}

		/// <summary>Creates a geometry through intersection between two geometries.</summary>
		/// <remarks>
		/// Creates a geometry through intersection between two geometries.
		/// See OperatorIntersection.
		/// </remarks>
		/// <param name="geometry1">The first geometry.</param>
		/// <param name="intersector">The geometry to intersect the first geometry.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>The geometry created through intersection.</returns>
		public static com.epl.geometry.Geometry Intersect(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry intersector, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorIntersection op = (com.epl.geometry.OperatorIntersection)factory.GetOperator(com.epl.geometry.Operator.Type.Intersection);
			com.epl.geometry.Geometry result = op.Execute(geometry1, intersector, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if one geometry is within another geometry.</summary>
		/// <remarks>
		/// Indicates if one geometry is within another geometry.
		/// See OperatorWithin.
		/// </remarks>
		/// <param name="geometry1">
		/// The base geometry that is tested for within relationship to
		/// the other geometry.
		/// </param>
		/// <param name="geometry2">
		/// The comparison geometry that is tested for the contains
		/// relationship to the other geometry.
		/// </param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>TRUE if the first geometry is within the other geometry.</returns>
		public static bool Within(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorWithin op = (com.epl.geometry.OperatorWithin)factory.GetOperator(com.epl.geometry.Operator.Type.Within);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if one geometry contains another geometry.</summary>
		/// <remarks>
		/// Indicates if one geometry contains another geometry.
		/// See OperatorContains.
		/// </remarks>
		/// <param name="geometry1">
		/// The geometry that is tested for the contains relationship to
		/// the other geometry..
		/// </param>
		/// <param name="geometry2">
		/// The geometry that is tested for within relationship to the
		/// other geometry.
		/// </param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>TRUE if geometry1 contains geometry2.</returns>
		public static bool Contains(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)factory.GetOperator(com.epl.geometry.Operator.Type.Contains);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if one geometry crosses another geometry.</summary>
		/// <remarks>
		/// Indicates if one geometry crosses another geometry.
		/// See OperatorCrosses.
		/// </remarks>
		/// <param name="geometry1">The geometry to cross.</param>
		/// <param name="geometry2">The geometry being crossed.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>TRUE if geometry1 crosses geometry2.</returns>
		public static bool Crosses(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorCrosses op = (com.epl.geometry.OperatorCrosses)factory.GetOperator(com.epl.geometry.Operator.Type.Crosses);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if one geometry touches another geometry.</summary>
		/// <remarks>
		/// Indicates if one geometry touches another geometry.
		/// See OperatorTouches.
		/// </remarks>
		/// <param name="geometry1">The geometry to touch.</param>
		/// <param name="geometry2">The geometry to be touched.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>TRUE if geometry1 touches geometry2.</returns>
		public static bool Touches(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorTouches op = (com.epl.geometry.OperatorTouches)factory.GetOperator(com.epl.geometry.Operator.Type.Touches);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if one geometry overlaps another geometry.</summary>
		/// <remarks>
		/// Indicates if one geometry overlaps another geometry.
		/// See OperatorOverlaps.
		/// </remarks>
		/// <param name="geometry1">The geometry to overlap.</param>
		/// <param name="geometry2">The geometry to be overlapped.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>TRUE if geometry1 overlaps geometry2.</returns>
		public static bool Overlaps(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)factory.GetOperator(com.epl.geometry.Operator.Type.Overlaps);
			bool result = op.Execute(geometry1, geometry2, spatialReference, null);
			return result;
		}

		/// <summary>Indicates if the given relation holds for the two geometries.</summary>
		/// <remarks>
		/// Indicates if the given relation holds for the two geometries.
		/// See OperatorRelate.
		/// </remarks>
		/// <param name="geometry1">The first geometry for the relation.</param>
		/// <param name="geometry2">The second geometry for the relation.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <param name="relation">The DE-9IM relation.</param>
		/// <returns>TRUE if the given relation holds between geometry1 and geometry2.</returns>
		public static bool Relate(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference, string relation)
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)factory.GetOperator(com.epl.geometry.Operator.Type.Relate);
			bool result = op.Execute(geometry1, geometry2, spatialReference, relation, null);
			return result;
		}

		/// <summary>Calculates the 2D planar distance between two geometries.</summary>
		/// <remarks>
		/// Calculates the 2D planar distance between two geometries.
		/// See OperatorDistance.
		/// </remarks>
		/// <param name="geometry1">Geometry.</param>
		/// <param name="geometry2">Geometry.</param>
		/// <param name="spatialReference">
		/// The spatial reference of the geometries. This parameter is not
		/// used and can be null.
		/// </param>
		/// <returns>The distance between the two geometries.</returns>
		public static double Distance(com.epl.geometry.Geometry geometry1, com.epl.geometry.Geometry geometry2, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorDistance op = (com.epl.geometry.OperatorDistance)factory.GetOperator(com.epl.geometry.Operator.Type.Distance);
			double result = op.Execute(geometry1, geometry2, null);
			return result;
		}

		/// <summary>Calculates the clipped geometry from a target geometry using an envelope.</summary>
		/// <remarks>
		/// Calculates the clipped geometry from a target geometry using an envelope.
		/// See OperatorClip.
		/// </remarks>
		/// <param name="geometry">The geometry to be clipped.</param>
		/// <param name="envelope">The envelope used to clip.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>The geometry created by clipping.</returns>
		public static com.epl.geometry.Geometry Clip(com.epl.geometry.Geometry geometry, com.epl.geometry.Envelope envelope, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorClip op = (com.epl.geometry.OperatorClip)factory.GetOperator(com.epl.geometry.Operator.Type.Clip);
			com.epl.geometry.Geometry result = op.Execute(geometry, com.epl.geometry.Envelope2D.Construct(envelope.GetXMin(), envelope.GetYMin(), envelope.GetXMax(), envelope.GetYMax()), spatialReference, null);
			return result;
		}

		/// <summary>Calculates the cut geometry from a target geometry using a polyline.</summary>
		/// <remarks>
		/// Calculates the cut geometry from a target geometry using a polyline. For
		/// Polylines, all left cuts will be grouped together in the first Geometry,
		/// Right cuts and coincident cuts are grouped in the second Geometry, and
		/// each undefined cut, along with any uncut parts, are output as separate
		/// Polylines. For Polygons, all left cuts are grouped in the first Polygon,
		/// all right cuts are in the second Polygon, and each undefined cut, along
		/// with any left-over parts after cutting, are output as a separate Polygon.
		/// If there were no cuts then the array will be empty. An undefined cut will
		/// only be produced if a left cut or right cut was produced, and there was a
		/// part left over after cutting or a cut is bounded to the left and right of
		/// the cutter.
		/// See OperatorCut.
		/// </remarks>
		/// <param name="cuttee">The geometry to be cut.</param>
		/// <param name="cutter">The polyline to cut the geometry.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <returns>An array of geometries created from cutting.</returns>
		public static com.epl.geometry.Geometry[] Cut(com.epl.geometry.Geometry cuttee, com.epl.geometry.Polyline cutter, com.epl.geometry.SpatialReference spatialReference)
		{
			if (cuttee == null || cutter == null)
			{
				return null;
			}
			com.epl.geometry.OperatorCut op = (com.epl.geometry.OperatorCut)factory.GetOperator(com.epl.geometry.Operator.Type.Cut);
			com.epl.geometry.GeometryCursor cursor = op.Execute(true, cuttee, cutter, spatialReference, null);
			System.Collections.Generic.List<com.epl.geometry.Geometry> cutsList = new System.Collections.Generic.List<com.epl.geometry.Geometry>();
			com.epl.geometry.Geometry geometry;
			while ((geometry = cursor.Next()) != null)
			{
				if (!geometry.IsEmpty())
				{
					cutsList.Add(geometry);
				}
			}
			return cutsList.ToArray();
		}

		/// <summary>
		/// Calculates a buffer polygon for each geometry at each of the
		/// corresponding specified distances.
		/// </summary>
		/// <remarks>
		/// Calculates a buffer polygon for each geometry at each of the
		/// corresponding specified distances.  It is assumed that all geometries have
		/// the same spatial reference. There is an option to union the
		/// returned geometries.
		/// See OperatorBuffer.
		/// </remarks>
		/// <param name="geometries">An array of geometries to be buffered.</param>
		/// <param name="spatialReference">The spatial reference of the geometries.</param>
		/// <param name="distances">The corresponding distances for the input geometries to be buffered.</param>
		/// <param name="toUnionResults">TRUE if all geometries buffered at a given distance are to be unioned into a single polygon.</param>
		/// <returns>The buffer of the geometries.</returns>
		public static com.epl.geometry.Polygon[] Buffer(com.epl.geometry.Geometry[] geometries, com.epl.geometry.SpatialReference spatialReference, double[] distances, bool toUnionResults)
		{
			// initially assume distances are in unit of spatial reference
			double[] bufferDistances = distances;
			com.epl.geometry.OperatorBuffer op = (com.epl.geometry.OperatorBuffer)factory.GetOperator(com.epl.geometry.Operator.Type.Buffer);
			if (toUnionResults)
			{
				com.epl.geometry.SimpleGeometryCursor inputGeometriesCursor = new com.epl.geometry.SimpleGeometryCursor(geometries);
				com.epl.geometry.GeometryCursor result = op.Execute(inputGeometriesCursor, spatialReference, bufferDistances, toUnionResults, null);
				System.Collections.Generic.List<com.epl.geometry.Polygon> resultGeoms = new System.Collections.Generic.List<com.epl.geometry.Polygon>();
				com.epl.geometry.Geometry g;
				while ((g = result.Next()) != null)
				{
					resultGeoms.Add((com.epl.geometry.Polygon)g);
				}
				com.epl.geometry.Polygon[] buffers = resultGeoms.ToArray();
				return buffers;
			}
			else
			{
				com.epl.geometry.Polygon[] buffers = new com.epl.geometry.Polygon[geometries.Length];
				for (int i = 0; i < geometries.Length; i++)
				{
					buffers[i] = (com.epl.geometry.Polygon)op.Execute(geometries[i], spatialReference, bufferDistances[i], null);
				}
				return buffers;
			}
		}

		/// <summary>
		/// Calculates a buffer polygon of the geometry as specified by the
		/// distance input.
		/// </summary>
		/// <remarks>
		/// Calculates a buffer polygon of the geometry as specified by the
		/// distance input. The buffer is implemented in the xy-plane.
		/// See OperatorBuffer
		/// </remarks>
		/// <param name="geometry">Geometry to be buffered.</param>
		/// <param name="spatialReference">The spatial reference of the geometry.</param>
		/// <param name="distance">The specified distance for buffer. Same units as the spatial reference.</param>
		/// <returns>The buffer polygon at the specified distances.</returns>
		public static com.epl.geometry.Polygon Buffer(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, double distance)
		{
			double bufferDistance = distance;
			com.epl.geometry.OperatorBuffer op = (com.epl.geometry.OperatorBuffer)factory.GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.Geometry result = op.Execute(geometry, spatialReference, bufferDistance, null);
			return (com.epl.geometry.Polygon)result;
		}

		/// <summary>Calculates the convex hull geometry.</summary>
		/// <remarks>
		/// Calculates the convex hull geometry.
		/// See OperatorConvexHull.
		/// </remarks>
		/// <param name="geometry">The input geometry.</param>
		/// <returns>
		/// Returns the convex hull.
		/// For a Point - returns the same point. For an Envelope -
		/// returns the same envelope. For a MultiPoint - If the point
		/// count is one, returns the same multipoint. If the point count
		/// is two, returns a polyline of the points. Otherwise computes
		/// and returns the convex hull polygon. For a Segment - returns a
		/// polyline consisting of the segment. For a Polyline - If
		/// consists of only one segment, returns the same polyline.
		/// Otherwise computes and returns the convex hull polygon. For a
		/// Polygon - If more than one path, or if the path isn't already
		/// convex, computes and returns the convex hull polygon.
		/// Otherwise returns the same polygon.
		/// </returns>
		public static com.epl.geometry.Geometry ConvexHull(com.epl.geometry.Geometry geometry)
		{
			com.epl.geometry.OperatorConvexHull op = (com.epl.geometry.OperatorConvexHull)factory.GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			return op.Execute(geometry, null);
		}

		/// <summary>Calculates the convex hull.</summary>
		/// <remarks>
		/// Calculates the convex hull.
		/// See OperatorConvexHull
		/// </remarks>
		/// <param name="geometries">The input geometry array.</param>
		/// <param name="b_merge">
		/// Put true if you want the convex hull of all the geometries in
		/// the array combined. Put false if you want the convex hull of
		/// each geometry in the array individually.
		/// </param>
		/// <returns>
		/// Returns an array of convex hulls. If b_merge is true, the result
		/// will be a one element array consisting of the merged convex hull.
		/// </returns>
		public static com.epl.geometry.Geometry[] ConvexHull(com.epl.geometry.Geometry[] geometries, bool b_merge)
		{
			com.epl.geometry.OperatorConvexHull op = (com.epl.geometry.OperatorConvexHull)factory.GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.SimpleGeometryCursor simple_cursor = new com.epl.geometry.SimpleGeometryCursor(geometries);
			com.epl.geometry.GeometryCursor cursor = op.Execute(simple_cursor, b_merge, null);
			System.Collections.Generic.List<com.epl.geometry.Geometry> resultGeoms = new System.Collections.Generic.List<com.epl.geometry.Geometry>();
			com.epl.geometry.Geometry g;
			while ((g = cursor.Next()) != null)
			{
				resultGeoms.Add(g);
			}
			com.epl.geometry.Geometry[] output = new com.epl.geometry.Geometry[resultGeoms.Count];
			for (int i = 0; i < resultGeoms.Count; i++)
			{
				output[i] = resultGeoms[i];
			}
			return output;
		}

		/// <summary>
		/// Finds the coordinate of the geometry which is closest to the specified
		/// point.
		/// </summary>
		/// <remarks>
		/// Finds the coordinate of the geometry which is closest to the specified
		/// point.
		/// See OperatorProximity2D.
		/// </remarks>
		/// <param name="inputPoint">The point to find the nearest coordinate in the geometry for.</param>
		/// <param name="geometry">The geometry to consider.</param>
		/// <returns>Proximity2DResult containing the nearest coordinate.</returns>
		public static com.epl.geometry.Proximity2DResult GetNearestCoordinate(com.epl.geometry.Geometry geometry, com.epl.geometry.Point inputPoint, bool bTestPolygonInterior)
		{
			com.epl.geometry.OperatorProximity2D proximity = (com.epl.geometry.OperatorProximity2D)factory.GetOperator(com.epl.geometry.Operator.Type.Proximity2D);
			com.epl.geometry.Proximity2DResult result = proximity.GetNearestCoordinate(geometry, inputPoint, bTestPolygonInterior);
			return result;
		}

		/// <summary>
		/// Finds nearest vertex on the geometry which is closed to the specified
		/// point.
		/// </summary>
		/// <remarks>
		/// Finds nearest vertex on the geometry which is closed to the specified
		/// point.
		/// See OperatorProximity2D.
		/// </remarks>
		/// <param name="inputPoint">The point to find the nearest vertex of the geometry for.</param>
		/// <param name="geometry">The geometry to consider.</param>
		/// <returns>Proximity2DResult containing the nearest vertex.</returns>
		public static com.epl.geometry.Proximity2DResult GetNearestVertex(com.epl.geometry.Geometry geometry, com.epl.geometry.Point inputPoint)
		{
			com.epl.geometry.OperatorProximity2D proximity = (com.epl.geometry.OperatorProximity2D)factory.GetOperator(com.epl.geometry.Operator.Type.Proximity2D);
			com.epl.geometry.Proximity2DResult result = proximity.GetNearestVertex(geometry, inputPoint);
			return result;
		}

		/// <summary>
		/// Finds all vertices in the given distance from the specified point, sorted
		/// from the closest to the furthest.
		/// </summary>
		/// <remarks>
		/// Finds all vertices in the given distance from the specified point, sorted
		/// from the closest to the furthest.
		/// See OperatorProximity2D.
		/// </remarks>
		/// <param name="inputPoint">The point to start from.</param>
		/// <param name="geometry">The geometry to consider.</param>
		/// <param name="searchRadius">The search radius.</param>
		/// <param name="maxVertexCountToReturn">The maximum number number of vertices to return.</param>
		/// <returns>Proximity2DResult containing the array of nearest vertices.</returns>
		public static com.epl.geometry.Proximity2DResult[] GetNearestVertices(com.epl.geometry.Geometry geometry, com.epl.geometry.Point inputPoint, double searchRadius, int maxVertexCountToReturn)
		{
			com.epl.geometry.OperatorProximity2D proximity = (com.epl.geometry.OperatorProximity2D)factory.GetOperator(com.epl.geometry.Operator.Type.Proximity2D);
			com.epl.geometry.Proximity2DResult[] results = proximity.GetNearestVertices(geometry, inputPoint, searchRadius, maxVertexCountToReturn);
			return results;
		}

		/// <summary>Performs the simplify operation on the geometry.</summary>
		/// <remarks>
		/// Performs the simplify operation on the geometry.
		/// See OperatorSimplify and See OperatorSimplifyOGC.
		/// </remarks>
		/// <param name="geometry">The geometry to be simplified.</param>
		/// <param name="spatialReference">The spatial reference of the geometry to be simplified.</param>
		/// <returns>The simplified geometry.</returns>
		public static com.epl.geometry.Geometry Simplify(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorSimplify op = (com.epl.geometry.OperatorSimplify)factory.GetOperator(com.epl.geometry.Operator.Type.Simplify);
			com.epl.geometry.Geometry result = op.Execute(geometry, spatialReference, false, null);
			return result;
		}

		/// <summary>Checks if the Geometry is simple.</summary>
		/// <remarks>
		/// Checks if the Geometry is simple.
		/// See OperatorSimplify.
		/// </remarks>
		/// <param name="geometry">The geometry to be checked.</param>
		/// <param name="spatialReference">The spatial reference of the geometry.</param>
		/// <returns>TRUE if the geometry is simple.</returns>
		internal static bool IsSimple(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference)
		{
			com.epl.geometry.OperatorSimplify op = (com.epl.geometry.OperatorSimplify)factory.GetOperator(com.epl.geometry.Operator.Type.Simplify);
			bool result = op.IsSimpleAsFeature(geometry, spatialReference, null);
			return result;
		}

		/// <summary>
		/// A geodesic distance is the shortest distance between any two points on the earth's surface when the earth's
		/// surface is approximated by a spheroid.
		/// </summary>
		/// <remarks>
		/// A geodesic distance is the shortest distance between any two points on the earth's surface when the earth's
		/// surface is approximated by a spheroid. The function returns the shortest distance between two points on the
		/// WGS84 spheroid.
		/// </remarks>
		/// <param name="ptFrom">The "from" point: long, lat in degrees.</param>
		/// <param name="ptTo">The "to" point: long, lat in degrees.</param>
		/// <returns>The geodesic distance between two points in meters.</returns>
		public static double GeodesicDistanceOnWGS84(com.epl.geometry.Point ptFrom, com.epl.geometry.Point ptTo)
		{
			return com.epl.geometry.SpatialReferenceImpl.GeodesicDistanceOnWGS84Impl(ptFrom, ptTo);
		}
	}
}
