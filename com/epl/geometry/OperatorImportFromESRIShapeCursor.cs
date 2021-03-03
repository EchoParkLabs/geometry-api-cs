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
	internal class OperatorImportFromESRIShapeCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.ByteBufferCursor m_inputShapeBuffers;

		internal int m_importFlags;

		internal int m_type;

		internal int m_index;

		public OperatorImportFromESRIShapeCursor(int importFlags, int type, com.epl.geometry.ByteBufferCursor shapeBuffers)
		{
			m_index = -1;
			if (shapeBuffers == null)
			{
				throw new com.epl.geometry.GeometryException("invalid argument");
			}
			m_importFlags = importFlags;
			m_type = type;
			m_inputShapeBuffers = shapeBuffers;
		}

		public override com.epl.geometry.Geometry Next()
		{
			System.IO.MemoryStream shapeBuffer = m_inputShapeBuffers.Next();
			if (shapeBuffer != null)
			{
				m_index = m_inputShapeBuffers.GetByteBufferID();
				return ImportFromESRIShape(new System.IO.BinaryReader(shapeBuffer));
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		private com.epl.geometry.Geometry ImportFromESRIShape( System.IO.BinaryReader shapeBuffer)
		{
			//java.nio.ByteOrder initialOrder = shapeBuffer.Order();
			//shapeBuffer.Order(java.nio.ByteOrder.LITTLE_ENDIAN);
			try
			{
				// read type
				shapeBuffer.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);int shapetype = shapeBuffer.ReadInt32();
				// Extract general type and modifiers
				int generaltype;
				int modifiers;
				switch (shapetype & com.epl.geometry.ShapeModifiers.ShapeBasicTypeMask)
				{
					case com.epl.geometry.ShapeType.ShapePolygon:
					{
						// Polygon
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolygon;
						modifiers = 0;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolygonZM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolygon;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs | com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolygonM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolygon;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolygonZ:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolygon;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeGeneralPolygon:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolygon;
						modifiers = shapetype & com.epl.geometry.ShapeModifiers.ShapeModifierMask;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolyline:
					{
						// Polyline
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolyline;
						modifiers = 0;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolylineZM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolyline;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs | (int)com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolylineM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolyline;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePolylineZ:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolyline;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeGeneralPolyline:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPolyline;
						modifiers = shapetype & com.epl.geometry.ShapeModifiers.ShapeModifierMask;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeMultiPoint:
					{
						// MultiPoint
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint;
						modifiers = 0;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeMultiPointZM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint;
						modifiers = (int)com.epl.geometry.ShapeModifiers.ShapeHasZs | (int)com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeMultiPointM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeMultiPointZ:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeGeneralMultiPoint:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralMultiPoint;
						modifiers = shapetype & com.epl.geometry.ShapeModifiers.ShapeModifierMask;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePoint:
					{
						// Point
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPoint;
						modifiers = 0;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePointZM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPoint;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs | (int)com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePointM:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPoint;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasMs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapePointZ:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPoint;
						modifiers = com.epl.geometry.ShapeModifiers.ShapeHasZs;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeGeneralPoint:
					{
						generaltype = com.epl.geometry.ShapeType.ShapeGeneralPoint;
						modifiers = shapetype & com.epl.geometry.ShapeModifiers.ShapeModifierMask;
						break;
					}

					case com.epl.geometry.ShapeType.ShapeNull:
					{
						// Null Geometry
						return null;
					}

					default:
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
				}
				switch (generaltype)
				{
					case com.epl.geometry.ShapeType.ShapeGeneralPolygon:
					{
						if (m_type != com.epl.geometry.Geometry.GeometryType.Polygon && m_type != com.epl.geometry.Geometry.GeometryType.Unknown && m_type != com.epl.geometry.Geometry.GeometryType.Envelope)
						{
							throw new com.epl.geometry.GeometryException("invalid shape type");
						}
						return ImportFromESRIShapeMultiPath(true, modifiers, shapeBuffer);
					}

					case com.epl.geometry.ShapeType.ShapeGeneralPolyline:
					{
						if (m_type != com.epl.geometry.Geometry.GeometryType.Polyline && m_type != com.epl.geometry.Geometry.GeometryType.Unknown && m_type != com.epl.geometry.Geometry.GeometryType.Envelope)
						{
							throw new com.epl.geometry.GeometryException("invalid shape type");
						}
						return ImportFromESRIShapeMultiPath(false, modifiers, shapeBuffer);
					}

					case com.epl.geometry.ShapeType.ShapeGeneralMultiPoint:
					{
						if (m_type != com.epl.geometry.Geometry.GeometryType.MultiPoint && m_type != com.epl.geometry.Geometry.GeometryType.Unknown && m_type != com.epl.geometry.Geometry.GeometryType.Envelope)
						{
							throw new com.epl.geometry.GeometryException("invalid shape type");
						}
						return ImportFromESRIShapeMultiPoint(modifiers, shapeBuffer);
					}

					case com.epl.geometry.ShapeType.ShapeGeneralPoint:
					{
						if (m_type != com.epl.geometry.Geometry.GeometryType.Point && m_type != com.epl.geometry.Geometry.GeometryType.MultiPoint && m_type != com.epl.geometry.Geometry.GeometryType.Unknown && m_type != com.epl.geometry.Geometry.GeometryType.Envelope)
						{
							throw new com.epl.geometry.GeometryException("invalid shape type");
						}
						return ImportFromESRIShapePoint(modifiers, shapeBuffer);
					}
				}
				return null;
			}
			finally
			{
				//shapeBuffer.Order(initialOrder);
			}
		}

		private com.epl.geometry.Geometry ImportFromESRIShapeMultiPath(bool bPolygon, int modifiers, System.IO.BinaryReader shapeBuffer)
		{
			shapeBuffer.BaseStream.Seek(4, System.IO.SeekOrigin.Begin);
			int offset = 4;
			bool bZs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasZs) != 0;
			bool bMs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasMs) != 0;
			bool bIDs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasIDs) != 0;
			bool bHasAttributes = bZs || bMs || bIDs;
			bool bHasBadRings = false;
			// read Envelope
			double xmin = shapeBuffer.ReadDouble();
			offset += 8;
			double ymin = shapeBuffer.ReadDouble();
			offset += 8;
			double xmax = shapeBuffer.ReadDouble();
			offset += 8;
			double ymax = shapeBuffer.ReadDouble();
			offset += 8;
			// read part count
			int originalPartCount = shapeBuffer.ReadInt32();
			offset += 4;
			int partCount = 0;
			// read point count
			int pointCount = shapeBuffer.ReadInt32();
			offset += 4;
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt32 ids = null;
			com.epl.geometry.AttributeStreamOfInt32 parts = null;
			com.epl.geometry.AttributeStreamOfInt8 pathFlags = null;
			com.epl.geometry.Envelope bbox = null;
			com.epl.geometry.MultiPath multipath = null;
			com.epl.geometry.MultiPathImpl multipathImpl = null;
			if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
			{
				if (bPolygon)
				{
					multipath = new com.epl.geometry.Polygon();
				}
				else
				{
					multipath = new com.epl.geometry.Polyline();
				}
				multipathImpl = (com.epl.geometry.MultiPathImpl)multipath._getImpl();
				if (pointCount > 0)
				{
					bbox = new com.epl.geometry.Envelope();
					bbox.SetCoords(xmin, ymin, xmax, ymax);
					parts = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(originalPartCount + 1);
					int previstart = -1;
					int lastCount = 0;
					for (int i = 0; i < originalPartCount; i++)
					{
						int istart = shapeBuffer.ReadInt32();
						offset += 4;
						lastCount = istart;
						if (previstart > istart || istart < 0)
						{
							// check that the part
							// indices in the
							// buffer are not
							// corrupted
							throw new com.epl.geometry.GeometryException("corrupted geometry");
						}
						if (istart != previstart)
						{
							parts.Write(partCount, istart);
							previstart = istart;
							partCount++;
						}
					}
					parts.Resize(partCount + 1);
					if (pointCount < lastCount)
					{
						// check that the point count in the
						// buffer is not corrupted
						throw new com.epl.geometry.GeometryException("corrupted geometry");
					}
					parts.Write(partCount, pointCount);
					pathFlags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(parts.Size(), unchecked((byte)0));
					// Create empty position stream
					position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.POSITION, pointCount);
					int startpart = parts.Read(0);
					// read xy coordinates
					int xyindex = 0;
					for (int ipart = 0; ipart < partCount; ipart++)
					{
						int endpartActual = parts.Read(ipart + 1);
						// for polygons we read one point less, then analyze if the
						// polygon is closed.
						int endpart = (bPolygon) ? endpartActual - 1 : endpartActual;
						double startx = shapeBuffer.ReadDouble();
						offset += 8;
						double starty = shapeBuffer.ReadDouble();
						offset += 8;
						position.Write(2 * xyindex, startx);
						position.Write(2 * xyindex + 1, starty);
						xyindex++;
						for (int i_1 = startpart + 1; i_1 < endpart; i_1++)
						{
							double x = shapeBuffer.ReadDouble();
							offset += 8;
							double y = shapeBuffer.ReadDouble();
							offset += 8;
							position.Write(2 * xyindex, x);
							position.Write(2 * xyindex + 1, y);
							xyindex++;
						}
						if (endpart - startpart < 2)
						{
							// a part with only one point
							multipathImpl.SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Unknown, 0.0, false);
						}
						if (bPolygon)
						{
							// read the last point of the part to decide
							// if we need to close the polygon
							if (startpart == endpart)
							{
								// a part with only one point
								parts.Write(ipart + 1, xyindex);
							}
							else
							{
								double x = shapeBuffer.ReadDouble();
								offset += 8;
								double y = shapeBuffer.ReadDouble();
								offset += 8;
								if (x != startx || y != starty)
								{
									// bad polygon. The
									// last point is
									// not the same
									// as the last
									// one. We need
									// to add it so
									// that we do
									// not loose it.
									position.Write(2 * xyindex, x);
									position.Write(2 * xyindex + 1, y);
									xyindex++;
									multipathImpl.SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Unknown, 0.0, false);
									bHasBadRings = true;
									// write part count to indicate we need to
									// account for one extra point
									// The count will be fixed after the attributes
									// are processed. So we write negative only when
									// there are attributes.
									parts.Write(ipart + 1, bHasAttributes ? -xyindex : xyindex);
								}
								else
								{
									parts.Write(ipart + 1, xyindex);
								}
							}
							pathFlags.SetBits(ipart, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
						}
						startpart = endpartActual;
					}
					if (bZs)
					{
						bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
					}
					if (bMs)
					{
						bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					}
					if (bIDs)
					{
						bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
					}
				}
			}
			else
			{
				bbox = new com.epl.geometry.Envelope();
				if (bZs)
				{
					bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				}
				if (bMs)
				{
					bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				}
				if (bIDs)
				{
					bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
				}
				if (pointCount > 0)
				{
					bbox.SetCoords(xmin, ymin, xmax, ymax);
					shapeBuffer.BaseStream.Seek(pointCount * 16 + originalPartCount * 4, System.IO.SeekOrigin.Current);
					offset += pointCount * 16 + originalPartCount * 4;
				}
				else
				{
					return (com.epl.geometry.Geometry)bbox;
				}
			}
			// read Zs
			if (bZs)
			{
				if (pointCount > 0)
				{
					double zmin = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					double zmax = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
					env.SetCoords(zmin, zmax);
					bbox.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, env);
					if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.Z, pointCount);
						bool bCreate = false;
						int startpart = parts.Read(0);
						for (int ipart = 0; ipart < partCount; ipart++)
						{
							int endpartActual = parts.Read(ipart + 1);
							int endpart = System.Math.Abs(endpartActual);
							double startz = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
							offset += 8;
							zs.Write(startpart, startz);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, startz))
							{
								bCreate = true;
							}
							for (int i = startpart + 1; i < endpart; i++)
							{
								double z = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
								offset += 8;
								zs.Write(i, z);
								if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, z))
								{
									bCreate = true;
								}
							}
							if (bPolygon && endpartActual > 0)
							{
								shapeBuffer.BaseStream.Seek(8, System.IO.SeekOrigin.Current);
								offset += 8;
							}
							startpart = endpart;
						}
						if (!bCreate)
						{
							zs = null;
						}
					}
					else
					{
						shapeBuffer.BaseStream.Seek(pointCount * 8, System.IO.SeekOrigin.Current);
						offset += pointCount * 8;
					}
				}
				if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					multipathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
			}
			// read Ms
			if (bMs)
			{
				if (pointCount > 0)
				{
					double mmin = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					double mmax = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
					env.SetCoords(mmin, mmax);
					bbox.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, env);
					if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.M, pointCount);
						bool bCreate = false;
						int startpart = parts.Read(0);
						for (int ipart = 0; ipart < partCount; ipart++)
						{
							int endpartActual = parts.Read(ipart + 1);
							int endpart = System.Math.Abs(endpartActual);
							double startm = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
							offset += 8;
							ms.Write(startpart, startm);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, startm))
							{
								bCreate = true;
							}
							for (int i = startpart + 1; i < endpart; i++)
							{
								double m = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
								offset += 8;
								ms.Write(i, m);
								if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, m))
								{
									bCreate = true;
								}
							}
							if (bPolygon && endpartActual > 0)
							{
								shapeBuffer.BaseStream.Seek(8, System.IO.SeekOrigin.Current);
								offset += 8;
							}
							startpart = endpart;
						}
						if (!bCreate)
						{
							ms = null;
						}
					}
					else
					{
						shapeBuffer.BaseStream.Seek(pointCount * 8, System.IO.SeekOrigin.Current);
						offset += pointCount * 8;
					}
				}
				if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					multipathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
			}
			// read IDs
			if (bIDs)
			{
				if (pointCount > 0)
				{
					double idmin = com.epl.geometry.NumberUtils.DoubleMax();
					double idmax = -com.epl.geometry.NumberUtils.DoubleMax();
					if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						ids = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.ID, pointCount);
						bool bCreate = false;
						int startpart = parts.Read(0);
						for (int ipart = 0; ipart < partCount; ipart++)
						{
							int endpartActual = parts.Read(ipart + 1);
							int endpart = System.Math.Abs(endpartActual);
							int startid = shapeBuffer.ReadInt32();
							offset += 4;
							ids.Write(startpart, startid);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.ID, startid))
							{
								bCreate = true;
							}
							for (int i = startpart + 1; i < endpart; i++)
							{
								int id = shapeBuffer.ReadInt32();
								offset += 4;
								ids.Write(i, id);
								if (!bCreate && !com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.ID, id))
								{
									bCreate = true;
								}
								if (idmin > id)
								{
									idmin = id;
								}
								else
								{
									if (idmax < id)
									{
										idmax = id;
									}
								}
							}
							if (bPolygon && endpartActual > 0)
							{
								shapeBuffer.BaseStream.Seek(4, System.IO.SeekOrigin.Current);
								offset += 4;
							}
							startpart = endpart;
						}
						if (!bCreate)
						{
							ids = null;
						}
					}
					else
					{
						for (int i = 0; i < pointCount; i++)
						{
							int id = shapeBuffer.ReadInt32();
							offset += 4;
							if (idmin > id)
							{
								idmin = id;
							}
							else
							{
								if (idmax < id)
								{
									idmax = id;
								}
							}
						}
					}
					com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
					env.SetCoords(idmin, idmax);
					bbox.SetInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0, env);
				}
				if (m_type == com.epl.geometry.Geometry.GeometryType.Polygon || m_type == com.epl.geometry.Geometry.GeometryType.Polyline || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					multipathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.ID, ids);
				}
			}
			if (bHasBadRings && bHasAttributes)
			{
				// revert our hack for bad polygons
				for (int ipart = 1; ipart < partCount + 1; ipart++)
				{
					int v = parts.Read(ipart);
					if (v < 0)
					{
						parts.Write(ipart, -v);
					}
				}
			}
			if (m_type == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				return (com.epl.geometry.Geometry)bbox;
			}
			if (pointCount > 0)
			{
				multipathImpl.SetPathStreamRef(parts);
				multipathImpl.SetPathFlagsStreamRef(pathFlags);
				multipathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multipathImpl.SetEnvelope(bbox);
			}
			if ((m_importFlags & com.epl.geometry.ShapeImportFlags.ShapeImportNonTrusted) == 0)
			{
				multipathImpl.SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, 0.0, false);
			}
			// We
			// use
			// tolerance
			// of 0.
			// What
			// should
			// we
			// instead?
			return (com.epl.geometry.Geometry)multipath;
		}

		private com.epl.geometry.Geometry ImportFromESRIShapeMultiPoint(int modifiers, System.IO.BinaryReader shapeBuffer)
		{
			shapeBuffer.BaseStream.Seek(4, System.IO.SeekOrigin.Begin);
			int offset = 4;
			bool bZs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasZs) != 0;
			bool bMs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasMs) != 0;
			bool bIDs = (modifiers & modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasIDs) != 0;
			double xmin = shapeBuffer.ReadDouble();
			offset += 8;
			double ymin = shapeBuffer.ReadDouble();
			offset += 8;
			double xmax = shapeBuffer.ReadDouble();
			offset += 8;
			double ymax = shapeBuffer.ReadDouble();
			offset += 8;
			int cPoints = shapeBuffer.ReadInt32();
			offset += 4;
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt32 ids = null;
			com.epl.geometry.Envelope bbox = null;
			com.epl.geometry.MultiPoint multipoint = null;
			com.epl.geometry.MultiPointImpl multipointImpl = null;
			if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
			{
				multipoint = new com.epl.geometry.MultiPoint();
				multipointImpl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
				if (cPoints > 0)
				{
					bbox = new com.epl.geometry.Envelope();
					multipointImpl.Resize(cPoints);
					position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.POSITION, cPoints);
					for (int i = 0; i < cPoints; i++)
					{
						double x = shapeBuffer.ReadDouble();
						offset += 8;
						double y = shapeBuffer.ReadDouble();
						offset += 8;
						position.Write(2 * i, x);
						position.Write(2 * i + 1, y);
					}
					multipointImpl.Resize(cPoints);
					bbox.SetCoords(xmin, ymin, xmax, ymax);
					if (bZs)
					{
						bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
					}
					if (bMs)
					{
						bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					}
					if (bIDs)
					{
						bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
					}
				}
			}
			else
			{
				bbox = new com.epl.geometry.Envelope();
				if (bZs)
				{
					bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				}
				if (bMs)
				{
					bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				}
				if (bIDs)
				{
					bbox.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
				}
				if (cPoints > 0)
				{
					bbox.SetCoords(xmin, ymin, xmax, ymax);
					shapeBuffer.BaseStream.Seek(cPoints * 16, System.IO.SeekOrigin.Current);
					offset += cPoints * 16;
				}
				else
				{
					return (com.epl.geometry.Geometry)bbox;
				}
			}
			if (bZs)
			{
				if (cPoints > 0)
				{
					double zmin = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					double zmax = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
					env.SetCoords(zmin, zmax);
					bbox.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, env);
					if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.Z, cPoints);
						bool bCreate = false;
						for (int i = 0; i < cPoints; i++)
						{
							double value = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
							offset += 8;
							zs.Write(i, value);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, value))
							{
								bCreate = true;
							}
						}
						if (!bCreate)
						{
							zs = null;
						}
					}
					else
					{
						shapeBuffer.BaseStream.Seek(cPoints * 8, System.IO.SeekOrigin.Current);
						offset += cPoints * 8;
					}
				}
				if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
			}
			if (bMs)
			{
				if (cPoints > 0)
				{
					double mmin = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					double mmax = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
					offset += 8;
					com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
					env.SetCoords(mmin, mmax);
					bbox.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, env);
					if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.M, cPoints);
						bool bCreate = false;
						for (int i = 0; i < cPoints; i++)
						{
							double value = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
							offset += 8;
							ms.Write(i, value);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, value))
							{
								bCreate = true;
							}
						}
						if (!bCreate)
						{
							ms = null;
						}
					}
					else
					{
						shapeBuffer.BaseStream.Seek(cPoints * 8, System.IO.SeekOrigin.Current);
						offset += cPoints * 8;
					}
				}
				if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
			}
			if (bIDs)
			{
				if (cPoints > 0)
				{
					double idmin = com.epl.geometry.NumberUtils.DoubleMax();
					double idmax = -com.epl.geometry.NumberUtils.DoubleMax();
					if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						ids = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.ID, cPoints);
						bool bCreate = false;
						for (int i = 0; i < cPoints; i++)
						{
							int value = shapeBuffer.ReadInt32();
							offset += 4;
							ids.Write(i, value);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.ID, value))
							{
								bCreate = true;
							}
							if (idmin > value)
							{
								idmin = value;
							}
							else
							{
								if (idmax < value)
								{
									idmax = value;
								}
							}
						}
						if (!bCreate)
						{
							ids = null;
						}
					}
					else
					{
						for (int i = 0; i < cPoints; i++)
						{
							int id = shapeBuffer.ReadInt32();
							offset += 4;
							if (idmin > id)
							{
								idmin = id;
							}
							else
							{
								if (idmax < id)
								{
									idmax = id;
								}
							}
						}
					}
					com.epl.geometry.Envelope1D env = new com.epl.geometry.Envelope1D();
					env.SetCoords(idmin, idmax);
					bbox.SetInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0, env);
				}
				if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint || m_type == com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.ID, ids);
				}
			}
			if (m_type == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				return (com.epl.geometry.Geometry)bbox;
			}
			if (cPoints > 0)
			{
				multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multipointImpl.SetEnvelope(bbox);
			}
			return (com.epl.geometry.Geometry)multipoint;
		}

		private com.epl.geometry.Geometry ImportFromESRIShapePoint(int modifiers, System.IO.BinaryReader shapeBuffer)
		{
			shapeBuffer.BaseStream.Seek(4, System.IO.SeekOrigin.Begin);
			int offset = 4;
			bool bZs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasZs) != 0;
			bool bMs = (modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasMs) != 0;
			bool bIDs = (modifiers & modifiers & (int)com.epl.geometry.ShapeModifiers.ShapeHasIDs) != 0;
			// read XY
			double x = shapeBuffer.ReadDouble();
			offset += 8;
			double y = shapeBuffer.ReadDouble();
			offset += 8;
			bool bEmpty = com.epl.geometry.NumberUtils.IsNaN(x);
			double z = com.epl.geometry.NumberUtils.NaN();
			if (bZs)
			{
				z = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
				offset += 8;
			}
			double m = com.epl.geometry.NumberUtils.NaN();
			if (bMs)
			{
				m = com.epl.geometry.Interop.TranslateFromAVNaN(shapeBuffer.ReadDouble());
				offset += 8;
			}
			int id = -1;
			if (bIDs)
			{
				id = shapeBuffer.ReadInt32();
				offset += 4;
			}
			if (m_type == com.epl.geometry.Geometry.GeometryType.MultiPoint)
			{
				com.epl.geometry.MultiPoint newmultipoint = new com.epl.geometry.MultiPoint();
				com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)newmultipoint._getImpl();
				if (!bEmpty)
				{
					com.epl.geometry.AttributeStreamBase newPositionStream = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.POSITION, 1);
					com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)newPositionStream;
					position.Write(0, x);
					position.Write(1, y);
					multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, newPositionStream);
					multipointImpl.Resize(1);
				}
				if (bZs)
				{
					multipointImpl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
					if (!bEmpty && !com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, z))
					{
						com.epl.geometry.AttributeStreamBase newZStream = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.Z, 1);
						newZStream.WriteAsDbl(0, z);
						multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, newZStream);
					}
				}
				if (bMs)
				{
					multipointImpl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					if (!bEmpty && !com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, m))
					{
						com.epl.geometry.AttributeStreamBase newMStream = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.M, 1);
						newMStream.WriteAsDbl(0, m);
						multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, newMStream);
					}
				}
				if (bIDs)
				{
					multipointImpl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
					if (!bEmpty && !com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.ID, id))
					{
						com.epl.geometry.AttributeStreamBase newIDStream = com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.ID, 1);
						newIDStream.WriteAsInt(0, id);
						multipointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.ID, newIDStream);
					}
				}
				return (com.epl.geometry.Geometry)newmultipoint;
			}
			else
			{
				if (m_type == com.epl.geometry.Geometry.GeometryType.Envelope)
				{
					com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope();
					envelope.SetCoords(x, y, x, y);
					if (bZs)
					{
						com.epl.geometry.Envelope1D interval = new com.epl.geometry.Envelope1D();
						interval.vmin = z;
						interval.vmax = z;
						envelope.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
						envelope.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, interval);
					}
					if (bMs)
					{
						com.epl.geometry.Envelope1D interval = new com.epl.geometry.Envelope1D();
						interval.vmin = m;
						interval.vmax = m;
						envelope.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
						envelope.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, interval);
					}
					if (bIDs)
					{
						com.epl.geometry.Envelope1D interval = new com.epl.geometry.Envelope1D();
						interval.vmin = id;
						interval.vmax = id;
						envelope.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
						envelope.SetInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0, interval);
					}
					return (com.epl.geometry.Geometry)envelope;
				}
			}
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			if (!bEmpty)
			{
				point.SetX(com.epl.geometry.Interop.TranslateFromAVNaN(x));
				point.SetY(com.epl.geometry.Interop.TranslateFromAVNaN(y));
			}
			// read Z
			if (bZs)
			{
				point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				if (!bEmpty)
				{
					point.SetZ(com.epl.geometry.Interop.TranslateFromAVNaN(z));
				}
			}
			// read M
			if (bMs)
			{
				point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				if (!bEmpty)
				{
					point.SetM(com.epl.geometry.Interop.TranslateFromAVNaN(m));
				}
			}
			// read ID
			if (bIDs)
			{
				point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
				if (!bEmpty)
				{
					point.SetID(id);
				}
			}
			return (com.epl.geometry.Geometry)point;
		}
	}
}
