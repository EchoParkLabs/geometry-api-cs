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
	internal sealed class RasterizedGeometry2DImpl : com.epl.geometry.RasterizedGeometry2D
	{
		internal int[] m_bitmap;

		internal int m_scanLineSize;

		internal int m_width;

		internal double m_dx;

		internal double m_dy;

		internal double m_x0;

		internal double m_y0;

		internal double m_toleranceXY;

		internal double m_stroke_half_widthX_pix;

		internal double m_stroke_half_widthY_pix;

		internal double m_stroke_half_width;

		internal com.epl.geometry.Envelope2D m_geomEnv;

		internal com.epl.geometry.Transformation2D m_transform;

		internal int m_dbgTestCount;

		internal com.epl.geometry.SimpleRasterizer m_rasterizer;

		internal com.epl.geometry.RasterizedGeometry2DImpl.ScanCallbackImpl m_callback;

		internal class ScanCallbackImpl : com.epl.geometry.SimpleRasterizer.ScanCallback
		{
			internal int[] m_bitmap;

			internal int m_scanlineWidth;

			internal int m_color;

			public ScanCallbackImpl(RasterizedGeometry2DImpl _enclosing, int[] bitmap, int scanlineWidth)
			{
				this._enclosing = _enclosing;
				// envelope of the raster in world coordinates
				this.m_scanlineWidth = scanlineWidth;
				this.m_bitmap = bitmap;
			}

			public virtual void SetColor(com.epl.geometry.SimpleRasterizer rasterizer, int color)
			{
				if (this.m_color != color)
				{
					rasterizer.Flush();
				}
				this.m_color = color;
			}

			// set new color
			public virtual void DrawScan(int[] scans, int scanCount3)
			{
				for (int i = 0; i < scanCount3; )
				{
					int x0 = scans[i++];
					int x1 = scans[i++];
					int y = scans[i++];
					int scanlineStart = y * this.m_scanlineWidth;
					for (int xx = x0; xx < x1; xx++)
					{
						this.m_bitmap[scanlineStart + (xx >> 4)] |= this.m_color << ((xx & 15) * 2);
					}
				}
			}

			private readonly RasterizedGeometry2DImpl _enclosing;
			// 2
			// bit
			// per
			// color
		}

		internal void FillMultiPath(com.epl.geometry.SimpleRasterizer rasterizer, com.epl.geometry.Transformation2D trans, com.epl.geometry.MultiPathImpl polygon, bool isWinding)
		{
			com.epl.geometry.SegmentIteratorImpl segIter = polygon.QuerySegmentIterator();
			com.epl.geometry.Point2D p1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D p2 = new com.epl.geometry.Point2D();
			while (segIter.NextPath())
			{
				while (segIter.HasNextSegment())
				{
					com.epl.geometry.Segment seg = segIter.NextSegment();
					if (seg.GetType() != com.epl.geometry.Geometry.Type.Line)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					// TODO:
					// densify
					// the
					// segment
					// here
					trans.Transform(seg.GetStartXY(), p1);
					trans.Transform(seg.GetEndXY(), p2);
					m_rasterizer.AddEdge(p1.x, p1.y, p2.x, p2.y);
				}
			}
			m_rasterizer.RenderEdges(isWinding ? com.epl.geometry.SimpleRasterizer.WINDING : com.epl.geometry.SimpleRasterizer.EVEN_ODD);
		}

		internal void FillPoints(com.epl.geometry.SimpleRasterizer rasterizer, com.epl.geometry.MultiPointImpl geom, double stroke_half_width)
		{
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal void FillConvexPolygon(com.epl.geometry.SimpleRasterizer rasterizer, com.epl.geometry.Point2D[] fan, int len)
		{
			for (int i = 1, n = len; i < n; i++)
			{
				rasterizer.AddEdge(fan[i - 1].x, fan[i - 1].y, fan[i].x, fan[i].y);
			}
			rasterizer.AddEdge(fan[len - 1].x, fan[len - 1].y, fan[0].x, fan[0].y);
			m_rasterizer.RenderEdges(com.epl.geometry.SimpleRasterizer.EVEN_ODD);
		}

		internal void FillEnvelope(com.epl.geometry.SimpleRasterizer rasterizer, com.epl.geometry.Envelope2D envIn)
		{
			rasterizer.FillEnvelope(envIn);
		}

		internal void StrokeDrawPolyPath(com.epl.geometry.SimpleRasterizer rasterizer, com.epl.geometry.MultiPathImpl polyPath, double tol)
		{
			com.epl.geometry.Point2D[] fan = new com.epl.geometry.Point2D[4];
			for (int i = 0; i < fan.Length; i++)
			{
				fan[i] = new com.epl.geometry.Point2D();
			}
			com.epl.geometry.SegmentIteratorImpl segIter = polyPath.QuerySegmentIterator();
			double strokeHalfWidth = m_transform.Transform(tol) + 1.5;
			double shortSegment = 0.25;
			com.epl.geometry.Point2D vec = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D vecA = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D vecB = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptStart = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptEnd = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D prev_start = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D prev_end = new com.epl.geometry.Point2D();
			double[] helper_xy_10_elm = new double[10];
			com.epl.geometry.Envelope2D segEnv = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Point2D ptOld = new com.epl.geometry.Point2D();
			while (segIter.NextPath())
			{
				bool hasFan = false;
				bool first = true;
				ptOld.SetCoords(0, 0);
				while (segIter.HasNextSegment())
				{
					com.epl.geometry.Segment seg = segIter.NextSegment();
					ptStart.x = seg.GetStartX();
					ptStart.y = seg.GetStartY();
					ptEnd.x = seg.GetEndX();
					ptEnd.y = seg.GetEndY();
					segEnv.SetEmpty();
					segEnv.Merge(ptStart.x, ptStart.y);
					segEnv.MergeNE(ptEnd.x, ptEnd.y);
					if (!m_geomEnv.IsIntersectingNE(segEnv))
					{
						if (hasFan)
						{
							rasterizer.StartAddingEdges();
							rasterizer.AddSegmentStroke(prev_start.x, prev_start.y, prev_end.x, prev_end.y, strokeHalfWidth, false, helper_xy_10_elm);
							rasterizer.RenderEdges(com.epl.geometry.SimpleRasterizer.EVEN_ODD);
							hasFan = false;
						}
						first = true;
						continue;
					}
					m_transform.Transform(ptEnd, ptEnd);
					if (first)
					{
						m_transform.Transform(ptStart, ptStart);
						ptOld.SetCoords(ptStart);
						first = false;
					}
					else
					{
						ptStart.SetCoords(ptOld);
					}
					prev_start.SetCoords(ptStart);
					prev_end.SetCoords(ptEnd);
					rasterizer.StartAddingEdges();
					hasFan = !rasterizer.AddSegmentStroke(prev_start.x, prev_start.y, prev_end.x, prev_end.y, strokeHalfWidth, true, helper_xy_10_elm);
					rasterizer.RenderEdges(com.epl.geometry.SimpleRasterizer.EVEN_ODD);
					if (!hasFan)
					{
						ptOld.SetCoords(prev_end);
					}
				}
				if (hasFan)
				{
					rasterizer.StartAddingEdges();
					hasFan = !rasterizer.AddSegmentStroke(prev_start.x, prev_start.y, prev_end.x, prev_end.y, strokeHalfWidth, false, helper_xy_10_elm);
					rasterizer.RenderEdges(com.epl.geometry.SimpleRasterizer.EVEN_ODD);
				}
			}
		}

		internal int WorldToPixX(double x)
		{
			return (int)(x * m_dx + m_x0);
		}

		internal int WorldToPixY(double y)
		{
			return (int)(y * m_dy + m_y0);
		}

		internal RasterizedGeometry2DImpl(com.epl.geometry.Geometry geom, double toleranceXY, int rasterSizeBytes)
		{
			// //_ASSERT(CanUseAccelerator(geom));
			Init((com.epl.geometry.MultiVertexGeometryImpl)geom._getImpl(), toleranceXY, rasterSizeBytes);
		}

		internal static com.epl.geometry.RasterizedGeometry2DImpl CreateImpl(com.epl.geometry.Geometry geom, double toleranceXY, int rasterSizeBytes)
		{
			com.epl.geometry.RasterizedGeometry2DImpl rgImpl = new com.epl.geometry.RasterizedGeometry2DImpl(geom, toleranceXY, rasterSizeBytes);
			return rgImpl;
		}

		private RasterizedGeometry2DImpl(com.epl.geometry.MultiVertexGeometryImpl geom, double toleranceXY, int rasterSizeBytes)
		{
			Init(geom, toleranceXY, rasterSizeBytes);
		}

		internal static com.epl.geometry.RasterizedGeometry2DImpl CreateImpl(com.epl.geometry.MultiVertexGeometryImpl geom, double toleranceXY, int rasterSizeBytes)
		{
			com.epl.geometry.RasterizedGeometry2DImpl rgImpl = new com.epl.geometry.RasterizedGeometry2DImpl(geom, toleranceXY, rasterSizeBytes);
			return rgImpl;
		}

		internal void Init(com.epl.geometry.MultiVertexGeometryImpl geom, double toleranceXY, int rasterSizeBytes)
		{
			// _ASSERT(CanUseAccelerator(geom));
			m_width = System.Math.Max((int)(System.Math.Sqrt(rasterSizeBytes) * 2 + 0.5), 64);
			m_scanLineSize = (m_width * 2 + 31) / 32;
			// 2 bits per pixel
			m_geomEnv = new com.epl.geometry.Envelope2D();
			m_toleranceXY = toleranceXY;
			// calculate bitmap size
			int size = 0;
			int width = m_width;
			int scanLineSize = m_scanLineSize;
			while (width >= 8)
			{
				size += width * scanLineSize;
				width /= 2;
				scanLineSize = (width * 2 + 31) / 32;
			}
			// allocate the bitmap, that contains the base and the mip-levels
			m_bitmap = new int[size];
			for (int i = 0; i < size; i++)
			{
				m_bitmap[i] = 0;
			}
			m_rasterizer = new com.epl.geometry.SimpleRasterizer();
			com.epl.geometry.RasterizedGeometry2DImpl.ScanCallbackImpl callback = new com.epl.geometry.RasterizedGeometry2DImpl.ScanCallbackImpl(this, m_bitmap, m_scanLineSize);
			m_callback = callback;
			m_rasterizer.Setup(m_width, m_width, callback);
			geom.QueryEnvelope2D(m_geomEnv);
			if (m_geomEnv.GetWidth() > m_width * m_geomEnv.GetHeight() || m_geomEnv.GetHeight() > m_geomEnv.GetWidth() * m_width)
			{
			}
			// the geometry is thin and the rasterizer is not needed.
			m_geomEnv.Inflate(toleranceXY, toleranceXY);
			com.epl.geometry.Envelope2D worldEnv = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D pixEnv = com.epl.geometry.Envelope2D.Construct(1, 1, m_width - 2, m_width - 2);
			double minWidth = toleranceXY * pixEnv.GetWidth();
			// min width is such
			// that the size of
			// one pixel is
			// equal to the
			// tolerance
			double minHeight = toleranceXY * pixEnv.GetHeight();
			worldEnv.SetCoords(m_geomEnv.GetCenter(), System.Math.Max(minWidth, m_geomEnv.GetWidth()), System.Math.Max(minHeight, m_geomEnv.GetHeight()));
			m_stroke_half_widthX_pix = worldEnv.GetWidth() / pixEnv.GetWidth();
			m_stroke_half_widthY_pix = worldEnv.GetHeight() / pixEnv.GetHeight();
			// The stroke half width. Later it will be inflated to account for
			// pixels size.
			m_stroke_half_width = m_toleranceXY;
			m_transform = new com.epl.geometry.Transformation2D();
			m_transform.InitializeFromRect(worldEnv, pixEnv);
			// geom to pixels
			com.epl.geometry.Transformation2D identityTransform = new com.epl.geometry.Transformation2D();
			switch (geom.GetType().Value())
			{
				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					callback.SetColor(m_rasterizer, 2);
					FillPoints(m_rasterizer, (com.epl.geometry.MultiPointImpl)geom, m_stroke_half_width);
					break;
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					callback.SetColor(m_rasterizer, 2);
					StrokeDrawPolyPath(m_rasterizer, (com.epl.geometry.MultiPathImpl)geom._getImpl(), m_stroke_half_width);
					break;
				}

				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					bool isWinding = false;
					// NOTE: change when winding is supported
					callback.SetColor(m_rasterizer, 1);
					FillMultiPath(m_rasterizer, m_transform, (com.epl.geometry.MultiPathImpl)geom, isWinding);
					callback.SetColor(m_rasterizer, 2);
					StrokeDrawPolyPath(m_rasterizer, (com.epl.geometry.MultiPathImpl)geom._getImpl(), m_stroke_half_width);
					break;
				}
			}
			m_dx = m_transform.xx;
			m_dy = m_transform.yy;
			m_x0 = m_transform.xd;
			m_y0 = m_transform.yd;
			BuildLevels();
		}

		//dbgSaveToBitmap("c:/temp/_dbg.bmp");
		internal bool TryRenderAsSmallEnvelope_(com.epl.geometry.Envelope2D env)
		{
			if (!env.IsIntersecting(m_geomEnv))
			{
				return true;
			}
			com.epl.geometry.Envelope2D envPix = new com.epl.geometry.Envelope2D();
			envPix.SetCoords(env);
			m_transform.Transform(env);
			double strokeHalfWidthPixX = m_stroke_half_widthX_pix;
			double strokeHalfWidthPixY = m_stroke_half_widthY_pix;
			if (envPix.GetWidth() > 2 * strokeHalfWidthPixX + 1 || envPix.GetHeight() > 2 * strokeHalfWidthPixY + 1)
			{
				return false;
			}
			// This envelope is too narrow/small, so that it can be just drawn as a
			// rectangle using only boundary color.
			envPix.Inflate(strokeHalfWidthPixX, strokeHalfWidthPixY);
			envPix.xmax += 1.0;
			envPix.ymax += 1.0;
			// take into account that it does not draw right and
			// bottom edges.
			m_callback.SetColor(m_rasterizer, 2);
			FillEnvelope(m_rasterizer, envPix);
			return true;
		}

		internal void BuildLevels()
		{
			m_rasterizer.Flush();
			int iStart = 0;
			int iStartNext = m_width * m_scanLineSize;
			int width = m_width;
			int widthNext = m_width / 2;
			int scanLineSize = m_scanLineSize;
			int scanLineSizeNext = (widthNext * 2 + 31) / 32;
			while (width > 8)
			{
				for (int iy = 0; iy < widthNext; iy++)
				{
					int iysrc1 = iy * 2;
					int iysrc2 = iy * 2 + 1;
					for (int ix = 0; ix < widthNext; ix++)
					{
						int ixsrc1 = ix * 2;
						int ixsrc2 = ix * 2 + 1;
						int divix1 = ixsrc1 >> 4;
						int modix1 = (ixsrc1 & 15) * 2;
						int divix2 = ixsrc2 >> 4;
						int modix2 = (ixsrc2 & 15) * 2;
						int res = (m_bitmap[iStart + scanLineSize * iysrc1 + divix1] >> modix1) & 3;
						res |= (m_bitmap[iStart + scanLineSize * iysrc1 + divix2] >> modix2) & 3;
						res |= (m_bitmap[iStart + scanLineSize * iysrc2 + divix1] >> modix1) & 3;
						res |= (m_bitmap[iStart + scanLineSize * iysrc2 + divix2] >> modix2) & 3;
						int divixDst = ix >> 4;
						int modixDst = (ix & 15) * 2;
						m_bitmap[iStartNext + scanLineSizeNext * iy + divixDst] |= res << modixDst;
					}
				}
				width = widthNext;
				scanLineSize = scanLineSizeNext;
				iStart = iStartNext;
				widthNext = width / 2;
				scanLineSizeNext = (widthNext * 2 + 31) / 32;
				iStartNext = iStart + scanLineSize * width;
			}
		}

		public override com.epl.geometry.RasterizedGeometry2D.HitType QueryPointInGeometry(double x, double y)
		{
			if (!m_geomEnv.Contains(x, y))
			{
				return com.epl.geometry.RasterizedGeometry2D.HitType.Outside;
			}
			int ix = WorldToPixX(x);
			int iy = WorldToPixY(y);
			if (ix < 0 || ix >= m_width || iy < 0 || iy >= m_width)
			{
				return com.epl.geometry.RasterizedGeometry2D.HitType.Outside;
			}
			int divix = ix >> 4;
			int modix = (ix & 15) * 2;
			int res = (m_bitmap[m_scanLineSize * iy + divix] >> modix) & 3;
			if (res == 0)
			{
				return com.epl.geometry.RasterizedGeometry2D.HitType.Outside;
			}
			else
			{
				if (res == 1)
				{
					return com.epl.geometry.RasterizedGeometry2D.HitType.Inside;
				}
				else
				{
					return com.epl.geometry.RasterizedGeometry2D.HitType.Border;
				}
			}
		}

		public override com.epl.geometry.RasterizedGeometry2D.HitType QueryEnvelopeInGeometry(com.epl.geometry.Envelope2D env)
		{
			if (!env.Intersect(m_geomEnv))
			{
				return com.epl.geometry.RasterizedGeometry2D.HitType.Outside;
			}
			int ixmin = WorldToPixX(env.xmin);
			int ixmax = WorldToPixX(env.xmax);
			int iymin = WorldToPixY(env.ymin);
			int iymax = WorldToPixY(env.ymax);
			if (ixmin < 0)
			{
				ixmin = 0;
			}
			if (iymin < 0)
			{
				iymin = 0;
			}
			if (ixmax >= m_width)
			{
				ixmax = m_width - 1;
			}
			if (iymax >= m_width)
			{
				iymax = m_width - 1;
			}
			if (ixmin > ixmax || iymin > iymax)
			{
				return com.epl.geometry.RasterizedGeometry2D.HitType.Outside;
			}
			int area = System.Math.Max(ixmax - ixmin, 1) * System.Math.Max(iymax - iymin, 1);
			int iStart = 0;
			int scanLineSize = m_scanLineSize;
			int width = m_width;
			int res = 0;
			while (true)
			{
				if (area < 32 || width < 16)
				{
					for (int iy = iymin; iy <= iymax; iy++)
					{
						for (int ix = ixmin; ix <= ixmax; ix++)
						{
							int divix = ix >> 4;
							int modix = (ix & 15) * 2;
							res = (m_bitmap[iStart + scanLineSize * iy + divix] >> modix) & 3;
							// read
							// two
							// bit
							// color.
							if (res > 1)
							{
								return com.epl.geometry.RasterizedGeometry2D.HitType.Border;
							}
						}
					}
					if (res == 0)
					{
						return com.epl.geometry.RasterizedGeometry2D.HitType.Outside;
					}
					else
					{
						if (res == 1)
						{
							return com.epl.geometry.RasterizedGeometry2D.HitType.Inside;
						}
					}
				}
				iStart += scanLineSize * width;
				width /= 2;
				scanLineSize = (width * 2 + 31) / 32;
				ixmin /= 2;
				iymin /= 2;
				ixmax /= 2;
				iymax /= 2;
				area = System.Math.Max(ixmax - ixmin, 1) * System.Math.Max(iymax - iymin, 1);
			}
		}

		public override double GetToleranceXY()
		{
			return m_toleranceXY;
		}

		public override int GetRasterSize()
		{
			return m_width * m_scanLineSize;
		}

		public override bool DbgSaveToBitmap(string fileName)
		{
			try
			{
				System.IO.BinaryWriter byteBuffer = new System.IO.BinaryWriter(System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate));
				int height = m_width;
				int width = m_width;
				int sz = 14 + 40 + 4 * m_width * height;
				// Write the BITMAPFILEHEADER
				//System.IO.MemoryStream byteBuffer = System.IO.MemoryStream.Allocate(sz);
				//byteBuffer.Write(java.nio.ByteOrder.LITTLE_ENDIAN);
				// byteBuffer.put((byte) 'M');
				byteBuffer.Write(unchecked((byte)66));
				byteBuffer.Write(unchecked((byte)77));
				// fwrite("BM", 1, 2, f); //bfType
				byteBuffer.Write(sz);
				// fwrite(&sz, 1, 4, f);//bfSize
				short zero16 = 0;
				byteBuffer.Write(zero16);
				// fwrite(&zero16, 1, 2, f);//bfReserved1
				byteBuffer.Write(zero16);
				// fwrite(&zero16, 1, 2, f);//bfReserved2
				int offset = 14 + 40;
				byteBuffer.Write(offset);
				// fwrite(&offset, 1, 4, f);//bfOffBits
				// Write the BITMAPINFOHEADER
				int biSize = 40;
				int biWidth = width;
				int biHeight = -height;
				short biPlanes = 1;
				short biBitCount = 32;
				int biCompression = 0;
				int biSizeImage = 4 * width * height;
				int biXPelsPerMeter = 0;
				int biYPelsPerMeter = 0;
				int biClrUsed = 0;
				int biClrImportant = 0;
				byteBuffer.Write(biSize);
				byteBuffer.Write(biWidth);
				byteBuffer.Write(biHeight);
				byteBuffer.Write(biPlanes);
				byteBuffer.Write(biBitCount);
				byteBuffer.Write(biCompression);
				byteBuffer.Write(biSizeImage);
				byteBuffer.Write(biXPelsPerMeter);
				byteBuffer.Write(biYPelsPerMeter);
				byteBuffer.Write(biClrUsed);
				byteBuffer.Write(biClrImportant);
				int[] colors = new int[] { unchecked((int)(0xFFFFFFFF)), unchecked((int)(0xFF000000)), unchecked((int)(0xFFFF0000)), unchecked((int)(0xFF00FF00)) };
				// int32_t* rgb4 = (int32_t*)malloc(biSizeImage);
				for (int y = 0; y < height; y++)
				{
					int scanlineIn = y * ((width * 2 + 31) / 32);
					int scanlineOut = offset + width * y;
					for (int x = 0; x < width; x++)
					{
						int res = (m_bitmap[scanlineIn + (x >> 4)] >> ((x & 15) * 2)) & 3;
						byteBuffer.Write(colors[res]);
					}
				}
				//byte[] b = ((byte[])byteBuffer.Write());
				//
				byteBuffer.Close();
				return true;
			}
			catch (System.IO.IOException)
			{
				return false;
			}
		}
	}
}
