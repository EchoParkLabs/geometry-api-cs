using Sharpen;

namespace com.epl.geometry.ogc
{
	public abstract class OGCCurve : com.epl.geometry.ogc.OGCGeometry
	{
		public abstract double Length();

		public abstract com.epl.geometry.ogc.OGCPoint StartPoint();

		public abstract com.epl.geometry.ogc.OGCPoint EndPoint();

		public abstract bool IsClosed();

		public virtual bool IsRing()
		{
			return IsSimple() && IsClosed();
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			if (IsClosed())
			{
				return new com.epl.geometry.ogc.OGCMultiPoint(new com.epl.geometry.MultiPoint(GetEsriGeometry().GetDescription()), esriSR);
			}
			else
			{
				// return empty multipoint;
				return new com.epl.geometry.ogc.OGCMultiPoint(StartPoint(), EndPoint());
			}
		}
	}
}
