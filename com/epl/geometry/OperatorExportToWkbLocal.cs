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
	internal class OperatorExportToWkbLocal : com.epl.geometry.OperatorExportToWkb
	{
		public override System.IO.MemoryStream Execute(int exportFlags, com.epl.geometry.Geometry geometry, com.epl.geometry.ProgressTracker progressTracker)
		{
			int size = ExportToWKB(exportFlags, geometry, null);
			System.IO.MemoryStream wkbBuffer = System.IO.MemoryStream.Allocate(size).Order(java.nio.ByteOrder.NativeOrder());
			ExportToWKB(exportFlags, geometry, wkbBuffer);
			return wkbBuffer;
		}

		public override int Execute(int exportFlags, com.epl.geometry.Geometry geometry, System.IO.MemoryStream wkbBuffer, com.epl.geometry.ProgressTracker progressTracker)
		{
			return ExportToWKB(exportFlags, geometry, wkbBuffer);
		}

		private static int ExportToWKB(int exportFlags, com.epl.geometry.Geometry geometry, System.IO.MemoryStream wkbBuffer)
		{
			if (geometry == null)
			{
				return 0;
			}
			int type = geometry.GetType().Value();
			switch (type)
			{
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags
						.wkbExportMultiPoint) != 0)
					{
						throw new com.epl.geometry.GeometryException("invalid argument");
					}
					return ExportPolygonToWKB(exportFlags, (com.epl.geometry.Polygon)geometry, wkbBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPoint
						) != 0)
					{
						throw new com.epl.geometry.GeometryException("invalid argument");
					}
					return ExportPolylineToWKB(exportFlags, (com.epl.geometry.Polyline)geometry, wkbBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags
						.wkbExportMultiPolygon) != 0)
					{
						throw new com.epl.geometry.GeometryException("invalid argument");
					}
					return ExportMultiPointToWKB(exportFlags, (com.epl.geometry.MultiPoint)geometry, wkbBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags
						.wkbExportMultiPolygon) != 0)
					{
						throw new com.epl.geometry.GeometryException("invalid argument");
					}
					return ExportPointToWKB(exportFlags, (com.epl.geometry.Point)geometry, wkbBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiLineString) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) != 0 || (exportFlags & com.epl.geometry.WkbExportFlags
						.wkbExportMultiPoint) != 0)
					{
						throw new com.epl.geometry.GeometryException("invalid argument");
					}
					return ExportEnvelopeToWKB(exportFlags, (com.epl.geometry.Envelope)geometry, wkbBuffer);
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		// return -1;
		private static int ExportPolygonToWKB(int exportFlags, com.epl.geometry.Polygon _polygon, System.IO.MemoryStream wkbBuffer)
		{
			com.epl.geometry.MultiPathImpl polygon = (com.epl.geometry.MultiPathImpl)_polygon._getImpl();
			if ((exportFlags & (int)com.epl.geometry.WkbExportFlags.wkbExportFailIfNotSimple) != 0)
			{
				int simple = polygon.GetIsSimple(0.0);
				if (simple != com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Strong)
				{
					throw new com.epl.geometry.GeometryException("non simple geometry");
				}
			}
			bool bExportZs = polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & (int)com.epl.geometry.WkbExportFlags.wkbExportStripZs) == 0;
			bool bExportMs = polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & (int)com.epl.geometry.WkbExportFlags.wkbExportStripMs) == 0;
			int polygonCount = polygon.GetOGCPolygonCount();
			if ((exportFlags & (int)com.epl.geometry.WkbExportFlags.wkbExportPolygon) != 0 && polygonCount > 1)
			{
				throw new System.ArgumentException();
			}
			int partCount = polygon.GetPathCount();
			int point_count = polygon.GetPointCount();
			point_count += partCount;
			// add 1 point per part
			if (point_count > 0 && polygonCount == 0)
			{
				throw new com.epl.geometry.GeometryException("corrupted geometry");
			}
			// In the WKB_export_defaults case, polygons gets exported as a
			// WKB_multi_polygon.
			// get size for buffer
			int size = 0;
			if ((exportFlags & (int)com.epl.geometry.WkbExportFlags.wkbExportPolygon) == 0 || polygonCount == 0)
			{
				size += 1 + 4 + 4;
			}
			/* byte order */
			/* wkbType */
			/* numPolygons */
			size += polygonCount * (1 + 4 + 4) + partCount * (4) + point_count * (2 * 8);
			/* byte order */
			/* wkbType */
			/* numRings */
			/* num_points */
			/*
			* xy
			* coordinates
			*/
			if (bExportZs)
			{
				size += (point_count * 8);
			}
			/* zs */
			if (bExportMs)
			{
				size += (point_count * 8);
			}
			/* ms */
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (wkbBuffer == null)
			{
				return (int)size;
			}
			else
			{
				if (wkbBuffer.Capacity() < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int offset = 0;
			byte byteOrder = unchecked((byte)(wkbBuffer.Order() == java.nio.ByteOrder.LITTLE_ENDIAN ? com.epl.geometry.WkbByteOrder.wkbNDR : com.epl.geometry.WkbByteOrder.wkbXDR));
			// Determine the wkb type
			int type;
			if (!bExportZs && !bExportMs)
			{
				type = com.epl.geometry.WkbGeometryType.wkbPolygon;
				if ((exportFlags & com.epl.geometry.WktExportFlags.wktExportPolygon) == 0)
				{
					wkbBuffer.Put(offset, byteOrder);
					offset += 1;
					wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygon);
					offset += 4;
					wkbBuffer.PutInt(offset, polygonCount);
					offset += 4;
				}
				else
				{
					if (polygonCount == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygon);
						offset += 4;
						wkbBuffer.PutInt(offset, 0);
						offset += 4;
					}
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					type = com.epl.geometry.WkbGeometryType.wkbPolygonZ;
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonZ);
						offset += 4;
						wkbBuffer.PutInt(offset, polygonCount);
						offset += 4;
					}
					else
					{
						if (polygonCount == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygonZ);
							offset += 4;
							wkbBuffer.PutInt(offset, 0);
							offset += 4;
						}
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						type = com.epl.geometry.WkbGeometryType.wkbPolygonM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonM);
							offset += 4;
							wkbBuffer.PutInt(offset, (int)polygonCount);
							offset += 4;
						}
						else
						{
							if (polygonCount == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygonM);
								offset += 4;
								wkbBuffer.PutInt(offset, 0);
								offset += 4;
							}
						}
					}
					else
					{
						type = com.epl.geometry.WkbGeometryType.wkbPolygonZM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM);
							offset += 4;
							wkbBuffer.PutInt(offset, polygonCount);
							offset += 4;
						}
						else
						{
							if (polygonCount == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygonZM);
								offset += 4;
								wkbBuffer.PutInt(offset, 0);
								offset += 4;
							}
						}
					}
				}
			}
			if (polygonCount == 0)
			{
				return offset;
			}
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(polygon.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			if (bExportZs)
			{
				if (polygon._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)(polygon.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z));
				}
			}
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			if (bExportMs)
			{
				if (polygon._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)(polygon.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M));
				}
			}
			int ipartend = 0;
			int ipolygonend = 0;
			for (int ipolygon = 0; ipolygon < (int)polygonCount; ipolygon++)
			{
				// write byte order
				wkbBuffer.Put(offset, byteOrder);
				offset += 1;
				// write type
				wkbBuffer.PutInt(offset, type);
				offset += 4;
				// get partcount for the ith polygon
				com.epl.geometry.AttributeStreamOfInt8 pathFlags = polygon.GetPathFlagsStreamRef();
				int ipolygonstart = ipolygonend;
				ipolygonend++;
				while (ipolygonend < partCount && (pathFlags.Read(ipolygonend) & com.epl.geometry.PathFlags.enumOGCStartPolygon) == 0)
				{
					ipolygonend++;
				}
				// write numRings
				wkbBuffer.PutInt(offset, ipolygonend - ipolygonstart);
				offset += 4;
				for (int ipart = ipolygonstart; ipart < ipolygonend; ipart++)
				{
					// get num_points
					int ipartstart = ipartend;
					ipartend = (int)polygon.GetPathEnd(ipart);
					// write num_points
					wkbBuffer.PutInt(offset, ipartend - ipartstart + 1);
					offset += 4;
					// duplicate the start point
					double x = position.Read(2 * ipartstart);
					double y = position.Read(2 * ipartstart + 1);
					wkbBuffer.PutDouble(offset, x);
					offset += 8;
					wkbBuffer.PutDouble(offset, y);
					offset += 8;
					if (bExportZs)
					{
						double z;
						if (zs != null)
						{
							z = zs.Read(ipartstart);
						}
						else
						{
							z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
						}
						wkbBuffer.PutDouble(offset, z);
						offset += 8;
					}
					if (bExportMs)
					{
						double m;
						if (ms != null)
						{
							m = ms.Read(ipartstart);
						}
						else
						{
							m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
						}
						wkbBuffer.PutDouble(offset, m);
						offset += 8;
					}
					// We must write to the buffer backwards - ogc polygon format is
					// opposite of shapefile format
					for (int i = ipartend - 1; i >= ipartstart; i--)
					{
						x = position.Read(2 * i);
						y = position.Read(2 * i + 1);
						wkbBuffer.PutDouble(offset, x);
						offset += 8;
						wkbBuffer.PutDouble(offset, y);
						offset += 8;
						if (bExportZs)
						{
							double z;
							if (zs != null)
							{
								z = zs.Read(i);
							}
							else
							{
								z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
							}
							wkbBuffer.PutDouble(offset, z);
							offset += 8;
						}
						if (bExportMs)
						{
							double m;
							if (ms != null)
							{
								m = ms.Read(i);
							}
							else
							{
								m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
							}
							wkbBuffer.PutDouble(offset, m);
							offset += 8;
						}
					}
				}
			}
			return offset;
		}

		private static int ExportPolylineToWKB(int exportFlags, com.epl.geometry.Polyline _polyline, System.IO.MemoryStream wkbBuffer)
		{
			com.epl.geometry.MultiPathImpl polyline = (com.epl.geometry.MultiPathImpl)_polyline._getImpl();
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportFailIfNotSimple) != 0)
			{
				int simple = polyline.GetIsSimple(0.0);
				if (simple < 1)
				{
					throw new com.epl.geometry.GeometryException("corrupted geometry");
				}
			}
			bool bExportZs = polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripZs) == 0;
			bool bExportMs = polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripMs) == 0;
			int partCount = polyline.GetPathCount();
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) != 0 && partCount > 1)
			{
				throw new System.ArgumentException();
			}
			int point_count = polyline.GetPointCount();
			for (int ipart = 0; ipart < partCount; ipart++)
			{
				if (polyline.IsClosedPath(ipart))
				{
					point_count++;
				}
			}
			// In the WKB_export_defaults case, polylines gets exported as a
			// WKB_multi_line_string
			// get size for buffer
			int size = 0;
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) == 0 || partCount == 0)
			{
				size += 1 + 4 + 4;
			}
			/* byte order */
			/* wkbType */
			/* numLineStrings */
			size += partCount * (1 + 4 + 4) + point_count * (2 * 8);
			/* byte order */
			/* wkbType */
			/* num_points */
			/* xy coordinates */
			if (bExportZs)
			{
				size += (point_count * 8);
			}
			/* zs */
			if (bExportMs)
			{
				size += (point_count * 8);
			}
			/* ms */
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (wkbBuffer == null)
			{
				return (int)size;
			}
			else
			{
				if (wkbBuffer.Capacity() < (int)size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int offset = 0;
			byte byteOrder = unchecked((byte)(wkbBuffer.Order() == java.nio.ByteOrder.LITTLE_ENDIAN ? com.epl.geometry.WkbByteOrder.wkbNDR : com.epl.geometry.WkbByteOrder.wkbXDR));
			// Determine the wkb type
			int type;
			if (!bExportZs && !bExportMs)
			{
				type = com.epl.geometry.WkbGeometryType.wkbLineString;
				if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) == 0)
				{
					wkbBuffer.Put(offset, byteOrder);
					offset += 1;
					wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiLineString);
					offset += 4;
					wkbBuffer.PutInt(offset, (int)partCount);
					offset += 4;
				}
				else
				{
					if (partCount == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineString);
						offset += 4;
						wkbBuffer.PutInt(offset, 0);
						offset += 4;
					}
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					type = com.epl.geometry.WkbGeometryType.wkbLineStringZ;
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiLineStringZ);
						offset += 4;
						wkbBuffer.PutInt(offset, (int)partCount);
						offset += 4;
					}
					else
					{
						if (partCount == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineStringZ);
							offset += 4;
							wkbBuffer.PutInt(offset, 0);
							offset += 4;
						}
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						type = com.epl.geometry.WkbGeometryType.wkbLineStringM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiLineStringM);
							offset += 4;
							wkbBuffer.PutInt(offset, (int)partCount);
							offset += 4;
						}
						else
						{
							if (partCount == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineStringM);
								offset += 4;
								wkbBuffer.PutInt(offset, 0);
								offset += 4;
							}
						}
					}
					else
					{
						type = com.epl.geometry.WkbGeometryType.wkbLineStringZM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportLineString) == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiLineStringZM);
							offset += 4;
							wkbBuffer.PutInt(offset, partCount);
							offset += 4;
						}
						else
						{
							if (partCount == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineStringZM);
								offset += 4;
								wkbBuffer.PutInt(offset, 0);
								offset += 4;
							}
						}
					}
				}
			}
			if (partCount == 0)
			{
				return offset;
			}
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(polyline.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			if (bExportZs)
			{
				if (polyline._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)(polyline.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z));
				}
			}
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			if (bExportMs)
			{
				if (polyline._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)(polyline.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M));
				}
			}
			int ipartend = 0;
			for (int ipart_1 = 0; ipart_1 < (int)partCount; ipart_1++)
			{
				// write byte order
				wkbBuffer.Put(offset, byteOrder);
				offset += 1;
				// write type
				wkbBuffer.PutInt(offset, type);
				offset += 4;
				// get start and end indices
				int ipartstart = ipartend;
				ipartend = (int)polyline.GetPathEnd(ipart_1);
				// write num_points
				int num_points = ipartend - ipartstart;
				if (polyline.IsClosedPath(ipart_1))
				{
					num_points++;
				}
				wkbBuffer.PutInt(offset, num_points);
				offset += 4;
				// write points
				for (int i = ipartstart; i < ipartend; i++)
				{
					double x = position.Read(2 * i);
					double y = position.Read(2 * i + 1);
					wkbBuffer.PutDouble(offset, x);
					offset += 8;
					wkbBuffer.PutDouble(offset, y);
					offset += 8;
					if (bExportZs)
					{
						double z;
						if (zs != null)
						{
							z = zs.Read(i);
						}
						else
						{
							z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
						}
						wkbBuffer.PutDouble(offset, z);
						offset += 8;
					}
					if (bExportMs)
					{
						double m;
						if (ms != null)
						{
							m = ms.Read(i);
						}
						else
						{
							m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
						}
						wkbBuffer.PutDouble(offset, m);
						offset += 8;
					}
				}
				// duplicate the start point if the Polyline is closed
				if (polyline.IsClosedPath(ipart_1))
				{
					double x = position.Read(2 * ipartstart);
					double y = position.Read(2 * ipartstart + 1);
					wkbBuffer.PutDouble(offset, x);
					offset += 8;
					wkbBuffer.PutDouble(offset, y);
					offset += 8;
					if (bExportZs)
					{
						double z;
						if (zs != null)
						{
							z = zs.Read(ipartstart);
						}
						else
						{
							z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
						}
						wkbBuffer.PutDouble(offset, z);
						offset += 8;
					}
					if (bExportMs)
					{
						double m;
						if (ms != null)
						{
							m = ms.Read(ipartstart);
						}
						else
						{
							m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
						}
						wkbBuffer.PutDouble(offset, m);
						offset += 8;
					}
				}
			}
			return offset;
		}

		private static int ExportMultiPointToWKB(int exportFlags, com.epl.geometry.MultiPoint _multipoint, System.IO.MemoryStream wkbBuffer)
		{
			com.epl.geometry.MultiPointImpl multipoint = (com.epl.geometry.MultiPointImpl)_multipoint._getImpl();
			bool bExportZs = multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripZs) == 0;
			bool bExportMs = multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripMs) == 0;
			int point_count = multipoint.GetPointCount();
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) != 0 && point_count > 1)
			{
				throw new System.ArgumentException();
			}
			// get size for buffer
			int size;
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) == 0)
			{
				size = 1 + 4 + 4 + point_count * (1 + 4 + 2 * 8);
				/* byte order */
				/* wkbType */
				/* num_points */
				/* byte order */
				/* wkbType */
				/*
				* xy
				* coordinates
				*/
				if (bExportZs)
				{
					size += (point_count * 8);
				}
				/* zs */
				if (bExportMs)
				{
					size += (point_count * 8);
				}
			}
			else
			{
				/* ms */
				size = 1 + 4 + 2 * 8;
				/* byte order */
				/* wkbType */
				/* xy coordinates */
				if (bExportZs)
				{
					size += 8;
				}
				/* z */
				if (bExportMs)
				{
					size += 8;
				}
			}
			/* m */
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (wkbBuffer == null)
			{
				return (int)size;
			}
			else
			{
				if (wkbBuffer.Capacity() < (int)size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int offset = 0;
			byte byteOrder = unchecked((byte)(wkbBuffer.Order() == java.nio.ByteOrder.LITTLE_ENDIAN ? com.epl.geometry.WkbByteOrder.wkbNDR : com.epl.geometry.WkbByteOrder.wkbXDR));
			// Determine the wkb type
			int type;
			if (!bExportZs && !bExportMs)
			{
				type = com.epl.geometry.WkbGeometryType.wkbPoint;
				if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) == 0)
				{
					wkbBuffer.Put(offset, byteOrder);
					offset += 1;
					wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPoint);
					offset += 4;
					wkbBuffer.PutInt(offset, (int)point_count);
					offset += 4;
				}
				else
				{
					if (point_count == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, type);
						offset += 4;
						wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
						offset += 8;
						wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
						offset += 8;
					}
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					type = com.epl.geometry.WkbGeometryType.wkbPointZ;
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPointZ);
						offset += 4;
						wkbBuffer.PutInt(offset, (int)point_count);
						offset += 4;
					}
					else
					{
						if (point_count == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, type);
							offset += 4;
							wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
							offset += 8;
							wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
							offset += 8;
							wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
							offset += 8;
						}
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						type = com.epl.geometry.WkbGeometryType.wkbPointM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPointM);
							offset += 4;
							wkbBuffer.PutInt(offset, (int)point_count);
							offset += 4;
						}
						else
						{
							if (point_count == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, type);
								offset += 4;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
							}
						}
					}
					else
					{
						type = com.epl.geometry.WkbGeometryType.wkbPointZM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPoint) == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM);
							offset += 4;
							wkbBuffer.PutInt(offset, point_count);
							offset += 4;
						}
						else
						{
							if (point_count == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, type);
								offset += 4;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
							}
						}
					}
				}
			}
			if (point_count == 0)
			{
				return offset;
			}
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(multipoint.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			if (bExportZs)
			{
				if (multipoint._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)(multipoint.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z));
				}
			}
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			if (bExportMs)
			{
				if (multipoint._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)(multipoint.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M));
				}
			}
			for (int i = 0; i < (int)point_count; i++)
			{
				// write byte order
				wkbBuffer.Put(offset, byteOrder);
				offset += 1;
				// write type
				wkbBuffer.PutInt(offset, type);
				offset += 4;
				// write xy coordinates
				double x = position.Read(2 * i);
				double y = position.Read(2 * i + 1);
				wkbBuffer.PutDouble(offset, x);
				offset += 8;
				wkbBuffer.PutDouble(offset, y);
				offset += 8;
				// write Z
				if (bExportZs)
				{
					double z;
					if (zs != null)
					{
						z = zs.Read(i);
					}
					else
					{
						z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
					}
					wkbBuffer.PutDouble(offset, z);
					offset += 8;
				}
				// write M
				if (bExportMs)
				{
					double m;
					if (ms != null)
					{
						m = ms.Read(i);
					}
					else
					{
						m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
					}
					wkbBuffer.PutDouble(offset, m);
					offset += 8;
				}
			}
			return offset;
		}

		private static int ExportPointToWKB(int exportFlags, com.epl.geometry.Point point, System.IO.MemoryStream wkbBuffer)
		{
			bool bExportZs = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripZs) == 0;
			bool bExportMs = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripMs) == 0;
			bool bEmpty = point.IsEmpty();
			int point_count = bEmpty ? 0 : 1;
			// get size for buffer
			int size;
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPoint) != 0)
			{
				size = 1 + 4 + 4 + point_count * (1 + 4 + 2 * 8);
				/* byte order */
				/* wkbType */
				/* num_points */
				/* byte order */
				/* wkbType */
				/*
				* xy
				* coordinates
				*/
				if (bExportZs)
				{
					size += (point_count * 8);
				}
				/* zs */
				if (bExportMs)
				{
					size += (point_count * 8);
				}
			}
			else
			{
				/* ms */
				size = 1 + 4 + 2 * 8;
				/* byte order */
				/* wkbType */
				/* xy coordinates */
				if (bExportZs)
				{
					size += 8;
				}
				/* z */
				if (bExportMs)
				{
					size += 8;
				}
			}
			/* m */
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (wkbBuffer == null)
			{
				return size;
			}
			else
			{
				if (wkbBuffer.Capacity() < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int offset = 0;
			byte byteOrder = unchecked((byte)(wkbBuffer.Order() == java.nio.ByteOrder.LITTLE_ENDIAN ? com.epl.geometry.WkbByteOrder.wkbNDR : com.epl.geometry.WkbByteOrder.wkbXDR));
			// Determine the wkb type
			int type;
			if (!bExportZs && !bExportMs)
			{
				type = com.epl.geometry.WkbGeometryType.wkbPoint;
				if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPoint) != 0)
				{
					wkbBuffer.Put(offset, byteOrder);
					offset += 1;
					wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPoint);
					offset += 4;
					wkbBuffer.PutInt(offset, (int)point_count);
					offset += 4;
				}
				else
				{
					if (point_count == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, type);
						offset += 4;
						wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
						offset += 8;
						wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
						offset += 8;
					}
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					type = com.epl.geometry.WkbGeometryType.wkbPointZ;
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPoint) != 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPointZ);
						offset += 4;
						wkbBuffer.PutInt(offset, (int)point_count);
						offset += 4;
					}
					else
					{
						if (point_count == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, type);
							offset += 4;
							wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
							offset += 8;
							wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
							offset += 8;
							wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
							offset += 8;
						}
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						type = com.epl.geometry.WkbGeometryType.wkbPointM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPoint) != 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPointM);
							offset += 4;
							wkbBuffer.PutInt(offset, (int)point_count);
							offset += 4;
						}
						else
						{
							if (point_count == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, type);
								offset += 4;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
							}
						}
					}
					else
					{
						type = com.epl.geometry.WkbGeometryType.wkbPointZM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPoint) != 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPointZM);
							offset += 4;
							wkbBuffer.PutInt(offset, (int)point_count);
							offset += 4;
						}
						else
						{
							if (point_count == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, type);
								offset += 4;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
								wkbBuffer.PutDouble(offset, com.epl.geometry.NumberUtils.TheNaN);
								offset += 8;
							}
						}
					}
				}
			}
			if (point_count == 0)
			{
				return offset;
			}
			// write byte order
			wkbBuffer.Put(offset, byteOrder);
			offset += 1;
			// write type
			wkbBuffer.PutInt(offset, type);
			offset += 4;
			// write xy coordinate
			double x = point.GetX();
			double y = point.GetY();
			wkbBuffer.PutDouble(offset, x);
			offset += 8;
			wkbBuffer.PutDouble(offset, y);
			offset += 8;
			// write Z
			if (bExportZs)
			{
				double z = point.GetZ();
				wkbBuffer.PutDouble(offset, z);
				offset += 8;
			}
			// write M
			if (bExportMs)
			{
				double m = point.GetM();
				wkbBuffer.PutDouble(offset, m);
				offset += 8;
			}
			return offset;
		}

		private static int ExportEnvelopeToWKB(int exportFlags, com.epl.geometry.Envelope envelope, System.IO.MemoryStream wkbBuffer)
		{
			bool bExportZs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripZs) == 0;
			bool bExportMs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.WkbExportFlags.wkbExportStripMs) == 0;
			bool bEmpty = envelope.IsEmpty();
			int partCount = bEmpty ? 0 : 1;
			int point_count = bEmpty ? 0 : 5;
			// Envelope by default is exported as a WKB_polygon
			// get size for buffer
			int size = 0;
			if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon) != 0 || partCount == 0)
			{
				size += 1 + 4 + 4;
			}
			/* byte order */
			/* wkbType */
			/* numPolygons */
			size += partCount * (1 + 4 + 4) + partCount * (4) + point_count * (2 * 8);
			/* byte order */
			/* wkbType */
			/* numRings */
			/* num_points */
			/*
			* xy
			* coordinates
			*/
			if (bExportZs)
			{
				size += (point_count * 8);
			}
			/* zs */
			if (bExportMs)
			{
				size += (point_count * 8);
			}
			/* ms */
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (wkbBuffer == null)
			{
				return size;
			}
			else
			{
				if (wkbBuffer.Capacity() < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int offset = 0;
			byte byteOrder = unchecked((byte)(wkbBuffer.Order() == java.nio.ByteOrder.LITTLE_ENDIAN ? com.epl.geometry.WkbByteOrder.wkbNDR : com.epl.geometry.WkbByteOrder.wkbXDR));
			// Determine the wkb type
			int type;
			if (!bExportZs && !bExportMs)
			{
				type = com.epl.geometry.WkbGeometryType.wkbPolygon;
				if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon) != 0)
				{
					wkbBuffer.Put(offset, byteOrder);
					offset += 1;
					wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygon);
					offset += 4;
					wkbBuffer.PutInt(offset, (int)partCount);
					offset += 4;
				}
				else
				{
					if (partCount == 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygon);
						offset += 4;
						wkbBuffer.PutInt(offset, 0);
						offset += 4;
					}
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					type = com.epl.geometry.WkbGeometryType.wkbPolygonZ;
					if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportPolygon) != 0)
					{
						wkbBuffer.Put(offset, byteOrder);
						offset += 1;
						wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonZ);
						offset += 4;
						wkbBuffer.PutInt(offset, partCount);
						offset += 4;
					}
					else
					{
						if (partCount == 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygonZ);
							offset += 4;
							wkbBuffer.PutInt(offset, 0);
							offset += 4;
						}
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						type = com.epl.geometry.WkbGeometryType.wkbPolygonM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon) != 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonM);
							offset += 4;
							wkbBuffer.PutInt(offset, partCount);
							offset += 4;
						}
						else
						{
							if (partCount == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygonM);
								offset += 4;
								wkbBuffer.PutInt(offset, 0);
								offset += 4;
							}
						}
					}
					else
					{
						type = com.epl.geometry.WkbGeometryType.wkbPolygonZM;
						if ((exportFlags & com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon) != 0)
						{
							wkbBuffer.Put(offset, byteOrder);
							offset += 1;
							wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM);
							offset += 4;
							wkbBuffer.PutInt(offset, partCount);
							offset += 4;
						}
						else
						{
							if (partCount == 0)
							{
								wkbBuffer.Put(offset, byteOrder);
								offset += 1;
								wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygonZM);
								offset += 4;
								wkbBuffer.PutInt(offset, 0);
								offset += 4;
							}
						}
					}
				}
			}
			if (partCount == 0)
			{
				return offset;
			}
			// write byte order
			wkbBuffer.Put(offset, byteOrder);
			offset += 1;
			// write type
			wkbBuffer.PutInt(offset, type);
			offset += 4;
			// write numRings
			wkbBuffer.PutInt(offset, 1);
			offset += 4;
			// write num_points
			wkbBuffer.PutInt(offset, 5);
			offset += 4;
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			envelope.QueryEnvelope2D(env);
			com.epl.geometry.Envelope1D z_interval = null;
			if (bExportZs)
			{
				z_interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			}
			com.epl.geometry.Envelope1D mInterval = null;
			if (bExportMs)
			{
				mInterval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
			}
			wkbBuffer.PutDouble(offset, env.xmin);
			offset += 8;
			wkbBuffer.PutDouble(offset, env.ymin);
			offset += 8;
			if (bExportZs)
			{
				wkbBuffer.PutDouble(offset, z_interval.vmin);
				offset += 8;
			}
			if (bExportMs)
			{
				wkbBuffer.PutDouble(offset, mInterval.vmin);
				offset += 8;
			}
			wkbBuffer.PutDouble(offset, env.xmax);
			offset += 8;
			wkbBuffer.PutDouble(offset, env.ymin);
			offset += 8;
			if (bExportZs)
			{
				wkbBuffer.PutDouble(offset, z_interval.vmax);
				offset += 8;
			}
			if (bExportMs)
			{
				wkbBuffer.PutDouble(offset, mInterval.vmax);
				offset += 8;
			}
			wkbBuffer.PutDouble(offset, env.xmax);
			offset += 8;
			wkbBuffer.PutDouble(offset, env.ymax);
			offset += 8;
			if (bExportZs)
			{
				wkbBuffer.PutDouble(offset, z_interval.vmin);
				offset += 8;
			}
			if (bExportMs)
			{
				wkbBuffer.PutDouble(offset, mInterval.vmin);
				offset += 8;
			}
			wkbBuffer.PutDouble(offset, env.xmin);
			offset += 8;
			wkbBuffer.PutDouble(offset, env.ymax);
			offset += 8;
			if (bExportZs)
			{
				wkbBuffer.PutDouble(offset, z_interval.vmax);
				offset += 8;
			}
			if (bExportMs)
			{
				wkbBuffer.PutDouble(offset, mInterval.vmax);
				offset += 8;
			}
			wkbBuffer.PutDouble(offset, env.xmin);
			offset += 8;
			wkbBuffer.PutDouble(offset, env.ymin);
			offset += 8;
			if (bExportZs)
			{
				wkbBuffer.PutDouble(offset, z_interval.vmin);
				offset += 8;
			}
			if (bExportMs)
			{
				wkbBuffer.PutDouble(offset, mInterval.vmin);
				offset += 8;
			}
			return offset;
		}
	}
}
