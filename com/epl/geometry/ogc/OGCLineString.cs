using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCLineString : com.epl.geometry.ogc.OGCCurve
	{
		/// <summary>The number of Points in this LineString.</summary>
		public virtual int NumPoints()
		{
			if (multiPath.IsEmpty())
			{
				return 0;
			}
			int d = multiPath.IsClosedPath(0) ? 1 : 0;
			return multiPath.GetPointCount() + d;
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportLineString);
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportLineString, GetEsriGeometry(), null);
		}

		/// <summary>Returns the specified Point N in this LineString.</summary>
		/// <param name="n">The 0 based index of the Point.</param>
		public virtual com.epl.geometry.ogc.OGCPoint PointN(int n)
		{
			int nn;
			if (multiPath.IsClosedPath(0) && n == multiPath.GetPathSize(0))
			{
				nn = multiPath.GetPathStart(0);
			}
			else
			{
				nn = n + multiPath.GetPathStart(0);
			}
			return (com.epl.geometry.ogc.OGCPoint)com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(multiPath.GetPoint(nn), esriSR);
		}

		public override bool IsClosed()
		{
			return multiPath.IsClosedPathInXYPlane(0);
		}

		public OGCLineString(com.epl.geometry.MultiPath mp, int pathIndex, com.epl.geometry.SpatialReference sr)
		{
			multiPath = new com.epl.geometry.Polyline();
			if (!mp.IsEmpty())
			{
				multiPath.AddPath(mp, pathIndex, true);
			}
			esriSR = sr;
		}

		public OGCLineString(com.epl.geometry.MultiPath mp, int pathIndex, com.epl.geometry.SpatialReference sr, bool reversed)
		{
			multiPath = new com.epl.geometry.Polyline();
			if (!mp.IsEmpty())
			{
				multiPath.AddPath(mp, pathIndex, !reversed);
			}
			esriSR = sr;
		}

		public override double Length()
		{
			return multiPath.CalculateLength2D();
		}

		public override com.epl.geometry.ogc.OGCPoint StartPoint()
		{
			return PointN(0);
		}

		public override com.epl.geometry.ogc.OGCPoint EndPoint()
		{
			return PointN(NumPoints() - 1);
		}

		public override string GeometryType()
		{
			return "LineString";
		}

		public override com.epl.geometry.ogc.OGCGeometry LocateAlong(double mValue)
		{
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.ogc.OGCGeometry LocateBetween(double mStart, double mEnd)
		{
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.Geometry GetEsriGeometry()
		{
			return multiPath;
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return new com.epl.geometry.ogc.OGCMultiLineString((com.epl.geometry.Polyline)multiPath, esriSR);
		}

		internal com.epl.geometry.MultiPath multiPath;
	}
}
