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
	internal class SimpleByteBufferCursor : com.epl.geometry.ByteBufferCursor
	{
		internal System.IO.MemoryStream m_byteBuffer;

		internal int m_index;

		internal int m_count;

		public SimpleByteBufferCursor(System.IO.MemoryStream byteBuffer)
		{
			m_byteBuffer = byteBuffer;
			m_index = -1;
			m_count = 1;
		}

		public override int GetByteBufferID()
		{
			return m_index;
		}

		public override System.IO.MemoryStream Next()
		{
			if (m_index < m_count - 1)
			{
				m_index++;
				return m_byteBuffer;
			}
			return null;
		}
	}
}
