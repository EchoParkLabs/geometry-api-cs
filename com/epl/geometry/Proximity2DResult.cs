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
	/// <summary>
	/// Proximity operators are used to find the distance between two geometries or
	/// the distance from a given point to the nearest point on another geometry.
	/// </summary>
	public class Proximity2DResult
	{
		internal com.epl.geometry.Point2D m_coordinate = new com.epl.geometry.Point2D();

		internal int m_vertexIndex;

		internal double m_distance;

		internal int m_info;

		/// <summary>Sets the right_side info to true or false.</summary>
		/// <param name="bRight">
		/// Whether the nearest coordinate is to the right or left of the
		/// geometry.
		/// </param>
		public virtual void SetRightSide(bool bRight)
		{
			if (bRight)
			{
				m_info |= (int)com.epl.geometry.OperatorProximity2D.ProxResultInfo.rightSide;
			}
			else
			{
				m_info &= ~(int)com.epl.geometry.OperatorProximity2D.ProxResultInfo.rightSide;
			}
		}

		/// <summary>Returns TRUE if the Proximity2DResult is empty.</summary>
		/// <remarks>
		/// Returns TRUE if the Proximity2DResult is empty. This only happens if the
		/// Geometry passed to the Proximity operator is empty.
		/// </remarks>
		public virtual bool IsEmpty()
		{
			return m_vertexIndex < 0;
		}

		/// <summary>
		/// Returns the closest coordinate for
		/// OperatorProximity2D.getNearestCoordinate or the vertex coordinates for
		/// the OperatorProximity2D.getNearestVertex and
		/// OperatorProximity2D.getNearestVertices.
		/// </summary>
		public virtual com.epl.geometry.Point GetCoordinate()
		{
			if (IsEmpty())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return new com.epl.geometry.Point(m_coordinate.x, m_coordinate.y);
		}

		/// <summary>Returns the vertex index.</summary>
		/// <remarks>
		/// Returns the vertex index. For OperatorProximity2D.getNearestCoordinate
		/// the behavior is: When the input is a polygon or an envelope and the
		/// bTestPolygonInterior is true, the value is zero. When the input is a
		/// polygon or an Envelope and the bTestPolygonInterior is false, the value
		/// is the start vertex index of a segment with the closest coordinate. When
		/// the input is a polyline, the value is the start vertex index of a segment
		/// with the closest coordinate. When the input is a point, the value is 0.
		/// When the input is a multipoint, the value is the closest vertex.
		/// </remarks>
		public virtual int GetVertexIndex()
		{
			if (IsEmpty())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return m_vertexIndex;
		}

		/// <summary>Returns the distance to the closest vertex or coordinate.</summary>
		public virtual double GetDistance()
		{
			if (IsEmpty())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return m_distance;
		}

		/// <summary>Returns true if the closest coordinate is to the right of the MultiPath.</summary>
		public virtual bool IsRightSide()
		{
			return (m_info & (int)com.epl.geometry.OperatorProximity2D.ProxResultInfo.rightSide) != 0;
		}

		internal virtual void _setParams(double x, double y, int vertexIndex, double distance)
		{
			m_coordinate.x = x;
			m_coordinate.y = y;
			m_vertexIndex = vertexIndex;
			m_distance = distance;
		}

		internal Proximity2DResult()
		{
			// static int _compare(Proximity2DResult v1, Proximity2DResult v2)
			// {
			// if (v1.m_distance < v2.m_distance)
			// return -1;
			// if (v1.m_distance == v2.m_distance)
			// return 0;
			//
			// return 1;
			// }
			m_vertexIndex = -1;
		}

		internal Proximity2DResult(com.epl.geometry.Point2D coordinate, int vertexIndex, double distance)
		{
			m_coordinate.SetCoords(coordinate);
			m_vertexIndex = vertexIndex;
			m_distance = distance;
			m_info = 0;
		}
	}
}
