/*
Copyright 2017 Echo Park Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

For additional information, contact:

email: info@echoparklabs.io
*/


namespace com.epl.geometry
{
	public abstract class RasterizedGeometry2D
	{
		[System.Serializable]
		public sealed class HitType
		{
			public static readonly com.epl.geometry.RasterizedGeometry2D.HitType Outside = new com.epl.geometry.RasterizedGeometry2D.HitType(0);

			public static readonly com.epl.geometry.RasterizedGeometry2D.HitType Inside = new com.epl.geometry.RasterizedGeometry2D.HitType(1);

			public static readonly com.epl.geometry.RasterizedGeometry2D.HitType Border = new com.epl.geometry.RasterizedGeometry2D.HitType(2);

			internal int enumVal;

			private HitType(int val)
			{
				// the test geometry is well outside the geometry bounds
				// the test geometry is well inside the geomety bounds
				// the test geometry is close to the bounds or intersects the
				// bounds
				enumVal = val;
			}
		}

		/// <summary>Test a point against the RasterizedGeometry</summary>
		public abstract com.epl.geometry.RasterizedGeometry2D.HitType QueryPointInGeometry(double x, double y);

		/// <summary>Test an envelope against the RasterizedGeometry.</summary>
		public abstract com.epl.geometry.RasterizedGeometry2D.HitType QueryEnvelopeInGeometry(com.epl.geometry.Envelope2D env);

		/// <summary>Creates a rasterized geometry from a given Geometry.</summary>
		/// <param name="geom">The input geometry to rasterize. It has to be a MultiVertexGeometry instance.</param>
		/// <param name="toleranceXY">
		/// The tolerance of the rasterization. Raster pixels that are
		/// closer than given tolerance to the Geometry will be set.
		/// </param>
		/// <param name="rasterSizeBytes">
		/// The max size of the raster in bytes. The raster has size of
		/// rasterSize x rasterSize. Polygons are rasterized into 2 bpp
		/// (bits per pixel) rasters while other geometries are rasterized
		/// into 1 bpp rasters. 32x32 pixel raster for a polygon would
		/// take 256 bytes of memory
		/// </param>
		public static com.epl.geometry.RasterizedGeometry2D Create(com.epl.geometry.Geometry geom, double toleranceXY, int rasterSizeBytes)
		{
			if (!CanUseAccelerator(geom))
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.RasterizedGeometry2DImpl gc = com.epl.geometry.RasterizedGeometry2DImpl.CreateImpl(geom, toleranceXY, rasterSizeBytes);
			return (com.epl.geometry.RasterizedGeometry2D)gc;
		}

		internal static com.epl.geometry.RasterizedGeometry2D Create(com.epl.geometry.MultiVertexGeometryImpl geom, double toleranceXY, int rasterSizeBytes)
		{
			if (!CanUseAccelerator(geom))
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.RasterizedGeometry2DImpl gc = com.epl.geometry.RasterizedGeometry2DImpl.CreateImpl(geom, toleranceXY, rasterSizeBytes);
			return (com.epl.geometry.RasterizedGeometry2D)gc;
		}

		public static int RasterSizeFromAccelerationDegree(com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			int value = 0;
			switch (accelDegree)
			{
				case com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMild:
				{
					value = 64 * 64 * 2 / 8;
					// 1k
					break;
				}

				case com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium:
				{
					value = 256 * 256 * 2 / 8;
					// 16k
					break;
				}

				case com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot:
				{
					value = 1024 * 1024 * 2 / 8;
					// 256k
					break;
				}

				default:
				{
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
			return value;
		}

		/// <summary>
		/// Checks whether the RasterizedGeometry2D accelerator can be used with the
		/// given geometry.
		/// </summary>
		internal static bool CanUseAccelerator(com.epl.geometry.Geometry geom)
		{
			if (geom.IsEmpty() || !(geom.GetType() == com.epl.geometry.Geometry.Type.Polyline || geom.GetType() == com.epl.geometry.Geometry.Type.Polygon))
			{
				return false;
			}
			return true;
		}

		/// <summary>Returns the tolerance for which the rasterized Geometry has been built.</summary>
		public abstract double GetToleranceXY();

		/// <summary>Returns raster size in bytes</summary>
		public abstract int GetRasterSize();

		/// <summary>Dumps the raster to a bmp file for debug purposes.</summary>
		/// <param name="fileName"/>
		/// <returns>true if success, false otherwise.</returns>
		public abstract bool DbgSaveToBitmap(string fileName);
	}
}
