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
	internal class SimpleJsonCursor : com.epl.geometry.JsonCursor
	{
		internal string m_jsonString;

		internal string[] m_jsonStringArray;

		internal int m_index;

		internal int m_count;

		public SimpleJsonCursor(string jsonString)
		{
			m_jsonString = jsonString;
			m_index = -1;
			m_count = 1;
		}

		public SimpleJsonCursor(string[] jsonStringArray)
		{
			m_jsonStringArray = jsonStringArray;
			m_index = -1;
			m_count = jsonStringArray.Length;
		}

		public override int GetID()
		{
			return m_index;
		}

		public override string Next()
		{
			if (m_index < m_count - 1)
			{
				m_index++;
				return m_jsonString != null ? m_jsonString : m_jsonStringArray[m_index];
			}
			return null;
		}
	}
}
