using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCMultiPoint : com.epl.geometry.ogc.OGCGeometryCollection
	{
		public override int NumGeometries()
		{
			return multiPoint.GetPointCount();
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportMultiPoint);
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiPoint, GetEsriGeometry(), null);
		}

		public override com.epl.geometry.ogc.OGCGeometry GeometryN(int n)
		{
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(multiPoint.GetPoint(n), esriSR);
		}

		public override string GeometryType()
		{
			return "MultiPoint";
		}

		/// <param name="mp">MultiPoint instance will be referenced by this OGC class</param>
		public OGCMultiPoint(com.epl.geometry.MultiPoint mp, com.epl.geometry.SpatialReference sr)
		{
			multiPoint = mp;
			esriSR = sr;
		}

		public OGCMultiPoint(com.epl.geometry.Point startPoint, com.epl.geometry.SpatialReference sr)
		{
			multiPoint = new com.epl.geometry.MultiPoint();
			multiPoint.Add((com.epl.geometry.Point)startPoint);
			esriSR = sr;
		}

		public OGCMultiPoint(com.epl.geometry.ogc.OGCPoint startPoint, com.epl.geometry.ogc.OGCPoint endPoint)
		{
			multiPoint = new com.epl.geometry.MultiPoint();
			multiPoint.Add((com.epl.geometry.Point)startPoint.GetEsriGeometry());
			multiPoint.Add((com.epl.geometry.Point)endPoint.GetEsriGeometry());
			esriSR = startPoint.esriSR;
		}

		public OGCMultiPoint(com.epl.geometry.SpatialReference sr)
		{
			esriSR = sr;
			multiPoint = new com.epl.geometry.MultiPoint();
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			return new com.epl.geometry.ogc.OGCMultiPoint((com.epl.geometry.MultiPoint)multiPoint.CreateInstance(), esriSR);
		}

		// return empty multipoint
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
			return multiPoint;
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return this;
		}

		private com.epl.geometry.MultiPoint multiPoint;
	}
}
