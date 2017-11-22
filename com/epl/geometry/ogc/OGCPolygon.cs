using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCPolygon : com.epl.geometry.ogc.OGCSurface
	{
		public OGCPolygon(com.epl.geometry.Polygon src, int exteriorRing, com.epl.geometry.SpatialReference sr)
		{
			polygon = new com.epl.geometry.Polygon();
			for (int i = exteriorRing, n = src.GetPathCount(); i < n; i++)
			{
				if (i > exteriorRing && src.IsExteriorRing(i))
				{
					break;
				}
				polygon.AddPath(src, i, true);
			}
			esriSR = sr;
		}

		public OGCPolygon(com.epl.geometry.Polygon geom, com.epl.geometry.SpatialReference sr)
		{
			polygon = geom;
			if (geom.GetExteriorRingCount() > 1)
			{
				throw new System.ArgumentException("Polygon has to have one exterior ring. Simplify geom with OperatorSimplify.");
			}
			esriSR = sr;
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportPolygon);
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportPolygon, GetEsriGeometry(), null);
		}

		/// <summary>Returns the exterior ring of this Polygon.</summary>
		/// <returns>OGCLinearRing instance.</returns>
		public virtual com.epl.geometry.ogc.OGCLineString ExteriorRing()
		{
			if (polygon.IsEmpty())
			{
				return new com.epl.geometry.ogc.OGCLinearRing((com.epl.geometry.Polygon)polygon.CreateInstance(), 0, esriSR, true);
			}
			return new com.epl.geometry.ogc.OGCLinearRing(polygon, 0, esriSR, true);
		}

		/// <summary>Returns the number of interior rings in this Polygon.</summary>
		public virtual int NumInteriorRing()
		{
			return polygon.GetPathCount() - 1;
		}

		/// <summary>Returns the Nth interior ring for this Polygon as a LineString.</summary>
		/// <param name="n">The 0 based index of the interior ring.</param>
		/// <returns>OGCLinearRing instance.</returns>
		public virtual com.epl.geometry.ogc.OGCLineString InteriorRingN(int n)
		{
			return new com.epl.geometry.ogc.OGCLinearRing(polygon, n + 1, esriSR, true);
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.Add(polygon, true);
			// adds reversed path
			return (com.epl.geometry.ogc.OGCMultiCurve)com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(polyline, esriSR, true);
		}

		public override string GeometryType()
		{
			return "Polygon";
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
			return new com.epl.geometry.ogc.OGCMultiPolygon(polygon, esriSR);
		}

		internal com.epl.geometry.Polygon polygon;
	}
}
