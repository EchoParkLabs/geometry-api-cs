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
	internal class OperatorSimplifyCursorOGC : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal com.epl.geometry.SpatialReference m_spatialReference;

		internal com.epl.geometry.ProgressTracker m_progressTracker;

		internal int m_index;

		internal bool m_bForceSimplify;

		internal OperatorSimplifyCursorOGC(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.SpatialReference spatialRef, bool bForceSimplify, com.epl.geometry.ProgressTracker progressTracker)
		{
			m_progressTracker = progressTracker;
			m_bForceSimplify = bForceSimplify;
			m_index = -1;
			if (geoms == null)
			{
				throw new System.ArgumentException();
			}
			m_inputGeometryCursor = geoms;
			m_spatialReference = spatialRef;
		}

		public override com.epl.geometry.Geometry Next()
		{
			com.epl.geometry.Geometry geometry;
			if ((geometry = m_inputGeometryCursor.Next()) != null)
			{
				// if (geometry =
				// m_inputGeometryCursor->Next())
				m_index = m_inputGeometryCursor.GetGeometryID();
				if ((m_progressTracker != null) && !(m_progressTracker.Progress(-1, -1)))
				{
					throw new System.Exception("user_canceled");
				}
				return Simplify(geometry);
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		internal virtual com.epl.geometry.Geometry Simplify(com.epl.geometry.Geometry geometry)
		{
			if (geometry == null)
			{
				throw new System.ArgumentException();
			}
			return com.epl.geometry.OperatorSimplifyLocalHelper.SimplifyOGC(geometry, m_spatialReference, m_bForceSimplify, m_progressTracker);
		}
	}
}
