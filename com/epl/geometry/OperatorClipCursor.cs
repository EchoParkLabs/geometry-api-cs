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
	internal class OperatorClipCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		internal com.epl.geometry.SpatialReferenceImpl m_spatialRefImpl;

		internal com.epl.geometry.Envelope2D m_envelope;

		internal double m_tolerance;

		internal int m_index;

		internal OperatorClipCursor(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.Envelope2D envelope, com.epl.geometry.SpatialReference spatial_ref, com.epl.geometry.ProgressTracker progress_tracker)
		{
			m_index = -1;
			if (geoms == null)
			{
				throw new System.ArgumentException();
			}
			m_envelope = envelope;
			m_inputGeometryCursor = geoms;
			m_spatialRefImpl = (com.epl.geometry.SpatialReferenceImpl)spatial_ref;
			m_tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(spatial_ref, envelope, false);
		}

		public override com.epl.geometry.Geometry Next()
		{
			com.epl.geometry.Geometry geometry;
			if ((geometry = m_inputGeometryCursor.Next()) != null)
			{
				m_index = m_inputGeometryCursor.GetGeometryID();
				return com.epl.geometry.Clipper.Clip(geometry, m_envelope, m_tolerance, 0.0);
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}
		// virtual bool IsRecycling() OVERRIDE { return false; }
	}
}
