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
	internal class OperatorOffsetCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeoms;

		internal com.epl.geometry.SpatialReferenceImpl m_spatialReference;

		internal com.epl.geometry.ProgressTracker m_progressTracker;

		internal double m_distance;

		internal double m_miterLimit;

		internal com.epl.geometry.OperatorOffset.JoinType m_joins;

		internal double m_flattenError;

		internal int m_index;

		internal OperatorOffsetCursor(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, double distance, com.epl.geometry.OperatorOffset.JoinType joins, double bevelRatio, double flattenError, com.epl.geometry.ProgressTracker progressTracker)
		{
			m_index = -1;
			m_inputGeoms = inputGeometries;
			m_spatialReference = (com.epl.geometry.SpatialReferenceImpl)sr;
			m_distance = distance;
			m_joins = joins;
			m_miterLimit = bevelRatio;
			m_flattenError = flattenError;
			m_progressTracker = progressTracker;
		}

		public override com.epl.geometry.Geometry Next()
		{
			com.epl.geometry.Geometry geom = m_inputGeoms.Next();
			if (geom != null)
			{
				m_index = m_inputGeoms.GetGeometryID();
				return Offset(geom);
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		internal virtual com.epl.geometry.Geometry Offset(com.epl.geometry.Geometry geom)
		{
			double tolerance;
			if (m_flattenError <= 0)
			{
				tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(m_spatialReference, geom, false);
			}
			else
			{
				tolerance = m_flattenError;
			}
			return com.epl.geometry.ConstructOffset.Execute(geom, m_distance, m_joins, m_miterLimit, tolerance, m_progressTracker);
		}
	}
}
