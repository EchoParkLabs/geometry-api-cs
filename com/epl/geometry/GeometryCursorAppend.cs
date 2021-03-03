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
	public class GeometryCursorAppend : com.epl.geometry.GeometryCursor
	{
		private com.epl.geometry.GeometryCursor m_cur1;

		private com.epl.geometry.GeometryCursor m_cur2;

		private com.epl.geometry.GeometryCursor m_cur;

		public GeometryCursorAppend(com.epl.geometry.GeometryCursor cur1, com.epl.geometry.GeometryCursor cur2)
		{
			m_cur1 = cur1;
			m_cur2 = cur2;
			m_cur = m_cur1;
		}

		public override com.epl.geometry.Geometry Next()
		{
			com.epl.geometry.Geometry g = m_cur.Next();
			if (g == null && m_cur != m_cur2)
			{
				m_cur = m_cur2;
				return m_cur.Next();
			}
			return g;
		}

		public override int GetGeometryID()
		{
			return m_cur.GetGeometryID();
		}
	}
}
