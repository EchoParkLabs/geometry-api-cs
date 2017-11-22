using Sharpen;

namespace com.epl.geometry.ogc
{
	public abstract class OGCMultiSurface : com.epl.geometry.ogc.OGCGeometryCollection
	{
		public virtual double Area()
		{
			return GetEsriGeometry().CalculateArea2D();
		}

		public virtual com.epl.geometry.ogc.OGCPoint Centroid()
		{
			// TODO
			throw new System.NotSupportedException();
		}

		public virtual com.epl.geometry.ogc.OGCPoint PointOnSurface()
		{
			// TODO
			throw new System.NotSupportedException();
		}
	}
}
