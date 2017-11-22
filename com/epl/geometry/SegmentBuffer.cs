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
	/// <summary>A helper class to provide reusable segment, line, etc instances.</summary>
	[System.Serializable]
	internal class SegmentBuffer
	{
		private const long serialVersionUID = 1L;

		internal com.epl.geometry.Line m_line;

		internal com.epl.geometry.Segment m_seg;

		public SegmentBuffer()
		{
			// PointerOf(Bezier) m_bez;
			m_line = null;
			m_seg = null;
		}

		public virtual com.epl.geometry.Segment Get()
		{
			return m_seg;
		}

		public virtual void Set(com.epl.geometry.Segment seg)
		{
			m_seg = seg;
			if (seg != null)
			{
				if (seg.GetType() == com.epl.geometry.Geometry.Type.Line)
				{
					com.epl.geometry.Line ln = (com.epl.geometry.Line)seg;
					m_line = ln;
				}
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
		}

		public virtual void Create(com.epl.geometry.Geometry.Type type)
		{
			if (type == com.epl.geometry.Geometry.Type.Line)
			{
				CreateLine();
			}
			else
			{
				throw new com.epl.geometry.GeometryException("not implemented");
			}
		}

		public virtual void CreateLine()
		{
			if (null == m_line)
			{
				m_line = new com.epl.geometry.Line();
			}
			m_seg = m_line;
		}
	}
}
