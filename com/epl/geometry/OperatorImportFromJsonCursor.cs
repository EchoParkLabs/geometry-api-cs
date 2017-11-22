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
	internal class OperatorImportFromJsonCursor : com.epl.geometry.MapGeometryCursor
	{
		internal com.epl.geometry.JsonParserCursor m_inputJsonParsers;

		internal int m_type;

		internal int m_index;

		public OperatorImportFromJsonCursor(int type, com.epl.geometry.JsonParserCursor jsonParsers)
		{
			m_index = -1;
			if (jsonParsers == null)
			{
				throw new System.ArgumentException();
			}
			m_type = type;
			m_inputJsonParsers = jsonParsers;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		public override com.epl.geometry.MapGeometry Next()
		{
			org.codehaus.jackson.JsonParser jsonParser;
			if ((jsonParser = m_inputJsonParsers.Next()) != null)
			{
				m_index = m_inputJsonParsers.GetID();
				return ImportFromJsonParser(m_type, new com.epl.geometry.JsonParserReader(jsonParser));
			}
			return null;
		}

		internal static com.epl.geometry.MapGeometry ImportFromJsonParser(int gt, com.epl.geometry.JsonReader parser)
		{
			com.epl.geometry.MapGeometry mp;
			try
			{
				if (!com.epl.geometry.JSONUtils.IsObjectStart(parser))
				{
					return null;
				}
				bool bFoundSpatial_reference = false;
				bool bFoundHasZ = false;
				bool bFoundHasM = false;
				bool bFoundPolygon = false;
				bool bFoundPolyline = false;
				bool bFoundMultiPoint = false;
				bool bFoundX = false;
				bool bFoundY = false;
				bool bFoundZ = false;
				bool bFoundM = false;
				bool bFoundXMin = false;
				bool bFoundYMin = false;
				bool bFoundXMax = false;
				bool bFoundYMax = false;
				bool bFoundZMin = false;
				bool bFoundZMax = false;
				bool bFoundMMin = false;
				bool bFoundMMax = false;
				double x = com.epl.geometry.NumberUtils.NaN();
				double y = com.epl.geometry.NumberUtils.NaN();
				double z = com.epl.geometry.NumberUtils.NaN();
				double m = com.epl.geometry.NumberUtils.NaN();
				double xmin = com.epl.geometry.NumberUtils.NaN();
				double ymin = com.epl.geometry.NumberUtils.NaN();
				double xmax = com.epl.geometry.NumberUtils.NaN();
				double ymax = com.epl.geometry.NumberUtils.NaN();
				double zmin = com.epl.geometry.NumberUtils.NaN();
				double zmax = com.epl.geometry.NumberUtils.NaN();
				double mmin = com.epl.geometry.NumberUtils.NaN();
				double mmax = com.epl.geometry.NumberUtils.NaN();
				bool bHasZ = false;
				bool bHasM = false;
				com.epl.geometry.AttributeStreamOfDbl @as = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
				com.epl.geometry.AttributeStreamOfDbl bs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
				com.epl.geometry.Geometry geometry = null;
				com.epl.geometry.SpatialReference spatial_reference = null;
				while (parser.NextToken() != org.codehaus.jackson.JsonToken.END_OBJECT)
				{
					string name = parser.CurrentString();
					parser.NextToken();
					if (!bFoundSpatial_reference && name.Equals("spatialReference"))
					{
						bFoundSpatial_reference = true;
						if (parser.CurrentToken() == org.codehaus.jackson.JsonToken.START_OBJECT)
						{
							spatial_reference = com.epl.geometry.SpatialReference.FromJson(parser);
						}
						else
						{
							if (parser.CurrentToken() != org.codehaus.jackson.JsonToken.VALUE_NULL)
							{
								throw new com.epl.geometry.GeometryException("failed to parse spatial reference: object or null is expected");
							}
						}
					}
					else
					{
						if (!bFoundHasZ && name.Equals("hasZ"))
						{
							bFoundHasZ = true;
							bHasZ = (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_TRUE);
						}
						else
						{
							if (!bFoundHasM && name.Equals("hasM"))
							{
								bFoundHasM = true;
								bHasM = (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_TRUE);
							}
							else
							{
								if (!bFoundPolygon && name.Equals("rings") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Polygon))
								{
									bFoundPolygon = true;
									geometry = ImportFromJsonMultiPath(true, parser, @as, bs);
									continue;
								}
								else
								{
									if (!bFoundPolyline && name.Equals("paths") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Polyline))
									{
										bFoundPolyline = true;
										geometry = ImportFromJsonMultiPath(false, parser, @as, bs);
										continue;
									}
									else
									{
										if (!bFoundMultiPoint && name.Equals("points") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.MultiPoint))
										{
											bFoundMultiPoint = true;
											geometry = ImportFromJsonMultiPoint(parser, @as, bs);
											continue;
										}
										else
										{
											if (!bFoundX && name.Equals("x") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Point))
											{
												bFoundX = true;
												x = ReadDouble(parser);
											}
											else
											{
												if (!bFoundY && name.Equals("y") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Point))
												{
													bFoundY = true;
													y = ReadDouble(parser);
												}
												else
												{
													if (!bFoundZ && name.Equals("z") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Point))
													{
														bFoundZ = true;
														z = ReadDouble(parser);
													}
													else
													{
														if (!bFoundM && name.Equals("m") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Point))
														{
															bFoundM = true;
															m = ReadDouble(parser);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					if (!bFoundXMin && name.Equals("xmin") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
					{
						bFoundXMin = true;
						xmin = ReadDouble(parser);
					}
					else
					{
						if (!bFoundYMin && name.Equals("ymin") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
						{
							bFoundYMin = true;
							ymin = ReadDouble(parser);
						}
						else
						{
							if (!bFoundMMin && name.Equals("mmin") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
							{
								bFoundMMin = true;
								mmin = ReadDouble(parser);
							}
							else
							{
								if (!bFoundZMin && name.Equals("zmin") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
								{
									bFoundZMin = true;
									zmin = ReadDouble(parser);
								}
								else
								{
									if (!bFoundXMax && name.Equals("xmax") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
									{
										bFoundXMax = true;
										xmax = ReadDouble(parser);
									}
									else
									{
										if (!bFoundYMax && name.Equals("ymax") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
										{
											bFoundYMax = true;
											ymax = ReadDouble(parser);
										}
										else
										{
											if (!bFoundMMax && name.Equals("mmax") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
											{
												bFoundMMax = true;
												mmax = ReadDouble(parser);
											}
											else
											{
												if (!bFoundZMax && name.Equals("zmax") && (gt == com.epl.geometry.Geometry.GeometryType.Unknown || gt == com.epl.geometry.Geometry.GeometryType.Envelope))
												{
													bFoundZMax = true;
													zmax = ReadDouble(parser);
												}
												else
												{
													Windup(parser);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (bFoundPolygon || bFoundPolyline || bFoundMultiPoint)
				{
					System.Diagnostics.Debug.Assert((geometry != null));
					com.epl.geometry.MultiVertexGeometryImpl mvImpl = (com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl();
					com.epl.geometry.AttributeStreamBase zs = null;
					com.epl.geometry.AttributeStreamBase ms = null;
					if (bHasZ)
					{
						geometry.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
						zs = @as;
					}
					if (bHasM)
					{
						geometry.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
						ms = !bHasZ ? @as : bs;
					}
					if (bHasZ && zs != null)
					{
						mvImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
					}
					if (bHasM && ms != null)
					{
						mvImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
					}
					mvImpl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				}
				else
				{
					if (bFoundX || bFoundY || bFoundY || bFoundZ)
					{
						if (com.epl.geometry.NumberUtils.IsNaN(y))
						{
							x = com.epl.geometry.NumberUtils.NaN();
						}
						com.epl.geometry.Point p = new com.epl.geometry.Point(x, y);
						if (bFoundZ)
						{
							p.SetZ(z);
						}
						if (bFoundM)
						{
							p.SetM(m);
						}
						geometry = p;
					}
					else
					{
						if (bFoundXMin || bFoundYMin || bFoundXMax || bFoundYMax || bFoundZMin || bFoundZMax || bFoundMMin || bFoundMMax)
						{
							if (com.epl.geometry.NumberUtils.IsNaN(ymin) || com.epl.geometry.NumberUtils.IsNaN(xmax) || com.epl.geometry.NumberUtils.IsNaN(ymax))
							{
								xmin = com.epl.geometry.NumberUtils.NaN();
							}
							com.epl.geometry.Envelope e = new com.epl.geometry.Envelope(xmin, ymin, xmax, ymax);
							if (bFoundZMin && bFoundZMax)
							{
								e.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, zmin, zmax);
							}
							if (bFoundMMin && bFoundMMax)
							{
								e.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, mmin, mmax);
							}
							geometry = e;
						}
					}
				}
				mp = new com.epl.geometry.MapGeometry(geometry, spatial_reference);
			}
			catch (System.Exception e)
			{
				e.PrintStackTrace();
				return null;
			}
			return mp;
		}

		/// <exception cref="System.Exception"/>
		public static com.epl.geometry.MapGeometry FromJsonToUnknown(com.epl.geometry.JsonReader parser)
		{
			return ImportFromJsonParser(com.epl.geometry.Geometry.GeometryType.Unknown, parser);
		}

		/// <exception cref="System.Exception"/>
		public static com.epl.geometry.MapGeometry FromJsonToEnvelope(com.epl.geometry.JsonReader parser)
		{
			return ImportFromJsonParser(com.epl.geometry.Geometry.GeometryType.Envelope, parser);
		}

		/// <exception cref="System.Exception"/>
		public static com.epl.geometry.MapGeometry FromJsonToPoint(com.epl.geometry.JsonReader parser)
		{
			return ImportFromJsonParser(com.epl.geometry.Geometry.GeometryType.Point, parser);
		}

		/// <exception cref="System.Exception"/>
		public static com.epl.geometry.MapGeometry FromJsonToPolygon(com.epl.geometry.JsonReader parser)
		{
			return ImportFromJsonParser(com.epl.geometry.Geometry.GeometryType.Polygon, parser);
		}

		/// <exception cref="System.Exception"/>
		public static com.epl.geometry.MapGeometry FromJsonToPolyline(com.epl.geometry.JsonReader parser)
		{
			return ImportFromJsonParser(com.epl.geometry.Geometry.GeometryType.Polyline, parser);
		}

		/// <exception cref="System.Exception"/>
		public static com.epl.geometry.MapGeometry FromJsonToMultiPoint(com.epl.geometry.JsonReader parser)
		{
			return ImportFromJsonParser(com.epl.geometry.Geometry.GeometryType.MultiPoint, parser);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		private static void Windup(com.epl.geometry.JsonReader parser)
		{
			parser.SkipChildren();
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		private static double ReadDouble(com.epl.geometry.JsonReader parser)
		{
			if (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_NULL || parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_STRING && parser.CurrentString().Equals("NaN"))
			{
				return com.epl.geometry.NumberUtils.NaN();
			}
			else
			{
				return parser.CurrentDoubleValue();
			}
		}

		/// <exception cref="System.Exception"/>
		private static com.epl.geometry.Geometry ImportFromJsonMultiPoint(com.epl.geometry.JsonReader parser, com.epl.geometry.AttributeStreamOfDbl @as, com.epl.geometry.AttributeStreamOfDbl bs)
		{
			if (parser.CurrentToken() != org.codehaus.jackson.JsonToken.START_ARRAY)
			{
				throw new com.epl.geometry.GeometryException("failed to parse multipoint: array of vertices is expected");
			}
			int point_count = 0;
			com.epl.geometry.MultiPoint multipoint;
			multipoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)(com.epl.geometry.AttributeStreamBase.CreateDoubleStream(2, 0));
			// At start of rings
			int sz;
			double[] buf = new double[4];
			while (parser.NextToken() != org.codehaus.jackson.JsonToken.END_ARRAY)
			{
				if (parser.CurrentToken() != org.codehaus.jackson.JsonToken.START_ARRAY)
				{
					throw new com.epl.geometry.GeometryException("failed to parse multipoint: array is expected, multipoint vertices consist of arrays of cooridinates");
				}
				sz = 0;
				while (parser.NextToken() != org.codehaus.jackson.JsonToken.END_ARRAY)
				{
					buf[sz++] = ReadDouble(parser);
				}
				if (sz < 2)
				{
					throw new com.epl.geometry.GeometryException("failed to parse multipoint: each vertex array has to have at least 2 elements");
				}
				if (position.Size() == 2 * point_count)
				{
					int c = point_count * 3;
					if (c % 2 != 0)
					{
						c++;
					}
					// have to be even
					position.Resize(c);
				}
				position.Write(2 * point_count, buf[0]);
				position.Write(2 * point_count + 1, buf[1]);
				if (@as.Size() == point_count)
				{
					int c = (point_count * 3) / 2;
					if (c < 4)
					{
						c = 4;
					}
					else
					{
						if (c < 16)
						{
							c = 16;
						}
					}
					@as.Resize(c);
				}
				if (sz > 2)
				{
					@as.Write(point_count, buf[2]);
				}
				else
				{
					@as.Write(point_count, com.epl.geometry.NumberUtils.NaN());
				}
				if (bs.Size() == point_count)
				{
					int c = (point_count * 3) / 2;
					if (c < 4)
					{
						c = 4;
					}
					else
					{
						if (c < 16)
						{
							c = 16;
						}
					}
					bs.Resize(c);
				}
				if (sz > 3)
				{
					bs.Write(point_count, buf[3]);
				}
				else
				{
					bs.Write(point_count, com.epl.geometry.NumberUtils.NaN());
				}
				point_count++;
			}
			if (point_count != 0)
			{
				com.epl.geometry.MultiPointImpl mp_impl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
				mp_impl.Resize(point_count);
				mp_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
			}
			return multipoint;
		}

		/// <exception cref="System.Exception"/>
		private static com.epl.geometry.Geometry ImportFromJsonMultiPath(bool b_polygon, com.epl.geometry.JsonReader parser, com.epl.geometry.AttributeStreamOfDbl @as, com.epl.geometry.AttributeStreamOfDbl bs)
		{
			if (parser.CurrentToken() != org.codehaus.jackson.JsonToken.START_ARRAY)
			{
				throw new com.epl.geometry.GeometryException("failed to parse multipath: array of array of vertices is expected");
			}
			com.epl.geometry.MultiPath multipath;
			if (b_polygon)
			{
				multipath = new com.epl.geometry.Polygon();
			}
			else
			{
				multipath = new com.epl.geometry.Polyline();
			}
			com.epl.geometry.AttributeStreamOfInt32 parts = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0);
			com.epl.geometry.AttributeStreamOfDbl position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(2, 0);
			com.epl.geometry.AttributeStreamOfInt8 pathFlags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(0);
			// set up min max variables
			double[] buf = new double[4];
			double[] start = new double[4];
			int point_count = 0;
			int path_count = 0;
			byte pathFlag = b_polygon ? unchecked((byte)com.epl.geometry.PathFlags.enumClosed) : 0;
			int requiredSize = b_polygon ? 3 : 2;
			// At start of rings
			while (parser.NextToken() != org.codehaus.jackson.JsonToken.END_ARRAY)
			{
				if (parser.CurrentToken() != org.codehaus.jackson.JsonToken.START_ARRAY)
				{
					throw new com.epl.geometry.GeometryException("failed to parse multipath: ring/path array is expected");
				}
				int pathPointCount = 0;
				bool b_first = true;
				int sz = 0;
				int szstart = 0;
				parser.NextToken();
				while (parser.CurrentToken() != org.codehaus.jackson.JsonToken.END_ARRAY)
				{
					if (parser.CurrentToken() != org.codehaus.jackson.JsonToken.START_ARRAY)
					{
						throw new com.epl.geometry.GeometryException("failed to parse multipath: array is expected, rings/paths vertices consist of arrays of cooridinates");
					}
					sz = 0;
					while (parser.NextToken() != org.codehaus.jackson.JsonToken.END_ARRAY)
					{
						buf[sz++] = ReadDouble(parser);
					}
					if (sz < 2)
					{
						throw new com.epl.geometry.GeometryException("failed to parse multipath: each vertex array has to have at least 2 elements");
					}
					parser.NextToken();
					do
					{
						if (position.Size() == point_count * 2)
						{
							int c = point_count * 3;
							if (c % 2 != 0)
							{
								c++;
							}
							// have to be even
							if (c < 8)
							{
								c = 8;
							}
							else
							{
								if (c < 32)
								{
									c = 32;
								}
							}
							position.Resize(c);
						}
						position.Write(2 * point_count, buf[0]);
						position.Write(2 * point_count + 1, buf[1]);
						if (@as.Size() == point_count)
						{
							int c = (point_count * 3) / 2;
							// have to be even
							if (c < 4)
							{
								c = 4;
							}
							else
							{
								if (c < 16)
								{
									c = 16;
								}
							}
							@as.Resize(c);
						}
						if (sz > 2)
						{
							@as.Write(point_count, buf[2]);
						}
						else
						{
							@as.Write(point_count, com.epl.geometry.NumberUtils.NaN());
						}
						if (bs.Size() == point_count)
						{
							int c = (point_count * 3) / 2;
							// have to be even
							if (c < 4)
							{
								c = 4;
							}
							else
							{
								if (c < 16)
								{
									c = 16;
								}
							}
							bs.Resize(c);
						}
						if (sz > 3)
						{
							bs.Write(point_count, buf[3]);
						}
						else
						{
							bs.Write(point_count, com.epl.geometry.NumberUtils.NaN());
						}
						if (b_first)
						{
							path_count++;
							parts.Add(point_count);
							pathFlags.Add(pathFlag);
							b_first = false;
							szstart = sz;
							start[0] = buf[0];
							start[1] = buf[1];
							start[2] = buf[2];
							start[3] = buf[3];
						}
						point_count++;
						pathPointCount++;
					}
					while (pathPointCount < requiredSize && parser.CurrentToken() == org.codehaus.jackson.JsonToken.END_ARRAY);
				}
				if (b_polygon && pathPointCount > requiredSize && sz == szstart && start[0] == buf[0] && start[1] == buf[1] && start[2] == buf[2] && start[3] == buf[3])
				{
					// remove the end point that is equal to the start point.
					point_count--;
					pathPointCount--;
				}
				if (pathPointCount == 0)
				{
					continue;
				}
			}
			// skip empty paths
			if (point_count != 0)
			{
				parts.Resize(path_count);
				pathFlags.Resize(path_count);
				if (point_count > 0)
				{
					parts.Add(point_count);
					pathFlags.Add(unchecked((byte)0));
				}
				com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)multipath._getImpl();
				mp_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				mp_impl.SetPathFlagsStreamRef(pathFlags);
				mp_impl.SetPathStreamRef(parts);
			}
			return multipath;
		}
	}
}
