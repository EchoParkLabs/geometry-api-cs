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
	internal class SimpleJsonReaderCursor : com.epl.geometry.JsonReaderCursor
	{
		internal com.epl.geometry.JsonReader m_jsonParser;

		internal com.epl.geometry.JsonReader[] m_jsonParserArray;

		internal int m_index;

		internal int m_count;

		public SimpleJsonReaderCursor(com.epl.geometry.JsonReader jsonString)
		{
			m_jsonParser = jsonString;
			m_index = -1;
			m_count = 1;
		}

		public SimpleJsonReaderCursor(com.epl.geometry.JsonReader[] jsonStringArray)
		{
			m_jsonParserArray = jsonStringArray;
			m_index = -1;
			m_count = jsonStringArray.Length;
		}

		public override int GetID()
		{
			return m_index;
		}

		public override com.epl.geometry.JsonReader Next()
		{
			if (m_index < m_count - 1)
			{
				m_index++;
				return m_jsonParser != null ? m_jsonParser : m_jsonParserArray[m_index];
			}
			return null;
		}
	}
}
