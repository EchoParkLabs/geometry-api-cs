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
/*
* To change this template, choose Tools | Templates
* and open the template in the editor.
*/


namespace com.epl.geometry
{
	internal class SweepMonkierComparator : com.epl.geometry.Treap.MonikerComparator
	{
		protected internal com.epl.geometry.EditShape m_shape;

		protected internal bool m_b_intersection_detected;

		protected internal com.epl.geometry.Point2D m_point_of_interest;

		protected internal com.epl.geometry.Line m_line_1;

		protected internal com.epl.geometry.Envelope1D m_env;

		protected internal int m_vertex_1;

		protected internal int m_current_node;

		protected internal double m_min_dist;

		protected internal double m_tolerance;

		internal SweepMonkierComparator(com.epl.geometry.EditShape shape, double tol)
		{
			m_shape = shape;
			m_tolerance = tol;
			m_b_intersection_detected = false;
			m_vertex_1 = -1;
			m_env = new com.epl.geometry.Envelope1D();
			m_point_of_interest = new com.epl.geometry.Point2D();
			m_point_of_interest.SetNaN();
			m_line_1 = new com.epl.geometry.Line();
			m_current_node = -1;
			m_min_dist = com.epl.geometry.NumberUtils.DoubleMax();
		}

		internal virtual int GetCurrentNode()
		{
			return m_current_node;
		}

		// Makes the comparator to forget about the last detected intersection.
		// Need to be called after the intersection has been resolved.
		internal virtual void ClearIntersectionDetectedFlag()
		{
			m_b_intersection_detected = false;
			m_min_dist = com.epl.geometry.NumberUtils.DoubleMax();
		}

		// Returns True if there has been intersection detected during compare call.
		// Once intersection is detected subsequent calls to compare method do
		// nothing until clear_intersection_detected_flag is called.
		internal virtual bool IntersectionDetected()
		{
			return m_b_intersection_detected;
		}

		internal virtual void SetPoint(com.epl.geometry.Point2D pt)
		{
			m_point_of_interest.SetCoords(pt);
		}

		// Compares the moniker, contained in the Moniker_comparator with the
		// element contained in the given node.
		internal override int Compare(com.epl.geometry.Treap treap, int node)
		{
			int vertex = treap.GetElement(node);
			return CompareVertex_(treap, node, vertex);
		}

		protected internal virtual int CompareVertex_(com.epl.geometry.Treap treap, int node, int vertex)
		{
			bool bCurve = m_shape.GetSegment(vertex) != null;
			if (!bCurve)
			{
				m_shape.QueryLineConnector(vertex, m_line_1);
				m_env.SetCoordsNoNaN_(m_line_1.GetStartX(), m_line_1.GetEndX());
			}
			if (bCurve)
			{
				throw new com.epl.geometry.GeometryException("not implemented");
			}
			if (m_point_of_interest.x + m_tolerance < m_env.vmin)
			{
				return -1;
			}
			if (m_point_of_interest.x - m_tolerance > m_env.vmax)
			{
				return 1;
			}
			if (m_line_1.GetStartY() == m_line_1.GetEndY())
			{
				m_current_node = node;
				m_b_intersection_detected = true;
				return 0;
			}
			m_line_1.OrientBottomUp_();
			com.epl.geometry.Point2D start = m_line_1.GetStartXY();
			com.epl.geometry.Point2D vector = new com.epl.geometry.Point2D();
			vector.Sub(m_line_1.GetEndXY(), start);
			vector.RightPerpendicular();
			com.epl.geometry.Point2D v_2 = new com.epl.geometry.Point2D();
			v_2.Sub(m_point_of_interest, start);
			double dot = vector.DotProduct(v_2);
			dot /= vector.Length();
			if (dot < -m_tolerance * 10)
			{
				return -1;
			}
			if (dot > m_tolerance * 10)
			{
				return 1;
			}
			if (m_line_1.IsIntersecting(m_point_of_interest, m_tolerance))
			{
				double absDot = System.Math.Abs(dot);
				if (absDot < m_min_dist)
				{
					m_current_node = node;
					m_min_dist = absDot;
				}
				m_b_intersection_detected = true;
				if (absDot < 0.25 * m_tolerance)
				{
					return 0;
				}
			}
			return dot < 0 ? -1 : 1;
		}
	}
}
