using Sharpen;

namespace com.epl.geometry.ogc
{
	public abstract class OGCMultiCurve : com.epl.geometry.ogc.OGCGeometryCollection
	{
		public override int NumGeometries()
		{
			com.epl.geometry.MultiPath mp = (com.epl.geometry.MultiPath)GetEsriGeometry();
			return mp.GetPathCount();
		}

		public virtual bool IsClosed()
		{
			com.epl.geometry.MultiPath mp = (com.epl.geometry.MultiPath)GetEsriGeometry();
			for (int i = 0, n = mp.GetPathCount(); i < n; i++)
			{
				if (!mp.IsClosedPathInXYPlane(i))
				{
					return false;
				}
			}
			return true;
		}

		public virtual double Length()
		{
			return GetEsriGeometry().CalculateLength2D();
		}
	}
}
