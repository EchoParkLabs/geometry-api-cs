/*
Copyright 2017-2021 David Raleigh

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

email: davidraleigh@gmail.com
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestRasterizedGeometry2D : NUnit.Framework.TestFixtureAttribute
	{
		internal virtual bool RgHelper(com.epl.geometry.RasterizedGeometry2D rg, com.epl.geometry.MultiPath mp)
		{
			com.epl.geometry.SegmentIterator iter = mp.QuerySegmentIterator();
			while (iter.NextPath())
			{
				while (iter.HasNextSegment())
				{
					com.epl.geometry.Segment seg = iter.NextSegment();
					int count = 20;
					for (int i = 0; i < count; i++)
					{
						double t = (1.0 * i / count);
						com.epl.geometry.Point2D pt = seg.GetCoord2D(t);
						com.epl.geometry.RasterizedGeometry2D.HitType hit = rg.QueryPointInGeometry(pt.x, pt.y);
						if (hit != com.epl.geometry.RasterizedGeometry2D.HitType.Border)
						{
							return false;
						}
					}
				}
			}
			if (mp.GetType() != com.epl.geometry.Geometry.Type.Polygon)
			{
				return true;
			}
			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)mp;
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			poly.QueryEnvelope2D(env);
			int count_1 = 100;
			for (int iy = 0; iy < count_1; iy++)
			{
				double ty = 1.0 * iy / count_1;
				double y = env.ymin * (1.0 - ty) + ty * env.ymax;
				for (int ix = 0; ix < count_1; ix++)
				{
					double tx = 1.0 * ix / count_1;
					double x = env.xmin * (1.0 - tx) + tx * env.xmax;
					com.epl.geometry.RasterizedGeometry2D.HitType hit = rg.QueryPointInGeometry(x, y);
					com.epl.geometry.PolygonUtils.PiPResult res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(poly, new com.epl.geometry.Point2D(x, y), 0);
					if (res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
					{
						bool bgood = (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Border || hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside);
						if (!bgood)
						{
							return false;
						}
					}
					else
					{
						if (res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
						{
							bool bgood = (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Border || hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside);
							if (!bgood)
							{
								return false;
							}
						}
						else
						{
							bool bgood = (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Border);
							if (!bgood)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		[NUnit.Framework.Test]
		public virtual void Test()
		{
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(100, 10);
				poly.LineTo(100, 100);
				poly.LineTo(10, 100);
				// create using move semantics. Usually we do not use this
				// approach.
				com.epl.geometry.RasterizedGeometry2D rg = com.epl.geometry.RasterizedGeometry2D.Create(poly, 0, 1024);
				//rg.dbgSaveToBitmap("c:/temp/_dbg.bmp");
				com.epl.geometry.RasterizedGeometry2D.HitType res;
				res = rg.QueryPointInGeometry(7, 10);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Outside);
				res = rg.QueryPointInGeometry(10, 10);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Border);
				res = rg.QueryPointInGeometry(50, 50);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Inside);
				NUnit.Framework.Assert.IsTrue(RgHelper(rg, poly));
			}
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				// create a star (non-simple)
				poly.StartPath(1, 0);
				poly.LineTo(5, 10);
				poly.LineTo(9, 0);
				poly.LineTo(0, 6);
				poly.LineTo(10, 6);
				com.epl.geometry.RasterizedGeometry2D rg = com.epl.geometry.RasterizedGeometry2D.Create(poly, 0, 1024);
				//rg.dbgSaveToBitmap("c:/temp/_dbg.bmp");
				com.epl.geometry.RasterizedGeometry2D.HitType res;
				res = rg.QueryPointInGeometry(5, 5.5);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Outside);
				res = rg.QueryPointInGeometry(5, 8);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Inside);
				res = rg.QueryPointInGeometry(1.63, 0.77);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Inside);
				res = rg.QueryPointInGeometry(1, 3);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Outside);
				res = rg.QueryPointInGeometry(1.6, 0.1);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.RasterizedGeometry2D.HitType.Outside);
				NUnit.Framework.Assert.IsTrue(RgHelper(rg, poly));
			}
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				// create a star (non-simple)
				poly.StartPath(1, 0);
				poly.LineTo(5, 10);
				poly.LineTo(9, 0);
				poly.LineTo(0, 6);
				poly.LineTo(10, 6);
				com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
				poly = (com.epl.geometry.Polygon)com.epl.geometry.OperatorSimplify.Local().Execute(poly, sr, true, null);
				com.epl.geometry.OperatorContains.Local().AccelerateGeometry(poly, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
				NUnit.Framework.Assert.IsFalse(com.epl.geometry.OperatorContains.Local().Execute(poly, new com.epl.geometry.Point(5, 5.5), sr, null));
				NUnit.Framework.Assert.IsTrue(com.epl.geometry.OperatorContains.Local().Execute(poly, new com.epl.geometry.Point(5, 8), sr, null));
				NUnit.Framework.Assert.IsTrue(com.epl.geometry.OperatorContains.Local().Execute(poly, new com.epl.geometry.Point(1.63, 0.77), sr, null));
				NUnit.Framework.Assert.IsFalse(com.epl.geometry.OperatorContains.Local().Execute(poly, new com.epl.geometry.Point(1, 3), sr, null));
				NUnit.Framework.Assert.IsFalse(com.epl.geometry.OperatorContains.Local().Execute(poly, new com.epl.geometry.Point(1.6, 0.1), sr, null));
			}
		}
		/*
		{
		Geometry g = OperatorFactoryLocal.loadGeometryFromEsriShapeDbg("c:/temp/_poly_final.bin");
		RasterizedGeometry2D rg1 = RasterizedGeometry2D
		.create(g, 0, 1024);//warmup
		rg1 = null;
		
		long t0 = System.nanoTime();
		RasterizedGeometry2D rg = RasterizedGeometry2D
		.create(g, 0, 1024 * 1024);
		long t1 = System.nanoTime();
		double d = (t1 - t0) / 1000000.0;
		System.out.printf("Time to rasterize the geometry: %f", d);
		
		rg.dbgSaveToBitmap("c:/temp/_dbg.bmp");
		for (;;){}
		}*/
	}
}
