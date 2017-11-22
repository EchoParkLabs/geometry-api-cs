using Sharpen;

namespace com.epl.geometry.ogc
{
	public sealed class OGCPoint : com.epl.geometry.ogc.OGCGeometry
	{
		public OGCPoint(com.epl.geometry.Point pt, com.epl.geometry.SpatialReference sr)
		{
			point = pt;
			esriSR = sr;
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportPoint);
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportPoint, GetEsriGeometry(), null);
		}

		public double X()
		{
			return point.GetX();
		}

		public double Y()
		{
			return point.GetY();
		}

		public double Z()
		{
			return point.GetZ();
		}

		public double M()
		{
			return point.GetM();
		}

		public override string GeometryType()
		{
			return "Point";
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			return new com.epl.geometry.ogc.OGCMultiPoint(new com.epl.geometry.MultiPoint(GetEsriGeometry().GetDescription()), esriSR);
		}

		// return empty point
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
			return point;
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return new com.epl.geometry.ogc.OGCMultiPoint(point, esriSR);
		}

		internal com.epl.geometry.Point point;
	}
}
