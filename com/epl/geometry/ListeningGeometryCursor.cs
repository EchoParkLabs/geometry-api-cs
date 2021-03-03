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
using System.Linq;

namespace com.epl.geometry
{
	/// <summary>A GeometryCursor implementation that allows pushing geometries into it.</summary>
	/// <remarks>
	/// A GeometryCursor implementation that allows pushing geometries into it.
	/// To be used with aggregating operations, OperatorUnion and OperatorConvexHull,
	/// when the geometries are not available at the time of the execute method call,
	/// but are coming in a stream.
	/// </remarks>
	public sealed class ListeningGeometryCursor : com.epl.geometry.GeometryCursor
	{
		internal System.Collections.Generic.LinkedList<com.epl.geometry.Geometry> m_geomList = new System.Collections.Generic.LinkedList<com.epl.geometry.Geometry>();

		internal int m_index = -1;

		public ListeningGeometryCursor()
		{
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		public override com.epl.geometry.Geometry Next()
		{
			if (m_geomList != null && !(m_geomList.Count == 0))
			{
				m_index++;
				com.epl.geometry.Geometry data = m_geomList.FirstOrDefault();
               m_geomList.RemoveFirst();
               return data;
			}
			m_geomList = null;
			//prevent the class from being used again
			return null;
		}

		/// <summary>Call this method to add geometry to the cursor.</summary>
		/// <remarks>
		/// Call this method to add geometry to the cursor. After this method is
		/// called, call immediately the tock() method on the GeometryCursor returned
		/// by the OperatorUnion (or OperatorConvexHull with b_merge == true). Call
		/// next() on the GeometryCursor returned by the OperatorUnion when done
		/// listening to incoming geometries to finish the union operation.
		/// </remarks>
		/// <param name="geom">The geometry to be pushed into the cursor.</param>
		public void Tick(com.epl.geometry.Geometry geom)
		{
			m_geomList.AddLast(geom);
		}
	}
}
