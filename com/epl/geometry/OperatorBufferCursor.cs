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
	internal class OperatorBufferCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.Bufferer m_bufferer = new com.epl.geometry.Bufferer();

		private com.epl.geometry.GeometryCursor m_inputGeoms;

		private com.epl.geometry.SpatialReferenceImpl m_Spatial_reference;

		private com.epl.geometry.ProgressTracker m_progress_tracker;

		private double[] m_distances;

		private com.epl.geometry.Envelope2D m_currentUnionEnvelope2D;

		internal double m_max_deviation;

		internal int m_max_vertices_in_full_circle;

		private int m_index;

		private int m_dindex;

		internal OperatorBufferCursor(com.epl.geometry.GeometryCursor inputGeoms, com.epl.geometry.SpatialReference sr, double[] distances, double max_deviation, int max_vertices, bool b_union, com.epl.geometry.ProgressTracker progress_tracker)
		{
			m_index = -1;
			m_inputGeoms = inputGeoms;
			m_max_deviation = max_deviation;
			m_max_vertices_in_full_circle = max_vertices;
			m_Spatial_reference = (com.epl.geometry.SpatialReferenceImpl)(sr);
			m_distances = distances;
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
			return m_bufferer.Buffer(geom, distance, m_Spatial_reference, m_max_deviation, m_max_vertices_in_full_circle, m_progress_tracker);
		}
	}
}
