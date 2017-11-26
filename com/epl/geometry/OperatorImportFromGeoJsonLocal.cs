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
	internal class OperatorImportFromGeoJsonLocal : com.epl.geometry.OperatorImportFromGeoJson
	{
		[System.Serializable]
		internal sealed class GeoJsonType
		{
			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType Point = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType LineString = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType Polygon = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType MultiPoint = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType MultiLineString = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType MultiPolygon = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			public static readonly com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType GeometryCollection = new com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType();

			internal static com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType FromGeoJsonValue(int v)
			{
				return com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Values()[v - 1];
			}

			public int Geogsjonvalue()
			{
				return Ordinal() + 1;
			}
		}

		internal abstract class GeoJsonValues
		{
			public const int Point = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Point.Geogsjonvalue();

			public const int LineString = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.LineString.Geogsjonvalue();

			public const int Polygon = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Polygon.Geogsjonvalue();

			public const int MultiPoint = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPoint.Geogsjonvalue();

			public const int MultiLineString = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiLineString.Geogsjonvalue();

			public const int MultiPolygon = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPolygon.Geogsjonvalue();

			public const int GeometryCollection = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.GeometryCollection.Geogsjonvalue();
		}

		internal static class GeoJsonValuesConstants
		{
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override com.epl.geometry.MapGeometry Execute(int importFlags, com.epl.geometry.Geometry.Type type, string geoJsonString, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.MapGeometry map_geometry = com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper.ImportFromGeoJson(importFlags, type, com.epl.geometry.JsonParserReader.CreateFromString(geoJsonString), progressTracker, false);
			return map_geometry;
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override com.epl.geometry.MapGeometry Execute(int importFlags, com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonReader jsonReader, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (jsonReader == null)
			{
				return null;
			}
			return com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper.ImportFromGeoJson(importFlags, type, jsonReader, progressTracker, false);
		}

		internal sealed class OperatorImportFromGeoJsonHelper
		{
			private com.epl.geometry.AttributeStreamOfDbl m_position;

			private com.epl.geometry.AttributeStreamOfDbl m_zs;

			private com.epl.geometry.AttributeStreamOfDbl m_ms;

			private com.epl.geometry.AttributeStreamOfInt32 m_paths;

			private com.epl.geometry.AttributeStreamOfInt8 m_path_flags;

			private com.epl.geometry.Point m_point;

			private bool m_b_has_zs;

			private bool m_b_has_ms;

			private bool m_b_has_zs_known;

			private bool m_b_has_ms_known;

			private int m_num_embeddings;

			internal int m_ogcType;

			internal OperatorImportFromGeoJsonHelper()
			{
				// special case for Points
				m_position = null;
				m_zs = null;
				m_ms = null;
				m_paths = null;
				m_path_flags = null;
				m_point = null;
				m_b_has_zs = false;
				m_b_has_ms = false;
				m_b_has_zs_known = false;
				m_b_has_ms_known = false;
				m_num_embeddings = 0;
				m_ogcType = 0;
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			internal static com.epl.geometry.MapGeometry ImportFromGeoJson(int importFlags, com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker, bool skip_coordinates)
			{
				com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper geo_json_helper = new com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper();
				com.epl.geometry.MapOGCStructure ms = geo_json_helper.ImportFromGeoJsonImpl(importFlags, type, json_iterator, progress_tracker, skip_coordinates, 0);
				if (geo_json_helper.m_ogcType == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonValues.GeometryCollection && !skip_coordinates)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				return new com.epl.geometry.MapGeometry(ms.m_ogcStructure.m_geometry, ms.m_spatialReference);
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			internal static com.epl.geometry.MapOGCStructure ImportFromGeoJson(int importFlags, com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker, bool skip_coordinates, int recursion)
			{
				com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper geo_json_helper = new com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper();
				com.epl.geometry.MapOGCStructure ms = geo_json_helper.ImportFromGeoJsonImpl(importFlags, type, json_iterator, progress_tracker, skip_coordinates, recursion);
				if (geo_json_helper.m_ogcType == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonValues.GeometryCollection && !skip_coordinates)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				return ms;
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			internal com.epl.geometry.MapOGCStructure ImportFromGeoJsonImpl(int importFlags, com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker, bool skip_coordinates, int recursion)
			{
				com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper geo_json_helper = this;
				bool b_type_found = false;
				bool b_coordinates_found = false;
				bool b_crs_found = false;
				bool b_crsURN_found = false;
				bool b_geometry_collection = false;
				bool b_geometries_found = false;
				com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType geo_json_type = null;
				com.epl.geometry.Geometry geometry = null;
				com.epl.geometry.SpatialReference spatial_reference = null;
				com.epl.geometry.JsonReader.Token current_token;
				string field_name = null;
				com.epl.geometry.MapOGCStructure ms = new com.epl.geometry.MapOGCStructure();
				while ((current_token = json_iterator.NextToken()) != com.epl.geometry.JsonReader.Token.END_OBJECT)
				{
					field_name = json_iterator.CurrentString();
					if (field_name.Equals("type"))
					{
						if (b_type_found)
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
						b_type_found = true;
						current_token = json_iterator.NextToken();
						if (current_token != com.epl.geometry.JsonReader.Token.VALUE_STRING)
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
						string s = json_iterator.CurrentString();
						try
						{
							geo_json_type = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.ValueOf(s);
						}
						catch (System.Exception)
						{
							throw new com.epl.geometry.JsonGeometryException(s);
						}
						if (geo_json_type == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.GeometryCollection)
						{
							if (type != com.epl.geometry.Geometry.Type.Unknown)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							b_geometry_collection = true;
						}
					}
					else
					{
						if (field_name.Equals("geometries"))
						{
							b_geometries_found = true;
							if (type != com.epl.geometry.Geometry.Type.Unknown)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							if (recursion > 10)
							{
								throw new com.epl.geometry.JsonGeometryException("deep geojson");
							}
							if (skip_coordinates)
							{
								json_iterator.SkipChildren();
							}
							else
							{
								current_token = json_iterator.NextToken();
								ms.m_ogcStructure = new com.epl.geometry.OGCStructure();
								ms.m_ogcStructure.m_type = com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonValues.GeometryCollection;
								ms.m_ogcStructure.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
								if (current_token == com.epl.geometry.JsonReader.Token.START_ARRAY)
								{
									current_token = json_iterator.NextToken();
									while (current_token != com.epl.geometry.JsonReader.Token.END_ARRAY)
									{
										com.epl.geometry.MapOGCStructure child = ImportFromGeoJson(importFlags | com.epl.geometry.GeoJsonImportFlags.geoJsonImportSkipCRS, type, json_iterator, progress_tracker, false, recursion + 1);
										ms.m_ogcStructure.m_structures.Add(child.m_ogcStructure);
										current_token = json_iterator.NextToken();
									}
								}
								else
								{
									if (current_token != com.epl.geometry.JsonReader.Token.VALUE_NULL)
									{
										throw new com.epl.geometry.JsonGeometryException("parsing error");
									}
								}
							}
						}
						else
						{
							if (field_name.Equals("coordinates"))
							{
								if (b_coordinates_found)
								{
									throw new com.epl.geometry.JsonGeometryException("parsing error");
								}
								b_coordinates_found = true;
								current_token = json_iterator.NextToken();
								if (skip_coordinates)
								{
									json_iterator.SkipChildren();
								}
								else
								{
									// According to the spec, the value of the
									// coordinates must be an array. However, I do an
									// extra check for null too.
									if (current_token != com.epl.geometry.JsonReader.Token.VALUE_NULL)
									{
										if (current_token != com.epl.geometry.JsonReader.Token.START_ARRAY)
										{
											throw new com.epl.geometry.JsonGeometryException("parsing error");
										}
										geo_json_helper.Import_coordinates_(json_iterator, progress_tracker);
									}
								}
							}
							else
							{
								if (field_name.Equals("crs"))
								{
									if (b_crs_found || b_crsURN_found)
									{
										throw new com.epl.geometry.JsonGeometryException("parsing error");
									}
									b_crs_found = true;
									current_token = json_iterator.NextToken();
									if ((importFlags & com.epl.geometry.GeoJsonImportFlags.geoJsonImportSkipCRS) == 0)
									{
										spatial_reference = ImportSpatialReferenceFromCrs(json_iterator, progress_tracker);
									}
									else
									{
										json_iterator.SkipChildren();
									}
								}
								else
								{
									if (field_name.Equals("crsURN"))
									{
										if (b_crs_found || b_crsURN_found)
										{
											throw new com.epl.geometry.JsonGeometryException("parsing error");
										}
										b_crsURN_found = true;
										current_token = json_iterator.NextToken();
										spatial_reference = ImportSpatialReferenceFromCrsUrn_(json_iterator, progress_tracker);
									}
									else
									{
										json_iterator.NextToken();
										json_iterator.SkipChildren();
									}
								}
							}
						}
					}
				}
				// According to the spec, a GeoJSON object must have both a type and
				// a coordinates array
				if (!b_type_found || (!b_geometry_collection && !b_coordinates_found && !skip_coordinates))
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				if ((!b_geometry_collection && b_geometries_found) || (b_geometry_collection && !b_geometries_found))
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				//found "geometries" but did not see "GeometryCollection"
				if (!skip_coordinates && !b_geometry_collection)
				{
					geometry = geo_json_helper.CreateGeometry_(geo_json_type, type.Value());
					ms.m_ogcStructure = new com.epl.geometry.OGCStructure();
					ms.m_ogcStructure.m_type = m_ogcType;
					ms.m_ogcStructure.m_geometry = geometry;
				}
				if (!b_crs_found && !b_crsURN_found && ((importFlags & com.epl.geometry.GeoJsonImportFlags.geoJsonImportSkipCRS) == 0) && ((importFlags & com.epl.geometry.GeoJsonImportFlags.geoJsonImportNoWGS84Default) == 0))
				{
					spatial_reference = com.epl.geometry.SpatialReference.Create(4326);
				}
				// the spec
				// gives a
				// default
				// of 4326
				// if no crs
				// is given
				ms.m_spatialReference = spatial_reference;
				return ms;
			}

			// We have to import the coordinates in the most general way possible to
			// not assume the type of geometry we're parsing.
			// JSON allows for unordered objects, so it's possible that the
			// coordinates array can come before the type tag when parsing
			// sequentially, otherwise
			// we would have to parse using a JSON_object, which would be easier,
			// but not as space/time efficient. So this function blindly imports the
			// coordinates
			// into the attribute stream(s), and will later assign them to a
			// geometry after the type tag is found.
			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private void Import_coordinates_(com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker)
			{
				System.Diagnostics.Debug.Assert((json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY));
				int coordinates_level_lower = 1;
				int coordinates_level_upper = 4;
				json_iterator.NextToken();
				while (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.END_ARRAY)
				{
					if (IsDouble_(json_iterator))
					{
						if (coordinates_level_upper > 1)
						{
							coordinates_level_upper = 1;
						}
					}
					else
					{
						if (json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY)
						{
							if (coordinates_level_lower < 2)
							{
								coordinates_level_lower = 2;
							}
						}
						else
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
					}
					if (coordinates_level_lower > coordinates_level_upper)
					{
						throw new System.ArgumentException("invalid argument");
					}
					if (coordinates_level_lower == coordinates_level_upper && coordinates_level_lower == 1)
					{
						// special
						// code
						// for
						// Points
						ReadCoordinateAsPoint_(json_iterator);
					}
					else
					{
						bool b_add_path_level_3 = true;
						bool b_polygon_start_level_4 = true;
						System.Diagnostics.Debug.Assert((json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY));
						json_iterator.NextToken();
						while (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.END_ARRAY)
						{
							if (IsDouble_(json_iterator))
							{
								if (coordinates_level_upper > 2)
								{
									coordinates_level_upper = 2;
								}
							}
							else
							{
								if (json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY)
								{
									if (coordinates_level_lower < 3)
									{
										coordinates_level_lower = 3;
									}
								}
								else
								{
									throw new com.epl.geometry.JsonGeometryException("parsing error");
								}
							}
							if (coordinates_level_lower > coordinates_level_upper)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							if (coordinates_level_lower == coordinates_level_upper && coordinates_level_lower == 2)
							{
								// LineString
								// or
								// MultiPoint
								AddCoordinate_(json_iterator);
							}
							else
							{
								bool b_add_path_level_4 = true;
								System.Diagnostics.Debug.Assert((json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY));
								json_iterator.NextToken();
								while (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.END_ARRAY)
								{
									if (IsDouble_(json_iterator))
									{
										if (coordinates_level_upper > 3)
										{
											coordinates_level_upper = 3;
										}
									}
									else
									{
										if (json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY)
										{
											if (coordinates_level_lower < 4)
											{
												coordinates_level_lower = 4;
											}
										}
										else
										{
											throw new com.epl.geometry.JsonGeometryException("parsing error");
										}
									}
									if (coordinates_level_lower > coordinates_level_upper)
									{
										throw new com.epl.geometry.JsonGeometryException("parsing error");
									}
									if (coordinates_level_lower == coordinates_level_upper && coordinates_level_lower == 3)
									{
										// Polygon
										// or
										// MultiLineString
										if (b_add_path_level_3)
										{
											AddPath_();
											b_add_path_level_3 = false;
										}
										AddCoordinate_(json_iterator);
									}
									else
									{
										System.Diagnostics.Debug.Assert((json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.START_ARRAY));
										json_iterator.NextToken();
										if (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.END_ARRAY)
										{
											if (!IsDouble_(json_iterator))
											{
												throw new com.epl.geometry.JsonGeometryException("parsing error");
											}
											System.Diagnostics.Debug.Assert((coordinates_level_lower == coordinates_level_upper && coordinates_level_lower == 4));
											// MultiPolygon
											if (b_add_path_level_4)
											{
												AddPath_();
												AddPathFlag_(b_polygon_start_level_4);
												b_add_path_level_4 = false;
												b_polygon_start_level_4 = false;
											}
											AddCoordinate_(json_iterator);
										}
										json_iterator.NextToken();
									}
								}
								json_iterator.NextToken();
							}
						}
						json_iterator.NextToken();
					}
				}
				if (m_paths != null)
				{
					m_paths.Add(m_position.Size() / 2);
				}
				// add final path size
				if (m_path_flags != null)
				{
					m_path_flags.Add(unchecked((byte)0));
				}
				// to match the paths size
				m_num_embeddings = coordinates_level_lower;
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private void ReadCoordinateAsPoint_(com.epl.geometry.JsonReader json_iterator)
			{
				System.Diagnostics.Debug.Assert((IsDouble_(json_iterator)));
				m_point = new com.epl.geometry.Point();
				double x = ReadDouble_(json_iterator);
				json_iterator.NextToken();
				double y = ReadDouble_(json_iterator);
				json_iterator.NextToken();
				if (com.epl.geometry.NumberUtils.IsNaN(y))
				{
					x = com.epl.geometry.NumberUtils.NaN();
				}
				m_point.SetXY(x, y);
				if (IsDouble_(json_iterator))
				{
					double z = ReadDouble_(json_iterator);
					json_iterator.NextToken();
					m_point.SetZ(z);
				}
				if (IsDouble_(json_iterator))
				{
					double m = ReadDouble_(json_iterator);
					json_iterator.NextToken();
					m_point.SetM(m);
				}
				if (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.END_ARRAY)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private void AddCoordinate_(com.epl.geometry.JsonReader json_iterator)
			{
				System.Diagnostics.Debug.Assert((IsDouble_(json_iterator)));
				if (m_position == null)
				{
					m_position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
				}
				double x = ReadDouble_(json_iterator);
				json_iterator.NextToken();
				double y = ReadDouble_(json_iterator);
				json_iterator.NextToken();
				int size = m_position.Size();
				m_position.Add(x);
				m_position.Add(y);
				if (IsDouble_(json_iterator))
				{
					if (!m_b_has_zs_known)
					{
						m_b_has_zs_known = true;
						m_b_has_zs = true;
						m_zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
					}
					else
					{
						if (!m_b_has_zs)
						{
							m_zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(size >> 1, com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z));
							m_b_has_zs = true;
						}
					}
					double z = ReadDouble_(json_iterator);
					json_iterator.NextToken();
					m_zs.Add(z);
				}
				else
				{
					if (!m_b_has_zs_known)
					{
						m_b_has_zs_known = true;
						m_b_has_zs = false;
					}
					else
					{
						if (m_b_has_zs)
						{
							m_zs.Add(com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.Z));
						}
					}
				}
				if (IsDouble_(json_iterator))
				{
					if (!m_b_has_ms_known)
					{
						m_b_has_ms_known = true;
						m_b_has_ms = true;
						m_ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
					}
					else
					{
						if (!m_b_has_ms)
						{
							m_ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(size >> 1, com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M));
							m_b_has_ms = true;
						}
					}
					double m = ReadDouble_(json_iterator);
					json_iterator.NextToken();
					m_ms.Add(m);
				}
				else
				{
					if (!m_b_has_ms_known)
					{
						m_b_has_ms_known = true;
						m_b_has_ms = false;
					}
					else
					{
						if (m_b_has_ms)
						{
							m_zs.Add(com.epl.geometry.VertexDescription.GetDefaultValue(com.epl.geometry.VertexDescription.Semantics.M));
						}
					}
				}
				if (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.END_ARRAY)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
			}

			private void AddPath_()
			{
				if (m_paths == null)
				{
					m_paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0);
				}
				if (m_position == null)
				{
					m_paths.Add(0);
				}
				else
				{
					m_paths.Add(m_position.Size() / 2);
				}
			}

			private void AddPathFlag_(bool b_polygon_start)
			{
				if (m_path_flags == null)
				{
					m_path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(0);
				}
				if (b_polygon_start)
				{
					m_path_flags.Add(unchecked((byte)(com.epl.geometry.PathFlags.enumClosed | com.epl.geometry.PathFlags.enumOGCStartPolygon)));
				}
				else
				{
					m_path_flags.Add(unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
				}
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private double ReadDouble_(com.epl.geometry.JsonReader json_iterator)
			{
				com.epl.geometry.JsonReader.Token current_token = json_iterator.CurrentToken();
				if (current_token == com.epl.geometry.JsonReader.Token.VALUE_NULL || (current_token == com.epl.geometry.JsonReader.Token.VALUE_STRING && json_iterator.CurrentString().Equals("NaN")))
				{
					return com.epl.geometry.NumberUtils.NaN();
				}
				else
				{
					return json_iterator.CurrentDoubleValue();
				}
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private bool IsDouble_(com.epl.geometry.JsonReader json_iterator)
			{
				com.epl.geometry.JsonReader.Token current_token = json_iterator.CurrentToken();
				if (current_token == com.epl.geometry.JsonReader.Token.VALUE_NUMBER_FLOAT)
				{
					return true;
				}
				if (current_token == com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT)
				{
					return true;
				}
				if (current_token == com.epl.geometry.JsonReader.Token.VALUE_NULL || (current_token == com.epl.geometry.JsonReader.Token.VALUE_STRING && json_iterator.CurrentString().Equals("NaN")))
				{
					return true;
				}
				return false;
			}

			//does not accept GeometryCollection
			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private com.epl.geometry.Geometry CreateGeometry_(com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType geo_json_type, int type)
			{
				com.epl.geometry.Geometry geometry;
				if (type != com.epl.geometry.Geometry.GeometryType.Unknown)
				{
					switch (type)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							if (geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPolygon && geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Polygon)
							{
								throw new com.epl.geometry.GeometryException("invalid shape type");
							}
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							if (geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiLineString && geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.LineString)
							{
								throw new com.epl.geometry.GeometryException("invalid shape type");
							}
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							if (geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPoint)
							{
								throw new com.epl.geometry.GeometryException("invalid shape type");
							}
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							if (geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Point)
							{
								throw new com.epl.geometry.GeometryException("invalid shape type");
							}
							break;
						}

						default:
						{
							throw new com.epl.geometry.GeometryException("invalid shape type");
						}
					}
				}
				m_ogcType = geo_json_type.Geogsjonvalue();
				if (geo_json_type == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.GeometryCollection)
				{
					throw new System.ArgumentException("invalid argument");
				}
				if (m_position == null && m_point == null)
				{
					switch (geo_json_type)
					{
						case com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Point:
						{
							if (m_num_embeddings > 1)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							geometry = new com.epl.geometry.Point();
							break;
						}

						case com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPoint:
						{
							if (m_num_embeddings > 2)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							geometry = new com.epl.geometry.MultiPoint();
							break;
						}

						case com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.LineString:
						{
							if (m_num_embeddings > 2)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							geometry = new com.epl.geometry.Polyline();
							break;
						}

						case com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiLineString:
						{
							if (m_num_embeddings > 3)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							geometry = new com.epl.geometry.Polyline();
							break;
						}

						case com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Polygon:
						{
							if (m_num_embeddings > 3)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							geometry = new com.epl.geometry.Polygon();
							break;
						}

						case com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPolygon:
						{
							System.Diagnostics.Debug.Assert((m_num_embeddings <= 4));
							geometry = new com.epl.geometry.Polygon();
							break;
						}

						default:
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
					}
				}
				else
				{
					if (m_num_embeddings == 1)
					{
						if (geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Point)
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
						System.Diagnostics.Debug.Assert((m_point != null));
						geometry = m_point;
					}
					else
					{
						if (m_num_embeddings == 2)
						{
							if (geo_json_type == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPoint)
							{
								geometry = CreateMultiPointFromStreams_();
							}
							else
							{
								if (geo_json_type == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.LineString)
								{
									geometry = CreatePolylineFromStreams_();
								}
								else
								{
									throw new com.epl.geometry.JsonGeometryException("parsing error");
								}
							}
						}
						else
						{
							if (m_num_embeddings == 3)
							{
								if (geo_json_type == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.Polygon)
								{
									geometry = CreatePolygonFromStreams_();
								}
								else
								{
									if (geo_json_type == com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiLineString)
									{
										geometry = CreatePolylineFromStreams_();
									}
									else
									{
										throw new com.epl.geometry.JsonGeometryException("parsing error");
									}
								}
							}
							else
							{
								if (geo_json_type != com.epl.geometry.OperatorImportFromGeoJsonLocal.GeoJsonType.MultiPolygon)
								{
									throw new com.epl.geometry.JsonGeometryException("parsing error");
								}
								geometry = CreatePolygonFromStreams_();
							}
						}
					}
				}
				return geometry;
			}

			private com.epl.geometry.Geometry CreatePolygonFromStreams_()
			{
				System.Diagnostics.Debug.Assert((m_position != null));
				System.Diagnostics.Debug.Assert((m_paths != null));
				System.Diagnostics.Debug.Assert(((m_num_embeddings == 3 && m_path_flags == null) || (m_num_embeddings == 4 && m_path_flags != null)));
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				com.epl.geometry.MultiPathImpl multi_path_impl = (com.epl.geometry.MultiPathImpl)polygon._getImpl();
				CheckPathPointCountsForMultiPath_(true);
				multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, m_position);
				if (m_b_has_zs)
				{
					System.Diagnostics.Debug.Assert((m_zs != null));
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, m_zs);
				}
				if (m_b_has_ms)
				{
					System.Diagnostics.Debug.Assert((m_ms != null));
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, m_ms);
				}
				if (m_path_flags == null)
				{
					m_path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(m_paths.Size(), unchecked((byte)0));
					m_path_flags.SetBits(0, unchecked((byte)(com.epl.geometry.PathFlags.enumClosed | com.epl.geometry.PathFlags.enumOGCStartPolygon)));
					for (int i = 1; i < m_path_flags.Size() - 1; i++)
					{
						m_path_flags.SetBits(i, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
					}
				}
				multi_path_impl.SetPathStreamRef(m_paths);
				multi_path_impl.SetPathFlagsStreamRef(m_path_flags);
				multi_path_impl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				com.epl.geometry.AttributeStreamOfInt8 path_flags_clone = new com.epl.geometry.AttributeStreamOfInt8(m_path_flags);
				for (int i_1 = 0; i_1 < path_flags_clone.Size() - 1; i_1++)
				{
					System.Diagnostics.Debug.Assert(((path_flags_clone.Read(i_1) & com.epl.geometry.PathFlags.enumClosed) != 0));
					System.Diagnostics.Debug.Assert(((m_path_flags.Read(i_1) & com.epl.geometry.PathFlags.enumClosed) != 0));
					if ((path_flags_clone.Read(i_1) & com.epl.geometry.PathFlags.enumOGCStartPolygon) != 0)
					{
						// Should
						// be
						// clockwise
						if (!com.epl.geometry.InternalUtils.IsClockwiseRing(multi_path_impl, i_1))
						{
							multi_path_impl.ReversePath(i_1);
						}
					}
					else
					{
						// make clockwise
						// Should be counter-clockwise
						if (com.epl.geometry.InternalUtils.IsClockwiseRing(multi_path_impl, i_1))
						{
							multi_path_impl.ReversePath(i_1);
						}
					}
				}
				// make
				// counter-clockwise
				multi_path_impl.SetPathFlagsStreamRef(path_flags_clone);
				multi_path_impl.ClearDirtyOGCFlags();
				return polygon;
			}

			private com.epl.geometry.Geometry CreatePolylineFromStreams_()
			{
				System.Diagnostics.Debug.Assert((m_position != null));
				System.Diagnostics.Debug.Assert(((m_num_embeddings == 2 && m_paths == null) || (m_num_embeddings == 3 && m_paths != null)));
				System.Diagnostics.Debug.Assert((m_path_flags == null));
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				com.epl.geometry.MultiPathImpl multi_path_impl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
				if (m_paths == null)
				{
					m_paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(0);
					m_paths.Add(0);
					m_paths.Add(m_position.Size() / 2);
				}
				CheckPathPointCountsForMultiPath_(false);
				multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, m_position);
				if (m_b_has_zs)
				{
					System.Diagnostics.Debug.Assert((m_zs != null));
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, m_zs);
				}
				if (m_b_has_ms)
				{
					System.Diagnostics.Debug.Assert((m_ms != null));
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, m_ms);
				}
				m_path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(m_paths.Size(), unchecked((byte)0));
				multi_path_impl.SetPathStreamRef(m_paths);
				multi_path_impl.SetPathFlagsStreamRef(m_path_flags);
				multi_path_impl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				return polyline;
			}

			private com.epl.geometry.Geometry CreateMultiPointFromStreams_()
			{
				System.Diagnostics.Debug.Assert((m_position != null));
				System.Diagnostics.Debug.Assert((m_paths == null));
				System.Diagnostics.Debug.Assert((m_path_flags == null));
				com.epl.geometry.MultiPoint multi_point = new com.epl.geometry.MultiPoint();
				com.epl.geometry.MultiPointImpl multi_point_impl = (com.epl.geometry.MultiPointImpl)multi_point._getImpl();
				multi_point_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, m_position);
				if (m_b_has_zs)
				{
					System.Diagnostics.Debug.Assert((m_zs != null));
					multi_point_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, m_zs);
				}
				if (m_b_has_ms)
				{
					System.Diagnostics.Debug.Assert((m_ms != null));
					multi_point_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, m_ms);
				}
				multi_point_impl.Resize(m_position.Size() / 2);
				multi_point_impl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				return multi_point;
			}

			private void CheckPathPointCountsForMultiPath_(bool b_is_polygon)
			{
				com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
				double z1 = 0.0;
				double z2 = 0.0;
				double m1 = 0.0;
				double m2 = 0.0;
				int path_count = m_paths.Size() - 1;
				int guess_adjustment = 0;
				if (b_is_polygon)
				{
					// Polygon
					guess_adjustment = path_count;
				}
				else
				{
					// may remove up to path_count
					// number of points
					// Polyline
					for (int path = 0; path < path_count; path++)
					{
						int path_size = m_paths.Read(path + 1) - m_paths.Read(path);
						if (path_size == 1)
						{
							guess_adjustment--;
						}
					}
					// will add a point for each path
					// containing only 1 point
					if (guess_adjustment == 0)
					{
						return;
					}
				}
				// all paths are okay
				com.epl.geometry.AttributeStreamOfDbl adjusted_position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(m_position.Size() - guess_adjustment);
				com.epl.geometry.AttributeStreamOfInt32 adjusted_paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(m_paths.Size());
				com.epl.geometry.AttributeStreamOfDbl adjusted_zs = null;
				com.epl.geometry.AttributeStreamOfDbl adjusted_ms = null;
				if (m_b_has_zs)
				{
					adjusted_zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(m_zs.Size() - guess_adjustment);
				}
				if (m_b_has_ms)
				{
					adjusted_ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(m_ms.Size() - guess_adjustment);
				}
				int adjusted_start = 0;
				adjusted_paths.Write(0, 0);
				for (int path_1 = 0; path_1 < path_count; path_1++)
				{
					int path_start = m_paths.Read(path_1);
					int path_end = m_paths.Read(path_1 + 1);
					int path_size = path_end - path_start;
					System.Diagnostics.Debug.Assert((path_size != 0));
					// we should not have added empty parts
					// on import
					if (path_size == 1)
					{
						InsertIntoAdjustedStreams_(adjusted_position, adjusted_zs, adjusted_ms, adjusted_start, path_start, path_size);
						InsertIntoAdjustedStreams_(adjusted_position, adjusted_zs, adjusted_ms, adjusted_start + 1, path_start, path_size);
						adjusted_start += 2;
					}
					else
					{
						if (path_size >= 3 && b_is_polygon)
						{
							m_position.Read(path_start * 2, pt1);
							m_position.Read((path_end - 1) * 2, pt2);
							if (m_b_has_zs)
							{
								z1 = m_zs.ReadAsDbl(path_start);
								z2 = m_zs.ReadAsDbl(path_end - 1);
							}
							if (m_b_has_ms)
							{
								m1 = m_ms.ReadAsDbl(path_start);
								m2 = m_ms.ReadAsDbl(path_end - 1);
							}
							if (pt1.Equals(pt2) && (com.epl.geometry.NumberUtils.IsNaN(z1) && com.epl.geometry.NumberUtils.IsNaN(z2) || z1 == z2) && (com.epl.geometry.NumberUtils.IsNaN(m1) && com.epl.geometry.NumberUtils.IsNaN(m2) || m1 == m2))
							{
								InsertIntoAdjustedStreams_(adjusted_position, adjusted_zs, adjusted_ms, adjusted_start, path_start, path_size - 1);
								adjusted_start += path_size - 1;
							}
							else
							{
								InsertIntoAdjustedStreams_(adjusted_position, adjusted_zs, adjusted_ms, adjusted_start, path_start, path_size);
								adjusted_start += path_size;
							}
						}
						else
						{
							InsertIntoAdjustedStreams_(adjusted_position, adjusted_zs, adjusted_ms, adjusted_start, path_start, path_size);
							adjusted_start += path_size;
						}
					}
					adjusted_paths.Write(path_1 + 1, adjusted_start);
				}
				m_position = adjusted_position;
				m_paths = adjusted_paths;
				m_zs = adjusted_zs;
				m_ms = adjusted_ms;
			}

			private void InsertIntoAdjustedStreams_(com.epl.geometry.AttributeStreamOfDbl adjusted_position, com.epl.geometry.AttributeStreamOfDbl adjusted_zs, com.epl.geometry.AttributeStreamOfDbl adjusted_ms, int adjusted_start, int path_start, int count)
			{
				adjusted_position.InsertRange(adjusted_start * 2, m_position, path_start * 2, count * 2, true, 2, adjusted_start * 2);
				if (m_b_has_zs)
				{
					adjusted_zs.InsertRange(adjusted_start, m_zs, path_start, count, true, 1, adjusted_start);
				}
				if (m_b_has_ms)
				{
					adjusted_ms.InsertRange(adjusted_start, m_ms, path_start, count, true, 1, adjusted_start);
				}
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			internal static com.epl.geometry.SpatialReference ImportSpatialReferenceFromCrs(com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker)
			{
				// According to the spec, a null crs corresponds to no spatial
				// reference
				if (json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_NULL)
				{
					return null;
				}
				if (json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_STRING)
				{
					// see
					// http://wiki.geojson.org/RFC-001
					// (this
					// is
					// deprecated,
					// but
					// there
					// may
					// be
					// data
					// with
					// this
					// format)
					string crs_short_form = json_iterator.CurrentString();
					int wkid = com.epl.geometry.GeoJsonCrsTables.GetWkidFromCrsShortForm(crs_short_form);
					if (wkid == -1)
					{
						throw new com.epl.geometry.GeometryException("not implemented");
					}
					com.epl.geometry.SpatialReference spatial_reference = null;
					try
					{
						spatial_reference = com.epl.geometry.SpatialReference.Create(wkid);
					}
					catch (System.Exception)
					{
					}
					return spatial_reference;
				}
				if (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.START_OBJECT)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				// This is to support all cases of crs identifiers I've seen. Some
				// may be rare or are legacy formats, but all are simple to
				// accomodate.
				bool b_found_type = false;
				bool b_found_properties = false;
				bool b_found_properties_name = false;
				bool b_found_properties_href = false;
				bool b_found_properties_urn = false;
				bool b_found_properties_url = false;
				bool b_found_properties_code = false;
				bool b_found_esriwkt = false;
				string crs_field = null;
				string properties_field = null;
				string crs_identifier_name = null;
				string crs_identifier_urn = null;
				string crs_identifier_href = null;
				string crs_identifier_url = null;
				string esriwkt = null;
				int crs_identifier_code = -1;
				com.epl.geometry.JsonReader.Token current_token;
				while (json_iterator.NextToken() != com.epl.geometry.JsonReader.Token.END_OBJECT)
				{
					crs_field = json_iterator.CurrentString();
					if (crs_field.Equals("type"))
					{
						if (b_found_type)
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
						b_found_type = true;
						current_token = json_iterator.NextToken();
						if (current_token != com.epl.geometry.JsonReader.Token.VALUE_STRING)
						{
							throw new com.epl.geometry.JsonGeometryException("parsing error");
						}
					}
					else
					{
						//type = json_iterator.currentString();
						if (crs_field.Equals("properties"))
						{
							if (b_found_properties)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							b_found_properties = true;
							current_token = json_iterator.NextToken();
							if (current_token != com.epl.geometry.JsonReader.Token.START_OBJECT)
							{
								throw new com.epl.geometry.JsonGeometryException("parsing error");
							}
							while (json_iterator.NextToken() != com.epl.geometry.JsonReader.Token.END_OBJECT)
							{
								properties_field = json_iterator.CurrentString();
								if (properties_field.Equals("name"))
								{
									if (b_found_properties_name)
									{
										throw new com.epl.geometry.JsonGeometryException("parsing error");
									}
									b_found_properties_name = true;
									crs_identifier_name = GetCrsIdentifier_(json_iterator);
								}
								else
								{
									if (properties_field.Equals("href"))
									{
										if (b_found_properties_href)
										{
											throw new com.epl.geometry.JsonGeometryException("parsing error");
										}
										b_found_properties_href = true;
										crs_identifier_href = GetCrsIdentifier_(json_iterator);
									}
									else
									{
										if (properties_field.Equals("urn"))
										{
											if (b_found_properties_urn)
											{
												throw new com.epl.geometry.JsonGeometryException("parsing error");
											}
											b_found_properties_urn = true;
											crs_identifier_urn = GetCrsIdentifier_(json_iterator);
										}
										else
										{
											if (properties_field.Equals("url"))
											{
												if (b_found_properties_url)
												{
													throw new com.epl.geometry.JsonGeometryException("parsing error");
												}
												b_found_properties_url = true;
												crs_identifier_url = GetCrsIdentifier_(json_iterator);
											}
											else
											{
												if (properties_field.Equals("code"))
												{
													if (b_found_properties_code)
													{
														throw new com.epl.geometry.JsonGeometryException("parsing error");
													}
													b_found_properties_code = true;
													current_token = json_iterator.NextToken();
													if (current_token != com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT)
													{
														throw new com.epl.geometry.JsonGeometryException("parsing error");
													}
													crs_identifier_code = json_iterator.CurrentIntValue();
												}
												else
												{
													json_iterator.NextToken();
													json_iterator.SkipChildren();
												}
											}
										}
									}
								}
							}
						}
						else
						{
							if (crs_field.Equals("esriwkt"))
							{
								if (b_found_esriwkt)
								{
									throw new com.epl.geometry.JsonGeometryException("parsing error");
								}
								b_found_esriwkt = true;
								current_token = json_iterator.NextToken();
								if (current_token != com.epl.geometry.JsonReader.Token.VALUE_STRING)
								{
									throw new com.epl.geometry.JsonGeometryException("parsing error");
								}
								esriwkt = json_iterator.CurrentString();
							}
							else
							{
								json_iterator.NextToken();
								json_iterator.SkipChildren();
							}
						}
					}
				}
				if ((!b_found_type || !b_found_properties) && !b_found_esriwkt)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				int wkid_1 = -1;
				if (b_found_properties_name)
				{
					wkid_1 = com.epl.geometry.GeoJsonCrsTables.GetWkidFromCrsName(crs_identifier_name);
				}
				else
				{
					// see
					// http://wiki.geojson.org/GeoJSON_draft_version_6
					// (most
					// common)
					if (b_found_properties_href)
					{
						wkid_1 = com.epl.geometry.GeoJsonCrsTables.GetWkidFromCrsHref(crs_identifier_href);
					}
					else
					{
						// see
						// http://wiki.geojson.org/GeoJSON_draft_version_6
						// (somewhat
						// common)
						if (b_found_properties_urn)
						{
							wkid_1 = com.epl.geometry.GeoJsonCrsTables.GetWkidFromCrsOgcUrn(crs_identifier_urn);
						}
						else
						{
							// see
							// http://wiki.geojson.org/GeoJSON_draft_version_5
							// (rare)
							if (b_found_properties_url)
							{
								wkid_1 = com.epl.geometry.GeoJsonCrsTables.GetWkidFromCrsHref(crs_identifier_url);
							}
							else
							{
								// see
								// http://wiki.geojson.org/GeoJSON_draft_version_5
								// (rare)
								if (b_found_properties_code)
								{
									wkid_1 = crs_identifier_code;
								}
								else
								{
									// see
									// http://wiki.geojson.org/GeoJSON_draft_version_5
									// (rare)
									if (!b_found_esriwkt)
									{
										throw new com.epl.geometry.JsonGeometryException("parsing error");
									}
								}
							}
						}
					}
				}
				if (wkid_1 < 0 && !b_found_esriwkt && !b_found_properties_name)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				com.epl.geometry.SpatialReference spatial_reference_1 = null;
				if (wkid_1 > 0)
				{
					try
					{
						spatial_reference_1 = com.epl.geometry.SpatialReference.Create(wkid_1);
					}
					catch (System.Exception)
					{
					}
				}
				if (spatial_reference_1 == null)
				{
					try
					{
						if (b_found_esriwkt)
						{
							// I exported crs wkt strings like
							// this
							spatial_reference_1 = com.epl.geometry.SpatialReference.Create(esriwkt);
						}
						else
						{
							if (b_found_properties_name)
							{
								// AGOL exported crs
								// wkt strings like
								// this where the
								// crs identifier of
								// the properties
								// name is like
								// "ESRI:<wkt>"
								string potential_wkt = com.epl.geometry.GeoJsonCrsTables.GetWktFromCrsName(crs_identifier_name);
								spatial_reference_1 = com.epl.geometry.SpatialReference.Create(potential_wkt);
							}
						}
					}
					catch (System.Exception)
					{
					}
				}
				return spatial_reference_1;
			}

			// see http://geojsonwg.github.io/draft-geojson/draft.html
			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			internal static com.epl.geometry.SpatialReference ImportSpatialReferenceFromCrsUrn_(com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker)
			{
				// According to the spec, a null crs corresponds to no spatial
				// reference
				if (json_iterator.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_NULL)
				{
					return null;
				}
				if (json_iterator.CurrentToken() != com.epl.geometry.JsonReader.Token.VALUE_STRING)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				string crs_identifier_urn = json_iterator.CurrentString();
				int wkid = com.epl.geometry.GeoJsonCrsTables.GetWkidFromCrsName(crs_identifier_urn);
				// This
				// will
				// check
				// for
				// short
				// form
				// name,
				// as
				// well
				// as
				// long
				// form
				// URNs
				if (wkid == -1)
				{
					throw new com.epl.geometry.GeometryException("not implemented");
				}
				com.epl.geometry.SpatialReference spatial_reference = com.epl.geometry.SpatialReference.Create(wkid);
				return spatial_reference;
			}

			/// <exception cref="com.epl.geometry.JsonGeometryException"/>
			private static string GetCrsIdentifier_(com.epl.geometry.JsonReader json_iterator)
			{
				com.epl.geometry.JsonReader.Token current_token = json_iterator.NextToken();
				if (current_token != com.epl.geometry.JsonReader.Token.VALUE_STRING)
				{
					throw new com.epl.geometry.JsonGeometryException("parsing error");
				}
				return json_iterator.CurrentString();
			}
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override com.epl.geometry.MapOGCStructure ExecuteOGC(int import_flags, string geoJsonString, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return ExecuteOGC(import_flags, com.epl.geometry.JsonParserReader.CreateFromString(geoJsonString), progress_tracker);
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public virtual com.epl.geometry.MapOGCStructure ExecuteOGC(int import_flags, com.epl.geometry.JsonReader json_iterator, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.MapOGCStructure mapOGCStructure = com.epl.geometry.OperatorImportFromGeoJsonLocal.OperatorImportFromGeoJsonHelper.ImportFromGeoJson(import_flags, com.epl.geometry.Geometry.Type.Unknown, json_iterator, progress_tracker, false, 0);
			//This is to restore legacy behavior when we always return a geometry collection of one element.
			com.epl.geometry.MapOGCStructure res = new com.epl.geometry.MapOGCStructure();
			res.m_ogcStructure = new com.epl.geometry.OGCStructure();
			res.m_ogcStructure.m_type = 0;
			res.m_ogcStructure.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>();
			res.m_ogcStructure.m_structures.Add(mapOGCStructure.m_ogcStructure);
			res.m_spatialReference = mapOGCStructure.m_spatialReference;
			return res;
		}
	}
}
