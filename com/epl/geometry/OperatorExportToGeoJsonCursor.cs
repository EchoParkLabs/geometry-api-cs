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
Environmental Systems Research Institute, Inc.
Attn: Contracts Dept
380 New York Street
Redlands, California, USA 92373

email: contracts@esri.com
*/
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

email: info@echoparklabs.io
*/


namespace com.epl.geometry
{
	internal class OperatorExportToGeoJsonCursor : com.epl.geometry.JsonCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal com.epl.geometry.SpatialReference m_spatialReference;

		internal int m_index;

		internal int m_export_flags;

		public OperatorExportToGeoJsonCursor(int export_flags, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor)
		{
			m_index = -1;
			if (geometryCursor == null)
			{
				throw new System.ArgumentException();
			}
			m_export_flags = export_flags;
			m_spatialReference = spatialReference;
			m_inputGeometryCursor = geometryCursor;
		}

		public override int GetID()
		{
			return m_index;
		}

		public override string Next()
		{
			com.epl.geometry.Geometry geometry;
			if ((geometry = m_inputGeometryCursor.Next()) != null)
			{
				m_index = m_inputGeometryCursor.GetGeometryID();
				return ExportToGeoJson(m_export_flags, geometry, m_spatialReference);
			}
			return null;
		}

		// Mirrors wkt
		internal static string ExportToGeoJson(int export_flags, com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatial_reference)
		{
			if (geometry == null)
			{
				throw new System.ArgumentException(string.Empty);
			}
			com.epl.geometry.JsonWriter json_writer = new com.epl.geometry.JsonStringWriter();
			json_writer.StartObject();
			ExportGeometryToGeoJson_(export_flags, geometry, json_writer);
			if ((export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportSkipCRS) == 0)
			{
				json_writer.AddFieldName("crs");
				ExportSpatialReference(export_flags, spatial_reference, json_writer);
			}
			json_writer.EndObject();
			return (string)json_writer.GetJson();
		}

		internal static string ExportSpatialReference(int export_flags, com.epl.geometry.SpatialReference spatial_reference)
		{
			if (spatial_reference == null || (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportSkipCRS) != 0)
			{
				throw new System.ArgumentException(string.Empty);
			}
			com.epl.geometry.JsonWriter json_writer = new com.epl.geometry.JsonStringWriter();
			ExportSpatialReference(export_flags, spatial_reference, json_writer);
			return (string)json_writer.GetJson();
		}

		private static void ExportGeometryToGeoJson_(int export_flags, com.epl.geometry.Geometry geometry, com.epl.geometry.JsonWriter json_writer)
		{
			int type = geometry.GetType().Value();
			switch (type)
			{
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					ExportPolygonToGeoJson_(export_flags, (com.epl.geometry.Polygon)geometry, json_writer);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					ExportPolylineToGeoJson_(export_flags, (com.epl.geometry.Polyline)geometry, json_writer);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					ExportMultiPointToGeoJson_(export_flags, (com.epl.geometry.MultiPoint)geometry, json_writer);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					ExportPointToGeoJson_(export_flags, (com.epl.geometry.Point)geometry, json_writer);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					ExportEnvelopeToGeoJson_(export_flags, (com.epl.geometry.Envelope)geometry, json_writer);
					return;
				}

				default:
				{
					throw new System.Exception("not implemented for this geometry type");
				}
			}
		}

		private static void ExportSpatialReference(int export_flags, com.epl.geometry.SpatialReference spatial_reference, com.epl.geometry.JsonWriter json_writer)
		{
			if (spatial_reference != null)
			{
				int wkid = spatial_reference.GetLatestID();
				if (wkid <= 0)
				{
					throw new com.epl.geometry.GeometryException("invalid call");
				}
				json_writer.StartObject();
				json_writer.AddFieldName("type");
				json_writer.AddValueString("name");
				json_writer.AddFieldName("properties");
				json_writer.StartObject();
				json_writer.AddFieldName("name");
				string authority = ((com.epl.geometry.SpatialReferenceImpl)spatial_reference).GetAuthority();
				authority = authority.ToUpper();
				System.Text.StringBuilder crs_identifier = new System.Text.StringBuilder(authority);
				crs_identifier.Append(':');
				crs_identifier.Append(wkid);
				json_writer.AddValueString(crs_identifier.ToString());
				json_writer.EndObject();
				json_writer.EndObject();
			}
			else
			{
				json_writer.AddValueNull();
			}
		}

		// Mirrors wkt
		private static void ExportPolygonToGeoJson_(int export_flags, com.epl.geometry.Polygon polygon, com.epl.geometry.JsonWriter json_writer)
		{
			com.epl.geometry.MultiPathImpl polygon_impl = (com.epl.geometry.MultiPathImpl)(polygon._getImpl());
			if ((export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportFailIfNotSimple) != 0)
			{
				int simple = polygon_impl.GetIsSimple(0.0);
				if (simple != com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong)
				{
					throw new com.epl.geometry.GeometryException("corrupted geometry");
				}
			}
			int point_count = polygon.GetPointCount();
			int polygon_count = polygon_impl.GetOGCPolygonCount();
			if (point_count > 0 && polygon_count == 0)
			{
				throw new com.epl.geometry.GeometryException("corrupted geometry");
			}
			int precision = 17 - (31 & (export_flags >> 13));
			bool bFixedPoint = (com.epl.geometry.GeoJsonExportFlags.geoJsonExportPrecisionFixedPoint & export_flags) != 0;
			bool b_export_zs = polygon_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripZs) == 0;
			bool b_export_ms = polygon_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripMs) == 0;
			if (!b_export_zs && b_export_ms)
			{
				throw new System.ArgumentException("invalid argument");
			}
			int path_count = 0;
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt8 path_flags = null;
			com.epl.geometry.AttributeStreamOfInt32 paths = null;
			if (point_count > 0)
			{
				position = (com.epl.geometry.AttributeStreamOfDbl)polygon_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				path_flags = polygon_impl.GetPathFlagsStreamRef();
				paths = polygon_impl.GetPathStreamRef();
				path_count = polygon_impl.GetPathCount();
				if (b_export_zs)
				{
					if (polygon_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)polygon_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
					}
				}
				if (b_export_ms)
				{
					if (polygon_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)polygon_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
					}
				}
			}
			if ((export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportPreferMultiGeometry) == 0 && polygon_count <= 1)
			{
				PolygonTaggedText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, paths, path_count, json_writer);
			}
			else
			{
				MultiPolygonTaggedText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, polygon_count, path_count, json_writer);
			}
		}

		// Mirrors wkt
		private static void ExportPolylineToGeoJson_(int export_flags, com.epl.geometry.Polyline polyline, com.epl.geometry.JsonWriter json_writer)
		{
			com.epl.geometry.MultiPathImpl polyline_impl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
			int point_count = polyline_impl.GetPointCount();
			int path_count = polyline_impl.GetPathCount();
			if (point_count > 0 && path_count == 0)
			{
				throw new com.epl.geometry.GeometryException("corrupted geometry");
			}
			int precision = 17 - (31 & (export_flags >> 13));
			bool bFixedPoint = (com.epl.geometry.GeoJsonExportFlags.geoJsonExportPrecisionFixedPoint & export_flags) != 0;
			bool b_export_zs = polyline_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripZs) == 0;
			bool b_export_ms = polyline_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripMs) == 0;
			if (!b_export_zs && b_export_ms)
			{
				throw new System.ArgumentException("invalid argument");
			}
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt8 path_flags = null;
			com.epl.geometry.AttributeStreamOfInt32 paths = null;
			if (point_count > 0)
			{
				position = (com.epl.geometry.AttributeStreamOfDbl)polyline_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				path_flags = polyline_impl.GetPathFlagsStreamRef();
				paths = polyline_impl.GetPathStreamRef();
				if (b_export_zs)
				{
					if (polyline_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)polyline_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
					}
				}
				if (b_export_ms)
				{
					if (polyline_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)polyline_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
					}
				}
			}
			if ((export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportPreferMultiGeometry) == 0 && path_count <= 1)
			{
				LineStringTaggedText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, json_writer);
			}
			else
			{
				MultiLineStringTaggedText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, path_count, json_writer);
			}
		}

		// Mirrors wkt
		private static void ExportMultiPointToGeoJson_(int export_flags, com.epl.geometry.MultiPoint multipoint, com.epl.geometry.JsonWriter json_writer)
		{
			com.epl.geometry.MultiPointImpl multipoint_impl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
			int point_count = multipoint_impl.GetPointCount();
			int precision = 17 - (31 & (export_flags >> 13));
			bool bFixedPoint = (com.epl.geometry.GeoJsonExportFlags.geoJsonExportPrecisionFixedPoint & export_flags) != 0;
			bool b_export_zs = multipoint_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripZs) == 0;
			bool b_export_ms = multipoint_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripMs) == 0;
			if (!b_export_zs && b_export_ms)
			{
				throw new System.ArgumentException("invalid argument");
			}
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			if (point_count > 0)
			{
				position = (com.epl.geometry.AttributeStreamOfDbl)multipoint_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				if (b_export_zs)
				{
					if (multipoint_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)multipoint_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
					}
				}
				if (b_export_ms)
				{
					if (multipoint_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)multipoint_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
					}
				}
			}
			MultiPointTaggedText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, point_count, json_writer);
		}

		// Mirrors wkt
		private static void ExportPointToGeoJson_(int export_flags, com.epl.geometry.Point point, com.epl.geometry.JsonWriter json_writer)
		{
			int precision = 17 - (31 & (export_flags >> 13));
			bool bFixedPoint = (com.epl.geometry.GeoJsonExportFlags.geoJsonExportPrecisionFixedPoint & export_flags) != 0;
			bool b_export_zs = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripZs) == 0;
			bool b_export_ms = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripMs) == 0;
			if (!b_export_zs && b_export_ms)
			{
				throw new System.ArgumentException("invalid argument");
			}
			double x = com.epl.geometry.NumberUtils.NaN();
			double y = com.epl.geometry.NumberUtils.NaN();
			double z = com.epl.geometry.NumberUtils.NaN();
			double m = com.epl.geometry.NumberUtils.NaN();
			if (!point.IsEmpty())
			{
				x = point.GetX();
				y = point.GetY();
				if (b_export_zs)
				{
					z = point.GetZ();
				}
				if (b_export_ms)
				{
					m = point.GetM();
				}
			}
			if ((export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportPreferMultiGeometry) == 0)
			{
				PointTaggedText_(precision, bFixedPoint, b_export_zs, b_export_ms, x, y, z, m, json_writer);
			}
			else
			{
				MultiPointTaggedTextFromPoint_(precision, bFixedPoint, b_export_zs, b_export_ms, x, y, z, m, json_writer);
			}
		}

		// Mirrors wkt
		private static void ExportEnvelopeToGeoJson_(int export_flags, com.epl.geometry.Envelope envelope, com.epl.geometry.JsonWriter json_writer)
		{
			int precision = 17 - (31 & (export_flags >> 13));
			bool bFixedPoint = (com.epl.geometry.GeoJsonExportFlags.geoJsonExportPrecisionFixedPoint & export_flags) != 0;
			bool b_export_zs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripZs) == 0;
			bool b_export_ms = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportStripMs) == 0;
			if (!b_export_zs && b_export_ms)
			{
				throw new System.ArgumentException("invalid argument");
			}
			double xmin = com.epl.geometry.NumberUtils.NaN();
			double ymin = com.epl.geometry.NumberUtils.NaN();
			double xmax = com.epl.geometry.NumberUtils.NaN();
			double ymax = com.epl.geometry.NumberUtils.NaN();
			double zmin = com.epl.geometry.NumberUtils.NaN();
			double zmax = com.epl.geometry.NumberUtils.NaN();
			double mmin = com.epl.geometry.NumberUtils.NaN();
			double mmax = com.epl.geometry.NumberUtils.NaN();
			if (!envelope.IsEmpty())
			{
				xmin = envelope.GetXMin();
				ymin = envelope.GetYMin();
				xmax = envelope.GetXMax();
				ymax = envelope.GetYMax();
				com.epl.geometry.Envelope1D interval;
				if (b_export_zs)
				{
					interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
					zmin = interval.vmin;
					zmax = interval.vmax;
				}
				if (b_export_ms)
				{
					interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
					mmin = interval.vmin;
					mmax = interval.vmax;
				}
			}
			if ((export_flags & com.epl.geometry.GeoJsonExportFlags.geoJsonExportPreferMultiGeometry) == 0)
			{
				PolygonTaggedTextFromEnvelope_(precision, bFixedPoint, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, json_writer);
			}
			else
			{
				MultiPolygonTaggedTextFromEnvelope_(precision, bFixedPoint, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, json_writer);
			}
		}

		// Mirrors wkt
		private static void MultiPolygonTaggedText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8
			 path_flags, com.epl.geometry.AttributeStreamOfInt32 paths, int polygon_count, int path_count, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("MultiPolygon");
			json_writer.AddFieldName("coordinates");
			if (position == null)
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			json_writer.StartArray();
			MultiPolygonText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, polygon_count, path_count, json_writer);
			json_writer.EndArray();
		}

		// Mirrors wkt
		private static void MultiPolygonTaggedTextFromEnvelope_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("MultiPolygon");
			json_writer.AddFieldName("coordinates");
			if (com.epl.geometry.NumberUtils.IsNaN(xmin))
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			json_writer.StartArray();
			WriteEnvelopeAsGeoJsonPolygon_(precision, bFixedPoint, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, json_writer);
			json_writer.EndArray();
		}

		// Mirrors wkt
		private static void MultiLineStringTaggedText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8
			 path_flags, com.epl.geometry.AttributeStreamOfInt32 paths, int path_count, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("MultiLineString");
			json_writer.AddFieldName("coordinates");
			if (position == null)
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			json_writer.StartArray();
			MultiLineStringText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, path_count, json_writer);
			json_writer.EndArray();
		}

		// Mirrors wkt
		private static void MultiPointTaggedText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int point_count, com.epl.geometry.JsonWriter
			 json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("MultiPoint");
			json_writer.AddFieldName("coordinates");
			if (position == null)
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			LineStringText_(false, false, precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, 0, point_count, json_writer);
		}

		// Mirrors wkt
		private static void MultiPointTaggedTextFromPoint_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("MultiPoint");
			json_writer.AddFieldName("coordinates");
			if (com.epl.geometry.NumberUtils.IsNaN(x))
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			json_writer.StartArray();
			PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, x, y, z, m, json_writer);
			json_writer.EndArray();
		}

		// Mirrors wkt
		private static void PolygonTaggedText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32
			 paths, int path_count, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("Polygon");
			json_writer.AddFieldName("coordinates");
			if (position == null)
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			PolygonText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, paths, 0, path_count, json_writer);
		}

		// Mirrors wkt
		private static void PolygonTaggedTextFromEnvelope_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("Polygon");
			json_writer.AddFieldName("coordinates");
			if (com.epl.geometry.NumberUtils.IsNaN(xmin))
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			WriteEnvelopeAsGeoJsonPolygon_(precision, bFixedPoint, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, json_writer);
		}

		// Mirrors wkt
		private static void LineStringTaggedText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8
			 path_flags, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("LineString");
			json_writer.AddFieldName("coordinates");
			if (position == null)
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			bool b_closed = ((path_flags.Read(0) & com.epl.geometry.PathFlags.enumClosed) != 0);
			LineStringText_(false, b_closed, precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, 0, paths.Read(1), json_writer);
		}

		// Mirrors wkt
		private static void PointTaggedText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.AddFieldName("type");
			json_writer.AddValueString("Point");
			json_writer.AddFieldName("coordinates");
			if (com.epl.geometry.NumberUtils.IsNaN(x))
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, x, y, z, m, json_writer);
		}

		// Mirrors wkt
		private static void MultiPolygonText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8
			 path_flags, com.epl.geometry.AttributeStreamOfInt32 paths, int polygon_count, int path_count, com.epl.geometry.JsonWriter json_writer)
		{
			int polygon_start = 0;
			int polygon_end = 1;
			while (polygon_end < path_count && ((int)path_flags.Read(polygon_end) & com.epl.geometry.PathFlags.enumOGCStartPolygon) == 0)
			{
				polygon_end++;
			}
			PolygonText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, paths, polygon_start, polygon_end, json_writer);
			for (int ipolygon = 1; ipolygon < polygon_count; ipolygon++)
			{
				polygon_start = polygon_end;
				polygon_end++;
				while (polygon_end < path_count && ((int)path_flags.Read(polygon_end) & com.epl.geometry.PathFlags.enumOGCStartPolygon) == 0)
				{
					polygon_end++;
				}
				PolygonText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, paths, polygon_start, polygon_end, json_writer);
			}
		}

		// Mirrors wkt
		private static void MultiLineStringText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8
			 path_flags, com.epl.geometry.AttributeStreamOfInt32 paths, int path_count, com.epl.geometry.JsonWriter json_writer)
		{
			bool b_closed = ((path_flags.Read(0) & com.epl.geometry.PathFlags.enumClosed) != 0);
			LineStringText_(false, b_closed, precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, 0, paths.Read(1), json_writer);
			for (int path = 1; path < path_count; path++)
			{
				b_closed = ((path_flags.Read(path) & com.epl.geometry.PathFlags.enumClosed) != 0);
				int istart = paths.Read(path);
				int iend = paths.Read(path + 1);
				LineStringText_(false, b_closed, precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, istart, iend, json_writer);
			}
		}

		// Mirrors wkt
		private static void PolygonText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths
			, int polygon_start, int polygon_end, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.StartArray();
			int istart = paths.Read(polygon_start);
			int iend = paths.Read(polygon_start + 1);
			LineStringText_(true, true, precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, istart, iend, json_writer);
			for (int path = polygon_start + 1; path < polygon_end; path++)
			{
				istart = paths.Read(path);
				iend = paths.Read(path + 1);
				LineStringText_(true, true, precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, istart, iend, json_writer);
			}
			json_writer.EndArray();
		}

		// Mirrors wkt
		private static void LineStringText_(bool bRing, bool b_closed, int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int istart, int iend
			, com.epl.geometry.JsonWriter json_writer)
		{
			if (istart == iend)
			{
				json_writer.StartArray();
				json_writer.EndArray();
				return;
			}
			json_writer.StartArray();
			if (bRing)
			{
				PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, istart, json_writer);
				for (int point = iend - 1; point >= istart + 1; point--)
				{
					PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, point, json_writer);
				}
				PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, istart, json_writer);
			}
			else
			{
				for (int point = istart; point < iend - 1; point++)
				{
					PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, point, json_writer);
				}
				PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, iend - 1, json_writer);
				if (b_closed)
				{
					PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, zs, ms, position, istart, json_writer);
				}
			}
			json_writer.EndArray();
		}

		// Mirrors wkt
		private static int PointText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.StartArray();
			json_writer.AddValueDouble(x, precision, bFixedPoint);
			json_writer.AddValueDouble(y, precision, bFixedPoint);
			if (b_export_zs)
			{
				json_writer.AddValueDouble(z, precision, bFixedPoint);
			}
			if (b_export_ms)
			{
				json_writer.AddValueDouble(m, precision, bFixedPoint);
			}
			json_writer.EndArray();
			return 1;
		}

		// Mirrors wkt
		private static void PointText_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int point, com.epl.geometry.JsonWriter json_writer
			)
		{
			double x = position.ReadAsDbl(2 * point);
			double y = position.ReadAsDbl(2 * point + 1);
			double z = com.epl.geometry.NumberUtils.NaN();
			double m = com.epl.geometry.NumberUtils.NaN();
			if (b_export_zs)
			{
				z = (zs != null ? zs.ReadAsDbl(point) : com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z));
			}
			if (b_export_ms)
			{
				m = (ms != null ? ms.ReadAsDbl(point) : com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M));
			}
			PointText_(precision, bFixedPoint, b_export_zs, b_export_ms, x, y, z, m, json_writer);
		}

		// Mirrors wkt
		private static void WriteEnvelopeAsGeoJsonPolygon_(int precision, bool bFixedPoint, bool b_export_zs, bool b_export_ms, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax, com.epl.geometry.JsonWriter json_writer)
		{
			json_writer.StartArray();
			json_writer.StartArray();
			json_writer.StartArray();
			json_writer.AddValueDouble(xmin, precision, bFixedPoint);
			json_writer.AddValueDouble(ymin, precision, bFixedPoint);
			if (b_export_zs)
			{
				json_writer.AddValueDouble(zmin, precision, bFixedPoint);
			}
			if (b_export_ms)
			{
				json_writer.AddValueDouble(mmin, precision, bFixedPoint);
			}
			json_writer.EndArray();
			json_writer.StartArray();
			json_writer.AddValueDouble(xmax, precision, bFixedPoint);
			json_writer.AddValueDouble(ymin, precision, bFixedPoint);
			if (b_export_zs)
			{
				json_writer.AddValueDouble(zmax, precision, bFixedPoint);
			}
			if (b_export_ms)
			{
				json_writer.AddValueDouble(mmax, precision, bFixedPoint);
			}
			json_writer.EndArray();
			json_writer.StartArray();
			json_writer.AddValueDouble(xmax, precision, bFixedPoint);
			json_writer.AddValueDouble(ymax, precision, bFixedPoint);
			if (b_export_zs)
			{
				json_writer.AddValueDouble(zmin, precision, bFixedPoint);
			}
			if (b_export_ms)
			{
				json_writer.AddValueDouble(mmin, precision, bFixedPoint);
			}
			json_writer.EndArray();
			json_writer.StartArray();
			json_writer.AddValueDouble(xmin, precision, bFixedPoint);
			json_writer.AddValueDouble(ymax, precision, bFixedPoint);
			if (b_export_zs)
			{
				json_writer.AddValueDouble(zmax, precision, bFixedPoint);
			}
			if (b_export_ms)
			{
				json_writer.AddValueDouble(mmax, precision, bFixedPoint);
			}
			json_writer.EndArray();
			json_writer.StartArray();
			json_writer.AddValueDouble(xmin, precision, bFixedPoint);
			json_writer.AddValueDouble(ymin, precision, bFixedPoint);
			if (b_export_zs)
			{
				json_writer.AddValueDouble(zmin, precision, bFixedPoint);
			}
			if (b_export_ms)
			{
				json_writer.AddValueDouble(mmin, precision, bFixedPoint);
			}
			json_writer.EndArray();
			json_writer.EndArray();
			json_writer.EndArray();
		}
	}
}
