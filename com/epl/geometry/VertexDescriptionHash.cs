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
		internal System.Collections.Generic.Dictionary<int, System.WeakReference<com.epl.geometry.VertexDescription>> map = new System.Collections.Generic.Dictionary<int, System.WeakReference<com.epl.geometry.VertexDescription>>();

		private static com.epl.geometry.VertexDescription m_vd2D;

		private static com.epl.geometry.VertexDescription m_vd3D;

		private static readonly com.epl.geometry.VertexDescriptionHash INSTANCE = new com.epl.geometry.VertexDescriptionHash();

		private VertexDescriptionHash()
		{
			com.epl.geometry.VertexDescriptionDesignerImpl vdd2D = new com.epl.geometry.VertexDescriptionDesignerImpl();
			Add(vdd2D);
			com.epl.geometry.VertexDescriptionDesignerImpl vdd3D = new com.epl.geometry.VertexDescriptionDesignerImpl();
			vdd3D.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			Add(vdd3D);
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

		public com.epl.geometry.VertexDescription Add(com.epl.geometry.VertexDescriptionDesignerImpl vdd)
		{
			lock (this)
			{
				// Firstly quick test for 2D/3D descriptors.
				int h = vdd.GetHashCode();
				if ((m_vd2D != null) && m_vd2D.GetHashCode() == h)
				{
					if (vdd.IsDesignerFor(m_vd2D))
					{
						return m_vd2D;
					}
				}
				if ((m_vd3D != null) && (m_vd3D.GetHashCode() == h))
				{
					if (vdd.IsDesignerFor(m_vd3D))
					{
						return m_vd3D;
					}
				}
				// Now search in the hash.
				com.epl.geometry.VertexDescription vd = null;
				if (map.ContainsKey(h))
				{
					System.WeakReference<com.epl.geometry.VertexDescription> vdweak = map[h];
					if (!vdweak.TryGetTarget(out vd))
					{
						// GC'd VertexDescription
						map.Remove(h);
					}
				}
				if (vd == null)
				{
					// either not in map to begin with, or has been GC'd
					vd = vdd._createInternal();
					if (vd.GetAttributeCount() == 1)
					{
						m_vd2D = vd;
					}
					else
					{
						if ((vd.GetAttributeCount() == 2) && (vd.GetSemantics(1) == com.epl.geometry.VertexDescription.Semantics.Z))
						{
							m_vd3D = vd;
						}
						else
						{
							System.WeakReference<com.epl.geometry.VertexDescription> vdweak = new System.WeakReference<com.epl.geometry.VertexDescription>(vd);
							map[h] = vdweak;
						}
					}
				}
				return vd;
			}
		}
	}
}
