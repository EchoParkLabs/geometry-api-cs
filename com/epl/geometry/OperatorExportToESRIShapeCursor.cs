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
	internal class OperatorExportToESRIShapeCursor : com.epl.geometry.ByteBufferCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal int m_exportFlags;

		internal int m_index;

		internal System.IO.MemoryStream m_shapeBuffer;

		public OperatorExportToESRIShapeCursor(int exportFlags, com.epl.geometry.GeometryCursor geometryCursor)
		{
			m_index = -1;
			if (geometryCursor == null)
			{
				throw new com.epl.geometry.GeometryException("invalid argument");
			}
			m_exportFlags = exportFlags;
			m_inputGeometryCursor = geometryCursor;
			m_shapeBuffer = null;
		}

		public override int GetByteBufferID()
		{
			return m_index;
		}

		public override System.IO.MemoryStream Next()
		{
			com.epl.geometry.Geometry geometry = m_inputGeometryCursor.Next();
			if (geometry != null)
			{
				m_index = m_inputGeometryCursor.GetGeometryID();
				int size = ExportToESRIShape(m_exportFlags, geometry, null);
				if (m_shapeBuffer == null || size > m_shapeBuffer.Capacity)
				{
					m_shapeBuffer = new System.IO.MemoryStream(size);
				}
				ExportToESRIShape(m_exportFlags, geometry, new System.IO.BinaryWriter(m_shapeBuffer));
				return m_shapeBuffer;
			}
			return null;
		}

		internal static int ExportToESRIShape(int exportFlags, com.epl.geometry.Geometry geometry, System.IO.BinaryWriter shapeBuffer)
		{
			if (geometry == null)
			{
				if (shapeBuffer != null)
				{
					shapeBuffer.Write(com.epl.geometry.ShapeType.ShapeNull);
				}
				return 4;
			}
			int type = geometry.GetType().Value();
			switch (type)
			{
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					return ExportMultiPathToESRIShape(true, exportFlags, (com.epl.geometry.MultiPath)geometry, shapeBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					return ExportMultiPathToESRIShape(false, exportFlags, (com.epl.geometry.MultiPath)geometry, shapeBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					return ExportMultiPointToESRIShape(exportFlags, (com.epl.geometry.MultiPoint)geometry, shapeBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					return ExportPointToESRIShape(exportFlags, (com.epl.geometry.Point)geometry, shapeBuffer);
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					return ExportEnvelopeToESRIShape(exportFlags, (com.epl.geometry.Envelope)geometry, shapeBuffer);
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		// return -1;
		private static int ExportEnvelopeToESRIShape(int exportFlags, com.epl.geometry.Envelope envelope, System.IO.BinaryWriter shapeBuffer)
		{
			bool bExportZs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripZs) == 0;
			bool bExportMs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripMs) == 0;
			bool bExportIDs = envelope.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripIDs) == 0;
			bool bArcViewNaNs = (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportTrueNaNs) == 0;
			bool bEmpty = envelope.IsEmpty();
			int partCount = bEmpty ? 0 : 1;
			int pointCount = bEmpty ? 0 : 5;
			int size = (4) + (4 * 8) + (4) + (4) + (partCount * 4) + pointCount * 2 * 8;
			/* type */
			/* envelope */
			/* part count */
			/* point count */
			/* start indices */
			/* xy coordinates */
			if (bExportZs)
			{
				size += (2 * 8) + (pointCount * 8);
			}
			/* min max */
			/* zs */
			if (bExportMs)
			{
				size += (2 * 8) + (pointCount * 8);
			}
			/* min max */
			/* ms */
			if (bExportIDs)
			{
				size += (pointCount * 4);
			}
			/* ids */
			if (shapeBuffer == null)
			{
				return size;
			}
			else
			{
				if (((System.IO.MemoryStream)shapeBuffer.BaseStream).Capacity < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int type;
			// Determine the shape type
			if (!bExportZs && !bExportMs)
			{
				if (bExportIDs)
				{
					type = com.epl.geometry.ShapeType.ShapeGeneralPolygon | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
				}
				else
				{
					type = com.epl.geometry.ShapeType.ShapePolygon;
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					if (bExportIDs)
					{
						type = com.epl.geometry.ShapeType.ShapeGeneralPolygon | com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
					}
					else
					{
						type = com.epl.geometry.ShapeType.ShapePolygonZ;
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						if (bExportIDs)
						{
							type = com.epl.geometry.ShapeType.ShapeGeneralPolygon | com.epl.geometry.ShapeModifiers.ShapeHasMs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						else
						{
							type = com.epl.geometry.ShapeType.ShapePolygonM;
						}
					}
					else
					{
						if (bExportIDs)
						{
							type = com.epl.geometry.ShapeType.ShapeGeneralPolygon | com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasMs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						else
						{
							type = com.epl.geometry.ShapeType.ShapePolygonZM;
						}
					}
				}
			}
			int offset = 0;
			// write type
			shapeBuffer.Write(type);
			offset += 4;
			// write Envelope
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			envelope.QueryEnvelope2D(env);
			// calls _VerifyAllStreams
			shapeBuffer.Write(env.xmin);
			offset += 8;
			shapeBuffer.Write(env.ymin);
			offset += 8;
			shapeBuffer.Write(env.xmax);
			offset += 8;
			shapeBuffer.Write(env.ymax);
			offset += 8;
			// write part count
			shapeBuffer.Write(partCount);
			offset += 4;
			// write pointCount
			shapeBuffer.Write(pointCount);
			offset += 4;
			if (!bEmpty)
			{
				// write start index
				shapeBuffer.Write(0);
				offset += 4;
				// write xy coordinates
				shapeBuffer.Write(env.xmin);
				offset += 8;
				shapeBuffer.Write(env.ymin);
				offset += 8;
				shapeBuffer.Write(env.xmin);
				offset += 8;
				shapeBuffer.Write(env.ymax);
				offset += 8;
				shapeBuffer.Write(env.xmax);
				offset += 8;
				shapeBuffer.Write(env.ymax);
				offset += 8;
				shapeBuffer.Write(env.xmax);
				offset += 8;
				shapeBuffer.Write(env.ymin);
				offset += 8;
				shapeBuffer.Write(env.xmin);
				offset += 8;
				shapeBuffer.Write(env.ymin);
				offset += 8;
			}
			// write Zs
			if (bExportZs)
			{
				com.epl.geometry.Envelope1D zInterval;
				zInterval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
				double zmin = bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(zInterval.vmin) : zInterval.vmin;
				double zmax = bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(zInterval.vmax) : zInterval.vmax;
				// write min max values
				shapeBuffer.Write(zmin);
				offset += 8;
				shapeBuffer.Write(zmax);
				offset += 8;
				if (!bEmpty)
				{
					// write arbitrary z values
					shapeBuffer.Write(zmin);
					offset += 8;
					shapeBuffer.Write(zmax);
					offset += 8;
					shapeBuffer.Write(zmin);
					offset += 8;
					shapeBuffer.Write(zmax);
					offset += 8;
					shapeBuffer.Write(zmin);
					offset += 8;
				}
			}
			// write Ms
			if (bExportMs)
			{
				com.epl.geometry.Envelope1D mInterval;
				mInterval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
				double mmin = bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(mInterval.vmin) : mInterval.vmin;
				double mmax = bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(mInterval.vmax) : mInterval.vmax;
				// write min max values
				shapeBuffer.Write(mmin);
				offset += 8;
				shapeBuffer.Write(mmax);
				offset += 8;
				if (!bEmpty)
				{
					// write arbitrary m values
					shapeBuffer.Write(mmin);
					offset += 8;
					shapeBuffer.Write(mmax);
					offset += 8;
					shapeBuffer.Write(mmin);
					offset += 8;
					shapeBuffer.Write(mmax);
					offset += 8;
					shapeBuffer.Write(mmin);
					offset += 8;
				}
			}
			// write IDs
			if (bExportIDs && !bEmpty)
			{
				com.epl.geometry.Envelope1D idInterval;
				idInterval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0);
				int idmin = (int)idInterval.vmin;
				int idmax = (int)idInterval.vmax;
				// write arbitrary id values
				shapeBuffer.Write(idmin);
				offset += 4;
				shapeBuffer.Write(idmax);
				offset += 4;
				shapeBuffer.Write(idmin);
				offset += 4;
				shapeBuffer.Write(idmax);
				offset += 4;
				shapeBuffer.Write(idmin);
				offset += 4;
			}
			return offset;
		}

		private static int ExportPointToESRIShape(int exportFlags, com.epl.geometry.Point point, System.IO.BinaryWriter shapeBuffer)
		{
			bool bExportZ = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripZs) == 0;
			bool bExportM = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripMs) == 0;
			bool bExportID = point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripIDs) == 0;
			bool bArcViewNaNs = (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportTrueNaNs) == 0;
			int size = (4) + (2 * 8);
			/* type */
			/* xy coordinate */
			if (bExportZ)
			{
				size += 8;
			}
			if (bExportM)
			{
				size += 8;
			}
			if (bExportID)
			{
				size += 4;
			}
			if (shapeBuffer == null)
			{
				return size;
			}
			else
			{
				if (((System.IO.MemoryStream)shapeBuffer.BaseStream).Capacity < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int type;
			// Determine the shape type
			if (!bExportZ && !bExportM)
			{
				if (bExportID)
				{
					type = com.epl.geometry.ShapeType.ShapeGeneralPoint | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
				}
				else
				{
					type = com.epl.geometry.ShapeType.ShapePoint;
				}
			}
			else
			{
				if (bExportZ && !bExportM)
				{
					if (bExportID)
					{
						type = com.epl.geometry.ShapeType.ShapeGeneralPoint | com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
					}
					else
					{
						type = com.epl.geometry.ShapeType.ShapePointZ;
					}
				}
				else
				{
					if (bExportM && !bExportZ)
					{
						if (bExportID)
						{
							type = com.epl.geometry.ShapeType.ShapeGeneralPoint | com.epl.geometry.ShapeModifiers.ShapeHasMs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						else
						{
							type = com.epl.geometry.ShapeType.ShapePointM;
						}
					}
					else
					{
						if (bExportID)
						{
							type = com.epl.geometry.ShapeType.ShapeGeneralPoint | com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasMs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						else
						{
							type = com.epl.geometry.ShapeType.ShapePointZM;
						}
					}
				}
			}
			int offset = 0;
			// write type
			shapeBuffer.Write(type);
			offset += 4;
			bool bEmpty = point.IsEmpty();
			// write xy
			double x = !bEmpty ? point.GetX() : com.epl.geometry.NumberUtils.NaN();
			double y = !bEmpty ? point.GetY() : com.epl.geometry.NumberUtils.NaN();
			shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(x) : x);
			offset += 8;
			shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(y) : y);
			offset += 8;
			// write Z
			if (bExportZ)
			{
				double z = !bEmpty ? point.GetZ() : com.epl.geometry.NumberUtils.NaN();
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(z) : z);
				offset += 8;
			}
			// WriteM
			if (bExportM)
			{
				double m = !bEmpty ? point.GetM() : com.epl.geometry.NumberUtils.NaN();
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(m) : m);
				offset += 8;
			}
			// write ID
			if (bExportID)
			{
				int id = !bEmpty ? point.GetID() : 0;
				shapeBuffer.Write(id);
				offset += 4;
			}
			return offset;
		}

		private static int ExportMultiPointToESRIShape(int exportFlags, com.epl.geometry.MultiPoint multipoint, System.IO.BinaryWriter shapeBuffer)
		{
			com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
			bool bExportZs = multipointImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripZs) == 0;
			bool bExportMs = multipointImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripMs) == 0;
			bool bExportIDs = multipointImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripIDs) == 0;
			bool bArcViewNaNs = (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportTrueNaNs) == 0;
			int pointCount = multipointImpl.GetPointCount();
			int size = (4) + (4 * 8) + (4) + (pointCount * 2 * 8);
			/* type */
			/* envelope */
			/* point count */
			/* xy coordinates */
			if (bExportZs)
			{
				size += (2 * 8) + (pointCount * 8);
			}
			/* min max */
			/* zs */
			if (bExportMs)
			{
				size += (2 * 8) + (pointCount * 8);
			}
			/* min max */
			/* ms */
			if (bExportIDs)
			{
				size += pointCount * 4;
			}
			/* ids */
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (shapeBuffer == null)
			{
				return size;
			}
			else
			{
				if (((System.IO.MemoryStream)shapeBuffer.BaseStream).Capacity < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int type;
			// Determine the shape type
			if (!bExportZs && !bExportMs)
			{
				if (bExportIDs)
				{
					type = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
				}
				else
				{
					type = com.epl.geometry.ShapeType.ShapeMultiPoint;
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					if (bExportIDs)
					{
						type = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint | com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
					}
					else
					{
						type = com.epl.geometry.ShapeType.ShapeMultiPointZ;
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						if (bExportIDs)
						{
							type = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint | com.epl.geometry.ShapeModifiers.ShapeHasMs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						else
						{
							type = com.epl.geometry.ShapeType.ShapeMultiPointM;
						}
					}
					else
					{
						if (bExportIDs)
						{
							type = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint | com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasMs | com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						else
						{
							type = com.epl.geometry.ShapeType.ShapeMultiPointZM;
						}
					}
				}
			}
			// write type
			int offset = 0;
			shapeBuffer.Write(type);
			offset += 4;
			// write Envelope
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			multipointImpl.QueryEnvelope2D(env);
			// calls _VerifyAllStreams
			shapeBuffer.Write(env.xmin);
			offset += 8;
			shapeBuffer.Write(env.ymin);
			offset += 8;
			shapeBuffer.Write(env.xmax);
			offset += 8;
			shapeBuffer.Write(env.ymax);
			offset += 8;
			// write point count
			shapeBuffer.Write(pointCount);
			offset += 4;
			if (pointCount > 0)
			{
				// write xy coordinates
				com.epl.geometry.AttributeStreamBase positionStream = multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)positionStream;
				for (int i = 0; i < pointCount; i++)
				{
					double x = position.Read(2 * i);
					double y = position.Read(2 * i + 1);
					shapeBuffer.Write(x);
					offset += 8;
					shapeBuffer.Write(y);
					offset += 8;
				}
			}
			// write Zs
			if (bExportZs)
			{
				com.epl.geometry.Envelope1D zInterval = multipointImpl.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(zInterval.vmin) : zInterval.vmin);
				offset += 8;
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(zInterval.vmax) : zInterval.vmax);
				offset += 8;
				if (pointCount > 0)
				{
					if (multipointImpl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
					{
						com.epl.geometry.AttributeStreamOfDbl zs = (com.epl.geometry.AttributeStreamOfDbl)multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
						for (int i = 0; i < pointCount; i++)
						{
							double z = zs.Read(i);
							shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(z) : z);
							offset += 8;
						}
					}
					else
					{
						double z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
						if (bArcViewNaNs)
						{
							z = com.epl.geometry.Interop.TranslateToAVNaN(z);
						}
						// Can we write a function that writes all these values at
						// once instead of doing a for loop?
						for (int i = 0; i < pointCount; i++)
						{
							shapeBuffer.Write(z);
						}
						offset += 8;
					}
				}
			}
			// write Ms
			if (bExportMs)
			{
				com.epl.geometry.Envelope1D mInterval = multipointImpl.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(mInterval.vmin) : mInterval.vmin);
				offset += 8;
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(mInterval.vmax) : mInterval.vmax);
				offset += 8;
				if (pointCount > 0)
				{
					if (multipointImpl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						com.epl.geometry.AttributeStreamOfDbl ms = (com.epl.geometry.AttributeStreamOfDbl)multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
						for (int i = 0; i < pointCount; i++)
						{
							double m = ms.Read(i);
							shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(m) : m);
							offset += 8;
						}
					}
					else
					{
						double m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
						if (bArcViewNaNs)
						{
							m = com.epl.geometry.Interop.TranslateToAVNaN(m);
						}
						for (int i = 0; i < pointCount; i++)
						{
							shapeBuffer.Write(m);
						}
						offset += 8;
					}
				}
			}
			// write IDs
			if (bExportIDs)
			{
				if (pointCount > 0)
				{
					if (multipointImpl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.ID))
					{
						com.epl.geometry.AttributeStreamOfInt32 ids = (com.epl.geometry.AttributeStreamOfInt32)multipointImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.ID);
						for (int i = 0; i < pointCount; i++)
						{
							int id = ids.Read(i);
							shapeBuffer.Write(id);
							offset += 4;
						}
					}
					else
					{
						int id = (int)com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.ID);
						for (int i = 0; i < pointCount; i++)
						{
							shapeBuffer.Write(id);
						}
						offset += 4;
					}
				}
			}
			return offset;
		}

		private static int ExportMultiPathToESRIShape(bool bPolygon, int exportFlags, com.epl.geometry.MultiPath multipath, System.IO.BinaryWriter shapeBuffer)
		{
			com.epl.geometry.MultiPathImpl multipathImpl = (com.epl.geometry.MultiPathImpl)multipath._getImpl();
			bool bExportZs = multipathImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripZs) == 0;
			bool bExportMs = multipathImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripMs) == 0;
			bool bExportIDs = multipathImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID) && (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportStripIDs) == 0;
			bool bHasCurves = multipathImpl.HasNonLinearSegments();
			bool bArcViewNaNs = (exportFlags & com.epl.geometry.ShapeExportFlags.ShapeExportTrueNaNs) == 0;
			int partCount = multipathImpl.GetPathCount();
			int pointCount = multipathImpl.GetPointCount();
			if (!bPolygon)
			{
				for (int ipart = 0; ipart < partCount; ipart++)
				{
					if (multipath.IsClosedPath(ipart))
					{
						pointCount++;
					}
				}
			}
			else
			{
				pointCount += partCount;
			}
			int size = (4) + (4 * 8) + (4) + (4) + (partCount * 4) + pointCount * 2 * 8;
			/* type */
			/* envelope */
			/* part count */
			/* point count */
			/* start indices */
			/* xy coordinates */
			if (bExportZs)
			{
				size += (2 * 8) + (pointCount * 8);
			}
			/* min max */
			/* zs */
			if (bExportMs)
			{
				size += (2 * 8) + (pointCount * 8);
			}
			/* min max */
			/* ms */
			if (bExportIDs)
			{
				size += pointCount * 4;
			}
			/* ids */
			if (bHasCurves)
			{
			}
			// to-do: curves
			if (size >= com.epl.geometry.NumberUtils.IntMax())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (shapeBuffer == null)
			{
				return size;
			}
			else
			{
				if (((System.IO.MemoryStream)shapeBuffer.BaseStream).Capacity < size)
				{
					throw new com.epl.geometry.GeometryException("buffer is too small");
				}
			}
			int offset = 0;
			// Determine the shape type
			int type;
			if (!bExportZs && !bExportMs)
			{
				if (bExportIDs || bHasCurves)
				{
					type = bPolygon ? com.epl.geometry.ShapeType.ShapeGeneralPolygon : com.epl.geometry.ShapeType.ShapeGeneralPolyline;
					if (bExportIDs)
					{
						type |= com.epl.geometry.ShapeModifiers.ShapeHasIDs;
					}
					if (bHasCurves)
					{
						type |= com.epl.geometry.ShapeModifiers.ShapeHasCurves;
					}
				}
				else
				{
					type = bPolygon ? com.epl.geometry.ShapeType.ShapePolygon : com.epl.geometry.ShapeType.ShapePolyline;
				}
			}
			else
			{
				if (bExportZs && !bExportMs)
				{
					if (bExportIDs || bHasCurves)
					{
						type = bPolygon ? com.epl.geometry.ShapeType.ShapeGeneralPolygon : com.epl.geometry.ShapeType.ShapeGeneralPolyline;
						type |= com.epl.geometry.ShapeModifiers.ShapeHasZs;
						if (bExportIDs)
						{
							type |= com.epl.geometry.ShapeModifiers.ShapeHasIDs;
						}
						if (bHasCurves)
						{
							type |= com.epl.geometry.ShapeModifiers.ShapeHasCurves;
						}
					}
					else
					{
						type = bPolygon ? com.epl.geometry.ShapeType.ShapePolygonZ : com.epl.geometry.ShapeType.ShapePolylineZ;
					}
				}
				else
				{
					if (bExportMs && !bExportZs)
					{
						if (bExportIDs || bHasCurves)
						{
							type = bPolygon ? com.epl.geometry.ShapeType.ShapeGeneralPolygon : com.epl.geometry.ShapeType.ShapeGeneralPolyline;
							type |= com.epl.geometry.ShapeModifiers.ShapeHasMs;
							if (bExportIDs)
							{
								type |= com.epl.geometry.ShapeModifiers.ShapeHasIDs;
							}
							if (bHasCurves)
							{
								type |= com.epl.geometry.ShapeModifiers.ShapeHasCurves;
							}
						}
						else
						{
							type = bPolygon ? com.epl.geometry.ShapeType.ShapePolygonM : com.epl.geometry.ShapeType.ShapePolylineM;
						}
					}
					else
					{
						if (bExportIDs || bHasCurves)
						{
							type = bPolygon ? com.epl.geometry.ShapeType.ShapeGeneralPolygon : com.epl.geometry.ShapeType.ShapeGeneralPolyline;
							type |= com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasMs;
							if (bExportIDs)
							{
								type |= com.epl.geometry.ShapeModifiers.ShapeHasIDs;
							}
							if (bHasCurves)
							{
								type |= com.epl.geometry.ShapeModifiers.ShapeHasCurves;
							}
						}
						else
						{
							type = bPolygon ? com.epl.geometry.ShapeType.ShapePolygonZM : com.epl.geometry.ShapeType.ShapePolylineZM;
						}
					}
				}
			}
			// write type
			shapeBuffer.Write(type);
			offset += 4;
			// write Envelope
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			multipathImpl.QueryEnvelope2D(env);
			// calls _VerifyAllStreams
			shapeBuffer.Write(env.xmin);
			offset += 8;
			shapeBuffer.Write(env.ymin);
			offset += 8;
			shapeBuffer.Write(env.xmax);
			offset += 8;
			shapeBuffer.Write(env.ymax);
			offset += 8;
			// write part count
			shapeBuffer.Write(partCount);
			offset += 4;
			// to-do: return error if larger than 2^32 - 1
			// write pointCount
			shapeBuffer.Write(pointCount);
			offset += 4;
			// write start indices for each part
			int pointIndexDelta = 0;
			for (int ipart = 0; ipart < partCount; ipart++)
			{
				int istart = multipathImpl.GetPathStart(ipart) + pointIndexDelta;
				shapeBuffer.Write(istart);
				offset += 4;
				if (bPolygon || multipathImpl.IsClosedPath(ipart))
				{
					pointIndexDelta++;
				}
			}
			if (pointCount > 0)
			{
				// write xy coordinates
				com.epl.geometry.AttributeStreamBase positionStream = multipathImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
				com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)positionStream;
				for (int ipart = 0; ipart < partCount; ipart++)
				{
					int partStart = multipathImpl.GetPathStart(ipart);
					int partEnd = multipathImpl.GetPathEnd(ipart);
					for (int i = partStart; i < partEnd; i++)
					{
						double x = position.Read(2 * i);
						double y = position.Read(2 * i + 1);
						shapeBuffer.Write(x);
						offset += 8;
						shapeBuffer.Write(y);
						offset += 8;
					}
					// If the part is closed, then we need to duplicate the start
					// point
					if (bPolygon || multipathImpl.IsClosedPath(ipart))
					{
						double x = position.Read(2 * partStart);
						double y = position.Read(2 * partStart + 1);
						shapeBuffer.Write(x);
						offset += 8;
						shapeBuffer.Write(y);
						offset += 8;
					}
				}
			}
			// write Zs
			if (bExportZs)
			{
				com.epl.geometry.Envelope1D zInterval = multipathImpl.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(zInterval.vmin) : zInterval.vmin);
				offset += 8;
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(zInterval.vmax) : zInterval.vmax);
				offset += 8;
				if (pointCount > 0)
				{
					if (multipathImpl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.Z))
					{
						com.epl.geometry.AttributeStreamOfDbl zs = (com.epl.geometry.AttributeStreamOfDbl)multipathImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
						for (int ipart = 0; ipart < partCount; ipart++)
						{
							int partStart = multipathImpl.GetPathStart(ipart);
							int partEnd = multipathImpl.GetPathEnd(ipart);
							for (int i = partStart; i < partEnd; i++)
							{
								double z = zs.Read(i);
								shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(z) : z);
								offset += 8;
							}
							// If the part is closed, then we need to duplicate the
							// start z
							if (bPolygon || multipathImpl.IsClosedPath(ipart))
							{
								double z = zs.Read(partStart);
								shapeBuffer.Write(z);
								offset += 8;
							}
						}
					}
					else
					{
						double z = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z);
						if (bArcViewNaNs)
						{
							z = com.epl.geometry.Interop.TranslateToAVNaN(z);
						}
						for (int i = 0; i < pointCount; i++)
						{
							shapeBuffer.Write(z);
						}
						offset += 8;
					}
				}
			}
			// write Ms
			if (bExportMs)
			{
				com.epl.geometry.Envelope1D mInterval = multipathImpl.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(mInterval.vmin) : mInterval.vmin);
				offset += 8;
				shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(mInterval.vmax) : mInterval.vmax);
				offset += 8;
				if (pointCount > 0)
				{
					if (multipathImpl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.M))
					{
						com.epl.geometry.AttributeStreamOfDbl ms = (com.epl.geometry.AttributeStreamOfDbl)multipathImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
						for (int ipart = 0; ipart < partCount; ipart++)
						{
							int partStart = multipathImpl.GetPathStart(ipart);
							int partEnd = multipathImpl.GetPathEnd(ipart);
							for (int i = partStart; i < partEnd; i++)
							{
								double m = ms.Read(i);
								shapeBuffer.Write(bArcViewNaNs ? com.epl.geometry.Interop.TranslateToAVNaN(m) : m);
								offset += 8;
							}
							// If the part is closed, then we need to duplicate the
							// start m
							if (bPolygon || multipathImpl.IsClosedPath(ipart))
							{
								double m = ms.Read(partStart);
								shapeBuffer.Write(m);
								offset += 8;
							}
						}
					}
					else
					{
						double m = com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M);
						if (bArcViewNaNs)
						{
							m = com.epl.geometry.Interop.TranslateToAVNaN(m);
						}
						for (int i = 0; i < pointCount; i++)
						{
							shapeBuffer.Write(m);
						}
						offset += 8;
					}
				}
			}
			// write Curves
			if (bHasCurves)
			{
			}
			// to-do: We'll finish this later
			// write IDs
			if (bExportIDs)
			{
				if (pointCount > 0)
				{
					if (multipathImpl._attributeStreamIsAllocated(com.epl.geometry.VertexDescription.Semantics.ID))
					{
						com.epl.geometry.AttributeStreamOfInt32 ids = (com.epl.geometry.AttributeStreamOfInt32)multipathImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.ID);
						for (int ipart = 0; ipart < partCount; ipart++)
						{
							int partStart = multipathImpl.GetPathStart(ipart);
							int partEnd = multipathImpl.GetPathEnd(ipart);
							for (int i = partStart; i < partEnd; i++)
							{
								int id = ids.Read(i);
								shapeBuffer.Write(id);
								offset += 4;
							}
							// If the part is closed, then we need to duplicate the
							// start id
							if (bPolygon || multipathImpl.IsClosedPath(ipart))
							{
								int id = ids.Read(partStart);
								shapeBuffer.Write(id);
								offset += 4;
							}
						}
					}
					else
					{
						int id = (int)com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.ID);
						for (int i = 0; i < pointCount; i++)
						{
							shapeBuffer.Write(id);
						}
						offset += 4;
					}
				}
			}
			return offset;
		}
	}
}
