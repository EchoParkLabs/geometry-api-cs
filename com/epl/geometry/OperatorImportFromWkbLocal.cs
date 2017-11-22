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
	/// <summary>OperatorImportFromWkbLocal implementation.</summary>
	internal class OperatorImportFromWkbLocal : com.epl.geometry.OperatorImportFromWkb
	{
		internal sealed class WkbHelper
		{
			internal WkbHelper(System.IO.MemoryStream buffer)
			{
				wkbBuffer = buffer;
				adjustment = 0;
			}

			internal int GetInt(int offset)
			{
				return wkbBuffer.GetInt(adjustment + offset);
			}

			internal double GetDouble(int offset)
			{
				return wkbBuffer.GetDouble(adjustment + offset);
			}

			internal System.IO.MemoryStream wkbBuffer;

			internal int adjustment;
		}

		public override com.epl.geometry.Geometry Execute(int importFlags, com.epl.geometry.Geometry.Type type, System.IO.MemoryStream wkbBuffer, com.epl.geometry.ProgressTracker progress_tracker)
		{
			java.nio.ByteOrder initialOrder = wkbBuffer.Order();
			// read byte ordering
			int byteOrder = wkbBuffer.Get(0);
			if (byteOrder == com.epl.geometry.WkbByteOrder.wkbNDR)
			{
				wkbBuffer.Order(java.nio.ByteOrder.LITTLE_ENDIAN);
			}
			else
			{
				wkbBuffer.Order(java.nio.ByteOrder.BIG_ENDIAN);
			}
			com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper = new com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper(wkbBuffer);
			try
			{
				return ImportFromWkb(importFlags, type, wkbHelper);
			}
			finally
			{
				wkbBuffer.Order(initialOrder);
			}
		}

		public override com.epl.geometry.OGCStructure ExecuteOGC(int importFlags, System.IO.MemoryStream wkbBuffer, com.epl.geometry.ProgressTracker progress_tracker)
		{
			java.nio.ByteOrder initialOrder = wkbBuffer.Order();
			// read byte ordering
			int byteOrder = wkbBuffer.Get(0);
			if (byteOrder == com.epl.geometry.WkbByteOrder.wkbNDR)
			{
				wkbBuffer.Order(java.nio.ByteOrder.LITTLE_ENDIAN);
			}
			else
			{
				wkbBuffer.Order(java.nio.ByteOrder.BIG_ENDIAN);
			}
			System.Collections.Generic.List<com.epl.geometry.OGCStructure> stack = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			com.epl.geometry.AttributeStreamOfInt32 numGeometries = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 indices = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper = new com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper(wkbBuffer);
			com.epl.geometry.OGCStructure root = new com.epl.geometry.OGCStructure();
			root.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			stack.Add(root);
			// add dummy root
			numGeometries.Add(1);
			indices.Add(0);
			bool bCheckConsistentAttributes = false;
			bool bHasZs = false;
			bool bHasMs = false;
			try
			{
				while (!stack.IsEmpty())
				{
					if (indices.GetLast() == numGeometries.GetLast())
					{
						stack.Remove(stack.Count - 1);
						indices.RemoveLast();
						numGeometries.RemoveLast();
						continue;
					}
					com.epl.geometry.OGCStructure last = stack[stack.Count - 1];
					indices.Write(indices.Size() - 1, indices.GetLast() + 1);
					com.epl.geometry.Geometry geometry;
					int wkbType = wkbHelper.GetInt(1);
					int ogcType;
					// strip away attributes from type identifier
					if (wkbType > 3000)
					{
						ogcType = wkbType - 3000;
						if (bCheckConsistentAttributes)
						{
							if (!bHasZs || !bHasMs)
							{
								throw new System.ArgumentException();
							}
						}
						else
						{
							bHasZs = true;
							bHasMs = true;
							bCheckConsistentAttributes = true;
						}
					}
					else
					{
						if (wkbType > 2000)
						{
							ogcType = wkbType - 2000;
							if (bCheckConsistentAttributes)
							{
								if (bHasZs || !bHasMs)
								{
									throw new System.ArgumentException();
								}
							}
							else
							{
								bHasZs = false;
								bHasMs = true;
								bCheckConsistentAttributes = true;
							}
						}
						else
						{
							if (wkbType > 1000)
							{
								ogcType = wkbType - 1000;
								if (bCheckConsistentAttributes)
								{
									if (!bHasZs || bHasMs)
									{
										throw new System.ArgumentException();
									}
								}
								else
								{
									bHasZs = true;
									bHasMs = false;
									bCheckConsistentAttributes = true;
								}
							}
							else
							{
								ogcType = wkbType;
								if (bCheckConsistentAttributes)
								{
									if (bHasZs || bHasMs)
									{
										throw new System.ArgumentException();
									}
								}
								else
								{
									bHasZs = false;
									bHasMs = false;
									bCheckConsistentAttributes = true;
								}
							}
						}
					}
					if (ogcType == 7)
					{
						int count = wkbHelper.GetInt(5);
						wkbHelper.adjustment += 9;
						com.epl.geometry.OGCStructure next = new com.epl.geometry.OGCStructure();
						next.m_type = ogcType;
						next.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
						last.m_structures.Add(next);
						stack.Add(next);
						indices.Add(0);
						numGeometries.Add(count);
					}
					else
					{
						geometry = ImportFromWkb(importFlags, com.epl.geometry.Geometry.Type.Unknown, wkbHelper);
						com.epl.geometry.OGCStructure leaf = new com.epl.geometry.OGCStructure();
						leaf.m_type = ogcType;
						leaf.m_geometry = geometry;
						last.m_structures.Add(leaf);
					}
				}
			}
			finally
			{
				wkbBuffer.Order(initialOrder);
			}
			return root;
		}

		private static com.epl.geometry.Geometry ImportFromWkb(int importFlags, com.epl.geometry.Geometry.Type type, com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper)
		{
			// read type
			int wkbType = wkbHelper.GetInt(1);
			switch (wkbType)
			{
				case com.epl.geometry.WkbGeometryType.wkbPolygon:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(false, importFlags, false, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPolygonM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(false, importFlags, false, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPolygonZ:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(false, importFlags, true, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPolygonZM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(false, importFlags, true, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPolygon:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(true, importFlags, false, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPolygonM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(true, importFlags, false, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPolygonZ:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(true, importFlags, true, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polygon && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolygon(true, importFlags, true, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbLineString:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(false, importFlags, false, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbLineStringM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(false, importFlags, false, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbLineStringZ:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(false, importFlags, true, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbLineStringZM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(false, importFlags, true, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiLineString:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(true, importFlags, false, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiLineStringM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(true, importFlags, false, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiLineStringZ:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(true, importFlags, true, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiLineStringZM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Polyline && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPolyline(true, importFlags, true, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPoint:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.MultiPoint && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbMultiPoint(importFlags, false, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPointM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.MultiPoint && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbMultiPoint(importFlags, false, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPointZ:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.MultiPoint && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbMultiPoint(importFlags, true, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbMultiPointZM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.MultiPoint && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbMultiPoint(importFlags, true, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPoint:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Point && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPoint(importFlags, false, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPointM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Point && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPoint(importFlags, false, true, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPointZ:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Point && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPoint(importFlags, true, false, wkbHelper);
				}

				case com.epl.geometry.WkbGeometryType.wkbPointZM:
				{
					if (type.Value() != com.epl.geometry.Geometry.GeometryType.Point && type.Value() != com.epl.geometry.Geometry.GeometryType.Unknown)
					{
						throw new com.epl.geometry.GeometryException("invalid shape type");
					}
					return ImportFromWkbPoint(importFlags, true, true, wkbHelper);
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("invalid shape type");
				}
			}
		}

		private static com.epl.geometry.Geometry ImportFromWkbPolygon(bool bMultiPolygon, int importFlags, bool bZs, bool bMs, com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper)
		{
			int offset;
			int polygonCount;
			if (bMultiPolygon)
			{
				polygonCount = wkbHelper.GetInt(5);
				offset = 9;
			}
			else
			{
				polygonCount = 1;
				offset = 0;
			}
			// Find total point count and part count
			int point_count = 0;
			int partCount = 0;
			int tempOffset = offset;
			for (int ipolygon = 0; ipolygon < polygonCount; ipolygon++)
			{
				tempOffset += 5;
				// skip redundant byte order and type fields
				int ipartcount = wkbHelper.GetInt(tempOffset);
				tempOffset += 4;
				for (int ipart = 0; ipart < ipartcount; ipart++)
				{
					int ipointcount = wkbHelper.GetInt(tempOffset);
					tempOffset += 4;
					// If ipointcount == 0, then we have an empty part
					if (ipointcount == 0)
					{
						continue;
					}
					if (ipointcount <= 2)
					{
						tempOffset += ipointcount * 2 * 8;
						if (bZs)
						{
							tempOffset += ipointcount * 8;
						}
						if (bMs)
						{
							tempOffset += ipointcount * 8;
						}
						if (ipointcount == 1)
						{
							point_count += ipointcount + 1;
						}
						else
						{
							point_count += ipointcount;
						}
						partCount++;
						continue;
					}
					double startx = wkbHelper.GetDouble(tempOffset);
					tempOffset += 8;
					double starty = wkbHelper.GetDouble(tempOffset);
					tempOffset += 8;
					double startz = com.epl.geometry.NumberUtils.TheNaN;
					double startm = com.epl.geometry.NumberUtils.TheNaN;
					if (bZs)
					{
						startz = wkbHelper.GetDouble(tempOffset);
						tempOffset += 8;
					}
					if (bMs)
					{
						startm = wkbHelper.GetDouble(tempOffset);
						tempOffset += 8;
					}
					tempOffset += (ipointcount - 2) * 2 * 8;
					if (bZs)
					{
						tempOffset += (ipointcount - 2) * 8;
					}
					if (bMs)
					{
						tempOffset += (ipointcount - 2) * 8;
					}
					double endx = wkbHelper.GetDouble(tempOffset);
					tempOffset += 8;
					double endy = wkbHelper.GetDouble(tempOffset);
					tempOffset += 8;
					double endz = com.epl.geometry.NumberUtils.TheNaN;
					double endm = com.epl.geometry.NumberUtils.TheNaN;
					if (bZs)
					{
						endz = wkbHelper.GetDouble(tempOffset);
						tempOffset += 8;
					}
					if (bMs)
					{
						endm = wkbHelper.GetDouble(tempOffset);
						tempOffset += 8;
					}
					if ((startx == endx || (com.epl.geometry.NumberUtils.IsNaN(startx) && com.epl.geometry.NumberUtils.IsNaN(endx))) && (starty == endy || (com.epl.geometry.NumberUtils.IsNaN(starty) && com.epl.geometry.NumberUtils.IsNaN(endy))) && (!bZs || startz == endz || (com.epl.geometry.NumberUtils
						.IsNaN(startz) && com.epl.geometry.NumberUtils.IsNaN(endz))) && (!bMs || startm == endm || (com.epl.geometry.NumberUtils.IsNaN(startm) && com.epl.geometry.NumberUtils.IsNaN(endm))))
					{
						point_count += ipointcount - 1;
					}
					else
					{
						point_count += ipointcount;
					}
					partCount++;
				}
			}
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt32 parts = null;
			com.epl.geometry.AttributeStreamOfInt8 pathFlags = null;
			com.epl.geometry.Geometry newPolygon;
			com.epl.geometry.MultiPathImpl polygon;
			newPolygon = new com.epl.geometry.Polygon();
			polygon = (com.epl.geometry.MultiPathImpl)newPolygon._getImpl();
			if (bZs)
			{
				polygon.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			if (bMs)
			{
				polygon.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			}
			if (point_count > 0)
			{
				parts = (com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(partCount + 1, 0));
				pathFlags = (com.epl.geometry.AttributeStreamOfInt8)(com.epl.geometry.AttributeStreamBase.CreateByteStream(parts.Size(), unchecked((byte)com.epl.geometry.PathFlags.enumClosed)));
				position = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.POSITION, point_count));
				if (bZs)
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.Z, point_count));
				}
				if (bMs)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.M, point_count));
				}
			}
			bool bCreateMs = false;
			bool bCreateZs = false;
			int ipartend = 0;
			int ipolygonend = 0;
			int part_index = 0;
			// read Coordinates
			for (int ipolygon_1 = 0; ipolygon_1 < polygonCount; ipolygon_1++)
			{
				offset += 5;
				// skip redundant byte order and type fields
				int ipartcount = wkbHelper.GetInt(offset);
				offset += 4;
				int ipolygonstart = ipolygonend;
				ipolygonend = ipolygonstart + ipartcount;
				for (int ipart = ipolygonstart; ipart < ipolygonend; ipart++)
				{
					int ipointcount = wkbHelper.GetInt(offset);
					offset += 4;
					if (ipointcount == 0)
					{
						continue;
					}
					int ipartstart = ipartend;
					ipartend += ipointcount;
					bool bSkipLastPoint = true;
					if (ipointcount == 1)
					{
						ipartstart++;
						ipartend++;
						bSkipLastPoint = false;
					}
					else
					{
						if (ipointcount == 2)
						{
							bSkipLastPoint = false;
						}
						else
						{
							// Check if start point is equal to end point
							tempOffset = offset;
							double startx = wkbHelper.GetDouble(tempOffset);
							tempOffset += 8;
							double starty = wkbHelper.GetDouble(tempOffset);
							tempOffset += 8;
							double startz = com.epl.geometry.NumberUtils.TheNaN;
							double startm = com.epl.geometry.NumberUtils.TheNaN;
							if (bZs)
							{
								startz = wkbHelper.GetDouble(tempOffset);
								tempOffset += 8;
							}
							if (bMs)
							{
								startm = wkbHelper.GetDouble(tempOffset);
								tempOffset += 8;
							}
							tempOffset += (ipointcount - 2) * 2 * 8;
							if (bZs)
							{
								tempOffset += (ipointcount - 2) * 8;
							}
							if (bMs)
							{
								tempOffset += (ipointcount - 2) * 8;
							}
							double endx = wkbHelper.GetDouble(tempOffset);
							tempOffset += 8;
							double endy = wkbHelper.GetDouble(tempOffset);
							tempOffset += 8;
							double endz = com.epl.geometry.NumberUtils.TheNaN;
							double endm = com.epl.geometry.NumberUtils.TheNaN;
							if (bZs)
							{
								endz = wkbHelper.GetDouble(tempOffset);
								tempOffset += 8;
							}
							if (bMs)
							{
								endm = wkbHelper.GetDouble(tempOffset);
								tempOffset += 8;
							}
							if ((startx == endx || (com.epl.geometry.NumberUtils.IsNaN(startx) && com.epl.geometry.NumberUtils.IsNaN(endx))) && (starty == endy || (com.epl.geometry.NumberUtils.IsNaN(starty) && com.epl.geometry.NumberUtils.IsNaN(endy))) && (!bZs || startz == endz || (com.epl.geometry.NumberUtils
								.IsNaN(startz) && com.epl.geometry.NumberUtils.IsNaN(endz))) && (!bMs || startm == endm || (com.epl.geometry.NumberUtils.IsNaN(startm) && com.epl.geometry.NumberUtils.IsNaN(endm))))
							{
								ipartend--;
							}
							else
							{
								bSkipLastPoint = false;
							}
						}
					}
					if (ipart == ipolygonstart)
					{
						pathFlags.SetBits(ipart, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
					}
					parts.Write(++part_index, ipartend);
					// We must write from the buffer backwards - ogc polygon
					// format is opposite of shapefile format
					for (int i = ipartstart; i < ipartend; i++)
					{
						double x = wkbHelper.GetDouble(offset);
						offset += 8;
						double y = wkbHelper.GetDouble(offset);
						offset += 8;
						position.Write(2 * i, x);
						position.Write(2 * i + 1, y);
						if (bZs)
						{
							double z = wkbHelper.GetDouble(offset);
							offset += 8;
							zs.Write(i, z);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, z))
							{
								bCreateZs = true;
							}
						}
						if (bMs)
						{
							double m = wkbHelper.GetDouble(offset);
							offset += 8;
							ms.Write(i, m);
							if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, m))
							{
								bCreateMs = true;
							}
						}
					}
					if (bSkipLastPoint)
					{
						offset += 2 * 8;
						if (bZs)
						{
							offset += 8;
						}
						if (bMs)
						{
							offset += 8;
						}
					}
					else
					{
						if (ipointcount == 1)
						{
							double x = position.Read(2 * ipartstart);
							double y = position.Read(2 * ipartstart + 1);
							position.Write(2 * (ipartstart - 1), x);
							position.Write(2 * (ipartstart - 1) + 1, y);
							if (bZs)
							{
								double z = zs.Read(ipartstart);
								zs.Write(ipartstart - 1, z);
							}
							if (bMs)
							{
								double m = ms.Read(ipartstart);
								ms.Write(ipartstart - 1, m);
							}
						}
					}
				}
			}
			// set envelopes and assign AttributeStreams
			if (point_count > 0)
			{
				polygon.SetPathStreamRef(parts);
				// sets m_parts
				polygon.SetPathFlagsStreamRef(pathFlags);
				polygon.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				if (bZs)
				{
					if (!bCreateZs)
					{
						zs = null;
					}
					polygon.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (bMs)
				{
					if (!bCreateMs)
					{
						ms = null;
					}
					polygon.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				polygon.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				com.epl.geometry.AttributeStreamOfInt8 path_flags_clone = new com.epl.geometry.AttributeStreamOfInt8(pathFlags);
				for (int i = 0; i < path_flags_clone.Size() - 1; i++)
				{
					if (((int)path_flags_clone.Read(i) & (int)com.epl.geometry.PathFlags.enumOGCStartPolygon) != 0)
					{
						// Should
						// be
						// clockwise
						if (!com.epl.geometry.InternalUtils.IsClockwiseRing(polygon, i))
						{
							polygon.ReversePath(i);
						}
					}
					else
					{
						// make clockwise
						// Should be counter-clockwise
						if (com.epl.geometry.InternalUtils.IsClockwiseRing(polygon, i))
						{
							polygon.ReversePath(i);
						}
					}
				}
				// make counter-clockwise
				polygon.SetPathFlagsStreamRef(path_flags_clone);
			}
			if ((importFlags & (int)com.epl.geometry.WkbImportFlags.wkbImportNonTrusted) == 0)
			{
				polygon.SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, 0.0, false);
			}
			polygon.SetDirtyOGCFlags(false);
			wkbHelper.adjustment += offset;
			return newPolygon;
		}

		private static com.epl.geometry.Geometry ImportFromWkbPolyline(bool bMultiPolyline, int importFlags, bool bZs, bool bMs, com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper)
		{
			int offset;
			int originalPartCount;
			if (bMultiPolyline)
			{
				originalPartCount = wkbHelper.GetInt(5);
				offset = 9;
			}
			else
			{
				originalPartCount = 1;
				offset = 0;
			}
			// Find total point count and part count
			int point_count = 0;
			int partCount = 0;
			int tempOffset = offset;
			for (int ipart = 0; ipart < originalPartCount; ipart++)
			{
				tempOffset += 5;
				// skip redundant byte order and type fields
				int ipointcount = wkbHelper.GetInt(tempOffset);
				tempOffset += 4;
				// If ipointcount == 0, then we have an empty part
				if (ipointcount == 0)
				{
					continue;
				}
				point_count += ipointcount;
				partCount++;
				if (ipointcount == 1)
				{
					point_count++;
				}
				tempOffset += ipointcount * 2 * 8;
				if (bZs)
				{
					tempOffset += ipointcount * 8;
				}
				if (bMs)
				{
					tempOffset += ipointcount * 8;
				}
			}
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfInt32 parts = null;
			com.epl.geometry.AttributeStreamOfInt8 pathFlags = null;
			com.epl.geometry.Polyline newpolyline;
			com.epl.geometry.MultiPathImpl polyline;
			newpolyline = new com.epl.geometry.Polyline();
			polyline = (com.epl.geometry.MultiPathImpl)newpolyline._getImpl();
			if (bZs)
			{
				polyline.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			if (bMs)
			{
				polyline.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			}
			if (point_count > 0)
			{
				parts = (com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(partCount + 1, 0));
				pathFlags = (com.epl.geometry.AttributeStreamOfInt8)(com.epl.geometry.AttributeStreamBase.CreateByteStream(parts.Size(), unchecked((byte)0)));
				position = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.POSITION, point_count));
				if (bZs)
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.Z, point_count));
				}
				if (bMs)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.M, point_count));
				}
			}
			bool bCreateMs = false;
			bool bCreateZs = false;
			int ipartend = 0;
			int part_index = 0;
			// read Coordinates
			for (int ipart_1 = 0; ipart_1 < originalPartCount; ipart_1++)
			{
				offset += 5;
				// skip redundant byte order and type fields
				int ipointcount = wkbHelper.GetInt(offset);
				offset += 4;
				if (ipointcount == 0)
				{
					continue;
				}
				int ipartstart = ipartend;
				ipartend = ipartstart + ipointcount;
				if (ipointcount == 1)
				{
					ipartstart++;
					ipartend++;
				}
				parts.Write(++part_index, ipartend);
				for (int i = ipartstart; i < ipartend; i++)
				{
					double x = wkbHelper.GetDouble(offset);
					offset += 8;
					double y = wkbHelper.GetDouble(offset);
					offset += 8;
					position.Write(2 * i, x);
					position.Write(2 * i + 1, y);
					if (bZs)
					{
						double z = wkbHelper.GetDouble(offset);
						offset += 8;
						zs.Write(i, z);
						if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, z))
						{
							bCreateZs = true;
						}
					}
					if (bMs)
					{
						double m = wkbHelper.GetDouble(offset);
						offset += 8;
						ms.Write(i, m);
						if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, m))
						{
							bCreateMs = true;
						}
					}
				}
				if (ipointcount == 1)
				{
					double x = position.Read(2 * ipartstart);
					double y = position.Read(2 * ipartstart + 1);
					position.Write(2 * (ipartstart - 1), x);
					position.Write(2 * (ipartstart - 1) + 1, y);
					if (bZs)
					{
						double z = zs.Read(ipartstart);
						zs.Write(ipartstart - 1, z);
					}
					if (bMs)
					{
						double m = ms.Read(ipartstart);
						ms.Write(ipartstart - 1, m);
					}
				}
			}
			// set envelopes and assign AttributeStreams
			if (point_count > 0)
			{
				polyline.SetPathStreamRef(parts);
				// sets m_parts
				polyline.SetPathFlagsStreamRef(pathFlags);
				polyline.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				if (bZs)
				{
					if (!bCreateZs)
					{
						zs = null;
					}
					polyline.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (bMs)
				{
					if (!bCreateMs)
					{
						ms = null;
					}
					polyline.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				polyline.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
			}
			wkbHelper.adjustment += offset;
			return newpolyline;
		}

		private static com.epl.geometry.Geometry ImportFromWkbMultiPoint(int importFlags, bool bZs, bool bMs, com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper)
		{
			int offset = 5;
			// skip byte order and type
			// set point count
			int point_count = wkbHelper.GetInt(offset);
			offset += 4;
			com.epl.geometry.AttributeStreamOfDbl position = null;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.MultiPoint newmultipoint;
			com.epl.geometry.MultiPointImpl multipoint;
			newmultipoint = new com.epl.geometry.MultiPoint();
			multipoint = (com.epl.geometry.MultiPointImpl)newmultipoint._getImpl();
			if (bZs)
			{
				multipoint.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			}
			if (bMs)
			{
				multipoint.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			}
			if (point_count > 0)
			{
				position = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.POSITION, point_count));
				if (bZs)
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.Z, point_count));
				}
				if (bMs)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateAttributeStreamWithSemantics(com.epl.geometry.VertexDescription.Semantics.M, point_count));
				}
			}
			bool bCreateMs = false;
			bool bCreateZs = false;
			for (int i = 0; i < point_count; i++)
			{
				offset += 5;
				// skip redundant byte order and type fields
				// read xy coordinates
				double x = wkbHelper.GetDouble(offset);
				offset += 8;
				double y = wkbHelper.GetDouble(offset);
				offset += 8;
				position.Write(2 * i, x);
				position.Write(2 * i + 1, y);
				if (bZs)
				{
					double z = wkbHelper.GetDouble(offset);
					offset += 8;
					zs.Write(i, z);
					if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z, z))
					{
						bCreateZs = true;
					}
				}
				if (bMs)
				{
					double m = wkbHelper.GetDouble(offset);
					offset += 8;
					ms.Write(i, m);
					if (!com.epl.geometry.VertexDescription.IsDefaultValue(com.epl.geometry.VertexDescription.Semantics.M, m))
					{
						bCreateMs = true;
					}
				}
			}
			// set envelopes and assign AttributeStreams
			if (point_count > 0)
			{
				multipoint.Resize(point_count);
				multipoint.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				if (bZs)
				{
					if (!bCreateZs)
					{
						zs = null;
					}
					multipoint.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (bMs)
				{
					if (!bCreateMs)
					{
						ms = null;
					}
					multipoint.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				multipoint.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
			}
			wkbHelper.adjustment += offset;
			return newmultipoint;
		}

		private static com.epl.geometry.Geometry ImportFromWkbPoint(int importFlags, bool bZs, bool bMs, com.epl.geometry.OperatorImportFromWkbLocal.WkbHelper wkbHelper)
		{
			int offset = 5;
			// skip byte order and type
			// set xy coordinate
			double x = wkbHelper.GetDouble(offset);
			offset += 8;
			double y = wkbHelper.GetDouble(offset);
			offset += 8;
			double z = com.epl.geometry.NumberUtils.TheNaN;
			if (bZs)
			{
				z = wkbHelper.GetDouble(offset);
				offset += 8;
			}
			double m = com.epl.geometry.NumberUtils.TheNaN;
			if (bMs)
			{
				m = wkbHelper.GetDouble(offset);
				offset += 8;
			}
			bool bEmpty = com.epl.geometry.NumberUtils.IsNaN(x);
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			if (!bEmpty)
			{
				point.SetX(x);
				point.SetY(y);
			}
			// set Z
			if (bZs)
			{
				point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				if (!bEmpty)
				{
					point.SetZ(z);
				}
			}
			// set M
			if (bMs)
			{
				point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				if (!bEmpty)
				{
					point.SetM(m);
				}
			}
			wkbHelper.adjustment += offset;
			return point;
		}
	}
}
