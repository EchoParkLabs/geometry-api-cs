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
	internal sealed class JsonValueReader : com.epl.geometry.JsonReader
	{
		private object m_object;

		private org.codehaus.jackson.JsonToken m_currentToken;

		private System.Collections.Generic.List<org.codehaus.jackson.JsonToken> m_parentStack;

		private System.Collections.Generic.List<com.epl.geometry.JSONObjectEnumerator> m_objIters;

		private System.Collections.Generic.List<com.epl.geometry.JSONArrayEnumerator> m_arrIters;

		internal JsonValueReader(object @object)
		{
			m_object = @object;
			bool bJSONObject = (m_object is org.json.JSONObject);
			bool bJSONArray = (m_object is org.json.JSONArray);
			if (!bJSONObject && !bJSONArray)
			{
				throw new System.ArgumentException();
			}
			m_parentStack = new System.Collections.Generic.List<org.codehaus.jackson.JsonToken>(0);
			m_objIters = new System.Collections.Generic.List<com.epl.geometry.JSONObjectEnumerator>(0);
			m_arrIters = new System.Collections.Generic.List<com.epl.geometry.JSONArrayEnumerator>(0);
			m_parentStack.Capacity = 4;
			m_objIters.Capacity = 4;
			m_arrIters.Capacity = 4;
			if (bJSONObject)
			{
				com.epl.geometry.JSONObjectEnumerator objIter = new com.epl.geometry.JSONObjectEnumerator((org.json.JSONObject)m_object);
				m_parentStack.Add(org.codehaus.jackson.JsonToken.START_OBJECT);
				m_objIters.Add(objIter);
				m_currentToken = org.codehaus.jackson.JsonToken.START_OBJECT;
			}
			else
			{
				com.epl.geometry.JSONArrayEnumerator arrIter = new com.epl.geometry.JSONArrayEnumerator((org.json.JSONArray)m_object);
				m_parentStack.Add(org.codehaus.jackson.JsonToken.START_ARRAY);
				m_arrIters.Add(arrIter);
				m_currentToken = org.codehaus.jackson.JsonToken.START_ARRAY;
			}
		}

		private void SetCurrentToken_(object obj)
		{
			if (obj is string)
			{
				m_currentToken = org.codehaus.jackson.JsonToken.VALUE_STRING;
			}
			else
			{
				if (obj is double || obj is float)
				{
					m_currentToken = org.codehaus.jackson.JsonToken.VALUE_NUMBER_FLOAT;
				}
				else
				{
					if (obj is int || obj is long || obj is short)
					{
						m_currentToken = org.codehaus.jackson.JsonToken.VALUE_NUMBER_INT;
					}
					else
					{
						if (obj is bool)
						{
							bool bObj = (bool)obj;
							bool b = bObj;
							if (b)
							{
								m_currentToken = org.codehaus.jackson.JsonToken.VALUE_TRUE;
							}
							else
							{
								m_currentToken = org.codehaus.jackson.JsonToken.VALUE_FALSE;
							}
						}
						else
						{
							if (obj is org.json.JSONObject)
							{
								m_currentToken = org.codehaus.jackson.JsonToken.START_OBJECT;
							}
							else
							{
								if (obj is org.json.JSONArray)
								{
									m_currentToken = org.codehaus.jackson.JsonToken.START_ARRAY;
								}
								else
								{
									m_currentToken = org.codehaus.jackson.JsonToken.VALUE_NULL;
								}
							}
						}
					}
				}
			}
		}

		internal object CurrentObject_()
		{
			System.Diagnostics.Debug.Assert((!m_parentStack.IsEmpty()));
			org.codehaus.jackson.JsonToken parentType = m_parentStack[m_parentStack.Count - 1];
			if (parentType == org.codehaus.jackson.JsonToken.START_OBJECT)
			{
				com.epl.geometry.JSONObjectEnumerator objIter = m_objIters[m_objIters.Count - 1];
				return objIter.GetCurrentObject();
			}
			com.epl.geometry.JSONArrayEnumerator arrIter = m_arrIters[m_arrIters.Count - 1];
			return arrIter.GetCurrentObject();
		}

		/// <exception cref="System.Exception"/>
		internal override org.codehaus.jackson.JsonToken NextToken()
		{
			if (m_parentStack.IsEmpty())
			{
				m_currentToken = org.codehaus.jackson.JsonToken.NOT_AVAILABLE;
				return m_currentToken;
			}
			org.codehaus.jackson.JsonToken parentType = m_parentStack[m_parentStack.Count - 1];
			if (parentType == org.codehaus.jackson.JsonToken.START_OBJECT)
			{
				com.epl.geometry.JSONObjectEnumerator iterator = m_objIters[m_objIters.Count - 1];
				if (m_currentToken == org.codehaus.jackson.JsonToken.FIELD_NAME)
				{
					object nextJSONValue = iterator.GetCurrentObject();
					if (nextJSONValue is org.json.JSONObject)
					{
						m_parentStack.Add(org.codehaus.jackson.JsonToken.START_OBJECT);
						m_objIters.Add(new com.epl.geometry.JSONObjectEnumerator((org.json.JSONObject)nextJSONValue));
						m_currentToken = org.codehaus.jackson.JsonToken.START_OBJECT;
					}
					else
					{
						if (nextJSONValue is org.json.JSONArray)
						{
							m_parentStack.Add(org.codehaus.jackson.JsonToken.START_ARRAY);
							m_arrIters.Add(new com.epl.geometry.JSONArrayEnumerator((org.json.JSONArray)nextJSONValue));
							m_currentToken = org.codehaus.jackson.JsonToken.START_ARRAY;
						}
						else
						{
							SetCurrentToken_(nextJSONValue);
						}
					}
				}
				else
				{
					if (iterator.Next())
					{
						m_currentToken = org.codehaus.jackson.JsonToken.FIELD_NAME;
					}
					else
					{
						m_objIters.Remove(m_objIters.Count - 1);
						m_parentStack.Remove(m_parentStack.Count - 1);
						m_currentToken = org.codehaus.jackson.JsonToken.END_OBJECT;
					}
				}
			}
			else
			{
				System.Diagnostics.Debug.Assert((parentType == org.codehaus.jackson.JsonToken.START_ARRAY));
				com.epl.geometry.JSONArrayEnumerator iterator = m_arrIters[m_arrIters.Count - 1];
				if (iterator.Next())
				{
					object nextJSONValue = iterator.GetCurrentObject();
					if (nextJSONValue is org.json.JSONObject)
					{
						m_parentStack.Add(org.codehaus.jackson.JsonToken.START_OBJECT);
						m_objIters.Add(new com.epl.geometry.JSONObjectEnumerator((org.json.JSONObject)nextJSONValue));
						m_currentToken = org.codehaus.jackson.JsonToken.START_OBJECT;
					}
					else
					{
						if (nextJSONValue is org.json.JSONArray)
						{
							m_parentStack.Add(org.codehaus.jackson.JsonToken.START_ARRAY);
							m_arrIters.Add(new com.epl.geometry.JSONArrayEnumerator((org.json.JSONArray)nextJSONValue));
							m_currentToken = org.codehaus.jackson.JsonToken.START_ARRAY;
						}
						else
						{
							SetCurrentToken_(nextJSONValue);
						}
					}
				}
				else
				{
					m_arrIters.Remove(m_arrIters.Count - 1);
					m_parentStack.Remove(m_parentStack.Count - 1);
					m_currentToken = org.codehaus.jackson.JsonToken.END_ARRAY;
				}
			}
			return m_currentToken;
		}

		/// <exception cref="System.Exception"/>
		internal override org.codehaus.jackson.JsonToken CurrentToken()
		{
			return m_currentToken;
		}

		/// <exception cref="System.Exception"/>
		internal override void SkipChildren()
		{
			System.Diagnostics.Debug.Assert((!m_parentStack.IsEmpty()));
			if (m_currentToken != org.codehaus.jackson.JsonToken.START_OBJECT && m_currentToken != org.codehaus.jackson.JsonToken.START_ARRAY)
			{
				return;
			}
			org.codehaus.jackson.JsonToken parentType = m_parentStack[m_parentStack.Count - 1];
			if (parentType == org.codehaus.jackson.JsonToken.START_OBJECT)
			{
				m_objIters.Remove(m_objIters.Count - 1);
				m_parentStack.Remove(m_parentStack.Count - 1);
				m_currentToken = org.codehaus.jackson.JsonToken.END_OBJECT;
			}
			else
			{
				m_arrIters.Remove(m_arrIters.Count - 1);
				m_parentStack.Remove(m_parentStack.Count - 1);
				m_currentToken = org.codehaus.jackson.JsonToken.END_ARRAY;
			}
		}

		/// <exception cref="System.Exception"/>
		internal override string CurrentString()
		{
			if (m_currentToken == org.codehaus.jackson.JsonToken.FIELD_NAME)
			{
				return m_objIters[m_objIters.Count - 1].GetCurrentKey();
			}
			if (m_currentToken != org.codehaus.jackson.JsonToken.VALUE_STRING)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return ((string)CurrentObject_()).ToString();
		}

		/// <exception cref="System.Exception"/>
		internal override double CurrentDoubleValue()
		{
			if (m_currentToken != org.codehaus.jackson.JsonToken.VALUE_NUMBER_FLOAT && m_currentToken != org.codehaus.jackson.JsonToken.VALUE_NUMBER_INT)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return ((java.lang.Number)CurrentObject_());
		}

		/// <exception cref="System.Exception"/>
		internal override int CurrentIntValue()
		{
			if (m_currentToken != org.codehaus.jackson.JsonToken.VALUE_NUMBER_INT)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			return ((java.lang.Number)CurrentObject_());
		}
	}
}
