using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCMultiPolygon : com.epl.geometry.ogc.OGCMultiSurface
	{
		public OGCMultiPolygon(com.epl.geometry.Polygon src, com.epl.geometry.SpatialReference sr)
		{
			polygon = src;
			esriSR = sr;
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportMultiPolygon);
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon, GetEsriGeometry(), null);
		}

		public override string AsGeoJson()
		{
			com.epl.geometry.OperatorExportToGeoJson op = (com.epl.geometry.OperatorExportToGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			return op.Execute(com.epl.geometry.GeoJsonExportFlags.geoJsonExportPreferMultiGeometry, null, GetEsriGeometry());
		}

		public override int NumGeometries()
		{
			return polygon.GetExteriorRingCount();
		}

		public override com.epl.geometry.ogc.OGCGeometry GeometryN(int n)
		{
			int exterior = 0;
			for (int i = 0; i < polygon.GetPathCount(); i++)
			{
				if (polygon.IsExteriorRing(i))
				{
					exterior++;
				}
				if (exterior == n + 1)
				{
					return new com.epl.geometry.ogc.OGCPolygon(polygon, i, esriSR);
				}
			}
			throw new System.ArgumentException("geometryN: n out of range");
		}

		public override string GeometryType()
		{
			return "MultiPolygon";
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.Add(polygon, true);
			// adds reversed path
			return (com.epl.geometry.ogc.OGCMultiCurve)com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(polyline, esriSR, true);
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
			return polygon;
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return this;
		}

		internal com.epl.geometry.Polygon polygon;
	}
}
