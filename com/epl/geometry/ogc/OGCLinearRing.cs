using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCLinearRing : com.epl.geometry.ogc.OGCLineString
	{
		public OGCLinearRing(com.epl.geometry.MultiPath mp, int pathIndex, com.epl.geometry.SpatialReference sr, bool reversed)
			: base(mp, pathIndex, sr, reversed)
		{
			if (!mp.IsClosedPath(0))
			{
				throw new System.ArgumentException("LinearRing path must be closed");
			}
		}

		public override int NumPoints()
		{
			if (multiPath.IsEmpty())
			{
				return 0;
			}
			return multiPath.GetPointCount() + 1;
		}

		public override bool IsClosed()
		{
			return true;
		}

		public override bool IsRing()
		{
			return true;
		}

		public override com.epl.geometry.ogc.OGCPoint PointN(int n)
		{
			int nn;
			if (n == multiPath.GetPathSize(0))
			{
				nn = multiPath.GetPathStart(0);
			}
			else
			{
				nn = multiPath.GetPathStart(0) + n;
			}
			return (com.epl.geometry.ogc.OGCPoint)com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(multiPath.GetPoint(nn), esriSR);
		}
	}
}
