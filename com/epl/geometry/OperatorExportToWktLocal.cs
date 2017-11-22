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
	internal class OperatorExportToWktLocal : com.epl.geometry.OperatorExportToWkt
	{
		public override string Execute(int export_flags, com.epl.geometry.Geometry geometry, com.epl.geometry.ProgressTracker progress_tracker)
		{
			System.Text.StringBuilder @string = new System.Text.StringBuilder();
			ExportToWkt(export_flags, geometry, @string);
			return @string.ToString();
		}

		internal static void ExportToWkt(int export_flags, com.epl.geometry.Geometry geometry, System.Text.StringBuilder @string)
		{
			int type = geometry.GetType().Value();
			switch (type)
			{
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					if ((export_flags & com.epl.geometry.WktExportFlags.wktExportLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportMultiLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportPoint) != 0 || (export_flags & com.epl.geometry.WktExportFlags
						.wktExportMultiPoint) != 0)
					{
						throw new System.ArgumentException("Cannot export a Polygon as (Multi)LineString/(Multi)Point : " + export_flags);
					}
					ExportPolygonToWkt(export_flags, (com.epl.geometry.Polygon)geometry, @string);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					if ((export_flags & com.epl.geometry.WktExportFlags.wktExportPolygon) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportMultiPolygon) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportPoint) != 0 || (export_flags & com.epl.geometry.WktExportFlags.
						wktExportMultiPoint) != 0)
					{
						throw new System.ArgumentException("Cannot export a Polyline as (Multi)Polygon/(Multi)Point : " + export_flags);
					}
					ExportPolylineToWkt(export_flags, (com.epl.geometry.Polyline)geometry, @string);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					if ((export_flags & com.epl.geometry.WktExportFlags.wktExportLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportMultiLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportPolygon) != 0 || (export_flags & com.epl.geometry.WktExportFlags
						.wktExportMultiPolygon) != 0)
					{
						throw new System.ArgumentException("Cannot export a MultiPoint as (Multi)LineString/(Multi)Polygon: " + export_flags);
					}
					ExportMultiPointToWkt(export_flags, (com.epl.geometry.MultiPoint)geometry, @string);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					if ((export_flags & com.epl.geometry.WktExportFlags.wktExportLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportMultiLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportPolygon) != 0 || (export_flags & com.epl.geometry.WktExportFlags
						.wktExportMultiPolygon) != 0)
					{
						throw new System.ArgumentException("Cannot export a Point as (Multi)LineString/(Multi)Polygon: " + export_flags);
					}
					ExportPointToWkt(export_flags, (com.epl.geometry.Point)geometry, @string);
					return;
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					if ((export_flags & com.epl.geometry.WktExportFlags.wktExportLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportMultiLineString) != 0 || (export_flags & com.epl.geometry.WktExportFlags.wktExportPoint) != 0 || (export_flags & com.epl.geometry.WktExportFlags
						.wktExportMultiPoint) != 0)
					{
						throw new System.ArgumentException("Cannot export an Envelope as (Multi)LineString/(Multi)Point: " + export_flags);
					}
					ExportEnvelopeToWkt(export_flags, (com.epl.geometry.Envelope)geometry, @string);
					return;
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		internal static void ExportPolygonToWkt(int export_flags, com.epl.geometry.Polygon polygon, System.Text.StringBuilder @string)
		{
			com.epl.geometry.MultiPathImpl polygon_impl = (com.epl.geometry.MultiPathImpl)polygon._getImpl();
			if ((export_flags & com.epl.geometry.WktExportFlags.wktExportFailIfNotSimple) != 0)
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
			int precision = 17 - (7 & (export_flags >> 13));
			bool b_export_zs = polygon_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripZs) == 0;
			bool b_export_ms = polygon_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripMs) == 0;
			int path_count = 0;
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt8 path_flags = null;
			com.epl.geometry.AttributeStreamOfInt32 paths = null;
			if (point_count > 0)
			{
				position = (com.epl.geometry.AttributeStreamOfDbl)(polygon_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
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
			if ((export_flags & com.epl.geometry.WktExportFlags.wktExportPolygon) != 0)
			{
				if (polygon_count > 1)
				{
					throw new System.ArgumentException("Cannot export a Polygon with specified export flags: " + export_flags);
				}
				PolygonTaggedText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, path_count, @string);
			}
			else
			{
				MultiPolygonTaggedText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, polygon_count, path_count, @string);
			}
		}

		internal static void ExportPolylineToWkt(int export_flags, com.epl.geometry.Polyline polyline, System.Text.StringBuilder @string)
		{
			com.epl.geometry.MultiPathImpl polyline_impl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
			int point_count = polyline_impl.GetPointCount();
			int path_count = polyline_impl.GetPathCount();
			if (point_count > 0 && path_count == 0)
			{
				throw new com.epl.geometry.GeometryException("corrupted geometry");
			}
			int precision = 17 - (7 & (export_flags >> 13));
			bool b_export_zs = polyline_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripZs) == 0;
			bool b_export_ms = polyline_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripMs) == 0;
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
						zs = (com.epl.geometry.AttributeStreamOfDbl)(polyline_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z));
					}
				}
				if (b_export_ms)
				{
					if (polyline_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)(polyline_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M));
					}
				}
			}
			if ((export_flags & com.epl.geometry.WktExportFlags.wktExportLineString) != 0)
			{
				if (path_count > 1)
				{
					throw new System.ArgumentException("Cannot export a LineString with specified export flags: " + export_flags);
				}
				LineStringTaggedText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, @string);
			}
			else
			{
				MultiLineStringTaggedText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, path_count, @string);
			}
		}

		internal static void ExportMultiPointToWkt(int export_flags, com.epl.geometry.MultiPoint multipoint, System.Text.StringBuilder @string)
		{
			com.epl.geometry.MultiPointImpl multipoint_impl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
			int point_count = multipoint_impl.GetPointCount();
			int precision = 17 - (7 & (export_flags >> 13));
			bool b_export_zs = multipoint_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripZs) == 0;
			bool b_export_ms = multipoint_impl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripMs) == 0;
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			if (point_count > 0)
			{
				position = (com.epl.geometry.AttributeStreamOfDbl)(multipoint_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
				if (b_export_zs)
				{
					if (multipoint_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)(multipoint_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z));
					}
				}
				if (b_export_ms)
				{
					if (multipoint_impl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)(multipoint_impl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M));
					}
				}
			}
			if ((export_flags & com.epl.geometry.WktExportFlags.wktExportPoint) != 0)
			{
				if (point_count > 1)
				{
					throw new System.ArgumentException("Cannot export a Point with specified export flags: " + export_flags);
				}
				PointTaggedTextFromMultiPoint_(precision, b_export_zs, b_export_ms, zs, ms, position, @string);
			}
			else
			{
				MultiPointTaggedText_(precision, b_export_zs, b_export_ms, zs, ms, position, point_count, @string);
			}
		}

		internal static void ExportPointToWkt(int export_flags, com.epl.geometry.Point point, System.Text.StringBuilder @string)
		{
			int precision = 17 - (7 & (export_flags >> 13));
			bool b_export_zs = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripZs) == 0;
			bool b_export_ms = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripMs) == 0;
			double x = com.epl.geometry.NumberUtils.TheNaN;
			double y = com.epl.geometry.NumberUtils.TheNaN;
			double z = com.epl.geometry.NumberUtils.TheNaN;
			double m = com.epl.geometry.NumberUtils.TheNaN;
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
			if ((export_flags & com.epl.geometry.WktExportFlags.wktExportMultiPoint) != 0)
			{
				MultiPointTaggedTextFromPoint_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
			}
			else
			{
				PointTaggedText_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
			}
		}

		internal static void ExportEnvelopeToWkt(int export_flags, com.epl.geometry.Envelope envelope, System.Text.StringBuilder @string)
		{
			int precision = 17 - (7 & (export_flags >> 13));
			bool b_export_zs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripZs) == 0;
			bool b_export_ms = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (export_flags & com.epl.geometry.WktExportFlags.wktExportStripMs) == 0;
			double xmin = com.epl.geometry.NumberUtils.TheNaN;
			double ymin = com.epl.geometry.NumberUtils.TheNaN;
			double xmax = com.epl.geometry.NumberUtils.TheNaN;
			double ymax = com.epl.geometry.NumberUtils.TheNaN;
			double zmin = com.epl.geometry.NumberUtils.TheNaN;
			double zmax = com.epl.geometry.NumberUtils.TheNaN;
			double mmin = com.epl.geometry.NumberUtils.TheNaN;
			double mmax = com.epl.geometry.NumberUtils.TheNaN;
			com.epl.geometry.Envelope1D interval;
			if (!envelope.IsEmpty())
			{
				xmin = envelope.GetXMin();
				ymin = envelope.GetYMin();
				xmax = envelope.GetXMax();
				ymax = envelope.GetYMax();
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
			if ((export_flags & com.epl.geometry.WktExportFlags.wktExportMultiPolygon) != 0)
			{
				MultiPolygonTaggedTextFromEnvelope_(precision, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, @string);
			}
			else
			{
				PolygonTaggedTextFromEnvelope_(precision, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, @string);
			}
		}

		internal static void MultiPolygonTaggedText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags
			, com.epl.geometry.AttributeStreamOfInt32 paths, int polygon_count, int path_count, System.Text.StringBuilder @string)
		{
			@string.Append("MULTIPOLYGON ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (position == null)
			{
				@string.Append("EMPTY");
				return;
			}
			@string.Append('(');
			MultiPolygonText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, polygon_count, path_count, @string);
			@string.Append(')');
		}

		internal static void MultiPolygonTaggedTextFromEnvelope_(int precision, bool b_export_zs, bool b_export_ms, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax, System.Text.StringBuilder @string)
		{
			@string.Append("MULTIPOLYGON ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (com.epl.geometry.NumberUtils.IsNaN(xmin))
			{
				@string.Append("EMPTY");
				return;
			}
			@string.Append('(');
			WriteEnvelopeAsWktPolygon_(precision, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, @string);
			@string.Append(')');
		}

		internal static void MultiLineStringTaggedText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags
			, com.epl.geometry.AttributeStreamOfInt32 paths, int path_count, System.Text.StringBuilder @string)
		{
			@string.Append("MULTILINESTRING ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (position == null)
			{
				@string.Append("EMPTY");
				return;
			}
			@string.Append('(');
			MultiLineStringText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, path_count, @string);
			@string.Append(')');
		}

		internal static void MultiPointTaggedText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int point_count, System.Text.StringBuilder @string)
		{
			@string.Append("MULTIPOINT ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (position == null)
			{
				@string.Append("EMPTY");
				return;
			}
			@string.Append('(');
			MultiPointText_(precision, b_export_zs, b_export_ms, zs, ms, position, point_count, @string);
			@string.Append(')');
		}

		internal static void MultiPointTaggedTextFromPoint_(int precision, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, System.Text.StringBuilder @string)
		{
			@string.Append("MULTIPOINT ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (com.epl.geometry.NumberUtils.IsNaN(x))
			{
				@string.Append("EMPTY");
				return;
			}
			@string.Append('(');
			PointText_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
			@string.Append(')');
		}

		internal static void PolygonTaggedText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags, com.epl.geometry.AttributeStreamOfInt32
			 paths, int path_count, System.Text.StringBuilder @string)
		{
			@string.Append("POLYGON ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (position == null)
			{
				@string.Append("EMPTY");
				return;
			}
			PolygonText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, 0, path_count, @string);
		}

		internal static void PolygonTaggedTextFromEnvelope_(int precision, bool b_export_zs, bool b_export_ms, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax, System.Text.StringBuilder @string)
		{
			@string.Append("POLYGON ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (com.epl.geometry.NumberUtils.IsNaN(xmin))
			{
				@string.Append("EMPTY");
				return;
			}
			WriteEnvelopeAsWktPolygon_(precision, b_export_zs, b_export_ms, xmin, ymin, xmax, ymax, zmin, zmax, mmin, mmax, @string);
		}

		internal static void LineStringTaggedText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags, 
			com.epl.geometry.AttributeStreamOfInt32 paths, System.Text.StringBuilder @string)
		{
			@string.Append("LINESTRING ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (position == null)
			{
				@string.Append("EMPTY");
				return;
			}
			bool b_closed = ((path_flags.Read(0) & com.epl.geometry.PathFlags.enumClosed) != 0);
			LineStringText_(false, b_closed, precision, b_export_zs, b_export_ms, zs, ms, position, paths, 0, @string);
		}

		internal static void PointTaggedText_(int precision, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, System.Text.StringBuilder @string)
		{
			@string.Append("POINT ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (com.epl.geometry.NumberUtils.IsNaN(x))
			{
				@string.Append("EMPTY");
				return;
			}
			PointText_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
		}

		internal static void PointTaggedTextFromMultiPoint_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, System.Text.StringBuilder @string)
		{
			@string.Append("POINT ");
			if (b_export_zs && b_export_ms)
			{
				@string.Append("ZM ");
			}
			else
			{
				if (b_export_zs && !b_export_ms)
				{
					@string.Append("Z ");
				}
				else
				{
					if (!b_export_zs && b_export_ms)
					{
						@string.Append("M ");
					}
				}
			}
			if (position == null)
			{
				@string.Append("EMPTY");
				return;
			}
			PointText_(precision, b_export_zs, b_export_ms, zs, ms, position, 0, @string);
		}

		internal static void MultiPolygonText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags, com.epl.geometry.AttributeStreamOfInt32
			 paths, int polygon_count, int path_count, System.Text.StringBuilder @string)
		{
			int polygon_start = 0;
			int polygon_end = 1;
			while (polygon_end < path_count && (path_flags.Read(polygon_end) & com.epl.geometry.PathFlags.enumOGCStartPolygon) == 0)
			{
				polygon_end++;
			}
			PolygonText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, polygon_start, polygon_end, @string);
			for (int ipolygon = 1; ipolygon < polygon_count; ipolygon++)
			{
				polygon_start = polygon_end;
				polygon_end++;
				while (polygon_end < path_count && (path_flags.Read(polygon_end) & com.epl.geometry.PathFlags.enumOGCStartPolygon) == 0)
				{
					polygon_end++;
				}
				@string.Append(", ");
				PolygonText_(precision, b_export_zs, b_export_ms, zs, ms, position, path_flags, paths, polygon_start, polygon_end, @string);
			}
		}

		internal static void MultiLineStringText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags, com.epl.geometry.AttributeStreamOfInt32
			 paths, int path_count, System.Text.StringBuilder @string)
		{
			bool b_closed = ((path_flags.Read(0) & com.epl.geometry.PathFlags.enumClosed) != 0);
			LineStringText_(false, b_closed, precision, b_export_zs, b_export_ms, zs, ms, position, paths, 0, @string);
			for (int path = 1; path < path_count; path++)
			{
				@string.Append(", ");
				b_closed = ((path_flags.Read(path) & com.epl.geometry.PathFlags.enumClosed) != 0);
				LineStringText_(false, b_closed, precision, b_export_zs, b_export_ms, zs, ms, position, paths, path, @string);
			}
		}

		internal static void MultiPointText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int point_count, System.Text.StringBuilder @string)
		{
			PointText_(precision, b_export_zs, b_export_ms, zs, ms, position, 0, @string);
			for (int point = 1; point < point_count; point++)
			{
				@string.Append(", ");
				PointText_(precision, b_export_zs, b_export_ms, zs, ms, position, point, @string);
			}
		}

		internal static void PolygonText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt8 path_flags, com.epl.geometry.AttributeStreamOfInt32
			 paths, int polygon_start, int polygon_end, System.Text.StringBuilder @string)
		{
			@string.Append('(');
			LineStringText_(true, true, precision, b_export_zs, b_export_ms, zs, ms, position, paths, polygon_start, @string);
			for (int path = polygon_start + 1; path < polygon_end; path++)
			{
				@string.Append(", ");
				LineStringText_(true, true, precision, b_export_zs, b_export_ms, zs, ms, position, paths, path, @string);
			}
			@string.Append(')');
		}

		internal static void LineStringText_(bool bRing, bool b_closed, int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32
			 paths, int path, System.Text.StringBuilder @string)
		{
			int istart = paths.Read(path);
			int iend = paths.Read(path + 1);
			if (istart == iend)
			{
				@string.Append("EMPTY");
				return;
			}
			@string.Append('(');
			if (bRing)
			{
				Point_(precision, b_export_zs, b_export_ms, zs, ms, position, istart, @string);
				@string.Append(", ");
				for (int point = iend - 1; point >= istart + 1; point--)
				{
					Point_(precision, b_export_zs, b_export_ms, zs, ms, position, point, @string);
					@string.Append(", ");
				}
				Point_(precision, b_export_zs, b_export_ms, zs, ms, position, istart, @string);
			}
			else
			{
				for (int point = istart; point < iend - 1; point++)
				{
					Point_(precision, b_export_zs, b_export_ms, zs, ms, position, point, @string);
					@string.Append(", ");
				}
				Point_(precision, b_export_zs, b_export_ms, zs, ms, position, iend - 1, @string);
				if (b_closed)
				{
					@string.Append(", ");
					Point_(precision, b_export_zs, b_export_ms, zs, ms, position, istart, @string);
				}
			}
			@string.Append(')');
		}

		internal static int PointText_(int precision, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, System.Text.StringBuilder @string)
		{
			@string.Append('(');
			Point_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
			@string.Append(')');
			return 1;
		}

		internal static void PointText_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int point, System.Text.StringBuilder @string)
		{
			double x = position.Read(2 * point);
			double y = position.Read(2 * point + 1);
			double z = com.epl.geometry.NumberUtils.TheNaN;
			double m = com.epl.geometry.NumberUtils.TheNaN;
			if (b_export_zs)
			{
				z = (zs != null ? zs.Read(point) : com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z));
			}
			if (b_export_ms)
			{
				m = (ms != null ? ms.Read(point) : com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M));
			}
			PointText_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
		}

		internal static void Point_(int precision, bool b_export_zs, bool b_export_ms, double x, double y, double z, double m, System.Text.StringBuilder @string)
		{
			WriteSignedNumericLiteral_(x, precision, @string);
			@string.Append(' ');
			WriteSignedNumericLiteral_(y, precision, @string);
			if (b_export_zs)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(z, precision, @string);
			}
			if (b_export_ms)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(m, precision, @string);
			}
		}

		internal static void Point_(int precision, bool b_export_zs, bool b_export_ms, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, int point, System.Text.StringBuilder @string)
		{
			double x = position.Read(2 * point);
			double y = position.Read(2 * point + 1);
			double z = com.epl.geometry.NumberUtils.TheNaN;
			double m = com.epl.geometry.NumberUtils.TheNaN;
			if (b_export_zs)
			{
				z = (zs != null ? zs.Read(point) : com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z));
			}
			if (b_export_ms)
			{
				m = (ms != null ? ms.Read(point) : com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M));
			}
			Point_(precision, b_export_zs, b_export_ms, x, y, z, m, @string);
		}

		internal static bool WriteSignedNumericLiteral_(double v, int precision, System.Text.StringBuilder @string)
		{
			if (com.epl.geometry.NumberUtils.IsNaN(v))
			{
				@string.Append("NAN");
				return false;
			}
			com.epl.geometry.StringUtils.AppendDouble(v, precision, @string);
			return true;
		}

		internal static void WriteEnvelopeAsWktPolygon_(int precision, bool b_export_zs, bool b_export_ms, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax, System.Text.StringBuilder @string)
		{
			@string.Append("((");
			WriteSignedNumericLiteral_(xmin, precision, @string);
			@string.Append(' ');
			WriteSignedNumericLiteral_(ymin, precision, @string);
			if (b_export_zs)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(zmin, precision, @string);
			}
			if (b_export_ms)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(mmin, precision, @string);
			}
			@string.Append(", ");
			WriteSignedNumericLiteral_(xmax, precision, @string);
			@string.Append(' ');
			WriteSignedNumericLiteral_(ymin, precision, @string);
			if (b_export_zs)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(zmax, precision, @string);
			}
			if (b_export_ms)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(mmax, precision, @string);
			}
			@string.Append(", ");
			WriteSignedNumericLiteral_(xmax, precision, @string);
			@string.Append(' ');
			WriteSignedNumericLiteral_(ymax, precision, @string);
			if (b_export_zs)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(zmin, precision, @string);
			}
			if (b_export_ms)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(mmin, precision, @string);
			}
			@string.Append(", ");
			WriteSignedNumericLiteral_(xmin, precision, @string);
			@string.Append(' ');
			WriteSignedNumericLiteral_(ymax, precision, @string);
			if (b_export_zs)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(zmax, precision, @string);
			}
			if (b_export_ms)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(mmax, precision, @string);
			}
			@string.Append(", ");
			WriteSignedNumericLiteral_(xmin, precision, @string);
			@string.Append(' ');
			WriteSignedNumericLiteral_(ymin, precision, @string);
			if (b_export_zs)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(zmin, precision, @string);
			}
			if (b_export_ms)
			{
				@string.Append(' ');
				WriteSignedNumericLiteral_(mmin, precision, @string);
			}
			@string.Append("))");
		}
	}
}
