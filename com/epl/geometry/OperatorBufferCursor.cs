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
	internal class OperatorBufferCursor : com.epl.geometry.GeometryCursor
	{
		private com.epl.geometry.GeometryCursor m_inputGeoms;

		private com.epl.geometry.SpatialReferenceImpl m_Spatial_reference;

		private com.epl.geometry.ProgressTracker m_progress_tracker;

		private double[] m_distances;

		private com.epl.geometry.Envelope2D m_currentUnionEnvelope2D;

		private bool m_bUnion;

		private int m_index;

		private int m_dindex;

		internal OperatorBufferCursor(com.epl.geometry.GeometryCursor inputGeoms, com.epl.geometry.SpatialReference sr, double[] distances, bool b_union, com.epl.geometry.ProgressTracker progress_tracker)
		{
			m_index = -1;
			m_inputGeoms = inputGeoms;
			m_Spatial_reference = (com.epl.geometry.SpatialReferenceImpl)(sr);
			m_distances = distances;
			m_bUnion = b_union;
			m_currentUnionEnvelope2D = new com.epl.geometry.Envelope2D();
			m_currentUnionEnvelope2D.SetEmpty();
			m_dindex = -1;
			m_progress_tracker = progress_tracker;
		}

		public override com.epl.geometry.Geometry Next()
		{
			{
				com.epl.geometry.Geometry geom;
				while ((geom = m_inputGeoms.Next()) != null)
				{
					m_index = m_inputGeoms.GetGeometryID();
					if (m_dindex + 1 < m_distances.Length)
					{
						m_dindex++;
					}
					return Buffer(geom, m_distances[m_dindex]);
				}
				return null;
			}
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		// virtual bool IsRecycling() OVERRIDE { return false; }
		internal virtual com.epl.geometry.Geometry Buffer(com.epl.geometry.Geometry geom, double distance)
		{
			return com.epl.geometry.Bufferer.Buffer(geom, distance, m_Spatial_reference, com.epl.geometry.NumberUtils.TheNaN, 96, m_progress_tracker);
		}
	}
}
