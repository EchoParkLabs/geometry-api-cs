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
	internal sealed class OperatorBoundaryLocalCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.ProgressTracker m_progress_tracker;

		internal bool m_b_done;

		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal int m_index;

		internal OperatorBoundaryLocalCursor(com.epl.geometry.GeometryCursor inputGeoms, com.epl.geometry.ProgressTracker tracker)
		{
			m_inputGeometryCursor = inputGeoms;
			m_progress_tracker = tracker;
			m_b_done = false;
			m_index = -1;
		}

		public override com.epl.geometry.Geometry Next()
		{
			if (!m_b_done)
			{
				com.epl.geometry.Geometry geometry = m_inputGeometryCursor.Next();
				if (geometry != null)
				{
					m_index = m_inputGeometryCursor.GetGeometryID();
					return Calculate_boundary(geometry, m_progress_tracker);
				}
				m_b_done = true;
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		internal static com.epl.geometry.Geometry Calculate_boundary(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Geometry res = com.epl.geometry.Boundary.Calculate(geom, progress_tracker);
			if (res == null)
			{
				return new com.epl.geometry.Point(geom.GetDescription());
			}
			else
			{
				// cannot return null
				return res;
			}
		}
	}
}
