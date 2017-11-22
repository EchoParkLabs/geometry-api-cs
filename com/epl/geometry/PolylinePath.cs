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
	internal class PolylinePath
	{
		internal com.epl.geometry.Point2D m_fromPoint;

		internal com.epl.geometry.Point2D m_toPoint;

		internal double m_fromDist;

		internal double m_toDist;

		internal int m_path;

		internal bool m_used;

		public PolylinePath()
		{
		}

		public PolylinePath(com.epl.geometry.Point2D fromPoint, com.epl.geometry.Point2D toPoint, double fromDist, double toDist, int path)
		{
			// from lower left corner; -1.0 if point is not on
			// clipping bounday
			// from lower left corner; -1.0 if point is not on clipping
			// bounday
			// from polyline
			m_fromPoint = fromPoint;
			m_toPoint = toPoint;
			m_fromDist = fromDist;
			m_toDist = toDist;
			m_path = path;
			m_used = false;
		}

		internal virtual void SetValues(com.epl.geometry.Point2D fromPoint, com.epl.geometry.Point2D toPoint, double fromDist, double toDist, int path)
		{
			m_fromPoint = fromPoint;
			m_toPoint = toPoint;
			m_fromDist = fromDist;
			m_toDist = toDist;
			m_path = path;
			m_used = false;
		}
		// to be used in Use SORTARRAY
	}

	internal class PolylinePathComparator : System.Collections.Generic.IComparer<com.epl.geometry.PolylinePath>
	{
		public virtual int Compare(com.epl.geometry.PolylinePath v1, com.epl.geometry.PolylinePath v2)
		{
			if ((v1).m_fromDist < (v2).m_fromDist)
			{
				return -1;
			}
			else
			{
				if ((v1).m_fromDist > (v2).m_fromDist)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
