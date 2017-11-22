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
		/// <exception cref="org.json.JSONException"/>
		public override com.epl.geometry.MapGeometry Execute(int importFlags, com.epl.geometry.Geometry.Type type, string geoJsonString, com.epl.geometry.ProgressTracker progress_tracker)
		{
			org.json.JSONObject geoJsonObject = new org.json.JSONObject(geoJsonString);
			com.epl.geometry.Geometry geometry = ImportGeometryFromGeoJson_(importFlags, type, geoJsonObject);
			com.epl.geometry.SpatialReference spatialReference = ImportSpatialReferenceFromGeoJson_(geoJsonObject);
			com.epl.geometry.MapGeometry mapGeometry = new com.epl.geometry.MapGeometry(geometry, spatialReference);
			return mapGeometry;
		}

		/// <exception cref="org.json.JSONException"/>
		internal static org.json.JSONArray GetJSONArray(org.json.JSONObject obj, string name)
		{
			if (obj.Get(name) == org.json.JSONObject.NULL)
			{
				return new org.json.JSONArray();
			}
			else
			{
				return obj.GetJSONArray(name);
			}
		}

		/// <exception cref="org.json.JSONException"/>
		public override com.epl.geometry.MapOGCStructure ExecuteOGC(int import_flags, string geoJsonString, com.epl.geometry.ProgressTracker progress_tracker)
		{
			org.json.JSONObject geoJsonObject = new org.json.JSONObject(geoJsonString);
			System.Collections.Generic.List<com.epl.geometry.OGCStructure> structureStack = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			System.Collections.Generic.List<org.json.JSONObject> objectStack = new System.Collections.Generic.List<org.json.JSONObject>(0);
			com.epl.geometry.AttributeStreamOfInt32 indices = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.AttributeStreamOfInt32 numGeometries = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.OGCStructure root = new com.epl.geometry.OGCStructure();
			root.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			structureStack.Add(root);
			// add dummy root
			objectStack.Add(geoJsonObject);
			indices.Add(0);
			numGeometries.Add(1);
			while (!objectStack.IsEmpty())
			{
				if (indices.GetLast() == numGeometries.GetLast())
				{
					structureStack.Remove(structureStack.Count - 1);
					indices.RemoveLast();
					numGeometries.RemoveLast();
					continue;
				}
				com.epl.geometry.OGCStructure lastStructure = structureStack[structureStack.Count - 1];
				org.json.JSONObject lastObject = objectStack[objectStack.Count - 1];
				objectStack.Remove(objectStack.Count - 1);
				indices.Write(indices.Size() - 1, indices.GetLast() + 1);
				string typeString = lastObject.GetString("type");
				if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "GeometryCollection"))
				{
					com.epl.geometry.OGCStructure next = new com.epl.geometry.OGCStructure();
					next.m_type = 7;
					next.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
					lastStructure.m_structures.Add(next);
					structureStack.Add(next);
					org.json.JSONArray geometries = GetJSONArray(lastObject, "geometries");
					indices.Add(0);
					numGeometries.Add(geometries.Length());
					for (int i = geometries.Length() - 1; i >= 0; i--)
					{
						objectStack.Add(geometries.GetJSONObject(i));
					}
				}
				else
				{
					int ogcType;
					if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "Point"))
					{
						ogcType = 1;
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "LineString"))
						{
							ogcType = 2;
						}
						else
						{
							if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "Polygon"))
							{
								ogcType = 3;
							}
							else
							{
								if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "MultiPoint"))
								{
									ogcType = 4;
								}
								else
								{
									if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "MultiLineString"))
									{
										ogcType = 5;
									}
									else
									{
										if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "MultiPolygon"))
										{
											ogcType = 6;
										}
										else
										{
											throw new System.NotSupportedException();
										}
									}
								}
							}
						}
					}
					com.epl.geometry.Geometry geometry = ImportGeometryFromGeoJson_(import_flags, com.epl.geometry.Geometry.Type.Unknown, lastObject);
					com.epl.geometry.OGCStructure leaf = new com.epl.geometry.OGCStructure();
					leaf.m_type = ogcType;
					leaf.m_geometry = geometry;
					lastStructure.m_structures.Add(leaf);
				}
			}
			com.epl.geometry.MapOGCStructure mapOGCStructure = new com.epl.geometry.MapOGCStructure();
			mapOGCStructure.m_ogcStructure = root;
			mapOGCStructure.m_spatialReference = ImportSpatialReferenceFromGeoJson_(geoJsonObject);
			return mapOGCStructure;
		}

		/// <exception cref="org.json.JSONException"/>
		private static com.epl.geometry.SpatialReference ImportSpatialReferenceFromGeoJson_(org.json.JSONObject crsJSONObject)
		{
			string wkidString = crsJSONObject.OptString("crs", string.Empty);
			if (wkidString.Equals(string.Empty))
			{
				return com.epl.geometry.SpatialReference.Create(4326);
			}
			// wkidString will be of the form "EPSG:#" where # is an integer, the
			// EPSG ID.
			// If the ID is below 32,767, then the EPSG ID will agree with the
			// well-known (WKID).
			if (wkidString.Length <= 5)
			{
				throw new System.ArgumentException();
			}
			// Throws a JSON exception if this cannot appropriately be converted to
			// an integer.
			int wkid = int.Parse(Sharpen.Runtime.Substring(wkidString, 5));
			return com.epl.geometry.SpatialReference.Create(wkid);
		}

		/// <exception cref="org.json.JSONException"/>
		private static com.epl.geometry.Geometry ImportGeometryFromGeoJson_(int importFlags, com.epl.geometry.Geometry.Type type, org.json.JSONObject geometryJSONObject)
		{
			string typeString = geometryJSONObject.GetString("type");
			org.json.JSONArray coordinateArray = GetJSONArray(geometryJSONObject, "coordinates");
			if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "MultiPolygon"))
			{
				if (type != com.epl.geometry.Geometry.Type.Polygon && type != com.epl.geometry.Geometry.Type.Unknown)
				{
					throw new System.ArgumentException("invalid shapetype");
				}
				return PolygonTaggedText_(true, importFlags, coordinateArray);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "MultiLineString"))
				{
					if (type != com.epl.geometry.Geometry.Type.Polyline && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return LineStringTaggedText_(true, importFlags, coordinateArray);
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "MultiPoint"))
					{
						if (type != com.epl.geometry.Geometry.Type.MultiPoint && type != com.epl.geometry.Geometry.Type.Unknown)
						{
							throw new System.ArgumentException("invalid shapetype");
						}
						return MultiPointTaggedText_(importFlags, coordinateArray);
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "Polygon"))
						{
							if (type != com.epl.geometry.Geometry.Type.Polygon && type != com.epl.geometry.Geometry.Type.Unknown)
							{
								throw new System.ArgumentException("invalid shapetype");
							}
							return PolygonTaggedText_(false, importFlags, coordinateArray);
						}
						else
						{
							if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "LineString"))
							{
								if (type != com.epl.geometry.Geometry.Type.Polyline && type != com.epl.geometry.Geometry.Type.Unknown)
								{
									throw new System.ArgumentException("invalid shapetype");
								}
								return LineStringTaggedText_(false, importFlags, coordinateArray);
							}
							else
							{
								if (Sharpen.Runtime.EqualsIgnoreCase(typeString, "Point"))
								{
									if (type != com.epl.geometry.Geometry.Type.Point && type != com.epl.geometry.Geometry.Type.Unknown)
									{
										throw new System.ArgumentException("invalid shapetype");
									}
									return PointTaggedText_(importFlags, coordinateArray);
								}
								else
								{
									return null;
								}
							}
						}
					}
				}
			}
		}

		/// <exception cref="org.json.JSONException"/>
		private static com.epl.geometry.Geometry PolygonTaggedText_(bool bMultiPolygon, int importFlags, org.json.JSONArray coordinateArray)
		{
			com.epl.geometry.MultiPath multiPath;
			com.epl.geometry.MultiPathImpl multiPathImpl;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfDbl position;
			com.epl.geometry.AttributeStreamOfInt32 paths;
			com.epl.geometry.AttributeStreamOfInt8 path_flags;
			position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
			paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(1, 0);
			path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(1, unchecked((byte)0));
			multiPath = new com.epl.geometry.Polygon();
			multiPathImpl = (com.epl.geometry.MultiPathImpl)multiPath._getImpl();
			int pointCount;
			if (bMultiPolygon)
			{
				pointCount = MultiPolygonText_(zs, ms, position, paths, path_flags, coordinateArray);
			}
			else
			{
				pointCount = PolygonText_(zs, ms, position, paths, path_flags, 0, coordinateArray);
			}
			if (pointCount != 0)
			{
				System.Diagnostics.Debug.Assert((2 * pointCount == position.Size()));
				multiPathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multiPathImpl.SetPathStreamRef(paths);
				multiPathImpl.SetPathFlagsStreamRef(path_flags);
				if (zs != null)
				{
					multiPathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (ms != null)
				{
					multiPathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				multiPathImpl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				com.epl.geometry.AttributeStreamOfInt8 path_flags_clone = new com.epl.geometry.AttributeStreamOfInt8(path_flags);
				for (int i = 0; i < path_flags_clone.Size() - 1; i++)
				{
					if (((int)path_flags_clone.Read(i) & (int)com.epl.geometry.PathFlags.enumOGCStartPolygon) != 0)
					{
						// Should
						// be
						// clockwise
						if (!com.epl.geometry.InternalUtils.IsClockwiseRing(multiPathImpl, i))
						{
							multiPathImpl.ReversePath(i);
						}
					}
					else
					{
						// make clockwise
						// Should be counter-clockwise
						if (com.epl.geometry.InternalUtils.IsClockwiseRing(multiPathImpl, i))
						{
							multiPathImpl.ReversePath(i);
						}
					}
				}
				// make counter-clockwise
				multiPathImpl.SetPathFlagsStreamRef(path_flags_clone);
			}
			if ((importFlags & (int)com.epl.geometry.GeoJsonImportFlags.geoJsonImportNonTrusted) == 0)
			{
				multiPathImpl.SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, 0.0, false);
			}
			multiPathImpl.SetDirtyOGCFlags(false);
			return multiPath;
		}

		/// <exception cref="org.json.JSONException"/>
		private static com.epl.geometry.Geometry LineStringTaggedText_(bool bMultiLineString, int importFlags, org.json.JSONArray coordinateArray)
		{
			com.epl.geometry.MultiPath multiPath;
			com.epl.geometry.MultiPathImpl multiPathImpl;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfDbl position;
			com.epl.geometry.AttributeStreamOfInt32 paths;
			com.epl.geometry.AttributeStreamOfInt8 path_flags;
			position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
			paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(1, 0);
			path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(1, unchecked((byte)0));
			multiPath = new com.epl.geometry.Polyline();
			multiPathImpl = (com.epl.geometry.MultiPathImpl)multiPath._getImpl();
			int pointCount;
			if (bMultiLineString)
			{
				pointCount = MultiLineStringText_(zs, ms, position, paths, path_flags, coordinateArray);
			}
			else
			{
				pointCount = LineStringText_(false, zs, ms, position, paths, path_flags, coordinateArray);
			}
			if (pointCount != 0)
			{
				System.Diagnostics.Debug.Assert((2 * pointCount == position.Size()));
				multiPathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multiPathImpl.SetPathStreamRef(paths);
				multiPathImpl.SetPathFlagsStreamRef(path_flags);
				if (zs != null)
				{
					multiPathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (ms != null)
				{
					multiPathImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				multiPathImpl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
			}
			return multiPath;
		}

		/// <exception cref="org.json.JSONException"/>
		private static com.epl.geometry.Geometry MultiPointTaggedText_(int importFlags, org.json.JSONArray coordinateArray)
		{
			com.epl.geometry.MultiPoint multiPoint;
			com.epl.geometry.MultiPointImpl multiPointImpl;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfDbl position;
			position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
			multiPoint = new com.epl.geometry.MultiPoint();
			multiPointImpl = (com.epl.geometry.MultiPointImpl)multiPoint._getImpl();
			int pointCount = MultiPointText_(zs, ms, position, coordinateArray);
			if (pointCount != 0)
			{
				System.Diagnostics.Debug.Assert((2 * pointCount == position.Size()));
				multiPointImpl.Resize(pointCount);
				multiPointImpl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multiPointImpl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
			}
			return multiPoint;
		}

		/// <exception cref="org.json.JSONException"/>
		private static com.epl.geometry.Geometry PointTaggedText_(int importFlags, org.json.JSONArray coordinateArray)
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			int length = coordinateArray.Length();
			if (length == 0)
			{
				point.SetEmpty();
				return point;
			}
			point.SetXY(GetDouble_(coordinateArray, 0), GetDouble_(coordinateArray, 1));
			return point;
		}

		/// <exception cref="org.json.JSONException"/>
		private static int MultiPolygonText_(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags, org.json.JSONArray
			 coordinateArray)
		{
			// At start of MultiPolygonText
			int totalPointCount = 0;
			int length = coordinateArray.Length();
			if (length == 0)
			{
				return totalPointCount;
			}
			for (int current = 0; current < length; current++)
			{
				org.json.JSONArray subArray = coordinateArray.OptJSONArray(current);
				if (subArray == null)
				{
					// Entry should be a JSONArray representing a polygon, but it is
					// not a JSONArray.
					throw new System.ArgumentException(string.Empty);
				}
				// At start of PolygonText
				totalPointCount = PolygonText_(zs, ms, position, paths, path_flags, totalPointCount, subArray);
			}
			return totalPointCount;
		}

		/// <exception cref="org.json.JSONException"/>
		private static int MultiLineStringText_(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags, org.json.JSONArray
			 coordinateArray)
		{
			// At start of MultiLineStringText
			int totalPointCount = 0;
			int length = coordinateArray.Length();
			if (length == 0)
			{
				return totalPointCount;
			}
			for (int current = 0; current < length; current++)
			{
				org.json.JSONArray subArray = coordinateArray.OptJSONArray(current);
				if (subArray == null)
				{
					// Entry should be a JSONArray representing a line string, but
					// it is not a JSONArray.
					throw new System.ArgumentException(string.Empty);
				}
				// At start of LineStringText
				totalPointCount += LineStringText_(false, zs, ms, position, paths, path_flags, subArray);
			}
			return totalPointCount;
		}

		/// <exception cref="org.json.JSONException"/>
		private static int MultiPointText_(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, org.json.JSONArray coordinateArray)
		{
			// At start of MultiPointText
			int pointCount = 0;
			for (int current = 0; current < coordinateArray.Length(); current++)
			{
				org.json.JSONArray subArray = coordinateArray.OptJSONArray(current);
				if (subArray == null)
				{
					// Entry should be a JSONArray representing a point, but it is
					// not a JSONArray.
					throw new System.ArgumentException(string.Empty);
				}
				pointCount += PointText_(zs, ms, position, subArray);
			}
			return pointCount;
		}

		/// <exception cref="org.json.JSONException"/>
		private static int PolygonText_(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags, int totalPointCount
			, org.json.JSONArray coordinateArray)
		{
			// At start of PolygonText
			int length = coordinateArray.Length();
			if (length == 0)
			{
				return totalPointCount;
			}
			bool bFirstLineString = true;
			for (int current = 0; current < length; current++)
			{
				org.json.JSONArray subArray = coordinateArray.OptJSONArray(current);
				if (subArray == null)
				{
					// Entry should be a JSONArray representing a line string, but
					// it is not a JSONArray.
					throw new System.ArgumentException(string.Empty);
				}
				// At start of LineStringText
				int pointCount = LineStringText_(true, zs, ms, position, paths, path_flags, subArray);
				if (pointCount != 0)
				{
					if (bFirstLineString)
					{
						bFirstLineString = false;
						path_flags.SetBits(path_flags.Size() - 2, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
					}
					path_flags.SetBits(path_flags.Size() - 2, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
					totalPointCount += pointCount;
				}
			}
			return totalPointCount;
		}

		/// <exception cref="org.json.JSONException"/>
		private static int LineStringText_(bool bRing, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags
			, org.json.JSONArray coordinateArray)
		{
			// At start of LineStringText
			int pointCount = 0;
			int length = coordinateArray.Length();
			if (length == 0)
			{
				return pointCount;
			}
			bool bStartPath = true;
			double startX = com.epl.geometry.NumberUtils.TheNaN;
			double startY = com.epl.geometry.NumberUtils.TheNaN;
			double startZ = com.epl.geometry.NumberUtils.TheNaN;
			double startM = com.epl.geometry.NumberUtils.TheNaN;
			for (int current = 0; current < length; current++)
			{
				org.json.JSONArray subArray = coordinateArray.OptJSONArray(current);
				if (subArray == null)
				{
					// Entry should be a JSONArray representing a single point, but
					// it is not a JSONArray.
					throw new System.ArgumentException(string.Empty);
				}
				// At start of x
				double x = GetDouble_(subArray, 0);
				double y = GetDouble_(subArray, 1);
				double z = com.epl.geometry.NumberUtils.TheNaN;
				double m = com.epl.geometry.NumberUtils.TheNaN;
				bool bAddPoint = true;
				if (bRing && pointCount >= 2 && current == length - 1)
				{
					// If the last point in the ring is not equal to the start
					// point, then let's add it.
					if ((startX == x || (com.epl.geometry.NumberUtils.IsNaN(startX) && com.epl.geometry.NumberUtils.IsNaN(x))) && (startY == y || (com.epl.geometry.NumberUtils.IsNaN(startY) && com.epl.geometry.NumberUtils.IsNaN(y))))
					{
						bAddPoint = false;
					}
				}
				if (bAddPoint)
				{
					if (bStartPath)
					{
						bStartPath = false;
						startX = x;
						startY = y;
						startZ = z;
						startM = m;
					}
					pointCount++;
					AddToStreams_(zs, ms, position, x, y, z, m);
				}
			}
			if (pointCount == 1)
			{
				pointCount++;
				AddToStreams_(zs, ms, position, startX, startY, startZ, startM);
			}
			paths.Add(position.Size() / 2);
			path_flags.Add(unchecked((byte)0));
			return pointCount;
		}

		/// <exception cref="org.json.JSONException"/>
		private static int PointText_(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, org.json.JSONArray coordinateArray)
		{
			// At start of PointText
			int length = coordinateArray.Length();
			if (length == 0)
			{
				return 0;
			}
			// At start of x
			double x = GetDouble_(coordinateArray, 0);
			double y = GetDouble_(coordinateArray, 1);
			double z = com.epl.geometry.NumberUtils.TheNaN;
			double m = com.epl.geometry.NumberUtils.TheNaN;
			AddToStreams_(zs, ms, position, x, y, z, m);
			return 1;
		}

		private static void AddToStreams_(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, double x, double y, double z, double m)
		{
			position.Add(x);
			position.Add(y);
			if (zs != null)
			{
				zs.Add(z);
			}
			if (ms != null)
			{
				ms.Add(m);
			}
		}

		/// <exception cref="org.json.JSONException"/>
		private static double GetDouble_(org.json.JSONArray coordinateArray, int index)
		{
			if (index < 0 || index >= coordinateArray.Length())
			{
				throw new System.ArgumentException(string.Empty);
			}
			if (coordinateArray.IsNull(index))
			{
				return com.epl.geometry.NumberUtils.TheNaN;
			}
			if (coordinateArray.OptDouble(index, com.epl.geometry.NumberUtils.TheNaN) != com.epl.geometry.NumberUtils.TheNaN)
			{
				return coordinateArray.GetDouble(index);
			}
			throw new System.ArgumentException(string.Empty);
		}
	}
}
