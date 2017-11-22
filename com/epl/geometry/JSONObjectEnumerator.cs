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
	internal sealed class JSONObjectEnumerator
	{
		private org.json.JSONObject m_jsonObject;

		private bool m_bStarted;

		private int m_currentIndex;

		private string[] m_keys;

		internal JSONObjectEnumerator(org.json.JSONObject jsonObject)
		{
			m_bStarted = false;
			m_currentIndex = -1;
			m_jsonObject = jsonObject;
		}

		internal string GetCurrentKey()
		{
			if (!m_bStarted)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (m_currentIndex == m_jsonObject.Length())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return m_keys[m_currentIndex];
		}

		internal object GetCurrentObject()
		{
			if (!m_bStarted)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			if (m_currentIndex == m_jsonObject.Length())
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return m_jsonObject.Opt(m_keys[m_currentIndex]);
		}

		internal bool Next()
		{
			if (!m_bStarted)
			{
				m_currentIndex = 0;
				m_keys = org.json.JSONObject.GetNames(m_jsonObject);
				m_bStarted = true;
			}
			else
			{
				if (m_currentIndex != m_jsonObject.Length())
				{
					m_currentIndex++;
				}
			}
			return m_currentIndex != m_jsonObject.Length();
		}
	}
}
