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
	internal class OperatorExportToJsonCursor : com.epl.geometry.JsonCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal com.epl.geometry.SpatialReference m_spatialReference;

		internal int m_index;

		public OperatorExportToJsonCursor(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.GeometryCursor geometryCursor)
		{
			m_index = -1;
			if (geometryCursor == null)
			{
				throw new System.ArgumentException();
			}
			m_inputGeometryCursor = geometryCursor;
			m_spatialReference = spatialReference;
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
				return ExportToString(geometry, m_spatialReference, null);
			}
			return null;
		}

		internal static string ExportToString(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			com.epl.geometry.JsonWriter jsonWriter = new com.epl.geometry.JsonStringWriter();
			ExportToJson_(geometry, spatialReference, jsonWriter, exportProperties);
			return (string)jsonWriter.GetJson();
		}

		private static void ExportToJson_(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			try
			{
				int type = geometry.GetType().Value();
				switch (type)
				{
					case com.epl.geometry.Geometry.GeometryType.Point:
					{
						ExportPointToJson((com.epl.geometry.Point)geometry, spatialReference, jsonWriter, exportProperties);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.MultiPoint:
					{
						ExportMultiPointToJson((com.epl.geometry.MultiPoint)geometry, spatialReference, jsonWriter, exportProperties);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.Polyline:
					{
						ExportPolylineToJson((com.epl.geometry.Polyline)geometry, spatialReference, jsonWriter, exportProperties);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.Polygon:
					{
						ExportPolygonToJson((com.epl.geometry.Polygon)geometry, spatialReference, jsonWriter, exportProperties);
						break;
					}

					case com.epl.geometry.Geometry.GeometryType.Envelope:
					{
						ExportEnvelopeToJson((com.epl.geometry.Envelope)geometry, spatialReference, jsonWriter, exportProperties);
						break;
					}

					default:
					{
						throw new System.Exception("not implemented for this geometry type");
					}
				}
			}
			catch (System.Exception)
			{
			}
		}

		private static void ExportPolygonToJson(com.epl.geometry.Polygon pp, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			ExportPolypathToJson(pp, "rings", spatialReference, jsonWriter, exportProperties);
		}

		private static void ExportPolylineToJson(com.epl.geometry.Polyline pp, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			ExportPolypathToJson(pp, "paths", spatialReference, jsonWriter, exportProperties);
		}

		private static void ExportPolypathToJson(com.epl.geometry.MultiPath pp, string name, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			bool bExportZs = pp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			bool bExportMs = pp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			bool bPositionAsF = false;
			int decimals = 17;
			if (exportProperties != null)
			{
				object numberOfDecimalsXY = exportProperties["numberOfDecimalsXY"];
				if (numberOfDecimalsXY != null && numberOfDecimalsXY is java.lang.Number)
				{
					bPositionAsF = true;
					decimals = ((java.lang.Number)numberOfDecimalsXY);
				}
			}
			jsonWriter.StartObject();
			if (bExportZs)
			{
				jsonWriter.AddPairBoolean("hasZ", true);
			}
			if (bExportMs)
			{
				jsonWriter.AddPairBoolean("hasM", true);
			}
			jsonWriter.AddPairArray(name);
			if (!pp.IsEmpty())
			{
				int n = pp.GetPathCount();
				// rings or paths
				com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)pp._getImpl();
				// get impl for
				// faster
				// access
				com.epl.geometry.AttributeStreamOfDbl zs = null;
				com.epl.geometry.AttributeStreamOfDbl ms = null;
				if (bExportZs)
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
				}
				if (bExportMs)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
				}
				bool bPolygon = pp is com.epl.geometry.Polygon;
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				for (int i = 0; i < n; i++)
				{
					jsonWriter.AddValueArray();
					int startindex = pp.GetPathStart(i);
					int numVertices = pp.GetPathSize(i);
					double startx = 0.0;
					double starty = 0.0;
					double startz = com.epl.geometry.NumberUtils.NaN();
					double startm = com.epl.geometry.NumberUtils.NaN();
					double z = com.epl.geometry.NumberUtils.NaN();
					double m = com.epl.geometry.NumberUtils.NaN();
					bool bClosed = pp.IsClosedPath(i);
					for (int j = startindex; j < startindex + numVertices; j++)
					{
						pp.GetXY(j, pt);
						jsonWriter.AddValueArray();
						if (bPositionAsF)
						{
							jsonWriter.AddValueDouble(pt.x, decimals, true);
							jsonWriter.AddValueDouble(pt.y, decimals, true);
						}
						else
						{
							jsonWriter.AddValueDouble(pt.x);
							jsonWriter.AddValueDouble(pt.y);
						}
						if (bExportZs)
						{
							z = zs.Get(j);
							jsonWriter.AddValueDouble(z);
						}
						if (bExportMs)
						{
							m = ms.Get(j);
							jsonWriter.AddValueDouble(m);
						}
						if (j == startindex && bClosed)
						{
							startx = pt.x;
							starty = pt.y;
							startz = z;
							startm = m;
						}
						jsonWriter.EndArray();
					}
					// Close the Path/Ring by writing the Point at the start index
					if (bClosed && (startx != pt.x || starty != pt.y || (bExportZs && !(com.epl.geometry.NumberUtils.IsNaN(startz) && com.epl.geometry.NumberUtils.IsNaN(z)) && startz != z) || (bExportMs && !(com.epl.geometry.NumberUtils.IsNaN(startm) && com.epl.geometry.NumberUtils.IsNaN(m)) && startm
						 != m)))
					{
						pp.GetXY(startindex, pt);
						// getPoint(startindex);
						jsonWriter.AddValueArray();
						if (bPositionAsF)
						{
							jsonWriter.AddValueDouble(pt.x, decimals, true);
							jsonWriter.AddValueDouble(pt.y, decimals, true);
						}
						else
						{
							jsonWriter.AddValueDouble(pt.x);
							jsonWriter.AddValueDouble(pt.y);
						}
						if (bExportZs)
						{
							z = zs.Get(startindex);
							jsonWriter.AddValueDouble(z);
						}
						if (bExportMs)
						{
							m = ms.Get(startindex);
							jsonWriter.AddValueDouble(m);
						}
						jsonWriter.EndArray();
					}
					jsonWriter.EndArray();
				}
			}
			jsonWriter.EndArray();
			if (spatialReference != null)
			{
				WriteSR(spatialReference, jsonWriter);
			}
			jsonWriter.EndObject();
		}

		private static void ExportMultiPointToJson(com.epl.geometry.MultiPoint mpt, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			bool bExportZs = mpt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			bool bExportMs = mpt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			bool bPositionAsF = false;
			int decimals = 17;
			if (exportProperties != null)
			{
				object numberOfDecimalsXY = exportProperties["numberOfDecimalsXY"];
				if (numberOfDecimalsXY != null && numberOfDecimalsXY is java.lang.Number)
				{
					bPositionAsF = true;
					decimals = ((java.lang.Number)numberOfDecimalsXY);
				}
			}
			jsonWriter.StartObject();
			if (bExportZs)
			{
				jsonWriter.AddPairBoolean("hasZ", true);
			}
			if (bExportMs)
			{
				jsonWriter.AddPairBoolean("hasM", true);
			}
			jsonWriter.AddPairArray("points");
			if (!mpt.IsEmpty())
			{
				com.epl.geometry.MultiPointImpl mpImpl = (com.epl.geometry.MultiPointImpl)mpt._getImpl();
				// get impl
				// for
				// faster
				// access
				com.epl.geometry.AttributeStreamOfDbl zs = null;
				com.epl.geometry.AttributeStreamOfDbl ms = null;
				if (bExportZs)
				{
					zs = (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z);
				}
				if (bExportMs)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)mpImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M);
				}
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				int n = mpt.GetPointCount();
				for (int i = 0; i < n; i++)
				{
					mpt.GetXY(i, pt);
					jsonWriter.AddValueArray();
					if (bPositionAsF)
					{
						jsonWriter.AddValueDouble(pt.x, decimals, true);
						jsonWriter.AddValueDouble(pt.y, decimals, true);
					}
					else
					{
						jsonWriter.AddValueDouble(pt.x);
						jsonWriter.AddValueDouble(pt.y);
					}
					if (bExportZs)
					{
						double z = zs.Get(i);
						jsonWriter.AddValueDouble(z);
					}
					if (bExportMs)
					{
						double m = ms.Get(i);
						jsonWriter.AddValueDouble(m);
					}
					jsonWriter.EndArray();
				}
			}
			jsonWriter.EndArray();
			if (spatialReference != null)
			{
				WriteSR(spatialReference, jsonWriter);
			}
			jsonWriter.EndObject();
		}

		private static void ExportPointToJson(com.epl.geometry.Point pt, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			bool bExportZs = pt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			bool bExportMs = pt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			bool bPositionAsF = false;
			int decimals = 17;
			if (exportProperties != null)
			{
				object numberOfDecimalsXY = exportProperties["numberOfDecimalsXY"];
				if (numberOfDecimalsXY != null && numberOfDecimalsXY is java.lang.Number)
				{
					bPositionAsF = true;
					decimals = ((java.lang.Number)numberOfDecimalsXY);
				}
			}
			jsonWriter.StartObject();
			if (pt.IsEmpty())
			{
				jsonWriter.AddPairNull("x");
				jsonWriter.AddPairNull("y");
				if (bExportZs)
				{
					jsonWriter.AddPairNull("z");
				}
				if (bExportMs)
				{
					jsonWriter.AddPairNull("m");
				}
			}
			else
			{
				if (bPositionAsF)
				{
					jsonWriter.AddPairDouble("x", pt.GetX(), decimals, true);
					jsonWriter.AddPairDouble("y", pt.GetY(), decimals, true);
				}
				else
				{
					jsonWriter.AddPairDouble("x", pt.GetX());
					jsonWriter.AddPairDouble("y", pt.GetY());
				}
				if (bExportZs)
				{
					jsonWriter.AddPairDouble("z", pt.GetZ());
				}
				if (bExportMs)
				{
					jsonWriter.AddPairDouble("m", pt.GetM());
				}
			}
			if (spatialReference != null)
			{
				WriteSR(spatialReference, jsonWriter);
			}
			jsonWriter.EndObject();
		}

		private static void ExportEnvelopeToJson(com.epl.geometry.Envelope env, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter, System.Collections.Generic.IDictionary<string, object> exportProperties)
		{
			bool bExportZs = env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			bool bExportMs = env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			bool bPositionAsF = false;
			int decimals = 17;
			if (exportProperties != null)
			{
				object numberOfDecimalsXY = exportProperties["numberOfDecimalsXY"];
				if (numberOfDecimalsXY != null && numberOfDecimalsXY is java.lang.Number)
				{
					bPositionAsF = true;
					decimals = ((java.lang.Number)numberOfDecimalsXY);
				}
			}
			jsonWriter.StartObject();
			if (env.IsEmpty())
			{
				jsonWriter.AddPairNull("xmin");
				jsonWriter.AddPairNull("ymin");
				jsonWriter.AddPairNull("xmax");
				jsonWriter.AddPairNull("ymax");
				if (bExportZs)
				{
					jsonWriter.AddPairNull("zmin");
					jsonWriter.AddPairNull("zmax");
				}
				if (bExportMs)
				{
					jsonWriter.AddPairNull("mmin");
					jsonWriter.AddPairNull("mmax");
				}
			}
			else
			{
				if (bPositionAsF)
				{
					jsonWriter.AddPairDouble("xmin", env.GetXMin(), decimals, true);
					jsonWriter.AddPairDouble("ymin", env.GetYMin(), decimals, true);
					jsonWriter.AddPairDouble("xmax", env.GetXMax(), decimals, true);
					jsonWriter.AddPairDouble("ymax", env.GetYMax(), decimals, true);
				}
				else
				{
					jsonWriter.AddPairDouble("xmin", env.GetXMin());
					jsonWriter.AddPairDouble("ymin", env.GetYMin());
					jsonWriter.AddPairDouble("xmax", env.GetXMax());
					jsonWriter.AddPairDouble("ymax", env.GetYMax());
				}
				if (bExportZs)
				{
					com.epl.geometry.Envelope1D z = env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
					jsonWriter.AddPairDouble("zmin", z.vmin);
					jsonWriter.AddPairDouble("zmax", z.vmax);
				}
				if (bExportMs)
				{
					com.epl.geometry.Envelope1D m = env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
					jsonWriter.AddPairDouble("mmin", m.vmin);
					jsonWriter.AddPairDouble("mmax", m.vmax);
				}
			}
			if (spatialReference != null)
			{
				WriteSR(spatialReference, jsonWriter);
			}
			jsonWriter.EndObject();
		}

		private static void WriteSR(com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.JsonWriter jsonWriter)
		{
			int wkid = spatialReference.GetOldID();
			if (wkid > 0)
			{
				jsonWriter.AddPairObject("spatialReference");
				jsonWriter.AddPairInt("wkid", wkid);
				int latest_wkid = spatialReference.GetLatestID();
				if (latest_wkid > 0 && latest_wkid != wkid)
				{
					jsonWriter.AddPairInt("latestWkid", latest_wkid);
				}
				jsonWriter.EndObject();
			}
			else
			{
				string wkt = spatialReference.GetText();
				if (wkt != null)
				{
					jsonWriter.AddPairObject("spatialReference");
					jsonWriter.AddPairString("wkt", wkt);
					jsonWriter.EndObject();
				}
			}
		}
	}
}
