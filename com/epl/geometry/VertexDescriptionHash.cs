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
	/// A hash object singleton that stores all VertexDescription instances via
	/// WeakReference.
	/// </summary>
	/// <remarks>
	/// A hash object singleton that stores all VertexDescription instances via
	/// WeakReference. The purpose of the class is to keep track of created
	/// VertexDescription instances to prevent duplicates.
	/// </remarks>
	internal sealed class VertexDescriptionHash
	{
		internal System.Collections.Generic.Dictionary<int, com.epl.geometry.VertexDescription> m_map = new System.Collections.Generic.Dictionary<int, com.epl.geometry.VertexDescription>();

		private static com.epl.geometry.VertexDescription m_vd2D = new com.epl.geometry.VertexDescription(1);

		private static com.epl.geometry.VertexDescription m_vd3D = new com.epl.geometry.VertexDescription(3);

		private static readonly com.epl.geometry.VertexDescriptionHash INSTANCE = new com.epl.geometry.VertexDescriptionHash();

		private VertexDescriptionHash()
		{
			m_map[1] = m_vd2D;
			m_map[3] = m_vd3D;
		}

		public static com.epl.geometry.VertexDescriptionHash GetInstance()
		{
			return INSTANCE;
		}

		public com.epl.geometry.VertexDescription GetVD2D()
		{
			return m_vd2D;
		}

		public com.epl.geometry.VertexDescription GetVD3D()
		{
			return m_vd3D;
		}

		public com.epl.geometry.VertexDescription FindOrAdd(int bitSet)
		{
			if (bitSet == 1)
			{
				return m_vd2D;
			}
			if (bitSet == 3)
			{
				return m_vd3D;
			}
			lock (this)
			{
				com.epl.geometry.VertexDescription vd = null;
				if (!m_map.TryGetValue(bitSet, out vd))
				{
					vd = new com.epl.geometry.VertexDescription(bitSet);
					m_map[bitSet] = vd;
				}
				return vd;
			}
		}
	}
}
