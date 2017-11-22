using Sharpen;

namespace com.epl.geometry.ogc
{
	public abstract class OGCSurface : com.epl.geometry.ogc.OGCGeometry
	{
		public virtual double Area()
		{
			return GetEsriGeometry().CalculateArea2D();
		}

		public virtual com.epl.geometry.ogc.OGCPoint Centroid()
		{
			// TODO: implement me;
			throw new System.NotSupportedException();
		}

		public virtual com.epl.geometry.ogc.OGCPoint PointOnSurface()
		{
			// TODO: support this (need to port OperatorLabelPoint)
			throw new System.NotSupportedException();
		}

		public abstract override com.epl.geometry.ogc.OGCGeometry Boundary();
	}
}
