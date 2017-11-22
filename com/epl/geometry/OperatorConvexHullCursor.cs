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
	internal class OperatorConvexHullCursor : com.epl.geometry.GeometryCursor
	{
		private com.epl.geometry.ProgressTracker m_progress_tracker;

		private bool m_b_merge;

		private bool m_b_done;

		private com.epl.geometry.GeometryCursor m_inputGeometryCursor;

		private int m_index;

		internal com.epl.geometry.ConvexHull m_hull = new com.epl.geometry.ConvexHull();

		internal OperatorConvexHullCursor(bool b_merge, com.epl.geometry.GeometryCursor geoms, com.epl.geometry.ProgressTracker progress_tracker)
		{
			m_index = -1;
			if (geoms == null)
			{
				throw new System.ArgumentException();
			}
			m_b_merge = b_merge;
			m_b_done = false;
			m_inputGeometryCursor = geoms;
			m_progress_tracker = progress_tracker;
		}

		public override com.epl.geometry.Geometry Next()
		{
			if (m_b_merge)
			{
				if (!m_b_done)
				{
					com.epl.geometry.Geometry result = CalculateConvexHullMerging_(m_inputGeometryCursor, m_progress_tracker);
					m_b_done = true;
					return result;
				}
				return null;
			}
			if (!m_b_done)
			{
				com.epl.geometry.Geometry geometry = m_inputGeometryCursor.Next();
				if (geometry != null)
				{
					m_index = m_inputGeometryCursor.GetGeometryID();
					return CalculateConvexHull_(geometry, m_progress_tracker);
				}
				m_b_done = true;
			}
			return null;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		private com.epl.geometry.Geometry CalculateConvexHullMerging_(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Geometry geometry;
			while ((geometry = geoms.Next()) != null)
			{
				m_hull.AddGeometry(geometry);
			}
			return m_hull.GetBoundingGeometry();
		}

		public override bool Tock()
		{
			if (m_b_done)
			{
				return true;
			}
			if (!m_b_merge)
			{
				//Do not use tick/tock with the non-merging convex hull.
				//Call tick/next instead,
				//because tick pushes geometry into the cursor, and next performs a single convex hull on it. 
				throw new com.epl.geometry.GeometryException("Invalid call for non merging convex hull.");
			}
			com.epl.geometry.Geometry geometry = m_inputGeometryCursor.Next();
			if (geometry != null)
			{
				m_hull.AddGeometry(geometry);
				return true;
			}
			else
			{
				throw new com.epl.geometry.GeometryException("Expects a non-null geometry.");
			}
		}

		internal static com.epl.geometry.Geometry CalculateConvexHull_(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (IsConvex_(geom, progress_tracker))
			{
				return geom;
			}
			int type = geom.GetType().Value();
			if (com.epl.geometry.MultiPath.IsSegment(type))
			{
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(geom.GetDescription());
				polyline.AddSegment((com.epl.geometry.Segment)geom, true);
				return polyline;
			}
			if (type == com.epl.geometry.Geometry.GeometryType.MultiPoint)
			{
				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)geom;
				if (multi_point.GetPointCount() == 2)
				{
					com.epl.geometry.Point pt = new com.epl.geometry.Point();
					com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(geom.GetDescription());
					multi_point.GetPointByVal(0, pt);
					polyline.StartPath(pt);
					multi_point.GetPointByVal(1, pt);
					polyline.LineTo(pt);
					return polyline;
				}
			}
			com.epl.geometry.Polygon convex_hull = com.epl.geometry.ConvexHull.Construct((com.epl.geometry.MultiVertexGeometry)geom);
			return convex_hull;
		}

		internal static bool IsConvex_(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geom.IsEmpty())
			{
				return true;
			}
			// vacuously true
			int type = geom.GetType().Value();
			if (type == com.epl.geometry.Geometry.GeometryType.Point)
			{
				return true;
			}
			// vacuously true
			if (type == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				return true;
			}
			// always convex
			if (com.epl.geometry.MultiPath.IsSegment(type))
			{
				return false;
			}
			// upgrade to polyline
			if (type == com.epl.geometry.Geometry.GeometryType.MultiPoint)
			{
				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)geom;
				if (multi_point.GetPointCount() == 1)
				{
					return true;
				}
				// vacuously true
				return false;
			}
			// upgrade to polyline if point count is 2, otherwise
			// create convex hull
			if (type == com.epl.geometry.Geometry.GeometryType.Polyline)
			{
				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)geom;
				if (polyline.GetPathCount() == 1 && polyline.GetPointCount() <= 2)
				{
					return true;
				}
				// vacuously true
				return false;
			}
			// create convex hull
			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)geom;
			if (polygon.GetPathCount() != 1)
			{
				return false;
			}
			if (polygon.GetPointCount() <= 2)
			{
				return true;
			}
			// vacuously true
			return com.epl.geometry.ConvexHull.IsPathConvex(polygon, 0, progress_tracker);
		}
	}
}
