using Sharpen;

namespace com.epl.geometry.ogc
{
	public abstract class OGCGeometryCollection : com.epl.geometry.ogc.OGCGeometry
	{
		/// <summary>Returns the number of geometries in this GeometryCollection.</summary>
		public abstract int NumGeometries();

		/// <summary>Returns the Nth geometry in this GeometryCollection.</summary>
		/// <param name="n">The 0 based index of the geometry.</param>
		public abstract com.epl.geometry.ogc.OGCGeometry GeometryN(int n);
	}
}
