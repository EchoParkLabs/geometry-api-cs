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


namespace com.epl.geometry
{
	internal class OperatorCutCursor : com.epl.geometry.GeometryCursor
	{
		internal bool m_bConsiderTouch;

		internal com.epl.geometry.Geometry m_cuttee;

		internal com.epl.geometry.Polyline m_cutter;

		internal double m_tolerance;

		internal com.epl.geometry.ProgressTracker m_progressTracker;

		internal int m_cutIndex;

		internal System.Collections.Generic.List<com.epl.geometry.MultiPath> m_cuts = null;

		internal OperatorCutCursor(bool bConsiderTouch, com.epl.geometry.Geometry cuttee, com.epl.geometry.Polyline cutter, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.ProgressTracker progressTracker)
		{
			if (cuttee == null || cutter == null)
			{
				throw new com.epl.geometry.GeometryException("invalid argument");
			}
			m_bConsiderTouch = bConsiderTouch;
			m_cuttee = cuttee;
			m_cutter = cutter;
			com.epl.geometry.Envelope2D e = com.epl.geometry.InternalUtils.GetMergedExtent(cuttee, cutter);
			m_tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatialReference, e, true);
			m_cutIndex = -1;
			m_progressTracker = progressTracker;
		}

		public override int GetGeometryID()
		{
			return 0;
		}

		public override com.epl.geometry.Geometry Next()
		{
			GenerateCuts_();
			if (++m_cutIndex < m_cuts.Count)
			{
				return (com.epl.geometry.Geometry)m_cuts[m_cutIndex];
			}
			return null;
		}

		private void GenerateCuts_()
		{
			if (m_cuts != null)
			{
				return;
			}
			m_cuts = new System.Collections.Generic.List<com.epl.geometry.MultiPath>();
			com.epl.geometry.Geometry.Type type = m_cuttee.GetType();
			switch (type.Value())
			{
				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					Generate_polyline_cuts_();
					break;
				}

				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					Generate_polygon_cuts_();
					break;
				}

				default:
				{
					break;
				}
			}
		}

		// warning fix
		private void Generate_polyline_cuts_()
		{
			com.epl.geometry.MultiPath left = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPath right = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPath uncut = new com.epl.geometry.Polyline();
			m_cuts.Add(left);
			m_cuts.Add(right);
			System.Collections.Generic.List<com.epl.geometry.OperatorCutLocal.CutPair> cutPairs = new System.Collections.Generic.List<com.epl.geometry.OperatorCutLocal.CutPair>(0);
			com.epl.geometry.Cutter.CutPolyline(m_bConsiderTouch, (com.epl.geometry.Polyline)m_cuttee, m_cutter, m_tolerance, cutPairs, null, m_progressTracker);
			for (int icut = 0; icut < cutPairs.Count; icut++)
			{
				com.epl.geometry.OperatorCutLocal.CutPair cutPair = cutPairs[icut];
				if (cutPair.m_side == com.epl.geometry.OperatorCutLocal.Side.Left)
				{
					left.Add((com.epl.geometry.MultiPath)cutPair.m_geometry, false);
				}
				else
				{
					if (cutPair.m_side == com.epl.geometry.OperatorCutLocal.Side.Right || cutPair.m_side == com.epl.geometry.OperatorCutLocal.Side.Coincident)
					{
						right.Add((com.epl.geometry.MultiPath)cutPair.m_geometry, false);
					}
					else
					{
						if (cutPair.m_side == com.epl.geometry.OperatorCutLocal.Side.Undefined)
						{
							m_cuts.Add((com.epl.geometry.MultiPath)cutPair.m_geometry);
						}
						else
						{
							uncut.Add((com.epl.geometry.MultiPath)cutPair.m_geometry, false);
						}
					}
				}
			}
			if (!uncut.IsEmpty() && (!left.IsEmpty() || !right.IsEmpty() || m_cuts.Count >= 3))
			{
				m_cuts.Add(uncut);
			}
			if (left.IsEmpty() && right.IsEmpty() && m_cuts.Count < 3)
			{
				m_cuts.Clear();
			}
		}

		// no cuts
		private void Generate_polygon_cuts_()
		{
			com.epl.geometry.AttributeStreamOfInt32 cutHandles = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.EditShape shape = new com.epl.geometry.EditShape();
			int sideIndex = shape.CreateGeometryUserIndex();
			int cutteeHandle = shape.AddGeometry(m_cuttee);
			int cutterHandle = shape.AddGeometry(m_cutter);
			com.epl.geometry.TopologicalOperations topoOp = new com.epl.geometry.TopologicalOperations();
			try
			{
				topoOp.SetEditShapeCrackAndCluster(shape, m_tolerance, m_progressTracker);
				topoOp.Cut(sideIndex, cutteeHandle, cutterHandle, cutHandles);
				com.epl.geometry.Polygon cutteeRemainder = (com.epl.geometry.Polygon)shape.GetGeometry(cutteeHandle);
				com.epl.geometry.MultiPath left = new com.epl.geometry.Polygon();
				com.epl.geometry.MultiPath right = new com.epl.geometry.Polygon();
				m_cuts.Clear();
				m_cuts.Add(left);
				m_cuts.Add(right);
				for (int icutIndex = 0; icutIndex < cutHandles.Size(); icutIndex++)
				{
					com.epl.geometry.Geometry cutGeometry;
					{
						// intersection
						com.epl.geometry.EditShape shapeIntersect = new com.epl.geometry.EditShape();
						int geometryA = shapeIntersect.AddGeometry(cutteeRemainder);
						int geometryB = shapeIntersect.AddGeometry(shape.GetGeometry(cutHandles.Get(icutIndex)));
						topoOp.SetEditShape(shapeIntersect, m_progressTracker);
						int intersectHandle = topoOp.Intersection(geometryA, geometryB);
						cutGeometry = shapeIntersect.GetGeometry(intersectHandle);
						if (cutGeometry.IsEmpty())
						{
							continue;
						}
						int side = shape.GetGeometryUserIndex(cutHandles.Get(icutIndex), sideIndex);
						if (side == 2)
						{
							left.Add((com.epl.geometry.MultiPath)cutGeometry, false);
						}
						else
						{
							if (side == 1)
							{
								right.Add((com.epl.geometry.MultiPath)cutGeometry, false);
							}
							else
							{
								m_cuts.Add((com.epl.geometry.MultiPath)cutGeometry);
							}
						}
					}
					{
						// Undefined
						// difference
						com.epl.geometry.EditShape shapeDifference = new com.epl.geometry.EditShape();
						int geometryA = shapeDifference.AddGeometry(cutteeRemainder);
						int geometryB = shapeDifference.AddGeometry(shape.GetGeometry(cutHandles.Get(icutIndex)));
						topoOp.SetEditShape(shapeDifference, m_progressTracker);
						cutteeRemainder = (com.epl.geometry.Polygon)shapeDifference.GetGeometry(topoOp.Difference(geometryA, geometryB));
					}
				}
				if (!cutteeRemainder.IsEmpty() && cutHandles.Size() > 0)
				{
					m_cuts.Add((com.epl.geometry.MultiPath)cutteeRemainder);
				}
				if (left.IsEmpty() && right.IsEmpty())
				{
					m_cuts.Clear();
				}
			}
			finally
			{
				// no cuts
				topoOp.RemoveShape();
			}
		}
	}
}
