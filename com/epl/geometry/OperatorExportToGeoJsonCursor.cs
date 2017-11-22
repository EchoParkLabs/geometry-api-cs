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
Environmental Systems Research Institute, Inc.
Attn: Contracts Dept
380 New York Street
Redlands, California, USA 92373

email: contracts@esri.com
*/


namespace com.epl.geometry
{
	internal class OperatorExportToGeoJsonCursor : com.epl.geometry.JsonCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal int m_index;

		internal int m_wkid = -1;

		internal int m_latest_wkid = -1;

		internal string m_wkt = null;

		internal bool m_preferMulti = false;

		private static org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory();

		public OperatorExportToGeoJsonCursor(bool preferMulti, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor)
		{
			m_index = -1;
			if (geometryCursor == null)
			{
				throw new System.ArgumentException();
			}
			if (spatialReference != null && !spatialReference.IsLocal())
			{
				m_wkid = spatialReference.GetOldID();
				m_wkt = spatialReference.GetText();
				m_latest_wkid = spatialReference.GetLatestID();
			}
			m_inputGeometryCursor = geometryCursor;
			m_preferMulti = preferMulti;
		}

		public OperatorExportToGeoJsonCursor(com.epl.geometry.GeometryCursor geometryCursor)
		{
			m_index = -1;
			if (geometryCursor == null)
			{
				throw new System.ArgumentException();
			}
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
				return ExportToGeoJson(geometry);
			}
			return null;
		}

		private string ExportToGeoJson(com.epl.geometry.Geometry geometry)
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			try
			{
				org.codehaus.jackson.JsonGenerator g = factory.CreateJsonGenerator(sw);
				int type = geometry.GetType().Value();
				switch (type)
				{
					case com.epl.geometry.Geometry.GeometryType.Point:
					{
						ExportPointToGeoJson(g, (com.epl.geometry.Point)geometry);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.MultiPoint:
					{
						ExportMultiPointToGeoJson(g, (com.epl.geometry.MultiPoint)geometry);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.Polyline:
					{
						ExportMultiPathToGeoJson(g, (com.epl.geometry.Polyline)geometry);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.Polygon:
					{
						ExportMultiPathToGeoJson(g, (com.epl.geometry.Polygon)geometry);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.Envelope:
					{
						ExportEnvelopeToGeoJson(g, (com.epl.geometry.Envelope)geometry);
						break;
					}

					default:
					{
						throw new System.Exception("not implemented for this geometry type");
					}
				}
				return sw.GetBuffer().ToString();
			}
			catch (System.IO.IOException e)
			{
				e.PrintStackTrace();
				return null;
			}
		}

		/// <exception cref="org.codehaus.jackson.JsonGenerationException"/>
		/// <exception cref="System.IO.IOException"/>
		private void ExportPointToGeoJson(org.codehaus.jackson.JsonGenerator g, com.epl.geometry.Point p)
		{
			if (m_preferMulti)
			{
				com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
				mp.Add(p);
				ExportMultiPointToGeoJson(g, mp);
				return;
			}
			g.WriteStartObject();
			g.WriteFieldName("type");
			g.WriteString("Point");
			g.WriteFieldName("coordinates");
			if (p.IsEmpty())
			{
				g.WriteNull();
			}
			else
			{
				g.WriteStartArray();
				WriteDouble(p.GetX(), g);
				WriteDouble(p.GetY(), g);
				if (p.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z))
				{
					WriteDouble(p.GetZ(), g);
				}
				g.WriteEndArray();
			}
			g.WriteEndObject();
			g.Close();
		}

		/// <exception cref="org.codehaus.jackson.JsonGenerationException"/>
		/// <exception cref="System.IO.IOException"/>
		private void ExportMultiPointToGeoJson(org.codehaus.jackson.JsonGenerator g, com.epl.geometry.MultiPoint mp)
		{
			g.WriteStartObject();
			g.WriteFieldName("type");
			g.WriteString("MultiPoint");
			g.WriteFieldName("coordinates");
			if (mp.IsEmpty())
			{
				g.WriteNull();
			}
			else
			{
				g.WriteStartArray();
				com.epl.geometry.MultiPointImpl mpImpl = (com.epl.geometry.MultiPointImpl)mp._getImpl();
				com.epl.geometry.AttributeStreamOfDbl zs = mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) ? (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z) : null;
				com.epl.geometry.Point2D p = new com.epl.geometry.Point2D();
				int n = mp.GetPointCount();
				for (int i = 0; i < n; i++)
				{
					mp.GetXY(i, p);
					g.WriteStartArray();
					WriteDouble(p.x, g);
					WriteDouble(p.y, g);
					if (zs != null)
					{
						WriteDouble(zs.Get(i), g);
					}
					g.WriteEndArray();
				}
				g.WriteEndArray();
			}
			g.WriteEndObject();
			g.Close();
		}

		/// <exception cref="org.codehaus.jackson.JsonGenerationException"/>
		/// <exception cref="System.IO.IOException"/>
		private void ExportMultiPathToGeoJson(org.codehaus.jackson.JsonGenerator g, com.epl.geometry.MultiPath p)
		{
			com.epl.geometry.MultiPathImpl pImpl = (com.epl.geometry.MultiPathImpl)p._getImpl();
			bool isPolygon = pImpl.m_bPolygon;
			int polyCount = isPolygon ? pImpl.GetOGCPolygonCount() : 0;
			// check yo' polys playa
			g.WriteStartObject();
			g.WriteFieldName("type");
			bool bCollection = false;
			if (isPolygon)
			{
				if (polyCount >= 2 || m_preferMulti)
				{
					// single polys seem to have a polyCount of 0, multi polys seem to be >= 2
					g.WriteString("MultiPolygon");
					bCollection = true;
				}
				else
				{
					g.WriteString("Polygon");
				}
			}
			else
			{
				if (p.GetPathCount() > 1 || m_preferMulti)
				{
					// single polys seem to have a polyCount of 0, multi polys seem to be >= 2
					g.WriteString("MultiLineString");
					bCollection = true;
				}
				else
				{
					g.WriteString("LineString");
				}
			}
			g.WriteFieldName("coordinates");
			if (p.IsEmpty())
			{
				g.WriteNull();
			}
			else
			{
				ExportMultiPathToGeoJson(g, pImpl, bCollection);
			}
			g.WriteEndObject();
			g.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		private void ExportMultiPathToGeoJson(org.codehaus.jackson.JsonGenerator g, com.epl.geometry.MultiPathImpl pImpl, bool bCollection)
		{
			int startIndex;
			int vertices;
			if (bCollection)
			{
				g.WriteStartArray();
			}
			//AttributeStreamOfDbl position = (AttributeStreamOfDbl) pImpl.getAttributeStreamRef(VertexDescription.Semantics.POSITION);
			//AttributeStreamOfInt8 pathFlags = pImpl.getPathFlagsStreamRef();
			//AttributeStreamOfInt32 paths = pImpl.getPathStreamRef();
			int pathCount = pImpl.GetPathCount();
			bool isPolygon = pImpl.m_bPolygon;
			com.epl.geometry.AttributeStreamOfDbl zs = pImpl.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z) ? (com.epl.geometry.AttributeStreamOfDbl)pImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z) : null;
			for (int path = 0; path < pathCount; path++)
			{
				startIndex = pImpl.GetPathStart(path);
				vertices = pImpl.GetPathSize(path);
				bool isExtRing = isPolygon && pImpl.IsExteriorRing(path);
				if (isExtRing)
				{
					//only for polygons
					if (path > 0)
					{
						g.WriteEndArray();
					}
					//end of OGC polygon
					g.WriteStartArray();
				}
				//start of next OGC polygon
				WritePath(pImpl, g, path, startIndex, vertices, zs);
			}
			if (isPolygon)
			{
				g.WriteEndArray();
			}
			//end of last OGC polygon
			if (bCollection)
			{
				g.WriteEndArray();
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void ClosePath(com.epl.geometry.MultiPathImpl mp, org.codehaus.jackson.JsonGenerator g, int startIndex, com.epl.geometry.AttributeStreamOfDbl zs)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			// close ring
			mp.GetXY(startIndex, pt);
			g.WriteStartArray();
			WriteDouble(pt.x, g);
			WriteDouble(pt.y, g);
			if (zs != null)
			{
				WriteDouble(zs.Get(startIndex), g);
			}
			g.WriteEndArray();
		}

		/// <exception cref="System.IO.IOException"/>
		private void WritePath(com.epl.geometry.MultiPathImpl mp, org.codehaus.jackson.JsonGenerator g, int pathIndex, int startIndex, int vertices, com.epl.geometry.AttributeStreamOfDbl zs)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			g.WriteStartArray();
			for (int i = startIndex; i < startIndex + vertices; i++)
			{
				mp.GetXY(i, pt);
				g.WriteStartArray();
				WriteDouble(pt.x, g);
				WriteDouble(pt.y, g);
				if (zs != null)
				{
					WriteDouble(zs.Get(i), g);
				}
				g.WriteEndArray();
			}
			if (mp.IsClosedPath(pathIndex))
			{
				ClosePath(mp, g, startIndex, zs);
			}
			g.WriteEndArray();
		}

		/// <exception cref="org.codehaus.jackson.JsonGenerationException"/>
		/// <exception cref="System.IO.IOException"/>
		private void ExportEnvelopeToGeoJson(org.codehaus.jackson.JsonGenerator g, com.epl.geometry.Envelope e)
		{
			bool empty = e.IsEmpty();
			g.WriteStartObject();
			g.WriteFieldName("bbox");
			if (empty)
			{
				g.WriteNull();
			}
			else
			{
				g.WriteStartArray();
				WriteDouble(e.GetXMin(), g);
				WriteDouble(e.GetYMin(), g);
				WriteDouble(e.GetXMax(), g);
				WriteDouble(e.GetYMax(), g);
				g.WriteEndArray();
			}
			g.WriteEndObject();
			g.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="org.codehaus.jackson.JsonGenerationException"/>
		private void WriteDouble(double d, org.codehaus.jackson.JsonGenerator g)
		{
			if (com.epl.geometry.NumberUtils.IsNaN(d))
			{
				g.WriteNull();
			}
			else
			{
				g.WriteNumber(d);
			}
			return;
		}
	}
}
