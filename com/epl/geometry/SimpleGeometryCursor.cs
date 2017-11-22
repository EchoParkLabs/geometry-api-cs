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
using System.Linq;

namespace com.epl.geometry
{
	/// <summary>
	/// A simple GeometryCursor implementation that wraps a single Geometry or
	/// an array of Geometry classes
	/// </summary>
	public class SimpleGeometryCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.Geometry m_geom;

		internal System.Collections.Generic.IList<com.epl.geometry.Geometry> m_geomArray;

		internal int m_index;

		internal int m_count;

		public SimpleGeometryCursor(com.epl.geometry.Geometry geom)
		{
			m_geom = geom;
			m_index = -1;
			m_count = 1;
		}

		public SimpleGeometryCursor(com.epl.geometry.Geometry[] geoms)
		{
			m_geomArray = geoms.ToList();
			m_index = -1;
			m_count = geoms.Length;
		}

		public SimpleGeometryCursor(System.Collections.Generic.IList<com.epl.geometry.Geometry> geoms)
		{
			m_geomArray = geoms;
			m_index = -1;
			m_count = geoms.Count;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		public override com.epl.geometry.Geometry Next()
		{
			if (m_index < m_count - 1)
			{
				m_index++;
				return m_geom != null ? m_geom : m_geomArray[m_index];
			}
			return null;
		}
	}
}
