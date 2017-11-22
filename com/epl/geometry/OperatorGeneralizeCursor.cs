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
	internal sealed class OperatorGeneralizeCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.ProgressTracker m_progressTracker;

		internal com.epl.geometry.GeometryCursor m_geoms;

		internal double m_maxDeviation;

		internal bool m_bRemoveDegenerateParts;

		public OperatorGeneralizeCursor(com.epl.geometry.GeometryCursor geoms, double maxDeviation, bool bRemoveDegenerateParts, com.epl.geometry.ProgressTracker progressTracker)
		{
			m_geoms = geoms;
			m_maxDeviation = maxDeviation;
			m_progressTracker = progressTracker;
			m_bRemoveDegenerateParts = bRemoveDegenerateParts;
		}

		public override com.epl.geometry.Geometry Next()
		{
			// TODO Auto-generated method stub
			com.epl.geometry.Geometry geom = m_geoms.Next();
			if (geom == null)
			{
				return null;
			}
			return Generalize(geom);
		}

		public override int GetGeometryID()
		{
			// TODO Auto-generated method stub
			return m_geoms.GetGeometryID();
		}

		private com.epl.geometry.Geometry Generalize(com.epl.geometry.Geometry geom)
		{
			com.epl.geometry.Geometry.Type gt = geom.GetType();
			if (com.epl.geometry.Geometry.IsPoint(gt.Value()))
			{
				return geom;
			}
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon(geom.GetDescription());
				poly.AddEnvelope((com.epl.geometry.Envelope)geom, false);
				return Generalize(poly);
			}
			if (geom.IsEmpty())
			{
				return geom;
			}
			com.epl.geometry.MultiPath mp = (com.epl.geometry.MultiPath)geom;
			if (mp == null)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			com.epl.geometry.MultiPath dstmp = (com.epl.geometry.MultiPath)geom.CreateInstance();
			com.epl.geometry.Line line = new com.epl.geometry.Line();
			for (int ipath = 0, npath = mp.GetPathCount(); ipath < npath; ipath++)
			{
				GeneralizePath((com.epl.geometry.MultiPathImpl)mp._getImpl(), ipath, (com.epl.geometry.MultiPathImpl)dstmp._getImpl(), line);
			}
			return dstmp;
		}

		private void GeneralizePath(com.epl.geometry.MultiPathImpl mpsrc, int ipath, com.epl.geometry.MultiPathImpl mpdst, com.epl.geometry.Line lineHelper)
		{
			if (mpsrc.GetPathSize(ipath) < 2)
			{
				return;
			}
			int start = mpsrc.GetPathStart(ipath);
			int end = mpsrc.GetPathEnd(ipath) - 1;
			com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)mpsrc.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION);
			bool bClosed = mpsrc.IsClosedPath(ipath);
			com.epl.geometry.AttributeStreamOfInt32 stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			stack.Reserve(mpsrc.GetPathSize(ipath) + 1);
			com.epl.geometry.AttributeStreamOfInt32 resultStack = new com.epl.geometry.AttributeStreamOfInt32(0);
			resultStack.Reserve(mpsrc.GetPathSize(ipath) + 1);
			stack.Add(bClosed ? start : end);
			stack.Add(start);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			while (stack.Size() > 1)
			{
				int i1 = stack.GetLast();
				stack.RemoveLast();
				int i2 = stack.GetLast();
				mpsrc.GetXY(i1, pt);
				lineHelper.SetStartXY(pt);
				mpsrc.GetXY(i2, pt);
				lineHelper.SetEndXY(pt);
				int mid = FindGreatestDistance(lineHelper, pt, xy, i1, i2, end);
				if (mid >= 0)
				{
					stack.Add(mid);
					stack.Add(i1);
				}
				else
				{
					resultStack.Add(i1);
				}
			}
			if (!bClosed)
			{
				resultStack.Add(stack.Get(0));
			}
			if (resultStack.Size() == stack.Size())
			{
				mpdst.AddPath(mpsrc, ipath, true);
			}
			else
			{
				if (resultStack.Size() >= 2)
				{
					if (m_bRemoveDegenerateParts && resultStack.Size() == 2)
					{
						if (bClosed)
						{
							return;
						}
						double d = com.epl.geometry.Point2D.Distance(mpsrc.GetXY(resultStack.Get(0)), mpsrc.GetXY(resultStack.Get(1)));
						if (d <= m_maxDeviation)
						{
							return;
						}
					}
					com.epl.geometry.Point point = new com.epl.geometry.Point();
					for (int i = 0, n = resultStack.Size(); i < n; i++)
					{
						mpsrc.GetPointByVal(resultStack.Get(i), point);
						if (i == 0)
						{
							mpdst.StartPath(point);
						}
						else
						{
							mpdst.LineTo(point);
						}
					}
					if (bClosed)
					{
						if (!m_bRemoveDegenerateParts && resultStack.Size() == 2)
						{
							mpdst.LineTo(point);
						}
						mpdst.ClosePathWithLine();
					}
				}
			}
		}

		private int FindGreatestDistance(com.epl.geometry.Line line, com.epl.geometry.Point2D ptHelper, com.epl.geometry.AttributeStreamOfDbl xy, int start, int end, int pathEnd)
		{
			int to = end - 1;
			if (end <= start)
			{
				// closed path case. end is equal to the path start.
				to = pathEnd;
			}
			int mid = -1;
			double maxd = -1.0;
			for (int i = start + 1; i <= to; i++)
			{
				xy.Read(2 * i, ptHelper);
				double x1 = ptHelper.x;
				double y1 = ptHelper.y;
				double t = line.GetClosestCoordinate(ptHelper, false);
				line.GetCoord2D(t, ptHelper);
				ptHelper.x -= x1;
				ptHelper.y -= y1;
				double dist = ptHelper.Length();
				if (dist > m_maxDeviation && dist > maxd)
				{
					mid = i;
					maxd = dist;
				}
			}
			return mid;
		}
	}
}
