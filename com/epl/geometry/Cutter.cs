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
using System.Linq;

namespace com.epl.geometry
{
	internal class Cutter
	{
		internal class CompareVertices
		{
			internal int m_orderIndex;

			internal com.epl.geometry.EditShape m_editShape;

			internal CompareVertices(int orderIndex, com.epl.geometry.EditShape editShape)
			{
				m_orderIndex = orderIndex;
				m_editShape = editShape;
			}

			internal virtual int _compareVertices(int v1, int v2)
			{
				com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
				m_editShape.GetXY(v1, pt1);
				com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
				m_editShape.GetXY(v2, pt2);
				int res = pt1.Compare(pt2);
				if (res != 0)
				{
					return res;
				}
				int z1 = m_editShape.GetUserIndex(v1, m_orderIndex);
				int z2 = m_editShape.GetUserIndex(v2, m_orderIndex);
				if (z1 < z2)
				{
					return -1;
				}
				if (z1 == z2)
				{
					return 0;
				}
				return 1;
			}
		}

		internal class CutterVertexComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal com.epl.geometry.Cutter.CompareVertices m_compareVertices;

			internal CutterVertexComparer(com.epl.geometry.Cutter.CompareVertices _compareVertices)
			{
				m_compareVertices = _compareVertices;
			}

			public override int Compare(int v1, int v2)
			{
				return m_compareVertices._compareVertices(v1, v2);
			}
		}

		internal class CutEvent
		{
			internal int m_ivertexCuttee;

			internal int m_ipartCuttee;

			internal double m_scalarCuttee0;

			internal double m_scalarCuttee1;

			internal int m_count;

			internal int m_ivertexCutter;

			internal int m_ipartCutter;

			internal double m_scalarCutter0;

			internal double m_scalarCutter1;

			internal CutEvent(int ivertexCuttee, int ipartCuttee, double scalarCuttee0, double scalarCuttee1, int count, int ivertexCutter, int ipartCutter, double scalarCutter0, double scalarCutter1)
			{
				m_ivertexCuttee = ivertexCuttee;
				m_ipartCuttee = ipartCuttee;
				m_scalarCuttee0 = scalarCuttee0;
				m_scalarCuttee1 = scalarCuttee1;
				m_count = count;
				m_ivertexCutter = ivertexCutter;
				m_ipartCutter = ipartCutter;
				m_scalarCutter0 = scalarCutter0;
				m_scalarCutter1 = scalarCutter1;
			}
		}

		internal static com.epl.geometry.EditShape CutPolyline(bool bConsiderTouch, com.epl.geometry.Polyline cuttee, com.epl.geometry.Polyline cutter, double tolerance, System.Collections.Generic.List<com.epl.geometry.OperatorCutLocal.CutPair> cutPairs, com.epl.geometry.AttributeStreamOfInt32
			 segmentCounts, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (cuttee.IsEmpty())
			{
				com.epl.geometry.OperatorCutLocal.CutPair cutPair;
				cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(cuttee, com.epl.geometry.OperatorCutLocal.Side.Uncut, -1, -1, com.epl.geometry.NumberUtils.NaN(), com.epl.geometry.OperatorCutLocal.Side.Uncut, -1, -1, com.epl.geometry.NumberUtils.NaN(), -1, -1, com.epl.geometry.NumberUtils
					.NaN(), -1, -1, com.epl.geometry.NumberUtils.NaN());
				cutPairs.Add(cutPair);
				return null;
			}
			com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
			int cutteeHandle = editShape.AddGeometry(cuttee);
			int cutterHandle = editShape.AddGeometry(cutter);
			com.epl.geometry.CrackAndCluster.Execute(editShape, tolerance, progressTracker, true);
			int order = 0;
			int orderIndex = editShape.CreateUserIndex();
			for (int igeometry = editShape.GetFirstGeometry(); igeometry != -1; igeometry = editShape.GetNextGeometry(igeometry))
			{
				for (int ipath = editShape.GetFirstPath(igeometry); ipath != -1; ipath = editShape.GetNextPath(ipath))
				{
					for (int ivertex = editShape.GetFirstVertex(ipath), i = 0, n = editShape.GetPathSize(ipath); i < n; ivertex = editShape.GetNextVertex(ivertex), i++)
					{
						editShape.SetUserIndex(ivertex, orderIndex, order++);
					}
				}
			}
			System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents = _getCutEvents(orderIndex, editShape);
			_Cut(bConsiderTouch, false, cutEvents, editShape, cutPairs, segmentCounts);
			return editShape;
		}

		private static System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> _getCutEvents(int orderIndex, com.epl.geometry.EditShape editShape)
		{
			int pointCount = editShape.GetTotalPointCount();
			// Sort vertices lexicographically
			// Firstly copy allvertices to an array.
			com.epl.geometry.AttributeStreamOfInt32 vertices = new com.epl.geometry.AttributeStreamOfInt32(0);
			for (int igeometry = editShape.GetFirstGeometry(); igeometry != -1; igeometry = editShape.GetNextGeometry(igeometry))
			{
				for (int ipath = editShape.GetFirstPath(igeometry); ipath != -1; ipath = editShape.GetNextPath(ipath))
				{
					for (int ivertex = editShape.GetFirstVertex(ipath), i = 0, n = editShape.GetPathSize(ipath); i < n; ivertex = editShape.GetNextVertex(ivertex), i++)
					{
						vertices.Add(ivertex);
					}
				}
			}
			// Sort
			com.epl.geometry.Cutter.CompareVertices compareVertices = new com.epl.geometry.Cutter.CompareVertices(orderIndex, editShape);
			vertices.Sort(0, pointCount, new com.epl.geometry.Cutter.CutterVertexComparer(compareVertices));
			// SORTDYNAMICARRAYEX(vertices, index_type, 0, pointCount,
			// CutterVertexComparer, compareVertices);
			// Find Cut Events
			System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents = new System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent>(0);
			System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEventsTemp = new System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent>(0);
			int eventIndex = editShape.CreateUserIndex();
			int eventIndexTemp = editShape.CreateUserIndex();
			int cutteeHandle = editShape.GetFirstGeometry();
			int cutterHandle = editShape.GetNextGeometry(cutteeHandle);
			com.epl.geometry.Point2D pointCuttee = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pointCutter = new com.epl.geometry.Point2D();
			int ivertexCuttee = vertices.Get(0);
			int ipartCuttee = editShape.GetPathFromVertex(ivertexCuttee);
			int igeometryCuttee = editShape.GetGeometryFromPath(ipartCuttee);
			editShape.GetXY(ivertexCuttee, pointCuttee);
			int istart = 1;
			int ivertex_1 = 0;
			while (istart < pointCount - 1)
			{
				bool bCutEvent = false;
				for (int i = istart; i < pointCount; i++)
				{
					if (i == ivertex_1)
					{
						continue;
					}
					int ivertexCutter = vertices.Get(i);
					int ipartCutter = editShape.GetPathFromVertex(ivertexCutter);
					int igeometryCutter = editShape.GetGeometryFromPath(ipartCutter);
					editShape.GetXY(ivertexCutter, pointCutter);
					if (pointCuttee.IsEqual(pointCutter))
					{
						bool bCondition = igeometryCuttee == cutteeHandle && igeometryCutter == cutterHandle;
						if (bCondition)
						{
							bCutEvent = _cutteeCutterEvents(eventIndex, eventIndexTemp, editShape, cutEvents, cutEventsTemp, ipartCuttee, ivertexCuttee, ipartCutter, ivertexCutter);
						}
					}
					else
					{
						break;
					}
				}
				if (bCutEvent || ivertex_1 == istart - 1)
				{
					if (bCutEvent && (ivertex_1 == istart - 1))
					{
						istart--;
					}
					if (++ivertex_1 == pointCount)
					{
						break;
					}
					ivertexCuttee = vertices.Get(ivertex_1);
					ipartCuttee = editShape.GetPathFromVertex(ivertexCuttee);
					igeometryCuttee = editShape.GetGeometryFromPath(ipartCuttee);
					editShape.GetXY(ivertexCuttee, pointCuttee);
				}
				if (!bCutEvent)
				{
					istart = ivertex_1 + 1;
				}
			}
			System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEventsSorted = new System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent>(0);
			// Sort CutEvents
			int icutEvent;
			int icutEventTemp;
			for (int igeometry_1 = editShape.GetFirstGeometry(); igeometry_1 != -1; igeometry_1 = editShape.GetNextGeometry(igeometry_1))
			{
				for (int ipath_1 = editShape.GetFirstPath(igeometry_1); ipath_1 != -1; ipath_1 = editShape.GetNextPath(ipath_1))
				{
					for (int iv = editShape.GetFirstVertex(ipath_1), i = 0, n = editShape.GetPathSize(ipath_1); i < n; iv = editShape.GetNextVertex(iv), i++)
					{
						icutEventTemp = editShape.GetUserIndex(iv, eventIndexTemp);
						if (icutEventTemp >= 0)
						{
							// _ASSERT(cutEventsTemp.get(icutEventTemp).m_ivertexCuttee
							// == iv);
							while (icutEventTemp < cutEventsTemp.Count && cutEventsTemp[icutEventTemp].m_ivertexCuttee == iv)
							{
								cutEventsSorted.Add(cutEventsTemp[icutEventTemp++]);
							}
						}
						icutEvent = editShape.GetUserIndex(iv, eventIndex);
						if (icutEvent >= 0)
						{
							// _ASSERT(cutEvents->Get(icutEvent)->m_ivertexCuttee ==
							// iv);
							while (icutEvent < cutEvents.Count && cutEvents[icutEvent].m_ivertexCuttee == iv)
							{
								cutEventsSorted.Add(cutEvents[icutEvent++]);
							}
						}
					}
				}
			}
			// _ASSERT(cutEvents->Size() + cutEventsTemp->Size() ==
			// cutEventsSorted->Size());
			editShape.RemoveUserIndex(eventIndex);
			editShape.RemoveUserIndex(eventIndexTemp);
			return cutEventsSorted;
		}

		internal static bool _cutteeCutterEvents(int eventIndex, int eventIndexTemp, com.epl.geometry.EditShape editShape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEventsTemp, int ipartCuttee
			, int ivertexCuttee, int ipartCutter, int ivertexCutter)
		{
			int ilastVertexCuttee = editShape.GetLastVertex(ipartCuttee);
			int ilastVertexCutter = editShape.GetLastVertex(ipartCutter);
			int ifirstVertexCuttee = editShape.GetFirstVertex(ipartCuttee);
			int ifirstVertexCutter = editShape.GetFirstVertex(ipartCutter);
			int ivertexCutteePrev = editShape.GetPrevVertex(ivertexCuttee);
			int ivertexCutterPrev = editShape.GetPrevVertex(ivertexCutter);
			bool bEndEnd = false;
			bool bEndStart = false;
			bool bStartEnd = false;
			bool bStartStart = false;
			if (ivertexCuttee != ifirstVertexCuttee)
			{
				if (ivertexCutter != ifirstVertexCutter)
				{
					bEndEnd = _cutteeEndCutterEndEvent(eventIndex, editShape, cutEvents, ipartCuttee, ivertexCutteePrev, ipartCutter, ivertexCutterPrev);
				}
				if (ivertexCutter != ilastVertexCutter)
				{
					bEndStart = _cutteeEndCutterStartEvent(eventIndex, editShape, cutEvents, ipartCuttee, ivertexCutteePrev, ipartCutter, ivertexCutter);
				}
			}
			if (ivertexCuttee != ilastVertexCuttee)
			{
				if (ivertexCutter != ifirstVertexCutter)
				{
					bStartEnd = _cutteeStartCutterEndEvent(eventIndexTemp, editShape, cutEventsTemp, ipartCuttee, ivertexCuttee, ipartCutter, ivertexCutterPrev, ifirstVertexCuttee);
				}
				if (ivertexCutter != ilastVertexCutter)
				{
					bStartStart = _cutteeStartCutterStartEvent(eventIndexTemp, editShape, cutEventsTemp, ipartCuttee, ivertexCuttee, ipartCutter, ivertexCutter, ifirstVertexCuttee);
				}
			}
			if (bEndEnd && bEndStart && bStartEnd)
			{
				int iendstart = cutEvents.Count - 1;
				int istartend = (bStartStart ? cutEventsTemp.Count - 2 : cutEventsTemp.Count - 1);
				if (cutEventsTemp[istartend].m_count == 2)
				{
					// Replace bEndEnd with bEndStart, and remove duplicate
					// bEndStart (get rid of bEndEnd)
					cutEvents[iendstart - 1] = cutEvents[iendstart];
					cutEvents.RemoveAt(cutEvents.Count - 1);
				}
			}
			else
			{
				if (bEndEnd && bEndStart && bStartStart)
				{
					int istartstart = cutEventsTemp.Count - 1;
					if (cutEventsTemp[istartstart].m_count == 2)
					{
						// Remove bEndStart
						com.epl.geometry.Cutter.CutEvent lastEvent = cutEvents[cutEvents.Count - 1];
						cutEvents.Remove(cutEvents[cutEvents.Count - 1]);
						int icutEvent = editShape.GetUserIndex(lastEvent.m_ivertexCuttee, eventIndex);
						if (icutEvent == cutEvents.Count)
						{
							editShape.SetUserIndex(lastEvent.m_ivertexCuttee, eventIndex, -1);
						}
					}
				}
			}
			return bEndEnd || bEndStart || bStartEnd || bStartStart;
		}

		private static bool _cutteeEndCutterEndEvent(int eventIndex, com.epl.geometry.EditShape editShape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int ipartCuttee, int ivertexCuttee, int ipartCutter, int ivertexCutter)
		{
			com.epl.geometry.Segment segmentCuttee;
			com.epl.geometry.Segment segmentCutter;
			com.epl.geometry.Line lineCuttee = new com.epl.geometry.Line();
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			double[] scalarsCuttee = new double[2];
			double[] scalarsCutter = new double[2];
			com.epl.geometry.Cutter.CutEvent cutEvent;
			segmentCuttee = editShape.GetSegment(ivertexCuttee);
			if (segmentCuttee == null)
			{
				editShape.QueryLineConnector(ivertexCuttee, lineCuttee);
				segmentCuttee = lineCuttee;
			}
			segmentCutter = editShape.GetSegment(ivertexCutter);
			if (segmentCutter == null)
			{
				editShape.QueryLineConnector(ivertexCutter, lineCutter);
				segmentCutter = lineCutter;
			}
			int count = segmentCuttee.Intersect(segmentCutter, null, scalarsCuttee, scalarsCutter, 0.0);
			// _ASSERT(count > 0);
			int icutEvent;
			// If count == 2 (i.e. when they overlap), this this event would have
			// been discovered by _CutteeStartCutterStartEvent at the previous index
			if (count < 2)
			{
				cutEvent = new com.epl.geometry.Cutter.CutEvent(ivertexCuttee, ipartCuttee, scalarsCuttee[0], com.epl.geometry.NumberUtils.NaN(), count, ivertexCutter, ipartCutter, scalarsCutter[0], com.epl.geometry.NumberUtils.NaN());
				cutEvents.Add(cutEvent);
				icutEvent = editShape.GetUserIndex(ivertexCuttee, eventIndex);
				if (icutEvent < 0)
				{
					editShape.SetUserIndex(ivertexCuttee, eventIndex, cutEvents.Count - 1);
				}
			}
			return true;
		}

		private static bool _cutteeEndCutterStartEvent(int eventIndex, com.epl.geometry.EditShape editShape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int ipartCuttee, int ivertexCuttee, int ipartCutter, int ivertexCutter)
		{
			com.epl.geometry.Segment segmentCuttee;
			com.epl.geometry.Segment segmentCutter;
			com.epl.geometry.Line lineCuttee = new com.epl.geometry.Line();
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			double[] scalarsCuttee = new double[2];
			double[] scalarsCutter = new double[2];
			com.epl.geometry.Cutter.CutEvent cutEvent;
			segmentCuttee = editShape.GetSegment(ivertexCuttee);
			if (segmentCuttee == null)
			{
				editShape.QueryLineConnector(ivertexCuttee, lineCuttee);
				segmentCuttee = lineCuttee;
			}
			segmentCutter = editShape.GetSegment(ivertexCutter);
			if (segmentCutter == null)
			{
				editShape.QueryLineConnector(ivertexCutter, lineCutter);
				segmentCutter = lineCutter;
			}
			int count = segmentCuttee.Intersect(segmentCutter, null, scalarsCuttee, scalarsCutter, 0.0);
			// _ASSERT(count > 0);
			int icutEvent;
			// If count == 2 (i.e. when they overlap), this this event would have
			// been discovered by _CutteeStartCutterEndEvent at the previous index
			if (count < 2)
			{
				cutEvent = new com.epl.geometry.Cutter.CutEvent(ivertexCuttee, ipartCuttee, scalarsCuttee[0], com.epl.geometry.NumberUtils.NaN(), count, ivertexCutter, ipartCutter, scalarsCutter[0], com.epl.geometry.NumberUtils.NaN());
				cutEvents.Add(cutEvent);
				icutEvent = editShape.GetUserIndex(ivertexCuttee, eventIndex);
				if (icutEvent < 0)
				{
					editShape.SetUserIndex(ivertexCuttee, eventIndex, cutEvents.Count - 1);
				}
				return true;
			}
			return false;
		}

		private static bool _cutteeStartCutterEndEvent(int eventIndex, com.epl.geometry.EditShape editShape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int ipartCuttee, int ivertexCuttee, int ipartCutter, int ivertexCutter, int ifirstVertexCuttee)
		{
			com.epl.geometry.Segment segmentCuttee;
			com.epl.geometry.Segment segmentCutter;
			com.epl.geometry.Line lineCuttee = new com.epl.geometry.Line();
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			double[] scalarsCuttee = new double[2];
			double[] scalarsCutter = new double[2];
			com.epl.geometry.Cutter.CutEvent cutEvent;
			segmentCuttee = editShape.GetSegment(ivertexCuttee);
			if (segmentCuttee == null)
			{
				editShape.QueryLineConnector(ivertexCuttee, lineCuttee);
				segmentCuttee = lineCuttee;
			}
			segmentCutter = editShape.GetSegment(ivertexCutter);
			if (segmentCutter == null)
			{
				editShape.QueryLineConnector(ivertexCutter, lineCutter);
				segmentCutter = lineCutter;
			}
			int count = segmentCuttee.Intersect(segmentCutter, null, scalarsCuttee, scalarsCutter, 0.0);
			// _ASSERT(count > 0);
			int icutEvent;
			if (count == 2)
			{
				cutEvent = new com.epl.geometry.Cutter.CutEvent(ivertexCuttee, ipartCuttee, scalarsCuttee[0], scalarsCuttee[1], count, ivertexCutter, ipartCutter, scalarsCutter[0], scalarsCutter[1]);
				cutEvents.Add(cutEvent);
				icutEvent = editShape.GetUserIndex(ivertexCuttee, eventIndex);
				if (icutEvent < 0)
				{
					editShape.SetUserIndex(ivertexCuttee, eventIndex, cutEvents.Count - 1);
				}
				return true;
			}
			else
			{
				bool bCutEvent = false;
				if (ivertexCuttee == ifirstVertexCuttee)
				{
					cutEvent = new com.epl.geometry.Cutter.CutEvent(ivertexCuttee, ipartCuttee, scalarsCuttee[0], com.epl.geometry.NumberUtils.NaN(), count, ivertexCutter, ipartCutter, scalarsCutter[0], com.epl.geometry.NumberUtils.NaN());
					cutEvents.Add(cutEvent);
					icutEvent = editShape.GetUserIndex(ivertexCuttee, eventIndex);
					if (icutEvent < 0)
					{
						editShape.SetUserIndex(ivertexCuttee, eventIndex, cutEvents.Count - 1);
					}
					bCutEvent = true;
				}
				return bCutEvent;
			}
		}

		private static bool _cutteeStartCutterStartEvent(int eventIndex, com.epl.geometry.EditShape editShape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int ipartCuttee, int ivertexCuttee, int ipartCutter, int ivertexCutter, int ifirstVertexCuttee)
		{
			com.epl.geometry.Segment segmentCuttee;
			com.epl.geometry.Segment segmentCutter;
			com.epl.geometry.Line lineCuttee = new com.epl.geometry.Line();
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			double[] scalarsCuttee = new double[2];
			double[] scalarsCutter = new double[2];
			com.epl.geometry.Cutter.CutEvent cutEvent;
			segmentCuttee = editShape.GetSegment(ivertexCuttee);
			if (segmentCuttee == null)
			{
				editShape.QueryLineConnector(ivertexCuttee, lineCuttee);
				segmentCuttee = lineCuttee;
			}
			segmentCutter = editShape.GetSegment(ivertexCutter);
			if (segmentCutter == null)
			{
				editShape.QueryLineConnector(ivertexCutter, lineCutter);
				segmentCutter = lineCutter;
			}
			int count = segmentCuttee.Intersect(segmentCutter, null, scalarsCuttee, scalarsCutter, 0.0);
			// _ASSERT(count > 0);
			int icutEvent;
			if (count == 2)
			{
				cutEvent = new com.epl.geometry.Cutter.CutEvent(ivertexCuttee, ipartCuttee, scalarsCuttee[0], scalarsCuttee[1], count, ivertexCutter, ipartCutter, scalarsCutter[0], scalarsCutter[1]);
				cutEvents.Add(cutEvent);
				icutEvent = editShape.GetUserIndex(ivertexCuttee, eventIndex);
				if (icutEvent < 0)
				{
					editShape.SetUserIndex(ivertexCuttee, eventIndex, cutEvents.Count - 1);
				}
				return true;
			}
			else
			{
				bool bCutEvent = false;
				if (ivertexCuttee == ifirstVertexCuttee)
				{
					cutEvent = new com.epl.geometry.Cutter.CutEvent(ivertexCuttee, ipartCuttee, scalarsCuttee[0], com.epl.geometry.NumberUtils.NaN(), count, ivertexCutter, ipartCutter, scalarsCutter[0], com.epl.geometry.NumberUtils.NaN());
					cutEvents.Add(cutEvent);
					icutEvent = editShape.GetUserIndex(ivertexCuttee, eventIndex);
					if (icutEvent < 0)
					{
						editShape.SetUserIndex(ivertexCuttee, eventIndex, cutEvents.Count - 1);
					}
					bCutEvent = true;
				}
				return bCutEvent;
			}
		}

		internal static void _Cut(bool bConsiderTouch, bool bLocalCutsOnly, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, com.epl.geometry.EditShape shape, System.Collections.Generic.List<com.epl.geometry.OperatorCutLocal.CutPair> cutPairs, com.epl.geometry.AttributeStreamOfInt32
			 segmentCounts)
		{
			com.epl.geometry.OperatorCutLocal.CutPair cutPair;
			com.epl.geometry.Point2D[] tangents = new com.epl.geometry.Point2D[4];
			tangents[0] = new com.epl.geometry.Point2D();
			tangents[1] = new com.epl.geometry.Point2D();
			tangents[2] = new com.epl.geometry.Point2D();
			tangents[3] = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D tangent0 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D tangent1 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D tangent2 = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D tangent3 = new com.epl.geometry.Point2D();
			com.epl.geometry.SegmentBuffer segmentBufferCuttee = null;
			if (cutPairs != null)
			{
				segmentBufferCuttee = new com.epl.geometry.SegmentBuffer();
				segmentBufferCuttee.CreateLine();
			}
			com.epl.geometry.Segment segmentCuttee = null;
			int icutEvent = 0;
			com.epl.geometry.MultiPath multipath = null;
			com.epl.geometry.Line lineCuttee = new com.epl.geometry.Line();
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			int polyline = shape.GetFirstGeometry();
			for (int ipath = shape.GetFirstPath(polyline); ipath != -1; ipath = shape.GetNextPath(ipath))
			{
				int cut;
				int cutPrev = com.epl.geometry.OperatorCutLocal.Side.Uncut;
				int ipartCuttee = -1;
				int ivertexCuttee = -1;
				double scalarCuttee = com.epl.geometry.NumberUtils.NaN();
				int ipartCutteePrev = -1;
				int ivertexCutteePrev = -1;
				double scalarCutteePrev = com.epl.geometry.NumberUtils.NaN();
				int ipartCutter = -1;
				int ivertexCutter = -1;
				double scalarCutter = com.epl.geometry.NumberUtils.NaN();
				int ipartCutterPrev = -1;
				int ivertexCutterPrev = -1;
				double scalarCutterPrev = com.epl.geometry.NumberUtils.NaN();
				bool bNoCutYet = true;
				// Indicates whether a cut as occured for
				// the current part
				bool bCoincidentNotAdded = false;
				// Indicates whether the
				// current coincident
				// multipath has been added
				// to cutPairs
				bool bCurrentMultiPathNotAdded = true;
				// Indicates whether there
				// is a multipath not
				// yet added to cutPairs
				// (left, right, or
				// undefined)
				bool bStartNewPath = true;
				bool bCreateNewMultiPath = true;
				int segmentCount = 0;
				ipartCutteePrev = ipath;
				scalarCutteePrev = 0.0;
				for (int ivertex = shape.GetFirstVertex(ipath), n = shape.GetPathSize(ipath), i = 0; i < n; ivertex = shape.GetNextVertex(ivertex), i++)
				{
					segmentCuttee = shape.GetSegment(ivertex);
					if (segmentCuttee == null)
					{
						if (!shape.QueryLineConnector(ivertex, lineCuttee))
						{
							continue;
						}
						segmentCuttee = lineCuttee;
					}
					if (ivertexCutteePrev == -1)
					{
						ivertexCutteePrev = ivertex;
					}
					double lastScalarCuttee = 0.0;
					// last scalar along the current
					// segment
					while (icutEvent < cutEvents.Count && ivertex == cutEvents[icutEvent].m_ivertexCuttee)
					{
						ipartCuttee = cutEvents[icutEvent].m_ipartCuttee;
						ivertexCuttee = cutEvents[icutEvent].m_ivertexCuttee;
						scalarCuttee = cutEvents[icutEvent].m_scalarCuttee0;
						ipartCutter = cutEvents[icutEvent].m_ipartCutter;
						ivertexCutter = cutEvents[icutEvent].m_ivertexCutter;
						scalarCutter = cutEvents[icutEvent].m_scalarCutter0;
						if (cutEvents[icutEvent].m_count == 2)
						{
							// We have an overlap
							if (!bCoincidentNotAdded)
							{
								ipartCutteePrev = ipartCuttee;
								ivertexCutteePrev = ivertexCuttee;
								scalarCutteePrev = scalarCuttee;
								ipartCutterPrev = ipartCutter;
								ivertexCutterPrev = ivertexCutter;
								scalarCutterPrev = scalarCutter;
								cutPrev = com.epl.geometry.OperatorCutLocal.Side.Coincident;
								// Create new multipath
								if (cutPairs != null)
								{
									multipath = new com.epl.geometry.Polyline();
								}
								else
								{
									segmentCount = 0;
								}
								bCreateNewMultiPath = false;
								bStartNewPath = true;
							}
							scalarCuttee = cutEvents[icutEvent].m_scalarCuttee1;
							scalarCutter = cutEvents[icutEvent].m_scalarCutter1;
							if (cutPairs != null)
							{
								segmentCuttee.Cut(lastScalarCuttee, cutEvents[icutEvent].m_scalarCuttee1, segmentBufferCuttee);
								multipath.AddSegment(segmentBufferCuttee.Get(), bStartNewPath);
							}
							else
							{
								segmentCount++;
							}
							lastScalarCuttee = scalarCuttee;
							bCoincidentNotAdded = true;
							bNoCutYet = false;
							bStartNewPath = false;
							if (icutEvent + 1 == cutEvents.Count || cutEvents[icutEvent + 1].m_count != 2 || cutEvents[icutEvent + 1].m_ivertexCuttee == ivertexCuttee && cutEvents[icutEvent + 1].m_scalarCuttee0 != lastScalarCuttee)
							{
								if (cutPairs != null)
								{
									cutPair = new com.epl.geometry.OperatorCutLocal.CutPair((com.epl.geometry.Geometry)multipath, com.epl.geometry.OperatorCutLocal.Side.Coincident, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter
										, ipartCutterPrev, ivertexCutterPrev, scalarCutterPrev);
									cutPairs.Add(cutPair);
								}
								else
								{
									segmentCounts.Add(segmentCount);
								}
								ipartCutteePrev = ipartCuttee;
								ivertexCutteePrev = ivertexCuttee;
								scalarCutteePrev = scalarCuttee;
								ipartCutterPrev = ipartCutter;
								ivertexCutterPrev = ivertexCutter;
								scalarCutterPrev = scalarCutter;
								cutPrev = com.epl.geometry.OperatorCutLocal.Side.Coincident;
								bNoCutYet = false;
								bCoincidentNotAdded = false;
								bCreateNewMultiPath = true;
								bStartNewPath = true;
							}
							icutEvent++;
							continue;
						}
						int ivertexCutteePlus = shape.GetNextVertex(ivertexCuttee);
						int ivertexCutterPlus = shape.GetNextVertex(ivertexCutter);
						int ivertexCutterMinus = shape.GetPrevVertex(ivertexCutter);
						if (icutEvent < cutEvents.Count - 1 && cutEvents[icutEvent + 1].m_ivertexCuttee == ivertexCutteePlus && cutEvents[icutEvent + 1].m_ivertexCutter == ivertexCutter && cutEvents[icutEvent + 1].m_count == 2)
						{
							if (scalarCuttee != lastScalarCuttee)
							{
								if (bCreateNewMultiPath)
								{
									if (cutPairs != null)
									{
										multipath = new com.epl.geometry.Polyline();
									}
									else
									{
										segmentCount = 0;
									}
								}
								if (icutEvent > 0 && cutEvents[icutEvent - 1].m_ipartCuttee == ipartCuttee)
								{
									if (cutPrev == com.epl.geometry.OperatorCutLocal.Side.Right)
									{
										cut = com.epl.geometry.OperatorCutLocal.Side.Left;
									}
									else
									{
										if (cutPrev == com.epl.geometry.OperatorCutLocal.Side.Left)
										{
											cut = com.epl.geometry.OperatorCutLocal.Side.Right;
										}
										else
										{
											cut = com.epl.geometry.OperatorCutLocal.Side.Undefined;
										}
									}
								}
								else
								{
									cut = com.epl.geometry.OperatorCutLocal.Side.Undefined;
								}
								if (cutPairs != null)
								{
									segmentCuttee.Cut(lastScalarCuttee, scalarCuttee, segmentBufferCuttee);
									multipath.AddSegment(segmentBufferCuttee.Get(), bStartNewPath);
									cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, cut, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev, scalarCutterPrev);
									cutPairs.Add(cutPair);
								}
								else
								{
									segmentCount++;
									segmentCounts.Add(segmentCount);
								}
								lastScalarCuttee = scalarCuttee;
								ipartCutteePrev = ipartCuttee;
								ivertexCutteePrev = ivertexCuttee;
								scalarCutteePrev = scalarCuttee;
								ipartCutterPrev = ipartCutter;
								ivertexCutterPrev = ivertexCutter;
								scalarCutterPrev = scalarCutter;
								cutPrev = cut;
								bCurrentMultiPathNotAdded = false;
								bNoCutYet = false;
								bCreateNewMultiPath = true;
								bStartNewPath = true;
							}
							icutEvent++;
							continue;
						}
						bool bContinue = _cutterTangents(bConsiderTouch, shape, cutEvents, icutEvent, tangent0, tangent1);
						if (bContinue)
						{
							icutEvent++;
							continue;
						}
						_cutteeTangents(shape, cutEvents, icutEvent, ipath, ivertex, tangent2, tangent3);
						bool bCut = false;
						bool bTouch = false;
						bool bCutRight = true;
						if (!tangent0.IsEqual(tangent2) && !tangent1.IsEqual(tangent2) && !tangent0.IsEqual(tangent3) && !tangent1.IsEqual(tangent3))
						{
							tangents[0].SetCoords(tangent0);
							tangents[1].SetCoords(tangent1);
							tangents[2].SetCoords(tangent2);
							tangents[3].SetCoords(tangent3);
							System.Array.Sort(tangents, new com.epl.geometry.Point2D.VectorComparator());
							// SORTARRAY(tangents, Point2D,
							// Point2D::_VectorComparator);
							com.epl.geometry.Point2D value0 = (com.epl.geometry.Point2D)tangents[0];
							com.epl.geometry.Point2D value1 = (com.epl.geometry.Point2D)tangents[1];
							com.epl.geometry.Point2D value2 = (com.epl.geometry.Point2D)tangents[2];
							com.epl.geometry.Point2D value3 = (com.epl.geometry.Point2D)tangents[3];
							if (value0.IsEqual(tangent0))
							{
								if (value1.IsEqual(tangent1))
								{
									if (!bConsiderTouch)
									{
										bCut = false;
									}
									else
									{
										bCut = true;
										bTouch = true;
										bCutRight = false;
									}
								}
								else
								{
									if (value3.IsEqual(tangent1))
									{
										if (!bConsiderTouch)
										{
											bCut = false;
										}
										else
										{
											bCut = true;
											bTouch = true;
											bCutRight = true;
										}
									}
									else
									{
										bCut = true;
										bCutRight = value1.IsEqual(tangent2);
									}
								}
							}
							else
							{
								if (value1.IsEqual(tangent0))
								{
									if (value2.IsEqual(tangent1))
									{
										if (!bConsiderTouch)
										{
											bCut = false;
										}
										else
										{
											bCut = true;
											bTouch = true;
											bCutRight = false;
										}
									}
									else
									{
										if (value0.IsEqual(tangent1))
										{
											if (!bConsiderTouch)
											{
												bCut = false;
											}
											else
											{
												bCut = true;
												bTouch = true;
												bCutRight = true;
											}
										}
										else
										{
											bCut = true;
											bCutRight = value2.IsEqual(tangent2);
										}
									}
								}
								else
								{
									if (value2.IsEqual(tangent0))
									{
										if (value3.IsEqual(tangent1))
										{
											if (!bConsiderTouch)
											{
												bCut = false;
											}
											else
											{
												bCut = true;
												bTouch = true;
												bCutRight = false;
											}
										}
										else
										{
											if (value1.IsEqual(tangent1))
											{
												if (!bConsiderTouch)
												{
													bCut = false;
												}
												else
												{
													bCut = true;
													bTouch = true;
													bCutRight = true;
												}
											}
											else
											{
												bCut = true;
												bCutRight = value3.IsEqual(tangent2);
											}
										}
									}
									else
									{
										if (value0.IsEqual(tangent1))
										{
											if (!bConsiderTouch)
											{
												bCut = false;
											}
											else
											{
												bCut = true;
												bTouch = true;
												bCutRight = false;
											}
										}
										else
										{
											if (value2.IsEqual(tangent1))
											{
												if (!bConsiderTouch)
												{
													bCut = false;
												}
												else
												{
													bCut = true;
													bTouch = true;
													bCutRight = true;
												}
											}
											else
											{
												bCut = true;
												bCutRight = value0.IsEqual(tangent2);
											}
										}
									}
								}
							}
						}
						if (bCut)
						{
							bool bIsFirstSegmentInPath = (ivertex == ivertexCuttee);
							if (scalarCuttee != lastScalarCuttee || bIsFirstSegmentInPath && lastScalarCuttee == 0.0)
							{
								if (bCreateNewMultiPath)
								{
									if (cutPairs != null)
									{
										multipath = new com.epl.geometry.Polyline();
									}
									else
									{
										segmentCount = 0;
									}
								}
								if (cutPairs != null)
								{
									segmentCuttee.Cut(lastScalarCuttee, scalarCuttee, segmentBufferCuttee);
									multipath.AddSegment(segmentBufferCuttee.Get(), bStartNewPath);
								}
								else
								{
									segmentCount++;
								}
							}
							if (bCutRight)
							{
								if (cutPrev != com.epl.geometry.OperatorCutLocal.Side.Right || bLocalCutsOnly)
								{
									if (scalarCuttee != lastScalarCuttee || bIsFirstSegmentInPath && lastScalarCuttee == 0.0 || bLocalCutsOnly)
									{
										if (cutPairs != null)
										{
											cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, com.epl.geometry.OperatorCutLocal.Side.Right, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev
												, scalarCutterPrev);
											cutPairs.Add(cutPair);
										}
										else
										{
											segmentCounts.Add(segmentCount);
										}
									}
									if (!bTouch)
									{
										cutPrev = com.epl.geometry.OperatorCutLocal.Side.Right;
									}
									else
									{
										if (icutEvent == cutEvents.Count - 2 || cutEvents[icutEvent + 2].m_ipartCuttee != ipartCuttee)
										{
											cutPrev = com.epl.geometry.OperatorCutLocal.Side.Left;
										}
									}
								}
								else
								{
									if (scalarCuttee != lastScalarCuttee || bIsFirstSegmentInPath && lastScalarCuttee == 0.0 || bLocalCutsOnly)
									{
										if (cutPairs != null)
										{
											cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, com.epl.geometry.OperatorCutLocal.Side.Undefined, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev
												, scalarCutterPrev);
											cutPairs.Add(cutPair);
										}
										else
										{
											segmentCounts.Add(segmentCount);
										}
									}
									cutPrev = com.epl.geometry.OperatorCutLocal.Side.Right;
								}
							}
							else
							{
								if (cutPrev != com.epl.geometry.OperatorCutLocal.Side.Left || bLocalCutsOnly)
								{
									if (scalarCuttee != lastScalarCuttee || bIsFirstSegmentInPath && lastScalarCuttee == 0.0 || bLocalCutsOnly)
									{
										if (cutPairs != null)
										{
											cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, com.epl.geometry.OperatorCutLocal.Side.Left, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev
												, scalarCutterPrev);
											cutPairs.Add(cutPair);
										}
										else
										{
											segmentCounts.Add(segmentCount);
										}
									}
									if (!bTouch)
									{
										cutPrev = com.epl.geometry.OperatorCutLocal.Side.Left;
									}
									else
									{
										if (icutEvent == cutEvents.Count - 2 || cutEvents[icutEvent + 2].m_ipartCuttee != ipartCuttee)
										{
											cutPrev = com.epl.geometry.OperatorCutLocal.Side.Right;
										}
									}
								}
								else
								{
									if (scalarCuttee != lastScalarCuttee || bIsFirstSegmentInPath && lastScalarCuttee == 0.0 || bLocalCutsOnly)
									{
										if (cutPairs != null)
										{
											cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, com.epl.geometry.OperatorCutLocal.Side.Undefined, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev
												, scalarCutterPrev);
											cutPairs.Add(cutPair);
										}
										else
										{
											segmentCounts.Add(segmentCount);
										}
									}
									cutPrev = com.epl.geometry.OperatorCutLocal.Side.Left;
								}
							}
							if (scalarCuttee != lastScalarCuttee || bIsFirstSegmentInPath && lastScalarCuttee == 0.0 || bLocalCutsOnly)
							{
								lastScalarCuttee = scalarCuttee;
								ipartCutteePrev = ipartCuttee;
								ivertexCutteePrev = ivertexCuttee;
								scalarCutteePrev = scalarCuttee;
								ipartCutterPrev = ipartCutter;
								ivertexCutterPrev = ivertexCutter;
								scalarCutterPrev = scalarCutter;
								bCurrentMultiPathNotAdded = false;
								bNoCutYet = false;
								bCreateNewMultiPath = true;
								bStartNewPath = true;
							}
						}
						icutEvent++;
					}
					if (lastScalarCuttee != 1.0)
					{
						if (bCreateNewMultiPath)
						{
							if (cutPairs != null)
							{
								multipath = new com.epl.geometry.Polyline();
							}
							else
							{
								segmentCount = 0;
							}
						}
						if (cutPairs != null)
						{
							segmentCuttee.Cut(lastScalarCuttee, 1.0, segmentBufferCuttee);
							multipath.AddSegment(segmentBufferCuttee.Get(), bStartNewPath);
						}
						else
						{
							segmentCount++;
						}
						bCreateNewMultiPath = false;
						bStartNewPath = false;
						bCurrentMultiPathNotAdded = true;
					}
				}
				if (bCurrentMultiPathNotAdded)
				{
					scalarCuttee = 1.0;
					ivertexCuttee = shape.GetLastVertex(ipath);
					ivertexCuttee = shape.GetPrevVertex(ivertexCuttee);
					ipartCutter = -1;
					ivertexCutter = -1;
					scalarCutter = com.epl.geometry.NumberUtils.NaN();
					if (bNoCutYet)
					{
						if (cutPairs != null)
						{
							cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, com.epl.geometry.OperatorCutLocal.Side.Uncut, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev
								, scalarCutterPrev);
							cutPairs.Add(cutPair);
						}
						else
						{
							segmentCounts.Add(segmentCount);
						}
					}
					else
					{
						if (cutPrev == com.epl.geometry.OperatorCutLocal.Side.Right)
						{
							cut = com.epl.geometry.OperatorCutLocal.Side.Left;
						}
						else
						{
							if (cutPrev == com.epl.geometry.OperatorCutLocal.Side.Left)
							{
								cut = com.epl.geometry.OperatorCutLocal.Side.Right;
							}
							else
							{
								cut = com.epl.geometry.OperatorCutLocal.Side.Undefined;
							}
						}
						if (cutPairs != null)
						{
							cutPair = new com.epl.geometry.OperatorCutLocal.CutPair(multipath, cut, ipartCuttee, ivertexCuttee, scalarCuttee, cutPrev, ipartCutteePrev, ivertexCutteePrev, scalarCutteePrev, ipartCutter, ivertexCutter, scalarCutter, ipartCutterPrev, ivertexCutterPrev, scalarCutterPrev);
							cutPairs.Add(cutPair);
						}
						else
						{
							segmentCounts.Add(segmentCount);
						}
					}
				}
			}
		}

		internal static bool _cutterTangents(bool bConsiderTouch, com.epl.geometry.EditShape shape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int icutEvent, com.epl.geometry.Point2D tangent0, com.epl.geometry.Point2D tangent1)
		{
			double scalarCutter = cutEvents[icutEvent].m_scalarCutter0;
			if (scalarCutter == 1.0)
			{
				return _cutterEndTangents(bConsiderTouch, shape, cutEvents, icutEvent, tangent0, tangent1);
			}
			if (scalarCutter == 0.0)
			{
				return _cutterStartTangents(bConsiderTouch, shape, cutEvents, icutEvent, tangent0, tangent1);
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		internal static bool _cutterEndTangents(bool bConsiderTouch, com.epl.geometry.EditShape shape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int icutEvent, com.epl.geometry.Point2D tangent0, com.epl.geometry.Point2D tangent1)
		{
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			com.epl.geometry.Segment segmentCutter;
			int ivertexCuttee = cutEvents[icutEvent].m_ivertexCuttee;
			int ipartCutter = cutEvents[icutEvent].m_ipartCutter;
			int ivertexCutter = cutEvents[icutEvent].m_ivertexCutter;
			int ivertexCutteePrev = -1;
			int ipartCutterPrev = -1;
			int ivertexCutterPrev = -1;
			int countPrev = -1;
			if (!bConsiderTouch && icutEvent > 0)
			{
				com.epl.geometry.Cutter.CutEvent cutEvent = cutEvents[icutEvent - 1];
				ivertexCutteePrev = cutEvent.m_ivertexCuttee;
				ipartCutterPrev = cutEvent.m_ipartCutter;
				ivertexCutterPrev = cutEvent.m_ivertexCutter;
				countPrev = cutEvent.m_count;
			}
			int ivertexCutteeNext = -1;
			int ipartCutterNext = -1;
			int ivertexCutterNext = -1;
			int countNext = -1;
			if (icutEvent < cutEvents.Count - 1)
			{
				com.epl.geometry.Cutter.CutEvent cutEvent = cutEvents[icutEvent + 1];
				ivertexCutteeNext = cutEvent.m_ivertexCuttee;
				ipartCutterNext = cutEvent.m_ipartCutter;
				ivertexCutterNext = cutEvent.m_ivertexCutter;
				countNext = cutEvent.m_count;
			}
			int ivertexCutteePlus = shape.GetNextVertex(ivertexCuttee);
			int ivertexCutterPlus = shape.GetNextVertex(ivertexCutter);
			if (!bConsiderTouch)
			{
				if ((icutEvent > 0 && ivertexCutteePrev == ivertexCuttee && ipartCutterPrev == ipartCutter && ivertexCutterPrev == ivertexCutterPlus && countPrev == 2) || (icutEvent < cutEvents.Count - 1 && ivertexCutteeNext == ivertexCutteePlus && ipartCutterNext == ipartCutter && ivertexCutterNext == ivertexCutterPlus
					 && countNext == 2))
				{
					segmentCutter = shape.GetSegment(ivertexCutter);
					if (segmentCutter == null)
					{
						shape.QueryLineConnector(ivertexCutter, lineCutter);
						segmentCutter = lineCutter;
					}
					tangent1.SetCoords(segmentCutter._getTangent(1.0));
					tangent0.Negate(tangent1);
					tangent1.Normalize();
					tangent0.Normalize();
					return false;
				}
				if (icutEvent < cutEvents.Count - 1 && ivertexCutteeNext == ivertexCuttee && ipartCutterNext == ipartCutter && ivertexCutterNext == ivertexCutterPlus)
				{
					segmentCutter = shape.GetSegment(ivertexCutter);
					if (segmentCutter == null)
					{
						shape.QueryLineConnector(ivertexCutter, lineCutter);
						segmentCutter = lineCutter;
					}
					tangent0.SetCoords(segmentCutter._getTangent(1.0));
					segmentCutter = shape.GetSegment(ivertexCutterPlus);
					if (segmentCutter == null)
					{
						shape.QueryLineConnector(ivertexCutterPlus, lineCutter);
						segmentCutter = lineCutter;
					}
					tangent1.SetCoords(segmentCutter._getTangent(0.0));
					tangent0.Negate();
					tangent1.Normalize();
					tangent0.Normalize();
					return false;
				}
				return true;
			}
			if (icutEvent == cutEvents.Count - 1 || ivertexCutteeNext != ivertexCuttee || ipartCutterNext != ipartCutter || ivertexCutterNext != ivertexCutterPlus || countNext == 2)
			{
				segmentCutter = shape.GetSegment(ivertexCutter);
				if (segmentCutter == null)
				{
					shape.QueryLineConnector(ivertexCutter, lineCutter);
					segmentCutter = lineCutter;
				}
				tangent1.SetCoords(segmentCutter._getTangent(1.0));
				tangent0.Negate(tangent1);
				tangent1.Normalize();
				tangent0.Normalize();
				return false;
			}
			segmentCutter = shape.GetSegment(ivertexCutter);
			if (segmentCutter == null)
			{
				shape.QueryLineConnector(ivertexCutter, lineCutter);
				segmentCutter = lineCutter;
			}
			tangent0.SetCoords(segmentCutter._getTangent(1.0));
			segmentCutter = shape.GetSegment(ivertexCutterPlus);
			if (segmentCutter == null)
			{
				shape.QueryLineConnector(ivertexCutterPlus, lineCutter);
				segmentCutter = lineCutter;
			}
			tangent1.SetCoords(segmentCutter._getTangent(0.0));
			tangent0.Negate();
			tangent1.Normalize();
			tangent0.Normalize();
			return false;
		}

		internal static bool _cutterStartTangents(bool bConsiderTouch, com.epl.geometry.EditShape shape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int icutEvent, com.epl.geometry.Point2D tangent0, com.epl.geometry.Point2D tangent1)
		{
			com.epl.geometry.Line lineCutter = new com.epl.geometry.Line();
			com.epl.geometry.Segment segmentCutter;
			int ivertexCuttee = cutEvents[icutEvent].m_ivertexCuttee;
			int ipartCutter = cutEvents[icutEvent].m_ipartCutter;
			int ivertexCutter = cutEvents[icutEvent].m_ivertexCutter;
			int ivertexCutteeNext = -1;
			int ipartCutterNext = -1;
			int ivertexCutterNext = -1;
			int countNext = -1;
			if (!bConsiderTouch && icutEvent < cutEvents.Count - 1)
			{
				com.epl.geometry.Cutter.CutEvent cutEvent = cutEvents[icutEvent + 1];
				ivertexCutteeNext = cutEvent.m_ivertexCuttee;
				ipartCutterNext = cutEvent.m_ipartCutter;
				ivertexCutterNext = cutEvent.m_ivertexCutter;
				countNext = cutEvent.m_count;
			}
			int ivertexCutteePrev = -1;
			int ipartCutterPrev = -1;
			int ivertexCutterPrev = -1;
			int countPrev = -1;
			if (icutEvent > 0)
			{
				com.epl.geometry.Cutter.CutEvent cutEvent = cutEvents[icutEvent - 1];
				ivertexCutteePrev = cutEvent.m_ivertexCuttee;
				ipartCutterPrev = cutEvent.m_ipartCutter;
				ivertexCutterPrev = cutEvent.m_ivertexCutter;
				countPrev = cutEvent.m_count;
			}
			int ivertexCutteePlus = shape.GetNextVertex(ivertexCuttee);
			int ivertexCutterMinus = shape.GetPrevVertex(ivertexCutter);
			if (!bConsiderTouch)
			{
				if ((icutEvent > 0 && ivertexCutteePrev == ivertexCuttee && ipartCutterPrev == ipartCutter && ivertexCutterPrev == ivertexCutterMinus && countPrev == 2) || (icutEvent < cutEvents.Count - 1 && ivertexCutteeNext == ivertexCutteePlus && ipartCutterNext == ipartCutter && ivertexCutterNext == ivertexCutterMinus
					 && countNext == 2))
				{
					segmentCutter = shape.GetSegment(ivertexCutter);
					if (segmentCutter == null)
					{
						shape.QueryLineConnector(ivertexCutter, lineCutter);
						segmentCutter = lineCutter;
					}
					tangent1.SetCoords(segmentCutter._getTangent(0.0));
					tangent0.Negate(tangent1);
					tangent1.Normalize();
					tangent0.Normalize();
					return false;
				}
				return true;
			}
			if (icutEvent == 0 || ivertexCutteePrev != ivertexCuttee || ipartCutterPrev != ipartCutter || ivertexCutterPrev != ivertexCutterMinus || countPrev == 2)
			{
				segmentCutter = shape.GetSegment(ivertexCutter);
				if (segmentCutter == null)
				{
					shape.QueryLineConnector(ivertexCutter, lineCutter);
					segmentCutter = lineCutter;
				}
				tangent1.SetCoords(segmentCutter._getTangent(0.0));
				tangent0.Negate(tangent1);
				tangent1.Normalize();
				tangent0.Normalize();
				return false;
			}
			// Already processed the event
			return true;
		}

		internal static bool _cutteeTangents(com.epl.geometry.EditShape shape, System.Collections.Generic.List<com.epl.geometry.Cutter.CutEvent> cutEvents, int icutEvent, int ipath, int ivertex, com.epl.geometry.Point2D tangent2, com.epl.geometry.Point2D tangent3)
		{
			com.epl.geometry.Line lineCuttee = new com.epl.geometry.Line();
			com.epl.geometry.Segment segmentCuttee = shape.GetSegment(ivertex);
			if (segmentCuttee == null)
			{
				shape.QueryLineConnector(ivertex, lineCuttee);
				segmentCuttee = lineCuttee;
			}
			com.epl.geometry.Cutter.CutEvent cutEvent = cutEvents[icutEvent];
			int ivertexCuttee = cutEvent.m_ivertexCuttee;
			double scalarCuttee = cutEvent.m_scalarCuttee0;
			int ivertexCutteePlus = shape.GetNextVertex(ivertexCuttee);
			if (scalarCuttee == 1.0)
			{
				tangent2.SetCoords(segmentCuttee._getTangent(1.0));
				if (ivertexCutteePlus != -1 && ivertexCutteePlus != shape.GetLastVertex(ipath))
				{
					segmentCuttee = shape.GetSegment(ivertexCutteePlus);
					if (segmentCuttee == null)
					{
						shape.QueryLineConnector(ivertexCutteePlus, lineCuttee);
						segmentCuttee = lineCuttee;
					}
					tangent3.SetCoords(segmentCuttee._getTangent(0.0));
					segmentCuttee = shape.GetSegment(ivertexCuttee);
					if (segmentCuttee == null)
					{
						shape.QueryLineConnector(ivertexCuttee, lineCuttee);
						segmentCuttee = lineCuttee;
					}
				}
				else
				{
					tangent3.SetCoords(tangent2);
				}
				tangent2.Negate();
				tangent3.Normalize();
				tangent2.Normalize();
				return false;
			}
			if (scalarCuttee == 0.0)
			{
				tangent3.SetCoords(segmentCuttee._getTangent(scalarCuttee));
				tangent2.Negate(tangent3);
				tangent3.Normalize();
				tangent2.Normalize();
				return false;
			}
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}
	}
}
