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
using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCConcreteGeometryCollection : com.epl.geometry.ogc.OGCGeometryCollection
	{
		public OGCConcreteGeometryCollection(System.Collections.Generic.IList<com.epl.geometry.ogc.OGCGeometry> geoms, com.epl.geometry.SpatialReference sr)
		{
			geometries = geoms;
			esriSR = sr;
		}

		public OGCConcreteGeometryCollection(com.epl.geometry.ogc.OGCGeometry geom, com.epl.geometry.SpatialReference sr)
		{
			geometries = new System.Collections.Generic.List<com.epl.geometry.ogc.OGCGeometry>(1);
			geometries.Add(geom);
			esriSR = sr;
		}

		public override int Dimension()
		{
			int maxD = 0;
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				maxD = System.Math.Max(GeometryN(i).Dimension(), maxD);
			}
			return maxD;
		}

		public override int CoordinateDimension()
		{
			return IsEmpty() ? 2 : GeometryN(0).CoordinateDimension();
		}

		public override bool Is3D()
		{
			return !IsEmpty() && geometries[0].Is3D();
		}

		public override bool IsMeasured()
		{
			return !IsEmpty() && geometries[0].IsMeasured();
		}

		public override com.epl.geometry.ogc.OGCGeometry Envelope()
		{
			com.epl.geometry.GeometryCursor gc = GetEsriGeometryCursor();
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			for (com.epl.geometry.Geometry g = gc.Next(); g != null; g = gc.Next())
			{
				com.epl.geometry.Envelope e = new com.epl.geometry.Envelope();
				g.QueryEnvelope(e);
				env.Merge(e);
			}
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			polygon.AddEnvelope(env, false);
			return new com.epl.geometry.ogc.OGCPolygon(polygon, esriSR);
		}

		public override int NumGeometries()
		{
			return geometries.Count;
		}

		public override com.epl.geometry.ogc.OGCGeometry GeometryN(int n)
		{
			return geometries[n];
		}

		public override string GeometryType()
		{
			return "GeometryCollection";
		}

		public override string AsText()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("GEOMETRYCOLLECTION ");
			if (Is3D())
			{
				sb.Append('Z');
			}
			if (IsMeasured())
			{
				sb.Append('M');
			}
			if (Is3D() || IsMeasured())
			{
				sb.Append(' ');
			}
			int n = NumGeometries();
			if (n == 0)
			{
				sb.Append("EMPTY");
				return sb.ToString();
			}
			sb.Append('(');
			for (int i = 0; i < n; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				sb.Append(GeometryN(i).AsText());
			}
			sb.Append(')');
			return sb.ToString();
		}

		public override System.IO.MemoryStream AsBinary()
		{
			System.Collections.Generic.List<System.IO.MemoryStream> buffers = new System.Collections.Generic.List<System.IO.MemoryStream>(0);
			int size = 9;
			int n = NumGeometries();
			for (int i = 0; i < n; i++)
			{
				System.IO.MemoryStream buffer = GeometryN(i).AsBinary();
				buffers.Add(buffer);
				size += buffer.Capacity();
			}
			System.IO.MemoryStream wkbBuffer = System.IO.MemoryStream.Allocate(size).Order(java.nio.ByteOrder.NativeOrder());
			byte byteOrder = unchecked((byte)(wkbBuffer.Order() == java.nio.ByteOrder.LITTLE_ENDIAN ? 1 : 0));
			int wkbType = 7;
			if (Is3D())
			{
				wkbType += 1000;
			}
			if (IsMeasured())
			{
				wkbType += 2000;
			}
			wkbBuffer.Put(0, byteOrder);
			wkbBuffer.PutInt(1, wkbType);
			wkbBuffer.PutInt(5, n);
			int offset = 9;
			for (int i_1 = 0; i_1 < n; i_1++)
			{
				byte[] arr = ((byte[])buffers[i_1].Array());
				System.Array.Copy(arr, 0, ((byte[])wkbBuffer.Array()), offset, arr.Length);
				offset += arr.Length;
			}
			return wkbBuffer;
		}

		public override string AsGeoJson()
		{
			return AsGeoJsonImpl(com.epl.geometry.GeoJsonExportFlags.geoJsonExportDefaults);
		}

		internal override string AsGeoJsonImpl(int export_flags)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("{\"type\":\"GeometryCollection\",\"geometries\":");
			sb.Append("[");
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				if (i > 0)
				{
					sb.Append(",");
				}
				if (GeometryN(i) != null)
				{
					sb.Append(GeometryN(i).AsGeoJsonImpl(com.epl.geometry.GeoJsonExportFlags.geoJsonExportSkipCRS));
				}
			}
			sb.Append("],\"crs\":");
			if (esriSR != null)
			{
				string crs_value = com.epl.geometry.OperatorExportToGeoJson.Local().ExportSpatialReference(0, esriSR);
				sb.Append(crs_value);
			}
			else
			{
				sb.Append("\"null\"");
			}
			sb.Append("}");
			return sb.ToString();
		}

		public override bool IsEmpty()
		{
			return NumGeometries() == 0;
		}

		public override double MinZ()
		{
			double z = double.NaN;
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				z = i == 0 ? GeometryN(i).MinZ() : System.Math.Min(GeometryN(i).MinZ(), z);
			}
			return z;
		}

		public override double MaxZ()
		{
			double z = double.NaN;
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				z = i == 0 ? GeometryN(i).MaxZ() : System.Math.Min(GeometryN(i).MaxZ(), z);
			}
			return z;
		}

		public override double MinMeasure()
		{
			double z = double.NaN;
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				z = i == 0 ? GeometryN(i).MinMeasure() : System.Math.Min(GeometryN(i).MinMeasure(), z);
			}
			return z;
		}

		public override double MaxMeasure()
		{
			double z = double.NaN;
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				z = i == 0 ? GeometryN(i).MaxMeasure() : System.Math.Min(GeometryN(i).MaxMeasure(), z);
			}
			return z;
		}

		public override bool IsSimple()
		{
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				if (!GeometryN(i).IsSimple())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>makeSimpleRelaxed is not supported for the GeometryCollection instance.</summary>
		public override com.epl.geometry.ogc.OGCGeometry MakeSimple()
		{
			throw new System.NotSupportedException();
		}

		public override bool IsSimpleRelaxed()
		{
			for (int i = 0, n = NumGeometries(); i < n; i++)
			{
				if (!GeometryN(i).IsSimpleRelaxed())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>makeSimpleRelaxed is not supported for the GeometryCollection instance.</summary>
		public override com.epl.geometry.ogc.OGCGeometry MakeSimpleRelaxed(bool forceProcessing)
		{
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.ogc.OGCGeometry LocateAlong(double mValue)
		{
			// TODO Auto-generated method stub
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.ogc.OGCGeometry LocateBetween(double mStart, double mEnd)
		{
			// TODO Auto-generated method stub
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.Geometry GetEsriGeometry()
		{
			return null;
		}

		public override com.epl.geometry.GeometryCursor GetEsriGeometryCursor()
		{
			return new com.epl.geometry.ogc.OGCConcreteGeometryCollection.GeometryCursorOGC(geometries);
		}

		protected internal override bool IsConcreteGeometryCollection()
		{
			return true;
		}

		internal class GeometryCursorOGC : com.epl.geometry.GeometryCursor
		{
			private int m_index;

			private int m_ind;

			private System.Collections.Generic.IList<com.epl.geometry.ogc.OGCGeometry> m_geoms;

			internal com.epl.geometry.GeometryCursor m_curs;

			internal GeometryCursorOGC(System.Collections.Generic.IList<com.epl.geometry.ogc.OGCGeometry> geoms)
			{
				m_geoms = geoms;
				m_index = -1;
				m_curs = null;
				m_ind = 0;
			}

			public override com.epl.geometry.Geometry Next()
			{
				while (true)
				{
					if (m_curs != null)
					{
						com.epl.geometry.Geometry g = m_curs.Next();
						if (g != null)
						{
							m_index++;
							return g;
						}
						m_curs = null;
					}
					if (m_ind >= m_geoms.Count)
					{
						return null;
					}
					int i = m_ind;
					m_ind++;
					if (m_geoms[i] == null)
					{
						continue;
					}
					// filter out nulls
					if (!m_geoms[i].IsConcreteGeometryCollection())
					{
						m_index++;
						return m_geoms[i].GetEsriGeometry();
					}
					else
					{
						com.epl.geometry.ogc.OGCConcreteGeometryCollection gc = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)m_geoms[i];
						m_curs = new com.epl.geometry.ogc.OGCConcreteGeometryCollection.GeometryCursorOGC(gc.geometries);
						return Next();
					}
				}
			}

			public override int GetGeometryID()
			{
				return m_index;
			}
		}

		internal System.Collections.Generic.IList<com.epl.geometry.ogc.OGCGeometry> geometries;

		public override void SetSpatialReference(com.epl.geometry.SpatialReference esriSR_)
		{
			esriSR = esriSR_;
			for (int i = 0, n = geometries.Count; i < n; i++)
			{
				if (geometries[i] != null)
				{
					geometries[i].SetSpatialReference(esriSR_);
				}
			}
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return this;
		}

		public override string AsJson()
		{
			throw new System.NotSupportedException();
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (other.GetType() != GetType())
			{
				return false;
			}
			com.epl.geometry.ogc.OGCConcreteGeometryCollection another = (com.epl.geometry.ogc.OGCConcreteGeometryCollection)other;
			if (geometries != null)
			{
				if (!geometries.Equals(another.geometries))
				{
					return false;
				}
			}
			else
			{
				if (another.geometries != null)
				{
					return false;
				}
			}
			if (esriSR == another.esriSR)
			{
				return true;
			}
			if (esriSR != null && another.esriSR != null)
			{
				return esriSR.Equals(another.esriSR);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 1;
			if (geometries != null)
			{
				hash = geometries.GetHashCode();
			}
			if (esriSR != null)
			{
				hash = com.epl.geometry.NumberUtils.HashCombine(hash, esriSR.GetHashCode());
			}
			return hash;
		}
	}
}
